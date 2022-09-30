/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewHitboxes;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Toolkit.Framework.Clients.WebApi;
using StardewRoguelike.Bosses;
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

        public static IModEvents Events;

        public static IMultiplayerHelper MultiplayerHelper;

        public static IGameContentHelper GameContentHelper;

        public static IReflectionHelper ReflectionHelper;

        public static IModRegistry ModRegistry;

        public static IMonitor ModMonitor;

        /// <summary>
        /// Used for uploading the run statistics online.
        /// </summary>
        public static readonly HttpClient WebClient = new();

        /// <summary>
        /// Tracks the statistics of the game.
        /// </summary>
        public static readonly Stats Stats = new();

        public static readonly bool DebugMode = false;

        public static bool Invincible = false;

        public static bool ShouldShowModDisclaimer = false;

        public static ModConfig Config;

        /// <summary>
        /// Whether or not being able to upload a run online is disabled.
        /// </summary>
        public static bool DisableUpload = false;

        public static string CurrentVersion = null;
        public static string NewUpdateVersion = null;

        /// <summary>
        /// All assets that have been modified.
        /// The dictionary key is the name of the asset as the game requests it,
        /// the value is the filepath of the asset to replace it with.
        /// </summary>
        private readonly Dictionary<string, string> modifiedAssets = new()
        {
            { "Maps/Mine", "assets/Maps/custom-lobby.tmx" },
            { "Maps/Mines/custom-merchant", "assets/Maps/custom-merchant.tmx" },
            { "Maps/Mines/custom-merchant-curses", "assets/Maps/custom-merchant-curses.tmx" },
            { "Maps/Mines/custom-merchant-hard", "assets/Maps/custom-merchant-hard.tmx" },
            { "Maps/Mines/custom-merchant-curses-hard", "assets/Maps/custom-merchant-curses-hard.tmx" },
            { "Maps/Mines/custom-forge", "assets/Maps/custom-forge.tmx" },
            { "Maps/Mines/custom-defend", "assets/Maps/custom-defend.tmx" },
            { "Maps/Mines/custom-egg", "assets/Maps/custom-egg.tmx" },
            { "Maps/Mines/custom-hotspring", "assets/Maps/custom-hotspring.tmx" },
            { "Maps/Mines/custom-jotpk", "assets/Maps/custom-jotpk.tmx" },
            { "Maps/Mines/custom-dwarf", "assets/Maps/custom-dwarf.tmx" },
            { "Maps/Mines/custom-lavalurk", "assets/Maps/custom-lavalurk.tmx" },
            { "Maps/Mines/custom-slingshot", "assets/Maps/custom-slingshot.tmx" },
            { "Maps/Mines/custom-pickapath", "assets/Maps/custom-pickapath.tmx" },
            { "Maps/Mines/custom-speedrun", "assets/Maps/custom-speedrun.tmx" },
            { "Maps/Mines/custom-1", "assets/Maps/custom-1.tmx" },
            { "Maps/Mines/custom-2", "assets/Maps/custom-2.tmx" },
            { "Maps/Mines/custom-3", "assets/Maps/custom-3.tmx" },
            { "Maps/Mines/custom-4", "assets/Maps/custom-4.tmx" },
            { "Maps/Mines/custom-5", "assets/Maps/custom-5.tmx" },
            { "Maps/Mines/custom-6", "assets/Maps/custom-6.tmx" },
            { "Maps/Mines/custom-7", "assets/Maps/custom-7.tmx" },
            { "Maps/Mines/boss-dragon", "assets/Maps/boss-dragon.tmx" },
            { "Maps/Mines/boss-bug", "assets/Maps/boss-bug.tmx" },
            { "Maps/Mines/boss-slime", "assets/Maps/boss-slime.tmx" },
            { "Maps/Mines/boss-thunderkid", "assets/Maps/boss-thunderkid.tmx" },
            { "Maps/Mines/boss-shadowking", "assets/Maps/boss-shadowking.tmx" },
            { "Maps/Mines/boss-skeleton", "assets/Maps/boss-skeleton.tmx" },
            { "Maps/Mines/boss-queenbee", "assets/Maps/boss-queenbee.tmx" },
            { "Maps/Mines/boss-modulosaurus", "assets/Maps/boss-modulosaurus.tmx" },
            { "Maps/Festivals", "assets/Maps/Festivals.png" },
            { "TileSheets/Projectiles", "assets/TileSheets/Projectiles.png" },
            { "LooseSprites/Cursors", "assets/TileSheets/Cursors.png" },
            { "TerrainFeatures/CleansingCauldron", "assets/TileSheets/Cauldron.png" },
            { "TerrainFeatures/CleansingCauldronEmpty", "assets/TileSheets/CauldronEmpty.png" },
            { "TerrainFeatures/SpeedPad", "assets/TileSheets/speedrun_pads.png" },
        };

        private readonly Dictionary<string, string> modifiedStrings = new()
        {
            { "OptionsPage.cs.11281", "Access Perks" }
        };

        /// <summary>
        /// Custom audio files that get loaded into the game.
        /// </summary>
        /// The tuples in this list are in the format (name, category, filename, loop).
        private readonly List<(string, string, string, bool)> customAudio = new()
        {
            ("bee_boss", "Music", "bee_boss.ogg", true),
            ("megalovania", "Music", "megalovania.ogg", true),
            ("wofl", "Music", "wofl.ogg", true),
            ("hold_your_ground", "Music", "hold_your_ground.ogg", true)
        };

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
            Config = helper.ReadConfig<ModConfig>();
            Patch.PatchAll("tylergibbs2.StardewRoguelike");

            // Assign references to various helpers to
            // static fields.
            Events = helper.Events;
            MultiplayerHelper = helper.Multiplayer;
            GameContentHelper = helper.GameContent;
            ReflectionHelper = helper.Reflection;
            ModRegistry = helper.ModRegistry;
            ModMonitor = Monitor;

            // Load all custom audio
            SetupAudio();

            // Initialize all event handlers
            helper.Events.GameLoop.GameLaunched += SetupGMCM;
            helper.Events.GameLoop.GameLaunched += CheckForUpdate;
            helper.Events.GameLoop.SaveLoaded += Roguelike.SaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += Roguelike.ReturnedToTitle;
            helper.Events.GameLoop.TimeChanged += Roguelike.TimeChanged;
            helper.Events.GameLoop.UpdateTicked += Stats.WatchChanges;
            helper.Events.GameLoop.UpdateTicked += Roguelike.UpdateTicked;
            helper.Events.GameLoop.UpdateTicked += SpectatorMode.Update;
            helper.Events.GameLoop.UpdateTicked += Curse.UpdateTicked;

            helper.Events.Content.AssetRequested += AssetRequested;

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

            helper.ConsoleCommands.Add("rdebug", "Debugging command suite for the Roguelike.", DebugCommands.Parse);
            if (DebugMode)
                helper.Events.Input.ButtonPressed += DebugCommands.ButtonPressed;
        }

        public void CheckForUpdate(object sender, GameLaunchedEventArgs e)
        {
            object metadata = Helper.ModRegistry.Get(Helper.ModRegistry.ModID);
            ModEntryModel updateResult = (ModEntryModel)metadata.GetType().GetProperty("UpdateCheckData", BindingFlags.Instance | BindingFlags.Public).GetValue(metadata);

            CurrentVersion = (metadata as IModInfo).Manifest.Version.ToString();

            if (updateResult is null || updateResult.SuggestedUpdate is null)
                return;

            NewUpdateVersion = updateResult.SuggestedUpdate.Version.ToString();
        }

        public void RenderHud(object sender, RenderedHudEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (NewUpdateVersion is not null)
            {
                string newVersionText = $"New Update: {NewUpdateVersion}";
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
        public void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!ShouldShowModDisclaimer || e.NewMenu is not null || Config.SkipModDisclaimer)
                return;

            Game1.activeClickableMenu = new DisclaimerMenu();
            ShouldShowModDisclaimer = false;
        }

        /// <summary>
        /// Parses and loads the custom audio files into new cues and
        /// adds them to the game's soundbank.
        /// </summary>
        public void SetupAudio()
        {
            foreach (var (name, category, filename, loop) in customAudio)
            {
                string filePath = Path.Combine(Helper.DirectoryPath, "assets", "Audio", filename);
                string extension = Path.GetExtension(filePath);
                SoundEffect audio = extension switch
                {
                    ".wav" => SoundEffect.FromFile(filePath),
                    ".ogg" => OggLoader.OpenOggFile(filePath),
                    _ => throw new NotImplementedException(),
                };
                CueDefinition cue = new(name, audio, Game1.audioEngine.GetCategoryIndex(category), loop)
                {
                    instanceLimit = 1,
                    limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest
                };

                Game1.soundBank.AddCue(cue);
            }
        }

        private void SetupGMCM(object sender, GameLaunchedEventArgs e)
        {
            IGenericModConfigMenuApi configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Skip Mod Disclaimer",
                tooltip: () => "Skips the disclaimer for invalid mods",
                getValue: () => Config.SkipModDisclaimer,
                setValue: value => Config.SkipModDisclaimer = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Skip Character Creation",
                tooltip: () => "Skips the character creation screen in single player",
                getValue: () => Config.SkipCharacterCreation,
                setValue: value => Config.SkipCharacterCreation = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Disable Boss Splits",
                tooltip: () => "Disables the timer splits on boss kill",
                getValue: () => Config.DisableBossSplits,
                setValue: value => Config.DisableBossSplits = value
            );
        }

        /// <summary>
        /// Event handler for when a game asset is requested to be loaded.
        /// This handles loading all custom assets as defined in <see cref="modifiedAssets"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AssetRequestedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException">If a file attempting to be loaded has no implemented way to be loaded.</exception>
        public void AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (modifiedAssets.TryGetValue(e.Name.BaseName, out string fromPath))
            {
                string extension = Path.GetExtension(fromPath);
                switch (extension)
                {
                    case ".tmx":
                        e.LoadFromModFile<xTile.Map>(fromPath, AssetLoadPriority.High);
                        break;
                    case ".png":
                        e.LoadFromModFile<Texture2D>(fromPath, AssetLoadPriority.High);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Strings\\StringsFromCSFiles"))
            {
                e.Edit(editor =>
                {
                    IDictionary<string, string> data = editor.AsDictionary<string, string>().Data;
                    foreach (var (key, value) in modifiedStrings)
                        data[key] = value;
                });
            }
        }

        /// <summary>
        /// Event handler for when a multiplayer peer connects.
        /// This handles checking if a connected peer has mods that are invalid/illegal.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PeerConnectedEventArgs"/> instance containing the event data.</param>
        public void PeerConnected(object sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer || !e.Peer.HasSmapi)
                return;

            List<IMultiplayerPeerMod> invalidMods = GetInvalidMods(e.Peer);
            if (invalidMods.Count == 0)
                return;

            string error = $"Run can not be uploaded due to a client connecting with {invalidMods.Count} illegal mods. The server has to be re-created in order to upload runs.";
            Game1.chatBox.addErrorMessage(error);

            DisableUpload = true;
        }

        /// <summary>
        /// Event handler for messages between multiplayer mod clients.
        /// This handles player respawning and "game over"ing runs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ModMessageReceivedEventArgs"/> instance containing the event data.</param>
        public void ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            // Ignore all messages that aren't from this mod.
            if (e.FromModID != ModManifest.UniqueID)
                return;

            if (e.Type == "RespawnMessage") // Perform a respawn on this client
            {
                RespawnMessage message = e.ReadAs<RespawnMessage>();
                Stats.EndTime = null;

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
            else if (e.Type == "GameOver") // Signal that the game is over
                Roguelike.GameOver();
        }
    }
}