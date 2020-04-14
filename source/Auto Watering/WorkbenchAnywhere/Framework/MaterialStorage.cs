using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkbenchAnywhere.Utils;

namespace WorkbenchAnywhere.Framework
{
    public class MaterialStorage
    {
        private readonly HashSet<Chest> _assignedChests = new HashSet<Chest>();
        private readonly IMonitor _monitor;
        private ModConfig _config;
        private HashSet<string> _itemNamesToDeposit;
        private HashSet<int> _itemCategoriesToDeposit;
        private SButton _depositButton;
        private const string Tag = "workbenchanywhere:materials";

        public MaterialStorage(IMonitor monitor, ModConfig config)
        {
            _monitor = monitor;

            LoadConfig(config);
        }

        public void LoadConfig(ModConfig config)
        {
            _config = config;
            _itemNamesToDeposit = config.MaterialItemNames.ToHashSet();
            _itemCategoriesToDeposit = config.MaterialItemCategories.ToHashSet();
            _depositButton = InputUtils.ParseButton(config.DepositKey);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            try
            {
                if (_config.AllowRemoteDeposit && e.Button == _depositButton)
                {
                    DoDeposit();
                }
            }
            catch (Exception ex)
            {
                _monitor.Log($"Unhandled exception {ex}", LogLevel.Error);
            }
        }

        private void DoDeposit()
        {
            var itemsInChestGroupedByName = _assignedChests
                .SelectMany(chest => chest.items.Select(item => new { item, chest }))
                .GroupBy(chestItem => chestItem.item.Name)
                .ToDictionary(g => g.Key,
                    g => g.GroupBy(chestGroup => chestGroup.chest)
                        .OrderByDescending(chestGroup => chestGroup.Count())
                        .Select(chestGroup => chestGroup.Key));

            var itemsToDeposit = Game1.player.items
                .Where(item =>
                item != null &&
                (_itemCategoriesToDeposit.Contains(item.Category) ||
                _itemNamesToDeposit.Contains(item.Name) ||
                itemsInChestGroupedByName.ContainsKey(item.Name)));

            // TODO check mutexes?

            foreach (var item in itemsToDeposit)
            {
                if (itemsInChestGroupedByName.ContainsKey(item.Name))
                {
                    var chestsWithTargetItem = itemsInChestGroupedByName[item.Name];
                    DepositItem(item, chestsWithTargetItem.Concat(_assignedChests.Except(chestsWithTargetItem)));
                }
                else
                {
                    DepositItem(item, _assignedChests);
                }
            }

            Game1.playSound("Ship");
        }

        private Item DepositItem(Item item, IEnumerable<Chest> chestsToCheck)
        {
            var currentItem = item;
            foreach (var chest in chestsToCheck)
            {
                while (true)
                {
                    var resultItem = chest.addItem(currentItem);
                    if (resultItem == null)// item was put into chest
                    {
                        Game1.player.removeItemFromInventory(item);
                        return null;
                    }
                    else if (resultItem.Stack == currentItem.Stack)  // we got our item back with no stack changes, means chest is full
                        break;
                    currentItem = resultItem;
                }
            }

            if (currentItem != null)
                item.Stack = currentItem.Stack;     // could not fit all
            return currentItem;
        }

        public void RegisterEvents(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.World.ObjectListChanged += OnObjectListChanged;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            var builtLocations = Game1.locations.OfType<BuildableGameLocation>()
                .SelectMany(location => location.buildings)
                .Select(building => building.indoors.Value)
                .Where(location => location != null);
            var allLocations = Game1.locations.Concat(builtLocations);

            foreach (var location in allLocations)
            {
                foreach (var chest in location.objects.Values.OfType<Chest>())
                {
                    if (chest.HasTag(Tag))
                    {
                        _assignedChests.Add(chest);
                    }
                }
            }
        }

        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            foreach (var obj in e.Removed)
            {
                if (obj.Value is Chest chest && IsMaterialStorageChest(chest))
                    _assignedChests.Remove(chest);
            }
        }

        public bool IsMaterialStorageChest(Chest chest)
        {
            return _assignedChests.Contains(chest);
        }

        public void ToggleMaterialStorageChest(Chest chest)
        {
            var isMaterialChest = chest.ToggleTag(Tag);
            if (isMaterialChest)
                _assignedChests.Add(chest);
            else
                _assignedChests.Remove(chest);
        }

        private AnywhereCraftingPage GetPage(IClickableMenu menu, bool standalone)
        {
            return new AnywhereCraftingPage(
                menu.xPositionOnScreen,
                menu.yPositionOnScreen,
                menu.width,
                menu.height,
                false,
                standalone,
                _assignedChests.ToList()
               );
        }

        public void ReplaceGameMenu(GameMenu gameMenu)
        {
            if (gameMenu.pages[GameMenu.craftingTab] is AnywhereCraftingPage)
                return;

            gameMenu.pages[GameMenu.craftingTab] = GetPage(gameMenu, false);
        }

        public void ReplaceCraftingPage(CraftingPage craftingPage)
        {
            if (craftingPage is AnywhereCraftingPage)
                return;

            Game1.activeClickableMenu = GetPage(craftingPage, true);
        }

        /// <summary>
        /// Evaluate whether the current player has items for a given bluePrint
        /// Check the material chests in addition to user inventory
        /// </summary>
        /// <param name="bluePrint"></param>
        /// <returns></returns>
        /// <remarks>Derived from <see cref="BluePrint.doesFarmerHaveEnoughResourcesToBuild"/></remarks>
        public virtual bool HaveMatsFor(BluePrint bluePrint)
        {
            var allItems = Game1.player.items
                .Concat(_assignedChests.SelectMany(c => c.items))
                .ToList();
            
            foreach (var kvp in bluePrint.itemsRequired)
            {
                if (Game1.player.getItemCountInList(allItems, kvp.Key, 0) < kvp.Value)
                    return false;
            }

            return Game1.player.Money >= bluePrint.moneyRequired;
        }

        public virtual void ConsumeResources(BluePrint bluePrint)
        {
            var itemLists = new[] { Game1.player.items }
                .Concat(_assignedChests.Select(c => c.items))
                .ToArray();

            foreach (var kvp in bluePrint.itemsRequired)
            {
                var remainingCount = kvp.Value;
                foreach (var itemList in itemLists)
                {
                    if (remainingCount <= 0)
                        break;
                    remainingCount = itemList.ConsumeItem(kvp.Key, remainingCount);
                }

                if (remainingCount > 0)
                    _monitor.Log($"Could not get all materials (race condition from multiplayer?); remaining {remainingCount}", LogLevel.Warn);
            }

            Game1.player.Money -= bluePrint.moneyRequired;
        }
    }
}