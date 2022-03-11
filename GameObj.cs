﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GG_Downloader {
    public class GameObj {
        public IList<GgFile> GameFiles { get; set; }
        private const string Basedir = @"%homepath%\Saved Games\GOG_Downloader\"; // todo: make this a field in the settings file
        private string gameName { get; set; } // todo: Actually Fetch the game name from one of the websites
        
        public GameObj(string inputLink) {

            
        }

        internal static IEnumerable<GgFile> GetGameFiles(string inputUrl){ //Takes url, returns list of files
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
            GgFile.ParseFilePath($"{Basedir}{(Regex.IsMatch(Basedir, @"\\$") ? "" : @"\")}{Regex.Match(inputUrl,@"[^\/]+$")}");
            foreach (GgFile ggFile in ggFiles) {
                ggFile.FilePath = $"{Basedir}{(Regex.IsMatch(Basedir, @"\\$") ? "" : @"\")}{Regex.Match(inputUrl,@"[^\/]+$")}";
            }
            return ggFiles;
        }
    }
}