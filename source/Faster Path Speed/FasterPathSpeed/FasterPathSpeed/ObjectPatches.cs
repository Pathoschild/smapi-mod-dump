/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace FasterPathSpeed
{
    public class ObjectPatches
    {
        public static void PlacementAction_Postfix(Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer who)
        {
            try
            {
                FasterPathSpeed.ObjectPlacementAction(__instance, ref __result, location, x, y, who);
            }
            catch (System.Exception e)
            {
                ModEntry.Context.Monitor.Log($"Failed in {nameof(PlacementAction_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }
}
