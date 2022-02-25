using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tkn_ToolBox;

namespace mainFormDesign
{
    public partial class MainDefter : DevExpress.XtraEditors.XtraForm
    {
        public MainDefter()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            ribbonControl1.Minimized = true;
        }

        private void toolboxControl1_ItemClick(object sender, DevExpress.XtraToolbox.ToolboxItemClickEventArgs e)
        {
            if (e.Item.Caption == "toolboxItem4")
            {
                using (tToolBox t = new tToolBox())
                {
                    Form form = new Form();
                    form.Text = "Form, " + DateTime.Now.ToLongDateString();
                    t.ChildForm_View(form, this);
                }

            }
        }

        private void barButtonGuncelleme_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void skinPaletteRibbonGalleryBarItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //
        }

        private void skinPaletteRibbonGalleryBarItem1_GalleryItemClick(object sender, DevExpress.XtraBars.Ribbon.GalleryItemClickEventArgs e)
        {
            //
        }

        private void skinRibbonGalleryBarItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //
        }

        private void skinRibbonGalleryBarItem1_GalleryItemClick(object sender, DevExpress.XtraBars.Ribbon.GalleryItemClickEventArgs e)
        {
            //
        }

        private void skinDropDownButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //
        }

        private void skinDropDownButtonItem1_DownChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void skinDropDownButtonItem1_HyperlinkClick(object sender, DevExpress.Utils.HyperlinkClickEventArgs e)
        {

        }
    }
}
