using System;

namespace GG_Downloader
{
    public static class Program {
        public static void Main()
        {
            GameInstaller.InstallFromSetupExecutable(@"C:\Users\Townsend\Saved Games\GOG_Downloader\mule\game\setup_m.u.l.e._1.00_(45107).exe");
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
            GameInstaller.ExtractFilesFromDirectory($"{gameObj.GameDir}\\extras");
            GameInstaller.ExtractFilesFromDirectory($"{gameObj.GameDir}\\game");

        }
    }
}