using DiscordRPC;
using DiscordRPC.Message;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using Constants = StardewModdingAPI.Constants;
using LogLevel = StardewModdingAPI.LogLevel;
using Utility = StardewValley.Utility;

namespace SVRichPresence {
	public class RichPresenceMod : Mod {
		private static readonly Color blurple = new Color(114, 137, 218);
		private static readonly string clientId = "444517509148966923";
		private static readonly string steamId = "413150";
		private readonly Random rand = new Random();
		private ModConfig config = new ModConfig();
		private IRichPresenceAPI api;
		private DiscordRpcClient client;
		private readonly JoinRequestMessage[] requests = new JoinRequestMessage[ushort.MaxValue + 1];
		private ushort lastRequestID = 0;

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
				Monitor.Log("https://youtu.be/T3djXcx2ewQ", LogLevel.Alert);
			}
#endif
			if (Constants.TargetPlatform == GamePlatform.Android) {
				Monitor.Log("Discord RPC is not supported on Android.", LogLevel.Error);
				Monitor.Log("Aborting mod initialization.", LogLevel.Error);
				Dispose();
				return;
			}

			api = new RichPresenceAPI(this);
			client = new DiscordRpcClient(clientId,
				autoEvents: false,
				logger: new MonitorLogger(Monitor));
			client.RegisterUriScheme(steamId);
			client.OnReady += (sender, e) => {
				Monitor.Log("Connected to Discord: " + e.User.ToString(), LogLevel.Info);
			};
			client.OnJoin += (sender, args) => {
				Monitor.Log("Attempting to join game: " + args.Secret, LogLevel.Info);
				JoinGame(args.Secret);
			};
			client.OnJoinRequested += (sender, msg) => {
				string name = msg.User.Username;
				string tag = msg.User.ToString();
				ushort id = (ushort)rand.Next(ushort.MinValue, ushort.MaxValue);
				requests[id] = msg;
				lastRequestID = id;
				string hex = id.ToString("X");
				Monitor.Log(tag + " wants to join your game via Discord.", LogLevel.Alert);
				Monitor.Log("To respond type \"discord " + hex + " yes/no\" or just \"discord yes/no\"", LogLevel.Info);
				Game1.chatBox.addMessage(name + " wants to join your game via Discord.\nTo respond check the console or use Discord or its overlay.", blurple);
			};
			client.Initialize();
			client.SetSubscription(EventType.Join | EventType.JoinRequest);

			#region Console Commands
			Helper.ConsoleCommands.Add("discord",
				"Respond to a Discord join request.",
				(command, args) => {
					// Yes, I know this code is a mess.
					switch (args[0].ToLower()) {
						case "yes":
						case "y":
							Respond(lastRequestID, true);
							break;
						case "no":
						case "n":
							Respond(lastRequestID, false);
							break;
						default:
							try {
								var id = ushort.Parse(args[0], System.Globalization.NumberStyles.HexNumber);
								switch (args[1].ToLower()) {
									case "yes":
									case "y":
										Respond(id, true);
										break;
									case "no":
									case "n":
										Respond(id, false);
										break;
									default:
										Monitor.Log("Invalid response.", LogLevel.Error);
										break;
								}
							} catch (Exception) {
								Monitor.Log("Invalid request ID.", LogLevel.Error);
							}
							break;
					}
				}
			);
			Helper.ConsoleCommands.Add("DiscordRP_Join",
				"Join a co-op game via invite code.",
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
			#endregion
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
				timestampSession = Timestamps.Now;
			};

			ITagRegister tagReg = api.GetTagRegister(this);

			#region Default Tags

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
			#endregion
		}

		private void Respond(ushort id, Boolean response) {
			var request = requests[id];
			if (request == null)
				Monitor.Log("Request ID doesn't exist.", LogLevel.Error);
			client.Respond(request, response);
			Monitor.Log("Responding to join request. This may fail if 30 seconds have passed.", LogLevel.Info);
			requests[id] = null;
		}

		private void JoinGame(string inviteCode) {
			Game1.ExitToTitle(() => {
				object lobby = Program.sdk.Networking.GetLobbyFromInviteCode(inviteCode);
				if (lobby == null) return;
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

		private void LoadConfig() => config = Helper.ReadConfig<ModConfig>();

		private void SaveConfig() => Helper.WriteConfig(config);

		private Timestamps timestampSession;
		private Timestamps timestampFarm;
		private void SetTimestamp(object sender, EventArgs e) => SetTimestamp();
		private void SetTimestamp() => timestampFarm = Timestamps.Now;

		private void DoUpdate(object sender, UpdateTickedEventArgs e) {
			client.Invoke();
			if (e.IsMultipleOf(30))
				client.SetPresence(GetPresence());
		}

		private MenuPresence Conf => !Context.IsWorldReady ?
			config.MenuPresence : config.GamePresence;

		private RichPresence GetPresence() {
			var presence = new RichPresence {
				Details = api.FormatText(Conf.Details),
				State = api.FormatText(Conf.State)
			};
			var assets = new Assets {
				LargeImageKey = "default_large",
				LargeImageText = api.FormatText(Conf.LargeImageText),
				SmallImageText = api.FormatText(Conf.SmallImageText)
			};
			if (Conf.ForceSmallImage || assets.SmallImageText?.Length > 0)
				assets.SmallImageKey = "default_small";

			if (Context.IsWorldReady) {
				var conf = (GamePresence)Conf;
				if (conf.ShowSeason)
					assets.LargeImageKey = $"{Game1.currentSeason}_{FarmTypeKey()}";
				if (conf.ShowWeather)
					assets.SmallImageKey = "weather_" + WeatherKey();
				if (conf.ShowPlayTime)
					presence.Timestamps = timestampFarm;
				if (Context.IsMultiplayer && conf.AllowAskToJoin)
					try {
						presence.Party = new Party {
							ID = Game1.MasterPlayer.UniqueMultiplayerID.ToString(),
							Size = Game1.numberOfPlayers(),
							Max = Game1.getFarm().getNumberBuildingsConstructed("Cabin") + 1
						};
						presence.Secrets = new Secrets {
							JoinSecret = Game1.server.getInviteCode()
						};
					} catch { }
			}

			if (config.ShowGlobalPlayTime)
				presence.Timestamps = timestampSession;

			return presence.WithAssets(assets);
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
	}
}
