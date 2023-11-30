/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/StardewSandbox
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using xTile.Dimensions;
using xTile.Display;
using xTile.Layers;

namespace HatShopRestoration.Framework.Patches.xTiles
{
    internal class LayerPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Layer);

        internal LayerPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Layer.Draw), new[] { typeof(IDisplayDevice), typeof(xTile.Dimensions.Rectangle), typeof(xTile.Dimensions.Location), typeof(bool), typeof(int), typeof(float) }), postfix: new HarmonyMethod(GetType(), nameof(DrawPostfix)));
        }

        private static void DrawPostfix(Layer __instance, IDisplayDevice displayDevice, xTile.Dimensions.Rectangle mapViewport, Location displayOffset, bool wrapAround, int pixelZoom, float sort_offset)
        {
            if (__instance is null || String.IsNullOrEmpty(__instance.Id))
            {
                return;
            }

            if (Game1.currentLocation is null || Game1.currentLocation.NameOrUniqueName.Contains("Custom_PeacefulEnd_MouseShop") is false)
            {
                return;
            }

            if (__instance.Id.Equals("Back", StringComparison.OrdinalIgnoreCase) is true)
            {
                Game1.currentLocation.Map.GetLayer("Accessories").Draw(displayDevice, mapViewport, displayOffset, wrapAround, pixelZoom);
            }

            if (__instance.Id.Equals("Buildings", StringComparison.OrdinalIgnoreCase) is true)
            {
                Game1.currentLocation.Map.GetLayer("Secondary").Draw(displayDevice, mapViewport, displayOffset, wrapAround, pixelZoom);
            }
        }
    }
}