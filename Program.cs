using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace GG_Downloader
{
    public static class Program
    {
       
        public static void Main(string[] args)
        {
            IList<string> links = link_getter("https://gog-games.com/game/mule");
            foreach(string e in links) {
                Console.WriteLine(e);
            }

            // foreach (string i in links)
            // {
            //     Zippyshare.Downloadfile(i);
            // }
            Quit();
        }
        
       
        
        public static IList<string> link_getter(string inputUrl)
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            var options = new ChromeOptions();
            var driver = new ChromeDriver(options);

            //clicking on the captcha download button
            driver.Navigate().GoToUrl(inputUrl);
            driver.FindElement(By.CssSelector(".g-recaptcha")).Click();
            
            //waiting until the links load in, then extracting them from parent elements
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(120));
            wait.Until(e => e.FindElement(By.XPath("/html/body/div[4]/div[2]/div/div[2]/div/div[1]")));
            IList < IWebElement > foundLinks = driver.FindElements(By.TagName("a"));
            
            //filtering only the zippyshare links from the list of all links
            IList<string> foundFilteredLinks = new List<string>();
            foreach(IWebElement e in foundLinks) {
                if (e.GetAttribute(("href")).Contains("zippy"))
                {
                    foundFilteredLinks.Add(e.GetAttribute(("href")));
                }
            }
            driver.Quit();
            return foundFilteredLinks;
        } //input: gog-games url; output: zippy page URLS
        
        public static string file_url_getter(string rawPageLink)
        {
            string pageContent;
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(rawPageLink);
            HttpWebResponse myres = (HttpWebResponse)myReq.GetResponse();

            using (StreamReader sr = new StreamReader(myres.GetResponseStream()))
            {
                pageContent = sr.ReadToEnd();
            }

            if (pageContent.Contains("YourSearchWord"))
            {
                match = Regex.Match(pageContent,
                    @"document\.getElementById\('dlbutton'\)\.href = ""/(pd|d)/(.*)/"" \+ \(([0-9]+) % ([0-9]+) \+ ([0-9]+) % ([0-9]+)\) \+ ""/(.*)"";",
                    RegexOptions.IgnoreCase);
            }

            return "bloop";
        }

        private static void Quit()
        {
            Console.WriteLine("Done\nPress ENTER to Exit");

            ConsoleKeyInfo keyPress = Console.ReadKey(intercept: true);
            while (keyPress.Key != ConsoleKey.Enter)
            {
                keyPress = Console.ReadKey(intercept: true);
            }
        }
    }
}