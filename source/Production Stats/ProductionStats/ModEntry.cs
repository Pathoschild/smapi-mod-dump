/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

// derived from code by Jesse Plamondon-Willard under MIT license: https://github.com/Pathoschild/StardewMods

using ProductionStats.Components;
using ProductionStats.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace ProductionStats;

internal class ModEntry : Mod
{
    /// <summary>
    ///     Finds all the chests which are placed by the player in the world.
    /// </summary>
    private ChestFinder _chestFinder = null!; // Set on Entry.

    /// <summary>
    ///     The previous menus shown before the current lookup UI was opened.
    /// </summary>
    private readonly PerScreen<Stack<IClickableMenu>> _previousMenus = new(() => new());

    /// <summary>
    ///     Sort options which can be applied to the menu and change order in 
    ///     which items are shown.
    /// </summary>
    private readonly Queue<SortOrder> _sortOrders = new(
    [
        SortOrder.None,
        SortOrder.AscendingByName,
        SortOrder.DescendingByName,
        SortOrder.AscendingByCount,
        SortOrder.DescendingByCount,
    ]);

    /// <summary>
    /// Metrics calls which represents scopes which be displayed to user.
    /// </summary>
    private readonly Func<IEnumerable<ItemStock>>[] _metricOrders
        = new Func<IEnumerable<ItemStock>>[4];

    /// <summary>
    /// Metrics' titles displayed on page.
    /// </summary>
    private readonly string[] _metricsTitles = new string[4];

    /// <summary>
    /// Index of currently visible metric;
    /// </summary>
    private int _currentMetricIndex = 0;

    private ModConfig _config = null!; // Set in Entry;

    /// <summary>The configure key bindings.</summary>
    private ModConfigKeys _keys => _config.Controls;

    /// <summary>
    /// Tracks player's inventory to display useful metrics.
    /// </summary>
    private InventoryTracker _inventoryTracker = null!; // Set in OnSaveLoaded.

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">
    ///     Provides methods for interacting with the mod directory, 
    ///     such as read/writing a config file or custom JSON files.
    /// </param>
    public override void Entry(IModHelper helper)
    {
        _config = helper.ReadConfig<ModConfig>();
        _chestFinder = new ChestFinder(helper.Multiplayer);

        // define order of metrics displayed
        _metricOrders[0] = () => _inventoryTracker.ProducedToday();
        _metricOrders[1] = () => _inventoryTracker.ProducedThisWeek();
        _metricOrders[2] = () => _inventoryTracker.ProducedThisSeason();
        _metricOrders[3] = () => _inventoryTracker.ProducedThisYear();

        // define title of each metric
        _metricsTitles[0] = "Produced today";
        _metricsTitles[1] = "Produced this week";
        _metricsTitles[2] = "Produced this season";
        _metricsTitles[3] = "Produced this year";

        // hook up events
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.Saving += OnSaving;
        helper.Events.Display.MenuChanged += OnMenuChanged;
        helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        helper.Events.Player.InventoryChanged += OnInventoryChanged;
        helper.Events.World.ChestInventoryChanged += OnChestInventoryChanged;
    }

    private void OnSaving(object? sender, SavingEventArgs e)
    {
        Helper.Data.WriteSaveData("inventory-tracker-start", _inventoryTracker.Start);

        // I can't serialize entire Item object. Instead
        // I'm just going to save QualifiedItemId and recreate Item object
        // later.

        // If first version of tracked items is present in save,
        // continue using it, otherwise, switch to v2.
        // This was added to not break existing saves with different versions.
        if (Helper.Data.SaveDataKeyExists("inventory-tracker-items"))
        {
            var items = _inventoryTracker.TrackedItems.Select(x => x.ToSerializeable());
            Helper.Data.WriteSaveData("inventory-tracker-items", items);
        }
        else
        {
            var items = _inventoryTracker.TrackedItems.Select(x => x.ToSerializableV2());
            Helper.Data.WriteSaveData("inventory-tracker-items-v2", items);
        }
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        SDate inventoryTrackerStart = Helper.Data.ReadSaveData<SDate>("inventory-tracker-start")!;

        // initialize tracker and create spot of it in save data if
        // isn't there already.
        inventoryTrackerStart ??= SDate.Now();

        _inventoryTracker = new InventoryTracker(
            dateProvider: new InGameTimeProvider(),
            start: inventoryTrackerStart);

        Helper.Data.WriteSaveData("inventory-tracker-start", inventoryTrackerStart);

        // Because it is not possible to serialize complete Item
        // using WriteSaveData instead, I was able to serialize
        // QualifiedItemId along with rest of the data which later
        // will be used to recreate TrackedItem objects.
        if (Helper.Data.SaveDataKeyExists("inventory-tracker-items"))
        {
            var items = Helper.Data
                .ReadSaveData<IEnumerable<(string QualifiedItemId, int Count, SDate Date)>>("inventory-tracker-items");

            if (items is not null)
            {
                var trackedItems = items.Select(x => new TrackedItem(x));
                _inventoryTracker.TrackedItems = trackedItems.ToList();
            }
        }
        else
        {
            var items = Helper.Data
                .ReadSaveData<IEnumerable<(string QualifiedItemId, int Quality, int Count, SDate Date)>>("inventory-tracker-items-v2");

            if (items is not null)
            {
                var trackedItems = items.Select(x => new TrackedItem(x));
                _inventoryTracker.TrackedItems = trackedItems.ToList();
            }
        }

#if DEBUG
        foreach (var item in VisualizationDataset.Get())
        {
            _inventoryTracker.Add(item.Item, item.Count);
        }
#endif
    }

    private void OnChestInventoryChanged(object? sender, ChestInventoryChangedEventArgs e)
    {
        if (Context.IsWorldReady == false)
        {
            return;
        }

        // TODO: Allow only for local player to change inventory tracker.
        HandleQuantityChanges(e.QuantityChanged);
        HandleAdded(e.Added);
        HandleRemoved(e.Removed);
    }

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (Context.IsWorldReady == false)
        {
            return;
        }

        // track only local player's inventory change.
        if (e.IsLocalPlayer == false)
        {
            return;
        }

        HandleQuantityChanges(e.QuantityChanged);
        HandleAdded(e.Added);
        HandleRemoved(e.Removed);
    }

    private void HandleAdded(IEnumerable<Item> items)
    {
        foreach (Item item in items)
        {
            _inventoryTracker.Add(item, item.Stack);
        }
    }

    private void HandleRemoved(IEnumerable<Item> items)
    {
        foreach (Item item in items)
        {
            _inventoryTracker.Add(item, -item.Stack);
        }
    }

    private void HandleQuantityChanges(IEnumerable<ItemStackSizeChange> items)
    {
        // changes in size of the stack
        foreach (ItemStackSizeChange change in items)
        {
            _inventoryTracker.Add(change.Item, change.NewSize - change.OldSize);
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // get Generic Mod Config Menu's API (if it's installed)
        IGenericModConfigMenuApi? configMenu = Helper
            .ModRegistry
            .GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

        if (configMenu is null)
            return;

        // register mod
        configMenu.Register(
            mod: ModManifest,
            reset: () => _config = new ModConfig(),
            save: () => Helper.WriteConfig(_config)
        );

        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: () => "Controls",
            tooltip: () => "Section dedicated to interactions with this mod"
        );

        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => "Toggle menu",
            tooltip: () => "Toggles menu which display number of items in player's possession",
            getValue: () => _config.Controls.ToggleMenu,
            setValue: value => _config.Controls.ToggleMenu = value
        );

        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => "Change sort order",
            tooltip: () => "Changes sorting order which is used to display player's item in menu",
            getValue: () => _config.Controls.Sort,
            setValue: value => _config.Controls.Sort = value
        );

        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => "Focus filter search",
            tooltip: () => "Focus on search text box to allow typing in it.",
            getValue: () => _config.Controls.FocusSearch,
            setValue: value => _config.Controls.FocusSearch = value
        );

        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => "Toggle production menu",
            tooltip: () => "Toggles menu which display number of items based on when they were acquired.",
            getValue: () => _config.Controls.ToggleProductionMenu,
            setValue: value => _config.Controls.ToggleProductionMenu = value
        );

        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => "Go to next metric",
            tooltip: () => "Changes current metric view for the next available.",
            getValue: () => _config.Controls.NextMetric,
            setValue: value => _config.Controls.NextMetric = value);

        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => "Go to previous metric",
            tooltip: () => "Changes current metric view for the previous.",
            getValue: () => _config.Controls.PreviousMetric,
            setValue: value => _config.Controls.PreviousMetric = value);
    }

    /// <inheritdoc cref="IDisplayEvents.MenuChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        Monitor.Log("Restoring the previous menu");
        // restore the previous menu if it was hidden to show the lookup UI
        if (e.NewMenu == null
            && (e.OldMenu is ItemMenu)
            && _previousMenus.Value.Count != 0)
        {
            Game1.activeClickableMenu = _previousMenus.Value.Pop();
        }
    }

    /// <inheritdoc cref="IInputEvents.ButtonsChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (Context.IsWorldReady == false)
        {
            return;
        }

        if (_keys.ToggleProductionMenu.JustPressed())
        {
            ToggleProductionMenu();
        }
        else if (_keys.ToggleMenu.JustPressed())
        {
            ToggleMenu();
        }
        else if (_keys.Sort.JustPressed())
        {
            Sort();
        }
        else if (_keys.FocusSearch.JustPressed())
        {
            FocusSearch();
        }
        else if (_keys.ScrollUp.JustPressed())
        {
            (Game1.activeClickableMenu as IScrollableMenu)?.ScrollUp();
        }
        else if (_keys.ScrollDown.JustPressed())
        {
            (Game1.activeClickableMenu as IScrollableMenu)?.ScrollDown();
        }
        else if (_keys.PageUp.JustPressed())
        {
            (Game1.activeClickableMenu as IScrollableMenu)?.ScrollUp(Game1.activeClickableMenu.height);
        }
        else if (_keys.PageDown.JustPressed())
        {
            (Game1.activeClickableMenu as IScrollableMenu)?.ScrollDown(Game1.activeClickableMenu.height);
        }
        else if (_keys.NextMetric.JustPressed())
        {
            OnNextMetric(this, new ChangedPageArgs(_metricsTitles[_currentMetricIndex]));
        }
        else if (_keys.PreviousMetric.JustPressed())
        {
            OnPreviousMetric(this, new ChangedPageArgs(_metricsTitles[_currentMetricIndex]));
        }
    }

    private void OnNextMetric(object? sender, ChangedPageArgs e)
    {
        if (Game1.activeClickableMenu is not ProductionMenu menu)
        {
            Monitor.Log("Next metric can't be applied on this menu.");
            return;
        }

        // update new metric index
        _currentMetricIndex = (_currentMetricIndex + 1) % 4;
        Func<IEnumerable<ItemStock>> metric = _metricOrders[_currentMetricIndex];
        IEnumerable<ItemStock> production = metric.Invoke();

        ShowProductionMenuFor(production, _metricsTitles[_currentMetricIndex]);
    }

    private void OnPreviousMetric(object? sender, ChangedPageArgs e)
    {
        if (Game1.activeClickableMenu is not ProductionMenu menu)
        {
            Monitor.Log("Next metric can't be applied on this menu.");
            return;
        }

        // update new metric index
        _currentMetricIndex = (_currentMetricIndex - 1 < 0 ? 3 : _currentMetricIndex - 1) % 4;
        Func<IEnumerable<ItemStock>> metricFunc = _metricOrders[_currentMetricIndex];
        IEnumerable<ItemStock> production = metricFunc.Invoke();

        ShowProductionMenuFor(production, _metricsTitles[_currentMetricIndex]);
    }

    private void ToggleProductionMenu()
    {
        if (Game1.activeClickableMenu is ProductionMenu)
        {
            HideMenu();
            return;
        }
        ShowProductionMenu();
    }

    private void ShowProductionMenu()
    {
        Monitor.Log("Received a open production menu request");
        try
        {
            // get method responsible for getting items using selected metric.
            Func<IEnumerable<ItemStock>> metricFunc = _metricOrders[_currentMetricIndex];

            // get items
            IEnumerable<ItemStock> production = metricFunc.Invoke();
            if (production.Any() == false)
            {
                Monitor.Log($"Nothing got produced.");
                return;
            }

            // show production UI
            Monitor.Log($"Found {production.Count()} items to show");
            ShowProductionMenuFor(production, _metricsTitles[_currentMetricIndex]);
        }
        catch (Exception ex)
        {
            Monitor.Log($"An error occurred. {ex.Message}");
            throw;
        }
    }

    private void ShowProductionMenuFor(IEnumerable<ItemStock> production, string title)
    {
        production.Select(x => $"Showing {x.Item}::{x.Count}")
            .ToList()
            .ForEach(x => Monitor.Log(x));

        ProductionMenu menu = new(
            production: production,
            title: title,
            monitor: Monitor,
            reflectionHelper: Helper.Reflection,
            scroll: 160,
            forceFullScreen: false);

        // hookup events
        menu.ChangedToNextPage += OnNextMetric;
        menu.ChangedToPreviousPage += OnPreviousMetric;
        menu.Closing += OnMetricsClosing;

        Game1.displayHUD = false;

        PushMenu(menu);
    }

    private void OnMetricsClosing(object? sender, EventArgs e)
    {
        Game1.displayHUD = true;
    }

    private void FocusSearch()
    {
        if (Game1.activeClickableMenu is ItemMenu itemMenu)
        {
            itemMenu.FocusSearch();
            return;
        }
        else if (Game1.activeClickableMenu is ProductionMenu prodMenu)
        {
            prodMenu.FocusSearch();
            return;
        }

        Monitor.Log("Focus search can't be applied on this menu.");
        return;
    }

    private void Sort()
    {
        if (Game1.activeClickableMenu is not ItemMenu menu)
        {
            Monitor.Log("Sort can't be applied on this menu.");
            return;
        }

        // If we would allow sorting while focused on search textbox
        // would never print letter S in textbox.
        if (menu.IsSearchTextBoxFocused)
        {
            return;
        }

        // sort items
        SortOrder sortOrder = _sortOrders.Dequeue();

        menu.ApplySort(sortOrder);
        HUDMessage message = new($"View sorted by {sortOrder.GetDescription()}", 500f)
        {
            noIcon = true,
        };

        Game1.addHUDMessage(message);
        _sortOrders.Enqueue(sortOrder);
    }

    private void ToggleMenu()
    {
        if (Game1.activeClickableMenu is ItemMenu)
        {
            HideMenu();
            return;
        }
        ShowMenu();
    }

    private void ShowMenu()
    {
        Monitor.Log("Received a open menu request");
        try
        {
            // get items
            IEnumerable<ItemStock> items = GetItemSubjects();
            if (items.Any() == false)
            {
                Monitor.Log($"No items found.");
                return;
            }

            // show lookup UI
            Monitor.Log($"Found {items.Count()} items to show.");
            ShowMenuFor(items);
        }
        catch (Exception ex)
        {
            Monitor.Log($"An error occurred. {ex.Message}");
            throw;
        }
    }

    private void ShowMenuFor(IEnumerable<ItemStock> items)
    {
        items.Select(x => $"Showing {x.GetType().Name}::{x.Item.Name}.")
            .ToList()
            .ForEach(x => Monitor.Log(x));

        ItemMenu menu = new(
            itemStocks: items,
            monitor: Monitor,
            reflectionHelper: Helper.Reflection,
            scroll: 160,
            forceFullScreen: false);

        PushMenu(menu);
    }

    /// <summary>
    ///     Push a new menu onto the display stack, 
    ///     saving the previous menu if needed.
    /// </summary>
    /// <param name="menu">The menu to show.</param>
    private void PushMenu(IClickableMenu menu)
    {
        if (ShouldRestoreMenu(Game1.activeClickableMenu))
        {
            _previousMenus.Value.Push(Game1.activeClickableMenu);
            Helper.Reflection
                .GetField<IClickableMenu>(typeof(Game1), "_activeClickableMenu")
                // bypass Game1.activeClickableMenu, which disposes the previous menu
                .SetValue(menu);
        }
        else
        {
            Game1.activeClickableMenu = menu;
        }
    }

    /// <summary>
    ///     Get whether a given menu should be restored 
    ///     when the lookup ends.
    /// </summary>
    /// <param name="menu">The menu to check.</param>
    private static bool ShouldRestoreMenu(IClickableMenu? menu)
    {
        return menu switch
        {
            null => false, // no menu
            ItemMenu => false,
            ProductionMenu => false,
            _ => true,
        };
    }

    private IEnumerable<ItemStock> GetItemSubjects()
    {
        IEnumerable<Item> items = _chestFinder.GetChests()
                .Select(x => x.GetItemsForCurrentPlayer())
                .SelectMany(x => x) // Make list flat 
                .Concat(Game1.player.Items)
                .Where(x => x is not null);

        Dictionary<string, ItemStock> result = [];
        foreach (Item item in items)
        {
            if (result.ContainsKey(item.Name) == false)
            {
                result[item.Name] = new ItemStock(item);
            }
            result[item.Name].Count += item.Stack;
        }

        return result.Values;
    }

    /// <summary>Hide the lookup UI for the current target.</summary>
    private static void HideMenu()
    {
        if (Game1.activeClickableMenu is ItemMenu itemMenu)
        {
            itemMenu.QueueExit();
            Game1.displayHUD = true;
        }
        else if (Game1.activeClickableMenu is ProductionMenu prodMenu)
        {
            prodMenu.QueueExit();
            Game1.displayHUD = true;
        }
    }
}
