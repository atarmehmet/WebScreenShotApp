using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScreenShotter
{
    public class Shotter : IDisposable
    {
        private readonly IWebDriver _driver;

        public Shotter(string windowSize)
        {
            var options = new ChromeOptions();
            options.AddArguments(new List<string>() {
                            "window-size="+windowSize,
                            "start-maximized",
                            "log-level=3",
                            "headless", });

            var driver = new ChromeDriver(options);
            _driver = driver;
        }

        public Shotter()
        {
        }

        public Screenshot TakeScreenShot(string url)
        {
            url = !url.StartsWith("http") ? "http://" + url : url;
            _driver.Navigate().GoToUrl(url);
            return _driver.TakeScreenshot();
        }

        public string TakeScreenshot(string url)
        {
            return TakeScreenShot(url).AsBase64EncodedString;
        }

        public void TakeScreenshotAsFile(string url, string filePath)
        {
            var fileDirectory = Directory.GetParent(filePath);

            if (!fileDirectory.Exists)
                fileDirectory.Create();

            TakeScreenShot(url).SaveAsFile(filePath, ScreenshotImageFormat.Png); 
        }
        public void SaveAsFile(string filePath, string encodedFile)
        {
            var fileDirectory = Directory.GetParent(filePath);

            if (!fileDirectory.Exists)
                fileDirectory.Create();

            Screenshot ss = new Screenshot(encodedFile.ToString());

            ss.SaveAsFile(filePath, ScreenshotImageFormat.Png);
        }

        public void Dispose()
        {
            _driver?.Quit();
        }
    }
}
