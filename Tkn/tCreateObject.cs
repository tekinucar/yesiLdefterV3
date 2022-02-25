using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

using Tkn_InputPanel;
using Tkn_ToolBox;
using Tkn_Variable;
using Tkn_DevView;
using Tkn_Events;
using Tkn_DevColumn;
using Tkn_DefaultValue;
using Tkn_TablesRead;

using DevExpress.XtraBars.Ribbon;

using DevExpress.XtraGrid;

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;

using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.XtraGrid.Views.WinExplorer;

using DevExpress.XtraDataLayout;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.BandedGrid;
using Newtonsoft.Json;

namespace Tkn_CreateObject
{
    public class tCreateObject : tBase
    {
        tToolBox t = new tToolBox();

        #region Create_Form

        public Form Create_Form(string FormCode, string Caption, string MenuType, byte FormType, byte FormState)
        {
            Form tForm = Create_Form(FormCode, Caption, MenuType);

            tFormView(tForm, FormType, FormState);

            return tForm;
        }

        public Form Create_Form(string FormCode, string Caption, string MenuType)
        {
            tToolBox t = new tToolBox();
            tEvents ev = new tEvents();

            Form tForm = null;
            //RibbonForm tForm = new RibbonForm();

            if (MenuType != "Ribbon")
            {
                tForm = new Form();
                tForm.Padding = new System.Windows.Forms.Padding(v.Padding4);
            }
            if (MenuType == "Ribbon")
            {
                tForm = new RibbonForm();
                tForm.Padding = new System.Windows.Forms.Padding(0);
            }

            System.ComponentModel.IContainer components = null;
            components = new System.ComponentModel.Container();
            components.Add(tForm);

            tForm.AccessibleDescription = FormCode;
            tForm.Name = "tForm_" + t.AntiStr_Dot(FormCode);
            tForm.Text = Caption;
            tForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;

            //tForm.Tag = FormType;

            ev.tFormEventsAdd(tForm);

            // Formun üzerine gizli MemoEdit ekleniyor
            Create_MyFormBox(tForm, string.Empty);

            return tForm;
        }

        public Form tFormView(Form tForm, byte FormType, byte FormState)
        {
            tToolBox t = new tToolBox();

            //-------------
            // FormType
            // * 0, 1  : Normal Form
            // * 2 : MdiChild Form
            // * 3 : Main Form
            // * 4 : Dialog Form

            if (FormType <= 1) // Normal Form
            {
                //tForm_Properties(ds_FormItems, tForm, 0);

                if (FormState == 1) t.NormalForm_View(tForm, FormWindowState.Normal);
                if (FormState == 2) t.NormalForm_View(tForm, FormWindowState.Minimized);
                if (FormState == 3) t.NormalForm_View(tForm, FormWindowState.Maximized);

                //DevExp.Form_Design(tForm, ds_FormItems, Referans);
            }
            if (FormType == 2) // MdiChild Form
            {
                //DevExp.tForm_Properties(ds, tForm, 0);

                if (FormState == 1) t.ChildForm_View(tForm, Application.OpenForms[0], FormWindowState.Normal);
                if (FormState == 2) t.ChildForm_View(tForm, Application.OpenForms[0], FormWindowState.Minimized);
                if (FormState == 3) t.ChildForm_View(tForm, Application.OpenForms[0], FormWindowState.Maximized);
                //DevExp.Form_Design(tForm, ds_FormItems, Referans);
            }
            if (FormType == 3) // Main Form
            {
                //DevExp.tForm_Properties(ds_FormItems, mdifrm, 0);
                //DevExp.Form_Design(mdifrm, ds_FormItems, Referans);
            }
            if (FormType == 4) // Dialog Form
            {
                //DevExp.tForm_Properties(ds_FormItems, tForm, 0);
                //DevExp.Form_Design(tForm, ds_FormItems, Referans);

                if (FormState == 1) t.DialogForm_View(tForm, FormWindowState.Normal);
                if (FormState == 2) t.DialogForm_View(tForm, FormWindowState.Minimized);
                if (FormState == 3) t.DialogForm_View(tForm, FormWindowState.Maximized);
            }
            //--------------

            //----------
            return tForm;
        }

        #region tForm_Properties
        public void tForm_Properties(DataSet ds_FormItems, Form tForm, int i)
        {
            /*
            #region Tanımlamalar

            int Ref_Id = 0;
            int Item_Id = 0;
            byte Item_Type3 = 0;
            byte Item_Type4 = 0;
            byte Item_Type5 = 0;
            byte Item_Type6 = 0;
            byte Item_Type7 = 0;

            int width = 0;
            int height = 0;

            string Caption = string.Empty;
            string Dock_Style = string.Empty;

            #endregion Tanımlamalar

            Ref_Id = Convert.ToInt32(ds_FormItems.Tables[0].Rows[i]["REF_ID"]);
            Item_Id = Convert.ToInt32(ds_FormItems.Tables[0].Rows[i]["ITEM_ID"]);
            Caption = ds_FormItems.Tables[0].Rows[i]["ABOUT"].ToString();

            width = Convert.ToInt32(ds_FormItems.Tables[0].Rows[i]["SIZE_WIDTH"]);
            height = Convert.ToInt32(ds_FormItems.Tables[0].Rows[i]["SIZE_HEIGHT"]);

            Item_Type3 = Convert.ToByte(ds_FormItems.Tables[0].Rows[i]["ITEM_TYPE3"]);
            Item_Type4 = Convert.ToByte(ds_FormItems.Tables[0].Rows[i]["ITEM_TYPE4"]);
            Item_Type5 = Convert.ToByte(ds_FormItems.Tables[0].Rows[i]["ITEM_TYPE5"]);
            Item_Type6 = Convert.ToByte(ds_FormItems.Tables[0].Rows[i]["ITEM_TYPE6"]);
            Item_Type7 = Convert.ToByte(ds_FormItems.Tables[0].Rows[i]["ITEM_TYPE7"]);

            if (ds_FormItems.Tables[0].Rows[i]["CAPTION"].ToString() != "")
                Caption = ds_FormItems.Tables[0].Rows[i]["CAPTION"].ToString();

            if (Item_Id == sp.fr_Form)
            {
                tForm.Text = Caption;

                if ((width > 0) && (height > 0)) tForm.Size = new System.Drawing.Size(width, height);
                if ((width > 0) && (height == 0)) tForm.Size = new System.Drawing.Size(width, sp.Ekran_Height);
                if ((width == 0) && (height > 0)) tForm.Size = new System.Drawing.Size(sp.Ekran_Width, height);


                if (Item_Type3 == 1) tForm.FormBorderStyle = FormBorderStyle.None;
                if (Item_Type3 == 2) tForm.FormBorderStyle = FormBorderStyle.FixedSingle;
                if (Item_Type3 == 3) tForm.FormBorderStyle = FormBorderStyle.Fixed3D;
                if (Item_Type3 == 4) tForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                if (Item_Type3 == 5) tForm.FormBorderStyle = FormBorderStyle.Sizable;
                if (Item_Type3 == 6) tForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                if (Item_Type3 == 7) tForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;

                if (Item_Type4 == 1) tForm.StartPosition = FormStartPosition.Manual;
                if (Item_Type4 == 2) tForm.StartPosition = FormStartPosition.CenterScreen;
                if (Item_Type4 == 3) tForm.StartPosition = FormStartPosition.WindowsDefaultLocation;
                if (Item_Type4 == 4) tForm.StartPosition = FormStartPosition.WindowsDefaultBounds;
                if (Item_Type4 == 5) tForm.StartPosition = FormStartPosition.CenterParent;

                if (Item_Type5 == 1) tForm.TopMost = true;
                if (Item_Type5 == 2) tForm.TopMost = false;

                if (Item_Type6 == 1) tForm.IsMdiContainer = true;
                if (Item_Type6 == 2) tForm.IsMdiContainer = false;

                // TabbedMdiManeger 
                if (Item_Type7 == 1)
                {
                    System.ComponentModel.Container components = new System.ComponentModel.Container();
                    DevExpress.XtraTabbedMdi.XtraTabbedMdiManager tXtraTabbedMdiManager = new DevExpress.XtraTabbedMdi.XtraTabbedMdiManager(components);

                    tForm.Container.Add(tXtraTabbedMdiManager);
                    tXtraTabbedMdiManager.MdiParent = tForm;
                }
            }
            */

        }
        #endregion tForm_Properties

        #endregion Create Form

        #region Create_ViewControls

        public Control Create_View(DataRow Row, string TableIPCode, string MultiPageID)
        {
            tToolBox t = new tToolBox();

            Control cntrl = null;

            int RefID = t.Set(Row["REF_ID"].ToString(), "", 0);
            int ViewType = t.Set(Row["VIEW_CMP_TYPE"].ToString(), "", v.obj_vw_GridView);
            string Prop_Runtime = t.Set(Row["PROP_RUNTIME"].ToString(), "", "");
            string Prop_Navigator = t.Set(Row["PROP_NAVIGATOR"].ToString(), "", "");

            if (
                (ViewType == v.obj_vw_GridView) ||
                (ViewType == v.obj_vw_BandedGridView) ||
                (ViewType == v.obj_vw_AdvBandedGridView) ||
                (ViewType == v.obj_vw_GridLayoutView) ||
                (ViewType == v.obj_vw_WinExplorerView) ||
                (ViewType == v.obj_vw_TileView) ||
                (ViewType == v.obj_vw_CardView) ||
                (ViewType == v.obj_vw_VGridSingle) ||
                (ViewType == v.obj_vw_VGridMulti) ||
                (ViewType == v.obj_vw_TreeListView) ||
                (ViewType == v.obj_vw_DataLayoutView) ||
                (ViewType == v.obj_vw_WizardControl)
               )
            {
                // esas buradan dönüş yapılıyor
                cntrl = Create_View_(ViewType, RefID, TableIPCode, MultiPageID);

                // RunTime Sırasında yapılacak işlerin listesi
                if (t.IsNotNull(Prop_Runtime) || (t.IsNotNull(Prop_Navigator)))
                {
                    cntrl.AccessibleDescription = Prop_Runtime + v.ENTER + "|ds|" + v.ENTER + Prop_Navigator;
                }
            }

            return cntrl; // bu dönerse null döner 
        }

        public Control Create_View_(int ViewType, int RefID, string TableIPCode, string MultiPageID)
        {
            Control cntrl = null;

            #region Grid, BandedGrid, AdvBandedGrid, GridLayoutView, WinExplorerView

            if ((ViewType == v.obj_vw_GridView) ||
                (ViewType == v.obj_vw_BandedGridView) ||
                (ViewType == v.obj_vw_AdvBandedGridView) ||
                (ViewType == v.obj_vw_GridLayoutView) ||
                (ViewType == v.obj_vw_WinExplorerView) ||
                (ViewType == v.obj_vw_TileView) ||
                (ViewType == v.obj_vw_CardView)
                )
            {
                GridControl tGridControl = new GridControl();

                tGridControl.Name = "tGridControl_" + RefID.ToString();
                tGridControl.AccessibleName = TableIPCode + MultiPageID;
                tGridControl.Dock = DockStyle.Fill;

                tEvents ev = new tEvents();

                tGridControl.AllowDrop = true;
                tGridControl.DragOver += new System.Windows.Forms.DragEventHandler(ev.myGridControl_DragOver);
                tGridControl.DragDrop += new System.Windows.Forms.DragEventHandler(ev.myGridControl_DragDrop);
                tGridControl.DragEnter += new System.Windows.Forms.DragEventHandler(ev.myGridControl_DragEnter);
                tGridControl.DragLeave += new System.EventHandler(ev.myGridControl_DragLeave);
                tGridControl.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(ev.myGridControl_GiveFeedback);
                tGridControl.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(ev.myGridControl_QueryContinueDrag);
                tGridControl.Paint += new PaintEventHandler(ev.myGridControl_Paint);

                //this.gridControl1.DragOver += new System.Windows.Forms.DragEventHandler(this.gridControl1_DragOver);
                //this.gridControl1.DragLeave += new System.EventHandler(this.gridControl1_DragLeave);
                //this.gridControl1.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.gridControl1_GiveFeedback);
                //this.gridControl1.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.gridControl1_QueryContinueDrag);

                return tGridControl; // <<<< dönen cntrl
            }

            #endregion // Grids

            #region VerticalGrid

            if ((ViewType == v.obj_vw_VGridSingle) ||
                (ViewType == v.obj_vw_VGridMulti))
            {
                DevExpress.XtraVerticalGrid.VGridControl tVGridControl =
                                  new DevExpress.XtraVerticalGrid.VGridControl();

                tVGridControl.BeginUpdate();

                tVGridControl.Cursor = System.Windows.Forms.Cursors.SizeWE;
                tVGridControl.Name = "tVGridControl_" + RefID.ToString();
                tVGridControl.AccessibleName = TableIPCode + MultiPageID;
                //tVGridControl.Controls.Clear();
                tVGridControl.Rows.Clear();
                tVGridControl.Dock = DockStyle.Fill;
                tVGridControl.AllowDrop = true;
                //tVGridControl.TreeButtonStyle = TreeButtonStyle.TreeView;

                //tVGridControl.RowHeaderWidth = 150;
                //tVGridControl.RecordWidth = 150;
                tVGridControl.LayoutStyle = DevExpress.XtraVerticalGrid.LayoutViewStyle.SingleRecordView;

                if (ViewType == v.obj_vw_VGridSingle)
                {
                    tVGridControl.LayoutStyle = DevExpress.XtraVerticalGrid.LayoutViewStyle.SingleRecordView;
                }
                if (ViewType == v.obj_vw_VGridMulti)
                {
                    tVGridControl.LayoutStyle = DevExpress.XtraVerticalGrid.LayoutViewStyle.MultiRecordView;
                    tVGridControl.RowHeaderWidth = 150;
                }

                //tVGridControl.ScrollsStyle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
                tVGridControl.Appearance.Category.Font =
                     new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(1)));

                tVGridControl.ScrollVisibility = ScrollVisibility.Auto;
                tVGridControl.EndUpdate();

                return tVGridControl; // <<<< dönen cntrl
            }
            #endregion VerticalGrid

            #region TreeListView

            if (ViewType == v.obj_vw_TreeListView)
            {
                DevExpress.XtraTreeList.TreeList tTreeListView =
                    new DevExpress.XtraTreeList.TreeList();

                tTreeListView.Name = "tTreeListView_" + RefID.ToString();
                tTreeListView.AccessibleName = TableIPCode + MultiPageID;
                tTreeListView.Dock = DockStyle.Fill;
                tTreeListView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                tTreeListView.Location = new System.Drawing.Point(0, 0);
                //tTreeListView.OptionsBehavior.DragNodes = true;
                tTreeListView.OptionsView.AutoWidth = false;
                tTreeListView.OptionsBehavior.EnableFiltering = true;

                return tTreeListView; // <<<< dönen cntrl
            }

            #endregion TreeListView

            #region DataLayoutControl

            if (ViewType == v.obj_vw_DataLayoutView)
            {
                tEvents ev = new tEvents();

                DevExpress.XtraDataLayout.DataLayoutControl tDataLayoutControl =
                                       new DevExpress.XtraDataLayout.DataLayoutControl();

                tDataLayoutControl.BeginUpdate();
                tDataLayoutControl.SuspendLayout();
                tDataLayoutControl.Name = "tDataLayoutControl" + RefID.ToString();
                // DİKKAT : burayı sakın açma, data çalışmaz oluyor
                // tDataLayoutControl.DataSource = dsData.Tables[0];
                // ****
                //tDataLayoutControl.Dock = DockStyle.Fill;
                tDataLayoutControl.AccessibleName = TableIPCode + MultiPageID;
                tDataLayoutControl.AutoScroll = false;
                //tDataLayoutControl.OptionsItemText.TextAlignMode = TextAlignMode.AlignInGroups;
                tDataLayoutControl.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0);
                tDataLayoutControl.Appearance.ControlFocused.BackColor = v.AppearanceFocusedColor;
                tDataLayoutControl.Appearance.ControlFocused.Options.UseBackColor = true;
                tDataLayoutControl.OptionsView.AllowHotTrack = true;
                tDataLayoutControl.OptionsView.DrawItemBorders = true;
                tDataLayoutControl.OptionsView.HighlightFocusedItem = true;
                tDataLayoutControl.OptionsFocus.AllowFocusReadonlyEditors = false;
                tDataLayoutControl.OptionsFocus.MoveFocusDirection = MoveFocusDirection.DownThenAcross;

                //tDataLayoutControl.RegisterUserCustomizationForm(typeof(YesiLdefter.MSCustomization)); 

                DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1 =
                                   new DevExpress.XtraLayout.LayoutControlGroup();
                // 
                // layoutControlGroup1
                // 
                //layoutControlGroup1.BeginInit();
                //layoutControlGroup1.AppearanceItemCaption.BackColor = v.AppearanceItemCaptionColor1;
                //layoutControlGroup1.AppearanceItemCaption.BackColor2 = v.AppearanceItemCaptionColor2;
                //layoutControlGroup1.AppearanceItemCaption.Options.UseBackColor = true;

                layoutControlGroup1.Enabled = true;
                layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
                layoutControlGroup1.GroupBordersVisible = true;
                layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
                layoutControlGroup1.Name = "LCGroup_" + RefID.ToString();
                layoutControlGroup1.Text = "LCGroup_" + RefID.ToString();
                layoutControlGroup1.TextVisible = false;
                layoutControlGroup1.Size = new System.Drawing.Size(200, 200);
                layoutControlGroup1.DoubleClick += new System.EventHandler(ev.layoutControlGroup_DoubleClick);

                tDataLayoutControl.Root = layoutControlGroup1;

                //tDataLayoutControl.Root.Add(layoutControlGroup1);

                return tDataLayoutControl;
            }

            #endregion DataLayoutControl

            #region WizardControl
            if (ViewType == v.obj_vw_WizardControl)
            {
                DevExpress.XtraWizard.WizardControl tWizardControl = new DevExpress.XtraWizard.WizardControl();

                tWizardControl.Name = "tWizardControl" + RefID.ToString();
                //tWizardControl.DataSource = dsData.Tables[0];
                tWizardControl.Dock = DockStyle.Fill;
                tWizardControl.WizardStyle = DevExpress.XtraWizard.WizardStyle.WizardAero;
                //tWizardControl.AccessibleDefaultActionDescription = ClassFieldName;
                tWizardControl.AccessibleName = TableIPCode + MultiPageID;

                return tWizardControl;
            }
            #endregion

            return cntrl; // buradan dönüyorsa null döner
        }

        #endregion Create_ViewControls

        //------------------

        #region // tMyFormBox Create -- Her formun üzerindeki gizli bir memo ekleniyor
        public void Create_MyFormBox(Form tForm, string myFormLoadValue)
        {
            // Bu memo üzerine Form create sırasında gerekli olan bilgiler Set ediliyor
            // Formun için create olan IP ler buraya bakarak gerekli olan bilgileri Get edebiliyor
            if (tForm == null) return;

            tToolBox t = new tToolBox();
            string[] controls = new string[] { };
            Control c = t.Find_Control(tForm, "tMyFormBox", "", controls);

            if (c == null)
            {
                DevExpress.XtraEditors.MemoEdit tMyFormBox = new DevExpress.XtraEditors.MemoEdit();

                tMyFormBox.Visible = v.myFormBox_Visible;
                tMyFormBox.Name = "tMyFormBox_" + tForm.Name;
                tMyFormBox.Dock = DockStyle.Left;
                tMyFormBox.Size = new System.Drawing.Size(320, 500);
                tMyFormBox.Parent = tForm;
                tForm.Controls.Add(tMyFormBox);

                if (t.IsNotNull(myFormLoadValue))
                    tMyFormBox.EditValue = myFormLoadValue;
            }
        }
        #endregion

        #region Create_Navigator

        public void Create_Navigator(Form tForm, Control tPanelControl,
                                     DataRow row_Table, DataSet dsData,
                                     string TableIPCode, string External_TableIPCode)
        {
            tToolBox t = new tToolBox();
            tEvents ev = new tEvents();

            #region Tanımlar

            int RefID = t.Set(row_Table["REF_ID"].ToString(), "", 0);
            string TableName = t.Set(row_Table["LKP_TABLE_NAME"].ToString(), "", "");
            string navigator = t.Set(row_Table["NAVIGATOR"].ToString(), "", "");
            byte DBaseName = t.Set(row_Table["LKP_DBASE_NAME"].ToString(), "", (byte)3);

            string Key_FName = t.Set(row_Table["LKP_KEY_FNAME"].ToString(), "", "");
            string Master_Key_FName = t.Set(row_Table["MASTER_KEY_FNAME"].ToString(), row_Table["LKP_MASTER_KEY_FNAME"].ToString(), "");
            string Foreing_FName = t.Set(row_Table["FOREING_FNAME"].ToString(), row_Table["LKP_FOREING_FNAME"].ToString(), "");
            string Parent_FName = t.Set(row_Table["PARENT_FNAME"].ToString(), row_Table["LKP_PARENT_FNAME"].ToString(), "");

            string Prop_Navigator = t.Set(row_Table["PROP_NAVIGATOR"].ToString(), "", "");
            string Prop_Search = t.Set(row_Table["PROP_SEARCH"].ToString(), "", "");
            string Prop_Views = t.Set(row_Table["PROP_VIEWS"].ToString(), "", "");
            string SubView = t.Set(row_Table["PROP_SUBVIEW"].ToString(), row_Table["LKP_PROP_SUBVIEW"].ToString(), "");

            // sonuna yapılıyor, bu bilgi  simpleButton_yeni_alt_hesap  için gerekli 
            t.MyProperties_Set(ref Prop_Navigator, "KEY_FNAME", Key_FName);
            t.MyProperties_Set(ref Prop_Navigator, "MASTER_KEY_FNAME", Master_Key_FName);
            t.MyProperties_Set(ref Prop_Navigator, "FOREING_FNAME", Foreing_FName);
            t.MyProperties_Set(ref Prop_Navigator, "PARENT_FNAME", Parent_FName);

            // string NavButtonCaption = t.MyProperties_Get(Prop_Views, "NAVBUTTONCAPTION:");
            string NavButtonCaption = t.Find_Properties_Value(Prop_Views, "NAVBUTTONCAPTION");


            // SubView False ise
            // if (SubView.IndexOf("=SV_ENABLED:TRUE;") == -1) SubView = string.Empty;

            string value = string.Empty;

            string s1 = "=ROW_PROP_SUBVIEW:";
            string s2 = (char)34 + "SV_ENABLED" + (char)34 + ": " + (char)34 + "TRUE" + (char)34;
            //  "SV_ENABLED": "TRUE",

            if (SubView.IndexOf(s1) > -1)
                value = t.MyProperties_Get(SubView, "SV_ENABLED:");

            if (SubView.IndexOf(s2) > -1)
                value = t.Find_Properties_Value(SubView, "SV_ENABLED");

            if (value != "TRUE") SubView = string.Empty;

            /// DATA_READ_TYPE
            /// 1, '', 'Not Read Data'
            /// 2, '', 'Read RefID'
            /// 3, '', 'Detail Table'
            /// 4, '', 'SubDetail Table'
            /// 5, '', 'Data Collection'
            /// 6, '', 'Read SubView'
            int Data_Read_Type = t.Set(row_Table["DATA_READ"].ToString(), "", (byte)1);


            #endregion Tanımlar

            #region DataNavigator

            /// daha sonra datanavigator List için çekildiğinde bunları create sırasıyla gelmesi için
            /// oluşturulması sırasına göre Name oluşturuluyor, 
            /// isim sıralaması 10 sonra başlaması için  +11 verilmiştir 
            List<string> list = new List<string>();
            t.Find_DataNavigator_List(tForm, ref list);
            int dNCount = list.Count + 11;

            DevExpress.XtraEditors.DataNavigator tDataNavigator = new DevExpress.XtraEditors.DataNavigator();
            tDataNavigator.Visible = true;
            tDataNavigator.Width = 80;
            //tDataNavigator.Name = "tDataNavigator_" + RefID.ToString();
            tDataNavigator.Name = "tDataNavigator_" + dNCount.ToString();
            if (dsData != null)
            {
                if (dsData.Tables.Count > 0)
                    tDataNavigator.DataSource = dsData.Tables[0];
            }
            //tDataNavigator.TextStringFormat = "Kayıt {0}, {1}";
            tDataNavigator.TextStringFormat = "{0}, {1}";
            tDataNavigator.TextLocation = NavigatorButtonsTextLocation.Begin;
            tDataNavigator.PositionChanged += new System.EventHandler(ev.tdataNavigator_PositionChanged);
            tDataNavigator.ButtonClick += new DevExpress.XtraEditors.NavigatorButtonClickEventHandler(ev.tdataNavigator_ButtonClick);
            tDataNavigator.Dock = DockStyle.Right;
            tDataNavigator.TabStop = false;
            
            //tDataNavigator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            tDataNavigator.Appearance.BackColor = v.colorFocus;
            tDataNavigator.Appearance.Options.UseBackColor = false;

            tDataNavigator.Enter += new System.EventHandler(ev.dataNavigator_Enter);
            tDataNavigator.Leave += new System.EventHandler(ev.dataNavigator_Leave);
            
            tDataNavigator.Buttons.First.Visible = false;
            tDataNavigator.Buttons.Last.Visible = false;
            tDataNavigator.Buttons.Next.Visible = false;
            tDataNavigator.Buttons.NextPage.Visible = false;
            tDataNavigator.Buttons.Prev.Visible = false;
            tDataNavigator.Buttons.PrevPage.Visible = false;
            tDataNavigator.Buttons.Append.Visible = false;
            tDataNavigator.Buttons.CancelEdit.Visible = false;
            tDataNavigator.Buttons.EndEdit.Visible = false;
            tDataNavigator.Buttons.Remove.Visible = false;

            // Tablonu konumlandığı ID yi tutuyor, db.MyRecord() için gerekli
            tDataNavigator.Tag = 0;
            // runtime sırasında DataNavigator e ulaşmak için 
            tDataNavigator.AccessibleName = TableIPCode;
            // Tablonun Adını tutuyor, db.MyRecord() için gerekli
            tDataNavigator.AccessibleDefaultActionDescription = TableName;
            // Database in Türünü tutuyor, db.MyRecord() için gerekli
            tDataNavigator.AccessibleDescription = DBaseName.ToString();

            #endregion DataNavigator

            #region SubView
            if (t.IsNotNull(SubView))
            {
                tDataNavigator.AccessibleDescription = SubView;
                tDataNavigator.IsAccessible = true;
            }
            #endregion SubView

            #region Button Paneli

            DevExpress.XtraEditors.PanelControl NavPanel = new DevExpress.XtraEditors.PanelControl();
            NavPanel.Name = "tPanel_Navigator_" + RefID.ToString();
            NavPanel.Height = 26;
            NavPanel.Dock = DockStyle.Bottom;
            NavPanel.SendToBack();
            NavPanel.TabIndex = 2;
            // navigator butonlarının gösterilmesi istenmiyor ise panelide gizleyelim
            if (navigator == "")
            {
                NavPanel.Height = 2; // Visible = false kullanma
            }

            #endregion Button Paneli

            #region Create Buttons

            Create_Navigator_Buttons(NavPanel,
                                     tDataNavigator,
                                     TableIPCode,
                                     External_TableIPCode,
                                     navigator,
                                     Prop_Navigator,
                                     Prop_Search,
                                     NavButtonCaption,
                                     Data_Read_Type);

            #endregion Create Buttons

            tPanelControl.Controls.Add(NavPanel);
        }

        private void createDropDownButton(vNavigatorButton nButton)
        {
            int i = nButton.navigatorList.IndexOf(nButton.tabIndex.ToString() + " ");
            if (i == -1) i = nButton.navigatorList.IndexOf("[" + nButton.tabIndex.ToString() + "]");
            if (i > -1)
            {
                Int16 height = 23;

                using (tEvents ev = new tEvents())
                {
                    DevExpress.XtraEditors.DropDownButton dragDownButton_ek = new DevExpress.XtraEditors.DropDownButton();

                    dragDownButton_ek.Name = nButton.buttonName; //"dragDownButton_ek1";
                    dragDownButton_ek.Size = new System.Drawing.Size(nButton.width + 17, height);
                    dragDownButton_ek.Dock = nButton.dock; //DockStyle.Left;
                    dragDownButton_ek.TabIndex = nButton.tabIndex; //111;
                    //dragDownButton_ek.TabStop = false;
                    dragDownButton_ek.Text = ":.";
                    dragDownButton_ek.AccessibleName = nButton.TableIPCode;
                    if (nButton.events)
                        dragDownButton_ek.Click += new System.EventHandler(ev.btn_Navigotor_Click);

                    dragDownButton_ek.Enter += new System.EventHandler(ev.btn_Navigotor_Enter);
                    dragDownButton_ek.Leave += new System.EventHandler(ev.btn_Navigotor_Leave);

                    if (nButton.backColor != null)
                    {
                        dragDownButton_ek.Appearance.BackColor = nButton.backColor;   //System.Drawing.Color.LightGreen;
                        dragDownButton_ek.Appearance.Options.UseBackColor = true;
                        dragDownButton_ek.AppearancePressed.BackColor = System.Drawing.Color.LightGreen;
                        dragDownButton_ek.AppearancePressed.Options.UseBackColor = true;


                    }
                    nButton.navigatorPanel.Controls.Add(dragDownButton_ek);
                    Buttons_CaptionChange(dragDownButton_ek, nButton.navigatorList);
                }
            }
        }

        private void createCheckButton(vNavigatorButton nButton)
        {
            int i = nButton.navigatorList.IndexOf(nButton.tabIndex.ToString() + " ");
            if (i == -1) i = nButton.navigatorList.IndexOf("[" + nButton.tabIndex.ToString() + "]");
            if (i > -1)
            {
                Int16 height = 23;

                using (tEvents ev = new tEvents())
                {
                    DevExpress.XtraEditors.CheckButton checkButton_ek = new DevExpress.XtraEditors.CheckButton();

                    checkButton_ek.Name = nButton.buttonName; // "checkButton_ek1";
                    checkButton_ek.Size = new System.Drawing.Size(nButton.width, height);
                    checkButton_ek.Dock = nButton.dock;// DockStyle.Left;
                    checkButton_ek.TabIndex = nButton.tabIndex;//  101;
                    //checkButton_ek.TabStop = false;
                    checkButton_ek.Text = ":.";
                    checkButton_ek.AccessibleName = nButton.TableIPCode;
                    if (nButton.events)
                        checkButton_ek.Click += new System.EventHandler(ev.btn_Navigotor_Click);

                    checkButton_ek.Enter += new System.EventHandler(ev.btn_Navigotor_Enter);
                    checkButton_ek.Leave += new System.EventHandler(ev.btn_Navigotor_Leave);

                    if (nButton.backColor != null)
                    {
                        checkButton_ek.Appearance.BackColor = nButton.backColor;   //System.Drawing.Color.LightGreen;
                        checkButton_ek.Appearance.Options.UseBackColor = true;
                    }
                    nButton.navigatorPanel.Controls.Add(checkButton_ek);
                    Buttons_CaptionChange(checkButton_ek, nButton.navigatorList);
                }
            }
        }

        private SimpleButton createNavigatorButton(vNavigatorButton nButton)
        {
            //Control navigatorPanel,
            //string navigatorList,
            //string TableIPCode,
            //string buttonName,
            //string buttonText,
            //Int16 tabIndex,
            //Int16 width,
            //DockStyle dock,
            //string imageName

            int i = nButton.navigatorList.IndexOf(nButton.tabIndex.ToString() + " ");
            if (i == -1) i = nButton.navigatorList.IndexOf("[" + nButton.tabIndex.ToString() + "]");
            if (i > -1)
            {
                Int16 height = 23;

                using (tEvents ev = new tEvents())
                {
                    DevExpress.XtraEditors.SimpleButton simpleButton = new DevExpress.XtraEditors.SimpleButton();

                    simpleButton.Name = nButton.buttonName; // "simpleButton_cikis";
                    simpleButton.Size = new System.Drawing.Size(nButton.width, height);
                    simpleButton.Dock = nButton.dock; // DockStyle.Right;
                    simpleButton.TabIndex = nButton.tabIndex; //11;
                    //simpleButton.TabStop = false;
                    simpleButton.Text = nButton.buttonText; // "Çıkış";
                    simpleButton.AccessibleName = nButton.TableIPCode;
                    simpleButton.AccessibleDescription = nButton.propNavigator;
                    simpleButton.AccessibleDefaultActionDescription = nButton.propSearch;
                    simpleButton.Image = t.Find_Glyph(nButton.imageName);  //("CLOSE16");

                    if (nButton.events)
                        simpleButton.Click += new System.EventHandler(ev.btn_Navigotor_Click);

                    simpleButton.Enter += new System.EventHandler(ev.btn_Navigotor_Enter);
                    simpleButton.Leave += new System.EventHandler(ev.btn_Navigotor_Leave);

                    if (nButton.backColor != null)
                    {
                        simpleButton.Appearance.BackColor = nButton.backColor;   //System.Drawing.Color.LightGreen;
                        simpleButton.Appearance.Options.UseBackColor = true;
                    }
                    nButton.navigatorPanel.Controls.Add(simpleButton);
                    Buttons_CaptionChange(simpleButton, nButton.navigatorList);

                    return simpleButton;
                }
            }
            return null;
        }

        private void Create_Navigator_Buttons(Control NavPanel, DataNavigator tDataNavigator,
                                              string TableIPCode,
                                              string External_TableIPCode,
                                              string navigator,
                                              string Prop_Navigator,
                                              string Prop_Search,
                                              string NavButtonCaption,
                                              int Data_Read_Type)
        {
            int i = -1;
            int width = 70;
            Int16 width1 = 25;
            Int16 width2 = 35;
            Int16 width3 = 120;

            string xTableIPCode = TableIPCode;
            if (t.IsNotNull(External_TableIPCode))
                xTableIPCode = External_TableIPCode;
            DevExpress.XtraEditors.SimpleButton simpleButton = null;
            
            vNavigatorButton nButton = new vNavigatorButton();

            nButton.navigatorPanel = NavPanel;
            nButton.TableIPCode = TableIPCode;
            nButton.navigatorList = navigator;
            nButton.propNavigator = Prop_Navigator;
            nButton.backColor = v.colorFocus;
            nButton.events = true;

            /*
            public static Color colorFocus = System.Drawing.Color.LightGreen; // color1
            public static Color colorNew = System.Drawing.Color.MediumTurquoise; // color2
            public static Color colorExit = System.Drawing.Color.Gold; //color3
            public static Color colorNavigator = System.Drawing.Color.DarkOrange; // color4   yön butonları
            public static Color colorDelete = System.Drawing.Color.OrangeRed; // color5
            */
            //--------------

            #region dragDownButton_ek, checkButton_ek, simpleButton_ek

            nButton.buttonName = "dragDownButton_ek1";
            nButton.buttonText = ":.";
            nButton.dock = DockStyle.Left;
            nButton.tabIndex = 111;
            nButton.imageName = "";
            nButton.width = (width + 17);
            nButton.events = false;
            createDropDownButton(nButton);

            nButton.buttonName = "checkButton_ek1";
            nButton.buttonText = ":.";
            nButton.dock = DockStyle.Left;
            nButton.tabIndex = 101;
            nButton.imageName = "";
            nButton.width = (width);
            nButton.events = false;
            createCheckButton(nButton);
            
            nButton.buttonName = "simpleButton_ek2";
            nButton.buttonText = ":.";
            nButton.dock = DockStyle.Left;
            nButton.tabIndex = 92;
            nButton.imageName = "";
            nButton.width = (width);
            nButton.events = false;
            createNavigatorButton(nButton);

            nButton.buttonName = "simpleButton_ek1";
            nButton.buttonText = ":.";
            nButton.dock = DockStyle.Left;
            nButton.tabIndex = 91;
            nButton.imageName = "";
            nButton.width = (width);
            nButton.events = false;
            createNavigatorButton(nButton);

            #endregion
            
            //--------------

            #region collapse, expand, onay, yazıcı butonları

            nButton.buttonName = "simpleButton_CollExp";
            nButton.buttonText = "A";
            nButton.dock = DockStyle.Left;
            nButton.tabIndex = 72;
            nButton.imageName = "30_319_AutoExpand_16x16";
            nButton.width = width2;
            nButton.events = true;
            createNavigatorButton(nButton);
            //simpleButton_collexp.GroupIndex = -1;
            
            nButton.buttonName = "simpleButton_onayla";
            nButton.buttonText = "+";
            nButton.dock = DockStyle.Left;
            nButton.tabIndex = 71;
            nButton.imageName = "";
            nButton.width = width2;
            nButton.events = true;
            createNavigatorButton(nButton);

            nButton.buttonName = "simpleButton_onay_iptal";
            nButton.buttonText = "-";
            nButton.dock = DockStyle.Left;
            nButton.tabIndex = 71;
            nButton.imageName = "";
            nButton.width = width2;
            nButton.events = true;
            createNavigatorButton(nButton);

            nButton.buttonName = "simpleButton_yazici";
            nButton.buttonText = "Yazdır...";
            nButton.dock = DockStyle.Left;
            nButton.tabIndex = 81;
            nButton.imageName = "30_338_Printer_16x16";
            nButton.width = width;
            nButton.events = true;
            createNavigatorButton(nButton);

            #endregion

            //--------------

            #region   DataNavigator ve yön butonları

            nButton.backColor = v.colorNavigator;

            nButton.buttonName = "simpleButton_en_basa";
            nButton.buttonText = "|<";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 47;
            nButton.imageName = "";// "40_404_First_16x16";
            nButton.width = width1;
            nButton.events = true;
            simpleButton = createNavigatorButton(nButton);
            if (simpleButton != null)
                simpleButton.TabIndex = 247;
            //simpleButton_en_basa.ImageLocation = ImageLocation.MiddleCenter;

            nButton.buttonName = "simpleButton_onceki_syf";
            nButton.buttonText = "<<";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 46;
            nButton.imageName = "";// "40_404_DoublePrev_16x16";
            nButton.width = width1;
            nButton.events = true;
            simpleButton = createNavigatorButton(nButton);
            if (simpleButton != null)
                simpleButton.TabIndex = 246;
            //simpleButton_onceki_syf.ImageLocation = ImageLocation.MiddleCenter;

            nButton.buttonName = "simpleButton_onceki";
            nButton.buttonText = "<";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 45;
            nButton.imageName = "";// "NAVGERI16";
            nButton.width = width1;
            nButton.events = true;
            simpleButton = createNavigatorButton(nButton);
            if (simpleButton != null)
                simpleButton.TabIndex = 245;
            //simpleButton_onceki.ImageLocation = ImageLocation.MiddleCenter;

            #region DataNavigator

            // dataNavigator Her şartta visible = false de olsa panelin içinde bulunması gerekmektedir
            if (tDataNavigator != null)
            {
                tDataNavigator.TabIndex = 44;

                i = navigator.IndexOf(tDataNavigator.TabIndex.ToString());

                if (i == -1)
                {
                    //tDataNavigator.TabStop = false;
                    tDataNavigator.Width = 1;
                    tDataNavigator.Dock = DockStyle.Left;
                    tDataNavigator.TextLocation = NavigatorButtonsTextLocation.None;
                }    //    tDataNavigator.Visible = false;

                tDataNavigator.TabIndex = 244;
                NavPanel.Controls.Add(tDataNavigator);
                

            }
            //---
            #endregion

            nButton.buttonName = "simpleButton_sonraki";
            nButton.buttonText = ">";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 43;
            nButton.imageName = "";// "NAVILERI16";
            nButton.width = width1;
            nButton.events = true;
            simpleButton = createNavigatorButton(nButton);
            if (simpleButton != null)
                simpleButton.TabIndex = 243;
            //simpleButton_sonraki.ImageLocation = ImageLocation.MiddleCenter;

            nButton.buttonName = "simpleButton_sonraki_syf";
            nButton.buttonText = ">>";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 42;
            nButton.imageName = "";// "40_404_DoubleNext_16x16";
            nButton.width = width1;
            nButton.events = true;
            simpleButton = createNavigatorButton(nButton);
            if (simpleButton != null)
                simpleButton.TabIndex = 242;

            nButton.buttonName = "simpleButton_en_sona";
            nButton.buttonText = ">|";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 41;
            nButton.imageName = "";// "40_404_Last_16x16";
            nButton.width = width1;
            nButton.events = true;
            simpleButton = createNavigatorButton(nButton);
            if (simpleButton != null)
                simpleButton.TabIndex = 241;
            //simpleButton_en_sona.ImageLocation = ImageLocation.MiddleCenter;

            #endregion

            //--------------

            #region silSatır, silFiş

            nButton.backColor = v.colorDelete;

            nButton.buttonName = "simpleButton_sil_satir";
            nButton.buttonText = "Satır Sil";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 26;
            nButton.imageName = "40_408_Delete_16x16";
            nButton.width = width;
            nButton.events = true;
            createNavigatorButton(nButton);

            nButton.buttonName = "simpleButton_sil_fis";
            nButton.buttonText = "Sil";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 27;
            nButton.imageName = "30_308_ChartTitlesNone_16x16";
            nButton.width = width;
            nButton.events = true;
            createNavigatorButton(nButton);

            #endregion
            
            //--------------

            #region yeniKartAç, yeniHeap, yeniAltHesap

            nButton.backColor = v.colorNew;
            nButton.TableIPCode = xTableIPCode;

            nButton.buttonName = "simpleButton_yeni_kart_ac";
            nButton.buttonText = "Yeni ...";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 52;
            nButton.imageName = "30_301_AddFile_16x16";
            nButton.width = width;
            nButton.events = true;
            createNavigatorButton(nButton);
            
            nButton.buttonName = "simpleButton_yeni_hesap";
            nButton.buttonText = "Yeni";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 53;
            nButton.imageName = "YENIHESAP16";
            nButton.width = width;
            nButton.events = true;
            createNavigatorButton(nButton);
        
            nButton.buttonName = "simpleButton_yeni_alt_hesap";
            nButton.buttonText = "Yeni Alt ";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 54;
            nButton.imageName = "40_408_Delete_16x16";
            nButton.width = width;
            nButton.events = true;
            createNavigatorButton(nButton);
            //simpleButton_yeni_alt_hesap.AccessibleDescription = button54;

            nButton.TableIPCode = TableIPCode;

            #endregion
            
            //--------------

            #region kartAç, hesapAç, kaydetDevam, kaydet, kaydetYeni, kaydetÇık 

            nButton.backColor = v.colorSave;
            nButton.TableIPCode = xTableIPCode;
                        
            nButton.buttonName = "simpleButton_kaydet_cik";
            nButton.buttonText = "Kaydet, Çık";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 22;
            nButton.imageName = "30_341_SaveAndClose_16x16";
            nButton.width = width3;
            nButton.events = true;
            createNavigatorButton(nButton);

            
            nButton.buttonName = "simpleButton_kaydet_yeni";
            nButton.buttonText = "Kaydet, Yeni";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 23;
            nButton.imageName = "30_341_SaveAndNew_16x16";
            nButton.width = width3;
            nButton.events = true;
            createNavigatorButton(nButton);
                        
            nButton.buttonName = "simpleButton_kaydet";
            nButton.buttonText = "Kaydet";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 24;
            nButton.imageName = "KAYDET16";
            nButton.width = width;
            nButton.events = true;
            createNavigatorButton(nButton);
            
            nButton.buttonName = "simpleButton_kaydet_devam";
            nButton.buttonText = "Kaydet, Sonraki";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 25;
            nButton.imageName = "30_341_SaveDialog_16x16";
            nButton.width = width3;
            nButton.events = true;
            createNavigatorButton(nButton);

            nButton.TableIPCode = TableIPCode;

            nButton.backColor = v.colorFocus;

            // Kart Aç ve hesap Aç 
            nButton.buttonName = "simpleButton_kart_ac";
            nButton.buttonText = "Aç ...";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 51;
            nButton.imageName = "40_427_Article_16x16";
            nButton.width = width3;
            nButton.events = true;
            createNavigatorButton(nButton);

            // Hesap Aç // Hesap oku (aynı form içinde listedeki bir kaydın detayını başka IP ye aç/oku )
            nButton.buttonName = "simpleButton_hesap_ac";
            nButton.buttonText = "Göster";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 50;
            nButton.imageName = "30_348_Article_16x16";
            nButton.width = width3;
            nButton.events = true;
            createNavigatorButton(nButton);

            #endregion

            //--------------

            #region çıkış, sihirbazDevam, sihirbazGeri, listele, seç, ekle, arama

            nButton.buttonName = "simpleButton_arama";
            nButton.buttonText = "Arama Listesi";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 15;
            nButton.imageName = "40_429_Zoom_16x16";
            nButton.width = width;
            nButton.events = true;
            nButton.propSearch = Prop_Search;
            createNavigatorButton(nButton);
            nButton.propSearch = "";
            
            nButton.buttonName = "simpleButton_ekle";
            nButton.buttonText = "Listeye Ekle";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 14;
            nButton.imageName = "40_401_Apply_16x16";
            nButton.width = width3;
            nButton.events = true;
            createNavigatorButton(nButton);
            //simpleButton_ekle.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            //simpleButton_ekle.Appearance.Options.UseFont = true;
            
            nButton.buttonName = "simpleButton_sec";
            nButton.buttonText = "Seç, Çık"; // basligi + "Seç, Çık";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 13;
            nButton.imageName = "40_401_Apply_16x16";
            nButton.width = width3;
            nButton.events = true;
            createNavigatorButton(nButton);
            //simpleButton_sec.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            //simpleButton_sec.Appearance.Options.UseFont = true;


            nButton.buttonName = "simpleButton_listele";
            nButton.buttonText = "Listele";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 12;
            nButton.imageName = "30_319_ListBox_16x16";
            nButton.width = width;
            nButton.events = true;
            createNavigatorButton(nButton);

            nButton.buttonName = "simpleButton_sihirbaz_geri";
            nButton.buttonText = "Geri";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 48;
            nButton.imageName = "SIHIRBAZGERI16";
            nButton.width = width;
            nButton.events = true;
            createNavigatorButton(nButton);

            nButton.buttonName = "simpleButton_sihirbaz_devam";
            nButton.buttonText = "Devam";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 49;
            nButton.imageName = "SIHIRBAZDEVAM16";
            nButton.width = width;
            nButton.events = true;
            createNavigatorButton(nButton);

            // Çıkış butonu yanındaki boşluk
            DevExpress.XtraEditors.PanelControl Panel1 = new DevExpress.XtraEditors.PanelControl();
            Panel1.Name = "tPanel1";
            Panel1.Width = width2;
            Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            Panel1.Dock = DockStyle.Right;
            NavPanel.Controls.Add(Panel1);
            //---

            nButton.backColor = v.colorExit;

            nButton.buttonName = "simpleButton_cikis";
            nButton.buttonText = "Çıkış";
            nButton.dock = DockStyle.Right;
            nButton.tabIndex = 11;
            nButton.imageName = "CLOSE16";
            nButton.width = width;
            nButton.events = true;
            simpleButton = createNavigatorButton(nButton);
            if (simpleButton != null)
                simpleButton.TabIndex = 999;

            //-----
            #endregion

        }

        private void Create_Navigator_Buttons_Old(Control NavPanel, DataNavigator tDataNavigator, 
                                              string TableIPCode, 
                                              string External_TableIPCode,
                                              string navigator, 
                                              string Prop_Navigator, 
                                              string Prop_Search,
                                              string NavButtonCaption,
                                              int Data_Read_Type)
        {
            string xTableIPCode = TableIPCode;
            if (t.IsNotNull(External_TableIPCode))
                xTableIPCode = External_TableIPCode;


            #region Button Create

            DevExpress.XtraEditors.SimpleButton simpleButton_cikis = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_listele = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_sec = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_ekle = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_arama = new DevExpress.XtraEditors.SimpleButton();

            //DevExpress.XtraEditors.SimpleButton simpleButton_vazgec = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_kaydet_cik = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_kaydet_yeni = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_kaydet = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_kaydet_devam = new DevExpress.XtraEditors.SimpleButton();

            DevExpress.XtraEditors.SimpleButton simpleButton_sil_satir = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_sil_fis = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_iptal_navg = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_iptal_form = new DevExpress.XtraEditors.SimpleButton();

            DevExpress.XtraEditors.SimpleButton simpleButton_yeni_kart_ac = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_yeni_hesap = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_yeni_alt_hesap = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_kart_ac = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_hesap_ac = new DevExpress.XtraEditors.SimpleButton();

            //DevExpress.XtraEditors.SimpleButton simpleButton_yeni_fis_ac = new DevExpress.XtraEditors.SimpleButton();
            //DevExpress.XtraEditors.SimpleButton simpleButton_yeni_fis = new DevExpress.XtraEditors.SimpleButton();
            //DevExpress.XtraEditors.SimpleButton simpleButton_fisi_ac = new DevExpress.XtraEditors.SimpleButton();

            //ImageComboBoxEdit tImageComboBoxEdit_AppPlugList = new DevExpress.XtraEditors.ImageComboBoxEdit();

            DevExpress.XtraEditors.SimpleButton simpleButton_en_sona = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_sonraki_syf = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_sonraki = new DevExpress.XtraEditors.SimpleButton();

            DevExpress.XtraEditors.SimpleButton simpleButton_onceki = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_onceki_syf = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_en_basa = new DevExpress.XtraEditors.SimpleButton();

            DevExpress.XtraEditors.SimpleButton simpleButton_sihirbaz_geri = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_sihirbaz_devam = new DevExpress.XtraEditors.SimpleButton();

            DevExpress.XtraEditors.SimpleButton simpleButton_onayla = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_onay_iptal = new DevExpress.XtraEditors.SimpleButton();

            //DevExpress.XtraEditors.SimpleButton simpleButton_expand = new DevExpress.XtraEditors.SimpleButton();
            //DevExpress.XtraEditors.SimpleButton simpleButton_collapse = new DevExpress.XtraEditors.SimpleButton();
            //DevExpress.XtraEditors.CheckButton simpleButton_expand = new DevExpress.XtraEditors.CheckButton();
            //DevExpress.XtraEditors.CheckButton simpleButton_collapse = new DevExpress.XtraEditors.CheckButton();
            DevExpress.XtraEditors.CheckButton simpleButton_collexp = new DevExpress.XtraEditors.CheckButton();

            DevExpress.XtraEditors.SimpleButton simpleButton_yazici = new DevExpress.XtraEditors.SimpleButton();

            DevExpress.XtraEditors.SimpleButton simpleButton_ek1 = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_ek2 = new DevExpress.XtraEditors.SimpleButton();
            
            //DevExpress.XtraEditors.SimpleButton simpleButton_ek3 = new DevExpress.XtraEditors.SimpleButton();
            //DevExpress.XtraEditors.SimpleButton simpleButton_ek4 = new DevExpress.XtraEditors.SimpleButton();
            //DevExpress.XtraEditors.SimpleButton simpleButton_ek5 = new DevExpress.XtraEditors.SimpleButton();
            
            DevExpress.XtraEditors.CheckButton checkButton_ek1 = new DevExpress.XtraEditors.CheckButton();
            //DevExpress.XtraEditors.CheckButton checkButton_ek2 = new DevExpress.XtraEditors.CheckButton();
            //DevExpress.XtraEditors.CheckButton checkButton_ek3 = new DevExpress.XtraEditors.CheckButton();
            //DevExpress.XtraEditors.CheckButton checkButton_ek4 = new DevExpress.XtraEditors.CheckButton();
            //DevExpress.XtraEditors.CheckButton checkButton_ek5 = new DevExpress.XtraEditors.CheckButton();

            //DevExpress.XtraEditors.DropDownButton dragDownButton_ek1 = new DevExpress.XtraEditors.DropDownButton();
            //DevExpress.XtraEditors.DropDownButton dragDownButton_ek2 = new DevExpress.XtraEditors.DropDownButton();
            //DevExpress.XtraEditors.DropDownButton dragDownButton_ek3 = new DevExpress.XtraEditors.DropDownButton();
            //DevExpress.XtraEditors.DropDownButton dragDownButton_ek4 = new DevExpress.XtraEditors.DropDownButton();
            //DevExpress.XtraEditors.DropDownButton dragDownButton_ek5 = new DevExpress.XtraEditors.DropDownButton();
            
            #endregion Button Create
                       
            #region Button Propertios

            tEvents ev = new tEvents();
            
            Int16 width = 70;
            Int16 width1 = 25;
            Int16 width2 = 35;
            Int16 width3 = 120;
            Int16 height = 23;

            /*
            'NAVIGATOR',   50, '', '50 Hesap Aç'); // Kayıt Oku
            'NAVIGATOR',   51, '', '51 Kartı Aç');
            'NAVIGATOR',   52, '', '52 Yeni Hesap (Form ile)');
            'NAVIGATOR',   53, '', '53 Yeni Hesap');
            'NAVIGATOR',   54, '', '54 Yeni Alt Hesap');
            
            'NAVIGATOR',  210, '', '--Buton Başlıkları--');
            'NAVIGATOR',  211, '', '211 Dosya');
            'NAVIGATOR',  212, '', '212 Hesap');
            'NAVIGATOR',  213, '', '213 Fiş');
            'NAVIGATOR',  214, '', '214 Kart');
            */

            // Default 212 seçildi
            #region
            

            string basligi = "Hesabı ";
            string baslik = "Hesap ";
            int width_ek = 20;

            int i = navigator.IndexOf("[211]");
            if (i > -1)
            {
                basligi = "Dosyayı ";
                baslik = "Dosya ";
                width_ek = 20;
            }
            i = navigator.IndexOf("[212]");
            if (i > -1)
            {
                basligi = "Hesabı ";
                baslik = "Hesap ";
                width_ek = 20;
            }
            i = navigator.IndexOf("[213]");
            if (i > -1)
            {
                basligi = "Fişi ";
                baslik = "Fiş ";
                width_ek = 0;
            }
            i = navigator.IndexOf("[214]");
            if (i > -1)
            {
                basligi = "Kartı ";
                baslik = "Kart";
                width_ek = 0;
            }

            if (t.IsNotNull(NavButtonCaption))
            {
                basligi = NavButtonCaption + " ";
                baslik = NavButtonCaption;
                width_ek = 20;
            }
            #endregion

            #region Çıkış, listele, aç butonları

            //simpleButton_xxx.DialogResult = System.Windows.Forms.DialogResult.OK;

            simpleButton_cikis.Name = "simpleButton_cikis";
            simpleButton_cikis.Size = new System.Drawing.Size(width, height);
            simpleButton_cikis.Dock = DockStyle.Right;
            simpleButton_cikis.TabIndex = 11;
            simpleButton_cikis.TabStop = false;
            simpleButton_cikis.Text = "Çıkış";
            simpleButton_cikis.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_cikis.AccessibleName = TableIPCode;
            simpleButton_cikis.Image = t.Find_Glyph("CLOSE16");

            simpleButton_listele.Name = "simpleButton_listele";
            simpleButton_listele.Size = new System.Drawing.Size(width, height);
            simpleButton_listele.Dock = DockStyle.Right;
            simpleButton_listele.TabIndex = 12;
            simpleButton_listele.TabStop = false;
            simpleButton_listele.Text = "Listele";
            simpleButton_listele.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_listele.AccessibleName = TableIPCode;
            simpleButton_listele.Image = t.Find_Glyph("30_319_ListBox_16x16");
            

            simpleButton_sec.Name = "simpleButton_sec";
            simpleButton_sec.Size = new System.Drawing.Size(width3 + width_ek, height);
            simpleButton_sec.Dock = DockStyle.Right;
            simpleButton_sec.TabIndex = 13;
            simpleButton_sec.TabStop = false;
            simpleButton_sec.Text = basligi + "Seç, Çık";
            simpleButton_sec.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_sec.AccessibleName = TableIPCode;
            simpleButton_sec.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            simpleButton_sec.Appearance.Options.UseFont = true;
            simpleButton_sec.Image = t.Find_Glyph("40_401_Apply_16x16");

            simpleButton_ekle.Name = "simpleButton_ekle";
            simpleButton_ekle.Size = new System.Drawing.Size(width3 + width_ek, height);
            simpleButton_ekle.Dock = DockStyle.Right;
            simpleButton_ekle.TabIndex = 14;
            simpleButton_ekle.TabStop = false;
            simpleButton_ekle.Text = basligi + "Listeye Ekle";
            simpleButton_ekle.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_ekle.AccessibleName = TableIPCode;
            simpleButton_ekle.AccessibleDescription = Prop_Navigator; 
            simpleButton_ekle.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            simpleButton_ekle.Appearance.Options.UseFont = true;
            simpleButton_ekle.Image = t.Find_Glyph("40_401_Apply_16x16");

            simpleButton_arama.Name = "simpleButton_arama";
            //simpleButton_arama.Size = new System.Drawing.Size(width3 + width_ek, height);
            simpleButton_arama.Size = new System.Drawing.Size(width, height);
            simpleButton_arama.Dock = DockStyle.Right;
            simpleButton_arama.TabIndex = 15;
            simpleButton_arama.TabStop = false;
            //simpleButton_arama.Text = basligi + "Arama Listesi";
            simpleButton_arama.Text = "Arama Listesi";
            simpleButton_arama.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_arama.AccessibleName = TableIPCode;
            simpleButton_arama.AccessibleDescription = Prop_Navigator;
            simpleButton_arama.AccessibleDefaultActionDescription = Prop_Search;
            simpleButton_arama.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            simpleButton_arama.Appearance.Options.UseFont = true;
            simpleButton_arama.Image = t.Find_Glyph("40_429_Zoom_16x16");


            #endregion Çıkış, listele, aç butonları

            //---------------------------------------------------------
            #region  Kaydet, sil, iptal butonları
            //simpleButton_vazgec.Name = "simpleButton_vazgec";
            //simpleButton_vazgec.Size = new System.Drawing.Size(width, height);
            //simpleButton_vazgec.Dock = DockStyle.Right;
            //simpleButton_vazgec.TabIndex = 21;
            //simpleButton_vazgec.TabStop = false;
            //simpleButton_vazgec.Text = "Vazgeç";
            //simpleButton_vazgec.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            //simpleButton_vazgec.AccessibleName = TableIPCode;

            simpleButton_kaydet_cik.Name = "simpleButton_kaydet_cik";
            simpleButton_kaydet_cik.Size = new System.Drawing.Size(width, height);
            simpleButton_kaydet_cik.Dock = DockStyle.Right;
            simpleButton_kaydet_cik.TabIndex = 22;
            simpleButton_kaydet_cik.TabStop = false;
            simpleButton_kaydet_cik.Text = "Kaydet, Çık";
            simpleButton_kaydet_cik.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_kaydet_cik.AccessibleName = xTableIPCode;
            simpleButton_kaydet_cik.Image = t.Find_Glyph("30_341_SaveAndClose_16x16"); 

            simpleButton_kaydet_yeni.Name = "simpleButton_kaydet_yeni";
            simpleButton_kaydet_yeni.Size = new System.Drawing.Size(width, height);
            simpleButton_kaydet_yeni.Dock = DockStyle.Right;
            simpleButton_kaydet_yeni.TabIndex = 23;
            simpleButton_kaydet_yeni.TabStop = false;
            simpleButton_kaydet_yeni.Text = "Yeni";// "Kaydet, Yeni";
            simpleButton_kaydet_yeni.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_kaydet_yeni.AccessibleName = xTableIPCode;
            simpleButton_kaydet_yeni.Image = t.Find_Glyph("30_341_SaveAndNew_16x16"); 

            simpleButton_kaydet.Name = "simpleButton_kaydet";
            simpleButton_kaydet.Size = new System.Drawing.Size(width, height);
            simpleButton_kaydet.Dock = DockStyle.Right;
            simpleButton_kaydet.TabIndex = 24;
            simpleButton_kaydet.TabStop = false;
            simpleButton_kaydet.Text = "Kaydet";
            simpleButton_kaydet.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_kaydet.AccessibleName = xTableIPCode;
            simpleButton_kaydet.AccessibleDescription = Prop_Navigator;
            simpleButton_kaydet.Image = t.Find_Glyph("KAYDET16");

            simpleButton_kaydet_devam.Name = "simpleButton_kaydet_devam";
            simpleButton_kaydet_devam.Size = new System.Drawing.Size(width3 + width_ek, height);
            simpleButton_kaydet_devam.Dock = DockStyle.Right;
            simpleButton_kaydet_devam.TabIndex = 25;
            simpleButton_kaydet_devam.TabStop = false;
            simpleButton_kaydet_devam.Text = "Kaydet, Sonraki";
            simpleButton_kaydet_devam.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_kaydet_devam.AccessibleName = TableIPCode;
            simpleButton_kaydet_devam.Image = t.Find_Glyph("30_341_SaveDialog_16x16"); 

            simpleButton_sil_satir.Name = "simpleButton_sil_satir";
            simpleButton_sil_satir.Size = new System.Drawing.Size(width, height);
            simpleButton_sil_satir.Dock = DockStyle.Right;
            simpleButton_sil_satir.TabIndex = 26;
            simpleButton_sil_satir.TabStop = false;
            simpleButton_sil_satir.Text = "Satır Sil";
            simpleButton_sil_satir.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_sil_satir.AccessibleName = TableIPCode;
            simpleButton_sil_satir.Image = t.Find_Glyph("40_408_Delete_16x16");

            simpleButton_sil_fis.Name = "simpleButton_sil_fis";
            simpleButton_sil_fis.Size = new System.Drawing.Size(width + width_ek, height);
            simpleButton_sil_fis.Dock = DockStyle.Right;
            simpleButton_sil_fis.TabIndex = 27;
            simpleButton_sil_fis.TabStop = false;
            simpleButton_sil_fis.Text = basligi + "Sil";
            simpleButton_sil_fis.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_sil_fis.AccessibleName = TableIPCode;
            simpleButton_sil_fis.Image = t.Find_Glyph("30_308_ChartTitlesNone_16x16");

            simpleButton_iptal_navg.Name = "simpleButton_iptal_navg";
            simpleButton_iptal_navg.Size = new System.Drawing.Size(width, height);
            simpleButton_iptal_navg.Dock = DockStyle.Right;
            simpleButton_iptal_navg.TabIndex = 28;
            simpleButton_iptal_navg.TabStop = false;
            simpleButton_iptal_navg.Text = "İptal";
            simpleButton_iptal_navg.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_iptal_navg.AccessibleName = TableIPCode;
            //simpleButton_iptal_navg.Image = t.Find_Glyph("30_341_SaveDialog_16x16");

            simpleButton_iptal_form.Name = "simpleButton_iptal_form";
            simpleButton_iptal_form.Size = new System.Drawing.Size(width, height);
            simpleButton_iptal_form.Dock = DockStyle.Right;
            simpleButton_iptal_form.TabIndex = 29;
            simpleButton_iptal_form.TabStop = false;
            simpleButton_iptal_form.Text = "İptal";
            simpleButton_iptal_form.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_iptal_form.AccessibleName = TableIPCode;
            //simpleButton_iptal_form.Image = t.Find_Glyph("30_341_SaveDialog_16x16");

            #endregion Kaydet, sil, iptal butonları

            //---------------------------------------------------------
            #region Yeni butonları

            //string button51 = string.Empty;
            //string button52 = string.Empty;
            //string button53 = string.Empty;
            string button54 = string.Empty;
            //string button56 = string.Empty;
            //string button57 = string.Empty;
            //string button58 = string.Empty;
            string Key_FName = t.MyProperties_Get(Prop_Navigator, "KEY_FNAME:");
            string Master_Key_FName = t.MyProperties_Get(Prop_Navigator, "MASTER_KEY_FNAME:");
            string Foreing_FName = t.MyProperties_Get(Prop_Navigator, "FOREING_FNAME:");
            string Parent_FName = t.MyProperties_Get(Prop_Navigator, "PARENT_FNAME:");


            if (t.IsNotNull(Prop_Navigator))
            {
                //button51 = t.Find_Properies_Get_RowBlock(Prop_Navigator, "PROP_NAVIGATOR", "BUTTONTYPE:51");
                //button52 = t.Find_Properies_Get_RowBlock(Prop_Navigator, "PROP_NAVIGATOR", "BUTTONTYPE:52");
                //button53 = t.Find_Properies_Get_RowBlock(Prop_Navigator, "PROP_NAVIGATOR", "BUTTONTYPE:53");
                button54 = t.Find_Properies_Get_RowBlock(Prop_Navigator, "PROP_NAVIGATOR", "BUTTONTYPE:54");
                //button56 = t.Find_Properies_Get_RowBlock(Prop_Navigator, "PROP_NAVIGATOR", "BUTTONTYPE:56");
                //button57 = t.Find_Properies_Get_RowBlock(Prop_Navigator, "PROP_NAVIGATOR", "BUTTONTYPE:57");
                //button58 = t.Find_Properies_Get_RowBlock(Prop_Navigator, "PROP_NAVIGATOR", "BUTTONTYPE:58");
            }

            // sonuna yapılıyor, bu bilgi  simpleButton_yeni_alt_hesap  için gerekli 
            t.MyProperties_Set(ref button54, "KEY_FNAME", Key_FName);
            t.MyProperties_Set(ref button54, "MASTER_KEY_FNAME", Master_Key_FName);
            t.MyProperties_Set(ref button54, "FOREING_FNAME", Foreing_FName);
            t.MyProperties_Set(ref button54, "PARENT_FNAME", Parent_FName);


            // Hesap Aç, (Kart butonları >> ) Kart Aç, Yeni Kart Aç, Yeni Hesap

            // Hesap Aç // Hesap oku (aynı form içinde listedeki bir kaydın detayını başka IP ye aç/oku )
            simpleButton_hesap_ac.Name = "simpleButton_hesap_ac";
            simpleButton_hesap_ac.Size = new System.Drawing.Size(width + width_ek, height);
            simpleButton_hesap_ac.Dock = DockStyle.Right;
            simpleButton_hesap_ac.TabIndex = 50;
            simpleButton_hesap_ac.TabStop = false;
            simpleButton_hesap_ac.Text = basligi + " Aç";//"Hesabı Aç";
            simpleButton_hesap_ac.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_hesap_ac.AccessibleName = TableIPCode;
            simpleButton_hesap_ac.AccessibleDescription = Prop_Navigator; // button50;
            simpleButton_hesap_ac.Image = t.Find_Glyph("30_348_Article_16x16");


            // Kart Aç ve Fiş Aç 
            simpleButton_kart_ac.Name = "simpleButton_kart_ac";
            simpleButton_kart_ac.Size = new System.Drawing.Size(width + width_ek, height);
            simpleButton_kart_ac.Dock = DockStyle.Right;
            simpleButton_kart_ac.TabIndex = 51;
            simpleButton_kart_ac.TabStop = false;
            simpleButton_kart_ac.Text = basligi + "Aç ...";//"Kartı Aç...";
            simpleButton_kart_ac.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_kart_ac.AccessibleName = TableIPCode;
            simpleButton_kart_ac.AccessibleDescription = Prop_Navigator; // button51;
            simpleButton_kart_ac.Image = t.Find_Glyph("40_427_Article_16x16");

            simpleButton_yeni_kart_ac.Name = "simpleButton_yeni_kart_ac";
            simpleButton_yeni_kart_ac.Size = new System.Drawing.Size(width + width_ek, height);
            simpleButton_yeni_kart_ac.Dock = DockStyle.Right;
            simpleButton_yeni_kart_ac.TabIndex = 52;
            simpleButton_yeni_kart_ac.TabStop = false;
            simpleButton_yeni_kart_ac.Text = "Yeni " + baslik + "..."; //"Yeni Kart...";
            simpleButton_yeni_kart_ac.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_yeni_kart_ac.AccessibleName = TableIPCode;
            simpleButton_yeni_kart_ac.AccessibleDescription = Prop_Navigator; // button52;
            simpleButton_yeni_kart_ac.Image = t.Find_Glyph("30_301_AddFile_16x16");

            simpleButton_yeni_hesap.Name = "simpleButton_yeni_hesap";
            simpleButton_yeni_hesap.Size = new System.Drawing.Size(width + width_ek, height);
            simpleButton_yeni_hesap.Dock = DockStyle.Right;
            simpleButton_yeni_hesap.TabIndex = 53;
            simpleButton_yeni_hesap.TabStop = false;
            simpleButton_yeni_hesap.Text = "Yeni " + baslik;//"Yeni Hesap";
            simpleButton_yeni_hesap.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_yeni_hesap.AccessibleName = xTableIPCode;
            simpleButton_yeni_hesap.AccessibleDescription = Prop_Navigator; // button53;
            simpleButton_yeni_hesap.Image = t.Find_Glyph("YENIHESAP16");
            
            /// DATA_READ_TYPE
            /// 1, '', 'Not Read Data'
            /// 2, '', 'Read RefID'
            /// 3, '', 'Detail Table'
            /// 4, '', 'SubDetail Table'
            /// 5, '', 'Data Collection'
            /// 6, '', 'Read SubView'
            /// 
            ////if (Data_Read_Type == 1) 
            ////{
            ////    simpleButton_yeni_hesap.Tag = simpleButton_yeni_hesap.Text;
            ////    simpleButton_yeni_hesap.Text = "Vazgeç";
            ////}

            simpleButton_yeni_alt_hesap.Name = "simpleButton_yeni_alt_hesap";
            simpleButton_yeni_alt_hesap.Size = new System.Drawing.Size(width + width2, height);
            simpleButton_yeni_alt_hesap.Dock = DockStyle.Right;
            simpleButton_yeni_alt_hesap.TabIndex = 54;
            simpleButton_yeni_alt_hesap.TabStop = false;
            simpleButton_yeni_alt_hesap.Text = "Yeni Alt " + baslik;//"Yeni Alt Hesap";
            simpleButton_yeni_alt_hesap.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_yeni_alt_hesap.AccessibleName = xTableIPCode;
            simpleButton_yeni_alt_hesap.AccessibleDescription = button54;

            ////if (Data_Read_Type == 1)
            ////{
            ////    simpleButton_yeni_alt_hesap.Tag = simpleButton_yeni_alt_hesap.Text;
            ////    simpleButton_yeni_alt_hesap.Text = "Vazgeç";
            ////}

            #region
            /*
            // Fiş butonları, Fişi Aç, Yeni Fiş Aç, Yeni Hesap
            simpleButton_fisi_ac.Name = "simpleButton_fisi_ac";
            simpleButton_fisi_ac.Size = new System.Drawing.Size(width, height);
            simpleButton_fisi_ac.Dock = DockStyle.Right;
            simpleButton_fisi_ac.TabIndex = 56;
            simpleButton_fisi_ac.TabStop = false;
            simpleButton_fisi_ac.Text = "Fişi Aç...";
            simpleButton_fisi_ac.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_fisi_ac.AccessibleName = TableIPCode;
            simpleButton_fisi_ac.AccessibleDescription = Prop_Navigator;// button56;
            
            simpleButton_yeni_fis_ac.Name = "simpleButton_yeni_fis_ac";
            simpleButton_yeni_fis_ac.Size = new System.Drawing.Size(width, height);
            simpleButton_yeni_fis_ac.Dock = DockStyle.Right;
            simpleButton_yeni_fis_ac.TabIndex = 57;
            simpleButton_yeni_fis_ac.TabStop = false;
            simpleButton_yeni_fis_ac.Text = "Yeni Fiş Aç...";
            simpleButton_yeni_fis_ac.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_yeni_fis_ac.AccessibleName = TableIPCode;
            simpleButton_yeni_fis_ac.AccessibleDescription = Prop_Navigator;// button57;

            simpleButton_yeni_fis.Name = "simpleButton_yeni_fis";
            simpleButton_yeni_fis.Size = new System.Drawing.Size(width, height);
            simpleButton_yeni_fis.Dock = DockStyle.Right;
            simpleButton_yeni_fis.TabIndex = 58;
            simpleButton_yeni_fis.TabStop = false;
            simpleButton_yeni_fis.Text = "Yeni Fiş";
            simpleButton_yeni_fis.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_yeni_fis.AccessibleName = TableIPCode;
            simpleButton_yeni_fis.AccessibleDescription = Prop_Navigator;// button58;
            */
            #endregion

            #endregion Yeni butonları

            //---------------------------------------------------------
            #region Yön butonları, ileri - geri butonları
            //40_404_Prev_16x16
            simpleButton_en_sona.Name = "simpleButton_en_sona";
            simpleButton_en_sona.Size = new System.Drawing.Size(width1, 23);
            simpleButton_en_sona.Dock = DockStyle.Right;
            simpleButton_en_sona.TabIndex = 41;
            simpleButton_en_sona.TabStop = false;
            simpleButton_en_sona.Text = ">|";
            simpleButton_en_sona.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_en_sona.AccessibleName = TableIPCode;
            //simpleButton_en_sona.Image = t.Find_Glyph("40_404_Last_16x16"); // 20_204_Last_16x16
            //simpleButton_en_sona.ImageLocation = ImageLocation.MiddleCenter;

            simpleButton_sonraki_syf.Name = "simpleButton_sonraki_syf";
            simpleButton_sonraki_syf.Size = new System.Drawing.Size(width1, 23);
            simpleButton_sonraki_syf.Dock = DockStyle.Right;
            simpleButton_sonraki_syf.TabIndex = 42;
            simpleButton_sonraki_syf.TabStop = false;
            simpleButton_sonraki_syf.Text = ">>";
            simpleButton_sonraki_syf.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_sonraki_syf.AccessibleName = TableIPCode;
            //simpleButton_sonraki_syf.Image = t.Find_Glyph("40_404_DoubleNext_16x16"); // 20_204_DoubleNext_16x16


            simpleButton_sonraki.Name = "simpleButton_sonraki";
            simpleButton_sonraki.Size = new System.Drawing.Size(width1, 23);
            simpleButton_sonraki.Dock = DockStyle.Right;
            simpleButton_sonraki.TabIndex = 43;
            simpleButton_sonraki.TabStop = false;
            //simpleButton_sonraki.Text = ">";
            simpleButton_sonraki.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_sonraki.AccessibleName = TableIPCode;
            simpleButton_sonraki.Image = t.Find_Glyph("NAVILERI16"); // 20_217_Forward_16x16
            simpleButton_sonraki.ImageLocation = ImageLocation.MiddleCenter;

            
            if (tDataNavigator != null)
                tDataNavigator.TabIndex = 44;

            simpleButton_onceki.Name = "simpleButton_onceki";
            simpleButton_onceki.Size = new System.Drawing.Size(width1, 23);
            simpleButton_onceki.Dock = DockStyle.Right;
            simpleButton_onceki.TabIndex = 45;
            simpleButton_onceki.TabStop = false;
            //simpleButton_onceki.Text = "<";
            simpleButton_onceki.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_onceki.AccessibleName = TableIPCode;
            simpleButton_onceki.Image = t.Find_Glyph("NAVGERI16"); // 20_217_Backward_16x16
            simpleButton_onceki.ImageLocation = ImageLocation.MiddleCenter;

            simpleButton_onceki_syf.Name = "simpleButton_onceki_syf";
            simpleButton_onceki_syf.Size = new System.Drawing.Size(width1, 23);
            simpleButton_onceki_syf.Dock = DockStyle.Right;
            simpleButton_onceki_syf.TabIndex = 46;
            simpleButton_onceki_syf.TabStop = false;
            simpleButton_onceki_syf.Text = "<<";
            simpleButton_onceki_syf.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_onceki_syf.AccessibleName = TableIPCode;
            //simpleButton_onceki_syf.Image = t.Find_Glyph("40_404_DoublePrev_16x16"); // 20_204_DoublePrev_16x16
            //simpleButton_onceki_syf.ImageLocation = ImageLocation.MiddleCenter;

            simpleButton_en_basa.Name = "simpleButton_en_basa";
            simpleButton_en_basa.Size = new System.Drawing.Size(width1, 23);
            simpleButton_en_basa.Dock = DockStyle.Right;
            simpleButton_en_basa.TabIndex = 47;
            simpleButton_en_basa.TabStop = false;
            simpleButton_en_basa.Text = "|<";
            simpleButton_en_basa.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_en_basa.AccessibleName = TableIPCode;
            //simpleButton_en_basa.Image = t.Find_Glyph("40_404_First_16x16"); // 20_204_First_16x16
            //simpleButton_en_basa.ImageLocation = ImageLocation.MiddleCenter;


            //simpleButton_sihirbaz_geri
            simpleButton_sihirbaz_geri.Name = "simpleButton_sihirbaz_geri";
            simpleButton_sihirbaz_geri.Size = new System.Drawing.Size(width, 23);
            simpleButton_sihirbaz_geri.Dock = DockStyle.Right;
            simpleButton_sihirbaz_geri.TabIndex = 48;
            simpleButton_sihirbaz_geri.TabStop = false;
            simpleButton_sihirbaz_geri.Text = "Geri";
            simpleButton_sihirbaz_geri.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_sihirbaz_geri.AccessibleName = TableIPCode;
            simpleButton_sihirbaz_geri.Enabled = false;
            simpleButton_sihirbaz_geri.Image = t.Find_Glyph("SIHIRBAZGERI16");

            //simpleButton_sihirbaz_devam
            simpleButton_sihirbaz_devam.Name = "simpleButton_sihirbaz_devam";
            simpleButton_sihirbaz_devam.Size = new System.Drawing.Size(width, 23);
            simpleButton_sihirbaz_devam.Dock = DockStyle.Right;
            simpleButton_sihirbaz_devam.TabIndex = 49;
            simpleButton_sihirbaz_devam.TabStop = false;
            simpleButton_sihirbaz_devam.Text = "Devam";
            simpleButton_sihirbaz_devam.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_sihirbaz_devam.AccessibleName = TableIPCode;
            simpleButton_sihirbaz_devam.Image = t.Find_Glyph("SIHIRBAZDEVAM16");


            #endregion Yön butonları ileri - geri butonları

            //---------------------------------------------------------
            #region Onay butonları

            simpleButton_onayla.Name = "simpleButton_onayla";
            simpleButton_onayla.Size = new System.Drawing.Size(width2, 23);
            simpleButton_onayla.Dock = DockStyle.Left;
            simpleButton_onayla.TabIndex = 71;
            simpleButton_onayla.TabStop = false;
            simpleButton_onayla.Text = "+";
            simpleButton_onayla.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_onayla.AccessibleName = TableIPCode;

            simpleButton_onay_iptal.Name = "simpleButton_onay_iptal";
            simpleButton_onay_iptal.Size = new System.Drawing.Size(width2, 23);
            simpleButton_onay_iptal.Dock = DockStyle.Left;
            simpleButton_onay_iptal.TabIndex = 71;
            simpleButton_onay_iptal.TabStop = false;
            simpleButton_onay_iptal.Text = "-";
            simpleButton_onay_iptal.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_onay_iptal.AccessibleName = TableIPCode;

            #endregion Onay butonları

            #region collapse/expand butonları

            // 
            // simpleButton_Expand
            // 
            /*
            simpleButton_expand.Name = "simpleButton_Expand";
            simpleButton_expand.Size = new System.Drawing.Size(width2, 24);
            simpleButton_expand.Dock = System.Windows.Forms.DockStyle.Left;
            simpleButton_expand.TabIndex = 72;
            simpleButton_expand.TabStop = false;
            simpleButton_expand.Text = "Aç";
            simpleButton_expand.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_expand.AccessibleName = TableIPCode;
            simpleButton_expand.GroupIndex = 2;
            simpleButton_expand.Image = t.Find_Glyph("30_319_AutoExpand_16x16");

            // 
            // simpleButton_Collapse
            // 
            simpleButton_collapse.Name = "simpleButton_Collapse";
            simpleButton_collapse.Size = new System.Drawing.Size(width2, 23);
            simpleButton_collapse.Dock = System.Windows.Forms.DockStyle.Left;
            simpleButton_collapse.TabIndex = 72;
            simpleButton_collapse.TabStop = false;
            simpleButton_collapse.Text = "Kpt";
            simpleButton_collapse.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_collapse.AccessibleName = TableIPCode;
            simpleButton_collapse.GroupIndex = 2;
            //simpleButton_collapse.Image = t.Find_Glyph("30_319_AutoExpand_16x16");
            */
            // 
            // simpleButton_CollExp
            // 
            simpleButton_collexp.Name = "simpleButton_CollExp";
            simpleButton_collexp.Size = new System.Drawing.Size(width2, 23);
            simpleButton_collexp.Dock = System.Windows.Forms.DockStyle.Left;
            simpleButton_collexp.TabIndex = 72;
            simpleButton_collexp.TabStop = false;
            simpleButton_collexp.Text = "A";
            simpleButton_collexp.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_collexp.AccessibleName = TableIPCode;
            simpleButton_collexp.GroupIndex = -1;
            simpleButton_collexp.Image = t.Find_Glyph("30_319_AutoExpand_16x16");



            #endregion collapse/expand butonları

            #region yazıcı butonları

            simpleButton_yazici.Name = "simpleButton_yazici";
            simpleButton_yazici.Size = new System.Drawing.Size(width, 23);
            simpleButton_yazici.Dock = DockStyle.Left;
            simpleButton_yazici.TabIndex = 81;
            simpleButton_yazici.TabStop = false;
            simpleButton_yazici.Text = "Yazdır...";
            simpleButton_yazici.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_yazici.AccessibleName = TableIPCode;
            simpleButton_yazici.Image = t.Find_Glyph("30_338_Printer_16x16");

            #endregion yazıcı butonları

            #region Ek1,2,3,4,5 butonları

            simpleButton_ek1.Name = "simpleButton_ek1";
            simpleButton_ek1.Size = new System.Drawing.Size(width, 23);
            simpleButton_ek1.Dock = DockStyle.Left;
            simpleButton_ek1.TabIndex = 91;
            simpleButton_ek1.TabStop = false;
            simpleButton_ek1.Text = ":.";
            //simpleButton_ek1.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_ek1.AccessibleName = TableIPCode;

            simpleButton_ek2.Name = "simpleButton_ek2";
            simpleButton_ek2.Size = new System.Drawing.Size(width, 23);
            simpleButton_ek2.Dock = DockStyle.Left;
            simpleButton_ek2.TabIndex = 92;
            simpleButton_ek2.TabStop = false;
            simpleButton_ek2.Text = ":..";
            //simpleButton_ek2.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_ek2.AccessibleName = TableIPCode;

            #region 
            /*
            simpleButton_ek3.Name = "simpleButton_ek3";
            simpleButton_ek3.Size = new System.Drawing.Size(width, 23);
            simpleButton_ek3.Dock = DockStyle.Left;
            simpleButton_ek3.TabIndex = 93;
            simpleButton_ek3.TabStop = false;
            simpleButton_ek3.Text = ":...";
            //simpleButton_ek3.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_ek3.AccessibleName = TableIPCode;

            simpleButton_ek4.Name = "simpleButton_ek4";
            simpleButton_ek4.Size = new System.Drawing.Size(width, 23);
            simpleButton_ek4.Dock = DockStyle.Left;
            simpleButton_ek4.TabIndex = 94;
            simpleButton_ek4.TabStop = false;
            simpleButton_ek4.Text = ":....";
            //simpleButton_ek4.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_ek4.AccessibleName = TableIPCode;

            simpleButton_ek5.Name = "simpleButton_ek5";
            simpleButton_ek5.Size = new System.Drawing.Size(width, 23);
            simpleButton_ek5.Dock = DockStyle.Left;
            simpleButton_ek5.TabIndex = 95;
            simpleButton_ek5.TabStop = false;
            simpleButton_ek5.Text = ":.....";
            //simpleButton_ek5.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            simpleButton_ek5.AccessibleName = TableIPCode;
            */
            #endregion

            #endregion Ek1,2,3,4,5 butonları

            #region Ek1,2,3,4,5 checkButonlar

            //region Ek1,2,3,4,5 checkButonlar

            checkButton_ek1.Name = "checkButton_ek1";
            checkButton_ek1.Size = new System.Drawing.Size(width, 23);
            checkButton_ek1.Dock = DockStyle.Left;
            checkButton_ek1.TabIndex = 101;
            checkButton_ek1.TabStop = false;
            checkButton_ek1.Text = ":.";
            //checkButton_ek1.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            checkButton_ek1.AccessibleName = TableIPCode;

            /*
            checkButton_ek2.Name = "checkButton_ek2";
            checkButton_ek2.Size = new System.Drawing.Size(width, 23);
            checkButton_ek2.Dock = DockStyle.Left;
            checkButton_ek2.TabIndex = 102;
            checkButton_ek2.TabStop = false;
            checkButton_ek2.Text = ":.";
            //checkButton_ek2.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            checkButton_ek2.AccessibleName = TableIPCode;

            checkButton_ek3.Name = "checkButton_ek3";
            checkButton_ek3.Size = new System.Drawing.Size(width, 23);
            checkButton_ek3.Dock = DockStyle.Left;
            checkButton_ek3.TabIndex = 103;
            checkButton_ek3.TabStop = false;
            checkButton_ek3.Text = ":.";
            //checkButton_ek3.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            checkButton_ek3.AccessibleName = TableIPCode;

            checkButton_ek4.Name = "checkButton_ek4";
            checkButton_ek4.Size = new System.Drawing.Size(width, 23);
            checkButton_ek4.Dock = DockStyle.Left;
            checkButton_ek4.TabIndex = 104;
            checkButton_ek4.TabStop = false;
            checkButton_ek4.Text = ":.";
            //checkButton_ek4.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            checkButton_ek4.AccessibleName = TableIPCode;

            checkButton_ek5.Name = "checkButton_ek5";
            checkButton_ek5.Size = new System.Drawing.Size(width, 23);
            checkButton_ek5.Dock = DockStyle.Left;
            checkButton_ek5.TabIndex = 105;
            checkButton_ek5.TabStop = false;
            checkButton_ek5.Text = ":.";
            //checkButton_ek5.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            checkButton_ek5.AccessibleName = TableIPCode;
            */
            
            #endregion checkButonlar

            #region Ek1,2,3,4,5 DropDownButonlar
            /*
            dragDownButton_ek1.Name = "dragDownButton_ek1";
            dragDownButton_ek1.Size = new System.Drawing.Size(width + 17, 23);
            dragDownButton_ek1.Dock = DockStyle.Left;
            dragDownButton_ek1.TabIndex = 111;
            dragDownButton_ek1.TabStop = false;
            dragDownButton_ek1.Text = ":.";
            //dragDownButton_ek1.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            dragDownButton_ek1.AccessibleName = TableIPCode;

            dragDownButton_ek2.Name = "dragDownButton_ek2";
            dragDownButton_ek2.Size = new System.Drawing.Size(width + 17, 23);
            dragDownButton_ek2.Dock = DockStyle.Left;
            dragDownButton_ek2.TabIndex = 112;
            dragDownButton_ek2.TabStop = false;
            dragDownButton_ek2.Text = ":.";
            //dragDownButton_ek2.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            dragDownButton_ek2.AccessibleName = TableIPCode;

            dragDownButton_ek3.Name = "dragDownButton_ek3";
            dragDownButton_ek3.Size = new System.Drawing.Size(width + 17, 23);
            dragDownButton_ek3.Dock = DockStyle.Left;
            dragDownButton_ek3.TabIndex = 113;
            dragDownButton_ek3.TabStop = false;
            dragDownButton_ek3.Text = ":.";
            //dragDownButton_ek3.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            dragDownButton_ek3.AccessibleName = TableIPCode;

            dragDownButton_ek4.Name = "dragDownButton_ek4";
            dragDownButton_ek4.Size = new System.Drawing.Size(width + 17, 23);
            dragDownButton_ek4.Dock = DockStyle.Left;
            dragDownButton_ek4.TabIndex = 114;
            dragDownButton_ek4.TabStop = false;
            dragDownButton_ek4.Text = ":.";
            //dragDownButton_ek4.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            dragDownButton_ek4.AccessibleName = TableIPCode;

            dragDownButton_ek5.Name = "dragDownButton_ek5";
            dragDownButton_ek5.Size = new System.Drawing.Size(width + 17, 23);
            dragDownButton_ek5.Dock = DockStyle.Left;
            dragDownButton_ek5.TabIndex = 115;
            dragDownButton_ek5.TabStop = false;
            dragDownButton_ek5.Text = ":.";
            //dragDownButton_ek5.Click += new System.EventHandler(ev.btn_Navigotor_Click);
            dragDownButton_ek5.AccessibleName = TableIPCode;
            

            #endregion 
            */
            #endregion

            //DevExpress.XtraEditors.CheckButton checkButton_ek1 = new DevExpress.XtraEditors.CheckButton();
            //DevExpress.XtraEditors.DropDownButton dragDownButton_ek1 = new DevExpress.XtraEditors.DropDownButton();

            #endregion Button Propertios --------------------------------------

            #region Buttons, Panel Add
            //-----------------------------------------------------------------
            i = -1;

            #region Ek1,2,3,4,5 dragDownButtonları
            //-----------------------------------------------------------------
            // Ek1,2,3,4,5 dragDownButtonları
            //-----------------------------------------------------------------
            /*
            i = navigator.IndexOf(dragDownButton_ek5.TabIndex.ToString() + " ");
            if (i > -1) NavPanel.Controls.Add(dragDownButton_ek5);

            i = navigator.IndexOf(dragDownButton_ek4.TabIndex.ToString() + " ");
            if (i > -1) NavPanel.Controls.Add(dragDownButton_ek4);

            i = navigator.IndexOf(dragDownButton_ek3.TabIndex.ToString() + " ");
            if (i > -1) NavPanel.Controls.Add(dragDownButton_ek3);

            i = navigator.IndexOf(dragDownButton_ek2.TabIndex.ToString() + " ");
            if (i > -1) NavPanel.Controls.Add(dragDownButton_ek2);

            i = navigator.IndexOf(dragDownButton_ek1.TabIndex.ToString() + " ");
            if (i > -1) NavPanel.Controls.Add(dragDownButton_ek1);
            */
            #endregion

            #region Ek1,2,3,4,5 checkButtonları
            //-----------------------------------------------------------------
            // Ek1,2,3,4,5 checkButtonları
            //-----------------------------------------------------------------
            /*
            i = navigator.IndexOf(checkButton_ek5.TabIndex.ToString() + " ");
            if (i > -1) NavPanel.Controls.Add(checkButton_ek5);

            i = navigator.IndexOf(simpleButton_ek4.TabIndex.ToString() + " ");
            if (i > -1) NavPanel.Controls.Add(checkButton_ek4);

            i = navigator.IndexOf(checkButton_ek3.TabIndex.ToString() + " ");
            if (i > -1) NavPanel.Controls.Add(checkButton_ek3);

            i = navigator.IndexOf(checkButton_ek2.TabIndex.ToString() + " ");
            if (i > -1) NavPanel.Controls.Add(checkButton_ek2);
            */

            i = navigator.IndexOf(checkButton_ek1.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + checkButton_ek1.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(checkButton_ek1);
                Buttons_CaptionChange(checkButton_ek1, navigator);
            }
            #endregion

            #region Ek1,2,3,4,5 simpleButton_ek
            /*
            //-----------------------------------------------------------------
            // Ek1,2,3,4,5 butonları
            //-----------------------------------------------------------------
            i = navigator.IndexOf(simpleButton_ek5.TabIndex.ToString() + " ");
            if (i > -1) NavPanel.Controls.Add(simpleButton_ek5);

            i = navigator.IndexOf(simpleButton_ek4.TabIndex.ToString() + " ");
            if (i > -1) NavPanel.Controls.Add(simpleButton_ek4);

            i = navigator.IndexOf(simpleButton_ek3.TabIndex.ToString() + " ");
            if (i > -1) NavPanel.Controls.Add(simpleButton_ek3);
            */

            i = navigator.IndexOf(simpleButton_ek2.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_ek2.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_ek2);
                Buttons_CaptionChange(simpleButton_ek2, navigator);
            }

            i = navigator.IndexOf(simpleButton_ek1.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_ek1.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_ek1);
                Buttons_CaptionChange(simpleButton_ek1, navigator);
            }
            #endregion

            //-----------------------------------------------------------------
            // yazıcı butonları
            //-----------------------------------------------------------------
            i = navigator.IndexOf(simpleButton_yazici.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_yazici.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_yazici);
            }

            //-----------------------------------------------------------------
            // collapse/expand butonları
            //-----------------------------------------------------------------
            /*
            i = navigator.IndexOf(simpleButton_expand.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_expand.TabIndex.ToString() + "]");
            if (i > -1) NavPanel.Controls.Add(simpleButton_expand);

            i = navigator.IndexOf(simpleButton_collapse.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_collapse.TabIndex.ToString() + "]");
            if (i > -1) NavPanel.Controls.Add(simpleButton_collapse);
            */

            i = navigator.IndexOf(simpleButton_collexp.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_collexp.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_collexp);
            }

            //-----------------------------------------------------------------
            // onay butonları
            //-----------------------------------------------------------------
            i = navigator.IndexOf(simpleButton_onayla.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_onayla.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_onayla);
            }

            i = navigator.IndexOf(simpleButton_onay_iptal.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_onay_iptal.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_onay_iptal);
            }
            
            //-----------------------------------------------------------------
            // ileri - geri butonları
            //-----------------------------------------------------------------
            i = navigator.IndexOf(simpleButton_en_basa.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_en_basa.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_en_basa);
            }

            i = navigator.IndexOf(simpleButton_onceki_syf.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_onceki_syf.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_onceki_syf);
            }

            i = navigator.IndexOf(simpleButton_onceki.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_onceki.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_onceki);
            }

            // dataNavigator Her şartta visible = false de olsa panelin içinde bulunması gerekmektedir
            if (tDataNavigator != null)
            {
                i = navigator.IndexOf(tDataNavigator.TabIndex.ToString());

                if (i == -1)
                {
                    tDataNavigator.Width = 1;
                    tDataNavigator.Dock = DockStyle.Left;
                }    //    tDataNavigator.Visible = false;

                NavPanel.Controls.Add(tDataNavigator);
            }
            //---

            i = navigator.IndexOf(simpleButton_sonraki.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_sonraki.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_sonraki);
            }

            i = navigator.IndexOf(simpleButton_sonraki_syf.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf(simpleButton_sonraki_syf.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_sonraki_syf);
            }

            i = navigator.IndexOf(simpleButton_en_sona.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_en_sona.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_en_sona);
            }

            //-----------------------------------------------------------------
            // yeni butonları
            //-----------------------------------------------------------------

            //i = navigator.IndexOf(tImageComboBoxEdit_AppPlugList.TabIndex.ToString());
            //if (i > -1)
            //{
            //    // TableIPCode mutlaka tanımlı olması gerekiyor
            //    tEditImageComboBox_Fill(tForm, tImageComboBoxEdit_AppPlugList, TableIPCode);

            //    NavPanel.Controls.Add(tImageComboBoxEdit_AppPlugList);
            //}

            //i = navigator.IndexOf(simpleButton_yeni_fis_ac.TabIndex.ToString());
            //if (i > -1) NavPanel.Controls.Add(simpleButton_yeni_fis_ac);

            //i = navigator.IndexOf(simpleButton_yeni_fis.TabIndex.ToString());
            //if (i > -1) NavPanel.Controls.Add(simpleButton_yeni_fis);

            i = navigator.IndexOf(simpleButton_yeni_kart_ac.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_yeni_kart_ac.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_yeni_kart_ac);
                Buttons_CaptionChange(simpleButton_yeni_kart_ac, navigator);
            }

            i = navigator.IndexOf(simpleButton_yeni_alt_hesap.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_yeni_alt_hesap.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_yeni_alt_hesap);
                Buttons_CaptionChange(simpleButton_yeni_alt_hesap, navigator);
            }

            i = navigator.IndexOf(simpleButton_yeni_hesap.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_yeni_hesap.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_yeni_hesap);
                Buttons_CaptionChange(simpleButton_yeni_hesap, navigator);
            }
            //-----------------------------------------------------------------
            // kaydet, sil, iptal butonları
            //-----------------------------------------------------------------
            i = navigator.IndexOf(simpleButton_iptal_form.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_iptal_form.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_iptal_form);
            }

            i = navigator.IndexOf(simpleButton_iptal_navg.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_iptal_navg.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_iptal_navg);
            }

            i = navigator.IndexOf(simpleButton_sil_fis.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_sil_fis.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_sil_fis);
            }

            i = navigator.IndexOf(simpleButton_sil_satir.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_sil_satir.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_sil_satir);
            }

            i = navigator.IndexOf(simpleButton_kaydet_devam.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_kaydet_devam.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_kaydet_devam);
                Buttons_CaptionChange(simpleButton_kaydet_devam, navigator);
            }

            i = navigator.IndexOf(simpleButton_kaydet.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_kaydet.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_kaydet);
                Buttons_CaptionChange(simpleButton_kaydet, navigator);
            }

            i = navigator.IndexOf(simpleButton_kaydet_yeni.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_kaydet_yeni.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_kaydet_yeni);
                Buttons_CaptionChange(simpleButton_kaydet_yeni, navigator);
            }

            i = navigator.IndexOf(simpleButton_kaydet_cik.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_kaydet_cik.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_kaydet_cik);
                Buttons_CaptionChange(simpleButton_kaydet_cik, navigator);
            }

            //i = navigator.IndexOf(simpleButton_vazgec.TabIndex.ToString() + " ");
            //    navigator.IndexOf("[" + simpleButton_vazgec.TabIndex.ToString() + "]");
            //if (i > -1) NavPanel.Controls.Add(simpleButton_vazgec);

            //-----------------------------------------------------------------
            // çıkış, ara, seç, kart_ac, fis_ac butonları
            //-----------------------------------------------------------------
            
            //i = navigator.IndexOf(simpleButton_fisi_ac.TabIndex.ToString());
            //if (i > -1) NavPanel.Controls.Add(simpleButton_fisi_ac);

            i = navigator.IndexOf(simpleButton_kart_ac.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_kart_ac.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_kart_ac);
                Buttons_CaptionChange(simpleButton_kart_ac, navigator);
            }

            i = navigator.IndexOf(simpleButton_hesap_ac.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf(simpleButton_hesap_ac.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_hesap_ac);
                Buttons_CaptionChange(simpleButton_hesap_ac, navigator);
            }

            i = navigator.IndexOf(simpleButton_arama.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_arama.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_arama);
                Buttons_CaptionChange(simpleButton_arama, navigator);

                if (simpleButton_arama.Text.IndexOf("Sepet") > -1)
                    simpleButton_arama.Image = t.Find_Glyph("10_101_Buy_16x16");
            }

            i = navigator.IndexOf(simpleButton_ekle.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf(simpleButton_ekle.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_ekle);
                Buttons_CaptionChange(simpleButton_ekle, navigator);
            }

            i = navigator.IndexOf(simpleButton_sec.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_sec.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_sec);
                Buttons_CaptionChange(simpleButton_sec, navigator);
            }

            i = navigator.IndexOf(simpleButton_listele.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_listele.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_listele);
                Buttons_CaptionChange(simpleButton_listele, navigator);
            }

            i = navigator.IndexOf(simpleButton_sihirbaz_geri.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_sihirbaz_geri.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_sihirbaz_geri);
            }
            //---

            i = navigator.IndexOf(simpleButton_sihirbaz_devam.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_sihirbaz_devam.TabIndex.ToString() + "]");
            if (i > -1)
            {
                NavPanel.Controls.Add(simpleButton_sihirbaz_devam);
            }
            //---

            i = navigator.IndexOf(simpleButton_cikis.TabIndex.ToString() + " ");
            if (i == -1) i = navigator.IndexOf("[" + simpleButton_cikis.TabIndex.ToString() + "]");
            if (i > -1)
            {
                // Çıkış butonu yanındaki boşluk
                DevExpress.XtraEditors.PanelControl Panel1 = new DevExpress.XtraEditors.PanelControl();
                Panel1.Name = "tPanel1";
                Panel1.Width = width2;
                Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                Panel1.Dock = DockStyle.Right;

                NavPanel.Controls.Add(Panel1);
                // Çıkış Butonu
                simpleButton_cikis.TabIndex = 999;
                NavPanel.Controls.Add(simpleButton_cikis);
                Buttons_CaptionChange(simpleButton_cikis, navigator);
            }

            #endregion Buttons Panel Add
        
        }

        private void Buttons_CaptionChange(DevExpress.XtraEditors.SimpleButton simpleButton_, string navigator )
        {
            
            string buttonIndex = "[" + simpleButton_.TabIndex.ToString() + "]";

            string satir = string.Empty;
            string values = navigator;
            string index = string.Empty;
            string lkp_Caption = string.Empty;
            string lkp_newCaption = string.Empty;
            string lkp_newWidth = string.Empty;
                        
            //btn:1||[11] Çıkış||null||0|ds|

            #region read values
            while (values.IndexOf("btn:") > -1)
            {
                satir = t.Get_And_Clear(ref values, "|ds|") + "||";

                t.Get_And_Clear(ref satir, "btn:");

                index = t.Get_And_Clear(ref satir, "||");
                lkp_Caption = t.Get_And_Clear(ref satir, "||");
                lkp_newCaption = t.Get_And_Clear(ref satir, "||");
                lkp_newWidth = t.Get_And_Clear(ref satir, "||");

                if (lkp_Caption.IndexOf(buttonIndex) > -1)
                {
                    if (lkp_newCaption != "null")
                        simpleButton_.Text = lkp_newCaption;

                    if (lkp_newWidth != "0")
                        simpleButton_.Width = t.myInt32(lkp_newWidth);

                    break;
                }
            
            }
            #endregion

        }

        #endregion Create_Navigator



        #region Create_Properties >> 

        public string Create_PropertiesNavEdit(string thisValues)
        {
            tToolBox t = new tToolBox();

            v.con_OnayChange = true;

            thisValues = thisValues.Trim();

            Form tForm = Create_Form("", "PropertiesNavEdit", "");
            
            tForm.AccessibleDefaultActionDescription = thisValues;

            Create_PropertiesNav_Pages(tForm);
                        
            int i = 500;
            tForm.Width = i;
            tForm.Height = i + (i / 3);

            tFormView(tForm, 4, 1);

            if (tForm.AccessibleDefaultActionDescription != null)
                thisValues = tForm.AccessibleDefaultActionDescription.ToString();

            v.con_LkpOnayChange = false;
            v.con_OnayChange = false;

            return thisValues;
        }
      
        

        public string Create_PropertiesPlusEdit(string TableName, string Main_FieldName,
                      string Width, string thisValues)
        {
            tToolBox t = new tToolBox();
            
            string function_name = "Create PropertiesPlusEdit";
            t.Takipci(function_name, "", '{');

            #region Tanımlar

            tTablesRead tr = new tTablesRead();
            tCreateObject co = new tCreateObject();
            tEvents ev = new tEvents();
            DataSet ds_PropList = new DataSet();

            // PropertiesPlus da birden fazla blok oluşmakta
            // ilk block value okunsun diye ilk Id tespit ediliyor

            string paket_basi = Main_FieldName + "={" + v.ENTER;
            string paket_sonu = Main_FieldName + "=}";

            t.Str_Remove(ref thisValues, paket_basi);
            t.Str_Remove(ref thisValues, paket_sonu);

            thisValues = thisValues.Trim();
            
            //*********************************
            v.con_Value_Min = 999;
            //*********************************

            int Id = ev.Properies_Min_Block_Id(Main_FieldName, thisValues);

            string FirstBlockValue = ev.Properties_SingleBlockValue_Get(Main_FieldName, thisValues, Id);
            string TableIPCode = "tPropertiesPlusEdit";

            tr.MS_Properties_Read(ds_PropList, TableName, Main_FieldName);

            #endregion Tanımlar
            
            #region new Controls
            
            //Form tForm = Create_Form(TableIPCode, "PropertiesPlusEdit", "");
            Form tForm = Create_Form(Main_FieldName, "PropertiesPlusEdit", "");

            DevExpress.XtraEditors.PanelControl panelControl1 = new DevExpress.XtraEditors.PanelControl();
            DevExpress.XtraTab.XtraTabControl tTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            DevExpress.XtraTab.XtraTabPage tTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            DevExpress.XtraTab.XtraTabPage tTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            DevExpress.XtraEditors.GroupControl groupControl = new DevExpress.XtraEditors.GroupControl();
            // 
            // tTabControl1
            // 
            tTabControl1.Dock = DockStyle.Fill;
            tTabControl1.SelectedTabPage = tTabPage1;
            tTabControl1.TabIndex = 0;
            tTabControl1.Width = 300;
            tTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            tTabPage1,
            tTabPage2});
            // 
            // xtraTabPage1
            // 
            tTabPage1.Name = "xtraTabPage1";
            tTabPage1.Size = new System.Drawing.Size(355, 400);
            tTabPage1.Text = "Properties List";
            tTabPage1.Padding = new System.Windows.Forms.Padding(v.Padding4);
            // 
            // xtraTabPage2
            // 
            tTabPage2.Name = "xtraTabPage2";
            tTabPage2.Size = new System.Drawing.Size(355, 400);
            tTabPage2.Text = "Properties Block";
            tTabPage2.Padding = new System.Windows.Forms.Padding(v.Padding4);

            // 
            // panelControl
            // 
            panelControl1.Name = "panelControl1";
            panelControl1.Dock = DockStyle.Left;
            panelControl1.TabIndex = 0;
            panelControl1.Width = 300;
            panelControl1.Visible = true;
            //panelControl1.Controls.Add(groupControl);
            panelControl1.Controls.Add(tTabControl1);

            // 
            // groupControl
            // 
            groupControl.Text = " [ " + TableName + "." + Main_FieldName + " ]";
            groupControl.Visible = true;
            groupControl.Dock = DockStyle.Fill;
            groupControl.Padding = new System.Windows.Forms.Padding(v.Padding4);
            
            #endregion new Controls

            #region new VerticalGrid, listBoxControl, Columns, Buttons Create
            //
            // VerticalGrid Create
            //
            Control cntrl = new Control();
            cntrl = co.Create_View_(v.obj_vw_VGridSingle, 0, "tProperties", "");
             
            if (cntrl != null)
            {
                groupControl.Controls.Add(cntrl);

                Create_Properties_Columns((VGridControl)cntrl, ds_PropList, FirstBlockValue, TableIPCode);
                Create_Properties_Buttons((VGridControl)cntrl, TableIPCode);
                Create_Properties_List(tTabControl1, panelControl1, TableIPCode, thisValues, Main_FieldName, Id);

                ((VGridControl)cntrl).Name = "tForm_" + Main_FieldName;
                ((VGridControl)cntrl).AccessibleDescription = Main_FieldName;
                
                tForm.AccessibleDefaultActionDescription = thisValues;

                tForm.Controls.Add(groupControl);
                tForm.Controls.Add(panelControl1);

                ev.tProperties_VGrid_Enabled(tForm, (VGridControl)cntrl);
                
                tForm.Width = 700;
                tForm.Height = 600; 
                
                tFormView(tForm, 4, 1);

                if (tForm.AccessibleDefaultActionDescription != null)
                    thisValues = tForm.AccessibleDefaultActionDescription.ToString();

                if (thisValues.Length > 5)
                {
                    thisValues = paket_basi + thisValues + v.ENTER + paket_sonu;
                }
            }
                        
            #endregion new VerticalGrid Create

            return thisValues;
                       
        }

        
        public string Create_PropertiesPlusEdit_JSON(string TableName, string Main_FieldName,
                      string Width, string thisValues)
        {
            tToolBox t = new tToolBox();
                        
            #region Tanımlar

            tTablesRead tr = new tTablesRead();
            tCreateObject co = new tCreateObject();
            tEvents ev = new tEvents();
            DataSet ds_PropList = new DataSet();

            /// PropertiesPlus da birden fazla blok oluşmakta
            ///

            /// "SV_LIST": [
            /// {
            /// "CAPTION": "Anull",
            /// "SV_VALUE": "null",
            /// "TABLEIPCODE": "null"
            /// },
            /// {
            /// "CAPTION": "Bnull",
            /// "SV_VALUE": "null",
            /// "TABLEIPCODE": "null"
            /// },
            /// {
            /// "CAPTION": "Cnull",
            /// "SV_VALUE": "null",
            /// "TABLEIPCODE": "null"
            /// }
            /// ]

            string TableIPCode = "tPropertiesPlusEdit";
            string paket_basi_A = (char)34 + Main_FieldName + (char)34 + ": ["; //"GRID": [
            string paket_basi_B = (char)34 + Main_FieldName + (char)34 + ": {"; //"GRID": {
            string paket_basi_C = (char)34 + Main_FieldName + (char)34 + ": ";  //"GRID": 
            string paket_basi_Mevcut = "";

            //string paket_sonu_A = "]";
            //string paket_sonu_B = "}";

            string paket_turu = ev.tPropertiesPacketValue_Preparing(TableName, Main_FieldName, ref thisValues, ref paket_basi_Mevcut, TableIPCode);

            string FirstBlockValue = ev.Properties_SingleBlockValue_Get_JSON(thisValues, 0);
            
            tr.MS_Properties_Read(ds_PropList, TableName, Main_FieldName);

            #endregion Tanımlar

            #region new Controls

            //Form tForm = Create_Form(TableIPCode, "PropertiesPlusEdit", "");
            Form tForm = Create_Form(Main_FieldName, "PropertiesPlusEdit", "");

            DevExpress.XtraEditors.PanelControl panelControl1 = new DevExpress.XtraEditors.PanelControl();
            DevExpress.XtraTab.XtraTabControl tTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            DevExpress.XtraTab.XtraTabPage tTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            DevExpress.XtraTab.XtraTabPage tTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            DevExpress.XtraEditors.GroupControl groupControl = new DevExpress.XtraEditors.GroupControl();
            // 
            // tTabControl1
            // 
            tTabControl1.Dock = DockStyle.Fill;
            tTabControl1.SelectedTabPage = tTabPage1;
            tTabControl1.TabIndex = 0;
            tTabControl1.Width = 300;
            tTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            tTabPage1,
            tTabPage2});
            // 
            // xtraTabPage1
            // 
            tTabPage1.Name = "xtraTabPage1";
            tTabPage1.Size = new System.Drawing.Size(355, 400);
            tTabPage1.Text = "Properties List.JSON";
            tTabPage1.Padding = new System.Windows.Forms.Padding(v.Padding4);
            // 
            // xtraTabPage2
            // 
            tTabPage2.Name = "xtraTabPage2";
            tTabPage2.Size = new System.Drawing.Size(355, 400);
            tTabPage2.Text = "Properties Block.JSON";
            tTabPage2.Padding = new System.Windows.Forms.Padding(v.Padding4);

            // 
            // panelControl
            // 
            panelControl1.Name = "panelControl1";
            panelControl1.Dock = DockStyle.Left;
            panelControl1.TabIndex = 0;
            panelControl1.Width = 300;
            panelControl1.Visible = true;
            panelControl1.Controls.Add(tTabControl1);

            // 
            // groupControl
            // 
            groupControl.Text = " [ " + TableName + "." + Main_FieldName + " ]";
            groupControl.Visible = true;
            groupControl.Dock = DockStyle.Fill;
            groupControl.Padding = new System.Windows.Forms.Padding(v.Padding4);

            #endregion new Controls

            #region new VerticalGrid, listBoxControl, Columns, Buttons Create
            //
            // VerticalGrid Create
            //
            Control cntrl = new Control();
            cntrl = co.Create_View_(v.obj_vw_VGridSingle, 0, "tProperties", "");

            if (cntrl != null)
            {
                groupControl.Controls.Add(cntrl);

                Create_Properties_Columns_JSON((VGridControl)cntrl, ds_PropList, FirstBlockValue, TableIPCode);
                Create_Properties_Buttons_JSON((VGridControl)cntrl, TableIPCode);
                Create_Properties_List_JSON(tTabControl1, panelControl1, TableIPCode, thisValues, Main_FieldName);

                ((VGridControl)cntrl).Name = "tForm_" + Main_FieldName; // "tProperties";
                ((VGridControl)cntrl).AccessibleDescription = Main_FieldName;

                ((VGridControl)cntrl).Tag = TableName;

                tForm.AccessibleDefaultActionDescription = thisValues;
                tForm.Controls.Add(groupControl);
                tForm.Controls.Add(panelControl1);

                //ev.tProperties_VGrid_Enabled(tForm, (VGridControl)cntrl);

                tForm.Width = 700;
                tForm.Height = 600;

                tFormView(tForm, 4, 1);

                if (tForm.AccessibleDefaultActionDescription != null)
                    thisValues = tForm.AccessibleDefaultActionDescription.ToString();

                if (thisValues.Length > 5) 
                {
                    /// pakete [ ] işareti ekleniyor 

                    if (paket_turu == "[]")
                    {
                        if (paket_basi_Mevcut == "A")
                            thisValues = paket_basi_C + thisValues;
                        //thisValues = paket_basi_A + thisValues + v.ENTER + paket_sonu_A;

                        //if (thisValues.IndexOf(": [") == -1)
                        //{
                        //    thisValues = paket_basi_A + thisValues + v.ENTER + paket_sonu_A;
                        //}
                        //else 
                        //{
                        //    thisValues = paket_basi_C + thisValues;
                        //}
                    }
                    else if (paket_turu == "{}")
                    {
                        if (paket_basi_Mevcut == "B")
                            thisValues = paket_basi_C + thisValues;
                    }
                    else if (paket_turu == "")
                    {
                        thisValues = paket_basi_C + thisValues;
                    }
                }
            }

            #endregion new VerticalGrid Create

            return thisValues;

        }
        
        public string Create_PropertiesEdit(string TableName, string Main_FieldName,
                      string Width, string thisValues)
        {
            tToolBox t = new tToolBox();

            string function_name = "Create_PropertiesEdit";
            t.Takipci(function_name, "", '{');

            tTablesRead tr = new tTablesRead();
            tCreateObject co = new tCreateObject();
            DataSet ds_PropList = new DataSet();

            string TableIPCode = "tPropertiesEdit";
            string FirstValues = thisValues;

            tr.MS_Properties_Read(ds_PropList, TableName, Main_FieldName);

            string paket_basi = Main_FieldName + "={" + v.ENTER;
            string paket_sonu = Main_FieldName + "=}";

            t.Str_Remove(ref thisValues, paket_basi);
            t.Str_Remove(ref thisValues, paket_sonu);

            thisValues = thisValues.Trim();

            // Properties de sadece tek blok oluşmakta 
            
            //Form tForm = Create_Form(TableIPCode, "PropertiesEdit", "");
            Form tForm = Create_Form(Main_FieldName, "PropertiesEdit", "");

            Create_Properties_Pages(tForm);

            //DevExpress.XtraEditors.GroupControl groupControl = new DevExpress.XtraEditors.GroupControl();
            //groupControl.Text = " [ " + TableName + "." + Main_FieldName + " ]";
            //groupControl.Visible = true;
            //groupControl.Dock = DockStyle.Fill;

            Control page1 = t.Find_Control(tForm, "tabNavigationPage1");
            Control page2 = t.Find_Control(tForm, "tabNavigationPage2");
            
            Control cntrl = new Control();
            cntrl = co.Create_View_(v.obj_vw_VGridSingle, 0, "tProperties", "");
            if (cntrl != null)
            {
                //groupControl.Controls.Add(cntrl);
                page1.Controls.Add(cntrl);

                Create_Properties_Columns((VGridControl)cntrl, ds_PropList, thisValues, TableIPCode);
                Create_Properties_Buttons((VGridControl)cntrl, TableIPCode);

                ((VGridControl)cntrl).Name = "tForm_" + Main_FieldName;
                ((VGridControl)cntrl).AccessibleDescription = Main_FieldName;


                MemoEdit tEdit = new DevExpress.XtraEditors.MemoEdit();
                tEdit.Name = "memoEdit_PropertiesValue"; //"MemoEdit1";
                tEdit.Dock = DockStyle.Fill;
                tEdit.EditValue = thisValues; // FirstValues;

                page2.Controls.Add(tEdit);

                Create_PropertiesMemo_Buttons(page2, TableIPCode);
                
                tForm.AccessibleDefaultActionDescription = thisValues;

                //tForm.Controls.Add(groupControl);
                
                int i = t.myInt32(Width);
                if (i <= 100) i = 400;
                
                tForm.Width = i;
                tForm.Height = i + ( i / 3 ); 
                
                tFormView(tForm, 4, 1);

                if (tForm.AccessibleDefaultActionDescription != null)
                    thisValues = tForm.AccessibleDefaultActionDescription.ToString();

                if (thisValues.Length > 5)
                {
                    thisValues = paket_basi + thisValues + v.ENTER + paket_sonu;
                }
            }
            
            return thisValues;
      
            // işlem bittikten sonra kendisini create eden buttonEdite dönen değer
        }
        
        public string Create_PropertiesEdit_JSON(string TableName, string Main_FieldName,
                      string Width, string thisValues)
        {
            tToolBox t = new tToolBox();
            tTablesRead tr = new tTablesRead();
            tCreateObject co = new tCreateObject();
            tEvents ev = new tEvents();

            string TableIPCode = "tPropertiesEdit";
            string FirstValues = thisValues;
            
            thisValues = thisValues.Trim();

            /// eğer boş value geldiyse main_field in Modeline bakarak boş array istenir
            //if ((t.IsNotNull(thisValues) == false) || (thisValues == "[]") || (thisValues == "{}"))
            //     thisValues = t.Create_PropertiesEdit_Model_JSON(TableName, Main_FieldName);

            string paket_basi_A = (char)34 + Main_FieldName + (char)34 + ": ["; //"GRID": [
            string paket_basi_B = (char)34 + Main_FieldName + (char)34 + ": {"; //"GRID": {
            string paket_basi_C = (char)34 + Main_FieldName + (char)34 + ": ";  //"GRID": 
            //string paket_sonu_A = "]";
            //string paket_sonu_B = "}";
            string paket_basi_Mevcut = "";

            string paket_turu = ev.tPropertiesPacketValue_Preparing(TableName, Main_FieldName, 
                   ref thisValues, ref paket_basi_Mevcut, TableIPCode);
            
            // Properties de sadece tek blokla oluşmakta 

            //Form tForm = Create_Form(TableIPCode, "PropertiesEdit", "");
            Form tForm = Create_Form(Main_FieldName, "PropertiesEdit", "");

            Create_Properties_Pages(tForm);

            Control page1 = t.Find_Control(tForm, "tabNavigationPage1");
            Control page2 = t.Find_Control(tForm, "tabNavigationPage2");

            Control cntrl = new Control();
            cntrl = co.Create_View_(v.obj_vw_VGridSingle, 0, "tProperties", "");
            if (cntrl != null)
            {
                page1.Controls.Add(cntrl);

                DataSet ds_PropList = new DataSet();
                tr.MS_Properties_Read(ds_PropList, TableName, Main_FieldName);

                Create_Properties_Columns_JSON((VGridControl)cntrl, ds_PropList, thisValues, TableIPCode);
                Create_Properties_Buttons_JSON((VGridControl)cntrl, TableIPCode);

                ((VGridControl)cntrl).Name = "tForm_" + Main_FieldName;
                ((VGridControl)cntrl).AccessibleDescription = Main_FieldName;
                ((VGridControl)cntrl).Tag = TableName;

                MemoEdit tEdit = new DevExpress.XtraEditors.MemoEdit();
                tEdit.Name = "memoEdit_PropertiesValue";// "MemoEdit1";
                tEdit.Dock = DockStyle.Fill;
                tEdit.EditValue = thisValues; // FirstValues;

                page2.Controls.Add(tEdit);

                Create_PropertiesMemo_Buttons(page2, TableIPCode);

                tForm.AccessibleDefaultActionDescription = thisValues;

                int i = t.myInt32(Width);
                if (i <= 100) i = 400;

                tForm.Width = i;
                tForm.Height = i + (i / 3);

                tFormView(tForm, 4, 1);

                if (tForm.AccessibleDefaultActionDescription != null)
                    thisValues = tForm.AccessibleDefaultActionDescription.ToString();

                if (thisValues.Length > 5)
                {
                    /// pakete [ ] işareti ekleniyor 

                    if (paket_turu == "[]")
                    {
                        if (paket_basi_Mevcut == "A")
                            thisValues = paket_basi_C + thisValues;
                        //thisValues = paket_basi_A + thisValues + v.ENTER + paket_sonu_A;
                    }
                    else if (paket_turu == "{}")
                    {
                        if (paket_basi_Mevcut == "B")
                            thisValues = paket_basi_C + thisValues;
                    }
                    else if (paket_turu == "")
                    {
                        /// herhangi bir işleme gerek yok
                        //thisValues = thisValues;
                    }
                }
            }

            return thisValues;

            // işlem bittikten sonra kendisini create eden buttonEdite dönen değer
        }


        //----------------------------------------------------------------------------
        private void Create_PropertiesNav_Pages(Form tForm)
        {
            DevExpress.XtraEditors.GroupControl groupControl1 = new DevExpress.XtraEditors.GroupControl();

            tForm.Controls.Add(groupControl1);

            //
            // groupControl1
            // 
            groupControl1.Name = "groupControl1";
            groupControl1.Size = new System.Drawing.Size(400, 500);
            groupControl1.TabIndex = 1;
            groupControl1.Text = "Button Listesi";
            groupControl1.Dock = DockStyle.Fill;

            string TableIPCode = "3S_MSNAV.3S_MSNAV_01";
            //
            // InputPanel Create
            //
            tInputPanel ip = new tInputPanel();
            ip.Create_InputPanel(tForm, groupControl1, TableIPCode, v.IPdataType_DataView);

            Create_PropertiesNav_Buttons(tForm, TableIPCode);

            string thisValues = string.Empty;
            if (tForm.AccessibleDefaultActionDescription != null)
                thisValues = tForm.AccessibleDefaultActionDescription.ToString();

            if (t.IsNotNull(thisValues))
                Set_PropertiesNavEditValue(tForm, TableIPCode, thisValues);
        }

        private void Set_PropertiesNavEditValue(Form tForm, string TableIPCode, string thisValues)
        {
            tToolBox t = new tToolBox();

            string satir = string.Empty;
            string values = thisValues;
            string index = string.Empty;
            string lkp_Caption = string.Empty;
            string lkp_newCaption = string.Empty;
            string lkp_newWidth = string.Empty;
            int kontrol = 0;

            DataSet dsData = null;
            DataNavigator dN = null;
            t.Find_DataSet(tForm, ref dsData, ref dN, TableIPCode);

            if (t.IsNotNull(dsData) == false) return;

            int i2 = dsData.Tables[0].Rows.Count;

            //btn:1||[11] Çıkış||null||0|ds|

            #region read values
            while (values.IndexOf("btn:") > -1)
            {
                satir = t.Get_And_Clear(ref values, "|ds|") + "||";

                t.Get_And_Clear(ref satir, "btn:");

                index = t.Get_And_Clear(ref satir, "||");
                lkp_Caption = t.Get_And_Clear(ref satir, "||");
                lkp_newCaption = t.Get_And_Clear(ref satir, "||");
                lkp_newWidth = t.Get_And_Clear(ref satir, "||");

                for (int i = 0; i < i2; i++)
                {
                    if (dsData.Tables[0].Rows[i]["LKP_CAPTION"].ToString() == lkp_Caption)
                    {
                        dsData.Tables[0].Rows[i]["LKP_ONAY"] = 1;

                        if (lkp_newCaption != "null")
                            dsData.Tables[0].Rows[i]["LKP_NEW_CAPTION"] = lkp_newCaption;

                        if (lkp_newWidth != "0")
                            dsData.Tables[0].Rows[i]["LKP_NEW_WIDTH"] = lkp_newWidth;

                        kontrol++;

                        break;
                    }
                }

                
            }
            #endregion

            #region eski  kayıt düzenini okuma
            /// navigatorun eski kayıt sistemini okumak ve göstermek için
            /// [11] Çıkış, [24] Kaydet, [42] Sonraki Syf, [43] Sonraki, [44] Kayıt Adedi, [45] Önceki, [46] Önceki Syf, [53] Yeni Hesap

            if (kontrol == 0)
            {
                values = thisValues + ",";
                while (values.IndexOf(",") > -1)
                {
                    lkp_Caption = t.Get_And_Clear(ref values, ",");
                    lkp_Caption = lkp_Caption.Trim();

                    for (int i = 0; i < i2; i++)
                    {
                        if (dsData.Tables[0].Rows[i]["LKP_CAPTION"].ToString().IndexOf(lkp_Caption) > -1)
                        {
                            dsData.Tables[0].Rows[i]["LKP_ONAY"] = 1;
                            break;
                        }
                    }
                }
            } // if kontrol
            #endregion eski  kayıt düzenini okuma


        }
        
        private void Create_Properties_Pages(Form tForm)
        {

            DevExpress.XtraBars.Navigation.TabPane tabPane1 = new DevExpress.XtraBars.Navigation.TabPane();
            DevExpress.XtraBars.Navigation.TabNavigationPage tabNavigationPage1 = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            DevExpress.XtraBars.Navigation.TabNavigationPage tabNavigationPage2 = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            ((System.ComponentModel.ISupportInitialize)(tabPane1)).BeginInit();
            tabPane1.SuspendLayout();
            // 
            // tabPane1
            // 
            tabPane1.Controls.Add(tabNavigationPage1);
            tabPane1.Controls.Add(tabNavigationPage2);
            tabPane1.Location = new System.Drawing.Point(39, 29);
            tabPane1.Name = "tabPane1";
            tabPane1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            tabNavigationPage1,
            tabNavigationPage2});
            tabPane1.RegularSize = new System.Drawing.Size(588, 245);
            tabPane1.SelectedPage = tabNavigationPage1;
            tabPane1.Size = new System.Drawing.Size(588, 245);
            tabPane1.TabIndex = 0;
            tabPane1.Text = "tabPane1";
            tabPane1.Dock = DockStyle.Fill;
            // 
            // tabNavigationPage1
            // 
            tabNavigationPage1.Caption = "Properties";
            tabNavigationPage1.Name = "tabNavigationPage1";
            tabNavigationPage1.Size = new System.Drawing.Size(570, 200);
            // 
            // tabNavigationPage2
            // 
            tabNavigationPage2.Caption = "Full Text";
            tabNavigationPage2.Name = "tabNavigationPage2";
            tabNavigationPage2.Size = new System.Drawing.Size(570, 200);

            tForm.Controls.Add(tabPane1);
        }

        private void Create_PropertiesNav_Buttons(Control cntrl, string TableIPCode)
        {
            tEvents ev = new tEvents();

            DevExpress.XtraEditors.GroupControl groupControl_PropertiesButtons = new DevExpress.XtraEditors.GroupControl();
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Properties = new System.Windows.Forms.TableLayoutPanel();

            DevExpress.XtraEditors.SimpleButton simpleButton_Update = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Tamam = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Temizle = new DevExpress.XtraEditors.SimpleButton();

            cntrl.Controls.Add(groupControl_PropertiesButtons);

            //this.SuspendLayout();
            //
            // groupControl_PropertiesButtons
            // 
            groupControl_PropertiesButtons.Controls.Add(tableLayoutPanel_Properties);
            groupControl_PropertiesButtons.Name = "groupControl_PropertiesButtons_Memo";
            groupControl_PropertiesButtons.Size = new System.Drawing.Size(293, 52);
            groupControl_PropertiesButtons.TabIndex = 1;
            groupControl_PropertiesButtons.Text = "İşlemler";
            groupControl_PropertiesButtons.Dock = DockStyle.Bottom;
            // 
            // tableLayoutPanel_Properties
            // 
            //tableLayoutPanel_Properties.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            tableLayoutPanel_Properties.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            tableLayoutPanel_Properties.ColumnCount = 5;
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Temizle, 0, 0);
            //tableLayoutPanel_Properties.Controls.Add(simpleButton_Collapse, 1, 0);
            //tableLayoutPanel_Properties.Controls.Add(simpleButton_Expand, 2, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Update, 3, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Tamam, 4, 0);
            tableLayoutPanel_Properties.Location = new System.Drawing.Point(2, 21);
            tableLayoutPanel_Properties.Name = "tableLayoutPanel1";
            tableLayoutPanel_Properties.RowCount = 1;
            tableLayoutPanel_Properties.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel_Properties.TabIndex = 0;
            tableLayoutPanel_Properties.Dock = System.Windows.Forms.DockStyle.Fill;

            // 
            // simpleButton_Tamam
            // 
            simpleButton_Tamam.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Tamam.Location = new System.Drawing.Point(189, 6);
            simpleButton_Tamam.Name = "simpleButton_Close1";
            simpleButton_Tamam.Size = new System.Drawing.Size(94, 23);
            simpleButton_Tamam.TabIndex = 1;
            simpleButton_Tamam.Text = "&Close";

            simpleButton_Tamam.Click += new System.EventHandler(ev.btn_PropertiesNav_Click);
            simpleButton_Tamam.AccessibleName = TableIPCode;
                        
            // 
            // simpleButton_Update
            // 
            simpleButton_Update.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Update.Location = new System.Drawing.Point(189, 6);
            simpleButton_Update.Name = "simpleButton_Update1";
            simpleButton_Update.Size = new System.Drawing.Size(94, 23);
            simpleButton_Update.TabIndex = 2;
            simpleButton_Update.Text = "&Update";

            simpleButton_Update.Click += new System.EventHandler(ev.btn_PropertiesNav_Click);
            simpleButton_Update.AccessibleName = TableIPCode;
            // 
            // simpleButton_Temizle
            // 
            simpleButton_Temizle.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Temizle.Location = new System.Drawing.Point(6, 6);
            simpleButton_Temizle.Name = "simpleButton_Clear1";
            simpleButton_Temizle.Size = new System.Drawing.Size(93, 23);
            simpleButton_Temizle.TabIndex = 3;
            simpleButton_Temizle.Text = "&Clear";

            simpleButton_Temizle.Click += new System.EventHandler(ev.btn_PropertiesNav_Click);
            simpleButton_Temizle.AccessibleName = TableIPCode;
            /*
            // 
            // simpleButton_Collapse
            // 
            simpleButton_Collapse.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Collapse.Location = new System.Drawing.Point(105, 6);
            simpleButton_Collapse.Name = "simpleButton_Collapse";
            simpleButton_Collapse.Size = new System.Drawing.Size(36, 23);
            simpleButton_Collapse.TabIndex = 4;
            simpleButton_Collapse.Text = "Coll";

            simpleButton_Collapse.Click += new System.EventHandler(ev.btn_Properties_Click);
            simpleButton_Collapse.AccessibleName = TableIPCode;
            simpleButton_Collapse.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Expand
            // 
            simpleButton_Expand.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Expand.Location = new System.Drawing.Point(147, 6);
            simpleButton_Expand.Name = "simpleButton_Expand";
            simpleButton_Expand.Size = new System.Drawing.Size(36, 24);
            simpleButton_Expand.TabIndex = 5;
            simpleButton_Expand.Text = "Exp";

            simpleButton_Expand.Click += new System.EventHandler(ev.btn_Properties_Click);
            simpleButton_Expand.AccessibleName = TableIPCode;
            simpleButton_Expand.AccessibleDescription = tVGrid.Name.ToString();
            */

        }

        private void Create_PropertiesMemo_Buttons(Control page, string TableIPCode)
        {
            tEvents ev = new tEvents();

            DevExpress.XtraEditors.GroupControl groupControl_PropertiesButtons = new DevExpress.XtraEditors.GroupControl();
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Properties = new System.Windows.Forms.TableLayoutPanel();

            //DevExpress.XtraEditors.SimpleButton simpleButton_Temizle = new DevExpress.XtraEditors.SimpleButton();
            //DevExpress.XtraEditors.SimpleButton simpleButton_Collapse = new DevExpress.XtraEditors.SimpleButton();
            //DevExpress.XtraEditors.SimpleButton simpleButton_Expand = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Update = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Tamam = new DevExpress.XtraEditors.SimpleButton();

            //Control cntrl = tVGrid.Parent;
            page.Controls.Add(groupControl_PropertiesButtons);

            //this.SuspendLayout();
            //
            // groupControl_PropertiesButtons
            // 
            groupControl_PropertiesButtons.Controls.Add(tableLayoutPanel_Properties);
            groupControl_PropertiesButtons.Name = "groupControl_PropertiesButtons_Memo";
            groupControl_PropertiesButtons.Size = new System.Drawing.Size(293, 52);
            groupControl_PropertiesButtons.TabIndex = 1;
            groupControl_PropertiesButtons.Text = "İşlemler";
            groupControl_PropertiesButtons.Dock = DockStyle.Bottom;
            // 
            // tableLayoutPanel_Properties
            // 
            //tableLayoutPanel_Properties.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            tableLayoutPanel_Properties.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            tableLayoutPanel_Properties.ColumnCount = 5;
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            //tableLayoutPanel_Properties.Controls.Add(simpleButton_Temizle, 0, 0);
            //tableLayoutPanel_Properties.Controls.Add(simpleButton_Collapse, 1, 0);
            //tableLayoutPanel_Properties.Controls.Add(simpleButton_Expand, 2, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Update, 3, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Tamam, 4, 0);
            tableLayoutPanel_Properties.Location = new System.Drawing.Point(2, 21);
            tableLayoutPanel_Properties.Name = "tableLayoutPanel_Kriter_Memo";
            tableLayoutPanel_Properties.RowCount = 1;
            tableLayoutPanel_Properties.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel_Properties.TabIndex = 0;
            tableLayoutPanel_Properties.Dock = System.Windows.Forms.DockStyle.Fill;

            // 
            // simpleButton_Tamam
            // 
            simpleButton_Tamam.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Tamam.Location = new System.Drawing.Point(189, 6);
            simpleButton_Tamam.Name = "simpleButton_Close_Memo";
            simpleButton_Tamam.Size = new System.Drawing.Size(94, 23);
            simpleButton_Tamam.TabIndex = 1;
            simpleButton_Tamam.Text = "&Close Memo";

            simpleButton_Tamam.Click += new System.EventHandler(ev.btn_Properties_Click);
            simpleButton_Tamam.AccessibleName = TableIPCode;
            //simpleButton_Tamam.AccessibleDescription = tVGrid.Name.ToString();

            // 
            // simpleButton_Update
            // 
            simpleButton_Update.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Update.Location = new System.Drawing.Point(189, 6);
            simpleButton_Update.Name = "simpleButton_Update_Memo";
            simpleButton_Update.Size = new System.Drawing.Size(94, 23);
            simpleButton_Update.TabIndex = 2;
            simpleButton_Update.Text = "&Update Memo";

            simpleButton_Update.Click += new System.EventHandler(ev.btn_Properties_Click);
            simpleButton_Update.AccessibleName = TableIPCode;
            //simpleButton_Update.AccessibleDescription = tVGrid.Name.ToString();
            /*
            // 
            // simpleButton_Temizle
            // 
            simpleButton_Temizle.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Temizle.Location = new System.Drawing.Point(6, 6);
            simpleButton_Temizle.Name = "simpleButton_Clear";
            simpleButton_Temizle.Size = new System.Drawing.Size(93, 23);
            simpleButton_Temizle.TabIndex = 3;
            simpleButton_Temizle.Text = "&Clear";

            simpleButton_Temizle.Click += new System.EventHandler(ev.btn_Properties_Click);
            simpleButton_Temizle.AccessibleName = TableIPCode;
            simpleButton_Temizle.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Collapse
            // 
            simpleButton_Collapse.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Collapse.Location = new System.Drawing.Point(105, 6);
            simpleButton_Collapse.Name = "simpleButton_Collapse";
            simpleButton_Collapse.Size = new System.Drawing.Size(36, 23);
            simpleButton_Collapse.TabIndex = 4;
            simpleButton_Collapse.Text = "Coll";

            simpleButton_Collapse.Click += new System.EventHandler(ev.btn_Properties_Click);
            simpleButton_Collapse.AccessibleName = TableIPCode;
            simpleButton_Collapse.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Expand
            // 
            simpleButton_Expand.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Expand.Location = new System.Drawing.Point(147, 6);
            simpleButton_Expand.Name = "simpleButton_Expand";
            simpleButton_Expand.Size = new System.Drawing.Size(36, 24);
            simpleButton_Expand.TabIndex = 5;
            simpleButton_Expand.Text = "Exp";

            simpleButton_Expand.Click += new System.EventHandler(ev.btn_Properties_Click);
            simpleButton_Expand.AccessibleName = TableIPCode;
            simpleButton_Expand.AccessibleDescription = tVGrid.Name.ToString();
            */

        }

        //--- JSON
        private void Create_Properties_Columns_JSON(VGridControl tVGrid, DataSet dsFields,
             string thisValue, string prp_type)
        {
            tToolBox t = new tToolBox();
            tDevColumn dc = new tDevColumn();
            tDefaultValue df = new tDefaultValue();

            string s = string.Empty;
            string tTableName = string.Empty;
            string tRowFName = string.Empty;
            string tRowCaption = string.Empty;
            string tColumnType = string.Empty;
            string tTypeName = string.Empty;
            string tValue = string.Empty;
            //string paket_basi = string.Empty;
            //string paket_sonu = string.Empty;
            
            byte tRowType = 0;
            byte tCategoryNo = 0;
            byte tRowLineNo = 0;
            Int16 tRowFieldType = 0;
                        
            // emanete ver
            tVGrid.AccessibleDefaultActionDescription = thisValue;

            foreach (DataRow Row in dsFields.Tables[0].Rows)
            {
                tTableName = t.Set(Row["TABLENAME"].ToString(), "", "null");
                tRowFName = t.Set(Row["ROW_FIELDNAME"].ToString(), "", "null");
                tRowType = t.Set(Row["ROW_TYPE"].ToString(), "", (byte)2);
                tCategoryNo = t.Set(Row["ROW_CATEGORY_NO"].ToString(), "", (byte)0);
                tRowLineNo = t.Set(Row["ROW_LINE_NO"].ToString(), "", (byte)0);
                tRowCaption = t.Set(Row["ROW_CAPTION"].ToString(), "", tRowFName);
                tRowFieldType = t.Set(Row["ROW_FIELDTYPE"].ToString(), "", (Int16)167);
                tColumnType = t.Set(Row["ROW_COLUMN_TYPE"].ToString(), "", "TextEdit");
                tTypeName = t.Set(Row["LIST_TYPES_NAME"].ToString(), "", "");

                if (tRowType == 1)
                {
                    CategoryRow CatRow = new CategoryRow(tRowCaption);
                    CatRow.Name = "CategoryRow_" + tCategoryNo.ToString();
                    tVGrid.Rows.Add(CatRow);
                }

                if (tRowType == 2)
                {

                    tValue = t.Find_Properties_Value(thisValue, tRowFName);
                    
                    EditorRow EditRow = new EditorRow("EditRow_" + tRowFName);
                    EditRow.Properties.Caption = tRowCaption;
                    EditRow.Properties.FieldName = tRowFName;
                    EditRow.Tag = tRowFieldType;
                    EditRow.Properties.Value = tValue;

                    s = string.Empty;
                    t.MyProperties_Set(ref s, "TableName", tTableName);
                    t.MyProperties_Set(ref s, "FieldName", tRowFName);
                    t.MyProperties_Set(ref s, "ColumnType", tColumnType);
                    t.MyProperties_Set(ref s, "TypeName", tTypeName);
                    t.MyProperties_Set(ref s, "Width", "0");

                    dc.VGrid_ColumnEdit_(Row, EditRow, s, "", 1); //Tumu = hayır

                    tVGrid.Rows["CategoryRow_" + tCategoryNo.ToString()].ChildRows.Add(EditRow);
                }

            } // foreach
            
        }

        private void Create_Properties_Buttons_JSON(VGridControl tVGrid, string TableIPCode)
        {
            tEvents ev = new tEvents();

            DevExpress.XtraEditors.GroupControl groupControl_PropertiesButtons = new DevExpress.XtraEditors.GroupControl();
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Properties = new System.Windows.Forms.TableLayoutPanel();

            DevExpress.XtraEditors.SimpleButton simpleButton_Temizle = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Collapse = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Expand = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Update = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Tamam = new DevExpress.XtraEditors.SimpleButton();

            Control cntrl = tVGrid.Parent;
            cntrl.Controls.Add(groupControl_PropertiesButtons);

            //this.SuspendLayout();
            //
            // groupControl_PropertiesButtons
            // 
            groupControl_PropertiesButtons.Controls.Add(tableLayoutPanel_Properties);
            groupControl_PropertiesButtons.Name = "groupControl_PropertiesButtons";
            groupControl_PropertiesButtons.Size = new System.Drawing.Size(293, 52);
            groupControl_PropertiesButtons.TabIndex = 1;
            groupControl_PropertiesButtons.Text = "İşlemler.JS";
            groupControl_PropertiesButtons.Dock = DockStyle.Bottom;
            // 
            // tableLayoutPanel_Properties
            // 
            //tableLayoutPanel_Properties.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            tableLayoutPanel_Properties.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            tableLayoutPanel_Properties.ColumnCount = 5;
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Temizle, 0, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Collapse, 1, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Expand, 2, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Update, 3, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Tamam, 4, 0);
            tableLayoutPanel_Properties.Location = new System.Drawing.Point(2, 21);
            tableLayoutPanel_Properties.Name = "tableLayoutPanel_Kriter";
            tableLayoutPanel_Properties.RowCount = 1;
            tableLayoutPanel_Properties.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel_Properties.TabIndex = 0;
            tableLayoutPanel_Properties.Dock = System.Windows.Forms.DockStyle.Fill;

            // 
            // simpleButton_Tamam
            // 
            simpleButton_Tamam.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Tamam.Location = new System.Drawing.Point(189, 6);
            simpleButton_Tamam.Name = "simpleButton_Close";
            simpleButton_Tamam.Size = new System.Drawing.Size(94, 23);
            simpleButton_Tamam.TabIndex = 1;
            simpleButton_Tamam.Text = "&Close";

            simpleButton_Tamam.Click += new System.EventHandler(ev.btn_Properties_JSON_Click);
            simpleButton_Tamam.AccessibleName = TableIPCode;
            simpleButton_Tamam.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Update
            // 
            simpleButton_Update.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Update.Location = new System.Drawing.Point(189, 6);
            simpleButton_Update.Name = "simpleButton_Update";
            simpleButton_Update.Size = new System.Drawing.Size(94, 23);
            simpleButton_Update.TabIndex = 2;
            simpleButton_Update.Text = "&Update";

            simpleButton_Update.Click += new System.EventHandler(ev.btn_Properties_JSON_Click);
            simpleButton_Update.AccessibleName = TableIPCode;
            simpleButton_Update.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Temizle
            // 
            simpleButton_Temizle.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Temizle.Location = new System.Drawing.Point(6, 6);
            simpleButton_Temizle.Name = "simpleButton_Clear";
            simpleButton_Temizle.Size = new System.Drawing.Size(93, 23);
            simpleButton_Temizle.TabIndex = 3;
            simpleButton_Temizle.Text = "&Clear";

            simpleButton_Temizle.Click += new System.EventHandler(ev.btn_Properties_JSON_Click);
            simpleButton_Temizle.AccessibleName = TableIPCode;
            simpleButton_Temizle.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Collapse
            // 
            simpleButton_Collapse.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Collapse.Location = new System.Drawing.Point(105, 6);
            simpleButton_Collapse.Name = "simpleButton_Collapse";
            simpleButton_Collapse.Size = new System.Drawing.Size(36, 23);
            simpleButton_Collapse.TabIndex = 4;
            simpleButton_Collapse.Text = "Coll";

            simpleButton_Collapse.Click += new System.EventHandler(ev.btn_Properties_JSON_Click);
            simpleButton_Collapse.AccessibleName = TableIPCode;
            simpleButton_Collapse.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Expand
            // 
            simpleButton_Expand.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Expand.Location = new System.Drawing.Point(147, 6);
            simpleButton_Expand.Name = "simpleButton_Expand";
            simpleButton_Expand.Size = new System.Drawing.Size(36, 24);
            simpleButton_Expand.TabIndex = 5;
            simpleButton_Expand.Text = "Exp";

            simpleButton_Expand.Click += new System.EventHandler(ev.btn_Properties_JSON_Click);
            simpleButton_Expand.AccessibleName = TableIPCode;
            simpleButton_Expand.AccessibleDescription = tVGrid.Name.ToString();

        }

        private void Create_Properties_List_JSON(
                     DevExpress.XtraTab.XtraTabControl tTabControl1,
                     DevExpress.XtraEditors.PanelControl panelControl1,
                     string TableIPCode, string thisValue, string MainFName)//, int BlockId)
        {
            // old - bir şeklide eski yapı gelirse 
            if (thisValue.IndexOf("={") > -1)
            {
                return;
            }

            tToolBox t = new tToolBox();
            tEvents ev = new tEvents();

            #region Controls
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Properties = new System.Windows.Forms.TableLayoutPanel();
            DevExpress.XtraEditors.GroupControl groupControl_PropertiesPlusButtons = new DevExpress.XtraEditors.GroupControl();
            DevExpress.XtraEditors.ListBoxControl listBoxControl1 = new DevExpress.XtraEditors.ListBoxControl();
            DevExpress.XtraEditors.MemoEdit memoEdit1 = new DevExpress.XtraEditors.MemoEdit();
            DevExpress.XtraEditors.TextEdit textEdit_ViewID = new DevExpress.XtraEditors.TextEdit();
            DevExpress.XtraEditors.SimpleButton simpleButton_PrpSil = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_PrpEkle = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_BlokEkle = new DevExpress.XtraEditors.SimpleButton();

            //
            // groupControl_PropertiesButtons
            // 
            groupControl_PropertiesPlusButtons.Controls.Add(tableLayoutPanel_Properties);
            groupControl_PropertiesPlusButtons.Name = "groupControl_PropertiesPlusButtons";
            groupControl_PropertiesPlusButtons.Size = new System.Drawing.Size(293, 52);
            groupControl_PropertiesPlusButtons.TabIndex = 1;
            groupControl_PropertiesPlusButtons.Text = "İşlemler.JSON";
            groupControl_PropertiesPlusButtons.Dock = DockStyle.Bottom;
            // 
            // tableLayoutPanel_Properties
            // 
            tableLayoutPanel_Properties.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            tableLayoutPanel_Properties.ColumnCount = 3;
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            tableLayoutPanel_Properties.Controls.Add(simpleButton_PrpSil, 0, 0);
            tableLayoutPanel_Properties.Controls.Add(textEdit_ViewID, 1, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_PrpEkle, 2, 0);
            tableLayoutPanel_Properties.Location = new System.Drawing.Point(2, 21);
            tableLayoutPanel_Properties.Name = "tableLayoutPanel_PropertiesPlus";
            tableLayoutPanel_Properties.RowCount = 1;
            tableLayoutPanel_Properties.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel_Properties.TabIndex = 0;
            tableLayoutPanel_Properties.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // textEdit_ViewID
            // 
            textEdit_ViewID.Dock = System.Windows.Forms.DockStyle.Top;
            textEdit_ViewID.Name = "textEdit_ViewID";
            textEdit_ViewID.TabIndex = 3;
            textEdit_ViewID.Properties.ReadOnly = true;
            textEdit_ViewID.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_ViewID.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            textEdit_ViewID.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            textEdit_ViewID.Text = "0";//BlockId.ToString();
            // 
            // simpleButton_PrpEkle
            // 
            simpleButton_PrpEkle.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_PrpEkle.Location = new System.Drawing.Point(189, 6);
            simpleButton_PrpEkle.Name = "simpleButton_Ekle";
            simpleButton_PrpEkle.Size = new System.Drawing.Size(94, 23);
            simpleButton_PrpEkle.TabIndex = 1;
            simpleButton_PrpEkle.Text = "&Add";
            simpleButton_PrpEkle.Click += new System.EventHandler(ev.btn_Properties_JSON_Click);
            // 
            // simpleButton_PrpSil
            // 
            simpleButton_PrpSil.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_PrpSil.Location = new System.Drawing.Point(6, 6);
            simpleButton_PrpSil.Name = "simpleButton_Sil";
            simpleButton_PrpSil.Size = new System.Drawing.Size(93, 23);
            simpleButton_PrpSil.TabIndex = 2;
            simpleButton_PrpSil.Text = "&Delete";
            simpleButton_PrpSil.Click += new System.EventHandler(ev.btn_Properties_JSON_Click);
            //
            // Properties listesi 
            //
            listBoxControl1.Name = "listControl1";
            listBoxControl1.Dock = DockStyle.Fill;
            listBoxControl1.AccessibleName = TableIPCode;
            listBoxControl1.SelectedIndexChanged += new System.EventHandler(ev.tProperties_listBoxControl_SelectedIndexChanged_JSON);
            listBoxControl1.AccessibleDescription = MainFName;
            //
            // memoEdit1 
            //
            memoEdit1.Name = "memoEdit_PropertiesValue"; 
            memoEdit1.Dock = DockStyle.Fill;
            memoEdit1.Properties.AccessibleName = TableIPCode;
            memoEdit1.EditValue = thisValue;
            //
            // simpleButton_BlokEkle
            // 
            simpleButton_BlokEkle.Dock = System.Windows.Forms.DockStyle.Bottom;
            simpleButton_BlokEkle.Location = new System.Drawing.Point(6, 6);
            simpleButton_BlokEkle.Name = "simpleButton_BlokEkle";
            simpleButton_BlokEkle.Size = new System.Drawing.Size(93, 23);
            simpleButton_BlokEkle.TabIndex = 3;
            simpleButton_BlokEkle.Text = "&Memo Add";
            simpleButton_BlokEkle.Click += new System.EventHandler(ev.btn_Properties_JSON_Click);

            #endregion Controls

            /// 'Table1': [
            /// {
            ///    'id': 0,
            ///    'item': 'item 0'
            /// },
            /// {
            ///    'id': 1,
            ///    'item': 'item 1'
            /// }
            /// ]
            ///
            /// {
            ///   "Name": "Acme Ltd.",
            ///   "Employees": [
            ///     {
            ///       "$id": "1",
            ///       "Name": "George-Michael",
            ///       "Manager": null
            ///     },
            ///     {
            ///       "$id": "2",
            ///       "Name": "Maeby",
            ///       "Manager": {
            ///         "$ref": "1"
            ///       }
            ///     }
            ///   ]
            /// }

            #region listBoxControl u doldurma işlemi
            
            listBoxControl1.Tag = 1;

            if (((thisValue.IndexOf("[") > -1) &&
                 (thisValue.IndexOf("[") < 5)) == false)
                  thisValue = "[" + v.ENTER + thisValue + v.ENTER + "]";

            var myList = JsonConvert.DeserializeObject(thisValue);
            int q = 0;
            int i = 0;
            string value = string.Empty;
            foreach (Newtonsoft.Json.Linq.JToken item in (Newtonsoft.Json.Linq.JArray)myList)
            {
                q = 0;
                foreach (var item2 in (Newtonsoft.Json.Linq.JToken)item)
                {
                    // ilk bilgiyi alıp listBox a ekleyelim, genelde caption value
                    if (q == 0)
                    {
                        value = item2.ToString();
                        value = t.MyProperties_Get(value, (char)34 + ": " + (char)34); //"
                        value = " " + i.ToString() + " : " + value.Trim();
                        listBoxControl1.Items.Add(value);
                        break;
                    }
                    q++;
                }
                i++;
            }
            
            listBoxControl1.Tag = 0;
            listBoxControl1.SelectedIndex = 0;
            
            #endregion listBoxControl
            
            tTabControl1.TabPages[0].Controls.Add(listBoxControl1);
            tTabControl1.TabPages[1].Controls.Add(memoEdit1);
            tTabControl1.TabPages[1].Controls.Add(simpleButton_BlokEkle);
            panelControl1.Controls.Add(groupControl_PropertiesPlusButtons);

        }

        ///--- JSON

        ///--- OLD  
        private void Create_Properties_Columns(VGridControl tVGrid, DataSet dsFields, 
                     string thisValue, string prp_type)
        {
            tToolBox t = new tToolBox();
            string function_name = "Create_Properties_Column";
            t.Takipci(function_name, "", '{');

            tDevColumn dc = new tDevColumn();
            tDefaultValue df = new tDefaultValue();
            
            string s = string.Empty;
            string tTableName = string.Empty;
            string tRowFName = string.Empty;
            string tRowCaption = string.Empty;
            string tColumnType = string.Empty;
            string tTypeName = string.Empty;
            string tValue = string.Empty;
            string paket_basi = string.Empty;
            string paket_sonu = string.Empty;
            int j1, j2 = 0;

            byte tRowType = 0;
            byte tCategoryNo = 0;
            byte tRowLineNo = 0;
            Int16 tRowFieldType = 0;

            // emanete ver
            tVGrid.AccessibleDefaultActionDescription = thisValue;

            foreach (DataRow Row in dsFields.Tables[0].Rows)
            {
                tTableName = t.Set(Row["TABLENAME"].ToString(), "", "null");
                tRowFName = t.Set(Row["ROW_FIELDNAME"].ToString(), "", "null");
                tRowType = t.Set(Row["ROW_TYPE"].ToString(), "", (byte)2);
                tCategoryNo = t.Set(Row["ROW_CATEGORY_NO"].ToString(), "", (byte)0);
                tRowLineNo = t.Set(Row["ROW_LINE_NO"].ToString(), "", (byte)0);
                tRowCaption = t.Set(Row["ROW_CAPTION"].ToString(), "", tRowFName);
                tRowFieldType = t.Set(Row["ROW_FIELDTYPE"].ToString(), "", (Int16)167);
                tColumnType = t.Set(Row["ROW_COLUMN_TYPE"].ToString(), "", "TextEdit");
                tTypeName = t.Set(Row["LIST_TYPES_NAME"].ToString(), "", "");

                if (tRowType == 1)
                {
                    CategoryRow CatRow = new CategoryRow(tRowCaption);
                    CatRow.Name = "CategoryRow_" + tCategoryNo.ToString();
                    tVGrid.Rows.Add(CatRow);
                }

                if (tRowType == 2)
                {
                    s = thisValue;

                    paket_basi = tRowFName + "={" + v.ENTER;
                    paket_sonu = tRowFName + "=};" + v.ENTER;

                    if (s.IndexOf(paket_basi) > -1)
                    {
                        j1 = s.IndexOf(paket_basi);
                        j2 = s.IndexOf(paket_sonu) + paket_sonu.Length;
                        tValue = s.Substring(j1, j2-j1);
                    }
                    else
                    {
                        tValue = t.MyProperties_Get(s, "=" + tRowFName + ":");
                        tValue = tValue.Trim();
                    }

                    tValue = tValue.Trim();

                    if ((tValue == "null;\r\n") ||
                            (tValue == "null")) tValue = "";

                    if (tValue.Length > 0)
                    {
                        if (tValue.Substring(tValue.Length - 1, 1) == ";")
                        {
                            tValue = tValue.Substring(0, tValue.Length - 1);
                        }
                    }
                                        

                    EditorRow EditRow = new EditorRow("EditRow_" + tRowFName);
                    EditRow.Properties.Caption = tRowCaption;
                    EditRow.Properties.FieldName = tRowFName;
                    EditRow.Tag = tRowFieldType;
                    EditRow.Properties.Value = tValue; 

                    s = string.Empty;
                    t.MyProperties_Set(ref s, "TableName", tTableName);
                    t.MyProperties_Set(ref s, "FieldName", tRowFName);
                    t.MyProperties_Set(ref s, "ColumnType", tColumnType);
                    t.MyProperties_Set(ref s, "TypeName", tTypeName);
                    t.MyProperties_Set(ref s, "Width", "0");
            
                    dc.VGrid_ColumnEdit_(Row, EditRow, s, "", 1); //Tumu = hayır
                    
                    tVGrid.Rows["CategoryRow_" + tCategoryNo.ToString()].ChildRows.Add(EditRow);
                }
                
            } // foreach
            
            t.Takipci(function_name, "", '}');
        }

        private void Create_Properties_Buttons(VGridControl tVGrid, string TableIPCode)
        {
            tEvents ev = new tEvents();

            DevExpress.XtraEditors.GroupControl groupControl_PropertiesButtons = new DevExpress.XtraEditors.GroupControl();
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Properties = new System.Windows.Forms.TableLayoutPanel();
            
            DevExpress.XtraEditors.SimpleButton simpleButton_Temizle = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Collapse = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Expand = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Update = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Tamam = new DevExpress.XtraEditors.SimpleButton();
            
            Control cntrl = tVGrid.Parent;
            cntrl.Controls.Add(groupControl_PropertiesButtons);

            //this.SuspendLayout();
            //
            // groupControl_PropertiesButtons
            // 
            groupControl_PropertiesButtons.Controls.Add(tableLayoutPanel_Properties);
            groupControl_PropertiesButtons.Name = "groupControl_PropertiesButtons";
            groupControl_PropertiesButtons.Size = new System.Drawing.Size(293, 52); 
            groupControl_PropertiesButtons.TabIndex = 1;
            groupControl_PropertiesButtons.Text = "İşlemler";
            groupControl_PropertiesButtons.Dock = DockStyle.Bottom;
            // 
            // tableLayoutPanel_Properties
            // 
            //tableLayoutPanel_Properties.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            tableLayoutPanel_Properties.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            tableLayoutPanel_Properties.ColumnCount = 5;
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Temizle, 0, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Collapse, 1, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Expand, 2, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Update, 3, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_Tamam, 4, 0);
            tableLayoutPanel_Properties.Location = new System.Drawing.Point(2, 21);
            tableLayoutPanel_Properties.Name = "tableLayoutPanel_Kriter";
            tableLayoutPanel_Properties.RowCount = 1;
            tableLayoutPanel_Properties.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel_Properties.TabIndex = 0;
            tableLayoutPanel_Properties.Dock = System.Windows.Forms.DockStyle.Fill;
            
            // 
            // simpleButton_Tamam
            // 
            simpleButton_Tamam.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Tamam.Location = new System.Drawing.Point(189, 6);
            simpleButton_Tamam.Name = "simpleButton_Close";
            simpleButton_Tamam.Size = new System.Drawing.Size(94, 23);
            simpleButton_Tamam.TabIndex = 1;
            simpleButton_Tamam.Text = "&Close";

            simpleButton_Tamam.Click += new System.EventHandler(ev.btn_Properties_Click);
            simpleButton_Tamam.AccessibleName = TableIPCode;
            simpleButton_Tamam.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Update
            // 
            simpleButton_Update.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Update.Location = new System.Drawing.Point(189, 6);
            simpleButton_Update.Name = "simpleButton_Update";
            simpleButton_Update.Size = new System.Drawing.Size(94, 23);
            simpleButton_Update.TabIndex = 2;
            simpleButton_Update.Text = "&Update";

            simpleButton_Update.Click += new System.EventHandler(ev.btn_Properties_Click);
            simpleButton_Update.AccessibleName = TableIPCode;
            simpleButton_Update.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Temizle
            // 
            simpleButton_Temizle.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Temizle.Location = new System.Drawing.Point(6, 6);
            simpleButton_Temizle.Name = "simpleButton_Clear";
            simpleButton_Temizle.Size = new System.Drawing.Size(93, 23);
            simpleButton_Temizle.TabIndex = 3;
            simpleButton_Temizle.Text = "&Clear";

            simpleButton_Temizle.Click += new System.EventHandler(ev.btn_Properties_Click);
            simpleButton_Temizle.AccessibleName = TableIPCode;
            simpleButton_Temizle.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Collapse
            // 
            simpleButton_Collapse.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Collapse.Location = new System.Drawing.Point(105, 6);
            simpleButton_Collapse.Name = "simpleButton_Collapse";
            simpleButton_Collapse.Size = new System.Drawing.Size(36, 23);
            simpleButton_Collapse.TabIndex = 4;
            simpleButton_Collapse.Text = "Coll";

            simpleButton_Collapse.Click += new System.EventHandler(ev.btn_Properties_Click);
            simpleButton_Collapse.AccessibleName = TableIPCode;
            simpleButton_Collapse.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Expand
            // 
            simpleButton_Expand.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Expand.Location = new System.Drawing.Point(147, 6);
            simpleButton_Expand.Name = "simpleButton_Expand";
            simpleButton_Expand.Size = new System.Drawing.Size(36, 24);
            simpleButton_Expand.TabIndex = 5;
            simpleButton_Expand.Text = "Exp";

            simpleButton_Expand.Click += new System.EventHandler(ev.btn_Properties_Click);
            simpleButton_Expand.AccessibleName = TableIPCode;
            simpleButton_Expand.AccessibleDescription = tVGrid.Name.ToString();

        }

        private void Create_Properties_List(
                     DevExpress.XtraTab.XtraTabControl tTabControl1, 
                     DevExpress.XtraEditors.PanelControl panelControl1,
                     string TableIPCode, string thisValue, string MainFName, int BlockId)
        {
            tToolBox t = new tToolBox();
            tEvents ev = new tEvents();

            #region Controls
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Properties = new System.Windows.Forms.TableLayoutPanel();
            DevExpress.XtraEditors.GroupControl groupControl_PropertiesPlusButtons = new DevExpress.XtraEditors.GroupControl();
            DevExpress.XtraEditors.ListBoxControl listBoxControl1 = new DevExpress.XtraEditors.ListBoxControl();
            DevExpress.XtraEditors.MemoEdit memoEdit1 = new DevExpress.XtraEditors.MemoEdit();
            DevExpress.XtraEditors.TextEdit textEdit_ViewID = new DevExpress.XtraEditors.TextEdit();
            DevExpress.XtraEditors.SimpleButton simpleButton_PrpSil = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_PrpEkle = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_BlokEkle = new DevExpress.XtraEditors.SimpleButton();

            //
            // groupControl_PropertiesButtons
            // 
            groupControl_PropertiesPlusButtons.Controls.Add(tableLayoutPanel_Properties);
            groupControl_PropertiesPlusButtons.Name = "groupControl_PropertiesPlusButtons";
            groupControl_PropertiesPlusButtons.Size = new System.Drawing.Size(293, 52);
            groupControl_PropertiesPlusButtons.TabIndex = 1;
            groupControl_PropertiesPlusButtons.Text = "İşlemler";
            groupControl_PropertiesPlusButtons.Dock = DockStyle.Bottom;
            // 
            // tableLayoutPanel_Properties
            // 
            tableLayoutPanel_Properties.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            tableLayoutPanel_Properties.ColumnCount = 3;
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel_Properties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            tableLayoutPanel_Properties.Controls.Add(simpleButton_PrpSil, 0, 0);
            tableLayoutPanel_Properties.Controls.Add(textEdit_ViewID, 1, 0);
            tableLayoutPanel_Properties.Controls.Add(simpleButton_PrpEkle, 2, 0);
            tableLayoutPanel_Properties.Location = new System.Drawing.Point(2, 21);
            tableLayoutPanel_Properties.Name = "tableLayoutPanel_PropertiesPlus";
            tableLayoutPanel_Properties.RowCount = 1;
            tableLayoutPanel_Properties.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel_Properties.TabIndex = 0;
            tableLayoutPanel_Properties.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // textEdit_ViewID
            // 
            textEdit_ViewID.Dock = System.Windows.Forms.DockStyle.Top;
            textEdit_ViewID.Name = "textEdit_ViewID";
            textEdit_ViewID.TabIndex = 3;
            textEdit_ViewID.Properties.ReadOnly = true;
            textEdit_ViewID.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_ViewID.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            textEdit_ViewID.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            textEdit_ViewID.Text = BlockId.ToString();
            // 
            // simpleButton_PrpEkle
            // 
            simpleButton_PrpEkle.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_PrpEkle.Location = new System.Drawing.Point(189, 6);
            simpleButton_PrpEkle.Name = "simpleButton_Ekle";
            simpleButton_PrpEkle.Size = new System.Drawing.Size(94, 23);
            simpleButton_PrpEkle.TabIndex = 1;
            simpleButton_PrpEkle.Text = "&Add";
            simpleButton_PrpEkle.Click += new System.EventHandler(ev.btn_Properties_Click);
            // 
            // simpleButton_PrpSil
            // 
            simpleButton_PrpSil.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_PrpSil.Location = new System.Drawing.Point(6, 6);
            simpleButton_PrpSil.Name = "simpleButton_Sil";
            simpleButton_PrpSil.Size = new System.Drawing.Size(93, 23);
            simpleButton_PrpSil.TabIndex = 2;
            simpleButton_PrpSil.Text = "&Delete";
            simpleButton_PrpSil.Click += new System.EventHandler(ev.btn_Properties_Click);
            //
            // Properties listesi 
            //
            listBoxControl1.Name = "listControl1";
            listBoxControl1.Dock = DockStyle.Fill;
            listBoxControl1.AccessibleName = TableIPCode;
            listBoxControl1.SelectedIndexChanged += new System.EventHandler(ev.tProperties_listBoxControl_SelectedIndexChanged);
            listBoxControl1.AccessibleDescription = MainFName;
            //
            // memoEdit1 
            //
            memoEdit1.Name = "memoEdit_PropertiesValue"; 
            memoEdit1.Dock = DockStyle.Fill;
            memoEdit1.Properties.AccessibleName = TableIPCode;
            memoEdit1.EditValue = thisValue;
            //
            // simpleButton_BlokEkle
            // 
            simpleButton_BlokEkle.Dock = System.Windows.Forms.DockStyle.Bottom;
            simpleButton_BlokEkle.Location = new System.Drawing.Point(6, 6);
            simpleButton_BlokEkle.Name = "simpleButton_BlokEkle";
            simpleButton_BlokEkle.Size = new System.Drawing.Size(93, 23);
            simpleButton_BlokEkle.TabIndex = 3;
            simpleButton_BlokEkle.Text = "&Memo Add";
            simpleButton_BlokEkle.Click += new System.EventHandler(ev.btn_Properties_Click);

            #endregion Controls

            string blok = string.Empty;
            string caption = string.Empty;
            string fcaption = string.Empty;
            int Id = 0;
            listBoxControl1.Tag = 1;
            
            string lockE = "=ROWE_" + MainFName + ":";
            /* örnek            
            1=ROW_PROP_NAVIGATOR:1;    <<< başlangıç
            1=CAPTION:null;
            1=BUTTONTYPE:null;
            1=TABLEIPCODE_LIST:null;
            1=FORMNAME:null;
            1=FORMCODE:null;
            1=FORMTYPE:null;
            1=FORMSTATE:null;
            1=ROWE_PROP_NAVIGATOR:1;   <<< bitiş
            */
            
            #region // doğru caption bilgisi için aradaki paketler çıkarılacak

            int i1, i2, j1, j2 = 0;
            i1 = 0;
            i2 = thisValue.Length;
            
            string paket_fname = string.Empty;
            string paket_basi = string.Empty;
            string paket_sonu = string.Empty;
            
            // 1=TABLEIPCODE_LIST:TABLEIPCODE_LIST={    <<< başlangıç
            // 1=ROW_TABLEIPCODE_LIST:1;                
            // .....                                    <<< bu satırlar arası silinecek
            // 1=ROWE_TABLEIPCODE_LIST:1;
            // TABLEIPCODE_LIST=};                      <<< bitiş

            // iş bitiminde 
            // 1=TABLEIPCODE_LIST:null;                 <<< şekline dönecek 

            while (thisValue.IndexOf("={") > 0)
            {
                if (thisValue[i1] == '{')
                {
                    i1--;    // ={  eşittir işareti için 
                    j2 = i1; // bitiş no
                    j1 = i1; 
                    while (thisValue[j1] != ':')
                    {
                        j1--; // başlangıç no
                    }

                    if ((j1 > 0) && (j2 > 0))
                    {
                        j1++; // : sonrası başlasın diye
                        paket_fname = thisValue.Substring(j1, (j2 - j1));
                        paket_basi = paket_fname + "={";
                        paket_sonu = paket_fname + "=};" + v.ENTER;

                        j1 = thisValue.IndexOf(paket_basi);
                        j2 = thisValue.IndexOf(paket_sonu) + paket_sonu.Length;

                        thisValue = thisValue.Remove(j1, j2 - j1); // paket sil
                        thisValue = thisValue.Insert(j1, "null;" + v.ENTER); // yerine null koy

                        i2 = thisValue.Length;
                    }
                }
                i1++;
            }
            #endregion

            while (thisValue.IndexOf(lockE) > -1) 
            {
                blok = t.Get_And_Clear(ref thisValue, lockE);
                caption = ev.Properties_for_List_Caption(MainFName, blok, ref Id);
                if (fcaption == string.Empty) fcaption = caption;
                listBoxControl1.Items.Add(caption);
            }
            
            listBoxControl1.Tag = 0;
            listBoxControl1.SelectedIndex = 0;

            tTabControl1.TabPages[0].Controls.Add(listBoxControl1);
            tTabControl1.TabPages[1].Controls.Add(memoEdit1);
            tTabControl1.TabPages[1].Controls.Add(simpleButton_BlokEkle);
            panelControl1.Controls.Add(groupControl_PropertiesPlusButtons);
            
        }
        
        ///--- OLD

        #endregion Create_Properties  <<

        #region Create AdvBandedGrid_Group_Buttons
        public void Create_AdvBandedGrid_Group_Buttons(Form tForm, Control tPanelControl,
                                                       DataRow row_Table, DataSet ds_Fields)
        {
            tToolBox t = new tToolBox();
            tDevView dv = new tDevView();

            int RefID = t.Set(row_Table["REF_ID"].ToString(), "", 0);
            string TableIPCode = row_Table["TABLE_CODE"].ToString() + "." + row_Table["IP_CODE"].ToString();

            #region Button Paneli

            DevExpress.XtraEditors.PanelControl buttonsPanelControl = new DevExpress.XtraEditors.PanelControl();
            buttonsPanelControl.Name = "tPanel_Navigator_" + RefID.ToString();
            buttonsPanelControl.Height = 26;
            buttonsPanelControl.Dock = DockStyle.Top;
            buttonsPanelControl.SendToBack();

            #endregion Button Paneli

            #region Create Buttons

            // Add Bands >> Buton Paneli ve butonları oluştur
            dv.GridBands_Add(null, null, buttonsPanelControl, ds_Fields, TableIPCode);

            #endregion Create Buttons

            tPanelControl.Controls.Add(buttonsPanelControl);
        }
        #endregion Create AdvBandedGrid_Group_Buttons

        #region Create SpeedKriter / On/Off Kriter

        public void Create_SpeedKriter(Form tForm, Control tPanelControl,
                                       DataRow row_Table, DataSet dsFields, string MultiPageID)
        {
            tToolBox t = new tToolBox();
            
            int RefID = t.Set(row_Table["REF_ID"].ToString(), "", 0);
            string TableIPCode = row_Table["TABLE_CODE"].ToString() + "." + row_Table["IP_CODE"].ToString();

            if (t.IsNotNull(MultiPageID))
                if (MultiPageID.IndexOf("|") == -1)
                    MultiPageID = "|" + MultiPageID;

            int i2 = dsFields.Tables[0].Rows.Count;
            int i3 = 0;
            byte toperand_type = 0;
            
            // SpeedKriter Tespiti yapılıyor
            for (int i = 0; i < i2; i++)
            {
                toperand_type = t.Set(dsFields.Tables[0].Rows[i]["KRT_OPERAND_TYPE"].ToString(),"", (byte)0);
                if ((toperand_type == 3) || (toperand_type == 4))
                    i3++;
            }

            if (i3 == 0) return;

            tDevColumn dc = new tDevColumn();
 
            ///'KRT_OPERAND_TYPE',    0, '', '');
             ///'KRT_OPERAND_TYPE',    1, '', 'Even (Double)');
             ///'KRT_OPERAND_TYPE',    2, '', 'Odd  (Single)');
             ///'KRT_OPERAND_TYPE',    3, '', 'Speed (Double)');
             ///'KRT_OPERAND_TYPE',    4, '', 'Speed (Single)');
             ///'KRT_OPERAND_TYPE',    5, '', 'On/Off');
             ///'KRT_OPERAND_TYPE',    9, '', 'Visible=False');
             ///'KRT_OPERAND_TYPE',   11, '', '>=');
             ///'KRT_OPERAND_TYPE',   12, '', '>');
             ///'KRT_OPERAND_TYPE',   13, '', '<=');
             ///'KRT_OPERAND_TYPE',   14, '', '<');
             ///'KRT_OPERAND_TYPE',   15, '', '<>');
             ///'KRT_OPERAND_TYPE',   16, '', 'Benzerleri (%abc%)');
             ///'KRT_OPERAND_TYPE',   17, '', 'Benzerleri (abc%)');
 
            #region tableLayoutPanel

            System.Windows.Forms.TableLayoutPanel tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            //tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            //tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            tableLayoutPanel.Name = "tPanel_SpeedKriter_" + t.AntiStr_Dot(TableIPCode) + MultiPageID; //RefID.ToString();
            tableLayoutPanel.Height = 60;
            tableLayoutPanel.ColumnCount = i3 + 1;
            tableLayoutPanel.RowCount = 2;
            
            //tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());

            tableLayoutPanel.Padding = new System.Windows.Forms.Padding(v.Padding4);
            tableLayoutPanel.TabIndex = 0;
            tableLayoutPanel.Dock = DockStyle.Top;
            tableLayoutPanel.SendToBack();

            tPanelControl.Controls.Add(tableLayoutPanel);

            #endregion tableLayoutPanel
            
            #region  Tanımlar
            int row_no = 0;
            int twidth = 0;
            string tfield_name = string.Empty;
            string tcolumntype = string.Empty;
            string tcaption = string.Empty;
            string thint = string.Empty;
            Boolean tenabled = false;
            Boolean tvisible = false;
            #endregion

            #region // row içine Edit ler yerleştiriliyor
            foreach (DataRow Row in dsFields.Tables[0].Rows)
            {
                tvisible = t.Set(Row["CMP_VISIBLE"].ToString(), "", true);
                toperand_type = t.Set(Row["KRT_OPERAND_TYPE"].ToString(), "", (byte)0);

                if ((toperand_type == 3) || // 3,  Speed (Double)
                    (toperand_type == 4) || // 4,  Speed (Single)
                    (toperand_type == 5)    // 5,  On/Off
                    )
                {
                    //-----
                    tfield_name = t.Set(Row["LKP_FIELD_NAME"].ToString(), "", "");
                    tcolumntype = t.Set(Row["CMP_COLUMN_TYPE"].ToString(), Row["LKP_CMP_COLUMN_TYPE"].ToString(), "TextEdit");
                    tcaption = t.Set(Row["FCAPTION"].ToString(), Row["LKP_FCAPTION"].ToString(), tfield_name);
                    thint = t.Set(Row["FHINT"].ToString(), "", "");
                    tenabled = t.Set(Row["CMP_ENABLED"].ToString(), Row["LKP_FENABLED"].ToString(), true);
                    twidth = t.Set(Row["CMP_WIDTH"].ToString(), Row["LKP_CMP_WIDTH"].ToString(), 150);
                    //tgrp_sirano = t.Set(Row["KRT_LINE_NO"].ToString(), "", -1); // GROUP_LINE_NO

                    /// birinci nesne, Başlangıç
                    #region bir/bas
                    System.Windows.Forms.Panel item1 = new System.Windows.Forms.Panel();
                    DevExpress.XtraEditors.LabelControl labelControl1 = new DevExpress.XtraEditors.LabelControl();

                    item1.Name = "Panel_" + tfield_name;
                    item1.AccessibleName = TableIPCode + MultiPageID;
                    item1.Width = twidth;
                    labelControl1.Text = tcaption;
                    labelControl1.ForeColor = System.Drawing.SystemColors.ButtonShadow;

                    if (tcolumntype == "DateEdit") tcolumntype = "DateEdit_SpeedKriter_BAS";
                    if (tcolumntype == "tImageComboBoxEdit2Button") tcolumntype = "ImageComboBoxEdit";

                    //5, '', 'On/Off'
                    if (toperand_type == 5) tcolumntype = "ToggleSwitch";
                    
                    dc.tXtraEditors_Edit(Row, null, null, item1, tcolumntype, "", 2, ""); // .. ,dsData , ... , FormName
                                        
                    item1.Width = item1.Controls[0].Width + 1;
                    item1.Height = item1.Controls[0].Height + 1;

                    tableLayoutPanel.Controls.Add(labelControl1, row_no, 0);
                    tableLayoutPanel.Controls.Add(item1, row_no, 1);
                    #endregion bir/bas
                    
                    /// ikinci nesne (bitiş)
                    #region ikinci/bit
                    if (toperand_type == 3) // 3,  Speed (Double)
                    {
                        System.Windows.Forms.Panel item2 = new System.Windows.Forms.Panel();
                        DevExpress.XtraEditors.LabelControl labelControl2 = new DevExpress.XtraEditors.LabelControl();

                        item2.Name = "Panel_" + tfield_name;
                        item2.AccessibleName = TableIPCode + MultiPageID;
                        item2.Width = twidth;
                        labelControl2.Text = "";// tcaption;
                        labelControl2.ForeColor = System.Drawing.SystemColors.ButtonShadow;

                        if (tcolumntype == "DateEdit_SpeedKriter_BAS") tcolumntype = "DateEdit_SpeedKriter_BIT";
                        
                        dc.tXtraEditors_Edit(Row, null, null, item2, tcolumntype, "", 2, ""); // .. ,dsData , ... , FormName

                        item2.Width = item2.Controls[0].Width + 1;
                        item2.Height = item2.Controls[0].Height + 1;

                        tableLayoutPanel.ColumnCount++;

                        tableLayoutPanel.Controls.Add(labelControl2, row_no + 1, 0);
                        tableLayoutPanel.Controls.Add(item2, row_no + 1, 1);

                        row_no++;
                    }
                    #endregion ikinci/bit

                    //-----
                    row_no++;
                }
            }
            #endregion 
                        
        }

        #endregion Create SpeedKriter

        public void Create_GridFindPanel(Form tForm, Control tPanelControl, DevExpress.XtraGrid.GridControl cntrl)
        {
            bool find = false;

            string TableIPCode = ((DevExpress.XtraGrid.GridControl)cntrl).AccessibleName;
            string cntrlName = ((DevExpress.XtraGrid.GridControl)cntrl).Name.ToString();

            int findType = 0;
            /// ( 100 * find )  neden bunu yapıyorum ?
            /// findDelay = 100 ise standart find
            /// findDelay = 200 ise list && data  yı işaret ediyor 
            
            if (((DevExpress.XtraGrid.GridControl)cntrl).MainView.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
            {
                GridView view = ((DevExpress.XtraGrid.GridControl)cntrl).MainView as GridView;
                find = (view.OptionsFind.AlwaysVisible);
                // aslını gizleyelim yerine kendi panelimizi ekleyelim
                if (find) view.OptionsFind.AlwaysVisible = false;
                findType = view.OptionsFind.FindDelay; //= 100 * find;
            }

            if (((DevExpress.XtraGrid.GridControl)cntrl).MainView.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
            {
                AdvBandedGridView view = ((DevExpress.XtraGrid.GridControl)cntrl).MainView as AdvBandedGridView;
                find = (view.OptionsFind.AlwaysVisible);
                // aslını gizleyelim yerine kendi panelimizi ekleyelim
                if (find) view.OptionsFind.AlwaysVisible = false;
                findType = view.OptionsFind.FindDelay; //= 100 * find;
            }
            
            ((DevExpress.XtraGrid.GridControl)cntrl).TabIndex = 1;

            Create_MyFindPanel_(tForm, tPanelControl, find, TableIPCode, cntrlName, findType);
        }

        public void Create_TreeFindPanel(Form tForm, Control tPanelControl, DevExpress.XtraTreeList.TreeList cntrl)
        {
            bool find = false;

            string TableIPCode = ((DevExpress.XtraTreeList.TreeList)cntrl).AccessibleName;
            string cntrlName = ((DevExpress.XtraTreeList.TreeList)cntrl).Name.ToString();

            int findType = 0;
            /// ( 100 * find )  neden bunu yapıyorum ?
            /// findDelay = 100 ise standart find
            /// findDelay = 200 ise list && data  yı işaret ediyor 

            find = ((DevExpress.XtraTreeList.TreeList)cntrl).OptionsFind.AlwaysVisible;
            // aslını gizleyelim yerine kendi panelimizi ekleyelim
            if (find) ((DevExpress.XtraTreeList.TreeList)cntrl).OptionsFind.AlwaysVisible = false;
            findType = ((DevExpress.XtraTreeList.TreeList)cntrl).OptionsFind.FindDelay; // = 100 * find;
            
            Create_MyFindPanel_(tForm, tPanelControl, find, TableIPCode, cntrlName, findType);
        }

        private void Create_MyFindPanel_(Form tForm, 
            Control tPanelControl,
            bool find,
            string TableIPCode,
            string cntrlName,
            int findType
            )
        { 
            if (find)
            {
                tEvents ev = new tEvents();
                
                #region find edit create
                 
                /// label1
                /// 
                DevExpress.XtraEditors.LabelControl label1_Find = new DevExpress.XtraEditors.LabelControl();
                label1_Find.Text = "Aradığınız ?";
                label1_Find.ForeColor = System.Drawing.SystemColors.ButtonShadow;
                /// label2
                /// 
                DevExpress.XtraEditors.LabelControl label2_Find = new DevExpress.XtraEditors.LabelControl();
                label2_Find.Text = "Arama Yöntemi";
                label2_Find.ForeColor = System.Drawing.SystemColors.ButtonShadow;

                ///
                /// textEdit_Find
                /// 
                DevExpress.XtraEditors.TextEdit textEdit_Find = new DevExpress.XtraEditors.TextEdit();

                textEdit_Find.Location = new System.Drawing.Point(30, 1);
                textEdit_Find.Size = new System.Drawing.Size(200, 20);
                textEdit_Find.Name = "textEdit_" + cntrlName;
                //textEdit_Find.Properties.NullText = "Aradığınızı bulmak için, burayı kullanın ...";
                textEdit_Find.Properties.Appearance.ForeColor = v.AppearanceTextColor;
                textEdit_Find.Properties.AppearanceFocused.ForeColor = v.AppearanceFocusedTextColor;
                textEdit_Find.Properties.NullText = "Aradığınızı bulmak için, kelime girin ...";
                textEdit_Find.TabIndex = 0;
                textEdit_Find.EditValueChanged += new System.EventHandler(ev.textEdit_Find_EditValueChanged);
                textEdit_Find.Enter += new System.EventHandler(ev.textEdit_Find_Enter);
                textEdit_Find.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.textEdit_Find_KeyDown);
                

                // kullanılacak find type takibi için
                textEdit_Find.Tag = findType;

                /// diğer tDevColumn lada beraber çalışması için   AccessibleDefaultActionDescription seçilmiştir.
                /// if ((tcolumn_type == "ButtonEdit") || (tcolumn_type == "tSearchEdit"))  { .. }
                ///  
                textEdit_Find.Properties.AccessibleDefaultActionDescription = TableIPCode;

                ///
                /// tToggleSwitch
                /// 
                ToggleSwitch tToggleSwitch = new DevExpress.XtraEditors.ToggleSwitch();
                tToggleSwitch.Name = "toggleSwitch_" + cntrlName;
                tToggleSwitch.Location = new System.Drawing.Point(250, 1);
                tToggleSwitch.Size = new System.Drawing.Size(125,24);//(200, 24);
                tToggleSwitch.Properties.AccessibleDefaultActionDescription = TableIPCode;
                tToggleSwitch.Properties.OffText = "Liste"; //"Liste üzerinde Arama";
                tToggleSwitch.Properties.OnText = "Şartlı";  //"Data içinde Arama";
                tToggleSwitch.TabIndex = 2;

                //tToggleSwitch.IsOn = (v.search_CARI_ARAMA_TD == v.search_inData);
                /// findType = 100 ise standart find
                /// findType = 200 ise list && data  yı işaret ediyor 
                tToggleSwitch.IsOn = (findType == 200);  
                tToggleSwitch.EditValueChanged += new System.EventHandler(ev.toggleSwitch_Find_EditValueChanged);
                tToggleSwitch.TabIndex = 1;
                
                // standart ise
                if (findType == 100)
                    v.search_CARI_ARAMA_TD = v.search_onList;
                                
                /// 
                /// TableLayoutPanel
                /// 
                /// System.Windows.Forms.TableLayoutPanel 
                Control tableLayoutPanel = null;

                tableLayoutPanel = t.Find_Control(tForm, "tPanel_SpeedKriter_" + t.AntiStr_Dot(TableIPCode));

                if (tableLayoutPanel == null)
                {
                    tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
                    //((TableLayoutPanel)tableLayoutPanel).CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
                    //((TableLayoutPanel)tableLayoutPanel).CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
                    tableLayoutPanel.Name = "tPanel_SpeedKriter_" + t.AntiStr_Dot(TableIPCode); //RefID.ToString();
                    tableLayoutPanel.Height = 60;
                    ((TableLayoutPanel)tableLayoutPanel).ColumnCount = 3;
                    ((TableLayoutPanel)tableLayoutPanel).RowCount = 2;
                    ((TableLayoutPanel)tableLayoutPanel).Dock = DockStyle.Top;
                    ((TableLayoutPanel)tableLayoutPanel).SendToBack();

                    ((TableLayoutPanel)tableLayoutPanel).Controls.Add(label1_Find, 0, 0);
                    ((TableLayoutPanel)tableLayoutPanel).Controls.Add(textEdit_Find, 0, 1);

                    // list && Data ise
                    if (findType == 200)
                    {
                        ((TableLayoutPanel)tableLayoutPanel).Controls.Add(label2_Find, 1, 0);
                        ((TableLayoutPanel)tableLayoutPanel).Controls.Add(tToggleSwitch, 1, 1);
                    }

                    tPanelControl.Controls.Add(((TableLayoutPanel)tableLayoutPanel));
                }
                else
                {
                    /// SpeedKriterde yerleştirilmiş olan neseneler iki column kaydırılyor
                    /// 

                    ((TableLayoutPanel)tableLayoutPanel).ColumnCount = ((TableLayoutPanel)tableLayoutPanel).ColumnCount + 2;
                    int i2 = ((TableLayoutPanel)tableLayoutPanel).ColumnCount;

                    for (int i = i2-2; i >= 0; i--)
                    {
                        Control c1 = ((TableLayoutPanel)tableLayoutPanel).GetControlFromPosition(i, 0);
                        Control c2 = ((TableLayoutPanel)tableLayoutPanel).GetControlFromPosition(i, 1);

                        if (c1 != null && c2 != null)
                        {
                            ((TableLayoutPanel)tableLayoutPanel).SetColumn(c1, i + 2);  //SetRow(c2, 0);
                            ((TableLayoutPanel)tableLayoutPanel).SetColumn(c2, i + 2);  //SetRow(c1, 1);
                        }
                    }

                    // arama text i
                    ((TableLayoutPanel)tableLayoutPanel).Controls.Add(label1_Find, 0, 0);
                    ((TableLayoutPanel)tableLayoutPanel).Controls.Add(textEdit_Find, 0, 1);

                    // list && Data ise
                    if (findType == 200)
                    {
                        // arama toggleSwitch
                        ((TableLayoutPanel)tableLayoutPanel).Controls.Add(label2_Find, 1, 0);
                        ((TableLayoutPanel)tableLayoutPanel).Controls.Add(tToggleSwitch, 1, 1);
                    }
                }


                // Eğer Listeden arama aktif ise Tüme datanın dolması gerek
                if (tToggleSwitch.IsOn == false)
                    ev.InData_RunSQL(tForm, TableIPCode, "", null, null);

                /// Search açılırken bir value nin ATANMASI 
                /// yani direk olarak arancak kelimenin açılış sırasında yüklenmesi
                /// 

                if (t.IsNotNull(v.search_VALUE))
                {
                    if ((TableIPCode.IndexOf("3S_MSTBL") > -1) ||
                        (TableIPCode.IndexOf("3S_MSTBLIP") > -1)
                        )
                    {
                        /// bu bölüm biraz özel çözüm oldu
                        /// 
                        string TableCode = string.Empty;
                        string IPCode = string.Empty;
                        t.TableIPCode_Get(v.search_VALUE, ref TableCode, ref IPCode);

                        if (TableIPCode.IndexOf("3S_MSTBL") > -1)
                            textEdit_Find.Text = TableCode;
                        if (TableIPCode.IndexOf("3S_MSTBLIP") > -1)
                            textEdit_Find.Text = IPCode;
                    }
                    else
                    {
                        /// genel aramalar
                        textEdit_Find.Text = v.search_VALUE;
                    }
                    
                }

                #endregion find edit create
            }

        }
        
        public void Create_WinExplorerBands_Add(Control tPanelControl, GridControl tGridControl)
        {
            tEvents ev = new tEvents();

            ImageComboBoxEdit tEdit = new DevExpress.XtraEditors.ImageComboBoxEdit();
            tEdit.Name = "Column_OptionsViewStyles";
            tEdit.Dock = DockStyle.Left;
            //tGridControl.AccessibleName = TableIPCode;
            tEdit.Properties.AccessibleName = tGridControl.AccessibleName;
            
            foreach (WinExplorerViewStyle vs in Enum.GetValues(typeof(WinExplorerViewStyle)))
            {
                DevExpress.XtraEditors.Controls.ImageComboBoxItem item =
                new DevExpress.XtraEditors.Controls.ImageComboBoxItem(vs.ToString(), vs, -1);
                tEdit.Properties.Items.Add(item);
            }

            tEdit.EditValueChanged += new System.EventHandler(ev.barEditItem_WinExplorerViewStyle_EditValueChanged);
            //new System.EventHandler(ev.myForm_Activated);

            //tEdit.EditValue = 

            /*
        if (tEdit_ICB != null)
            tEdit_ICB.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(caption, Convert.ToInt16(value), type));


        foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
        {
            ImageComboBoxItem cbItem = new ImageComboBoxItem(orientation.ToString(), orientation);
            repositoryItemImageComboBox1.Items.Add(cbItem);
        }
        barEditItem2.EditValue = Orientation.Horizontal;
        barEditItem2.EditValueChanged += barEditItem2_EditValueChanged;



        if (!e.Item.Checked)
            return;
        switch (e.Item.Caption)
        {
            case "Extra large icons":
                allowAsyncLoad = true;
                this.winExplorerView1.OptionsView.Style = WinExplorerViewStyle.ExtraLarge;
                break;
            case "Large icons":
                allowAsyncLoad = true;
                this.winExplorerView1.OptionsView.Style = WinExplorerViewStyle.Large;
                break;
            case "Medium icons":
                allowAsyncLoad = true;
                this.winExplorerView1.OptionsView.Style = WinExplorerViewStyle.Medium;
                break;
            case "Small icons":
                this.winExplorerView1.OptionsView.Style = WinExplorerViewStyle.Small;
                break;
            case "List":
                this.winExplorerView1.OptionsView.Style = WinExplorerViewStyle.List;
                break;
            case "Tiles":
                this.winExplorerView1.OptionsView.Style = WinExplorerViewStyle.Tiles;
                break;
            case "Content":
                this.winExplorerView1.OptionsView.Style = WinExplorerViewStyle.Content;
                break;
        }
        UpdateGridOptionsImageLoad(allowAsyncLoad);

        */
            #region Button Paneli

            DevExpress.XtraEditors.PanelControl buttonsPanelControl = new DevExpress.XtraEditors.PanelControl();
            buttonsPanelControl.Name = "tPanel_Navigator_";// + RefID.ToString();
            buttonsPanelControl.Height = 26;
            buttonsPanelControl.Dock = DockStyle.Top;
            buttonsPanelControl.SendToBack();

            buttonsPanelControl.Controls.Add(tEdit);

            #endregion Button Paneli

            tPanelControl.Controls.Add(buttonsPanelControl);
        }

        #region Create_PictureControl

        public void Create_PictureControl(Control tPanelControl,
               string TableName, string FieldName)
        {
             tToolBox t = new tToolBox();

            // DevExpress.XtraLayout.BaseLayoutItem bl_item,
            //DataRow row_Table, DataSet ds_Fields,

             //int Item_Id = 0;
             string Dock_Style = "Top";

            DevExpress.XtraEditors.PanelControl tPanelControl_Picture0 = new DevExpress.XtraEditors.PanelControl();
            DevExpress.XtraEditors.PictureEdit  tPictureEdit = new DevExpress.XtraEditors.PictureEdit();
            DevExpress.XtraEditors.PanelControl tPanelControl_Picture1 = new DevExpress.XtraEditors.PanelControl();
            DevExpress.XtraEditors.PanelControl tPanelControl_Picture2 = new DevExpress.XtraEditors.PanelControl();

            DevExpress.XtraEditors.SimpleButton tSimpleButton_Delete = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton tSimpleButton_Load = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton tSimpleButton_SaveAs = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton tSimpleButton_Save = new DevExpress.XtraEditors.SimpleButton();

            DevExpress.XtraEditors.SimpleButton tSimpleButton_Full_Size = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton tSimpleButton_Fit = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.ZoomTrackBarControl tZoomTrackBarControl = new DevExpress.XtraEditors.ZoomTrackBarControl();

            // 
            // tPictureEdit
            // 
            tPictureEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            tPictureEdit.Location = new System.Drawing.Point(10, 29);
            //tPictureEdit.MenuManager = this.barManager1;
            tPictureEdit.Name = "tPictureEdit_" + FieldName;
            tPictureEdit.Size = new System.Drawing.Size(177, 165);
            tPictureEdit.TabIndex = 0;
            tPictureEdit.Properties.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.True;
            tPictureEdit.Properties.ShowScrollBars = true;
            tPictureEdit.AccessibleName = FieldName;
            // 
            // tSimpleButton_Delete
            // 
            tSimpleButton_Delete.Dock = System.Windows.Forms.DockStyle.Left;
            //tSimpleButton_Delete.Location = new System.Drawing.Point(47, 2);
            tSimpleButton_Delete.Name = "tSimpleButton_Delete";
            tSimpleButton_Delete.Size = new System.Drawing.Size(32, 23);
            tSimpleButton_Delete.TabIndex = 3;
            tSimpleButton_Delete.Text = "Sil";
            tSimpleButton_Delete.AccessibleName = FieldName;
            // 
            // tSimpleButton_Load
            // 
            tSimpleButton_Load.Dock = System.Windows.Forms.DockStyle.Left;
            //tSimpleButton_Load.Location = new System.Drawing.Point(79, 2);
            tSimpleButton_Load.Name = "tSimpleButton_Load";
            tSimpleButton_Load.Size = new System.Drawing.Size(32, 23);
            tSimpleButton_Load.TabIndex = 2;
            tSimpleButton_Load.Text = "Yükle";
            tSimpleButton_Load.AccessibleName = FieldName;
            //tSimpleButton_Load.Click += new System.EventHandler(DevExpCC.btn_Picture_Load_Click);
            // 
            // tSimpleButton_SaveAs
            // 
            tSimpleButton_SaveAs.Dock = System.Windows.Forms.DockStyle.Left;
            //tSimpleButton_SaveAs.Location = new System.Drawing.Point(111, 2);
            tSimpleButton_SaveAs.Name = "tSimpleButton_SaveAs";
            tSimpleButton_SaveAs.Size = new System.Drawing.Size(32, 23);
            tSimpleButton_SaveAs.TabIndex = 1;
            tSimpleButton_SaveAs.Text = "Farklı Kaydet";
            tSimpleButton_SaveAs.AccessibleName = FieldName;
            //tSimpleButton_SaveAs.Click += new System.EventHandler(DevExpCC.btn_Picture_SaveAs_Click);
            // 
            // tSimpleButton_Save
            // 
            tSimpleButton_Save.Dock = System.Windows.Forms.DockStyle.Fill;
            //tSimpleButton_Save.Location = new System.Drawing.Point(143, 2);
            tSimpleButton_Save.Name = "tSimpleButton_Save";
            tSimpleButton_Save.Size = new System.Drawing.Size(32, 23);
            tSimpleButton_Save.TabIndex = 0;
            tSimpleButton_Save.Text = "Kaydet";
            tSimpleButton_Save.AccessibleName = FieldName;
            // 
            // tPanelControl_Picture1
            // 
            //defaultToolTipController1.SetAllowHtmlText(this.tPanelControl_Picture, DevExpress.Utils.DefaultBoolean.Default);
            tPanelControl_Picture1.Controls.Add(tSimpleButton_Save);
            tPanelControl_Picture1.Controls.Add(tSimpleButton_SaveAs);
            tPanelControl_Picture1.Controls.Add(tSimpleButton_Load);
            tPanelControl_Picture1.Controls.Add(tSimpleButton_Delete);
            tPanelControl_Picture1.Dock = System.Windows.Forms.DockStyle.Bottom;
            tPanelControl_Picture1.Location = new System.Drawing.Point(10, 194);
            tPanelControl_Picture1.Name = "tPanelControl_Picture1";
            tPanelControl_Picture1.Padding = new System.Windows.Forms.Padding(2);
            tPanelControl_Picture1.Size = new System.Drawing.Size(177, 27);
            tPanelControl_Picture1.TabIndex = 0;

            // 
            // tZoomTrackBarControl
            // 
            tZoomTrackBarControl.Dock = System.Windows.Forms.DockStyle.Fill;
            tZoomTrackBarControl.EditValue = 100;
            tZoomTrackBarControl.Value = 100;
            tZoomTrackBarControl.Location = new System.Drawing.Point(68, 4);
            //tZoomTrackBarControl.MenuManager = this.barManager1;
            tZoomTrackBarControl.Name = "tZoomTrackBarControl_" + FieldName;
            tZoomTrackBarControl.Properties.LargeChange = 25;
            tZoomTrackBarControl.Properties.Maximum = 400;
            tZoomTrackBarControl.Properties.Minimum = 10;
            tZoomTrackBarControl.Value = 100;
            tZoomTrackBarControl.Properties.ScrollThumbStyle = DevExpress.XtraEditors.Repository.ScrollThumbStyle.ArrowDownRight;
            //tZoomTrackBarControl.Properties.ValueChanged += new System.EventHandler(tZoomTrackBarControl_Properties_ValueChanged);
            tZoomTrackBarControl.Properties.SmallChange = 10;
            tZoomTrackBarControl.Size = new System.Drawing.Size(105, 16);
            tZoomTrackBarControl.AccessibleName = FieldName;
            tZoomTrackBarControl.TabIndex = 2;
            // 
            // tSimpleButton_Fit
            // 
            tSimpleButton_Fit.Dock = System.Windows.Forms.DockStyle.Left;
            tSimpleButton_Fit.Location = new System.Drawing.Point(36, 4);
            tSimpleButton_Fit.Name = "tSimpleButton_Fit";
            tSimpleButton_Fit.Size = new System.Drawing.Size(32, 16);
            tSimpleButton_Fit.TabIndex = 1;
            tSimpleButton_Fit.Text = "Fit";
            tSimpleButton_Fit.AccessibleName = FieldName;
            //tSimpleButton_Fit.Click += new System.EventHandler(DevExpCC.btn_Picture_Fit_Click);
            // 
            // tSimpleButton_Full_Size
            // 
            tSimpleButton_Full_Size.Dock = System.Windows.Forms.DockStyle.Left;
            tSimpleButton_Full_Size.Location = new System.Drawing.Point(4, 4);
            tSimpleButton_Full_Size.Name = "tSimpleButton_Full_Size";
            tSimpleButton_Full_Size.Size = new System.Drawing.Size(32, 16);
            tSimpleButton_Full_Size.TabIndex = 0;
            tSimpleButton_Full_Size.Text = "Full";
            tSimpleButton_Full_Size.AccessibleName = FieldName;
            //tSimpleButton_Full_Size.Click += new System.EventHandler(DevExpCC.btn_Picture_Full_Click);
            // 
            // tPanelControl_Picture2
            // 
            //defaultToolTipController1.SetAllowHtmlText(this.tPanelControl_Picture, DevExpress.Utils.DefaultBoolean.Default);
            tPanelControl_Picture2.Controls.Add(tZoomTrackBarControl);
            tPanelControl_Picture2.Controls.Add(tSimpleButton_Fit);
            tPanelControl_Picture2.Controls.Add(tSimpleButton_Full_Size);
            tPanelControl_Picture2.Dock = System.Windows.Forms.DockStyle.Bottom;
            tPanelControl_Picture2.Location = new System.Drawing.Point(10, 194);
            tPanelControl_Picture2.Name = "tPanelControl_Picture2";
            tPanelControl_Picture2.Padding = new System.Windows.Forms.Padding(2);
            tPanelControl_Picture2.Size = new System.Drawing.Size(177, 27);
            tPanelControl_Picture2.TabIndex = 1;
            // 
            // tGroupControl_Picture
            // 
            //defaultToolTipController1.SetAllowHtmlText(this.tGroupControl_Picture, DevExpress.Utils.DefaultBoolean.Default);
            tPanelControl_Picture0.Controls.Add(tPictureEdit);
            tPanelControl_Picture0.Controls.Add(tPanelControl_Picture2);
            tPanelControl_Picture0.Controls.Add(tPanelControl_Picture1);
            tPanelControl_Picture0.Location = new System.Drawing.Point(8, 16);
            tPanelControl_Picture0.Name = "tPanelControl_Picture0";
            tPanelControl_Picture0.Padding = new System.Windows.Forms.Padding(8);
            tPanelControl_Picture0.Size = new System.Drawing.Size(197, 251);
            tPanelControl_Picture0.TabIndex = 42;


            if (Dock_Style != string.Empty)
            {
                if (Dock_Style == "Bottom") tPanelControl_Picture0.Dock = DockStyle.Bottom;
                if (Dock_Style == "Fill") tPanelControl_Picture0.Dock = DockStyle.Fill;
                if (Dock_Style == "Left") tPanelControl_Picture0.Dock = DockStyle.Left;
                if (Dock_Style == "None") tPanelControl_Picture0.Dock = DockStyle.None;
                if (Dock_Style == "Right") tPanelControl_Picture0.Dock = DockStyle.Right;
                if (Dock_Style == "Top") tPanelControl_Picture0.Dock = DockStyle.Top;
            }

            if (tPanelControl != null)
                tPanelControl.Controls.Add(tPanelControl_Picture0);

            //if (bl_item != null)
            //    ((LayoutControlGroup)bl_item).

            /*
            if (Item_Id == sp.fr_GroupControl)
            {
                ((GroupControl)sender).Controls.Add(tPanelControl_Picture0);
            }

            if (Item_Id == sp.fr_PanelControl)
            {
                ((PanelControl)sender).Controls.Add(tPanelControl_Picture0);
            }

            if (Item_Id == sp.fr_TabPage)
            {
                ((TabPage)sender).Controls.Add(tPanelControl_Picture0);
            }

            if (Item_Id == sp.fr_DockContainer)
            {
                ((DockPanel)sender).Controls.Add(tPanelControl_Picture0);
            }

            if (Item_Id == sp.fr_DockPanel)
            {
                ((DockPanel)sender).Controls.Add(tPanelControl_Picture0);
            }
            */
        }

        
        #endregion Create_PictureControl

        #region Create_Parametre

        public string Create_Param_Form(DataSet dsKrtr, string TableName, Boolean tKisitli, int tTableType)
        {
            Form tForm = Create_Form(TableName, "Parametre", "");

            DevExpress.XtraEditors.PanelControl panelControl1 = new DevExpress.XtraEditors.PanelControl();
            // 
            // panelControl
            // 
            panelControl1.Name = "panelControl1";
            panelControl1.Dock = DockStyle.Fill;
            panelControl1.TabIndex = 0;
            panelControl1.Width = 225;
            panelControl1.Visible = true;

            tForm.Controls.Add(panelControl1);

            //int i = t.myInt32(Width);
            //if (i <= 100) i = 550;

            tForm.Tag = "DIALOG";
            tForm.Width = 400;//i;
            tForm.Height = 600;//i;

            return Create_Param_Panel(tForm, panelControl1, dsKrtr, TableName, tKisitli, tTableType);

        }

        public string Create_Param_Panel(Form tForm, Control panelControl1, DataSet dsKrtr, 
            string TableName, Boolean tKisitli, int tTableType)
        {
            tToolBox t = new tToolBox();
            tDevView dv = new tDevView();
            tEvents ev = new tEvents();

            string function_name = "Create_Param";
            t.Takipci(function_name, "", '{');
            
            #region VievControl Create
            
            #region tDataControl_Krtr_Fields_

            // Sorgular için tablonun field listesi ve fieldlere ait bilgiler okunuyor

            // Sorgu için gerekeli olan bilgiler dsKrtr üzerinde bulunmaktadır
            // bu bilgilere daha sonra runtime sırasında da gerek duyulmakta
            // bu DataSet e ulaşmak için başka bir işe yaramayan bir control oluşturulmakta ve
            // bu controle bağlanmak, runtime sırasında önce bu controle onun sayesinde de bu DataSet e 
            // tekrar ulaşılmakta
            
            DevExpress.XtraDataLayout.DataLayoutControl DataLControl1 = new DevExpress.XtraDataLayout.DataLayoutControl();
            DataLControl1.AccessibleName = TableName;
            DataLControl1.Name = "tDataControl_Krtr_" + TableName;
            DataLControl1.Visible = false;

            if (dsKrtr != null)
                DataLControl1.DataSource = dsKrtr.Tables[0];
            
            #endregion tDataControl_Krtr_Fields_

            Control cntrl = new Control();
            cntrl = Create_View_(v.obj_vw_VGridMulti, 0, TableName, "");

            ((VGridControl)cntrl).BeginUpdate();
            ((VGridControl)cntrl).AccessibleName = "tPARAMS_" + TableName;
            ((VGridControl)cntrl).Tag = tTableType;
            ((VGridControl)cntrl).KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);
            ((VGridControl)cntrl).KeyUp += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyUp);
            ((VGridControl)cntrl).KeyPress += new System.Windows.Forms.KeyPressEventHandler(ev.myVGridControl_Kriter_KeyPress);
            // RunTime Sırasında yapılacak işlerin listesi
            //if (t.IsNotNull(Prop_Runtime))
            //{
            //    cntrl.AccessibleDescription = Prop_Runtime;
            //}

            panelControl1.Controls.Add(DataLControl1);
            panelControl1.Controls.Add(cntrl);

            Create_Param_Columns((VGridControl)cntrl, dsKrtr, TableName, tKisitli, tTableType);

            Create_Param_Buttons((VGridControl)cntrl, TableName, tKisitli);
            
            ((VGridControl)cntrl).EndUpdate();

            
            #endregion VievControl Create

            #region dsParamValue Create

            
            DataSet dsParamValue = new DataSet();
            string xsql = t.RSQL_ParamFields(dsKrtr, tTableType);

            #region örnek
            /*
             * tTableType == 1 için şeklinde sql geliyor
             * 
              select t1.* , t2.* , t3.*
              from KURSIYER t1, ADRES t2, KIMLIK t3
              where t1.ID = -1  
              and t2.ID = -1  
              and t3.ID = -1 ";
             *
             * tTableType == 6 için şeklinde sql geliyor
             * 
              declare @PART_ID int
              declare @PART_NAME varchar(20)
              declare @BAS_TARIH smalldatetime
              declare @GIRIS_YOK int
              declare @GIRIS_YAPTI int

             Select 
                @PART_ID PART_ID
              , @PART_NAME PART_NAME
              , @BAS_TARIH BAS_TARIH
              , @GIRIS_YOK GIRIS_YOK
              , @GIRIS_YAPTI GIRIS_YAPTI
             * 
             */
            #endregion örnek
            
            t.Data_Read_Execute(dsParamValue, ref xsql, TableName + "_ParamValue", null);

            if (tTableType == 1) // Table ise
            {
                DataRow str1 = dsParamValue.Tables[0].NewRow();
                DataRow str2 = dsParamValue.Tables[0].NewRow();

                dsParamValue.Tables[0].Rows.Add(str1);
                dsParamValue.Tables[0].Rows.Add(str2);    
            }

            if (tTableType == 6) // Select/TableIPCode ise
            {
                DataRow str1 = dsParamValue.Tables[0].NewRow();
                //DataRow str2 = dsParamValue.Tables[0].NewRow();

                dsParamValue.Tables[0].Rows.Add(str1);
                //dsParamValue.Tables[0].Rows.Add(str2);
            }

            ((VGridControl)cntrl).DataSource = dsParamValue.Tables[0];

            string newKriterler = string.Empty;
            string ftype = string.Empty;

            if (tForm.Tag != null)
                ftype = tForm.Tag.ToString();

            if (ftype == "DIALOG")
            {
                tFormView(tForm, 4, 1);
                newKriterler = tForm.AccessibleDefaultActionDescription;
            }
                        
            #endregion dsWhere

            t.Takipci(function_name, "", '}');

            return newKriterler;

        }

        public void Create_Param_Columns(VGridControl tVGrid, DataSet dsKrtr,
                     string TableName, Boolean Kist, int tTableType)
        {
            tToolBox t = new tToolBox();
            string function_name = "Create_Param_Columns";
            t.Takipci(function_name, "", '{');

            //tDefaultValue df = new tDefaultValue();

            string tTableName = string.Empty;
            string FindTable = "_FULL";

            if (Kist) FindTable = "_KIST"; 
            
            #region fields list
            /*
            ,[TABLE_CODE]
      ,[FIELD_NO]
      ,[FIELD_NAME]
      ,[FIELD_TYPE]
      ,[FIELD_LENGTH]
      ,[FAUTOINC]
      ,[FNOTNULL]
      ,[FREADONLY]
      ,[FENABLED]
      ,[FVISIBLE]
      ,[FINDEX]
      ,[FPICTURE]
      ,[FCAPTION]
      ,[FHINT]
      ,[DEFAULT_TYPE]
      ,[DEFAULT_NUMERIC]
      ,[DEFAULT_TEXT]
      ,[DEFAULT_INT]
      ,[DEFAULT_SP]
      ,[DEFAULT_SETUP]
      ,[CMP_COLUMN_TYPE]
      ,[CMP_WIDTH]
      ,[CMP_DISPLAY_FORMAT]
      ,[VALIDATION_OPERATOR]
      ,[VALIDATION_VALUE1]
      ,[VALIDATION_VALUE2]
      ,[VALIDATION_ERRORTEXT]
      ,[VALIDATION_ERRORTYPE]
      ,[MASTER_TABLEIPCODE]
      ,[SEARCH_TABLEIPCODE]
      ,[MASTER_KEY_FNAME]
      ,[LIST_TYPES_NAME]
      ,[GROUP_NO]
      ,[GROUP_LINE_NO]
      ,[FJOIN_TYPE]
      ,[FJOIN_TABLE_NAME]
      ,[FJOIN_TABLE_ALIAS]
      ,[FJOIN_KEY_FNAME]
      ,[FJOIN_WHERE]
      ,[KRT_LINE_NO]
      ,[KRT_CAPTION]
      ,[KRT_OPERAND_TYPE]
      ,[KRT_LIKE]
      ,[KRT_DEFAULT1]
      ,[KRT_DEFAULT2]
      ,[KRT_ALIAS]
      ,[KRT_TABLE_ALIAS]

            */
            #endregion fields list

            int i2 = dsKrtr.Tables.Count;
            int TableNo = 0;

            for (int i = 0; i < i2; i++)
            {
                tTableName = dsKrtr.Tables[i].TableName.ToString();
                
                if (tTableName.IndexOf(FindTable) > 0)
                {
                    // Kısıtlı liste yok ise full hazırla
                    if ((Kist) &&
                        (dsKrtr.Tables[tTableName].Rows.Count == 0))
                    {
                        Kist = false;
                        FindTable = "_FULL";
                    }

                    Create_Param_Columns_(tVGrid, dsKrtr, tTableName, TableNo, Kist, tTableType);
                    TableNo++;
                    
                    
                }

            } // for (int i

            t.Takipci(function_name, "", '}');
        }


        private void Create_Param_Columns_(VGridControl tVGrid, DataSet dsKrtr,
                     string TableName, int TableNo, Boolean Kist, int tTableType)
        {
            tDevColumn dc = new tDevColumn();
            
            string s = string.Empty;
            string tTableName = string.Empty;
            string tTableCode = string.Empty;
            string tRowFName = string.Empty;
            string tKrtFName = string.Empty;
            string tRowCaption = string.Empty;
            string tColumnType = string.Empty;
            string tTypeName = string.Empty;
            string tValue = string.Empty;
            string paket_basi = string.Empty;
            string paket_sonu = string.Empty;
            string TableCaption = string.Empty;

            //byte tRowType = 0;
            int tCategoryNo = 0;
            //byte tRowLineNo = 0;
            Int16 tRowFieldType = 0;
                        
            TableCaption = t.Set(dsKrtr.Tables[TableName].Namespace.ToString(), TableName, ":.");
            CategoryRow CatRow = new CategoryRow(TableCaption);
            CatRow.Name = "CategoryRow_" + TableNo.ToString();

            tVGrid.Rows.Add(CatRow);

            foreach (DataRow Row in dsKrtr.Tables[TableName].Rows)
            {
                tCategoryNo = TableNo; 
                tTableName = TableName; //t.Set(Row["TABLENAME"].ToString(), "", "null");
                                
                    
                if (tTableType == 1) // Table ise
                {
                    tTableCode = t.Set(Row["KRT_ALIAS"].ToString(), Row["KRT_TABLE_ALIAS"].ToString(), Row["TABLE_CODE"].ToString());
                    tRowFName = t.Set(Row["FIELD_NAME"].ToString(), "", "null");
                    tKrtFName = t.Set(Row["KRT_CAPTION"].ToString(), Row["FIELD_NAME"].ToString(), "");
                    tRowCaption = t.Set(Row["FCAPTION"].ToString(), "", tRowFName);
                    tRowFieldType = t.Set(Row["FIELD_TYPE"].ToString(), "", (Int16)167);
                    tColumnType = t.Set(Row["CMP_COLUMN_TYPE"].ToString(), "", "TextEdit");
                    tTypeName = t.Set(Row["LIST_TYPES_NAME"].ToString(), "", "");
                }

                if (tTableType == 6) // TableIPCode ise
                {
                    tTableCode = t.Set(Row["LKP_KRT_ALIAS"].ToString(), Row["LKP_KRT_TABLE_ALIAS"].ToString(), "");

                    if (t.IsNotNull(tTableCode) == false)
                        tTableCode = t.Set(Row["KRT_ALIAS"].ToString(), Row["KRT_TABLE_ALIAS"].ToString(), Row["TABLE_CODE"].ToString()); 
                    
                    tRowFName = t.Set(Row["LKP_FIELD_NAME"].ToString(), "", "null");
                    tKrtFName = t.Set(Row["KRT_CAPTION"].ToString(), Row["LKP_KRT_CAPTION"].ToString(), Row["LKP_FIELD_NAME"].ToString());
                    tRowCaption = t.Set(Row["FCAPTION"].ToString(), Row["LKP_FCAPTION"].ToString(), tRowFName);
                    tRowFieldType = t.Set(Row["LKP_FIELD_TYPE"].ToString(), "", (Int16)167);
                    tColumnType = t.Set(Row["CMP_COLUMN_TYPE"].ToString(), Row["LKP_CMP_COLUMN_TYPE"].ToString(), "TextEdit");
                    tTypeName = t.Set(Row["LIST_TYPES_NAME"].ToString(), Row["LKP_LIST_TYPES_NAME"].ToString(), "");
                }
                
                                
                //if (tRowFName.IndexOf("LKP_") == -1)
                //{
                    EditorRow EditRow = new EditorRow("EditRow_" + tRowFName);
                    EditRow.Properties.Caption = tRowCaption;
                    EditRow.Properties.FieldName = tRowFName;
                    EditRow.Tag = tRowFieldType;
                    EditRow.Properties.Value = tValue;
                    if (tTableCode.IndexOf("[") == -1)
                         EditRow.Name = "[" + tTableCode + "]." + tKrtFName;
                    else EditRow.Name = tTableCode + "." + tKrtFName;

                    //if (tRowFName == "AD")
                    //{
                    //    EditRow.Properties.UnboundType = DevExpress.Data.UnboundColumnType.String;
                    //    EditRow.Properties.UnboundExpression = "[AD] == ':.'";

                    //}
                    
                    s = string.Empty;
                    t.MyProperties_Set(ref s, "TableName", tTableName);
                    t.MyProperties_Set(ref s, "FieldName", tRowFName);
                    t.MyProperties_Set(ref s, "ColumnType", tColumnType);
                    t.MyProperties_Set(ref s, "TypeName", tTypeName);
                    t.MyProperties_Set(ref s, "Width", "0");

                    dc.VGrid_ColumnEdit_(Row, EditRow, s, "", 1); // Tumu = hayır

                    tVGrid.Rows["CategoryRow_" + tCategoryNo.ToString()].ChildRows.Add(EditRow);
                //}
            }
        }

        private void Create_Param_Buttons(VGridControl tVGrid, string TableIPCode, Boolean Kist)
        {
            tEvents ev = new tEvents();

            
            DevExpress.XtraEditors.GroupControl groupControl_ParamButtons = new DevExpress.XtraEditors.GroupControl();
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Param = new System.Windows.Forms.TableLayoutPanel();

            DevExpress.XtraEditors.SimpleButton simpleButton_Temizle = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Collapse = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Expand = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Type = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Tamam = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Close = new DevExpress.XtraEditors.SimpleButton();

            Control cntrl = tVGrid.Parent;
            cntrl.Controls.Add(groupControl_ParamButtons);

            //this.SuspendLayout();
            //
            // groupControl_PropertiesButtons
            // 
            groupControl_ParamButtons.Controls.Add(tableLayoutPanel_Param);
            groupControl_ParamButtons.Name = "groupControl_ParamButtons";
            groupControl_ParamButtons.Size = new System.Drawing.Size(293, 52);
            groupControl_ParamButtons.TabIndex = 1;
            groupControl_ParamButtons.Text = "İşlemler";
            groupControl_ParamButtons.Dock = DockStyle.Bottom;
            // 
            // tableLayoutPanel_Properties
            // 
            //tableLayoutPanel_Param.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            tableLayoutPanel_Param.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            tableLayoutPanel_Param.ColumnCount = 6;
            tableLayoutPanel_Param.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel_Param.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel_Param.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel_Param.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel_Param.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel_Param.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel_Param.Controls.Add(simpleButton_Temizle, 0, 0);
            tableLayoutPanel_Param.Controls.Add(simpleButton_Collapse, 1, 0);
            tableLayoutPanel_Param.Controls.Add(simpleButton_Expand, 2, 0);
            tableLayoutPanel_Param.Controls.Add(simpleButton_Type, 3, 0);
            tableLayoutPanel_Param.Controls.Add(simpleButton_Tamam, 4, 0);
            tableLayoutPanel_Param.Controls.Add(simpleButton_Close, 5, 0);

            tableLayoutPanel_Param.Location = new System.Drawing.Point(2, 21);
            tableLayoutPanel_Param.Name = "tableLayoutPanel_Param";
            tableLayoutPanel_Param.RowCount = 1;
            tableLayoutPanel_Param.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel_Param.TabIndex = 0;
            tableLayoutPanel_Param.Dock = System.Windows.Forms.DockStyle.Fill;

            // 
            // simpleButton_Close
            // 
            simpleButton_Close.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Close.Location = new System.Drawing.Point(189, 6);
            simpleButton_Close.Name = "simpleButton_Close";
            simpleButton_Close.Size = new System.Drawing.Size(94, 23);
            simpleButton_Close.TabIndex = 1;
            simpleButton_Close.Text = "&Çıkış";

            simpleButton_Close.Click += new System.EventHandler(ev.btn_Param_Click);
            simpleButton_Close.AccessibleName = TableIPCode;
            simpleButton_Close.AccessibleDescription = tVGrid.Name.ToString();
            
            // 
            // simpleButton_Tamam
            // 
            simpleButton_Tamam.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Tamam.Location = new System.Drawing.Point(189, 6);
            simpleButton_Tamam.Name = "simpleButton_Tamam";
            simpleButton_Tamam.Size = new System.Drawing.Size(94, 23);
            simpleButton_Tamam.TabIndex = 1;
            simpleButton_Tamam.Text = "&Listele";

            simpleButton_Tamam.Click += new System.EventHandler(ev.btn_Param_Click);
            simpleButton_Tamam.AccessibleName = TableIPCode;
            simpleButton_Tamam.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Type
            // 
            simpleButton_Type.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Type.Location = new System.Drawing.Point(189, 6);
            simpleButton_Type.Name = "simpleButton_Type";
            simpleButton_Type.Size = new System.Drawing.Size(94, 23);
            simpleButton_Type.TabIndex = 2;
            if (Kist) simpleButton_Type.Text = "Daha fazla";
            else simpleButton_Type.Text = "Kısıtlı";

            simpleButton_Type.Click += new System.EventHandler(ev.btn_Param_Click);
            simpleButton_Type.AccessibleName = TableIPCode;
            simpleButton_Type.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Temizle
            // 
            simpleButton_Temizle.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Temizle.Location = new System.Drawing.Point(6, 6);
            simpleButton_Temizle.Name = "simpleButton_Clear";
            simpleButton_Temizle.Size = new System.Drawing.Size(93, 23);
            simpleButton_Temizle.TabIndex = 3;
            simpleButton_Temizle.Text = "&Temizle";

            simpleButton_Temizle.Click += new System.EventHandler(ev.btn_Param_Click);
            simpleButton_Temizle.AccessibleName = TableIPCode;
            simpleButton_Temizle.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Collapse
            // 
            simpleButton_Collapse.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Collapse.Location = new System.Drawing.Point(105, 6);
            simpleButton_Collapse.Name = "simpleButton_Collapse";
            simpleButton_Collapse.Size = new System.Drawing.Size(36, 23);
            simpleButton_Collapse.TabIndex = 4;
            simpleButton_Collapse.Text = "Kapat";

            simpleButton_Collapse.Click += new System.EventHandler(ev.btn_Param_Click);
            simpleButton_Collapse.AccessibleName = TableIPCode;
            simpleButton_Collapse.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Expand
            // 
            simpleButton_Expand.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Expand.Location = new System.Drawing.Point(147, 6);
            simpleButton_Expand.Name = "simpleButton_Expand";
            simpleButton_Expand.Size = new System.Drawing.Size(36, 24);
            simpleButton_Expand.TabIndex = 5;
            simpleButton_Expand.Text = "Aç";

            simpleButton_Expand.Click += new System.EventHandler(ev.btn_Param_Click);
            simpleButton_Expand.AccessibleName = TableIPCode;
            simpleButton_Expand.AccessibleDescription = tVGrid.Name.ToString();

        }


        #endregion Create_Parametre

        #region Create_Delete_Function

        public DialogResult Create_Delete_Form(DataSet dsData, string TableIPCode)
        {
            
            string TableName = "DELETE";

            Form tForm = Create_Form(TableName, "Kayıt Silme Onayı", "");

            DevExpress.XtraEditors.PanelControl panelControl1 = new DevExpress.XtraEditors.PanelControl();
            DevExpress.XtraEditors.PanelControl panelControl2 = new DevExpress.XtraEditors.PanelControl();
            // 
            // panelControl
            // 
            panelControl1.Name = "panelControl1";
            panelControl1.Dock = DockStyle.Fill;
            panelControl1.TabIndex = 0;
            panelControl1.Width = 225;
            panelControl1.Visible = true;

            panelControl2.Name = "panelControl2";
            panelControl2.Dock = DockStyle.Bottom;
            panelControl2.TabIndex = 0;
            panelControl2.Height = 80;
            panelControl2.Visible = true;

            // 
            // labelControl1
            // 
            DevExpress.XtraEditors.LabelControl labelControl1 = new DevExpress.XtraEditors.LabelControl();
            labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            labelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            labelControl1.LineColor = System.Drawing.SystemColors.GradientActiveCaption;
            labelControl1.LineLocation = DevExpress.XtraEditors.LineLocation.Bottom;
            labelControl1.LineVisible = true;
            labelControl1.Name = "labelControl1";
            labelControl1.Padding = new System.Windows.Forms.Padding(4);
            labelControl1.Size = new System.Drawing.Size(298, 33);
            labelControl1.TabIndex = 0;
            labelControl1.Dock = DockStyle.Top;
            labelControl1.Text = "Kayıt silinecek onaylıyor musunuz ?";

            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            DevExpress.XtraEditors.SimpleButton simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton2 = new DevExpress.XtraEditors.SimpleButton();

            // 
            // tableLayoutPanel1
            //
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.71429F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.71429F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.71429F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.71429F));
            tableLayoutPanel1.Controls.Add(simpleButton1, 3, 1);
            tableLayoutPanel1.Controls.Add(simpleButton2, 1, 1);
            //tableLayoutPanel1.Location = new System.Drawing.Point(524, 260);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.Size = new System.Drawing.Size(295, 44);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // simpleButton1
            // 
            simpleButton1.DialogResult = System.Windows.Forms.DialogResult.No;
            simpleButton1.Dock = System.Windows.Forms.DockStyle.Fill;
            //simpleButton1.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.Image")));
            simpleButton1.Location = new System.Drawing.Point(160, 11);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new System.Drawing.Size(99, 20);
            simpleButton1.TabIndex = 0;
            simpleButton1.Text = "Hayır";
            // 
            // simpleButton2
            // 
            simpleButton2.DialogResult = System.Windows.Forms.DialogResult.Yes;
            simpleButton2.Dock = System.Windows.Forms.DockStyle.Fill;
            //simpleButton2.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton2.Image")));
            simpleButton2.Location = new System.Drawing.Point(34, 11);
            simpleButton2.Name = "simpleButton2";
            simpleButton2.Size = new System.Drawing.Size(99, 20);
            simpleButton2.TabIndex = 1;
            simpleButton2.Text = "Evet";
            
            panelControl2.Controls.Add(tableLayoutPanel1);
            panelControl2.Controls.Add(labelControl1);
            
            // Define the border style of the form to a dialog box.
            tForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            // Set the accept button of the form to button1.
            tForm.AcceptButton = simpleButton2;
            // Set the cancel button of the form to button2.
            tForm.CancelButton = simpleButton1;

            tForm.Controls.Add(panelControl2);
            tForm.Controls.Add(panelControl1);

            
            Control cntrl = new Control();
            cntrl = Create_View_(v.obj_vw_VGridSingle, 0, TableName, "");

            DataSet ds_Table = new DataSet();
            DataSet ds_Fields = new DataSet();

            tTablesRead tr = new tTablesRead();

            tr.MS_Tables_IP_Read(ds_Table, TableIPCode);
            tr.MS_Fields_IP_Read(ds_Fields, TableIPCode);

            DataRow row_Table = ds_Table.Tables[0].Rows[0] as DataRow;

            tDevView dv = new tDevView();
            dv.tVGrid_Create(row_Table, ds_Fields, dsData, (DevExpress.XtraVerticalGrid.VGridControl)cntrl);

            //((VGridControl)cntrl).DataSource = dsData;
            //((VGridControl)cntrl)

            panelControl1.Controls.Add(cntrl);
                     
                       

            tForm.Tag = "DIALOG";
            tForm.Width = 500;//i;
            tForm.Height = 400;//i;

            tFormView(tForm, 4, 1);
            DialogResult cevap = tForm.DialogResult;
            
            return cevap;
        }

        #endregion Create_Delete_Function

        public void Create_AlertForm()
        {
            /*
            using DevExpress.XtraBars.Alerter;

            // Create a regular custom button.
            AlertButton btn1 = new AlertButton(Image.FromFile(@"c:\folder-16x16.png"));
            btn1.Hint = "Open file";
            btn1.Name = "buttonOpen";
            // Create a check custom button.
            AlertButton btn2 = new AlertButton(Image.FromFile(@"c:\clock-16x16.png"));
            btn2.Style = AlertButtonStyle.CheckButton;
            btn2.Down = true;
            btn2.Hint = "Alert On";
            btn2.Name = "buttonAlert";
            // Add buttons to the AlertControl and subscribe to the events to process button clicks
            alertControl1.Buttons.Add(btn1);
            alertControl1.Buttons.Add(btn2);
            alertControl1.ButtonClick += new AlertButtonClickEventHandler(alertControl1_ButtonClick);
            alertControl1.ButtonDownChanged +=
                new AlertButtonDownChangedEventHandler(alertControl1_ButtonDownChanged);

            // Show a sample alert window.
            AlertInfo info = new AlertInfo("New Window", "Text");
            alertControl1.Show(this, info);

            void alertControl1_ButtonDownChanged(object sender,
            AlertButtonDownChangedEventArgs e) {
                if (e.ButtonName == "buttonOpen")
                {
        //...
    }
            }

void alertControl1_ButtonClick(object sender, AlertButtonClickEventArgs e) {
    if (e.ButtonName == "buttonAlert")
    {
        //...
    }
}

            */
        }
    }
}
