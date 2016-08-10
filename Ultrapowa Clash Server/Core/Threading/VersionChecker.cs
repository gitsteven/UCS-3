using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using Microsoft.Win32;

namespace UCS.Core.Web
{
    internal class VersionChecker
    {/*
        internal static void VersionMain()
        {
            try
            {
                WebClient wc = new WebClient();
                string Version = wc.DownloadString("https://static.smartclashcoc.com/UCS/version.txt");
                if (Version == "0.7.0.0")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine();
                    Console.WriteLine("--------------------------------------------------------------------------------");
                    Console.WriteLine();
                    Console.WriteLine("[UCS][INFO]  ->  UCS up-to-date");
                    Console.WriteLine();
                    Console.WriteLine("--------------------------------------------------------------------------------");
                    Console.WriteLine();
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine();
                    Console.WriteLine("--------------------------------------------------------------------------------");
                    Console.WriteLine();
                    Console.WriteLine("[UCS][ERROR]  ->  UCS has a new update. Download the latest version from GitHub");
                    Console.WriteLine("[UCS][ERROR]  ->  Current new version is : " + Version);
                    Console.WriteLine();
                    Console.WriteLine("--------------------------------------------------------------------------------");
                    Console.WriteLine();
                    Console.ResetColor();
                }
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine("[UCS][ERROR]  ->  Problem in checking UCS version");
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine();
                Console.ResetColor();
            }
        }*/
    }
}