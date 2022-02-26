using System;
using System.Text.RegularExpressions;

namespace GG_Downloader
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(Uri.UnescapeDataString(Regex.Match("https://www17.zippyshare.com/d/j3Ezawz5/32364/game-mule-%2845107%29.rar", 
                    "(?<=(\\d\\d\\d\\d\\d\\/))\\S+").ToString()));
            
            
            // IList<string> zippyLinks = LinkRetriever.GogGetZippyLink("https://gog-games.com/game/mule");
            // foreach(string e in zippyLinks) {
            //     // Console.WriteLine("Link: " + LinkRetriever.ZippyGetFileLink(e) + "\t Filename: " + LinkRetriever.ZippyGetFileName(e));
            //     // Console.WriteLine("" + LinkRetriever.ZippyGetFileName(e));
            //     Console.WriteLine("Link: " + LinkRetriever.ZippyGetFileLink(e));
            //     Console.WriteLine("\t Filename: " + LinkRetriever.ZippyGetFileName(e));
            //     
            // }
            //
            // Quit();
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