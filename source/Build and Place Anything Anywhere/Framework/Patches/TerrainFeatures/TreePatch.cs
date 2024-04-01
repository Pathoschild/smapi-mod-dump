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
using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.TerrainFeatures;
using static StardewValley.Minigames.MineCart;
using xTile.Tiles;
using Microsoft.Xna.Framework;

namespace AnythingAnywhere.Framework.Patches.TerrainFeatures
{
    internal class TreePatch : PatchTemplate
    {
        private readonly Type _object = typeof(Tree);

        internal TreePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }
        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Tree.IsGrowthBlockedByNearbyTree)), postfix: new HarmonyMethod(GetType(), nameof(IsGrowthBlockedByNearbyTreePostfix)));
        }

        public static void IsGrowthBlockedByNearbyTreePostfix(Tree __instance, ref bool __result)
        {
            if (ModEntry.modConfig.EnableWildTreeTweaks)
                __result = false;
        }
    }
}
