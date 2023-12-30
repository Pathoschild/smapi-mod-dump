/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HauntedPineapple/Stendew-Valley
**
*************************************************/

/// Project: Stendew Valley
/// File: ModEntry.cs
/// Description: Entry file for the mod
/// Author: Team Stendew Valley

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.SDKs;

namespace StendewValley
{
    /// <summary>The mod entry point</summary>
    internal sealed class ModEntry : Mod
    {
        // Private mod configuration file
        private static ModConfig Config;

        // Name of the maps in Content Patcher
        private readonly string MAIN_ISLAND_NAME = "Custom_MainIsland";
        private readonly string STEN_HOUSE_NAME = "Custom_StenHouse";
        private readonly string MAIN_ISLAND_CAVE_NAME = "Custom_MainIslandCave";

        // Custom Game Locations
        private GameLocation mainIsland;
        private GameLocation stenHouse;
        private GameLocation mainIslandCave;

        // List of spawnable zones in stens house
        List<Rectangle> stenHouseZones = new List<Rectangle>();

        // Boulder object
        private CustomLargeObject boulder;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Set up ModConfig
            Config = this.Helper.ReadConfig<ModConfig>();

            // Hook up events
            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

            // Spawnable zones in stens house
            stenHouseZones.Add(new Rectangle(7, 5, 9, 3));
            stenHouseZones.Add(new Rectangle(6, 14, 5, 4));

            // Hook up button press event
            Helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            // Instantiate harmony patcher
            var harmony = new Harmony(ModManifest.UniqueID);

            try
            {
                // Patch monster behaviour
                harmony.Patch(
                   original: AccessTools.Method(typeof(Monster), nameof(Monster.updateMovement)),
                   prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Monster_updateMovement_prefix))
                );

                this.Monitor.Log("Patches succeeded", LogLevel.Debug);
            }
            catch (Exception ex) 
            {
                this.Monitor.Log("Something failed", LogLevel.Debug);
            }
        }

        /// <summary>
        /// Setup event run when the game is launched
        /// </summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="e">Event arguments</param>
        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            // Loads the configuration menu from API
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is not null)
            {
                this.Monitor.Log($"Loaded Configurations Menu", LogLevel.Debug);

                // register mod
                configMenu.Register(
                    mod: ModManifest,
                    reset: () => Config = new ModConfig(),
                    save: () => Helper.WriteConfig(Config)
                );

                // Add options to custom menu
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => "Min Slimes Per Day",
                    getValue: () => Config.MinSlimesPerDay,
                    setValue: value => Config.MinSlimesPerDay = value
                );
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => "Max Slimes Per Day",
                    getValue: () => Config.MaxSlimesPerDay,
                    setValue: value => Config.MaxSlimesPerDay = value
                );
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => "Max Total Slimes Cave",
                    getValue: () => Config.MaxTotalSlimesCave,
                    setValue: value => Config.MaxTotalSlimesCave = value
                );
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => "Max Total Slimes Sten's House",
                    getValue: () => Config.MaxTotalSlimesHouse,
                    setValue: value => Config.MaxTotalSlimesHouse = value
                );
            }
        }

        /// <summary>
        /// Event run when loading into a save (for initial logic)
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Set up the location variables
            mainIsland = GetGameLocationByName(MAIN_ISLAND_NAME);
            stenHouse = GetGameLocationByName(STEN_HOUSE_NAME);
            mainIslandCave = GetGameLocationByName(MAIN_ISLAND_CAVE_NAME);

            // Set up the custom boulders
            InitializeBoulder();

            // Boulder loaded initially
            boulder.Enabled = true;

            // Spawn extra slimes
            if (Config.MaxSlimesPerDay < Config.MinSlimesPerDay)
            {
                Config.MaxSlimesPerDay = Config.MinSlimesPerDay;
                Helper.WriteConfig(Config);
            }

            // Hook up hat effect
            Game1.player.hat.fieldChangeEvent += HatChanged;
        }

        /// <summary>
        /// Event run when each day starts (for reset logic)
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (Config.MaxSlimesPerDay < Config.MinSlimesPerDay)
            {
                Config.MaxSlimesPerDay = Config.MinSlimesPerDay;
                Helper.WriteConfig(Config);
            }

            // Spawn custom slimes
            SpawnSlimesStenHouse();
            SpawnSlimesCave();

            // Hat logic
            ApplyHatEffect();

            // Boulder Quest update
            UpdateBoulderQuest();
        }

        /// <summary>
        /// Helper event for spawning slimes in Sten's house
        /// </summary>
        private void SpawnSlimesStenHouse()
        {
            // Gets the number of existing slimes in the map
            int existing = 0;
            if (stenHouse.characters.Count > 0)
            {
                // Looping backwards through list
                for (int i = stenHouse.characters.Count - 1; i >= 0; i--)
                {
                    // Increment existing for every slime
                    if (stenHouse.characters[i] is Monster)
                        existing++;
                }
            }

            // Calculates the amount of slimes to spawn
            int amount = Math.Min(Config.MaxTotalSlimesCave - existing, Game1.random.Next(Config.MinSlimesPerDay, Config.MaxSlimesPerDay + 1));
            if (amount <= 0)
                return;
            
            // Vector of already used positions (to ensure a spread out spawn group)
            List<Vector2> used = new();

            // Loop through and spawn new slimes
            for (int i = 0; i < amount; i++)
            {
                // Generate new position within a given spawn area
                Rectangle rect = stenHouseZones[Game1.random.Next(stenHouseZones.Count)];
                Vector2 pos = new Vector2(rect.X + Game1.random.Next(rect.Width), rect.Y + Game1.random.Next(rect.Height)) * 64;

                // If this position is already used, skip
                if (used.Contains(pos))
                    continue;

                // Create a new slime and add it to Sten's house
                Monster m = new GreenSlime(pos, 0);
                stenHouse.characters.Add(m);
                used.Add(pos);

                this.Monitor.Log("Added slime to Sten's House", LogLevel.Debug);
            }
        }

        /// <summary>
        /// Helper method for spawning slimes in the cave
        /// </summary>
        private void SpawnSlimesCave()
        {
            // Gets the number of existing slimes in the map
            int existing = 0;
            if (mainIslandCave.characters.Count > 0)
            {
                // Looping backwards through list
                for (int i = mainIslandCave.characters.Count - 1; i >= 0; i--)
                {
                    // Increment existing for every slime
                    if (mainIslandCave.characters[i] is Monster)
                        existing++;
                }
            }

            // Calculates the amount of slimes to spawn
            int amount = Math.Min(Config.MaxTotalSlimesCave - existing, Game1.random.Next(Config.MinSlimesPerDay, Config.MaxSlimesPerDay + 1));
            if (amount <= 0)
                return;

            // Vector of already used positions (to ensure a spread out spawn group)
            List<Vector2> used = new();

            // Loop through and spawn new slimes
            for (int i = 0; i < amount; i++)
            {
                // Generate new position in the cave
                Vector2 pos = new Vector2(6 + Game1.random.Next(14), 6 + Game1.random.Next(16)) * 64;

                // If this position is already used, skip
                if (used.Contains(pos))
                    continue;

                // Create a new slime and add it to the Cave
                Monster m = new GreenSlime(pos, 0);
                mainIslandCave.characters.Add(m);
                used.Add(pos);

                this.Monitor.Log("Added slime to the Cave", LogLevel.Debug);
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // Check boulder quest
            UpdateBoulderQuest();
        }

        /// <summary>
        /// Makes monsters passive when wearing the special item
        /// </summary>
        /// <param name="__instance">Instance of a monster</param>
        /// <param name="time">Time variable (required)</param>
        /// <returns>True if continuing to base method, false if skipping it</returns>
        private static bool Monster_updateMovement_prefix(Monster __instance, GameTime time)
        {
            if (Config.PassiveMobs)
            {
                // Calls default movement method (no aggro)
                __instance.defaultMovementBehavior(time);
                return false;   // Skips continuation of original method
            }
            // Continue on to the original method
            return true;
        }

        // Helper methods
        /// <summary>
        /// Search for a reference of a GameLocation by the map's name.
        /// </summary>
        /// <param name="locationName">Name of the map (Woods, Forest, etc.)</param>
        /// <returns></returns>
        private GameLocation GetGameLocationByName(string locationName)
        {
            // Loop through all locations to get a ref to given map
            IList<GameLocation> locations = Game1.locations;
            for (int i = Game1.locations.Count - 1; i >= 0; i--)
            {
                if (locations[i].Name == locationName)
                {
                    return locations[i];
                }
            }

            // No map was found out of all GameLocations
            throw new KeyNotFoundException(
                "Map \"" + locationName + "\" not found in Game1.locations" +
                " (has this method been called before OnSaveLoaded()?)");
        }
        
        /// <summary>
        /// Helper method to apply the status effect of the hat
        /// </summary>
        private void ApplyHatEffect()
        {
            // Cleanup (and reseting the effect)
            this.Monitor.Log("Mods are not peaceful", LogLevel.Debug);
            Config.PassiveMobs = false;

            // Sets effect if wearing the right hat
            if (Game1.player.hat.Value != null && Game1.player.hat.Value.Name == "Bald Cap")
            {
                this.Monitor.Log("Bald Cap Equiped. Mobs are now peaceful", LogLevel.Debug);
                Config.PassiveMobs = true;
            }
        }

        /// <summary>
        /// Event raised when the player switches hats
        /// </summary>
        /// <param name="field">Reference to the hat</param>
        /// <param name="oldValue">Value store for the previous hat</param>
        /// <param name="newValue">Value store for the new hat</param>
        public void HatChanged(Netcode.NetRef<StardewValley.Objects.Hat> field, StardewValley.Objects.Hat oldValue, StardewValley.Objects.Hat newValue)
        {
            // Differ to the cusotm hat logic method
            ApplyHatEffect();
        }

        /// <summary>
        /// Helper method to spawn a boulder in front of the Cave entrance
        /// </summary>
        private void InitializeBoulder()
        {
            // Spawn the custom boulder
            CustomLargeObject new_boulder = new CustomLargeObject("Test", true, mainIsland);
            new_boulder.SetSprite(58, 62, "Buildings", 898, 5);
            new_boulder.SetSprite(59, 62, "Buildings", 899, 5);
            new_boulder.SetSprite(58, 63, "Buildings", 923, 5);
            new_boulder.SetSprite(59, 63, "Buildings", 924, 5);
            new_boulder.Spawn();

            // Set the global variable for the boulder
            boulder = new_boulder;
        }

        /// <summary>
        /// Helper method for updating the boulder quest
        /// </summary>
        private void UpdateBoulderQuest()
        {
            // If the player gets or has mail with the quest complete
            if (Game1.player.hasOrWillReceiveMail("Custom_PaulQuest_complete"))
            {
                boulder.Enabled = false;
            }
            else
            {
                boulder.Enabled = true;
            }
        }

    }
}