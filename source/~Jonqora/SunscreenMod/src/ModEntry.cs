using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.IO;
using static SunscreenMod.Flags;

namespace SunscreenMod
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        internal static ModEntry Instance { get; private set; }

        /// <summary>Json Assets API instance.</summary>
        internal JsonAssets.IApi JA { get; private set; }

        /// <summary>Whether the next tick is the first one.</summary>
        private bool IsFirstTick = true;


        //Class instance fields to be accessible
        /// <summary>Data about villagers who react to a sunburn.</summary>
        private Reactions Reacts;

        /// <summary>Manages in-game behaviour of lotion products.</summary>
        public Lotions Lotion;

        /// <summary>Data about sunscreen application and status.</summary>
        public SunscreenProtection Sunscreen;

        /// <summary>Data about sunburn level, new burn, sun damage, and skin color.</summary>
        public Sunburn Burn;


        /// <summary>Whether the save file is loaded.</summary>
        internal bool IsSaveReady = false;

        /// <summary>Cumulative total available UV exposure, reset each day. (Only for debug logs, doesn't affect player.)</summary>
        private int TotalUVExposure = 0;


        /*********
        ** Accessors
        *********/
        internal static ModConfig Config => ModConfig.Instance;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Make resources available.
            Instance = this;
            ModConfig.Load();

            // Add console commands.
            ConsoleCommands.Apply();

            // Listen for game events.
            helper.Events.GameLoop.GameLaunched += this.onGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.onUpdateTicked;
            helper.Events.GameLoop.ReturnedToTitle += this.onReturnedToTitle;
            helper.Events.GameLoop.SaveLoaded += this.onSaveLoaded;
            helper.Events.GameLoop.DayStarted += this.onDayStarted;
            helper.Events.GameLoop.DayEnding += this.onDayEnding;
            helper.Events.GameLoop.UpdateTicked += this.onHalfSecondUpdateTicked;
            helper.Events.GameLoop.TimeChanged += this.onTimeChanged;
            helper.Events.Player.Warped += this.onWarped;
            helper.Events.Input.ButtonPressed += this.onButtonPressed;
        }


        /*********
        ** Private methods
        *********/
        /****
        ** GameLoop Event handlers
        ****/
        /// <summary>
        /// Set up API integrations and instatiate Lotions (to handle JA items). Raised after the game is launched.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ModConfig.SetUpMenu();

            JA = Helper.ModRegistry.GetApi<JsonAssets.IApi>("spacechase0.JsonAssets");
            if (JA != null)
            {
                JA.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets", "JA"));
            }
            else
            {
                Monitor.LogOnce("Could not connect to Json Assets. It may not be installed or working properly.", LogLevel.Error);
            }

            Lotion = new Lotions();
        }

        /// <summary>
        /// Loads asset editors on the second update tick, after Content Patcher has loaded most others.
        /// Raised after the game state is updated. Only runs twice then unhooks itself.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (this.IsFirstTick)
            {
                this.IsFirstTick = false;
            }
            else // Is second update tick
            {
                Instance.Helper.Events.GameLoop.UpdateTicked -= this.onUpdateTicked; // Don't check again

                // Set up asset loaders/editors to load after CP content packs
                Instance.Helper.Content.AssetEditors.Add(new UVIndex());
                Instance.Helper.Content.AssetEditors.Add(new SkinColors());
            }
        }

        /// <summary>
        /// Clears save-specific data for villager reactions, sunscreen, and sunburn. Raised when the player returns to title screen.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            //clear any previous data from other saves? Initialize zeros and all?
            IsSaveReady = false;

            Reacts = null;
            Sunscreen = null;
            Burn = null;
        }

        /// <summary>
        /// Re-initialize reacts, lotions, sunscreen and burn data. Refresh TV channel data. Raised when a save is loaded.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            //initialize new instances of Burn and Sunscreen
            Reacts = new Reactions();
            Lotion = new Lotions();
            Sunscreen = new SunscreenProtection();
            Burn = new Sunburn();

            Helper.Content.InvalidateCache("Strings\\StringsFromCSFiles"); //Refresh TV weather info

            IsSaveReady = true;
        }

        /// <summary>
        /// Raised at the start of a new day OR when a farmhand connects in multiplayer. Checks if it is actually a new day before performing daily tasks.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onDayStarted(object sender, DayStartedEventArgs e)
        {
            if (HasFlag("NewDay")) //Ensure once a day only, including for farmhands
            {
                DoNewDayStuff();
                //read data from mail flags for burn level and new burn
            }
        }

        /// <summary>
        /// Change skin color back to normal and add NewDay flag before saving player data. Raised when an in-game day ends.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onDayEnding(object sender, DayEndingEventArgs e)
        {
            //Clear burn debuffs? Nah, speed is the only true debuff enabled right now, and that clears itself.

            if (Game1.stats.DaysPlayed == 0) //Before the first day of the game
            {
                return;
            }

            //change skin color back to normal
            int? normalSkin = Burn.GetNormalSkinFlag();
            if (normalSkin != null)
            {
                int normalSkinIndex = (int)normalSkin - 1;
                Game1.player.changeSkinColor(normalSkinIndex, true);
            }

            AddFlag("NewDay");
            Lotion.HasAppliedAloeToday = false;
        }

        /// <summary>
        /// Prompt nearby villagers to react to a sunburnt player. Raised every update tick (runs every half-second, if a save is loaded).
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onHalfSecondUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!e.IsMultipleOf(30) || //Only run every half-second.
                !IsSaveReady) //Ignore this before a save is loaded.
            {
                return;
            }

            Sunscreen.UpdateStatus(); //Checks and updates if sunscreen has worn off or washed off (more timely alerts when swimming)

            if (Game1.activeClickableMenu == null &&
                Game1.currentMinigame == null &&
                !Game1.eventUp && !Game1.dialogueUp && //Don't do this during events or when you can't see
                Config.VillagerReactions && //Config setting
                Burn.IsSunburnt()) //Only react if sunburnt
            {
                Reacts.NearbyNPCsReact();
            }
        }

        /// <summary>
        /// Update suncreen status and check for damage from sun exposure. Raised when the game time changes (10-minute intervals).
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onTimeChanged(object sender, TimeChangedEventArgs e)
        {
            SDVTime time = SDVTime.CurrentTime;
            int UV = UVIndex.UVIntensityAt(time);
            int uvIndex = Convert.ToInt32((double)UV / 25);
            TotalUVExposure += UV;
            if (Config.DebugMode)
            {
                Monitor.Log($"Time is {time.Get12HourTime()} - current UV strength is {UV} or index {uvIndex}. Total exposure is {TotalUVExposure}", LogLevel.Debug);
            }

            //Possibly redundant? (Also in this.onHalfSecondUpdateTicked)
            Sunscreen.UpdateStatus(); //Checks and updates if sunscreen has worn off or washed off

            if (Config.EnableSunburn &&
                Config.SunburnPossible(SDate.Now()) && //Check config settings
                e.NewTime != e.OldTime && e.NewTime % 10 == 0 &&
                Game1.currentLocation.IsOutdoors &&
                !Sunscreen.IsProtected()) // Outdoors and not protected by sunscreen
            {
                Burn.CheckForBurnDamage(time);
            }
        }


        /****
        ** Player Event handlers
        ****/
        /// <summary>
        /// Clear the list of villagers who have already reacted to a sunburn. Raised when the player is warped to any location.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onWarped(object sender, WarpedEventArgs e)
        {
            if (!IsSaveReady)
            {
                return; //Ignore this before a save is loaded.
            }
            //clear the list of NPCs who have reacted (or initialize a new one??)
            Reacts.ClearReacts();
        }


        /****
        ** Input Event handlers
        ****/
        /// <summary>
        /// Allow the player to apply sunscreen or other lotion with right click. Raised when the player presses any button.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !IsSaveReady)
            {
                return; //Ignore button input before a save is loaded.
            }

            if (Game1.didPlayerJustRightClick() || //Did we use the right button to apply a lotion?
                (Constants.TargetPlatform == GamePlatform.Android && e.Button == SButton.MouseLeft)) //Android support
            {
                if (CanUseItem() && HoldingNonEdibleObject() && TappedOnFarmerIfAndroid(e.Cursor))
                {
                    Item itemToUse = (Item)Game1.player.ActiveObject;
                    if (Lotion.IsLotion(itemToUse))
                    {
                        Lotion.ApplyQuestion(itemToUse);
                        Helper.Input.Suppress(e.Button);
                    }
                }
            }
        }

        /****
        ** Helper Functions
        ****/

        #region functions for onButtonPressed
        /// <summary>
        /// Checks to make sure the player is not currently eating or in a menu, minigame, or event.
        /// </summary>
        /// <returns>true if player can use an item, false otherwise</returns>
        private bool CanUseItem()
        {
            return Game1.activeClickableMenu == null &&
                Game1.currentMinigame == null &&
                !Game1.eventUp && !Game1.dialogueUp && //Not in a menu or minigame or event or dialogue
                !Game1.player.isEating &&
                !Game1.player.canOnlyWalk && !Game1.player.FarmerSprite.PauseForSingleAnimation && !Game1.fadeToBlack; //Idk, vanilla code has em
        }

        /// <summary>Checks if the active item is a non-edible object.</summary>
        private bool HoldingNonEdibleObject()
        {
            return Game1.player.ActiveObject != null && //Player is holding something, it is an Object
                Game1.player.ActiveObject.Edibility == -300; //It is not an edible object
        }

        /// <summary>Checks if an android player's cursor is on their farmer.</summary>
        /// <returns>true if player did tap on farmer, or not on android; false otherwise</returns>
        private bool TappedOnFarmerIfAndroid(ICursorPosition cursor)
        {
            int x = (int)cursor.AbsolutePixels.X;
            int y = (int)cursor.AbsolutePixels.Y;
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                return new Rectangle((int)Game1.player.position.X, (int)Game1.player.position.Y - 85, 64, 125).Contains(x, y); //From android code
            }
            else
            {
                return true; //Game1.player.GetBoundingBox().Contains(x, y);
            }
        }
        #endregion functions for onButtonPressed

        /// <summary>
        /// Perform new day tasks, including:
        /// - refresh TV channel data
        /// - reset UV data and lotion data
        /// - obtain and update sunburn severity level
        /// - lose start of day health and energy if sunburnt
        /// - display a message if sunburnt or newly healed
        /// </summary>
        private void DoNewDayStuff()
        {
            if (Config.DebugMode) Monitor.Log("Doing new day stuff.", LogLevel.Info);
            Helper.Content.InvalidateCache("Strings\\StringsFromCSFiles"); //Refresh TV weather info

            TotalUVExposure = 0;

            Lotion.HasAppliedAloeToday = false;
            Sunscreen.RemoveSunscreen();

            int initialLevel = Burn.SunburnLevel; //Yesterday's sunburn severity level

            Burn.UpdateForNewDay(); //Includes a message about taking new damage or sunburn improving

            int burnLevel = Burn.SunburnLevel;
            Farmer who = Game1.player;

            //if has sunburn damage
            if (burnLevel > 0)
            {
                who.health -= Config.HealthLossPerLevel * burnLevel; //Lose health at the start of a day
                who.Stamina -= Config.EnergyLossPerLevel * burnLevel; //Lose energy at the start of a day
                Monitor.Log($"New day: Lost {Config.HealthLossPerLevel * burnLevel} health " +
                    $"and {Config.EnergyLossPerLevel * burnLevel} energy " +
                    $"due to sunburn damage.", LogLevel.Debug);
            }
            //if has sunburn, or healed existing sunburn from yesterday
            if (burnLevel > 0 || (initialLevel != burnLevel && burnLevel == 0)) Burn.DisplaySunburnStatus(); //Info: new sunburn or newly healed
            
            if (Config.DebugMode) Monitor.Log($"Current health: {who.health}/{who.maxHealth} | Current stamina: {who.Stamina}/{who.MaxStamina}", LogLevel.Info);

            RemoveFlag("NewDay");
        }
    }
}