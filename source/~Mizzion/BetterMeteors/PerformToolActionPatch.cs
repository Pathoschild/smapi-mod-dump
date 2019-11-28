using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace BetterMeteors
{
    internal class PerformToolActionPatch
    {
        public static IMonitor monitor;
        public static IModHelper helper;
        public static BetterMeteorsConfig config;
        public PerformToolActionPatch(IMonitor _monitor, IModHelper _helper, BetterMeteorsConfig _config)
        {
            monitor = _monitor;
            helper = _helper;
            config = _config;
        }
        public static bool performToolAction(ref ResourceClump __instance, ref Tool t, ref int damage,
            ref Vector2 tileLocation, ref GameLocation location)
        {
            if (__instance.parentSheetIndex.Value == 622 && config.EnableDebugMode)
            {
                monitor.Log("Found a Meteor.");
                return false;
            }
            return true;
        }
    }
}
