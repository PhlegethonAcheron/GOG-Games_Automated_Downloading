﻿using System;
using System.Collections.Generic;

namespace GG_Downloader
{
    public static class Program
    {
       
        public static void Main(string[] args)
        {
            IList<string> zippyLinks = LinkRetriever.GogGetZippyLink("https://gog-games.com/game/mule");
            foreach(string e in zippyLinks) {
                Console.WriteLine(LinkRetriever.ZippyGetFileLink(e));
                Console.WriteLine(LinkRetriever.ZippyGetFileName(e));
            }
            
            Quit();
        }
        
        private static void Quit()
        {
            Console.WriteLine("Done\nPress ENTER to Exit");

            ConsoleKeyInfo keyPress = Console.ReadKey(intercept: true);
            while (keyPress.Key != ConsoleKey.Enter)
            {
                keyPress = Console.ReadKey(intercept: true);
            }
        }
    }
}