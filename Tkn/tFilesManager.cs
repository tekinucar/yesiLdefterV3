using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using DevExpress.Utils.Helpers;
using DevExpress.XtraGrid.Views.WinExplorer;
using DevExpress.XtraEditors;

using Tkn_ToolBox;

namespace Tkn_FileManager
{
    public class tFilesManager : tBase
    {
        tToolBox t = new tToolBox();

        object tFileDataTable = null;
        bool loadDrives = false;
        string currentPath;


        protected string StartupPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.Desktop); } }
        //public BreadCrumbEdit BreadCrumb { get { return editBreadCrumb; } }

        // Form üzerindeki mevcut bir nesneyi bulduktan sonra onu bir değişken gibi kullnamak için bir metod

        //public DevExpress.XtraGrid.GridControl mygridControl(Form tForm)
        //{
        //    string[] controls = new string[] { };
        //    Control cntrl = null;
        //    cntrl = t.Find_Control(tForm, "gridControl", "", controls);
        //    return ((DevExpress.XtraGrid.GridControl)cntrl);
        //} 
        private DevExpress.XtraGrid.GridControl gridControlFile = null;
        private DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView winExplorerViewFile = null;
        private DevExpress.XtraEditors.BreadCrumbEdit BreadCrumb = null;

        public void CreateFileManager(Control control)
        {

            Form tForm = t.Find_Form(control);

            //string[] controls = new string[] { };
            //Control splitContainer1 = null;
            //splitContainer1 = t.Find_Control(tForm, "splitContainer1", "", controls);
                        
            #region 
            DevExpress.XtraEditors.PanelControl panelControl1 = new DevExpress.XtraEditors.PanelControl();
            DevExpress.XtraEditors.LabelControl btnUpTo = new DevExpress.XtraEditors.LabelControl();
            DevExpress.XtraEditors.LabelControl btnNavigationHistory = new DevExpress.XtraEditors.LabelControl();
            DevExpress.XtraEditors.LabelControl btnForward = new DevExpress.XtraEditors.LabelControl();
            DevExpress.XtraEditors.LabelControl btnBack = new DevExpress.XtraEditors.LabelControl();
            System.Windows.Forms.SplitContainer splitContainer1 = new System.Windows.Forms.SplitContainer();
            System.Windows.Forms.SplitContainer splitContainer2 = new System.Windows.Forms.SplitContainer();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(splitContainer1);
            panelControl1.Controls.Add(btnUpTo);
            panelControl1.Controls.Add(btnNavigationHistory);
            panelControl1.Controls.Add(btnForward);
            panelControl1.Controls.Add(btnBack);
            panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            panelControl1.Location = new System.Drawing.Point(2, 20);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new System.Drawing.Size(769, 33);
            panelControl1.TabIndex = 7;
            // 
            // btnUpTo
            // 
            btnUpTo.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            btnUpTo.Appearance.ImageIndex = 5;
            btnUpTo.AppearanceHovered.ImageIndex = 2;
            btnUpTo.AppearancePressed.ImageIndex = 8;
            btnUpTo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            btnUpTo.Dock = System.Windows.Forms.DockStyle.Left;
            btnUpTo.Location = new System.Drawing.Point(68, 2);
            btnUpTo.Name = "btnUpTo";
            btnUpTo.Size = new System.Drawing.Size(25, 29);
            btnUpTo.TabIndex = 11;
            // 
            // btnNavigationHistory
            // 
            btnNavigationHistory.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            btnNavigationHistory.Appearance.ImageIndex = 2;
            btnNavigationHistory.AppearanceHovered.ImageIndex = 1;
            btnNavigationHistory.AppearancePressed.ImageIndex = 3;
            btnNavigationHistory.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            btnNavigationHistory.Dock = System.Windows.Forms.DockStyle.Left;
            btnNavigationHistory.Location = new System.Drawing.Point(52, 2);
            btnNavigationHistory.Name = "btnNavigationHistory";
            btnNavigationHistory.Size = new System.Drawing.Size(16, 29);
            btnNavigationHistory.TabIndex = 10;
            // 
            // btnForward
            // 
            btnForward.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            btnForward.Appearance.ImageIndex = 4;
            btnForward.AppearanceHovered.ImageIndex = 1;
            btnForward.AppearancePressed.ImageIndex = 7;
            btnForward.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            btnForward.Dock = System.Windows.Forms.DockStyle.Left;
            btnForward.Location = new System.Drawing.Point(27, 2);
            btnForward.Name = "btnForward";
            btnForward.Size = new System.Drawing.Size(25, 29);
            btnForward.TabIndex = 8;
            // 
            // btnBack
            // 
            btnBack.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            btnBack.Appearance.ImageIndex = 3;
            btnBack.AppearanceHovered.ImageIndex = 0;
            btnBack.AppearancePressed.ImageIndex = 6;
            btnBack.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            btnBack.Dock = System.Windows.Forms.DockStyle.Left;
            btnBack.Location = new System.Drawing.Point(2, 2);
            btnBack.Name = "btnBack";
            btnBack.Size = new System.Drawing.Size(25, 29);
            btnBack.TabIndex = 7;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(93, 2);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Size = new System.Drawing.Size(674, 29);
            splitContainer1.SplitterDistance = 449;
            splitContainer1.TabIndex = 12;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point(2, 53);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Size = new System.Drawing.Size(769, 439);
            splitContainer2.SplitterDistance = 256;
            splitContainer2.TabIndex = 8;

            control.Controls.Add(splitContainer2);
            control.Controls.Add(panelControl1);

            #endregion


            // 
            #region // treeList1
            //
            DevExpress.XtraTreeList.TreeList treeList1 = new DevExpress.XtraTreeList.TreeList();
            DevExpress.XtraTreeList.Columns.TreeListColumn colName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            DevExpress.XtraTreeList.Columns.TreeListColumn colType = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            DevExpress.XtraTreeList.Columns.TreeListColumn colSize = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
            treeList1.Location = new System.Drawing.Point(2, 2);
            treeList1.Name = "treeList1";
            treeList1.OptionsBehavior.Editable = false;
            treeList1.OptionsView.EnableAppearanceEvenRow = true;
            treeList1.Size = new System.Drawing.Size(252, 355);
            treeList1.TabIndex = 1;
            // 
            // colName
            // 
            colName.Caption = "Name";
            colName.FieldName = "Name";
            colName.MinWidth = 33;
            colName.Name = "colName";
            colName.Visible = true;
            colName.VisibleIndex = 0;
            colName.Width = 369;
            // 
            // colType
            // 
            colType.Caption = "Type";
            colType.FieldName = "Type";
            colType.Name = "colType";
            colType.Width = 117;
            // 
            // colSize
            // 
            colSize.Caption = "Size(Bytes)";
            colSize.FieldName = "Size";
            colSize.Name = "colSize";
            colSize.Width = 117;
            // 
            // TreeList Add Columns
            // 
            treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] { colName, colType, colSize });
            //
            // TreeList Add Events 
            //
            PopulateTreeList(ref treeList1, "");
            #endregion treeList

            #region // GridControl

            DevExpress.XtraGrid.GridControl gridControl = new DevExpress.XtraGrid.GridControl();
            DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView winExplorerView = new DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView();
            DevExpress.XtraGrid.Columns.GridColumn columnName = new DevExpress.XtraGrid.Columns.GridColumn();
            DevExpress.XtraGrid.Columns.GridColumn columnPath = new DevExpress.XtraGrid.Columns.GridColumn();
            DevExpress.XtraGrid.Columns.GridColumn columnCheck = new DevExpress.XtraGrid.Columns.GridColumn();
            DevExpress.XtraGrid.Columns.GridColumn columnGroup = new DevExpress.XtraGrid.Columns.GridColumn();
            DevExpress.XtraGrid.Columns.GridColumn columnImage = new DevExpress.XtraGrid.Columns.GridColumn();
            // 
            // gridControl
            // 
            gridControl.AllowDrop = true;
            gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridControl.Location = new System.Drawing.Point(0, 0);
            gridControl.MainView = winExplorerView;
            //gridControl.MenuManager = this.RibbonControl;
            gridControl.Name = "gridControl";
            gridControl.Size = new System.Drawing.Size(416, 217);
            gridControl.TabIndex = 3;
            gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            winExplorerView});
            // 
            // winExplorerView
            // 
            winExplorerView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            winExplorerView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            columnName,
            columnPath,
            columnCheck,
            columnGroup,
            columnImage});
            winExplorerView.ColumnSet.CheckBoxColumn = columnCheck;
            winExplorerView.ColumnSet.DescriptionColumn = columnPath;
            winExplorerView.ColumnSet.ExtraLargeImageColumn = columnImage;
            winExplorerView.ColumnSet.GroupColumn = columnGroup;
            winExplorerView.ColumnSet.LargeImageColumn = columnImage;
            winExplorerView.ColumnSet.MediumImageColumn = columnImage;
            winExplorerView.ColumnSet.SmallImageColumn = columnImage;
            winExplorerView.ColumnSet.TextColumn = columnName;
            winExplorerView.GridControl = gridControl;
            winExplorerView.GroupCount = 1;
            winExplorerView.Name = "winExplorerView";
            winExplorerView.OptionsBehavior.Editable = false;
            winExplorerView.OptionsSelection.AllowMarqueeSelection = true;
            winExplorerView.OptionsSelection.ItemSelectionMode = DevExpress.XtraGrid.Views.WinExplorer.IconItemSelectionMode.Click;
            winExplorerView.OptionsSelection.MultiSelect = true;
            winExplorerView.OptionsView.DrawCheckedItemsAsSelected = true;
            winExplorerView.OptionsView.ImageLayoutMode = DevExpress.Utils.Drawing.ImageLayoutMode.Stretch;
            winExplorerView.OptionsView.ShowCheckBoxInGroupCaption = true;
            winExplorerView.OptionsView.ShowExpandCollapseButtons = true;
            winExplorerView.OptionsView.ShowViewCaption = true;
            winExplorerView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(columnGroup, DevExpress.Data.ColumnSortOrder.Ascending)});
            //winExplorerView.ItemClick += new DevExpress.XtraGrid.Views.WinExplorer.WinExplorerViewItemClickEventHandler(OnWinExplorerViewItemClick);
            winExplorerView.ItemDoubleClick += new DevExpress.XtraGrid.Views.WinExplorer.WinExplorerViewItemDoubleClickEventHandler(myOnWinExplorerViewItemDoubleClick);
            //winExplorerView.KeyDown += new System.Windows.Forms.KeyEventHandler(OnWinExplorerViewKeyDown);

            // 
            // columnName
            // 
            columnName.Caption = "columnName";
            columnName.FieldName = "Name";
            columnName.Name = "columnName";
            columnName.Visible = true;
            columnName.VisibleIndex = 0;
            // 
            // columnPath
            // 
            columnPath.Caption = "columnPath";
            columnPath.FieldName = "Path";
            columnPath.Name = "columnPath";
            columnPath.Visible = true;
            columnPath.VisibleIndex = 0;
            // 
            // columnCheck
            // 
            columnCheck.Caption = "columnCheck";
            columnCheck.FieldName = "IsCheck";
            columnCheck.Name = "columnCheck";
            columnCheck.Visible = true;
            columnCheck.VisibleIndex = 0;
            // 
            // columnGroup
            // 
            columnGroup.Caption = "columnGroup";
            columnGroup.FieldName = "Group";
            columnGroup.Name = "columnGroup";
            columnGroup.Visible = true;
            columnGroup.VisibleIndex = 0;
            // 
            // columnImage
            // 
            columnImage.Caption = "columnImage";
            columnImage.FieldName = "Image";
            columnImage.Name = "columnImage";
            columnImage.Visible = true;
            columnImage.VisibleIndex = 0;


            gridControlFile = gridControl;
            winExplorerViewFile = winExplorerView;

            #endregion GridControl
            
            #region // BreadCrumbEdit 
            DevExpress.XtraEditors.BreadCrumbEdit editBreadCrumb = new DevExpress.XtraEditors.BreadCrumbEdit();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.XtraEditors.BreadCrumbNode breadCrumbNode1 = new DevExpress.XtraEditors.BreadCrumbNode();
            DevExpress.XtraEditors.BreadCrumbNode breadCrumbNode2 = new DevExpress.XtraEditors.BreadCrumbNode();
            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(((Form)tForm));
            
            // 
            // editBreadCrumb
            // 
            editBreadCrumb.Dock = System.Windows.Forms.DockStyle.Fill;
            editBreadCrumb.Location = new System.Drawing.Point(4, 4);
            editBreadCrumb.Name = "editBreadCrumb";
            editBreadCrumb.Properties.AutoHeight = false;
            editBreadCrumb.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinDown, "", 18, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "", null, null, true),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Redo, "", 15, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject2, "", null, null, true)});
            editBreadCrumb.Properties.DropDownRows = 12;
            editBreadCrumb.Properties.ImageIndex = 0;
            breadCrumbNode1.Caption = "Root";
            breadCrumbNode1.Persistent = true;
            breadCrumbNode1.PopulateOnDemand = true;
            breadCrumbNode1.ShowCaption = false;
            breadCrumbNode1.Value = "Root";
            breadCrumbNode2.Caption = "Computer";
            breadCrumbNode2.Persistent = true;
            breadCrumbNode2.PopulateOnDemand = true;
            breadCrumbNode2.Value = "Computer";
            editBreadCrumb.Properties.Nodes.AddRange(new DevExpress.XtraEditors.BreadCrumbNode[] {
            breadCrumbNode1,
            breadCrumbNode2});
            editBreadCrumb.Properties.RootImageIndex = 0;
            editBreadCrumb.Properties.SortNodesByCaption = true;
            editBreadCrumb.Size = new System.Drawing.Size(428, 21);
            editBreadCrumb.TabIndex = 14;
            
            DevExpress.XtraEditors.ButtonEdit EditSearch = new DevExpress.XtraEditors.ButtonEdit();
            // 
            // EditSearch
            // 
            EditSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            EditSearch.Location = new System.Drawing.Point(4, 4);
            EditSearch.Name = "EditSearch";
            EditSearch.Properties.AutoHeight = false;
            EditSearch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            //new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("EditSearch.Properties.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject3, "", null, null, true)});
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(null)), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject3, "", null, null, true)});
            EditSearch.Size = new System.Drawing.Size(226, 21);
            EditSearch.TabIndex = 12;

            BreadCrumb = editBreadCrumb;
            InitializeBreadCrumb();

            #endregion // BreadCrumbEdit
                     
            splitContainer1.Panel1.Controls.Add(editBreadCrumb);
            splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(4);
            splitContainer1.Panel2.Controls.Add(EditSearch);
            splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(4);

            splitContainer2.Panel1.Controls.Add(treeList1);
            splitContainer2.Panel2.Controls.Add(gridControl);

            // metod 1
            //string[] controls = new string[] { };
            //Control grid = null;
            //grid = t.Find_Control(tForm, "gridControl", "", controls);

            // metod 2
            //DevExpress.XtraGrid.GridControl grid = mygridControl(tForm);

            this.currentPath = "c:\\Preject_S";
                        
            DirectoryView(tForm);

        }

        //
        // TreeList
        //
        public void PopulateTreeList(ref DevExpress.XtraTreeList.TreeList treeList1, string path)
        {
            treeList1.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(mytreeList_CustomDrawNodeCell);
            treeList1.VirtualTreeGetChildNodes += new DevExpress.XtraTreeList.VirtualTreeGetChildNodesEventHandler(mytreeList_VirtualTreeGetChildNodes);
            treeList1.VirtualTreeGetCellValue += new DevExpress.XtraTreeList.VirtualTreeGetCellValueEventHandler(mytreeList_VirtualTreeGetCellValue);
            treeList1.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(mytreeList_GetStateImage);
            treeList1.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(mytreeList_FocusedNodeChanged);

            treeList1.DataSource = new object();
        }

        public void mytreeList_GetStateImage(object sender, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            if (e.Node.GetDisplayText("Type") == "Folder")
                e.NodeImageIndex = e.Node.Expanded ? 1 : 0;
            else if (e.Node.GetDisplayText("Type") == "File") e.NodeImageIndex = 2;
            else e.NodeImageIndex = 3;
        }

        public void mytreeList_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            //if (e.Column == this.colSize)
            if (e.Column.Name == "colSize")
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

            //if (e.Column == this.colName)
            if (e.Column.Name == "colName")
            {
                if (e.Node.GetDisplayText("Type") == "File")
                {
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
        }

        public void mytreeList_VirtualTreeGetChildNodes(object sender, DevExpress.XtraTreeList.VirtualTreeGetChildNodesInfo e)
        {
            Cursor current = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            if (!loadDrives)
            {
                string[] roots = Directory.GetLogicalDrives();
                e.Children = roots;
                loadDrives = true;
            }
            else {
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

        public void mytreeList_VirtualTreeGetCellValue(object sender, DevExpress.XtraTreeList.VirtualTreeGetCellValueInfo e)
        {
            DirectoryInfo di = new DirectoryInfo((string)e.Node);
            if (e.Column.Name == "colName") e.CellData = di.Name;
            if (e.Column.Name == "colType")
            {
                if (IsDrive((string)e.Node)) e.CellData = "Drive";
                else if (!IsFile(di))
                    e.CellData = "Folder";
                else
                    e.CellData = "File";
            }
            if (e.Column.Name == "colSize")
            {
                if (IsFile(di))
                {
                    e.CellData = new FileInfo((string)e.Node).Length;
                }
                else e.CellData = null;
            }

        }

        public void mytreeList_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
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

                Form tForm = t.Find_Form(sender);

                DirectoryView(tForm);
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

        //
        // DevExpress.WinExplorer
        //
        #region // DevExpress.WinExplorer

        void DirectoryView(Form tForm)
        {
            if (t.IsNotNull(this.currentPath) == false) return;
            /*
            string[] controls = new string[] { };
            Control grid = null;
            grid = t.Find_Control(tForm, "gridControl", "", controls);

            DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView winExplorerView =
                    ((DevExpress.XtraGrid.GridControl)grid).MainView as DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView;
            */
            Cursor oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                if (!string.IsNullOrEmpty(this.currentPath))
                {

                    try
                    {
                        /*
                        ((DevExpress.XtraGrid.GridControl)grid).DataSource = 
                            FileSystemHelper.GetFileSystemEntries(
                                this.currentPath, 
                                GetItemSizeType(((DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView)winExplorerView).OptionsView.Style), // GetItemSizeType(ViewStyle)
                                GetItemSize(((DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView)winExplorerView).OptionsView.Style)      // GetItemSize(ViewStyle) 
                                );
                        */

                        gridControlFile.DataSource =
                            FileSystemHelper.GetFileSystemEntries(
                                this.currentPath,
                                GetItemSizeType(winExplorerViewFile.OptionsView.Style), // GetItemSizeType(ViewStyle)
                                GetItemSize(winExplorerViewFile.OptionsView.Style)      // GetItemSize(ViewStyle) 
                                );


                        #region // resimlerin yüklemesi

                        //tFileDataTable = ((DevExpress.XtraGrid.GridControl)grid).DataSource;
                        tFileDataTable = gridControlFile.DataSource;

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
                                        //var fs = (FileSystemEntry)winExplorerView.GetRow(ii);
                                        var fs = (FileSystemEntry)winExplorerViewFile.GetRow(ii);
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

                            //for (int i = 0; i < this.winExplorerView.RowCount; i++) this.winExplorerView.InvertRowSelection(i);

                            /*
                            for (int i = 0; i < this.winExplorerView.RowCount-1; i++)
                            {
                                this.winExplorerView.InvertRowSelection(i);
                                var fs = (FileSystemEntry)winExplorerView.GetRow(i);
                                //s = s + fs.Name + v.ENTER;

                            }
                            */
                            //MessageBox.Show(s);

                            /*
                            //int[] rows = winExplorerView.GetSelectedRows();
                            int[] rows = winExplorerView.get
                            for (int i = 0; i < rows.Length; i++)
                            {
                                //list.Add((FileSystemEntry)winExplorerView.GetRow(rows[i]));
                            }
                            */


                        } // if (tDataTable != null)
                        #endregion // resimlerin yüklemesi
                    }
                    catch (Exception)
                    {
                        //((DevExpress.XtraGrid.GridControl)grid).DataSource = null;
                        gridControlFile.DataSource = null;
                    }
                }
                else
                    gridControlFile.DataSource = null;

               
                //((DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView)winExplorerView).RefreshData();
                winExplorerViewFile.RefreshData();
                //EnsureSearchEdit();
                //BeginInvoke(
                //new MethodInvoker(((DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView)winExplorerView).ClearSelection));
            }
            finally
            {
                Cursor.Current = oldCursor;
            }

        }

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

        //public WinExplorerViewStyle ViewStyle { get { return this.winExplorerView.OptionsView.Style; } }

        void EnsureSearchEdit()
        {
            //EditSearch.Properties.NullText = "Search " + FileSystemHelper.GetDirName(this.currentPath);
            //EditSearch.EditValue = null;
            //this.winExplorerView.FindFilterText = string.Empty;
        }

        void myOnWinExplorerViewItemDoubleClick(object sender, WinExplorerViewItemDoubleClickEventArgs e)
        {

            if (e.MouseInfo.Button != MouseButtons.Left) return;
            ((DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView)sender).ClearSelection();
            //((FileSystemEntry)e.ItemInfo.Row.RowKey).DoAction(this);
                        
            FileSystemEntry selectitem = (FileSystemEntry)((DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView)sender).GetRow(((DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView)sender).FocusedRowHandle);
            
            //this.Text = selectitem.Path.ToString();
            this.currentPath = selectitem.Path.ToString();

            Form tForm = t.Find_Form(sender);

            //DirectoryView(tForm);

            /*
            FileSystemInfo selectitem2 = new FileInfo(selectitem.Path.ToString());

            MessageBox.Show(selectitem2.Attributes.ToString());
            */

            FileAttributes attributes = File.GetAttributes(selectitem.Path.ToString());

            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DirectoryView(tForm);
            }

            if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
            {
                MessageBox.Show(this.currentPath);
            }




            //  Process.Start(((sender as TreeList).FocusedNode[treeListColumn5] as FileSystemInfo).FullName, null);
            //Process.Start(((sender as TreeList).FocusedNode[treeListColumn5] as FileSystemInfo).FullName, null);

            //---
            //DevExpress.Utils.Text.StringInfo si = e.ItemInfo.TextInfo;

            //string s = e.ItemInfo.Description + v.ENTER;
            //s = s + e.ItemInfo.Text + v.ENTER;

            //this.Text = s;

            //for (int i = 0; i < this.winExplorerView.RowCount; i++) this.winExplorerView.InvertRowSelection(i);
        }

        #endregion


        #region BreadCrumb
        void InitializeBreadCrumb()
        {
            this.currentPath = StartupPath;
            //BreadCrumb.Path = this.currentPath;
            foreach (DriveInfo driveInfo in FileSystemHelper.GetFixedDrives())
            {
                BreadCrumb.Properties.History.Add(new BreadCrumbHistoryItem(driveInfo.RootDirectory.ToString()));
            }
        }


        #endregion BreadCrumb



    }
}
