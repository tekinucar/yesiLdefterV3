using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Windows.Forms;
using Tkn_Events;
using Tkn_Registry;
using Tkn_Save;
using Tkn_SQLs;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_User : DevExpress.XtraEditors.XtraForm
    {
        tToolBox t = new tToolBox();

        tRegistry reg = new tRegistry();

        // UL = UserLogin
        DataSet ds_UL = null;
        DataNavigator dN_UL = null;
        // NU = NewUser
        DataSet ds_NU = null;
        DataNavigator dN_NU = null;
        // UK = UserKey
        DataSet ds_UK = null;
        DataNavigator dN_UK = null;
        // FL = FirmList
        DataSet ds_FL = null;
        DataNavigator dN_FL = null;


        // sorgular için
        DataNavigator dN_Query = new DataNavigator();
        DataSet ds_Query = new DataSet();
        DataSet ds_Query2 = new DataSet();

        Control cmb_EMail = null;
        Control txt_Pass = null;
        Control btn_BHatirla = null;
        Control btn_SifremiUnuttum = null;
        Control tree_FirmList = null;

        Control uk_user_mail = null;
        Control uk_old_user_pass = null;
        Control uk_new_user_pass = null;
        Control uk_rpt_user_pass = null;


        int u_user_Last_FirmId = 0;
        string u_user_email = string.Empty;
        string u_user_key = string.Empty;
        string u_db_user_key = string.Empty;

        string tSql = string.Empty;
        string TableIPCode = string.Empty;
        string NewUser_TableIPCode = "UST/PMS/User.NewUser_F01";//"SYSUSER.SYSUSER_F01";
        string FirmList_TableIPCode = "UST/PMS/Firm.UserFirmList_L01";//"SYSFIRM.SYSFIRM_L02";

        string regPath = v.registryPath;//"Software\\Üstad\\YesiLdefter";


        public ms_User()
        {
            /// .
            /// comp - user - firm ilişkisi ?
            /// 
            /// prog açılışında 
            /// önce comp bilgileri
            /// sonra user bilgileri
            /// user için tanımlı olan user_firm_guid varsa o firma/shop 
            /// yoksa 
            /// comp kartında tanımlı olan comp_firm_guid e göre firma/shop baz alınacak
            /// 
            /// yani kullanıcının kendisi için tanımlı firmaları var ise o listeye göre çalışır
            /// eğer kullanıcı için firm_guid yok ise Comp için tanımlı olan firma çalışır
            /// 
            /// comp taki veya user daki xxxx_firm_guid hangisi olursa olsun
            /// kayıtlı bu firm_guidin kendisi ve kendisine bağlı alt firm's/shop's listelenir
            /// böyle birden fazla firma ve şube gelirse 
            /// kullanıcıya hangisinde işlem yapacağı (firma listesinden) sorulur
            /// tek firma veya shop olursa kullanıcaya sorulmadan direk program o firm_id üzerinden
            /// çalışmaya başlar.
            /// 
            /// Peki comp'unda firm_guidi yoksa o zamanda kullanıcının karşına bizim TEST firmaları listelenir
            /// 

            InitializeComponent();

            tEventsForm evf = new tEventsForm();

            this.Load += new System.EventHandler(evf.myForm_Load);
            this.Shown += new System.EventHandler(evf.myForm_Shown);
            this.Shown += new System.EventHandler(this.ms_User_Shown);

            this.KeyPreview = true;
        }

        private void ms_User_Shown(object sender, EventArgs e)
        {
            //
            // Sisteme Giriş // User Login ---------------------------------------
            //
            TableIPCode = "UST/PMS/User.Login_F01";// "SYSUSER.SYSUSER_D01";

            t.Find_DataSet(this, ref ds_UL, ref dN_UL, TableIPCode);

            Control cntrl = null;
            string[] controls = new string[] { };
            cntrl = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Dock = DockStyle.Right;
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Width = 75;
                //((DevExpress.XtraEditors.SimpleButton)cntrl).Text = "İleri";
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_SistemeGiris_Ileri);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("SIHIRBAZDEVAM16");
            }

            btn_BHatirla = t.Find_Control(this, "checkButton_ek1", TableIPCode, controls);

            btn_SifremiUnuttum = t.Find_Control(this, "simpleButton_ek2", TableIPCode, controls);
            if (btn_SifremiUnuttum != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)btn_SifremiUnuttum).Click += new System.EventHandler(btn_SifremiUnuttumClick);
            }

            cmb_EMail = t.Find_Control(this, "Column_USER_EMAIL", TableIPCode, controls);
            txt_Pass = t.Find_Control(this, "Column_USER_KEY", TableIPCode, controls);

            if (cmb_EMail != null)
            {
                ((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).EnterMoveNextControl = true;
                ((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).EditValueChanged
                    += new System.EventHandler(cmb_EMail_EditValueChanged);
            }

            if (txt_Pass != null)
            {
                //((DevExpress.XtraEditors.TextEdit)txt_Pass).EnterMoveNextControl = true;
                ((DevExpress.XtraEditors.TextEdit)txt_Pass).KeyDown += new KeyEventHandler(txt_Pass_KeyDown);
                ((DevExpress.XtraEditors.TextEdit)txt_Pass).Properties.PasswordChar = '*';
            }

            //
            // Yeni Kullanıcı // New User ---------------------------------------------------
            // 
            TableIPCode = NewUser_TableIPCode; // "SYSUSER.SYSUSER_F01";

            t.Find_DataSet(this, ref ds_NU, ref dN_NU, TableIPCode);

            cntrl = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Dock = DockStyle.Right;
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_YeniKullanici_Kaydet);
            }

            //
            // Şifre Değişikliği ----------------------------------------------------------
            // 
            TableIPCode = "UST/PMS/User.NewPassword_F01";// "SYSUSER.SYSUSER_F02";

            t.Find_DataSet(this, ref ds_UK, ref dN_UK, TableIPCode);

            cntrl = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Dock = DockStyle.Right;
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_Yeni_SifreClick);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("KAYDET16");
            }


            uk_user_mail = t.Find_Control(this, "Column_USER_EMAIL", TableIPCode, controls);
            if (uk_user_mail != null)
            {
                ((DevExpress.XtraEditors.TextEdit)uk_user_mail).EnterMoveNextControl = true;
            }

            uk_old_user_pass = t.Find_Control(this, "Column_USER_FIRSTNAME", TableIPCode, controls);
            if (uk_old_user_pass != null)
            {
                ((DevExpress.XtraEditors.TextEdit)uk_old_user_pass).EnterMoveNextControl = true;
                //((DevExpress.XtraEditors.TextEdit)uk_old_user_pass).Properties.PasswordChar = '*';
            }

            uk_new_user_pass = t.Find_Control(this, "Column_USER_LASTNAME", TableIPCode, controls);
            if (uk_new_user_pass != null)
            {
                ((DevExpress.XtraEditors.TextEdit)uk_new_user_pass).EnterMoveNextControl = true;
                //((DevExpress.XtraEditors.TextEdit)uk_new_user_pass).Properties.PasswordChar = '*';
            }

            uk_rpt_user_pass = t.Find_Control(this, "Column_USER_KEY", TableIPCode, controls);
            if (uk_rpt_user_pass != null)
            {
                ((DevExpress.XtraEditors.TextEdit)uk_rpt_user_pass).EnterMoveNextControl = true;
                //((DevExpress.XtraEditors.TextEdit)uk_rpt_user_pass).Properties.PasswordChar = '*';
            }

            //
            // FirmList -------------------------------------------------------------------
            //
            TableIPCode = FirmList_TableIPCode;

            cntrl = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Dock = DockStyle.Right;
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_FirmListSec_Click);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("SIHIRBAZDEVAM16");
            }

            // şimdilik gerek kalmadı (XtraTreeList.TreeList) 
            //tree_FirmList = t.Find_Control_View(this, TableIPCode);

            //
            // ----------------------------------------------------------------------------
            //
            GetUserRegistry();

            v.SP_UserLOGIN = false;

            if (cmb_EMail != null)
            {
                ((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).Focus();
            }


            
        }

        void btn_SistemeGiris_Ileri(object sender, EventArgs e)
        {
            CheckInput();
        }

        void cmb_EMail_EditValueChanged(object sender, EventArgs e)
        {
            ((DevExpress.XtraEditors.TextEdit)txt_Pass).EditValue = "";
        }

        void txt_Pass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                CheckInput();
            }
        }

        void CheckInput()
        {
            if (t.IsNotNull(ds_UL) && (dN_UL != null))
            {
                if (dN_UL.Position > -1)
                {
                    // buradaki bilgi databaseden gelmiyor kullanıcı elle giriyor
                    //user_email = ds_UL.Tables[0].Rows[dN_UL.Position]["USER_EMAIL"].ToString().Trim();
                    //user_key = ds_UL.Tables[0].Rows[dN_UL.Position]["USER_KEY"].ToString().Trim(); ;
                    u_user_email = ((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).EditValue.ToString();
                    u_user_key = ((DevExpress.XtraEditors.TextEdit)txt_Pass).EditValue.ToString();

                    // şimdi [ e-mail ile şifre ] databaseden kontrol ediliyor
                    tSql = @" Select * from SYS_USERS where USER_EMAIL = '" + u_user_email + @"' and USER_KEY = '" + u_user_key + "' ";

                    t.SQL_Read_Execute(v.dBaseNo.Manager, ds_Query, ref tSql, "SYS_USERS", "UserLogin");

                    if (t.IsNotNull(ds_Query) == false)
                    {
                        //böyle bir email varmı diye kontrol et
                        GetUser(u_user_email, "FIND");
                    }
                    else
                    {
                        if (ds_Query.Tables.Count > 0)
                            dN_Query.DataSource = ds_Query.Tables[0];

                        if (ds_Query.Tables[0].Rows.Count == 0)
                        {
                            //böyle bir email varmı diye kontrol et
                            GetUser(u_user_email, "FIND");
                        }

                        if (ds_Query.Tables[0].Rows.Count == 1)
                        {
                            //e-mail ve şifre girdikte sonra gelen sonucu değerlendir
                            CheckUser(ds_Query, dN_Query);
                        }

                        if (ds_Query.Tables[0].Rows.Count > 1)
                        {
                            /// birden fazla kayıt geliyorsa aynı emailden birden fazla mevcut 
                            /// sorun var demektir. çünki bir email, bir kayıt esasına göre çalışılacak
                            /// 
                            MessageBox.Show(u_user_email + v.ENTER2 +
                                "Veritabanında birden fazla aynı e-mail mevcut.", "Çok Ciddi Problem",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            v.SP_UserLOGIN = false;
                        }

                    }
                }
            }
        }

        void read_eMail(DataSet ds, string user_EMail)
        {
            tSql = @" Select * from SYS_USERS where USER_EMAIL = '" + user_EMail + @"' ";

            t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "SYS_USERS", "FindUser");
        }

        void GetUser(string user_Email, string work)
        {
            read_eMail(ds_Query, user_Email);

            if (ds_Query.Tables.Count == 1)
            {
                if (ds_Query.Tables[0].Rows.Count == 0)
                {
                    string soru = user_Email + "  böyle bir hesap bulunamadı. \r\n\r\n Yeni bir kullanıcı oluşturmak ister misiniz  ?";
                    DialogResult cevap = t.mySoru(soru);
                    if (DialogResult.Yes == cevap)
                    {
                        t.SelectPage(this, "BACKVIEW", "NEWUSER", -1);
                    }
                }

                if ((work == "FIND") & (ds_Query.Tables[0].Rows.Count == 1))
                {
                    MessageBox.Show(" Şifrenizde bir sorun olabilir.\r\n\r\n Yeniden deneyebilir veya yeni bir şifre alabilirsiniz.");
                }

                if ((work == "SEND_EMAIL") & (ds_Query.Tables[0].Rows.Count == 1))
                {
                    int Id = t.myInt32(ds_Query.Tables[0].Rows[0]["ID"].ToString());
                    Int16 IsActive = t.myInt16(ds_Query.Tables[0].Rows[0]["ISACTIVE"].ToString());
                    string userFullName = ds_Query.Tables[0].Rows[0]["USER_FULLNAME"].ToString();
                    u_db_user_key = ds_Query.Tables[0].Rows[0]["USER_KEY"].ToString();

                    // 0.pasif, 1.aktif 
                    if (IsActive < 2)
                    {
                        Send_EMail(user_Email, userFullName, u_db_user_key);
                    }
                    else
                    {

                    }
                }
            }
        }

        void Send_EMail(string myUserEMail, string myUserFullName, string myUserKey)
        {
            //
            eMail email = new eMail();

            email.toMailAddress = myUserEMail;
            email.subject = "Kullanıcı kodu ";
            email.message =
                "<p> Üstad Bilişim veritabanında kayıtlı olan, </p>" +
                "<p> Kullanıcı Adı Soyadı : " + myUserFullName + "<br>" +
                " Kullanıcı Şifreniz : " + myUserKey + "</p>";

            t.eMailSend3(email);
        }

        void CheckUser(DataSet dsQuery, DataNavigator dNQuery)
        {
            if (t.IsNotNull(dsQuery))
            {
                if (dNQuery.Position > -2) // sürekli -1 geliyor neden 0 dan başlamıyor ???
                {
                    //Int16 IsActive = t.myInt16(dsQuery.Tables[0].Rows[dNQuery.Position]["ISACTIVE"].ToString());
                    ////db_user_email = dsQuery.Tables[0].Rows[dNQuery.Position]["USER_EMAIL"].ToString();
                    //db_user_key = dsQuery.Tables[0].Rows[dNQuery.Position]["USER_KEY"].ToString();

                    int UserId = t.myInt32(dsQuery.Tables[0].Rows[0]["ID"].ToString());
                    Int16 IsActive = t.myInt16(dsQuery.Tables[0].Rows[0]["ISACTIVE"].ToString());
                    string userFullName = dsQuery.Tables[0].Rows[0]["USER_FULLNAME"].ToString();
                    string userFirmGuid = dsQuery.Tables[0].Rows[0]["USER_FIRM_GUID"].ToString();
                    u_db_user_key = dsQuery.Tables[0].Rows[0]["USER_KEY"].ToString();

                    if ((IsActive == 0) && (u_user_key == u_db_user_key))
                    {
                        MessageBox.Show(
                        "Kullanıcı hesabınız henüz AKTİF değil." + v.ENTER +
                        "Hesabınız AKTİF hale getirilecektir. " + v.ENTER2 +
                        "Lütfen en kısa zamanda şifrenizi değiştirin. ",
                        "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        SetUserIsActive(UserId);
                    }

                    if (u_user_key == u_db_user_key)
                    {

                        // RegEdit defterine kayıt
                        //
                        SetUserRegistry(UserId);

                        // Set User Values
                        //
                        v.tUser.SP_USER_ID = UserId;
                        v.tUser.SP_USER_ISACTIVE = Convert.ToInt16(dsQuery.Tables[0].Rows[0]["ISACTIVE"].ToString());
                        v.tUser.SP_USER_FIRM_GUID = dsQuery.Tables[0].Rows[0]["USER_FIRM_GUID"].ToString();
                        v.tUser.SP_USER_GUID = dsQuery.Tables[0].Rows[0]["USER_GUID"].ToString();
                        v.tUser.SP_USER_FULLNAME = dsQuery.Tables[0].Rows[0]["USER_FULLNAME"].ToString();
                        v.tUser.SP_USER_FIRSTNAME = dsQuery.Tables[0].Rows[0]["USER_FIRSTNAME"].ToString();
                        v.tUser.SP_USER_LASTNAME = dsQuery.Tables[0].Rows[0]["USER_LASTNAME"].ToString();
                        v.tUser.SP_USER_EMAIL = dsQuery.Tables[0].Rows[0]["USER_EMAIL"].ToString();
                        v.tUser.SP_USER_KEY = u_db_user_key;

                        // Kullanıcı çalışacağı firmayı sececek
                        //
                        SelectFirm();

                    }
                }
            }
        }

        void SelectFirm()
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

            if ((!string.IsNullOrEmpty(v.tUser.SP_USER_FIRM_GUID)) &&  // user_firm_guid varsa
                (!string.IsNullOrEmpty(v.tComp.SP_COMP_FIRM_GUID)))    // comp_firm_guid varsa
                FirmList(v.tUser.SP_USER_FIRM_GUID);

            if ((string.IsNullOrEmpty(v.tUser.SP_USER_FIRM_GUID)) &&  // user_firm_guid yoksa 
                (!string.IsNullOrEmpty(v.tComp.SP_COMP_FIRM_GUID)))   // comp_firm_guid varsa
                FirmList(v.tComp.SP_COMP_FIRM_GUID);

            if (string.IsNullOrEmpty(v.tComp.SP_COMP_FIRM_GUID))  // comp_firm_guid yoksa kesinlikle TEST
                FirmList("TEST");

        }

        void FirmList(string myGuid)
        {
            //set @_firmGUID = '046C4C69-EE2A-43C3-8E69-FF8A29F40844'

            /// hangi guidle içeriye giriş yapılıyor, sakla
            /// 
            v.SP_FIRM_GUID = myGuid;

            tSQLs sql = new tSQLs();

            //tSql = sql.SQL_SYS_FIRM_List(myGuid, v.tFirmListType.AllFirm);
            tSql = sql.SQL_SYS_FIRM_List(myGuid, v.tFirmListType.OnlySelect);

            t.SQL_Read_Execute(v.dBaseNo.Manager, ds_Query2, ref tSql, "SYS_FIRMS", "FirmList");

            if (t.IsNotNull(ds_Query2))
            {
                // tek firma geldiği için hangisiyle çalışacaksın diye sormak anlamsız olur
                // gerekli bilgileri al formu kapat, main formuna devam et
                if (ds_Query2.Tables[0].Rows.Count == 1)
                {
                    string ufl = "";
                    DataRow row = ds_Query2.Tables[0].Rows[0];
                    t.firmAboutAdd(row, ref ufl);
                    // en sondaki virgülü sil 
                    //
                    t.tLast_Char_Remove(ref ufl);

                    // kullınıcının çalışma yapabileceği firması
                    t.selectFirm(0);


                    // kullanıcının firma listesi
                    v.SP_FIRM_USERLIST = ufl;
                    v.SP_FIRM_MULTI = false;

                    //
                    SetUserRegistryFirm(v.tUser.SP_USER_ID, v.SP_FIRM_ID);

                    // Login onayı
                    //
                    v.SP_UserLOGIN = true;

                    // form close
                    this.Close();
                }

                // birden fazla firma varsa hangisiyle çalışacaksın diye sormak gerekiyor
                // bu seferde sormazsan tuhaf olur
                if (ds_Query2.Tables[0].Rows.Count > 1)
                {
                    t.SelectPage(this, "BACKVIEW", "FIRMLIST", -1);

                    firmListRead();
                }
            }

            if (t.IsNotNull(ds_Query2) == false)
            {
                // ??? bilmem
            }

        }

        void firmListRead()
        {
            // Form üzerinde firma Listesini gösterecek bir InputPanel mevcut
            // bunun datasetinde şu an için bir kayıt yok
            // burada kullanıcının kullanabileceği firma veya firmaListesi okunuyor
            //
            // SYSFIRM.SYSFIRM_L02 
            // 
            t.Find_DataSet(this, ref ds_FL, ref dN_FL, FirmList_TableIPCode);
            // okundu

            // okunan bu firma bilgileri v.tFirmUserList ' esine ekleniyor
            // 
            if (ds_FL != null)
            {
                t.Data_Read_Execute(this, ds_FL, ref tSql, "SYS_FIRMS", null);

                if (tree_FirmList != null)
                    if (tree_FirmList.GetType().ToString() == "DevExpress.XtraTreeList.TreeList")
                        ((DevExpress.XtraTreeList.TreeList)tree_FirmList).ExpandAll();

                string ufl = "";
                int pos = 0;

                foreach (DataRow row in ds_FL.Tables[0].Rows)
                {
                    // regedit e kayıtlı olan Id
                    if (Convert.ToInt32(row["ID"].ToString()) == u_user_Last_FirmId)
                    {
                        dN_FL.Position = pos;
                        //break;
                    }

                    t.firmAboutAdd(row, ref ufl);

                    pos++;
                }

                // en sondaki virgülü sil 
                //
                t.tLast_Char_Remove(ref ufl);

                // kullanıcının firma listesi
                //
                v.SP_FIRM_USERLIST = ufl;

                // kullanıcı birden fazla firma kullanabilecek
                //
                v.SP_FIRM_MULTI = true;

                // listeye set focus için
                //
                Control cntrl = t.Find_Control_View(this, FirmList_TableIPCode);

                // set et
                if (cntrl != null)
                    t.tFormActiveControl(this, cntrl);
            }
        }

        void btn_FirmListSec_Click(object sender, EventArgs e)
        {
            /// Buraya geldiysen 
            /// kullanıcı için birden fazla firma seçeneği var demek ki
            /// kullanıcı bu botuna basar ve seçilen değerler alınır lets go ... main form
            ///

            t.selectFirm(dN_FL.Position);

            SetUserRegistryFirm(v.tUser.SP_USER_ID, v.SP_FIRM_ID);

            // Login onayı
            //
            v.SP_UserLOGIN = true;

            // form close
            this.Close();
        }

        void SetUserIsActive(int Id)
        {
            tSql = @" Update SYS_USERS set ISACTIVE = 1 where ID = " + Id.ToString() + @" ";

            t.SQL_Read_Execute(v.dBaseNo.Manager, ds_Query2, ref tSql, "SYS_USERS", "SetUserIsActive");
        }

        void SetUserRegistryFirm(int UserId, int FirmId)
        {
            /*
            var regUser = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"" + regPath);

            regUser.SetValue("userFirm" + UserId.ToString(), FirmId.ToString(), Microsoft.Win32.RegistryValueKind.String);
            regUser.SetValue("userLastFirm", FirmId.ToString(), Microsoft.Win32.RegistryValueKind.String);
            */
            reg.SetUstadRegistry("userFirm" + UserId.ToString(), FirmId.ToString());
            reg.SetUstadRegistry("userLastFirm", FirmId.ToString());
        }

        void SetUserRegistry(int UserId)
        {
            //var regUser = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"" + regPath);

            //
            // user işlemleri
            //
            //regUser.SetValue("userEMail" + UserId.ToString(), u_user_email, Microsoft.Win32.RegistryValueKind.String);
            reg.SetUstadRegistry("userEMail" + UserId.ToString(), u_user_email);

            // 
            // last işlemler
            //
            //regUser.SetValue("userLastLogin", u_user_email, Microsoft.Win32.RegistryValueKind.String);
            reg.SetUstadRegistry("userLastLogin", u_user_email);

            if (((DevExpress.XtraEditors.CheckButton)btn_BHatirla).Checked)
            {
                //regUser.SetValue("userRemember", "true", Microsoft.Win32.RegistryValueKind.String);
                //regUser.SetValue("userLastKey", u_user_key, Microsoft.Win32.RegistryValueKind.String);
                reg.SetUstadRegistry("userRemember", "true");
                reg.SetUstadRegistry("userLastKey", u_user_key);
            }
            else
            {
                //regUser.SetValue("userRemember", "false", Microsoft.Win32.RegistryValueKind.String);
                //regUser.SetValue("userLastKey", "", Microsoft.Win32.RegistryValueKind.String);
                reg.SetUstadRegistry("userRemember", "false");
                reg.SetUstadRegistry("userLastKey", "");
            }

        }

        void GetUserRegistry()
        {
            var regUser = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"" + regPath);

            if (regUser != null)
            {
                //cmbKullaniciAd.Text = klnadsifre.GetValue("klnad") != null ? klnadsifre.GetValue("klnad").ToString() : "";
                //txtSifre.Text = klnadsifre.GetValue("sifre") != null ? klnadsifre.GetValue("sifre").ToString() : "";

                if (cmb_EMail != null)
                {
                    // e-mail listesi combo için okunuyor
                    ((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).Properties.Items.AddRange(reg.GetUstadEMailList());

                    // en son giriş yapan e-mail combonun text ine atanıyor
                    if (cmb_EMail != null)
                        ((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).EditValue = regUser.GetValue("userLastLogin");
                    if (uk_user_mail != null)
                        ((DevExpress.XtraEditors.TextEdit)uk_user_mail).EditValue = regUser.GetValue("userLastLogin");
                    // beni hatırla butonu true/false ayarlanıyor
                    if (regUser.GetValue("userRemember") != null)
                        ((DevExpress.XtraEditors.CheckButton)btn_BHatirla).Checked = (regUser.GetValue("userRemember").ToString() == "true");
                    // beni hatırla onaylıysa enson girilen key okunuyor ve şifreye atanıyor
                    if (((DevExpress.XtraEditors.CheckButton)btn_BHatirla).Checked)
                        ((DevExpress.XtraEditors.TextEdit)txt_Pass).EditValue = regUser.GetValue("userLastKey");

                    if (regUser.GetValue("userLastFirm") != null)
                    {
                        try
                        {
                            u_user_Last_FirmId = Convert.ToInt32(regUser.GetValue("userLastFirm"));
                        }
                        catch (Exception)
                        {
                            u_user_Last_FirmId = 0;
                        }
                    }



                    /* buradaki  reg.getRegistryValue  neden kullanılmadı ?
                     * bu tRegistry.getRegistryValue() her çağırma işleminde regUser tekrar tekra çağırılmakta
                     * oysa burada bir defa regUser çağırılmakta. Açılan bu kapı defalarca okunabilmekte
                     * 
                     * 
                    if (cmb_EMail != null)
                        ((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).EditValue = reg.getRegistryValue("userLastLogin");

                    if (uk_user_mail != null)
                       ((DevExpress.XtraEditors.TextEdit)uk_user_mail).EditValue = reg.getRegistryValue("userLastLogin");

                    // beni hatırla butonu true/false ayarlanıyor
                    if (regUser.GetValue("userRemember") != null)
                       ((DevExpress.XtraEditors.CheckButton)btn_BHatirla).Checked = (reg.getRegistryValue("userRemember").ToString() == "true");

                    // beni hatırla onaylıysa enson girilen key okunuyor ve şifreye atanıyor
                    if (((DevExpress.XtraEditors.CheckButton)btn_BHatirla).Checked)
                        ((DevExpress.XtraEditors.TextEdit)txt_Pass).EditValue = reg.getRegistryValue("userLastKey");
                    
                    //regUser.SetValue("userFirm" + UserId.ToString(), FirmId.ToString(), Microsoft.Win32.RegistryValueKind.String);
                    if (regUser.GetValue("userLastFirm") != null)
                    {
                        try
                        {
                            u_user_Last_FirmId = Convert.ToInt32(reg.getRegistryValue("userLastFirm"));
                        }
                        catch (Exception)
                        {
                            u_user_Last_FirmId = 0;
                        }
                    }
                    */
                }
            }
        }

        void btn_YeniKullanici_Kaydet(object sender, EventArgs e)
        {
            if (t.IsNotNull(ds_NU) && (dN_NU != null))
            {
                if (dN_NU.Position > -1)
                {
                    string user_Email = ds_NU.Tables[0].Rows[0]["USER_EMAIL"].ToString();
                    read_eMail(ds_Query, user_Email);

                    if (ds_Query.Tables.Count == 1)
                    {
                        if (ds_Query.Tables[0].Rows.Count == 1)
                        {
                            MessageBox.Show(user_Email + "  hesabı mevcut. .....");
                            return;
                        }
                        else if (ds_Query.Tables[0].Rows.Count == 0)
                        {
                            tSave sv = new tSave();
                            if (sv.tDataSave(this, NewUser_TableIPCode))
                            {
                                //t.ButtonEnabledAll(tForm, TableIPCode, true);
                                MessageBox.Show(user_Email + " hesabınız kaydedilmiştir.\r\n\r\n Şifreniz bu hesaba GÖNDERİLECEK (Unutma bu kısım yazılmadı daha)");
                                t.SelectPage(this, "BACKVIEW", "USERLOGIN", -1);
                            }
                        }
                    }

                }
            }
        }

        void btn_SifremiUnuttumClick(object sender, EventArgs e)
        {
            u_user_email = ((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).EditValue.ToString();

            GetUser(u_user_email, "SEND_EMAIL");
        }

        void btn_Yeni_SifreClick(object sender, EventArgs e)
        {
            if (t.IsNotNull(ds_UK) && (dN_UK != null))
            {
                if (dN_UK.Position > -1)
                {
                    string user_email = ((DevExpress.XtraEditors.TextEdit)uk_user_mail).EditValue.ToString();
                    string user_old_pass = ((DevExpress.XtraEditors.TextEdit)uk_old_user_pass).EditValue.ToString();
                    string user_new_pass = ((DevExpress.XtraEditors.TextEdit)uk_new_user_pass).EditValue.ToString();
                    string user_rpt_pass = ((DevExpress.XtraEditors.TextEdit)uk_rpt_user_pass).EditValue.ToString();

                    // şimdi [ e-mail ile şifre ] databaseden kontrol ediliyor
                    tSql = @" Select * from SYS_USERS where USER_EMAIL = '" + user_email + @"' and USER_KEY = '" + user_old_pass + "' ";

                    t.SQL_Read_Execute(v.dBaseNo.Manager, ds_Query, ref tSql, "SYS_USERS", "UserLogin");

                    if (t.IsNotNull(ds_Query))
                    {
                        int userId = t.myInt32(ds_Query.Tables[0].Rows[0]["ID"].ToString());
                        Int16 IsActive = t.myInt16(ds_Query.Tables[0].Rows[0]["ISACTIVE"].ToString());
                        string userFullName = ds_Query.Tables[0].Rows[0]["USER_FULLNAME"].ToString();
                        string db_user_key = ds_Query.Tables[0].Rows[0]["USER_KEY"].ToString();

                        if (IsActive != 1)
                        {
                            MessageBox.Show("DİKKAT : Bu hesap AKTİF değildir.");
                            return;
                        }

                        if (IsActive == 1)
                        {
                            if (db_user_key == user_old_pass)
                            {
                                if ((user_new_pass == user_rpt_pass) && // yeni şifre = tekrarlanan şifre
                                    (user_new_pass != "") &&            // yeni şifre boş değilse
                                    (user_new_pass != user_old_pass)    // yeni şifre ile eski şifre  aynı değilse
                                    )
                                {
                                    tSql = @" Update MS_USERS set
                                    USER_KEY = '" + user_new_pass + @"'
                                    where ID = " + userId.ToString();

                                    bool onay = t.Data_Read_Execute(this, ds_Query2, ref tSql, "NEW_USER_KEY", null);

                                    if (onay) MessageBox.Show("Şifreniz başarıyla güncellenmiştir...");
                                }

                                if (user_new_pass == "")
                                    MessageBox.Show("Lütfen bir şifre giriniz. Boş geçemezsiniz!");

                                if (user_new_pass != user_rpt_pass)
                                    MessageBox.Show("DİKKAT : Yeni şifre ile tekrar yazdığınız şifre aynı değil...");

                            }
                        }
                    }

                    if (t.IsNotNull(ds_Query) == false)
                    {
                        //böyle bir email varmı diye kontrol et
                        GetUser(user_email, "FIND");
                    }
                }
            }
        }


    }
}
