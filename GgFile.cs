using System.Text;

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
            StringBuilder toStringBuilder = new StringBuilder();
            return toStringBuilder.ToString();
        }
    }
}