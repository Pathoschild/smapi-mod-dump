/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kakashigr/stardew-radioactivetools
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace RadioactiveTools.Framework {
    internal class BlacksmithInitializer {
        private static readonly int UpgradeCost = ModEntry.Config.RadioactiveToolCost;

        public static void Init(IModEvents events) {
            events.Display.MenuChanged += OnMenuChanged;
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnMenuChanged(object sender, MenuChangedEventArgs e) {


            if (!(e.NewMenu is ShopMenu menu)) {
                return;
            }
            List<int> categories = ModEntry.ModHelper.Reflection.GetField<List<int>>(menu, "categoriesToSellHere").GetValue();
            // Blacksmith
            if (categories.Contains(Object.GemCategory) && categories.Contains(Object.mineralsCategory) && categories.Contains(Object.metalResources)) {
                Farmer who = Game1.player;

                Tool toolFromName1 = who.getToolFromName("Axe");
                Tool toolFromName2 = who.getToolFromName("Watering Can");
                Tool toolFromName3 = who.getToolFromName("Pickaxe");
                Tool toolFromName4 = who.getToolFromName("Hoe");
                Tool tool;

                List<ISalable> forSale = menu.forSale;
                Dictionary<ISalable, int[]> stock = menu.itemPriceAndStock;

                if (toolFromName1 != null && toolFromName1.UpgradeLevel == 4) {
                    tool = new Axe { UpgradeLevel = 5 };
                    forSale.Add(tool);
                    stock.Add(tool, new[] { UpgradeCost, 5, 910 });
                }
                if (toolFromName2 != null && toolFromName2.UpgradeLevel == 4) {
                    tool = new WateringCan { UpgradeLevel = 5 };
                    forSale.Add(tool);
                    stock.Add(tool, new[] { UpgradeCost, 5, 910 });
                }
                if (toolFromName3 != null && toolFromName3.UpgradeLevel == 4) {
                    tool = new Pickaxe { UpgradeLevel = 5 };
                    forSale.Add(tool);
                    stock.Add(tool, new[] { UpgradeCost, 5, 910 });
                }
                if (toolFromName4 != null && toolFromName4.UpgradeLevel == 4) {
                    tool = new Hoe { UpgradeLevel = 5 };
                    forSale.Add(tool);
                    stock.Add(tool, new[] { UpgradeCost, 5, 910 });
                }

            }
            else if ( categories.Contains(Object.FishCategory) ) {
                Farmer who = Game1.player;
                Tool tool;
                List<ISalable> forSale = menu.forSale;
                Dictionary<ISalable, int[]> stock = menu.itemPriceAndStock;
                Tool toolFromName5 = who.getToolFromName("Iridium Rod");
                if (toolFromName5 != null) {
                    tool = new FishingRod(9);
                    forSale.Add(tool);
                    stock.Add(tool, new[] { 25000, 5, 910 });
                }
            }
            return;
            

        }
    }
}
