using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.SchemeHandler;
using System.Reflection;

namespace YesiLdefter.CEFSharp
{
    public static class CEFHelper
    {
        
        private static ChromiumWebBrowser _cefBrowser = null;

        public static ChromiumWebBrowser CreateBrowser
        {
            get
            {
                if (_cefBrowser != null)
                {
                    return _cefBrowser;
                }

                CefSettings settings = new CefSettings() { RemoteDebuggingPort = 8090 };
                settings.CefCommandLineArgs.Add("remote-allow-origins", "*");

                /* local html kullanacığın zaman açacaksın
                 * 
                settings.RegisterScheme(new CefCustomScheme
                {
                    SchemeName = CustomProtocolSchemeHandlerFactory.SchemeName,
                    SchemeHandlerFactory = new CustomProtocolSchemeHandlerFactory()
                });

                // yüklemek için

                chromiumWebBrowser.LoadUrl("resource://ui/index.html");

                /// unutma index.html sayfasını projenin kaynak kodlarına ekledikten sonra (yesiLdefterV3.sln)
                /// index.html dosyasının properties ini açarak
                /// Build Action : Content
                /// Build Action : Embedded Resource  
                /// şeklinde değiştirmen gerekiyor
                /// Eğer index.html içinde çağırdığın resim dosyaları var ise onlara aynısı yapacaksın

                */

                Cef.Initialize(settings);
                _cefBrowser = new ChromiumWebBrowser();

                return _cefBrowser;
            }
        }

        /// proje içinde localdeki bir html sayfasını açmak istediğinde kullanırsın
        /// 

        public class ResourceSchemeHandler : ResourceHandler
        {
            public override CefReturnValue ProcessRequestAsync(IRequest request, ICallback callback)
            {
                Uri u = new Uri(request.Url);
                string file = u.Authority + u.AbsolutePath;

                Assembly ass = Assembly.GetExecutingAssembly();
                string resourcePath = ass.GetName().Name + "." + file.Replace("/", ".");

                if (ass.GetManifestResourceStream(resourcePath) != null)
                {
                    Stream = ass.GetManifestResourceStream(resourcePath);

                    switch (Path.GetExtension(file))
                    {
                        case ".html":
                            MimeType = "text/html";
                            break;
                        case ".js":
                            MimeType = "text/javascript";
                            break;
                        case ".png":
                            MimeType = "image/png";
                            break;
                        case ".jpg":
                        case ".jpeg":
                            MimeType = "image/jpeg";
                            break;
                        case ".gif":
                            MimeType = "image/gif";
                            break;
                        case ".appchache":
                        case ".manifest":
                            MimeType = "text/cache-manifest";
                            break;
                        default:
                            MimeType = "application/octet-stream";
                            break;
                    }

                    callback.Continue();
                    return CefReturnValue.Continue;
                }

                callback.Dispose();
                return CefReturnValue.Cancel;
            }
        }
    
        public class CustomProtocolSchemeHandlerFactory : ISchemeHandlerFactory
        {
            public const string SchemeName = "resource";
            public IResourceHandler Create (IBrowser browser, IFrame frame, string schemeName, IRequest request)
            {
                return new ResourceSchemeHandler();
            }
        }
                
    }
}
