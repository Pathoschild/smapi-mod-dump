/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using Circuit.UI;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace Circuit
{
    public enum StartingOption
    {
        BoatRepaired,
        SkullKey,
        SewerKey,
        TownKey,
        Kent,
        MinesElevators
    }

    public class ModEntry : Mod
    {
        internal static ModEntry Instance { get; private set; } = null!;

        internal ModConfig Config { get; private set; } = null!;

        public bool IsActiveForSave { get; private set; } = false;

        public int RunRngSeed { get; set; } = Guid.NewGuid().GetHashCode();

        public Random RunRng { get; set; }

        public int NewRunDurationSeconds { get; set; } = 7200;

        public HashSet<StartingOption> StartingOptions { get; } = new();

        public TaskManager? TaskManager { get; private set; } = null;

        public EventManager? EventManager { get; private set; } = null;

        public bool Debug { get; }

        public ModEntry() : base()
        {
            var assemblyConfiguration = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyConfigurationAttribute>();
            Debug = assemblyConfiguration?.Configuration == "Debug";

            RunRng = new(RunRngSeed);
        }

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<ModConfig>();

            Harmony harmony = new(ModManifest.UniqueID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // Validates that the switch statements have no bad paths
            var allTasks = Enum.GetValues<CircuitTask>();
            Logger.Log($"{allTasks.Length} total tasks", LogLevel.Info);
            foreach (var task in allTasks)
            {
                CircuitTasks.GetTaskDifficulty(task);
                CircuitTasks.GetTaskDisplayText(task);
                CircuitTasks.GetTaskPoints(task);
            }

            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.SaveCreated += OnSaveCreated;
            helper.Events.GameLoop.Saving += ContentManager.UnloadMaps;

            helper.Events.Input.ButtonPressed += OnButtonPressed;

            helper.Events.Content.AssetRequested += ContentManager.OnAssetRequested;

            helper.ConsoleCommands.Add("circuit", "Root command for the Circuit mod. circuit help for more info.", ConsoleCommands.HandleCommand);
        }

        public static bool ShouldPatch(EventType? activeEvent = null)
        {
            if (activeEvent is null)
                return Instance.IsActiveForSave;

            return Instance.IsActiveForSave && EventManager.EventIsActive(activeEvent.Value);
        }

        public void DestroyGameManagers()
        {
            TaskManager?.UnbindEvents(Helper.Events);
            TaskManager = null;

            EventManager?.UnbindEvents(Helper.Events);
            EventManager = null;
        }

        public void RecreateGameManagers()
        {
            DestroyGameManagers();

            TaskManager = new TaskManager(random: RunRng);
            TaskManager.BindEvents(Helper.Events);

            EventManager = new EventManager(runDurationSeconds: NewRunDurationSeconds, random: RunRng);
            EventManager.BindEvents(Helper.Events);
        }

        private void InitializeStartingOptions(HashSet<StartingOption> options)
        {
            foreach (StartingOption option in options)
            {
                switch (option)
                {
                    case StartingOption.BoatRepaired:
                        Game1.player.mailReceived.Add("willyBoatTicketMachine");
                        Game1.player.mailReceived.Add("willyBoatFixed");
                        Game1.player.mailReceived.Add("willyBackRoomInvitation");
                        break;
                    case StartingOption.SkullKey:
                        Game1.player.hasSkullKey = true;
                        break;
                    case StartingOption.SewerKey:
                        Game1.player.hasRustyKey = true;
                        break;
                    case StartingOption.TownKey:
                        Game1.player.HasTownKey = true;
                        break;
                    case StartingOption.Kent:
                        if (Game1.getCharacterFromName("Kent", mustBeVillager: true, useLocationsListOnly: true) == null)
                            Game1.getLocationFromNameInLocationsList("SamHouse")
                                .addCharacter(
                                    new NPC(new AnimatedSprite("Characters\\Kent", 0, 16, 32), new Vector2(512f, 832f), "SamHouse", 2, "Kent", datable: false, null, Game1.content.Load<Texture2D>("Portraits\\Kent")
                                )
                            );

                        if (!Game1.player.friendshipData.ContainsKey("Kent"))
                            Game1.player.friendshipData.Add("Kent", new Friendship());
                        break;
                    case StartingOption.MinesElevators:
                        MineShaft.lowestLevelReached = 120;
                        break;
                }
            }
        }

        public bool TryStart(out string? error)
        {
            error = null;
            if (EventManager is null)
            {
                error = "Cannot start, there is no active Event Manager.";
                return false;
            }
            else if (EventManager.RunTimer.IsStarted)
            {
                error = "Cannot start, the Event Manager has already been started.";
                return false;
            }

            InitializeStartingOptions(StartingOptions);

            Game1.warpHome();
            EventManager.RunTimer.IsStarted = true;

            return true;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !IsActiveForSave)
                return;

            if (EventManager is not null && !EventManager.RunTimer.IsStarted)
                Game1.gameTimeInterval = 0;
        }

        private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            IsActiveForSave = false;
            DestroyGameManagers();
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (IsActiveForSave && Context.IsMultiplayer)
                IsActiveForSave = false;

            if (Debug)
                IsActiveForSave = true;

            if (!IsActiveForSave)
                return;

            NewRunDurationSeconds = 7200;
            StartingOptions.Clear();

            ContentManager.LoadMaps();
            RecreateGameManagers();

            Game1.warpFarmer("CircuitSpawnRoom", 31, 38, false);

            Game1.chatBox.addInfoMessage("When ready, type /start to begin the run.");
        }

        private void OnSaveCreated(object? sender, SaveCreatedEventArgs e)
        {
            IsActiveForSave = true;
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !IsActiveForSave)
                return;

            if (Config.OpenTaskView.JustPressed())
                Game1.activeClickableMenu = new TaskListMenu();
        }
    }
}
