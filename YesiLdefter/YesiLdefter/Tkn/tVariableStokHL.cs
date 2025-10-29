using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tkn_VariableStokHL
{
    public static class vs
    {
        public static faturaGonderici tfaturaGonderici = new faturaGonderici();
        public static stokB tStokB = new stokB();
        public static stokS tStokS = new stokS();
    }

    public class stokB
    {
        public stokB()
        {
            Clear();
        }

        public int Id { get; set; }
        public int ParentId { get; set; }
        public int FirmId { get; set; }
        public bool IsActive { get; set; }
        public bool IsPaid { get; set; } /* default : 0 = ödenmedi,  1 = Ödendi */
        public bool IsCommit { get; set; } /* default : 0 = Muh. işlenmedi, 1 = Muh. İşlendi */
        public bool IsLock { get; set; } /* default : 0 = Açık, 1 = Kilitli  */

        //-- belge tip bilgileri
        //-- 10
        public Int16 GCTipiId { get; set; } /* 0 = etkisiz, 1 = Giriş Hareketi, 2 = Çıkış Hareketi */
        public Int16 BelgeTipiId { get; set; } /* Yeşildefter kontrolü için gerekli  */
        public Int16 TuruTipiId { get; set; } /* 1. Sipariş, 2. İrsaliye, 3. Fatura   */
        public Int16 MuhasebeTipiId { get; set; } /* 25. Maddi duran varlık alımı, 601. Yurtdışı satışlar vb... */

        //-- 20
        public Int16 FProfiliTipiId { get; set; } /* TICARI, TEMEL, IHRACAT, YOLCUBERABERFAT */
        public Int16 FaturaTipiId { get; set; } /* e-Fatura/e-Arşiv için gerekli : TICARIFATURA, ... */
        public Int16 IlaveFaturaTipiId { get; set; } /* e-Fatura/e-Arşiv için gerekli : SAGLIK_ECZ, ...   */
        public string GIBGUID { get; set; } /* uniqueidentifier  null, */
        public string GIBEttn { get; set; }
        public string GTBKodu { get; set; }
        public string GIBInvoiceScenario { get; set; }
        
        //-- 30
        public Int16 GonderimTipiId { get; set; } /* 1. elektronik belge, 2. kağıt  */
        public int InternetSiteId { get; set; } /* e-fatura üzerinde satış hangi site üzerinden yapıldığı bildiriliyor : firmanın satış yaptığı internet siteleri tablosu henüz yok */
        public Int16 OdemeTipiId { get; set; } /* KREDIKARTI/BANKAKARTI, ...   */
        public string OdemeTipiAciklama { get; set; } /* Ödeme tipi diğer seçildiğinde doldurulamsı zorunlu alan  */
        public Int16 OdemeAraciTipiId { get; set; } /* Firmanın kullandığı ödeme aracısı olarak kullandığı Aracılar : BKM EXPRESS, ... tablosu henüz yok */
        public DateTime OdemeTarihi { get; set; } /* ödemenin alındığı tarih */

        //--fatura bilgileri
        //-- 50 Belge Hakkında
        public DateTime BelgeTarihi { get; set; }
        public string BelgeSaati { get; set; }
        public string BelgeSeri { get; set; }
        public string BelgeNo { get; set; }

        //-- 60
        public Int16 VadeGun { get; set; }
        public DateTime SonOdemeTarihi { get; set; }
        public DateTime GecerlilikTarihi { get; set; } /* sigortanın son tarihi */
        public string FaturaNotu { get; set; } /* e-fatura */ /*---Aciklama           nVarChar(100) null, iptal */

        //-- 70
        public string ParaTipiId { get; set; } /* belgenin para birimi */
        public Int16 DovizYeriId { get; set; }
        public decimal DovizKuru { get; set; }

        //-- cari bilgisi : Üün/Hizmet alan cari
        //-- 80
        public Int16 CariTipiId { get; set; } /* 1. Gerçek Kişi, 2. Özel Firma, 3. Kamu Kurumu */
        public int TalepId { get; set; }
        public int CariId { get; set; }
        public string CariUnvan { get; set; } /* cari hesabı yok ise */
        public string CariTcVkNo { get; set; }
        public string CariVergiDairesiKodu { get; set; }  /*-- SaymanlikKodu */
        public string CariVergiDairesiAdi { get; set; }
        public string CariTelefonu { get; set; }
        public string CariEMail { get; set; }
        public string CariEFaturaAliasName { get; set; }
        public string CariExtraInformation { get; set; }

        //-- 90 : fatura üzerinde görünecek
        public string CariUlkeAdi { get; set; }
        public string CariIl { get; set; }
        public string CariIlce { get; set; }
        public string CariMahalleKoy { get; set; }
        public string CariCaddeSokak { get; set; }
        public string CariDisKapiNo { get; set; }
        public string CariIcKapiNo { get; set; }

        //-- Ödemeyi yapacak cari : Ürün/hizmeti alan kurum farklı, ödemeyi yapan kurum farklı oluyor : Örnek : xxxx fakültei mal aldı, yyyy saymanlığı ödeyecek
        //-- 110 
        public int OdeyecekCariId { get; set; } /* saymanlık carid Id */
        public int OdeyecekAdresId { get; set; } /* saymanlık adresi */
        public int TeslimatAdresId { get; set; } /* -- satın alan farklı, ödeyecek farklı, bir de teslim yeri farklı ise kulanılacak */
        public int KargoCariId { get; set; } /* Kargo şirketi */
        public DateTime GonderimTarihi { get; set; } /* kargoya verildiği tarih */


        //-- 110 irsaliye için detail tablosu hazırlanacak
        //--sipariş bilgisi
        //--irsaliye bilgisi
        //--ökc bilgisi

        //-- 130 loglama için kullan
        public DateTime KayitTarihi { get; set; }

        //-- Satırların toplamı
        //-- 200 
        public decimal FsIOVergisizToplamTutar { get; set; }  //* LineExtensionAmount   : İskonto öncesi  Birim fiyat x miktar : vergisiz */
        public decimal FsIskontoTutariToplam { get; set; }    /* AllowanceTotalAmount  : İskontolar toplamı */
        public decimal FsISVergisizToplamTutar { get; set; }  /* TaxExclusiveAmount    : İskonto sonrası Birim fiyat x miktar : vergisiz */
        public decimal FsEkVergiTutari { get; set; }
        public decimal FsOtvTutari { get; set; }

        public decimal FsKdv1Matrahi { get; set; }
        public decimal FsKdv1Orani { get; set; }
        public decimal FsKdv1Tutari { get; set; }

        public decimal FsKdv2Matrahi { get; set; }
        public decimal FsKdv2Orani { get; set; }
        public decimal FsKdv2Tutari { get; set; }

        public decimal FsKdv3Matrahi { get; set; }
        public decimal FsKdv3Orani { get; set; }
        public decimal FsKdv3Tutari { get; set; }

        public decimal FsKdvToplami { get; set; }
        public decimal FsKdv0Matrahi { get; set; }


        public decimal FsToplamTutarNormalKdvli { get; set; }    /* ISKdvMatrahi + ISKdvTutari */
        public decimal FsStopajTutari { get; set; }              /* ISVergisizToplamTutar * %StopajOrani */
        public decimal FsToplamTutarNormalKdvliNet { get; set; } /* ( ISKdvMatrahi + ISKdvTutari) - ISStopajTutari */


        public decimal FsTevkifatTutariAliciyaAit { get; set; }  /* ( ISKdvTutari / Tevkifat Paydası )  x Tevkifat Payı  veya (( TevPayı / TevPaydası ) * ISKdvTutari   */
        public decimal FsHesaplananKdvSaticiyaAit { get; set; }  /*   ISKdvTutari - ISTevkifatTutariAliciyaAit   */
        public decimal FsToplamTutarTevkifatKdvli { get; set; }  /*   ISKdvMatrahi + ISHesaplananKdvSaticiyaAit  */

        public decimal FsVergilerDahilToplamTutar { get; set; }  /* genel Toplam */ /*  ya FSToplamTutarNormalKdvliNet yada FSToplamTutarTevkifatKdvli  atanacak  */
        public decimal FsYuvarlamaTutari { get; set; }


        //-- Lkp fileds list
        public string Lkp_FProfiliTipi { get; set; }
        public string Lkp_FaturaTipi { get; set; }
        public string Lkp_ParaTipi { get; set; }
        public string Lkp_GCTipi { get; set; }
        public string Lkp_BelgeTipi { get; set; }
        public string Lkp_TuruTipi { get; set; }
        public string Lkp_MuhasebeTipi { get; set; }

        //-- Join fields list
        public string Lkp_InternetSitesi { get; set; }
        public string Lkp_OdemeAraciTipi { get; set; }
        public string Lkp_OdemeTipi { get; set; } /* 1  KREDIKARTI/BANKAKARTI, 2   EFT/HAVALE, 3   KAPIDAODEME, 4   ODEMEARACISI */
        public string Lkp_KargoAdi { get; set; }
        public string Lkp_KargoVKN { get; set; }


        //-- 
        public string CariAdi
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CariUnvan)) return string.Empty;
                var parts = CariUnvan.Trim().Split(' ');
                if (parts.Length < 2) return CariUnvan; // Soyad yok gibi davran
                return string.Join(" ", parts.Take(parts.Length - 1));
            }
        }
        public string CariSoyadi
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CariUnvan)) return string.Empty;
                var parts = CariUnvan.Trim().Split(' ');
                if (parts.Length < 2) return string.Empty;
                return parts.Last();
            }
        }
        public void Clear()
        {

        }

    }

    public class stokS
    {
        public stokS()
        {
            Clear();
        }

        public int Id { get; set; }
        public int ParentId { get; set; }
        public int FirmId { get; set; }
        public int BelgeStokBId { get; set; } /* BelgeStokB.Id  : BelgeStokB başlık tablosu */

        //-- 10
        public int TalepId { get; set; }
        public int CariId { get; set; } /* HesapCariId    : Cari Hesap kartı */
        public int StokId { get; set; } /* HesapStok.Id   : Stok veya Hizmet kartı */
        public int VarlikId { get; set; } /* HesapFinans.Id : Varlık hesabı için */
   
        //-- 20 belge tip bilgileri
        public Int16 GCTipiId { get; set; } /* 0 = etkisiz, 1 = Giriş Hareketi, 2 = Çıkış Hareketi */
        public Int16 BelgeTipiId { get; set; } /*   */
        public Int16 MuhasebeTipiId { get; set; } /*   */
        public Int16 TuruTipiId { get; set; } /* 1. Sipariş, 2. İrsaliye, 3. Fatura   */

        //-- 30
        public DateTime IslemTarihi { get; set; }

        //-- 40   
        public Int16 SatirNo { get; set; }
        public string UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public string Aciklama { get; set; }
        public string SatirNotu { get; set; }

        //-- 50
        public string GIPKodu { get; set; } /* (Gelir İdaresi Paket Kodu), mal ve hizmetlerin vergisel sınıflandırılması için kullanılan 8 haneli bir koddur. */
        public string GTIPKodu { get; set; } /* (Gümrük Tarife İstatistik Pozisyonu) */
        public string UreticiUrunKodu { get; set; }
        public string SaticiUrunKodu { get; set; }
        public string AliciUrunKodu { get; set; }


        //-- 60
        public decimal Miktar { get; set; }
        public decimal GCMiktar { get; set; }
        public string BirimTipiId { get; set; }

        //-- 70
        public string OTVKodu { get; set; }
        public decimal OTVOrani { get; set; }
        public decimal OTVBirimTutari { get; set; }

        //-- 80
        public Int16 KdvTipiId { get; set; }
        public decimal KdvOrani { get; set; }
        public string MuafiyetKodu { get; set; }
        public string IstisnaKodu { get; set; }
        public decimal IstisnaOrani { get; set; }

        //-- 90
        public string ParaTipiId { get; set; } /* belgenin para birimi */
        public Int16 DovizYeriId { get; set; }
        public decimal DovizKuru { get; set; }

        //--110
        public decimal dBirimFiyati { get; set; }
        public decimal dBirimOTVliTutari { get; set; }
        public decimal dBirimOTVKdvliFiyati { get; set; }
        public decimal dToplamTutar { get; set; }
        public decimal dToplamOTViTutari { get; set; }
        public decimal dToplamOTVKdvliTutar { get; set; }
        public decimal dOvtTutari { get; set; }
        public decimal dKdvTutari { get; set; }


        //--120
        public decimal BirimFiyati { get; set; }
        public decimal BirimOTVliTutari { get; set; }
        public decimal BirimOTVKdvliFiyati { get; set; }
        public decimal ToplamTutar { get; set; }
        public decimal ToplamOTViTutari { get; set; }
        public decimal ToplamOTVKdvliTutar { get; set; }
        public decimal OvtTutari { get; set; }
        public decimal KdvTutari { get; set; }

        //--130   
        public decimal IskontoOrani1 { get; set; }
        public decimal IskontoOrani2 { get; set; }
        public decimal IskontoOrani3 { get; set; }
        public decimal IskontoOrani4 { get; set; }
        public decimal IskontoOraniToplam { get; set; }

        //--140
        public decimal IskontoTutari1 { get; set; }
        public decimal IskontoTutari2 { get; set; }
        public decimal IskontoTutari3 { get; set; }
        public decimal IskontoTutari4 { get; set; }
        public decimal IskontoTutariToplam { get; set; }

        //-- 150
        public string IskontoAciklamasi1 { get; set; }
        public string IskontoAciklamasi2 { get; set; }
        public string IskontoAciklamasi3 { get; set; }
        public string IskontoAciklamasi4 { get; set; }


        //-- 160 
        public decimal ArtiUcretOrani1 { get; set; }
        public decimal ArtiUcretOrani2 { get; set; }
        public decimal ArtiUcretOrani3 { get; set; }
        //-- 170
        public decimal ArtiUcret1 { get; set; }
        public decimal ArtiUcret2 { get; set; }
        public decimal ArtiUcret3 { get; set; }
        //-- 180
        public string ArtiUcretAciklamasi1 { get; set; } 
        public string ArtiUcretAciklamasi2 { get; set; } 
        public string ArtiUcretAciklamasi3 { get; set; }



        //-- 190
        public decimal TevkifatKodu { get; set; }
        public decimal TevkifatPayi { get; set; } /* 2, 1 - 9, %90,  */
        public decimal TevkifatPaydasi { get; set; }   /* 3, 10,    100   */
        public decimal TevkifatOrani { get; set; } /* 2/3 , 2/10 ... 9/10 veya %90 gibi açıklamalar taşır  */


        //-- 200
        public string StopajKodu { get; set; }       /* Devletin Stopaj Listesi */
        public string StopajAciklamasi { get; set; } /* Devletin Stopaj Listesi */
        public decimal StopajOrani { get; set; }     /* listeden de gelebilir, kullanıca girebilmeli */



        //-- 210	
        public decimal dISBirimFiyati { get; set; }
        public decimal dISBirimFiyatiOtvli { get; set; }
        public decimal dISBirimFiyatiOtvKdvli { get; set; }


        //-- 220
        public decimal dISVergisizToplamTutar { get; set; }
        public decimal dISEkVergiTutari { get; set; } 
        public decimal dISOtvTutari { get; set; }
        public decimal dISKdvMatrahi { get; set; } 
        public decimal dISKdvTutari { get; set; }
        public decimal dISToplamTutarNormalKdvli { get; set; }
        public decimal dISStopajTutari { get; set; }
        public decimal dISToplamTutarNormalKdvliNet { get; set; }

        //-- 230
        public decimal dISTevkifatTutariAliciyaAit { get; set; }
        public decimal dSHesaplananKdvSaticiyaAit { get; set; }
        public decimal dISToplamTutarTevkifatKdvli { get; set; }


        //-- IS  : Iskonto Sonrası
        //-- 260 : TL cinsinden
        public decimal ISBirimFiyati { get; set; }         /* birim fiyatı : 100 tl */
        public decimal ISBirimFiyatiOtvli { get; set; }    /* ( birim fiyatı + ( birim fiyatı x %OtvOrani ) )  veya ( birim fiyatı + OtvBirimTutari )  */
        public decimal ISBirimFiyatiOtvKdvli { get; set; } /* ISBirimFiyatiOtvli + (( ISBirimFiyatiOtvli / 100 ) x %KdvOranı ) */

        //-- 270
        public decimal ISVergisizToplamTutar { get; set; }       /* Birim fiyat x miktar : vergisiz */
        public decimal ISEkVergiTutari { get; set; }             /* = sum(IBelgeStokSEkVergi.HesaplananVergi)  */
        public decimal ISOtvTutari { get; set; }                 /* OtvBirimTutari varsa ? ( OTVBirimTutari x Miktar )  : ( ( ISBirimFiyati x %OTVOrani ) x Miktar )     */
        public decimal ISKdvMatrahi { get; set; }                /* ISToplamTutar + ISEkVergiTutari + ISOtvTutari */
        public decimal ISKdvTutari { get; set; }                 /* ( ISKdvMatrahi / 100 ) x Kdv Oranı */
        public decimal ISToplamTutarNormalKdvli { get; set; }    /*   ISKdvMatrahi + ISKdvTutari */
        public decimal ISStopajTutari { get; set; }              /* ISVergisizToplamTutar * %StopajOrani */
        public decimal ISToplamTutarNormalKdvliNet { get; set; } /* ( ISKdvMatrahi + ISKdvTutari) - ISStopajTutari */


        //-- 280
        public decimal ISTevkifatTutariAliciyaAit { get; set; } /* ( ISKdvTutari / Tevkifat Paydası )  x Tevkifat Payı  veya (( TevPayı / TevPaydası ) * ISKdvTutari  */
        public decimal ISHesaplananKdvSaticiyaAit { get; set; } /* ISKdvTutari - ISTevkifatTutariAliciyaAit */
        public decimal ISToplamTutarTevkifatKdvli { get; set; } /* ISKdvMatrahi + ISHesaplananKdvSaticiyaAit */


        //-- 290 
        public decimal dISVergilerDahilToplamTutar { get; set; }  /*  ya ISToplamTutarNormalKdvli yada ISToplamTutarTevkifatKdvli  atanacak  */
        public decimal ISVergilerDahilToplamTutar { get; set; }   /*  ya ISToplamTutarNormalKdvli yada ISToplamTutarTevkifatKdvli  atanacak  */


        //-- 310
        //--FiyatGirisTipiId SmallInt      null, gerek kalmadı
        //--IskontoTipiId SmallInt      null, /* iskonto oran mı?, tutar mı?, para eksik tahsilatı mı ?  */
        public decimal MaxIskontoOrani { get; set; } /* Fiyat tablosundan geliyor */

        //--330
        public DateTime KayitTarihi { get; set; }


        public void Clear()
        {

        }

    }


    public class faturaGonderici
    {
        public faturaGonderici()
        {
            Clear();
        }

        public string Unvan { get; set; }
        public string VergiDairesiAdi { get; set; }
        public string Vkn { get; set; }
        public string MersisNo { get; set; }
        public string TicaretSicilNo { get; set; }

        public string Ulke { get; set; }
        public string Il { get; set; }
        public string Ilce { get; set; }
        public string MahhalleKoy { get; set; }
        public string CaddeSokak { get; set; }
        public string DisKapiNo { get; set; }
        public string IcKapiNo { get; set; }

        public void Clear()
        {
        }
    }

    /*
                        PartyName = new PartyNameType { Name = new NameType1 { Value = txtGondericiUnvan.Text } },
                        PartyIdentification = new PartyIdentificationType[] { 
                            new PartyIdentificationType() { ID = new IDType { Value = txtGondericiVkn.Text, schemeID = "VKN" } }, 
                            new PartyIdentificationType() { ID = new IDType { Value = "12345669-111", schemeID = "MERSISNO" } }, 
                            new PartyIdentificationType() { ID = new IDType { Value = "12345669-111", schemeID = "TICARETSICILNO" } } },

                        PostalAddress = new AddressType
                        {
                            CityName = new CityNameType { Value = txtGondericiIl.Text },
                            StreetName = new StreetNameType { Value = txtGondericiCaddeSokak.Text },
                            Country = new CountryType { Name = new NameType1 { Value = txtGondericiUlke.Text } },
                            Room = new RoomType { Value = txtGondericiKapiNo.Text },
                            BuildingNumber = new BuildingNumberType { Value = txtGondericiKapiNo.Text },
                            CitySubdivisionName = new CitySubdivisionNameType { Value = txtGoncericiIlce.Text },
                        },
                        // PartyIdentification = new PartyIdentificationType[] { new PartyIdentificationType() { ID = new IDType { Value = "77777777701", schemeID = "TCKN" } } },
                        // Person = new PersonType{ FirstName= new FirstNameType{ Value="Ahmet"}, FamilyName= new FamilyNameType{ Value="Altınordu"} },
                        // PartyTaxScheme = new PartyTaxSchemeType { TaxScheme = new TaxSchemeType { Name = new NameType1 { Value = "Esenler" } } },
                        PartyTaxScheme = new PartyTaxSchemeType { TaxScheme = new TaxSchemeType { Name = new NameType1 { Value = txtGondericiVergiDairesi.Text } } },

    */

}
