using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Windows.Forms;
using Tkn_Events;
using Tkn_Registry;
using Tkn_SQLs;
using Tkn_ToolBox;
using Tkn_UserFirms;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_UserFirmList : Form
    {
        tToolBox t = new tToolBox();
        tRegistry reg = new tRegistry();
        tUserFirms userFirms = new tUserFirms();

        DataSet dsUserFirmList = null; 
        DataNavigator dNUserFirmList = null;
        DataSet ds_Query2 = new DataSet();
        
        string tSql = string.Empty;
        string TableIPCode = string.Empty;
        string FirmList_TableIPCode = "UST/CRM/UstadFirms.UserFirmList_L02";
        

        string regPath = v.registryPath;


        public ms_UserFirmList()
        {
            InitializeComponent();

            tEventsForm evf = new tEventsForm();

            this.Load += new System.EventHandler(evf.myForm_Load);
            this.Shown += new System.EventHandler(evf.myForm_Shown);
            this.Shown += new System.EventHandler(this.ms_UserFirmList_Shown);

            this.KeyPreview = true;
        }

        private void ms_UserFirmList_Shown(object sender, EventArgs e)
        {
            //
            // FirmList -------------------------------------------------------------------
            //
            TableIPCode = FirmList_TableIPCode;

            Control cntrl = null;
            string[] controls = new string[] { };
            cntrl = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Dock = DockStyle.Right;
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_FirmListSec_Click);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("SIHIRBAZDEVAM16");
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Dock = DockStyle.Right;
            }

            t.Find_DataSet(this, ref dsUserFirmList, ref dNUserFirmList, FirmList_TableIPCode);
        }

        void btn_FirmListSec_Click(object sender, EventArgs e)
        {
            if (t.IsNotNull(dsUserFirmList))
            {
                DataRow row = dsUserFirmList.Tables[0].Rows[dNUserFirmList.Position];
                userFirms.readUstadFirmAbout(this, row);
            }
        }



    }
}
