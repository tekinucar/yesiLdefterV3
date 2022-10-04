using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tkn_CreateDatabase;
using Tkn_Events;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_ProjectTables : Form
    {
        tToolBox t = new tToolBox();
        tDatabase db = new tDatabase();
        tEventsMenu evm = new tEventsMenu();

        DataSet dsFirm = null;
        DataNavigator dNFirm = null;
        DataSet dsTables = null;
        DataNavigator dNTables = null;
        DataSet dsProcedures = null;
        DataNavigator dNProcedures = null;
        DataSet dsFunctions = null;
        DataNavigator dNFunctions = null;

        Control tabControl = null;

        string fSchemaName = "SchemaName";
        string fTableName = "TableName";
        string fProcedureName = "ProcedureName";
        string fFunctionName = "FunctionName";
        string fParentTable = "ParentTable";
        string fHasATrigger = "HasATrigger";
        string fSqlScript = "SqlScript";

        string fServerNameIP = "ServerNameIP";
        string fDatabaseName = "DatabaseName";
        string fDbLoginName = "DbLoginName";
        string fDbPass = "DbPass";
        

        string tableIPCode = "";
        string tablesTableIPCode = "";
        string proceduresTableIPCode = "";
        string functionsTableIPCode = "";
        
        string menuName1 = "MENU_" + "UST/PMS/PMS/ProjectTables";
        string buttonSingFileReadDbWrite = "SINGLE_FILE_READ_DB_WRITE";
        string buttonAllFileReadDbWrite = "ALL_FILE_READ_DB_WRITE";

        string menuName2 = "MENU_" + "UST/PMS/PMS/ProjectTablesCreate";
        string buttonDbConnection = "DB_CONNECTION";
        string buttonDbCreate = "DB_CREATE";
        string buttonFieldList = "FIELDS_LIST";
        string buttonDataView = "DATA_VIEW";

        string buttonTablesList = "TABLES_LIST";
        string buttonSingleTableCreate = "SINGLE_TABLE_CREATE";
        string buttonTablesCreate = "TABLES_CREATE";
        string buttonTriggersCreate = "TRIGGERS_CREATE";

        string buttonProceduresList = "PROCEDURES_LIST";
        string buttonSingleProcedureCreate = "SINGLE_PROCEDURE_CREATE";
        string buttonProceduresCreate = "PROCEDURES_CREATE";

        string buttonFunctionsList = "FUNCTIONS_LIST";
        string buttonSingleFunctionCreate = "SINGLE_FUNCTION_CREATE";
        string buttonFunctionsCreate = "FUNCTIONS_CREATE";

        public ms_ProjectTables()
        {
            InitializeComponent();

            tEventsForm evf = new tEventsForm();

            this.Load += new System.EventHandler(evf.myForm_Load);
            this.Shown += new System.EventHandler(evf.myForm_Shown);
            this.Shown += new System.EventHandler(this.ms_ProjectTables_Shown);

            this.KeyPreview = true;
        }

        private void ms_ProjectTables_Shown(object sender, EventArgs e)
        {
            // Table scriplerini MsProjectTables tablosuna kaydetmek
            if (dsTables == null)
            {
                tablesTableIPCode = "UST/PMS/MsProjectTables.List_L01";
                t.Find_DataSet(this, ref dsTables, ref dNTables, tablesTableIPCode);

                // Tables, storedPurocedures, function SqlScript insert işlemleri
                if (t.IsNotNull(dsTables))
                {
                    preparingTablesProceduresFunctionsScripts();
                }
            }

            // Yeni Firma için Database, tables, procedures ve functions oluşturmak için
            if (dsFirm == null)
            {
                tableIPCode = "UST/CRM/UstadFirms.DBKart_F02";
                t.Find_DataSet(this, ref dsFirm, ref dNFirm, tableIPCode);

                // Database create işlemleri
                if (t.IsNotNull(dsFirm))
                {
                    preparingDBCreate();
                }
            }
        }
        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                // menu1
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonSingFileReadDbWrite) singleFileReadDbWrite();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonAllFileReadDbWrite) allFileReadDbWrite();

                //menu2
                //if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == ) ();
                //if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == ) ();
            }
            // SubItem butonlar
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                // database
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonDbConnection) databaseConnectTEST();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonDbCreate) databaseCreate();
                // tables
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonTablesList) tablesList((DevExpress.XtraBars.Navigation.TileNavItem)sender);
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonSingleTableCreate) singleTableCreate();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonTablesCreate) tablesCreate();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonFieldList) fieldList();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonDataView) dataView();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonTriggersCreate) triggersCreate();
                

                // procedures
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonProceduresList) tablesList((DevExpress.XtraBars.Navigation.TileNavItem)sender);
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonSingleProcedureCreate) singleProcedureCreate();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonProceduresCreate) proceduresCreate();
                // functions
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonFunctionsList) tablesList((DevExpress.XtraBars.Navigation.TileNavItem)sender);
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonSingleFunctionCreate) singleFunctionCreate();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonFunctionsCreate) functionsCreate();
            }
        }

        #region Table scriplerini MsProjectTables tablosuna kaydetmek
        private void singleFileReadDbWrite()
        {
            int pageIndex = t.Find_TabControlPageIndex(this.tabControl);
            
            string fileName = "";
            string vSchemaName = "";
            string vTableName = "";
            string vParentTable = "";
            string vHasATrigger = "";

            v.active_DB.runDBaseNo = v.dBaseNo.UstadCrm;

            if (pageIndex == 0)
            {
                if (t.IsNotNull(dsTables))
                {
                    vSchemaName = dsTables.Tables[0].Rows[dNTables.Position][fSchemaName].ToString();
                    vTableName = dsTables.Tables[0].Rows[dNTables.Position][fTableName].ToString();
                    vParentTable = dsTables.Tables[0].Rows[dNTables.Position][fParentTable].ToString();
                    vHasATrigger = dsTables.Tables[0].Rows[dNTables.Position][fHasATrigger].ToString();

                    fileName = vTableName;
                    if (vSchemaName != "dbo")
                        fileName = vSchemaName + "_" + vTableName;

                    // parentTable olan tablolar başka bir tablonun Lkp tablolarıdır
                    // onlar kendisinin bağlı olduğu tablo içerisinde create ediliyorlar
                    // parentTable ismi olmayan Lkp_xxx tablolar ise bağımsız tablolardır
                    //
                    if (vParentTable == "")
                    {
                        tFileReadAndWrite(dsTables, dNTables, fSqlScript, fileName);
                    }
                }
            }

            if (pageIndex == 1)
            {
                fileName = dsProcedures.Tables[0].Rows[dNProcedures.Position][fProcedureName].ToString();
                
                if (fileName != "")
                {
                    tFileReadAndWrite(dsProcedures, dNProcedures, fSqlScript, fileName);
                }
            }

            if (pageIndex == 2)
            {
                fileName = dsFunctions.Tables[0].Rows[dNFunctions.Position][fFunctionName].ToString();

                if (fileName != "")
                {
                    tFileReadAndWrite(dsFunctions, dNFunctions, fSqlScript, fileName);
                }
            }
        }

        private void allFileReadDbWrite()
        {
            int pageIndex = t.Find_TabControlPageIndex(this.tabControl);

            if (pageIndex == 0)
            {
                allTableFileReadDbWrite();
            }
            if (pageIndex == 1)
            {
                allProcedureFileReadDbWrite();
            }
            if (pageIndex == 2)
            {
                allFunctionFileReadDbWrite();
            }
        }

        private void allTableFileReadDbWrite()
        {
            allFileReadDbWrite(dsTables, dNTables, "Table",
                fSchemaName, fTableName, fSqlScript, fParentTable);
        }

        private void allProcedureFileReadDbWrite()
        {
            allFileReadDbWrite(dsProcedures, dNProcedures, "Procedure",
                fSchemaName, fProcedureName, fSqlScript, "");
        }

        private void allFunctionFileReadDbWrite()
        {
            allFileReadDbWrite(dsFunctions, dNFunctions, "Function",
                fSchemaName, fFunctionName, fSqlScript, "");
        }

        private void allFileReadDbWrite(DataSet ds, DataNavigator dN, 
            string fileTypeName, 
            string fieldSchemaName, 
            string fieldEntityName, 
            string fieldSqlScript,
            string fieldParentName)
        {
            string fileName = "";
            string vSchemaName = "";
            string vEntityName = "";
            string vParentTable = "";

            v.active_DB.runDBaseNo = v.dBaseNo.UstadCrm;

            if (t.IsNotNull(ds))
            {
                string soru = " Tüm  " + fileTypeName + "  ?.txt dosyalarından okunarak database yazılacak. Onaylıyor musunuz ?";
                DialogResult cevap = t.mySoru(soru);
                if (DialogResult.Yes == cevap)
                {
                    int length = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < length; i++)
                    {
                        dN.Position = i;

                        vSchemaName = ds.Tables[0].Rows[i][fieldSchemaName].ToString();
                        vEntityName = ds.Tables[0].Rows[i][fieldEntityName].ToString();
                        vParentTable = "";

                        if (fieldParentName != "")
                            vParentTable = ds.Tables[0].Rows[i][fieldParentName].ToString();

                        /// parentTable ?
                        /// parentTable olan tablolar başka bir tablonun Lkp tablolarıdır
                        /// onlar kendisinin bağlı olduğu tablo içerisinde create ediliyorlar
                        /// parentTable ismi olmayan Lkp_xxx tablolar ise bağımsız tablolardır
                        ///
                        if (vParentTable == "")
                        {
                            fileName = vEntityName;
                            if (vSchemaName != "dbo")
                                fileName = vSchemaName + "_" + vEntityName;

                            tFileReadAndWrite(ds, dN, fieldSqlScript, fileName);
                        }
                    }
                    t.FlyoutMessage(fileTypeName + " dosyalarını okuma ve database yazma", "İşlemler tamamlandı...");
                }
            }
        }


        private bool tFileReadAndWrite(DataSet dsData, DataNavigator dNData, string fieldName, string fileName)
        {
            bool onay = false;
            string fName = string.Empty;
            string text = "";
            try
            {
                fName = v.EXE_ScriptsPath + "\\" + fileName + ".txt";
                if (File.Exists(@"" + fName))
                {
                    text = t.tReadTextFile(fName);
                }

                if (text != "")
                {
                    dsData.Tables[0].Rows[dNData.Position][fieldName] = text;

                    // kaydı aç
                    dsData.Tables[0].CaseSensitive = false;

                    dNData.Tag = dNData.Position;
                    NavigatorButton btnEnd = dNData.Buttons.EndEdit;
                    dNData.Buttons.DoClick(btnEnd);
                }
            }
            catch
            { }

            return onay;
        }

        #endregion Tablo scriplerini MsProjectTables tablosuna kaydetmek

        #region Database Create İşlemleri
        private void databaseConnectTEST()
        {
            dbConnction(true, true);
        }
        private void databaseCreate()
        {
            bool onay = false;
            onay = dbConnction(false, true);

            if (onay)
            {
                vTable vt = new vTable();
                vt.DBaseNo = v.dBaseNo.NewDatabase;
                vt.DBaseType = v.dBaseType.MSSQL;
                vt.DBaseName = v.newFirm_DB.databaseName;
                vt.msSqlConnection = v.newFirm_DB.MSSQLConn;

                onay = db.tDatabaseFind(vt);

                if (onay)
                {
                    t.FlyoutMessage("Database : " + vt.DBaseName, "Bu isimde bir database mevcut ....");
                }

                if (onay == false)
                {
                    string soru = v.newFirm_DB.databaseName + " isimli database yok, oluşturulacak. Onaylıyor musunuz ?";
                    DialogResult cevap = t.mySoru(soru);
                    if (DialogResult.Yes == cevap)
                    {
                        onay = db.tDatabaseCreate(vt);
                    }

                    // database oluşturuldu şimdi Lkp schema oluşturalım
                    if (onay)
                    {
                        // bu sefer yeni database in connection sağlansın
                        dbConnction(false, false);
                        
                        vt.SchemasCode = "Lkp";
                        vt.msSqlConnection = v.newFirm_DB.MSSQLConn;
                        
                        onay = false;
                        onay = db.tSchemaCreate(vt);

                        if (onay)
                        {
                            //MessageBox.Show("Database ve Schema (Lkp) başarıyla açılmıştır ...");
                            t.FlyoutMessage(":) Tebrikler...", "Database ve Schema (Lkp) başarıyla açılmıştır ...");
                        }
                    }
                }
            }
        }

        #endregion Database Create İşlemleri

        #region Tables, Procedures, Functions List
        private void tablesList(object sender)
        {
            bool onay = dbConnction(false, false);

            if (onay)
            {
                string buttonName = "";
                
                // gerçekte istekde bulunan butonun ismi
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name != null)
                    buttonName = ((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name.ToString();

                string islemButtonName = "item_FOPEN_SUBVIEW";
                string myFormLoadValue = "";
                string tableIPCode_ = "";

                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Appearance.Name != null)
                {
                    if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Tag != null)
                        myFormLoadValue = ((DevExpress.XtraBars.Navigation.TileNavItem)sender).Tag.ToString();

                    // bu işlev çalışsın diye bu isim verilerek istek gönderiliyor
                    evm.commonMenuClick(this, islemButtonName, "", myFormLoadValue);
                    
                    // tables listesini bulmak için hangi IPCode olduğunu bul
                    tableIPCode_ = findListTableIPCode(myFormLoadValue);

                    if (buttonName == buttonTablesList)
                        this.tablesTableIPCode = tableIPCode_;
                    if (buttonName == buttonProceduresList)
                        this.proceduresTableIPCode = tableIPCode_;
                    if (buttonName == buttonFunctionsList)
                        this.functionsTableIPCode = tableIPCode_;

                    // bulunan tableIPCode_ ile dsTables ve dNTables i bul
                    preparingObjectList();
                }
            }
            else MessageBox.Show("DİKKAT : Database ile bağlantı kurulamadı...");

        }
        private string findListTableIPCode(string myFormLoadValue)
        {
            string tableIPCode_ = "";
            string propNavigator = string.Empty;
            string workType = "";
            PROP_NAVIGATOR prop1_ = null;
            List<PROP_NAVIGATOR> propList_ = null;

            if (t.IsNotNull(myFormLoadValue))
                propNavigator = myFormLoadValue;
            
            // propNavigator ü temizle ve json a çevir
            if (propNavigator != "")
            {
                t.readProNavigator(propNavigator, ref prop1_, ref propList_);

                if (propList_ != null)
                {
                    foreach (PROP_NAVIGATOR prop_ in propList_)
                    {
                        foreach (TABLEIPCODE_LIST item in prop_.TABLEIPCODE_LIST)
                        {
                            workType = item.WORKTYPE.ToString();

                            if (workType == "SVIEW")
                            {
                                tableIPCode_ = item.TABLEIPCODE.ToString();
                                //TableAlias = item.TABLEALIAS.ToString();
                                //KeyFName = item.KEYFNAME.ToString();
                                break;
                            }
                        }
                    }
                }
            }

            return tableIPCode_;
        }
        private void preparingObjectList()
        {
            if (this.tablesTableIPCode != "")
            {
                if (dsTables == null)
                {
                    t.Find_DataSet(this, ref dsTables, ref dNTables, this.tablesTableIPCode);
                }
            }
            if (this.proceduresTableIPCode != "")
            {
                if (dsProcedures == null)
                {
                    t.Find_DataSet(this, ref dsProcedures, ref dNProcedures, this.proceduresTableIPCode);
                }
            }
            if (this.functionsTableIPCode != "")
            {
                if (dsFunctions == null)
                {
                    t.Find_DataSet(this, ref dsFunctions, ref dNFunctions, this.functionsTableIPCode);
                }
            }
        }

        #endregion Tables, Procedures, Functions List

        #region Tables işlemleri
        private void singleTableCreate()
        {
            if (t.IsNotNull(dsTables))
            {
                bool onay = true;
                
                v.active_DB.runDBaseNo = v.dBaseNo.NewDatabase;

                string tName = dsTables.Tables[0].Rows[dNTables.Position][fTableName].ToString();

                string soru = tName + " isimli tablo oluşturulacak. Onaylıyor musunuz ?";
                DialogResult cevap = t.mySoru(soru);
                if (DialogResult.Yes == cevap)
                {
                    onay = preparingCreateTable(dNTables.Position);

                    if (onay)
                        t.FlyoutMessage(tName, "Table oluşturuldu...");
                }
            }
        }
        private void tablesCreate()
        {
            if (t.IsNotNull(dsTables))
            {
                bool onay = true;
                vTable vt = new vTable();
                v.active_DB.runDBaseNo = v.dBaseNo.NewDatabase;

                string soru = " Tüm tablo listesi oluşturulacak. Onaylıyor musunuz ?";
                DialogResult cevap = t.mySoru(soru);
                if (DialogResult.Yes == cevap)
                {
                    // çalışmıyor sebebini anlamadım.
                    //t.WaitFormOpen(this, "Tables create...");
                    //v.SP_OpenApplication = true;
                    //---

                    int length = dsTables.Tables[0].Rows.Count;
                    for (int i = 0; i < length; i++)
                    {
                        dNTables.Position = i;
                        onay = preparingCreateTable(dNTables.Position);
                    }

                    //--- çalışmıyor
                    //v.IsWaitOpen = false;
                    //t.WaitFormClose();
                    //----
                    t.FlyoutMessage("Table oluşturma","İşlemler tamamlandı...");
                }
            }
        }
        private bool preparingCreateTable(int pos)
        {
            bool onay = false;
            string fileName = "";
            string vSchemaName = "";
            string vTableName = "";
            string vParentTable = "";
            //string vHasATrigger = "";
            string vSqlScript = "";
            vTable vt = new vTable();

            vSchemaName = dsTables.Tables[0].Rows[pos][fSchemaName].ToString();
            vTableName = dsTables.Tables[0].Rows[pos][fTableName].ToString();
            vParentTable = dsTables.Tables[0].Rows[pos][fParentTable].ToString();
            //vHasATrigger = dsTables.Tables[0].Rows[pos][fHasATrigger].ToString();
            vSqlScript = dsTables.Tables[0].Rows[pos][fSqlScript].ToString();

            vt.DBaseNo = v.dBaseNo.NewDatabase;
            vt.SchemasCode = vSchemaName;
            vt.TableName = vTableName;

            fileName = vTableName;
            if (vSchemaName != "dbo")
                fileName = vSchemaName + "_" + vTableName;

            Application.DoEvents();

            // parentTable olan tablolar başka bir tablonun Lkp tablolarıdır
            // onlar kendisinin bağlı olduğu tablo içerisinde create ediliyorlar
            // parentTable ismi olmayan Lkp_xxx tablolar ise bağımsız tablolardır
            //
            if ((vParentTable == "") && (t.IsNotNull(vSqlScript)))
            {
                v.active_DB.runDBaseNo = v.dBaseNo.NewDatabase;

                // tablo var mı diye kontrol et
                onay = db.tTableFind(fileName, vt);

                // tablo yok ise
                if (onay == false)
                {
                    onay = db.tTableCreate(vSqlScript, vt);
                }

                // tablo oluşturuldu şimdi trigger leri oluşturalım
                onay = false;
                onay = db.tTriggerCreate(vSqlScript, vt);

                // tablo oluşturuldu şimdi Lkp.xxxx tabloları oluşturalım
                onay = false;
                onay = db.tLkpTableCreate(vSqlScript, vt);
            }

            Thread.Sleep(100);

            return onay;
        }
        private void fieldList()
        {

        }
        private void dataView()
        {

        }
        private void triggersCreate()
        {
            if (t.IsNotNull(dsTables))
            {
                bool onay = true;
                vTable vt = new vTable();
                v.active_DB.runDBaseNo = v.dBaseNo.NewDatabase;

                string soru = " Tüm trigger listesi yeniden oluşturulacak. Onaylıyor musunuz ?";
                DialogResult cevap = t.mySoru(soru);
                if (DialogResult.Yes == cevap)
                {
                    // çalışmıyor sebebini anlamadım.
                    //t.WaitFormOpen(this, "Tables create...");
                    //v.SP_OpenApplication = true;
                    //---

                    int length = dsTables.Tables[0].Rows.Count;
                    for (int i = 0; i < length; i++)
                    {
                        dNTables.Position = i;
                        onay = preparingCreateTrigger(dNTables.Position);
                    }

                    //--- çalışmıyor
                    //v.IsWaitOpen = false;
                    //t.WaitFormClose();
                    //----
                    t.FlyoutMessage("Trigger oluşturma", "İşlemler tamamlandı...");
                }
            }
        }

        private bool preparingCreateTrigger(int pos)
        {
            bool onay = false;
            string fileName = "";
            string vSchemaName = "";
            string vTableName = "";
            string vParentTable = "";
            //string vHasATrigger = "";
            string vSqlScript = "";
            vTable vt = new vTable();

            vSchemaName = dsTables.Tables[0].Rows[pos][fSchemaName].ToString();
            vTableName = dsTables.Tables[0].Rows[pos][fTableName].ToString();
            vParentTable = dsTables.Tables[0].Rows[pos][fParentTable].ToString();
            //vHasATrigger = dsTables.Tables[0].Rows[pos][fHasATrigger].ToString();
            vSqlScript = dsTables.Tables[0].Rows[pos][fSqlScript].ToString();

            vt.DBaseNo = v.dBaseNo.NewDatabase;
            vt.SchemasCode = vSchemaName;
            vt.TableName = vTableName;

            fileName = vTableName;
            if (vSchemaName != "dbo")
                fileName = vSchemaName + "_" + vTableName;

            Application.DoEvents();

            // parentTable olan tablolar başka bir tablonun Lkp tablolarıdır
            // onlar kendisinin bağlı olduğu tablo içerisinde create ediliyorlar
            // parentTable ismi olmayan Lkp_xxx tablolar ise bağımsız tablolardır
            //
            if ((vParentTable == "") && (t.IsNotNull(vSqlScript)))
            {
                v.active_DB.runDBaseNo = v.dBaseNo.NewDatabase;

                onay = db.tTriggerCreate(vSqlScript, vt);
            }

            Thread.Sleep(100);

            return onay;
        }

        #endregion Tables işlemleri

        #region Procedures İşlemleri
        private void singleProcedureCreate()
        {
            if (t.IsNotNull(dsProcedures))
            {
                bool onay = true;

                v.active_DB.runDBaseNo = v.dBaseNo.NewDatabase;

                string tName = dsProcedures.Tables[0].Rows[dNProcedures.Position][fProcedureName].ToString();

                string soru = tName + " isimli procedure oluşturulacak. Onaylıyor musunuz ?";
                DialogResult cevap = t.mySoru(soru);
                if (DialogResult.Yes == cevap)
                {
                    onay = preparingCreateProcedure(dNProcedures.Position);

                    if (onay)
                        t.FlyoutMessage(tName, "Procedure oluşturuldu...");
                }
            }
        }
        private void proceduresCreate()
        {
            if (t.IsNotNull(dsProcedures))
            {
                bool onay = true;
                vTable vt = new vTable();
                v.active_DB.runDBaseNo = v.dBaseNo.NewDatabase;

                string soru = " Tüm procedure listesi oluşturulacak. Onaylıyor musunuz ?";
                DialogResult cevap = t.mySoru(soru);
                if (DialogResult.Yes == cevap)
                {
                    int length = dsProcedures.Tables[0].Rows.Count;
                    for (int i = 0; i < length; i++)
                    {
                        dNProcedures.Position = i;
                        onay = preparingCreateProcedure(dNProcedures.Position);
                        //if (onay == false) break;
                    }
                    t.FlyoutMessage("Procedure oluşturma", "İşlemler tamamlandı...");
                }
            }
        }
        private bool preparingCreateProcedure(int pos)
        {
            bool onay = false;
            string vSchemaName = "";
            string vProcedureName = "";
            string vSqlScript = "";
            vTable vt = new vTable();

            vSchemaName = dsProcedures.Tables[0].Rows[pos][fSchemaName].ToString();
            vProcedureName = dsProcedures.Tables[0].Rows[pos][fProcedureName].ToString();
            vSqlScript = dsProcedures.Tables[0].Rows[pos][fSqlScript].ToString();

            vt.DBaseNo = v.dBaseNo.NewDatabase;
            vt.SchemasCode = vSchemaName;
            vt.TableName = vProcedureName;
            
            Application.DoEvents();

            if (t.IsNotNull(vSqlScript))
            {
                v.active_DB.runDBaseNo = v.dBaseNo.NewDatabase;
                onay = false;
                onay = db.tStoredProcedureDrop(vt);
                onay = db.tStoredProcedureCreate(vSqlScript, vt);
            }

            Thread.Sleep(100);

            return onay;
        }

        #endregion Procedures İşlemleri

        #region Functions İşlemleri
        private void singleFunctionCreate()
        {
            if (t.IsNotNull(dsFunctions))
            {
                bool onay = true;

                v.active_DB.runDBaseNo = v.dBaseNo.NewDatabase;

                string tName = dsFunctions.Tables[0].Rows[dNFunctions.Position][fFunctionName].ToString();

                string soru = tName + " isimli function oluşturulacak. Onaylıyor musunuz ?";
                DialogResult cevap = t.mySoru(soru);
                if (DialogResult.Yes == cevap)
                {
                    onay = preparingCreateFunction(dNFunctions.Position);

                    if (onay)
                        t.FlyoutMessage(tName, "Function oluşturuldu...");

                }
            }
        }
        private void functionsCreate()
        {
            if (t.IsNotNull(dsFunctions))
            {
                bool onay = true;
                vTable vt = new vTable();
                v.active_DB.runDBaseNo = v.dBaseNo.NewDatabase;

                string soru = " Tüm function listesi oluşturulacak. Onaylıyor musunuz ?";
                DialogResult cevap = t.mySoru(soru);
                if (DialogResult.Yes == cevap)
                {
                    int length = dsFunctions.Tables[0].Rows.Count;
                    for (int i = 0; i < length; i++)
                    {
                        dNFunctions.Position = i;
                        onay = preparingCreateFunction(dNFunctions.Position);
                        //if (onay == false) break;
                    }
                    t.FlyoutMessage("Function oluşturma", "İşlemler tamamlandı...");
                }
            }
        }
        private bool preparingCreateFunction(int pos)
        {
            bool onay = false;
            string vFunctionName = "";
            string vSqlScript = "";
            vTable vt = new vTable();

            vFunctionName = dsFunctions.Tables[0].Rows[pos][fFunctionName].ToString();
            vSqlScript = dsFunctions.Tables[0].Rows[pos][fSqlScript].ToString();

            vt.DBaseNo = v.dBaseNo.NewDatabase;
            vt.SchemasCode = "dbo";
            vt.TableName = vFunctionName;

            Application.DoEvents();

            if (t.IsNotNull(vSqlScript))
            {
                v.active_DB.runDBaseNo = v.dBaseNo.NewDatabase;
                onay = false;
                onay = db.tFunctionDrop(vt);
                onay = db.tFunctionCreate(vSqlScript, vt);
            }

            Thread.Sleep(100);

            return onay;
        }

        #endregion Functions İşlemleri

        #region sub functions

        private void preparingTablesProceduresFunctionsScripts()
        {
            proceduresTableIPCode = "UST/PMS/MsProjectProcedures.List_L01";
            functionsTableIPCode = "UST/PMS/MsProjectFunctions.List_L01";
            //
            t.Find_DataSet(this, ref dsProcedures, ref dNProcedures, proceduresTableIPCode);
            t.Find_DataSet(this, ref dsFunctions, ref dNFunctions, functionsTableIPCode);
            //
            t.Find_Button_AddClick(this, menuName1, buttonSingFileReadDbWrite, myNavElementClick);
            t.Find_Button_AddClick(this, menuName1, buttonAllFileReadDbWrite, myNavElementClick);
            //
            string TabControlName = "tabControl_SUBVIEW";
            this.tabControl = null;
            this.tabControl = t.Find_Control(this, TabControlName);

            if (this.tabControl == null)
                MessageBox.Show("DİKKAT : tabControl_SUBVIEW isimli control bulunamadı.");
        }
        private void preparingDBCreate()
        {
            if (t.IsNotNull(dsFirm))
            {
                // preparing Tables, Proceduru, Function write database
                t.Find_Button_AddClick(this, menuName2, buttonSingFileReadDbWrite, myNavElementClick);
                t.Find_Button_AddClick(this, menuName2, buttonAllFileReadDbWrite, myNavElementClick);
                // database
                t.Find_Button_AddClick(this, menuName2, buttonDbConnection, myNavElementClick);
                t.Find_Button_AddClick(this, menuName2, buttonDbCreate, myNavElementClick);
                // tables
                t.Find_Button_AddClick(this, menuName2, buttonTablesList, myNavElementClick);
                t.Find_Button_AddClick(this, menuName2, buttonSingleTableCreate, myNavElementClick);
                t.Find_Button_AddClick(this, menuName2, buttonTablesCreate, myNavElementClick);
                t.Find_Button_AddClick(this, menuName2, buttonFieldList, myNavElementClick);
                t.Find_Button_AddClick(this, menuName2, buttonDataView, myNavElementClick);
                t.Find_Button_AddClick(this, menuName2, buttonTriggersCreate, myNavElementClick);
                // procedures
                t.Find_Button_AddClick(this, menuName2, buttonProceduresList, myNavElementClick);
                t.Find_Button_AddClick(this, menuName2, buttonSingleProcedureCreate, myNavElementClick);
                t.Find_Button_AddClick(this, menuName2, buttonProceduresCreate, myNavElementClick);
                // functions
                t.Find_Button_AddClick(this, menuName2, buttonFunctionsList, myNavElementClick);
                t.Find_Button_AddClick(this, menuName2, buttonSingleFunctionCreate, myNavElementClick);
                t.Find_Button_AddClick(this, menuName2, buttonFunctionsCreate, myNavElementClick);
            }
        }
        private bool dbConnction(bool test, bool masterConnect)
        {
            bool onay = false;
            
            if (t.IsNotNull(dsFirm))
            {
                string vDatabaseName = "";
                string vDbPass = "";

                if (v.newFirm_DB.MSSQLConn != null)
                    v.newFirm_DB.MSSQLConn.Close();

                v.newFirm_DB.dBaseNo = v.dBaseNo.NewDatabase;
                v.newFirm_DB.dBType = v.dBaseType.MSSQL;
                v.newFirm_DB.serverName = dsFirm.Tables[0].Rows[dNFirm.Position][fServerNameIP].ToString();
                v.newFirm_DB.databaseName = dsFirm.Tables[0].Rows[dNFirm.Position][fDatabaseName].ToString();
                v.newFirm_DB.userName = dsFirm.Tables[0].Rows[dNFirm.Position][fDbLoginName].ToString();
                vDbPass = dsFirm.Tables[0].Rows[dNFirm.Position][fDbPass].ToString();

                // test veya database create için -master- database ile açılması gerekiyor
                // diğer durumlarda tanımlı olan database ismi atanıyor : 'Mtsk00001234' gibi
                vDatabaseName = v.newFirm_DB.databaseName;
                if (masterConnect)
                    vDatabaseName = "master";

                if (vDbPass != "")
                    v.newFirm_DB.psw = "Password = " + vDbPass + ";"; 
                else v.newFirm_DB.psw = "";

                v.newFirm_DB.connectionText =
                    string.Format(" Data Source = {0}; Initial Catalog = {1}; User ID = {2}; {3} MultipleActiveResultSets = True ",
                    v.newFirm_DB.serverName,
                    vDatabaseName,
                    v.newFirm_DB.userName,
                    v.newFirm_DB.psw);

                v.newFirm_DB.MSSQLConn = new SqlConnection(v.newFirm_DB.connectionText);
                //v.newFirm_DB.MSSQLConn.StateChange += new StateChangeEventHandler(DBConnectStateProject);

                t.WaitFormOpen(v.mainForm, "Database bağlantısı test ediliyor...");
                v.SP_OpenApplication = true;

                //v.newFirm_DB.projectMSSQLConn.Close();

                onay = t.Db_Open(v.newFirm_DB.MSSQLConn);

                v.IsWaitOpen = false;
                t.WaitFormClose();

                if (onay && test && masterConnect) 
                    MessageBox.Show(":) Bağlantı başarıyla sağlanmıştır ...");
            }

            return onay;
        }

        #endregion sub database Create functions

        
    }
}
