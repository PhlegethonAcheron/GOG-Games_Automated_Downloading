using System;
using System.IO;
using Microsoft.VisualBasic;

namespace GG_Downloader
{
    public static class Program {
        public static void Main()
        {
            var path = @"C:\Users\Townsend\Saved Games\GOG_Downloader\mule\extras";
            var isDir = File.GetAttributes(path).HasFlag(FileAttribute.Directory);
            // HappyPathTesting();
            Console.WriteLine(isDir.ToString());

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
            gameObj.ExtractDownloads();
            gameObj.CleanupDownloads();
            gameObj.InstallDownloads();
        }
    }
}