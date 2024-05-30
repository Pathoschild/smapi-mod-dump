/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/GiftTasteHelper
**
*************************************************/

using GenericModConfigMenu;
using GiftTasteHelper.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace GiftTasteHelper
{
    internal class GiftTasteHelper : Mod
    {
        internal class MultiplayerContext
        {
            public Dictionary<Type, IGiftHelper> GiftHelpers = new();
            public IGiftHelper? CurrentGiftHelper = null;
            public bool Active = false;
        }

        /*********
        ** Properties
        *********/
        private ModConfig Config = new();
        private readonly Dictionary<long, MultiplayerContext> MultiplayerGiftHelperMap = new();
        private Dictionary<Type, IGiftHelper> GiftHelpers 
        { 
            get
            {
                if (MultiplayerGiftHelperMap.TryGetValue(Game1.player.UniqueMultiplayerID, out var context))
                {
                    return context.GiftHelpers;
                }
                else
                {
                    return new();
                }
            }
            set
            {
                if (MultiplayerGiftHelperMap.TryGetValue(Game1.player.UniqueMultiplayerID, out var context))
                {
                    context.GiftHelpers = value;
                }
                else
                {
                    MultiplayerGiftHelperMap[Game1.player.UniqueMultiplayerID] = new() 
                    { 
                        GiftHelpers = value
                    };
                }
            }
        }
        private IGiftHelper? CurrentGiftHelper
        {
            get
            {
                if (MultiplayerGiftHelperMap.TryGetValue(Game1.player.UniqueMultiplayerID, out var context))
                {
                    return context.CurrentGiftHelper;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (MultiplayerGiftHelperMap.TryGetValue(Game1.player.UniqueMultiplayerID, out var context))
                {
                    context.CurrentGiftHelper = value;
                }
                else
                {
                    MultiplayerGiftHelperMap[Game1.player.UniqueMultiplayerID] = new()
                    {
                        CurrentGiftHelper = value
                    };
                }
            }
        }
        private IGiftDatabase? GiftDatabase;
        private IGiftMonitor? GiftMonitor;
        private IGiftDataProvider? DataProvider;
        private bool ReloadHelpers = false;
        private bool WasResized;
        private bool CheckGiftGivenNextInput = false;
        private bool IsActiveMenu
        {
            get
            {
                if (MultiplayerGiftHelperMap.TryGetValue(Game1.player.UniqueMultiplayerID, out var context))
                {
                    return context.Active;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (MultiplayerGiftHelperMap.TryGetValue(Game1.player.UniqueMultiplayerID, out var context))
                {
                    context.Active = value;
                }
                else
                {
                    MultiplayerGiftHelperMap[Game1.player.UniqueMultiplayerID] = new()
                    {
                        Active = value
                    };
                }
            }
        }

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Any logic in here is stuff that should only be done once. Anything that needs 
            // to be reset/reload when we 'hot-reload' should go in Startup().

            // Set the monitor ref so we can have a cheeky global log function
            Utils.InitLog(this.Monitor);

            helper.Events.Display.WindowResized += (sender, e) => this.WasResized = true;
            helper.Events.GameLoop.SaveLoaded += (sender, e) => Initialize();
            helper.Events.GameLoop.ReturnedToTitle += (sender, e) => Shutdown();
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.GameLaunched += RegisterConfigMenu;
            helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
            helper.Events.Multiplayer.PeerDisconnected += OnPeerDisconnected;

            InitDebugCommands(this.Helper);
            Startup();
        }

        private void OnPeerDisconnected(object? sender, PeerDisconnectedEventArgs e)
        {
            MultiplayerGiftHelperMap.Remove(e.Peer.PlayerID);
        }

        private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
        {
            var context = new MultiplayerContext();
            RebuildGiftHelpers(Helper, context);
            MultiplayerGiftHelperMap.Add(e.Peer.PlayerID, context);
        }

        // Called when the mod starts up or is being hot-reloaded.
        private void Startup()
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            this.GiftMonitor = new GiftMonitor();
            this.GiftMonitor.GiftGiven += OnGiftGiven;

            // Wait until after the save is loaded before loading the helpers.
            this.ReloadHelpers = true;
        }

        // Must be called after a save has been loaded.
        private void Initialize()
        {
            this.GiftMonitor?.Load();

            if (this.ReloadHelpers)
            {
                Utils.DebugLog("Reloading gift helpers");
                this.ReloadHelpers = false;

                var context = new MultiplayerContext();
                RebuildDataProvider(Helper, context);
                MultiplayerGiftHelperMap[Game1.player.UniqueMultiplayerID] = context;
            }
        }

        private Action<T> RebuildDatabaseAfterAction<T>(Action<T> action)
        {
            return new Action<T>(value =>
            {
                action.Invoke(value);
                Shutdown();
                if (Context.IsWorldReady)
                {
                    Initialize();
                }
            });
        }

        private void RegisterConfigMenu(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => Helper.Translation.Get("options.display")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("options.showOnCalendar"),
                tooltip: () => Helper.Translation.Get("options.showOnCalendar.desc"),
                getValue: () => this.Config.ShowOnCalendar,
                setValue: RebuildDatabaseAfterAction<bool>(value => this.Config.ShowOnCalendar = value)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("options.showOnSocialPage"),
                tooltip: () => Helper.Translation.Get("options.showOnSocialPage.desc"),
                getValue: () => this.Config.ShowOnSocialPage,
                setValue: RebuildDatabaseAfterAction<bool>(value => this.Config.ShowOnSocialPage = value)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("options.showOnlyKnownGifts"),
                tooltip: () => Helper.Translation.Get("options.showOnlyKnownGifts.desc"),
                getValue: () => this.Config.ShowOnlyKnownGifts,
                setValue: RebuildDatabaseAfterAction<bool>(value => this.Config.ShowOnlyKnownGifts = value)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("options.showUniversalGifts"),
                tooltip: () => Helper.Translation.Get("options.showUniversalGifts.desc"),
                getValue: () => this.Config.ShowUniversalGifts,
                setValue: value => this.Config.ShowUniversalGifts = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("options.showGiftsForUnmetNPCs"),
                tooltip: () => Helper.Translation.Get("options.showGiftsForUnmetNPCs.desc"),
                getValue: () => this.Config.ShowGiftsForUnmetNPCs,
                setValue: value => this.Config.ShowGiftsForUnmetNPCs = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("options.hideTooltipWhenNoGiftsKnown"),
                tooltip: () => Helper.Translation.Get("options.hideTooltipWhenNoGiftsKnown.desc"),
                getValue: () => this.Config.HideTooltipWhenNoGiftsKnown,
                setValue: value => this.Config.HideTooltipWhenNoGiftsKnown = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("options.colorizeUniversalGiftNames"),
                tooltip: () => Helper.Translation.Get("options.colorizeUniversalGiftNames.desc"),
                getValue: () => this.Config.ColorizeUniversalGiftNames,
                setValue: value => this.Config.ColorizeUniversalGiftNames = value
            );

            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => Helper.Translation.Get("options.other")
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("options.maxGiftsToDisplay"),
                tooltip: () => Helper.Translation.Get("options.maxGiftsToDisplay.desc"),
                min: 0,
                interval: 1,
                getValue: () => this.Config.MaxGiftsToDisplay,
                setValue: value => this.Config.MaxGiftsToDisplay = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("options.shareKnownGiftsWithAllSaves"),
                tooltip: () => Helper.Translation.Get("options.shareKnownGiftsWithAllSaves.desc"),
                getValue: () => this.Config.ShareKnownGiftsWithAllSaves,
                setValue: RebuildDatabaseAfterAction<bool>(value => this.Config.ShareKnownGiftsWithAllSaves = value)
            );
        }

        private void Shutdown()
        {
            // A different save may be chosen which can have different progress, so we need to reload the db and helpers.
            UnsubscribeEvents();
            this.ReloadHelpers = true;
            this.CurrentGiftHelper = null;

            Helper.Events.Display.MenuChanged -= OnMenuChanged;
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (Game1.dayOfMonth == 1 && this.GiftHelpers.ContainsKey(typeof(Billboard)))
            {
                // Reset the birthdays when season changes
                this.GiftHelpers[typeof(Billboard)].Reset();
            }
            this.GiftMonitor?.Reset();
        }

        private void RebuildDataProvider(IModHelper helper, MultiplayerContext context)
        {
            Utils.DebugLog("Initializing gift helpers");

            // TODO: add a reload method to the gift helpers instead of fully re-creating them
            // Force the gift info to be rebuilt
            DataProvider = null;
            if (Config.ShowOnlyKnownGifts)
            {
                // The prefix is purely for convenience. Mostly so I know which is which.
                string prefix = new(Game1.player.Name.Where(char.IsLetterOrDigit).ToArray());
                string path = this.Config.ShareKnownGiftsWithAllSaves
                    ? Path.Combine(StoredGiftDatabase.DBRoot, StoredGiftDatabase.DBFileName)
                    : Path.Combine(StoredGiftDatabase.DBRoot, $"{prefix}_{Game1.player.UniqueMultiplayerID}", StoredGiftDatabase.DBFileName);

                GiftDatabase = new StoredGiftDatabase(helper, path);

                if (!this.Config.ShareKnownGiftsWithAllSaves)
                {
                    var folderName = Constants.SaveFolderName;
                    if (folderName is not null)
                    {
                        string oldPath = Path.Combine(StoredGiftDatabase.DBRoot, folderName, StoredGiftDatabase.DBFileName);
                        string fullOldPath = Path.Combine(helper.DirectoryPath, oldPath);
                        if (File.Exists(fullOldPath))
                        {
                            Utils.DebugLog($"Found old DB at {oldPath}. Migrating to {path}.", LogLevel.Info);
                            StoredGiftDatabase dbRef = (StoredGiftDatabase)GiftDatabase;
                            StoredGiftDatabase.MigrateDatabase(helper, oldPath, ref dbRef);
                        }
                    }
                }

                DataProvider = new ProgressionGiftDataProvider(GiftDatabase);
                helper.Events.Input.ButtonPressed += CheckGift_OnButtonPressed;
            }
            else
            {
                GiftDatabase = new GiftDatabase(helper);
                DataProvider = new AllGiftDataProvider(GiftDatabase);
                helper.Events.Input.ButtonPressed -= CheckGift_OnButtonPressed;
            }

            RebuildGiftHelpers(helper, context);
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void RebuildGiftHelpers(IModHelper helper, MultiplayerContext context)
        {
            if (DataProvider is null)
            {
                Utils.DebugLog($"{nameof(RebuildGiftHelpers)} called when {nameof(DataProvider)} is null", LogLevel.Debug);
                return;
            }

            // Add the helpers if they're enabled in config
            context.CurrentGiftHelper = null;
            context.GiftHelpers = new Dictionary<Type, IGiftHelper>();
            if (Config.ShowOnCalendar)
            {
                context.GiftHelpers.Add(typeof(Billboard), new CalendarGiftHelper(DataProvider, Config, helper.Reflection, helper.Translation));
            }
            if (Config.ShowOnSocialPage)
            {
                context.GiftHelpers.Add(typeof(GameMenu), new SocialPageGiftHelper(DataProvider, Config, helper.Reflection, helper.Translation));
            }
        }

        private bool HasActiveMenu()
        {
            foreach (var context in MultiplayerGiftHelperMap.Values)
            {
                if (context.Active)
                {
                    return true;
                }
            }

            return false;
        }

        #region Gift Monitor Handling
        // ===================================================================
        // Gift Monitor Handling
        // ===================================================================

        private void OnGiftGiven(string npc, string itemId)
        {
            var taste = Utils.GetTasteForGift(npc, itemId);
            if (taste != GiftTaste.MAX)
            {
                Utils.DebugLog($"Gift given to Npc: {npc} | item: {itemId} | taste: {taste}");
                this.GiftDatabase?.AddGift(npc, itemId, taste);
            }

            this.CheckGiftGivenNextInput = false;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void CheckGift_OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (this.GiftMonitor is null)
            {
                return;
            }

            // Giving a gift with a controller is done on button down so we can't use the same
            // trick to do the check if the gift was given. Since a dialogue is always opened after giving a gift
            // the user has to press something to proceed so it's during that input that we'll do the check (if the flag is set).
            if (this.CheckGiftGivenNextInput)
            {
                this.GiftMonitor.CheckGiftGiven();
            }

            switch (e.Button)
            {
                case SButton.ControllerA:
                    this.GiftMonitor.UpdateHeldGift();
                    this.CheckGiftGivenNextInput = this.GiftMonitor.IsHoldingValidGift;
                    break;

                case SButton.MouseLeft:
                    this.GiftMonitor.CheckGiftGiven();
                    break;

                case SButton.MouseRight:
                    this.GiftMonitor.UpdateHeldGift();
                    break;
            }
        }
        #endregion Gift Monitor Handling

        #region Menu Handling
        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (Game1.activeClickableMenu != e.NewMenu)
            {
                return;
            }

            // menu closed
            if (e.NewMenu == null)
            {
                if (this.CurrentGiftHelper != null)
                {
                    Utils.DebugLog("Closing current helper: " + this.CurrentGiftHelper.GetType());
                    UnsubscribeEvents();
                    this.CurrentGiftHelper.OnClose();
                }
                return;
            }

            // menu opened/changed
            Type newMenuType = e.NewMenu.GetType();
            if (this.WasResized && this.CurrentGiftHelper != null && this.CurrentGiftHelper.IsOpen && e.OldMenu?.GetType() == newMenuType)
            {
                // resize event
                Utils.DebugLog("[OnClickableMenuChanged] Invoking resize event on helper: " + this.CurrentGiftHelper.GetType());

                this.CurrentGiftHelper.OnResize(e.NewMenu);
                this.WasResized = false;
                return;
            }
            this.WasResized = false;

            // close SocialPageGiftHelper if profile menu is opened
            if (newMenuType == typeof(ProfileMenu) && this.CurrentGiftHelper != null && this.CurrentGiftHelper is SocialPageGiftHelper)
            {
                Utils.DebugLog("Closing current helper: " + this.CurrentGiftHelper.GetType());
                UnsubscribeEvents();
                this.CurrentGiftHelper.OnClose();
                return;
            }


            if (this.GiftHelpers.TryGetValue(newMenuType, out var helper))
            {
                // Close the current gift helper
                if (this.CurrentGiftHelper != null)
                {
                    Utils.DebugLog("[OnClickableMenuChanged] Closing current helper: " + this.CurrentGiftHelper.GetType());

                    UnsubscribeEvents();

                    this.CurrentGiftHelper.OnClose();
                }

                this.CurrentGiftHelper = helper;
                if (!this.CurrentGiftHelper.IsInitialized)
                {
                    Utils.DebugLog("[OnClickableMenuChanged] initialized helper: " + this.CurrentGiftHelper.GetType());

                    this.CurrentGiftHelper.Init(e.NewMenu);
                }

                if (this.CurrentGiftHelper.OnOpen(e.NewMenu))
                {
                    Utils.DebugLog("[OnClickableMenuChanged] Successfully opened helper: " + this.CurrentGiftHelper.GetType());

                    // Only subscribe to the events if it opened successfully
                    SubscribeEvents();
                }
            }
        }
        #endregion Menu Handling

        /// <summary>Raised after the player moves the in-game cursor.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnCursorMoved(object? sender, CursorMovedEventArgs e)
        {
            // This is now supported after changing settings in-game
            // Debug.Assert(this.CurrentGiftHelper != null, "OnCursorMoved listener invoked when currentGiftHelper is null.");

            if (IsActiveMenu && this.CurrentGiftHelper != null && this.CurrentGiftHelper.CanTick())
            {
                this.CurrentGiftHelper.OnCursorMoved(e);
            }
        }

        /// <summary>Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRendered(object? sender, RenderedActiveMenuEventArgs e)
        {
            // This is now supported after changing settings in-game
            // Debug.Assert(this.CurrentGiftHelper != null, "OnPostRenderEvent listener invoked when currentGiftHelper is null.");

            if (IsActiveMenu && this.CurrentGiftHelper != null && this.CurrentGiftHelper.CanDraw())
            {
                this.CurrentGiftHelper.OnDraw();
            }
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            // This is now supported after changing settings in-game
            // Debug.Assert(this.CurrentGiftHelper != null, "OnUpdateTicked listener invoked when currentGiftHelper is null.");

            if (IsActiveMenu && this.CurrentGiftHelper != null && this.CurrentGiftHelper.CanTick())
            {
                this.CurrentGiftHelper.OnPostUpdate(e);
            }
        }

        private void UnsubscribeEvents()
        {
            IsActiveMenu = false;

            if (!HasActiveMenu())
            {
                Helper.Events.Input.CursorMoved -= OnCursorMoved;
                Helper.Events.Display.RenderedActiveMenu -= this.OnRendered;
                Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            }
        }

        private void SubscribeEvents()
        {
            if (!HasActiveMenu())
            {
                Helper.Events.Input.CursorMoved += OnCursorMoved;
                Helper.Events.Display.RenderedActiveMenu += OnRendered;
                Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            }

            IsActiveMenu = true;
        }

        #region Debug
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "DEBUG")]
        void InitDebugCommands(IModHelper helper)
        {
#if DEBUG
            helper.ConsoleCommands.Add("reload", "Reload config", (name, args) =>
            {
                Shutdown();
                Startup();
                Initialize();
            });

            helper.ConsoleCommands.Add("printinfo", "Print debug info", (name, args) =>
            {
                Utils.DebugLog("==== Printing Debug info =====");
                foreach (var farmhand in Game1.getAllFarmers())
                {
                    Utils.DebugLog($"Farmer name: {farmhand.Name} | unique Id: {farmhand.UniqueMultiplayerID}");
                }
            });

            helper.ConsoleCommands.Add("resetgifts", "Reset gifts", (name, args) =>
            {
                foreach (var friendship in Game1.player.friendshipData.Pairs)
                {
                    friendship.Value.GiftsThisWeek = 0;
                    friendship.Value.GiftsToday = 0;
                    this.GiftMonitor?.Reset();
                }
            });

            helper.ConsoleCommands.Add("printcoords", "asdf", (name, args) =>
            {
                Utils.DebugLog($"Player coords: {Game1.player.position} | location: {Game1.player.currentLocation.Name}");
            });

            helper.ConsoleCommands.Add("teleport", "", (name, args) =>
            {
                string location = args.Length > 0 ? args[0] : "Town";
                int x = 2590, y = 3650; // in front of billboard
                if (location.ToLower() == "home")
                {
                    location = "FarmHouse";
                    x = (int)Game1.player.mostRecentBed.X;
                    y = (int)Game1.player.mostRecentBed.Y;
                }
                else if (args.Length == 3)
                {
                    try
                    {
                        x = int.Parse(args[1]);
                        y = int.Parse(args[2]);
                    }
                    catch (Exception)
                    {
                        Utils.DebugLog("Error parsing params", LogLevel.Error);
                    }
                }
                Game1.warpFarmer(location, x / Game1.tileSize, y / Game1.tileSize, false);
            });

            helper.ConsoleCommands.Add("setup", "", (name, args) =>
            {
                Game1.timeOfDay = 1000;
                Game1.warpFarmer("SamHouse", 306, 339, false);
                // DebugCommands.TryHandle(new[] { "world_settime", "1000" });
                // DebugCommands.TryHandle(new[] { "teleport", "SamHouse", "306", "339" });
                var items = new[] { "74", "773", "417", "324", "92", "220", "176", "417", "404", "22", "18" }; // Test items for all of jodi's tastes.
                foreach (var item in items)
                {
                    Game1.player.addItemToInventory(ItemRegistry.Create(item, 10));
                    // DebugCommands.TryHandle(new[] { "player_add", "Object", item.ToString(), "10" });
                }
            });
#endif
        }
        #endregion
    }
}
