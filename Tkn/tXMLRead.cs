using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.IO;

using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_XMLRead
{
    class tXMLRead
    {
        #region Tanımlar 
        
        public static string SPF_XML_FILENAME = "ManagerSetup";
        public static string SPF_SERVER_PCNAME = string.Empty;
        public static string SPF_PROJE_DBNAME = string.Empty;
        public static string SPF_MANAGER_DBNAME = string.Empty;
        public static string SPF_MAINMANAGER_DBNAME = "SystemMS";

        public static string SPF_USER_NAME = string.Empty;
        public static string SPF_PASSWORD = string.Empty;
        public static string SPF_MENU = string.Empty;
        public static string SPF_VT_FIRM = string.Empty;
        public static string SPF_VT_FIRMSERIAL = string.Empty;


        //: xml başlıkları SP ler       
        public static string xml_SERVER_PCNAME = "SERVER_PCNAME";
        public static string xml_USERNAME = "USERNAME";
        public static string xml_PASSWORD = "PASSWORD";
        public static string xml_PROJE_DBNAME = "PROJE_DBNAME";
        public static string xml_MANAGER = "MANAGER_DBNAME";
        public static string xml_MAINMANAGER = "MAINMANAGER_DBNAME";

        public static string xml_MENU = "MENU";
        public static string xml_VT_FIRM = "VT_FIRM";
        public static string xml_VT_FIRMSERIAL = "VT_99";

        public static byte XML_READ_SP = 0;
        public static byte MSSQL_SP = 1;

        public enum SP_atama : byte { XML_READ_SP, MSSQL_SP }
        public static string Uretici_Firma_01 = "Ankara Türkiye";

        #endregion Tanımlar

        #region tXML 

        public void XML_UyapCreate(string filename, 
            DataSet dsIcraDosyasi, int dNIcraDosyasi_RowId)
        {
            XmlDocument XMLdoc = new XmlDocument();
            XmlNode docNode = XMLdoc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            XMLdoc.AppendChild(docNode);

            XmlNode docComment = XMLdoc.CreateComment(Uretici_Firma_01);
            XMLdoc.AppendChild(docComment);

            XmlNode exchange = XMLdoc.CreateElement("exchangeData");
            XMLdoc.AppendChild(exchange);

            XmlNode exchangeHeader = XMLdoc.CreateElement("exchangeHeader");
            XmlAttribute versiyon = XMLdoc.CreateAttribute("versiyon");
            versiyon.Value = "1.2";
            exchangeHeader.Attributes.Append(versiyon);

            exchange.AppendChild(exchangeHeader);
            
            //---

            XmlNode VekilKisi = UyapXML_VekilKisi_Preparing(XMLdoc);
            XmlNode taraf1 = UyapXML_taraf_Preparing(XMLdoc, "1");
            XmlNode taraf2 = UyapXML_taraf_Preparing(XMLdoc, "2");

            XmlNode dosya = UyapXML_dosya_Preparing(XMLdoc, dsIcraDosyasi, dNIcraDosyasi_RowId);
                    dosya.AppendChild(VekilKisi);
                    dosya.AppendChild(taraf1);
                    dosya.AppendChild(taraf2);
            
            XmlNode dosyalar = XMLdoc.CreateElement("dosyalar");
                    dosyalar.AppendChild(dosya);
            
            exchange.AppendChild(dosyalar);
                        
            XMLdoc.Save(@"" + filename + ".xml");

            // create a tab page
            //XmlExplorerTabPage tabPage = new XmlExplorerTabPage();
        }

        
        private XmlNode UyapXML_dosya_Preparing(XmlDocument XMLdoc, DataSet dsData, int RowId)
        {

            #region 
            /*
            < !ELEMENT dosya(cek | senet | taraf | VekilKisi | police | kontratKefil | digerAlacak | evrak | ref | ilam) * >
            < !ATTLIST dosya
                id                         ID #IMPLIED
                dosyaTipi                  CDATA #REQUIRED
                dosyaTuru                  (0) #IMPLIED
                takipTuru(0 | 1) "1"
                takipYolu(0 | 1 | 2 | 3 | 4 | 5) #IMPLIED
                takipSekli(0 | 1 | 2 | 3 | 4 | 5 | 6) #IMPLIED						
                alacaklininTalepEttigiHak   CDATA #IMPLIED
                BK84MaddeUygulansin(E | H) "H"
                BSMVUygulansin(E | H) "H"
                KKDFUygulansin(E | H) "H"
                aciklama48e9                CDATA #IMPLIED
                dosyaBelirleyicisi          CDATA #IMPLIED
                mahiyetKodu(1007 | 1107 | 1207 | 1307 | 1407 | 2007 | 3007 | 4007 | 5007 | 6007 | 7007 | 8008 | 9009 | 1045 | 2045 | 3045 | 4045) #IMPLIED
            >
            */
            #endregion 

            tToolBox t = new tToolBox();
                         

            XmlNode dosya = XMLdoc.CreateElement("dosya");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "0";

            XmlAttribute dosyaTipi = XML_Value_Preparing(XMLdoc, dsData, RowId, "dosyaTipi");//XMLdoc.CreateAttribute("dosyaTipi");
            //dosyaTipi.Value = "Genel İcra Dairesi";

            XmlAttribute takipTuru = XML_Value_Preparing(XMLdoc, dsData, RowId, "takipTuru");//XMLdoc.CreateAttribute("takipTuru");
            //takipTuru.Value = "0";

            XmlAttribute takipYolu = XML_Value_Preparing(XMLdoc, dsData, RowId, "takipYolu");//XMLdoc.CreateAttribute("takipYolu");
            //takipYolu.Value = "1";

            XmlAttribute takipSekli = XML_Value_Preparing(XMLdoc, dsData, RowId, "takipSekli");//XMLdoc.CreateAttribute("takipSekli");
            //takipSekli.Value = "0";

            XmlAttribute alacaklininTalepEttigiHak = XML_Value_Preparing(XMLdoc, dsData, RowId, "alacaklininTalepEttigiHak");//XMLdoc.CreateAttribute("alacaklininTalepEttigiHak");
            //alacaklininTalepEttigiHak.Value = "Toplam tutardaki alacağın (asıl alacak ile faizinin fazlaya dair taleplerimiz ve haklarımız  ile sair kanuni haklarımız saklı kalmak kaydı ile) tahsil tarihine kadar asıl alacağa işleyecek faiz, masraf ve vekalet ücretinin tahsili talebi ile kısmi ödemelerin (B.K. m:100'e göre) öncelikle faize,  masraflara ve ücret-i vekalete mahsuben hesaplanması talebimizdir.";

            XmlAttribute BK84MaddeUygulansin = XML_Value_Preparing(XMLdoc, dsData, RowId, "BK84MaddeUygulansin");//XMLdoc.CreateAttribute("BK84MaddeUygulansin");
            //BK84MaddeUygulansin.Value = "H";

            XmlAttribute BSMVUygulansin = XML_Value_Preparing(XMLdoc, dsData, RowId, "BSMVUygulansin");//XMLdoc.CreateAttribute("BSMVUygulansin");
            //BSMVUygulansin.Value = "H";

            XmlAttribute KKDFUygulansin = XML_Value_Preparing(XMLdoc, dsData, RowId, "KKDFUygulansin");//XMLdoc.CreateAttribute("KKDFUygulansin");
            //KKDFUygulansin.Value = "H";
            
            XmlAttribute aciklama48e9 = XML_Value_Preparing(XMLdoc, dsData, RowId, "aciklama48e9");//XMLdoc.CreateAttribute("aciklama48e9");
            //aciklama48e9.Value = "Haciz";

            XmlAttribute dosyaBelirleyicisi = XML_Value_Preparing(XMLdoc, dsData, RowId, "dosyaBelirleyicisi");//XMLdoc.CreateAttribute("dosyaBelirleyicisi");
            //dosyaBelirleyicisi.Value = "";

            XmlAttribute mahiyetKodu = XML_Value_Preparing(XMLdoc, dsData, RowId, "mahiyetKodu");//XMLdoc.CreateAttribute("mahiyetKodu");
                                                                                                 //mahiyetKodu.Value = "";
            dosya.Attributes.Append(id);
            dosya.Attributes.Append(dosyaTipi);
            dosya.Attributes.Append(takipTuru);
            dosya.Attributes.Append(takipYolu);
            dosya.Attributes.Append(takipSekli);
            dosya.Attributes.Append(alacaklininTalepEttigiHak);
            dosya.Attributes.Append(BK84MaddeUygulansin);
            dosya.Attributes.Append(BSMVUygulansin);
            dosya.Attributes.Append(KKDFUygulansin);
            dosya.Attributes.Append(aciklama48e9);
            dosya.Attributes.Append(dosyaBelirleyicisi);
            dosya.Attributes.Append(mahiyetKodu);


            /*
            dosya.Attributes.Append(mahiyetKodu);
            dosya.Attributes.Append(dosyaBelirleyicisi);
            dosya.Attributes.Append(aciklama48e9);
            dosya.Attributes.Append(KKDFUygulansin);
            dosya.Attributes.Append(BSMVUygulansin);
            dosya.Attributes.Append(BK84MaddeUygulansin);
            dosya.Attributes.Append(alacaklininTalepEttigiHak);
            dosya.Attributes.Append(takipSekli);
            dosya.Attributes.Append(takipYolu);
            dosya.Attributes.Append(takipTuru);
            dosya.Attributes.Append(dosyaTipi);
            dosya.Attributes.Append(id);
            */

            return dosya;
        }
                
        private XmlNode UyapXML_VekilKisi_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
            <!ELEMENT VekilKisi 
             ((vekil, kisiTumBilgileri, adres?) | 
             (vekil, adres?, kisiTumBilgileri) | 
             (adres?, vekil, kisiTumBilgileri) | 
             (adres?, kisiTumBilgileri, vekil) | 
             (kisiTumBilgileri, adres?, vekil) | 
             (kisiTumBilgileri, vekil, adres?))>
            <!ATTLIST VekilKisi
                 id	     ID #IMPLIED
            >
            */
            #endregion

            XmlNode VekilKisi = XMLdoc.CreateElement("VekilKisi");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "VekilKisi_1";

            VekilKisi.Attributes.Append(id);

            XmlNode vekil = UyapXML_vekil_Preparing(XMLdoc);
            XmlNode kisiTumBilgileri = UyapXML_kisiTumBilgileri_Preparing(XMLdoc);
            XmlNode adres = UyapXML_adres_Preparing(XMLdoc);

            VekilKisi.AppendChild(vekil);
            VekilKisi.AppendChild(kisiTumBilgileri);
            VekilKisi.AppendChild(adres);

            return VekilKisi;
        }

        private XmlNode UyapXML_vekil_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
             <!ELEMENT vekil EMPTY>
              <!ATTLIST vekil
                 id               ID #IMPLIED
                 baroNo           CDATA #IMPLIED
                 tbbNo            CDATA #IMPLIED
                 avukatlikBuroAdi CDATA #IMPLIED
                 tcKimlikNo       CDATA #IMPLIED
                 adi              CDATA #IMPLIED
                 soyadi           CDATA #IMPLIED
                 vergiNo          CDATA #IMPLIED
                 vekilTipi        (B | K | S) "B"
                 bakanlikDosyaNo  CDATA #IMPLIED
                 kapanmaNedeni    (0 | 1 | 2)  #IMPLIED
                 kurumAvukatiMi   (E | H) "H"
                 sigortaliMi      (E | H) "H"
                 borcluVekiliMi   (E | H) "H"
              >
            */
            #endregion

            XmlNode vekil = XMLdoc.CreateElement("vekil");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";

            XmlAttribute baroNo = XMLdoc.CreateAttribute("baroNo");
            baroNo.Value = "123";

            XmlAttribute tbbNo = XMLdoc.CreateAttribute("tbbNo");
            tbbNo.Value = "456";

            XmlAttribute avukatlikBuroAdi = XMLdoc.CreateAttribute("avukatlikBuroAdi");
            avukatlikBuroAdi.Value = "adil";

            XmlAttribute tcKimlikNo = XMLdoc.CreateAttribute("tcKimlikNo");
            tcKimlikNo.Value = "12345678901";

            XmlAttribute adi = XMLdoc.CreateAttribute("adi");
            adi.Value = "Mehmet";

            XmlAttribute soyadi = XMLdoc.CreateAttribute("soyadi");
            soyadi.Value = "Sayın";

            XmlAttribute vergiNo = XMLdoc.CreateAttribute("vergiNo");
            vergiNo.Value = "456789123";

            XmlAttribute vekilTipi = XMLdoc.CreateAttribute("vekilTipi");
            vekilTipi.Value = "B";

            XmlAttribute bakanlikDosyaNo = XMLdoc.CreateAttribute("bakanlikDosyaNo");
            bakanlikDosyaNo.Value = "12345";

            XmlAttribute kapanmaNedeni = XMLdoc.CreateAttribute("kapanmaNedeni");
            kapanmaNedeni.Value = "0";

            XmlAttribute kurumAvukatiMi = XMLdoc.CreateAttribute("kurumAvukatiMi");
            kurumAvukatiMi.Value = "0";

            XmlAttribute sigortaliMi = XMLdoc.CreateAttribute("sigortaliMi");
            sigortaliMi.Value = "H";

            XmlAttribute borcluVekiliMi = XMLdoc.CreateAttribute("borcluVekiliMi");
            borcluVekiliMi.Value = "H";

            
            vekil.Attributes.Append(borcluVekiliMi);
            vekil.Attributes.Append(sigortaliMi);
            vekil.Attributes.Append(kurumAvukatiMi);
            vekil.Attributes.Append(kapanmaNedeni);
            vekil.Attributes.Append(bakanlikDosyaNo);
            vekil.Attributes.Append(vekilTipi);
            vekil.Attributes.Append(vergiNo);
            vekil.Attributes.Append(soyadi);
            vekil.Attributes.Append(adi);
            vekil.Attributes.Append(tcKimlikNo);
            vekil.Attributes.Append(avukatlikBuroAdi);
            vekil.Attributes.Append(tbbNo);
            vekil.Attributes.Append(baroNo);
            vekil.Attributes.Append(id);
            
            return vekil;
        }

        private XmlNode UyapXML_kisiTumBilgileri_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
            <!ELEMENT kisiTumBilgileri EMPTY>
              <!ATTLIST kisiTumBilgileri
                id						ID 	  #IMPLIED
                adi						CDATA #IMPLIED
                soyadi					CDATA #IMPLIED
                oncekiSoyadi			CDATA #IMPLIED
                babaAdi					CDATA #IMPLIED
                anaAdi					CDATA #IMPLIED
                dogumTarihi				CDATA #IMPLIED
                tcKimlikNo				CDATA #IMPLIED
                cinsiyeti				(K | E) "E"
                dogumYeri				CDATA #IMPLIED
                nufusaKayitIlKodu	    CDATA #IMPLIED
                nufusaKayitIlceKodu	    CDATA #IMPLIED
                mahKoy					CDATA #IMPLIED
                ciltNo					CDATA #IMPLIED
                aileSiraNo				CDATA #IMPLIED
                siraNo					CDATA #IMPLIED
                cuzdanSeriNo			CDATA #IMPLIED
                cuzdanNo				CDATA #IMPLIED
                kayitNo					CDATA #IMPLIED
                verildigiTarih			CDATA #IMPLIED
                verildigiIlKodu			CDATA #IMPLIED
                verildigiIlceKodu		CDATA #IMPLIED
                verilisNedeni			CDATA #IMPLIED
                ybnNfsKayitliOldgYer 	CDATA #IMPLIED
                verildigiIlAdi			CDATA #IMPLIED
                verildigiIlceAdi		CDATA #IMPLIED
                vergiNo					CDATA #IMPLIED
                >
            */
            #endregion 

            XmlNode kisiTumBilgileri = XMLdoc.CreateElement("kisiTumBilgileri");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";
            XmlAttribute adi = XMLdoc.CreateAttribute("adi");
            adi.Value = "Mehmet";
            XmlAttribute soyadi = XMLdoc.CreateAttribute("soyadi");
            soyadi.Value = "Sayın";
            XmlAttribute oncekiSoyadi = XMLdoc.CreateAttribute("oncekiSoyadi");
            oncekiSoyadi.Value = "";
            XmlAttribute babaAdi = XMLdoc.CreateAttribute("babaAdi");
            babaAdi.Value = "Mahmut";
            XmlAttribute anaAdi = XMLdoc.CreateAttribute("anaAdi");
            anaAdi.Value = "Ayşe";
            XmlAttribute dogumTarihi = XMLdoc.CreateAttribute("dogumTarihi");
            dogumTarihi.Value = "01.05.1970";
            XmlAttribute tcKimlikNo = XMLdoc.CreateAttribute("tcKimlikNo");
            tcKimlikNo.Value = "12345678901";
            XmlAttribute cinsiyeti = XMLdoc.CreateAttribute("cinsiyeti");
            cinsiyeti.Value = "E";
            XmlAttribute dogumYeri = XMLdoc.CreateAttribute("dogumYeri");
            dogumYeri.Value = "Ceyhan";
            XmlAttribute nufusaKayitIlKodu = XMLdoc.CreateAttribute("nufusaKayitIlKodu");
            nufusaKayitIlKodu.Value = "01";
            XmlAttribute nufusaKayitIlceKodu = XMLdoc.CreateAttribute("nufusaKayitIlceKodu");
            nufusaKayitIlceKodu.Value = "03";
            XmlAttribute verilisNedeni = XMLdoc.CreateAttribute("verilisNedeni");
            verilisNedeni.Value = "Yenileme";
            XmlAttribute ybnNfsKayitliOldgYer = XMLdoc.CreateAttribute("ybnNfsKayitliOldgYer");
            ybnNfsKayitliOldgYer.Value = "";
            XmlAttribute verildigiIlAdi = XMLdoc.CreateAttribute("verildigiIlAdi");
            verildigiIlAdi.Value = "06";
            XmlAttribute verildigiIlceAdi = XMLdoc.CreateAttribute("verildigiIlceAdi");
            verildigiIlceAdi.Value = "06";
            XmlAttribute vergiNo = XMLdoc.CreateAttribute("vergiNo");
            vergiNo.Value = "123456";

            kisiTumBilgileri.Attributes.Append(vergiNo);
            kisiTumBilgileri.Attributes.Append(verildigiIlceAdi);
            kisiTumBilgileri.Attributes.Append(verildigiIlAdi);
            kisiTumBilgileri.Attributes.Append(ybnNfsKayitliOldgYer);
            kisiTumBilgileri.Attributes.Append(verilisNedeni);
            kisiTumBilgileri.Attributes.Append(nufusaKayitIlceKodu);
            kisiTumBilgileri.Attributes.Append(nufusaKayitIlKodu);
            kisiTumBilgileri.Attributes.Append(dogumYeri);
            kisiTumBilgileri.Attributes.Append(cinsiyeti);
            kisiTumBilgileri.Attributes.Append(tcKimlikNo);
            kisiTumBilgileri.Attributes.Append(dogumTarihi);
            kisiTumBilgileri.Attributes.Append(anaAdi);
            kisiTumBilgileri.Attributes.Append(babaAdi);
            kisiTumBilgileri.Attributes.Append(oncekiSoyadi);
            kisiTumBilgileri.Attributes.Append(soyadi);
            kisiTumBilgileri.Attributes.Append(adi);
            kisiTumBilgileri.Attributes.Append(id);
            
            return kisiTumBilgileri;
        }

        private XmlNode UyapXML_kurum_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
            <!ELEMENT kurum EMPTY>
             <!ATTLIST kurum
               id							ID	#IMPLIED
               kurumAdi						CDATA #IMPLIED
               vergiNo						CDATA #IMPLIED
               vergiDairesi					cDATA #IMPLIED
               ticaretSicilNo				CDATA #IMPLIED
               ticaretSicilNoVerildigiYer	CDATA #IMPLIED
               kamuOzel						(O | K) "O"
               sskIsyeriSicilNo				CDATA #IMPLIED
               harcDurumu					(0 | 1)  "1"
               mersisNo                     CDATA #IMPLIED
               >
            */
            #endregion

            XmlNode kurum = XMLdoc.CreateElement("kurum");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";

            XmlAttribute kurumAdi = XMLdoc.CreateAttribute("kurumAdi");
            kurumAdi.Value = "Falan A.Ş.";

            XmlAttribute vergiNo = XMLdoc.CreateAttribute("vergiNo");
            vergiNo.Value = "3456789";

            XmlAttribute vergiDairesi = XMLdoc.CreateAttribute("vergiDairesi");
            vergiDairesi.Value = "Maltepe";

            XmlAttribute ticaretSicilNo = XMLdoc.CreateAttribute("ticaretSicilNo");
            ticaretSicilNo.Value = "56789123";

            XmlAttribute ticaretSicilNoVerildigiYer = XMLdoc.CreateAttribute("ticaretSicilNoVerildigiYer");
            ticaretSicilNoVerildigiYer.Value = "ANKARA";

            XmlAttribute kamuOzel = XMLdoc.CreateAttribute("kamuOzel");
            kamuOzel.Value = "O";

            XmlAttribute sskIsyeriSicilNo = XMLdoc.CreateAttribute("sskIsyeriSicilNo");
            sskIsyeriSicilNo.Value = "1234567";

            XmlAttribute harcDurumu = XMLdoc.CreateAttribute("harcDurumu");
            harcDurumu.Value = "0";

            XmlAttribute mersisNo = XMLdoc.CreateAttribute("mersisNo");
            mersisNo.Value = "";


            kurum.Attributes.Append(mersisNo);
            kurum.Attributes.Append(harcDurumu);
            kurum.Attributes.Append(sskIsyeriSicilNo);
            kurum.Attributes.Append(kamuOzel);
            kurum.Attributes.Append(ticaretSicilNoVerildigiYer);
            kurum.Attributes.Append(ticaretSicilNo);
            kurum.Attributes.Append(vergiDairesi);
            kurum.Attributes.Append(vergiNo);
            kurum.Attributes.Append(kurumAdi);
            kurum.Attributes.Append(id);

            return kurum;
        }
        
        private XmlNode UyapXML_adres_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
            <!ELEMENT adres EMPTY>
             <!ATTLIST adres 
               id					ID	#IMPLIED
               adresTuru			CDATA #IMPLIED
               adresTuruAciklama	CDATA #IMPLIED
               ilKodu				CDATA #IMPLIED
               il					CDATA #IMPLIED
               ilce				    CDATA #IMPLIED
               ilceKodu             CDATA #IMPLIED
               postaKodu			CDATA #IMPLIED
               adres				CDATA #IMPLIED
               telefon				CDATA #IMPLIED
               cepTelefon			CDATA #IMPLIED
               fax					CDATA #IMPLIED
               elektronikPostaAdresi	CDATA #IMPLIED
               > 
            */
            #endregion

            XmlNode madres = XMLdoc.CreateElement("adres");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";

            XmlAttribute adresTuru = XMLdoc.CreateAttribute("adresTuru");
            adresTuru.Value = "ADRTR00001";

            XmlAttribute adresTuruAciklama = XMLdoc.CreateAttribute("adresTuruAciklama");
            adresTuruAciklama.Value = "Yurtiçi İkametgah Adresi";

            XmlAttribute ilKodu = XMLdoc.CreateAttribute("ilKodu");
            ilKodu.Value = "06";

            XmlAttribute il = XMLdoc.CreateAttribute("il");
            il.Value = "ANKARA";

            XmlAttribute ilce = XMLdoc.CreateAttribute("ilce");
            ilce.Value = "Çankaya";

            XmlAttribute ilceKodu = XMLdoc.CreateAttribute("ilceKodu");
            ilceKodu.Value = "08";

            XmlAttribute postaKodu = XMLdoc.CreateAttribute("postaKodu");
            postaKodu.Value = "12345";

            XmlAttribute adres = XMLdoc.CreateAttribute("adres");
            adres.Value = "STARAZBURG NO : 28 / 22 SIHIYE";

            XmlAttribute telefon = XMLdoc.CreateAttribute("telefon");
            telefon.Value = "3124567845";

            XmlAttribute cepTelefon = XMLdoc.CreateAttribute("cepTelefon");
            cepTelefon.Value = "5324581274";

            XmlAttribute fax = XMLdoc.CreateAttribute("fax");
            fax.Value = "3124859585";

            XmlAttribute elektronikPostaAdresi = XMLdoc.CreateAttribute("elektronikPostaAdresi");
            elektronikPostaAdresi.Value = "mehmetaslan@hotmail.com";


            madres.Attributes.Append(elektronikPostaAdresi);
            madres.Attributes.Append(fax);
            madres.Attributes.Append(cepTelefon);
            madres.Attributes.Append(telefon);
            madres.Attributes.Append(adres);
            madres.Attributes.Append(postaKodu);
            madres.Attributes.Append(ilceKodu);
            madres.Attributes.Append(ilce);
            madres.Attributes.Append(il);
            madres.Attributes.Append(ilKodu);
            madres.Attributes.Append(adresTuruAciklama);
            madres.Attributes.Append(adresTuru);
            madres.Attributes.Append(id);

            return madres;
        }

        //--- Taraflar

        private XmlNode UyapXML_taraf_Preparing(XmlDocument XMLdoc, string tarafNo)
        {
            #region 
            /*
            <!ELEMENT taraf ( 
               ((kisiKurumBilgileri | ref), rolTur,iban?, ref*) | 
               ((kisiKurumBilgileri | ref), ref*, rolTur,iban?) | 
               (ref*, (kisiKurumBilgileri | ref), rolTur,iban?) | 
               (rolTur ,iban?, (kisiKurumBilgileri | ref), ref*) |
               (rolTur ,iban? , ref*, (kisiKurumBilgileri | ref)) |
               (ref*, rolTur ,iban?, (kisiKurumBilgileri | ref)) )>
            <!ATTLIST taraf
                 id       ID #IMPLIED
            >
            */
            #endregion

            XmlNode taraf = XMLdoc.CreateElement("taraf");

            XmlAttribute id = XMLdoc.CreateAttribute("id");

            XmlNode kisiKurumBilgileri = null;
            
            if (tarafNo == "1")
            {
                id.Value = "taraf_1";
                kisiKurumBilgileri = UyapXML_kisiKurumBilgileri_Preparing(XMLdoc);

                XmlNode kisiTumBilgileri = UyapXML_kisiTumBilgileri_Preparing(XMLdoc);
                XmlNode adres = UyapXML_adres_Preparing(XMLdoc);

                kisiKurumBilgileri.AppendChild(kisiTumBilgileri);
                kisiKurumBilgileri.AppendChild(adres);
            }

            if (tarafNo == "2")
            {
                id.Value = "taraf_2";
                kisiKurumBilgileri = UyapXML_kisiKurumBilgileri_Preparing(XMLdoc);

                XmlNode kurum = UyapXML_kurum_Preparing(XMLdoc);
                XmlNode adres = UyapXML_adres_Preparing(XMLdoc);

                kisiKurumBilgileri.AppendChild(kurum);
                kisiKurumBilgileri.AppendChild(adres);
            }

            
            taraf.Attributes.Append(id);
            
            if (kisiKurumBilgileri != null)
                taraf.AppendChild(kisiKurumBilgileri);
            
            XmlNode rolTur = UyapXML_rolTur_Preparing(XMLdoc, tarafNo);
            XmlNode ref_ = UyapXML_ref_Preparing(XMLdoc, tarafNo);

            if (rolTur != null)
                taraf.AppendChild(rolTur);

            if (ref_ != null)
                taraf.AppendChild(ref_);

            return taraf;
        }

        private XmlNode UyapXML_kisiKurumBilgileri_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
             <!ELEMENT kisiKurumBilgileri (((kurum | kisiTumBilgileri | ref), (adres | ref)?) | ((adres | ref)?, (kurum | kisiTumBilgileri | ref)))>
              <!ATTLIST kisiKurumBilgileri
                 id		ID #IMPLIED
                 ad	CDATA	#IMPLIED
              >
            */
            #endregion

            XmlNode kisiKurumBilgileri = XMLdoc.CreateElement("kisiKurumBilgileri");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";

            XmlAttribute ad = XMLdoc.CreateAttribute("ad");
            ad.Value = "Tekin UÇAR";
            
            kisiKurumBilgileri.Attributes.Append(ad);
            kisiKurumBilgileri.Attributes.Append(id);

            return kisiKurumBilgileri;
        }

        private XmlNode UyapXML_rolTur_Preparing(XmlDocument XMLdoc, string tarafNo)
        {
            /*
            <!ELEMENT rolTur EMPTY>
              <!ATTLIST rolTur
                rolID	CDATA #IMPLIED
                Rol		CDATA #IMPLIED
              >
            */

            XmlNode rolTur = XMLdoc.CreateElement("rolTur");

            XmlAttribute rolID = XMLdoc.CreateAttribute("rolID");
            
            XmlAttribute Rol = XMLdoc.CreateAttribute("Rol");

            if (tarafNo == "1")
            {
                rolID.Value = "21";
                Rol.Value = "ALACAKLI";
            }
            if (tarafNo == "2")
            {
                rolID.Value = "22";
                Rol.Value = "BORÇLU/MÜFLİS";
            }
            
            rolTur.Attributes.Append(Rol);
            rolTur.Attributes.Append(rolID);
            
            return rolTur;
        }

        private XmlNode UyapXML_ref_Preparing(XmlDocument XMLdoc, string tarafNo)
        {
            /*
            <!ELEMENT ref EMPTY>
             <!ATTLIST ref
               to        CDATA  #REQUIRED
               id        IDREF  #REQUIRED
             >
            */

            XmlNode ref_ = XMLdoc.CreateElement("ref"); 

            XmlAttribute to = XMLdoc.CreateAttribute("to");
            to.Value = "VekilKisi";

            XmlAttribute id = XMLdoc.CreateAttribute("id");

            if (tarafNo == "1")
            {
                id.Value = "VekilKisi_1";
            }
            if (tarafNo == "2")
            {
                id.Value = "";
            }

            if (id.Value != "")
            {
                ref_.Attributes.Append(to);
                ref_.Attributes.Append(id);
            }
            else ref_ = null;

            return ref_;
        }

        //--- dayanaklar

        private XmlNode UyapXML_alacakKalemi_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
            <!ELEMENT alacakKalemi (taraf | ref | faiz)*>
             <!ATTLIST alacakKalemi
              id						ID #IMPLIED
              alacakKalemKod			CDATA #IMPLIED
              alacakKalemAdi			CDATA #IMPLIED
              alacakKalemTutar		    CDATA #IMPLIED
              alacakKalemIlkTutar		CDATA #IMPLIED
              alacakKalemTip			CDATA #IMPLIED
              tutarTur				    CDATA #IMPLIED
              tutarAdi				    CDATA #IMPLIED
              sabitTaksitTarihi		    CDATA #IMPLIED
              dovizKurCevrimi			CDATA #IMPLIED
              akdiFaiz				    CDATA #IMPLIED
              alacakKalemKodAciklama	CDATA #IMPLIED
              aciklama				    CDATA #IMPLIED
              alacakKalemKodTuru		CDATA #IMPLIED
             >
            */
            #endregion

            XmlNode alacakKalemi = XMLdoc.CreateElement("alacakKalemi");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";
            XmlAttribute alacakKalemKod = XMLdoc.CreateAttribute("alacakKalemKod");
            alacakKalemKod.Value = "1";
            XmlAttribute alacakKalemAdi = XMLdoc.CreateAttribute("alacakKalemAdi");
            alacakKalemAdi.Value = "1";
            XmlAttribute alacakKalemTutar = XMLdoc.CreateAttribute("alacakKalemTutar");
            alacakKalemTutar.Value = "1";
            XmlAttribute alacakKalemIlkTutar = XMLdoc.CreateAttribute("alacakKalemIlkTutar");
            alacakKalemIlkTutar.Value = "1";
            XmlAttribute alacakKalemTip = XMLdoc.CreateAttribute("alacakKalemTip");
            alacakKalemTip.Value = "1";
            XmlAttribute tutarTur = XMLdoc.CreateAttribute("tutarTur");
            tutarTur.Value = "1";
            XmlAttribute tutarAdi = XMLdoc.CreateAttribute("tutarAdi");
            tutarAdi.Value = "1";
            XmlAttribute sabitTaksitTarihi = XMLdoc.CreateAttribute("sabitTaksitTarihi");
            sabitTaksitTarihi.Value = "1";
            XmlAttribute dovizKurCevrimi = XMLdoc.CreateAttribute("dovizKurCevrimi");
            dovizKurCevrimi.Value = "1";
            XmlAttribute akdiFaiz = XMLdoc.CreateAttribute("akdiFaiz");
            akdiFaiz.Value = "1";
            XmlAttribute alacakKalemKodAciklama = XMLdoc.CreateAttribute("alacakKalemKodAciklama");
            alacakKalemKodAciklama.Value = "1";
            XmlAttribute aciklama = XMLdoc.CreateAttribute("aciklama");
            aciklama.Value = "1";
            XmlAttribute alacakKalemKodTuru = XMLdoc.CreateAttribute("alacakKalemKodTuru");
            alacakKalemKodTuru.Value = "1";

            alacakKalemi.Attributes.Append(alacakKalemKodTuru);
            alacakKalemi.Attributes.Append(aciklama);
            alacakKalemi.Attributes.Append(alacakKalemKodAciklama);
            alacakKalemi.Attributes.Append(akdiFaiz);
            alacakKalemi.Attributes.Append(dovizKurCevrimi);
            alacakKalemi.Attributes.Append(sabitTaksitTarihi);
            alacakKalemi.Attributes.Append(tutarAdi);
            alacakKalemi.Attributes.Append(tutarTur);
            alacakKalemi.Attributes.Append(alacakKalemTip);
            alacakKalemi.Attributes.Append(alacakKalemIlkTutar);
            alacakKalemi.Attributes.Append(alacakKalemTutar);
            alacakKalemi.Attributes.Append(alacakKalemAdi);
            alacakKalemi.Attributes.Append(alacakKalemKod);
            alacakKalemi.Attributes.Append(id);

            // alacakKalemi.AppendChild()
            // ++ ref
            // ++ faiz

            return alacakKalemi;
        }

        private XmlNode UyapXML_faiz_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
             <!ELEMENT faiz EMPTY>
              <!ATTLIST faiz
                id						ID #IMPLIED
                baslangicTarihi			CDATA #IMPLIED
                bitisTarihi				CDATA #IMPLIED
                faizOran				CDATA #IMPLIED
                faizTipKod				CDATA #IMPLIED
                faizTipKodAciklama		CDATA #IMPLIED
                faizTutar				CDATA #IMPLIED
                faizTutarTur			CDATA #IMPLIED
                faizTutarTurAdi	    	CDATA #IMPLIED
                faizSureTip			    CDATA #IMPLIED
              >
            */
            #endregion

            XmlNode faiz = XMLdoc.CreateElement("faiz");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";
            XmlAttribute baslangicTarihi = XMLdoc.CreateAttribute("baslangicTarihi");
            baslangicTarihi.Value = "1";
            XmlAttribute bitisTarihi = XMLdoc.CreateAttribute("bitisTarihi");
            bitisTarihi.Value = "1";
            XmlAttribute faizOran = XMLdoc.CreateAttribute("faizOran");
            faizOran.Value = "1";
            XmlAttribute faizTipKod = XMLdoc.CreateAttribute("faizTipKod");
            faizTipKod.Value = "1";
            XmlAttribute faizTipKodAciklama = XMLdoc.CreateAttribute("faizTipKodAciklama");
            faizTipKodAciklama.Value = "1";
            XmlAttribute faizTutar = XMLdoc.CreateAttribute("faizTutar");
            faizTutar.Value = "1";
            XmlAttribute faizTutarTur = XMLdoc.CreateAttribute("faizTutarTur");
            faizTutarTur.Value = "1";
            XmlAttribute faizTutarTurAdi = XMLdoc.CreateAttribute("faizTutarTurAdi");
            faizTutarTurAdi.Value = "1";
            XmlAttribute faizSureTip = XMLdoc.CreateAttribute("faizSureTip");
            faizSureTip.Value = "1";

            faiz.Attributes.Append(faizSureTip);
            faiz.Attributes.Append(faizTutarTurAdi);
            faiz.Attributes.Append(faizTutarTur);
            faiz.Attributes.Append(faizTutar);
            faiz.Attributes.Append(faizTipKodAciklama);
            faiz.Attributes.Append(faizTipKod);
            faiz.Attributes.Append(faizOran);
            faiz.Attributes.Append(bitisTarihi);
            faiz.Attributes.Append(baslangicTarihi);
            faiz.Attributes.Append(id);

            return faiz;
        }

        private XmlNode UyapXML_cek_Preparing(XmlDocument XMLdoc)
        {
            #region 
            /*
            <!ELEMENT cek (alacakKalemi | taraf | ref)*>
              <!ATTLIST cek
              id				ID #IMPLIED
              islemlerBasladimi	(E | H) "H"
              kocanNo			CDATA #IMPLIED
              seriNo			CDATA #IMPLIED	
              kesideTarihi		CDATA #IMPLIED
              kesideYeri		CDATA #IMPLIED
              tutar				CDATA #IMPLIED
              tutarTur			CDATA #IMPLIED
              tutarTurAciklama	CDATA #IMPLIED
              odemeYeri			CDATA #IMPLIED
              ibrazTarihi		CDATA #IMPLIED
              bankaID			CDATA #IMPLIED
              bankaSubeKod		CDATA #IMPLIED
              bankaAdi			CDATA #IMPLIED
              bankaSubeAdi		CDATA #IMPLIED
              bankaSubeIl		CDATA #IMPLIED
              bankaSubeIlce		CDATA #IMPLIED
              bankaSubeAdres	CDATA #IMPLIED
              >
            */
            #endregion

            XmlNode cek = XMLdoc.CreateElement("cek");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";
            XmlAttribute islemlerBasladimi = XMLdoc.CreateAttribute("islemlerBasladimi");
            islemlerBasladimi.Value = "1";
            XmlAttribute kocanNo = XMLdoc.CreateAttribute("kocanNo");
            kocanNo.Value = "1";
            XmlAttribute seriNo = XMLdoc.CreateAttribute("seriNo");
            seriNo.Value = "1";
            XmlAttribute kesideTarihi = XMLdoc.CreateAttribute("kesideTarihi");
            kesideTarihi.Value = "1";
            XmlAttribute kesideYeri = XMLdoc.CreateAttribute("kesideYeri");
            kesideYeri.Value = "1";
            XmlAttribute tutar = XMLdoc.CreateAttribute("tutar");
            tutar.Value = "1";
            XmlAttribute tutarTur = XMLdoc.CreateAttribute("tutarTur");
            tutarTur.Value = "1";
            XmlAttribute tutarTurAciklama = XMLdoc.CreateAttribute("tutarTurAciklama");
            tutarTurAciklama.Value = "1";
            XmlAttribute odemeYeri = XMLdoc.CreateAttribute("odemeYeri");
            odemeYeri.Value = "1";
            XmlAttribute ibrazTarihi = XMLdoc.CreateAttribute("ibrazTarihi");
            ibrazTarihi.Value = "1";
            XmlAttribute bankaID = XMLdoc.CreateAttribute("bankaID");
            bankaID.Value = "1";
            XmlAttribute bankaSubeKod = XMLdoc.CreateAttribute("bankaSubeKod");
            bankaSubeKod.Value = "1";
            XmlAttribute bankaAdi = XMLdoc.CreateAttribute("bankaAdi");
            bankaAdi.Value = "1";
            XmlAttribute bankaSubeAdi = XMLdoc.CreateAttribute("bankaSubeAdi");
            bankaSubeAdi.Value = "1";
            XmlAttribute bankaSubeIl = XMLdoc.CreateAttribute("bankaSubeIl");
            bankaSubeIl.Value = "1";
            XmlAttribute bankaSubeIlce = XMLdoc.CreateAttribute("bankaSubeIlce");
            bankaSubeIlce.Value = "1";
            XmlAttribute bankaSubeAdres = XMLdoc.CreateAttribute("bankaSubeAdres");
            bankaSubeAdres.Value = "1";

            cek.Attributes.Append(bankaSubeAdres);
            cek.Attributes.Append(bankaSubeIlce);
            cek.Attributes.Append(bankaSubeIl);
            cek.Attributes.Append(bankaSubeAdi);
            cek.Attributes.Append(bankaAdi);
            cek.Attributes.Append(bankaSubeKod);
            cek.Attributes.Append(bankaID);
            cek.Attributes.Append(ibrazTarihi);
            cek.Attributes.Append(odemeYeri);
            cek.Attributes.Append(tutarTurAciklama);
            cek.Attributes.Append(tutarTur);
            cek.Attributes.Append(tutar);
            cek.Attributes.Append(kesideYeri);
            cek.Attributes.Append(kesideTarihi);
            cek.Attributes.Append(seriNo);
            cek.Attributes.Append(kocanNo);
            cek.Attributes.Append(islemlerBasladimi);
            cek.Attributes.Append(id);

            return cek;

        }

        private XmlNode UyapXML_senet_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
            <!ELEMENT senet (alacakKalemi | taraf | ref)*>
             <!ATTLIST senet
              id						ID #IMPLIED
              islemlerBasladimi		(E | H) "H"
              tanzimTarihi			CDATA #IMPLIED
              belgeninTutari			cDATA #IMPLIED
              tutarTur				CDATA #IMPLIED
              tutarAdi				CDATA #IMPLIED
              olmasiGrknPulDegeri		CDATA #IMPLIED
              uzerindekiPulunDegeri	CDATA #IMPLIED
              odemeYeri				CDATA #IMPLIED
              tanzimYeri				CDATA #IMPLIED
              vadeTarihi				CDATA #IMPLIED
              >
            */
            #endregion

            XmlNode senet = XMLdoc.CreateElement("senet");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";
            XmlAttribute islemlerBasladimi = XMLdoc.CreateAttribute("islemlerBasladimi");
            islemlerBasladimi.Value = "1";
            XmlAttribute tanzimTarihi = XMLdoc.CreateAttribute("tanzimTarihi");
            tanzimTarihi.Value = "1";
            XmlAttribute belgeninTutari = XMLdoc.CreateAttribute("belgeninTutari");
            belgeninTutari.Value = "1";
            XmlAttribute tutarTur = XMLdoc.CreateAttribute("tutarTur");
            tutarTur.Value = "1";
            XmlAttribute tutarAdi = XMLdoc.CreateAttribute("tutarAdi");
            tutarAdi.Value = "1";
            XmlAttribute olmasiGrknPulDegeri = XMLdoc.CreateAttribute("olmasiGrknPulDegeri");
            olmasiGrknPulDegeri.Value = "1";
            XmlAttribute uzerindekiPulunDegeri = XMLdoc.CreateAttribute("uzerindekiPulunDegeri");
            uzerindekiPulunDegeri.Value = "1";
            XmlAttribute odemeYeri = XMLdoc.CreateAttribute("odemeYeri");
            odemeYeri.Value = "1";
            XmlAttribute tanzimYeri = XMLdoc.CreateAttribute("tanzimYeri");
            tanzimYeri.Value = "1";
            XmlAttribute vadeTarihi = XMLdoc.CreateAttribute("vadeTarihi");
            vadeTarihi.Value = "1";

            senet.Attributes.Append(vadeTarihi);
            senet.Attributes.Append(tanzimYeri);
            senet.Attributes.Append(odemeYeri);
            senet.Attributes.Append(uzerindekiPulunDegeri);
            senet.Attributes.Append(olmasiGrknPulDegeri);
            senet.Attributes.Append(tutarAdi);
            senet.Attributes.Append(tutarTur);
            senet.Attributes.Append(belgeninTutari);
            senet.Attributes.Append(tanzimTarihi);
            senet.Attributes.Append(islemlerBasladimi);
            senet.Attributes.Append(id);

            return senet;
        }

        private XmlNode UyapXML_police_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
             <!ELEMENT police (alacakKalemi | taraf | ref)*>
              <!ATTLIST police
               id						ID #IMPLIED
               islemlerBasladimi		(E | H) "H"
               kesideTarihi			    CDATA #IMPLIED
               belgeninTutari			cDATA #IMPLIED
               tutarTur				    CDATA #IMPLIED
               tutarAdi				    CDATA #IMPLIED
               vadeTarihi				CDATA #IMPLIED
               odemeYeri				CDATA #IMPLIED
               kesideYeri				CDATA #IMPLIED
               olmasiGrknPulDegeri		CDATA #IMPLIED
               uzerindekiPulunDegeri	CDATA #IMPLIED
               lehtarAdSoyad			CDATA #IMPLIED
               kesideciAdSoyad			CDATA #IMPLIED
               odeyecekKisiAdSoyad	    CDATA #IMPLIED
              >
            */
            #endregion

            XmlNode police = XMLdoc.CreateElement("police");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";
            XmlAttribute islemlerBasladimi = XMLdoc.CreateAttribute("islemlerBasladimi");
            islemlerBasladimi.Value = "1";
            XmlAttribute kesideTarihi = XMLdoc.CreateAttribute("kesideTarihi");
            kesideTarihi.Value = "1";
            XmlAttribute belgeninTutari = XMLdoc.CreateAttribute("belgeninTutari");
            belgeninTutari.Value = "1";
            XmlAttribute tutarTur = XMLdoc.CreateAttribute("tutarTur");
            tutarTur.Value = "1";
            XmlAttribute tutarAdi = XMLdoc.CreateAttribute("tutarAdi");
            tutarAdi.Value = "1";
            XmlAttribute vadeTarihi = XMLdoc.CreateAttribute("vadeTarihi");
            vadeTarihi.Value = "1";
            XmlAttribute odemeYeri = XMLdoc.CreateAttribute("odemeYeri");
            odemeYeri.Value = "1";
            XmlAttribute kesideYeri = XMLdoc.CreateAttribute("kesideYeri");
            kesideYeri.Value = "1";
            XmlAttribute olmasiGrknPulDegeri = XMLdoc.CreateAttribute("olmasiGrknPulDegeri");
            olmasiGrknPulDegeri.Value = "1";
            XmlAttribute uzerindekiPulunDegeri = XMLdoc.CreateAttribute("uzerindekiPulunDegeri");
            uzerindekiPulunDegeri.Value = "1";
            XmlAttribute lehtarAdSoyad = XMLdoc.CreateAttribute("lehtarAdSoyad");
            lehtarAdSoyad.Value = "1";
            XmlAttribute kesideciAdSoyad = XMLdoc.CreateAttribute("kesideciAdSoyad");
            kesideciAdSoyad.Value = "1";
            XmlAttribute odeyecekKisiAdSoyad = XMLdoc.CreateAttribute("odeyecekKisiAdSoyad");
            odeyecekKisiAdSoyad.Value = "1";


            police.Attributes.Append(odeyecekKisiAdSoyad);
            police.Attributes.Append(kesideciAdSoyad);
            police.Attributes.Append(lehtarAdSoyad);
            police.Attributes.Append(uzerindekiPulunDegeri);
            police.Attributes.Append(olmasiGrknPulDegeri);
            police.Attributes.Append(kesideYeri);
            police.Attributes.Append(odemeYeri);
            police.Attributes.Append(vadeTarihi);
            police.Attributes.Append(tutarAdi);
            police.Attributes.Append(tutarTur);
            police.Attributes.Append(belgeninTutari);
            police.Attributes.Append(kesideTarihi);
            police.Attributes.Append(islemlerBasladimi);
            police.Attributes.Append(id);

            return police;
        }

        private XmlNode UyapXML_kontrat_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
             <!ELEMENT kontrat (alacakKalemi | taraf | ref)*>
              <!ATTLIST kontrat 
                id							ID #IMPLIED
                sozlesmeSozluBelgeli		(S | B) "B"
                belgeninTutari				CDATA #IMPLIED
                tutarTur					CDATA #IMPLIED
                gecerlilikBaslangicTarihi	CDATA #IMPLIED
                gecerlilikSonlanmaTarihi	CDATA #IMPLIED
                gecerliOlduguSure			CDATA #IMPLIED
                uzerindekiPulDegeri			CDATA #IMPLIED
                olmasiGerekenPulDegeri		CDATA #IMPLIED
                aciklama					CDATA #IMPLIED
                belgeIslemleriBaslatiliyormu	(E | H) "H"
                hazirlanisTarihi			CDATA #IMPLIED
                tutarTurAciklama			CDATA #IMPLIED
                adresAciklama				CDATA #IMPLIED
               >
            */
            #endregion

            XmlNode kontrat = XMLdoc.CreateElement("kontrat");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";
            XmlAttribute sozlesmeSozluBelgeli = XMLdoc.CreateAttribute("sozlesmeSozluBelgeli");
            sozlesmeSozluBelgeli.Value = "1";
            XmlAttribute belgeninTutari = XMLdoc.CreateAttribute("belgeninTutari");
            belgeninTutari.Value = "1";
            XmlAttribute tutarTur = XMLdoc.CreateAttribute("tutarTur");
            tutarTur.Value = "1";
            XmlAttribute gecerlilikBaslangicTarihi = XMLdoc.CreateAttribute("gecerlilikBaslangicTarihi");
            gecerlilikBaslangicTarihi.Value = "1";
            XmlAttribute gecerlilikSonlanmaTarihi = XMLdoc.CreateAttribute("gecerlilikSonlanmaTarihi");
            gecerlilikSonlanmaTarihi.Value = "1";
            XmlAttribute gecerliOlduguSure = XMLdoc.CreateAttribute("gecerliOlduguSure");
            gecerliOlduguSure.Value = "1";
            XmlAttribute uzerindekiPulDegeri = XMLdoc.CreateAttribute("uzerindekiPulDegeri");
            uzerindekiPulDegeri.Value = "1";
            XmlAttribute olmasiGerekenPulDegeri = XMLdoc.CreateAttribute("olmasiGerekenPulDegeri");
            olmasiGerekenPulDegeri.Value = "1";
            XmlAttribute aciklama = XMLdoc.CreateAttribute("aciklama");
            aciklama.Value = "1";
            XmlAttribute belgeIslemleriBaslatiliyormu = XMLdoc.CreateAttribute("belgeIslemleriBaslatiliyormu");
            belgeIslemleriBaslatiliyormu.Value = "1";
            XmlAttribute hazirlanisTarihi = XMLdoc.CreateAttribute("hazirlanisTarihi");
            hazirlanisTarihi.Value = "1";
            XmlAttribute tutarTurAciklama = XMLdoc.CreateAttribute("tutarTurAciklama");
            tutarTurAciklama.Value = "1";
            XmlAttribute adresAciklama = XMLdoc.CreateAttribute("adresAciklama");
            adresAciklama.Value = "1";

            kontrat.Attributes.Append(adresAciklama);
            kontrat.Attributes.Append(tutarTurAciklama);
            kontrat.Attributes.Append(hazirlanisTarihi);
            kontrat.Attributes.Append(belgeIslemleriBaslatiliyormu);
            kontrat.Attributes.Append(aciklama);
            kontrat.Attributes.Append(olmasiGerekenPulDegeri);
            kontrat.Attributes.Append(uzerindekiPulDegeri);
            kontrat.Attributes.Append(gecerliOlduguSure);
            kontrat.Attributes.Append(gecerlilikSonlanmaTarihi);
            kontrat.Attributes.Append(gecerlilikBaslangicTarihi);
            kontrat.Attributes.Append(tutarTur);
            kontrat.Attributes.Append(belgeninTutari);
            kontrat.Attributes.Append(sozlesmeSozluBelgeli);
            kontrat.Attributes.Append(id);

            return kontrat;
        }

        private XmlNode UyapXML_kontratKefil_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
             <!ELEMENT kontratKefil ( ((kontrat | ref), (adres | ref)?) | ((adres | ref)?, (kontrat | ref)) )>
              <!ATTLIST kontratKefil
               id	ID #IMPLIED
              >
            */
            #endregion

            XmlNode kontratKefil = XMLdoc.CreateElement("kontratKefil");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";

            kontratKefil.Attributes.Append(id);

            return kontratKefil;
        }
        
        private XmlNode UyapXML_digerAlacak_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
             <!ELEMENT digerAlacak (alacakKalemi | taraf | ref)*>
              <!ATTLIST digerAlacak
               id					ID #IMPLIED
               digerAlacakAciklama	CDATA #IMPLIED
               tarih				CDATA #IMPLIED
               tutar				CDATA #IMPLIED
               alacakNo				CDATA #IMPLIED
               tutarTur				CDATA #IMPLIED
               tutarAdi				CDATA #IMPLIED
               >
            */
            #endregion

            XmlNode digerAlacak = XMLdoc.CreateElement("digerAlacak");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";
            XmlAttribute digerAlacakAciklama = XMLdoc.CreateAttribute("digerAlacakAciklama");
            digerAlacakAciklama.Value = "1";
            XmlAttribute tarih = XMLdoc.CreateAttribute("tarih");
            tarih.Value = "1";
            XmlAttribute tutar = XMLdoc.CreateAttribute("tutar");
            tutar.Value = "1";
            XmlAttribute alacakNo = XMLdoc.CreateAttribute("alacakNo");
            alacakNo.Value = "1";
            XmlAttribute tutarTur = XMLdoc.CreateAttribute("tutarTur");
            tutarTur.Value = "1";
            XmlAttribute tutarAdi = XMLdoc.CreateAttribute("tutarAdi");
            tutarAdi.Value = "1";

            digerAlacak.Attributes.Append(tutarAdi);
            digerAlacak.Attributes.Append(tutarTur);
            digerAlacak.Attributes.Append(alacakNo);
            digerAlacak.Attributes.Append(tutar);
            digerAlacak.Attributes.Append(tarih);
            digerAlacak.Attributes.Append(digerAlacakAciklama);
            digerAlacak.Attributes.Append(id);

            return digerAlacak;
        }
        
        private XmlNode UyapXML_ilam_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
             <!ELEMENT ilam ((paraylaOlculemeyenAlacak | alacakKalemi)*, teminat?, (paraylaOlculemeyenAlacak, alacakKalemi)*)>
              <!ATTLIST ilam
                id					ID #IMPLIED
                ilamTarihi			CDATA #IMPLIED
                ilamKararNoYil		CDATA #IMPLIED
                ilamKararSira		CDATA #IMPLIED
                tcKimlikNo			CDATA #IMPLIED
                ilamDosyaNoYil		CDATA #IMPLIED
                ilamDosyaSira		CDATA #IMPLIED
                ilamAciklama		CDATA #IMPLIED
                ilamiVerenMahkeme	CDATA #IMPLIED
                davaAcilisTarih		CDATA #IMPLIED
                kesifTarih			CDATA #IMPLIED
                kesinlesmeTarih		CDATA #IMPLIED
                ilamKurumTip		CDATA #IMPLIED
                ilamKurumAd			CDATA #IMPLIED
                >
            */
            #endregion

            XmlNode ilam = XMLdoc.CreateElement("ilam");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";
            XmlAttribute ilamTarihi = XMLdoc.CreateAttribute("ilamTarihi");
            ilamTarihi.Value = "1";
            XmlAttribute ilamKararNoYil = XMLdoc.CreateAttribute("ilamKararNoYil");
            ilamKararNoYil.Value = "1";
            XmlAttribute ilamKararSira = XMLdoc.CreateAttribute("ilamKararSira");
            ilamKararSira.Value = "1";
            XmlAttribute tcKimlikNo = XMLdoc.CreateAttribute("tcKimlikNo");
            tcKimlikNo.Value = "1";
            XmlAttribute ilamDosyaNoYil = XMLdoc.CreateAttribute("ilamDosyaNoYil");
            ilamDosyaNoYil.Value = "1";
            XmlAttribute ilamDosyaSira = XMLdoc.CreateAttribute("ilamDosyaSira");
            ilamDosyaSira.Value = "1";
            XmlAttribute ilamAciklama = XMLdoc.CreateAttribute("ilamAciklama");
            ilamAciklama.Value = "1";
            XmlAttribute ilamiVerenMahkeme = XMLdoc.CreateAttribute("ilamiVerenMahkeme");
            ilamiVerenMahkeme.Value = "1";
            XmlAttribute davaAcilisTarih = XMLdoc.CreateAttribute("davaAcilisTarih");
            davaAcilisTarih.Value = "1";
            XmlAttribute kesifTarih = XMLdoc.CreateAttribute("kesifTarih");
            kesifTarih.Value = "1";
            XmlAttribute kesinlesmeTarih = XMLdoc.CreateAttribute("kesinlesmeTarih");
            kesinlesmeTarih.Value = "1";
            XmlAttribute ilamKurumTip = XMLdoc.CreateAttribute("ilamKurumTip");
            ilamKurumTip.Value = "1";
            XmlAttribute ilamKurumAd = XMLdoc.CreateAttribute("ilamKurumAd");
            ilamKurumAd.Value = "1";


            ilam.Attributes.Append(ilamKurumAd);
            ilam.Attributes.Append(ilamKurumTip);
            ilam.Attributes.Append(kesinlesmeTarih);
            ilam.Attributes.Append(kesifTarih);
            ilam.Attributes.Append(davaAcilisTarih);
            ilam.Attributes.Append(ilamiVerenMahkeme);
            ilam.Attributes.Append(ilamAciklama);
            ilam.Attributes.Append(ilamDosyaSira);
            ilam.Attributes.Append(ilamDosyaNoYil);
            ilam.Attributes.Append(tcKimlikNo);
            ilam.Attributes.Append(ilamKararSira);
            ilam.Attributes.Append(ilamKararNoYil);
            ilam.Attributes.Append(ilamTarihi);
            ilam.Attributes.Append(id);

            return ilam;
        }

        private XmlNode UyapXML_teminat_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
             <!ELEMENT teminat EMPTY>
              <!ATTLIST teminat
               id					ID #IMPLIED
               teminatTutar			CDATA #IMPLIED
               tutarTur				CDATA #IMPLIED
               ilamAciklama			CDATA #IMPLIED
               tahsilatMakbuzNo		CDATA #IMPLIED
               tahsilatMakbuzTarihi	CDATA #IMPLIED
               teminatNo			CDATA #IMPLIED
               teminatiVerenKurum	CDATA #IMPLIED
               teminatTipi			CDATA #IMPLIED
               teminatNedeni		CDATA #IMPLIED
              >
            */
            #endregion

            XmlNode teminat = XMLdoc.CreateElement("teminat");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";
            XmlAttribute teminatTutar = XMLdoc.CreateAttribute("teminatTutar");
            teminatTutar.Value = "1";
            XmlAttribute tutarTur = XMLdoc.CreateAttribute("tutarTur");
            tutarTur.Value = "1";
            XmlAttribute ilamAciklama = XMLdoc.CreateAttribute("ilamAciklama");
            ilamAciklama.Value = "1";
            XmlAttribute tahsilatMakbuzNo = XMLdoc.CreateAttribute("tahsilatMakbuzNo");
            tahsilatMakbuzNo.Value = "1";
            XmlAttribute tahsilatMakbuzTarihi = XMLdoc.CreateAttribute("tahsilatMakbuzTarihi");
            tahsilatMakbuzTarihi.Value = "1";
            XmlAttribute teminatNo = XMLdoc.CreateAttribute("teminatNo");
            teminatNo.Value = "1";
            XmlAttribute teminatiVerenKurum = XMLdoc.CreateAttribute("teminatiVerenKurum");
            teminatiVerenKurum.Value = "1";
            XmlAttribute teminatTipi = XMLdoc.CreateAttribute("teminatTipi");
            teminatTipi.Value = "1";
            XmlAttribute teminatNedeni = XMLdoc.CreateAttribute("teminatNedeni");
            teminatNedeni.Value = "1";


            teminat.Attributes.Append(teminatNedeni);
            teminat.Attributes.Append(teminatTipi);
            teminat.Attributes.Append(teminatiVerenKurum);
            teminat.Attributes.Append(teminatNo);
            teminat.Attributes.Append(tahsilatMakbuzTarihi);
            teminat.Attributes.Append(tahsilatMakbuzNo);
            teminat.Attributes.Append(ilamAciklama);
            teminat.Attributes.Append(tutarTur);
            teminat.Attributes.Append(teminatTutar);
            teminat.Attributes.Append(id);

            return teminat;
        }

        private XmlNode UyapXML_paraylaOlculemeyenAlacak_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
             <!ELEMENT paraylaOlculemeyenAlacak EMPTY>
              <!ATTLIST paraylaOlculemeyenAlacak
               id		  ID #IMPLIED
               aciklama	  CDATA #IMPLIED
              >
            */
            #endregion

            XmlNode paraylaOlculemeyenAlacak = XMLdoc.CreateElement("paraylaOlculemeyenAlacak");

            XmlAttribute id = XMLdoc.CreateAttribute("id");
            id.Value = "1";
            XmlAttribute aciklama = XMLdoc.CreateAttribute("aciklama");
            aciklama.Value = "1";


            paraylaOlculemeyenAlacak.Attributes.Append(aciklama);
            paraylaOlculemeyenAlacak.Attributes.Append(id);

            return paraylaOlculemeyenAlacak;
        }

        private XmlNode UyapXML_iban_Preparing(XmlDocument XMLdoc)
        {
            #region
            /*
             <!ELEMENT iban EMPTY>
              <!ATTLIST iban
                 no		CDATA #IMPLIED
              >
            */
            #endregion

            XmlNode iban = XMLdoc.CreateElement("iban");

            XmlAttribute no = XMLdoc.CreateAttribute("no");
            no.Value = "1";
            
            iban.Attributes.Append(no);

            return iban;
        }

        //---    
        private XmlAttribute XML_Value_Preparing(XmlDocument XMLdoc, DataSet dsData, int RowId, string XMLFieldName)
        {
            tToolBox t = new tToolBox();

            // 1. xmle yüklenecek olan data okunuyor
            /// dsData : gelen datasetin  tablename_FIELDS isimli bir ek tablosu daha mevcut
            /// bu tabloda  XMLFieldName  uygun olduğu gerçek data fieldi tespit ediliyor ve
            /// onun value si okunuyor 
            /// Datanın hangi satırda konumlandığı buraya gönderilmeden önce tespit ediliyor ve
            /// o RowId deki data okunuyor

            // 2. XmlAttribute nesnesini oluştur 
            /// ve uygun bir XmlAttribute create ediliyor ve ona bu okunan value 
            /// set edilerek geriye gönderiliyor

            int i2 = 0;
            string function_name = "XML_Value_Preparing";
            string myProp = dsData.Namespace.ToString();
            string Table_Name = t.Set(t.MyProperties_Get(myProp, "=TableName:"), "", "TABLE1");
            string TableFields = Table_Name + "_FIELDS";
            string fname = string.Empty;
            string xml_fname = string.Empty;
            string value = string.Empty;

            try
            {
                i2 = dsData.Tables[TableFields].Rows.Count;

                if (i2 == 0)
                    MessageBox.Show("DİKKAT : " + Table_Name + " isimli tablonun field listesi bulunamadı ... " + v.ENTER2 +
                                    "( Tables[xxxx_FIELDS].Rows.Count = 0 )", function_name);
            }
            catch
            {
                MessageBox.Show("DİKKAT : " + Table_Name + " isimli tablonun field listesi bulunamadı ... " + v.ENTER2 +
                                "( Tables[xxxx_FIELDS].Rows.Count = 0 )", function_name);
                i2 = 0;
            }


            for (int i = 0; i < i2; i++)
            {
                fname = dsData.Tables[TableFields].Rows[i]["name"].ToString();
                xml_fname = dsData.Tables[TableFields].Rows[i]["XML_FIELD_NAME"].ToString();

                if (XMLFieldName == xml_fname)
                {

                    value = dsData.Tables[Table_Name].Rows[RowId][fname].ToString();
                    break;
                }
            }

            XmlAttribute newAttribute = XMLdoc.CreateAttribute(XMLFieldName);
            newAttribute.Value = value;

            return newAttribute;
        }


        //------------------------------------------------------------------------------
        public void XML_Create(string filename)
        {
            XmlDocument XMLdoc = new XmlDocument();

            XmlNode docNode = XMLdoc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            XMLdoc.AppendChild(docNode);

            XmlNode docComment = XMLdoc.CreateComment(Uretici_Firma_01);
            XMLdoc.AppendChild(docComment);

            XmlNode TablesNode = XMLdoc.CreateElement("SPValues");
            XMLdoc.AppendChild(TablesNode);

            XmlNode Row_Node = XMLdoc.CreateElement("Row");
            XmlAttribute Id_Attribute = XMLdoc.CreateAttribute("id");
            Id_Attribute.Value = "00";
            Row_Node.Attributes.Append(Id_Attribute);

            XmlNode SP_Node = XMLdoc.CreateElement("SP");
            SP_Node.AppendChild(XMLdoc.CreateTextNode("sp_caption"));
            Row_Node.AppendChild(SP_Node);

            XmlNode Value_Node = XMLdoc.CreateElement("Value");
            Value_Node.AppendChild(XMLdoc.CreateTextNode("sp_value"));
            Row_Node.AppendChild(Value_Node);

            TablesNode.AppendChild(Row_Node);

            XMLdoc.Save(@"" + filename + ".xml");

        }

        public void XML_UyapInsert(string filename)
        {
            /*
            XmlDocument XMLdoc = new XmlDocument();

            XMLdoc.Load(@"" + filename + ".xml");

            XmlNode TablesNode = XMLdoc.SelectSingleNode("exchangeData");
            XmlAttribute header_Attribute = XMLdoc.CreateAttribute("exchangeHeader versiyon");
            header_Attribute.Value = "1.1";
            TablesNode.Attributes.Append(header_Attribute);

            XmlNode dosyalar = XMLdoc.CreateElement("dosyalar");
            //dosyalar.AppendChild(XMLdoc.CreateTextNode(spcaption));
            //Row_Node.AppendChild(SP_Node);




            TablesNode.AppendChild(dosyalar);

            XMLdoc.Save(@"" + filename + ".xml");
            */
        }


        public void XML_Insert(string filename, string rowid, string spcaption, string spvalue)
        {
            XmlDocument XMLdoc = new XmlDocument();

            XMLdoc.Load(@"" + filename + ".xml");

            XmlNode TablesNode = XMLdoc.SelectSingleNode("SPValues");

            XmlNode Row_Node = XMLdoc.CreateElement("Row");
            XmlAttribute Id_Attribute = XMLdoc.CreateAttribute("id");
            Id_Attribute.Value = rowid;
            Row_Node.Attributes.Append(Id_Attribute);

            XmlNode SP_Node = XMLdoc.CreateElement("SP");
            SP_Node.AppendChild(XMLdoc.CreateTextNode(spcaption));
            Row_Node.AppendChild(SP_Node);

            XmlNode Value_Node = XMLdoc.CreateElement("Value");
            Value_Node.AppendChild(XMLdoc.CreateTextNode(spvalue));
            Row_Node.AppendChild(Value_Node);

            TablesNode.AppendChild(Row_Node);

            XMLdoc.Save(@"" + filename + ".xml");
        }

        public void XML_Update(string filename, string spcaption, string spvalue)
        {
            XmlDocument XMLdoc = new XmlDocument();

            XMLdoc.Load(@"" + filename + ".xml");

            for (int i = 0; i < XMLdoc.DocumentElement.ChildNodes.Count; i++)
            {
                if (XMLdoc.DocumentElement.ChildNodes[i].ChildNodes[0].InnerText == spcaption)
                {
                    XMLdoc.DocumentElement.ChildNodes[i].ChildNodes[1].InnerText = spvalue;
                    break;
                }
            }


            XMLdoc.Save(@"" + filename + ".xml");

            //XDocument XMLdoc = XDocument.Load(@"" + filename + ".xml");
            //var query = from c in XMLdoc.Elements("SPValues").Elements("Row").Elements("SP")
            //select c;
            //foreach (XElement book in query)
            //{
            //    if (book.Value == spcaption)
            //    {
            //        book.Attribute("Value").Value = "xxxxx";
            //        MessageBox.Show(book.NodeType.ToString() + " : ooookkkk : " + book.Name + " : " + book.Value);
            //    }
            //    else
            //        MessageBox.Show(book.NodeType.ToString() + " : " + book.Name + " : " + book.Value);
            //}

        }

        
        public string XML_Read(string filename, string find_header_caption, ref string my_read_value)
        {
            //  public static void XML_Read(string filename, params string[] find_sp_name)

            Boolean sp_read = false;
            Boolean value_read = false;
            string sp_name = "";
            string sp_value = "";

            // xml içinde aranan SP nin içeriği yazılımcı tarafından boş gönderilebilir 
            if (find_header_caption == "")
            {
                MessageBox.Show("HATA : Değişkenin içeriği yok ... (" + filename + ".xml)");
                return "Error";
            }

            try
            {
                // xml dosyayı okuma için aç
                XmlTextReader reader2 = new XmlTextReader(@"" + filename + ".xml");
                reader2.Read();
                reader2.Close();
            }  // try
            catch
            {
                tToolBox t = new tToolBox();
                string soru = "DİKKAT : " + filename + " isminde aranan XML dosya bulunamadı... " + "\r\n\r\n" + "Bu isimde bir xml dosya oluşturulacak, Onaylıyor musunuz ?";
                DialogResult cevap = t.mySoru(soru);

                if (DialogResult.Yes == cevap)
                {
                    if (filename.IndexOf("Uyap") < 0) XML_Create(filename);
                    //if (filename.IndexOf("Uyap") >= 0) XML_UyapCreate(filename);
                }
                else
                {
                    Application.Exit();
                }
            }


            XmlTextReader reader = new XmlTextReader(@"" + filename + ".xml");

            //reader.Read() = true
            // xml dosyayı oku
            while (reader.Read() == true)
            {
                if ((reader.NodeType.ToString() != "Whitespace") &&
                    (reader.NodeType.ToString() != "XmlDeclaration") &&
                    (reader.NodeType.ToString() != "Comment") &&
                    (reader.NodeType.ToString() != "EndElement"))
                {
                    // okunan node Element ve SP alanı ise
                    if ((reader.NodeType.ToString() == "Element") &&
                        (reader.Name == "SP")) sp_read = true;

                    // SP alanı ise ve okunan text ile aranan text eşit ise 
                    if ((sp_read) &&
                        (reader.Value == find_header_caption))
                        if (reader.NodeType.ToString() == "Text")
                        {
                            sp_name = reader.Value;
                            sp_read = false;
                        }

                    // okunan Element ve Value alanı ve aranan SP text bulunmuş ise
                    if ((reader.NodeType.ToString() == "Element") &&
                        (reader.Name == "Value") &&
                        (sp_name == find_header_caption)) value_read = true;

                    // value alanı okunmaya hazır ise ve value textini bulmuşsak
                    if (value_read)
                    {
                        if (reader.NodeType.ToString() == "Text")
                        {
                            sp_value = reader.Value;
                            value_read = false;
                        }

                        //MessageBox.Show(reader.NodeType.ToString() + " : " + reader.Name + " : " + reader.Value + " : " + reader.ValueType.Name.ToString());

                    }

                    // eğer doğru sp bulunmuş ise ve value değeri okunmuş ise arama bitti 
                    if ((sp_name != "") && (sp_value != ""))
                    {
                        my_read_value = sp_value;
                        sp_name = "";
                        sp_value = "";
                        reader.Close();
                    }

                } // if ((reader.NodeType
            } // while

            // eğer value okuma konumuna kadar gelinmiş fakat value text boş ise
            if ((sp_read) && (sp_name == ""))
            {
                MessageBox.Show("DİKKAT : " + find_header_caption + " için kayıt bulunamadı ...");
                sp_value = "Error";
                reader.Close();
            }


            // eğer value okuma konumuna kadar gelinmiş fakat value text boş ise
            if ((value_read) && (sp_value == ""))
            {
                MessageBox.Show("DİKKAT : " + find_header_caption + " için değer bulunamadı ...");
                sp_value = "Error";
                reader.Close();
            }

            return sp_value;
        }

        #endregion tXML

        //--------

        #region Database Öndeğerleri
        public void Database_Ondegerleri_Oku()
        {
            // xml den value leri okumak için gerekli olan ilk sp değerleri atanıyor

            //--------*****  SP_First_Value((byte)SP_atama.XML_READ_SP);

            // Sırayla SP ler ve Value - değerleri okunacak
            tToolBox t = new tToolBox();
            vUserInputBox iBox = new vUserInputBox();

            // SERVERNAME
            if (XML_Read(SPF_XML_FILENAME, xml_SERVER_PCNAME, ref SPF_SERVER_PCNAME) == "Error") 
            {
                iBox.title = "SQL Server PC Name";
                iBox.promptText = "SQL in yüklü olduğu Bilgisayarın ( Server ) Adı :";
                iBox.value = SPF_SERVER_PCNAME;
                iBox.displayFormat = "";
                iBox.fieldType = 0;
                //if (t.InputBox("SQL Server PC Name", "SQL in yüklü olduğu Bilgisayarın ( Server ) Adı :", ref SPF_SERVER_PCNAME) == DialogResult.OK)
                if (t.UserInpuBox(iBox) == DialogResult.OK)
                {
                    SPF_SERVER_PCNAME = iBox.value;
                    XML_Insert(SPF_XML_FILENAME, "01", xml_SERVER_PCNAME, SPF_SERVER_PCNAME);  
                }
            }

            //v.SP_SERVER_PCNAME = SPF_SERVER_PCNAME;
            v.active_DB.projectServerName = SPF_SERVER_PCNAME;

            // USER_NAME
            if (XML_Read(SPF_XML_FILENAME, xml_USERNAME, ref SPF_USER_NAME) == "Error")
            {
                iBox.title = "Database için User Name";
                iBox.promptText = "Database bağlantısı için User Adı (sa) :";
                iBox.value = SPF_USER_NAME;
                iBox.displayFormat = "";
                iBox.fieldType = 0;
                
                //if (t.InputBox("Database için User Name", "Database bağlantısı için User Adı (sa) :", ref SPF_USER_NAME) == DialogResult.OK)
                if (t.UserInpuBox(iBox) == DialogResult.OK)
                {
                    SPF_USER_NAME = iBox.value;
                    XML_Insert(SPF_XML_FILENAME, "02", xml_USERNAME, SPF_USER_NAME);
                }
            }

            //v.db_USER_NAME = SPF_USER_NAME;
            v.active_DB.projectUserName = SPF_USER_NAME;

            // USER NAME için PASSWORD
            if (XML_Read(SPF_XML_FILENAME, xml_PASSWORD, ref SPF_PASSWORD) == "Error")
            {
                iBox.title = "Database User Şifresi";
                iBox.promptText = "Database User Şifresi :";
                iBox.value = SPF_PASSWORD;
                iBox.displayFormat = "";
                iBox.fieldType = 0;

                //if (t.InputBox("Database User Şifresi", "Database User Şifresi :", ref SPF_PASSWORD) == DialogResult.OK)
                if (t.UserInpuBox(iBox) == DialogResult.OK)
                {
                    SPF_PASSWORD = iBox.value;
                    XML_Insert(SPF_XML_FILENAME, "03", xml_PASSWORD, SPF_PASSWORD);
                }
            }

            //v.db_PASSWORD = SPF_PASSWORD;
            v.active_DB.projectPsw = SPF_PASSWORD;

            // PROJE_DBNAME
            if (XML_Read(SPF_XML_FILENAME, xml_PROJE_DBNAME, ref SPF_PROJE_DBNAME) == "Error")
            {
                iBox.title = "Projenin Database Adı";
                iBox.promptText = "Projenin Database Adı :";
                iBox.value = SPF_PROJE_DBNAME;
                iBox.displayFormat = "";
                iBox.fieldType = 0;

                //if (t.InputBox("Projenin Database Adı", "Projenin Database Adı :", ref SPF_PROJE_DBNAME) == DialogResult.OK)
                if (t.UserInpuBox(iBox) == DialogResult.OK)
                {
                    SPF_PROJE_DBNAME = iBox.value;
                    XML_Insert(SPF_XML_FILENAME, "04", xml_PROJE_DBNAME, SPF_PROJE_DBNAME);
                }
            }
            v.active_DB.projectDBName = SPF_PROJE_DBNAME;

            
            // MANAGER_DBNAME
            if (XML_Read(SPF_XML_FILENAME, xml_MANAGER, ref SPF_MANAGER_DBNAME) == "Error")
            {
                iBox.title = "ManagerServer Database Adı";
                iBox.promptText = "ManagerServer Database Adı :";
                iBox.value = SPF_MANAGER_DBNAME;
                iBox.displayFormat = "";
                iBox.fieldType = 0;

                //if (t.InputBox("ManagerServer Database Adı", "ManagerServer Database Adı :", ref SPF_MANAGER_DBNAME) == DialogResult.OK)
                if (t.UserInpuBox(iBox) == DialogResult.OK)
                {
                    SPF_MANAGER_DBNAME = iBox.value;
                    XML_Insert(SPF_XML_FILENAME, "05", xml_MANAGER, SPF_MANAGER_DBNAME);
                }
            }
            v.active_DB.managerDBName = SPF_MANAGER_DBNAME;

            // MAINMANAGER_DBNAME
            if (XML_Read(SPF_XML_FILENAME, xml_MAINMANAGER, ref SPF_MAINMANAGER_DBNAME) == "Error")
            {
                iBox.title = "Main ManagerServer Database Adı";
                iBox.promptText = "Main ManagerServer Database Adı :";
                iBox.value = SPF_MAINMANAGER_DBNAME;
                iBox.displayFormat = "";
                iBox.fieldType = 0;

                //if (t.InputBox("Main ManagerServer Database Adı", "Main ManagerServer Database Adı :", ref SPF_MAINMANAGER_DBNAME) == DialogResult.OK)
                if (t.UserInpuBox(iBox) == DialogResult.OK)
                {
                    SPF_MAINMANAGER_DBNAME = iBox.value;
                    if (t.IsNotNull(SPF_MAINMANAGER_DBNAME) == false)
                        SPF_MAINMANAGER_DBNAME = "SystemMS";

                    XML_Insert(SPF_XML_FILENAME, "06", xml_MAINMANAGER, SPF_MAINMANAGER_DBNAME);
                }
            }
            v.db_MAINMANAGER_DBNAME = SPF_MAINMANAGER_DBNAME;

            
                
            
            // VT_FIRM CODE 
            if (XML_Read(SPF_XML_FILENAME, xml_VT_FIRM, ref SPF_VT_FIRM) == "Error")
            {
                iBox.title = "Firma Kodu";
                iBox.promptText = "Firma Kodu :";
                iBox.value = SPF_VT_FIRM;
                iBox.displayFormat = "";
                iBox.fieldType = 0;

                //if (t.InputBox("Firma Kodu", "Firma Kodu :", ref SPF_VT_FIRM) == DialogResult.OK)
                if (t.UserInpuBox(iBox) == DialogResult.OK)
                {
                    SPF_VT_FIRM = iBox.value;
                    XML_Insert(SPF_XML_FILENAME, "21", xml_VT_FIRM, SPF_VT_FIRM);
                }
            }
            v.SP_FIRM_ID = t.myInt32(SPF_VT_FIRM);

            // MENU ID
            if (XML_Read(SPF_XML_FILENAME, xml_MENU, ref SPF_MENU) == "Error")
            {
                iBox.title = "Main Menü ID";
                iBox.promptText = "Main Menü ID :";
                iBox.value = SPF_MENU;
                iBox.displayFormat = "";
                iBox.fieldType = 0;

                //if (t.InputBox("Main Menü ID", "Main Menü ID :", ref SPF_MENU) == DialogResult.OK)
                if (t.UserInpuBox(iBox) == DialogResult.OK)
                {
                    SPF_MENU = iBox.value;
                    XML_Insert(SPF_XML_FILENAME, "41", xml_MENU, SPF_MENU);
                }
            }
            v.SP_MENU = SPF_MENU;


            // VT_FIRM SERIAL 
            if (XML_Read(SPF_XML_FILENAME, xml_VT_FIRMSERIAL, ref SPF_VT_FIRMSERIAL) == "Error")
            {
                iBox.title = "Firma Seri Kodu";
                iBox.promptText = "Firma Seri Kodu :";
                iBox.value = SPF_VT_FIRMSERIAL;
                iBox.displayFormat = "";
                iBox.fieldType = 0;

                //if (t.InputBox("Firma Seri Kodu", "Firma Seri Kodu :", ref SPF_VT_FIRMSERIAL) == DialogResult.OK)
                if (t.UserInpuBox(iBox) == DialogResult.OK)
                {
                    SPF_VT_FIRMSERIAL = iBox.value;
                    XML_Insert(SPF_XML_FILENAME, "99", xml_VT_FIRMSERIAL, SPF_VT_FIRMSERIAL);
                }
            }



            // OKUNAN VERİLER BİRLEŞTİRİLECEK 

            // xml den okunan değerler, birleştiriliyor ve böylece
            // database ulaşmayı sağlayacak script hazırlanmış oluyor

            ////**************  SP_First_Value((byte)SP_atama.MSSQL_SP);
        }
        #endregion

        #region Database Öndeğerlerini Yenile
        public void Database_Ondegerleri_Yenile()
        {

            tToolBox t = new tToolBox();
            vUserInputBox iBox = new vUserInputBox();

            //****  SP_First_Value((byte)SP_atama.XML_READ_SP);

            XML_Read(SPF_XML_FILENAME, xml_SERVER_PCNAME, ref SPF_SERVER_PCNAME);

            iBox.Clear();
            iBox.title = "SQL Server PC Name";
            iBox.promptText = "SQL in yüklü olduğu Bilgisayarın ( Server ) Adı :";
            iBox.value = SPF_SERVER_PCNAME;
            iBox.displayFormat = "";
            iBox.fieldType = 0;

            //if (t.InputBox("SQL Server PC Name", "SQL in yüklü olduğu Bilgisayarın ( Server ) Adı :", ref SPF_SERVER_PCNAME) == DialogResult.OK)
            if (t.UserInpuBox(iBox) == DialogResult.OK)
            {
                SPF_SERVER_PCNAME = iBox.value;
                XML_Update(SPF_XML_FILENAME, xml_SERVER_PCNAME, SPF_SERVER_PCNAME);
            }

            XML_Read(SPF_XML_FILENAME, xml_USERNAME, ref SPF_USER_NAME);
            iBox.title = "Database için User Name";
            iBox.promptText = "Database bağlantısı için User Adı (sa) :";
            iBox.value = SPF_USER_NAME;
            iBox.displayFormat = "";
            iBox.fieldType = 0;

            //if (t.InputBox("Database için User Name", "Database bağlantısı için User Adı (sa) :", ref SPF_USER_NAME) == DialogResult.OK)
            if (t.UserInpuBox(iBox) == DialogResult.OK)
            {
                SPF_USER_NAME = iBox.value;
                XML_Update(SPF_XML_FILENAME, xml_USERNAME, SPF_USER_NAME);
            }

            XML_Read(SPF_XML_FILENAME, xml_PASSWORD, ref SPF_PASSWORD);

            iBox.title = "Database User Şifresi";
            iBox.promptText = "Database User Şifresi :";
            iBox.value = SPF_PASSWORD;
            iBox.displayFormat = "";
            iBox.fieldType = 0;

            //if (t.InputBox("Database User Şifresi", "Database User Şifresi :", ref SPF_PASSWORD) == DialogResult.OK)
            if (t.UserInpuBox(iBox) == DialogResult.OK)
            {
                SPF_PASSWORD = iBox.value;
                XML_Update(SPF_XML_FILENAME, xml_PASSWORD, SPF_PASSWORD);
            }

            XML_Read(SPF_XML_FILENAME, xml_PROJE_DBNAME, ref SPF_PROJE_DBNAME);

            iBox.title = "Projenin Database Adı";
            iBox.promptText = "Projenin Database Adı :";
            iBox.value = SPF_PROJE_DBNAME;
            iBox.displayFormat = "";
            iBox.fieldType = 0;

            //if (t.InputBox("Projenin Database Adı", "Projenin Database Adı :", ref SPF_PROJE_DBNAME) == DialogResult.OK)
            if (t.UserInpuBox(iBox) == DialogResult.OK)
            {
                SPF_PROJE_DBNAME = iBox.value;
                XML_Update(SPF_XML_FILENAME, xml_PROJE_DBNAME, SPF_PROJE_DBNAME);
            }
            
            XML_Read(SPF_XML_FILENAME, xml_MANAGER, ref SPF_MANAGER_DBNAME);

            iBox.title = "ManagerServer Database Adı";
            iBox.promptText = "ManagerServer Database Adı :";
            iBox.value = SPF_MANAGER_DBNAME;
            iBox.displayFormat = "";
            iBox.fieldType = 0;

            //if (t.InputBox("ManagerServer Database Adı", "ManagerServer Database Adı :", ref SPF_MANAGER_DBNAME) == DialogResult.OK)
            if (t.UserInpuBox(iBox) == DialogResult.OK)
            {
                SPF_MANAGER_DBNAME = iBox.value;
                XML_Update(SPF_XML_FILENAME, xml_MANAGER, SPF_MANAGER_DBNAME);
            }
/*
            XML_Read(SPF_XML_FILENAME, xml_MENU, ref SPF_MENU);
            if (t.InputBox("Main Menü ID", "Main Menü ID :", ref SPF_MENU) == DialogResult.OK)
            {
                XML_Update(SPF_XML_FILENAME, xml_MENU, SPF_MENU);
            }

            XML_Read(SPF_XML_FILENAME, xml_VT_FIRM, ref SPF_VT_FIRM);
            if (t.InputBox("Firma Kodu", "Firma Kodu :", ref SPF_VT_FIRM) == DialogResult.OK)
            {
                XML_Update(SPF_XML_FILENAME, xml_VT_FIRM, SPF_VT_FIRM);
            }

            XML_Read(SPF_XML_FILENAME, xml_VT_FIRMSERIAL, ref SPF_VT_FIRMSERIAL);
            if (t.InputBox("Firma Seri Kodu", "Firma Seri Kodu :", ref SPF_VT_FIRMSERIAL) == DialogResult.OK)
            {
                XML_Update(SPF_XML_FILENAME, xml_VT_FIRMSERIAL, SPF_VT_FIRMSERIAL);
            }

*/
            // OKUNAN VERİLER BİRLEŞTİRİLECEK 

            // xml den okunan değerler, birleştiriliyor ve böylece
            // database ulaşmayı sağlayacak script hazırlanmış oluyor

            //************** SP_First_Value((byte)SP_atama.MSSQL_SP);

        }
        #endregion

        /*
        public DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new DevExpress.XtraEditors.XtraForm();//new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Font = new Font("Tahoma", 8);
            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "Tamam";
            buttonCancel.Text = "Vazgeç";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();

            value = textBox.Text;

            return dialogResult;
        }
        */


    }
}
