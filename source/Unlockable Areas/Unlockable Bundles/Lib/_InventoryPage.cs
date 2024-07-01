/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.Lib
{
    public class _InventoryPage
    {
        public static Texture2D BundleOverviewIcon;
        private static ClickableTextureComponent BundleOverviewButton;

        private const int OverviewButtonId = 6401;
        private const int JunimoButtonId = 898;
        private const int OrganizeButtonId = InventoryPage.region_organizeButton;
        private const int LastInventorySlotId = 11;
        public static void Initialize()
        {
            BundleOverviewIcon = Helper.GameContent.Load<Texture2D>("UnlockableBundles/UI/BundleOverviewIcon");

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.DeclaredConstructor(typeof(InventoryPage), new[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(typeof(_InventoryPage), nameof(_InventoryPage.Constructor_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.performHoverAction)),
                postfix: new HarmonyMethod(typeof(_InventoryPage), nameof(_InventoryPage.performHoverAction_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.receiveLeftClick)),
                prefix: new HarmonyMethod(typeof(_InventoryPage), nameof(_InventoryPage.receiveLeftClick_Prefix))
            );

            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(InventoryPage), nameof(InventoryPage.draw), new[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(_InventoryPage), nameof(_InventoryPage.draw_Postfix))
            );

            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(IClickableMenu), nameof(IClickableMenu.populateClickableComponentList)),
                postfix: new HarmonyMethod(typeof(_InventoryPage), nameof(_InventoryPage.populateClickableComponentList_Postfix))
            );
        }

        //Mod compatibility fixes over who gets what spot in the inventory page go here :)
        public static void Constructor_Postfix(InventoryPage __instance)
        {
            Point pos = new Point(__instance.xPositionOnScreen + __instance.width, __instance.yPositionOnScreen + 96);
            var leftNeighborID = LastInventorySlotId;
            var downNeighborID = OrganizeButtonId;

            if (__instance.junimoNoteIcon is not null) {
                leftNeighborID = JunimoButtonId;
                __instance.junimoNoteIcon.rightNeighborID = OverviewButtonId;
                pos.X += 80;

            } else
                __instance.organizeButton.upNeighborID = OverviewButtonId;

            BundleOverviewButton = new ClickableTextureComponent("", new Rectangle(pos.X, pos.Y, 64, 64), "", Helper.Translation.Get("ub_overview_button"), BundleOverviewIcon, new Rectangle(0, 0, 61, 56), 1f) {
                myID = OverviewButtonId,
                leftNeighborID = leftNeighborID,
                downNeighborID = downNeighborID
            };
        }

        public static void populateClickableComponentList_Postfix(IClickableMenu __instance)
        {
            if (__instance is InventoryPage)
                __instance.allClickableComponents.Add(BundleOverviewButton);
        }


        public static void performHoverAction_Postfix(int x, int y, InventoryPage __instance)
        {
            BundleOverviewButton.tryHover(x, y);
            if (BundleOverviewButton.containsPoint(x, y))
                __instance.hoverText = Helper.Translation.Get("ub_overview_button");
        }

        public static bool receiveLeftClick_Prefix(int x, int y, bool playSound, InventoryPage __instance)
        {
            if (BundleOverviewButton.containsPoint(x, y) && __instance.readyToClose()) {
                Game1.activeClickableMenu = new BundleOverviewMenu();
                return false;
            }

            return true;
        }

        public static void draw_Postfix(SpriteBatch b)
        {
            BundleOverviewButton?.draw(b);
        }
    }
}
