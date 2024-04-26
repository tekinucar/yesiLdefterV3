using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
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

                    driver = new ChromeDriver(chromeDriverService, new ChromeOptions());

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
