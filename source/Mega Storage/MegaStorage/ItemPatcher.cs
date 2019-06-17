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
            foreach (var niceChest in NiceChestFactory.NiceChests)
            {
                Register(niceChest);
            }
        }

        private void Register(NiceChest niceChest)
        {
            _monitor.VerboseLog($"Registering {niceChest.ItemName} ({niceChest.ItemId})");
            Game1.bigCraftablesInformation[niceChest.ItemId] = niceChest.BigCraftableInfo;
            CraftingRecipe.craftingRecipes[niceChest.ItemName] = niceChest.RecipeString;
            Game1.player.craftingRecipes[niceChest.ItemName] = 0;
        }

        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            _monitor.VerboseLog("OnInventoryChanged");
            if (!e.IsLocalPlayer || e.Added.Count() != 1)
                return;

            var addedItem = e.Added.Single();
            if (addedItem is NiceChest)
                return;

            if (!NiceChestFactory.IsNiceChest(addedItem))
                return;

            _monitor.VerboseLog("OnInventoryChanged: converting");

            var index = Game1.player.Items.IndexOf(addedItem);
            Game1.player.Items[index] = NiceChestFactory.Create(addedItem.ParentSheetIndex);
        }

        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            _monitor.VerboseLog("OnObjectListChanged");
            if (e.Added.Count() != 1)
                return;

            var addedItemPosition = e.Added.Single();
            var addedItem = addedItemPosition.Value;
            if (addedItem is NiceChest)
                return;

            if (!NiceChestFactory.IsNiceChest(addedItem))
                return;

            _monitor.VerboseLog("OnObjectListChanged: converting");

            var position = addedItemPosition.Key;
            e.Location.objects[position] = NiceChestFactory.Create(addedItem.ParentSheetIndex);
        }

    }
}
