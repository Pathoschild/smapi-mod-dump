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
using xTile.Dimensions;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.drawBackground))]
    class DrawExtraBackLayer
    {
        public static bool Prefix(GameLocation __instance)
        {
            __instance.Map.GetLayer("BackBack")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, wrapAround: false, 4);
            return true;
        }
    }
}
