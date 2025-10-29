using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Tkn_Events;
using Tkn_ToolBox;
using Tkn_Save;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_MtskSinavRandevu : Form
    {
        #region Tanımlar

        tToolBox t = new tToolBox();
        //tSave sv = new tSave();

        string TableIPCode = string.Empty;
        string TableIPCode_RandevuFormu = string.Empty;

        DataSet ds_Adaylar = null;
        DataNavigator dN_Adaylar = null;
        DataSet ds_RandevuFormu = null;
        DataNavigator dN_RandevuFormu = null;

        #endregion Tanımlar
        public ms_MtskSinavRandevu()
        {
            InitializeComponent();
        }

        private void ms_MtskSinavRandevu_Shown(object sender, EventArgs e)
        {
            // Randevu Formu
            TableIPCode = "UST/MEB/MtskSinavRandevu.YeniForm_L01";
            TableIPCode_RandevuFormu = TableIPCode;
            t.Find_DataSet(this, ref ds_RandevuFormu, ref dN_RandevuFormu, TableIPCode);


            // Sırayla Randevu Formuna Ekle
            TableIPCode = "UST/MEB/MtskAdayTakip.uSinavDurumRandevuPlanla_L01";
            t.Find_DataSet(this, ref ds_Adaylar, ref dN_Adaylar, TableIPCode);

            Control cntrl = null;
            string[] controls = new string[] { };
            cntrl = null;
            cntrl = t.Find_Control(this, "simpleButton_ek2", TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_SiraylaRandevuFormunaEkle);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("KAYDET16");
            }

        }

        private void btn_SiraylaRandevuFormunaEkle(object sender, EventArgs e)
        {
            if (planOncesiDataKontrol() == false) return;

            AdaylariSiraylaPlanlamayaEkle();
        }

        private void AdaylariSiraylaPlanlamayaEkle()
        {
            string idFName = "AdayId";
            string adayTcNoFName = "Lkp_AdayTcNo";
            string adayAdiFName = "Lkp_AdayTamAdiSoyadi";
            int adayId = 0;
            string adayTcNo = "";
            string adayAdiSoyadi = "";

            string targetTarih = "Tarih";
            string sourceTarih = "Lkp_SinavTarihi";
            string targetEgitimAraci = "EgitimAraciId";
            string sourceEgitimAraci = "Lkp_PlakaNumarasi";
            string targetPersonelId = "PersonelId";
            string sourcePersonelId = "Lkp_PersonelId";

            int randevuAdayCount = ds_RandevuFormu.Tables[0].Rows.Count;
            string id = "";

            if (dN_Adaylar.Position == -1)
            {
                MessageBox.Show("Aday bulunamadı...");
                return;
            }
            dN_Adaylar.Position = 0;

            for (int i = 0; i < randevuAdayCount; i++)
            {
                id = ds_RandevuFormu.Tables[0].Rows[i][idFName].ToString();

                if (id == "")
                {
                    adayId = t.myInt32(ds_Adaylar.Tables[0].Rows[dN_Adaylar.Position]["AdayId"].ToString());
                    adayTcNo = ds_Adaylar.Tables[0].Rows[dN_Adaylar.Position]["Lkp_AdayTcNo"].ToString();
                    adayAdiSoyadi = ds_Adaylar.Tables[0].Rows[dN_Adaylar.Position]["Lkp_AdayTamAdiSoyadi"].ToString();

                    if (idFName != "")
                    {
                        ds_RandevuFormu.Tables[0].Rows[i][idFName] = adayId;
                        ds_RandevuFormu.Tables[0].Rows[i][adayTcNoFName] = adayTcNo;
                        ds_RandevuFormu.Tables[0].Rows[i][adayAdiFName] = adayAdiSoyadi;

                        ds_RandevuFormu.Tables[0].Rows[i][targetTarih] = ds_RandevuFormu.Tables[0].Rows[i][sourceTarih];
                        ds_RandevuFormu.Tables[0].Rows[i][targetEgitimAraci] = ds_RandevuFormu.Tables[0].Rows[i][sourceEgitimAraci];
                        ds_RandevuFormu.Tables[0].Rows[i][targetPersonelId] = ds_RandevuFormu.Tables[0].Rows[i][sourcePersonelId];

                        dN_Adaylar.Tag = dN_Adaylar.Position;
                        //NavigatorButton btnEndEdit = dN_Adaylar.Buttons.EndEdit;
                        //dN_Adaylar.Buttons.DoClick(btnEndEdit);

                        v.tButtonHint.Clear();
                        v.tButtonHint.tForm = this;
                        v.tButtonHint.tableIPCode = TableIPCode_RandevuFormu;
                        v.tButtonHint.buttonType = v.tButtonType.btKaydet;

                        tEventsButton evb = new tEventsButton();
                        evb.btnClick(v.tButtonHint);

                        //ds_RandevuFormu.Tables[0].AcceptChanges();

                        if (ds_Adaylar.Tables[0].Rows.Count-1 == dN_Adaylar.Position) break;

                        dN_Adaylar.Position++;
                        dN_RandevuFormu.Position++;
                    }
                }
            }

            ListeleriRefreshle();
        }

        private void ListeleriRefreshle()
        {
            DataSet ds = null;
            TableIPCode = "UST/MEB/MtskSinavRandevu.Liste_L01";
            ds = t.Find_DataSet(this, "", TableIPCode, "");
            Refresh_(ds);
            TableIPCode = "UST/MEB/MtskAdayTakip.uSinavDurumRandevuPlanlaSertikalar_T01";
            ds = t.Find_DataSet(this, "", TableIPCode, "");
            Refresh_(ds);
            TableIPCode = "UST/MEB/MtskAdayTakip.uSinavDurumRandevuPlanla_L01";
            ds = t.Find_DataSet(this, "", TableIPCode, "");
            Refresh_(ds);
        }

        private void Refresh_(DataSet ds)
        {
            if (ds != null)
            {
                /// sadece kendisi refresh olsun 
                /// kendisine bağlı olanlar refresh olmasın
                /// dataNavigator_PositionChanged( 
                ///
                v.con_Cancel = true;
                t.TableRefresh(this, ds);//, TABLEIPCODE);
            }
        }

        private bool planOncesiDataKontrol()
        {
            bool onay = true;

            if (t.IsNotNull(ds_Adaylar) == false)
            {
                MessageBox.Show("Dikkat : Aday Liste bulunumadı ...");
                onay = false;
            }

            return onay;
        }

    }
}
