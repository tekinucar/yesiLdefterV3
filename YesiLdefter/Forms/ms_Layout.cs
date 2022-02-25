using DevExpress.XtraEditors;
using System;
using System.Data;
using Tkn_ToolBox;

namespace YesiLdefter
{
    public partial class ms_Layout : DevExpress.XtraEditors.XtraForm
    {
        tToolBox t = new tToolBox();

        DataSet ds_FormList = null;
        DataNavigator dN_FormList = null;

        string TableIPCode = string.Empty;
        string menuName = "MENU_UST/T01/SYS/MENU_MSLY";
        string buttonDialog = "item_63";
        string buttonNormal = "item_65";
        string buttonChild = "item_66";

        public ms_Layout()
        {
            InitializeComponent();
        }

        private void ms_Layout_Shown(object sender, EventArgs e)
        {
            t.Find_Button_AddClick(this, menuName, buttonDialog, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonNormal, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonChild, myNavElementClick);

            if (ds_FormList == null)
            {
                TableIPCode = "3S_MSLYT.3S_MSLYT_01";
                t.Find_DataSet(this, ref ds_FormList, ref dN_FormList, TableIPCode);
            }
        }

        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonDialog) TestDialog();
            if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonNormal) TestNormal();
            if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonChild) TestChild();
        }

        private void TestDialog()
        {
            string FormCode = "DNM_01";

            if (ds_FormList != null)
            {
                if (dN_FormList.Position > -1)
                    FormCode = ds_FormList.Tables[0].Rows[dN_FormList.Position]["MASTER_CODE"].ToString();
            }

            string Prop_Navigator = @"
            0=FORMNAME:null;
            0=FORMCODE:" + FormCode + @";
            0=FORMTYPE:DIALOG;
            0=FORMSTATE:NORMAL;
            ";

            t.OpenForm(new ms_Form(), Prop_Navigator);
        }

        private void TestNormal()
        {
            string FormCode = "DNM_01";

            if (ds_FormList != null)
            {
                if (dN_FormList.Position > -1)
                    FormCode = ds_FormList.Tables[0].Rows[dN_FormList.Position]["MASTER_CODE"].ToString();
            }

            string Prop_Navigator = @"
            0=FORMNAME:null;
            0=FORMCODE:" + FormCode + @";
            0=FORMTYPE:NORMAL;
            0=FORMSTATE:NORMAL;
            ";

            t.OpenForm(new ms_Form(), Prop_Navigator);
        }

        private void TestChild()
        {
            string FormCode = "DNM_01";

            if (ds_FormList != null)
            {
                if (dN_FormList.Position > -1)
                    FormCode = ds_FormList.Tables[0].Rows[dN_FormList.Position]["MASTER_CODE"].ToString();
            }

            string Prop_Navigator = @"
            0=FORMNAME:null;
            0=FORMCODE:" + FormCode + @";
            0=FORMTYPE:CHILD;
            0=FORMSTATE:NORMAL;
            ";

            t.OpenForm(new ms_Form(), Prop_Navigator);
        }

    }
}