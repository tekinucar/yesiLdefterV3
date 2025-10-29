using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;

namespace YesiLcihazlar
{
  /*
    public static class v
    {
        public static bool bIsConnected = false;//the boolean value identifies whether the device is connected
        public static int iMachineNumber = 1;
        public static int idwErrorCode = 0;

        public static DataTable dsUserInfo = null;
        public static DataTable dsFingerLogData = null;
    }
*/

    public static class v
    {
        public static DataSet dsCihazList = null;
        public static DataSet dsCihazLogIcmal1 = null;
        public static DataSet dsCihazLogIcmal2 = null;
        public static DataSet dsCihazLogDetay = null;
        
        public static DataSet dsUserInfo = null;
        public static DataSet dsFingerLogData = null;

        public static DataSet dsCihazEmirYeniList = null;
        public static DataSet dsCihazEmirEskiList = null;

        public static DataSet dsCihazTest = null;


        // picture nesnesi ile Save() functionu arasında veri taşıyıcı
        public static Form mainForm { get; set; }

        public static string spFormName = "ustad";
        public static string registryPath = "Software\\Üstad\\YesiLdefter";
        public static Control formLastActiveControl { get; set; }

        public static Image con_Image_Original = null;
        public static byte[] con_Images = null;
        public static string con_Images_FieldName { get; set; }
        public static string con_Images_Path { get; set; }
        public static string con_Images_MasterPath { get; set; }
        public static Rectangle con_Images_Selection;
        public static bool con_Images_Selecting;
        public static long con_Images_Length = 0;

        //public static string con_Images_Name { get; set; }

        //public static DevExpress.Utils.Animation.TransitionManager transitionManager1 = 
        //    new DevExpress.Utils.Animation.TransitionManager();

        public static WebCam_Capture.WebCamCapture webCamCapture1 = null;

        public static int sayac = 0;
        public static bool ControlListView = false; //true;
        public static string ControlList { get; set; }

        // *** DB Variables *** // 
        public static string ENTER = "\r\n";
        public static string ENTER2 = "\r\n\r\n";

        //public static string SP_SERVER_PCNAME { get; set; }    // PC NAME
        public static string db_MASTER_DBNAME = "master";    // { get; set; }
        //public static string db_MANAGER_DBNAME { get; set; } // "ManagerServer"
        public static string db_MAINMANAGER_DBNAME = "SystemMS"; // { get; set; } // "ManagerServer"
        //public static string db_PROJE_DBNAME { get; set; }   // SP_DATABASENAME
        //public static string db_USER_NAME { get; set; }
        //public static string db_PASSWORD { get; set; }

        public static string SP_MENU { get; set; }
        public static string EXE_PATH = string.Empty;
        public static string sp_Sakla = string.Empty;


        public static Boolean SP_UserIN = false;
        public static Boolean SP_UserLOGIN = true;

        public static Boolean SP_ConnectBool = false;
        public static Boolean SP_ConnBool_Manager = false;
        public static Boolean SP_ConnBool_Manager_Old = false;
        public static Boolean SP_ConnBool_Project = false;
        public static Boolean SP_ConnBool_Project_Old = false;

        public static Boolean SP_ConnBool_FirmPeriod = false;
        public static Boolean SP_ConnBool_FirmMain = false;

        public static string SP_Conn_Caption { get; set; }

        public static string SP_Conn_Text_Master { get; set; }
        //public static string SP_Conn_Text_Manager { get; set; }
        public static string SP_Conn_Text_MainManager { get; set; }
        //public static string SP_Conn_Text_Project { get; set; }

        public static string SP_Conn_Text_Proje_MySQL { get; set; }
        public static string SP_Conn_Text_MySQL { get; set; }

        public static SqlConnection SP_Conn_Master_MSSQL = new SqlConnection();
        public static SqlConnection SP_Conn_Manager_MSSQL = new SqlConnection();
        //public static SqlConnection SP_Conn_MainManager_MSSQL = new SqlConnection();
        //public static SqlConnection SP_Conn_Proje_MSSQL = new SqlConnection();

        //public static MySqlConnection SP_Conn_Proje_MySQL = new MySqlConnection();


        public static Boolean IsWaitOpen = false;
        public static Boolean SP_OpenApplication = false;
        public static Boolean SP_ConnectBool_TR = false;
        public static Boolean SP_ConnectBool_Source = false;
        public static Boolean SP_ConnectBool_Target = false;

        public static string SP_Connect_Source_DBType = "";
        public static string SP_Connect_Target_DBType = "";

        public static string db_MSSQL = "MSSQL";
        public static string db_MySQL = "MySQL";


        /* Transfer işlemleri için */
        public static SqlConnection SP_Conn_MsSQL = new SqlConnection(null);
        public static SqlConnection SP_Conn_MsSQL_TR = new SqlConnection(null);
        //public static MySqlConnection SP_Conn_MySQL = new MySqlConnection(null);
        //--

        // *** Global DataSet *** //
        public static DataSet dsMS_Tables_IP = new DataSet();
        public static DataSet ds_TypesList = new DataSet();
        public static DataSet ds_MsTypesList = new DataSet();
        public static DataSet ds_Firm = new DataSet();
        public static DataSet ds_Variables = new DataSet();
        public static DataSet ds_Icons = new DataSet();
        public static DataSet ds_Computer = new DataSet();
        public static DataSet ds_ExeUpdates = new DataSet();
        public static DataSet ds_LookUpTableList = new DataSet();




        /// Hangi SMS bilgisi isteniyorsa seçilir.
        //public static SP_SMS : string
        //{
        public static string SMS_KullaniciAdi { get; set; }
        public static string SMS_Sifre { get; set; }
        public static string SMS_BayiiKodu { get; set; }
        public static string SMS_Origin { get; set; }

        public static bool mailSent = false;
        //}

        #region IP_VIEW_TYPE
        // * 1,  'Data View'
        // * 2,  'Kriter View'
        // * 3,  'Kategori View'
        // * 4,  'HGS View'
        public static byte IPdataType_DataView = 1;
        public static byte IPdataType_Kriterler = 2;
        public static byte IPdataType_Kategori = 3;
        public static byte IPdataType_HGSView = 4;
        #endregion IP_VIEW_TYPE

        #region Layout type

        public static string lyt_Name = "tLayout_";
        public static string lyt_menu = "menu";
        public static string lyt_dockPanel = "dockPanel";
        public static string lyt_tableLayoutPanel = "tableLayoutPanel";
        public static string lyt_splitContainer = "splitContainer";
        public static string lyt_groupControl = "groupControl";
        public static string lyt_panelControl = "panelControl";
        public static string lyt_tabPane = "tabPane";
        public static string lyt_tabNavigationPage = "tabNavigationPage";
        public static string lyt_navigationPane = "navigationPane";
        public static string lyt_navigationPage = "navigationPage";
        public static string lyt_backstageViewControl = "backstageViewControl";
        public static string lyt_backstageViewTabItem = "backstageViewTabItem";
        public static string lyt_backstageViewButtonItem = "backstageViewButtonItem";
        public static string lyt_backstageViewItemSeparator = "backstageViewItemSeparator";
        public static string lyt_childBackstageView = "childBackstageView";
        public static string lyt_tabControl = "tabControl";
        public static string lyt_tabPage = "tabPage";

        public static string lyt_documentManager = "documentManager";
        public static string lyt_dataWizard = "dataWizard";

        public static string lyt_webBrowser = "webBrowser";
        public static string lyt_headerPanel = "headerPanel";
        public static string lyt_editPanel = "editPanel";
        public static string lyt_labelControl = "labelControl";
        public static string lyt_BarcodeControl1 = "barcodeControl1";


        //public static string lyt_ = "";
        //public static string lyt_ = "";
        //public static string lyt_ = "";

        #endregion

        // *** Save *** //
        #region Save

        public static byte SP_DataBaseType = 1; /* 1 = MSSQL, 2 = MySQl,  ???   */

        public static byte KAYDET = 0;
        public static byte N_SATIR_KAYDET = 1;
        public static byte SQL_OLUSTUR = 2;
        public static byte SQL_OLUSTUR_IDENTITY_YOK = 3;
        public static byte N_SATIR_SQL_OLUSTUR = 4;

        public enum Save : byte { KAYDET, N_SATIR_KAYDET, SQL_OLUSTUR, SQL_OLUSTUR_IDENTITY_YOK, N_SATIR_SQL_OLUSTUR }

        public static string SP_MSSQL_BEGIN = " begin transaction " + ENTER;
        public static string SP_MSSQL_END = " commit transaction " + ENTER;
        public static string SP_MSSQL_LINE_END = ENTER;

        public static string SP_MySQL_BEGIN = " start transaction; " + ENTER;
        public static string SP_MySQL_END = " commit; " + ENTER;
        public static string SP_MySQL_LINE_END = ";" + ENTER;

        #endregion Save 

        #region Enum

        public enum myProperties
        {
            Block,
            Row,
            Column
        };

        //public enum TableType : byte
        //{
        //    None = 0, Table = 1, View = 2, StoredProcedure = 3,
        //    Function = 4, Trigger = 5, Select = 6  
        //}

        public enum dBaseNo : byte
        {
            None = 0,
            Master = 1,
            Manager = 2,
            //MainManager = 3, İPTAL
            Project = 4,
            FirmMainDB = 5,
            FirmPeriodDB = 6
        }

        public enum dBaseType : byte
        {
            None,
            MSSQL,
            MySQL
        }

        public enum TableType
        {
            None, Table, View, StoredProcedure,
            Function, Trigger, Select
        }

        public enum ShowParamView : byte
        {
            TapPage = 0, Form = 1
        }

        public static byte ond_ShowParamView = 0;

        public enum Tumu : byte
        {
            None = 0, False = 1, True = 2
        }

        public enum ReportTool : byte
        {
            DevExpress = 1, FastReport = 2
        }

        //public enum DataReadType : byte
        //{
        //    None = 0, NotReadData = 1, ReadRefID = 2, DetailTable = 3, 
        //    SubDetailTable = 4, DataCollection = 5, ReadSubView = 6
        //}

        public enum DataReadType
        {
            None, NotReadData, ReadRefID, DetailTable,
            SubDetailTable, DataCollection, ReadSubView
        }

        public enum tInvokeMember
        {
            none,
            click,
            onchange,
            submit
        };

        public enum tWorkTD
        {
            NewAndRef,
            NewData,
            Refresh_SubDetail,
            Refresh_Data,
            Save,

        }

        public enum tWorkWhom
        {
            All,
            Only,
            Childs
        }

        public enum tFirmListType
        {
            OnlySelect,
            AllFirm
        }

        //public enum tControlName 
        //{
        //    Column_ ,
        //    item_
        //}

        /*
    11	Kayıt	1
12	Giriş	1
13	Çıkış	1
14	Giriş-Çıkış	1
15	Görüş	1
16	Sayım	1
21	Kayıt	2
22	Giriş	2
23	Çıkış	2
24	Giriş-Çıkış	2
25	Mola	2
    */
        public enum cihazCalismaTipi : byte
        {
            None = 0,
            Kayit = 11,
            Giris = 12,
            Cikis = 13,
            GirisCikis = 14,
            Gorus = 15,
            Sayim = 16
        }

        public enum tEnabled
        {
            Enable,
            Disable
        }
        #endregion Enum

        // *** Object Variables *** //
        #region OBJECT List

        public static int obj_CommonControl { get; set; }
        public static int obj_SimpleButton { get; set; }
        public static int obj_LabelControl { get; set; }

        public static int obj_BarcodControl { get; set; }
        public static int obj_PictureControl { get; set; }
        public static int obj_Load_RUN { get; set; }

        public static int obj_DataNavigator { get; set; }

        public static Int16 obj_vw_GridView = 1010; //{ get; set; }
        public static Int16 obj_vw_BandedGridView = 1020; //{ get; set; }
        public static Int16 obj_vw_AdvBandedGridView = 1030; //{ get; set; }
        public static Int16 obj_vw_GridLayoutView = 1040; //{ get; set; }
        public static Int16 obj_vw_WinExplorerView = 1050; //{ get; set; }
        public static Int16 obj_vw_TileView = 1060; //{ get; set; }
        public static Int16 obj_vw_CardView = 1070; //{ get; set; }


        public static Int16 obj_vw_VGridSingle = 2010; //{ get; set; }
        public static Int16 obj_vw_VGridMulti = 2012; //{ get; set; }
        public static Int16 obj_vw_TreeListView = 2020; //{ get; set; }
        public static Int16 obj_vw_PivotGrid = 2030; //{ get; set; }

        public static Int16 obj_vw_DataLayoutView = 3010; //{ get; set; }
        public static Int16 obj_vw_WizardControl = 4010; //{ get; set; }

        public static Int16 obj_vw_InputBoxControl { get; set; }
        public static byte obj_vw_KriterView { get; set; }
        public static byte obj_vw_CategoryView { get; set; }
        public static byte obj_vw_HVGView { get; set; }

        #endregion

        #region DockType
        public static byte dock_None = 0;
        public static byte dock_Left = 1;
        public static byte dock_Right = 2;
        public static byte dock_Top = 3;
        public static byte dock_Bottom = 4;
        public static byte dock_Fill = 5;
        #endregion


        #region /// *** Global Variables *** //

        public static string con_SQL { get; set; }
        public static string SQL { get; set; }
        public static string SQLSave { get; set; }
        public static string SQLState { get; set; }
        public static string Kullaniciya_Mesaj_Var { get; set; }
        public static Boolean Kullaniciya_Mesaj_Show = false;
        public static Boolean Onay { get; set; }

        public static Boolean Takip = false;
        public static Boolean Takip_Find = false;
        public static Int16 Takip_Adim = 0;
        public static string bosluk = string.Empty;
        public static string Takip_Listesi = string.Empty;
        public static string FormLoad = "FormLoad";
        public static string ProjectName { get; set; }

        public static string Wait_Caption = " Lütfen bekleyin...";
        public static string Wait_Desc_ProgramYukleniyor = "Program yükleniyor...";
        public static string Wait_Desc_ProgramYukDevam = "Program yüklenmeye devam ediyor...";
        public static string Wait_Desc_DBBaglanti = " Bağlantı kuruluyor...";
        public static string DBRec_Insert = " Yeni kayıt işlemi gerçekleşti... ";
        public static string DBRec_Update = " adet kayıt düzeltme işlemi gerçekleşti... ";
        public static string DBRec_ListAdd = " Başarıyla listeye eklenmiştir...";
        public static string dataStateNull = "DataState:null";
        public static string dataStateUpdate = "DataState:Update";

        public static string softCode { get; set; }
        public static string projectCode { get; set; }

        public static int Screen_Width { get; set; }
        public static int Screen_Height { get; set; }
        public static int NavBar_Width { get; set; }
        public static int Ribbon_Height { get; set; }
        public static int Padding4 = 4;
        public static int Padding8 = 8;
        public static int GroupPadding = 4;

        public static int dropTargetRowHandle = -1;
        public static int DropTargetRowHandle
        {
            get { return dropTargetRowHandle; }
            set
            {
                dropTargetRowHandle = value;
                //gridControl1.Invalidate();
            }
        }


        public static Boolean myFormBox_Visible = false;
        //public static Point myGrid_Location { get; set; } şimdilik gerek kalmadı
        //public static Boolean con_SubDetail_Refresh = true;
        public static Boolean con_Refresh = false;
        public static Boolean con_SubWork_Refresh = true;
        public static Boolean con_Cancel = false;
        public static Boolean con_AddIP = false;
        public static Boolean con_Parametre = true;
        public static Boolean con_AutoNewRecords = false;
        public static Boolean con_NewRecords = false;
        public static Boolean con_OnayChange = false;
        public static Boolean con_OnaySave = false;
        public static Boolean con_LkpOnayChange = false;
        public static Boolean con_ColumnValueChanging = false;
        public static Boolean con_PositionChange = false;
        public static Boolean con_ExtraChange = false;
        public static Boolean con_Expression = false;
        public static Boolean con_FormAfterCreateView = false;
        public static Boolean con_DefaultValuePreparing = false;

        public static DataRow con_DataRow = null;

        public static Int16 con_FinansHesapTuru = 0;
        public static int con_FinansHesapID = 0;

        public static int con_SubView_FIRST_POSITION = 0;

        public static int con_Value_Max = 0;
        public static int con_Value_Min = 0;
        public static string con_Value_New = string.Empty;
        public static string con_Value_Old = string.Empty;
        public static string con_Expression_Send_Value = string.Empty;
        public static string con_Expression_View = string.Empty;
        public static string con_Expression_FieldName = string.Empty;
        public static string con_InputBox_FieldName = string.Empty;
        public static string con_AddIP_TableCode = string.Empty;
        public static string con_AddIP_FieldName = string.Empty;
        public static string con_AddIP_Value = string.Empty;
        public static string con_AddListIP_TableIPCode = string.Empty;

        public static string con_Source_ParentControl_Tag_Value = string.Empty;
        public static string con_Menu_Prop_Value = string.Empty;

        public static string con_GotoRecord = string.Empty;
        public static string con_GotoRecord_TableIPCode = string.Empty;
        public static string con_GotoRecord_FName = string.Empty;
        public static string con_GotoRecord_Value = string.Empty;
        public static int con_GotoRecord_Position = -1;

        public static Form con_DragDropForm { get; set; }
        public static string con_DragDropSourceTableIPCode = string.Empty; // veya = FIRST
        public static string con_DragDropTargetTableIPCode = string.Empty; // veya = SECOND 
        public static string con_DragDropOpenFormInTableIPCode = string.Empty; // ve = THIRD 

        public static string con_TableIPCode = string.Empty;
        public static string con_SearchValue = string.Empty;
        public static string con_FormLoadValue = string.Empty;
        public static string con_GridMouseValue = string.Empty;
        public static Boolean con_DragDropEdit = false;

        public static string con_ManuelFormLoadValue = string.Empty;
        public static string myReportViewFormLoadValue = string.Empty;
        public static string mySMSViewFormLoadValue = string.Empty;
        public static string mySMSSetupFormLoadValue = string.Empty;

        public static string Properties = "Properties";
        public static string PropertiesPlus = "PropertiesPlus";
        public static string PropertiesNav = "PropertiesNav";
        public static string SearchEngine = "SearchEngine";
        public static string ButtonEdit = "Buttons";



        public static Color Validate_New = Color.LightGoldenrodYellow; //Yellow; //Color.LightGoldenrodYellow;
        public static Color Validate_Ok = Color.GreenYellow;
        public static Color Validate_Not = Color.MistyRose;
        public static Color AppearanceFocusedColor = Color.MediumSpringGreen;
        public static Color ribbonColor = AppearanceFocusedColor;

        //Color.LightSeaGreen; //Color.GreenYellow;  //Color.PaleGreen;

        public static Color colorFocus = AppearanceFocusedColor;// System.Drawing.Color.LightGreen;
        public static Color colorSave = AppearanceFocusedColor;//System.Drawing.Color.LimeGreen;
        public static Color colorNew = AppearanceFocusedColor;//System.Drawing.Color.Turquoise;
        public static Color colorNavigator = AppearanceFocusedColor;//System.Drawing.Color.PaleTurquoise;
        public static Color colorDelete = System.Drawing.Color.LightPink;
        public static Color colorOrder = AppearanceFocusedColor;//System.Drawing.Color.LemonChiffon;

        public static Color colorExit = System.Drawing.Color.Gold; //color3

        public static Color AppearanceTextColor = Color.DarkGreen;  //MediumSeaGreen;
        public static Color AppearanceFocusedTextColor = Color.Black;
        public static Color AppearanceItemCaptionColor1 = Color.CornflowerBlue; // System.Drawing.Color.Cornsilk;
        public static Color AppearanceItemCaptionColor2 = Color.Coral; // System.Drawing.Color.FloralWhite;


        // *** Sorgu Türleri *** //
        public static byte Bos = 0;
        public static byte EsitVeBuyuk = 1;
        public static byte Buyuk = 2;
        public static byte Esit = 3;
        public static byte EsitVeKucuk = 4;
        public static byte Kucuk = 5;
        public static byte Benzerleri_Tam = 6;
        public static byte Benzerleri_Sol = 7;
        public static byte EsitDegil = 8;




        // *** Date & Time Variables *** //
        public static DateTime DONEM_BASI_TARIH = DateTime.Now.AddDays(-1 * DateTime.Now.Day);
        public static DateTime DONEM_BITIS_TARIH;
        public static DateTime BUGUN_TARIH;// = DateTime.Now;
        public static DateTime BU_HAFTA_BASI_TARIH;
        public static DateTime BU_HAFTA_SONU_TARIH;
        public static DateTime BU_AY_BASI_TARIH;
        public static DateTime BU_AY_SONU_TARIH;
        public static DateTime BIR_HAFTA_ONCEKI_TARIH = DateTime.Now.AddDays(-7);
        public static DateTime IKI_HAFTA_ONCEKI_TARIH = DateTime.Now.AddDays(-14);
        public static DateTime ON_GUN_ONCEKI_TARIH = DateTime.Now.AddDays(-10);
        public static DateTime GECEN_AY_BUGUN = DateTime.Now.AddMonths(-1);
        public static DateTime GELECEK_AY_BUGUN = DateTime.Now.AddMonths(1);

        public static int BUGUN_GUN = 0;
        public static int BUGUN_AY = 0;
        public static int BUGUN_YIL = 0;

        public static string Declare_bugun = string.Empty;
        public static string Declare_Gun = string.Empty;
        public static string Declare_Ay = string.Empty;
        public static string Declare_Yil = string.Empty;

        public static string TARIH_SAAT = DateTime.Now.ToString();
        public static string SAAT_KISA = DateTime.Now.ToShortTimeString();
        public static string SAAT_UZUN = DateTime.Now.ToLongTimeString();
        public static string SAAT_UZUN2 = DateTime.Now.ToLongTimeString();

        // *** Proje Tanımları için *** //

        // *** Çalışılan Firma Tablosu *** //
        public static string ds_TypesNames = "";
        public static string ds_MsTypesNames = "";
        public static string ds_LookUpTableNames = "";

        public static string sp_OpeFormState = "";
        public static string sp_activeSkinName = "";
        public static string sp_deactiveSkinName = "Blueprint";//"High Contrast White";
        //public static SkinSvgPalette sp_DeactiveSkinPalette = SkinSvgPalette.Bezier.HighContrastWhite;
        //public static SkinStyle sp_DeactiveSkin = SkinStyle.Blueprint;

        // firma bilgileri
        // 
        public static int SP_FIRM_ID = 0;
        // false olursa kullanıcı tek firmaya kullanabilir
        // true  olursa kullanıcı aynı anda birden fazla firma kullanabilir
        //
        public static bool SP_FIRM_MULTI = false;
        public static string SP_FIRM_USERLIST { get; set; }
        public static string SP_FIRM_FULLLIST { get; set; }
        public static string SP_FIRM_GUID { get; set; }
        public static string SP_FIRM_NAME = "Tekin UÇAR";//{ get; set; }
        public static string SP_FIRM_TABLENAME { get; set; }
        public static string SP_FIRM_TABLEKEYFNAME { get; set; }
        public static string SP_FIRM_TABLECAPTIONFNAME { get; set; }
        public static string SP_FIRM_REF_FNAME = "LOCAL_ID";
        public static string SP_FIRM_USE_PACKAGE = "ONM_KOBI";
        public static string SP_FIRM_USE_PACKAGE_OLD = "ONM_KOBI";
        public static string SP_FIRM_USE_MENU_TYPE = "toolBox";



        //public static int vt_FIRM_ID = 1;    // 2001   geçiçi değiştirildi, aslı 0
        //public static int vt_SHOP_ID = 2;    // 2002   geçiçi değiştirildi, aslı 0
        //public static int vt_COMP_ID = 6;    // 2003   geçiçi değiştirildi, aslı 0
        public static int vt_PERIOD_ID = 0;  // 2004   donem_ID 

        //public static int vt_USER_ID = 0;         // 2005  
        //public static string vt_USER_CODE = "";   // 2006
        //public static string vt_USER_NAME = "";   // 2007
        public static string vt_USER_SHOP_LIST = "";

        public static bool searchOnay = false;
        public static bool searchSet = false;
        public static bool searchEnter = true;
        public static int searchCount = 0;
        public static string search_onList = "onList";
        public static string search_inData = "inData";
        public static string search_CARI_ARAMA_TD = search_inData; //search_onList;//
        public static string search_STOK_ARAMA_TD = "";
        public static string search_VALUE = "";
        #endregion

        #region Keys
        public static Keys Key_Exit = Keys.Escape;
        public static Keys Key_YeniSatir = Keys.Insert;

        public static Keys Key_SearchEngine = Keys.F1;
        public static Keys Key_Kaydet = Keys.F3;
        public static Keys Key_Yeni = Keys.F4;
        public static Keys Key_KaydetYeni = Keys.F4;
        public static Keys Key_NewSub = Keys.F6;
        public static Keys Key_Listele = Keys.F5; // Listele
        public static Keys Key_ListeHazirla = Keys.F7; // Liste_Hazirla
        public static Keys Key_ListyeEkle = Keys.F7;  // Listeye Ekle
        public static Keys Key_SecCik = Keys.F7;   // Seç ve Çık
        public static Keys Key_BelgeyiAc = Keys.F9;
        public static Keys Key_Next = Keys.Next;
        public static Keys Key_Prior = Keys.Prior;
        #endregion

        #region Navigator

        //10 
        //'12 Ara'); 
        public static byte nv_11_Cikis = 11;
        public static byte nv_13_Sec = 13;
        //20
        public static byte nv_21_Vazgec = 21;
        public static byte nv_22_Kaydet = 22;//24;
        public static byte nv_23_Kaydet_Yeni = 23;
        public static byte nv_24_Kaydet_Cik = 24; //22;
        public static byte nv_26_Sil_Satir = 26;
        public static byte nv_27_Sil_Fis = 27;
        public static byte nv_28_Iptal_Navg_Ref = 28;
        public static byte nv_29_Iptal_Form_Kpt = 29;
        //30
        public static byte nv_31_Yeni_Form_Ici = 31;
        public static byte nv_32_Yeni_Form_Ile = 32;
        public static byte nv_33_Yeni_Fis_Form_Ici = 33;
        public static byte nv_34_Yeni_Fis_Form_Ile = 34;
        public static byte nv_35_Fis_Listesi = 35;
        //40
        public static byte nv_41_En_Sona = 41;
        public static byte nv_42_Sonraki_Syf = 42;
        public static byte nv_43_Sonraki = 43;
        public static byte nv_44_Kayit_Adedi = 44;
        public static byte nv_45_Onceki = 45;
        public static byte nv_46_Onceki_Syf = 46;
        public static byte nv_47_En_Basa = 47;
        //50
        public static byte nv_51_Karti_Ac = 51;
        public static byte nv_52_Fisi_Ac = 52;
        public static byte nv_53_Form_Ac = 53;
        public static byte nv_54_IP_Ac = 54;
        public static byte nv_55_IB_Ac = 55;
        public static byte nv_56_CP_Ac = 56;
        //101 Procedure RUN');
        public static byte nv_101_Pro_RUN = 101;
        public static byte nv_102_AUTO_INS = 102;


        public enum tNavigatorButton : byte
        {
            nv_11_Cikis = 11,
            nv_12_Listele = 12,
            nv_13_Sec = 13,
            nv_14_Ekle = 14,
            nv_15_Liste_Hazirla = 15,
            nv_21_Vazgec = 21,
            nv_22_Kaydet_Cik = 22,
            nv_23_Kaydet_Yeni = 23,
            nv_24_Kaydet = 24,
            nv_26_Sil_Satir = 26,
            nv_27_Sil_Fis = 27,
            nv_28_Iptal_Navg_Ref = 28,
            nv_29_Iptal_Form_Kpt = 29,
            nv_41_En_Sona = 41,
            nv_42_Sonraki_Syf = 42,
            nv_43_Sonraki = 43,
            nv_44_Kayit_Adedi = 44,
            nv_45_Onceki = 45,
            nv_46_Onceki_Syf = 46,
            nv_47_En_Basa = 47,
            nv_50_Hesap_Ac = 50,
            nv_51_Karti_Ac = 51,
            nv_52_Yeni_Kart_FormIle = 52,
            nv_53_Yeni_Hesap = 53,
            nv_53_Yeni_Hesap_Vazgec = 153,
            nv_54_Yeni_Alt_Hesap = 54,
            nv_55_Run_Expression = 55,
            nv_56_Run_TableIPCode = 56,
            nv_57_Input_Box = 57,
            nv_58_Search = 58,
            nv_59_SubView_Open = 59,
            nv_71_Onay_EkleKaldir = 71,
            nv_72_Grup_AcKapa = 75,
            nv_81_Yazici = 81,

            nv_101_Pro_RUN = 101
        }

        public enum tBeforeAfter
        {
            Before,
            After
        }

        public enum tButtonType : byte
        {
            btNone = 0,
            btEnter = 1,
            btEscape = 2,
            btCikis = 11,
            btSihirbazDevam = 49,
            btSihirbazGeri = 48,
            btListele = 12,
            btSecCik = 13,
            btListeyeEkle = 14,
            btListeHazirla = 15,
            btKaydet = 22,
            btKaydetYeni = 23,
            btKaydetCik = 24,
            btKaydetSonraki = 25,
            btSilSatir = 26,
            btSilFis = 27,
            btEnSona = 41,
            btSonrakiSayfa = 42,
            btSonraki = 43,
            btOnceki = 45,
            btOncekiSayfa = 46,
            btEnBasa = 47,
            btHesapAcOku = 50,
            btKartAc = 51,
            btYeniHesapForm = 52,
            btYeniHesap = 53,
            btYeniAltHesap = 54,
            btFormulleriHesapla = 55, // nv_55_Run_Expression
            btDataTransferi = 56,     // nv_56_Run_TableIPCode
            btInputBox = 57,
            btArama = 58,
            btOpenSubView = 59,
            btYeniSatir = 60,
            btOnayEkleKaldir = 71,
            btCollExp = 72,
            btYazici = 81,
            btRaporAgaci = 82,
            btEk1 = 91,
            btEk2 = 92
        }

        #endregion


        public enum cihazTalepTipi : byte
        {
            chConnect = 0,
            chDisconnnect = 1,
            chTest = 2,
            chLogCount = 3,
            chGetTarihSaat = 4,
            chSetTarihSaat = 5,
            chGetAllUserAndFPs = 6,
            chGetAllLogs = 7,
            chSetAllUserAndFPs = 8,
            chReset = 9,

            chSetNewUser = 11,
            chGetNewUserFP = 12,
            chSetNewUserSayim = 13,

            chSetOldUser = 14, // eski userin bio izini set için
            chGetOldUserFP = 15, // eski userin kayıt cihazından bio izini almak
            chSetOldUserSayim = 16, // eski userin yeni bio izini sayim cihazına gönder

            chSetSayim = 21,   // sadece yeni kul sayım cihaz ekle
            chSetTahliye = 22,
            chSetGorev = 23,
            chSetGorus = 24,

            chGetGorev = 33,
            chGetGorus = 34,
            chGetSayim = 35,
            
            chDelGorev = 43,
            chDelGorus = 44,
            chDelTahliye = 45,
            chDelLogData = 46,
            chDelUserFPLog = 47, // << chDelNewUser

            chIcmal = 50,
            chIcmalNewUser = 51,
            chIcmalOldUser = 52,
            chIcmalTahliye = 53,

            chNull = 100
        }




        /// 1.Privilage parametresi kullanıcı ayrıcalığını belirtir. 
        ///   0 : ortak kullanıcı, 
        ///   1 : kayıt memuru
        ///   2 : yönetici
        ///   3 : süper yöneticiyi belirtir. 
        ///   
        public enum tCihazPrivilage
        {
            prOrtakKulanici = 0,
            prKayitMemuru = 1,
            prYonetici = 2,
            prSuperYonetici = 3
        }

        public static DBTypes active_DB = new DBTypes();
        // Bilgisayar hakkında
        public static Comp tComp = new Comp();
        // User-Kullanıcı hakkında
        public static User tUser = new User();
        public static cComputer tComputer = new cComputer();
        // Exe hakkında
        public static exeAbout tExeAbout = new exeAbout();

        public static List<FirmAbout> tFirmUserList = new List<FirmAbout>();
        public static List<FirmAbout> tFirmFullList = new List<FirmAbout>();

        
        public static cihazHesap tCihazHesap = new cihazHesap();
        public static List<cihazHesap> tCihazHesapList = new List<cihazHesap>();
        public static IslemTarihi tIslemTarihi = new IslemTarihi();

        /// cihazlar olmadan genel işleyişin testi için
        /// true  : cihazlar sisteme bağlı  
        /// false : cihazlar sisteme bağlı değil
        /// 
        ///public static bool CihazIsActive = true;   
        public static bool CihazIsActive = false;
        public static bool manuelTalepRun = false;
        public static bool autoTalepRun = false;


        public static System.Windows.Forms.ProgressBar progressBar1 = new System.Windows.Forms.ProgressBar();
        public static System.Windows.Forms.Label mesajLabel1 = new System.Windows.Forms.Label();
        public static System.Windows.Forms.ListBox listBox1 = null;

        /* 
        
        
        // button hakkında bilgi
        public static vButtonHint tButtonHint = new vButtonHint();
        */
    }


    public class DBTypes
    {
        /// o an hangi db işlem yapılacaksa onu nosunu ver 
        /// gitti fonksiyonda ona göre işlem yapılır
        /// 
        ///
        public DBTypes()
        {
            managerDBaseNo = v.dBaseNo.Manager;
            projectDBaseNo = v.dBaseNo.Project;
            firmMainDBaseNo = v.dBaseNo.FirmMainDB;
            firmPeriodDBaseNo = v.dBaseNo.FirmPeriodDB;

            projectDBType = 0;
            projectServerName = "";
            projectDBName = "";
            projectUserName = "";
            projectPsw = "";
            projectConnectionText = "";

            firmMainDBaseNo = 0;
            firmMainDBType = 0;
            firmMainServerName = "";
            firmMainDBName = "";
            firmMainUserName = "";
            firmMainPsw = "";
            firmMainConnectionText = "";

            firmPeriodDBaseNo = 0;
            firmPeriodDBType = 0;
            firmPeriodServerName = "";
            firmPeriodDBName = "";
            firmPeriodUserName = "";
            firmPeriodPsw = "";
            firmPeriodConnectionText = "";

        }
        /// <summary>
        /// runDBaseNo ile o anda hangi database üzerinde çalışacağı 
        /// hakkında bilgi vermek/almak için kullanılıyor
        /// Örnek : t.tTableFind() 
        /// </summary>
        public v.dBaseNo runDBaseNo { get; set; }

        public v.dBaseNo managerDBaseNo { get; set; }
        public v.dBaseType managerDBType { get; set; }
        public string managerServerName { get; set; }
        public string managerDBName { get; set; }
        public string managerUserName { get; set; }
        public string managerPsw { get; set; }
        public string managerConnectionText { get; set; }
        public SqlConnection managerMSSQLConn { get; set; }

        public v.dBaseNo projectDBaseNo { get; set; }
        public v.dBaseType projectDBType { get; set; }
        public string projectServerName { get; set; }
        public string projectDBName { get; set; }
        public string projectUserName { get; set; }
        public string projectPsw { get; set; }
        public string projectConnectionText { get; set; }
        public SqlConnection projectMSSQLConn { get; set; }
        //public MySqlConnection projectMySQLConn { get; set; }

        public v.dBaseNo firmMainDBaseNo { get; set; }
        public v.dBaseType firmMainDBType { get; set; }
        public string firmMainServerName { get; set; }
        public string firmMainDBName { get; set; }
        public string firmMainUserName { get; set; }
        public string firmMainPsw { get; set; }
        public string firmMainConnectionText { get; set; }
        public SqlConnection firmMainMSSQLConn { get; set; }
        //public MySqlConnection firmMainMySQLConn { get; set; }

        public v.dBaseNo firmPeriodDBaseNo { get; set; }
        public v.dBaseType firmPeriodDBType { get; set; }
        public string firmPeriodServerName { get; set; }
        public string firmPeriodDBName { get; set; }
        public string firmPeriodUserName { get; set; }
        public string firmPeriodPsw { get; set; }
        public string firmPeriodConnectionText { get; set; }
        public SqlConnection firmPeriodMSSQLConn { get; set; }
        
    }

    public class vTable
    {
        public vTable()
        {
            //Clear()
        }

        //private byte DBaseNo_ = 0;
        private v.dBaseNo DBaseNo_;
        private v.dBaseType DBaseType_;
        private byte TableCount_ = 0;
        private byte TableType_ = 0;

        private string DBaseName_ = "";
        private string TableName_ = "";
        private string TableIPCode_ = "";
        private string TableCode_ = "";
        private string IPCode_ = "";
        private string Cargo_ = "";
        private string KeyId_FName_ = "";
        private string functionName_ = "";
        private string SoftwareCode_ = "";
        private string ProjectCode_ = "";
        private bool RunTime_ = false;

        private SqlConnection msSqlConnection_ = null;
        
        public v.dBaseNo DBaseNo
        {
            get { return DBaseNo_; }
            set { DBaseNo_ = value; }
        }
        public v.dBaseType DBaseType
        {
            get { return DBaseType_; }
            set { DBaseType_ = value; }
        }
        public string DBaseName
        {
            get { return DBaseName_; }
            set { DBaseName_ = value; }
        }
        public string TableName
        {
            get { return TableName_; }
            set { TableName_ = value; }
        }
        public string TableIPCode
        {
            get { return TableIPCode_; }
            set { TableIPCode_ = value; }
        }
        public string TableCode
        {
            get { return TableCode_; }
            set { TableCode_ = value; }
        }
        public string IPCode
        {
            get { return IPCode_; }
            set { IPCode_ = value; }
        }
        public string Cargo
        {
            get { return Cargo_; }
            set { Cargo_ = value; }
        }
        public byte TableCount
        {
            get { return TableCount_; }
            set { TableCount_ = value; }
        }
        public byte TableType
        {
            get { return TableType_; }
            set { TableType_ = value; }
        }
        public string KeyId_FName
        {
            get { return KeyId_FName_; }
            set { KeyId_FName_ = value; }
        }
        public string functionName
        {
            get { return functionName_; }
            set { functionName_ = value; }
        }
        public string SoftwareCode
        {
            get { return SoftwareCode_; }
            set { SoftwareCode_ = value; }
        }
        public string ProjectCode
        {
            get { return ProjectCode_; }
            set { ProjectCode_ = value; }
        }
        public bool RunTime
        {
            get { return RunTime_; }
            set { RunTime_ = value; }
        }
        public SqlConnection msSqlConnection
        {
            get { return msSqlConnection_; }
            set { msSqlConnection_ = value; }
        }
        

        public void Clear()
        {
            DBaseNo_ = 0;
            DBaseType_ = v.dBaseType.None;
            DBaseName_ = "";
            TableName_ = "";
            TableIPCode_ = "";
            TableCode_ = "";
            IPCode_ = "";
            KeyId_FName_ = "";
            functionName_ = "";
            Cargo_ = "";
            TableCount_ = 0;
            RunTime_ = false;
            msSqlConnection_ = null;
        }
    }


    public class cComputer
    {
        private string SystemName_ = "";
        private string Network_MACAddress_ = "";

        private string Processor_Name_ = "";
        private string Processor_Id_ = "";

        private string DiskDrive_Name_ = "";
        private string DiskDrive_Model_ = "";
        private string DiskDrive_SerialNumber_ = "";

        public string SystemName
        {
            get { return SystemName_; }
            set { SystemName_ = value; }
        }
        public string Network_MACAddress
        {
            get { return Network_MACAddress_; }
            set { Network_MACAddress_ = value; }
        }

        public string Processor_Name
        {
            get { return Processor_Name_; }
            set { Processor_Name_ = value; }
        }
        public string Processor_Id
        {
            get { return Processor_Id_; }
            set { Processor_Id_ = value; }
        }


        public string DiskDrive_Name
        {
            get { return DiskDrive_Name_; }
            set { DiskDrive_Name_ = value; }
        }
        public string DiskDrive_Model
        {
            get { return DiskDrive_Model_; }
            set { DiskDrive_Model_ = value; }
        }
        public string DiskDrive_SerialNumber
        {
            get { return DiskDrive_SerialNumber_; }
            set { DiskDrive_SerialNumber_ = value; }
        }


        ///Win32_Processor
        ///
        ///string SystemName
        ///string Name 
        ///string ProcessorId
        ///
        ///Win32_DiskDrive
        ///
        ///string   Name;
        ///string   Model;
        ///string   SerialNumber;

    }
    // computer bilgileri 
    public class Comp
    {
        public int SP_COMP_ID = 0;
        public int SP_COMP_ISACTIVE = 0;
        public string SP_COMP_FIRM_GUID { get; set; }
        public string SP_COMP_SYSTEM_NAME { get; set; }
        public string SP_COMP_MACADDRESS { get; set; }
        public string SP_COMP_PROCESSOR_ID { get; set; }
        public int SP_COMP_FIRM_ID { get; set; }
    }

    // user/kullanıcı bilgileri
    public class User
    {
        public int SP_USER_ID = 0;
        public int SP_USER_ISACTIVE = 0;
        public string SP_USER_FIRM_GUID { get; set; }
        public string SP_USER_GUID { get; set; }
        public string SP_USER_FULLNAME { get; set; }
        public string SP_USER_FIRSTNAME { get; set; }
        public string SP_USER_LASTNAME { get; set; }
        public string SP_USER_EMAIL { get; set; }
        public string SP_USER_KEY { get; set; }
    }

    // exe hakkındaki bilgiler
    public class exeAbout
    {
        // şu an çalışan exe
        public string activeVersionNo { get; set; }
        public string activeExeName { get; set; }
        public string activePath { get; set; }

        // ftp deki exe
        public string ftpVersionNo { get; set; }
        public string ftpExeName { get; set; }
        public string ftpPacketName { get; set; }

        // yeni hazırlanan exe
        public string newVersionNo { get; set; }
        public string newFileName { get; set; }
        public string newPacketName { get; set; }
        public string newPathFileName { get; set; }
    }
    
    public class vUserInputBox
    {
        private string title_ = string.Empty;
        private string promptText_ = string.Empty;
        private string value_ = string.Empty;
        private string displayFormat_ = string.Empty;
        private int fieldType_ = 0;

        public string title
        {
            get { return title_; }
            set { title_ = value; }
        }
        public string promptText
        {
            get { return promptText_; }
            set { promptText_ = value; }
        }
        public string value
        {
            get { return value_; }
            set { value_ = value; }
        }
        public string displayFormat
        {
            get { return displayFormat_; }
            set { displayFormat_ = value; }
        }
        public int fieldType
        {
            get { return fieldType_; }
            set { fieldType_ = value; }
        }

        public void Clear()
        {
            title_ = "";
            promptText_ = "";
            value_ = "";
            displayFormat_ = "";
            fieldType_ = 0;
        }
    }


    public class FirmUserList
    {
        public FirmUserList()
        {
            firmAbout = new List<FirmAbout>();
        }
        public List<FirmAbout> firmAbout { get; set; }
    }

    public class FirmFullList
    {
        public FirmFullList()
        {
            firmFullList = new List<FirmAbout>();
        }
        public List<FirmAbout> firmFullList { get; set; }
    }

    public class FirmAbout
    {
        public int FirmId { get; set; }
        public string FirmName { get; set; }
        public string FirmGuid { get; set; }
        public string usePackage { get; set; }

        /*
        [FIRM_DB_TYPE]
      ,[FIRM_DB_NAME]
      ,[FIRM_MAINDB_FORMAT]
      ,[FIRM_PERIODDB_FORMAT]
      ,[FIRM_SERVER_TYPE]
      ,[FIRM_SERVER_NAME]
      ,[FIRM_AUTHENTICATION]
      ,[FIRM_LOGIN]
      ,[FIRM_PASSWORD]
      */
        public string firmDBType { get; set; }
        public string firmDBName { get; set; }
        public string firmMainDBFormat { get; set; }
        public string firmPeriodDBFormat { get; set; }
        public string firmServerType { get; set; }
        public string firmServerName { get; set; }
        public string firmAuthentication { get; set; }
        public string firmLogin { get; set; }
        public string firmPassword { get; set; }

        public string firmMainConnText { get; set; }
        public string firmPeriodConnText { get; set; }

        public void Clear()
        {
            FirmId = 0;
            FirmName = "";
            FirmGuid = "";
            usePackage = "";

        }
    }



    //-----

    internal class fingerUserInfo
    {
        public int MachineNumber { get; set; }
        public string EnrollNumber { get; set; }
        public string Name { get; set; }
        public int FingerIndex { get; set; }
        public string TmpData { get; set; }
        public int Privelage { get; set; }
        public string Password { get; set; }
        public bool Enabled { get; set; }
        public string iFlag { get; set; }
        public string CardNumber { get; set; }
    }

    //MachineInfo
    public class fingerLogData
    {
        public int MachineNumber { get; set; }
        public int IndRegID { get; set; }
        public string DateTimeRecord { get; set; }

        public DateTime DateOnlyRecord
        {
            get { return DateTime.Parse(DateTime.Parse(DateTimeRecord).ToString("yyyy-MM-dd")); }
        }
        public DateTime TimeOnlyRecord
        {
            get { return DateTime.Parse(DateTime.Parse(DateTimeRecord).ToString("hh:mm:ss tt")); }
        }

    }

    public enum ClearFlag
    {
        UserData = 5,
        FingerPrintTemplate = 2
    }


    public class cihazHesapList
    {
        public cihazHesapList()
        {
            cihazHesap = new List<cihazHesap>();
        }
        public List<cihazHesap> cihazHesap { get; set; }
    }

    public class cihazHesap
    {
        public int Id { get; set; }
        public int CihazId { get; set; }
        public bool Connnect  { get; set; }
        public int Adet { get; set; }
        public string CihazCalismaTipi { get; set; }
        public string CihazAdi { get; set; }
        public string CihazIp { get; set; }
        public string CihazPort { get; set; }

        public fingerK50_SDKHelper Sdk { get; set; }

    }

    public class IslemTarihi
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public int Gun { get; set; }

    }

}