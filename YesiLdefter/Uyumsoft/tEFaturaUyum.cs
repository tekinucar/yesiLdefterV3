using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YesiLdefter.ServiceReferenceUyumTest;
using Tkn_VariableStokHL;
using Tkn_ToolBox;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;
using System.IO;

namespace Tkn_EFaturaUyum
{
    public class tEFaturaUyum
    {
        tToolBox t = new tToolBox();

        public InvoiceInfo CreateInvoice(faturaGonderici tFaturaGonderici, List<stokB> tStokB, List<stokS> tStokS)
        {
            var invoice = new InvoiceType
            {
                #region Genel Fatura Bilgileri
                ProfileID = new ProfileIDType { Value = tStokB[0].Lkp_FProfiliTipi }, // cmbFaturaTuru.Text },
                CopyIndicator = new CopyIndicatorType { Value = false },
                UUID = new UUIDType { Value = Guid.NewGuid().ToString() }, //Set edilmediğinde sistem tarafından otomatik verilir. 
                IssueDate = new IssueDateType { Value = tStokB[0].BelgeTarihi }, // dpFaturaTarihi.Value },
                IssueTime = new IssueTimeType { Value = tStokB[0].BelgeTarihi }, // dpFaturaTarihi.Value },
                InvoiceTypeCode = new InvoiceTypeCodeType { Value = tStokB[0].Lkp_FaturaTipi }, // cmbInvoicetypeCode.Text },
                Note = new NoteType[] {
                    new NoteType { Value = tStokB[0].FaturaNotu }, //  txtFaturaNotu.Text }, 
                    new NoteType { Value = tStokB[0].FaturaNotu }, // txtFaturaNotu.Text }, 
                    new NoteType { Value = "Bayi No: 112221" },
                    new NoteType { Value = "Test Not alanı 3" } },
                DocumentCurrencyCode = new DocumentCurrencyCodeType { Value = "TRY" },
                PricingCurrencyCode = new PricingCurrencyCodeType { Value = "TRY" },
                LineCountNumeric = new LineCountNumericType { Value = tStokS.Count },  //2 },
                //PaymentTerms = new PaymentTermsType { Note = new NoteType { Value = "30 gün vadeli" }, Amount = new AmountType1 { Value = 100, currencyID = "TRY" } },
                //PaymentMeans = new PaymentMeansType[] { new PaymentMeansType { PaymentDueDate = new PaymentDueDateType { Value = DateTime.Now.AddDays(15) }, PaymentMeansCode = new PaymentMeansCodeType1 { Value = "42" } } },
                //Delivery = new DeliveryType { DeliveryParty = new PartyType { };
                // PricingExchangeRate = new ExchangeRateType{ SourceCurrencyCode= "TRY",}
                #endregion

                #region SGK fatura alanları
                //AccountingCost = cmbInvoicetypeCode.Text == "SGK" ? new AccountingCostType { Value = cmbSgkInvoicetype.Text } : null,
                //InvoicePeriod = new PeriodType { StartDate = new StartDateType { Value = dpInvoicePeriodStart.Value }, EndDate = new EndDateType { Value = dpInvoicePeriodEnd.Value } },
                #endregion

                AllowanceCharge = new AllowanceChargeType[]
                {
                    new AllowanceChargeType {
                        ChargeIndicator = new ChargeIndicatorType { Value = true },
                        Amount = new AmountType2 { currencyID = "TRY", Value = 100 },
                        AllowanceChargeReason = new AllowanceChargeReasonType { Value = "Bayi İskontosu" }, }
                },

                //  BillingReference = new BillingReferenceType {   BillingReferenceLine = new BillingReferenceLineType[] { new BillingReferenceLineType {  } } }

                // AllowanceCharge = new AllowanceChargeType[] { new AllowanceChargeType { AllowanceChargeReason="Sigorta", ChargeIndicator = true },  }

                #region İrsaliye Bilgileri

                /// Irsaliye dosyasi               
                ///
                //DespatchDocumentReference = new DocumentReferenceType[]{ 
                //    new DocumentReferenceType{
                //        IssueDate= new IssueDateType{ Value=DateTime.Now},  
                //        DocumentType= new DocumentTypeType{  Value = "Irsaliye" }, 
                //        ID= new IDType{Value="IRS000000001"}},
                //    new DocumentReferenceType{
                //        IssueDate= new IssueDateType{ Value=DateTime.Now},  
                //        DocumentType= new DocumentTypeType{  Value = "Irsaliye" }, 
                //        ID= new IDType{Value="IRS000000002"}}},

                #endregion

                #region Xslt ve Ek belgeler
                ///Fatura içerisinde görünüm dosyasını set etme. Değer geçilmediğinde varsayılan xslt kullanılır. 
                
                AdditionalDocumentReference = GetXsltAndDocuments(),
                //

                ////               AdditionalDocumentReference = new DocumentReferenceType { DocumentType= new DocumentTypeType{ Value="SATINALAMA BELGESİ"}, IssueDate=new IssueDateType{ Value= DateTime.Now},ID= new IDType{ Value="12345"}};
                //#endregion

                //#region Additional Document Reference
                //new DocumentReferenceType[]{
                //    new  DocumentReferenceType {
                //    ID = new IDType{ Value = new Guid().ToString()},
                //    IssueDate = new IssueDateType{ Value = DateTime.Now},
                //    Attachment= new AttachmentType{ 
                //                                    EmbeddedDocumentBinaryObject= new EmbeddedDocumentBinaryObjectType{ 
                //                                                                                                       filename="customxslt.xslt", 
                //                                                                                                        encodingCode= "Base64",
                //                                                                                                         mimeCode= BinaryObjectMimeCodeContentType.applicationxml,
                //                                                                                                        format="", 
                //                                                                                                        characterSetCode="UTF-8",
                //                                                                                                        Value = Encoding.UTF8.GetBytes(Properties.Resources.xslt) }}},


                // },
                #endregion

                #region Order Document Reference // Sipariş Belgesi Referansı
                // tkn : ben kapattım
                // OrderReference = GetOrderReference(),
                #endregion

                #region Fatura Seri ve numarası
                // tkn : FaturaNo sistem tarafından verilsin 
                ID = new IDType { Value = "" }, //txtFaturaNumarasi.Text }, //Set edilmediğinde sistem tarafından otomatik verilir. 
                #endregion

                #region Gönderici Bilgileri - AccountingSupplierParty
                AccountingSupplierParty = new SupplierPartyType
                {
                    Party = new PartyType
                    {
                        PartyName = new PartyNameType { Name = new NameType1 { Value = tFaturaGonderici.Unvan } },// txtGondericiUnvan.Text } },
                        PartyIdentification = new PartyIdentificationType[] {
                            new PartyIdentificationType() { ID = new IDType { Value = tFaturaGonderici.Vkn, schemeID = "VKN" } },   //txtGondericiVkn.Text, schemeID = "VKN" } }, 
                            new PartyIdentificationType() { ID = new IDType { Value = tFaturaGonderici.MersisNo, schemeID = "MERSISNO" } },  //"12345669-111", schemeID = "MERSISNO" } }, 
                            new PartyIdentificationType() { ID = new IDType { Value = tFaturaGonderici.TicaretSicilNo, schemeID = "TICARETSICILNO" } } }, //"12345669-111", schemeID = "TICARETSICILNO" } } },

                        PostalAddress = new AddressType
                        {
                            CityName = new CityNameType { Value = tFaturaGonderici.Il }, // txtGondericiIl.Text },
                            StreetName = new StreetNameType { Value = tFaturaGonderici.CaddeSokak },  // txtGondericiCaddeSokak.Text },
                            Country = new CountryType { Name = new NameType1 { Value = tFaturaGonderici.Ulke } }, // txtGondericiUlke.Text } },
                            Room = new RoomType { Value = tFaturaGonderici.IcKapiNo }, // txtGondericiKapiNo.Text },
                            BuildingNumber = new BuildingNumberType { Value = tFaturaGonderici.DisKapiNo }, // txtGondericiKapiNo.Text },
                            CitySubdivisionName = new CitySubdivisionNameType { Value = tFaturaGonderici.Ilce }, // txtGoncericiIlce.Text },
                        },
                        // PartyIdentification = new PartyIdentificationType[] { new PartyIdentificationType() { ID = new IDType { Value = "77777777701", schemeID = "TCKN" } } },
                        // Person = new PersonType{ FirstName= new FirstNameType{ Value="Ahmet"}, FamilyName= new FamilyNameType{ Value="Altınordu"} },
                        // PartyTaxScheme = new PartyTaxSchemeType { TaxScheme = new TaxSchemeType { Name = new NameType1 { Value = "Esenler" } } },
                        PartyTaxScheme = new PartyTaxSchemeType { TaxScheme = new TaxSchemeType { Name = new NameType1 { Value = tFaturaGonderici.VergiDairesiAdi } } }, // txtGondericiVergiDairesi.Text } } },
                    }
                },
                #endregion

                #region Alıcı Bilgileri 
                AccountingCustomerParty = GetAccountingCustomerParty(tStokB[0]),
                #endregion Alıcı Bilgileri 

                #region IHRACAT veya YOLCUBERABERFATURA ise Alıcı Bilgileri
                BuyerCustomerParty = GetBuyerCustomerParty(tStokB[0]),
                #endregion

                /// Taksiler için galiba
                /// TaxRepresentativeParty = GetTaxRepresantiveParty(tStokB),

                #region Fatura Satırları - InvoiceLines
                /// Fatura Satırları
                InvoiceLine = GetInvoiceLines(tStokS),
                #endregion

                #region Vergi Alt Toplamları - TaxTotal

                /// Fatura altı Genel KDV ler
                TaxTotal = GetHeaderTaxTotalTypes(tStokB[0]),

                #endregion

                #region Tevkifatlar

                // WithholdingTaxTotal = new TaxTotalType[] { new TaxTotalType { TaxSubtotal,taxamo     } }

                #endregion

                #region Yasal Alt Toplamlar - Legal Monetary Total
                LegalMonetaryTotal = GetMonetaryTotalType(tStokB[0]),
                #endregion

            };

            #region e-Arşiv Fatura Bilgileri
            //Bu alanda eğer fatura bir e-arşiv faturası ise doldurulması gerkene alanlar doldurulmalıdır.
            EArchiveInvoiceInformation earchiveinfo = GetEArchiveInvoice(tStokB[0]);
            #endregion

            return new InvoiceInfo
            {
                EArchiveInvoiceInfo = earchiveinfo,
                LocalDocumentId = "", // txtLocalDocumentId.Text, entegrator versin
                Invoice = invoice,
                TargetCustomer = new CustomerInfo { Alias = tStokB[0].CariEFaturaAliasName == "" ? "" : tStokB[0].CariEFaturaAliasName },  //txtAliciAlias.Text == "" ? "" : txtAliciAlias.Text },
                Scenario = InvoiceScenarioChoosen.Automated,
                ExtraInformation = "",
                // şimdilik gerek yok gibi
                //ExtraInformation =  tStokB[0].CariExtraInformation == "" ? null : tStokB[0].CariExtraInformation, //  txtExtraInformation.Text == "" ? null : txtExtraInformation.Text,

                //Notification = new NotificationInformation { 

                //    new MailingInformation { //Birden fazla bilgilendirme yapısı desteklenmiştir. Örneği muhasebeciye attachment olan diğer kişilere link olan mail gönderimi yapılmak istenirse yeni bir instance oluşturulup farklı gönderimler yapılabilir. 
                //    EnableNotification = true, //Mail gönderilecek mi bilgisi? 
                //    Attachment = new MailAttachmentInformation { Xml=true,Pdf=true }, //Mailde attachment olacaksa hangi tipte attachment olacak. 
                //    //EmailAccountIdentifier = "127ADE38-0BCB-4AC3-9830-B30A939AA8E9", //Bu Id canlı sistemde ayrıca sizinle paylaşılacaktır. Bir firmanın 1'den fazla mail sunucusu kullanılaiblir. Hangi sunucu ise o sunucu buradan belirtilecek
                //    To = "faruk.kaygisiz@uyumsoft.com.tr", //mail kime/kimlere gönderilecek
                //   // BodyXsltIdentifier = "C5A2BD86-4054-4387-9499-831AC6B108CA", // Bu Id canlı sistemde bizim tarafımızdan size sağlanacaktır. 
                //    Subject = "1234567689 abone numaranıza ait faturanız" // Mailin Subjecti ne olacak. 

                //    }
                //}

            };

        }
            
        private MonetaryTotalType GetMonetaryTotalType(stokB tStokB)
        {
            /// MonetaryTotalType (Fatura Toplamları) Nedir?
            /// MonetaryTotalType (UBL'de LegalMonetaryTotal), 
            /// e-faturanın toplam finansal değerlerini (ara toplam, vergi, indirim, ödenecek tutar vb.) bir araya getiren kritik yapıdır. 
            /// Faturanın "özet bölümü" gibi düşünülebilir.
            /// 


            /// Temel Bileşenleri ve Formüller:
            /// Alan                      Formül                                      Açıklama
            /// ---------------------------------------------------------------------------------------------------------------------------- 
            /// LineExtensionAmount       -                                            Vergi öncesi satır toplamı(Tüm satırların net tutarı)
            /// TaxExclusiveAmount        LineExtensionAmount + Allowance / Charge     Vergi matrahı(KDV öncesi son tutar)
            /// TaxInclusiveAmount        TaxExclusiveAmount + TotalTax                KDV dahil toplam
            /// AllowanceTotalAmount      -                                            Fatura geneli indirimler toplamı(negatif değer)
            /// ChargeTotalAmount         -                                            Fatura geneli ek ücretler toplamı(pozitif değer)
            /// PayableAmount             TaxInclusiveAmount                           Ödenecek son tutar(Faturanın tahsilat değeri)
            /// PayableRoundingAmount     -                                            Yuvarlama farkı(0.01 - 0.99 kuruş düzeltmeleri)
            /// ---------------------------------------------------------------------------------------------------------------------------- 

            MonetaryTotalType legalMonetaryTotal = new MonetaryTotalType
            {
                LineExtensionAmount = new LineExtensionAmountType { Value = tStokB.FsIOVergisizToplamTutar, currencyID = "TRY" },
                TaxExclusiveAmount = new TaxExclusiveAmountType { Value = tStokB.FsISVergisizToplamTutar, currencyID = "TRY" },
                TaxInclusiveAmount = new TaxInclusiveAmountType { Value = tStokB.FsVergilerDahilToplamTutar, currencyID = "TRY" },
                AllowanceTotalAmount = new AllowanceTotalAmountType { Value = tStokB.FsIskontoTutariToplam, currencyID = "TRY" },
                PayableAmount = new PayableAmountType { Value = tStokB.FsVergilerDahilToplamTutar, currencyID = "TRY" },
                PayableRoundingAmount = new PayableRoundingAmountType { Value = tStokB.FsYuvarlamaTutari, currencyID = "TRY" }
            };

            return legalMonetaryTotal;
        }


        private TaxTotalType[] GetWithholdingTaxTotalType()
        {
            /// Tevkifatlı fatura hesaplaması, 
            /// Türkiye’de KDV(Katma Değer Vergisi) uygulamasına göre bazı mal ve hizmet alımlarında, 
            /// KDV’nin bir kısmının alıcı tarafından kesilerek doğrudan vergi dairesine ödenmesi esasına dayanır. 
            /// Bu işlem, özellikle vergi güvenliğini artırmak ve tahsilatı garanti altına almak için kullanılır.
            /// 

            /// Hesaplama Adımları
            /// İşlem Bedelini Belirle Faturadaki KDV hariç tutar(örneğin: 10.000 TL).
            /// KDV Tutarını Hesapla KDV oranı genellikle % 20’dir: KDV = 10.000 TL × 0.20 = 2.000 TL
            /// Tevkifat Oranını Uygula Örneğin tevkifat oranı 7 / 10 ise: Tevkifat Tutarı = 2.000 TL × 0.70 = 1.400 TL Bu tutar alıcı tarafından vergi dairesine beyan edilir.
            /// Satıcının Tahsil Edeceği KDV 2.000 TL - 1.400 TL = 600 TL Satıcı sadece bu kısmı tahsil eder ve beyan eder.
            /// Toplam Fatura Tutarı 10.000 TL + 600 TL = 10.600 TL Alıcı bu tutarı satıcıya öder, 1.400 TL’yi ise doğrudan devlete yatırır.
            ///


            return null;
        }

        /// 
        /// Fatura satırları
        /// 
        private InvoiceLineType[] GetInvoiceLines(List<stokS> tStokS)
        {
            if (tStokS.Count == 0) return null;
            int lineCount = tStokS.Count;

            InvoiceLineType[] invoiceLines = new InvoiceLineType[lineCount];

            for (int i = 0; i < lineCount; i++)
            {
                invoiceLines[i] = new InvoiceLineType
                {
                    /// Ürün Açıklamaları
                    Item = new ItemType
                    {
                        Name = new NameType1 { Value = tStokS[i].UrunAdi }, //  txtUrunAdi1.Text },
                        BrandName = new BrandNameType { Value = tStokS[i].Marka }, //txtMarka1.Text },
                        BuyersItemIdentification = new ItemIdentificationType { ID = new IDType { Value = tStokS[i].AliciUrunKodu } },//  txtAliciKodu1.Text } },
                        ModelName = new ModelNameType { Value = tStokS[i].Model },  //txtModel1.Text },
                        Description = new DescriptionType { Value = tStokS[i].Aciklama },  // txtAciklama1.Text },
                        ManufacturersItemIdentification = new ItemIdentificationType { ID = new IDType { Value = tStokS[i].UreticiUrunKodu } },  //txtUreticiKodu1.Text } },
                        SellersItemIdentification = new ItemIdentificationType { ID = new IDType { Value = tStokS[i].SaticiUrunKodu } }, // txtSaticiKodu1.Text } },
                    },

                    /// İskonto indirimleri / Ek ücretlerde buraya eklenecek
                    AllowanceCharge = GetAllowanceChargeTypes(tStokS[i]),
                    
                    /// Birim Fiyat
                    Price = new PriceType
                    {
                        PriceAmount = new PriceAmountType { Value = Convert.ToDecimal(tStokS[i].BirimFiyati), currencyID = "TRY" }  // txtFiyat1.Text), currencyID = "TRY" }
                    },

                    /// Miktar
                    InvoicedQuantity = new InvoicedQuantityType
                    {
                        unitCode = tStokS[i].BirimTipiId, //   "NIU",
                        Value = Convert.ToDecimal(tStokS[i].Miktar) // txtMiktar1.Text)
                    },

                    /// Satır Notu
                    //Note = new NoteType { Value = tStokS[i].SatirNotu }, //txtSatirNotu1.Text },
                                        
                    /// Kdv ve ÖTV vergisi, diğer vergiler eklenmedi gerekirse ekle
                    TaxTotal = GetTaxTotalTypes(tStokS[i]),

                    /// Satır No
                    ID = new IDType { Value = tStokS[i].SatirNo.ToString() },

                    /// Fatura satırındaki `LineExtensionAmount`, 
                    /// **satırın toplam net tutarını** (vergiler hariç) ifade eder. 
                    /// Bu tutar, satırdaki mal/hizmetin miktarı ile birim fiyatının çarpımından, 
                    /// varsa satır seviyesindeki indirimler çıkarılıp ek ücretler eklenerek hesaplanır.
                    /// 
                    /// Hesaplama Formülü	(Miktar × Birim Fiyat) - İndirimler + Ek Ücretler
                    ///
                    /// Dikkat : ISToplamTutar da henüz ek ücretler yok
                    /// 
                    LineExtensionAmount = new LineExtensionAmountType { Value = Convert.ToDecimal(tStokS[i].ISVergisizToplamTutar), currencyID = "TRY" },  //txtToplamTutar1.Text), currencyID = "TRY" },


                    /// Delivery = new DeliveryType[] { ??? }
                    /// 
                    /// Fatura satırındaki `DeliveryType` (Teslimat Bilgisi) yapısı, 
                    /// **malın teslimatına ilişkin detayları 
                    /// **(teslim tarihi, adresi, miktarı vb.) tutar.
                    /// Bu yapı, genellikle aşağıdaki durumlarda kullanılır:
                    /// 1. * *Kısmi Teslimatlar * *: Bir siparişin birden fazla sevkiyatla tamamlanması.
                    /// 2. * *Çoklu Sevkiyat Noktaları**: Farklı depo/ adreslerden yapılan gönderimler.
                    /// 3. * *Teslimat Takibi * *: Malın müşteriye ne zaman ulaştığının belgelenmesi.
                    /// ### `DeliveryType` Yapısının Bileşenleri:
                    ///| **Bileşen * *               | **Veri Tipi * *          | **Açıklama * *             
                    ///| ----------------------------| -------------------------| ---------------------------------------------------|
                    ///| `ActualDeliveryDate`      | `DateType`             | Malın fiilen teslim edildiği tarih                                          |
                    ///| `DeliveryLocation`        | `LocationType`         | Teslimat adresi(depo, şube, koordinat vb.) |
                    ///| `RequestedDeliveryPeriod` | `PeriodType`           | Talep edilen teslimat zaman aralığı(başlangıç/ bitiş tarihi)                |
                    ///| `DeliveredQuantity`       | `QuantityType`         | Bu satırda teslim edilen miktar(satır miktarından az olabilir)             |


                    /// OrderLineReference = new OrderLineReferenceType[] { new OrderLineReferenceType { OrderReference = new OrderReferenceType { ID = new IDType { Value = "a" } } } }

                }; // invoiceLines[i]
            } // for
            return invoiceLines;
        }

        /// 
        /// XSLT ve Belgeleri Alın
        /// 
        private DocumentReferenceType[] GetXsltAndDocuments()
        {
            DocumentReferenceType[] docs = new DocumentReferenceType[3];
            //if (chkXsltSet.Checked)
            //{

            docs[0] = new DocumentReferenceType
            {
                ID = new IDType { Value = new Guid().ToString() },
                IssueDate = new IssueDateType { Value = DateTime.Now },
                DocumentType = new DocumentTypeType { Value = "123456" },
                DocumentTypeCode = new DocumentTypeCodeType { Value = "MUKELLEF_KODU" },
                DocumentDescription = new DocumentDescriptionType[] { new DocumentDescriptionType { Value = "Kurum Adı" } },
            };

            docs[1] = new DocumentReferenceType
            {
                ID = new IDType { Value = new Guid().ToString() },
                IssueDate = new IssueDateType { Value = DateTime.Now },
                DocumentType = new DocumentTypeType { Value = "123456" },
                DocumentTypeCode = new DocumentTypeCodeType { Value = "MUKELLEF_ADI" },
                DocumentDescription = new DocumentDescriptionType[] { new DocumentDescriptionType { Value = "Kurum Kodu" } },
            };
            docs[2] = new DocumentReferenceType
            {
                ID = new IDType { Value = new Guid().ToString() },
                IssueDate = new IssueDateType { Value = DateTime.Now },
                DocumentType = new DocumentTypeType { Value = "123456" },
                DocumentTypeCode = new DocumentTypeCodeType { Value = "DOSYA_NO" },
                DocumentDescription = new DocumentDescriptionType[] { new DocumentDescriptionType { Value = "DOSYA NO" } },
            };


            docs[0] = new DocumentReferenceType
            {
                ID = new IDType { Value = new Guid().ToString() },
                IssueDate = new IssueDateType { Value = DateTime.Now },
                DocumentType = new DocumentTypeType { Value = "xslt" },
                Attachment = new AttachmentType
                {
                    EmbeddedDocumentBinaryObject = new EmbeddedDocumentBinaryObjectType
                    {
                        filename = "customxslt.xslt",
                        encodingCode = "Base64",
                        mimeCode = "application/xml",
                        format = "",
                        characterSetCode = "UTF-8",
                        Value = Encoding.UTF8.GetBytes(YesiLdefter.Properties.Resources.XSLTFile)  
                    }
                }
            };
            
            return docs;
            // };


            //if (chkSetInvoiceXslt.Checked)
            //{
            //    DocumentReferenceType doc = new DocumentReferenceType();
            //    //doc.ID = new IDType { Value = new Guid().ToString() };
            //   // doc.IssueDate = new IssueDateType { Value = DateTime.Now };
            //    AttachmentType atc = new AttachmentType { };
            //    EmbeddedDocumentBinaryObjectType emb = new EmbeddedDocumentBinaryObjectType();
            //    emb.filename = "customxslt.xslt";
            //    emb.encodingCode = "Base64";
            //    emb.mimeCode = "applicationxml";
            //    emb.format = "";
            //    emb.characterSetCode = "UTF-8";
            //    emb.Value = Encoding.UTF8.GetBytes(txtInvoiceXslt.Text);

            //    atc.EmbeddedDocumentBinaryObject = emb;
            //    doc.Attachment = atc;
            //    docs[0] = doc;

            //    return docs;
            //}
            //else
            //{
            //    return null;
            //}
        }
        /// 
        /// Sipariş Belgesi Referansı
        /// 
        private OrderReferenceType GetOrderReference()
        {
            OrderReferenceType orderref = new OrderReferenceType();
            orderref = new OrderReferenceType { 
                ID = new IDType { Value = "ORD1234567" }, 
                IssueDate = new IssueDateType { Value = DateTime.Now } };
            return orderref;
        }
        /// 
        /// Muhasebe Müşteri Partisi Alın 
        /// 
        private CustomerPartyType GetAccountingCustomerParty(stokB tStokB)
        {
            string AliciUnvan = "";
            string AliciSoyadi = "";

            /// 1, 'Gerçek kişi'
            /// 2, 'Tüzel kişi (Özel firma, dernek vb)'
            /// 3, 'Kamu kurumları'

            if (tStokB.CariTipiId == 1)
            {
                AliciUnvan = tStokB.CariAdi;
                AliciSoyadi = tStokB.CariSoyadi;
            }
            else
                AliciUnvan = tStokB.CariUnvan;

            CustomerPartyType customer;

            PersonType person = new PersonType { 
                FamilyName = new FamilyNameType { Value = AliciSoyadi }, // txtAliciSoyad.Text }, 
                FirstName = new FirstNameType { Value = AliciUnvan }     // txtAliciUnvan.Text } 
            };

            //if (cmbFaturaTuru.Text == "IHRACAT" || cmbFaturaTuru.Text == "YOLCUBERABERFATURA")
            if (tStokB.Lkp_FProfiliTipi == "IHRACAT" || tStokB.Lkp_FProfiliTipi == "YOLCUBERABERFATURA")
            {
                #region Gümrük Ticaret Bakanlığı Bilgileri - AccountingCustomerParty
                customer = new CustomerPartyType
                {
                    Party = new PartyType
                    {
                        PartyName = new PartyNameType { Name = new NameType1 { Value = "GÜMRÜK VE TİCARET BAKANLIĞI BİLGİ İŞLEM DAİRESİ BAŞKANLIĞI" } },
                        PartyIdentification = new PartyIdentificationType[1] { new PartyIdentificationType() { ID = new IDType { Value = "1460415308", schemeID = "VKN" } } },
                        PostalAddress = new AddressType
                        {
                            CityName = new CityNameType { Value = "Ankara" },
                            StreetName = new StreetNameType { Value = ">Üniversiteler Mahallesi Dumlupınar Bulvar" },
                            Country = new CountryType { Name = new NameType1 { Value = "Türkiye" } },

                            BuildingNumber = new BuildingNumberType { Value = "151" },
                            CitySubdivisionName = new CitySubdivisionNameType { Value = "Çankaya" }
                        },
                        PartyTaxScheme = new PartyTaxSchemeType { TaxScheme = new TaxSchemeType { Name = new NameType1 { Value = "Ulus" } } },
                    }
                };
                #endregion

                return customer;
            }
            else
            {
                #region Alıcı Bilgileri - AccountingCustomerParty
                customer = new CustomerPartyType
                {
                    Party = new PartyType
                    {
                        // VKN  : Vergi Kimlik No : 10 karekterden oluşur
                        // TCKN : TC Kimlik No    : 11 karekterden oluşur
                        PartyName = new PartyNameType { Name = new NameType1 { Value = tStokB.CariUnvan } }, // txtAliciUnvan.Text } },
                        PartyIdentification = new PartyIdentificationType[1] { 
                            new PartyIdentificationType() { 
                                ID = new IDType { Value = tStokB.CariTcVkNo, schemeID = tStokB.CariTcVkNo.Length == 10 ? "VKN" : "TCKN" } } },
                              //ID = new IDType { Value = txtAliciVkn.Text, schemeID = txtAliciVkn.Text.Length == 10 ? "VKN" : "TCKN" } } },

                        PostalAddress = new AddressType
                        {
                            CityName = new CityNameType { Value = tStokB.CariIl }, // txtAliciIl.Text },
                            StreetName = new StreetNameType { Value = tStokB.CariCaddeSokak }, //  txtAliciCaddeSokak.Text },
                            Country = new CountryType { Name = new NameType1 { Value = tStokB.CariUlkeAdi } },//  txtAliciUlke.Text } },
                            Room = new RoomType { Value = tStokB.CariIcKapiNo }, //  txtAliciKapiNo.Text },
                            BuildingNumber = new BuildingNumberType { Value = tStokB.CariDisKapiNo }, // txtAliciKapiNo.Text },
                            CitySubdivisionName = new CitySubdivisionNameType { Value = tStokB.CariIlce } //  txtGoncericiIlce.Text }   <<<< bence hatalı yazmışlar
                        },
                        Contact = new ContactType 
                        { 
                            Telefax = new TelefaxType { Value = "" }, //"22111222" }, 
                            ElectronicMail = new ElectronicMailType { Value = tStokB.CariEMail },  //"test@test.com" }, 
                            Telephone = new TelephoneType { Value = tStokB.CariTelefonu } //"0212200022" } 
                        },
                        WebsiteURI = new WebsiteURIType { Value = tStokB.Lkp_InternetSitesi },  //"Web Sitesi" },
                        PartyTaxScheme = new PartyTaxSchemeType 
                        { 
                            TaxScheme = new TaxSchemeType { 
                                Name = new NameType1 { Value = tStokB.CariVergiDairesiAdi }  //txtAliciVergiDairesi.Text } 
                            } 
                        },
                        Person = tStokB.CariTcVkNo.Length == 11 ? person : null
                        //Person = txtAliciVkn.Text.Length == 11 ? person : null
                    }
                };

                #endregion

                return customer;
            }
        }
        ///
        /// Alici Müşteri Partisini Alın ( IHRACAT, YOLCUBERABERFATURA )
        ///
        private CustomerPartyType GetBuyerCustomerParty(stokB tStokB)
        {
            CustomerPartyType customer;
            #region İhracatçı Bilgileri - BuyerCustomerParty
            //if (cmbFaturaTuru.Text == "IHRACAT")
            if (tStokB.Lkp_FProfiliTipi == "IHRACAT")
            {
                customer = new CustomerPartyType
                {
                    Party = new PartyType
                    {
                        PartyName = new PartyNameType { Name = new NameType1 { Value = tStokB.CariUnvan } },// txtAliciUnvan.Text } },
                        PartyIdentification = new PartyIdentificationType[1] { new PartyIdentificationType() { ID = new IDType { Value = "EXPORT", schemeID = "PARTYTYPE" } } },
                        PostalAddress = new AddressType
                        {
                            CityName = new CityNameType { Value = tStokB.CariIl },// txtAliciIl.Text },
                            StreetName = new StreetNameType { Value = tStokB.CariIcKapiNo },// txtAliciCaddeSokak.Text },
                            Country = new CountryType { Name = new NameType1 { Value = tStokB.CariUlkeAdi } },// txtAliciUlke.Text } },
                            Room = new RoomType { Value = tStokB.CariIcKapiNo },// txtAliciKapiNo.Text },
                            BuildingNumber = new BuildingNumberType { Value = tStokB.CariDisKapiNo },// txtAliciKapiNo.Text },
                            CitySubdivisionName = new CitySubdivisionNameType { Value = tStokB.CariIlce }// txtGoncericiIlce.Text }
                        },
                        PartyLegalEntity = new PartyLegalEntityType[] {
                            new PartyLegalEntityType { RegistrationName =
                            new RegistrationNameType {
                                Value = tStokB.CariUnvan },// txtAliciUnvan.Text }, 
                                CompanyID = new CompanyIDType { Value = tStokB.CariTcVkNo } } }, //txtAliciVkn.Text } } },

                        //Contact = new ContactType { Telefax = new TelefaxType { Value = "22111222" }, ElectronicMail = new ElectronicMailType { Value = "test@xyz.com" }, Telephone = new TelephoneType { Value = "0212200022" } },
                        //WebsiteURI = new WebsiteURIType { Value = "Web Sitesi" },

                        //PartyTaxScheme = new PartyTaxSchemeType { TaxScheme = new TaxSchemeType { Name = new NameType1 { Value = txtAliciVergiDairesi.Text } } },
                        //Person = new PersonType { FirstName = new FirstNameType { Value = "Ahmet" }, FamilyName = new FamilyNameType { Value = "Altınordu" } },
                    }
                };
                return customer;
            }
            #endregion
            #region Turist Bilgileri - BuyerCustomerParty
            //if (cmbFaturaTuru.Text == "YOLCUBERABERFATURA")
            if (tStokB.Lkp_FProfiliTipi == "YOLCUBERABERFATURA")
            {
                customer = new CustomerPartyType
                {
                    Party = new PartyType
                    {
                        Person = new PersonType
                        {
                            FirstName = new FirstNameType { Value = tStokB.CariAdi },//  "JOHN" },
                            FamilyName = new FamilyNameType { Value = tStokB.CariSoyadi },//  "DOE" },
                            NationalityID = new NationalityIDType { Value = "TR" },
                            IdentityDocumentReference = new DocumentReferenceType { 
                                ID = new IDType { Value = "PSPTNO1234567 ?????" }, 
                                IssueDate = new IssueDateType { Value = new DateTime(2005, 1, 2) } }
                        },
                    
                        PartyIdentification = new PartyIdentificationType[1] { new PartyIdentificationType() { ID = new IDType { Value = "TAXFREE", schemeID = "PARTYTYPE" } } },
                        PostalAddress = new AddressType
                        {
                            CityName = new CityNameType { Value = tStokB.CariIl },//  txtAliciIl.Text },
                            StreetName = new StreetNameType { Value = tStokB.CariCaddeSokak },//  txtAliciCaddeSokak.Text },
                            Country = new CountryType { Name = new NameType1 { Value = tStokB.CariUlkeAdi } },// txtAliciUlke.Text } },
                            Room = new RoomType { Value = tStokB.CariIcKapiNo },//  txtAliciKapiNo.Text },
                            BuildingNumber = new BuildingNumberType { Value = tStokB.CariDisKapiNo },// txtAliciKapiNo.Text },
                            CitySubdivisionName = new CitySubdivisionNameType { Value = tStokB.CariIlce }// txtGoncericiIlce.Text }
                        },

                        PartyLegalEntity = new PartyLegalEntityType[] {
                            new PartyLegalEntityType {
                                RegistrationName = new RegistrationNameType { Value = tStokB.CariUnvan },// txtAliciUnvan.Text }, 
                                CompanyID = new CompanyIDType { Value = tStokB.CariTcVkNo } } },// txtAliciVkn.Text } } },

                        //Contact = new ContactType { Telefax = new TelefaxType { Value = "22111222" }, ElectronicMail = new ElectronicMailType { Value = "test@crssoft.com" }, Telephone = new TelephoneType { Value = "0212200022" } },
                        //WebsiteURI = new WebsiteURIType { Value = "Web Sitesi" },

                        //PartyTaxScheme = new PartyTaxSchemeType { TaxScheme = new TaxSchemeType { Name = new NameType1 { Value = txtAliciVergiDairesi.Text } } },
                        //Person = new PersonType { FirstName = new FirstNameType { Value = "Ahmet" }, FamilyName = new FamilyNameType { Value = "Altınordu" } },
                    }
                };
                return customer;
            }
            #endregion
            else
            {
                return null;
            }

        }
        ///
        /// Galiba Taxi ler için
        ///
        private PartyType GetTaxRepresantiveParty(stokB tStokB)
        {
            PartyType customer;

            //if (cmbFaturaTuru.Text == "YOLCUBERABERFATURA")
            if (tStokB.Lkp_FProfiliTipi == "YOLCUBERABERFATURA")
            {
                #region Tax Free Aracı kurum Bilgileri - TaxRepresantiveParty
                customer = new PartyType
                {
                    PartyName = new PartyNameType { Name = new NameType1 { Value = "Tax Free Aracı kurum A.Ş." } },
                    PartyIdentification = new PartyIdentificationType[2] { 
                        new PartyIdentificationType() { 
                            ID = new IDType { Value = "1234567891", schemeID = "ARACIKURUMVKN" } }, 
                        new PartyIdentificationType() { 
                            ID = new IDType { Value = "urn:mail:yolcuberaberpk@aracikurum.com", schemeID = "ARACIKURUMETIKET" } } },
                    PostalAddress = new AddressType
                    {
                        CityName = new CityNameType { Value = "İstanbul" },
                        StreetName = new StreetNameType { Value = "Levent Mah. No:1 " },
                        Country = new CountryType { Name = new NameType1 { Value = "Türkiye" } },
                        CitySubdivisionName = new CitySubdivisionNameType { Value = "Şişli" }
                    },
                };
                #endregion
                return customer;
            }
            else
            {
                return null;
            }

        }
        /// 
        /// E-Arşiv Faturası ise
        /// 
        private EArchiveInvoiceInformation GetEArchiveInvoice(stokB tStokB)
        {
            // Id  CariTipi
            // 1   Gerçek kişi
            // 2   Tüzel kişi(Özel firma, dernek vb)
            // 3   Kamu kurumları
            EArchiveInvoiceInformation earchiveinfo;

            if (tStokB.CariTipiId == 1)
            {
                earchiveinfo = new EArchiveInvoiceInformation
                {
                    //DeliveryType = rbtnEArchiveElectronic.Checked ? InvoiceDeliveryType.Electronic : InvoiceDeliveryType.Paper, //kağıt ortamda olduğunda Paper değeri set edilmelidir.
                    DeliveryType = tStokB.GonderimTipiId == 1 ? InvoiceDeliveryType.Electronic : InvoiceDeliveryType.Paper,

                    //Eğer ilgili fatura bir internet satışına ait ise InternetSalesInfo nesnesinde gerekli değerler dolu olmalıdır. 
                    InternetSalesInfo = GetInternetSalesInformation(tStokB)
                };
                return earchiveinfo;
            }
            else
            {
                return null;
            }
        }
        /// 
        /// Internet üzerinden satış yapıldıysa
        /// 
        private InternetSalesInformation GetInternetSalesInformation(stokB tStokB)
        {
            InternetSalesInformation internetSalesInfo;

            if (tStokB.InternetSiteId > 0)
            {
                // Id OdemeTipi
                // 1   KREDIKARTI/BANKAKARTI
                // 2   EFT/HAVALE
                // 3   KAPIDAODEME
                // 4   ODEMEARACISI
                // 5   DIGER -

                internetSalesInfo = new InternetSalesInformation
                {
                    PaymentDate = DateTime.Now, //Ödeme Tarihi
                    //PaymentMidierName = txtEArchivePaymentMidierName.Text == "" ? null : txtEArchivePaymentMidierName.Text, //Ödeme Şekli
                    PaymentMidierName = tStokB.OdemeAraciTipiId == 0 ? null : tStokB.Lkp_OdemeAraciTipi, // BKM EXPRESS gibi
                    //PaymentType = cmbEArchivePaymentType.Text == "" ? null : cmbEArchivePaymentType.Text == "DIGER - " ? cmbEArchivePaymentType.Text + txtEArchivePaymentDesc.Text : cmbEArchivePaymentType.Text, //Ödeme Şekli 
                    PaymentType = tStokB.OdemeTipiId == 0 ? null : tStokB.OdemeTipiId == 5 ? "DIGER - " + tStokB.OdemeTipiAciklama : tStokB.Lkp_OdemeTipi,
                    //Gönderi Bilgileri
                    ShipmentInfo = new ShipmentInformation
                    {
                        //Taşıyıcı Firma Bilgileri
                        Carier = new ShipmentCarier
                        {
                            //SenderName = txtEArchiveSenderTitle.Text == "" ? null : txtEArchiveSenderTitle.Text, //Taşıyıcı(Kargo) Şirketi Adı
                            SenderName = tStokB.KargoCariId == 0 ? null : tStokB.Lkp_KargoAdi,
                            //SenderTcknVkn = txtEArchiveSenderVKN.Text == "" ? null : txtEArchiveSenderVKN.Text, //Taşıyıcı(Kargo) Şirketi VKN
                            SenderTcknVkn = tStokB.KargoCariId == 0 ? null : tStokB.Lkp_KargoVKN,
                        },
                        //SendDate = DateTime.Now,//dtpEArchiveSendDate.Value == new DateTime(2500, 1, 1) ? DateTime.MinValue : dtpEArchiveSendDate.Value, //Gönderim-Kargo Tarihi
                        SendDate = tStokB.GonderimTarihi,
                    },
                    //WebAddress = txtEArchiveWebAddress.Text == "" ? null : txtEArchiveWebAddress.Text, //Satışın yapıldığı internet sitesi adresi 
                    WebAddress = tStokB.InternetSiteId == 0 ? null : tStokB.Lkp_InternetSitesi,
                };
                return internetSalesInfo;
            }
            else
            {
                return null;
            }
        }
        /// 
        /// Satırladan İskontoları al 
        /// 
        private AllowanceChargeType[] GetAllowanceChargeTypes(stokS tStokS)
        {

            /* -- ek ücret
                 AllowanceCharge = new AllowanceChargeType{ 

                     ChargeIndicator = new ChargeIndicatorType{  Value=true},
                     MultiplierFactorNumeric = new MultiplierFactorNumericType{ Value = Math.Round(Convert.ToDecimal(txtIskontoOrani1.Text),2)/100},
                     Amount= new AmountType2 { Value = Math.Round(Convert.ToDecimal(txtIskontoOrani1.Text),2), currencyID= "TRY"},
                     },
            -- indirim/iskonto
                 AllowanceCharge = new AllowanceChargeType[] { 
                     new AllowanceChargeType { 
                         Amount = new AmountType2 { Value = 100, currencyID = "TRY" }, 
                         ChargeIndicator = new ChargeIndicatorType { Value = false }, 
                         PerUnitAmount = new PerUnitAmountType { currencyID = "TRY", Value = 100 } } },
          */

            AllowanceChargeType allowanceChargeType1 = GetAllowanceChargeTypes(tStokS.IskontoOrani1, tStokS.IskontoTutari1, tStokS.IskontoAciklamasi1, tStokS.ToplamTutar);
            AllowanceChargeType allowanceChargeType2 = GetAllowanceChargeTypes(tStokS.IskontoOrani2, tStokS.IskontoTutari2, tStokS.IskontoAciklamasi2, tStokS.ToplamTutar - tStokS.IskontoTutari1);
            AllowanceChargeType allowanceChargeType3 = GetAllowanceChargeTypes(tStokS.IskontoOrani3, tStokS.IskontoTutari3, tStokS.IskontoAciklamasi3, tStokS.ToplamTutar - tStokS.IskontoTutari1 - tStokS.IskontoTutari2);
            AllowanceChargeType allowanceChargeType4 = GetAllowanceChargeTypes(tStokS.IskontoOrani4, tStokS.IskontoTutari4, tStokS.IskontoAciklamasi4, tStokS.ToplamTutar - tStokS.IskontoTutari1 - tStokS.IskontoTutari2 - tStokS.IskontoTutari3);

            AllowanceChargeType[] allowanceCharges = new AllowanceChargeType[]
            {
                allowanceChargeType1,
                allowanceChargeType2,
                allowanceChargeType3,
                allowanceChargeType4
            };
            return allowanceCharges;
        }
        /// 
        /// Satırlardan Iskonto al devamı 
        /// 
        private AllowanceChargeType GetAllowanceChargeTypes(decimal IskontoOrani, decimal IskontoTutari, string IskontoAciklamasi, decimal bazTutar)
        {
            AllowanceChargeType allowanceChargeType;

            if (IskontoOrani > 0 || IskontoTutari > 0)
            {
                allowanceChargeType = new AllowanceChargeType
                {
                    // İskonto işareti
                    ChargeIndicator = new ChargeIndicatorType { Value = false }, // false ise iskonto anlamına geliyor, true ise ek ücret anlamına geliyor
                    // İskonto tutari
                    Amount = new AmountType2 { Value = Math.Round(Convert.ToDecimal(IskontoTutari), 2), currencyID = "TRY" },
                    // İskonto oranı
                    MultiplierFactorNumeric = new MultiplierFactorNumericType { Value = Math.Round(Convert.ToDecimal(IskontoOrani), 2) / 100 },
                    // İskonto Açıklaması
                    AllowanceChargeReason = new AllowanceChargeReasonType { Value = IskontoAciklamasi },
                    // Yüzdesel indirimlerde indirimin uygulandığı baz tutar(Para birimli)
                    BaseAmount = new BaseAmountType { Value = bazTutar, currencyID = "TRY" },
                };
                return allowanceChargeType;
            }
            else
            {
                return null;
            }

            // Parametre Açıklamaları:
            //-----------------------------------------------------------------------------------------------------------------
            // Parametre                Değer Tipi         Açıklama
            //-----------------------------------------------------------------------------------------------------------------
            // ChargeIndicator          bool               true = Ek ücret(charge), false = İndirim(allowance)
            // AllowanceChargeReason    string             İndirim / ek ücret açıklaması(Ör: "Toplu alım indirimi")
            // MultiplierFactorNumeric  decimal?           Yüzdesel indirimlerde kullanılır(Ör: % 10 için 0.10)
            // Amount                   AmountType         İndirim / ek ücret tutarı(Para birimi zorunlu)
            // BaseAmount               BaseAmountType     Yüzdesel indirimlerde indirimin uygulandığı baz tutar(Para birimli)
            //-----------------------------------------------------------------------------------------------------------------
        }
        /// 
        /// Satırlardan ÖTV ve KDV bilgisini al
        /// 
        private TaxTotalType GetTaxTotalTypes(stokS tStokS)
        {
            // Fatura Satırındaki TaxTotalType Açıklaması ve Örnek
            // TaxTotalType, fatura satırına uygulanan vergilerin toplamını ve detaylarını tutan yapıdır.
            // Her bir vergi türü(KDV, ÖTV vb.) için ayrı hesaplamalar içerir.

            /*
            TaxTotalType TaxTotal = new TaxTotalType
            {
                TaxSubtotal = new TaxSubtotalType[]{
                    new TaxSubtotalType{
                        Percent = new PercentType1 {Value=Convert.ToDecimal(txtKdvOrani1.Text)},

                        TaxCategory = new TaxCategoryType{
                          TaxScheme = new TaxSchemeType{
                              TaxTypeCode = new TaxTypeCodeType{Value = "0015"},
                              Name =new NameType1{Value="KDV"}},
                          TaxExemptionReason=new TaxExemptionReasonType{ Value="Promosyon Ürün"}},

                        TaxAmount = new TaxAmountType{ Value = ((100+Convert.ToDecimal(txtKdvOrani1.Text))/100) * Convert.ToDecimal(txtFiyat1.Text) * (Convert.ToDecimal(txtMiktar1.Text)), currencyID= "TRY" },

                        PerUnitAmount = new PerUnitAmountType{Value = ((100+Convert.ToDecimal(txtKdvOrani1.Text))/100)* Convert.ToDecimal(txtFiyat1.Text), currencyID= "TRY"},
                }},
                TaxAmount = new TaxAmountType { Value = Convert.ToDecimal(txtKdvTutar2.Text) },
            };
            */

            TaxTotalType taxTotal;
            TaxSubtotalType taxSubtotalType_OTV;
            TaxSubtotalType taxSubtotalType_KDV;

            if (tStokS.OTVKodu != "")
            {
                taxSubtotalType_OTV = new TaxSubtotalType
                {
                    TaxableAmount = new TaxableAmountType { Value = tStokS.ISVergisizToplamTutar, currencyID = "TRY" },
                    TaxAmount = new TaxAmountType { Value = tStokS.OvtTutari, currencyID = "TRY" },
                    Percent = new PercentType1 { Value = Math.Round(tStokS.OTVOrani, 2) },
                    PerUnitAmount = new PerUnitAmountType { Value = tStokS.OTVBirimTutari, currencyID = "TRY" },
                    TaxCategory = new TaxCategoryType
                    {
                        TaxScheme = new TaxSchemeType
                        {
                            Name = new NameType1 { Value = "ÖTV" },
                            TaxTypeCode = new TaxTypeCodeType { Value = tStokS.OTVKodu }
                        },
                    }
                };
            }
            else
            {
                taxSubtotalType_OTV = null;
            }

            taxSubtotalType_KDV = new TaxSubtotalType
            {
                TaxableAmount = new TaxableAmountType { Value = tStokS.ISKdvMatrahi, currencyID = "TRY" },
                TaxAmount = new TaxAmountType { Value = tStokS.ISKdvTutari, currencyID = "TRY" },
                Percent = new PercentType1 { Value = Math.Round(tStokS.KdvOrani, 2) },
                TaxCategory = new TaxCategoryType
                {
                    TaxScheme = new TaxSchemeType
                    {
                        Name = new NameType1 { Value = "KDV" },
                        TaxTypeCode = new TaxTypeCodeType { Value = "0015" }
                    },
                }
            };

            if (tStokS.OTVKodu != "")
            {
                /// Ötv + Kdv varsa
                taxTotal = new TaxTotalType
                {
                    // TOPLAM VERGİ = KDV (180 TL) + ÖTV (200 TL) = 380 TL
                    TaxAmount = new TaxAmountType { Value = tStokS.ISOtvTutari + tStokS.ISKdvTutari },
                    TaxSubtotal = new TaxSubtotalType[]{
                        taxSubtotalType_OTV,
                        taxSubtotalType_KDV,
                    }
                };
            }
            else
            {
                /// Sadece kdv varsa
                taxTotal = new TaxTotalType
                {
                    // TOPLAM VERGİ = KDV (180 TL)
                    TaxAmount = new TaxAmountType { Value = tStokS.ISKdvTutari },
                    TaxSubtotal = new TaxSubtotalType[]{
                        taxSubtotalType_KDV,
                    }
                };
            }

            return taxTotal;

            //TaxSubtotal = new TaxSubtotalType[]

            // --------------------------------------------------------------------------------------------------------------
            // Yapının Temel Bileşenleri:
            // --------------------------------------------------------------------------------------------------------------
            // Bileşen          Veri Tipi             Açıklama
            // --------------------------------------------------------------------------------------------------------------
            // TaxAmount        AmountType            Satır için toplam vergi tutarı(KDV + ÖTV + diğerleri)
            // TaxSubtotal      TaxSubtotalType[]     Her bir vergi kaleminin detaylarını içeren dizi(KDV, ÖTV ayrı ayrı)
            // --------------------------------------------------------------------------------------------------------------
            // TaxSubtotalType  İçindeki Kritik Alanlar:
            // --------------------------------------------------------------------------------------------------------------
            // Alan             Açıklama
            // --------------------------------------------------------------------------------------------------------------
            // TaxableAmount    Verginin hesaplandığı matrah(KDV öncesi net tutar)
            // TaxAmount        İlgili vergi türüne ait tutar(Ör: KDV tutarı)
            // Percent          Vergi oranı(Ör: % 18 KDV için 18.00)
            // TaxCategory      Vergi türünü tanımlayan kategorisi(KDV, ÖTV, tevkifat vb.)
            // TaxScheme        Vergi şeması bilgileri(Vergi kodu ve adı)
            // --------------------------------------------------------------------------------------------------------------
        }
        /// 
        /// Fatura başlığındaki KDV ve ÖTV
        /// 
        private TaxTotalType[] GetHeaderTaxTotalTypes(stokB tStokB)
        {
            TaxTotalType taxTotal;
            TaxSubtotalType taxSubtotalType_Kdv1;
            TaxSubtotalType taxSubtotalType_Kdv2;
            TaxSubtotalType taxSubtotalType_Kdv3;
            TaxSubtotalType taxSubtotalType_Kdv0;
            int count = 0;

            if (tStokB.FsKdv1Tutari > 0)
            {
                taxSubtotalType_Kdv1 = new TaxSubtotalType
                {
                    TaxableAmount = new TaxableAmountType { Value = tStokB.FsKdv1Matrahi, currencyID = "TRY" },
                    TaxAmount = new TaxAmountType { Value = tStokB.FsKdv1Tutari, currencyID = "TRY" },
                    Percent = new PercentType1 { Value = Math.Round(tStokB.FsKdv1Orani, 2) },
                    TaxCategory = new TaxCategoryType
                    {
                        TaxScheme = new TaxSchemeType
                        {
                            Name = new NameType1 { Value = "KDV" },
                            TaxTypeCode = new TaxTypeCodeType { Value = "0015" }
                        },
                    }
                };
                count++;
            }
            else
            {
                taxSubtotalType_Kdv1 = null;
            }

            if (tStokB.FsKdv2Tutari > 0)
            {
                taxSubtotalType_Kdv2 = new TaxSubtotalType
                {
                    TaxableAmount = new TaxableAmountType { Value = tStokB.FsKdv2Matrahi, currencyID = "TRY" },
                    TaxAmount = new TaxAmountType { Value = tStokB.FsKdv2Tutari, currencyID = "TRY" },
                    Percent = new PercentType1 { Value = Math.Round(tStokB.FsKdv2Orani, 2) },
                    TaxCategory = new TaxCategoryType
                    {
                        TaxScheme = new TaxSchemeType
                        {
                            Name = new NameType1 { Value = "KDV" },
                            TaxTypeCode = new TaxTypeCodeType { Value = "0015" }
                        },
                    }
                };
                count++;
            }
            else
            {
                taxSubtotalType_Kdv2 = null;
            }

            if (tStokB.FsKdv3Tutari > 0)
            {
                taxSubtotalType_Kdv3 = new TaxSubtotalType
                {
                    TaxableAmount = new TaxableAmountType { Value = tStokB.FsKdv3Matrahi, currencyID = "TRY" },
                    TaxAmount = new TaxAmountType { Value = tStokB.FsKdv3Tutari, currencyID = "TRY" },
                    Percent = new PercentType1 { Value = Math.Round(tStokB.FsKdv3Orani, 2) },
                    TaxCategory = new TaxCategoryType
                    {
                        TaxScheme = new TaxSchemeType
                        {
                            Name = new NameType1 { Value = "KDV" },
                            TaxTypeCode = new TaxTypeCodeType { Value = "0015" }
                        },
                    }
                };
                count++;
            }
            else
            {
                taxSubtotalType_Kdv3 = null;
            }
/*  unutma 
            /// Muafiyet veya Istisna olunca (İhracat)
            if (tStokB.FsKdv0Matrahi > 0)
            {
                taxSubtotalType_Kdv0 = new TaxSubtotalType
                {
                    TaxableAmount = new TaxableAmountType { Value = tStokB.FsKdv0Matrahi, currencyID = "TRY" },
                    TaxAmount = new TaxAmountType { Value = Math.Round(Convert.ToDecimal(0), 2), currencyID = "TRY" },
                    Percent = new PercentType1 { Value = Math.Round(tStokB.Kdv0Orani, 2) },
                    TaxCategory = new TaxCategoryType
                    {
                        TaxScheme = new TaxSchemeType
                        {
                            Name = new NameType1 { Value = "KDV" },
                            TaxTypeCode = new TaxTypeCodeType { Value = "0015" }
                        },
                        TaxExemptionReason = new TaxExemptionReasonType { Value = tStokB.KdvMuafiyetNedeni }, //  "11/1-a Mal ihracatı" },
                        TaxExemptionReasonCode = new TaxExemptionReasonCodeType { Value = tStokB.KdvMuafiyetNedeniKodu }  //  "301" }
                    }
                };
                count++;
            }
            else
            {
                taxSubtotalType_Kdv0 = null;
            }

*/
            /// Genel toplamı hazırlıyoruz
//**unutma            decimal muafKdvTutari = tStokB.FsKdv0Tutari;
            decimal toplamKdvTutari = tStokB.FsKdv1Tutari + tStokB.FsKdv2Tutari + tStokB.FsKdv3Tutari;
            decimal tahsilEdilecekKdvTutari = toplamKdvTutari;///****** UNUTMA - muafKdvTutari;

            if (tahsilEdilecekKdvTutari < 0) tahsilEdilecekKdvTutari = 0;

            int addCount = 1;
            TaxSubtotalType[] taxSubtotal = new TaxSubtotalType[count];

            if (taxSubtotalType_Kdv1 != null)
            {
                taxSubtotal[addCount] = taxSubtotalType_Kdv1;
                addCount++;
            }
            if (taxSubtotalType_Kdv2 != null)
            {
                taxSubtotal[addCount] = taxSubtotalType_Kdv2;
                addCount++;
            }
            if (taxSubtotalType_Kdv3 != null)
            {
                taxSubtotal[addCount] = taxSubtotalType_Kdv3;
                addCount++;
            }
            /**unutma
            if (taxSubtotalType_Kdv0 != null)
            {
                taxSubtotal[addCount] = taxSubtotalType_Kdv0;
                addCount++;
            }
*/
            taxTotal = new TaxTotalType
            {
                TaxAmount = new TaxAmountType { Value = Math.Round(tahsilEdilecekKdvTutari, 2), currencyID = "TRY" },
                TaxSubtotal = taxSubtotal,
            };

            return new TaxTotalType[] { taxTotal };

            //TaxSubtotal = new TaxSubtotalType[]

            // --------------------------------------------------------------------------------------------------------------
            // Yapının Temel Bileşenleri:
            // --------------------------------------------------------------------------------------------------------------
            // Bileşen          Veri Tipi             Açıklama
            // --------------------------------------------------------------------------------------------------------------
            // TaxAmount        AmountType            Satır için toplam vergi tutarı(KDV + ÖTV + diğerleri)
            // TaxSubtotal      TaxSubtotalType[]     Her bir vergi kaleminin detaylarını içeren dizi(KDV, ÖTV ayrı ayrı)
            // --------------------------------------------------------------------------------------------------------------
            // TaxSubtotalType  İçindeki Kritik Alanlar:
            // --------------------------------------------------------------------------------------------------------------
            // Alan             Açıklama
            // --------------------------------------------------------------------------------------------------------------
            // TaxableAmount    Verginin hesaplandığı matrah(KDV öncesi net tutar)
            // TaxAmount        İlgili vergi türüne ait tutar(Ör: KDV tutarı)
            // Percent          Vergi oranı(Ör: % 18 KDV için 18.00)
            // TaxCategory      Vergi türünü tanımlayan kategorisi(KDV, ÖTV, tevkifat vb.)
            // TaxScheme        Vergi şeması bilgileri(Vergi kodu ve adı)
            // --------------------------------------------------------------------------------------------------------------
        }

        /*
 e-Fatura'da WithholdingTaxTotal Nedir?
 WithholdingTaxTotal (Türkçe: Stopaj Vergisi Toplamı)
 e-fatura XML yapısında, faturayı kesen satıcı adına alıcı tarafından stopaj (tevkifat) yoluyla kesilen vergilerin toplam tutarını gösteren zorunlu bir alandır. 
 --------------------------------------------------------------------------------------
 Stopaj (Tevkifat) Nedir? (alıcının satıcı adına vergi kesmesi) 
 --------------------------------------------------------------------------------------
 Alıcı, satıcıya ödeme yaparken kanunen belirlenmiş oranlarda vergi kesintisi yapar.
 Kesilen bu vergi, satıcı adına alıcı tarafından vergi dairesine ödenir.
 En çok karıştırılan ayrıntıya dikkat : Bu alanın vergi matrahı değil, kesilen vergi tutarıdır. 
 Örnek durumlar: Hizmet alımları, serbest meslek ödemeleri, yurt dışı ödemeleri.

         * 
         * 
Zorunlu Alan: Stopaj uygulanan işlemlerde mutlaka doldurulmalıdır.

Vergi Türü Kodları:
------------------------------
0060: Gelir Vergisi Stopajı
0070: Kurumlar Vergisi Stopajı
1040: SGK Teşvik Kesintisi

Matrah: Stopaj, genellikle KDV hariç tutar üzerinden hesaplanır.
e-Fatura Geçerliliği: Eksik veya hatalı stopaj bilgisi, faturanın reddine neden olabilir.

        */

        /*
            private xxx()
            {
                // Fatura başlığına tevkifat toplamı
                var withholdingTaxTotal = new TaxTotalType
                {
                    TaxAmount = new TaxAmountType { Value = 1400.00m, currencyID = "TRY" },
                    TaxSubtotal = new List<TaxSubtotalType>
            {
                new TaxSubtotalType
                {
                    TaxableAmount = new TaxableAmountType { Value = 10000.00m, currencyID = "TRY" },
                    TaxAmount = new TaxAmountType { Value = 1400.00m, currencyID = "TRY" },
                    CalculationSequenceNumeric = new CalculationSequenceNumericType { Value = 1 },
                    Percent = new PercentType { Value = 14.0m },
                    TaxCategory = new TaxCategoryType
                    {
                        TaxScheme = new TaxSchemeType
                        {
                            Name = new NameType1 { Value = "KDV Tevkifat" },
                            TaxTypeCode = new TaxTypeCodeType { Value = "0015" }
                        }
                    }
                }
            }
                };
            }
        */
        /*
        private yyyy()
        {
            // Satır bazlı tevkifat
            var invoiceLine = new InvoiceLineType
            {
                ID = new IDType { Value = "1" },
                InvoicedQuantity = new InvoicedQuantityType { Value = 1, unitCode = "NIU" },
                LineExtensionAmount = new LineExtensionAmountType { Value = 10000.00m, currencyID = "TRY" },
                TaxTotal = new List<TaxTotalType>
        {
            new TaxTotalType
            {
                TaxAmount = new TaxAmountType { Value = 1400.00m, currencyID = "TRY" },
                TaxSubtotal = new List<TaxSubtotalType>
                {
                    new TaxSubtotalType
                    {
                        TaxableAmount = new TaxableAmountType { Value = 10000.00m, currencyID = "TRY" },
                        TaxAmount = new TaxAmountType { Value = 1400.00m, currencyID = "TRY" },
                        Percent = new PercentType { Value = 14.0m },
                        TaxCategory = new TaxCategoryType
                        {
                            TaxScheme = new TaxSchemeType
                            {
                                Name = new NameType1 { Value = "KDV Tevkifat" },
                                TaxTypeCode = new TaxTypeCodeType { Value = "0015" }
                            }
                        }
                    }
                }
            }
        }
            };
        }
        */

    }
}
