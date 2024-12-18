using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Windows.Forms;
using Tkn_DefaultValue;
using Tkn_Events;
using Tkn_Save;
using Tkn_SQLs;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_DataCopy
{
    public class tDataCopy : tBase
    {

        #region *tDC_Run

        //public Boolean tDC_Run(Form tForm, SqlConnection SqlConn, string DC_Code)
        public Boolean tDC_Run(Form tForm, string DC_Code)
        {
            tToolBox t = new tToolBox();
            string function_name = "tDC_Run";
            //t.Takipci(function_name, "", '{');

            #region Tanımlar

            bool onay = true;
            DataSet ds_DC = new DataSet();
            DataSet ds_DCLine = new DataSet();

            // DataCopy bilgilerini oku
            if (tDC_Read(ds_DC, ds_DCLine, DC_Code) == false)
                return false;

            // Okunan bilgiler
            string dc_caption = t.Set(ds_DC.Tables[0].Rows[0]["DC_CAPTION"].ToString(), "", "");
            string dc_about = t.Set(ds_DC.Tables[0].Rows[0]["DC_ABOUT"].ToString(), "", "");
            string question = t.Set(ds_DC.Tables[0].Rows[0]["QUESTION"].ToString(), "", "");
            string final_message = t.Set(ds_DC.Tables[0].Rows[0]["FINAL_MESSAGE"].ToString(), "", "");
            Int16 dc_type = t.Set(ds_DC.Tables[0].Rows[0]["DC_TYPE"].ToString(), "", (Int16)0);
            Int16 row_type = t.Set(ds_DC.Tables[0].Rows[0]["TROW_TYPE"].ToString(), "", (Int16)0);
            Int16 event_type = t.Set(ds_DC.Tables[0].Rows[0]["EVENT_TYPE"].ToString(), "", (Int16)0);
            Int16 work_type = t.Set(ds_DC.Tables[0].Rows[0]["WORK_TYPE"].ToString(), "", (Int16)0);
            Int16 display_type = t.Set(ds_DC.Tables[0].Rows[0]["DISPLAY_TYPE"].ToString(), "", (Int16)0);
            string target_table_code = t.Set(ds_DC.Tables[0].Rows[0]["TARGET_TABLE_CODE"].ToString(), "", "");
            string target_ip_code = t.Set(ds_DC.Tables[0].Rows[0]["TARGET_IP_CODE"].ToString(), "", "");
            string source_table_code = t.Set(ds_DC.Tables[0].Rows[0]["SOURCE_TABLE_CODE"].ToString(), "", "");
            string source_ip_code = t.Set(ds_DC.Tables[0].Rows[0]["SOURCE_IP_CODE"].ToString(), "", "");
            string source_checkfname = t.Set(ds_DC.Tables[0].Rows[0]["SOURCE_CHECK_FNAME"].ToString(), "", "");
            string source_checkvalue = t.Set(ds_DC.Tables[0].Rows[0]["SOURCE_CHECK_VALUE"].ToString(), "", "");
            string source_operandType = t.Set(ds_DC.Tables[0].Rows[0]["SOURCE_OPERAND_TYPE"].ToString(), "", "");
            string target_checkfname = t.Set(ds_DC.Tables[0].Rows[0]["TARGET_CHECK_FNAME"].ToString(), "", "");
            string target_checkvalue = t.Set(ds_DC.Tables[0].Rows[0]["TARGET_CHECK_VALUE"].ToString(), "", "");
            string target_operandType = t.Set(ds_DC.Tables[0].Rows[0]["TARGET_OPERAND_TYPE"].ToString(), "", "");
            bool sourceOnay = true;
            bool targetOnay = true;
            string read_value = "";

            /// * 'TROW_TYPE'  row_type
            /// * 0, 'none'
            /// * 1, 'Single Row'
            /// * 2, 'Multi Rows'
            /// * 3, 'Selected Rows'

            /// * work_type
            /// * 0, 1   Önce atamaları yap, sonra Kayıt yapma
            /// *    2   Önce atamaları yap, sonra Kaydet
            /// *    3   Satır Düzenle, Row.Update()
            /// *    4   Satır Sil, Row.Delete()
            /// *    5   DragDrop ile, Form.New(),Row.New()

            // Hedef tablo, işlem yapılan tablo
            string Target_TableIPCode = target_table_code + "." + target_ip_code;
            // Kaynak tablo
            string Source_TableIPCode = source_table_code + "." + source_ip_code;

            #endregion Tanımlar

            #region Eksik bilgi kontrolü --------------------- 

            if (t.IsNotNull(target_table_code) == false)
            {
                onay = false;
                MessageBox.Show("DİKKAT : " + DC_Code + v.ENTER2 +
                                "Kodlu DataCopy fonksiyonunda Target Table Code eksik...", function_name);
                return onay;
            }

            if (t.IsNotNull(target_ip_code) == false)
            {
                onay = false;
                MessageBox.Show("DİKKAT : " + DC_Code + v.ENTER2 +
                                "Kodlu DataCopy fonksiyonunda Target IP Code eksik...", function_name);
                return onay;
            }

            if (t.IsNotNull(source_table_code) == false)
            {
                onay = false;
                MessageBox.Show("DİKKAT : " + DC_Code + v.ENTER2 +
                                "Kodlu DataCopy fonksiyonunda Source Table Code eksik...", function_name);
                return onay;
            }

            if (t.IsNotNull(source_ip_code) == false)
            {
                onay = false;
                MessageBox.Show("DİKKAT : " + DC_Code + v.ENTER2 +
                                "Kodlu DataCopy fonksiyonunda Source IP Code eksik...", function_name);
                return onay;
            }

            //  3, 'Selected Row'
            if (row_type == 3)
            {
                // dragdroptan çağrılan işlem değilse
                if (Source_TableIPCode.IndexOf("DRAGDROP") == -1)
                {
                    if ((t.IsNotNull(source_checkfname) == false) ||
                        (t.IsNotNull(source_checkvalue) == false))
                    {
                        MessageBox.Show("DİKKAT : Eksik bilgi mevcut ..." + v.ENTER +
                            "source_checkfname : " + source_checkfname + v.ENTER +
                            "source_checkvalue : " + source_checkvalue, function_name);
                        return onay;
                    }
                }
            }
            
            #endregion eksik bilgi kontrolü

            #region işlem yapılacak Kaynak ve Hedef DataSetleri

            DataSet ds_Target = null;
            DataSet ds_Source = null;
            DataNavigator dN_Target = null;
            DataNavigator dN_Source = null;

            //ds_Target = t.Find_DataSet(tForm, "", Target_TableIPCode, "");
            t.Find_DataSet(tForm, ref ds_Target, ref dN_Target, Target_TableIPCode);

            if (ds_Target == null) return false;

            /// dc_run çalışması hakkında 
            /// work_type in (1,2,3,4)
            /// Normal DataCopy
            ///   kaynak (source) belli, hedef (target) belli olan tablolar arasında data kopyalama işlemi
            /// work_type in (5)
            /// YeniForm, YeniRow DataCopy
            ///   dragdrop sırasında kaynak (source) ile hedef (target) tablonun üzerine bırakılıyor fakat
            ///   bu iki tablonun verileri kullanılarak yeni bir form açarak 
            ///   üçüncü bir tabloda yeni satır oluşturma işlemi

            if (work_type == 5)
            {
                // dragdrop başka bir form üzerinde gerçekleşiyor ve
                // oradan yeni bir form oluşturuluyor 
                // bu yeni oluşan form şu an buradaki tForm
                // dragdrop un oluştuğu yeni form ise v.con_DragDropForm da

                Target_TableIPCode = v.con_DragDropOpenFormInTableIPCode;
                t.Find_DataSet(tForm, ref ds_Target, ref dN_Target, Target_TableIPCode);

                Source_TableIPCode = v.con_DragDropSourceTableIPCode;
                t.Find_DataSet(tForm, ref ds_Source, ref dN_Source, Source_TableIPCode);
            }
            else
            {
                //ds_Source = t.Find_DataSet(tForm, "", Source_TableIPCode, "");
                t.Find_DataSet(tForm, ref ds_Source, ref dN_Source, Source_TableIPCode);
            }

            if ((t.IsNotNull(source_checkfname)) &
                (t.IsNotNull(source_checkvalue)) &
                (t.IsNotNull(source_operandType)) &
                (dN_Source.Position > -1))
            {
                read_value = ds_Source.Tables[0].Rows[dN_Source.Position][source_checkfname].ToString();
                if (read_value == "") read_value = "0";
                sourceOnay = t.myOperandControl(read_value, source_checkvalue, source_operandType);
                if (sourceOnay == false) return sourceOnay;
            }
            if ((t.IsNotNull(target_checkfname)) &
                (t.IsNotNull(target_checkvalue)) &
                (t.IsNotNull(target_operandType)) &
                (dN_Target.Position > -1))
            {
                read_value = ds_Target.Tables[0].Rows[dN_Target.Position][target_checkfname].ToString();
                if (read_value == "") read_value = "0";
                targetOnay = t.myOperandControl(read_value, target_checkvalue, target_operandType);
                if (targetOnay == false) return targetOnay;
            }

            #endregion işlem yapılacak Kaynak ve Hedef DataSetleri

            #region Soru var ise
            if (t.IsNotNull(question))
            {
                DialogResult answer = t.mySoru(question);

                switch (answer)
                {
                    case DialogResult.Yes:
                        {
                            onay = true;
                            break; // break ifadesini sakın silme
                        }
                    case DialogResult.No:
                        {
                            onay = false;
                            break; // break ifadesini sakın silme
                        }
                    case DialogResult.Cancel:
                        {
                            onay = false;
                            break; // break ifadesini sakın silme
                        }
                }

                if (onay == false) return onay;
            }
            #endregion Soru var ise

            #region DataCopy / Fill Rows

            Control target_cntrl = new Control();
            target_cntrl = t.Find_Control_View(tForm, Target_TableIPCode);

            /// * work_type
            /// * 0, 1   Önce atamaları yap, sonra Kayıt yapma
            /// *    2   Önce atamaları yap, sonra Kaydet
            /// *    3   Satır Düzenle, Row.Update()
            /// *    4   Satır Sil, Row.Delete()
            /// *    5   DragDrop ile yeni fiş, Form.New(),Row.New()

            // Update, Edit değilse ve DragDropEdit false ise geri dön
            #region (work_type == 3)
            if ((work_type == 3) && (v.con_DragDropEdit == false))
            {

                // Mouse ile bir kaynaktan bir veriyi alıp bir Grid üzerine götürüp bıraktınızda 
                // doğru satırı bulmak gerekiyor ( DataNavigator.Position ) 

                // bunun için geri dönüp DataNavigator ün position u 
                // mouse hangi kaydın üstünde ise ona göre set edilmesi gerekiyor
                // dönüyor, myGridView_MouseMove de set oluyor ve oradan buraya tekrar gönderiliyor
                // tekrar geldiğinde v.con_DragDropEdit == true olduğu için burayı atlıyor ve
                // 3. work_type = ( Update, Edit ) işini yapıyor
                if ((target_cntrl.GetType().ToString() == "DevExpress.XtraGrid.Views.Grid.GridView") ||
                    (target_cntrl.GetType().ToString() == "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView") ||
                    (target_cntrl.GetType().ToString() == "DevExpress.XtraGrid.Views.Layout.LayoutView") ||
                    (target_cntrl.GetType().ToString() == "DevExpress.XtraVerticalGrid.VGridControl") ||
                    (target_cntrl.GetType().ToString() == "DevExpress.XtraTreeList.TreeList") ||
                    (target_cntrl.GetType().ToString() == "DevExpress.XtraDataLayout.DataLayoutControl")
                   )
                {
                    v.con_DragDropEdit = true;
                    return false;

                    /// Buradan dönünce myGridView_MouseMove gidiyor ve orada
                    /// if (v.con_DragDropEdit) { } işlemlerde 
                    /// mousenin hangi kayıt üzerinde olduğu tespit ediliyor ve 
                    /// myDragDrop_RUN_() tekrar tetikleniyor
                }
            }
            #endregion


            if ((work_type == 1) ||   // 
                (work_type == 2) ||   //
                (work_type == 3) ||   // Update, Edit
                (work_type == 5)
                )
                onay = tFill_Target_Rows(tForm,
                             ds_Target, ds_Source, ds_DCLine,
                             row_type, work_type,
                             Target_TableIPCode, Source_TableIPCode,
                             source_checkfname, source_checkvalue);


            // Delete
            if (work_type == 4)
                tDelete_Target_Rows(tForm, ds_Target, ds_Source, ds_DCLine, row_type, Target_TableIPCode, Source_TableIPCode);

            #endregion DataCopy / Fill Rows

            #region İşbitiminde 

            if ((event_type == 1) && (onay))
            {
                // tdataNavigator_PositionChanged pas geçsin
                v.con_Cancel = true;
                // silme butonu için click oluştur
                NavigatorButton btn = dN_Source.Buttons.Remove;
                // silme butonu çalısın
                dN_Source.Buttons.DoClick(btn);
                // değişikler onaylasın
                ds_Source.AcceptChanges();

            }

            // işlem yapılan View varsa, nesnenin Enabled i ayarla  

            if ((target_cntrl != null) && (ds_Target != null))
            {
                // TEKRAR AÇMA ÜZERİNE YENI INSERT EDİLEN DATA GİDİYOR
                // t.TableRefresh(tForm, ds_Target);
                ////t.Control_Enabled(ds_Target, target_cntrl);
            }

            // Finalda mesaj varsa
            if (t.IsNotNull(final_message) && (onay))
            {
                MessageBox.Show(final_message, "İşlem sonu mesaj :");
            }

            #endregion İşbitiminde 

            #region işlem İPTAL olursa
            if (onay == false)
            {
                //MessageBox.Show("DİKKAT : İşlem İPTAL edilmiştir.", "İşlem sonu mesaj :");
                v.Kullaniciya_Mesaj_Var = "DİKKAT : İşlem İPTAL edilmiştir.";

                // işlem iptal olduğu için yeni oluşan satırı sil
                if ((work_type == 1) ||
                    (work_type == 2))
                {
                    // tdataNavigator_PositionChanged pas geçsin
                    v.con_Cancel = true;
                    // yeni satıra konumlansın
                    dN_Target.Position = ds_Target.Tables[0].Rows.Count - 1;
                    // silme butonu için click oluştur
                    NavigatorButton btn = dN_Target.Buttons.Remove;
                    // silme butonu çalısın
                    dN_Target.Buttons.DoClick(btn);
                    // değişikler onaylasın
                    ds_Target.AcceptChanges();
                }
            }
            #endregion

            //t.Takipci(function_name, "", '}');

            return onay;
        }

        #endregion *tDC_Run

        #region *tDC Sub Function

        private bool tDC_Read(DataSet ds_DC, DataSet ds_DCLine, string DC_Code)
        {
            tToolBox t = new tToolBox();

            bool onay = true;

            // DataCopy
            t.preparing_DataCopyList(DC_Code);

            DataTable dt = v.ds_DataCopy.Tables[DC_Code];
            if (dt == null) onay = false;
            if (onay)
            {
                ds_DC.Tables.Add(dt.Copy());
                dt.Dispose();
            }

            if (onay)
            {
                // DataCopyLines
                t.preparing_DataCopyLinesList(DC_Code);

                DataTable dtL = v.ds_DataCopyLines.Tables[DC_Code];
                if (dtL == null) onay = false;
                if (onay)
                {
                    if (dtL.Rows.Count > 0)
                        ds_DCLine.Tables.Add(dtL.Copy());
                    else onay = false; // satırları yok ise

                    dtL.Dispose();
                }
            }

            if (onay == false)
            {
                MessageBox.Show("DİKKAT : " + DC_Code + v.ENTER2 +
                                "kodlu DataCopy bilgileri okunamadı ... ", "tDC_Read");
            }
            
            return onay;
        }

        private void tDelete_Target_Rows(Form tForm,
                     DataSet ds_Target, DataSet ds_Source, DataSet ds_DCLine,
                     Int16 trow_type,
                     string Target_TableIPCode, string Source_TableIPCode)
        {
            tToolBox t = new tToolBox();
            //string function_name = "DC for tDelete_Target_Rows";

            #region Tanımlar

            Int16 dc_source_type = 0;
            string target_tableipcode = string.Empty;
            string target_fieldname = string.Empty;
            //string source_tableipcode = string.Empty;
            //string source_fieldname = string.Empty;
            //string inputbox_message = string.Empty;
            //Int16 inputbox_type = 0;
            //string inputbox_default1 = string.Empty;
            //string inputbox_default2 = string.Empty;
            string Read_Value = string.Empty;
            //byte chechk_field = 0;
            //int LineNo = 1;

            int pos = 0;
            int i1 = ds_DCLine.Tables[0].Rows.Count;

            string Sql = string.Empty;
            string table_name = string.Empty;
            string where_and = string.Empty;

            #endregion

            /// * 'TROW_TYPE'  row_type
            /// * 0, 'none'
            /// * 1, 'Single Row'
            /// * 2, 'Multi Rows'
            /// * 3, 'Selected Rows'

            for (int i = 0; i < i1; i++)
            {
                #region Read DCLine

                dc_source_type = t.Set(ds_DCLine.Tables[0].Rows[i]["DC_SOURCE_TYPE"].ToString(), "", (Int16)0);
                target_tableipcode = Target_TableIPCode; //t.Set(ds_DCLine.Tables[0].Rows[i]["TARGET_TABLEIPCODE"].ToString(), Target_TableIPCode, "");
                target_fieldname = t.Set(ds_DCLine.Tables[0].Rows[i]["TARGET_FIELDNAME"].ToString(), "", "");
                //source_tableipcode = t.Set(ds_DCLine.Tables[0].Rows[i]["SOURCE_TABLEIPCODE"].ToString(), Source_TableIPCode, "");
                //source_fieldname = t.Set(ds_DCLine.Tables[0].Rows[i]["SOURCE_FIELDNAME"].ToString(), "", "");
                //inputbox_message = t.Set(ds_DCLine.Tables[0].Rows[i]["INPUTBOX_MESSAGE"].ToString(), "", "");
                //inputbox_type = t.Set(ds_DCLine.Tables[0].Rows[i]["INPUTBOX_TYPE"].ToString(), "", (Int16)0);
                //inputbox_default1 = t.Set(ds_DCLine.Tables[0].Rows[i]["INPUTBOX_DEFAULT1"].ToString(), "", "");
                //inputbox_default2 = t.Set(ds_DCLine.Tables[0].Rows[i]["INPUTBOX_DEFAULT2"].ToString(), "", "");
                //chechk_field = t.Set(ds_DCLine.Tables[0].Rows[i]["CHECK_FIELD"].ToString(), "", (byte)0); ;

                #endregion Read DCLine

                #region //  1, 'Single Row'
                if (trow_type == 1)
                {
                    if ((dc_source_type > 0) && (target_fieldname != ""))
                    {
                        pos = t.Find_DataNavigator_Position(tForm, target_tableipcode);
                        Read_Value = t.Set(ds_Target.Tables[0].Rows[pos][target_fieldname].ToString(), "", "-1");

                        where_and = where_and + " and " + target_fieldname + " = " + Read_Value + " ";
                    }
                }
                #endregion

            }

            table_name = t.Set(ds_Target.Tables[0].TableName.ToString(), "", "");

            Sql = " delete " + table_name
                + " where 0 = 0 "
                + where_and;

            if (t.IsNotNull(table_name) && t.IsNotNull(where_and))
            {
                try
                {
                    //t.SQL_ExecuteNon(ds_Target, ref Sql, null);

                    DataSet ds_Query = new DataSet();
                    t.SQL_Read_Execute(v.dBaseNo.Project, ds_Query, ref Sql, table_name, "");

                    v.tButtonHint.Clear();
                    v.tButtonHint.tForm = tForm;
                    v.tButtonHint.tableIPCode = Target_TableIPCode;
                    v.tButtonHint.buttonType = v.tButtonType.btListele;
                    tEventsButton evb = new tEventsButton();
                    evb.btnClick(v.tButtonHint);

                    v.tButtonHint.Clear();
                    v.tButtonHint.tForm = tForm;
                    v.tButtonHint.tableIPCode = Source_TableIPCode;
                    v.tButtonHint.buttonType = v.tButtonType.btListele;
                    //tEventsButton evb = new tEventsButton();
                    evb.btnClick(v.tButtonHint);
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }

        private Boolean tFill_Target_Rows(Form tForm,
                DataSet ds_Target, DataSet ds_Source, DataSet ds_DCLine,
                Int16 row_type, Int16 work_type,
                string Target_TableIPCode, string Source_TableIPCode,
                string source_checkfname, string source_checkvalue)
        {

            if (ds_Target == null)
            {
                MessageBox.Show("DİKKAT : Data kaynağı bulunamadı. " + Target_TableIPCode);
                return false;
            }

            if (ds_Source == null)
            {
                MessageBox.Show("DİKKAT : Data kaynağı bulunamadı. " + Source_TableIPCode);
                return false;
            }

            tToolBox t = new tToolBox();
            string function_name = "tDC_Fill_Target_Rows";

            tDefaultValue df = new tDefaultValue();

            /// * 'TROW_TYPE'  row_type
            /// * 0, 'none'
            /// * 1, 'Single Row'
            /// * 2, 'Multi Rows'
            /// * 3, 'Selected Rows'

            /// * work_type
            /// * 0, 1   Önce atamaları yap, sonra Kayıt yapma
            /// *    2   Önce atamaları yap, sonra Kaydet
            /// *    3   Satır Düzenle, Row.Update()
            /// *    4   Satır Sil, Row.Delete()
            /// *    5   DragDrop ile yeni fiş, Form.New(),Row.New()

            #region Tanımlar

            bool onay = true;
            bool lkp_onay = true;

            string target_tableipcode = string.Empty;
            string target_fieldname = string.Empty;
            string source_tableipcode = string.Empty;
            string source_fieldname = string.Empty;

            string inputbox_message = string.Empty;
            string inputbox_default1 = string.Empty;
            string inputbox_default2 = string.Empty;
            string Read_Value = string.Empty;

            int i1 = ds_DCLine.Tables[0].Rows.Count;
            int iSourceCount = 0;

            iSourceCount = ds_Source.Tables[0].Rows.Count;

            #endregion Tanımlar

            //--------------------------------------------------------------------------------

            #region işleme başla

            //bos row satır/larını oluştur 
            DataRow New_Row = null;
            //DataNavigator tDataNavigator = null;

            // burayı seçenekli hale getir
            // 1. Dataseti boşalt             : boş sayfa oluştur kayıt ekle
            // 2. Dataset üzerinden devam et  : mevcut satırlar üzerine yeni row ekle

            // 1.
            // t.TableClean(tForm, ds_Target);
            // 2.
            // hiç bir şey yapma

            // ilk atamada kaydetmesin kullanıcıyı beklesin
            // ds_Target.Tables[0].Namespace = "NewRecord";


            #region //  1, 'Single Row'
            if (row_type == 1)
            {
                /// * work_type
                /// * 0, 1   Önce atamaları yap, sonra Kayıt yapma
                /// *    2   Önce atamaları yap, sonra Kaydet
                /// *    3   Satır Düzenle, Row.Update()
                /// *    4   Satır Sil, Row.Delete()
                /// *    5   DragDrop ile yeni fiş, Form.New(),Row.New()

                #region // target / hedef tablonun default_value hazırla
                if ((work_type == 1) || (work_type == 2))
                {
                    New_Row = ds_Target.Tables[0].NewRow();

                    df.tDefaultValue_And_Validation
                                    (tForm,
                                     ds_Target,
                                     New_Row,
                                     Target_TableIPCode,
                                     function_name);

                    // VALIDATION_INSERT çalışması için Insert olmasın diye
                    //ds_Target.Tables[0].Namespace = "NewRecord";
                    ds_Target.Tables[0].CaseSensitive = true;
                    ds_Target.Tables[0].Rows.Add(New_Row);
                }
                #endregion

                onay = Fill_Line(tForm, ds_Target, ds_Source, ds_DCLine,
                                 work_type, Target_TableIPCode, Source_TableIPCode, -1);

                if (onay == false)
                    return onay;
            }
            #endregion

            #region //  2, 'Multi Rows'  or  3, 'Selected Row'
            if ((row_type == 2) ||
                (row_type == 3))
            {
                // her zaman tüm kaynak satırlar okunsun, işlem yapılsın
                if (row_type == 2) lkp_onay = true;

                for (int iSourcePos = 0; iSourcePos < iSourceCount; iSourcePos++)
                {
                    if ((row_type == 3) && t.IsNotNull(source_checkfname)) // Selected Row lara bakarak işlem yap
                    {
                        string value = ds_Source.Tables[0].Rows[iSourcePos][source_checkfname].ToString();

                        if (value == source_checkvalue)
                            lkp_onay = true;
                        else lkp_onay = false;
                    }

                    if (lkp_onay)
                    {
                        /// * work_type
                        /// * 0, 1   Önce atamaları yap, sonra Kayıt yapma
                        /// *    2   Önce atamaları yap, sonra Kaydet
                        /// *    3   Satır Düzenle, Row.Update()
                        /// *    4   Satır Sil, Row.Delete()
                        /// *    5   DragDrop ile yeni fiş, Form.New(),Row.New()

                        #region // target / hedef tablonun default_value hazırla
                        if ((work_type == 1) ||
                            (work_type == 2))
                        {


                            //ds_Target.Tables[0].Namespace = "NewRecord";
                            ds_Target.Tables[0].CaseSensitive = true;

                            New_Row = ds_Target.Tables[0].NewRow();

                            df.tDefaultValue_And_Validation
                                            (tForm,
                                             ds_Target,
                                             New_Row,
                                             Target_TableIPCode,
                                             function_name);

                            //ds_Target.Tables[0].Namespace = "NewRecord";
                            ds_Target.Tables[0].CaseSensitive = true;
                            ds_Target.Tables[0].Rows.Add(New_Row);
                        }
                        #endregion

                        onay = Fill_Line(tForm, ds_Target, ds_Source, ds_DCLine,
                                         work_type, Target_TableIPCode, Source_TableIPCode, iSourcePos);

                        if (onay == false)
                            break;
                    }

                }

                if (onay == false) 
                    return onay;

                t.TableRefresh(tForm, ds_Target);

            }
            #endregion //  2, 'Multi Rows'  3, 'Selected Row'

            #endregion 

            return onay;
        }


        private Boolean Fill_Line(Form tForm,
                                  DataSet ds_Target, DataSet ds_Source, DataSet ds_DCLine,
                                  Int16 work_type,
                                  string Target_TableIPCode, string Source_TableIPCode,
                                  int Source_Position)
        {
            tToolBox t = new tToolBox();
            vUserInputBox iBox = new vUserInputBox();

            DataSet ds_Source_frk = null;
            DataNavigator dN_Target = t.Find_DataNavigator(tForm, Target_TableIPCode);

            bool onay = true;
            Int16 dc_source_type = 0;
            string target_tableipcode = string.Empty;
            string target_fieldname = string.Empty;
            string source_tableipcode = string.Empty;
            string source_fieldname = string.Empty;
            string inputbox_message = string.Empty;
            string inputbox_default1 = string.Empty;
            string inputbox_default2 = string.Empty;
            string Read_Value = string.Empty;
            string chechk_field = string.Empty;
            string displayFormat = string.Empty;
            
            Int16 inputbox_type = 0;
            int LineNo = 1;
            int pos = 0;
            int ftype = 0;

            int i1 = ds_DCLine.Tables[0].Rows.Count;
            int i2 = ds_Source.Tables[0].Rows.Count;

            // source / kaynak data tükenmiş işlemi bırak
            //
            if (i2 == 0) return false;

            //--------------------------------------------------------------------------------
            // işlem yapılacak data copy bilgileri
            // 
            #region // DcLine döngüsü
            for (int i = 0; i < i1; i++)
            {
                #region read DCLine
                Read_Value = "";
                try
                {
                    dc_source_type = t.Set(ds_DCLine.Tables[0].Rows[i]["DC_SOURCE_TYPE"].ToString(), "", (Int16)0);
                    target_tableipcode = t.Set(ds_DCLine.Tables[0].Rows[i]["TARGET_TABLEIPCODE"].ToString(), Target_TableIPCode, "");
                    target_fieldname = t.Set(ds_DCLine.Tables[0].Rows[i]["TARGET_FIELDNAME"].ToString(), "", "");
                    source_tableipcode = t.Set(ds_DCLine.Tables[0].Rows[i]["SOURCE_TABLEIPCODE"].ToString(), Source_TableIPCode, "");
                    source_fieldname = t.Set(ds_DCLine.Tables[0].Rows[i]["SOURCE_FIELDNAME"].ToString(), "", "");
                    inputbox_message = t.Set(ds_DCLine.Tables[0].Rows[i]["INPUTBOX_MESSAGE"].ToString(), "", "");
                    inputbox_type = t.Set(ds_DCLine.Tables[0].Rows[i]["INPUTBOX_TYPE"].ToString(), "", (Int16)0);
                    inputbox_default1 = t.Set(ds_DCLine.Tables[0].Rows[i]["INPUTBOX_DEFAULT1"].ToString(), "", "");
                    inputbox_default2 = t.Set(ds_DCLine.Tables[0].Rows[i]["INPUTBOX_DEFAULT2"].ToString(), "", "");
                    chechk_field = t.Set(ds_DCLine.Tables[0].Rows[i]["CHECK_FIELD"].ToString(), "", "False");
                }
                catch (Exception e)
                {
                    onay = false;
                    MessageBox.Show("DİKKAT : " + e.Message);
                    //throw;
                    break;
                }
                
                #endregion read DCLine

                /// * 'DC_SOURCE_TYPE',  
                /// * 0,  'none'
                /// * 1,  'Source TableIPCode READ'
                /// * 2,  'Default1 value READ' 
                /// * 3,  'InputBox READ'
                /// * 4,  'Line No'

                #region dc_source-type - Read_Value set

                // 1,  'Source TableIPCode READ'
                #region 1
                if ((dc_source_type == 1) && (t.IsNotNull(source_fieldname)))
                {
                    // Kaynak Oku
                    if ((ds_Source.DataSetName != source_tableipcode) &&
                        (source_tableipcode != "SOURCE")
                        )
                    {
                        if (source_tableipcode != "TARGET")
                            ds_Source_frk = t.Find_DataSet(tForm, "", source_tableipcode, "");
                        if (source_tableipcode == "TARGET")
                            ds_Source_frk = t.Find_DataSet(v.con_DragDropForm, "", v.con_DragDropTargetTableIPCode, "");

                        if (ds_Source_frk != null)
                        {
                            if (source_tableipcode != "TARGET")
                                pos = t.Find_DataNavigator_Position(tForm, source_tableipcode);

                            if (source_tableipcode == "TARGET")
                                pos = t.Find_DataNavigator_Position(v.con_DragDropForm, v.con_DragDropTargetTableIPCode);

                            try
                            {
                                if ((pos > -1) && (ds_Source_frk.Tables[0].Rows.Count > 0))
                                    Read_Value = ds_Source_frk.Tables[0].Rows[pos][source_fieldname].ToString();
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show("(source_fieldName = '" + source_fieldname + "') okumasında sorun oluştu." + v.ENTER +
                                    "(read_Value = '" + Read_Value + "')" + v.ENTER2 + e.Message.ToString());
                                //throw;
                            }

                        }
                    }
                    else
                    {
                        if (Source_Position == -1)
                        {
                            if (work_type == 5)
                            {
                                if (source_tableipcode == "SOURCE")
                                    pos = t.Find_DataNavigator_Position(v.con_DragDropForm, v.con_DragDropSourceTableIPCode);
                            }
                            else
                                pos = t.Find_DataNavigator_Position(tForm, source_tableipcode);

                            try
                            {
                                if ((pos > -1) && (ds_Source.Tables[0].Rows.Count > 0))
                                    Read_Value = ds_Source.Tables[0].Rows[pos][source_fieldname].ToString();
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show("(source_fieldName = '" + source_fieldname + "') okumasında sorun oluştu." + v.ENTER +
                                    "(read_Value = '" + Read_Value + "')" + v.ENTER2 + e.Message.ToString());
                                //throw;
                            }

                        }
                        else
                        {
                            if (ds_Source.Tables[0].Rows.Count > 0)
                                Read_Value = ds_Source.Tables[0].Rows[Source_Position][source_fieldname].ToString();
                        }
                    }

                }
                #endregion 1

                // 2,  'Default1 value READ'
                #region 2
                if (dc_source_type == 2)
                {
                    Read_Value = inputbox_default1;
                }
                #endregion 2

                // 3,  'InputBox READ'
                #region 3
                if (dc_source_type == 3)
                {
                    displayFormat = string.Empty;
                    ftype = t.Find_Field_Type_Id(ds_Target, target_fieldname, ref displayFormat);

                    Read_Value = "";
                    if (t.IsNotNull(inputbox_default1))
                        Read_Value = inputbox_default1;

                    iBox.Clear();
                    iBox.title = "Veri Girişi";
                    iBox.promptText = inputbox_message;
                    iBox.value = Read_Value;
                    iBox.displayFormat = displayFormat;
                    iBox.fieldType = ftype;

                    // inputBox kimin için soru soruyor bunu anlamak için
                    // inputbox_default2 ye display fieldname saklanıyor, ve o fielddeki isim okunarak 
                    // inputboxda display edilecek
                    // örnek : 
                    // EKMEK için           << display fieldName << inputbox_default2
                    // Lütfen Miktar girin  << inputbox_message
                    #region
                    if (t.IsNotNull(inputbox_default2))
                    {
                        if (Source_Position == -1)
                        {
                            pos = t.Find_DataNavigator_Position(tForm, source_tableipcode);
                            if ((pos > -1) && (ds_Source.Tables[0].Rows.Count > 0))
                                Read_Value = ds_Source.Tables[0].Rows[pos][inputbox_default2].ToString();
                        }
                        else
                        {
                            if (ds_Source.Tables[0].Rows.Count > 0)
                                Read_Value = ds_Source.Tables[0].Rows[Source_Position][inputbox_default2].ToString();
                        }

                        iBox.promptText = Read_Value + " için " + v.ENTER + inputbox_message;
                    }
                    #endregion

                    DialogResult dialogResult = t.UserInpuBox(iBox);

                    Read_Value = iBox.value;

                    //DialogResult dialogResult = t.InputBox("Data Copy", inputbox_message, ref Read_Value, ftype);

                    switch (dialogResult)
                    {
                        case DialogResult.OK:
                            {
                                break;
                            }
                        case DialogResult.Cancel:
                            {
                                onay = false;
                                return onay;
                            }
                    }

                    if ((chechk_field.ToUpper() == "TRUE") && t.IsNotNull(Read_Value) == false)
                    {
                        onay = false;
                        break;
                    }

                }
                #endregion 3

                // 4,  'Line No'
                #region 4
                if (dc_source_type == 4)
                {
                    Read_Value = LineNo.ToString();
                    LineNo++;
                }
                #endregion 4

                #endregion // dc_source-type - Read_Value set

                if (dc_source_type > 0)
                {
                    /// * work_type
                    /// * 0, 1   Önce atamaları yap, sonra Kayıt yapma
                    /// *    2   Önce atamaları yap, sonra Kaydet
                    /// *    3   Satır Düzenle, Row.Update()
                    /// *    4   Satır Sil, Row.Delete()
                    /// *    5   DragDrop ile yeni fiş, Form.New(),Row.New()
                    /// 
                    if (work_type < 3)
                    {
                        pos = ds_Target.Tables[0].Rows.Count - 1;
                    }

                    if ((work_type == 3) ||
                        (work_type == 5))
                        pos = dN_Target.Position;

                    try
                    {
                        //if (t.IsNotNull(Read_Value))
                        //{
                            Read_Value = t.tCheckedValue(ds_Target, target_fieldname, Read_Value);

                            ds_Target.Tables[0].Rows[pos][target_fieldname] = Read_Value;
                        //}
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("(target_fieldName = '" + target_fieldname + "') atamasında sorun oluştu." + v.ENTER +
                            "(read_Value = '" + Read_Value + "')" + v.ENTER2 + e.Message.ToString());
                        //throw;
                    }

                }

            }
            #endregion // DCLine döngüsü


            #region // Final işlemleri

            // atamayı yap

            /// * work_type
            /// * 0, 1   Önce atamaları yap, sonra Kayıt yapma
            /// *    2   Önce atamaları yap, sonra Kaydet
            /// *    3   Satır Düzenle, Row.Update()
            /// *    4   Satır Sil, Row.Delete()
            /// *    5   DragDrop ile yeni fiş, Form.New(),Row.New()

            if ((dc_source_type > 0) && (onay))
            {
                //pos = t.Find_DataNavigator_Position(tForm, target_tableipcode);

                if (work_type < 3)
                {
                    pos = ds_Target.Tables[0].Rows.Count - 1;

                    v.con_LkpOnayChange = true; // tDefaultValue_And_Validation çalışmasın diye
                    v.con_Cancel = true;
                    dN_Target.Position = pos;
                    dN_Target.Tag = pos;
                    v.con_LkpOnayChange = false;
                }

                if (work_type == 3)
                    pos = dN_Target.Position;

                try
                {
                    Read_Value = t.tCheckedValue(ds_Target, target_fieldname, Read_Value);
                    ds_Target.Tables[0].Rows[pos][target_fieldname] = Read_Value;
                }
                catch (Exception e)
                {
                    MessageBox.Show("(target_fieldName = '" + target_fieldname + "') atamasında sorun oluştu." + v.ENTER +
                            "(read_Value = '" + Read_Value + "')" + v.ENTER2 + e.Message.ToString());
                    //throw;
                }

            }

            if (((work_type == 2) || (work_type == 3))
                && (onay))
            {
                //kaydet

                if (work_type == 2)
                    dN_Target.Position = ds_Target.Tables[0].Rows.Count - 1;

                dN_Target.Tag = dN_Target.Position;

                string State = "";
                string id_value = ds_Target.Tables[0].Rows[dN_Target.Position][0].ToString();
                if (id_value == "")
                    State = "dsInsert";
                else State = "dsEdit";

                if ((work_type == 2) ||
                    ((work_type == 3) && (State == "dsEdit")))
                {
                    // yeni satır işaretini kaldır
                    ds_Target.Tables[0].CaseSensitive = false;
                    tSave sv = new tSave();
                    sv.tDataSave(tForm, ds_Target, dN_Target, dN_Target.Position);
                }

                if ((work_type == 3) && (State == "dsInsert"))
                {
                    ds_Target.Tables[0].AcceptChanges();
                }
            }

            #endregion // Final işlemleri

            return onay;
        }


        #endregion *tDC Sub Function

    }
}
