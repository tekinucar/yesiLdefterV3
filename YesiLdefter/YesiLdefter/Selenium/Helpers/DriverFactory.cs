using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using Tkn_Variable;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace YesiLdefter.Selenium.Helpers
{
    public enum DriverToUse
    {
        InternetExplorer,
        Chrome,
        Firefox
    }
    public class DriverFactory
    {
        private static FirefoxOptions FirefoxOptions
        {
            get
            {
                var firefoxProfile = new FirefoxOptions();
                firefoxProfile.SetPreference("network.automatic-ntlm-auth.trusted-uris", "http://localhost");
                return firefoxProfile;
            }
        }

        public IWebDriver Create()
        {
            IWebDriver driver;


            var driverToUse =
               (DriverToUse)Enum.Parse(typeof(DriverToUse), SeleniumHelper.DriverToUse);

            switch (driverToUse)
            {
                case DriverToUse.InternetExplorer:
                    driver = new InternetExplorerDriver(AppDomain.CurrentDomain.BaseDirectory, new InternetExplorerOptions(), TimeSpan.FromMinutes(5));
                    break;
                case DriverToUse.Firefox:
                    var firefoxProfile = FirefoxOptions;
                    driver = new FirefoxDriver(firefoxProfile);
                    driver.Manage().Window.Maximize();
                    break;
                case DriverToUse.Chrome:

                    var driverVersion = new WebDriverManager.DriverConfigs.Impl.ChromeConfig()
                                .GetMatchingBrowserVersion();
                    var driverPath = $"./Chrome/{driverVersion}/X64/";

                    new WebDriverManager.DriverManager()
                        .SetUpDriver(new WebDriverManager.DriverConfigs.Impl.ChromeConfig(),
                                     WebDriverManager.Helpers.VersionResolveStrategy.MatchingBrowser);

                    var chromeDriverService = ChromeDriverService.CreateDefaultService(driverPath);
                    chromeDriverService.HideCommandPromptWindow = true;

                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--disable-gpu");
                    chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArgument("--remote-debugging-port=9222"); // data:, sorunu azaltır

                    // Temiz profil kullan (önceki oturumun artıkları sorun yaratmasın)
                    chromeOptions.AddArgument("--user-data-dir=" + Path.Combine(Path.GetTempPath(), "SeleniumProfile_" + Guid.NewGuid()));

                    // İndirme ayarları
                    string downloadDirectory = v.EXE_TempPath;
                    chromeOptions.AddUserProfilePreference("download.default_directory", downloadDirectory);
                    chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                    chromeOptions.AddUserProfilePreference("directory_upgrade", true);

                    driver = new ChromeDriver(chromeDriverService, chromeOptions);

                    /*
                    /// ChromeDriver'ı otomatik olarak ayarla
                    //new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig());
                    new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);

                    /// Get the downloaded ChromeDriver version
                    /// burda klasör içindeki mevcut chromedriver.exe nin versiyonunu çekiyor
                    /// bu versiyon no ya görede chrome pathine bakılıyor
                    var driverVersion = new ChromeConfig().GetMatchingBrowserVersion();
                    var driverPath = $"./Chrome/{driverVersion}/X64/";

                    /// komut satırı penceresinin açılmasını engelliyor
                    var chromeDriverService = ChromeDriverService.CreateDefaultService(driverPath);
                    chromeDriverService.HideCommandPromptWindow = true;

                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--disable-gpu"); // Disable GPU acceleration
                    chromeOptions.AddArgument("--no-sandbox");
                    string downloadDirectory = v.EXE_GIBDownloadPath;
                    chromeOptions.AddUserProfilePreference("download.default_directory", downloadDirectory);
                    chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                    chromeOptions.AddUserProfilePreference("directory_upgrade", true);
                    chromeOptions.AddArgument("--remote-debugging-port=9222");

                    driver = new ChromeDriver(chromeDriverService, chromeOptions);
                    */
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //driver.Manage().Window.Maximize();
            var timeouts = driver.Manage().Timeouts();

            timeouts.ImplicitWait = TimeSpan.FromSeconds(SeleniumHelper.ImplicitlyWait);
            timeouts.PageLoad = TimeSpan.FromSeconds(SeleniumHelper.PageLoadTimeout);

            // Suppress the onbeforeunload event first. This prevents the application hanging on a dialog box that does not close.
            ((IJavaScriptExecutor)driver).ExecuteScript("window.onbeforeunload = function(e){};");
            return driver;
        }
    }
}
