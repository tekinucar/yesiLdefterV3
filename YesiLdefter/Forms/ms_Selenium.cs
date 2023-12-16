using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Tkn_Events;
using Tkn_Save;
using Tkn_ToolBox;

using YesiLdefter.Selenium.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace YesiLdefter
{
    public partial class ms_Selenium : DevExpress.XtraEditors.XtraForm
    {
        tToolBox t = new tToolBox();

        IWebDriver webDriver;

        string menuName = "MENU_" + "UST/PMS/PMS/SeleniumMebbis";
        string buttonTest = "ButtonTest";

        public ms_Selenium()
        {
            InitializeComponent();

            tEventsForm evf = new tEventsForm();

            this.Load += new System.EventHandler(evf.myForm_Load);
            this.Shown += new System.EventHandler(evf.myForm_Shown);
            this.Shown += new System.EventHandler(this.ms_Selenium_Shown);

            this.KeyPreview = true;

            
        }

        private void ms_Selenium_Shown(object sender, EventArgs e)
        {
            t.Find_Button_AddClick(this, menuName, buttonTest, myNavElementClick);
        }

        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonTest) Test();
                //if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonNormal) TestNormal();
                //if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonChild) TestChild();
            }
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                //if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonInsertPaketOlustur) InsertPaketOlustur();
                //if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonPaketiGonder) PaketiGonder();
            }
        }

        void Test()
        {
            //SeleniumHelper.TargetURL = "https://mebbis.meb.gov.tr/";
            SeleniumHelper.TargetURL = "https://ehliyethakkinda.com/";
            SeleniumHelper.ResetDriver();
            var seleniumWebDriver = SeleniumHelper.WebDriver;

            //seleniumWebDriver.

            IWebElement esinav = seleniumWebDriver.FindElement(By.LinkText("e-Sınav Deneme"));

            esinav.Click();

            IWebElement ornek = seleniumWebDriver.FindElement(By.Id("IPButton[91]"));

            ornek.Click();


            var url = seleniumWebDriver.Url;



            /* Direk 
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = false;
            webDriver = new ChromeDriver(chromeDriverService, new ChromeOptions());
            webDriver.Navigate().GoToUrl("https://mebbis.meb.gov.tr/");
            */

        }


    }
}
