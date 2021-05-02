/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Omegasis.SaveAnywhere.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;

namespace Omegasis.SaveAnywhere
{
    /// <summary>The mod entry point.</summary>
    public class SaveAnywhere : Mod
    {

        public static SaveAnywhere Instance;
        /*********
        ** Fields
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /// <summary>Provides methods for saving and loading game data.</summary>
        public SaveManager SaveManager;


        /// <summary>The parsed schedules by NPC name.</summary>
        private readonly IDictionary<string, string> NpcSchedules = new Dictionary<string, string>();

        /// <summary>Whether villager schedules should be reset now.</summary>
        private bool ShouldResetSchedules;

        /// <summary>Whether we're performing a non-vanilla save (i.e. not by sleeping in bed).</summary>
        public bool IsCustomSaving;

        /// <summary>Used to access the Mod's helper from other files associated with the mod.</summary>
        public static IModHelper ModHelper;

        /// <summary>Used to access the Mod's monitor to allow for debug logging in other files associated with the mod.</summary>
        public static IMonitor ModMonitor;

        private Dictionary<GameLocation, List<Monster>> monsters;

        private bool customMenuOpen;

        private bool firstLoad;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            this.SaveManager = new SaveManager(this.Helper, this.Helper.Reflection, onLoaded: () => this.ShouldResetSchedules = true);

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;
            helper.Events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;

            ModHelper = helper;
            ModMonitor = this.Monitor;
            this.customMenuOpen = false;
            Instance = this;
            this.firstLoad = false;
        }

        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            this.firstLoad = false;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // reset state
            this.ShouldResetSchedules = false;
            // load positions
            this.SaveManager.LoadData();
            //this.SaveManager.ClearData();

        }


        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {

            // let save manager run background logic
            if (Context.IsWorldReady)
            {
                if (!Game1.player.IsMainPlayer) return;
                this.SaveManager.Update();
            }



            if (Game1.activeClickableMenu == null && Context.IsWorldReady)
            {
                this.IsCustomSaving = false;
            }

            if (Game1.activeClickableMenu == null && !this.customMenuOpen) return;
            if (Game1.activeClickableMenu == null && this.customMenuOpen)
            {
                this.customMenuOpen = false;
                return;
            }
            if (Game1.activeClickableMenu != null)
            {
                if (Game1.activeClickableMenu.GetType() == typeof(NewSaveGameMenuV2))
                {
                    this.customMenuOpen = true;
                }
            }
        }

        /// <summary>Saves all monsters from the game world.</summary>
        public void cleanMonsters()
        {
            this.monsters = new Dictionary<GameLocation, List<Monster>>();

            foreach (GameLocation loc in Game1.locations)
            {
                this.monsters.Add(loc, new List<Monster>());
                foreach (var npc in loc.characters)
                {
                    if (npc is Monster monster)
                    {
                        this.Monitor.Log(npc.Name);
                        this.monsters[loc].Add(monster);
                    }
                }
                foreach (var monster in this.monsters[loc])
                    loc.characters.Remove(monster);
            }


        }

        /// <summary>Adds all saved monster back into the game world.</summary>
        public static void RestoreMonsters()
        {
            foreach (var pair in SaveAnywhere.Instance.monsters)
            {
                foreach (Monster m in pair.Value)
                {
                    pair.Key.addCharacter(m);
                }
            }
            SaveAnywhere.Instance.monsters.Clear();
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            //this.Monitor.Log("On day started called.", LogLevel.Info);

            if (this.IsCustomSaving == false)
            {
                if (this.firstLoad == false)
                {
                    this.firstLoad = true;
                    if (this.SaveManager.saveDataExists())
                    {
                        this.ShouldResetSchedules = false;
                        this.ApplySchedules();
                    }
                }
                else if (this.firstLoad == true)
                {

                    this.SaveManager.ClearData(); //Clean the save state on consecutive days to ensure save files aren't lost inbetween incase the player accidently quits.
                }

                //this.Monitor.Log("Cleaning old save file.", LogLevel.Info);

                // reload NPC schedules
                this.ShouldResetSchedules = true;

                // reset NPC schedules

                /*
                // update NPC schedules
                this.NpcSchedules.Clear();
                foreach (NPC npc in Utility.getAllCharacters())
                {
                    if (!this.NpcSchedules.ContainsKey(npc.Name))
                        this.NpcSchedules.Add(npc.Name, this.ParseSchedule(npc));
                }
                */
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            // initiate save (if valid context)
            if (e.Button == this.Config.SaveKey)
            {

                if (Game1.eventUp) return;
                if (Game1.isFestival()) return;
                if (Game1.client == null)
                {

                    // validate: community center Junimos can't be saved

                    if (Game1.player.currentLocation.getCharacters().OfType<Junimo>().Any())
                    {
                        Game1.addHUDMessage(new HUDMessage("The spirits don't want you to save here.", HUDMessage.error_type));
                        return;
                    }

                    // save
                    this.IsCustomSaving = true;
                    this.SaveManager.BeginSaveData();
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage("Only server hosts can save anywhere.", HUDMessage.error_type));
                }
            }
        }

        /// <summary>Apply the NPC schedules to each NPC.</summary>
        private void ApplySchedules()
        {
            if (Game1.weatherIcon == Game1.weather_festival || Game1.isFestival() || Game1.eventUp)
                return;

            // apply for each NPC
            foreach (GameLocation loc in Game1.locations)
            {
                foreach (NPC npc in loc.characters)
                {
                    if (npc.isVillager() == false)
                        continue;

                    npc.fillInSchedule();
                    continue;
                }

            }

        }

        public override object GetApi()
        {

            return new SaveAnywhereAPI();
        }
    }
}
