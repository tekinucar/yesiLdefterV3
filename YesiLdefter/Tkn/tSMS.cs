using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tkn_Variable;
using Tkn_ToolBox;
using Tkn_SQLs;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Net;
using System.IO;
using Tkn_Save;

namespace Tkn_SMS
{
    public class tSMS : tBase
    {
        tToolBox t = new tToolBox();

        private List<vSMSSettings> readCrsSmsSettings()
        {
            DataSet ds = new DataSet();

            tSQLs sql = new tSQLs();
            string tSql = sql.Sql_CrsSmsSettings();

            t.SQL_Read_Execute(v.dBaseNo.Project, ds, ref tSql, "CrsSmsSettings", "CrsSmsSettings");

            List<vSMSSettings> SMSSettings_ = t.RunQueryModels<vSMSSettings>(ds);

            return SMSSettings_;
        }

        public bool getSmsSettings(ref vSMSSettings settings)
        {
            bool onay = true;

            /// Firmanın Sms tanımları okunuyor
            /// 
            List<vSMSSettings> SMSSettings_ = readCrsSmsSettings();
            settings = SMSSettings_[0];

            if ((t.IsNotNull(settings.KullaniciAdi) == false) ||
                (t.IsNotNull(settings.Sifre) == false) ||
                (t.IsNotNull(settings.BayiiKodu) == false) ||
                (t.IsNotNull(settings.Origin) == false))
            {
                MessageBox.Show("DİKKAT : SMS tanımlarında eksiklik mevcut ..." + v.ENTER2 +
                    "Kullanıcı Adı, Şifre, Bayii Kodu ve Origin bilgileriniz kontrol ediniz.", "SMS bildirim kanalı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return onay;
        }

        private bool preparingSmsKanalBilgileri(Form tForm, bildirimPaketi tBildirim_, ref DataSet ds, ref DataNavigator dN, ref vSMSSettings settings)
        {
            bool onay = true;

            onay = getSmsSettings(ref settings);

            if (onay == false) return onay;

            /// Hazırlanan bildirim listesi
            /// 
            string tableIPCodeLines = tBildirim_.TableIPCodeLines;
            /// Bildirim lines tablosu
            t.Find_DataSet(tForm, ref ds, ref dN, tableIPCodeLines);

            return onay;
        }

        public int getSMSCreditCount(Form tForm, bildirimPaketi tBildirim_)
        {
            int count = -1;
            bool onay = true;

            DataSet dsLines = null;
            DataNavigator dNLines = null;
            vSMSSettings settings = null;

            onay = preparingSmsKanalBilgileri(tForm, tBildirim_, ref dsLines, ref dNLines, ref settings);

            if (onay)
            {
                string count_ = getSmsCredit(settings);
                Control cntrl = t.Find_Control(tForm, "tLabel_KalanKontorSayisi");
                if (cntrl != null)
                {
                    count = t.myInt32(count_);
                    ((DevExpress.XtraEditors.LabelControl)cntrl).Text = count.ToString();
                }
            }

            return count;
        }

        public bool bildirimleriSMSKanaliylaGonder(Form tForm, bildirimPaketi tBildirim_)
        {
            bool onay = true;

            DataSet dsLines = null;
            DataNavigator dNLines = null;
            vSMSSettings settings = null;

            onay = preparingSmsKanalBilgileri(tForm, tBildirim_, ref dsLines, ref dNLines, ref settings);

            if (onay == false)
                return onay;

            onay = bildirimleriSMSKanaliylaGonder_(tForm, tBildirim_, dsLines, dNLines, settings);

            return onay;
        }
        public bool bildirimleriSMSKanaliylaGonder_(Form tForm, bildirimPaketi tBildirim_, DataSet dsLines, DataNavigator dNLines, vSMSSettings settings)
        {
            bool onay = true;

            Int16 stateTypeId = 0;
            string telefonNo = "";
            string bildirimMetni = "";
            /// "1", "Hemen gönder"
            /// "2", "Zamanlanmış gönderim (Tarih ve saat belirterek)"
            Int16 gonderimType = tBildirim_.secilenGonderimTypeId;
            string gonderimTarihi = tBildirim_.secilenGonderimTarihi.ToString();
            string gonderimSaati = tBildirim_.secilenGonderimSaati;
            string servisinCevabi = "";

            int count = dsLines.Tables[0].Rows.Count;

            for (int i = 0; i < count; i++)
            {
                dNLines.Position = i;

                stateTypeId = t.myInt16(dsLines.Tables[0].Rows[dNLines.Position]["StateTypeId"].ToString());

                if (stateTypeId < 2) // Henüz gönderilmemiş ise
                {
                    telefonNo = dsLines.Tables[0].Rows[dNLines.Position]["TelefonNo"].ToString();
                    bildirimMetni = dsLines.Tables[0].Rows[dNLines.Position]["Mesaj"].ToString();

                    /// test için kullanılıyor
                    //servisinCevabi = "ID:" + t.getNewFileGuidName;
                    
                    /// esas çalışan servis
                    /// 
                    servisinCevabi = sendSmsMessage(settings, telefonNo, bildirimMetni, gonderimType, gonderimTarihi, gonderimSaati);

                    if (servisinCevabi != "")
                    {
                        dsLines.Tables[0].Rows[dNLines.Position]["StateTypeId"] = 2; // Gönderildi işareti
                        dsLines.Tables[0].Rows[dNLines.Position]["ServisinCevabi"] = servisinCevabi;
                        dsLines.Tables[0].Rows[dNLines.Position]["IsLock"] = 1; // Kilitle
                        tSave sv = new tSave();
                        sv.tDataSave(tForm, dsLines, dNLines, dNLines.Position);
                    }
                }
            }

            return onay;
        }


        public bool bildirimleriSMSKanalindaSorgula(Form tForm, bildirimPaketi tBildirim_, vSMSSettings settings, DataSet dsLines, DataNavigator dNLines)
        {
            bool onay = true;

            /*
            DataSet dsLines = null;
            DataNavigator dNLines = null;
            vSMSSettings settings = null;

            onay = preparingSmsKanalBilgileri(tForm, tBildirim_, ref dsLines, ref dNLines, ref settings);

            if (onay == false)
                return onay;
            */
            string mesajId = "";
            string telefonNo = "";
            string raporTarihi = "";
            string raporSorgusu = "";
            Int16 raporTypeId = 0;
            int count = dsLines.Tables[0].Rows.Count;

            for (int i = 0; i < count; i++)
            {
                dNLines.Position = i;

                telefonNo = dsLines.Tables[0].Rows[dNLines.Position]["TelefonNo"].ToString();
                mesajId = dsLines.Tables[0].Rows[dNLines.Position]["ServisinCevabi"].ToString();
                raporTypeId = t.myInt16(dsLines.Tables[0].Rows[dNLines.Position]["RaporTypeId"].ToString());

                /// raporTypeId = 2 iletildi olumlu ise bir daha sormasın 
                if ((mesajId.IndexOf("ID:") == 0) && (raporTypeId != 2)) 
                {
                    raporSorgusu = getSmsMessageReport(settings, mesajId, telefonNo, ref raporTarihi);

                    if (raporSorgusu != "")
                    {
                        dsLines.Tables[0].Rows[dNLines.Position]["RaporTypeId"] = t.myInt16(raporSorgusu);
                        dsLines.Tables[0].Rows[dNLines.Position]["RaporTarihi"] = raporTarihi;
                        tSave sv = new tSave();
                        sv.tDataSave(tForm, dsLines, dNLines, dNLines.Position);
                    }
                }
            }

            return onay;
        }

        private string sendSmsMessage(vSMSSettings settings, string telefonNo, string bildirimMetni, Int16 gonderimType, string gonderimTarihi, string gonderimSaati)
        {
            string servisinCevabi = "";
            string telNo = t.TelefonNumarasiGecerliMi(telefonNo);
            string mesajMetni = t.TurkceKarakterDuzenle(bildirimMetni);
            string gonTarihi = t.SmsTarihKontrolu(gonderimTarihi);
            string gonSaati = t.SmsSaatKontrolu(gonderimSaati);

            if (telNo == "") return "Telefon No uygun değil.";
            if (mesajMetni == "") return "Bildirim metni uygun değil.";

            /// "1", "Hemen gönder"
            /// "2", "Zamanlanmış gönderim (Tarih ve saat belirterek)"
            if (gonderimType == 1)
            {
                gonTarihi = "";
                gonSaati = "";
            }


            /// default 3g bilişim
            /// 
            string url = "";

            /// Şuanda bu kod yapısında bunu ayrışımı yok ama bu bilgi dursun
            ///
            /// AYNI MESAJIN ÇOK NUMARAYA GÖNDERİLMESİ
            if (settings.ServisTypeId == 1 || settings.ServisTypeId == 0)
                url = "http://gateway.3gmesaj.com/SendSmsMany.aspx";
            ///
            /// FARKLI MESAJLARIN FARLI NUMARALARA YOLLANMASI 
            //if (settings.ServisTypeId == 1 || settings.ServisTypeId == 0)
            //    url = "http://gateway.3gmesaj.com/SendSmsMulti.aspx";


            /// 0. Normal SMS 160 Karakter       : strType = "1";
            /// 1. Uzun SMS  612 Karekter        : strType = "5";
            /// 2. Normal SMS Türkçe 70 Karakter : strType = "6";
            /// 3. Uzun SMS Türkçe 268 Karakter  : strType = "7";  
            /// 
            string strType = "1";
            if (mesajMetni.Length > 160)
                strType = "5";
            
            StringBuilder sb = new StringBuilder();
            string stringResponse = "";

            sb.Append("<?xml version='1.0' encoding='ISO-8859-9'?> " + v.ENTER);
            sb.Append("<MainmsgBody>" + v.ENTER);
            sb.Append("<UserName>" + settings.KullaniciAdi + "</UserName>" + v.ENTER);
            sb.Append("<PassWord>" + settings.Sifre + "</PassWord>" + v.ENTER);
            sb.Append("<CompanyCode>" + settings.BayiiKodu + "</CompanyCode>" + v.ENTER);
            sb.Append("<Type>" + strType + "</Type>" + v.ENTER);
            sb.Append("<Developer></Developer>" + v.ENTER);
            sb.Append("<Originator>" + settings.Origin + "</Originator>" + v.ENTER);
            sb.Append("<Version></Version>" + v.ENTER);
            sb.Append("<Mesgbody><![CDATA[" + mesajMetni + "]]></Mesgbody>" + v.ENTER);
            sb.Append("<Numbers>" + telNo + "</Numbers>" + v.ENTER);
            sb.Append("<SDate>" + gonTarihi + gonSaati + "</SDate>" + v.ENTER); /// ddmmyyyyhhmm olması gerekiyor
            sb.Append("<EDate>" + "</EDate>" + v.ENTER);
            sb.Append("</MainmsgBody>" + v.ENTER);

            stringResponse = SmsService(url, sb.ToString());

            if (stringResponse.IndexOf("ID:") == 0)
            {
                servisinCevabi = stringResponse;
            }
            else
            {
                if (stringResponse != "")
                {
                    servisinCevabi = ErrorHandling(stringResponse);
                    MessageBox.Show("" + ErrorHandling(stringResponse) + "", "SMS Gönderim Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            return servisinCevabi;
        }

        private string getSmsMessageReport(vSMSSettings settings, string mesajId, string telefonNo, ref string raporTarihi)
        {

            string telNo = "90" + t.TelefonNumarasiGecerliMi(telefonNo);
            string mesajId_ = mesajId.Replace("ID:", "");
            string servisinCevabi = "";
            string tur = "";
            /// default 3g bilişim
            /// 
            string url = "";
            /// DETAY RAPOR ALIMI  
            if (settings.ServisTypeId == 1 || settings.ServisTypeId == 0)
                url = "http://gateway.3gmesaj.com/Report.aspx";

            String stringResponse = "";
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version='1.0' encoding='ISO-8859-9'?>");
            sb.Append("<ReportMain>");
            sb.Append("<UserName>" + settings.KullaniciAdi + "</UserName>" + v.ENTER);
            sb.Append("<PassWord>" + settings.Sifre + "</PassWord>" + v.ENTER);
            sb.Append("<CompanyCode>" + settings.BayiiKodu + "</CompanyCode>" + v.ENTER);
            sb.Append("<Msgid>" + mesajId + "</Msgid>" + v.ENTER);
            sb.Append("<Type>" + tur + "</Type>" + v.ENTER);
            sb.Append("<Delm>;</Delm>");
            sb.Append("</ReportMain>");

            stringResponse = SmsService(url, sb.ToString());

            ///               ID:xxxxxxxxx 90TelefonNo  cevap gönderildiği tarih
            /// stringResponse = 299038440 905301257470 2 120325121114  
            ///                  1234567890123456789012345678901234567890
            ///                           1         2         3     
            /// veya
            /// stringResponse = 20
            /// dönüyor
            /// 
            stringResponse = stringResponse.Replace(mesajId_, "");
            stringResponse = stringResponse.Replace(telNo, "");
            stringResponse = stringResponse.Trim();

            if (stringResponse.Length > 3)
                servisinCevabi = stringResponse.Substring(0, stringResponse.IndexOf(" "));
            if (stringResponse.Length > 0 && stringResponse.Length < 3)
                servisinCevabi = stringResponse;

            /// böyle yapmamın sebebi RaorTypeId listesi için int çeviriyorum
            /// 1 ve 01 şeklinde şaçma sapan bir cevap döndüğü için 100 + ile ayırmak zorunda kaldım
            if (servisinCevabi == "00") servisinCevabi = "1" + servisinCevabi;
            if (servisinCevabi == "01") servisinCevabi = "1" + servisinCevabi;
            if (servisinCevabi == "20") servisinCevabi = "1" + servisinCevabi;
            if (servisinCevabi == "21") servisinCevabi = "1" + servisinCevabi;

            if ((stringResponse.Length > 10) && (stringResponse.IndexOf(" ") > -1))
            {
                /// 120325121114 sonucunu aldık
                raporTarihi = stringResponse.Substring(stringResponse.IndexOf(" "));
                raporTarihi = raporTarihi.Trim();
                /// Anlaşılır hale çevir : 12.03.2025 12:11
                raporTarihi = raporTarihi.Substring(0, 2) + "." + raporTarihi.Substring(2, 2) + ".20" + raporTarihi.Substring(4, 2) + " " + raporTarihi.Substring(6, 2) + ":" + raporTarihi.Substring(8, 2);
            }

            return servisinCevabi;
        }

        public string getSmsCredit(vSMSSettings settings)
        {
            string servisinCevabi = "";
            string tur = "";
            /// default 3g bilişim
            /// 
            string url = "";
            /// DETAY RAPOR ALIMI  
            if (settings.ServisTypeId == 1 || settings.ServisTypeId == 0)
                url = "http://gateway.3gmesaj.com/QueryCredit.aspx";

            String stringResponse = "";
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version='1.0' encoding='ISO-8859-9'?>");
            sb.Append("<Main>");
            sb.Append("<UserName>" + settings.KullaniciAdi + "</UserName>" + v.ENTER);
            sb.Append("<PassWord>" + settings.Sifre + "</PassWord>" + v.ENTER);
            sb.Append("<CompanyCode>" + settings.BayiiKodu + "</CompanyCode>" + v.ENTER);
            sb.Append("</Main>");

            stringResponse = SmsService(url, sb.ToString());

            if (stringResponse.IndexOf("OK") == 0)
            {
                servisinCevabi = stringResponse;
            }
            else
            {
                if (stringResponse != "")
                {
                    servisinCevabi = ErrorHandling(stringResponse);
                    MessageBox.Show("" + ErrorHandling(stringResponse) + "", "SMS Kredi Sorgu Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            return servisinCevabi;

        }




        /// <summary>
        /// Mesaj gönderme işlemini gerçekleştiren fonksiyon.
        /// </summary>
        /// <param name="URL">Hangi adrese "POST" işleminin gerçekleştirileceği.</param>
        /// <param name="XML">Gönderilecek olan string formatta XML dosyası.</param>
        /// <returns></returns>
        private string SmsService(string URL, string XML)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(XML);
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //HttpStatus' a göre handle işlemi gerçekleştirilebilinir.
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();
                return responseStr;
            }
            else
            {
                return "Hata oluştu ( SmsService ) !";
            }
        }


        /// <summary>
        /// Gelen hata koduna göre hata mesajını döndürür.
        /// </summary>
        /// <param name="HataNumarasi">Response içerisinde gelen hata numarası.</param>
        /// <returns></returns>
        private string ErrorHandling(string HataNumarasi)
        {
            string cevap = "";

            switch (HataNumarasi)
            {
                case "00":
                    cevap = "Kullanıcı Bilgileri Boş";
                    break;
                case "01":
                    cevap = "Kullanıcı Bilgileri Hatalı";
                    break;
                case "02":
                    cevap = "Hesap Kapalı";
                    break;
                case "03":
                    cevap = "Kontör Hatası";
                    break;
                case "04":
                    cevap = "Bayi Kodunuz Hatalı";
                    break;
                case "05":
                    cevap = "Originator Bilginiz Hatalı";
                    break;
                case "06":
                    cevap = "Yapılan İşlem İçin Yetkiniz Yok";
                    break;
                case "10":
                    cevap = "Geçersiz IP Adresi";
                    break;
                case "14":
                    cevap = "Mesaj Metni Girilmemiş";
                    break;
                case "15":
                    cevap = "GSM Numarası Girilmemiş";
                    break;
                case "20":
                    cevap = "Rapor Hazır Değil";
                    break;
                case "27":
                    cevap = "Aylık Atım Limitiniz Yetersiz";
                    break;
                case "100":
                    cevap = "XML Hatası";
                    break;
                default: return "Hata Numarası : " + HataNumarasi;
            }
            return cevap;
        }

    }



}
