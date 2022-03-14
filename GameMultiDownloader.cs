using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GG_Downloader {
    public static class GameMultiDownloader {
        public static async void FileAsyncDownloader(GameObj gameObj) {
            Console.WriteLine($"Started Downloading game {gameObj.GameName}");
            var tasks = new List<Task>();
            var webClient = new HttpClient();
            foreach (var gameFile in gameObj.GameFiles) {
                var t = DownloadFile(new Uri(gameFile.FileDirectUrl), $"{gameFile.FilePath}{gameFile.FileName}",
                    webClient);
                tasks.Add(t);
            }
            await Task.WhenAll(tasks);
            Console.WriteLine($"Finished Downloading game {gameObj.GameName} to {gameObj.GameDir}");
        }
        
        private static async Task DownloadFile(Uri fileUri, string destFilePath, HttpClient webClient) {
            Console.WriteLine($"Started Downloading {fileUri} to {destFilePath}");
            using var inStream = await webClient.GetStreamAsync(fileUri);
            using var outStream = new FileStream(destFilePath, FileMode.Create);
            await inStream.CopyToAsync(outStream);
            Console.WriteLine($"\tFinished Downloading {fileUri} to {destFilePath}");
        }
    }
}