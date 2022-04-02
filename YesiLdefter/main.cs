using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using Tkn_Events;
using Tkn_Ftp;
using Tkn_InputPanel;
using Tkn_Menu;
using Tkn_Registry;
using Tkn_SQLs;
using Tkn_Starter;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class main : XtraForm
    {
        #region tanımlar

        //tEvents ev = new tEvents();
        tEventsForm evf = new tEventsForm();
        tToolBox t = new tToolBox();
        tInputPanel ip = new tInputPanel();
        tMenu mn = new tMenu();

        
        string TableIPCODE = string.Empty;

        Control toolboxControl1 = null;
        Control ribbonControl1 = null;
        object skinUserFirm = null;

        DevExpress.XtraBars.Ribbon.RibbonControl ribbon = null;

        DevExpress.XtraBars.BarStaticItem barMesajlar = null;
        DevExpress.XtraBars.BarButtonItem barButtonGuncelleme = null;

        DevExpress.XtraBars.BarEditItem mainProgressBar = null;
        DevExpress.XtraBars.BarEditItem barEditItemCari = null;

        DevExpress.XtraBars.BarEditItem barPrjConn = null;
        DevExpress.XtraBars.BarEditItem barMSConn = null;

        /// <summary>
        /// 
        /// </summary>        
        CihazLogs cl = new CihazLogs();
        //DevExpress.XtraGrid.Views.Tile.TileView cihazTileView = null;
        /*
        System.Windows.Forms.Timer timerCihazLogGetIcmal = null;
        DataSet dsCihazLogIcmal = null;
        DataNavigator dNCihazLogIcmal = null;
        //bool cihazLogIcmalRefresh = false;
        //int cihazLogIcmalPos = 0;
        */

        #endregion

        private void main_Load(object sender, EventArgs e)
        {

        }

        public main()
        {

            // ... 
            // 
            System.Globalization.CultureInfo tr = new System.Globalization.CultureInfo("tr-TR");

            System.Threading.Thread.CurrentThread.CurrentCulture = tr;

            #region appOpenSetDefaaultSkin
            WindowsFormsSettings.EnableFormSkins();
            //UserLookAndFeel.Default.SetSkinStyle("VS2010");
            //UserLookAndFeel.Default.SetSkinStyle(SkinSvgPalette.Office2019Colorful.Forest);
            //UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Whiteprint);
            //UserLookAndFeel.Default.SetSkinStyle(SkinSvgPalette.Bezier.Grasshopper);
            UserLookAndFeel.Default.StyleChanged += Default_StyleChanged;
            #endregion

            #region preparing mainForm

            v.mainForm = this;
            v.Wait_Caption = v.Wait_Caption.PadRight(100);
            //v.Wait_Desc_ProgramYukleniyor = v.Wait_Desc_ProgramYukleniyor.PadRight(100);
            //v.Wait_Desc_ProgramYukDevam = v.Wait_Desc_ProgramYukDevam.PadRight(100);
            //v.Wait_Desc_DBBaglanti = v.Wait_Desc_DBBaglanti.PadRight(100);

            //SplashScreenManager.ShowForm(this, typeof(DevExpress.XtraWaitForm.AutoLayoutDemoWaitForm), true, true, false);

            t.WaitFormOpen(v.mainForm, "yesiLdefter hazırlanıyor ...");
            v.SP_OpenApplication = true;

            // formun kendisi
            InitializeComponent();

            // ana ekranının hazırlanması
            // yani menuler burada hazırlanıyor
            using (tMainForm f = new tMainForm())
            {
                f.preparingMainForm(this);

                //f.preparingDockPanel(this, "SEK/CEV/prcCihazLogGetIcmal.Icmal_L01");
            }

            this.Load += new System.EventHandler(evf.myForm_Load);
            this.Shown += new System.EventHandler(evf.myForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(mainForm_KeyDown);
            //this.Activated += new System.EventHandler(mainForm_Activated);
            //this.Deactivate += new System.EventHandler(mainForm_Deactivate);
            this.KeyPreview = true;

            #endregion mainForm

            #region Starter

            t.WaitFormOpen(v.mainForm, "Starter ...");

            using (tStarter s = new tStarter())
            {
                s.InitStart();
            }

            #endregion

            #region UserLOGIN

            if (v.SP_UserLOGIN)
            {
                // login işlemleri
                Login();
                // application set skins
                t.getUserLookAndFeelSkins();
            }

            #endregion

            if (v.tMainFirm.MenuCode == "SEK/CEV/AYR/MAINTOP")
            {
                System.Windows.Forms.Timer timerCihazLogGetIcmal = new System.Windows.Forms.Timer(this.components);
                cl.CihazLog(this, timerCihazLogGetIcmal);
            }


            #region -- açılışın sonu

            chechkedPaths();

            v.SP_OpenApplication = false;

            v.IsWaitOpen = false;
            t.WaitFormClose();

            //SplashScreenManager.CloseForm(false);
            v.SQL = "";
            #endregion
        }

        #region Login

        void Login()
        {
            if (v.SP_UserLOGIN)
            {
                setMenuItems();

                //timer_Kullaniciya_Mesaj_Varmi.Enabled = true;

                t.WaitFormOpen(v.mainForm, "ManagerDB Connection...");
                t.Db_Open(v.active_DB.managerMSSQLConn);

                if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                {
                    t.WaitFormOpen(v.mainForm, "ProjectDB MSSQL Connection...");
                    t.Db_Open(v.active_DB.projectMSSQLConn);
                }
                
                t.WaitFormOpen(v.mainForm, "SysTypes Load ...");
                t.SYS_Types_Read();

                //t.WaitFormOpen(v.mainForm, "Read : SysGlyph ...");
                //t.SYS_Glyph_Read();

                preparingMenus();

                versionChecked();

                setMainFormCaption();
            }
        }

        void setMainFormCaption()
        {
            // ---
            this.Text = "Üstad'ın yeşiL defteri   Ver : " +
                v.tExeAbout.activeVersionNo.Substring(2, 6) + "." +
                v.tExeAbout.activeVersionNo.Substring(9, 4) +
                "    [ " + v.tMainFirm.FirmId.ToString() + " : " + v.tMainFirm.FirmShortName + " ] ";
        }

        void chechkedPaths()
        {
            //string subPath = "Temp"; // Your code goes here
            //bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));

            //if (!exists)
            System.IO.Directory.CreateDirectory(v.EXE_TempPath);
            System.IO.Directory.CreateDirectory(v.EXE_ScriptsPath);

        }

        #endregion

        #region preparingMenus

        void preparingMenus()
        {
            short menuType = mn.getCreateMenuType(v.tMainFirm.MenuCode);

            // kullanıcının seçtiği firmanın kullandığı menü
            //
            v.tMainFirm.MenuCodeOld = v.tMainFirm.MenuCode;
            //
            if (menuType == 102)
            {
                mn.Create_Menu(ribbon, v.tMainFirm.MenuCode, "");
                mn.alterRibbon(ribbon, "");

                ribbon.Minimized = false;
            }
            if (menuType == 109)
            {
                mn.Create_Menu(toolboxControl1, v.tMainFirm.MenuCode, "");
                mn.alterRibbon(ribbon, "yesiL");

            }

            // Kullanıcıların Ortak Menüsü
            //
            mn.Create_Menu(ribbon, "UST/PMS/PMS/PublicUser", "");

            if ((v.tUser.UserDbTypeId == 1) || // yazılım
                (v.tUser.UserDbTypeId == 21))  // kurucu
            {
                
                t.WaitFormOpen(v.mainForm, "Menu Create ...");
                mn.Create_Menu(ribbon, "UST/PMS/PMS/MsV3Menu", "");
            }

            setMenuItems();
        }

        void setMenuItems()
        {
            toolboxControl1 = t.Find_Control(this, "toolboxControl1");
            ribbonControl1 = t.Find_Control(this, "ribbonControl1");

            ribbon = ((DevExpress.XtraBars.Ribbon.RibbonControl)ribbonControl1);
            
            foreach (var item in ribbon.Items)
            {
                if (item.GetType().ToString() == "DevExpress.XtraBars.BarStaticItem")
                {
                    if ((item.ToString() == "Mesajlar") &&
                        (barMesajlar == null))
                    {
                        barMesajlar = (DevExpress.XtraBars.BarStaticItem)item;
                        barMesajlar.Caption = "";
                    }
                }
                
                if (item.GetType().ToString() == "DevExpress.XtraBars.BarButtonItem")
                {
                    if ((((DevExpress.XtraBars.BarButtonItem)item).Name.ToString() == "barButtonGuncelleme") &&
                        (barButtonGuncelleme == null))
                    {
                        barButtonGuncelleme = (DevExpress.XtraBars.BarButtonItem)item;
                        barButtonGuncelleme.ItemClick += 
                            new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonGuncelleme_ItemClick);
                    }
                }

                if (item.GetType().ToString() == "DevExpress.XtraBars.BarEditItem")
                {
                    if (((DevExpress.XtraBars.BarEditItem)item).Name == "mainProgressBar")
                        mainProgressBar = (DevExpress.XtraBars.BarEditItem)item;
                    if (((DevExpress.XtraBars.BarEditItem)item).Name == "barEditItemCari")
                        barEditItemCari = (DevExpress.XtraBars.BarEditItem)item;

                    if (((DevExpress.XtraBars.BarEditItem)item).Name == "barPrjConn")
                        barPrjConn = (DevExpress.XtraBars.BarEditItem)item;
                    if (((DevExpress.XtraBars.BarEditItem)item).Name == "barMSConn")
                        barMSConn = (DevExpress.XtraBars.BarEditItem)item;
                }
            }
            
            //string ss = "";

            int i3 = ribbon.Pages.Count;
            int i5 = 0;
            for (int i2 = 0; i2 < i3; i2++)
            {
                i5 = ribbon.Pages[i2].Groups.Count;
                
                for (int i4 = 0; i4 < i5; i4++)
                {
                    foreach (var item in ribbon.Pages[i2].Groups[i4].ItemLinks)
                    {
                        //ss = ss + item.GetType().ToString() + v.ENTER;

                        if (item.GetType().ToString() == "DevExpress.XtraBars.BarLargeButtonItemLink")
                        {
                            if (((DevExpress.XtraBars.BarLargeButtonItemLink)item).Item.Name.ToString() == "btnExeCompress")
                            {
                                //MessageBox.Show("oley");
                                ((DevExpress.XtraBars.BarLargeButtonItemLink)item).Item.ItemClick 
                                      += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExeCompress_ItemClick);
                            }
                            if (((DevExpress.XtraBars.BarLargeButtonItemLink)item).Item.Name.ToString() == "btnExeFtpUpload")
                            {
                                ((DevExpress.XtraBars.BarLargeButtonItemLink)item).Item.ItemClick
                                      += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExeUpload_ItemClick);
                            }
                            //btnFirmList
                            if (((DevExpress.XtraBars.BarLargeButtonItemLink)item).Item.Name.ToString() == "btnFirmList_FOPEN_IP") 
                            {
                                ((DevExpress.XtraBars.BarLargeButtonItemLink)item).Item.ItemClick
                                      += new DevExpress.XtraBars.ItemClickEventHandler(this.btnFirmList_ItemClick);
                            }
                            //btnTEST (her türlü test kodları için bu eventi kullanabilirsin)
                            if (((DevExpress.XtraBars.BarLargeButtonItemLink)item).Item.Name.ToString() == "btnTEST")
                            {
                                ((DevExpress.XtraBars.BarLargeButtonItemLink)item).Item.ItemClick
                                      += new DevExpress.XtraBars.ItemClickEventHandler(this.btnTEST_ItemClick);
                            }
                        }
                    }
                    
                }
            }
            //MessageBox.Show(ss);

            //((DevExpress.XtraToolbox.ToolboxControl)toolboxControl1).SelectedGroup = 0;
            
            // bunu kullanıcının seçmesini sağla
            //((DevExpress.XtraToolbox.ToolboxControl)toolboxControl1).SelectedGroupIndex = 0;
        }

        #endregion

        #region buttonsClick
        //
        private void ribbon_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tToolBox t = new tToolBox();

            #region Tanımlar 
            string TableIPCode = string.Empty;
            string ButtonName = string.Empty;
            //string Button_Click_Type = string.Empty;
            string myFormLoadValue = string.Empty;
            #endregion Tanımlar

            if (e.Item.GetType().ToString() == "DevExpress.XtraBars.BarLargeButtonItem")
            {
                ButtonName = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Name.ToString();
                TableIPCode = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).AccessibleName;
                myFormLoadValue = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).AccessibleDescription;

                // AccessibleDescription NEDEN boşalıyor sebebi bulunamadı
                if ((t.IsNotNull(myFormLoadValue) == false) &&
                    ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag != null)
                    myFormLoadValue = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag.ToString();
            }

            if (e.Item.GetType().ToString() == "DevExpress.XtraBars.BarButtonItem")
            {
                ButtonName = ((DevExpress.XtraBars.BarButtonItem)e.Item).Name.ToString();
                TableIPCode = ((DevExpress.XtraBars.BarButtonItem)e.Item).AccessibleName;
                myFormLoadValue = ((DevExpress.XtraBars.BarButtonItem)e.Item).AccessibleDescription;

                // AccessibleDescription NEDEN boşalıyor sebebi bulunamadı
                if ((t.IsNotNull(myFormLoadValue) == false) &&
                    ((DevExpress.XtraBars.BarButtonItem)e.Item).Tag != null)
                    myFormLoadValue = ((DevExpress.XtraBars.BarButtonItem)e.Item).Tag.ToString();
            }

            //Form tForm = t.Find_Form(sender);

            //ButtonClickRUN(tForm, ButtonName, TableIPCode, myFormLoadValue);

        }

               

        //btnFirmList
        private void btnFirmList_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // ms_UserFirmList formundaki 
            // btn_FirmListSec sonrası
            //
            t.AllFormsClose();
            //
            setMainFormCaption();

            // kullanıcının seçtiği firmanın kullandığı menü
            //
            if (v.tMainFirm.MenuCodeOld != v.tMainFirm.MenuCode)
            {
                // değişien firmanın menüsü 
                v.tMainFirm.MenuCodeOld = v.tMainFirm.MenuCode;

                mn.Create_Menu(toolboxControl1, v.tMainFirm.MenuCode, "");
            }

            t.getUserLookAndFeelSkins();
        }

        //btnTEST
        private void btnTEST_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /// burada istediğin test kodunu çalıştırabilirsin
            /// işin bitince yazdıklarını silersen iyi olur
            ///

            //MessageBox.Show(UserLookAndFeel.Default.SkinName.ToString());
            MessageBox.Show(
                " SkinName : " + UserLookAndFeel.Default.SkinName.ToString() + " // " + v.ENTER +
                " ActiveSkinName : " + UserLookAndFeel.Default.ActiveSkinName.ToString() + " // " + v.ENTER +
                " ActiveStyle : " + UserLookAndFeel.Default.ActiveStyle.ToString() + " // " + v.ENTER +
                " ActiveSvgPaletteName : " + UserLookAndFeel.Default.ActiveSvgPaletteName.ToString()
                );


        }

        #endregion buttonsClick

        #region Compress

        private void btnExeCompress_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CompressFile();
        }

        public bool CompressFile()
        {
            bool onay = false;
            DirectoryInfo di = new DirectoryInfo(v.tExeAbout.activePath);

            foreach (FileInfo fi in di.GetFiles())
            {
                //for specific file 
                if (fi.ToString() == v.tExeAbout.activeExeName)
                {
                    onay = Compress(fi);
                    break;
                }
            }

            if (onay)
            {
                v.Kullaniciya_Mesaj_Var = "Exe paketlendi ...";
                MessageBox.Show("Exe Paketlendi ...");
            }

            return onay;
        }

        public bool Compress(FileInfo fi)
        {
            bool onay = false;

            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Prevent compressing hidden and 
                // already compressed files.
                if ((File.GetAttributes(fi.FullName)
                    & FileAttributes.Hidden)
                    != FileAttributes.Hidden & fi.Extension != ".gz")
                {
                    //string myFileName = fi.FullName + ".gz";
                    string myFileName = fi.FullName.Remove(fi.FullName.IndexOf(fi.Extension), fi.Extension.Length) + ".gz";

                    //exe kendisini compress yapacaksa
                    if (fi.FullName.IndexOf(v.tExeAbout.activeExeName) > -1)
                    {
                        /// activeVersionNo : 20190329_1545
                        /// activeFileName  : YesiLdefter.exe >> YesiLdefter_20190329_1545.gz  şeklinde olacak
                        /// activePath      : E:\TekinOzel\yesiLdefter\yesiLdefterV3\YesiLdefter\bin\Debug\YesiLdefter_20190329_1553.gz
                        /// dikkat : kafan karışmasın şuan çalıştığın exeyi ftpye atmak istiyorsun
                        /// bu nedenle active olandan faydalanıyor onu new diye ftp göndeririyoruz

                        v.tExeAbout.newVersionNo = v.tExeAbout.activeVersionNo;
                        v.tExeAbout.newFileName = v.tExeAbout.activeExeName;

                        v.tExeAbout.newPacketName =
                            //v.tExeAbout.activeExeName.Remove(v.tExeAbout.activeExeName.IndexOf(fi.Extension), fi.Extension.Length) + "_" +
                            v.tExeAbout.activeExeName.Remove(v.tExeAbout.activeExeName.IndexOf(".exe"), 4) + "_" +
                            v.tExeAbout.activeVersionNo + ".gz";

                        v.tExeAbout.newPathFileName =
                            v.tExeAbout.activePath + "\\" + v.tExeAbout.newPacketName;
                        // bu da aynı sonucu veriyor 
                        //fi.FullName.Remove(fi.FullName.IndexOf(fi.Extension), fi.Extension.Length) + "_" + v.tExeAbout.activeVersionNo + ".gz";

                        myFileName = v.tExeAbout.newPathFileName;
                    }

                    // Create the compressed file.
                    using (FileStream outFile = File.Create(myFileName))
                    {
                        using (GZipStream Compress = new GZipStream(outFile, CompressionMode.Compress))
                        {
                            // Copy the source file into 
                            // the compression stream.
                            inFile.CopyTo(Compress);

                            onay = true;

                            //Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
                            //    fi.Name, fi.Length.ToString(), outFile.Length.ToString());
                        }
                    }
                }
            }

            return onay;
        }

        #endregion

        #region ftpUpload

        private void btnExeUpload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool onay = t.ftpUpload();

            if (onay)
            {
                tSQLs sqls = new tSQLs();
                DataSet ds = new DataSet();
                string sql = sqls.SQL_SYS_UPDATES_INSERT();
                if (t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref sql, "", "SYS_UPDATES"))
                    MessageBox.Show("Exe Ftp'ye yüklendi ...");
            }
        }

        #endregion ftpUpload

        #region ftpDownload 

        


        private void barButtonGuncelleme_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool onay = false;
            //
            onay = t.ftpDownload(v.tExeAbout.ftpPacketName);
            //
            if (onay)
                onay = ExtractFile();
            //
            if (onay)
                onay = fileNameChange();

            if (onay)
            {
                LaunchCommandLineApp();
            }

        }

        

        private void LaunchCommandLineApp()
        {
            // For the example
            //const string ex1 = "C:\\";
            //const string ex2 = "C:\\Dir";

            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = v.tExeAbout.activeExeName;   //"dcm2jpg.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //startInfo.Arguments = "-f j -o \"" + ex1 + "\" -z 1.0 -s y " + ex2;


            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    //exeProcess.WaitForExit();
                    Application.Exit();
                }
            }
            catch
            {
                // Log error.
            }
        }

        #endregion

        #region Extract

        public bool ExtractFile()
        {

            bool onay = false;

            DirectoryInfo di = new DirectoryInfo(v.tExeAbout.activePath);

            foreach (FileInfo fi in di.GetFiles())
            {
                //for specific file 
                if (fi.ToString() == v.tExeAbout.ftpPacketName)
                {
                    onay = Extract(fi);
                    break;
                }
            }

            return onay;
        }

        public bool Extract(FileInfo fi)
        {
            bool onay = false;

            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                string currentFileName = fi.FullName;
                string newFileName = fi.FullName.Remove(fi.FullName.IndexOf(fi.Extension), fi.Extension.Length) + ".exe";

                using (FileStream deCompressedFile = File.Create(newFileName))
                {
                    using (GZipStream deCompress = new GZipStream(inFile, CompressionMode.Decompress))
                    {
                        if (fi.Extension == ".gz")
                        {
                            deCompress.CopyTo(deCompressedFile);
                            onay = true;
                        }
                    }
                }
            }

            return onay;
        }

        public bool fileNameChange()
        {
            bool onay = false;

            try
            {
                // mevcut exe yi yedekleyelim

                // YesiLdefter.exe  >>>  YesiLdefter_20190328_2205.exe  ismine çevrilecek
                string newFileName = v.tExeAbout.activeExeName.Remove(v.tExeAbout.activeExeName.IndexOf(".exe"), 4) + "_" +
                                     v.tExeAbout.activeVersionNo + ".exe";
                // newFileName varsa sil
                File.Delete(newFileName);
                // YesiLdefter.exe  >>>  YesiLdefter_20190328_2205.exe
                File.Move(v.tExeAbout.activeExeName, newFileName);

                // ftp den inen exe nin adını değiştirelim

                // YesiLdefter_20190331_1843.gz   >>> YesiLdefter_20190331_1843.exe
                newFileName = v.tExeAbout.ftpPacketName.Remove(v.tExeAbout.ftpPacketName.IndexOf(".gz"), 3) + ".exe";

                // YesiLdefter_20190331_1843.exe  >>> YesiLdefter.exe
                File.Move(newFileName, v.tExeAbout.activeExeName);

                onay = true;
            }
            catch (Exception e1)
            {
                MessageBox.Show("Hata : Dosya isimleri değiştirilirken problem oluştu ..." + v.ENTER2 + e1.Message);
                throw;
            }
            return onay;
        }

        #endregion

        #region versionChecked
        private void versionChecked()
        {
            string activeVer = v.tExeAbout.activeVersionNo.Replace("_","");
            string ftpVer = v.tExeAbout.ftpVersionNo.Replace("_","");

            if (t.myLong(activeVer) < t.myLong(ftpVer))
            {
                 barButtonGuncelleme.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
        }
        #endregion

        #region skinChanged

        private void Default_StyleChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            if (v.sp_activeSkinName != "READ")
            {
                if (UserLookAndFeel.Default.ActiveSkinName.ToString().IndexOf(v.sp_deactiveSkinName) > -1) return;
                if (UserLookAndFeel.Default.ActiveSvgPaletteName.ToString().IndexOf(v.sp_deactiveSkinName) > -1) return;
            }
            else
                v.sp_activeSkinName = "";

            // eğer yeni load oluyarsa buradaki işlemler çalışmasın
            if (skinUserFirm != null) return;

            setUserLookAndFeelSkins();
        }

        private void setUserLookAndFeelSkins()
        {
            var item = UserLookAndFeel.Default.ActiveSkinName.ToString();

            if (UserLookAndFeel.Default.ActiveSvgPaletteName.ToString() != "")
            {
                /// object olarak set edileceği zaman
                /// UserLookAndFeel.Default.SetSkinStyle(SkinSvgPalette.Office2019Colorful.Forest);
                /// UserLookAndFeel.Default.SetSkinStyle(SkinSvgPalette.Bezier.LeafRustle);

                /// string olarak set edileceği zaman
                /// Office 2019 Colorful||Forest
                /// The Bezier||LeafRustle
                item = item + "||" + UserLookAndFeel.Default.ActiveSvgPaletteName.ToString();
            }

            tRegistry reg = new tRegistry();
            // tüm kullanıcılar için geçerli
            //reg.SetUstadRegistry("userSkin", e.Item.Value.ToString());
            // kullanıcı bazlı
            //reg.SetUstadRegistry("userSkin_" + v.tUser.UserId.ToString(), e.Item.Value.ToString());
            // kullanıcı kullandığı her firma için ayrı ayrı
            reg.SetUstadRegistry("userSkin_" + v.tUser.UserId.ToString() + "_" + v.SP_FIRM_ID.ToString(),
                                 item.ToString());
            /*
            MessageBox.Show(
               " SkinName : " + UserLookAndFeel.Default.SkinName.ToString() + " // " + v.ENTER +
               " ActiveSkinName : " + UserLookAndFeel.Default.ActiveSkinName.ToString() + " // " + v.ENTER +
               " ActiveStyle : " + UserLookAndFeel.Default.ActiveStyle.ToString() + " // " + v.ENTER +
               " ActiveSvgPaletteName : " + UserLookAndFeel.Default.ActiveSvgPaletteName.ToString() + v.ENTER2 +
               " newName : " + item.ToString()
               );
            */

            //this.Text = this.Text + ",s";
        }

        #endregion

        #region timer_

        private void timer_Mesaj_Suresi_Bitti_Tick(object sender, EventArgs e)
        {
            if ((!string.IsNullOrEmpty(v.Kullaniciya_Mesaj_Var)) |
                (barMesajlar.Caption != ""))
            {
                barMesajlar.Caption = "";
                v.Kullaniciya_Mesaj_Var = "";
                timer_Mesaj_Suresi_Bitti.Enabled = false;

            }
        }

        private void timer_Kullaniciya_Mesaj_Varmi_Tick(object sender, EventArgs e)
        {
            barMSConn.EditValue = v.SP_ConnBool_Manager;
            barPrjConn.EditValue = v.SP_ConnBool_Project;

            if (!string.IsNullOrEmpty(v.Kullaniciya_Mesaj_Var))
            {
                barMesajlar.Caption = v.Kullaniciya_Mesaj_Var;
                timer_Mesaj_Suresi_Bitti.Enabled = true;
                v.Kullaniciya_Mesaj_Var = "";

                /*
                if ((v.Kullaniciya_Mesaj_Var == v.DBRec_Insert) ||
                    (v.Kullaniciya_Mesaj_Var.IndexOf(v.DBRec_Update) > -1))
                {
                    if (v.SP_OpenApplication == false)
                    {
                        //    v.SP_OpenApplication = true;

                        SplashScreenManager.ShowForm(v.mainForm, typeof(DevExpress.XtraWaitForm.AutoLayoutDemoWaitForm), true, true, false);
                        SplashScreenManager.Default.SetWaitFormCaption(v.Wait_Caption);
                        SplashScreenManager.Default.SetWaitFormDescription(v.Kullaniciya_Mesaj_Var);

                        Thread.Sleep(500);
                        SplashScreenManager.CloseForm(false);

                        //    v.SP_OpenApplication = false;
                        v.Kullaniciya_Mesaj_Var = "";
                    }
                }
                */
            }
        }

        #endregion

        public void mainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled) return;

            tEventsForm evf = new tEventsForm();

            // sadece main form açıkken
            if (Application.OpenForms.Count == 1)
            {
                e.Handled = evf.myMenuShortKeyClick((Form)sender, e.KeyCode.ToString());

                if (e.Handled == false)
                    if ((e.Control) && (e.KeyCode != Keys.ControlKey))
                    {
                        e.Handled = evf.myMenuShortKeyClick((Form)sender, "C+" + e.KeyCode);
                    }

                if (e.Handled == false)
                    if (e.KeyCode == Keys.Escape)
                    {
                        tToolBox t = new tToolBox();
                        DialogResult cevap = t.mySoru("EXIT");
                        if (DialogResult.Yes == cevap)
                        {
                            Application.Exit();
                        }
                    }
            }
        }
               
        public void mainForm_Activated(object sender, EventArgs e)
        {
            //v.Kullaniciya_Mesaj_Var = "Form Activated";
            //this.Text = this.Text + ",a";
            /*
            v.sp_activeSkinName = "Form_Activated";
            tToolBox t = new tToolBox();
            t.getUserLookAndFeelSkins();
            */
        }

        public void mainForm_Deactivate(object sender, EventArgs e)
        {
            //MessageBox.Show("myForm_Deactivate : " + ((Form)sender).Text);
            //v.Kullaniciya_Mesaj_Var = "Form Deactivate";
            //this.Text = this.Text + ",d";
            /*
            if ((v.sp_OpenFormState != "DIALOG") &
                (v.sp_OpenFormState != "NORMAL") &
                (v.sp_OpenFormState != "CHILD"))
                UserLookAndFeel.Default.SetSkinStyle(v.sp_DeactiveSkin);
            */
            //UserLookAndFeel.Default.SetSkinStyle(v.sp_DeactiveSkinPalette);
        }

        

        //*****

        /*
        internal class CustomApplicationSettings : ApplicationSettingsBase
        {
            [UserScopedSetting(), DefaultSettingValue("Sharp")]
            public string SkinName
            {
                get { return this("SkinName").ToString(); }
                set { this("SkinName") = value; }
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            CustomApplicationSettings ApplicationSettings = new CustomApplicationSettings();
            UserLookAndFeel.Default.SkinName = ApplicationSettings.SkinName;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            ApplicationSettings.SkinName = UserLookAndFeel.Default.SkinName;
            ApplicationSettings.Save();
        }
        */
    }
}
