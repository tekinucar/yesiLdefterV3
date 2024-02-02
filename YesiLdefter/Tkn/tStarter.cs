using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
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
            v.EXE_ScriptsPath = v.EXE_PATH + "\\Scripts";
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
            t.ftpDownloadIniFile();

            t.WaitFormOpen(v.mainForm, "GetMacAdress ...");
            Get_MacAddress();

            t.WaitFormOpen(v.mainForm, "Preparing Connection Strings ...");
            InitPreparingConnection();

            t.WaitFormOpen(v.mainForm, "ManagerDB Connection...");
            Db_Open(v.active_DB.managerMSSQLConn);

            //t.WaitFormOpen(v.mainForm, "Read : SysGlyph ...");
            //SYS_Glyph_Read();

            t.WaitFormOpen(v.mainForm, "Preparing User Form ...");
            setLoginSkins();

            if (v.active_DB.localDbUses == false)
                InitLoginUser(); // Ustad YesiLdester user girişi
            else InitTabimLoginUser();

            t.WaitFormOpen(v.mainForm, "Computer Info ...");
            InitLoginComputer();

            t.WaitFormOpen(v.mainForm, "Screen Size Get ...");
            Screen_Sizes_Get();

            t.MSSQL_Server_Tarihi();

            t.YilAyRead();

            v.SP_UserIN = true;
        }

        void setLoginSkins()
        {
            #region appOpenSetDefaaultSkin
            WindowsFormsSettings.EnableFormSkins();
            if (v.active_DB.mainManagerDbUses)
                UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Whiteprint);
            else
                UserLookAndFeel.Default.SetSkinStyle(SkinSvgPalette.Office2019White.Default);//  Yale);
            #endregion
        }

        #region Variable Set

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
            v.active_DB.ustadCrmDBType = v.dBaseType.MSSQL;
            v.active_DB.projectDBType = v.dBaseType.MSSQL;
                        
            ///
            /// main Manager DB Connections
            /// 
            #region
            
            v.active_DB.managerUserName = "sa";
            
            if (v.active_DB.mainManagerDbUses)
                 v.active_DB.managerPsw = v.mainManagerPass;
            else v.active_DB.managerPsw = v.publishManagerPass;

            v.active_DB.managerConnectionText =
                string.Format(" Data Source = {0}; Initial Catalog = {1}; User ID = {2}; {3} MultipleActiveResultSets = True ",
                v.active_DB.managerServerName,
                v.active_DB.managerDBName,
                v.active_DB.managerUserName,
                v.active_DB.managerPsw);

            v.active_DB.managerMSSQLConn = new SqlConnection(v.active_DB.managerConnectionText);
            v.active_DB.managerMSSQLConn.StateChange += new StateChangeEventHandler(DBConnectStateManager);
            #endregion

            ///
            /// publish Manager DB Connections
            /// 
            #region
            v.publishManager_DB.dBaseNo = v.dBaseNo.publishManager;
            v.publishManager_DB.userName = "sa";
            v.publishManager_DB.psw = v.publishManagerPass;
            v.publishManager_DB.connectionText =
                string.Format(" Data Source = {0}; Initial Catalog = {1}; User ID = {2}; {3} MultipleActiveResultSets = True ",
                v.publishManager_DB.serverName,
                v.publishManager_DB.databaseName,
                v.publishManager_DB.userName,
                v.publishManager_DB.psw);

            v.publishManager_DB.MSSQLConn = new SqlConnection(v.publishManager_DB.connectionText);
            v.publishManager_DB.MSSQLConn.StateChange += new StateChangeEventHandler(DBConnectStateManager);
            #endregion

            ///
            /// UstadCRM DB Connections
            /// 
            #region

            //v.active_DB.ustadCrmDBName = "UstadCRM";
            v.active_DB.ustadCrmUserName = "sa";
            
            if (v.active_DB.mainManagerDbUses)
                 v.active_DB.ustadCrmPsw = v.mainManagerPass;
            else v.active_DB.ustadCrmPsw = v.publishManagerPass;

            v.active_DB.ustadCrmConnectionText =
                string.Format(" Data Source = {0}; Initial Catalog = {1}; User ID = {2}; {3} MultipleActiveResultSets = True ",
                v.active_DB.ustadCrmServerName,
                v.active_DB.ustadCrmDBName,
                v.active_DB.ustadCrmUserName,
                v.active_DB.ustadCrmPsw);

            v.active_DB.ustadCrmMSSQLConn = new SqlConnection(v.active_DB.ustadCrmConnectionText);
            v.active_DB.ustadCrmMSSQLConn.StateChange += new StateChangeEventHandler(DBConnectStateManager);
            
            #endregion

            ///
            /// master DB Connections (MSSQL.master)
            /// 
            #region

            v.active_DB.masterDBName = "master";
            v.active_DB.masterUserName = "sa";

            if (v.active_DB.mainManagerDbUses)
                 v.active_DB.masterPsw = v.mainManagerPass;
            else v.active_DB.masterPsw = v.publishManagerPass;

            v.active_DB.masterConnectionText =
                string.Format(" Data Source = {0}; Initial Catalog = {1}; User ID = {2}; {3} MultipleActiveResultSets = True ",
                v.active_DB.masterServerName,
                v.active_DB.masterDBName,
                v.active_DB.masterUserName,
                v.active_DB.masterPsw);

            v.active_DB.masterMSSQLConn = new SqlConnection(v.active_DB.masterConnectionText);
            v.active_DB.masterMSSQLConn.StateChange += new StateChangeEventHandler(DBConnectStateManager);

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

            string tSql = @" Select * from UstadComputers where NetworkMacAddress = '" + compKey + "' ";

            SQL_Read_Execute(v.dBaseNo.UstadCrm, v.ds_Computer, ref tSql, "UstadComputers", "InitLoginComputer");

            if (IsNotNull(v.ds_Computer))
            {
                /// computer için planlanmış firma guid bilgisi
                ///
                v.tComp.SP_COMP_ISACTIVE = Convert.ToBoolean(v.ds_Computer.Tables[0].Rows[0]["IsActive"].ToString());

                // Aktif ise
                if (v.tComp.SP_COMP_ISACTIVE)
                {
                    v.tComp.SP_COMP_ID = Convert.ToInt32(v.ds_Computer.Tables[0].Rows[0]["ComputerId"].ToString());
                    v.tComp.SP_COMP_FIRM_GUID = v.ds_Computer.Tables[0].Rows[0]["FirmGUID"].ToString();
                    v.tComp.SP_COMP_SYSTEM_NAME = v.ds_Computer.Tables[0].Rows[0]["SystemName"].ToString();
                    v.tComp.SP_COMP_MACADDRESS = v.ds_Computer.Tables[0].Rows[0]["NetworkMacAddress"].ToString();
                    v.tComp.SP_COMP_PROCESSOR_ID = v.ds_Computer.Tables[0].Rows[0]["ProcessorId"].ToString();
                }

                // Pasif ise
                if (v.tComp.SP_COMP_ISACTIVE == false)
                {
                    MessageBox.Show("Bilgisayarınız PASİF durumda. \r\n\r\n Destek ekibini arayarak bilgisayarınız AKTİF ettirebilirsiniz ...", "DİKKAT : ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                /*
                // diğer modlarda ise
                if (v.tComp.SP_COMP_ISACTIVE)
                {
                    MessageBox.Show("Bilgisayarınız  ( IsAcvite : " + v.tComp.SP_COMP_ISACTIVE.ToString() + " ) durumda.", "DİKKAT : ", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                */
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
            v.Primary_Screen_Width = Screen.PrimaryScreen.Bounds.Width;
            v.Primary_Screen_Height = Screen.PrimaryScreen.Bounds.Height - 50;
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
            string FormCode = "UST/CRM/ABO/Computer";
            OpenFormPreparing(FormName, FormCode, v.formType.Dialog);
        }
        #endregion InitRegisterComputer

        #region InitLoginUser
        void InitLoginUser()
        {
            string FormName = "ms_User";
            string FormCode = "UST/CRM/ABO/UstadUserLogin";
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

        void InitTabimLoginUser()
        {
            string FormName = "ms_TabimMtsk";
            string FormCode = "UST/MEB/TB1/TbmWelcome";
            OpenFormPreparing(FormName, FormCode, v.formType.Dialog);
        }


        #region orders

        
        #endregion orders
    }
}
