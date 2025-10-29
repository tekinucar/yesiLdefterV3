namespace yesildeftertest.Forms
{
    partial class WhatsAppMainForm
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
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.gridConversations = new System.Windows.Forms.DataGridView();
            this.gridMessages = new System.Windows.Forms.DataGridView();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.lblSelectedCustomer = new System.Windows.Forms.Label();
            this.lblConversationStatus = new System.Windows.Forms.Label();
            this._statusUpdateTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gridConversations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMessages)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // gridConversations
            // 
            this.gridConversations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridConversations.Location = new System.Drawing.Point(12, 28);
            this.gridConversations.Name = "gridConversations";
            this.gridConversations.Size = new System.Drawing.Size(300, 350);
            this.gridConversations.TabIndex = 2;
            // 
            // gridMessages
            // 
            this.gridMessages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMessages.Location = new System.Drawing.Point(318, 28);
            this.gridMessages.Name = "gridMessages";
            this.gridMessages.Size = new System.Drawing.Size(470, 300);
            this.gridMessages.TabIndex = 3;
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(318, 334);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(390, 44);
            this.txtMessage.TabIndex = 4;
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Location = new System.Drawing.Point(714, 334);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(74, 44);
            this.btnSendMessage.TabIndex = 5;
            this.btnSendMessage.Text = "Gönder";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            // 
            // lblSelectedCustomer
            // 
            this.lblSelectedCustomer.AutoSize = true;
            this.lblSelectedCustomer.Location = new System.Drawing.Point(318, 12);
            this.lblSelectedCustomer.Name = "lblSelectedCustomer";
            this.lblSelectedCustomer.Size = new System.Drawing.Size(0, 13);
            this.lblSelectedCustomer.TabIndex = 6;
            // 
            // lblConversationStatus
            // 
            this.lblConversationStatus.AutoSize = true;
            this.lblConversationStatus.Location = new System.Drawing.Point(12, 381);
            this.lblConversationStatus.Name = "lblConversationStatus";
            this.lblConversationStatus.Size = new System.Drawing.Size(0, 13);
            this.lblConversationStatus.TabIndex = 7;
            // 
            // _statusUpdateTimer
            // 
            this._statusUpdateTimer.Interval = 1000;
            this._statusUpdateTimer.Tick += new System.EventHandler(this.UpdateStatusDisplay);
            // 
            // WhatsAppMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblConversationStatus);
            this.Controls.Add(this.lblSelectedCustomer);
            this.Controls.Add(this.btnSendMessage);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.gridMessages);
            this.Controls.Add(this.gridConversations);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "WhatsAppMainForm";
            this.Text = "WhatsApp Main Form";
            this.Load += new System.EventHandler(this.WhatsAppMainForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WhatsAppMainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.gridConversations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMessages)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.DataGridView gridConversations;
        private System.Windows.Forms.DataGridView gridMessages;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.Label lblSelectedCustomer;
        private System.Windows.Forms.Label lblConversationStatus;
    }
}
