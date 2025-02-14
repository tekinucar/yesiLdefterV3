using DevExpress.Utils.Helpers;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.WinExplorer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Tkn_InputPanel;
using Tkn_Save;
using Tkn_ToolBox;
using Tkn_Variable;
using WIA;
using Tkn_Layout;
using System.Management;

using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Additions.Chrome;

using Spire.Doc;
using Spire.Doc.Documents;
using System.Drawing.Imaging;


namespace YesiLdefter
{
    public partial class ms_Pictures : DevExpress.XtraEditors.XtraForm
    {
        private class vImageProperties
        {
            public int width { get; set; }
            public int height { get; set; }
            public float horizontalResolation { get; set; }
            public float verticalResolation { get; set; }
            public long byteSize { get; set; }
            public decimal kbSize { get; set; }
            public System.Drawing.Imaging.ImageFormat imageFormat { get; set; }
            public string imagePath { get; set; }
            public string imageName { get; set; }

            public void Clear()
            {
                width = 0;
                height = 0;
                horizontalResolation = 0;
                verticalResolation = 0;
                byteSize = 0;
                kbSize = 0;
                imageFormat = null;
                imagePath = string.Empty;
                imageName = string.Empty;
            }
        }

        tToolBox t = new tToolBox();
        #region tanımlar
        object tFileDataTable = null;
        bool loadDrives = false;
        string currentPath = string.Empty;
        string desktopPath = string.Empty;
        string myPicturesPath = string.Empty;
        string myDocumentsPath = string.Empty;
        string imageType = "Normal";

        int cropX;
        int cropY;
        int cropWidth;
        int cropHeight;
        int startCropX;
        int startCropY;
        int autoCropWidth;
        int autoCropHeight;
        int autoDPI;
        string autoFormat = "Jpeg";

        string activeControlName = null;
        Bitmap OriginalImage = null;
        Pen cropPen;
        Pen cropPen2;
        Pen cropPen3;
        
        PictureEditViewInfo viewInfo = null;
        DevExpress.XtraEditors.HScrollBar hScrollBar = null;
        DevExpress.XtraEditors.VScrollBar vScrollBar = null;
        int zoomPercent = 0;

        vImageProperties tImageProperties = new vImageProperties();

        DataSet dsImages = null;        // dsDataTarget
        DataNavigator dNImages = null;  // dNTarget

        FilterInfoCollection fico;
        VideoCaptureDevice vcd;

        string imagesSourceFormName = "";
        string imagesSourceTableIPCode = "";
        string imagesSourceFieldName = "";
        string imagesMasterTableIPCode = "";
        string tarayiciYokMesaji = "Bilgisayarınıza bağlı tarayıcı tespit edilemedi.";
        bool IsActiveOlcek = false;
        string olcekName = "";

        bool onayWidth = false;
        bool onayHeight = false;
        bool onayDPI = false;
        bool onayKb = false;
        bool onayFormat = false;
        #endregion
        public ms_Pictures()
        {

            InitializeComponent();

            GlyphLoad();

            Files_Preparing();

            AllDrives();

            PropertiesChange();

            ImagesMasterTableIPCode(v.tResimEditor);

            AddScannerList();
            
            AddWebCamList();

            printImageProperties();

            startCropX = 0;
            startCropY = 0;
                        
            cropPen = new Pen(Color.Black, 1);
            cropPen.DashStyle = DashStyle.DashDotDot;

            cropPen2 = new Pen(Color.Red, 1);
            cropPen2.DashStyle = DashStyle.DashDotDot;

            cropPen3 = new Pen(Color.MediumSpringGreen, 4);
            cropPen3.DashStyle = DashStyle.DashDotDot;

            pictureEdit1.BackColor = System.Drawing.Color.WhiteSmoke;
        }

        
        void AddWebCamList()
        {
            Int16 camNo = 1; 
            fico = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (AForge.Video.DirectShow.FilterInfo item in fico)
            {
                imageComboBox_WebCam.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(item.Name, (short)camNo, -1));
                camNo++;
            }
            if (camNo == 1) // kamera bulamadıysa
                imageComboBox_WebCam.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Bilgisayarınıza bağlı kamera tespit edilemedi.", (short)1, -1));

            barEditItem_WebCam.EditValue = (short)1;
            //imageComboBox_WebCam.Items[0].Value = 1;
        }
        void AddScannerList()
        {
            Int16 scanNo = 1;
            string firstScanner = "";
            DeviceManager deviceManager = new DeviceManager();
            foreach (DeviceInfo deviceInfo in deviceManager.DeviceInfos)
            {
                if (deviceInfo.Type == WiaDeviceType.ScannerDeviceType)
                {
                    //Console.WriteLine("Scanner found: " + deviceInfo.Properties["Name"].get_Value());
                    //imageComboBox_Tarayici.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(deviceInfo.Properties["Name"].get_Value(), (short)scanNo, -1));
                    itemComboBox_Tarayici1.Items.Add(new DevExpress.XtraEditors.Controls.ComboBoxItem(deviceInfo.Properties["Name"].get_Value()));
                    if (firstScanner == "")
                        firstScanner = deviceInfo.Properties["Name"].get_Value();
                    scanNo++;
                }
            }

            //if (scanNo == 1) // tarayıcı bulamadıysa
            //    imageComboBox_Tarayici.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(tarayiciYokMesaji, (short)1, -1));

            barEditItem_Tarayici.EditValue = firstScanner;
        }
        private void ms_Pictures_Shown(object sender, EventArgs e)
        {
            //
            //pictureEdit1. için kaynak olan masterTableIPCode
            if (dsImages != null)
            {
                Control cntrl = t.Find_Control_View(this, this.imagesMasterTableIPCode);

                if (t.IsNotNull(v.con_ImagesMasterDataSet))
                {
                    //dsImages = v.con_ImagesMasterDataSet.Clone();
                    dsImages = v.con_ImagesMasterDataSet.Copy();
                    dNImages.DataSource = dsImages.Tables[0];

                    if ((cntrl != null) &&
                        (dsImages.Tables.Count > 0))
                    {
                        if (cntrl.GetType().ToString() == "DevExpress.XtraGrid.GridControl")
                            ((DevExpress.XtraGrid.GridControl)cntrl).DataSource = dsImages.Tables[0];

                        if (cntrl.GetType().ToString() == "DevExpress.XtraVerticalGrid.VGridControl")
                            ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).DataSource = dsImages.Tables[0];

                        if (cntrl.GetType().ToString() == "DevExpress.XtraDataLayout.DataLayoutControl")
                            ((DevExpress.XtraDataLayout.DataLayoutControl)cntrl).DataSource = dsImages.Tables[0];

                        if (cntrl.GetType().ToString() == "DevExpress.XtraTreeList.TreeList")
                            ((DevExpress.XtraTreeList.TreeList)cntrl).DataSource = dsImages.Tables[0];

                        if (cntrl.GetType().ToString() == "DevExpress.XtraEditors.DataNavigator")
                            ((DevExpress.XtraEditors.DataNavigator)cntrl).DataSource = dsImages.Tables[0];
                    }
                }

                dNImages.PositionChanged += new System.EventHandler(dataNavigator_PositionChanged);

                // işlem yapılacak resimin row unu bul ve göster
                int i = -1;
                if (t.IsNotNull(dsImages))
                    i = t.Find_GotoRecord(dsImages, "LkpFieldName", this.imagesSourceFieldName);
                dNImages.Position = i;

                // pictureEdit1 database bağla
                pictureEdit1.DataBindings.Add(new Binding("EditValue", dsImages.Tables[0], "LkpImage"));
                // resim hakkındaki bilgiler okunsun ekrana basılsın
                dataNavigator_PositionChanged((object)dNImages, null);

                // Database Kaydet butonunu ayarla
                cntrl = null;
                string[] controls = new string[] { };
                cntrl = t.Find_Control(this, "simpleButton_ek1", this.imagesMasterTableIPCode, controls);

                if (cntrl != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_DatabaseKaydet_ItemClick);
                    ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("KAYDET16");
                }
            }
        }
        void ImagesMasterTableIPCode(vResimEditor tResimEditor)
        {
            #region IP_VIEW_TYPE
            // * 1,  'Data View'
            // * 2,  'Kriter View'
            // * 3,  'Kategori View'
            // * 4,  'HGS View'
            // IPdataType_DataView = 1;
            // IPdataType_Kriterler = 2;
            // IPdataType_Kategori = 3;
            // IPdataType_HGSView = 4;
            #endregion IP_VIEW_TYPE

            /// kaynak olacak TableIPCode var ise
            /// 
            if (t.IsNotNull(tResimEditor.listTableIPCode))
            {
                tInputPanel ip = new tInputPanel();
                ip.Create_InputPanel(this, groupControlList, tResimEditor.listTableIPCode, 1, true);
            }
            /// kaynak olacak FormCode var ise
            /// 
            if (t.IsNotNull(tResimEditor.formCode))
            {
                tLayout l = new tLayout();
                l.Create_Layout(this, tResimEditor.formCode, groupControlList);
            }
            else groupControlList.Visible = false;

            /// Images ve datası olan FormCode var ise
            /// 
            if (t.IsNotNull(tResimEditor.imagesMasterFormCode))
            {
                tLayout l = new tLayout();
                l.Create_Layout(this, tResimEditor.imagesMasterFormCode, groupControlKisi);
            }

            /// sadece images listesi olacak TableIPCode var ise
            /// 
            if (t.IsNotNull(tResimEditor.imagesMasterTableIPCode))
            {
                this.imagesSourceFormName = tResimEditor.imagesSourceFormName;  //v.con_ImagesSourceFormName;
                this.imagesSourceTableIPCode = tResimEditor.imagesSourceTableIPCode; // v.con_ImagesSourceTableIPCode;
                this.imagesSourceFieldName = tResimEditor.imagesSourceFieldName;// v.con_ImagesSourceFieldName;
                this.imagesMasterTableIPCode = tResimEditor.imagesMasterTableIPCode;// //v.con_ImagesMasterTableIPCode;

                // fieldName üzerinden small ifadesini çıkar
                this.imagesSourceFieldName = this.imagesSourceFieldName.Replace("Small", "");

                if (t.IsNotNull(tResimEditor.imagesMasterFormCode) == false)
                {
                    tInputPanel ip = new tInputPanel();
                    // ip.Create_InputPanel(this, splitContainerControl1.Panel1, this.imagesMasterTableIPCode, 1, true);
                    ip.Create_InputPanel(this, groupControlKisi, this.imagesMasterTableIPCode, 1, true);
                }

                t.Find_DataSet(this, ref dsImages, ref dNImages, this.imagesMasterTableIPCode);
            }

        }

        void PropertiesChange()
        {
            barEditItem_ZoomLine.EditValue = 100;
            zoomTrackBarControl1.LargeChange = 25;
            zoomTrackBarControl1.Maximum = 400;
            zoomTrackBarControl1.Middle = 5;
            zoomTrackBarControl1.Minimum = 0;
            zoomTrackBarControl1.ScrollThumbStyle = DevExpress.XtraEditors.Repository.ScrollThumbStyle.ArrowDownRight;
            //zoomTrackBarControl1.AccessibleName = FieldName;
            //zoomTrackBarControl1.AccessibleDescription = FormName;

            barEditItem_Boyut.EditValue = GetSizeMode(pictureEdit1.Properties.SizeMode);

            tButtonEdit_AutoCorps.ButtonClick += new
                DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_AutoCorps_ButtonClick);

            tButtonEdit_Compress.ButtonClick += new
                DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_Compress_ButtonClick);

            tButtonEdit_DPI.ButtonClick += new
                DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_DPI_ButtonClick);

            tButtonEdit_Quality.ButtonClick += new
                DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_Quality_ButtonClick);

            tButtonEdit_Zoom.ButtonClick += new
                DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_Zoom_ButtonClick);

            this.pictureEdit1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureEdit1_MouseDown);
            this.pictureEdit1.MouseLeave += new System.EventHandler(this.pictureEdit1_MouseLeave);
            this.pictureEdit1.MouseHover += new System.EventHandler(this.pictureEdit1_MouseHover);
            this.pictureEdit1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureEdit1_MouseMove);
            this.pictureEdit1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureEdit1_MouseUp);
            this.pictureEdit1.Validated += new System.EventHandler(this.pictureEdit1_Validated);
            this.pictureEdit1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureEdit1_Paint);
            this.pictureEdit1.ZoomPercentChanged += new System.EventHandler(this.pictureEdit1_ZoomPercentChanged);

            this.zoomPercent = (int)pictureEdit1.Properties.ZoomPercent;
        }

        int GetSizeMode(DevExpress.XtraEditors.Controls.PictureSizeMode sizeMode)
        {
            switch (sizeMode)
            {
                case DevExpress.XtraEditors.Controls.PictureSizeMode.Clip: return 0;
                case DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze: return 5;
                case DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch: return 1;
                case DevExpress.XtraEditors.Controls.PictureSizeMode.StretchHorizontal: return 3;
                case DevExpress.XtraEditors.Controls.PictureSizeMode.StretchVertical: return 4;
                case DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom: return 2;
                default: return 0;
            }
        }

        void Files_Preparing()
        {
            /*
            // treelist
            this.treeList1.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(this.treeList1_GetStateImage);
            this.treeList1.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeList1_FocusedNodeChanged);
            this.treeList1.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.treeList1_CustomDrawNodeCell);
            this.treeList1.VirtualTreeGetChildNodes += new DevExpress.XtraTreeList.VirtualTreeGetChildNodesEventHandler(this.treeList1_VirtualTreeGetChildNodes);
            this.treeList1.VirtualTreeGetCellValue += new DevExpress.XtraTreeList.VirtualTreeGetCellValueEventHandler(this.treeList1_VirtualTreeGetCellValue);
            // treelist
            treeList1.DataSource = new object();
            */

            this.winExplorerView.ItemClick += new DevExpress.XtraGrid.Views.WinExplorer.WinExplorerViewItemClickEventHandler(this.OnWinExplorerViewItemClick);
            this.winExplorerView.ItemDoubleClick += new DevExpress.XtraGrid.Views.WinExplorer.WinExplorerViewItemDoubleClickEventHandler(this.OnWinExplorerViewItemDoubleClick);
            this.winExplorerView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnWinExplorerViewKeyDown);
        }

        void GlyphLoad()
        {
            if (v.ds_Icons != null)
            {
                string glName = string.Empty;

                glName = "30_301_ImportImage_32x32";
                btn_TarayicidanAl.LargeGlyph = t.Find_Glyph(glName);
                glName = "30_301_Open2_32x32";
                btn_DosyadanAl.LargeGlyph = t.Find_Glyph(glName);
                glName = "SAVE32";
                btn_DosyayaKaydet.LargeGlyph = t.Find_Glyph(glName);
                glName = "30_317_ExportToIMG_16x16";
                btn_FarkliKaydet.Glyph = t.Find_Glyph(glName);
                glName = "DELETE16";
                btn_ResimSil.Glyph = t.Find_Glyph(glName);
                glName = "30_316_Cut_16x16";
                barStaticItem_Kirp.Glyph = t.Find_Glyph(glName);
                barButtonItem_Kirp.Glyph = t.Find_Glyph(glName);
                glName = "40_417_HighlightActiveElements_16x16";
                barStaticItem_Sikistir.Glyph = t.Find_Glyph(glName);
                glName = "40_406_CheckBox_16x16";
                barStaticItem_DPI.Glyph = t.Find_Glyph(glName);

                glName = "30_311_Image_32x32";
                tabPage_Resim.Image = t.Find_Glyph(glName);
                glName = "10_104_FolderPanel_32x32";
                tabPage_Dosya.Image = t.Find_Glyph(glName);

                glName = "30_312_InitialState_32x32";
                PageGroupViewEmpty.LargeGlyph = t.Find_Glyph(glName);


                glName = "40_419_PageOrientation_16x16";
                barCheckItem_Manuel.Glyph = t.Find_Glyph(glName);
                glName = "40_419_PaperSize_16x16";
                barCheckItem_Otomatik.Glyph = t.Find_Glyph(glName);
                glName = "40_415_Undo_16x16";
                barButtonItem_Vazgec.Glyph = t.Find_Glyph(glName);

                glName = "";
                glName = "";
                glName = "";

            }
        }

        private void navigationPane1_SelectedPageChanged(object sender, DevExpress.XtraBars.Navigation.SelectedPageChangedEventArgs e)
        {
            if (tabPane1.SelectedPage == tabPage_Resim)
                ribbonControl1.SelectedPage = ribbonControl1.Pages[0];
            if (tabPane1.SelectedPage == tabPage_Dosya)
                ribbonControl1.SelectedPage = ribbonControl1.Pages[2];
        }

        private void ribbonControl1_SelectedPageChanged(object sender, EventArgs e)
        {
            if (ribbonControl1.SelectedPage == ribbonControl1.Pages[0] ||
                ribbonControl1.SelectedPage == ribbonControl1.Pages[1])
                tabPane1.SelectedPage = tabPage_Resim;
            if (ribbonControl1.SelectedPage == ribbonControl1.Pages[2])
                tabPane1.SelectedPage = tabPage_Dosya;
        }

        #region // DevExpress </treeList1>

        private void treeList1_VirtualTreeGetChildNodes(object sender, DevExpress.XtraTreeList.VirtualTreeGetChildNodesInfo e)
        {

            Cursor current = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            if (!loadDrives)
            {
                string[] roots = Directory.GetLogicalDrives();
                e.Children = roots;
                loadDrives = true;
            }
            else
            {
                try
                {
                    string path = (string)e.Node;
                    if (Directory.Exists(path))
                    {
                        string[] dirs = Directory.GetDirectories(path);
                        //string[] files = Directory.GetFiles(path);
                        string[] arr = new string[dirs.Length];// + files.Length];
                        dirs.CopyTo(arr, 0);
                        //files.CopyTo(arr, dirs.Length);
                        e.Children = arr;

                    }
                    else e.Children = new object[] { };
                }
                catch { e.Children = new object[] { }; }
            }
            Cursor.Current = current;

        }

        private void treeList1_VirtualTreeGetCellValue(object sender, DevExpress.XtraTreeList.VirtualTreeGetCellValueInfo e)
        {

            DirectoryInfo di = new DirectoryInfo((string)e.Node);
            if (e.Column == colName) e.CellData = di.Name;
            if (e.Column == colType)
            {
                if (IsDrive((string)e.Node)) e.CellData = "Drive";
                else if (!IsFile(di))
                    e.CellData = "Folder";
                else
                    e.CellData = "File";
            }
            if (e.Column == colSize)
            {
                if (IsFile(di))
                {
                    e.CellData = new FileInfo((string)e.Node).Length;
                }
                else e.CellData = null;
            }

        }

        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {

            DevExpress.XtraTreeList.Nodes.TreeListNode node = e.Node;

            if (node.GetValue(0) != null)
            {
                string ret = node.GetValue(0).ToString();
                while (node.ParentNode != null)
                {
                    node = node.ParentNode;
                    ret = node.GetValue(0).ToString() + "\\" + ret;
                }

                // WinExplorer View
                this.currentPath = ret;

                DirectoryView();
            }

        }

        private void treeList1_GetStateImage(object sender, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            if (e.Node.GetDisplayText("Type") == "Folder")
                e.NodeImageIndex = e.Node.Expanded ? 1 : 0;
            else if (e.Node.GetDisplayText("Type") == "File") e.NodeImageIndex = 2;
            else e.NodeImageIndex = 3;
        }

        private void treeList1_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            if (e.Column == this.colSize)
            {
                if (e.Node.GetDisplayText("Type") == "File")
                {
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, FontStyle.Italic);
                    Int64 size = Convert.ToInt64(e.Node.GetValue("Size"));
                    if (size >= 1024)
                        e.CellText = string.Format("{0:### ### ###} KB", size / 1024);
                    else e.CellText = string.Format("{0} Bytes", size);
                }
                else e.CellText = String.Format("<{0}>", e.Node.GetDisplayText("Type"));
            }

            if (e.Column == this.colName)
            {
                if (e.Node.GetDisplayText("Type") == "File")
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
        }

        bool IsFile(DirectoryInfo info)
        {
            try
            {
                return (info.Attributes & FileAttributes.Directory) == 0;
            }
            catch
            {
                return false;
            }
        }

        bool IsDrive(string val)
        {
            string[] drives = Directory.GetLogicalDrives();
            foreach (string drive in drives)
            {
                if (drive.Equals(val)) return true;
            }
            return false;
        }

        #endregion // DevExpress </treeList1>

        #region WinExplorerView

        IconSizeType GetItemSizeType(WinExplorerViewStyle viewStyle)
        {
            switch (viewStyle)
            {
                case WinExplorerViewStyle.Large:
                case WinExplorerViewStyle.ExtraLarge: return IconSizeType.ExtraLarge;
                case WinExplorerViewStyle.List:
                case WinExplorerViewStyle.Small: return IconSizeType.Small;
                case WinExplorerViewStyle.Tiles:
                case WinExplorerViewStyle.Medium: return IconSizeType.Medium;
                case WinExplorerViewStyle.Content: return IconSizeType.Large;
                default: return IconSizeType.ExtraLarge;
            }
        }

        public WinExplorerViewStyle ViewStyle { get { return this.winExplorerView.OptionsView.Style; } }

        Size GetItemSize(WinExplorerViewStyle viewStyle)
        {
            switch (viewStyle)
            {
                case WinExplorerViewStyle.ExtraLarge: return new Size(256, 256);
                case WinExplorerViewStyle.Large: return new Size(96, 96);
                case WinExplorerViewStyle.Content: return new Size(32, 32);
                case WinExplorerViewStyle.Small: return new Size(16, 16);
                case WinExplorerViewStyle.Tiles:
                case WinExplorerViewStyle.Default:
                case WinExplorerViewStyle.List:
                case WinExplorerViewStyle.Medium:
                default: return new Size(96, 96);
            }
        }

        void EnsureSearchEdit()
        {
            EditSearch.Properties.NullText = "Search " + FileSystemHelper.GetDirName(this.currentPath);
            EditSearch.EditValue = null;
            this.winExplorerView.FindFilterText = string.Empty;
        }

        void DirectoryView()
        {
            if (t.IsNotNull(this.currentPath) == false) return;

            Cursor oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                if (!string.IsNullOrEmpty(this.currentPath))
                {
                    try
                    {
                        gridControl.DataSource = FileSystemHelper.GetFileSystemEntries(this.currentPath, GetItemSizeType(ViewStyle), GetItemSize(ViewStyle));

                        #region // resimlerin yüklemesi

                        tFileDataTable = gridControl.DataSource;

                        if (tFileDataTable != null)
                        {
                            string type = string.Empty;
                            string path = string.Empty;
                            string name = string.Empty;

                            int ii = 0;
                            foreach (var item in ((DevExpress.Utils.Helpers.FileSystemEntryCollection)tFileDataTable))
                            {
                                type = item.ToString();

                                if (type == "DevExpress.Utils.Helpers.FileEntry")
                                {
                                    //•BMP•GIF•JPEG•PNG•TIFF
                                    path = item.Path;
                                    if ((path.IndexOf(".bmp") > 0) ||
                                        (path.IndexOf(".gif") > 0) ||
                                        (path.IndexOf(".jpg") > 0) ||
                                        (path.IndexOf(".jpeg") > 0) ||
                                        (path.IndexOf(".png") > 0) ||
                                        (path.IndexOf(".tiff") > 0))
                                    {
                                        var fs = (FileSystemEntry)winExplorerView.GetRow(ii);
                                        try
                                        {
                                            fs.Image = Image.FromFile(path, true);
                                        }
                                        catch (System.IO.FileNotFoundException)
                                        {
                                            //
                                        }
                                    }

                                }
                                ii++;
                            } // foreach                          
                        } // if (tDataTable != null)
                        #endregion // resimlerin yüklemesi

                    }
                    catch (Exception)
                    {
                        gridControl.DataSource = null;
                    }
                }
                else
                    gridControl.DataSource = null;

                winExplorerView.RefreshData();
                EnsureSearchEdit();
                BeginInvoke(new MethodInvoker(winExplorerView.ClearSelection));
            }
            finally
            {
                Cursor.Current = oldCursor;
            }
        }

        void OnViewStyleGalleryItemCheckedChanged(object sender, GalleryItemEventArgs e)
        {
            GalleryItem item = e.Item;
            if (!item.Checked) return;
            WinExplorerViewStyle viewStyle = (WinExplorerViewStyle)Enum.Parse(typeof(WinExplorerViewStyle), item.Tag.ToString());
            this.winExplorerView.OptionsView.Style = viewStyle;
            FileSystemImageCache.Cache.ClearCache();
            DirectoryView();
        }
        void OnWinExplorerViewKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

            if (e.KeyCode != Keys.Enter) return;
            FileSystemEntry entry = GetSelectedEntries().LastOrDefault();
            //if (entry != null) entry.DoAction(this);
        }
        void OnWinExplorerViewItemClick(object sender, WinExplorerViewItemClickEventArgs e)
        {
            if (e.MouseInfo.Button == MouseButtons.Right) itemPopupMenu.ShowPopup(Cursor.Position);
        }
        void OnWinExplorerViewItemDoubleClick(object sender, WinExplorerViewItemDoubleClickEventArgs e)
        {

            if (e.MouseInfo.Button != MouseButtons.Left) return;
            winExplorerView.ClearSelection();
            //((FileSystemEntry)e.ItemInfo.Row.RowKey).DoAction(this);


            FileSystemEntry selectitem = (FileSystemEntry)winExplorerView.GetRow(winExplorerView.FocusedRowHandle);

            this.Text = selectitem.Path.ToString();
            this.currentPath = selectitem.Path.ToString();

            DirectoryView();

            //---
            //DevExpress.Utils.Text.StringInfo si = e.ItemInfo.TextInfo;

            //string s = e.ItemInfo.Description + v.ENTER;
            //s = s + e.ItemInfo.Text + v.ENTER;

            //this.Text = s;

            //for (int i = 0; i < this.winExplorerView.RowCount; i++) this.winExplorerView.InvertRowSelection(i);
        }

        void OnShowCheckBoxesItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.winExplorerView.OptionsView.ShowCheckBoxes = ((BarCheckItem)e.Item).Checked;
        }
        void OnShowFileNameExtensionsCheckItemClick(object sender, ItemClickEventArgs e)
        {
            FileSystemEntryCollection col = gridControl.DataSource as FileSystemEntryCollection;
            if (col == null) return;
            col.ShowExtensions = ((BarCheckItem)e.Item).Checked;
            gridControl.RefreshDataSource();
        }
        void OnShowHiddenItemsCheckItemClick(object sender, ItemClickEventArgs e)
        {
            btnHideSelectedItems.Enabled = !((BarCheckItem)e.Item).Checked;
        }
        void OnOptionsItemClick(object sender, ItemClickEventArgs e)
        {
            IEnumerable<FileSystemEntry> entries = GetSelectedEntries();
            if (entries.Count() == 0)
            {
                // unutma
                //FileSystemHelper.ShellExecuteFileInfo(this.currentPath, ShellExecuteInfoFileType.Properties);
                //FileSystemHelper.
                return;
            }
            
            //foreach (FileSystemEntry entry in entries) entry.ShowProperties();
        }

        List<FileSystemEntry> GetSelectedEntries() { return GetSelectedEntries(false); }
        List<FileSystemEntry> GetSelectedEntries(bool sort)
        {
            List<FileSystemEntry> list = new List<FileSystemEntry>();
            int[] rows = winExplorerView.GetSelectedRows();
            for (int i = 0; i < rows.Length; i++)
            {
                list.Add((FileSystemEntry)winExplorerView.GetRow(rows[i]));
            }
            if (sort) list.Sort(new FileSytemEntryComparer());
            return list;
        }

        #endregion WinExplorerView

        #region Microsoft Tree

        private void PopulateTreeView()
        {
            TreeNode rootNode;

            DirectoryInfo info = new DirectoryInfo(@"../..");
            //DirectoryInfo info = new DirectoryInfo(@"c:\\Preject_S\\");

            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }

        private void AddPath(TreeNode node, string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);

            if (info.Exists)
            {
                node = new TreeNode(info.Name);
                node.Tag = info;
                //info.GetDirectories("*.*", SearchOption.TopDirectoryOnly)
                GetDirectories(info.GetDirectories(), node);
                treeView1.Nodes.Add(node);
            }
        }

        private void AllDrives()
        {
            TreeNode rootNode;

            this.desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            rootNode = new TreeNode("Desktop");
            AddPath(rootNode, this.desktopPath);

            this.myPicturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            rootNode = new TreeNode("MyPictures");
            AddPath(rootNode, this.myPicturesPath);

            this.myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            rootNode = new TreeNode("MyDocuments");
            AddPath(rootNode, this.myDocumentsPath);

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady == true)
                {
                    //"  Drive {0}", d.Name);
                    //"  Drive type: {0}", d.DriveType);
                    //"  Volume label: {0}", d.VolumeLabel);
                    //"  File system: {0}", d.DriveFormat);
                    //"  Available space to current user:{0, 15} bytes", d.AvailableFreeSpace);
                    //"  Total available space:          {0, 15} bytes", d.TotalFreeSpace);
                    //"  Total size of drive:            {0, 15} bytes", d.TotalSize);

                    DirectoryInfo info = new DirectoryInfo(d.Name + "\\");

                    if (info.Exists)
                    {
                        rootNode = new TreeNode(info.Name);
                        rootNode.Tag = info;
                        //info.GetDirectories("*.*", SearchOption.TopDirectoryOnly)
                        GetDirectories(info.GetDirectories(), rootNode);
                        treeView1.Nodes.Add(rootNode);
                    }
                }
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {

            TreeNode aNode;
            //DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                //System.UnauthorizedAccessException
                if (
                    ((subDir.Attributes & FileAttributes.System) == 0) &&
                    ((subDir.Attributes & FileAttributes.Hidden) == 0) &&
                    ((subDir.Attributes & FileAttributes.ReadOnly) == 0)
                   )
                {
                    aNode = new TreeNode(subDir.Name, 0, 0);
                    aNode.Tag = subDir;
                    aNode.ImageKey = "folder";
                    /*
                    try
                    {
                        if ((subDir.Name != "inetpub") &&
                            (subDir.Name != "Qoobox"))
                        {
                            subSubDirs = subDir.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
                            if (subSubDirs.Length != 0)
                            {
                                GetDirectories(subSubDirs, aNode);
                            }
                        }
                    }
                    catch (Exception)
                    {

                        //throw;
                    }
                    */

                    nodeToAddTo.Nodes.Add(aNode);
                }
            }

        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string path = e.Node.FullPath;

            //Desktop 
            if (path.IndexOf("Desktop") > -1)
                path = path.Replace("Desktop", this.desktopPath);
            //MyPictures
            if (path.IndexOf("Pictures") > -1)
                path = path.Replace("Pictures", this.myPicturesPath);
            //MyDocuments
            if (path.IndexOf("Documents") > -1)
                path = path.Replace("Documents", this.myDocumentsPath);


            //e.Node
            if (e.Node.Nodes.Count == 0)
            {
                try
                {
                    DirectoryInfo info = new DirectoryInfo(path);
                    GetDirectories(info.GetDirectories(), e.Node);
                }
                catch (Exception)
                {
                    //throw;
                }
            }

            // WinExplorer View
            this.currentPath = path;

            DirectoryView();

        }

        #endregion Microsoft Tree

        #region Resim Edit

        private void btn_DatabaseKaydet_ItemClick(object sender, EventArgs e)
        {
            dataBaseKaydet();

            if (v.SP_TabimDbConnection)
            {
                int pos = dNImages.Position;
                t.TableRefresh(this, dsImages);
                dNImages.Position = pos;
            }
        }

        private void btn_TarayicidanAl_ItemClick(object sender, ItemClickEventArgs e)
        {
            v.con_LkpOnayChange = true;

            //string scannerName = imageComboBox_Tarayici.Items.ToString();
            string scannerName = barEditItem_Tarayici.EditValue.ToString();

            var item = imageComboBox_Tarayici.Items;
            if (scannerName == tarayiciYokMesaji)
            {
                t.FlyoutMessage(this, "Dikkat : ", tarayiciYokMesaji);
                return;
            }
            //
            this.hScrollBar = null;
            this.vScrollBar = null;
            this.viewInfo = null;

            ImageFile ImgFile = null;

            try
            {
                //Tarayıcı kontrolü, bağlı değilse catch e düşer.

                /*    
                WIA.CommonDialog dialog = new WIA.CommonDialog();

                //Tarayıcı bağlı değil ise img alınırken hataya düşüyor, catch ile yakalanıp tarayıcı olmadığı basılıyor.
                //if (((DevExpress.XtraEditors.CheckEdit)checkEdit_Tarayici).Checked)
                
                if (this.checkEdit_Tarayici.Checked)
                {
                    //Her girişte Cihaz seçimi yapılacaksa.
                    ImgFile = dialog.ShowAcquireImage(WiaDeviceType.ScannerDeviceType, WiaImageIntent.ColorIntent, WiaImageBias.MinimizeSize, "{00000000-0000-0000-0000-000000000000}", true, true, false);
                }
                else
                {
                    //cihaz seçimi yapılmaksızın seçili cihaz ile sürekli devam edilecekse.
                    ImgFile = dialog.ShowAcquireImage(WiaDeviceType.ScannerDeviceType, WiaImageIntent.ColorIntent, WiaImageBias.MinimizeSize, "{00000000-0000-0000-0000-000000000000}", false, true, false);
                //}
                */


                try
                {
                    // exenin bulunduğu path altına images isimli path içine guid isimli dosya hazırlanıyor
                    string ImagesPath = t.Find_Path("images") + tFileGuidName + ".jpg";

                    DeviceManager deviceManager = new DeviceManager();
                    Device device = null;

                    foreach (DeviceInfo deviceInfo in deviceManager.DeviceInfos)
                    {
                        if (deviceInfo.Type == WiaDeviceType.ScannerDeviceType && 
                            deviceInfo.Properties["Name"].get_Value().ToString() == scannerName)
                        {
                            device = deviceInfo.Connect();
                            break;
                        }
                    }

                    if (device != null)
                    {
                        WIA.CommonDialog dialog = new WIA.CommonDialog();

                        //ImgFile = (ImageFile)dialog.ShowTransfer(item, WiaFormatJPEG.WiaFormatJPEG, false);

                        if (this.imageType == "Biyometrik" || this.imageType == "Imza")
                        {
                            this.btn_Zoom.EditValue = "100";
                            this.barEditItem_ZoomLine.EditValue = 100;
                            ImgFile = (ImageFile)dialog.ShowAcquireImage(WiaDeviceType.ScannerDeviceType, WiaImageIntent.ColorIntent, WiaImageBias.MinimizeSize, "{00000000-0000-0000-0000-000000000000}", false, true, false);
                        }
                        else
                        {
                            this.btn_Zoom.EditValue = "35";
                            this.barEditItem_ZoomLine.EditValue = 35;
                            ImgFile = (ImageFile)dialog.ShowAcquireImage(WiaDeviceType.ScannerDeviceType, WiaImageIntent.TextIntent, WiaImageBias.MinimizeSize, "{00000000-0000-0000-0000-000000000000}", false, true, false);
                            //ImgFile = (ImageFile)dialog.ShowAcquireImage(FormatID.wiaFormatJPEG, WiaImageIntent.ColorIntent, "{00000000-0000-0000-0000-000000000000}");
                                //WiaDeviceType.ScannerDeviceType, WiaImageIntent.TextIntent, WiaImageBias.MinimizeSize, "{00000000-0000-0000-0000-000000000000}", false, true, false);

                        }
                        if (ImgFile != null)
                        {
                            ImgFile.SaveFile(ImagesPath);
                            t.AlertMessage("Tarayıcı", "Evrak tarama işlemi tamamlandı.");
                        }

                        //WIA.CommonDialog dlg = new WIA.CommonDialog();
                        //WIA.ImageFile imgFile = dlg.ShowAcquireImage(WIA.FormatID.wiaFormatBMP, WIA.Intent.wiaIntentPreview);

                        // Görüntüyü kaydetme (örneğin, BMP formatında)
                        //imgFile.SaveFile("taramali_goruntu.bmp");
                    }
                    else
                    {
                        MessageBox.Show("Tarayıcı bulunamadı.");
                    }

                    //Tarama bölümüne girip iptal dedikten sonra işlemler devam ediyor ve hataya sebebiyet veriyor. Kontrol altına alındı.img.savefile da patlıyor iptal dendiğinde.
                    //ImgFile.SaveFile(ImagesPath);

                    if (ImgFile != null)
                    {
                        t.WaitFormOpen(v.mainForm, "Resim yükleniyor...");

                        pictureEdit1.Image = Image.FromFile(ImagesPath);

                        newImageProperties(ImagesPath);

                        if ((tImageProperties.horizontalResolation < ( autoDPI - 10 )) || // 400 - 10 : 390 na kadar kabul
                            (tImageProperties.horizontalResolation > 600))  
                        {
                            //MessageBox.Show("DİKKAT : Tarama işleminiz 600 Dpi olmalı, onun için resim taramayı 600 Dpi olacak şekilde yeniden yapın...");
                            //pictureEdit1.Image = null;
                            //return;
                        }

                        Format_Ayarla();
                        DPI_Ayarla();

                        if (autoDPI == 400 && autoCropWidth < pictureEdit1.Image.Width)
                        {
                           imageCompress_For_400DPI();
                        }


                        if (tImageProperties.kbSize > 1000)
                        {
                            MessageBox.Show("DİKKAT : Tarama sonucu resminizin boyutu çok yüksek geldi. " + v.ENTER2
                                + tImageProperties.kbSize.ToString() + " kb " + v.ENTER2
                                + "Sıkıntı yaşayabilirsiniz. Onun için resmi yeniden taramanızı öneririz. Taramadan önce Dpi kontrol ediniz...");
                            //pictureEdit1.Image = null;
                            //return;
                        }

                        preparing_OriginalImage();

                        v.SP_OpenApplication = false;
                        v.IsWaitOpen = false;
                        t.WaitFormClose();
                    }

                    // yeni resmin orjinal halide hafızaya alınsın
                    v.con_Images_MasterPath = "";
                }
                catch (Exception)
                {
                    //Hatayı alıp programın devam etmesi adına herhangi birşey girilmedi.
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Bilgisayara bağlı bir tarayıcı bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void btn_WebCamdenAl_ItemClick(object sender, ItemClickEventArgs e)
        {
            WebCamClick();
        }

        void WebCamClick()
        {
            v.con_LkpOnayChange = true;

            string caption = btn_WebCamdenAl.Caption;

            if (caption == "Kameradan Al")
            {
                pictureEdit1.Visible = false;
                pictureEdit2.Visible = true;
                btn_WebCamResimCek.Enabled = true;

                if (vcd == null)
                {
                    vcd = new VideoCaptureDevice(fico[Convert.ToInt32(barEditItem_WebCam.EditValue) - 1].MonikerString);
                    vcd.NewFrame += Vcd_NewFrame;
                }

                if (vcd.IsRunning) vcd.Stop();
                vcd.Start();

                btn_WebCamdenAl.Caption = "Kamerayı kapat";
            }
            else
            {
                pictureEdit1.Visible = true;
                pictureEdit2.Visible = false;
                btn_WebCamResimCek.Enabled = false;

                if (vcd != null)
                    if (vcd.IsRunning) vcd.Stop();
                btn_WebCamdenAl.Caption = "Kameradan Al";
            }
        }

        private void Vcd_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // Devexpress pictureEdit1 hata veriyor
                // Bitmap frameBitmap = (Bitmap)eventArgs.Frame.Clone();
                // pictureEdit1.Image = frameBitmap;

                pictureEdit2.Image = (Bitmap)eventArgs.Frame.Clone();
            }
            catch (Exception)
            {
                //throw;
            }
        }

        private void btn_WebCamResimCek_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (vcd != null)
            {
                try
                {
                    //pictureEdit1.Image = pictureEdit2.Image;
                    //pictureEdit1.Image = new Bitmap(pictureEdit2.Image, pictureEdit2.Image.Width, pictureEdit2.Image.Height);
                    //pictureEdit1.Image = new Bitmap(pictureEdit2.Image, pictureEdit2.Width, pictureEdit2.Height);

                    cropX = 0;
                    cropY = 0;
                    cropWidth = pictureEdit2.Width;
                    cropHeight = pictureEdit2.Height;
                    myImageCrop2(pictureEdit2);

                    pictureEdit1.Image = pictureEdit2.Image;

                    if (vcd.IsRunning) vcd.Stop();
                    WebCamClick();

                    printImageProperties();
                }
                catch (Exception)
                {
                    //throw;
                }
            }
        }

        private void btn_DosyadanAl_ItemClick(object sender, ItemClickEventArgs e)
        {
            v.con_LkpOnayChange = true;

            //
            this.hScrollBar = null;
            this.vScrollBar = null;
            this.viewInfo = null;

            //Bilgiler kaydedilirken gelen resmin neye dair olduğunu anlayıp ona göre kayıt gerekli.
            //Bundan dolayı tip bilgisi enum olarak saklanıyor.
            //HelperMethod.IslemTur = HelperMethod.Tur.Dosya;

            //Resim seçilir, resmin yolu pbx1 in tag ine atılır.
            OpenFileDialog fdialog = new OpenFileDialog();
            fdialog.Filter = "Pictures|*.*";
            //fdialog.InitialDirectory = "C://"; 
            string ImagesPath = string.Empty;

            if (DialogResult.OK == fdialog.ShowDialog())
            {
                // şimdilik gerek kalmadı
                //v.con_Images_Name = fdialog.SafeFileName; // RESİM ADI.
                ImagesPath = fdialog.FileName;   // Seçilen resme ait tam yol.
                string newImagesPath = ImagesPath;

                if (ImagesPath.IndexOf(".pdf") > -1)
                {
                    int kirpPixel = 45;
                    newImagesPath = readPdfFileSpire(ImagesPath);
                    Image readPdfImage = Image.FromFile(newImagesPath);
                    Image restoreImage = CropImage(readPdfImage, 0, kirpPixel, readPdfImage.Width, readPdfImage.Height - kirpPixel, autoDPI);
                    pictureEdit1.Image = restoreImage;
                }
                else if (ImagesPath.IndexOf(".html") > -1)
                {
                    convertHtmlToPDFSpire(ImagesPath);
                    //convertHtmlToPDFAspose(ImagesPath);
                }
                else
                {
                    pictureEdit1.Image = Image.FromFile(newImagesPath);
                }
            }

            fdialog.Dispose();

            if (ImagesPath != null)
            {
                newImageProperties(ImagesPath);
                Format_Ayarla();
                DPI_Ayarla();
                preparing_OriginalImage();
            }
        }

        private string readPdfFileSpire(string fileName)
        {
            Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
            doc.LoadFromFile(fileName);

            string outputFileName = fileName.Replace(".pdf", "");
            string outputName = "";
            string firstOutputName = "";

            //Save to images
            for (int i = 0; i < doc.Pages.Count; i++)
            {
                //String fileName = String.Format("ToPNG-img-{0}.png", i);
                outputName = outputFileName + $"_{i + 1}.jpg";
                if (firstOutputName == "") firstOutputName = outputName;

                using (Image image_ = doc.SaveAsImage(i, 100, 100))
                {
                    if (File.Exists(outputName))
                    {
                        // Dosya varsa sil
                        File.Delete(outputName);
                    }
                    image_.Save(outputName, ImageFormat.Jpeg);
                }
            }
            return firstOutputName;
        }
        
        private void btn_DosyayaKaydet_ItemClick(object sender, ItemClickEventArgs e)
        {
            //string Images_Path = t.Find_Path("images") + tFileGuidName + ".jpg";

            if (pictureEdit1.Image != null)
            {
                try
                {
                    string tamAdi = imageName();
                    string Images_Path = t.Find_Path("images") + tamAdi + ".jpg";
                    pictureEdit1.Image.Save(Images_Path, ImageFormat.Jpeg);

                    MessageBox.Show("Dosyaya başarıyla kaydedilmiştir..." + v.ENTER2 + Images_Path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //throw;
                }
            }
        }

        private void btn_imgIstenenDuzenle_ItemClick(object sender, ItemClickEventArgs e)
        {
            resmi100KbAltinaDusur();
        }

        private string imageName()
        {
            string tamAdi = "noname";
            if (t.IsNotNull(dsImages))
            {
                tamAdi = dsImages.Tables[0].Rows[dNImages.Position]["TamAdiSoyadi"].ToString();
                tamAdi = tamAdi.Replace(" ", "");
                tamAdi = tamAdi + "_" + dsImages.Tables[0].Rows[dNImages.Position]["TcNo"].ToString() 
                                + "_" + dsImages.Tables[0].Rows[dNImages.Position]["LkpFieldName"].ToString();
            }
            return tamAdi;
        }

        private void btn_FarkliKaydet_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveFileDialog fdialog = new SaveFileDialog();
            fdialog.Filter = "Pictures|*.*";
            //fdialog.InitialDirectory = "C://"; 
            string ImagesPath = string.Empty;

            fdialog.FileName = imageName() + ".jpg";

            if (DialogResult.OK == fdialog.ShowDialog())
            {
                ImagesPath = fdialog.FileName;   // Seçilen resme ait tam yol.

                pictureEdit1.Image.Save(ImagesPath, ImageFormat.Jpeg);
            }
        }

        private void btn_ResimSil_ItemClick(object sender, ItemClickEventArgs e)
        {
            //
            this.hScrollBar = null;
            this.vScrollBar = null;
            this.viewInfo = null;

            string soru = " Resim veri tabanından silinecek. Onaylıyor musunuz ?";
            DialogResult cevap = t.mySoru(soru);
            if (DialogResult.Yes == cevap)
            {
                dataBasedenResmiSil();
            }
           
        }
        // Sıkıştır
        private void btn_Sikistir_ItemClick(object sender, ItemClickEventArgs e)
        {
            //
        }

        private void btn_Kalite_ItemClick(object sender, ItemClickEventArgs e)
        {
            //
            //MessageBox.Show("kalite");
        }

        private void barEditItem_Boyut_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.Controls.PictureSizeMode myMode =
               (DevExpress.XtraEditors.Controls.PictureSizeMode)Enum.Parse(typeof(DevExpress.XtraEditors.Controls.PictureSizeMode), barEditItem_Boyut.EditValue.ToString());

            pictureEdit1.Properties.SizeMode = myMode;


            //
            // Özet:
            //     A picture is not stretched. 
            //     Bir resim uzatılmamış
            // Clip = 0,
            //
            // Özet:
            //     A picture is stretched in order to fit within the area of an editor (or editor's dropdown window).
            //     Bir düzenleyicinin alanına (veya editörün açılır penceresine) sığması için bir resim uzatılır.
            // Stretch = 1,
            //
            // Özet:
            //     A picture is stretched proportionally. The picture fits within the area of an editor (or editor's dropdown window) at least in one direction.
            //     Bir resim orantılı olarak uzatılır.Resim, bir düzenleyicinin alanına(veya editörün açılır penceresine) en az bir yönde sığar.
            // Zoom = 2,
            //
            // Özet:
            //     A picture is stretched horizontally. Its height remains unchanged.
            //     Bir resim yatay olarak uzatılır.Yüksekliği değişmeden kalır.
            // StretchHorizontal = 3,
            //
            // Özet:
            //     A picture is stretched vertically. Its width remains unchanged.
            //     Bir resim dikey olarak uzatılır.Genişliği değişmeden kalır.
            // StretchVertical = 4,
            //
            // Özet:
            //     An image is displayed as is if its actual size is smaller than the size of the container. 
            //     If the image size is larger than the container's size, the image is shrunk proportionally to fit the container's bounds.
            //     Bir görüntü, gerçek boyutu kabın boyutundan daha küçükmüş gibi görüntülenir.
            //     Görüntü boyutu kabın boyutundan daha büyükse, kabın sınırlarına uyması için görüntü orantılı olarak küçültülür.
            // Squeeze = 5


        }

        #endregion Resim Edit

        #region pictureEdit1 events

        private void pictureEdit1_MouseUp(object sender, MouseEventArgs e)
        {
            //Point my = tImagePoint(pictureEdit1, e.Location);
            //oCropX = my.X;
            //oCropY = my.Y;

            Cursor = Cursors.Default;
        }

        private void pictureEdit1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //barButtonItem_Vazgec.Checked = true;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //if ((barCheckItem_Manuel.Checked) || (barCheckItem_Otomatik.Checked))
                if (this.IsActiveOlcek)
                {
                    this.zoomPercent = (int)pictureEdit1.Properties.ZoomPercent;

                    if (this.viewInfo == null)
                        tPictureEdit_CommonSetup(pictureEdit1, ref hScrollBar, ref vScrollBar, ref viewInfo);

                    Cursor = Cursors.Cross;

                    cropX = e.X;
                    cropY = e.Y;

                    //Point my = ScreenPoint(pictureEdit1, e.Location);
                    //Point my = pictureEdit1.PointToClient(new Point(e.X, e.Y));
                    
                    Point my = tImagePoint(pictureEdit1, e.Location);
                    startCropX = my.X;
                    startCropY = my.Y;
                    
                    drawRectangle();
                }
            }
        }

        private void pictureEdit1_MouseMove(object sender, MouseEventArgs e)
        {

            if (pictureEdit1.Image == null)
                return;

            //Point my1 = ScreenPoint(pictureEdit1, e.Location);
            //Point my2 = pictureEdit1.PointToClient(new Point(e.X, e.Y));
            //Point my3 = ImagePoint(pictureEdit1, e.Location);

            barStaticItem_X.Caption = " X : " + e.X.ToString() + "/" + startCropX.ToString();
            barStaticItem_Y.Caption = " Y : " + e.Y.ToString() + "/" + startCropY.ToString();

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //if (barCheckItem_Manuel.Checked)
                if (this.olcekName == "Manuel")
                {
                    cropWidth = e.X - cropX;
                    cropHeight = e.Y - cropY;
                }

                //if (barCheckItem_Otomatik.Checked)
                if (this.olcekName == "Auto")
                {
                    cropX = e.X;
                    cropY = e.Y;
                    cropWidth = autoCropWidth;
                    cropHeight = autoCropHeight;
                }

                Cursor = Cursors.Cross;
            }

            if ((e.Button == System.Windows.Forms.MouseButtons.Left) || (this.IsActiveOlcek))
                //(barCheckItem_Manuel.Checked) || (barCheckItem_Otomatik.Checked))
            {
                drawRectangle();
            }

        }
        
        private void pictureEdit1_MouseHover(object sender, EventArgs e)
        {
            //if ((cropX > 0) && (cropY > 0))
            drawRectangle();
        }

        private void pictureEdit1_MouseLeave(object sender, EventArgs e)
        {
            //if ((cropX > 0) && (cropY > 0))
            drawRectangle();
        }

        private void pictureEdit1_Validated(object sender, EventArgs e)
        {
            //if ((cropX > 0) && (cropY > 0))
            drawRectangle();
            //pictureEdit1.CreateGraphics().DrawRectangle(cropPen, 0, 0, pictureEdit1.Image.Width, pictureEdit1.Image.Height);
        }

        private void pictureEdit1_Paint(object sender, PaintEventArgs e)
        {
            //
        }

        private void pictureEdit1_ZoomPercentChanged(object sender, EventArgs e)
        {
            drawRectangle();
        }

        void drawRectangle()
        {
            
            pictureEdit1.Refresh();

            //if ((barCheckItem_Manuel.Checked) ||  (barCheckItem_Otomatik.Checked))
            if (this.IsActiveOlcek)
            {
                //if (this.zoomPercent <= 0)
                //    this.zoomPercent = (int)pictureEdit1.Properties.ZoomPercent;

                decimal oran = 0;
                int _cropWidth = 0;
                int _cropHeight = 0;

                //if (barCheckItem_Otomatik.Checked)
                if (this.olcekName == "Auto")
                {
                    oran = ((decimal)this.zoomPercent / 100);
                    _cropWidth = (int)((decimal)cropWidth * oran);
                    _cropHeight = (int)((decimal)cropHeight * oran);
                }
                else
                {
                    _cropWidth = cropWidth;
                    _cropHeight = cropHeight;
                }

                pictureEdit1.CreateGraphics().DrawRectangle(cropPen, cropX, cropY, _cropWidth, _cropHeight);

                pictureEdit1.CreateGraphics().DrawLine(cropPen2, 
                    cropX + (_cropWidth / 2), cropY, 
                    cropX + (_cropWidth / 2), cropY + _cropHeight);


                barStaticItem_H.Caption = "Yükseklik: " + cropHeight.ToString();
                barStaticItem_W.Caption = "Genişlik : " + cropWidth.ToString();

            } else
            {
                barStaticItem_H.Caption = "Yükseklik: ";
                barStaticItem_W.Caption = "Genişlik : ";
            }
        }

        #endregion pictureEdit1 events
                


        #region Corps - Kırp

        private void barCheckItem_Otomatik_ItemClick(object sender, ItemClickEventArgs e)
        {
            kirpmaIslemi(true);
            this.olcekName = "Auto";
        }
        private void barCheckItem_Manuel_ItemClick(object sender, ItemClickEventArgs e)
        {
            cropWidth = 300;
            cropHeight = 300;

            kirpmaIslemi(true);
            this.olcekName = "Manuel";
        }
        private void kirpmaIslemi(bool IsActive)
        {
            this.barCheckItem_Otomatik.Enabled = !IsActive;
            this.barCheckItem_Manuel.Enabled = !IsActive;

            this.barButtonItem_Kirp.Enabled = IsActive;
            //this.barButtonItem_Vazgec.Enabled = IsActive;

            this.IsActiveOlcek = IsActive;

            if (IsActive)
                checkedOrjinalImageControl();
        }

        private void barButtonItem_Kirp_ItemClick(object sender, ItemClickEventArgs e)
        {
            myImageCrop(pictureEdit1);

            kirpmaIslemi(false);

            // vazgeçebilir
            //this.barButtonItem_Vazgec.Enabled = true;

            printImageProperties();

            t.AlertMessage("Bilgilendirme", "Resim kırpma işlem yapılmıştır...");
            //MessageBox.Show("Kırpma işlemi gerçekleştirildi...");
        }
        private void barButtonItem_Vazgec_ItemClick(object sender, ItemClickEventArgs e)
        {
            /// ne yaparsan yap ilk orjinal kalite yakalnmıyor
            /// bu nedenle tekrar dosyadan okundu
            ///
            if (OriginalImage != null)
                pictureEdit1.Image = new Bitmap(OriginalImage, OriginalImage.Width, OriginalImage.Height);

            printImageProperties();

            kirpmaIslemi(false);
        }
        private void btnKirpOnayi_ItemClick(object sender, ItemClickEventArgs e)
        {
            // çalışmadı
            /*
            kirpmaIslemi(false);
            this.btnKirpOnayi.Enabled = false;

            MessageBox.Show("Kırpma işlemi gerçekleştirildi...");

            printImageProperties();
            */
        }

        private void btn_AutoCorps_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //string FieldName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleName;
            //string FormName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleDescription;
            //Form tForm = Application.OpenForms[FormName];

            string txt = ((DevExpress.XtraEditors.ButtonEdit)sender).Text;
            int value = t.myInt32(txt);

            if (e.Button.Index < 2)
            {
                // 10 ile 100 arası olsun ( 100 arttırılabilir )

                if (e.Button.Index == 0) value = value - 10;
                if (e.Button.Index == 1) value = value + 10;
                if (value < 10) value = 10;
                if (value > 100) value = 100;

                ((DevExpress.XtraEditors.ButtonEdit)sender).Text = value.ToString() + "  pixel";
            }

            // Otomatik Kirp
            if (e.Button.Index == 2) // Uygula
            {
                startCropX = value;
                startCropY = value;
                /// * 2 ? anlamı yüksekliğin ve genişliğin her iki yanından kırpma yapması için
                cropWidth = pictureEdit1.Image.Width - (value * 2);
                cropHeight = pictureEdit1.Image.Height - (value * 2);

                if ((cropWidth > (value * 2)) &&
                    (cropHeight > (value * 2))
                    ) myImageCrop(pictureEdit1);

                MessageBox.Show("Kırpma işlemi gerçekleştirildi...");
            }
        }

        private void myImageCrop(DevExpress.XtraEditors.PictureEdit pictureEdit_)
        {
            if (cropWidth < 1) return;

            //if (this.zoomPercent <= 0)
            //    this.zoomPercent = (int)pictureEdit1.Properties.ZoomPercent;

            try
            {
                pictureEdit_.CreateGraphics().DrawRectangle(cropPen3, cropX, cropY, cropWidth, cropHeight);

                /*
                /// zoom dan dolayı genişlik ve yükseklik yeniden hesaplanıyor
                /// 
                decimal oran = 1; // ((decimal)this.zoomPercent/100);
                //cropWidth = (int)((decimal)cropWidth * (100 / (decimal)this.zoomPercent));
                //cropHeight = (int)((decimal)cropHeight * (100 / (decimal)this.zoomPercent));
                
                cropWidth = (int)((decimal)cropWidth * oran);
                cropHeight = (int)((decimal)cropHeight * oran);
                */

                // kaynak okunacak alan
                Rectangle sourceRect = new Rectangle(startCropX+1, startCropY+1, cropWidth, cropHeight);
                // yeni resim çerçevesi
                Rectangle destinationRect = new Rectangle(0, 0, cropWidth, cropHeight);
                
                // yeni image hazırlanıyor
                Bitmap _img = new Bitmap(cropWidth, cropHeight);
                _img.SetResolution(pictureEdit_.Image.HorizontalResolution, pictureEdit_.Image.VerticalResolution);
                /// for cropinf image
                Graphics gr = Graphics.FromImage(_img);
                /// create graphics
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                // yeni resmi oluşturuyor
                gr.DrawImage(pictureEdit_.Image, destinationRect, sourceRect, GraphicsUnit.Pixel);
                _img.SetResolution(autoDPI, autoDPI);
                // oluşan yeni image takrar pictureEdit1 yükleniyor
                pictureEdit_.Image = _img;
                /// new image end
                cropX = 0;
                cropY = 0;
                cropWidth = 0;
                cropHeight = 0;
                Cursor = Cursors.Default;
                
                printImageProperties();

                //MessageBox.Show("Kırpma işlemi gerçekleştirildi...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }

        }
        private void myImageCrop2(System.Windows.Forms.PictureBox pictureEdit_)
        {
            if (cropWidth < 1) return;

            //if (this.zoomPercent <= 0)
            //    this.zoomPercent = (int)pictureEdit1.Properties.ZoomPercent;

            try
            {
                pictureEdit_.CreateGraphics().DrawRectangle(cropPen3, cropX, cropY, cropWidth, cropHeight);

                /// zoom dan dolayı genişlik ve yükseklik yeniden hesaplanıyor
                /// 
                decimal oran = 1;// (100 / (decimal)this.zoomPercent);
                //cropWidth = (int)((decimal)cropWidth * (100 / (decimal)this.zoomPercent));
                //cropHeight = (int)((decimal)cropHeight * (100 / (decimal)this.zoomPercent));
                cropWidth = (int)((decimal)cropWidth * oran);
                cropHeight = (int)((decimal)cropHeight * oran);

                // kaynak okunacak alan
                Rectangle sourceRect = new Rectangle(startCropX, startCropY, cropWidth, cropHeight);
                // yeni resim çerçevesi
                Rectangle destinationRect = new Rectangle(0, 0, cropWidth, cropHeight);

                // yeni image hazırlanıyor
                Bitmap _img = new Bitmap(cropWidth, cropHeight);
                _img.SetResolution(pictureEdit_.Image.HorizontalResolution, pictureEdit_.Image.VerticalResolution);
                /// for cropinf image
                Graphics gr = Graphics.FromImage(_img);
                /// create graphics
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                // yeni resmi oluşturuyor
                gr.DrawImage(pictureEdit_.Image, destinationRect, sourceRect, GraphicsUnit.Pixel);
                _img.SetResolution(autoDPI, autoDPI);
                // oluşan yeni image takrar pictureEdit1 yükleniyor
                pictureEdit_.Image = _img;
                /// new image end
                cropX = 0;
                cropY = 0;
                cropWidth = 0;
                cropHeight = 0;
                Cursor = Cursors.Default;

                //printImageProperties();

                //MessageBox.Show("Kırpma işlemi gerçekleştirildi...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }

        }

        private void imageCompress_For_400DPI()
        {
            int oldWidth = 0;
            int oldHeight = 0;
            int newWidth = 0;
            int newHeight = 0;
            int newValue = 3325; // bu rakam deneme yanılma yoluyla bulunda

            // program çalışırsken 400 dpi olan biyometrik resmin Auto ölçek ile en uygun olan küçültme oranı 
            // gözlenerek yapıldı

            oldWidth = pictureEdit1.Image.Width;
            oldHeight = pictureEdit1.Image.Height;

            Bitmap workingImage = new Bitmap(pictureEdit1.Image, oldWidth, oldHeight);
            workingImage.SetResolution(pictureEdit1.Image.HorizontalResolution, pictureEdit1.Image.VerticalResolution);
            preparingNewWidthNewHeight(newValue, oldWidth, oldHeight, ref newWidth, ref newHeight);

            if (newWidth < 0 || newHeight < 0)
            {
                newWidth = oldWidth;
                newHeight = oldHeight;
            }

            Bitmap _img = myImageCompress_(workingImage, newWidth, newHeight);

            pictureEdit1.Image = _img;

            if (oldHeight > 3000) // 3000 pixelden büyükse
            {
                this.btn_Zoom.EditValue = "35";
                this.barEditItem_ZoomLine.EditValue = 35;
            }


            t.AlertMessage("Bilgilendirme", "400 DPI için özel resim sıkıştırma işlemi uygulanmıştır...");
        }
        //var croppedImage = CropImage(Image.FromStream(Request.Files[0].InputStream), x, y, 500, 500);
        public Bitmap CropImage(Image oldImage, int x, int y, int width, int height, int newDpi)
        {

            //Rectangle rectDestination = new Rectangle(x, y, width, height);
            // kaynak okunacak alan
            Rectangle sourceRect = new Rectangle(x + 1, y + 1, width, height);
            // yeni resim çerçevesi
            Rectangle destinationRect = new Rectangle(0, 0, width, height);


            //Bitmap _newImage = new Bitmap(rectDestination.Width, rectDestination.Height);
            Bitmap _newImage = new Bitmap(sourceRect.Width, sourceRect.Height);
            _newImage.SetResolution(oldImage.HorizontalResolution, oldImage.VerticalResolution);

            Graphics gr = Graphics.FromImage(_newImage);
            gr.CompositingQuality = CompositingQuality.Default;
            gr.SmoothingMode = SmoothingMode.Default;
            gr.InterpolationMode = InterpolationMode.Bicubic;
            gr.PixelOffsetMode = PixelOffsetMode.Default;
            //gr.DrawImage(oldImage, new Rectangle(0, 0, _newImage.Width, _newImage.Height), rectDestination, GraphicsUnit.Pixel);
            //gr.DrawImage(oldImage, new Rectangle(0, 0, _newImage.Width, _newImage.Height), sourceRect, GraphicsUnit.Pixel);
            gr.DrawImage(oldImage, destinationRect, sourceRect, GraphicsUnit.Pixel);

            if (newDpi > 0)
                _newImage.SetResolution(newDpi, newDpi);

            return _newImage;
        }

        #endregion Corps - Kırp

        #region Compress - Sıkıştır

        public void btn_Compress_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            tToolBox t = new tToolBox();

            //string FieldName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleName;
            //string FormName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleDescription;
            string txt = ((DevExpress.XtraEditors.ButtonEdit)sender).Text;
            //txt = txt.Replace("pixel", "");
            int value = t.myInt32(txt);

            // 10 ile 100 arası olsun ( 100 arttırılabilir )

            if (e.Button.Index == 0) value = value - 10;
            if (e.Button.Index == 1) value = value + 10;
            if (value < 10) value = 10;
            //if (value > 500) value = 500;

            ((DevExpress.XtraEditors.ButtonEdit)sender).Text = value.ToString() + "  pixel";

            if (e.Button.Index == 2) // Uygula butonu
            {
                myImageCompress(value);
            }
        }

        /// <summary>
        /// Resmin üzerinde verilen h-w ye göre küçültme işlemi yapar.
        /// </summary>
        ///public string myImageCompress(Image Resim, int mypixel)
        ///
        public void myImageCompress(int newValue) //mypixel
        {
            //Size yeni_boyut = new Size(-1, -1);
            int oldWidth = 0;
            int oldHeight = 0;
            //float nPercent = 0;
            //float nPercentW = 0;
            //float nPercentH = 0;

            /// First we define a rectangle with the help of already calculated points
            ///if (this.OriginalImage == null)
            ///    this.OriginalImage = new Bitmap(pictureEdit1.Image, pictureEdit1.Image.Width, pictureEdit1.Image.Height);
            ///Bitmap workingImage = new Bitmap(pictureEdit1.Image, pictureEdit1.Image.Width, pictureEdit1.Image.Height);

            try
            {
                oldWidth = pictureEdit1.Image.Width;
                oldHeight = pictureEdit1.Image.Height;
                int newWidth = 0; 
                int newHeight = 0;

                preparingNewWidthNewHeight(newValue, oldWidth, oldHeight, ref newWidth, ref newHeight);

                /*
                /// Original image
                //Bitmap _img = new Bitmap(newWidth, newHeight, workingImage.PixelFormat);
                Bitmap _img = new Bitmap(newWidth, newHeight, pictureEdit1.Image.PixelFormat);
                _img.SetResolution(pictureEdit1.Image.HorizontalResolution, pictureEdit1.Image.VerticalResolution);

                /// for new image
                Graphics g = Graphics.FromImage(_img);
                /// create graphics
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                /// eskiden yeniye aktarım yapılıyor
                //g.DrawImage(workingImage, 0, 0, newWidth, newHeight);
                g.DrawImage(pictureEdit1.Image, 0, 0, newWidth, newHeight);
                */
                Bitmap workingImage = new Bitmap(pictureEdit1.Image, pictureEdit1.Image.Width, pictureEdit1.Image.Height);
                workingImage.SetResolution(pictureEdit1.Image.HorizontalResolution, pictureEdit1.Image.VerticalResolution);

                Bitmap _img = myImageCompress_(workingImage, newWidth, newHeight);

                pictureEdit1.Image = _img;

                Cursor = Cursors.Default;

                printImageProperties();

                //MessageBox.Show("Sıkıştırma işlemi gerçekleştirildi...");
                t.AlertMessage("Bilgilendirme", "Sıkıştırma işlemi gerçekleştirildi...");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }
        }

        private Bitmap myImageCompress_(Bitmap workingImage, int newWidth, int newHeight)
        {
            Bitmap _img = new Bitmap(newWidth, newHeight,  workingImage.PixelFormat);
            _img.SetResolution(workingImage.HorizontalResolution, workingImage.VerticalResolution);
            
            /// for new small image
            Graphics g = Graphics.FromImage(_img);
            /// create graphics
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            
            g.DrawImage(workingImage, 0, 0, newWidth, newHeight);

            return _img;
        }

        private void preparingNewWidthNewHeight(int newPixel, int oldWidth, int oldHeight, ref int newWidth, ref int newHeight)
        {
            /// Sıkıştıma işlemi için yeni newWidth ve newHeight hesaplanıyor
            /// 


            if (autoDPI == 400 && (autoCropHeight == oldHeight || autoCropWidth == oldWidth))
            {
                newWidth = oldWidth;
                newHeight = oldHeight;
                return;
            }

            Size yeni_boyut = new Size(-1, -1);
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            if ((oldWidth < 20) || (oldHeight < 20)) return;// string.Empty;

            if (newPixel < 10) newPixel = 10;

            yeni_boyut.Width = (oldWidth - newPixel);
            yeni_boyut.Height = (oldHeight - newPixel);

            nPercentW = ((float)yeni_boyut.Width / (float)oldWidth);
            nPercentH = ((float)yeni_boyut.Height / (float)oldHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
            }
            else
            {
                nPercent = nPercentW;
            }

            /// nPercent : küçültme oranı : 0.????
            /// bu sayede oldWidth küçülerek newWidth elde ediliyor

            /// tarayıcı tarafında kırıpılarak gelirse
            /// kullanıcı tarama sırasında önce önizleme yapıyor ve kırpılmış vaziyette tarama yaptırıyor ise
            /// 
            /// 600 dpi taranınca resim büyük geliyor
            if (autoDPI == 400 && (autoCropHeight != oldHeight || autoCropWidth != oldWidth))
            {
                int fark = 120;
                if (autoCropHeight == 512 && oldHeight > 512) fark = autoCropHeight + 140; // biyometrik için
                if (autoCropHeight == 472 && oldHeight > 472) fark = autoCropHeight + 10;  // imza için

                if (oldHeight > autoCropHeight && oldHeight < 2000)
                    nPercent = (float)(fark) / (float)oldHeight;  /// 650 / ??????
            }

            newWidth = (int)(oldWidth * nPercent);
            newHeight = (int)(oldHeight * nPercent);
        }

        #endregion Compress - Sıkştır

        #region DPI
        public void btn_DPI_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            tToolBox t = new tToolBox();

            //string FieldName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleName;
            //string FormName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleDescription;
            string txt = ((DevExpress.XtraEditors.ButtonEdit)sender).Text;
            int value = t.myInt32(txt);

            // 100 ile 1000 arası olsun ( 1000 maximum )

            if (e.Button.Index == 0) value = value - 50;
            if (e.Button.Index == 1) value = value + 50;
            if (value <= 100) value = 96;
            if (value > 1000) value = 1000;

            if (value == 46) value = 50;
            if (value == 146) value = 150;

            ((DevExpress.XtraEditors.ButtonEdit)sender).Text = value.ToString() + "  dpi";

            if (e.Button.Index == 2) // Uygula butonu
            {
                myImageDPI(value);
            }
        }

        /// <summary>
        /// Resmin dpi ayarlarını değiştirir.
        /// </summary>
        private void myImageDPI(int newDpi)
        {
            /// Dpi = Dots per inch , Bir inç karedeki nokta sayısı demektir. 
            /// Bir inch diye tabir edilen 2,54 cm’lik alanda bulunan nokta sayısına denir. 
            /// 2,54 cm x 2,54 cm kare içine düşen nokta sayısı. 
            /// Bir inç kare ye ne kadar çok nokta sığdırırsanız o kadar kaliteli sonuç elde edersiniz.
            ///
            /// Örnek verek olursak;
            /// 72 Dpi bir inç kare içinde 72 nokta demektir.
            /// 300 Dpi bir inç kare içinde 300 nokta demektir.
            /// 1440 Dpi bir inç kare içinde 1440 nokta demektir.

            try
            {
                Bitmap workingImage = null;
                int width = pictureEdit1.Image.Width;
                int height = pictureEdit1.Image.Height;
                float horizontalResolation = pictureEdit1.Image.HorizontalResolution;
                float verticalResolation = pictureEdit1.Image.VerticalResolution;

                workingImage = new Bitmap(pictureEdit1.Image, width, height);

                /// dpi değişikliğin en basit hali
                /// dpi change
                /// 
                /// Bitmap _img = new Bitmap(pictureEdit1.Image, width, height);
                /// _img.SetResolution(newDpi, newDpi);

                Bitmap _img = null;

                if (newDpi == autoDPI) // == 400
                {
                    //if (autoCropWidth > 0 && autoCropHeight > 0)
                    //    _img = new Bitmap(pictureEdit1.Image, autoCropWidth, autoCropHeight);
                    //else _img = new Bitmap(pictureEdit1.Image, width, height);
                    _img = new Bitmap(pictureEdit1.Image, width, height);
                    _img.SetResolution(newDpi, newDpi);
                }

                if (newDpi != autoDPI) // != 400
                {
                    _img = new Bitmap(pictureEdit1.Image, width, height);
                    _img.SetResolution(newDpi, newDpi);
                    
                    // yeni değerleri bunlar olsun
                    //autoCropWidth = width;
                    //autoCropHeight = height;
                    autoDPI = newDpi;
                }

                //pictureEdit1.Image = _img;

                // Klasör oluşturulur 
                string Images_Path = t.Find_Path("images") + tFileGuidName + ".jpg";

                _img.Save(Images_Path, ImageFormat.Jpeg);

                pictureEdit1.Image = Image.FromFile(Images_Path);

                printImageProperties();

                //MessageBox.Show("DPI düzenlemesi yapılmıştır...");
                t.AlertMessage("Bilgilendirme", "DPI düzenlemesi yapılmıştır...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                //throw;
            }
            
            //---

            #region başka bir yöntem
            /* 
            /// Burda gerçekleşen olay şu 
            /// bir resmi 96 dpi dan 300 dpi yüksekseltiğinde resmin width ve height de artması gerekiyor
            /// İşte burada bu büyümede gerçekleşiyor

            ///  300 > 96
            if (newDpi > horizontalResolation)
            {
               width = (int)((decimal)width * ((decimal)newDpi / (decimal)horizontalResolation));
               height = (int)((decimal)height * ((decimal)newDpi / (decimal)verticalResolation));
            }
                        
            /// yeni çizilen alan ölçüsü
            Rectangle rect = new Rectangle(0, 0, width, height);

            /// Original image
            Bitmap _img = new Bitmap(width, height);

            /// new dpi set
            _img.SetResolution(newDpi, newDpi);
                        
            /// for cropinf image
            Graphics g = Graphics.FromImage(_img);
            /// create graphics
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            
            /// set image attributes
            g.DrawImage(workingImage, 0, 0, rect, GraphicsUnit.Pixel);
            */
            #endregion

        }

        #endregion DPI

        #region Quality Kalite

        private void btn_Quality_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            tToolBox t = new tToolBox();

            //string FieldName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleName;
            //string FormName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleDescription;
            string txt = ((DevExpress.XtraEditors.ButtonEdit)sender).Text;
            int value = t.myInt32(txt);

            // 100 ile 1000 arası olsun ( 1000 maximum )

            if (e.Button.Index == 0) value = value - 10;
            if (e.Button.Index == 1) value = value + 10;
            if (value <= 10) value = 10;
            if (value > 100) value = 100;

            ((DevExpress.XtraEditors.ButtonEdit)sender).Text = value.ToString();

            if (e.Button.Index == 2) // Uygula butonu ise
            {
                myImageQuality(value);
            }
        }

        /// <summary>
        /// Resmin Quality ayarlarını değiştirir.
        /// </summary>
        private void myImageQuality(int newValue)
        {
            /// newValue 1 ile 100 arası olması gerekiyor
            ///   1 en düşük kalite
            /// 100 en yüksek kalite
            /// 
            
            ImageCodecInfo myImageCodecInfo;
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter = null;
            EncoderParameters myEncoderParameters = null;
            Bitmap workingImage = null;

            try
            {
                workingImage = new Bitmap(pictureEdit1.Image, pictureEdit1.Image.Width, pictureEdit1.Image.Height);

                /// önce resmin kalitesi yükseltilecek            
                myImageCodecInfo = TipBilgisi("image/jpeg");

                myEncoder = System.Drawing.Imaging.Encoder.Quality;

                myEncoderParameters = new EncoderParameters(1);

                // orjinali
                // myEncoderParameter = new EncoderParameter(myEncoder, 65L);
                myEncoderParameter = new EncoderParameter(myEncoder, (long)newValue);

                myEncoderParameters.Param[0] = myEncoderParameter;

                // Klasör oluşturulur 
                string Images_Path = t.Find_Path("images") + tFileGuidName + ".jpg";

                workingImage.Save(Images_Path, myImageCodecInfo, myEncoderParameters);

                pictureEdit1.Image = Image.FromFile(Images_Path);

                printImageProperties();

                t.AlertMessage("Bilgilendirme", "Resim kaliletesini düzenleme çalışması yapılmıştır...");
                //MessageBox.Show("Resim kaliletesini düzenleme çalışması yapılmıştır...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                //throw;
            }

            workingImage.Dispose();
            myEncoderParameter.Dispose();
            myEncoderParameters.Dispose();
        }

        private ImageCodecInfo TipBilgisi(string mimeType)
        {
            //Üst metodun yardımcı metodu.
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        #endregion Kalite

        #region Zoom events

        private void btn_Zoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btn_Zoom.EditValue = "100";
            barEditItem_ZoomLine.EditValue = btn_Zoom.EditValue.ToString();
            pictureEdit1.Properties.ZoomPercent = t.myInt32(barEditItem_ZoomLine.EditValue.ToString());
        }

        private void zoomTrackBarControl1_EditValueChanged(object sender, EventArgs e)
        {
            string mySender = ((DevExpress.XtraBars.BarEditItem)sender).Edit.Name.ToString();

            if ((this.activeControlName == "DevExpress.XtraEditors.ZoomTrackBarControl") ||
                (this.activeControlName == null))
            {
                btn_Zoom.EditValue = barEditItem_ZoomLine.EditValue.ToString();
                pictureEdit1.Properties.ZoomPercent = t.myInt32(barEditItem_ZoomLine.EditValue.ToString());

                printImageProperties();
            }
        }

        private void btn_Zoom_EditValueChanged(object sender, EventArgs e)
        {
            if (this.activeControlName == "DevExpress.XtraEditors.ButtonEdit")
            {
                barEditItem_ZoomLine.EditValue = btn_Zoom.EditValue.ToString();
                pictureEdit1.Properties.ZoomPercent = t.myInt32(barEditItem_ZoomLine.EditValue.ToString());

                printImageProperties();
            }
        }

        public void myControl_Enter(object sender, EventArgs e)
        {
            this.activeControlName = sender.GetType().ToString();
        }

        public void myControl_Leave(object sender, EventArgs e)
        {
            //
        }

        #endregion Zoom events


        #region subFunctions       
        
        private void dataBaseKaydet()
        {
            if (dNImages.Position > -1)
            {
                bool onay = imageKayitIcinUygunmu();

                if (onay == false) return;

                string idValue = dsImages.Tables[0].Rows[dNImages.Position]["Id"].ToString();
                string belgeAdi = dsImages.Tables[0].Rows[dNImages.Position]["LkpBelgeAdi"].ToString();
                string tableName = dsImages.Tables[0].Rows[dNImages.Position]["LkpTableName"].ToString();
                string fieldName = dsImages.Tables[0].Rows[dNImages.Position]["LkpFieldName"].ToString();
                string smallFieldName = dsImages.Tables[0].Rows[dNImages.Position]["LkpSmallFieldName"].ToString();
                string idFieldName = dsImages.Tables[0].Rows[dNImages.Position]["LkpIdFieldName"].ToString();
                string masterIdValue = dsImages.Tables[0].Rows[dNImages.Position]["LkpMasterIdValue"].ToString();


                // resim png formatına dönüyor
                //v.con_Images = t.imageBinaryArrayConverterMem((byte[])dsImages.Tables[0].Rows[dNImages.Position]["LkpImage"]);

                string fileGuid = tFileGuidName;
                string Images_Path = t.Find_Path("images") + fileGuid + ".jpg";
                Bitmap workingImage = new Bitmap(pictureEdit1.Image, pictureEdit1.Image.Width, pictureEdit1.Image.Height);
                workingImage.SetResolution(autoDPI, autoDPI);
                workingImage.Save(Images_Path, ImageFormat.Jpeg);
                pictureEdit1.Image = workingImage;

                long imageLength = 0;
                v.con_Images = t.imageBinaryArrayConverter(Images_Path, ref imageLength);
                v.con_Images_FieldName = fieldName;


                if (t.IsNotNull(smallFieldName))
                {
                    // % 80 oranında küçült
                    int newWidth = (int)(workingImage.Width * 0.2);
                    int newHeight = (int)(workingImage.Height * 0.2);

                    Bitmap _img = myImageCompress_(workingImage, newWidth, newHeight);

                    Images_Path = t.Find_Path("images") + fileGuid + "_Small.jpg";
                    _img.Save(Images_Path, ImageFormat.Jpeg);

                    v.con_Images2 = t.imageBinaryArrayConverter(Images_Path, ref imageLength);
                    v.con_Images_FieldName2 = smallFieldName;

                    dsImages.Tables[0].Rows[dNImages.Position]["LkpSmallImage"] = v.con_Images2;
                }


                dsImages.Tables[0].Rows[dNImages.Position]["LkpImage"] = v.con_Images;
                dsImages.Tables[0].AcceptChanges();

                string tSql = "";
                // büyük normal resmi kaydediyor

                if (idValue != "0")
                {
                    if (t.IsNotNull(smallFieldName) == false)
                        tSql = " Update " + tableName + " set "
                        + fieldName + " =  @" + fieldName
                        + " where " + idFieldName + " = " + idValue;
                    else // hem normal hemde small resmi aynı anda kaydediyor
                        tSql = " Update " + tableName + " set "
                        + fieldName + " =  @" + fieldName + ", "
                        + smallFieldName + " =  @" + smallFieldName
                        + " where " + idFieldName + " = " + idValue;
                }
                else
                {
                    if (v.SP_TabimDbConnection)
                    {
                        string dosyaIsmi = "";
                        if (belgeAdi == "Öğrenim Belgesi") dosyaIsmi = "OGRBELGE";
                        if (belgeAdi == "Sağlık Raporu") dosyaIsmi = "SAGLIK";
                        if (belgeAdi == "Adli Sicil Raporu") dosyaIsmi = "SAVCILIK";
                        if (belgeAdi == "İmza") dosyaIsmi = "IMZA";
                        if (belgeAdi == "Sözleşme 1") dosyaIsmi = "SOZLESME1";
                        if (belgeAdi == "Sözleşme 2") dosyaIsmi = "SOZLESME2";

                        tSql = " INSERT INTO [dbo].[EksEvraklar] ([UlasKursiyer],[DosyaIsmi],[Belge],[ET]) Values ( "
                        + masterIdValue + ", "
                        + "'" + dosyaIsmi + "', "
                        + " @" + fieldName + ", "
                        + " 1 )"
                        + " Select max("+idFieldName+") as " + idFieldName + ", 'dsInsert' as dsState from [dbo].[EksEvraklar] ";
                        /*
INSERT INTO [dbo].[EksEvraklar]
           ([UlasKursiyer]
           ,[DosyaIsmi]
           ,[Belge]
           ,[ET])
     VALUES
           (<UlasKursiyer, int,>
           ,<DosyaIsmi, varchar(15),>
           ,<Belge, image,>
           ,<ET, bit,>)
                        */
                    }
                }

                try
                {
                    tSave sv = new tSave();
                    vTable vt = new vTable();
                    t.Preparing_DataSet(this, dsImages, vt);
                    v.con_Refresh = sv.Record_SQL_RUN(dsImages, vt, "dsEdit", dNImages.Position, ref tSql, "");

                    t.AlertMessage(":)", "Resim başarıyla kaydedildi...");
                    //t.FlyoutMessage(this, ":)", "Resim başarıyla kaydedildi...");
                    //MessageBox.Show("Resim başarıyla kaydedildi...");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    //throw;
                }
            }
        }
        
        private bool imageKayitIcinUygunmu()
        {
            bool onay = true;

            getImageProperties(pictureEdit1);

            string mesaj = "";
            /// DPI uygunmu
            /// 
            if (autoDPI != tImageProperties.horizontalResolation)
            {
                DPI_Ayarla();
                /// Yeniden ayarlanan DPI yı tekrar kontrol et
                getImageProperties(pictureEdit1);

                if (autoDPI != tImageProperties.horizontalResolation)
                {

                    mesaj = "DPI uygun değil." + v.ENTER
                    + "   Gerekli olan DPI = " + autoDPI.ToString() + ",  mevcut DPI : " + tImageProperties.horizontalResolation.ToString() + v.ENTER
                    + "";
                    onay = false;
                }
            }

            if ((autoCropWidth > 0 && autoCropWidth != tImageProperties.width) ||
                (autoCropHeight > 0 && autoCropHeight != tImageProperties.height))
            {
                preparingAutoSize();
            }

            /// Width uygunmu
                /// 
            if (autoCropWidth > 0 && autoCropWidth != tImageProperties.width)
            {
                mesaj += "Resim genişlik uygun değil." + v.ENTER
                    + "   Gerekli olan genişlik = " + autoCropWidth.ToString() + ",  mevcut genişlik : " + tImageProperties.width.ToString() + v.ENTER
                    + "";
                onay = false;
            }

            /// Height uygunmu
            /// 
            if (autoCropHeight > 0 && autoCropHeight != tImageProperties.height)
            {
                mesaj += "Resim yüksekliği uygun değil." + v.ENTER
                    + "   Gerekli olan yükseklik = " + autoCropHeight.ToString() + ",  mevcut yükseklik : " + tImageProperties.height.ToString() + v.ENTER
                    + "";
                onay = false;
            }


            /// 100 kb uygunmu
            /// 
            if (onay)
            {
                if (tImageProperties.kbSize > 100)
                {
                    t.AlertMessage("Bilgilendirme", "Resim kayıt öncesi 100 kb altına düşürülmeye çalışılıyor...");

                    resmi100KbAltinaDusur();

                    /// resim üzerinde yeni işlem yapıldığı için tekrar kontrol gerekiyor
                    /// 
                    getImageProperties(pictureEdit1);

                    /// resim kb düşürme işlemi yapıldığı halde yinede uygun değilse
                    if (tImageProperties.kbSize > 100)
                    {
                        mesaj = "Resim 100 kb dan büyük olduğu için uygun değil." + v.ENTER
                        + "   Resim boyut 100 kb olması gerekiyor,  mevcut ise " + tImageProperties.kbSize.ToString() + " kb " + v.ENTER
                        + "";
                        onay = false;
                    }
                }
            }

            if (onay == false)
            {
                MessageBox.Show("DİKKAT : " + v.ENTER2 + mesaj);
            }

            return onay;
        }

        private void preparingAutoSize()
        {
            int oldWidth = pictureEdit1.Image.Width;
            int oldHeight = pictureEdit1.Image.Height;
            int newWidth = autoCropWidth;
            int newHeight = autoCropHeight;

            Bitmap workingImage = new Bitmap(pictureEdit1.Image, oldWidth, oldHeight);
            workingImage.SetResolution(pictureEdit1.Image.HorizontalResolution, pictureEdit1.Image.VerticalResolution);

            Bitmap _img = myImageCompress_(workingImage, newWidth, newHeight);

            getBitmapProperties(_img);

            pictureEdit1.Image = _img;
            printImageProperties();

            newHeight = (int)tImageProperties.height;
            newWidth = (int)tImageProperties.width;

            if (autoCropWidth == newWidth && autoCropHeight == newHeight)
                t.AlertMessage("Düzeltme", "Otomotik olarak genişlik ve yükseklik düzenlemesi yapıldı...");
            else t.AlertMessage("Başarısız Düzeltme", "Otomotik olarak genişlik ve yükseklik ayarlaması yapılamadı...");

        }

        private void dataBasedenResmiSil()
        {
            if (dNImages.Position > -1)
            {
                string idValue = dsImages.Tables[0].Rows[dNImages.Position]["Id"].ToString();
                string tableName = dsImages.Tables[0].Rows[dNImages.Position]["LkpTableName"].ToString();
                string fieldName = dsImages.Tables[0].Rows[dNImages.Position]["LkpFieldName"].ToString();
                string smallFieldName = dsImages.Tables[0].Rows[dNImages.Position]["LkpSmallFieldName"].ToString();
                string idFieldName = dsImages.Tables[0].Rows[dNImages.Position]["LkpIdFieldName"].ToString();

                if (t.IsNotNull(smallFieldName))
                {
                    dsImages.Tables[0].Rows[dNImages.Position]["LkpSmallImage"] = null;
                }

                dsImages.Tables[0].Rows[dNImages.Position]["LkpImage"] = null;
                dsImages.Tables[0].AcceptChanges();

                string tSql = "";
                // büyük normal resmi kaydediyor
                if (t.IsNotNull(smallFieldName) == false)
                    tSql = " Update " + tableName + " set "
                    + fieldName + " =  null "
                    + " where " + idFieldName + " = " + idValue;
                else // hem normal hemde small resmi aynı anda kaydediyor
                    tSql = " Update " + tableName + " set "
                    + fieldName + " =  null, "
                    + smallFieldName + " =  null "
                    + " where " + idFieldName + " = " + idValue;

                try
                {
                    tSave sv = new tSave();
                    vTable vt = new vTable();
                    t.Preparing_DataSet(this, dsImages, vt);
                    v.con_Refresh = sv.Record_SQL_RUN(dsImages, vt, "dsEdit", dNImages.Position, ref tSql, "");

                    t.AlertMessage(":)", "Resim başarıyla silindi...");
                    t.FlyoutMessage(this, ":)", "Resim başarıyla silindi...");
                    //MessageBox.Show("Resim başarıyla kaydedildi...");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    //throw;
                }
            }

        }

        private void preparing_OriginalImage()
        {
            OriginalImage = null;
            // resmin orjinalini sakla
            if (pictureEdit1.Image != null)
            {
                OriginalImage = new Bitmap(pictureEdit1.Image, pictureEdit1.Image.Width, pictureEdit1.Image.Height);
                OriginalImage.SetResolution(pictureEdit1.Image.HorizontalResolution, pictureEdit1.Image.VerticalResolution);
            }
        }

        private void dataNavigator_PositionChanged(object sender, EventArgs e)
        {
            preparing_OriginalImage();

            autoCropWidth = 0;
            autoCropHeight = 0;
            autoDPI = 0;

            if (dNImages.Position > -1)
            {
                this.imageType = dsImages.Tables[0].Rows[dNImages.Position]["LkpImageType"].ToString();
                autoCropWidth = t.myInt32(dsImages.Tables[0].Rows[dNImages.Position]["LkpWidth"].ToString());
                autoCropHeight = t.myInt32(dsImages.Tables[0].Rows[dNImages.Position]["LkpHeight"].ToString());
                autoDPI = t.myInt32(dsImages.Tables[0].Rows[dNImages.Position]["LkpDpi"].ToString());
            }

            if (imageType == "Normal")
                barEditItem_ResimTuru.EditValue = (short)0;
            if (imageType == "KiloByteKontrollu")
                barEditItem_ResimTuru.EditValue = (short)1;
            if (imageType == "Biyometrik")
                barEditItem_ResimTuru.EditValue = (short)2;
            if (imageType == "Imza")
                barEditItem_ResimTuru.EditValue = (short)3;


            //if (autoCropWidth == 0) autoCropWidth = 394;
            //if (autoCropHeight == 0) autoCropHeight = 512;
            if (autoDPI == 0) autoDPI = 96;

            cropWidth = autoCropWidth;
            cropHeight = autoCropHeight;

            btn_DPI.EditValue = autoDPI;

            this.barStaticItem_imgDPIIstenen.Caption = autoDPI.ToString() + " dpi";
            this.barStaticItem_imgWHIstenen.Caption = autoCropWidth.ToString() + " X " + autoCropHeight.ToString();
            this.barStaticItem_imgKbIstenen.Caption = "max : 100 kb";


            printImageProperties();
        }

        private void newImageProperties(string ImagesPath)
        {
            // yeni veya düzenleme görmüş image hakkındaki bilgiler
            getImageProperties(pictureEdit1);

            tImageProperties.imagePath = ImagesPath.ToString();
                
            printImageProperties();
        }

        private void getImageProperties(PictureEdit pictureEdit_)
        {
            if (pictureEdit_.Image == null) return;
            tImageProperties.Clear();
            int horResolation = (int)pictureEdit_.Image.HorizontalResolution;
            int verResolation = (int)pictureEdit_.Image.VerticalResolution;
            System.Drawing.Imaging.ImageFormat imageFormat = tGetImageFormat(pictureEdit_.Image);

            if (imageFormat == System.Drawing.Imaging.ImageFormat.MemoryBmp)
                imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;

            tImageProperties.imageFormat = imageFormat;
            tImageProperties.width = pictureEdit_.Image.Width;
            tImageProperties.height = pictureEdit_.Image.Height;
            tImageProperties.horizontalResolation = horResolation; // pictureEdit1.Image.HorizontalResolution;
            tImageProperties.verticalResolation = verResolation; // pictureEdit1.Image.VerticalResolution;
            long imageByteSize;
            using (var ms = new MemoryStream())
            {
                //pictureEdit1.Image.Save(ms, ImageFormat.Jpeg);
                pictureEdit_.Image.Save(ms, tImageProperties.imageFormat);
                imageByteSize = ms.Length;
            }
            tImageProperties.byteSize = imageByteSize;
            tImageProperties.kbSize = (int)((decimal)imageByteSize / 1024);
        }

        private void getBitmapProperties(Bitmap bitmap)
        {
            tImageProperties.Clear();

            int horResolation = (int)bitmap.HorizontalResolution;  //pictureEdit1.Image.HorizontalResolution;
            int verResolation = (int)bitmap.VerticalResolution;    //pictureEdit1.Image.VerticalResolution;
            System.Drawing.Imaging.ImageFormat imageFormat = tGetImageFormat(bitmap.RawFormat);
            
            if (imageFormat == System.Drawing.Imaging.ImageFormat.MemoryBmp)
                imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;

            tImageProperties.imageFormat = imageFormat;
            tImageProperties.width = bitmap.Width;   // pictureEdit1.Image.Width;
            tImageProperties.height = bitmap.Height; // pictureEdit1.Image.Height;
            tImageProperties.horizontalResolation = horResolation; // pictureEdit1.Image.HorizontalResolution;
            tImageProperties.verticalResolation = verResolation; // pictureEdit1.Image.VerticalResolution;
            long imageByteSize;
            using (var ms = new MemoryStream())
            {
                //pictureEdit1.Image.Save(ms, ImageFormat.Jpeg);
                bitmap.Save(ms, tImageProperties.imageFormat);
                imageByteSize = ms.Length;
            }
            tImageProperties.byteSize = imageByteSize;
            tImageProperties.kbSize = (int)((decimal)imageByteSize / 1024);
        }

        private void resmi100KbAltinaDusur()
        {
            ///  
            //myImageCompress(value);
            int oldWidth = 0;
            int oldHeight = 0;
            int newWidth = 0;
            int newHeight = 0;
            int newValue = 0;
            int farkValue = 0;
            int oldSize = 0;
            int newSize = 0;
            int farkSize = 0;
            bool onayCompress = false;

            getImageProperties(pictureEdit1);

            oldSize = (int)tImageProperties.kbSize;

            if (oldSize > 100)
            {
                /// Nedenini anlayamadığım bir şekilde mevcut Jpeg formatı PNG ye dönüşüyor
                /// onun için tekrar düzenleme yapılıyor
                //if (tImageProperties.imageFormat.ToString() != "Jpeg")
                //{
                    myImageDPI(autoDPI);

                    getImageProperties(pictureEdit1);

                    t.AlertMessage("Düzeltme", "Kb fazlası veya Jpeg olmadığı için DPI düzenlemesi yapıldı...");

                    oldSize = (int)tImageProperties.kbSize;
                //}

                /// yukarıdaki düzeltme işlemi yüzünden tekrar kontrol gerekiyor
                if (oldSize > 100)
                    onayCompress = true;
                else onayCompress = false;
            }

            if (onayCompress)
            {
                Bitmap _img = null;

                newValue = 300;
                farkValue = 10;

                t.WaitFormOpen(v.mainForm, "");
                t.WaitFormOpen(v.mainForm, "Resim 100 kb altına düşürülüyor...");

                while (onayCompress)
                {
                    oldWidth = pictureEdit1.Image.Width;
                    oldHeight = pictureEdit1.Image.Height;
                    newWidth = 0;
                    newHeight = 0;

                    Bitmap workingImage = new Bitmap(pictureEdit1.Image, oldWidth, oldHeight);
                    workingImage.SetResolution(pictureEdit1.Image.HorizontalResolution, pictureEdit1.Image.VerticalResolution);

                    preparingNewWidthNewHeight(newValue, oldWidth, oldHeight, ref newWidth, ref newHeight);

                    _img = myImageCompress_(workingImage, newWidth, newHeight);

                    getBitmapProperties(_img);

                    newSize = (int)tImageProperties.kbSize;
                    farkSize = 100 - newSize;

                    if (farkSize > 0 && farkSize <= 20 && newSize < 100)
                    {
                        pictureEdit1.Image = _img;
                        printImageProperties();
                        onayCompress = false; // işlem bitti
                    }
                    else
                    {
                        // yeni resim küçük, yalnız fazla küçülmüş 
                        if (farkSize > 20)
                        {
                            newValue = newValue - farkValue;
                            //farkValue -= 5;
                            if (farkValue <= 0) farkValue = 10;

                            if (newValue < 100)
                                onayCompress = false; // işlem bitti
                        }

                        // yeni resim halen 100 kd büyük
                        if (farkSize <= 0)
                        {
                            newValue = newValue + farkValue;
                            //farkValue += 10;

                            if (newValue > 10000)
                                onayCompress = false; // işlem bitti
                        }
                    }
                }

                v.SP_OpenApplication = false;
                v.IsWaitOpen = false;
                t.WaitFormClose();
            }
        }

        private void printImageProperties()
        {
            /// Resim Hakkında group
            ///

            if (pictureEdit1.Image == null) //(tImageProperties.originalImage == null)
            {
                barStaticItem_imgDPI.Caption = "0 dpi";
                barStaticItem_imgWH.Caption = "0 x 0";
                barStaticItem_imgKb.Caption = "0.0 kb";
                return;
            }

            getImageProperties(pictureEdit1);

            try
            {
                barStaticItem_imgDPI.Caption = tImageProperties.horizontalResolation.ToString() + " dpi";
                barStaticItem_imgWH.Caption = tImageProperties.width.ToString() + " x " + tImageProperties.height.ToString();
                barStaticItem_imgKb.Caption = tImageProperties.kbSize.ToString() + " kb";
                barStaticItem_Format.Caption = "Format : " + tImageProperties.imageFormat.ToString();

                onayWidth = (autoCropWidth == tImageProperties.width);
                onayHeight = (autoCropHeight == tImageProperties.height);
                onayDPI = (autoDPI == tImageProperties.horizontalResolation);
                onayFormat = (autoFormat == tImageProperties.imageFormat.ToString());

                onayKb = true;
                if (tImageProperties.kbSize > 100) onayKb = false;
            }
            catch (Exception)
            {
                //throw;
            }

            if (onayDPI && onayWidth && onayHeight && onayKb) 
            {
                btn_imgIstenenDuzenle.Enabled = false;
            }
            else
            {
                btn_imgIstenenDuzenle.Enabled = true;
            }

        }

        private System.Drawing.Imaging.ImageFormat tGetImageFormat(ImageFormat RawFormat)
        {
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                return System.Drawing.Imaging.ImageFormat.Bmp;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                return System.Drawing.Imaging.ImageFormat.Png;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Emf))
                return System.Drawing.Imaging.ImageFormat.Emf;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Exif))
                return System.Drawing.Imaging.ImageFormat.Exif;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                return System.Drawing.Imaging.ImageFormat.Gif;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                return System.Drawing.Imaging.ImageFormat.Icon;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.MemoryBmp))
                return System.Drawing.Imaging.ImageFormat.MemoryBmp;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
                return System.Drawing.Imaging.ImageFormat.Tiff;
            else
                return System.Drawing.Imaging.ImageFormat.Wmf;
        }
        private System.Drawing.Imaging.ImageFormat tGetImageFormat(System.Drawing.Image img)
        {
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                return System.Drawing.Imaging.ImageFormat.Bmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                return System.Drawing.Imaging.ImageFormat.Png;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Emf))
                return System.Drawing.Imaging.ImageFormat.Emf;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Exif))
                return System.Drawing.Imaging.ImageFormat.Exif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                return System.Drawing.Imaging.ImageFormat.Gif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                return System.Drawing.Imaging.ImageFormat.Icon;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.MemoryBmp))
                return System.Drawing.Imaging.ImageFormat.MemoryBmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
                return System.Drawing.Imaging.ImageFormat.Tiff;
            else
                return System.Drawing.Imaging.ImageFormat.Wmf;
        }
        
        /// <summary>
        /// İstenen sayıda guid döndürür. 
        /// Sayı çok önemli değil, resim create yapılmasının ardından save işlemi sonrası zaten pics altından resimler temizleniyor.
        /// </summary>
        private string tFileGuidName
        {
            get
            {
                string guid = Guid.NewGuid().ToString();
                return guid.Substring(0, 20);
            }
        }
        
        /// <summary>
        /// 
        /// Extension method for PictureEdit to convert screen coordinates to image coordinates.
        /// 
        private Point tImagePoint(PictureEdit pictureEdit, Point screenPoint)
        {
            if (viewInfo == null)
            {
                if (!tPictureEdit_CommonSetup(pictureEdit, ref hScrollBar, ref vScrollBar, ref viewInfo))
                    return Point.Empty;
            }

            this.zoomPercent = (int)pictureEdit1.Properties.ZoomPercent;

            //int imageX = tScreenToImage(screenPoint.X, (int)viewInfo.PicturePosition.X, hScrollBar, zoomPercent);
            //int imageY = tScreenToImage(screenPoint.Y, (int)viewInfo.PicturePosition.Y, vScrollBar, zoomPercent);
            int imageX = tScreenToImage(screenPoint.X, (int)this.viewInfo.PictureScreenBounds.X, this.hScrollBar, this.zoomPercent);
            int imageY = tScreenToImage(screenPoint.Y, (int)this.viewInfo.PictureScreenBounds.Y, this.vScrollBar, this.zoomPercent);

            //if (imageX < 0 || imageX >= pictureEdit.Image.Width ||
            //    imageY < 0 || imageY >= pictureEdit.Image.Height)
            //    return Point.Empty;
            if (imageX < 0) imageX = 1;
            if (imageY < 0) imageY = 1;

            return new Point(imageX, imageY);
        }

        /// Extension method for PictureEdit to convert image coordinates to screen coordinates, but
        /// without checking if the screen coordinate is in the displayable window. This can be an
        /// advantage in some situations, for example drawing a line from a point that is displayable
        /// to a point that is outside the window - let CGI do the clipping.
        /// 
        private Point tScreenPoint(PictureEdit pictureEdit, Point imagePoint)
        {
            if (viewInfo == null)
            {
                if (!tPictureEdit_CommonSetup(pictureEdit, ref hScrollBar, ref vScrollBar, ref viewInfo))
                    return Point.Empty;
            }

            int zoomPercent = (int)pictureEdit1.Properties.ZoomPercent;

            return new Point(tImageToScreen(imagePoint.X, (int)viewInfo.PicturePosition.X, hScrollBar, zoomPercent),
                             tImageToScreen(imagePoint.Y, (int)viewInfo.PicturePosition.Y, vScrollBar, zoomPercent));
        }

        private bool tPictureEdit_CommonSetup(PictureEdit pictureEdit,
                                              ref DevExpress.XtraEditors.HScrollBar hScrollBar,
                                              ref DevExpress.XtraEditors.VScrollBar vScrollBar,
                                              ref PictureEditViewInfo viewInfo)
        {
            if (pictureEdit.Image == null || pictureEdit.Controls.Count < 2)
                return false;

            vScrollBar = pictureEdit.Controls[0] as DevExpress.XtraEditors.VScrollBar;
            hScrollBar = pictureEdit.Controls[1] as DevExpress.XtraEditors.HScrollBar;
            viewInfo = pictureEdit.GetViewInfo() as PictureEditViewInfo;

            return hScrollBar != null && vScrollBar != null && viewInfo != null;
        }

        private int tScreenToImage(int screenCoord, int pictureStart,
                                   ScrollBarBase scrollBar, int ZoomPercent)
        {
            return (screenCoord + (scrollBar.Visible ? scrollBar.Value : -pictureStart)) *
                                                                                    100 / ZoomPercent;
        }

        private int tImageToScreen(int imageCoord, int pictureStart,
                                   ScrollBarBase scrollBar, int zoomPercent)
        {
            return
               imageCoord * zoomPercent / 100 - (scrollBar.Visible ? scrollBar.Value : -pictureStart);
        }

        private void checkedOrjinalImageControl()
        {
            if ((pictureEdit1.Image != null) && (OriginalImage == null))
            {
                OriginalImage = new Bitmap(pictureEdit1.Image, pictureEdit1.Image.Width, pictureEdit1.Image.Height);
                OriginalImage.SetResolution(pictureEdit1.Image.HorizontalResolution, pictureEdit1.Image.VerticalResolution);
            }
        }


        private void DPI_Ayarla()
        {
            if (onayDPI == false)
            {
                myImageDPI(autoDPI);
            }
        }

        private void Format_Ayarla()
        {
            if (onayFormat == false)
            {
                // Resim kalitesi aracılığı ile formatı değişitiriliyor
                //myImageQuality(100);  
                myImageDPI(autoDPI);
                t.AlertMessage("Bilgilendirme", "Resim formatı Jpeg yapılmıştır...");
                preparing_OriginalImage();
            }
        }


        #endregion subFunctions


        private void convertHtmlToPDFSpire(string ImagesPath)
        {
            //Specify the input URL and output PDF file path
            string inputUrl = @"https://www.e-iceblue.com/Tutorials/Spire.PDF/Spire.PDF-Program-Guide/C-/VB.NET-Convert-Image-to-PDF.html";
            string outputFile = @"HtmlToPDF.pdf";



            inputUrl = ImagesPath;
            outputFile = ImagesPath.Replace(".html", ".jpg");

            try
            {
                //Create a Document instance
                Document mydoc = new Document();
                //Load an HTML sample document
                mydoc.LoadFromFile(ImagesPath, Spire.Doc.FileFormat.Html, XHTMLValidationType.None);
                //Save to image. You can convert HTML to BMP, JPEG, PNG, GIF, Tiff etc
                Image image = mydoc.SaveToImages(0, ImageType.Bitmap);
                image.Save(outputFile, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception e)
            {

                //throw;
            }




            /*
            //Specify the path to the Chrome plugin
            string chromeLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            //Create an instance of the ChromeHtmlConverter class
            ChromeHtmlConverter converter = new ChromeHtmlConverter(chromeLocation);
            // Create an instance of the ConvertOptions class
            ConvertOptions options = new ConvertOptions();
            //Set conversion timeout
            options.Timeout = 10 * 3000;
            //Set paper size and page margins of the converted PDF
            options.PageSettings = new PageSettings()
            {
                PaperWidth = 8.27,
                PaperHeight = 11.69,
                MarginTop = 0,
                MarginLeft = 0,
                MarginRight = 0,
                MarginBottom = 0
            };
            //Convert the URL to PDF
            converter.ConvertToPdf(inputUrl, outputFile, options);
            */




        }


    }
}