/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SereneGreenhouse
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System.Reflection;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Linq;
using xTile.Dimensions;
using xTile.Tiles;
using System.Collections.Generic;

namespace SereneGreenhouse.Patches.GameLocation
{
    [HarmonyPatch]
    public class GameLocationCheckForMusic
    {
        private static IMonitor monitor = ModEntry.monitor;

        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.checkForMusic));
        }

        internal static bool Prefix(StardewValley.GameLocation __instance, GameTime time)
        {
            if (__instance.Name != "Greenhouse")
            {
                return true;
            }

            if (Game1.isMusicContextActiveButNotPlaying())
            {
                if (Game1.isRaining)
                {
                    Game1.changeMusicTrack("rain");
                }
                else if (!Game1.isDarkOut(__instance))
                {
                    Game1.changeMusicTrack("woodsTheme");
                }
            }

            return false;
        }
    }
}
