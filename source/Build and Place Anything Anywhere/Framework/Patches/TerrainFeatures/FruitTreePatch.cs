/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using System;

namespace AnythingAnywhere.Framework.Patches.TerrainFeatures
{
    internal class FruitTreePatch : PatchTemplate
    {
        private readonly Type _object = typeof(FruitTree);

        internal FruitTreePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }
        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(FruitTree.IsGrowthBlocked), new[] { typeof(Vector2), typeof(GameLocation) }), postfix: new HarmonyMethod(GetType(), nameof(IsGrowthBlockedPostfix)));
        }

        public static void IsGrowthBlockedPostfix(FruitTree __instance, Vector2 tileLocation, GameLocation environment, ref bool __result)
        {
            if (ModEntry.modConfig.EnableFruitTreeTweaks)
                __result = false;
        }
    }
}
