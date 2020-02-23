using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConvenientChests.StashToChests;
using StardewValley;
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

        private void CraftingMenuShown(object sender, EventArgs e) {
            var isCooking = Game1.activeClickableMenu is CraftingPage || Game1.activeClickableMenu.GetType().ToString() == "CookingSkill.NewCraftingPage";
            var page = isCooking
                           ? Game1.activeClickableMenu
                           : (Game1.activeClickableMenu as GameMenu)?.pages[MenuListener.CraftingMenuTab] as CraftingPage;

            if (page == null)
                return;

            // Find nearby chests
            var nearbyChests = GetChests(isCooking).ToList();
            if (!nearbyChests.Any())
                return;

            // Add them as material containers to current CraftingPage
            var prop = page.GetType().GetField("_materialContainers", BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop == null) {
                ModEntry.Log($"CraftFromChests failed: {page.GetType()}._materialContainers not found.");
                return;
            }

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