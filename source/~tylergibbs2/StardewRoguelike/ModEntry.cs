/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewHitboxes;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Toolkit.Framework.Clients.WebApi;
using StardewRoguelike.Bosses;
using StardewRoguelike.Netcode;
using StardewRoguelike.UI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace StardewRoguelike
{
    public class ModEntry : Mod
    {
        public static readonly string WebsiteBaseUrl = "https://roguelike.tyler.solutions";

        public static ModEntry Instance { get; private set; } = null!;

        public static IModEvents Events { get; private set; } = null!;

        public static IDataHelper DataHelper { get; private set; } = null!;

        public static IMultiplayerHelper MultiplayerHelper { get; private set; } = null!;

        public static IGameContentHelper GameContentHelper { get; private set; } = null!;

        public static IReflectionHelper ReflectionHelper { get; private set; } = null!;

        public static IModRegistry ModRegistry { get; private set; } = null!;

        public static IMonitor ModMonitor { get; private set; } = null!;

        /// <summary>
        /// Used for uploading the run statistics online.
        /// </summary>
        public static readonly HttpClient WebClient = new();

        /// <summary>
        /// Tracks the statistics of the game.
        /// </summary>
        public static readonly Stats ActiveStats = new();

        public static bool Invincible { get; set; } = false;

        public static bool ShouldShowModDisclaimer { get; set; } = false;

        public static ModConfig Config { get; private set; } = null!;

        /// <summary>
        /// Whether or not being able to upload a run online is disabled.
        /// </summary>
        public static bool DisableUpload { get; set; } = false;

        public static string? CurrentVersion { get; private set; } = null;
        public static string? NewUpdateVersion { get; private set; } = null;

        /// <summary>
        /// Manifest IDs of mods that are allowed to be played with while
        /// also being able to upload a run online.
        /// </summary>
        public static readonly List<string> WhitelistedMods = new()
        {
            "SMAPI.ErrorHandler",
            "SMAPI.SaveBackup",
            "tylergibbs2.StardewRoguelike",
            "tylergibbs2.StardewNametags",
            "tylergibbs2.DefaultFarmer",
            "spacechase0.GenericModConfigMenu",
            "jltaylor-us.GMCMOptions",
            "thespbgamer.ZoomLevel"
        };

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;

            I18n.Init(helper.Translation);
            Config = helper.ReadConfig<ModConfig>();
            Patch.PatchAll("tylergibbs2.StardewRoguelike");

            // Assign references to various helpers to
            // static fields.
            Events = helper.Events;
            DataHelper = helper.Data;
            MultiplayerHelper = helper.Multiplayer;
            GameContentHelper = helper.GameContent;
            ReflectionHelper = helper.Reflection;
            ModRegistry = helper.ModRegistry;
            ModMonitor = Monitor;

            // Load all custom audio
            ContentManager.SetupAudio();

            // Initialize all event handlers
            helper.Events.GameLoop.GameLaunched += SetupGMCM;
            helper.Events.GameLoop.GameLaunched += CheckForUpdate;
            helper.Events.GameLoop.SaveLoaded += Roguelike.SaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += Roguelike.ReturnedToTitle;
            helper.Events.GameLoop.TimeChanged += Roguelike.TimeChanged;
            helper.Events.GameLoop.UpdateTicked += ActiveStats.WatchChanges;
            helper.Events.GameLoop.UpdateTicked += Roguelike.UpdateTicked;
            helper.Events.GameLoop.UpdateTicked += SpectatorMode.Update;
            helper.Events.GameLoop.UpdateTicked += Curse.UpdateTicked;

            helper.Events.Content.AssetRequested += ContentManager.AssetRequested;

            helper.Events.Multiplayer.ModMessageReceived += ModMessageReceived;
            helper.Events.Multiplayer.PeerConnected += PeerConnected;

            helper.Events.Display.MenuChanged += MenuChanged;
            helper.Events.Display.MenuChanged += Roguelike.MenuChanged;
            helper.Events.Display.RenderedHud += RenderHud;

            helper.Events.Player.Warped += Roguelike.PlayerWarped;
            helper.Events.Player.Warped += Merchant.PlayerWarped;
            helper.Events.Player.Warped += ChallengeFloor.PlayerWarped;
            helper.Events.Player.Warped += BossManager.PlayerWarped;
            helper.Events.Player.Warped += SpectatorMode.RespawnPlayers;

            helper.Events.Input.ButtonPressed += Roguelike.ButtonPressed;

            var assemblyConfiguration = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyConfigurationAttribute>();
            if (assemblyConfiguration?.Configuration == "Debug")
            {
                helper.ConsoleCommands.Add("rdebug", "Debugging command suite for the Roguelike.", DebugCommands.Parse);
                helper.Events.Input.ButtonPressed += DebugCommands.ButtonPressed;
            }
        }

        public void CheckForUpdate(object? sender, GameLaunchedEventArgs e)
        {
            object? metadata = Helper.ModRegistry.Get(Helper.ModRegistry.ModID);
            if (metadata is null)
                return;

            ModEntryModel? updateResult = (ModEntryModel?)metadata.GetType().GetProperty("UpdateCheckData", BindingFlags.Instance | BindingFlags.Public)!.GetValue(metadata);

            CurrentVersion = ((IModInfo)metadata).Manifest.Version.ToString();

            if (updateResult is null || updateResult.SuggestedUpdate is null)
                return;

            NewUpdateVersion = updateResult.SuggestedUpdate.Version.ToString();
        }

        public void RenderHud(object? sender, RenderedHudEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (NewUpdateVersion is not null)
            {
                string newVersionText = I18n.UI_NewUpdate(version: NewUpdateVersion);
                Vector2 newVersionTextSize = Game1.smallFont.MeasureString(newVersionText);

                Point drawPos = new((int)(Game1.uiViewport.Width - newVersionTextSize.X - 40), 140);

                IClickableMenu.drawTextureBox(
                    e.SpriteBatch,
                    drawPos.X - 15,
                    drawPos.Y - 12,
                    (int)newVersionTextSize.X + 33,
                    (int)newVersionTextSize.Y + 25,
                    Color.White
                );

                Utility.drawTextWithShadow(
                    e.SpriteBatch,
                    newVersionText,
                    Game1.smallFont,
                    new Vector2(drawPos.X, drawPos.Y + 3),
                    Color.Black
                );
            }
            if (CurrentVersion is not null)
            {
                Vector2 versionTextSize = Game1.tinyFont.MeasureString(CurrentVersion);

                Vector2 drawPos = new((Game1.uiViewport.Width - versionTextSize.X - 100), 80);

                e.SpriteBatch.DrawString(
                    Game1.tinyFont,
                    CurrentVersion,
                    drawPos,
                    Color.White
                );
            }
        }

        /// <summary>
        /// Gets all installed mods for mods are aren't on the whitelist.
        /// </summary>
        /// <returns>All mods that aren't whitelisted.</returns>
        public static List<IModInfo> GetInvalidMods()
        {
            List<IModInfo> invalidMods = new();

            foreach (IModInfo mod in ModRegistry.GetAll())
            {
                if (!WhitelistedMods.Contains(mod.Manifest.UniqueID))
                    invalidMods.Add(mod);
            }

            if (invalidMods.Count > 0)
                ModMonitor.Log("Found invalid mods: " + string.Join(", ", invalidMods.Select(m => m.Manifest.Name)), LogLevel.Debug);
            else
                ModMonitor.Log("No invalid mods found.", LogLevel.Debug);

            return invalidMods;
        }

        /// <summary>
        /// Gets all installed mods that aren't whitelisted. from a multiplayer peer connection.
        /// </summary>
        /// <param name="peer">The multiplayer peer.</param>
        /// <returns>All mods that aren't whitelisted.</returns>
        public static List<IMultiplayerPeerMod> GetInvalidMods(IMultiplayerPeer peer)
        {
            List<IMultiplayerPeerMod> invalidMods = new();

            foreach (IMultiplayerPeerMod mod in peer.Mods)
            {
                if (!WhitelistedMods.Contains(mod.ID))
                    invalidMods.Add(mod);
            }

            if (invalidMods.Count > 0)
                ModMonitor.Log("Found invalid mods for multiplayer peer: " + string.Join(", ", invalidMods.Select(m => m.Name)), LogLevel.Info);
            else
                ModMonitor.Log("No invalid mods found for multiplayer peer.", LogLevel.Info);

            return invalidMods;
        }

        /// <summary>
        /// Event handler for when the Game1 activeClickableMenu changes.
        /// This handles showing the disclaimer for if the player has invalid mods.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MenuChangedEventArgs"/> instance containing the event data.</param>
        public void MenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu is GameMenu && e.NewMenu is null)
            {
                Helper.Data.WriteGlobalData("RoguelikeGameOptions", Game1.options);
                return;
            }

            if (!ShouldShowModDisclaimer || e.NewMenu is not null || Config.SkipModDisclaimer)
                return;

            Game1.activeClickableMenu = new DisclaimerMenu();
            ShouldShowModDisclaimer = false;
        }

        private void SetupGMCM(object? sender, GameLaunchedEventArgs e)
        {
            IGenericModConfigMenuApi? configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_SkipDisclaimer_Name(),
                tooltip: () => I18n.Config_SkipDisclaimer_Tooltip(),
                getValue: () => Config.SkipModDisclaimer,
                setValue: value => Config.SkipModDisclaimer = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_SkipCharacter_Name(),
                tooltip: () => I18n.Config_SkipCharacter_Tooltip(),
                getValue: () => Config.SkipCharacterCreation,
                setValue: value => Config.SkipCharacterCreation = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_BossSplits_Name(),
                tooltip: () => I18n.Config_BossSplits_Tooltip(),
                getValue: () => Config.DisableBossSplits,
                setValue: value => Config.DisableBossSplits = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_AutomaticallyFaceMouse_Name(),
                tooltip: () => I18n.Config_AutomaticallyFaceMouse_Tooltip(),
                getValue: () => Config.AutomaticallyFaceMouse,
                setValue: value => Config.AutomaticallyFaceMouse = value
            );
        }

        /// <summary>
        /// Event handler for when a multiplayer peer connects.
        /// This handles checking if a connected peer has mods that are invalid/illegal.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PeerConnectedEventArgs"/> instance containing the event data.</param>
        public void PeerConnected(object? sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer || !e.Peer.HasSmapi)
                return;

            List<IMultiplayerPeerMod> invalidMods = GetInvalidMods(e.Peer);
            if (invalidMods.Count == 0)
                return;

            string error = I18n.Roguelike_InvalidClientMods(count: invalidMods.Count);
            Game1.chatBox.addErrorMessage(error);

            DisableUpload = true;
        }

        /// <summary>
        /// Event handler for messages between multiplayer mod clients.
        /// This handles player respawning and "game over"ing runs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ModMessageReceivedEventArgs"/> instance containing the event data.</param>
        public void ModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            // Ignore all messages that aren't from this mod.
            if (e.FromModID != ModManifest.UniqueID)
                return;

            if (e.Type == "RespawnMessage") // Perform a respawn on this client
            {
                RespawnMessage message = e.ReadAs<RespawnMessage>();
                ActiveStats.EndTime = null;

                SpectatorMode.ExitSpectatorMode(message.RespawnLevel);
                Invincible = false;
                Game1.player.health = Game1.player.maxHealth;
            }
            else if (e.Type == "BossDeath")
            {
                BossDeathMessage message = e.ReadAs<BossDeathMessage>();
                Game1.onScreenMenus.Add(
                    new BossKillAnnounceMenu(message.BossName, message.KillSeconds)
                );
            }
            else if (e.Type == "AttackIndicator")
            {
                AttackIndicatorMessage message = e.ReadAs<AttackIndicatorMessage>();
                message.Trigger();
            }
            else if (e.Type == "GameOver") // Signal that the game is over
                Roguelike.GameOver();
        }
    }
}
