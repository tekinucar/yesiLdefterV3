using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using Tkn_Events;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_ReportDesign : Form
    {
        tToolBox t = new tToolBox();

        public ms_ReportDesign()
        {
            InitializeComponent();

            tEventsForm evf = new tEventsForm();

            this.Load += new System.EventHandler(evf.myForm_Load);
            this.Shown += new System.EventHandler(evf.myForm_Shown);
            this.Shown += new System.EventHandler(this.ms_ReportDesign_Shown);

            this.KeyPreview = true;
        }

        private void ms_ReportDesign_Shown(object sender, EventArgs e)
        {

        }


    }
}
