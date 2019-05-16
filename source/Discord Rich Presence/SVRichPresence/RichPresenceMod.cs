using Discord;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Constants = StardewModdingAPI.Constants;
using LogLevel = StardewModdingAPI.LogLevel;
using Utility = StardewValley.Utility;

namespace SVRichPresence {
	public class RichPresenceMod : Mod {
		private const long clientId = 444517509148966923;
		private const int steamId = 413150;
		private ModConfig config = new ModConfig();
		private IRichPresenceAPI api;
		private Discord.Discord discord;
		private ActivityManager activityManager;

		public override void Entry(IModHelper helper) {
#if DEBUG
			Monitor.Log("THIS IS A DEBUG BUILD...", LogLevel.Alert);
			Monitor.Log("...FOR DEBUGGING...", LogLevel.Alert);
			Monitor.Log("...AND STUFF...", LogLevel.Alert);
			if (ModManifest.Version.IsPrerelease()) {
				Monitor.Log("oh wait this is a pre-release.", LogLevel.Info);
				Monitor.Log("carry on.", LogLevel.Info);
			} else {
				Monitor.Log("If you're Fayne, keep up the good work. :)", LogLevel.Alert);
				Monitor.Log("If you're not Fayne...", LogLevel.Alert);
				Monitor.Log("...please go yell at Fayne...", LogLevel.Alert);
				Monitor.Log("...because you shouldn't have this...", LogLevel.Alert);
				Monitor.Log("...it's for debugging. (:", LogLevel.Alert);
			}
#else
			if (ModManifest.Version.IsPrerelease()) {
				Monitor.Log("WAIT A MINUTE.", LogLevel.Alert);
				Monitor.Log("FAYNE.", LogLevel.Alert);
				Monitor.Log("WHY DID YOU RELEASE A NON-DEBUG DEV BUILD?!", LogLevel.Alert);
				Monitor.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", LogLevel.Alert);
			}
#endif

			AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs e) => {
				try {
					var name = new AssemblyName(e.Name);
					foreach (FileInfo dll in new DirectoryInfo(Helper.DirectoryPath).EnumerateFiles("*.dll")) {
						if (name.Name.Equals(AssemblyName.GetAssemblyName(dll.FullName).Name, StringComparison.InvariantCultureIgnoreCase))
							return Assembly.LoadFrom(dll.FullName);
					}
				} catch { }
				return null;
			};

			api = new RichPresenceAPI(this);
			try {
				discord = new Discord.Discord(clientId, (ulong)CreateFlags.NoRequireDiscord);
			} catch (Discord.ResultException e) {
				Monitor.Log("Failed to initialize Discord SDK: " + e.Message, LogLevel.Error);
				Monitor.Log("Rich Presence cannot be activated. Restart the game to try again.", LogLevel.Error);
				Dispose();
				return;
			}
			discord.SetLogHook(Discord.LogLevel.Debug, (Discord.LogLevel level, string message) => {
				LogLevel sdlevel = LogLevel.Trace;
				switch (level) {
					case Discord.LogLevel.Error:
						sdlevel = LogLevel.Error;
						break;
					case Discord.LogLevel.Warn:
						sdlevel = LogLevel.Warn;
						break;
					case Discord.LogLevel.Info:
						sdlevel = LogLevel.Info;
						break;
					case Discord.LogLevel.Debug:
						sdlevel = LogLevel.Debug;
						break;
				};
				Monitor.Log("DISCORD: " + message, sdlevel);
			});
			discord.GetUserManager().OnCurrentUserUpdate += () => {
				User user = discord.GetUserManager().GetCurrentUser();
				Monitor.Log("Connected to Discord: " + GetDiscordTag(user, true), LogLevel.Info);
			};
			activityManager = discord.GetActivityManager();
			activityManager.RegisterSteam(steamId);

			activityManager.OnActivityJoin += (string secret) => {
				Monitor.Log("Attempting to join game", LogLevel.Info);
				JoinGame(secret);
			};

			activityManager.OnActivityJoinRequest += (ref User user) => {
				string tag = GetDiscordTag(user);
				Monitor.Log(tag + " is requesting to join your game.", LogLevel.Alert);
				Monitor.Log("You can respond to this request in Discord Overlay.", LogLevel.Info);
				Game1.chatBox.addInfoMessage(tag + " is requesting to join your game. You can respond to this request in Discord Overlay.");
			};

			Helper.ConsoleCommands.Add("DiscordRP_TestJoin",
				"Command for debugging.",
				(string command, string[] args) => {
					JoinGame(string.Join(" ", args));
				}
			);
			Helper.ConsoleCommands.Add("DiscordRP_Reload",
				"Reloads the config for Discord Rich Presence.",
				(string command, string[] args) => {
					LoadConfig();
					Monitor.Log("Config reloaded.", LogLevel.Info);
				}
			);
			Helper.ConsoleCommands.Add("DiscordRP_Format",
				"Formats and prints a provided configuration string.",
				(string command, string[] args) => {
					string text = api.FormatText(string.Join(" ", args));
					Monitor.Log("Result: " + text, LogLevel.Info);
				}
			);
			Helper.ConsoleCommands.Add("DiscordRP_Tags",
				"Lists tags usable for configuration strings.",
				(string command, string[] args) => {
					IDictionary<string, string> tags =
						string.Join("", args).ToLower().StartsWith("all") ?
						api.ListTags("[NULL]", "[ERROR]") : api.ListTags(removeNull: false);
					IDictionary<string, IDictionary<string, string>> groups =
						new Dictionary<string, IDictionary<string, string>>();
					foreach (KeyValuePair<string, string> tag in tags) {
						string owner = api.GetTagOwner(tag.Key) ?? "Unknown-Mod";
						if (!groups.ContainsKey(owner))
							groups[owner] = new Dictionary<string, string>();
						groups[owner][tag.Key] = tag.Value;
					}
					IList<string> output = new List<string>(tags.Count + groups.Count) {
						"Available Tags:"
					};
					int longest = 0;
					foreach (KeyValuePair<string, string> tag in groups[ModManifest.UniqueID])
						if (tag.Value != null)
							longest = Math.Max(longest, tag.Key.Length);
					int nulls = 0;
					foreach (KeyValuePair<string, string> tag in groups[ModManifest.UniqueID])
						if (tag.Value is null) nulls++;
						else output.Add("  {{ " + tag.Key.PadLeft(longest) + " }}: " + tag.Value);
					foreach (KeyValuePair<string, IDictionary<string, string>> group in groups) {
						if (group.Key == ModManifest.UniqueID)
							continue;
						string head = group.Value.Count + " tag";
						if (group.Value.Count != 1)
							head += "s";
						head += " from " + (Helper.ModRegistry.Get(group.Key)?.Manifest.Name ?? "an unknown mod");
						output.Add(head);
						longest = 0;
						foreach (KeyValuePair<string, string> tag in group.Value)
							if (tag.Value != null)
								longest = Math.Max(longest, tag.Key.Length);
						foreach (KeyValuePair<string, string> tag in group.Value)
							if (tag.Value == null) nulls++;
							else output.Add("  {{ " + tag.Key.PadLeft(longest) + " }}: " + tag.Value);
					}
					if (nulls > 0)
						output.Add(nulls + " tag" + (nulls != 1 ? "s" : "") + " unavailable; type `DiscordRP_Tags all` to show all");
					Monitor.Log(string.Join(Environment.NewLine, output), LogLevel.Info);
				}
			);
			LoadConfig();

			Helper.Events.Input.ButtonReleased += HandleButton;
			Helper.Events.GameLoop.UpdateTicked += DoUpdate;
			Helper.Events.GameLoop.SaveLoaded += SetTimestamp;
			Helper.Events.GameLoop.ReturnedToTitle += SetTimestamp;
			Helper.Events.GameLoop.SaveLoaded += (object sender, SaveLoadedEventArgs e) =>
				api.GamePresence = "Getting Started";
			Helper.Events.GameLoop.SaveCreated += (object sender, SaveCreatedEventArgs e) =>
				api.GamePresence = "Starting a New Game";
			Helper.Events.GameLoop.GameLaunched += (object sender, GameLaunchedEventArgs e) => {
				SetTimestamp();
				timestampSession = Now();
			};

			ITagRegister tagReg = api.GetTagRegister(this);

			tagReg.SetTag("Activity", () => api.GamePresence);
			tagReg.SetTag("ModCount", () => Helper.ModRegistry.GetAll().Count());
			tagReg.SetTag("SMAPIVersion", () => Constants.ApiVersion.ToString());
			tagReg.SetTag("StardewVersion", () => Game1.version);
			tagReg.SetTag("Song", () => Utility.getSongTitleFromCueName(Game1.currentSong?.Name ?? api.None));

			// All the tags below are only available while in-game.

			tagReg.SetTag("Name", () => Game1.player.Name, true);
			tagReg.SetTag("Farm", () => Game1.content.LoadString("Strings\\UI:Inventory_FarmName", api.GetTag("FarmName")), true);
			tagReg.SetTag("FarmName", () => Game1.player.farmName, true);
			tagReg.SetTag("PetName", () => Game1.player.hasPet() ? Game1.player.getPetDisplayName() : api.None, true);
			tagReg.SetTag("Location", () => Game1.currentLocation.Name, true);
			tagReg.SetTag("RomanticInterest", () => Utility.getTopRomanticInterest(Game1.player)?.getName() ?? api.None, true);
			tagReg.SetTag("PercentComplete", () => Utility.percentGameComplete(), true);

			tagReg.SetTag("Money", () => {
				// Copied from LoadGameMenu
				string text = Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", Utility.getNumberWithCommas(Game1.player.Money));
				if (Game1.player.Money == 1 && LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt)
					text = text.Substring(0, text.Length - 1);
				return text;
			}, true);
			tagReg.SetTag("MoneyNumber", () => Game1.player.Money, true);
			tagReg.SetTag("MoneyCommas", () => Utility.getNumberWithCommas(Game1.player.Money), true);
			tagReg.SetTag("Level", () => Game1.content.LoadString("Strings\\UI:Inventory_PortraitHover_Level", Game1.player.Level.ToString()), true);
			tagReg.SetTag("LevelNumber", () => Game1.player.Level, true);
			tagReg.SetTag("Title", () => Game1.player.getTitle(), true);
			tagReg.SetTag("TotalTime", () => Utility.getHoursMinutesStringFromMilliseconds(Game1.player.millisecondsPlayed), true);

			tagReg.SetTag("Health", () => Game1.player.health, true);
			tagReg.SetTag("HealthMax", () => Game1.player.maxHealth, true);
			tagReg.SetTag("HealthPercent", () => (double)Game1.player.health / Game1.player.maxHealth * 100, 2, true);
			tagReg.SetTag("Energy", () => Game1.player.Stamina.ToString(), true);
			tagReg.SetTag("EnergyMax", () => Game1.player.MaxStamina, true);
			tagReg.SetTag("EnergyPercent", () => (double)Game1.player.Stamina / Game1.player.MaxStamina * 100, 2, true);

			tagReg.SetTag("Time", () => Game1.getTimeOfDayString(Game1.timeOfDay), true);
			tagReg.SetTag("Date", () => Utility.getDateString(), true);
			tagReg.SetTag("Season", () => Utility.getSeasonNameFromNumber(Utility.getSeasonNumber(SDate.Now().Season)), true);
			tagReg.SetTag("DayOfWeek", () => Game1.shortDayDisplayNameFromDayOfSeason(SDate.Now().Day), true);

			tagReg.SetTag("Day", () => SDate.Now().Day, true);
			tagReg.SetTag("DayPad", () => $"{SDate.Now().Day:00}", true);
			tagReg.SetTag("DaySuffix", () => Utility.getNumberEnding(SDate.Now().Day), true);
			tagReg.SetTag("Year", () => SDate.Now().Year, true);
			tagReg.SetTag("YearSuffix", () => Utility.getNumberEnding(SDate.Now().Year), true);

			tagReg.SetTag("GameVerb", () =>
				Context.IsMultiplayer && Context.IsMainPlayer ? "Hosting" : "Playing", true);
			tagReg.SetTag("GameNoun", () => Context.IsMultiplayer ? "Co-op" : "Solo", true);
			tagReg.SetTag("GameInfo", () => api.GetTag("GameVerb") + " " + api.GetTag("GameNoun"), true);
		}

		private void JoinGame(string inviteCode) {
			object lobby = Program.sdk.Networking.GetLobbyFromInviteCode(inviteCode);
			if (lobby == null) return;
			Game1.ExitToTitle(() => {
				TitleMenu.subMenu = new FarmhandMenu(Program.sdk.Networking.CreateClient(lobby));
			});
		}

		public override object GetApi() => api;

		private void HandleButton(object sender, ButtonReleasedEventArgs e) {
			if (e.Button != config.ReloadConfigButton)
				return;
			try {
				LoadConfig();
				Game1.addHUDMessage(new HUDMessage("DiscordRP config reloaded.", HUDMessage.newQuest_type));
			} catch (Exception err) {
				Game1.addHUDMessage(new HUDMessage("Failed to reload DiscordRP config. Check console.", HUDMessage.error_type));
				Monitor.Log(err.ToString(), LogLevel.Error);
			}
		}

		private int Now() => (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

		private void LoadConfig() => config = Helper.ReadConfig<ModConfig>();
		private void SaveConfig() => Helper.WriteConfig(config);

		private long timestampSession;
		private long timestampFarm;
		private void SetTimestamp(object sender, EventArgs e) => SetTimestamp();
		private void SetTimestamp() => timestampFarm = Now();

		private void DoUpdate(object sender, UpdateTickedEventArgs e) {
			try {
				discord.RunCallbacks();
				if (e.IsMultipleOf(30))
					activityManager.UpdateActivity(GetActivity(), (result) => {
						if (result != Result.Ok)
							Monitor.Log("Update Activity: " + result);
					});
			} catch { }
		}

		private MenuPresence Conf => !Context.IsWorldReady ?
			config.MenuPresence : config.GamePresence;

		private Discord.Activity GetActivity() {
			var activity = new Activity {
				Details = api.FormatText(Conf.Details),
				State = api.FormatText(Conf.State),
				Assets = {
					LargeImage = "default_large",
					LargeText = api.FormatText(Conf.LargeImageText),
					SmallText = api.FormatText(Conf.SmallImageText)
				}
			};
			if (Conf.ForceSmallImage || activity.Assets.SmallText.Length > 0)
				activity.Assets.SmallImage = "default_small";

			if (Context.IsWorldReady) {
				var conf = (GamePresence)Conf;
				if (conf.ShowSeason)
					activity.Assets.LargeImage = $"{Game1.currentSeason}_{FarmTypeKey()}";
				if (conf.ShowWeather)
					activity.Assets.SmallImage = "weather_" + WeatherKey();
				if (conf.ShowPlayTime)
					activity.Timestamps.Start = timestampFarm;
				if (Context.IsMultiplayer && conf.AllowAskToJoin)
					try {
						activity.Party.Id = Game1.MasterPlayer.UniqueMultiplayerID.ToString();
						activity.Party.Size.CurrentSize = Game1.numberOfPlayers();
						activity.Party.Size.MaxSize = Game1.getFarm().getNumberBuildingsConstructed("Cabin") + 1;
						activity.Secrets.Join = Game1.server.getInviteCode();
					} catch { }
			}

			if (config.ShowGlobalPlayTime)
				activity.Timestamps.Start = timestampSession;

			return activity;
		}

		private string FarmTypeKey() {
			if (!((GamePresence)Conf).ShowFarmType)
				return "default";
			switch (Game1.whichFarm) {
				case Farm.default_layout:
					return "standard";
				case Farm.riverlands_layout:
					return "riverland";
				case Farm.forest_layout:
					return "forest";
				case Farm.mountains_layout:
					return "hilltop";
				case Farm.combat_layout:
					return "wilderness";
				default:
					return "default";
			}
		}

		private string WeatherKey() {
			if (Game1.isRaining)
				return Game1.isLightning ? "stormy" : "rainy";
			if (Game1.isDebrisWeather)
				return "windy_" + Game1.currentSeason;
			if (Game1.isSnowing)
				return "snowy";
			if (Game1.weddingToday)
				return "wedding";
			if (Game1.isFestival())
				return "festival";
			return "sunny";
		}

		private string GetDiscordTag(User user, bool includeId = false) {
			string ret = user.Username + "#" + user.Discriminator;
			if (includeId)
				ret += " (" + user.Id + ")";
			return ret;
		}
	}
}
