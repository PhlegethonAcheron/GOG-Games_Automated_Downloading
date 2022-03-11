using System;
using System.Linq;

namespace GG_Downloader
{
    public static class Program {
        public static void Main()
        {
            Console.WriteLine("Enter gog URL:\n" + "https://www.gog.com/ru/game/mule");
            string urlInput = "https://www.gog.com/ru/game/mule";
            // urlInput = Console.ReadLine();
            var ggFileList = GameObj.GetGameFiles(urlInput);
            ggFileList.ToList().ForEach(Console.WriteLine);
            Quit();
        }
        
        private static void Quit()
        {
            Console.WriteLine("Done\nPress ENTER to Exit");

            ConsoleKeyInfo keyPress = Console.ReadKey(true);
            while (keyPress.Key != ConsoleKey.Enter) keyPress = Console.ReadKey(intercept: true);
        }

    }
}