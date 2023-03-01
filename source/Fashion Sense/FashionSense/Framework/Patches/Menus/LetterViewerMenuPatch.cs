/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Patches.ShopLocations;
using FashionSense.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Menus;
using System;

namespace FashionSense.Framework.Patches.Menus
{
    internal class LetterViewerMenuPatch : PatchTemplate
    {
        private readonly Type _menu = typeof(LetterViewerMenu);

        internal LetterViewerMenuPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Constructor(_menu, new[] { typeof(string), typeof(string), typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(LetterViewerMenuPostfix)));
        }

        private static void LetterViewerMenuPostfix(LetterViewerMenu __instance, string mail, string mailTitle, bool fromCollection = false)
        {
            if (mailTitle.Equals(ModDataKeys.LETTER_HAND_MIRROR, StringComparison.OrdinalIgnoreCase) && fromCollection is false)
            {
                __instance.itemsToGrab.Add(new ClickableComponent(new Rectangle(__instance.xPositionOnScreen + __instance.width / 2 - 48, __instance.yPositionOnScreen + __instance.height - 32 - 96, 96, 96), SeedShopPatch.GetHandMirrorTool())
                {
                    myID = 104,
                    leftNeighborID = 101,
                    rightNeighborID = 102
                });

                _monitor.Log("Added the Hand Mirror to the LetterViewerMenu", LogLevel.Trace);
            }
        }
    }
}
