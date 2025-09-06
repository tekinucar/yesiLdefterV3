using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tkn_DevView;
using Tkn_Events;
using Tkn_ToolBox;
using Tkn_Variable;
using Tkn_InputPanel;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraEditors.Repository;
using Tkn_TablesRead;
using Tkn_DevColumn;
using Tkn_Save;

namespace YesiLdefter
{
    public partial class ms_MtskDersProgrami : Form
    {
        #region Tanımlar

        tToolBox t = new tToolBox();
        tInputPanel ip = new tInputPanel();
        tDevView dv = new tDevView();
        tEvents ev = new tEvents();
        tTablesRead tr = new tTablesRead();
        tDevColumn dc = new tDevColumn();
        tSave sv = new tSave();

        public enum formTipi 
        {
            SablonHazirlamaFormu, // SablonFormu
            PlanlamaFormu     // planlamaFormu
        };

        public enum planTipi
        {
            teorik,
            uygulama
        };

        public enum gunTipi : byte
        {
            none = 0,
            _5IsGunu2GunIzin = 1,
            _6IsGunu1GunIzin = 2
        };

        public enum saatTipi : byte
        {
            none = 0,
            tekSaat = 1,
            _7_8_ciftSaat = 2,
            _8_9_ciftSaat = 3
        }

        public enum aracTipi : byte
        {
            none = 0,
            simülatorArac = 1,
            sadeceArac = 2
        }

        public enum ogreticiTipiId : byte
        {
            none = 0,
            _1_OgreticiUsta = 1,
            _2_OgreticiUsta = 2
        }

        public enum adayTipiId : byte
        {
            none = 0,
            _4_Aday = 4,
            _5_Aday = 5,
            _6_Aday = 6,
            _7_Aday = 7,
            _8_Aday = 8
        }

        //-- Teorik
        public class MtskSablonTeorikB
        {
            public int Id { get; set; }
            public string SablonAdi { get; set; }
            public gunTipi gunTipiId { get; set; }
            public Int16 toplamIsGunuSayisi { get; set; }
            public saatTipi saatTipiId { get; set; }
            public Int16 toplamDersSaati { get; set; }
            public Int16 gundeMaxSaat { get; set; }
            public Int16 haftadaMaxSaat { get; set; }

            public void Clear()
            {
                Id = 0;
                SablonAdi = "";
                gunTipiId = gunTipi.none;
                toplamIsGunuSayisi = 0;
                saatTipiId = saatTipi.none;
                toplamDersSaati = 0;
                gundeMaxSaat = 0;
                haftadaMaxSaat = 0;
            }
        }

        public class MtskSablonTeorikS
        {
            public int pos { get; set; }
            public int Id { get; set; }
            public int SablonTeorikBId { get; set; }
            public bool IsActive { get; set; }
            public Int16 GunId { get; set; }
            public Int16 SaatTipiId { get; set; }
            public Int16 GrupTipiId { get; set; }
            public Int16 SubeTipiId { get; set; }
            public Int16 DerslikTipiId { get; set; }
            public Int16 DerslikAdiTipiId { get; set; }
            public Int16 DersSaatiTipiId { get; set; }
            public Int16 EgitimTipiId { get; set; }

            public void Clear()
            {
                pos = 0;
                Id = 0;
                SablonTeorikBId = 0;
                IsActive = false;
                GunId = 0;
                SaatTipiId = 0;
                GrupTipiId = 0;
                SubeTipiId = 0;
                DerslikTipiId = 0;
                DerslikAdiTipiId = 0;
                DersSaatiTipiId = 0;
                EgitimTipiId = 0;
            }
        }


        //-- Uygulama

        public class MtskSablonUygulamaB
        {
            public int Id { get; set; }
            public string SablonAdi { get; set; }
            public gunTipi gunTipiId { get; set; }
            public Int16 toplamIsGunuSayisi { get; set; }
            public saatTipi saatTipiId { get; set; }
            public Int16 MevcutSertifikaTipiId { get; set; }
            public Int16 IstenenSertifikaTipiId { get; set; }
            public Int16 toplamDersSaati { get; set; }
            public aracTipi aracTipiId { get; set; }
            public ogreticiTipiId ogreticiTipiId { get; set; }
            public adayTipiId adayTipiId { get; set; }
            public Int16 gundeMaxSaat { get; set; }
            public Int16 haftadaMaxSaat { get; set; }

            public void Clear()
            {
                Id = 0;
                SablonAdi = "";
                gunTipiId = gunTipi.none;
                toplamIsGunuSayisi = 0;
                saatTipiId = saatTipi.none;
                MevcutSertifikaTipiId = 0;
                IstenenSertifikaTipiId = 0;
                toplamDersSaati = 0;
                aracTipiId = aracTipi.none;
                ogreticiTipiId = ogreticiTipiId.none;
                adayTipiId = adayTipiId.none;
                gundeMaxSaat = 0;
                haftadaMaxSaat = 0;
        }
        }

        public class MtskSablonUygulamaS
        {
            public int pos { get; set; }
            public int Id { get; set; }
            public int SablonUygulamaBId { get; set; }
            public bool IsActive { get; set; }
            public Int16 GunId { get; set; }
            public Int16 SaatTipiId { get; set; }
            public Int16 AracTipiId { get; set; }
            public Int16 OgreticiTipiId { get; set; }
            public Int16 AdayTipiId { get; set; }

            public void Clear()
            {
                pos = 0;
                Id = 0;
                SablonUygulamaBId = 0;
                IsActive = false;
                GunId = 0;
                SaatTipiId = 0;
                AracTipiId = 0;
                OgreticiTipiId = 0;
                AdayTipiId = 0;
            }
        }
        

        bool IsPreparing = false;
        string _dBaseNo = "";
        string TableIPCode = string.Empty;
        string teoSablonHazirlamaTableIPCode = "UST/MEB/MtskSablonTeorikB.Planlama_L01";
        string teoPlanlamaTableIPCode = "UST/MEB/MtskSablonTeorikB.TeoPlanlama_L01";
        string uygSablonHazirlamaTableIPCode = "UST/MEB/MtskSablonUygulamaB.Planlama_L01";
        string uygPlanlamaTableIPCode = "UST/MEB/MtskSablonUygulamaB.Planlama_L02";
        string buttonInsertPaketOlustur = "ButtonPaketOlustur";

        string menuNameTeorik = "MENU_" + "UST/MEB/MTS/SablonTeorik";
        string menuNameUygulama = "MENU_" + "UST/MEB/MTS/SablonUygulama";

        string cumleBaslik = "";
        string cumleSatirlar = "";

        string TabControlName = string.Empty;
        Control tabControl = null;
        AdvBandedGridView tGridView = null;

        // TasarımFormu
        DataSet ds_Fields = null;

        DataSet ds_SablonB = null;
        DataNavigator dN_SablonB = null;

        DataSet ds_SablonS = null;
        DataNavigator dN_SablonS = null;

        DataSet ds_SablonSSave = null;
        DataNavigator dN_SablonSSave = null;

        //PlanlamaFormu
        DataSet ds_UygPlanYapilacakAdaylar = null;
        DataNavigator dN_UygPlanYapilacakAdaylar = null;

        DataSet ds_MtskSablonUygulamaBPlanlama = null;
        DataNavigator dN_MtskSablonUygulamaBPlanlama = null;

        DataSet ds_MtskSablonUygulamaFKayit_F01 = null;
        DataNavigator dN_MtskSablonUygulamaFKayit_F01 = null;

        DataSet ds_MtskUygulamaliDersPlan_L01 = null;
        DataNavigator dN_MtskUygulamaliDersPlan_L01 = null;

        MtskSablonTeorikB teoSablonB = new MtskSablonTeorikB();
        MtskSablonTeorikS teoTaskakS = new MtskSablonTeorikS();

        MtskSablonUygulamaB uygSablonB = new MtskSablonUygulamaB();
        MtskSablonUygulamaS uygTaskakS = new MtskSablonUygulamaS();

        formTipi _formTipi;
        planTipi _planTipi;

        #endregion Tanımlar

        public ms_MtskDersProgrami()
        {
            InitializeComponent();
        }

        private void ms_MtskDersProgrami_Shown(object sender, EventArgs e)
        {
            

            // Teorik Sablon Hazırlama Fonksiyonlar
            //
            //TableIPCode = "UST/MEB/MtskSablonTeorikB.Planlama_L01";
            t.Find_DataSet(this, ref ds_SablonB, ref dN_SablonB, teoSablonHazirlamaTableIPCode);

            if (ds_SablonB != null)
            {
                string myProp = ds_SablonB.Namespace;
                _dBaseNo = t.MyProperties_Get(myProp, "DBaseNo:");

                _planTipi = planTipi.teorik;

                if (ds_SablonB.DataSetName == teoSablonHazirlamaTableIPCode)
                    preparingTeorikSablonHazirlamaFormu();

                t.Find_Button_AddClick(this, menuNameTeorik, buttonInsertPaketOlustur, myNavElementClick);

                return;
            }

            // Teorik Planlama Fonksiyonlar
            //
            //TableIPCode = "UST/MEB/MtskSablonTeorikB.Planlama_L02";
            t.Find_DataSet(this, ref ds_SablonB, ref dN_SablonB, teoPlanlamaTableIPCode);

            if (ds_SablonB != null)
            {
                string myProp = ds_SablonB.Namespace;
                _dBaseNo = t.MyProperties_Get(myProp, "DBaseNo:");

                _planTipi = planTipi.teorik;

                if (ds_SablonB.DataSetName == teoPlanlamaTableIPCode)
                    preparingPlanTeorikFormu();
                
                return;
            }


            // Uygulama Sablon Hazırlama Fonksiyonları
            //
            //TableIPCode = "UST/MEB/MtskSablonUygulamaB.Planlama_L01";
            t.Find_DataSet(this, ref ds_SablonB, ref dN_SablonB, uygSablonHazirlamaTableIPCode);

            if (ds_SablonB != null)
            {
                string myProp = ds_SablonB.Namespace;
                _dBaseNo = t.MyProperties_Get(myProp, "DBaseNo:");

                _planTipi = planTipi.uygulama;

                if (ds_SablonB.DataSetName == uygSablonHazirlamaTableIPCode)
                    preparingUygulamaSablonHazirlamaFormu();

                t.Find_Button_AddClick(this, menuNameUygulama, buttonInsertPaketOlustur, myNavElementClick);

                return;
            }

            // Uygulama Planlama Fonksiyonlar
            //             
            //TableIPCode = "UST/MEB/MtskSablonUygulamaB.Planlama_L02";
            t.Find_DataSet(this, ref ds_SablonB, ref dN_SablonB, uygPlanlamaTableIPCode);

            if (ds_SablonB != null)
            {
                string myProp = ds_SablonB.Namespace;
                _dBaseNo = t.MyProperties_Get(myProp, "DBaseNo:");

                _planTipi = planTipi.uygulama;

                if (ds_SablonB.DataSetName == uygPlanlamaTableIPCode)
                    preparingPlanlamaFormu();

                return;
            }

        }

        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonInsertPaketOlustur) InsertPaketOlustur();
                //if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonPaketiGonder) PaketiGonder();
            }
        }

        /// Planlama Formu
        /// 

        #region TeorikPlanlamaFormu
        private void preparingPlanTeorikFormu()
        {
            this._formTipi = formTipi.PlanlamaFormu;

            dN_SablonB.PositionChanged += new System.EventHandler(dataNavigatorB_PositionChanged);

            // Sablon Göster
            //TableIPCode = "UST/MEB/MtskSablonTeorikB.Planlama_L02";
            Control cntrl = null;
            string[] controls = new string[] { };
            cntrl = t.Find_Control(this, "simpleButton_ek1", teoPlanlamaTableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_planlamaFormuTeorikSablonGoster);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("KAYDET16");
            }


            /*
            // Planı Oluştur
            TableIPCode = "UST/MEB/MtskSablonTeorikF.Planlar_L01";
            //t.Find_DataSet(this, ref ds_MtskUygulamaliDersPlan_L01, ref dN_MtskUygulamaliDersPlan_L01, TableIPCode);

            cntrl = null;
            cntrl = t.Find_Control(this, "simpleButton_ek2", TableIPCode, controls);

            if (cntrl != null)
            {
                //((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_planlamaFormuTeorikPlaniOlustur);
                //((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("KAYDET16");
            }
            */

            TableIPCode = "UST/MEB/MtskSablonTeorikS.Planlama_F01";
            this.ds_Fields = new DataSet();
            tr.MS_Fields_IP_Read(this.ds_Fields, TableIPCode);
        }

        private void btn_planlamaFormuTeorikSablonGoster(object sender, EventArgs e)
        {
            //
            TabControlName = "tabControl_SUBVIEW";
            tabControl = null;
            tabControl = t.Find_Control(this, TabControlName);

            getSablonTeorik(ds_SablonB, dN_SablonB);
            preparingSablon(teoSablonB.Id, teoSablonB.SablonAdi);
        }

        private void btn_planlamaFormuTeorikPlaniOlustur(object sender, EventArgs e)
        {
            //if (planOncesiDataKontrol() == false) return;

            //if (planOncesiPlanlamaFormuKontolu() == false) return;
            //MessageBox.Show("ok");

            /// Dashboard
            /// Aday İşlemleri
            ///    Yeni Aday Kaydı
            ///    Adaylar Listesi
            ///    Adayların Mebbis İşlemleri
            /// Dönem İşlemleri
            ///    Dönem Açılışları    
            ///    Teorik Dersler Planlama
            ///    Teorik Sınavlar
            /// Uygulamalı Dersler
            ///    Uygulama Dersleri Planlama
            ///    Uygulama Sınavları
            ///    Başarısız Adaylar
            /// Sertifikalar
            /// 


            /// 
            /// 1. gelecek dönem için alınan aday kayıtları
            /// 2. dönem başlarken belge kontrolleri
            /// 3. dönem açılış işlemleri - dönem açılışları, teorik ders planlama ve mebbise bildirimi
            /// 4. devam eden dönem - derslerin takibi
            /// 5. e-sınav başvuruları
            /// 6. e-sınav sonuçları
            /// 7. uygulama ders planlama ve mebbis kaydı
            /// 8. uygulama ders planları - takipleri
            /// 9. uygulama ders sınav başvuruları
            /// 10. uygulama sınav sonuçları alma
            /// 11. başarısız aday işlemleri
            /// 12. sertifika hazırlama
            /// 

            /// Mali İşlemler
            /// 1. adayları borçlandırma ve bilgilendirme
            /// 2. alacakların takibi
            /// 3. sınav ücretlerinin planlanması ve takibi
            /// 4. işletme giderlerin kaydı
            /// 5. personel ücretlerinin planlaması ve işlenmesi
            /// 6. Mali müşavir için dosya hazırlamak
            /// 7. 


        }

        #endregion TeorikPlanlamaFormu

        #region UygulamaPlanlamaFormu

        private void preparingPlanlamaFormu()
        {
            this._formTipi = formTipi.PlanlamaFormu;

            dN_SablonB.PositionChanged += new System.EventHandler(dataNavigatorB_PositionChanged);

            // Sablon Göster
            //TableIPCode = "UST/MEB/MtskSablonUygulamaB.Planlama_L02";
            Control cntrl = null;
            string[] controls = new string[] { };
            cntrl = t.Find_Control(this, "simpleButton_ek1", uygPlanlamaTableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_planlamaFormuUygulamaSablonGoster);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("KAYDET16");
            }

            // Sırayla Planlamaya Ekle
            TableIPCode = "UST/MEB/MtskAdayTakip.UygPlanYapilacakAdaylar_L02";
            t.Find_DataSet(this, ref ds_UygPlanYapilacakAdaylar, ref dN_UygPlanYapilacakAdaylar, TableIPCode);

            cntrl = null;
            cntrl = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_planlamaFormuSiraylaPlanlamayaEkle);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("KAYDET16");
            }

            // Sablon      
            //TableIPCode = "UST/MEB/MtskSablonUygulamaB.Planlama_L02";
            t.Find_DataSet(this, ref ds_MtskSablonUygulamaBPlanlama, ref dN_MtskSablonUygulamaBPlanlama, uygPlanlamaTableIPCode);
            if (ds_MtskSablonUygulamaBPlanlama != null)
            {
                dN_MtskSablonUygulamaBPlanlama.PositionChanged += new System.EventHandler(dN_MtskSablonUygulamaBPlanlama_PositionChanged);
            }

            // Planlama Formu
            TableIPCode = "UST/MEB/MtskSablonUygulamaF.Kayit_F01";
            t.Find_DataSet(this, ref ds_MtskSablonUygulamaFKayit_F01, ref dN_MtskSablonUygulamaFKayit_F01, TableIPCode);

            // Planı Oluştur
            TableIPCode = "UST/MEB/MtskSablonUygulamaF.Kayit_F01";// "UST/MEB/MtskUygulamaliDers.Plan_L01";
            t.Find_DataSet(this, ref ds_MtskUygulamaliDersPlan_L01, ref dN_MtskUygulamaliDersPlan_L01, TableIPCode);

            cntrl = null;
            cntrl = t.Find_Control(this, "simpleButton_ek2", TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_planlamaFormuUygulamaPlaniOlustur);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("KAYDET16");
            }

            TableIPCode = "UST/MEB/MtskSablonUygulamaS.Planlama_F01";
            this.ds_Fields = new DataSet();
            tr.MS_Fields_IP_Read(this.ds_Fields, TableIPCode);

        }

        private void btn_planlamaFormuUygulamaSablonGoster(object sender, EventArgs e)
        {
            //
            TabControlName = "tabControl_SUBVIEW";
            tabControl = null;
            tabControl = t.Find_Control(this, TabControlName);

            getSablonUygulama(ds_SablonB, dN_SablonB);
            preparingSablon(uygSablonB.Id, uygSablonB.SablonAdi);
        }

        private void dN_MtskSablonUygulamaBPlanlama_PositionChanged(object sender, EventArgs e)
        {
            getSablonUygulama(ds_MtskSablonUygulamaBPlanlama, dN_MtskSablonUygulamaBPlanlama);
        }
        
        private void btn_planlamaFormuSiraylaPlanlamayaEkle(object sender, EventArgs e)
        {
            if (planOncesiDataKontrol() == false) return;

            AdaylariSiraylaPlanlamayaEkle();
        }

        private void AdaylariSiraylaPlanlamayaEkle()
        {
            // Aday Listesi
            // ds_MtskAdayUDersListe_02 
            // dN_MtskAdayUDersListe_02

            // Sablon
            // ds_MtskSablonUygulamaBPlanlama
            // dN_MtskSablonUygulamaBPlanlama

            // Planlama Formu
            // ds_MtskSablonUygulamaFKayit_F01
            // dN_MtskSablonUygulamaFKayit_F01

            // Planı Oluştur, Ders Planı Sonucu
            // ds_MtskUygulamaliDersPlan_L01
            // dN_MtskUygulamaliDersPlan_L01

            Int16 AdaySayisi = (Int16)uygSablonB.adayTipiId;

            //MessageBox.Show(AdaySayisi.ToString());

            string idFieldName = "";
            string captionFieldName = "";
            int adayId = 0;
            string adayAdiSoyadi = "";
            int uygDersToplami = 0;
            int count = ds_UygPlanYapilacakAdaylar.Tables[0].Rows.Count;
            for (int i = 0; i < count; i++)
            {
                 adayId = t.myInt32(ds_UygPlanYapilacakAdaylar.Tables[0].Rows[i]["AdayId"].ToString());
                 adayAdiSoyadi = ds_UygPlanYapilacakAdaylar.Tables[0].Rows[i]["Lkp_AdayTamAdiSoyadi"].ToString();
                 uygDersToplami = t.myInt32(ds_UygPlanYapilacakAdaylar.Tables[0].Rows[i]["UygulamaliDersToplami"].ToString());

                 //if (uygDersToplami == 0)
                 //{
                     getBosAdayFieldName(AdaySayisi, ref idFieldName, ref captionFieldName);
                     if (idFieldName != "")
                     {
                         ds_MtskSablonUygulamaFKayit_F01.Tables[0].Rows[dN_MtskSablonUygulamaFKayit_F01.Position][idFieldName] = adayId;
                         ds_MtskSablonUygulamaFKayit_F01.Tables[0].Rows[dN_MtskSablonUygulamaFKayit_F01.Position][captionFieldName] = adayAdiSoyadi;
                     }
                 //}
                
            }

            ds_MtskSablonUygulamaFKayit_F01.Tables[0].AcceptChanges();
        }

        private void getBosAdayFieldName(Int16 AdaySayisi, ref string idFieldName, ref string captionFieldName)
        {
            //Aday1Id Int           null,
            //Aday2Id Int           null,
            //Aday3Id Int           null,
            //Aday4Id Int           null,
            //Aday5Id Int           null,
            //Aday6Id Int           null,
            //Aday7Id Int           null,
            //Aday8Id Int           null,
            //Lkp_AdayAdiSoyadi1 nVarchar(100)  null,
            //...

            int pos = dN_MtskSablonUygulamaFKayit_F01.Position;
            //

            int i = 1;
            while (i < AdaySayisi + 1)
            {
                idFieldName = "Aday" + i.ToString() + "Id";

                if ((ds_MtskSablonUygulamaFKayit_F01.Tables[0].Rows[pos][idFieldName].ToString() == "") ||
                    (ds_MtskSablonUygulamaFKayit_F01.Tables[0].Rows[pos][idFieldName].ToString() == "0"))
                {
                    captionFieldName = "Lkp_AdayAdiSoyadi" + i.ToString();
                    return;
                }
                i++;
            }
            return;
        }
        
        private void btn_planlamaFormuUygulamaPlaniOlustur(object sender, EventArgs e)
        {
            if (planOncesiDataKontrol() == false) return;

            if (planOncesiPlanlamaFormuKontolu() == false) return;

        }

        private bool planOncesiDataKontrol()
        {
            bool onay = true;

            if (t.IsNotNull(ds_UygPlanYapilacakAdaylar) == false)
            {
                MessageBox.Show("Dikkat : Aday Liste bulunumadı ...");
                onay = false;
            }

            if (t.IsNotNull(ds_MtskSablonUygulamaBPlanlama) == false)
            {
                MessageBox.Show("Dikkat : Şablon bulunumadı, lütfen şablon seçin ...");
                onay = false;
            }

            if (t.IsNotNull(ds_MtskSablonUygulamaFKayit_F01) == false)
            {
                MessageBox.Show("Dikkat : Planlama Formu bulunumadı ...");
                onay = false;
            }

            if (ds_MtskUygulamaliDersPlan_L01 == null)
            {
                MessageBox.Show("Dikkat : Ders Planı Sonuç Formu bulunumadı ...");
                onay = false;
            }

            return onay;
        }

        private bool planOncesiPlanlamaFormuKontolu()
        {
            bool onay = true;

            // BaslangicTarihi Date 
            // SimulatorId Int 
            // AracId Int   
            // Ogretici1Id Int 
            // Ogretici2Id Int 

            string Tarih = ds_MtskSablonUygulamaFKayit_F01.Tables[0].Rows[dN_MtskSablonUygulamaFKayit_F01.Position]["BaslangicTarihi"].ToString();
            string SimulatorId = ds_MtskSablonUygulamaFKayit_F01.Tables[0].Rows[dN_MtskSablonUygulamaFKayit_F01.Position]["SimulatorId"].ToString();
            string AracId = ds_MtskSablonUygulamaFKayit_F01.Tables[0].Rows[dN_MtskSablonUygulamaFKayit_F01.Position]["AracId"].ToString();
            string Ogretici1Id = ds_MtskSablonUygulamaFKayit_F01.Tables[0].Rows[dN_MtskSablonUygulamaFKayit_F01.Position]["Ogretici1Id"].ToString();
            string Ogretici2Id = ds_MtskSablonUygulamaFKayit_F01.Tables[0].Rows[dN_MtskSablonUygulamaFKayit_F01.Position]["Ogretici2Id"].ToString();
            string mesaj = "";

            if (Tarih == "") mesaj = "Başlangıç Tarihini" + v.ENTER2;

            if ((uygSablonB.aracTipiId == aracTipi.simülatorArac) &&
                (SimulatorId == ""))
                mesaj = mesaj + "Simülatörü" + v.ENTER2;

            if (AracId == "") mesaj = mesaj + "Eğitim Aracını" + v.ENTER2;

            if ((Ogretici1Id == "") || (Ogretici2Id == "")) mesaj = mesaj + "Usta Öğreticileri" + v.ENTER2;

            if (mesaj != "")
            {
                onay = false;
                mesaj = mesaj + "Eksiklerini tamamlamanız gerekiyor...";
                MessageBox.Show(mesaj);
            }
            
            return onay;
        }

        #endregion UygulamaPlanlamaFormu

        /// Sablon Hazırlama Formu
        /// 

        #region TeorikSablonHazirlamaFormu
        private void preparingTeorikSablonHazirlamaFormu()
        {
            this._formTipi = formTipi.SablonHazirlamaFormu;

            dN_SablonB.PositionChanged += new System.EventHandler(dataNavigatorB_PositionChanged);

            TableIPCode = "UST/MEB/MtskSablonTeorikB.Planlama_F01";
            Control cntrl = null;
            string[] controls = new string[] { };
            cntrl = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_SablonFormuTeorikSablonHazirla);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("KAYDET16");
            }

            TableIPCode = "UST/MEB/MtskSablonTeorikS.Planlama_F01";
            this.ds_Fields = new DataSet();
            tr.MS_Fields_IP_Read(this.ds_Fields, TableIPCode);
        }
        
        private void getSablonTeorik(DataSet ds, DataNavigator dN)
        {
            int pos = dN.Position;
            if (pos == -1) return;
            teoSablonB.Clear();
            teoSablonB.Id = t.myInt32(ds.Tables[0].Rows[pos]["Id"].ToString());
            teoSablonB.SablonAdi = ds.Tables[0].Rows[pos]["SablonAdi"].ToString();
            teoSablonB.gunTipiId = (gunTipi)Convert.ToByte(ds.Tables[0].Rows[pos]["GunTipiId"].ToString());
            teoSablonB.toplamIsGunuSayisi = t.myInt16(ds.Tables[0].Rows[pos]["ToplamIsGunuSayisi"].ToString());
            teoSablonB.saatTipiId = (saatTipi)Convert.ToByte(ds.Tables[0].Rows[pos]["SaatTipiId"].ToString());
            teoSablonB.toplamDersSaati = t.myInt16(ds.Tables[0].Rows[pos]["ToplamDersSaati"].ToString());
            teoSablonB.gundeMaxSaat = t.myInt16(ds.Tables[0].Rows[pos]["GundeMaxSaat"].ToString());
            teoSablonB.haftadaMaxSaat = t.myInt16(ds.Tables[0].Rows[pos]["HaftadaMaxSaat"].ToString());
        }

        private void btn_SablonFormuTeorikSablonHazirla(object sender, EventArgs e)
        {
            //
            TabControlName = "tabControl_SUBVIEW";
            tabControl = null;
            tabControl = t.Find_Control(this, TabControlName);

            getSablonTeorik(ds_SablonB, dN_SablonB);
            preparingSablon(teoSablonB.Id, teoSablonB.SablonAdi);
        }
        
        private void readGridDataTeorik(int pos, Int16 gunId)
        {
            ///,gun1.Id              as gun1_Id 
            ///,gun1.SablonTeorikBId as gun1_SablonTeorikBId
            ///,gun1.IsActive        as gun1_IsActive
            ///,gun1.GunId      as gun1_GunId        /* 1,2,3... gun  */
            ///,gun1.SaatTipiId as gun1_SaatTipiId  /* 7,...12...18,19,20.. */
            ///,gun1.GrupTipiId as gun1_GrupTipiId  /* grup 1,2,3 */
            ///,gun1.SubeTipiId as gun1_SubeTipiId  /* a,b,c ... */
            ///,gun1.DerslikTipiId    as gun1_DerslikTipiId  /* 1. Trafik ve Çevre Bilgisi, İlk Yardım, Araç Tekniği */
            ///,gun1.DerslikAdiTipiId as gun1_DerslikAdiTipiId /* Derslik-1, -2 ... -10 */
            ///,gun1.DersSaatiTipiId  as gun1_DersSaatiTipiId
            ///,gun1.EgitimTipiId     as gun1_EgitimTipiId  /* 1. Normal, 2. Telafi */
                                 
            string x = gunId.ToString();
            string field = "";
            string value = "";

            bool IsActive = Convert.ToBoolean(ds_SablonS.Tables[0].Rows[pos]["IsActive"].ToString());

            if (IsActive)
            {

                teoTaskakS.pos = pos;

                field = "gun" + x + "_Id";
                teoTaskakS.Id = t.myInt32(ds_SablonS.Tables[0].Rows[pos][field].ToString());

                field = "gun" + x + "_SablonTeorikBId";
                teoTaskakS.SablonTeorikBId = t.myInt32(ds_SablonS.Tables[0].Rows[pos][field].ToString());

                if (teoTaskakS.SablonTeorikBId == 0) teoTaskakS.SablonTeorikBId = teoSablonB.Id;

                field = "gun" + x + "_IsActive";
                value = ds_SablonS.Tables[0].Rows[pos][field].ToString();
                if (value == "")
                {
                    ds_SablonS.Tables[0].Rows[pos][field] = true;
                }
                teoTaskakS.IsActive = Convert.ToBoolean(ds_SablonS.Tables[0].Rows[pos][field].ToString());

                field = "gun" + x + "_GunId";
                teoTaskakS.GunId = t.myInt16(ds_SablonS.Tables[0].Rows[pos][field].ToString());

                if (teoTaskakS.GunId == 0) teoTaskakS.GunId = gunId;

                field = "gun" + x + "_SaatTipiId";
                teoTaskakS.SaatTipiId = t.myInt16(ds_SablonS.Tables[0].Rows[pos][field].ToString());

                if (teoTaskakS.SaatTipiId == 0) teoTaskakS.SaatTipiId = t.myInt16(ds_SablonS.Tables[0].Rows[pos]["Lkp_SaatId"].ToString());

                field = "gun" + x + "_GrupTipiId";
                teoTaskakS.GrupTipiId = t.myInt16(ds_SablonS.Tables[0].Rows[pos][field].ToString());
                field = "gun" + x + "_SubeTipiId";
                teoTaskakS.SubeTipiId = t.myInt16(ds_SablonS.Tables[0].Rows[pos][field].ToString());

                field = "gun" + x + "_DerslikTipiId";
                teoTaskakS.DerslikTipiId = t.myInt16(ds_SablonS.Tables[0].Rows[pos][field].ToString());
                field = "gun" + x + "_DerslikAdiTipiId";
                teoTaskakS.DerslikAdiTipiId = t.myInt16(ds_SablonS.Tables[0].Rows[pos][field].ToString());
                field = "gun" + x + "_DersSaatiTipiId";
                teoTaskakS.DersSaatiTipiId = t.myInt16(ds_SablonS.Tables[0].Rows[pos][field].ToString());
                field = "gun" + x + "_EgitimTipiId";
                teoTaskakS.EgitimTipiId = t.myInt16(ds_SablonS.Tables[0].Rows[pos][field].ToString());
            }
        }

        private void writeDataTeorik()
        {
            ///,gun1.Id              as gun1_Id 
            ///,gun1.SablonTeorikBId as gun1_SablonTeorikBId
            ///,gun1.IsActive        as gun1_IsActive
            ///,gun1.GunId      as gun1_GunId                  /* 1,2,3... gun  */
            ///,gun1.SaatTipiId as gun1_SaatTipiId             /* 7,...12...18,19,20.. */
            ///,gun1.GrupTipiId as gun1_GrupTipiId             /* grup 1,2,3 */
            ///,gun1.SubeTipiId as gun1_SubeTipiId             /* a,b,c ... */
            ///,gun1.DerslikTipiId    as gun1_DerslikTipiId    /* 1. Trafik ve Çevre Bilgisi, İlk Yardım, Araç Tekniği */
            ///,gun1.DerslikAdiTipiId as gun1_DerslikAdiTipiId /* Derslik-1, -2 ... -10 */
            ///,gun1.DersSaatiTipiId  as gun1_DersSaatiTipiId
            ///,gun1.EgitimTipiId     as gun1_EgitimTipiId     /* 1. Normal, 2. Telafi */

            //bool kontrol = (teoTaskakS.GrupTipiId > 0) && (teoTaskakS.SubeTipiId > 0) && (teoTaskakS.DerslikTipiId > 0);
            bool kontrol = (teoTaskakS.DerslikTipiId > 0);

            if (kontrol == false) return;

            string State = "";
            int position = 0;
            ds_SablonSSave.Tables[0].CaseSensitive = false;

            if (teoTaskakS.Id == 0)
            {
                State = "dsInsert";
                DataRow newRow = ds_SablonSSave.Tables[0].NewRow();

                newRow["SablonTeorikBId"] = teoTaskakS.SablonTeorikBId;
                newRow["IsActive"] = teoTaskakS.IsActive;
                newRow["GunId"] = teoTaskakS.GunId;
                newRow["SaatTipiId"] = teoTaskakS.SaatTipiId;
                newRow["GrupTipiId"] = teoTaskakS.GrupTipiId;
                newRow["SubeTipiId"] = teoTaskakS.SubeTipiId;
                newRow["DerslikTipiId"] = teoTaskakS.DerslikTipiId;
                newRow["DerslikAdiTipiId"] = teoTaskakS.DerslikAdiTipiId;
                newRow["DersSaatiTipiId"] = teoTaskakS.DersSaatiTipiId;
                newRow["EgitimTipiId"] = teoTaskakS.EgitimTipiId;

                ds_SablonSSave.Tables[0].Rows.Add(newRow);

                position = ds_SablonSSave.Tables[0].Rows.Count - 1;
            }
            else
            {
                int count = ds_SablonSSave.Tables[0].Rows.Count;

                for (int i = 0; i < count; i++)
                {
                    if (teoTaskakS.Id == t.myInt32(ds_SablonSSave.Tables[0].Rows[i]["Id"].ToString()))
                    {
                        State = "dsEdit";
                        ds_SablonSSave.Tables[0].Rows[i]["SablonTeorikBId"] = teoTaskakS.SablonTeorikBId;
                        ds_SablonSSave.Tables[0].Rows[i]["IsActive"] = teoTaskakS.IsActive;
                        ds_SablonSSave.Tables[0].Rows[i]["GunId"] = teoTaskakS.GunId;
                        ds_SablonSSave.Tables[0].Rows[i]["SaatTipiId"] = teoTaskakS.SaatTipiId;
                        ds_SablonSSave.Tables[0].Rows[i]["GrupTipiId"] = teoTaskakS.GrupTipiId;
                        ds_SablonSSave.Tables[0].Rows[i]["SubeTipiId"] = teoTaskakS.SubeTipiId;
                        ds_SablonSSave.Tables[0].Rows[i]["DerslikTipiId"] = teoTaskakS.DerslikTipiId;
                        ds_SablonSSave.Tables[0].Rows[i]["DerslikAdiTipiId"] = teoTaskakS.DerslikAdiTipiId;
                        ds_SablonSSave.Tables[0].Rows[i]["DersSaatiTipiId"] = teoTaskakS.DersSaatiTipiId;
                        ds_SablonSSave.Tables[0].Rows[i]["EgitimTipiId"] = teoTaskakS.EgitimTipiId;

                        //ds_SablonSSave.Tables[0].
                        position = i;
                        break;
                    }
                }
            }

            sv.tDataSave(this, ds_SablonSSave, dN_SablonSSave, position);

            if (State == "dsInsert")
            {
                string field = "";

                field = "gun" + teoTaskakS.GunId.ToString() + "_Id";
                ds_SablonS.Tables[0].Rows[teoTaskakS.pos][field] =
                    ds_SablonSSave.Tables[0].Rows[position]["Id"];

                field = "gun" + teoTaskakS.GunId.ToString() + "_SablonTeorikBId";
                ds_SablonS.Tables[0].Rows[teoTaskakS.pos][field] =
                    ds_SablonSSave.Tables[0].Rows[position]["SablonTeorikBId"];

                field = "gun" + teoTaskakS.GunId.ToString() + "_GunId";
                ds_SablonS.Tables[0].Rows[teoTaskakS.pos][field] =
                    ds_SablonSSave.Tables[0].Rows[position]["GunId"];

                field = "gun" + teoTaskakS.GunId.ToString() + "_SaatTipiId";
                ds_SablonS.Tables[0].Rows[teoTaskakS.pos][field] =
                    ds_SablonSSave.Tables[0].Rows[position]["SaatTipiId"];
            }

        }
               
        #endregion TeorikSablonHazirlamaFormu

        #region UygulamaSablonHazirlamaFormu
        private void preparingUygulamaSablonHazirlamaFormu()
        {
            this._formTipi = formTipi.SablonHazirlamaFormu;

            dN_SablonB.PositionChanged += new System.EventHandler(dataNavigatorB_PositionChanged);

            TableIPCode = "UST/MEB/MtskSablonUygulamaB.Planlama_F01";
            Control cntrl = null;
            string[] controls = new string[] { };
            cntrl = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_SablonFormuUygulamaSablonHazirla);
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = t.Find_Glyph("KAYDET16");
            }
            
            TableIPCode = "UST/MEB/MtskSablonUygulamaS.Planlama_F01";
            this.ds_Fields = new DataSet();
            tr.MS_Fields_IP_Read(this.ds_Fields, TableIPCode);
        }

        private void getSablonUygulama(DataSet ds, DataNavigator dN)
        {
            int pos = dN.Position;
            if (pos == -1) return;
            uygSablonB.Clear();
            uygSablonB.Id = t.myInt32(ds.Tables[0].Rows[pos]["Id"].ToString());
            uygSablonB.SablonAdi = ds.Tables[0].Rows[pos]["SablonAdi"].ToString();
            uygSablonB.gunTipiId = (gunTipi)Convert.ToByte(ds.Tables[0].Rows[pos]["GunTipiId"].ToString());
            uygSablonB.toplamIsGunuSayisi = t.myInt16(ds.Tables[0].Rows[pos]["ToplamIsGunuSayisi"].ToString());
            uygSablonB.saatTipiId = (saatTipi)Convert.ToByte(ds.Tables[0].Rows[pos]["SaatTipiId"].ToString());
            uygSablonB.MevcutSertifikaTipiId = t.myInt16(ds.Tables[0].Rows[pos]["MevcutSertifikaTipiId"].ToString());
            uygSablonB.IstenenSertifikaTipiId = t.myInt16(ds.Tables[0].Rows[pos]["IstenenSertifikaTipiId"].ToString());
            uygSablonB.toplamDersSaati = t.myInt16(ds.Tables[0].Rows[pos]["ToplamDersSaati"].ToString());
            uygSablonB.aracTipiId = (aracTipi)Convert.ToByte(ds.Tables[0].Rows[pos]["AracTipiId"].ToString());
            uygSablonB.ogreticiTipiId = (ogreticiTipiId)Convert.ToByte(ds.Tables[0].Rows[pos]["OgreticiTipiId"].ToString());
            uygSablonB.adayTipiId = (adayTipiId)Convert.ToByte(ds.Tables[0].Rows[pos]["AdayTipiId"].ToString());
            uygSablonB.gundeMaxSaat = t.myInt16(ds.Tables[0].Rows[pos]["GundeMaxSaat"].ToString());
            uygSablonB.haftadaMaxSaat = t.myInt16(ds.Tables[0].Rows[pos]["HaftadaMaxSaat"].ToString());
        }

        private void btn_SablonFormuUygulamaSablonHazirla(object sender, EventArgs e)
        {
            //
            TabControlName = "tabControl_SUBVIEW";
            tabControl = null;
            tabControl = t.Find_Control(this, TabControlName);

            getSablonUygulama(ds_SablonB, dN_SablonB);
            preparingSablon(uygSablonB.Id, uygSablonB.SablonAdi);
        }
        
        private void readGridDataUygulama(int pos, Int16 gunId)
        {
            ///,gun1.Id                as gun1_Id 
            ///,gun1.SablonUygulamaBId as gun1_SablonUygulamaBId 
            ///,gun1.IsActive          as gun1_IsActive 
            ///,gun1.GunId             as gun1_GunId 
            ///,gun1.SaatTipiId        as gun1_SaatTipiId 
            ///,gun1.AracTipiId        as gun1_AracTipiId 
            ///,gun1.OgreticiTipiId    as gun1_OgreticiTipiId 
            ///,gun1.AdayTipiId        as gun1_AdayTipiId 

            string x = gunId.ToString();
            string field = "";
            string value = "";
            
            bool IsActive = Convert.ToBoolean(ds_SablonS.Tables[0].Rows[pos]["IsActive"].ToString());

            if (IsActive)
            {
                uygTaskakS.pos = pos;

                field = "gun" + x + "_Id";
                uygTaskakS.Id = t.myInt32(ds_SablonS.Tables[0].Rows[pos][field].ToString());

                field = "gun" + x + "_SablonUygulamaBId";
                uygTaskakS.SablonUygulamaBId = t.myInt32(ds_SablonS.Tables[0].Rows[pos][field].ToString());

                if (uygTaskakS.SablonUygulamaBId == 0) uygTaskakS.SablonUygulamaBId = uygSablonB.Id;

                field = "gun" + x + "_IsActive";
                value = ds_SablonS.Tables[0].Rows[pos][field].ToString();
                if (value == "")
                {
                    ds_SablonS.Tables[0].Rows[pos][field] = true;
                }
                uygTaskakS.IsActive = Convert.ToBoolean(ds_SablonS.Tables[0].Rows[pos][field].ToString());

                field = "gun" + x + "_GunId";
                uygTaskakS.GunId = t.myInt16(ds_SablonS.Tables[0].Rows[pos][field].ToString());

                if (uygTaskakS.GunId == 0) uygTaskakS.GunId = gunId;

                field = "gun" + x + "_SaatTipiId";
                uygTaskakS.SaatTipiId = t.myInt16(ds_SablonS.Tables[0].Rows[pos][field].ToString());

                if (uygTaskakS.SaatTipiId == 0) uygTaskakS.SaatTipiId = t.myInt16(ds_SablonS.Tables[0].Rows[pos]["Lkp_SaatId"].ToString());

                field = "gun" + x + "_AracTipiId";
                uygTaskakS.AracTipiId = t.myInt16(ds_SablonS.Tables[0].Rows[pos][field].ToString());

                field = "gun" + x + "_OgreticiTipiId";
                uygTaskakS.OgreticiTipiId = t.myInt16(ds_SablonS.Tables[0].Rows[pos][field].ToString());

                field = "gun" + x + "_AdayTipiId";
                uygTaskakS.AdayTipiId = t.myInt16(ds_SablonS.Tables[0].Rows[pos][field].ToString());
            }
        }

        private void writeDataUygula()
        {
            ///,gun1.Id                as gun1_Id 
            ///,gun1.SablonUygulamaBId as gun1_SablonUygulamaBId 
            ///,gun1.IsActive          as gun1_IsActive 
            ///,gun1.GunId             as gun1_GunId 
            ///,gun1.SaatTipiId        as gun1_SaatTipiId 
            ///,gun1.AracTipiId        as gun1_AracTipiId 
            ///,gun1.OgreticiTipiId    as gun1_OgreticiTipiId 
            ///,gun1.AdayTipiId        as gun1_AdayTipiId 

            bool kontrol = (uygTaskakS.AracTipiId > 0) && (uygTaskakS.OgreticiTipiId > 0) && (uygTaskakS.AdayTipiId > 0);

            if (kontrol == false) return;

            string State = "";
            int position = 0;
            ds_SablonSSave.Tables[0].CaseSensitive = false;

            if (uygTaskakS.Id == 0)
            {
                State = "dsInsert";
                DataRow newRow = ds_SablonSSave.Tables[0].NewRow();

                newRow["SablonUygulamaBId"] = uygTaskakS.SablonUygulamaBId;
                newRow["IsActive"] = uygTaskakS.IsActive;
                newRow["GunId"] = uygTaskakS.GunId;
                newRow["SaatTipiId"] = uygTaskakS.SaatTipiId;
                newRow["AracTipiId"] = uygTaskakS.AracTipiId;
                newRow["OgreticiTipiId"] = uygTaskakS.OgreticiTipiId;
                newRow["AdayTipiId"] = uygTaskakS.AdayTipiId;
                                
                ds_SablonSSave.Tables[0].Rows.Add(newRow);

                position = ds_SablonSSave.Tables[0].Rows.Count - 1;
            }
            else
            {
                int count = ds_SablonSSave.Tables[0].Rows.Count;

                for (int i = 0; i < count; i++)
                {
                    if (uygTaskakS.Id == t.myInt32(ds_SablonSSave.Tables[0].Rows[i]["Id"].ToString()))
                    {
                        State = "dsEdit";
                        ds_SablonSSave.Tables[0].Rows[i]["SablonUygulamaBId"] = uygTaskakS.SablonUygulamaBId;
                        ds_SablonSSave.Tables[0].Rows[i]["IsActive"] = uygTaskakS.IsActive;
                        ds_SablonSSave.Tables[0].Rows[i]["GunId"] = uygTaskakS.GunId;
                        ds_SablonSSave.Tables[0].Rows[i]["SaatTipiId"] = uygTaskakS.SaatTipiId;
                        ds_SablonSSave.Tables[0].Rows[i]["AracTipiId"] = uygTaskakS.AracTipiId;
                        ds_SablonSSave.Tables[0].Rows[i]["OgreticiTipiId"] = uygTaskakS.OgreticiTipiId;
                        ds_SablonSSave.Tables[0].Rows[i]["AdayTipiId"] = uygTaskakS.AdayTipiId;
                        //ds_SablonSSave.Tables[0].
                        position = i;
                        break;
                    }
                }
            }
                        
            sv.tDataSave(this, ds_SablonSSave, dN_SablonSSave, position);

            if (State == "dsInsert")
            {
                string field = "";
                
                field = "gun" + uygTaskakS.GunId.ToString() + "_Id";
                ds_SablonS.Tables[0].Rows[uygTaskakS.pos][field] =
                    ds_SablonSSave.Tables[0].Rows[position]["Id"];

                field = "gun" + uygTaskakS.GunId.ToString() + "_SablonUygulamaBId";
                ds_SablonS.Tables[0].Rows[uygTaskakS.pos][field] =
                    ds_SablonSSave.Tables[0].Rows[position]["SablonUygulamaBId"];

                field = "gun" + uygTaskakS.GunId.ToString() + "_GunId";
                ds_SablonS.Tables[0].Rows[uygTaskakS.pos][field] =
                    ds_SablonSSave.Tables[0].Rows[position]["GunId"];

                field = "gun" + uygTaskakS.GunId.ToString() + "_SaatTipiId";
                ds_SablonS.Tables[0].Rows[uygTaskakS.pos][field] =
                    ds_SablonSSave.Tables[0].Rows[position]["SaatTipiId"];
            }

        }
        
        #endregion UygulamaSablonHazirlamaFormu

        /// Ortak Fonksiyonlar
        /// 
        
        #region Teorik ve Uygulama Sablon Hazirlama Ortak Fonksiyonlar 2

        private void dataNavigatorB_PositionChanged(object sender, EventArgs e)
        {
            if (_planTipi == planTipi.teorik)
                getSablonTeorik(ds_SablonB, dN_SablonB);

            if (_planTipi == planTipi.uygulama)
                getSablonUygulama(ds_SablonB, dN_SablonB);
        }

        private void dataNavigatorS_PositionChanged(object sender, EventArgs e)
        {
            if (IsPreparing) return;

            mtskSablonSKaydet(sender);

            ((DevExpress.XtraEditors.DataNavigator)sender).Tag = ((DevExpress.XtraEditors.DataNavigator)sender).Position;
        }

        private void mtskSablonSKaydet(object sender)
        {
            if (this._formTipi != formTipi.SablonHazirlamaFormu) return;

            object x = ((DevExpress.XtraEditors.DataNavigator)sender).Tag;

            int oldPosition = ((int)x);
            Int16 gunId = 1;
            
            for (int i = 0; i < this.tGridView.Bands.Count; i++)
            {
                if (tGridView.Bands[i].Caption.IndexOf(gunId.ToString() + ". İş Günü") > -1)
                {
                    if (_planTipi == planTipi.teorik)
                    {
                        teoTaskakS.Clear();
                        readGridDataTeorik(oldPosition, gunId);
                        writeDataTeorik();
                    }

                    if (_planTipi == planTipi.uygulama)
                    {
                        uygTaskakS.Clear();
                        readGridDataUygulama(oldPosition, gunId);
                        writeDataUygula();
                    }
                    
                    gunId++;
                }
            }
        }

        #endregion Teorik ve Uygulama Sablon Hazirlama Ortak Fonksiyonlar 2

        #region Teorik ve Uygulama Sablon Hazirlama Ortak Fonksiyonlar 1

        private void preparingSablon(int refId, string SablonAdi)
        {
            int ViewType = v.obj_vw_GridView;
            string ReadValue = "";
            
            TableIPCode = "SablonHazirlama_" + refId;
            Control gridCntrl = dv.Create_View_(ViewType, refId, TableIPCode, "");
            //
            DevExpress.XtraEditors.PanelControl panelControl1 = new DevExpress.XtraEditors.PanelControl();
            panelControl1.Dock = DockStyle.Fill;

            Control cntrl = null;
            string[] controls = new string[] {
                    "DevExpress.XtraTab.XtraTabPage",
                    "DevExpress.XtraBars.Navigation.NavigationPage",
                    "DevExpress.XtraBars.Navigation.TabNavigationPage",
                    "DevExpress.XtraBars.Ribbon.BackstageViewClientControl"
                };

            ev.CreateOrSelect_Page(this, TabControlName, "", TableIPCode, ReadValue, SablonAdi, true);
            cntrl = t.Find_Control(this, "tTabPage_" + t.AntiStr_Dot(TableIPCode + ReadValue), "", controls);

            // TabPage bulunduysa üzerinde istenen IP (InputPanel) oluşturuluyor
            if (cntrl != null)
            {
                if (cntrl.Controls.Count == 0)
                {
                    preparingGrid(panelControl1, (GridControl)gridCntrl);
                    cntrl.Controls.Add(panelControl1);
                }
            }
        }

        private void preparingGrid(PanelControl panelControl1, GridControl tGridControl)
        {
            int refId = 0;
            string caption = "";
            Int16 toplamIsGunuSayisi = 0;
            saatTipi _saatTipi = saatTipi.none;

            if (_planTipi == planTipi.teorik)
            {
                refId = teoSablonB.Id;
                caption = teoSablonB.SablonAdi;
                toplamIsGunuSayisi = teoSablonB.toplamIsGunuSayisi;
                _saatTipi = teoSablonB.saatTipiId;
            }

            if (_planTipi == planTipi.uygulama)
            {
                refId = uygSablonB.Id;
                caption = uygSablonB.SablonAdi;
                toplamIsGunuSayisi = uygSablonB.toplamIsGunuSayisi;
                _saatTipi = uygSablonB.saatTipiId;
            }

            tGridView = new AdvBandedGridView(tGridControl);
            tGridView.Name = "tGridView_" + refId.ToString();
            tGridControl.MainView = tGridView;
            tGridControl.Dock = DockStyle.Fill;
            tGridControl.BringToFront();

            int find = 0;

            tGridView.OptionsFind.AllowFindPanel = false;
            tGridView.OptionsFind.AlwaysVisible = (find > 0);
            /// ( 100 * find )  neden bunu yapıyorum ?
            /// findDelay = 100 ise standart find
            /// findDelay = 200 ise list && data  yı işaret ediyor 
            tGridView.OptionsFind.FindDelay = 100 * find;
            tGridView.OptionsFind.FindMode = DevExpress.XtraEditors.FindMode.Always;  //FindClick; //Always;
            tGridView.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            tGridView.OptionsView.ColumnAutoWidth = false;
            tGridView.OptionsView.RowAutoHeight = true;
            tGridView.OptionsBehavior.AllowIncrementalSearch = true;
            tGridView.OptionsNavigation.EnterMoveNextColumn = true;
            tGridView.OptionsSelection.InvertSelection = (find == 0);

            tGridView.Appearance.FocusedCell.BackColor = v.colorNew;
            tGridView.Appearance.FocusedCell.Options.UseBackColor = true;
            tGridView.Appearance.FocusedRow.BackColor = v.colorNavigator;
            tGridView.Appearance.FocusedRow.Options.UseBackColor = true;

            tGridView.Appearance.FocusedCell.BorderColor = v.colorExit;
            tGridView.Appearance.FocusedCell.Options.UseBorderColor = true;
            tGridView.Appearance.FocusedRow.BorderColor = v.colorExit;
            tGridView.Appearance.FocusedRow.Options.UseBorderColor = true;

            tGridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            tGridView.OptionsView.GroupDrawMode = GroupDrawMode.Office2003;
            tGridView.OptionsBehavior.AutoExpandAllGroups = true;
            tGridView.OptionsView.EnableAppearanceOddRow = true;

            tGridBands_Add(tGridView);
            
            if (_planTipi == planTipi.teorik)
                tAdvBandedGrid_Columns_Create_Teorik(tGridView, toplamIsGunuSayisi);
            
            if (_planTipi == planTipi.uygulama)
                tAdvBandedGrid_Columns_Create_Uygulama(tGridView, toplamIsGunuSayisi);

            IsPreparing = true;

            tDataSource_Add(panelControl1, tGridControl, toplamIsGunuSayisi, _saatTipi, refId);

            if (_planTipi == planTipi.uygulama)
                preparingAracPlani(toplamIsGunuSayisi);

            if (this._formTipi == formTipi.PlanlamaFormu)
                tGridView.OptionsView.AllowCellMerge = true;

            IsPreparing = false;
        }

        private void tGridBands_Add(AdvBandedGridView tGridView)
        {
            gunTipi _gunTipi = gunTipi.none;
            Int16 toplamIsGunuSayisi = 0;
            Int16 IsGunuSayisi = 0;
            Int16 IzinGunuSayisi = 0;
            Int16 say = 1;
            Int16 isGunu = 1;
            Int16 izinGunu = 1;

            if (_planTipi == planTipi.teorik)
            {
                _gunTipi = teoSablonB.gunTipiId;
                toplamIsGunuSayisi = teoSablonB.toplamIsGunuSayisi;
            }

            if (_planTipi == planTipi.uygulama)
            {
                _gunTipi = uygSablonB.gunTipiId;
                toplamIsGunuSayisi = uygSablonB.toplamIsGunuSayisi;
            }


            GridBand Gband0 = new GridBand();
            Gband0.Caption = "Hakkında";
            Gband0.Visible = true;
            Gband0.VisibleIndex = 0;
            Gband0.Width = 250;
            tGridView.Bands.AddRange(new GridBand[] { Gband0 });


            /// gunTipi._5IsGunu2GunIzin
            /// gunTipi._6IsGunu1GunIzin
            /// 
            if (_gunTipi == gunTipi._5IsGunu2GunIzin)
            {
                IsGunuSayisi = 5;
                IzinGunuSayisi = 2;
            }
            if (_gunTipi == gunTipi._6IsGunu1GunIzin)
            {
                IsGunuSayisi = 6;
                IzinGunuSayisi = 1;
            }
            if (toplamIsGunuSayisi == 0)
            {
                if (_gunTipi == gunTipi._5IsGunu2GunIzin) toplamIsGunuSayisi = 5;
                if (_gunTipi == gunTipi._6IsGunu1GunIzin) toplamIsGunuSayisi = 6;
            }

            while (say <= toplamIsGunuSayisi)
            {
                GridBand Gband = new GridBand();

                if (isGunu > IsGunuSayisi)
                {
                    Gband.Caption = "İzin";
                    izinGunu++;
                }

                if (isGunu <= IsGunuSayisi)
                {
                    Gband.Caption = say.ToString() + ". İş Günü";
                    isGunu++;
                    say++;
                }

                Gband.Visible = true;
                Gband.VisibleIndex = say;
                Gband.Width = 150;

                tGridView.Bands.AddRange(new GridBand[] { Gband });

                if ((isGunu > IsGunuSayisi) && (izinGunu > IzinGunuSayisi))
                {
                    isGunu = 1;
                    izinGunu = 1;
                }
            }

        }

        private void tAdvBandedGrid_Columns_Create_Teorik(AdvBandedGridView tGridView, Int16 toplamIsGunuSayisi)
        {
            string tfield_name = "";
            string caption = "";
            Int16 gunId = 1;

            DataRow Row = null;
            Row = getFieldsRow("SaatTipiId");
            tfield_name = "Lkp_SaatTipi";
            caption = "Saat";
            createColumn(tGridView, Row, 0, 0, tfield_name, caption);

            while (gunId <= toplamIsGunuSayisi)
            {
                //,gunx.GrupTipiId       as gunx_GrupTipiId
                //,gunx.SubeTipiId       as gunx_SubeTipiId
                //,gunx.DerslikTipiId    as gunx_DerslikTipiId
                //,gunx.DerslikAdiTipiId as gunx_DerslikAdiTipiId
                //,gunx.EgitimTipiId     as gunx_EgitimTipiId

                Row = getFieldsRow("EgitimTipiId");
                tfield_name = "gun" + gunId.ToString() + "_EgitimTipiId";
                caption = "Eğitim Tipi";
                createColumn(tGridView, Row, gunId, 5, tfield_name, caption);

                Row = getFieldsRow("DerslikAdiTipiId");
                tfield_name = "gun" + gunId.ToString() + "_DerslikAdiTipiId";
                caption = "Derslik Adı";
                createColumn(tGridView, Row, gunId, 4, tfield_name, caption);

                Row = getFieldsRow("DerslikTipiId");
                tfield_name = "gun" + gunId.ToString() + "_DerslikTipiId";
                caption = "Ders";
                createColumn(tGridView, Row, gunId, 3, tfield_name, caption);

                Row = getFieldsRow("SubeTipiId");
                tfield_name = "gun" + gunId.ToString() + "_SubeTipiId";
                caption = "Şube";
                createColumn(tGridView, Row, gunId, 2, tfield_name, caption);

                Row = getFieldsRow("GrupTipiId");
                tfield_name = "gun" + gunId.ToString() + "_GrupTipiId";
                caption = "Grup";
                createColumn(tGridView, Row, gunId, 1, tfield_name, caption);

                gunId++;
            }
        }

        private void tAdvBandedGrid_Columns_Create_Uygulama(AdvBandedGridView tGridView, Int16 toplamIsGunuSayisi)
        {
            string tfield_name = "";
            string caption = "";
            Int16 gunId = 1;

            DataRow Row = null;
            Row = getFieldsRow("SaatTipiId");
            tfield_name = "Lkp_SaatTipi";
            caption = "Saat";
            createColumn(tGridView, Row, 0, 0, tfield_name, caption);

            while (gunId <= toplamIsGunuSayisi)
            {
                //,gunx.AracTipiId as gunx_AracTipiId
                //,gunx.OgreticiTipiId as gunx_OgreticiTipiId
                //,gunx.AdayTipiId as gunx_AdayTipiId

                Row = getFieldsRow("AdayTipiId");
                tfield_name = "gun" + gunId.ToString() + "_AdayTipiId";
                caption = "Aday";
                createColumn(tGridView, Row, gunId, 3, tfield_name, caption);

                Row = getFieldsRow("OgreticiTipiId");
                tfield_name = "gun" + gunId.ToString() + "_OgreticiTipiId";
                caption = "Usta Öğretici";
                createColumn(tGridView, Row, gunId, 2, tfield_name, caption);

                Row = getFieldsRow("AracTipiId");
                tfield_name = "gun" + gunId.ToString() + "_AracTipiId";
                caption = "Araç";
                createColumn(tGridView, Row, gunId, 1, tfield_name, caption);

                //Row = getFieldsRow("Id");
                //tfield_name = "gun" + gunId.ToString() + "_Id";
                //caption = "Id";
                //createColumn(tGridView, Row, gunId, 0, tfield_name, caption);

                gunId++;
            }
        }

        private DataRow getFieldsRow(string fieldName)
        {
            DataRow row = null;
            for (int i = 0; i < this.ds_Fields.Tables[0].Rows.Count; i++)
            {
                if (ds_Fields.Tables[0].Rows[i]["LKP_FIELD_NAME"].ToString() == fieldName)
                {
                    row = ds_Fields.Tables[0].Rows[i];
                    break;
                }
            }
            return row;
        }
        
        private void createColumn(AdvBandedGridView tGridView, DataRow Row, Int16 gunId, Int16 line_no, string tfield_name, string caption)
        {
            BandedGridColumn column = new BandedGridColumn();
            column.Name = "Column_" + tfield_name;
            column.FieldName = tfield_name;
            column.Caption = caption;
            column.Visible = true;
            column.OptionsColumn.AllowEdit = true;
            column.Width = 70;
            column.MinWidth = 30;

            if (this._formTipi == formTipi.PlanlamaFormu)
                column.OptionsColumn.AllowEdit = false;

            if (gunId > 0)
            {
                if (line_no > 0)
                {
                    RepositoryItemImageComboBox tEdit = new RepositoryItemImageComboBox();
                    tEdit.Name = "Column_" + tfield_name;
                    //tEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(evg.myRepositoryItemEdit_KeyDown);
                    //tEdit.AccessibleName = TableIPCode;
                    dc.RepositoryItemImageComboBox_Fill(tEdit, Row, "", 1);

                    column.ColumnEdit = tEdit;
                }

                for (int i = 0; i < tGridView.Bands.Count; i++)
                {
                    if (tGridView.Bands[i].Caption.IndexOf(gunId.ToString() + ". İş Günü") > -1)
                    {
                        column.GroupIndex = i;
                        column.VisibleIndex = line_no;
                        tGridView.Bands[i].Columns.Add(column);
                    }
                }
            }

            if (gunId == 0)
            {
                tGridView.Bands[gunId].Columns.Add(column);
            }
        }

        private void tDataSource_Add(PanelControl panelControl, GridControl tGridControl, Int16 toplamIsGunuSayisi, saatTipi _saatTipi, int RefId)
        {
            //-- Grid üzerinde view için 
            ds_SablonS = new DataSet();
            dN_SablonS = new DataNavigator();
            //-- Grid üzerine girilen bilgilerin database aktarılması için
            ds_SablonSSave = new DataSet();
            dN_SablonSSave = new DataNavigator();

            #region Grid View için
            string tableName = "";
            string fields = "";
            string leftJoin = "";
            Int16 gunId = 1;
            while (gunId <= toplamIsGunuSayisi)
            {
                fields += preaparingFields(gunId);
                leftJoin += preparingLeftJoin(RefId, gunId);
                gunId++;
            }

            string tSql = "";

            if (_planTipi == planTipi.teorik)
                tSql = @" 
 Select 
  saat.Id       as Lkp_SaatId 
 ,saat.SaatTipi as Lkp_SaatTipi 
" + fields + @"
  From Lkp.MtskSablonTeorikSSaatTipi as saat 
" + leftJoin + @" 
  Where saat.Id > 100
  and   saat.GrupTipiId = " + Convert.ToString((byte)_saatTipi);
            

            if (_planTipi == planTipi.uygulama)
                tSql = @" 
 Select 
  saat.Id       as Lkp_SaatId 
 ,saat.SaatTipi as Lkp_SaatTipi 
" + fields + @"
  From Lkp.MtskSablonUygulamaSSaatTipi as saat 
" + leftJoin + @" 
  Where saat.GrupTipiId = " + Convert.ToString((byte)_saatTipi);

            if (_planTipi == planTipi.teorik) tableName = "MtskSablonTeorikS";
            if (_planTipi == planTipi.uygulama) tableName = "MtskSablonUygulamaS";


            string myProp = string.Empty;
            t.MyProperties_Set(ref myProp, "DBaseNo", _dBaseNo);
            t.MyProperties_Set(ref myProp, "TableName", tableName);
            t.MyProperties_Set(ref myProp, "SqlFirst", tSql);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");

            //t.Preparing_DataSet(dsData, vt);

            ds_SablonS.Namespace = myProp;

            try
            {
                t.Data_Read_Execute(this, ds_SablonS, ref tSql, tableName, null);
            }
            catch (Exception)
            {
                throw;
            }

            if (ds_SablonS != null)
                if (ds_SablonS.Tables.Count > 0)
                {
                    tGridControl.DataSource = ds_SablonS.Tables[0];
                    dN_SablonS.DataSource = ds_SablonS.Tables[0];
                    dN_SablonS.PositionChanged += new System.EventHandler(dataNavigatorS_PositionChanged);
                    dN_SablonS.Tag = 0;
                }
            #endregion Grid View için

            #region Database için

            if (_planTipi == planTipi.teorik)
                tSql = @" Select * From MtskSablonTeorikS Where IsActive = 0 and SablonTeorikBId = " + RefId.ToString();

            if (_planTipi == planTipi.uygulama)
                tSql = @" Select * From MtskSablonUygulamaS Where IsActive = 0 and SablonUygulamaBId = " + RefId.ToString();

            myProp = string.Empty;
            t.MyProperties_Set(ref myProp, "DBaseNo", _dBaseNo);
            t.MyProperties_Set(ref myProp, "TableName", tableName);
            t.MyProperties_Set(ref myProp, "SqlFirst", tSql);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");
            t.MyProperties_Set(ref myProp, "TableType", "1");
            t.MyProperties_Set(ref myProp, "Cargo", "data");
            t.MyProperties_Set(ref myProp, "KeyFName", "Id");

            ds_SablonSSave.Namespace = myProp;

            try
            {
                t.Data_Read_Execute(this, ds_SablonSSave, ref tSql, tableName, null);
            }
            catch (Exception)
            {
                throw;
            }

            if (ds_SablonSSave != null)
                if (ds_SablonSSave.Tables.Count > 0)
                {
                    dN_SablonSSave.DataSource = ds_SablonSSave.Tables[0];
                }

            #endregion Database için


            DevExpress.XtraEditors.PanelControl NavPanel = new DevExpress.XtraEditors.PanelControl();
            NavPanel.Name = "tPanel_Navigator_" + RefId.ToString();
            NavPanel.Height = 26;
            NavPanel.Dock = DockStyle.Bottom;
            NavPanel.SendToBack();
            NavPanel.TabIndex = 1000;
            NavPanel.TabStop = true;

            NavPanel.Controls.Add(dN_SablonS);
            NavPanel.Controls.Add(dN_SablonSSave);

            panelControl.Controls.Add(tGridControl);
            panelControl.Controls.Add(NavPanel);
        }
        
        private string preaparingFields(Int16 gunId)
        {
            string x = gunId.ToString();
            string fields = "";

            if (_planTipi == planTipi.teorik)
                fields = ""
                + " ,gun" + x + ".Id                as gun" + x + "_Id " + v.ENTER
                + " ,gun" + x + ".SablonTeorikBId   as gun" + x + "_SablonTeorikBId " + v.ENTER
                + " ,gun" + x + ".IsActive          as gun" + x + "_IsActive " + v.ENTER
                + " ,gun" + x + ".GunId             as gun" + x + "_GunId " + v.ENTER
                + " ,gun" + x + ".SaatTipiId        as gun" + x + "_SaatTipiId " + v.ENTER
                + " ,gun" + x + ".GrupTipiId        as gun" + x + "_GrupTipiId " + v.ENTER
                + " ,gun" + x + ".SubeTipiId        as gun" + x + "_SubeTipiId " + v.ENTER
                + " ,gun" + x + ".DerslikTipiId     as gun" + x + "_DerslikTipiId " + v.ENTER
                + " ,gun" + x + ".DerslikAdiTipiId  as gun" + x + "_DerslikAdiTipiId " + v.ENTER
                + " ,gun" + x + ".DersSaatiTipiId   as gun" + x + "_DersSaatiTipiId " + v.ENTER
                + " ,gun" + x + ".EgitimTipiId      as gun" + x + "_EgitimTipiId " + v.ENTER
                + " ";

            if (_planTipi == planTipi.uygulama)
                fields = ""
                + " ,gun" + x + ".Id                as gun" + x + "_Id " + v.ENTER
                + " ,gun" + x + ".SablonUygulamaBId as gun" + x + "_SablonUygulamaBId " + v.ENTER
                + " ,gun" + x + ".IsActive          as gun" + x + "_IsActive " + v.ENTER
                + " ,gun" + x + ".GunId             as gun" + x + "_GunId " + v.ENTER
                + " ,gun" + x + ".SaatTipiId        as gun" + x + "_SaatTipiId " + v.ENTER
                + " ,gun" + x + ".AracTipiId        as gun" + x + "_AracTipiId " + v.ENTER
                + " ,gun" + x + ".OgreticiTipiId    as gun" + x + "_OgreticiTipiId " + v.ENTER
                + " ,gun" + x + ".AdayTipiId        as gun" + x + "_AdayTipiId " + v.ENTER
                + " ";

            return fields;
        }

        private string preparingLeftJoin(int SablonBId, Int16 gunId)
        {
            string x = gunId.ToString();
            string leftjoin = "";

            if (_planTipi == planTipi.teorik)
                leftjoin = "    left outer join MtskSablonTeorikS as gun" + x + " on ( saat.Id = gun" + x + ".SaatTipiId and gun" + x + ".GunId = " + x + " and gun" + x + ".SablonTeorikBId = " + SablonBId.ToString() + " and gun" + x + ".IsActive = 1 ) " + v.ENTER;

            if (_planTipi == planTipi.uygulama)
                leftjoin = "    left outer join MtskSablonUygulamaS as gun" + x + " on ( saat.Id = gun" + x + ".SaatTipiId and gun" + x + ".GunId = " + x + " and gun" + x + ".SablonUygulamaBId = " + SablonBId.ToString() + " and gun" + x + ".IsActive = 1  ) " + v.ENTER;

            return leftjoin;
        }
        
        private void preparingAracPlani(Int16 toplamIsGunuSayisi)
        {
            if (ds_SablonS == null) return;

            aracTipi _aracTipi = uygSablonB.aracTipiId;
            string x = "";
            string fieldName = "";
            Int16 gunId = 1;
            Int16 value = 0;

            for (int i = 0; i < this.ds_SablonS.Tables[0].Rows.Count; i++)
            {
                gunId = 1;
                while (gunId <= toplamIsGunuSayisi)
                {
                    if ((_aracTipi == aracTipi.simülatorArac) && (gunId == 1))
                        value = 1;
                    else value = 2;
                    x = gunId.ToString();
                    fieldName = "gun" + x + "_AracTipiId";
                    this.ds_SablonS.Tables[0].Rows[i][fieldName] = value;
                    gunId++;
                }
            }
        }

        #endregion Teorik ve Uygulama Sablon Hazirlama Ortak Fonksiyonlar 1



        private void InsertPaketOlustur()
        {
            if (t.IsNotNull(ds_SablonB) == false) return;


            // _planTipi = planTipi.uygulama;

            string myProp = ds_SablonB.Namespace;
            string databaseName = t.MyProperties_Get(myProp, "DBaseName:");
            string schemaName = t.MyProperties_Get(myProp, "SchemasCode:");


            string soru = " Şablonlar için INSERT paketi oluşturulacak, Onaylıyor musunuz ?";
            DialogResult cevap = t.mySoru(soru);
            if (DialogResult.Yes == cevap)
            {
                cumleBaslik = "";
                cumleSatirlar = "";

                if (_planTipi == planTipi.teorik)
                {
                    cumleBaslik = preparingInsertScript(databaseName, schemaName, "MtskSablonTeorikB");
                    cumleSatirlar = preparingInsertScript(databaseName, schemaName, "MtskSablonTeorikS");
                }
                if (_planTipi == planTipi.uygulama)
                {
                    cumleBaslik = preparingInsertScript(databaseName, schemaName, "MtskSablonUygulamaB");
                    cumleSatirlar = preparingInsertScript(databaseName, schemaName, "MtskSablonUygulamaS");
                }

                PaketiGonder();
            }
        }

        private string preparingInsertScript(string databaseName, string schemaName, string tableName)
        {
            vScripts scripts = new vScripts();

            scripts.SourceDBaseName = databaseName;
            scripts.SchemaName = schemaName;
            scripts.SourceTableName = tableName;
            scripts.TableIPCode = "";
            scripts.Where = " 0 = 0 ";
            scripts.IdentityInsertOnOff = true;

            string cumle = t.preparingInsertScript(scripts);

            return cumle;
        }

        private void PaketiGonder()
        {
            if (cumleBaslik != "") t.runScript(v.dBaseNo.publishManager, cumleBaslik);
            if (cumleSatirlar != "") t.runScript(v.dBaseNo.publishManager, cumleSatirlar);

            t.FlyoutMessage(this, "Web Manager Database Update", "Insert paketler gönderildi...");
        }



    }
}
