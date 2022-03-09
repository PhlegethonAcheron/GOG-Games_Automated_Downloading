using System.Text;

namespace GG_Downloader
{
    public class GgFile {
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

        public GgFile(string fileName, string sourceUrl, string hostUrl, string fileDirectUrl, string filePath, double fileSize) { //full Constructor
            this.FileName = fileName;
            this.SourceUrl = sourceUrl;
            this.HostUrl = hostUrl;
            this.FileDirectUrl = fileDirectUrl;
            this.FilePath = filePath;
            this.FileSize = fileSize;
        }
        
        public GgFile(string sourceUrl, string hostUrl) {
            this.SourceUrl = sourceUrl;
            this.HostUrl = hostUrl;
            this.FileDirectUrl = LinkRetriever.ZippyGetFileLink(hostUrl);
            this.FileSize = LinkRetriever.ZippyGetFileSize(hostUrl);
            this.FileName = LinkRetriever.ZippyGetFileName(hostUrl);
        }
    }
}