/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Leaper), "IsValidLandingTile")]
    internal class LeaperValidLandingTile
    {
        public static void Postfix(Leaper __instance, ref bool __result, Vector2 tile)
        {
            if (__instance.currentLocation.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Water", "Back") is not null)
                __result = false;
        }
    }
}
