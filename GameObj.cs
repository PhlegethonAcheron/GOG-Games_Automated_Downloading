using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace GG_Downloader {
    public class GameObj {
        public IList<GgFile> GameFiles { get; set; }
        private const string Basedir = @"%homepath%\Saved Games\GOG_Downloader\"; // todo: make this a field in the settings file
        public string GameName { get; set; } // todo: Actually Fetch the game name from one of the websites
        public string GameDir { get; set; }

        public GameObj(string inputUrl) {
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
            GameName = Regex.Match(inputUrl, @"[^\/]+$").ToString();
            GameDir = GgFile.ParseFilePath($"{Basedir}{(Regex.IsMatch(Basedir, @"\\$") ? "" : @"\")}{GameName}");
            GameFiles = LinkRetriever.GogGetZippyLink(ggUrl)
                .Select(zippyUrl => new GgFile(inputUrl, zippyUrl, GameDir)).ToList();
            foreach (GgFile gameFile in GameFiles) {
                gameFile.FilePath = $"{gameFile.FilePath}{@"\"}{(gameFile.FileName.Contains("extra") ? "extras" : "game")}{@"\"}{gameFile.FileName}";
            }
        }

        public void StartDownloads() {
            GgFile.CreatePathDirs($"{this.GameDir}{@"\"}extras");
            GgFile.CreatePathDirs($"{this.GameDir}{@"\"}game");
            GameMultiDownloader.FileAsyncDownloader(this).Wait();
        }


        public static void ExtractFilesFromDirectory(string inputDirectoryPath) {
            var fileNames = Directory.GetFiles(inputDirectoryPath);
            foreach (var fileName in fileNames) {
                if (Regex.IsMatch(fileName, @"((part(0+)1\.rar))") || !fileName.Contains("part")) {
                    ExtractRar(fileName);
                }
            }
        }

        private static void ExtractRar(string filePath) {
            var homeDir = Regex.Replace(Regex.Replace(filePath, @"[^\\]+$", ""), @"\\$", "");
            using var archive = RarArchive.Open(filePath);
            foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
            {
                entry.WriteToDirectory(homeDir, new ExtractionOptions()
                {
                    ExtractFullPath = true,
                    Overwrite = true
                });
            }
        }

        public override string ToString() {
            var gameFileInfoBuilder = new StringBuilder();
            foreach (var gameFile in GameFiles) {
                gameFileInfoBuilder.Append(gameFile);
            }
            return $"Game Name: {GameName}\t Game Location: {Basedir}\n Game File Information: {gameFileInfoBuilder}";
        }
    }
}