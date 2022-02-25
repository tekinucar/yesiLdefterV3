using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraNavBar;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.XtraGrid.Views.WinExplorer;
using DevExpress.XtraGrid.Views.WinExplorer.ViewInfo;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraDataLayout;
using DevExpress.XtraVerticalGrid;

using Tkn_ToolBox;
using Tkn_Variable;
using Tkn_CreateObject;
using Tkn_DataCopy;
using Tkn_DefaultValue;
using Tkn_InputPanel;
using Tkn_Save;
using Tkn_Forms;

using Microsoft.JScript;        // needs a reference to Microsoft.JScript.dll
using Microsoft.JScript.Vsa;
using Newtonsoft.Json;
using System.Drawing.Drawing2D;
using DevExpress.XtraEditors.Repository;

namespace Tkn_Events
{

    public class tEvents : tBase
    {

        GridHitInfo gridHitInfo = null;
        DevExpress.XtraGrid.Views.WinExplorer.ViewInfo.WinExplorerViewHitInfo winExplorerHitInfo = null;
        TileViewHitInfo tileHitInfo = null;
        //CardHitInfo cardHitInfo = null;

        #region Form Events Functions

        public void tFormEventsAdd(Form tForm)
        {

            tForm.Load += new System.EventHandler(myForm_Load);
            tForm.Shown += new System.EventHandler(myForm_Shown);
            tForm.Enter += new System.EventHandler(myForm_Enter);
            tForm.Leave += new System.EventHandler(myForm_Leave);

            tForm.Activated += new System.EventHandler(myForm_Activated);
            tForm.Deactivate += new System.EventHandler(myForm_Deactivate);
            tForm.Validating += new System.ComponentModel.CancelEventHandler(myForm_Validating);
            tForm.Validated += new System.EventHandler(myForm_Validated);

            tForm.FormClosing += new System.Windows.Forms.FormClosingEventHandler(myForm_FormClosing);
            tForm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(myForm_FormClosed);

            tForm.KeyDown += new System.Windows.Forms.KeyEventHandler(myForm_KeyDown);
            tForm.KeyPreview = true;

        }

        public void myForm_Activated(object sender, EventArgs e)
        {
            //MessageBox.Show("myForm_Activated");
            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";Act";

            // Kayıt işlemi gerçekleştiyse
            if (((Form)sender).HelpButton == true)
            {
                myForm_Refresh(((Form)sender), "myForm_Activated");
            }
        }

        public void myForm_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("myForm_Load");
            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";Load";

        }

        public void myForm_Shown(object sender, EventArgs e)
        {
            //MessageBox.Show("myForm_Shown");
            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";Show";

            if ((v.SP_UserLOGIN == false) &&
                (v.SP_UserIN))
                Application.Exit();

            if (v.con_FormAfterCreateView)
            {
                tExtraCreateView((Form)sender);

                v.con_FormAfterCreateView = false;
            }

            //if (v.con_AutoNewRecords)
            //{
            //    //AutoNewRecords((Form)sender);
            //    v.con_PositionChange = true;

            //    tDataNavigatorList((Form)sender, "tNewData");

            //    v.con_PositionChange = false;
            //    v.con_AutoNewRecords = false;
            //}

            #region Detail-SubDetail varmı?
            if (((Form)sender).IsAccessible == true)
            {
                // IsAccessible == true  ise formun üzerinde 
                // kendisine (detail'e) bağlı alt tablolar(subdetail) vardır 

                // Detail Table
                //    |____ SubDetail Table

                // Detail Table da bir row değişikliği olduğunda buna bağlı olan  
                // SubDetail Tablolarında yeniden okunması gerekiyor

                //---iptal---Detail_SubDetail_Refresh((Form)sender);

                //tDataNavigatorList((Form)sender, "Detail_SubDetail_Refresh");
            }
            #endregion Detail-SubDetail varmı?

            if (v.con_GotoRecord == "ONdialog")
            {
                // DialogForm da bu işlem çalışmadığı için
                // FormLoad sırasında bu işlemleri tekrar yapmak gerekiyor

                // search Engine içinden tetikleniyor
                tToolBox t = new tToolBox();
                t.tGotoRecord((Form)sender, null,
                    v.con_TableIPCode,
                    v.con_GotoRecord_FName,
                    v.con_GotoRecord_Value,
                    v.con_GotoRecord_Position);

            }
            

            /// Yeni Süreç ----
            /// 
            vSubWork vSW = new vSubWork();
            vSW._01_tForm = (Form)sender;
            vSW._02_TableIPCode = "";
            vSW._03_WorkTD = v.tWorkTD.NewAndRef;
            vSW._04_WorkWhom = v.tWorkWhom.All;

            tSubWork_(vSW);

            //v.con_AutoNewRecords = false;
            /// end yeni süreç

            #region Category
            // Eğer ekranda Kategori - Category var ise oku
            //Category_Form_Shown(((Form)sender));
            #endregion Category

            

        }

        public void myForm_Refresh(Form tForm, string Kim)
        {
            //MessageBox.Show("myForm_Refresh");
            Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";Refr=" + Kim;
                        
            /// Yeni Süreç ----
            /// 
            vSubWork vSW = new vSubWork();
            vSW._01_tForm = tForm;
            vSW._02_TableIPCode = "";
            vSW._03_WorkTD = v.tWorkTD.Refresh_Data;
            vSW._04_WorkWhom = v.tWorkWhom.All;
            tSubWork_(vSW);


            if (v.con_GotoRecord == "ONdialog")
            {
                // DialogForm da bu işlem çalışmadığı için
                // FormLoad sırasında bu işlemleri tekrar yapmak gerekiyor

                // search Engine içinden tetikleniyor
                tToolBox t = new tToolBox();
                t.tGotoRecord(tForm, null,
                    v.con_TableIPCode,
                    v.con_GotoRecord_FName,
                    v.con_GotoRecord_Value,
                    v.con_GotoRecord_Position);

                tForm.HelpButton = false;
            }


            /// Açma : ? : Fiş ve sepet formları arasında ikiside açıkken 
            /// kullanıcı manuel olarak git gel yapabiliyor
            ///
            //tForm.HelpButton = false;
        }

        public void myForm_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (Application.OpenForms[0].Text.Length > 200)
                Application.OpenForms[0].Text = "";
            Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";"+e.KeyCode.ToString();
            */
            if (e.KeyCode == Keys.Escape)
            {
                if (((Form)sender).Name == "tSearchForm")
                    ((Form)sender).Close();
            }

            if (e.KeyCode == v.Key_SearchEngine)
            {
                //
                //Application.OpenForms[0].Text = ((Form)sender).ActiveControl.ToString();
            }

            if (e.KeyCode == v.Key_Save)
            {
                AutoSave(((Form)sender));
            }

            

        }

        public void myForm_Enter(object sender, EventArgs e)
        {
            //MessageBox.Show("myForm_Enter : " + ((Form)sender).Text);
            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";Ent";

            // Kayıt işlemi gerçekleştiyse
            //if (((Form)sender).HelpButton == true)
            //{
            //    myForm_Refresh(((Form)sender), "myForm_Enter");
            //}
        }

        public void myForm_Leave(object sender, EventArgs e)
        {
            //MessageBox.Show("myForm_Leave : " + ((Form)sender).Text);
            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";Lea";


        }

        public void myForm_Deactivate(object sender, EventArgs e)
        {
            //MessageBox.Show("myForm_Deactivate : " + ((Form)sender).Text);

            //if (Application.OpenForms.Count > 0)
            //    Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";Deact";


        }

        public void myForm_Validating(object sender, EventArgs e)
        {
            //MessageBox.Show("myForm_Validating");
            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";Valing";


        }

        public void myForm_Validated(object sender, EventArgs e)
        {
            //MessageBox.Show("myForm_Validated");
            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";Valted";


        }

        public void myForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //MessageBox.Show("myForm_FormClosed : " + ((Form)sender).Text);
        }

        public void myForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //MessageBox.Show("myForm_FormClosing : " + ((Form)sender).Text);
        }

        #region Form Buttons Click

        public void FormNavMenu_ElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            tToolBox t = new tToolBox();

            #region Tanımlar
            string TableIPCode = string.Empty;
            string ButtonName = string.Empty;
            string Prop_Navigator = string.Empty;
            string Button_Click_Type = string.Empty;
            string myFormLoadValue = string.Empty;

            if (e.Element.ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                ButtonName = ((DevExpress.XtraBars.Navigation.NavButton)e.Element).Name.ToString();

                if (((DevExpress.XtraBars.Navigation.NavButton)e.Element).Tag != null)
                {
                    string myProp = ((DevExpress.XtraBars.Navigation.NavButton)e.Element).Tag.ToString();
                }

                #region FEXIT
                if (ButtonName.IndexOf("FEXIT") > 0)
                {
                    Form tForm = t.Find_Form(sender);

                    if (tForm != null) tForm.Dispose();
                }
                #endregion FEXIT

                /*
                TableIPCode = ((DevExpress.XtraBars.Navigation.NavButton)e.Element).AccessibleName;
                myFormLoadValue = ((DevExpress.XtraBars.Navigation.NavButton)e.Element).AccessibleDescription;

                // AccessibleDescription NEDEN boşalıyor sebebi bulunamadı
                if ((myFormLoadValue == "") &&
                    ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag != null)
                    myFormLoadValue = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag.ToString();

                if (((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag != null)
                    Button_Click_Type = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag.ToString();
                */
            }
            #endregion

            // SET ETME FORMATI
            //btn_FEXIT.ElementClick
            //    += new DevExpress.XtraBars.Navigation.NavElementClickEventHandler(ev.FormNavMenu_ElementClick);
        }

        #region MenuEvents


        #region ToolboxControl_ItemClick

        public void tToolboxControl_ItemClick(object sender, DevExpress.XtraToolbox.ToolboxItemClickEventArgs e)
        {
            using (tToolBox t = new tToolBox())
            {
                #region Tanımlar 
                string TableIPCode = string.Empty;
                string ButtonName = string.Empty;
                string myFormLoadValue = string.Empty;
                #endregion Tanımlar

                ButtonName = ((DevExpress.XtraToolbox.ToolboxItem)e.Item).Name.ToString();
                
                Form tForm = ((DevExpress.XtraToolbox.ToolboxControl)sender).FindForm();

                if (e.Item.Tag != null)
                    myFormLoadValue = e.Item.Tag.ToString();

                ButtonClickRUN(tForm, ButtonName, TableIPCode, myFormLoadValue);
            }
        }

        /// 
        /// Burası dinamik Banka/Şube/Hesaplar listeleri için hazırlanmıştı 
        /// silme 
        /// 
        public void tToolboxControl_ItemClick_Old(object sender, DevExpress.XtraToolbox.ToolboxItemClickEventArgs e)
        {
            if (((DevExpress.XtraToolbox.ToolboxControl)sender).Groups.Count <= 0) return;

            tToolBox t = new tToolBox();

            string selectItemValue = string.Empty;

            if (e.Item.Tag != null)
                selectItemValue = e.Item.Tag.ToString();

            if (t.IsNotNull(selectItemValue) == false) return;

            string Read_TableIPCode = ((DevExpress.XtraToolbox.ToolboxControl)sender).AccessibleName;
            string DetailFName = ((DevExpress.XtraToolbox.ToolboxControl)sender).AccessibleDescription;

            // MessageBox.Show(e.Item.Caption + " : " + value + " : " + Read_TableIPCode+ " : " + DetailFName);

            ((DevExpress.XtraToolbox.ToolboxControl)sender).OptionsView.MenuButtonCaption = e.Item.Caption;

            if (t.IsNotNull(Read_TableIPCode) &&
                t.IsNotNull(DetailFName)
                )
            {
                Form tForm = ((DevExpress.XtraToolbox.ToolboxControl)sender).FindForm();
                DataSet dsRead = null;
                DataNavigator tdN_Read = null;

                t.Find_DataSet(tForm, ref dsRead, ref tdN_Read, Read_TableIPCode);
                int i2 = dsRead.Tables[0].Rows.Count;

                string read_CaptionFName = string.Empty;
                string read_KeyFName = string.Empty;
                string chc_FName = string.Empty;
                string chc_Value = string.Empty;

                string Prop_Navigator = string.Empty;
                string itemCaption = string.Empty;
                string itemValue = string.Empty;
                string itemName = string.Empty;

                // tüm grupları sırayla ele alalım
                foreach (DevExpress.XtraToolbox.ToolboxGroup pGroup in ((DevExpress.XtraToolbox.ToolboxControl)sender).Groups)
                {
                    // MainGroup ise ellenmez, yani ana hesaplar (Banka - Şubeleri)
                    // diğer gruplar ise önce temizleniyor
                    // sonra seçilen hesabın (Banka - Şube nin ) .... açıklamaya devam et ?

                    if (pGroup.Name.IndexOf("item_MainGroup_") == -1)
                    {
                        pGroup.Items.Clear();
                        pGroup.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
                        pGroup.Appearance.Normal.ForeColor = System.Drawing.Color.Black;
                        pGroup.Appearance.Normal.Options.UseFont = true;
                        pGroup.Appearance.Normal.Options.UseForeColor = true;

                        if (pGroup.Tag != null)
                        {
                            Prop_Navigator = pGroup.Tag.ToString();

                            // Group bilgileri için 
                            read_CaptionFName = t.MyProperties_Get(Prop_Navigator, "RTABLEIPCODE:"); // esasında okunan display edilecek field
                            read_KeyFName = t.MyProperties_Get(Prop_Navigator, "RKEYFNAME:");  // esasında okunan ref Id field

                            chc_FName = t.MyProperties_Get(Prop_Navigator, "CHC_FNAME:");  // esasında okunan check field name
                            chc_Value = t.MyProperties_Get(Prop_Navigator, "CHC_VALUE:");  // esasında okunan check value field name

                            for (int i = 0; i < i2; i++)
                            {
                                if ((dsRead.Tables[0].Rows[i][chc_FName].ToString() == chc_Value) &&
                                    (dsRead.Tables[0].Rows[i][DetailFName].ToString() == selectItemValue))
                                {

                                    itemCaption = dsRead.Tables[0].Rows[i][read_CaptionFName].ToString();
                                    itemValue = dsRead.Tables[0].Rows[i][read_KeyFName].ToString();

                                    DevExpress.XtraToolbox.ToolboxItem barButtonItem = new DevExpress.XtraToolbox.ToolboxItem();

                                    itemName = "item_" + itemValue;// refid;

                                    barButtonItem.Name = itemName;
                                    barButtonItem.Caption = itemCaption;
                                    barButtonItem.Tag = itemValue;

                                    barButtonItem.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
                                    barButtonItem.Appearance.Normal.ForeColor = System.Drawing.Color.Black;
                                    barButtonItem.Appearance.Normal.Options.UseFont = true;
                                    barButtonItem.Appearance.Normal.Options.UseForeColor = true;

                                    pGroup.Items.Add(barButtonItem);

                                    pGroup.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
                                    pGroup.Appearance.Normal.ForeColor = System.Drawing.Color.Green;
                                    pGroup.Appearance.Normal.Options.UseFont = true;
                                    pGroup.Appearance.Normal.Options.UseForeColor = true;
                                }
                            }
                        }

                    }
                }

            }

        }
        
        #endregion ToolboxControl_ItemClick
        
        public void tTileItem_ItemClick(object sender, DevExpress.XtraEditors.TileItemEventArgs e)
        {
            using (tToolBox t = new tToolBox())
            {
                #region Tanımlar 
                string TableIPCode = string.Empty;
                string ButtonName = string.Empty;
                string myFormLoadValue = string.Empty;
                string formName = string.Empty;
                #endregion Tanımlar

                ButtonName = ((DevExpress.XtraEditors.TileItem)sender).Name.ToString();

                DevExpress.XtraEditors.TileGroup pGroup = ((DevExpress.XtraEditors.TileItem)sender).Group;

                //DevExpress.XtraEditors.ITileControl tileControl = pGroup.Control;
                //Form tForm =  tileControl   //.FindForm();

                if (pGroup.Tag != null)
                    formName = pGroup.Tag.ToString();
                    //((DevExpress.XtraEditors.TileItem)sender).Appearance.Name.ToString();
                Form tForm = Application.OpenForms[formName];


                if (((DevExpress.XtraEditors.TileItem)sender).Tag != null)
                    myFormLoadValue = ((DevExpress.XtraEditors.TileItem)sender).Tag.ToString();

                ButtonClickRUN(tForm, ButtonName, TableIPCode, myFormLoadValue);
            }
        }

        public void tNavBarItem_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            tToolBox t = new tToolBox();
            //tEvents ev = 
            #region Tanımlar 
            string TableIPCode = string.Empty;
            string ButtonName = string.Empty;
            string myFormLoadValue = string.Empty;
            #endregion Tanımlar

            ButtonName = ((DevExpress.XtraNavBar.NavBarItem)sender).Name.ToString();
            ButtonName = e.Link.ItemName.ToString();

            if (e.Link.Item.Tag != null)
                myFormLoadValue = e.Link.Item.Tag.ToString();

            Form tForm = ((DevExpress.XtraNavBar.NavBarItem)sender).NavBar.FindForm();

            ButtonClickRUN(tForm, ButtonName, TableIPCode, myFormLoadValue);

            //MessageBox.Show(ButtonName+"//"+tForm.Name);
        }

        public void tAccordionControlElement_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            #region Tanımlar 
            string TableIPCode = string.Empty;
            string ButtonName = string.Empty;
            string myFormLoadValue = string.Empty;
            #endregion Tanımlar

            ButtonName = ((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Name.ToString();
            Form tForm = ((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).AccordionControl.FindForm();

            if (((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Tag != null)
                myFormLoadValue = ((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Tag.ToString();

            ButtonClickRUN(tForm, ButtonName, TableIPCode, myFormLoadValue);

            //MessageBox.Show(ButtonName + "//" + tForm.Name);
        }

        public void tNavButton_ElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            tToolBox t = new tToolBox();

            #region Tanımlar 
            string TableIPCode = string.Empty;
            string ButtonName = string.Empty;
            string myFormLoadValue = string.Empty;
            #endregion Tanımlar

            ButtonName = ((DevExpress.XtraBars.Navigation.NavButton)sender).Name.ToString();

            string formName = string.Empty;

            if (((DevExpress.XtraBars.Navigation.NavButton)sender).Appearance.Name != null)
            {
                formName = ((DevExpress.XtraBars.Navigation.NavButton)sender).Appearance.Name.ToString();

                Form tForm = Application.OpenForms[formName];

                //  Form tForm = ((DevExpress.XtraBars.Navigation.TileNavElement)sender).TileNavPane.FindForm();

                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Tag != null)
                    myFormLoadValue = ((DevExpress.XtraBars.Navigation.NavButton)sender).Tag.ToString();

                ButtonClickRUN(tForm, ButtonName, TableIPCode, myFormLoadValue);
            }
        }

        public void FormMenu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tToolBox t = new tToolBox();

            #region Tanımlar 
            string TableIPCode = string.Empty;
            string ButtonName = string.Empty;
            //string Button_Click_Type = string.Empty;
            string myFormLoadValue = string.Empty;
            #endregion Tanımlar


            if (e.Item.ToString() == "DevExpress.XtraBars.BarLargeButtonItem")
            {
                ButtonName = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Name.ToString();
                TableIPCode = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).AccessibleName;
                myFormLoadValue = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).AccessibleDescription;

                // AccessibleDescription NEDEN boşalıyor sebebi bulunamadı
                if ((t.IsNotNull(myFormLoadValue) == false) &&
                    ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag != null)
                    myFormLoadValue = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag.ToString();

                //if (((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag != null)
                //    Button_Click_Type = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag.ToString();
            }

            if (e.Item.ToString() == "DevExpress.XtraBars.BarButtonItem")
            {
                ButtonName = ((DevExpress.XtraBars.BarButtonItem)e.Item).Name.ToString();
                TableIPCode = ((DevExpress.XtraBars.BarButtonItem)e.Item).AccessibleName;
                myFormLoadValue = ((DevExpress.XtraBars.BarButtonItem)e.Item).AccessibleDescription;

                // AccessibleDescription NEDEN boşalıyor sebebi bulunamadı
                if ((t.IsNotNull(myFormLoadValue) == false) &&
                    ((DevExpress.XtraBars.BarButtonItem)e.Item).Tag != null)
                    myFormLoadValue = ((DevExpress.XtraBars.BarButtonItem)e.Item).Tag.ToString();

                //if (((DevExpress.XtraBars.BarButtonItem)e.Item).Tag != null)
                //    Button_Click_Type = ((DevExpress.XtraBars.BarButtonItem)e.Item).Tag.ToString();
            }

            Form tForm = t.Find_Form(sender);

            ButtonClickRUN(tForm, ButtonName, TableIPCode, myFormLoadValue);

        }

        #endregion MenuEvents

        public void ButtonClickRUN(Form tForm, string ButtonName, string TableIPCode, string myFormLoadValue)
        {
            tToolBox t = new tToolBox();
            string Prop_Navigator = string.Empty;

            #region Run_PropNavigator
            if (ButtonName.IndexOf("item_") > 0)
            {
                MessageBox.Show("Run_PropNavigator yazılacak...");   
            }
            #endregion 


            #region FLIST_IP
            if (ButtonName.IndexOf("FLIST_IP") > 0)
            {
                if (tForm != null)
                    tSubView_Preparing(tForm, "TabPage2", "", TableIPCode, "", "", "");
            }
            #endregion FLIST_IP

            #region FOPEN_IP, FNEW
            if ((ButtonName.IndexOf("FNEW_IP") > 0) ||
                (ButtonName.IndexOf("FOPEN_IP") > 0) ||
                (ButtonName.IndexOf("FOPEN_SUBVIEW") > 0)
                )
            {
                Control cntrl = null;

                if (t.IsNotNull(TableIPCode))
                    cntrl = t.Find_Control_View(tForm, TableIPCode);

                if ((cntrl != null) && t.IsNotNull(TableIPCode))
                {
                    if (cntrl.ToString() == "DevExpress.XtraGrid.GridControl")
                    {
                        //GridView tView = ((DevExpress.XtraGrid.GridControl)cntrl).MainView as GridView;

                        //if (tView.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
                        //{
                        //    if ((((DevExpress.XtraGrid.Views.Grid.GridView)tView).GridControl).AccessibleDescription != null)
                        //    Prop_Navigator = (((DevExpress.XtraGrid.Views.Grid.GridView)tView).GridControl).AccessibleDescription;
                        //}
                        //if (tView.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
                        //{
                        //    Prop_Navigator = (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)tView).GridControl).AccessibleDescription;
                        //}

                        if (((GridControl)cntrl).AccessibleDescription != null)
                            Prop_Navigator = ((GridControl)cntrl).AccessibleDescription.ToString();
                    }

                    if (cntrl.ToString() == "DevExpress.XtraTreeList.TreeList")
                    {
                        if (((DevExpress.XtraTreeList.TreeList)cntrl).AccessibleDescription != null)
                            Prop_Navigator = ((DevExpress.XtraTreeList.TreeList)cntrl).AccessibleDescription.ToString();
                    }


                }//if (cntrl != null)

                if ((cntrl == null) || t.IsNotNull(TableIPCode) == false)
                {
                    if (t.IsNotNull(myFormLoadValue))
                        Prop_Navigator = myFormLoadValue;
                }

                // 51 - kart + form
                if (t.IsNotNull(Prop_Navigator))
                {
                    tForm.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                    if (ButtonName.IndexOf("FOPEN_IP") > 0)
                    {
                        navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_51_Karti_Ac);
                    }
                    if (ButtonName.IndexOf("FNEW_IP") > 0)
                    {
                        Prop_Navigator = Prop_Navigator.Replace((char)34, (char)39);
                        if (Prop_Navigator.IndexOf("'BUTTONTYPE': '52'") > -1)
                            navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_52_Yeni_Kart_FormIle);
                        if (Prop_Navigator.IndexOf("'BUTTONTYPE': '53'") > -1)
                            navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_53_Yeni_Hesap);
                    }
                    if (ButtonName.IndexOf("FOPEN_SUBVIEW") > 0)
                    {
                        navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_59_SubView_Open);
                    }

                    tForm.Cursor = System.Windows.Forms.Cursors.Default;
                }// 51
            }
            #endregion FOPEN_IP, FNEW_IP

            #region FSAVE
            if (ButtonName.IndexOf("FSAVE") > 0)
            {
                if (tForm != null) AutoSave(tForm);
            }
            #endregion FSAVE

            #region FSAVEXIT
            if (ButtonName.IndexOf("FSAVEXIT") > 0)
            {
                if (tForm != null)
                {
                    AutoSave(tForm);
                    tForm.Dispose();
                }
            }
            #endregion FSAVEXIT

            #region FSAVENEW
            if (ButtonName.IndexOf("FSAVENEW") > 0)
            {
                if (tForm != null) AutoSave(tForm);
            }
            #endregion FSAVENEW

            #region FEXIT
            if (ButtonName.IndexOf("FEXIT") > 0)
            {
                if (tForm != null) tForm.Dispose();
            }
            #endregion FEXIT

            #region APPEXIT
            if (ButtonName.IndexOf("APPEXIT") > 0)
            {
                t.AllFormsClose();

                Application.Exit();
            }
            #endregion APPEXIT

            #region REPORTVIEW
            if (ButtonName.IndexOf("REPORTVIEW") > 0)
            {
                string RaporHesapKodu = t.MyProperties_Get(myFormLoadValue, "RaporHesapKodu:");
                string KonuOlan_TableIPCode = t.MyProperties_Get(myFormLoadValue, "KonuOlan_TableIPCode:");
                string KonuOlan_ID = t.MyProperties_Get(myFormLoadValue, "KonuOlan_ID:");
                string KonuOlanaAit_Liste_TableIPCode = t.MyProperties_Get(myFormLoadValue, "KonuOlanaAit_Liste_TableIPCode:");
                string Parametre = t.MyProperties_Get(myFormLoadValue, "Parametre:");

                v.con_FormLoadValue = string.Empty;
                v.con_FormLoadValue = Preparing_REPORTVIEW(RaporHesapKodu, KonuOlan_TableIPCode, KonuOlan_ID, KonuOlanaAit_Liste_TableIPCode, Parametre);
                tForms fr = new tForms();
                Form tNewForm = null;
                tNewForm = fr.Get_Form("MSReportView");
                t.ChildForm_View(tNewForm, Application.OpenForms[0], FormWindowState.Maximized, v.con_FormLoadValue); //myFormLoadValue);
                //t.ChildForm_View(new MSReportView() , Application.OpenForms[0] , FormWindowState.Maximized, myFormLoadValue);
            }
            #endregion REPORTVIEW

            #region SMS_SEND
            if (ButtonName.IndexOf("SMS_SEND") > 0)
            {
                //Form SourceForm = t.Find_Form(sender);

                //TableIPCode üzeribnde gelen bilgi SMS_SABLOND.BASLIK_KODU fieldini işaret ediyor

                v.con_FormLoadValue = v.mySMSViewFormLoadValue;
                t.MyProperties_Set(ref v.con_FormLoadValue, v.FormLoad, "TargetLikeValue", TableIPCode);
                t.MyProperties_Set(ref v.con_FormLoadValue, v.FormLoad, "END", string.Empty);

                if (tForm != null) //SourceForm
                {
                    tForms fr = new tForms();
                    Form tNewForm = fr.Get_Form("tn_SMSGonder", tForm); //SourceForm

                    //t.DialogForm_View(tn_SMSGonder(this), FormWindowState.Maximized);
                    t.ChildForm_View(tNewForm, Application.OpenForms[0]);
                }
            }
            #endregion SMS_SEND

            #region SMS_SETUP
            if (ButtonName.IndexOf("SMS_SETUP") > 0)
            {
                //Form SourceForm = t.Find_Form(sender);

                //TableIPCode üzeribnde gelen bilgi SMS_SABLOND.BASLIK_KODU fieldini işaret ediyor

                v.con_FormLoadValue = v.mySMSSetupFormLoadValue;
                t.MyProperties_Set(ref v.con_FormLoadValue, v.FormLoad, "TargetLikeValue", TableIPCode);
                t.MyProperties_Set(ref v.con_FormLoadValue, v.FormLoad, "END", string.Empty);

                if (tForm != null) // SourceForm
                {
                    tForms fr = new tForms();
                    Form tNewForm = fr.Get_Form("tn_SMSSablonuTanimla", tForm); // SourceForm

                    t.ChildForm_View(tNewForm, Application.OpenForms[0]);
                }
            }
            #endregion FSMS_SEND

        }

        private string Preparing_REPORTVIEW(string RaporHesapKodu, string KonuOlan_TableIPCode, string KonuOlan_ID,
                string KonuOlanaAit_Liste_TableIPCode, string Parametre)
        {
            tToolBox t = new tToolBox();
            // -------------------------------------------------------------
            // ReportView açıldığı zaman okunacak raporlar
            // Select a.* 
            // from MS_REPORTS a
            // where 0 = 0 
            // and a.REPORT_CODE like 'KRS%' 
            string myFormLoadValue = v.myReportViewFormLoadValue;
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "TargetLikeValue", RaporHesapKodu);
            // -------------------------------------------------------------
            // ReportView açıldığı Konu Olan IP 
            // Konu olan Panelinde view edilecek IP (InputPanel) bilgiler set edilir
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "AddIP_Form", "MSREPORTVIEW");
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "AddIP_TableIPCode", KonuOlan_TableIPCode);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "AddIP_Value", KonuOlan_ID);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "AddListIP_TableIPCode", KonuOlanaAit_Liste_TableIPCode);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "Parametre", Parametre);  // false or true
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "END", string.Empty);

            //v.FormLoad = myFormLoadValue;

            return myFormLoadValue;

            // İşlemin Devamı  
            // btn_REPORTVIEW.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ev.FormMenu_ItemClick);
            // ev.FormMenu_ItemClick >> event te devam ediyor
        }

        public void AutoSave(Form tForm)
        {
            // Form on All DataSet Save
            v.SQLSave = string.Empty;
            //tDataNavigatorList(tForm, "tDataSave");

            vSubWork vSW = new vSubWork();
            vSW._01_tForm = tForm;
            vSW._02_TableIPCode = "";
            vSW._03_WorkTD = v.tWorkTD.Save;
            vSW._04_WorkWhom = v.tWorkWhom.All;
            tSubWork_(vSW);
        }

        #endregion Form Buttons Click


        #endregion Form Events Functions

        #region tDataNavigator / PositionChanged

        public void tdataNavigator_PositionChanged(object sender, EventArgs e)
        {
            #region Giriş kontrolleri
            // LKP_ONAY için ise işlem olmasın
            if ((v.con_OnayChange) || (v.con_PositionChange)) return;
            // 
            if (v.con_Cancel == true)
            {
                v.con_Cancel = false;
                return;
            }

            #endregion

            tToolBox t = new tToolBox();

            string function_name = "tdataNavigator_PositionChanged";
            t.Takipci(function_name, "", '{');

            int NewPosition = ((DevExpress.XtraEditors.DataNavigator)sender).Position;
            object tDataTable = ((DevExpress.XtraEditors.DataNavigator)sender).DataSource;
            DataSet dsData = ((DataTable)tDataTable).DataSet;

            //((DataTable)tDataTable).Columns[""].Expression

            //dsData.Tables[0].Columns[""].Expression

            #region Detail-SubDetail Table
            // Kendisine bağlı subdetail var ise
            if (((DevExpress.XtraEditors.DataNavigator)sender).IsAccessible == true)
            {
                if (((DevExpress.XtraEditors.DataNavigator)sender).Position > -1)
                {
                    Form tForm = t.Find_Form(sender);
                    string TableIPCode = dsData.DataSetName;

                    // eski süreç
                    //
                    Data_Refresh(tForm, dsData, ((DevExpress.XtraEditors.DataNavigator)sender));

                    // yenisi bir türlü tree de başarılı olmadı sebebini bulamadım

                    /// Yeni Süreç ----
                    /// 

                    //vSubWork vSW = new vSubWork();
                    //vSW._01_tForm = tForm;
                    //vSW._02_TableIPCode = "";
                    //vSW._03_WorkTD = v.tWorkTD.Refresh;
                    //vSW._04_WorkWhom = v.tWorkWhom.All;
                    //tSubWork_(vSW);

                }

            }
            #endregion Detail-SubDetail Table

            // yeni kayıt ise
            if (v.con_NewRecords)
            {
                v.con_NewRecords = false;
                ((DevExpress.XtraEditors.DataNavigator)sender).Tag = ((DevExpress.XtraEditors.DataNavigator)sender).Position;
                return;
            }

            #region HasChanges
            if ((dsData.HasChanges()) &&
                (v.con_LkpOnayChange == false)) // silme işlemi değil ise
            {
                object x = ((DevExpress.XtraEditors.DataNavigator)sender).Tag;

                int OldPosition = ((int)x);

                // -97 Delete işlemine işaret ediyor ( Rows[position].Delete() )

                if ((OldPosition != -97) && // Değil ise 
                    (OldPosition > -1))
                {
                    string TableIPCode = dsData.DataSetName;

                    Form tForm = t.Find_Form(sender);

                    tDefaultValue df = new tDefaultValue();
                    if (df.tDefaultValue_And_Validation(
                            tForm,
                            dsData,
                            dsData.Tables[0].Rows[OldPosition],
                            TableIPCode,
                            function_name) == true) // Table Save 
                    {

                        tSave sv = new tSave();

                        sv.tDataSave(tForm, dsData, ((DevExpress.XtraEditors.DataNavigator)sender), OldPosition);

                        v.SQL = v.ENTER + "tdataNavigator_PositionChanged [ " + TableIPCode + " ]" + v.SQL;
                        try
                        {
                            // sebebini anlamadım, fakat yukarıda tTable_Update functiona gidince orada 
                            // ds.Tables[0].AcceptChanges(); komutundan sonra position değerini unutuyor onun için  
                            // buraya geldiği sıradaki positon değeri kendisine tekrar hatırlatılıyor.

                            // ve bu yeniden hatırlatma eğer table Insert değil ise yani Edit state modunda ise yapılıyor
                            string ix = dsData.Tables[0].Rows[OldPosition][0].ToString();
                            if (ix != "")
                                ((DevExpress.XtraEditors.DataNavigator)sender).Position = NewPosition;
                        }
                        catch
                        {
                            // burada hata olarak yakalanır
                        }
                    }
                    else
                    {
                        // işlem onaylanmadı ise
                        t.ButtonEnabledAll(tForm, TableIPCode, true);

                        v.Kullaniciya_Mesaj_Var = "İPTAL : Kayıt işleminden vazgeçildi ....";
                    }

                }


            }
            #endregion HasChanges

            ((DevExpress.XtraEditors.DataNavigator)sender).Tag = ((DevExpress.XtraEditors.DataNavigator)sender).Position;

            dsData.Dispose();

            t.Takipci(function_name, "", '}');

        }

        #endregion tDataNavigator / PositionChanged

        #region tDataNavigator / ButtonClick Events

        public void tdataNavigator_ButtonClick(object sender, NavigatorButtonClickEventArgs e)
        {
            if (e.Button.ButtonType.ToString() == "EndEdit")
            {
                tToolBox t = new tToolBox();

                object tDataTable = ((DevExpress.XtraEditors.DataNavigator)sender).DataSource;
                DataSet dsData = ((DataTable)tDataTable).DataSet;

                object x = ((DevExpress.XtraEditors.DataNavigator)sender).Tag;
                int Old_ID = ((int)x);



                if (e.Button.ButtonType.ToString() == "EndEdit")
                {
                    /// bu şarta gerek duyarsan 
                    /// NavigatorButton btnEnd = dN_MSFieldsIP.Buttons.EndEdit; çalıştırdğın satırdan önce
                    /// dN_MSFieldsIP.Tag = dN_MSFieldsIP.Position;   satırını yerleştirin
                    /// 
                    ////if (
                    ////(((DevExpress.XtraEditors.DataNavigator)sender).Position > -1) &&
                    ////(((DevExpress.XtraEditors.DataNavigator)sender).Position > Rec_ID)
                    ////)
                    ////    Old_ID = ((DevExpress.XtraEditors.DataNavigator)sender).Position;

                    tSave sv = new tSave();
                    Form tForm = t.Find_Form((DataNavigator)sender);
                    sv.tDataSave(tForm, dsData, ((DevExpress.XtraEditors.DataNavigator)sender), Old_ID);
                }

                dsData.Dispose();
            }

            //e.Button.ButtonType == NavigatorButtonType.Edit
            if (e.Button.ButtonType.ToString() == "Append")
            {
                // Append İşlemini durdur
                if (v.con_Cancel) e.Handled = true;
            }

            // hangi butona basıldığının bilgisini saklıyor
            // text e tDetail_SubDetail_Read bilgisi atılıyor onun için başka bir properties bul eğer gerekirse

            //((DevExpress.XtraEditors.DataNavigator)sender).Text = e.Button.ButtonType.ToString();
        }

        public void barButtonItem_NavigotorItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tToolBox t = new tToolBox();

            string TableIPCode = string.Empty;
            string ButtonName = string.Empty;
            string Caption = string.Empty;
            string Prop_Navigator = string.Empty;
            string Button_Click_Type = string.Empty;

            if (e.Item.ToString() == "DevExpress.XtraBars.BarLargeButtonItem")
            {
                ButtonName = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Name.ToString();
                Caption = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Caption;
                TableIPCode = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).AccessibleName;
                if (((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag != null)
                    Button_Click_Type = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag.ToString();
            }

            if (e.Item.ToString() == "DevExpress.XtraBars.BarButtonItem")
            {
                ButtonName = ((DevExpress.XtraBars.BarButtonItem)e.Item).Name.ToString();
                Caption = ((DevExpress.XtraBars.BarButtonItem)e.Item).Caption;
                TableIPCode = ((DevExpress.XtraBars.BarButtonItem)e.Item).AccessibleName;
                if (((DevExpress.XtraBars.BarButtonItem)e.Item).Tag != null)
                    Button_Click_Type = ((DevExpress.XtraBars.BarButtonItem)e.Item).Tag.ToString();
            }

            // simpleButton_  ifadesi isimden siliniyor
            ButtonName = ButtonName.Substring(13, ButtonName.Length - 13);

            Prop_Navigator = "BarButtonItem";

            // 1. formu tespit edilir
            Form tForm = t.Find_Form(sender);

            btn_Navigotor_Click(tForm, null, TableIPCode, ButtonName, Caption, Prop_Navigator, Button_Click_Type);
        }

        public void btn_Navigotor_Enter(object sender, EventArgs e)
        {
            ((DevExpress.XtraEditors.SimpleButton)sender).BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
        }

        public void btn_Navigotor_Leave(object sender, EventArgs e)
        {
            ((DevExpress.XtraEditors.SimpleButton)sender).BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
        }

        public void dataNavigator_Enter(object sender, EventArgs e)
        {
            //((DevExpress.XtraEditors.DataNavigator)sender).BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            ((DevExpress.XtraEditors.DataNavigator)sender).Appearance.Options.UseBackColor = true;
        }

        public void dataNavigator_Leave(object sender, EventArgs e)
        {
            //((DevExpress.XtraEditors.DataNavigator)sender).BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            ((DevExpress.XtraEditors.DataNavigator)sender).Appearance.Options.UseBackColor = false;
        }


        public void btn_Navigotor_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            string Button_Click_Type = ((DevExpress.XtraEditors.SimpleButton)sender).TabIndex.ToString();
            string ButtonName = ((DevExpress.XtraEditors.SimpleButton)sender).Name.ToString();
            string Caption = ((DevExpress.XtraEditors.SimpleButton)sender).Text;

            // simpleButton_  ifadesi isimden siliniyor
            ButtonName = ButtonName.Substring(13, ButtonName.Length - 13);

            string TableIPCode = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;

            string Prop_Navigator = string.Empty;
            string Prop_Search = string.Empty;

            if (((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDescription != null)
            {
                Prop_Navigator = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDescription.ToString();
            }

            if (((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDefaultActionDescription != null)
            {
                //  "simpleButton_arama" butonunda search bilgileri varsa 
                //   
                Prop_Search = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDefaultActionDescription.ToString();

                if (t.IsNotNull(Prop_Search))
                    Prop_Navigator = Prop_Search;
            }


            // 1. formu tespit edilir
            Form tForm = t.Find_Form(sender);

            btn_Navigotor_Click(tForm, sender, TableIPCode, ButtonName, Caption, Prop_Navigator, Button_Click_Type);

        }

        public void btn_Navigotor_Click(Form tForm, object sender,
                    string TableIPCode,
                    string ButtonName,
                    string Caption,
                    string Prop_Navigator,
                    string Button_Click_Type)
        {
            tToolBox t = new tToolBox();
            string function_name = "btn_Navigator_Click";
            t.Takipci(function_name, "", '{');

            #region işlevsel butonlar

            if ((ButtonName == "cikis") ||       // 11
                (ButtonName == "iptal_navg") ||  // 28
                (ButtonName == "iptal_form")     // 29
                )
            {
                t.Find_DataNavigator_ButtonLink(tForm, TableIPCode, Button_Click_Type);
            }

            if (ButtonName == "listele")  // 12
            {
                DataSet ds = t.Find_DataSet(tForm, "", TableIPCode, function_name + "/listele");
                if (ds != null)
                {
                    t.TableRefresh(tForm, ds, TableIPCode);
                    
                    //
                    t.ViewControl_Enabled(tForm, ds, TableIPCode);

                    // bu IPCode bağlı ExternalIPCode olabilir...
                    t.ViewControl_Enabled_ExtarnalIP(tForm, ds);

                }
            }

            // 13 Search value set
            if (ButtonName == "sec")  
            {
                v.searchSet = true;
                t.TableRowGet(tForm, TableIPCode);
                tForm.Dispose();
            }
            // 14 Sepet içinde Listeye ekle
            if (ButtonName == "ekle")  // 14
            {
                navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_14_Ekle);
            }
            // 15 Sepeti aç
            if (ButtonName == "arama")  // 15
            {
                string s2 = (char)34 + "TABLEIPCODE_LIST" + (char)34 + ": [";
                if (Prop_Navigator.IndexOf(s2) > -1)
                {
                    // refresh 
                    tForm.HelpButton = true;
                    //
                    navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_15_Arama);
                }
            }

            if (ButtonName == "sihirbaz_devam")
            {
                if (Caption == "Kaydet")
                {
                    /// devam özelliğinden çıkıp, kaydet özelliği çalışsın diye 
                    ButtonName = "kaydet";
                }
                else
                    t.NextPage(tForm, TableIPCode);
            }

            if (ButtonName == "sihirbaz_geri")
            {
                t.PrevPage(tForm, TableIPCode);
            }

            if (ButtonName == "sihirbaz_sonra")
            {
                t.NextPage(tForm, TableIPCode);
            }
            
            if (ButtonName == "kaydet_cik")   // 22
            {
                tSave sv = new tSave();
                if (sv.tDataSave(tForm, TableIPCode))
                {
                    tForm.Dispose();
                }
            }

            if (ButtonName == "kaydet_yeni") // 23
            {
                tSave sv = new tSave();
                if (sv.tDataSave(tForm, TableIPCode))
                {
                    t.ButtonEnabledAll(tForm, TableIPCode, true);
                    tNewData(tForm, TableIPCode);
                }
            }

            if (ButtonName == "kaydet")   // 24
            {
                /// kaydet butonuna extra yük atılmış 
                /// bir kaydet butonu tuşu çalıştırıldığında, 
                /// kendisinden önce başka IP lerde kayıt ettirilebilir
                if (t.IsNotNull(Prop_Navigator))
                {
                    // "BUTTONTYPE": "24"";
                    string s2 = (char)34 + "BUTTONTYPE" + (char)34 + ": " + (char)34 + "24";
                    if ((Prop_Navigator.IndexOf("BUTTONTYPE:24") > 1) ||
                        (Prop_Navigator.IndexOf(s2) > 1))
                    {
                        v.con_SubWork_Refresh = false;

                        navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_24_Kaydet);
                    }
                }

                /// şimdi basılan butonun kaydını
                tSave sv = new tSave();
                if (sv.tDataSave(tForm, TableIPCode))
                {
                    t.ButtonEnabledAll(tForm, TableIPCode, true);
                    // 
                    v.con_SubWork_Refresh = true;
                }

            }

            if (ButtonName == "kaydet_devam")   // 25
            {
                tSave sv = new tSave();

                if (sv.tDataSave(tForm, TableIPCode))
                {
                    t.NextPage(tForm, TableIPCode);
                    t.ButtonEnabledAll(tForm, TableIPCode, true);
                    t.tFormActiveView(tForm, TableIPCode);
                }
            }

            if (ButtonName == "sil_satir")   // 26
            {
                SatirSil(tForm, TableIPCode);
            }

            if (ButtonName == "sil_fis")   // 27
            {
                //t.Find_MOS("Win32_Processor","Name");
                //t.Find_MOS("Win32_DiskDrive", "Name");
                //t.Find_MOS("Win32_NetworkAdapter", "Name");
                //t.Find_MOS_MacAddr();


            }

            if (ButtonName == "hesap_ac")   // 50 - hesap aç
            {
                navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_50_Hesap_Ac);
            }

            if (ButtonName == "kart_ac")   // 51 - kart + form
            {
                navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_51_Karti_Ac);
            }

            //simpleButton_yeni_kart_ac.Name = "simpleButton_yeni_kart_ac";
            if (ButtonName == "yeni_kart_ac")   // 52 - kart + form
            {
                navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_52_Yeni_Kart_FormIle);
            }

            //simpleButton_yeni_hesap.Name = "simpleButton_yeni_hesap";
            //simpleButton_yeni_alt_hesap.Name = "simpleButton_yeni_alt_hesap";
            #region // 53 - yeni_hesap || 54 - yeni_alt_hesap 
            if (((ButtonName == "yeni_hesap") ||       // 53 - yeni hesap
                 (ButtonName == "yeni_alt_hesap")) &&  // 54 - yeni alt hesap
                (Caption.IndexOf("Yeni") > -1)
                )
            {
                /// yeni butonuna extra yük atılmış 
                /// bir yeni butonu tuşu çalıştırıldığında, 
                /// kendisinden önce başka IP lerde yeni için hazırlanabilir
                if (t.IsNotNull(Prop_Navigator))
                {
                    string s1 = "BUTTONTYPE:53";
                    string s2 = (char)34 + "BUTTONTYPE" + (char)34 + ": " + (char)34 + "53" + (char)34;

                    if ((Prop_Navigator.IndexOf(s1) > -1) ||
                        (Prop_Navigator.IndexOf(s2) > -1))
                    {
                        v.con_PositionChange = true;
                        v.con_ExtraChange = true;

                        navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_53_Yeni_Hesap);
                    }
                }

                string Key_FName = t.MyProperties_Get(Prop_Navigator, "KEY_FNAME:");
                string Master_Key_FName = t.MyProperties_Get(Prop_Navigator, "MASTER_KEY_FNAME:");
                string Foreing_FName = t.MyProperties_Get(Prop_Navigator, "FOREING_FNAME:");
                string Parent_FName = t.MyProperties_Get(Prop_Navigator, "PARENT_FNAME:");

                // yeni_alt_hesap
                if (t.IsNotNull(Parent_FName))
                {
                    if (t.IsNotNull(Master_Key_FName))
                        Key_FName = Master_Key_FName;

                    string UstHesapValue = Find_ParentValue(tForm, TableIPCode, Key_FName, Parent_FName, ButtonName);

                    tNewData(tForm, TableIPCode, Key_FName, Parent_FName, UstHesapValue);
                }
                else // yeni_hesap
                {
                    tNewData(tForm, TableIPCode);
                }

                v.con_PositionChange = false;
                v.con_ExtraChange = false;
            }
            #endregion 

            #region // Vazgeç işlemi (yeni_hesap) 
            if (((ButtonName == "yeni_hesap") ||       // 53 - yeni hesap/kart
                 (ButtonName == "yeni_alt_hesap")) &&  // 54 - kart veya form
                (Caption.IndexOf("Vazgeç") > -1)
                )
            {
                /// 
                tCancelData(tForm, sender, TableIPCode);

                if ((ButtonName == "yeni_hesap") &&
                    (t.IsNotNull(Prop_Navigator)))
                {
                    string s1 = "BUTTONTYPE:53";
                    string s2 = (char)34 + "BUTTONTYPE" + (char)34 + ": " + (char)34 + "53" + (char)34;

                    if ((Prop_Navigator.IndexOf(s1) > -1) ||
                        (Prop_Navigator.IndexOf(s2) > -1))
                    {
                        v.con_PositionChange = true;
                        v.con_ExtraChange = true;

                        navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_53_Yeni_Hesap_Vazgec);

                        v.con_PositionChange = false;
                        v.con_ExtraChange = false;

                        v.con_NewRecords = false;
                    }
                }

                vSubWork vSW = new vSubWork();
                vSW._01_tForm = tForm;
                vSW._02_TableIPCode = TableIPCode;
                vSW._03_WorkTD = v.tWorkTD.Refresh_SubDetail;
                vSW._04_WorkWhom = v.tWorkWhom.Childs;
                tSubWork_(vSW);

                //tCancelData(tForm, sender, TableIPCode);

            }
            #endregion

            /// yeni düzende gerek yok
            if (ButtonName == "fisi_ac")   // 56 - fiş + form
            {
                t.OpenForm(tForm, Prop_Navigator);
            }

            if (ButtonName == "yeni_fis_ac")   // 57 - fiş + form
            {
                tNewForm_NewData(tForm, Prop_Navigator);
            }

            if (ButtonName == "yeni_fis")   // 58 - fiş
            {
                tNewData(tForm, TableIPCode);

                if (sender != null)
                    ((DevExpress.XtraEditors.SimpleButton)sender).Enabled = false;
            }

            if (ButtonName == "vazgec")   // 21  ( gerek kalmadı ilerde başka bir iş için kullanabilirsin )
            {
                DataNavigator tDataNavigator = t.Find_DataNavigator(tForm, TableIPCode);//, function_name + "/vazgec");
                NavigatorButton btn = tDataNavigator.Buttons.CancelEdit;
                tDataNavigator.Buttons.DoClick(btn);
            }

            #endregion işlevsel butonlar

            #region yön butonları

            if (ButtonName == "en_sona")
            {
                DataNavigator tDataNavigator = t.Find_DataNavigator(tForm, TableIPCode);//, function_name + "/en_son");
                NavigatorButton btn = tDataNavigator.Buttons.Last;
                tDataNavigator.Buttons.DoClick(btn);
            }

            if (ButtonName == "sonraki_syf")
            {
                DataNavigator tDataNavigator = t.Find_DataNavigator(tForm, TableIPCode);//, function_name + "/sonraki_syf");
                NavigatorButton btn = tDataNavigator.Buttons.NextPage;
                tDataNavigator.Buttons.DoClick(btn);
            }

            if (ButtonName == "sonraki")
            {
                DataNavigator tDataNavigator = t.Find_DataNavigator(tForm, TableIPCode);//, function_name + "/sonraki");
                NavigatorButton btn = tDataNavigator.Buttons.Next;
                tDataNavigator.Buttons.DoClick(btn);
            }

            if (ButtonName == "onceki")
            {
                DataNavigator tDataNavigator = t.Find_DataNavigator(tForm, TableIPCode);//, function_name + "/onceki");
                NavigatorButton btn = tDataNavigator.Buttons.Prev;
                tDataNavigator.Buttons.DoClick(btn);
            }

            if (ButtonName == "onceki_syf")
            {
                DataNavigator tDataNavigator = t.Find_DataNavigator(tForm, TableIPCode);//, function_name + "/onceki_syf");
                NavigatorButton btn = tDataNavigator.Buttons.PrevPage;
                tDataNavigator.Buttons.DoClick(btn);
            }

            if (ButtonName == "en_basa")
            {
                DataNavigator tDataNavigator = t.Find_DataNavigator(tForm, TableIPCode);//, function_name + "/en_basa");
                NavigatorButton btn = tDataNavigator.Buttons.First;
                tDataNavigator.Buttons.DoClick(btn);
            }

            #endregion yön butonları

            #region onay butonları
            if ((ButtonName == "onayla") | (ButtonName == "onay_iptal"))
            {
                OnayIslemi(tForm, TableIPCode, ButtonName);
            }
            #endregion onay butonları

            #region collapse/expand butonları
            //if ((ButtonName == "Collapse") | (ButtonName == "Expand"))
            if (ButtonName == "CollExp")
            {
                if (((DevExpress.XtraEditors.CheckButton)sender).Checked)
                {
                    ButtonName = "Collapse";
                    ((DevExpress.XtraEditors.CheckButton)sender).Text = "A";
                }
                else
                {
                    ButtonName = "Expand";
                    ((DevExpress.XtraEditors.CheckButton)sender).Text = "K";
                }

                CollExpIslemi(tForm, TableIPCode, ButtonName);
            }
            #endregion collapse/expand butonları

            #region yazici butonları
            if (ButtonName == "yazici")
            {
                YaziciIslemi(tForm, TableIPCode);
            }
            #endregion onay butonları

            t.Takipci(function_name, "", '}');

        }

        #region navigatorButtonExec_
        
        public void navigatorButtonExec_Keys(Form tForm, Keys tKeys, string TableIPCode, string Prop_Navigator)
        {
            tToolBox t = new tToolBox();

            /// simpleButton_sec     veya
            /// simpleButton_ekle    butonu varmı kontrol edilecek ve 
            /// butona göre işlem yapılacak
            /// 
            string[] controls = new string[] { };
            Control btn = null;

            if ((tKeys == Keys.Return) | (tKeys == Keys.Enter))
            {
                #region _sec
                btn = t.Find_Control(tForm, "simpleButton_sec", TableIPCode, controls);

                /// Seç butonu ise
                if (btn != null)
                {
                    v.searchSet = true;
                    t.TableRowGet(tForm, TableIPCode);
                    tForm.Dispose();
                    return;
                }
                #endregion

                #region _ekle
                if (btn == null)
                {
                    btn = t.Find_Control(tForm, "simpleButton_ekle", TableIPCode, controls);

                    /// Ekle butonu ise
                    if (btn != null)
                    {
                        navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_14_Ekle);
                        return;
                        //e.Handled = false;
                    }
                }
                #endregion

                #region _kart_ac // 51 - kart + form
                if (btn == null)
                {
                    btn = t.Find_Control(tForm, "simpleButton_kart_ac", TableIPCode, controls);

                    /// Ekle butonu ise
                    if (btn != null)
                    {
                        navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_51_Karti_Ac);
                        return;
                    }
                }
                #endregion

            }

            if (tKeys == Keys.Y)
            {
                #region _yeni_kart_ac // 52 - kart + form
                if (btn == null)
                {
                    btn = t.Find_Control(tForm, "simpleButton_yeni_kart_ac", TableIPCode, controls);

                    /// Ekle butonu ise
                    if (btn != null)
                    {
                        navigatorButtonExec_(tForm, TableIPCode, Prop_Navigator, v.tNavigatorButton.nv_52_Yeni_Kart_FormIle);
                        return;
                    }
                }
                #endregion
            }

            if ((tKeys == Keys.C) || (tKeys == Keys.F4))
            {
                tForm.Dispose();
                return;
            }

        }

        public void navigatorButtonExec_(Form tForm, string mst_TableIPCode, string Prop_Navigator, v.tNavigatorButton buttonType)
        {
            string s1 = "=ROW_PROP_NAVIGATOR:";
            string s2 = (char)34 + "TABLEIPCODE_LIST" + (char)34 + ": [";
            // "TABLEIPCODE_LIST": [
            string s3 = (char)39 + "TABLEIPCODE_LIST" + (char)39 + ": [";
            // 'TABLEIPCODE_LIST': [

            if (Prop_Navigator.IndexOf(s1) > -1)
                navigatorButtonExec_OLD(tForm, mst_TableIPCode, Prop_Navigator, buttonType);

            if ((Prop_Navigator.IndexOf(s2) > -1) ||
                (Prop_Navigator.IndexOf(s3) > -1))
                 navigatorButtonExec_JSON(tForm, mst_TableIPCode, Prop_Navigator, buttonType);

        }

        public void navigatorButtonExec_JSON(Form tForm, string mst_TableIPCode, string Prop_Navigator, v.tNavigatorButton buttonType)
        {
            tToolBox t = new tToolBox();

            /// form açma işlemi 
            /// Her zamana aynı formu aç' da olabilir ( chc_xxx fieldleri boş )
            /// veya xxx fieldına bak                 ( chc_xxx fieldleri doludur )
            /// değeri 5 ise  xxxformunu aç
            /// değeri 6 ise  yyyformunu aç
            /// değeri 8 ise  zzzformunu aç
            /// veya
            /// değeri 5, 6, 10 ise xxxformunu aç
            /// değeri 7, 8, 9  ise yyyformunu aç  da olabilir

            bool form_open = false;
            string Ek_Bilgi = string.Empty;
            string block = Prop_Navigator;

            int i1 = Prop_Navigator.IndexOf("=KEY_FNAME:");
            if (i1 > -1)
            {
                Ek_Bilgi = Prop_Navigator.Substring(i1, (Prop_Navigator.Length - i1));
                Prop_Navigator = Prop_Navigator.Substring(0, i1);
            }

            // varsa sil
            t.Str_Remove(ref Prop_Navigator, "|ds|");

            List<PROP_NAVIGATOR> packet = new List<PROP_NAVIGATOR>();
            Prop_Navigator = Prop_Navigator.Replace((char)34, (char)39);
            int i = Prop_Navigator.IndexOf("[");
            if ((i == -1) || (i > 10))
                Prop_Navigator = "[" + Prop_Navigator + "]";
            var prop_ = JsonConvert.DeserializeAnonymousType(Prop_Navigator, packet);

            foreach (PROP_NAVIGATOR item in prop_)
            {
                if (buttonType.ToString().IndexOf(item.BUTTONTYPE.ToString()) > -1)
                {
                    form_open = CheckValue(tForm, item, mst_TableIPCode);

                    #region // form açılması için onaylandı ise
                    if (form_open)
                    {
                        if (buttonType == v.tNavigatorButton.nv_14_Ekle)
                        {
                            tEkle_Butonu_JSON(tForm, item);
                        }

                        if (buttonType == v.tNavigatorButton.nv_15_Arama)
                        {
                            // kullanımda değil
                            // Search_Engine(sender, v.con_Value_Old, 1, Prop_Navigator);

                            // Prop_Navigator üzerinden nv_15_Arama bilgisi var
                            foreach (TABLEIPCODE_LIST listitem in item.TABLEIPCODE_LIST)
                            {
                                if (listitem.WORKTYPE.ToString() == "SAVEDATA")
                                {
                                    string TABLEIPCODE = listitem.TABLEIPCODE.ToString();

                                    v.con_PositionChange = true;
                                    v.con_ExtraChange = true;

                                    if (t.IsNotNull(TABLEIPCODE))
                                    {
                                        tSave sv = new tSave();
                                        if (sv.tDataSave(tForm, TABLEIPCODE))
                                        {
                                            //t.ButtonEnabledAll(tForm, TABLEIPCODE, true);
                                        }
                                    }

                                    v.con_PositionChange = false;
                                    v.con_ExtraChange = false;
                                }

                                if (listitem.WORKTYPE.ToString() == "READ")
                                    t.OpenForm_JSON(tForm, item);
                            }
                        }

                        // birden fazla 
                        if (buttonType == v.tNavigatorButton.nv_24_Kaydet)
                        {
                            tExtraKaydet_Butonu_JSON(tForm, item);
                        }

                        if (buttonType == v.tNavigatorButton.nv_50_Hesap_Ac)
                        {
                            tHesapAc_Butonu_JSON(tForm, item);
                        }

                        if (buttonType == v.tNavigatorButton.nv_51_Karti_Ac)
                        {
                            t.OpenForm_JSON(tForm, item);
                        }
                                                
                        if (buttonType == v.tNavigatorButton.nv_52_Yeni_Kart_FormIle)
                        {
                            tNewForm_NewData_JSON(tForm, item);
                        }

                        // birden fazla 
                        if (buttonType == v.tNavigatorButton.nv_53_Yeni_Hesap)
                        {
                            tNewData_JSON(tForm, item);
                        }

                        // birden fazla 
                        if (buttonType == v.tNavigatorButton.nv_53_Yeni_Hesap_Vazgec)
                        {
                            ////tExtraCancel_Data_JSON(tForm, item);
                            MessageBox.Show("buraya bak : nv_53_Yeni_Hesap_Vazgec");
                        }

                        if (buttonType == v.tNavigatorButton.nv_57_Input_Box)
                        {
                            //
                            InputBox(tForm, mst_TableIPCode, item);
                        }

                        if (buttonType == v.tNavigatorButton.nv_58_Search)
                        {
                            //
                        }

                        if (buttonType == v.tNavigatorButton.nv_59_SubView_Open)
                        {
                            tSubView_(tForm, block, "", "", "");// selectItemValue, Caption, MenuValue);
                            //position = -1;
                        }

                        // birden fazla aynı komut satırı olabilir
                        // örnek 2 adet nv_15_Arama olabilir
                        //break;
                    }
                    #endregion

                }
            }

            /// [
            /// {
            ///   "CAPTION": "null",
            ///   "BUTTONTYPE": "null",
            ///   "TABLEIPCODE_LIST": [],
            ///   "FORMNAME": "null",
            ///   "FORMCODE": "null",
            ///   "FORMTYPE": "null",
            ///   "FORMSTATE": "null",
            ///   "CHC_IPCODE": "null",
            ///   "CHC_FNAME": "null",
            ///   "CHC_VALUE": "null",
            ///   "CHC_OPERAND": "null"
            /// }
            /// ]
        }
        /*
        private bool CheckValue(Form tForm, PROP_NAVIGATOR prop_item2, string mst_TableIPCode)
        {
            bool onay = true;

            PROP_NAVIGATOR prop_item = new PROP_NAVIGATOR();
            
            prop_item.CHC_IPCODE = prop_item2.CHC_IPCODE;
            prop_item.CHC_FNAME = prop_item2.CHC_FNAME;
            prop_item.CHC_VALUE = prop_item2.CHC_VALUE;
            prop_item.CHC_OPERAND = prop_item2.CHC_OPERAND;
            prop_item.CHC_IPCODE_SEC = prop_item2.CHC_IPCODE_SEC;
            prop_item.CHC_FNAME_SEC = prop_item2.CHC_FNAME_SEC;
            prop_item.CHC_VALUE_SEC = prop_item2.CHC_VALUE_SEC;
            prop_item.CHC_OPERAND_SEC = prop_item2.CHC_OPERAND_SEC;

            if ((mst_TableIPCode.LastIndexOf(prop_item2.CHC_IPCODE.ToString()) > -1) &&
                (mst_TableIPCode != prop_item2.CHC_IPCODE.ToString()) &&
                (prop_item2.CHC_IPCODE.ToString() != "")
            )
            {
                /// RTABLEIPCODE:FNLNVOL.FNLNVOL_BNL01
                /// RTABLEIPCODE:FNLNVOL.FNLNVOL_BNL01|23
                foreach (var item in prop_item2.TABLEIPCODE_LIST)
                {
                    if (item.RTABLEIPCODE.ToString() == prop_item2.CHC_IPCODE.ToString())
                        item.RTABLEIPCODE = mst_TableIPCode;
                }
            }
                        
            onay = CheckValue(tForm, prop_item, mst_TableIPCode);

            return onay;
        }
        */
        private bool CheckValue(Form tForm, PROP_NAVIGATOR prop_item, string mst_TableIPCode)
        {
            tToolBox t = new tToolBox();

            // formun açılması için default onay ver 
            // chc_Value     şartı var ise ona göre yeniden değerlendir = form_open1
            // chc_Value_SEC şartı var ise ona göre yeniden değerlendir = form_open2
            bool form_open1 = true;
            bool form_open2 = true;

            string read_value = string.Empty;

            // FIRST = SOURCE
            string chc_IPCode = prop_item.CHC_IPCODE.ToString();
            string chc_FName = prop_item.CHC_FNAME.ToString();
            string chc_Value = prop_item.CHC_VALUE.ToString();
            string chc_Operand = prop_item.CHC_OPERAND.ToString();

            /// bu kısım sonradan eklendiği için gerekli oldu
            /// önceki prop_item larda null geliyor
            if (prop_item.CHC_IPCODE_SEC == null) prop_item.CHC_IPCODE_SEC = "";
            if (prop_item.CHC_FNAME_SEC == null) prop_item.CHC_FNAME_SEC = "";
            if (prop_item.CHC_VALUE_SEC == null) prop_item.CHC_VALUE_SEC = "";
            if (prop_item.CHC_OPERAND_SEC == null) prop_item.CHC_OPERAND_SEC = "";

            // SEC = SECOND or TARGET
            string chc_IPCode_SEC = prop_item.CHC_IPCODE_SEC.ToString();
            string chc_FName_SEC = prop_item.CHC_FNAME_SEC.ToString();
            string chc_Value_SEC = prop_item.CHC_VALUE_SEC.ToString();
            string chc_Operand_SEC = prop_item.CHC_OPERAND_SEC.ToString();

            /// bu IPCode ler dragdrop sırasında oluşuyor

            if (((chc_IPCode == "FIRST") || (chc_IPCode == "SOURCE")) &&
                t.IsNotNull(v.con_DragDropSourceTableIPCode))
                chc_IPCode = v.con_DragDropSourceTableIPCode;

            if (((chc_IPCode_SEC == "SECOND") || (chc_IPCode_SEC == "TARGET")) &&
                t.IsNotNull(v.con_DragDropTargetTableIPCode))
                chc_IPCode_SEC = v.con_DragDropTargetTableIPCode;


            #region Check işlemleri / First veya Soruce için şart varsa
            if (t.IsNotNull(chc_IPCode) &&
                t.IsNotNull(chc_FName) &&
                t.IsNotNull(chc_Value))
            {
                // chc_xxxx ler ile bir fieldin value sine bakarak istedğimiz formu açabiliriz

                // yani  CARI_TIPI = 5 veya CARI_TIPI = 6 veya CARI_TIPI = 10 için  XXXXform unu aç

                // chc_FName  = CARI_TIPI   <<< 
                // chc_Value  = 5, 6, 10    <<< şeklinde olabilir 

                // value ile bize tek bir değer döner 
                // bu nedenle chc_Value içinde sorgudan bize dönen value değeri varmı diye kontrol edilir
                read_value = t.TableFieldValueGet(tForm, chc_IPCode, chc_FName);

                #region // eğer boş ise
                if (t.IsNotNull(read_value) == false)
                {
                    //tTabPage_FNLNVOL_FNLNVOL_BNL0123
                    //FNLNVOL.FNLNVOL_BNL01|23   grid üzerindeki IPCode bu olunca chc_IPCode ile bulunamıyor

                    /// chc_IPCode      = FNLNVOL.FNLNVOL_BNL01   
                    /// mst_TableIPCode = FNLNVOL.FNLNVOL_BNL01|23

                    if (mst_TableIPCode.LastIndexOf(chc_IPCode) > -1)
                    {
                        read_value = t.TableFieldValueGet(tForm, mst_TableIPCode, chc_FName);

                        
                        if (t.IsNotNull(read_value))
                        {
                            /// RTABLEIPCODE:FNLNVOL.FNLNVOL_BNL01
                            /// RTABLEIPCODE:FNLNVOL.FNLNVOL_BNL01|23

                            //t.Str_Replace(ref block, "RTABLEIPCODE:" + chc_IPCode, "RTABLEIPCODE:" + mst_TableIPCode);

                            foreach (var item in prop_item.TABLEIPCODE_LIST)
                            {
                                if (item.RTABLEIPCODE.ToString() == chc_IPCode)
                                    item.RTABLEIPCODE = mst_TableIPCode;
                            }
                        }

                        //eğer boşluksa zaten herhangi bir işlem veri girii yok demektir
                        if (read_value == "") read_value = "0";

                    }

                }
                #endregion boş ise

                // eğer bir daha boş değer gelir ise
                //if (t.IsNotNull(read_value) == false) read_value = "0";

                if (read_value != "")
                {
                    if ((chc_Value.IndexOf(read_value) > -1) &&
                        (t.IsNotNull(chc_Operand) == false))
                    {
                        // eğer buraya kadar geldiyse 
                        // öndeki chc_xxxx kontrollerinden geçti,  
                        // yani onayı hak etti demektir
                        form_open1 = true;
                    }

                    if (t.IsNotNull(chc_Operand))
                    {
                        form_open1 = t.myOperandControl(read_value, chc_Value, chc_Operand);
                    }
                }
            }
            #endregion Check işlemleri1

            #region Check işlemleri / Second veya Target için şart varsa
            if (t.IsNotNull(chc_IPCode_SEC) &&
                t.IsNotNull(chc_FName_SEC) &&
                t.IsNotNull(chc_Value_SEC))
            {
                read_value = t.TableFieldValueGet(tForm, chc_IPCode_SEC, chc_FName_SEC);

                if (read_value != "")
                {
                    if ((chc_Value_SEC.IndexOf(read_value) > -1) &&
                        (t.IsNotNull(chc_Operand_SEC) == false))
                    {
                        // eğer buraya kadar geldiyse 
                        // öndeki chc_xxxx kontrollerinden geçti,  
                        // yani onayı hak etti demektir
                        form_open2 = true;
                    }

                    if (t.IsNotNull(chc_Operand_SEC))
                    {
                        form_open2 = t.myOperandControl(read_value, chc_Value_SEC, chc_Operand_SEC);
                    }
                }

            }
            #endregion Check işlemleri2

            return ((form_open1) && (form_open2));
        }

        public void navigatorButtonExec_OLD(Form tForm, string mst_TableIPCode, string Prop_Navigator, v.tNavigatorButton buttonType)
        {
            tToolBox t = new tToolBox();

            /// form açma işlemi 
            /// Her zamana aynı formu aç' da olabilir ( chc_xxx fieldleri boş )
            /// veya xxx fieldına bak                 ( chc_xxx fieldleri doludur )
            /// değeri 5 ise  xxxformunu aç
            /// değeri 6 ise  yyyformunu aç
            /// değeri 8 ise  zzzformunu aç
            /// veya
            /// değeri 5, 6, 10 ise xxxformunu aç
            /// değeri 7, 8, 9  ise yyyformunu aç  da olabilir

            int position = 0;
            int bpos = 0;
            string block = "";
            string btype = "";

            bool form_open = false;
            string chc_IPCode = "";
            string chc_FName = "";
            string chc_Value = "";
            string chc_Operand = "";
            string s = "";

            while (position >= 0)
            {
                // v.myProperties.Row
                block = t.Get_Properties_Value(v.myProperties.Row, Prop_Navigator, "PROP_NAVIGATOR", ref position);

                // 14, 15, 50, 51, 52 gibi rakamlar geliyor
                btype = t.Get_Properties_Value(v.myProperties.Column, block, "BUTTONTYPE", ref bpos);

                s = buttonType.ToString();

                /// buttonType içinde btype değerleri varmı ? diye kontrol ediliyor 
                /// buttonType >> nv_14_Ekle
                /// buttonType >> nv_50_Hesap_Ac
                /// buttonType >> nv_51_Karti_Ac
                /// buttonType >> nv_52_Yeni_Kart_FormIle

                if (s.IndexOf(btype) > 0)
                {
                    chc_IPCode = t.Get_Properties_Value(v.myProperties.Column, block, "CHC_IPCODE", ref bpos);
                    chc_FName = t.Get_Properties_Value(v.myProperties.Column, block, "CHC_FNAME", ref bpos);
                    chc_Value = t.Get_Properties_Value(v.myProperties.Column, block, "CHC_VALUE", ref bpos);
                    chc_Operand = t.Get_Properties_Value(v.myProperties.Column, block, "CHC_OPERAND", ref bpos);

                    #region Check işlemleri
                    if ((chc_IPCode != "") && (chc_FName != "") && (chc_Value != ""))
                    {
                        // chc_xxxx ler ile bir fieldin value sine bakarak istedğimiz formu açabiliriz

                        // yani  CARI_TIPI = 5 veya CARI_TIPI = 6 veya CARI_TIPI = 10 için  XXXXform unu aç

                        // chc_FName  = CARI_TIPI   <<< 
                        // chc_Value  = 5, 6, 10    <<< şeklinde olabilir 

                        // value ile bize tek bir değer döner 
                        // bu nedenle chc_Value içinde sorgudan bize dönen value değeri varmı diye kontrol edilir
                        string read_value = t.TableFieldValueGet(tForm, chc_IPCode, chc_FName);

                        // eğer boş ise
                        if (t.IsNotNull(read_value) == false)
                        {
                            //tTabPage_FNLNVOL_FNLNVOL_BNL0123
                            //FNLNVOL.FNLNVOL_BNL01|23   grid üzerindeki IPCode bu olunca chc_IPCode ile bulunamıyor

                            /// chc_IPCode      = FNLNVOL.FNLNVOL_BNL01   
                            /// mst_TableIPCode = FNLNVOL.FNLNVOL_BNL01|23

                            if (mst_TableIPCode.LastIndexOf(chc_IPCode) > -1)
                            {
                                read_value = t.TableFieldValueGet(tForm, mst_TableIPCode, chc_FName);

                                if (t.IsNotNull(read_value))
                                {
                                    /// RTABLEIPCODE:FNLNVOL.FNLNVOL_BNL01
                                    /// RTABLEIPCODE:FNLNVOL.FNLNVOL_BNL01|23
                                    t.Str_Replace(ref block, "RTABLEIPCODE:" + chc_IPCode, "RTABLEIPCODE:" + mst_TableIPCode);
                                }
                            }
                        }

                        // eğer bir daha boş değer gelir ise
                        //if (t.IsNotNull(read_value) == false) read_value = "0";

                        if (read_value != "")
                        {
                            if ((chc_Value.IndexOf(read_value) > -1) &&
                                (t.IsNotNull(chc_Operand) == false))
                            {
                                // eğer buraya kadar geldiyse 
                                // öndeki chc_xxxx kontrollerinden geçti demekki 
                                // onun için onayı hak etti demektir
                                form_open = true;
                            }
                            if (t.IsNotNull(chc_Operand))
                            {
                                form_open = t.myOperandControl(read_value, chc_Value, chc_Operand);
                            }
                        }
                    }
                    else
                    {
                        // Eğer chc_Value şartı yok ise hemen onay ver
                        form_open = true;
                    }
                    #endregion Check işlemleri

                    #region // form açılması için onaylandı ise
                    if (form_open)
                    {
                        if (buttonType == v.tNavigatorButton.nv_14_Ekle)
                        {
                            tEkle_Butonu(tForm, block);
                        }

                        if (buttonType == v.tNavigatorButton.nv_15_Arama)
                        {
                            // şimdilik çözüm bulamadım
                            //Search_Engine(sender, v.con_Value_Old, 1, Prop_Navigator);
                            position = -1;
                        }

                        // birden fazla 
                        if (buttonType == v.tNavigatorButton.nv_24_Kaydet)
                        {
                            tExtraKaydet_Butonu(tForm, block);
                        }

                        if (buttonType == v.tNavigatorButton.nv_50_Hesap_Ac)
                        {
                            tHesapAc_Butonu(tForm, block);
                        }

                        if (buttonType == v.tNavigatorButton.nv_51_Karti_Ac)
                        {
                            t.OpenForm(tForm, block);
                            position = -1;
                        }

                        // birden fazla 
                        if (buttonType == v.tNavigatorButton.nv_53_Yeni_Hesap)
                        {
                            tExtraYeni_Butonu(tForm, block);
                        }

                        // birden fazla 
                        if (buttonType == v.tNavigatorButton.nv_53_Yeni_Hesap_Vazgec)
                        {
                            tExtraCancel_Data(tForm, block);
                        }

                        if (buttonType == v.tNavigatorButton.nv_52_Yeni_Kart_FormIle)
                        {
                            tNewForm_NewData(tForm, block);
                            position = -1;
                        }

                        if (buttonType == v.tNavigatorButton.nv_59_SubView_Open)
                        {
                            //tSubView_Open(tForm, block);
                            tSubView_(tForm, block, "", "", "");// selectItemValue, Caption, MenuValue);
                            position = -1;
                        }

                    }
                    #endregion
                }
            }


            ///                 ROW_LINE_NO ROW_CAPTION                              ROW_FIELDNAME                            ROW_FIELDTYPE ROW_COLUMN_TYPE                LIST_TYPES_NAME
            /// --------------- ----------- ---------------------------------------- ---------------------------------------- ------------- ------------------------------ --------------------
            /// 1               1           Navigator Buttons                        NULL                                     0             NULL                           NULL
            ///  1               2           Caption                                  CAPTION                                  0             NULL                           NULL
            ///  1               3           Button Type                              BUTTONTYPE                               0             ImageComboBoxEdit              NAVIGATOR2
            ///  1               4           TableIPCode List                         TABLEIPCODE_LIST                         0             tPropertiesPlusEdit            NULL
            /// 2               1           Open Form                                NULL                                     0             NULL                           NULL
            ///  2               2           Form Name                                FORMNAME                                 0             NULL                           NULL
            ///  2               3           Form Code                                FORMCODE                                 0             NULL                           NULL
            ///  2               4           Form Type                                FORMTYPE                                 0             ImageComboBoxEdit              FORMTYPE
            ///  2               5           Form State                               FORMSTATE                                0             ImageComboBoxEdit              FORMSTATE
            /// 3               1           Check Value                              NULL                                     0             NULL                           NULL
            ///  3               2           Check TableIPCode                        CHC_IPCODE                               0             NULL                           NULL
            ///  3               3           Check FieldName                          CHC_FNAME                                0             NULL                           NULL
            ///  3               4           Chech Value                              CHC_VALUE                                0             NULL                           NULL

        }

        #endregion navigatorButtonExec_

        #region tExtraKaydet_Butonu // 24
        public void tExtraKaydet_Butonu_JSON(Form tForm, PROP_NAVIGATOR prop_)
        {
            tToolBox t = new tToolBox();

            #region örnek
            /*
            */
            #endregion örnek

            string TABLEIPCODE = string.Empty;
            string WORKTYPE = string.Empty;

            foreach (var item in prop_.TABLEIPCODE_LIST)
            {
                WORKTYPE = item.WORKTYPE.ToString();
                TABLEIPCODE = t.Set(item.TABLEIPCODE.ToString(), "", "");

                #region
                if (WORKTYPE == "SAVEDATA")
                {
                    v.con_PositionChange = true;
                    v.con_ExtraChange = true;

                    if (t.IsNotNull(TABLEIPCODE))
                    {
                        tSave sv = new tSave();
                        if (sv.tDataSave(tForm, TABLEIPCODE))
                        {
                            t.ButtonEnabledAll(tForm, TABLEIPCODE, true);
                        }
                    }

                    v.con_PositionChange = false;
                    v.con_ExtraChange = false;
                }
                else
                if (WORKTYPE == "READ")
                {
                    if (t.IsNotNull(TABLEIPCODE))
                    {
                        //MessageBox.Show("Burdan devam");
                        DataSet ds = t.Find_DataSet(tForm, "", TABLEIPCODE, "");

                        if (ds != null)
                        {
                            t.TableRefresh(tForm, ds, TABLEIPCODE);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Bu bölüm eksik hemen yazmaya başla : " + WORKTYPE);
                }
                #endregion
            }
        }

        public void tExtraKaydet_Butonu(Form tForm, string Prop_Navigator_Block)
        {
            tToolBox t = new tToolBox();

            #region örnek
            /*
            PROP_NAVIGATOR={
            1=ROW_PROP_NAVIGATOR:1;
            1=CAPTION:EXTRA CARİYİ KAYDET;
            1=BUTTONTYPE:24;
            1=TABLEIPCODE_LIST:TABLEIPCODE_LIST={

            // birden fazla kayıt edebilir

            1=ROW_TABLEIPCODE_LIST:1;
            1=CAPTION:CARİ KAYDET;
            1=TABLEIPCODE:HCR.HCR_YBR_TESTB2;
            1=TABLEALIAS:null;
            1=KEYFNAME:null;
            1=RTABLEIPCODE:null;
            1=RKEYFNAME:null;
            1=MSETVALUE:null;
            1=WORKTYPE:SAVEDATA;
            1=CONTROLNAME:null;
            1=ROWE_TABLEIPCODE_LIST:1;

            2=ROW_TABLEIPCODE_LIST:2;
            2=CAPTION:CARİ KAYDET2;
            2=TABLEIPCODE:HCR.HCR_YAL_TESTA2;
            2=TABLEALIAS:null;
            2=KEYFNAME:null;
            2=RTABLEIPCODE:null;
            2=RKEYFNAME:null;
            2=MSETVALUE:null;
            2=WORKTYPE:SAVEDATA;
            2=CONTROLNAME:null;
            2=ROWE_TABLEIPCODE_LIST:2;

            
            TABLEIPCODE_LIST=};

            ....
            */
            #endregion örnek

            string s = Prop_Navigator_Block;

            if (s.IndexOf("TABLEIPCODE_LIST") > -1)
            {
                string TABLEIPCODE = string.Empty;
                string WORKTYPE = string.Empty;
                string row_block = string.Empty;
                string lockE = "=ROWE_";

                v.con_PositionChange = true;
                v.con_ExtraChange = true;

                while (s.IndexOf(lockE) > -1)
                {
                    row_block = t.Find_Properies_Get_RowBlock(ref s, "TABLEIPCODE_LIST");
                    WORKTYPE = t.MyProperties_Get(row_block, "WORKTYPE:");

                    if (WORKTYPE == "SAVEDATA")
                    {
                        TABLEIPCODE = t.MyProperties_Get(row_block, "TABLEIPCODE:");

                        if (t.IsNotNull(TABLEIPCODE))
                        {
                            tSave sv = new tSave();
                            if (sv.tDataSave(tForm, TABLEIPCODE))
                            {
                                t.ButtonEnabledAll(tForm, TABLEIPCODE, true);
                            }
                        }
                    } //if (row_block.IndexOf 
                } //while ( 

                v.con_PositionChange = false;
                v.con_ExtraChange = false;
            }

        }
        #endregion

        #region tEkle_Butonu // 14
        public void tEkle_Butonu_JSON(Form tForm, PROP_NAVIGATOR prop_)
        {
            tToolBox t = new tToolBox();

            #region örnek
            /*
            
            */
            #endregion örnek

            string WORKTYPE = string.Empty;
            string oldWORKTYPE = string.Empty;
            string TABLEIPCODE = string.Empty;
            string KEYFNAME = string.Empty;
            string RTABLEIPCODE = string.Empty;
            string RKEYFNAME = string.Empty;
            string DataCopyCode = string.Empty;
            string oldTableIPCode = string.Empty;
            bool old_PositionChange = false;

            DataSet dsData = null;
            DataNavigator tDataNavigator = null;
            DataSet dsDataR = null;
            DataNavigator tDataNavigatorR = null;

            foreach (var item in prop_.TABLEIPCODE_LIST)
            {
                WORKTYPE = item.WORKTYPE.ToString();

                #region 1
                if ((WORKTYPE == "READ") ||
                    (WORKTYPE == "SETDATA"))
                {
                    oldWORKTYPE = WORKTYPE;
                    TABLEIPCODE = t.Set(item.TABLEIPCODE.ToString(), "", "");
                    KEYFNAME = t.Set(item.KEYFNAME.ToString(), "", "");
                    RTABLEIPCODE = t.Set(item.RTABLEIPCODE.ToString(), "", "");
                    RKEYFNAME = t.Set(item.RKEYFNAME.ToString(), "", "");

                    dsData = null;
                    tDataNavigator = null;
                    t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TABLEIPCODE);

                    #region /// eğer table halen açılmamışsa önce açmak gerekiyor
                    if ((dsData != null) && (t.IsNotNull(KEYFNAME)) && (tDataNavigator.Position == -1))
                    {
                        old_PositionChange = v.con_PositionChange;
                        v.con_PositionChange = true;

                        tNewData(tForm, dsData, tDataNavigator, TABLEIPCODE, "", "", "");

                        // aşağıda tekrar yapmasın diye
                        oldTableIPCode = TABLEIPCODE;

                        if (old_PositionChange == false)
                            v.con_PositionChange = false;
                    }
                    #endregion

                    #region
                    if ((dsData != null) && (t.IsNotNull(KEYFNAME)) && (tDataNavigator.Position > -1))
                    {
                        dsDataR = null;
                        tDataNavigatorR = null;
                        t.Find_DataSet(tForm, ref dsDataR, ref tDataNavigatorR, RTABLEIPCODE);

                        if ((t.IsNotNull(dsDataR)) && (t.IsNotNull(RKEYFNAME)))
                        {
                            if (TABLEIPCODE != oldTableIPCode)
                            {
                                tNewData(tForm, dsData, tDataNavigator, TABLEIPCODE, KEYFNAME, "", "");
                                oldTableIPCode = TABLEIPCODE;
                            }

                            dsData.Tables[0].Rows[tDataNavigator.Position][KEYFNAME] =
                                dsDataR.Tables[0].Rows[tDataNavigatorR.Position][RKEYFNAME];
                        }
                    }
                    #endregion
                }
                #endregion

                #region 2
                if (t.IsNotNull(oldTableIPCode) && (dsData == null))
                {
                    t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, oldTableIPCode);
                }
                #endregion

                #region 3
                if ((dsData != null) &&
                    (tDataNavigator != null))
                {
                    if (oldWORKTYPE == "SETDATA")
                    {
                        //if (dsData.Tables[0].Namespace == "NewRecord")
                        //    dsData.Tables[0].Namespace = "";
                        if (dsData.Tables[0].CaseSensitive == true)
                            dsData.Tables[0].CaseSensitive = false;

                        tDefaultValue df = new tDefaultValue();
                        if (df.tDefaultValue_And_Validation
                                            (tForm,
                                             dsData,
                                             dsData.Tables[0].Rows[tDataNavigator.Position],
                                             TABLEIPCODE,
                                             "tData_Save") == true) // Table Save 
                        {
                            tSave sv = new tSave();
                            sv.tDataSave(tForm, dsData, tDataNavigator, tDataNavigator.Position);
                            v.Kullaniciya_Mesaj_Var = v.DBRec_ListAdd;

                            v.con_Refresh = true;
                        }
                    }

                    if (oldWORKTYPE == "READ")
                    {
                        dsData.Tables[0].Rows[tDataNavigator.Position].AcceptChanges();
                    }

                    // yeri yanlış
                    if (oldWORKTYPE == "SETDATA")
                    {
                        if (dsData.Tables[0].CaseSensitive == true)
                            dsData.Tables[0].CaseSensitive = false;

                        tSave sv = new tSave();
                        sv.tDataSave(tForm, dsData, tDataNavigator, tDataNavigator.Position);
                        v.con_Refresh = true;
                    }
                }
                #endregion 3

                #region 4 Run DataCopy
                if (WORKTYPE == "RDC")
                {
                    DataCopyCode = t.Set(item.DCCODE.ToString(), "", "");

                    if (t.IsNotNull(DataCopyCode))
                    {
                        tDataCopy dc = new tDataCopy();
                        dc.tDC_Run(tForm, DataCopyCode);
                        //dc.tDC_Run(tForm, v.SP_Conn_Proje_MSSQL, DataCopyCode);
                    }
                }
                #endregion


            } // foreach
        }

        public void tEkle_Butonu(Form tForm, string Prop_Navigator_Block)
        {
            tToolBox t = new tToolBox();
            //string func_name = "tEkle_Butonu";
            //MessageBox.Show(Prop_Navigator_Block);

            #region örnek
            /*
            1=ROW_PROP_NAVIGATOR:1;
            1=CAPTION:Ekle - Alacaklı;
            1=BUTTONTYPE:14;
            1=TABLEIPCODE_LIST:TABLEIPCODE_LIST={

                1=ROW_TABLEIPCODE_LIST:1;
                1=CAPTION:Ekle - CARI_ID set;
                1=TABLEIPCODE:AVI_MDCR.AVI_MDCR_ALC;
                1=TABLEALIAS:[AVI_MDCR];
                1=KEYFNAME:CARI_ID;
                1=RTABLEIPCODE:HCR.HCR_LST01;
                1=RKEYFNAME:ID;
                1=MSETVALUE:null;
                1=WORKTYPE:SETDATA;
                1=CONTROLNAME:null;
                1=ROWE_TABLEIPCODE_LIST:1;

                2=ROW_TABLEIPCODE_LIST:2;
                2=CAPTION:Ekle - LKP_TARAF_TAMADI set;
                2=TABLEIPCODE:AVI_MDCR.AVI_MDCR_ALC;
                2=TABLEALIAS:[AVI_MDCR];
                2=KEYFNAME:LKP_TARAF_TAMADI;
                2=RTABLEIPCODE:HCR.HCR_LST01;
                2=RKEYFNAME:TAMADI;
                2=MSETVALUE:null;
                2=WORKTYPE:SETDATA;
                2=CONTROLNAME:null;
                2=ROWE_TABLEIPCODE_LIST:2;

            TABLEIPCODE_LIST=};

            1=FORMNAME:null;
            1=FORMCODE:null;
            1=FORMTYPE:null;
            1=FORMSTATE:null;
            1=CHC_IPCODE:null;
            1=CHC_FNAME:null;
            1=CHC_VALUE:null;
            1=ROWE_PROP_NAVIGATOR:1;

            */
            #endregion örnek

            string s = Prop_Navigator_Block;

            if (s.IndexOf("TABLEIPCODE_LIST") > -1)
            {
                bool old_PositionChange = false;
                string TABLEIPCODE = string.Empty;
                string KEYFNAME = string.Empty;
                string RTABLEIPCODE = string.Empty;
                string RKEYFNAME = string.Empty;
                string WORKTYPE = string.Empty;
                string oldWORKTYPE = string.Empty;

                string oldTableIPCode = string.Empty;
                string row_block = string.Empty;
                string lockE = "=ROWE_";

                DataSet dsData = null;
                DataNavigator tDataNavigator = null;
                DataSet dsDataR = null;
                DataNavigator tDataNavigatorR = null;

                while (s.IndexOf(lockE) > -1)
                {
                    row_block = t.Find_Properies_Get_RowBlock(ref s, "TABLEIPCODE_LIST");
                    WORKTYPE = t.MyProperties_Get(row_block, "WORKTYPE:");

                    if ((WORKTYPE == "READ") ||
                        (WORKTYPE == "SETDATA")
                        )
                    {
                        oldWORKTYPE = WORKTYPE;
                        TABLEIPCODE = t.MyProperties_Get(row_block, "TABLEIPCODE:");
                        KEYFNAME = t.MyProperties_Get(row_block, "KEYFNAME:");
                        RTABLEIPCODE = t.MyProperties_Get(row_block, "RTABLEIPCODE:");
                        RKEYFNAME = t.MyProperties_Get(row_block, "RKEYFNAME:");

                        dsData = null;
                        tDataNavigator = null;
                        t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TABLEIPCODE);//, func_name);

                        /// eğer table halen açılmamışsa önce açmak gerekiyor
                        if ((dsData != null) && (t.IsNotNull(KEYFNAME)) && (tDataNavigator.Position == -1))
                        {
                            old_PositionChange = v.con_PositionChange;
                            v.con_PositionChange = true;

                            tNewData(tForm, dsData, tDataNavigator, TABLEIPCODE, "", "", "");

                            // aşağıda tekrar yapmasın diye
                            oldTableIPCode = TABLEIPCODE;

                            if (old_PositionChange == false)
                                v.con_PositionChange = false;
                        }

                        if ((dsData != null) && (t.IsNotNull(KEYFNAME)) && (tDataNavigator.Position > -1))
                        {
                            dsDataR = null;
                            tDataNavigatorR = null;
                            t.Find_DataSet(tForm, ref dsDataR, ref tDataNavigatorR, RTABLEIPCODE);//, func_name);

                            if ((t.IsNotNull(dsDataR)) && (t.IsNotNull(RKEYFNAME)))
                            {
                                if (TABLEIPCODE != oldTableIPCode)
                                {
                                    tNewData(tForm, dsData, tDataNavigator, TABLEIPCODE, KEYFNAME, "", "");
                                    oldTableIPCode = TABLEIPCODE;
                                }

                                dsData.Tables[0].Rows[tDataNavigator.Position][KEYFNAME] =
                                    dsDataR.Tables[0].Rows[tDataNavigatorR.Position][RKEYFNAME];
                            }

                        }
                    } //if (row_block.IndexOf 
                } //while ( 


                if (t.IsNotNull(oldTableIPCode) && (dsData == null))
                {
                    t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, oldTableIPCode);//, func_name);
                }

                if ((dsData != null) &&
                    (tDataNavigator != null)
                    )
                {
                    if (oldWORKTYPE == "SETDATA")
                    {
                        //if (dsData.Tables[0].Namespace == "NewRecord")
                        //    dsData.Tables[0].Namespace = "";
                        if (dsData.Tables[0].CaseSensitive == true)
                            dsData.Tables[0].CaseSensitive = false;

                        tDefaultValue df = new tDefaultValue();
                        if (df.tDefaultValue_And_Validation
                                            (tForm,
                                             dsData,
                                             dsData.Tables[0].Rows[tDataNavigator.Position],
                                             TABLEIPCODE,
                                             "tData_Save") == true) // Table Save 
                        {
                            tSave sv = new tSave();
                            sv.tDataSave(tForm, dsData, tDataNavigator, tDataNavigator.Position);
                            v.Kullaniciya_Mesaj_Var = v.DBRec_ListAdd;

                            v.con_Refresh = true;
                        }
                    }

                    if (oldWORKTYPE == "READ")
                    {
                        dsData.Tables[0].Rows[tDataNavigator.Position].AcceptChanges();
                    }

                    // yeri yanlış
                    if (oldWORKTYPE == "SETDATA")
                    {
                        if (dsData.Tables[0].CaseSensitive == true)
                            dsData.Tables[0].CaseSensitive = false;

                        tSave sv = new tSave();
                        sv.tDataSave(tForm, dsData, tDataNavigator, tDataNavigator.Position);
                        v.con_Refresh = true;
                    }
                }

            } // if (s
        }
        #endregion tEkle_Butonu

        #region tHesapAc_Butonu // 50
        public void tHesapAc_Butonu_JSON(Form tForm, PROP_NAVIGATOR prop_)
        {
            tToolBox t = new tToolBox();

            /// aynı form içinde en az iki IP var
            /// birinci IP de liste var, ikinci IP de listedeki kaydın detayı var
            /// ama bu master-detail değil, çünkü liste üzerindeki kayıt sadece kullanıcı
            /// istediği zaman (Hesap Aç) ile detayı görmek istediği zaman açılacak
            /// 

            string WORKTYPE = string.Empty;
            string TABLEIPCODE = string.Empty;
            string KEYFNAME = string.Empty;
            string RTABLEIPCODE = string.Empty;
            string RKEYFNAME = string.Empty;
            string oldWORKTYPE = string.Empty;
            string oldTableIPCode = string.Empty;
            string readValue = string.Empty;

            DataSet dsData = null;
            DataNavigator tDataNavigator = null;
            DataSet dsDataR = null;
            DataNavigator tDataNavigatorR = null;

            foreach (var item in prop_.TABLEIPCODE_LIST)
            {
                WORKTYPE = item.WORKTYPE.ToString();

                #region
                if (WORKTYPE == "READ")
                {
                    oldWORKTYPE = WORKTYPE;
                    TABLEIPCODE = t.Set(item.TABLEIPCODE.ToString(), "", "");
                    KEYFNAME = t.Set(item.KEYFNAME.ToString(), "", "");
                    RTABLEIPCODE = t.Set(item.RTABLEIPCODE.ToString(), "", "");
                    RKEYFNAME = t.Set(item.RKEYFNAME.ToString(), "", "");

                    dsData = null;
                    tDataNavigator = null;
                    t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TABLEIPCODE);

                    if ((dsData != null) && (t.IsNotNull(KEYFNAME)))
                    {
                        dsDataR = null;
                        tDataNavigatorR = null;
                        t.Find_DataSet(tForm, ref dsDataR, ref tDataNavigatorR, RTABLEIPCODE);

                        if ((t.IsNotNull(dsDataR)) && (t.IsNotNull(RKEYFNAME)))
                        {
                            /// okunacak/açlılacak kaydın id si ( Listeden okunuyor )
                            readValue = dsDataR.Tables[0].Rows[tDataNavigatorR.Position][RKEYFNAME].ToString();

                            /// okunacak/açıklacak IP  
                            string myProp = dsData.Namespace.ToString();
                            string TableLabel = t.MyProperties_Get(myProp, "TableLabel:");
                            t.Alias_Control(ref TableLabel);
                            string newValue = "and " + TableLabel + KEYFNAME + " = " + readValue + " ";

                            External_Kriterleri_Uygula(dsData, TableLabel, newValue, null);
                        }
                    }// if dsData
                }
                #endregion
            }
        }

        public void tHesapAc_Butonu(Form tForm, string Prop_Navigator_Block)
        {
            tToolBox t = new tToolBox();

            string s = Prop_Navigator_Block;

            if (s.IndexOf("TABLEIPCODE_LIST") > -1)
            {
                string TABLEIPCODE = string.Empty;
                string KEYFNAME = string.Empty;
                string RTABLEIPCODE = string.Empty;
                string RKEYFNAME = string.Empty;
                string WORKTYPE = string.Empty;
                string oldWORKTYPE = string.Empty;

                string oldTableIPCode = string.Empty;
                string row_block = string.Empty;
                string lockE = "=ROWE_";

                string readValue = string.Empty;

                /// aynı form içinde en az iki IP var
                /// birinci IP de liste var, ikinci IP de listedeki kaydın detayı var
                /// ama bu master-detail değil, çünkü liste üzerindeki kayıt sadece kullanıcı
                /// istediği zaman (Hesap Aç) ile detayı görmek istediği zaman açılacak
                /// 

                DataSet dsData = null;
                DataNavigator tDataNavigator = null;
                DataSet dsDataR = null;
                DataNavigator tDataNavigatorR = null;

                while (s.IndexOf(lockE) > -1)
                {
                    row_block = t.Find_Properies_Get_RowBlock(ref s, "TABLEIPCODE_LIST");
                    WORKTYPE = t.MyProperties_Get(row_block, "WORKTYPE:");

                    if (WORKTYPE == "READ")
                    {
                        oldWORKTYPE = WORKTYPE;
                        TABLEIPCODE = t.MyProperties_Get(row_block, "TABLEIPCODE:");
                        KEYFNAME = t.MyProperties_Get(row_block, "KEYFNAME:");
                        RTABLEIPCODE = t.MyProperties_Get(row_block, "RTABLEIPCODE:");
                        RKEYFNAME = t.MyProperties_Get(row_block, "RKEYFNAME:");

                        dsData = null;
                        tDataNavigator = null;
                        t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TABLEIPCODE);

                        if ((dsData != null) && (t.IsNotNull(KEYFNAME)))
                        {
                            dsDataR = null;
                            tDataNavigatorR = null;
                            t.Find_DataSet(tForm, ref dsDataR, ref tDataNavigatorR, RTABLEIPCODE);

                            if ((t.IsNotNull(dsDataR)) && (t.IsNotNull(RKEYFNAME)))
                            {
                                /// okunacak/açlılacak kaydın id si ( Listeden okunuyor )
                                readValue = dsDataR.Tables[0].Rows[tDataNavigatorR.Position][RKEYFNAME].ToString();

                                /// okunacak/açıklacak IP  
                                string myProp = dsData.Namespace.ToString();
                                string TableLabel = t.MyProperties_Get(myProp, "TableLabel:");
                                t.Alias_Control(ref TableLabel);
                                string newValue = "and " + TableLabel + KEYFNAME + " = " + readValue + " ";

                                External_Kriterleri_Uygula(dsData, TableLabel, newValue, null);

                            }
                        }// if dsData
                    } //if (row_block.IndexOf 
                } // while s
            } // if s.IndexOf
        }
        #endregion

        #region tNewForm_NewData  // 52
        public void tNewForm_NewData(Form tForm, string Prop_Navigator)
        {
            tToolBox t = new tToolBox();
            // refresh 
            tForm.HelpButton = true;
            //
            t.OpenForm(tForm, Prop_Navigator);
        }

        public void tNewForm_NewData_JSON(Form tForm, PROP_NAVIGATOR prop_)
        {
            tToolBox t = new tToolBox();
            //
            t.OpenForm_JSON(tForm, prop_);

            DataSet ds = null;
            string WORKTYPE = string.Empty;
            foreach (var item in prop_.TABLEIPCODE_LIST)
            {
                WORKTYPE = item.WORKTYPE.ToString();

                #region
                if (WORKTYPE == "GOTO")
                {
                    // save çalıştıysa
                    if (v.con_GotoRecord == "ONdialog")
                    {
                        
                        v.con_TableIPCode = t.Set(item.TABLEIPCODE.ToString(), "", "");
                        v.con_GotoRecord_FName = t.Set(item.KEYFNAME.ToString(), "", "");

                        /// burada dataset neden ihtiyaç var diye sorabilirsin
                        /// aslında gerek yok
                        /// fakat tNewForm_NewData yu kulanan menu buttonu birden fazla ekranda kullanılabilir
                        /// yani New in ardından birden fazla GOTO komutu çağrılmış olabilir
                        /// eğer doğru form ise aşağıdaki işlemler yapılsın diye DataSet kontrol ediliyor
                        /// Örnek : Yeni Müşteri butonu için 
                        /// CR.CR_OMARA_L01 veya 
                        /// CR.CR_OMARA_L02 listesi üzerinde GOTO yapılabilir
                        /// 
                        ds = t.Find_DataSet(tForm, "", v.con_TableIPCode, "");

                        if (ds != null)
                        {
                            // refresh 
                            //tForm.HelpButton = true;
                            //
                            //myForm_Refresh(tForm, "tNewForm_NewData");

                            //
                            t.TableRefresh(tForm, ds);

                            //
                            t.tGotoRecord(tForm, ds,
                                v.con_TableIPCode,
                                v.con_GotoRecord_FName,
                                v.con_GotoRecord_Value,
                                v.con_GotoRecord_Position);


                        }
                    }
                }
                #endregion
            }

        }
        #endregion tNewForm_NewData

        #region tExtraYeni_Butonu // 53
        public void tExtraYeni_Butonu(Form tForm, string Prop_Navigator_Block)
        {
            tToolBox t = new tToolBox();

            #region örnek
            /*
            2=ROW_PROP_NAVIGATOR:2;
            2=CAPTION:EXTRA CARİYİ YENİLE;
            2=BUTTONTYPE:53;
            2=TABLEIPCODE_LIST:TABLEIPCODE_LIST={
            
            1=ROW_TABLEIPCODE_LIST:1;
            1=CAPTION:CARİYİ YENİLE;
            1=TABLEIPCODE:HCR.HCR_YBR_TESTB2;
            1=TABLEALIAS:null;
            1=KEYFNAME:null;
            1=RTABLEIPCODE:null;
            1=RKEYFNAME:null;
            1=MSETVALUE:null;
            1=WORKTYPE:NEW;
            1=CONTROLNAME:null;
            1=ROWE_TABLEIPCODE_LIST:1;
            TABLEIPCODE_LIST=};

            2=FORMNAME:null;
            2=FORMCODE:null;
            2=FORMTYPE:null;
            2=FORMSTATE:null;
            2=CHC_IPCODE:null;
            2=CHC_FNAME:null;
            2=CHC_VALUE:null;
            2=CHC_OPERAND:null;
            2=ROWE_PROP_NAVIGATOR:2;
            */
            #endregion örnek

            string s = Prop_Navigator_Block;

            if (s.IndexOf("TABLEIPCODE_LIST") > -1)
            {
                v.con_ExtraChange = true;

                string TABLEIPCODE = string.Empty;
                string WORKTYPE = string.Empty;
                string row_block = string.Empty;
                string lockE = "=ROWE_";

                while (s.IndexOf(lockE) > -1)
                {
                    row_block = t.Find_Properies_Get_RowBlock(ref s, "TABLEIPCODE_LIST");
                    WORKTYPE = t.MyProperties_Get(row_block, "WORKTYPE:");

                    if (WORKTYPE == "NEW")
                    {
                        TABLEIPCODE = t.MyProperties_Get(row_block, "TABLEIPCODE:");

                        if (t.IsNotNull(TABLEIPCODE))
                        {
                            tNewData(tForm, TABLEIPCODE, "", "", "");
                        }
                    } //if (row_block.IndexOf 
                } //while

                v.con_ExtraChange = false;
            }

        }
        #endregion

        #region tNewData // 53
        public void tNewData_JSON(Form tForm, PROP_NAVIGATOR prop_)
        {
            tToolBox t = new tToolBox();

            string workType = string.Empty;
            string TableIPCode = string.Empty;
            string keyFName = string.Empty;
            
            foreach (var item in prop_.TABLEIPCODE_LIST)
            {
                workType = item.WORKTYPE.ToString();

                #region
                if (workType == "NEW")
                {
                    TableIPCode = t.Set(item.TABLEIPCODE.ToString(), "", "");
                    keyFName = t.Set(item.KEYFNAME.ToString(), "", "");

                    tNewData(tForm, TableIPCode, keyFName, "", "");
                }
                #endregion
            }

        }
        
        public void tNewData(Form tForm, string TableIPCode)
        {
            tNewData(tForm, TableIPCode, "", "", "");
        }

        public void tNewData(Form tForm, string TableIPCode, string Key_FName, string Parent_FName, string Value)
        {
            tToolBox t = new tToolBox();

            // Eğer bu fonksiyonu çağıran yerde dsData ve DataNavigator mevcut ise
            // aşağıdaki yeni üçüncü tNewData ya yönlendir

            DataSet dS = null;
            DataNavigator dN = null;
            t.Find_DataSet(tForm, ref dS, ref dN, TableIPCode);

            #region dsData
            if (dS != null)
            {
                //if (dsData.Tables[0].Namespace == "NewRecord") return;
                if (dS.Tables[0].CaseSensitive == true) return;

                tNewData(tForm, dS, dN, TableIPCode, Key_FName, Parent_FName, Value);

                if (v.con_PositionChange == false)
                {
                    if (dN.IsAccessible == true)
                    {
                        vSubWork vSW = new vSubWork();
                        vSW._01_tForm = tForm;
                        vSW._02_TableIPCode = TableIPCode;
                        vSW._03_WorkTD = v.tWorkTD.Refresh_SubDetail;
                        vSW._04_WorkWhom = v.tWorkWhom.Childs;
                        tSubWork_(vSW);
                    }
                }
            }
            #endregion  dsData
        }

        public void tNewData(Form tForm, DataSet dsData, DataNavigator dN,
               string TableIPCode, string Key_FName, string Parent_FName, string Value)
        {

            #region dsData
            if (dsData != null)
            {
                //if (dsData.Tables[0].Namespace == "NewRecord") return;
                if (dsData.Tables[0].CaseSensitive == true) return;

                tToolBox t = new tToolBox();

                bool old_PositionChange = false;
                string myProp = dsData.Namespace;
                string DetailSubDetail = t.MyProperties_Get(myProp, "DetailSubDetail:");

                //= DetailSubDetail:True;
                //About_Detail_SubDetail:
                //= Detail_SubDetail:AVI_DOS.AVI_DOS_05 || ID ||[AVI_DCK].ICRA_DOSYA_ID || 56 || 31 || 0 |||||| 578 | ds |;
                if (DetailSubDetail == "True")
                {
                    string SubDetail_List = t.MyProperties_Get(myProp, "About_Detail_SubDetail:");
                    if (SubDetail_MasterIDValueChecked(tForm, SubDetail_List) == false)
                    {
                        if (v.con_PositionChange == false)
                        {
                            MessageBox.Show("Lütfen öncelikli olan bilgileri girmeniz gerekiyor ...");
                            return;
                        }
                    }
                }

                //dsData.Tables[0].Namespace = "NewRecord";
                dsData.Tables[0].CaseSensitive = true;

                DataRow New_row = dsData.Tables[0].NewRow();

                tDefaultValue df = new tDefaultValue();
                df.tDefaultValue_And_Validation
                                (tForm,
                                 dsData,
                                 New_row,
                                 TableIPCode,
                                 "tData_NewRecord");

                if (t.IsNotNull(Parent_FName) && t.IsNotNull(Value))
                {
                    New_row[Parent_FName] = Value;

                    // fields tablsou varsa
                    if (dsData.Tables.Count > 1)
                    {
                        // field tipini öğrenelim
                        string displayFormat = string.Empty;
                        int ftype = t.Find_Field_Type_Id(dsData, Key_FName, ref displayFormat);

                        if ((t.IsNotNull(Key_FName)) &&
                            (t.IsNotNull(Value)) &&
                            (ftype == 167) // VarChar() ise
                            )
                        {
                            New_row[Key_FName] = Value + ".";
                        }
                    }
                }

                //dsData.Tables[0].Namespace = "NewRecord";

                old_PositionChange = v.con_PositionChange;
                v.con_NewRecords = true;
                v.con_PositionChange = true;

                dsData.Tables[0].Rows.Add(New_row);

                if (dN != null)
                {
                    NavigatorButton btn = dN.Buttons.Last;
                    dN.Buttons.DoClick(btn);
                }

                v.con_NewRecords = false;
                /// geldiğinde true ise true kalsın diye
                if (old_PositionChange == false)
                    v.con_PositionChange = false;

                // yeni data işleminden sonra yapılacak işlemler
                tAfter_RUN(tForm, dsData, "tNewData");

                Control cntrl = null;
                string[] controls = new string[] { };
                // diğer _yeni_hesap larda olacak
                cntrl = t.Find_Control(tForm, "simpleButton_yeni_hesap", TableIPCode, controls);
                YeniButtonChangeText(cntrl);

                // Control Enabled
                t.ViewControl_Enabled(tForm, dsData, TableIPCode);
                // bu IPCode bağlı ExternalIPCode olabilir...
                t.ViewControl_Enabled_ExtarnalIP(tForm, dsData);

            }
            #endregion  dsData

        }

        private void YeniButtonChangeText(Control cntrl)
        {
            if (cntrl == null) return;

            tToolBox t = new tToolBox();

            if (((DevExpress.XtraEditors.SimpleButton)cntrl).Text != "Vazgeç")
            {
                // mevcut captionu sakla  ( .Tag = "Yeni Hesap" )
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Tag = ((DevExpress.XtraEditors.SimpleButton)cntrl).Text;
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Text = "Vazgeç";
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("YENIVAZGEC16");
            }
            else
            {
                // Vazgeç yerine eskisini yaz
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Text = ((DevExpress.XtraEditors.SimpleButton)cntrl).Tag.ToString();
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("YENIHESAP16");
            }
        }

        private void tAfter_RUN(Form tForm, DataSet dSData, string functionName)
        {
            /// x işleminden sonra yapılacak işlemler

            tToolBox t = new tToolBox();

            string myProp = dSData.Namespace.ToString();

            if (functionName == "tNewData")
            {
                if (myProp.IndexOf("=DataWizardTabPage:True;") > -1)
                {
                    string TableIPCode = t.MyProperties_Get(myProp, "TableIPCode:");

                    t.PageChange(tForm, TableIPCode, "FIRST");
                }
            }

        }

        #endregion   tNewData

        #region tExtraCancel_Data // 153 ( Yeni / Vazgeç ) işlemi
        public void tExtraCancel_Data(Form tForm, string Prop_Navigator_Block)
        {
            tToolBox t = new tToolBox();

            #region örnek
            /*
            2=ROW_PROP_NAVIGATOR:2;
            2=CAPTION:EXTRA CARİYİ YENİLE;
            2=BUTTONTYPE:53;
            2=TABLEIPCODE_LIST:TABLEIPCODE_LIST={
            
            1=ROW_TABLEIPCODE_LIST:1;
            1=CAPTION:CARİYİ YENİLE;
            1=TABLEIPCODE:HCR.HCR_YBR_TESTB2;
            1=TABLEALIAS:null;
            1=KEYFNAME:null;
            1=RTABLEIPCODE:null;
            1=RKEYFNAME:null;
            1=MSETVALUE:null;
            1=WORKTYPE:NEW;
            1=CONTROLNAME:null;
            1=ROWE_TABLEIPCODE_LIST:1;
            TABLEIPCODE_LIST=};

            2=FORMNAME:null;
            2=FORMCODE:null;
            2=FORMTYPE:null;
            2=FORMSTATE:null;
            2=CHC_IPCODE:null;
            2=CHC_FNAME:null;
            2=CHC_VALUE:null;
            2=CHC_OPERAND:null;
            2=ROWE_PROP_NAVIGATOR:2;
            */
            #endregion örnek

            string s = Prop_Navigator_Block;

            if (s.IndexOf("TABLEIPCODE_LIST") > -1)
            {
                v.con_ExtraChange = true;

                string TABLEIPCODE = string.Empty;
                string WORKTYPE = string.Empty;
                string row_block = string.Empty;
                string lockE = "=ROWE_";

                while (s.IndexOf(lockE) > -1)
                {
                    row_block = t.Find_Properies_Get_RowBlock(ref s, "TABLEIPCODE_LIST");
                    WORKTYPE = t.MyProperties_Get(row_block, "WORKTYPE:");

                    /// New pozisyonunda bekleyen ds lerin oluşturulan yeni row ları iptal edilecek
                    if (WORKTYPE == "NEW")
                    {
                        TABLEIPCODE = t.MyProperties_Get(row_block, "TABLEIPCODE:");

                        if (t.IsNotNull(TABLEIPCODE))
                        {
                            tCancelData(tForm, null, TABLEIPCODE);
                        }
                    } //if (row_block.IndexOf 
                } //while

                v.con_ExtraChange = false;
            }

        }
        #endregion

        #region tCancelData
        public void tCancelData(Form tForm, object sender, string TableIPCode)
        {
            tToolBox t = new tToolBox();

            DataSet dsData = null;
            DataNavigator dN = null;
            t.Find_DataSet(tForm, ref dsData, ref dN, TableIPCode);

            if (dsData != null)
            {
                if (dsData.Tables[0].CaseSensitive == true)
                    dsData.Tables[0].CaseSensitive = false;

                NavigatorButton btn = dN.Buttons.Remove;
                dN.Buttons.DoClick(btn);
                dsData.AcceptChanges();

                if (sender != null)
                {
                    // sakladığın caption yeniden kullan ( Vazgeç <<< Yeni Hesap )
                    if (((DevExpress.XtraEditors.SimpleButton)sender).Tag != null)
                    {
                        ((DevExpress.XtraEditors.SimpleButton)sender).Text =
                            ((DevExpress.XtraEditors.SimpleButton)sender).Tag.ToString();
                        ((DevExpress.XtraEditors.SimpleButton)sender).Image = t.Find_Glyph("40_401_AddFile_16x16");
                    }
                }

                ////if (v.con_PositionChange == false)
                ////{
                ////    if (dN.IsAccessible == true)
                ////    {
                ////        vSubWork vSW = new vSubWork();
                ////        vSW._01_tForm = tForm;
                ////        vSW._02_TableIPCode = TableIPCode;
                ////        vSW._03_WorkTD = v.tWorkTD.Refresh;
                ////        vSW._04_WorkWhom = v.tWorkWhom.Childs;
                ////        //tSubWork_(vSW);
                ////    }
                ////}

                ///
                t.ViewControl_Enabled(tForm, dsData, TableIPCode);
                /// bu IPCode bağlı ExternalIPCode olabilir...
                t.ViewControl_Enabled_ExtarnalIP(tForm, dsData);
            }
        }
        #endregion tCancelData

        #region button ile diğer func

        private void YaziciIslemi(Form tForm, string TableIPCode)
        {
            tToolBox t = new tToolBox();

            //MessageBox.Show("yzc");

            Control cntrl = null;
            cntrl = t.Find_Control_View(tForm, TableIPCode);

            if (cntrl != null)
            {
                GridControl grid = null;
                if (cntrl.ToString() == "DevExpress.XtraGrid.GridControl")
                    grid = cntrl as GridControl;

                grid.ShowRibbonPrintPreview();
            }

        }

        private void OnayIslemi(Form tForm, string TableIPCode, string ButtonName)
        {
            tToolBox t = new tToolBox();

            DataNavigator tDataNavigator = t.Find_DataNavigator(tForm, TableIPCode);
            if (tDataNavigator != null)
            {
                object tDataTable = tDataNavigator.DataSource;
                DataSet dsData = ((DataTable)tDataTable).DataSet;

                if (t.IsNotNull(dsData))
                {
                    v.con_OnayChange = true;

                    int i2 = dsData.Tables[0].Rows.Count;
                    for (int i = 0; i < i2; i++)
                    {
                        if (ButtonName == "onayla")
                            dsData.Tables[0].Rows[i]["LKP_ONAY"] = 1;
                        if (ButtonName == "onay_iptal")
                            dsData.Tables[0].Rows[i]["LKP_ONAY"] = 0;
                    }

                    if (tDataNavigator.IsAccessible == true)
                    {
                        Data_Refresh(tForm, dsData, tDataNavigator);
                    }
                    v.con_OnayChange = false;
                }
            }
        }

        private void CollExpIslemi(Form tForm, string TableIPCode, string ButtonName)
        {
            tToolBox t = new tToolBox();

            Control cntrl = t.Find_Control_View(tForm, TableIPCode);

            if (cntrl != null)
            {
                if (cntrl.ToString() == "DevExpress.XtraGrid.GridControl")
                {
                    //if (ButtonName == "Collapse")
                    //    ((DevExpress.XtraGrid.GridControl)cntrl)
                    //if (ButtonName == "Expand")
                    //    ((DevExpress.XtraGrid.GridControl)cntrl).MainView.ExpandAllRows();

                    //((GridView)grid.MainView).ExpandAllGroups();

                    if (((DevExpress.XtraGrid.GridControl)cntrl).MainView.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
                    {
                        GridView view = ((DevExpress.XtraGrid.GridControl)cntrl).MainView as GridView;
                        if (ButtonName == "Expand")
                            view.ExpandAllGroups();
                        if (ButtonName == "Collapse")
                            view.CollapseAllGroups();
                    }

                    if (((DevExpress.XtraGrid.GridControl)cntrl).MainView.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
                    {
                        AdvBandedGridView view = ((DevExpress.XtraGrid.GridControl)cntrl).MainView as AdvBandedGridView;
                        if (ButtonName == "Expand")
                            view.ExpandAllGroups();
                        if (ButtonName == "Collapse")
                            view.CollapseAllGroups();
                    }

                }

                if (cntrl.ToString() == "DevExpress.XtraVerticalGrid.VGridControl")
                {
                    if (ButtonName == "Collapse")
                        ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).CollapseAllRows();
                    if (ButtonName == "Expand")
                        ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).ExpandAllRows();
                }

                if (cntrl.ToString() == "DevExpress.XtraTreeList.TreeList")
                {
                    if (ButtonName == "Collapse")
                        ((DevExpress.XtraTreeList.TreeList)cntrl).CollapseAll();
                    if (ButtonName == "Expand")
                        ((DevExpress.XtraTreeList.TreeList)cntrl).ExpandAll();
                }

            }

        }

        private void SatirSil(Form tForm, string TableIPCode)
        {
            tToolBox t = new tToolBox();

            DataNavigator tDataNavigator = t.Find_DataNavigator(tForm, TableIPCode);
            if (tDataNavigator != null)
            {
                object tDataTable = tDataNavigator.DataSource;
                DataSet dsData = ((DataTable)tDataTable).DataSet;
                int pos = tDataNavigator.Position;

                if (pos == -1) return;

                // Gerekli olan verileri topla
                vTable vt = new vTable();
                t.Preparing_DataSet(dsData, vt);

                //string myProp = dsData.Namespace;
                //string TableName = t.MyProperties_Get(myProp, "TableName:");
                //string KeyFName = t.MyProperties_Get(myProp, "KeyFName:");
                //Int16 DBaseNo = t.Set(t.MyProperties_Get(myProp, "DBaseNo:"),"", (byte)0);


                if (t.IsNotNull(vt.TableName) && t.IsNotNull(vt.KeyId_FName))
                {
                    string RefId = dsData.Tables[vt.TableName].Rows[pos][vt.KeyId_FName].ToString();

                    if (t.IsNotNull(RefId))
                    {
                        string single_select =
                            " Select * from " + vt.TableName +
                            " where " + vt.KeyId_FName + " = " + RefId;

                        string delete_sql =
                            " Delete from " + vt.TableName +
                            " where " + vt.KeyId_FName + " = " + RefId;

                        DataSet ds_SingleRow = new DataSet();
                        //t.Data_Read_Execute(ds_SingleRow, ref single_select, TableName, null);
                        t.SQL_Read_Execute(vt.DBaseNo, ds_SingleRow, ref single_select, vt.TableName, "");


                        if (t.IsNotNull(ds_SingleRow))
                        {
                            tCreateObject co = new tCreateObject();
                            if (co.Create_Delete_Form(ds_SingleRow, TableIPCode) == DialogResult.Yes)
                            {
                                try
                                {
                                    //if (t.SQL_ExecuteNon(ds_SingleRow, ref delete_sql, null))
                                    if (t.Sql_ExecuteNon(ds_SingleRow, ref delete_sql, vt))
                                    {
                                        // buton click ten dolayı tDataSave gidiyor, gidince fonksiyonun 
                                        // girişi geri dönmesi için true ataması yapılıyor
                                        v.con_LkpOnayChange = true;

                                        NavigatorButton btn = tDataNavigator.Buttons.Remove;
                                        tDataNavigator.Buttons.DoClick(btn);

                                        dsData.AcceptChanges();

                                        //if (myProp.IndexOf("Prop_Runtime:True") > 0)
                                        if (vt.RunTime)
                                        {
                                            //tEvents ev = new tEvents();
                                            Prop_RunTimeClick(tForm, null, TableIPCode, v.nv_26_Sil_Satir);
                                        }
                                        v.Kullaniciya_Mesaj_Var = "Kayıt SİLME işlemi başarıyla sonuçlandı.!";

                                        // görevi bitti
                                        v.con_LkpOnayChange = false;
                                    }
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show("DİKKAT : Silme işlemi sırasında bir sorun oluştu..." + v.ENTER2 + e.Message.ToString());
                                    //throw;
                                }
                            }

                        }
                    }
                }
            }
        }

        private void Run_Search(Form tForm, string TableIPCode, string myProp)
        {
            // Run Search Form
            run_Prop_Navigator(tForm, TableIPCode, myProp, v.tNavigatorButton.nv_58_Search);
            //
            // search işleminden hemen sonra datası okunmak için TableIPCode olabilir
            // ve yine search işleminden sonra veriler hesaplanmak istenebilir
            if (v.searchOnay)
            {
                // okunan bu IP den SETDATA yapılmaktadır
                if (myProp.IndexOf("'BUTTONTYPE': '56'") > -1)
                    run_Prop_Navigator(tForm, TableIPCode, myProp, v.tNavigatorButton.nv_56_Run_TableIPCode);

                // çalıştırılacak formüller var ise git hesapla
                if (myProp.IndexOf("'BUTTONTYPE': '55'") > -1)
                    run_Prop_Navigator(tForm, TableIPCode, myProp, v.tNavigatorButton.nv_55_Run_Expression);
            }
        }

        private void Run_Expression(Form tForm, string TableIPCode)
        {
            // hesaplamalar çalışıyor
            myWork_EXPRESSION(tForm, TableIPCode, "", "");
        }

        private void InputBox(Form tForm, string TableIPCode, PROP_NAVIGATOR prop_)
        {
            tToolBox t = new tToolBox();
            vUserInputBox iBox = new vUserInputBox();

            bool onay = true;
            string target_FName = string.Empty;
            string read_FName = string.Empty;
            string read_Caption = string.Empty;
            string inputbox_Message = string.Empty;
            string inputbox_Default1 = string.Empty;
            string input_Value = string.Empty;

            //foreach (PROP_NAVIGATOR item in prop_)
            //{ }

            ///"TABLEIPCODE_LIST": [
            ///{
            ///  "CAPTION": "Miktar Sor",
            ///  "TABLEIPCODE": "null",
            ///  "TABLEALIAS": "null",
            ///  "KEYFNAME": "MIKTAR",
            ///  "RTABLEIPCODE": "null",
            ///  "RKEYFNAME": "URUN_ADI",
            ///  "MSETVALUE": "null",
            ///  "WORKTYPE": "IBOX",
            ///  "CONTROLNAME": "null",
            ///  "DCCODE": "null"
            ///}

            int pos = -1;
            DataSet dS = null;
            DataNavigator dN = null;
            t.Find_DataSet(tForm, ref dS, ref dN, TableIPCode);

            if (dN != null) pos = dN.Position;

            foreach (TABLEIPCODE_LIST item in prop_.TABLEIPCODE_LIST)
            {
                if (item.WORKTYPE.ToString() == "IBOX")
                {
                    target_FName = item.KEYFNAME.ToString();
                    read_FName = item.RKEYFNAME.ToString();
                    inputbox_Message = item.CAPTION.ToString();
                    inputbox_Default1 = item.MSETVALUE.ToString();
                    break;
                }
            }
                        
            string displayFormat = string.Empty;
            int ftype = t.Find_Field_Type_Id(dS, target_FName, ref displayFormat);

            input_Value = "";
            if (t.IsNotNull(inputbox_Default1))
                input_Value = inputbox_Default1;
            
            // inputBox kimin için soru soruyor bunu anlamak için
            // RKEYFNAME de display fieldname saklanıyor, ve o field deki isim okunarak 
            // inputboxda display edilecek
            // örnek : 
            // EKMEK için           << display fieldName << RKEYFNAME
            // Lütfen Miktar girin  << inputbox_message
            #region

            if (pos > -1)
            {
                if (t.IsNotNull(target_FName))
                    input_Value = dS.Tables[0].Rows[pos][target_FName].ToString();
                if (t.IsNotNull(read_FName))
                    read_Caption = dS.Tables[0].Rows[pos][read_FName].ToString();
            }

            if (t.IsNotNull(input_Value) == false)
                input_Value = "1";

            inputbox_Message = read_Caption + " için " + v.ENTER + inputbox_Message;
            
            #endregion

            iBox.Clear();
            iBox.title = "Veri Girişi";
            iBox.promptText = inputbox_Message;
            iBox.value = input_Value;
            iBox.displayFormat = displayFormat;
            iBox.fieldType = ftype;

            DialogResult dialogResult = t.UserInpuBox(iBox);

            input_Value = iBox.value;

            switch (dialogResult)
            {
                case DialogResult.OK:
                    {
                        onay = true;
                        break;
                    }
                case DialogResult.Cancel:
                    {
                        onay = false;
                        break;
                    }
            }

            if (onay)
            {
                // ilk atama yapılıyor
                dS.Tables[0].Rows[pos][target_FName] = input_Value;
                

                /// hesaplama görevi Run_Expression metoduna atandı
                // hesaplamalar çalışıyor
                //myWork_EXPRESSION(tForm, TableIPCode, v.con_Expression_FieldName, input_Value);
                                
                // atama kabul ediliyor
                dS.Tables[0].AcceptChanges();
                // tekrar Editlemek için en son yapılan atama tekrar yapılıyor
                dS.Tables[0].Rows[pos][target_FName] = input_Value;

                // atanan veriler anında viewcontrol üzerinde görünsün
                viewControl_FocusedValue(tForm, TableIPCode);
                
                //
                iBox.Clear();
                v.con_Expression_FieldName = "";
            }
            
        }

        private void Run_TableIPCode(Form tForm, string TableIPCode, PROP_NAVIGATOR prop_)
        {
            /// Run_TableIPCode ile burada okunan data ile SETDATA işlemi yapılmaktadır
            /// yani istenen datayı oku, ekrandaki başka bir dataset e ata.
            /// Örnek : Fiyat Tablosunu oku Faturya set et...
            /// 
            tToolBox t = new tToolBox();

            // burda yeni okunacak data bulunmakta
            // 
            string read_TableIPCode = prop_.TARGET_TABLEIPCODE.ToString();

            if (t.IsNotNull(read_TableIPCode))
            {
                tInputPanel ip = new tInputPanel();

                // DENEME İÇİN KAPATTIM GEREKSİZ GALİBA  07.04.2018
                //ip.Create_InputPanel(tForm, tPanelControl, Read_TableIPCode, 1);

                DataSet dsRead = ip.Create_DataSet(tForm, read_TableIPCode);
                DataNavigator dNRead = new DataNavigator();
                if (t.IsNotNull(dsRead))
                {
                    dNRead.DataSource = dsRead.Tables[0];
                    dNRead.Position = 0;
                }              

                DataSet dsTarget = null;
                DataNavigator dNTarget = null;
                t.Find_DataSet(tForm, ref dsTarget, ref dNTarget, TableIPCode);
                
                tSetData_(prop_.TABLEIPCODE_LIST,
                          dsRead, dNRead, 
                          dsTarget, dNTarget);

                // deneme 
                /// hesaplama görevi Run_Expression metoduna atandı
                // hesaplamalar çalışıyor
                //v.con_Expression_FieldName
                //myWork_EXPRESSION(tForm, TableIPCode, "", "");
            }
        }

        private void tSetData_(List<TABLEIPCODE_LIST> TableIPCodeList,
            DataSet read_dsData,
            DataNavigator read_dN,
            DataSet target_dsData,
            DataNavigator target_dN
            )
        {
            ///"TABLEIPCODE_LIST": [
            ///    {
            ///    "CAPTION": "ID > Cari ID",
            ///    "TABLEIPCODE": "null",
            ///    "TABLEALIAS": "null",
            ///    "KEYFNAME": "CARI_ID",
            ///    "RTABLEIPCODE": "null",
            ///    "RKEYFNAME": "ID",
            ///    "MSETVALUE": "null",
            ///    "WORKTYPE": "SETDATA",
            ///    "CONTROLNAME": "null",
            ///    "DCCODE": "null"
            ///    }
            ///    

            /// new search value set
            if (TableIPCodeList != null)
            {
                tToolBox t = new tToolBox();

                string T_FNAME = string.Empty;
                string R_FNAME = string.Empty;
                //string MSETVALUE = string.Empty;

                ///{
                ///  "CAPTION": "ID > Cari ID",
                ///  "TABLEIPCODE": "null",
                ///  "TABLEALIAS": "null",
                ///  "KEYFNAME": "CARI_ID",
                ///  "RTABLEIPCODE": "null",
                ///  "RKEYFNAME": "ID",
                ///  "MSETVALUE": "null",
                ///  "WORKTYPE": "SETDATA",
                ///  "CONTROLNAME": "null",
                ///  "DCCODE": "null"
                ///},

                foreach (TABLEIPCODE_LIST item in TableIPCodeList)
                {
                    T_FNAME = item.KEYFNAME.ToString();
                    R_FNAME = item.RKEYFNAME.ToString();
                    
                    if (t.IsNotNull(read_dsData))
                    {
                        try
                        {
                            if (read_dN.Position > -1)
                                item.MSETVALUE = read_dsData.Tables[0].Rows[read_dN.Position][R_FNAME].ToString();
                            else item.MSETVALUE = read_dsData.Tables[0].Rows[0][R_FNAME].ToString(); // position == -1 ise
                        }
                        catch (Exception e1)
                        {
                            //throw;
                            MessageBox.Show("HATA : " + R_FNAME + v.ENTER2 + e1.ToString());
                        }
                    }

                    if (t.IsNotNull(target_dsData))
                    {
                        try
                        {
                            target_dsData.Tables[0].Rows[target_dN.Position][T_FNAME] = item.MSETVALUE;
                        }
                        catch (Exception e2)
                        {
                            //throw;
                            MessageBox.Show("HATA : " + T_FNAME + v.ENTER2 + e2.ToString());
                        }
                    }

                }
            }

        }

        #endregion button ile diğer func

        #endregion dataNavigator Events

        #region myGridView Events

        private Form myGridFormAndTableIPCode_Get(object sender, ref string TableIPCode)
        {
            Form tForm = null;

            string s = string.Empty;

            if (sender.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
            {
                tForm = (((DevExpress.XtraGrid.Views.Grid.GridView)sender).GridControl).FindForm();
                TableIPCode = (((DevExpress.XtraGrid.Views.Grid.GridView)sender).GridControl).AccessibleName;
            }
            if (sender.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
            {
                tForm = (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).GridControl).FindForm();
                TableIPCode = (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).GridControl).AccessibleName;
            }
            if (sender.ToString() == "DevExpress.XtraEditors.ButtonEdit")
            {
                tForm = ((DevExpress.XtraEditors.ButtonEdit)sender).FindForm();
                TableIPCode = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleName;
            }
            if (sender.ToString() == "DevExpress.XtraEditors.TextEdit")
            {
                tForm = ((DevExpress.XtraEditors.TextEdit)sender).FindForm();
                TableIPCode = ((DevExpress.XtraEditors.TextEdit)sender).AccessibleName;
            }

            return tForm;
        }

        private void myGridHint_(object sender,
            ref Form tForm,
            ref string TableIPCode,
            ref string grid_Prop_Navigator,
            ref string column_Prop_Navigator,
            ref string column_Value)
        {

            tForm = myGridFormAndTableIPCode_Get(sender, ref TableIPCode);

            if (sender.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
            {
                if ((((DevExpress.XtraGrid.Views.Grid.GridView)sender).GridControl).AccessibleDescription != null)
                    grid_Prop_Navigator = (((DevExpress.XtraGrid.Views.Grid.GridView)sender).GridControl).AccessibleDescription;

                if (((DevExpress.XtraGrid.Views.Grid.GridView)sender).FocusedColumn != null)
                {
                    if (((DevExpress.XtraGrid.Views.Grid.GridView)sender).FocusedColumn.ColumnEdit.AccessibleDescription != null)
                        column_Prop_Navigator = ((DevExpress.XtraGrid.Views.Grid.GridView)sender).FocusedColumn.ColumnEdit.AccessibleDescription;
                }
                if (((DevExpress.XtraGrid.Views.Grid.GridView)sender).EditingValue != null)
                    column_Value = ((DevExpress.XtraGrid.Views.Grid.GridView)sender).EditingValue.ToString();

                if ((string.IsNullOrEmpty(column_Value)) && (column_Value != ""))
                    column_Value = ((DevExpress.XtraGrid.Views.Grid.GridView)sender).GetFocusedValue().ToString();
            }

            if (sender.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
            {
                if ((((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).GridControl).AccessibleDescription != null)
                    grid_Prop_Navigator = (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).GridControl).AccessibleDescription;

                if (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).FocusedColumn != null)
                {
                    if (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).FocusedColumn.ColumnEdit != null)
                        if (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).FocusedColumn.ColumnEdit.AccessibleDescription != null)
                            column_Prop_Navigator = ((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).FocusedColumn.ColumnEdit.AccessibleDescription;
                }
                if (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).EditingValue != null)
                    column_Value = ((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).EditingValue.ToString();

                if ((string.IsNullOrEmpty(column_Value)) && (column_Value != ""))
                    column_Value = ((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).GetFocusedValue().ToString();
            }

            if (sender.ToString() == "DevExpress.XtraEditors.ButtonEdit")
            {
                // parent ile GridControl geliyor 
                //MessageBox.Show(((DevExpress.XtraEditors.ButtonEdit)sender).Parent.Name.ToString());

                if (((DevExpress.XtraEditors.ButtonEdit)sender).Properties.AccessibleDescription != null)
                    column_Prop_Navigator = ((DevExpress.XtraEditors.ButtonEdit)sender).Properties.AccessibleDescription;

                if (((DevExpress.XtraEditors.ButtonEdit)sender).EditValue != null)
                    column_Value = ((DevExpress.XtraEditors.ButtonEdit)sender).EditValue.ToString();
            }

            if (sender.ToString() == "DevExpress.XtraEditors.TextEdit")
            {
                if (((DevExpress.XtraEditors.TextEdit)sender).Properties.AccessibleDescription != null)
                    column_Prop_Navigator = ((DevExpress.XtraEditors.TextEdit)sender).Properties.AccessibleDescription;

                if (((DevExpress.XtraEditors.TextEdit)sender).EditValue != null)
                    column_Value = ((DevExpress.XtraEditors.TextEdit)sender).EditValue.ToString();
            }

        }

        private void myWork_Keys_(object sender, KeyEventArgs e)
        {
            Form tForm = null;
            string TableIPCode = string.Empty;
            string grid_Prop_Navigator = string.Empty;
            string column_Prop_Navigator = string.Empty;
            string column_Value = string.Empty;

            myGridHint_(sender,
                ref tForm,
                ref TableIPCode,
                ref grid_Prop_Navigator,
                ref column_Prop_Navigator,
                ref column_Value);
            
            if (e.KeyCode == v.Key_SearchEngine)
            {
                v.con_Value_Old = column_Value;

                //if ((v.con_Value_New != v.con_Value_Old) && (v.con_Value_New != "-1"))
                if (v.con_Value_New != "-1")
                {
                    string myProp = column_Prop_Navigator.Replace((char)34, (char)39);
                    
                    Control cntrl = null;

                    if (sender.ToString() == "DevExpress.XtraEditors.ButtonEdit")
                        cntrl = (Control)sender;

                    Run_Search(tForm, TableIPCode, myProp);
                                        
                    if (cntrl != null)
                        tForm.ActiveControl = cntrl;
                }
            } 
            else 
            if (e.KeyCode == v.Key_NewLine)
            {
                // grid için yeni satır oluştur
                tSave sv = new tSave();
                if (sv.tDataSave(tForm, TableIPCode))
                {
                    tToolBox t = new tToolBox();
                    t.ButtonEnabledAll(tForm, TableIPCode, true);
                    tNewData(tForm, TableIPCode);
                    t.tFormActiveView(tForm, TableIPCode);
                }
            }
            else
            {
                navigatorButtonExec_Keys(tForm, e.KeyCode, TableIPCode, grid_Prop_Navigator);
            }

            

            // ? bu ne
            //if (e.Control && e.KeyCode == Keys.F4) return;

        }


        private void myWork_EXPRESSION(Form tForm, string TableIPCode, string fieldName, string newValue)
        {
            tToolBox t = new tToolBox();

            DataSet dsData = null;
            DataNavigator tDataNavigator = null;
            t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TableIPCode);

            //Application.OpenForms[0].Text = "Expression : " + e.Column.FieldName + " 1,";

            v.Kullaniciya_Mesaj_Var = "Hesaplar çalışıyor : " + fieldName;

            v.con_Expression_View = "1. Faz ------- " + v.ENTER;
            v.con_Expression_Send_Value = newValue; 
            v.con_Expression = true;
            Preparing_Expression(tForm, dsData, tDataNavigator.Position, TableIPCode, fieldName, v.con_Expression_Send_Value);
            /// bazı formullerde kendinden sonraki fieldlerden değer alıyor 
            /// fakat henüz orada bir işlem yapılmamış olduğu için işlem eksik kalıyor
            /// ikinci defa tekrar hesaplamaya gidince aradığı değeri bulmuş oluyor
            v.con_Expression_View = v.con_Expression_View + "2. Faz ------- " + v.ENTER;
            v.con_Expression = true;
            Preparing_Expression(tForm, dsData, tDataNavigator.Position, TableIPCode, fieldName, v.con_Expression_Send_Value);
            /// -----
        }

        public void myRepositoryItemEdit_KeyDown(object sender, KeyEventArgs e)
        {
            //Application.OpenForms[0].Text = ";" + e.KeyCode.ToString();

            #region KeyCode
            if ((e.KeyCode == Keys.Return) ||
                (e.KeyCode == Keys.Enter) ||
                (e.Control && e.KeyCode == Keys.Y) ||
                (e.Control && e.KeyCode == Keys.C) ||
                (e.Control && e.KeyCode == Keys.F4) ||
                (e.KeyCode == v.Key_SearchEngine) ||
                (e.KeyCode == v.Key_Save) ||
                (e.KeyCode == v.Key_NewLine)
                )
            {
                //if (e.KeyCode == v.Key_NewLine) e.KeyCode 

                myWork_Keys_(sender, e);
            }
            #endregion KeyCode
        }

        private void myGridView_Work_PropNavigator_(object sender, string eventType)
        {
            string Prop_Navigator = "";
            string TableIPCode = string.Empty;

            Form tForm = myGridFormAndTableIPCode_Get(sender, ref TableIPCode);

            if (eventType == "DoubleClick")
            {
                if (sender.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
                {
                    Prop_Navigator = (((DevExpress.XtraGrid.Views.Grid.GridView)sender).GridControl).AccessibleDescription;
                }
                if (sender.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
                {
                    Prop_Navigator = (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).GridControl).AccessibleDescription;
                }
            }

            if (eventType == "FocusedColumn")
            {
                
            }

            if ((Prop_Navigator == "") ||
                (Prop_Navigator == "null") ||
                (Prop_Navigator == null))
            {
                navigatorButtonExec_Keys(tForm, Keys.Return, TableIPCode, "");
                return;
            }
            else
            {
                Prop_Navigator = Prop_Navigator.Replace((char)34, (char)39);

                
                // form aç
                //if (Prop_Navigator.IndexOf("'BUTTONTYPE': '51'") > -1)
                //    navigatorButtonExec_Keys(tForm, Keys.Return, TableIPCode, Prop_Navigator);

                navigatorButtonExec_Keys(tForm, Keys.Return, TableIPCode, Prop_Navigator);
            }
        }

        public void myGridView_GotFocus(object sender, EventArgs e)
        {
            //gridView1.Appearance.FocusedCell.BackColor = Color.PaleGreen;

            if (sender.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
            {
                GridView view = sender as GridView;

                view.Appearance.FocusedCell.BackColor = v.AppearanceFocusedColor;
            }

            if (sender.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
            {
                GridView view = sender as AdvBandedGridView;

                view.Appearance.FocusedCell.BackColor = v.AppearanceFocusedColor;
            }

            if (sender.ToString() == "DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView")
            {
                DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView view = sender as DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView;

                //view.Appearance.     
            }

            if (sender.ToString() == "DevExpress.XtraVerticalGrid.VGridControl")
            {
                DevExpress.XtraVerticalGrid.VGridControl view = sender as DevExpress.XtraVerticalGrid.VGridControl;

                view.Appearance.FocusedCell.BackColor = v.AppearanceFocusedColor;
            }

        }

        public void myGridView_LostFocus(object sender, EventArgs e)
        {
            /// DİKKAT : BURAYI AÇINDA CHECKBOX ÇALIŞMAMAYA BAŞLIYOR
            /// 
            /*
            if (sender.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
            {
                GridView view = sender as GridView;

                //view.Appearance.FocusedCell.BackColor = view.Appearance.FocusedCell.BackColor2;
                //view.Appearance.FocusedCell.BackColor = System.Drawing.Color.Empty;
            }

            if (sender.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
            {
                GridView view = sender as AdvBandedGridView;

                //view.Appearance.FocusedCell.BackColor = view.Appearance.FocusedCell.BackColor2;
                //view.Appearance.FocusedCell.BackColor = System.Drawing.Color.Empty;
            }
            */
        }

        public void myGridView_ColumnFilterChanged(object sender, EventArgs e)
        {
            //
            //e.ToString();

            //Form f = ((DevExpress.XtraGrid.Views.Grid.GridView)sender).GridControl.FindForm();
            //f.Text =  ((DevExpress.XtraGrid.Views.Grid.GridView)sender).RowCount.ToString();
        }

        public void myGridView_DoubleClick(object sender, EventArgs e)
        {
            //MessageBox.Show("myGridView_DoubleClick");

            myGridView_Work_PropNavigator_(sender, "DoubleClick");
        }

        public void myGridView_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            //string TableIPCode = string.Empty;
            //Form tForm = myGridFormAndTableIPCode_Get(sender, ref TableIPCode);
            //tForm.Text = tForm.Text + ", " + e.Column.FieldName;

            if (e.Column.FieldName == "LKP_ONAY")
            {
                v.con_LkpOnayChange = true;

                tToolBox t = new tToolBox();
                string TableIPCode = string.Empty;
                Form tForm = myGridFormAndTableIPCode_Get(sender, ref TableIPCode);
                DataSet dsData = null;
                DataNavigator tDataNavigator = null;
                t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TableIPCode);

                #region Detail-SubDetail Table
                // Kendisine bağlı subdetail var ise
                if (tDataNavigator.IsAccessible == true)
                {
                    if (tDataNavigator.Position > -1)
                    {
                        string value = dsData.Tables[0].Rows[tDataNavigator.Position]["LKP_ONAY"].ToString();
                        if (value == "1")
                        {
                            dsData.Tables[0].Rows[tDataNavigator.Position]["LKP_ONAY"] = 0;
                        }
                        else
                        {
                            dsData.Tables[0].Rows[tDataNavigator.Position]["LKP_ONAY"] = 1;
                        }
                        Data_Refresh(tForm, dsData, tDataNavigator);
                    }
                }
                #endregion Detail-SubDetail Table

            }


            //e.Column.UnboundExpression

        }

        public void myGridView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //Application.OpenForms[0].Text = "myGridView_CellValueChanged : " + sender.ToString() + "; " + e.Column.FieldName.ToString();

            #region 
            if (e.Column.Tag != null)
            {
                if (e.Column.Tag.ToString() == "EXPRESSION")
                {
                    string TableIPCode = string.Empty;
                    Form tForm = myGridFormAndTableIPCode_Get(sender, ref TableIPCode);
                    
                    myWork_EXPRESSION(tForm, TableIPCode, e.Column.FieldName, e.Value.ToString());

                    // Eğer return olmaz ise alttaki fonksiyonlar çalışıyor
                    // bu nedenle return kaldırma

                    //((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).Invalidate();

                    return;
                }

            }
            #endregion

        }

        public void myGridView_ColumnChanged(object sender, EventArgs e)
        {
            //Application.OpenForms[0].Text = "ColumnChanged : " + sender.ToString() + "; " + e.ToString();
        }

        public void myGridView_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            //Application.OpenForms[0].Text = e.FocusedColumn.FieldName;

            //string TableIPCode = string.Empty;
            //Form tForm = myGridFormAndTableIPCode_Get(sender, ref TableIPCode);
            //tForm.Text = tForm.Text + ", " + e.FocusedColumn.FieldName;

            //e.FocusedColumn.ColumnEdit

            //myGridView_Work_PropNavigator_(e.FocusedColumn, "FocusedColumn");

            Form tForm = null;
            string TableIPCode = string.Empty;
            string grid_Prop_Navigator = string.Empty;
            string column_Prop_Navigator = string.Empty;
            string column_Value = string.Empty;

            myGridHint_(sender,
                ref tForm,
                ref TableIPCode,
                ref grid_Prop_Navigator,
                ref column_Prop_Navigator,
                ref column_Value);
      
            if (string.IsNullOrEmpty(column_Prop_Navigator) == false)
            {
                //MessageBox.Show(column_Value + v.ENTER2 + column_Prop_Navigator);
                column_Prop_Navigator = column_Prop_Navigator.Replace((char)34, (char)39);

                // InputBox
                if (column_Prop_Navigator.IndexOf("'BUTTONTYPE': '57'") > -1)
                {
                    navigatorButtonExec_(tForm, TableIPCode, column_Prop_Navigator, v.tNavigatorButton.nv_57_Input_Box);

                    v.con_Expression = true;
                    v.con_Expression_FieldName = e.FocusedColumn.FieldName;
                }
            }
        }
        
        public void myGridView_KeyDown(object sender, KeyEventArgs e)
        {
            //...
            //MessageBox.Show(sender.ToString());

            v.con_Expression = true;
                        
            #region KeyCode
            if ((e.KeyCode == Keys.Return) ||
                (e.KeyCode == Keys.Enter) ||
                (e.Control && e.KeyCode == Keys.Y) ||
                (e.Control && e.KeyCode == Keys.C) ||
                (e.Control && e.KeyCode == Keys.F4) ||
                (e.KeyCode == v.Key_SearchEngine) ||
                (e.KeyCode == v.Key_Save) ||
                (e.KeyCode == v.Key_NewLine)
                )
            {
                myWork_Keys_(sender, e);
            }
            #endregion KeyCode
                        
        }

        public void myGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.con_Expression = true;

            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ":" + e.KeyChar.ToString();

            
        }

        public void myGridView_KeyUp(object sender, KeyEventArgs e)
        {
            //...
            v.con_Expression = true;

            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";" + e.KeyCode.ToString();

        }

        public void myGridView_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
        }

        public void myGridView_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            /*
            string TableIPCode = (((DevExpress.XtraGrid.Views.Grid.GridView)sender).GridControl).AccessibleName;
            Form tForm = (((DevExpress.XtraGrid.Views.Grid.GridView)sender).GridControl).FindForm();
            DataRow row = ((DevExpress.XtraGrid.Views.Grid.GridView)sender).GetFocusedDataRow();

            tDefaultValue df = new tDefaultValue();
            e.Valid = df. tDefaultValue_Fill_Check(tForm, row, TableIPCode);
            */


            /*  şimdilik gerek değil 
             * 
            tToolBox t = new tToolBox();
            string function_name = "myGridView_ValidateRow";
            t.Takipci(function_name, "", '{');

            //...
            // (((DevExpress.XtraGrid.Views.Grid.GridView)sender).ChildGridLevelName == "true");
            // (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).ChildGridLevelName == "true")

            Form tForm = null;
            string TableIPCode = string.Empty;
            string s = string.Empty;

            if (sender.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
            {
                tForm = (((DevExpress.XtraGrid.Views.Grid.GridView)sender).GridControl).FindForm();
                TableIPCode = (((DevExpress.XtraGrid.Views.Grid.GridView)sender).GridControl).AccessibleName;
                //s = ((DevExpress.XtraGrid.Views.Grid.GridView)sender).ChildGridLevelName;
                ((DevExpress.XtraGrid.Views.Grid.GridView)sender).ChildGridLevelName = "";

            }
            if (sender.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
            {
                tForm = (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).GridControl).FindForm();
                TableIPCode = (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).GridControl).AccessibleName;
                //s = ((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).ChildGridLevelName;
                ((DevExpress.XtraGrid.Views.Grid.GridView)sender).ChildGridLevelName = "";
            }
            */

            //if ((tForm != null) && (t.IsNotNull(TableIPCode)))
            //{
            //tButton btn = new tButton();
            //e.Valid = btn.tTable_Save(tForm, TableIPCode, false, function_name);
            //}

        }

        public void myGridView_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            //public static void myGridView_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
            //...

            if (sender.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
            {
                ((DevExpress.XtraGrid.Views.Grid.GridView)sender).ChildGridLevelName = "true";
            }

            if (sender.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
            {
                ((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).ChildGridLevelName = "true";
            }
        }

        public void myGridView_BeforeLeaveRow(object sender, RowAllowEventArgs e)
        {
            /*
            tToolBox t = new tToolBox();
            
            //...
            // (((DevExpress.XtraGrid.Views.Grid.GridView)sender).ChildGridLevelName == "true");
            // (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).ChildGridLevelName == "true")

            Form tForm = null;
            string TableIPCode = string.Empty;
            string s = string.Empty;

            if (sender.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
            {
                tForm = (((DevExpress.XtraGrid.Views.Grid.GridView)sender).GridControl).FindForm();
                TableIPCode = (((DevExpress.XtraGrid.Views.Grid.GridView)sender).GridControl).AccessibleName;
                s = ((DevExpress.XtraGrid.Views.Grid.GridView)sender).ChildGridLevelName;
                ((DevExpress.XtraGrid.Views.Grid.GridView)sender).ChildGridLevelName = "";

            }
            if (sender.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
            {
                tForm = (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).GridControl).FindForm();
                TableIPCode = (((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).GridControl).AccessibleName;
                s = ((DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView)sender).ChildGridLevelName;
                ((DevExpress.XtraGrid.Views.Grid.GridView)sender).ChildGridLevelName = "";
            }

            if (s == "true")
            {
                //tButton btn = new tButton();
                //btn.tTable_Save(tForm, TableIPCode, true, function_name);
            }
            */
            
        }

        public void myGridView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        public void myGridView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            //v.myGrid_Location = e.Location;

            //Application.OpenForms[0].Text = "MouseDown : " + v.myGrid_Location.ToString();

            if (sender.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
            {
                GridView view = sender as GridView;

                gridHitInfo = null;

                GridHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));

                if (Control.ModifierKeys != Keys.None)
                    return;

                if (e.Button == MouseButtons.Left && hitInfo.InRow && hitInfo.RowHandle != GridControl.NewItemRowHandle)
                    gridHitInfo = hitInfo;
            }

            if (sender.ToString() == "DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView")
            {
                DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView view = sender as DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView;

                winExplorerHitInfo = null;

                WinExplorerViewHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));

                if (Control.ModifierKeys != Keys.None)
                    return;

                if (e.Button == MouseButtons.Left && hitInfo.InItem && hitInfo.RowHandle != GridControl.NewItemRowHandle)
                    winExplorerHitInfo = hitInfo;
            }


        }

        public void myGridView_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //Application.OpenForms[0].Text = "MouseMove : " + ...

            /// DragDrop Edit
            /// 
            /// aslında burası yanlış tetikleme yeri NEDEN ?
            /// gridControl_DragDrop anından sonra gridControl_MouseMove tetikleniyor
            /// onun için burdan iş başlatılmış (tDC_Run)
            /// işin aslı myGridControl_DragDrop sırasında işlem başlamalı
            /// 

            #region Mouse gezerken bulunduğu kaydın position değişmesi sağlanıyor

            /// DİKKAT : Burası DataCopy içindeki tDC_Run tarafından tetikleniyor yani çalışıyor 
            /// DC.WORK_TYPE > de 
            /// 3 = Satır Düzenle, Row.Update()   işlemini yapmak için önce 
            /// burada mouse hangi kaydın üzerinde ise 
            /// DataNavigator ün Position u değiştirmek gerekiyor

            if (v.con_DragDropEdit)
            {
                tToolBox t = new tToolBox();

                // yukarıda sender olan nesne gridControl içindeki view
                // bu view aracılığıyla gridControla ulaşıyoruz
                GridControl grid = null;

                if (sender.ToString() == "DevExpress.XtraGrid.Views.Layout.LayoutView")
                {
                    LayoutView view = sender as DevExpress.XtraGrid.Views.Layout.LayoutView;

                    /// Get a View at the current point.
                    //BaseView view = grid.GetViewAt(e.Location);
                    /// Retrieve information on the current View element.
                    BaseHitInfo baseHI = view.CalcHitInfo(e.Location);
                    LayoutViewHitInfo gridHI = baseHI as LayoutViewHitInfo;

                    if ((gridHI != null) && (gridHI.RowHandle > -1))
                    {
                        //string rowID = view.GetRowCellValue(gridHI.RowHandle, view.Columns[0]).ToString();
                        //string rowID = view.GetFocusedDataSourceRowIndex().ToString();

                        v.con_GridMouseValue = view.GetDataSourceRowIndex(gridHI.RowHandle).ToString();
                        grid = ((DevExpress.XtraGrid.Views.Layout.LayoutView)sender).GridControl as GridControl;
                    }
                }

                if (sender.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
                {
                    GridView view = sender as GridView;
                    BaseHitInfo baseHI = view.CalcHitInfo(e.Location);
                    GridHitInfo gridHI = baseHI as GridHitInfo;

                    if ((gridHI != null) && (gridHI.RowHandle > 0))
                    {
                        v.con_GridMouseValue = view.GetDataSourceRowIndex(gridHI.RowHandle).ToString();
                        grid = ((DevExpress.XtraGrid.Views.Grid.GridView)sender).GridControl as GridControl;
                    }
                }

                if (grid != null)
                {
                    Form tForm = t.Find_Form(grid);
                    string Prop_RunTime = grid.AccessibleDescription;

                    // Herzaman yemiyor
                    //if (gridHI.InLayoutItem)
                    //    view.FocusedRowHandle = gridHI.RowHandle;
                    // bunun yerine manuel yaptım 
                    myGridDataNavigatorPositionSet(grid, t.myInt32(v.con_GridMouseValue));

                    myDragDrop_RUN_(tForm, Prop_RunTime);

                    grid.Invalidate();

                    // işi sonlandır
                    v.con_DragDropEdit = false;
                }
            }

            #endregion Mouse gezerken


            #region Drag anında mouse nin ucunda kutu ve + işareti oluşturyor

            if (sender.ToString() == "DevExpress.XtraGrid.Views.Layout.LayoutView")
            {
                LayoutView view = sender as DevExpress.XtraGrid.Views.Layout.LayoutView;
                if (e.Button == MouseButtons.Left && gridHitInfo != null)
                {
                    Size dragSize = SystemInformation.DragSize;
                    Rectangle dragRect = new Rectangle(new Point(gridHitInfo.HitPoint.X - dragSize.Width / 2,
                        gridHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

                    if (!dragRect.Contains(new Point(e.X, e.Y)))
                    {
                        view.GridControl.DoDragDrop(gridHitInfo, DragDropEffects.All);
                        gridHitInfo = null;
                    }
                }
            }

            if (sender.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
            {
                GridView view = sender as GridView;
                if (e.Button == MouseButtons.Left && gridHitInfo != null)
                {
                    Size dragSize = SystemInformation.DragSize;
                    Rectangle dragRect = new Rectangle(new Point(gridHitInfo.HitPoint.X - dragSize.Width / 2,
                        gridHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

                    if (!dragRect.Contains(new Point(e.X, e.Y)))
                    {
                        view.GridControl.DoDragDrop(gridHitInfo, DragDropEffects.All);
                        gridHitInfo = null;
                    }
                }
            }

            if (sender.ToString() == "DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView")
            {
                DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView view =
                    sender as DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView;

                if (e.Button == MouseButtons.Left && winExplorerHitInfo != null)
                {
                    Size dragSize = SystemInformation.DragSize;
                    Rectangle dragRect = new Rectangle(new Point(winExplorerHitInfo.HitPoint.X - dragSize.Width / 2,
                        winExplorerHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

                    if (!dragRect.Contains(new Point(e.X, e.Y)))
                    {
                        view.GridControl.DoDragDrop(winExplorerHitInfo, DragDropEffects.All);
                        winExplorerHitInfo = null;
                    }
                }
            }

            #endregion Drag anında

        }

        public void myTileView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            TileView view = sender as TileView;
            if (view == null) return;
            tileHitInfo = null;
            TileViewHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));
            if (Control.ModifierKeys != Keys.None) return;
            if (e.Button == MouseButtons.Left && hitInfo.RowHandle >= 0)
                tileHitInfo = hitInfo;
        }

        public void myTileView_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            TileView view = sender as TileView;
            if (view == null) return;
            if (e.Button == MouseButtons.Left && tileHitInfo != null)
            {
                Size dragSize = SystemInformation.DragSize;
                Rectangle dragRect = new Rectangle(new Point(tileHitInfo.HitPoint.X - dragSize.Width / 2,
                    tileHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

                if (!dragRect.Contains(new Point(e.X, e.Y)))
                {
                    DataRow row = view.GetDataRow(tileHitInfo.RowHandle);
                    view.GridControl.DoDragDrop(row, DragDropEffects.Move);
                    tileHitInfo = null;
                    DevExpress.Utils.DXMouseEventArgs.GetMouseArgs(e).Handled = true;
                }
            }
        }

        public void myTileView_ItemClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
            //
            //MessageBox.Show("aaa");
        }

        public void myTileView_ItemDoubleClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
            //
            //MessageBox.Show("ddd");
        }

        public void myCardView_CustomDrawCardCaption(object sender, DevExpress.XtraGrid.Views.Card.CardCaptionCustomDrawEventArgs e)
        {
            CardView view = sender as CardView;
            bool isFocused = e.RowHandle == view.FocusedRowHandle;
            // A brush to draw the background of the card caption.
            Brush backBrush;
            if (isFocused)
            {
                //backBrush = e.Cache.GetGradientBrush(e.Bounds, Color.LavenderBlush, Color.Navy,
                backBrush = e.Cache.GetGradientBrush(e.Bounds, v.AppearanceTextColor, Color.White,
                  LinearGradientMode.Vertical);

                Rectangle r = e.Bounds;
                // Draw a 3D border.

                ControlPaint.DrawBorder3D(e.Graphics, r, Border3DStyle.RaisedInner);

                r.Inflate(-1, -1);
                // Fill the background.
                e.Graphics.FillRectangle(backBrush, r);
                r.Inflate(-2, 0);

                // Draw the text.
                Brush foreBrush = Brushes.Black; //  v.AppearanceFocusedTextColor;
                e.Appearance.DrawString(e.Cache, view.GetCardCaption(e.RowHandle), r, foreBrush);
                // Default painting is not required.
                e.Handled = true;
            }

            /*
   CardView view = sender as CardView;
   bool isFocused = e.RowHandle == view.FocusedRowHandle;
   // A brush to draw the background of the card caption.
   Brush backBrush;
   if(isFocused)            
      backBrush = e.Cache.GetGradientBrush(e.Bounds, Color.LavenderBlush, Color.Navy, 
        LinearGradientMode.Vertical);
   else
      backBrush = e.Cache.GetGradientBrush(e.Bounds, Color.Cornsilk, Color.DarkKhaki, 
        LinearGradientMode.Vertical);
   // A brush to draw the text.
   Brush foreBrush = isFocused ? Brushes.White : Brushes.Chocolate;
   Rectangle r = e.Bounds;
   // Draw a 3D border.
   ControlPaint.DrawBorder3D(e.Graphics, r, Border3DStyle.RaisedInner);
   r.Inflate(-1, -1);
   // Fill the background.
   e.Graphics.FillRectangle(backBrush, r);         
   r.Inflate(-2, 0);         
   // Draw the text.
   e.Appearance.DrawString(e.Cache, view.GetCardCaption(e.RowHandle), r, foreBrush);
   // Default painting is not required.
   e.Handled = true;
            */
        }

        // --- GridControl

        public void myGridControl_DragLeave(object sender, EventArgs e)
        {
            //Application.OpenForms[0].Text = "DragLeave : " + sender.ToString(); 
        }

        public void myGridControl_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            // Application.OpenForms[0].Text = "GiveFeedback : " + sender.ToString();
        }

        public void myGridControl_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            // Application.OpenForms[0].Text = "QueryContinueDrag : " + sender.ToString();
        }

        public void myGridControl_DragEnter(object sender, DragEventArgs e)
        {
            //Application.OpenForms[0].Text = "DragEnter : " + sender.ToString();

            //e.Effect = DragDropEffects.Copy;

            //GridControl grid = sender as GridControl;
            //GridView view = grid.MainView as GridView;

            //v.Row_DragOver = view.GetFocusedDataRow();
        }

        public void myGridControl_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            //v.myGrid_Location = new Point(e.X, e.Y);

            //Application.OpenForms[0].Text = "DragOver : " + sender.ToString() + " : " + v.myGrid_Location.ToString();

            //if (e.Data.GetDataPresent(typeof(DataRow)))
            //    e.Effect = DragDropEffects.Move;
            //else e.Effect = DragDropEffects.None;

            //GridControl grid = sender as GridControl;
            //GridView view = grid.MainView as GridView;

            if (e.Data.GetDataPresent(typeof(DataRow)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.All;  //None;

            if (v.con_DragDropSourceTableIPCode == "")
            {
                GridControl grid = sender as GridControl;
                v.con_DragDropSourceTableIPCode = grid.AccessibleName;
            }
        }

        public void myGridControl_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            //v.myGrid_Location = new Point(e.X, e.Y);
            //Application.OpenForms[0].Text = "DragDrop : " + sender.ToString() + " : " + v.myGrid_Location.ToString();

            tToolBox t = new tToolBox();

            GridControl grid = sender as GridControl;

            #region  

            if (grid.MainView.ToString() == "DevExpress.XtraGrid.Views.Layout.LayoutView")
            {
                LayoutView view = grid.MainView as LayoutView;
                //DataRow row = view.GetFocusedDataRow();
                Point pt = grid.PointToClient(new Point(e.X, e.Y));

                LayoutViewHitInfo hitInfo = view.CalcHitInfo(pt);
                //if (hitInfo.HitTest == LayoutViewHitTest.? //  GridHitTest.EmptyRow)
                //    v.DropTargetRowHandle = view.DataRowCount;
                //else
                v.DropTargetRowHandle = hitInfo.RowHandle;

                if (v.DropTargetRowHandle < 0)
                    v.DropTargetRowHandle = view.SourceRowHandle;

                v.con_GridMouseValue = view.GetDataSourceRowIndex(v.DropTargetRowHandle).ToString();

                grid.Invalidate();
            }

            if (grid.MainView.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
            {
                //GridControl grid = sender as GridControl;
                //GridView view = grid.MainView as GridView;
                //GridHitInfo srcHitInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
                //GridHitInfo hitInfo = view.CalcHitInfo(grid.PointToClient(new Point(e.X, e.Y)));
                //int sourceRow = srcHitInfo.RowHandle;
                //int targetRow = hitInfo.RowHandle;
                //Application.OpenForms[0].Text = view.GetDataSourceRowIndex[sourceRow].ToString();
                //DataRow row = ((DevExpress.XtraGrid.Views.Grid.GridView)sender).GetFocusedDataRow();
                //MoveRow(view, sourceRow, targetRow);


                /// Mouse nin hangi row üzerinde olduğunu bulmak için
                /// aşağıdaki işlemler yapılıyor

                // gridControl içindeki viewi tespit et
                GridView view = grid.MainView as GridView;
                Point pt = grid.PointToClient(new Point(e.X, e.Y));

                GridHitInfo hitInfo = view.CalcHitInfo(pt);
                if (hitInfo.HitTest == GridHitTest.EmptyRow)
                    v.DropTargetRowHandle = view.DataRowCount;
                else
                    v.DropTargetRowHandle = hitInfo.RowHandle;

                /*
                BaseHitInfo baseHI = view.CalcHitInfo(v.myGrid_Location);
                GridHitInfo gridHI = baseHI as GridHitInfo;

                if (gridHI.HitTest == GridHitTest.EmptyRow)
                    v.DropTargetRowHandle = view.DataRowCount;
                else
                    v.DropTargetRowHandle = gridHI.RowHandle;
                */

                if (v.DropTargetRowHandle < 0)
                    v.DropTargetRowHandle = view.SourceRowHandle;

                //if ((gridHI != null) && (gridHI.RowHandle > -1))
                //{
                //    v.con_GridMouseValue = view.GetDataSourceRowIndex(gridHI.RowHandle).ToString();
                //}

                v.con_GridMouseValue = view.GetDataSourceRowIndex(v.DropTargetRowHandle).ToString();

                grid.Invalidate();

                //v.DropTargetRowHandle = -1;
            }

            if (grid.MainView.ToString() == "DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView")
            {
                //DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView view =
                //    grid.MainView as DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView;

                ////DevExpress.XtraGrid.Views.WinExplorer.ViewInfo.WinExplorerViewInfo
                ////WinExplorerViewHitInfo;

                //WinExplorerViewHitInfo srcHitInfo =
                //    e.Data.GetData(typeof(WinExplorerViewHitInfo)) as WinExplorerViewHitInfo;

                //int sourceRow = srcHitInfo.RowHandle;
            }

            if (grid.MainView.ToString() == "DevExpress.XtraGrid.Views.Card.CardView")
            {
                // gridControl içindeki viewi tespit et
                CardView view = grid.MainView as CardView;
                Point pt = grid.PointToClient(new Point(e.X, e.Y));

                CardHitInfo hitInfo = view.CalcHitInfo(pt);
                //if (hitInfo.HitTest == CardHitTest.EmptyRow)
                //    v.DropTargetRowHandle = view.DataRowCount;
                //else
                v.DropTargetRowHandle = hitInfo.RowHandle;

                if (v.DropTargetRowHandle < 0)
                    v.DropTargetRowHandle = view.SourceRowHandle;

                v.con_GridMouseValue = view.GetDataSourceRowIndex(v.DropTargetRowHandle).ToString();

                grid.Invalidate();
            }

            if (grid.MainView.ToString() == "DevExpress.XtraGrid.Views.Tile.TileView")
            {
                DevExpress.XtraGrid.Views.Tile.TileView view = grid.MainView as TileView;

                Point pt = grid.PointToClient(new Point(e.X, e.Y));

                //TileControlHitTest.

                TileViewHitInfo hitInfo = view.CalcHitInfo(pt);
                //if (hitInfo.HitTest == TileControlHitTest.  EmptyRow)
                //    v.DropTargetRowHandle = view.DataRowCount;
                //else
                v.DropTargetRowHandle = hitInfo.RowHandle;

                if (v.DropTargetRowHandle < 0)
                    v.DropTargetRowHandle = view.SourceRowHandle;

                v.con_GridMouseValue = view.GetDataSourceRowIndex(v.DropTargetRowHandle).ToString();

                grid.Invalidate();
            }

            if (grid.MainView.ToString() == "DevExpress.XtraVerticalGrid.VGridControl")
            {

            }
            #endregion

            if (grid != null)
            {
                if (t.IsNotNull(grid.AccessibleDescription))
                {
                    Form tForm = t.Find_Form(sender);
                    string Prop_RunTime = grid.AccessibleDescription;
                    v.con_DragDropTargetTableIPCode = grid.AccessibleName;

                    myGridDataNavigatorPositionSet(grid, t.myInt32(v.con_GridMouseValue));

                    myDragDrop_RUN_(tForm, Prop_RunTime);

                    grid.Invalidate();
                }
            }

        }

        public void myGridControl_Paint(object sender, PaintEventArgs e)
        {
            /*
            // aşağıdaki kodlar çalışmasın diye
            if (v.DropTargetRowHandle != -5) return;
            ///------------------------------------


            if (v.DropTargetRowHandle < 0) return;

            GridControl grid = (GridControl)sender;

            if (grid.MainView.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
            {
                GridView view = (GridView)grid.MainView;

                bool isBottomLine = (v.DropTargetRowHandle + 1) == view.DataRowCount;

                GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                //GridRowInfo rowInfo = viewInfo.GetGridRowInfo(isBottomLine ? v.DropTargetRowHandle - 1 : v.DropTargetRowHandle);
                GridRowInfo rowInfo = viewInfo.GetGridRowInfo(isBottomLine ? v.DropTargetRowHandle : v.DropTargetRowHandle);

                if (rowInfo == null) return;

                Point p1, p2;

                // satırın üsütünü çiyor buna gerek yok sürekli altını çizsin

                //if (isBottomLine)
                //{
                //    p1 = new Point(rowInfo.Bounds.Left, rowInfo.Bounds.Bottom - 1);
                //    p2 = new Point(rowInfo.Bounds.Right, rowInfo.Bounds.Bottom - 1);
                //}
                //else {
                //    p1 = new Point(rowInfo.Bounds.Left, rowInfo.Bounds.Top - 1);
                //    p2 = new Point(rowInfo.Bounds.Right, rowInfo.Bounds.Top - 1);
                //}

                p1 = new Point(rowInfo.Bounds.Left, rowInfo.Bounds.Bottom - 1);
                p2 = new Point(rowInfo.Bounds.Right, rowInfo.Bounds.Bottom - 1);

                e.Graphics.DrawLine(Pens.Blue, p1, p2);

                //v.DropTargetRowHandle = -1;
            }
            */
        }


        private void myGridDataNavigatorPositionSet(GridControl grid, int tPosition)
        {
            tToolBox t = new tToolBox();

            Form tForm = t.Find_Form(grid);
            string target_tableipcode = grid.AccessibleName;

            if (tPosition > -1)
            {
                DataNavigator tDataNavigator = t.Find_DataNavigator(tForm, target_tableipcode);

                tDataNavigator.Position = tPosition;
                //Application.OpenForms[0].Text = ":"+tPosition.ToString();
            }
        }

        private void myDragDrop_RUN_(Form tForm, string Prop_RunTime)
        {
            if ((Prop_RunTime == "") || (Prop_RunTime == null)) return;

            string s1 = "=ROW_PROP_RUNTIME:";
            string s2 = (char)34 + "DRAGDROP" + (char)34 + ": [";

            if (Prop_RunTime.IndexOf(s1) > -1)
            {
                tToolBox t = new tToolBox();
                Prop_RunTime = t.Find_Properies_Get_FieldBlock(Prop_RunTime, "DRAGDROP");
                myDragDrop_RUN_OLD(tForm, Prop_RunTime);
            }
            /// JSON
            if (Prop_RunTime.IndexOf(s2) > -1)
            {
                myDragDrop_RUN_JSON(tForm, Prop_RunTime);
            }

        }

        private void myDragDrop_RUN_JSON(Form tForm, string Prop_RunTime)
        {
            tToolBox t = new tToolBox();
            tDataCopy dc = new tDataCopy();

            string RunTime = string.Empty;
            string Prop_Navigator = string.Empty;
            t.String_Parcala(Prop_RunTime, ref RunTime, ref Prop_Navigator, "|ds|");

            PROP_RUNTIME packet = new PROP_RUNTIME();
            RunTime = RunTime.Replace((char)34, (char)39);
            var prop_ = JsonConvert.DeserializeAnonymousType(RunTime, packet);

            bool sonuc = false;
            string JBTYPE = string.Empty;
            string JBCODE = string.Empty;
            string JBAFTER = string.Empty;
            string JBNAVIGATOR = string.Empty;

            foreach (var item in prop_.DRAGDROP)
            {
                JBTYPE = item.JBTYPE.ToString();
                JBCODE = item.JBCODE.ToString();
                JBAFTER = item.JBAFTER.ToString();
                if (item.JBNAVIGATOR != null)
                    JBNAVIGATOR = item.JBNAVIGATOR.ToString();

                #region JBTYPE "DC"
                if (JBTYPE == "DC")
                {
                    if (t.IsNotNull(JBCODE))
                    {
                        //sonuc = dc.tDC_Run(tForm, v.SP_Conn_Proje_MSSQL, JBCODE);
                        sonuc = dc.tDC_Run(tForm, JBCODE);
                    }
                }
                #endregion JBTYPE

                #region JBTYPE "RPN" = Run Prop Navigator
                if (JBTYPE == "RPN")
                {
                    // tDC_Run() da  work_type = 5 için gerekiyor
                    v.con_DragDropForm = tForm;

                    if (JBNAVIGATOR == "52")
                        navigatorButtonExec_(tForm, "", Prop_Navigator, v.tNavigatorButton.nv_52_Yeni_Kart_FormIle);

                    //v.con_DragDropSourceTableIPCode = "";
                }
                #endregion


                if (sonuc)
                {
                    if ((JBAFTER == "DRD") && t.IsNotNull(v.con_DragDropSourceTableIPCode))
                    {
                        #region Find DataSet
                        DataSet dsData = t.Find_DataSet(tForm, "", v.con_DragDropSourceTableIPCode, "");
                        if (dsData != null)
                        {
                            DataNavigator tDataNavigator = t.Find_DataNavigator(tForm, v.con_DragDropSourceTableIPCode);
                            int pos = tDataNavigator.Position;

                            #region pos > -1
                            if (pos > -1)
                            {
                                // Delete işlemi gerçekleşecek ve 
                                // tdataNavigator_PositionChanged gidecek
                                // Gittiğinde silme olduğu için tDataSave işlemi gerçekleşmeyecek
                                tDataNavigator.Tag = -97;

                                dsData.Tables[0].Rows[pos].Delete();
                                dsData.Tables[0].AcceptChanges();

                                v.con_DragDropSourceTableIPCode = "";

                                // Her zaman tdataNavigator_PositionChanged tetiklenmiyor ( özellikle 0 position da )
                                // bu nedenle atama yapılıyor 
                                tDataNavigator.Tag = tDataNavigator.Position;

                                // kaydet
                                //btn_Navigotor_Click(tForm, target_tableipcode, "kaydet", "24");
                            }
                            #endregion pos > -1
                        }
                        #endregion Find DataSet
                    }
                }



            }


        }

        private void myDragDrop_RUN_OLD(Form tForm, string Prop_RunTime)
        {
            tToolBox t = new tToolBox();

            #region DRAGDROP var ise

            if (t.IsNotNull(Prop_RunTime))
            {
                bool sonuc = false;
                sonuc = myJobs_RUN(tForm, Prop_RunTime);

                #region // JobAfter = JBAFTER:DRD; -- Drop Row Delete
                if ((sonuc) &&
                    (Prop_RunTime.IndexOf("JBAFTER:DRD") > -1))
                {
                    if (t.IsNotNull(v.con_DragDropSourceTableIPCode))
                    {
                        #region Find DataSet
                        DataSet dsData = t.Find_DataSet(tForm, "", v.con_DragDropSourceTableIPCode, "");
                        if (dsData != null)
                        {
                            //int pos = t.Find_DataNavigator_Position(tForm, v.con_DragDropSourceTableIPCode);
                            DataNavigator tDataNavigator = t.Find_DataNavigator(tForm, v.con_DragDropSourceTableIPCode);
                            int pos = tDataNavigator.Position;

                            #region pos > -1
                            if (pos > -1)
                            {
                                // Delete işlemi gerçekleşecek ve 
                                // tdataNavigator_PositionChanged gidecek
                                // Gittiğinde silme olduğu için tDataSave işlemi gerçekleşmeyecek
                                tDataNavigator.Tag = -97;

                                dsData.Tables[0].Rows[pos].Delete();
                                dsData.Tables[0].AcceptChanges();

                                v.con_DragDropSourceTableIPCode = "";

                                // Her zaman tdataNavigator_PositionChanged tetiklenmiyor ( özellikle 0 position da )
                                // bu nedenle atama yapılıyor 
                                tDataNavigator.Tag = tDataNavigator.Position;

                                // kaydet
                                //btn_Navigotor_Click(tForm, target_tableipcode, "kaydet", "24");
                            }
                            #endregion pos > -1
                        }
                        #endregion Find DataSet
                    }
                }
                /*//<ALOCK_0>
                    =BEGIN:;
                    =DRAGDROP://<BLOCK_0>
                    1=BEGIN:1;
                    1=CAPTION:Personel Copy;
                    1=JBTYPE:DC;
                    1=JBCODE:DC_VR_02;
                    1=JBAFTER:DRD;
                    1=END:1;
                    //<BLOCK_1>;
                    =DRAGDROP:FIN;
                    =BUTTON:null;
                    =BUTTON:FIN;
                    =END:;
                    //<ALOCK_1>
                */
                #endregion // JobAfter
            }

            #endregion DRAGDROP
        }

        public bool myJobs_RUN(Form tForm, string jobs)
        {
            tToolBox t = new tToolBox();
            tDataCopy dc = new tDataCopy();

            //string lockE = t.myBlock("tPropertiesPlusEdit", "END");
            string row_block = string.Empty;
            string caption = string.Empty;
            string JBTYPE = string.Empty;
            string JBCODE = string.Empty;
            string JBAFTER = string.Empty;
            bool sonuc = false;
            /* 
            DRAGDROP={
            1=ROW_DRAGDROP:1;
            1=CAPTION:Personeli Vard. Listene Ekle;
            1=JBTYPE:DC;
            1=JBCODE:DC_VR_02;
            1=JBAFTER:DRD;
            1=ROWE_DRAGDROP:1;
            DRAGDROP=};
            */

            string lockE = "=ROWE_";

            while (jobs.IndexOf(lockE) > -1)
            {
                row_block = t.Find_Properies_Get_RowBlock(ref jobs, "DRAGDROP");

                caption = t.MyProperties_Get(row_block, "CAPTION:");
                JBTYPE = t.MyProperties_Get(row_block, "JBTYPE:");
                JBCODE = t.MyProperties_Get(row_block, "JBCODE:");
                JBAFTER = t.MyProperties_Get(row_block, "JBAFTER:");

                if (JBTYPE == "DC")
                {
                    if (t.IsNotNull(JBCODE))
                    {
                        //sonuc = dc.tDC_Run(tForm, v.SP_Conn_Proje_MSSQL, JBCODE);
                        sonuc = dc.tDC_Run(tForm, JBCODE);
                    }
                }
            }

            return sonuc;
        }

        private void MoveRow(GridView view, int sourceRow, int targetRow)
        {

            if (sourceRow == targetRow || sourceRow == targetRow + 1)
                return;

            //GridView view = gridFieldView;
            DataRow row1 = view.GetDataRow(targetRow);
            DataRow row2 = view.GetDataRow(targetRow + 1);
            DataRow dragRow = view.GetDataRow(sourceRow);

            //decimal val1 = (decimal)row1[OrderFieldName];
            //if (row2 == null)
            //    dragRow[OrderFieldName] = val1 + 1;
            //else
            //{
            //    decimal val2 = (decimal)row2[OrderFieldName];
            //    dragRow[OrderFieldName] = (val1 + val2) / 2;
            //}
        }
                
        #endregion myGridView Events

        #region myAdvBandedGridGroupButton Events

        public void checkGridGroupButton_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();
            string function_name = "checkGridGroupButton_Click";
            t.Takipci(function_name, "", '{');

            Form tForm = t.Find_Form(sender);
            int h = ((DevExpress.XtraEditors.CheckButton)sender).TabIndex;
            string TableIPCode = ((DevExpress.XtraEditors.CheckButton)sender).AccessibleDescription;

            Control cntrl = null;
            cntrl = t.Find_Control_View(tForm, TableIPCode);

            if (cntrl != null)
            {
                AdvBandedGridView tView = ((DevExpress.XtraGrid.GridControl)cntrl).MainView as AdvBandedGridView;
                tView.BeginInit();
                int j = tView.Bands.Count;
                for (int i = 0; i < j; i++)
                {
                    if (tView.Bands[i].Fixed == DevExpress.XtraGrid.Columns.FixedStyle.None)
                    {
                        tView.Bands[i].Visible = (i == h);
                    }
                }
                tView.EndInit();
            }

            t.Takipci(function_name, "", '}');
        }

        #endregion myAdvBandedGridGroupButton Events

        #region tXtraEdit_ Events

        public void tXtraEdit_ToggleSwitch_EditValueChanged(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            #region Tanımlar

            string SubDetail_TableIPCode = string.Empty;
            string mst_TableIPCode = string.Empty;
            string FieldName = string.Empty;
            string Value = string.Empty;
            string Tag = string.Empty;

            Form tForm = t.Find_TableIPCode_XtraEditors(sender, ref mst_TableIPCode, ref FieldName, ref Value, ref Tag);
            SubDetail_TableIPCode = mst_TableIPCode; // master da / sub da kendisi

            // yeniden okunacak dataseti bul
            DataSet dsSubDetail_Data = t.Find_DataSet(tForm, "", mst_TableIPCode, "");

            string myProp = dsSubDetail_Data.Namespace.ToString();
            //string SubDetail_List = t.MyProperties_Get(myProp, "About_Detail_SubDetail:");

            int DataReadType = t.myInt32(t.MyProperties_Get(myProp, "DataReadType:"));

            string SqlF = t.MyProperties_Get(myProp, "SqlFirst:");
            string SqlS = t.MyProperties_Get(myProp, "SqlSecond:");

            string Sql_OldF = SqlF;
            string Sql_OldS = SqlS;

            #endregion Tanımlar

            //***
            Preparing_OnOff(ref SqlF, FieldName, Value);

            if (Sql_OldS != "")
                Preparing_OnOff(ref SqlS, FieldName, Value);
            //***

            SubDetail_Run(tForm, dsSubDetail_Data, myProp, SqlF, Sql_OldF, SqlS, Sql_OldS, SubDetail_TableIPCode, DataReadType);

        }

        public void tXtraEdit_ImageComboBoxEdit_EditValueChanged(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            #region Tanımlar

            string SubDetail_TableIPCode = string.Empty;
            string mst_TableIPCode = string.Empty;
            string FieldName = string.Empty;
            string Value = string.Empty;
            string Tag = string.Empty;

            Form tForm = t.Find_TableIPCode_XtraEditors(sender, ref mst_TableIPCode, ref FieldName, ref Value, ref Tag);

            SubDetail_TableIPCode = mst_TableIPCode; // master da / sub da kendisi

            // yeniden okunacak dataseti bul
            DataSet ds_SubDetail = null;
            DataNavigator dN_SubDetail = null;
            t.Find_DataSet(tForm, ref ds_SubDetail, ref dN_SubDetail, SubDetail_TableIPCode);

            if (ds_SubDetail == null) return;

            string myProp = string.Empty;
            string SubDetail_List = string.Empty;
            string Prop_SubView = string.Empty;

            myProp = ds_SubDetail.Namespace.ToString();
            SubDetail_List = t.MyProperties_Get(myProp, "About_Detail_SubDetail:");

            Prop_SubView = t.MyProperties_Get(myProp, "Prop_SubView:");
            //Prop_SubView = dN_SubDetail.AccessibleDescription;

            #endregion Tanımlar

            #region SubDetail_List
            if (t.IsNotNull(SubDetail_List))
            {
                //About_Detail_SubDetail:
                //=Detail_SubDetail:GRP.GRP_03||ID||[K].GRUP_ID||56||32||LKP_ONAY||1||2774|ds|
                //=Detail_SubDetail:SNVLIST_ADAY.SNVLIST_ADAY_01||||[K].E_SINAV||104||3||E_SINAV||||2868|ds|
                //=Detail_SubDetail:SNVLIST_ADAY.SNVLIST_ADAY_01||||[K].OGRDURUM||52||1||OGRDURUM||||2772|ds|

                int DataReadType = t.myInt32(t.MyProperties_Get(myProp, "DataReadType:"));

                string SqlF = t.MyProperties_Get(myProp, "SqlFirst:");
                string SqlS = t.MyProperties_Get(myProp, "SqlSecond:");

                string Sql_OldF = SqlF;
                string Sql_OldS = SqlS;

                //***
                if ((Sql_OldF != "") && (Sql_OldS == ""))
                {
                    SubDetail_Preparing(tForm, ref SqlF,
                                        null, null, //mst_TableIPCode, 
                                        ds_SubDetail, SubDetail_List, SubDetail_TableIPCode,
                                        DataReadType, FieldName, Value);
                }

                if (Sql_OldS != "")
                {
                    SubDetail_Preparing(tForm, ref SqlS,
                                        null, null, //mst_TableIPCode,
                                        ds_SubDetail, SubDetail_List, SubDetail_TableIPCode,
                                        DataReadType, FieldName, Value);
                }
                //***

                SubDetail_Run(tForm, ds_SubDetail, myProp, SqlF, Sql_OldF, SqlS, Sql_OldS, SubDetail_TableIPCode, DataReadType);
            }
            #endregion SubDetail_List

            #region Prop_SubView
            if (t.IsNotNull(Prop_SubView))
            {
                if (sender.ToString() == "DevExpress.XtraEditors.ImageComboBoxEdit")
                {
                    string Now_Value = ((DevExpress.XtraEditors.ImageComboBoxEdit)sender).EditValue.ToString();
                    SubView_Preparing(tForm, Prop_SubView, Now_Value);
                }
            }
            #endregion Prop_SubView

        }

        private void SubView_Preparing(Form tForm, string Prop_SubView, string Now_Value)
        {
            string s1 = "=ROW_PROP_SUBVIEW:";
            string s2 = (char)34 + "SV_LIST" + (char)34 + ": [";
            //"SV_LIST": [

            if (Prop_SubView.IndexOf(s1) > -1)
                SubView_Preparing_OLD(tForm, Prop_SubView, Now_Value);

            if (Prop_SubView.IndexOf(s2) > -1)
                SubView_Preparing_JSON(tForm, Prop_SubView, Now_Value);

        }

        private void SubView_Preparing_JSON(Form tForm, string Prop_SubView, string Now_Value)
        {
            tToolBox t = new tToolBox();

            PROP_SUBVIEW packet = new PROP_SUBVIEW();
            Prop_SubView = Prop_SubView.Replace((char)34, (char)39);
            var prop_ = JsonConvert.DeserializeAnonymousType(Prop_SubView, packet);

            string TableIPCode = string.Empty;
            string SM_PageName = string.Empty;

            if (prop_.SV_ENABLED.ToString() == "TRUE")
            {
                foreach (var item in prop_.SV_LIST)
                {
                    if (item.SV_VALUE == Now_Value)
                    {
                        TableIPCode = t.Set(item.TABLEIPCODE.ToString(), "", "");
                        // PageName var ise bir menunün (Ribbon un) page ismidir ve bu page show edilececek
                        SM_PageName = t.Set(item.SM_PAGENAME.ToString(), "", "");

                        if (t.IsNotNull(TableIPCode))
                        {
                            tSubView_Preparing(tForm, prop_.SV_VIEW_TYPE.ToString(), "", TableIPCode, "", "", SM_PageName);
                            break;
                        }
                    }
                }
            }
        }

        private void SubView_Preparing_OLD(Form tForm, string Prop_SubView, string Now_Value)
        {
            tToolBox t = new tToolBox();

            string s = string.Empty;
            string TableIPCode = string.Empty;
            string SM_PageName = string.Empty;

            string SubView1 = Prop_SubView;

            if (SubView1.IndexOf("=SV_ENABLED:TRUE;") > -1)
            {
                vSubView vSV = new vSubView();
                SubView_Get(vSV, Prop_SubView);

                #region TABLEIPCODE tespiti
                s = "=SV_VALUE:" + Now_Value + ";";

                if (SubView1.IndexOf(s) > 0)
                {
                    SubView1 = t.AfterGet_And_BeforeClear(ref SubView1, s, false);

                    TableIPCode = t.MyProperties_Get(SubView1, "=TABLEIPCODE:");

                    // PageName var ise bir menunün (Ribbon un) page ismidir ve bu page show edilececek
                    SM_PageName = t.MyProperties_Get(SubView1, "=SM_PAGENAME:");
                }
                #endregion TABLEIPCODE tespiti

                #region SubView Create
                if (t.IsNotNull(TableIPCode))
                {
                    tSubView_Preparing(tForm, vSV.SV_ViewType, "", TableIPCode, "", "", SM_PageName);
                }
                #endregion

            }
        }

        public void tXtraEdit_EditValueChanged(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            #region Tanımlar

            string TableIPCode = string.Empty;
            string FieldName = string.Empty;
            string Value = string.Empty;
            string Tag = string.Empty;

            Form tForm = t.Find_TableIPCode_XtraEditors(sender, ref TableIPCode, ref FieldName, ref Value, ref Tag);

            #endregion Tanımlar

            #region Expression
            if (Tag == "EXPRESSION")
            {
                // okunacak dataseti bul
                DataSet dsData = null;
                DataNavigator tDataNavigator = null;
                t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TableIPCode);

                //string myProp = dsData.Namespace.ToString();

                Preparing_Expression(tForm, dsData, tDataNavigator.Position, TableIPCode, FieldName, Value);

                // Eğer return olmaz ise alttaki fonksiyonlar çalışıyor
                // bu nedenle return kaldırma
                return;
            }
            #endregion Expression
        }

        public void tXtraEdit_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            //v.SQL = v.SQL + ",2";
            //tToolBox t = new tToolBox();
        }

        public void tXtraEdit_KeyDown(object sender, KeyEventArgs e)
        {
            v.con_Expression = true;
        }

        public void tXtraEdit_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.con_Expression = true;
        }

        public void tXtraEdit_KeyUp(object sender, KeyEventArgs e)
        {
            v.con_Expression = true;
        }

        public void myBarcodeEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (
                ((e.KeyCode == Keys.Decimal) ||
                 (e.KeyCode == Keys.Subtract) ||
                 (e.KeyCode == Keys.Divide) ||
                 (e.KeyCode == Keys.Add) ||
                 (e.KeyCode == Keys.Multiply) ||
                 (e.KeyCode == Keys.Enter)) &&
                 (((DevExpress.XtraEditors.TextEdit)sender).EditValue != null)
                )
            {
                int l1 = 0;
                string Veri = ((DevExpress.XtraEditors.TextEdit)sender).EditValue.ToString();

                if (e.KeyCode != Keys.Enter)
                    Veri = Veri.Substring(0, Veri.Length - 1);

                l1 = Veri.Length;

                // 6 karekterden düşük ise 99999
                if ((l1 > 0) && (l1 < 6))
                {
                    tToolBox t = new tToolBox();
                    Form tForm = t.Find_Form(sender);
                    string[] controls = new string[] { };
                    Control cntrl = t.Find_Control(tForm, "tEditMiktar", "", controls);

                    if (cntrl != null)
                    {
                        // editMiktar 
                        ((DevExpress.XtraEditors.TextEdit)cntrl).EditValue = Veri;

                        int fontSize = 20;
                        if (l1 > 3) fontSize = 14;
                        ((DevExpress.XtraEditors.TextEdit)cntrl).Font =
                            new Font(((DevExpress.XtraEditors.TextEdit)cntrl).Font.FontFamily, fontSize);

                        // editBarcode
                        ((DevExpress.XtraEditors.TextEdit)sender).EditValue = "";
                    }
                }


            }

        }


        #endregion tXtraEdit_ Events

        #region buttonEdit Events

        public void buttonEdit_Enter(object sender, EventArgs e)
        {
            //string function_name = "buttonEdit_Enter";
            //sp.Takipci(function_name, "", '{');
            tToolBox t = new tToolBox();

            string func_name = string.Empty;
            string myProp = "";

            //if (sender.ToString() == "DevExpress.XtraEditors.ButtonEdit")
            myProp = ((DevExpress.XtraEditors.ButtonEdit)sender).Properties.AccessibleDescription;

            func_name = t.MyProperties_Get(myProp, "Type:");

            if (func_name == v.SearchEngine)
            {
                v.con_Value_New = "";
                if (((DevExpress.XtraEditors.ButtonEdit)sender).EditValue != null)
                    v.con_Value_New = ((DevExpress.XtraEditors.ButtonEdit)sender).EditValue.ToString();
            }

            //sp.Takipci(function_name, "", '}');
        }

        public void buttonEdit_Leave(object sender, EventArgs e)
        {
            //string function_name = "buttonEdit_Leave";
            //sp.Takipci(function_name, "", '{');
            tToolBox t = new tToolBox();

            string func_name = string.Empty;
            string myProp = ((DevExpress.XtraEditors.ButtonEdit)sender).Properties.AccessibleDescription;

            func_name = t.MyProperties_Get(myProp, "Type:");

            if (func_name == v.SearchEngine)
            {
                if (((DevExpress.XtraEditors.ButtonEdit)sender).EditValue != null)
                    v.con_Value_Old = ((DevExpress.XtraEditors.ButtonEdit)sender).EditValue.ToString();

                if ((v.con_Value_New != v.con_Value_Old) &&
                    (v.con_Value_New != "-1") &&
                    (v.con_Value_New != ""))
                {
                    MessageBox.Show("Search için çalışma yapılmış.. Burayı gözden geçir (buttonEdit_Leave)");
                    //Search_Engines(sender, v.con_Value_Old, 2, "BEEP");
                }
            }
            //sp.Takipci(function_name, "", '}');
        }

        public void buttonEdit_KeyDown(object sender, KeyEventArgs e)
        {
            //string function_name = "buttonEdit_KeyDown";
            //sp.Takipci(function_name, "", '{');

            if (e.KeyCode == Keys.Return)
            {
                tToolBox t = new tToolBox();

                string func_name = string.Empty;
                string myProp = ((DevExpress.XtraEditors.ButtonEdit)sender).Properties.AccessibleDescription;

                func_name = t.MyProperties_Get(myProp, "Type:");

                if (func_name == v.SearchEngine)
                {
                    if (((DevExpress.XtraEditors.ButtonEdit)sender).EditValue != null)
                        v.con_Value_Old = ((DevExpress.XtraEditors.ButtonEdit)sender).EditValue.ToString();

                    if ((v.con_Value_New != v.con_Value_Old) && (v.con_Value_New != "-1"))
                    {
                        MessageBox.Show("Search için çalışma yapılmış.. Burayı gözden geçir (buttonEdit_KeyDown)");
                        //Search_Engines(sender, v.con_Value_Old, 2, "BEEP");
                    }
                }
            }
            //sp.Takipci(function_name, "", '}');
        }

        public void buttonEdit_KeyPress(object sender, KeyPressEventArgs e)
        {
            //MessageBox.Show("buttonEdit1_KeyPress");
        }

        public void buttonEdit_KeyUp(object sender, KeyEventArgs e)
        {
            //MessageBox.Show("buttonEdit1_KeyUp");
        }

        public void buttonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            tToolBox t = new tToolBox();
            
            string func_name = string.Empty;
            string TargetTableIPCode = string.Empty;
            string myProp = ((DevExpress.XtraEditors.ButtonEdit)sender).Properties.AccessibleDescription;
            
            if (t.IsNotNull(myProp))
            {

                #region Find Form
                Form tForm = null;

                if (sender.ToString() == "DevExpress.XtraEditors.ImageComboBoxEdit")
                {
                    TargetTableIPCode = ((DevExpress.XtraEditors.ImageComboBoxEdit)sender).Properties.AccessibleName;
                    tForm = ((DevExpress.XtraEditors.ImageComboBoxEdit)sender).FindForm();
                }

                if (sender.ToString() == "DevExpress.XtraEditors.ButtonEdit")
                {
                    TargetTableIPCode = ((DevExpress.XtraEditors.ButtonEdit)sender).Properties.AccessibleName;
                    tForm = ((DevExpress.XtraEditors.ButtonEdit)sender).FindForm();
                }

                if (sender.ToString() == "DevExpress.XtraEditors.SimpleButton")
                {
                    TargetTableIPCode = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;
                    tForm = ((DevExpress.XtraEditors.SimpleButton)sender).FindForm();
                }
                #endregion


                //Type: SearchEngine;[
                //   {
                //     "CAPTION": "Search",
                //     "BUTTONTYPE": "58",
                //     "TABLEIPCODE_LIST": [

                myProp = myProp.Replace((char)34, (char)39);

                func_name = t.MyProperties_Get(myProp, "Type:");
                                
                #region Search_Engines and Buttons
                if ((func_name == v.SearchEngine) || (func_name == v.ButtonEdit))
                {
                    if (sender.ToString() == "DevExpress.XtraEditors.ImageComboBoxEdit")
                    {
                        /// atama yapılacak değer GET_FIELD_LIST={ içinden tespit edilmeli ama hangisi ?
                        /// aslında gerekte yok, çünkü gelen değer  editvalue  
                        v.con_Value_Old = string.Empty;
                    }

                    if (sender.ToString() == "DevExpress.XtraEditors.ButtonEdit")
                    {
                        if (((DevExpress.XtraEditors.ButtonEdit)sender).EditValue != null)
                            v.con_Value_Old = ((DevExpress.XtraEditors.ButtonEdit)sender).EditValue.ToString();
                    }

                    //if (e.Button.Index == 0)
                    if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis)
                    {
                        // eğer value yoksa yeni kart aç
                        if (v.con_Value_Old == "")
                        {
                            run_Prop_Navigator(tForm, TargetTableIPCode, myProp, v.tNavigatorButton.nv_52_Yeni_Kart_FormIle);
                        }
                        // eğer value varsa kartı aç
                        if (v.con_Value_Old != "")
                        {
                            run_Prop_Navigator(tForm, TargetTableIPCode, myProp, v.tNavigatorButton.nv_51_Karti_Ac);
                        }
                    }

                    //if (e.Button.Index == 1)
                    if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
                    {
                        Run_Search(tForm, TargetTableIPCode, myProp);
                    }
                }
                #endregion Search_Engines

                #region Properties & Plus 
                if ((func_name == v.Properties) ||
                    (func_name == v.PropertiesPlus))
                {
                    string TableName = t.MyProperties_Get(myProp, "TableName:");
                    string FieldName = t.MyProperties_Get(myProp, "FieldName:");
                    string Width = t.MyProperties_Get(myProp, "Width:");

                    string thisValue = ((DevExpress.XtraEditors.ButtonEdit)sender).EditValue.ToString();

                    tCreateObject co = new tCreateObject();

                    string NewValue = thisValue;

                    string s1 = "=ROW_";// PROP_NAVIGATOR:";
                    //string s2 = (char)34 + "TABLEIPCODE_LIST" + (char)34 + ": [";

                    if (thisValue.IndexOf(s1) > -1)
                    {
                        if (func_name == v.Properties)
                            NewValue = co.Create_PropertiesEdit(TableName, FieldName, Width, thisValue);

                        if (func_name == v.PropertiesPlus)
                            NewValue = co.Create_PropertiesPlusEdit(TableName, FieldName, Width, thisValue);
                    }
                    else //if (thisValue.IndexOf(s2) > -1)
                    {
                        if (func_name == v.Properties)
                            NewValue = co.Create_PropertiesEdit_JSON(TableName, FieldName, Width, thisValue);

                        if (func_name == v.PropertiesPlus)
                            NewValue = co.Create_PropertiesPlusEdit_JSON(TableName, FieldName, Width, thisValue);
                    }

                    ((DevExpress.XtraEditors.ButtonEdit)sender).EditValue = NewValue;
                }
                #endregion Properties & Plus 

                #region PropertiesNav
                if (func_name == v.PropertiesNav)       
                {
                    //string TableName = t.MyProperties_Get(myProp, "TableName:");
                    //string FieldName = t.MyProperties_Get(myProp, "FieldName:");
                    //string Width = t.MyProperties_Get(myProp, "Width:");

                    string thisValue = ((DevExpress.XtraEditors.ButtonEdit)sender).EditValue.ToString();

                    tCreateObject co = new tCreateObject();

                    string NewValue = thisValue;

                    NewValue = co.Create_PropertiesNavEdit(thisValue);

                    ((DevExpress.XtraEditors.ButtonEdit)sender).EditValue = NewValue;
                }
                #endregion PropertiesNav 

            }
                        
        }
        
        //public void buttonEdit_ButtonClick_(Form tForm, string mst_TableIPCode, string Prop_Navigator, v.tNavigatorButton buttonType)
        public void run_Prop_Navigator(Form tForm, string mst_TableIPCode, string Prop_Navigator, v.tNavigatorButton buttonType)
        {
            tToolBox t = new tToolBox();

            bool form_open = false;
            
            List<PROP_NAVIGATOR> packet = new List<PROP_NAVIGATOR>();

            t.Str_Remove(ref Prop_Navigator, "Type:SearchEngine;");
            t.Str_Remove(ref Prop_Navigator, "Type:Buttons;");
            
            Prop_Navigator = Prop_Navigator.Replace((char)34, (char)39);
            int i = Prop_Navigator.IndexOf("[");
            if ((i == -1) || (i > 10))
                Prop_Navigator = "[" + Prop_Navigator + "]";
            var prop_ = JsonConvert.DeserializeAnonymousType(Prop_Navigator, packet);

            foreach (PROP_NAVIGATOR item in prop_)
            {

                if (buttonType.ToString().IndexOf(item.BUTTONTYPE.ToString()) > -1)
                {
                    form_open = CheckValue(tForm, item, mst_TableIPCode);

                    #region // form açılması için onaylandı ise
                    if (form_open)
                    {
                        if (buttonType == v.tNavigatorButton.nv_58_Search)
                        {
                            v.searchOnay = false;
                            v.searchOnay = Search_Engines(tForm, mst_TableIPCode, v.con_Value_Old, item);

                            if (v.searchOnay)
                            {
                                i = Prop_Navigator.IndexOf("'WORKTYPE': 'IBOX'");
                                if (i > -1)
                                    InputBox(tForm, mst_TableIPCode, item);
                            }
                        }

                        if ((buttonType == v.tNavigatorButton.nv_52_Yeni_Kart_FormIle) &&
                            (v.con_Value_Old == ""))
                        {
                            v.con_DataRow = null;

                            tNewForm_NewData_JSON(tForm, item);

                            /// New Value Set
                            ///
                            if (v.con_DataRow != null)
                                Search_Engine_SetValues_JSON(tForm, mst_TableIPCode, null, item.TABLEIPCODE_LIST);

                            v.con_Value_Old = "";
                            // foreach  
                            break;
                        }

                        if ((buttonType == v.tNavigatorButton.nv_51_Karti_Ac) &&
                            (v.con_Value_Old != ""))
                        {
                            v.con_DataRow = null;

                            t.OpenForm_JSON(tForm, item);

                            /// New Value Set
                            ///
                            if (v.con_DataRow != null)
                                Search_Engine_SetValues_JSON(tForm, mst_TableIPCode, null, item.TABLEIPCODE_LIST);

                            v.con_Value_Old = "";
                            // foreach  
                            break;
                        }

                        if (buttonType == v.tNavigatorButton.nv_55_Run_Expression)
                        {
                            Run_Expression(tForm, mst_TableIPCode);
                        }

                        if (buttonType == v.tNavigatorButton.nv_56_Run_TableIPCode)
                        {
                            Run_TableIPCode(tForm, mst_TableIPCode, item); 
                        }

                    }
                    #endregion
                }
            }
        }


        #region SubFunctions for Event

        public void ImageComboBoxEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
#region Button[1] click ise
            if (e.Button.Index == 1)
            {
                //MessageBox.Show("aaaaa");
                buttonEdit_ButtonClick(sender, e);
            }
#endregion  Button[1] click ise
        }

        public string ButtonEdit_AND(DataRow Row)
        {
            //  TLKP_TYPE
            //--101 = Proje Seçenekleri (Lkp)
            //--103 = Kullanıcı Seçenekleri (Lkp)
            //--105 = Data Seçenekleri (Lkp)

            //  FLKP_TYPE
            //--4 = Join for 1 Field (Kaynak Tablo)
            //--5 = Join for 2 Fields (Kaynak Tablo)
            //--6 = Join for Partner Fields

            string s = string.Empty;
            Int16 Lkp_Type = System.Convert.ToInt16(Row["LKP_FLKP_TYPE"].ToString());

            if (Row["LKP_PARTNER_FIELD_NAME"] != null)
                if (Row["LKP_PARTNER_FIELD_NAME"].ToString() != "")
                {
                    s = " and a.TABLE_CODE = '" + Row["LKP_TABLE_CODE"].ToString() + "' " + v.ENTER +
                        " and a.TABLE_ID_FNAME = '" + Row["LKP_PARTNER_FIELD_NAME"].ToString() + "' " + v.ENTER;
                }

            return s;
        }

        #endregion SubFunctions for Event

        #endregion buttonEdit Events

        #region tProperties Buttons Click  >>

        public void btn_PropertiesNav_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            #region Tanımlar
            // tüm bu olayların oluştuğu form tespit ediliyor
            Form tForm = t.Find_Form(sender);
            DataSet dsData = null;
            DataNavigator dN = null;

            string TableIPCode = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;
            string tbutton = ((DevExpress.XtraEditors.SimpleButton)sender).Text;

            #endregion Tanımlar

            #region Close
            if (tbutton == "&Close")
            {
                tForm.Close();
            }
            #endregion Close

            #region Update
            if (tbutton == "&Update")
            {
                t.Find_DataSet(tForm, ref dsData, ref dN, TableIPCode);

                if (t.IsNotNull(dsData))
                {
                    string onay = string.Empty;
                    string lkp_Caption = string.Empty;
                    string lkp_newCaption = string.Empty;
                    string lkp_newWidth = string.Empty;
                    string value = string.Empty;

                    int i2 = dsData.Tables[0].Rows.Count;
                    for (int i = 0; i < i2; i++)
                    {
                        onay = dsData.Tables[0].Rows[i]["LKP_ONAY"].ToString();
                        if (onay == "True")
                        {
                            lkp_Caption = dsData.Tables[0].Rows[i]["LKP_CAPTION"].ToString();
                            lkp_newCaption = dsData.Tables[0].Rows[i]["LKP_NEW_CAPTION"].ToString();
                            lkp_newWidth = dsData.Tables[0].Rows[i]["LKP_NEW_WIDTH"].ToString();

                            if (t.IsNotNull(lkp_newCaption) == false) lkp_newCaption = "null";
                            if (t.IsNotNull(lkp_newWidth) == false) lkp_newWidth = "0";

                            /// onaylı satırlar toparlanarak kaydedilmesi için 
                            /// çağrıldğı noktaya gönderilecek
                            /// 
                            value = value 
                                + "btn:"
                                + i.ToString() + "||" // index
                                + lkp_Caption + "||"
                                + lkp_newCaption + "||"
                                + lkp_newWidth + "|ds|" + v.ENTER;
                        }
                    }

                    tForm.AccessibleDefaultActionDescription = value;

                    tForm.Close();
                }
            }
            #endregion Update
                        
            #region Clear
            if (tbutton == "&Clear")
            {
                t.Find_DataSet(tForm, ref dsData, ref dN, TableIPCode);

                int i2 = dsData.Tables[0].Rows.Count;
                for (int i = 0; i < i2; i++)
                {
                    dsData.Tables[0].Rows[i]["LKP_ONAY"] = 0;
                }
            }
            #endregion Clear
        }
        
        public void btn_Properties_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();
       
            string prp_type = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;

            #region Tanımlar
            // tüm bu olayların oluştuğu form tespit ediliyor
            Form tForm = t.Find_Form(sender);

            string tbutton = ((DevExpress.XtraEditors.SimpleButton)sender).Text;

            // VgridControl tespit ediliyor
            string[] controls = new string[] { "DevExpress.XtraVerticalGrid.VGridControl" };
            Control cntrl = t.Find_Control(tForm, "", "tProperties", controls);

            #endregion Tanımlar

            #region Close
            if (tbutton == "&Close")
            {
                tPreperties_SingleBlock_Add(tForm, (DevExpress.XtraVerticalGrid.VGridControl)cntrl, prp_type);

                tForm.Close();
            }
            if (tbutton == "&Close Memo")
            {
                tForm.Close();
            }
            #endregion Close

            #region Update
            if (tbutton == "&Update")
            {
                tPreperties_SingleBlock_Add(tForm, (DevExpress.XtraVerticalGrid.VGridControl)cntrl, prp_type);
            }
            #endregion Update

            #region Update Memo (properties) / Memo Add (propertiesplus)
            if ((tbutton == "&Update Memo") ||
                (tbutton == "&Memo Add"))
            {
                string memoName = "";

                //if (tbutton == "&Update Memo") memoName = "memoEdit_PropertiesValue";// "MemoEdit1";
                //if (tbutton == "&Memo Add")
                    
                memoName = "memoEdit_PropertiesValue";

                Control memo = t.Find_Control(tForm, memoName);

                if (memo != null)
                {
                    string value = string.Empty;
                    if (((DevExpress.XtraEditors.MemoEdit)memo).EditValue != null)
                        value = ((DevExpress.XtraEditors.MemoEdit)memo).EditValue.ToString();

                    if (t.IsNotNull(value) == false)
                    {
                        string soru = "Veriler silinecek, Onaylıyor musunuz ?";
                        DialogResult cevap = t.mySoru(soru);
                        if (DialogResult.Yes == cevap)
                        {
                            tForm.AccessibleDefaultActionDescription = value;
                        }
                    }
                    else tForm.AccessibleDefaultActionDescription = value;
                }
            }
            #endregion Update

            #region Clear
            if (tbutton == "&Clear")
            {
                ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).FocusPrev();

                tProperties_Button_Clear((DevExpress.XtraVerticalGrid.VGridControl)cntrl);
            }
            #endregion Clear

            #region RowCollapse
            if (tbutton == "Kpt")
            {
                ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).CollapseAllRows();
            }
            #endregion RowCollapse

            #region RowExpand
            if (tbutton == "Aç")
            {
                ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).ExpandAllRows();
            }
            #endregion RowExpand

            #region Add (New Block Add)
            if (tbutton == "&Add")
            {
                tProperties_Button_Add(tForm, (DevExpress.XtraVerticalGrid.VGridControl)cntrl, prp_type);
            }
            #endregion Add

            #region Memo Add (New Memo Add)
            if (tbutton == "&Memo Add")
            {

            }
            #endregion Add

            #region Delete
            if (tbutton == "&Delete")
            {
                tProperties_Button_Delete(tForm, (DevExpress.XtraVerticalGrid.VGridControl)cntrl);
            }
            #endregion Delete

        }

        public void btn_Properties_JSON_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            string prp_type = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;

            #region Tanımlar
            // tüm bu olayların oluştuğu form tespit ediliyor
            Form tForm = t.Find_Form(sender);

            string tbutton = ((DevExpress.XtraEditors.SimpleButton)sender).Text;

            // VgridControl tespit ediliyor
            string[] controls = new string[] { "DevExpress.XtraVerticalGrid.VGridControl" };
            Control cntrl = t.Find_Control(tForm, "", "tProperties", controls);

            #endregion Tanımlar

            #region Close
            if (tbutton == "&Close")
            {
                tPreperties_SingleBlock_Add_JSON(tForm, (DevExpress.XtraVerticalGrid.VGridControl)cntrl);//, prp_type);

                tForm.Close();
            }
            if (tbutton == "&Close Memo")
            {
                tForm.Close();
            }
            #endregion Close

            #region Update
            if (tbutton == "&Update")
            {
                tPreperties_SingleBlock_Add_JSON(tForm, (DevExpress.XtraVerticalGrid.VGridControl)cntrl);//, prp_type);
            }
            #endregion Update

            #region Update Memo (properties) / Memo Add (propertiesplus)
            if ((tbutton == "&Update Memo") ||
                (tbutton == "&Memo Add"))
            {
                string memoName = "";

                //if (tbutton == "&Update Memo") memoName = "MemoEdit1";
                //if (tbutton == "&Memo Add") memoName = "memoEdit_PropertiesValue";
                memoName = "memoEdit_PropertiesValue";

                Control memo = t.Find_Control(tForm, memoName);

                if (memo != null)
                {
                    string value = string.Empty;
                    if (((DevExpress.XtraEditors.MemoEdit)memo).EditValue != null)
                        value = ((DevExpress.XtraEditors.MemoEdit)memo).EditValue.ToString();

                    if (t.IsNotNull(value) == false)
                    {
                        string soru = "Veriler silinecek, Onaylıyor musunuz ?";
                        DialogResult cevap = t.mySoru(soru);
                        if (DialogResult.Yes == cevap)
                        {
                            tForm.AccessibleDefaultActionDescription = value;
                        }
                    }
                    else tForm.AccessibleDefaultActionDescription = value;
                }
            }
            #endregion Update

            #region Clear
            if (tbutton == "&Clear")
            {
                ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).FocusPrev();

                tProperties_Button_Clear((DevExpress.XtraVerticalGrid.VGridControl)cntrl);
            }
            #endregion Clear

            #region RowCollapse
            if (tbutton == "Kpt")
            {
                ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).CollapseAllRows();
            }
            #endregion RowCollapse

            #region RowExpand
            if (tbutton == "Aç")
            {
                ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).ExpandAllRows();
            }
            #endregion RowExpand

            #region Add (New Block Add)
            if (tbutton == "&Add")
            {
                tProperties_Button_Add_JSON(tForm, (DevExpress.XtraVerticalGrid.VGridControl)cntrl);//, prp_type);
            }
            #endregion Add

            #region Memo Add (New Memo Add)
            if (tbutton == "&Memo Add")
            {

            }
            #endregion Add

            #region Delete
            if (tbutton == "&Delete")
            {
                tProperties_Button_Delete(tForm, (DevExpress.XtraVerticalGrid.VGridControl)cntrl);
            }
            #endregion Delete

        }


        #region Sub tProperties Functions

        private void tProperties_Button_Clear(VGridControl VGrid)
        {
            tToolBox t = new tToolBox();
            string function_name = "tProperties_Button_Clear";
            t.Takipci(function_name, "", '{');

            int t1 = VGrid.Rows.Count;
            int t2 = 0;

            for (int i1 = 0; i1 < t1; i1++)
            {
                // category altındaki edittext sayısı
                t2 = VGrid.Rows[i1].ChildRows.Count;

                for (int i2 = 0; i2 < t2; i2++)
                {
                    VGrid.Rows[i1].ChildRows[i2].Properties.Value = "";
                }
            }

            t.Takipci(function_name, "", '}');
        }

        private void tProperties_Button_Add(Form tForm, VGridControl VGrid, string prp_type)
        {
            tToolBox t = new tToolBox();
            string function_name = "tProperties Button_Add";
            t.Takipci(function_name, "", '{');

            if (VGrid != null)
            {
                // Mevcut VGridi olanı Blockla / Paketle
                tPreperties_SingleBlock_Add(tForm, VGrid, prp_type);

                // Yeni ID tespit edilecek
                string MainFName = VGrid.AccessibleDescription.ToString();
                string OldValue = tProperties_Memo_Value_Get(tForm);
                int NewId = tProperties_New_Block_Id(MainFName, OldValue);

                // Yeni ID tespit edildiyse 
                if (NewId > 0)
                {
                    // testpit edilen yeni ID yi ekrandaki textEdit e set et
                    tProperties_textEdit_Value_Set(tForm, NewId.ToString());

                    // VGridi önce temizle
                    tProperties_Button_Clear(VGrid);

                    tForm.Refresh();

                    // boş bir şekilde yeni blokla / pakele ve onuda FullBlocks üzerine set et
                    tPreperties_SingleBlock_Add(tForm, VGrid, prp_type);
                }
            }

            t.Takipci(function_name, "", '}');
        }

        private void tProperties_Button_Add_JSON(Form tForm, VGridControl VGrid)//, string prp_type)
        {
            tToolBox t = new tToolBox();

            if (VGrid != null)
            {
                // Mevcut VGridi olanı Blockla / Paketle
                tPreperties_SingleBlock_Add_JSON(tForm, VGrid);//, prp_type);

                string TableName = "";
                if (VGrid.Tag != null)
                    VGrid.Tag.ToString();
                string Main_FieldName = VGrid.AccessibleDescription.ToString();

                /// yeni boş bir blok oluşturmak için
                string thisValues = "";
                string TableIPCode = "tPropertiesPlusEdit";
                string paket_basi_Mevcut = "";

                string paket_turu = tPropertiesPacketValue_Preparing(TableName, Main_FieldName, 
                       ref thisValues, ref paket_basi_Mevcut, TableIPCode);

                tProperties_VGrid_Value_Set_JSON(VGrid, thisValues);
            }

        }
        
        private void tProperties_Button_Delete(Form tForm, VGridControl VGrid)
        {
            tToolBox t = new tToolBox();

            DialogResult answer = t.mySoru("sil");

            switch (answer)
            {
                case DialogResult.No:
                    {
                        string OldFullBlockValues = tProperties_Memo_Value_Get(tForm);
                        string remove_value = string.Empty;
                        if (VGrid.AccessibleDefaultActionDescription != null)
                            remove_value = VGrid.AccessibleDefaultActionDescription.ToString();
                        //string remove_value = v.con_Value_Old + v.ENTER;

                        t.Str_Remove(ref OldFullBlockValues, remove_value);

                        OldFullBlockValues = OldFullBlockValues + v.ENTER;

                        // kırpıntı kalırsa sıfırlasın 
                        if (OldFullBlockValues.Length < 20)
                        {
                            OldFullBlockValues = "";

                            // block Id yide sıfırla
                            tProperties_textEdit_Value_Set(tForm, "0");

                            // En son olarak VGridi temizle
                            VGrid.FocusPrev();
                            tProperties_Button_Clear(VGrid);
                        }

                        // Memoya kalanı set et
                        tProperties_Memo_Value_Set(tForm, OldFullBlockValues);
                        // listboxtan sil
                        tProperties_listBoxControl_Value_Remove(tForm, remove_value);
                        // VGrid Enabledi düzenle
                        tProperties_VGrid_Enabled(tForm, VGrid);

                        break; // break ifadesini sakın silme
                    }
            }

        }

        //-------------------

        public void tProperties_VGrid_Enabled(Form tForm, VGridControl VGrid)
        {
            tToolBox t = new tToolBox();

            //Form tForm = Application.OpenForms[""];

            string i = tProperties_textEdit_Value_Get(tForm);

            VGrid.Enabled = (t.myInt32(i) > 0);
        }
        
        //---JSON        
        private void tPreperties_SingleBlock_Add_JSON(Form tForm, VGridControl VGrid)//, string prp_type)
        {
            //tToolBox t = new tToolBox();
                        
            string OldBlock = string.Empty;
            string NewBlock = string.Empty;

            tProperties_VGrid_Values_Get_JSON(tForm, VGrid, ref OldBlock, ref NewBlock);

            // Paketlenen yeni block u mevcut FullBlocks ile add veya update et
                        
            tProperties_FullBlocks_Add_JSON(tForm, VGrid, OldBlock, NewBlock);            
        }

        private void tProperties_VGrid_Values_Get_JSON(Form tForm, VGridControl VGrid,
                     ref string OldValue, ref string NewValue)
        {
            string block = string.Empty;
            string fname = string.Empty;
            string value = string.Empty;
            
            int t1 = VGrid.Rows.Count;
            int t2 = 0;
            
            for (int i1 = 0; i1 < t1; i1++)
            {
                // category altındaki edittext sayısı
                t2 = VGrid.Rows[i1].ChildRows.Count;

                for (int i2 = 0; i2 < t2; i2++)
                {
                    fname = VGrid.Rows[i1].ChildRows[i2].Properties.FieldName.ToString();
                    value = VGrid.Rows[i1].ChildRows[i2].Properties.Value.ToString();
                    if (value == "") value = "null";

                    /// "animal": "dog",
                    /// şeklinde fieldName ve value birleştirmesi gerçekleştiriliyor
                    /// en başta iki adet boşluk var silme
                    /// en sonda bir adet virgül var 
                    if ((value.IndexOf("{") > -1) && (value.IndexOf("{") < 5)) 
                    {
                        block = block +
                            "  " + (char)34 + fname + (char)34 + ": " + value + "," + v.ENTER;
                    }
                    else if (value.IndexOf(": [") > -1)
                    {
                        block = block + value + "," + v.ENTER;
                    }
                    else 
                    {
                        block = block +
                            "  " + (char)34 + fname + (char)34 + ": " + (char)34 + value + (char)34 + "," + v.ENTER;
                    }
                    
                }
            }

            /// boşluklar alınıyor 
            /// block = block.Trim(); 
            /// ? trim en baştaki boşlukları siliyor, bunu silinmemesi gerekiyor 
            /// en sondaki boşluk ve ENTER işareti silinecek 
            block = block.TrimEnd();
            ///
            /// en sondaki virgül siliniyor
            ///
            block = block.Substring(0, block.Length - 1);

            /// artık min seviyedeki paket hazır
            /// 
            ///* örnek çıkış           
            ///
            ///{
            ///  "CAPTION": "A konusu",
            ///  "SV_VALUE": "null",
            ///  "TABLEIPCODE": "null"
            ///}
            NewValue = "{" + v.ENTER + block + v.ENTER + "}";
            
            /// "SV_ENABLED": "null",
            /// "SV_KEYFNAME": "null",
            /// "SV_CAPTION_FNAME": "null",
            /// "SV_CMP_TYPE": "null",
            /// "SV_CMP_LOCATION": "null",
            /// "SV_VIEW_TYPE": "null",
            /// "SV_LIST": []
                        
            if (VGrid.AccessibleDefaultActionDescription != null)
                OldValue = VGrid.AccessibleDefaultActionDescription.ToString();
            else OldValue = "";

            VGrid.AccessibleDefaultActionDescription = NewValue;
        }

        private void tProperties_FullBlocks_Add_JSON(Form tForm, VGridControl VGrid, string OldBlock, string NewBlock)
        {

            tToolBox t = new tToolBox();

            int itemId = 0;
            string s1 = string.Empty;
            string s2 = string.Empty;
            string s3 = string.Empty;
            bool list = false;

            string OldFullBlock = tProperties_Memo_Value_Get(tForm);
            Newtonsoft.Json.Linq.JArray myList = null;
            
            if ((t.IsNotNull(OldFullBlock)) &&
                (OldFullBlock.IndexOf("[") > -1) &&
                (OldFullBlock.IndexOf("[") < 5))
            {

                myList = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(OldFullBlock);

                s1 = myList.ToString();
                
                /// myList içinde [] işaretleri var ise array yapısı mevcut olduğunu gösterir
                /// Eğer bu işaretler var ise diğerlerinide aynı yapıya çevirmek gerkerir.
                ///
                
                OldBlock = "[" + v.ENTER + OldBlock + v.ENTER + "]";
                NewBlock = "[" + v.ENTER + NewBlock + v.ENTER + "]";

                list = true;
            }
            /// yoksa birde bunu kontrol et
            /// 
            if ((t.IsNotNull(OldFullBlock)) &&
                (OldFullBlock.IndexOf("{") > -1) &&
                (OldFullBlock.IndexOf("{") < 5))
            {
                //myList 
                var myL = JsonConvert.DeserializeObject(OldFullBlock);

                s1 = myL.ToString();
            }
            
            var myOldList = JsonConvert.DeserializeObject(OldBlock);
            var myNewList = JsonConvert.DeserializeObject(NewBlock);
            
            s2 = myOldList.ToString();
            s3 = myNewList.ToString();

            if ((list) && (s2.Length > 6) && (s3.Length > 6))
            {
                s2 = s2.Remove(0, 3);
                s2 = s2.Remove(s2.Length - 3, 3);
                s3 = s3.Remove(0, 3);
                s3 = s3.Remove(s3.Length - 3, 3);
            }
            
            ///if (OldFullBlock.IndexOf(OldBlock) > -1)
            if (s1.IndexOf(s2) > -1)
            {
                /// eğer tüm liste içinde değiştirilen liste var ise
                /// mevcut liste içinde bu eski item yeni item ile değiştirilecek

                //t.Str_Replace(ref OldFullBlock, OldBlock, NewBlock);
                t.Str_Replace(ref s1, s2, s3);

                OldFullBlock = s1;

                /*
                var aa = ((Newtonsoft.Json.Linq.JArray)myList)[0];
                var xx = aa.Children();
                foreach (var item in xx)
                {

                }
                */
            }
            else
            {
                /// eğer tüm liste içinde bu liste yok ise 
                /// mevcut listeye eklenecek
                /// 
                ///((Newtonsoft.Json.Linq.JArray)myList).Add(myNewList);
                var s4 = JsonConvert.DeserializeObject(s3);

                if (myList != null)
                {
                    ((Newtonsoft.Json.Linq.JArray)myList).Add(s4);
                    OldFullBlock = JsonConvert.SerializeObject(myList, Newtonsoft.Json.Formatting.Indented);
                }
                else
                {
                    OldFullBlock = JsonConvert.SerializeObject(s4, Newtonsoft.Json.Formatting.Indented);
                }
                
                itemId = -1;
            }
                        
            /// Memoya set et
            ///
            tProperties_Memo_Value_Set(tForm, OldFullBlock);

            /// Listeye Captionunu ekle
            tProperties_listBoxControl_Value_Set_JSON(tForm, NewBlock, itemId);

            /// 
            tForm.AccessibleDefaultActionDescription = OldFullBlock;
            
        }

        public void tProperties_listBoxControl_SelectedIndexChanged_JSON(object sender, EventArgs e)
        {
            if (((DevExpress.XtraEditors.ListBoxControl)sender).Tag != null)
                if (((DevExpress.XtraEditors.ListBoxControl)sender).Tag.ToString() == "1") return;

            string line_value = ((DevExpress.XtraEditors.ListBoxControl)sender).SelectedValue.ToString();
            string prp_type = ((DevExpress.XtraEditors.ListBoxControl)sender).AccessibleName;
            string MainFName = ((DevExpress.XtraEditors.ListBoxControl)sender).AccessibleDescription;

            tProperties_listBoxControlSelectedChanged_JSON(line_value, MainFName);
        }

        private void tProperties_listBoxControlSelectedChanged_JSON(string line_value, string MainFName)
        {
            tToolBox t = new tToolBox();
                        
            //Form tForm = Application.OpenForms["tForm_" + prp_type];
            Form tForm = Application.OpenForms["tForm_" + MainFName];

            if (tForm != null)
            {
                Control cntrl = tProperties_VGrid_Get(tForm);
                //string MainFName = ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).AccessibleDescription.ToString();
                string Block_Id = t.Get_And_Clear(ref line_value, ":");
                string FullBlockValue = tProperties_Memo_Value_Get(tForm);

                int Id = t.myInt32(Block_Id);

                string SelectBlockValue = Properties_SingleBlockValue_Get_JSON(FullBlockValue, Id);
                
                tProperties_textEdit_Value_Set(tForm, Id.ToString());

                if (cntrl != null)
                {
                    tProperties_VGrid_Value_Set_JSON(((DevExpress.XtraVerticalGrid.VGridControl)cntrl), SelectBlockValue);
                }
                
            }
        }

        private void tProperties_listBoxControl_Value_Set_JSON(Form tForm, string NewValue, int itemId)
        {
            tToolBox t = new tToolBox();

            string[] controls = new string[] { };
            Control cntrl = t.Find_Control(tForm, "listControl1", "", controls);
            

            if (cntrl != null)
            {
                Control vcntrl = tProperties_VGrid_Get(tForm);
                string MainFName = ((DevExpress.XtraVerticalGrid.VGridControl)vcntrl).AccessibleDescription.ToString();
                                
                string Caption = Properties_SingleBlockValue_Get_JSON(NewValue, -1);

                string value = string.Empty;
                int Ic = ((DevExpress.XtraEditors.ListBoxControl)cntrl).ItemCount;
                int Id = ((DevExpress.XtraEditors.ListBoxControl)cntrl).SelectedIndex;

                // listeyi guncelle
                if (itemId == 0)
                {
                    for (int i = 0; i < Ic; i++)
                    {
                        value = ((DevExpress.XtraEditors.ListBoxControl)cntrl).Items[i].ToString();

                        //value = ((DevExpress.XtraEditors.ListBoxControl)cntrl).SelectedValue.ToString();
                        if (value.IndexOf(Id.ToString()) > -1)
                        {
                            Caption = " " + i.ToString() + " : " + Caption;
                            ((DevExpress.XtraEditors.ListBoxControl)cntrl).Tag = 1;
                            ((DevExpress.XtraEditors.ListBoxControl)cntrl).Items.RemoveAt(i);
                            ((DevExpress.XtraEditors.ListBoxControl)cntrl).Items.Insert(i, Caption);
                            ((DevExpress.XtraEditors.ListBoxControl)cntrl).SelectedIndex = i; // yeniden seçsin
                            ((DevExpress.XtraEditors.ListBoxControl)cntrl).Tag = 0;
                            break;
                        }
                    }
                }

                if (itemId == -1)
                {
                    Caption = " " + Ic.ToString() + " : " + Caption;
                    ((DevExpress.XtraEditors.ListBoxControl)cntrl).Items.Add(Caption);

                    ((DevExpress.XtraEditors.ListBoxControl)cntrl).SelectedIndex =
                        ((DevExpress.XtraEditors.ListBoxControl)cntrl).Items.Count - 1;
                }
            }
        }
        
        private void tProperties_VGrid_Value_Set_JSON(VGridControl VGrid, string SelectBlockValue)
        {
            tToolBox t = new tToolBox();
            
            int t1 = VGrid.Rows.Count;
            int t2 = 0;
            string s = string.Empty;
            string fname = string.Empty;
            string tValue = string.Empty;

            VGrid.AccessibleDefaultActionDescription = SelectBlockValue;
            
            for (int i1 = 0; i1 < t1; i1++)
            {
                // category altındaki edittext sayısı
                t2 = VGrid.Rows[i1].ChildRows.Count;

                for (int i2 = 0; i2 < t2; i2++)
                {
                    fname = VGrid.Rows[i1].ChildRows[i2].Properties.FieldName.ToString();

                    tValue = t.Find_Properties_Value(SelectBlockValue, fname);
                                        
                    VGrid.Rows[i1].ChildRows[i2].Properties.Value = tValue;
                }
            }
        }

        public string tPropertiesPacketValue_Preparing(string TableName, string Main_FieldName,
            ref string thisValues, ref string packetHeader, string packetType)
        {
            tToolBox t = new tToolBox();

            string paket_turu = "";
            string paket_basi_A = (char)34 + Main_FieldName + (char)34 + ": ["; //"GRID": [
            string paket_basi_B = (char)34 + Main_FieldName + (char)34 + ": {"; //"GRID": {
            string paket_basi_C = (char)34 + Main_FieldName + (char)34 + ": ";  //"GRID": 

            string paket_sonu_A = "]";
            string paket_sonu_B = "}";

            if (t.IsNotNull(thisValues) == false)
            {
                /// boş bir yeni model yapı isteniyor
                thisValues = t.Create_PropertiesEdit_Model_JSON(TableName, Main_FieldName);

                if (packetType == "tPropertiesPlusEdit")
                {
                    thisValues = "[" + v.ENTER + thisValues + v.ENTER + "]";
                    paket_turu = "[]";
                }
            }
                        
            int j1 = thisValues.IndexOf(paket_basi_A);
            if (j1 > -1)
            {
                j1 = j1 + (paket_basi_A.Length - 1); // [ işareti silinmeden
                int j2 = thisValues.IndexOf(paket_sonu_A, j1) + paket_sonu_A.Length;
                thisValues = thisValues.Substring(j1, (j2 - j1)); // ] işareti silinmeden
                thisValues = thisValues.Trim();
                paket_turu = "[]";
                packetHeader = "A";
            }

            /// paket array ise  
            /// 
            if (thisValues == "[]")
            {
                /// boş bir yeni model yapı isteniyor
                thisValues = t.Create_PropertiesEdit_Model_JSON(TableName, Main_FieldName);
                /// bunları eklediğimiz zaman Array durumuna geçiyor
                thisValues = "[" + v.ENTER + thisValues + v.ENTER + "]";
                paket_turu = "[]";
                packetHeader = "A";
            }

            j1 = thisValues.IndexOf(paket_basi_B);
            if (j1 > -1)
            {
                j1 = j1 + (paket_basi_B.Length - 1); // { işareti silinmeden
                int j2 = thisValues.IndexOf(paket_sonu_B, j1) + paket_sonu_B.Length;
                thisValues = thisValues.Substring(j1, (j2 - j1)); // } işareti silinmeden
                thisValues = thisValues.Trim();
                paket_turu = "{}";
                packetHeader = "B";
            }

            

            /// packet object ise
            /// 
            ////if ((thisValues.IndexOf(paket_basi_B) > -1))
            ////{
            ////    /// boş bir yeni model yapı isteniyor
            ////    thisValues = t.Create_PropertiesEdit_Model_JSON(TableName, Main_FieldName);
            ////    paket_turu = "{}";
            ////    MessageBox.Show("if ((thisValues.IndexOf(paket_basi_B) > -1))");
            ////}
            
            if ((t.IsNotNull(thisValues)) && 
                (paket_turu == "") &&
                (packetType == "tPropertiesPlusEdit")
                )
            {
                 paket_turu = "[]";
            }

            if ((t.IsNotNull(thisValues)) &&
                (paket_turu == "") &&
                (packetType == "tPropertiesEdit")
                )
            {
                paket_turu = "{}";
            }
            return paket_turu;
        }


        //---JSON

        //--- OLD 

        private void tPreperties_SingleBlock_Add(Form tForm, VGridControl VGrid, string prp_type)
        {
            tToolBox t = new tToolBox();
            string function_name = "tPreperties SingleBlock Add";
            t.Takipci(function_name, "", '{');

            // PropertiesPlus ta olan textEdit üzerindeki valueyi okuyacak 
            string objno = tProperties_textEdit_Value_Get(tForm);

            tProperties_VGrid_Enabled(tForm, VGrid);

            if (objno == "0") return;

            //VGrid üzerindeki Veriyi Paketle / Blokla

            objno = t.myInt32(objno).ToString();

            string OldBlock = string.Empty;
            string NewBlock = string.Empty;

            tProperties_VGrid_Values_Get(tForm, VGrid, ref OldBlock, ref NewBlock, objno);

            // Paketlenen yeni block u mevcut FullBlocks ile add/update et

            tProperties_FullBlocks_Add(tForm, VGrid, t.myInt32(objno), OldBlock, NewBlock);

            t.Takipci(function_name, "", '}');

        }

        private void tProperties_VGrid_Values_Get(Form tForm, VGridControl VGrid,
                     ref string OldValue, ref string NewValue, string objno)
        {
            tToolBox t = new tToolBox();

            string block = string.Empty;
            string fname = string.Empty;
            string value = string.Empty;
            string paket_basi = string.Empty;
            //string paket_sonu = string.Empty;

            string MainFName = VGrid.AccessibleDescription.ToString();

            int t1 = VGrid.Rows.Count;
            int t2 = 0;

            t.MyProperties_Set(ref block, objno, "ROW_" + MainFName, objno);   //"ROW_FieldName:1");

            for (int i1 = 0; i1 < t1; i1++)
            {
                // category altındaki edittext sayısı
                t2 = VGrid.Rows[i1].ChildRows.Count;

                for (int i2 = 0; i2 < t2; i2++)
                {
                    fname = VGrid.Rows[i1].ChildRows[i2].Properties.FieldName.ToString();
                    value = VGrid.Rows[i1].ChildRows[i2].Properties.Value.ToString();

                    paket_basi = fname + "={";
                    //paket_sonu = fname + "=}";

                    if (value.IndexOf(paket_basi) > -1)
                    {
                        block = block +
                            objno + "=" + fname + ":" + value + ";" + v.ENTER;

                        /*
                        1=ROW_MainFName:1;
                        1=CAPTION:aaaaaa1222;
                        1=BUTTONTYPE:null;
                        1=TABLEIPCODE_LIST:TABLEIPCODE_LIST={
                        1=ROW:1;
                        1=CAPTION:qqq;
                        1=ROWE:1;
                        TABLEIPCODE_LIST=}
                        1=FORMNAME:null;
                        1=FORMCODE:null;
                        1=FORMTYPE:null;
                        1=FORMSTATE:null;
                        1=ROWE_MainFName:1;
                        */
                    }
                    else
                    {
                        t.MyProperties_Set(ref block, objno, fname, value);
                    }
                }
            }

            t.MyProperties_Set(ref block, objno, "ROWE_" + MainFName, objno);   //"ROWE_FieldName"

            NewValue = block.Trim();

            if (VGrid.AccessibleDefaultActionDescription != null)
                OldValue = VGrid.AccessibleDefaultActionDescription.ToString();
            else OldValue = "";

            VGrid.AccessibleDefaultActionDescription = NewValue;

            /* örnek            
            1=ROW:1;
            1=CAPTION:null;
            1=BUTTONTYPE:null;
            1=TABLEIPCODE_LIST:null;
            1=FORMNAME:null;
            1=FORMCODE:null;
            1=FORMTYPE:null;
            1=FORMSTATE:null;
            1=ROWE:1;
            */
        }

        private void tProperties_FullBlocks_Add(Form tForm, VGridControl VGrid, int Block_ID, string OldBlock, string NewBlock)
        {
            tToolBox t = new tToolBox();

            string OldFullBlockValues = tProperties_Memo_Value_Get(tForm);

            //=PROP_NAVIGATOR:{;
            string MainFName = VGrid.AccessibleDescription.ToString();
            string s = "=ROW_" + MainFName + ":" + Block_ID.ToString();

            if (OldFullBlockValues.IndexOf(s) > -1) // varsa
            {
                // Eskiyi block paketini iptal et yerine yeni paketle değiştir

                NewBlock = NewBlock.Trim();

                // ------------------------------------------------------------------------
                // DİKKAT : CAPTION ismi kullanırken 1234567890 gibi rakamlar kullan MA !!!
                // ------------------------------------------------------------------------

                t.Str_Replace(ref OldFullBlockValues, OldBlock, NewBlock);
            }
            else
            {
                // Mevcuda yeni block ekle
                if (OldFullBlockValues == "")
                    OldFullBlockValues = OldFullBlockValues.Trim() + NewBlock.Trim();
                else OldFullBlockValues = OldFullBlockValues.Trim() + v.ENTER + NewBlock.Trim();

            }

            // Memoya set et
            tProperties_Memo_Value_Set(tForm, OldFullBlockValues);

            // Listeye Captionunu ekle
            tProperties_listBoxControl_Value_Set(tForm, NewBlock);

            tForm.AccessibleDefaultActionDescription = OldFullBlockValues;

        }
        
        //---OLD

        

        private Control tProperties_VGrid_Get(Form tForm)
        {
            tToolBox t = new tToolBox();

            // properties VGrid tespit et
            string[] controls = new string[] { "DevExpress.XtraVerticalGrid.VGridControl" };
            Control cntrl = t.Find_Control(tForm, "", "tProperties", controls);

            return cntrl;
        }

        // old value vgridin üzerine set ediliyor
        private void tProperties_VGrid_Value_Set(VGridControl VGrid, string singleBlockValue)
        {
            //******************************
            //v.con_Value_Old = singleBlockValue;
            //******************************

            tToolBox t = new tToolBox();
            string function_name = "t_Properties_VGrid_Value_Set";
            t.Takipci(function_name, "", '{');

            int t1 = VGrid.Rows.Count;
            int t2 = 0;
            int j1, j2 = 0;
            string s = string.Empty;
            string fname = string.Empty;
            string tValue = string.Empty;

            string paket_basi = string.Empty;
            string paket_sonu = string.Empty;

            VGrid.AccessibleDefaultActionDescription = singleBlockValue;

            for (int i1 = 0; i1 < t1; i1++)
            {
                // category altındaki edittext sayısı
                t2 = VGrid.Rows[i1].ChildRows.Count;

                for (int i2 = 0; i2 < t2; i2++)
                {
                    s = singleBlockValue;

                    fname = VGrid.Rows[i1].ChildRows[i2].Properties.FieldName.ToString();

                    paket_basi = fname + "={" + v.ENTER;
                    paket_sonu = fname + "=}";

                    if (s.IndexOf(paket_basi) > -1)
                    {
                        j1 = s.IndexOf(paket_basi);
                        j2 = ((s.IndexOf(paket_sonu)) + paket_sonu.Length) - j1;
                        tValue = s.Substring(j1, j2);
                    }
                    else
                    {
                        tValue = t.MyProperties_Get(s, fname + ":");
                    }

                    tValue = tValue.Trim();

                    VGrid.Rows[i1].ChildRows[i2].Properties.Value = tValue;
                }
            }

            t.Takipci(function_name, "", '}');
        }

        private string tProperties_Memo_Value_Get(Form tForm)
        {
            tToolBox t = new tToolBox();

            string OldValue = string.Empty;
            string[] controls = new string[] { };
            Control cntrl = t.Find_Control(tForm, "memoEdit_PropertiesValue", "", controls);

            if (cntrl != null)
            {
                if (((DevExpress.XtraEditors.MemoEdit)cntrl).EditValue != null)
                {
                    OldValue = ((DevExpress.XtraEditors.MemoEdit)cntrl).EditValue.ToString();
                }
            }

            return OldValue;
        }

        private void tProperties_Memo_Value_Set(Form tForm, string NewValue)
        {
            tToolBox t = new tToolBox();

            string[] controls = new string[] { };
            Control cntrl = t.Find_Control(tForm, "memoEdit_PropertiesValue", "", controls);

            if (cntrl != null)
            {
                if (((DevExpress.XtraEditors.MemoEdit)cntrl).EditValue != null)
                {
                    ((DevExpress.XtraEditors.MemoEdit)cntrl).EditValue = NewValue;
                }
            }
        }

        private string tProperties_textEdit_Value_Get(Form tForm)
        {
            tToolBox t = new tToolBox();

            string OldValue = string.Empty;
            string[] controls = new string[] { };
            Control cntrl = t.Find_Control(tForm, "textEdit_ViewID", "", controls);

            if (cntrl != null)
            {
                if (((DevExpress.XtraEditors.TextEdit)cntrl).EditValue != null)
                {
                    OldValue = ((DevExpress.XtraEditors.TextEdit)cntrl).EditValue.ToString();
                }
            }

            return OldValue;
        }

        private void tProperties_textEdit_Value_Set(Form tForm, string NewValue)
        {
            tToolBox t = new tToolBox();

            string[] controls = new string[] { };
            Control cntrl = t.Find_Control(tForm, "textEdit_ViewID", "", controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.TextEdit)cntrl).EditValue = NewValue;
            }
        }

        private void tProperties_listBoxControl_Value_Set(Form tForm, string NewValue)
        {
            tToolBox t = new tToolBox();

            string[] controls = new string[] { };
            Control cntrl = t.Find_Control(tForm, "listControl1", "", controls);
            int Id = 0;

            if (cntrl != null)
            {
                Control vcntrl = tProperties_VGrid_Get(tForm);
                string MainFName = ((DevExpress.XtraVerticalGrid.VGridControl)vcntrl).AccessibleDescription.ToString();

                // SingleBlock son hali
                string Caption = Properties_for_List_Caption(MainFName, NewValue, ref Id);

                bool onay = false;
                string value = string.Empty;
                int j = ((DevExpress.XtraEditors.ListBoxControl)cntrl).ItemCount;

                for (int i = 0; i < j; i++)
                {
                    value = ((DevExpress.XtraEditors.ListBoxControl)cntrl).Items[i].ToString();
                    //SelectedIndex = i;

                    //value = ((DevExpress.XtraEditors.ListBoxControl)cntrl).SelectedValue.ToString();
                    if (value.IndexOf(Id.ToString()) > -1)
                    {
                        ((DevExpress.XtraEditors.ListBoxControl)cntrl).Tag = 1;
                        ((DevExpress.XtraEditors.ListBoxControl)cntrl).Items.RemoveAt(i);
                        ((DevExpress.XtraEditors.ListBoxControl)cntrl).Items.Insert(i, Caption);
                        ((DevExpress.XtraEditors.ListBoxControl)cntrl).SelectedIndex = i; // yeniden seçsin
                        ((DevExpress.XtraEditors.ListBoxControl)cntrl).Tag = 0;
                        onay = true;
                        break;
                    }
                }

                if (onay == false)
                {
                    ((DevExpress.XtraEditors.ListBoxControl)cntrl).Items.Add(Caption);

                    ((DevExpress.XtraEditors.ListBoxControl)cntrl).SelectedIndex =
                        ((DevExpress.XtraEditors.ListBoxControl)cntrl).Items.Count - 1;
                }
            }
        }

        private void tProperties_listBoxControl_Value_Remove(Form tForm, string OldValue)
        {
            tToolBox t = new tToolBox();

            string[] controls = new string[] { };
            Control cntrl = t.Find_Control(tForm, "listControl1", "", controls);
            int Id = 0;

            if (cntrl != null)
            {
                Control vcntrl = tProperties_VGrid_Get(tForm);
                string MainFName = ((DevExpress.XtraVerticalGrid.VGridControl)vcntrl).AccessibleDescription.ToString();

                // SingleBlock son hali
                string Caption = Properties_for_List_Caption(MainFName, OldValue, ref Id);

                string value = string.Empty;
                int j = ((DevExpress.XtraEditors.ListBoxControl)cntrl).ItemCount;

                for (int i = 0; i < j; i++)
                {
                    ((DevExpress.XtraEditors.ListBoxControl)cntrl).SelectedIndex = i;

                    value = ((DevExpress.XtraEditors.ListBoxControl)cntrl).SelectedValue.ToString();

                    if (value.IndexOf(Id.ToString()) > -1)
                    {
                        ((DevExpress.XtraEditors.ListBoxControl)cntrl).Tag = 1;
                        ((DevExpress.XtraEditors.ListBoxControl)cntrl).Items.RemoveAt(i);
                        ((DevExpress.XtraEditors.ListBoxControl)cntrl).Tag = 0;

                        if (((DevExpress.XtraEditors.ListBoxControl)cntrl).ItemCount > 0)
                        {
                            tProperties_listBoxControlSelectedChanged(
                                ((DevExpress.XtraEditors.ListBoxControl)cntrl).SelectedValue.ToString(),
                                ((DevExpress.XtraEditors.ListBoxControl)cntrl).AccessibleName.ToString());
                        }

                        //((DevExpress.XtraEditors.ListBoxControl)cntrl).SelectedIndex = 0; // yeniden seçsin

                        break;
                    }
                }
            }
        }

        public void tProperties_listBoxControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((DevExpress.XtraEditors.ListBoxControl)sender).Tag != null)
                if (((DevExpress.XtraEditors.ListBoxControl)sender).Tag.ToString() == "1") return;

            string line_value = ((DevExpress.XtraEditors.ListBoxControl)sender).SelectedValue.ToString();
            string prp_type = ((DevExpress.XtraEditors.ListBoxControl)sender).AccessibleName;
            string MainFName = ((DevExpress.XtraEditors.ListBoxControl)sender).AccessibleDescription;

            tProperties_listBoxControlSelectedChanged(line_value, MainFName);
        }

        private void tProperties_listBoxControlSelectedChanged(string line_value, string MainFName)
        {
            tToolBox t = new tToolBox();

            //t.Find_Form(sender); listboxtan bulamadı

            //Form tForm = Application.OpenForms["tForm_" + prp_type];
            Form tForm = Application.OpenForms["tForm_" + MainFName];

            if (tForm != null)
            {
                Control cntrl = tProperties_VGrid_Get(tForm);
                //string MainFName = ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).AccessibleDescription.ToString();
                string Block_Id = t.Get_And_Clear(ref line_value, ":");
                string FullBlockValue = tProperties_Memo_Value_Get(tForm);

                int Id = t.myInt32(Block_Id);

                string BlockValue = Properties_SingleBlockValue_Get(MainFName, FullBlockValue, Id);

                tProperties_textEdit_Value_Set(tForm, Id.ToString());

                if (cntrl != null)
                {
                    tProperties_VGrid_Value_Set(((DevExpress.XtraVerticalGrid.VGridControl)cntrl), BlockValue);
                }
            }
        }

        private int tProperties_New_Block_Id(string MainFName, string OldValue)
        {
            tToolBox t = new tToolBox();
            int j = 0;

            v.con_Value_Max = 0;
            string block = string.Empty;
            string bname = string.Empty;

            while (OldValue.IndexOf("ROWE_" + MainFName) > -1)
            {
                block = t.Get_And_Clear(ref OldValue, "ROWE_" + MainFName);

                bname = t.MyProperties_Get(block, "ROW_" + MainFName + ":");

                j = t.myMaxValue(bname);
            }
            // j ile en büyük değer geliyor ve 1 arttırılıyor
            j++;
            return j;
        }

        #region PropertiesPlus SubFunctions

        public string Properties_SingleBlockValue_Get_JSON(string FullBlocks, int BlockID)
        {
            // old - bir şeklide eski yapı gelirse 
            if (FullBlocks.IndexOf("={") > -1)
            {
                return "";
            }

            /// change " çift tırnağı  ' tek tırnak ile değiştir
            ///
            FullBlocks = FullBlocks.Replace((char)34, (char)39);

            var myList = JsonConvert.DeserializeObject(FullBlocks);
                        
            int i = 0;
            string value = string.Empty;

            // Type = Object  or  Type = Array
            var x = myList.GetType();
            if (x.Name == "JArray")
            {
                foreach (var item in (Newtonsoft.Json.Linq.JArray)myList)
                {
                    if (i == BlockID)
                    {
                        value = value + (Object)item.ToString();
                        break;
                    }

                    /// (BlockID == -1) ise
                    /// ilk nesnenin valuesi alınacak
                    /// bu genelde CAPTION value oluyor
                    if (BlockID == -1)
                    {
                        foreach (var item2 in (Newtonsoft.Json.Linq.JObject)item)
                        {
                            value = item2.Value.ToString();
                            break;
                        }
                    }

                    i++;
                }
            }

            if (x.Name == "JObject")
            {
                value = ((Newtonsoft.Json.Linq.JObject)myList).ToString();
                ///value = "[" + v.ENTER + value + v.ENTER + "]";
                ///

                /// ilk nesnenin valuesi alınacak
                /// bu genelde CAPTION value oluyor
                if (BlockID == -1)
                {
                    foreach (var item in (Newtonsoft.Json.Linq.JObject)myList)
                    {
                        value = item.Value.ToString();
                        break;
                    }
                }
            }

            return value;
        }

        public string Properties_SingleBlockValue_Get(string MainFName, string FullBlocks, int BlockID)
        {
            tToolBox t = new tToolBox();

            string s = FullBlocks;
            string s2 = string.Empty;
            string s3 = string.Empty;
            string block = string.Empty;
            bool onay = false;

            while (s.IndexOf("=ROWE_" + MainFName + ":") > -1)
            {
                block = t.BeforeGet_And_AfterClear(ref s, "=ROWE_" + MainFName + ":", true);

                if (block.IndexOf("ROW_" + MainFName + ":" + BlockID.ToString()) > -1)
                {
                    s2 = BlockID.ToString() + "=ROW_" + MainFName + ":" + BlockID.ToString();

                    s3 = t.AfterGet_And_BeforeClear(ref block, s2, false);

                    block = s3 + BlockID.ToString() + ";";
                    onay = true;
                    break;
                }
            }

            if (onay == false)
            {
                // eğer bulamaz ise ilk bloğu gönder
                s = FullBlocks;
                block = t.BeforeGet_And_AfterClear(ref s, "=ROWE_" + MainFName + ":", true);
            }

            return block = block.Trim();
        }

        public string Properties_for_List_Caption(string MainFName, string Single_Block_Value, ref int Id)
        {
            tToolBox t = new tToolBox();
            string bname = t.MyProperties_Get(Single_Block_Value, "ROW_" + MainFName + ":");
            string about = t.MyProperties_Get(Single_Block_Value, "CAPTION:");
            string capiton = "  " + bname.PadRight(3) + ": " + about.PadRight(50);
            Id = t.myInt32(bname);
            return capiton;
        }

        // Maximum Id
        public int Properies_Max_Block_Id(string MainFName, string OldBlockValues)
        {
            tToolBox t = new tToolBox();

            string block = string.Empty;
            string bname = string.Empty;
            int Id = 0;

            while (OldBlockValues.IndexOf("=ROWE_" + MainFName + ":") > -1)
            {
                block = t.Get_And_Clear(ref OldBlockValues, "=ROWE_" + MainFName + ":");
                bname = t.MyProperties_Get(block, "ROW_" + MainFName + ":");
                Id = t.myMaxValue(bname);
            }

            return Id++;
        }

        // Minumum Id
        public int Properies_Min_Block_Id(string MainFName, string OldBlockValues)
        {
            tToolBox t = new tToolBox();

            string block = string.Empty;
            string bname = string.Empty;
            int Id = 0;

            while (OldBlockValues.IndexOf("=ROWE_" + MainFName + ":") > -1)
            {
                block = t.Get_And_Clear(ref OldBlockValues, "=ROWE_" + MainFName + ":");
                bname = t.MyProperties_Get(block, "ROW_" + MainFName + ":");
                Id = t.myMinValue(bname);
            }

            //if (Id == 0) Id++;

            return Id;
        }

        #endregion PropertiesPlus SubFunctions

        #endregion Sub tProperties Functions

        //------------------

        #endregion tProperties Buttons Click << 

        #region vGridView Kriter Key Functions  >>

        public void myVGridControl_Kriter_KeyDown(object sender, KeyEventArgs e)
        {
            string myObject = sender.ToString();

            object mySender = null;

            if (e.KeyCode == Keys.Return)
            {
                if (myObject != "DevExpress.XtraVerticalGrid.VGridControl")
                {

#region Objeects
                    if (myObject == "DevExpress.XtraEditors.ButtonEdit")
                        mySender = ((DevExpress.XtraEditors.ButtonEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.CalcEdit")
                        mySender = ((DevExpress.XtraEditors.CalcEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.CheckButton")
                        mySender = ((DevExpress.XtraEditors.CheckButton)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.CheckedComboBoxEdit")
                        mySender = ((DevExpress.XtraEditors.CheckedComboBoxEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.CheckEdit")
                        mySender = ((DevExpress.XtraEditors.CheckEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.CheckedListBoxControl")
                        mySender = ((DevExpress.XtraEditors.CheckedListBoxControl)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.ComboBoxEdit")
                        mySender = ((DevExpress.XtraEditors.ComboBoxEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.DateEdit")
                        mySender = ((DevExpress.XtraEditors.DateEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.HyperLinkEdit")
                        mySender = ((DevExpress.XtraEditors.HyperLinkEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.ImageComboBoxEdit")
                        mySender = ((DevExpress.XtraEditors.ImageComboBoxEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.ImageEdit")
                        mySender = ((DevExpress.XtraEditors.ImageEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.ImageListBoxControl")
                        mySender = ((DevExpress.XtraEditors.ImageListBoxControl)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.LabelControl")
                        mySender = ((DevExpress.XtraEditors.LabelControl)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.ListBoxControl")
                        mySender = ((DevExpress.XtraEditors.ListBoxControl)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.LookUpEdit")
                        mySender = ((DevExpress.XtraEditors.LookUpEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.MemoEdit")
                        mySender = ((DevExpress.XtraEditors.MemoEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.MemoExEdit")
                        mySender = ((DevExpress.XtraEditors.MemoExEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.MRUEdit")
                        mySender = ((DevExpress.XtraEditors.MRUEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.PictureEdit")
                        mySender = ((DevExpress.XtraEditors.PictureEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.PopupContainerControl")
                        mySender = ((DevExpress.XtraEditors.PopupContainerControl)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.PopupContainerEdit")
                        mySender = ((DevExpress.XtraEditors.PopupContainerEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.RangeTrackBarControl")
                        mySender = ((DevExpress.XtraEditors.RangeTrackBarControl)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.SpinEdit")
                        mySender = ((DevExpress.XtraEditors.SpinEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.TextEdit")
                        mySender = ((DevExpress.XtraEditors.TextEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.TimeEdit")
                        mySender = ((DevExpress.XtraEditors.TimeEdit)sender).Parent;

                    if (myObject == "DevExpress.XtraEditors.TrackBarControl")
                        mySender = ((DevExpress.XtraEditors.TrackBarControl)sender).Parent;

                    //if (myObject == "DevExpress.XtraEditors.")
                    //    mySender = ((DevExpress.XtraEditors.)sender).Parent;

                    //DevExpress.XtraEditors.ButtonEdit
                    //DevExpress.XtraEditors.CalcEdit
                    //DevExpress.XtraEditors.CheckButton
                    //DevExpress.XtraEditors.CheckedComboBoxEdit
                    //DevExpress.XtraEditors.CheckEdit
                    //DevExpress.XtraEditors.CheckedListBoxControl
                    //DevExpress.XtraEditors.ComboBoxEdit
                    //DevExpress.XtraEditors.DateEdit
                    //DevExpress.XtraEditors.HyperLinkEdit
                    //DevExpress.XtraEditors.ImageComboBoxEdit
                    //DevExpress.XtraEditors.ImageEdit
                    //DevExpress.XtraEditors.ImageListBoxControl
                    //DevExpress.XtraEditors.LabelControl
                    //DevExpress.XtraEditors.ListBoxControl
                    //DevExpress.XtraEditors.LookUpEdit
                    //DevExpress.XtraEditors.MemoEdit
                    //DevExpress.XtraEditors.MemoExEdit
                    //DevExpress.XtraEditors.MRUEdit
                    //DevExpress.XtraEditors.PictureEdit
                    //DevExpress.XtraEditors.PopupContainerControl
                    //DevExpress.XtraEditors.PopupContainerEdit
                    //DevExpress.XtraEditors.RangeTrackBarControl
                    //DevExpress.XtraEditors.SpinEdit
                    //DevExpress.XtraEditors.TextEdit
                    //DevExpress.XtraEditors.TimeEdit
                    //DevExpress.XtraEditors.TrackBarControl
#endregion Objeects
                }
            }

            if (mySender != null)
            {
                if (mySender.ToString() == "DevExpress.XtraVerticalGrid.VGridControl")
                {
                    if (e.KeyCode == Keys.Return)
                    {
                        Form tForm = ((DevExpress.XtraVerticalGrid.VGridControl)mySender).FindForm();

                        string tTableIPCode = string.Empty;

                        // tGridControl.AccessibleName = "KRITER_" + TableIPCode;
                        int j = ((DevExpress.XtraVerticalGrid.VGridControl)mySender).AccessibleName.IndexOf("KRITER_");
                        if (j > -1)
                        {
                            j = j + 7; // "KRITER_" length
                            tTableIPCode = ((DevExpress.XtraVerticalGrid.VGridControl)mySender).AccessibleName.ToString();
                            tTableIPCode = tTableIPCode.Substring(j, tTableIPCode.Length - j);
                        }
                        else
                        {
                            tTableIPCode = ((DevExpress.XtraVerticalGrid.VGridControl)mySender).AccessibleName.ToString();
                        }

                        tToolBox t = new tToolBox();

                        string[] controls = new string[] { "DevExpress.XtraEditors.SimpleButton" };
                        Control c = t.Find_Control(tForm, "simpleButton_Kriter_Listele", tTableIPCode, controls);
                        if (c != null)
                            btn_KriterListele_Click(((DevExpress.XtraEditors.SimpleButton)c), EventArgs.Empty);
                        //((DevExpress.XtraEditors.SimpleButton)c).PerformClick();
                    }
                }
            }
        }

        public void myVGridControl_Kriter_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        public void myVGridControl_Kriter_KeyUp(object sender, KeyEventArgs e)
        {

        }

        #endregion vGridView Kriter Key Functions <<

        #region Prop_RunTime
                
        public void Prop_RunTimeClick(Form tForm, DataSet dsData, string TableIPCode, byte Button_Type)
        {
            tToolBox t = new tToolBox();

            Control cntrl = null;
            cntrl = t.Find_Control_View(tForm, TableIPCode);

            if (cntrl != null)
            {
                if (cntrl.AccessibleDescription != null)
                {
                    string Prop_RunTime = cntrl.AccessibleDescription.ToString();

                    string s1 = "=ROW_PROP_RUNTIME:";
                    string s2 = (char)34 + "AUTO_LST" + (char)34 + ": [";
                    /// "AUTO_LST": [

                    if (Prop_RunTime.IndexOf(s1) > -1)
                    {
                        if (Button_Type == v.nv_102_AUTO_INS)
                            Prop_RunTime_Work_AUTO_INS(tForm, dsData, Prop_RunTime);
                        else Prop_RunTime_Work(tForm, Prop_RunTime, Button_Type);
                    }

                    if (Prop_RunTime.IndexOf(s2) > -1)
                    {
                        if (Button_Type == v.nv_102_AUTO_INS)
                            Prop_RunTime_Work_AUTO_INS_JSON(tForm, dsData, Prop_RunTime);
                        else Prop_RunTime_Work_JSON(tForm, Prop_RunTime, Button_Type);
                    }
                }
            }
        }

        public void Prop_RunTime_Work(Form tForm, string Prop_RunTime, byte Button_Type)
        {

            #region AUTO_LST

            tToolBox t = new tToolBox();
            
            if (t.IsData(ref Prop_RunTime, "AUTO_LST"))
            {

                /* MS_PROPERTIES
                FIELDNAME                 ROW_CAPTION             ROW_FIELDNAME          ROW_COLUMN_TYPE
                ------------------------ ----------------------- --------------------- -----------------------
                AUTO_LST                  Button Type             BUTTONTYPE             ImageComboBoxEdit
                AUTO_LST                  TableIPCode List        TABLEIPCODE_LIST2      tPropertiesPlusEdit
                TABLEIPCODE_LIST2         Target TableIPCode      TABLEIPCODE            NULL
                */

                // AUTO_LST ve TABLEIPCODE_LIST2 birer block
                // BUTTONTYPE ve TABLEIPCODE ise birer field

                // block ve row tanımları
                string fblock1 = string.Empty;
                string fname1 = "AUTO_LST";
                string lockE1 = "=ROWE_" + fname1;
                string rblock1 = string.Empty;

                string fblock2 = string.Empty;
                string fname2 = "TABLEIPCODE_LIST2";
                string lockE2 = "=ROWE_" + fname2;
                string rblock2 = string.Empty;

                // içteki field tanımları
                string BUTTONTYPE = string.Empty;
                string TABLEIPCODE = string.Empty;

                // önce tüm bloğu al  (AUTO_LST)
                //fblock1 = t.Find_Properies_Get_FieldBlock(Prop_RunTime, fname1);
                fblock1 = Prop_RunTime;

                while (fblock1.IndexOf(lockE1) > -1)
                {
                    // tüm bloğu row larını ayır  (AUTO_LST yi rowlara ayır)
                    rblock1 = t.Find_Properies_Get_RowBlock(ref fblock1, fname1);

                    BUTTONTYPE = t.MyProperties_Get(rblock1, "BUTTONTYPE:");

                    // v.nv_24_Kaydet 
                    // v.nv_26_Sil_Satir
                    if (BUTTONTYPE == Button_Type.ToString())
                    {
                        fblock2 = t.Find_Properies_Get_FieldBlock(rblock1, "TABLEIPCODE_LIST2");

                        // TABLEIPCODE_LIST2 bloğa başla
                        while (fblock2.IndexOf(lockE2) > -1)
                        {
                            // ikinci bloğun row larını ayır (TABLEIPCODE_LIST2)
                            rblock2 = t.Find_Properies_Get_RowBlock(ref fblock2, fname2);

                            TABLEIPCODE = t.MyProperties_Get(rblock2, "TABLEIPCODE:");

                            if (t.IsNotNull(TABLEIPCODE))
                            {
                                DataSet ds = t.Find_DataSet(tForm, "", TABLEIPCODE, "");
                                if (ds != null)
                                {
                                    t.TableRefresh(tForm, ds, TABLEIPCODE);
                                }
                                //MessageBox.Show(TABLEIPCODE);
                            }

                        }

                    } // if (buttontype)
                }


                #region örnek
                /*                
                PROP_RUNTIME={
                0=ROW_PROP_RUNTIME:0;
                0=DRAGDROP:null;
                0=PRL_KRT:null;
                0=AUTO_KRT:null;
                0=AUTO_LST:AUTO_LST={
                1=ROW_AUTO_LST:1;
                1=CAPTION:Kaydet Butonu;
                1=BUTTONTYPE:24;
                1=TABLEIPCODE_LIST2:TABLEIPCODE_LIST2={
                1=ROW_TABLEIPCODE_LIST2:1;
                1=CAPTION:Teorik Sınav Notları;
                1=TABLEIPCODE:SNVLIST.SNVLIST_06;
                1=ROWE_TABLEIPCODE_LIST2:1;
                TABLEIPCODE_LIST2=};
                1=ROWE_AUTO_LST:1;
                AUTO_LST=};
                0=ROWE_PROP_RUNTIME:0;
                PROP_RUNTIME=}
                */
                #endregion örnek
            }

            #endregion AUTO_LST

        }

        public void Prop_RunTime_Work_JSON(Form tForm, string Prop_RunTime, byte Button_Type)
        {
            tToolBox t = new tToolBox();

            string RunTime = string.Empty;
            string Navigator = string.Empty;
            t.String_Parcala(Prop_RunTime, ref RunTime, ref Navigator, "|ds|");

            PROP_RUNTIME packet = new PROP_RUNTIME();
            RunTime = RunTime.Replace((char)34, (char)39);
            var prop_ = JsonConvert.DeserializeAnonymousType(RunTime, packet);

            string TABLEIPCODE = string.Empty;

            #region AUTO_LST

            foreach (var item in prop_.AUTO_LST)
            {
                /// v.nv_24_Kaydet 
                /// v.nv_26_Sil_Satir
                if (item.BUTTONTYPE.ToString() == Button_Type.ToString())
                {
                    foreach (var item2 in item.TABLEIPCODE_LIST2)
                    {
                        TABLEIPCODE = item2.TABLEIPCODE.ToString();

                        if (t.IsNotNull(TABLEIPCODE))
                        {
                            DataSet ds = t.Find_DataSet(tForm, "", TABLEIPCODE, "");
                            if (ds != null)
                            {
                                t.TableRefresh(tForm, ds, TABLEIPCODE);
                            }
                        }
                    }
                }
            }
            #endregion AUTO_LST

        }

        public void Prop_RunTime_Work_AUTO_INS(Form tForm, DataSet dsData, string Prop_RunTime)
        {
            tToolBox t = new tToolBox();

            // Kendisinden önce kayıt oluşturması gerek bir IP var ise o çalışacak
            // ??= Yani fiş satırının ilk kaydı oluşacaksa kendinden önce fiş başlığının 
            //     kaydı oluşması gerekir. okey

            string Header_TableIPCode = string.Empty;

            Header_TableIPCode = t.MyProperties_Get(Prop_RunTime, "AUTO_INS_MST_IP:");

            if (t.IsNotNull(Header_TableIPCode))
            {
                if (dsData != null)
                {
                    if (dsData.Tables[0].Rows.Count == 1)
                    {
                        string myProp = dsData.Namespace;
                        string KeyFName = t.MyProperties_Get(myProp, "KeyFName:");

                        if (dsData.Tables[0].Rows[0][KeyFName].ToString() == "")
                        {

                            DataSet dsDataHeader = t.Find_DataSet(tForm, "", Header_TableIPCode, "");

                            if (dsDataHeader != null)
                            {
                                if (dsDataHeader.Tables[0].Rows.Count == 0)
                                {
                                    tNewData(tForm, Header_TableIPCode);
                                }

                                tSave sv = new tSave();
                                sv.tDataSave(tForm, Header_TableIPCode);
                                t.ButtonEnabledAll(tForm, Header_TableIPCode, true);

                            }

                        }
                    }
                }
            }
        }

        public void Prop_RunTime_Work_AUTO_INS_JSON(Form tForm, DataSet dsData, string Prop_RunTime)
        {
            tToolBox t = new tToolBox();

            // Kendisinden önce kayıt oluşturması gerek bir IP var ise o çalışacak
            // ??= Yani fiş satırının ilk kaydı oluşacaksa kendinden önce fiş başlığının 
            //     kaydı oluşması gerekir. okey

            string RunTime = string.Empty;
            string Navigator = string.Empty;
            t.String_Parcala(Prop_RunTime, ref RunTime, ref Navigator, "|ds|");
            
            PROP_RUNTIME packet = new PROP_RUNTIME();
            RunTime = RunTime.Replace((char)34, (char)39);
            var prop_ = JsonConvert.DeserializeAnonymousType(RunTime, packet);
            
            string Header_TableIPCode = string.Empty;
            
            Header_TableIPCode = prop_.AUTO_INS_MST_IP.ToString();

            if (t.IsNotNull(Header_TableIPCode))
            {
                if (dsData != null)
                {
                    if (dsData.Tables[0].Rows.Count == 1)
                    {
                        string myProp = dsData.Namespace;
                        string KeyFName = t.MyProperties_Get(myProp, "KeyFName:");

                        if (dsData.Tables[0].Rows[0][KeyFName].ToString() == "")
                        {
                            DataSet dsDataHeader = t.Find_DataSet(tForm, "", Header_TableIPCode, "");

                            if (dsDataHeader != null)
                            {
                                if (dsDataHeader.Tables[0].Rows.Count == 0)
                                {
                                    tNewData(tForm, Header_TableIPCode);
                                }

                                tSave sv = new tSave();
                                sv.tDataSave(tForm, Header_TableIPCode);
                                t.ButtonEnabledAll(tForm, Header_TableIPCode, true);
                            }
                        }
                    }
                }
            }
        }
        
        #endregion Prop_RunTime

        #region Kriter Buttons Click >>

        public void btn_KriterListele_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();
            string function_name = "btn_KriterListe_Click";
            t.Takipci(function_name, "", '{');

            #region Tanımlar

            string TableIPCode = string.Empty;
            string VGridName = string.Empty;

            // tüm bu olayların oluştuğu form tespit ediliyor
            Form tForm = t.Find_Form(sender);

            string tbutton = ((DevExpress.XtraEditors.SimpleButton)sender).Text;
            string Where_Add = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDefaultActionDescription;

            //WaitForm WaitForm2 = new WaitForm();
            //SplashScreenManager.ShowForm(tForm, typeof(WaitForm2), true, true, false);

            // nesneler arasında bağlantıyı sağlayacak olan Table Kodu ve IP Kodu tespit ediliyor
            TableIPCode = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;

            // butonun AccessibleDescription üzerinde kendisinin bağımlı olduğu VGrid in adı mevcut
            // bu button create edilirken buraya yazılmıştı.
            VGridName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDescription;
            // VgridControl tespit ediliyor
            DevExpress.XtraVerticalGrid.VGridControl VGrid = t.Find_VGridControl(tForm, VGridName, "");

            string Prop_RunTime = VGrid.AccessibleDescription;

            #endregion Tanımlar

            #region Listele
            if (tbutton == "&Listele")
            {
                DataSet dsData = t.Find_DataSet(tForm, "", TableIPCode, function_name);
                if (dsData != null)
                {
                    Control cntrl = new Control();
                    cntrl = t.Find_Control_View(tForm, TableIPCode);

                    VGrid.FocusPrev();
                    Kriterleri_Uygula(dsData, VGrid, Where_Add, cntrl);

                    #region diğer işlemler
                    if (v.con_GotoRecord == "ON")
                    {
                        t.tGotoRecord(tForm, dsData, v.con_TableIPCode, v.con_GotoRecord_FName, v.con_GotoRecord_Value, -1);
                    }

                    if (v.con_SearchValue != "")
                    {
                        if (cntrl != null)
                        {
                            if (cntrl.ToString() == "DevExpress.XtraGrid.GridControl")
                            {
                                // unutma TKN 

                                //GridView tView = ((DevExpress.XtraGrid.GridControl)cntrl).MainView as GridView;
                                //tView.ApplyFindFilter(sp.con_SearchValue);
                                //tView
                            }

                        }
                    }
                    #endregion diğer işlemler

                    #region RunTime / Paralel TableIPCode

                    if (t.IsNotNull(Prop_RunTime))
                        Prop_RunTime_PRL_KRT(tForm, VGrid, Prop_RunTime, Where_Add);

                    #endregion RunTime

                }
                else
                {
                    MessageBox.Show("DİKKAT : Kriterlerin uygulanacağı DataControl tespit edilemedi ...");
                }

            }
            #endregion Listele

            #region Temizle
            if (tbutton == "&Temizle")
            {
                VGrid.FocusPrev();
                Kriterleri_Temizle(VGrid);
            }
            #endregion Temizle

            #region RowCollapse
            if (tbutton == "Kpt")
            {
                VGrid.CollapseAllRows();
            }
            #endregion RowCollapse

            #region RowExpand
            if (tbutton == "Aç")
            {
                VGrid.ExpandAllRows();
            }
            #endregion RowExpand

            //Close Wait Form
            //SplashScreenManager.CloseForm(false);

            t.Takipci(function_name, "", '}');
        }

        private void Prop_RunTime_PRL_KRT(Form tForm, DevExpress.XtraVerticalGrid.VGridControl VGrid, string Prop_RunTime, string Where_Add)
        {
            string s1 = "=ROW_PROP_RUNTIME:";
            string s2 = (char)34 + "PRL_KRT" + (char)34 + ": [";
            // "PRL_KRT": [

            if (Prop_RunTime.IndexOf(s1) > -1)
                Prl_Krt(tForm, VGrid, Prop_RunTime, Where_Add);

            if (Prop_RunTime.IndexOf(s2) > -1)
                Prl_Krt_JSON(tForm, VGrid, Prop_RunTime, Where_Add);
        }

        private void Prl_Krt_JSON(Form tForm, DevExpress.XtraVerticalGrid.VGridControl VGrid, string Prop_RunTime, string Where_Add)
        {
            tToolBox t = new tToolBox();

            string RunTime = string.Empty;
            string Navigator = string.Empty;
            t.String_Parcala(Prop_RunTime, ref RunTime, ref Navigator, "|ds|");

            PROP_RUNTIME packet = new PROP_RUNTIME();
            RunTime = RunTime.Replace((char)34, (char)39);
            var prop_ = JsonConvert.DeserializeAnonymousType(RunTime, packet);

            DataSet dsData_Prl = null;
            string Prl_TableIPCode = string.Empty;
            Control cntrl = new Control();

            foreach (var item in prop_.PRL_KRT)
            {
                Prl_TableIPCode = item.TABLEIPCODE.ToString();
                if (t.IsNotNull(Prl_TableIPCode))
                {
                    cntrl = t.Find_Control_View(tForm, Prl_TableIPCode);
                    dsData_Prl = t.Find_DataSet(tForm, "", Prl_TableIPCode, "");
                    if (dsData_Prl != null)
                    {
                        VGrid.FocusPrev();
                        Kriterleri_Uygula(dsData_Prl, VGrid, Where_Add, cntrl);
                    }
                }
            }

            #region örnek kod
            /*
            "PRL_KRT": [
            {
              "CAPTION": "Finans Bakiye",
              "TABLEIPCODE": "FBKY.FBKY_01"
            }
            ],
            */
            #endregion örnek kod
        }

        private void Prl_Krt(Form tForm, DevExpress.XtraVerticalGrid.VGridControl VGrid, string Prop_RunTime, string Where_Add)
        {
            tToolBox t = new tToolBox();

            if (t.IsData(ref Prop_RunTime, "PRL_KRT"))
            {
                string lockE = "=ROWE_";
                string row_block = string.Empty;

                DataSet dsData_Prl = null;
                string Prl_TableIPCode = string.Empty;
                Control cntrl = new Control();

                while (Prop_RunTime.IndexOf(lockE) > -1)
                {
                    row_block = t.Find_Properies_Get_RowBlock(ref Prop_RunTime, "PRL_KRT");

                    Prl_TableIPCode = t.MyProperties_Get(row_block, "TABLEIPCODE:");
                    if (t.IsNotNull(Prl_TableIPCode))
                    {
                        cntrl = t.Find_Control_View(tForm, Prl_TableIPCode);
                        dsData_Prl = t.Find_DataSet(tForm, "", Prl_TableIPCode, "");
                        if (dsData_Prl != null)
                        {
                            VGrid.FocusPrev();
                            Kriterleri_Uygula(dsData_Prl, VGrid, Where_Add, cntrl);
                        }
                    }
                }

                #region örnek kod
                /*
                1=ROW_PRL_KRT:1;
                1=CAPTION:Finans Bakiye;
                1=TABLEIPCODE:FBKY.FBKY_01;
                1=ROWE_PRL_KRT:1;
                */
                #endregion örnek kod
            }
        }

        public void External_Kriterleri_Uygula(DataSet dsData, string alias, string Where_Add, Control cntrl)
        {
            tToolBox t = new tToolBox();
            string function_name = "External Kriterleri_Uygula";
            t.Takipci(function_name, "", '{');

            #region Tanımlar

            // ŞİMDİLİK BUNU TEMİZLİĞİ External_Kriterleri_Uygula yı çağırdığın yerde yap
            // daha sonra /*xxxxKRITERLER*/ arasını temizleyerek yeni kriterleri at
            // böylece temizleğe gerek kalmaz her yeni kriter gerektiğinde yenisi çaılışır

            //t.tSqlSecond_Clear(ref dsData);
            // -----


            string myProp = dsData.Namespace.ToString();

            if (t.IsNotNull(alias) == false)
                alias = t.MyProperties_Get(myProp, "=TableLabel:");

            t.Alias_Control(ref alias);

            string aSQL = t.MyProperties_Get(myProp, "=SqlFirst:");
            aSQL = t.MySQL_Clear(aSQL);

            int fKRITERLER = aSQL.IndexOf("/*" + alias + "KRITERLER*/") + 14 + alias.Length;

            #endregion Tanımlar

            #region SQL'i çalıştır

            if ((fKRITERLER > 15) && t.IsNotNull(Where_Add))
            {
                aSQL = aSQL.Insert(fKRITERLER, Where_Add);

                t.Data_Read_Execute(dsData, ref aSQL, "", cntrl);
            }
            #endregion SQL'i çalıştır

            t.Takipci(function_name, "", '}');
        }

        private void Kriterleri_Uygula(DataSet dsData, VGridControl VGrid, string Where_Add, Control cntrl)
        {
            tToolBox t = new tToolBox();
            string function_name = "Kriterleri_Uygula";
            t.Takipci(function_name, "", '{');

            #region Tanımlar
            t.tSqlSecond_Clear(ref dsData);
            string myProp = dsData.Namespace.ToString();
            string alias = t.MyProperties_Get(myProp, "=TableLabel:");
            string aSQL = t.MyProperties_Get(myProp, "=SqlFirst:");
            string fullname = string.Empty;
            string fname = string.Empty;
            string fvalue = string.Empty;
            string fvalue2 = string.Empty;
            string tmpkriter = string.Empty;
            string fkriter = v.ENTER;
            string foperand = string.Empty;
            string foperand_id = string.Empty;
            string ttable_alias = string.Empty;
            string tkrt_alias = string.Empty;
            int t1 = VGrid.Rows.Count;
            int t2 = 0;
            int ffieldtype = 0;
            //int fKRITERLER = 0;
            aSQL = t.MySQL_Clear(aSQL);
            #endregion Tanımlar

            #region Vgrid row döngüsü
            // Vgrid üzerindeki category sayısı
            for (int i1 = 0; i1 < t1; i1++)
            {
                // category altındaki edittext sayısı
                t2 = VGrid.Rows[i1].ChildRows.Count;

                // t2 = 4 ise sorgu tipi even  (başlangıç, bitiş)
                // t2 = 2 ise sorgu tipi odd   (tek sorgu)

                #region categori altı
                for (int i2 = 0; i2 < t2; i2++)
                {
                    fullname = VGrid.Rows[i1].ChildRows[i2].Properties.FieldName.ToString();

                    t.TableIPCode_Get(fullname, ref ttable_alias, ref fname);

                    tkrt_alias = VGrid.Rows[i1].ChildRows[i2].Properties.CustomizationCaption.ToString();

                    if (t.IsNotNull(tkrt_alias))
                    {
                        //if (tkrt_alias.IndexOf(".") < 0)
                        //   fname = tkrt_alias + "." + fname;
                        //else fname = tkrt_alias + fname;

                        if (fullname.IndexOf(".") < 0)
                        {
                            fname = tkrt_alias + fname;
                        }
                    }

                    fvalue = string.Empty;
                    fvalue2 = string.Empty;
                    foperand = string.Empty;
                    foperand_id = string.Empty;
                    ffieldtype = 0;

                    #region // fieldin tipi nedir, tespiti

                    if (VGrid.Rows[i1].ChildRows[i2].Tag != null)
                        ffieldtype = System.Convert.ToInt32(VGrid.Rows[i1].ChildRows[i2].Tag);

                    // field tipine göre veri hazırlnacak 
                    if (VGrid.Rows[i1].ChildRows[i2].Properties.Value != null)
                    {
                        fvalue = VGrid.Rows[i1].ChildRows[i2].Properties.Value.ToString();

                        #region // Date or SmallDatetime
                        //if ((ffieldtype == 40) || (ffieldtype == 58)) 
                        //{
                        //    if ((i2 == 1) && (t2 == 4)) // even ise
                        //    {
                        //        //fvalue = fvalue.Substring(0, 12) + " 59:00:00'";
                        //        if (t.IsNotNull(fvalue))
                        //        {
                        //            // TARIHI 1 GUN ARTTIRARAK SAAT 00:00:00 SİKİNTİSİ

                        //            //DateTime x = Convert.ToDateTime(fvalue);
                        //            //x = x.AddDays(1);
                        //            //fvalue = Convert.ToString(x);
                        //        }
                        //    }
                        //}
                        #endregion

                        fvalue = t.tData_Convert(fvalue, ffieldtype);

                        // bu ikinci veriye Like lar yüzünden ihtiyaç duyuldu.
                        fvalue2 = t.Str_Check(VGrid.Rows[i1].ChildRows[i2].Properties.Value.ToString());
                    }
                    #endregion

                    #region // Vgridin satırı sorgu çeşitleri değilse ve elle girilmiş veri var  ise

                    if ((fname.IndexOf("bas_sorgu_") == -1) &&
                        (fname.IndexOf("bit_sorgu_") == -1) &&
                        (fname.IndexOf("null") == -1) &&
                        ((t.IsNotNull(fvalue)) || (t.IsNotNull(fvalue2)))
                        )
                    {
                        // operand tipi belirlenecek
                        if (t2 == 4) // even ise (çift)
                        {
                            if (VGrid.Rows[i1].ChildRows[i2 + 2].Properties.Value != null)
                                foperand_id = VGrid.Rows[i1].ChildRows[i2 + 2].Properties.Value.ToString();
                        }

                        if (t2 == 2) // odd ise (tek)
                        {
                            if (VGrid.Rows[i1].ChildRows[i2 + 1].Properties.Value != null)
                                foperand_id = VGrid.Rows[i1].ChildRows[i2 + 1].Properties.Value.ToString();
                        }

                        foperand = Kriter_Operand_Find(foperand_id);
                        //---

                        // parçalar bir araya geldi, şimdi and ile başlayan yeni bir where parçası oluştulacak

                        if (foperand == "%%")
                            tmpkriter = tmpkriter + " and " + fullname + " like '%" + fvalue2 + "%' " + v.ENTER;
                        else if (foperand == "%")
                            tmpkriter = tmpkriter + " and " + fullname + " like '" + fvalue2 + "%' " + v.ENTER;
                        else if (foperand != string.Empty)
                            tmpkriter = tmpkriter + " and " + fullname + " " + foperand + " " + fvalue + v.ENTER;

                        // ------

                        if (t.IsNotNull(tmpkriter))
                        {
                            if (t.IsNotNull(tkrt_alias))
                            {
                                aSQL = t.SQLWhereAdd(aSQL, tkrt_alias, tmpkriter, "KRITER");
                            }
                            else
                            {
                                fkriter = fkriter + tmpkriter;
                            }

                            tmpkriter = string.Empty;
                        }

                    }
                    #endregion

                    // --------------------------

                }
                #endregion categori altı


            }
            #endregion Vgrid row döngüsü

            #region SQL'i çalıştır

            if ((t.IsNotNull(fkriter)) || (t.IsNotNull(Where_Add)))
            {
                if (t.IsNotNull(fkriter))
                    t.tKriter_Ekle(ref aSQL, alias, fkriter);
                if (t.IsNotNull(Where_Add))
                    t.tKriter_Ekle(ref aSQL, alias, Where_Add);
            }

            aSQL = t.MySQL_Clear(aSQL);

            t.Data_Read_Execute(dsData, ref aSQL, "", cntrl);

            //for (int i1 = 0; i1 < t1; i1++)
            //{
            //    // category altındaki edittext sayısı
            //    t2 = VGrid.Rows[i1].ChildRows.Count;
            //    for (int i2 = 0; i2 < t2; i2++)
            //    {
            //        VGrid.Rows[i1].ChildRows[i2].Expanded = true;
            //    }
            // veya
            //    VGrid.FullExpandRow(VGrid.Rows[i1]);
            // ------------------   
            //    VGrid.Rows[i1].Expanded = true;
            //}

            #endregion SQL'i çalıştır

            t.Takipci(function_name, "", '}');
        }

        private string Kriter_Operand_Find(string operand_id)
        {
            string sonuc = string.Empty;

            if (operand_id == string.Empty) sonuc = "";
            if (operand_id == "0") sonuc = "";
            if (operand_id == "1") sonuc = ">=";
            if (operand_id == "2") sonuc = ">";
            if (operand_id == "3") sonuc = "=";
            if (operand_id == "4") sonuc = "<=";
            if (operand_id == "5") sonuc = "<";
            if (operand_id == "6") sonuc = "%%";
            if (operand_id == "7") sonuc = "%";

            return sonuc;
        }

        private void Kriterleri_Temizle(VGridControl VGrid)
        {
            tToolBox t = new tToolBox();
            string function_name = "Kriterleri_Temizle";
            t.Takipci(function_name, "", '{');

            //VGrid.Rows.Count;
            int t1 = VGrid.Rows.Count;
            int t2 = 0;
            string fname = string.Empty;

            for (int i1 = 0; i1 < t1; i1++)
            {
                // category altındaki edittext sayısı
                t2 = VGrid.Rows[i1].ChildRows.Count;

                // t2 = 4 ise sorgu tipi even  (başlangıç, bitiş)
                // t2 = 2 ise sorgu tipi odd   (tek sorgu)

                for (int i2 = 0; i2 < t2; i2++)
                {
                    fname = VGrid.Rows[i1].ChildRows[i2].Properties.FieldName.ToString();

                    if ((fname.IndexOf("bas_sorgu_") == -1) &&
                        (fname.IndexOf("bit_sorgu_") == -1))
                    {
                        VGrid.Rows[i1].ChildRows[i2].Properties.Value = "";

                        if (t2 == 4) // even ise
                        {
                            if (i2 == 0)
                                VGrid.Rows[i1].ChildRows[i2 + 2].Properties.Value = ((short)(v.EsitVeBuyuk));

                            if (i2 == 1)
                                VGrid.Rows[i1].ChildRows[i2 + 2].Properties.Value = ((short)(v.EsitVeKucuk));
                        }

                        if (t2 == 2) // odd ise
                        {
                            if (VGrid.Rows[i1].ChildRows[i2 + 1].Properties.RowEdit.Name.ToString() == "ODD_LIKE")
                                VGrid.Rows[i1].ChildRows[i2 + 1].Properties.Value = ((short)(v.Benzerleri_Tam));

                            if (VGrid.Rows[i1].ChildRows[i2 + 1].Properties.RowEdit.Name.ToString() == "ODD_NOTLIKE")
                                VGrid.Rows[i1].ChildRows[i2 + 1].Properties.Value = ((short)(v.Esit));
                        }
                    }

                }
            }

            t.Takipci(function_name, "", '}');
        }

        #endregion  Kriter Buttons Click <<

        #region DataWizard Buttons Click >>
        public void btn_DataWizard_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            // şimdilik gerek kalmadı
        }
        #endregion DataWizard Buttons Click <<

        #region Param Buttons Click >>

        public void btn_Param_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();
            string function_name = "btn Param_Click";
            t.Takipci(function_name, "", '{');

            // TableName / TableIPCode geliyor
            string prp_type = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;
            string Kriterler = string.Empty;

#region Tanımlar
            // tüm bu olayların oluştuğu form tespit ediliyor
            Form tForm = t.Find_Form(sender);

            string tbutton = ((DevExpress.XtraEditors.SimpleButton)sender).Text;

            // VgridControl tespit ediliyor
            string[] controls = new string[] { "DevExpress.XtraVerticalGrid.VGridControl" };
            Control cntrl = t.Find_Control(tForm, "", "tPARAMS_" + prp_type, controls);
#endregion Tanımlar

#region Cloase
            if (tbutton == "&Çıkış")
            {
                tForm.AccessibleDefaultActionDescription = "CLOSE";
                tForm.Close();
            }
#endregion Close

#region Listele
            if (tbutton == "&Listele")
            {
                string ftype = string.Empty;
                if (tForm.Tag != null)
                    ftype = tForm.Tag.ToString();

                Kriterler = string.Empty;
                Kriterler = t.Read_ParamsValue(tForm, (DevExpress.XtraVerticalGrid.VGridControl)cntrl, prp_type);

                if (ftype == "DIALOG")
                {
                    tForm.AccessibleDefaultActionDescription = Kriterler;
                    tForm.Close();
                }
                else
                {
                    // Devam işlemi 
                    ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDefaultActionDescription = Kriterler;
                }
            }
#endregion Listele

#region Daha fazla
            if (tbutton == "Daha fazla")
            {
                ((DevExpress.XtraEditors.SimpleButton)sender).Text = "Kısıtlı";
                Param_Type_Create(tForm, (DevExpress.XtraVerticalGrid.VGridControl)cntrl, prp_type, false,
                              t.myInt32(((DevExpress.XtraVerticalGrid.VGridControl)cntrl).Tag.ToString()));
            }
#endregion Daha fazla

#region Kısıtlı
            if (tbutton == "Kısıtlı")
            {
                ((DevExpress.XtraEditors.SimpleButton)sender).Text = "Daha fazla";
                Param_Type_Create(tForm, (DevExpress.XtraVerticalGrid.VGridControl)cntrl, prp_type, true,
                              t.myInt32(((DevExpress.XtraVerticalGrid.VGridControl)cntrl).Tag.ToString()));
            }
#endregion Kısıtlı

#region Temizle
            if (tbutton == "&Temizle")
            {
                ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).FocusPrev();

                t.Clear_ParamsValue(tForm, prp_type);
                //tProperties_Button_Clear((DevExpress.XtraVerticalGrid.VGridControl)cntrl);
            }
#endregion Temizle

#region RowCollapse
            if (tbutton == "Kapat")
            {
                ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).CollapseAllRows();
            }
#endregion RowCollapse

#region RowExpand
            if (tbutton == "Aç")
            {
                ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).ExpandAllRows();
            }
#endregion RowExpand

            t.Takipci(function_name, "", '}');
        }

        #region Sub Param Functions

        private void Param_Type_Create(Form tForm, VGridControl tVGridControl, string TableName, Boolean Kist, int tTableType)
        {
            tToolBox t = new tToolBox();

            DataSet dsKrtr = t.Find_DataSet(tForm, "", TableName, "");

            if (dsKrtr != null)
            {
                tCreateObject co = new tCreateObject();

                //tVGridControl.Rows.
                tVGridControl.Rows.Clear();

                co.Create_Param_Columns(tVGridControl, dsKrtr, TableName, Kist, tTableType);
            }

        }

        #endregion Sub Param Functions

        #endregion  Param Buttons Click <<


        #region Category Events

        public void ctg_DataNavigator_CatList_PositionChanged(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            Form tForm = t.Find_Form(sender);

            ctg_Category_Detail_Read(tForm, ((DevExpress.XtraEditors.DataNavigator)sender));
        }

        public void ctg_DataNavigator_CatDetail_PositionChanged(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            Form tForm = t.Find_Form(sender);

            int NewPosition = ((DevExpress.XtraEditors.DataNavigator)sender).Position;
            object tDataTable = ((DevExpress.XtraEditors.DataNavigator)sender).DataSource;
            DataSet tdsData = ((DataTable)tDataTable).DataSet;

            if (NewPosition == -1) return;

            Control cntrl = new Control();
            cntrl = t.Find_Control_View(tForm, "tTreeList_CatDetail_");

            if (cntrl != null)
            {
                string FieldName = ((DevExpress.XtraTreeList.TreeList)cntrl).AccessibleDescription;
                Int16 FieldType = System.Convert.ToInt16(((DevExpress.XtraTreeList.TreeList)cntrl).AccessibleDefaultActionDescription);

                string TableIPCode = tdsData.Namespace;
                string fvalue = tdsData.Tables[0].Rows[NewPosition]["LKP_ID"].ToString();

                string TableCode = string.Empty;
                string IPCode = string.Empty;

                t.TableIPCode_Get(TableIPCode, ref TableCode, ref IPCode);
                //---

                string Where_And = " and  [" + TableCode + "]." + FieldName + " = " + t.tData_Convert(fvalue, FieldType);

                //--- Tümü seçilmiş ise
                if (fvalue == "-100")
                    Where_And = string.Empty;

                string[] controls = new string[] { "DevExpress.XtraEditors.SimpleButton" };
                Control c = t.Find_Control(tForm, "simpleButton_Kriter_Listele", TableIPCode, controls);
                if (c != null)
                {
                    ((DevExpress.XtraEditors.SimpleButton)c).AccessibleDefaultActionDescription = Where_And;
                    //((DevExpress.XtraEditors.SimpleButton)c).PerformClick();
                    btn_KriterListele_Click(((DevExpress.XtraEditors.SimpleButton)c), EventArgs.Empty);

                }

                //MessageBox.Show(Where_And);
            }
        }

        private void ctg_Category_Detail_Read(Form tForm, DevExpress.XtraEditors.DataNavigator tDataNavigator)
        {

            int NewPosition = tDataNavigator.Position;
            object tDataTable = tDataNavigator.DataSource;
            DataSet tdsData = ((DataTable)tDataTable).DataSet;

            if (NewPosition == -1) return;

            string TableName = tdsData.Namespace;

            string FieldName = tdsData.Tables[0].Rows[NewPosition]["FIELD_NAME"].ToString();
            Int16 FieldType = System.Convert.ToInt16(tdsData.Tables[0].Rows[NewPosition]["FIELD_TYPE"].ToString());

            string Type_Name = tdsData.Tables[0].Rows[NewPosition]["LOOKUP_CODE"].ToString();
            Int16 Type = System.Convert.ToInt16(tdsData.Tables[0].Rows[NewPosition]["TLKP_TYPE"].ToString());

            string SQL = string.Empty;

            // Data Seçenekleri
            if (Type == 107)
            {
                //SQL = tSql.DATA_Types_SQL(Type_Name);
            }
            else
            {
                //SQL = tSql.ImageComboBox_Fill_SQL(Type_Name, Type);
            }

            if (SQL != "")
            {
                ctg_Category_Detail_Read(tForm, TableName, SQL, FieldName, FieldType);
            }

        }

        private void ctg_Category_Detail_Read(Form tForm, string TableName, string SQL,
                                              string FieldName, Int16 FieldType)
        {
            tToolBox t = new tToolBox();

            string function_name = "Category_Detail_Read";

            Control cntrl = new Control();
            cntrl = t.Find_Control_View(tForm, "tTreeList_CatDetail_");

            if (cntrl != null)
            {
                ((DevExpress.XtraTreeList.TreeList)cntrl).AccessibleDescription = FieldName;
                ((DevExpress.XtraTreeList.TreeList)cntrl).AccessibleDefaultActionDescription = FieldType.ToString();

                object tDataTable = ((DevExpress.XtraTreeList.TreeList)cntrl).DataSource;
                DataSet tdsData = ((DataTable)tDataTable).DataSet;

                t.TableRemove(tdsData);

                t.SQL_Read_Execute(v.dBaseNo.Project, tdsData, ref SQL, "", function_name);

                DataNavigator tDataNavigator = null;
                tDataNavigator = t.Find_DataNavigator(tForm, "tDataNavigator_CatDetail");//, function_name);

                ((DevExpress.XtraTreeList.TreeList)cntrl).DataSource = tdsData.Tables[0];
                tDataNavigator.DataSource = tdsData.Tables[0];
            }

        }

        private void ctg_Category_Form_Shown(Form tForm)
        {
            tToolBox t = new tToolBox();
            //string function_name = "Category_Form_Shown";

            DataNavigator tDataNavigator = null;

            tDataNavigator = t.Find_DataNavigator(tForm, "tDataNavigator_CatList");//, function_name);

            if (tDataNavigator != null)
            {
                ctg_Category_Detail_Read(tForm, tDataNavigator);
            }

        }

        #endregion Category Events

        #region WizardControl Button Click

        public void tWizardControl_CancelClick(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string Question = string.Empty;
            Question = "İşlemden vazgeçmek istediğinize emin misiniz?";

            DialogResult answer = MessageBox.Show(Question, "Onay İşlemi", MessageBoxButtons.YesNo);

            switch (answer)
            {
                case DialogResult.Yes:
                    {
                        Control c = ((DevExpress.XtraWizard.WizardControl)sender).Parent;

                        ((DevExpress.XtraWizard.WizardControl)sender).Dispose();

                        c.Dispose();

                        break; // break ifadesini sakın silme
                    }
                case DialogResult.No:
                    {
                        break; // break ifadesini sakın silme
                    }
                case DialogResult.Cancel:
                    {
                        break; // break ifadesini sakın silme
                    }

            }

        }

        public void tWizardControl_FinishClick(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tToolBox t = new tToolBox();

            Form tForm = t.Find_Form(sender);
            string TableIPCode = ((DevExpress.XtraWizard.WizardControl)sender).AccessibleName;

            t.Find_DataNavigator_ButtonLink(tForm, TableIPCode, v.nv_24_Kaydet.ToString());

            //simpleButton_kaydet.TabIndex = 24;
            //simpleButton_kaydet.Text = "Kaydet";
            //simpleButton_kaydet.Click += new System.EventHandler(btn_Navigotor_Click);
            //simpleButton_kaydet.AccessibleName = TableIPCode;
        }

        public void tWizardControl_HelpClick(object sender, DevExpress.XtraWizard.WizardButtonClickEventArgs e)
        {
            //MessageBox.Show("help..");
        }

        public void tWizardControl_NextClick(object sender, DevExpress.XtraWizard.WizardCommandButtonClickEventArgs e)
        {
            //MessageBox.Show("next..  " + sender.ToString() + " // " + e.Page.Name.ToString());
            tToolBox t = new tToolBox();
            Form tForm = ((DevExpress.XtraWizard.WizardControl)sender).FindForm();
            Control page = t.Find_Control(tForm, e.Page.Name.ToString());
            if (page != null)
            {
                MessageBox.Show("next..  " + sender.ToString() + " // " + e.Page.Name.ToString());
            }
        }

        public void tWizardControl_PrevClick(object sender, DevExpress.XtraWizard.WizardCommandButtonClickEventArgs e)
        {
            //MessageBox.Show("prev..");
        }

        #endregion WizardControl Button Click

        #region AccordionDinamikElement_Click
        public void tAccordionDinamikElement_Click(object sender, EventArgs e)
        {
            DevExpress.XtraBars.Navigation.AccordionControl menuControl =
                ((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).AccordionControl;

            if (menuControl.Elements.Count <= 0) return;

            //tToolBox t = new tToolBox();

            string ButtonName = string.Empty;
            string Caption = string.Empty;
            string selectItemValue = string.Empty;
            string selectItemHint = string.Empty;

            ButtonName = ((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Name.ToString();
            Caption = ((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Text;

            if (((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Tag != null)
                selectItemValue = ((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Tag.ToString();

            
            // RibbonPage Visible için gerekli olan değer
            // Yani AccordionControl üzerinde hangi item tıklandıysa
            // onunla ilgili bir TabPage oluşturuluyor
            // birde konuyla ilgili RibbonPage visible = true ediliyor
            // bu üç nesne arasındaki bağlantı accordion menu için hazırlanan selectteki GRUPVALUE ile çözülüyor
            
            // click lenen itemın hangi grup altında olduğunu AccordionControl üzerinde bir yere işaret bırakıyor
            // bu işarete bakarak kendisine bağlı olan Ribbon page leri ayarlanıyor (visible)

            if (((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Hint != null)
                selectItemHint = ((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Hint.ToString();
            
            // bunula yeni açılan tabPage bağlı RibbonPage tespit etmeye yarıyor
            menuControl.AccessibleDefaultActionDescription = selectItemHint;

            // selectItemHint değerini yeni oluşturulan tabPage nin  AccessibleDefaultActionDescription set ettiğimizdede
            // kullanıcı manuel tabPage değişitirince hangi RibbonPage aktif etmesi gerektiğini biliyor

            //if (t.IsNotNull(selectItemValue) == false) return;
            if ((selectItemValue == string.Empty) ||
                (selectItemValue == "")) return;

            string Read_TableIPCode = menuControl.AccessibleName;
            string DetailFName = menuControl.AccessibleDescription;

            //MessageBox.Show(
            //     ((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Text + " : " + selectItemValue + " : " + Read_TableIPCode+ " : " + DetailFName);


            //((DevExpress.XtraToolbox.ToolboxControl)sender).OptionsView.MenuButtonCaption = e.Item.Caption;

            // Group altındaki hesaplar / item lar
            if (ButtonName.IndexOf("item_MainItem_") == -1)
            {
                DevExpress.XtraBars.Navigation.AccordionControlElement owner =
                    ((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).OwnerElement;

                AccordionDinamik_SubPage_Preparing(menuControl, owner, selectItemValue, Caption, selectItemHint); // selectItemHint = MenuValue
            }

            // Main group altındaki diğer group lar
            if (ButtonName.IndexOf("item_MainItem_") > -1)
            {
                AccordionDinamik_SubGroup_Preparing(menuControl, Read_TableIPCode, DetailFName, selectItemValue);
            }

        }

        private void AccordionDinamik_SubPage_Preparing(
            DevExpress.XtraBars.Navigation.AccordionControl menuControl,
            DevExpress.XtraBars.Navigation.AccordionControlElement owner,
            string selectItemValue,
            string Caption,
            string MenuValue
            )
        {
            if (owner.Tag == null)
            {
                MessageBox.Show("DİKKAT : Açılacak sayfa için gerekli bilgiler tanımlanmamış ...");
            }

            if (owner.Tag != null)
            {
                Form tForm = menuControl.FindForm();
                string Prop_Navigator = owner.Tag.ToString();

                //MessageBox.Show(owner.Name.ToString() + v.ENTER + Prop_Navigator);

                tSubView_(tForm, Prop_Navigator, selectItemValue, Caption, MenuValue);
            }
        }

        private void AccordionDinamik_SubGroup_Preparing(
              DevExpress.XtraBars.Navigation.AccordionControl menuControl,
              string Read_TableIPCode, string DetailFName, string selectItemValue)
        {
            // menu hazır olan ile yeni seçilmiş olan MainItem aynı olabilir
            // yani menu zaten hazır tekrar aynı item clicke basılmış olabilir
            // yeniden aynı sub menuleri tekrarlamsın diye geri dön marş marş

            if (menuControl.Tag != null)
            {
                if (menuControl.Tag.ToString() == selectItemValue)
                    return;
            }

            menuControl.Tag = selectItemValue;

            tToolBox t = new tToolBox();
            
            if (t.IsNotNull(Read_TableIPCode) &&
                t.IsNotNull(DetailFName)
                )
            {
                #region
                tEvents ev = new tEvents();

                Form tForm = menuControl.FindForm();
                DataSet dsRead = null;
                DataNavigator tdN_Read = null;

                t.Find_DataSet(tForm, ref dsRead, ref tdN_Read, Read_TableIPCode);

                if (t.IsNotNull(dsRead) == false)
                {
                    v.Kullaniciya_Mesaj_Var = "Data yok : " + Read_TableIPCode;
                    return;
                }

                int i2 = dsRead.Tables[0].Rows.Count;
                int elementCount = 1;

                string read_CaptionFName = string.Empty;
                string read_KeyFName = string.Empty;
                string chc_FName = string.Empty;
                string chc_Value = string.Empty;

                string Prop_Navigator = string.Empty;
                string itemCaption = string.Empty;
                string itemValue = string.Empty;
                string itemName = string.Empty;
                string s1, s2 = string.Empty;

                // tüm grupları sırayla ele alalım
                foreach (DevExpress.XtraBars.Navigation.AccordionControlElement pGroup in menuControl.Elements)
                {
                    // MainGroup ise ellenmez, yani ana hesaplar (Banka - Şubeleri)
                    // diğer gruplar ise önce temizleniyor
                    // sonra seçilen hesabın (Banka - Şube nin ) .... açıklamaya devam et ?
                    #region
                    if ((pGroup.Name.IndexOf("item_MainGroup_") == -1) &&
                        (pGroup.Name.IndexOf("item_Group_") > -1))
                    {
                        elementCount = 1;
                        pGroup.Elements.Clear();
                        pGroup.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
                        pGroup.Appearance.Normal.ForeColor = System.Drawing.Color.Black;
                        pGroup.Appearance.Normal.Options.UseFont = true;
                        pGroup.Appearance.Normal.Options.UseForeColor = true;

                        #region
                        if (pGroup.Tag != null)
                        {
                            Prop_Navigator = pGroup.Tag.ToString();

                            s1 = "=ROW_PROP_NAVIGATOR:";
                            s2 = (char)34 + "TABLEIPCODE_LIST" + (char)34 + ": [";
                            // "TABLEIPCODE_LIST": [

                            // OLD
                            if (Prop_Navigator.IndexOf(s1) > -1)
                            {
                                // Group bilgileri için 
                                read_CaptionFName = t.MyProperties_Get(Prop_Navigator, "RTABLEIPCODE:"); // esasında okunan display edilecek field
                                read_KeyFName = t.MyProperties_Get(Prop_Navigator, "RKEYFNAME:");  // esasında okunan ref Id field

                                chc_FName = t.MyProperties_Get(Prop_Navigator, "CHC_FNAME:");  // esasında okunan check field name
                                chc_Value = t.MyProperties_Get(Prop_Navigator, "CHC_VALUE:");  // esasında okunan check value field name
                            }

                            // JSON
                            if (Prop_Navigator.IndexOf(s2) > -1)
                            {
                                PROP_NAVIGATOR packet = new PROP_NAVIGATOR();
                                Prop_Navigator = Prop_Navigator.Replace((char)34, (char)39);
                                var prop_ = JsonConvert.DeserializeAnonymousType(Prop_Navigator, packet);

                                // Group bilgileri için 
                                foreach (var item in prop_.TABLEIPCODE_LIST)
                                {
                                    read_CaptionFName = item.RTABLEIPCODE;
                                    read_KeyFName = item.RKEYFNAME;
                                }
                                
                                chc_FName = prop_.CHC_FNAME;
                                chc_Value = prop_.CHC_VALUE;
                            }
                            
                            #region
                            for (int i = 0; i < i2; i++)
                            {
                                if ((dsRead.Tables[0].Rows[i][chc_FName].ToString() == chc_Value) &&
                                    (dsRead.Tables[0].Rows[i][DetailFName].ToString() == selectItemValue))
                                {

                                    itemCaption = dsRead.Tables[0].Rows[i][read_CaptionFName].ToString();
                                    itemValue = dsRead.Tables[0].Rows[i][read_KeyFName].ToString();

                                    //DevExpress.XtraToolbox.ToolboxItem barButtonItem = new DevExpress.XtraToolbox.ToolboxItem();
                                    DevExpress.XtraBars.Navigation.AccordionControlElement barButtonItem =
                                       new DevExpress.XtraBars.Navigation.AccordionControlElement();

                                    itemName = "item_" + itemValue;// refid;

                                    barButtonItem.Name = itemName;
                                    barButtonItem.Text = itemCaption;
                                    barButtonItem.Tag = itemValue;
                                    barButtonItem.Hint = chc_Value;

                                    barButtonItem.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
                                    barButtonItem.Appearance.Normal.ForeColor = System.Drawing.Color.Black;
                                    barButtonItem.Appearance.Normal.Options.UseFont = true;
                                    barButtonItem.Appearance.Normal.Options.UseForeColor = true;

                                    barButtonItem.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
                                    barButtonItem.Click += new System.EventHandler(ev.tAccordionDinamikElement_Click);

                                    pGroup.Elements.Add(barButtonItem);
                                    elementCount++;
                                }
                            }
                            #endregion

                            if (elementCount > 1)
                            {
                                pGroup.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
                                //pGroup.Appearance.Normal.ForeColor = System.Drawing.Color.Green;
                                pGroup.Appearance.Normal.Options.UseFont = true;
                                pGroup.Appearance.Normal.Options.UseForeColor = true;
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion
            }
        }

        #endregion AccordionDinamikElement_Click



        //-------------------------------

        #region SubFunctions

        #region Data_Refresh
        public void Data_Refresh(Form tForm, DataSet dsData, DataNavigator tDataNavigator)
        {
            /// *** işin sıralaması değişti

            // eski sürüm

            // Database okunuyor
            //
            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";DR_sdR1;";

            tSubDetail_Refresh(dsData, tDataNavigator);
            //tSubWork_Refresh_(tForm, dsData, tDataNavigator);

            // View/Show işi yapılıyor
            //
            if (tDataNavigator.AccessibleDescription != null)
                tSubView_Refresh(tForm, dsData, tDataNavigator);
            
        }
        #endregion Data_Refresh

        #region Search Engine / Arama Motoru

        public bool Search_Engines(Form tForm, string TargetTableIPCode, string Search_Values, PROP_NAVIGATOR prop_)
        {
            /// NEW SEARCH 3
            /// buraya gelen bilgi bunlar
            /// 
            ///Type:SearchEngine;[
            ///{
            ///"CAPTION": "Search",
            ///"BUTTONTYPE": "58",
            ///"TABLEIPCODE_LIST": [
            ///    {
            ///    "CAPTION": "ID > Cari ID",
            ///    "TABLEIPCODE": "null",
            ///    "TABLEALIAS": "null",
            ///    "KEYFNAME": "CARI_ID",
            ///    "RTABLEIPCODE": "null",
            ///    "RKEYFNAME": "ID",
            ///    "MSETVALUE": "null",
            ///    "WORKTYPE": "SETDATA",
            ///    "CONTROLNAME": "null",
            ///    "DCCODE": "null"
            ///    }
            ///    ],
            ///"FORMNAME": "null",
            ///"FORMCODE": "CR.CR_OMARA_L01",
            ///"FORMTYPE": "null",
            ///"FORMSTATE": "null",
            ///"CHC_IPCODE": "null",
            ///"CHC_FNAME": "null",
            ///"CHC_VALUE": "null",
            ///"CHC_OPERAND": "null",
            ///"CHC_IPCODE_SEC": "null",
            ///"CHC_FNAME_SEC": "null",
            ///"CHC_VALUE_SEC": "null",
            ///"CHC_OPERAND_SEC": "null"
            ///  }
            ///]
            
            tToolBox t = new tToolBox(); 

            bool onay = false;

            v.search_VALUE = Search_Values;

            /// SYS_VARIABLES tablosu ise extra işlem mevcut
            /// bu tabloda her row da ayrı ayrı Search bilgisi mevcut
            /// yani bir satırda Kasalar için search ederken, altındaki rowda bankaları search edebilir
            #region SYS_VARIABLES
            //if (myPropSearch.IndexOf("SYS_VARIABLES") > 0)
            //{
            //    DataSet ds = null;
            //    DataNavigator dN = null;

            //    t.Find_DataSet(tForm, ref ds, ref dN, TargetTableIPCode);
            //    //PROP_SEARCH
            //    if (t.IsNotNull(ds))
            //    {
            //        if (ds.Tables[0].Rows[dN.Position]["LKP_PROP_NAVIGATOR"] != null)
            //            myPropSearch = ds.Tables[0].Rows[dN.Position]["LKP_PROP_NAVIGATOR"].ToString();

            //        if (t.IsNotNull(myPropSearch) == false) return false;
            //    }
            //}
            #endregion

            
            string SearchTableIPCode = prop_.TARGET_TABLEIPCODE.ToString();
            string SearchFormCode = prop_.FORMCODE.ToString();
            
            /// sadece SearchTableIPCode var ise 
            if (t.IsNotNull(SearchTableIPCode) && (t.IsNotNull(SearchFormCode) == false))
                onay = Search_Engine_TableIPCode_JSON_2(tForm, prop_, TargetTableIPCode);

            
            /// sadece SearchFormCode var ise 
            if ((t.IsNotNull(SearchTableIPCode) == false) && t.IsNotNull(SearchFormCode))
                onay = Search_Engine_FormCode_JSON(tForm, prop_, TargetTableIPCode);
            
            /// işi bitti
            v.search_VALUE = "";

            return onay;
        }

        public Boolean Search_Engines_IPTAL(object sender, string Search_Values, byte search_type, string myPropSearch)
        {
            tToolBox t = new tToolBox();

            if (t.IsNotNull(myPropSearch) == false) return false;

            //string function_name = "SearchEngine";
            string TargetTableIPCode = string.Empty;

            v.search_VALUE = Search_Values;

            #region Find Form
            Form tForm = null;

            if (sender.ToString() == "DevExpress.XtraEditors.ImageComboBoxEdit")
            {
                TargetTableIPCode = ((DevExpress.XtraEditors.ImageComboBoxEdit)sender).Properties.AccessibleName;
                tForm = ((DevExpress.XtraEditors.ImageComboBoxEdit)sender).FindForm();
            }

            if (sender.ToString() == "DevExpress.XtraEditors.ButtonEdit")
            {
                TargetTableIPCode = ((DevExpress.XtraEditors.ButtonEdit)sender).Properties.AccessibleName;
                tForm = ((DevExpress.XtraEditors.ButtonEdit)sender).FindForm();
            }

            if (sender.ToString() == "DevExpress.XtraEditors.SimpleButton")
            {
                TargetTableIPCode = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;
                tForm = ((DevExpress.XtraEditors.SimpleButton)sender).FindForm();
            }
            #endregion

            /// SYS_VARIABLES tablosu ise extra işlem mevcut
            /// bu tabloda her row da ayrı ayrı Search bilgisi mevcut
            /// yani bir satırda Kasalar için search ederken, altındaki rowda bankaları search edebilir
            #region SYS_VARIABLES
            if (myPropSearch.IndexOf("SYS_VARIABLES") > 0)
            {
                DataSet ds = null;
                DataNavigator dN = null;

                t.Find_DataSet(tForm, ref ds, ref dN, TargetTableIPCode);
                //PROP_SEARCH
                if (t.IsNotNull(ds))
                {
                    if (ds.Tables[0].Rows[dN.Position]["LKP_PROP_NAVIGATOR"] != null)
                        myPropSearch = ds.Tables[0].Rows[dN.Position]["LKP_PROP_NAVIGATOR"].ToString();

                    if (t.IsNotNull(myPropSearch) == false) return false;
                }
            }
            #endregion

            bool onay = false;
            string SearchTableIPCode = string.Empty;
            string SearchFormCode = string.Empty;
                        
            string s1 = "=ROW_PROP_SEARCH:0";
            string s2 = (char)34 + "GET_FIELD_LIST" + (char)34 + ": [";
            //"GET_FIELD_LIST": [

            #region old search
            if (myPropSearch.IndexOf(s1) > -1)
            {
                SearchTableIPCode = t.MyProperties_Get(myPropSearch, "TABLEIPCODE:");
                SearchFormCode = t.MyProperties_Get(myPropSearch, "FORMCODE:");

                /// sadece SearchTableIPCode var ise 
                if (t.IsNotNull(SearchTableIPCode) && (t.IsNotNull(SearchFormCode) == false))
                    onay = Search_Engine_TableIPCode(tForm, myPropSearch, TargetTableIPCode);

                /// sadece SearchFormCode var ise 
                if ((t.IsNotNull(SearchTableIPCode) == false) && t.IsNotNull(SearchFormCode))
                    onay = Search_Engine_FormCode(tForm, myPropSearch, TargetTableIPCode);
            }
            #endregion

            #region JSON new search
            if (myPropSearch.IndexOf(s2) > -1)
            {
                PROP_SEARCH packet = new PROP_SEARCH();
                t.Str_Remove(ref myPropSearch, "Type:SearchEngine;");
                myPropSearch = myPropSearch.Replace((char)34, (char)39);
                var prop_ = JsonConvert.DeserializeAnonymousType(myPropSearch, packet);
                
                SearchTableIPCode = prop_.TABLEIPCODE.ToString();
                SearchFormCode = prop_.FORMCODE.ToString();

                /// sadece SearchTableIPCode var ise 
                ///if (t.IsNotNull(SearchTableIPCode) && (t.IsNotNull(SearchFormCode) == false))
                ///    onay = Search_Engine_TableIPCode_JSON(tForm, prop_, TargetTableIPCode);

                /// sadece SearchFormCode var ise 
                ///if ((t.IsNotNull(SearchTableIPCode) == false) && t.IsNotNull(SearchFormCode))
                ///    onay = Search_Engine_FormCode_JSON(tForm, TargetTableIPCode, prop_);
            }
            #endregion
           
            /// işi bitti
            v.search_VALUE = "";

            return onay;
        }

        /// ----- OLD
        /// 
        
        private bool Search_Engine_TableIPCode(Form tForm, string myPropSearch, string TargetTableIPCode)
        {
            tToolBox t = new tToolBox();
            tInputPanel ip = new tInputPanel();

            Boolean snc = false;
            string SearchTableIPCode = t.MyProperties_Get(myPropSearch, "TABLEIPCODE:");
            string Caption = t.MyProperties_Get(myPropSearch, "CAPTION:");
            string GetFieldList = t.Find_Properies_Get_FieldBlock(myPropSearch, "GET_FIELD_LIST");
            string FormWidth = t.MyProperties_Get(myPropSearch, "FORM_WIDTH:");
            string FormHeight = t.MyProperties_Get(myPropSearch, "FORM_HEIGHT:");
            
            #region Create tSearchForm 

            int width = 0;
            int height = 0;
            if (FormWidth != "MAX") width = t.myInt32(FormWidth);
            if (FormHeight != "MAX") height = t.myInt32(FormHeight);

            if (width == 0) width = 800;
            if (height == 0) height = 600;

            ///
            /// New Form
            ///
            tEvents ev = new tEvents();
            XtraForm tSearchForm = new XtraForm();
            
            tSearchForm.Size = new Size(width, height);
            tFormEventsAdd(tSearchForm);

            ///
            /// groupControl
            ///
            DevExpress.XtraEditors.GroupControl groupControl = new GroupControl();
            groupControl.Text = Caption;
            groupControl.Dock = DockStyle.Fill;

            if (t.IsNotNull(SearchTableIPCode))
                ip.Create_InputPanel(tSearchForm, groupControl, SearchTableIPCode, 1);

            tSearchForm.Controls.Add(groupControl);

            /// Open SearchForm
            /// 
            t.DialogForm_View(tSearchForm, FormWindowState.Normal);

            Search_Engine_SetValues(tForm, GetFieldList, TargetTableIPCode);

            #endregion Create tSearchForm 


            return snc;
        }

        private bool Search_Engine_FormCode(Form tForm, string myPropSearch, string TargetTableIPCode)
        {
            tToolBox t = new tToolBox();
            //tInputPanel ip = new tInputPanel();

            Boolean snc = false;
            string SearchFormCode = t.MyProperties_Get(myPropSearch, "FORMCODE:");
            string Caption = t.MyProperties_Get(myPropSearch, "CAPTION:");
            string GetFieldList = t.Find_Properies_Get_FieldBlock(myPropSearch, "GET_FIELD_LIST");
            string FormWidth = t.MyProperties_Get(myPropSearch, "FORM_WIDTH:");
            string FormHeight = t.MyProperties_Get(myPropSearch, "FORM_HEIGHT:");

            int width = 0;
            int height = 0;
            if (FormWidth != "MAX") width = t.myInt32(FormWidth);
            if (FormHeight != "MAX") height = t.myInt32(FormHeight);

            if (width == 0) width = 800;
            if (height == 0) height = 600;

            ///
            /// New Form
            ///
            tEvents ev = new tEvents();

            XtraForm tSearchForm = new XtraForm();
            
            ev.tFormEventsAdd(tSearchForm);
            tSearchForm.Size = new Size(width, height);

            string Prop_Navigator = @"
            0=FORMNAME:null;
            0=FORMCODE:" + SearchFormCode + @";
            0=FORMTYPE:DIALOG;
            0=FORMSTATE:NORMAL;
            ";

            t.OpenForm(tSearchForm, Prop_Navigator);

            Search_Engine_SetValues(tForm, GetFieldList, TargetTableIPCode);

            return snc;
        }

        private void Search_Engine_SetValues(Form tForm, string GetFieldList, string TargetTableIPCode)
        {
            tToolBox t = new tToolBox();

            /// Seçilen DataRow 
            ///
            if (v.con_DataRow != null)
            {
                #region
                //1=ROW_GET_FIELD_LIST:1;
                //1=CAPTION:takipTuru;
                //1=T_FNAME:takipTuru;
                //1=R_FNAME:SYS_TURU_ID;
                //1=MSETVALUE:null;
                //1=ROWE_GET_FIELD_LIST:1;
                //2=ROW_GET_FIELD_LIST:2;
                //2=CAPTION:takipYolu;
                //2=T_FNAME:takipYolu;
                //2=R_FNAME:null;
                //2=MSETVALUE:null;
                //2=ROWE_GET_FIELD_LIST:2;
                //3=ROW_GET_FIELD_LIST:3;
                //3=CAPTION:takipSekli;
                //3=T_FNAME:takipSekli;
                //3=R_FNAME:null;
                //3=MSETVALUE:null;
                //3=ROWE_GET_FIELD_LIST:3;
                #endregion

                #region GetFieldList işlemleri
                /// Seçilen Row un değerleri okunur ve bekleyen dsData ya set edilir
                /// 
                string s = GetFieldList;
                if (s.IndexOf("GET_FIELD_LIST") > -1)
                {
                    /*
                    GET_FIELD_LIST={
                    1=ROW_GET_FIELD_LIST:1;
                    1=CAPTION:G32;
                    1=T_FNAME:GYLPH_32;
                    1=R_FNAME:LKP_FULL_NAME;
                    1=MSETVALUE:null;
                    1=ROWE_GET_FIELD_LIST:1;
                    GET_FIELD_LIST=};
                    */
                    string T_FNAME = string.Empty;
                    string R_FNAME = string.Empty;
                    string MSETVALUE = string.Empty;

                    string row_block = string.Empty;
                    string lockE = "=ROWE_";
                    bool editing = false;

                    DataSet dsData = null;
                    DataNavigator tDataNavigator = null;

                    t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TargetTableIPCode);//, function_name);

                    while (s.IndexOf(lockE) > -1)
                    {
                        row_block = t.Find_Properies_Get_RowBlock(ref s, "GET_FIELD_LIST");

                        T_FNAME = t.MyProperties_Get(row_block, "T_FNAME:");
                        R_FNAME = t.MyProperties_Get(row_block, "R_FNAME:");
                        MSETVALUE = t.MyProperties_Get(row_block, "MSETVALUE:");

                        try
                        {
                            dsData.Tables[0].Rows[tDataNavigator.Position][T_FNAME] = v.con_DataRow[R_FNAME];
                            editing = true;
                        }
                        catch (Exception)
                        {
                            //
                            //throw;
                        }

                    }//while ( 

                    /// yukarıdaki atamalar tetiklensin diye  (gerek yok gibi / dataLoyout da kontrol edilecek)
                    /// ekran refresh olsun diye aşağıdaki işlem yapılıyor
                    if (editing)
                    {
                        // atama önce kabul ediliyor
                        dsData.Tables[0].AcceptChanges();

                        // tekrar editlemek için en son yapılan atama tekrar yapılıyor
                        try
                        {
                            dsData.Tables[0].Rows[tDataNavigator.Position][T_FNAME] = v.con_DataRow[R_FNAME];
                        }
                        catch (Exception)
                        {
                            //
                            //throw;
                        }
                    }

                    /// atanan veriler anında viewcontrol üzerinde görünsün
                    ///
                    #region 
                    Control viewcntrl = t.Find_Control_View(tForm, TargetTableIPCode);

                    if (viewcntrl != null)
                    {
                        if (viewcntrl.ToString() == "DevExpress.XtraGrid.GridControl")
                        {
                            if (((DevExpress.XtraGrid.GridControl)viewcntrl).MainView.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
                            {
                                GridView view = ((DevExpress.XtraGrid.GridControl)viewcntrl).MainView as GridView;
                                view.SetFocusedValue(view.FocusedValue.ToString());

                                /// satır / row değiştirme işlemi yapıyor ileride gerekebilir
                                ///if (view.IsLastRow)
                                ///    view.MovePrev();
                                ///else view.MoveNext();
                            }

                            if (((DevExpress.XtraGrid.GridControl)viewcntrl).MainView.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
                            {
                                AdvBandedGridView view = ((DevExpress.XtraGrid.GridControl)viewcntrl).MainView as AdvBandedGridView;
                                view.SetFocusedValue(view.FocusedValue.ToString());
                            }
                        }
                        if (viewcntrl.ToString() == "DevExpress.XtraTreeList.TreeList")
                        {
                            //myObject = ((DevExpress.XtraTreeList.TreeList)viewcntrl).Parent;
                        }
                        if (viewcntrl.ToString() == "DevExpress.XtraDataLayout.DataLayoutControl")
                        {
                            //myObject = ((DevExpress.XtraDataLayout.DataLayoutControl)viewcntrl).Parent;
                        }
                        if (viewcntrl.ToString() == "DevExpress.XtraVerticalGrid.VGridControl")
                        {
                            //myObject = ((DevExpress.XtraVerticalGrid.VGridControl)viewcntrl).Parent;
                        }
                    }
                #endregion

                } // if (s
                #endregion GetFieldList işlemleri
            }

        }

        /// ----- JSONs
        /// 
        
        /// New    
        private bool Search_Engine_TableIPCode_JSON_2(Form tForm, PROP_NAVIGATOR prop_, string TargetTableIPCode)
        {
            /// new search engine tableIPCode

            tToolBox t = new tToolBox();
            tInputPanel ip = new tInputPanel();
            tEvents ev = new tEvents();

            bool onay = false;
            string Caption = prop_.CAPTION.ToString();
            string SearchTableIPCode = prop_.TARGET_TABLEIPCODE.ToString();
            string FormWidth = prop_.FORM_WIDTH.ToString();
            string FormHeight = prop_.FORM_HEIGHT.ToString();
            
            /// Create tSearchForm
            /// 
            #region Create tSearchForm 

            int width = 0;
            int height = 0;
            if (FormWidth != "MAX") width = t.myInt32(FormWidth);
            if (FormHeight != "MAX") height = t.myInt32(FormHeight);

            if (width == 0) width = 800;
            if (height == 0) height = 600;

            //
            // New Form
            //
            Form tSearchForm = new Form();
            tSearchForm.Size = new Size(width, height);
            tSearchForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            tSearchForm.Name = "tSearchForm";
            ev.tFormEventsAdd(tSearchForm);

            //
            // tabPane1
            //
            DevExpress.XtraBars.Navigation.TabPane tabPane1 = new DevExpress.XtraBars.Navigation.TabPane();
            ((System.ComponentModel.ISupportInitialize)(tabPane1)).BeginInit();
                        
            tSearchForm.Controls.Add(tabPane1);
                        
            //
            // tabPane1
            // 
            tabPane1.BringToFront();
            tabPane1.Location = new System.Drawing.Point(0, 0);
            tabPane1.Name = v.lyt_Name + "_Search";
            tabPane1.RegularSize = new System.Drawing.Size(v.Screen_Width, v.Screen_Height);
            tabPane1.SelectedPageIndex = 0;
            tabPane1.Size = new System.Drawing.Size(v.Screen_Width, v.Screen_Height);
            tabPane1.TabIndex = 0;
            tabPane1.Text = v.lyt_Name + "_Search";
            tabPane1.Dock = System.Windows.Forms.DockStyle.Fill;

            //
            // tabNavigationPage1
            //
            DevExpress.XtraBars.Navigation.TabNavigationPage tabNavigationPage1 = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            tabNavigationPage1.Name = v.lyt_Name + "_Search1";
            tabNavigationPage1.Caption = Caption + "   ";
            
            //
            // InputPanel Create
            //
            if (t.IsNotNull(SearchTableIPCode))
                ip.Create_InputPanel(tSearchForm, tabNavigationPage1, SearchTableIPCode, 1);
                        
            tabPane1.Controls.Add(tabNavigationPage1);
            tabPane1.Pages.Add(tabNavigationPage1);

            ((System.ComponentModel.ISupportInitialize)(tabPane1)).EndInit();
            tabPane1.ResumeLayout(false);

            tSearchForm.Controls.Add(tabPane1);

            /*
            /// deneme
            /// groupControl
            ///
            DevExpress.XtraEditors.GroupControl groupControl = new GroupControl();
            groupControl.Text = Caption;
            groupControl.Dock = DockStyle.Fill;

            if (t.IsNotNull(SearchTableIPCode))
                ip.Create_InputPanel(tSearchForm, groupControl, SearchTableIPCode, 1);
            
            tSearchForm.Controls.Add(groupControl);

            /// deneme---------------
            */
            #endregion Create tSearchForm 

            /// Old Value Set
            /// 
            if (t.IsNotNull(v.search_VALUE))
            {
                Search_Engine_FindValues_JSON(
                    tForm, TargetTableIPCode,
                    null, prop_.TABLEIPCODE_LIST,
                    tSearchForm, SearchTableIPCode);
            }
            
            /// Open SearchForm
            /// 
            t.DialogForm_View(tSearchForm, FormWindowState.Normal);

            /// New Value Set
            /// 
            if (v.searchSet)
                onay = Search_Engine_SetValues_JSON(tForm, TargetTableIPCode, null, prop_.TABLEIPCODE_LIST);
            else onay = false;

            v.searchSet = false;

            return onay;
        }
        /// Old
        private bool Search_Engine_TableIPCode_JSON(Form tForm, PROP_SEARCH prop_, string TargetTableIPCode)
        {
            tToolBox t = new tToolBox();
            tInputPanel ip = new tInputPanel();

            Boolean snc = false;
            string Caption = prop_.CAPTION.ToString();
            string SearchTableIPCode = prop_.TABLEIPCODE.ToString();
            string FormWidth = prop_.FORM_WIDTH.ToString();
            string FormHeight = prop_.FORM_HEIGHT.ToString();
            
            #region Create tSearchForm 

            int width = 0;
            int height = 0;
            if (FormWidth != "MAX") width = t.myInt32(FormWidth);
            if (FormHeight != "MAX") height = t.myInt32(FormHeight);

            if (width == 0) width = 800;
            if (height == 0) height = 600;

            ///
            /// New Form
            ///
            tEvents ev = new tEvents();

            XtraForm tSearchForm = new XtraForm();

            ev.tFormEventsAdd(tSearchForm);
            tSearchForm.Size = new Size(width, height);
            tSearchForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;

            ///
            /// groupControl
            ///
            DevExpress.XtraEditors.GroupControl groupControl = new GroupControl();
            groupControl.Text = Caption;
            groupControl.Dock = DockStyle.Fill;

            if (t.IsNotNull(SearchTableIPCode))
                ip.Create_InputPanel(tSearchForm, groupControl, SearchTableIPCode, 1);

            tSearchForm.Controls.Add(groupControl);

            if (t.IsNotNull(v.search_VALUE))
            {
                Search_Engine_FindValues_JSON(
                    tForm, TargetTableIPCode, 
                    prop_.GET_FIELD_LIST, null,
                    tSearchForm, SearchTableIPCode);
            }
            
            /// Open SearchForm
            /// 
            t.DialogForm_View(tSearchForm, FormWindowState.Normal);

            Search_Engine_SetValues_JSON(tForm, TargetTableIPCode, prop_.GET_FIELD_LIST, null);

            #endregion Create tSearchForm 
            
            return snc;
        }
        
        /// Old  idi şimdi  New oldu
        private bool Search_Engine_FormCode_JSON(Form tForm, PROP_NAVIGATOR prop_, string TargetTableIPCode)
        {
            tToolBox t = new tToolBox();
            //tInputPanel ip = new tInputPanel();

            bool onay = false;
            string Caption = prop_.CAPTION.ToString();
            string FormWidth = prop_.FORM_WIDTH.ToString();
            string FormHeight = prop_.FORM_HEIGHT.ToString();
            string SearchFormCode = prop_.FORMCODE.ToString();

            int width = 0;
            int height = 0;
            if (FormWidth != "MAX") width = t.myInt32(FormWidth);
            if (FormHeight != "MAX") height = t.myInt32(FormHeight);

            if (width == 0) width = 800;
            if (height == 0) height = 600;

            /// New Form
            ///
            tEvents ev = new tEvents();

            Form tSearchForm = new Form();
            
            ev.tFormEventsAdd(tSearchForm);
            tSearchForm.Size = new Size(width, height);
            tSearchForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            
            string Prop_Navigator = @"
            0=FORMNAME:null;
            0=FORMCODE:" + SearchFormCode + @";
            0=FORMTYPE:DIALOG;
            0=FORMSTATE:NORMAL;
            ";

            //// ne yapacağımı bilemedim
            ////if (t.IsNotNull(v.search_VALUE))
            ////{
            ////    Search_Engine_FindValues_JSON(
            ////        tForm, TargetTableIPCode, prop_.GET_FIELD_LIST,
            ////        tSearchForm, SearchTableIPCode);
            ////}

            t.OpenForm(tSearchForm, Prop_Navigator);

            /// New Value Set
            /// 
            if (v.searchSet)
                onay = Search_Engine_SetValues_JSON(tForm, TargetTableIPCode, null, prop_.TABLEIPCODE_LIST);
            else onay = false;

            v.searchSet = false;

            return onay;
        }

        private void Search_Engine_FindValues_JSON(
            Form tForm, 
            string TargetTableIPCode, 
            List<GET_FIELD_LIST> GetFieldList,
            List<TABLEIPCODE_LIST> TableIPCodeList,
            Form tSearchForm, 
            string SearchTableIPCode)
        {
            /// Amaç : önceden seçilmiş bir kaydın 
            /// arama listesi üzerinde, gidip yeniden set focus olmasını sağlamak
                        
            tToolBox t = new tToolBox();

            string s = string.Empty;
            string T_FNAME = string.Empty;
            string R_FNAME = string.Empty;
            //string MSETVALUE = string.Empty;
                        
            DataSet dsData = null;
            DataNavigator tDataNavigator = null;
            t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TargetTableIPCode);

            if (t.IsNotNull(dsData))
            {
                /// old search value set
                if (GetFieldList != null)
                { 
                    foreach (var item in GetFieldList)
                    {
                        T_FNAME = item.T_FNAME.ToString();
                        R_FNAME = item.R_FNAME.ToString();
                        try
                        {
                            item.MSETVALUE = dsData.Tables[0].Rows[tDataNavigator.Position][T_FNAME].ToString();
                            //s = s + item.T_FNAME.ToString() + "//" + item.R_FNAME.ToString() + "//" + item.MSETVALUE + v.ENTER;
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                    }
                }

                /// new search value set
                if (TableIPCodeList != null)
                {
                    
                    ///{
                    ///  "CAPTION": "ID > Cari ID",
                    ///  "TABLEIPCODE": "null",
                    ///  "TABLEALIAS": "null",
                    ///  "KEYFNAME": "CARI_ID",
                    ///  "RTABLEIPCODE": "null",
                    ///  "RKEYFNAME": "ID",
                    ///  "MSETVALUE": "null",
                    ///  "WORKTYPE": "SETDATA",
                    ///  "CONTROLNAME": "null",
                    ///  "DCCODE": "null"
                    ///},
                    
                    foreach (var item in TableIPCodeList)
                    {
                        T_FNAME = item.KEYFNAME.ToString();
                        R_FNAME = item.RKEYFNAME.ToString();   
                        try
                        {
                            item.MSETVALUE = dsData.Tables[0].Rows[tDataNavigator.Position][T_FNAME].ToString();
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                    }
                }

            }
            
            /// gelen Value ye uygun kayıtları arama motorunda listele
            /// 
            InData_RunSQL(tSearchForm, SearchTableIPCode, v.search_VALUE, GetFieldList, TableIPCodeList);
                        
        }
        
        private bool Search_Engine_SetValues_JSON(Form tForm, string TargetTableIPCode, 
            List<GET_FIELD_LIST> GetFieldList, 
            List<TABLEIPCODE_LIST> TableIPCodeList)
        {
            /// Seçilen DataRow 
            ///
            bool onay = false;

            if (v.con_DataRow != null)
            {
                tToolBox t = new tToolBox();

                #region SetValues işlemleri
                /// Seçilen Row un değerleri okunur ve bekleyen dsData ya set edilir

                string uT_FNAME = string.Empty;
                string uR_FNAME = string.Empty;

                string T_FNAME = string.Empty;
                string R_FNAME = string.Empty;
                string MSETVALUE = string.Empty;
                string WORKTYPE = string.Empty;
                bool editing = false;

                DataSet dsData = null;
                DataNavigator tDataNavigator = null;
                t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TargetTableIPCode);

                #region GetFieldList
                if (GetFieldList != null)
                {
                    foreach (var item in GetFieldList)
                    {
                        T_FNAME = item.T_FNAME.ToString();
                        R_FNAME = item.R_FNAME.ToString();
                        MSETVALUE = item.MSETVALUE.ToString();

                        try
                        {
                            dsData.Tables[0].Rows[tDataNavigator.Position][T_FNAME] = v.con_DataRow[R_FNAME];
                            editing = true;
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("DİKKAT : Hatalı alan isimleri : " + v.ENTER + 
                                "Target FieldName : " + T_FNAME + v.ENTER +
                                "Source FieldName : " + R_FNAME );
                            //throw;
                        }
                    }
                }
                #endregion

                #region TableIPCodeList
                if (TableIPCodeList != null)
                {
                    foreach (var item in TableIPCodeList)
                    {
                        T_FNAME = item.KEYFNAME.ToString();
                        R_FNAME = item.RKEYFNAME.ToString();
                        MSETVALUE = item.MSETVALUE.ToString();
                        WORKTYPE = item.WORKTYPE.ToString();

                        if (WORKTYPE == "SETDATA")
                        {
                            try
                            {
                                dsData.Tables[0].Rows[tDataNavigator.Position][T_FNAME] = v.con_DataRow[R_FNAME];
                                uT_FNAME = T_FNAME;
                                uR_FNAME = R_FNAME;
                                editing = true;
                            }
                            catch (Exception)
                            {
                                //
                                //throw;
                            }
                        }

                        if (WORKTYPE == "IBOX")
                        {

                        }
                    }
                }
                #endregion

                /// yukarıdaki atamalar tetiklensin diye  (gerek yok gibi / dataLayout da kontrol edilecek)
                /// ekran refresh olsun diye aşağıdaki işlem yapılıyor
                #region
                if (editing)
                {
                    // atama önce kabul ediliyor
                    dsData.Tables[0].AcceptChanges();
                    // tekrar Editlemek için en son yapılan atama tekrar yapılıyor
                    try
                    {
                        // 
                        dsData.Tables[0].Rows[tDataNavigator.Position][uT_FNAME] = v.con_DataRow[uR_FNAME];
                        //
                        onay = true;
                    }
                    catch (Exception)
                    {
                        //
                        onay = false;
                    }
                }
                #endregion

                /// atanan veriler anında viewcontrol üzerinde görünsün
                ///
                viewControl_FocusedValue(tForm, TargetTableIPCode);

                #endregion SetValues işlemleri

            }

            return onay;
        }

        private void viewControl_FocusedValue(Form tForm, string TableIPCode)
        {
            #region 
            tToolBox t = new tToolBox();

            Control viewcntrl = t.Find_Control_View(tForm, TableIPCode);

            if (viewcntrl != null)
            {
                if (viewcntrl.ToString() == "DevExpress.XtraGrid.GridControl")
                {
                    if (((DevExpress.XtraGrid.GridControl)viewcntrl).MainView.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
                    {
                        GridView view = ((DevExpress.XtraGrid.GridControl)viewcntrl).MainView as GridView;
                        view.SetFocusedValue(view.FocusedValue.ToString());

                        /// satır / row değiştirme işlemi yapıyor ileride gerekebilir
                        ///if (view.IsLastRow)
                        ///    view.MovePrev();
                        ///else view.MoveNext();
                    }
                    if (((DevExpress.XtraGrid.GridControl)viewcntrl).MainView.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
                    {
                        AdvBandedGridView view = ((DevExpress.XtraGrid.GridControl)viewcntrl).MainView as AdvBandedGridView;
                        view.SetFocusedValue(view.FocusedValue.ToString());
                    }
                }
                if (viewcntrl.ToString() == "DevExpress.XtraTreeList.TreeList")
                {
                    //myObject = ((DevExpress.XtraTreeList.TreeList)viewcntrl).Parent;
                }
                if (viewcntrl.ToString() == "DevExpress.XtraDataLayout.DataLayoutControl")
                {
                    //myObject = ((DevExpress.XtraDataLayout.DataLayoutControl)viewcntrl).Parent;
                }
                if (viewcntrl.ToString() == "DevExpress.XtraVerticalGrid.VGridControl")
                {
                    //myObject = ((DevExpress.XtraVerticalGrid.VGridControl)viewcntrl).Parent;
                }
            }
            
            #endregion 
        }

        #endregion Search Engine / Arama Motoru

        #region AddIP_TableIPCode Run
        public void AddIP_TableIPCode(Form tForm)
        {
            tToolBox t = new tToolBox();

            string AddIP_Form = string.Empty;
            string AddIP_TableIPCode = string.Empty;
            string AddIP_TableCode = string.Empty;
            string AddIP_FieldName = string.Empty;
            string AddIP_Value = string.Empty;
            string AddListIP_TableIPCode = string.Empty;
            string Parametre = string.Empty;

            string form_prp = t.Set(t.myFormBox_Values(tForm, v.FormLoad),  // Formun üzerindeki MemoEdit ten alınıyor
                                     v.con_FormLoadValue,                    // Gecici Hafızaya aktarılan bilgi
                                     string.Empty);

            AddIP_Form = t.MyProperties_Get(form_prp, "AddIP_Form:");
            // Konu olan bilgisi ( GRUP bilgisi )
            AddIP_TableIPCode = t.MyProperties_Get(form_prp, "AddIP_TableIPCode:");
            AddIP_TableCode = t.MyProperties_Get(form_prp, "AddIP_TableCode:");
            AddIP_FieldName = t.MyProperties_Get(form_prp, "AddIP_FieldName:");
            AddIP_Value = t.Set(t.MyProperties_Get(form_prp, "AddIP_Value:"), "", "0");
            // Konu olana ait alt liste ( Konu olan = GRUP, Alt Liste = Gruba Kayıtlı Öğrenci Listesi )
            AddListIP_TableIPCode = t.MyProperties_Get(form_prp, "AddListIP_TableIPCode:");

            Parametre = t.Set(t.MyProperties_Get(form_prp, "Parametre:"), "", "TRUE");

            // bu özellik şimdilik sadece MSReportView için eklendi
            if (AddIP_Form == "MSREPORTVIEW")
            {
                Control cntrl = null;
                string[] controls = new string[] { };

                cntrl = t.Find_Control(tForm, "groupControl_Konu_Olan", "", controls);

                if ((cntrl != null) && (t.IsNotNull(AddIP_TableIPCode)))
                {
                    tInputPanel ip = new tInputPanel();

                    ((DevExpress.XtraEditors.GroupControl)cntrl).Visible = true;
                    ip.Create_InputPanel(tForm, ((DevExpress.XtraEditors.GroupControl)cntrl), AddIP_TableIPCode, 1);

                    v.con_AddIP_TableCode = AddIP_TableCode;
                    v.con_AddIP_FieldName = AddIP_FieldName;
                    v.con_AddIP_Value = AddIP_Value;
                    v.con_AddListIP_TableIPCode = AddListIP_TableIPCode;
                }

                if (Parametre.ToUpper() == "FALSE")
                    v.con_Parametre = false;
                else v.con_Parametre = true;

                if (t.IsNotNull(AddListIP_TableIPCode))
                {
                    // her durumuda gösterilsin
                    v.con_Parametre = true;
                    tSubView_Preparing(tForm, "dockPanel", "dockPanel_List", AddListIP_TableIPCode, "", "", "");
                }
            }

        }
        #endregion AddIP_TableIPCode Run

        #region AutoNewRecords Tables Run
        public void AutoNewRecords(Form tForm)
        {

        }
        #endregion AutoNewRecords Tables Run

        #region Preparing_OnOff

        private void Preparing_OnOff(ref string Sql, string FieldName, string Value)
        {
            tToolBox t = new tToolBox();

            // / *TEORIK_SINAV_TARIHI.ON* /
            // and convert(smalldatetime, ISNULL(K.TEORIK_SINAV_TARIHI,"01.01.1900"), 103) <= TR.SINAV_TARIHI
            // / *TEORIK_SINAV_TARIHI.OFF* /

            string str_bgn = "/*" + FieldName + ".ON*/";
            string str_end = "/*" + FieldName + ".OFF*/";
            int i_bgn = Sql.IndexOf(str_bgn) + str_bgn.Count() + 2;
            int i_end = Sql.IndexOf(str_end);

            if ((i_bgn > -1) && (i_end > -1))
            {
                string old_and = Sql.Substring(i_bgn, i_end - i_bgn);
                string new_and = old_and;

                if (old_and.IndexOf("-- and") > -1)
                {
                    t.Str_Replace(ref new_and, "-- and", "and");
                }
                else
                {
                    t.Str_Replace(ref new_and, "and", "-- and");
                }

                t.Str_Replace(ref Sql, old_and, new_and);
            }
            //MessageBox.Show("||" + old_and + "||" + new_and+"||");
        }

        #endregion Preparing_OnOff

        #region Prop_Navigator örnek kod

        /*
            PROP_NAVIGATOR={
            1=ROW_PROP_NAVIGATOR:1;
            1=CAPTION:aaaaa;
   >>       1=BUTTONTYPE:51;
            1=TABLEIPCODE_LIST:TABLEIPCODE_LIST={
            1=ROW_TABLEIPCODE_LIST:1;
            1=CAPTION:qqqqq;
            1=TABLEIPCODE:null;
            1=TABLEALIAS:null;
            1=KEYFNAME:null;
            1=RTABLEIPCODE:null;
            1=RKEYFNAME:null;
            1=MSETVALUE:null;
            1=WORKTYPE:null;
            1=ROWE_TABLEIPCODE_LIST:1;
            TABLEIPCODE_LIST=};
            1=FORMNAME:null;
            1=FORMCODE:null;
            1=FORMTYPE:null;
            1=FORMSTATE:null;
            1=ROWE_PROP_NAVIGATOR:1;
         * 
            2=ROW_PROP_NAVIGATOR:2;
            2=CAPTION:bbbbb;
   >>       2=BUTTONTYPE:52;
            2=TABLEIPCODE_LIST:TABLEIPCODE_LIST={
            1=ROW_TABLEIPCODE_LIST:1;
            1=CAPTION:qqqqq;
            1=TABLEIPCODE:null;
            1=TABLEALIAS:null;
            1=KEYFNAME:null;
            1=RTABLEIPCODE:null;
            1=RKEYFNAME:null;
            1=MSETVALUE:null;
            1=WORKTYPE:null;
            1=ROWE_TABLEIPCODE_LIST:1;
            2=ROW_TABLEIPCODE_LIST:2;
            2=CAPTION:wwwww;
            2=TABLEIPCODE:shssddsg;
            2=TABLEALIAS:sdfsadf;
            2=KEYFNAME:sdfsadf;
            2=RTABLEIPCODE:sdfasdfdf;
            2=RKEYFNAME:sdfdfsg;
            2=MSETVALUE:234;
            2=WORKTYPE:READ;
            2=ROWE_TABLEIPCODE_LIST:2;
            TABLEIPCODE_LIST=};
            2=FORMNAME:null;
            2=FORMCODE:null;
            2=FORMTYPE:null;
            2=FORMSTATE:null;
            2=ROWE_PROP_NAVIGATOR:2;
         *          
            3=ROW_PROP_NAVIGATOR:3;
            3=CAPTION:ccccccc;
     >>     3=BUTTONTYPE:56;
            3=TABLEIPCODE_LIST:null;
            3=FORMNAME:null;
            3=FORMCODE:null;
            3=FORMTYPE:null;
            3=FORMSTATE:null;
            3=ROWE_PROP_NAVIGATOR:3;
            PROP_NAVIGATOR=}
            */

        #endregion Prop_Navigator

        #region Find_ParentValue
        public string Find_ParentValue(Form tForm, string TableIPCode,
            string Key_FName, string Parent_FName, string hesapName)
        {
            tToolBox t = new tToolBox();
            string function_name = "tParentValue";

            /// yeni_hesap
            /// yeni_alt_hesap

            if ((t.IsNotNull(Key_FName) == false) &&
                (hesapName == "yeni_hesap"))
            {
                MessageBox.Show("DİKKAT : Alt hesap oluşturmak için gerekli olan tablonun KEY FIELDNAME belli değil...", function_name);
                return null;
            }
            if ((t.IsNotNull(Parent_FName) == false) &&
                (hesapName == "yeni_alt_hesap"))
            {
                MessageBox.Show("DİKKAT : Alt hesap oluşturmak için gerekli olan tablonun PARENT FIELDNAME belli değil...", function_name);
                return null;
            }

            DataSet ds = null;
            DataNavigator dN = null;
            string find_parentvalue = string.Empty;

            t.Find_DataSet(tForm, ref ds, ref dN, TableIPCode);

            if (t.IsNotNull(ds))
            {
                if ((dN.Position > -1) && (hesapName == "yeni_hesap"))
                    find_parentvalue = ds.Tables[0].Rows[dN.Position][Parent_FName].ToString();
                if ((dN.Position > -1) && (hesapName == "yeni_alt_hesap"))
                    find_parentvalue = ds.Tables[0].Rows[dN.Position][Key_FName].ToString();
            }

            return find_parentvalue;
        }
        #endregion Find_ParentValue

        #region Expression - Column üzerindeki formüller hesaplanıyor

        private class vExpression
        {
            private Form _tForm = null;
            private Control _cntrl = null;
            private DataSet _dsData = null;
            private int _pos = 0;
            private string _exp_type = string.Empty;
            private string _exp_value = string.Empty;
            private string _focus_field = string.Empty;
            private string _exp_formul_fname = string.Empty;
            private string _extra_fname = string.Empty;
            private string _send_FieldName = string.Empty;
            private string _send_Value = string.Empty;
            private string _pa = string.Empty;
            private string _pk = string.Empty;

            public Form tForm
            {
                get { return _tForm; }
                set { _tForm = value; }
            }
            public Control cntrl
            {
                get { return _cntrl; }
                set { _cntrl = value; }
            }
            public DataSet dsData
            {
                get { return _dsData; }
                set { _dsData = value; }
            }
            public int pos
            {
                get { return _pos; }
                set { _pos = value; }
            }
            public string exp_type
            {
                get { return _exp_type; }
                set { _exp_type = value; }
            }
            public string exp_value
            {
                get { return _exp_value; }
                set { _exp_value = value; }
            }
            public string focus_field
            {
                get { return _focus_field; }
                set { _focus_field = value; }
            }
            public string exp_formul_fname
            {
                get { return _exp_formul_fname; }
                set { _exp_formul_fname = value; }
            }
            public string extra_fname
            {
                get { return _extra_fname; }
                set { _extra_fname = value; }
            }
            public string send_FieldName
            {
                get { return _send_FieldName; }
                set { _send_FieldName = value; }
            }
            public string send_Value
            {
                get { return _send_Value; }
                set { _send_Value = value; }
            }
            public string pa
            {
                get { return _pa; }
                set { _pa = value; }
            }
            public string pk
            {
                get { return _pk; }
                set { _pk = value; }
            }
        }

        public void Preparing_Expression(Form tForm, DataSet dsData, int pos,
                              string TableIPCode, string send_FieldName, string send_Value)
        {
            // EXPRESSION = formül
            // send_FieldName = şu anda veri girişi yapılan field yani change olan
            // send_Value     = şu anda girilen veri 
            if ((pos == -1) || (v.con_Expression == false)) return;

            if ((send_FieldName != "") && (send_Value == "")) send_Value = "0";

            tToolBox t = new tToolBox();
                        
            if (t.Find_TableFields(dsData, null)) 
            {
                string tname1 = dsData.Tables[1].TableName;
                string tname2 = dsData.Tables[2].TableName;

                int i4 = dsData.Tables[tname1].Rows.Count;
                
                string prop_Expression = string.Empty;
                string exp_formul_fname = string.Empty;
                
                // DataLayoutControl ise
                #region yapılan değişikliği view üzerine işle

                Control cntrl = null;
                cntrl = t.Find_Control_View(tForm, TableIPCode);

                #endregion

                string s2 = (char)34 + "EXP_TYPE" + (char)34 + ":";
                /// EXP_TYPE":

                /// formül iki şekilde olabilir
                /// birincisi  [

                vExpression vExp = new vExpression();

                vExp.tForm = tForm;
                vExp.cntrl = cntrl;
                vExp.dsData = dsData;
                vExp.pos = pos;
                vExp.send_FieldName = send_FieldName;
                vExp.send_Value = send_Value;
                
                List<PROP_EXPRESSION> packet = new List<PROP_EXPRESSION>();

                #region // tablonu fieldleri
                for (int i = 0; i < i4; i++)
                {
                    /// EXP_TYPE 
                    /// COMP = Compute
                    /// SETDATA = Set Data
                    vExp.exp_type = "";
                    /// "formül gelecek";
                    vExp.exp_value = "";
                    
                    // formülün sahibi olan field
                    exp_formul_fname = dsData.Tables[tname1].Rows[i]["name"].ToString();

                    // formül okunuyor 
                    // birleşik hali böyleydi (xxx_fields )
                    //prop_Expression = dsData.Tables[tname2].Rows[i]["PROP_EXPRESSION"].ToString();

                    // yeni hali böyle ( xxx_fields ve xxx_fields2 )
                    prop_Expression = prop_Expression_Get(dsData, tname2, exp_formul_fname); 
                        //.Tables[tname2].Rows[i]["PROP_EXPRESSION"].ToString();
                    
                    vExp.exp_formul_fname = exp_formul_fname;
                    vExp.extra_fname = "";

                    // sonuc değişkenini boşaltalım
                    //value = "";

                    if (t.IsNotNull(prop_Expression))
                        v.con_Expression_View = v.con_Expression_View + 
                            "  " + exp_formul_fname + " : field " + v.ENTER;

                    // json format varsa
                    if (prop_Expression.IndexOf(s2) > -1)
                    {
                        //
                        //if (vExp.exp_formul_fname.ToString() == "TEVKIFAT_PAY")
                        //if (vExp.exp_formul_fname.ToString() == "ISKONTO_ORANI")
                        //{
                        //    // maksat durdurmak
                        //    v.Kullaniciya_Mesaj_Var = vExp.exp_formul_fname.ToString();
                        //}

                        // prop_Expression : eğer birden fazla ( JSON ) yorum ve hesap varsa 
                        // sırayla işlem yapması için Exp_Preparinge git ve orada her json satırı için teker teker hesapla
                        vExp.pa = "<";
                        vExp.pk = ">";
                        vExp.exp_value = ""; 
                        Exp_Preparing(t, vExp, packet, prop_Expression);
                    }
                    else
                    {
                        // prop_Expression içinde sadece tek bir formul var ise direk buradan git çalış

                        // json değilde düz yazılmış formul ise
                        vExp.pa = "[";
                        vExp.pk = "]";
                        vExp.exp_value = prop_Expression;
                        vExp.focus_field = "FALSE"; // tek formul olduğu için
                        Exp_Set(t, vExp);
                    }

                    if (t.IsNotNull(prop_Expression))
                        v.con_Expression_View = v.con_Expression_View + v.ENTER + "-------------- " + v.ENTER;

                } // for 

                #endregion
            }

            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + " 3,";

            //tForm.Text = tForm.Text + ":>" + value + "<:";
        }
                
        
        private string Exp_Preparing(tToolBox t
            , vExpression vExp
            , List<PROP_EXPRESSION> packet
            , string prop_Expression )
        {
            prop_Expression = prop_Expression.Replace((char)34, (char)39);
            int i = prop_Expression.IndexOf("[");
            if ((i == -1) || (i > 10))
                prop_Expression = "[" + prop_Expression + "]";
            var prop_ = JsonConvert.DeserializeAnonymousType(prop_Expression, packet);

            string formul_sahibi = vExp.exp_formul_fname.ToString();
            //string caption = string.Empty;
            //string exp_Type = string.Empty;
            //string exp_Value = string.Empty;
            string extra_FName = string.Empty;
            //string focus_field = string.Empty;

            // FIRST
            string chc_IPCode = string.Empty;
            string chc_FName = string.Empty;
            string chc_Value = string.Empty;
            string chc_Operand = string.Empty;
            // SEC = SECOND
            string chc_IPCode_SEC = string.Empty;
            string chc_FName_SEC = string.Empty;
            string chc_Value_SEC = string.Empty;
            string chc_Operand_SEC = string.Empty;

            
            // gelen prop_ içinde birden fazla yorum veya formul olabilir
            //if (vExp.exp_formul_fname.ToString() == "ISK1_ORANI")
            //{
            //    // maksat durdurmak
            //    v.Kullaniciya_Mesaj_Var = vExp.exp_formul_fname.ToString();
            //}


            foreach (PROP_EXPRESSION item in prop_)
            {
                //caption = item.CAPTION.ToString(); 
                //exp_Type = item.EXP_TYPE.ToString();
                //exp_Value = item.EXP_VALUE.ToString();
                //focus_field = item.FOCUS_FIELD.ToString();
                chc_IPCode = item.CHC_IPCODE.ToString();
                chc_FName = item.CHC_FNAME.ToString();
                chc_Value = item.CHC_VALUE.ToString();
                chc_Operand = item.CHC_OPERAND.ToString();
                chc_IPCode_SEC = item.CHC_IPCODE_SEC.ToString();
                chc_FName_SEC = item.CHC_FNAME_SEC.ToString();
                chc_Value_SEC = item.CHC_VALUE_SEC.ToString();
                chc_Operand_SEC = item.CHC_OPERAND_SEC.ToString();
                                
                if (Exp_Check(t, vExp.dsData, vExp.pos
                    , chc_FName
                    , chc_Value
                    , chc_Operand
                    , chc_FName_SEC
                    , chc_Value_SEC 
                    , chc_Operand_SEC 
                    ) == true)
                {
                    /// EXP_TYPE 
                    /// COMP = Compute
                    /// SETDATA = Set Data
                    vExp.exp_type = item.EXP_TYPE.ToString();
                    vExp.exp_value = item.EXP_VALUE.ToString(); // bir satırdaki formul;
                    vExp.extra_fname = item.EXTRA_FNAME.ToString();
                    vExp.focus_field = item.FOCUS_FIELD.ToString();

                    if (vExp.exp_value == "null")
                    {
                        MessageBox.Show("DİKKAT : " + formul_sahibi + " field\'in ( " + item.CAPTION.ToString() + " ) ait formül satır bulunamadı... ");
                    }

                    /// burdan itibaren bu döngüdeki formülü çalıştırmaya gidiyor 
                    /// birden fazla yorum ve hesap olabilir
                    Exp_Set(t, vExp);
                }
                                
            }

            return "";
        }

        private void Exp_Set(tToolBox t, vExpression vExp)
        {
            // bir satırdaki formul;
            if (t.IsNotNull(vExp.exp_value) == false) return;

            int p1 = 0;
            int p2 = 0;
            string s = "";
            string fname = "";
            string workFName = vExp.exp_formul_fname.ToString();
            string value = "";

            // eğer başka bir field için işlem yapılacak ise
            if (t.IsNotNull(vExp.extra_fname.ToString()))
                workFName = vExp.extra_fname.ToString();
            
            // formül bulundu ise 
            // yorum içinde workFName yi kullanma esas olarak vExp.exp_formul_fname üzerinde işlem yapılıyor
            if (
                (vExp.exp_value != "") &&
                ( (vExp.exp_formul_fname != vExp.send_FieldName) && (vExp.focus_field == "FALSE") ) ||
                ( (vExp.exp_formul_fname == vExp.send_FieldName) && (vExp.focus_field == "TRUE") )
               )
            {
                v.con_Expression_View = v.con_Expression_View +
                "  " + vExp.exp_type + " : " + workFName + " := " + vExp.exp_value;

                //if (workFName == "ISKONTO_ORANI")
                //{
                //    // maksat durdurmak
                //    v.Kullaniciya_Mesaj_Var = workFName;
                //}
                
                #region formul üzerindeki isimlerin yerine değerleriyle (value) değiştir
                //while (exp_value.IndexOf("<") > -1)
                while (vExp.exp_value.IndexOf(vExp.pa) > -1)
                {
                    //p1 = exp_value.IndexOf("<");
                    //p2 = exp_value.IndexOf(">");
                    p1 = vExp.exp_value.IndexOf(vExp.pa);
                    p2 = vExp.exp_value.IndexOf(vExp.pk);

                    if ((p1 > -1) && (p2 > -1))
                    {
                        fname = vExp.exp_value.Substring(p1 + 1, (p2 - p1) - 1);
                        if (vExp.pos > -1)
                        {
                            value = vExp.dsData.Tables[0].Rows[vExp.pos][fname].ToString();

                            // şuan girilen değeri al
                            if (fname == vExp.send_FieldName)
                                value = vExp.send_Value;
                            
                            if (value == "") value = "0";
                            if (value.ToUpper() == "TRUE") value = "1";
                            if (value.ToUpper() == "FALSE") value = "0";

                            //t.Str_Replace(ref exp_value, "<" + fname + ">", value);
                            s = vExp.exp_value;
                            t.Str_Replace(ref s, vExp.pa + fname + vExp.pk, value);
                            vExp.exp_value = s;
                        }
                    }
                }
                #endregion formül üzerindeki

                #region formülü hesapla ve ilgili fielde ata
                s = vExp.exp_value;
                t.Str_Replace(ref s, ",", ".");
                vExp.exp_value = s;

                #region COMP
                if ((vExp.exp_type == "") || (vExp.exp_type == "COMP"))
                {
                    value = JScriptEval(vExp.exp_value).ToString();
                    if ((value == "") || (value == "NaN")) value = "0";

                    v.con_Expression_View = v.con_Expression_View + " : " + vExp.exp_value + " := " + value + v.ENTER;
                }
                #endregion

                #region SETDATA
                if (vExp.exp_type == "SETDATA")
                {
                    // exp_value while içinde temizlik için işleme başladığında gerçek değeride okunmuş oluyor
                    // <ISK_KADEME_ORANI>  >>>  t.Str_Replace(ref exp_value, "<" + fname + ">", value);  
                    // yukardaki satırda atanmak istenen değer zaten okunmuş ve atamaya hazır durumda bekliyor

                    if (vExp.exp_value != "")
                    {
                        //    value = vExp.exp_value;
                        vExp.dsData.Tables[0].Rows[vExp.pos][workFName] = System.Convert.ToDecimal(vExp.exp_value);

                        v.con_Expression_View = v.con_Expression_View + " := " + vExp.exp_value + v.ENTER;
                        
                        if ((vExp.focus_field == "TRUE") &&
                            (workFName == vExp.send_FieldName.ToString())
                            //(vExp.exp_formul_fname.ToString() == vExp.send_FieldName.ToString())
                            )
                        { 
                            vExp.send_Value = vExp.exp_value;
                            v.con_Expression_Send_Value = vExp.exp_value;
                        }
                        //MessageBox.Show(vExp.exp_formul_fname.ToString() + ";" + vExp.send_FieldName.ToString());
                        /// Burası aşağıdaki madde için yapıldı
                        ///"CAPTION": "fazla ise",
                        ///"EXP_TYPE": "SETDATA",
                        ///"EXP_VALUE": "10",
                        ///"EXTRA_FNAME": "null",
                        ///"FOCUS_FIELD": "TRUE",
                        ///"CHC_IPCODE": "null",
                        ///"CHC_FNAME": "TEVKIFAT_PAY",
                        ///"CHC_VALUE": "10",
                        ///"CHC_OPERAND": ">",
                    }

                }
                #endregion

                try
                {
                    v.con_Expression = false;
                    if ((value != "") && (value != "null"))
                        vExp.dsData.Tables[0].Rows[vExp.pos][workFName] = System.Convert.ToDecimal(value);
                }
                catch (Exception e)
                {
                    MessageBox.Show(vExp.exp_formul_fname + " : " + workFName + " = " + value + " ; " + v.ENTER2 + e.Message, "Uyarı");
                }
                //t.Str_Replace(ref value, ",", ".");
                #endregion

                // DataLayoutControl ise
                #region yapılan değişikliği view üzerine işle
                if ((vExp.cntrl != null) &&
                    (vExp.cntrl.ToString() == "DevExpress.XtraDataLayout.DataLayoutControl"))
                {
                    DataLayoutControl_Refresh(vExp.tForm, vExp.cntrl, workFName, value);
                }
                #endregion
            }
        }

        private bool Exp_Check(
             tToolBox t
            , DataSet dsData, int pos
            , string chc_FName
            , string chc_Value
            , string chc_Operand
            , string chc_FName_SEC
            , string chc_Value_SEC
            , string chc_Operand_SEC
            )
        {
            bool onay1 = true;
            bool onay2 = true;

            string read_value = "";

            #region Check 1 işlemleri 
            if (//t.IsNotNull(chc_IPCode) &&
                (t.IsNotNull(chc_FName)) &&
                (t.IsNotNull(chc_Value)))
            {
                read_value = dsData.Tables[0].Rows[pos][chc_FName].ToString();
                
                if (read_value != "")
                {
                    if ((chc_Value.IndexOf(read_value) > -1) &&
                        (chc_Operand == ""))
                    {
                        // eğer buraya kadar geldiyse 
                        // öndeki chc_xxxx kontrollerinden geçti,  
                        // yani onayı hak etti demektir
                        onay1 = true;
                    }

                    if (t.IsNotNull(chc_Operand))
                    {
                        onay1 = t.myOperandControl(read_value, chc_Value, chc_Operand);
                    }
                }
            }
            #endregion Check işlemleri1

            #region Check 2 işlemleri 
            if (//t.IsNotNull(chc_IPCode_SEC) &&
                (t.IsNotNull(chc_FName_SEC)) &&
                (t.IsNotNull(chc_Value_SEC)))
            {

                read_value = dsData.Tables[0].Rows[pos][chc_FName_SEC].ToString();

                if (read_value != "")
                {
                    if ((chc_Value_SEC.IndexOf(read_value) > -1) &&
                        (chc_Operand_SEC == ""))
                    {
                        // eğer buraya kadar geldiyse 
                        // öndeki chc_xxxx kontrollerinden geçti,  
                        // yani onayı hak etti demektir
                        onay2 = true;
                    }

                    if (t.IsNotNull(chc_Operand_SEC))
                    {
                        onay2 = t.myOperandControl(read_value, chc_Value_SEC, chc_Operand_SEC);
                    }
                }
            }
            #endregion Check işlemleri2

            return (onay1 && onay2);
        }
        
        private double JScriptEval(string expr)
        {
            Microsoft.JScript.Vsa.VsaEngine myEngine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();

            return double.Parse(Microsoft.JScript.Eval.JScriptEvaluate(expr, myEngine).ToString());

            // error checking etc removed for brevity
            //return double.Parse(Eval.JScriptEvaluate(expr, _engine).ToString());
            //private readonly VsaEngine _engine = VsaEngine.CreateEngine();
        }

        private string prop_Expression_Get(DataSet ds, string tableName, string fName)
        {
            string s = "";
            int j = ds.Tables[tableName].Rows.Count;

            for (int i = 0; i < j; i++)
            {
                if (ds.Tables[tableName].Rows[i]["FIELD_NAME"].ToString() == fName)
                {
                    s = ds.Tables[tableName].Rows[i]["PROP_EXPRESSION"].ToString();
                    break;
                }
            }
            return s;
        }

        #endregion Expression

        #region DataLayoutControl_Refresh
        public void DataLayoutControl_Refresh(Form tForm, Control cntrl, string FieldName, string Value)
        {
            foreach (Control item in ((DataLayoutControl)cntrl).Controls)
            {
                System.Windows.Forms.Binding b = item.DataBindings["EditValue"];
                if (b != null)
                {
                    v.con_Expression = false;
                    if (item.Name == "Column_" + FieldName)
                        item.Text = Value;
                    v.con_Expression = false;
                    b.WriteValue();
                }
            }
        }
        #endregion 

        #region layoutControlGroup_DoubleClick
        public void layoutControlGroup_DoubleClick(object sender, EventArgs e)
        {
            //MessageBox.Show("dbclick");

            //(((DevExpress.XtraLayout.LayoutControlGroup)sender).cus
            /*
            System.IO.Stream stream;
            stream = new System.IO.MemoryStream();
            layoutControl1.SaveLayoutToStream(stream);
            stream.Seek(0, System.IO.SeekOrigin.Begin);

            // ...
            layoutControl1.RestoreLayoutFromStream(stream);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            */
            /*
            string layoutFileName = "layout.xml";
            //...
            private void Form1_Load(object sender, EventArgs e)
            {
                if (System.IO.File.Exists(layoutFileName))
                   layoutControl1.RestoreLayoutFromXml(layoutFileName);
            }

            private void Form1_Closing(object sender, FormClosingEventArgs e)
            {
                layoutControl1.SaveLayoutToXml(layoutFileName);
            }

                private void btnRestoreAll_Click(object sender, EventArgs e) {
        // Restores all the hidden layout items and groups from the Customization Form. 
        while (this.OwnerControl.HiddenItems.Count > 0)
            this.OwnerControl.HiddenItems[0].RestoreFromCustomization();
             }


           */
        }
        #endregion 

        #region myEdit_
        public void myEdit_(object tEdit, ref string TableIPCode, ref string tag)
        {

            // Find_TableIPCode_XtraEditors ile aynı işi yapıyor

            string tcolumn_type = string.Empty;
            tcolumn_type = tEdit.GetType().Name;

            #region
            if (tcolumn_type == "ButtonEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.ButtonEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.ButtonEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.ButtonEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "CalcEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.CalcEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.CalcEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.CalcEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "CheckButton")
            {
                TableIPCode = ((DevExpress.XtraEditors.CheckButton)tEdit).AccessibleName;
                tag = ((DevExpress.XtraEditors.CheckButton)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.CheckButton)tEdit).FindForm();
            }
            if (tcolumn_type == "CheckedComboBoxEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.CheckedComboBoxEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.CheckedComboBoxEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.CheckedComboBoxEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "CheckEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.CheckEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.CheckEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.CheckEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "CheckedListBoxControl")
            {
                TableIPCode = ((DevExpress.XtraEditors.CheckedListBoxControl)tEdit).AccessibleName;
                tag = ((DevExpress.XtraEditors.CheckedListBoxControl)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.CheckedListBoxControl)tEdit).FindForm();
            }
            if (tcolumn_type == "ComboBoxEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.ComboBoxEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.ComboBoxEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.ComboBoxEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "DateEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.DateEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.DateEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.DateEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "HyperLinkEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.HyperLinkEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.HyperLinkEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.HyperLinkEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "ImageComboBoxEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.ImageComboBoxEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.ImageComboBoxEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.ImageComboBoxEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "ImageEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.ImageEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.ImageEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.ImageEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "ImageListBoxControl")
            {
                TableIPCode = ((DevExpress.XtraEditors.ImageListBoxControl)tEdit).AccessibleName;
                tag = ((DevExpress.XtraEditors.ImageListBoxControl)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.ImageListBoxControl)tEdit).FindForm();
            }
            if (tcolumn_type == "LabelControl")
            {
                TableIPCode = ((DevExpress.XtraEditors.LabelControl)tEdit).AccessibleName;
                tag = ((DevExpress.XtraEditors.LabelControl)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.LabelControl)tEdit).FindForm();
            }
            if (tcolumn_type == "ListBoxControl")
            {
                TableIPCode = ((DevExpress.XtraEditors.ListBoxControl)tEdit).AccessibleName;
                tag = ((DevExpress.XtraEditors.ListBoxControl)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.ListBoxControl)tEdit).FindForm();
            }
            if (tcolumn_type == "LookUpEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.LookUpEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.LookUpEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.LookUpEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "MemoEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.MemoEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.MemoEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.MemoEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "MemoExEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.MemoExEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.MemoExEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.MemoExEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "MRUEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.MemoExEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.MemoExEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.MemoExEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "PictureEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.PictureEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.PictureEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.PictureEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "PopupContainerControl")
            {
                TableIPCode = ((DevExpress.XtraEditors.PopupContainerControl)tEdit).AccessibleName;
                tag = ((DevExpress.XtraEditors.PopupContainerControl)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.PopupContainerControl)tEdit).FindForm();
            }
            if (tcolumn_type == "PopupContainerEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.PopupContainerEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.PopupContainerEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.PopupContainerEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "RadioGroup")
            {
                TableIPCode = ((DevExpress.XtraEditors.RadioGroup)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.RadioGroup)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.RadioGroup)tEdit).FindForm();
            }
            if (tcolumn_type == "RangeTrackBarControl")
            {
                TableIPCode = ((DevExpress.XtraEditors.RangeTrackBarControl)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.RangeTrackBarControl)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.RangeTrackBarControl)tEdit).FindForm();
            }
            if (tcolumn_type == "SpinEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.SpinEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.SpinEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.SpinEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "TextEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.TextEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.TextEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.TextEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "TimeEdit")
            {
                TableIPCode = ((DevExpress.XtraEditors.TimeEdit)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.TimeEdit)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.TimeEdit)tEdit).FindForm();
            }
            if (tcolumn_type == "TrackBarControl")
            {
                TableIPCode = ((DevExpress.XtraEditors.TrackBarControl)tEdit).Properties.AccessibleName;
                tag = ((DevExpress.XtraEditors.TrackBarControl)tEdit).Tag.ToString();
                //tForm = ((DevExpress.XtraEditors.TrackBarControl)tEdit).FindForm();
            }
#endregion

        }
        #endregion 

        #region barEditItem_WinExplorerViewStyle_EditValueChanged
        public void barEditItem_WinExplorerViewStyle_EditValueChanged(object sender, EventArgs e)
        {
            //var orientation = (Orientation)barEditItem2.EditValue;
            //tileView1.OptionsTiles.Orientation = orientation;
            tToolBox t = new tToolBox();

            var WinExpViewStyle = (WinExplorerViewStyle)((ImageComboBoxEdit)sender).EditValue;

            string TableIPCode = ((ImageComboBoxEdit)sender).Properties.AccessibleName;

            Form tForm = ((ImageComboBoxEdit)sender).FindForm();

            Control cntrl = null;
            cntrl = t.Find_Control_View(tForm, TableIPCode);
            if (cntrl != null)
            {
                DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView tGridView = ((GridControl)cntrl).MainView as WinExplorerView;
                tGridView.OptionsView.Style = (WinExplorerViewStyle)WinExpViewStyle;
            }
        }
        #endregion 

        #region tileNavPane_SelectedElementChanged
        public void tileNavPane_SelectedElementChanged(object sender, DevExpress.XtraBars.Navigation.TileNavElementEventArgs e)
        {
            if (e.Element.Tag != null)
            {
                tToolBox t = new tToolBox();

                string formName = string.Empty;


                if (e.Element.Appearance.Name != null)
                    formName = e.Element.Appearance.Name.ToString();
                Form tForm = Application.OpenForms[formName];
                                
                string s = "";
                s = e.Element.Tag.ToString();

                // s = "3||guidbilgisi||HPFRM.HPFRM_USER|ds|"
                // s = "3||guidbilgisi||HPFRM.HPFRM_FULL|ds|"
                
                int firmId = t.myInt32(t.Get_And_Clear(ref s, "||"));
                string firm_Guid = t.Get_And_Clear(ref s, "||");
                string firm_TableIPCode = t.Get_And_Clear(ref s, "|ds|");

                if (t.IsNotNull(firm_TableIPCode))
                {
                    DataSet ds = null;
                    DataNavigator dN = null;
                    t.Find_DataSet(tForm, ref ds, ref dN, firm_TableIPCode);

                    if (ds != null)
                    {
                        // SP_FIRM_REF_FNAME = LOCAL_ID
                        int pos = t.Find_GotoRecord(ds, v.SP_FIRM_REF_FNAME, firmId.ToString());
                        dN.Position = pos;

                        // 
                        v.SP_FIRM_ID = firmId;
                        v.SP_FIRM_NAME = ds.Tables[0].Rows[dN.Position]["FIRM_NAME"].ToString();

                        //
                        v.Kullaniciya_Mesaj_Show = true;
                        v.Kullaniciya_Mesaj_Var = v.SP_FIRM_NAME;

                        Application.OpenForms[0].Text = Application.OpenForms[0].AccessibleName + "   [ " + v.SP_FIRM_NAME + " , " + v.SP_FIRM_ID.ToString() + " ]";
                    }
                }
            }
        }
        #endregion 
        
        #region Search Engine / Arama Motoru Events
        
        public void textEdit_Find_EditValueChanged(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            Control cntrl = null;
            Form tForm = ((DevExpress.XtraEditors.TextEdit)sender).FindForm();
            string TableIPCode = ((DevExpress.XtraEditors.TextEdit)sender).Properties.AccessibleDefaultActionDescription;
            cntrl = t.Find_Control_View(tForm, TableIPCode);

            if (cntrl != null)
            {
                if (cntrl.ToString() == "DevExpress.XtraGrid.GridControl")
                {
                    if (((DevExpress.XtraGrid.GridControl)cntrl).MainView.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
                    {
                        GridView view = ((DevExpress.XtraGrid.GridControl)cntrl).MainView as GridView;
                        view.FindFilterText = "\"" + ((DevExpress.XtraEditors.TextEdit)sender).Text + "\"";
                    }

                    if (((DevExpress.XtraGrid.GridControl)cntrl).MainView.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
                    {
                        AdvBandedGridView view = ((DevExpress.XtraGrid.GridControl)cntrl).MainView as AdvBandedGridView;
                        view.FindFilterText = "\"" + ((DevExpress.XtraEditors.TextEdit)sender).Text + "\"";
                    }
                }
                if (cntrl.ToString() == "DevExpress.XtraTreeList.TreeList")
                {
                    ((DevExpress.XtraTreeList.TreeList)cntrl).FindFilterText =
                        "\"" + ((DevExpress.XtraEditors.TextEdit)sender).Text + "\"";
                }

                //if (!string.IsNullOrEmpty(gridView1.FindFilterText) && !gridView1.FindFilterText.Contains('"'))
                //    gridView1.FindFilterText = "\"" + gridView1.FindFilterText + "\"";
            }

            /// ( 100 * find )  neden bunu yapıyorum ?
            /// findDelay = 100 ise standart find
            /// findDelay = 200 ise list && data  yı işaret ediyor 
            int findType = t.myInt32(((DevExpress.XtraEditors.TextEdit)sender).Tag.ToString());

            int valueCount = ((DevExpress.XtraEditors.TextEdit)sender).Text.Length;
            if ((findType == 200) &&
                (v.searchCount > 0) &&
                (v.searchCount > valueCount))
                InData_Close(tForm, TableIPCode);
        }

        public void textEdit_Find_KeyDown(object sender, KeyEventArgs e)
        {
            //Application.OpenForms[0].Text = e.KeyCode.ToString();

            if ((e.KeyCode == Keys.Up) ||
                (e.KeyCode == Keys.Down) ||
                (e.KeyCode == Keys.PageUp) ||
                (e.KeyCode == Keys.PageDown) ||
                (e.KeyCode == Keys.Home) ||
                (e.KeyCode == Keys.End) ||
                (e.KeyCode == Keys.Return) ||
                (e.KeyCode == Keys.Enter) ||
                (e.KeyCode == Keys.Delete) ||
                (e.KeyCode == Keys.ShiftKey) ||
                (e.KeyCode == Keys.Add) ||
                (e.KeyCode == Keys.ControlKey) ||
                (e.Control && e.KeyCode == Keys.Y) ||
                (e.Control && e.KeyCode == Keys.C) ||
                (e.Control && e.KeyCode == Keys.F4) 
                )
            {
                tToolBox t = new tToolBox();
                
                Control cntrl = null;
                Form tForm = ((DevExpress.XtraEditors.TextEdit)sender).FindForm();
                string TableIPCode = ((DevExpress.XtraEditors.TextEdit)sender).Properties.AccessibleDefaultActionDescription;
                cntrl = t.Find_Control_View(tForm, TableIPCode);
                string Prop_Navigator = string.Empty;

                if (cntrl != null)
                {
                    #region GridControl
                    if (cntrl.ToString() == "DevExpress.XtraGrid.GridControl")
                    {
                        if (((DevExpress.XtraGrid.GridControl)cntrl).MainView != null)
                        {
                            #region GridView
                            if (((DevExpress.XtraGrid.GridControl)cntrl).MainView.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
                            {
                                GridView view = ((DevExpress.XtraGrid.GridControl)cntrl).MainView as GridView;
                                if (((DevExpress.XtraGrid.GridControl)cntrl).AccessibleDescription != null)
                                   Prop_Navigator = ((DevExpress.XtraGrid.GridControl)cntrl).AccessibleDescription.ToString();

                                #region
                                if (e.KeyCode == Keys.Down)
                                {
                                    if (view.FocusedRowHandle < view.DataRowCount - 1)
                                    {
                                        view.UnselectRow(view.FocusedRowHandle);
                                        view.FocusedRowHandle += 1;
                                        view.SelectRow(view.FocusedRowHandle);
                                        e.Handled = true;
                                    }
                                }
                                if (e.KeyCode == Keys.Up)
                                {
                                    if (view.FocusedRowHandle > 0)
                                    {
                                        view.UnselectRow(view.FocusedRowHandle);
                                        view.FocusedRowHandle -= 1;
                                        view.SelectRow(view.FocusedRowHandle);
                                        e.Handled = true;
                                    }
                                }
                                if (e.KeyCode == Keys.PageDown)
                                {
                                    if ((view.FocusedRowHandle + 20) < view.DataRowCount - 1)
                                    {
                                        view.UnselectRow(view.FocusedRowHandle);
                                        view.FocusedRowHandle += 20;
                                        view.SelectRow(view.FocusedRowHandle);
                                        e.Handled = true;
                                    }
                                }
                                if (e.KeyCode == Keys.PageUp)
                                {
                                    if ((view.FocusedRowHandle - 20) > 0)
                                    {
                                        view.UnselectRow(view.FocusedRowHandle);
                                        view.FocusedRowHandle -= 20;
                                        view.SelectRow(view.FocusedRowHandle);
                                        e.Handled = true;
                                    }
                                }
                                if (e.KeyCode == Keys.Home)
                                {
                                    if (view.FocusedRowHandle > 0)
                                    {
                                        view.UnselectRow(view.FocusedRowHandle);
                                        view.FocusedRowHandle = 0;
                                        view.SelectRow(view.FocusedRowHandle);
                                        e.Handled = true;
                                    }
                                }
                                if (e.KeyCode == Keys.End)
                                {
                                    if (view.DataRowCount > 0)
                                    {
                                        view.UnselectRow(view.FocusedRowHandle);
                                        view.FocusedRowHandle = view.DataRowCount - 1;
                                        view.SelectRow(view.FocusedRowHandle);
                                        e.Handled = true;
                                    }
                                }
                                #endregion
                            }
                            #endregion GridView

                            #region AdvBandedGridView
                            if (((DevExpress.XtraGrid.GridControl)cntrl).MainView.ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
                            {
                                AdvBandedGridView view = ((DevExpress.XtraGrid.GridControl)cntrl).MainView as AdvBandedGridView;
                                if (((DevExpress.XtraGrid.GridControl)cntrl).AccessibleDescription != null)
                                    Prop_Navigator = ((DevExpress.XtraGrid.GridControl)cntrl).AccessibleDescription.ToString();

                                #region
                                if (e.KeyCode == Keys.Down)
                                {
                                    if (view.FocusedRowHandle < view.DataRowCount - 1)
                                    {
                                        view.UnselectRow(view.FocusedRowHandle);
                                        view.FocusedRowHandle += 1;
                                        view.SelectRow(view.FocusedRowHandle);
                                        e.Handled = true;
                                    }
                                }
                                if (e.KeyCode == Keys.Up)
                                {
                                    if (view.FocusedRowHandle > 0)
                                    {
                                        view.UnselectRow(view.FocusedRowHandle);
                                        view.FocusedRowHandle -= 1;
                                        view.SelectRow(view.FocusedRowHandle);
                                        e.Handled = true;
                                    }
                                }
                                if (e.KeyCode == Keys.PageDown)
                                {
                                    if ((view.FocusedRowHandle + 20) < view.DataRowCount - 1)
                                    {
                                        view.UnselectRow(view.FocusedRowHandle);
                                        view.FocusedRowHandle += 20;
                                        view.SelectRow(view.FocusedRowHandle);
                                        e.Handled = true;
                                    }
                                }
                                if (e.KeyCode == Keys.PageUp)
                                {
                                    if ((view.FocusedRowHandle - 20) > 0)
                                    {
                                        view.UnselectRow(view.FocusedRowHandle);
                                        view.FocusedRowHandle -= 20;
                                        view.SelectRow(view.FocusedRowHandle);
                                        e.Handled = true;
                                    }
                                }
                                if (e.KeyCode == Keys.Home)
                                {
                                    if (view.FocusedRowHandle > 0)
                                    {
                                        view.UnselectRow(view.FocusedRowHandle);
                                        view.FocusedRowHandle = 0;
                                        view.SelectRow(view.FocusedRowHandle);
                                        e.Handled = true;
                                    }
                                }
                                if (e.KeyCode == Keys.End)
                                {
                                    if (view.DataRowCount > 0)
                                    {
                                        view.UnselectRow(view.FocusedRowHandle);
                                        view.FocusedRowHandle = view.DataRowCount - 1;
                                        view.SelectRow(view.FocusedRowHandle);
                                        e.Handled = true;
                                    }
                                }
                                #endregion
                            }
                            #endregion AdvBandedGridView
                        }
                    }
                    #endregion GridControl

                    #region TreeList
                    if (cntrl.ToString() == "DevExpress.XtraTreeList.TreeList")
                    {
                        if (((DevExpress.XtraTreeList.TreeList)cntrl).AccessibleDescription != null)
                            Prop_Navigator = ((DevExpress.XtraTreeList.TreeList)cntrl).AccessibleDescription.ToString();

                        if (e.KeyCode == Keys.Down)
                        {
                            //if (((DevExpress.XtraTreeList.TreeList)cntrl).FocusedRowHandle < ((DevExpress.XtraTreeList.TreeList)cntrl).DataRowCount - 1)
                            //{
                            //((DevExpress.XtraTreeList.TreeList)cntrl).FocusedNode.NextNode. //.FocusedRowHandle += 1;
                            e.Handled = true;
                            //}
                        }
                        if (e.KeyCode == Keys.Up)
                        {
                            //if (view.FocusedRowHandle > 0)
                            //{
                            //    view.FocusedRowHandle -= 1;
                            e.Handled = true;
                            //}
                        }
                    }
                    #endregion
                }


                #region Keys.Return & v.search_CARI_ARAMA_TD
                if (((e.KeyCode == Keys.Return) ||
                     (e.KeyCode == Keys.Enter)) &&
                     (v.searchCount == 0))
                {
                    if (v.search_CARI_ARAMA_TD == v.search_inData)
                    {
                        //MessageBox.Show("indata " + TableIPCode);
                        InData_RunSQL(tForm, TableIPCode, ((DevExpress.XtraEditors.TextEdit)sender).Text, null, null);

                        e.Handled = true;
                    }

                    if (v.search_CARI_ARAMA_TD == v.search_onList)
                    {
                        //MessageBox.Show("onlist");
                    }
                }
                #endregion Keys.Return

                //if (e.Control && e.KeyCode == Keys.Y)
                //{
                //    navigatorButtonExec_Keys(tForm, Keys.Y, TableIPCode, Prop_Navigator);
                //}

                if (e.Control)
                {
                    navigatorButtonExec_Keys(tForm, e.KeyCode, TableIPCode, Prop_Navigator);
                }

                #region Keys.Return & e.handle = false
                if (((e.KeyCode == Keys.Return) ||
                     (e.KeyCode == Keys.Enter)) &&
                     (e.Handled == false)
                   )
                {
                    v.searchCount = 0;

                    navigatorButtonExec_Keys(tForm, e.KeyCode, TableIPCode, Prop_Navigator);
                    
                    //e.Handled = true;

                    if (((DevExpress.XtraEditors.TextEdit)sender).Text.IndexOf("Aradığınız") == -1)
                    {
                        v.searchCount = ((DevExpress.XtraEditors.TextEdit)sender).Text.Length;
                        ((DevExpress.XtraEditors.TextEdit)sender).SelectionStart = v.searchCount;
                    }
                }
                #endregion Keys.Return & e.handle = false

                #region Keys.Delete
                if ((e.KeyCode == Keys.Delete) &&
                    (v.search_CARI_ARAMA_TD == v.search_inData))
                {
                    Search_New(tForm, TableIPCode, ((DevExpress.XtraEditors.TextEdit)sender).Properties.AccessibleName);

                    e.Handled = true;
                }
                #endregion Keys.Delete

            }

            /*
            if (e.KeyCode == Keys.Return)
            {
                view.StartIncrementalSearch(((DevExpress.XtraEditors.TextEdit)sender).Text);
                view.FocusedColumn = view.Columns["TAMADI"];
            }
            */
        }

        public void textEdit_Find_Enter(object sender, EventArgs e)
        {
            int findType = 0;

            if (sender.ToString() == "DevExpress.XtraEditors.ButtonEdit")
            {
                findType = (int)((DevExpress.XtraEditors.ButtonEdit)sender).Tag;
                v.searchCount = ((DevExpress.XtraEditors.ButtonEdit)sender).Text.Length;

                // create sırasında geçici yüklenmiş değer
                if (findType == -100)
                {
                    // şimdilik geçici çözüm
                    findType = 200;
                    ((DevExpress.XtraEditors.ButtonEdit)sender).Tag = 200;
                    //----
                }
            }
            if (sender.ToString() == "DevExpress.XtraEditors.TextEdit")
            {
                findType = (int)((DevExpress.XtraEditors.TextEdit)sender).Tag;
                v.searchCount = ((DevExpress.XtraEditors.TextEdit)sender).Text.Length;
            }
            if (findType == 100) v.search_CARI_ARAMA_TD = v.search_onList;
            if (findType == 200) v.search_CARI_ARAMA_TD = v.search_inData;

        }

        public void toggleSwitch_Find_EditValueChanged(object sender, EventArgs e)
        {
            if (((DevExpress.XtraEditors.ToggleSwitch)sender).IsOn)
                v.search_CARI_ARAMA_TD = v.search_inData;
            else v.search_CARI_ARAMA_TD = v.search_onList;

            //if (((DevExpress.XtraEditors.ToggleSwitch)sender).IsOn)

            Form tForm = ((DevExpress.XtraEditors.ToggleSwitch)sender).FindForm();
            string TableIPCode = ((DevExpress.XtraEditors.ToggleSwitch)sender).Properties.AccessibleDefaultActionDescription;

            if (((DevExpress.XtraEditors.ToggleSwitch)sender).IsOn)
            {
                // v.search_inData
                InData_Close(tForm, TableIPCode);
            }
            else
            {
                // v.search_onList
                InData_RunSQL(tForm, TableIPCode, "<break>", null, null);
            }
        }

        public void InData_Close(Form tForm, string TableIPCode)
        {
            tToolBox t = new tToolBox();

            if (t.IsNotNull(TableIPCode))
            {
                DataSet dsData = null;
                DataNavigator tDataNavigator = null;
                t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TableIPCode);

                if (dsData != null)
                {
                    dsData.Tables[0].Clear();
                    v.searchCount = 0;
                }
            }
        }

        // GotoRecord   "ONdialog"  FOR SEARCH
        // SEARCH  için  GotoRecord  "ONdialog"  
        public void InData_RunSQL(Form tForm, string TableIPCode, string value, 
            List<GET_FIELD_LIST> GetFieldList,
            List<TABLEIPCODE_LIST> TableIPCodeList)
        {
            if (value.Length == 0)
            {
                //MessageBox.Show("Lütfen en azından bir harf girin ...");
                return;
            }
            if (value == "<break>") value = "";

            tToolBox t = new tToolBox();

            if (t.IsNotNull(TableIPCode))
            {
                DataSet dsData = null;
                DataNavigator tDataNavigator = null;
                t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TableIPCode);

                if (dsData != null)
                {
                    string myProp = dsData.Namespace.ToString();
                    string alias = t.MyProperties_Get(myProp, "=TableLabel:");
                    string find_FName = t.MyProperties_Get(myProp, "=FindFName:");
                    string DataFind = t.MyProperties_Get(myProp, "DataFind:");

                    t.Alias_Control(ref alias);

                    if (t.IsNotNull(find_FName) == false)
                    {
                        MessageBox.Show("DİKKAT : " + TableIPCode + " içindeki FindFName tanımlı değil !");
                        return;
                    }

                    if (t.IsNotNull(find_FName))
                        if (find_FName.IndexOf(alias) == -1)
                            find_FName = alias + find_FName;

                    string newValue = "and " + find_FName + " like '%" + value + "%' ";

                    /// DataFind    0= Find yok, 1. standart, 2. List&Data   
                    if (DataFind == "2")
                        External_Kriterleri_Uygula(dsData, alias, newValue, null);

                    v.searchCount = value.Length;
                    
                    #region /// goto record edilecek kayıt mevcut

                    if (dsData.Tables[0].Rows.Count > 0)
                    {
                        //string T_FNAME = string.Empty;
                        string R_FNAME = string.Empty;
                        string MSETVALUE = string.Empty;
                        string WORKTYPE = string.Empty;

                        // aranacak fields ve value sayısı
                        int fc = 0; // TableIPCodeList.Count
                        int vc = 0; // value count
                        bool onay = false;

                        // arama listesinde bulunan kayıt sayısı
                        int rc = dsData.Tables[0].Rows.Count;
                        for (int i = 0; i < rc; i++)
                        {
                            /// aranan kayıt hakkındaki bilgileri oku ve
                            /// okunan tüm alanların eşit olmasını sağla ( ıd = xxx, Ad = yyyyy, ... ) gibi
                            vc = 0;
                            if (GetFieldList != null) fc = GetFieldList.Count;
                            if (TableIPCodeList != null) fc = TableIPCodeList.Count;

                            #region GetFieldList 
                            if (GetFieldList != null)
                            {
                                foreach (var item in GetFieldList)
                                {
                                    //T_FNAME = item.T_FNAME.ToString();
                                    R_FNAME = item.R_FNAME.ToString();       // arama listesindeki fieldin adı
                                    MSETVALUE = item.MSETVALUE.ToString();   // bu fieldin valuesi
                                                                             // Not : GetFieldList üzerinde birden fazla field ve value mevcut
                                                                             // bu fieldlerin ve value si eşit oluncaya kadar arama listesi taranır
                                                                             // cari listesinde birden fazla ( ad = tekin uçar ) olabilir 
                                                                             // doğru kaydı bulabilmek için ( id = xxx ) de aranır

                                    if (dsData.Tables[0].Rows[i][R_FNAME].ToString() == MSETVALUE)
                                    {
                                        vc++;
                                        if (fc == vc)
                                        {
                                            // aran kayıt bulundu, set oldu, işlem bitti

                                            // ama bu set çalışmıyor çünkü dialogFormda işe yaramıyor
                                            tDataNavigator.Position = i;
                                            tDataNavigator.Tag = i;

                                            // dialogForm için bu atama ile FormShown da yeniden set işlemi mevcut
                                            v.con_GotoRecord = "ONdialog";
                                            v.con_GotoRecord_Position = i;
                                            
                                            onay = true;

                                            break; // foreach e ait break
                                        }
                                    }// if
                                }// foreach
                            }//if not null
                            #endregion GetFieldList 
                            
                            #region TableIPCodeList 
                            if (TableIPCodeList != null)
                            {
                                foreach (var item in TableIPCodeList)
                                {
                                    R_FNAME = item.RKEYFNAME.ToString();     // arama listesindeki fieldin adı
                                    MSETVALUE = item.MSETVALUE.ToString();   // bu fieldin valuesi
                                    WORKTYPE = item.WORKTYPE.ToString();     // Not : GetFieldList üzerinde birden fazla field ve value mevcut
                                                                             // bu fieldlerin ve value si eşit oluncaya kadar arama listesi taranır
                                                                             // cari listesinde birden fazla ( ad = tekin uçar ) olabilir 
                                                                             // doğru kaydı bulabilmek için ( id = xxx ) de aranır
                                    // listede setdata dan başka işlerde olabiliyor
                                    if (WORKTYPE != "SETDATA")
                                        fc--;
                                    
                                    if (dsData.Tables[0].Rows[i][R_FNAME].ToString() == MSETVALUE)
                                        vc++;
                                    
                                    if (fc == vc)
                                    {
                                        // aran kayıt bulundu, set oldu, işlem bitti
                                        // örnek: 3 field var, üçüde eşitlendiyse

                                        // ama bu set çalışmıyor çünkü dialogFormda işe yaramıyor
                                        tDataNavigator.Position = i;
                                        tDataNavigator.Tag = i;

                                        // dialogForm için bu atama ile FormShown da yeniden set işlemi mevcut
                                        v.con_GotoRecord = "ONdialog";
                                        v.con_GotoRecord_Position = i;

                                        onay = true;

                                        break; // foreach e ait break
                                    }


                                }// foreach
                            }//if not null
                            #endregion TableIPCodeList 
                            
                            if (onay)
                            {
                                break; // if e ait break
                            }

                        }// for
                    }
                    #endregion
                }
            }
        }

        public void Search_New(Form tForm, string Search_TableIPCode, string Target_TableIPCode)
        {
            tToolBox t = new tToolBox();

            if (t.IsNotNull(Target_TableIPCode))
            {
                DataSet ds = null;
                DataNavigator dN = null;
                t.Find_DataSet(tForm, ref ds, ref dN, Target_TableIPCode);

                if (ds != null)
                {

                    string state = t.Find_TableState(ds, dN, "", 0);
                    if (state != "dsInsert") return;

                    //
                    // Target ds de kullanılan mevcut yeni oluşan row'u sil
                    //
                    //NavigatorButton btn = dN.Buttons.Remove;
                    //dN.Buttons.DoClick(btn);
                    //ds.AcceptChanges();
                    
                    /// bu Remove benim 1 günümü yedi, hatalı çalıştı
                    /// onun yerine Refreshle kurtuldum
                    t.TableRefresh(tForm, ds);

                    //
                    // Target ds de yeni row oluştur
                    //
                    ds.Tables[0].CaseSensitive = false;
                    tNewData(tForm, Target_TableIPCode);


                    //
                    // Arama Motoru Listesini Temizle 
                    //
                    v.searchCount = 0;
                    //InData_Close(tForm, Search_TableIPCode);
                    //   textEdit_Find_EditValueChanged gittiğinde bu fonk. zaten çalışıyor

                }
            }
        }

        #endregion Search

        #endregion SubFunctions

        #region SUBVIEW

        #region tSubView_
        public void tSubView_(Form tForm, string Prop_Navigator, string selectItemValue, string caption, string MenuValue)
        {
            tToolBox t = new tToolBox();

            #region 

            /// 0=ROW_PROP_NAVIGATOR:0;
            /// 0=CAPTION:null;
            /// 0=BUTTONTYPE:null;
            /// 0=TABLEIPCODE_LIST:TABLEIPCODE_LIST={
            /// 1=ROW_TABLEIPCODE_LIST:1;
            /// 1=CAPTION:Detail Group Bilgileri;
            ///
            /// 1=TABLEIPCODE:FNLNVOL.FNLNVOL_KSL01;
            /// 1=TABLEALIAS:PARAM_SET;
            /// 1=KEYFNAME:AND ( [FNLNVOL].BHESAP_ID = {0} OR [FNLNVOL].AHESAP_ID = {0} );
            ///
            /// 1=RTABLEIPCODE:LKP_FULL_NAME;
            /// 1=RKEYFNAME:LKP_REF_ID;
            /// 1=MSETVALUE:null;
            /// 1=WORKTYPE:READ;
            /// 1=CONTROLNAME:null;
            /// 1=ROWE_TABLEIPCODE_LIST:1;
            /// TABLEIPCODE_LIST=};
            ///
            /// 0=FORMNAME:null;
            /// 0=FORMCODE:null;
            /// 0=FORMTYPE:null;
            /// 0=FORMSTATE:null;
            /// 0=CHC_IPCODE:HPBNK.HPBNK_02;
            /// 0=CHC_FNAME:LKP_FNS_TIPI;
            /// 0=CHC_VALUE:2020;
            /// 0=CHC_OPERAND:=;
            /// 0=ROWE_PROP_NAVIGATOR:0;                    

            #endregion

            string TableIPCode = string.Empty;
            string TableAlias = string.Empty;
            string KeyFName = string.Empty;

            string s1 = "=ROW_PROP_NAVIGATOR:";
            string s0 = ":TABLEIPCODE_LIST={";
            string s2 = (char)34 + "TABLEIPCODE_LIST" + (char)34 + ": [";
            string s3 = (char)39 + "TABLEIPCODE_LIST" + (char)39 + ": [";
            // "TABLEIPCODE_LIST": [

            // OLD
            if ((Prop_Navigator.IndexOf(s1) > -1) ||
                (Prop_Navigator.IndexOf(s0) > -1))
            {
                TableIPCode = t.MyProperties_Get(Prop_Navigator, "=TABLEIPCODE:");
                TableAlias = t.MyProperties_Get(Prop_Navigator, "=TABLEALIAS:");
                KeyFName = t.MyProperties_Get(Prop_Navigator, "=KEYFNAME:");
            }



            // JSON
            if ((Prop_Navigator.IndexOf(s2) > -1) ||
                (Prop_Navigator.IndexOf(s3) > -1))
            {
                PROP_NAVIGATOR packet = new PROP_NAVIGATOR();
                Prop_Navigator = Prop_Navigator.Replace((char)34, (char)39);
                PROP_NAVIGATOR prop_ = null;

                prop_ = JsonConvert.DeserializeAnonymousType(Prop_Navigator, packet);
                

                // Group bilgileri için 
                foreach (var item in prop_.TABLEIPCODE_LIST)
                {
                    TableIPCode = item.TABLEIPCODE;
                    TableAlias = item.TABLEALIAS;
                    KeyFName = item.KEYFNAME;

                    if (TableAlias.IndexOf("[") == -1)
                        TableAlias = "[" + TableAlias + "]";
                }
            }

            //-----
            // 1. Tablo
            string myFormLoadValue = string.Empty;
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "BEGIN", string.Empty);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "TargetTableIPCode", TableIPCode);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "TargetTableAlias", TableAlias);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "TargetFieldName", KeyFName);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "TargetValue", selectItemValue);
            // 2. Tablo
            //t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "SecondTableIPCode", TableIPCode);
            //t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "SecondTableAlias", TableAlias);
            //t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "SecondFieldName", "AHESAP_ID");
            //t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "SecondValue", "1");
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "END", string.Empty);

            v.con_FormLoadValue = myFormLoadValue;

            tSubView_Preparing(tForm, "TabPage", "", TableIPCode, selectItemValue, caption, MenuValue);
        }
        #endregion 

        #region tSubView_Preparing
        public void tSubView_Preparing(Form tForm,
            string ViewType, string Form_Code, string TableIPCode,
            string ReadValue, string ReadCaption, string MenuValue)
        {
            tToolBox t = new tToolBox();

            #region TabPage ise
            if ((ViewType == "TabPage") ||
                (ViewType == "TabPage2"))
            {
                // yok ise yeni TabPage oluşturulacak
                // var ise Show etmesi sağlanacak
                string TabControlName = "tabControl_SUBVIEW";
                CreateOrSelect_Page(tForm, TabControlName, TableIPCode, ReadValue);

                // yukarıda oluşturulan TabPage aranıyor
                Control cntrl = null;
                string[] controls = new string[] {
                    "DevExpress.XtraTab.XtraTabPage",
                    "DevExpress.XtraBars.Navigation.NavigationPage",
                    "DevExpress.XtraBars.Navigation.TabNavigationPage",
                    "DevExpress.XtraBars.Ribbon.BackstageViewClientControl"
                };

                cntrl = t.Find_Control(tForm, "tTabPage_" + t.AntiStr_Dot(TableIPCode + ReadValue), "", controls);

                // TabPage bulunduysa üzerinde istenen IP (InputPanel) oluşturuluyor
                if (cntrl != null)
                {
                    /// TabPage nin içi boş ise InputPanelin içi hazırlanması gerekiyor
                    /// if (((DevExpress.XtraTab.XtraTabPage)cntrl).Controls.Count == 0)
                    /// 
                    if (cntrl.Controls.Count == 0)
                    {
                        tInputPanel ip = new tInputPanel();

                        #region TabPage
                        if (ViewType == "TabPage")
                        {
                            ip.Create_InputPanel(tForm, cntrl, TableIPCode, ReadValue, 1);

                            if (t.IsNotNull(ReadCaption))
                                cntrl.Text = "   " + ReadCaption + "   ";
                            //cntrl.Text = " [ " + ReadCaption + " ].[ " + ReadValue + " ] ";

                            /// 'DEFAULT_TYPE', 22, '', 'Source ParentControl.Tag READ'); 
                            ///  Viewin içinde bulunduğu control ün tag ındaki value yi okumak için
                            /// 
                            ///cntrl.Tag = ReadValue;
                            cntrl.Tag = ReadValue + "||" + ReadCaption + "||" + MenuValue;
                            v.con_Source_ParentControl_Tag_Value = ReadValue + "||" + ReadCaption + "||" + MenuValue;

                            // tabPage ile RibbonPage bağlantısını sağlamak için gerekiyor
                            // tabPege değiştiğinde RibbonPagede ona göre değişsin 
                            if (t.IsNotNull(MenuValue))
                                cntrl.AccessibleDefaultActionDescription = MenuValue;

                            //tTabPage_FNLNVOL_FNLNVOL_BNL0123
                            //FNLNVOL.FNLNVOL_BNL01|23   grid.
                        }
                        #endregion TabPage

                        #region TabPage2
                        if (ViewType == "TabPage2")
                        {
                            SplitContainer splitContainer1 = new System.Windows.Forms.SplitContainer();
                            // 
                            // splitContainer1
                            // 
                            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
                            splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
                            splitContainer1.Location = new System.Drawing.Point(0, 0);
                            splitContainer1.Name = "splitContainer1";
                            splitContainer1.Size = new System.Drawing.Size(958, 308);
                            splitContainer1.SplitterDistance = 195;
                            splitContainer1.TabIndex = 0;

                            ((DevExpress.XtraTab.XtraTabPage)cntrl).Controls.Add(splitContainer1);

                            ip.Create_InputPanel(tForm, splitContainer1.Panel1, TableIPCode, 2);
                            ip.Create_InputPanel(tForm, splitContainer1.Panel2, TableIPCode, 1);

                            ((DevExpress.XtraTab.XtraTabPage)cntrl).Text = splitContainer1.Panel2.Text;
                        }
                        #endregion TabPage2
                    }
                }

            }
            #endregion TabPage

            #region dockPanel ise
            if (ViewType == "dockPanel")
            {
                // yukarıda oluşturulan TabPage aranıyor
                Control cntrl = null;
                string[] controls = new string[] { "DevExpress.XtraBars.Docking.DockPanel" };
                cntrl = t.Find_Control(tForm, Form_Code, "", controls);

                // TabPage bulunduysa üzerinde istenen IP (InputPanel) oluşturuluyor
                if (cntrl != null)
                {
                    // dockPanel boş ise InputPanelin içi hazırlanması gerekiyor
                    if (((DevExpress.XtraBars.Docking.DockPanel)cntrl).Controls.Count == 1)
                    {
                        tInputPanel ip = new tInputPanel();
                        ip.Create_InputPanel(tForm, ((DevExpress.XtraBars.Docking.DockPanel)cntrl), TableIPCode, 1);
                    }
                }
            }
            #endregion dockPanel ise

            // PageName var ise bir menunün (Ribbon un) page ismidir ve bu page show edilececek
            //#region Menu Page Show
            //if (t.IsNotNull(SM_PageName))
            //{
            //    /*
            //    Form tForm = t.Find_Form(sender);

            //    Control c = t.Find_Control(tForm, "MENU_VZ_BNK_TP");

            //    if (c != null)
            //    {

            //    }
            //    */
            //}
            //#endregion Menu Page Show
        }
        #endregion 

        #region CreateOrSelect TabPage 

        public void CreateOrSelect_Page(Form tForm, string TabControlName, string TableIPCode, string ReadValue)
        {
            tToolBox t = new tToolBox();

            string TableIPCode_NotDot = t.AntiStr_Dot(TableIPCode);

            // TabPage aranıyor
            Control c = t.Find_Control(tForm, "tTabPage_" + TableIPCode_NotDot + ReadValue); // , "", controls);

            /// SelectedPage
            /// 
            ///     "DevExpress.XtraTab.XtraTabPage"
            ///     "DevExpress.XtraBars.Navigation.NavigationPage"
            ///     "DevExpress.XtraBars.Navigation.TabNavigationPage"
            ///     "DevExpress.XtraBars.Ribbon.BackstageViewClientControl"
            ///     

#region SelectedPage var ise bulduğu sayfayı select et geri dön
            if (c != null)
            {
#region XtraTabPage
                if (c.ToString() == "DevExpress.XtraTab.XtraTabPage")
                {
                    // hangi sayfayı gösteriyor ( TableIPCode )
                    ((DevExpress.XtraTab.XtraTabPage)c).TabControl.AccessibleName =
                        ((DevExpress.XtraTab.XtraTabPage)c).AccessibleName;

                    ((DevExpress.XtraTab.XtraTabPage)c).TabControl.SelectedTabPage =
                        ((DevExpress.XtraTab.XtraTabPage)c);
                }
#endregion

#region NavigationPage
                if (c.ToString() == "DevExpress.XtraBars.Navigation.NavigationPage")
                {

                    ((DevExpress.XtraBars.Navigation.NavigationPage)c).Parent.AccessibleName =
                        ((DevExpress.XtraBars.Navigation.NavigationPage)c).AccessibleName;

                    Control cntrl = ((DevExpress.XtraBars.Navigation.NavigationPage)c).Parent;
                    if (cntrl.ToString() == "DevExpress.XtraBars.Navigation.NavigationPane")
                    {
                        ((DevExpress.XtraBars.Navigation.NavigationPane)cntrl).SelectedPage =
                            ((DevExpress.XtraBars.Navigation.NavigationPage)c);
                    }
                }
#endregion

#region TabNavigationPage
                if (c.ToString() == "DevExpress.XtraBars.Navigation.TabNavigationPage")
                {

                    ((DevExpress.XtraBars.Navigation.TabNavigationPage)c).Parent.AccessibleName =
                        ((DevExpress.XtraBars.Navigation.TabNavigationPage)c).AccessibleName;

                    Control cntrl = ((DevExpress.XtraBars.Navigation.TabNavigationPage)c).Parent;
                    if (cntrl.ToString() == "DevExpress.XtraBars.Navigation.TabPane")
                    {
                        ((DevExpress.XtraBars.Navigation.TabPane)cntrl).SelectedPage =
                            ((DevExpress.XtraBars.Navigation.TabNavigationPage)c);
                    }
                }
#endregion

                return;
            }
#endregion

            // ------------------------------------------------------------------------------

            // TabPageyi iki şekilde bulabiliriz
            // 1. Form üzerindeki TabPage aranır ve ilk bulunan getirilir
            // 2. TabPage nin MasterKey i belirtilmiş ise direk o TabPage aranır

            string Item_MasterKey = string.Empty;
            string Caption = string.Empty;

            if (t.IsNotNull(TableIPCode_NotDot))
            {
                Item_MasterKey = t.Find_Button_Value(tForm, TableIPCode_NotDot, "=FormCode:");
                Caption = t.Find_Button_Value(tForm, TableIPCode_NotDot, "=Caption:");
            }

            // TabControl aranıyor 
            if (t.IsNotNull(Item_MasterKey) == false)
            {
                c = t.Find_Control(tForm, TabControlName);
            }

            if (t.IsNotNull(Item_MasterKey))
            {
                c = t.Find_Control_Tag(tForm, Item_MasterKey);
            }

            if (c == null)
            {
                c = t.Find_Control(tForm, TabControlName);
            }

            /// TabControl or 
            /// TabPane or
            /// NavigationPane or
            /// 
            /// bulunduysa yeni Page oluştur

            if (c != null)
            {
#region XtraTab.XtraTabControl
                if (c.ToString() == "DevExpress.XtraTab.XtraTabControl")
                {
                    // yeni TabPage hazırlanıyor
                    DevExpress.XtraTab.XtraTabPage tTabPage = new DevExpress.XtraTab.XtraTabPage();

                    tTabPage.AccessibleName = TableIPCode;
                    tTabPage.AccessibleDescription = TableIPCode_NotDot;

                    tTabPage.Name = "tTabPage_" + TableIPCode_NotDot + "|" + ReadValue;
                    tTabPage.Tag = Item_MasterKey;
                    tTabPage.Text = Caption;

                    tTabPage.Padding = new System.Windows.Forms.Padding(v.Padding4);

                    // Bulunan TabControl ün üzerine yeni bir TabPage ekleniyor
                    ((DevExpress.XtraTab.XtraTabControl)c).AccessibleName = TableIPCode;
                    ((DevExpress.XtraTab.XtraTabControl)c).AccessibleDescription = TableIPCode_NotDot + "|" + ReadValue;
                    ((DevExpress.XtraTab.XtraTabControl)c).TabPages.Add(tTabPage);
                    ((DevExpress.XtraTab.XtraTabControl)c).SelectedTabPage = tTabPage;
                }
#endregion

#region Navigation.NavigationPane
                if (c.ToString() == "DevExpress.XtraBars.Navigation.NavigationPane")
                {
                    // yeni TabPage hazırlanıyor
                    DevExpress.XtraBars.Navigation.NavigationPage tTabPage =
                        new DevExpress.XtraBars.Navigation.NavigationPage();

                    tTabPage.AccessibleName = TableIPCode;
                    tTabPage.AccessibleDescription = TableIPCode_NotDot + "|" + ReadValue;

                    tTabPage.Name = "tTabPage_" + TableIPCode_NotDot + ReadValue;
                    tTabPage.Tag = Item_MasterKey;
                    tTabPage.Text = Caption;

                    //tTabPage.Padding = new System.Windows.Forms.Padding(v.Padding4);

                    ((System.ComponentModel.ISupportInitialize)(((DevExpress.XtraBars.Navigation.NavigationPane)c))).BeginInit();

                    // Bulunan TabControl ün üzerine yeni bir TabPage ekleniyor
                    ((DevExpress.XtraBars.Navigation.NavigationPane)c).AccessibleName = TableIPCode;
                    ((DevExpress.XtraBars.Navigation.NavigationPane)c).AccessibleDescription = TableIPCode_NotDot + "|" + ReadValue;
                    ((DevExpress.XtraBars.Navigation.NavigationPane)c).Controls.Add(tTabPage);
                    ((DevExpress.XtraBars.Navigation.NavigationPane)c).Pages.Add((DevExpress.XtraBars.Navigation.NavigationPage)tTabPage);
                    ((DevExpress.XtraBars.Navigation.NavigationPane)c).SelectedPage = tTabPage;

                    ((System.ComponentModel.ISupportInitialize)(((DevExpress.XtraBars.Navigation.NavigationPane)c))).EndInit();

                }
#endregion

#region Navigation.TabPane
                if (c.ToString() == "DevExpress.XtraBars.Navigation.TabPane")
                {
                    // yeni TabPage hazırlanıyor
                    DevExpress.XtraBars.Navigation.TabNavigationPage tTabPage =
                        new DevExpress.XtraBars.Navigation.TabNavigationPage();

                    tTabPage.AccessibleName = TableIPCode;
                    tTabPage.AccessibleDescription = TableIPCode_NotDot + "|" + ReadValue;

                    tTabPage.Name = "tTabPage_" + TableIPCode_NotDot + ReadValue;
                    tTabPage.Tag = Item_MasterKey;
                    tTabPage.Text = Caption;

                    //tTabPage.Padding = new System.Windows.Forms.Padding(v.Padding4);

                    ((System.ComponentModel.ISupportInitialize)(((DevExpress.XtraBars.Navigation.TabPane)c))).BeginInit();

                    // Bulunan TabControl ün üzerine yeni bir TabPage ekleniyor
                    ((DevExpress.XtraBars.Navigation.TabPane)c).AccessibleName = TableIPCode;
                    ((DevExpress.XtraBars.Navigation.TabPane)c).AccessibleDescription = TableIPCode_NotDot + "|" + ReadValue;
                    ((DevExpress.XtraBars.Navigation.TabPane)c).Controls.Add(tTabPage);
                    ((DevExpress.XtraBars.Navigation.TabPane)c).Pages.Add((DevExpress.XtraBars.Navigation.TabNavigationPage)tTabPage);
                    ((DevExpress.XtraBars.Navigation.TabPane)c).SelectedPage = tTabPage;

                    ((System.ComponentModel.ISupportInitialize)(((DevExpress.XtraBars.Navigation.TabPane)c))).EndInit();

                }
#endregion
            }


        }

        #endregion CreateOrSelect TabPage

        #region tSubView_Refresh
        public void tSubView_Refresh(Form tForm, DataSet dsData, DevExpress.XtraEditors.DataNavigator tDataNavigator)
        {
            string SubView1 = tDataNavigator.AccessibleDescription;

            string s1 = "=ROW_PROP_SUBVIEW:";
            string s2 = (char)34 + "SV_ENABLED" + (char)34 + ": " + (char)34 + "TRUE" + (char)34;
            //  "SV_ENABLED": "TRUE",

            if (SubView1.IndexOf(s1) > -1)
                tSubView_Refresh_OLD(tForm, dsData, tDataNavigator);

            if (SubView1.IndexOf(s2) > -1)
                tSubView_Refresh_JSON(tForm, dsData, tDataNavigator);
        }
        #endregion 

        #region tSubView_Refresh_JSON / tSubView_Refresh_OLD
        public void tSubView_Refresh_JSON(Form tForm, DataSet dsData, DevExpress.XtraEditors.DataNavigator tDataNavigator)
        {
            tToolBox t = new tToolBox();

            //Form tForm = t.Find_Form(tDataNavigator);
            string SubView = tDataNavigator.AccessibleDescription;

            string value = t.Find_Properties_Value(SubView, "SV_ENABLED");

            if (value == "TRUE")
            {
                PROP_SUBVIEW packet = new PROP_SUBVIEW();
                SubView = SubView.Replace((char)34, (char)39);
                var prop_ = JsonConvert.DeserializeAnonymousType(SubView, packet);

                string SV_KeyFName = prop_.SV_KEYFNAME.ToString();
                string SV_CaptionFName = prop_.SV_CAPTION_FNAME.ToString();
                string SV_CmpType = prop_.SV_CMP_TYPE.ToString();
                string SV_CmpLocation = prop_.SV_CMP_LOCATION.ToString();
                string SV_ViewType = prop_.SV_VIEW_TYPE.ToString();
                List<SV_LIST> SV_List = prop_.SV_LIST;

                string Now_Value = string.Empty;
                string TableIPCode = string.Empty;
                string SM_PageName = string.Empty;

                #region TABLEIPCODE tespiti

                if (t.IsNotNull(SV_KeyFName))
                {
                    // SUBVIEW ilk okunduğunda ve yine ilk önce hangi position gitmesi gerekiyorsa o sağlanıyor
                    if ((v.con_SubView_FIRST_POSITION > -1) &&
                        (v.con_SubView_FIRST_POSITION != tDataNavigator.Position))
                    {
                        tDataNavigator.Position = v.con_SubView_FIRST_POSITION;
                        v.con_SubView_FIRST_POSITION = -1;
                    }
                    
                    //Now_Value = detail_Row[SV_KeyFName].ToString();
                    if (tDataNavigator.Position > -1)
                    {
                        try
                        {
                            Now_Value = dsData.Tables[0].Rows[tDataNavigator.Position][SV_KeyFName].ToString();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message.ToString());
                        }
                    }

                }

                foreach (var item in SV_List)
                {
                    if (item.SV_VALUE == Now_Value)
                    {
                        TableIPCode = item.TABLEIPCODE.ToString();
                        // PageName var ise bir menunün (Ribbon un) page ismidir ve bu page show edilececek
                        SM_PageName = item.SM_PAGENAME.ToString();

                        #region SubView Create
                        if (t.IsNotNull(TableIPCode))
                        {
                            tSubView_Preparing(tForm, SV_ViewType, "", TableIPCode, "", "", SM_PageName);
                            break;
                        }
                        #endregion
                    }
                }                
                #endregion TABLEIPCODE tespiti
            }

        }
        
        public void tSubView_Refresh_OLD(Form tForm, DataSet dsData, DevExpress.XtraEditors.DataNavigator tDataNavigator)
        {
            tToolBox t = new tToolBox();

            //Form tForm = t.Find_Form(tDataNavigator);
            string SubView1 = tDataNavigator.AccessibleDescription;

            if (SubView1.IndexOf("=SV_ENABLED:TRUE;") > -1)
            {
                // yeni setfocus olan satırın bilgileri alınıyor
                //DataRow detail_Row = dsData.Tables[0].Rows[tDataNavigator.Position] as DataRow;

                vSubView vSV = new vSubView();
                SubView_Get(vSV, SubView1);

                string Now_Value = string.Empty;
                string s = string.Empty;
                string TableIPCode = string.Empty;
                string SM_PageName = string.Empty;

                #region TABLEIPCODE tespiti

                if (t.IsNotNull(vSV.SV_KeyFName))
                {
                    // GotoRecord da atama olsa dahi değer değişmediği için 
                    // burada bu şekilde uydurma bir metoda gidildi....
                    //if ((tDataNavigator.Tag != null) &&
                    //    (tDataNavigator.Position == -1))
                    //{
                    //    if (tDataNavigator.Position != t.myInt32(tDataNavigator.Tag.ToString()))
                    //        tDataNavigator.Position = t.myInt32(tDataNavigator.Tag.ToString());
                    //}

                    // SUBVIEW ilk okunduğunda ve yine ilk önce hangi position gitmesi gerekiyorsa o sağlanıyor
                    if (v.con_SubView_FIRST_POSITION > -1)
                    {
                        tDataNavigator.Position = v.con_SubView_FIRST_POSITION;
                        v.con_SubView_FIRST_POSITION = -1;
                    }

                    //---

                    //Now_Value = detail_Row[SV_KeyFName].ToString();
                    if (tDataNavigator.Position > -1)
                    {
                        try
                        {
                            Now_Value = dsData.Tables[0].Rows[tDataNavigator.Position][vSV.SV_KeyFName].ToString();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message.ToString());
                        }
                    }

                }

                s = "=SV_VALUE:" + Now_Value + ";";

                if (SubView1.IndexOf(s) > 0)
                {
                    SubView1 = t.AfterGet_And_BeforeClear(ref SubView1, s, false);

                    TableIPCode = t.MyProperties_Get(SubView1, "=TABLEIPCODE:");

                    // PageName var ise bir menunün (Ribbon un) page ismidir ve bu page show edilececek
                    SM_PageName = t.MyProperties_Get(SubView1, "=SM_PAGENAME:");
                }
                #endregion TABLEIPCODE tespiti

                #region SubView Create
                if (t.IsNotNull(TableIPCode))
                {
                    tSubView_Preparing(tForm, vSV.SV_ViewType, "", TableIPCode, "", "", SM_PageName);
                }
                #endregion
            }

            #region örnek
            /*
            PROP_SUBVIEW={
            0=ROW_PROP_SUBVIEW:0;
            0=SV_ENABLED:TRUE;
            0=SV_KEYFNAME:LIST_TYPE;
            0=SV_CAPTION_FNAME:null;
            0=SV_CMP_TYPE:null;
            0=SV_CMP_LOCATION:null;
            0=SV_VIEW_TYPE:TabPage;
            0=SV_LIST:SV_LIST={
            1=ROW_SV_LIST:1;
            1=CAPTION:MS_TABLES için;
            1=SV_VALUE:Table;
            1=TABLEIPCODE:T01_MSTABLES.T01_MSTABLES_02;
            1=SM_PAGENAME:        <<< yeni 
            1=ROWE_SV_LIST:1;
            2=ROW_SV_LIST:2;
            2=CAPTION:MS_TABLESIP için;
            2=SV_VALUE:InputPanel;
            2=TABLEIPCODE:T02_MSTABLESIP.T02_MSTABLESIP_02;
            2=SM_PAGENAME:        <<< yeni
            2=ROWE_SV_LIST:2;
            SV_LIST=};
            0=ROWE_PROP_SUBVIEW:0;
            PROP_SUBVIEW=}

            */
            #endregion örnek

        }
        #endregion 

        #region tSubView_Param
        public void tSubView_Param(Form tForm, string ViewType, DataSet dsKrtr,
                                   string TableIPCode, Boolean tKisitli, int tTableType)
        {
            tToolBox t = new tToolBox();

            #region TabPage ise
            if (ViewType == "TabPage")
            {
                //tInputPanel ip = new tInputPanel();
                //tCreateObject co = new tCreateObject();

                // yok ise yeni TabPage oluşturulacak
                // var ise Show etmesi sağlanacak
                string TabControlName = "tabControl_SUBVIEW";
                CreateOrSelect_Page(tForm, TabControlName, TableIPCode, "");

                // yukarıda oluşturulan TabPage aranıyor
                Control cntrl = null;
                string[] controls = new string[] { "" }; //DevExpress.XtraTab.XtraTabPage
                cntrl = t.Find_Control(tForm, "tTabPage_" + t.AntiStr_Dot(TableIPCode), "", controls);

                // TabPage bulunduysa üzerinde istenen IP (InputPanel) oluşturuluyor
                if (cntrl != null)
                {

                    // TabPage nin içi boş ise Parametreler için hazırlanması gerekiyor
                    if (((DevExpress.XtraTab.XtraTabPage)cntrl).Controls.Count == 0)
                    {
                        if (dsKrtr != null)
                        {
                            tCreateObject co = new tCreateObject();
                            co.Create_Param_Panel(tForm, ((DevExpress.XtraTab.XtraTabPage)cntrl), dsKrtr,
                                                  TableIPCode, tKisitli, tTableType);
                        }
                        //ip.Create_InputPanel(tForm, ((DevExpress.XtraTab.XtraTabPage)cntrl), TableIPCode, 1);
                    }
                }

            }
            #endregion TabPage

        }
        #endregion 

        #region tExtraCreateView
        public void tExtraCreateView(Form tForm)
        {
            tToolBox t = new tToolBox();

            string form_prp = t.Set(t.myFormBox_Values(tForm, v.FormLoad), v.con_FormLoadValue, string.Empty);

            if (t.IsNotNull(form_prp) == false) return;

            string s = form_prp;

            if (s.IndexOf("TABLEIPCODE_LIST") > -1)
            {
                string TABLEIPCODE = string.Empty;
                string TABLEALIAS = string.Empty;
                string KEYFNAME = string.Empty;
                string MSETVALUE = string.Empty;
                string WORKTYPE = string.Empty;
                string CONTROLNAME = string.Empty;

                string row_block = string.Empty;
                string lockE = "=ROWE_";

                while (s.IndexOf(lockE) > -1)
                {
                    row_block = t.Find_Properies_Get_RowBlock(ref s, "TABLEIPCODE_LIST");

                    if (row_block.IndexOf("CREATEVIEW") > -1)
                    {
                        TABLEIPCODE = t.MyProperties_Get(row_block, "TABLEIPCODE:");
                        CONTROLNAME = t.MyProperties_Get(row_block, "CONTROLNAME:");

                        if (t.IsNotNull(CONTROLNAME))
                        {
                            Control cntrl = null;
                            string[] controls = new string[] { };
                            cntrl = t.Find_Control(tForm, CONTROLNAME, "", controls);

                            if (cntrl != null)
                            {
                                tInputPanel ip = new tInputPanel();
                                ip.Create_InputPanel(tForm, cntrl, TABLEIPCODE, 1);
                            } // if (cntrl != 
                            else
                            {
                                MessageBox.Show("DİKKAT : ExtraCreateView için " + CONTROLNAME + " isimli kontrol bulunamadı");
                            }
                        } // if (t.IsNotNull(
                    } //if (row_block.IndexOf 
                } //while ( 
            } // if (s

            //v.con_FormLoadValue = string.Empty; 
        }
        #endregion 

        #region SubView_Get
        private void SubView_Get(vSubView vSV, string SubView)
        {
            tToolBox t = new tToolBox();

            vSV.SV_KeyFName = t.MyProperties_Get(SubView, "SV_KEYFNAME:");
            vSV.SV_CaptionFName = t.MyProperties_Get(SubView, "SV_CAPTION_FNAME:");
            vSV.SV_CmpType = t.MyProperties_Get(SubView, "SV_CMP_TYPE:");
            vSV.SV_CmpLocation = t.MyProperties_Get(SubView, "SV_CMP_LOCATION:");
            vSV.SV_ViewType = t.MyProperties_Get(SubView, "SV_VIEW_TYPE:");
            vSV.SV_List = t.MyProperties_Get(SubView, "SV_LIST:");
        }
        #endregion 

        #endregion SUBVIEW

        #region tSubWork.. 

        /// new sub functions 
        /// -------------------------------------------------
        /// tForm
        /// TableIPCode
        /// WorkTD = NewData, Refresh
        /// WorkWhom = All, Only, Childs
        ///
        public void tSubWork_(vSubWork vSW)
        {
            if (v.con_ExtraChange == true) return;
             
            tToolBox t = new tToolBox();

            #region Tanımlar

            bool onay = true;
            bool old_PositionChange = false;
            string TableIPCode = string.Empty;
            string myProp = string.Empty;
            string AutoInsert = string.Empty;
            string DataFind = string.Empty;
            string DetailSubDetail = string.Empty;
            string DataCopyCode = string.Empty;

            Form tForm = vSW._01_tForm;

            /// Eğer bir TableIPCode gelmişse
            /// ya kendisi için işlem yapılacak, 
            /// ya da kendisine bağlı (Master-Detail ile) childs ları
            string Main_TableIPCode = vSW._02_TableIPCode;

            #endregion Tanımlar

            #region DataNavigator Listesi Hazırlanıyor

            //Control cntrl = new Control();
            //string[] controls = new string[] { "DevExpress.XtraEditors.DataNavigator" };
            //List<string> list = new List<string>();
            //t.Find_Control_List(tForm, list, controls);

            List<string> list = new List<string>();
            t.Find_DataNavigator_List(tForm, ref list);

            Control cntrl = new Control();
            string[] controls = new string[] { "DevExpress.XtraEditors.DataNavigator" };

            #endregion

            #region DataNavigator Listesi

            foreach (string value in list)
            {
                cntrl = t.Find_Control(tForm, value, "", controls);
                if (cntrl != null)
                {
                    onay = true;
                    DataNavigator dN = (DevExpress.XtraEditors.DataNavigator)cntrl;
                        
                    if ((dN.Name.IndexOf("CatList") > -1) ||
                        (dN.Name.IndexOf("CatDetail") > -1))
                    {
                        onay = false;
                    }
                                    
                    TableIPCode = dN.AccessibleName.ToString();

                    if ((vSW._04_WorkWhom == v.tWorkWhom.Childs) &&
                        (Main_TableIPCode != TableIPCode) &&
                        (Main_TableIPCode != "")
                        ) onay = false;

                    if (dN.DataSource == null)
                        onay = false;

                    if (onay)
                    {
                        object tDataTable = dN.DataSource;
                        DataSet dsData = ((DataTable)tDataTable).DataSet;

                        myProp = dsData.Namespace;
                        AutoInsert = t.MyProperties_Get(myProp, "AutoInsert:");
                        DataFind = t.MyProperties_Get(myProp, "DataFind:");
                        DetailSubDetail = t.MyProperties_Get(myProp, "DetailSubDetail:");
                        DataCopyCode = t.MyProperties_Get(myProp, "DataCopyCode:");

                        #region tWorkTD.Save - tDataSave 
                        if (vSW._03_WorkTD == v.tWorkTD.Save)
                        {
                            //
                            tSave sv = new tSave();
                            //
                            old_PositionChange = v.con_PositionChange;
                            v.con_PositionChange = true;
                            //
                            sv.tDataSave(tForm, TableIPCode);
                            //
                            if (old_PositionChange == false)
                                v.con_PositionChange = false;
                        }
                        #endregion tDataSave

                        #region Refresh_SubDetail
                        if ((vSW._03_WorkTD == v.tWorkTD.Refresh_SubDetail) ||
                            (vSW._03_WorkTD == v.tWorkTD.NewAndRef))
                        {
                            // önce View/Show işi yapılıyor 
                            // tSubView_Refresh(dsData, dN);
                            
                            //if (dN.Position > -1)
                                tSubWork_Refresh_(tForm, dsData, dN);
                        }
                        #endregion Refresh_SubDetail

                        #region NewData
                        if ((vSW._03_WorkTD == v.tWorkTD.NewData) ||
                            (vSW._03_WorkTD == v.tWorkTD.NewAndRef))
                        {
                            if (t.IsNotNull(v.con_Source_ParentControl_Tag_Value))
                                Source_ParentControl_Tag_Value(tForm, TableIPCode);
                            
                            if (dsData.Tables[0].Rows.Count == 0)
                            {
                                /// bu tablolar Master-Detail ile bir yere bağlı olmayan tablolardır
                                /// DataFind   /* 0= Find yok, 1. standart, 2. List&Data   */

                                //v.con_ExtraChange = true;

                                //if ((DetailSubDetail != "True") && (DataFind == "0")) 
                                if ((DataFind == "0") &&
                                    (AutoInsert == "True"))
                                {
                                    old_PositionChange = v.con_PositionChange;
                                    v.con_PositionChange = true;

                                    tNewData(tForm, dsData, dN, TableIPCode, "", "", "");

                                    if (old_PositionChange == false)
                                        v.con_PositionChange = false;

                                    /// DataCopyCode bilgisi dragdrop sırasında 
                                    /// ( work_type = 5 ) sırasında atanıyor
                                    /// 
                                    #region
                                    if (t.IsNotNull(DataCopyCode))
                                    {
                                        // tDC_Run gidince oradaki 
                                        // if ((work_type == 3) && (v.con_DragDropEdit == false))  işlemine başlamasın
                                        v.con_DragDropEdit = true;
                                        //***

                                        /// şimdi buda nerden çıktı deme
                                        /// dataCopy çalışınca targetIP için kendi üzerindeki bakar 
                                        /// fakat aynı dataCopy i birden fazla formlar ve onların üzerindeki targetIP ler içinde kullanabiliriz
                                        /// bu nedenle yeni açılan formun üzerindeki tableIP alırız TargetIP yaparız
                                        /// böylelikle bir dataCopy tanımlarıyla birden fazla form (work_type = 5) için kullanmış oluruz :) nasıl ?
                                        /// 
                                        
                                        v.con_DragDropOpenFormInTableIPCode = TableIPCode;

                                        tDataCopy dc = new tDataCopy();
                                        dc.tDC_Run(tForm, DataCopyCode);
                                        //dc.tDC_Run(tForm, v.SP_Conn_Proje_MSSQL, DataCopyCode);

                                        v.con_DragDropEdit = false;
                                        //***
                                    }
                                    #endregion
                                }
                            }

                            //= DetailSubDetail:True;
                            //About_Detail_SubDetail:
                            //= Detail_SubDetail:AVI_DOS.AVI_DOS_05 || ID ||[AVI_DCK].ICRA_DOSYA_ID || 56 || 31 || 0 |||||| 578 | ds |;
                            ////if (DetailSubDetail == "True")
                            ////{
                            ////    ViewControl_Enabled(tForm, dsData, TableIPCode);
                            ////}

                        }
                        #endregion NewData

                        if (vSW._03_WorkTD == v.tWorkTD.Refresh_Data)
                        {
                            t.TableRefresh(tForm, dsData, TableIPCode);
                            // 15.5.2018
                            t.ViewControl_Enabled(tForm, dsData, TableIPCode);
                            // bu IPCode bağlı ExternalIPCode olabilir...
                            t.ViewControl_Enabled_ExtarnalIP(tForm, dsData);
                        }

                        // 15.05.2018
                        //if (DataFind == "0")
                        //{
                        //    t.ViewControl_Enabled(tForm, dsData, TableIPCode);
                        //    // bu IPCode bağlı ExternalIPCode olabilir...
                        //    t.ViewControl_Enabled_ExtarnalIP(tForm, dsData);
                        //}

                    }// if onay

                } // if cntrl != null
            }//foreach
            
            #endregion DataNavigator Listesi

        }
        
        //public void tSubDetail_Refresh(DataSet ds_Master, DataNavigator dN_Master)
        public void tSubWork_Refresh_(Form tForm, DataSet dsData, DataNavigator dN)
        {
            /// SubDetail_TableIPCode
            /// 
            /// Master-Detail ile bu tabloya bağlı olan  
            /// SubDetail listesi sırayla refresh olacak

            if (v.con_SubWork_Refresh == false) return;

            string SubDetail_List = dN.Text;

            if ((SubDetail_List == string.Empty) ||
                (SubDetail_List == "")) return;

            tToolBox t = new tToolBox();

            if (tForm == null)
                tForm = t.Find_Form(dN);

            int position = dN.Position;
            string SubDetail_TableIPCode = string.Empty;
            string TableIPCode = dN.AccessibleName;

            //v.sayac++;
            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";" + v.sayac.ToString();// + ";" + TableIPCode;

            v.con_PositionChange = true;

            while (SubDetail_List.IndexOf("SubDetail_TableIPCode:") > -1)
            {
                SubDetail_TableIPCode = t.Get_And_Clear(ref SubDetail_List, ";");
                SubDetail_TableIPCode = t.MyProperties_Get(SubDetail_TableIPCode, "SubDetail_TableIPCode:");

                tSubDetail_Read(tForm, dsData, dN, TableIPCode, SubDetail_TableIPCode);
            }

            v.con_PositionChange = false;
        }

        #region Detail_SubDetail_Read

        public void tSubDetail_Refresh(DataSet ds_Master, DataNavigator dN_Master)
        {
            tToolBox t = new tToolBox();

            Form tForm = t.Find_Form(dN_Master);
            int position = dN_Master.Position;
            string SubDetail_List = dN_Master.Text;
            string SubDetail_TableIPCode = string.Empty;
            string mst_TableIPCode = dN_Master.AccessibleName;

            // Master/Detail Tabloya bağlı olan  SubDetail listesi sırayla refresh olacak
            // Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";sdR1;" + mst_TableIPCode;
            while (SubDetail_List.IndexOf("SubDetail_TableIPCode:") > -1)
            {
                SubDetail_TableIPCode = t.Get_And_Clear(ref SubDetail_List, ";");
                SubDetail_TableIPCode = t.MyProperties_Get(SubDetail_TableIPCode, "SubDetail_TableIPCode:");

                tSubDetail_Read(tForm, ds_Master, dN_Master, mst_TableIPCode, SubDetail_TableIPCode);
            }
        }

        public void tSubDetail_Read(Form tForm,
            DataSet ds_Master,
            DataNavigator dN_Master,
            string mst_TableIPCode,
            string SubDetail_TableIPCode)
        {
            tToolBox t = new tToolBox();
            //string function_name = "tSubDetail_Read";

            #region Tanımlar

            // yeniden okunacak SubDetail dataseti bul
            //DataSet dsSubDetail_Data = t.Find_DataSet(tForm, "", SubDetail_TableIPCode, function_name);
            DataSet dsSubDetail_Data = null;
            DataNavigator dN_SubDetail = null;
            t.Find_DataSet(tForm, ref dsSubDetail_Data, ref dN_SubDetail, SubDetail_TableIPCode);


            if (dsSubDetail_Data == null)
            {
                v.Kullaniciya_Mesaj_Var =
                    "DİKKAT : " + SubDetail_TableIPCode + " için DataSet tespit edilemedi...";
                return;
            }

            if (dsSubDetail_Data.Tables[0].CaseSensitive == true)
                dsSubDetail_Data.Tables[0].CaseSensitive = false;

            string myProp = dsSubDetail_Data.Namespace.ToString();

            int DataReadType = t.myInt32(t.MyProperties_Get(myProp, "DataReadType:"));

            string SubDetail_List = t.MyProperties_Get(myProp, "About_Detail_SubDetail:");
            string AutoInsert = t.MyProperties_Get(myProp, "AutoInsert:");

            string SqlF = t.MyProperties_Get(myProp, "SqlFirst:");
            string SqlS = t.MyProperties_Get(myProp, "SqlSecond:");
            string Sql_OldF = SqlF;
            string Sql_OldS = SqlS;

            #endregion Tanımlar

            if ((Sql_OldF != "") && (Sql_OldS == ""))
            {
                SubDetail_Preparing(tForm, ref SqlF,
                                    ds_Master, dN_Master, //mst_TableIPCode,
                                    dsSubDetail_Data, SubDetail_List, SubDetail_TableIPCode,
                                    DataReadType, "", "");

                SubDetail_Run(tForm, dsSubDetail_Data, myProp,
                    SqlF, Sql_OldF, SqlS, Sql_OldS,
                    SubDetail_TableIPCode, DataReadType);
            }

            if (Sql_OldS != "")
            {
                SubDetail_Preparing(tForm, ref SqlS,
                                    ds_Master, dN_Master, //mst_TableIPCode,
                                    dsSubDetail_Data, SubDetail_List, SubDetail_TableIPCode,
                                    DataReadType, "", "");

                SubDetail_Run(tForm, dsSubDetail_Data, myProp,
                    SqlF, Sql_OldF, SqlS, Sql_OldS,
                    SubDetail_TableIPCode, DataReadType);
            }

            if (AutoInsert == "True")
            {
                if (ds_Master == null)
                {
                    if (dsSubDetail_Data.Tables[0].Rows.Count == 0)
                        tNewData(tForm, SubDetail_TableIPCode);
                }
                else
                {
                    if ((ds_Master.Tables[0].Rows.Count > 0) &&
                        (dsSubDetail_Data.Tables[0].Rows.Count == 0))
                        tNewData(tForm, SubDetail_TableIPCode);
                }
            }

        }

        private bool SubDetail_MasterIDValueChecked(Form tForm, string SubDetail_List)
        {
            tToolBox t = new tToolBox();

            #region Tanımlar
            bool onay = true;
            string satir = string.Empty;
            string read_mst_TableIPCode = string.Empty;
            string read_mst_FName = string.Empty;
            string read_sub_FName = string.Empty;
            string read_field_type = string.Empty;
            string OperandType = string.Empty;
            string read_RefId = string.Empty;
            string read_mst_value = string.Empty;
            string mst_TableIPCode = string.Empty;
            string mst_CheckFName = string.Empty;
            string mst_CheckValue = string.Empty;
            byte default_type = 0;
            bool value = true;
            #endregion Tanımlar

            //= DetailSubDetail:True;
            //About_Detail_SubDetail:
            //= Detail_SubDetail: AVI_DOS.AVI_DOS_05       || ID           || [AVI_DCK].ICRA_DOSYA_ID          || 56  || 31 || 0 |||||| 578  | ds |;
            //= Detail_SubDetail: 3S_MSTBLIP.3S_MSTBLIP_02 || @TABLEIPCODE || [3S_MSTBLIP_VWO].LKP_TABLEIPCODE || 167 || 0  || 0 |||||| 1065 | ds |;
            #region read values
            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";sdR2;" + mst_TableIPCode;
            while (SubDetail_List.IndexOf("Detail_SubDetail:") > -1)
            {
                satir = t.Get_And_Clear(ref SubDetail_List, "|ds|") + "||";

                t.Get_And_Clear(ref satir, "Detail_SubDetail:");

                //=Detail_SubDetail:3S_MSTBLIP.3S_MSTBLIP_02||@TABLEIPCODE||[3S_MSTBLIP_VWO].LKP_TABLEIPCODE||167||0||0||||||1065|ds|

                // okunacak field
                read_mst_TableIPCode = t.Get_And_Clear(ref satir, "||");
                read_mst_FName = t.Get_And_Clear(ref satir, "||");
                // atanacak field
                read_sub_FName = t.Get_And_Clear(ref satir, "||");
                read_field_type = t.Get_And_Clear(ref satir, "||");
                // --
                default_type = System.Convert.ToByte(t.Get_And_Clear(ref satir, "||"));
                OperandType = t.Get_And_Clear(ref satir, "||");
                mst_CheckFName = t.Get_And_Clear(ref satir, "||");
                mst_CheckValue = t.Get_And_Clear(ref satir, "||");
                read_RefId = t.Get_And_Clear(ref satir, "||");

                if (t.IsNotNull(read_mst_TableIPCode))
                    read_mst_value = t.Find_TableIPCode_Value(tForm, read_mst_TableIPCode, read_mst_FName);

                /// tüm verileri (master-detail) kontrol edilecek
                /// bir defa false gelse hepsi için false dönecek
                /// 
                value = t.Get_ValueTrue(t.myInt32(read_field_type), read_mst_value);

                if (value == false)
                    onay = false;
            }
            #endregion

            return onay;
        }

        private void SubDetail_Preparing(Form tForm, ref string Sql,
                   DataSet ds_Master, DataNavigator dN_Master,
                   DataSet dsSubDetail_Data,
                   string SubDetail_List, string SubDetail_TableIPCode,
                   int DataReadType, string Speed_FName, string Speed_Value)
        {
            tToolBox t = new tToolBox();

            #region Tanımlar
            string satir = string.Empty;
            string new_And = string.Empty;
            string read_mst_TableIPCode = string.Empty;
            string read_mst_FName = string.Empty;
            string read_sub_FName = string.Empty;
            string read_sub_FName2 = string.Empty;
            //string read_sub_FName3 = string.Empty;
            string read_field_type = string.Empty;
            string OperandType = string.Empty;
            string read_RefId = string.Empty;
            string read_mst_value = string.Empty;
            string mst_TableIPCode = string.Empty;
            string mst_CheckFName = string.Empty;
            string mst_CheckValue = string.Empty;
            string Operand = string.Empty;
            string clean_TableIPCode = string.Empty;

            string str_bgn = string.Empty;
            string str_end = string.Empty;
            int i_bgn = 0;
            int i_end = 0;
            int f_bgn = 0;
            int f_end = 0;

            int ga1 = -1; // @ güzel a
            int ga2 = -1; // @ güzel a
            int pos = 0;
            byte default_type = 0;
            byte buldu = 0;

            DataRow mst_Row = null;
            #endregion Tanımlar

            // yeni setfocus olan Master satırın bilgileri alınıyor

            mst_TableIPCode = t.Find_TableIPCode(dN_Master);

            if (t.IsNotNull(ds_Master))
            {
                mst_Row = ds_Master.Tables[0].Rows[dN_Master.Position] as DataRow;
            }

            if (t.IsNotNull(mst_TableIPCode) == false)
                mst_TableIPCode = SubDetail_TableIPCode;

            /// Konuyla ilgili ÖRNEKLER
            /// 
            /// SubDetail table ait About_Detail_SubDetail listesi okunarak 
            /// sırayla Master/Detail tablodan veriler okunacak ve işlenecek
            /// 
            /// About_Detail_SubDetail:
            /// =Detail_SubDetail:GRP.GRP_03||ID||[K].GRUP_ID||56||32||LKP_ONAY||1||2774|ds|
            /// =Detail_SubDetail:SNVLIST_ADAY.SNVLIST_ADAY_01||||[K].E_SINAV||104||3||E_SINAV||||2868|ds|
            /// =Detail_SubDetail:SNVLIST_ADAY.SNVLIST_ADAY_01||||[K].OGRDURUM||52||1||OGRDURUM||||2772|ds|
            /// =Detail_SubDetail:FNLNVOL.FNLNVOL_XYL01||||[FNLNVOL].ISLEM_TRHS||58||0||3||ISLEM_TRHS||||3827|ds|
            ///
            /// About_Detail_SubDetail:
            /// =Detail_SubDetail:VTSNL_01.VTSNL_01_01 || PART_ID ||[VRSNL_09].@partId || 56 || 31 || 0 |||||| 1434 | ds |
            /// =Detail_SubDetail:|||| @yil || 56 || 53 || 0 |||||| 2002 | ds |
            /// =Detail_SubDetail:|||| @ay || 56 || 53 || 0 |||||| 2003 | ds |
            ///
            /// 'KRT_OPERAND_TYPE',    0,  ''
            /// 'KRT_OPERAND_TYPE',    1,  'Even (Double)'
            /// 'KRT_OPERAND_TYPE',    2,  'Odd  (Single)'
            /// 'KRT_OPERAND_TYPE',    3,  'Speed (Double)'
            /// 'KRT_OPERAND_TYPE',    4,  'Speed (Single)'
            /// 'KRT_OPERAND_TYPE',    5,  'On/Off'
            /// 'KRT_OPERAND_TYPE',    9,  'Visible=False'
            /// 'KRT_OPERAND_TYPE',   11,   > =
            /// 'KRT_OPERAND_TYPE',   12,   >
            /// 'KRT_OPERAND_TYPE',   13,   K = 
            ///  KRT_OPERAND_TYPE ,   14,   K 
            ///  KRT_OPERAND_TYPE ,   15,   K >
            /// 'KRT_OPERAND_TYPE',   16,  "Benzerleri (%abc%)"
            /// 'KRT_OPERAND_TYPE',   17,  "Benzerleri (abc%)"
            ///  K harfi < 
            ///  

            #region SubDetail_List Read
            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";sdR3;" + mst_TableIPCode;
            while (SubDetail_List.IndexOf("Detail_SubDetail:") > -1)
            {
                #region read values
                satir = t.Get_And_Clear(ref SubDetail_List, "|ds|") + "||";

                t.Get_And_Clear(ref satir, "Detail_SubDetail:");

                //=Detail_SubDetail:3S_MSTBLIP.3S_MSTBLIP_02||@TABLEIPCODE||[3S_MSTBLIP_VWO].LKP_TABLEIPCODE||167||0||0||||||1065|ds|

                new_And = string.Empty;
                // okunacak field
                read_mst_TableIPCode = t.Get_And_Clear(ref satir, "||");
                read_mst_FName = t.Get_And_Clear(ref satir, "||");
                // atanacak field
                // [HPCARI].ID
                read_sub_FName = t.Get_And_Clear(ref satir, "||");
                // HPCARI.ID
                read_sub_FName2 = read_sub_FName.Replace("[", "");
                read_sub_FName2 = read_sub_FName2.Replace("]", "");
                // ID
                //read_sub_FName3 = t.Alias_Clear(read_sub_FName);
                // --
                read_field_type = t.Get_And_Clear(ref satir, "||");
                // --
                default_type = System.Convert.ToByte(t.Get_And_Clear(ref satir, "||"));
                OperandType = t.Get_And_Clear(ref satir, "||");
                mst_CheckFName = t.Get_And_Clear(ref satir, "||");
                mst_CheckValue = t.Get_And_Clear(ref satir, "||");
                read_RefId = t.Get_And_Clear(ref satir, "||");

                if (OperandType == "=") Operand = " = ";
                if (OperandType == "0") Operand = " = ";
                if (OperandType == "2") Operand = " = "; // Odd  (Single) 
                if (OperandType == "11") Operand = " >= ";
                if (OperandType == "12") Operand = " > ";
                if (OperandType == "13") Operand = " <= ";
                if (OperandType == "14") Operand = " < ";
                if (OperandType == "15") Operand = " <> ";
                #endregion set

                // eğer sql de -99 var ise sql boşu boşuna çalışmasın 
                // eğer value  -98 ise bulunduğu and koşulu iptal edilecek

                // MultiPageID var ise
                // FNLNVOL.FNLNVOL_BNL01 == FNLNVOL.FNLNVOL_BNL01|23
                // if (read_mst_TableIPCode == mst_TableIPCode)

                clean_TableIPCode = mst_TableIPCode;
                if (mst_TableIPCode.IndexOf("|") > -1)
                {
                    clean_TableIPCode = mst_TableIPCode.Substring(0, mst_TableIPCode.IndexOf("|"));
                }
                //---

                if (read_mst_TableIPCode == clean_TableIPCode)
                {
                    // Set @xxxx   mı yoksa  and GCB_ID = x  mı    
                    ga1 = read_mst_FName.IndexOf("@");
                    ga2 = read_sub_FName.IndexOf("@");

                    if (ga1 > -1)
                        read_mst_FName = read_mst_FName.Substring(ga1, read_mst_FName.Length - ga1);
                    if (ga2 > -1)
                        read_sub_FName = read_sub_FName.Substring(ga2, read_sub_FName.Length - ga2);

                    // normal Where in altındaki and ile başlayan bağlantılar için

                    #region SpeedKriter
                    if (t.IsNotNull(Speed_FName) &&
                        ((OperandType == "3") || // SpeedKriter Double
                         (OperandType == "4"))   // SpeedKriter Single
                        )
                    {
                        // SpeedKriter Double
                        if (OperandType == "3")
                        {
                            /// read_field_type = 58
                            /// and Convert(Date, [AJLNTLK].REC_DATE, 103) >= Convert(Date, '25.12.2016', 103)--:D.SD.2809:-->=
                            /// and Convert(Date, [AJLNTLK].REC_DATE, 103) <= Convert(Date, '25.12.2016', 103)--:D.SD.2809:--<=

                            /// >= işlemleri
                            if (Speed_FName.IndexOf("_BAS") > -1)
                            {
                                str_bgn = " and " + read_sub_FName + "  >=";
                                if ((read_field_type == "40") ||
                                    (read_field_type == "58"))
                                {
                                    if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                                        str_bgn = " and Convert(Date, " + read_sub_FName + ", 103)  >=";
                                    if (v.active_DB.projectDBType == v.dBaseType.MySQL)
                                        str_bgn = " and Convert(" + read_sub_FName + ", Date)  >=";
                                }

                                i_bgn = Sql.IndexOf(str_bgn);

                                str_end = "   -- :D.SD." + read_RefId + ": -->=";
                                i_end = Sql.IndexOf(str_end);

                                new_And = " and " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), read_sub_FName, Speed_Value, "and", ">=");
                            }

                            /// <= işlemleri
                            if (Speed_FName.IndexOf("_BIT") > -1)
                            {
                                str_bgn = " and " + read_sub_FName + "  <=";
                                if ((read_field_type == "40") ||
                                    (read_field_type == "58"))
                                {
                                    if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                                        str_bgn = " and Convert(Date, " + read_sub_FName + ", 103)  <=";
                                    if (v.active_DB.projectDBType == v.dBaseType.MySQL)
                                        str_bgn = " and Convert(" + read_sub_FName + ", Date)  <=";
                                }

                                i_bgn = Sql.IndexOf(str_bgn);

                                str_end = "   -- :D.SD." + read_RefId + ": --<=";
                                i_end = Sql.IndexOf(str_end);

                                new_And = " and " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), read_sub_FName, Speed_Value, "and", "<=");
                            }

                            ///------
                            /// Stored Procedures için hazırlık biraz aşağıda devam ediyor
                        }

                        // SpeedKriter Single
                        if (OperandType == "4")
                        {
                            /// read_field_type = 58
                            /// and Convert(Date, [AJLNTLK].REC_DATE, 103)  =  Convert(Date, '25.12.2016', 103)     -- :D.SD.2809: --

                            str_bgn = " and " + read_sub_FName;
                            if (read_field_type == "58")
                            {
                                if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                                    str_bgn = " and Convert(Date, " + read_sub_FName;
                                if (v.active_DB.projectDBType == v.dBaseType.MySQL)
                                    str_bgn = " and Convert(" + read_sub_FName + ", Date";
                            }

                            i_bgn = Sql.IndexOf(str_bgn);

                            str_end = "   -- :D.SD." + read_RefId + ": --";
                            i_end = Sql.IndexOf(str_end);

                            new_And = " and " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), read_sub_FName, Speed_Value, "and", OperandType);
                        }

                        // Stored Procedures için hazırlık burada
                        if ((Sql.IndexOf("set @" + Speed_FName) > -1) &&
                            (Speed_FName.IndexOf(mst_CheckFName) > -1))
                        {
                            // OperandType == "3" olunca Speed_FName xxx_BAS ve xxx_BIT şeklinde geliyor
                            // OperandType == "4" olunca Speed_FName == mst_CheckFName şeklinde

                            /// Aşağıdaki sql manuel hazırlanıyor
                            /// xxx_BAS ve xxx_BIT değişkenleri 
                            /// SpeedKriterde field olarak kullanılan isim 
                            /// @fieldName_BAS ve @fieldName_BIT  şeklinde hazırlanır
                            /// NOT : {baslamaTarihi} ve {bitisTarihi}  değişkenleri SQL_Preparing içinde tanımlı

                            /// declare @ISLEM_TRHS_BAS date
                            /// declare @ISLEM_TRHS_BIT date
                            /// set @ISLEM_TRHS_BAS = {baslamaTarihi}  
                            /// set @ISLEM_TRHS_BIT = {bitisTarihi}
                            /// EXEC [dbo].[prc_FN_LINEVOL_FULLHESAP]
                            ///  @baslamaTarihi = @ISLEM_TRHS_BAS,
                            ///  @bitisTarihi = @ISLEM_TRHS_BIT

                            str_bgn = "set @" + Speed_FName + " = ";

                            i_bgn = Sql.IndexOf(str_bgn);

                            str_end = "\r\n";
                            i_end = Sql.IndexOf(str_end, i_bgn);

                            new_And = "set " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), Speed_FName, Speed_Value, "@", "=");
                        }

                    }
                    #endregion

                    #region and [a].GCB = x   değişimi
                    if ((ga1 == -1) && (ga2 == -1) && (default_type == 31))
                    {
                        str_bgn = " and " + read_sub_FName + Operand;
                        i_bgn = Sql.IndexOf(str_bgn);

                        if (i_bgn == -1)
                        {
                            str_bgn = " and " + read_sub_FName2 + Operand;
                            i_bgn = Sql.IndexOf(str_bgn);
                        }

                        if (i_bgn == -1)
                        {
                            str_bgn = " -- and " + read_sub_FName + Operand;
                            i_bgn = Sql.IndexOf(str_bgn);
                        }

                        if (i_bgn == -1)
                        {
                            // and Convert(Date, [SYSOTV].TARIH, 103)  = Convert(Date,'01.01.1900', 103)    -- :D.SD.5839: --
                            if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                                str_bgn = " and Convert(Date, " + read_sub_FName;
                            if (v.active_DB.projectDBType == v.dBaseType.MySQL)
                                str_bgn = " and Convert(" + read_sub_FName + ", Date";

                            i_bgn = Sql.IndexOf(str_bgn);
                        }

                        str_end = "   -- :D.SD." + read_RefId + ": --";
                        i_end = Sql.IndexOf(str_end);

                        if (mst_Row != null)
                        {
                            read_mst_value = mst_Row[read_mst_FName].ToString();
                        }
                        else
                        {
                            read_mst_value = "-1";
                        }

                        if (read_mst_value != "-98")
                        {
                            new_And = " and " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), read_sub_FName, read_mst_value, "and", OperandType);
                        }

                        // satırı geçici iptal etmek için
                        if (read_mst_value == "-98")
                        {
                            new_And = " -- and " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), read_sub_FName, read_mst_value, "and", OperandType);
                        }
                    }
                    #endregion

                    #region /*prm*/ @partId = -98  veya  set @partId = -98
                    if (((ga2 > -1) ||
                         (OperandType == "2")) && // Odd  (Single)
                        (default_type == 31))     // master-detail 
                    {
                        // EXEC [dbo].[LST_VR_VARDIYA_IOLN_GCF]
                        //  /*prm*/ @partId = -98   -- :D.SD.2004: --

                        // Stored Procedures fieldlerinde kriter_FName, kriter_Operand_Type (odd single) olunca
                        // ve kriter_FName de @ işareti koymaya gerek kalmadı
                        // yalınız kriter fieldname parametre ismiyle aynı olsun 
                        if (ga2 == -1)
                        {
                            read_sub_FName = "@" + t.Alias_Clear(read_sub_FName);
                            // alttaki yorumlara takılmasın diye
                            ga2 = 1;
                        }

                        // MSSQL için
                        str_bgn = "  /*prm*/ " + read_sub_FName + Operand;
                        i_bgn = Sql.IndexOf(str_bgn);
                        buldu = 0;

                        if (i_bgn == -1)
                        {
                            str_bgn = ", /*prm*/ " + read_sub_FName + Operand;
                            i_bgn = Sql.IndexOf(str_bgn);
                            buldu = 1;
                        }


                        // MySQL için
                        if (i_bgn == -1)
                        {
                            str_bgn = "  /*prm." + read_sub_FName + "*/";
                            i_bgn = Sql.IndexOf(str_bgn);
                            buldu = 0;
                        }

                        if (i_bgn == -1)
                        {
                            str_bgn = ", /*prm." + read_sub_FName + "*/ ";
                            i_bgn = Sql.IndexOf(str_bgn);
                            buldu = 1;
                        }


                        // MSSQL için 
                        if (i_bgn == -1)
                        {
                            //set @yil = 2016  
                            //set @ay = 4
                            //set @partId = -98
                            str_bgn = "set " + read_sub_FName + Operand;
                            i_bgn = Sql.IndexOf(str_bgn);
                            buldu = 2;
                        }

                        str_end = "   -- :D.SD." + read_RefId + ": --";
                        i_end = Sql.IndexOf(str_end);

                        if ((i_end == -1) && (i_bgn > -1))
                        {
                            //i_end == -1 ise satır sonunu bulalım
                            for (int i = i_bgn; i < Sql.Length; i++)
                            {
                                if (Sql[i] == '\r') { i_end = i; break; }
                            }
                        }

                        if (mst_Row != null)
                        {
                            read_mst_value = mst_Row[read_mst_FName].ToString();
                        }
                        else
                        {
                            read_mst_value = "0";
                        }

                        // MSSQL ise
                        if (Sql.IndexOf("EXEC") > -1)
                        {
                            if (buldu == 0)
                                new_And = "  /*prm*/ " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), read_sub_FName, read_mst_value, "and", OperandType);
                            if (buldu == 1)
                                new_And = ", /*prm*/ " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), read_sub_FName, read_mst_value, "and", OperandType);
                            if (buldu == 2)
                                new_And = "set " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), read_sub_FName, read_mst_value, "and", OperandType);
                        }
                        // MySQL ise
                        if (Sql.IndexOf("CALL") > -1)
                        {
                            if (buldu == 0)
                                new_And = "  /*prm." + read_sub_FName + "*/ " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), "", read_mst_value, "and", "null");
                            if (buldu == 1)
                                new_And = ", /*prm." + read_sub_FName + "*/ " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), "", read_mst_value, "and", "null");

                            //if (buldu == 2)
                            //    new_And = "set " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), read_sub_FName, read_mst_value, "and", OperandType);
                        }


                    }
                    #endregion

                    #region default_type == 51, 52, 53
                    if ((ga1 == -1) && (ga2 == -1) &&
                        ((default_type == 51) || (default_type == 52) || (default_type == 53)
                         ))
                    {
                        //EXEC [dbo].[prc_FINANS_BAKIYE]
                        //  /*prm*/ @IslemTarihi_Bas = '15.11.2015'    -- :D.SD.4922: --
                        //, /*prm*/ @IslemTarihi_Bit = '15.11.2015'    -- :D.SD.4923: --

                        str_bgn = "/*prm*/ " + read_sub_FName + Operand;
                        i_bgn = Sql.IndexOf(str_bgn);

                        //if (i_bgn == -1)
                        //{
                        //    str_bgn = "/*prm*/ " + read_sub_FName3 + Operand;
                        //    i_bgn = Sql.IndexOf(str_bgn);
                        //}

                        str_end = "   -- :D.SD." + read_RefId + ": --";
                        i_end = Sql.IndexOf(str_end);

                        read_mst_value = t.Find_Kriter_Value(tForm, read_mst_TableIPCode, read_mst_FName, default_type);
                        if (read_mst_value == "") read_mst_value = "0";

                        if (read_mst_value != "-98")
                        {
                            new_And = "/*prm*/ " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), read_sub_FName, read_mst_value, "and", OperandType);
                        }
                    }
                    #endregion

                    #region and [a].GCB in ( x1, x2 ... ) değişimi
                    if ((ga1 == -1) && (default_type == 32))
                    {
                        str_bgn = "/*" + read_sub_FName + "_INLIST*/";
                        str_end = "/*" + read_sub_FName + "_INEND*/";
                        i_bgn = Sql.IndexOf(str_bgn) + str_bgn.Count() + 2;
                        i_end = Sql.IndexOf(str_end);

                        read_mst_value = Preparing_Select_Value_List(ds_Master, read_mst_FName, mst_CheckFName, mst_CheckValue);
                        new_And = read_mst_value + v.ENTER;

                        /*
                             and [K].GRUP_ID in (   -- :D.SD.2774: --
                             / *[K].GRUP_ID_INLIST* /
                               2990
                             , 2983
                             / *[K].GRUP_ID_INEND* /
                             )
                       */

                    }
                    #endregion

                    #region  Set @GCB_ID = x    değişimi
                    // Declare @xxxxx  ve  Set @xxxxx ifadeleri için 
                    if ((ga1 > -1) && (ga2 == -1))
                    {
                        // atama yapılcak field
                        // read_sub_FName 
                        // read_field_type 

                        read_sub_FName = t.Alias_Clear(read_sub_FName);

                        //str_bgn = "set " + read_mst_FName;
                        str_bgn = "set @" + read_sub_FName;
                        str_end = "   -- :D.SD." + read_RefId + ": --";

                        if (mst_Row != null)
                        {
                            //if (read_mst_FName.IndexOf("@") > -1)
                            read_mst_value = mst_Row[read_mst_FName.Substring(1, read_mst_FName.Length - 1)].ToString();
                            //else read_mst_value = mst_Row[read_mst_FName].ToString();
                        }
                        else
                        {
                            read_mst_value = "0";
                        }

                        if (read_mst_value != "-98")
                        {
                            i_bgn = Sql.IndexOf(str_bgn);
                            i_end = Sql.IndexOf(str_end);

                            new_And = "set " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), read_sub_FName, read_mst_value, "@", OperandType);
                            //new_And = "set " + t.Set_FieldName_Value_(System.Convert.ToInt16(read_field_type), read_mst_FName, read_mst_value, "@", OperandType);

                        }
                    }
                    #endregion

                    #region // Data Collection değil ise
                    if (DataReadType != 5)
                    {
                        if ((new_And != "") && (i_bgn >= 0) && (i_end > 0))
                        {
                            //Sql = Sql.Remove(i_bgn, (i_end - i_bgn));
                            //Sql = Sql.Insert(i_bgn, new_And);

                            f_bgn = 0;
                            f_end = 0;
                            //int i1 = Sql.IndexOf(FullHeader1, f1);
                            while ((i_end > f_end) && (f_end > -1))
                            {
                                Sql = Sql.Remove(i_bgn, (i_end - i_bgn));

                                Sql = Sql.Insert(i_bgn, new_And);

                                f_bgn = Sql.IndexOf(str_bgn, i_bgn + new_And.Length);
                                f_end = Sql.IndexOf(str_end, i_end + 2);// new_And.Length);

                                if ((f_bgn > 0) && (f_end > 0))
                                {
                                    i_bgn = f_bgn;
                                    i_end = f_end;
                                    f_end = 0;
                                }
                            }
                        }
                    }
                    #endregion

                    #region // Data Collection ise
                    if (DataReadType == 5)
                    {
                        #region // sql de -99 yok ise Value Atama işlemleri gerçekleşsin
                        if (Sql.IndexOf("-99") == -1)
                        {
                            try
                            {
                                if (t.IsNotNull(dsSubDetail_Data))
                                {
                                    read_sub_FName = read_sub_FName.Substring(read_sub_FName.IndexOf(".") + 1);
                                    pos = t.Find_DataNavigator_Position(tForm, SubDetail_TableIPCode);
                                    dsSubDetail_Data.Tables[0].Rows[pos][read_sub_FName] = read_mst_value;
                                }
                            }
                            catch (Exception)
                            {
                                //..
                                throw;
                            }
                        }
                        #endregion

                        #region // sql de -99 varsa onlarda silinsin
                        if (Sql.IndexOf("-99") > -1)
                        {
                            if ((new_And != "") && (i_bgn >= 0) && (i_end > 0))
                            {
                                Sql = Sql.Remove(i_bgn, (i_end - i_bgn));
                                Sql = Sql.Insert(i_bgn, new_And);
                            }
                        }
                        #endregion
                    }
                    #endregion // Data Collection ise

                }
            }
            #endregion SubDetail_List Read
        }


        private void SubDetail_Run(Form tForm, DataSet dsSubDetail_Data,
                                   string myProp,
                                   string SqlF, string Sql_OldF, string SqlS, string Sql_OldS,
                                   string TableIPCode, int DataReadType)
        {

            #region Sonuçları işle ve Gerekirse dsSubDetail i yeniden oku ( t.Data_Read_Execute )

            tToolBox t = new tToolBox();

            // yeni SQLi SqlFirste yerleştir 
            t.Str_Replace(ref myProp, Sql_OldF, SqlF);

            // yeni SQLi SqlSecond yerleştir 
            if (Sql_OldS != "")
                t.Str_Replace(ref myProp, Sql_OldS, SqlS);

            // yerine ata
            dsSubDetail_Data.Namespace = myProp;

            string Sql = string.Empty;

            if (Sql_OldS != "")
                Sql = SqlS;
            else Sql = SqlF;

            // Sql içinde Detail-SubDetail e bağlı atama kalmadığı zaman
            // SubDetail i yeniden çalıştırabiliriz.
            if (Sql.IndexOf("-99") == -1)
            {
                Control cntrl = null;
                cntrl = t.Find_Control_View(tForm, TableIPCode);

                // Data Collection değilse
                if (DataReadType != 5)
                {
                    //
                    //v.SQLSave = v.ENTER2 + TableIPCode + v.ENTER + Sql + v.SQLSave;
                    v.SQLSave = v.ENTER + TableIPCode + v.SQLSave;
                    //---

                    //
                    t.Data_Read_Execute(dsSubDetail_Data, ref Sql, "", cntrl);

                    /// External_TableIPCode kullanan diğer cntrl leride bulup enabled özelliğini tetiklemek gerekiyor
                    ///

                    /// 20.03.18
                    /// yeni deneme şimdilik kapatalım 

                    ///t.External_Controls_Enabled(tForm, dsSubDetail_Data, cntrl);

                    // eğer okunan data boş geliyorsa ve kendine bağlı alt datasetler olabilir

                    // ***   BURASI GEREKMEYE BİLİR DENEME YAPILIYOR
                    ///  burası gerekiyormuşşşşşş
                    ///  okunan data sıfır kayıt olunca positionChange çalışmıyor
                    ///  bu nedenle kendisine bağlı diğer dsDatalarda yeniden refreshlenmeli
                    ///  
                    if (dsSubDetail_Data.Tables[0].Rows.Count == 0)
                    {
                        string SubDetail_List = t.MyProperties_Get(myProp, "About_Detail_SubDetail:");

                        if (t.IsNotNull(SubDetail_List))
                        {
                            DataNavigator dN = null;
                            dN = t.Find_DataNavigator(tForm, TableIPCode);

                            // sonra database okunuyor
                            //tSubDetail_Refresh(dsSubDetail_Data, dN); eski hali

                            /// 20.03.18 kapatalım
                            ///tSubWork_Refresh_(tForm, dsSubDetail_Data, dN);
                        }
                    }

                    // YENİ EKLENDİ
                    t.ViewControl_Enabled(tForm, dsSubDetail_Data, TableIPCode);
                    // bu IPCode bağlı ExternalIPCode olabilir...
                    t.ViewControl_Enabled_ExtarnalIP(tForm, dsSubDetail_Data);
                                        
                }

            }
            #endregion Sonuçları işle ve Gerekirse dsSubDetail i yeniden oku

        }


        private string Preparing_Select_Value_List(DataSet dsData, string read_mst_FName,
                        string mst_CheckFName, string mst_CheckValue)
        {
            tToolBox t = new tToolBox();

            string s = string.Empty;

            if ((t.IsNotNull(dsData) == false) ||
                (t.IsNotNull(read_mst_FName) == false) ||
                (t.IsNotNull(mst_CheckFName) == false) ||
                (t.IsNotNull(mst_CheckValue) == false)
                )
            {
                MessageBox.Show("DİKKAT : Eksik bilgi mevcut ..." + v.ENTER +
                    "Master FieldName : " + read_mst_FName + v.ENTER +
                    "Check  FieldName : " + mst_CheckFName + v.ENTER +
                    "Check  Value : " + mst_CheckValue + v.ENTER2 +
                    " + Master Data olmaya bilir ..."
                    );
                return null;
            }

            int i1 = dsData.Tables[0].Rows.Count;

            for (int i = 0; i < i1; i++)
            {
                if (dsData.Tables[0].Rows[i][mst_CheckFName].ToString() == mst_CheckValue)
                {
                    s = s + " , " + dsData.Tables[0].Rows[i][read_mst_FName].ToString() + v.ENTER;
                }
            }

            if (s.Length > 2) s = s.Remove(0, 2);

            if (t.IsNotNull(s) == false) s = " -1 ";

            return s;
        }

        #endregion Detail_SubDetail_Read



        private void Source_ParentControl_Tag_Value(Form tForm, string TableIPCode)
        {
            tToolBox t = new tToolBox();

            #region v.con_Source_ParentControl_Tag_Value
            /// 
            /// IPCode tanımların 
            /// Default value de ( Source ParentControl Tag READ ) var ise
            /// geçici hafıza ile buraya gelen value yi ilgili Control_View.Parent  in tag ına
            /// yüklemek gerekiyor
            /// Şimdilik bunu çözüm buldum ( İlgili : Banka Hareketleri ekranı ) 
            /// Bu özellikle MultiPageID özelliği kullanılan sayfalarda 
            /// yeni bağımsız fiş ekranı açıldğında gerek duyuluyor
            /// 
            if (t.IsNotNull(v.con_Source_ParentControl_Tag_Value))
            {
                Control vCntrl = t.Find_Control_View(tForm, TableIPCode);// + MultiPageID);
                if (vCntrl != null)
                {
                    Control parent = vCntrl.Parent;
                    if (parent != null)
                    {
                        parent.Tag = v.con_Source_ParentControl_Tag_Value;

                        v.con_Source_ParentControl_Tag_Value = string.Empty;
                    }
                }
            }
            #endregion con_Source_ParentControl_Tag_Value
        }
        
        
        #endregion tSubWork.. 

        #region KULLANILMAYACAK FUNC

        #region tDataNavigatorList
        public void tDataNavigatorList(Form tForm, string WorkType)
        {
            tToolBox t = new tToolBox();

            // WorkType == "tDataSave"
            // WorkType == "tNewData"
            // WorkType == "Detail_SubDetail_Refresh"

            #region Tanımlar

            string TableIPCode = string.Empty;
            //string MultiPageID = string.Empty;

            Control cntrl = new Control();

            string[] controls = new string[] { "DevExpress.XtraEditors.DataNavigator" };
            List<string> list = new List<string>();
            t.Find_Control_List(tForm, list, controls);

            #endregion Tanımlar

            #region DataNavigator Listesi

            foreach (string value in list)
            {
                cntrl = t.Find_Control(tForm, value, "", controls);

                if (cntrl != null)
                {
                    if (((DevExpress.XtraEditors.DataNavigator)cntrl).AccessibleName != null)
                    {
                        TableIPCode = ((DevExpress.XtraEditors.DataNavigator)cntrl).AccessibleName.ToString();

                        if (t.IsNotNull(TableIPCode))
                        {
                            #region tDataSave
                            if (WorkType == "tDataSave")
                            {
                                tSave sv = new tSave();
                                sv.tDataSave(tForm, TableIPCode);

                                // gerekipi germediğini bilmiyorum
                                //ViewControl_Enabled(tForm, dsData, TableIPCode);
                            }
                            #endregion tDataSave

                            #region order
                            if ((WorkType == "tNewData") ||
                                (WorkType == "Detail_SubDetail_Refresh"))
                            {
                                object tDataTable = ((DevExpress.XtraEditors.DataNavigator)cntrl).DataSource;
                                DataSet dsData = ((DataTable)tDataTable).DataSet;

                                string myProp = dsData.Namespace;
                                string AutoInsert = t.MyProperties_Get(myProp, "AutoInsert:");
                                string DetailSubDetail = t.MyProperties_Get(myProp, "DetailSubDetail:");

                                if (dsData != null)  //(t.IsNotNull(dsData))
                                {
                                    #region tNewData
                                    if ((WorkType == "tNewData") && (AutoInsert == "True"))
                                    {
                                        #region v.con_Source_ParentControl_Tag_Value
                                        /// IPCode tanımların 
                                        /// Default value de ( Source ParentControl Tag READ ) var ise
                                        /// geçici hafıza ile buraya gelen value yi ilgili Control_View.Parent  in tag ına
                                        /// yüklemek gerekiyor
                                        /// Şimdilik bunu çözüm buldum ( İlgili : Banka Hareketleri ekranı ) 
                                        /// Bu özellikle MultiPageID özelliği kullanılan sayfalarda 
                                        /// yeni bağımsız fiş ekranı açıldğında gerek duyuluyor
                                        /// 

                                        if (t.IsNotNull(v.con_Source_ParentControl_Tag_Value))
                                        {
                                            Control vCntrl = t.Find_Control_View(tForm, TableIPCode);// + MultiPageID);
                                            if (vCntrl != null)
                                            {
                                                Control parent = vCntrl.Parent;
                                                if (parent != null)
                                                {
                                                    parent.Tag = v.con_Source_ParentControl_Tag_Value;

                                                    v.con_Source_ParentControl_Tag_Value = string.Empty;
                                                }
                                            }
                                        }
                                        #endregion con_Source_ParentControl_Tag_Value

                                        if (dsData.Tables[0].Rows.Count == 0)
                                        {
                                            /// bu tablolar Master-Detail ile bir yere bağlı olmayan tablolardır
                                            if (DetailSubDetail != "True")
                                            {
                                                //dsData.Tables[0].Namespace = "NewRecord";
                                                tNewData(tForm, TableIPCode);
                                            }
                                        }

                                    }
                                    #endregion tNewData

                                    if (WorkType == "tNewData")
                                    {
                                        //= DetailSubDetail:True;
                                        //About_Detail_SubDetail:
                                        //= Detail_SubDetail:AVI_DOS.AVI_DOS_05 || ID ||[AVI_DCK].ICRA_DOSYA_ID || 56 || 31 || 0 |||||| 578 | ds |;
                                        if (DetailSubDetail == "True")
                                        {
                                            //
                                            t.ViewControl_Enabled(tForm, dsData, TableIPCode);
                                            // bu IPCode bağlı ExternalIPCode olabilir...
                                            t.ViewControl_Enabled_ExtarnalIP(tForm, dsData);
                                        }
                                    }

                                    #region Detail_SubDetail_Refresh
                                    if (WorkType == "Detail_SubDetail_Refresh")
                                    {
                                        if (t.IsNotNull(dsData) &&
                                            (((DevExpress.XtraEditors.DataNavigator)cntrl).Position > -1))
                                        {

                                            // önce View/Show işi yapılıyor 

                                            tSubView_Refresh(tForm, dsData, ((DevExpress.XtraEditors.DataNavigator)cntrl));

                                            // sonra database okunuyor
                                            //Application.OpenForms[0].Text = Application.OpenForms[0].Text + ";dnL_sdR1;";

                                            //tSubDetail_Refresh(dsData, ((DevExpress.XtraEditors.DataNavigator)cntrl));
                                            tSubWork_Refresh_(tForm, dsData, ((DevExpress.XtraEditors.DataNavigator)cntrl));
                                        }

                                    }
                                    #endregion Detail_SubDetail_Refresh

                                } // if (dsData != null)
                            } // if ((WorkType ==
                            #endregion order
                        }

                    }
                } // if (cntrl != null)
            }

            #endregion DataNavigator Listesi 


        }
        #endregion tDataNavigatorList

        ////public void Detail_SubDetail_Refresh(Form tForm)
        ////{
        ////    //if (v.con_SubDetail_Refresh == false) return;
        ////    tDataNavigatorList(tForm, "Detail_SubDetail_Refresh");
        ////}


        #endregion KULLANILMAYACAK

    }
}
