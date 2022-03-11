using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GG_Downloader {
    public class GgFile {
        public GgFile(string fileName, string sourceUrl, string hostUrl, string fileDirectUrl, string filePath,
            double fileSize) { //full Constructor
            FileName = fileName;
            SourceUrl = sourceUrl;
            HostUrl = hostUrl;
            FileDirectUrl = fileDirectUrl;
            FilePath = filePath;
            FileSize = fileSize;
        }

        public GgFile(string sourceUrl, string hostUrl) {
            SourceUrl = sourceUrl;
            HostUrl = hostUrl;
            FileDirectUrl = LinkRetriever.ZippyGetFileLink(hostUrl);
            FileSize = LinkRetriever.ZippyGetFileSize(hostUrl);
            FileName = LinkRetriever.ZippyGetFileName(hostUrl);
        }

        //_sourceUrl: OG URL input | _hostUrl: Raw Zippy URL | _fileDirectUrl: ddl zippy url
        private string FileName { get; }
        private string SourceUrl { get; }
        private string HostUrl { get; }
        private string FileDirectUrl { get; }
        protected internal string FilePath { get; set; }
        private double FileSize { get; }


        public override string ToString() {
            return $"File Name: {FileName}\tFile Size: {FileSize}MB\nSource URL: {SourceUrl}\tHost URL: {HostUrl}" +
                   $"\tDirect File URL: {FileDirectUrl}\nOutput File Path: {this.FilePath}\n";
        }
        
        
        #region Creating File Path

        /// <summary>
        /// Method <c>ParseFilePath()</c> takes an input path as string, throws exception if the drive specified does not exist.
        /// Creates all directories required to make the full path exist.
        /// Can take absolute paths or paths prefixed by windows environment variables as argument.
        /// Throw exception if: Drive specified does not exist, or if it is not a valid path format.
        /// </summary>
        public static void ParseFilePath(string inputPath) {
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