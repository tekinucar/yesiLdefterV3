namespace YesiLdefter
{
    partial class main
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(main));
            this.timer_Kullaniciya_Mesaj_Varmi = new System.Windows.Forms.Timer(this.components);
            this.timer_Mesaj_Suresi_Bitti = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer_Kullaniciya_Mesaj_Varmi
            // 
            this.timer_Kullaniciya_Mesaj_Varmi.Tick += new System.EventHandler(this.timer_Kullaniciya_Mesaj_Varmi_Tick);
            // 
            // timer_Mesaj_Suresi_Bitti
            // 
            this.timer_Mesaj_Suresi_Bitti.Interval = 6000;
            this.timer_Mesaj_Suresi_Bitti.Tick += new System.EventHandler(this.timer_Mesaj_Suresi_Bitti_Tick);
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1698, 910);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("main.IconOptions.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "yesiLdefter";
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.Shown += new System.EventHandler(this.mainForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer_Kullaniciya_Mesaj_Varmi;
        private System.Windows.Forms.Timer timer_Mesaj_Suresi_Bitti;
    }
}

