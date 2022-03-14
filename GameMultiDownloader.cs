using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GG_Downloader {
    public static class GameMultiDownloader {
        public static async void FileAsyncDownloader(GameObj gameObj) {
            Console.WriteLine($"Started Downloading game {gameObj.GameName}");
            GgFile.CreatePathDirs(gameObj.GameDir);
            var webClient = new HttpClient();
            var tasks = gameObj.GameFiles.Select(gameFile => DownloadFile(new Uri(gameFile.FileDirectUrl),
                $"{gameFile.FilePath}{@"\"}{gameFile.FileName}", webClient)).ToList();
            await Task.WhenAll(tasks);
            Console.WriteLine($"Finished Downloading game {gameObj.GameName} to {gameObj.GameDir}");
        }

        private static async Task DownloadFile(Uri fileUri, string destFilePath, HttpClient webClient) {
            Console.WriteLine($"Started Downloading {fileUri} to {destFilePath}");
            using var client = webClient;
            using var inStream = await client.GetStreamAsync(fileUri);
            using var outStream = new FileStream(destFilePath, FileMode.Create);
            await inStream.CopyToAsync(outStream);
            Console.WriteLine($"\tFinished Downloading {fileUri} to {destFilePath}");
        }
    }
}