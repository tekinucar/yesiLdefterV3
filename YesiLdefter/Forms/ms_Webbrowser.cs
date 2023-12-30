using DevExpress.XtraEditors;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tkn_ToolBox;
using Tkn_Variable;
using Tkn_InputPanel;
using Tkn_Events;
using System.Threading;
using System.Net;
using System.IO;
using Tkn_CookieReader;
using YesiLdefter.Selenium;
using YesiLdefter.Entities;

namespace YesiLdefter
{
    public partial class ms_Webbrowser : Form
    {
        #region Tanımlar
        tToolBox t = new tToolBox();
        tInputPanel ip = new tInputPanel();
        tEventsButton evb = new tEventsButton();

        MsWebPagesService msPagesService = new MsWebPagesService();

        int nodeId = 0;
        int parentId = 0;
        int kayitBaslangisId = 0;
        
        //int columnId = 0;
        //ArrayList oldParent = new ArrayList();
        //List<int> oldParent = new List<int>();

        string TableIPCode = string.Empty;

        // sadece login için kullanılan dataset
        DataSet ds_LoginPageNodes = null;
        // veri transferlerinin (web <=> dbase) yapıldığı datasetler
        DataSet ds_ScrapingDbConnectionList = null;
        DataSet ds_WebNodeItemsList = null;

        // silinecek
        /*
        DataSet ds_PageNodes = null;
        DataSet ds_ScrapingTriggers = null;
        //DataSet ds_TriggerNodes = null;
        DataSet ds_DisplayNoneNodes = null;
        DataSet ds_LoadCount = null;
        */
        DataSet ds_DbaseTable = null;
        DataNavigator dN_DbaseTable = null;
        DataSet ds_DbaseSiraliTable = null;
        DataNavigator dN_DbaseSiraliTable = null;
        //---

        // Analysis işlemleri
        DataSet ds_AnalysisNodes = null;
        DataNavigator dN_AnalysisNodes = null;
        DataSet ds_AnalysisNodeItems = null;
        DataNavigator dN_AnalysisNodeItems = null;

        /// Scraping
        DataSet ds_MsWebPages = null;
        DataNavigator dN_MsWebPages = null;
        DataSet ds_MsWebNodes = null;
        DataNavigator dN_MsWebNodes = null;

        WebBrowser webAnalysis = null;
        WebBrowser webTest = null;
        WebBrowser webMain = null;


        //HtmlAgilityPack.HtmlNode htmlMainBody = null;

        //System.Windows.Forms.HtmlDocument htmlDocument = null;
        //System.IO.Stream htmlDocumentStream = null;
        //string htmlDocumentText = null;
        string htmlDocumentBody = null;
        HtmlElementCollection htmlTablePages = null;
        int selectedTablePageNo = 0;
        string tablePageName = "";
        string tablePageUrl = "";

        System.Windows.Forms.TableLayoutPanel tableLayoutPanel1 = null;
        System.Windows.Forms.TableLayoutPanel tableLayoutPanel2 = null;
        Control btn_AnalysisView = null;
        Control btn_AnalysisGetNodeItems = null;

        Control btn_PageView = null;
        Control btn_PageViewAnalysis = null;

        Control btn_LineGet = null;   // 1 satırı get  ediyor  analysis tarafında kullanılmakta
        Control btn_LinePost = null;  // 1 satırı post ediyor  analysis tarafında kullanılmakta
        Control btn_AlwaysSet = null; // AlwaysSet : TcNo sorgula gibi

        Control btn_FullGet1 = null;  // birinci get butonu
        Control btn_FullGet2 = null;  // ikinci  get butonu
        Control btn_FullPost1 = null; // birinci post butonu
        Control btn_FullPost2 = null; // ikinci  post butonu
        Control btn_FullSave = null;  // save    post butonu

        Control btn_WebScraping1 = null;
        Control btn_OneByOne = null;
        Control btn_Test = null;

        string menuName = "MENU_" + "UST/PMS/PMS/WEBANALYSIS";
        string buttonGIB = "buttonGIB";
        string buttonMEB = "buttonMEB";
        string buttonCONNECT = "buttonCONNECT";
        string buttonANALYSIS = "buttonANALYSIS";

        string buttonAnalysisGoBack = "buttonAnalysisGoBack";
        string buttonAnalysisGoForward = "buttonAnalysisGoForward";
        string buttonScrapingGoBack = "buttonScrapingGoBack";
        string buttonScrapingGoForward = "buttonScrapingGoForward";

        string aktifPageCode = "";
        string talepEdilenUrl = "";
        string talepEdilenUrl2 = "";
        string talepOncesiUrl = "";
        string errorPageUrl = "";
        string loginPageUrl = "";
        string aktifUrl = "";
        string sessionIdAndToken = "";


        //birbirini tetikleyen işler

        bool myWebTableRowToDatabase = false;
        bool myTriggerItemButton = false;

        Int16 myLoadNodeCount = 0;
        Int16 myDisplayNoneCount = 0;
        Int16 myPageRefreshCount = 0;
        Int16 myButton3Count = 0;
        Int16 myButton4Count = 0;
        Int16 myButton5Count = 0;
        Int16 myButton6Count = 0;
        Int16 myButton7Count = 0;
        Int16 myTabelFieldCount = 0;
        Int16 myNoneCount = 0;
        Int16 myNodeCount = 0; // tüm nodeler

        Int16 talepPageLeft = 0;
        Int16 talepPageTop = 0;

        // 1. grup trigger
        // 1. grup trigger listesi içinde table Trigger işleride olabiliyor
        //
        bool myDocumentCompleted = false;
        bool myTriggering = false;
        bool myTriggerInvoke = false;
        bool myTriggerOneByOne = false;
        bool myTriggerPageRefresh = false;
        bool myTriggerPageRefreshTick = false;
        int myTriggerPosition = 0;

        string myTriggerList = "";
        webNodeValue myTriggerWnv = null;
        //v.tWebEventsType myTriggerButtonEventsType = v.tWebEventsType.none;

        // 2. grup trigger
        // trigger table içinde ItemButton işleride olabiliyor
        //
        bool myTriggeringTable = false;
        bool myTriggeringItemButton = false;
        int myTriggerTableRowNo = 0;
        int myTriggerTableCount = 0;
        string myTriggerItemButonList = "";
        string myNokta = "";
        DataRow myTriggerTableRow = null;
        webNodeValue myTriggerTableWnv = null;
        tRow myTriggerWmvTableRows = null;

        v.tWebRequestType myTriggerWebRequestType = v.tWebRequestType.none;
        v.tWebEventsType myTriggerEventsType = v.tWebEventsType.none;

        v.tWebRequestType loadBeforeWebRequestType = v.tWebRequestType.none;
        v.tWebEventsType loadBeforeEventsType = v.tWebEventsType.none;

        List<MsWebPage> msWebPage = null;
        List<MsWebPage> msWebPages = null;
        List<MsWebNode> msWebNodes = null;
        List<MsWebScrapingDbFields> msWebScrapingFields = null;

        webForm f = new webForm();

        #endregion Tanımlar

        public ms_Webbrowser()
        {
            InitializeComponent();
        }

        #region Form preparing
        private void ms_Webbrowser_Shown(object sender, EventArgs e)
        {
            /// Analysis dataSet var mı? 
            /// varsa Analysis Formu açılmıştır
            /// 
            TableIPCode = "UST/PMS/MsWebNodes.Analysis_L01";
            t.Find_DataSet(this, ref ds_AnalysisNodes, ref dN_AnalysisNodes, TableIPCode);

            /// Bu dataset var ise Analysis formun ayarlarını hazırla
            /// 
            if (dN_AnalysisNodes != null)
                formAnalysisPreparing();
            else formScrapingPreparing();

            // scraping ilişkisi olan TableIPCode ve ilgili fieldler
            // 
            this.msWebScrapingFields = msPagesService.readScrapingTablesAndFields(this.msWebPages);
            
            // gerekli düzeltmeleri yapınca bunu sil gerek kalmayacak
            //
            ds_ScrapingDbConnectionList = msPagesService.readScrapingTablesAndFieldsOld(this.msWebPages);


            v.SQL = "";
            timerTrigger.Interval = 2000;
        }

        /// simpleButton_ek1 :  line get  / pageView
        /// simpleButton_ek2 :  line post / AlwaysSet Tc sorgula gibi
        /// simpleButton_ek3 :  full get1  : v.tWebEventsType.button3
        /// simpleButton_ek4 :  full get2  : v.tWebEventsType.button4
        /// simpleButton_ek5 :  full post1 : v.tWebEventsType.button5
        /// simpleButton_ek6 :  full post2 : v.tWebEventsType.button6
        /// simpleButton_ek7 :  full save  : v.tWebEventsType.button7
        /// aynı anda Ek3 ve Ek4 var ise button3 ve button4 devreye girecek ikisi aynı anda yoksa v.tWebEventsType.none çalışacak
        /// aynı anda Ek5 ve Ek6 var ise button5 ve button6 devreye girecek ikisi aynı anda yoksa v.tWebEventsType.none çalışacak

        #region Analysis Form
        private void formAnalysisPreparing()
        {
            preparingTabPage1Buttons();
            preparingTabPage2Buttons();
            preparingOrderControls();
        }
        private void preparingTabPage1Buttons()
        {
            //Control cntrl = null;
            string[] controls = new string[] { };

            #region tabPage1
            /// tabPage 1
            ///
            TableIPCode = "UST/PMS/MsWebNodes.Analysis_L01";

            /// ms_Webbrowser_Shown da tespit edildiği için tekrara gerek yok
            /// t.Find_DataSet(this, ref ds_AnalysisNodes, ref dN_AnalysisNodes, TableIPCode);

            if (dN_AnalysisNodes != null)
                dN_AnalysisNodes.PositionChanged += new System.EventHandler(dN_AnalysisNodes_PositionChanged);

            btn_AnalysisView = t.Find_Control(this, "checkButton_ek1", TableIPCode, controls);


            TableIPCode = "UST/PMS/MsWebNodeItems.Analysis_L01";

            btn_AnalysisGetNodeItems = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);

            if (btn_AnalysisGetNodeItems != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)btn_AnalysisGetNodeItems).Click += new System.EventHandler(myAnalysisGetNodeItemsClick);
            }

            #endregion tabPage1 

        }
        private void preparingTabPage2Buttons()
        {
            //Control cntrl = null;
            string[] controls = new string[] { };

            #region tabPage 2
            /// tabPage 2
            ///

            #region UST/PMS/MsWebPages.Analysis_L01
            
            TableIPCode = "UST/PMS/MsWebPages.Analysis_L01";
            
            t.Find_DataSet(this, ref ds_MsWebPages, ref dN_MsWebPages, TableIPCode);

            if (t.IsNotNull(ds_MsWebPages))
            {
                dN_MsWebPages.PositionChanged += new System.EventHandler(dNScrapingPages_PositionChanged);
                preparingMsWebPages();
                preparingMsWebNodesFields();
            }

            btn_PageView = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);
            if (btn_PageView != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)btn_PageView).Click += new System.EventHandler(myPageViewClick);
            }

            btn_PageViewAnalysis = t.Find_Control(this, "simpleButton_ek2", TableIPCode, controls);
            if (btn_PageViewAnalysis != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)btn_PageViewAnalysis).Click += new System.EventHandler(myPageViewAnalysisClick);
            }

            #endregion UST/PMS/MsWebPages.Analysis_L01
                        
            #region UST/PMS/MsWebNodes.Analysis_L02
            ///
            TableIPCode = "UST/PMS/MsWebNodes.Analysis_L02";
            t.Find_DataSet(this, ref ds_MsWebNodes, ref dN_MsWebNodes, TableIPCode);

            /// Get işlemleri
            btn_LineGet = t.Find_Control(this, "simpleButton_ek3", TableIPCode, controls);
            if (btn_LineGet != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)btn_LineGet).Click += new System.EventHandler(myLineGetTestClick);
            }
            btn_FullGet1 = t.Find_Control(this, "simpleButton_ek4", TableIPCode, controls);
            if (btn_FullGet1 != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)btn_FullGet1).Click += new System.EventHandler(myFullGet1_Click);
            }
            /// Post işlemleri
            btn_LinePost = t.Find_Control(this, "simpleButton_ek5", TableIPCode, controls);
            if (btn_LinePost != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)btn_LinePost).Click += new System.EventHandler(myLinePostTestClick);
            }
            btn_FullPost1 = t.Find_Control(this, "simpleButton_ek6", TableIPCode, controls);
            if (btn_FullPost1 != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)btn_FullPost1).Click += new System.EventHandler(myFullPost1_Click);
            }
            ///
            #endregion UST/PMS/MsWebNodes.Analysis_L02

            TableIPCode = "UST/PMS/MsWebNodeItems.Analysis_L01";
            t.Find_DataSet(this, ref ds_AnalysisNodeItems, ref dN_AnalysisNodeItems, TableIPCode);

            #endregion tabPage 2

        }
        private void preparingOrderControls()
        {
            Control cntrl = null;
            string[] controls = new string[] { };

            #region order control
            /// order control
            /// 
            cntrl = t.Find_Control(this, "tableLayoutPanel1");
            tableLayoutPanel1 = ((System.Windows.Forms.TableLayoutPanel)cntrl);
            cntrl = t.Find_Control(this, "tableLayoutPanel2");
            tableLayoutPanel2 = ((System.Windows.Forms.TableLayoutPanel)cntrl);

            webAnalysis = new WebBrowser();
            webTest = new WebBrowser();
            if (webMain == null)
            {
                webMain = new WebBrowser();
                webMain.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webMain_DocumentCompleted);
            }

            tableLayoutPanel1.Controls.Add(webAnalysis, 0, 0);
            tableLayoutPanel1.SetRowSpan(webAnalysis, 2);

            //tabPageHtmlView
            cntrl = t.Find_Control(this, "tabPageHtmlView");
            if (cntrl != null)
                cntrl.Controls.Add(webTest);

            tableLayoutPanel2.Controls.Add(webMain, 1, 0);
            tableLayoutPanel2.SetRowSpan(webMain, 2);

            webAnalysis.Dock = DockStyle.Fill;
            webTest.Dock = DockStyle.Fill;
            webMain.Dock = DockStyle.Fill;

            webAnalysis.ScriptErrorsSuppressed = true;
            webTest.ScriptErrorsSuppressed = true;
            webMain.ScriptErrorsSuppressed = true;

            webAnalysis.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webAnalysis_DocumentCompleted);

            menuName = "MENU_" + "UST/PMS/PMS/WEBANALYSIS";
            t.Find_Button_AddClick(this, menuName, buttonGIB, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonMEB, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonCONNECT, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonANALYSIS, myNavElementClick);

            t.Find_Button_AddClick(this, menuName, buttonAnalysisGoBack, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonAnalysisGoForward, myNavElementClick);

            #endregion order control
        }

        #endregion Analysis Form

        #region Scraping Form
        private void formScrapingPreparing()
        {
            MsWebPagesButtonsPreparing();
            MsWebNodesButtonsPreparing();

            webMain = (WebBrowser)t.Find_Control(this, "WebMain");
            if (webMain != null)
            {
                webMain.ScriptErrorsSuppressed = true;
                webMain.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webMain_DocumentCompleted);
            }
            else
            {
                MessageBox.Show("DİKKAT : Form tasarım sırasında WebBrowser için CmpName = WebMain tanımı bulunamadı...");
            }

            menuName = "MENU_" + "UST/PMS/PMS/WEBSCRAPING";
            t.Find_Button_AddClick(this, menuName, buttonScrapingGoBack, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonScrapingGoForward, myNavElementClick);
        }
        private void MsWebPagesButtonsPreparing()
        {
            /// MsWebPages tablosu
            /// 

            TableIPCode = t.Find_TableIPCode(this, "MsWebPages");

            if (t.IsNotNull(TableIPCode) == false) return;

            //Control cntrl = null;
            string[] controls = new string[] { };

            t.Find_DataSet(this, ref ds_MsWebPages, ref dN_MsWebPages, TableIPCode);

            if (t.IsNotNull(ds_MsWebPages))
            {
                dN_MsWebPages.PositionChanged += new System.EventHandler(dNScrapingPages_PositionChanged);
                preparingMsWebPages();
                preparingMsWebNodesFields();

                /// simpleButton_ek1 :  line get   / pageView
                /// simpleButton_ek2 :  line set   / AlwaysSet Tc sorgula gibi
                /// simpleButton_ek3 :  full get1  : v.tWebEventsType.button3
                /// simpleButton_ek4 :  full get2  : v.tWebEventsType.button4
                /// simpleButton_ek5 :  full set1  : v.tWebEventsType.button5
                /// simpleButton_ek6 :  full set2  : v.tWebEventsType.button6
                /// simpleButton_ek7 :  full save  : v.tWebEventsType.button7

                btn_PageView = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);
                // Page View
                if (btn_PageView != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_PageView).Click += new System.EventHandler(myPageViewClick);
                }

                // Bu hiç kullanılmadı 
                //
                // Bilgileri Sorgula / AlwaysSet  (TcNo sorgula gibi)
                btn_AlwaysSet = t.Find_Control(this, "simpleButton_ek2", TableIPCode, controls);
                if (btn_AlwaysSet != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_AlwaysSet).Click += new System.EventHandler(myAlwaysSetClick);
                }

                // Bilgileri Al button3 : get
                btn_FullGet1 = t.Find_Control(this, "simpleButton_ek3", TableIPCode, controls);
                if (btn_FullGet1 != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_FullGet1).Click += new System.EventHandler(myFullGet1_Click);
                }
                // Bilgileri Al button4 : get
                btn_FullGet2 = t.Find_Control(this, "simpleButton_ek4", TableIPCode, controls);
                if (btn_FullGet2 != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_FullGet2).Click += new System.EventHandler(myFullGet2_Click);
                }

                // Bilgileri Gönder button5 : set
                btn_FullPost1 = t.Find_Control(this, "simpleButton_ek5", TableIPCode, controls);
                if (btn_FullPost1 != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_FullPost1).Click += new System.EventHandler(myFullPost1_Click);
                }
                // Bilgileri Gönder button6 : set
                btn_FullPost2 = t.Find_Control(this, "simpleButton_ek6", TableIPCode, controls);
                if (btn_FullPost2 != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_FullPost2).Click += new System.EventHandler(myFullPost2_Click);
                }

                // Bilgileri Kaydet
                btn_FullSave = t.Find_Control(this, "simpleButton_ek7", TableIPCode, controls);
                if (btn_FullSave != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_FullSave).Click += new System.EventHandler(myFullSave_Click);
                }
            }

        }
        private void MsWebNodesButtonsPreparing()
        {
            /// MsWebNodes tablosu
            /// 
            TableIPCode = t.Find_TableIPCode(this, "MsWebNodes");

            if (t.IsNotNull(TableIPCode) == false)
            {
                MessageBox.Show("DİKKAT : MsWebNodes tablosu bulunamadı...(Form bilgisi olmayabilir... ( MS_LAYOUT )) ");
                return;
            }
            t.Find_DataSet(this, ref ds_MsWebNodes, ref dN_MsWebNodes, TableIPCode);

            // Hiç kullanma ihtiyacı olmadı 
            //
            /*
            //Control cntrl = null;
            string[] controls = new string[] { };

            if (t.IsNotNull(ds_ScrapingNodes))
            {
                btn_LineGet = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);
                if (btn_LineGet != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_LineGet).Click += new System.EventHandler(myLineGetTestClick);
                }

                btn_LinePost = t.Find_Control(this, "simpleButton_ek2", TableIPCode, controls);
                if (btn_LinePost != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_LinePost).Click += new System.EventHandler(myLinePostTestClick);
                }

                btn_FullGet = t.Find_Control(this, "simpleButton_ek3", TableIPCode, controls);
                if (btn_FullGet != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_FullGet).Click += new System.EventHandler(myFullGetTestClick);
                }

                btn_FullPost = t.Find_Control(this, "simpleButton_ek4", TableIPCode, controls);
                if (btn_FullPost != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_FullPost).Click += new System.EventHandler(myFullPostTestClick);
                }
            }
            */
        }

        #endregion Scraping Form

        private void dNScrapingPages_PositionChanged(object sender, EventArgs e)
        {
            eventsTypeCount(ds_MsWebNodes);

            preparingMsWebNodesFields();

            this.btn_OneByOne = null;
            this.myTriggerOneByOne = false;
            this.ds_DbaseSiraliTable = null;
            this.dN_DbaseSiraliTable = null;
        }

        private void preparingMsWebPages()
        {
            msWebPages = t.RunQueryModels<MsWebPage>(ds_MsWebPages);
        }
        private void preparingMsWebNodesFields()
        {
            msWebPage = t.RunQueryModelsSingle<MsWebPage>(ds_MsWebPages, dN_MsWebPages.Position);
            msWebNodes = t.RunQueryModels<MsWebNode>(ds_MsWebNodes);
        }
        #endregion Form preparing

        #region Analysis buttonları
        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonGIB) GIBPage();
            if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonMEB) MEBPage();
            if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonANALYSIS) Analysis();

            if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonAnalysisGoBack) GoBackAnalysis();
            if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonAnalysisGoForward) GoForwardAnalysis();

            if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonScrapingGoBack) GoBackMain();
            if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonScrapingGoForward) GoForwardMain();
        }
        private void GIBPage()
        {
            webAnalysis.Navigate("https://test.efatura.gov.tr/efatura/login.jsp");
        }
        private void MEBPage()
        {
            //webAnalysis.Navigate("https://mebbis.meb.gov.tr/default.aspx");
            webAnalysis.Navigate("https://mebbis.meb.gov.tr/KurumListesi.aspx");
        }
        private void GoBackAnalysis()
        {
            webAnalysis.GoBack();
        }
        private void GoForwardAnalysis()
        {
            webAnalysis.GoForward();
        }
        private void GoBackMain()
        {
            webMain.GoBack();
        }
        private void GoForwardMain()
        {
            webMain.GoForward();
        }

        #endregion Analysis buttonları

        #region Test Button
        private void myTestClick(object sender, EventArgs e)
        {
            Test();
        }
        private async Task<string> Test()
        {
            /*
                            // 1. deneme
                            WebClient wClient = new WebClient();
                            
                            wClient.Headers.Add($"Accept", $"* /*");
                            wClient.Headers.Add($"Referer", $"https://mebbis.meb.gov.tr/SKT/skt02001.aspx");
                            wClient.Headers.Add($"Accept-Language", $"tr-TR");
                            wClient.Headers.Add($"Accept-Encoding", $"gzip, deflate");
                            wClient.Headers.Add($"User-Agent", $"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.2; WOW64; Trident/7.0; .NET4.0C; .NET4.0E; .NET CLR 2.0.50727; .NET CLR 3.0.30729; .NET CLR 3.5.30729; InfoPath.3)");
                            wClient.Headers.Add($"Host", $"mebbis.meb.gov.tr");
                            //wClient.Headers.Add($"Connection", $"Keep-Alive");
                            wClient.Headers.Add($"Cookie", $"ASP.NET_SessionId=vo5wqgkyy4rfcudwgc1agade; __RequestVerificationToken=QFRLdG8NbJR4gkjqyARZ4feWs29RtbTZnuPDkBbdn0yHRqkAKH6Mlo9aawS5FvriU34oVh7ZMpSg7oQp-6_p3xyiSHtyJdjg4dLG4ImcEs01");

                            //var response = await wClient.DownloadData(urlDownload);
                            wClient.DownloadFile(urlDownload, "C:\\SqlData\\" + urlDownload.Substring(urlDownload.LastIndexOf('/')));
            */

            //string htmlContent = "";
            string url = @"";
            url = @"https://mebbis.meb.gov.tr/default.aspx";

            ///"txtKullaniciAd", "99969564"
            ///"txtSifre", "Ata.5514126")

            ///"txtKullaniciAd", "HATİCEKAYA20"
            ///"txtSifre", "246810")



            var baseAddress = new Uri(url);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                var content = new FormUrlEncodedContent(new[]
                {
        new KeyValuePair<string, string>("txtKullaniciAd", "99969564"),
        new KeyValuePair<string, string>("txtSifre", "Ata.5514126"),
                });
                cookieContainer.Add(baseAddress, new Cookie("CookieName", "madhav"));
                var result = client.PostAsync("", content).Result;
                result.EnsureSuccessStatusCode();
                var value = result.Headers.ToString();

                string sessionId = t.myGetValue(value, "ASP.NET_SessionId=", ";");
                string token = t.myGetValue(value, "__RequestVerificationToken=", ";");

            }

            return "";
        }

        #endregion Test Button

        #region Analysis
        private void Analysis()
        {
            if (string.IsNullOrEmpty(this.htmlDocumentBody)) return;

            /*
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(this.htmlDocumentBody);
            //HtmlAgilityPack.HtmlNode htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//body");
            HtmlAgilityPack.HtmlNode htmlBody = htmlDoc.DocumentNode;
            */

            HtmlAgilityPack.HtmlNode htmlBody = null;
            loadBody(this.htmlDocumentBody, ref htmlBody);

            this.nodeId = 1;
            parentId = 0;
            if (htmlBody != null)
                listNode_(htmlBody.ChildNodes);
        }
        private void loadBody(string htmlDocumentBody, ref HtmlAgilityPack.HtmlNode htmlBody)
        {
            /// htmlDocumentBody ile okunan sayfanın html yapısı string olarak geliyor
            /// burada herbir tag node nesnesine dönüşüyor
            /// 
            if (string.IsNullOrEmpty(this.htmlDocumentBody)) return;
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(this.htmlDocumentBody);
            htmlBody = htmlDoc.DocumentNode;

            // tüm page bilgisi alındıktan (head+body) sonra bu şekilde body e ulaşılmaya çalışıldığında 
            // input nodeler gelmiyor
            //
            //HtmlAgilityPack.HtmlNode htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//body");
        }
        private void listNode_(HtmlNodeCollection listNode)
        {
            foreach (HtmlNode nNode in listNode)
            {
                if (nNode.GetType().ToString() != "HtmlAgilityPack.HtmlTextNode")
                {
                    listAdd(this.nodeId, parentId, nNode);

                    if (nNode.ChildNodes != null)
                    {
                        parentId = -1;
                        listNode_(nNode.ChildNodes);
                    }
                }
            }
        }
        private void listAdd(int NodeId, int parentId, HtmlNode nNode)
        {
            if (nNode.Name == "br") return;

            DataRow row = ds_AnalysisNodes.Tables[0].NewRow();

            row["NodeId"] = NodeId;
            row["ParentId"] = parentId;
            row["IsActive"] = 0;
            row["TagName"] = nNode.Name;

            if (nNode.Attributes["id"] != null) row["AttId"] = nNode.Attributes["id"].Value;
            if (nNode.Attributes["name"] != null) row["AttName"] = nNode.Attributes["name"].Value;
            if (nNode.Attributes["class"] != null) row["AttClass"] = nNode.Attributes["class"].Value;
            if (nNode.Attributes["type"] != null) row["AttType"] = nNode.Attributes["type"].Value;
            if (nNode.Attributes["role"] != null) row["AttRole"] = nNode.Attributes["role"].Value;
            if (nNode.Attributes["href"] != null) row["AttHRef"] = nNode.Attributes["href"].Value;
            if (nNode.Attributes["src"] != null) row["AttSrc"] = nNode.Attributes["src"].Value;
            if (nNode.XPath != null) row["xPath"] = nNode.XPath;

            row["InnerHtml"] = nNode.InnerHtml;
            row["InnerText"] = nNode.InnerText;
            row["OuterHtml"] = nNode.OuterHtml;
            //row["OuterText"] = nNode.OuterText; burda outer text yokmuş

            ds_AnalysisNodes.Tables[0].Rows.Add(row);

            this.nodeId++;
        }

        #endregion Analysis

        #region analysis buttons

        private void myAnalysisGetNodeItemsClick(object sender, EventArgs e)
        {

            if (ds_AnalysisNodes != null)
            {
                webNodeValue wnv = new webNodeValue();
                wnv.workRequestType = v.tWebRequestType.get;

                DataRow row = ds_AnalysisNodes.Tables[0].Rows[dN_AnalysisNodes.Position];

                msPagesService.nodeValuesPreparing(row, ref wnv, this.aktifPageCode);

                WebScrapingAsync(webAnalysis, wnv); // myAnalysisGetNodeItemsClick
            }
        }

        private void myPageViewAnalysisClick(object sender, EventArgs e)
        {
            //myPageViewClickAsync(sender, e, webAnalysis);
            myPageViewClickAsync(webAnalysis);
        }
        
        private void myLineGetTestClick(object sender, EventArgs e)
        {
            if (ds_MsWebNodes != null)
            {
                webNodeValue wnv = new webNodeValue();
                wnv.workRequestType = v.tWebRequestType.get;

                DataRow row = ds_MsWebNodes.Tables[0].Rows[dN_MsWebNodes.Position];

                msPagesService.nodeValuesPreparing(row, ref wnv, this.aktifPageCode);

                // node nin items okunacak (getNodeItems)
                // sonrada MsWebNodeItems tablosuna yazılacak  
                if (wnv.TagName == "select")
                    wnv.workRequestType = v.tWebRequestType.getNodeItems;

                WebScrapingAsync(webMain, wnv); // myLineGetTestClick

                if (wnv.TagName == "select")
                {
                    /// select node ye ait (value, text) listesinide MsWebNodeItems tablosuna yaz
                    transferFromWebSelectToDatabase(wnv);
                    //MessageBox.Show("İşlem tamamlandı...");
                }
            }
        }

        private void myLinePostTestClick(object sender, EventArgs e)
        {
            if (ds_MsWebNodes != null)
            {
                webNodeValue wnv = new webNodeValue();
                wnv.workRequestType = v.tWebRequestType.post;

                DataRow row = ds_MsWebNodes.Tables[0].Rows[dN_MsWebNodes.Position];

                msPagesService.nodeValuesPreparing(row, ref wnv, aktifPageCode);

                WebScrapingAsync(webMain, wnv); // myLinePostTestClick
            }
        }

        #endregion analysis buttons

        /// 
        /// Userın kullanığı butonlar
        /// 
        #region user buttons
        private void myPageViewClick(object sender, EventArgs e)
        {
            // page ait nodelerin tespiti
            eventsTypeCount(ds_MsWebNodes);

            myPageViewClickAsync(webMain);
        }

        private void myAlwaysSetClick(object sender, EventArgs e)
        {
            v.SQL = "";

            Cursor.Current = Cursors.WaitCursor;
            startNodesBefore();
            startNodesRun(ds_MsWebNodes, v.tWebRequestType.alwaysSet, v.tWebEventsType.buttonAlwaysSet);
        }

        private void myFullGet1_Click(object sender, EventArgs e)
        {
            v.SQL = "";

            Cursor.Current = Cursors.WaitCursor;
            startNodesBefore();
            startNodesRun(ds_MsWebNodes, v.tWebRequestType.get, v.tWebEventsType.button3);
        }

        private void myFullGet2_Click(object sender, EventArgs e)
        {
            v.SQL = "";

            Cursor.Current = Cursors.WaitCursor;
            startNodesBefore();
            startNodesRun(ds_MsWebNodes, v.tWebRequestType.get, v.tWebEventsType.button4);
        }

        private void myFullPost1_Click(object sender, EventArgs e)
        {
            /* 
            HtmlElement htmlTable = webMain.Document.GetElementById(this.tablePageName);
            // bu kopya daha sonra selectTablePage için kulllanılıyor
            this.htmlTablePages = htmlTable.GetElementsByTagName("table");
            selectTablePage(webMain);
            */
            v.SQL = "";

            Cursor.Current = Cursors.WaitCursor;
            startNodesBefore();
            startNodesRun(ds_MsWebNodes, v.tWebRequestType.post, v.tWebEventsType.button5);
        }

        private void myFullPost2_Click(object sender, EventArgs e)
        {
            v.SQL = "";

            Cursor.Current = Cursors.WaitCursor;
            startNodesBefore();
            startNodesRun(ds_MsWebNodes, v.tWebRequestType.post, v.tWebEventsType.button6);
        }

        private void myFullSave_Click(object sender, EventArgs e)
        {
            v.SQL = "";

            Cursor.Current = Cursors.WaitCursor;
            startNodesBefore();
            startNodesRun(ds_MsWebNodes, v.tWebRequestType.post, v.tWebEventsType.button7);
        }
        
        #endregion user buttons
                

        #region startNodesRun

        /// Nodes çalıştırılmadan önceki hazırlık işlemleri
        /// 
        private void startNodesBefore()
        {
            timerTrigger.Interval = 2000;
            this.myTriggerInvoke = false; // sorun çıkarırsa startTriggersBeforeForButton() kullan ve butonlara ekle
            this.myDocumentCompleted = false;
            //this.myTriggerOneByOne = false;
            //this.ds_DbaseSiraliTable = null;
            //this.dN_DbaseSiraliTable = null;
            this.myTriggerPageRefresh = false;
            this.myTriggerPageRefreshTick = false;
            this.myTriggerEventsType = v.tWebEventsType.none;

            this.htmlTablePages = null;
            this.selectedTablePageNo = 0;
            
            v.con_Images = null;
            v.con_Images_FieldName = "";
            v.con_Images2 = null;
            v.con_Images_FieldName2 = "";
        }

        /// İşlem No : 1 : eventsType sayısını tespit et, bulunan sayı node sayısıdır
        /// 
        private void startNodesRun(DataSet ds, v.tWebRequestType webRequestType, v.tWebEventsType eventsType)
        {
            myNokta = ". ";
            v.SQL = v.SQL + v.ENTER2 + myNokta + "startNodesRun";

            //--- hazırlık 
            timerTrigger.Enabled = false;
            this.myTriggerList = "";
            this.myTriggerPosition = 1;
            this.myTriggerWebRequestType = webRequestType;
            this.myTriggerEventsType = eventsType;
            //emanet
            this.myTriggerTableRowNo = 0;

            if (this.myNodeCount > 0)
            {
                t.WaitFormOpen(v.mainForm, "");

                // 2. adıma gider
                this.myTriggering = true;
                runTriggerNodesAsync(ds); //startTriggers

                myNokta = ". ";
                v.SQL = v.SQL + v.ENTER2 + myNokta + "startNodesRun : END";

                if (countControl() == false)
                {
                    v.SQL = v.SQL + v.ENTER + myNokta + "startNodesRun : END 2 ";
                    // table ve itembutton beraber olunca sorun çıkardı : eğitim aracı  <<<<< myTableTriggering  diye yenisini ekle
                    this.myTriggering = false;

                    v.IsWaitOpen = false;
                    t.WaitFormClose();
                }
            }
        }

        /// İşlem No : 2 : nodeleri sırayla çalıştır, çalıştırdığın nodelerin listesini tut
        ///
        private async Task runTriggerNodesAsync(DataSet ds)
        {
            myNokta = ".. ";

            if (countControl() == false)
            {
                v.SQL = v.SQL + v.ENTER + myNokta + " runTriggerNodes : END ";

                // tetiklenecek başka node kalmadı
                this.myTriggering = false;

                if ((this.myDisplayNoneCount > 0) &&
                    (this.myDocumentCompleted)) //&& (this.myTriggerInvoke == false))
                {
                    /* btnListele invoke = click yüzünden kapattım. Tekrar gerekirse bir işaret koy */

                    v.SQL = v.SQL + v.ENTER + myNokta + " runTriggerNodesAsync() : this.myDisplayNoneCount > 0 : timerTrigger.Enabled =  true ";
                    timerTrigger.Interval = 500;
                    timerTrigger.Enabled = true;
                }

                Cursor.Current = Cursors.Default;

                t.AlertMessage("", "İşlem tamamlandı...");

                return;
            }

            bool isActive = false;
            bool onay = false;
            int pos = 0;
            int nodeId = 0;
            string nodeNo = "";
            v.tWebEventsType dbEventsType = v.tWebEventsType.none;

            // çalıştırılmak istenen nodeyi bul ve çalıştır
            foreach (DataRow eventRow in ds.Tables[0].Rows)
            {
                isActive = (bool)eventRow["IsActive"];
                nodeId = t.myInt32(eventRow["NodeId"].ToString());
                dbEventsType = (v.tWebEventsType)t.myInt16(eventRow["EventsType"].ToString());

                onay = false;
                onay = countControl();

                if (this.myTriggerEventsType == dbEventsType)
                    onay = true;

                if (isActive == false)
                    onay = false;

                if (onay)
                {
                    nodeNo = "|" + nodeId.ToString() + "|";

                    // nodeId ile bu satır çalıştı mı diye kontrol için sadece
                    pos = this.myTriggerList.IndexOf(nodeNo);

                    // -1 ise daha önce çalışmamış 
                    if (pos == -1)
                    {
                        v.SQL = v.SQL + v.ENTER2 + myNokta + "runTriggerNodes : nodeId = " + nodeId.ToString();
                        // 3. adıma gider
                        await runScrapingAsync(ds, this.myTriggerWebRequestType, this.myTriggerEventsType, nodeId, 0);

                        this.myTriggerPosition++;

                        this.myTriggerList = this.myTriggerList + nodeNo;

                        myNokta = ".. ";
                        v.SQL = v.SQL + v.ENTER + myNokta + "this.myTriggerList : " + this.myTriggerList;

                        Thread.Sleep(100);

                        if ((this.myTriggerPageRefresh) ||
                            (this.myTriggerInvoke))
                        {
                            if (this.myTriggerPageRefresh)
                                v.SQL = v.SQL + v.ENTER + myNokta + "pageRefresh : break";
                            if (this.myTriggerInvoke)
                                v.SQL = v.SQL + v.ENTER + myNokta + "invoke : break";
                            break;
                        }
                    }
                }
            }

            if (countControl() == false)
            {
                v.SQL = v.SQL + v.ENTER + myNokta + "runTriggerNodes : END 2 ";
                // table ve itembutton beraber olunca sorun çıkardı : eğitim aracı  <<<<< myTableTriggering  diye yenisini ekle
                this.myTriggering = false;

                v.IsWaitOpen = false;
                t.WaitFormClose();
            }

        }

        private bool countControl()
        {
            bool onay = true;

            Int16 _LoadNodeCount = 0;

            //if (this.myPageRefreshCount > 0)
            //    _LoadNodeCount = this.myLoadNodeCount;

            //if (this.myNodeCount + 1 <= this.myTriggerPosition)
            
            //if (//((this.myTriggerEventsType == v.tWebEventsType.load) && (this.myLoadNodeCount + 1 <= this.myTriggerPosition)) ||
            //    ((this.myTriggerEventsType == v.tWebEventsType.displayNone) && (this.myDisplayNoneCount + 1 <= this.myTriggerPosition)) ||
            //    ((this.myTriggerEventsType == v.tWebEventsType.none) && (_LoadNodeCount + this.myNoneCount + 1 <= this.myTriggerPosition)) ||
            //    ((this.myTriggerEventsType == v.tWebEventsType.button3) && (this.myButton3Count > 0) && (_LoadNodeCount + this.myButton3Count + 1 <= this.myTriggerPosition)) ||
            //    ((this.myTriggerEventsType == v.tWebEventsType.button4) && (this.myButton4Count > 0) && (_LoadNodeCount + this.myButton4Count + 1 <= this.myTriggerPosition)) ||
            //    ((this.myTriggerEventsType == v.tWebEventsType.button5) && (this.myButton5Count > 0) && (_LoadNodeCount + this.myButton5Count + 1 <= this.myTriggerPosition)) ||
            //    ((this.myTriggerEventsType == v.tWebEventsType.button6) && (this.myButton6Count > 0) && (_LoadNodeCount + this.myButton6Count + 1 <= this.myTriggerPosition)) ||
            //    ((this.myTriggerEventsType == v.tWebEventsType.button7) && (this.myButton7Count > 0) && (_LoadNodeCount + this.myButton7Count + 1 <= this.myTriggerPosition)) ||
            //    ((this.myTriggerEventsType == v.tWebEventsType.tableField) && (_LoadNodeCount + this.myTabelFieldCount + 1 <= this.myTriggerPosition)) ||
            //    (this.myNodeCount < this.myTriggerPosition)
            //    )
            //{
            //    onay = false;
            //}

            if (this.myNodeCount < this.myTriggerPosition)
            {
                onay = false;
            }

            return onay;
        }

        /// İşlem No : 3 : Çalıştırılmak istenen nodeyi db table üzerinde bul, ön kontrolleri yap, uygunsa webScraping e gönder
        /// 
        private async Task runScrapingAsync(DataSet ds, v.tWebRequestType workRequestType, v.tWebEventsType workEventsType, int workNodeId, int notWorkNodeId)
        {
            myNokta = "... ";
            v.SQL = v.SQL + v.ENTER + myNokta + "runScraping";

            if (ds != null)
            {
                bool isActive = false;
                int nodeId = 0;

                v.tWebEventsType eventsType = v.tWebEventsType.none;
                v.tWebInjectType injectType = v.tWebInjectType.none; /* none, set&get, set, get,  */

                webNodeValue wnv = new webNodeValue();

                //DataRow row = null;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    nodeId = t.myInt32(row["NodeId"].ToString());
                    isActive = (bool)row["IsActive"];
                    eventsType = (v.tWebEventsType)t.myInt16(row["EventsType"].ToString());
                    injectType = (v.tWebInjectType)t.myInt16(row["InjectType"].ToString());

                    // çalışması istenen bir type var ise ( workEventsType )
                    // okunan node uygun değilse çalışmasın
                    if ((workEventsType != eventsType) &&
                        (workNodeId == 0))
                        isActive = false;

                    // çalışması istenen bir NodeId var ise ( workNodeId )
                    // okunan node uygun değilse çalışmasın
                    if (workNodeId > 0)
                    {
                        if (nodeId != workNodeId)
                            isActive = false;
                        if (nodeId == workNodeId)// durdurmak için
                            isActive = true;
                    }

                    // bu nodeId çalışmasın 
                    // table verileri kaydeildirken save sırasında tekrar tekrar aynı table çalışmasın
                    if (notWorkNodeId > 0)
                    {
                        if (nodeId == notWorkNodeId)
                            isActive = false;
                    }

                    // workRequest isteklerinde post ise Set ve Get&Set çalışsın
                    if (nodeId == workNodeId)
                    {
                        if ((workEventsType != v.tWebEventsType.load) &&
                            (workEventsType != v.tWebEventsType.displayNone))
                        {
                            if (workRequestType == v.tWebRequestType.post &&
                                (injectType == v.tWebInjectType.Get ||
                                 injectType == v.tWebInjectType.none ||
                                (injectType == v.tWebInjectType.AlwaysSet && btn_AlwaysSet != null)
                                 ))
                            {
                                v.SQL = v.SQL + v.ENTER + myNokta + "runScraping : SET çalışcak. ( Get, AlwaysSet veya none )  node olduğu için çalışmaması gerekiyor. NodeId : " + nodeId.ToString();
                                isActive = false;
                                break;
                            }

                            // workRequest istekleri get ise Get ve Get&Set çalışsın
                            if (workRequestType == v.tWebRequestType.get &&
                                (injectType == v.tWebInjectType.Set ||
                                 injectType == v.tWebInjectType.none ||
                                (injectType == v.tWebInjectType.AlwaysSet && btn_AlwaysSet != null)
                                 ))
                            {
                                v.SQL = v.SQL + v.ENTER + myNokta + "runScraping : GET çalışacak. ( Set, AlwaysSet veya none )  node olduğu için çalışmaması gerekiyor. NodeId : " + nodeId.ToString();
                                isActive = false;
                                break;
                            }

                            // workRequest istekleri get ise Get ve Get&Set çalışsın
                            if ((workRequestType == v.tWebRequestType.alwaysSet) &&
                                (injectType != v.tWebInjectType.AlwaysSet))

                            {
                                v.SQL = v.SQL + v.ENTER + myNokta + "runScraping : AlwaysSet çalıcak. ( Get, Set veya none )  node olduğu için çalışmaması gerekiyor. NodeId : " + nodeId.ToString();
                                isActive = false;
                                break;
                            }
                        }
                    }

                    if (isActive)
                    {
                        // scraping için gerekli verileri hazırla
                        //
                        wnv.workRequestType = workRequestType;
                        wnv.workEventsType = workEventsType;

                        // 4. adıma gider
                        await WebScrapingBefore(row, wnv);

                        myNokta = "... ";

                        wnv.Clear();

                        // istenen NodeId çalıştı döngüden çık
                        //
                        if (workNodeId > 0)
                        {
                            if ((this.myTriggerPageRefresh) ||
                                (this.myTriggerInvoke))
                            {
                                if (this.myTriggerPageRefresh) v.SQL = v.SQL + v.ENTER + myNokta + "pageRefresh : break";
                                if (this.myTriggerInvoke) v.SQL = v.SQL + v.ENTER + myNokta + "invoke : break";
                                break;
                            }

                            // documentComplatedeki çalışmadıysa yani oraya gitmediyse
                            // çünkü get işlemlerinde documentComplate tetiklenmiyor
                            //
                            if ((timerTrigger.Enabled == false) &&
                                (this.myTriggeringItemButton == false))
                            {
                                //wnv.Clear();
                                //timerTrigger.Enabled = true;
                                break;
                            }
                        }
                    }

                }//foreach

            }//ds
        }

        #endregion startTriggers, runTrigger

        #region WebScraping

        /// İşlem No : 4 : PageRefresh ve webScraping işleminden önce 
        ///  set olacak data ise db den oku ve hazır et, (databaseden webe gönderilecek data)
        ///  get olacak data ise hangi tabloya atılacak ise onu bul ve yaz ( webden al, database yaz)

        private async Task WebScrapingBefore(DataRow row, webNodeValue wnv)
        {
            myNokta = ".... ";

            string pageRefresh_ = row["PageRefresh"].ToString();

            if (pageRefresh_ == "True")
            {
                pageRefresh(webMain, row);
                return;
            }

            // database tanımlı olan node bilgilerini al
            //
            msPagesService.nodeValuesPreparing(row, ref wnv, this.aktifPageCode);

            v.SQL = v.SQL + v.ENTER + myNokta + "WebScrapingBefore : TagName = " + wnv.TagName + " : " + wnv.AttId;

            // database deki veriyi web e aktar için 
            // db deki veriyi wnv.writeValue üzerine aktar

            if (wnv.InjectType == v.tWebInjectType.Set ||
                wnv.InjectType == v.tWebInjectType.AlwaysSet ||
               (wnv.InjectType == v.tWebInjectType.GetAndSet && wnv.workRequestType == v.tWebRequestType.post))
            {
                if (wnv.TagName != "table")
                    transferFromDatabaseToWeb(wnv);

                if (wnv.TagName == "table")
                {
                    /// webe transfer edilecek tablonun field bilgilerini databaseden al
                    if (wnv.tTable == null)
                    {
                        this.myTriggeringTable = false;
                        this.myTriggerTableWnv = null;
                        this.myTriggeringItemButton = false;
                        this.myTriggerTableCount = 0;
                        this.myTriggerTableRowNo = 0;

                        findRightDbTables(wnv);
                    }
                }

            }

            // scraping işlemini gerçekleştir
            // 5. adıma/son işleme gider
            await WebScrapingAsync(webMain, wnv); // WebScrapingBefore

            //  web deki veriyi database aktar
            //  webden alınan veriyi (readValue yi)  db ye aktar

            if ((wnv.InjectType == v.tWebInjectType.Get ||
                (wnv.InjectType == v.tWebInjectType.GetAndSet && wnv.workRequestType == v.tWebRequestType.get)) &&
                (this.myTriggerInvoke == false)) // invoke gerçekleşmişse hiç başlama : get sırasında set edip bilgi çağrılıyor demekki
            {
                if (wnv.TagName != "table")
                    transferFromWebToDatabase(wnv);

                if (wnv.TagName == "table")
                {
                    /// okunan tabloyu db ye yaz
                    if (wnv.tTable != null)
                    {
                        //https://mebbis.meb.gov.tr/SKT/skt02006.aspx
                        if (webMain.Url.ToString() == "https://mebbis.meb.gov.tr/SKT/skt02006.aspx")
                        {
                            vUserInputBox iBox = new vUserInputBox();
                            iBox.Clear();
                            iBox.title = "Kaydın başlamasını istediğiniz sıra no";
                            iBox.promptText = "Sıra No  :";
                            iBox.value = "0";
                            iBox.displayFormat = "";
                            iBox.fieldType = 0;

                            // ınput box ile sorulan ise kimden kopyalanacak (old) bilgisi
                            if (t.UserInpuBox(iBox) == DialogResult.OK)
                            {
                                this.kayitBaslangisId = int.Parse(iBox.value);
                            }
                        }
                        v.con_EditSaveCount = 0;
                        this.myTriggeringTable = true;
                        this.myTriggerTableWnv = null;
                        this.myTriggerTableWnv = wnv.Copy();

                        this.myTriggeringItemButton = false;
                        this.myTriggerTableCount = wnv.tTable.tRows.Count;
                        //this.myTriggerTableRowNo = 0;
                        transferFromWebTableToDatabase(this.myTriggerTableWnv);
                    }
                }

                if (wnv.TagName == "select")
                {
                    /// okunan tabloyu db ye yaz
                    if (wnv.tTable != null)
                    {
                        if (wnv.tTable.tRows.Count > 0)
                        {
                            /// select node ye ait (value, text) listesinide ilgili tabloya yaz
                            transferFromWebSelectToDatabase(wnv);
                            //MessageBox.Show("İşlem tamamlandı...");
                        }
                    }
                }
            }
        }

        private async Task pageRefresh(WebBrowser wb, DataRow row)
        {
            if (this.myTriggerPageRefresh == false)
            {
                this.myTriggerPageRefresh = true;
                this.myTriggerPageRefreshTick = true;

                v.SQL = v.SQL + v.ENTER + myNokta + " Page Refresh : start";

                await myPageViewClickAsync(webMain);
                
                v.SQL = v.SQL + v.ENTER + myNokta + " Page Refresh : END";

                t.ReadyComplate(2000);
            }
        }

        private async Task WebScrapingAsync(WebBrowser wb, webNodeValue wnv)
        {
            //, v.tWebRequestType request
            myNokta = "..... ";
            this.myTriggerInvoke = false;
            this.myDocumentCompleted = false;

            if (wb.Document == null) return;

            //HtmlNode nNode = null;
            string TagName = wnv.TagName.ToLower();

            v.SQL = v.SQL + v.ENTER + myNokta + "WebScraping : " + TagName;

            //string AttId = "";
            //string AttName = "";
            //string AttClass = "";
            string AttType = wnv.AttType;
            string AttRole = wnv.AttRole;
            string AttHRef = wnv.AttHRef;
            string AttSrc = wnv.AttSrc;
            string XPath = wnv.XPath;
            string InnerText = wnv.InnerText;
            string OuterText = wnv.OuterText;

            v.tWebInjectType injectType = wnv.InjectType;
            v.tWebInvokeMember invokeMember = wnv.InvokeMember;
            v.tWebRequestType workRequestType = wnv.workRequestType;
            v.tWebEventsType eventsType = wnv.EventsType;
            v.tWebEventsType workEventsType = wnv.workEventsType;

            string writeValue = "";
            if (!string.IsNullOrEmpty(wnv.writeValue))
                writeValue = wnv.writeValue;

            string inner = "";
            string idName = "";
            string readValue = "";
            HtmlElement element = null;

            if (wnv.AttId != "") idName = wnv.AttId;
            if ((wnv.AttId == "") && (wnv.AttName != "")) idName = wnv.AttName;

            if (wnv.EventsType == v.tWebEventsType.displayNone)
            {
                displayNone(TagName, idName, XPath, InnerText);
                return;
            }

            // şimdilik sadece itemButton var
            // aktif olan sayfadaki tüm uygun elementleri topla
            // burada sadece itemButton olan elementler toplanıyor 

            //if (eventsType == v.tWebEventsType.itemButton)
            if (AttRole == "ItemButton")
            {
                // table listesindeki satırın detayını açmak için kullanılıyor
                // 
                string src = "";
                HtmlElementCollection elements = wb.Document.GetElementsByTagName(TagName);
                foreach (HtmlElement item in elements)
                {
                    src = item.GetAttribute("src");

                    if (src.IndexOf(AttSrc) > -1)
                    {
                        // uygun olan element leri topla
                        wnv.elements.Add(item);
                    }
                }
            }

            if (TagName == "a")
            {
                if (!string.IsNullOrEmpty(AttHRef) && AttRole != "button")
                    AttRole = "Button";
            }

            if (TagName == "input")
            {
                if ((AttType == "submit") ||
                    (AttType == "file"))
                    invokeMember = v.tWebInvokeMember.click;
            }

            if (TagName == "button")
            {
                if (invokeMember == v.tWebInvokeMember.none)
                    invokeMember = v.tWebInvokeMember.click;
            }

            if (TagName == "script")
            {
                if (AttType == "text/javascript")
                {
                    HtmlElement node = wb.Document.GetElementById(idName);
                    var html = node.InnerHtml.ToString();
                    if (html.IndexOf("ASPx.createControl(ASPxClientListBox") > -1)
                    {
                        //html = html.Replace("<!--", "");
                        //html = html.Replace("//-->", "");
                        html = html.Remove(0, html.IndexOf("'itemsInfo"));
                        int pos = html.IndexOf("]") + 1;
                        html = html.Remove(pos, html.Length - pos);
                        html = "{" + html + "}";

                        //var prop_ = t.readProp<PROP_VIEWS_LAYOUT>(Prop_Views);
                        itemsInfoo itemsInfo_ = t.readProp<itemsInfoo>(html);

                        var x0 = itemsInfo_.itemsInfo[0].text;
                        var x1 = itemsInfo_.itemsInfo[1].text;
                        var x2 = itemsInfo_.itemsInfo[2].text;
                        var x3 = itemsInfo_.itemsInfo[3].text;

                    }
                    //readValue = wb.Document.GetElementById(idName).GetAttribute("itemsInfo");

                    var productsHtml = wb.Document.GetElementById(idName).Document.InvokeScript("cmbKurumTuru_DDD_L");
                    //InnerHtml.Where(x => HttpUtility.HtmlDecode(x.Attributes[":buyable"].Value)).ToList();
                    //DocumentNode.Descendants("ul")
                    //.Where(node => node.GetAttributeValue("id", "")
                    //.Equals("ListViewInner")).ToList();

                    /*
                    var buyableJsonList = node.Select(x => HttpUtility.HtmlDecode(x.Attributes[":buyable"].Value)).ToList();

                    var buyables = buyableJsons.Select(x => JsonConvert.DeserializeObject<Buyable>(x)).ToList();


                    var productsHtml = htmlDocument.DocumentNode.Descendants("ul")
                        .Where(node => node.GetAttributeValue("id", "")
                        .Equals("ListViewInner")).ToList();
                    */
                }
            }

            if (TagName == "select")
            {
                if (workRequestType == v.tWebRequestType.getNodeItems)
                    selectItemsRead(wb, ref wnv, idName);

                if ((workRequestType == v.tWebRequestType.get) &&
                    (AttRole == "ItemTable"))
                    selectItemsRead(wb, ref wnv, idName);

                // select in böyle bir özelliği yok ben manuel ekliyorum
                if (AttRole == "SetCaption")
                {
                    // elimizde select e ait text mevcut. bize bu textin valuesi gerekiyor. onuda nodenin kendi listesine bakarak buluyoruz
                    // text'i söyle sana value'sini vereyim

                    if ((injectType == v.tWebInjectType.Set) ||
                        (injectType == v.tWebInjectType.GetAndSet && workRequestType == v.tWebRequestType.post) ||
                        (injectType == v.tWebInjectType.AlwaysSet))
                        writeValue = selectItemsGetValue(wb, ref wnv, idName, writeValue);
                }
            }

            if (AttRole == "Button")
            {
                //nNode = this.htmlMainBody.SelectSingleNode(XPath);
                //nNode.? click yok diyorlar
                HtmlElementCollection elements = wb.Document.GetElementsByTagName(TagName);
                foreach (HtmlElement item in elements)
                {
                    if (item.InnerText == InnerText)
                    {
                        item.InvokeMember("click");
                        break;
                    }
                }
                return;
            }

            if ((injectType == v.tWebInjectType.Get ||
                (injectType == v.tWebInjectType.GetAndSet && workRequestType == v.tWebRequestType.get)) &&
                (TagName == "table"))
            {
                t.WebReadyComplate(wb);
                getHtmlTable(wb, ref wnv, idName);
                t.WebReadyComplate(wb);

                if (wnv.tTable == null) v.SQL = v.SQL + v.ENTER + myNokta + "WebScraping : table null";
                else
                {
                    v.SQL = v.SQL + v.ENTER + myNokta + "WebScraping : table Count = " + wnv.tTable.tRows.Count;
                }
            }


            if ((injectType == v.tWebInjectType.Set ||
                (injectType == v.tWebInjectType.GetAndSet && workRequestType == v.tWebRequestType.post)) &&
                (TagName == "table"))
            {
                t.WebReadyComplate(wb);
                postHtmlTable(wb, ref wnv, idName);
                t.WebReadyComplate(wb);

                if (wnv.tTable == null) v.SQL = v.SQL + v.ENTER + myNokta + "WebScraping : table null";
                else
                {
                    v.SQL = v.SQL + v.ENTER + myNokta + "WebScraping : table Count = " + wnv.tTable.tRows.Count;
                }
            }


            if ((injectType == v.tWebInjectType.Set ||
                (injectType == v.tWebInjectType.GetAndSet && workRequestType == v.tWebRequestType.post) ||
                // workRequestType = sadece alwaysSet isteği ve alwaysSet butonu var ise ( TC Sorgula gibi )
                (injectType == v.tWebInjectType.AlwaysSet && workRequestType == v.tWebRequestType.alwaysSet && btn_AlwaysSet != null) ||
                // workRequestType = Get veya Set isteği ve alwaysSet butonu yok ise 
                (injectType == v.tWebInjectType.AlwaysSet && workRequestType != v.tWebRequestType.alwaysSet && btn_AlwaysSet == null) ||
                // load sırasında 
                (injectType == v.tWebInjectType.AlwaysSet && workEventsType == v.tWebEventsType.load)
               ) &&
                (TagName == "input" || TagName == "select") &&
                (writeValue != "") &&
                (idName != ""))
            {
                //
                // Value atama işlemi 
                //
                #region
                try
                {
                    if ((AttType != "file") && (AttType != "checkbox"))
                    {
                        element = null;
                        element = wb.Document.GetElementById(idName);
                        if (element != null)
                        {
                            element.SetAttribute("value", writeValue);
                            //wb.Document.GetElementById(idName).SetAttribute("value", writeValue);
                            v.SQL = v.SQL + v.ENTER + myNokta + " set value : " + writeValue;

                            /* Bunu silme başka bir set işlemi için gerek olabilir
                             * tarihi set edemiyordum nedenin option value olması zannediyordum
                             * aslında gg/aa/yyyy  ile  gg.aa.yyyy farkından olduğunu fark ettim
                             * 
                            if (element.InnerHtml != null)
                            {
                                if (element.InnerHtml.IndexOf("option value=") > -1)
                                {
                                    / * combonun içi
                                     * 
	                                 <option selected="selected" value="-1"></option>
	                                 <option value="12020134-05/12/2021">05/12/2021</option>
	                                 <option value="12020134-04/12/2021">04/12/2021</option>                                    
                                    * /
                                    if (idName != "")
                                    {
                                        wb.Document.GetElementById(idName).SetAttribute("value", writeValue);
                                        v.SQL = v.SQL + v.ENTER + myNokta + " set value : " + writeValue;
                                    }
                                }
                            }
                            */
                        }

                        if (writeValue.ToLower().IndexOf("selectedindex=") > -1)
                        {
                            writeValue = writeValue.Replace("selectedindex=", "");
                            element.SetAttribute("selectedindex", writeValue);

                            v.SQL = v.SQL + v.ENTER + myNokta + " set selectedindex : " + writeValue;
                        }

                        t.WebReadyComplate(wb);
                    }
                    if ((AttType == "checkbox") || (AttType == "radio"))
                    {
                        if (writeValue == "True")
                        {
                            element = null;
                            element = wb.Document.GetElementById(idName);
                            if (element != null)
                            {
                                if (AttType == "checkbox")
                                    element.SetAttribute("checked", writeValue);
                                if (AttType == "radio")
                                    element.SetAttribute("value", "1");

                                v.SQL = v.SQL + v.ENTER + myNokta + " set checked : " + writeValue;
                            }
                        }

                        //if ((writeValue == "False") && (invokeMember > v.tWebInvokeMember.none))
                        if ((writeValue != "True") && (invokeMember > v.tWebInvokeMember.none))
                            invokeMember = v.tWebInvokeMember.none;
                    }
                    if (AttType == "file")
                    {
                        await Task.Delay(500);
                        SendKeys.Send(writeValue);// + "{ENTER}");

                        v.SQL = v.SQL + v.ENTER + myNokta + " set file (SendKeys.Send) : " + writeValue;
                    }
                }
                catch (Exception exc1)
                {
                    inner = (exc1.InnerException != null ? exc1.InnerException.ToString() : exc1.Message.ToString());

                    t.WebReadyComplate(wb);
                    MessageBox.Show("DİKKAT [error 1001] : [ " + idName + " (" + writeValue + ") ] veri ataması sırasında sorun oluştu ..." +
                        v.ENTER2 + inner);
                }
                #endregion
            }

            if ((injectType == v.tWebInjectType.Get ||
                (injectType == v.tWebInjectType.GetAndSet && workRequestType == v.tWebRequestType.get)) &&
                (TagName == "input" || TagName == "span" || TagName == "select" || TagName == "img") &&
                (idName != ""))
            {
                //
                // Value atama işlemi 
                //
                #region
                try
                {
                    if (TagName == "input" || TagName == "select")
                    {
                        element = null;
                        element = wb.Document.GetElementById(idName);
                        if (element != null)
                        {
                            readValue = element.GetAttribute("value");
                            //readValue = wb.Document.GetElementById(idName).GetAttribute("value");

                            // select in value si değilde text kısmı okunacak ise 
                            // writeValeu veya AttRole bizim tarafımızdan manuel işaretleniyor
                            if ((TagName == "select") &&
                                ((wnv.writeValue == "InnerText") ||
                                 (AttRole == "GetCaption") || (AttRole == "text") || (AttRole == "InnerText")
                                 ))
                            {
                                foreach (HtmlElement item in element.Children)
                                {
                                    if (readValue == item.GetAttribute("value"))
                                    {
                                        readValue = item.InnerText;
                                        v.SQL = v.SQL + v.ENTER + myNokta + " get select : " + readValue;
                                        break;
                                    }
                                }
                            }
                        }

                    }
                    if (TagName == "span")
                    {
                        element = null;
                        element = wb.Document.GetElementById(idName);
                        if (element != null)
                        {
                            readValue = element.InnerText;
                            v.SQL = v.SQL + v.ENTER + myNokta + " get span : " + readValue;
                            //readValue = wb.Document.GetElementById(idName).InnerText;
                        }
                    }
                    if (TagName == "img")
                    {
                        element = null;
                        element = wb.Document.GetElementById(idName);

                        if (element != null)
                        {
                            //örnek :
                            //string urlDownload = @"https://mebbis.meb.gov.tr/SKT/AResimGoster.aspx";
                            string urlDownload = element.GetAttribute("src");

                            if (this.sessionIdAndToken == "")
                                this.sessionIdAndToken = tCookieReader.GetCookie($"https://mebbis.meb.gov.tr/default.aspx");

                            readValue = msPagesService.ImageDownload(urlDownload, this.sessionIdAndToken, this.aktifUrl);

                            //v.SQL = v.SQL + v.ENTER + " get img : " + readValue;
                        }
                    }
                    if (AttType == "checkbox")
                    {
                        element = null;
                        element = wb.Document.GetElementById(idName);
                        if (element != null)
                        {
                            readValue = element.GetAttribute("checked");
                            v.SQL = v.SQL + v.ENTER + myNokta + " get checkbox : " + readValue;
                        }
                    }
                    t.WebReadyComplate(wb);
                }
                catch (Exception exc1)
                {
                    inner = (exc1.InnerException != null ? exc1.InnerException.ToString() : exc1.Message.ToString());

                    t.WebReadyComplate(wb);
                    MessageBox.Show("DİKKAT [error 1002] : [ " + idName + " ] veri okuması sırasında sorun oluştu ..." +
                        v.ENTER2 + inner);
                }
                #endregion

            }

            //
            // Atanan valueden sonra nesnenin tetiklenmesi gerekirse
            //
            if (invokeMember > v.tWebInvokeMember.none)
            {
                if (injectType == v.tWebInjectType.Set ||
                    injectType == v.tWebInjectType.AlwaysSet ||
                   (injectType == v.tWebInjectType.GetAndSet && workRequestType == v.tWebRequestType.post))
                {

                    string invoke = "";

                    // this.myTriggerInvoke 
                    // invoke çalışınca her zaman webbrowserDocumentCompleted  tetiklenmiyor
                    // bu event çalıştımı çalışmadı mı diye 
                    // this.myDocumentCompleted kullanılıyor
                    //

                    this.myDocumentCompleted = false;
                    this.myTriggerInvoke = true;
                    timerTrigger.Enabled = false;

                    if (invokeMember == v.tWebInvokeMember.click) invoke = "click";
                    if (invokeMember == v.tWebInvokeMember.submit) invoke = "submit";
                    if ((invokeMember == v.tWebInvokeMember.onchange) &&
                        (writeValue != ""))
                        invoke = "onchange";
                    if ((invokeMember == v.tWebInvokeMember.onchangeDontDocComplate) &&
                        (writeValue != ""))
                        invoke = "onchange";

                    // bazı nodeler onchange olduğu halde webBrowserDocumentComplate gerçekleşmiyor, yinede olmuş gibi davran
                    //if (invokeMember == v.tWebInvokeMember.onchangeDontDocComplate)
                    if (invokeMember != v.tWebInvokeMember.onchange)
                    {
                        // this.myDocumentCompleted = true;

                        // burayı açacağın zaman aday dönem kayıt sırasında ucretli seçilince çalışmıyor 
                        // aslıdan timer2 de invokemember break yapamıyor

                    }

                    v.SQL = v.SQL + myNokta + " : invoke = " + invoke;

                    try
                    {
                        if ((idName != "") && (invoke != ""))
                        {
                            t.WebReadyComplate(wb);

                            element = null;
                            element = wb.Document.GetElementById(idName);
                            if (element != null)
                                wb.Document.GetElementById(idName).InvokeMember(invoke);

                            Thread.Sleep(1000);
                            Application.DoEvents();

                            t.WebReadyComplate(wb);

                            //if ((this.myDisplayNoneCount > 0) &&
                            //    (this.myTriggering == false) && (invoke == "click"))
                            //{
                            //    Application.DoEvents();
                            //    timerTrigger.Interval = 100;
                            //    this.timerTrigger.Enabled = true;
                            //    v.SQL = v.SQL + v.ENTER + myNokta + " invokeMember : myDisplayNoneCount > 0 :timerTrigger.Enabled = true";
                            //}
                        }
                    }
                    catch (Exception exc2)
                    {
                        inner = (exc2.InnerException != null ? exc2.InnerException.ToString() : exc2.Message.ToString());

                        t.WebReadyComplate(wb);

                        MessageBox.Show("DİKKAT [error 1002] : [ " + idName + " (" + writeValue + "), (" + invoke + ") ] verinin çalıştırılması sırasında sorun oluştu ..." +
                            v.ENTER2 + inner);
                    }

                }
            }

            wnv.readValue = readValue;
        }

        #endregion WebScraping

        #region timerTrigger
        private void timerTrigger_Tick(object sender, EventArgs e)
        {
            timerTrigger.Enabled = false;

            timerTriggerAsync();
        }
        private async Task timerTriggerAsync()
        {
            if ((this.myTriggeringItemButton) &&
                (this.myTriggeringTable) &&
                (this.myTriggerWmvTableRows != null) &&
                (this.myTriggerPageRefreshTick))
            {
                // burası pageRefresh olunca devreye giriyor
                // yani alınacak bilgilerin listesi -x- sayfasında detay bilgisi -y- sayfasında ise 
                // örnek : personel listesi ve detayı sayfası
                //
                v.SQL = v.SQL + v.ENTER2 + myNokta + " timerTrigger_Tick 1.2 start : ( TriggeringTable )";

                if (this.myTriggerTableCount >= this.myTriggerTableRowNo)
                {
                    this.myTriggerPageRefreshTick = false;

                    // bir sonraki satır için tekrar yeniden 
                    transferFromWebTableToDatabase(this.myTriggerTableWnv);
                }

                v.SQL = v.SQL + v.ENTER + myNokta + " timerTrigger_Tick 1.2 end : ( TriggeringTable )";
                return;
            }

            if ((this.myTriggeringItemButton) &&
                (this.myTriggeringTable) &&
                (this.myTriggerWmvTableRows != null) &&
                (this.myTriggerPageRefreshTick == false))
            {
                // burasıda; sayfada detay bilgileri var ve her column bilgisini tek tek okumayı gerçekleştiriyor
                // burası 2 şekilde çalışıyor
                // birincisi : liste bilgisi ve detay bilgisi aynı sayfada ise detayları okuyor : örnek : eğitim araçları sayfası 
                // ikincisi  : liste -x- syafasında detay -y- sayfasında ise detayları okuyor   : örnek : personel listesi ve detayı sayfası
                //
                v.SQL = v.SQL + v.ENTER2 + myNokta + " timerTrigger_Tick 1.1 start : ( TriggeringTable )";

                this.myTriggerPageRefresh = false;

                transferFromWebTableRowToDatabase();

                v.SQL = v.SQL + v.ENTER + myNokta + " timerTrigger_Tick 1.1 end : ( TriggeringTable )";
                return;
            }

            if ((this.myTriggering) &&
                (this.myTriggeringTable == false) &&
                (this.myTriggerPageRefreshTick == false))
            {
                v.SQL = v.SQL + v.ENTER2 + myNokta + " timerTrigger_Tick 2 start : ( Triggering )";

                // döngü yeniden başlayacağı için mevcut invoke iptal olsun
                this.myTriggerInvoke = false;

                while (this.myTriggering)
                {
                    //await runTriggerNodesAsync(ds_TriggerNodes); //timerTriggerAsync() timerTrigger_Tick 2
                    await runTriggerNodesAsync(ds_MsWebNodes); //timerTriggerAsync() timerTrigger_Tick 2

                    // invoke çalıştığına göre DocumentComplate ninde çalışması gerekiyor 
                    // onun için buradan çıkmak gerekiyor
                    // orada timer tekrar aktif ediliyor
                    //
                    /*
                    if ((this.myTriggerInvoke) &&
                        (this.myDocumentCompleted))
                    {
                        this.myTriggerInvoke = false;
                        //break;
                    }
                    */
                    if (this.myTriggerInvoke)
                    {
                        v.SQL = v.SQL + v.ENTER + myNokta + " invoke : break : timerTrigger_Tick 2 ";
                        break;
                    }

                }

                if (countControl() == false)
                {
                    this.myTriggering = false;
                    this.myDocumentCompleted = true;
                }

                v.SQL = v.SQL + v.ENTER + myNokta + " timerTrigger_Tick 2 end : ( Triggering )";

                /*
                if (this.myTriggering)
                {
                    v.IsWaitOpen = false;
                    t.WaitFormClose();

                    return;
                }
                */
            }

            if ((this.myTriggering) &&
                (this.myTriggeringTable) &&
                (this.myTriggerPageRefreshTick == false))
            {

                if (this.myTriggerTableWnv.tTable == null)
                {
                    v.SQL = v.SQL + v.ENTER2 + myNokta + " timerTrigger_Tick 3 start : ( TriggerTableWnv )";

                    await WebScrapingAsync(webMain, this.myTriggerTableWnv); // timerTriggerAsync

                    v.SQL = v.SQL + v.ENTER + myNokta + " timerTrigger_Tick 3 end : ( TriggerTableWnv ) ";
                }

            }

            if ((this.myTriggering) &&
                (this.myTriggerPageRefresh) &&
                (this.myTriggerPageRefreshTick))
            {
                v.SQL = v.SQL + v.ENTER2 + myNokta + " timerTrigger_Tick 4 start : ( PageRefresh )";

                this.myTriggerPageRefresh = false;
                this.myTriggerPageRefreshTick = false;
                // döngü yeniden başlayacağı için mevcut invoke iptal olsun
                this.myTriggerInvoke = false;

                Thread.Sleep(100);

                while (this.myTriggering)
                {
                    //await runTriggerNodesAsync(ds_TriggerNodes); // timerTriggerAsync() timerTrigger_Tick 4
                    await runTriggerNodesAsync(ds_MsWebNodes); // timerTriggerAsync() timerTrigger_Tick 4

                    // invoke çalıştığına göre DocumentComplate ninde çalışması gerekiyor 
                    // onun için buradan çıkmak gerekiyor
                    // orada timer tekrar aktif ediliyor
                    //
                    //if ((this.myTriggerInvoke) &&
                    //    (this.myDocumentCompleted))
                    //{
                    //    //this.myTriggerInvoke = false;
                    //    break;
                    //}
                    if (this.myTriggerInvoke)
                    {
                        v.SQL = v.SQL + v.ENTER + myNokta + " invoke : break : timerTrigger_Tick 4 ";
                        break;
                    }
                }

                v.SQL = v.SQL + v.ENTER + myNokta + " timerTrigger_Tick 4 end : ( PageRefresh )";

                if ((this.loadBeforeWebRequestType != v.tWebRequestType.none) ||
                    (this.loadBeforeEventsType != v.tWebEventsType.none))
                {
                    this.myTriggerWebRequestType = this.loadBeforeWebRequestType;
                    this.myTriggerEventsType = this.loadBeforeEventsType;
                }

                if (countControl() == false)
                {
                    this.myTriggering = false;
                    this.myDocumentCompleted = true;
                }
                else
                {
                    if (this.myTriggerWebRequestType != v.tWebRequestType.none)
                        this.myTriggering = true;
                }

                if (this.myTriggering)
                {
                    v.IsWaitOpen = false;
                    t.WaitFormClose();
                    return;
                }
            }

            if ((this.myTriggerOneByOne) &&
                (this.myTriggering == false))
            {
                if (((DevExpress.XtraEditors.CheckButton)btn_OneByOne).Checked)
                {
                    v.SQL = v.SQL + v.ENTER2 + myNokta + " timerTrigger_Tick 5 start ( TriggerOneByOne )";
                    t.ReadyComplate(5000);

                    if (btnOneByOneNextRecord() == false)
                    {
                        v.SQL = v.SQL + v.ENTER + myNokta + " timerTrigger_Tick 5 end. Data END ( TriggerOneByOne ) ";
                        return;
                    }
                    // sakla
                    v.tWebRequestType webRequestType_ = this.myTriggerWebRequestType;
                    v.tWebEventsType eventsType_ = this.myTriggerEventsType;

                    startNodesBefore();

                    // hatırla
                    this.myTriggerWebRequestType = webRequestType_;
                    this.myTriggerEventsType = eventsType_;

                    startNodesRun(ds_MsWebNodes, this.myTriggerWebRequestType, this.myTriggerEventsType);

                    v.SQL = v.SQL + v.ENTER + myNokta + " timerTrigger_Tick 5 end : ( TriggerOneByOne )";
                }
            }

            if ((this.myDisplayNoneCount > 0) &&
                (this.myTriggering == false) &&
                (this.myDocumentCompleted))
            {
                v.SQL = v.SQL + v.ENTER2 + myNokta + " timerTrigger_Tick 6 start : ( DisplayNone )";
                await runDisplayNoneAsync();
                timerTrigger.Interval = 2000;
                v.SQL = v.SQL + v.ENTER + myNokta + " timerTrigger_Tick 6 end : ( DisplayNone )";
            }

            v.IsWaitOpen = false;
            t.WaitFormClose();

        }

        #endregion timerTrigger

        #region transferFrom... , saveRow

        private void transferFromWebToDatabase(webNodeValue wnv)
        {
            v.SQL = v.SQL + v.ENTER + myNokta + " transferFromWebToDatabase : Get : " + wnv.dbFieldName;

            /// web deki veriyi database aktar
            ///
            if (t.IsNotNull(ds_ScrapingDbConnectionList) == false) return;

            wnv.dbFieldName = "";
            wnv.dbLookUpField = false;

            //durdurmak için 
            /*
            if (wnv.nodeId == 1293)
                wnv.dbFieldName = "";
            if (wnv.nodeId == 1294)
                wnv.dbFieldName = "";
            */

            DataRow dbRow = findRightRow(wnv, v.tSelect.Get);

            // webden okunan veriyi db ye aktardığı an
            if (dbRow != null)
            {
                //t.tCheckedValue(dbRow, wnv.dbFieldName, wnv.readValue);

                if (wnv.dbFieldName.IndexOf("Resim") == -1)
                {
                    v.SQL = v.SQL + wnv.readValue;
                    //if (t.IsNotNull(wnv.readValue))
                    //    dbRow[wnv.dbFieldName] = wnv.readValue;

                    if (t.IsNotNull(wnv.readValue))
                    {
                        string itemText = wnv.readValue;

                        if (wnv.dbLookUpField == false)
                        {
                            if (wnv.dbFieldType == 108)
                            {
                                itemText = wnv.readValue.Replace(" TL", "");
                                itemText = wnv.readValue.Replace("TL", "");
                            }
                            if (itemText != "")
                                dbRow[wnv.dbFieldName] = itemText;
                        }
                        else
                        {
                            string itemValue = findNodeItemsValue(wnv, itemText);
                            if (itemValue != "")
                                dbRow[wnv.dbFieldName] = itemValue;
                        }

                        dbRow.AcceptChanges();
                    }
                }
                else
                {
                    v.SQL = v.SQL + wnv.dbFieldName;

                    if (t.IsNotNull(v.con_Images_FieldName) == false)
                         v.con_Images_FieldName  = wnv.dbFieldName;
                    else v.con_Images_FieldName2 = wnv.dbFieldName;

                    //v.EXE_TempPath+"\\AResimGoster.aspx"
                    string fileName = wnv.readValue;

                    //byte[] theBytes = Encoding.UTF8.GetBytes(wnv.readValue);
                    long imageLength = 0;
                    if (v.con_Images == null)
                         v.con_Images  = t.imageBinaryArrayConverter(fileName, ref imageLength);
                    else v.con_Images2 = t.imageBinaryArrayConverter(fileName, ref imageLength);

                    if (t.IsNotNull(v.con_Images_FieldName))
                    {
                        if (v.con_Images_FieldName.IndexOf("Small") > -1)
                            v.con_Images = msPagesService.ResmiKucult(v.con_Images);
                    }

                    if (t.IsNotNull(v.con_Images_FieldName2))
                    {
                        if (v.con_Images_FieldName2.IndexOf("Small") > -1)
                            v.con_Images2 = msPagesService.ResmiKucult(v.con_Images2);
                    }
                    //if (t.IsNotNull(wnv.readValue))
                    //    dbRow[wnv.dbFieldName] = theBytes; // Encoding.UTF8.GetBytes(wnv.readValue);
                    if ((v.con_Images != null) && (v.con_Images2 == null)) 
                        dbRow[wnv.dbFieldName] = v.con_Images;
                    if ((v.con_Images != null) && (v.con_Images2 != null))
                        dbRow[wnv.dbFieldName] = v.con_Images2;
                }
            }

            // get işlemi bitti ve artık database yazma işlemi gerekiyor
            if (wnv.GetSave)
            {
                if (t.IsNotNull(ds_DbaseTable))
                   msPagesService.dbButtonClick(this, ds_DbaseTable.DataSetName, v.tButtonType.btKaydet);
            }

        }

        
        
        private void transferFromDatabaseToWeb(webNodeValue wnv)
        {
            v.SQL = v.SQL + v.ENTER + myNokta + " transferFromDatabaseToWeb : Set : ";

            /// database deki veriyi web e aktar
            ///
            if (t.IsNotNull(ds_ScrapingDbConnectionList) == false) return;

            wnv.dbFieldName = "";
            wnv.dbLookUpField = false;

            DataRow dbRow = findRightRow(wnv, v.tSelect.Set);

            // db den okuyarak web göndereceği veri db den aldığı an
            if (dbRow != null)
            {
                //if (wnv.dbLookUpField == false)
                Int16 ftype = wnv.dbFieldType;

                // date veya smalldate
                if ((ftype == 40) || (ftype == 58) || (ftype == 61))
                {
                    string value = dbRow[wnv.dbFieldName].ToString();
                    if (value != "")
                        wnv.writeValue = Convert.ToDateTime(value).ToString("dd.MM.yyyy"); //.Substring(0,10);

                }
                else if (ftype == 108)
                {
                    string value = dbRow[wnv.dbFieldName].ToString();
                    if (value != "")
                    {
                        //value = value.Substring(0, value.IndexOf(",") + 3);
                        wnv.writeValue = string.Format("{0:0.00}", Convert.ToDecimal(value));// Convert.ToString(Convert.ToDouble(value).ToString("D"));
                    }
                }
                else
                {
                    wnv.writeValue = dbRow[wnv.dbFieldName].ToString();
                }

                v.SQL = v.SQL + wnv.dbFieldName + " ; " + wnv.writeValue;
            }
        }
        
        private void transferFromWebTableToDatabase(webNodeValue wnv)
        {
            v.SQL = v.SQL + v.ENTER + myNokta + " transferFromWebTableToDatabase";

            Application.DoEvents();

            if (this.myTriggerTableCount <= this.myTriggerTableRowNo)
            {
                // bitti
                // tetiklenecek başka row kalmadı
                this.myTriggering = false;
                this.myTriggeringTable = false;
                return;
            }

            if ((this.myTriggerPageRefresh) &&
                (this.myTriggerPageRefreshTick))
                return;

            if (wnv.tTable == null)
                return;

            tTable tb = wnv.tTable;

            this.myTriggerWmvTableRows = null;
            this.myTriggerWmvTableRows = tb.tRows[this.myTriggerTableRowNo];

            // Tablo üzerinde bulunduğu satırdaki bilgilerin detayını göstermek için buton var mı ?
            // varsa uygula
            this.myTriggerItemButton = findTableItemButton(this.myTriggerWmvTableRows, this.myTriggerTableRowNo);//, ref wnv);

            // itemButton false ise buradan çağrılıyor 
            if ((this.myTriggerItemButton == false) && (this.myTriggerTableRowNo == 0))
            {
                // itemButton olmadığı için table nin bütün row larını bir defada buradan kayıt işlemini gerçekleştir
                //
                int value = 0;
                foreach (tRow row in tb.tRows)
                {
                    // geçici çözüm burayı sil
                    value = t.myInt32(row.tColumns[0].value.ToString());

                    // kayitBaslangisId : bazı table data listesi çok büyük olabiliyor (sertifika alanların listesi gibi)
                    // kayitBaslangisId ile hangi kayıttan başlayabileceğine karar verebiliriz
                    // çünkü bazen liste daha bitmemişken herhangi bir sebeple işlem durmuş olabiliyor
                    // önceki kayıt edilmiş olanlar tekrar kayıt işlemiyle uğraşmasın diye istenen yerden kaydın başlanması 
                    // sağlanabiliyor

                    if (value >= kayitBaslangisId)
                    {
                        saveRowAsync(this.myTriggerTableWnv, row);

                        if (msPagesService.editKayitKontrolu()) break;
                    }

                    
                }
                this.myTriggeringTable = false;
            }

            if (this.myTriggerItemButton)
            {
                // tetikleme işi timerTriggerAsync() içinde 1.1
                //
                // transferFromWebTableRowToDatabase();
            }

        }


        private async Task transferFromWebTableRowToDatabase()
        {
            v.SQL = v.SQL + v.ENTER + myNokta + " transferFromWebTableRowToDatabase";

            this.myWebTableRowToDatabase = true;

            await saveRowAsync(this.myTriggerTableWnv, this.myTriggerWmvTableRows);

            //if (editKayitKontrolu()) return;

            this.myWebTableRowToDatabase = false;

            this.myTriggerTableRowNo++;

            if (this.myTriggerTableCount >= this.myTriggerTableRowNo)
            {
                if (this.myTriggerPageRefresh == false)
                {
                    // bir sonraki satır için tekrar yeniden 
                    transferFromWebTableToDatabase(this.myTriggerTableWnv);
                }
            }
        }
        private void transferFromWebSelectToDatabase(webNodeValue wnv)
        {
            v.SQL = v.SQL + v.ENTER + myNokta + " transferFromWebSelectToDatabase";

            /// Web den okunan tabloyu dB ye aktarma işlemleri
            ///

            /// tabloyu ele al
            /// 
            if (wnv.tTable == null) return;
            tTable tb = wnv.tTable;
            /// data yoksa geri dön
            if (tb.tRows.Count <= 1) return;

            ds_DbaseTable = null;
            dN_DbaseTable = null;
            //DataRow dbRow = null;

            int rowNo = 0;
            //Int16 colNo = 0;
            bool onay = false;
            //bool firstRow = true;
            //bool itemButton = false;
            //string itemText = "";
            //string itemValue = "";


            /// sırayla row ları ele al
            ///
            foreach (tRow rows in tb.tRows)
            {
                // her zaman true olsun
                onay = true;

                /// 0 sıfırı atlamasının nedeni caption row
                if (rowNo == 0)
                {
                    if (wnv.TagName == "table")
                        onay = false;

                    if (wnv.TagName == "select")
                        onay = true;
                }

                if (onay)
                {
                    saveRowAsync(wnv, rows);

                    if (msPagesService.editKayitKontrolu()) break;
                }
                rowNo++;
            }
        }
        private async Task saveRowAsync(webNodeValue wnv, tRow tableRows)
        {
            v.SQL = v.SQL + v.ENTER + myNokta + " saveRow";

            /// table in bir row unu alıp tüm column larını tarayarak db ye kaydet işlemini gerçekleşitiriyor
            ///
            DataRow dbRow = null;
            string itemText = "";
            string itemValue = "";
            Int16 colNo = 0;
            bool firstColumn = true;

            #region wnv.table verilerini db ye aktaralım

            foreach (tColumn column in tableRows.tColumns)
            {
                /// web üzerinden okunacak column noyu bildir
                wnv.tTableColNo = colNo;
                /// bu column datasının aktarılacağı tabloyu ve column nu bul
                wnv.dbFieldName = "";
                wnv.dbLookUpField = false;

                /// column üzerindeki bilgiler hangi db ye yazılacak tespit edelim
                if (wnv.TagName == "table")
                    dbRow = findRightRow(wnv, v.tSelect.Get);

                // burada select içindeki value ve text istenen bir tabloya yazılıcak
                if ((wnv.TagName == "select") &&
                    (wnv.AttRole == "ItemTable")) // (wnv.EventsType == v.tWebEventsType.itemTable))
                    dbRow = findRightRow(wnv, v.tSelect.Get);

                // burada select içindeki value ve text MsWebNodeItems tablosuna yazılacak
                if ((wnv.TagName == "select") &&
                    (wnv.AttRole != "ItemTable"))//(wnv.EventsType != v.tWebEventsType.itemTable))
                    dbRow = findRightRowForSelect(wnv, v.tSelect.Get);

                /// yeni data satırı oluştur
                /// tabloy aktarmaya başladığında ilk yeni row oluştursun
                /// bir defa çalışıyor
                if ((firstColumn) && (dbRow != null))
                {
                    msPagesService.dbButtonClick(this, ds_DbaseTable.DataSetName, v.tButtonType.btYeniHesapSatir);
                    dbRow = ds_DbaseTable.Tables[0].Rows[dN_DbaseTable.Position];
                    firstColumn = false;
                }

                /// dbRow geldiyse ilgili tablo ve column bulunmuş demektir
                /// webden okunan veriyi db ye aktardığı an
                if (dbRow != null)
                {
                    // select node için itemValue tespiti
                    itemText = tableRows.tColumns[colNo].value;

                    if (t.IsNotNull(wnv.dbFieldName))
                    {
                        if (wnv.dbLookUpField == false)
                        {
                            if (itemText != "")
                                dbRow[wnv.dbFieldName] = itemText;
                        }
                        else
                        {
                            itemValue = findNodeItemsValue(wnv, itemText);

                            if (itemValue != "")
                                dbRow[wnv.dbFieldName] = itemValue;
                        }
                    }
                }

                colNo++;
            }

            #endregion wnv.table verilerini db ye aktaralım

            /// Sıra geldi wvn.table dışındaki web üzerinde bulunan başka verileri alalım
            /// 
            //if (wnv.TagName == "table")
            if (this.myWebTableRowToDatabase)
            {
                v.SQL = v.SQL + v.ENTER + myNokta + " saveRow : start other nodes read ";
                //runScrapingAsync(ds_MsWebNodes, this.myTriggerWebRequestType, this.myTriggerEventsType, 0, wnv.nodeId);
                runScrapingAsync(ds_MsWebNodes, this.myTriggerWebRequestType, v.tWebEventsType.tableField, 0, wnv.nodeId);
            }

            if ((wnv.TagName == "table") ||
                (wnv.TagName == "select"))
            {
                /// final : yeni dbrow oluşturuldu, 
                /// wnv.table üzerindeki tüm colomn tarandı ve gerekli atamalar yapıldı
                /// wnv.table dışındaki diğer column lar da tarandı ve bulununca onlarda web den alındı
                /// bir row un okunması tamalandı, o zaman bunu kaydedelim, ve yeni satır açalım
                /// 
                if (t.IsNotNull(ds_DbaseTable) && (wnv.DontSave == false))
                {
                    msPagesService.dbButtonClick(this, ds_DbaseTable.DataSetName, v.tButtonType.btKaydetYeni);
                }
            }
        }

        #endregion transferFrom... , saveRow

        #region find functions

        private void findOneByOneButton(string TableIPCode)
        {
            string[] controls = new string[] { };

            this.btn_OneByOne = null;
            this.btn_OneByOne = t.Find_Control(this, "checkButton_ek1", TableIPCode, controls);

            if (this.btn_OneByOne != null)
            {
                this.myTriggerOneByOne = true;
            }
            if ((this.myTriggerOneByOne) &&
                (this.ds_DbaseSiraliTable == null))
            {
                // aslında az önce bulunan dataseti tekrar buluyoruz 
                // fakat işlem bitene kadar bu dataset elimizde olacak
                t.Find_DataSet(this, ref ds_DbaseSiraliTable, ref dN_DbaseSiraliTable, TableIPCode);
            }
        }

        private bool findTableItemButton(tRow dataRow, int dataRowNo)//, ref webNodeValue wnv)
        {
            // Tablo üzerinde bulunduğu satırdaki bilgilerin detayını göstermek için buton var mı ?
            // 
            v.SQL = v.SQL + v.ENTER + myNokta + " find tableItemButton RowNo : " + dataRowNo.ToString();

            bool onay = false;
            bool isActive = false;
            //Int16 eventsType = 0;
            string value = "";
            string AttRole = "";

            //foreach (DataRow row in ds_ScrapingNodes.Tables[0].Rows)
            foreach (DataRow nodeRow in ds_MsWebNodes.Tables[0].Rows)
            {
                isActive = (bool)nodeRow["IsActive"];
                //eventsType = t.myInt16(row["EventsType"].ToString());
                AttRole = nodeRow["AttRole"].ToString();

                //if (eventsType == (Int16)v.tWebEventsType.itemButton)
                if (AttRole == "ItemButton")
                {
                    // şimdilik elimizde sadece
                    // tablonun column.value  ile  ds_ScrapingNodes  karşılaştırabileceğimiz sadece src adresi var
                    // 

                    // ds_ScrapingNodes üzerindeki value 
                    value = nodeRow["AttSrc"].ToString();

                    int i2 = dataRow.tColumns.Count;
                    for (int i = 0; i < i2; i++)
                    {
                        if (value == dataRow.tColumns[i].value)
                        {
                            // burada ds_ScrapingNodes içindeki itemButton ait olan detay bilgisi gerekiyor
                            //
                            this.myTriggerTableRow = nodeRow;

                            tableItemButtonClickAsync(dataRowNo);
                            onay = true;
                            break;
                        }
                    }
                    /*
                    foreach (tColumn column in dataRow.tColumns)
                    {
                        if (value == column.value)
                        {
                            // burada ds_ScrapingNodes içindeki itemButton ait olan detay bilgisi gerekiyor
                            //
                            this.myTriggerTableRow = nodeRow;
                            
                            tableItemButtonClickAsync(dataRowNo);
                            onay = true;
                            break;
                        }
                    }
                    */
                    // columns taradı ve bulamadı
                    break;
                }
            }

            return onay;
        }

        private string findNodeItemsValue(webNodeValue wnv, string findText)
        {
            if (t.IsNotNull(ds_WebNodeItemsList) == false) return "";

            string value = "";
            int nodeId_ = wnv.nodeId;
            string pageCode_ = "";
            string itemText_ = "";
            string itemValue_ = "";
            findText = findText.Trim();
            
            int length = ds_WebNodeItemsList.Tables[0].Rows.Count;
            for (int i = 0; i < length; i++)
            {
                pageCode_ = ds_WebNodeItemsList.Tables[0].Rows[i]["PageCode"].ToString().Trim();
                itemValue_ = ds_WebNodeItemsList.Tables[0].Rows[i]["ItemValue"].ToString().Trim();
                itemText_ = ds_WebNodeItemsList.Tables[0].Rows[i]["ItemText"].ToString().Trim();

                /// nodeId kontrolü olmadı  
                /// get ve set node ler farklı oluyor
                /// get ederken tablolalarda alabiliyoruz
                /// set ederken select nodeyi kullanıyoruz
                /// haliyle nodeId tutmuyor
                /// 
                if (wnv.pageCode == pageCode_)
                {
                    if ((findText == itemText_) || (findText == itemValue_))
                    {
                        value = ds_WebNodeItemsList.Tables[0].Rows[i]["ItemValue"].ToString();
                        return value;
                    }
                }
                else
                {
                    if ((findText == itemText_) || (findText == itemValue_))
                    {
                        if ((pageCode_ == "DONEMLER") || (pageCode_ == "GRUPLAR") || (pageCode_ == "SUBELER"))
                        {
                            value = ds_WebNodeItemsList.Tables[0].Rows[i]["ItemValue"].ToString();
                            return value;
                        }
                    }
                }
            }

            // bir eşleşmezse bulmaz ise gelen value dönsün
            // böylece bazı WebNodeItems lar saklanmaya gerek kalmadı
            if (value == "")
            {
                MessageBox.Show("DİKKAT : MsWebNodeItems tablosunda aranan text in valuesi tespit edilemedi ..." + v.ENTER2 + wnv.AttName + " = " + findText);
                value = findText;
            }
            return value;
        }

        private DataRow findRightRow(webNodeValue wnv, v.tSelect select)
        {
            //v.SQL = v.SQL + v.ENTER + "findRightRow";

            string _TableIPCode = "";
            string _pageCode = wnv.pageCode;
            string _dbPageCode = "";
            int _nodeId = wnv.nodeId;
            Int16 _colNo = 0;
            int _dbNodeId = 0;
            Int16 _dbColNo = 0;
            Control cntrl = null;

            if (wnv.tTable != null)
                _colNo = wnv.tTableColNo;

            DataRow findDbRow = null;

            // okunacak veya yazılacak tablo
            //if (ds_DbaseTable == null)
            //    ds_DbaseTable = new DataSet();

            //if (colNo == 6)
            //    Thread.Sleep(100);

            foreach (DataRow row in ds_ScrapingDbConnectionList.Tables[0].Rows)
            {
                if (select == v.tSelect.Get)
                {
                    _dbNodeId = t.myInt32(row["WebScrapingGetNodeId"].ToString());
                    _dbColNo = t.myInt16(row["WebScrapingGetColumnNo"].ToString());
                }
                if (select == v.tSelect.Set)
                {
                    _dbNodeId = t.myInt32(row["WebScrapingSetNodeId"].ToString());
                    _dbColNo = t.myInt16(row["WebScrapingSetColumnNo"].ToString());
                }

                // durdurmakmak için
                //if (dbColNo == 8)
                //    dbColNo = 8;

                _dbPageCode = row["WebScrapingPageCode"].ToString();

                if ((_dbPageCode == _pageCode) &&
                    (_dbNodeId == _nodeId) &&
                    (_dbColNo == _colNo))
                {
                    // yazılacak veya okunacak Table
                    _TableIPCode = row["TableIPCode"].ToString();

                    // böyle TableIPCode bulduk fakat form üzerinde böyle bir control var mı
                    // çünkü WebScrapingPageCode tanımı birden fazla InputPanel de kullanılabiliyor
                    cntrl = t.Find_Control_View(this, _TableIPCode);

                    // varsa devam
                    if (cntrl != null)
                    {
                        // yazılacak veya okunacak fieldName
                        wnv.dbFieldName = row["FIELD_NAME"].ToString();
                        wnv.dbLookUpField = Convert.ToBoolean(row["FLOOKUP_FIELD"].ToString());
                        wnv.dbFieldType = t.myInt16(row["FIELD_TYPE"].ToString());

                        ds_DbaseTable = null;
                        dN_DbaseTable = null;
                        t.Find_DataSet(this, ref ds_DbaseTable, ref dN_DbaseTable, _TableIPCode);

                        if (t.IsNotNull(ds_DbaseTable))
                        {
                            if (dN_DbaseTable.Position == -1)
                            {
                                dN_DbaseTable.Position = 0;
                                dN_DbaseTable.Tag = 0;
                                findDbRow = ds_DbaseTable.Tables[0].Rows[0];
                            }
                            else
                                findDbRow = ds_DbaseTable.Tables[0].Rows[dN_DbaseTable.Position];

                            /// Sıralı işlem hangi TableIPCode üzerinde gerçekleşecek onu bulalım
                            ///
                            if (this.myTriggerOneByOne == false)// &&
                                                                //(this.ds_DbaseSiraliTable == null))
                            {
                                findOneByOneButton(_TableIPCode);
                            }

                            return findDbRow;
                        }
                    }
                }

            }

            if (findDbRow == null)
            {
                // v.SQL = v.SQL + v.ENTER + "DİKKAT : findRightRow, uygun row bulunamadı. PageCode : " + pageCode + ", nodeId : " + nodeId.ToString();
            }

            // işlem buraya kadar geldiyse null dönüyor
            //
            return findDbRow;
        }

        private DataRow findRightRowForSelect(webNodeValue wnv, v.tSelect select)
        {
            //v.SQL = v.SQL + v.ENTER + "findRightRowForSelect";

            string pageCode = wnv.pageCode;
            int nodeId = wnv.nodeId;
            Int16 colNo = 0;
            //int dbNode = 0;
            Int16 dbColNo = 0;

            if (wnv.tTable != null)
                colNo = wnv.tTableColNo;

            DataRow findDbRow = null;

            // okunacak veya yazılacak tablo
            if (ds_DbaseTable == null)
                ds_DbaseTable = new DataSet();

            foreach (DataRow row in ds_ScrapingDbConnectionList.Tables[0].Rows)
            {
                if (select == v.tSelect.Get)
                {
                    //dbNode = Convert.ToInt32(row["WebScrapingGetNodeId"].ToString());
                    dbColNo = Convert.ToInt16(row["WebScrapingGetColumnNo"].ToString());
                }
                if (select == v.tSelect.Set)
                {
                    //dbNode = Convert.ToInt32(row["WebScrapingSetNodeId"].ToString());
                    dbColNo = Convert.ToInt16(row["WebScrapingSetColumnNo"].ToString());
                }

                if ((row["WebScrapingPageCode"].ToString() == "SELECTNODEITEMS") &&
                    (dbColNo == colNo))
                {
                    // yazılacak veya okunacak Table
                    string TableIPCode = row["TableIPCode"].ToString();

                    // yazılacak veya okunacak fieldName
                    wnv.dbFieldName = row["FIELD_NAME"].ToString();
                    wnv.dbLookUpField = Convert.ToBoolean(row["FLOOKUP_FIELD"].ToString());
                    wnv.dbFieldType = t.myInt16(row["FIELD_TYPE"].ToString());

                    if ((ds_DbaseTable == null) ||
                        (ds_DbaseTable.DataSetName != TableIPCode))
                    {
                        ds_DbaseTable = null;
                        dN_DbaseTable = null;
                        t.Find_DataSet(this, ref ds_DbaseTable, ref dN_DbaseTable, TableIPCode);
                    }

                    if (t.IsNotNull(ds_DbaseTable))
                    {
                        findDbRow = ds_DbaseTable.Tables[0].Rows[dN_DbaseTable.Position];

                        return findDbRow;
                    }
                }
            }

            if (findDbRow == null)
            {
                // v.SQL = v.SQL + v.ENTER + "DİKKAT : findRightRowForSelect, uygun row bulunamadı. PageCode : " + pageCode + ", nodeId : " + nodeId.ToString();
            }

            return findDbRow;
        }

        //KRT_OPERAND_TYPE == 10
        private void findRightDbTables(webNodeValue wnv)
        {
            // database tablsoundaki web deki bir tabloya veri atacağımız zaman iki si arasında bir eşleştirme yapmak gerekiyor
            // örnek : table deki bir TCNo yu alıp webdeki tabloda önce onu bulmamız gerekiyor
            // şimdi db table deki veri önce tespit alanları edelim

            string _TableIPCode = "";
            string _pageCode = wnv.pageCode;
            string _dbPageCode = "";
            string _krtOperandType = "";
            
            tTable _tTable = new tTable();

            // sadece eşleştirme yapılacak anahtar fieldi tespit ediyor
            foreach (DataRow row in ds_ScrapingDbConnectionList.Tables[0].Rows)
            {
                _dbPageCode = row["WebScrapingPageCode"].ToString();
                _krtOperandType = row["KRT_OPERAND_TYPE"].ToString();
                _TableIPCode = row["TableIPCode"].ToString();
                if (_pageCode == _dbPageCode)
                {
                    if (_krtOperandType == "10") // eşleşme yapılack field nerde ise aradığımız TableIPCode o dur.
                    {
                        // örnek : TCNo hangi gibi keyFieldName tespit ediliyor, karşılaştırma için kullanılacak
                        wnv.tTableColNo = t.myInt16(row["WebScrapingSetColumnNo"].ToString());
                        wnv.dbFieldName = row["FIELD_NAME"].ToString();
                        wnv.TableIPCode = _TableIPCode;
                        break;
                    }
                }
            }

            // anahtar field dışındaki atama yapılacak diğer fieldleri tespit ediyor
            foreach (DataRow row in ds_ScrapingDbConnectionList.Tables[0].Rows)
            {
                _dbPageCode = row["WebScrapingPageCode"].ToString();
                _krtOperandType = row["KRT_OPERAND_TYPE"].ToString();
                _TableIPCode = row["TableIPCode"].ToString();
                if (_pageCode == _dbPageCode)
                {
                    // wnv.TableIPCode inin içeriği _krtOperandType == "10" sağlanınca bulunuyor 
                    // harici zamanlarda boş
                    if ((_TableIPCode == wnv.TableIPCode) &&
                        (_krtOperandType != "10"))
                    {
                        // database tablodan okunacak fieldler tespit ediliyor

                        tRow _tRow = new tRow();

                        // html table üzerindeki column no
                        tColumn _tColumnNo = new tColumn();
                        _tColumnNo.value = row["WebScrapingSetColumnNo"].ToString();
                        _tRow.tColumns.Add(_tColumnNo);

                        // database table üzerindeki okunacak fieldName
                        tColumn _tColumn = new tColumn();
                        _tColumn.value = row["FIELD_NAME"].ToString();
                        _tRow.tColumns.Add(_tColumn);

                        // o fieldin value değerini html table ye taşıma için kullanılacak şimdilik sadece boş olan column
                        tColumn _tColumnValue = new tColumn();
                        _tColumnValue.value = "";
                        _tRow.tColumns.Add(_tColumnValue);

                        _tTable.tRows.Add(_tRow);
                    }
                }
            }

            wnv.tTable = _tTable;

        }

        #endregion find 

        #region btnOneByOne - Next Record

        private bool btnOneByOneNextRecord()
        {
            bool onay = true;

            if (((DevExpress.XtraEditors.CheckButton)this.btn_OneByOne).Checked)
            {
                if (this.ds_DbaseSiraliTable != null)
                {

                    if (this.ds_DbaseSiraliTable.Tables[0].Rows.Count == this.dN_DbaseSiraliTable.Position + 1)
                    {
                        NavigatorButton btnPrev = this.dN_DbaseSiraliTable.Buttons.Prev;
                        this.dN_DbaseSiraliTable.Buttons.DoClick(btnPrev);

                        this.myTriggerOneByOne = false;
                        ((DevExpress.XtraEditors.CheckButton)this.btn_OneByOne).Checked = false;

                        onay = false;

                        t.AlertMessage("", "Sıralı işler tamamlandı ...");
                    }

                    NavigatorButton btnNext = this.dN_DbaseSiraliTable.Buttons.Next;
                    this.dN_DbaseSiraliTable.Buttons.DoClick(btnNext);

                    //v.SQL = v.SQL + v.ENTER + myNokta + " btnOneByOneNextRecord() : timerTrigger.Enabled =  true ";

                    //timerTrigger.Interval = 1000;// 1500;
                    //timerTrigger.Enabled = true;
                }
            }

            return onay;
        }

        #endregion btnOneByOne - Next Record

        #region Resim download

        
        public void DownloadFile(string UrlString)
        {
            string DescFilePath = "C:\\SqlData";
            string fileName = System.IO.Path.GetFileName(UrlString);
            string descFilePathAndName =
                System.IO.Path.Combine(DescFilePath, fileName);
            try
            {
                WebRequest myre = WebRequest.Create(UrlString);
            }
            catch
            {
                return;// false;
            }
            try
            {
                byte[] fileData;
                using (WebClient client = new WebClient())
                {
                    fileData = client.DownloadData(UrlString);
                }
                using (FileStream fs =
                      new FileStream(descFilePathAndName, FileMode.OpenOrCreate))
                {
                    fs.Write(fileData, 0, fileData.Length);
                }
                return;// true;
            }
            catch (Exception ex)
            {
                throw new Exception("download field", ex.InnerException);
            }
            /////https://support.microsoft.com/en-us/help/2512241/how-to-upload-and-download-files-from-a-remote-server-in-asp-net
        }

        public System.Drawing.Image DownloadImageFromUrl(string imageUrl)
        {
            System.Drawing.Image image = null;

            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                image = System.Drawing.Image.FromStream(stream);

                webResponse.Close();
            }
            catch (Exception ex)
            {
                return null;
            }

            return image;
        }

        // Get the picture at a given URL.
        private Image GetPicture(string url)
        {
            try
            {
                url = url.Trim();
                //if (!url.ToLower().StartsWith("http://"))
                //    url = "http://" + url;
                WebClient web_client = new WebClient();
                MemoryStream image_stream =
                    new MemoryStream(web_client.DownloadData(url));
                return Image.FromStream(image_stream);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error downloading picture " +
                    url + '\n' + ex.Message,
                    "Download Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            return null;
        }

        /// <summary>
        /// session ve token almak için örnek
        /// şimdilik kullanılmıyor
        /// </summary>
        private void getSessionAndToken()
        {
            string url = @"";
            url = @"https://mebbis.meb.gov.tr/default.aspx";

            ///"txtKullaniciAd", "99969564"
            ///"txtSifre", "Ata.5514126")

            var baseAddress = new Uri(url);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                //UNUTMA MEBBİS KODU değişecek
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("txtKullaniciAd", "99969564"),
                    new KeyValuePair<string, string>("txtSifre", "Ata.5514126"),
                });
                cookieContainer.Add(baseAddress, new Cookie("CookieName", "madhav"));
                var result = client.PostAsync("", content).Result;
                result.EnsureSuccessStatusCode();
                var value = result.Headers.ToString();

                //this.sessionId = t.myGetValue(value, "ASP.NET_SessionId=", ";");
                //this.token = t.myGetValue(value, "__RequestVerificationToken=", ";");
            }
        }

        #endregion Resim

        #region ItemButton ( ItemTable içinde çalışıyor )
        private async Task tableItemButtonClickAsync(int RowNo)
        {
            v.SQL = v.SQL + v.ENTER + myNokta + " preparing tableItemButtonClick : RowNo : " + RowNo.ToString();

            webNodeValue wnv_ = new webNodeValue();

            // scraping için gerekli verileri hazırla
            //
            wnv_.workRequestType = this.myTriggerWebRequestType;
            //wnv_.workEventsType = v.tWebEventsType.itemButton;
            wnv_.AttRole = "ItemButton";

            msPagesService.nodeValuesPreparing(this.myTriggerTableRow, ref wnv_, this.aktifPageCode);

            // clik yapılacak olan nodeyi bulup getirmek için scraping işlemini gerçekleştir
            //
            await WebScrapingAsync(webMain, wnv_); // tableItemButtonClickAsync

            if (wnv_ != null)
            {
                // sadece istenen RowNo yu için itemButton click le
                //
                if (RowNo >= 0)
                    applyTableItemButtonClick_(wnv_, RowNo);

                // burada tetiklenme başlayınca sırayla kendi kendini tetikliyor
                // henüz gerek olmadı
                if (RowNo == -1)
                    runAllItemButton_();
            }
        }
        private void applyTableItemButtonClick_(webNodeValue wnv_, int RowNo)
        {
            v.SQL = v.SQL + v.ENTER + myNokta + " apply tableItemButtonClick RowNo : " + RowNo.ToString();
            //if (this.myTriggerWnv.elements != null)
            if (wnv_.elements != null)
            {
                if (wnv_.elements.Count > RowNo)
                {
                    //HtmlElement item = this.myTriggerWnv.elements[RowNo];
                    HtmlElement item = wnv_.elements[RowNo];
                    if (item != null)
                    {
                        this.myTriggeringItemButton = true;
                        item.InvokeMember("click");

                        Application.DoEvents();
                        if (this.timerTrigger.Enabled == false)
                        {
                            this.timerTrigger.Enabled = true;
                            v.SQL = v.SQL + v.ENTER + myNokta + " applyTableItemButtonClick_ : for start : timerTrigger.Enabled = true";
                        }

                    }
                }
            }
        }
        private void runAllItemButton_()
        {
            // DİKKAT : burası şimdili kullanılmıyor, gerekli olacak bir durum oluşmadı
            //
            // burası şimdilik sadece sırayla click leniyor 
            // click ten sonra ne iş yapacağı belirtilmedi 
            // -----

            /// burada sadece ItemButton olarak seçilen nodeler sadece click leniyor
            /// clicklendikten sonra  xxx_DocumentCompleted(...  de bu algılanıyor ve
            /// this.timerTrigger.Enabled =  true;  tetikleniyor
            /// timer tetiklenice timerTrigger_Tick( .. içindeki komutlar çalışyor ve
            /// yeniden runItemButton(); çalışyor taki   this.myTriggering = false  olana kadar 

            if (this.myTriggerWnv == null) return;

            if (this.myTriggerTableCount <= this.myTriggerTableRowNo)
            {
                // bitti
                // tetiklenecek başka itemButton kalmadı
                this.myTriggeringTable = false;
                return;
            }

            int pos = -1;
            string rowNo = "";

            if (this.myTriggerTableCount > this.myTriggerTableRowNo)
            {
                for (int i = 0; i < this.myTriggerTableCount; i++)
                {
                    // ||1||,||2||,||3|| ...
                    rowNo = "||" + (i + 1).ToString() + "||";

                    // rowNo ile bu satır çalıştı mı diye kontrol için sadece
                    pos = this.myTriggerItemButonList.IndexOf(rowNo);

                    // -1 ise daha önce çalışmamış 
                    if (pos == -1)
                    {
                        HtmlElement item = this.myTriggerWnv.elements[i];

                        //*
                        //* <img onmouseover="this.src="/images/toolimages/open_kucuk_a.gif";this.style.cursor="hand";" onmouseout="this.src="/images/toolimages/open_kucuk.gif";this.style.cursor="default";" 
                        //* onclick="fnIslemSec("99969564|01/08/2017|NMZVAN93C15010059");" src="/images/toolimages/open_kucuk.gif">
                        //*

                        item.InvokeMember("click");

                        Thread.Sleep(500);

                        // bu şüpheli
                        //this.myTriggerTableRowNo++;

                        rowNo = "||" + (this.myTriggerTableRowNo).ToString() + "||";

                        this.myTriggerItemButonList = this.myTriggerItemButonList + rowNo;

                        break;
                    }
                }
            }
        }

        #endregion ItemButton

        #region subFunctions
        private async Task<bool> loadPage(WebBrowser wb, string url)
        {
            //
            if (!string.IsNullOrEmpty(url))
            {
                wb.Navigate(url);
                return true;
            }
            else
            {
                MessageBox.Show("DİKKAT : Lütfen  - page url - yi girin...");
                return false;
            }
        }
        private async Task myPageViewClickAsync(WebBrowser wb)
        {
            if (ds_MsWebPages != null)
            {
                bool onay = false;
                string url = "";

                v.SQL = v.SQL + v.ENTER + myNokta + " PageView : ";

                this.aktifPageCode = ds_MsWebPages.Tables[0].Rows[dN_MsWebPages.Position]["PageCode"].ToString();

                url = ds_MsWebPages.Tables[0].Rows[dN_MsWebPages.Position]["PageUrl"].ToString();
                this.talepEdilenUrl = url;
                this.talepEdilenUrl2 = url;
                url = ds_MsWebPages.Tables[0].Rows[dN_MsWebPages.Position]["BeforePageUrl"].ToString();
                this.talepOncesiUrl = url;

                this.talepPageLeft = t.myInt16(ds_MsWebPages.Tables[0].Rows[dN_MsWebPages.Position]["PageLeft"].ToString());
                this.talepPageTop = t.myInt16(ds_MsWebPages.Tables[0].Rows[dN_MsWebPages.Position]["PageTop"].ToString());

                if (this.aktifUrl != this.talepEdilenUrl)
                {
                    if (t.IsNotNull(this.talepOncesiUrl))
                        onay = await loadPage(wb, this.talepOncesiUrl);
                    else
                        onay = await loadPage(wb, this.talepEdilenUrl);
                }
                else
                {
                    onay = await loadPage(wb, this.talepEdilenUrl);
                }
            }
        }

        private void displayNone(string tagName, string idName, string XPath, string InnerText)
        {
            //document.getElementById("demo").hidden = "display : none";

            //if (this.aktifUrl == this.talepEdilenUrl2)
            //{
            //    if (this.talepPageLeft != 0 || this.talepPageTop != 0) return;
            //}

            if (t.IsNotNull(idName))
            {
                if (tagName == "table")
                {
                    HtmlElementCollection elements = webMain.Document.GetElementsByTagName(tagName);
                    foreach (HtmlElement item in elements)
                    {
                        if (item.Id == idName)
                        {
                            if (InnerText != "")
                            {
                                if (item.InnerText.IndexOf(InnerText) > -1)
                                {
                                    item.InvokeMember("click");
                                    item.SetAttribute("hidden", "display : none");
                                    break;
                                }
                            }
                            else
                            {
                                item.InvokeMember("click");
                                item.SetAttribute("hidden", "display : none");
                            }
                        }
                    }
                }
                else
                {
                    HtmlElement element = null;
                    element = null;
                    element = webMain.Document.GetElementById(idName);
                    if (element != null)
                        element.SetAttribute("hidden", "display : none");
                }
            }
            else
            {
                /* xpath le hedef nodeyi bulamadım
                 * 
                //XPath = "//body[1]" + XPath;
                this.htmlDocumentBody = webMain.Document.Body.InnerHtml;
                HtmlAgilityPack.HtmlNode htmlBody = null;
                loadBody(this.htmlDocumentBody, ref htmlBody);

                if (htmlBody != null)
                {
                    HtmlNode node = null;
                    node = htmlBody.SelectSingleNode(XPath);
                    if (node !=null)
                    {
                        //node.  SetAttribute("hidden", "display : none");
                        node.SetAttributeValue("hidden", "display : none");
                    }

                    HtmlNodeCollection nodes = null;
                    nodes = htmlBody.SelectNodes(XPath);
                }
                */
            }

        }
                
        private void getHtmlTable(WebBrowser wb, ref webNodeValue wnv, string idName)
        {
            wnv.tTable = null;// Mevcudu yok et, yeni hazırlanacak
            HtmlElement htmlTable = wb.Document.GetElementById(idName);
            if (htmlTable == null) return;

            // okunacak table içinde pages tables var mı kontrol etmek gerekiyor
            // 
            //this.htmlTablePages = null;
            //this.selectedTablePageNo = 0;
                        
            if (areThereTablePages(htmlTable))
            {
                this.tablePageName = idName;
                this.tablePageUrl = wb.Url.ToString();
            }
                        
            HtmlElementCollection htmlRows = htmlTable.GetElementsByTagName("tr");
            int rowCount = htmlRows.Count;
            int colCount = 0;
            int pos = 0;
            string value = "";
            string html = "";

            string dtyHtml = "";
            string dtyValue = "";
            string dtyIdName = "";
            string dtyType = "";

            tTable _tTable = new tTable();

            //for (int i = 0; i < rowCount; i++)
            for (int i = 1; i < rowCount; i++)
            {
                HtmlElement hRow = htmlRows[i];

                if (hRow.Name != "pages")
                {
                    tRow _tRow = new tRow();

                    HtmlElementCollection htmlCols = hRow.GetElementsByTagName("td");
                    colCount = htmlCols.Count;

                    for (int i2 = 0; i2 < colCount; i2++)
                    {
                        HtmlElement hCol = htmlCols[i2];

                        // <input name="dgListele$ctl04$ab" id="dgListele_ctl04_chkOnayBekliyor" type="radio" checked="checked" value="chkOnayBekliyor">
                        // <input name="dgListele$ctl04$ab" id="dgListele_ctl04_chkOnayDurum" type="radio" value="chkOnayDurum">

                        //HtmlElementCollection kDetay = kolon.Children;
                        dtyHtml = hCol.InnerHtml;

                        //dtyAhref = hCol.Children[0].GetAttribute("href");  << çalışıyor 
                        //dtySrc = hCol.Children[0].GetAttribute("src");     << çalışmıyor

                        if (dtyHtml.IndexOf("type=\"radio\"") > -1)
                        {
                            //dtyValue = hCol.Children[0].GetAttribute("value");
                            dtyValue = hCol.Children[0].GetAttribute("");
                            dtyIdName = hCol.Children[0].GetAttribute("id");
                            dtyType = hCol.Children[0].GetAttribute("type");
                            if (dtyType == "radio")
                            {
                                wb.Document.GetElementById(dtyIdName).InvokeMember("click");
                            }
                        }

                        if (hCol.InnerText != null)
                        {
                            value = hCol.InnerText.Trim();
                        }
                        else
                        {
                            // <img onmouseover="this.src="/images/toolimages/open_kucuk_a.gif";this.style.cursor="hand";" 
                            // onmouseout="this.src="/images/toolimages/open_kucuk.gif";this.style.cursor="default";" 
                            // onclick="fnIslemSec("99969564|01/08/2017|NMZVAN93C15010059");" 
                            // src="/images/toolimages/open_kucuk.gif">

                            pos = -1;
                            pos = hCol.OuterHtml.IndexOf("onclick");

                            // <td align="center" style="width: 30px;">
                            // <a href="javascript:__doPostBack('dgIstekOnaylanan','Select$0')">
                            // <img title="Aç" onmouseover="this.src='/images/toolimages/open_kucuk_a.gif';this.style.cursor='pointer';" onmouseout="this.src='/images/toolimages/open_kucuk.gif';this.style.cursor='default';" src="/images/toolimages/open_kucuk.gif">
                            // </a></td>                        

                            if (pos == -1) // 
                                pos = hCol.OuterHtml.IndexOf("__doPostBack(");

                            if (pos > -1)
                            {
                                //value = hCol.GetAttribute("src");
                                //System.Windows.Forms.HtmlDocument doc = preparingHtmlDocument(hCol.OuterHtml);
                                //value = doc.GetElementsByTagName("img")[0].GetAttribute("src");

                                //   /images/toolimages/open_kucuk.gif

                                html = hCol.OuterHtml.Remove(0, hCol.OuterHtml.IndexOf(" src=") + 6);
                                value = html.Remove(html.IndexOf(">") - 1);
                            }
                            else value = "";
                        }
                        tColumn _tColumn = new tColumn();
                        _tColumn.value = value;
                        _tRow.tColumns.Add(_tColumn);
                    }

                    if (_tRow.tColumns.Count > 0)
                    {
                        if (_tRow.tColumns[0].value != "pages")
                           _tTable.tRows.Add(_tRow);
                    }

                }

                if (hRow.Name == "pages")
                {
                    //işlem yapma
                }
            }

            wnv.tTable = _tTable;
        }

        // table içinde sayfalar var mı ?
        private bool areThereTablePages(HtmlElement htmlTable)
        {
            // Pages için table varmı kontrol et, varsa -pages- şeklinde işaretler 
            // böylece table dan data okurken page bilgisini bulunduran satırları atlasın
            //
            // <tbody><tr>
            // <td><span>1</span></td>
            // <td><a href="javascript:__doPostBack('dgIstekOnaylanan','Page$2')"> 2 </a></td>
            // <td><a href="javascript:__doPostBack('dgIstekOnaylanan','Page$3')"> 3 </a></td>
            // </tr>
            // </tbody>

            bool onay = false;
            // bu kopya ise esas tabloya müdehale için kullanılıyor
            HtmlElementCollection htmlTablePages_ = htmlTable.GetElementsByTagName("table");
            
            int tableCount = htmlTablePages_.Count;
            int rowCount = 0;
            int colCount = 0;

            for (int i = 0; i < tableCount; i++)
            {
                if ((htmlTablePages_[i].InnerHtml.IndexOf("javascript:__doPostBack(") > -1) &&
                    (htmlTablePages_[i].InnerHtml.IndexOf("Page$") > -1))
                {
                    HtmlElement hTable = htmlTablePages_[i];
                    HtmlElementCollection htmlRows = hTable.GetElementsByTagName("tr");
                    rowCount = htmlRows.Count;
                    
                    for (int i2 = 0; i2 < rowCount; i2++)
                    {
                        HtmlElement hRow = htmlRows[i2];
                        HtmlElementCollection htmlCols = hRow.GetElementsByTagName("td");
                        colCount = htmlCols.Count;

                        for (int i3 = 0; i3 < colCount; i3++)
                        {

                            // sayfa 1 için durum
                            // hcol.InnerHtml = <span>1</span>
                            // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$2')">2</a>
                            // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$3')">3</a>
                            //
                            // sayfa 2 için durum
                            // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$1')">1</a>
                            // hcol.InnerHtml = <span>2</span>
                            // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$3')">3</a>
                            //
                            // sayfa 3 için durum
                            // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$1')">1</a>
                            // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$2')">2</a>
                            // hcol.InnerHtml = <span>3</span>

                            HtmlElement hCol = htmlCols[i3];

                            if (hCol.InnerHtml.IndexOf("<span>") > -1)
                                this.selectedTablePageNo = i3 + 1;
                        }

                        if (hRow.TagName == "TR")
                        {
                            hRow.InnerText = "pages";
                            hRow.Name = "pages";
                        }
                    }
                }
            }

            if (tableCount > 0) 
                onay = true;

            return onay;
        }

        private void selectTablePage(WebBrowser wb)
        {
            
            if (this.htmlTablePages == null) return;

            v.SQL = v.SQL + v.ENTER + myNokta + " selectTablePage : " + this.selectedTablePageNo.ToString();

            int tableCount = this.htmlTablePages.Count;
            int rowCount = 0;
            int colCount = 0;

            // htmlTablePages birden fazla olabiliyor, tablonun başında ve tablonun en altında page ler için tablePages oluyor
            // click lemek için bir tanesine ulaşmamız yeterli olduğu için tableCount yerine 1 kullanıldı.

            //for (int i = 0; i < tableCount; i++)
            for (int i = 0; i < 1; i++)
            {
                if ((this.htmlTablePages[i].InnerHtml.IndexOf("javascript:__doPostBack(") > -1) &&
                    (this.htmlTablePages[i].InnerHtml.IndexOf("Page$") > -1))
                {
                    HtmlElement hTable = this.htmlTablePages[i];
                    HtmlElementCollection htmlRows = hTable.GetElementsByTagName("tr");
                    rowCount = htmlRows.Count;

                    for (int i2 = 0; i2 < rowCount; i2++)
                    {
                        HtmlElement hRow = htmlRows[i2];
                        HtmlElementCollection htmlCols = hRow.GetElementsByTagName("td");
                        colCount = htmlCols.Count;

                        for (int i3 = 0; i3 < colCount; i3++)
                        {
                            // sayfa 1 için durum
                            // hcol.InnerHtml = <span>1</span>
                            // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$2')">2</a>
                            // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$3')">3</a>
                            //
                            // sayfa 2 için durum
                            // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$1')">1</a>
                            // hcol.InnerHtml = <span>2</span>
                            // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$3')">3</a>
                            //
                            // sayfa 3 için durum
                            // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$1')">1</a>
                            // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$2')">2</a>
                            // hcol.InnerHtml = <span>3</span>

                            HtmlElement hCol = htmlCols[i3];

                            if (i3 + 1 == this.selectedTablePageNo)
                            {
                                string dtyIdName = "Page$" + this.selectedTablePageNo.ToString();

                                // bu metod çalışmadı
                                // string doPostBack = hCol.Children[0].GetAttribute("href");
                                // object y = wb.Document.InvokeScript(this.tablePageName, new string[] { doPostBack });
                                // object y = wb.Document.InvokeScript(this.tablePageName, new string[] { dtyIdName });

                                // bu metodda çalışmadı
                                // hCol.InvokeMember("click");

                                // bu metod çalışıyor
                                hCol.Children[0].SetAttribute("id", dtyIdName);
                                wb.Document.GetElementById(dtyIdName).InvokeMember("click");
                            }
                        }
                    }
                }
            }

        }

        // database üzerindeki tabloyu bul ve gönderilecek colums/kolonları oku 
        // ve okunan bu bu colums/kolonları htmlTable anahtar value ile post için gönder
        private void postHtmlTable(WebBrowser wb, ref webNodeValue wnv, string idName)
        {
            string _TableIPCode = wnv.TableIPCode;
            string _dbKeyFieldName = wnv.dbFieldName;
            tTable _tTable = wnv.tTable;

            string _dbFieldName = "";
            string _dbValue = "";

            if (t.IsNotNull(_TableIPCode))
            {
                DataSet ds = null;
                DataNavigator dN = null;
                string[] controls = new string[] { };
                t.Find_DataSet(this, ref ds, ref dN, _TableIPCode);

                // transfer edilecek database table bulundu
                if (t.IsNotNull(ds))
                {
                    // database tablosunun row/satırını oku, colums/kolonları tTable üzeri ata 
                    // htmlTable ye post için gönder, sonra sıradaki database tablosunun bir sonraki satırına geç
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        // örnek : TcNo okunuyor
                        wnv.keyValue = row[_dbKeyFieldName].ToString();

                        // TcNo haricindeki diğer colums/kolonlar okunuyor 
                        foreach (tRow item in _tTable.tRows)
                        {
                            _dbFieldName = item.tColumns[1].value;
                            _dbValue = row[_dbFieldName].ToString();
                            item.tColumns[2].value = _dbValue;
                        }

                        // hazırlanan bilgiyi html tabloya post et
                        postHtmlTable_(wb, ref wnv, idName);
                        
                    }
                }
            }
        }

        ///  html tablodaki doğru satırı/row u bul ve bulduğun satırın columns value leri doldur
        private void postHtmlTable_(WebBrowser wb, ref webNodeValue wnv, string idName)
        {
            Int16 _keyColumnNo = wnv.tTableColNo;
            string _keyValue = wnv.keyValue;
            tTable _tTable = wnv.tTable;
                                    
            HtmlElement htmlTable = wb.Document.GetElementById(idName);
            if (htmlTable == null) return;
            HtmlElementCollection htmlRows = htmlTable.GetElementsByTagName("tr");
            int rowCount = htmlRows.Count;
            int colCount = 0;
            
            string dtyValue = "";

            bool onay = false;
            for (int i = 1; i < rowCount; i++)
            {
                HtmlElement hRow = htmlRows[i];
                HtmlElementCollection htmlCols = hRow.GetElementsByTagName("td");
                colCount = htmlCols.Count;

                onay = false;
                for (int i2 = 0; i2 < colCount; i2++)
                {
                    // eşleştirme yapacağımız kolon ise
                    if (_keyColumnNo == i2)
                    {
                        HtmlElement hCol = htmlCols[i2];

                        dtyValue = hCol.InnerText.Trim();

                        // anahtar değer dogru satırda ise ( TcNo == "xxx")
                        if (_keyValue == dtyValue)
                        {
                            postColumsValue_(wb, htmlCols, _tTable);
                            onay = true;
                            break;
                        }
                    }
                }
                if (onay) break;
            }
        }

        /// onaylanmış html satırı ise db Table den okunan value html columns value ye atanır
        private void postColumsValue_(WebBrowser wb, HtmlElementCollection htmlCols, tTable _tTable)
        {
            Int16 _colNo = 0;
            string _dbValue = "";
            
            string dtyHtml = "";
            //string dtyValue = "";
            string dtyIdName = "";
            string dtyType = "";

            int colCount = htmlCols.Count;
            
            foreach (tRow _tRow in _tTable.tRows)
            {
                // database den gelen veriler
                _colNo = t.myInt16(_tRow.tColumns[0].value.ToString());
                _dbValue = _tRow.tColumns[2].value;

                // html üzerindeki tablonun colonları
                for (int i2 = 0; i2 < colCount; i2++)
                {
                    if (_colNo == i2)
                    {
                        HtmlElement hCol = htmlCols[i2];

                        //<input name="dgListele$ctl04$ab" id="dgListele_ctl04_chkOnayBekliyor" type="radio" checked="checked" value="chkOnayBekliyor">
                        //<input name="dgListele$ctl04$ab" id="dgListele_ctl04_chkOnayDurum" type="radio" value="chkOnayDurum">

                        dtyHtml = hCol.InnerHtml;
                        
                        if (dtyHtml.IndexOf("type=\"radio\"") > -1)
                        {
                            //dtyValue = hCol.Children[0].GetAttribute("value");
                            dtyIdName = hCol.Children[0].GetAttribute("id");
                            dtyType = hCol.Children[0].GetAttribute("type");

                            if ((dtyType == "radio") && (_dbValue == "True"))
                            {
                                wb.Document.GetElementById(dtyIdName).InvokeMember("click");
                                break;
                            }
                        }

                        if (dtyHtml.ToLower().IndexOf("selectedindex=") > -1)
                        {
                            //writeValue = writeValue.Replace("selectedindex=", "");
                            hCol.SetAttribute("selectedindex", _dbValue);
                            v.SQL = v.SQL + v.ENTER + myNokta + " set selectedindex : " + _dbValue;
                            break;
                        }

                        if (dtyHtml.IndexOf("select name=") > -1)
                        {
                            /* table içinde combo 
                             * 
                            <select name="dgListele$ctl04$cmbAracPlaka" class="frminput" id="dgListele_ctl04_cmbAracPlaka" style="color: black;">
                               <option value="-1"></option>
                               <option value="20ABJ177">20ABJ177</option>
                               <option value="20ABJ334">20ABJ334</option>
                               <option value="20BT473">20BT473</option>
                            </select>
                            */

                            // yemiyor
                            //hCol.SetAttribute("option value", _dbValue);

                            dtyIdName = hCol.Children[0].GetAttribute("id");
                            if (dtyIdName != "")
                            {
                                wb.Document.GetElementById(dtyIdName).SetAttribute("value", _dbValue);
                                v.SQL = v.SQL + v.ENTER + myNokta + " set value : " + _dbValue;
                                break;
                            }
                        }

                        if (dtyHtml.IndexOf("type=\"radio\"") == -1)
                        {
                            hCol.SetAttribute("value", _dbValue);
                            v.SQL = v.SQL + v.ENTER + myNokta + " set value : " + _dbValue;
                            break;
                        }
                    }
                }
            }
        }
        
        private void selectItemsRead(WebBrowser wb, ref webNodeValue wnv, string idName)
        {
            HtmlElement node = wb.Document.GetElementById(idName);

            if (node == null) return;

            string value = "";
            string text = "";

            tTable table = new tTable();
            
            foreach (HtmlElement item in node.Children)
            {
                tRow row = new tRow();
                value = item.GetAttribute("value");
                text = item.InnerText;

                tColumn column1 = new tColumn();
                column1.value = value;
                tColumn column2 = new tColumn();
                column2.value = text;

                row.tColumns.Add(column1);
                row.tColumns.Add(column2);

                table.tRows.Add(row);
            }

            wnv.tTable = table;
        }

        private string selectItemsGetValue(WebBrowser wb, ref webNodeValue wnv, string idName, string findText)
        {
            HtmlElement node = wb.Document.GetElementById(idName);

            if (node == null) return "";

            string value = "";
            string text = "";
            
            foreach (HtmlElement item in node.Children)
            {
                value = item.GetAttribute("value");
                text = item.InnerText;

                // tarih formatı ise 
                // mebbiste tarih ayırımı gg/aa/yyyy şeklinde
                // bizim database de ise  gg.aa.yyyy şeklindedir
                if (text != null)
                {
                    if ((text.Substring(2, 1) == "/") &&
                        (text.Substring(5, 1) == "/"))
                        text = text.Replace("/", ".");

                    if (findText == text)
                        break;
                }
            }

            return value;
        }

        private void dN_AnalysisNodes_PositionChanged(object sender, EventArgs e)
        {
            if (btn_AnalysisView != null)
            {
                if (((DevExpress.XtraEditors.CheckButton)btn_AnalysisView).Checked)
                {
                    htmlView();
                }
            }
        }

        private void htmlView()
        {
            if (ds_AnalysisNodes != null)
            {
                if (dN_AnalysisNodes.Position > -1)
                {
                    string html = ds_AnalysisNodes.Tables[0].Rows[dN_AnalysisNodes.Position]["outerHtml"].ToString();
                    webTest.DocumentText = html;
                }
            }
        }

        

        #endregion subFunctions

        #region webBrowser events

        private void webAnalysis_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            /// bunları gerek olursa açarsın
            ///
            this.htmlDocumentBody = ((WebBrowser)sender).Document.Body.InnerHtml;

            this.aktifUrl = ((WebBrowser)sender).Url.ToString();
            
            Thread.Sleep(100);

            openPageControlAsync(((WebBrowser)sender));

            //this.htmlDocument = ((WebBrowser)sender).Document;
            //this.htmlDocumentStream = ((WebBrowser)sender).DocumentStream;
            //this.htmlDocumentText = ((WebBrowser)sender).DocumentText;

            /*
            tableList = ((WebBrowser)sender).Document.Body.GetElementsByTagName("table");
            inputList = ((WebBrowser)sender).Document.Body.GetElementsByTagName("input");
            selectList = ((WebBrowser)sender).Document.Body.GetElementsByTagName("select");
            buttonList = ((WebBrowser)sender).Document.Body.GetElementsByTagName("button");
            //strongList = ((WebBrowser)sender).Document.Body.GetElementsByTagName("strong");
            spanList = ((WebBrowser)sender).Document.Body.GetElementsByTagName("span");
            */

        }

        private void webMain_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            v.SQL = v.SQL + v.ENTER + myNokta + " webMain_DocumentCompleted : END";

            this.myDocumentCompleted = true;

            this.aktifUrl = ((WebBrowser)sender).Url.ToString();

            /// sayfanın left ve top değerlerini değiştirir
            if (this.aktifUrl == this.talepEdilenUrl2)
            {
                if (this.talepPageLeft != 0 || this.talepPageTop != 0)
                   ((WebBrowser)sender).Document.Body.Style = 
                        "left: " + this.talepPageLeft.ToString() + "px; "
                      + "top: " + this.talepPageTop.ToString()+ "px; "
                      + "position: absolute;";
            }
                        
            openPageControlAsync(((WebBrowser)sender));

            if (((this.myTriggering) ||
                 (this.myTriggeringTable)
                 ) &&
                (this.selectedTablePageNo == 0))
            {
                if (
                    //(this.myTriggerItemButton) ||
                    //(this.myTriggeringItemButton) ||
                    (this.myTriggeringTable) ||
                    (this.myTriggerInvoke) ||
                    (this.myTriggerPageRefresh) ||
                    (this.myTriggerOneByOne)
                    )
                {
                    Application.DoEvents();
                    if (this.timerTrigger.Enabled == false)
                    {
                        this.timerTrigger.Enabled = true;
                        v.SQL = v.SQL + v.ENTER + myNokta + " webMain_DocumentCompleted : for start : timerTrigger.Enabled = true";
                    }
                }
            }

            if ((this.myDisplayNoneCount > 0) &&
                (this.myTriggering == false) &&
                (this.talepEdilenUrl == ""))
            {
                Application.DoEvents();
                timerTrigger.Interval = 100;
                this.timerTrigger.Enabled = true;
                v.SQL = v.SQL + v.ENTER + myNokta + " webMain_DocumentCompleted : for strat : myDisplayNoneCount > 0 :timerTrigger.Enabled = true";
            }

            if ((this.timerTrigger.Enabled) && (v.IsWaitOpen = false))
            {
                t.WaitFormOpen(v.mainForm, "");
                v.IsWaitOpen = true;
            }

            if (this.myTriggerPageRefresh)
            {
                // table pages varsa onu clickle 
                if ((this.selectedTablePageNo > 0) &&
                    (this.tablePageUrl == this.aktifUrl))
                {
                    //MessageBox.Show("selectedpage");

                    // bu kopya daha sonra selectTablePage için kulllanılıyor
                    HtmlElement htmlTable = ((WebBrowser)sender).Document.GetElementById(this.tablePageName);
                    this.htmlTablePages = htmlTable.GetElementsByTagName("table");

                    selectTablePage((WebBrowser)sender);

                    Application.DoEvents();
                    if (this.timerTrigger.Enabled == false)
                    {
                        this.timerTrigger.Enabled = true;
                        v.SQL = v.SQL + v.ENTER + myNokta + " webMain_DocumentCompleted, selectTablePage : for start : timerTrigger.Enabled = true";
                    }
                }
            }
        }

        public bool WebReadyComplate(WebBrowser wb)
        {
            while (wb.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
            return true;
        }

        private async Task openPageControlAsync(WebBrowser wb)
        {
            if (t.IsNotNull(this.talepEdilenUrl) == false) 
                return;

            bool onay = false;

            if (this.talepEdilenUrl != this.aktifUrl)
            {
                /// aktif url Login page mi kontrol et
                ///

                /// bu atamalar geçici f class bu forma uygulayınca bunu atamaları sil
                f.aktifUrl = this.aktifUrl;
                f.loginPageUrl = this.loginPageUrl;
                f.errorPageUrl = this.errorPageUrl;

                onay = msPagesService.readLoginPageControl(ref ds_LoginPageNodes, f);// this.aktifUrl, ref this.loginPageUrl, ref this.errorPageUrl);

                this.aktifUrl = f.aktifUrl;
                this.loginPageUrl = f.loginPageUrl;
                this.errorPageUrl= f.errorPageUrl;

                if (onay)
                {
                    await runLoginPage(wb);
                    return;
                }
                else 
                {
                    if (this.aktifUrl.IndexOf("oturumsonu") > -1)
                    {
                        return;
                    }
                    /// aktif url Login page değil ise nedir ?
                    /// 
                    /// talep öncesi başka bir url çağrısı var ise
                    ///
                    if (t.IsNotNull(this.talepOncesiUrl))
                    {
                        /// aktif url ne talep edilen ne de talep öncesi url değil ise 
                        /// talepOncesiUrl yi çağır
                        ///
                        if ((this.talepOncesiUrl != this.aktifUrl) &&
                            (this.talepEdilenUrl != this.aktifUrl))
                            loadPage(wb, this.talepOncesiUrl);

                        /// talep öncesi geldiyse sıra esas talep edilen url ye sıra geldi
                        ///
                        if (this.talepOncesiUrl == this.aktifUrl)
                        {
                            loadPage(wb, this.talepEdilenUrl);

                            if (t.IsNotNull(this.aktifPageCode))
                            {
                                ds_WebNodeItemsList = null;
                                ds_WebNodeItemsList = msPagesService.readNodeItemsOld(this.aktifPageCode);
                            }
                        }
                    }
                    else
                    {
                        /// talepOncesiUrl yok ise şimdi talepEdilenUrl yi çağıralım 
                        ///
                        loadPage(wb, this.talepEdilenUrl);
                    }
                }
            }

            if (this.talepEdilenUrl == this.aktifUrl)
            {
                this.htmlDocumentBody = wb.Document.Body.InnerHtml;

                //
                if (this.myLoadNodeCount > 0)
                {
                    await runLoadAsync();
                    this.talepEdilenUrl = "";
                }
                
                if (this.myLoadNodeCount == 0) 
                {
                    this.talepEdilenUrl = "";
                }
            }
        }

        private async Task runLoginPage(WebBrowser wb)
        {
            if (this.errorPageUrl == this.aktifUrl)
            {
                loadPage(wb, this.loginPageUrl);
            }
            else if (this.aktifUrl.IndexOf("oturumsonu") > -1)
            {
                return;
            }
            else
            {
                /// login işlemlerini gerçekleştir
                /// 
                await runScrapingAsync(this.ds_LoginPageNodes, v.tWebRequestType.post, v.tWebEventsType.none, 0, 0);

                t.WebReadyComplate(wb);
            }
        }

        private async Task runLoadAsync()
        {
            /// istenen sayfaya açıldı, açılışta çalışması istenen komutlar varsa çalışsın artık

            v.SQL = v.SQL + v.ENTER2 + myNokta + " runLoad : ";

            this.myTriggering = true;

            this.loadBeforeWebRequestType = this.myTriggerWebRequestType;
            this.loadBeforeEventsType = this.myTriggerEventsType;

            startNodesRun(ds_MsWebNodes, v.tWebRequestType.none, v.tWebEventsType.load);

            
            Thread.Sleep(100);
        }

        private async Task runDisplayNoneAsync()
        {
            /// gizlemek istenen nodeler varsa sırayla gizler

            v.SQL = v.SQL + v.ENTER + myNokta + " runDisplayNone : ";

            startNodesBefore();

            this.myTriggering = true;
            //this.myTriggerEventsType = v.tWebEventsType.displayNone;
            startNodesRun(ds_MsWebNodes, v.tWebRequestType.none, v.tWebEventsType.displayNone);
            
            Thread.Sleep(100);
        }

        #endregion webBrowser events

        #region read db tables
        
        private void readTriggerNodes(ref DataSet ds, string pageCode, v.tWebEventsType eventsType)
        {
            /* burayada gerek kalmadı
             * 
             'WEBNODE_EVENTS_TYPE',  0, 'none'
             'WEBNODE_EVENTS_TYPE',  1, 'Load'
             'WEBNODE_EVENTS_TYPE',  2, 'DisplayNone'
             'WEBNODE_EVENTS_TYPE',  3, 'PageRefresh'
             'WEBNODE_EVENTS_TYPE', 141, 'Button3',2,3,4,5 
            
             'WEBNODE_ATT_ROLE', 11, 'ItemTable'
             'WEBNODE_ATT_ROLE', 12, 'ItemButton'
             */

            if (ds != null)
            {
                if (ds.Namespace == pageCode)
                    return;
            }

            ds = null;
            ds = new DataSet();

            string tSql =
                @" Select * From [dbo].[MsWebNodes]
            Where IsActive = 1 
            and PageCode = '" + pageCode + @"'";
            /*
            if (eventsType == v.tWebEventsType.load)
                tSql += " and EventsType = 1 ";
            else if (eventsType == v.tWebEventsType.displayNone)
                tSql += " and EventsType = 2 ";
            else if (eventsType == v.tWebEventsType.pageRefresh)
                tSql += " and (EventsType > 10 or EventsType = 0 or EventsType = 1) ";
            else
                tSql += " and (EventsType > 10 or EventsType = 0) ";
            */
            tSql += "order by LineNumber, NodeId ";

            t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "PageNodes", "PageNodes");

            //eventsTypeCount(ds);

            ds.Namespace = pageCode;
        }
        private void eventsTypeCount(DataSet ds)
        {
            /// page ait nodelerin tespiti
            /// 
            v.tWebEventsType dbEventsType = v.tWebEventsType.none;
            this.myNodeCount = 0; // tüm nodelerin sayısı
            this.myLoadNodeCount = 0;
            this.myDisplayNoneCount = 0;
            this.myPageRefreshCount = 0;
            this.myButton3Count = 0;
            this.myButton4Count = 0;
            this.myButton5Count = 0;
            this.myButton6Count = 0;
            this.myButton7Count = 0;
            this.myTabelFieldCount = 0;
            this.myNoneCount = 0;

            if (t.IsNotNull(ds) == false) return;

            string pageRefresh = "";
            string isActive = "";
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                dbEventsType = (v.tWebEventsType)t.myInt16(row["EventsType"].ToString());
                pageRefresh = row["PageRefresh"].ToString();
                isActive = row["IsActive"].ToString();

                if (isActive == "True")
                {
                    this.myNodeCount++;
                    if (dbEventsType == v.tWebEventsType.load) this.myLoadNodeCount++;
                    if (dbEventsType == v.tWebEventsType.displayNone) this.myDisplayNoneCount++;
                    if (dbEventsType == v.tWebEventsType.button3) this.myButton3Count++;
                    if (dbEventsType == v.tWebEventsType.button4) this.myButton4Count++;
                    if (dbEventsType == v.tWebEventsType.button5) this.myButton5Count++;
                    if (dbEventsType == v.tWebEventsType.button6) this.myButton6Count++;
                    if (dbEventsType == v.tWebEventsType.button7) this.myButton7Count++;
                    if (dbEventsType == v.tWebEventsType.tableField) this.myTabelFieldCount++;
                    if (dbEventsType == v.tWebEventsType.none) this.myNoneCount++;
                    if (pageRefresh == "True") this.myPageRefreshCount++;
                }
            }

        }
        
        #endregion read db tables

        //-------------------------------------------------------------------------------------

        private System.Windows.Forms.HtmlDocument preparingHtmlDocument(string htmlCode)
        {
            /// burası olmadı
            WebBrowser wb = new WebBrowser();
            wb.Navigate(new Uri("about:blank"));
            System.Windows.Forms.HtmlDocument doc = wb.Document;
            HtmlElement tableElem = doc.CreateElement(htmlCode);
            doc.Body.AppendChild(tableElem);
            return doc;
        }
        
        private static async void GetHtmlAsync(string url)
        {
            // bir listeyi okumak için
            HttpClient httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);

            var productsHtml = htmlDocument.DocumentNode.Descendants("ul")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("ListViewInner")).ToList();

            var productListItems = productsHtml[0].Descendants("li")
                .Where(node => node.GetAttributeValue("id", "")
                .Contains("item")).ToList();

            foreach (var producListItem in productListItems)
            {

            } 

        }

    }
    
    //https://mebbis.meb.gov.tr/oturumsonuyaz.html
}
