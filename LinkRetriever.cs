using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace GG_Downloader
{
    public static class LinkRetriever
    {
        
        public static void DownloadFile(string fileUrl){
                
            #region validInputVerification
            //checking if it's a valid zippyshare url (matches "https" through"/v/someIdentifier")
            var m = Regex.Match(fileUrl, @"https?://((?:[\w\-]+))\.*zippyshare\.com/\w/(\w+)", RegexOptions.IgnoreCase);
            if (!m.Success)
            {
                Console.WriteLine("Invalid zippyshare link!");
                return;
            }

            #endregion 

            // I believe this is breaking the thing up into the first bit and the unique ID
            var serverId = m.Groups[1].Captures[0].Value;

            //Gets the Matched string that contains all the info needed to assemble the link
            using (var client = new System.Net.WebClient())
            {
                var website = client.DownloadString(fileUrl);
                
                string fileLink = ZippyGetFileLink(website, serverId);
                string fileName = Regex.Replace(Regex.Replace(Regex.Match(fileLink, "game(.*)\\.rar").ToString(), "%28", "("), "%29", ")");
                var outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    (Regex.Match(fileName, "(?<=-)(.*)(?=-)")) + "\\");
                
                Directory.CreateDirectory(outputDir);
                Console.WriteLine("Starting download of " + fileLink);
                client.DownloadFile(fileLink,  outputDir + fileName);
                Console.WriteLine("Completed Download of " + fileLink + "\nFile can be found in " + outputDir);
            }
        }
        private static string ZippyGetFileLinkComplete(string rawZippyUrl) {
            //checking if it's a valid zippyshare url (matches "https" through"/v/someIdentifier")
            var m = Regex.Match(rawZippyUrl, @"https?://((?:[\w\-]+))\.*zippyshare\.com/\w/(\w+)", RegexOptions.IgnoreCase);
            if (!m.Success)
            {
                return ("Invalid zippyshare link!");
            }

            // I believe this is breaking the thing up into the first bit and the unique ID
            var server = m.Groups[1].Captures[0].Value;
            
            //Gets the Matched string that contains all the info needed to assemble the link
            string website = new System.Net.WebClient().DownloadString(rawZippyUrl);

            //Verifying that the file exists
            var regex = "File does not exist on this server";
            var match1 = Regex.Match(website, regex, RegexOptions.IgnoreCase);
            regex = "File has expired and does not exist anymore on this server";
            var match2 = Regex.Match(website, regex, RegexOptions.IgnoreCase);
            if (match1.Success || match2.Success)
            {
                // Console.WriteLine("File doesn't exist!");
                return ("File doesn't exist!");
            }

            // extracting the line of JS that is used to generate the file link 
            var pattern_elements =
                @"document\.getElementById\('dlbutton'\)\.href = ""/(pd|d)/(.*)/"" \+ \(([0-9]+) % ([0-9]+) \+ ([0-9]+) % ([0-9]+)\) \+ ""/(.*)"";";
            string matchedString = Regex.Match(website, pattern_elements, RegexOptions.IgnoreCase).ToString();

            // Console.WriteLine(matchedString);

            //Parsing the numbers out of the string
            List<int> nums = new List<int>();
            foreach (Match match in Regex.Matches((Regex.Match(matchedString, "(?<=( \\())(.*)(?=(\\)))").ToString()), "(\\d+)")){
                nums.Add(int.Parse(match.ToString()));
            }
            
            var fileId = Regex.Match(matchedString, "(?<=( \")).*(?=(\" ))").ToString();
            
            var fileName = (Regex.Match(matchedString, "(?<=(\\+ \")).*(?=(\";))").ToString());
          
            var fileNumber = nums[0] % nums[1] + nums[2] % nums[3];
            return ("https://" + server + ".zippyshare.com" + fileId + fileNumber + fileName);
        }
        private static bool IsValidZippyLink(string fileUrl) {
            var m = Regex.Match(fileUrl, @"https?://((?:[\w\-]+))\.*zippyshare\.com/\w/(\w+)", RegexOptions.IgnoreCase);
            if (!m.Success)
            {
                Console.WriteLine("Invalid zippyshare link!");
                return false;
            }
            else {
                return true;
            }
        }
        private static string ZippyGetFileLink(string website, string server)
        {
            //Verifying that the file exists
            var regex = "File does not exist on this server";
            var match1 = Regex.Match(website, regex, RegexOptions.IgnoreCase);
            regex = "File has expired and does not exist anymore on this server";
            var match2 = Regex.Match(website, regex, RegexOptions.IgnoreCase);
            if (match1.Success || match2.Success)
            {
                // Console.WriteLine("File doesn't exist!");
                return ("File doesn't exist!");
            }

            // extracting the line of JS that is used to generate the file link 
            var pattern_elements =
                @"document\.getElementById\('dlbutton'\)\.href = ""/(pd|d)/(.*)/"" \+ \(([0-9]+) % ([0-9]+) \+ ([0-9]+) % ([0-9]+)\) \+ ""/(.*)"";";
            string matchedString = Regex.Match(website, pattern_elements, RegexOptions.IgnoreCase).ToString();

            // Console.WriteLine(matchedString);

            //Parsing the numbers out of the string
            List<int> nums = new List<int>();
            foreach (Match match in Regex.Matches((Regex.Match(matchedString, "(?<=( \\())(.*)(?=(\\)))").ToString()), "(\\d+)")){
                nums.Add(int.Parse(match.ToString()));
            }
            
            var fileId = Regex.Match(matchedString, "(?<=( \")).*(?=(\" ))").ToString();
            
            var fileName = (Regex.Match(matchedString, "(?<=(\\+ \")).*(?=(\";))").ToString());
          
            var fileNumber = nums[0] % nums[1] + nums[2] % nums[3];
            return ("https://" + server + ".zippyshare.com" + fileId + fileNumber + fileName);
        }
        public static IList<string> GogGetZippyLink(string inputUrl)
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
    }
}