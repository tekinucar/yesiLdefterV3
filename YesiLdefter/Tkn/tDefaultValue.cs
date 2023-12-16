using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Windows.Forms;
using Tkn_Save;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_DefaultValue
{
    public class tDefaultValue : tBase
    {

        // bu bölümü biraz daha düzenlemen gerekiyor

        #region *tDefaultValue_And_Validation

        public bool tDefaultValue_And_Validation(
                               Form tForm,
                               DataSet dsData_Target,
                               DataRow focus_data_row,
                               string Target_TableIPCode,
                               string Kim)
        {

            #region Tanımlar
                       
            bool onay = true;
            string s = string.Empty;
            string fname = string.Empty;
            string validation_err = string.Empty;
            string default_fill_err = string.Empty;
            string checked_sonucu = string.Empty;

            #endregion

            tToolBox t = new tToolBox();
            string function_name = "tDefaultValue_And_Validation";

            // Bu fonkisyona kimler geliyor ...

            // Kim = "btn_Navigotor_HVG_Click"
            // Kim = "tData_Save" 

            if (Kim == "tData_NewRecord")
                t.MSSQL_Server_Tarihi();

            // InputPanel (MS_FieldsIP) bilgilerinin olduğu DataSet
            DataSet dsMS_Fields_IP = t.Find_DataSet(tForm, "tDataControl_IP_Fields_" + Target_TableIPCode, "", function_name);

            #region Sırayla fieldleri doldurma ve kontrol (fill and validation) işlemi yapılacak

            if (t.IsNotNull(dsMS_Fields_IP) == false) return false;

            v.con_DefaultValuePreparing = true;

            /// onay her zaman true geliyor aslında sorun var
            /// err mesajlarıyla onay = false dönüşüyor
            ///
            onay = defaultValue_And_Validation_(tForm
                 , dsData_Target
                 , focus_data_row
                 , dsMS_Fields_IP
                 , ref default_fill_err
                 , ref validation_err
                 , Kim);

            v.con_DefaultValuePreparing = false;

            dsData_Target.Tables[0].AcceptChanges();

            #endregion Sırayla column --------------------------------------------------

            #region İşlemlerin sonunda Hataları ekrana yazdır --------------------------

            if (validation_err != string.Empty)
            {
                if (validation_err != "ROW_DELETE")
                {
                    //MessageBox.Show(validation_err, "Veri Doğrulama - Validation");
                    t.FlyoutMessage(tForm, "Veri Doğrulama - Validation", validation_err);
                }
                else
                {
                    focus_data_row.Delete();
                    dsData_Target.AcceptChanges();
                }
                onay = false;
            }

            if (default_fill_err != string.Empty)
            {
                if (default_fill_err.IndexOf("MasterTabloKayıtHatası") == -1)
                    t.FlyoutMessage(tForm, "Veri Doldurma - Default Value Fill", default_fill_err);
                //MessageBox.Show(default_fill_err, "Veri Doldurma - Default Value Fill");

                onay = false;
            }

            #endregion ------------------------------------------------------------------

            //t.Takipci(function_name, "", '}');

            return onay;
        }

        private bool defaultValue_And_Validation_(Form tForm
            , DataSet dsData_Target
            , DataRow focus_data_row
            , DataSet dsMS_Fields_IP
            , ref string default_fill_err
            , ref string validation_err
            , string Kim)
        {
            tToolBox t = new tToolBox();
            string fname = string.Empty;
            string getErr = "";
            byte dfl_type = 0;
            byte vld_operator = 0;
            bool onay = false;

            foreach (DataRow FieldRow in dsMS_Fields_IP.Tables[0].Rows)
            {
                fname = t.Set(FieldRow["LKP_FIELD_NAME"].ToString(), "", "null");

                #region Default Value Fill

                if ((Kim == "tData_NewRecord") ||
                    (Kim == "tData_Save") ||
                    (Kim == "tDC_Fill_Target_Rows") ||
                    (Kim == "tdataNavigator_PositionChanged")
                    )
                {
                    dfl_type = t.Set(FieldRow["DEFAULT_TYPE"].ToString(), FieldRow["LKP_DEFAULT_TYPE"].ToString(), (byte)0);

                    if (dfl_type != 0)
                    {
                        getErr = string.Empty;
                        
                        tDefault_Value_Fill(tForm,
                                            FieldRow,
                                            dsData_Target,
                                            focus_data_row,
                                            ref getErr,
                                            Kim);
                        
                        if (getErr != null)
                            default_fill_err = default_fill_err + getErr;
                    }
                }

                #endregion Default Value Fill

                #region Validation

                vld_operator = t.Set(FieldRow["VALIDATION_OPERATOR"].ToString(), FieldRow["LKP_VALIDATION_OPERATOR"].ToString(), (byte)0);

                if (vld_operator > 11)
                {
                    if (Kim == "tData_NewRecord")
                    {
                        //string[] controls = new string[] { };
                        //Control c = t.Find_Control(tForm, fname, "FIELDNAME", controls);
                        //if (c != null)
                        //{
                        //    c.BackColor = v.Validate_New;
                        //}
                    }

                    if ((Kim == "tData_Save") ||
                        (Kim == "tdataNavigator_PositionChanged"))
                    {
                        getErr = "";
                        getErr = tValidation_Check(FieldRow, focus_data_row);

                        if (getErr != "ROW_DELETE")
                            validation_err = validation_err + getErr;
                        else
                        {
                            validation_err = getErr; // ROW_DELETE mesajı gidecek
                            break;
                        }
                    }
                }
                
                #endregion Validation

            }

            default_fill_err = default_fill_err.Trim();
            validation_err = validation_err.Trim();

            if ((default_fill_err == "") &&
                (validation_err == "") &&
                (onay == false)) onay = true;

            return onay;
        }

        #endregion tDefaultValue_Fill_And_Validation

        #region tDefault Value Fill - SubFunction
        private bool tDefault_Value_Fill(Form tForm,
                                         DataRow Field_Row,
                                         DataSet dsTargetData,
                                         DataRow focus_row,
                                         ref string sendErrorMessage,
                                         string Kim)
        {

            tToolBox t = new tToolBox();
            
            #region Tanımlar
            bool onay = true;
            byte default_type = 0;
            byte default_type2 = 0;
            bool old_PositionChange = false;
            string s = string.Empty;
            string fname = string.Empty;
            string fcaption = string.Empty;
            string err = string.Empty;
            string fautoinc = string.Empty;
            string newValue = string.Empty;
            string myProp = string.Empty;
            string TableIPCode = string.Empty;
            string MultiPageID = string.Empty;
            #endregion

            // Kim = "btn_Navigotor_HVG_Click"
            // Kim = "tData_Save" 

            #region  Default Type  tespiti

            fname = t.Set(Field_Row["LKP_FIELD_NAME"].ToString(), "", "");
            fautoinc = t.Set(Field_Row["LKP_FAUTOINC"].ToString(), "", "False");
            fcaption = t.Set(Field_Row["FCAPTION"].ToString(), Field_Row["LKP_FCAPTION"].ToString(), fname);
            default_type = t.Set(Field_Row["DEFAULT_TYPE"].ToString(), Field_Row["LKP_DEFAULT_TYPE"].ToString(), (byte)0);
            default_type2 = t.Set(Field_Row["DEFAULT_TYPE2"].ToString(), "0", (byte)0);

            myProp = dsTargetData.Namespace;
            TableIPCode = t.MyProperties_Get(myProp, "TableIPCode:");
            MultiPageID = t.MyProperties_Get(myProp, "MultiPageID:");

            // Eğer AutoInc Field yani REF_ID ise
            if (fautoinc == "True")
            {
                sendErrorMessage = err;
                return true;
            }

            try
            {
                // daha önce column doldurulmuş ise bir daha atama yapmasın
                if (t.IsNotNull(focus_row[fname].ToString()))
                {
                    //'DEFAULT_TYPE', 21, '', 'Source TableIPCode READ'
                    if (default_type != 21)
                    {
                        sendErrorMessage = err;
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(TableIPCode + " : içinde [ " + fname + " ] field bulunamadı." + v.ENTER2 + e.Message.ToString(), "tDefault Value Fill");
            }

            #endregion Default Type  tespiti

            // Tespit edilen Default Type Göre -------------------
            #region Var // tData_NewRecord
            if ((default_type >= 1) &&
                (default_type <= 6) &&
                ((Kim == "tData_NewRecord") ||
                 (Kim == "tDC_Fill_Target_Rows") ||
                 (t.IsNotNull(focus_row[fname].ToString()) == false)
                 ))
            {
                try
                {
                    if (default_type == 1) // Var (Integer)
                    {
                        focus_row[fname] = t.Set(Field_Row["DEFAULT_INT"].ToString(), Field_Row["LKP_DEFAULT_INT"].ToString(), 0);
                    }

                    if (default_type == 2) // Var (Numeric)
                    {
                        focus_row[fname] = t.Set(Field_Row["DEFAULT_NUMERIC"].ToString(), Field_Row["LKP_DEFAULT_NUMERIC"].ToString(), (decimal)0);
                    }

                    if (default_type == 3) // Var (Text)
                    {
                        focus_row[fname] = t.Set(Field_Row["DEFAULT_TEXT"].ToString(), Field_Row["LKP_DEFAULT_TEXT"].ToString(), "");
                    }

                    if (default_type == 4) // Var (SP) SP_xxxxx
                    {
                        s = t.Set(Field_Row["DEFAULT_SP"].ToString(), Field_Row["LKP_DEFAULT_SP"].ToString(), "0");
                        if (s != "0")
                            focus_row[fname] = tSP_Value_Load(s);
                    }

                    if ((default_type == 5) || (default_type == 6)) // 5=Var (SETUP/Value) or 6=Var (SETUP/Caption) 
                    {
                        s = t.Set(Field_Row["DEFAULT_SETUP"].ToString(), Field_Row["LKP_DEFAULT_SETUP"].ToString(), "0");

                        if (t.IsNotNull(s))
                            focus_row[fname] = tSETUP_Load(s, default_type);
                    }

                    
                    //if (focus_row[fname] != null)
                    //    newValue = focus_row[fname].ToString();

                }
                catch (Exception e)
                {
                    MessageBox.Show(TableIPCode + " : içinde [ " + fname + " ] field bulunamadı." + v.ENTER2 + e.Message.ToString(), "tDefault Value Fill");
                }
            }
            #endregion Var

            #region Master-Detail & Foreing Key // Kim == full

            ///'DEFAULT_TYPE', 21, '', 'Source TableIPCode READ'); -- 33 > 21
            ///'DEFAULT_TYPE', 22, '', 'Source ParentControl.Tag READ'); 
            ///'DEFAULT_TYPE', 31, '', '1Master=Detail');
            ///'DEFAULT_TYPE', 32, '', '2Master=Detail Multi');
            ///'DEFAULT_TYPE', 41, '', 'Line No'); -- 32 > 41

            if ((default_type == 21) || // DEFAULT_TYPE', 21, '', 'Source TableIPCode READ'
                (default_type == 31))   // DEFAULT_TYPE', 31, '', 'Master=Detail'
            {
                string master_TableIPCode = t.Set(Field_Row["MASTER_TABLEIPCODE"].ToString(), Field_Row["LKP_MASTER_TABLEIPCODE"].ToString(), "");
                string master_FieldName = t.Set(Field_Row["MASTER_KEY_FNAME"].ToString(), Field_Row["LKP_MASTER_KEY_FNAME"].ToString(), "");
                DataSet dsData_Source = null;
                DataNavigator dN = null;
                int position = -1;

                // Eğer field için master-detail bağlantısı yapılmış ise

                if ((master_TableIPCode != "") && (master_FieldName != ""))
                {
                    if (master_FieldName.IndexOf("@") == 0)
                        master_FieldName = master_FieldName.Substring(1);

                    //dsData_Source = t.Find_DataSet(tForm, "", master_TableIPCode, function_name);
                    //position <<< ReadID = t.Find_DataNavigator_Position(tForm, dsData_Source, master_TableIPCode);
                    t.Find_DataSet(tForm, ref dsData_Source, ref dN, master_TableIPCode);

                    if ((dN != null) && (t.IsNotNull(master_TableIPCode)))
                    {
                        if (dN.Tag != null)
                        {
                            position = dN.Position;
                            if (Kim == "tdataNavigator_PositionChanged")
                                position = Convert.ToInt32(dN.Tag.ToString());
                        }

                        if ((dsData_Source != null) &&
                            (position > -1) &&
                            (t.IsNotNull(master_FieldName))
                            )
                        {
                            var value = dsData_Source.Tables[0].Rows[position][master_FieldName];

                            /// eğer master ID nin valuesi alınıyor ve value 0 ise 
                            /// master table önce kaydetmek gerekiyor
                            if (Kim == "tData_Save")
                            {
                                if (dsData_Source.Tables[0].Columns[0].Caption.ToString() == master_FieldName)
                                {
                                    if ((value.ToString() == "0") ||
                                        (value.ToString() == ""))
                                    {
                                        old_PositionChange = v.con_PositionChange;
                                        v.con_PositionChange = true;

                                        tSave sv = new tSave();
                                        onay = sv.tDataSave(tForm, master_TableIPCode);
                                        if (onay)
                                        {
                                            value = dsData_Source.Tables[0].Rows[position][master_FieldName];
                                        }
                                        else err = "MasterTabloKayıtHatası";

                                        if (old_PositionChange == false)
                                            v.con_PositionChange = false;
                                    }
                                }
                            }

                            try
                            {
                                focus_row[fname] = value;
                            }
                            catch (Exception e)
                            {
                                onay = false;
                                MessageBox.Show(TableIPCode + " : içinde [ " + fname + " ] field bulunamadı." + v.ENTER2 + e.Message.ToString(), "tDefault Value Fill");
                            }

                        }
                    }
                }

                if ((dsData_Source == null) || (t.IsNotNull(master_FieldName) == false))
                {
                    onay = false;
                    err = TableIPCode + " : içinde [ " + master_TableIPCode + " | " + master_FieldName + " ] masterTableIPCode / masterKeyFName bilgisinde eksiklik mevcut ..." + v.ENTER2 + err;
                }

                if (dsData_Source != null)
                {
                    dsData_Source.Dispose();
                }
            }

            #endregion Master-Detail

            #region LineNo 

            if (default_type == 41) // LineNo
            {
                focus_row[fname] = dsTargetData.Tables[0].Rows.Count + 1;
            }

            #endregion LineNo

            #region // 22, 23, 24  
            // 22 Source ParentControl.TagValue READ
            // 23 Source ParentControl.TagCaption READ 
            // 24 Source ParentControl.TagType READ

            if ((default_type == 22) || (default_type2 == 22) ||
                (default_type == 23) || (default_type2 == 23) ||
                (default_type == 24) || (default_type2 == 24))
            {

                Control cntrl = t.Find_Control_View(tForm, TableIPCode + MultiPageID);
                if (cntrl != null)
                {
                    Control parent = cntrl.Parent;
                    if (parent != null)
                    {
                        string TagValue = "";
                        
                        if (parent.Tag != null)
                            TagValue = parent.Tag.ToString();

                        // MsReports tablosundaki FormCode için gerekli 
                        // ms_Report create edilirken
                        if ((tForm.AccessibleDescription == null) && (t.IsNotNull(v.con_Source_FormCodeAndName)))
                            TagValue = v.con_Source_FormCodeAndName + "||";
                        // ms_Report createden sonra
                        if (tForm.AccessibleDescription != null)
                            TagValue = tForm.AccessibleDescription + "||";
                        //---

                        // sorunun kaynağını aramadım 
                        if (TagValue == "||||") TagValue = "";

                        if (t.IsNotNull(TagValue))
                        {
                            string value2 = string.Empty;
                            string caption = string.Empty;
                            string typeValue = string.Empty;

                            // gelen veri
                            // TagValue = "5||Merkez Kasa" 
                            if (TagValue.IndexOf("||") > -1)
                            {
                                //t.String_Parcala(TagValue, ref value, ref caption, "||");

                                value2 = t.Get_And_Clear(ref TagValue, "||");
                                caption = t.Get_And_Clear(ref TagValue, "||");
                                typeValue = t.Get_And_Clear(ref TagValue, "||");

                                try
                                {
                                    // value READ
                                    if ((default_type == 22) || (default_type2 == 22))
                                        focus_row[fname] = value2;
                                    // caption READ
                                    if ((default_type == 23) || (default_type2 == 23))
                                        focus_row[fname] = caption;
                                    // typeValue READ
                                    if ((default_type == 24) || (default_type2 == 24))
                                        focus_row[fname] = typeValue;
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(TableIPCode + " : içinde [ " + fname + " ] field bulunamadı." + v.ENTER2 + e.Message.ToString(), "tDefault Value Fill");
                                }
                            }

                            if (TagValue.IndexOf("||") == -1)
                            {
                                // henüz gerek olmadı
                            }
                        }
                        
                    }
                }

            }
            #endregion 22, Source ..

            #region Default Type çeşitleri
            /// V3 hali DEFAULT_TYPE
            ///*  0, '', '');
            ///*  1, '', 'Var (Integer)');
            ///*  2, '', 'Var (Numeric)');
            ///*  3, '', 'Var (Text)');
            ///*  4, '', 'Var (SP)');
            ///*  5, '', 'Var (Setup/Value)');
            ///*  6, '', 'Var (Setup/Caption)');
            ///*  21, '', 'Source TableIPCode READ'); -- 33 > 21
            ///*  22, '', 'Source ParentControl.TagValue READ');
            ///*  23, '', 'Source ParentControl.TagCaption READ');
            ///*  31, '', 'Master=Detail');
            ///*  32, '', 'Master=Detail Multi');
            ///*  41, '', 'Line No'); -- 32 > 41
            ///*  51, '', 'Kriter READ (Even.Bas)'); 
            ///*  52, '', 'Kriter READ (Even.Bit)'); 
            ///*  53, '', 'Kriter READ (Odd)'); 
            #endregion

            if (err != string.Empty) err = err + v.ENTER;

            //focus_row.AcceptChanges();
            //dsTargetData.Tables[0].AcceptChanges();

            sendErrorMessage = err;
            return onay;
        }
        #endregion tDefault Value Fill

        #region tValidation_Check
        public string tValidation_Check(DataRow Field_Row, DataRow data_row)
        {

            #region Tanımlar
            byte ftype = 0;
            byte vld_operator = 0;
            byte ErrType = 0;
            byte tCompare = 0;
            Boolean vld_err = false;

            string fname = string.Empty;
            string fcaption = string.Empty;
            string Value1_ = string.Empty;
            string Value2_ = string.Empty;
            string ErrText = string.Empty;
            string sonuc = string.Empty;
            string d_Value = string.Empty;
            #endregion

            tToolBox t = new tToolBox();

            fname = t.Set(Field_Row["LKP_FIELD_NAME"].ToString(), "", "null");
            fcaption = t.Set(Field_Row["FCAPTION"].ToString(), Field_Row["LKP_FCAPTION"].ToString(), fname);
            ftype = t.Set(Field_Row["LKP_FIELD_TYPE"].ToString(), "", (byte)0);
            vld_operator = t.Set(Field_Row["VALIDATION_OPERATOR"].ToString(), Field_Row["LKP_VALIDATION_OPERATOR"].ToString(), (byte)0);

            if (vld_operator > 0)
            {
                Value1_ = t.Set(Field_Row["VALIDATION_VALUE1"].ToString(), Field_Row["LKP_VALIDATION_VALUE1"].ToString(), "");
                Value2_ = t.Set(Field_Row["VALIDATION_VALUE2"].ToString(), Field_Row["LKP_VALIDATION_VALUE2"].ToString(), "");
                ErrText = t.Set(Field_Row["VALIDATION_ERRORTEXT"].ToString(), Field_Row["LKP_VALIDATION_ERRORTEXT"].ToString(), "");
                ErrType = t.Set(Field_Row["VALIDATION_ERRORTYPE"].ToString(), Field_Row["LKP_VALIDATION_ERRORTYPE"].ToString(), (byte)0);
            }

            d_Value = data_row[fname].ToString();

            tCompare = tValue_Compare(d_Value, Value1_, Value2_, ftype);

            if (vld_operator == 18) //18.Greater_Büyük
            {
                if ((tCompare != v.Buyuk) ||
                    (d_Value == "")) vld_err = true; // c.BackColor = v.Validate_Not;
                //if ((tCompare == v.Buyuk) || (tCompare == v.EsitVeBuyuk)) c.BackColor = v.Validate_Ok;
            }
            if (vld_operator == 19)//19.GreaterOrEqual_BüyükYadaEsit
            {
                //if (tCompare == v.EsitVeBuyuk) c.BackColor = v.Validate_Ok;
                if (tCompare != v.EsitVeBuyuk) vld_err = true; //c.BackColor = v.Validate_Not;
            }
            if (vld_operator == 21) //21.IsNotBlank_BosDegilse
            {
                if ((d_Value == string.Empty) ||
                    (d_Value == "") ||
                    (d_Value == "0")) vld_err = true; //c.BackColor = v.Validate_Not;
                //if (Sender_Value != string.Empty) c.BackColor = v.Validate_Ok;
            }
            if (vld_operator == 29) //29.NotEquals_EşitDeğil
            {
                if ((d_Value == Value1_) ||
                    (d_Value == "")) vld_err = true; // c.BackColor = v.Validate_Not;
                //if (Sender_Value != Value1) c.BackColor = v.Validate_Ok;
            }
            if (vld_operator == 31) // 31.IsBlankDelete_BoşiseSil (Yeni satır sırasında gerekiyor)
            {
                if (d_Value == "")
                {
                    vld_err = true;
                    ErrText = "ROW_DELETE";
                }
            }


            if (vld_err)
            {
                if ((vld_operator == 21) && (t.IsNotNull(ErrText) == false))  //21.IsNotBlank_BosDegilse
                    sonuc = fcaption + " : Boş geçilemez ";

                //else if (vld_operator == 29) //29.NotEquals_EşitDeğil
                //    sonuc = fcaption + " : Eşit değil ( "+ d_Value + " ) ";

                else sonuc = fcaption + " : " + ErrText;

                if (sonuc != string.Empty)
                    sonuc = sonuc + v.ENTER;
            }

            // 31.IsBlankDelete_BoşiseSil
            if (ErrText == "ROW_DELETE")
                sonuc = ErrText;

            return sonuc;

            //11.NotProvider_Yok
            //12.AnyOf_HerhangiBirisi
            //13.BeginsWith_IleBaslayan
            //14.Between_Arasında
            //15.Contains_Iceriyor
            //16.EndsWith_IleBiter
            //17.Equals_Esittir
            //18.Greater_Büyük
            //19.GreaterOrEqual_BüyükYadaEsit
            //20.IsBlank_BosMu
            //21.IsNotBlank_BosDegildir
            //22.Less_DahaAz
            //23.LessOrEqual_AzYadaEsit
            //24.Like_Gibi 
            //25.None_Hicbiri
            //26.NotAnyOf_HerhangiBiriDegil
            //27.NotBetween_ArasındaDegil
            //28.NotContains_IceriyorDeğil
            //29.NotEquals_DegilEşittir
            //30.NotLike_GibiDegil
            //31.IsBlankDelete_BoşiseSil
        }
        #endregion // Validation 

        #region Validating

        public Byte tValue_Compare(string InValue, string Value1, string Value2, Int16 ftype)
        {
            byte snc = 0;

            #region // değişken tipleri
            /*
            * text 175,167,99,35
            * rkm  56, 48, 127, 52, 60, 62, 59, 104, 108
            * date 58

            *  56 int
            *  52 smallint
            * 127 bigint
            *  48 tinyint
         
            *  59 real
            *  60 money
            *  62 float
            * 108 numeric
            *  
            * 167 varchar
            * 175 char
            *  35 text
            *  99 ntext
            *  
            *  58 smalldatetime
            *  40 date
            *  
            *  34 image
            * 173 binary
            * 165 varbinary
        
            * 104 bit
            */
            #endregion

            #region // * 167 varchar * 175 char * 35 text * 99 ntext
            if ((ftype == 167) | (ftype == 175) | (ftype == 35) | (ftype == 99))
            {
                try
                {
                    if (InValue == Value1) snc = v.Esit;
                    if (InValue != Value1) snc = v.EsitDegil;
                    if ((InValue == "null") || (InValue == "")) snc = v.Bos;
                }
                catch
                {
                    snc = v.Bos;
                }
            }
            #endregion

            #region // * bigint * tinyint
            if ((ftype == 127) | (ftype == 48))
            {
                ulong rkmi = 0;
                ulong rkm1 = 0;
                ulong rkm2 = 0;

                if ((InValue == "") || (InValue == "null")) InValue = "0";
                if ((Value1 == "") || (Value1 == "null")) Value1 = "0";
                if ((Value2 == "") || (Value2 == "null")) Value2 = "0";

                try
                {
                    rkmi = Convert.ToUInt64(InValue);
                    rkm1 = Convert.ToUInt64(Value1);
                    rkm2 = Convert.ToUInt64(Value2);
                    if (rkmi >= rkm1) snc = v.EsitVeBuyuk;
                    if (rkmi > rkm1) snc = v.Buyuk;
                    if (rkmi == rkm1) snc = v.Esit;
                    if (rkmi <= rkm1) snc = v.EsitVeKucuk;
                    if (rkmi < rkm1) snc = v.Kucuk;
                }
                catch
                {
                    snc = v.Bos;
                }
            }
            #endregion

            #region // * int * smallint
            if ((ftype == 56) | (ftype == 52))
            {
                int rkmi = 0;
                int rkm1 = 0;
                int rkm2 = 0;

                if ((InValue == "") || (InValue == "null")) InValue = "0";
                if ((Value1 == "") || (Value1 == "null")) Value1 = "0";
                if ((Value2 == "") || (Value2 == "null")) Value2 = "0";

                try
                {
                    rkmi = Convert.ToInt32(InValue);
                    rkm1 = Convert.ToInt32(Value1);
                    rkm2 = Convert.ToInt32(Value2);
                    if (rkmi >= rkm1) snc = v.EsitVeBuyuk;
                    if (rkmi > rkm1) snc = v.Buyuk;
                    if (rkmi == rkm1) snc = v.Esit;
                    if (rkmi <= rkm1) snc = v.EsitVeKucuk;
                    if (rkmi < rkm1) snc = v.Kucuk;
                }
                catch
                {
                    snc = v.Bos;
                }
            }
            #endregion

            #region // * 59 real * 60 money * 62 float * 108 numeric
            if ((ftype == 59) | (ftype == 60) | (ftype == 62) | (ftype == 108))
            {
                decimal rkmi = 0;
                decimal rkm1 = 0;
                decimal rkm2 = 0;

                if ((InValue == "") || (InValue == "null")) InValue = "0";
                if ((Value1 == "") || (Value1 == "null")) Value1 = "0";
                if ((Value2 == "") || (Value2 == "null")) Value2 = "0";

                try
                {
                    rkmi = Convert.ToDecimal(InValue);
                    rkm1 = Convert.ToDecimal(Value1);
                    rkm2 = Convert.ToDecimal(Value2);
                    if (rkmi >= rkm1) snc = v.EsitVeBuyuk;
                    if (rkmi > rkm1) snc = v.Buyuk;
                    if (rkmi == rkm1) snc = v.Esit;
                    if (rkmi <= rkm1) snc = v.EsitVeKucuk;
                    if (rkmi < rkm1) snc = v.Kucuk;
                }
                catch
                {
                    snc = v.Bos;
                }
            }
            #endregion

            #region // * 58 smalldatetime * 40 date
            if ((ftype == 58) | (ftype == 40) | (ftype == 61))
            {
                DateTime rkmi = DateTime.MinValue;
                DateTime rkm1 = DateTime.MinValue;
                DateTime rkm2 = DateTime.MinValue;

                if ((InValue == "") || (InValue == "null")) InValue = "01.01.1899";
                if ((Value1 == "") || (Value1 == "null")) Value1 = "01.01.1899";
                if ((Value2 == "") || (Value2 == "null")) Value2 = "01.01.1899";

                try
                {
                    rkmi = Convert.ToDateTime(InValue);
                    rkm1 = Convert.ToDateTime(Value1);
                    rkm2 = Convert.ToDateTime(Value2);
                    if (rkmi >= rkm1) snc = v.EsitVeBuyuk;
                    if (rkmi > rkm1) snc = v.Buyuk;
                    if (rkmi == rkm1) snc = v.Esit;
                    if (rkmi <= rkm1) snc = v.EsitVeKucuk;
                    if (rkmi < rkm1) snc = v.Kucuk;
                }
                catch
                {
                    snc = v.Bos;
                }
            }
            #endregion

            return snc;
        }

        public string tSP_Value_Load(string def)
        {
            string s = "";

            if ((def == "1001") | (def == "DONEM_BASI_TARIH")) s = Convert.ToString(v.DONEM_BASI_TARIH.ToShortDateString());
            if ((def == "1002") | (def == "DONEM_BITIS_TARIH")) s = Convert.ToString(v.DONEM_BITIS_TARIH.ToShortDateString());
            if ((def == "1003") | (def == "BUGUN_TARIH")) s = Convert.ToString(v.BUGUN_TARIH.ToShortDateString());
            if ((def == "1004") | (def == "BU_HAFTA_BASI_TARIH")) s = Convert.ToString(v.BU_HAFTA_BASI_TARIH.ToShortDateString());
            if ((def == "1005") | (def == "BU_HAFTA_SONU_TARIH")) s = Convert.ToString(v.BU_HAFTA_SONU_TARIH.ToShortDateString());
            if ((def == "1006") | (def == "BU_AY_BASI_TARIH")) s = Convert.ToString(v.BU_AY_BASI_TARIH.ToShortDateString());
            if ((def == "1007") | (def == "BU_AY_SONU_TARIH")) s = Convert.ToString(v.BU_AY_SONU_TARIH.ToShortDateString());
            if ((def == "1008") | (def == "BIR_HAFTA_ONCEKI_TARIH")) s = Convert.ToString(v.BIR_HAFTA_ONCEKI_TARIH.ToShortDateString());
            if ((def == "1009") | (def == "IKI_HAFTA_ONCEKI_TARIH")) s = Convert.ToString(v.IKI_HAFTA_ONCEKI_TARIH.ToShortDateString());
            if ((def == "1010") | (def == "ON_GUN_ONCEKI_TARIH")) s = Convert.ToString(v.ON_GUN_ONCEKI_TARIH.ToShortDateString());
            if ((def == "1011") | (def == "GECEN_AY_BUGUN")) s = Convert.ToString(v.GECEN_AY_BUGUN.ToShortDateString());
            if ((def == "1012") | (def == "GELECEK_AY_BUGUN")) s = Convert.ToString(v.GELECEK_AY_BUGUN.ToShortDateString());
            if ((def == "1021") | (def == "BUGUN_GUN")) s = Convert.ToString(v.BUGUN_TARIH.Day);
            if ((def == "1022") | (def == "BUGUN_AY")) s = Convert.ToString(v.BUGUN_TARIH.Month);
            if ((def == "1023") | (def == "BUGUN_YIL")) s = Convert.ToString(v.BUGUN_TARIH.Year);
            
            if ((def == "1205") | (def == "BUGUN_YILAY")) s = v.BUGUN_YILAY.ToString();
            
            if (def == "1201") s = v.TARIH_SAAT.ToString();
            if (def == "1202") s = v.SAAT_KISA.ToString();
            if (def == "1203") s = v.SAAT_UZUN.ToString();
            if (def == "1204") s = v.SAAT_UZUN2.ToString();

            /// prog açılışında 
            /// önce comp bilgileri
            /// sonra user bilgileri
            /// user için tanımlı olan user_firm_guid varsa o firm 
            /// yoksa 
            /// comp kartında tanımlı olan comp_firm_guid e göre firm çalışacak
            /// 

            if (def == "2003") s = v.tComp.SP_COMP_ID.ToString();
            if (def == "2005") s = v.tUser.UserId.ToString();
            if (def == "2001") s = v.SP_FIRM_ID.ToString();

            //if (def == "2002") s = v.vt_SHOP_ID.ToString(); İPTAL

            //PERIOD = DONEM,_ID : Bundan vazgeçildi
            //dönem gerektiren tablolarda işlem tarihine dayalı ISLEM_YIL baz alınacak
            //if (def == "2004") s = v.vt_PERIOD_ID.ToString(); İPTAL


            return s;
        }

        public string tSETUP_Load(string def, byte default_type)
        {
            /// olması gereken set etme sırası
            /// ters sıra olduğuna dikkat
            /// 
            /// 6.bilgisayar
            /// 5.kullanıcı
            /// 4.departman
            /// 3.bölüm
            /// 2.şube
            /// 1.firma

            /// kullanıcıyı   sys_user tablosundan 
            /// firmayı       sys_firm tablosundan
            /// 


            tToolBox t = new tToolBox();
            
            if ((def == "DEF_HP_FIRM_ID") || (def == "2001"))
            {
                return v.SP_FIRM_ID.ToString();
            }
            if ((def == "DEF_HP_FIRM_GUID") || (def == "2002"))
            {
                return v.tMainFirm.FirmGuid;
            }

            if ((def == "DEF_HP_USER_ID") || (def == "2005"))
            {
                return v.tUser.UserId.ToString();
            }
            if ((def == "DEF_HP_USER_GUID") || (def == "2006"))
            {
                return v.tUser.UserGUID;
            }
            if ((def == "DEF_HP_USER_FIRM_GUID") || (def == "2007"))
            {
                return v.tUser.UserFirmGUID;
            }
            
            if (t.IsNotNull(v.ds_Variables) == false)
                return "0"; //"0"; null;

            string s = string.Empty;
            int i2 = v.ds_Variables.Tables[0].Rows.Count;

            for (int i = 0; i < i2; i++)
            {
                if (v.ds_Variables.Tables[0].Rows[i]["VARIABLE_CODE"].ToString() == def)
                {
                    // value
                    if (default_type == 5) s = v.ds_Variables.Tables[0].Rows[i]["CLIENT_VALUE"].ToString();
                    // caption
                    //if (default_type == 6) s = v.ds_Variables.Tables[0].Rows[i]["LKP_CLIENT_VALUE_CAPTION"].ToString();
                    if (default_type == 6) s = v.ds_Variables.Tables[0].Rows[i]["LKP_VARIABLE_CAPTION"].ToString();
                    break;
                }
            }

            return s;




            /// VARIABLE_CODE  ÖRNEK
            /// -----------------------------
            /// DEF_HP_FIRM_ID
            /// DEF_HP_SHOP_ID
            /// DEF_HP_PART_ID
            /// DEF_HP_DEPARTMAN_ID
            /// DEF_HP_USER_ID
            /// DEF_FNS_KASA_TL_ID
            /// DEF_FNS_KASA_USA_ID
            /// DEF_FNS_KASA_EURO_ID
            /// DEF_FNS_BANKA_HES_ID
            /// DEF_FNS_BANKA_POS_ID
            /// DEF_FNS_BANKA_KK_ID

        }

        #endregion Validating

    }


    /*
    public static void myEdit_Validating(object sender, CancelEventArgs e)
    {
        string function_name = "myEdit_Validating";

        #region Tanımlar

        int row = 0;
        int position = 0;
        Int16 ftype = 0;
        Int16 lkp_vld_operator = 0;
        Int16 vld_operator = 0;
        Int16 ErrType = 0;
        Int16 Operator = 0;
        byte tCompare = 0;

        string TableIPCode = string.Empty;
        string field_name = string.Empty;
        string Sender_Value = string.Empty;
        string Value1 = string.Empty;
        string Value2 = string.Empty;
        string ErrText = string.Empty;

        Form tForm = null;

        #endregion

        myEdit_(sender, ref TableIPCode, ref tForm, ref row);

        #region

        if (TableIPCode != string.Empty)
        {
            #region // Read
            DataSet dsMS_IP_Fields = tDataSet_Find(tForm, "tDataControl_IP_Fields_" + TableIPCode, "", function_name);
            if (dsMS_IP_Fields != null)
            {
                field_name = dsMS_IP_Fields.Tables[0].Rows[row]["LKP_FIELD_NAME"].ToString();
                ftype = Convert.ToInt16(dsMS_IP_Fields.Tables[0].Rows[row]["LKP_FIELD_TYPE"].ToString());
                lkp_vld_operator = Convert.ToInt16(tp.IsNull(dsMS_IP_Fields.Tables[0].Rows[row]["LKP_VALIDATION_OPERATOR"].ToString(), "0"));
                vld_operator = Convert.ToInt16(tp.IsNull(dsMS_IP_Fields.Tables[0].Rows[row]["VALIDATION_OPERATOR"].ToString(), "0"));

                if ((lkp_vld_operator > 11) || (vld_operator > 11))
                {
                    if ((lkp_vld_operator > 11) && (vld_operator < 12))
                    {
                        Operator = Convert.ToInt16(tp.IsNull(dsMS_IP_Fields.Tables[0].Rows[row]["LKP_VALIDATION_OPERATOR"].ToString(), "0"));
                        Value1 = tp.IsNull(dsMS_IP_Fields.Tables[0].Rows[row]["LKP_VALIDATION_VALUE1"].ToString(), "");
                        Value2 = tp.IsNull(dsMS_IP_Fields.Tables[0].Rows[row]["LKP_VALIDATION_VALUE2"].ToString(), "");
                        ErrText = tp.IsNull(dsMS_IP_Fields.Tables[0].Rows[row]["LKP_VALIDATION_ERRORTEXT"].ToString(), "Hata");
                        ErrType = Convert.ToInt16(tp.IsNull(dsMS_IP_Fields.Tables[0].Rows[row]["LKP_VALIDATION_ERRORTYPE"].ToString(), "0"));
                    }
                    if (vld_operator > 11)
                    {
                        Operator = Convert.ToInt16(tp.IsNull(dsMS_IP_Fields.Tables[0].Rows[row]["VALIDATION_OPERATOR"].ToString(), "0"));
                        Value1 = tp.IsNull(dsMS_IP_Fields.Tables[0].Rows[row]["VALIDATION_VALUE1"].ToString(), "");
                        Value2 = tp.IsNull(dsMS_IP_Fields.Tables[0].Rows[row]["VALIDATION_VALUE2"].ToString(), "");
                        ErrText = tp.IsNull(dsMS_IP_Fields.Tables[0].Rows[row]["VALIDATION_ERRORTEXT"].ToString(), "Hata");
                        ErrType = Convert.ToInt16(tp.IsNull(dsMS_IP_Fields.Tables[0].Rows[row]["VALIDATION_ERRORTYPE"].ToString(), "0"));
                    }
                }
            }
            #endregion
        
            #region // Data Validating

            //11.NotProvider_Yok
            //12.AnyOf_HerhangiBirisi
            //13.BeginsWith_IleBaslayan
            //14.Between_Arasında
            //15.Contains_Iceriyor
            //16.EndsWith_IleBiter
            //17.Equals_Esittir
            //18.Greater_Büyük
            //19.GreaterOrEqual_BüyükYadaEsit
            //20.IsBlank_BosMu
            //21.IsNotBlank_BosDegildir
            //22.Less_DahaAz
            //23.LessOrEqual_AzYadaEsit
            //24.Like_Gibi 
            //25.None_Hicbiri
            //26.NotAnyOf_HerhangiBiriDegil
            //27.NotBetween_ArasındaDegil
            //28.NotContains_IceriyorDeğil
            //29.NotEquals_DegilEşittir
            //30.NotLike_GibiDegil

            DataSet dsData = DevExpCC.tDataSet_Find(tForm, "", TableIPCode, function_name);

            if (dsData != null)
            {
                position = tDataNavigator_Position_Find(tForm, TableIPCode);
                Sender_Value = dsData.Tables[0].Rows[position][field_name].ToString();

                string[] controls = new string[] { };
                Control c = DevExpCC.Control_Find(tForm, field_name, "FIELDNAME", controls);
                if (c != null)
                {
                    // o an henuz dataset e post edilmemiş bilgi var onu ele alıyoruz
                    Sender_Value = c.Text;

                    tCompare = tValue_Compare(Sender_Value, Value1, Value2, ftype);

                    if (Operator == 18) //18.Greater_Büyük
                    {
                        if ((tCompare != v.Buyuk) && (tCompare == v.EsitVeBuyuk)) c.BackColor = v.Validate_Not;
                        if ((tCompare == v.Buyuk) || (tCompare == v.EsitVeBuyuk)) c.BackColor = v.Validate_Ok;
                    }
                    if (Operator == 19)//19.GreaterOrEqual_BüyükYadaEsit
                    {
                        if (tCompare == v.EsitVeBuyuk) c.BackColor = v.Validate_Ok;
                        if (tCompare != v.EsitVeBuyuk) c.BackColor = v.Validate_Not;
                    }
                    if (Operator == 21) //21.IsNotBlank_BosDegildir
                    {
                        if (Sender_Value == string.Empty) c.BackColor = v.Validate_Not;
                        if (Sender_Value != string.Empty) c.BackColor = v.Validate_Ok;
                    }
                    if (Operator == 29) //29.NotEquals_DegilEşittir
                    {
                        if (Sender_Value == Value1) c.BackColor = v.Validate_Not;
                        if (Sender_Value != Value1) c.BackColor = v.Validate_Ok;
                    }
                }

            }
            #endregion
        }

        #endregion

    }
    */

}
