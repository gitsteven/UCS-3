/*
 * Program : Ultrapowa Clash Server
 * Description : A C# Writted 'Clash of Clans' Server Emulator !
 *
 * Authors:  Jean-Baptiste Martin <Ultrapowa at Ultrapowa.com>,
 *           And the Official Ultrapowa Developement Team
 *
 * Copyright (c) 2016  UltraPowa
 * All Rights Reserved.
 */

using System;
using System.Threading;
using static System.Configuration.ConfigurationManager;
using static UCS.Helpers.Utils;
using static UCS.Helpers.CommandParser;
using static System.Console;
using static System.Environment;
using UCS.Core.Web;

namespace UCS.Core.Threading
{
    internal class ConsoleThread
    {
        #region Private Fields
        static string Command;

        #endregion Private Fields

        #region Private Properties

        static Thread T { get; set; }

        #endregion Private Properties

        #region Public Methods

        internal static void Start()
        {
            T = new Thread(() =>
            {
                Title = VersionTitle;
                WriteLine(
                    @"
    888     888 888    88888888888 8888888b.         d8888 8888888b.   .d88888b.  888       888        d8888
    888     888 888        888     888   Y88b       d88888 888   Y88b d88P' 'Y88b 888   o   888       d88888
    888     888 888        888     888    888      d88P888 888    888 888     888 888  d8b  888      d88P888
    888     888 888        888     888   d88P     d88P 888 888   d88P 888     888 888 d888b 888     d88P 888
    888     888 888        888     8888888P'     d88P  888 8888888P'  888     888 888d88888b888    d88P  888
    888     888 888        888     888 T88b     d88P   888 888        888     888 88888P Y88888   d88P   888
    Y88b. .d88P 888        888     888  T88b   d8888888888 888        Y88b. .d88P 8888P   Y8888  d8888888888
     'Y88888P'  88888888   888     888   T88b d88P     888 888         'Y88888P'  888P     Y888 d88P     888
                  ");
                if (OpenedInstances > 1)
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("[UCS]    You seem to run UCS more than once.");
                    WriteLine("[UCS]    Aborting..");
                    ResetColor();
                    ReadKey();
                    Exit(0);
                }
                WriteLine("[UCS]    -> This program is by the Ultrapowa Network development team.");
                WriteLine("[UCS]    -> You can find the source at www.ultrapowa.com and https://github.com/Clashoflights/UCS/");
                WriteLine("[UCS]    -> Don't forget to visit www.ultrapowa.com daily for updates!");
                WriteLine("[UCS]    -> UCS is now starting...");
                WriteLine("");
               // VersionChecker.VersionMain();
                //Debugger.SetLogLevel(int.Parse(AppSettings["loggingLevel"]));
                MemoryThread.Start();
                NetworkThread.Start();
                while ((Command = ReadLine()) != null)
                    Parse(Command);
            });
            T.Start();
        }

        public static void Stop()
        {
            if (T.ThreadState == ThreadState.Running)
                T.Abort();
        }

        #endregion Public Methods
    }
}