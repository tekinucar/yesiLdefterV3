using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Windows.Forms;
using Tkn_Forms;
using Tkn_Menu;
using Tkn_Save;
using Tkn_ToolBox;
using Tkn_Variable;


namespace YesiLdefter
{
    public partial class ms_Menu2 : DevExpress.XtraEditors.XtraForm
    {
        tToolBox t = new tToolBox();
        tSave sv = new tSave();

        DataSet ds_ItemList = null;
        DataNavigator dN_ItemList = null;

        string TableIPCode = string.Empty;
        string menuName = "MENU_" + "UST/PMS/PMS/Menu";
        string buttonTest = "ButtonTest";
        string buttonInsertPaketOlustur = "ButtonPaketOlustur";
        string buttonPaketiGonder = "ButtonPaketiGonder";
        string cumleMsItems = "";
        public ms_Menu2()
        {
            InitializeComponent();
        }

        private void ms_Menu2_Shown(object sender, EventArgs e)
        {
            t.Find_Button_AddClick(this, menuName, buttonTest, myNavElementClick);

            t.Find_Button_AddClick(this, menuName, buttonInsertPaketOlustur, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonPaketiGonder, myNavElementClick);

            if (ds_ItemList == null)
            {
                TableIPCode = "UST/T01/3S_MSITEM.3S_MSITEM_21";
                t.Find_DataSet(this, ref ds_ItemList, ref dN_ItemList, TableIPCode);
            }
        }

        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonTest) Test();
            }

            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonInsertPaketOlustur) InsertPaketOlustur();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonPaketiGonder) PaketiGonder();
            }
        }
    
        private void Test()
        {
            tMenu mn = new tMenu();

            if (ds_ItemList != null)
            {
                if (dN_ItemList.Position > -1)
                {
                    TableIPCode = ds_ItemList.Tables[0].Rows[dN_ItemList.Position]["MASTER_CODE"].ToString();
                    if (TableIPCode != "")
                    {
                        Form tNewForm = null;
                        tForms fr = new tForms();
                        tNewForm = fr.Get_Form("ms_Form");
                        t.ChildForm_View(tNewForm, Application.OpenForms[0], FormWindowState.Maximized, "");
                        tNewForm.Text = "Menu Testi";

                        mn.Create_Menu_IN_Control(tNewForm, TableIPCode, string.Empty);

                    }
                }
            }
        }

        private void InsertPaketOlustur()
        {
            if (t.IsNotNull(ds_ItemList) == false) return;

            string masterCode = ds_ItemList.Tables[0].Rows[dN_ItemList.Position]["MASTER_CODE"].ToString();

            string soru = masterCode + " formu için INSERT paketi oluşturulacak, Onaylıyor musunuz ?";
            DialogResult cevap = t.mySoru(soru);
            if (DialogResult.Yes == cevap)
            {
                cumleMsItems = "";
                cumleMsItems = preparingInsertScript(masterCode);

                //t.FlyoutMessage(this, "Web Manager Database Update", "Insert paketler hazırlandı...");

                PaketiGonder();
            }
        }

        private string preparingInsertScript(string tableCode)
        {
            vScripts scripts = new vScripts();

            scripts.SourceDBaseName = t.Find_dBLongName(Convert.ToString((byte)v.dBaseNo.Manager));  
            scripts.SchemaName = "dbo"; 
            scripts.SourceTableName = "MS_ITEMS";
            scripts.TableIPCode = "";
            scripts.Where = string.Format(" MASTER_CODE = '{0}' ", tableCode);
            scripts.IdentityInsertOnOff = false;

            string cumle = t.preparingInsertScript(scripts);

            return cumle;
        }

        private void PaketiGonder()
        {
            if (cumleMsItems != "")
                t.runScript(v.dBaseNo.publishManager, cumleMsItems);

            t.FlyoutMessage(this, "Web Manager Database Update", "Insert paketler gönderildi...");
        }

}
}