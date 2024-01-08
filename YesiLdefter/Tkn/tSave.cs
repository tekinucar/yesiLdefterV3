using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Tkn_DefaultValue;
using Tkn_Events;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_Save
{
    public class tSave : tBase
    {
        #region tDataSave

        tToolBox t = new tToolBox();

        public bool tDataSave(Form tForm, string TableIPCode)
        {

            // LKP_ONAY için ise işlem olmasın
            if (v.con_OnayChange) return false;

            if (v.con_LkpOnayChange)
            {
                v.con_LkpOnayChange = false;
                return false;
            }

            tToolBox t = new tToolBox();

            v.con_OnaySave = false;
            bool onay = false;
            string function_name = "tData_Save";

            DataSet dsData = t.Find_DataSet(tForm, "", TableIPCode, function_name);

            if (dsData != null)
            {
                //if (dsData.HasChanges() == false) return;

                // NewRocord işareti kayıtla bereber boşaltılıyor
                //if (dsData.Tables[0].Namespace == "NewRecord")
                //    dsData.Tables[0].Namespace = "";
                if (dsData.Tables[0].CaseSensitive == true)
                    dsData.Tables[0].CaseSensitive = false;

                DataNavigator tDataNavigator = t.Find_DataNavigator(tForm, TableIPCode);//, function_name);
                int pos = tDataNavigator.Position;

                if (pos > -1)
                {
                    
                    // DataLayout nesnesi olunca bulundğu texttedeki değişiklikleri algılamıyordu
                    // bu nedenle cursor ün buluduğu nesneden exit yapması sağlandı.

                    // burası işe yaramıyor
                    // commonGridClick() buraya bak
                    //
                    tDataNavigator.TabStop = true;
                    tDataNavigator.Focus();
                    tDataNavigator.TabStop = false;
                    // 

                    //dsData.Tables[0].AcceptChanges();
                    string myProp = dsData.Namespace;
                    // Kayıttan ÖNCE yapması gerekenler var sie
                    if (myProp.IndexOf("Prop_Runtime:True") > 0)
                    {
                        tEvents ev = new tEvents();
                        ev.Prop_RunTimeClick(tForm, dsData, TableIPCode, v.tButtonType.btAutoInsert); //v.nv_102_AUTO_INS);
                    }

                    tDefaultValue df = new tDefaultValue();
                    if (df.tDefaultValue_And_Validation
                                        (tForm,
                                         dsData,
                                         dsData.Tables[0].Rows[tDataNavigator.Position],
                                         TableIPCode,
                                         function_name) == true) // Table Save 
                    {
                        /// Save İşlemi
                        /// 
                        tDataSave(tForm, dsData, tDataNavigator, pos);
                        onay = true;

                        /// SubDetail bağlantısı varsa refresh et
                        if ((v.con_PositionChange == false) &&
                            (tDataNavigator.IsAccessible == true))
                        {
                            tEvents ev = new tEvents();
                            //ev.Data_Refresh(dsData, tDataNavigator);
                            //ev.tDataNavigatorList(tForm, "Detail_SubDetail_Refresh");
                            vSubWork vSW = new vSubWork();
                            vSW._01_tForm = tForm;
                            vSW._02_TableIPCode = TableIPCode;
                            vSW._03_WorkTD = v.tWorkTD.NewAndRef;
                            vSW._04_WorkWhom = v.tWorkWhom.Childs;
                            ev.tSubWork_(vSW);
                        }
                    }
                    else onay = false; // default value den onay gelmedi
                }
            }

            v.con_OnaySave = onay;
            return onay;
        }

        public void tDataSave(Form tForm, DataSet dsData,
             DevExpress.XtraEditors.DataNavigator tDataNavigator, int pos)
        {

            if (v.con_LkpOnayChange)
            {
                v.con_LkpOnayChange = false;
                return;
            }

            tToolBox t = new tToolBox();

            if ((dsData != null) && (pos > -1))
            {
                // field bilgisi yoksa işleme girmesin 
                // zaten bu data select datasıdır
                if (t.Find_TableFields(dsData) == false) return;

                vTable vt = new vTable();

                //v.Kullaniciya_Mesaj_Var = "Kayıt işlemi gerçekleşiyor...";
                //t.WaitFormOpen(tForm, v.Kullaniciya_Mesaj_Var);
                //v.SP_OpenApplication = true;

                //if (dsData.HasChanges())
                //if (dsData.Tables[0].Namespace != "NewRecord")
                if (dsData.Tables[0].CaseSensitive == false)
                {
                    t.Preparing_DataSet(tForm, dsData, vt);

                    //t.ButtonEnabledAll(tForm, vt.TableIPCode, true);

                    v.Kullaniciya_Mesaj_Show = true;
                    v.Kullaniciya_Mesaj_Var = MyRecord(dsData, vt, pos, (byte)v.Save.KAYDET);
                }

                // ilk defa defalt değerler doldurulduğunda buraya geldiğinde hemen insert gerçekleşmemesi için
                // bu metod kullanıldı,
                // ilk geldiğindiğinde kayda gitmez, ama newrecord ifadesi silinir böylece bir daha geldiğinde kayıt gerçekleşir

                //if (dsData.Tables[0].Namespace == "NewRecord")
                //    dsData.Tables[0].Namespace = "";
                if (dsData.Tables[0].CaseSensitive == true)
                    dsData.Tables[0].CaseSensitive = false;

                // Kayıttan SONRA yapması gerekenler var sie
                //if (myProp.IndexOf("Prop_Runtime:True") > 0)
                if (vt.RunTime)
                {
                    tEvents ev = new tEvents();
                    ev.Prop_RunTimeClick(tForm, null, vt.TableIPCode, v.tButtonType.btKaydet);// v.nv_22_Kaydet);
                }
                                
                //t.WaitFormClose();
                //v.SP_OpenApplication = false;
            }
        }

        #endregion tDataSave 

        /* Save About ---------------------------------------------------------------------------------------
               Sonuc  = 0 = KAYDET                   = Cümleleri çalıştır / uygula,
                        1 = N_SATIR_KAYDET           = N satır, tamamına tek cumle oluştur ve burada çalıştır
                        2 = SQL_OLUSTUR              = Cümleleri geriye gönder,
                        3 = SQL_OLUSTUR_IDENTITY_YOK = Identity_ID yi eklemeden geriye gönder 
                        4 = N_SATIR_SQL_OLUSTUR      = N satır, tamamına tek cumle oluştur ve geriye gönder
             
               Bu functionda
               Tek tablo, tek kayıt için cümleler (insert veya update)
                   Sonuc = 0 veya 2 veya 3  olabilir

               Tek tablo, n satır kayıt için cümleler için tamamına tek cümle (insert veya update)
                   Sonuc = 1 veya 4 olabilir

               Tek tablo, n satır kayıt ama her satır için ayrı ayrı cümle (insert veya update)
                   Sonuc = 0 veya 2 veya 3  olabilir

               oluşturulablilir....
         ---------------------------------------------------------------------------------------------------*/

        #region Insert Scripts

        public string Insert_Scrip_Single(DataSet ds_Target, string Target_TableName, int target_pos, SqlConnection VTbaglanti)
        {

            ds_Target.Tables[0].Rows[target_pos][0] = -999;

            //master = MyRecord(ds_Target, Target_TableName, target_pos,
            //                  (byte)v.Save.SQL_OLUSTUR_IDENTITY_YOK, VTbaglanti);

            vTable vt = new vTable();
            vt.TableName = Target_TableName;
            vt.msSqlConnection = VTbaglanti;
            vt.DBaseNo = v.dBaseNo.Manager;
            vt.DBaseType = v.dBaseType.MSSQL;
            vt.DBaseName = v.active_DB.managerDBName;
            vt.Cargo = "data";

            string master = MyRecord(ds_Target, vt, target_pos, v.Save.SQL_OLUSTUR_IDENTITY_YOK);

            return master;
        }

        public string Insert_Script_Multi(DataSet dsTarget, vTable vt) 
        {
            string master = string.Empty;
            /*
             * // string dBaseName, string Target_TableName, SqlConnection VTbaglanti)
            vTable vt = new vTable();
            vt.TableName = Target_TableName;
            vt.msSqlConnection = VTbaglanti;
            vt.DBaseNo = v.dBaseNo.Manager;
            vt.DBaseType = v.dBaseType.MSSQL;
            vt.DBaseName = dBaseName; //v.active_DB.managerDBName;
            vt.Cargo = "data";
            */
            int count = dsTarget.Tables[0].Rows.Count;
            string tableName = "";

            if (vt.IdentityInsertOnOff)
            {
                if (vt.SchemasCode != "") 
                    tableName = vt.SchemasCode + ".";
                
                tableName = tableName + vt.TableName;

                master = " SET IDENTITY_INSERT " + tableName + " ON " + v.ENTER;
            }

            for (int i = 0; i < count; i++)
            {
                if (dsTarget.Tables[0].Rows[i][0].ToString() != "")
                {
                    if (vt.IdentityInsertOnOff == false)
                        dsTarget.Tables[0].Rows[i][0] = -999;

                    master = master +
                             MyRecord(dsTarget, vt, i, v.Save.SQL_OLUSTUR_IDENTITY_YOK);
                }
            }

            if (vt.IdentityInsertOnOff)
            {
                master = master + v.ENTER + " SET IDENTITY_INSERT " + tableName + " OFF ";
            }

            return master;
        }

        public string Insert_Script_Master_Detail(
                DataSet ds_Master, string Master_TableName, int master_pos,
                DataSet ds_Detail, string Detail_TableName,
                SqlConnection VTbaglanti)
        {
            string snc = string.Empty;

            /* şimdilik düzenlemedim gerek olunca burayıda düzenle tkn 
             * 
            vTable vt = new vTable();
            vt.TableName = Target_TableName;
            vt.msSqlConnection = VTbaglanti;
            vt.DBaseNo = (byte)2;
            vt.DBaseType = v.dBaseType.MSSQL;
            vt.DBaseName = v.active_DB.managerDBName;

            string master = string.Empty;
            string detail = string.Empty;
            
            // master
            ds_Master.Tables[0].Rows[master_pos][0] = -999;

            master = MyRecord(ds_Master, Master_TableName, master_pos,
                              (byte)v.Save.SQL_OLUSTUR_IDENTITY_YOK, VTbaglanti);

            // detail
            int j = ds_Detail.Tables[0].Rows.Count;

            for (int i = 0; i < j; i++)
            {
                ds_Detail.Tables[0].Rows[i][0] = -999;

                detail = detail +
                    MyRecord(ds_Detail, Detail_TableName, i,
                             (byte)v.Save.SQL_OLUSTUR_IDENTITY_YOK, VTbaglanti);

            }

            snc = master + v.ENTER2 + detail;
            */

            return snc;
        }

        #endregion Insert Scripts

        #region Line_Records

        public string Line_Records(DataSet dsA, DataSet dsB,
                                   string Table_Name_A, string Table_Name_B,
                                   string master_key_fname, string detail_key_fname,
                                   int master_key_id, int detail_key_id,
                                   int A_Position, int B_Position,
                                   byte Sonuc, SqlConnection SqlConn)
        {
            // dsA  Fiş Başlığı na ait bilgiler
            // dsB  Fiş Satırları na ait bilgiler 

            // ilk atamalar ----------------------------------------------------------------------------------------
            #region ilk atamalar
            string Sonuc_Cumle = "";
            string State_A = "";
            string State_B = "";

            string Identity_A_ID = "";
            string Cumle_Baslik = "";
            string Cumle_Satirlar = "";

            //string Key_Id_A_FieldName = "";
            //string Key_Id_B_FieldName = "";

            string Cumle_Detay = "";
            string TriggerSQL = string.Empty;
            int j = 0;

            #endregion
            //------------------------------------------------------------------------------------------------------

            // A tablosunun işlemleri ( Fiş Başlığı Tablosu ) ------------------------------------------------------
            #region A tablosunun işlemleri

            string ix = dsA.Tables[0].Rows[A_Position][0].ToString();
            if (ix == "")
            {
                State_A = "dsInsert";

                if ((Sonuc == 0) | (Sonuc == 2))
                    Identity_A_ID =
                      " Declare @" + master_key_fname + " as Integer " + v.ENTER +
                      " Select  @" + master_key_fname + " = @@IDENTITY " + v.ENTER;
            }
            else State_A = "dsEdit";

            // insert veya update cümlesi oluşmakta

            /// şimdilik beklemede kal
            /// Cumle_Baslik = Kayit_Cumlesi_Olustur(dsA, Table_Name_A, State_A, out Key_Id_A_FieldName, A_Position, ref TriggerSQL);

            #endregion
            //------------------------------------------------------------------------------------------------------

            // B tablosunun işlemleri ( Fiş Satırları Tablosu ) ----------------------------------------------------
            #region B tablosunun işlemleri

            j = dsB.Tables[0].Rows.Count;
            for (int i = 0; i < j; i++)
            {
                // Gelen kaydın Insert ve Edit mi olmasına karar veriliyor
                // Tabloların 1. fieldi her zaman IDENTITY ID  kabul ediliyor
                ix = dsB.Tables[0].Rows[i][0].ToString();

                if (ix == "")
                {
                    State_B = "dsInsert";

                    if (State_A == "dsInsert")
                        dsB.Tables[0].Rows[i][detail_key_fname] = -1970;
                    else dsB.Tables[0].Rows[i][detail_key_fname] = dsA.Tables[0].Rows[A_Position][master_key_fname];
                }
                else State_B = "dsEdit";

                // insert veya update cümlesi oluşmakta

                /// şimdilik beklemede kal
                /// Cumle_Detay = Kayit_Cumlesi_Olustur(dsB, Table_Name_B, State_B, out Key_Id_B_FieldName, i, ref TriggerSQL);

                // Insert satırı oluştuktan sonra -1970 var ise @master_key_fname değişkeniyle yerdeğiştir
                if (State_B == "dsInsert")
                    if (Cumle_Detay.IndexOf("-1970") > -1)
                        Cumle_Detay.Replace("-1970", "@" + master_key_fname);

                Cumle_Satirlar = Cumle_Satirlar + Cumle_Detay + v.ENTER;
            }
            #endregion
            //------------------------------------------------------------------------------------------------------


            // Oluşan Cümlelere Karar Verme işlemi------------------------------------------------------------------
            #region Oluşan Cümlelere Karar Verme işlemi
            if ((Sonuc == 2) | (Sonuc == 3) | (Sonuc == 4))
            {
                Sonuc_Cumle = Cumle_Baslik + v.ENTER +
                              Cumle_Satirlar + v.ENTER + //Yama_Cumle + ENTER +
                              Identity_A_ID + v.ENTER;
            }
            else
            {
                // Eğer function buraya kadar geldiyse ( Sonuc in [0,1] ) sql Cümleyi çalıştır
                // ve cümleyi göndermeye gerek kalmadı onu yerine
                // yeni veya düzenleme kaydı gerçekleşti diye mesaj dönmektedir

                Sonuc_Cumle = v.SP_MSSQL_BEGIN + v.ENTER +
                              Cumle_Baslik + v.ENTER +
                              Cumle_Satirlar + v.ENTER + //Yama_Cumle + ENTER +
                              Identity_A_ID + v.ENTER +
                              v.SP_MSSQL_END;

                MessageBox.Show("MySQl çevrimi sıraında eksik kaldı : Line_Records() ");
                /*
                if (Record_SQL_RUN(SqlConn, dsA, State_A, Table_Name_A, Key_Id_A_FieldName, A_Position, ref Sonuc_Cumle, TriggerSQL))
                {
                    dsA.Tables[0].AcceptChanges();
                    dsB.Tables[0].AcceptChanges();
                }
                */
            }


            #endregion
            //------------------------------------------------------------------------------------------------------

            return Sonuc_Cumle;
        }

        #endregion Line_Records

        #region MyRecord

        public string MyRecord(DataSet ds,
                               vTable vt,
                               int Position,
                               v.Save save) //byte Sonuc)
        {
            // ilk atamalar ----------------------------------------------------------------------------------------
            #region ilk atamalar
            string Sonuc_Cumle = "";
            string State = "";
            //string Cumle_1 = "";
            string sqlCommand = ""; // Cumle_2
            string Identity_ID = "";
            string Cumle_Detay = "";
            string id_value = "";
            string TriggerSQL = string.Empty;

            string Table_Name = vt.TableName;
            string Key_Id_FieldName = vt.KeyId_FName;
            bool identityInsertOnOff = vt.IdentityInsertOnOff;

            int j = 0;

            string begin = "";
            string end = "";
            string line_end = "";

            if (vt.DBaseType == v.dBaseType.MSSQL)
            {
                begin = v.SP_MSSQL_BEGIN;       // = " begin transaction " + ENTER;
                end = v.SP_MSSQL_END;           // = " commit transaction " + ENTER;
                line_end = v.SP_MSSQL_LINE_END; // = ENTER;
            }
            
            // Gelen kaydın Insert ve Edit mi olmasına karar veriliyor
            // Tabloların 1. fieldi her zaman IDENTITY ID  kabul ediliyor
            // Eğer farklı bir durum söz konusu ise başka bir metodla başınızın çaresine bakın   Tkn

            try
            {
                id_value = ds.Tables[Table_Name].Rows[Position][0].ToString();
            }
            catch
            {
                //MessageBox.Show("DİKKAT : " + Table_Name + " için hatalı POSITION id...");
                return ""; // dataset üzerinde veri yoksa burada hata olarak yakalanır
            }

            if ((id_value == "") || 
                (id_value == "-999") || 
                (identityInsertOnOff))
            {
                State = "dsInsert";
                //if ((Sonuc == 0) | (Sonuc == 2))
                //    Identity_ID = " select @@IDENTITY as ID ";
            }
            else State = "dsEdit";

            #endregion
            //------------------------------------------------------------------------------------------------------

            // insert veya update cümlesi oluşturma işlemi başlıyor ------------------------------------------------
            #region Cümleleri Oluştur

            //if ((Sonuc == 0) | (Sonuc == 2) | (Sonuc == 3))
            if ((save == v.Save.KAYDET) |
                (save == v.Save.SQL_OLUSTUR) |
                (save == v.Save.SQL_OLUSTUR_IDENTITY_YOK))
            {
                sqlCommand = Kayit_Cumlesi_Olustur(ds, vt, State, Position, ref TriggerSQL);
                if (sqlCommand == "DONTSAVE") 
                    return string.Empty;
            }

            //if ((Sonuc == 1) | (Sonuc == 4))
            if ((save == v.Save.N_SATIR_KAYDET) |
                (save == v.Save.N_SATIR_SQL_OLUSTUR))
            {
                // Tek tablo, Çok kayıtlı ve
                j = ds.Tables[0].Rows.Count;
                for (int i = 0; i < j; i++)
                {
                    id_value = ds.Tables[Table_Name].Rows[i][0].ToString();
                    if (id_value == "")
                        State = "dsInsert";
                    else State = "dsEdit";

                    // insert veya update cümlesi oluşmakta
                    Cumle_Detay = Kayit_Cumlesi_Olustur(ds, vt, State, i, ref TriggerSQL);
                    if (Cumle_Detay == "DONTSAVE") 
                        return string.Empty;

                    sqlCommand = sqlCommand + Cumle_Detay + v.ENTER;
                }
            }
            #endregion
            //------------------------------------------------------------------------------------------------------ 

            if ((id_value == "") || (id_value == "-999"))
            {
                // eğer tabloya bağlı trigger ve ona bağlı başka tablolar varsa id şaşırıyor
                //if ((Sonuc == 0) | (Sonuc == 2))
                //    Identity_ID = " select @@IDENTITY as ID ";

                //if ((Sonuc == 0) | (Sonuc == 2))
                if ((save == v.Save.KAYDET) |
                    (save == v.Save.SQL_OLUSTUR))
                {
                    if (vt.SchemasCode != "")
                         Identity_ID = " select MAX(" + Key_Id_FieldName + ") as "+ Key_Id_FieldName + ", 'dsInsert' as dsState from " + vt.SchemasCode + "." + Table_Name;
                    else Identity_ID = " select MAX(" + Key_Id_FieldName + ") as "+ Key_Id_FieldName + ", 'dsInsert' as dsState from " + Table_Name;
                }
            }

            // Oluşan Cümlelere Karar Verme işlemi------------------------------------------------------------------
            #region Oluşan Cümlelere Karar Verme işlemi
            //if ((Sonuc == 2) | (Sonuc == 3) | (Sonuc == 4))
            if ((save == v.Save.SQL_OLUSTUR) |
                (save == v.Save.SQL_OLUSTUR_IDENTITY_YOK) |
                (save == v.Save.N_SATIR_SQL_OLUSTUR)
                )
            {
                if (Identity_ID != "")
                {
                    if (sqlCommand.IndexOf("--IdentityID") > 0)
                    {
                        t.Str_Replace(ref sqlCommand, "--IdentityID", Identity_ID);

                        //Sonuc_Cumle = Cumle_1 + line_end +
                        //              Cumle_2 + line_end;
                        Sonuc_Cumle = sqlCommand;
                    }
                    else
                    {
                        //Sonuc_Cumle = Cumle_1 + line_end +
                        //              Cumle_2 + line_end +
                        //              Identity_ID + line_end;
                        Sonuc_Cumle = sqlCommand + line_end + Identity_ID + line_end;
                    }
                }
                else
                {
                    //Sonuc_Cumle = Cumle_1 + line_end +
                    //              Cumle_2 + line_end;
                    Sonuc_Cumle = sqlCommand;

                }
            }
            
            if ((save == v.Save.KAYDET) |
                (save == v.Save.N_SATIR_KAYDET))
            {
                // Eğer function buraya kadar geldiyse ( Sonuc in [0,1] ) sql Cümleyi çalıştır
                // ve cümleyi göndermeye gerek kalmadı onu yerine
                // yeni veya düzenleme kaydı gerçekleşti diye mesaj dönmekte
                if (Identity_ID != "")
                {
                    //if (Cumle_2.IndexOf("--IdentityID") > 0)
                    if (sqlCommand.IndexOf("--IdentityID") > 0)
                    {
                        //t.Str_Replace(ref Cumle_2, "--IdentityID", Identity_ID);
                        //Sonuc_Cumle = begin +
                        //              Cumle_1 + line_end +
                        //              Cumle_2 + line_end +
                        //              end;
                        t.Str_Replace(ref sqlCommand, "--IdentityID", Identity_ID);

                        Sonuc_Cumle = begin +
                                      sqlCommand +
                                      end;

                    }
                    else
                    {
                        //Sonuc_Cumle = begin +
                        //              Cumle_1 + line_end +
                        //              Cumle_2 + line_end +
                        //              Identity_ID + line_end +
                        //              end;

                        Sonuc_Cumle = begin +
                                      sqlCommand + line_end +
                                      Identity_ID + line_end +
                                      end;

                    }
                }
                else
                {
                    //Sonuc_Cumle = begin +
                    //              Cumle_1 + line_end +
                    //              Cumle_2 + line_end +
                    //              end;

                    Sonuc_Cumle = begin +
                                  sqlCommand +
                                  end;
                }

                v.con_Refresh = Record_SQL_RUN(ds, vt, State, Position, ref Sonuc_Cumle, TriggerSQL);
            }
            #endregion
            //------------------------------------------------------------------------------------------------------ 

            return Sonuc_Cumle;
        }

        #endregion MyRecord

        #region Kayit_Cumlesi_Olustur


        public string Kayit_Cumlesi_Olustur(
            DataSet ds,
            vTable vt,
            string State,
            int Position,
            ref string TriggerSQL)
        {

            #region İlk atamalar

            if (vt.Cargo != "data") return "";

            tToolBox t = new tToolBox();

            string SchemasCode = vt.SchemasCode;
            string tableName = vt.TableName;
            string Key_Id_FieldName = vt.KeyId_FName;
            bool identityInsertOnOff = vt.IdentityInsertOnOff;
            //string MyStr = "";
            string sonuc = "";
            string MyInsert = "";
            string MyEdit = "";
            string MyStr2 = "";
            string MyStr3 = "";
            string MyField = "";
            string MyValue = "";
            string MyIfW = string.Empty;
            string fname = "";
            string onceki_fname = "";
            string Lkp_fname = "";
            string fvalue = "";
            string bos = "   ";
            string ValidationInsert = string.Empty;
            string fForeing = string.Empty;
            string fTrigger = string.Empty;
            string fTriggerFields = string.Empty;
            string displayFormat = string.Empty;
            string fVisible = string.Empty;
            string fieldNewValue = "";

            int ftype = 0;
            int fmax_length = 0;
            int c = 0;
            bool fIdentity = false;
            bool IsChanges = false;

            //List<string> fTriggerFieldList = new List<string>();

            TriggerSQL = string.Empty;

            string myProp = ds.Namespace.ToString();
            string SqlF = t.Set(t.MyProperties_Get(myProp, "=SqlFirst:"), "", "");
            string TableIPCode = t.MyProperties_Get(myProp, "TableIPCode:");
            byte TableType = t.Set(t.MyProperties_Get(myProp, "=TableType:"), "", (byte)1);
            int DataReadType = t.myInt32(t.MyProperties_Get(myProp, "DataReadType:"));

            // Dont Save
            if (DataReadType == 7)
            {
                return "DONTSAVE";
            }

            string line_end = "";

            if (vt.DBaseType == v.dBaseType.MSSQL)
            {
                line_end = v.SP_MSSQL_LINE_END; // = ENTER;
            }

            if (t.IsNotNull(SchemasCode) == false) 
                SchemasCode = "[dbo].";

            if (SchemasCode.IndexOf("[") == -1)
                SchemasCode = "[" + SchemasCode + "].";

            if (SchemasCode.IndexOf(".") == -1)
                SchemasCode = SchemasCode + "."; 

            MyEdit = "  update "+ SchemasCode + "[" + tableName + "] set ";

            //tableFields = tableName + "_FIELDS";

            if (State == "dsInsert")
                IsChanges = true;

            try
            {
                //c = ds.Tables[tableFields].Rows.Count;
                c = v.ds_MsTableFields.Tables[tableName].Rows.Count;
            }
            catch
            {
                c = 0;
            }
            #endregion İlk atamalar

            #region Field Liste Döngüsü
            for (int i = 0; i < c; i++)
            {
                if (vt.DBaseType == v.dBaseType.MSSQL)
                {
                    //fname = ds.Tables[tableFields].Rows[i]["name"].ToString();
                    //ftype = Convert.ToInt32(ds.Tables[tableFields].Rows[i]["user_type_id"].ToString());
                    //fIdentity = (Boolean)(ds.Tables[tableFields].Rows[i]["is_identity"]);
                    //fmax_length = Convert.ToInt32(ds.Tables[tableFields].Rows[i]["max_length"].ToString());
                    fname = v.ds_MsTableFields.Tables[tableName].Rows[i]["name"].ToString();
                    ftype = Convert.ToInt32(v.ds_MsTableFields.Tables[tableName].Rows[i]["user_type_id"].ToString());
                    fIdentity = (Boolean)(v.ds_MsTableFields.Tables[tableName].Rows[i]["is_identity"]);
                    fmax_length = Convert.ToInt32(v.ds_MsTableFields.Tables[tableName].Rows[i]["max_length"].ToString());

                    // varchar(max), nvarchar(max) olunca -1 geliyor
                    if (fmax_length == -1) fmax_length = 128000;

                    if (ftype == 36) // GUID / uniqueidentifier
                        fmax_length = 128000;
                    if (ftype == 231) // nVarchar ise
                        fmax_length = fmax_length / 2;

                }

                if (v.active_DB.mainManagerDbUses) // tableFields + "2"
                    t.OtherValues_Get(ds, TableIPCode, fname, ref ValidationInsert, ref fForeing, ref fTrigger, ref displayFormat, ref fVisible);
                else
                    t.OtherValues_Get(v.ds_TableIPCodeFields, TableIPCode, fname, ref ValidationInsert, ref fForeing, ref fTrigger, ref displayFormat, ref fVisible);

                // field ismi LKP_ veya  rowguid  ise
                //Lkp_fname = "";
                //if (fname.Length > 4)
                //    Lkp_fname = fname.Substring(0, 4);
                Lkp_fname = fname;
                if ((fname.IndexOf("LKP_") > -1) ||
                    (fname.IndexOf("rowguid") > -1))
                    Lkp_fname = "LKP_";
                
                // Anahtar ID fieldname dönüyor
                if (fIdentity == true) 
                    Key_Id_FieldName = fname;

                // dsData yapısı TableType == Table değilise
                if ((TableType != 1) &&  // Table
                    (TableType != 3))    // StoredProcedure
                {
                    // TableFields listesindeki fieldname  
                    // select edilen cümle içinde var mı / yok mu
                    // yok ise işlem yapılmasın

                    //if (SqlF.IndexOf(fname) == -1)
                    //{
                    //    Lkp_fname = "LKP_";
                    //}
                }

                // bazen kriter sorgulamalardan dolayı çift olabiliyor ( >=, =< ) 
                // FIELD_IP listesinden çift field olabiliyor

                if ((onceki_fname == fname) && (onceki_fname != ""))
                {
                    Lkp_fname = "LKP_";
                }

                // onceki_fname nin ataması, bir sonraki field aynı mı diye karşılaştırılacak 
                onceki_fname = fname;

                // dataset üzerinden veriyi al
                fvalue = "";
                // ilk  Lkp_fname == "LKP_"  atamasını TableType != 1 ile alıyor 
                if (Lkp_fname != "LKP_")
                    fvalue = ds.Tables[0].Rows[Position][fname].ToString();

                // Foreing field ve value değeri yok ise cümle içine girmesin
                if ((fForeing == "True") && (fvalue == ""))
                    Lkp_fname = "LKP_";
                if ((fForeing == "True") && (fvalue == "0"))
                    fvalue = "null";


                //------------------------ INSERT ------------------------------------------------
                #region INSERT
                if ((State == "dsInsert") & 
                    ( (fIdentity == false) |                // Identity olmayan fied  veya
                      ((fIdentity) & (identityInsertOnOff)) // Identity ve OnOff true ise yani RefId field da isteniyorsa 
                    ) &
                    (Lkp_fname != "LKP_") & 
                    (Lkp_fname != "rowg")
                    )
                {

                    //* rakam  56, 48, 127, 52, 60, 62, 59, 106, 108
                    if ((ftype == 56) | (ftype == 48) | (ftype == 127) | (ftype == 52) |
                        (ftype == 60) | (ftype == 62) | (ftype == 59) | (ftype == 106) | (ftype == 108))
                    {

                        // Eğer veri (Rakam) yok ise Sıfır bas
                        if (fvalue == "") fvalue = "0";

                        // rakam türü , ile ayrılmış ise , yerine . işareti değiştiriliyor 
                        if (fvalue.IndexOf(",") > -1)
                            fvalue = fvalue.Replace(",", ".");

                        // alınan değeri stringe ekle
                        MyField = MyField + bos + fname + ", ";
                        MyValue = MyValue + fvalue + ", ";
                    }

                    //* text = (char)175, (varchar)167, (ntext)99, (text)35, (nchar)239, (nvarchar)231, (uniqueidentifier)36 //39 
                    if ((ftype == 175) | (ftype == 167) | (ftype == 99) | (ftype == 35) | (ftype == 239) | (ftype == 231) | (ftype == 36))
                    {
                        MyField = MyField + bos + fname + ", ";

                        // Eğer veri yok ise null bas
                        if ((fvalue == "") || (fvalue == "null"))
                        {
                            MyValue = MyValue + "null, ";
                        }
                        else
                        {
                            if (fvalue.Length > fmax_length)
                                fvalue = fvalue.Substring(0, fmax_length-1);

                            // alınan değeri stringe ekle
                            MyValue = MyValue + "'" + t.Str_Check(fvalue) + "', ";
                        }
                    }

                    //* bit türü 104
                    if (ftype == 104)
                    {
                        MyField = MyField + bos + fname + ", ";

                        if ((fvalue == "False") || (fvalue == ""))
                        {
                            MyValue = MyValue + "0, ";
                        }
                        else if (fvalue == "True")
                        {
                            MyValue = MyValue + "1, ";
                        }
                        else
                        {
                            MyValue = MyValue + "null, ";
                        }
                    }

                    //* date 40
                    if ((ftype == 40) || (ftype == 61))
                    {
                        MyField = MyField + bos + fname + ", ";

                        if ((fvalue != "") && (fvalue.IndexOf("01.01.0001") == -1) && fvalue != "null")
                        {
                            MyValue = MyValue + t.Tarih_Formati(Convert.ToDateTime(fvalue)) + ", ";
                        }
                        else
                        {
                            MyValue = MyValue + "null, ";
                        }
                    }

                    //* time türü 41
                    if (ftype == 41)
                    {
                        MyField = MyField + bos + fname + ", ";

                        if (fvalue != "")
                        {
                            MyValue = MyValue + "'" + t.Str_Check(fvalue) + "', ";
                        }
                        else
                        {
                            MyValue = MyValue + "null, ";
                        }
                    }

                    // smalldatetime 58
                    if (ftype == 58)
                    {
                        MyField = MyField + bos + fname + ", ";

                        if ((fvalue != "") && (fvalue.IndexOf("01.01.0001") == -1) && fvalue != "null")
                        {
                            MyValue = MyValue + t.TarihSaat_Formati(Convert.ToDateTime(fvalue)) + ", ";
                        }
                        else
                        {
                            MyValue = MyValue + "null, ";
                        }
                    }

                    //* image 34, varbinary (max) 165 
                    if ((ftype == 34) || (ftype == 165))
                    {
                        // bir tabloda 1 den fazla resim olabiliyor (şimdilik 2 resim kontrolü var)
                        //
                        // 1. resim var ise
                        if ((v.con_Images_FieldName == fname) && (v.con_Images != null))
                        {
                            MyField = MyField + bos + fname + ", ";
                            MyValue = MyValue + " @" + fname + ", ";
                            // üzerindeki eskim varsa tekrar onu gösteriyor
                            // bu nedenle bu atama yapılıyor
                            ds.Tables[0].Rows[Position][fname] = v.con_Images;
                        }
                        // 2. resim var ise
                        if ((v.con_Images_FieldName2 == fname) && (v.con_Images2 != null))
                        {
                            MyField = MyField + bos + fname + ", ";
                            MyValue = MyValue + " @" + fname + ", ";
                            // üzerindeki eskim varsa tekrar onu gösteriyor
                            // bu nedenle bu atama yapılıyor
                            ds.Tables[0].Rows[Position][fname] = v.con_Images2;
                        }

                        // 1. resim null ise
                        if ((v.con_Images_FieldName == fname) && (v.con_Images == null))
                        {
                            MyField = MyField + bos + fname + ", ";
                            MyValue = MyValue + " null , ";
                            // üzerindeki eskim varsa tekrar onu gösteriyor
                            // bu nedenle bu atama yapılıyor
                            ds.Tables[0].Rows[Position][fname] = null;
                        }
                        // 2. resim null ise
                        if ((v.con_Images_FieldName2 == fname) && (v.con_Images2 == null))
                        {
                            MyField = MyField + bos + fname + ", ";
                            MyValue = MyValue + " null , ";
                            // üzerindeki eskim varsa tekrar onu gösteriyor
                            // bu nedenle bu atama yapılıyor
                            ds.Tables[0].Rows[Position][fname] = null;
                        }
                    }

                    MyField = MyField + v.ENTER;
                    MyValue = MyValue + "  ";
                }
                #endregion INSERT
                //-------------------------------------------------------------------------------- 

                //------------------------ EDIT --------------------------------------------------
                #region EDIT
                //if ( (State == "dsEdit") &  
                //     (Lkp_fname != "LKP_") & (Lkp_fname != "rowg"))

                // ValidationInsert == "True" için sürekli edit cümleside hazırlansın 

                if ((Lkp_fname != "LKP_") & (Lkp_fname != "rowg"))
                {
                    // Tablonun ID fieldi
                    if (fIdentity == true)
                    {
                        MyStr2 = " where [" + fname + "] = " + fvalue + " " + line_end;
                        MyStr3 = " select " + fname + ", 'dsEdit' as dsState from [" + tableName + "] where 0 = 0 ";
                        // Tarihce için gerekiyor
                        // KeyID_Value := Value; 
                    }
                }

                if (//(State == "dsEdit") & 
                    (Lkp_fname != "LKP_") & (Lkp_fname != "rowg") & 
                    (fVisible == "True"))
                {

                    // column için yeni update değeri
                    fieldNewValue = "";

                    //* rakam türleri  56, 48, 127, 52, 60, 62, 59, 106, 108
                    if ((fIdentity == false) &
                       ((ftype == 56) | (ftype == 48) | (ftype == 127) | (ftype == 52) |
                        (ftype == 60) | (ftype == 62) | (ftype == 59) | (ftype == 106) | (ftype == 108)))
                    {
                        // Eğer veri (Rakam) yok ise Sıfır bas
                        if (fvalue == "") fvalue = "0";

                        // rakam türü , ile ayrılmış ise , yerine . işareti değiştiriliyor 
                        if (fvalue.IndexOf(",") > -1)
                            fvalue = fvalue.Replace(",", ".");

                        // alınan değeri stringe ekle
                        //MyEdit = MyEdit + " [" + fname + "] = " + fvalue + ", ";
                        fieldNewValue = " [" + fname + "] = " + fvalue + ", ";
                    }

                    //* text = (char)175, (varchar)167, (ntext)99, (text)35, (nchar)239, (nvarchar)231, (uniqueidentifier)36 //39
                    if ((ftype == 175) | (ftype == 167) | (ftype == 99) | (ftype == 35) | (ftype == 239) | (ftype == 231) | (ftype == 36))
                    {
                        if (fvalue.Length > fmax_length)
                            fvalue = fvalue.Substring(0, fmax_length - 1);

                        // alınan değeri stringe ekle
                        if ((fvalue != "") && (fvalue != "null"))
                            fieldNewValue = " [" + fname + "] = " + "'" + t.Str_Check(fvalue) + "', ";
                        else fieldNewValue = " [" + fname + "] = " + "null, ";// Eğer veri yok ise null bas
                    }

                    //* bit türü 104
                    if (ftype == 104)
                    {
                        if ((fvalue == "") || (fvalue == "False"))
                            fieldNewValue = " [" + fname + "] = " + "0, ";
                        else fieldNewValue = " [" + fname + "] = " + "1, ";
                    }

                    //* datetime türü 40
                    if ((ftype == 40) || (ftype == 61))
                    {
                        if ((fvalue != "") && (fvalue.IndexOf("01.01.0001") == -1) && fvalue != "null")
                            fieldNewValue = " [" + fname + "] = " + t.Tarih_Formati(Convert.ToDateTime(fvalue)) + ", ";
                        else fieldNewValue = " [" + fname + "] = " + "null, ";
                    }

                    //* time türü 41
                    if (ftype == 41)
                    {
                        if (fvalue != "")
                            fieldNewValue = " [" + fname + "] = '" + t.Str_Check(fvalue) + "', ";
                        else fieldNewValue = " [" + fname + "] = " + "null, ";
                    }

                    //* datetime türü  58
                    if (ftype == 58)
                    {
                        if ((fvalue != "") && (fvalue.IndexOf("01.01.0001") == -1) && fvalue != "null")
                            fieldNewValue = " [" + fname + "] = " + t.TarihSaat_Formati(Convert.ToDateTime(fvalue)) + ", ";
                        else fieldNewValue = " [" + fname + "] = " + "null, ";
                    }

                    //* img, image 34, varbinary (max) 165 
                    if ((ftype == 34) || (ftype == 165))
                    {
                        // 1. Resim
                        if ((v.con_Images_FieldName == fname) && (v.con_Images != null))
                        {
                            fieldNewValue = " [" + fname + "] = " + " @" + fname + " , ";
                            // üzerindeki eskim varsa tekrar onu gösteriyor
                            // bu nedenle bu atama yapılıyor
                            ds.Tables[0].Rows[Position][fname] = v.con_Images;
                        }
                        if ((v.con_Images_FieldName == fname) && (v.con_Images == null))
                        {
                            fieldNewValue = " [" + fname + "] = null , ";
                            // üzerindeki eskim varsa tekrar onu gösteriyor
                            // bu nedenle bu atama yapılıyor
                            ds.Tables[0].Rows[Position][fname] = null;
                        }
                        // 2. Resim
                        if ((v.con_Images_FieldName2 == fname) && (v.con_Images2 != null))
                        {
                            fieldNewValue = " [" + fname + "] = " + " @" + fname + " , ";
                            // üzerindeki eskim varsa tekrar onu gösteriyor
                            // bu nedenle bu atama yapılıyor
                            ds.Tables[0].Rows[Position][fname] = v.con_Images2;
                        }
                        if ((v.con_Images_FieldName2 == fname) && (v.con_Images2 == null))
                        {
                            fieldNewValue = " [" + fname + "] = null , ";
                            // üzerindeki eskim varsa tekrar onu gösteriyor
                            // bu nedenle bu atama yapılıyor
                            ds.Tables[0].Rows[Position][fname] = null;
                        }
                    }

                    //MyEdit = MyEdit + v.ENTER;

                    // sadece belli field ismi seçilmişse o fieldler update olacak
                    // diğer fieldler update olmayacak
                    if (v.onlyTheseFields != "")
                    {
                        // onlyTheseFields listesinde bu field yok ise update bilgisini boşalt
                        if (v.onlyTheseFields.IndexOf(fname) == -1)
                            fieldNewValue = "";
                    }

                    if (fieldNewValue != "")
                    {
                        MyEdit = MyEdit + fieldNewValue + v.ENTER;
                        IsChanges = true;
                    }
                }
                #endregion EDIT
                //---------------------------------------------------------------------------------

                //------------------------ Select Control ----------------------------------------
                #region Select Control
                if ((ValidationInsert == "True") &&
                    (Lkp_fname != "LKP_") &&
                    (Lkp_fname != "rowg"))
                {
                    //* rakam türleri  56, 48, 127, 52, 60, 62, 59, 106, 108
                    if ((fIdentity == false) &
                       ((ftype == 56) | (ftype == 48) | (ftype == 127) | (ftype == 52) |
                        (ftype == 60) | (ftype == 62) | (ftype == 59) | (ftype == 106) | (ftype == 108)))
                    {
                        // Eğer veri (Rakam) yok ise Sıfır bas
                        if (fvalue == "") fvalue = "0";

                        // rakam türü , ile ayrılmış ise , yerine . işareti değiştiriliyor 
                        if (fvalue.IndexOf(",") > -1)
                            fvalue = fvalue.Replace(",", ".");

                        // alınan değeri stringe ekle
                        MyIfW = MyIfW + " and [" + fname + "] = " + fvalue + " ";
                    }

                    //* text = (char)175, (varchar)167, (ntext)99, (text)35, (nchar)239, (nvarchar)231, (uniqueidentifier)36 //39
                    if ((ftype == 175) | (ftype == 167) | (ftype == 99) | (ftype == 35) | (ftype == 239) | (ftype == 231) | (ftype == 36))
                    {
                        // alınan değeri stringe ekle
                        if ((fvalue != "") && (fvalue != "null"))
                            MyIfW = MyIfW + " and [" + fname + "] = " + "'" + t.Str_Check(fvalue) + "' ";
                        else MyIfW = MyIfW + " and [" + fname + "] = " + "null ";// Eğer veri yok ise null bas
                    }

                    //* bit türü 104
                    if (ftype == 104)
                    {
                        if ((fvalue == "") || (fvalue == "False"))
                            MyIfW = MyIfW + " and [" + fname + "] = 0 ";
                        else MyIfW = MyIfW + " and [" + fname + "] = 1 ";
                    }

                    //* datetime türü 40
                    if ((ftype == 40) || (ftype == 61))
                    {
                        if (fvalue != "")
                            MyIfW = MyIfW + " and [" + fname + "] = " + t.Tarih_Formati(Convert.ToDateTime(fvalue)) + " ";
                        else MyIfW = MyIfW + " and [" + fname + "] = null ";
                    }

                    //* time türü 41
                    if (ftype == 41)
                    {
                        if (fvalue != "")
                            MyIfW = MyIfW + " and [" + fname + "] = '" + t.Str_Check(fvalue) + "' ";
                        else MyIfW = MyIfW + " and [" + fname + "] = " + "null ";
                    }

                    //* datetime türü  58
                    if (ftype == 58)
                    {
                        if (fvalue != "")
                            MyIfW = MyIfW + " and [" + fname + "] = " + t.TarihSaat_Formati(Convert.ToDateTime(fvalue)) + " ";
                        else MyIfW = MyIfW + " and [" + fname + "] = " + "null ";
                    }

                    //* image 34
                    if (ftype == 34)
                    {
                        //if (fvalue != "")
                        //    //MyStr = MyStr + " [" + fname + "] = " + fvalue + ", ";
                        //    MyStr = MyStr + " [" + fname + "] = " + "CONVERT(varchar(8000), convert(binary(8000)," + Convert.ToString((byte[])ds.Tables[0].Rows[Position][fname]) + ")), ";//:Convert.ToString((byte[])ds.Tables[0].Rows[Position][fname]);

                        //  //"  (select * FROM OPENROWSET(BULK '" + Convert.ToByte( ds.Tables[0].Rows[Position][fname]) + "', SINGLE_BLOB) AS img) ";
                        //else MyStr = MyStr + " [" + fname + "] = " + "null, ";
                    }

                    MyIfW = MyIfW + v.ENTER;


                }
                #endregion Select Control
                //---------------------------------------------------------------------------------

                //------------------------ Triggers Fields --------------------------------------
                #region Triggers Fields Control
                if ((fTrigger == "True") &&
                    (Lkp_fname != "LKP_") &&
                    (Lkp_fname != "rowg"))
                {
                    // trigger ile atama yapılan fieldlerin listesi hazırlanıyor 

                    // DİKKAT : Sadece trigger ile atama yapılan fieldler listeleniyor

                    // select için isimler hazırlanıyor 
                    fTriggerFields = fTriggerFields + bos + fname + ", ";
                    // field list 
                    //fTriggerFieldList.Add(fname);
                }
                #endregion Triggers Fileds Control
                //---------------------------------------------------------------------------------

            }
            #endregion Field Liste Döngüsü

            //--------------------------- SONUC İŞLEMLERİ ---------------------------------------

            #region Sonuc İşlemleri

            if (State == "dsInsert")
            {
                // En Sondaki ', ' siliniyor
                t.tLast_Char_Remove(ref MyField);
                t.tLast_Char_Remove(ref MyValue);

                if (MyIfW != "")
                {
                    t.tLast_Char_Remove(ref MyEdit);
                    /*         
                    if ( Select count(*) ADET from xxxxx
                    where ID = yyy ) = 0 
                    begin
  
                       INSERT INTO ( ... )

                    end else
                    begin

                       UPDATE ....

                    end
                    */

                    // sorunu bulamadım
                    MyInsert =
                        " if ( Select count(*) ADET from "+ SchemasCode + "[" + tableName + "] where 0 = 0 " + v.ENTER
                      + MyIfW + " ) = 0 " + v.ENTER
                      + " begin " + v.ENTER
                      + " insert into "+ SchemasCode + "[" + tableName + "] ( " + v.ENTER
                      + MyField + " ) values " + v.ENTER
                      + " ( " + MyValue + " ) " + line_end
                      + " --IdentityID " + v.ENTER
                      //+ " --Trigger " + v.ENTER
                      + " end else " + v.ENTER
                      + " begin " + v.ENTER
                      + MyEdit + v.ENTER 
                      + " where 0 = 0 " + v.ENTER 
                      + MyIfW
                      // MyStr3 = " select " + fname + " from [" + tableName + "] where 0 = 0 ";
                      + MyStr3 + MyIfW  
                    + " end "; 
                }
                else
                {
                    MyInsert = 
                        " insert into "+ SchemasCode + "[" + tableName + "] ( " + v.ENTER +
                          MyField + " ) values " + v.ENTER +
                        " ( " + MyValue + " ) " + line_end +
                        " --IdentityID " + line_end;
                }
                sonuc = MyInsert;
            }

            if (State == "dsEdit")
            {
                // En Sondaki ', ' siliniyor
                t.tLast_Char_Remove(ref MyEdit);

                MyEdit = MyEdit + v.ENTER2 + "  " + MyStr2 + v.ENTER2;
                sonuc = MyEdit;
            }

            // eğer bir değişiklik yoksa 
            if (IsChanges == false) sonuc = "DONTSAVE";

            if (fTriggerFields.Length > 0)
            {
                string s = string.Empty;

                // En Sondaki ', ' siliniyor
                fTriggerFields = fTriggerFields.Substring(0, fTriggerFields.Length - 2);

                //   Aşağıdaki sorgu oluşturmak için
                //   Select  @@IDENTITY as ID
                //   , GRUP_TAMADI from [GRUP] where ID = @@IDENTITY   

                if (State == "dsInsert")
                {
                    // "  , " + fTriggerFields + " from [" + tableName + "] "

                    TriggerSQL =
                        "  Select " + fTriggerFields + " from [" + tableName + "] "
                      + "  where " + Key_Id_FieldName + " =  @@IDENTITY " + v.ENTER; // where id = xxx  

                    TriggerSQL =
                        "  Select " + fTriggerFields + " from [" + tableName + "] "
                      + "  where " + Key_Id_FieldName + " = "; // where id = rec_id   
                }

                if (State == "dsEdit")
                {
                    //   Aşağıdaki sorgu oluşturmak için
                    //   Select GRUP_TAMADI from [GRUP] where ID = xxxx 

                    TriggerSQL =
                        "  Select " + fTriggerFields + " from [" + tableName + "] "
                      + "  " + MyStr2 + v.ENTER; // where id = xxx
                }

                //if (MyStr.IndexOf("--Trigger") > 0)
                //    t.Str_Replace(ref MyStr, "--Trigger", s);
            }

            #endregion Sonuc İşlemleri

            //-----------------------------------------------------------------------------------

            return sonuc; // MyStr;

        } // Kayit_Cumlesi_Olustur


        #endregion Kayit_Cumlesi_Olustur

        #region Record_SQL_RUN
        public Boolean Record_SQL_RUN(DataSet ds,
            vTable vt,
            string State,
            int Position,
            ref string SQL,
            string TriggerSQL)
        {
            Boolean sonuc = false;
            string Table_Name = vt.TableName;
            string Key_Id_FieldName = vt.KeyId_FName;

            tToolBox t = new tToolBox();

            if (SQL != "")
            {
                v.SQLSave = v.ENTER + SQL + v.SQLSave;

                SqlConnection SqlConn = vt.msSqlConnection;
                SqlCommand SqlKomut = null;
                SqlDataReader myReader;
                DataTable table = new DataTable();

                if (vt.DBaseType == v.dBaseType.MSSQL)
                {
                    t.Db_Open(SqlConn);
                    SqlKomut = new SqlCommand(SQL, SqlConn);
                    
                }
                
                // 1. Resim
                if (t.IsNotNull(v.con_Images_FieldName) && (v.con_Images != null))
                {
                    SqlKomut.Parameters.Add(new SqlParameter("@" + v.con_Images_FieldName, v.con_Images));

                    // işi burada bitti bir sonraki kayıt için boşaltalım....
                    v.con_Images_FieldName = "";
                    v.con_Images = null;
                }
                // 2. Resim
                if (t.IsNotNull(v.con_Images_FieldName2) && (v.con_Images2 != null))
                {
                    SqlKomut.Parameters.Add(new SqlParameter("@" + v.con_Images_FieldName2, v.con_Images2));

                    // işi burada bitti bir sonraki kayıt için boşaltalım....
                    v.con_Images_FieldName2 = "";
                    v.con_Images2 = null;
                }
                // 3. Resim
                if (t.IsNotNull(v.con_Images_FieldName3) && (v.con_Images3 != null))
                {
                    SqlKomut.Parameters.Add(new SqlParameter("@" + v.con_Images_FieldName3, v.con_Images3));

                    // işi burada bitti bir sonraki kayıt için boşaltalım....
                    v.con_Images_FieldName3 = "";
                    v.con_Images3 = null;
                }
                // 4. Resim
                if (t.IsNotNull(v.con_Images_FieldName4) && (v.con_Images4 != null))
                {
                    SqlKomut.Parameters.Add(new SqlParameter("@" + v.con_Images_FieldName4, v.con_Images4));

                    // işi burada bitti bir sonraki kayıt için boşaltalım....
                    v.con_Images_FieldName4 = "";
                    v.con_Images4 = null;
                }

                if (State == "dsInsert")
                {
                    string rec_id = "0";
                    //string old_rec_id = "0";

                    try
                    {
                        myReader = SqlKomut.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        rec_id = table.Rows[0][Key_Id_FieldName].ToString();

                        // şuanda gerçekleşen state Insert gibi görünsede aslında kontrollü sql ile o kaydın update olduğu tespit ediliyor
                        // kayıt için gönderilen datanın sürekli olarak dsEdit olduğu tespit edilirse kullanıcıdan işlemi durdurmak için onay istenebilir.
                        //
                        v.con_Save_dsState = "dsInsert";
                        if (table.Columns.Count == 2)
                        {
                            string checkFName = table.Columns[1].ToString();
                            if (checkFName == "dsState")
                                v.con_Save_dsState = table.Rows[0][checkFName].ToString();

                            if (v.con_Save_dsState == "dsEdit")
                                v.con_EditSaveCount += 1;
                        }


                        /// ValidationInsert devreye girince kayıt gerçekleşmeden dönebiliyor
                    }
                    catch (Exception e)
                    {
                        /// uzak bağlantıda ilk insert denemesinde hata veriyor
                        /// o hata oluştursa aynı sql bir daha çalışınca
                        /// işlem bu sefer gerçekleşiyor
                        /// bu geçici çözümdür
                        /// 
                        MessageBox.Show("HATA : Insert : " + v.ENTER + e.Message + v.ENTER2 + SQL);
                        //throw;
                    }

                    if (rec_id != "0")
                    {
                        if (TriggerSQL != "")
                            TriggerSQL = TriggerSQL + rec_id;
                                                
                        // key Id filedin insertten gelen id si datasete yükleniyor
                        ds.Tables[Table_Name].Rows[Position][Key_Id_FieldName] = rec_id;

                        v.con_GotoRecord = "ONdialog";
                        v.con_GotoRecord_FName = Key_Id_FieldName;
                        v.con_GotoRecord_Value = rec_id;
                        v.con_GotoRecord_Position = -1;

                        if (ds.Namespace != null)
                        {
                            string myProp = ds.Namespace.ToString();
                            if (t.IsNotNull(myProp))
                            {
                                string TableLabel = t.MyProperties_Get(myProp, "TableLabel:");
                                t.Alias_Control(ref TableLabel);
                                string oldValue = "and " + TableLabel + Key_Id_FieldName + " = 0";
                                string newValue = "and " + TableLabel + Key_Id_FieldName + " = " + rec_id;

                                string SqlS = "=SqlSecond:" + t.MyProperties_Get(myProp, "SqlSecond:");
                                string Sql_OldS = SqlS;
                                t.Str_Replace(ref SqlS, oldValue, newValue);

                                //string NewValue = "=KeyIDValue:" + rec_id + ";";
                                //t.Str_Replace(ref myProp, oldValue, newValue);
                                t.Str_Replace(ref myProp, Sql_OldS, SqlS);

                                ds.Namespace = myProp;
                            }
                        }

                        // insert cümlesi burada çalıştırıldığı için talep edene sadece işlemin gerçekleştiğine dair mesaj gitmekte
                        SQL = v.DBRec_Insert;
                        sonuc = true;
                    }

                    if (rec_id == "0")
                    {
                        // en son oluşan insert satırın silinmesi gerekiyor
                        int i = ds.Tables[Table_Name].Rows.Count;

                        if (i > 0)
                            ds.Tables[Table_Name].Rows[i - 1].Delete();

                        sonuc = false;
                    }

                }

                if (State == "dsEdit")
                {
                    try
                    {
                        string adet = "";

                        if (vt.DBaseType == v.dBaseType.MSSQL)
                            adet = SqlKomut.ExecuteNonQuery().ToString();

                        // Edit cümlesi burada çalıştırıldığı için talep edene sadece işlemin gerçekleştiğine dair mesaj gitmekte
                        SQL = " " + adet + v.DBRec_Update;
                        sonuc = true;
                    }
                    catch
                    {
                        MessageBox.Show("HATA : Update : " + v.ENTER + SQL);
                    }

                }

                if (sonuc == true)
                {
                    // unutma position sorunu var

                    #region Trigger Sonuclarını Aktarma İşlemi
                    if (TriggerSQL.Length > 0)
                    {
                        DataSet dsTrg = new DataSet();

                        t.SQL_Read_Execute(vt.DBaseNo, dsTrg, ref TriggerSQL, "TRIGGER", "Record_SQL_RUN");

                        if (t.IsNotNull(dsTrg))
                        {
                            int i2 = dsTrg.Tables[0].Columns.Count;
                            string fname = string.Empty;

                            for (int i = 0; i < i2; i++)
                            {
                                fname = dsTrg.Tables[0].Columns[i].ColumnName;
                                ds.Tables[Table_Name].Rows[Position][fname] = dsTrg.Tables[0].Rows[0][fname];
                            }

                            dsTrg.Dispose();
                        }
                    }
                    #endregion Trigger Sonuclarını Aktarma İşlemi

                    ds.Tables[0].AcceptChanges();

                    t.mypropChanged(ref ds, v.dataStateUpdate, v.dataStateNull);

                    v.SQLState = State;
                }
                else
                {
                    v.SQLState = "ERROR";
                }
            } // if (SQL != "")

            //sp.con_Refresh = sonuc;

            return sonuc;
        }
        #endregion

        private int mySQL_FieldType(string fieldType)
        {
            int ftype = 0;

            if (fieldType == "int") ftype = 56;

            if (fieldType == "smallint") ftype = 52;

            if (fieldType == "varchar") ftype = 167;

            if (fieldType == "decimal") ftype = 108; // veya 106

            if (fieldType == "date") ftype = 40;

            if (fieldType == "datetime") ftype = 58;

            if (ftype == 0)
            {
                MessageBox.Show("MySQL field type için eksik tanımlama : (" + fieldType + ")");
            }

            return ftype;
        }


        #region MSSQL Veri Tipleri ve ID leri

        /*
        column_id   name                    system_type_id user_type_id max_length is_nullable is_identity
        ----------- ---------------------- -------------- ----------- ---------- ----------- -----------
        
        1           CHAR                    175            175          10         1           0
        2           VARCHAR                 167            167          10         1           0
        3           TINYINT                 48             48           1          1           0
        4           BIGINT                  127            127          8          1           0
        5           INT                     56             56           4          1           0
        6           SMALLINT                52             52           2          1           0
        7           BIT                     104            104          1          1           0
        8           MONEY                   60             60           8          1           0
        9           FLOAT                   62             62           8          1           0
        10          REAL                    59             59           4          1           0
        11          NTEXT                   99             99           16         1           0
        12          BINARY                  173            173          10         1           0
        13          VARBINARY(MAX)          165            165          10         1           0
        14          IMAGE                   34             34           16         1           0
        15          NUMERIC                 108,106        108,106      5          1           0
        16          DATETIME                58             58           4          1           0
        17          DATE                    40             40           3          1           0      precision  scala 
        10	        TIME                    41             41           5          1           0        16         7  
        18          TEXT                    35             35           16         1           0        
        19          NVARCHAR(MAX)           231            231          -1         1           0
        20          NVARBINARY              xxx            xxx          xx         1           0
         0          VARCHAR(MAX)            xxx            xxx          xx         1           0
         0          uniqueidentifier        36             36           16         1           0

        * text 175,167,99,35, 231
        * rkm  56, 48, 127, 52, 60, 62, 59, 104, 108
        * date 58
        * uniqueidentifier  36

        *  56 int
        *  52 smallint
        * 127 bigint
        * 104 bit
        * 108 numeric
        *  62 float
        *  59 real
        *  60 money
        *  48 tinyint
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
    */

        #endregion

    }

}
