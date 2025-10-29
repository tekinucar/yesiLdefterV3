using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using QRCoder;

namespace yesildeftertest.Forms
{
    /// <summary>
    /// QR Code display form for mobile authentication
    /// </summary>
    public partial class QRCodeForm : Form
    {
        #region Fields

        private readonly QRCodePayload _qrData;
        private System.Windows.Forms.Timer _refreshTimer;

        #endregion

        #region Constructor

        public QRCodeForm(QRCodePayload qrData)
        {
            _qrData = qrData ?? throw new ArgumentNullException(nameof(qrData));
            
            InitializeComponent();
            SetupForm();
            GenerateQRCode();
        }

        #endregion

        #region Setup

        private void SetupForm()
        {
            this.Text = "WhatsApp Mobile Girişi - QR Kod";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(500, 600);
            this.BackColor = ColorTranslator.FromHtml("#F5F7F9");

            // Setup button colors
            btnClose.BackColor = ColorTranslator.FromHtml("#6C757D");
            btnClose.ForeColor = Color.White;
            btnRefresh.BackColor = ColorTranslator.FromHtml("#007ACC");
            btnRefresh.ForeColor = Color.White;
            btnCopy.BackColor = ColorTranslator.FromHtml("#28A745");
            btnCopy.ForeColor = Color.White;

            // Setup events
            btnClose.Click += BtnClose_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnCopy.Click += BtnCopy_Click;

            // Setup refresh timer (refresh every 30 seconds)
            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 30000; // 30 seconds
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();

            // Setup form closing
            this.FormClosing += QRCodeForm_FormClosing;
        }

        #endregion

        #region Event Handlers

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            GenerateQRCode();
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (_jsonTextBox != null)
            {
                _jsonTextBox.SelectAll();
                _jsonTextBox.Copy();
                MessageBox.Show("JSON verisi panoya kopyalandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            GenerateQRCode();
        }

        private void QRCodeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
        }

        #endregion

        #region QR Code Generation

        /// <summary>
        /// Generate QR code from the payload data
        /// </summary>
        private void GenerateQRCode()
        {
            try
            {
                // Convert QR data to JSON
                var jsonData = JsonSerializer.Serialize(_qrData, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                // Update JSON text box
                if (_jsonTextBox != null)
                {
                    _jsonTextBox.Text = jsonData;
                }

                // Generate QR code
                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(jsonData, QRCodeGenerator.ECCLevel.Q);
                    using (var qrCode = new QRCode(qrCodeData))
                    {
                        var qrCodeImage = qrCode.GetGraphic(20);
                        
                        // Update QR code picture box
                        if (_qrPictureBox != null)
                        {
                            _qrPictureBox.Image?.Dispose();
                            _qrPictureBox.Image = qrCodeImage;
                        }
                    }
                }

                // Update status
                lblStatus.Text = $"QR Kod oluşturuldu - {DateTime.Now:HH:mm:ss}";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"QR Kod oluşturma hatası: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Save QR code as image file
        /// </summary>
        public void SaveQRCodeAsImage(string filePath)
        {
            try
            {
                if (_qrPictureBox?.Image != null)
                {
                    _qrPictureBox.Image.Save(filePath, ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"QR kod kaydetme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Get QR code as base64 string
        /// </summary>
        public string GetQRCodeAsBase64()
        {
            try
            {
                if (_qrPictureBox?.Image != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        _qrPictureBox.Image.Save(ms, ImageFormat.Png);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"QR kod base64 dönüştürme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return string.Empty;
        }

        #endregion

        #region Cleanup

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _refreshTimer?.Stop();
                _refreshTimer?.Dispose();
                _qrPictureBox?.Image?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }

    /// <summary>
    /// QR Code payload model
    /// </summary>
    public class QRCodePayload
    {
        public string firmGUID { get; set; }
        public string userGUID { get; set; }
        public string tcNoTelefonNo { get; set; }
        public int userId { get; set; }
        public bool isActive { get; set; }
        public string userFullName { get; set; }
        public string userFirstName { get; set; }
        public string userLastName { get; set; }
        public string userEMail { get; set; }
    }
}
