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
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_DestekServiceTool : Form
    {
        tToolBox t = new tToolBox();

        string menuName = "MENU_" + "UST/PMS/PMS/DestekServiceTool";

        string buttonDbUpdates = "ButtonDbUpdates";
        string buttonSqlView = "ButtonSQLView";   // SQL View
        string buttonRecSQLView = "ButtonRecSQLView";
        string editPanelName = "editpanel_ViewSQL";
        Control editpanel_ViewSQL = null;

        string TableIPCode = string.Empty;

        DataSet ds_DbUpdates = null;
        DataNavigator dN_DbUpdates = null;

        public ms_DestekServiceTool()
        {
            InitializeComponent();
        }

        private void ms_DestekServiceTool_Shown(object sender, EventArgs e)
        {
            TableIPCode = "UST/PMS/DbUpdates.List_L01";
            t.Find_DataSet(this, ref ds_DbUpdates, ref dN_DbUpdates, TableIPCode);


            t.Find_Button_AddClick(this, menuName, buttonDbUpdates, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonSqlView, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonRecSQLView, myNavElementClick);

            //
            // aranan nesne memoEdit ()
            // memoEdit aslında bir panelin içinde sıfırncı kontrol olarak duruyor
            //
            editpanel_ViewSQL = t.Find_Control(this, editPanelName);
            if (editpanel_ViewSQL != null)
            {
                if (((DevExpress.XtraEditors.PanelControl)editpanel_ViewSQL).Controls.Count > 0)
                {
                    ((DevExpress.XtraEditors.PanelControl)editpanel_ViewSQL).Controls[0].Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
                }
            }
        }

        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonDbUpdates) DbUpdatesRefresh();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonSqlView) SqlView();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonRecSQLView) SqlRecView();
                //if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonExpression) ExpressionView();
            }
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                //if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonMsfMsfIP) CopyMsfMsfIP();
                //if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonMsfIPMsfIP) CopyMsfIPMsfIP();
                //if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonMsTIPMsTIP) CopyMsTIPMsTIP();
            }
        }
        private void DbUpdatesRefresh()
        {
            string soru = "DbUpdates tablosundaki işlemler tekrarlanacak, Onaylıyor musunuz ?";
            DialogResult cevap = t.mySoru(soru);
            if (DialogResult.Yes == cevap)
            {
                string tSql = " Delete from dbo.DbUpdates ";
                DataSet ds_Query = new DataSet();
                bool onay = t.SQL_Read_Execute(v.dBaseNo.Project, ds_Query, ref tSql, "DbUpdates", "");

                onay = t.dbUpdatesChecked();

                if (onay)
                {
                    t.TableRefresh(this, ds_DbUpdates);

                    t.FlyoutMessage(this, "DbUpdates", "İşlem tamamlanmıştır.");
                }
                else
                {
                    MessageBox.Show("DİKKAT : İşlem sırasında sorun oluştu...");
                }
            }
        }
        private void SqlView()
        {
            // Test sonucu oluşan yeni viewe ait SQL de memoEdit te gösterilecek
            // memoEdit aslında bir panelin içinde sıfırncı kontrol olarak duruyor
            //
            viewText(v.SQL);
        }
        private void SqlRecView()
        {
            viewText(v.SQLSave);
        }

        private void viewText(string text)
        {
            if (editpanel_ViewSQL != null)
            {
                if (((DevExpress.XtraEditors.PanelControl)editpanel_ViewSQL).Controls.Count > 0)
                {
                    ((DevExpress.XtraEditors.PanelControl)editpanel_ViewSQL).Controls[0].Text = text;
                }
            }
        }

    }
}
