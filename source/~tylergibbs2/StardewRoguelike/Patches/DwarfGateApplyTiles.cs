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
using StardewValley.Locations;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(DwarfGate), "ApplyTiles")]
    internal class DwarfGateApplyTiles
    {
        private static (int, int) GetTileIndex(xTile.Map map, int x, int y)
        {
            return Roguelike.GetTileIndexForMap(map, "volcano_dungeon", x + y * 16, doSum: true);
        }

        public static void Postfix(DwarfGate __instance)
        {
            xTile.Map map = Game1.content.Load<xTile.Map>(__instance.locationRef.Value.mapPath.Value);

            if (__instance.localOpened)
                __instance.locationRef.Value.removeTile(__instance.tilePosition.X, __instance.tilePosition.Y + 1, "Buildings");
            else
            {
                var (tileSheetIndex, tileIndex) = GetTileIndex(map, 0, 0);
                __instance.locationRef.Value.setMapTileIndex(__instance.tilePosition.X, __instance.tilePosition.Y + 1, tileIndex, "Buildings", whichTileSheet: tileSheetIndex);
            }
        }
    }
}
