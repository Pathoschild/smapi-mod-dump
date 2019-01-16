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
        /*********
        ** Fields
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /// <summary>Provides methods for saving and loading game data.</summary>
        private SaveManager SaveManager;

        /// <summary>The parsed schedules by NPC name.</summary>
        private readonly IDictionary<string, string> NpcSchedules = new Dictionary<string, string>();

        /// <summary>Whether villager schedules should be reset now.</summary>
        private bool ShouldResetSchedules;

        /// <summary>Whether we're performing a non-vanilla save (i.e. not by sleeping in bed).</summary>
        private bool IsCustomSaving;

        /// <summary>Used to access the Mod's helper from other files associated with the mod.</summary>
        public static IModHelper ModHelper;

        /// <summary>Used to access the Mod's monitor to allow for debug logging in other files associated with the mod.</summary>
        public static IMonitor ModMonitor;

        private List<Monster> monsters;

        private bool customMenuOpen;


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
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            ModHelper = helper;
            ModMonitor = this.Monitor;
            this.customMenuOpen = false;
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
            this.IsCustomSaving = false;
            this.ShouldResetSchedules = false;

            // load positions
            this.SaveManager.LoadData();
        }

        /// <summary>Raised after the game finishes writing data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaved(object sender, SavedEventArgs e)
        {
            // clear custom data after a normal save (to avoid restoring old state)
            if (!this.IsCustomSaving)
                this.SaveManager.ClearData();
            else
            {
                this.IsCustomSaving = false;
            }
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

            // reset NPC schedules
            if (Context.IsWorldReady && this.ShouldResetSchedules)
            {
                this.ShouldResetSchedules = false;
                this.ApplySchedules();
            }

            if (Game1.activeClickableMenu == null && !this.customMenuOpen) return;
            if (Game1.activeClickableMenu == null && this.customMenuOpen)
            {
                this.restoreMonsters();
                this.customMenuOpen = false;
                return;
            }
            if (Game1.activeClickableMenu != null)
            {
                if (Game1.activeClickableMenu.GetType() == typeof(NewSaveGameMenu))
                {
                    this.customMenuOpen = true;
                }
            }
        }

        /// <summary>Saves all monsters from the game world.</summary>
        private void cleanMonsters()
        {
            this.monsters = new List<Monster>();

            foreach (var npc in Game1.player.currentLocation.characters)
            {
                try
                {
                    if (npc is Monster monster)
                        this.monsters.Add(monster);
                }
                catch { }
            }

            foreach (var monster in this.monsters)
                Game1.player.currentLocation.characters.Remove(monster);
        }

        /// <summary>Adds all saved monster back into the game world.</summary>
        private void restoreMonsters()
        {
            foreach (var monster in this.monsters)
                Game1.player.currentLocation.characters.Add(monster);
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // reload NPC schedules
            this.ShouldResetSchedules = true;

            // update NPC schedules
            this.NpcSchedules.Clear();
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (!this.NpcSchedules.ContainsKey(npc.Name))
                    this.NpcSchedules.Add(npc.Name, this.ParseSchedule(npc));
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
                if (Game1.client == null)
                {
                    this.cleanMonsters();
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
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (npc.DirectionsToNewLocation != null || npc.isMoving() || npc.Schedule == null || npc.controller != null || npc is Horse)
                    continue;

                // get raw schedule from XNBs
                IDictionary<string, string> rawSchedule = this.GetRawSchedule(npc.Name);
                if (rawSchedule == null)
                    continue;

                // get schedule data
                if (!this.NpcSchedules.TryGetValue(npc.Name, out string scheduleData) || string.IsNullOrEmpty(scheduleData))
                {
                    //this.Monitor.Log("THIS IS AWKWARD");
                    continue;
                }

                // get schedule script
                if (!rawSchedule.TryGetValue(scheduleData, out string script))
                    continue;

                // parse entries
                string[] entries = script.Split('/');
                int index = 0;
                foreach (string _ in entries)
                {
                    string[] fields = entries[index].Split(' ');

                    // handle GOTO command
                    if (fields.Contains("GOTO"))
                    {
                        for (int i = 0; i < fields.Length; i++)
                        {
                            string s = fields[i];
                            if (s == "GOTO")
                            {
                                rawSchedule.TryGetValue(fields[i + 1], out script);
                                string[] newEntries = script.Split('/');
                                fields = newEntries[0].Split(' ');
                            }
                        }
                    }

                    // parse schedule script
                    SchedulePathDescription schedulePathDescription;
                    try
                    {
                        if (Convert.ToInt32(fields[0]) > Game1.timeOfDay) break;
                        string endMap = Convert.ToString(fields[1]);
                        int x = Convert.ToInt32(fields[2]);
                        int y = Convert.ToInt32(fields[3]);
                        int endFacingDir = Convert.ToInt32(fields[4]);

                        schedulePathDescription = this.Helper.Reflection
                            .GetMethod(npc, "pathfindToNextScheduleLocation")
                            .Invoke<SchedulePathDescription>(npc.currentLocation.Name, npc.getTileX(), npc.getTileY(), endMap, x, y, endFacingDir, null, null);
                        index++;
                    }
                    catch
                    {
                        continue;
                    }

                    npc.DirectionsToNewLocation = schedulePathDescription;
                    npc.controller = new PathFindController(npc.DirectionsToNewLocation.route, npc, Utility.getGameLocationOfCharacter(npc))
                    {
                        finalFacingDirection = npc.DirectionsToNewLocation.facingDirection,
                        endBehaviorFunction = null
                    };
                }
            }
        }

        /// <summary>Get an NPC's raw schedule data from the XNB files.</summary>
        /// <param name="npcName">The NPC name whose schedules to read.</param>
        /// <returns>Returns the NPC schedule if found, else <c>null</c>.</returns>
        private IDictionary<string, string> GetRawSchedule(string npcName)
        {
            try
            {
                return Game1.content.Load<Dictionary<string, string>>($"Characters\\schedules\\{npcName}");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Load the raw schedule data for an NPC.</summary>
        /// <param name="npc">The NPC whose schedule to read.</param>
        private string ParseSchedule(NPC npc)
        {
            // set flags
            if (npc.Name.Equals("Robin") || Game1.player.currentUpgrade != null)
                npc.IsInvisible = false;
            if (npc.Name.Equals("Willy") && Game1.stats.DaysPlayed < 2u)
                npc.IsInvisible = true;
            else if (npc.Schedule != null)
                npc.followSchedule = true;

            // read schedule data
            IDictionary<string, string> schedule = this.GetRawSchedule(npc.Name);
            if (schedule == null)
                return "";

            // do stuff
            if (npc.isMarried())
            {
                string dayName = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
                if ((npc.Name.Equals("Penny") && (dayName.Equals("Tue") || dayName.Equals("Wed") || dayName.Equals("Fri"))) || (npc.Name.Equals("Maru") && (dayName.Equals("Tue") || dayName.Equals("Thu"))) || (npc.Name.Equals("Harvey") && (dayName.Equals("Tue") || dayName.Equals("Thu"))))
                {
                    this.Helper.Reflection
                        .GetField<string>(npc, "nameOfTodaysSchedule")
                        .SetValue("marriageJob");
                    return "marriageJob";
                }
                if (!Game1.isRaining && schedule.ContainsKey("marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                {
                    this.Helper.Reflection
                        .GetField<string>(npc, "nameOfTodaysSchedule")
                        .SetValue("marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth));
                    return "marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
                }
                npc.followSchedule = false;
                return null;
            }
            else
            {
                if (schedule.ContainsKey(Game1.currentSeason + "_" + Game1.dayOfMonth))
                    return Game1.currentSeason + "_" + Game1.dayOfMonth;
                int i;
                Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship f);
                for (i = (Game1.player.friendshipData.ContainsKey(npc.Name) ? (f.Points / 250) : -1); i > 0; i--)
                {
                    if (schedule.ContainsKey(Game1.dayOfMonth + "_" + i))
                        return Game1.dayOfMonth + "_" + i;
                }
                if (schedule.ContainsKey(string.Empty + Game1.dayOfMonth))
                    return string.Empty + Game1.dayOfMonth;
                if (npc.Name.Equals("Pam") && Game1.player.mailReceived.Contains("ccVault"))
                    return "bus";
                if (Game1.isRaining)
                {
                    if (Game1.random.NextDouble() < 0.5 && schedule.ContainsKey("rain2"))
                        return "rain2";
                    if (schedule.ContainsKey("rain"))
                        return "rain";
                }
                List<string> list = new List<string> { Game1.currentSeason, Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) };
                Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship friendship);
                i = (Game1.player.friendshipData.ContainsKey(npc.Name) ? (friendship.Points / 250) : -1);
                while (i > 0)
                {
                    list.Add(string.Empty + i);
                    if (schedule.ContainsKey(string.Join("_", list)))
                    {
                        return string.Join("_", list);
                    }
                    i--;
                    list.RemoveAt(list.Count - 1);
                }
                if (schedule.ContainsKey(string.Join("_", list)))
                {
                    return string.Join("_", list);
                }
                if (schedule.ContainsKey(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                {
                    return Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
                }
                if (schedule.ContainsKey(Game1.currentSeason))
                {
                    return Game1.currentSeason;
                }
                if (schedule.ContainsKey("spring_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                {
                    return "spring_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
                }
                list.RemoveAt(list.Count - 1);
                list.Add("spring");
                Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship friendship2);
                i = (Game1.player.friendshipData.ContainsKey(npc.Name) ? (friendship2.Points / 250) : -1);
                while (i > 0)
                {
                    list.Add(string.Empty + i);
                    if (schedule.ContainsKey(string.Join("_", list)))
                        return string.Join("_", list);
                    i--;
                    list.RemoveAt(list.Count - 1);
                }
                return schedule.ContainsKey("spring")
                    ? "spring"
                    : null;
            }
        }
    }
}
