
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using Tkn_CreateObject;
using Tkn_DefaultValue;
using Tkn_DevColumn;
using Tkn_DevView;
using Tkn_Events;
using Tkn_SQLs;
using Tkn_TablesRead;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_InputPanel
{
    public class tInputPanel : tBase
    {
        #region Create InputPanel

        public void Create_InputPanel(
             Form tForm,
             string tPanelControlName,
             string TableIPCode,
             byte IP_View_Type,
             bool isCloseWaitForm)
        {
            tToolBox t = new tToolBox();

            Control cntrl = null;
            //string[] controls = new string[] { "" };
            cntrl = t.Find_Control(tForm, tPanelControlName); //, "", controls);

            if (cntrl != null)
            {
                Create_InputPanel(tForm, cntrl, TableIPCode, "", IP_View_Type, isCloseWaitForm);
            }
        }

        public void Create_InputPanel(
            Form tForm,
            Control tPanelControl,
            string TableIPCode,
            byte IP_View_Type,
            bool isCloseWaitForm)
        {
            Create_InputPanel(tForm, tPanelControl, TableIPCode, "", IP_View_Type, isCloseWaitForm);
        }

        public void Create_InputPanel(
            Form tForm,
            Control tPanelControl,
            string TableIPCode,
            string MultiPageID,
            byte IP_View_Type,
            bool isCloseWaitForm)
        {
            if (tPanelControl.Controls.Count >= 2) return;

            tToolBox t = new tToolBox();
            string function_name = "Create_InputPanel";
            t.Takipci(function_name, "", '{');

            //MessageBox.Show("InputPanel");

            /* InputPanelin yapısı
             * -----------------------------------------------------------------------------------------
             * PanelControl          ( tPanelControl aslında diğer tüm controlleride temsil etmektedir )
             * |__ ViewControl       ( GridConrol )
             *     |__ ViewObject    ( GridView   )
             */

            //v.Kullaniciya_Mesaj_Var = "IP hazırlanıyor : " + TableIPCode;

            #region TableIP and FieldsIP Read

            string softCode = "";
            string projectCode = "";
            string TableCode = string.Empty;
            string IPCode = string.Empty;
            //v.con_AutoNewRecords = false;

            t.TableIPCode_Get(TableIPCode, ref softCode, ref projectCode, ref TableCode, ref IPCode);

            DataSet ds_Table = new DataSet();
            DataSet ds_Fields = new DataSet();

            tTablesRead tr = new tTablesRead();

            if (v.con_ResimEditorRun)
            {
                /// Resim Editörünü arka arkaya açarken ds_Fields tablosunu çift okuyor bu nedenle datalar çift geliyor
                /// Nedeni bilinmiyor
                /// tr.MS_Fields_IP_Read buraya gitmeden önce biraz bekletme ile sorun çözüldü
                /// 
                Thread.Sleep(500);
                v.con_ResimEditorRun = false;
            }

            tr.MS_Tables_IP_Read(ds_Table, TableIPCode);
            tr.MS_Fields_IP_Read(ds_Fields, TableIPCode);

            /// Gridin aşağıdaki özelliği için kontrol kapatıldı... 
            /// if (tGridView.Columns.Count == 0)
            ///     tGridView.OptionsBehavior.AutoPopulateColumns = true;

            //if ((t.IsNotNull(ds_Table) == false) ||
            //    (t.IsNotNull(ds_Fields) == false)) return;

            if (t.IsNotNull(ds_Table) == false) return;


            DataRow row_Table = ds_Table.Tables[0].Rows[0] as DataRow;

            if (t.IsNotNull(MultiPageID))
                MultiPageID = "|" + MultiPageID;

            #endregion TableIP and FieldsIP Read

            #region Tanımlar

            string RefId = "[" + t.Set(row_Table["REF_ID"].ToString(), "", "") + "] ";
            string External_TableIPCode = t.Set(row_Table["EXTERNAL_IP_CODE"].ToString(), "", "");

            string caption = t.Set(row_Table["IP_CAPTION"].ToString(),
                                   row_Table["LKP_TB_CAPTION"].ToString(),
                                   row_Table["LKP_TABLE_NAME"].ToString());

            string TableName = t.Set(row_Table["LKP_TABLE_NAME"].ToString(), "", "");

            //[REFID]
            //tPanelControl.Text = RefId + caption;
            tPanelControl.Text = caption;

            if (tPanelControl.GetType().ToString() == "DevExpress.XtraEditors.XtraUserControl")
                tPanelControl.Parent.Text = caption;

            t.WaitFormOpen(v.mainForm, caption);

            #endregion Tanımlar

            /// * IP_VIEW_TYPE
            /// * 1,  'Data View'
            /// * 2,  'Kriter View'
            /// * 3,  'Kategori View'
            /// * 4,  'HGS View'
            /// *

            #region Data View 
            if (IP_View_Type == 1)
            {
                Create_Data_View(tForm, tPanelControl, TableIPCode, External_TableIPCode, MultiPageID, ds_Table, ds_Fields);
            }
            #endregion Data View

            #region Kriter View
            if (IP_View_Type == 2)
            {
                Create_Kriter(tForm, tPanelControl, TableIPCode, MultiPageID, ds_Table, ds_Fields);
            }
            #endregion Kriter View

            #region Category/Kategori View
            if (IP_View_Type == 3)
            {
                Create_Category(tForm, tPanelControl, TableIPCode, TableName, ds_Table, ds_Fields);
            }
            #endregion Kategori View

            #region HGS View
            if (IP_View_Type == 4)
            {

            }
            #endregion HGS View

            tPanelControl.Padding = new System.Windows.Forms.Padding(v.Padding4);

            if (isCloseWaitForm)
            {
                v.IsWaitOpen = false;
                t.WaitFormClose();
            }

            t.Takipci(function_name, "", '}');
        }

        #endregion Create InputPanel

        #region Sub InputPanel Functions

        #region Create_Data_View

        private void Create_Data_View(Form tForm, Control tPanelControl,
                     string TableIPCode, string External_TableIPCode, string MultiPageID,
                     DataSet ds_Table, DataSet ds_Fields)
        {
            tCreateObject co = new tCreateObject();
            tDevView dv = new tDevView();
            tToolBox t = new tToolBox();

            string function_name = "Create_Data_View";
            //t.Takipci(function_name, "", '{');

            #region Tanımlar
            DataSet ds_Data = new DataSet();
            DataRow row_Table = ds_Table.Tables[0].Rows[0];

            bool tabStop = true;
            string navigator_buttons = t.Set(row_Table["NAVIGATOR"].ToString(), "", "");

            vTableAbout vTA = new vTableAbout();
            t.Table_About(vTA, ds_Fields);

            #endregion Tanımlar 

            #region dsData Read

            // TableIPCode hazırlanıyor (  External TableIPCode yok ise kendisine ait dsData hazırlansın )
            if (t.IsNotNull(External_TableIPCode) == false)
            {
                //v.Kullaniciya_Mesaj_Var = "Data Read : " + TableIPCode;
                //string bbb = "";
                //if (TableIPCode == "UST/MEB/MtskTeorikDers.TeoPlanlamaManuelSonucu")
                //    bbb = "aaa";

                tSQLs sql = new tSQLs();
                sql.Preparing_dsData(tForm, row_Table, ds_Fields, ref ds_Data, MultiPageID, vTA);
            }
            else
            {
                /// External_TableIPCode var ise başka bir IP ye bağlanacak demektir
                /// InputPanel in kendisine ait bir DataSet i olmayacak ve başka bir DataSet e bağlanacak ise
                /// bu bağlanacağı dataset bulunur ve onun üzerine 
                /// External_IP=True  ve 
                /// External_TableIPCode=.... birinci bağlantı
                /// External_TableIPCode=.... ikinci bağlantı External_TableIPCode eklenir
                /// 

                ds_Data = t.Find_DataSet(tForm, "", External_TableIPCode, function_name + "/(External TableIPCode)");
                if (ds_Data != null)
                {
                    //ds_Data.DataSetName = External_TableIPCode; gerek yok zaten aynı value mevcut

                    string myProp = ds_Data.Namespace.ToString();

                    /// External_IP=True
                    if (myProp.IndexOf("External_IP") == -1)
                        t.MyProperties_Set(ref myProp, "External_IP", "True");
                    /// bir ıp ye birden fazla extra_IP bağlı olabilir 
                    t.MyProperties_Set(ref myProp, "External_TableIPCode", TableIPCode);

                    ds_Data.Namespace = myProp;

                    /// bu işaretleme  ( t.External_Controls_Enabled ) içinde kontrola yarıyor
                    
                }
                else
                {
                    //MessageBox.Show("DİKKAT : " + External_TableIPCode + " için DataControl tespit edilemedi ...", function_name + "(External TableIPCode)");
                    t.AlertMessage("External_TableIPCode bağlantısı", External_TableIPCode + " için data bulunamadı...");
                }
            }

            #endregion dsData Read

            #region tDataControl_IP_Fields_ 

            /// InputPanel için tablonun field listesi ve fieldlere ait bilgiler okunuyor
            ///
            /// InputPanel için gerekeli olan bilgiler ds_Fields üzerinde bulunmaktadır
            /// bu bilgilere daha sonra runtime sırasında da gerek duyulmakta ( Default value için )
            /// bu DataSet e ulaşmak için başka bir işe yaramayan bir control oluşturulmakta ve
            /// bu controle bağlanmak, runtime sırasında önce bu controle onun sayesinde de bu DataSet e 
            /// tekrar ulaşılmakta
            DevExpress.XtraDataLayout.DataLayoutControl DataLControl = new DevExpress.XtraDataLayout.DataLayoutControl();
            DataLControl.Name = "tDataControl_IP_Fields_" + TableIPCode + MultiPageID;
            DataLControl.DataSource = ds_Fields.Tables[0];
            DataLControl.Visible = false;

            tPanelControl.Controls.Add(DataLControl);

            #endregion tDataControl_IP_Fields_

            /// Muhakkak oluşturulması gerekiyor

            #region NavigatorPanel

            //co.Create_Navigator(tForm, tPanelControl, row_Table, ds_Data, 
            //                    TableIPCode + MultiPageID, External_TableIPCode);

            if (tPanelControl.ToString() != "DevExpress.XtraBars.Navigation.NavigationPane")
            {
                co.Create_Navigator(tForm, tPanelControl, row_Table, ds_Data, ds_Fields,
                                    TableIPCode + MultiPageID, External_TableIPCode);
            }
            else
            {
                co.Create_Navigator(tForm, tPanelControl.Parent, row_Table, ds_Data, ds_Fields,
                                    TableIPCode + MultiPageID, External_TableIPCode);
            }

            #endregion NavigatorPanel

            #region Create VievControl 

            int ViewType = 0;
            Control cntrl = new Control();
            cntrl = t.Find_Control_View(tForm, TableIPCode);

            // Acaba doğru controlu mü (ViewControl) buldu ?
            // Yani : Birden fazla PanelControl var ve  
            //        bunların içinde aynı TableIPCode lu Viewler olabilir
            if (cntrl != null)
            {
                if ((cntrl).Parent.Name != tPanelControl.Name)
                {
                    cntrl = null;
                }
            }

            // ViewControl yok ise yenisini oluştur
            if (cntrl == null)
            {
                ViewType = t.Set(row_Table["VIEW_CMP_TYPE"].ToString(), "", v.obj_vw_GridView);

                cntrl = dv.Create_View(row_Table, TableIPCode, MultiPageID);

                if (cntrl != null)
                {
                    /// tPanelControl != NavigationPane (değilse)
                    /// NavigationPane dataWizard içinden geliyor
                    /// 
                    if (tPanelControl.ToString() != "DevExpress.XtraBars.Navigation.NavigationPane")
                    {
                        tPanelControl.Controls.Add(cntrl);
                        cntrl.Dock = DockStyle.Fill;
                        cntrl.BringToFront();
                        cntrl.TabIndex = 0;
                    }
                    else
                    {
                        //tPanelControl.Controls.Add(cntrl);
                        //cntrl.Left = 100;
                        //cntrl.Top = 100;
                        //cntrl.SendToBack();
                        //cntrl.Visible = false;
                    }

                }
            }

            #endregion Create VievControl

            #region preparing ViewControl

            // ViewControl var ise veya yeni oluşturulmuş ise ViewObject ekle 

            if (cntrl != null)
            {
                // ViewControlun tipi belli değil ise tespit ediliyor
                if (ViewType == 0)
                    ViewType = t.Find_Control_Type(cntrl);

                if (ViewType == v.obj_vw_GridView)
                {
                    dv.tGridView_Create(row_Table, ds_Fields, ds_Data, (DevExpress.XtraGrid.GridControl)cntrl, TableIPCode, ref tabStop);
                    co.Create_SpeedKriter(tForm, tPanelControl, row_Table, ds_Fields, MultiPageID);
                    co.Create_GridFindPanel(tForm, tPanelControl, (DevExpress.XtraGrid.GridControl)cntrl);
                    //((DevExpress.XtraGrid.GridControl)cntrl).MainView.EndInit();
                }

                if (ViewType == v.obj_vw_BandedGridView)
                {

                }

                if ((ViewType == v.obj_vw_BandedGridView) ||
                    (ViewType == v.obj_vw_AdvBandedGridView))
                {
                    dv.tAdvBandedGridView_Create(tForm, row_Table, ds_Fields, ds_Data, (DevExpress.XtraGrid.GridControl)cntrl);
                    co.Create_AdvBandedGrid_Group_Buttons(tForm, tPanelControl, row_Table, ds_Fields, TableIPCode);
                    co.Create_SpeedKriter(tForm, tPanelControl, row_Table, ds_Fields, MultiPageID);
                    co.Create_GridFindPanel(tForm, tPanelControl, (DevExpress.XtraGrid.GridControl)cntrl);
                    //((DevExpress.XtraGrid.GridControl)cntrl).MainView.EndInit();
                }

                if (ViewType == v.obj_vw_GridLayoutView)
                {
                    dv.tGridLayoutView_Create(row_Table, ds_Fields, ds_Data, (DevExpress.XtraGrid.GridControl)cntrl, TableIPCode);
                    //((DevExpress.XtraGrid.GridControl)cntrl).MainView.EndInit();
                }

                if (ViewType == v.obj_vw_CardView)
                {
                    dv.tCardView_Create(row_Table, ds_Fields, ds_Data, (DevExpress.XtraGrid.GridControl)cntrl, TableIPCode);
                    //((DevExpress.XtraGrid.GridControl)cntrl).MainView.EndInit();
                }

                if (ViewType == v.obj_vw_WinExplorerView)
                {
                    dv.tWinExplorerView_Create(row_Table, ds_Fields, ds_Data, (DevExpress.XtraGrid.GridControl)cntrl, TableIPCode);
                    co.Create_WinExplorerBands_Add(tPanelControl, (DevExpress.XtraGrid.GridControl)cntrl);
                    //((DevExpress.XtraGrid.GridControl)cntrl).MainView.EndInit();
                }

                if (ViewType == v.obj_vw_TileView)
                {
                    dv.tTileView_Create(row_Table, ds_Fields, ds_Data, (DevExpress.XtraGrid.GridControl)cntrl);
                    //((DevExpress.XtraGrid.GridControl)cntrl).MainView.EndInit();
                }

                if ((ViewType == v.obj_vw_DataLayoutView) || (ViewType == v.obj_vw_HtmlEditorsView))
                {
                    if (tPanelControl.ToString() != "DevExpress.XtraBars.Navigation.NavigationPane")
                    {
                        string FormName = tForm?.Name.ToString();
                        dv.tDataLayoutView_Create(row_Table, ds_Fields, ds_Data,
                            (DevExpress.XtraDataLayout.DataLayoutControl)cntrl, 1, FormName);
                        //((DevExpress.XtraDataLayout.DataLayoutControl)cntrl).EndInit();
                        
                    }
                    else
                    {
                        /// (tPanelControl.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavigationPane"))
                        /// 
                        string FormName = tForm.Name.ToString();
                        dv.tDataWizard_Create(row_Table, ds_Fields, ds_Data,
                            (DevExpress.XtraBars.Navigation.NavigationPane)tPanelControl, 1, FormName, vTA);
                    }
                }

                if (ViewType == v.obj_vw_TreeListView)
                {
                    dv.tTreeListView_Create(row_Table, ds_Fields, ds_Data, (DevExpress.XtraTreeList.TreeList)cntrl, TableIPCode);
                    co.Create_TreeFindPanel(tForm, tPanelControl, (DevExpress.XtraTreeList.TreeList)cntrl);
                    //((DevExpress.XtraTreeList.TreeList)cntrl).EndInit();
                }

                if ((ViewType == v.obj_vw_VGridSingle) ||
                    (ViewType == v.obj_vw_VGridMulti))
                {
                    dv.tVGrid_Create(row_Table, ds_Fields, ds_Data, (DevExpress.XtraVerticalGrid.VGridControl)cntrl);
                    //((DevExpress.XtraVerticalGrid.VGridControl)cntrl).EndInit();
                }

                if (ViewType == v.obj_vw_WizardControl)
                {
                    dv.tWizardControl_Create(row_Table, ds_Fields, ds_Data, (DevExpress.XtraWizard.WizardControl)cntrl);
                }

                if (ViewType == v.obj_vw_CalenderAndScheduler)
                {
                    dv.tCalenderAndScheduler_Create(row_Table, ds_Fields, ds_Data, (DevExpress.XtraScheduler.SchedulerControl)cntrl, tPanelControl);

                    // okuduğu ilk tarihe odaklanması için (cntrl).Start = ...
                    t.ViewControl_Enabled(tForm, ds_Data, TableIPCode);
                }

                if (ViewType == v.obj_vw_ChartsView)
                {
                    dv.tChartControl_Create(row_Table, ds_Fields, ds_Data, (DevExpress.XtraCharts.ChartControl)cntrl);
                }

                if (t.IsNotNull(External_TableIPCode))
                {
                    // Daha sonra Extranal_TableIPCode kullanan viewleri tespit etmek için işaretleniyor
                    // cntrl.AccessibleDescription = External_TableIPCode;
                    // PROP_RUNTIME AUTO_LST sırasında iptal edildi ÇAKIŞIYOR
                    // yeni adresi
                    cntrl.AccessibleDefaultActionDescription = External_TableIPCode;
                }

                tEventsGrid evg = new tEventsGrid();
                cntrl.DragEnter += new System.Windows.Forms.DragEventHandler(evg.myGridView_DragEnter);

                // add datapaneli ve navigator
                // t.Control_Enabled(ds_Data, cntrl);
            }

            #endregion preparing ViewControl 

            if (tPanelControl.TabIndex == 0)
            {
                tPanelControl.TabStop = true;
                tPanelControl.TabIndex = t.Find_DataNavigator_Count(tForm);//
            }

            if (TableIPCode.IndexOf("HPFRM") > -1)
            {
                if ((TableIPCode.IndexOf("HPFRM.HPFRM_USER") > -1) ||
                    (TableIPCode.IndexOf("HPFRM.HPFRM_FIRM") > -1))
                {
                    v.con_GotoRecord_FName = "LOCAL_ID";
                    v.con_GotoRecord_Value = v.SP_FIRM_ID.ToString();
                    t.tGotoRecord(tForm, ds_Data, v.con_GotoRecord_TableIPCode, v.con_GotoRecord_FName, v.con_GotoRecord_Value, -1);
                }
            }

            if ((v.con_GotoRecord == "ON") &&
                (TableIPCode == v.con_GotoRecord_TableIPCode))
            {
                t.tGotoRecord(tForm, ds_Data, v.con_GotoRecord_TableIPCode, v.con_GotoRecord_FName, v.con_GotoRecord_Value, -1);
            }

            //t.Takipci(function_name, "", '}');
        }

        #endregion Create_Data_View

        #region Create_Kriter

        private void Create_Kriter(Form tForm, Control tPanelControl,
                     string TableIPCode, string MultiPageID,
                     DataSet ds_Table, DataSet ds_Fields)
        {
            tToolBox t = new tToolBox();
            string function_name = "Create_Kriter";
            t.Takipci(function_name, "", '{');

            #region Tanımlar

            tCreateObject co = new tCreateObject();
            DataRow row_Table = ds_Table.Tables[0].Rows[0] as DataRow;

            int RefID = t.Set(row_Table["REF_ID"].ToString(), "", 0);
            string Prop_Runtime = t.Set(row_Table["PROP_RUNTIME"].ToString(), "", "");

            #endregion Tanımlar 

            #region VievControl Create

            tDevView dv = new tDevView();
            tEventsGrid evg = new tEventsGrid();

            Control cntrl = new Control();
            cntrl = t.Find_Control_View(tForm, TableIPCode);

            // ViewControl yok ise yenisini oluştur
            if ((cntrl == null) ||
                (cntrl.ToString() != "DevExpress.XtraVerticalGrid.VGridControl"))
            {
                cntrl = dv.Create_View_(v.obj_vw_VGridSingle, RefID, TableIPCode, MultiPageID);

                ((VGridControl)cntrl).BeginUpdate();
                ((VGridControl)cntrl).AccessibleName = "KRITER_" + TableIPCode;
                ((VGridControl)cntrl).OptionsFind.Visibility = FindPanelVisibility.Always;

                ((VGridControl)cntrl).KeyDown += new System.Windows.Forms.KeyEventHandler(evg.myVGridControl_Kriter_KeyDown);
                ((VGridControl)cntrl).KeyUp += new System.Windows.Forms.KeyEventHandler(evg.myVGridControl_Kriter_KeyUp);
                ((VGridControl)cntrl).KeyPress += new System.Windows.Forms.KeyPressEventHandler(evg.myVGridControl_Kriter_KeyPress);

                // RunTime Sırasında yapılacak işlerin listesi
                if (t.IsNotNull(Prop_Runtime))
                {
                    //cntrl.AccessibleDescription = Prop_Runtime;
                    // RunTime Sırasında yapılacak işlerin listesi
                    if (t.IsNotNull(Prop_Runtime)) //|| (t.IsNotNull(Prop_Navigator)))
                    {
                        cntrl.AccessibleDescription = Prop_Runtime + v.ENTER + "|ds|" + v.ENTER;// + Prop_Navigator;
                    }
                }

                //tVGrid.BestFit();
                //tVGrid.RecordWidth = (tVGrid.Width / 10) * 6;
                //tVGrid.RowHeaderWidth = (tVGrid.Width / 10) * 4;

                int w = tPanelControl.Width;

                ((VGridControl)cntrl).Width = w;
                ((VGridControl)cntrl).RowHeaderWidth = (w / 10) * 4;
                ((VGridControl)cntrl).RecordWidth = (w / 10) * 6;

                tPanelControl.Controls.Add(cntrl);
            }

            Create_Kriter_Column((VGridControl)cntrl, ds_Fields);

            Create_Kriter_Buttons((VGridControl)cntrl, TableIPCode);

            ((VGridControl)cntrl).EndUpdate();

            #endregion VievControl

            t.Takipci(function_name, "", '}');
        }

        private void Create_Kriter_Column(VGridControl tVGrid, DataSet dsFields)
        {
            tToolBox t = new tToolBox();
            string function_name = "Create_Kriter_Column";
            t.Takipci(function_name, "", '{');

            tDevColumn dc = new tDevColumn();
            tDefaultValue df = new tDefaultValue();

            RepositoryItemImageComboBox Even_Start_List = new RepositoryItemImageComboBox();
            RepositoryItemImageComboBox Even_Stop_List = new RepositoryItemImageComboBox();
            RepositoryItemImageComboBox Odd_Like_List = new RepositoryItemImageComboBox();
            RepositoryItemImageComboBox Odd_NotLike_List = new RepositoryItemImageComboBox();

            dc.tRepositoryItem_Fill(Even_Start_List, null, null, null, null, null, "KRT_EVEN_START", "", 2); // Tumu evet
            dc.tRepositoryItem_Fill(Even_Stop_List, null, null, null, null, null, "KRT_EVEN_STOP", "", 2);
            dc.tRepositoryItem_Fill(Odd_Like_List, null, null, null, null, null, "KRT_ODD_LIKE", "", 2);
            dc.tRepositoryItem_Fill(Odd_NotLike_List, null, null, null, null, null, "KRT_ODD_NOTLIKE", "", 2);

            string fname = string.Empty;
            string tcaption = string.Empty;
            string tTableLabel = string.Empty;
            string tfieldname = string.Empty;
            string tcolumn_type = string.Empty;
            string tdefault1 = string.Empty;
            string tdefault2 = string.Empty;
            string tkrt_alias = string.Empty;
            string tkrt_table_alias = string.Empty;

            int tfieldtype = 0;
            Int16 tkrt_sort_no = 0;
            byte toperand_type = 0;
            Boolean tkriter_like = false;

            foreach (DataRow Row in dsFields.Tables[0].Rows)
            {
                fname = t.Set(Row["KRT_CAPTION"].ToString(), Row["LKP_FIELD_NAME"].ToString(), "null");
                tcaption = t.Set(Row["FCAPTION"].ToString(), Row["LKP_FCAPTION"].ToString(), fname);
                tTableLabel = "[" + Row["LKP_TABLE_CODE"].ToString() + "]";
                tfieldname = tTableLabel + "." + fname;
                tfieldtype = t.Set(Row["LKP_FIELD_TYPE"].ToString(), "", (int)167);
                toperand_type = t.Set(Row["KRT_OPERAND_TYPE"].ToString(), "", (byte)0);
                tkriter_like = t.Set(Row["KRT_LIKE"].ToString(), "", false);
                tkrt_sort_no = t.Set(Row["KRT_LINE_NO"].ToString(), "", (Int16)0);
                tcolumn_type = t.Set(Row["CMP_COLUMN_TYPE"].ToString(), Row["LKP_CMP_COLUMN_TYPE"].ToString(), "TextEdit");
                tdefault1 = t.Set(Row["KRT_DEFAULT1"].ToString(), "", "");
                tdefault2 = t.Set(Row["KRT_DEFAULT2"].ToString(), "", "");
                tkrt_alias = t.Set(Row["KRT_ALIAS"].ToString(), "", "");
                tkrt_table_alias = t.Set(Row["KRT_TABLE_ALIAS"].ToString(), "", "");

                // Eğer özellikle başka bir ailas belitilmişse bu alias kullanılacak
                if (t.IsNotNull(tkrt_table_alias))
                {
                    tTableLabel = tkrt_table_alias;
                    tfieldname = tTableLabel + "." + fname;
                }

                CategoryRow row = new CategoryRow(tcaption);
                row.Name = "Column_" + fname;
                row.Expanded = true;

                #region even - Double
                if (toperand_type == 1)
                {
                    EditorRow rowA1 = new EditorRow("bas_" + fname);
                    rowA1.Properties.Caption = "Başlangıç";
                    rowA1.Properties.FieldName = tfieldname;
                    rowA1.Properties.CustomizationCaption = tkrt_alias;
                    rowA1.Tag = tfieldtype;

                    EditorRow rowA2 = new EditorRow("bit_" + fname);
                    rowA2.Properties.Caption = "Bitiş";
                    rowA2.Properties.FieldName = tfieldname;
                    rowA2.Properties.CustomizationCaption = tkrt_alias;
                    rowA2.Tag = tfieldtype;

                    EditorRow rowB1 = new EditorRow("bas_sorgu_" + fname);
                    rowB1.Properties.Caption = "Başlangıç Sorgu";
                    EditorRow rowB2 = new EditorRow("bit_sorgu_" + fname);
                    rowB2.Properties.Caption = "Bitiş Sorgu";

                    dc.VGrid_ColumnEdit(Row, rowA1, tcolumn_type);
                    dc.VGrid_ColumnEdit(Row, rowA2, tcolumn_type);

                    // default değerler dolduruluyor
                    if (tcolumn_type == "DateEdit")
                    {
                        if (t.IsNotNull(tdefault1))
                            rowA1.Properties.Value = df.tSP_Value_Load(tdefault1);
                        if (t.IsNotNull(tdefault2))
                            rowA2.Properties.Value = df.tSP_Value_Load(tdefault2);
                    }

                    rowB1.Properties.RowEdit = Even_Start_List;
                    rowB2.Properties.RowEdit = Even_Stop_List;

                    rowB1.Properties.Value = ((short)(v.EsitVeBuyuk));
                    rowB2.Properties.Value = ((short)(v.EsitVeKucuk));

                    row.ChildRows.Add(rowA1);
                    row.ChildRows.Add(rowA2);
                    row.ChildRows.Add(rowB1);
                    row.ChildRows.Add(rowB2);

                }
                #endregion even

                #region odd - Single
                if (toperand_type == 2)
                {
                    EditorRow rowA1 = new EditorRow("bas_" + fname);
                    rowA1.Properties.Caption = tcaption;
                    rowA1.Properties.FieldName = tfieldname;
                    rowA1.Properties.CustomizationCaption = tkrt_alias;
                    rowA1.Tag = tfieldtype;

                    EditorRow rowB1 = new EditorRow("bas_sorgu_" + fname);
                    rowB1.Properties.Caption = "Sorgu";

                    dc.VGrid_ColumnEdit(Row, rowA1, tcolumn_type);

                    // default değerler dolduruluyor
                    if (tcolumn_type == "DateEdit")
                    {
                        if (t.IsNotNull(tdefault1))
                            rowA1.Properties.Value = df.tSP_Value_Load(tdefault1);
                    }


                    if (tkriter_like)
                    {
                        rowB1.Properties.RowEdit = Odd_Like_List;
                        rowB1.Properties.Value = ((short)(v.Benzerleri_Tam));
                    }
                    else
                    {
                        rowB1.Properties.RowEdit = Odd_NotLike_List;
                        rowB1.Properties.Value = ((short)(v.Esit));
                    }

                    row.ChildRows.Add(rowA1);
                    row.ChildRows.Add(rowB1);
                }
                #endregion odd

                if (tkrt_sort_no > 0)
                {
                    if (tVGrid != null)
                        tVGrid.Rows.Add(row);
                }


            } // foreach

            tVGrid.CollapseAllRows();

            t.Takipci(function_name, "", '}');
        }

        private void Create_Kriter_Buttons(VGridControl tVGrid, string TableIPCode)
        {
            tEvents ev = new tEvents();

            DevExpress.XtraEditors.GroupControl groupControl_KriterButonlari = new DevExpress.XtraEditors.GroupControl();
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Kriter = new System.Windows.Forms.TableLayoutPanel();
            DevExpress.XtraEditors.SimpleButton simpleButton_Temizle = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Collapse = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Kriter_Listele = new DevExpress.XtraEditors.SimpleButton();
            DevExpress.XtraEditors.SimpleButton simpleButton_Expand = new DevExpress.XtraEditors.SimpleButton();

            //tGridControl.Controls.Add(groupControl_krt);
            Control c = tVGrid.Parent;
            c.Controls.Add(groupControl_KriterButonlari);

            //this.SuspendLayout();
            //
            // groupControl_KriterButonlari
            // 
            groupControl_KriterButonlari.Controls.Add(tableLayoutPanel_Kriter);
            //groupControl_KriterButonlari.Location = new System.Drawing.Point(0, 0);
            groupControl_KriterButonlari.Name = "groupControl_KriterButonlari";
            groupControl_KriterButonlari.Size = new System.Drawing.Size(293, 52); //62
            groupControl_KriterButonlari.TabIndex = 1;
            groupControl_KriterButonlari.Text = "Kriter";
            groupControl_KriterButonlari.Dock = DockStyle.Bottom;
            // 
            // tableLayoutPanel_Kriter
            // 
            //tableLayoutPanel_Kriter.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            tableLayoutPanel_Kriter.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            tableLayoutPanel_Kriter.ColumnCount = 4;
            tableLayoutPanel_Kriter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            tableLayoutPanel_Kriter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            tableLayoutPanel_Kriter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            tableLayoutPanel_Kriter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            tableLayoutPanel_Kriter.Controls.Add(simpleButton_Temizle, 0, 0);
            tableLayoutPanel_Kriter.Controls.Add(simpleButton_Collapse, 1, 0);
            tableLayoutPanel_Kriter.Controls.Add(simpleButton_Expand, 2, 0);
            tableLayoutPanel_Kriter.Controls.Add(simpleButton_Kriter_Listele, 3, 0);
            tableLayoutPanel_Kriter.Location = new System.Drawing.Point(2, 21);
            tableLayoutPanel_Kriter.Name = "tableLayoutPanel_Kriter";
            //tableLayoutPanel_Kriter.Padding = new System.Windows.Forms.Padding(1);
            tableLayoutPanel_Kriter.RowCount = 1;
            tableLayoutPanel_Kriter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            //tableLayoutPanel_Kriter.Size = new System.Drawing.Size(289, 39);
            tableLayoutPanel_Kriter.TabIndex = 0;
            tableLayoutPanel_Kriter.Dock = System.Windows.Forms.DockStyle.Fill;
            //tableLayoutPanel_Kriter.Refresh();


            // 
            // simpleButton_Listele
            // 
            simpleButton_Kriter_Listele.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Kriter_Listele.Location = new System.Drawing.Point(189, 6);
            simpleButton_Kriter_Listele.Name = "simpleButton_Kriter_Listele";
            simpleButton_Kriter_Listele.Size = new System.Drawing.Size(94, 23);
            simpleButton_Kriter_Listele.TabIndex = 1;
            simpleButton_Kriter_Listele.Text = "&Listele";

            simpleButton_Kriter_Listele.Click += new System.EventHandler(ev.btn_KriterListele_Click);
            simpleButton_Kriter_Listele.AccessibleName = TableIPCode;
            simpleButton_Kriter_Listele.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Temizle
            // 
            simpleButton_Temizle.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Temizle.Location = new System.Drawing.Point(6, 6);
            simpleButton_Temizle.Name = "simpleButton_Temizle";
            simpleButton_Temizle.Size = new System.Drawing.Size(93, 23);
            simpleButton_Temizle.TabIndex = 2;
            simpleButton_Temizle.Text = "&Temizle";

            simpleButton_Temizle.Click += new System.EventHandler(ev.btn_KriterListele_Click);
            simpleButton_Temizle.AccessibleName = TableIPCode;
            simpleButton_Temizle.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Collapse
            // 
            simpleButton_Collapse.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Collapse.Location = new System.Drawing.Point(105, 6);
            simpleButton_Collapse.Name = "simpleButton_Collapse";
            simpleButton_Collapse.Size = new System.Drawing.Size(36, 23);
            simpleButton_Collapse.TabIndex = 3;
            simpleButton_Collapse.Text = "Kpt";

            simpleButton_Collapse.Click += new System.EventHandler(ev.btn_KriterListele_Click);
            simpleButton_Collapse.AccessibleName = TableIPCode;
            simpleButton_Collapse.AccessibleDescription = tVGrid.Name.ToString();
            // 
            // simpleButton_Expand
            // 
            simpleButton_Expand.Dock = System.Windows.Forms.DockStyle.Top;
            simpleButton_Expand.Location = new System.Drawing.Point(147, 6);
            simpleButton_Expand.Name = "simpleButton_Expand";
            simpleButton_Expand.Size = new System.Drawing.Size(36, 24);
            simpleButton_Expand.TabIndex = 4;
            simpleButton_Expand.Text = "Aç";

            simpleButton_Expand.Click += new System.EventHandler(ev.btn_KriterListele_Click);
            simpleButton_Expand.AccessibleName = TableIPCode;
            simpleButton_Expand.AccessibleDescription = tVGrid.Name.ToString();

        }

        #endregion Create_Kriter

        #region Create_Kategori

        private void Create_Category(Form tForm, Control tPanelControl,
                     string TableIPCode, string TableName,
                     DataSet ds_Table, DataSet ds_Fields)
        {
            tToolBox t = new tToolBox();
            string function_name = "Create_Kategori";
            t.Takipci(function_name, "", '{');

            #region Tanımlar

            tEvents ev = new tEvents();

            DataRow row_Table = ds_Table.Tables[0].Rows[0] as DataRow;

            //string TableName = t.Set(row_Table["LKP_TABLE_NAME"].ToString(), "", "");
            //string navigator = t.Set(row_Table["NAVIGATOR"].ToString(), "", "");
            byte DBaseNo = t.Set(row_Table["LKP_DBASE_TYPE"].ToString(), "", (byte)3);

            string sSQL = string.Empty;
            string softCode = "";
            string projectCode = "";
            string TableCode = string.Empty;
            string IPCode = string.Empty;

            t.TableIPCode_Get(TableIPCode, ref softCode, ref projectCode, ref TableCode, ref IPCode);

            int RefID = t.Set(row_Table["REF_ID"].ToString(), "", 0);

            #endregion Tanımlar

            #region SplitContainerControl
            DevExpress.XtraEditors.SplitContainerControl tSplitContainerControl =
                       new DevExpress.XtraEditors.SplitContainerControl();

            tSplitContainerControl.Name = "tSplitContainerControl_" + RefID.ToString();
            tSplitContainerControl.Text = "tSplitContainerControl_" + RefID.ToString();
            //tSplitContainerControl.Tag = MasterKey;
            tSplitContainerControl.AutoSize = false;
            tSplitContainerControl.SplitterPosition = 200;
            tSplitContainerControl.Horizontal = false;
            tSplitContainerControl.Dock = DockStyle.Fill;

            //tSplitContainerControl.Padding = new System.Windows.Forms.Padding(sp.Padding);
            //tSplitContainerControl.Panel1.Padding = new System.Windows.Forms.Padding(sp.Padding);
            //tSplitContainerControl.Panel2.Padding = new System.Windows.Forms.Padding(sp.Padding);
            #endregion SplitContainerControl

            #region Panel1 - CategoryList
            DevExpress.XtraEditors.PanelControl tPanelControl1 = new DevExpress.XtraEditors.PanelControl();
            tPanelControl1.Name = "tPanelControl1_" + RefID.ToString();
            tPanelControl1.Height = 26;
            tPanelControl1.Dock = DockStyle.Bottom;

            DevExpress.XtraGrid.GridControl tGridControl = new DevExpress.XtraGrid.GridControl();
            tGridControl.Name = "tGridControl_" + RefID.ToString();
            tGridControl.AccessibleName = "tGridControl_Category_" + RefID.ToString(); // + TableName;
            tGridControl.Dock = DockStyle.Fill;

            GridView tGridView = new GridView(tGridControl);
            tGridView.Name = "tGridView_" + RefID.ToString();
            tGridView.OptionsView.RowAutoHeight = true;
            tGridView.OptionsView.ShowGroupPanel = false;

            GridColumn column = new GridColumn();
            column.Name = "Column_" + RefID.ToString(); // +lkp_tfieldname;
            column.FieldName = "CAPTION"; //lkp_tfieldname;
            column.Visible = true;
            column.VisibleIndex = 0;
            column.OptionsColumn.AllowEdit = true;
            column.Caption = "Kategori Listesi";
            column.Width = 100;
            column.OptionsColumn.ReadOnly = true;
            column.OptionsColumn.AllowEdit = false;
            tGridView.Columns.Add(column);

            tGridControl.MainView = tGridView;

            DataSet dsData_CatList = new DataSet();
            dsData_CatList.DataSetName = "tCategoryList";
            dsData_CatList.Namespace = TableName;

            tSQLs sql = new tSQLs();
            sSQL = sql.CategoryList(TableCode);

            v.SQL = v.SQL + v.ENTER + "[" + TableName + "]" + v.ENTER + sSQL + v.ENTER;

            if (dsData_CatList.Tables.Count > 0)
                tGridControl.DataSource = dsData_CatList.Tables[0];

            DevExpress.XtraEditors.DataNavigator tDataNavigator_CatList = new DevExpress.XtraEditors.DataNavigator();
            tDataNavigator_CatList.Width = 100;
            tDataNavigator_CatList.Name = "tDataNavigator_CatList";// +RefID.ToString();
            //tDataNavigator_CatList.AccessibleName = "tDataNavigator_CatList";

            // Tablonu konumlandığı ID yi tutuyor, db.MyRecord() için gerekli
            tDataNavigator_CatList.Tag = 0;
            // runtime sırasında DataNavigator e ulaşmak için 
            tDataNavigator_CatList.AccessibleName = TableIPCode;
            // Tablonun Adını tutuyor, db.MyRecord() için gerekli
            tDataNavigator_CatList.AccessibleDefaultActionDescription = TableName;
            // Database in Türünü tutuyor, db.MyRecord() için gerekli
            tDataNavigator_CatList.AccessibleDescription = DBaseNo.ToString();

            tDataNavigator_CatList.PositionChanged += new System.EventHandler(ev.ctg_DataNavigator_CatList_PositionChanged);
            tDataNavigator_CatList.Dock = DockStyle.Bottom;
            tDataNavigator_CatList.Visible = false;
            if (dsData_CatList.Tables.Count > 0)
                tDataNavigator_CatList.DataSource = dsData_CatList.Tables[0];


            #endregion Panel1 - CategoryList

            #region Panel2 - CategoryDetail
            DevExpress.XtraEditors.PanelControl tPanelControl2 = new DevExpress.XtraEditors.PanelControl();
            tPanelControl2.Name = "tPanelControl2_" + RefID.ToString();
            tPanelControl2.Height = 26;
            tPanelControl2.Dock = DockStyle.Bottom;

            DevExpress.XtraTreeList.TreeList tTreeList = new DevExpress.XtraTreeList.TreeList();
            tTreeList.BeginInit();
            tTreeList.Name = "tTreeList_CatDetail_";// + RefID.ToString();
            tTreeList.AccessibleName = "tTreeList_CatDetail_" + RefID.ToString(); // +TableName;
            tTreeList.Dock = DockStyle.Fill;

            /*
            DevExpress.XtraTreeList.Columns.TreeListColumn tTLcolumn_Id =
                new DevExpress.XtraTreeList.Columns.TreeListColumn();

            tTLcolumn_Id.Caption = "Id";
            tTLcolumn_Id.FieldName = "LKP_ID";
            tTLcolumn_Id.Name = "tTLcolumn_Id";
            tTLcolumn_Id.Visible = true;
            tTLcolumn_Id.VisibleIndex = 0;
            tTLcolumn_Id.Width = 20;
            */
            DevExpress.XtraTreeList.Columns.TreeListColumn tTLcolumn_Caption =
                new DevExpress.XtraTreeList.Columns.TreeListColumn();

            tTLcolumn_Caption.Caption = "Başlık";
            tTLcolumn_Caption.FieldName = "LKP_CAPTION";
            tTLcolumn_Caption.Name = "tTLcolumn_Caption";
            tTLcolumn_Caption.Visible = true;
            tTLcolumn_Caption.VisibleIndex = 0;
            tTLcolumn_Caption.Width = 80;
            tTLcolumn_Caption.OptionsColumn.AllowEdit = false;

            tTreeList.KeyFieldName = "LKP_ID";
            tTreeList.ParentFieldName = "IMAGE_ID";
            //tTreeList.OptionsBehavior.DragNodes = true;

            tTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
                //tTLcolumn_Id,
                tTLcolumn_Caption});

            tTreeList.EndInit();

            DataSet dsData_CatDetail = new DataSet();
            dsData_CatDetail.DataSetName = "tCategoryDetail_";
            dsData_CatDetail.Namespace = TableIPCode;

            // ekrana ilk önce boş data okunsun

            sSQL = "select UNUTMA";//sql.APP_Types_SQL(-1);
            //************

            if (dsData_CatDetail.Tables.Count > 0)
                tTreeList.DataSource = dsData_CatDetail.Tables[0];

            DevExpress.XtraEditors.DataNavigator tDataNavigator_CatDetail = new DevExpress.XtraEditors.DataNavigator();
            tDataNavigator_CatDetail.Width = 100;
            tDataNavigator_CatDetail.Name = "tDataNavigator_CatDetail" + RefID.ToString();
            //tDataNavigator_CatDetail.AccessibleName = "tDataNavigator_CatDetail";

            // Tablonu konumlandığı ID yi tutuyor, db.MyRecord() için gerekli
            tDataNavigator_CatDetail.Tag = 0;
            // runtime sırasında DataNavigator e ulaşmak için 
            tDataNavigator_CatDetail.AccessibleName = TableIPCode;
            // Tablonun Adını tutuyor, db.MyRecord() için gerekli
            tDataNavigator_CatDetail.AccessibleDefaultActionDescription = TableName;
            // Database in Türünü tutuyor, db.MyRecord() için gerekli
            tDataNavigator_CatDetail.AccessibleDescription = DBaseNo.ToString();


            tDataNavigator_CatDetail.PositionChanged += new System.EventHandler(ev.ctg_DataNavigator_CatDetail_PositionChanged);
            tDataNavigator_CatDetail.Dock = DockStyle.Bottom;
            tDataNavigator_CatDetail.Visible = false;
            if (dsData_CatDetail.Tables.Count > 0)
                tDataNavigator_CatDetail.DataSource = dsData_CatDetail.Tables[0];

            #endregion Panel2 - CategoryDetail

            #region Controls.Add

            tSplitContainerControl.Panel1.Controls.Add(tPanelControl1);
            tSplitContainerControl.Panel1.Controls.Add(tGridControl);
            tSplitContainerControl.Panel1.Controls.Add(tDataNavigator_CatList);

            tSplitContainerControl.Panel2.Controls.Add(tPanelControl2);
            tSplitContainerControl.Panel2.Controls.Add(tTreeList);
            tSplitContainerControl.Panel2.Controls.Add(tDataNavigator_CatDetail);

            tPanelControl.Controls.Add(tSplitContainerControl);

            #endregion Controls.Add

            t.Takipci(function_name, "", '}');
        }


        #endregion Create_Kategori

        #endregion Sub InputPanel Functions

        #region Create_DataSet

        // İstenilen TableIPCode  ye ait sadece dsData kısmı oluşturuluyor
        public DataSet Create_DataSet(Form tForm, string TableIPCode, bool IsKisitlama)
        {
            tToolBox t = new tToolBox();
            
            #region TableIP and FieldsIP Read

            string softCode = "";
            string projectCode = "";
            string TableCode = string.Empty;
            string IPCode = string.Empty;

            t.TableIPCode_Get(TableIPCode, ref softCode, ref projectCode, ref TableCode, ref IPCode);

            DataSet ds_Table = new DataSet();
            DataSet ds_Fields = new DataSet();
            // Sonuc olarak dönecek olan DataSet
            DataSet ds_Data = new DataSet();

            tTablesRead tr = new tTablesRead();

            tr.MS_Tables_IP_Read(ds_Table, TableIPCode);
            tr.MS_Fields_IP_Read(ds_Fields, TableIPCode);

            DataRow row_Table = ds_Table.Tables[0].Rows[0] as DataRow;

            vTableAbout vTA = new vTableAbout();
            t.Table_About(vTA, ds_Fields);

            /// Kıstlama yaparak tabloyu aç böylece içi dolu data gelmesin
            vTA.IsKisitlama = IsKisitlama;

            tSQLs sql = new tSQLs();
            sql.Preparing_dsData(tForm, row_Table, ds_Fields, ref ds_Data, "", vTA);

            #endregion TableIP and FieldsIP Read
                        
            return ds_Data;
        }

        #endregion Create_DataSet

    }
}
