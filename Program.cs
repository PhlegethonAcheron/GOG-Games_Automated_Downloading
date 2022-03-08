using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GG_Downloader
{
    public static class Program {
        private static string _basedir = @"%homepath%\Saved Games\Gog_Downloader\"; // todo: make this a field in the settings file
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

        
        public static void GetFileInfoList(string inputUrl){ //Takes url, returns list of files
            LinkRetriever.LinkType linkClass = LinkRetriever.ValidateInputLink(inputUrl);
            // ReSharper disable once NotAccessedVariable
            string ggUrl; // URL for the actual source of the game; if gog.com, converted to gog-games.com link
            switch (linkClass) {
                case LinkRetriever.LinkType.InvalidLinkFormat:
                    throw new ArgumentException("Given input is not a valid URL.", inputUrl);
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
            var zippyLinks = LinkRetriever.GogGetZippyLink("https://gog-games.com/game/mule");
            IList<GgFile> ggFiles = zippyLinks.Select(zippyUrl => new GgFile(inputUrl, zippyUrl)).ToList();
            // foreach (GgFile ggFile in ggFiles) {
            // todo: set base dir for each file to be downloaded     
            // } 
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

        /// <summary>
        /// Method <c>ParseFilePath()</c> takes an input path as string, throws exception if the drive specified does not exist.
        /// Creates all directories required to make the full path exist.
        /// Can take absolute paths or paths prefixed by windows environment variables as argument.
        /// Throw exception if: Drive specified does not exist, or if it is not a valid path format.
        /// </summary>
        private static void ParseFilePath(string inputPath) {
            // Check that the input is actually a path
            if (!Regex.IsMatch (inputPath, @"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$")) {
                throw new ArgumentException("Input is not a valid path", inputPath);
            }

            string absPath = Regex.IsMatch(inputPath, @"(?<=%).*(?=%)")
                ? Regex.Replace("%.*%",
                    Environment.GetEnvironmentVariable(Regex.Match(inputPath, @"(?<=%).*(?=%)").ToString())
                    ?? string.Empty, inputPath)
                : inputPath;
            
            string[] splitPath = absPath.Split('\\');
            StringBuilder pathBuilder = new StringBuilder();
            foreach (string pathSubString in splitPath) {
                pathBuilder.Append(pathSubString).Append(@"\");
                if (!Directory.Exists(pathBuilder.ToString())) {
                    if (pathSubString.Contains(":")) {
                        throw new ArgumentException("Specified Drive letter does not exist", pathBuilder.ToString());
                    }
                    else {
                        Directory.CreateDirectory(pathBuilder.ToString());
                    }
                    
                }
            }
        }
    }
}


// todo: make list of GgFiles given a GG/GOG URL

// todo: consider how I might implement the downloading of multiple games; potentially creating an object out of a game?
// todo: make folder and downloads be at %homepath%\Saved Games\Gog_Downloader\<GameName> by default
// todo: dirs for extras, downloads(temp), game
// todo: settings file, cli args
// todo: download manager