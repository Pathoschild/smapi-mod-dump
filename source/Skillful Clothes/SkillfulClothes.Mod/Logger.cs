/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Configuration;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes
{
    static class Logger
    {
        static IMonitor monitor;
        static bool verbose;

        public static void Init(IMonitor _monitor, bool _verbose)
        {
            monitor = _monitor;
            verbose = _verbose;
        }

        public static void Info(String message)
        {
            if (verbose)
            {
                monitor?.Log(message, LogLevel.Info);
            }            
        }

        public static void Debug(String message)
        {
            if (verbose)
            {
                monitor?.Log(message, LogLevel.Debug);
            }
        }

        public static void Warn(String message)
        {
            if (verbose)
            {
                monitor?.Log(message, LogLevel.Warn);
            }
        }

        public static void Error(String message)
        {
            monitor?.Log(message, LogLevel.Error);
        }
    }
}
