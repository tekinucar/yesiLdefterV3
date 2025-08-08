using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

using Tkn_Events;
using Tkn_ToolBox;
using Tkn_InvoiceTasks;
using Tkn_EFaturaUyum;
using Tkn_VariableStokHL;
using Tkn_Save;
using Tkn_Events;
using YesiLdefter.ServiceReferenceUyumTest;
using DevExpress.XtraEditors;


namespace YesiLdefter
{
    public partial class ms_EFatura : Form
    {
        #region Tanımlar
        tToolBox t = new tToolBox();
        tEvents ev = new tEvents();
        tEFaturaUyum ef = new tEFaturaUyum();

        string menuName = "MENU_" + "UST/OMS/STS/EFatura";
        string buttonKayitliMukellef = "KAYITLI_MUKELLEF";
        string buttonFaturayiGonder = "EFATURAYI_GONDER";
        string buttonFaturaOnizleme = "FATURA_ONIZLEME";

        faturaGonderici tFaturaGonderici = null;
        List<stokB> tStokB = null;
        List<stokS> tStokS = null;

        DataSet ds_stokB = null;
        DataNavigator dN_stokB = null;
        DataSet ds_stokS = null;
        DataNavigator dN_stokS = null;

        WebBrowser webFatura = null;

        #endregion Tanımlar
        public ms_EFatura()
        {
            InitializeComponent();

            tEventsForm evf = new tEventsForm();

            this.Load += new System.EventHandler(evf.myForm_Load);
            this.Shown += new System.EventHandler(evf.myForm_Shown);
            this.Shown += new System.EventHandler(this.ms_EFatura_Shown);

            this.KeyPreview = true;
        }

        private void ms_EFatura_Shown(object sender, EventArgs e)
        {

            t.Find_Button_AddClick(this, menuName, buttonKayitliMukellef, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonFaturayiGonder, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonFaturaOnizleme, myNavElementClick);

            //t.Find_NavButton_Control(this, menuName, buttonManuelSave, ref buttonManuelSaveControl);
            //t.Find_NavButton_Control(this, menuName, buttonAutoSave, ref buttonAutoSaveControl);

            if (preparingDataSets() == false) this.Close();

            /// 
            ///
            preparingWebBrowser();
        }

        private void preparingWebBrowser()
        {
            webFatura = (WebBrowser)t.Find_Control(this, "WebFatura");

            if (webFatura != null)
            {
                webFatura.ScriptErrorsSuppressed = true;
                webFatura.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webFatura_DocumentCompleted);
            }
            else
            {
                MessageBox.Show("DİKKAT : Fatura önizlemesinde kullanılacak WebBrowser için CmpName = WebFatura tanımı bulunamadı...");
            }
        }
        private void webFatura_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ///---
        }

        private bool preparingDataSets()
        {
            string stokB_TableIPCode = t.Find_TableIPCode(this, "OnmBelgeStokB");

            if (t.IsNotNull(stokB_TableIPCode) == false)
            {
                MessageBox.Show("DİKKAT : Fatura Başlığı tablosu bulunamadı...(Form bilgisi olmayabilir... ( ms_EFatura )) ");
                return false;
            }

            string stokS_TableIPCode = t.Find_TableIPCode(this, "OnmBelgeStokS");

            if (t.IsNotNull(stokS_TableIPCode) == false)
            {
                MessageBox.Show("DİKKAT : Fatura Satırları tablosu bulunamadı...(Form bilgisi olmayabilir... ( ms_EFatura )) ");
                return false;
            }

            t.Find_DataSet(this, ref ds_stokB, ref dN_stokB, stokB_TableIPCode);
            t.Find_DataSet(this, ref ds_stokS, ref dN_stokS, stokS_TableIPCode);

            return true;
        }

        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonKayitliMukellef) KayitliMukellef();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonFaturayiGonder) EFaturayiGonder();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonFaturaOnizleme) FaturaOnizleme();
            }
            // SubItem butonlar
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                //if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonMsfMsfIP) CopyMsfMsfIP();
            }
        }


        private void KayitliMukellef()
        {
            string lblResultText = string.Empty;
            string txtOrnekVknText = "1111111104";
            var client = tInvoiceTasks.Instance.CreateClient();
            try
            {
                var response = client.IsEInvoiceUser(tInvoiceTasks.Instance.GetUserInfo(), txtOrnekVknText, "");
                if (response.Value)
                {
                    lblResultText = "Giridğiniz VKN kayıtlı mükelleftir.";
                }
                else
                {
                    lblResultText = "Giridğiniz VKN kayıtlı mükellef değildir.";
                }

                MessageBox.Show(lblResultText);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void EFaturayiGonder()
        {
            string belgeNo = this.ds_stokB.Tables[0].Rows[0]["BelgeNo"].ToString();

            if (belgeNo != "")
            {
                t.FlyoutMessage(this, "Bilgilendirme", "Bu fatura daha önce gönderilmiştir, tekrar gönderemezsiniz...");
                return;
            }

            string soru = " Fatura gönderilecek, onaylıyor musunuz ?";
            DialogResult cevap = t.mySoru(soru);
            if (DialogResult.Yes != cevap) return;

            /// WebService ile bağlantı kuruluyor
            /// 
            var client = tInvoiceTasks.Instance.CreateClient();

            /// Gönderilecek fatura datasını oku
            /// 
            preparingInvoiceData();

            /// Faturayı oluştur
            /// 
            var invoiceInfo =  ef.CreateInvoice(tFaturaGonderici, tStokB, tStokS);

            /// Faturayı zarfla
            /// 
            InvoiceInfo[] invoices = new InvoiceInfo[1];
            invoices[0] = invoiceInfo;

            /// Faturayı gönder
            /// 
            var response = client.SendInvoice(tInvoiceTasks.Instance.GetUserInfo(), invoices);

            // c3267533-e5cf-4ade-bc2b-3cc0562c9b85 ilk kayıt ıd :)

            if (response.IsSucceded)
            {
                this.ds_stokB.Tables[0].Rows[0]["GIBUUID"] = response.Value[0].Id.ToString();
                this.ds_stokB.Tables[0].Rows[0]["BelgeNo"] = response.Value[0].Number.ToString();
                this.ds_stokB.Tables[0].Rows[0]["GIBInvoiceScenario"] = response.Value[0].InvoiceScenario.ToString();
                this.ds_stokB.Tables[0].AcceptChanges();

                tSave sv = new tSave();
                sv.tDataSave(this, this.ds_stokB, this.dN_stokB, this.dN_stokB.Position);

                MessageBox.Show(
                    string.Format("Fatura Gönderildi\n UUID:{0} \n ID:{1} \n Fatura Tipi:{2} ",
                            response.Value[0].Id.ToString(),
                            response.Value[0].Number.ToString(),
                            response.Value[0].InvoiceScenario.ToString()
                            )
                            );
            }
            else
            {
                MessageBox.Show(response.Message);
            }


        }

        private void preparingInvoiceData()
        {
            tFaturaGonderici = null;
            tStokB = null;
            tStokS = null;

            tFaturaGonderici = new faturaGonderici();
            tStokB = new List<stokB>();
            tStokS = new List<stokS>();

            tFaturaGonderici.Unvan = "Uyumsoft Bilgi Sistemleri ve Teknolojileri A.Ş.";
            tFaturaGonderici.Vkn = "9000068418";
            tFaturaGonderici.VergiDairesiAdi = "Esenler";
            tFaturaGonderici.Ulke = "Türkiye";
            tFaturaGonderici.Il = "İstanbul";
            tFaturaGonderici.Ilce = "Esenler";
            tFaturaGonderici.MahhalleKoy = "Esenler";
            tFaturaGonderici.CaddeSokak = "YTÜ Davutpaşa Kampüsü";
            tFaturaGonderici.DisKapiNo = "B2";
            tFaturaGonderici.IcKapiNo = "401";
            tFaturaGonderici.MersisNo = "12345669-111";
            tFaturaGonderici.TicaretSicilNo = "12345669-111";

            /// Fatura başlığı
            tStokB = t.RunQueryModels<stokB>(this.ds_stokB);
            /// Fatura satırları
            tStokS = t.RunQueryModels<stokS>(this.ds_stokS);
        }

        private void FaturaOnizleme()
        {
            /// Önizlemesi yapılacak fatura datasını oku
            /// 
            preparingInvoiceData();
            
            /// Faturayı oluştur
            /// 
            var invoiceInfo = ef.CreateInvoice(tFaturaGonderici, tStokB, tStokS);
            var invoice = new InvoiceType[1];
            invoice[0] = invoiceInfo.Invoice;

            /// 
            ///
            InvoiceType invoice_ = invoice[0];
            ///
            ///
            showInvoice(invoice_);

        }

        private void showInvoice(InvoiceType invoice)
        {
            if (webFatura == null)
            {
                MessageBox.Show("WebFatura isimli WebBrowser bulunamdı...");
                return;
            }
            try
            {
                var xslt = string.Empty;

                if (invoice.AdditionalDocumentReference != null)
                {
                    AttachmentType attachment = null;
                    DocumentReferenceType doc;
                    byte[] xsltObject = null;

                    for (int i = 0; i < invoice.AdditionalDocumentReference.Length; i++)
                    {
                        doc = invoice.AdditionalDocumentReference[i];
                        attachment = doc.Attachment;
                        if (attachment != null && attachment.EmbeddedDocumentBinaryObject.filename != null)
                        {
                            string fileName = attachment.EmbeddedDocumentBinaryObject.filename;
                            if (Path.GetExtension(fileName) == ".xslt" || Path.GetExtension(fileName) == ".XSLT")
                            {
                                xsltObject = attachment.EmbeddedDocumentBinaryObject.Value;
                            }
                        }
                    }

                    if (xsltObject != null)
                    {
                        #region örnek
                        //var fileStream = File.Create("");
                        //fileStream.Write(xsltObject, 0, 0);
                        //fileStream.

                        //using (var stream = new FileStream(xsltObject))
                        //{
                        //    stream.Seek(0, SeekOrigin.Begin);

                        //    using (var reader = new StreamReader(stream))
                        //    {

                        //        xslt = reader.ReadToEnd();

                        //        //xslt = xslt.Replace("n1:Invoice", "Invoice");

                        //        XmlSerializer serializer = new XmlSerializer(typeof(Invoice));
                        //        using (MemoryStream mstr = new MemoryStream())
                        //        {
                        //            serializer.Serialize(mstr, invoice, InvoiceNamespaces);

                        //            string xml = Encoding.UTF8.GetString(mstr.ToArray());
                        //            webBrowser1.DocumentText = TransformXMLToHTML(xml, xslt);
                        //        }
                        //    }
                        //}
                        #endregion

                        using (var stream = new MemoryStream(xsltObject))
                        {
                            stream.Seek(0, SeekOrigin.Begin);

                            using (var reader = new StreamReader(stream))
                            {

                                xslt = reader.ReadToEnd();

                                //xslt = xslt.Replace("n1:Invoice", "Invoice");
                                var rootAttribute = new XmlRootAttribute("Invoice")
                                {
                                    Namespace = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2",
                                    IsNullable = false
                                };

                                XmlSerializer serializer = new XmlSerializer(typeof(InvoiceType), rootAttribute);
                                using (MemoryStream mstr = new MemoryStream())
                                {
                                    serializer.Serialize(mstr, invoice, InvoiceNamespaces);

                                    string xml = Encoding.UTF8.GetString(mstr.ToArray());
                                    webFatura.DocumentText = TransformXMLToHTML(xml, xslt);

                                    string controlName = "TabPage";
                                    ev.subViewExec(this, controlName, "", "", "tLayout_30_20", "", "", "");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Xslt Dosyası Bulunamadı");
                    }
                }
                else
                {

                    MessageBox.Show("Varsayılan Xslt üzerinden görüntüleme yapılacak");
                    xslt = Properties.Resources.XSLTFile;
                    var rootAttribute = new XmlRootAttribute("Invoice")
                    {
                        Namespace = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2",
                        IsNullable = false
                    };

                    XmlSerializer serializer = new XmlSerializer(typeof(InvoiceType), rootAttribute);
                    using (MemoryStream mstr = new MemoryStream())
                    {
                        serializer.Serialize(mstr, invoice, InvoiceNamespaces);

                        string xml = Encoding.UTF8.GetString(mstr.ToArray());
                        webFatura.DocumentText = TransformXMLToHTML(xml, xslt);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatura gösteriminde hata : " + ex.Message);
                //throw;
            }
        }

        /// XML serialization sırasında namespace'lerin doğru yapılabilmesi için namespace tanımlamaları
        /// 
        private XmlSerializerNamespaces _InvoiceNamespaces;
        private XmlSerializerNamespaces InvoiceNamespaces
        {
            get
            {
                if (_InvoiceNamespaces == null)
                {
                    _InvoiceNamespaces = new XmlSerializerNamespaces();
                    _InvoiceNamespaces.Add("", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
                    _InvoiceNamespaces.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                    _InvoiceNamespaces.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                    _InvoiceNamespaces.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                    _InvoiceNamespaces.Add("cctc", "urn:un:unece:uncefact:documentation:2");
                    _InvoiceNamespaces.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
                    _InvoiceNamespaces.Add("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
                    _InvoiceNamespaces.Add("ubltr", "urn:oasis:names:specification:ubl:schema:xsd:TurkishCustomizationExtensionComponents");
                    _InvoiceNamespaces.Add("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
                    _InvoiceNamespaces.Add("xades", "http://uri.etsi.org/01903/v1.3.2#");
                    _InvoiceNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                }
                return _InvoiceNamespaces;
            }
        }
        
        /// Bu metot verilen xml ve xslt parametrelerini kullanarak html transformu sağlar. 
        /// 
        private string TransformXMLToHTML(string inputXml, string xsltString)
        {
            StringWriter results = null;
            XslCompiledTransform transform = new XslCompiledTransform();
            using (XmlReader reader = XmlReader.Create(new StringReader(xsltString)))
            {
                try
                {
                    transform.Load(reader);
                    results = new StringWriter();
                    using (XmlReader reader2 = XmlReader.Create(new StringReader(inputXml)))
                    {
                        transform.Transform(reader2, null, results);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(" HTML dönüşümü başarısız: {0}", ex.Message));

                }
            }
            if (results != null)
            {
                return results.ToString();
            }
            else
            {
                return null;
            }
        }

    }
}
