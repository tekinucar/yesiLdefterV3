using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using yesildeftertest.Services;

namespace yesildeftertest.Forms
{
    /// <summary>
    /// Main WhatsApp customer service interface
    /// Clean implementation focused on core functionality
    /// </summary>
    public partial class WhatsAppMainForm : Form
    {
        #region tanımlar

        private readonly WhatsAppService _whatsAppService;
        private ConversationModel _selectedConversation;
        private System.Windows.Forms.Timer _statusUpdateTimer;

        #endregion

        #region constructor

        public WhatsAppMainForm(WhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService ?? throw new ArgumentNullException(nameof(whatsAppService));
            
            InitializeComponent();
            SetupForm();
            SetupEvents();
        }

        #endregion

        #region Setup

        private void SetupForm()
        {
            this.Text = $"Ustad WhatsApp - {GlobalVariables.tWhatsAppOperator.OperatorFullName} ({GlobalVariables.tWhatsAppOperator.OperatorRole})";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = ColorTranslator.FromHtml("#F5F7F9");

            // Setup status timer
            _statusUpdateTimer = new System.Windows.Forms.Timer();
            _statusUpdateTimer.Interval = 1000;
            _statusUpdateTimer.Tick += UpdateStatusDisplay;
            _statusUpdateTimer.Start();

            // Configure role-based UI
            ConfigureRoleBasedUI();
        }

        private void SetupEvents()
        {
            // WhatsApp service events
            _whatsAppService.MessageReceived += OnMessageReceived;
            _whatsAppService.ConversationUpdated += OnConversationUpdated;
            _whatsAppService.StatusChanged += OnStatusChanged;

            // Form events
            this.Load += WhatsAppMainForm_Load;
            this.FormClosing += WhatsAppMainForm_FormClosing;

            // Control events
            btnSendMessage.Click += BtnSendMessage_Click;
            txtMessage.KeyPress += TxtMessage_KeyPress;
            gridConversations.SelectionChanged += GridConversations_SelectionChanged;
        }

        private void ConfigureRoleBasedUI()
        {
            // Show admin features only for admin users
            bool isAdmin = GlobalVariables.tWhatsAppOperator.OperatorRole == "Admin";
            
            if (toolStrip1.Items["btnAdminPanel"] is ToolStripButton adminBtn)
            {
                adminBtn.Visible = isAdmin;
            }

            // Update operator info
            if (statusStrip1.Items["lblOperatorInfo"] is ToolStripStatusLabel operatorLabel)
            {
                operatorLabel.Text = $"{GlobalVariables.tWhatsAppOperator.OperatorFullName} - {GlobalVariables.tWhatsAppOperator.OperatorRole}";
            }
        }

        #endregion

        #region Form Events

        private async void WhatsAppMainForm_Load(object sender, EventArgs e)
        {
            await RefreshData();
        }

        private async void WhatsAppMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _statusUpdateTimer?.Stop();
            
            // Ask for confirmation
            var result = MessageBox.Show(
                "WhatsApp uygulamasını kapatmak istediğinizden emin misiniz?",
                "Çıkış Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            // Cleanup
            try
            {
                _whatsAppService?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cleanup error: {ex.Message}");
            }
        }

        #endregion

        #region Message Handling

        private async void BtnSendMessage_Click(object sender, EventArgs e)
        {
            await SendCurrentMessage();
        }

        private async void TxtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !Control.ModifierKeys.HasFlag(Keys.Shift))
            {
                e.Handled = true;
                await SendCurrentMessage();
            }
        }

        private async Task SendCurrentMessage()
        {
            if (_selectedConversation == null || string.IsNullOrWhiteSpace(txtMessage.Text))
                return;

            var message = txtMessage.Text.Trim();
            
            try
            {
                bool success = await _whatsAppService.SendMessageAsync(_selectedConversation.CustomerNumber, message);
                
                if (success)
                {
                    txtMessage.Clear();
                    txtMessage.Focus();
                }
                else
                {
                    MessageBox.Show("Mesaj gönderilemedi. Lütfen tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mesaj gönderme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Conversation Selection

        private async void GridConversations_SelectionChanged(object sender, EventArgs e)
        {
            if (gridConversations.SelectedRows.Count > 0)
            {
                var selectedRow = gridConversations.SelectedRows[0];
                var conversationId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                
                _selectedConversation = _whatsAppService.Conversations.FirstOrDefault(c => c.Id == conversationId);
                
                if (_selectedConversation != null)
                {
                    // Update UI
                    UpdateSelectedConversationUI();
                    
                    // Load messages
                    await _whatsAppService.LoadConversationMessagesAsync(conversationId);
                }
            }
        }

        private void UpdateSelectedConversationUI()
        {
            if (_selectedConversation == null)
                return;

            // Update customer info
            lblSelectedCustomer.Text = $"Müşteri: {_selectedConversation.CustomerNumber}";
            if (!string.IsNullOrEmpty(_selectedConversation.CustomerName))
            {
                lblSelectedCustomer.Text += $" ({_selectedConversation.CustomerName})";
            }

            // Enable message controls
            txtMessage.Enabled = true;
            btnSendMessage.Enabled = true;
            txtMessage.Focus();

            // Update conversation status
            lblConversationStatus.Text = $"Durum: {_selectedConversation.DisplayStatus}";
        }

        #endregion

        #region Data Refresh

        private async Task RefreshData()
        {
            try
            {
                // Refresh conversations
                UpdateConversationsGrid();
                
                // Update status
                UpdateConnectionStatus();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Refresh data error: {ex.Message}");
            }
        }

        private void UpdateConversationsGrid()
        {
            try
            {
                var bindingSource = new BindingSource();
                bindingSource.DataSource = _whatsAppService.Conversations;
                gridConversations.DataSource = bindingSource;

                // Configure columns if not already configured
                if (gridConversations.Columns.Count > 0 && gridConversations.Columns[0].HeaderText != "Müşteri")
                {
                    ConfigureConversationsColumns();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update conversations grid error: {ex.Message}");
            }
        }

        private void ConfigureConversationsColumns()
        {
            gridConversations.Columns["Id"].Visible = false;
            gridConversations.Columns["CustomerNumber"].HeaderText = "Müşteri";
            gridConversations.Columns["CustomerNumber"].Width = 120;
            
            if (gridConversations.Columns.Contains("CustomerName"))
            {
                gridConversations.Columns["CustomerName"].HeaderText = "İsim";
                gridConversations.Columns["CustomerName"].Width = 100;
            }
            
            gridConversations.Columns["LastMessage"].HeaderText = "Son Mesaj";
            gridConversations.Columns["LastMessage"].Width = 200;
            
            gridConversations.Columns["DisplayStatus"].HeaderText = "Durum";
            gridConversations.Columns["DisplayStatus"].Width = 80;
            
            gridConversations.Columns["UnreadCount"].HeaderText = "Okunmamış";
            gridConversations.Columns["UnreadCount"].Width = 80;
        }

        private void UpdateConnectionStatus()
        {
            var statusLabel = statusStrip1.Items["lblStatus"] as ToolStripStatusLabel;
            if (statusLabel != null)
            {
                if (_whatsAppService.IsConnected)
                {
                    statusLabel.Text = "Bağlı";
                    statusLabel.ForeColor = Color.Green;
                }
                else
                {
                    statusLabel.Text = "Bağlantı Yok";
                    statusLabel.ForeColor = Color.Red;
                }
            }
        }

        #endregion

        #region WhatsApp Service Events

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            this.Invoke(() =>
            {
                // Update conversations grid
                UpdateConversationsGrid();
                
                // If this is the selected conversation, refresh messages
                if (_selectedConversation?.Id == e.Message.ConversationId)
                {
                    UpdateMessagesGrid();
                }

                // Show notification
                ShowMessageNotification(e.Message, e.Conversation);
            });
        }

        private void OnConversationUpdated(object sender, ConversationUpdatedEventArgs e)
        {
            this.Invoke(() =>
            {
                UpdateConversationsGrid();
            });
        }

        private void OnStatusChanged(object sender, string status)
        {
            this.Invoke(() =>
            {
                var statusLabel = statusStrip1.Items["lblStatus"] as ToolStripStatusLabel;
                if (statusLabel != null)
                {
                    statusLabel.Text = status;
                }
            });
        }

        #endregion

        #region UI Updates

        private void UpdateMessagesGrid()
        {
            try
            {
                var bindingSource = new BindingSource();
                bindingSource.DataSource = _whatsAppService.CurrentMessages;
                gridMessages.DataSource = bindingSource;

                // Scroll to bottom
                if (gridMessages.Rows.Count > 0)
                {
                    gridMessages.FirstDisplayedScrollingRowIndex = gridMessages.Rows.Count - 1;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update messages grid error: {ex.Message}");
            }
        }

        private void ShowMessageNotification(MessageModel message, ConversationModel conversation)
        {
            try
            {
                // Bring window to front
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
                
                this.Activate();
                this.BringToFront();

                // Play notification sound
                if (GlobalVariables.tWhatsAppConfig.EnableSounds)
                {
                    System.Media.SystemSounds.Beep.Play();
                }

                // Flash taskbar
                FlashWindow.Flash(this);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Notification error: {ex.Message}");
            }
        }

        private void UpdateStatusDisplay(object sender, EventArgs e)
        {
            UpdateConnectionStatus();
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _statusUpdateTimer?.Stop();
                _statusUpdateTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// Windows API helper for taskbar flashing
    /// </summary>
    public static class FlashWindow
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        public static void Flash(Form form)
        {
            try
            {
                var fInfo = new FLASHWINFO();
                fInfo.cbSize = Convert.ToUInt32(System.Runtime.InteropServices.Marshal.SizeOf(fInfo));
                fInfo.hwnd = form.Handle;
                fInfo.dwFlags = FLASHW_TRAY;
                fInfo.uCount = 3;
                fInfo.dwTimeout = 0;

                FlashWindowEx(ref fInfo);
            }
            catch
            {
                // Ignore flash errors
            }
        }

        private struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hwnd;
            public uint dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }

        private const uint FLASHW_TRAY = 2;
    }

    #endregion
}
