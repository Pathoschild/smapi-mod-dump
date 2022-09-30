/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Saitoue/Orchard
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;







namespace Orchard.Patches
{
    internal class ObjectPatches
    {

        /// <summary>
        ///  adds Fruit Trees to canBePlacedHere for Tree Fertilizer
        /// </summary>
        [HarmonyPatch(typeof(Object), nameof(Object.canBePlacedHere))]
        internal class FertilizerPlacement
        {
            [HarmonyPostfix]
            protected static void Postfix(Object __instance, ref bool __result, GameLocation l, Vector2 tile)
            {
                int a = __instance.ParentSheetIndex;
                if (a == 805 && l.terrainFeatures.ContainsKey(tile) && l.terrainFeatures[tile] is FruitTree)
                {
                    __result = true;
                }
            }

        }
    }
}
