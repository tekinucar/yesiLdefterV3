using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace yesildeftertest.Authentication
{
    /// <summary>
    /// Unified authentication manager for Ustad Desktop WhatsApp integration
    /// Handles login, token management, and cross-platform authentication
    /// </summary>
    public class AuthenticationManager : tBase, IDisposable
    {
        #region tanımlar

        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private HubConnection _hubConnection;
        private System.Threading.Timer? _tokenRefreshTimer;
        private bool _disposed = false;
        
        public event EventHandler<AuthenticationEventArgs> AuthenticationChanged;
        public event EventHandler<string> ConnectionStatusChanged;
        public event EventHandler AuthenticationCompleted;
        public event EventHandler<string> AuthenticationFailed;

        #endregion

        #region Properties

        public bool IsAuthenticated => GlobalVariables.SP_UserLOGIN && !string.IsNullOrEmpty(GlobalVariables.tWhatsAppOperator.JwtToken);
        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
        public string CurrentOperatorName => GlobalVariables.tWhatsAppOperator.OperatorFullName;
        public string CurrentOperatorRole => GlobalVariables.tWhatsAppOperator.OperatorRole;
        public DateTime TokenExpiry => GlobalVariables.tWhatsAppOperator.TokenExpiry;

        #endregion

        #region constructor

        public AuthenticationManager(string? apiBaseUrl = null)
        {
            _apiBaseUrl = apiBaseUrl ?? GlobalVariables.tWhatsAppConfig.ApiBaseUrl ?? "http://localhost:5000";

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiBaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };


            preparingDefaultValues();
            setupTokenRefreshTimer();
        }

        #endregion

        #region Authentication Methods

        /// <summary>
        /// Authenticate user with username and password
        /// Returns authentication result with detailed information
        /// </summary>
        public async Task<AuthenticationResult> AuthenticateAsync(string username, string password, bool rememberMe = false)
        {
            try
            {
                debugMessage($"Authentication attempt for user: {username}");

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return AuthenticationResult.Failure("Kullanıcı adı ve şifre gereklidir");
                }

                // Call centralized Auth API
                // If Turnstile token was collected at UI, we placed it into GlobalVariables
                string tToken = GlobalVariables.tWhatsAppOperator.TurnstileToken;
                var req = new LoginRequest { UserName = username, Password = password, TurnstileToken = tToken };
                var response = await _httpClient.PostAsJsonAsync("/auth/login", req);

                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    debugMessage($"API auth failed: {response.StatusCode} {err}");
                    return AuthenticationResult.Failure("Giriş başarısız. Bilgilerinizi kontrol edin.");
                }

                var apiLogin = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (apiLogin == null || string.IsNullOrEmpty(apiLogin.Token))
                {
                    return AuthenticationResult.Failure("Geçersiz yanıt alındı.");
                }

                await StoreAuthenticationInfo(apiLogin, username, rememberMe);
                OnAuthenticationChanged(new AuthenticationEventArgs(true, username, apiLogin.Role));
                AuthenticationCompleted?.Invoke(this, EventArgs.Empty);
                debugMessage($"Authentication successful for: {username}");
                return AuthenticationResult.Success(apiLogin);
            }
            catch (Exception ex)
            {
                debugMessage($"Unexpected authentication error: {ex.Message}");
                AuthenticationFailed?.Invoke(this, "Beklenmeyen hata oluştu");
                return AuthenticationResult.Failure("Beklenmeyen hata oluştu");
            }
        }


        /// <summary>
        /// Logout current user and cleanup resources
        /// </summary>
        public async Task LogoutAsync()
        {
            try
            {
                debugMessage($"Logout initiated for: {GlobalVariables.tWhatsAppOperator.OperatorUserName}");

                // Disconnect SignalR
                if (_hubConnection != null)
                {
                    await _hubConnection.DisposeAsync();
                    _hubConnection = null;
                }

                // Clear authentication data
                ClearAuthenticationInfo();

                // Raise authentication event
                OnAuthenticationChanged(new AuthenticationEventArgs(false, "", ""));

                debugMessage("Logout completed");
            }
            catch (Exception ex)
            {
                debugMessage($"Logout error: {ex.Message}");
            }
        }

        #endregion

        #region Token Management

        /// <summary>
        /// Check if current token is valid and not expired
        /// </summary>
        public bool IsTokenValid()
        {
            return !string.IsNullOrEmpty(GlobalVariables.tWhatsAppOperator.JwtToken) &&
                   GlobalVariables.tWhatsAppOperator.TokenExpiry > DateTime.UtcNow.AddMinutes(5);
        }

        /// <summary>
        /// Refresh JWT token if needed
        /// </summary>
        public async Task<bool> RefreshTokenIfNeededAsync()
        {
            try
            {
                if (IsTokenValid())
                    return true;

                debugMessage("Token refresh needed");

                // Try to refresh token
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GlobalVariables.tWhatsAppOperator.JwtToken);

                var response = await _httpClient.PostAsync("/auth/refresh", null);

                if (response.IsSuccessStatusCode)
                {
                    var refreshResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (refreshResponse != null)
                    {
                        GlobalVariables.tWhatsAppOperator.JwtToken = refreshResponse.Token;
                        GlobalVariables.tWhatsAppOperator.TokenExpiry = DateTime.UtcNow.AddMinutes(480);

                        debugMessage("Token refreshed successfully");
                        return true;
                    }
                }

                debugMessage("Token refresh failed - re-authentication required");
                return false;
            }
            catch (Exception ex)
            {
                debugMessage($"Token refresh error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region SignalR Connection

        /// <summary>
        /// Initialize SignalR connection for real-time messaging
        /// </summary>
        public async Task<bool> InitializeSignalRConnection()
        {
            try
            {
                if (_hubConnection != null)
                {
                    await _hubConnection.DisposeAsync();
                }

                debugMessage("Initializing SignalR connection");
                OnConnectionStatusChanged("Bağlanıyor...");

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl($"{_apiBaseUrl}/messagehub", options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(GlobalVariables.tWhatsAppOperator.JwtToken);
                    })
                    .WithAutomaticReconnect(new[] {
                        TimeSpan.Zero,
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(30)
                    })
                    .Build();

                // Register connection events
                _hubConnection.Reconnecting += (error) =>
                {
                    OnConnectionStatusChanged("Yeniden bağlanıyor...");
                    return Task.CompletedTask;
                };

                _hubConnection.Reconnected += (connectionId) =>
                {
                    OnConnectionStatusChanged("Bağlı");
                    GlobalVariables.SP_WhatsAppConnected = true;
                    return Task.CompletedTask;
                };

                _hubConnection.Closed += (error) =>
                {
                    OnConnectionStatusChanged("Bağlantı kesildi");
                    GlobalVariables.SP_WhatsAppConnected = false;
                    return Task.CompletedTask;
                };

                await _hubConnection.StartAsync();

                OnConnectionStatusChanged("Bağlı");
                GlobalVariables.SP_WhatsAppConnected = true;

                debugMessage("SignalR connection established");
                return true;
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged("Bağlantı hatası");
                debugMessage($"SignalR connection error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get SignalR hub connection for messaging
        /// </summary>
        public HubConnection GetHubConnection()
        {
            return _hubConnection;
        }

        #endregion

        #region Cross-Platform Integration


        #endregion

        #region Private Methods

        /// <summary>
        /// Store authentication information in global variables and registry
        /// </summary>
        private async Task StoreAuthenticationInfo(LoginResponse loginResponse, string username, bool rememberMe)
        {
            // Update global variables
            GlobalVariables.tWhatsAppOperator.OperatorId = loginResponse.OperatorId;
            GlobalVariables.tWhatsAppOperator.OperatorUserName = username;
            GlobalVariables.tWhatsAppOperator.OperatorFullName = loginResponse.FullName;
            GlobalVariables.tWhatsAppOperator.OperatorRole = loginResponse.Role;
            GlobalVariables.tWhatsAppOperator.JwtToken = loginResponse.Token;
            GlobalVariables.tWhatsAppOperator.FirmId = loginResponse.FirmId;
            GlobalVariables.tWhatsAppOperator.TokenExpiry = DateTime.UtcNow.AddMinutes(480); // 8 hours
            GlobalVariables.tWhatsAppOperator.IsConnected = true;

            // Update global state
            GlobalVariables.SP_UserLOGIN = true;
            GlobalVariables.SP_UserIN = true;

            // Clear any one-off Turnstile token after successful login
            GlobalVariables.tWhatsAppOperator.TurnstileToken = null;

            // Store in registry if remember me is checked
            if (rememberMe)
            {
                await StoreCredentialsInRegistry(username);
            }

            // Update HTTP client authorization header
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.Token);
        }

        /// <summary>
        /// Store credentials in Windows Registry
        /// </summary>
        private async Task StoreCredentialsInRegistry(string username)
        {
            try
            {
                var regPath = GlobalVariables.registryPath;
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regPath))
                {
                    key?.SetValue("LastOperator", username);
                    key?.SetValue("LastLoginTime", DateTime.Now.ToString());
                    key?.SetValue("ApiUrl", _apiBaseUrl);
                }

                GlobalVariables.tUserRegister.WhatsAppLastOperator = username;
                debugMessage("Credentials stored in registry");
            }
            catch (Exception ex)
            {
                debugMessage($"Registry storage error: {ex.Message}");
            }
        }

        /// <summary>
        /// Load saved credentials from Windows Registry
        /// </summary>
        public string LoadSavedUsername()
        {
            try
            {
                var regPath = GlobalVariables.registryPath;
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(regPath))
                {
                    var savedUsername = key?.GetValue("LastOperator") as string;
                    if (!string.IsNullOrEmpty(savedUsername))
                    {
                        GlobalVariables.tUserRegister.WhatsAppLastOperator = savedUsername;
                        debugMessage($"Loaded saved username: {savedUsername}");
                        return savedUsername;
                    }
                }
            }
            catch (Exception ex)
            {
                debugMessage($"Registry load error: {ex.Message}");
            }

            return string.Empty;
        }

        /// <summary>
        /// Clear authentication information
        /// </summary>
        private void ClearAuthenticationInfo()
        {
            GlobalVariables.clearWhatsAppOperatorInfo();
            GlobalVariables.SP_UserLOGIN = false;
            GlobalVariables.SP_UserIN = false;
            GlobalVariables.SP_WhatsAppConnected = false;

            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        /// <summary>
        /// Setup automatic token refresh timer
        /// </summary>
        private void setupTokenRefreshTimer()
        {
            _tokenRefreshTimer = new System.Threading.Timer(async (state) =>
            {
                if (IsAuthenticated && !await RefreshTokenIfNeededAsync())
                {
                    // Token refresh failed - user needs to re-authenticate
                    await LogoutAsync();
                    OnAuthenticationChanged(new AuthenticationEventArgs(false, "", "", "Token expired - please login again"));
                }
            }, null, TimeSpan.FromMinutes(60), TimeSpan.FromMinutes(60)); // Check every hour
        }


        #endregion

        #region Event Handlers

        protected virtual void OnAuthenticationChanged(AuthenticationEventArgs e)
        {
            AuthenticationChanged?.Invoke(this, e);
        }

        protected virtual void OnConnectionStatusChanged(string status)
        {
            ConnectionStatusChanged?.Invoke(this, status);
        }

        #endregion

        #region IDisposable

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected new virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _tokenRefreshTimer?.Dispose();
                _hubConnection?.DisposeAsync();
                _httpClient?.Dispose();
                _disposed = true;
            }
        }

        #endregion

        #region Data Models

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


        #endregion
    }

    #region Authentication Result Classes

    /// <summary>
    /// Authentication operation result
    /// </summary>
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public AuthenticationManager.LoginResponse LoginData { get; set; }

        public static AuthenticationResult Success(AuthenticationManager.LoginResponse loginData)
        {
            return new AuthenticationResult
            {
                IsSuccess = true,
                LoginData = loginData
            };
        }

        public static AuthenticationResult Failure(string errorMessage)
        {
            return new AuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }

    /// <summary>
    /// Authentication event arguments
    /// </summary>
    public class AuthenticationEventArgs : EventArgs
    {
        public bool IsAuthenticated { get; }
        public string Username { get; }
        public string Role { get; }
        public string Message { get; }

        public AuthenticationEventArgs(bool isAuthenticated, string username, string role, string message = "")
        {
            IsAuthenticated = isAuthenticated;
            Username = username;
            Role = role;
            Message = message;
        }
    }

    #endregion
}
