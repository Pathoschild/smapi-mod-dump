using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SVRichPresence {
	public class RichPresenceMod : Mod {
		private const string applicationId = "444517509148966923";
		private ModConfig config = new ModConfig();
		private IRichPresenceAPI api;

		public override void Entry(IModHelper helper) {
#if DEBUG
			Monitor.Log("THIS IS A DEBUG BUILD...", LogLevel.Alert);
			Monitor.Log("...FOR DEBUGGING...", LogLevel.Alert);
			Monitor.Log("...AND STUFF...", LogLevel.Alert);
			if (ModManifest.Version.IsPrerelease()) {
				Monitor.Log("oh wait this is a dev build.", LogLevel.Info);
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
			api = new RichPresenceAPI(this);
			DiscordRpc.EventHandlers handlers = new DiscordRpc.EventHandlers();
			DiscordRpc.Initialize(applicationId, ref handlers, false, "413150");
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
						if (group.Key == this.ModManifest.UniqueID)
							continue;
						string head = group.Value.Count + " tag";
						if (group.Value.Count != 1)
							head += "s";
						head += " from " + (Helper.ModRegistry.Get(group.Key)?.Name ?? "an unknown mod");
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
			InputEvents.ButtonReleased += HandleButton;
			GameEvents.HalfSecondTick += DoUpdate;
			SaveEvents.AfterLoad += SetTimestamp;
			SaveEvents.AfterReturnToTitle += SetTimestamp;
			SaveEvents.AfterLoad += (object sender, EventArgs e) =>
				api.GamePresence = "Getting Started";
			SaveEvents.AfterCreate += (object sender, EventArgs e) =>
				api.GamePresence = "Starting a New Game";
			GameEvents.FirstUpdateTick += (object sender, EventArgs e) => {
				SetTimestamp();
				timestampSession = GetTimestamp();
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
			tagReg.SetTag("HealthPercent", () => (double) Game1.player.health / Game1.player.maxHealth * 100, 2, true);
			tagReg.SetTag("Energy", () => Game1.player.Stamina.ToString(), true);
			tagReg.SetTag("EnergyMax", () => Game1.player.MaxStamina, true);
			tagReg.SetTag("EnergyPercent", () => (double) Game1.player.Stamina / Game1.player.MaxStamina * 100, 2, true);

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

		public override object GetApi() => api;

		private void HandleButton(object sender, EventArgsInput e) {
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
		private void SaveConfig() => Helper.WriteConfig<ModConfig>(config);

		private long timestampSession = 0;
		private long timestampFarm = 0;
		private void SetTimestamp(object sender, EventArgs e) => SetTimestamp();
		private void SetTimestamp() => timestampFarm = GetTimestamp();

		private long GetTimestamp() {
			DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return Convert.ToInt64((DateTime.UtcNow - epoch).TotalSeconds);
		}

		private void DoUpdate(object sender, EventArgs e) =>
			DiscordRpc.UpdatePresence(GetPresence());

		private MenuPresence Conf => !Context.IsWorldReady ?
			config.MenuPresence : config.GamePresence;

		private DiscordRpc.RichPresence GetPresence() {
			DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence {
				details = api.FormatText(Conf.Details),
				state = api.FormatText(Conf.State),
				largeImageKey = "default_large",
				largeImageText = api.FormatText(Conf.LargeImageText),
				smallImageText = api.FormatText(Conf.SmallImageText)
			};
			if (Conf.ForceSmallImage)
				presence.smallImageKey = "default_small";
			if (presence.smallImageText != null)
				presence.smallImageKey = presence.smallImageKey ?? "default_small";

			if (Context.IsWorldReady) {
				GamePresence conf = (GamePresence) Conf;
				if (conf.ShowSeason)
					presence.largeImageKey = $"{Game1.currentSeason}_{FarmTypeKey()}";
				if (conf.ShowWeather)
					presence.smallImageKey = "weather_" + WeatherKey();
				if (conf.ShowPlayTime)
					presence.startTimestamp = timestampFarm;
				if (Context.IsMultiplayer && conf.ShowPlayerCount) {
					presence.partyId = Game1.MasterPlayer.UniqueMultiplayerID.ToString();
					presence.partySize = Game1.numberOfPlayers();
					presence.partyMax = Game1.getFarm().getNumberBuildingsConstructed("Cabin") + 1;
					presence.joinSecret = Game1.server.getInviteCode();
				}
			}

			if (config.ShowGlobalPlayTime)
				presence.startTimestamp = timestampSession;

			return presence;
		}

		private string FarmTypeKey() {
			if (!((GamePresence) Conf).ShowFarmType)
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
