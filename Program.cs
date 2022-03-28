using System;

namespace GG_Downloader
{
    public static class Program {
        public static void Main() {
            HappyPathTesting();
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
            Console.WriteLine("Enter gog URL:");
            var urlInput = Console.ReadLine();
            var gameObj = new GameObj(urlInput);
            Console.WriteLine(gameObj);
            gameObj.StartDownloads();
            gameObj.ExtractDownloads();
            gameObj.InstallDownloads();
            gameObj.CleanupDownloadsV2();
        }
    }
}
//make installer work when there is a .bin file
//proper CLI arguments
//not deleting everything
//not rely on chrome
//do a better job of finding things