using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpic.Alerts;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Interpic.Web.Selenium
{
    public class SeleniumWrapper
    {
        private ChromeDriver driver;

        public void Start()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--start-fullscreen");
            options.AddArgument("headless");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            driver = new ChromeDriver(service, options);

        }

        public void Navigate(string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        public byte[] MakeScreenShot()
        {
            Screenshot screenshot = driver.GetScreenshot();
            return screenshot.AsByteArray;
        }

        public (Size s, Point p) GetElementBounds(string xpath)
        {
            IWebElement element = driver.FindElementByXPath(xpath);
            if (element != null)
            {
                return (element.Size, element.Location);
            }

            return (Size.Empty, Point.Empty);
        }

        public string GetHtml()
        {
            return driver.PageSource;
        }

        public void Close()
        {
            driver.Quit();
        }
    }
}
