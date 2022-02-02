namespace GG_Downloader
{
    public class File {
        string _fileName, _sourceUrl, _hostUrl, _fileDirectUrl, _filePath;
        public File(string fileName, string sourceUrl, string hostUrl, string fileDirectUrl, string filePath) {
            this._fileName = fileName;
            this._sourceUrl = sourceUrl;
            this._hostUrl = hostUrl;
            this._fileDirectUrl = fileDirectUrl;
            this._filePath = filePath;
        }
        private File() {
            
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
    }
}