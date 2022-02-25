using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Windows.Forms;
using Tkn_Events;
using Tkn_Registry;
using Tkn_SQLs;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_UserFirmList : Form
    {
        tToolBox t = new tToolBox();
        tRegistry reg = new tRegistry();

        // FL = FirmList
        DataSet ds_FL = null;
        DataNavigator dN_FL = null;
        DataSet ds_Query2 = new DataSet();
        Control tree_FirmList = null;

        string tSql = string.Empty;
        string TableIPCode = string.Empty;
        string FirmList_TableIPCode = "UST/PMS/Firm.UserFirmList_L01";//"SYSFIRM.SYSFIRM_L02";
        int u_user_Last_FirmId = 0;

        string regPath = v.registryPath;


        public ms_UserFirmList()
        {
            InitializeComponent();

            tEventsForm evf = new tEventsForm();

            this.Load += new System.EventHandler(evf.myForm_Load);
            this.Shown += new System.EventHandler(evf.myForm_Shown);
            this.Shown += new System.EventHandler(this.ms_UserFirmList_Shown);

            this.KeyPreview = true;
        }

        private void ms_UserFirmList_Shown(object sender, EventArgs e)
        {
            //
            // FirmList -------------------------------------------------------------------
            //
            TableIPCode = FirmList_TableIPCode;

            Control cntrl = null;
            string[] controls = new string[] { };
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

            // Kullanıcı çalışacağı firmayı sececek
            //
            SelectFirm();
        }

        void GetUserRegistry()
        {
            var regUser = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"" + regPath);

            if (regUser != null)
            {

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
            }
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
                v.tFirmUserList.Clear();
                v.SP_FIRM_USERLIST = "";

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
                    //this.Close();
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



    }
}
