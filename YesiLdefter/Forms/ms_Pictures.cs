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

        object tFileDataTable = null;
        bool loadDrives = false;
        string currentPath = string.Empty;
        string desktopPath = string.Empty;
        string myPicturesPath = string.Empty;
        string myDocumentsPath = string.Empty;

        int cropX;
        int cropY;
        int cropWidth;
        int cropHeight;
        int startCropX;
        int startCropY;
        int autoCropWidth;
        int autoCropHeight;
        int autoDPI;

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

        DataSet dsDataTarget = null;
        DataNavigator dNTarget = null;

        FilterInfoCollection fico;
        VideoCaptureDevice vcd;

        string imagesSourceFormName = "";
        string imagesSourceTableIPCode = "";
        string imagesSourceFieldName = "";
        string imagesMasterTableIPCode = "";
        string tarayiciYokMesaji = "Bilgisayarınıza bağlı tarayıcı tespit edilemedi.";
        bool IsActiveOlcek = false;
        string olcekName = "";

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
                        
            cropPen = new Pen(Color.Black, 2);
            cropPen.DashStyle = DashStyle.DashDotDot;

            cropPen2 = new Pen(Color.Red, 2);
            cropPen2.DashStyle = DashStyle.DashDotDot;

            cropPen3 = new Pen(Color.MediumSpringGreen, 4);
            cropPen3.DashStyle = DashStyle.DashDotDot;
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
            DeviceManager deviceManager = new DeviceManager();
            foreach (DeviceInfo deviceInfo in deviceManager.DeviceInfos)
            {
                if (deviceInfo.Type == WiaDeviceType.ScannerDeviceType)
                {
                    //Console.WriteLine("Scanner found: " + deviceInfo.Properties["Name"].get_Value());
                    imageComboBox_Tarayici.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(deviceInfo.Properties["Name"].get_Value(), (short)scanNo, -1));
                    scanNo++;
                }
            }

            if (scanNo == 1) // tarayıcı bulamadıysa
                imageComboBox_Tarayici.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(tarayiciYokMesaji, (short)1, -1));

            barEditItem_Tarayici.EditValue = (short)1;
            //imageComboBox_Tarayici.Items[0].Value = 1;
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
                        
            //if (t.IsNotNull(v.con_ImagesMasterTableIPCode))
            if (t.IsNotNull(tResimEditor.imagesMasterTableIPCode))
            {
                this.imagesSourceFormName = tResimEditor.imagesSourceFormName;  //v.con_ImagesSourceFormName;
                this.imagesSourceTableIPCode = tResimEditor.imagesSourceTableIPCode; // v.con_ImagesSourceTableIPCode;
                this.imagesSourceFieldName = tResimEditor.imagesSourceFieldName;// v.con_ImagesSourceFieldName;
                this.imagesMasterTableIPCode = tResimEditor.imagesMasterTableIPCode;// //v.con_ImagesMasterTableIPCode;

                // fieldName üzerinden small ifadesini çıkar
                this.imagesSourceFieldName = this.imagesSourceFieldName.Replace("Small", "");

                tInputPanel ip = new tInputPanel();
                ip.Create_InputPanel(this, splitContainerControl1.Panel1, this.imagesMasterTableIPCode, 1, true);

                t.Find_DataSet(this, ref dsDataTarget, ref dNTarget, this.imagesMasterTableIPCode);
                
                //pictureEdit1. için kaynak olan masterTableIPCode
                if (dsDataTarget != null)
                {
                    Control cntrl = t.Find_Control_View(this, this.imagesMasterTableIPCode);
                    
                    //dsDataTarget = null;
                    //dsDataTarget = v.con_ImagesMasterDataSet.Clone();
                    dsDataTarget = v.con_ImagesMasterDataSet.Copy();
                    dNTarget.DataSource = dsDataTarget.Tables[0];
                    dNTarget.PositionChanged += new System.EventHandler(dataNavigator_PositionChanged);
                    
                    if ((cntrl != null) &&
                        (dsDataTarget.Tables.Count > 0))
                    {
                        if (cntrl.GetType().ToString() == "DevExpress.XtraGrid.GridControl")
                            ((DevExpress.XtraGrid.GridControl)cntrl).DataSource = dsDataTarget.Tables[0];

                        if (cntrl.GetType().ToString() == "DevExpress.XtraVerticalGrid.VGridControl")
                            ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).DataSource = dsDataTarget.Tables[0];

                        if (cntrl.GetType().ToString() == "DevExpress.XtraDataLayout.DataLayoutControl")
                            ((DevExpress.XtraDataLayout.DataLayoutControl)cntrl).DataSource =  dsDataTarget.Tables[0];

                        if (cntrl.GetType().ToString() == "DevExpress.XtraTreeList.TreeList")
                            ((DevExpress.XtraTreeList.TreeList)cntrl).DataSource = dsDataTarget.Tables[0];

                        if (cntrl.GetType().ToString() == "DevExpress.XtraEditors.DataNavigator")
                            ((DevExpress.XtraEditors.DataNavigator)cntrl).DataSource = dsDataTarget.Tables[0];
                    }

                    // işlem yapılacak resimin row unu bul ve göster
                    int i = -1;
                    if (t.IsNotNull(dsDataTarget))
                        i = t.Find_GotoRecord(dsDataTarget, "LkpFieldName", this.imagesSourceFieldName);
                    dNTarget.Position = i;

                    // pictureEdit1 database bağla
                    pictureEdit1.DataBindings.Add(new Binding("EditValue", dsDataTarget.Tables[0], "LkpImage"));

                    // resim hakkındaki bilgiler okunsun ekrana basılsın
                    dataNavigator_PositionChanged((object)dNTarget, null);

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
                navigationPage_Resim.Image = t.Find_Glyph(glName);
                glName = "10_104_FolderPanel_32x32";
                navigationPage_Dosya.Image = t.Find_Glyph(glName);

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
            if (navigationPane1.SelectedPage == navigationPage_Resim)
                ribbonControl1.SelectedPage = ribbonControl1.Pages[0];
            if (navigationPane1.SelectedPage == navigationPage_Dosya)
                ribbonControl1.SelectedPage = ribbonControl1.Pages[1];
        }

        private void ribbonControl1_SelectedPageChanged(object sender, EventArgs e)
        {
            if (ribbonControl1.SelectedPage == ribbonControl1.Pages[0])
                navigationPane1.SelectedPage = navigationPage_Resim;
            if (ribbonControl1.SelectedPage == ribbonControl1.Pages[1])
                navigationPane1.SelectedPage = navigationPage_Dosya;
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
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Italic);
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
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
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

                DirectoryInfo info = new DirectoryInfo(path);
                try
                {
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
        }

        private void btn_TarayicidanAl_ItemClick(object sender, ItemClickEventArgs e)
        {
            string scannerName = barEditItem_Tarayici.EditValue.ToString();

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
                        Item item = device.Items[1];
                        WIA.CommonDialog dialog = new WIA.CommonDialog();
                
                        //ImgFile = (ImageFile)dialog.ShowTransfer(item, WiaFormatJPEG.WiaFormatJPEG, false);
                        ImgFile = (ImageFile)dialog.ShowAcquireImage(WiaDeviceType.ScannerDeviceType, WiaImageIntent.ColorIntent, WiaImageBias.MinimizeSize, "{00000000-0000-0000-0000-000000000000}", false, true, false);

                        ImgFile.SaveFile(ImagesPath);
                        MessageBox.Show("Tarama işlemi tamamlandı.");
                    }
                    else
                    {
                        MessageBox.Show("Tarayıcı bulunamadı.");
                    }

                    //Tarama bölümüne girip iptal dedikten sonra işlemler devam ediyor ve hataya sebebiyet veriyor. Kontrol altına alındı.img.savefile da patlıyor iptal dendiğinde.
                    //ImgFile.SaveFile(ImagesPath);

                    if (ImgFile != null)
                        pictureEdit1.Image = Image.FromFile(ImagesPath);

                    newImageProperties(ImagesPath);

                    // yeni resmin orjinal halide hafızaya alınsın
                    v.con_Images_MasterPath = "";
                    //v.con_Image_Original = null;

                    // Kaydet butonunu kayıt için diğer işlemleri hazırla
                    //Preparing_ImageSave(FormName, FieldName, Images_Path);
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
                    pictureEdit1.Image = pictureEdit2.Image;
                    if (vcd.IsRunning) vcd.Stop();
                    WebCamClick();
                }
                catch (Exception)
                {
                    //throw;
                }
            }
        }

        private void btn_DosyadanAl_ItemClick(object sender, ItemClickEventArgs e)
        {
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

                //string FieldName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;
                //string FormName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDescription;

                // yeni dosya/Image yükleniyor
                //v.con_Images_MasterPath = "";      //(orjinal dosya adı saklanıyor)
                //v.con_Image_Original = null;     //(orjinal resim image nesnesi üzerine alınıyor)

                pictureEdit1.Image = Image.FromFile(ImagesPath);

                //Preparing_ImageSave(FormName, FieldName, ImagesPath);
            }

            fdialog.Dispose();

            if (ImagesPath != null)
                newImageProperties(ImagesPath);
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

        private string imageName()
        {
            string tamAdi = "noname";
            if (t.IsNotNull(dsDataTarget))
            {
                tamAdi = dsDataTarget.Tables[0].Rows[dNTarget.Position]["TamAdiSoyadi"].ToString();
                tamAdi = tamAdi.Replace(" ", "");
                tamAdi = tamAdi + "_" + dsDataTarget.Tables[0].Rows[dNTarget.Position]["TcNo"].ToString() 
                                + "_" + dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpFieldName"].ToString();
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
                barButtonItem_Vazgec.Checked = true;
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
        }

        private void pictureEdit1_Paint(object sender, PaintEventArgs e)
        {
            //drawRectangle();
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
            kirpmaIslemi(true);
            this.olcekName = "Manuel";
        }
        private void kirpmaIslemi(bool IsActive)
        {
            this.barCheckItem_Otomatik.Enabled = !IsActive;
            this.barCheckItem_Manuel.Enabled = !IsActive;

            this.barButtonItem_Kirp.Enabled = IsActive;
            this.barButtonItem_Vazgec.Enabled = IsActive;

            this.IsActiveOlcek = IsActive;

            if (IsActive)
                checkedOrjinalImageControl();
            //else this.btnKirpOnayi.Enabled = false;
        }

        private void barButtonItem_Kirp_ItemClick(object sender, ItemClickEventArgs e)
        {
            myImageCrop();

            kirpmaIslemi(false);

            printImageProperties();

            MessageBox.Show("Kırpma işlemi gerçekleştirildi...");
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
                    ) myImageCrop();

                MessageBox.Show("Kırpma işlemi gerçekleştirildi...");
            }
        }

        private void myImageCrop()
        {
            if (cropWidth < 1) return;

            //if (this.zoomPercent <= 0)
            //    this.zoomPercent = (int)pictureEdit1.Properties.ZoomPercent;

            try
            {
                pictureEdit1.CreateGraphics().DrawRectangle(cropPen3, cropX, cropY, cropWidth, cropHeight);
                                
                /// zoom dan dolayı genişlik ve yükseklik yeniden hesaplanıyor
                /// 
                int oran = (int)(100 / (decimal)this.zoomPercent);
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
                _img.SetResolution(pictureEdit1.Image.HorizontalResolution, pictureEdit1.Image.VerticalResolution);
                /// for cropinf image
                Graphics gr = Graphics.FromImage(_img);
                /// create graphics
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                // yeni resmi oluşturuyor
                gr.DrawImage(pictureEdit1.Image, destinationRect, sourceRect, GraphicsUnit.Pixel);
                _img.SetResolution(autoDPI, autoDPI);
                // oluşan yeni image takrar pictureEdit1 yükleniyor
                pictureEdit1.Image = _img;
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

        //var croppedImage = CropImage(Image.FromStream(Request.Files[0].InputStream), x, y, 500, 500);
        public Bitmap CropImage(Image image, int x, int y, int width, int height)
        {
            Rectangle rectDestination = new Rectangle(x, y, width, height);
            Bitmap bmp = new Bitmap(rectDestination.Width, rectDestination.Height);
            Graphics gr = Graphics.FromImage(bmp);
            gr.CompositingQuality = CompositingQuality.Default;
            gr.SmoothingMode = SmoothingMode.Default;
            gr.InterpolationMode = InterpolationMode.Bicubic;
            gr.PixelOffsetMode = PixelOffsetMode.Default;
            gr.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), rectDestination, GraphicsUnit.Pixel);
            return bmp;
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
            if (value > 500) value = 500;

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
        public void myImageCompress(int newValue) //mypixel
        {
            Size yeni_boyut = new Size(-1, -1);
            int width = 0;
            int height = 0;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            /// First we define a rectangle with the help of already calculated points
            ///if (this.OriginalImage == null)
            ///    this.OriginalImage = new Bitmap(pictureEdit1.Image, pictureEdit1.Image.Width, pictureEdit1.Image.Height);
            ///Bitmap workingImage = new Bitmap(pictureEdit1.Image, pictureEdit1.Image.Width, pictureEdit1.Image.Height);

            try
            {
                width = pictureEdit1.Image.Width;
                height = pictureEdit1.Image.Height;

                if ((width < 20) || (height < 20)) return;// string.Empty;

                if (newValue < 10) newValue = 10;

                yeni_boyut.Width = (width - newValue);
                yeni_boyut.Height = (height - newValue);

                nPercentW = ((float)yeni_boyut.Width / (float)width);
                nPercentH = ((float)yeni_boyut.Height / (float)height);

                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                }
                else
                {
                    nPercent = nPercentW;
                }

                int newWidth = (int)(width * nPercent);
                int newHeight = (int)(height * nPercent);

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
                //workingImage.Dispose();

                printImageProperties();

                MessageBox.Show("Sıkıştırma işlemi gerçekleştirildi...");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }
        }

        private Bitmap myImageCompress_(Bitmap workingImage, int newWidth, int newHeight)
        {
            Bitmap _img = new Bitmap(newWidth, newHeight, workingImage.PixelFormat);
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
                    _img = new Bitmap(pictureEdit1.Image, autoCropWidth, autoCropHeight);
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

                pictureEdit1.Image = _img;

                printImageProperties();

                MessageBox.Show("DPI düzenlemesi yapılmıştır...");
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

                MessageBox.Show("Resim kaliletesini düzenleme çalışması yapılmıştır...");
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
            }
        }

        private void btn_Zoom_EditValueChanged(object sender, EventArgs e)
        {
            if (this.activeControlName == "DevExpress.XtraEditors.ButtonEdit")
            {
                barEditItem_ZoomLine.EditValue = btn_Zoom.EditValue.ToString();
                pictureEdit1.Properties.ZoomPercent = t.myInt32(barEditItem_ZoomLine.EditValue.ToString());
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
            if (dNTarget.Position > -1)
            {
                string idValue = dsDataTarget.Tables[0].Rows[dNTarget.Position]["Id"].ToString();
                string tableName = dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpTableName"].ToString();
                string fieldName = dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpFieldName"].ToString();
                string smallFieldName = dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpSmallFieldName"].ToString();
                string idFieldName = dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpIdFieldName"].ToString();


                // resim png formatına dönüyor
                //v.con_Images = t.imageBinaryArrayConverterMem((byte[])dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpImage"]);

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

                    dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpSmallImage"] = v.con_Images2;
                }


                dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpImage"] = v.con_Images;
                dsDataTarget.Tables[0].AcceptChanges();

                string tSql = "";
                // büyük normal resmi kaydediyor
                if (t.IsNotNull(smallFieldName) == false)
                    tSql = " Update " + tableName + " set "
                    + fieldName + " =  @" + fieldName
                    + " where " + idFieldName + " = " + idValue;
                else // hem normal hemde small resmi aynı anda kaydediyor
                    tSql = " Update " + tableName + " set "
                    + fieldName + " =  @" + fieldName + ", "
                    + smallFieldName + " =  @" + smallFieldName
                    + " where " + idFieldName + " = " + idValue;

                try
                {
                    tSave sv = new tSave();
                    vTable vt = new vTable();
                    t.Preparing_DataSet(this, dsDataTarget, vt);
                    v.con_Refresh = sv.Record_SQL_RUN(dsDataTarget, vt, "dsEdit", dNTarget.Position, ref tSql, "");

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
        
        private void dataBasedenResmiSil()
        {
            if (dNTarget.Position > -1)
            {
                string idValue = dsDataTarget.Tables[0].Rows[dNTarget.Position]["Id"].ToString();
                string tableName = dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpTableName"].ToString();
                string fieldName = dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpFieldName"].ToString();
                string smallFieldName = dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpSmallFieldName"].ToString();
                string idFieldName = dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpIdFieldName"].ToString();

                if (t.IsNotNull(smallFieldName))
                {
                    dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpSmallImage"] = null;
                }

                dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpImage"] = null;
                dsDataTarget.Tables[0].AcceptChanges();

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
                    t.Preparing_DataSet(this, dsDataTarget, vt);
                    v.con_Refresh = sv.Record_SQL_RUN(dsDataTarget, vt, "dsEdit", dNTarget.Position, ref tSql, "");

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

        private void dataNavigator_PositionChanged(object sender, EventArgs e)
        {
            // resmin orjinalini sakla
            if (pictureEdit1.Image != null)
            {
                OriginalImage = new Bitmap(pictureEdit1.Image, pictureEdit1.Image.Width, pictureEdit1.Image.Height);
                OriginalImage.SetResolution(pictureEdit1.Image.HorizontalResolution, pictureEdit1.Image.VerticalResolution);
            }
            else OriginalImage = null;

            string imageType = "Normal";

            autoCropWidth = 0;
            autoCropHeight = 0;
            autoDPI = 0;

            if (dNTarget.Position > -1)
            {
                imageType = dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpImageType"].ToString();
                autoCropWidth = t.myInt32(dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpWidth"].ToString());
                autoCropHeight = t.myInt32(dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpHeight"].ToString());
                autoDPI = t.myInt32(dsDataTarget.Tables[0].Rows[dNTarget.Position]["LkpDpi"].ToString());
            }

            if (imageType == "Normal")
                barEditItem_ResimTuru.EditValue = (short)0;
            if (imageType == "KiloByteKontrollu")
                barEditItem_ResimTuru.EditValue = (short)1;
            if (imageType == "Biyometrik")
                barEditItem_ResimTuru.EditValue = (short)2;

            if (autoCropWidth == 0) autoCropWidth = 394;
            if (autoCropHeight == 0) autoCropHeight = 512;
            if (autoDPI == 0) autoDPI = 96;

            cropWidth = autoCropWidth;
            cropHeight = autoCropHeight;

            btn_DPI.EditValue = autoDPI;

            printImageProperties();
        }

        private void newImageProperties(string ImagesPath)
        {
            // yeni veya düzenleme görmüş image hakkındaki bilgiler
            getImageProperties();

            tImageProperties.imagePath = ImagesPath.ToString();
                
            printImageProperties();
        }

        private void getImageProperties()
        {
            tImageProperties.Clear();
            int horResolation = (int)pictureEdit1.Image.HorizontalResolution;
            int verResolation = (int)pictureEdit1.Image.VerticalResolution;
            System.Drawing.Imaging.ImageFormat imageFormat = tGetImageFormat(pictureEdit1.Image);

            if (imageFormat == System.Drawing.Imaging.ImageFormat.MemoryBmp)
                imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;

            tImageProperties.imageFormat = imageFormat;
            tImageProperties.width = pictureEdit1.Image.Width;
            tImageProperties.height = pictureEdit1.Image.Height;
            tImageProperties.horizontalResolation = horResolation; // pictureEdit1.Image.HorizontalResolution;
            tImageProperties.verticalResolation = verResolation; // pictureEdit1.Image.VerticalResolution;
            long imageByteSize;
            using (var ms = new MemoryStream())
            {
                //pictureEdit1.Image.Save(ms, ImageFormat.Jpeg);
                pictureEdit1.Image.Save(ms, tImageProperties.imageFormat);
                imageByteSize = ms.Length;
            }
            tImageProperties.byteSize = imageByteSize;
            tImageProperties.kbSize = (decimal)((decimal)imageByteSize / 1000);
        }

        private void printImageProperties()
        {
            /// Resim Hakkında group
            ///
            if (pictureEdit1.Image == null) //(tImageProperties.originalImage == null)
            {
                barStaticItem_imgKb.Caption = "0.0 kb";
                barStaticItem_imgWH.Caption = "0 x 0";
                barStaticItem_imgDPI.Caption = "0 dpi";
                return;
            }

            getImageProperties();

            try
            {
                barStaticItem_imgKb.Caption = tImageProperties.kbSize.ToString() + " kb";
                barStaticItem_imgWH.Caption = tImageProperties.width.ToString() + " x " + tImageProperties.height.ToString();
                barStaticItem_Format.Caption = "Format : " + tImageProperties.imageFormat.ToString();
                barStaticItem_imgDPI.Caption = tImageProperties.horizontalResolation.ToString() + " dpi";
            }
            catch (Exception)
            {
                //throw;
            }
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

        #endregion subFunctions
    }
}