using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace GG_Downloader {
    public static class LinkRetriever {
        public enum LinkType {
            Gog,
            GogLocalized,
            GogGames,
            InvalidResource, //used when it isn't a .../game/ URL
            InvalidWebsite, //Not one of the accepted link formats
            InvalidLinkFormat
        } //used to return types of links
        
        public static string ZippyExtractDLlink(string website) {
            // Console.WriteLine(website);
            var doc = new HtmlDocument();
            doc.LoadHtml(website);
            
            //gets the parent div of the bit I want
            string lrbox = doc.GetElementbyId("lrbox").InnerHtml;
            
            //gets the script that constructs the link
            string math = Regex.Match(lrbox, "<script type=\"text/javascript\">[\\S\\s]+?</script>", RegexOptions.Multiline).ToString();
            
            //the JS gets the properties of an element, so i'm extracting that element and properties
            string annoyingElementName = Regex.Match(math, @"(?<=var [a-z] = document.getElementById\(')[a-z]+(?='\)\.get)").ToString();
            string annoyingElementAttr = Regex.Match(math, @"(?<=var [a-z] = document.getElementById\('[a-z]+'\)\.getAttribute\(')[a-z]+").ToString();
            var annoyingValue = doc.GetElementbyId(annoyingElementName).Attributes[annoyingElementAttr].Value;

            #region rewriting the javascript to make it a standalone function that I can execute
            
            //inserting the annoying value into the JS code
            string mathFixed = Regex.Replace(math, @"(?<=var [a-z] = )document\.getElementBy.+(?=;)", annoyingValue);
            
            //rewriting it to make it ane executable function
            mathFixed = Regex.Replace(mathFixed, @"if \(document[\S\s]+?\}", "return linkbits;");
            mathFixed = Regex.Replace(mathFixed, @"document\.getElementById\('dlbutton'\)\.href", "linkbits");            mathFixed = Regex.Replace(mathFixed, @"document\.getElementById\('dlbutton'\)\.href", "linkbits");
            mathFixed = Regex.Replace(mathFixed, "<script type=\"text/javascript\">", "var e = function(){");
            mathFixed = Regex.Replace(mathFixed, "</script>", "}; e();");

            #endregion         
            
            var engine = new Jurassic.ScriptEngine();
            var fuckingfinallyitactuallyworks = engine.Evaluate(mathFixed);
            
            return fuckingfinallyitactuallyworks.ToString();
        }

        public static string ZippyGetFileLink(string rawZippyUrl) {
            #region LinkValidityCheck
            //checking if it's a valid zippyshare url (matches "https" through"/v/someIdentifier")
            var m = Regex.Match(rawZippyUrl, @"https?://((?:[\w\-]+))\.*zippyshare\.com/\w/(\w+)",
                RegexOptions.IgnoreCase);
            if (!m.Success) {
                return ("Invalid zippyshare link!");
            }
            var server = m.Groups[1].Captures[0].Value;
            #endregion
            
            //gets the Matched string that contains all the info needed to assemble the link
            string website = new WebClient().DownloadString(rawZippyUrl);
            
            //Verifying that the file exists
            #region FileExistsCheck

            //Verifying that the file exists
            var regex = "File does not exist on this server";
            var match1 = Regex.Match(website, regex, RegexOptions.IgnoreCase);
            regex = "File has expired and does not exist anymore on this server";
            var match2 = Regex.Match(website, regex, RegexOptions.IgnoreCase);
            if (match1.Success || match2.Success) {
                // Console.WriteLine("File doesn't exist!");
                return ("File doesn't exist!");
            }

            #endregion
            
            //extracting the line of JS that is used to generate the file link
            var constructedFilePath = $"https://{server}.zippyshare.com{ZippyExtractDLlink(website)}";
            return constructedFilePath;
        }

        public static double ZippyGetFileSize(string rawZippyUrl) {
            // Console.WriteLine(rawZippyUrl);
            string website = new WebClient().DownloadString(rawZippyUrl);
            // Console.WriteLine(website);
            // \d+\.\d+ ((MB)|(KB))
            string sizeAsString = Regex.Match(website, @"(\d+|(\d+\.\d+)) ((MB)|(KB))").ToString();
            double sizeInMBytes = Double.Parse(Regex.Match(sizeAsString, @"(\d+|(\d+\.\d+))").ToString());
            return sizeInMBytes;
        }

        public static string ZippyGetFileName(string fileLink) {
            //Takes output of ZippyGetFileLink, returns unescaped filename
            return Uri.UnescapeDataString(Regex.Match(fileLink, @"[^/]+$").ToString());
        }

        public static IList<string> GogGetZippyLink(string inputUrl) {
            new DriverManager().SetUpDriver(new ChromeConfig());
            var options = new ChromeOptions();
            var driver = new ChromeDriver(options);
            Console.WriteLine("Started WebDriver");
            //clicking on the captcha download button
            Console.WriteLine(inputUrl);
            driver.Navigate().GoToUrl(inputUrl);

            driver.FindElement(By.CssSelector(".g-recaptcha")).Click();

            //waiting until the links load in, then extracting them from parent elements
            
            try {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(120));
                wait.Until(e => e.FindElement(By.XPath("/html/body/div[4]/div[2]/div/div[2]/div[1]/div[3]/div[3]/a[1]")));
            }
            catch (FormatException e) {
                Console.WriteLine($"{e}\nWaiting for the element to exist failed, falling back to iteratively checking for Zippy Links.");
                bool matcher = false;
                while (matcher == false) {
                    IList<IWebElement> linkChecker = driver.FindElements(By.TagName("a"));
                    var linkCheckerLink = linkChecker.FirstOrDefault(x => x.ToString().Contains("zippy"));
                    if (linkCheckerLink != null) {
                        matcher = true;
                    }
                }
                throw;
            }
            
            IList<IWebElement> foundLinks = driver.FindElements(By.TagName("a"));

            //filtering only the zippyshare links from the list of all links
            IList<string> foundFilteredLinks = new List<string>();
            foreach (IWebElement e in foundLinks) {
                if (e.GetAttribute(("href")).Contains("zippy")) {
                    Console.WriteLine(e.GetAttribute(("href")));
                    foundFilteredLinks.Add(e.GetAttribute(("href")));
                }
            }

            driver.Quit();
            return foundFilteredLinks;
        } //input: gog-games url; output: zippy page URLS

        private static bool IsGgPageFound(string ggUrl) {
            Console.WriteLine(ggUrl);
            string website = new WebClient().DownloadString(ggUrl);
            
            return website.Contains("404 - Good Old Downloads");
        } //checks if the gg page exists

        public static LinkType ValidateInputLink(string inputLink) {
            if (!Regex.IsMatch(inputLink, @"^https?://")) {
                inputLink = $"https://{inputLink}";
            }
            LinkType currentValidityState;
            if (!Regex.IsMatch(inputLink, @"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$")) {
                
                currentValidityState = LinkType.InvalidLinkFormat;
            }
            else if (!Regex.IsMatch(inputLink, "(gog-games|gog)")) {
                currentValidityState = LinkType.InvalidWebsite;
            }
            else if (Regex.IsMatch(inputLink, @"https:\/\/www\.gog\.com\/[a-z][a-z]\/game\/\w+")) {
                currentValidityState = LinkType.GogLocalized;
            } 
            else if (Regex.IsMatch(inputLink, @"https:\/\/(www\.)?(gog-games|gog)\.com\/game\/\w+")) {
                currentValidityState = inputLink.Contains("gog-games") ? LinkType.GogGames : LinkType.Gog;
            }
            else {
                currentValidityState = LinkType.InvalidResource;
            }
            
            return currentValidityState;
        }

        public static string GogLinkConversion(string gogLink) {
            string gogGamesLink = Regex.IsMatch(gogLink, @"https:\/\/www\.gog\.com(\/[a-z][a-z])?\/game\/\w+")
                ? Regex.Replace(gogLink, @"https:\/\/www\.gog\.com(\/[a-z][a-z])?\/game\/",
                    @"https://www.gog-games.com/game/")
                : gogLink.Replace("https://gog.com", "https://gog-games.com");
            return IsGgPageFound(gogGamesLink)
                ? throw new ServerException("Gog-games page returned 404. Requested URL: " + gogGamesLink)
                : gogGamesLink;
        }
    }
}