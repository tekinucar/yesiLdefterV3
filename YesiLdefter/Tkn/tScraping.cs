using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tkn_WebScraping
{
    class tScraping
    {
    }

    public class CrawlerKeyValuePair
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class CrawlerKeyValuePairCollection
    {
        private List<CrawlerKeyValuePair> _list = new List<CrawlerKeyValuePair>();

        public void Add(string key, string value)
        {
            _list.Add(new CrawlerKeyValuePair() { Key = key, Value = value });
        }

        public void Clear()
        {
            _list.Clear();
        }

        public void Remove(string key)
        {
            _list.RemoveAll(x => x.Key == key);
        }

        public List<CrawlerKeyValuePair> List
        {
            get { return _list; }
        }

        public CrawlerKeyValuePair this[int index] => _list[index];

        public string this[string key]
        {
            get
            {
                var item = _list.FirstOrDefault(x => x.Key == key);
                if (item != null)
                {
                    return item.Value;
                }
                return "";
            }
            set
            {
                var crawlerKeyValuePairList = _list.Where(x => x.Key == key).ToList();
                if (crawlerKeyValuePairList.Count > 0)
                {
                    foreach (var crawlerKeyValuePair in crawlerKeyValuePairList)
                    {
                        crawlerKeyValuePair.Value = value;
                    }
                }
                else
                {
                    Add(key, value);
                }
            }
        }

    }

    public class CrawlerHttpClient
    {
        private readonly System.Net.Http.HttpClient _client = null;
        public CrawlerKeyValuePairCollection Headers { get; } = new CrawlerKeyValuePairCollection();

        public CrawlerHttpClient()
        {
            var cookieContainer = new System.Net.CookieContainer();
            var httpClientHandler = new System.Net.Http.HttpClientHandler() { CookieContainer = cookieContainer };
            _client = new System.Net.Http.HttpClient(httpClientHandler);

        }

        public async Task<string> Get(string url)
        {
            _client.DefaultRequestHeaders.Clear();
            foreach (var header in Headers.List)
            {
                if (header.Key.ToLowerInvariant() == "accept-encoding")
                    continue;

                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var httpResponseString = await _client.GetStringAsync(url);
            return httpResponseString;
        }

        public async Task<string> Get(string url, CrawlerKeyValuePairCollection getParameters, bool isEncode = false)
        {
            //www.ornek.com/?product=t-tshirt&color=black&size=small
            string parameterString = "";
            foreach (var parameter in getParameters.List)
            {
                parameterString += (parameterString == "") ? "?" : "&";
                if (isEncode)
                    parameterString +=
                        System.Net.WebUtility.UrlDecode(parameter.Key) + "=" +
                        System.Net.WebUtility.UrlDecode(parameter.Value);
                else
                    parameterString += parameter.Key + "=" + parameter.Value;
            }

            var result = url + parameterString;
            return await Get(result);
        }

        public async Task<string> Post(string url, string encodeParameters, string media="appllication/x-www-form-urlencoded")
        {
            _client.DefaultRequestHeaders.Clear();
            foreach (var header in Headers.List)
            {
                if (header.Key.ToLowerInvariant() == "accept-encoding")
                    continue;
                if (header.Key.ToLowerInvariant() == "content-type")
                    continue;
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var decodeString = System.Net.WebUtility.UrlDecode(encodeParameters);
            var stringContent = new System.Net.Http.StringContent(decodeString, Encoding.UTF8, media);

            var httpResponse = await _client.PostAsync(url, stringContent);

            httpResponse.EnsureSuccessStatusCode();

            var httpResponseString = await httpResponse.Content.ReadAsStringAsync();

            return httpResponseString;

        }

        public async Task<string> Post(string url, CrawlerKeyValuePairCollection parameters, bool isEncode = false)
        {
            var parameterString = "";
            foreach (var parameter in parameters.List)
            {
                if (isEncode)
                    parameterString +=
                        System.Net.WebUtility.UrlDecode(parameter.Key) + "=" +
                        System.Net.WebUtility.UrlDecode(parameter.Value) + "&";
                else
                    parameterString += parameter.Key + "=" + parameter.Value + "&";
            }

            if (parameterString.Length > 0)
                parameterString = parameterString.Remove(parameterString.Length - 1, 1);

            return await Post(url, parameterString);
        }


    }

    public interface ICrawler
    {
        string CrawlerName { get; }
        Task<string> StartTheCrawler();
        List<string> LogMessageList { get; set; }
    }

    public abstract class CrawlerBase : ICrawler
    {
        public abstract string CrawlerName { get; }

        public List<string> LogMessageList { get; set; }

        public abstract Task<string> StartTheCrawler();

        public void SendMessage(string message)
        {
            Console.WriteLine(message);
        }
        
    }

    public class RegexHelper
    {
        public static string ParseString(string regexPattern, string text, string groupName)
        {
            string result = "";

            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var regexMatch = regex.Match(text);
            if (regexMatch.Success)
                result = regexMatch.Groups[groupName].ToString();

            return result;
        }

        public List<string> ParseStringMultiResult(string regexPattern, string text, string groupName)
        {
            var result = new List<string>();
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var regexMatch = regex.Matches(text);
            foreach (Match match in regexMatch)
            {
                result.Add(match.Groups[groupName].ToString());
            }
            
            return result;
        }

        public Dictionary<string, string> ParseStringMultiGroup(string regexPattern, string text, params string[] groupNames)
        {
            var result = new Dictionary<string, string>();
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var regexMatch = regex.Matches(text);
            foreach (Match match in regexMatch)
            {
                foreach (var groupName in groupNames)
                {
                    result.Add(groupName, match.Groups[groupName].ToString());
                }
            }
            return result;
        }


    }

    public class FiddlerHelper
    {
        public enum CurrentState
        {
            firstRow,
            headers,
            parameterValues,
            finish
        }

        public void textBox1Changed()
        {
            var url = "";
            var method = "";
            bool isCompleted = false;
            var headers = new Dictionary<string, string>();
            var currentState = CurrentState.firstRow;
            var parameters = "";
            bool parameterChecked = false; // form üzerindeki seçenek
            bool decodeChecked = false;    // form üzerinde

            // bunu ayarla
            string[] textLines = new string[5000];
            //List<string> text new List<string>();
            //text.Add

            for (int i = 0; i < 5000; i++)
            {
                var line = textLines[i];
                if (currentState == CurrentState.firstRow)
                {
                    var regexPattern = "(?<method>POST|GET) (?<url>.+) HTTP";
                    var regexMatch = new Regex(regexPattern).Match(line);
                    if (regexMatch.Success)
                    {
                        url = regexMatch.Groups["url"].ToString();
                        method = regexMatch.Groups["method"].ToString();
                        currentState = CurrentState.headers;
                        continue;
                    }
                }
                else if (currentState == CurrentState.headers)
                {
                    if (line == "")
                    {
                        if (method == "POST")
                        {
                            currentState = CurrentState.parameterValues;
                        }
                        else
                        {
                            currentState = CurrentState.finish;
                        }
                        continue;
                    }

                    var regexPattern = @"(?<key>[A-Za-z\-]+):(?<value>.+)";
                    var regexMatch = new Regex(regexPattern).Match(line);
                    if (regexMatch.Success)
                    {
                        var key = regexMatch.Groups["key"].ToString();
                        var value = regexMatch.Groups["value"].ToString();
                        headers.Add(key, value);
                    }
                }
                else if (currentState == CurrentState.parameterValues)
                {
                    //email=tekin&
                    //password=123456&
                    //remember_me=1

                    //postparametersCollection.Add("kullaniciAdi","tekin");
                    //postparametersCollection.Add("password","123456");

                    if (line == "" || line.StartsWith("HTTP/"))
                    {
                        currentState = CurrentState.finish;
                        continue;
                    }

                    if (parameterChecked)
                    {
                        parameters += line;
                    }
                    else
                    {
                        parameters += "postParametersCollection.Clear();\r\n";
                        var parameterParts = line.Split('&');
                        foreach (var parameterPart in parameterParts)
                        {
                            if (parameterPart == "") continue;
                            var nameValuePart = parameterPart.Split('=');
                            if (decodeChecked)
                            {
                                parameters += "postParametersCollection.Add($\"" +
                                    System.Net.WebUtility.UrlDecode(nameValuePart[0]) + "\",$\"" +
                                    System.Net.WebUtility.UrlDecode(nameValuePart[1]) + "\");\r\n";
                            }
                            else
                            {
                                parameters += "postParametersCollection.Add($\"" +
                                    nameValuePart[0] + "\",$\"" +
                                    nameValuePart[1] + "\");\r\n";
                            }
                        }
                    }
                }
                else if (currentState == CurrentState.finish)
                {
                    if (isCompleted == false)
                    {
                        var headerLines = "#region Header\r\n\r\n";
                        headerLines += "WebClient.Headers.Clear();\r\n";
                        foreach (var header in headers)
                        {
                            if (header.Key.ToLowerInvariant().Trim() == "connection") continue;
                            if (header.Key.ToLowerInvariant().Trim() == "cookie") continue;
                            if (header.Key.ToLowerInvariant().Trim() == "content-length") continue;
                            headerLines += $"WebClient.Headers.Add($\"{header.Key}\",$\"{header.Value.Trim()}\");\r\n";
                        }

                        headerLines += "#endregion\r\n\r\n";

                        var parameterLines = "";
                        if (method == "POST")
                        {
                            parameterLines = "#regin Post-Parameters\r\n\r\n";
                            if (parameterChecked)
                            {
                                parameterLines += "postParameter=$\"" + parameters.Replace("\"", "\\\"") + "\";\r\n";
                            }
                            else
                            {
                                parameterLines += parameters + "\r\n";
                            }

                            parameterLines += "#endregion\r\n\r\n";
                        }
                        else if (method == "GET" && parameterChecked == false)
                        {
                            parameterLines = "#regin Get-Parameters\r\n\r\n";
                            var urlRegexPattern = "[\\?&]+(?<paramname>[^=]*)=(?<paramvalue>[^&]*)";
                            var regex = new Regex(urlRegexPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            var regexMatch = regex.Matches(url);
                            parameterLines += "getParametersCollection.Clear();\r\n";
                            foreach (Match match in regexMatch)
                            {
                                if (decodeChecked)
                                {
                                    parameterLines += "getParametersCollection.Add($\"" +
                                        System.Net.WebUtility.UrlDecode(match.Groups["paramname"].ToString().Replace("\"", "\\\"")) + "\",$\"" +
                                        System.Net.WebUtility.UrlDecode(match.Groups["paramvalue"].ToString().Replace("\"", "\\\"")) + "\");\r\n";
                                }
                                else
                                {
                                    parameterLines += "getParametersCollection.Add($\"" +
                                        match.Groups["paramname"].ToString() + "\",$\"" +
                                        match.Groups["paramvalue"].ToString() + "\");\r\n";
                                }

                                parameterLines += "#endregion\r\n\r\n";
                            }
                        }

                        var finalLines = "";
                        if (method == "GET")
                        {
                            if (parameterChecked == false)
                            {
                                //RegexHelper.ParseString
                                var urlBase = RegexHelper.ParseString("(?<urlbase>http[^\\?]*)", url, "urlbase");
                                if (decodeChecked)
                                {
                                    finalLines += "response = await WebClint.Get(&\"" + urlBase + "\",getParametersCollection,true);\r\n";
                                }
                                else
                                {
                                    finalLines += "response = await WebClint.Get(&\"" + urlBase + "\",getParametersCollection,false);\r\n";
                                }
                            }
                            else
                            {
                                finalLines += "response = await WebClint.Get(&\"" + url + "\");\r\n";
                            }
                        }
                        else
                        {
                            if (parameterChecked)
                            {
                                finalLines += "response = await WebClint.Post(&\"" + url + "\");\r\n";
                            }
                            else
                            {
                                if (decodeChecked)
                                {
                                    finalLines += "response = await WebClint.Post(&\"" + url + "\",postParametersCollection,true);\r\n";
                                }
                                else
                                {
                                    finalLines += "response = await WebClint.Post(&\"" + url + "\",postParametersCollection,false);\r\n";
                                }
                            }
                        }

                        string titleText = " abc ";
                        string textBox2 = "------" + (titleText ?? "Undefined") + "------\r\n";
                        textBox2 += headerLines + "\r\n";
                        textBox2 += parameterLines + "\r\n";
                        textBox2 += finalLines + "\r\n";

                        isCompleted = true;
                        headers.Clear();
                        url = "";
                        method = "";
                        parameters = "";

                    }
                
                    if (line.StartsWith("-----"))
                    {
                        currentState = CurrentState.firstRow;
                        isCompleted = false;
                    }
                }
            }
        }

    }

}
