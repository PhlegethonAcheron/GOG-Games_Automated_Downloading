using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace GG_Downloader
{
    public static class Program
    {
       
        public static void Main(string[] args)
        {
            IList<string> zippyLinks = LinkRetriever.GogGetZippyLink("https://gog-games.com/game/mule");
            foreach(string e in zippyLinks) {
                Console.WriteLine(LinkRetriever.ZippyGetFileLink(e));
                Console.WriteLine(LinkRetriever.);
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