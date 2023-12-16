using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using Tkn_ToolBox;
using Tkn_Variable;
using System.Net;
using Tkn_CookieReader;
using YesiLdefter.Entities;

namespace YesiLdefter.Selenium
{
    public class MsWebPagesService
    {
        tToolBox t = new tToolBox();
        //public bool readLoginPageControl(ref DataSet ds_LoginPageNodes, string aktifUrl, ref string loginPageUrl, ref string errorPageUrl)
        public bool readLoginPageControl(ref DataSet ds_LoginPageNodes, webForm f)
        {
            if (f.aktifUrl == "") return false;

            if (t.IsNotNull(ds_LoginPageNodes))
            {
                /// Daha önce aynı login url yüklenmişse bir daha okumaya gerek yok
                ///
                if (ds_LoginPageNodes.Namespace == f.aktifUrl)
                    return true;
            }

            bool onay = false;
            readLoginPageNodes(ref ds_LoginPageNodes, f.aktifUrl);

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

        private bool readLoginPageNodes(ref DataSet ds, string Url)
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
              and ( b.PageUrl = '" + Url + "' or b.ErrorPageUrl = '" + Url + "' )  order by a.NodeId ";

            onay = t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "LoginPageNodes", "LoginPage");

            return onay;
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

    }
}
