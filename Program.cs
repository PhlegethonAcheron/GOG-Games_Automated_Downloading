using System;
using System.Collections.Generic;

namespace GG_Downloader
{
    public static class Program
    {
        public static void Main(string[] args)
        {
          // todo: CLI Input  
            IList<string> zippyLinks = LinkRetriever.GogGetZippyLink("https://gog-games.com/game/mule");
            foreach(string e in zippyLinks) {
                // Console.WriteLine("Link: " + LinkRetriever.ZippyGetFileLink(e) + "\t Filename: " + LinkRetriever.ZippyGetFileName(e));
                // Console.WriteLine("" + LinkRetriever.ZippyGetFileName(e));
                Console.WriteLine("Link: " + LinkRetriever.ZippyGetFileLink(e));
                Console.WriteLine("Filename: " + LinkRetriever.ZippyGetFileName(LinkRetriever.ZippyGetFileLink(e)) +
                                  "\tSize: " + LinkRetriever.ZippyGetFileSize(e) + " MB");
                
            }
            
            
            Quit();
        }
        
        // private static IList<GgFile> 

        public static void GetFileInfoList(string inputUrl){ //Takes url, returns list of files
            LinkRetriever.LinkType linkClass = LinkRetriever.ValidateInputLink(inputUrl);
            // ReSharper disable once NotAccessedVariable
            string ggUrl; // URL for the actual source of the game; if gog.com, converted to gog-games.com link
            switch (linkClass) {
                case LinkRetriever.LinkType.InvalidResource:
                    throw new ArgumentException("Given input URL is not a valid link to a game.", inputUrl);
                case LinkRetriever.LinkType.InvalidWebsite:
                    throw new ArgumentException("Given input is not a valid website. Valid websites are: \"gog-games.com\" and \"gog.com\".", inputUrl);
                case LinkRetriever.LinkType.Gog:
                    // ReSharper disable once RedundantAssignment
                    ggUrl = LinkRetriever.GogLinkConversion(inputUrl);
                    break;
                case LinkRetriever.LinkType.GogGames:
                    // ReSharper disable once RedundantAssignment
                    ggUrl = inputUrl;
                    break;
            } //dealing with the different potential URL inputs
            // todo: continue with the creation of the file objects
            // todo: consider how I might implement the downloading of multiple games; potentially creating an object out of a game?
            
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