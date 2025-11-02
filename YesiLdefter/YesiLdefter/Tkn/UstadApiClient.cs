using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tkn_Variable;
using Newtonsoft.Json;

namespace Tkn_UstadAPI
{
    /// <summary>
    /// Secure API client for Ustad.API authentication and user operations
    /// No database connection strings are exposed - all operations go through the API
    /// </summary>
    public class UstadApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private bool _disposed = false;

        public UstadApiClient(string apiBaseUrl = "http://localhost:5000")
        {
            _apiBaseUrl = apiBaseUrl.TrimEnd('/');
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiBaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        /// <summary>
        /// Login with email and password, returns JWT token and user info
        /// </summary>
        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            try
            {
                var request = new LoginRequest
                {
                    UserName = email,
                    Password = password,
                    TurnstileToken = "" // Optional - can be added if needed
                };

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/auth/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Login failed: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                
                // Store token for subsequent requests
                if (!string.IsNullOrEmpty(loginResponse?.Token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.Token);
                }

                return loginResponse;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API connection error: {ex.Message}", ex);
            }
            catch (TaskCanceledException)
            {
                throw new Exception("API request timeout. Please check if the API is running.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Login error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if user email exists in the system
        /// </summary>
        public async Task<UserExistsResponse> CheckUserExistsAsync(string email)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/auth/user/exists?email={Uri.EscapeDataString(email)}");

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return new UserExistsResponse { Exists = false };
                    }
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Check user failed: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserExistsResponse>(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Check user error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Change user password (requires old password verification)
        /// </summary>
        public async Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            try
            {
                var request = new ChangePasswordRequest
                {
                    Email = email,
                    OldPassword = oldPassword,
                    NewPassword = newPassword
                };

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/auth/changepassword", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Password change failed: {errorContent}");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Change password error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Request password reset (sends email with reset token)
        /// </summary>
        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            try
            {
                var request = new ResetPasswordRequestRequest { UserName = email };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/auth/resetPasswordRequest", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Password reset request failed: {errorContent}");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Password reset request error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get user firms list using UserGUID
        /// </summary>
        public async Task<List<FirmInfo>> GetUserFirmsAsync(string userGUID)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/UstadFirm/user/{Uri.EscapeDataString(userGUID)}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Get user firms failed: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var firms = JsonConvert.DeserializeObject<List<FirmInfo>>(responseContent);
                return firms ?? new List<FirmInfo>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Get user firms error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get firm details by FirmGUID
        /// </summary>
        public async Task<FirmDetails> GetFirmDetailsAsync(string firmGUID)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/UstadFirm/{Uri.EscapeDataString(firmGUID)}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Get firm details failed: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<FirmDetails>(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Get firm details error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Set authorization token for authenticated requests
        /// </summary>
        public void SetAuthToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = null;
            else
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }

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

        public class UserExistsResponse
        {
            public bool Exists { get; set; }
            public int? UserId { get; set; }
            public string UserFullName { get; set; }
            public bool IsActive { get; set; }
        }

        public class ChangePasswordRequest
        {
            public string Email { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        public class ResetPasswordRequestRequest
        {
            public string UserName { get; set; }
        }

        public class FirmInfo
        {
            public int Id { get; set; }
            public int FirmId { get; set; }
            public string FirmGUID { get; set; }
            public string FirmLongName { get; set; }
            public int? UserId { get; set; }
            public string UserGUID { get; set; }
            public string UserFullName { get; set; }
            public bool IsActive { get; set; }
        }

        public class FirmDetails
        {
            public Firm Firm { get; set; }
        }

        public class Firm
        {
            public int FirmId { get; set; }
            public string FirmGUID { get; set; }
            public string FirmLongName { get; set; }
            public int UserCount { get; set; }
            public UserInfo[] Users { get; set; }
        }

        public class UserInfo
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string UserGUID { get; set; }
            public string UserFullName { get; set; }
            public string UserEMail { get; set; }
            public bool IsActive { get; set; }
        }

        #endregion
    }
}

