/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Object = StardewValley.Object;

namespace DynamicReflections.Framework.Patches.Tools
{
    internal class ToolPatch : PatchTemplate
    {
        private readonly Type _type = typeof(Tool);

        internal ToolPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_type, nameof(Tool.doesShowTileLocationMarker), null), postfix: new HarmonyMethod(GetType(), nameof(DoesShowTileLocationMarkerPostfix)));
            harmony.Patch(AccessTools.Method(_type, nameof(Tool.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
        }

        private static void DoesShowTileLocationMarkerPostfix(Tool __instance, ref bool __result)
        {
            if (DynamicReflections.isFilteringMirror || DynamicReflections.isFilteringWater || DynamicReflections.isFilteringPuddles)
            {
                __result = false;
            }
        }

        private static bool DrawPrefix(Tool __instance, ref Farmer ___lastUser, ref int __state, SpriteBatch b)
        {
            if (DynamicReflections.isFilteringMirror || DynamicReflections.isFilteringWater || DynamicReflections.isFilteringPuddles)
            {
                return false;
            }

            return true;
        }
    }
}
