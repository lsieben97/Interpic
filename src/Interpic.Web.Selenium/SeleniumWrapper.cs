using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpic.Alerts;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Opera;
using OpenQA.Selenium.Remote;

namespace Interpic.Web.Selenium
{
    public class SeleniumWrapper
    {
        public RemoteWebDriver Driver { get; set; }
        public BrowserType Browsertype { get; set; }

        public SeleniumWrapper(BrowserType type)
        {
            this.Browsertype = type;
        }

        public enum BrowserType
        {
            Chrome,
            FireFox
        }
        public void Start()
        {
            switch (Browsertype)
            {
                case BrowserType.Chrome:
                    StartChrome();
                    break;
                case BrowserType.FireFox:
                    StartFireFox();
                    break;
            }
        }

        private void StartFireFox()
        {
            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("-headless");
            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            Driver = new FirefoxDriver(service, options);
            Driver.Manage().Window.Maximize();
        }

        private void StartChrome()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--start-fullscreen");
            options.AddArgument("headless");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            Driver = new ChromeDriver(service, options);
        }

        public void Navigate(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        public byte[] MakeScreenShot()
        {
            Screenshot screenshot = Driver.GetScreenshot();
            return screenshot.AsByteArray;
        }

        public (Size s, Point p) GetElementBounds(string xpath)
        {
            IWebElement element = Driver.FindElementByXPath(xpath);
            if (element != null)
            {
                return (element.Size, element.Location);
            }

            return (Size.Empty, Point.Empty);
        }

        public string GetHtml()
        {
            return Driver.PageSource;
        }

        public void Close()
        {
            Driver.Quit();
        }
    }
}
