using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GG_Downloader
{
    public static class Program {
        private const string Basedir = @"%homepath%\Saved Games\Gog_Downloader\"; // todo: make this a field in the settings file

        public static void Main()
        {
            Console.WriteLine("Enter gog URL:\n" + "https://www.gog.com/ru/game/mule");
            string urlInput = "https://www.gog.com/ru/game/mule";
            // urlInput = Console.ReadLine();
            var ggFileList = GetFileInfoList(urlInput);
            ggFileList.ToList().ForEach(Console.WriteLine);
            Quit();
        }
        private static IEnumerable<GgFile> GetFileInfoList(string inputUrl){ //Takes url, returns list of files
            LinkRetriever.LinkType linkClass = LinkRetriever.ValidateInputLink(inputUrl);
            string ggUrl; // URL for the actual source of the game; if gog.com, converted to gog-games.com link
            switch (linkClass) {
                case LinkRetriever.LinkType.InvalidLinkFormat:
                    throw new ArgumentException("Given input is not a valid URL.", inputUrl);
                case LinkRetriever.LinkType.InvalidResource:
                    throw new ArgumentException("Given input URL is not a valid link to a game.", inputUrl);
                case LinkRetriever.LinkType.InvalidWebsite:
                    throw new ArgumentException("Given input is not a valid website. Valid websites are: \"gog-games.com\" and \"gog.com\".", inputUrl);
                case LinkRetriever.LinkType.Gog:
                    ggUrl = LinkRetriever.GogLinkConversion(inputUrl);
                    break;
                case LinkRetriever.LinkType.GogGames:
                    ggUrl = inputUrl;
                    break;
                case LinkRetriever.LinkType.GogLocalized:
                    ggUrl = LinkRetriever.GogLinkConversion(inputUrl);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            } //dealing with the different potential URL inputs
            
            var zippyLinks = LinkRetriever.GogGetZippyLink(ggUrl);
            IList<GgFile> ggFiles = zippyLinks.Select(zippyUrl => new GgFile(inputUrl, zippyUrl)).ToList();
            ParseFilePath($"{Basedir}{(Regex.IsMatch(Basedir, @"\\$") ? "" : @"\")}{Regex.Match(inputUrl,@"[^\/]+$")}");
            foreach (GgFile ggFile in ggFiles) {
                ggFile.FilePath = $"{Basedir}{(Regex.IsMatch(Basedir, @"\\$") ? "" : @"\")}{Regex.Match(inputUrl,@"[^\/]+$")}";
            }
            return ggFiles;
        }
        private static void Quit()
        {
            Console.WriteLine("Done\nPress ENTER to Exit");

            ConsoleKeyInfo keyPress = Console.ReadKey(true);
            while (keyPress.Key != ConsoleKey.Enter) keyPress = Console.ReadKey(intercept: true);
        }

        #region Creating File Path

        /// <summary>
        /// Method <c>ParseFilePath()</c> takes an input path as string, throws exception if the drive specified does not exist.
        /// Creates all directories required to make the full path exist.
        /// Can take absolute paths or paths prefixed by windows environment variables as argument.
        /// Throw exception if: Drive specified does not exist, or if it is not a valid path format.
        /// </summary>
        private static void ParseFilePath(string inputPath) {
            // Check that the input is actually a path
            if (!Regex.IsMatch (inputPath, @"(([A-Z]\:)|(%[A-z]+%))(\\[A-z_\-\s0-9\.\\]+)+")) {
                throw new ArgumentException("Input is not a valid path", inputPath);
            }

            var absPath = Regex.IsMatch(inputPath, "(?<=%).*(?=%)")
                ? Regex.Replace(inputPath, @"%.*%",
                    Environment.GetEnvironmentVariable(Regex.Match(inputPath, @"(?<=%).*(?=%)").ToString()) ??
                    throw new ArgumentException(
                        $"{Regex.Match(inputPath, @"(?<=%).*(?=%)")} is not a valid environment variable."))
                : inputPath;

            string[] splitPath = absPath.Split('\\');
            StringBuilder pathBuilder = new StringBuilder();
            foreach (string pathSubString in splitPath) {
                pathBuilder.Append(pathSubString).Append(@"\");
                if (Directory.Exists(pathBuilder.ToString())) continue;
                if (!pathSubString.Contains(":")) {
                    Directory.CreateDirectory(pathBuilder.ToString());
                }
                else {
                    throw new ArgumentException("Specified Drive letter does not exist", pathBuilder.ToString());
                }
            }
        }

        #endregion
        
    }
}