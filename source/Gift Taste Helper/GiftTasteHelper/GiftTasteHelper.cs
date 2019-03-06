using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GiftTasteHelper.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace GiftTasteHelper
{
    internal class GiftTasteHelper : Mod
    {
        /*********
        ** Properties
        *********/
        private ModConfig Config;
        private Dictionary<Type, IGiftHelper> GiftHelpers;
        private IGiftHelper CurrentGiftHelper;
        private IGiftDatabase GiftDatabase;
        private IGiftMonitor GiftMonitor;
        private bool ReloadHelpers = false;
        private bool WasResized;
        private bool CheckGiftGivenNextInput = false;

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

            InitDebugCommands(this.Helper);

            Startup();
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
            this.GiftMonitor.Load();

            if (this.ReloadHelpers)
            {
                Utils.DebugLog("Reloading gift helpers");
                LoadGiftHelpers(Helper);
                this.ReloadHelpers = false;
            }
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
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.dayOfMonth == 1 && this.GiftHelpers.ContainsKey(typeof(Billboard)))
            {
                // Reset the birthdays when season changes
                this.GiftHelpers[typeof(Billboard)].Reset();
            }
            this.GiftMonitor.Reset();
        }

        private void LoadGiftHelpers(IModHelper helper)
        {
            Utils.DebugLog("Initializing gift helpers");

            // TODO: add a reload method to the gift helpers instead of fully re-creating them
            // Force the gift info to be rebuilt
            IGiftDataProvider dataProvider = null;
            if (Config.ShowOnlyKnownGifts)
            {
                // The prefix is purely for convenience. Mostly so I know which is which.
                string prefix = new string(Game1.player.Name.Where(char.IsLetterOrDigit).ToArray());
                string path = this.Config.ShareKnownGiftsWithAllSaves
                    ? Path.Combine(StoredGiftDatabase.DBRoot, StoredGiftDatabase.DBFileName)
                    : Path.Combine(StoredGiftDatabase.DBRoot, $"{prefix}_{Game1.player.UniqueMultiplayerID}", StoredGiftDatabase.DBFileName);

                GiftDatabase = new StoredGiftDatabase(helper, path);

                if (this.ModManifest.Version.IsNewerThan("2.7") && !this.Config.ShareKnownGiftsWithAllSaves)
                {
                    string oldPath = Path.Combine(StoredGiftDatabase.DBRoot, Constants.SaveFolderName, StoredGiftDatabase.DBFileName);
                    string fullOldPath = Path.Combine(helper.DirectoryPath, oldPath);
                    if (File.Exists(fullOldPath))
                    {
                        Utils.DebugLog($"Found old DB at {oldPath}. Migrating to {path}.", LogLevel.Info);
                        StoredGiftDatabase dbRef = (StoredGiftDatabase)GiftDatabase;
                        StoredGiftDatabase.MigrateDatabase(helper, oldPath, ref dbRef);
                    }
                }

                dataProvider = new ProgressionGiftDataProvider(GiftDatabase);
                helper.Events.Input.ButtonPressed += CheckGift_OnButtonPressed;
            }
            else
            {
                GiftDatabase = new GiftDatabase(helper);
                dataProvider = new AllGiftDataProvider(GiftDatabase);
                helper.Events.Input.ButtonPressed -= CheckGift_OnButtonPressed;
            }

            // Add the helpers if they're enabled in config
            CurrentGiftHelper = null;
            this.GiftHelpers = new Dictionary<Type, IGiftHelper>();
            if (Config.ShowOnCalendar)
            {
                this.GiftHelpers.Add(typeof(Billboard), new CalendarGiftHelper(dataProvider, Config, helper.Reflection, helper.Translation));
            }
            if (Config.ShowOnSocialPage)
            {
                this.GiftHelpers.Add(typeof(GameMenu), new SocialPageGiftHelper(dataProvider, Config, helper.Reflection, helper.Translation));
            }

            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        #region Gift Monitor Handling
        // ===================================================================
        // Gift Monitor Handling
        // ===================================================================

        private void OnGiftGiven(string npc, int itemId)
        {
            var taste = Utils.GetTasteForGift(npc, itemId);
            if (taste != GiftTaste.MAX)
            {
                Utils.DebugLog($"Gift given to Npc: {npc} | item: {itemId} | taste: {taste}");
                this.GiftDatabase.AddGift(npc, itemId, taste);
            }

            this.CheckGiftGivenNextInput = false;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void CheckGift_OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Giving a gift with a controller is done on button down so we can't use the same
            // trick to do the check if the gift was given. Since a dialogue is always opened after giving a gift
            // the user has to press something to proceed so it's during that input that we'll do the check (if the flag is set).
            if (this.CheckGiftGivenNextInput)
                this.GiftMonitor.CheckGiftGiven();

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
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
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

            if (this.GiftHelpers.ContainsKey(newMenuType))
            {
                // Close the current gift helper
                if (this.CurrentGiftHelper != null)
                {
                    Utils.DebugLog("[OnClickableMenuChanged] Closing current helper: " + this.CurrentGiftHelper.GetType());

                    UnsubscribeEvents();

                    this.CurrentGiftHelper.OnClose();
                }

                this.CurrentGiftHelper = this.GiftHelpers[newMenuType];
                if (!this.CurrentGiftHelper.IsInitialized)
                {
                    Utils.DebugLog("[OnClickableMenuChanged initialized helper: " + this.CurrentGiftHelper.GetType());

                    this.CurrentGiftHelper.Init(e.NewMenu);
                }

                if (this.CurrentGiftHelper.OnOpen(e.NewMenu))
                {
                    Utils.DebugLog("[OnClickableMenuChanged Successfully opened helper: " + this.CurrentGiftHelper.GetType());

                    // Only subscribe to the events if it opened successfully
                    SubscribeEvents();
                }
            }
        }
        #endregion Menu Handling

        /// <summary>Raised after the player moves the in-game cursor.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnCursorMoved(object sender, CursorMovedEventArgs e)
        {
            Debug.Assert(this.CurrentGiftHelper != null, "OnCursorMoved listener invoked when currentGiftHelper is null.");

            if (this.CurrentGiftHelper.CanTick())
            {
                this.CurrentGiftHelper.OnCursorMoved(e);
            }
        }

        /// <summary>Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRendered(object sender, RenderedEventArgs e)
        {
            Debug.Assert(this.CurrentGiftHelper != null, "OnPostRenderEvent listener invoked when currentGiftHelper is null.");

            if (this.CurrentGiftHelper.CanDraw())
            {
                this.CurrentGiftHelper.OnDraw();
            }
        }

        private void UnsubscribeEvents()
        {
            Helper.Events.Input.CursorMoved -= OnCursorMoved;
            Helper.Events.Display.Rendered -= this.OnRendered;
        }

        private void SubscribeEvents()
        {
            Helper.Events.Input.CursorMoved += OnCursorMoved;
            Helper.Events.Display.Rendered += this.OnRendered;
        }

        #region Debug
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
                    this.GiftMonitor.Reset();
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
                helper.ConsoleCommands.Trigger("world_settime", new string[] { "1000" });
                helper.ConsoleCommands.Trigger("teleport", new string[] { "SamHouse", "306", "339" });
                var items = new int[] { 74, 773, 417, 324, 92, 220, 176, 417, 404, 22, 18 }; // Test items for all of jodi's tastes.
                foreach (var item in items)
                {
                    helper.ConsoleCommands.Trigger("player_add", new string[] { "Object", item.ToString(), "10" });
                }
            });
#endif
        }
        #endregion
    }
}
