using System.Linq;
using MegaStorage.Mapping;
using MegaStorage.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MegaStorage
{
    public class ItemPatcher
    {
        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;

        public ItemPatcher(IModHelper modHelper, IMonitor monitor)
        {
            _modHelper = modHelper;
            _monitor = monitor;
        }

        public void Start()
        {
            _modHelper.Events.Player.InventoryChanged += OnInventoryChanged;
            _modHelper.Events.World.ObjectListChanged += OnObjectListChanged;
            _modHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }
        
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            foreach (var customChest in CustomChestFactory.CustomChests)
            {
                Register(customChest);
            }
        }

        private void Register(CustomChest customChest)
        {
            _monitor.VerboseLog($"Registering {customChest.Config.Name} ({customChest.Config.Id})");
            _monitor.VerboseLog($"Recipe: {customChest.Config.Recipe}");
            _monitor.VerboseLog($"BigCraftableInfo: {customChest.BigCraftableInfo}");
            Game1.bigCraftablesInformation[customChest.Config.Id] = customChest.BigCraftableInfo;
            CraftingRecipe.craftingRecipes[customChest.Config.Name] = customChest.RecipeString;
            Game1.player.craftingRecipes[customChest.Config.Name] = 0;
        }

        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            _monitor.VerboseLog("OnInventoryChanged");
            if (!e.IsLocalPlayer || e.Added.Count() != 1)
                return;

            var addedItem = e.Added.Single();
            if (addedItem is CustomChest)
                return;

            if (!CustomChestFactory.IsCustomChest(addedItem))
                return;

            _monitor.VerboseLog("OnInventoryChanged: converting");

            var index = Game1.player.Items.IndexOf(addedItem);
            var item = Game1.player.Items[index];
            Game1.player.Items[index] = item.ToCustomChest();
        }

        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            _monitor.VerboseLog("OnObjectListChanged");
            if (e.Added.Count() != 1)
                return;

            var addedItemPosition = e.Added.Single();
            var addedItem = addedItemPosition.Value;
            if (addedItem is CustomChest)
                return;

            if (!CustomChestFactory.IsCustomChest(addedItem))
                return;

            _monitor.VerboseLog("OnObjectListChanged: converting");

            var position = addedItemPosition.Key;
            var item = e.Location.objects[position];
            e.Location.objects[position] = item.ToCustomChest();
        }

    }
}
