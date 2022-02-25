namespace mainFormDesign
{
    partial class tileControl
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
            DevExpress.XtraEditors.TileItemElement tileItemElement1 = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement3 = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement4 = new DevExpress.XtraEditors.TileItemElement();
            this.tileControl1 = new DevExpress.XtraEditors.TileControl();
            this.tileGroup1 = new DevExpress.XtraEditors.TileGroup();
            this.tileItem1 = new DevExpress.XtraEditors.TileItem();
            this.tileItem3 = new DevExpress.XtraEditors.TileItem();
            this.tileItem2 = new DevExpress.XtraEditors.TileItem();
            this.tileItem4 = new DevExpress.XtraEditors.TileItem();
            this.SuspendLayout();
            // 
            // tileControl1
            // 
            this.tileControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.tileControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileControl1.Groups.Add(this.tileGroup1);
            this.tileControl1.Location = new System.Drawing.Point(8, 8);
            this.tileControl1.MaxId = 4;
            this.tileControl1.Name = "tileControl1";
            this.tileControl1.ShowGroupText = true;
            this.tileControl1.ShowText = true;
            this.tileControl1.Size = new System.Drawing.Size(784, 434);
            this.tileControl1.TabIndex = 0;
            this.tileControl1.Text = "tileControl1";
            // 
            // tileGroup1
            // 
            this.tileGroup1.Items.Add(this.tileItem1);
            this.tileGroup1.Items.Add(this.tileItem3);
            this.tileGroup1.Items.Add(this.tileItem2);
            this.tileGroup1.Items.Add(this.tileItem4);
            this.tileGroup1.Name = "tileGroup1";
            this.tileGroup1.Text = "tileGroup1";
            // 
            // tileItem1
            // 
            tileItemElement1.Text = "tileItem1";
            this.tileItem1.Elements.Add(tileItemElement1);
            this.tileItem1.Id = 0;
            this.tileItem1.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
            this.tileItem1.Name = "tileItem1";
            this.tileItem1.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileItem1_ItemClick);
            // 
            // tileItem3
            // 
            tileItemElement2.Text = "tileItem3";
            this.tileItem3.Elements.Add(tileItemElement2);
            this.tileItem3.Id = 2;
            this.tileItem3.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
            this.tileItem3.Name = "tileItem3";
            // 
            // tileItem2
            // 
            tileItemElement3.Text = "tileItem2";
            this.tileItem2.Elements.Add(tileItemElement3);
            this.tileItem2.Id = 1;
            this.tileItem2.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tileItem2.Name = "tileItem2";
            // 
            // tileItem4
            // 
            tileItemElement4.Text = "tileItem4";
            this.tileItem4.Elements.Add(tileItemElement4);
            this.tileItem4.Id = 3;
            this.tileItem4.ItemSize = DevExpress.XtraEditors.TileItemSize.Large;
            this.tileItem4.Name = "tileItem4";
            // 
            // tileControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tileControl1);
            this.Name = "tileControl";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Text = "tileControl";
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TileControl tileControl1;
        private DevExpress.XtraEditors.TileGroup tileGroup1;
        private DevExpress.XtraEditors.TileItem tileItem1;
        private DevExpress.XtraEditors.TileItem tileItem2;
        private DevExpress.XtraEditors.TileItem tileItem3;
        private DevExpress.XtraEditors.TileItem tileItem4;
    }
}