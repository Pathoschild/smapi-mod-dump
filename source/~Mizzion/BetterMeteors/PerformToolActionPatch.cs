/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

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
