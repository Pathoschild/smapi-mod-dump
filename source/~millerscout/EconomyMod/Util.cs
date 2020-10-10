/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;

namespace EconomyMod
{
    public static class Util
    {
        public static IMonitor Monitor;

        public static ModConfig Config;

        public static IManifest ModManifest;

        public static bool IsDebug { get; internal set; }

        public static IModHelper Helper;
        public static bool IsValidMultiplayer => Context.IsWorldReady && Game1.IsMultiplayer;
    }
}
