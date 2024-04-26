/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System.Linq;
using ConvenientChests.CategorizeChests.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using Utility = ConvenientChests.CategorizeChests.Utility;

namespace ConvenientChests.StashToChests {
    public class StashFromAnywhereModule : Module {
        private static StackLogic.AcceptingFunction CategorizedAcceptingFunction =>
            (chest, item) => ModEntry.CategorizeChests.ChestAcceptsItem(chest, item);

        private static StackLogic.AcceptingFunction ExistingStacksAcceptingFunction =>
            (chest, item) => chest.ContainsItem(item);

        public StashFromAnywhereModule(ModEntry modEntry) : base(modEntry) {}

        public override void Activate() {
            IsActive = true;

            Events.Input.ButtonPressed += OnButtonPressed;
        }

        public override void Deactivate() {
            IsActive = false;

            Events.Input.ButtonPressed -= OnButtonPressed;
        }

        private void StashGlobally() {
            ModEntry.Log("Stash to anywhere");

            // try to stash to fridge first
            var success = false;
            if (Config.StashAnywhereToFridge && ChestExtension.GetFridge(Game1.player) is {} fridge) {
                success |= StackLogic.TryStashToChest(fridge, CategorizedAcceptingFunction);
                success |= StackLogic.TryStashToChest(fridge, ExistingStacksAcceptingFunction);
            }

            // try to find all chests by location
            if (Game1.player.currentLocation == null)
                return;

            var chests = Utility.GetLocations()
                                .SelectMany(location => location.Objects.Pairs)
                                .Select(pair => pair.Value)
                                .OfType<Chest>()
                                .ToList();

            // stash by category
            success |= StackLogic.TryStashToChests(chests, CategorizedAcceptingFunction);

            // stash by existing
            if (Config.StashToExistingStacks)
                success |= StackLogic.TryStashToChests(chests, ExistingStacksAcceptingFunction);

            if (success)
                Game1.playSound(StackLogic.StashCueName);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
            if (!Config.StashAnywhere || e.Button != Config.StashAnywhereKey)
                return;

            StashGlobally();
        }
    }
}