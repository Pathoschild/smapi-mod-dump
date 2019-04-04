
using CustomEmojis.Framework.Extensions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using CustomEmojis.Framework.Patches;
using CustomEmojis.Framework;
using CustomEmojis.Patches;
using CustomEmojis.Framework.Constants;
using CustomEmojis.Framework.Menu;
using System.IO;

namespace CustomEmojis {
	/// <summary>The mod entry class loaded by SMAPI.</summary>
	public class ModEntry : Mod {

		private EmojiAssetsLoader emojiAssetsLoader;
		private CachedMessageEmojis cachedMessages;
		private ModConfig config;
		private ModData modData;

		// TODO: Remove. Used for debugging
		public static IMonitor ModMonitor { get; private set; }
#if DEBUG
		public static Logger ModLogger { get; private set; }
		private ModDebugData modDebugData;
#endif

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper) {

			ModMonitor = Monitor;

#if DEBUG
			//Logger.InitLogger(helper.DirectoryPath + "\\logfile.txt", false, Monitor);
#endif

			#region Harmony Patches

			ModPatchControl patchControl = new ModPatchControl(helper);
			patchControl.PatchList.Add(new MultiplayerPatch.ProcessIncomingMessagePatch());
			patchControl.PatchList.Add(new MultiplayerPatch.AddPlayerPatch());
			patchControl.PatchList.Add(new MultiplayerPatch.PlayerDisconnectedPatch());
			//PatchControl.PatchList.Add(new MultiplayerPatch.SendChatMessagePatch());
			//PatchControl.PatchList.Add(new MultiplayerPatch.ReceiveChatMessagePatch());
			patchControl.PatchList.Add(new Game1Patch.DrawOverlaysPatch());
			/*
			PatchControl.PatchList.Add(new MultiplayerPatch.ReceivePlayerIntroductionPatch());
			*/

			IClassPatch receiveChatMessage = null;
			IClassPatch addMessagePatch = null;
			if(helper.ModRegistry.IsLoaded("cat.chatcommands")) {
				receiveChatMessage = new ModSupportPatch.ChatCommands_ReceiveChatMessage();
				addMessagePatch = new ModSupportPatch.ChatCommands_AddMessagePatch();
				patchControl.PatchList.Add(new ModSupportPatch.ChatCommands_AddConsoleMessagePatch(helper.Reflection));
			}

			patchControl.PatchList.Add(receiveChatMessage ?? new ChatBoxPatch.ReceiveChatMessagePatch());
			patchControl.PatchList.Add(addMessagePatch ?? new ChatBoxPatch.AddMessagePatch());

			patchControl.ApplyPatch();

			#endregion

			this.Monitor.Log("Loading mod config...", LogLevel.Trace);
			this.config = helper.ReadConfig<ModConfig>();

#if !DEBUG || TRUE
#if DEBUG
			ModLogger = new Logger(Path.Combine(helper.DirectoryPath, "logfile.txt"), false, Monitor);
#endif
			this.Monitor.Log("Loading mod data...", LogLevel.Trace);
			this.modData = this.Helper.Data.ReadJsonFile<ModData>(ModPaths.Data.Path);
			if(this.modData == null) {
				this.Monitor.Log("Mod data file not found. (harmless info)", LogLevel.Trace);
				this.modData = new ModData(helper, config.ImageExtensions) {
					WatchedPaths = new List<string>() {
						 ModPaths.Assets.InputFolder
					}
				};
			} else {
				modData.FileExtensionsFilter = config.ImageExtensions;
				modData.ModHelper = helper;
			}
			
			emojiAssetsLoader = new EmojiAssetsLoader(helper, modData, config, EmojiMenu.EMOJI_SIZE);

#else
			this.Monitor.Log("Loading debug data file...", LogLevel.Trace);
			this.modDebugData = this.Helper.Data.ReadJsonFile<ModDebugData>("debugData.json") ?? new ModDebugData();

			//string configPath = Path.Combine(Constants.ExecutionPath, "Mods", "SkipIntro", "config.json");
			//string json = File.ReadAllText(configPath);
			//dynamic jsonObj = JsonConvert.DeserializeObject(json);
			//if((string)(jsonObj["SkipTo"]) == "HostCoop") {
			//	jsonObj["SkipTo"] = "JoinCoop";
			//	modDebugData.IsHost = true;
			//} else {
			//	jsonObj["SkipTo"] = "HostCoop";
			//	modDebugData.IsHost = false;
			//}
			//string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
			//File.WriteAllText(configPath, output);

			if(modDebugData.ActAsHost()) {

				this.Helper.Data.WriteJsonFile("debugData.json", modDebugData);
				Monitor.Log($"====> HOST <====");

				ModLogger = new Logger(Path.Combine(helper.DirectoryPath, "logfile.txt"), false, Monitor);
				this.Monitor.Log("Loading mod data...", LogLevel.Trace);
				this.modData = this.Helper.Data.ReadJsonFile<ModData>(ModPaths.Data.Path);
				if(this.modData == null) {
					this.Monitor.Log("Mod data file not found. (harmless info)", LogLevel.Trace);
					this.modData = new ModData(helper, config.ImageExtensions) {
						WatchedPaths = new List<string>() {
							ModPaths.Assets.InputFolder
						}
					};
				} else {
					modData.FileExtensionsFilter = config.ImageExtensions;
					modData.ModHelper = helper;
				}

				emojiAssetsLoader = new EmojiAssetsLoader(helper, modData, config, EmojiMenu.EMOJI_SIZE);

			} else {
				this.Helper.Data.WriteJsonFile("debugData.json", modDebugData);
				Monitor.Log($"====> CLIENT <====");
				ModLogger = new Logger(Path.Combine(helper.DirectoryPath, "logfileClient.txt"), false, Monitor);
				ModPaths.Assets.InputFolder = ModPaths.Assets.InputFolder + "CLIENT";
				ModPaths.Assets.Folder = ModPaths.Assets.Folder + "CLIENT";
				ModPaths.Data.Path = "dataCLIENT.json";

				this.Monitor.Log("Loading mod data...", LogLevel.Trace);
				this.modData = this.Helper.Data.ReadJsonFile<ModData>(ModPaths.Data.Path);
				if(this.modData == null) {
					this.Monitor.Log("Mod data file not found. (harmless info)", LogLevel.Trace);
					this.modData = new ModData(helper, config.ImageExtensions) {
						WatchedPaths = new List<string>() {
						ModPaths.Assets.InputFolder
					}
					};
				} else {
					modData.FileExtensionsFilter = config.ImageExtensions;
					modData.ModHelper = helper;
				}

				emojiAssetsLoader = new EmojiAssetsLoader(helper, modData, config, EmojiMenu.EMOJI_SIZE);

			}
#endif

			helper.Content.AssetLoaders.Add(emojiAssetsLoader);

			//helper.ConsoleCommands.Add("reload_emojis", "Reload the game emojis with the new ones found in the mod folder.", this.ReloadEmojis);

			helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

		}

		/*********
		** Private methods
		*********/

		/// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnSaveLoaded(object sender, EventArgs e) {

#if DEBUG
			// Pause time and set it to 09:00
			Helper.ConsoleCommands.Trigger("world_freezetime", new string[] { "1" });
			Helper.ConsoleCommands.Trigger("world_settime", new string[] { "0900" });
			ModLogger.Log($"Player UniqueMultiplayerID: {Game1.player.UniqueMultiplayerID}");
#endif

			if(modData.ShouldSaveData()) {
				this.Monitor.Log("File changes detected. Saving mod data...", LogLevel.Trace);
				modData.FilesChanged = false;
				modData.DataChanged = false;
				this.Helper.Data.WriteJsonFile(ModPaths.Data.Path, modData);
				//emojiAssetsLoader.ReloadAsset(); // FIXME: Cache not invalidating properly
			} else {
				this.Monitor.Log("No file changes detected.", LogLevel.Trace);
			}

			this.Monitor.Log($"Custom emojis added: {emojiAssetsLoader.CustomTextureAdded}");
			if(emojiAssetsLoader.CustomTextureAdded) {
				this.Monitor.Log($"Custom emojis found: {emojiAssetsLoader.NumberCustomEmojisAdded}");
				this.Monitor.Log($"Total emojis counted by Stardew Valley: {EmojiMenu.totalEmojis}");
				emojiAssetsLoader.UpdateTotalEmojis();
				this.Monitor.Log($"Total emojis counted after ammount fix: {emojiAssetsLoader.TotalNumberEmojis}");
			}

			cachedMessages = new CachedMessageEmojis(Helper, emojiAssetsLoader.TotalNumberEmojis - emojiAssetsLoader.NumberCustomEmojisAdded);
			Game1.onScreenMenus.Add(cachedMessages);

			if(!Game1.IsMasterGame && emojiAssetsLoader.CustomTextureAdded) {
				Multiplayer multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
				multiplayer.RequestEmojiTexture();
			}

		}

		/// <summary>Reload the game emojis with the new ones found in the mod folder.</summary>
		/// <param name="command">The name of the command invoked.</param>
		/// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
		private void ReloadEmojis(string command, string[] args) {
			emojiAssetsLoader?.ReloadAsset();
		}

	}

}
