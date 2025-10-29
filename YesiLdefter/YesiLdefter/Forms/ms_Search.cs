using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Tkn_Events;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_Search : Form
    {
        tToolBox t = new tToolBox();

        public ms_Search()
        {
            InitializeComponent();

            tEventsForm evf = new tEventsForm();

            this.Load += new System.EventHandler(evf.myForm_Load);
            this.Shown += new System.EventHandler(evf.myForm_Shown);
            this.Shown += new System.EventHandler(this.ms_Search_Shown);

            this.KeyPreview = true;
        }

        private void ms_Search_Shown(object sender, EventArgs e)
        {
            //if (t.IsNotNull(v.tSearch.searchInputValue))
            //{
            //    Control cntrl = t.Find_Control(this, "textEdit_Find_" + t.AntiStr_Dot(v.tSearch.SearchTableIPCode));
                
            //    if (cntrl != null)
            //    {
            //        ((DevExpress.XtraEditors.TextEdit)cntrl).EditValue = v.tSearch.searchInputValue;
            //        ((DevExpress.XtraEditors.TextEdit)cntrl).SelectionStart = v.tSearch.searchInputValue.Length + 1;
            //        ((DevExpress.XtraEditors.TextEdit)cntrl).Focus();
            //    }
            //}
        }
    }
}
