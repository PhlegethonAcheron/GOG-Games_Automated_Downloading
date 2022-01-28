using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace GG_Downloader
{
    public class WebClientEx : WebClient
    {
        public WebClientEx()
        {
        }
    
        public WebClientEx(CookieContainer container)
        {
            this.CookieContainer = container;
        }
    
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();
    
        protected override WebRequest GetWebRequest(Uri address)
        {
            var r = base.GetWebRequest(address);
            var request = r as HttpWebRequest;
            if (request != null) request.CookieContainer = CookieContainer;
            return r;
        }
    
        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            var response = base.GetWebResponse(request, result);
            ReadCookies(response);
            return response;
        }
    
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = base.GetWebResponse(request);
            ReadCookies(response);
            return response;
        }
    
        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null)
            {
                var cookies = response.Cookies;
                CookieContainer.Add(cookies);
            }
        }
    }

    public class Zippyshare
    {

        public static void PrintRegexMatches(Match m)
        {
            for (int i = 1; i <= 2; i++)
            {
                Group g = m.Groups[i];
                Console.WriteLine("Group"+i+"='" + g + "'");
                CaptureCollection cc = g.Captures;
                for (int j = 0; j < cc.Count; j++)
                {
                    Capture c = cc[j];
                    Console.WriteLine("Capture"+j+"='" + c + "', Position="+c.Index);
                }
            }
        }

        public static void GetFileLink(string fileUrl){
            
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
                
                string fileLink = ParseFileLink(website, serverId);
                string fileName = Regex.Replace(Regex.Replace(Regex.Match(fileLink, "game(.*)\\.rar").ToString(), "%28", "("), "%29", ")");
                var outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    (Regex.Match(fileName, "(?<=-)(.*)(?=-)").ToString()) + "\\");
                
                Directory.CreateDirectory(outputDir);
                Console.WriteLine("Starting download of " + fileLink);
                client.DownloadFile(fileLink,  outputDir + fileName);
                Console.WriteLine("Completed Download of " + fileLink + "\nFile can be found in " + outputDir);
            }
        }

        private static string ParseFileLink(string website, string server)
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

        public static void Downloadfile(string fileUrl) {
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
            var server = m.Groups[1].Captures[0].Value;
            var fid = m.Groups[2].Captures[0].Value;
            Console.WriteLine($"args1: {"Server: "} args2: {server}" +
                              $"args3: {"\nFileID: "} args4: {fid}");
            
            using (var client = new WebClientEx())
            {
                var website = client.DownloadString(fileUrl);
                
                var regex = "File does not exist on this server";
                var match = Regex.Match(website, regex, RegexOptions.IgnoreCase);
                regex = "File has expired and does not exist anymore on this server";
                var match2 = Regex.Match(website, regex, RegexOptions.IgnoreCase);
                if (match.Success || match2.Success)
                {
                    Console.WriteLine("File doesn't exist!");
                    return;
                }

                match = Regex.Match(website,
                    @"document\.getElementById\('dlbutton'\)\.href = ""/(pd|d)/(.*)/"" \+ \(([0-9]+) % ([0-9]+) \+ ([0-9]+) % ([0-9]+)\) \+ ""/(.*)"";",
                    RegexOptions.IgnoreCase);
                if (!match.Success)
                    return;
                if (match.Groups.Count != 6)
                    return;
                var fileId = match.Groups[1].Captures[0].Value;
                var a = int.Parse(match.Groups[2].Captures[0].Value);
                var b = int.Parse(match.Groups[3].Captures[0].Value);
                var c = int.Parse(match.Groups[4].Captures[0].Value);
                var d = int.Parse(match.Groups[5].Captures[0].Value);
                var fileName = match.Groups[6].Captures[0].Value;
                var num = a % b + c % d;

                Console.WriteLine("Downloading: " + "https://" + server + ".zippyshare.com/d/" + fid + "/" + num + "/" +
                                  fileName);
                client.DownloadFile("https://" + server + ".zippyshare.com/d/" + fileId + "/" + num + "/" + fileName,
                    fileName);
            }
        }
    }
}