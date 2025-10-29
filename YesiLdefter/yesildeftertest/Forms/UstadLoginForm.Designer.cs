namespace yesildeftertest.Forms
{
    partial class UstadLoginForm
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
            pnlLogin = new Panel();
            lblTitle = new Label();
            lblEmail = new Label();
            txtEmail = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            chkRememberMe = new CheckBox();
            lnkForgotPassword = new LinkLabel();
            lblTurnstile = new Label();
            webBrowserTurnstile = new WebBrowser();
            btnLogin = new Button();
            btnCancel = new Button();
            pnlTenantSelection = new Panel();
            lblSelectFirm = new Label();
            lstFirms = new ListView();
            colFirmName = new ColumnHeader();
            colFirmGUID = new ColumnHeader();
            btnNext = new Button();
            button1 = new Button();
            buttonQR = new Button();
            pnlLogin.SuspendLayout();
            pnlTenantSelection.SuspendLayout();
            SuspendLayout();
            // 
            // pnlLogin
            // 
            pnlLogin.Controls.Add(buttonQR);
            pnlLogin.Controls.Add(button1);
            pnlLogin.Controls.Add(lblTitle);
            pnlLogin.Controls.Add(lblEmail);
            pnlLogin.Controls.Add(txtEmail);
            pnlLogin.Controls.Add(lblPassword);
            pnlLogin.Controls.Add(txtPassword);
            pnlLogin.Controls.Add(chkRememberMe);
            pnlLogin.Controls.Add(lnkForgotPassword);
            pnlLogin.Controls.Add(lblTurnstile);
            pnlLogin.Controls.Add(webBrowserTurnstile);
            pnlLogin.Controls.Add(btnLogin);
            pnlLogin.Controls.Add(btnCancel);
            pnlLogin.Dock = DockStyle.Fill;
            pnlLogin.Location = new Point(0, 0);
            pnlLogin.Margin = new Padding(4, 3, 4, 3);
            pnlLogin.Name = "pnlLogin";
            pnlLogin.Size = new Size(583, 577);
            pnlLogin.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(41, 92, 0);
            lblTitle.Location = new Point(58, 35);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(222, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "YesiLdefter için Giriş";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI", 10F);
            lblEmail.Location = new Point(58, 115);
            lblEmail.Margin = new Padding(4, 0, 4, 0);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(48, 19);
            lblEmail.TabIndex = 1;
            lblEmail.Text = "e-Mail";
            // 
            // txtEmail
            // 
            txtEmail.Font = new Font("Segoe UI", 12F);
            txtEmail.Location = new Point(58, 144);
            txtEmail.Margin = new Padding(4, 3, 4, 3);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(408, 29);
            txtEmail.TabIndex = 2;
            txtEmail.Text = "canberk.ucar.mail@gmail.com";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 10F);
            lblPassword.Location = new Point(58, 196);
            lblPassword.Margin = new Padding(4, 0, 4, 0);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(35, 19);
            lblPassword.TabIndex = 3;
            lblPassword.Text = "Şifre";
            // 
            // txtPassword
            // 
            txtPassword.Font = new Font("Segoe UI", 12F);
            txtPassword.Location = new Point(58, 225);
            txtPassword.Margin = new Padding(4, 3, 4, 3);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(408, 29);
            txtPassword.TabIndex = 4;
            txtPassword.Text = "*****";
            // 
            // chkRememberMe
            // 
            chkRememberMe.AutoSize = true;
            chkRememberMe.Font = new Font("Segoe UI", 9F);
            chkRememberMe.Location = new Point(58, 277);
            chkRememberMe.Margin = new Padding(4, 3, 4, 3);
            chkRememberMe.Name = "chkRememberMe";
            chkRememberMe.Size = new Size(87, 19);
            chkRememberMe.TabIndex = 5;
            chkRememberMe.Text = "Beni Hatırla";
            chkRememberMe.UseVisualStyleBackColor = true;
            // 
            // lnkForgotPassword
            // 
            lnkForgotPassword.AutoSize = true;
            lnkForgotPassword.Font = new Font("Segoe UI", 9F);
            lnkForgotPassword.Location = new Point(58, 312);
            lnkForgotPassword.Margin = new Padding(4, 0, 4, 0);
            lnkForgotPassword.Name = "lnkForgotPassword";
            lnkForgotPassword.Size = new Size(95, 15);
            lnkForgotPassword.TabIndex = 6;
            lnkForgotPassword.TabStop = true;
            lnkForgotPassword.Text = "Şifremi Unuttum";
            // 
            // lblTurnstile
            // 
            lblTurnstile.AutoSize = true;
            lblTurnstile.Font = new Font("Segoe UI", 9F);
            lblTurnstile.ForeColor = Color.FromArgb(102, 102, 102);
            lblTurnstile.Location = new Point(58, 300);
            lblTurnstile.Margin = new Padding(4, 0, 4, 0);
            lblTurnstile.Name = "lblTurnstile";
            lblTurnstile.Size = new Size(188, 15);
            lblTurnstile.TabIndex = 10;
            lblTurnstile.Text = "Güvenlik doğrulaması (Cloudflare)";
            // 
            // webBrowserTurnstile
            // 
            webBrowserTurnstile.Location = new Point(58, 323);
            webBrowserTurnstile.Margin = new Padding(4, 3, 4, 3);
            webBrowserTurnstile.MinimumSize = new Size(23, 23);
            webBrowserTurnstile.Name = "webBrowserTurnstile";
            webBrowserTurnstile.Size = new Size(467, 75);
            webBrowserTurnstile.TabIndex = 9;
            webBrowserTurnstile.DocumentCompleted += WebBrowserTurnstile_DocumentCompleted;
            // 
            // btnLogin
            // 
            btnLogin.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnLogin.Location = new Point(175, 404);
            btnLogin.Margin = new Padding(4, 3, 4, 3);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(117, 46);
            btnLogin.TabIndex = 7;
            btnLogin.Text = "Giriş Yap";
            btnLogin.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            btnCancel.Font = new Font("Segoe UI", 10F);
            btnCancel.Location = new Point(315, 404);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(93, 46);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Çıkış";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // pnlTenantSelection
            // 
            pnlTenantSelection.Controls.Add(lblSelectFirm);
            pnlTenantSelection.Controls.Add(lstFirms);
            pnlTenantSelection.Controls.Add(btnNext);
            pnlTenantSelection.Dock = DockStyle.Fill;
            pnlTenantSelection.Location = new Point(0, 0);
            pnlTenantSelection.Margin = new Padding(4, 3, 4, 3);
            pnlTenantSelection.Name = "pnlTenantSelection";
            pnlTenantSelection.Size = new Size(583, 577);
            pnlTenantSelection.TabIndex = 1;
            pnlTenantSelection.Visible = false;
            // 
            // lblSelectFirm
            // 
            lblSelectFirm.AutoSize = true;
            lblSelectFirm.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblSelectFirm.ForeColor = Color.FromArgb(41, 92, 0);
            lblSelectFirm.Location = new Point(58, 35);
            lblSelectFirm.Margin = new Padding(4, 0, 4, 0);
            lblSelectFirm.Name = "lblSelectFirm";
            lblSelectFirm.Size = new Size(144, 30);
            lblSelectFirm.TabIndex = 0;
            lblSelectFirm.Text = "Firma Seçimi";
            lblSelectFirm.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lstFirms
            // 
            lstFirms.Columns.AddRange(new ColumnHeader[] { colFirmName, colFirmGUID });
            lstFirms.FullRowSelect = true;
            lstFirms.GridLines = true;
            lstFirms.Location = new Point(58, 92);
            lstFirms.Margin = new Padding(4, 3, 4, 3);
            lstFirms.MultiSelect = false;
            lstFirms.Name = "lstFirms";
            lstFirms.Size = new Size(408, 230);
            lstFirms.TabIndex = 1;
            lstFirms.UseCompatibleStateImageBehavior = false;
            lstFirms.View = View.Details;
            // 
            // colFirmName
            // 
            colFirmName.Text = "Firma Adı";
            colFirmName.Width = 200;
            // 
            // colFirmGUID
            // 
            colFirmGUID.Text = "Firm GUID";
            colFirmGUID.Width = 140;
            // 
            // btnNext
            // 
            btnNext.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnNext.Location = new Point(233, 346);
            btnNext.Margin = new Padding(4, 3, 4, 3);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(117, 46);
            btnNext.TabIndex = 2;
            btnNext.Text = "İleri";
            btnNext.UseVisualStyleBackColor = false;
            btnNext.Visible = false;
            // 
            // button1
            // 
            button1.Location = new Point(175, 493);
            button1.Name = "button1";
            button1.Size = new Size(117, 23);
            button1.TabIndex = 13;
            button1.Text = "WhatsApp ";
            button1.UseVisualStyleBackColor = true;
            // 
            // buttonQR
            // 
            buttonQR.Location = new Point(315, 493);
            buttonQR.Name = "buttonQR";
            buttonQR.Size = new Size(93, 23);
            buttonQR.TabIndex = 14;
            buttonQR.Text = "QR Oluştur";
            buttonQR.UseVisualStyleBackColor = true;
            // 
            // UstadLoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(583, 577);
            Controls.Add(pnlLogin);
            Controls.Add(pnlTenantSelection);
            Margin = new Padding(4, 3, 4, 3);
            Name = "UstadLoginForm";
            Text = "YesiLdefter - Kullanıcı Girişi";
            pnlLogin.ResumeLayout(false);
            pnlLogin.PerformLayout();
            pnlTenantSelection.ResumeLayout(false);
            pnlTenantSelection.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlLogin;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkRememberMe;
        private System.Windows.Forms.LinkLabel lnkForgotPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.WebBrowser webBrowserTurnstile;
        private System.Windows.Forms.Label lblTurnstile;
        private System.Windows.Forms.Panel pnlTenantSelection;
        private System.Windows.Forms.Label lblSelectFirm;
        private System.Windows.Forms.ListView lstFirms;
        private System.Windows.Forms.ColumnHeader colFirmName;
        private System.Windows.Forms.ColumnHeader colFirmGUID;
        private System.Windows.Forms.Button btnNext;
        private Button buttonQR;
        private Button button1;
        private Button buttonQR;
        private Button buttonWhatsapp;
    }
}
