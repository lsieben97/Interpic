using System.Drawing;
using Interpic.Settings;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
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
        public void Start(SettingsCollection settings)
        {
            switch (Browsertype)
            {
                case BrowserType.Chrome:
                    StartChrome(settings);
                    break;
                case BrowserType.FireFox:
                    StartFireFox(settings);
                    break;
            }
        }

        private void StartFireFox(SettingsCollection settings)
        {
            FirefoxOptions options = new FirefoxOptions();
            if (settings.GetBooleanSetting("debugEnabled"))
            {
                if (!settings.GetBooleanSetting("browserVisible"))
                {
                    options.AddArgument("-headless");
                }
            }
            else
            {
                options.AddArgument("-headless");
            }
            
            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            Driver = new FirefoxDriver(service, options);
            Driver.Manage().Window.Maximize();
        }

        private void StartChrome(SettingsCollection settings)
        {
            ChromeOptions options = new ChromeOptions();
            if (settings.GetBooleanSetting("debugEnabled"))
            {
                if (!settings.GetBooleanSetting("browserVisible"))
                {
                    options.AddArgument("--start-fullscreen");
                    options.AddArgument("headless");
                }
            }
            else
            {
                options.AddArgument("--start-fullscreen");
                options.AddArgument("headless");
            }
            options.AddArgument("--start-maximized");
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
