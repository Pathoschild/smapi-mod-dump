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
using StardewValley.Locations;
using StardewValley.Monsters;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(MineShaft), "tryToAddMonster")]
    internal class TryToAddMonsterPatch
    {
        public static bool Prefix(MineShaft __instance, Monster m, int tileX, int tileY)
        {
            Roguelike.AdjustMonster(__instance, ref m);

            if (__instance.isTileClearForMineObjects(tileX, tileY) && !__instance.isTileOccupied(new Vector2(tileX, tileY)))
            {
                m.setTilePosition(tileX, tileY);
                __instance.characters.Add(m);
            }

            return false;
        }
    }
}
