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

            
            // https://www17.zippyshare.com/v/j3Ezawz5/file.html
            // https://www101.zippyshare.com/v/SLrsee4W/file.html
            
            
            
            // Console.WriteLine("First Link: " + "https://www17.zippyshare.com/v/j3Ezawz5/file.html");
            // LinkRetriever.DownloadFile("https://www17.zippyshare.com/v/j3Ezawz5/file.html");
  
            // IList<string> links = link_getter("https://gog-games.com/game/mule");
            // foreach(string e in links) {
            //     Console.WriteLine(e);
            //     Zippyshare.GetFileLink(e);
            // }
            
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

//Minimum viable Product TODOS:
//TODO: Rewrite RegExp that extracts the file name, currently won't work on the goodies links
//TODO: integrate link_getter into zippyshare.cs, figure out better naming scheme
//TODO: integrate with Playnite extension
//TODO: Use "Install" button
//TODO: Adapt Xbox browser for this use case
//TODO: Figure out how to get the game imported after it's been downloaded and installed. Possibly taking the shortcut that gog adds to the start menu or something?
//TODO: Figure out metadata importing
//TODO: Extract file on download
//TODO: Run the installer
//TODO: Run DLC Installers

//Other TODOS: 
//TODO: Make completely encapsulated, for easier playnite integration later on
//TODO: Check Disk Space
//TODO: Add way to deal with "goodies"

//Multithreading TODOs:
//TODO: Multithreading in file Download: https://stackoverflow.com/questions/9459225/asynchronous-file-download-with-progress-bar
//Also might want to refer back to java multithreading I did for class in fall
//TODO: Create method to handle the download threads; input: num threads, list of links
//TODO: Look into playnite displaying status bars for downloads
//TODO: Implement better download stats


//Proposed Program Structure: 
// Main recieves gg-games link, queries zippyshare.cs's zippy_link_getter to receieve list of raw links
// List of links sent to method that deals with getting all the file links, returns file links
// goodies links and main links sorted into two lists

