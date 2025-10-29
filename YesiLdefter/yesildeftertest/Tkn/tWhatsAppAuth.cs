using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yesildeftertest
{
    /// <summary>
    /// WhatsApp Authentication - Tekin's pattern ile entegre
    /// Güvenli kimlik doğrulama sistemi
    /// </summary>
    public class tWhatsAppAuth : tBase
    {
        #region tanımlar - Tekin's pattern

        private tToolBox t;
        private HttpClient httpClient;
        private string apiBaseUrl;

        #endregion

        #region constructor

        public tWhatsAppAuth()
        {
            preparingDefaultValues();
            t = new tToolBox();
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            apiBaseUrl = GlobalVariables.Instance.whatsAppConfig.ApiBaseUrl;
        }

        #endregion

        #region Authentication Methods - Tekin's pattern

        /// <summary>
        /// Operatör girişi - Tekin's pattern (checkedInput benzeri)
        /// </summary>
        public async Task<bool> operatorLogin(string userName, string password)
        {
            try
            {
                debugMessage($"Operatör girişi deneniyor: {userName}");

                // Login request hazırla
                var loginRequest = new
                {
                    UserName = userName,
                    Password = password
                };

                string jsonContent = JsonSerializer.Serialize(loginRequest);
                string url = $"{apiBaseUrl}/auth/login";

                // HTTP headers hazırla
                var headers = new System.Collections.Generic.Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json"
                };

                // Login isteği gönder
                string response = await t.HTTP_POST(url, jsonContent, headers);

                if (isNotNull(response))
                {
                    // Response parse et
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(response);
                    
                    if (loginResponse != null && isNotNull(loginResponse.Token))
                    {
                        // Başarılı giriş - operatör bilgilerini kaydet
                        saveOperatorInfo(loginResponse, userName);
                        
                        debugMessage($"Operatör girişi başarılı: {loginResponse.FullName}");
                        return true;
                    }
                }

                debugMessage("Operatör girişi başarısız - geçersiz yanıt");
                return false;
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("401"))
            {
                errorMessage("Kullanıcı adı veya şifre hatalı.");
                return false;
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("423"))
            {
                errorMessage("Hesabınız kilitlenmiştir. Lütfen daha sonra tekrar deneyin.");
                return false;
            }
            catch (Exception ex)
            {
                errorMessage("Giriş sırasında hata oluştu", ex);
                return false;
            }
        }

        /// <summary>
        /// Operatör bilgilerini kaydet - Tekin's pattern
        /// </summary>
        private void saveOperatorInfo(LoginResponse loginResponse, string userName)
        {
            // Global değişkenlere kaydet - Tekin's pattern
            GlobalVariables.tWhatsAppOperator.OperatorId = loginResponse.OperatorId;
            GlobalVariables.tWhatsAppOperator.OperatorUserName = userName;
            GlobalVariables.tWhatsAppOperator.OperatorFullName = loginResponse.FullName;
            GlobalVariables.tWhatsAppOperator.OperatorRole = loginResponse.Role;
            GlobalVariables.tWhatsAppOperator.JwtToken = loginResponse.Token;
            GlobalVariables.tWhatsAppOperator.FirmId = loginResponse.FirmId;
            GlobalVariables.tWhatsAppOperator.IsConnected = true;
            GlobalVariables.tWhatsAppOperator.TokenExpiry = DateTime.UtcNow.AddMinutes(480); // 8 saat

            // Global state update - Tekin's pattern
            GlobalVariables.SP_WhatsAppConnected = true;
            GlobalVariables.SP_UserLOGIN = true;

            // Registry'ye kaydet - Tekin's pattern
            saveOperatorRegistry(userName);
        }

        /// <summary>
        /// Registry'ye operatör bilgilerini kaydet - Tekin's pattern
        /// </summary>
        private void saveOperatorRegistry(string userName)
        {
            try
            {
                string regPath = GlobalVariables.registryPath;
                
                t.Registry_Write(regPath, "LastOperator", userName);
                t.Registry_Write(regPath, "ApiUrl", apiBaseUrl);
                t.Registry_Write(regPath, "LastLoginTime", DateTime.Now.ToString());
                
                debugMessage("Operatör bilgileri registry'ye kaydedildi");
            }
            catch (Exception ex)
            {
                debugMessage($"Registry kayıt hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Registry'den operatör bilgilerini oku - Tekin's pattern (GetUserRegistry benzeri)
        /// </summary>
        public void getOperatorRegistry()
        {
            try
            {
                string regPath = GlobalVariables.registryPath;
                
                GlobalVariables.Instance.userRegister.WhatsAppLastOperator = t.Registry_Read(regPath, "LastOperator", "");
                GlobalVariables.Instance.userRegister.WhatsAppApiUrl = t.Registry_Read(regPath, "ApiUrl", "http://localhost:5000");
                GlobalVariables.Instance.whatsAppConfig.ApiBaseUrl = GlobalVariables.Instance.userRegister.WhatsAppApiUrl;
                apiBaseUrl = GlobalVariables.Instance.whatsAppConfig.ApiBaseUrl;
                
                debugMessage("Operatör bilgileri registry'den okundu");
            }
            catch (Exception ex)
            {
                debugMessage($"Registry okuma hatası: {ex.Message}");
            }
        }

        #endregion

        #region Token Management

        /// <summary>
        /// Token geçerlilik kontrolü
        /// </summary>
        public bool isTokenValid()
        {
            return GlobalVariables.isTokenValid();
        }

        /// <summary>
        /// Token yenileme
        /// </summary>
        public async Task<bool> refreshToken()
        {
            try
            {
                if (string.IsNullOrEmpty(GlobalVariables.tWhatsAppOperator.JwtToken))
                    return false;

                string url = $"{apiBaseUrl}/auth/refresh";
                var headers = new System.Collections.Generic.Dictionary<string, string>
                {
                    ["Authorization"] = $"Bearer {GlobalVariables.tWhatsAppOperator.JwtToken}"
                };

                string response = await t.HTTP_POST(url, "{}", headers);
                
                if (isNotNull(response))
                {
                    var refreshResponse = JsonSerializer.Deserialize<LoginResponse>(response);
                    if (refreshResponse != null && isNotNull(refreshResponse.Token))
                    {
                        GlobalVariables.tWhatsAppOperator.JwtToken = refreshResponse.Token;
                        GlobalVariables.tWhatsAppOperator.TokenExpiry = DateTime.UtcNow.AddMinutes(480);
                        
                        debugMessage("Token yenilendi");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                debugMessage($"Token yenileme hatası: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Password Reset - Tekin's pattern

        /// <summary>
        /// Şifre sıfırlama talebi - Tekin's pattern (btn_SifremiUnuttumClick benzeri)
        /// </summary>
        public async Task<bool> sifremiUnuttum(string userName)
        {
            try
            {
                debugMessage($"Şifre sıfırlama talebi: {userName}");

                var resetRequest = new { UserName = userName };
                string jsonContent = JsonSerializer.Serialize(resetRequest);
                string url = $"{apiBaseUrl}/auth/reset/start";

                string response = await t.HTTP_POST(url, jsonContent);

                if (isNotNull(response))
                {
                    var resetResponse = JsonSerializer.Deserialize<PasswordResetResponse>(response);
                    if (resetResponse != null)
                    {
                        infoMessage($"Şifre sıfırlama kodu gönderildi.\n\nKod: {resetResponse.Token}\n\n(Gerçek uygulamada bu kod SMS/Email ile gönderilir)");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                errorMessage("Şifre sıfırlama hatası", ex);
                return false;
            }
        }

        /// <summary>
        /// Şifre sıfırlama onayı
        /// </summary>
        public async Task<bool> sifreResetOnay(string token, string newPassword)
        {
            try
            {
                debugMessage("Şifre sıfırlama onayı");

                var confirmRequest = new 
                { 
                    Token = token, 
                    NewPassword = newPassword 
                };
                
                string jsonContent = JsonSerializer.Serialize(confirmRequest);
                string url = $"{apiBaseUrl}/auth/reset/confirm";

                string response = await t.HTTP_POST(url, jsonContent);

                if (isNotNull(response))
                {
                    infoMessage("Şifreniz başarıyla güncellenmiştir.");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                errorMessage("Şifre güncelleme hatası", ex);
                return false;
            }
        }

        #endregion

        #region Logout

        /// <summary>
        /// Çıkış yap - Tekin's pattern
        /// </summary>
        public void operatorLogout()
        {
            debugMessage("Operatör çıkışı");

            // Global değişkenleri temizle - Tekin's pattern
            GlobalVariables.clearWhatsAppOperatorInfo();
            GlobalVariables.SP_UserLOGIN = false;
        }

        #endregion

        #region Response Models

        private class LoginResponse
        {
            public string Token { get; set; }
            public int OperatorId { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public int FirmId { get; set; }
        }

        private class PasswordResetResponse
        {
            public string Token { get; set; }
            public string Message { get; set; }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            httpClient?.Dispose();
        }

        #endregion
    }
}
