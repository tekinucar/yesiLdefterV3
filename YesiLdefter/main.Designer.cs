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
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1364, 696);
            this.Name = "main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "yesiLdefter";
            this.Load += new System.EventHandler(this.main_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer_Kullaniciya_Mesaj_Varmi;
        private System.Windows.Forms.Timer timer_Mesaj_Suresi_Bitti;
    }
}

