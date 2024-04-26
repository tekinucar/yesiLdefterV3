using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tkn_SQLs;
using Tkn_Registry;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_UserFirms
{
    public class tUserFirms : tBase
    {
        tToolBox t = new tToolBox();
        tRegistry reg = new tRegistry();
        tSQLs Sqls = new tSQLs();

        //DataSet dsUserFirmList = null;
        //DataNavigator dNUserFirmList = null;
        string FirmList_TableIPCode = "";
       
        // User giriş yaptı, firma seçecek
        #region User Select Firms

        public void UserSelectFirm(Form tForm, DataSet dsQuery, DataNavigator dNQuery, string userKey,
            ref DataSet dsUserFirmList, ref DataNavigator dNUserFirmList, string firmList_TableIPCode)
        {
            //dsUserFirmList = dsData;
            //dNUserFirmList = tDataNavigator;

            if (t.IsNotNull(dsQuery))
            {
                FirmList_TableIPCode = firmList_TableIPCode;

                if (dNQuery.Position > -2) // sürekli -1 geliyor neden 0 dan başlamıyor ???
                {
                    //Int16 IsActive = t.myInt16(dsQuery.Tables[0].Rows[dNQuery.Position]["ISACTIVE"].ToString());
                    ////db_user_email = dsQuery.Tables[0].Rows[dNQuery.Position]["USER_EMAIL"].ToString();
                    //db_user_key = dsQuery.Tables[0].Rows[dNQuery.Position]["USER_KEY"].ToString();

                    int UserId = t.myInt32(dsQuery.Tables[0].Rows[0]["UserId"].ToString());
                    bool IsActive = Convert.ToBoolean(dsQuery.Tables[0].Rows[0]["IsActive"].ToString());
                    string userFullName = dsQuery.Tables[0].Rows[0]["UserFullName"].ToString();
                    string userFirmGuid = dsQuery.Tables[0].Rows[0]["FirmGUID"].ToString();
                    Int16 userDbTypeId = t.myInt16(dsQuery.Tables[0].Rows[0]["DbTypeId"].ToString());
                    string u_db_user_key = dsQuery.Tables[0].Rows[0]["UserKey"].ToString();

                    if ((IsActive == false) && (userKey == u_db_user_key))
                    {
                        MessageBox.Show(
                        "Kullanıcı hesabınız henüz AKTİF değil." + v.ENTER +
                        "Hesabınız AKTİF hale getirilecektir. " + v.ENTER2 +
                        "Lütfen en kısa zamanda şifrenizi değiştirin. ",
                        "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // ****** ms_User için de bu kod

                        //SetUserIsActive(UserId);
                    }

                    if (userKey == u_db_user_key)
                    {

                        // RegEdit defterine kayıt
                        //
                        SetUserRegistry(UserId);

                        // Set User Values
                        //
                        v.tUser.UserId = UserId;
                        v.tUser.IsActive = Convert.ToBoolean(dsQuery.Tables[0].Rows[0]["IsActive"].ToString());
                        v.tUser.UserGUID = dsQuery.Tables[0].Rows[0]["UserGUID"].ToString();
                        v.tUser.UserFirmGUID = dsQuery.Tables[0].Rows[0]["FirmGUID"].ToString();
                        v.tUser.FullName = dsQuery.Tables[0].Rows[0]["UserFullName"].ToString();
                        v.tUser.FirstName = dsQuery.Tables[0].Rows[0]["UserFirstName"].ToString();
                        v.tUser.LastName = dsQuery.Tables[0].Rows[0]["UserLastName"].ToString();
                        v.tUser.UserTcNo = dsQuery.Tables[0].Rows[0]["UserTcNo"].ToString();
                        v.tUser.eMail = dsQuery.Tables[0].Rows[0]["UserEMail"].ToString();
                        v.tUser.MobileNo = dsQuery.Tables[0].Rows[0]["UserMobileNo"].ToString();
                        v.tUser.MebbisCode = dsQuery.Tables[0].Rows[0]["MebbisCode"].ToString();
                        v.tUser.MebbisPass = dsQuery.Tables[0].Rows[0]["MebbisPass"].ToString();

                        v.tUser.Key = u_db_user_key;
                        v.tUser.UserDbTypeId = userDbTypeId;

                        if (v.tUser.UserFirmGUID == "")
                        {
                            MessageBox.Show("Kullanıcı için çalışacağı firma tanımlı değil...", "Firma tanımı yok");
                            Application.Exit();
                        }
                        
                        // Kullanıcı çalışacağı firmayı sececek
                        //
                        SelectFirm(tForm, ref dsUserFirmList, ref dNUserFirmList);
                    }
                }
            }
        }


        public void UserTabimFirm(Form tForm, DataSet ds, string userKey)
            //ref DataSet dsUserFirmList, ref DataNavigator dNUserFirmList, string firmList_TableIPCode)
        {
            t.getTabimUserAbout(ds);
            SetUserRegistry(v.tUser.UserId);
        }


        public bool getFirmAboutWithUserFirmGUID(string firmGUID)
        {
            bool onay = false;
            DataSet ds_Query = new DataSet();
            string Sql = Sqls.Sql_UstadFirmsWithFirmGUID(v.tUser.UserFirmGUID);

            t.SQL_Read_Execute(v.dBaseNo.UstadCrm, ds_Query, ref Sql, "UserFirmGUID", "FirmAbout");

            if (t.IsNotNull(ds_Query))
            {
                /// tek firma geldiği için hangisiyle çalışacaksın diye sormak anlamsız olur
                /// gerekli bilgileri al formu kapat, main formuna devam et
                ///
                if (ds_Query.Tables[0].Rows.Count == 1)
                {
                    ///
                    /// kullanıcının bağlanabileceği firmaya ait bilgileri al
                    /// 
                    DataRow row = ds_Query.Tables[0].Rows[0];
                    t.getFirmAbout(row, ref v.tMainFirm);
                    onay = true;
                }

                // birden fazla firma varsa hangisiyle çalışacaksın diye sormak gerekiyor
                // bu seferde sormazsan tuhaf olur
                //
                if (ds_Query.Tables[0].Rows.Count > 1)
                {
                    MessageBox.Show("DİKKAT : Kullanıcının FirmGUID bilgisiyle birden fazla firma bilgisi geliyor...");
                }
            }

            ds_Query.Dispose();

            if (onay == false)
                MessageBox.Show("DİKKAT : Kullanıcının FirmGUID bilgisiyle eşleşen firma bulunamıyor...");

            return onay;
        }

        void SelectFirm(Form tForm, ref DataSet dsUserFirmList, ref DataNavigator dNUserFirmList)
        {
            /// evet geldik zurnanın zırt dediği yere
            /// 
            /// Kontrol konuları
            /// 1. computer_firm_guid ve 
            /// 2. user_firm_guid  
            /// comp_firm_guid value yok ise KESİN TEST FİRMALARI çalışacak 
            /// eğer comp_firm_guid olmaz ise firma istedeği kadar user tanımlaya bilir.
            /// 
            /// User_Firm_Guid var ise kullanıcının firmaları devreye girecek
            ///                yoksa comp_Firm_Guid firmaları devreye girecek
            ///                
            /// peki xxxx_firm_guid de firma tanımlıyken nasıl oluyorda birden fazla firma veya şube listeleniyor
            /// O da şöyle oluyor : bu xxxx_firm_guid de bağlı child firma veya şubeler varsa onlarda geliyor :) 
            /// yani bu xxxx_firm_guid in ID si kimlerin PARENT_ID sinde var ise onlarda geliyor
            /// 

            DataSet ds_Query = new DataSet();
            string Sql = Sqls.Sql_prcUstadUserFirmsList() + v.tUser.UserId.ToString();

            t.SQL_Read_Execute(v.dBaseNo.UstadCrm, ds_Query, ref Sql, "UserFirmList", "FirmList");

            if (t.IsNotNull(ds_Query))
            {
                /// tek firma geldiği için hangisiyle çalışacaksın diye sormak anlamsız olur
                /// gerekli bilgileri al formu kapat, main formuna devam et
                ///
                if (ds_Query.Tables[0].Rows.Count == 1)
                {
                    ///
                    /// kullanıcının bağlanabileceği database ait bilgileri al
                    /// 
                    DataRow row = ds_Query.Tables[0].Rows[0];
                    readUstadFirmAbout(tForm, row);
                }

                // birden fazla firma varsa hangisiyle çalışacaksın diye sormak gerekiyor
                // bu seferde sormazsan tuhaf olur
                //
                if (ds_Query.Tables[0].Rows.Count > 1)
                {
                    // user ın firma listesinin gösterileceği page yi aç
                    t.SelectPage(tForm, "BACKVIEW", "FIRMLIST", -1);
                    // user ın firma listesini hazırla ve göster
                    firmListRead(tForm, ref dsUserFirmList, ref dNUserFirmList);
                }
            }

            ds_Query.Dispose();
        }

        void firmListRead(Form tForm, ref DataSet dsUserFirmList, ref DataNavigator dNUserFirmList)
        {
            // Form üzerinde firma Listesini gösterecek bir InputPanel mevcut
            // bunun datasetinde şu an için bir kayıt yok
            // burada kullanıcının kullanabileceği firma veya firmaListesi okunuyor
            //
            t.Find_DataSet(tForm, ref dsUserFirmList, ref dNUserFirmList, FirmList_TableIPCode);
            // okundu

            // user ın görebileceği firmların listesi okunacak
            // 
            if (dsUserFirmList != null)
            {
                string Sql = Sqls.Sql_prcUstadUserFirmsList() + v.tUser.UserId.ToString(); 
                string myProp = string.Empty;
                t.MyProperties_Set(ref myProp, "DBaseNo", "3");
                t.MyProperties_Set(ref myProp, "TableName", "UstadFirms");
                t.MyProperties_Set(ref myProp, "SqlFirst", Sql);
                t.MyProperties_Set(ref myProp, "SqlSecond", "null");
                t.MyProperties_Set(ref myProp, "TableType", "3");
                t.MyProperties_Set(ref myProp, "Cargo", "data");
                t.MyProperties_Set(ref myProp, "KeyFName", "FirmId");
                dsUserFirmList.Namespace = myProp;

                t.Data_Read_Execute(tForm, dsUserFirmList, ref Sql, "UstadFirms", null);

                // kullanıcının en son çalıştığı firmayı bul
                //
                setLastSelectFirmPosition(v.tUserRegister.UserLastFirmId, ref dsUserFirmList, ref dNUserFirmList);

                Control cntrl = t.Find_Control_View(tForm, FirmList_TableIPCode);

                // firmaların listelendiği controle focus ol
                // bu focustan sonra kullanıcı hangi firmayı açacağını ekranda seçerek açacak
                //
                if (cntrl != null)
                    t.tFormActiveControl(tForm, cntrl);
            }
        }
        void setLastSelectFirmPosition(int userLastFirmId, ref DataSet dsUserFirmList, ref DataNavigator dNUserFirmList)
        {
            if (t.IsNotNull(dsUserFirmList))
            {
                int pos = 0;
                foreach (DataRow row in dsUserFirmList.Tables[0].Rows)
                {
                    // regedit e kayıtlı olan Id
                    if (Convert.ToInt32(row["FirmId"].ToString()) == userLastFirmId)
                    {
                        dNUserFirmList.Position = pos;
                        break;
                    }
                    pos++;
                }
            }
        }

        public void readUstadFirmAbout(Form tForm, DataRow row)
        {
            /// UstadCrm den gelen Firmaya ait bilgileri v.tMainFirm üzerine oku
            /// 
            t.getFirmAbout(row, ref v.tMainFirm);
            ///
            /// kullınıcının çalışma yapabileceği firması
            ///
            t.setSelectFirm(v.tMainFirm);
            ///
            /// User giriş yaptığı firmayı registere yaz
            ///
            SetUserRegistryFirm(v.tUser.UserId, v.tMainFirm.FirmId);
            ///
            /// Login onayı
            ///
            v.SP_UserLOGIN = true;
            ///
            /// form close
            ///
            tForm.Close();
        }


        #endregion User Select Firms

        void SetUserRegistryFirm(int UserId, int FirmId)
        {
            reg.SetUstadRegistry("userFirm" + UserId.ToString(), FirmId.ToString());
            reg.SetUstadRegistry("userLastFirm", FirmId.ToString());
        }

        public void GetUserRegistry(string regPath)
        {
            var regUser = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"" + regPath);

            if (regUser != null)
            {
                try
                {
                    v.tUserRegister.eMailList.AddRange(reg.GetUstadEMailList());
                }
                catch (Exception)
                {
                    //throw;
                }
                try
                {
                    v.tUserRegister.UserLastLoginEMail = regUser.GetValue("userLastLogin")?.ToString();
                }
                catch (Exception)
                {
                    v.tUserRegister.UserLastLoginEMail = "";
                }
                try
                {
                    v.tUserRegister.UserRemember = (regUser.GetValue("userRemember")?.ToString() == "true");
                }
                catch (Exception)
                {
                    v.tUserRegister.UserRemember = false;
                }
                try
                {
                    v.tUserRegister.UserLastKey = regUser.GetValue("userLastKey")?.ToString();
                }
                catch (Exception)
                {
                    v.tUserRegister.UserLastKey = "";
                }                  
                if (regUser.GetValue("userLastFirm") != null)
                {
                    try
                    {
                        v.tUserRegister.UserLastFirmId = Convert.ToInt32(regUser.GetValue("userLastFirm"));
                    }
                    catch (Exception)
                    {
                        v.tUserRegister.UserLastFirmId = 0;
                    }
                }
            }
        }

        public void SetUserRegistry(int UserId)
        {
            //var regUser = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"" + regPath);

            //
            // user işlemleri
            //
            reg.SetUstadRegistry("eMail" + UserId.ToString(), v.tUserRegister.UserLastLoginEMail);

            // 
            // last işlemler
            //
            reg.SetUstadRegistry("userLastLogin", v.tUserRegister.UserLastLoginEMail);

            if (v.tUserRegister.UserRemember)
            {
                reg.SetUstadRegistry("userRemember", "true");
                reg.SetUstadRegistry("userLastKey", v.tUserRegister.UserLastKey);
            }
            else
            {
                reg.SetUstadRegistry("userRemember", "false");
                reg.SetUstadRegistry("userLastKey", "");
            }
        }

        public void checkedRegistry(string regPath)
        {
            try
            {
                var regUser = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"" + regPath);
            }
            catch (Exception)
            {
                reg.SetUstadRegistry("create time", DateTime.Now.ToString());
                //throw;
            }
        }
    }
}
