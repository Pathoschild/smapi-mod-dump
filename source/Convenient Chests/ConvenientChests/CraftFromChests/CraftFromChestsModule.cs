using System;
using System.Collections.Generic;
using System.Linq;
using ConvenientChests.StackToNearbyChests;
using Harmony;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientChests.CraftFromChests {
    public class CraftFromChestsModule : Module {
        private MenuListener MenuListener;


        public CraftFromChestsModule(ModEntry modEntry) : base(modEntry) {
            this.MenuListener = new MenuListener(this.Events);
        }

        public override void Activate() {
            IsActive = true;

            // Register Events
            this.MenuListener.RegisterEvents();
            this.MenuListener.CraftingMenuShown += CraftingMenuShown;
        }

        public override void Deactivate() {
            IsActive = false;

            // Unregister Events
            this.MenuListener.CraftingMenuShown -= CraftingMenuShown;
            this.MenuListener.UnregisterEvents();
        }

        private void CraftingMenuShown(object sender, EventArgs e) {
            var isCooking = Game1.activeClickableMenu is CraftingPage;
            var page = isCooking
                           ? Game1.activeClickableMenu as CraftingPage
                           : (Game1.activeClickableMenu as GameMenu)?.pages[GameMenu.craftingTab] as CraftingPage;

            if (page == null)
                return;

            // Find nearby chests
            var nearbyChests = GetChests(isCooking).ToList();
            if (!nearbyChests.Any())
                return;

            // Add them as material containers to current CraftingPage
            var prop     = AccessTools.Field(page.GetType(), "_materialContainers");
            var original = prop.GetValue(page) as List<Chest>;
            var modified = new List<Chest>();
            if (original?.Count > 0)
                modified.AddRange(original);
            modified.AddRange(nearbyChests);

            prop.SetValue(page, modified.Distinct().ToList());
        }

        private IEnumerable<Chest> GetChests(bool isCookingScreen) {
            // nearby chests
            var chests = Game1.player.GetNearbyChests(Config.CraftRadius).Where(c => c.items.Any(i => i != null)).ToList();
            foreach (var c in chests)
                yield return c;

            // always add fridge when on cooking screen
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