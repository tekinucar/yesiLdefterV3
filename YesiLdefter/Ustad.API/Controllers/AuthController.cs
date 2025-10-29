using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ustad.API.Controllers
{
    [ApiController]
    [Route("auth")] 
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class LoginRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string TurnstileToken { get; set; }
        }

        public class LoginResponse
        {
            public string Token { get; set; }
            public int OperatorId { get; set; }
            public string UserGUID { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public int FirmId { get; set; }
        }

        public class RegisterRequest
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string Password { get; set; }
            public int? FirmId { get; set; }
            public string Role { get; set; }
            public string TurnstileToken { get; set; }
        }

        public class ResetStartRequest { public string UserName { get; set; } }
        public class ResetConfirmRequest { public string Token { get; set; } public string NewPassword { get; set; } }

        private string BuildConnectionString()
        {
            string host = Environment.GetEnvironmentVariable("DB_HOST") ?? _configuration["Db:Host"]; // e.g. 46.101.255.224
            string port = Environment.GetEnvironmentVariable("DB_PORT") ?? _configuration["Db:Port"] ?? "1433";
            string user = Environment.GetEnvironmentVariable("DB_USER") ?? _configuration["Db:User"];
            string pass = Environment.GetEnvironmentVariable("DB_PASS") ?? _configuration["Db:Pass"];
            string db   = Environment.GetEnvironmentVariable("DB_NAME") ?? _configuration["Db:Name"];
            return !string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(db)
                ? $"Data Source={host},{port}; Initial Catalog={db}; User ID={user}; Password={pass}; TrustServerCertificate=true; Encrypt=false; MultipleActiveResultSets=True"
                : _configuration.GetConnectionString("BulutCrm"); // Use BulutCrm which has UstadUsers table
        }

        private async Task<bool> ValidateTurnstileAsync(string token)
        {
            if (string.IsNullOrEmpty(token)) return true; // not required everywhere
            var turnstileSecret = Environment.GetEnvironmentVariable("TURNSTILE_SECRET") ?? _configuration["Turnstile:Secret"];
            if (string.IsNullOrEmpty(turnstileSecret)) return true; // disabled
            try
            {
                using var cf = new HttpClient();
                var verify = await cf.PostAsync(
                    "https://challenges.cloudflare.com/turnstile/v0/siteverify",
                    new FormUrlEncodedContent(new[]
                    {
                        new System.Collections.Generic.KeyValuePair<string,string>("secret", turnstileSecret),
                        new System.Collections.Generic.KeyValuePair<string,string>("response", token)
                    }));
                if (!verify.IsSuccessStatusCode) return false;
                var obj = await verify.Content.ReadFromJsonAsync<dynamic>();
                return obj != null && obj.success == true;
            }
            catch { return false; }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.UserName) || string.IsNullOrWhiteSpace(request?.Password))
                return BadRequest("Kullanıcı adı ve şifre zorunludur.");

            // Optional Cloudflare Turnstile verification
            if (!await ValidateTurnstileAsync(request.TurnstileToken))
                return Unauthorized("Turnstile doğrulaması başarısız.");

            // TEST MODE: If database is not accessible, use mock authentication
            if (await IsDatabaseAccessible() == false)
            {
                return await LoginTestMode(request);
            }

            // Get DB connection string from environment variables if provided
            string connStr = BuildConnectionString();

            // Find user in UstadUsers table with secure password extension
            int userId = 0; string fullName = ""; string role = "Agent"; string userKeyPlain = null;
            string firmGuid = ""; string userGuid = ""; bool isActive = false;
            byte[] hash = null; byte[] salt = null; int iterations = 0;

            using (var con = new SqlConnection(connStr))
            {
                await con.OpenAsync();
                
                // Create secure password extension table if it doesn't exist
                using (var createCmd = con.CreateCommand())
                {
                    createCmd.CommandText = @"
IF OBJECT_ID('UstadUserSecurePasswords') IS NULL
BEGIN
    CREATE TABLE UstadUserSecurePasswords (
        UserId INT PRIMARY KEY,
        PasswordHash VARBINARY(512) NOT NULL,
        Salt VARBINARY(128) NOT NULL,
        Iterations INT NOT NULL,
        CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
        FOREIGN KEY (UserId) REFERENCES UstadUsers(UserId)
    );
    CREATE INDEX IX_UstadUserSecurePasswords_UserId ON UstadUserSecurePasswords(UserId);
END";
                    await createCmd.ExecuteNonQueryAsync();
                }

                // Query user with optional secure password
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT 
    u.UserId, 
    COALESCE(u.UserFullName, '') AS FullName,
    COALESCE(u.UserKey, '') AS UserKey,
    COALESCE(u.FirmGUID, '') AS FirmGUID,
    COALESCE(u.UserGUID, '') AS UserGUID,
    CASE WHEN u.IsActive = 1 THEN 1 ELSE 0 END AS IsActive,
    sp.PasswordHash,
    sp.Salt,
    sp.Iterations
FROM UstadUsers u
LEFT JOIN UstadUserSecurePasswords sp ON u.UserId = sp.UserId
WHERE (u.UserEMail = @u OR u.UserTcNo = @u OR u.UserMobileNo = @u) 
  AND u.IsActive = 1";
                    
                    cmd.Parameters.AddWithValue("@u", request.UserName);
                    using var r = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
                    if (!await r.ReadAsync())
                    {
                        return Unauthorized("Kullanıcı bulunamadı veya aktif değil.");
                    }

                    userId = r.GetInt32("UserId");
                    fullName = r.GetString("FullName");
                    userKeyPlain = r.GetString("UserKey");
                    firmGuid = r.GetString("FirmGUID");
                    userGuid = r.GetString("UserGUID");
                    isActive = r.GetInt32("IsActive") == 1;
                    
                    // Check for secure password
                    if (!r.IsDBNull("PasswordHash"))
                    {
                        hash = (byte[])r["PasswordHash"];
                        salt = (byte[])r["Salt"];
                        iterations = r.GetInt32("Iterations");
                    }
                }
            }

            // Validate password - prefer secure hash, fallback to plain text
            bool verified = false;
            bool usingSecurePassword = false;
            
            if (hash != null && salt != null && iterations > 0)
            {
                // Use secure PBKDF2 verification
                verified = Classes.SecurePasswordHasher.Verify(request.Password, hash, salt, iterations);
                usingSecurePassword = true;
            }
            else if (!string.IsNullOrEmpty(userKeyPlain))
            {
                // Fallback to legacy plain text verification
                verified = string.Equals(userKeyPlain, request.Password);
                usingSecurePassword = false;
            }

            if (!verified)
                return Unauthorized("Şifre hatalı.");

            // If user logged in with legacy password, upgrade to secure password
            if (!usingSecurePassword && verified)
            {
                await UpgradeUserToSecurePassword(connStr, userId, request.Password);
            }

            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? _configuration["Jwt:Key"] ?? "CHANGE_ME_DEV_KEY_32CHARS_MIN";
            var issuer = _configuration["Jwt:Issuer"] ?? "UstadAuth";
            var audience = _configuration["Jwt:Audience"] ?? "UstadClients";
            var expiresMin = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var m) ? m : 480;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, fullName ?? string.Empty),
                new Claim(ClaimTypes.Role, role ?? "Agent"),
                new Claim("firm", firmGuid ?? string.Empty),
                new Claim("uname", request.UserName)
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMin),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new LoginResponse
            {
                Token = jwt,
                OperatorId = userId,
                UserGUID = userGuid,
                FullName = fullName,
                Role = role,
                FirmId = 0 // UstadUsers doesn't have numeric FirmId, using FirmGUID instead
            });
        }

        [HttpPost("login-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginRequest request)
        {
            var res = await Login(request);
            if (res is ObjectResult objResult && objResult.StatusCode < 400)
            {
                var payload = objResult.Value as LoginResponse;
                if (payload != null && (string.Equals(payload.Role, "Admin", StringComparison.OrdinalIgnoreCase) || 
                                       string.Equals(payload.FullName, "Tekin Uçar", StringComparison.OrdinalIgnoreCase)))
                {
                    // Override role to Admin for specific users
                    payload.Role = "Admin";
                    return Ok(payload);
                }
            }
            return Unauthorized("Admin yetkisi gerekli.");
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            // Registration is disabled - users must be created via existing UstadUsers management
            return BadRequest("Kayıt işlemi devre dışı. Lütfen yöneticinize başvurun.");
        }

        [HttpPost("resetPasswordRequest")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] ResetStartRequest req)
        {
            if (string.IsNullOrWhiteSpace(req?.UserName)) return BadRequest();
            
            using var con = new SqlConnection(BuildConnectionString());
            await con.OpenAsync();
            
            // Create reset tokens table if it doesn't exist
            using (var createCmd = con.CreateCommand())
            {
                createCmd.CommandText = @"
IF OBJECT_ID('UstadUserResetTokens') IS NULL
BEGIN
    CREATE TABLE UstadUserResetTokens (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Token NVARCHAR(128) NOT NULL UNIQUE,
        ExpiresAt DATETIME2 NOT NULL,
        Used BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
        FOREIGN KEY (UserId) REFERENCES UstadUsers(UserId)
    );
END";
                await createCmd.ExecuteNonQueryAsync();
            }
            
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
DECLARE @userId INT = (SELECT TOP 1 UserId FROM UstadUsers WHERE (UserEMail=@u OR UserTcNo=@u OR UserMobileNo=@u) AND IsActive=1);
IF @userId IS NULL BEGIN RAISERROR('User not found',16,1); RETURN; END
DECLARE @token NVARCHAR(128) = REPLACE(CONVERT(NVARCHAR(128), NEWID()),'-','');
INSERT INTO UstadUserResetTokens(UserId, Token, ExpiresAt) VALUES(@userId, @token, DATEADD(MINUTE,30,SYSUTCDATETIME()));
SELECT @token AS Token;";
            cmd.Parameters.AddWithValue("@u", req.UserName);
            var token = (string)await cmd.ExecuteScalarAsync();
            return Ok(new { token });
        }

        [HttpPost("resetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetConfirmRequest req)
        {
            if (string.IsNullOrWhiteSpace(req?.Token) || string.IsNullOrWhiteSpace(req?.NewPassword)) return BadRequest();
            
            var hashResult = Classes.SecurePasswordHasher.HashPassword(req.NewPassword);
            byte[] h = hashResult.Hash;
            byte[] s = hashResult.Salt;
            int i = hashResult.Iterations;
            using var con = new SqlConnection(BuildConnectionString());
            await con.OpenAsync();
            using var tr = con.BeginTransaction();
            try
            {
                int userId;
                using (var sel = con.CreateCommand())
                {
                    sel.Transaction = tr;
                    sel.CommandText = "SELECT UserId FROM UstadUserResetTokens WHERE Token=@t AND Used=0 AND ExpiresAt>SYSUTCDATETIME()";
                    sel.Parameters.AddWithValue("@t", req.Token);
                    var obj = await sel.ExecuteScalarAsync();
                    if (obj == null) { await tr.RollbackAsync(); return BadRequest("Token invalid/expired"); }
                    userId = (int)obj;
                }
                
                // Update or insert secure password
                using (var up = con.CreateCommand())
                {
                    up.Transaction = tr;
                    up.CommandText = @"
IF EXISTS (SELECT 1 FROM UstadUserSecurePasswords WHERE UserId=@id)
    UPDATE UstadUserSecurePasswords SET PasswordHash=@h, Salt=@s, Iterations=@i WHERE UserId=@id
ELSE
    INSERT INTO UstadUserSecurePasswords (UserId, PasswordHash, Salt, Iterations) VALUES (@id, @h, @s, @i)";
                    up.Parameters.Add("@h", SqlDbType.VarBinary, h.Length).Value = h;
                    up.Parameters.Add("@s", SqlDbType.VarBinary, s.Length).Value = s;
                    up.Parameters.AddWithValue("@i", i);
                    up.Parameters.AddWithValue("@id", userId);
                    await up.ExecuteNonQueryAsync();
                }
                
                using (var used = con.CreateCommand())
                {
                    used.Transaction = tr;
                    used.CommandText = "UPDATE UstadUserResetTokens SET Used=1 WHERE Token=@t";
                    used.Parameters.AddWithValue("@t", req.Token);
                    await used.ExecuteNonQueryAsync();
                }
                await tr.CommitAsync();
                return Ok();
            }
            catch
            {
                await tr.RollbackAsync();
                throw;
            }
        }

        [HttpPost("refresh")]
        [Authorize]
        public IActionResult Refresh()
        {
            // Re-issue token for current user
            var name = User.Identity?.Name ?? "";
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Agent";
            var firm = User.FindFirst("firm")?.Value ?? "0";
            var uname = User.FindFirst("uname")?.Value ?? "";

            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? _configuration["Jwt:Key"] ?? "CHANGE_ME_DEV_KEY_32CHARS_MIN";
            var issuer = _configuration["Jwt:Issuer"] ?? "UstadAuth";
            var audience = _configuration["Jwt:Audience"] ?? "UstadClients";
            var expiresMin = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var m) ? m : 480;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role),
                new Claim("firm", firm),
                new Claim("uname", uname)
            };
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddMinutes(expiresMin), signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new LoginResponse { 
                Token = jwt, 
                OperatorId = int.TryParse(id, out var opId) ? opId : 0, 
                FullName = name, 
                Role = role, 
                FirmId = int.TryParse(firm, out var firmId) ? firmId : 0 
            });
        }

        /// <summary>
        /// Upgrade a user from plain text password to secure PBKDF2 hash
        /// </summary>
        private async Task UpgradeUserToSecurePassword(string connectionString, int userId, string plainPassword)
        {
            try
            {
                var hashResult = Classes.SecurePasswordHasher.HashPassword(plainPassword);
                byte[] hash = hashResult.Hash;
                byte[] salt = hashResult.Salt;
                int iterations = hashResult.Iterations;
                
                using var con = new SqlConnection(connectionString);
                await con.OpenAsync();
                using var cmd = con.CreateCommand();
                cmd.CommandText = @"
INSERT INTO UstadUserSecurePasswords (UserId, PasswordHash, Salt, Iterations) 
VALUES (@userId, @hash, @salt, @iterations)";
                
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.Add("@hash", SqlDbType.VarBinary, hash.Length).Value = hash;
                cmd.Parameters.Add("@salt", SqlDbType.VarBinary, salt.Length).Value = salt;
                cmd.Parameters.AddWithValue("@iterations", iterations);
                
                await cmd.ExecuteNonQueryAsync();
            }
            catch
            {
                // Ignore upgrade errors - user can still login with legacy password
            }
        }

        /// <summary>
        /// Check if database is accessible
        /// </summary>
        private async Task<bool> IsDatabaseAccessible()
        {
            try
            {
                using var con = new SqlConnection(BuildConnectionString());
                await con.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Test mode login for when database is not accessible
        /// </summary>
        private async Task<IActionResult> LoginTestMode(LoginRequest request)
        {
            // Mock users for testing
            var testUsers = new Dictionary<string, (string password, string fullName, string role)>
            {
                { "tekinucar70@hotmail.com", ("7470", "Tekin Uçar", "Admin") },
                { "canberk.ucar.mail@gmail.com", ("84352", "Canberk Uçar", "Agent") },
                { "test@test.com", ("123456", "Test User", "Agent") }
            };

            if (testUsers.TryGetValue(request.UserName, out var user) && user.password == request.Password)
            {
                var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? _configuration["Jwt:Key"] ?? "CHANGE_ME_DEV_KEY_32CHARS_MIN";
                var issuer = _configuration["Jwt:Issuer"] ?? "UstadAuth";
                var audience = _configuration["Jwt:Audience"] ?? "UstadClients";
                var expiresMin = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var m) ? m : 480;

                var claims = new[]
                {
                    new Claim("sub", "1"),
                    new Claim("name", user.fullName),
                    new Claim("role", user.role),
                    new Claim("firm", "TEST_FIRM_GUID"),
                    new Claim("uname", request.UserName)
                };

                var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddMinutes(expiresMin), signingCredentials: creds);
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new LoginResponse 
                { 
                    Token = jwt, 
                    OperatorId = 1, 
                    UserGUID = "0BEBBBCC-B388-4E1F-80AF-C0C581DC3FDD", // Mock UserGUID for test mode
                    FullName = user.fullName, 
                    Role = user.role, 
                    FirmId = 0 
                });
            }

            return Unauthorized("Test modunda geçersiz kullanıcı adı veya şifre.");
        }
    }
}


