using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tkn_Forms;
using Tkn_Report;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_Events
{
    public class tEventsMenu : tBase  
    {
        tEvents ev = new tEvents();
        
        #region MenuEvents

        #region tileNavPane

        public void tTileNavPane_Enter(object sender, EventArgs e)
        {
            ((DevExpress.XtraBars.Navigation.TileNavPane)sender).Appearance.BackColor = v.AppearanceFocusedColor;
            ((DevExpress.XtraBars.Navigation.TileNavPane)sender).Appearance.ForeColor = v.AppearanceTextColor;
        }

        public void tTileNavPane_Leave(object sender, EventArgs e)
        {
            ((DevExpress.XtraBars.Navigation.TileNavPane)sender).Appearance.BackColor =
                ((DevExpress.XtraBars.Navigation.TileNavPane)sender).Appearance.BackColor2;
            ((DevExpress.XtraBars.Navigation.TileNavPane)sender).Appearance.ForeColor =
                ((DevExpress.XtraBars.Navigation.TileNavPane)sender).AppearanceSelected.ForeColor;
        }

        public void tileNavPane_SelectedElementChanged(object sender, DevExpress.XtraBars.Navigation.TileNavElementEventArgs e)
        {
            //
        }

        #endregion

        #region RibbonControl

        public void tRibbonControl_KeyDown(object sender, KeyEventArgs e)
        {
            //v.Kullaniciya_Mesaj_Var = "Ribbon : " + e.KeyCode.ToString();
        }

        public void tRibbonControl_Enter(object sender, EventArgs e)
        {
            //MessageBox.Show("ok");
            //v.ribbonColor = ((DevExpress.XtraBars.Ribbon.RibbonControl)sender).BackColor;
            //((DevExpress.XtraBars.Ribbon.RibbonControl)sender).ColorScheme = RibbonControlColorScheme.Green;// v.AppearanceFocusedColor;
            //v.Kullaniciya_Mesaj_Var = ((DevExpress.XtraBars.Ribbon.RibbonControl)sender).ColorScheme.ToString();
            /*
            int i = ((DevExpress.XtraBars.Ribbon.RibbonControl)sender).SelectedPage.PageIndex;
            ((DevExpress.XtraBars.Ribbon.RibbonControl)sender).Pages[i].Appearance.BackColor = v.AppearanceFocusedColor;
            ((DevExpress.XtraBars.Ribbon.RibbonControl)sender).Pages[i].Appearance.Options.UseBackColor = true;
            ((DevExpress.XtraBars.Ribbon.RibbonControl)sender).Pages[i].Appearance.ForeColor = v.AppearanceTextColor;
            */
        }

        public void tRibbonControl_Leave(object sender, EventArgs e)
        {
            //((DevExpress.XtraBars.Ribbon.RibbonControl)sender).BackColor = v.ribbonColor;
            //((DevExpress.XtraBars.Ribbon.RibbonControl)sender).ColorScheme = RibbonControlColorScheme.Default;// v.AppearanceFocusedColor;
            //v.Kullaniciya_Mesaj_Var = ((DevExpress.XtraBars.Ribbon.RibbonControl)sender).ColorScheme.ToString();
            /*
            (((DevExpress.XtraBars.Ribbon.RibbonControl)sender).Appearance.BackColor =
                (((DevExpress.XtraBars.Ribbon.RibbonControl)sender).Appearance.BackColor2;
            (((DevExpress.XtraBars.Ribbon.RibbonControl)sender).Appearance.ForeColor =
                (((DevExpress.XtraBars.Ribbon.RibbonControl)sender).AppearanceSelected.ForeColor;*/
        }

        public void formRibbonMenu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            #region Tanımlar
            tToolBox t = new tToolBox();

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

                //if (((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag != null)
                //    Button_Click_Type = ((DevExpress.XtraBars.BarLargeButtonItem)e.Item).Tag.ToString();
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

                //if (((DevExpress.XtraBars.BarButtonItem)e.Item).Tag != null)
                //    Button_Click_Type = ((DevExpress.XtraBars.BarButtonItem)e.Item).Tag.ToString();
            }

            Form tForm = t.Find_Form(sender);

            commonMenuClick(tForm, ButtonName, TableIPCode, myFormLoadValue);

        }

        #endregion
        
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

                commonMenuClick(tForm, ButtonName, TableIPCode, myFormLoadValue);
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

            //MessageBox.Show(e.Item.Caption + " : " + value + " : " + Read_TableIPCode+ " : " + DetailFName);

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

        #endregion MenuEvents

        #region commonMenu
        public bool commonMenuClick(Form tForm, string ButtonName, string tableIPCode, string myFormLoadValue)
        {
            if (tForm == null) return false;

            tToolBox t = new tToolBox();

            bool onay = true;
            string propNavigator = string.Empty;
            PROP_NAVIGATOR prop_ = null;
            List<PROP_NAVIGATOR> propList_ = null;

            propNavigator = t.getPropNavigator(tForm, tableIPCode);
            propNavigator = propNavigator.Replace((char)34, (char)39);

            if ((t.IsNotNull(tableIPCode) == false) ||
                (propNavigator == ""))
            {
                if (t.IsNotNull(myFormLoadValue))
                    propNavigator = myFormLoadValue;
            }

            // propNavigator ü temizle ve json a çevir
            if (propNavigator != "")
            {
                t.readProNavigator(propNavigator, ref prop_, ref propList_);
            }

            if (propList_ != null)
            {
                // NEW_IP için gerekli
                foreach (PROP_NAVIGATOR item in propList_)
                {
                    if (onay)
                        onay = commonMenuClick_(tForm, ButtonName, tableIPCode, myFormLoadValue, propNavigator, item, t);
                    break; // şimdilik bir defa çalışsın 
                }
            }
            else
            {
                if (onay)
                    onay = commonMenuClick_(tForm, ButtonName, tableIPCode, myFormLoadValue, propNavigator, null, t);
            }
            return onay;
        }

        private bool commonMenuClick_(Form tForm, string ButtonName, string tableIPCode, string myFormLoadValue, string propNavigator, PROP_NAVIGATOR prop_, tToolBox t)
        {
            bool onay = true;
            v.tButtonType buttonType = v.tButtonType.btNone;

            if ((tableIPCode == "") &&
                (prop_ != null))
                tableIPCode = prop_.TARGET_TABLEIPCODE.ToString();

            #region FOPEN_IP, FNEW, FOPEN_SUBVIEW
            if ((ButtonName.IndexOf("FNEW_IP") > 0) ||
                (ButtonName.IndexOf("FOPEN_IP") > 0) ||
                (ButtonName.IndexOf("FOPEN_SUBVIEW") > 0)
                )
            {
                v.con_Source_ParentControl_Tag_Value = getSourceParentControlTagValue(tForm);
                                

                if (ButtonName.IndexOf("FOPEN_IP") > 0)
                    buttonType = v.tButtonType.btKartAc;

                if (ButtonName.IndexOf("FNEW_IP") > 0)
                {
                    string bType = prop_.BUTTONTYPE.ToString();

                    if (bType == ((byte)v.tButtonType.btYeniKart).ToString())
                        buttonType = v.tButtonType.btYeniKart;
                    if (bType == ((byte)v.tButtonType.btYeniAltHesap).ToString())
                        buttonType = v.tButtonType.btYeniAltHesap;
                    if (bType == ((byte)v.tButtonType.btYeniKartSatir).ToString())
                        buttonType = v.tButtonType.btYeniKartSatir;
                    if (bType == ((byte)v.tButtonType.btYeniAltHesapSatir).ToString())
                        buttonType = v.tButtonType.btYeniAltHesapSatir;
                }

                //if (propNavigator.IndexOf("'BUTTONTYPE': '125'") > -1)
                if (ButtonName.IndexOf("FOPEN_SUBVIEW") > 0)
                {
                    buttonType = v.tButtonType.btOpenSubView;
                }
            }
            #endregion

            #region FSAVE
            if (ButtonName.IndexOf("FSAVE") > 0)
            {
                ev.AutoSave(tForm);
                onay = true; 
            }
            #endregion FSAVE

            #region FSAVEXIT
            if (ButtonName.IndexOf("FSAVEXIT") > 0)
            {
                // "FSAVE" içinde çalıştı
                //ev.AutoSave(tForm);
                if (onay)
                {
                    tForm.Dispose();
                    return onay;
                }
            }
            #endregion FSAVEXIT

            #region FSAVENEW
            if (ButtonName.IndexOf("FSAVENEW") > 0)
            {
                // "FSAVE" içinde çalıştı
                //ev.AutoSave(tForm);

                // new nerede ?
                if (onay)
                {

                    //return onay;
                }
            }
            #endregion FSAVENEW

            #region FEXIT
            if (ButtonName.IndexOf("FEXIT") > 0)
            {
                //if (v.cefBrowser_ != null)
                //    v.cefBrowser_.Parent = null;
                tForm.Dispose();
                return onay;
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
            if (ButtonName.IndexOf("FREPORTS") > 0)
            {
                v.con_Source_FormName = tForm.Name;
                if (tForm.AccessibleName != null)
                {
                    v.con_Source_FormCode = tForm.AccessibleName;
                    v.con_Source_FormCodeAndName = tForm.AccessibleName + "||" + tForm.Name + "||";
                }
                string FormName = "ms_Reports";
                string FormCode = "UST/PMS/PMS/ReportsView";
                t.OpenFormPreparing(FormName, FormCode, v.formType.Child);
            }
            /*
            if (ButtonName.IndexOf("FREPORTDESIGN") > 0)
            {
                v.con_Source_FormName = tForm.Name;

                tReport rapor = new tReport();
                rapor.tShowReportDesigner();
            }
            */
            if (ButtonName.IndexOf("REPORTVIEW") > 0)
            {
                string RaporHesapKodu = t.MyProperties_Get(myFormLoadValue, "RaporHesapKodu:");
                string KonuOlan_TableIPCode = t.MyProperties_Get(myFormLoadValue, "KonuOlan_TableIPCode:");
                string KonuOlan_ID = t.MyProperties_Get(myFormLoadValue, "KonuOlan_ID:");
                string KonuOlanaAit_Liste_TableIPCode = t.MyProperties_Get(myFormLoadValue, "KonuOlanaAit_Liste_TableIPCode:");
                string Parametre = t.MyProperties_Get(myFormLoadValue, "Parametre:");

                v.con_FormLoadValue = string.Empty;
                v.con_FormLoadValue = ev.Preparing_REPORTVIEW(RaporHesapKodu, KonuOlan_TableIPCode, KonuOlan_ID, KonuOlanaAit_Liste_TableIPCode, Parametre);
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
                t.MyProperties_Set(ref v.con_FormLoadValue, v.FormLoad, "TargetLikeValue", tableIPCode);
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
                t.MyProperties_Set(ref v.con_FormLoadValue, v.FormLoad, "TargetLikeValue", tableIPCode);
                t.MyProperties_Set(ref v.con_FormLoadValue, v.FormLoad, "END", string.Empty);

                if (tForm != null) // SourceForm
                {
                    tForms fr = new tForms();
                    Form tNewForm = fr.Get_Form("tn_SMSSablonuTanimla", tForm); // SourceForm

                    t.ChildForm_View(tNewForm, Application.OpenForms[0]);
                }
            }
            #endregion FSMS_SEND

            #region YILAYBACK - NEXT
            if (ButtonName.IndexOf("YILAY_BACK") > 0)
            {
                v.DONEMTIPI_YILAY_OLD = v.DONEMTIPI_YILAY;
                int value = v.DONEMTIPI_YILAY;
                string caption = getYilAyCaption(value);
                v.DONEMTIPI_YILAY = getBackYilAy(value, v.BackNext.back, ref caption);
                setYilAyCaption(tForm, v.DONEMTIPI_YILAY, caption, myFormLoadValue, t);
                yilAyFormRefresh(tForm);
                //MessageBox.Show(v.DONEMTIPI_YILAY.ToString() + " ; " + caption);
            }
            if (ButtonName.IndexOf("YILAY_NEXT") > 0)
            {
                v.DONEMTIPI_YILAY_OLD = v.DONEMTIPI_YILAY;
                int value = v.DONEMTIPI_YILAY;
                string caption = getYilAyCaption(value);
                v.DONEMTIPI_YILAY = getBackYilAy(value, v.BackNext.next, ref caption);
                setYilAyCaption(tForm, v.DONEMTIPI_YILAY, caption, myFormLoadValue, t);
                yilAyFormRefresh(tForm);
                //MessageBox.Show(v.DONEMTIPI_YILAY.ToString() + " ; " + caption);
            }
            #endregion YILAYBACK - NEXT

            if ((onay) &&
                (buttonType != v.tButtonType.btNone))
            {
                v.tButtonHint.Clear();
                v.tButtonHint.tForm = tForm;
                v.tButtonHint.tableIPCode = tableIPCode;
                v.tButtonHint.propNavigator = propNavigator;
                v.tButtonHint.buttonType = buttonType;
                v.tButtonHint.senderType = "Menu";
                tEventsButton evb = new tEventsButton();
                onay = evb.btnClick(v.tButtonHint);
            }

            return onay;
        }

        #region sub commonMenuClick functions

        private string getSourceParentControlTagValue(Form tForm) //con_Source_ParentControl_Tag_Value
        {
            tToolBox t = new tToolBox();
            string value = "";
            if (tForm.AccessibleName == null) return value;

            // form üzerinde hem işaret hemde value değerleri var ise direk onu alalım
            if (tForm.AccessibleName.IndexOf("con_Source_ParentControl_Tag_Value||") > -1)
            {
                value = tForm.AccessibleName.ToString();
                // işareti silelim kalan value değerlerini gönderelim
                value = value.Replace("con_Source_ParentControl_Tag_Value||", "");
                v.con_Source_ParentControl_Tag_Value = value;
                return value;
            }

            if (tForm.AccessibleName.IndexOf("con_Source_ParentControl_Tag_Value") > -1)
            {
                string TabControlName = "tabControl_SUBVIEW";
                Control cntrl = null;
                cntrl = t.Find_Control(tForm, TabControlName);

                /// TabControl or 
                /// TabPane or
                /// NavigationPane or

                if (cntrl != null)
                {
                    #region XtraTab.XtraTabControl
                    if (cntrl.GetType().ToString() == "DevExpress.XtraTab.XtraTabControl")
                    {
                        DevExpress.XtraTab.XtraTabPage tTabPage =
                            ((DevExpress.XtraTab.XtraTabControl)cntrl).SelectedTabPage;
                        if (tTabPage.Tag != null)
                        {
                            value = tTabPage.Tag.ToString();
                            return value;
                        }
                    }
                    #endregion

                    #region Navigation.NavigationPane
                    if (cntrl.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavigationPane")
                    {
                        var tTabPage = ((DevExpress.XtraBars.Navigation.NavigationPane)cntrl).SelectedPage; //.SelectedPage;

                        if (tTabPage == null)
                            return "";

                        if (((DevExpress.XtraBars.Navigation.NavigationPage)tTabPage).Tag != null)
                        {
                            value = ((DevExpress.XtraBars.Navigation.NavigationPage)tTabPage).Tag.ToString();
                            return value;
                        }
                    }
                    #endregion

                    #region Navigation.TabPane
                    if (cntrl.GetType().ToString() == "DevExpress.XtraBars.Navigation.TabPane")
                    {
                        DevExpress.XtraBars.Navigation.TabNavigationPage tTabPage =
                            ((DevExpress.XtraBars.Navigation.TabPane)cntrl).SelectedPage;
                        if (tTabPage.Tag != null)
                            value = tTabPage.Tag.ToString();
                        return value;
                    }
                    #endregion
                }
            }

            return value;
        }
        private int getBackYilAy(int yilAy, v.BackNext backNext, ref string yilAyCapiton)
        {
            int rowNo = 0;
            int yeniYilAy = yilAy;
            int count = v.ds_DonemTipiList.Tables[0].Rows.Count;

            for (int i = 0; i < count; i++)
            {
                if (yilAy.ToString() == v.ds_DonemTipiList.Tables[0].Rows[i][0].ToString())
                {
                    rowNo = i;
                    break;
                }
            }

            // back ise
            if (backNext == v.BackNext.back)
            {
                if (count > rowNo + 1)
                {
                    yeniYilAy = Convert.ToInt32(v.ds_DonemTipiList.Tables[0].Rows[rowNo + 1][0].ToString());
                    yilAyCapiton = v.ds_DonemTipiList.Tables[0].Rows[rowNo + 1][1].ToString();
                }
            }
            // next ise
            if (backNext == v.BackNext.next)
            {
                if (rowNo > 0)
                {
                    yeniYilAy = Convert.ToInt32(v.ds_DonemTipiList.Tables[0].Rows[rowNo - 1][0].ToString());
                    yilAyCapiton = v.ds_DonemTipiList.Tables[0].Rows[rowNo - 1][1].ToString();
                }
            }

            return yeniYilAy;
        }
        private void setYilAyCaption(Form tForm, int yilAy, string caption, string menuControlName, tToolBox t)
        {
            //tItem.Caption = t.getYilAyCaption(yilAy);

            Control c = null;
            string[] controls = new string[] { };

            c = t.Find_Control(tForm, menuControlName, "", controls);

            if (c != null)
            {
                if (c.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavPane")
                {
                    DevExpress.XtraBars.Navigation.TileNavPane tnPane = null;
                    tnPane = c as DevExpress.XtraBars.Navigation.TileNavPane;
                    string bname = string.Empty;
                    //string iname = string.Empty;

                    int i3 = tnPane.Buttons.Count;
                    //int i5 = 0;
                    for (int i2 = 0; i2 < i3; i2++)
                    {
                        bname = tnPane.Buttons[i2].Element.Name.ToString();
                        if (bname == "item_YILAY") //buttonName)
                        {
                            tnPane.Buttons[i2].Element.Caption = caption;
                            //tnPane.Buttons[i2].Element.ElementClick += null;
                            //tnPane.Buttons[i2].Element.ElementClick += myClick;
                            break;
                        }
                    }
                }
            }
        }

        private void yilAyFormRefresh(Form tForm)
        {
            vSubWork vSW = new vSubWork();
            vSW._01_tForm = tForm;
            vSW._02_TableIPCode = "";
            vSW._03_WorkTD = v.tWorkTD.Refresf_DataYilAy;
            vSW._04_WorkWhom = v.tWorkWhom.All;

            ev.tSubWork_(vSW);
        }


        public string getYilAyCaption(int yilAy)
        {
            tToolBox t = new tToolBox();
            if (t.IsNotNull(v.ds_DonemTipiList) == false) return "";

            string fieldName = "BelgeDonemTipi";
            string caption = "";
            string yilAy_ = yilAy.ToString();

            int count = v.ds_DonemTipiList.Tables[0].Rows.Count;

            if ((v.tMainFirm.SectorTypeId == (Int16)v.msSectorType.UstadMtsk) ||
                (v.tMainFirm.SectorTypeId == (Int16)v.msSectorType.TabimMtsk) ||
                (v.tMainFirm.SectorTypeId == (Int16)v.msSectorType.TabimSrc) ||
                (v.tMainFirm.SectorTypeId == (Int16)v.msSectorType.TabimIsmak))
                fieldName = "DonemTipi";

            for (int i = 0; i < count; i++)
            {
                if (yilAy_ == v.ds_DonemTipiList.Tables[0].Rows[i]["Id"].ToString())
                {
                    caption = v.ds_DonemTipiList.Tables[0].Rows[i][fieldName].ToString();
                    break;
                }
            }

            return caption;
        }

        #endregion sub commonMenuClick functions

        #endregion commonMenu

        #region Form shortcutKeys and Buttons 

        public bool findKeyAdvGridGroupButtons(Form tForm, string keyCode)
        {
            tToolBox t = new tToolBox();
            tEventsGrid evg = new tEventsGrid();
            bool keyFind = false;

            Control cntrl = cntrl = null;
            // DevExpress.XtraEditors.PanelControl
            cntrl = t.Find_Control(tForm, "tAdvGridGroupButtons");
            if (cntrl != null)
            {
                foreach (var item in cntrl.Controls)
                {
                    if (item.GetType().ToString() == "DevExpress.XtraEditors.CheckButton")
                    {
                        if (((DevExpress.XtraEditors.CheckButton)item).Text.IndexOf(keyCode) > -1)
                        {
                            keyFind = true;
                            ((DevExpress.XtraEditors.CheckButton)item).Checked = true;
                            evg.checkGridGroupButton_Click(item, EventArgs.Empty);
                            break;
                        }
                    }
                }
            }

            return keyFind;
        }

        public bool findKeyCode(DevExpress.XtraEditors.TileControl mControl, string keyCode)
        {
            bool keyFind = false;

            string s = "";

            foreach (DevExpress.XtraEditors.TileGroup pGroup in mControl.Groups)
            {
                foreach (var item in pGroup.Items)
                {
                    if (((DevExpress.XtraEditors.TileItem)item).Text != null)
                        s = ((DevExpress.XtraEditors.TileItem)item).Text;
                    if (((DevExpress.XtraEditors.TileItem)item).Text2 != null)
                        s = s + ((DevExpress.XtraEditors.TileItem)item).Text2;
                    if (((DevExpress.XtraEditors.TileItem)item).Text3 != null)
                        s = s + ((DevExpress.XtraEditors.TileItem)item).Text3;
                    if (((DevExpress.XtraEditors.TileItem)item).Text4 != null)
                        s = s + ((DevExpress.XtraEditors.TileItem)item).Text4;
                    //if (((DevExpress.XtraEditors.TileItem)item).Text5 != null)
                    //    s = s + ((DevExpress.XtraEditors.TileItem)item).Text5;
                    //if (((DevExpress.XtraEditors.TileItem)item).Text6 != null)
                    //    s = s + ((DevExpress.XtraEditors.TileItem)item).Text6;

                    if (s.IndexOf(keyCode) > -1)
                    {
                        keyFind = true;
                        tTileItem_ItemClick(((DevExpress.XtraEditors.TileItem)item), null);
                        break;
                    }
                }
                if (keyFind) break;
            }

            return keyFind;
        }

        public bool findKeyCode(DevExpress.XtraBars.Navigation.TileNavPane mControl, string keyCode)
        {
            bool keyFind = false;

            int i3 = mControl.Buttons.Count;
            for (int i2 = 0; i2 < i3; i2++)
            {
                if (mControl.Buttons[i2].Element.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavCategory")
                {

                    if (((DevExpress.XtraBars.Navigation.TileNavCategory)mControl.Buttons[i2].Element).TileText.ToString().IndexOf(keyCode) > -1)
                    {
                        //tNavButton_ElementClick((object)((DevExpress.XtraBars.Navigation.TileNavCategory)mControl.Buttons[i2].Element), null);
                        mControl.SelectedElement = ((DevExpress.XtraBars.Navigation.TileNavCategory)mControl.Buttons[i2].Element);
                        break;
                    }


                    foreach (var item in ((DevExpress.XtraBars.Navigation.TileNavCategory)mControl.Buttons[i2].Element).Items)
                    {
                        if (((DevExpress.XtraBars.Navigation.TileNavItem)item).TileText.ToString().IndexOf(keyCode) > -1)
                        {
                            keyFind = true;
                            tNavButton_ElementClick((object)((DevExpress.XtraBars.Navigation.TileNavItem)item), null);
                            break;
                        }

                        if (((DevExpress.XtraBars.Navigation.TileNavItem)item).SubItems.Count > 0)
                        {
                            foreach (var subItem in ((DevExpress.XtraBars.Navigation.TileNavItem)item).SubItems)
                            {
                                if (((DevExpress.XtraBars.Navigation.TileNavSubItem)subItem).TileText.ToString().IndexOf(keyCode) > -1)
                                {
                                    keyFind = true;
                                    tNavButton_ElementClick((object)((DevExpress.XtraBars.Navigation.TileNavSubItem)subItem), null);
                                    break;
                                }
                            }
                        }
                    }
                }

                if ((mControl.Buttons[i2].Element.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton") &&
                    (mControl.Buttons[i2].Element.Caption.ToString().IndexOf(keyCode) > -1))
                {
                    //mControl.SelectedElement = 
                    //   (DevExpress.XtraBars.Navigation.NavButton)mControl.Buttons[i2].Element;

                    //tNavButton_ElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
                    keyFind = true;
                    tNavButton_ElementClick(mControl.Buttons[i2].Element, null);
                    break;
                }
            }

            return keyFind;
        }

        public bool findKeyCode(DevExpress.XtraToolbox.ToolboxControl mControl, string keyCode)
        {
            bool keyFind = false;

            // groups içinde ara
            int i = 0;
            foreach (var item in mControl.Groups)
            {
                if (((DevExpress.XtraToolbox.ToolboxGroup)item).Caption.IndexOf(keyCode) > -1)
                {
                    keyFind = true;
                    mControl.SelectedGroupIndex = i;
                    break;
                }
                i++;
            }

            // aranan keyCode group içinde değilse group altındaki items olabilir
            // bu nedenle sadece select olan group içindekilere bakılıyor
            //
            if ((keyFind == false) &&
                (mControl.SelectedGroupIndex > -1))
            {
                foreach (DevExpress.XtraToolbox.ToolboxItem item in mControl.Groups[mControl.SelectedGroupIndex].Items)
                {
                    //DevExpress.XtraToolbox.ToolboxItem
                    if (item.Caption.IndexOf(keyCode) > -1)
                    {
                        keyFind = true;
                        tToolboxControl_ItemClick((object)mControl, new DevExpress.XtraToolbox.ToolboxItemClickEventArgs(item));
                        break;
                    }
                }
            }

            return keyFind;
        }


        #endregion Form shortcutKeys and Buttons 

        #region diğer menu click ler
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

                if (pGroup.Tag != null)
                    formName = pGroup.Tag.ToString();

                Form tForm = Application.OpenForms[formName];

                if (((DevExpress.XtraEditors.TileItem)sender).Tag != null)
                    myFormLoadValue = ((DevExpress.XtraEditors.TileItem)sender).Tag.ToString();

                commonMenuClick(tForm, ButtonName, TableIPCode, myFormLoadValue);
            }
        }

        public void tNavBarItem_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
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

            commonMenuClick(tForm, ButtonName, TableIPCode, myFormLoadValue);
        }

        public void tAccordionControlElement_Click(object sender, EventArgs e)
        {
            #region Tanımlar 
            string TableIPCode = string.Empty;
            string ButtonName = string.Empty;
            string myFormLoadValue = string.Empty;
            #endregion Tanımlar

            ButtonName = ((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Name.ToString();
            Form tForm = ((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).AccordionControl.FindForm();

            if (((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Tag != null)
                myFormLoadValue = ((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Tag.ToString();

            commonMenuClick(tForm, ButtonName, TableIPCode, myFormLoadValue);
        }

        public void tNavButtonFirms_ElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
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
                        //v.SP_FIRM_ID = firmId;
                        //v.SP_FirmShortName = ds.Tables[0].Rows[dN.Position]["FIRM_NAME"].ToString();

                        //
                        //v.Kullaniciya_Mesaj_Show = true;
                        //v.Kullaniciya_Mesaj_Var = v.SP_FirmShortName;

                        //Application.OpenForms[0].Text = Application.OpenForms[0].AccessibleName + "   [ " + v.SP_FirmShortName + " , " + v.SP_FIRM_ID.ToString() + " ]";
                    }
                }
            }
        }

        public void tNavButton_ElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            #region Tanımlar 
            tToolBox t = new tToolBox();
            string TableIPCode = string.Empty;
            string ButtonName = string.Empty;
            string myFormLoadValue = string.Empty;
            string formName = string.Empty;
            #endregion Tanımlar

            //DevExpress.XtraBars.Navigation.TileNavItem
            //DevExpress.XtraBars.Navigation.TileNavSubItem

            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavSubItem")
            {
                ButtonName = ((DevExpress.XtraBars.Navigation.TileNavSubItem)sender).Name.ToString();
                // buradan null dönüyor
                //DevExpress.XtraBars.Navigation.TileNavPane mControl = ((DevExpress.XtraBars.Navigation.TileNavSubItem)sender).TileNavPane;

                if (((DevExpress.XtraBars.Navigation.TileNavSubItem)sender).Appearance.Name != null)
                {
                    formName = ((DevExpress.XtraBars.Navigation.TileNavSubItem)sender).Appearance.Name.ToString();

                    Form tForm = Application.OpenForms[formName];

                    if (((DevExpress.XtraBars.Navigation.TileNavSubItem)sender).Tag != null)
                        myFormLoadValue = ((DevExpress.XtraBars.Navigation.TileNavSubItem)sender).Tag.ToString();

                    // Belge Türü Seçiniz  << butonu
                    Control mControl = t.findControlMenu(tForm, "DevExpress.XtraBars.Navigation.TileNavPane");

                    shortcutButtonSet((DevExpress.XtraBars.Navigation.TileNavPane)mControl,
                                     ((DevExpress.XtraBars.Navigation.TileNavSubItem)sender).Caption, ButtonName, myFormLoadValue);

                    commonMenuClick(tForm, ButtonName, TableIPCode, myFormLoadValue);
                }
            }

            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                ButtonName = ((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name.ToString();
                DevExpress.XtraBars.Navigation.TileNavPane mControl = ((DevExpress.XtraBars.Navigation.TileNavItem)sender).TileNavPane;

                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Appearance.Name != null)
                {
                    formName = ((DevExpress.XtraBars.Navigation.TileNavItem)sender).Appearance.Name.ToString();

                    Form tForm = Application.OpenForms[formName];

                    if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Tag != null)
                        myFormLoadValue = ((DevExpress.XtraBars.Navigation.TileNavItem)sender).Tag.ToString();

                    // Belge Türü Seçiniz  << butonu
                    shortcutButtonSet(mControl, ((DevExpress.XtraBars.Navigation.TileNavItem)sender).Caption, ButtonName, myFormLoadValue);

                    commonMenuClick(tForm, ButtonName, TableIPCode, myFormLoadValue);
                }
            }

            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                ButtonName = ((DevExpress.XtraBars.Navigation.NavButton)sender).Name.ToString();

                // DİKKAT mControl ü tespit edemedim
                //
                //TileNavPaneViewInfo ViewInfo =  
                //  ((DevExpress.XtraBars.Navigation.NavButton)sender).GetViewInfo() as TileNavPaneViewInfo;
                //DevExpress.XtraBars.Navigation.TileNavPane mControl = ((DevExpress.XtraBars.Navigation.NavButton)sender). ;

                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Appearance.Name != null)
                {
                    formName = ((DevExpress.XtraBars.Navigation.NavButton)sender).Appearance.Name.ToString();

                    Form tForm = Application.OpenForms[formName];

                    //  Form tForm = ((DevExpress.XtraBars.Navigation.TileNavElement)sender).TileNavPane.FindForm();

                    if (((DevExpress.XtraBars.Navigation.NavButton)sender).Tag != null)
                        myFormLoadValue = ((DevExpress.XtraBars.Navigation.NavButton)sender).Tag.ToString();

                    // Belge Türü Seçiniz  << butonu
                    // aslında navButton görünürde olduğu için gerekte yok
                    //shortcutButtonSet(mControl, ((DevExpress.XtraBars.Navigation.TileNavItem)sender).Caption, ButtonName, myFormLoadValue);

                    //((DevExpress.XtraBars.Navigation.NavButton)sender).

                    commonMenuClick(tForm, ButtonName, TableIPCode, myFormLoadValue);
                }
            }

        }

        private void shortcutButtonSet(DevExpress.XtraBars.Navigation.TileNavPane mControl, string caption, string buttonName, string myFormLoadValue)
        {
            int i3 = mControl.Buttons.Count;
            for (int i2 = 0; i2 < i3; i2++)
            {
                /*
                if (mControl.Buttons[i2].Element.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavCategory")
                {
                
                    if (((DevExpress.XtraBars.Navigation.TileNavCategory)mControl.Buttons[i2].Element).TileText.ToString().IndexOf(keyCode) > -1)
                    {
                        //tNavButton_ElementClick((object)((DevExpress.XtraBars.Navigation.TileNavCategory)mControl.Buttons[i2].Element), null);
                        mControl.SelectedElement = ((DevExpress.XtraBars.Navigation.TileNavCategory)mControl.Buttons[i2].Element);
                        break;
                    }
                    
                    int i5 = ((DevExpress.XtraBars.Navigation.TileNavCategory)mControl.Buttons[i2].Element).Items.Count;
                    for (int i4 = 0; i4 < i5; i4++)
                    {
                        if (((DevExpress.XtraBars.Navigation.TileNavCategory)mControl.Buttons[i2].Element).Items[i4].TileText.ToString().IndexOf(keyCode) > -1)
                        {
                            keyFind = true;
                            tNavButton_ElementClick((object)((DevExpress.XtraBars.Navigation.TileNavCategory)mControl.Buttons[i2].Element).Items[i4], null);
                            break;
                        }
                    }
                }
                */
                if ((mControl.Buttons[i2].Element.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton") &&
                    (mControl.Buttons[i2].Element.Name.ToString().IndexOf("SHORTCUT") > -1))
                {
                    mControl.Buttons[i2].Element.Caption = caption;
                    mControl.Buttons[i2].Element.Name = buttonName + "_SHORTCUT";
                    mControl.Buttons[i2].Element.Tag = myFormLoadValue;
                    break;
                }
            }
        }

        #endregion

        
    }
}
