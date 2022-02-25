using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using Tkn_Registry;

namespace YesiLcihazlar
{
    public class cihazStarter : tBase
    {
        tToolBox t = new tToolBox();
        cihazSqls Sql = new cihazSqls();

        public void InitStart()
        {
            v.EXE_PATH = Path.GetDirectoryName(Application.ExecutablePath);
            v.tExeAbout.activeExeName = Application.ProductName + ".exe";
            v.tExeAbout.activePath = Application.StartupPath;
            // output : { 25.03.2019 22:59:22 }
            DateTime dt = File.GetLastWriteTime(System.IO.Path.Combine(v.tExeAbout.activePath, v.tExeAbout.activeExeName));

            string yil = dt.Year.ToString();
            string ay = dt.Month.ToString();
            string gun = dt.Day.ToString();
            string saat = dt.Hour.ToString();
            string dakk = dt.Minute.ToString();

            if (ay.Length == 1) ay = "0" + ay;
            if (gun.Length == 1) gun = "0" + gun;
            if (saat.Length == 1) saat = "0" + saat;
            if (dakk.Length == 1) dakk = "0" + dakk;

            // output = { 20190325_2259 }
            v.tExeAbout.activeVersionNo = yil + ay + gun + "_" + saat + dakk;

            System.Globalization.CultureInfo tr = new System.Globalization.CultureInfo("tr-TR");
            System.Threading.Thread.CurrentThread.CurrentCulture = tr;
                       
            //t.WaitFormOpen(v.mainForm, "GetMacAdress ...");
            Get_MacAddress();

            //t.WaitFormOpen(v.mainForm, "Preparing Connection Strings ...");
            InitPreparingConnection();

            //t.WaitFormOpen(v.mainForm, "ManagerDB Connection...");
            t.Db_Open(v.active_DB.managerMSSQLConn);
                        
            //t.WaitFormOpen(v.mainForm, "Computer Info ...");
            InitLoginComputer();

            //t.WaitFormOpen(v.mainForm, "Preparing User Form ...");
            InitLoginUser();

            //t.WaitFormOpen(v.mainForm, "Screen Size Get ...");
            //Screen_Sizes_Get();
                        
            t.MSSQL_Server_Tarihi();

            v.SP_UserIN = true;
        }
        
        void Get_MacAddress()
        {
            /// Read computer network ethernet mac address
            /// 
            String macAddr = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic => nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();

            v.tComputer.Network_MACAddress = macAddr;
        }

        void InitPreparingConnection() // SqlConnection text set
        {
            ///
            /// ------------------------------------------------
            ///
            /// hangi database hangi databaseServer de çalışıyor  
            /// şimdilik manuel set ediyorum
            /// 
            v.active_DB.managerDBType = v.dBaseType.MSSQL;
            v.active_DB.projectDBType = v.dBaseType.MSSQL;
            //v.active_DB.projectDBType = v.dBaseType.MySQL;

            ///
            /// Manager DB Connections
            /// 
            #region

            v.active_DB.managerServerName = "94.73.145.8";
            v.active_DB.managerDBName = "MSV3DFTRBLT";
            v.active_DB.managerUserName = "user4601";
            v.active_DB.managerPsw = "Password = CanBerk98;";

            v.active_DB.managerConnectionText =
                string.Format(" Data Source = {0}; Initial Catalog = {1}; User ID = {2}; {3} MultipleActiveResultSets = True ",
                v.active_DB.managerServerName,
                v.active_DB.managerDBName,
                v.active_DB.managerUserName,
                v.active_DB.managerPsw);

            v.active_DB.managerMSSQLConn = new SqlConnection(v.active_DB.managerConnectionText);
            //v.active_DB.managerMSSQLConn.StateChange += new StateChangeEventHandler(DBConnectStateManager);

            #endregion

            // DİKKAT : BU METODU KULLANMA MASTER-DETAIL de DETAIL kırılıyor
            // v.SP_Conn_Text_Manager_MSSQL = " Server=94.73.145.8; Database=MSV3DFTRBLT; Uid=user4601;Pwd=CanBerk98";
        }
        
        void InitLoginComputer()
        {
            //MessageBox.Show(v.tComputer.Network_MACAddress);

            /// burada computer hakkında bilgi toplanıyor
            /// computer hakkındaki bilgi merkez datada bulunmakta (MVS3..)
            /// her computer network ethernet macaddresiyle takip edilmekte
            /// (MSV3..) datada computer bilgisi yoksa buradan 
            /// computer register formu açılmakta.
            /// Compter hakkında toplanan bilgiler ekranda müdehale edilemeyecek durumdadır
            /// Kullanıcıdan, hangi firma için kullanacak ise firm_guid istenmektedir
            /// eğer firm_guid yok ise sadece test firmalarını görebilir
            /// Firm_Guid aldığında da bu computer bilgileri sayesinde 
            /// firma için kayıt olan computer sayısı / lisans tespit edilmiş olacak

            bool onay = true;
            string compKey = v.tComputer.Network_MACAddress;

            // eğer bir şekilde eğer macaddresi gelmez ise
            //
            //if (IsNotNull(compKey) == false)
            //{
            //    ComputerAbout();
            //    compKey = v.tComputer.Processor_Id;
            //}

            string tSql = @" Select * from SYS_COMPS where NETWORK_MACADDRESS = '" + compKey + "' ";

            t.SQL_Read_Execute(v.dBaseNo.Manager, v.ds_Computer, ref tSql, "SYS_COMPS", "InitLoginComputer");

            if (t.IsNotNull(v.ds_Computer))
            {
                /// computer için planlanmış firma guid bilgisi
                ///
                v.tComp.SP_COMP_ISACTIVE = Convert.ToInt16(v.ds_Computer.Tables[0].Rows[0]["ISACTIVE"].ToString());

                // Aktif ise
                if (v.tComp.SP_COMP_ISACTIVE == 1)
                {
                    v.tComp.SP_COMP_ID = Convert.ToInt32(v.ds_Computer.Tables[0].Rows[0]["ID"].ToString());
                    v.tComp.SP_COMP_FIRM_ID = t.myInt32(v.ds_Computer.Tables[0].Rows[0]["COMP_FIRM_ID"].ToString());
                    v.tComp.SP_COMP_FIRM_GUID = v.ds_Computer.Tables[0].Rows[0]["COMP_FIRM_GUID"].ToString();
                    v.tComp.SP_COMP_SYSTEM_NAME = v.ds_Computer.Tables[0].Rows[0]["SYSTEM_NAME"].ToString();
                    v.tComp.SP_COMP_MACADDRESS = v.ds_Computer.Tables[0].Rows[0]["NETWORK_MACADDRESS"].ToString();
                    v.tComp.SP_COMP_PROCESSOR_ID = v.ds_Computer.Tables[0].Rows[0]["PROCESSOR_ID"].ToString();
                }

                // Pasif ise
                if (v.tComp.SP_COMP_ISACTIVE == 0)
                {
                    onay = false;
                    MessageBox.Show("Bilgisayarınız PASİF durumda. \r\n\r\n Destek ekibini arayarak bilgisayarınız AKTİF ettirebilirsiniz ...", "DİKKAT : ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // diğer modlarda ise
                if (v.tComp.SP_COMP_ISACTIVE > 1)
                {
                    onay = false;
                    MessageBox.Show("Bilgisayarınız  ( IsAcvite : " + v.tComp.SP_COMP_ISACTIVE.ToString() + " ) durumda.", "DİKKAT : ", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }

            }
            else
            {
                /// computer register için form
                onay = InitRegisterComputer(v.ds_Computer);
            }

            if (onay)
            {
                if (!string.IsNullOrEmpty(v.tComp.SP_COMP_FIRM_GUID))   // comp_firm_guid varsa
                    FirmList(v.tComp.SP_COMP_FIRM_GUID);
            }
        }

        void FirmList(string myGuid)
        {
            //set @_firmGUID = '046C4C69-EE2A-43C3-8E69-FF8A29F40844'

            /// hangi guidle içeriye giriş yapılıyor, sakla
            /// 
            v.SP_FIRM_GUID = myGuid;

            DataSet ds = new DataSet();

            string tSql = Sql.SQL_SYS_FIRM_List(myGuid, v.tFirmListType.OnlySelect);

            t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "SYS_FIRMS", "FirmList");

            if (t.IsNotNull(ds))
            {
                // tek firma geldiği için hangisiyle çalışacaksın diye sormak anlamsız olur
                // gerekli bilgileri al formu kapat, main formuna devam et
                if (ds.Tables[0].Rows.Count == 1)
                {
                    string ufl = "";
                    DataRow row = ds.Tables[0].Rows[0];
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
                    //SetUserRegistryFirm(v.tUser.SP_USER_ID, v.SP_FIRM_ID);

                    // Login onayı
                    //
                    v.SP_UserLOGIN = true;
                }
            }
        }
        
        #region InitRegisterComputer
        private bool InitRegisterComputer(DataSet dataSet)
        {
            bool onay = true;

            vUserInputBox iBox = new vUserInputBox();

            iBox.Clear();
            iBox.title = "Firmamın / Kurumun Yazılım Anahtarı";
            iBox.promptText = "Yazılım Anahtarı  :";
            iBox.value = "";// OldIPCode;
            iBox.displayFormat = "";
            iBox.fieldType = 0;

            // ınput box ile sorulan ise kimden kopyalanacak (old) bilgisi
            if (t.UserInpuBox(iBox) == DialogResult.OK)
            {
                string guid = iBox.value;

                if (t.IsNotNull(guid))
                {
                    try
                    {
                        string Sql = @" Select * from SYS_FIRMS where FIRM_GUID = '" + guid + "' ";
                        DataSet ds = new DataSet();
                        t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref Sql, "SYS_FIRMS", "InitLoginComputer");

                        if (ds.Tables[0].Rows.Count == 1)
                        {
                            int firmId = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"].ToString());
                            SYS_COMPS_Fill(dataSet, guid, firmId);
                        }
                        else
                        {
                            onay = false;
                            MessageBox.Show("DİKKAT : Girdiğiniz anahtar için uygun firma / kurum bulunamadı...");
                        }
                        ds.Dispose();
                    }
                    catch (Exception)
                    {
                        onay = false;
                        MessageBox.Show("DİKKAT : Hatalı anahtar girdiniz.");
                        //throw;
                    }
                    
                    
                }
                
            }

            return onay;
        }
        
        private void SYS_COMPS_Fill(DataSet dataSet, string guid, int firmId)
        {
            t.ComputerAbout();

            DataRow row = dataSet.Tables[0].NewRow();
            row["ISACTIVE"] = 1;
            row["SYSTEM_NAME"] = v.tComputer.SystemName.ToString();
            row["NETWORK_MACADDRESS"] = v.tComputer.Network_MACAddress.ToString();
            row["PROCESSOR_NAME"] = v.tComputer.Processor_Name.ToString();
            row["PROCESSOR_ID"] = v.tComputer.Processor_Id.ToString();
            row["DISK_MODEL"] = v.tComputer.DiskDrive_Model.ToString();
            row["DISK_SERIALNUMBER"] = v.tComputer.DiskDrive_SerialNumber.ToString();
            row["OPERATION_MODE_TD"] = 0;
            row["COMP_FIRM_GUID"] = guid;
            row["COMP_FIRM_ID"] = firmId;
            
            dataSet.Tables[0].Rows.Add(row);

            /// kullanıcın yeni girdiği firm_guidi al
            ///
            v.tComp.SP_COMP_FIRM_ID = firmId;
            v.tComp.SP_COMP_FIRM_GUID = guid;

            /// compter yeni kayıt sırasında aktif kabul edilecek
            /// 
            v.tComp.SP_COMP_ISACTIVE = 1;
            
            string Sql = SYS_COMPS_Insert_Sql(row);


            DataSet ds = new DataSet();

            string myProp = string.Empty;
            t.MyProperties_Set(ref myProp, "DBaseNo", "2");
            t.MyProperties_Set(ref myProp, "TableName", "SYS_COMP");
            ds.Namespace = myProp;

            t.Data_Read_Execute(ds, ref Sql, "SYS_COMP", null);
            //t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref Sql, "SYS_COMP", null);
            ds.Dispose();

        }

        private string SYS_COMPS_Insert_Sql(DataRow row)
        {
            string Sql = @"
     INSERT INTO [dbo].[SYS_COMPS]
           ([PARENT_ID]
           ,[ISACTIVE]
           ,[COMP_FIRM_GUID]
           ,[SYSTEM_NAME]
           ,[NETWORK_MACADDRESS]
           ,[PROCESSOR_NAME]
           ,[PROCESSOR_ID]
           ,[DISK_MODEL]
           ,[DISK_SERIALNUMBER]
           ,[OPERATION_MODE_TD]
           ,[REC_DATE]
           ,[COMP_FIRM_ID])
     VALUES
           (0";
            Sql = Sql 
            + "," + row["ISACTIVE"]
            + ",'" + row["COMP_FIRM_GUID"] + "'"     //<COMP_FIRM_GUID, varchar(50),>
            + ",'" + row["SYSTEM_NAME"] + "'"        //<SYSTEM_NAME, varchar(50),>
            + ",'" + row["NETWORK_MACADDRESS"] + "'" //<NETWORK_MACADDRESS, varchar(30),>
            + ",'" + row["PROCESSOR_NAME"] + "'"     //<PROCESSOR_NAME, varchar(50),>
            + ",'" + row["PROCESSOR_ID"] + "'"       //<PROCESSOR_ID, varchar(50),>
            + ",'" + row["DISK_MODEL"] + "'"         //<DISK_MODEL, varchar(50),>
            + ",'" + row["DISK_SERIALNUMBER"] + "'"  //<DISK_SERIALNUMBER, varchar(50),>
            + "," + row["OPERATION_MODE_TD"]         //<OPERATION_MODE_TD, smallint,>
            + ", getdate() "                         //+ row["REC_DATE"] //<REC_DATE, date,>
            + "," + row["COMP_FIRM_ID"]              //<COMP_FIRM_ID, int,>
            + ")";
        return Sql;
        }
        
        #endregion InitRegisterComputer

        #region InitLoginUser
        void InitLoginUser()
        {

            /*
            string FormName = "ms_User";
            string FormCode = "UST/PMS/PMS/SYS_USERLOGIN";// "SYS_USERLOGIN";

            string Prop_Navigator = @"
            0=FORMNAME:" + FormName + @";
            0=FORMCODE:" + FormCode + @";
            0=FORMTYPE:DIALOG;
            0=FORMSTATE:NORMAL;
            ";

            OpenForm(null, Prop_Navigator);
            */

            v.SP_UserLOGIN = true;
            //v.SP_UserLOGIN = false;

        }
        #endregion InitLoginUser

        void GetDeviceRegistry()
        {
            //tRegistry reg = new tRegistry();

            //deviceFirmGuid = reg.getRegistryValue("deviceFirmGuid");

            /*
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

        
                }
            }
            */
        }
        
    }
}
