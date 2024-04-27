using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using DevExpress.XtraEditors;

using Tkn_CookieReader;
using Tkn_Events;
using Tkn_Save;
using Tkn_ToolBox;
using Tkn_Variable;

using YesiLdefter.Selenium.Helpers;
using YesiLdefter.Selenium;
using YesiLdefter.Entities;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

using Tesseract;
using System.Net;


namespace YesiLdefter
{
    public partial class ms_Selenium : Form
    {
        #region Tanımlar
        tToolBox t = new tToolBox();

        //IWebDriver webDriver;
        MsWebPagesService msPagesService = new MsWebPagesService();

        /// Scraping
        DataSet ds_MsWebPages = null;
        DataNavigator dN_MsWebPages = null;
        DataSet ds_MsWebNodes = null;
        DataNavigator dN_MsWebNodes = null;
        // sadece login için kullanılan dataset
        DataSet ds_LoginPageNodes = null;
        // web sayfalarının adını gösteren viewControl
        Control view_MsWebPages = null;

        // User butonları
        Control btn_PageView = null;
        //Control btn_PageViewAnalysis = null;
        Control btn_AlwaysSet = null;
        Control btn_FullGet1 = null;  // birinci get butonu
        Control btn_FullGet2 = null;  // ikinci  get butonu
        Control btn_FullPost1 = null; // birinci post butonu
        Control btn_FullPost2 = null; // ikinci  post butonu
        Control btn_FullSave = null;  // save    post butonu
        Control btn_AutoSubmit = null;// auto    kaydet butonu 
        Control btn_SiraliIslem = null;

        // Test butonları
        Control btn_LineGet = null;   // test get
        Control btn_LinePost = null;  // test set
        Control btn_FullGetTest1 = null;   // test get butonu
        Control btn_FullGetTest2 = null;   // test get butonu
        Control btn_FullPostTest1 = null;  // test set butonu
        Control btn_FullPostTest2 = null;  // test set butonu

        string menuName = "MENU_" + "UST/PMS/PMS/SeleniumMebbis";
        string buttonMebbisGiris = "ButtonMebbisGiris";
        string buttonAutoSave = "ButtonOtomatikMebbisKaydet";
        string buttonManuelSave = "ButtonManuelMebbisKaydet";
        DevExpress.XtraBars.Navigation.NavElement buttonManuelSaveControl = null;
        DevExpress.XtraBars.Navigation.NavElement buttonAutoSaveControl = null;


        DevExpress.XtraBars.Navigation.NavButton navButtonMebbisKaydet = null;

        string TableIPCode = string.Empty;

        //IWebDriver webDriver_ = null;

        List<MsWebPage> msWebPage_ = null;
        List<MsWebPage> msWebPages_ = null;
        
        List<MsWebNode> msWebNodes_ = null;
        List<MsWebNode> msWebLoginNodes_ = null;

        List<MsWebScrapingDbFields> msWebScrapingDbFields_ = null;
        List<webNodeItemsList> aktifPageNodeItemsList_ = null;

        webWorkPageNodes workPageNodes_ = new webWorkPageNodes();
        webForm f = new webForm();

        #endregion

        public ms_Selenium()
        {
            InitializeComponent();

            tEventsForm evf = new tEventsForm();

            this.Load += new System.EventHandler(evf.myForm_Load);
            this.Shown += new System.EventHandler(evf.myForm_Shown);
            this.Shown += new System.EventHandler(this.ms_SeleniumAnalysis_Shown);

            this.KeyPreview = true;
        }
        #region Form preparing
        private void ms_SeleniumAnalysis_Shown(object sender, EventArgs e)
        {
            MsWebNodesButtonsPreparing();
            MsWebPagesButtonsPreparing();
            preparingWebPagesViewControl();
            preparingMsWebLoginPage();


            // TileNavMenu buttons
            t.Find_Button_AddClick(this, menuName, buttonMebbisGiris, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonManuelSave, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonAutoSave, myNavElementClick);
            t.Find_NavButton_Control(this, menuName, buttonManuelSave, ref buttonManuelSaveControl);
            t.Find_NavButton_Control(this, menuName, buttonAutoSave, ref buttonAutoSaveControl);
            if (buttonAutoSaveControl != null)
                myNavElementClick(buttonAutoSaveControl, null);

            // scraping ilişkisi olan TableIPCode ve ilgili fieldler
            // 
            this.msWebScrapingDbFields_ = msPagesService.readScrapingTablesAndFields(this.msWebPages_);
            //msPagesService.checkedSiraliIslemVarmi(this, this.workPageNodes_, this.msWebScrapingDbFields_);

            /// Kullanıcının mebbisCode ve şifresini yeniden oku
            /// değiştirmiş olabilir
            /// 
            msPagesService.getMebbisCode();
            //MessageBox.Show("Mebbis : " + v.tUser.MebbisCode + " : " + v.tUser.MebbisPass);

            /// DataSet ve DataNavigatorleri işaretle
            /// 
            preparingDataSets();
        }

        private void preparingDataSets()
        {
            #region DataNavigator Listesi Hazırlanıyor

            List<string> list = new List<string>();
            t.Find_DataNavigator_List(this, ref list);

            Control cntrl = new Control();
            string[] controls = new string[] { "DevExpress.XtraEditors.DataNavigator" };

            foreach (string value in list)
            {
                cntrl = t.Find_Control(this, value, "", controls);
                if (cntrl != null)
                {
                    ((DevExpress.XtraEditors.DataNavigator)cntrl).PositionChanged += new System.EventHandler(dataNavigator_PositionChanged);
                } // if cntrl != null
            }//foreach

            #endregion DataNavigator Listesi
        }
        private void preparingWebMain()
        {
            /*
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            webMain = new ChromeDriver(chromeDriverService, new ChromeOptions());
            //webMain.Manage().Window.Maximize();
            webMain.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(SeleniumHelper.ImplicitlyWait);
            webMain.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(SeleniumHelper.PageLoadTimeout);
            */

            SeleniumHelper.ResetDriver();
            v.webDriver_ = SeleniumHelper.WebDriver;

            preparingWebDriverChangeSize(v.webDriver_);
        }
        private void preparingWebDriverChangeSize(IWebDriver wb)
        {
            /// Main Form size change
            /// 
            Form mainF = Application.OpenForms[0];

            Screen screen = Screen.FromControl(mainF); // formun bulunduğu ekranı al
            //string text = screen.DeviceName; // ekranın adını etikete yaz
            bool primary = screen.Primary; // Birinci ekran mı kontrol et 

            /// Birinci ekranda ise
            /// 
            if (primary)
            {
                mainF.WindowState = FormWindowState.Normal;
                mainF.Left = 0;
                mainF.Top = 0;
                mainF.Size = new Size(v.Primary_Screen_Width / 2, v.Primary_Screen_Height);

                wb.Manage().Window.Size = new Size(v.Primary_Screen_Width / 2, v.Primary_Screen_Height);
                wb.Manage().Window.Position = new Point(v.Primary_Screen_Width / 2, 0);
            }
            else
            {
                /// - 140 browser nedense tam Y koordinatına gelmiyor, -140 ile yaklaştırıyorum
                wb.Manage().Window.Position = new Point(screen.Bounds.X / 2, screen.Bounds.Y);
                wb.Manage().Window.Size = new Size(screen.Bounds.Width / 2, screen.Bounds.Height - 50);

                /// Başka bir ekranda ise
                /// 
                mainF.WindowState = FormWindowState.Normal;
                mainF.Left = screen.Bounds.X;
                mainF.Top = screen.Bounds.Y;
                mainF.Size = new Size(screen.Bounds.Width / 2, screen.Bounds.Height - 50);
            }

            //Screen[] screens = Screen.AllScreens; // tüm ekranları al
            //if (screens.Length > 1) // birden fazla ekran varsa
            //{
            //    Screen secondScreen = screens[1]; // ikinci ekranı al
            //    this.Location = secondScreen.WorkingArea.Location; // formun konumunu ikinci ekranın çalışma alanının konumuna ayarla
            //}


            /// Main Form size change
            /// 
            //Application.OpenForms[0].WindowState = FormWindowState.Normal;
            //Application.OpenForms[0].Left = 0;
            //Application.OpenForms[0].Top = 0;
            //Application.OpenForms[0].Size = new Size(v.Primary_Screen_Width / 2, v.Primary_Screen_Height);

            /// Form boyutunu değiştirmek için,
            /// Bu özellik, formun genişliğini ve yüksekliğini belirten bir System.Drawing.Size nesnesi alır
            /// driver.Manage().Window.Size = new System.Drawing.Size(800, 600);
            /// 
            //wb.Manage().Window.Size = new Size(v.Primary_Screen_Width / 2, v.Primary_Screen_Height);

            /// Form konumunu değiştirmek için, 
            /// Bu özellik, formun ekranın sol üst köşesine göre x ve y koordinatlarını belirten bir System.Drawing.Point nesnesi alır
            /// driver.Manage().Window.Position = new System.Drawing.Point(0, 0);
            /// 
            //wb.Manage().Window.Position = new Point(v.Primary_Screen_Width / 2, 0);

            /// Formu tam ekran yapmak için, 
            /// Bu metot, formu mevcut ekran çözünürlüğüne göre en büyük boyuta getirir
            /// driver.Manage().Window.Maximize();
            /// 
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

                /// simpleButton_ek1 :  line get  / pageView
                /// simpleButton_ek2 :  line post / AlwaysSet Tc sorgula gibi
                /// simpleButton_ek3 :  full get1  : v.tWebEventsType.button1
                /// simpleButton_ek4 :  full get2  : v.tWebEventsType.button2
                /// simpleButton_ek5 :  full post1 : v.tWebEventsType.button3
                /// simpleButton_ek6 :  full post2 : v.tWebEventsType.button4
                /// simpleButton_ek7 :  full save  : v.tWebEventsType.button5

                btn_PageView = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);
                // Page View
                if (btn_PageView != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_PageView).Click += new System.EventHandler(myPageViewClick);
                }
                #region
                //
                // Bilgileri Sorgula / AlwaysSet  (TcNo sorgula gibi) // Bu henüz hiç kullanılmadı 
                btn_AlwaysSet = t.Find_Control(this, "simpleButton_ek2", TableIPCode, controls);
                if (btn_AlwaysSet != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_AlwaysSet).Click += new System.EventHandler(myAlwaysSetClick);
                }
                // Bilgileri Al 1
                btn_FullGet1 = t.Find_Control(this, "simpleButton_ek3", TableIPCode, controls);
                if (btn_FullGet1 != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_FullGet1).Click += new System.EventHandler(myFullGet1Click);
                }
                // Bilgileri Al 2
                btn_FullGet2 = t.Find_Control(this, "simpleButton_ek4", TableIPCode, controls);
                if (btn_FullGet2 != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_FullGet2).Click += new System.EventHandler(myFullGet2Click);
                }
                // Bilgileri Gönder 1
                btn_FullPost1 = t.Find_Control(this, "simpleButton_ek5", TableIPCode, controls);
                if (btn_FullPost1 != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_FullPost1).Click += new System.EventHandler(myFullPost1Click);
                }
                // Bilgileri Gönder 2
                btn_FullPost2 = t.Find_Control(this, "simpleButton_ek6", TableIPCode, controls);
                if (btn_FullPost2 != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_FullPost2).Click += new System.EventHandler(myFullPost2Click);
                }
                // Bilgileri Kaydet
                btn_FullSave = t.Find_Control(this, "simpleButton_ek7", TableIPCode, controls);
                if (btn_FullSave != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_FullSave).Click += new System.EventHandler(myFullSaveClick);
                }
                // Otomatik kaydet için
                btn_AutoSubmit = t.Find_Control(this, "checkButton_ek1", TableIPCode, controls);
                if (btn_AutoSubmit != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)btn_AutoSubmit).Click += new System.EventHandler(myAutoSubmit);
                }
                #endregion
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
            
            //Control cntrl = null;
            string[] controls = new string[] { };

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

            btn_FullGetTest1 = t.Find_Control(this, "simpleButton_ek3", TableIPCode, controls);
            if (btn_FullGetTest1 != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)btn_FullGetTest1).Click += new System.EventHandler(myFullGet1Click);
            }
            btn_FullGetTest2 = t.Find_Control(this, "simpleButton_ek4", TableIPCode, controls);
            if (btn_FullGetTest2 != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)btn_FullGetTest2).Click += new System.EventHandler(myFullGet2Click);
            }

            btn_FullPostTest1 = t.Find_Control(this, "simpleButton_ek5", TableIPCode, controls);
            if (btn_FullPostTest1 != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)btn_FullPostTest1).Click += new System.EventHandler(myFullPost1Click);
            }
            btn_FullPostTest2 = t.Find_Control(this, "simpleButton_ek6", TableIPCode, controls);
            if (btn_FullPostTest2 != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)btn_FullPostTest2).Click += new System.EventHandler(myFullPost2Click);
            }
            
        }
        private void preparingWebPagesViewControl()
        {
            TableIPCode = t.Find_TableIPCode(this, "MsWebPages");
            view_MsWebPages = t.Find_Control_View(this, TableIPCode);
            if (view_MsWebPages != null)
            {
                string type = view_MsWebPages.GetType().ToString();
                if (type == "DevExpress.XtraGrid.GridControl")
                {
                    DevExpress.XtraGrid.GridControl gridControl = (DevExpress.XtraGrid.GridControl)view_MsWebPages; 

                    string viewName = gridControl.MainView.GetType().ToString();
                    
                    if (viewName == "DevExpress.XtraGrid.Views.Tile.TileView")
                    {
                        DevExpress.XtraGrid.Views.Tile.TileView tTileView = (DevExpress.XtraGrid.Views.Tile.TileView)gridControl.MainView;
                        tTileView.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(myTileView_ItemClick);
                    }
                }
            }
        }
        private async void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonMebbisGiris)
                {
                    await loginPageViev();
                }
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonManuelSave)
                {
                    f.autoSubmit = false;
                    buttonManuelSaveControl.Appearance.BackColor = v.colorAutoSave;
                    buttonManuelSaveControl.Appearance.ForeColor = v.AppearanceTextColor;

                    buttonAutoSaveControl.Appearance.BackColor = ((DevExpress.XtraBars.Navigation.NavButton)sender).Appearance.BackColor2;
                    buttonAutoSaveControl.Appearance.ForeColor = ((DevExpress.XtraBars.Navigation.NavButton)sender).AppearanceSelected.ForeColor;
                }
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonAutoSave)
                {
                    f.autoSubmit = true;
                    buttonManuelSaveControl.Appearance.BackColor = ((DevExpress.XtraBars.Navigation.NavButton)sender).Appearance.BackColor2;
                    buttonManuelSaveControl.Appearance.ForeColor = ((DevExpress.XtraBars.Navigation.NavButton)sender).AppearanceSelected.ForeColor;

                    buttonAutoSaveControl.Appearance.BackColor = v.colorAutoSave;
                    buttonAutoSaveControl.Appearance.ForeColor = v.AppearanceTextColor;
                }

                btn_FullSave.Visible = !f.autoSubmit;
            }

            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                //if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonInsertPaketOlustur) InsertPaketOlustur();
            }
        }
        public async void myTileView_ItemClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
            
            if (t.IsNotNull(ds_MsWebPages) == false) return;
            if (dN_MsWebPages.Position == -1) return;

            string soru = "Mebbis Giriş sayfasını açmak ister misiniz ?";
            //if (v.webDriver_ != null) soru = "Yeniden Mebbis Giriş sayfasını"

            string loginPage = ds_MsWebPages.Tables[0].Rows[dN_MsWebPages.Position]["LoginPage"].ToString();

            if (loginPage == "True")
            {
                DialogResult cevap = t.mySoru(soru);
                if (DialogResult.Yes == cevap)
                {
                    await loginPageViev();

                    //try
                    //{
                    //    await loginPageViev();
                    //}
                    //catch (Exception ex)
                    //{
                    //    //İstek gönderilirken bir hata oluştu
                    //    MessageBox.Show("İstek gönderilirken bir hata oluştu" + v.ENTER + ex.Message.ToString());
                    //    //throw;
                    //}

                }
            }
        }
        
        #region Test buttons click
        private async void myLineGetTestClick(object sender, EventArgs e)
        {
            if (ds_MsWebNodes != null)
            {
                webNodeValue wnv = new webNodeValue();
                wnv.workRequestType = v.tWebRequestType.get;

                DataRow row = ds_MsWebNodes.Tables[0].Rows[dN_MsWebNodes.Position];

                msPagesService.nodeValuesPreparing(row, ref wnv, f.aktifPageCode);
                                
                // node nin items okunacak (getNodeItems)
                // sonrada MsWebNodeItems tablosuna yazılacak  
                if (wnv.TagName == "select")
                    wnv.workRequestType = v.tWebRequestType.getNodeItems;

                await WebScrapingAsync(v.webDriver_, wnv);

                if (wnv.TagName == "select")
                {
                    /// select node ye ait (value, text) listesinide MsWebNodeItems tablosuna yaz
                    //transferFromWebSelectToDatabase(wnv);
                    //MessageBox.Show("İşlem tamamlandı...");
                }
            }
        }
        private async void myLinePostTestClick(object sender, EventArgs e)
        {
            if (ds_MsWebNodes != null)
            {
                webNodeValue wnv = new webNodeValue();
                wnv.workRequestType = v.tWebRequestType.post;

                DataRow row = ds_MsWebNodes.Tables[0].Rows[dN_MsWebNodes.Position];

                msPagesService.nodeValuesPreparing(row, ref wnv, f.aktifPageCode);


                // node nin items okunacak (getNodeItems)
                // sonrada MsWebNodeItems tablosuna yazılacak  
                //if (wnv.TagName == "select")
                //    wnv.workRequestType = v.tWebRequestType.getNodeItems;

                await WebScrapingAsync(v.webDriver_, wnv);

                //if (wnv.TagName == "select")
                //{
                /// select node ye ait (value, text) listesinide MsWebNodeItems tablosuna yaz
                //transferFromWebSelectToDatabase(wnv);
                //MessageBox.Show("İşlem tamamlandı...");
                //}
            }
        }

        private void myFullGetTest1Click(object sender, EventArgs e)
        {

        }
        private void myFullGetTest2Click(object sender, EventArgs e)
        {

        }
        private void myFullPostTest1Click(object sender, EventArgs e)
        {

        }
        private void myFullPostTest2Click(object sender, EventArgs e)
        {

        }
        
        #endregion Test buttons click

        private void dNScrapingPages_PositionChanged(object sender, EventArgs e)
        {
            preparingMsWebNodesFields();
            preparingAktifPageLoad();

            if (f.autoSubmit)
            {
                if ((this.workPageNodes_.aktifPageCode != "MTSKADAYRESIM") &&
                    (this.workPageNodes_.aktifPageCode != "MTSKADAYSOZLESME") &&
                    (this.workPageNodes_.aktifPageCode != "MTSKADAYIMZA"))
                    btn_FullPost1.Visible = true;
                else btn_FullPost1.Visible = false;

                if ((this.workPageNodes_.aktifPageCode == "MTSKADAYRESIM") ||
                    (this.workPageNodes_.aktifPageCode == "MTSKADAYSOZLESME") ||
                    (this.workPageNodes_.aktifPageCode == "MTSKADAYIMZA"))
                    btn_FullPost2.Visible = true;
                else btn_FullPost2.Visible = false;
            }
            else
            {
                if (btn_FullPost1 != null) btn_FullPost1.Visible = true;
                if (btn_FullPost2 != null) btn_FullPost2.Visible = true;
                if (btn_FullSave != null) btn_FullSave.Visible = true;
            }
        }
        private async void dataNavigator_PositionChanged(object sender, EventArgs e)
        {
            ///
            if (f.tableIPCodesInLoad != "")
            {
                object tDataTable = ((DevExpress.XtraEditors.DataNavigator)sender).DataSource;
                DataSet dsData = ((DataTable)tDataTable).DataSet;
                string tableIPCode_ = "";
                if (dsData.DataSetName != null)
                    tableIPCode_ = dsData.DataSetName.ToString();
                
                if ((f.tableIPCodesInLoad.IndexOf(tableIPCode_) > -1) && 
                    (f.tableIPCodesInLoad.IndexOf(f.tableIPCodeIsSave) <= 0 )) // save olan Dataset ise atla
                {
                    t.WaitFormOpen(this, "Sayfa değiştiriliyor ...");
                    f.loadWorking = false;
                    f.pageRefreshWorking = true;
                    await startNodes(this.msWebNodes_, this.workPageNodes_, v.tWebRequestType.post, v.tWebEventsType.load);
                    f.loadWorking = true;
                    v.IsWaitOpen = false;
                    t.WaitFormClose();
                }
            }
        }
        private async void preparingAktifPageLoad()
        {
            if (v.webDriver_ != null)
            {
                await myPageViewClickAsync(v.webDriver_, this.msWebPage_);
            }
        }
        private void preparingMsWebPages()
        {
            this.msWebPages_ = t.RunQueryModels<MsWebPage>(ds_MsWebPages);
        }
        private void preparingMsWebNodesFields()
        {
            this.f.Clear();
            this.msWebPage_ = t.RunQueryModelsSingle<MsWebPage>(ds_MsWebPages, dN_MsWebPages.Position);
            this.msWebNodes_ = t.RunQueryModels<MsWebNode>(ds_MsWebNodes);
            this.workPageNodes_.Clear();
            this.workPageNodes_.aktifPageCode = this.msWebPage_[0].PageCode;
            this.workPageNodes_.aktifPageUrl = this.msWebPage_[0].PageUrl;
            
            msPagesService.checkedSiraliIslemVarmi(this, this.workPageNodes_, this.msWebScrapingDbFields_);
            //this.btn_SiraliIslem = this.workPageNodes_.siraliIslem_Btn;
        }
        private void preparingMsWebLoginPage()
        {
            bool onay = false;

            if (t.IsNotNull(ds_LoginPageNodes) == false)
            {
                f.aktifUrl = "loginPageYükle";
                onay = msPagesService.readLoginPageControl(ref ds_LoginPageNodes, f);
                f.aktifUrl = "";

                if ((onay) && (this.msWebLoginNodes_ == null))
                    this.msWebLoginNodes_ = t.RunQueryModels<MsWebNode>(ds_LoginPageNodes);
            }
        }

        #endregion Form preparing
        /// 
        /// Userın kullanığı butonlar
        /// 
        #region user buttons
        private async void myPageViewClick(object sender, EventArgs e)
        {
            await loginPageViev();
        }
        private async Task loginPageViev()
        {
            f.Clear();

            if (v.webDriver_ == null)
                preparingWebMain();

            await myPageViewClickAsync(v.webDriver_, this.msWebPage_);
        }
        private void myAlwaysSetClick(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            //startNodesBefore();
            //startNodesRun(ds_MsWebNodes, v.tWebRequestType.alwaysSet, v.tWebEventsType.buttonAlwaysSet);
        }

        private async void myFullGet1Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            await startNodes(this.msWebNodes_, this.workPageNodes_, v.tWebRequestType.get, v.tWebEventsType.button3);
        }

        private async void myFullGet2Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            await startNodes (this.msWebNodes_, this.workPageNodes_, v.tWebRequestType.get, v.tWebEventsType.button4);
        }

        private async void myFullPost1Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            await startNodes(this.msWebNodes_, this.workPageNodes_, v.tWebRequestType.post, v.tWebEventsType.button5);
        }

        private async void myFullPost2Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            await startNodes(this.msWebNodes_, this.workPageNodes_, v.tWebRequestType.post, v.tWebEventsType.button6);
        }

        private async void myFullSaveClick(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            await startNodes(this.msWebNodes_, this.workPageNodes_, v.tWebRequestType.post, v.tWebEventsType.button7);
        }
        private void myAutoSubmit(object sender, EventArgs e)
        {
            //Cursor.Current = Cursors.WaitCursor;
            //startNodesRun(ds_MsWebNodes, v.tWebRequestType.post, v.tWebEventsType.button7);
        }
        #endregion user buttons

        #region Scraping functions               
        private async Task startNodes(List<MsWebNode> msWebNodes, webWorkPageNodes workPageNodes,  v.tWebRequestType workRequestType, v.tWebEventsType workEventsType)
        {
            bool onayList = false;
            bool onayRequest = false;
            bool onayPageRefresh = false;
            bool onayValue = false;
            bool onaySiraliIslem = false;
            /// daha çalışanları temizle yani
            /// load çalıştı diyelim
            /// kullanıcı veri gönder/veri al talebinde bulununca 
            /// loada ait çalışma listesini temizle 
            ///
            workPageNodes.nodeIdList = "";
            /// kullanıcı tekrar istekte bulundu onun için hatayı kapat ve 
            /// save işlemini sıfırla
            /// 
            f.anErrorOccurred = false;
            f.tableIPCodeIsSave = "";
            //f.tableIPCodesInLoad = ""; Açma 

            foreach (MsWebNode item in msWebNodes)
            {
                if (f.anErrorOccurred) break;

                //if ((item.EventsType == (short)v.tWebEventsType.button5) && (workEventsType == v.tWebEventsType.button5))
                //{
                //   // test
                //   //string aa = "aaa555";
                //}
                //if ((item.EventsType == (short)v.tWebEventsType.button6) && (workEventsType == v.tWebEventsType.button6))
                //{
                //    // test
                //    //string aa = "aaa6666";
                //}

                //MessageBox.Show(item.TagName + " ; " + item.AttId + " ; " + item.AttName + " ; ");

                // 2. adımda 
                // IsActive = 1, nodeIdList += item.Id
                // 
                onayList = listControl(item, workPageNodes);

                // 3. adımda
                // v.tWebRequestType workRequestType,
                // v.tWebEventsType  workEventsType,
                // eventsType = (v.tWebEventsType)t.myInt16(row["EventsType"].ToString());
                // injectType = (v.tWebInjectType)t.myInt16(row["InjectType"].ToString());
                // kontrolleri
                onayRequest = requestContol(item, workRequestType, workEventsType);
                                
                // PageRefresh satırımı kontrol ediliyor
                // pageRefresh ise sub functionda çalışsın ve false dönsün
                // true dönerse pageRefresh değil işleme devam et
                onayPageRefresh = await pageRefresh(v.webDriver_, item);

                if ((onayList) && (onayRequest) && (onayPageRefresh))
                {
                    webNodeValue wnv = new webNodeValue();
                    wnv.workRequestType = workRequestType;
                    wnv.workEventsType = workEventsType;
                    wnv.pageCode = workPageNodes.aktifPageCode;
                    msPagesService.nodeValuesPreparing(item, ref wnv);

                    // 4. adım 
                    //
                    // WebScrapingAsync öncesi (set için) yapılacak işler
                    // bazı atamalar olumsuz olabiliyor, örn : resim datası yok ise
                    //
                    onayValue = await WebScrapingBefore(wnv);

                    // OperandControl kriter karşılaştırma var mı?
                    if (onayValue)
                        onayValue = valueChecked(wnv);

                    // 5. adım scraping
                    // WebScrapingAsync
                    //
                    if (onayValue)
                    {
                        await WebScrapingAsync(v.webDriver_, wnv);

                        // 4. adım
                        //
                        // WebScrapingAsync sonrası (get için ) yapılacak işler
                        await WebScrapingAfter(wnv);
                    }
                }
            }

            /// Sıralı islem butonu çalışıcak
            if ((workEventsType == v.tWebEventsType.button5) ||
                (workEventsType == v.tWebEventsType.button6))
            {
                if ((workRequestType == v.tWebRequestType.post) &&
                    (workPageNodes.siraliIslemVar) &&
                    (f.anErrorOccurred == false))
                {
                    /// kullanıcı onayı sıralı işlemi iptal edebilir
                    workPageNodes.siraliIslemAktif = ((DevExpress.XtraEditors.CheckButton)workPageNodes.siraliIslem_Btn).Checked;

                    if (workPageNodes.siraliIslemAktif)
                    {
                        onaySiraliIslem = nextDataSetRow(workPageNodes);
                        if (onaySiraliIslem)
                            await startNodes(msWebNodes, workPageNodes, workRequestType, workEventsType);
                    }
                }
            }
        }
        private bool nextDataSetRow(webWorkPageNodes workPageNodes)
        {
            bool onay = false;
            DataSet ds = workPageNodes.siraliIslem_ds;
            DataNavigator dN = workPageNodes.siraliIslem_dN;
            if (ds != null)
            {
                if (dN.Tag == null) dN.Tag = -1;
                //if ((int)dN.Tag == dN.Position) return false;
                if (ds.Tables[0].Rows.Count == (int)dN.Tag + 1)
                {
                    t.FlyoutMessage(this, "Bilgilendirme", "Sıralı atma işlemi tamamlandı...");
                    return false;
                }
                NavigatorButton btnNext = dN.Buttons.Next;
                dN.Buttons.DoClick(btnNext);
                dN.Tag = dN.Position;
                Application.DoEvents();
                onay = true;
            }
            return onay;
        }
        private async Task<bool> pageRefresh(IWebDriver wb, MsWebNode item)
        {
            bool onay = true;
            if ((item.PageRefresh == true) &&
                (item.IsActive))
            {
                /// PageRefresh isteği şimdilik sadece DataNavigator position değişikliğinde aktif ediliyor
                /// 
                if (f.pageRefreshWorking)
                {
                    f.pageRefreshWorking = false;
                    await myPageViewClickAsync(wb, this.msWebPage_);
                    /// pageRefresh satırı (item) buarda çalıştı, dönüşte tekrar çalışmasın diye false olarak geri dönüyor
                    /// 
                    onay = false;
                }
            }
            return onay; 
        }
        private async Task<bool> WebScrapingBefore(webNodeValue wnv)
        {
            /// database deki veriyi web e aktar için 
            /// db deki veriyi wnv.writeValue üzerine aktar
            /// her işlem için onay ver, 
            /// eğer hatalı bir durum oluşursa onaylama
            /// 
            bool onay = true;

            if (wnv.InjectType == v.tWebInjectType.Set ||
                wnv.InjectType == v.tWebInjectType.AlwaysSet ||
               (wnv.InjectType == v.tWebInjectType.GetAndSet && wnv.workRequestType == v.tWebRequestType.post))
            {
                if (wnv.TagName != "table")
                    onay = msPagesService.transferFromDatabaseToWeb(this, wnv, msWebScrapingDbFields_);

                if (wnv.TagName == "table")
                {
                    /// webe transfer edilecek tablonun field bilgilerini databaseden al
                    if (wnv.tTable == null)
                    {
                //        this.myTriggeringTable = false;
                //        this.myTriggerTableWnv = null;
                //        this.myTriggeringItemButton = false;
                //        this.myTriggerTableCount = 0;
                //        this.myTriggerTableRowNo = 0;

                       msPagesService.findRightDbTables(wnv, msWebScrapingDbFields_);
                    }
                    onay = true;
                }

                /// Load işlemleri sırasında ve set işlemi için bir dataSet ten bilgi alındı ise
                /// kullanıcı bu dataSet üzerinde position değiştirdiğinde 
                /// bulunduğu web sayfası tekrar yenilenmesi gerekiyor
                /// Örnek : kullanıcı SınavTarih listesinde pos değiştirince Mebbis sinav listesi sayfasıda ona göre değişmesi gerekiyor
                /// 
                if ((wnv.workEventsType == v.tWebEventsType.load) && (onay)) 
                {
                    if (wnv.ds != null)
                        if (wnv.ds.DataSetName != null)
                        {
                            string code = wnv.ds.DataSetName.ToString();
                            if (f.tableIPCodesInLoad.IndexOf(code) == -1)
                            {
                                f.tableIPCodesInLoad += "||" + code + "||";
                            }
                        }
                }

                if ((wnv.workEventsType == v.tWebEventsType.load) && (onay == false))
                    onay = true;

                /// Özel istisnai durumlar
                if (onay == false)
                {
                    // item item ile çalışma eventi aynı olmalı : örn : button5 == button5
                    if ((wnv.EventsType == wnv.workEventsType) ||
                        (wnv.EventsType == v.tWebEventsType.none))
                    {
                        /// kayıt butonu ise onayla
                        if (wnv.EventsType == v.tWebEventsType.button7) onay = true;
                        if (wnv.AttRole == "Button") onay = true;
                        if (wnv.AttType == "submit") onay = true;
                        if (wnv.InvokeMember == v.tWebInvokeMember.autoSubmit) onay = true;
                        if (t.IsNotNull(wnv.writeValue)) onay = true; //
                        if (wnv.TagName == "div") onay = true;  // testresult yüzünden eklendi
                        if (wnv.TagName == "span") onay = true; // kayıt sonucunu yazan element
                    }
                }
            }

            if (wnv.writeValue != null)
                if (wnv.writeValue.IndexOf("Error") > -1)
                    f.anErrorOccurred = true;
            
            return onay;
        }
        private async Task WebScrapingAfter(webNodeValue wnv)
        {
            //  web deki veriyi database aktar
            //  webden alınan veriyi (readValue yi)  db ye aktar
            
            if ((wnv.InjectType == v.tWebInjectType.Get ||
                (wnv.InjectType == v.tWebInjectType.GetAndSet && wnv.workRequestType == v.tWebRequestType.get)) &&
                (wnv.IsInvoke == false)) //(this.myTriggerInvoke == false)) // invoke gerçekleşmişse hiç başlama : get sırasında set edip bilgi çağrılıyor demekki
            {
                
                if (wnv.TagName != "table")
                    msPagesService.transferFromWebToDatabase(this, wnv, msWebScrapingDbFields_, aktifPageNodeItemsList_, f);

                if (wnv.TagName == "table")
                {
                    /// okunan tabloyu db ye yaz
                    if (wnv.tTable != null)
                    {
                        ////https://mebbis.meb.gov.tr/SKT/skt02006.aspx
                        //if (webMain.Url.ToString() == "https://mebbis.meb.gov.tr/SKT/skt02006.aspx")
                        //{
                        //    vUserInputBox iBox = new vUserInputBox();
                        //    iBox.Clear();
                        //    iBox.title = "Kaydın başlamasını istediğiniz sıra no";
                        //    iBox.promptText = "Sıra No  :";
                        //    iBox.value = "1";
                        //    iBox.displayFormat = "";
                        //    iBox.fieldType = 0;

                        //    // ınput box ile sorulan ise kimden kopyalanacak (old) bilgisi
                        //    if (t.UserInpuBox(iBox) == DialogResult.OK)
                        //    {
                        //        this.kayitBaslangisId = int.Parse(iBox.value);
                        //    }
                        //}
                        //v.con_EditSaveCount = 0;
                        //this.myTriggeringTable = true;
                        //this.myTriggerTableWnv = null;
                        //this.myTriggerTableWnv = wnv.Copy();

                        //this.myTriggeringItemButton = false;
                        //this.myTriggerTableCount = wnv.tTable.tRows.Count;
                        //this.myTriggerTableRowNo = 0;

                        webNodeValue myTriggerTableWnv = wnv.Copy();
                                                
                        await msPagesService.transferFromWebTableToDatabase(this, myTriggerTableWnv, msWebNodes_, msWebScrapingDbFields_, aktifPageNodeItemsList_);

                        t.TableRefresh(this, myTriggerTableWnv.TableIPCode);
                        //t.TableRefresh(this, wnv.TableIPCode);

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
                            msPagesService.transferFromWebSelectToDatabase(this, wnv, msWebScrapingDbFields_, aktifPageNodeItemsList_, f);
                            //MessageBox.Show("İşlem tamamlandı...");
                        }
                    }
                }
            }
        }
        
        private bool listControl(MsWebNode item, webWorkPageNodes workPageNodes)
        {
            // 1. aktif mi
            // 2. daha önce çalışmış mı çalışmamış mı ?
            bool onay = true;
            
            onay = item.IsActive;

            // debug durdurmak için
            //if (item.NodeId.ToString() == "3372")
            //    onay = true;

            if (onay)
            {
                // nodeId ile bu satır çalıştı mı diye kontrol için sadece
                string nodeNo = "|" + item.NodeId.ToString() + "|";
                int pos = workPageNodes.nodeIdList.IndexOf(nodeNo);
                // -1 ise daha önce çalışmamış 
                if (pos == -1)
                    workPageNodes.nodeIdList += nodeNo;
                else onay = false;
            }

            return onay;
        }
        private bool requestContol(MsWebNode item, v.tWebRequestType workRequestType, v.tWebEventsType workEventsType)
        {
            /// istek çeşitleri
            /// startNodes(v.tWebRequestType.get,  v.tWebEventsType.button3);
            /// startNodes(v.tWebRequestType.get,  v.tWebEventsType.button4);
            /// startNodes(v.tWebRequestType.post, v.tWebEventsType.button5);
            /// startNodes(v.tWebRequestType.post, v.tWebEventsType.button6);
            /// startNodes(v.tWebRequestType.post, v.tWebEventsType.button7);

            /// tWebInjectType
            /// none,
            /// AlwaysSet,
            /// Get,
            /// Set,
            /// GetAndSet

            /// tWebRequestType
            /// none,
            /// get,
            /// post,
            /// put,
            /// delete,
            /// getNodeItems,
            /// postNodeItems,
            /// alwaysSet

            /// tWebEventsType
            /// none = 0,
            /// load = 1,
            /// displayNone = 2,
            /// buttonAlwaysSet = 140,
            /// button3 = 143, 
            /// button4 = 144, 
            /// button5 = 145, 
            /// button6 = 146, 
            /// button7 = 147, 
            /// tableField = 148, 
            /// pageRefresh = 200

            bool onay = false;

            v.tWebInjectType injectType_ = (v.tWebInjectType)item.InjectType;
            v.tWebEventsType eventsType_ = (v.tWebEventsType)item.EventsType;
                        
            // istek load ise ve
            // event = node load ise
            if (workEventsType == v.tWebEventsType.load)
                if (eventsType_ ==  v.tWebEventsType.load) onay = true;

            // istek button3 ise
            // item.event = button3 veya none ise : true
            if (workEventsType == v.tWebEventsType.button3)
                if (eventsType_ == v.tWebEventsType.button3 || eventsType_ == v.tWebEventsType.none) onay = true;
            if (workEventsType == v.tWebEventsType.button4)
                if (eventsType_ == v.tWebEventsType.button4 || eventsType_ == v.tWebEventsType.none) onay = true;
            if (workEventsType == v.tWebEventsType.button5)
                if (eventsType_ == v.tWebEventsType.button5 || eventsType_ == v.tWebEventsType.none) onay = true;
            if (workEventsType == v.tWebEventsType.button6)
                if (eventsType_ == v.tWebEventsType.button6 || eventsType_ == v.tWebEventsType.none) onay = true;
            
            if (workEventsType == v.tWebEventsType.button7)
                if (eventsType_ == v.tWebEventsType.button7) onay = true;

            // post işlemleri ve autoSubmit var ise
            if ((workEventsType == v.tWebEventsType.button5) ||
                (workEventsType == v.tWebEventsType.button6))
                    if (item.InvokeMember == (Int16)v.tWebInvokeMember.autoSubmit) onay = true;

            if (workEventsType == v.tWebEventsType.tableField)
                if (eventsType_ == v.tWebEventsType.tableField) onay = true;

            if (onay)
            {
                // istek get ise
                // item.inject = none veya set veya alwaysset ise 
                if (workRequestType == v.tWebRequestType.get)
                    if (injectType_ == v.tWebInjectType.none ||
                        injectType_ == v.tWebInjectType.Set || 
                        injectType_ == v.tWebInjectType.AlwaysSet) onay = false;
                // istek post
                // item.inject = none veya get ise 
                if (workRequestType == v.tWebRequestType.post)
                    if (injectType_ == v.tWebInjectType.none ||
                        injectType_ == v.tWebInjectType.Get) onay = false;
            }

            if (eventsType_ == v.tWebEventsType.displayNone) onay = true;
            
            // false olabilir
            if (item.IsActive == false) onay = false;

            return onay;
        }
        private bool valueChecked(webNodeValue wnv)
        {
            bool onay = true;

            if (wnv.KrtOperandType == null) return onay;
            if (wnv.KrtOperandType == "") return onay;

            onay = t.myOperandControl(wnv.writeValue , wnv.CheckValue, wnv.KrtOperandType);

            return onay;
        }

        /// WebScrapingAsync - Kazıma işlemi
        /// 
        /// 
        private async Task WebScrapingAsync(IWebDriver wb, webNodeValue wnv)
        {
            if (f.anErrorOccurred) return;
            if (wb.PageSource == null) return;
            if (wnv.TagName == null) return;

            string AttType = wnv.AttType;
            string AttRole = wnv.AttRole;
            string AttHRef = wnv.AttHRef;
            string AttSrc = wnv.AttSrc;
            string XPath = wnv.XPath;
            string InnerText = wnv.InnerText;
            string OuterText = wnv.OuterText;
            string TagName = wnv.TagName.ToLower();
            string idName = "";
            string readValue = "";
            string writeValue = "";

            v.tWebInjectType injectType = wnv.InjectType;
            v.tWebInvokeMember invokeMember = wnv.InvokeMember;
            v.tWebRequestType workRequestType = wnv.workRequestType;
            v.tWebEventsType eventsType = wnv.EventsType;
            v.tWebEventsType workEventsType = wnv.workEventsType;

            if (!string.IsNullOrEmpty(wnv.writeValue)) writeValue = wnv.writeValue;
            if (wnv.AttId != "") idName = wnv.AttId;
            if ((wnv.AttId == "") && (wnv.AttName != "")) idName = wnv.AttName;

            ///
            /// displayNone işlemi
            /// 
            if (wnv.EventsType == v.tWebEventsType.displayNone)
            {
                displayNone(wb, TagName, idName, XPath, InnerText);
                return;
            }
            
            if (AttRole == "ItemButton")
            {
                //if (eventsType == v.tWebEventsType.itemButton)
                // table listesindeki satırın detayını açmak için kullanılıyor
                // 
                preparingItemButtonsList(wb, wnv, TagName, AttSrc);
            }
            
            if (TagName == "a")
            {
                // button TagName olmayan fakat click eventi olan taga button rolü yükleniyor 
                if (!string.IsNullOrEmpty(AttHRef) && AttRole != "button")
                    AttRole = "Button";
            }

            if (TagName == "input")
            {
                if (AttType == "submit") // || (AttType == "file"))
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
                    
                }
            }
            
            if (TagName == "select")
            {
                if (workRequestType == v.tWebRequestType.getNodeItems)
                    selectItemsRead(wb, ref wnv, idName);

                if ((workRequestType == v.tWebRequestType.get) &&
                    (AttRole == "ItemTable"))
                {
                    selectItemsRead(wb, ref wnv, idName);
                    TagName = ""; // aşağıdaki get işlemine girmesin, teğet geçsin
                }

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
             
            if (TagName == "div")
            {
                await divOperations(wb, wnv);
                return;
            }

            if (TagName == "span")
            {
                await spanOperations(wb, wnv);
                return;
            }

            if (AttRole == "Button")
            {
                // tablolar üzerindeki liste satırları üzerindeki butonlar
                // veya kaydet, refresh, yardım masası gibi toolbar butonları
                //buttonsClick(wb, TagName, idName, InnerText, OuterText, invokeMember, wnv.workEventsType);
                buttonsClick(wb, wnv);
                return;
            }

            /// SecurityImage
            /// 
            if (AttRole == "SecurityI")
            {
                bool secOnay = await getSecurityImageValue(wb, wnv, idName);
            }
            /// Güvenlik kodunu sor veya başka bir değerde sorabilir
            /// 
            if (AttRole == "InputBox")
            {
                writeValue = await getInputBoxValue(wb, wnv);
                wnv.writeValue = writeValue;
            }
            /// SecurityAgain : Güvenlik Kodu Tekrarı
            /// 
            if (AttRole == "SecurityA")
            {
                /// Bu işleme gelmeden önce SecurityImage veya InputBox aracılığı ile
                /// güvenlik kodu okunmuş olmalı
                /// 
                wnv.writeValue = f.securityCode;
                writeValue = f.securityCode;
            }
            
            if (TagName == "table")
            {
                if (injectType == v.tWebInjectType.Get ||
                   (injectType == v.tWebInjectType.GetAndSet && workRequestType == v.tWebRequestType.get))
                {
                    getHtmlTable(wb, ref wnv, idName);
                }

                if ((injectType == v.tWebInjectType.Set ||
                    (injectType == v.tWebInjectType.GetAndSet && workRequestType == v.tWebRequestType.post)) &&
                    (TagName == "table"))
                {
                    postHtmlTable(wb, ref wnv, idName);
                }
            }

            ///
            /// Set işlemleri 
            /// 
            if ((injectType == v.tWebInjectType.Set ||
                (injectType == v.tWebInjectType.GetAndSet && workRequestType == v.tWebRequestType.post) ||
                (injectType == v.tWebInjectType.AlwaysSet && workEventsType == v.tWebEventsType.load)
               ) &&
                (TagName == "input" || TagName == "select") &&
                (writeValue != "") &&
                (idName != ""))
            {
                invokeMember = await setElementValues(wb, TagName, AttType, idName, writeValue, invokeMember);
            }

            ///
            /// Get işlemleri 
            /// 
            if ((injectType == v.tWebInjectType.Get ||
                (injectType == v.tWebInjectType.GetAndSet && workRequestType == v.tWebRequestType.get)) &&
                (TagName == "input" || TagName == "span" || TagName == "select" || TagName == "img") &&
                (idName != ""))
            {
                await getElementValues(wb, wnv, TagName, AttType, AttRole, idName);
            }

            if (f.anErrorOccurred) return;

            #region
              
            //
            // Atanan valueden sonra nesnenin tetiklenmesi gerekirse
            //
            if (invokeMember > v.tWebInvokeMember.none)
            {
                if (injectType == v.tWebInjectType.Set ||
                    injectType == v.tWebInjectType.AlwaysSet ||
                   (injectType == v.tWebInjectType.GetAndSet && workRequestType == v.tWebRequestType.post))
                {
                    //this.myDocumentCompleted = false;
                    //this.myTriggerInvoke = true;           <<<< gerek kalmadı
                    //timerTrigger.Enabled = false;
                    await invokeMemberExec(wb, wnv, invokeMember, writeValue, idName);
                }
            }
            
            #endregion
            wnv.readValue = readValue;
        }

        #endregion Scraping functions

        #region subFunctions
        private async Task myPageViewClickAsync(IWebDriver wb, List<MsWebPage> msWebPage)
        {
            if (msWebPage[0].Id > 0) 
            {
                bool onay = false;

                //v.SQL = v.SQL + v.ENTER + myNokta + " PageView : ";

                f.aktifPageCode = msWebPage[0].PageCode;    
                f.talepEdilenUrl = msWebPage[0].PageUrl;
                f.talepEdilenUrl2 = msWebPage[0].PageUrl;
                f.talepOncesiUrl = msWebPage[0].BeforePageUrl; 
                f.talepPageLeft = msWebPage[0].PageLeft;       
                f.talepPageTop = msWebPage[0].PageTop;         

                if (f.aktifUrl != f.talepEdilenUrl)
                {
                    if (t.IsNotNull(f.talepOncesiUrl))
                        onay = await loadPageUrl(wb, f.talepOncesiUrl);
                    else
                        onay = await loadPageUrl(wb, f.talepEdilenUrl);
                }
                else
                {
                    onay = await loadPageUrl(wb, f.talepEdilenUrl);
                }
            }
        }
        private async Task<bool> loadPageUrl(IWebDriver wb, string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    //wb.Url = url;
                    wb.Navigate().GoToUrl(url);
                }
                catch (Exception)
                {
                    /// Kullanıcı manuel webBrowser kapatmış olabilir
                    /// 
                    v.webDriver_ = null;
                    wb = null;
                    preparingWebMain();
                    wb = v.webDriver_;
                    try
                    {
                        wb.Navigate().GoToUrl(url);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error Navigate ( " + url + " )" + v.ENTER2 + ex.Message);
                        //throw;
                    }
                    //throw;
                }
                
                f.aktifUrl = wb.Url;

                openPageControlAsync(wb);
                return true;
            }
            else
            {
                MessageBox.Show("DİKKAT : Lütfen  - page url - yi girin...");
                return false;
            }
        }
        private async Task openPageControlAsync(IWebDriver wb)
        {
            if (t.IsNotNull(f.talepEdilenUrl) == false)
                return;

            bool onay = false;
                        

            if (f.talepEdilenUrl != f.aktifUrl)
            {
                if (f.aktifUrl.IndexOf("oturumsonu") > -1)
                {
                    return;
                }

                /// aktif url Login page değil ise nedir ?
                /// 
                /// talep öncesi başka bir url çağrısı var ise
                ///
                if (t.IsNotNull(f.talepOncesiUrl))
                {
                    /// aktif url ne talep edilen ne de talep öncesi url değil ise 
                    /// talepOncesiUrl yi çağır
                    ///
                    if ((f.talepOncesiUrl != f.aktifUrl) &&
                        (f.talepEdilenUrl != f.aktifUrl))
                        await runLoginPage(v.webDriver_);
                        

                    /// talep öncesi geldiyse sıra esas talep edilen url ye sıra geldi
                    ///
                    if (f.talepOncesiUrl == f.aktifUrl)
                    {
                        loadPageUrl(wb, f.talepEdilenUrl);
                            
                        // nodenin içindeki itemValue ve itemText listesi (combo içerikleri)
                        if (t.IsNotNull(f.aktifPageCode))
                            aktifPageNodeItemsList_ = msPagesService.readNodeItems(f.aktifPageCode);
                    }
                }
                else
                {
                    /// talepOncesiUrl yok ise şimdi talepEdilenUrl yi çağıralım 
                    ///
                    loadPageUrl(wb, f.talepEdilenUrl);
                }
            }

            if (f.talepEdilenUrl == f.aktifUrl)
            {
                if (f.loadWorking == false)
                {
                    t.WaitFormOpen(this, "Sayfa yükleniyor ...");
                    await startNodes(this.msWebNodes_, this.workPageNodes_, v.tWebRequestType.post, v.tWebEventsType.load);
                    f.loadWorking = true;
                    v.IsWaitOpen = false;
                    t.WaitFormClose();
                }
            }

        }
        private async Task runLoginPage(IWebDriver wb)
        {
            /// burası kullanıcı 
            if (f.errorPageUrl == f.aktifUrl)
            {
                loadPageUrl(wb, f.loginPageUrl);
            }
            else if (f.aktifUrl.IndexOf("oturumsonu") > -1)
            {
                return;
            }
            else
            {

                //t.FlyoutMessage(this, "Yeniden giriş :", "Bağlıntınız kopmuş durumda yeniden giriş işlemi yapılacaktır ...");
                t.AlertMessage("Bağlantı kopukluğu :", "Bağlıntınız kopmuş durumda yeniden giriş işlemi yapılacaktır ...");
                /// Login page bilgilerine focus ol
                if (t.IsNotNull(ds_MsWebPages))
                {
                    dN_MsWebPages.Position = 0;
                    Application.DoEvents();
                    ((DevExpress.XtraGrid.GridControl)view_MsWebPages).MainView.Focus();
                }
                /// login işlemlerini gerçekleştir
                /// 
                //await startNodes(this.msWebLoginNodes_, this.workPageNodes_, v.tWebRequestType.post, v.tWebEventsType.load);
                

            }
        }
        private void displayNone(IWebDriver wb, string tagName, string idName, string XPath, string InnerText)
        {
            if (t.IsNotNull(idName))
            {
                /*
                IWebElement element = wb.FindElement(By.Id(idName));
                if (element != null)
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)wb;
                    js.ExecuteScript("arguments[0].style.display = 'none';", element);
                }
                */
                IList<IWebElement> elements = wb.FindElements(By.TagName(tagName));
                foreach (IWebElement element in elements)
                {
                    string elementId = element.GetAttribute("id");
                    if (elementId == idName)
                    {
                        string innerText = element.Text;
                        if (InnerText != "")
                        {
                            if (innerText.IndexOf(InnerText) > -1)
                            {
                                IJavaScriptExecutor js = (IJavaScriptExecutor)wb;
                                js.ExecuteScript("arguments[0].style.display = 'none';", element);
                                break;
                            }
                        }
                        else
                        {
                            IJavaScriptExecutor js = (IJavaScriptExecutor)wb;
                            js.ExecuteScript("arguments[0].style.display = 'none';", element);
                        }
                    }
                }
                #region
                /*
                if (tagName == "table")
                {
                    IWebElement element = wb.FindElement(By.Id(idName));
                    if (element != null)
                    {
                        IJavaScriptExecutor js = (IJavaScriptExecutor)wb;
                        js.ExecuteScript("arguments[0].style.display = 'none';", element);
                    }
                }
                else
                {
                    IWebElement element = wb.FindElement(By.Id(idName));
                    if (element != null)
                    {
                        IJavaScriptExecutor js = (IJavaScriptExecutor)wb;
                        js.ExecuteScript("arguments[0].style.display = 'none';", element);
                    }
                }
                */
                #endregion
            }
            else
            {
                //
            }

        }
        private void preparingItemButtonsList(IWebDriver wb, webNodeValue wnv, string tagName, string attSrc)
        {
            // şimdilik sadece itemButton var
            // aktif olan sayfadaki tüm uygun elementleri topla
            // burada sadece itemButton olan elementler toplanıyor 

            // table listesindeki satırın detayını açmak için kullanılıyor
            // 
            string src = "";
            IList<IWebElement> elements = wb.FindElements(By.TagName(tagName));
            foreach (IWebElement item in elements)
            {
                //Console.WriteLine(element.Text);
                src = item.GetAttribute("src");
                if (src.IndexOf(attSrc) > -1)
                {
                    // uygun olan element leri topla
                    wnv.elementsSelenium.Add(item);
                }
            }
        }
        private void selectItemsRead(IWebDriver wb, ref webNodeValue wnv, string idName)
        {
            //SelectElement selectElement = new SelectElement(driver.FindElement(By.Id("comboElementId")));
            //List<IWebElement> options = selectElement.Options.ToList();
            //List<string> optionValues = new List<string>();
            //List<string> optionTexts = new List<string>();
            //foreach (IWebElement option in options)
            //{
            //    optionValues.Add(option.GetAttribute("value"));
            //    optionTexts.Add(option.Text);
            //}

            t.WaitFormOpen(v.mainForm, "Kaynak veriler okunuyor...");
            
            SelectElement selectElement = new SelectElement(wb.FindElement(By.Id(idName)));
            if (selectElement == null) return;
            List<IWebElement> options = selectElement.Options.ToList();

            tTable table = new tTable();
            string value = "";
            string text = "";
            foreach (IWebElement item in options)
            {
                tRow row = new tRow();
                value = item.GetAttribute("value");
                text = item.Text;

                tColumn column1 = new tColumn();
                column1.value = value;
                tColumn column2 = new tColumn();
                column2.value = text;

                row.tColumns.Add(column1);
                row.tColumns.Add(column2);

                table.tRows.Add(row);
            }
            wnv.tTable = table;

            v.IsWaitOpen = false;
            t.WaitFormClose();
        }
        private string selectItemsGetValue(IWebDriver wb, ref webNodeValue wnv, string idName, string findText)
        {
            IList<IWebElement> elements = wb.FindElements(By.Id(idName));
            
            if (elements == null) return "";

            string value = "";
            string text = "";

            foreach (IWebElement item in elements)
            {
                value = item.GetAttribute("value");
                text = item.Text;

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
        //private void buttonsClick(IWebDriver wb, string tagName, string idName, string innerText, string outerText, v.tWebInvokeMember invokeMember, v.tWebEventsType workEventsType)
        private async Task buttonsClick(IWebDriver wb, webNodeValue wnv)
        {
            if (f.anErrorOccurred) return;

            //string tagName, string idName, string innerText, string outerText, v.tWebInvokeMember invokeMember, v.tWebEventsType workEventsType

            if (wnv.InvokeMember == v.tWebInvokeMember.autoSubmit)
            {
                SetAutoSubmit(wb, wnv);
                return;
            }

            string idName = "";
            if (wnv.AttId != "") idName = wnv.AttId;
            if ((wnv.AttId == "") && (wnv.AttName != "")) idName = wnv.AttName;

            if (t.IsNotNull(idName) == false)
            {
                IList<IWebElement> elements = wb.FindElements(By.TagName(wnv.TagName));
                foreach (IWebElement item in elements)
                {
                    if (item.Text == wnv.InnerText)
                    {
                        item.Click();
                        break;
                    }
                }
            }
            else
            {
                try
                {
                    IWebElement element = wb.FindElement(By.Id(idName));
                    if (element != null)
                        element.Click();
                }
                catch (Exception exc1)
                {
                    f.anErrorOccurred = true;

                    string inner = (exc1.InnerException != null ? exc1.InnerException.ToString() : exc1.Message.ToString());

                    MessageBox.Show("DİKKAT [error 1003] : [ " + idName + " ] sırasında sorun oluştu ..." +
                        v.ENTER2 + inner);
                }
            }
        }
        private void SetAutoSubmit(IWebDriver wb, webNodeValue wnv)
        {
            // AutoSubmit, otomatik kaydet var ise
            bool onay = f.autoSubmit;

            // btn_FullSave click'lenmişse 
            if (wnv.workEventsType == v.tWebEventsType.button7) onay = true;

            if (onay == false)
            {
                t.FlyoutMessage(this, "Uyarı ", wnv.OuterText);
                return;
            }

            string idName = "";
            if (wnv.AttId != "") idName = wnv.AttId;
            if ((wnv.AttId == "") && (wnv.AttName != "")) idName = wnv.AttName;

            try
            {
                IWebElement element = wb.FindElement(By.Id(idName));
                if (element != null)
                    element.Click();

                if (f.autoSubmit)
                {
                    t.AlertMessage("İşlem onayı", wnv.InnerText);
                }
            }
            catch (Exception exc1)
            {
                f.anErrorOccurred = true;

                string inner = (exc1.InnerException != null ? exc1.InnerException.ToString() : exc1.Message.ToString());

                MessageBox.Show("DİKKAT [error 1004] : [ " + idName + " ] sırasında sorun oluştu ..." +
                    v.ENTER2 + inner);
            }
        }
        private async Task divOperations(IWebDriver wb, webNodeValue wnv)
        {
            if (wnv.TagName == "div")
            {
                string idName = "";
                if (wnv.AttId != "") idName = wnv.AttId;
                if ((wnv.AttId == "") && (wnv.AttName != "")) idName = wnv.AttName;

                IWebElement element = wb.FindElement(By.Id(idName));

                if (element != null)
                {
                    string className = wnv.AttClass;
                    if (t.IsNotNull(className) == false) className = "error"; 
                    
                    IList<IWebElement> errorElements = element.FindElements(By.ClassName(className));

                    if (errorElements != null)
                    {
                        if (errorElements.Count > 0)
                        {
                            f.anErrorOccurred = true;
                            t.FlyoutMessage(this, "Hata", wnv.OuterText);
                        }
                    }
                }
            }
        }
        private async Task spanOperations(IWebDriver wb, webNodeValue wnv)
        {
            if (wnv.TagName == "span")
            {
                string idName = "";
                if (wnv.AttId != "") idName = wnv.AttId;
                if ((wnv.AttId == "") && (wnv.AttName != "")) idName = wnv.AttName;

                try
                {
                    // elementi  1 saniye ara
                    wb.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);

                    IWebElement element = wb.FindElement(By.Id(idName));

                    if (element != null)
                    {
                        string innerText = wnv.InnerText;
                        string elementText = element.Text;

                        /// İşlem sonuçları sonunda kullanıcıya browser üzerinde verilen mesajlar
                        /// Örnek : Kayıt İşleminiz Başarı İle Gerçekleşti. veya diğer hata mesajları
                        /// 
                        if (innerText != elementText)
                            t.FlyoutMessage(this, "Uyarı :", elementText);
                        else
                        {
                            if (wnv.AttRole == "Success")
                            {
                                if (t.IsNotNull(wnv.ds))
                                {
                                    if (wnv.dbFieldType == 104) // bit
                                        wnv.ds.Tables[0].Rows[wnv.dN.Position][wnv.dbFieldName] = 1;

                                    if (wnv.GetSave)
                                    {
                                        msPagesService.dbButtonClick(this, wnv.ds.DataSetName, v.tButtonType.btKaydet);
                                    }
                                }
                                //t.FlyoutMessage(this, "Bilgilendirme", elementText);
                                t.AlertMessage("Bilgilendirme", elementText);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //
                }
                
                wb.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            }
        }
        private async Task<v.tWebInvokeMember> setElementValues(IWebDriver wb, string tagName, string attType, string idName, string writeValue, v.tWebInvokeMember invokeMember)
        {
            //
            // Value atama işlemi 
            //
            #region
            IWebElement element = null;
            try
            {
                if ((attType != "file") && (attType != "checkbox") && (attType != "radio"))
                {
                    element = wb.FindElement(By.Id(idName));   
                    if (element != null)
                    {
                        if (tagName == "select")
                        {
                            //IList<IWebElement> comboOptionElements = element.FindElements(By.TagName("option"));
                            //comboOptionElements.FirstOrDefault(x => x.Text == writeValue)?.Click();
                            
                            SelectElement oSelect = new SelectElement(element);
                            List<string> values = oSelect.Options.Select(option => option.GetAttribute("value")).ToList();
                            string value = values.Find(s => s.Contains(writeValue)); 

                            if (value != null)
                            {
                                oSelect.SelectByValue(writeValue);
                                //oSelect.SelectByIndex(index);
                                //oSelect.SelectByText(text);
                            }
                            /*
                            SelectElement oSelect = new SelectElement(driver.FindElement(By.Id(Element_ID)));
                            oSelect.SelectByIndex(index);
                            oSelect.SelectByText(text);
                            oSelect.SelectByValue(value);
                            */
                        }
                        else
                        {
                            // normal text gibi inputlar 
                            if (attType != "radio")
                            {
                                if (idName.IndexOf("Tarih") > -1)
                                    writeValue = writeValue.Replace(".", "/");

                                element.Click();
                                element.Clear();
                                element.SendKeys(writeValue);
                            }
                            if (attType == "radio") 
                                element.SendKeys(writeValue);
                        }

                        /*
                        //cmbEgitimDonemi
                        IWebElement cmbEgitimDonemi = seleniumWebDriver.FindElement(By.Name("cmbEgitimDonemi"));
                        IWebElement cmbGrubu = seleniumWebDriver.FindElement(By.Name("cmbGrubu"));
                        IWebElement cmbSubesi = seleniumWebDriver.FindElement(By.Name("cmbSubesi"));

                        // SelectElement sınıfını kullanarak "select" elementini seçin
                        IList<IWebElement> cmbEgitimDonemiOptionElements = cmbEgitimDonemi.FindElements(By.TagName("option"));
                        cmbEgitimDonemiOptionElements.FirstOrDefault(x => x.Text == kursiyer.Donem)?.Click();

                        IList<IWebElement> cmbGrubuOptionElements = cmbGrubu.FindElements(By.TagName("option"));
                        cmbGrubuOptionElements.FirstOrDefault(x => x.Text == kursiyer.Grup)?.Click();

                        IList<IWebElement> cmbSubesiOptionElements = cmbSubesi.FindElements(By.TagName("option"));
                        cmbSubesiOptionElements.FirstOrDefault(x => x.Text == kursiyer.Sube)?.Click();
                        */
                        //v.SQL = v.SQL + v.ENTER + myNokta + " set value : " + writeValue;
                    }

                    if (writeValue.ToLower().IndexOf("selectedindex=") > -1)
                    {
                        writeValue = writeValue.Replace("selectedindex=", "");
                        //element.SetAttribute("selectedindex", writeValue);

                        IList<IWebElement> optionElements = element.FindElements(By.TagName("option"));
                        optionElements.FirstOrDefault(x => x.Text == writeValue)?.Click();

                        //v.SQL = v.SQL + v.ENTER + myNokta + " set selectedindex : " + writeValue;
                    }

                    //t.WebReadyComplate(wb);
                }
                if ((attType == "checkbox") || (attType == "radio"))
                {
                    if (writeValue == "True")
                    {
                        element = wb.FindElement(By.Id(idName));

                        if (element != null)
                        {
                            if (attType == "checkbox")
                            {
                                //element.SetAttribute("checked", writeValue);
                                //IJavaScriptExecutor js = (IJavaScriptExecutor)wb;
                                //js.ExecuteScript("arguments[0].checked = '"+writeValue+"';", element);
                                // görevini burada yaptı, dönüşte tekrar clicklemesin
                                element.Click();
                                invokeMember = v.tWebInvokeMember.none;
                            }
                            if (attType == "radio")
                            {
                                //element.SetAttribute("value", "1");
                                //IJavaScriptExecutor js = (IJavaScriptExecutor)wb;
                                //js.ExecuteScript("arguments[0].value = '1';", element);
                                element.Click();
                                invokeMember = v.tWebInvokeMember.none;
                            }
                            //v.SQL = v.SQL + v.ENTER + myNokta + " set checked : " + writeValue;
                        }
                    }

                    //if ((writeValue == "False") && (invokeMember > v.tWebInvokeMember.none))
                    if ((writeValue != "True") && (invokeMember > v.tWebInvokeMember.none))
                        invokeMember = v.tWebInvokeMember.none;
                }
                if (attType == "file")
                {
                    //if (attRole == "ImageData")

                    element = wb.FindElement(By.Id(idName));
                    if (element != null)
                    {
                        //Bu komut klavyeye basar gibi davranıyor, gönder.exe gibi çalışıyor
                        //SendKeys.Send(writeValue);// + "{ENTER}");
                        
                        // Bu işlemde direkt nesneye fileName atama yapıyor  
                        // Resim yükleniyor .....
                        //
                        element.SendKeys(writeValue);
                        Thread.Sleep(2000);
                    }
                    //v.SQL = v.SQL + v.ENTER + myNokta + " set file (SendKeys.Send) : " + writeValue;
                }
            }
            catch (Exception exc1)
            {
                f.anErrorOccurred = true;

                string inner = (exc1.InnerException != null ? exc1.InnerException.ToString() : exc1.Message.ToString());

                MessageBox.Show("DİKKAT [error 1001] : [ " + idName + " (" + writeValue + ") ] veri ataması sırasında sorun oluştu ..." +
                    v.ENTER2 + inner);
            }
            return invokeMember;
            #endregion
        }
        private async Task<string> getElementValues(IWebDriver wb, webNodeValue wnv, string tagName, string attType, string attRole, string idName)
        {
            //
            // Value alma/okuma işlemi 
            //
            #region
            IWebElement element = null;
            string readValue = "";
            try
            {
                if (tagName == "input" || tagName == "select")
                {
                    element = null;
                    element = wb.FindElement(By.Id(idName)); 
                    if (element != null)
                    {
                        //tagName == "input"
                        readValue = element.Text;

                        // select in value si değilde text kısmı okunacak ise 
                        // writeValeu veya AttRole bizim tarafımızdan manuel işaretleniyor

                        if ((tagName == "select") && (attRole != "ItemTable"))
                        {
                            if ((wnv.writeValue == "InnerText") ||
                                (attRole == "GetCaption") || 
                                (attRole == "text") || 
                                (attRole == "InnerText"))
                            { 
                                SelectElement selectElement = new SelectElement(wb.FindElement(By.Id(idName)));
                                readValue = selectElement.SelectedOption.Text;
                            }
                            else
                            {
                                SelectElement selectElement = new SelectElement(wb.FindElement(By.Id(idName)));
                                readValue = selectElement.SelectedOption.GetAttribute("value");
                            }
                        }
                        // önceki hali
                        //readValue = element.GetAttribute("value");
                        //if ((tagName == "select") &&
                        //    ((wnv.writeValue == "InnerText") ||
                        //     (attRole == "GetCaption") || (attRole == "text") || (attRole == "InnerText")
                        //     ))
                        //{
                        //    foreach (HtmlElement item in element.Children)
                        //    {
                        //        if (readValue == item.GetAttribute("value"))
                        //        {
                        //            readValue = item.InnerText;
                        //            v.SQL = v.SQL + v.ENTER + myNokta + " get select : " + readValue;
                        //            break;
                        //        }
                        //    }
                        //}
                    }

                }
                if (tagName == "span")
                {
                    element = null;
                    element = wb.FindElement(By.Id(idName));
                    if (element != null)
                    {
                        readValue = element.Text;
                        //v.SQL = v.SQL + v.ENTER + myNokta + " get span : " + readValue;
                    }
                }
                if (tagName == "img")
                {
                    element = null;
                    element = wb.FindElement(By.Id(idName));

                    if (element != null)
                    {
                        //örnek :
                        //string urlDownload = @"https://mebbis.meb.gov.tr/SKT/AResimGoster.aspx";
                        string urlDownload = element.GetAttribute("src");

                        if (f.sessionIdAndToken == "")
                            f.sessionIdAndToken = tCookieReader.GetCookie($"https://mebbis.meb.gov.tr/default.aspx");

                        readValue = msPagesService.ImageDownload(urlDownload, f.sessionIdAndToken, f.aktifUrl);

                        //v.SQL = v.SQL + v.ENTER + " get img : " + readValue;
                    }
                }
                if (attType == "checkbox")
                {
                    element = null;
                    element = wb.FindElement(By.Id(idName));
                    if (element != null)
                    {
                        readValue = element.GetAttribute("checked");
                        //v.SQL = v.SQL + v.ENTER + myNokta + " get checkbox : " + readValue;
                    }
                }

                //t.WebReadyComplate(wb);
            }
            catch (Exception exc1)
            {
                string inner = (exc1.InnerException != null ? exc1.InnerException.ToString() : exc1.Message.ToString());

                //t.WebReadyComplate(wb);
                MessageBox.Show("DİKKAT [error 1002] : [ " + idName + " ] veri okuması sırasında sorun oluştu ..." +
                    v.ENTER2 + inner);
            }
            return readValue;
            #endregion
        }
        private async Task invokeMemberExec(IWebDriver wb, webNodeValue wnv, v.tWebInvokeMember invokeMember, string writeValue, string idName)
        {
            if (f.anErrorOccurred) return;

            string invoke = "";

            // this.myTriggerInvoke 
            // invoke çalışınca her zaman webbrowserDocumentCompleted  tetiklenmiyor
            // bu event çalıştımı çalışmadı mı diye 
            // this.myDocumentCompleted kullanılıyor
            //
            if (wnv.InvokeMember == v.tWebInvokeMember.autoSubmit)
            {
                SetAutoSubmit(wb, wnv);
                return;
            }
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

            //v.SQL = v.SQL + myNokta + " : invoke = " + invoke;

            try
            {
                if ((idName != "") && (invoke != ""))
                {
                    //t.WebReadyComplate(wb);

                    IWebElement element = null;
                    element = wb.FindElement(By.Id(idName));
                    if (element != null)
                    {
                        //wb.Document.GetElementById(idName).InvokeMember(invoke);
                        if (invoke == "click")
                            element.Click();
                        if (invoke == "submit")
                            element.Submit();
                        if (invoke == "onchange")
                        {
                            IList<IWebElement> optionElements = element.FindElements(By.TagName("option"));
                            optionElements.FirstOrDefault(x => x.Text == writeValue)?.Click();
                        }
                        
                        wnv.IsInvoke = true; // invoke çalıştı
                    }
                    Thread.Sleep(1000);
                    Application.DoEvents();

                    //t.WebReadyComplate(wb);
                }
            }
            catch (Exception exc2)
            {
                f.anErrorOccurred = true;

                string inner = (exc2.InnerException != null ? exc2.InnerException.ToString() : exc2.Message.ToString());

                //t.WebReadyComplate(wb);

                MessageBox.Show("DİKKAT [error 1002] : [ " + idName + " (" + writeValue + "), (" + invoke + ") ] verinin çalıştırılması sırasında sorun oluştu ..." +
                    v.ENTER2 + inner);
            }
        }
        private async Task<bool> getSecurityImageValue(IWebDriver wb, webNodeValue wnv, string idName)
        {
            bool onay = false;
            return onay;
            string value = "";
            // Locate the image element
            IWebElement image = wb.FindElement(By.Id(idName));
            // Get the src attribute of the image
            string src = image.GetAttribute("src");

            /*
            // gelen byte image çevir
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image oldImage = Image.FromStream(ms);

            // image yi bitmap a çevir
            Bitmap workingImage = new Bitmap(oldImage, oldImage.Width, oldImage.Height);
            */

            // Download the image from the src URL
            using (WebClient webClient = new WebClient())
            {
                /*
                // Create a CookieContainer object
                CookieContainer cookieContainer = new CookieContainer();

                // Add some cookies to the CookieContainer
                cookieContainer.Add(new System.Net.Cookie("name", "value", "/", "example.com"));

                // Set the CookieContainer for the WebClient
                webClient.CookieContainer = cookieContainer;

                // Get the image source URL
                //string src = "https://example.com/getphoto.action?memberInfo.memberNumber=123";

                // Download the image data from the src URL
                byte[] data = webClient.DownloadData(src);

                // Save the image data to a file
                System.IO.File.WriteAllBytes("image.jpg", data);
                */

                /*
                                try
                                {
                                    byte[] data = webClient.DownloadData(src);
                                    // Save the image as a Bitmap object
                                    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(data))
                                {
                                    Bitmap bitmap = new Bitmap(memoryStream);

                                    // Create a Tesseract engine with the tessdata folder and the language
                                    using (TesseractEngine engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                                    {
                                        // Process the Bitmap object and get the Page object
                                        using (Page page = engine.Process(bitmap))
                                        {
                                            // Get the text from the Page object
                                            value = page.GetText();
                                            wnv.writeValue = value;
                                            if (value != "") onay = true;
                                        }
                                    }
                                }
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.Message);
                                    //throw;
                                }
                 */
            }

            return onay;

            #region 
            /*
            // Create a Chrome driver
            IWebDriver driver = new ChromeDriver();

            // Navigate to the web page with the image
            driver.Navigate().GoToUrl("https://example.com");

            // Locate the image element
            IWebElement image = driver.FindElement(By.Id("security-code"));

            // Get the src attribute of the image
            string src = image.GetAttribute("src");

            // Download the image from the src URL
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(src);

                // Save the image as a Bitmap object
                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(data))
                {
                    Bitmap bitmap = new Bitmap(memoryStream);

                    // Create a Tesseract engine with the tessdata folder and the language
                    using (TesseractEngine engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                    {
                        // Process the Bitmap object and get the Page object
                        using (Page page = engine.Process(bitmap))
                        {
                            // Get the text from the Page object
                            string text = page.GetText();

                            // Compare the text with the expected security code
                            if (text == "1234")
                            {
                                // Security code matched
                                Console.WriteLine("Security code matched");
                            }
                            else
                            {
                                // Security code did not match
                                Console.WriteLine("Security code did not match");
                            }
                        }
                    }
                }
            }

            // Close the driver
            driver.Quit();

            */
            #endregion
        }
        private async Task<string> getInputBoxValue(IWebDriver wb, webNodeValue wnv)
        {
            vUserInputBox iBox = new vUserInputBox();
            iBox.Clear();
            iBox.title = wnv.OuterText;
            iBox.promptText = wnv.InnerText;
            iBox.value = wnv.writeValue;
            iBox.displayFormat = "";
            iBox.fieldType = 0;

            // ınput box ile sorulan ise kimden kopyalanacak (old) bilgisi
            if (t.UserInpuBox(iBox) == DialogResult.OK)
            {
                wnv.writeValue = iBox.value;
            }

            if ((wnv.OuterText.IndexOf("Güvenlik") > -1) ||
                (wnv.InnerText.IndexOf("Güvenlik") > -1))
                f.securityCode = wnv.writeValue;
             
            return wnv.writeValue;
        }

        #endregion subFunctions


        #region getHtmlTable
        private void getHtmlTable(IWebDriver wb, ref webNodeValue wnv, string idName)
        {
            // Mevcudu yok et, yeni hazırlanacak
            wnv.tTable = null;

            IWebElement htmlTable = wb.FindElement(By.Id(idName));
            
            if (htmlTable == null) return;

            IList<IWebElement> htmlRows = htmlTable.FindElements(By.TagName("tr"));
            
            int rowCount = htmlRows.Count;
            int colCount = 0;
            int pos = 0;
            string name = "";
            string value = "";
            string dtyHtml = "";

            tTable _tTable = new tTable();
            
            for (int i = 1; i < rowCount; i++)
            {
                IWebElement hRow = htmlRows[i];

                name = hRow.GetAttribute("name");

                if (name != "pages")
                {
                    tRow _tRow = new tRow();

                    IList<IWebElement> htmlCols = hRow.FindElements(By.TagName("td"));
                    colCount = htmlCols.Count;

                    for (int i2 = 0; i2 < colCount; i2++)
                    {
                        value = "";

                        IWebElement hCol = htmlCols[i2];

                        // <input name="dgListele$ctl04$ab" id="dgListele_ctl04_chkOnayBekliyor" type="radio" checked="checked" value="chkOnayBekliyor">
                        // <input name="dgListele$ctl04$ab" id="dgListele_ctl04_chkOnayDurum" type="radio" value="chkOnayDurum">
                        dtyHtml = hCol.GetAttribute("innerHTML");

                        if (dtyHtml.IndexOf("type=\"radio\"") > -1)
                        {
                            //hCol.Click();  get sırasında neden click yapmışım ?
                        }

                        if (hCol.Text != null)
                        {
                            value = hCol.Text.Trim();
                        }
                        else
                        {
                            // <img onmouseover="this.src="/images/toolimages/open_kucuk_a.gif";this.style.cursor="hand";" 
                            // onmouseout="this.src="/images/toolimages/open_kucuk.gif";this.style.cursor="default";" 
                            // onclick="fnIslemSec("99969564|01/08/2017|NMZVAN93C15010059");" 
                            // src="/images/toolimages/open_kucuk.gif">

                            pos = -1;
                            dtyHtml = hCol.GetAttribute("outerHTML");
                            pos = dtyHtml.IndexOf("onclick");

                            // <td align="center" style="width: 30px;">
                            // <a href="javascript:__doPostBack('dgIstekOnaylanan','Select$0')">
                            // <img title="Aç" onmouseover="this.src='/images/toolimages/open_kucuk_a.gif';this.style.cursor='pointer';" onmouseout="this.src='/images/toolimages/open_kucuk.gif';this.style.cursor='default';" src="/images/toolimages/open_kucuk.gif">
                            // </a></td>                        

                            if (pos == -1) // 
                                pos = dtyHtml.IndexOf("__doPostBack(");

                            if (pos > -1)
                            {
                                //   /images/toolimages/open_kucuk.gif

                                //html = hCol.OuterHtml.Remove(0, hCol.OuterHtml.IndexOf(" src=") + 6);
                                //value = html.Remove(html.IndexOf(">") - 1);
                                value = dtyHtml.Remove(0, dtyHtml.IndexOf(" src=") + 6);
                                value = value.Remove(dtyHtml.IndexOf(">") - 1);
                            }
                            else value = "";
                        }

                        // Add column
                        tColumn _tColumn = new tColumn();
                        _tColumn.value = value;
                        _tRow.tColumns.Add(_tColumn);
                    }

                    // Add row
                    if (_tRow.tColumns.Count > 0)
                    {
                        if (_tRow.tColumns[0].value != "pages")
                            _tTable.tRows.Add(_tRow);
                    }
                }

            }
            // Add table
            wnv.tTable = _tTable;

/*
            // Grab the table
            WebElement table = driver.findElement(By.id("searchResultsGrid"));

            // Now get all the TR elements from the table
            List<WebElement> allRows = table.findElements(By.tagName("tr"));

            // And iterate over them, getting the cells
            for (WebElement row : allRows)
            {
                List<WebElement> cells = row.findElements(By.tagName("td"));
                for (WebElement cell : cells)
                {
                    // Do something with the cell
                }
            }
*/


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
            //HtmlElementCollection htmlTablePages_ = htmlTable.GetElementsByTagName("table");

            //int tableCount = htmlTablePages_.Count;
            //int rowCount = 0;
            //int colCount = 0;

            //for (int i = 0; i < tableCount; i++)
            //{
            //    if ((htmlTablePages_[i].InnerHtml.IndexOf("javascript:__doPostBack(") > -1) &&
            //        (htmlTablePages_[i].InnerHtml.IndexOf("Page$") > -1))
            //    {
            //        HtmlElement hTable = htmlTablePages_[i];
            //        HtmlElementCollection htmlRows = hTable.GetElementsByTagName("tr");
            //        rowCount = htmlRows.Count;

            //        for (int i2 = 0; i2 < rowCount; i2++)
            //        {
            //            HtmlElement hRow = htmlRows[i2];
            //            HtmlElementCollection htmlCols = hRow.GetElementsByTagName("td");
            //            colCount = htmlCols.Count;

            //            for (int i3 = 0; i3 < colCount; i3++)
            //            {

            //                // sayfa 1 için durum
            //                // hcol.InnerHtml = <span>1</span>
            //                // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$2')">2</a>
            //                // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$3')">3</a>
            //                //
            //                // sayfa 2 için durum
            //                // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$1')">1</a>
            //                // hcol.InnerHtml = <span>2</span>
            //                // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$3')">3</a>
            //                //
            //                // sayfa 3 için durum
            //                // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$1')">1</a>
            //                // hcol.InnerHtml = <a href="javascript:__doPostBack('dgIstekOnaylanan','Page$2')">2</a>
            //                // hcol.InnerHtml = <span>3</span>

            //                HtmlElement hCol = htmlCols[i3];

            //                if (hCol.InnerHtml.IndexOf("<span>") > -1)
            //                    this.selectedTablePageNo = i3 + 1;
            //            }

            //            if (hRow.TagName == "TR")
            //            {
            //                hRow.InnerText = "pages";
            //                hRow.Name = "pages";
            //            }
            //        }
            //    }
            //}

            //if (tableCount > 0)
            //    onay = true;

            /// burası seleniuma göre yeniden düznlenecek
            onay = true;
            /// this.selectedTablePageNo = 1;

            return onay;
        }

        #endregion getHtmlTable

        #region postHtmlTable
        // database üzerindeki tabloyu bul ve gönderilecek colums/kolonları oku 
        // ve okunan bu bu colums/kolonları htmlTable anahtar value ile post için gönder
        private void postHtmlTable(IWebDriver wb, ref webNodeValue wnv, string idName)
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
        private void postHtmlTable_(IWebDriver wb, ref webNodeValue wnv, string idName)
        {
            Int16 _keyColumnNo = wnv.tTableColNo;
            string _keyValue = wnv.keyValue;
            tTable _tTable = wnv.tTable;

            IWebElement htmlTable = wb.FindElement(By.Id(idName));
            //HtmlElement htmlTable = wb.Document.GetElementById(idName);

            if (htmlTable == null) return;

            //HtmlElementCollection htmlRows = htmlTable.GetElementsByTagName("tr");
            IList<IWebElement> htmlRows = wb.FindElements(By.TagName("tr"));
            int rowCount = htmlRows.Count;
            int colCount = 0;

            string dtyValue = "";

            bool onay = false;
            for (int i = 1; i < rowCount; i++)
            {
                //HtmlElement hRow = htmlRows[i];
                //HtmlElementCollection htmlCols = hRow.GetElementsByTagName("td");

                IWebElement hRow = htmlRows[i];
                IList<IWebElement> htmlCols = hRow.FindElements(By.TagName("td"));

                colCount = htmlCols.Count;

                onay = false;
                for (int i2 = 0; i2 < colCount; i2++)
                {
                    // eşleştirme yapacağımız kolon ise
                    if (_keyColumnNo == i2)
                    {
                        //HtmlElement hCol = htmlCols[i2];
                        IWebElement hCol = htmlCols[i2];

                        dtyValue = hCol.Text.Trim();

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
        private void postColumsValue_(IWebDriver wb, IList<IWebElement> htmlCols, tTable _tTable)
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
                        //HtmlElement hCol = htmlCols[i2];
                        IWebElement hCol = htmlCols[i2];

                        //<input name="dgListele$ctl04$ab" id="dgListele_ctl04_chkOnayBekliyor" type="radio" checked="checked" value="chkOnayBekliyor">
                        //<input name="dgListele$ctl04$ab" id="dgListele_ctl04_chkOnayDurum" type="radio" value="chkOnayDurum">

                        dtyHtml = hCol.GetAttribute("innerHTML");

                        if (dtyHtml.IndexOf("type=\"radio\"") > -1)
                        {
                            //dtyValue = hCol.Children[0].GetAttribute("value");
                            dtyIdName = hCol.GetAttribute("id");//  Children[0].GetAttribute("id");
                            dtyType = hCol.GetAttribute("type");  //Children[0].GetAttribute("type");

                            if ((dtyType == "radio") && (_dbValue == "True"))
                            {
                                //wb.Document.GetElementById(dtyIdName).InvokeMember("click");
                                wb.FindElement(By.Id(dtyIdName)).Click();
                                break;
                            }
                        }

                        if (dtyHtml.ToLower().IndexOf("selectedindex=") > -1)
                        {
                            //hCol.SetAttribute("selectedindex", _dbValue);

                            // denemedim çalışmı çalışmaz mı bilmiyorum
                            IJavaScriptExecutor js = (IJavaScriptExecutor)wb;
                            js.ExecuteScript("arguments[0].selectedindex = '"+ _dbValue + "';", hCol);

                            //v.SQL = v.SQL + v.ENTER + myNokta + " set selectedindex : " + _dbValue;
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


                            //dtyIdName = hCol.Children[0].GetAttribute("id");
                            dtyIdName = hCol.GetAttribute("id");
                            if (dtyIdName != "")
                            {
                                //Seleniuma göre düzenle
                                ////wb.Document.GetElementById(dtyIdName).SetAttribute("value", _dbValue);

                                //v.SQL = v.SQL + v.ENTER + myNokta + " set value : " + _dbValue;
                                break;
                            }
                        }

                        if (dtyHtml.IndexOf("type=\"radio\"") == -1)
                        {
                            //Seleniuma göre düzenle
                            //hCol.SetAttribute("value", _dbValue);
                            //v.SQL = v.SQL + v.ENTER + myNokta + " set value : " + _dbValue;
                            break;
                        }
                    }
                }
            }
        }

        #endregion postHtmlTable

    }

}
