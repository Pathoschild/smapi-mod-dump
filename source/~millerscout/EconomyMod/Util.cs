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
