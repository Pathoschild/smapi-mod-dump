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
        private StackLogic.AcceptingFunction CategorizedAcceptingFunction    { get; set; }
        private StackLogic.AcceptingFunction ExistingStacksAcceptingFunction { get; set; }

        public StashFromAnywhereModule(ModEntry modEntry) : base(modEntry) { }

        private StackLogic.AcceptingFunction CreateCategorizedAcceptingFunction() {
            if (Config.StashAnywhere) {
                return (chest, item) => ModEntry.CategorizeChests.ChestAcceptsItem(chest, item);
            }

            return (chest, item) => false;
        }

        private StackLogic.AcceptingFunction CreateExistingStacksAcceptingFunction() {
            if (Config.StashAnywhere && Config.StashAnywhereToExistingStacks) {
                return (chest, item) => chest.ContainsItem(item);
            }

            return (chest, item) => false;
        }

        public override void Activate() {
            IsActive = true;

            CategorizedAcceptingFunction    = CreateCategorizedAcceptingFunction();
            ExistingStacksAcceptingFunction = CreateExistingStacksAcceptingFunction();

            this.Events.Input.ButtonPressed += OnButtonPressed;
        }

        public override void Deactivate() {
            IsActive                        =  false;
            this.Events.Input.ButtonPressed -= OnButtonPressed;
        }

        private void StashGlobally() {
            // try to stash to fridge first
            if (Config.StashAnywhereToFridge && ChestExtension.GetFridge(Game1.player) is Chest fridge) {
                StackLogic.StashToChest(fridge, CategorizedAcceptingFunction);
                StackLogic.StashToChest(fridge, ExistingStacksAcceptingFunction);
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
            foreach (var chest in chests)
                StackLogic.StashToChest(chest, CategorizedAcceptingFunction);

            // stash by existing
            foreach (var chest in chests)
                StackLogic.StashToChest(chest, ExistingStacksAcceptingFunction);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
            if (e.Button == Config.StashAnywhereKey)
                StashGlobally();
        }
    }
}