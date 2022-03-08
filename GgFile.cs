using System;
using System.Text;
using System.Text.RegularExpressions;

namespace GG_Downloader
{
    public class GgFile {
        //_sourceUrl: OG URL input | _hostUrl: Raw Zippy URL | _fileDirectUrl: ddl zippy url
        string _fileName, _sourceUrl, _hostUrl, _fileDirectUrl, _filePath;
        private double _fileSize;

        public override string ToString() {
            StringBuilder toStringBuilder = new StringBuilder();
            return toStringBuilder.ToString();
        }

        public GgFile(string fileName, string sourceUrl, string hostUrl, string fileDirectUrl, string filePath, double fileSize) { //full Constructor
            this._fileName = fileName;
            this._sourceUrl = sourceUrl;
            this._hostUrl = hostUrl;
            this._fileDirectUrl = fileDirectUrl;
            this._filePath = filePath;
            this._fileSize = fileSize;
        }
        
        public GgFile(string sourceUrl, string hostUrl) {
            this._sourceUrl = sourceUrl;
            this._hostUrl = hostUrl;
            this._fileDirectUrl = LinkRetriever.ZippyGetFileLink(hostUrl);
            this._fileSize = LinkRetriever.ZippyGetFileSize(hostUrl);
            this._fileName = LinkRetriever.ZippyGetFileName(hostUrl);
        } //this constructor is only missing the filePath;
          //Delaying the filepath until I get some form of settings working

          public void InitDirStructure(string baseDirAsEnvVar) {
              string absBaseDir = "dummy Text, because taking a different approach";
              if (Regex.IsMatch(baseDirAsEnvVar, @"(?<=%).*(?=%)")) {
                  absBaseDir = Environment.GetEnvironmentVariable(
                      Regex.Match(baseDirAsEnvVar, @"(?<=%).*(?=%)").ToString());
              }
              // absBaseDir = (Regex.IsMatch(baseDirAsEnvVar, @"(?<=%).*(?=%)")) ? String.Format("%s")
              // absBaseDir = $"{}"
          }
        
        #region Getters And Setters

        public double FileSize {
            //FileSize is in MB
            get => _fileSize;
            set => _fileSize = value;
        }

        public string FileName {
            get => _fileName;
            set => _fileName = value;
        }

        public string SourceUrl {
            get => _sourceUrl;
            set => _sourceUrl = value;
        }

        public string HostUrl {
            get => _hostUrl;
            set => _hostUrl = value;
        }

        public string FileDirectUrl {
            get => _fileDirectUrl;
            set => _fileDirectUrl = value;
        }

        public string FilePath {
            get => _filePath;
            set => _filePath = value;
        }

        #endregion

    }
}