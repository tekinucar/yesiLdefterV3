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
using Tkn_Events;
using Tkn_InputPanel;
using Tkn_Save;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_IPList2 : Form //DevExpress.XtraEditors.XtraForm
    {
        tToolBox t = new tToolBox();
        tInputPanel ip = new tInputPanel();
        tEventsButton evb = new tEventsButton();
        vUserInputBox iBox = new vUserInputBox();
        tSave sv = new tSave();

        string TableIPCode = string.Empty;
        
        DataSet ds_MSTables = null;
        DataNavigator dN_MSTables = null;
        DataSet ds_MSFields = null;
        DataNavigator dN_MSFields = null;

        DataSet ds_MSTablesIP = null;
        DataNavigator dN_MSTablesIP = null;
        DataSet ds_MSFieldsIP = null;
        DataNavigator dN_MSFieldsIP = null;
        DataSet ds_GroupsIP = null;
        DataNavigator dN_GroupsIP = null;


        Control editpanel_TestSQL = null;

        string menuName = "MENU_" + "UST/PMS/PMS/InputPanel";

        string buttonMsTIPMsTIP = "ButtonCopyTablesIP";         // Copy MsTablesIP > MsTablesIP 
        string buttonMsfMsfIP = "ButtonCopyFieldsFieldsIP";     // Copy MsFields > MsFieldsIP   
        string buttonMsfIPMsfIP = "ButtonCopyFieldsIPFieldsIP"; // Copy MsFieldsIP > MsFieldsIP 

        string buttonIPTest = "ButtonIPTestEt";   // IP yi Test Et
        string buttonSqlView = "ButtonSQLView";   // SQL View
        string buttonSqlRec = "ButtonRecordSQL";  // Record SQL
        string buttonExpression = "ButtonExpressionView"; // Expression View
        string buttonClearSQL = "ButtonClearSQL"; // Clear SQL
        string buttonModelClass = "ButtonModelClass"; // Create ModelClass

        string buttonInsertPaketOlustur = "ButtonPaketOlustur";
        string buttonPaketiGonder = "ButtonPaketiGonder";

        string cumleMsTables = "";
        string cumleMsFields = "";
        string cumleMsTablesIP = "";
        string cumleMsFieldsIP = "";
        string cumleMsGroups = "";

        /// Tables/Fields/Groups
        /// UST/T01/3S_MSTBL.3S_MSTBL_05
        /// UST/T01/3S_MSFLD.3S_MSFLD_05
        /// UST/T01/3S_MSGRP.3S_MSGRP_04
        /// TablesIP/FieldsIP/Groups
        /// UST/T01/3S_MSTBLIP.3S_MSTBLIP_05
        /// UST/T01/3S_MSFLDIP.3S_MSFLDIP_05
        /// UST/T01/3S_MSGRP.3S_MSGRP_05

        public ms_IPList2()
        {
            InitializeComponent();
        }

        private void ms_IPList2_Shown(object sender, EventArgs e)
        {
            t.Find_Button_AddClick(this, menuName, buttonMsTIPMsTIP, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonMsfMsfIP, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonMsfIPMsfIP, myNavElementClick);
            
            t.Find_Button_AddClick(this, menuName, buttonIPTest, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonSqlView, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonSqlRec, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonExpression, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonClearSQL, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonModelClass, myNavElementClick);

            t.Find_Button_AddClick(this, menuName, buttonInsertPaketOlustur, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonPaketiGonder, myNavElementClick);

            if (ds_MSTables == null)
            {
                ///---
                TableIPCode = "UST/T01/3S_MSTBL.3S_MSTBL_05";
                t.Find_DataSet(this, ref ds_MSTables, ref dN_MSTables, TableIPCode);

                TableIPCode = "UST/T01/3S_MSFLD.3S_MSFLD_05";
                t.Find_DataSet(this, ref ds_MSFields, ref dN_MSFields, TableIPCode);

                ///---
                TableIPCode = "UST/T01/3S_MSTBLIP.3S_MSTBLIP_05";
                t.Find_DataSet(this, ref ds_MSTablesIP, ref dN_MSTablesIP, TableIPCode);

                TableIPCode = "UST/T01/3S_MSFLDIP.3S_MSFLDIP_05";
                t.Find_DataSet(this, ref ds_MSFieldsIP, ref dN_MSFieldsIP, TableIPCode);

                TableIPCode = "UST/T01/3S_MSGRP.3S_MSGRP_05";
                t.Find_DataSet(this, ref ds_GroupsIP, ref dN_GroupsIP, TableIPCode);
            }

            //
            // aranan nesne memoEdit ()
            // memoEdit aslında bir panelin içinde sıfırncı kontrol olarak duruyor
            //
            editpanel_TestSQL = t.Find_Control(this, v.lyt_Name + "40_40_01");
            if (editpanel_TestSQL != null)
            {
                if (((DevExpress.XtraEditors.PanelControl)editpanel_TestSQL).Controls.Count > 0)
                {
                    ((DevExpress.XtraEditors.PanelControl)editpanel_TestSQL).Controls[0].Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
                }
            }
        }

        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonIPTest) Test();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonSqlView) SqlView();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonSqlRec) SqlRec();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonExpression) ExpressionView();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonClearSQL) ClearSql();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonModelClass) CreateModelClass();
            }
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonMsfMsfIP) CopyMsfMsfIP();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonMsfIPMsfIP) CopyMsfIPMsfIP();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonMsTIPMsTIP) CopyMsTIPMsTIP();

                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonInsertPaketOlustur) InsertPaketOlustur();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonPaketiGonder) PaketiGonder();
            }
        }

        private void Test()
        {

            // Test sayfasını açalım, seçelim
            // ya
            // t.SelectPage(this, v.lyt_Name + "14", v.lyt_Name + "39", -1);
            // yada
            //t.SelectPage(this, v.lyt_Name + "14", "", 2);
            t.SelectPage(this, v.lyt_Name + "40", "", 2);
            // ---

            Control page = t.Find_Control(this, v.lyt_Name + "40_03");// "39");

            if (page != null)
            {
                if (page.Controls.Count > 0)
                    page.Controls.Clear();

                if (ds_MSTablesIP != null)
                {
                    if (dN_MSTablesIP.Position > -1)
                    {
                        // Test edilecek IP Code alınıyor
                        //
                        TableIPCode =
                            ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["SOFTWARE_CODE"].ToString() + "/" +
                            ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["PROJECT_CODE"].ToString() + "/" +
                            ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["TABLE_CODE"].ToString() + "." +
                            ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["IP_CODE"].ToString();

                        // Test işlemi gerçekleştiriliyor
                        //
                        ip.Create_InputPanel(this, page, TableIPCode, 1);

                        // Test sonucu dataset olmadığı zaman yeni create edilen VievControl nesnesi Enabled = false dönüyor
                        // onun Enabled = true yapılıyor 
                        Control cntrl = t.Find_Control_View(this, TableIPCode);

                        if (cntrl != null)
                        {
                            cntrl.Enabled = true;
                        }

                        // Test sonucu oluşan yeni viewe ait SQL de memoEdit te gösterilecek
                        // memoEdit aslında bir panelin içinde sıfırncı kontrol olarak duruyor
                        //
                        viewText(v.con_SQL);
                    }
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

        private void SqlRec()
        {
            // Test sonucu oluşan yeni viewe ait SQL de memoEdit te gösterilecek
            // memoEdit aslında bir panelin içinde sıfırncı kontrol olarak duruyor
            //
            viewText(v.SQLSave);
        }

        private void ExpressionView()
        {
            viewText(v.con_Expression_View);
        }

        private void ClearSql()
        {
            v.SQL = "";
        }

        private void CreateModelClass()
        {
            if (t.IsNotNull(ds_MSFields) == false) return;
            if (t.IsNotNull(ds_MSTables) == false) return;

            string content = "";
            int refId = t.myInt32(ds_MSTables.Tables[0].Rows[dN_MSTables.Position]["REF_ID"].ToString());

            string Sql = CreateModelClassSql(refId);

            v.SQL = Sql + v.SQL;

            DataSet dsModel = new DataSet();
            t.SQL_Read_Execute(v.dBaseNo.Manager, dsModel, ref Sql, "TableModel", null);
            if (t.IsNotNull(dsModel))
                content = dsModel.Tables[0].Rows[0][0].ToString();
            dsModel.Dispose();

            viewText(content);
        }

        private void CopyMsfMsfIP()
        {
            if (t.IsNotNull(ds_MSFields) == false) return;
            if (ds_MSFieldsIP == null) return;
            if (t.IsNotNull(ds_MSTablesIP) == false) return;

            if (ds_MSFieldsIP.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("DİKKAT : Bu MsFieldsIP de daha önce tanımlanmış field(s) mevcut. " + v.ENTER +
                    "Bu nedenle işleme devam edemiyorum ...");
                return;
            }

            string soru = "MS_FIELDS tablosundaki fieldler kopyalanacak, Onaylıyor musunuz ?";
            DialogResult cevap = t.mySoru(soru);
            if (DialogResult.Yes == cevap)
            {
                TableIPCode = "UST/T01/3S_MSFLDIP.3S_MSFLDIP_05";

                int field_no = 0;

                foreach (DataRow row in ds_MSFields.Tables[0].Rows)
                {
                    field_no = t.myInt32(row["FIELD_NO"].ToString());

                    //NavigatorButton btnApp = dN_MSFieldsIP.Buttons.Append;
                    //dN_MSFieldsIP.Buttons.DoClick(btnApp);
                    evb.newData(this, TableIPCode);

                    ds_MSFieldsIP.Tables[0].Rows[dN_MSFieldsIP.Position]["FIELD_NO"] = field_no.ToString();

                    // kaydı aç
                    ds_MSFieldsIP.Tables[0].CaseSensitive = false;

                    dN_MSFieldsIP.Tag = dN_MSFieldsIP.Position;
                    NavigatorButton btnEnd = dN_MSFieldsIP.Buttons.EndEdit;
                    dN_MSFieldsIP.Buttons.DoClick(btnEnd);
                }
            }

            t.TableRefresh(this, ds_MSFieldsIP);


        }

        private void CopyMsfIPMsfIP()
        {
            if (t.IsNotNull(ds_MSFields) == false) return;
            if (ds_MSFieldsIP == null) return;
            if (t.IsNotNull(ds_MSTablesIP) == false) return;

            if (ds_MSFieldsIP.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("DİKKAT : Bu MsFieldsIP de daha önce tanımlanmış field(s) mevcut. " + v.ENTER +
                    "Bu nedenle işleme devam edemiyorum ...");
                return;
            }

            // cursor Kimin üzerinde ise o yeni ıp
            string newIPCode = ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["IP_CODE"].ToString();
            string TableCode = ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["TABLE_CODE"].ToString();
            string OldIPCode = ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["TABLE_CODE"].ToString() + "_";

            string SoftwareCode = ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["SOFTWARE_CODE"].ToString();
            string ProjectCode = ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["PROJECT_CODE"].ToString();

            iBox.Clear();
            iBox.title = "Kopyalanacak MS_FIELDS_IP Kodu";
            iBox.promptText = "Kopyalanacak MS_FIELDS_IP IPCode  :";
            iBox.value = OldIPCode;
            iBox.displayFormat = "";
            iBox.fieldType = 0;

            // ınput box ile sorulan ise kimden kopyalanacak (old) bilgisi
            if (t.UserInpuBox(iBox) == DialogResult.OK)
            {
                OldIPCode = iBox.value;

                if (t.IsNotNull(newIPCode))
                {
                    string Sql = MSFieldsIP_Insert_Sql(TableCode, OldIPCode, newIPCode, SoftwareCode, ProjectCode);

                    DataSet dsCopy = new DataSet();
                    t.SQL_Read_Execute(v.dBaseNo.Manager, dsCopy, ref Sql, "MS_FIELDS_IP", null);
                    dsCopy.Dispose();
                    t.TableRefresh(this, ds_MSFieldsIP);

                }
            }


        }

        private string CreateModelClassSql(int RefId)
        {
            return @" EXEC [dbo].[prc_GetTableModel] @RefId = " + RefId.ToString();
        }

        private string MSFieldsIP_Insert_Sql(string TableCode, string OldIPCode, string NewIPCode,
              string SoftwareCode, string ProjectCode)
        {

            return @"
           INSERT INTO [dbo].[MS_FIELDS_IP]
           ([TABLE_CODE]
           ,[IP_CODE]
           ,[FIELD_NO]
           ,[FCAPTION]
           ,[FHINT]
           ,[DEFAULT_TYPE]
           ,[DEFAULT_NUMERIC]
           ,[DEFAULT_TEXT]
           ,[DEFAULT_INT]
           ,[DEFAULT_SP]
           ,[DEFAULT_SETUP]
           ,[LIST_TYPES_NAME]
           ,[CMP_COLUMN_TYPE]
           ,[CMP_WIDTH]
           ,[CMP_HEIGHT]
           ,[CMP_TOP]
           ,[CMP_LEFT]
           ,[CMP_READONLY]
           ,[CMP_ENABLED]
           ,[CMP_VISIBLE]
           ,[CMP_DISPLAY_FORMAT]
           ,[CMP_EDIT_FORMAT]
           ,[CMP_FONT_NAME]
           ,[CMP_FONT_SIZE]
           ,[CMP_FONT_STYLE]
           ,[CMP_FONT_COLOR]
           ,[CMP_BACK_COLOR]
           ,[VALIDATION_OPERATOR]
           ,[VALIDATION_VALUE1]
           ,[VALIDATION_VALUE2]
           ,[VALIDATION_ERRORTEXT]
           ,[VALIDATION_ERRORTYPE]
           ,[VALIDATION_INSERT]
           ,[MASTER_TABLEIPCODE]
           ,[SEARCH_TABLEIPCODE]
           ,[MASTER_KEY_FNAME]
           ,[GROUP_NO]
           ,[GROUP_LINE_NO]
           ,[KRT_LINE_NO]
           ,[KRT_CAPTION]
           ,[KRT_OPERAND_TYPE]
           ,[KRT_LIKE]
           ,[KRT_DEFAULT1]
           ,[KRT_DEFAULT2]
           ,[KRT_ALIAS]
           ,[KRT_TABLE_ALIAS]
           ,[MOVE_ITEM_TYPE]
           ,[MOVE_ITEM_NAME]
           ,[MOVE_TYPE]
           ,[MOVE_LOCATION]
           ,[MOVE_LAYOUT_TYPE]
           ,[FJOIN_TYPE]
           ,[FJOIN_TABLE_NAME]
           ,[FJOIN_TABLE_ALIAS]
           ,[FJOIN_KEY_FNAME]
           ,[FJOIN_WHERE]
           ,[MASTER_CHECK_FNAME]
           ,[MASTER_CHECK_VALUE]
           ,[CMP_SORT_TYPE]
           ,[CMP_SUMMARY_TYPE]
           ,[CMP_FORMAT_TYPE]
           ,[EXPRESSION_TYPE]
           ,[PROP_EXPRESSION]
           ,[PROP_NAVIGATOR]
           ,[XML_FIELD_NAME]
           ,[SOFTWARE_CODE]
	       ,[PROJECT_CODE]
           ,[FJOIN_CAPTION_FNAME]
           ,[WebScrapingPageCode]
           ,[WebScrapingGetNodeId]
           ,[WebScrapingGetColumnNo]
           ,[WebScrapingSetNodeId]
           ,[WebScrapingSetColumnNo]
           )
           
      SELECT 
       [TABLE_CODE]
      ,'" + NewIPCode + @"'
      ,[FIELD_NO]
      ,[FCAPTION]
      ,[FHINT]
      ,[DEFAULT_TYPE]
      ,[DEFAULT_NUMERIC]
      ,[DEFAULT_TEXT]
      ,[DEFAULT_INT]
      ,[DEFAULT_SP]
      ,[DEFAULT_SETUP]
      ,[LIST_TYPES_NAME]
      ,[CMP_COLUMN_TYPE]
      ,[CMP_WIDTH]
      ,[CMP_HEIGHT]
      ,[CMP_TOP]
      ,[CMP_LEFT]
      ,[CMP_READONLY]
      ,[CMP_ENABLED]
      ,[CMP_VISIBLE]
      ,[CMP_DISPLAY_FORMAT]
      ,[CMP_EDIT_FORMAT]
      ,[CMP_FONT_NAME]
      ,[CMP_FONT_SIZE]
      ,[CMP_FONT_STYLE]
      ,[CMP_FONT_COLOR]
      ,[CMP_BACK_COLOR]
      ,[VALIDATION_OPERATOR]
      ,[VALIDATION_VALUE1]
      ,[VALIDATION_VALUE2]
      ,[VALIDATION_ERRORTEXT]
      ,[VALIDATION_ERRORTYPE]
      ,[VALIDATION_INSERT]
      ,[MASTER_TABLEIPCODE]
      ,[SEARCH_TABLEIPCODE]
      ,[MASTER_KEY_FNAME]
      ,[GROUP_NO]
      ,[GROUP_LINE_NO]
      ,[KRT_LINE_NO]
      ,[KRT_CAPTION]
      ,[KRT_OPERAND_TYPE]
      ,[KRT_LIKE]
      ,[KRT_DEFAULT1]
      ,[KRT_DEFAULT2]
      ,[KRT_ALIAS]
      ,[KRT_TABLE_ALIAS]
      ,[MOVE_ITEM_TYPE]
      ,[MOVE_ITEM_NAME]
      ,[MOVE_TYPE]
      ,[MOVE_LOCATION]
      ,[MOVE_LAYOUT_TYPE]
      ,[FJOIN_TYPE]
      ,[FJOIN_TABLE_NAME]
      ,[FJOIN_TABLE_ALIAS]
      ,[FJOIN_KEY_FNAME]
      ,[FJOIN_WHERE]
      ,[MASTER_CHECK_FNAME]
      ,[MASTER_CHECK_VALUE]
      ,[CMP_SORT_TYPE]
      ,[CMP_SUMMARY_TYPE]
      ,[CMP_FORMAT_TYPE]
      ,[EXPRESSION_TYPE]
      ,[PROP_EXPRESSION]
      ,[PROP_NAVIGATOR]
      ,[XML_FIELD_NAME]
      ,[SOFTWARE_CODE]
	  ,[PROJECT_CODE]
      ,[FJOIN_CAPTION_FNAME]
      ,[WebScrapingPageCode]
      ,[WebScrapingGetNodeId]
      ,[WebScrapingGetColumnNo]
      ,[WebScrapingSetNodeId]
      ,[WebScrapingSetColumnNo]

      FROM [dbo].[MS_FIELDS_IP]
      where [IP_CODE] = '" + OldIPCode + @"'
      and   [TABLE_CODE] = '" + TableCode + @"'
      and   [SOFTWARE_CODE] = '" + SoftwareCode + @"'
	  and   [PROJECT_CODE] = '" + ProjectCode + @"'
      order by [FIELD_NO]  

      Select count(*) ADET  
      FROM [dbo].[MS_FIELDS_IP]
      where [IP_CODE] = '" + NewIPCode + @"'
      and   [TABLE_CODE] = '" + TableCode + @"'
      and   [SOFTWARE_CODE] = '" + SoftwareCode + @"'
	  and   [PROJECT_CODE] = '" + ProjectCode + @"'
      ";

        }

        private void CopyMsTIPMsTIP()
        {
            if (t.IsNotNull(ds_MSFields) == false) return;
            if (ds_MSFieldsIP == null) return;
            if (t.IsNotNull(ds_MSTablesIP) == false) return;

            //if (ds_MSFieldsIP.Tables[0].Rows.Count > 0)
            //{
            //    MessageBox.Show("DİKKAT : Bu MsFieldsIP de daha önce tanımlanmış field(s) mevcut. " + v.ENTER +
            //        "Bu nedenle işleme devam edemiyorum ...");
            //    return;
            //}

            string OldIPCode = ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["IP_CODE"].ToString();
            string TableCode = ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["TABLE_CODE"].ToString();
            string NewIPCode = ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["TABLE_CODE"].ToString() + "_";
            string SoftwareCode = ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["SOFTWARE_CODE"].ToString();
            string ProjectCode = ds_MSTablesIP.Tables[0].Rows[dN_MSTablesIP.Position]["PROJECT_CODE"].ToString();

            iBox.Clear();
            iBox.title = "Kopyalanacak : " + TableCode + "." + OldIPCode;
            iBox.promptText = "Yeni MS_TABLESIPCode : ";
            iBox.value = NewIPCode;
            iBox.displayFormat = "";
            iBox.fieldType = 0;

            if (t.UserInpuBox(iBox) == DialogResult.OK)
            {
                NewIPCode = iBox.value;

                if (t.IsNotNull(NewIPCode))
                {
                    //MessageBox.Show(OldIPCode +"//"+ NewIPCode);

                    string Sql = MSTablesIP_Insert_Sql(TableCode, OldIPCode, NewIPCode, SoftwareCode, ProjectCode);

                    DataSet dsCopy = new DataSet();
                    t.SQL_Read_Execute(v.dBaseNo.Manager, dsCopy, ref Sql, "MS_TABLES_IP", null);
                    dsCopy.Dispose();
                    t.TableRefresh(this, ds_MSTablesIP);

                }
            }
        }

        private string MSTablesIP_Insert_Sql(string TableCode, string OldIPCode, string NewIPCode,
             string SoftwareCode, string ProjectCode)
        {
            return @"
           INSERT INTO [dbo].[MS_TABLES_IP]
           ([TABLE_CODE]
           ,[TABLE_TYPE]
           ,[IP_CODE]
           ,[TABLEIPCODE]
           ,[IP_TYPE]
           ,[IP_ABOUT]
           ,[IP_CAPTION]
           ,[IP_SELECT_SQL]
           ,[IP_WHERE_SQL]
           ,[IP_ORDER_BY]
           ,[EXTERNAL_IP_CODE]
           ,[VIEW_CMP_TYPE]
           ,[CMP_PROPERTIES]
           ,[NAVIGATOR]
           ,[DATA_READ]
           ,[FIND_FNAME]
           ,[DATA_FIND]
           ,[AUTO_INSERT]
           ,[MASTER_TABLEIPCODE]
           ,[MASTER_TABLE_NAME]
           ,[MASTER_KEY_FNAME]
           ,[FOREING_FNAME]
           ,[PARENT_FNAME]
           ,[BLOCK_PRIMARY_FNAME]
           ,[BLOCK_WHERE_FNAME]
           ,[BLOCK_TYPE_CODE]
           ,[PROP_SUBVIEW]
           ,[PROP_JOINTABLE]
           ,[PROP_VIEWS]
           ,[PROP_RUNTIME]
           ,[PROP_NAVIGATOR]
           ,[PROP_FOREING]
           ,[PROP_SEARCH]
           ,[SOFTWARE_CODE]
           ,[PROJECT_CODE] 
         )
           SELECT [TABLE_CODE]
           ,[TABLE_TYPE]
           ,'" + NewIPCode + @"' 
           ,[TABLEIPCODE]
           ,[IP_TYPE]
           ,[IP_ABOUT]
           ,[IP_CAPTION]
           ,[IP_SELECT_SQL]
           ,[IP_WHERE_SQL]
           ,[IP_ORDER_BY]
           ,[EXTERNAL_IP_CODE]
           ,[VIEW_CMP_TYPE]
           ,[CMP_PROPERTIES]
           ,[NAVIGATOR]
           ,[DATA_READ]
           ,[FIND_FNAME]
           ,[DATA_FIND]
           ,[AUTO_INSERT]
           ,[MASTER_TABLEIPCODE]
           ,[MASTER_TABLE_NAME]
           ,[MASTER_KEY_FNAME]
           ,[FOREING_FNAME]
           ,[PARENT_FNAME]
           ,[BLOCK_PRIMARY_FNAME]
           ,[BLOCK_WHERE_FNAME]
           ,[BLOCK_TYPE_CODE]
           ,[PROP_SUBVIEW]
           ,[PROP_JOINTABLE]
           ,[PROP_VIEWS]
           ,[PROP_RUNTIME]
           ,[PROP_NAVIGATOR]
           ,[PROP_FOREING]
           ,[PROP_SEARCH]
           ,[SOFTWARE_CODE]
           ,[PROJECT_CODE]

           FROM [dbo].[MS_TABLES_IP]
           Where [TABLE_CODE] = '" + TableCode + @"'
           and   [IP_CODE] = '" + OldIPCode + @"'
           and   [SOFTWARE_CODE] = '" + SoftwareCode + @"'
	       and   [PROJECT_CODE] = '" + ProjectCode + @"'
               
           Select count(*) ADET  
           FROM [dbo].[MS_TABLES_IP]
           where [IP_CODE] = '" + NewIPCode + @"'
           and   [TABLE_CODE] = '" + TableCode + @"'
           and   [SOFTWARE_CODE] = '" + SoftwareCode + @"'
	       and   [PROJECT_CODE] = '" + ProjectCode + @"'
           ";
        }

        private void viewText(string text)
        {
            if (editpanel_TestSQL != null)
            {
                if (((DevExpress.XtraEditors.PanelControl)editpanel_TestSQL).Controls.Count > 0)
                {
                    ((DevExpress.XtraEditors.PanelControl)editpanel_TestSQL).Controls[0].Text = text;
                }
            }
        }

        private void InsertPaketOlustur()
        {
            if (t.IsNotNull(ds_MSTables) == false) return;

            string tableCode = ds_MSTables.Tables[0].Rows[dN_MSTables.Position]["TABLE_CODE"].ToString();

            string myProp = ds_MSTables.Namespace;
            string databaseName = t.MyProperties_Get(myProp, "DBaseName:");
            string schemaName = t.MyProperties_Get(myProp, "SchemasCode:");

            string soru = tableCode + " tablosu için INSERT paketi oluşturulacak, Onaylıyor musunuz ?";
            DialogResult cevap = t.mySoru(soru);
            if (DialogResult.Yes == cevap)
            {
                cumleMsTables = "";
                cumleMsFields = "";
                cumleMsTablesIP = "";
                cumleMsFieldsIP = "";
                cumleMsGroups = "";

                cumleMsTables = preparingInsertScript(databaseName, schemaName, "MS_TABLES", tableCode);
                cumleMsFields = preparingInsertScript(databaseName, schemaName, "MS_FIELDS", tableCode);
                cumleMsTablesIP = preparingInsertScript(databaseName, schemaName, "MS_TABLES_IP", tableCode);
                cumleMsFieldsIP = preparingInsertScript(databaseName, schemaName, "MS_FIELDS_IP", tableCode);
                cumleMsGroups = preparingInsertScript(databaseName, schemaName, "MS_GROUPS", tableCode);

                viewText(
                    cumleMsTables + v.ENTER2 +
                    cumleMsFields + v.ENTER2 +
                    cumleMsTablesIP + v.ENTER2 +
                    cumleMsFieldsIP + v.ENTER2 +
                    cumleMsGroups);

                //t.FlyoutMessage(this, "Web Manager Database Update", "Insert paketler hazırlandı...");

                PaketiGonder();
            }
        }

        
        private string preparingInsertScript(string databaseName, string schemaName, string tableName, string tableCode)
        {
            vScripts scripts = new vScripts();

            scripts.SourceDBaseName = databaseName;
            scripts.SchemaName = schemaName;
            scripts.SourceTableName = tableName;
            scripts.Where = string.Format(" TABLE_CODE = '{0}' ", tableCode);
            scripts.IdentityInsertOnOff = false;

            string cumle = t.preparingInsertScript(scripts);

            return cumle;
        }
        
        private void PaketiGonder()
        {
            if (cumleMsTables != "") t.runScript(v.dBaseNo.publishManager, cumleMsTables); 
            if (cumleMsFields != "") t.runScript(v.dBaseNo.publishManager, cumleMsFields);
            if (cumleMsTablesIP != "") t.runScript(v.dBaseNo.publishManager, cumleMsTablesIP);
            if (cumleMsFieldsIP != "") t.runScript(v.dBaseNo.publishManager, cumleMsFieldsIP);
            if (cumleMsGroups != "") t.runScript(v.dBaseNo.publishManager, cumleMsGroups);

            t.FlyoutMessage(this, "Web Manager Database Update", "Insert paketler gönderildi...");
        }
        
    }

}
