/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.UI;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using xTile.Dimensions;

namespace FashionSense.Framework.Patches.GameLocations
{
    internal class GameLocationPatch : PatchTemplate
    {
        private readonly Type _object = typeof(GameLocation);

        internal GameLocationPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.performAction), new[] { typeof(string), typeof(Farmer), typeof(Location) }), postfix: new HarmonyMethod(GetType(), nameof(PerformActionPostfix)));
        }

        private static void PerformActionPostfix(GameLocation __instance, ref bool __result, string fullActionString, Farmer who, Location tileLocation)
        {
            if (__result is false && Game1.activeClickableMenu is null && fullActionString.Equals("OpenFashionSense", StringComparison.OrdinalIgnoreCase))
            {
                Game1.activeClickableMenu = new HandMirrorMenu();
                __result = true;
            }
        }
    }
}
