/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConvenientChests.StashToChests;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientChests.CraftFromChests {
    public class CraftFromChestsModule : Module {
        private readonly MenuListener MenuListener;


        public CraftFromChestsModule(ModEntry modEntry) : base(modEntry) {
            MenuListener = new MenuListener(Events);
        }

        public override void Activate() {
            IsActive = true;

            // Register Events
            MenuListener.RegisterEvents();
            MenuListener.CraftingMenuShown += CraftingMenuShown;
        }

        public override void Deactivate() {
            IsActive = false;

            // Unregister Events
            MenuListener.CraftingMenuShown -= CraftingMenuShown;
            MenuListener.UnregisterEvents();
        }

        private void CraftingMenuShown(object sender, CraftingMenuArgs e) {
            var page = e.Page;
            if (page == null)
                return;

            // Find nearby chests
            var nearbyChests = GetChests(e.IsCookingPage).ToList();
            if (!nearbyChests.Any())
                return;

            // Add them as material containers to current CraftingPage
            var inventories = nearbyChests.Select(chest => chest.Items as IInventory);

            if (page._materialContainers == null)
                page._materialContainers = inventories.ToList();

            else
                page._materialContainers.AddRange(inventories);
        }

        private IEnumerable<Chest> GetChests(bool isCookingScreen) {
            // nearby chests
            var chests = Game1.player.GetNearbyChests(Config.CraftRadius).Where(c => c.Items.Any(i => i != null))
                .ToList();
            foreach (var c in chests)
                yield return c;

            // always add home fridge when on cooking screen
            if (!isCookingScreen)
                yield break;

            var house = Game1.player.currentLocation as FarmHouse ?? Utility.getHomeOfFarmer(Game1.player);
            if (house == null || house.upgradeLevel == 0)
                yield break;

            var fridge = house.fridge.Value;
            if (!chests.Contains(fridge))
                yield return fridge;
        }
    }
}