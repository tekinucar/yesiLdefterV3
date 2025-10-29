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
                cumleMsLayout = preparingInsertScript(masterCode);

                //t.FlyoutMessage(this, "Web Manager Database Update", "Insert paketler hazırlandı...");

                PaketiGonder();
            }
        }

        private string preparingInsertScript(string tableCode)
        {
            vScripts scripts = new vScripts();

            scripts.SourceDBaseName = t.Find_dBLongName(Convert.ToString((byte)v.dBaseNo.Manager));  
            scripts.SchemaName = "dbo"; 
            scripts.SourceTableName = "MS_LAYOUT";
            scripts.TableIPCode = "";
            scripts.Where = string.Format(" MASTER_CODE = '{0}' ", tableCode);
            scripts.IdentityInsertOnOff = false;

            string cumle = t.preparingInsertScript(scripts);

            return cumle;
        }

        private void PaketiGonder()
        {
            if (cumleMsLayout != "") 
                t.runScript(v.dBaseNo.publishManager, cumleMsLayout);
            
            t.FlyoutMessage(this, "Web Manager Database Update", "Insert paketler gönderildi...");
        }


    }
}
