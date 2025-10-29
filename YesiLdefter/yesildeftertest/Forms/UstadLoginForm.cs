using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using yesildeftertest.Authentication;
using yesildeftertest.Models;

namespace yesildeftertest.Forms
{
    /// <summary>
    /// Ustad API integrated login form with tenant selection
    /// </summary>
    public partial class UstadLoginForm : Form
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private bool _isLoading = false;
        private List<FirmInfo> _userFirms = new List<FirmInfo>();
        private AuthenticationResult _authResult;
        private string _turnstileToken = "";

        #endregion

        #region Constructor

        public UstadLoginForm(string apiBaseUrl = "http://localhost:5000")
        {
            _apiBaseUrl = apiBaseUrl;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiBaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            InitializeComponent();
            SetupForm();
        }

        #endregion

        #region Setup

        private void SetupForm()
        {
            this.Text = "YesiLdefter - Kullanıcı Girişi";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(500, 500);
            this.BackColor = ColorTranslator.FromHtml("#F5F7F9");

            // Setup button colors
            btnLogin.BackColor = ColorTranslator.FromHtml("#295C00");
            btnLogin.ForeColor = Color.White;
            btnNext.BackColor = ColorTranslator.FromHtml("#007ACC");
            btnNext.ForeColor = Color.White;

            // Setup events
            btnLogin.Click += BtnLogin_Click;
            btnNext.Click += BtnNext_Click;
            btnCancel.Click += BtnCancel_Click;
            
            // Initialize Turnstile
            InitializeTurnstile();
            lnkForgotPassword.Click += LnkForgotPassword_Click;
            txtPassword.KeyPress += TxtPassword_KeyPress;
            txtEmail.KeyPress += TxtEmail_KeyPress;

            // Initially hide tenant selection
            pnlTenantSelection.Visible = false;
            btnNext.Visible = false;
        }

        #endregion

        #region Event Handlers

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            await PerformLogin();
        }

        private async void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                await PerformLogin();
            }
        }

        private async void TxtEmail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                await PerformLogin();
            }
        }

        private async void BtnNext_Click(object sender, EventArgs e)
        {
            await SelectTenant();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void LnkForgotPassword_Click(object sender, EventArgs e)
        {
            ShowPasswordResetDialog();
        }

        #endregion

        #region Login Logic

        /// <summary>
        /// Perform login operation
        /// </summary>
        private async Task PerformLogin()
        {
            if (_isLoading)
                return;

            var email = txtEmail.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("E-posta adresi boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Şifre boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(_turnstileToken))
            {
                MessageBox.Show("Lütfen güvenlik doğrulamasını tamamlayın.\n\nEğer widget görünmüyorsa, 'Tamam' butonuna basarak devam edebilirsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // For testing purposes, set a mock token if user clicks OK
                _turnstileToken = "MOCK_TOKEN_FOR_TESTING";
            }

            SetLoadingState(true);

            try
            {
                var loginRequest = new
                {
                    UserName = email,
                    Password = password,
                    TurnstileToken = _turnstileToken
                };

                var response = await _httpClient.PostAsJsonAsync("/auth/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<AuthenticationManager.LoginResponse>();
                    
                    if (loginResponse != null)
                    {
                        _authResult = AuthenticationResult.Success(loginResponse);
                        
                        // Get user firms for tenant selection
                        await LoadUserFirms(loginResponse.UserGUID);
                        
                        if (_userFirms.Count == 1)
                        {
                            // Only one firm, auto-select
                            await SelectTenant(_userFirms[0]);
                        }
                        else if (_userFirms.Count > 1)
                        {
                            // Multiple firms, show selection
                            ShowTenantSelection();
                        }
                        else
                        {
                            MessageBox.Show("Kullanıcıya atanmış firma bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Giriş başarısız: {errorContent}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Giriş sırasında hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        /// <summary>
        /// Load user firms for tenant selection
        /// </summary>
        private async Task LoadUserFirms(string userGUID)
        {
            try
            {
                // Call the API to get user firms
                var response = await _httpClient.GetAsync($"/UstadFirm/user/{userGUID}");
                
                if (response.IsSuccessStatusCode)
                {
                    var firms = await response.Content.ReadFromJsonAsync<FirmInfo[]>();
                    _userFirms = firms?.ToList() ?? new List<FirmInfo>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Firma listesi alınamadı: {errorContent}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _userFirms = new List<FirmInfo>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Firma listesi yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _userFirms = new List<FirmInfo>();
            }
        }

        /// <summary>
        /// Show tenant selection panel
        /// </summary>
        private void ShowTenantSelection()
        {
            pnlLogin.Visible = false;
            pnlTenantSelection.Visible = true;
            btnNext.Visible = true;
            btnLogin.Visible = false;

            // Populate firm list
            lstFirms.Items.Clear();
            foreach (var firm in _userFirms)
            {
                var item = new ListViewItem(firm.FirmLongName);
                item.SubItems.Add(firm.FirmGUID);
                item.Tag = firm;
                lstFirms.Items.Add(item);
            }

            if (lstFirms.Items.Count > 0)
            {
                lstFirms.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// Select tenant and proceed
        /// </summary>
        private async Task SelectTenant(FirmInfo firm = null)
        {
            if (firm == null)
            {
                if (lstFirms.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Lütfen bir firma seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                firm = (FirmInfo)lstFirms.SelectedItems[0].Tag;
            }

            try
            {
                // Create QR data from actual authentication result
                var qrData = new QRCodePayload
                {
                    firmGUID = firm.FirmGUID,
                    userGUID = _authResult.LoginData.UserGUID,
                    tcNoTelefonNo = "7470", // This should come from user data
                    userId = _authResult.LoginData.OperatorId,
                    isActive = true,
                    userFullName = _authResult.LoginData.FullName,
                    userFirstName = _authResult.LoginData.FullName.Split(' ')[0],
                    userLastName = _authResult.LoginData.FullName.Split(' ').Length > 1 ? string.Join(" ", _authResult.LoginData.FullName.Split(' ').Skip(1)) : "",
                    userEMail = txtEmail.Text.Trim()
                };
                
                // Store authentication info
                StoreAuthenticationInfo(_authResult.LoginData, firm, qrData);
                
                // Show QR code form
                ShowQRCodeForm(qrData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Firma seçimi sırasında hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Show QR code form
        /// </summary>
        private void ShowQRCodeForm(QRCodePayload qrData)
        {
            using (var qrForm = new QRCodeForm(qrData))
            {
                qrForm.ShowDialog(this);
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Store authentication information
        /// </summary>
        private void StoreAuthenticationInfo(AuthenticationManager.LoginResponse loginData, FirmInfo firm, QRCodePayload qrData)
        {
            // Update global variables
            GlobalVariables.tWhatsAppOperator.OperatorId = loginData.OperatorId;
            GlobalVariables.tWhatsAppOperator.OperatorUserName = txtEmail.Text.Trim();
            GlobalVariables.tWhatsAppOperator.OperatorFullName = loginData.FullName;
            GlobalVariables.tWhatsAppOperator.OperatorRole = loginData.Role;
            GlobalVariables.tWhatsAppOperator.JwtToken = loginData.Token;
            GlobalVariables.tWhatsAppOperator.FirmId = firm.FirmId;
            GlobalVariables.tWhatsAppOperator.TokenExpiry = DateTime.UtcNow.AddMinutes(480);
            GlobalVariables.tWhatsAppOperator.IsConnected = true;

            // Store QR data
            GlobalVariables.tWhatsAppOperator.FirmGUID = firm.FirmGUID;
            GlobalVariables.tWhatsAppOperator.UserGUID = qrData.userGUID;

            // Update global state
            GlobalVariables.SP_UserLOGIN = true;
            GlobalVariables.SP_UserIN = true;
            GlobalVariables.SP_WhatsAppConnected = true;
        }

        /// <summary>
        /// Show password reset dialog
        /// </summary>
        private void ShowPasswordResetDialog()
        {
            var email = txtEmail.Text.Trim();
            
            // For now, show a simple message
            MessageBox.Show($"Şifre sıfırlama özelliği henüz aktif değil.\nE-posta: {email}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Set loading state for UI
        /// </summary>
        private void SetLoadingState(bool loading)
        {
            _isLoading = loading;
            
            btnLogin.Enabled = !loading;
            btnNext.Enabled = !loading;
            btnCancel.Enabled = !loading;
            txtEmail.Enabled = !loading;
            txtPassword.Enabled = !loading;
            lnkForgotPassword.Enabled = !loading;

            btnLogin.Text = loading ? "Giriş yapılıyor..." : "Giriş Yap";
            this.Cursor = loading ? Cursors.WaitCursor : Cursors.Default;
        }

        #endregion

        #region Data Models


        public class FirmInfo
        {
            public int FirmId { get; set; }
            public string FirmGUID { get; set; }
            public string FirmLongName { get; set; }
            public int UserCount { get; set; }
        }


        #endregion

        #region Turnstile

        /// <summary>
        /// Initialize Cloudflare Turnstile widget
        /// </summary>
        private void InitializeTurnstile()
        {
            try
            {
                // Make sure the WebBrowser is visible
                webBrowserTurnstile.Visible = true;
                webBrowserTurnstile.ScriptErrorsSuppressed = true;
                
                // Create HTML content for Turnstile widget
                string htmlContent = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <script src='https://challenges.cloudflare.com/turnstile/v0/api.js' async defer></script>
    <style>
        body { margin: 0; padding: 5px; font-family: Arial, sans-serif; background: #ffffff; }
        .turnstile-container { text-align: center; }
        .cf-turnstile { margin: 0 auto; }
    </style>
</head>
<body>
    <div class='turnstile-container'>
        <div class='cf-turnstile' 
             data-sitekey='0x4AAAAAAABkMYinukE8nzY' 
             data-callback='onTurnstileSuccess'
             data-error-callback='onTurnstileError'
             data-expired-callback='onTurnstileExpired'
             data-theme='light'
             data-size='normal'>
        </div>
    </div>
    <script>
        function onTurnstileSuccess(token) {
            try {
                window.external.notify('SUCCESS:' + token);
            } catch(e) {
                console.log('Token received:', token);
            }
        }
        function onTurnstileError(error) {
            try {
                window.external.notify('ERROR:' + error);
            } catch(e) {
                console.log('Turnstile error:', error);
            }
        }
        function onTurnstileExpired() {
            try {
                window.external.notify('EXPIRED');
            } catch(e) {
                console.log('Turnstile expired');
            }
        }
    </script>
</body>
</html>";

                webBrowserTurnstile.DocumentText = htmlContent;
            }
            catch (Exception ex)
            {
                // If Turnstile fails to load, show error but continue
                lblTurnstile.Text = "Güvenlik doğrulaması yüklenemedi (devam edebilirsiniz)";
                lblTurnstile.ForeColor = System.Drawing.Color.Orange;
                webBrowserTurnstile.Visible = false;
                _turnstileToken = "MOCK_TOKEN_FOR_TESTING"; // Allow login without Turnstile for testing
            }
        }

        /// <summary>
        /// Handle Turnstile widget events
        /// </summary>
        private void WebBrowserTurnstile_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                // Enable script errors to be handled
                webBrowserTurnstile.ScriptErrorsSuppressed = true;
            }
            catch (Exception ex)
            {
                // Ignore errors
            }
        }

        /// <summary>
        /// Set Turnstile token (called from JavaScript)
        /// </summary>
        public void SetTurnstileToken(string token)
        {
            _turnstileToken = token;
            lblTurnstile.Text = "✓ Güvenlik doğrulaması tamamlandı";
            lblTurnstile.ForeColor = System.Drawing.Color.Green;
        }

        /// <summary>
        /// Handle WebBrowser notifications from Turnstile
        /// </summary>
        private void WebBrowserTurnstile_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            // Handle any navigation if needed
        }

        #endregion

        #region Cleanup

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
