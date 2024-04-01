/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

using ChestFeatureSet.Framework;
using ChestFeatureSet.Framework.CFSChest;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ChestFeatureSet.StashToChests
{
    public class StashToChestsModule : Module
    {
        public StashToChestsModule(ModEntry modEntry) : base(modEntry) { }

        public delegate bool AcceptingFunction(Chest c, Item i);

        public CFSChestController CFSChestController { get; private set; }

        public override void Activate()
        {
            this.IsActive = true;

            Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
            textures.Add("SelectedComponent", this.ModEntry.Helper.ModContent.Load<Texture2D>("assets/unableToStash.png"));
            textures.Add("DeselectedComponent", this.ModEntry.Helper.ModContent.Load<Texture2D>("assets/ableToStash.png"));

            var sourceRect = new Rectangle(0, 0, textures["SelectedComponent"].Width, textures["SelectedComponent"].Height);

            this.CFSChestController = new CFSChestController(this.ModEntry, "StashToChests.json", textures, sourceRect, 1);

            this.Events.Display.MenuChanged += this.OnMenuChanged;
            this.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        public override void Deactivate()
        {
            IsActive = false;

            this.CFSChestController.Deactivate();
            this.CFSChestController = null;

            this.Events.Display.MenuChanged -= this.OnMenuChanged;
            this.Events.Input.ButtonPressed -= this.OnButtonPressed;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            this.CFSChestController.UpdateOpenedChest();
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == this.Config.StashKey)
                this.TryToStash();
        }

        private void TryToStash()
        {
            if (Game1.player.currentLocation == null)
                return;

            if (Game1.activeClickableMenu is ItemGrabMenu m && m.context is Chest c)
                this.StashToChest(c);
            else
                this.StashToChests(this.Config.StashRadius);
        }

        private void StashToChest(Chest chest)
        {
            var stashList = this.GetTheChestStashList(chest);

            if (stashList.Any() && chest.DumpItemsToChest(Game1.player.Items, stashList).Any())
                Game1.playSound(Game1.soundBank.GetCue("Ship").Name);
            else
                Game1.showRedMessage("Failed To Stash");
        }

        private void StashToChests(int radius)
        {
            var movedAtLeastOne = false;

            //Monitor.Log(Game1.player.currentLocation.parentLocationName, LogLevel.Info);

            IEnumerable<Chest>? chests = null;
            if (this.Config.StashLocationSetting is "Anywhere")
                chests = ChestExtension.GetAllChests().Select(chestPair => chestPair.Chest);
            else if (this.Config.StashLocationSetting is "FarmArea" && LocationExtension.FarmArea.Contains(Game1.player.currentLocation.Name))
                chests = ChestExtension.GetAreaChests(LocationExtension.FarmArea).Select(chestPair => chestPair.Chest);
            else
                chests = Game1.player.GetNearbyChests(radius);

            if (!chests.Any())
                return;

            foreach (var chest in chests)
            {
                var stashList = this.GetTheChestStashList(chest);

                if (!stashList.Any() || this.CFSChestController.GetChests().Contains(chest))
                    continue;

                var movedItems = chest.DumpItemsToChest(Game1.player.Items, stashList);
                if (movedItems.Any())
                    movedAtLeastOne = true;
            }

            if (movedAtLeastOne)
                Game1.playSound(Game1.soundBank.GetCue("Ship").Name);
            else
                Game1.showRedMessage("Failed To Stash");
        }

        private List<Item> GetTheChestStashList(Chest chest)
        {
            var f = this.GetAcceptingFunction();

            if (this.Config.LockItems && this.ModEntry.LockItems != null)
                return Game1.player.Items.Where(i => i != null)
                                         .Where(i => !this.ModEntry.LockItems.CFSItemController.GetItems().Contains(i))
                                         .Where(i => f(chest, i))
                                         .ToList();
            else
                return Game1.player.Items.Where(i => i != null)
                                         .Where(i => f(chest, i))
                                         .ToList();
        }

        private AcceptingFunction GetAcceptingFunction()
        {
            if (this.Config.OnlyStashToExistingStacks)
                return (chest, item) => chest.ContainsItemByStack(item);
            else
                return (chest, item) => chest.ContainsItem(item);
        }
    }
}
