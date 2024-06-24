using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Tkn_InputPanel;
using Tkn_TablesRead;
using Tkn_ToolBox;
using Tkn_Variable;
using Tkn_Events;
using Tkn_Forms;
using Tkn_Layout;

namespace Tkn_Events
{

    public class tSearch : tBase
    {
        tEvents ev = new tEvents();

        #region directSearch

        public bool directSearch(vButtonHint buttonHint)
        {
            tEventsButton evb = new tEventsButton();

            bool onay = false;
            bool oncekiOnay = false;
            Form tForm = buttonHint.tForm;
            string mainTableIPCode = buttonHint.tableIPCode; // esas atamanın yapılacağı tableIPCode
            string myProp = buttonHint.propNavigator;
            string searchValue = buttonHint.columnEditValue;
                    
            /// 1. nesne üzerinde daha önce set edilmiş value varsa onay verme yeniden arama süreci başlamasın
            /// 2. gelen inputValue önce sorgulansın sorgu sonucu tek bir record dönüyorsa gelen record direk set edilsin ve arama motoru açılmasın
            /// 3. birden fazla sonuç gelirse veya hiç kayıt bulunmazsa arama moturu açılsın ve kullanıcı istediği kaydı seçsin veya arasın
            ///  
        
            // arama motorunun IP si
            string searchTableIPCode = "";
            tToolBox t = new tToolBox();
            List<PROP_NAVIGATOR> prop_ = t.readPropList<PROP_NAVIGATOR>(myProp);
            v.tButtonType propButtonType = v.tButtonType.btNone;

            foreach (PROP_NAVIGATOR item in prop_)
            {
                if (item.BUTTONTYPE.ToString() != "null")
                    propButtonType = ev.getClickType(Convert.ToInt32(item.BUTTONTYPE.ToString()));

                //if (item.BUTTONTYPE.ToString().IndexOf("58") > -1)
                //if (item.BUTTONTYPE.ToString() == ((byte)v.tButtonType.btArama).ToString())
                if (propButtonType == v.tButtonType.btArama)
                {
                    // önce search / atama yapılmış mı kontrol et
                    //
                    //onay = checkSearchEngineValues(tForm, targetTableIPCode, item.TABLEIPCODE_LIST);
                    // bu kontrol olmadı çünki search atamalarında "" olan atamalarda olabiliyor FalanFilanKodu gibi
                    // çünki her kartın FalanFilanKodu yok
                    
                    if (onay == false)
                    {
                        // arama için kullanılacak tableIPCode
                        if (t.IsNotNull(item.READ_TABLEIPCODE))
                            searchTableIPCode = item.READ_TABLEIPCODE.ToString();

                        if (t.IsNotNull(searchTableIPCode) == false)
                            searchTableIPCode = item.TARGET_TABLEIPCODE.ToString();

                        if (t.IsNotNull(searchTableIPCode) == false)
                        {
                            MessageBox.Show("DİKKAT : Arama listesi gerekli olan TableIPCode tanımlı değil ... (READ_TABLEIPCODE veya TARGET_TABLEIPCODE) ");
                            onay = false;
                            oncekiOnay = onay;
                            v.searchOnay = onay;
                        }
                        else
                        {
                            onay = getSearchValues(tForm, searchTableIPCode, myProp, searchValue, mainTableIPCode, item);
                            oncekiOnay = onay;
                            v.searchOnay = onay;
                        }
                    }
                    //return onay;
                }

                // Aramadan sonraki işlemler
                //
                
                if ((oncekiOnay) &&
                    (propButtonType != v.tButtonType.btArama))
                {
                    buttonHint.tForm = tForm;

                    if (propButtonType == v.tButtonType.btFormulleriHesapla)
                        buttonHint.buttonType = v.tButtonType.btFormulleriHesapla;
                    if (propButtonType == v.tButtonType.btDataTransferi)
                        buttonHint.buttonType = v.tButtonType.btDataTransferi;
                    if (propButtonType == v.tButtonType.btInputBox)
                        buttonHint.buttonType = v.tButtonType.btInputBox;
                    if (propButtonType == v.tButtonType.btOpenSubView)
                        buttonHint.buttonType = v.tButtonType.btOpenSubView;

                    // propList i tek tek işlemek gerekiyor
                    buttonHint.prop_ = item;
                    // search sonrası işlem çalışsın
                    onay = evb.singleButtonEvent(buttonHint);
                }
            }
            
            return onay;
        }

        private bool getSearchValues(Form tForm,
            string searchTableIPCode,
            string myProp,
            string searchValue,
            string targetTableIPCode,
            PROP_NAVIGATOR prop_)
        {
            /// Amaç : tek value yi form açmadan varlığını kontrol etmek
            /// varsa search atamalarını yapmak

            /// sifır value ise yeni kartı açmayı deneyelim

            /// TableIPCode = Arama moturunun
            /// TargetTableIPCode = bulunan kaydın set edileceği ekrandaki aktif form 
            
            bool onay = false;
            
            tToolBox t = new tToolBox();
            tInputPanel ip = new tInputPanel();

            DataSet dsSearchTableIP = ip.Create_DataSet(tForm, searchTableIPCode);

            string myProp_ = dsSearchTableIP.Namespace.ToString();
            string tSql = t.Set(t.MyProperties_Get(myProp_, "SqlSecond:"),
                                t.MyProperties_Get(myProp_, "SqlFirst:"), "");
                        
            tTablesRead tr = new tTablesRead();
            DataSet dsMSTableIp = new DataSet();

            tr.MS_Tables_IP_Read(dsMSTableIp, searchTableIPCode);

            if (t.IsNotNull(dsMSTableIp) == false)
                return false;
                        
            string TableName = dsMSTableIp.Tables[0].Rows[0]["LKP_TABLE_NAME"].ToString();
            string TableLabel = "[" + t.Set(dsMSTableIp.Tables[0].Rows[0]["TABLE_CODE"].ToString(), "", "") + "]";
            string FindFName = dsMSTableIp.Tables[0].Rows[0]["FIND_FNAME"].ToString();
            string schemasCode = dsMSTableIp.Tables[0].Rows[0]["LKP_SCHEMAS_CODE"].ToString();
            
            if (t.IsNotNull(schemasCode) == false) 
                schemasCode = "dbo";
                                    
            if (t.IsNotNull(FindFName) == false)
            {
                MessageBox.Show("DİKKAT : ( " + searchTableIPCode + " ) için -Find FName- tanımı eksik ...");
                return onay;
            }

            string whereSQL = preparinSearchSql(dsSearchTableIP, TableLabel, FindFName, searchValue);

            //string tSql = "Select * from " + schemasCode + "." + TableName + " where 0 = 0 " + findValues;

            tSql = t.SQLWhereAdd(tSql, TableLabel, whereSQL, "DEFAULT");

            DataSet ds_Query = new DataSet();
            t.SQL_Read_Execute(v.dBaseNo.Project, ds_Query, ref tSql, TableName, "searchVaule");

            v.searchOnay = false;

            if (t.IsNotNull(ds_Query) == false)
            {
                if (ds_Query.Tables[0].Rows.Count == 0)
                {
                    //MessageBox.Show("yeni kart açalım...");

                    //  arama sonucu hiç bir kayıt bulunamadıysa 
                    //  yeni hesap eklemek için hazırlanmış form için gerekli bilgiler
                    //   
                    string Prop_Search = dsMSTableIp.Tables[0].Rows[0]["PROP_SEARCH"].ToString();

                    if (t.IsNotNull(Prop_Search))
                    {
                        onay = addNewRecored(Prop_Search);
                        if (onay)
                        {
                            // yeni hesabı, arama isteği yapan targetTableIPCode ye gönder
                            v.searchOnay = setSearchEngineValues(tForm, targetTableIPCode, null, prop_.TABLEIPCODE_LIST);
                            onay = v.searchOnay;
                        }
                    }
                }
            }

            if (t.IsNotNull(ds_Query))
            { 
                if (ds_Query.Tables[0].Rows.Count == 1)
                {
                    // aranıp bulunan kayıt
                    v.con_DataRow = ds_Query.Tables[0].Rows[0];

                    v.searchOnay = setSearchEngineValues(tForm, targetTableIPCode, null, prop_.TABLEIPCODE_LIST);
                    onay = v.searchOnay;
                }

                if (ds_Query.Tables[0].Rows.Count > 1)
                {
                    v.searchOnay = searchEngines(tForm, targetTableIPCode, searchValue, prop_);

                    onay = v.searchOnay;
                }

            }

            ds_Query.Dispose();
            return onay;
        }

        private string preparinSearchSql(DataSet dsSearchTableIP, string tableLabel, string findFName, string searchValue)
        {
            tToolBox t = new tToolBox();
           
            bool isIntField = false;
            int isIntValue = 0;
            string fName = "";
            string whereSQL = "";

            if ((findFName.ToString().IndexOf("||") > -1) && // birden fazla field var
                (findFName.ToString().IndexOf("==") == -1))  // Int field için eşleştirme yok
            {
                findFName = findFName + "||";
                while (findFName.IndexOf("||") > -1)
                {
                    fName = t.Get_And_Clear(ref findFName, "||");

                    //checkLabel(ref tableLabel, ref fName);

                    if (fName != "")
                    {
                        if (whereSQL == "")
                            whereSQL = whereSQL + "and (" + fName + " like '%" + searchValue + "%'";
                        else whereSQL = whereSQL + " or  " + fName + " like '%" + searchValue + "%'";
                    }
                }
            }

            // bir field üzerinde sadece eşitse çalışacak ( Id ler için gerekli oldu Aday.Id gibi ) 
            if ((findFName.ToString().IndexOf("||") > -1) && // birden fazla field var
                (findFName.ToString().IndexOf("==") > -1))   // Int field için eşleştirme var    
            {
                // Id||TamAdiSoyadi||==
                findFName = findFName.Replace("==", "");

                while (findFName.IndexOf("||") > -1)
                {
                    fName = t.Get_And_Clear(ref findFName, "||");

                    //checkLabel(ref tableLabel, ref fName);

                    if (fName != "")
                    {
                        // karşılaştırmayı aranan value ile yapmamız gerekiyor 
                        // int bir value mi arıyor, yoksa string bir value mi arıyor

                        isIntField = t.IsIntFieldType(dsSearchTableIP, fName);
                        isIntValue = t.myInt32(searchValue);

                        // Int field değil ise (TamAdiSoyadi gibi birşey ise

                        if ((isIntValue == 0) && (isIntField == false))
                        {
                            if (whereSQL == "")
                                whereSQL = whereSQL + "and (" + fName + " like '%" + searchValue + "%'";
                            else whereSQL = whereSQL + " or  " + fName + " like '%" + searchValue + "%'";
                        }

                        if ((isIntValue > 0) && (isIntField))
                        {
                            //whereSQL = " and " + tableLabel + "." + fName + " = " + searchValue + " ";
                            if (tableLabel != "")
                                 whereSQL = " and " + tableLabel + "." + fName + " = " + searchValue + " ";
                            else whereSQL = " and " + findFName + " = " + searchValue + " ";
                        }
                    }
                }
            }

            // yani tek field yazılmışsa
            if ((findFName.ToString().IndexOf("||") == -1) && // birden fazla field yok
                (findFName.ToString().IndexOf("==") == -1) && // Int field için eşleştirme yok
                (whereSQL == ""))
            {
                if (checkLabel(ref tableLabel, ref findFName))
                {
                    if (tableLabel != "")
                        whereSQL = " and " + tableLabel + "." + findFName + " like '%" + searchValue + "%'";
                    else whereSQL = " and " + findFName + " like '%" + searchValue + "%'";
                }
            }

            if (whereSQL.IndexOf("and (") > -1) whereSQL = whereSQL + ")";

            return whereSQL;
        }

        private bool checkLabel(ref string tableLabel, ref string fieldName)
        {
            // tableLabel = MtskAday
            // fieldName  = MtskAday.TamAdiSoyadi
            // veya
            // tableLabel = [MtskAday]
            // fieldName  = [MtskAday].TamAdiSoyadi
            //

            bool onay = true;

            if (fieldName == "")
            {
                MessageBox.Show("Arama yapılacak olan -Find FieldName- belli değil...");
                onay = false;
                return onay;
            }


            if (fieldName.IndexOf(tableLabel + ".") > -1)
            {
                fieldName = fieldName.Replace(tableLabel + ".", "");
                return onay;
            }

            // tableLabel = [MtskAday]
            // fieldName  = MtskAday.TamAdiSoyadi
            //
            tableLabel = tableLabel.Replace("[", "");
            tableLabel = tableLabel.Replace("]", "");

            if (fieldName.IndexOf(tableLabel + ".") > -1)
            {
                fieldName = fieldName.Replace(tableLabel + ".", "");
                return onay;
            }

            // tableLabel = MtskAday
            // fieldName  = MtskAdayBelgeler.Id

            if (fieldName.IndexOf(".") > -1)
            {
                if ((fieldName.IndexOf(tableLabel + ".") == -1) &&
                    (fieldName.IndexOf("[" + tableLabel + "].") == -1))
                {
                    tableLabel = "";
                    return onay;
                }
            }

            return onay;
        }

        private bool addNewRecored(string Prop_Search)
        {
            tToolBox t = new tToolBox();

            bool onay = false;

            PROP_SEARCH prop_ = t.readProp<PROP_SEARCH>(Prop_Search);

            //v.search_readTableIPCode = prop_.TABLEIPCODE.ToString();
            string FORMCODE = prop_.FORMCODE.ToString();
            //string FORMTYPE = "DIALOG";

            //if //((t.IsNotNull(v.search_readTableIPCode) == false) ||
            if (t.IsNotNull(FORMCODE) == false)
            {
                //MessageBox
                return onay;
            } 

            tForms fr = new tForms();
            Form tNewForm = fr.Get_Form("ms_Form");

            // MS_LAYOUT Preparing
            // form code var ise boş formu kullanarak Layout dizayn et
            if (FORMCODE != "")
            {
                tLayout l = new tLayout();
                l.Create_Layout(tNewForm, FORMCODE);
            }

            if (tNewForm != null)
            {
                /// ?? nedir 
                tNewForm.Opacity = 50;
                //tNewForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;

                /// refresh özelliğini taşı
                /// açılan newForm da kayıt gerçekleşirse on 
                //tNewForm.HelpButton = tForm.HelpButton;

                #region
                //if (FORMTYPE == "DIALOG")
                //{
                t.DialogForm_View(tNewForm, FormWindowState.Normal, "");// myFormLoadValue);
                onay = v.searchOnay;
                //}
                
                #endregion
            }
            else MessageBox.Show("Form bulunamadı...");

            if (Cursor.Current != Cursors.Default)
                Cursor.Current = Cursors.Default;

            return onay;
        }
        #endregion directSearh

        #region Search Engine / Arama Motoru

        public bool searchEngines(Form tForm, string targetTableIPCode, string searchValue, PROP_NAVIGATOR prop_)
        {
            if (prop_ == null) return false;

            tToolBox t = new tToolBox();
            bool onay = false;

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

            string SearchTableIPCode = "";
            
            if (t.IsNotNull(prop_.READ_TABLEIPCODE))
                SearchTableIPCode = prop_.READ_TABLEIPCODE.ToString();
            
            if (t.IsNotNull(SearchTableIPCode) == false)
                if (t.IsNotNull(prop_.TARGET_TABLEIPCODE))
                    SearchTableIPCode = prop_.TARGET_TABLEIPCODE.ToString();

            string SearchFormCode = prop_.FORMCODE.ToString();

            v.con_SearchValue = searchValue;

            /// sadece SearchTableIPCode var ise 
            if (t.IsNotNull(SearchTableIPCode) && (t.IsNotNull(SearchFormCode) == false))
                onay = searchEngineByTableIPCode(tForm, prop_, targetTableIPCode); // SearchTableIPCode);

            /// sadece SearchFormCode var ise 
            //if ((t.IsNotNull(SearchTableIPCode) == false) && t.IsNotNull(SearchFormCode))
            if (t.IsNotNull(SearchFormCode))
                onay = searchEngineByFormCode(tForm, prop_, targetTableIPCode);

            /// işi bitti
            v.con_SearchValue = "";

            return onay;
        }

        /// New    
        private bool searchEngineByTableIPCode(Form tForm, PROP_NAVIGATOR prop_, string targetTableIPCode)
        {
            /// new search engine tableIPCode

            tToolBox t = new tToolBox();
            
            bool onay = false;
            
            Form tSearchForm = createSearchForm(prop_);

            /// Open SearchForm
            /// 
            t.DialogForm_View(tSearchForm, FormWindowState.Normal);
                        
            /// set Values 
            /// 
            if (v.searchSet)
                onay = setSearchEngineValues(tForm, targetTableIPCode, null, prop_.TABLEIPCODE_LIST);
            else onay = false;
            
            if ((v.searchEnter) && (onay))
            {
                // Sonraki kontrole geç
                // burayı TableIPCode içinde bir işarete bağla
                //
                // nextControl(tForm);
            }
            /// belirli durumlarda false gelince onunda tekrar true olsun
            /// 
            v.searchEnter = true;
            v.searchSet = false;

            return onay;
        }

        private void nextControl(Form tForm)
        {
            /// set Next Control ( Arama işlemi bitince bir sonraki control'e geç
            ///
            string formName = tForm.Name.ToString();
            Control searchControl = tForm.ActiveControl;

            /// Nihayi çalışır hali bu
            /// 
            Application.OpenForms[formName].ActiveControl = searchControl;
            
            System.Windows.Forms.SendKeys.Send("{ENTER}");

            v.con_SearchValue = "";

            #region
            /*   FormCollection için örnek
            FormCollection fc = Application.OpenForms;
            foreach (Form frm in fc)
            {
                if (frm.Text == formName)
                {
                    //return true;
                }
            }
            */
            /* Bu örnekte çalışıyor
            int c = Application.OpenForms.Count;
            for (int i = 0; i < c; i++)
            {
                var x = Application.OpenForms[i].Focused;

                if (Application.OpenForms[i].Name.ToString() == formName)
                {
                    //Application.OpenForms[i].Activate();
                    //Application.OpenForms[i].Focus();
                    //Application.OpenForms[i].Select();
                    Application.OpenForms[i].ActiveControl = searchControl;
                    System.Windows.Forms.SendKeys.Send("{ENTER}");
                }
            }
            */
            #endregion


        }

        private Form createSearchForm(PROP_NAVIGATOR prop_)
        {
            tToolBox t = new tToolBox();
            tEventsForm evf = new tEventsForm();
            tInputPanel ip = new tInputPanel();

            string Caption = prop_.CAPTION.ToString();
            string FormWidth = prop_.FORM_WIDTH.ToString();
            string FormHeight = prop_.FORM_HEIGHT.ToString();
            string SearchTableIPCode = "";

            if (t.IsNotNull(prop_.READ_TABLEIPCODE))
                SearchTableIPCode = prop_.READ_TABLEIPCODE.ToString();

            if (t.IsNotNull(SearchTableIPCode) == false)
                if (t.IsNotNull(prop_.TARGET_TABLEIPCODE))
                    SearchTableIPCode = prop_.TARGET_TABLEIPCODE.ToString();

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
            evf.myFormEventsAdd(tSearchForm);

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
                ip.Create_InputPanel(tSearchForm, tabNavigationPage1, SearchTableIPCode, 1, true);


            tabPane1.Controls.Add(tabNavigationPage1);
            tabPane1.Pages.Add(tabNavigationPage1);

            ((System.ComponentModel.ISupportInitialize)(tabPane1)).EndInit();
            tabPane1.ResumeLayout(false);

            tSearchForm.Controls.Add(tabPane1);
                        
            #endregion Create tSearchForm 

            return tSearchForm;
        }

        private bool searchEngineByFormCode(Form tForm, PROP_NAVIGATOR prop_, string TargetTableIPCode)
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
            tEventsForm evf = new tEventsForm();

            Form tSearchForm = new Form();

            evf.myFormEventsAdd(tSearchForm);
            tSearchForm.Size = new Size(width, height);
            tSearchForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;

            string Prop_Navigator = @"
            0=FORMNAME:null;
            0=FORMCODE:" + SearchFormCode + @";
            0=FORMTYPE:DIALOG;
            0=FORMSTATE:NORMAL;
            ";

            t.OpenForm(tSearchForm, Prop_Navigator);

            /// New Value Set
            /// 
            if (v.searchSet)
                onay = setSearchEngineValues(tForm, TargetTableIPCode, null, prop_.TABLEIPCODE_LIST);
            else onay = false;

            
            if ((v.searchEnter) && (onay))
            {
                nextControl(tForm);
            }

            v.searchSet = false;

            return onay;
        }

        private bool checkSearchEngineValues(Form tForm, string TargetTableIPCode, List<TABLEIPCODE_LIST> TableIPCodeList)
        {
            tToolBox t = new tToolBox();
            bool onay = false;
            string targetFName = string.Empty;
            string value = string.Empty;
            string WORKTYPE = string.Empty;
            int fieldCount = 0;
            int valueCount = 0;

            DataSet dsData = null;
            DataNavigator tDataNavigator = null;
            t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TargetTableIPCode);


            #region TableIPCodeList
            if (TableIPCodeList != null)
            {
                foreach (var item in TableIPCodeList)
                {
                    targetFName = item.KEYFNAME.ToString();
                    WORKTYPE = item.WORKTYPE.ToString();

                    /// dikkat : emir setData fakat SETDATA çalıştırılmıyor onun yerine daha önce set edilmiş data sayısı okunuyor
                    /// önceden atama yapılan data var mı diye kontrol ediliyor
                    ///
                    if (WORKTYPE == "SETDATA") // dikkat : şu an set işlemi yapmıyor, sadece read gerçekleşiyor
                    {
                        try
                        {
                            value = dsData.Tables[0].Rows[tDataNavigator.Position][targetFName].ToString(); // = v.con_DataRow[sourceFName];
                            fieldCount++;
                            if (t.IsNotNull(value))
                                valueCount++;
                        }
                        catch (Exception)
                        {
                            //
                            //throw;
                        }
                    }
                }
            }
            #endregion

            //if ((fieldCount > 0) && (valueCount > 0))
            if (fieldCount == valueCount)
                onay = true;

            return onay;
        }

        /// Search işleminden sonra burada SETDATA işlemleri yapılıyor
        /// 
        public bool setSearchEngineValues(Form tForm, string TargetTableIPCode,
            List<GET_FIELD_LIST> GetFieldList,
            List<TABLEIPCODE_LIST> TableIPCodeList)
        {
            tToolBox t = new tToolBox();

            /// Seçilen DataRow 
            ///
            bool onay = false;

            Application.DoEvents();
            t.WaitFormOpen(v.mainForm, "");
            t.WaitFormOpen(v.mainForm, "Atama işlemleri yapılıyor...");

            if (v.con_DataRow != null)
            {
                #region SetValues işlemleri
                /// Seçilen Row un değerleri okunur ve bekleyen dsData ya set edilir

                string uT_FNAME = string.Empty;
                string uR_FNAME = string.Empty;

                string targetFName = string.Empty;
                string sourceFName = string.Empty;
                string MSETVALUE = string.Empty;
                string WORKTYPE = string.Empty;
                bool editing = false;
                string readValue = string.Empty;

                DataSet dsData = null;
                DataNavigator tDataNavigator = null;
                t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TargetTableIPCode);

                #region GetFieldList
                if (GetFieldList != null)
                {
                    foreach (var item in GetFieldList)
                    {
                        targetFName = item.T_FNAME.ToString();
                        sourceFName = item.R_FNAME.ToString();
                        MSETVALUE = item.MSETVALUE.ToString();
                        try
                        {
                            dsData.Tables[0].Rows[tDataNavigator.Position][targetFName] = v.con_DataRow[sourceFName];
                            editing = true;
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("DİKKAT : Hatalı alan isimleri : " + v.ENTER +
                                "Target FieldName : " + targetFName + v.ENTER +
                                "Source FieldName : " + sourceFName);
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
                        targetFName = item.KEYFNAME.ToString();
                        sourceFName = item.RKEYFNAME.ToString();
                        MSETVALUE = item.MSETVALUE.ToString();
                        WORKTYPE = item.WORKTYPE.ToString();
                                                
                        if (WORKTYPE == "SETDATA")
                        {
                            //dsData.Tables[0].Rows[tDataNavigator.Position][targetFName] = v.con_DataRow[sourceFName];
                            try
                            {
                                readValue = v.con_DataRow[sourceFName].ToString();
                                
                            }
                            catch (Exception)
                            {
                                readValue = "Err";
                                MessageBox.Show("DİKKAT : Source ( " + sourceFName + " )  fieldName sorunlu ... Target fieldName : " + targetFName);
                                //throw;
                            }
                            try
                            {
                                // readValue "" veya null olunca sorun oluyor
                                // bu neden okunan (readValue) kontrol ediliyor, gerekli düzeltme yapılıyor öyle atanıyor
                                // 
                                readValue = t.tCheckedValue(dsData, targetFName, readValue);

                                dsData.Tables[0].Rows[tDataNavigator.Position][targetFName] = readValue;

                                uT_FNAME = targetFName;
                                uR_FNAME = sourceFName;
                                editing = true;
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("DİKKAT : Target ( " + targetFName + " )  fieldName sorunlu ... Sourece fieldName : " + sourceFName);
                                //throw;
                            }
                        }

                        if (WORKTYPE == "IBOX")
                        {
                            tEventsButton evb = new tEventsButton();
                            evb.InputBox(tForm, TargetTableIPCode, item);
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
                ev.viewControlFocusedValue(tForm, TargetTableIPCode);

                #endregion SetValues işlemleri

            }

            v.IsWaitOpen = false;
            t.WaitFormClose();

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
            ev.InData_RunSQL(tSearchForm, SearchTableIPCode, v.con_SearchValue, GetFieldList, TableIPCodeList);

        }




        #endregion Search Engine / Arama Motoru

        #region findListDataEngines
        public bool findListDataEngines(Form tForm, string targetTableIPCode, string searchValue, PROP_NAVIGATOR prop_)
        {
            if (prop_ == null) return false;

            tToolBox t = new tToolBox();
            bool onay = false;

            string SearchTableIPCode = "";

            if (t.IsNotNull(prop_.READ_TABLEIPCODE))
                SearchTableIPCode = prop_.READ_TABLEIPCODE.ToString();

            if (t.IsNotNull(SearchTableIPCode) == false)
                if (t.IsNotNull(prop_.TARGET_TABLEIPCODE))
                    SearchTableIPCode = prop_.TARGET_TABLEIPCODE.ToString();


            if (t.IsNotNull(SearchTableIPCode))
            {
                DataSet dsSearch = null;
                DataNavigator dNSearch = null;
                t.Find_DataSet(tForm, ref dsSearch, ref dNSearch, SearchTableIPCode);

                if (dsSearch != null)
                {
                    string myProp_ = dsSearch.Namespace.ToString();
                    string DBaseNo = t.MyProperties_Get(myProp_, "DBaseNo:");
                    v.dBaseNo dBaseNo = t.getDBaseNo(DBaseNo);
                    string TableName = t.MyProperties_Get(myProp_, "TableName:");
                    string TableLabel = t.MyProperties_Get(myProp_, "TableLabel:");
                    string FindFName = t.MyProperties_Get(myProp_, "FindFName:");
                    string tSql = t.Set(t.MyProperties_Get(myProp_, "SqlSecond:"),
                                        t.MyProperties_Get(myProp_, "SqlFirst:"), "");
                    
                    string whereSQL = preparinSearchSql(dsSearch, TableLabel, FindFName, searchValue);

                    //string tSql = "Select * from " + schemasCode + "." + TableName + " where 0 = 0 " + findValues;

                    whereSQL =
                      " /*KISITLAMALAR|1|*/ " + v.ENTER 
                    + whereSQL + v.ENTER 
                    + " /*KISITLAMALAR|2|*/ " + v.ENTER;

                    tSql = t.kisitlamalarClear(tSql);

                    tSql = t.SQLWhereAdd(tSql, TableLabel, whereSQL, "DEFAULT");

                    t.SQL_Read_Execute(tForm, dBaseNo, dsSearch, ref tSql, TableName, "searchVaule");

                    if (t.IsNotNull(dsSearch)) onay = true;
                }
            }

            return onay;
        }

        #endregion findListDataEngines
    }
}
