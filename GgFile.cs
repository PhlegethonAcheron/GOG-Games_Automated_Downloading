namespace GG_Downloader
{
    public class GgFile {
        //_sourceUrl: gog-games.com url     _hostUrl: Raw Zippy URL     _fileDirectUrl: ddl zippy url
        string _fileName, _sourceUrl, _hostUrl, _fileDirectUrl, _filePath;
        private double _fileSize;

        public GgFile(string fileName, string sourceUrl, string hostUrl, string fileDirectUrl, string filePath, double fileSize) { //full Constructor
            this._fileName = fileName;
            this._sourceUrl = sourceUrl;
            this._hostUrl = hostUrl;
            this._fileDirectUrl = fileDirectUrl;
            this._filePath = filePath;
            this._fileSize = fileSize;
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