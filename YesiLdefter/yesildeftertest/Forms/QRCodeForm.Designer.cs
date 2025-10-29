namespace yesildeftertest.Forms
{
    partial class QRCodeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblInstructions = new System.Windows.Forms.Label();
            this._qrPictureBox = new System.Windows.Forms.PictureBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblJsonTitle = new System.Windows.Forms.Label();
            this._jsonTextBox = new System.Windows.Forms.TextBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._qrPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#295C00");
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(460, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "WhatsApp Mobile Girişi - QR Kod";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblInstructions
            // 
            this.lblInstructions.AutoSize = true;
            this.lblInstructions.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblInstructions.ForeColor = System.Drawing.ColorTranslator.FromHtml("#666666");
            this.lblInstructions.Location = new System.Drawing.Point(20, 60);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(460, 19);
            this.lblInstructions.TabIndex = 1;
            this.lblInstructions.Text = "Mobil uygulamanızla bu QR kodu tarayın veya JSON verisini kopyalayın";
            this.lblInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _qrPictureBox
            // 
            this._qrPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._qrPictureBox.Location = new System.Drawing.Point(150, 90);
            this._qrPictureBox.Name = "_qrPictureBox";
            this._qrPictureBox.Size = new System.Drawing.Size(200, 200);
            this._qrPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._qrPictureBox.TabIndex = 2;
            this._qrPictureBox.TabStop = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.Green;
            this.lblStatus.Location = new System.Drawing.Point(20, 300);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(100, 15);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "QR Kod oluşturuldu";
            // 
            // lblJsonTitle
            // 
            this.lblJsonTitle.AutoSize = true;
            this.lblJsonTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblJsonTitle.Location = new System.Drawing.Point(20, 330);
            this.lblJsonTitle.Name = "lblJsonTitle";
            this.lblJsonTitle.Size = new System.Drawing.Size(100, 19);
            this.lblJsonTitle.TabIndex = 4;
            this.lblJsonTitle.Text = "JSON Verisi:";
            // 
            // _jsonTextBox
            // 
            this._jsonTextBox.Font = new System.Drawing.Font("Consolas", 8F);
            this._jsonTextBox.Location = new System.Drawing.Point(20, 355);
            this._jsonTextBox.Multiline = true;
            this._jsonTextBox.Name = "_jsonTextBox";
            this._jsonTextBox.ReadOnly = true;
            this._jsonTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._jsonTextBox.Size = new System.Drawing.Size(460, 120);
            this._jsonTextBox.TabIndex = 5;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnRefresh.Location = new System.Drawing.Point(200, 490);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(80, 35);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "Yenile";
            this.btnRefresh.UseVisualStyleBackColor = false;
            // 
            // btnCopy
            // 
            this.btnCopy.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCopy.Location = new System.Drawing.Point(300, 490);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(80, 35);
            this.btnCopy.TabIndex = 7;
            this.btnCopy.Text = "Kopyala";
            this.btnCopy.UseVisualStyleBackColor = false;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnClose.Location = new System.Drawing.Point(400, 490);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 35);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "Kapat";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // QRCodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 600);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this._jsonTextBox);
            this.Controls.Add(this.lblJsonTitle);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this._qrPictureBox);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.lblTitle);
            this.Name = "QRCodeForm";
            this.Text = "WhatsApp Mobile Girişi - QR Kod";
            ((System.ComponentModel.ISupportInitialize)(this._qrPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblInstructions;
        private System.Windows.Forms.PictureBox _qrPictureBox;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblJsonTitle;
        private System.Windows.Forms.TextBox _jsonTextBox;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnClose;
    }
}
