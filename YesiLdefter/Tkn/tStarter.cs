using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_Starter
{
    public class tStarter : tToolBox
    {
        public void InitStart()
        {
            tToolBox t = new tToolBox();

            /*
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            
            string version = fileVersionInfo.ProductVersion;
            MessageBox.Show(fileVersionInfo.ToString());
            MessageBox.Show(
                fileVersionInfo.ProductMajorPart.ToString() + "/" +
                fileVersionInfo.ProductMinorPart.ToString() + "/" +
                fileVersionInfo.ProductBuildPart.ToString() + "/" +
                fileVersionInfo.ProductPrivatePart.ToString());
            
            string w_file = fileVersionInfo.FileName.ToString(); //"MyProgram.exe";
            string w_directory = Directory.GetCurrentDirectory();
            */

            v.EXE_PATH = Path.GetDirectoryName(Application.ExecutablePath);
            v.EXE_TempPath = v.EXE_PATH + "\\Temp";
            v.tExeAbout.activeExeName = Application.ProductName + ".exe";
            v.tExeAbout.activePath = Application.StartupPath;
                       

            // output : { 25.03.2019 22:59:22 }
            DateTime dt = File.GetLastWriteTime(System.IO.Path.Combine(v.tExeAbout.activePath, v.tExeAbout.activeExeName));

            string yil = dt.Year.ToString();
            string ay = dt.Month.ToString();
            string gun = dt.Day.ToString();
            string saat = dt.Hour.ToString();
            string dakk = dt.Minute.ToString();

            if (ay.Length == 1) ay = "0" + ay;
            if (gun.Length == 1) gun = "0" + gun;
            if (saat.Length == 1) saat = "0" + saat;
            if (dakk.Length == 1) dakk = "0" + dakk;

            // output = { 20190325_2259 }
            v.tExeAbout.activeVersionNo = yil + ay + gun + "_" + saat + dakk;

            System.Globalization.CultureInfo tr = new System.Globalization.CultureInfo("tr-TR");
            System.Threading.Thread.CurrentThread.CurrentCulture = tr;


            /*
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
            ci.NumberFormat.CurrencySymbol = "tkn";
            ci.NumberFormat.CurrencyDecimalDigits = 2;
            ci.NumberFormat.CurrencyDecimalSeparator = ",";
            ci.NumberFormat.CurrencyGroupSeparator = ".";
            
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            */

            // YesiLdefter.Ini
            // YesiLdefterConnection.Ini
            //
            t.ftpDowloadIniFile();


            t.WaitFormOpen(v.mainForm, "GetMacAdress ...");
            Get_MacAddress();

            t.WaitFormOpen(v.mainForm, "Preparing Connection Strings ...");
            InitPreparingConnection();

            t.WaitFormOpen(v.mainForm, "ManagerDB Connection...");
            Db_Open(v.active_DB.managerMSSQLConn);

            //t.WaitFormOpen(v.mainForm, "Read : SysGlyph ...");
            //SYS_Glyph_Read();

            t.WaitFormOpen(v.mainForm, "Computer Info ...");
            InitLoginComputer();

            t.WaitFormOpen(v.mainForm, "Preparing User Form ...");
            InitLoginUser();

            t.WaitFormOpen(v.mainForm, "Screen Size Get ...");
            Screen_Sizes_Get();

            t.WaitFormOpen(v.mainForm, "Default Project Tables Create ...");
            //zirve için kapatıldı
            ///PROJECT_Default_Tables_Create();
            ///InitPreparingOrder_Variable();
            //****

            t.MSSQL_Server_Tarihi();

            v.SP_UserIN = true;
        }

        #region Variable Set


        void InitVariableLoad()
        {

        }

        void Get_MacAddress()
        {
            /// Read computer network ethernet mac address
            /// 
            String macAddr = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic => nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();

            v.tComputer.Network_MACAddress = macAddr;
        }

        void InitPreparingConnection() // SqlConnection text set
        {
            ///
            /// ------------------------------------------------
            ///
            /// hangi database hangi databaseServer de çalışıyor  
            /// şimdilik manuel set ediyorum
            /// 
            v.active_DB.managerDBType = v.dBaseType.MSSQL;
            v.active_DB.projectDBType = v.dBaseType.MSSQL;
            //v.active_DB.projectDBType = v.dBaseType.MySQL;
                        
            ///
            /// Manager DB Connections
            /// 
            #region
            /*
            v.active_DB.managerServerName = "94.73.145.8";
            v.active_DB.managerDBName = "MSV3DFTRBLT";
            v.active_DB.managerUserName = "user4601";
            v.active_DB.managerPsw = "Password = CanBerk98;";
            */
            
            //v.active_DB.managerServerName = "DESKTOPTKN\\SQLEXPRESS";
            v.active_DB.managerDBName = "MSV3DFTRBLT";
            v.active_DB.managerUserName = "sa";
            v.active_DB.managerPsw = "Password = 1;";
            
            v.active_DB.managerConnectionText =
                string.Format(" Data Source = {0}; Initial Catalog = {1}; User ID = {2}; {3} MultipleActiveResultSets = True ",
                v.active_DB.managerServerName,
                v.active_DB.managerDBName,
                v.active_DB.managerUserName,
                v.active_DB.managerPsw);

            v.active_DB.managerMSSQLConn = new SqlConnection(v.active_DB.managerConnectionText);
            v.active_DB.managerMSSQLConn.StateChange += new StateChangeEventHandler(DBConnectStateManager);

            #endregion

            #region iptal gerek kalmadı


            ///
            /// MySQL Project DB Connections
            /// 
            if (v.active_DB.projectDBType == v.dBaseType.MySQL)
            {
                v.active_DB.projectServerName = "";
                v.active_DB.projectDBName = "";
                v.active_DB.projectUserName = "";
                v.active_DB.projectPsw = "";

                v.active_DB.projectConnectionText =
                    string.Format(" Server = {0}; Database = {1}; Uid = {2}; {3} ",
                    v.active_DB.projectServerName,
                    v.active_DB.projectDBName,
                    v.active_DB.projectUserName,
                    v.active_DB.projectPsw);

                v.active_DB.projectMySQLConn = new MySqlConnection(v.active_DB.projectConnectionText);
            }

            #endregion

            // DİKKAT : BU METODU KULLANMA MASTER-DETAIL de DETAIL kırılıyor
            // v.SP_Conn_Text_Manager_MSSQL = " Server=94.73.145.8; Database=MSV3DFTRBLT; Uid=user4601;Pwd=CanBerk98";

        }

        void InitLoginComputer()
        {
            //MessageBox.Show(v.tComputer.Network_MACAddress);

            /// burada computer hakkında bilgi toplanıyor
            /// computer hakkındaki bilgi merkez datada bulunmakta (MVS3..)
            /// her computer network ethernet macaddresiyle takip edilmekte
            /// (MSV3..) datada computer bilgisi yoksa buradan 
            /// computer register formu açılmakta.
            /// Compter hakkında toplanan bilgiler ekranda müdehale edilemeyecek durumdadır
            /// Kullanıcıdan, hangi firma için kullanacak ise firm_guid istenmektedir
            /// eğer firm_guid yok ise sadece test firmalarını görebilir
            /// Firm_Guid aldığında da bu computer bilgileri sayesinde 
            /// firma için kayıt olan computer sayısı / lisans tespit edilmiş olacak

            string compKey = v.tComputer.Network_MACAddress;

            // eğer bir şekilde eğer macaddresi gelmez ise
            //
            //if (IsNotNull(compKey) == false)
            //{
            //    ComputerAbout();
            //    compKey = v.tComputer.Processor_Id;
            //}

            string tSql = @" Select * from SYS_COMPS where NETWORK_MACADDRESS = '" + compKey + "' ";

            SQL_Read_Execute(v.dBaseNo.Manager, v.ds_Computer, ref tSql, "SYS_COMPS", "InitLoginComputer");

            if (IsNotNull(v.ds_Computer))
            {
                /// computer için planlanmış firma guid bilgisi
                ///
                v.tComp.SP_COMP_ISACTIVE = Convert.ToInt16(v.ds_Computer.Tables[0].Rows[0]["ISACTIVE"].ToString());

                // Aktif ise
                if (v.tComp.SP_COMP_ISACTIVE == 1)
                {
                    v.tComp.SP_COMP_ID = Convert.ToInt32(v.ds_Computer.Tables[0].Rows[0]["ID"].ToString());
                    v.tComp.SP_COMP_FIRM_GUID = v.ds_Computer.Tables[0].Rows[0]["COMP_FIRM_GUID"].ToString();
                    v.tComp.SP_COMP_SYSTEM_NAME = v.ds_Computer.Tables[0].Rows[0]["SYSTEM_NAME"].ToString();
                    v.tComp.SP_COMP_MACADDRESS = v.ds_Computer.Tables[0].Rows[0]["NETWORK_MACADDRESS"].ToString();
                    v.tComp.SP_COMP_PROCESSOR_ID = v.ds_Computer.Tables[0].Rows[0]["PROCESSOR_ID"].ToString();
                }

                // Pasif ise
                if (v.tComp.SP_COMP_ISACTIVE == 0)
                {
                    MessageBox.Show("Bilgisayarınız PASİF durumda. \r\n\r\n Destek ekibini arayarak bilgisayarınız AKTİF ettirebilirsiniz ...", "DİKKAT : ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // diğer modlarda ise
                if (v.tComp.SP_COMP_ISACTIVE > 1)
                {
                    MessageBox.Show("Bilgisayarınız  ( IsAcvite : " + v.tComp.SP_COMP_ISACTIVE.ToString() + " ) durumda.", "DİKKAT : ", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }

            }
            else
            {
                /// computer register için form
                InitRegisterComputer();
            }
        }

        void Screen_Sizes_Get()
        {
            v.Screen_Width = Screen.PrimaryScreen.Bounds.Width - (20 + v.NavBar_Width);
            v.Screen_Height = Screen.PrimaryScreen.Bounds.Height - (90 + v.Ribbon_Height);
        }

        void InitPreparingOrder_Variable()
        {
            //tToolBox t = new tToolBox();

            v.myReportViewFormLoadValue = string.Empty;
            MyProperties_Set(ref v.myReportViewFormLoadValue, v.FormLoad, "BEGIN", string.Empty);
            MyProperties_Set(ref v.myReportViewFormLoadValue, v.FormLoad, "TargetTableIPCode", "T15_MSRPR.T15_MSRPR_03");
            MyProperties_Set(ref v.myReportViewFormLoadValue, v.FormLoad, "TargetTableAlias", "[T15_MSRPR]");
            MyProperties_Set(ref v.myReportViewFormLoadValue, v.FormLoad, "TargetFieldName", "REPORT_CODE");

            v.mySMSViewFormLoadValue = string.Empty;
            MyProperties_Set(ref v.mySMSViewFormLoadValue, v.FormLoad, "BEGIN", string.Empty);
            MyProperties_Set(ref v.mySMSViewFormLoadValue, v.FormLoad, "TargetTableIPCode", "SMSSB.SMSSB_02");
            MyProperties_Set(ref v.mySMSViewFormLoadValue, v.FormLoad, "TargetTableAlias", "[SMSSB]");
            MyProperties_Set(ref v.mySMSViewFormLoadValue, v.FormLoad, "TargetFieldName", "BASLIK_KODU");

            v.mySMSSetupFormLoadValue = string.Empty;
            MyProperties_Set(ref v.mySMSSetupFormLoadValue, v.FormLoad, "BEGIN", string.Empty);
            MyProperties_Set(ref v.mySMSSetupFormLoadValue, v.FormLoad, "TargetTableIPCode", "SMSSB.SMSSB_01");
            MyProperties_Set(ref v.mySMSSetupFormLoadValue, v.FormLoad, "TargetTableAlias", "[SMSSB]");
            MyProperties_Set(ref v.mySMSSetupFormLoadValue, v.FormLoad, "TargetFieldName", "BASLIK_KODU");



            if (v.SP_ConnBool_Manager)
            {
                // Tarih ve Saat değişkenlerini hazırla
                //if (v.BUGUN_TARIH.ToString() == "01.01.0001 00:00:00")
                if (v.BUGUN_TARIH.Year == 1)
                    MSSQL_Server_Tarihi();

                // Sys_Types Listesini hazırla
                //t.SYS_Types_Read();
            }
        }

        #endregion Variable Set

        #region InitRegisterComputer
        void InitRegisterComputer()
        {
            /// Computer hakkındaki verileri topla
            /// 
            ComputerAbout();

            /// Computeri, merkezdeki db ye (MSV3..) kaydedecek formu aç
            /// 
            string FormName = "ms_Computer";
            string FormCode = "UST/T01/MSV/SYS_COMP_F01";// "SYS_COMP_F01";

            string Prop_Navigator = @"
            0=FORMNAME:" + FormName + @";
            0=FORMCODE:" + FormCode + @";
            0=FORMTYPE:DIALOG;
            0=FORMSTATE:NORMAL;
            ";

            OpenForm(null, Prop_Navigator);

        }
        #endregion InitRegisterComputer

        #region InitLoginUser
        void InitLoginUser()
        {
            string FormName = "ms_User";
            string FormCode = "UST/PMS/PMS/SYS_USERLOGIN";
            OpenFormPreparing(FormName, FormCode, v.formType.Dialog);

            /*
            string Prop_Navigator = @"
            0=FORMNAME:" + FormName + @";
            0=FORMCODE:" + FormCode + @";
            0=FORMTYPE:DIALOG;
            0=FORMSTATE:NORMAL;
            ";
            OpenForm(null, Prop_Navigator);
            */

            //v.SP_UserLOGIN = true;
            //v.SP_UserLOGIN = false;

        }
        #endregion InitLoginUser

        #region orders

        public void PROJECT_Default_Tables_Create()
        {
            tToolBox t = new tToolBox();

            v.active_DB.runDBaseNo = v.dBaseNo.Project;

            t.tTableFind("HP_FIRMS");
            t.tTableFind("HP_COMPS");
            t.tTableFind("HP_USERS");
            t.tTableFind("HP_UPDATES");

            t.tTableFind("SYS_TYPES_H");
            t.tTableFind("SYS_TYPES_L");
            t.tTableFind("SYS_TYPES_T");
            t.tTableFind("SYS_TYPES_U");

            t.tTableFind("SYS_VARIABLES");
            t.tTableFind("MS_VARIABLES");

            t.tTableFind("HP_CARI");
            t.tTableFind("HP_ADRES");
            t.tTableFind("HP_ILETISIM");

            t.tTableFind("HP_FINANS");
            t.tTableFind("HP_VEZNE");
            t.tTableFind("FN_LINEVOL");
            t.tTableFind("FN_LINEVOL_BA");

            t.tTableFind("HP_URUN");
            t.tTableFind("STK_FIYAT");
            t.tTableFind("STK_GCB");
            t.tTableFind("STK_GCS");
            t.tTableFind("SYS_OTV");

            t.tTableFind("AJ_LINEDOC");
            t.tTableFind("AJ_LINETALK");

            //t.tStoredProceduresFind("");

            t.tStoredProceduresFind("prc_AJ_LINEDOC");
            t.tStoredProceduresFind("prc_CLIENT_LIST");
            t.tStoredProceduresFind("prc_FIRM_IN_USERS");
            t.tStoredProceduresFind("prc_STK_FIYAT_GET");
            t.tStoredProceduresFind("prc_STK_GCS_CALC");
            t.tStoredProceduresFind("prc_SYS_VARIABLES");
            t.tStoredProceduresFind("prc_SYS_VARIABLES_FULL");
            t.tStoredProceduresFind("prc_VEZNE_ITEMS");


            //t.tTriggerFind("");

            t.tTriggerFind("trg_AJ_LINEDOC");
            t.tTriggerFind("trg_FN_LINEVOL");
            t.tTriggerFind("trg_HP_CARI");
            t.tTriggerFind("trg_HP_FINANS");
            t.tTriggerFind("trg_HP_URUN");
            t.tTriggerFind("trg_HP_VEZNE");
            t.tTriggerFind("trg_STK_FIYAT");
            t.tTriggerFind("trg_STK_GCS");
            t.tTriggerFind("trg_SYS_OTV");
            //t.tTriggerFind("");
            //t.tTriggerFind("");


            //t.tPreparingData("");

            t.tPreparingData("data_SYS_TYPES_L");
            // şimdilik gerek yok
            //t.tPreparingData("data_SYS_TYPES_L_AVK");

            // yeni hazırlanacak
            ///t.tPreparingData("data_SYS_TYPES_T");


        }

        public void MSV3_Tables_Create()
        {
            tToolBox t = new tToolBox();

            v.active_DB.runDBaseNo = v.dBaseNo.Manager;

            t.tTableFind("MS_TABLES");
            t.tTableFind("MS_FIELDS");
            t.tTableFind("MS_TABLES_IP");
            t.tTableFind("MS_FIELDS_IP");

            t.tTableFind("MS_DC");
            t.tTableFind("MS_DC_LINE");

            t.tTableFind("MS_GROUPS");
            t.tTableFind("MS_PROPERTIES");
            t.tTableFind("MS_REPORTS");
            t.tTableFind("MS_TYPES");
            t.tTableFind("MS_VARIABLES");
            t.tTableFind("MS_ITEMS");
            t.tTableFind("MS_LAYOUT");

            t.tTableFind("SYS_TYPES_H");
            t.tTableFind("SYS_TYPES_L");
            t.tTableFind("SYS_TYPES_T");

            t.tTableFind("SYS_FIRMS");
            t.tTableFind("SYS_COMPS");
            t.tTableFind("SYS_USERS");
            t.tTableFind("SYS_UPDATES");


            t.tTriggerFind("trg_SYS_COMPS");
            t.tTriggerFind("trg_SYS_FIRMS");
            t.tTriggerFind("trg_SYS_USERS");

        }

        public void MSV3_Database_Update()
        {


        }

        #endregion orders
    }
}
