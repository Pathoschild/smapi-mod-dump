/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures;
using AlternativeTextures.Framework.Models;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches
{
    internal class UtilityPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Utility);

        internal UtilityPatch(IMonitor modMonitor) : base(modMonitor)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Utility.getCarpenterStock), null), postfix: new HarmonyMethod(GetType(), nameof(GetCarpenterStockPostFix)));
        }

        private static void GetCarpenterStockPostFix(Utility __instance, ref Dictionary<ISalable, int[]> __result)
        {
            __result.Add(GetPaintBucketTool(), new int[2] { 500, 1 });
            __result.Add(GetPaintBrushTool(), new int[2] { 500, 1 });
        }
    }
}
