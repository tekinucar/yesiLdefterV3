using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using fingerIZKEM;


namespace YesiLcihazlar
{
    public partial class frmZkTeco : Form
    {
        fingerK50_SDKHelper SDK = null;

        fingerProcedures fP = new fingerProcedures();

        private string cihazIP = "192.168.2.131";
        private string cihazPort = "4370";

        public frmZkTeco()
        {
            InitializeComponent();

            SDK = new fingerK50_SDKHelper(fP.RaiseDeviceEvent);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            bool ret = SDK.Connect_Net(cihazIP, Convert.ToInt32(cihazPort));
            if (ret) textBox1.Text = "connect";
            else textBox1.Text = "disconnect";
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            SDK.Disconnect();
            textBox1.Text = "disconnect";
        }



        #region yeni kişiyi cihaza gönder

        private void btnSetUser_Click(object sender, EventArgs e)
        {
            /*
            fP.tSetUserInfo(SDK, 1
                , txtId.Text
                , txtName.Text
                , 0
                , ""
                , ""
                );
                */
            //fP.SetUserDataToDevice(SDK, 1, "1", "tekin uçar");

            
        }


        private void btnGetUser_Click(object sender, EventArgs e)
        {
            /*
            
            while (objZkeeper.SSR_GetAllUserInfo(machineNumber, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
            {
                for (idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
                {
                    if (objZkeeper.GetUserTmpExStr(machineNumber, sdwEnrollNumber, idwFingerIndex, out iFlag, out sTmpData, out iTmpLength))
                    {
            */

            //SDK.GetUserTmpEx

            Cursor = Cursors.WaitCursor;

            //UserMng.SDK.sta_GetUserInfo(UserMng.lbSysOutputInfo, txtUserID, txtName, cbPrivilege, txtCardnumber, txtPassword);

            Cursor = Cursors.Default;

        }


        //public int sta_GetUserInfo(ListBox lblOutputInfo, TextBox txtUserID, TextBox txtName, ComboBox cbPrivilege, TextBox txtCardnumber, TextBox txtPassword)
        public int sta_GetUserInfo(int iMachineNumber, string userId)
        {
            /*
            if (GetConnectState() == false)
            {
                lblOutputInfo.Items.Add("*Please connect first!");
                return -1024;
            }

            if (txtUserID.Text.Trim() == "")
            {
                lblOutputInfo.Items.Add("*Please input user id first!");
                return -1023;
            }

            int iPIN2Width = 0;
            string strTemp = "";
            axCZKEM1.GetSysOption(GetMachineNumber(), "~PIN2Width", out strTemp);
            iPIN2Width = Convert.ToInt32(strTemp);

            if (txtUserID.Text.Length > iPIN2Width)
            {
                lblOutputInfo.Items.Add("*User ID error! The max length is " + iPIN2Width.ToString());
                return -1022;
            }
            */

            /*
            int idwErrorCode = 0;
            int iPrivilege = 0;
            string strName = "";
            string strCardno = "";
            string strPassword = "";
            bool bEnabled = false;

            string userName = "";
            string userPassword = "";
            string Cardnumber = "";

            //axCZKEM1.EnableDevice(iMachineNumber, false);
            SDK.EnableDevice(iMachineNumber, false);
            //if (axCZKEM1.SSR_GetUserInfo(iMachineNumber, txtUserID.Text.Trim(), out strName, out strPassword, out iPrivilege, out bEnabled))//upload the user's information(card number included)
            if (SDK.SSR_GetUserInfo(iMachineNumber, userId, out strName, out strPassword, out iPrivilege, out bEnabled))//upload the user's information(card number included)
            {
                SDK.GetStrCardNumber(out strCardno);
                if (strCardno.Equals("0"))
                {
                    strCardno = "";
                }
                userName = strName;
                userPassword = strPassword;
                txtCardnumber.Text = strCardno;
                cbPrivilege.SelectedIndex = iPrivilege;
                lblOutputInfo.Items.Add("Get user information successfully");
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                //modify by Leonard 2017/12/18
                txtName.Text = " ";
                txtPassword.Text = " ";
                txtCardnumber.Text = " ";
                cbPrivilege.SelectedIndex = 5;
                lblOutputInfo.Items.Add("The User is not exist");
                //end by Leonard
                lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);
            */
            return 1;
        }

        /*
        void get()
        {
            if (objZkeeper.GetUserTmpExStr(machineNumber, sdwEnrollNumber, idwFingerIndex, out iFlag, out sTmpData, out iTmpLength))
            {
                fingerUserInfo fpInfo = new fingerUserInfo();
                fpInfo.MachineNumber = machineNumber;
                fpInfo.EnrollNumber = sdwEnrollNumber;
                fpInfo.Name = sName;
                fpInfo.FingerIndex = idwFingerIndex;
                fpInfo.TmpData = sTmpData;
                fpInfo.Privelage = iPrivilege;
                fpInfo.Password = sPassword;
                fpInfo.Enabled = bEnabled;
                fpInfo.iFlag = iFlag.ToString();

                lstFPTemplates.Add(fpInfo);
            }
        }
        */
        #endregion




        #region kişiyi güncelle


        void CihazlariGuncelle()
        {
            fingerK50_SDKHelper SDK2 = new fingerK50_SDKHelper(fP.RaiseDeviceEvent);
            fP.DeleteUserFromMachine(SDK2, 1, 1);// cihaz.Id, oHukumlu.Id);
            fP.SetUserDataToDevice(SDK2, 1, "1", "tekin uçar");
            
            /*
            DeviceManipulator deviceManipulator = new DeviceManipulator();
            SDKHelper SDK2 = new SDKHelper(RaiseDeviceEvent);
            try
            {
                // List<CihazKarti> listInfo = new CihazKartiVeriKatmani().SayimCihazlari();
                List<CihazKarti> listInfo = new CihazKartiVeriKatmani().GuncellenecekCihazListesi();

                foreach (var cihaz in listInfo)
                {
                    if (IsDeviceConnected) { SDK2.Disconnect(); isDeviceConnected = false; }

                    bool ret = SDK2.Connect_Net(cihaz.IP.Trim(), Convert.ToInt32(cihaz.Port));

                    if (ret)
                    {
                        if (oHukumlu.Durum)
                        {
                            oHukumlu.CihazlaraGonderildi = deviceManipulator.DeleteUSerFromMachine(SDK2, cihaz.Id, oHukumlu.Id);
                            oHukumlu.CihazlaraGonderildi = deviceManipulator.PushUserDataToDevice(SDK2, cihaz.Id, oHukumlu.Id.ToString(), oHukumlu.AdSoyad);
                        }
                        else
                            oHukumlu.CihazlaraGonderildi = deviceManipulator.DeleteUSerFromMachine(SDK2, cihaz.Id, oHukumlu.Id);

                        //if (oHukumlu.CihazlaraGonderildi)
                        //{
                        //    MessageBox.Show(oHukumlu.AdSoyad + " " + cihaz.Ad + " cihazında güncellendi.");
                        //}

                    }
                    else
                    {
                        MessageBox.Show(cihaz.Ad + " isimli cihaza bağlanamadı. ");
                    }
                    SDK2.Disconnect();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata.Bağlanamadı." + ex.Message);
            }
            finally
            {
                SDK2.Disconnect();

                MessageBox.Show(oHukumlu.AdSoyad + " verileri cihazlarda güncellendi");
            }
            */
        }

        #endregion

        #region tüm kişilerin listesi

        private void btnGetUserAllData_Click(object sender, EventArgs e)
        {
            
        }


        #endregion

        #region Cihazdan Dataları oku

        private void Btn_CihazdanAl_Click(object sender, EventArgs e)
        {
            //ICollection<MachineInfo> list = new DeviceManipulator().GetLogData(SDK, cihaz.Id);
            ICollection<fingerLogData> list = fP.GetLogData(SDK, 1);

            /*

            List<CihazKarti> listInfo = new CihazKartiVeriKatmani().Liste().Where(c => c.KullanimSekli == KullanimSekli.GirisCikis).ToList();
            OkumaVeriKatmani okumaVeriKatmani = new OkumaVeriKatmani();
            try
            {
                Cursor = Cursors.WaitCursor;
                foreach (var cihaz in listInfo)
                {
                    if (IsDeviceConnected) { SDK.Disconnect(); isDeviceConnected = false; }


                    bool ret = SDK.Connect_Net(cihaz.IP.Trim(), Convert.ToInt32(cihaz.Port));
                    if (ret)
                    {
                        DateTime? sonOkumaZamanı = okumaVeriKatmani.SonOkumaZamani(cihaz.Id);
                        List<MachineInfo> yeniOkumalar = null;

                        ICollection<MachineInfo> list = new DeviceManipulator().GetLogData(SDK, cihaz.Id);
                        if (sonOkumaZamanı.HasValue)
                        {
                            string sonOkumaZamaniString = sonOkumaZamanı.Value.Year.ToString() + "-" +
                                                          sonOkumaZamanı.Value.Month.ToString() + "-" +
                                                          sonOkumaZamanı.Value.Day.ToString() + "-" + " " +
                                                          sonOkumaZamanı.Value.Hour.ToString() + ":" +
                                                          sonOkumaZamanı.Value.Minute.ToString() + ":" +
                                                          sonOkumaZamanı.Value.Second.ToString();
                            yeniOkumalar = list.Where(m => Convert.ToDateTime(m.DateTimeRecord) >= sonOkumaZamanı).ToList();
                        }
                        else
                            yeniOkumalar = list.ToList();

                        if (yeniOkumalar != null)
                        {
                            foreach (var item in yeniOkumalar)
                            {
                                Okuma okuma = new Okuma();
                                okuma.Aciklama = ""; // "Verify OK.UserID=" + item["User Id"].ToString() + " isInvalid=" + item["WorkCode"] + " state=" + item["Verify State"] + " verifystyle=" + item["Verify Type"].ToString();
                                okuma.OkunanId = Convert.ToInt32(item.IndRegID);
                                okuma.Zaman = Convert.ToDateTime(item.DateTimeRecord);
                                okuma.CihazId = cihaz.Id;
                                okumaVeriKatmani.Ekle(okuma);
                            }
                        }

                        yeniOkumalar = null;

                    }

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                // tekin
                // Yeni eklenecek komutlar
                SayimIcmaliVeriKatmani sayimIcmali = new SayimIcmaliVeriKatmani();
                sayimIcmali.SayimIcmaliSumExec();
                // ---

                SDK.Disconnect();
                this.Cursor = Cursors.Default;
            }
            MessageBox.Show("Güncelleme tamamlandı.");
            Cursor = Cursors.Default;
            Sorgula();
            */

        }

        #endregion


        #region cihaza bilgileri gönder
        private void Btn_AktarimBaslat_Click(object sender, EventArgs e)
        {
            List<fingerUserInfo> users = new List<fingerUserInfo>();
            fP.UploadFTPTemplate(SDK, 1, users);

            /*
            List<fingerUserInfo> users = new List<fingerUserInfo>();
            DeviceManipulator device = new DeviceManipulator();
            label2.Text = "Cihaza bağlanıyor...";
            try
            {
                if (IsDeviceConnected) { SDK.Disconnect(); isDeviceConnected = false; }
                Cursor = Cursors.WaitCursor;
                bool ret = SDK.Connect_Net(CihazInfo.IP.Trim(), Convert.ToInt32(CihazInfo.Port));

                if (ret)
                {
                    label2.Text = "Aktarım başladı...";

                    for (int i = 0; i < hukumluListesi.Count; i++)
                    {
                        Hukumlu hukumlu = hukumluListesi[i];
                        UserInfo info = new UserInfo
                        {
                            Enabled = true,
                            FingerIndex = 0,
                            EnrollNumber = hukumlu.Id.ToString(),
                            iFlag = "1",
                            MachineNumber = CihazInfo.Id,
                            Name = new StringIslemleri().TurkceKarakterDuzelt(hukumlu.AdSoyad),
                            Password = "",
                            Privelage = 0,
                            TmpData = hukumlu.ParmakID

                        };
                        users.Add(info);

                        progressBar1.Value++;
                        progressBar1.Update();
                    }

                    bool sonuc = device.UploadFTPTemplate(SDK, CihazInfo.Id, users);
                    label2.Text = "Aktarım tamamlandı...";
                    //SDK.Disconnect();
                }
                else
                    MessageBox.Show("Cihaza bağlanamadı!");
            }
            catch (Exception ex)
            {

            }
            finally
            {
                SDK.Disconnect();
                Cursor = Cursors.Default;
            }

    */
        }

        #endregion

        #region silme işlemi
        private void btn_HukumluSil_Click(object sender, EventArgs e)
        {

            fP.ClearData(SDK, 1, ClearFlag.UserData);
            /*
            int[] selRows = ((GridView)grd_KayitliCihazListesi.MainView).GetSelectedRows();
            CihazInfo cihazInfo = (CihazInfo)(((GridView)grd_KayitliCihazListesi.MainView).GetRow(selRows[0]));

            DialogResult result = MessageBox.Show(cihazInfo.Ad + " cihazındaki bütün hükümlü kayıtları silinecektir. Onaylıyor musunuz?", "Cihazdan Silme İşlemi", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {

                List<Hukumlu> hukumluListesi = new HukumluVeriKatmani().Liste().Where(h => h.Durum).ToList();
                try
                {
                    if (IsDeviceConnected) { SDK.Disconnect(); isDeviceConnected = false; }
                    Cursor = Cursors.WaitCursor;
                    bool ret = SDK.Connect_Net(cihazInfo.IP.Trim(), Convert.ToInt32(cihazInfo.Port));

                    if (ret)
                    {
                        deviceManipulator.ClearData(SDK, cihazInfo.Id, ClearFlag.UserData);
                        ////for (int i = 0; i < hukumluListesi.Count; i++)
                        ////{
                        ////    SDK.DeleteEnrollData(cihazInfo.Id, hukumluListesi[i].Id, 0, 0);
                        ////}

                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    SDK.Disconnect();
                    this.Cursor = Cursors.Default;

                }

            }
            */
        }

        #endregion

        #region Kayıt Cihazından Parmak Kaydını Al
        void ListeGetir()
        {
            /*
            List<CihazKarti> cihazListesi = new CihazKartiVeriKatmani().Liste();
            List<CihazKarti> kayitCihazlari = cihazListesi.FindAll(c => c.KullanimSekli == KullanimSekli.Kayit);
            sayimCihazlari = cihazListesi.FindAll(c => c.KullanimSekli == KullanimSekli.Sayim);

            parmakiziOlmayanHukumluler = new List<Hukumlu>();
            DeviceManipulator deviceManipulator = new DeviceManipulator();

            comboBox1.Text = "Aktif";// Liste acılırken aktıf olan kumlulerı getırır

            try
            {
                Cursor = Cursors.WaitCursor;
                foreach (var cihaz in kayitCihazlari)
                {
                    if (IsDeviceConnected) { SDK.Disconnect(); isDeviceConnected = false; }

                    bool ret = SDK.Connect_Net(cihaz.IP.Trim(), Convert.ToInt32(cihaz.Port));
                    string fingerPrint = string.Empty;
                    int fingerLength = 0;
                    if (ret)
                    {

                        List<UserInfo> fingerPrints = deviceManipulator.GetAllUserFPInfo(SDK, cihaz.Id).ToList();

                        for (int i = 0; i < fingerPrints.Count; i++)
                        {
                            hukumluVeriKatmani.ParmakIziIDGuncelle(Convert.ToInt32(fingerPrints[i].EnrollNumber), fingerPrints[i].TmpData);
                            parmakiziOlmayanHukumluler.Add(hukumluVeriKatmani.IDileGetir(Convert.ToInt32(fingerPrints[i].EnrollNumber)));
                        }
                        deviceManipulator.ClearData(SDK, cihaz.Id, ClearFlag.UserData);
                        SDK.Disconnect();

                        //foreach (var hukumlu in parmakiziOlmayanHukumluler)
                        //{
                        //    if(deviceManipulator.GetUserFingerPrint(SDK, cihaz.Id, hukumlu.Id, ref fingerPrint, fingerLength))
                        //    {
                        //            hukumlu.ParmakID = fingerPrint;
                        //            hukumluVeriKatmani.ParmakIziIDGuncelle(hukumlu.Id, fingerPrint.ToString());
                        //    }     
                        //}

                    }
                    else if (ret)
                    {
                        MessageBox.Show(cihaz.Ad + " isimli cihaza bağlanamadı. ");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata.Bağlanamadı." + ex.Message);
                return;
            }
            finally
            {
                SDK.Disconnect();
                Cursor = Cursors.Default;
            }

            grd_HukumluListesi.DataSource = null;
            grd_HukumluListesi.DataSource = liste;
            grd_HukumluListesi.RefreshDataSource();

            backgroundWorker1.RunWorkerAsync();

            */

        }




        #endregion

        private void btnReadAttLog_Click(object sender, EventArgs e)
        {
            fP.tReadAttLog(SDK, 1, v.dsFingerLogData, null);
            //fP.tReadNewAttLog(SDK, 1, v.dsFingerLogData);
            //fP.tReadLogByPeriod(SDK, 1, v.dsFingerLogData, "00:01", "22:00");

            dataGridView1.DataSource = v.dsFingerLogData;
        }
    }
}
