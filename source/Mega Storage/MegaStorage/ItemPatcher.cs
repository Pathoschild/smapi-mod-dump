using MegaStorage.Framework;
using MegaStorage.Framework.Models;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

namespace MegaStorage
{
    public class ItemPatcher
    {
        public void Start()
        {
            MegaStorageMod.ModHelper.Events.Player.InventoryChanged += OnInventoryChanged;
            MegaStorageMod.ModHelper.Events.World.ChestInventoryChanged += OnChestInventoryChanged;
            MegaStorageMod.ModHelper.Events.World.ObjectListChanged += OnObjectListChanged;
        }

        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            MegaStorageMod.ModMonitor.VerboseLog("OnInventoryChanged");
            if (!e.IsLocalPlayer || e.Added.Count() != 1)
            {
                return;
            }

            var addedItem = e.Added.Single();
            if (addedItem is CustomChest)
            {
                return;
            }

            if (!CustomChestFactory.ShouldBeCustomChest(addedItem))
            {
                return;
            }

            MegaStorageMod.ModMonitor.VerboseLog("OnInventoryChanged: converting");

            var index = Game1.player.Items.IndexOf(addedItem);
            Game1.player.Items[index] = addedItem.ToCustomChest();
        }

        private void OnChestInventoryChanged(object sender, ChestInventoryChangedEventArgs e)
        {
            MegaStorageMod.ModMonitor.VerboseLog("OnChestInventoryChanged");
            if (e.Added.Count() != 1)
            {
                return;
            }

            var addedItem = e.Added.Single();
            if (addedItem is CustomChest)
            {
                return;
            }

            if (!CustomChestFactory.ShouldBeCustomChest(addedItem))
            {
                return;
            }

            MegaStorageMod.ModMonitor.VerboseLog("OnChestInventoryChanged: converting");

            var index = Game1.player.Items.IndexOf(addedItem);
            Game1.player.Items[index] = addedItem.ToCustomChest();
        }

        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            MegaStorageMod.ModMonitor.VerboseLog("OnObjectListChanged");
            if (e.Added.Count() != 1)
            {
                return;
            }

            var addedItemPosition = e.Added.Single();
            var addedItem = addedItemPosition.Value;
            if (addedItem is CustomChest)
            {
                return;
            }

            if (!CustomChestFactory.ShouldBeCustomChest(addedItem))
            {
                return;
            }

            MegaStorageMod.ModMonitor.VerboseLog("OnObjectListChanged: converting");

            var position = addedItemPosition.Key;
            var item = e.Location.objects[position];
            e.Location.objects[position] = item.ToCustomChest(position);
        }

    }
}
