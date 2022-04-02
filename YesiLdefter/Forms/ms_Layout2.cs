using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tkn_Save;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_Layout2 : Form
    {
        tToolBox t = new tToolBox();
        tSave sv = new tSave();

        DataSet ds_FormList = null;
        DataNavigator dN_FormList = null;

        string TableIPCode = string.Empty;
        string menuName = "MENU_" + "UST/PMS/PMS/Layout";
        string buttonDialog = "ButtonDialog";
        string buttonNormal = "ButtonNormal";
        string buttonChild = "ButtonChild";
        string buttonInsertPaketOlustur = "ButtonPaketOlustur";
        string buttonPaketiGonder = "ButtonPaketiGonder";
        string cumleMsLayout = "";

        public ms_Layout2()
        {
            InitializeComponent();
        }

        private void ms_Layout2_Shown(object sender, EventArgs e)
        {
            t.Find_Button_AddClick(this, menuName, buttonDialog, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonNormal, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonChild, myNavElementClick);
            
            t.Find_Button_AddClick(this, menuName, buttonInsertPaketOlustur, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonPaketiGonder, myNavElementClick);

            if (ds_FormList == null)
            {
                TableIPCode = "UST/T01/3S_MSLYT.3S_MSLYT_21";//"3S_MSLYT.3S_MSLYT_01";
                t.Find_DataSet(this, ref ds_FormList, ref dN_FormList, TableIPCode);
            }
        }

        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonDialog) TestDialog();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonNormal) TestNormal();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonChild) TestChild();
            }
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonInsertPaketOlustur) InsertPaketOlustur();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonPaketiGonder) PaketiGonder();
            }
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

        private void InsertPaketOlustur()
        {
            if (t.IsNotNull(ds_FormList) == false) return;

            string masterCode = ds_FormList.Tables[0].Rows[dN_FormList.Position]["MASTER_CODE"].ToString();

            string soru = masterCode + " formu için INSERT paketi oluşturulacak, Onaylıyor musunuz ?";
            DialogResult cevap = t.mySoru(soru);
            if (DialogResult.Yes == cevap)
            {
                cumleMsLayout = "";
                cumleMsLayout = preparingInsertScript("MS_LAYOUT", masterCode);

                //t.FlyoutMessage("Web Manager Database Update", "Insert paketler hazırlandı...");

                PaketiGonder();
            }
        }

        private string preparingInsertScript(string tableName, string tableCode)
        {
            DataSet dsQuery = new DataSet();
            string cumleDelete = " delete from {0} Where MASTER_CODE = '{1}' ";
            string cumleSelect = " Select * from {0} Where MASTER_CODE = '{1}' ";
            string cumle = "";
            string tSql = "";
            string myProp = string.Empty;

            cumle = string.Format(cumleDelete, tableName, tableCode) + v.ENTER2;

            tSql = string.Format(cumleSelect, tableName, tableCode);
            t.MyProperties_Set(ref myProp, "DBaseNo", Convert.ToString((byte)v.dBaseNo.Manager));
            t.MyProperties_Set(ref myProp, "TableName", tableName);
            t.MyProperties_Set(ref myProp, "SqlFirst", tSql);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");
            t.MyProperties_Set(ref myProp, "TableType", "1");
            t.MyProperties_Set(ref myProp, "Cargo", "data");
            t.MyProperties_Set(ref myProp, "KeyFName", "");

            dsQuery.Namespace = myProp;

            t.Data_Read_Execute(this, dsQuery, ref tSql, tableName, null);
            if (t.IsNotNull(dsQuery))
            {
                cumle = cumle + sv.Insert_Script_Multi(dsQuery, tableName, v.active_DB.managerMSSQLConn);
            }

            dsQuery.Dispose();

            return cumle;
        }

        private void PaketiGonder()
        {
            if (cumleMsLayout != "") 
                t.runScript(cumleMsLayout);
            
            t.FlyoutMessage("Web Manager Database Update", "Insert paketler gönderildi...");
        }


    }
}
