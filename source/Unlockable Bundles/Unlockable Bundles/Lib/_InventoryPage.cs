/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
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

namespace Unlockable_Bundles.Lib
{
    public class _InventoryPage
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static Texture2D BundleOverviewIcon;
        private static ClickableTextureComponent BundleOverviewButton;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            BundleOverviewIcon = Helper.ModContent.Load<Texture2D>("assets/BundleOverviewIcon.png");

            var harmony = new Harmony(Mod.ModManifest.UniqueID);

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
        }

        public static void Constructor_Postfix(InventoryPage __instance)
        {
            Point pos = new Point(__instance.xPositionOnScreen + __instance.width, __instance.yPositionOnScreen + 96);

            if (__instance.junimoNoteIcon is not null) {
                __instance.junimoNoteIcon.rightNeighborID = 700;
  
                pos.X += 80;
            }
                

            BundleOverviewButton = new ClickableTextureComponent("", new Rectangle(pos.X, pos.Y, 64, 64), "", "Bundle Overview", BundleOverviewIcon, new Rectangle(0, 0, 60, 55), 1f) {
                myID = 700,
                leftNeighborID = 898
            };
        }

        public static void performHoverAction_Postfix(int x, int y, InventoryPage __instance)
        {
            BundleOverviewButton.tryHover(x, y);
            if (BundleOverviewButton.containsPoint(x, y))
                __instance.hoverText = "Bundle Overview";
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
