namespace yesildeftertest
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtPhone;
        private TextBox txtMessage;
        private Button btnSend;
        private ListBox listBoxLog;
        private Label lblPhone;
        private Label lblMessage;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtPhone = new TextBox();
            this.txtMessage = new TextBox();
            this.btnSend = new Button();
            this.listBoxLog = new ListBox();
            this.lblPhone = new Label();
            this.lblMessage = new Label();
            this.SuspendLayout();
            // 
            // lblPhone
            // 
            this.lblPhone.Text = "Telefon (E.164):";
            this.lblPhone.Location = new System.Drawing.Point(12, 15);
            this.lblPhone.AutoSize = true;
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(120, 12);
            this.txtPhone.Width = 200;
            // 
            // lblMessage
            // 
            this.lblMessage.Text = "Mesaj:";
            this.lblMessage.Location = new System.Drawing.Point(12, 50);
            this.lblMessage.AutoSize = true;
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(120, 47);
            this.txtMessage.Width = 300;
            this.txtMessage.Height = 60;
            this.txtMessage.Multiline = true;
            // 
            // btnSend
            // 
            this.btnSend.Text = "Gönder";
            this.btnSend.Location = new System.Drawing.Point(120, 120);
            // 
            // listBoxLog
            // 
            this.listBoxLog.Location = new System.Drawing.Point(15, 160);
            this.listBoxLog.Width = 500;
            this.listBoxLog.Height = 200;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(540, 380);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.listBoxLog);
            this.Text = "WhatsApp Business Cloud API Demo";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
