
namespace YesiLdefter
{
    partial class ms_FastReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ms_FastReport));
            this.designerControl1 = new FastReport.Design.StandardDesigner.DesignerControl();
            this.report1 = new FastReport.Report();
            this.previewControl1 = new FastReport.Preview.PreviewControl();
            ((System.ComponentModel.ISupportInitialize)(this.designerControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.report1)).BeginInit();
            this.SuspendLayout();
            // 
            // designerControl1
            // 
            this.designerControl1.AskSave = true;
            this.designerControl1.LayoutState = resources.GetString("designerControl1.LayoutState");
            this.designerControl1.Location = new System.Drawing.Point(0, 0);
            this.designerControl1.Name = "DesignerControl";
            this.designerControl1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.designerControl1.TabIndex = 0;
            this.designerControl1.UIStyle = FastReport.Utils.UIStyle.VisualStudio2012Light;
            this.designerControl1.Zoom = 1F;
            // 
            // report1
            // 
            this.report1.NeedRefresh = false;
            this.report1.ReportResourceString = resources.GetString("report1.ReportResourceString");
            this.report1.Tag = null;
            // 
            // previewControl1
            // 
            this.previewControl1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.previewControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewControl1.Font = new System.Drawing.Font("Tahoma", 8F);
            this.previewControl1.Location = new System.Drawing.Point(0, 0);
            this.previewControl1.Name = "previewControl1";
            this.previewControl1.PageOffset = new System.Drawing.Point(10, 10);
            this.previewControl1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.previewControl1.SaveInitialDirectory = null;
            this.previewControl1.Size = new System.Drawing.Size(800, 450);
            this.previewControl1.TabIndex = 0;
            // 
            // ms_FastReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.previewControl1);
            this.Name = "ms_FastReport";
            this.Text = "ms_FastReport";
            ((System.ComponentModel.ISupportInitialize)(this.designerControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.report1)).EndInit();
            this.ResumeLayout(false);

        }

        private FastReport.Design.StandardDesigner.DesignerControl designerControl1;
        private FastReport.Report report1;
        private FastReport.Preview.PreviewControl previewControl1;

        #endregion

        //private FastReport.Design.StandardDesigner.DesignerControl designerControl1;
        //private FastReport.Preview.PreviewControl previewControl1;
    }
}