using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Windows.Forms;
using Tkn_Forms;
using Tkn_Menu;
using Tkn_ToolBox;


namespace YesiLdefter
{
    public partial class ms_Menu : DevExpress.XtraEditors.XtraForm
    {
        tToolBox t = new tToolBox();

        DataSet ds_ItemList = null;
        DataNavigator dN_ItemList = null;

        string TableIPCode = string.Empty;
        string menuName = "MENU_UST/T01/SYS/MSMN";
        string buttonTest = "item_67";

        public ms_Menu()
        {
            InitializeComponent();
        }

        private void ms_Menu_Shown(object sender, EventArgs e)
        {
            t.Find_Button_AddClick(this, menuName, buttonTest, myNavElementClick);

            if (ds_ItemList == null)
            {
                TableIPCode = "3S_MSITEM.3S_MSITEM_01";
                t.Find_DataSet(this, ref ds_ItemList, ref dN_ItemList, TableIPCode);
            }
        }

        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonTest) Test();
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

    }
}