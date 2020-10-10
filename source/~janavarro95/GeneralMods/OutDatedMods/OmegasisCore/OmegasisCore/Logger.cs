/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasisCore
{
    public class Logger
    {
        private static object _MessageLock = new object();

        public static void WriteMessage(string message,ConsoleColor textColor=ConsoleColor.White, ConsoleColor backgroundColor=ConsoleColor.Black)
        {
            lock (_MessageLock)
            {
                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = textColor;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        public static void GoodMessage(string message)
        {
            //WriteMessage(message, textColor, backgroundColor);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(ModInfo.ModName+": "+message);
            Console.ResetColor();
        }

    }
}
