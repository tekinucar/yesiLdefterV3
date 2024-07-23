using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

using Tkn_Events;
using Tkn_ToolBox;
using Tkn_Variable;
using Tkn_SQLs;
using Tkn_CookieReader;
using YesiLdefter.Entities;
using DevExpress.XtraEditors;

namespace YesiLdefter.Selenium
{
    public class MsWebPagesService
    {
        tToolBox t = new tToolBox();
        tSQLs sql = new tSQLs();

        //public bool readLoginPageControl(ref DataSet ds_LoginPageNodes, string aktifUrl, ref string loginPageUrl, ref string errorPageUrl)
        public bool readLoginPageControl(ref DataSet ds_LoginPageNodes, webForm f)
        {
            if (f.aktifUrl == "") return false;

            if (t.IsNotNull(ds_LoginPageNodes))
            {
                /// Daha önce aynı login url yüklenmişse bir daha okumaya gerek yok
                ///
                //if (ds_LoginPageNodes.Namespace == f.aktifUrl)
                return true;
            }

            bool onay = false;
            readLoginPageNodes(ref ds_LoginPageNodes);

            if (t.IsNotNull(ds_LoginPageNodes))
            {
                f.loginPageUrl = ds_LoginPageNodes.Tables[0].Rows[0]["PageUrl"].ToString();
                f.errorPageUrl = ds_LoginPageNodes.Tables[0].Rows[0]["ErrorPageUrl"].ToString();

                // eğer error page açılmışsa 
                if (f.errorPageUrl == f.aktifUrl)
                    f.aktifUrl = f.loginPageUrl;

                /// evet LoginPage 
                ds_LoginPageNodes.Namespace = f.aktifUrl;

                onay = true;
            }
            return onay;
        }

        private bool readLoginPageNodes(ref DataSet ds)
        {
            bool onay = false;

            /// ilk defa geldiğinde
            if (ds == null)
                ds = new DataSet();

            string tSql = @" 
              Select a.*, b.PageUrl, b.ErrorPageUrl 
              from MsWebNodes a, MsWebPages b
              Where a.IsActive = 1
              and b.LoginPage = 1
              and a.PageCode = b.PageCode
              and b.PageCode = '" + v.tWebLoginPageCode + @"' 
              order by a.NodeId ";

            //and(b.PageUrl = '" + Url + "' or b.ErrorPageUrl = '" + Url + "')  "

            onay = t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "LoginPageNodes", "LoginPage");

            return onay;
        }

        public List<webNodeItemsList> readNodeItems(string pageCode)
        {
            // bir nodenin içindeki itemValue ve itemText listesi (combo içerikleri)

            // her web page için ayrı ayrı okunacak bu liste
            DataSet ds = new DataSet();

            string pages = "'" + pageCode + "'," + "'DONEMLER','GRUPLAR','SUBELER'";

            string tSql = sql.Sql_WebNodeItemsList(pages);

            t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "MsWebNodeItems", "MsWebNodeItems");
                        
            List<webNodeItemsList> webNodeItemsList = t.RunQueryModels<webNodeItemsList>(ds);

            return webNodeItemsList;
        }
        public DataSet readNodeItemsOld(string pageCode)
        {
            // her web page için ayrı ayrı okunacak bu liste
            DataSet ds = new DataSet();

            string pages = "'" + pageCode + "'," + "'DONEMLER','GRUPLAR','SUBELER'";

            string tSql = sql.Sql_WebNodeItemsList(pages);

            t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "MsWebNodeItems", "MsWebNodeItems");

            return ds;
        }

        public string ImageDownload(string urlDownload, string sessionIdAndToken, string aktifUrl)
        {
            string url = urlDownload;
            string result = "";

            // value nin içeriği böyle bir şey
            //ASP.NET_SessionId=zi3lg1kml2pnn35ax1ervzzn; __RequestVerificationToken=xXxQr11ekzeKXPuXkcTB91oW8vUbGAhP0Rcqdx-S-SSQBho0b6kvkCAmN0PUX6rj4fEABihHlFv6-Ab6wRwxdKUHs1W1NbEBC-x0SuYa9bM1

            //this.sessionId = getFindValue(value, "ASP.NET_SessionId=", ";");
            //this.token = getFindValue(value, "__RequestVerificationToken=", ";");

            WebClient wClient = new WebClient();

            wClient.Headers.Add($"Accept", $"*/*");
            //wClient.Headers.Add($"Referer", $"https://mebbis.meb.gov.tr/SKT/skt02001.aspx");
            wClient.Headers.Add($"Referer", $"" + aktifUrl + "");
            wClient.Headers.Add($"Accept-Language", $"tr-TR");
            wClient.Headers.Add($"Accept-Encoding", $"gzip, deflate");
            wClient.Headers.Add($"User-Agent", $"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.2; WOW64; Trident/7.0; .NET4.0C; .NET4.0E; .NET CLR 2.0.50727; .NET CLR 3.0.30729; .NET CLR 3.5.30729; InfoPath.3)");
            wClient.Headers.Add($"Host", $"mebbis.meb.gov.tr");
            //wClient.Headers.Add($"Connection", $"Keep-Alive");
            //wClient.Headers.Add($"Cookie", $"ASP.NET_SessionId=vo5wqgkyy4rfcudwgc1agade; __RequestVerificationToken=QFRLdG8NbJR4gkjqyARZ4feWs29RtbTZnuPDkBbdn0yHRqkAKH6Mlo9aawS5FvriU34oVh7ZMpSg7oQp-6_p3xyiSHtyJdjg4dLG4ImcEs01");
            //wClient.Headers.Add($"Cookie", $""+this.sessionId + "; " + this.token+"");
            wClient.Headers.Add($"Cookie", $"" + sessionIdAndToken);
            //var response = await wClient.DownloadData(urlDownload);

            //https://mebbis.meb.gov.tr/SKT/AResimGosterSozlesme.aspx?ImageID=221936811702021011

            if (url.IndexOf("?ImageID=") > -1)
            {
                string newUrl = url.Remove(url.IndexOf("?ImageID="));
                result = v.EXE_TempPath + "\\" + newUrl.Substring(url.LastIndexOf('/') + 1);
            }
            else
                result = v.EXE_TempPath + "\\" + url.Substring(url.LastIndexOf('/') + 1);

            wClient.DownloadFile(url, result);

            //Stream data = wClient.OpenRead(new Uri(url));
            //StreamReader reader = new StreamReader(data);
            //string content = reader.ReadToEnd();

            return result;
        }

        // node hakkındaki bilgilerin wnv üzerine aktarılması
        public void nodeValuesPreparing(DataRow row, ref webNodeValue wnv, string aktifPageCode)
        {
            wnv.pageCode = aktifPageCode;
            wnv.nodeId = Convert.ToInt32(row["NodeId"].ToString());
            wnv.TagName = row["TagName"].ToString();
            wnv.AttId = row["AttId"].ToString();
            wnv.AttName = row["AttName"].ToString();
            wnv.AttClass = row["AttClass"].ToString();
            wnv.AttType = row["AttType"].ToString();
            wnv.AttRole = row["AttRole"].ToString();
            wnv.AttHRef = row["AttHRef"].ToString();
            wnv.AttSrc = row["AttSrc"].ToString();
            wnv.XPath = row["XPath"].ToString();
            wnv.InnerText = row["InnerText"].ToString();
            wnv.OuterText = row["OuterText"].ToString();
            wnv.InjectType = (v.tWebInjectType)Convert.ToInt16(row["InjectType"].ToString());
            wnv.InvokeMember = (v.tWebInvokeMember)Convert.ToInt16(row["InvokeMember"].ToString());
            wnv.DontSave = Convert.ToBoolean(row["DontSave"].ToString());
            wnv.GetSave = Convert.ToBoolean(row["GetSave"].ToString());
            wnv.writeValue = row["TestValue"].ToString();
            wnv.EventsType = (v.tWebEventsType)t.myInt16(row["EventsType"].ToString());

            if (wnv.writeValue == "BUGUN_YILAY")
                wnv.writeValue = v.BUGUN_YILAY.ToString();

            if (wnv.writeValue == "MEBBIS_KODU")
            {
                if (v.tUser.MebbisCode != "")
                    wnv.writeValue = v.tUser.MebbisCode;
                else wnv.writeValue = v.tMainFirm.MebbisCode;
            }

            if (wnv.writeValue == "MEBBIS_SIFRE")
            {
                if (v.tUser.MebbisPass != "")
                    wnv.writeValue = v.tUser.MebbisPass;
                else wnv.writeValue = v.tMainFirm.MebbisPass;
            }
        }

        public void nodeValuesPreparing(MsWebNode item, ref webNodeValue wnv)
        {
            //wnv.pageCode = aktifPageCode;
            wnv.nodeId = item.NodeId;// Convert.ToInt32(row["NodeId"].ToString());
            wnv.TagName = item.TagName;// row["TagName"].ToString();
            wnv.AttId = item.AttId;// row["AttId"].ToString();
            wnv.AttName = item.AttName;// row["AttName"].ToString();
            wnv.AttClass = item.AttClass;// row["AttClass"].ToString();
            wnv.AttType = item.AttType;// row["AttType"].ToString();
            wnv.AttRole = item.AttRole;// row["AttRole"].ToString();
            wnv.AttHRef = item.AttHRef;// row["AttHRef"].ToString();
            wnv.AttSrc = item.AttSrc;// row["AttSrc"].ToString();
            wnv.XPath = item.XPath;// row["XPath"].ToString();
            wnv.InnerText = item.InnerText;// row["InnerText"].ToString();
            wnv.OuterText = item.OuterText; // row["OuterText"].ToString();
            wnv.InjectType = (v.tWebInjectType)item.InjectType;   // Convert.ToInt16(row["InjectType"].ToString());
            wnv.InvokeMember = (v.tWebInvokeMember)item.InvokeMember;  //Convert.ToInt16(row["InvokeMember"].ToString());
            wnv.DontSave = item.DontSave;// Convert.ToBoolean(row["DontSave"].ToString());
            wnv.GetSave = item.GetSave;// Convert.ToBoolean(row["GetSave"].ToString());
            wnv.writeValue = item.TestValue;// row["TestValue"].ToString();
            wnv.EventsType = (v.tWebEventsType)item.EventsType;   //t.myInt16(row["EventsType"].ToString());
            wnv.KrtOperandType = item.KrtOperandType;
            wnv.CheckValue = item.CheckValue;

            if (wnv.writeValue == "BUGUN_YILAY")
                wnv.writeValue = v.BUGUN_YILAY.ToString();

            if (wnv.writeValue == "MEBBIS_KODU")
            {
                if (v.tUser.MebbisCode != "")
                    wnv.writeValue = v.tUser.MebbisCode;
                else wnv.writeValue = v.tMainFirm.MebbisCode;
            }

            if (wnv.writeValue == "MEBBIS_SIFRE")
            {
                if (v.tUser.MebbisPass != "")
                    wnv.writeValue = v.tUser.MebbisPass;
                else wnv.writeValue = v.tMainFirm.MebbisPass;
            }
        }

        public List<MsWebScrapingDbFields> readScrapingTablesAndFields(List<MsWebPage> msWebPages)
        {
            if (msWebPages.Count == 0) 
                return null;

            DataSet ds = new DataSet();

            // MsWebNodeItems tablosuna items ları atabilmek için en başa bu pageCode ekleniyor
            string pageCodes = "'SELECTNODEITEMS',";

            foreach (MsWebPage item in msWebPages)
            {
                pageCodes += "'" + item.PageCode + "',";
            }

            // son virgülü sil
            pageCodes = pageCodes.Remove(pageCodes.Length - 1, 1);
                         
            string tSql = sql.Sql_WebScrapingFieldsList(pageCodes);
                        
            /// webNode ler ile db field leri arasındaki ilişkilerin listesi bu tabloda
            /// 
            t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "ScrapingTables", "ScrapingTables");

            List<MsWebScrapingDbFields> msWebScrapingFields = t.RunQueryModels<MsWebScrapingDbFields>(ds);

            return msWebScrapingFields;
        }
        public DataSet readScrapingTablesAndFieldsOld(List<MsWebPage> msWebPages)
        {
            if (msWebPages.Count == 0)
                return null;

            DataSet ds = new DataSet();

            // MsWebNodeItems tablosuna items ları atabilmek için en başa bu pageCode ekleniyor
            string pageCodes = "'SELECTNODEITEMS',";

            foreach (MsWebPage item in msWebPages)
            {
                pageCodes += "'" + item.PageCode + "',";
            }

            // son virgülü sil
            pageCodes = pageCodes.Remove(pageCodes.Length - 1, 1);

            tSQLs sql = new tSQLs();
            string tSql = sql.Sql_WebScrapingFieldsListOld(pageCodes);

            /// webNode ler ile db field leri arasındaki ilişkilerin listesi bu tabloda
            /// 
            t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "ScrapingTables", "ScrapingTables");

            return ds;
        }

        public void transferFromWebToDatabase(webNodeValue wnv)
        {
           // hataları düzelt
            
            /*

            /// web deki veriyi database aktar
            ///
            if (t.IsNotNull(ds_ScrapingDbConnectionList) == false) return;

            wnv.dbFieldName = "";
            wnv.dbLookUpField = false;

            DataRow dbRow = findRightRow(wnv, v.tSelect.Get);

            // webden okunan veriyi db ye aktardığı an
            if (dbRow != null)
            {
                //t.tCheckedValue(dbRow, wnv.dbFieldName, wnv.readValue);

                if (wnv.dbFieldName.IndexOf("Resim") == -1)
                {
                    v.SQL = v.SQL + wnv.readValue;
                    //if (t.IsNotNull(wnv.readValue))
                    //    dbRow[wnv.dbFieldName] = wnv.readValue;

                    if (t.IsNotNull(wnv.readValue))
                    {
                        string itemText = wnv.readValue;

                        if (wnv.dbLookUpField == false)
                        {
                            if (wnv.dbFieldType == 108)
                            {
                                itemText = wnv.readValue.Replace(" TL", "");
                                itemText = wnv.readValue.Replace("TL", "");
                            }
                            if (itemText != "")
                                dbRow[wnv.dbFieldName] = itemText;
                        }
                        else
                        {
                            string itemValue = findNodeItemsValue(wnv, itemText);
                            if (itemValue != "")
                                dbRow[wnv.dbFieldName] = itemValue;
                        }

                        dbRow.AcceptChanges();
                    }
                }
                else
                {
                    v.SQL = v.SQL + wnv.dbFieldName;

                    if (t.IsNotNull(v.con_Images_FieldName) == false)
                        v.con_Images_FieldName = wnv.dbFieldName;
                    else v.con_Images_FieldName2 = wnv.dbFieldName;

                    //v.EXE_TempPath+"\\AResimGoster.aspx"
                    string fileName = wnv.readValue;

                    //byte[] theBytes = Encoding.UTF8.GetBytes(wnv.readValue);
                    long imageLength = 0;
                    if (v.con_Images == null)
                        v.con_Images = t.imageBinaryArrayConverter(fileName, ref imageLength);
                    else v.con_Images2 = t.imageBinaryArrayConverter(fileName, ref imageLength);

                    if (t.IsNotNull(v.con_Images_FieldName))
                    {
                        if (v.con_Images_FieldName.IndexOf("Small") > -1)
                            v.con_Images = ResmiKucult(v.con_Images);
                    }

                    if (t.IsNotNull(v.con_Images_FieldName2))
                    {
                        if (v.con_Images_FieldName2.IndexOf("Small") > -1)
                            v.con_Images2 = ResmiKucult(v.con_Images2);
                    }
                    //if (t.IsNotNull(wnv.readValue))
                    //    dbRow[wnv.dbFieldName] = theBytes; // Encoding.UTF8.GetBytes(wnv.readValue);
                    if ((v.con_Images != null) && (v.con_Images2 == null))
                        dbRow[wnv.dbFieldName] = v.con_Images;
                    if ((v.con_Images != null) && (v.con_Images2 != null))
                        dbRow[wnv.dbFieldName] = v.con_Images2;
                }
            }

            // get işlemi bitti ve artık database yazma işlemi gerekiyor
            if (wnv.GetSave)
            {
                if (t.IsNotNull(ds_DbaseTable))
                    msPagesService.dbButtonClick(this, ds_DbaseTable.DataSetName, v.tButtonType.btKaydet);
            }


            */
        }

        public bool transferFromDatabaseToWeb(Form tForm, webNodeValue wnv, List<MsWebScrapingDbFields> msWebScrapingDbFields)
        {
            //v.SQL = v.SQL + v.ENTER + myNokta + " transferFromDatabaseToWeb : Set : ";

            /// database deki veriyi web e aktar
            ///

            //if (t.IsNotNull(ds_ScrapingDbConnectionList) == false) return;
            bool onay = false;

            if (msWebScrapingDbFields.Count == 0) 
                return onay;

            wnv.dbFieldName = "";
            wnv.dbLookUpField = false;

            try
            {
                DataRow dbRow = findRightRow(tForm, wnv, v.tSelect.Set, msWebScrapingDbFields);

                // db den okuyarak web göndereceği veri db den aldığı an
                if (dbRow != null)
                {
                    Int16 ftype = wnv.dbFieldType;

                    if (wnv.dbFieldName.IndexOf("Resim") == -1)
                    {
                        // date veya smalldate
                        if ((ftype == 40) || (ftype == 58) || (ftype == 61))
                        {
                            string value = dbRow[wnv.dbFieldName].ToString();
                            if (value != "")
                                wnv.writeValue = Convert.ToDateTime(value).ToString("dd.MM.yyyy"); //.Substring(0,10);

                        }
                        else if (ftype == 108)
                        {
                            string value = dbRow[wnv.dbFieldName].ToString();
                            if (value != "")
                            {
                                //value = value.Substring(0, value.IndexOf(",") + 3);
                                wnv.writeValue = string.Format("{0:0.00}", Convert.ToDecimal(value));// Convert.ToString(Convert.ToDouble(value).ToString("D"));
                            }
                        }
                        else
                        {
                            wnv.writeValue = dbRow[wnv.dbFieldName].ToString();
                        }
                        onay = true;
                    }
                    else
                    {
                        bool valueOnay = false;
                        string imgName = wnv.writeValue;
                        if (imgName == "") imgName = wnv.dbFieldName;

                        try
                        {
                            var value = (byte[])dbRow[wnv.dbFieldName];
                            valueOnay = true;
                        }
                        catch (Exception)
                        {
                            valueOnay = false;
                        }

                        if (valueOnay)
                        {
                            string Images_Path = t.Find_Path("images") + imgName + ".jpg";
                            long imageLength = 0;
                            byte[] byteResim = t.imageBinaryArrayConverterMem((byte[])dbRow[wnv.dbFieldName], ref imageLength);
                            onay = t.imageBinaryArrayConverterFile(byteResim, Images_Path);
                            if (onay)
                                wnv.writeValue = Images_Path;
                            else
                                wnv.writeValue = "Error Images";
                        }
                        else
                        {
                            onay = false;
                            MessageBox.Show($"DİKKAT : {imgName} için resim kaydı bulunamadı...", imgName);
                        }
                    }
                }
            }
            catch (Exception)
            {
                onay = false;
                throw;
            }
            return onay;
        }
        public void findRightDbTables(webNodeValue wnv, List<MsWebScrapingDbFields> msWebScrapingFields)
        {
            // database tablsoundaki web deki bir tabloya veri atacağımız zaman iki si arasında bir eşleştirme yapmak gerekiyor
            // örnek : table deki bir TCNo yu alıp webdeki tabloda önce onu bulmamız gerekiyor
            // şimdi db table deki veri önce tespit alanları edelim

            string _TableIPCode = "";
            string _pageCode = wnv.pageCode;
            string _dbPageCode = "";
            Int16 _krtOperandType = 0;
            
            tTable _tTable = new tTable();

            // sadece eşleştirme yapılacak anahtar fieldi tespit ediyor
            //foreach (DataRow row in ds_ScrapingDbConnectionList.Tables[0].Rows)
            foreach (MsWebScrapingDbFields item in msWebScrapingFields)
            {
                _dbPageCode = item.WebScrapingPageCode;  // row["WebScrapingPageCode"].ToString();
                _krtOperandType = item.KrtOperandType;   // row["KRT_OPERAND_TYPE"].ToString();
                if (_pageCode == _dbPageCode)
                {
                    if (_krtOperandType == 10) // eşleşme yapılack field nerde ise aradığımız TableIPCode o dur.
                    {
                        // örnek : TCNo hangi gibi keyFieldName tespit ediliyor, karşılaştırma için kullanılacak
                        wnv.tTableColNo = item.WebScrapingSetColumnNo;  // t.myInt16(row["WebScrapingSetColumnNo"].ToString());
                        wnv.dbFieldName = item.FieldName;  // row["FIELD_NAME"].ToString();
                        wnv.TableIPCode = item.TableIPCode;
                        break;
                    }
                }
            }

            // anahtar field dışındaki atama yapılacak diğer fieldleri tespit ediyor
            //foreach (DataRow row in ds_ScrapingDbConnectionList.Tables[0].Rows)
            foreach (MsWebScrapingDbFields item in msWebScrapingFields)
            {
                _dbPageCode = item.WebScrapingPageCode;  //row["WebScrapingPageCode"].ToString();
                _krtOperandType = item.KrtOperandType; // row["KRT_OPERAND_TYPE"].ToString();
                _TableIPCode = item.TableIPCode;  // row["TableIPCode"].ToString();
                if (_pageCode == _dbPageCode)
                {
                    // wnv.TableIPCode inin içeriği _krtOperandType == "10" sağlanınca bulunuyor 
                    // harici zamanlarda boş
                    if ((_TableIPCode == wnv.TableIPCode) &&
                        (_krtOperandType != 10))
                    {
                        // database tablodan okunacak fieldler tespit ediliyor

                        tRow _tRow = new tRow();

                        // html table üzerindeki column no
                        tColumn _tColumnNo = new tColumn();
                        _tColumnNo.value = item.WebScrapingSetColumnNo.ToString(); // row["WebScrapingSetColumnNo"].ToString();
                        _tRow.tColumns.Add(_tColumnNo);

                        // database table üzerindeki okunacak fieldName
                        tColumn _tColumn = new tColumn();
                        _tColumn.value = item.FieldName; // row["FIELD_NAME"].ToString();
                        _tRow.tColumns.Add(_tColumn);

                        // o fieldin value değerini html table ye taşıma için kullanılacak şimdilik sadece boş olan column
                        tColumn _tColumnValue = new tColumn();
                        _tColumnValue.value = "";
                        _tRow.tColumns.Add(_tColumnValue);

                        _tTable.tRows.Add(_tRow);
                    }
                }
            }

            wnv.tTable = _tTable;

        }
        public void transferFromWebToDatabase(Form tForm, webNodeValue wnv, List<MsWebScrapingDbFields> msWebScrapingDbFields, List<webNodeItemsList> aktifPageNodeItemsList, webForm f)
        {
            //v.SQL = v.SQL + v.ENTER + myNokta + " transferFromWebToDatabase : Get : " + wnv.dbFieldName;

            /// web deki veriyi database aktar
            ///
            if (msWebScrapingDbFields == null) return;
            if (msWebScrapingDbFields.Count == 0) return;

            wnv.dbFieldName = "";
            wnv.dbLookUpField = false;
                        
            DataRow dbRow = findRightRow(tForm, wnv, v.tSelect.Get, msWebScrapingDbFields);

            /// tableIPCode tespit edildi ... save işlemi başlayacak olan DataSet belli oldu
            ///
            f.tableIPCodeIsSave = wnv.TableIPCode;

            // webden okunan veriyi db ye aktardığı an
            if (dbRow != null)
            {
                //t.tCheckedValue(dbRow, wnv.dbFieldName, wnv.readValue);

                if (wnv.dbFieldName.IndexOf("Resim") == -1)
                {
                    v.SQL = v.SQL + wnv.readValue;
                    //if (t.IsNotNull(wnv.readValue))
                    //    dbRow[wnv.dbFieldName] = wnv.readValue;

                    if (t.IsNotNull(wnv.readValue))
                    {
                        string itemText = wnv.readValue;

                        if (wnv.dbLookUpField == false)
                        {
                            if (wnv.dbFieldType == 108)
                            {
                                itemText = wnv.readValue.Replace(" TL", "");
                                itemText = wnv.readValue.Replace("TL", "");
                            }
                            if (itemText != "")
                                dbRow[wnv.dbFieldName] = itemText;
                        }
                        else
                        {
                            string itemValue = findNodeItemsValue(wnv, aktifPageNodeItemsList, itemText);
                            if (itemValue != "")
                                dbRow[wnv.dbFieldName] = itemValue;
                        }

                        dbRow.AcceptChanges();
                    }
                }
                else
                {
                    //v.SQL = v.SQL + wnv.dbFieldName;

                    if (t.IsNotNull(v.con_Images_FieldName) == false)
                        v.con_Images_FieldName = wnv.dbFieldName;
                    else v.con_Images_FieldName2 = wnv.dbFieldName;

                    //v.EXE_TempPath+"\\AResimGoster.aspx"
                    string fileName = wnv.readValue;

                    //byte[] theBytes = Encoding.UTF8.GetBytes(wnv.readValue);
                    long imageLength = 0;
                    if (v.con_Images == null)
                        v.con_Images = t.imageBinaryArrayConverter(fileName, ref imageLength);
                    else v.con_Images2 = t.imageBinaryArrayConverter(fileName, ref imageLength);

                    if (t.IsNotNull(v.con_Images_FieldName))
                    {
                        if (v.con_Images_FieldName.IndexOf("Small") > -1)
                            v.con_Images = ResmiKucult(v.con_Images);
                    }

                    if (t.IsNotNull(v.con_Images_FieldName2))
                    {
                        if (v.con_Images_FieldName2.IndexOf("Small") > -1)
                            v.con_Images2 = ResmiKucult(v.con_Images2);
                    }
                    //if (t.IsNotNull(wnv.readValue))
                    //    dbRow[wnv.dbFieldName] = theBytes; // Encoding.UTF8.GetBytes(wnv.readValue);
                    if ((v.con_Images != null) && (v.con_Images2 == null))
                        dbRow[wnv.dbFieldName] = v.con_Images;
                    if ((v.con_Images != null) && (v.con_Images2 != null))
                        dbRow[wnv.dbFieldName] = v.con_Images2;
                }
            }

            // get işlemi bitti ve artık database yazma işlemi gerekiyor
            if (wnv.GetSave)
            {
                if (t.IsNotNull(wnv.ds)) //  ds_DbaseTable))
                    dbButtonClick(tForm, wnv.ds.DataSetName, v.tButtonType.btKaydet);
            }

        }
        public async Task transferFromWebTableToDatabase(Form tForm, webNodeValue wnv, List<MsWebNode> msWebNodes, List<MsWebScrapingDbFields> msWebScrapingDbFields, List<webNodeItemsList> aktifPageNodeItemsList)
        {
            //v.SQL = v.SQL + v.ENTER + myNokta + " transferFromWebTableToDatabase";
            
            Application.DoEvents();
            
            //if (this.myTriggerTableCount <= this.myTriggerTableRowNo)
            //{
            //    // bitti
            //    // tetiklenecek başka row kalmadı
            //    this.myTriggering = false;
            //    this.myTriggeringTable = false;
            //    return;
            //}

            //if ((this.myTriggerPageRefresh) &&
            //    (this.myTriggerPageRefreshTick))
            //    return;

            if (wnv.tTable == null)
                return;

            t.WaitFormOpen(tForm, "Alınan bilgiler veritabanına yazılıyor ...");

            tTable tb = wnv.tTable;

            //this.myTriggerWmvTableRows = null;
            //this.myTriggerWmvTableRows = tb.tRows[this.myTriggerTableRowNo];

            int myTriggerTableRowNo = 0;
            tRow myTriggerWmvTableRows = tb.tRows[myTriggerTableRowNo];

            ///
            /// Tablo üzerinde bulunduğu satırdaki bilgilerin detayını göstermek için buton var mı ?
            ///

            /// itemButton varsa uygula
            ///

            //this.myTriggerItemButton = findTableItemButton(msWebNodes, this.myTriggerWmvTableRows, this.myTriggerTableRowNo);
            bool myTriggerItemButton = findTableItemButton(msWebNodes, myTriggerWmvTableRows, myTriggerTableRowNo);

            /// itemButton yoksa 
            /// itemButton false ise burada kayıt işlemi yapılıyor
            /// 

            //if ((this.myTriggerItemButton == false) && (this.myTriggerTableRowNo == 0))
            if ((myTriggerItemButton == false) && (myTriggerTableRowNo == 0))
            {
                // itemButton olmadığı için table nin bütün row larını bir defada buradan kayıt işlemini gerçekleştir
                //
                int value = 0;
                foreach (tRow row in tb.tRows)
                {
                    // geçici çözüm burayı sil
                    value = t.myInt32(row.tColumns[0].value.ToString());

                    // kayitBaslangisId : bazı table data listesi çok büyük olabiliyor (sertifika alanların listesi gibi)
                    // kayitBaslangisId ile hangi kayıttan başlayabileceğine karar verebiliriz
                    // çünkü bazen liste daha bitmemişken herhangi bir sebeple işlem durmuş olabiliyor
                    // önceki kayıt edilmiş olanlar tekrar kayıt işlemiyle uğraşmasın diye istenen yerden kaydın başlanması 
                    // sağlanabiliyor

                    // şimdilik sıfırı her halükarda çalışsın
                    int kayitBaslangisId = 0; // burası yeniden düzenlemeli
                    //--

                    if (value >= kayitBaslangisId)
                    {
                        //saveRowAsync(tForm, this.myTriggerTableWnv, row, msWebScrapingDbFields, aktifPageNodeItemsList);
                        saveRowAsync(tForm, wnv, row, msWebScrapingDbFields, aktifPageNodeItemsList);

                        if (editKayitKontrolu()) break;
                    }
                }
                //this.myTriggeringTable = false;
            }

            v.IsWaitOpen = false;
            t.WaitFormClose();

            /// itemButton varsa kayıt işlem aşağıdaki adreste yapılıyor
            ///

            //if (this.myTriggerItemButton)
            //{
            //    // tetikleme işi timerTriggerAsync() içinde 1.1
            //    //
            //    // transferFromWebTableRowToDatabase();
            //}

        }
        public void transferFromWebSelectToDatabase(Form tForm, webNodeValue wnv, List<MsWebScrapingDbFields> msWebScrapingDbFields, List<webNodeItemsList> aktifPageNodeItemsList, webForm f)
        {
            //v.SQL = v.SQL + v.ENTER + myNokta + " transferFromWebSelectToDatabase";

            /// Web den okunan tabloyu dB ye aktarma işlemleri
            ///

            /// tabloyu ele al
            /// 
            if (wnv.tTable == null) return;
            tTable tb = wnv.tTable;
            /// data yoksa geri dön
            if (tb.tRows.Count <= 1) return;

            int rowNo = 0;
            bool onay = false;


            string kacAdet = KullaniciyaSor(wnv.InnerText, wnv.OuterText, wnv.writeValue);
            int userCount = t.myInt32(kacAdet);
            //int.Parse(iBox.value);

            /// tableIPCode tespit edildi ... save işlemi başlayacak olan DataSet belli oldu
            ///
            f.tableIPCodeIsSave = wnv.TableIPCode;

            /// sırayla row ları ele al
            ///
            foreach (tRow rows in tb.tRows)
            {
                // her zaman true olsun
                onay = true;

                /// 0 sıfırı atlamasının nedeni caption row
                if (rowNo == 0)
                {
                    if (wnv.TagName == "table")
                        onay = false;

                    if (wnv.TagName == "select")
                        onay = true;
                }

                if (onay)
                {
                    saveRowAsync(tForm, wnv, rows, msWebScrapingDbFields, aktifPageNodeItemsList);

                    if (editKayitKontrolu()) break;
                }
                rowNo++;

                if (rowNo >= userCount)
                {
                    MessageBox.Show(userCount.ToString() + " adet kayıt alındı...");
                    break;
                }
            }
        }

        private string KullaniciyaSor(string title, string label, string defaultValue)
        {
            vUserInputBox iBox = new vUserInputBox();
            iBox.Clear();
            iBox.title = title;         // "Kaydın başlamasını istediğiniz sıra no";
            iBox.promptText = label;    // "Sıra No  :";
            iBox.value = defaultValue;  // "0";
            iBox.displayFormat = "";
            iBox.fieldType = 0;

            string cevap = "";
            if (t.UserInpuBox(iBox) == DialogResult.OK)
            {
                cevap = iBox.value;
            }
            return cevap;
        }

        private DataRow findRightRow(Form tForm, webNodeValue wnv, v.tSelect select, List<MsWebScrapingDbFields> msWebScrapingDbFields)
        {
            //v.SQL = v.SQL + v.ENTER + "findRightRow";

            string _TableIPCode = "";
            string _pageCode = wnv.pageCode;
            string _dbPageCode = "";
            int _nodeId = wnv.nodeId;
            Int16 _colNo = 0;
            int _dbNodeId = 0;
            Int16 _dbColNo = 0;
            Control cntrl = null;

            if (wnv.tTable != null)
                _colNo = wnv.tTableColNo;

            DataRow findDbRow = null;

            //foreach (DataRow row in ds_ScrapingDbConnectionList.Tables[0].Rows)
            foreach (MsWebScrapingDbFields item in msWebScrapingDbFields)
            {
                _dbPageCode = item.WebScrapingPageCode;  //row["WebScrapingPageCode"].ToString();

                if (_pageCode == _dbPageCode)
                {
                    if (select == v.tSelect.Get)
                    {
                        _dbNodeId = item.WebScrapingGetNodeId;   //t.myInt32(row["WebScrapingGetNodeId"].ToString());
                        _dbColNo = item.WebScrapingGetColumnNo;  //t.myInt16(row["WebScrapingGetColumnNo"].ToString());
                    }
                    if (select == v.tSelect.Set)
                    {
                        _dbNodeId = item.WebScrapingSetNodeId; //t.myInt32(row["WebScrapingSetNodeId"].ToString());
                        _dbColNo = item.WebScrapingSetColumnNo;//t.myInt16(row["WebScrapingSetColumnNo"].ToString());
                    }

                    if ((_dbPageCode == _pageCode) &&
                        (_dbNodeId == _nodeId) &&
                        (_dbColNo == _colNo))
                    {
                        // yazılacak veya okunacak Table
                        _TableIPCode = item.TableIPCode;  //row["TableIPCode"].ToString();

                        // böyle TableIPCode bulduk fakat form üzerinde böyle bir control var mı
                        // çünkü WebScrapingPageCode tanımı birden fazla InputPanel de kullanılabiliyor
                        cntrl = t.Find_Control_View(tForm, _TableIPCode);

                        // varsa devam
                        if (cntrl != null)
                        {
                            // yazılacak veya okunacak fieldName
                            wnv.TableIPCode = _TableIPCode; // iş bitimde DataSet Refresh için gerekli
                            wnv.dbFieldName = item.FieldName;       //row["FIELD_NAME"].ToString();
                            wnv.dbLookUpField = item.FLookUpField;  //Convert.ToBoolean(row["FLOOKUP_FIELD"].ToString());
                            wnv.dbFieldType = item.FieldType;       //t.myInt16(row["FIELD_TYPE"].ToString());

                            DataSet ds = null;
                            DataNavigator dN = null;
                            t.Find_DataSet(tForm, ref ds, ref dN, _TableIPCode);

                            if (t.IsNotNull(ds))
                            {
                                if (dN.Position == -1)
                                {
                                    dN.Position = 0;
                                    dN.Tag = 0;
                                    findDbRow = ds.Tables[0].Rows[0];
                                }
                                else
                                    findDbRow = ds.Tables[0].Rows[dN.Position];

                                // işlem yapılan DataSet ve DataNavigator
                                wnv.ds = ds;
                                wnv.dN = dN;

                                /********** bu işleme gerek kalmadı 
                                 * çünkü sıralı işlem var mı yok önceden tespit edilmeye başlandı
                                 * 

                                /// Sıralı işlem hangi TableIPCode üzerinde gerçekleşecek onu bulalım
                                ///
                                if (this.myTriggerOneByOne == false)// &&
                                                                    //(this.ds_DbaseSiraliTable == null))
                                {
                                    findOneByOneButton(_TableIPCode);
                                }

                                ********/

                                return findDbRow;
                            }
                            else
                            {
                                /// Table var fakat row yok ise
                                /// 
                                if (ds != null)
                                {
                                    if (ds.Tables != null)
                                    {
                                        if (ds.Tables[0].Rows.Count == 0)
                                        {
                                            findDbRow = ds.Tables[0].NewRow();
                                            wnv.ds = ds;
                                            wnv.dN = dN;
                                        }
                                        return findDbRow;
                                    }
                                }
                            }

                        }
                    }
                }
            }

            if (findDbRow == null)
            {
                // v.SQL = v.SQL + v.ENTER + "DİKKAT : findRightRow, uygun row bulunamadı. PageCode : " + pageCode + ", nodeId : " + nodeId.ToString();
            }

            // işlem buraya kadar geldiyse null dönüyor
            //
            return findDbRow;
        }
        private DataRow findRightRowForSelect(Form tForm, webNodeValue wnv, v.tSelect select, List<MsWebScrapingDbFields> msWebScrapingDbFields)
        {
            string pageCode = wnv.pageCode;
            int nodeId = wnv.nodeId;
            Int16 colNo = 0;
            Int16 dbColNo = 0;

            if (wnv.tTable != null)
                colNo = wnv.tTableColNo;

            DataRow findDbRow = null;

            //foreach (DataRow row in ds_ScrapingDbConnectionList.Tables[0].Rows)
            foreach (MsWebScrapingDbFields item in msWebScrapingDbFields)
            {
                if (select == v.tSelect.Get)
                {
                    //dbNode = Convert.ToInt32(row["WebScrapingGetNodeId"].ToString());
                    dbColNo = item.WebScrapingGetColumnNo; //Convert.ToInt16(row["WebScrapingGetColumnNo"].ToString());
                }
                if (select == v.tSelect.Set)
                {
                    //dbNode = Convert.ToInt32(row["WebScrapingSetNodeId"].ToString());
                    dbColNo = item.WebScrapingSetColumnNo; //Convert.ToInt16(row["WebScrapingSetColumnNo"].ToString());
                }

                //if ((row["WebScrapingPageCode"].ToString() == "SELECTNODEITEMS") &&
                if ((item.WebScrapingPageCode == "SELECTNODEITEMS") &&
                    (dbColNo == colNo))
                {
                    // yazılacak veya okunacak Table
                    string _TableIPCode = item.TableIPCode; //row["TableIPCode"].ToString();

                    // yazılacak veya okunacak fieldName
                    wnv.dbFieldName = item.FieldName; //row["FIELD_NAME"].ToString();
                    wnv.dbLookUpField = item.FLookUpField; //Convert.ToBoolean(row["FLOOKUP_FIELD"].ToString());
                    wnv.dbFieldType = item.FieldType; //t.myInt16(row["FIELD_TYPE"].ToString());

                    DataSet ds = null;
                    DataNavigator dN = null;
                    t.Find_DataSet(tForm, ref ds, ref dN, _TableIPCode);

                    if (t.IsNotNull(ds))
                    {
                        if (dN.Position == -1)
                        {
                            dN.Position = 0;
                            dN.Tag = 0;
                            findDbRow = ds.Tables[0].Rows[0];
                        }
                        else
                            findDbRow = ds.Tables[0].Rows[dN.Position];

                        // işlem yapılan DataSet ve DataNavigator
                        wnv.ds = ds;
                        wnv.dN = dN;

                        return findDbRow;
                    }

                    /*
                    if ((ds_DbaseTable == null) ||
                        (ds_DbaseTable.DataSetName != TableIPCode))
                    {
                        ds_DbaseTable = null;
                        dN_DbaseTable = null;
                        t.Find_DataSet(this, ref ds_DbaseTable, ref dN_DbaseTable, TableIPCode);
                    }

                    if (t.IsNotNull(ds_DbaseTable))
                    {
                        findDbRow = ds_DbaseTable.Tables[0].Rows[dN_DbaseTable.Position];

                        return findDbRow;
                    }
                    */
                }
            }

            if (findDbRow == null)
            {
                // v.SQL = v.SQL + v.ENTER + "DİKKAT : findRightRowForSelect, uygun row bulunamadı. PageCode : " + pageCode + ", nodeId : " + nodeId.ToString();
            }

            return findDbRow;
        }
        public void checkedSiraliIslemVarmi(Form tForm, webWorkPageNodes workPageNodes, List<MsWebScrapingDbFields> msWebScrapingFields)
        {
            if (msWebScrapingFields == null) return;
            if (msWebScrapingFields.Count == 0) return;

            string tableIPCode = "";
            string notFoundTableIPCodeList = "";
            bool taramaOnayi = true;
            bool siraliIslemButonOnayi = false;

            while (taramaOnayi)
            {
                tableIPCode = findTableIPCode(workPageNodes.aktifPageCode, msWebScrapingFields, notFoundTableIPCodeList);

                if (tableIPCode == "")
                {
                    taramaOnayi = false;
                }

                siraliIslemButonOnayi = findSiraliIslemButonu(tForm, workPageNodes, tableIPCode);

                if (siraliIslemButonOnayi)
                {
                    taramaOnayi = false;
                }
                else
                {
                    notFoundTableIPCodeList += "||" + tableIPCode;
                }
            }

        }
        private bool findSiraliIslemButonu(Form tForm, webWorkPageNodes workPageNodes, string tableIPCode)
        {
            bool onay = false;

            string[] controls = new string[] { };
            Control btn_SiraliIslem = null;
            btn_SiraliIslem = t.Find_Control(tForm, "checkButton_ek1", tableIPCode, controls);

            if (btn_SiraliIslem != null)
            {
                workPageNodes.siraliIslemVar = true;
                workPageNodes.siraliIslemTableIPCode = tableIPCode;
                workPageNodes.siraliIslem_Btn = btn_SiraliIslem;

                /// ilk bulduğunda default olarak işaretli olsun
                /// 
                ((DevExpress.XtraEditors.CheckButton)btn_SiraliIslem).Checked = true;
                
                workPageNodes.siraliIslemAktif = ((DevExpress.XtraEditors.CheckButton)btn_SiraliIslem).Checked;

                DataSet ds = null;
                DataNavigator dN = null;
                t.Find_DataSet(tForm, ref ds, ref dN, tableIPCode);
                if (ds != null)
                {
                    workPageNodes.siraliIslem_ds = ds;
                    workPageNodes.siraliIslem_dN = dN;
                    onay = true;
                }
            }
            
            return onay;
        }
        private string findTableIPCode(string aktifPageCode, List<MsWebScrapingDbFields> msWebScrapingFields, string notFoundTableIPCodeList)
        {
            string tableIPCode = "";
            foreach (MsWebScrapingDbFields item in msWebScrapingFields)
            {
                if (item.WebScrapingPageCode == aktifPageCode)
                {
                    if (notFoundTableIPCodeList.IndexOf(item.TableIPCode) == -1)
                    {
                        tableIPCode = item.TableIPCode;
                        break;
                    }
                }
            }
            return tableIPCode;
        }
        private string findNodeItemsValue(webNodeValue wnv, List<webNodeItemsList> aktifPageNodeItemsList, string findText)
        {
            //if (t.IsNotNull(ds_WebNodeItemsList) == false) return "";
            if (aktifPageNodeItemsList == null) return "";
            if (aktifPageNodeItemsList.Count == 0) return "";

            string value = "";
            int nodeId_ = wnv.nodeId;
            string pageCode_ = "";
            string itemText_ = "";
            string itemValue_ = "";
            findText = findText.Trim();

            //int length = ds_WebNodeItemsList.Tables[0].Rows.Count;


            //for (int i = 0; i < length; i++)
            foreach (webNodeItemsList item in aktifPageNodeItemsList)
            {
                pageCode_ = item.PageCode;   // ds_WebNodeItemsList.Tables[0].Rows[i]["PageCode"].ToString().Trim();
                itemValue_ = item.ItemValue; // ds_WebNodeItemsList.Tables[0].Rows[i]["ItemValue"].ToString().Trim();
                itemText_ = item.ItemText;   // ds_WebNodeItemsList.Tables[0].Rows[i]["ItemText"].ToString().Trim();

                /// nodeId kontrolü olmadı  
                /// get ve set node ler farklı oluyor
                /// get ederken tablolalarda alabiliyoruz
                /// set ederken select nodeyi kullanıyoruz
                /// haliyle nodeId tutmuyor
                /// 
                if (wnv.pageCode == pageCode_)
                {
                    if ((findText == itemText_) || (findText == itemValue_))
                    {
                        value = itemValue_; //ds_WebNodeItemsList.Tables[0].Rows[i]["ItemValue"].ToString();
                        return value;
                    }
                }
                else
                {
                    if ((findText == itemText_) || (findText == itemValue_))
                    {
                        if ((pageCode_ == "DONEMLER") || (pageCode_ == "GRUPLAR") || (pageCode_ == "SUBELER"))
                        {
                            value = itemValue_; // ds_WebNodeItemsList.Tables[0].Rows[i]["ItemValue"].ToString();
                            return value;
                        }
                    }
                }
            }

            // bir eşleşmezse bulmaz ise gelen value dönsün
            // böylece bazı WebNodeItems lar saklanmaya gerek kalmadı
            if (value == "")
            {
                MessageBox.Show("DİKKAT : MsWebNodeItems tablosunda aranan text in valuesi tespit edilemedi ..." + v.ENTER2 + wnv.AttName + " = " + findText);
                value = findText;
            }
            return value;
        }
        public bool findTableItemButton(List<MsWebNode> msWebNodes, tRow dataRow, int dataRowNo)
        {
            // Tablo üzerinde bulunduğu satırdaki bilgilerin detayını göstermek için buton var mı ?
            // 
            //v.SQL = v.SQL + v.ENTER + myNokta + " find tableItemButton RowNo : " + dataRowNo.ToString();

            bool onay = false;
            bool isActive = false;
            string value = "";
            string AttRole = "";

            //foreach (DataRow nodeRow in ds_MsWebNodes.Tables[0].Rows)
            foreach (MsWebNode item in msWebNodes)
            {
                isActive = item.IsActive; //(bool)nodeRow["IsActive"];
                AttRole = item.AttRole; //nodeRow["AttRole"].ToString();

                if (AttRole == "ItemButton")
                {
                    // şimdilik elimizde sadece
                    // tablonun column.value  ile  ds_ScrapingNodes  karşılaştırabileceğimiz sadece src adresi var
                    // 

                    // ds_ScrapingNodes üzerindeki value 
                    value = item.AttSrc; //nodeRow["AttSrc"].ToString();

                    int i2 = dataRow.tColumns.Count;
                    for (int i = 0; i < i2; i++)
                    {
                        if (value == dataRow.tColumns[i].value)
                        {
                            // burada ds_ScrapingNodes içindeki itemButton ait olan detay bilgisi gerekiyor
                            //
                            //this.myTriggerTableRow = nodeRow;

                            //tableItemButtonClickAsync(dataRowNo);

                            /// Buraya geldiysen Seleniuma göre itembutonu clickle

                            onay = true;
                            break;
                        }
                    }
                    // columns taradı ve bulamadı
                    break;
                }
            }

            return onay;
        }

        private async Task<bool> saveRowAsync(Form tForm, webNodeValue wnv, tRow tableRows, List<MsWebScrapingDbFields> msWebScrapingDbFields, List<webNodeItemsList> aktifPageNodeItemsList)
        {
            //v.SQL = v.SQL + v.ENTER + myNokta + " saveRow";

            /// table in bir row unu alıp tüm column larını tarayarak db ye kaydet işlemini gerçekleşitiriyor
            ///
            DataRow dbRow = null;
            bool onay = true;
            string itemText = "";
            string itemValue = "";
            Int16 colNo = 0;
            bool firstColumn = true;

            #region wnv.table verilerini db ye aktaralım

            foreach (tColumn column in tableRows.tColumns)
            {
                /// web üzerinden okunacak column noyu bildir
                wnv.tTableColNo = colNo;
                /// bu column datasının aktarılacağı tabloyu ve column nu bul
                wnv.dbFieldName = "";
                wnv.dbLookUpField = false;

                /// column üzerindeki bilgiler hangi db ye yazılacak tespit edelim
                if (wnv.TagName == "table")
                    dbRow = findRightRow(tForm, wnv, v.tSelect.Get, msWebScrapingDbFields);

                // burada select içindeki value ve text istenen bir tabloya yazılıcak
                if ((wnv.TagName == "select") &&
                    (wnv.AttRole == "ItemTable")) // (wnv.EventsType == v.tWebEventsType.itemTable))
                    dbRow = findRightRow(tForm, wnv, v.tSelect.Get, msWebScrapingDbFields);

                // burada select içindeki value ve text MsWebNodeItems tablosuna yazılacak
                if ((wnv.TagName == "select") &&
                    (wnv.AttRole != "ItemTable"))//(wnv.EventsType != v.tWebEventsType.itemTable))
                    dbRow = findRightRowForSelect(tForm, wnv, v.tSelect.Get, msWebScrapingDbFields);

                /// yeni data satırı oluştur
                /// tabloy aktarmaya başladığında ilk yeni row oluştursun
                /// bir defa çalışıyor
                if ((firstColumn) && (dbRow != null))
                {
                    dbButtonClick(tForm, wnv.ds.DataSetName, v.tButtonType.btYeniHesapSatir);
                    dbRow = wnv.ds.Tables[0].Rows[wnv.dN.Position];
                    firstColumn = false;
                }

                /// dbRow geldiyse ilgili tablo ve column bulunmuş demektir
                /// webden okunan veriyi db ye aktardığı an
                if (dbRow != null)
                {
                    // select node için itemValue tespiti
                    itemText = tableRows.tColumns[colNo].value;

                    if (t.IsNotNull(wnv.dbFieldName))
                    {
                        if (wnv.dbLookUpField == false)
                        {
                            try
                            {
                                if (itemText != "")
                                    dbRow[wnv.dbFieldName] = itemText;
                            }
                            catch (Exception exc1)
                            {
                                onay = false;
                                MessageBox.Show("Error, db Field set : " + wnv.dbFieldName + " = " + itemText + v.ENTER2 + exc1.Message);
                                //throw;
                            }
                        }
                        else
                        {
                            itemValue = findNodeItemsValue(wnv, aktifPageNodeItemsList, itemText);

                            if (itemValue != "")
                                dbRow[wnv.dbFieldName] = itemValue;
                        }
                    }
                }

                colNo++;
            }

            #endregion wnv.table verilerini db ye aktaralım

            
            /// selenium göre bura düzenlecek
            
            /// Sıra geldi wvn.table dışındaki web üzerinde bulunan başka verileri alalım
            /// 
            ////if (wnv.TagName == "table")
            //if (this.myWebTableRowToDatabase)
            //{
            //    v.SQL = v.SQL + v.ENTER + myNokta + " saveRow : start other nodes read ";
            //    //runScrapingAsync(ds_MsWebNodes, this.myTriggerWebRequestType, this.myTriggerEventsType, 0, wnv.nodeId);
            //    runScrapingAsync(ds_MsWebNodes, this.myTriggerWebRequestType, v.tWebEventsType.tableField, 0, wnv.nodeId);
            //}

            if ((wnv.TagName == "table") ||
                (wnv.TagName == "select"))
            {
                /// final : yeni dbrow oluşturuldu, 
                /// wnv.table üzerindeki tüm colomn tarandı ve gerekli atamalar yapıldı
                /// wnv.table dışındaki diğer column lar da tarandı ve bulununca onlarda web den alındı
                /// bir row un okunması tamalandı, o zaman bunu kaydedelim, ve yeni satır açalım
                /// 
                if (t.IsNotNull(wnv.ds) && (wnv.DontSave == false))
                {
                    dbButtonClick(tForm, wnv.ds.DataSetName, v.tButtonType.btKaydetYeni);
                }
            }

            return onay;
        }


        public byte[] ResmiKucult(byte[] byteArrayIn)
        {
            // gelen byte image çevir
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image oldImage = Image.FromStream(ms);

            // image yi bitmap a çevir
            Bitmap workingImage = new Bitmap(oldImage, oldImage.Width, oldImage.Height);

            // % 80 oranında küçült
            int newWidth = (int)(workingImage.Width * 0.2);
            int newHeight = (int)(workingImage.Height * 0.2);

            Bitmap _img = myImageCompress_(workingImage, newWidth, newHeight);

            /// tekrar byte dönüştür ve gönder
            ms = null;
            ms = new MemoryStream();
            _img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
        public void dbButtonClick(Form tForm, string tableIPCode, v.tButtonType buttonType)
        {
            string type_ = "";
            if (buttonType == v.tButtonType.btKaydet) type_ = "Kaydet";
            if (buttonType == v.tButtonType.btYeniHesapSatir) type_ = "YeniHesapSatir";
            if (buttonType == v.tButtonType.btKaydetYeni) type_ = "KaydetYeni";

            //v.SQL = v.SQL + v.ENTER + myNokta + " dbButtonClick ( " + type_ + " ) ";

            v.tButtonHint.Clear();
            v.tButtonHint.tForm = tForm;
            v.tButtonHint.tableIPCode = tableIPCode;
            v.tButtonHint.buttonType = buttonType;
            tEventsButton evb = new tEventsButton();
            evb.btnClick(v.tButtonHint);

            Application.DoEvents();
        }
        private Bitmap myImageCompress_(Bitmap workingImage, int newWidth, int newHeight)
        {
            Bitmap _img = new Bitmap(newWidth, newHeight, workingImage.PixelFormat);
            _img.SetResolution(workingImage.HorizontalResolution, workingImage.VerticalResolution);

            /// for new small image
            Graphics g = Graphics.FromImage(_img);
            /// create graphics
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            g.DrawImage(workingImage, 0, 0, newWidth, newHeight);

            return _img;
        }
        public bool editKayitKontrolu()
        {
            bool onay = false;

            if ((v.con_EditSaveCount == 10) || (v.con_EditSaveCount == 25) || (v.con_EditSaveCount == 50))
            {
                string soru = "Sürekli olarak eski kayıtlar denk gelmeye başladı. İşlemi durdurmak ister misiniz ?";
                DialogResult cevap = t.mySoru(soru);
                if (DialogResult.Yes == cevap)
                {
                    v.con_EditSaveCount = 0;
                    onay = true;
                }
            }
            return onay;
        }

        public void getMebbisCode()
        {
            string tSql = "";
            DataSet ds = new DataSet();

            if (v.SP_TabimDbConnection)
            {
                tSql = sql.preparingTabimUsersSql("", "", v.tUser.UserId); // UserId ile giriş
                t.SQL_Read_Execute(v.dBaseNo.Local, ds, ref tSql, "TabimUsers", "FindUser");
            }
            else
            {
                tSql = sql.preparingUstadUsersSql("", "", v.tUser.UserId);
                t.SQL_Read_Execute(v.dBaseNo.UstadCrm, ds, ref tSql, "Users", "FindUser");
            }

            if (t.IsNotNull(ds))
            {
                if (v.SP_TabimDbConnection)
                {
                    v.tUser.MebbisCode = ds.Tables[0].Rows[0]["MebbisCode"].ToString();
                    v.tUser.MebbisPass = ds.Tables[0].Rows[0]["MebbisPass"].ToString();
                }
                else
                {
                    v.tUser.MebbisCode = ds.Tables[0].Rows[0]["MebbisCode"].ToString();
                    v.tUser.MebbisPass = ds.Tables[0].Rows[0]["MebbisPass"].ToString();
                }
            }

        }

        ///-- CEFSharp ile Selenium Formların ortak fonksiyon çalışmaları
        ///
        public v.tBrowserType GetBrowserType(Form tForm)
        {
            v.tBrowserType browserType = v.tBrowserType.none;

            string acc = tForm.AccessibilityObject.ToString();

            if (acc.IndexOf("ms_CefSharp") > -1) browserType = v.tBrowserType.CefSharp;
            if (acc.IndexOf("ms_Selenium") > -1) browserType = v.tBrowserType.Selenium;

            return browserType;
        }

        public void scrapingPages_PositionChanged(webWorkPageNodes workPageNodes, webForm f)
        {
            if (f.autoSubmit)
            {
                if ((workPageNodes.aktifPageCode != "MTSKADAYRESIM") &&
                    (workPageNodes.aktifPageCode != "MTSKADAYSOZLESME") &&
                    (workPageNodes.aktifPageCode != "MTSKADAYIMZA"))
                    f.btn_FullPost1.Visible = true;
                else f.btn_FullPost1.Visible = false;

                if ((workPageNodes.aktifPageCode == "MTSKADAYRESIM") ||
                    (workPageNodes.aktifPageCode == "MTSKADAYSOZLESME") ||
                    (workPageNodes.aktifPageCode == "MTSKADAYIMZA"))
                    f.btn_FullPost2.Visible = true;
                else f.btn_FullPost2.Visible = false;
            }
            else
            {
                if (f.btn_FullPost1 != null) f.btn_FullPost1.Visible = true;
                if (f.btn_FullPost2 != null) f.btn_FullPost2.Visible = true;
                if (f.btn_FullSave != null) f.btn_FullSave.Visible = true;
            }
        }

        public void preparingMsWebPagesButtons(Form tForm, webForm f, string TableIPCode)
        {
            /// simpleButton_ek1 :  line get  / pageView
            /// simpleButton_ek2 :  line post / AlwaysSet Tc sorgula gibi
            /// simpleButton_ek3 :  full get1  : v.tWebEventsType.button1
            /// simpleButton_ek4 :  full get2  : v.tWebEventsType.button2
            /// simpleButton_ek5 :  full post1 : v.tWebEventsType.button3
            /// simpleButton_ek6 :  full post2 : v.tWebEventsType.button4
            /// simpleButton_ek7 :  full save  : v.tWebEventsType.button5
            //Control cntrl = null;
            string[] controls = new string[] { };
            #region
            f.btn_PageView = t.Find_Control(tForm, "simpleButton_ek1", TableIPCode, controls);
            // Page View
            //if (f.btn_PageView != null)
            //{
            //    ((DevExpress.XtraEditors.SimpleButton)f.btn_PageView).Click += new System.EventHandler(myPageViewClick);
            //}
            // Bilgileri Sorgula / AlwaysSet  (TcNo sorgula gibi) // Bu henüz hiç kullanılmadı 
            f.btn_AlwaysSet = t.Find_Control(tForm, "simpleButton_ek2", TableIPCode, controls);
            //if (f.btn_AlwaysSet != null)
            //{
            //    ((DevExpress.XtraEditors.SimpleButton)f.btn_AlwaysSet).Click += new System.EventHandler(myAlwaysSetClick);
            //}
            // Bilgileri Al 1
            f.btn_FullGet1 = t.Find_Control(tForm, "simpleButton_ek3", TableIPCode, controls);
            //if (f.btn_FullGet1 != null)
            //{
            //    ((DevExpress.XtraEditors.SimpleButton)f.btn_FullGet1).Click += new System.EventHandler(myFullGet1Click);
            //}
            // Bilgileri Al 2
            f.btn_FullGet2 = t.Find_Control(tForm, "simpleButton_ek4", TableIPCode, controls);
            //if (f.btn_FullGet2 != null)
            //{
            //    ((DevExpress.XtraEditors.SimpleButton)f.btn_FullGet2).Click += new System.EventHandler(myFullGet2Click);
            //}
            // Bilgileri Gönder 1
            f.btn_FullPost1 = t.Find_Control(tForm, "simpleButton_ek5", TableIPCode, controls);
            //if (f.btn_FullPost1 != null)
            //{
            //    ((DevExpress.XtraEditors.SimpleButton)f.btn_FullPost1).Click += new System.EventHandler(myFullPost1Click);
            //}
            // Bilgileri Gönder 2
            f.btn_FullPost2 = t.Find_Control(tForm, "simpleButton_ek6", TableIPCode, controls);
            //if (f.btn_FullPost2 != null)
            //{
            //    ((DevExpress.XtraEditors.SimpleButton)f.btn_FullPost2).Click += new System.EventHandler(myFullPost2Click);
            //}
            // Bilgileri Kaydet
            f.btn_FullSave = t.Find_Control(tForm, "simpleButton_ek7", TableIPCode, controls);
            //if (f.btn_FullSave != null)
            //{
            //    ((DevExpress.XtraEditors.SimpleButton)f.btn_FullSave).Click += new System.EventHandler(myFullSaveClick);
            //}
            // Otomatik kaydet için
            f.btn_AutoSubmit = t.Find_Control(tForm, "checkButton_ek1", TableIPCode, controls);
            //if (f.btn_AutoSubmit != null)
            //{
            //    ((DevExpress.XtraEditors.SimpleButton)f.btn_AutoSubmit).Click += new System.EventHandler(myAutoSubmit);
            //}
            #endregion


        }

        public void preparingMsWebPages(DataSet ds_MsWebPages, ref List<MsWebPage> msWebPages)
        {
            msWebPages = t.RunQueryModels<MsWebPage>(ds_MsWebPages);
        }
        public void preparingMsWebNodesFields(Form tForm,
            ref List<MsWebPage> msWebPage,
            ref List<MsWebNode> msWebNodes,
            ref webWorkPageNodes workPageNodes,
            List<MsWebScrapingDbFields> msWebScrapingDbFields,
            DataSet ds_MsWebPages,
            DataSet ds_MsWebNodes,
            DataNavigator dN_MsWebPages
            )
        {
            msWebPage = t.RunQueryModelsSingle<MsWebPage>(ds_MsWebPages, dN_MsWebPages.Position);
            msWebNodes = t.RunQueryModels<MsWebNode>(ds_MsWebNodes);
            
            workPageNodes.Clear();
            workPageNodes.aktifPageCode = msWebPage[0].PageCode;
            workPageNodes.aktifPageUrl = msWebPage[0].PageUrl;

            //msPagesService.
            checkedSiraliIslemVarmi(tForm, workPageNodes, msWebScrapingDbFields);
        }

        public bool LoginOnayi(DataSet ds_MsWebPages, DataNavigator dN_MsWebPages)
        {
            bool onay = false;
            string soru = "Mebbis Giriş sayfasını açmak ister misiniz ?";

            string loginPage = ds_MsWebPages.Tables[0].Rows[dN_MsWebPages.Position]["LoginPage"].ToString();

            if (loginPage == "True")
            {
                DialogResult cevap = t.mySoru(soru);
                if (DialogResult.Yes == cevap)
                {
                    onay = true;
                }
            }

            return onay;
        }
        /*
            Form tForm = (((DevExpress.XtraGrid.Views.Tile.TileView)sender).GridControl).FindForm();
            v.browserType browserType = msPagesService.GetBrowserType(tForm);
            if (browserType == v.browserType.Selenium)

        */

        public void getPageUrls(webForm f, List<MsWebPage> msWebPage)
        {
            f.aktifPageCode = msWebPage[0].PageCode;
            f.talepEdilenUrl = msWebPage[0].PageUrl;
            f.talepEdilenUrl2 = msWebPage[0].PageUrl;
            f.talepOncesiUrl = msWebPage[0].BeforePageUrl;
            f.talepPageLeft = msWebPage[0].PageLeft;
            f.talepPageTop = msWebPage[0].PageTop;
        }

        public void preparingMsWebLoginPage(webForm f, DataSet ds_LoginPageNodes, List<MsWebNode> msWebLoginNodes)
        {
            bool onay = false;

            if (t.IsNotNull(ds_LoginPageNodes) == false)
            {
                f.aktifUrl = "loginPageYükle";
                onay = readLoginPageControl(ref ds_LoginPageNodes, f);
                f.aktifUrl = "";

                if ((onay) && (msWebLoginNodes == null))
                    msWebLoginNodes = t.RunQueryModels<MsWebNode>(ds_LoginPageNodes);
            }
        }

        public void preparingDataSets(Form tForm, System.EventHandler positionChanged)
        {
            #region DataNavigator Listesi Hazırlanıyor

            List<string> list = new List<string>();
            t.Find_DataNavigator_List(tForm, ref list);

            Control cntrl = new Control();
            string[] controls = new string[] { "DevExpress.XtraEditors.DataNavigator" };

            foreach (string value in list)
            {
                cntrl = t.Find_Control(tForm, value, "", controls);
                if (cntrl != null)
                {
                    //((DevExpress.XtraEditors.DataNavigator)cntrl).PositionChanged += new System.EventHandler(dataNavigator_PositionChanged);
                    ((DevExpress.XtraEditors.DataNavigator)cntrl).PositionChanged += new System.EventHandler(positionChanged);
                } // if cntrl != null
            }//foreach

            #endregion DataNavigator Listesi
        }

    }
}
