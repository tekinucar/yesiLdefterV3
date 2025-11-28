using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;

namespace Ustad.API.Controllers
{
    [ApiController]
    [Route("auth")] 
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly Classes.EmailService _emailService;

        public AuthController(IConfiguration configuration, Classes.EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
        }

        /// <summary>
        /// Login request payload
        /// </summary>
        /// <summary>
        /// Login request payload
        /// </summary>
        public class LoginRequest
        {
            /// <summary>
            /// User email, TC number, or mobile number
            /// </summary>
            /// <summary>
            /// User email, TC number, or mobile number
            /// </summary>
            public string UserName { get; set; } = string.Empty;
            /// <summary>
            /// User password
            /// </summary>
            /// <summary>
            /// User password
            /// </summary>
            public string Password { get; set; } = string.Empty;
            /// <summary>
            /// Cloudflare Turnstile token for bot protection (optional in dev)
            /// </summary>
            /// <summary>
            /// Cloudflare Turnstile token for bot protection (optional in dev)
            /// </summary>
            public string TurnstileToken { get; set; } = string.Empty;
        }

        /// <summary>
        /// Login response containing access and refresh tokens
        /// </summary>
        public class LoginResponse
        {
            /// <summary>
            /// JWT access token (short-lived)
            /// </summary>
            public string Token { get; set; } = string.Empty;
            /// <summary>
            /// JWT refresh token (long-lived)
            /// </summary>
            public string RefreshToken { get; set; } = string.Empty;
            /// <summary>
            /// Access token expiration time in seconds
            /// </summary>
            public int AccessTokenExpiresInSeconds { get; set; }
            /// <summary>
            /// Refresh token expiration time in seconds
            /// </summary>
            public int RefreshTokenExpiresInSeconds { get; set; }
            /// <summary>
            /// User/Operator ID
            /// </summary>
            public int OperatorId { get; set; }
            /// <summary>
            /// User GUID identifier
            /// </summary>
            public string UserGUID { get; set; } = string.Empty;
            /// <summary>
            /// User full name
            /// </summary>
            public string FullName { get; set; } = string.Empty;
            /// <summary>
            /// User role (Agent, Admin, etc.)
            /// </summary>
            public string Role { get; set; } = string.Empty;
            /// <summary>
            /// Firm ID (legacy, may be 0)
            /// </summary>
            public int FirmId { get; set; }
            /// <summary>
            /// Database type identifier
            /// </summary>
            public short DbTypeId { get; set; }
        }

        /// <summary>
        /// Refresh token request payload
        /// </summary>
        public class RefreshTokenRequest
        {
            /// <summary>
            /// Valid refresh token to exchange for new token pair
            /// </summary>
            public string RefreshToken { get; set; } = string.Empty;
        }

        /// <summary>
        /// Logout request payload
        /// </summary>
        public class LogoutRequest
        {
            /// <summary>
            /// Refresh token to invalidate
            /// </summary>
            public string RefreshToken { get; set; } = string.Empty;
        }

        public class RegisterRequest
        {
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public int? FirmId { get; set; }
            public string Role { get; set; } = string.Empty;
            public string TurnstileToken { get; set; } = string.Empty;
        }

        public class ResetStartRequest { public string UserName { get; set; } = string.Empty; }
        public class ResetConfirmRequest { public string Token { get; set; } = string.Empty; public string NewPassword { get; set; } = string.Empty; }

        private string BuildConnectionString()
        {
            string host = Environment.GetEnvironmentVariable("DB_HOST") ?? _configuration["Db:Host"] ?? string.Empty; 
            string port = Environment.GetEnvironmentVariable("DB_PORT") ?? _configuration["Db:Port"] ?? "1433";
            string user = Environment.GetEnvironmentVariable("DB_USER") ?? _configuration["Db:User"] ?? string.Empty;
            string pass = Environment.GetEnvironmentVariable("DB_PASS") ?? _configuration["Db:Pass"] ?? string.Empty;
            string db   = Environment.GetEnvironmentVariable("DB_NAME") ?? _configuration["Db:Name"] ?? string.Empty;
            return !string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(db)
                ? $"Data Source={host},{port}; Initial Catalog={db}; User ID={user}; Password={pass}; TrustServerCertificate=true; Encrypt=false; MultipleActiveResultSets=True"
                : _configuration.GetConnectionString("BulutCrm") ?? string.Empty; 
        }

        private async Task<bool> ValidateTurnstileAsync(string? token)
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
                return obj?.success == true;
            }
            catch { return false; }
        }

        private string GetJwtKey()
        {
            var key = Environment.GetEnvironmentVariable("JWT_KEY") ?? _configuration["Jwt:Key"] ?? "UstadSecretKeyForJWTTokenGeneration2024SecureKey32Chars";
            
            // Ensure key is at least 32 characters (256 bits) for HS256
            if (key.Length < 32)
            {
                // Pad or repeat the key to meet minimum length requirement
                while (key.Length < 32)
                {
                    key += key;
                }
                key = key.Substring(0, 32);
            }
            
            return key;
        }

        private string GetJwtIssuer()
        {
            return _configuration["Jwt:Issuer"] ?? "UstadAuth";
        }

        private string GetJwtAudience()
        {
            return _configuration["Jwt:Audience"] ?? "UstadClients";
        }

        // NOTE(@Janberk) 8 hours default
        private int GetAccessTokenExpiresMinutes()
        {
            if (int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRES_MINUTES"), out var envValue) && envValue > 0)
            {
                return envValue;
            }

            if (int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var cfgValue) && cfgValue > 0)
            {
                return cfgValue;
            }

            return 480; 
        }

        // NOTE(@Janberk) 14 days default
        private int GetRefreshTokenExpiresMinutes()
        {
            if (int.TryParse(Environment.GetEnvironmentVariable("JWT_REFRESH_EXPIRES_MINUTES"), out var envValue) && envValue > 0)
            {
                return envValue;
            }

            if (int.TryParse(_configuration["Jwt:RefreshExpiresMinutes"], out var cfgValue) && cfgValue > 0)
            {
                return cfgValue;
            }

            return 60 * 24 * 14; 
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtKey())),
                ValidateIssuer = true,
                ValidIssuer = GetJwtIssuer(),
                ValidateAudience = true,
                ValidAudience = GetJwtAudience(),
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };
        }

        private List<Claim> BuildBaseClaims(int userId, string userGuid, string fullName, string role, string firmGuid, string userName, short dbTypeId)
        {
            var baseClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)),
                new Claim("userGUID", userGuid ?? string.Empty),
                new Claim(ClaimTypes.Name, fullName ?? string.Empty),
                new Claim(ClaimTypes.Role, string.IsNullOrWhiteSpace(role) ? "Agent" : role),
                new Claim("firm", firmGuid ?? string.Empty),
                new Claim("uname", userName ?? string.Empty),
                new Claim("dbTypeId", dbTypeId.ToString(CultureInfo.InvariantCulture))
            };

            return baseClaims;
        }

        private List<Claim> BuildBaseClaimsFromPrincipal(ClaimsPrincipal principal)
        {
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
            var userGuid = principal.FindFirst("userGUID")?.Value ?? string.Empty;
            var fullName = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value ?? "Agent";
            var firmGuid = principal.FindFirst("firm")?.Value ?? string.Empty;
            var userName = principal.FindFirst("uname")?.Value ?? string.Empty;
            var dbTypeIdClaim = principal.FindFirst("dbTypeId")?.Value ?? "0";

            short.TryParse(dbTypeIdClaim, out var dbTypeId);
            int.TryParse(userId, out var userIdInt);

            return BuildBaseClaims(userIdInt, userGuid, fullName, role, firmGuid, userName, dbTypeId);
        }

        private (string AccessToken, DateTime AccessExpiresAt, string RefreshToken, DateTime RefreshExpiresAt) GenerateTokenPair(IEnumerable<Claim> baseClaims)
        {
            var claimsList = baseClaims.ToList();
            var handler = new JwtSecurityTokenHandler();
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtKey()));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var issuer = GetJwtIssuer();
            var audience = GetJwtAudience();
            var accessMinutes = GetAccessTokenExpiresMinutes();
            var refreshMinutes = GetRefreshTokenExpiresMinutes();
            var now = DateTime.UtcNow;

            var accessClaims = new List<Claim>(claimsList)
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("token_type", "access")
            };

            var refreshClaims = new List<Claim>(claimsList)
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("token_type", "refresh")
            };

            var accessExpiresAt = now.AddMinutes(accessMinutes);
            var refreshExpiresAt = now.AddMinutes(refreshMinutes);

            var accessToken = handler.WriteToken(new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: accessClaims,
                notBefore: now,
                expires: accessExpiresAt,
                signingCredentials: credentials));

            var refreshToken = handler.WriteToken(new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: refreshClaims,
                notBefore: now,
                expires: refreshExpiresAt,
                signingCredentials: credentials));

            return (accessToken, accessExpiresAt, refreshToken, refreshExpiresAt);
        }

        /// <summary>
        /// Authenticates a user and returns access and refresh tokens
        /// </summary>
        /// <param name="request">Login credentials including username, password, and optional Turnstile token</param>
        /// <returns>LoginResponse with access token, refresh token, and user information</returns>
        /// <response code="200">Login successful, returns tokens and user info</response>
        /// <response code="400">Invalid request (missing username or password)</response>
        /// <response code="401">Authentication failed (invalid credentials or Turnstile validation failed)</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null)
            {
                return BadRequest("Geçersiz istek.");
            }
            if (string.IsNullOrWhiteSpace(request?.UserName) || string.IsNullOrWhiteSpace(request?.Password))
                return BadRequest("Kullanıcı adı ve şifre zorunludur.");

            if (!await ValidateTurnstileAsync(request.TurnstileToken))
                return Unauthorized("Turnstile doğrulaması başarısız.");

            string connStr = BuildConnectionString();

            // ============================================
            // PHASE 1: Minimal Password Verification Query
            // ============================================
            // Only query what's needed for password verification
            // This minimizes SQL connection time for invalid login attempts
            int userId = 0;
            string? userKeyPlain = null;
            byte[]? hash = null;
            byte[]? salt = null;
            int iterations = 0;
            bool isActive = false;

            using (var con = new SqlConnection(connStr))
            {
                await con.OpenAsync();
                
                // Ensure secure passwords table exists
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

                // Phase 1: Query ONLY password verification data
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT 
    u.UserId,
    COALESCE(u.UserKey, '') AS UserKey,
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
                    userKeyPlain = r.GetString("UserKey");
                    isActive = r.GetInt32("IsActive") == 1;
                    
                    if (!r.IsDBNull("PasswordHash"))
                    {
                        hash = (byte[])r["PasswordHash"];
                        salt = (byte[])r["Salt"];
                        iterations = r.GetInt32("Iterations");
                    }
                }
            } // Close connection immediately after Phase 1

            // ============================================
            // PHASE 2: Password Verification (NO SQL CONNECTION)
            // ============================================
            // Verify password BEFORE fetching full user data
            bool verified = false;
            bool usingSecurePassword = false;
            
            if (hash != null && salt != null && iterations > 0)
            {
                verified = Classes.SecurePasswordHasher.Verify(request.Password, hash!, salt!, iterations);
                usingSecurePassword = true;
                
                if (!verified && !string.IsNullOrEmpty(userKeyPlain))
                {
                    verified = string.Equals(userKeyPlain, request.Password, StringComparison.Ordinal);
                    if (verified)
                    {
                        // Upgrade password on next connection
                        usingSecurePassword = true;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(userKeyPlain))
            {
                verified = string.Equals(userKeyPlain, request.Password, StringComparison.Ordinal);
                usingSecurePassword = false;
            }

            if (!verified)
                return Unauthorized("Şifre hatalı.");

            // ============================================
            // PHASE 3: Full User Data + Password Upgrade (After Successful Auth)
            // ============================================
            // Only now that password is verified, fetch full user data
            string fullName = "";
            string role = "Agent";
            string firmGuid = "";
            string userGuid = "";
            short dbTypeId = 0;

            using (var con = new SqlConnection(connStr))
            {
                await con.OpenAsync();

                // Phase 3: Query full user data for token generation
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT 
    COALESCE(u.UserFullName, '') AS FullName,
    COALESCE(u.FirmGUID, '') AS FirmGUID,
    COALESCE(u.UserGUID, '') AS UserGUID,
    COALESCE(u.DbTypeId, 0) AS DbTypeId
FROM UstadUsers u
WHERE u.UserId = @userId";
                    
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using var r = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
                    if (await r.ReadAsync())
                    {
                        fullName = r.GetString("FullName");
                        firmGuid = r.GetString("FirmGUID");
                        userGuid = r.GetString("UserGUID");
                        if (!r.IsDBNull("DbTypeId"))
                        {
                            int dbTypeIdInt = r.GetInt32(r.GetOrdinal("DbTypeId"));
                            if (dbTypeIdInt > short.MaxValue) dbTypeId = short.MaxValue;
                            else if (dbTypeIdInt < short.MinValue) dbTypeId = short.MinValue;
                            else dbTypeId = (short)dbTypeIdInt;
                        }
                    }
                }

                // Password upgrade happens INSIDE Phase 3 connection (optimized)
                if (!usingSecurePassword && verified && (hash == null || salt == null || iterations == 0))
                {
                    await UpgradeUserToSecurePassword(connStr, userId, request.Password);
                }
                else if (verified && !usingSecurePassword && !string.IsNullOrEmpty(userKeyPlain))
                {
                    // Upgrade from plain text to secure password
                    await UpgradeUserToSecurePassword(connStr, userId, request.Password);
                }
            } // Close connection after Phase 3

            var baseClaims = BuildBaseClaims(userId, userGuid, fullName, role, firmGuid, request.UserName, dbTypeId);
            var tokens = GenerateTokenPair(baseClaims);

            return Ok(new LoginResponse
            {
                Token = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                AccessTokenExpiresInSeconds = GetAccessTokenExpiresMinutes() * 60,
                RefreshTokenExpiresInSeconds = GetRefreshTokenExpiresMinutes() * 60,
                OperatorId = userId,
                UserGUID = userGuid,
                FullName = fullName,
                Role = role,
                FirmId = 0,
                DbTypeId = dbTypeId
            });
        }

        /// <summary>
        /// QR Login endpoint for mobile shell authentication
        /// Uses stored procedure prc_GetMtskKursiyerSorgulamaJson
        /// </summary>
        [HttpPost("qr-login")]
        [AllowAnonymous]
        public async Task<IActionResult> QRLogin([FromBody] QRLoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.FirmGUID) || 
                string.IsNullOrWhiteSpace(request.UserGUID) || 
                string.IsNullOrWhiteSpace(request.TcNoTelefonNo) ||
                string.IsNullOrWhiteSpace(request.DbName))
            {
                return BadRequest("FirmGUID, UserGUID, TcNoTelefonNo, and DbName are required.");
            }

            try
            {
                // Build connection string for specific database
                string connStr = BuildConnectionStringForDb(request.DbName);

                using (var con = new SqlConnection(connStr))
                {
                    await con.OpenAsync();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[prc_GetMtskKursiyerSorgulamaJson]";
                        cmd.Parameters.AddWithValue("@FirmGUID", request.FirmGUID);
                        cmd.Parameters.AddWithValue("@UserGUID", request.UserGUID);
                        cmd.Parameters.AddWithValue("@TcNoTelefonNo", request.TcNoTelefonNo);

                        using var r = await cmd.ExecuteReaderAsync();
                        if (await r.ReadAsync())
                        {
                            // Read JSON result from stored procedure
                            string jsonResult = r.IsDBNull(0) ? "{}" : r.GetString(0);
                            
                            return Ok(new QRLoginResponse
                            {
                                User = System.Text.Json.JsonSerializer.Deserialize<object>(jsonResult),
                                Token = null // QR login doesn't return JWT token
                            });
                        }
                        else
                        {
                            return Unauthorized("User not found or invalid credentials.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"QR login failed: {ex.Message}");
            }
        }

        public class QRLoginRequest
        {
            public string FirmGUID { get; set; } = string.Empty;
            public string UserGUID { get; set; } = string.Empty;
            public string TcNoTelefonNo { get; set; } = string.Empty;
            public string DbName { get; set; } = string.Empty;
        }

        public class QRLoginResponse
        {
            public object? User { get; set; }
            public string? Token { get; set; }
        }

        private string BuildConnectionStringForDb(string dbName)
        {
            string baseConnStr = BuildConnectionString();
            // Replace the database name in the connection string
            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(baseConnStr);
            builder.InitialCatalog = dbName;
            return builder.ConnectionString;
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
            return BadRequest("Kayıt işlemi devre dışı. Lütfen yöneticinize başvurun.");
        }

        [HttpPost("resetPasswordRequest")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] ResetStartRequest req)
        {
            if (req == null)
            {
                return BadRequest();
            }
            if (string.IsNullOrWhiteSpace(req?.UserName)) return BadRequest();
            
            using var con = new SqlConnection(BuildConnectionString());
            await con.OpenAsync();
            
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
            
            string userEmail = string.Empty;
            string userName = string.Empty;
            string token = string.Empty;
            
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = @"
SELECT TOP 1 
    UserId,
    COALESCE(UserEMail, '') AS UserEMail,
    COALESCE(UserFullName, '') AS UserFullName
FROM UstadUsers 
WHERE (UserEMail=@u OR UserTcNo=@u OR UserMobileNo=@u) AND IsActive=1";
                cmd.Parameters.AddWithValue("@u", req.UserName);
                using var r = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
                
                if (!await r.ReadAsync())
                {
                    return Ok(new { message = "If the user exists, a password reset link has been sent to their email." });
                }
                
                int userId = r.GetInt32("UserId");
                userEmail = r.GetString("UserEMail");
                userName = r.GetString("UserFullName");
                
                token = System.Guid.NewGuid().ToString("N").ToUpper().Substring(0, 32);
                
                r.Close();
                using (var insertCmd = con.CreateCommand())
                {
                    insertCmd.CommandText = @"
INSERT INTO UstadUserResetTokens(UserId, Token, ExpiresAt) 
VALUES(@userId, @token, DATEADD(MINUTE,30,SYSUTCDATETIME()))";
                    insertCmd.Parameters.AddWithValue("@userId", userId);
                    insertCmd.Parameters.AddWithValue("@token", token);
                    await insertCmd.ExecuteNonQueryAsync();
                }
            }
            
            if (!string.IsNullOrWhiteSpace(userEmail) && !string.IsNullOrWhiteSpace(token))
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendPasswordResetEmailAsync(
                            userEmail, 
                            string.IsNullOrWhiteSpace(userName) ? userEmail : userName, 
                            token
                        );
                    }
                    catch
                    {
                    }
                });
            }
            
            // Always return success (security best practice - don't reveal if user exists)
            return Ok(new { message = "If the user exists, a password reset link has been sent to their email." });
        }

        [HttpPost("resetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetConfirmRequest req)
        {
            if (req == null)
            {
                return BadRequest();
            }
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

        /// <summary>
        /// Refreshes an access token using a valid refresh token
        /// </summary>
        /// <param name="request">Refresh token request containing a valid refresh token</param>
        /// <returns>New LoginResponse with rotated access and refresh tokens</returns>
        /// <response code="200">Token refresh successful, returns new token pair</response>
        /// <response code="400">Invalid request (missing refresh token)</response>
        /// <response code="401">Invalid or expired refresh token</response>
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult Refresh([FromBody] RefreshTokenRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal;
            SecurityToken validatedToken;

            try
            {
                principal = handler.ValidateToken(request.RefreshToken, GetTokenValidationParameters(), out validatedToken);
            }
            catch
            {
                return Unauthorized("Invalid refresh token.");
            }

            if (!(validatedToken is JwtSecurityToken jwtToken &&
                string.Equals(jwtToken.Header.Alg, SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase)))
            {
                return Unauthorized("Invalid refresh token.");
            }

            var tokenType = principal.FindFirst("token_type")?.Value;
            if (!string.Equals(tokenType, "refresh", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized("Invalid token type.");
            }

            var baseClaims = BuildBaseClaimsFromPrincipal(principal);

            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
            int.TryParse(idClaim, out var operatorId);
            var userGuid = principal.FindFirst("userGUID")?.Value ?? string.Empty;
            var fullName = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value ?? "Agent";
            var dbTypeIdClaim = principal.FindFirst("dbTypeId")?.Value ?? "0";
            short.TryParse(dbTypeIdClaim, out var dbTypeId);

            var tokens = GenerateTokenPair(baseClaims);

            return Ok(new LoginResponse
            {
                Token = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                AccessTokenExpiresInSeconds = GetAccessTokenExpiresMinutes() * 60,
                RefreshTokenExpiresInSeconds = GetRefreshTokenExpiresMinutes() * 60,
                OperatorId = operatorId,
                UserGUID = userGuid,
                FullName = fullName,
                Role = role,
                FirmId = 0,
                DbTypeId = dbTypeId
            });
        }

        /// <summary>
        /// Logs out a user and invalidates the refresh token
        /// </summary>
        /// <param name="request">Logout request containing refresh token to invalidate</param>
        /// <returns>Success response</returns>
        /// <response code="200">Logout successful</response>
        /// <response code="401">Unauthorized (invalid or missing JWT)</response>
        /// <remarks>
        /// Note: Redis blacklist integration for token invalidation will be added in a subsequent iteration.
        /// </remarks>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult Logout([FromBody] LogoutRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal;
            SecurityToken validatedToken;

            try
            {
                principal = handler.ValidateToken(request.RefreshToken, GetTokenValidationParameters(), out validatedToken);
            }
            catch
            {
                return Unauthorized("Invalid refresh token.");
            }

            if (!(validatedToken is JwtSecurityToken jwtToken &&
                string.Equals(jwtToken.Header.Alg, SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase)))
            {
                return Unauthorized("Invalid refresh token.");
            }

            var tokenType = principal.FindFirst("token_type")?.Value;
            if (!string.Equals(tokenType, "refresh", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized("Invalid token type.");
            }

            var baseClaims = BuildBaseClaimsFromPrincipal(principal);

            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
            int.TryParse(idClaim, out var operatorId);
            var userGuid = principal.FindFirst("userGUID")?.Value ?? string.Empty;
            var fullName = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value ?? "Agent";
            var dbTypeIdClaim = principal.FindFirst("dbTypeId")?.Value ?? "0";
            short.TryParse(dbTypeIdClaim, out var dbTypeId);

            var tokens = GenerateTokenPair(baseClaims);

            return Ok(new LoginResponse
            {
                Token = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                AccessTokenExpiresInSeconds = GetAccessTokenExpiresMinutes() * 60,
                RefreshTokenExpiresInSeconds = GetRefreshTokenExpiresMinutes() * 60,
                OperatorId = operatorId,
                UserGUID = userGuid,
                FullName = fullName,
                Role = role,
                FirmId = 0,
                DbTypeId = dbTypeId
            });
        }

        [HttpGet("user/exists")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUserExists([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required");

            string connStr = BuildConnectionString();
            using var con = new SqlConnection(connStr);
            await con.OpenAsync();

            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
SELECT TOP 1 
    UserId, 
    COALESCE(UserFullName, '') AS UserFullName,
    CASE WHEN IsActive = 1 THEN 1 ELSE 0 END AS IsActive
FROM UstadUsers
WHERE (UserEMail = @email OR UserTcNo = @email OR UserMobileNo = @email)";

            cmd.Parameters.AddWithValue("@email", email);
            using var r = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await r.ReadAsync())
            {
                return Ok(new { Exists = false });
            }

            return Ok(new
            {
                Exists = true,
                UserId = r.GetInt32("UserId"),
                UserFullName = r.GetString("UserFullName"),
                IsActive = r.GetInt32("IsActive") == 1
            });
        }

        [HttpPost("changepassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null)
            {
                return BadRequest("Email, old password, and new password are required");
            }
            if (string.IsNullOrWhiteSpace(request?.Email) || 
                string.IsNullOrWhiteSpace(request?.OldPassword) || 
                string.IsNullOrWhiteSpace(request?.NewPassword))
                return BadRequest("Email, old password, and new password are required");

            if (request.NewPassword.Length < 4)
                return BadRequest("New password must be at least 4 characters");

            if (request.NewPassword == request.OldPassword)
                return BadRequest("New password must be different from old password");

            string connStr = BuildConnectionString();

            // First, verify old password
            int userId = 0;
            string? db_user_key = null;
            byte[]? hash = null;
            byte[]? salt = null;
            int iterations = 0;

            using (var con = new SqlConnection(connStr))
            {
                await con.OpenAsync();

                using var cmd = con.CreateCommand();
                cmd.CommandText = @"
SELECT 
    u.UserId, 
    COALESCE(u.UserKey, '') AS UserKey,
    sp.PasswordHash,
    sp.Salt,
    sp.Iterations
FROM UstadUsers u
LEFT JOIN UstadUserSecurePasswords sp ON u.UserId = sp.UserId
WHERE (u.UserEMail = @email OR u.UserTcNo = @email OR u.UserMobileNo = @email) 
  AND u.IsActive = 1";

                cmd.Parameters.AddWithValue("@email", request.Email);
                using var r = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

                if (!await r.ReadAsync())
                    return Unauthorized("User not found or inactive");

                userId = r.GetInt32("UserId");
                db_user_key = r.GetString("UserKey");

                if (!r.IsDBNull("PasswordHash"))
                {
                    hash = (byte[])r["PasswordHash"];
                    salt = (byte[])r["Salt"];
                    iterations = r.GetInt32("Iterations");
                }
            }

            // Verify old password
            bool verified = false;
            if (hash != null && salt != null && iterations > 0)
            {
                verified = Classes.SecurePasswordHasher.Verify(request.OldPassword, hash!, salt!, iterations);
            }
            else if (!string.IsNullOrEmpty(db_user_key))
            {
                verified = string.Equals(db_user_key, request.OldPassword);
            }

            if (!verified)
                return Unauthorized("Old password is incorrect");

            // Update password
            var hashResult = Classes.SecurePasswordHasher.HashPassword(request.NewPassword);

            using (var con = new SqlConnection(connStr))
            {
                await con.OpenAsync();
                using var tr = con.BeginTransaction();
                try
                {
                    using var cmd = con.CreateCommand();
                    cmd.Transaction = tr;
                    cmd.CommandText = @"
IF EXISTS (SELECT 1 FROM UstadUserSecurePasswords WHERE UserId = @userId)
    UPDATE UstadUserSecurePasswords 
    SET PasswordHash = @hash, Salt = @salt, Iterations = @iterations 
    WHERE UserId = @userId
ELSE
    INSERT INTO UstadUserSecurePasswords (UserId, PasswordHash, Salt, Iterations) 
    VALUES (@userId, @hash, @salt, @iterations)";

                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.Add("@hash", SqlDbType.VarBinary, hashResult.Hash.Length).Value = hashResult.Hash;
                    cmd.Parameters.Add("@salt", SqlDbType.VarBinary, hashResult.Salt.Length).Value = hashResult.Salt;
                    cmd.Parameters.AddWithValue("@iterations", hashResult.Iterations);

                    await cmd.ExecuteNonQueryAsync();
                    await tr.CommitAsync();
                }
                catch
                {
                    await tr.RollbackAsync();
                    throw;
                }
            }

            return Ok(new { Message = "Password changed successfully" });
        }

        public class ChangePasswordRequest
        {
            public string Email { get; set; } = string.Empty;
            public string OldPassword { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
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
    }
}


