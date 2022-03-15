using System;
using System.Linq;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace GG_Downloader
{
    public static class Program {
        public static void Main()
        {
            var homeDir = @"C:\Users\Townsend\Saved Games\TestFiles";
            using (var archive = RarArchive.Open($"{homeDir}{@"\"}Exported Playlists.part01.rar"))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(homeDir, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
            // HappyPathTesting();

            Quit();
        }
        
        private static void Quit()
        {
            Console.WriteLine("Done\nPress ENTER to Exit");
            var keyPress = Console.ReadKey(true);
            while (keyPress.Key != ConsoleKey.Enter) keyPress = Console.ReadKey(intercept: true);
        }

        // ReSharper disable once UnusedMember.Local
        private static void HappyPathTesting() {
            Console.WriteLine("Enter gog URL:\n" + "https://www.gog.com/ru/game/mule");
            const string urlInput = "https://www.gog.com/ru/game/mule";
            var gameObj = new GameObj(urlInput);
            Console.WriteLine(gameObj);
            gameObj.StartDownloads();
            
        }
    }
}