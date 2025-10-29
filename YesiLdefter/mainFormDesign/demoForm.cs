using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mainFormDesign
{
    public partial class demoForm : Form
    {
        public demoForm()
        {
            InitializeComponent();
        }

        private void textEdit1_Enter(object sender, EventArgs e)
        {
            // sender
            if (((DevExpress.XtraEditors.TextEdit)sender).Enabled == false)
            {
                //((DevExpress.XtraEditors.TextEdit)sender).EnterMoveNextControl  //GetNextControl()
            }
        }

        private void tabPane1_SelectedPageChanged(object sender, DevExpress.XtraBars.Navigation.SelectedPageChangedEventArgs e)
        {
            label1.Text = "TabPane = " + tabPane1.SelectedPage.Tag.ToString();
        }

        private void navigationPane1_SelectedPageChanged(object sender, DevExpress.XtraBars.Navigation.SelectedPageChangedEventArgs e)
        {

        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {

        }

        private void vGridControl1_Enter(object sender, EventArgs e)
        {

        }

        private void vGridControl1_Leave(object sender, EventArgs e)
        {

        }
    }
}
