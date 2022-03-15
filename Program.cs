using System;

namespace GG_Downloader
{
    public static class Program {
        public static void Main()
        {
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
            Console.WriteLine("Enter gog URL:\n" + "https://www.gog.com/ru/game/mule");
            const string urlInput = "https://www.gog.com/ru/game/mule";
            var gameObj = new GameObj(urlInput);
            Console.WriteLine(gameObj);
            gameObj.StartDownloads();
            // GameObj.ExtractFilesFromDirectory($"{gameObj.GameDir}\\extras");
            // GameObj.ExtractFilesFromDirectory($"{gameObj.GameDir}\\game");

        }
    }
}