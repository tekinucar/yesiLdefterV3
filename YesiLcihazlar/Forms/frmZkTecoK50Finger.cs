using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YesiLcihazlar.Forms
{
    public partial class frmZkTecoK50Finger : Form
    {
        tToolBox t = new tToolBox();
        cihazSqls Sql = new cihazSqls();
        fingerProcedures fP = new fingerProcedures();

        int cihazId = 0;
        string cihazAdi = "";
        string cihazCalismaTipi = "";
        string cihazIp = "";
        string cihazPort = "";

        int selCihazId = 0;
        int selCihazCalismaTipiId = 0;
        string selCihazAdi = "";
        string selCihazIp = "";
        string selCihazPort = "";
        string globalLog = "";

        int icmal1TalepCount = 0;
        int icmal2TalepCount = 0;

        //Cursor = Cursors.WaitCursor;
        //Cursor = Cursors.Default;
        //Application.DoEvents();

        public frmZkTecoK50Finger()
        {
            InitializeComponent();

            // yapılack işlerin icmali
            CihazLogGet(v.cihazTalepTipi.chIcmal, v.dsCihazLogIcmal1);

            // cihazları oku
            getCihazList();

            if (v.dsCihazList.Tables[0].Rows.Count > 0)
                preparingCihazList(v.dsCihazList);

            v.tIslemTarihi.Yil = DateTime.Now.Year;
            v.tIslemTarihi.Ay = DateTime.Now.Month;
            v.tIslemTarihi.Gun = DateTime.Now.Day;

            // 1 saniye =  1000
            // 1 dakika = 60000

            timer_CihazEmir.Interval = 1000 * 5; //  5 saniye
            timer_CihazEmir.Enabled = true;
            timer_Icmal1.Interval = 60000 * 10;  // 10 dak
            timer_Icmal1.Interval = 10000;  // 10 saniye
            timer_Icmal1.Enabled = true;
            timer_Icmal2.Interval = 60000 * 5;   //  5 dak
            timer_Icmal2.Interval = 5000;  // 5 saniye
            timer_Icmal2.Enabled = false;

            v.listBox1 = listBox1;
        }

        private void getCihazList()
        {
            string tSql = Sql.CihazHesapListSql(ref v.dsCihazList);

            try
            {
                dataGridView2.Tag = "1";

                if (v.dsCihazList.Tables.Count > 0)
                {
                    if (v.dsCihazList.Tables[0].Rows.Count > 0)
                        v.dsCihazList.Tables[0].Clear();//  RemoveAt(0);
                }

                t.Data_Read_Execute(v.dsCihazList, ref tSql, "CihazHesap", null);

                dataGridView2.DataSource = v.dsCihazList.Tables[0];

                dataGridView2.Tag = "0";
                dataGridView2.Rows[0].Selected = true;
                getCihazValues();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void preparingCihazList(DataSet table)
        {
            int rowId = 0;
            foreach (DataRow row in table.Tables[0].Rows)
            {
                cihazId = t.myInt32(row["CihazId"].ToString());
                cihazAdi = row["CihazAdi"].ToString();
                cihazCalismaTipi = row["CalismaTipi"].ToString();
                cihazIp = row["CihazIp"].ToString();
                cihazPort = row["CihazPort"].ToString();
                if (cihazPort == "")
                    cihazPort = "4370";

                cihazHesap ch = new cihazHesap();
                ch.Id = rowId;
                ch.CihazId = cihazId;
                ch.Connnect = false;
                ch.CihazAdi = cihazAdi;
                ch.CihazCalismaTipi = cihazCalismaTipi;
                ch.CihazIp = cihazIp;
                ch.CihazPort = cihazPort;

                fingerK50_SDKHelper SDK = new fingerK50_SDKHelper(fP.RaiseDeviceEvent);
                ch.Sdk = SDK;

                v.tCihazHesapList.Add(ch);

                rowId++;
            }
        }

        private void CihazLogGet(v.cihazTalepTipi talep, DataSet ds)
        {
            string Param = fP.talepNameGet(talep);
            try
            {
                if (talep == v.cihazTalepTipi.chIcmal)
                {
                    dataGridView1.Tag = "1";

                    getDetay(Param, ds);

                    if (ds.Tables.Count > 0)
                    {
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = ds.Tables[0];
                    }
                    else MessageBox.Show("DİKKAT : " + Param + " için gerekli data tablosu bulunamadı...  ");

                    dataGridView1.Tag = "0";

                }
                else
                {
                    dataGridView3.Tag = "1";

                    getDetay(Param, ds);

                    if (ds.Tables.Count > 0)
                    {
                        dataGridView3.DataSource = null;
                        dataGridView3.DataSource = ds.Tables[0];
                    }
                    else MessageBox.Show("DİKKAT : " + Param + " için gerekli data tablosu bulunamadı...  ");
                    
                    dataGridView3.Tag = "0";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void getDetay(string Param, DataSet dS)
        {
            string tSql = Sql.prc_CihazLogGetSql(Param, ref dS);

            try
            {
                if (dS.Tables.Count > 0)
                    dS.Tables.Clear();

                t.Data_Read_Execute(dS, ref tSql, "CihazLogGet", null);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void myViewLog(string s)
        {
            Application.DoEvents();
            listBox1.Items.Add(s);
            Application.DoEvents();
        }


        private void tViewData(DataSet table)
        {
            dataGridView3.DataSource = null;
            dataGridView3.DataSource = table.Tables[0];
        }


        private void workFinish(int sonuc, string mesaj)
        {
            if (sonuc == 1)
            {
                v.progressBar1.Value = 0;
                v.mesajLabel1.Text = mesaj;
            }
        }



        #region Cihaz Kontrol İşlemleri

        private void btnCihazTest_Click(object sender, EventArgs e)
        {
            myViewLog(fP.deviceWork(selCihazId, v.cihazTalepTipi.chTest));
        }
        private void btnCihazLogCount_Click(object sender, EventArgs e)
        {
            myViewLog(fP.deviceWork(selCihazId, v.cihazTalepTipi.chLogCount));
        }
        private void btnGetCihazSetTarihSaat_Click(object sender, EventArgs e)
        {
            myViewLog(fP.deviceWork(selCihazId, v.cihazTalepTipi.chGetTarihSaat));
        }
        private void btnSetCihazSetTarihSaat_Click(object sender, EventArgs e)
        {
            myViewLog(fP.deviceWork(selCihazId, v.cihazTalepTipi.chSetTarihSaat));
        }
        private void btnCihazReset_Click(object sender, EventArgs e)
        {
            myViewLog(fP.deviceWork(selCihazId, v.cihazTalepTipi.chReset));
        }

        #endregion cihaz kontrol işlemleri

        #region Cihazdan Data Okuma

        private void btnGetAllLogs_Click(object sender, EventArgs e)
        {
            manuelGetAllLogs();
        }
        private void btnGetAllUserAndFPs_Click(object sender, EventArgs e)
        {
            manuelGetAllUserAndFPs();
        }

        private void manuelGetAllLogs()
        {
            if (fP.manuelTalepRunStart() == false) return;

            fP.tGetAllLogs(selCihazId, v.dsFingerLogData, v.tIslemTarihi);

            dataGridView6.DataSource = null;
            dataGridView6.DataSource = v.dsFingerLogData.Tables[0];

            v.manuelTalepRun = false;
        }

        private void manuelGetAllUserAndFPs()
        {
            if (fP.manuelTalepRunStart() == false) return;

            fP.tGetAllUserAndFPs(selCihazId, v.dsUserInfo);
            tViewData(v.dsUserInfo);

            v.manuelTalepRun = false;
        }


        #endregion Cihazdan Data Okuma

        #region Cihaza Bilgi Gönder 
        private void btnSetAllUserAndFPs_Click(object sender, EventArgs e)
        {
            if (fP.manuelTalepRunStart() == false) return;
            setAllUserAndFPs();
            v.manuelTalepRun = false;
        }

        private void setAllUserAndFPs()
        {
            // parmak izi olan tüm kullanicilari oku
            CihazLogGet(v.cihazTalepTipi.chGetAllUserAndFPs, v.dsCihazLogDetay); // "GetFullUserFp"
            // gelen tüm kullanıcıları cihaza gönder
            sendTable(v.dsCihazLogDetay, v.cihazTalepTipi.chSetAllUserAndFPs);
        }

        #endregion Cihaza Bilgi Gönder

        #region Cihazda Kayıt Silme işlemleri

        private void btnClearAllLogs_Click(object sender, EventArgs e)
        {
            ClearAllLogs();
        }

        private void ClearAllLogs()
        {
            if (v.CihazIsActive)
            {
                fP.sta_ClearAllLogs(selCihazId, ref globalLog);
                myViewLog(globalLog);
            }
            else
            {
                myViewLog(Sql.cihazTestLogClearSql(selCihazId));
            }
        }

        private void btnClearAllFps_Click(object sender, EventArgs e)
        {
            ClearAllFps();
        }

        private void ClearAllFps()
        {
            if (v.CihazIsActive)
            {
                fP.sta_ClearAllFps(selCihazId, ref globalLog);
                myViewLog(globalLog);
            }
            else
            {
                myViewLog(Sql.cihazTestUserClearFpsSql(selCihazId));
            }
        }

        private void btnClearAllUsers_Click(object sender, EventArgs e)
        {
            ClearAllUsers();
        }

        private void ClearAllUsers()
        {
            if (v.CihazIsActive)
            {
                fP.sta_ClearAllUsers(selCihazId, ref globalLog);
                myViewLog(globalLog);
            }
            else
            {
                myViewLog(Sql.cihazTestUserClearUsersSql(selCihazId));
            }
        }

        private void btnClearAllData_Click(object sender, EventArgs e)
        {
            ClearAllData();
        }

        private void ClearAllData()
        {
            if (v.CihazIsActive)
            {
                fP.sta_ClearAllData(selCihazId, ref globalLog);
                myViewLog(globalLog);
            }
            else
            {
                Sql.cihazTestAllClearSql(selCihazId, ref globalLog);
                myViewLog(globalLog);
            }
        }

        #endregion Cihazda Kayıt Silme işlemleri

        #region Kayıt Cihazı İşlemler

        private void btnSetYeniUser_Click(object sender, EventArgs e)
        {
            tSetNewUser();
        }

        private void btnGetYeniUserFP_Click(object sender, EventArgs e)
        {
            tGetNewUserFP();
        }

        private void btnDelYeniUser_Click(object sender, EventArgs e)
        {
            tDelUserFPLog();
        }

        private bool tDelUserFPLog()
        {
            bool onay = false;
            if (fP.manuelTalepRunStart() == false) return onay;
            onay =
            fP.talepRun(v.dsCihazList,
                        v.cihazCalismaTipi.Kayit,
                        v.dsUserInfo,
                        v.cihazTalepTipi.chDelUserFPLog,
                        null);

            tViewData(v.dsUserInfo);
            v.manuelTalepRun = false;
            return onay;
        }

        private bool tSetNewUser()
        {
            bool onay = false;
            if (fP.manuelTalepRunStart() == false) return onay;
            CihazLogGet(v.cihazTalepTipi.chSetNewUser, v.dsCihazLogDetay);
            onay = sendTable(v.dsCihazLogDetay, v.cihazTalepTipi.chSetNewUser);
            v.manuelTalepRun = false;
            return onay;
        }
        private bool tGetNewUserFP()
        {
            bool onay = false;
            if (fP.manuelTalepRunStart() == false) return onay;
            onay = 
            fP.talepRun(v.dsCihazList,
                        v.cihazCalismaTipi.Kayit,
                        v.dsUserInfo,
                        v.cihazTalepTipi.chGetNewUserFP,
                        null);

            tViewData(v.dsUserInfo);
            v.manuelTalepRun = false;
            return onay;
        }
        private bool tSetNewUserSayim()
        {
            // kullanıcı talebi
            // yeni eklenen kullanıcıları sayım cihazlarına ekle
            CihazLogGet(v.cihazTalepTipi.chSetNewUserSayim, v.dsCihazLogDetay);
            return sendTable(v.dsCihazLogDetay, v.cihazTalepTipi.chSetSayim);
        }


        private bool tSetOldUser()
        {
            bool onay = false;
            if (fP.manuelTalepRunStart() == false) return onay;
            CihazLogGet(v.cihazTalepTipi.chSetOldUser, v.dsCihazLogDetay);
            onay = sendTable(v.dsCihazLogDetay, v.cihazTalepTipi.chSetOldUser);
            v.manuelTalepRun = false;
            return onay;
        }
        private bool tGetOldUserFP()
        {
            bool onay = false;
            if (fP.manuelTalepRunStart() == false) return onay;
            onay =
            fP.talepRun(v.dsCihazList,
                        v.cihazCalismaTipi.Kayit,
                        v.dsUserInfo,
                        v.cihazTalepTipi.chGetOldUserFP,
                        null);

            tViewData(v.dsUserInfo);
            v.manuelTalepRun = false;
            return onay;
        }
        private bool tSetOldUserSayim()
        {
            // kullanıcı talebi
            // eski kullanıcıların yeni eklenen bio izlerini sayım cihazlarına ekle
            CihazLogGet(v.cihazTalepTipi.chSetOldUserSayim, v.dsCihazLogDetay);
            return sendTable(v.dsCihazLogDetay, v.cihazTalepTipi.chSetSayim);
        }

        #endregion Kayıt Cihazı İşlemler

        #region Auto işlemlerin kullanıcı tarafından başlatılması

        private void btnSetYeni_Click(object sender, EventArgs e)
        {
            if (fP.manuelTalepRunStart() == false) return;
            tSetSayim();
            v.manuelTalepRun = false;
        }
        private void btnSetTahliye_Click(object sender, EventArgs e)
        {
            if (fP.manuelTalepRunStart() == false) return;
            tSetTahliye();
            v.manuelTalepRun = false;
        }
        private void btnSetGorev_Click(object sender, EventArgs e)
        {
            if (fP.manuelTalepRunStart() == false) return;
            tSetGorev();
            v.manuelTalepRun = false;
        }
        private void btnSetGorus_Click(object sender, EventArgs e)
        {
            if (fP.manuelTalepRunStart() == false) return;
            tSetGorus();
            v.manuelTalepRun = false;
        }
        private void btnGetGorev_Click(object sender, EventArgs e)
        {
            if (fP.manuelTalepRunStart() == false) return;
            tGetGorev();
            v.manuelTalepRun = false;
        }
        private void btnGetGorus_Click(object sender, EventArgs e)
        {
            if (fP.manuelTalepRunStart() == false) return;
            tGetGorus();
            v.manuelTalepRun = false;
        }
        private void btnGetSayim_Click(object sender, EventArgs e)
        {
            if (fP.manuelTalepRunStart() == false) return;
            tGetSayim();
            v.manuelTalepRun = false;
        }
        private void btnDelGorev_Click(object sender, EventArgs e)
        {
            if (fP.manuelTalepRunStart() == false) return;
            tDelGorev();
            v.manuelTalepRun = false;
        }
        private void btnDelGorus_Click(object sender, EventArgs e)
        {
            if (fP.manuelTalepRunStart() == false) return;
            tDelGorus();
            v.manuelTalepRun = false;
        }
        private void btnDelTahliye_Click(object sender, EventArgs e)
        {
            if (fP.manuelTalepRunStart() == false) return;
            tDelTahliye();
            v.manuelTalepRun = false;
        }
        
        #endregion Auto işlemlerin kullanıcı tarafından başlatılması
        
        #region AutoTalep İşlemler
        private bool tSetSayim()
        {
            // yeni eklenen kullanıcıları sayım cihazlarına ekle
            CihazLogGet(v.cihazTalepTipi.chSetSayim, v.dsCihazLogDetay);
            return sendTable(v.dsCihazLogDetay, v.cihazTalepTipi.chSetSayim);
        }
        private bool tSetTahliye()
        {
            CihazLogGet(v.cihazTalepTipi.chSetTahliye, v.dsCihazLogDetay);
            return sendTable(v.dsCihazLogDetay, v.cihazTalepTipi.chSetTahliye);
        }
        private bool tSetGorev()
        {
            CihazLogGet(v.cihazTalepTipi.chSetGorev, v.dsCihazLogDetay);
            return sendTable(v.dsCihazLogDetay, v.cihazTalepTipi.chSetGorev);
        }
        private bool tSetGorus()
        {
            CihazLogGet(v.cihazTalepTipi.chSetGorus, v.dsCihazLogDetay);
            return sendTable(v.dsCihazLogDetay, v.cihazTalepTipi.chSetGorus);
        }
        private bool tGetGorev()
        {
            bool onay = false;
            // görevlileri okunacak 
            CihazLogGet(v.cihazTalepTipi.chGetGorev, v.dsCihazLogDetay);

            // görevlilerin GC cihazlardaki parmak okutmalarını oku ve db ye kaydet
            onay = 
            fP.talepRun(v.dsCihazList,
                        v.cihazCalismaTipi.GirisCikis,
                        v.dsFingerLogData,
                        v.cihazTalepTipi.chGetGorev,
                        v.tIslemTarihi);

            dataGridView6.DataSource = null;
            dataGridView6.DataSource = v.dsFingerLogData.Tables[0];

            return onay;
        }
        private bool tGetGorus()
        {
            bool onay = false;

            CihazLogGet(v.cihazTalepTipi.chGetGorus, v.dsCihazLogDetay);

            onay = 
            fP.talepRun(v.dsCihazList,
                        v.cihazCalismaTipi.Gorus,
                        v.dsFingerLogData,
                        v.cihazTalepTipi.chGetGorus,
                        v.tIslemTarihi);

            dataGridView6.DataSource = null;
            dataGridView6.DataSource = v.dsFingerLogData.Tables[0];

            return onay;
        }
        private bool tGetSayim()
        {
            bool onay = false;
            CihazLogGet(v.cihazTalepTipi.chGetSayim, v.dsCihazLogDetay);

            onay = 
            fP.talepRun(v.dsCihazList,
                        v.cihazCalismaTipi.Sayim,
                        v.dsFingerLogData,
                        v.cihazTalepTipi.chGetSayim,
                        v.tIslemTarihi);

            dataGridView6.DataSource = null;
            dataGridView6.DataSource = v.dsFingerLogData.Tables[0];

            return onay;
        }
        private bool tDelGorev()
        {
            CihazLogGet(v.cihazTalepTipi.chDelGorev, v.dsCihazLogDetay);
            return sendTable(v.dsCihazLogDetay, v.cihazTalepTipi.chDelGorev);
            // test için
            //fP.tGetAllUserAndFPs(selCihazId, v.dsUserInfo);
            //sendTable(v.dsUserInfo, v.cihazTalepTipi.chDelGorev);
        }
        private bool tDelGorus()
        {
            CihazLogGet(v.cihazTalepTipi.chDelGorus, v.dsCihazLogDetay);
            return sendTable(v.dsCihazLogDetay, v.cihazTalepTipi.chDelGorus);
        }
        private bool tDelTahliye()
        {
            CihazLogGet(v.cihazTalepTipi.chDelTahliye, v.dsCihazLogDetay);
            return sendTable(v.dsCihazLogDetay, v.cihazTalepTipi.chDelTahliye);
        }

        #endregion AutoTalep İşlemler

        #region sendTable
        private bool sendTable(DataSet ds, v.cihazTalepTipi talep)
        {
            //if (v.dsCihazLogDetay.Tables.Count == 0) return;
            //if (v.dsCihazLogDetay.Tables[0].Rows.Count == 0) return;

            bool onay = false;

            // seçili cihaza gönder
            if (talep == v.cihazTalepTipi.chSetAllUserAndFPs)
                onay = fP.tBatch_SetAllUserFPInfo(selCihazId, ds, talep);
            else onay = fP.tBatch_SetEPUserFPInfo(ds, talep);

            // Icmali tekrar oku
            CihazLogGet(v.cihazTalepTipi.chIcmal, v.dsCihazLogIcmal1);

            return onay;
        }
        #endregion sendTable


        #region Grid Select 

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.Tag.ToString() == "1") return;

            if (dataGridView1.CurrentCell == null) return;

            DataGridViewRow row = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex];

            string param = row.Cells["Param"].Value.ToString();

            v.cihazTalepTipi talep = fP.talepNameGet(param);

            CihazLogGet(talep, v.dsCihazLogDetay);
        }
        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.Tag.ToString() == "1") return;
            getCihazValues();
        }
        private void getCihazValues()
        {
            if (dataGridView2.CurrentCell == null) return;

            DataGridViewRow row = dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex];

            selCihazId = Convert.ToInt32(row.Cells["CihazId"].Value.ToString());
            selCihazAdi = row.Cells["CihazAdi"].Value.ToString();
            selCihazIp = row.Cells["CihazIP"].Value.ToString();
            selCihazPort = row.Cells["CihazPort"].Value.ToString();
            selCihazCalismaTipiId = Convert.ToInt32(row.Cells["CalismaTipiId"].Value.ToString());

            labCihazAdi.Text = selCihazAdi;
            labCihazIP.Text = selCihazIp;
            labCihazPort.Text = selCihazPort;
        }





        #endregion Grid Select 

        #region CihazEmir leri
        private void timer_CihazEmir_Tick(object sender, EventArgs e)
        {
            if ((v.manuelTalepRun == false) &&
                (v.autoTalepRun == false))
                getCihazEmirList();
        }

        private void getCihazEmirList()
        {
            string tSqlYeni = Sql.CihazEmirYenilerSql(ref v.dsCihazEmirYeniList);
            string tSqlEski = Sql.CihazEmirEskilerSql(ref v.dsCihazEmirEskiList);

            try
            {
                if (v.dsCihazEmirYeniList.Tables.Count > 0)
                {
                    if (v.dsCihazEmirYeniList.Tables[0].Rows.Count > 0)
                        v.dsCihazEmirYeniList.Tables[0].Clear();
                }

                if (v.dsCihazEmirEskiList.Tables.Count > 0)
                {
                    if (v.dsCihazEmirEskiList.Tables[0].Rows.Count > 0)
                        v.dsCihazEmirEskiList.Tables[0].Clear();
                }

                t.Data_Read_Execute(v.dsCihazEmirYeniList, ref tSqlYeni, "CihazEmir", null);
                t.Data_Read_Execute(v.dsCihazEmirEskiList, ref tSqlEski, "CihazEmir", null);

                dataGridView4.DataSource = v.dsCihazEmirYeniList.Tables[0];
                dataGridView5.DataSource = v.dsCihazEmirEskiList.Tables[0];

                dataGridView4.Columns[0].Width = 50;
                dataGridView5.Columns[0].Width = 50;

                int yeniTalepCount = 0;
                yeniTalepCount = v.dsCihazEmirYeniList.Tables[0].Rows.Count;

                if (yeniTalepCount > 0)
                {
                    KullaniciTalepleriniUygula();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void KullaniciTalepleriniUygula()
        {
            if ((v.manuelTalepRun == true) ||
                (v.autoTalepRun == true)) return;

            bool sonuc = false;
            int talepId = 0;
            Int16 value = 0;
            
            listBox1.Items.Add("Kullanıcı talepleri uygulanacak ...");

            v.manuelTalepRun = true;

            foreach (DataRow row in v.dsCihazEmirYeniList.Tables[0].Rows)
            {
                talepId = t.myInt32(row["Id"].ToString());
                value = t.myInt16(row["TalepTipiId"].ToString());

                sonuc = TalepUygula(value);

                // işlem başarılı sonuçlanmışsa emirin işi yapıldı, emri sonlandır
                if (sonuc)
                    Sql.prc_CihazEmir(talepId);
            }

            v.manuelTalepRun = false;
        }

        private bool TalepUygula(Int16 value)
        {
            bool onay = false;
            
            if (value == (Int16)v.cihazTalepTipi.chSetNewUser) onay = tSetNewUser();
            if (value == (Int16)v.cihazTalepTipi.chGetNewUserFP) onay = tGetNewUserFP();
            if (value == (Int16)v.cihazTalepTipi.chSetNewUserSayim) onay = tSetNewUserSayim();

            if (value == (Int16)v.cihazTalepTipi.chSetOldUser) onay = tSetOldUser();
            if (value == (Int16)v.cihazTalepTipi.chGetOldUserFP) onay = tGetOldUserFP();
            if (value == (Int16)v.cihazTalepTipi.chSetOldUserSayim) onay = tSetOldUserSayim();


            if (value == (Int16)v.cihazTalepTipi.chSetSayim) onay = tSetSayim();
            if (value == (Int16)v.cihazTalepTipi.chSetTahliye) onay = tSetTahliye();
            if (value == (Int16)v.cihazTalepTipi.chSetGorev) onay = tSetGorev();
            if (value == (Int16)v.cihazTalepTipi.chSetGorus) onay = tSetGorus();
            if (value == (Int16)v.cihazTalepTipi.chGetGorev) onay = tGetGorev();
            if (value == (Int16)v.cihazTalepTipi.chGetGorus) onay = tGetGorus();
            if (value == (Int16)v.cihazTalepTipi.chGetSayim) onay = tGetSayim();
            if (value == (Int16)v.cihazTalepTipi.chDelGorev) onay = tDelGorev();
            if (value == (Int16)v.cihazTalepTipi.chDelGorus) onay = tDelGorus();
            if (value == (Int16)v.cihazTalepTipi.chDelTahliye) onay = tDelTahliye();
            
            return onay;
        }

        #endregion

        #region AutoTalepRun yönetimi
        private void timer_Icmal1_Tick(object sender, EventArgs e)
        {
            if ((v.manuelTalepRun == false) &&
                (v.autoTalepRun == false))
                icmal1Read();
        }

        private void icmal1Read()
        {
            CihazLogGet(v.cihazTalepTipi.chIcmal, v.dsCihazLogIcmal1);

            icmal1TalepCount = getTalepCount(v.dsCihazLogIcmal1);

            // eğer yapılacak iş var ikinci sayımı başlat 
            if ((icmal1TalepCount > 0) && (icmal1TalepCount != icmal2TalepCount))
                timer_Icmal2.Enabled = true;
                        
            listBox1.Items.Add("Icmal 1 okundu ...");
        }

        private void timer_Icmal2_Tick(object sender, EventArgs e)
        {
            if ((v.manuelTalepRun == false) &&
                (v.autoTalepRun == false))
                icmal2Read();
        }

        private void icmal2Read()
        {
          
            CihazLogGet(v.cihazTalepTipi.chIcmal, v.dsCihazLogIcmal2);

            icmal2TalepCount = getTalepCount(v.dsCihazLogIcmal2);

            /// eğer 1. okuma ve 2. okuma eşit değilse
            /// kullanıcı tarafında yapılan işlemler var anlamına geliyor
            /// kullanıcı işlemlerine halen devam ediyor bitirmesini beklemek gerekiyor
            /// 
            
            //if (icmal1TalepCount == 0) -- test koşulu
            if ((icmal1TalepCount > 0) && (icmal1TalepCount != icmal2TalepCount))
            {
                // timer1 kapat geri aç yani sayımını sıfırla,
                // yeniden saymaya başlasın
                timer_Icmal1.Enabled = false;
                timer_Icmal1.Enabled = true;
                // timer2 yide kapat
                timer_Icmal2.Enabled = false;
            }

            listBox1.Items.Add("Icmal 2 okundu ...");

            // birinci ve ikinci okuma arasında fark yoksa auto işlemler başlayabilir
            
            //if (icmal1TalepCount == 0) -- test koşulu
            if ((icmal1TalepCount > 0) && (icmal1TalepCount == icmal2TalepCount))
            {
                autoTalepRun(v.dsCihazLogIcmal1);
            }
        }

        private void autoTalepRun(DataSet ds)
        {
            /// tam auto işlemler başlayacak sıra kullanıcı manuel işlem başlatabilir
            /// o zamanda auto işlem iptal, bir sonraki kontrole kadar auto işlem beklesin
            ///
            if (v.manuelTalepRun) return;

            listBox1.Items.Add(":: Otomatik talepler çalıştı ...");
            
            v.autoTalepRun = true;

            string param = "";
            v.cihazTalepTipi talep = v.cihazTalepTipi.chNull;


            foreach (DataRow row in ds.Tables[0].Rows)
            {
                param = row["param"].ToString();
                talep = fP.talepNameGet(param);

                //if (talep != v.cihazTalepTipi.chGetSayim)
                //    TalepUygula((Int16)talep);
            }

            timer_Icmal1.Enabled = false;
            timer_Icmal1.Enabled = true;

            timer_Icmal2.Enabled = false;

            listBox1.Items.Add(":: Otomatik talepler bitti ................... ");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            listBoxClear();

            icmal2TalepCount = 0;
            v.autoTalepRun = false;
        }

        private void listBoxClear()
        {
            int count = 0;

            if (listBox1.Items.Count > 100) count = listBox1.Items.Count - 10;
            
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    listBox1.Items.RemoveAt(0);
                }
            }
        }

        private int getTalepCount(DataSet ds)
        {
            int count = 0;

            string param = "";
            int adet = 0;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                param = row["param"].ToString();
                if (param != "GetSayim")
                {
                    adet = t.myInt32(row["Adet"].ToString());
                    count = count + adet;
                }
            }

            return count;
        }

        #endregion AutoTalepRun
    }

}
