/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BinaryLip/ScheduleViewer
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ScheduleViewer.Interfaces;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleViewer
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        public static ModConfig Config;
        public static IMonitor Console;
        public static IModHelper ModHelper;
        private static DialogueBox ErrorDialogue = null;

        public const string ModMessageSchedule = "the day's schedule";
        public const string ModMessageCurrentLocation = "NPC current location update";

        public static readonly string CustomDataPath = "Mods/BinaryLip.ScheduleViewer/TileAreas";

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // set up static properties
            Console = this.Monitor;
            ModHelper = helper;
            Config = helper.ReadConfig<ModConfig>();
            // set up event handlers
            //helper.Events.Content.AssetReady += OnAssetReady; //not sure if needed
            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
            helper.Events.World.NpcListChanged += OnNpcListChanged;
        }

        /*********
        ** Private methods
        *********/
        /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Schedule.LoadTileAreas();
            if (Game1.IsMasterGame)
            {
                Schedule.ClearSchedules();
            }
        }

        /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // check for mismatched GameVersion and Mods between host and current player
            foreach (IMultiplayerPeer peer in this.Helper.Multiplayer.GetConnectedPlayers())
            {
                if (peer.IsHost)
                {
                    List<string> errors = new();
                    var modDiffs = peer.Mods?.Where(mod => !mod.Version.Equals(this.Helper.ModRegistry.Get(mod.ID)?.Manifest.Version)).Select(mod => new { mod.Name, HostVersion = mod.Version.ToString(), PlayerVersion = this.Helper.ModRegistry.Get(mod.ID)?.Manifest.Version.ToString() });
                    if (peer.HasSmapi && peer.GameVersion.ToString() != Game1.version)
                    {
                        errors.Add(this.Helper.Translation.Get("error.mismatch_game_version"));
                    }
                    if (!peer.HasSmapi || modDiffs.Any())
                    {
                        errors.Add(this.Helper.Translation.Get("error.mismatch_mods"));
                        foreach (var diff in modDiffs)
                        {
                            Console.LogOnce($"Warning! Found mismatched mod: \"{diff.Name}\". Host version: {diff.HostVersion} Your vesion: {diff.PlayerVersion}", LogLevel.Warn);
                        }
                    }
                    if (errors.Any())
                    {
                        ErrorDialogue = new DialogueBox(string.Join("^", errors));
                    }
                    break;
                }
            }

            // broadcast the new day's schedule if multiplayer
            if (Game1.IsMasterGame && this.Helper.Multiplayer.GetConnectedPlayers().Any())
            {
                Schedule.SendSchedules();
            }
        }

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            static ModConfig.SortType parseSortType(string value)
            {
                _ = Enum.TryParse(value, out ModConfig.SortType sortType);
                return sortType;
            }

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenuApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenuApi != null)
            {
                // register mod
                configMenuApi.Register(
                    ModManifest,
                    () => Config = new ModConfig(),
                    () => Helper.WriteConfig(Config)
                );

                // add some config options
                configMenuApi.AddSectionTitle(ModManifest, () => this.Helper.Translation.Get("config.title.general"));
                configMenuApi.AddKeybind(
                    ModManifest,
                    name: () => this.Helper.Translation.Get("config.option.show_schedule_key.name"),
                    getValue: () => Config.ShowSchedulesKey,
                    setValue: value => Config.ShowSchedulesKey = value
                );
                configMenuApi.AddBoolOption(
                    ModManifest,
                    name: () => this.Helper.Translation.Get("config.option.use_address.name"),
                    tooltip: () => this.Helper.Translation.Get("config.option.use_address.description"),
                    getValue: () => Config.UseAddress,
                    setValue: value => Config.UseAddress = value
                );
                configMenuApi.AddBoolOption(
                    ModManifest,
                    name: () => this.Helper.Translation.Get("config.option.disable_hover.name"),
                    tooltip: () => this.Helper.Translation.Get("config.option.disable_hover.description"),
                    getValue: () => Config.DisableHover,
                    setValue: value => Config.DisableHover = value
                );
                configMenuApi.AddBoolOption(
                    ModManifest,
                    name: () => this.Helper.Translation.Get("config.option.use_large_font.name"),
                    tooltip: () => this.Helper.Translation.Get("config.option.use_large_font.description"),
                    getValue: () => Config.UseLargerFontForScheduleDetails,
                    setValue: value => Config.UseLargerFontForScheduleDetails = value
                );
                configMenuApi.AddSectionTitle(ModManifest, () => this.Helper.Translation.Get("config.title.filter_sort"));
                configMenuApi.AddTextOption(
                    ModManifest,
                    name: () => this.Helper.Translation.Get("config.option.sort_options.name"),
                    tooltip: () => this.Helper.Translation.Get("config.option.sort_options.description"),
                    getValue: () => Config.NPCSortOrder.ToString(),
                    setValue: value => Config.NPCSortOrder = parseSortType(value),
                    allowedValues: Enum.GetNames(typeof(ModConfig.SortType)),
                    formatAllowedValue: type => this.Helper.Translation.Get($"config.option.sort_options.option_{(ushort)parseSortType(type)}")
                );
                configMenuApi.AddBoolOption(
                    ModManifest,
                    name: () => this.Helper.Translation.Get("config.option.only_show_met_npcs.name"),
                    tooltip: () => this.Helper.Translation.Get("config.option.only_show_met_npcs.description"),
                    getValue: () => Config.OnlyShowMetNPCs,
                    setValue: value => Config.OnlyShowMetNPCs = value
                );
                configMenuApi.AddBoolOption(
                    ModManifest,
                    name: () => this.Helper.Translation.Get("config.option.only_show_socializable_npcs.name"),
                    tooltip: () => this.Helper.Translation.Get("config.option.only_show_socializable_npcs.description"),
                    getValue: () => Config.OnlyShowSocializableNPCs,
                    setValue: value => Config.OnlyShowSocializableNPCs = value
                );
            }

            var mobilePhoneApi = Helper.ModRegistry.GetApi<IMobilePhoneApi>("JoXW.MobilePhone");
            if (mobilePhoneApi != null)
            {
                Texture2D appIcon = Helper.ModContent.Load<Texture2D>("assets/app_icon.png");
                bool success = mobilePhoneApi.AddApp(Helper.ModRegistry.ModID, "Schedule Viewer", OpenMenu, appIcon);
                Console.Log($"Added app to Mobile Phone Continued successfully? {success}", LogLevel.Debug);
            }

            var toolbarIconsApi = Helper.ModRegistry.GetApi<IToolbarIconsApi>("furyx639.ToolbarIcons");
            if (toolbarIconsApi != null)
            {
                toolbarIconsApi.AddToolbarIcon(ModManifest.UniqueID, Helper.ModContent.GetInternalAssetName("assets/Icons.png").BaseName, new Rectangle(0, 24, 16, 16), this.Helper.Translation.Get("schedule_details_page.header"));
                toolbarIconsApi.Subscribe(args =>
                {
                    if (args.Id.Equals(ModManifest.UniqueID))
                    {
                        OpenMenu();
                    }
                });
            }
        }

        /// <inheritdoc cref="IInputEvents.ButtonsChanged"/>
        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }
            try
            {
                // open menu
                if (e.Pressed.Contains(Config.ShowSchedulesKey))
                {
                    // open if no conflict
                    if (Game1.activeClickableMenu == null)
                    {
                        if (Context.IsPlayerFree && !Game1.player.UsingTool && !Game1.player.isEating && Game1.currentMinigame == null)
                        {
                            OpenMenu();
                        }
                    }
                    // open from GameMenu if it's safe to close the GameMenu
                    else if (Game1.activeClickableMenu is GameMenu)
                    {
                        if (Game1.activeClickableMenu.readyToClose())
                        {
                            OpenMenu();
                        }
                    }
                    // close SchedulePage menu
                    else if (Game1.activeClickableMenu is SchedulesPage)
                    {
                        if (Game1.activeClickableMenu.readyToClose())
                        {
                            Game1.activeClickableMenu.exitThisMenu();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Log($"Error opening the Schedule Viewer. See details below:\n{ex}", LogLevel.Error);
            }
        }

        /// <inheritdoc cref="IMultiplayerEvents.PeerConnected"/>
        private void OnPeerConnected(object sender, PeerConnectedEventArgs e)
        {
            // broadcast the schedule
            if (Game1.IsMasterGame)
            {
                Schedule.SendSchedules();
            }
        }

        /// <inheritdoc cref="IMultiplayerEvents.ModMessageReceived"/>
        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == this.ModManifest.UniqueID)
            {
                Console.Log($"Received {e.Type} from host.", LogLevel.Trace);
                switch (e.Type)
                {
                    case ModMessageSchedule:
                        Schedule.ReceiveSchedules(e.ReadAs<(int, Dictionary<string, Schedule.NPCSchedule>)>());
                        break;
                    case ModMessageCurrentLocation:
                        var (location, npcsToUpdate) = e.ReadAs<(string, string[])>();
                        string locationName = Schedule.PrettyPrintLocationName(location);
                        Schedule.UpdateCurrentLocation(locationName, npcsToUpdate);
                        break;
                }
            }
        }

        /// <inheritdoc cref="IWorldEvents.NpcListChanged"/>
        private void OnNpcListChanged(object sender, NpcListChangedEventArgs e)
        {
            // update current location for NPCs
            if (Game1.IsMasterGame && Schedule.HasSchedules())
            {
                string[] npcsToUpdate = Schedule.GetSchedules()
                    .Where(schedule => e.Added.Any(npc => npc.Name.Equals(schedule.Key))) // find npcs that moved locations
                    .Select(item => item.Key) // select their internal name
                    .ToArray();
                if (npcsToUpdate.Length > 0)
                {
                    string locationName = Schedule.PrettyPrintLocationName(e.Location);
                    Schedule.UpdateCurrentLocation(locationName, npcsToUpdate);
                    this.Helper.Multiplayer.SendMessage<(string, string[])>((e.Location.Name, npcsToUpdate), ModMessageCurrentLocation);
                }
            }
        }

        /// <inheritdoc cref="IDisplayEvents.MenuChanged"/>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (ErrorDialogue != null && e.OldMenu == ErrorDialogue)
            {
                Game1.activeClickableMenu = new SchedulesPage();
            }
            if (e.NewMenu is SchedulesPage)
            {
                ErrorDialogue = null;
            }
        }

        /// <inheritdoc cref="IContentEvents.AssetRequested"/>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo(CustomDataPath))
            {
                e.LoadFromModFile<Dictionary<string, JObject>>("assets/tile_areas.json", AssetLoadPriority.Medium);
            }
        }

        /// <inheritdoc cref="IContentEvents.AssetReady"/>
        private void OnAssetReady(object sender, AssetReadyEventArgs e)
        {
            // update tile areas if asset gets updated. will this ever happen?
            if (e.Name.IsEquivalentTo(CustomDataPath))
            {
                Schedule.LoadTileAreas();
            }
        }

        private static void OpenMenu()
        {
            if (ErrorDialogue != null)
            {
                Game1.activeClickableMenu = ErrorDialogue;
            }
            else
            {
                Game1.activeClickableMenu = new SchedulesPage();
            }
        }
    }
}