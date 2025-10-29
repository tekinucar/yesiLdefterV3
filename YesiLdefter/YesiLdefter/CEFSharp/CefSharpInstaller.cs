using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YesiLdefter.CEFSharp
{
    public class CefSharpInstaller
    {
       // private const string CefSharpVersion = "129.0.110";
        private const string CefSharpUrl = "https://github.com/cefsharp/CefSharp/releases/download/v{CefSharpVersion}/";

        /// <summary>
        /// ÇALIŞTIRAMADIM
        /// </summary>
        /// <returns></returns>

        public static async Task EnsureCefSharpComponents()
        {
            string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var requiredFiles = new[]
            {
            $"CefSharp.Core.Runtime.dll",
            $"CefSharp.Core.dll",
            $"CefSharp.dll",
            $"CefSharp.WinForms.dll",
            "libcef.dll",
            "chrome_elf.dll",
            "icudtl.dat",
            "snapshot_blob.bin",
            "v8_context_snapshot.bin",
"Resources/cef.pak",
"Resources/cef_100_percent.pak",
"Resources/CefSharp.BrowserSubprocess.Core.dll",
"Resources/CefSharp.BrowserSubprocess.Core.pdb",
"Resources/CefSharp.BrowserSubprocess.exe"
        };




            foreach (var file in requiredFiles)
            {
                string localPath = Path.Combine(appDirectory, file);
                if (!File.Exists(localPath))
                {
                    try
                    {
                        string directory = Path.GetDirectoryName(localPath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        string downloadUrl = $"{CefSharpUrl}{file}";
                        await DownloadFileAsync(downloadUrl, localPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Dosya indirilirken hata oluştu: {file}\n{ex.Message}");
                    }
                }
            }
        }

        private static async Task DownloadFileAsync(string url, string localPath)
        {
            string CefSharpVersion = "129.0.110";
            
            using (WebClient client = new WebClient())
            {
                localPath = localPath.Replace("Resources/", "x64\\");
                url = url.Replace("v{CefSharpVersion}", "v"+ CefSharpVersion);
                await client.DownloadFileTaskAsync(new Uri(url), localPath);
            }
        }
    }
}