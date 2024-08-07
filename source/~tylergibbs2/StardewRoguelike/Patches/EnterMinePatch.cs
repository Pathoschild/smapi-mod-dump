/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1.enterMine))]
    internal class EnterMinePatch
    {
        public static bool Prefix()
        {
            Roguelike.NextFloor();

            Game1.inMine = true;
            // Warp to mine level 1. We handle choosing the map
            // elsewhere.
            Game1.warpFarmer($"UndergroundMine1/{Roguelike.CurrentLevel}", 6, 6, 2);

            return false;
        }
    }
}
