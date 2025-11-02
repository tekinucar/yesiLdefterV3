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
using Tkn_UserFirms;
using Tkn_UstadAPI;
using System.Threading.Tasks;

namespace YesiLdefter
{
    public partial class ms_User : DevExpress.XtraEditors.XtraForm
    {
        #region Tanımlar

        // DEMO MODE: API ONLY - Legacy methods comm

        tToolBox t = new tToolBox();
        tSQLs Sqls = new tSQLs();
        tRegistry reg = new tRegistry();
        tUserFirms userFirms = new tUserFirms();
        UstadApiClient apiClient = null; // API client for secure authentication

        // UL = UserLogin
        DataSet ds_UL = null;
        DataNavigator dN_UL = null;
        // NU = NewUser
        DataSet ds_NU = null;
        DataNavigator dN_NU = null;
        // UK = Key
        DataSet ds_UK = null;
        DataNavigator dN_UK = null;
        // FL = FirmList
        DataSet dsUserFirmList = null;
        DataNavigator dNUserFirmList = null;

        // sorgular için
        DataNavigator dN_Query = new DataNavigator();
        DataSet ds_Query = new DataSet();
        DataSet ds_Query2 = new DataSet();

        Control cmb_EMail = null;
        Control txt_Pass = null;
        Control btn_BHatirla = null;
        Control btn_SifremiUnuttum = null;
        
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

        string Login_TableIPCode = "UST/CRM/UstadUsers.Login_F01"; 
        string NewPass_TableIPCode = "UST/CRM/UstadUsers.NewPassword_F01";
        string NewUser_TableIPCode = "UST/CRM/UstadUsers.NewUser_F01";
        string FirmList_TableIPCode = "UST/CRM/UstadFirmsUsers.KullanicininFirmaSecimi_L01";
            //"UST/CRM/UstadFirms.UserFirmList_L01";
        
        string regPath = v.registryPath;//"Software\\Üstad\\YesiLdefter";
        string apiBaseUrl = "http://localhost:5000"; // API base URL - can be configured
        #endregion

        public ms_User()
        {
            /// .
            /// comp - user - firm ilişkisi ?
            /// 
            /// prog açılışında 
            /// önce  user bilgileri
            /// sonra comp bilgileri

            // burası değişti 
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
            this.Leave += new System.EventHandler(evf.myForm_Leave);

            this.KeyPreview = true;
        }

        private void ms_User_Shown(object sender, EventArgs e)
        {
            //
            // Sisteme Giriş // User Login ---------------------------------------
            //
            #region
            Application.DoEvents();

            t.Find_DataSet(this, ref ds_UL, ref dN_UL, Login_TableIPCode);

            Control cntrl = null;
            string[] controls = new string[] { };
            cntrl = t.Find_Control(this, "simpleButton_ek1", Login_TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Dock = DockStyle.Right;
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Width = 75;
                //((DevExpress.XtraEditors.SimpleButton)cntrl).Text = "İleri";
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_SistemeGiris_Ileri);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("SIHIRBAZDEVAM16");
            }

            btn_BHatirla = t.Find_Control(this, "checkButton_ek1", Login_TableIPCode, controls);

            btn_SifremiUnuttum = t.Find_Control(this, "simpleButton_ek2", Login_TableIPCode, controls);
            if (btn_SifremiUnuttum != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)btn_SifremiUnuttum).Click += new System.EventHandler(btn_SifremiUnuttumClick);
            }

            cmb_EMail = t.Find_Control(this, "Column_UserEMail", Login_TableIPCode, controls);
            txt_Pass = t.Find_Control(this, "Column_UserKey", Login_TableIPCode, controls);

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
                //((DevExpress.XtraEditors.TextEdit)txt_Pass).Properties.PasswordChar = '*';
            }
            #endregion
            //
            // Yeni Kullanıcı // New User ----------------------------------------
            // 
            #region
            t.Find_DataSet(this, ref ds_NU, ref dN_NU, NewUser_TableIPCode);

            cntrl = t.Find_Control(this, "simpleButton_ek1", NewUser_TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Dock = DockStyle.Right;
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_YeniKullanici_Kaydet);
            }
            #endregion
            //
            // Şifre Değişikliği -------------------------------------------------
            // 
            #region
            t.Find_DataSet(this, ref ds_UK, ref dN_UK, NewPass_TableIPCode);

            cntrl = t.Find_Control(this, "simpleButton_ek1", NewPass_TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Dock = DockStyle.Right;
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_Yeni_SifreClick);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("KAYDET16");
            }

            uk_user_mail = t.Find_Control(this, "Column_UserEMail", NewPass_TableIPCode, controls);
            if (uk_user_mail != null)
            {
                ((DevExpress.XtraEditors.TextEdit)uk_user_mail).EnterMoveNextControl = true;
            }

            uk_old_user_pass = t.Find_Control(this, "Column_UserFirstName", NewPass_TableIPCode, controls);
            if (uk_old_user_pass != null)
            {
                ((DevExpress.XtraEditors.TextEdit)uk_old_user_pass).EnterMoveNextControl = true;
                //((DevExpress.XtraEditors.TextEdit)uk_old_user_pass).Properties.PasswordChar = '*';
            }

            uk_new_user_pass = t.Find_Control(this, "Column_UserLastName", NewPass_TableIPCode, controls);
            if (uk_new_user_pass != null)
            {
                ((DevExpress.XtraEditors.TextEdit)uk_new_user_pass).EnterMoveNextControl = true;
                //((DevExpress.XtraEditors.TextEdit)uk_new_user_pass).Properties.PasswordChar = '*';
            }

            uk_rpt_user_pass = t.Find_Control(this, "Column_UserKey", NewPass_TableIPCode, controls);
            if (uk_rpt_user_pass != null)
            {
                ((DevExpress.XtraEditors.TextEdit)uk_rpt_user_pass).EnterMoveNextControl = true;
                //((DevExpress.XtraEditors.TextEdit)uk_rpt_user_pass).Properties.PasswordChar = '*';
            }
            #endregion

            //
            // FirmList ----------------------------------------------------------
            //
            #region
            cntrl = t.Find_Control(this, "simpleButton_ek1", FirmList_TableIPCode, controls);
            
            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Dock = DockStyle.Right;
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_FirmListSec_Click);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("SIHIRBAZDEVAM16");
            }
            #endregion

            //
            // -------------------------------------------------------------------
            //
            GetUserRegistry();

            // Initialize API client for secure authentication
            try
            {
                apiClient = new UstadApiClient(apiBaseUrl);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API client initialization failed: {ex.Message}");
            }

            v.SP_UserLOGIN = false;

            if (cmb_EMail != null)
            {
                ((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).Focus();
            }
        }

        void btn_SistemeGiris_Ileri(object sender, EventArgs e)
        {
            // DEMO MODE: API ONLY - Legacy methods commented out for security
            if (apiClient == null)
            {
                MessageBox.Show("API bağlantısı kurulamadı. API servisinin çalıştığından emin olun.\n\n" + 
                    "API URL: " + apiBaseUrl, "API Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DebugLog("ERROR: API client is null - cannot proceed with authentication");
                return;
            }

            DebugLog("=== LOGIN PROCESS STARTED ===");
            DebugLog($"Email: {((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail)?.EditValue?.ToString() ?? "NULL"}");
            
            checkedInputApi(); // Secure API-based authentication ONLY
            
            // LEGACY METHOD COMMENTED OUT FOR DEMO/SECURITY
            //checkedInput(); // Legacy SQL-based (contains connection strings - SECURITY RISK)
        }

        void cmb_EMail_EditValueChanged(object sender, EventArgs e)
        {
            ((DevExpress.XtraEditors.TextEdit)txt_Pass).EditValue = "";
        }

        void txt_Pass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                // DEMO MODE: API ONLY
                if (apiClient != null)
                {
                    checkedInputApi(); // Secure API-based authentication
                }
                else
                {
                    MessageBox.Show("API bağlantısı kurulamadı. Lütfen API servisinin çalıştığından emin olun.", 
                        "API Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// LEGACY METHOD - DEPRECATED - Use checkedInputApi() instead
        /// This method contains database connection strings (security risk)
        /// Kept for backward compatibility but should not be used
        /// </summary>
        [Obsolete("Use checkedInputApi() instead. This method contains database connection strings.", false)]
        void checkedInput()
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

                    v.tUserRegister.UserLastLoginEMail = u_user_email;
                    v.tUserRegister.UserLastKey = u_user_key;
                    v.tUserRegister.UserRemember = ((DevExpress.XtraEditors.CheckButton)btn_BHatirla).Checked;
                    
                    t.TableRemove(ds_Query);

                    // şimdi [ e-mail ile şifre ] databaseden kontrol ediliyor
                    tSql = Sqls.preparingUstadUsersSql(u_user_email, u_user_key, 0);
                    t.SQL_Read_Execute(v.dBaseNo.UstadCrm, ds_Query, ref tSql, "UstadUsers", "UserLogin");

                    if (t.IsNotNull(ds_Query) == false)
                    {
                        //böyle bir email varmı diye kontrol et
                        checkedUser(u_user_email, "FIND");
                    }
                    else
                    {
                        if (ds_Query.Tables.Count > 0)
                            dN_Query.DataSource = ds_Query.Tables[0];

                        if (ds_Query.Tables[0].Rows.Count == 0)
                        {
                            //böyle bir email varmı diye kontrol et
                            checkedUser(u_user_email, "FIND");
                        }

                        if (ds_Query.Tables[0].Rows.Count == 1)
                        {
                            //e-mail ve şifre girdikte sonra gelen sonucu değerlendir
                            userFirms.UserSelectFirm(this, ds_Query, dN_Query, u_user_key, 
                                ref dsUserFirmList, ref dNUserFirmList, FirmList_TableIPCode);
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
        ============================================================================ */

        /// <summary>
        /// API-based secure authentication - uses HTTP API instead of direct database access
        /// No database connection strings are exposed to the client application
        /// </summary>
        async void checkedInputApi()
        {
            if (apiClient == null)
            {
                DebugLog("ERROR: API client is null");
                MessageBox.Show("API bağlantısı kurulamadı. Lütfen API servisinin çalıştığından emin olun.", 
                    "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (t.IsNotNull(ds_UL) && (dN_UL != null))
            {
                if (dN_UL.Position > -1)
                {
                    try
                    {
                        // Get user input
                        u_user_email = ((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).EditValue?.ToString() ?? "";
                        u_user_key = ((DevExpress.XtraEditors.TextEdit)txt_Pass).EditValue?.ToString() ?? "";

                        string emailStatus = string.IsNullOrWhiteSpace(u_user_email) ? "EMPTY" : "OK";
                        string passStatus = string.IsNullOrWhiteSpace(u_user_key) ? "EMPTY" : "OK";
                        DebugLog("Input validation - Email: " + emailStatus + ", Password: " + passStatus);

                        if (string.IsNullOrWhiteSpace(u_user_email))
                        {
                            DebugLog("ERROR: Email is empty");
                            MessageBox.Show("Lütfen e-posta adresinizi girin.", "Eksik Bilgi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(u_user_key))
                        {
                            DebugLog("ERROR: Password is empty");
                            MessageBox.Show("Lütfen şifrenizi girin.", "Eksik Bilgi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Store user preferences
                        v.tUserRegister.UserLastLoginEMail = u_user_email;
                        v.tUserRegister.UserLastKey = u_user_key;
                        v.tUserRegister.UserRemember = ((DevExpress.XtraEditors.CheckButton)btn_BHatirla).Checked;

                        string emailPart = u_user_email.Length > 3 ? u_user_email.Substring(0, 3) + "***" : "***";
                        DebugLog("Calling API: LoginAsync(" + emailPart + ")");
                        
                        // Authenticate via API
                        var loginResponse = await apiClient.LoginAsync(u_user_email, u_user_key);

                        if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                        {
                            DebugLog("Login SUCCESS - UserId: " + loginResponse.OperatorId + ", UserGUID: " + loginResponse.UserGUID + ", FullName: " + loginResponse.FullName);
                            
                            // Login successful - update user data
                            v.tUser.UserId = loginResponse.OperatorId;
                            v.tUser.UserGUID = loginResponse.UserGUID;
                            v.tUser.FullName = loginResponse.FullName;
                            v.tUser.eMail = u_user_email;

                            // Store authentication token for future API calls
                            apiClient.SetAuthToken(loginResponse.Token);
                            DebugLog("JWT token stored for API calls");

                            DebugLog("Fetching user firms for UserGUID: " + loginResponse.UserGUID);
                            
                            // Get user firms via API
                            var userFirmsList = await apiClient.GetUserFirmsAsync(loginResponse.UserGUID);

                            int firmCount = userFirmsList != null ? userFirmsList.Count : 0;
                            DebugLog("User firms retrieved: " + firmCount + " firm(s)");

                            if (userFirmsList != null && userFirmsList.Count > 0)
                            {
                                if (userFirmsList.Count == 1)
                                {
                                    DebugLog("Single firm found - Auto-selecting: " + userFirmsList[0].FirmLongName);
                                    // Single firm - auto-select and proceed
                                    var firm = userFirmsList[0];
                                    await SelectFirmFromApiAsync(firm);
                                }
                                else
                                {
                                    DebugLog("Multiple firms found (" + userFirmsList.Count + ") - Showing selection UI");
                                    // Multiple firms - show selection
                                    await ShowFirmSelectionFromApiAsync(userFirmsList, loginResponse);
                                }
                            }
                            else
                            {
                                DebugLog("ERROR: No firms found for user");
                                MessageBox.Show("Kullanıcıya atanmış firma bulunamadı.", "Firma Bulunamadı", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                v.SP_UserLOGIN = false;
                            }
                        }
                        else
                        {
                            DebugLog("ERROR: Login failed - Invalid response or empty token");
                            MessageBox.Show("Giriş başarısız. Lütfen bilgilerinizi kontrol edin.", "Giriş Hatası", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            v.SP_UserLOGIN = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugLog("EXCEPTION in checkedInputApi: " + ex.GetType().Name + " - " + ex.Message);
                        DebugLog("Stack trace: " + (ex.StackTrace ?? "N/A"));
                        
                        string errorMsg = ex.Message;
                        if (errorMsg.Contains("401") || errorMsg.Contains("Unauthorized"))
                        {
                            DebugLog("Unauthorized - Checking if user exists");
                            // Check if user exists
                            checkedUserApi(u_user_email, "FIND");
                        }
                        else
                        {
                            MessageBox.Show("Giriş sırasında hata oluştu:\n" + errorMsg, "Hata", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            v.SP_UserLOGIN = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// LEGACY METHOD - DEPRECATED - Use checkedUserApi() instead
        /// This method contains database connection strings (security risk)
        /// </summary>
        [Obsolete("Use API methods instead. This method contains database connection strings.", false)]
        void read_eMail(DataSet ds, string user_EMail)
        {
            tSql = Sqls.preparingUstadUsersSql(user_EMail, "", 0);
            t.SQL_Read_Execute(v.dBaseNo.UstadCrm, ds, ref tSql, "UstadUsers", "FindUser");
        }

        /// <summary>
        /// LEGACY METHOD - DEPRECATED - Use checkedUserApi() instead
        /// This method contains database connection strings (security risk)
        /// </summary>
        [Obsolete("Use checkedUserApi() instead. This method contains database connection strings.", false)]
        void checkedUser(string user_Email, string work)
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
                    int Id = t.myInt32(ds_Query.Tables[0].Rows[0]["UserId"].ToString());
                    Int16 IsActive = t.myInt16(ds_Query.Tables[0].Rows[0]["IsActive"].ToString());
                    string userFullName = ds_Query.Tables[0].Rows[0]["UserFullName"].ToString();
                    u_db_user_key = ds_Query.Tables[0].Rows[0]["UserKey"].ToString();

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
        ============================================================================ */

        /// <summary>
        /// API-based user check - uses HTTP API instead of direct database access
        /// Checks if user exists and handles different scenarios
        /// </summary>
        async void checkedUserApi(string user_Email, string work)
        {
            DebugLog("=== CHECK USER API ===");
            DebugLog("Email: " + user_Email + ", Work: " + work);
            
            if (apiClient == null)
            {
                DebugLog("ERROR: API client is null");
                MessageBox.Show("API bağlantısı kurulamadı. API servisinin çalıştığından emin olun.", 
                    "API Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // LEGACY FALLBACK COMMENTED OUT FOR DEMO
                //checkedUser(user_Email, work);
                return;
            }

            try
            {
                DebugLog("Calling API: CheckUserExistsAsync(" + user_Email + ")");
                var userExists = await apiClient.CheckUserExistsAsync(user_Email);
                DebugLog("User exists response: Exists=" + userExists.Exists + ", IsActive=" + userExists.IsActive + ", UserId=" + (userExists.UserId?.ToString() ?? "null"));

                if (!userExists.Exists)
                {
                    // User not found
                    if (work == "FIND" || work == "SEND_EMAIL")
                    {
                        string soru = user_Email + "  böyle bir hesap bulunamadı. \r\n\r\n Yeni bir kullanıcı oluşturmak ister misiniz  ?";
                        DialogResult cevap = t.mySoru(soru);
                        if (DialogResult.Yes == cevap)
                        {
                            t.SelectPage(this, "BACKVIEW", "NEWUSER", -1);
                        }
                    }
                    return;
                }

                if (work == "FIND")
                {
                    // User exists but password might be wrong
                    MessageBox.Show(" Şifrenizde bir sorun olabilir.\r\n\r\n Yeniden deneyebilir veya yeni bir şifre alabilirsiniz.", 
                        "Şifre Sorunu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (work == "SEND_EMAIL")
                {
                    // Request password reset via API
                    if (userExists.IsActive)
                    {
                        try
                        {
                            await apiClient.RequestPasswordResetAsync(user_Email);
                            MessageBox.Show("Şifre sıfırlama talebi gönderildi.\nLütfen e-postanızı kontrol edin.", 
                                "Şifre Sıfırlama", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Şifre sıfırlama talebi gönderilemedi:\n" + ex.Message, 
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Bu hesap aktif değil.", "Pasif Hesap", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı kontrolü sırasında hata oluştu:\n{ex.Message}", "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Select firm and complete login when single firm is available
        /// </summary>
        async Task SelectFirmFromApiAsync(UstadApiClient.FirmInfo firm)
        {
            try
            {
                // Get full firm details
                var firmDetails = await apiClient.GetFirmDetailsAsync(firm.FirmGUID);

                if (firmDetails?.Firm != null)
                {
                    // Update global firm information
                    v.tMainFirm.FirmId = firmDetails.Firm.FirmId;
                    v.tMainFirm.FirmGuid = firmDetails.Firm.FirmGUID;
                    v.tMainFirm.FirmLongName = firmDetails.Firm.FirmLongName;

                    // Store user firm selection
                    SetUserRegistryFirm(v.tUser.UserId, firmDetails.Firm.FirmId);

                    // Mark login as successful
                    v.SP_UserLOGIN = true;

                    // Close login form
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Firma bilgileri alınırken hata oluştu:\n" + ex.Message, "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Show firm selection UI when multiple firms are available
        /// </summary>
        async Task ShowFirmSelectionFromApiAsync(System.Collections.Generic.List<UstadApiClient.FirmInfo> firms, UstadApiClient.LoginResponse loginResponse)
        {
            try
            {
                DebugLog("=== SHOW FIRM SELECTION UI ===");
                DebugLog("Firms count: " + firms.Count);
                
                // Load firm list into dataset for UI display
                // This maintains compatibility with existing UI code
                t.TableRemove(dsUserFirmList);
                dsUserFirmList = new System.Data.DataSet();
                var table = dsUserFirmList.Tables.Add("FirmList");
                
                table.Columns.Add("FirmId", typeof(int));
                table.Columns.Add("FirmGUID", typeof(string));
                table.Columns.Add("FirmLongName", typeof(string));

                foreach (var firm in firms)
                {
                    var row = table.NewRow();
                    row["FirmId"] = firm.FirmId;
                    row["FirmGUID"] = firm.FirmGUID ?? "";
                    row["FirmLongName"] = firm.FirmLongName ?? "";
                    table.Rows.Add(row);
                    DebugLog("Added firm: ID=" + firm.FirmId + ", GUID=" + (firm.FirmGUID ?? "null") + ", Name=" + (firm.FirmLongName ?? "null"));
                }

                DebugLog("Dataset table row count: " + table.Rows.Count);

                if (dNUserFirmList == null)
                {
                    dNUserFirmList = new DataNavigator();
                    DebugLog("Created new DataNavigator");
                }
                
                dNUserFirmList.DataSource = table;
                DebugLog("DataNavigator data source set. Position: " + dNUserFirmList.Position + ", Count: " + table.Rows.Count);

                // Ensure dataset is properly bound to UI
                t.Find_DataSet(this, ref dsUserFirmList, ref dNUserFirmList, FirmList_TableIPCode);
                DebugLog("DataSet found/bound to form controls");

                // Refresh the table to ensure UI updates
                if (dsUserFirmList != null && dsUserFirmList.Tables.Count > 0)
                {
                    t.tSqlSecond_Set(ref dsUserFirmList, "null");
                    t.TableRefresh(this, dsUserFirmList);
                    DebugLog("Table refreshed in UI");
                }

                // Show firm selection page
                DebugLog("Switching to FIRMLIST page");
                t.SelectPage(this, "BACKVIEW", "FIRMLIST", -1);

                // Small delay to ensure page transition
                await System.Threading.Tasks.Task.Delay(100);
                Application.DoEvents();

                // Refresh again after page switch
                if (dsUserFirmList != null && dsUserFirmList.Tables.Count > 0)
                {
                    t.TableRefresh(this, dsUserFirmList);
                    DebugLog("Table refreshed after page switch");
                }

                // Set last selected firm position if available
                if (v.tUserRegister.UserLastFirmId > 0)
                {
                    DebugLog("Looking for last selected firm ID: " + v.tUserRegister.UserLastFirmId);
                    int pos = 0;
                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        if (Convert.ToInt32(row["FirmId"]) == v.tUserRegister.UserLastFirmId)
                        {
                            dNUserFirmList.Position = pos;
                            DebugLog("Set DataNavigator position to " + pos + " for last selected firm");
                            break;
                        }
                        pos++;
                    }
                }
                else
                {
                    // Set to first item if no last selection
                    if (table.Rows.Count > 0)
                    {
                        dNUserFirmList.Position = 0;
                        DebugLog("Set DataNavigator position to 0 (first firm)");
                    }
                }

                // Focus on firm list control
                Control cntrl = t.Find_Control_View(this, FirmList_TableIPCode);
                if (cntrl != null)
                {
                    DebugLog("Found firm list control: " + cntrl.GetType().Name);
                    t.tFormActiveControl(this, cntrl);
                }
                else
                {
                    DebugLog("WARNING: Firm list control not found!");
                }

                // Final refresh to ensure UI is visible
                Application.DoEvents();
                DebugLog("=== FIRM SELECTION UI READY ===");
            }
            catch (Exception ex)
            {
                DebugLog("EXCEPTION in ShowFirmSelectionFromApiAsync: " + ex.GetType().Name + " - " + ex.Message);
                DebugLog("Stack trace: " + (ex.StackTrace ?? "N/A"));
                MessageBox.Show("Firma listesi gösterilirken hata oluştu:\n" + ex.Message, "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Send_EMail(string myeMail, string myUserFullName, string myKey)
        {
            //
            eMail email = new eMail();

            email.toMailAddress = myeMail;
            email.subject = "Kullanıcı kodu ";
            email.message =
                "<p> Üstad Bilişim veritabanında kayıtlı olan, </p>" +
                "<p> Kullanıcı Adı Soyadı : " + myUserFullName + "<br>" +
                " Kullanıcı Şifreniz : " + myKey + "</p>";

            t.eMailSend3(email);
        }

        #region User giriş yaptı, firma seçti

        void btn_FirmListSec_Click(object sender, EventArgs e)
        {
            /// Buraya geldiysen 
            /// kullanıcı için birden fazla firma seçeneği var demek ki
            /// kullanıcı bu butona basar ve seçilen değerler alınır lets go ... main form
            ///
            if (t.IsNotNull(dsUserFirmList))
            {
                DataRow row = dsUserFirmList.Tables[0].Rows[dNUserFirmList.Position];
                
                // Check if this is API-based flow or legacy flow
                if (apiClient != null && row.Table.Columns.Contains("FirmGUID"))
                {
                    // API-based flow
                    btn_FirmListSec_ClickApi(row);
                }
                else
                {
                    // Legacy SQL-based flow
                    userFirms.readUstadFirmAbout(this, row);
                }
            }
        }

        /// <summary>
        /// API-based firm selection handler
        /// </summary>
        async void btn_FirmListSec_ClickApi(DataRow row)
        {
            try
            {
                string firmGUID = row["FirmGUID"]?.ToString();
                if (string.IsNullOrEmpty(firmGUID))
                {
                    MessageBox.Show("Firma bilgisi bulunamadı.", "Hata", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get firm details via API
                var firmDetails = await apiClient.GetFirmDetailsAsync(firmGUID);

                if (firmDetails?.Firm != null)
                {
                    // Update global firm information
                    v.tMainFirm.FirmId = firmDetails.Firm.FirmId;
                    v.tMainFirm.FirmGuid = firmDetails.Firm.FirmGUID;
                    v.tMainFirm.FirmLongName = firmDetails.Firm.FirmLongName;

                    // Store user firm selection in registry
                    SetUserRegistryFirm(v.tUser.UserId, firmDetails.Firm.FirmId);

                    // Mark login as successful
                    v.SP_UserLOGIN = true;

                    // Close login form
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Firma bilgileri alınamadı.", "Hata", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Firma seçimi sırasında hata oluştu:\n" + ex.Message, "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Helper method to store user firm selection in registry
        /// </summary>
        void SetUserRegistryFirm(int userId, int firmId)
        {
            reg.SetUstadRegistry("userFirm" + userId.ToString(), firmId.ToString());
            reg.SetUstadRegistry("userLastFirm", firmId.ToString());
            v.tUserRegister.UserLastFirmId = firmId;
        }
        
        #endregion User giriş yaptı, firma seçti

        /// <summary>
        /// LEGACY METHOD - DEPRECATED - Should use API endpoint if needed
        /// This method contains database connection strings (security risk)
        /// </summary>
        [Obsolete("This method contains database connection strings.", false)]
        void SetUserIsActive(int Id)
        {
            tSql = Sqls.preparingUstadUsersSql("", "", Id);
            t.SQL_Read_Execute(v.dBaseNo.UstadCrm, ds_Query2, ref tSql, "UstadUsers", "SetUserIsActive");
        }
                
        void GetUserRegistry()
        {
            userFirms.GetUserRegistry(regPath);
            
            if (cmb_EMail != null)
            {
                // e-mail listesi combo için okunuyor
                ((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).Properties.Items.AddRange(v.tUserRegister.eMailList);
                // en son giriş yapan e-mail combonun text ine atanıyor
                if (cmb_EMail != null)
                    ((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).EditValue = v.tUserRegister.UserLastLoginEMail;
                if (uk_user_mail != null)
                    ((DevExpress.XtraEditors.TextEdit)uk_user_mail).EditValue = v.tUserRegister.UserLastLoginEMail;
                if (btn_BHatirla != null)
                {
                    ((DevExpress.XtraEditors.CheckButton)btn_BHatirla).Checked = v.tUserRegister.UserRemember;

                    // beni hatırla onaylıysa enson girilen key okunuyor ve şifreye atanıyor
                    if (((DevExpress.XtraEditors.CheckButton)btn_BHatirla).Checked)
                        ((DevExpress.XtraEditors.TextEdit)txt_Pass).EditValue = v.tUserRegister.UserLastKey;
                }
                u_user_Last_FirmId = v.tUserRegister.UserLastFirmId;
            }
        }

        void btn_YeniKullanici_Kaydet(object sender, EventArgs e)
        {
            if (t.IsNotNull(ds_NU) && (dN_NU != null))
            {
                if (dN_NU.Position > -1)
                {
                    string user_Email = ds_NU.Tables[0].Rows[0]["UserEMail"].ToString();
                    
                    // DEMO MODE: API ONLY - Check if user exists via API
                    if (apiClient != null)
                    {
                        btn_YeniKullanici_KaydetApi(user_Email);
                    }
                    else
                    {
                        MessageBox.Show("API bağlantısı kurulamadı. Kullanıcı kaydı yapılamıyor.", 
                            "API Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// API-based new user registration check
        /// </summary>
        async void btn_YeniKullanici_KaydetApi(string user_Email)
        {
            try
            {
                DebugLog("Checking if user exists: " + user_Email);
                var userExists = await apiClient.CheckUserExistsAsync(user_Email);

                if (userExists.Exists)
                {
                    MessageBox.Show(user_Email + "  hesabı mevcut. Lütfen giriş sayfasına dönün.", "Kullanıcı Mevcut", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    // User doesn't exist - proceed with registration
                    // Note: User registration should be done through API endpoint
                    // For now, showing message that registration needs API endpoint
                    MessageBox.Show("Yeni kullanıcı kaydı için API endpoint'ine ihtiyaç var.\n\n" + 
                        user_Email + " için kayıt işlemi şu anda desteklenmiyor.", 
                        "Kayıt Bilgisi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Optionally redirect to login
                    // t.SelectPage(this, "BACKVIEW", "USERLOGIN", -1);
                }
            }
            catch (Exception ex)
            {
                DebugLog("EXCEPTION in btn_YeniKullanici_KaydetApi: " + ex.Message);
                MessageBox.Show("Kullanıcı kontrolü sırasında hata oluştu:\n" + ex.Message, "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void btn_SifremiUnuttumClick(object sender, EventArgs e)
        {
            u_user_email = ((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).EditValue.ToString();

            DebugLog("Password reset requested for: " + u_user_email);

            // DEMO MODE: API ONLY - Legacy method commented out
            if (apiClient == null)
            {
                MessageBox.Show("API bağlantısı kurulamadı. API servisinin çalıştığından emin olun.", 
                    "API Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DebugLog("ERROR: API client is null - cannot process password reset");
                return;
            }

            checkedUserApi(u_user_email, "SEND_EMAIL"); // Secure API-based
            
            // LEGACY METHOD COMMENTED OUT FOR DEMO
            //checkedUser(u_user_email, "SEND_EMAIL"); // Legacy SQL-based (contains connection strings - SECURITY RISK)
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

                    // Use secure API method if available, fallback to legacy SQL method
                    if (apiClient != null)
                    {
                        btn_Yeni_SifreClickApi(user_email, user_old_pass, user_new_pass, user_rpt_pass);
                    }
                    else
                    {
                        btn_Yeni_SifreClickLegacy(user_email, user_old_pass, user_new_pass, user_rpt_pass);
                    }
                }
            }
        }

        /// <summary>
        /// Secure API-based password change - no database connection strings exposed
        /// </summary>
        async void btn_Yeni_SifreClickApi(string user_email, string user_old_pass, string user_new_pass, string user_rpt_pass)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(user_email))
                {
                    MessageBox.Show("Lütfen e-posta adresinizi giriniz.", "Eksik Bilgi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(user_old_pass))
                {
                    MessageBox.Show("Lütfen mevcut şifrenizi giriniz.", "Eksik Bilgi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(user_new_pass))
                {
                    MessageBox.Show("Lütfen yeni şifrenizi giriniz. Boş geçemezsiniz!", "Eksik Bilgi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (user_new_pass != user_rpt_pass)
                {
                    MessageBox.Show("DİKKAT : Yeni şifre ile tekrar yazdığınız şifre aynı değil...", "Şifre Uyumsuz", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (user_new_pass == user_old_pass)
                {
                    MessageBox.Show("Yeni şifre ile eski şifre aynı olamaz.", "Geçersiz Şifre", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (user_new_pass.Length < 4)
                {
                    MessageBox.Show("Lütfen en az 4 karakterlik bir şifre giriniz.", "Şifre Kısa", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Change password via secure API
                bool success = await apiClient.ChangePasswordAsync(user_email, user_old_pass, user_new_pass);

                if (success)
                {
                    MessageBox.Show("Şifreniz başarıyla güncellenmiştir...", "Başarılı", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                if (errorMsg.Contains("401") || errorMsg.Contains("Unauthorized") || errorMsg.Contains("incorrect"))
                {
                    MessageBox.Show("Mevcut şifreniz hatalı. Lütfen kontrol edin.", "Şifre Hatası", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (errorMsg.Contains("not found") || errorMsg.Contains("inactive"))
                {
                    checkedUserApi(user_email, "FIND");
                }
                else
                {
                            MessageBox.Show("Şifre değiştirme sırasında hata oluştu:\n" + errorMsg, "Hata", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Legacy SQL-based password change - CONTAINS DATABASE CONNECTION STRINGS (SECURITY RISK)
        /// Kept for backward compatibility when API is not available
        /// </summary>
        void btn_Yeni_SifreClickLegacy(string user_email, string user_old_pass, string user_new_pass, string user_rpt_pass)
        {
            // şimdi [ e-mail ile şifre ] databaseden kontrol ediliyor
            tSql = Sqls.preparingUstadUsersSql(user_email, user_old_pass, 0); 
            t.SQL_Read_Execute(v.dBaseNo.UstadCrm, ds_Query, ref tSql, "UstadUser", "UserLogin");

            if (t.IsNotNull(ds_Query))
            {
                int userId = t.myInt32(ds_Query.Tables[0].Rows[0]["UserId"].ToString());
                bool IsActive = Convert.ToBoolean(ds_Query.Tables[0].Rows[0]["IsActive"].ToString());
                string userFullName = ds_Query.Tables[0].Rows[0]["UserFullName"].ToString();
                string db_user_key = ds_Query.Tables[0].Rows[0]["UserKey"].ToString();

                if (IsActive == false)
                {
                    MessageBox.Show("DİKKAT : Bu hesap AKTİF değildir.");
                    return;
                }

                if (IsActive)
                {
                    if (db_user_key == user_old_pass)
                    {
                        if ((user_new_pass == user_rpt_pass) && // yeni şifre = tekrarlanan şifre
                            (user_new_pass != "") &&            // yeni şifre boş değilse
                            (user_new_pass != user_old_pass) && // yeni şifre ile eski şifre  aynı değilse
                            (user_new_pass.Length >= 4)         // en az 4 karekter   
                            )
                        {
                            tSql = Sqls.preparingUstadUsersSql("", user_new_pass, userId);

                            string myProp = string.Empty;
                            t.MyProperties_Set(ref myProp, "DBaseNo", "3");
                            t.MyProperties_Set(ref myProp, "TableName", "UstadUsers");
                            t.MyProperties_Set(ref myProp, "SqlFirst", tSql);
                            t.MyProperties_Set(ref myProp, "SqlSecond", "null");
                            ds_Query2.Namespace = myProp;

                            bool onay = t.Data_Read_Execute(this, ds_Query2, ref tSql, "NEW_USER_KEY", null);

                            if (onay) MessageBox.Show("Şifreniz başarıyla güncellenmiştir...");
                        }

                        if (user_new_pass == "")
                            MessageBox.Show("Lütfen bir şifre giriniz. Boş geçemezsiniz!");

                        if (user_new_pass != user_rpt_pass)
                            MessageBox.Show("DİKKAT : Yeni şifre ile tekrar yazdığınız şifre aynı değil...");

                        if (user_new_pass.Length < 4)
                            MessageBox.Show("Lütfen en az 4 karekterlik bir şifre giriniz.");
                    }
                }
            }

                    if (t.IsNotNull(ds_Query) == false)
                    {
                        //böyle bir email varmı diye kontrol et
                        // DEMO MODE: API ONLY
                        if (apiClient != null)
                        {
                            checkedUserApi(user_email, "FIND");
                        }
                        else
                        {
                            MessageBox.Show("API bağlantısı kurulamadı. Kullanıcı kontrolü yapılamıyor.", 
                                "API Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
        }

        private void ms_User_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Cleanup API client resources
            apiClient?.Dispose();
            apiClient = null;
        }

        private void ms_User_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((v.SP_UserLOGIN == false) &&
                (v.SP_UserIN == false))
                v.SP_ApplicationExit = true;
        }

        /// <summary>
        /// Debug logging method - outputs to Debug console and can be viewed in Visual Studio Output window
        /// </summary>
        void DebugLog(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logMessage = "[" + timestamp + "] [ms_User] " + message;
            
            // Output to Debug console (visible in Visual Studio Output window)
            System.Diagnostics.Debug.WriteLine(logMessage);
            
            // Also output to Console if available
            try
            {
                Console.WriteLine(logMessage);
            }
            catch { }
        }
    }
}
