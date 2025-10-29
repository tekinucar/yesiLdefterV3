using yesildeftertest.Authentication;
using yesildeftertest.Services;
using yesildeftertest.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yesildeftertest
{
    /// <summary>
    /// Main application entry point for Ustad WhatsApp Desktop Integration
    /// Handles application initialization and authentication flow
    /// </summary>
    public partial class Form1 : Form
    {
        #region Fields

        private AuthenticationManager? _authManager;
        private WhatsAppService? _whatsAppService;
        private readonly tToolBox t = new tToolBox();
        private readonly tBase baseHelper = new tBase();

        #endregion

        #region Constructor

        public Form1(string[]? args = null)
        {
            InitializeComponent();

            this.Text = "Ustad WhatsApp Desktop";
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            // Initialize services
            InitializeServices();

            // Process command line arguments
            ProcessArguments(args);

            // Setup form events
            this.Load += Form1_Load;
            this.Shown += Form1_Shown;
            this.FormClosing += Form1_FormClosing;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize core services for WhatsApp integration
        /// </summary>
        private void InitializeServices()
        {
            try
            {
                baseHelper.debugMessage("Initializing services");

                // Initialize authentication manager
                _authManager = new AuthenticationManager();
                _authManager.AuthenticationCompleted += OnAuthenticationCompleted;
                _authManager.AuthenticationFailed += OnAuthenticationFailed;

                // Initialize WhatsApp service
                _whatsAppService = new WhatsAppService(_authManager);
                _whatsAppService.StatusChanged += OnWhatsAppStatusChanged;
                _whatsAppService.ConversationUpdated += OnConversationUpdated;
                _whatsAppService.MessageReceived += OnMessageReceived;

                baseHelper.debugMessage("Services initialized successfully");
            }
            catch (Exception ex)
            {
                baseHelper.debugMessage($"Service initialization error: {ex.Message}");
                MessageBox.Show($"Servis başlatma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Process command line arguments
        /// </summary>
        private void ProcessArguments(string[]? args)
        {
            if (args != null && args.Length > 0)
            {
                baseHelper.debugMessage($"Command line arguments: {string.Join(", ", args)}");
                
                // Process specific arguments if needed
                foreach (var arg in args)
                {
                    if (arg.StartsWith("/"))
                    {
                        baseHelper.debugMessage($"Processing argument: {arg}");
                    }
                }
            }
        }

        #endregion

        #region Form Events

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                baseHelper.debugMessage("Form1_Load started");
                
                // Initialize WhatsApp configuration
                GlobalVariables.tWhatsAppConfig.ApiBaseUrl = "http://143.198.228.153:5000";
                GlobalVariables.tWhatsAppConfig.EnableNotifications = true;
                GlobalVariables.tWhatsAppConfig.EnableSounds = true;
                GlobalVariables.tWhatsAppConfig.TurnstileSiteKey = ""; // Set your Turnstile site key here

                // Remove direct DB connections; use API layer instead
                GlobalVariables.active_DB.whatsAppMSSQLConn = string.Empty;
                GlobalVariables.active_DB.managerMSSQLConn = string.Empty;

                baseHelper.debugMessage("Configuration initialized");
                
                // Start authentication flow
                await StartAuthenticationFlow();
            }
            catch (Exception ex)
            {
                baseHelper.debugMessage($"Form1_Load error: {ex.Message}");
                MessageBox.Show($"Uygulama başlatma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            baseHelper.debugMessage("Form1_Shown - Application ready");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                baseHelper.debugMessage("Form1_FormClosing - Cleaning up");
                
                // Cleanup services
                _whatsAppService?.Dispose();
                _authManager?.Dispose();
                
                baseHelper.debugMessage("Cleanup completed");
            }
            catch (Exception ex)
            {
                baseHelper.debugMessage($"Form1_FormClosing error: {ex.Message}");
            }
        }

        #endregion

        #region Authentication Flow

        /// <summary>
        /// Start the authentication flow
        /// </summary>
        private async Task StartAuthenticationFlow()
        {
            try
            {
                baseHelper.debugMessage("Starting authentication flow");

                // Check if user is already authenticated
                if (GlobalVariables.tWhatsAppOperator.IsAuthenticated)
                {
                    baseHelper.debugMessage("User already authenticated");
                    OnAuthenticationCompleted(this, EventArgs.Empty);
                    return;
                }

                // Show Ustad login form
                using (var loginForm = new Forms.UstadLoginForm("http://localhost:5000"))
                {
                    var result = loginForm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        baseHelper.debugMessage("Login successful");
                        OnAuthenticationCompleted(this, EventArgs.Empty);
                    }
                    else
                    {
                        baseHelper.debugMessage("Login cancelled or failed");
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                baseHelper.debugMessage($"Authentication flow error: {ex.Message}");
                MessageBox.Show($"Giriş hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        #endregion

        #region Event Handlers

        private async void OnAuthenticationCompleted(object sender, EventArgs e)
        {
            try
            {
                baseHelper.debugMessage("Authentication completed successfully");
                
                // Set WhatsApp as connected
                GlobalVariables.SP_WhatsAppConnected = true;
                
                // Start WhatsApp service
                if (_whatsAppService != null)
                {
                    await _whatsAppService.StartAsync();
                }
                
                // Show main WhatsApp form
                ShowMainWhatsAppForm();
            }
            catch (Exception ex)
            {
                baseHelper.debugMessage($"OnAuthenticationCompleted error: {ex.Message}");
                MessageBox.Show($"Kimlik doğrulama sonrası hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnAuthenticationFailed(object sender, string error)
        {
            baseHelper.debugMessage($"Authentication failed: {error}");
            MessageBox.Show($"Giriş başarısız: {error}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void OnWhatsAppStatusChanged(object sender, string status)
        {
            baseHelper.debugMessage($"WhatsApp status: {status}");
        }

        private void OnConversationUpdated(object sender, ConversationUpdatedEventArgs e)
        {
            baseHelper.debugMessage($"Conversation updated: {e.Conversation.CustomerName}");
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            baseHelper.debugMessage($"Message received: {e.Message.Body}");
        }

        #endregion

        #region UI Methods

        /// <summary>
        /// Show the main WhatsApp form
        /// </summary>
        private void ShowMainWhatsAppForm()
        {
            try
            {
                baseHelper.debugMessage("Showing main WhatsApp form");
                
                using (var mainForm = new Forms.WhatsAppMainForm(_whatsAppService))
                {
                    mainForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                baseHelper.debugMessage($"ShowMainWhatsAppForm error: {ex.Message}");
                MessageBox.Show($"Ana form hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

    }
}