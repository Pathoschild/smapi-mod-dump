
using MapPings.Framework;
using MapPings.Framework.Utilities;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;

namespace MapPings {

	public class ModEntry : Mod {

		private ModConfig modConfig;
		private MapOverlay mapOverlay;
		//private ModData modData;

		public static IMonitor ModMonitor { get; internal set; }
		public static IModHelper ModHelper { get; internal set; }
		public static Logger ModLogger { get; private set; }

		public override void Entry(IModHelper helper) {

			ModMonitor = Monitor;
			ModHelper = Helper;

			ModLogger = new Logger(Monitor, Path.Combine(helper.DirectoryPath, "logfile.txt"), false) {
				LogToOutput = LogOutput.Console
			};

			this.Monitor.Log("Loading mod config...", LogLevel.Trace);
			this.modConfig = helper.ReadConfig<ModConfig>();

			helper.ConsoleCommands.Add("ping_color", "Set the ping color. As the host player, can change other players ping color.\n\n" +
				"Usage: ping_color [player] <color>|<RGB>\n\n" +
					"- player:\n\tOptional. The player name. If omitted the target player will be you.\n\n" +
					"- value:\n\tThe color for the ping. Can be use a color name, the command 'color-list' shows all the available colors." +
					"\n\tThe color can be also specified in RGB." +
					"\n\te.g.: ping_color \"Player Name\" 153 50 204 ", this.PingColor);

			helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
			
		}

		private void PingColor(string command, string[] args) {

			if(mapOverlay != null) {

				if(args.Length > 0) {

					int index = 0;

					bool playerFound = false;
					Farmer targetPlayer = null;

					if(args.Length == 2 || args.Length == 4) {

						if(Game1.IsMasterGame) {

							string playerName = args[index];

							foreach(Farmer farmer in Game1.getAllFarmers()) {
								if(farmer.Name.Equals(playerName, StringComparison.CurrentCultureIgnoreCase)) {
									targetPlayer = farmer;
									playerFound = true;
								}
							}

							index++;

							if(!playerFound) {
								this.Monitor.Log($"Player '{playerName}' not found.");
							}

						} else {
							this.Monitor.Log($"You must be the host player to change other players color.");
						}

					} else {
						targetPlayer = Game1.player;
						playerFound = true;
					}

					if(playerFound) {
						Color? color = null;
						if(args.Length >= 3) {

							bool containsR = int.TryParse(args[index], out int r);
							index++;

							bool containsG = int.TryParse(args[index], out int g);
							index++;

							bool containsB = int.TryParse(args[index], out int b);

							if(containsR && containsG && containsB) {
								bool validValues = true;
								if(r < 0 || r > 255) {
									validValues = false;
									Monitor.Log("The R value must be between 0 and 255.");
								}
								if(g < 0 || g > 255) {
									validValues = false;
									Monitor.Log("The G value must be between 0 and 255.");
								}
								if(b < 0 && b < 256) {
									validValues = false;
									Monitor.Log("The B value must be between 0 and 255.");
								}
								if(validValues) {
									color = new Color(r, g, b);
								}
							}
						} else {
							color = ModUtilities.GetColorFromName(args[index]);
						}

						if(color.HasValue) {
							mapOverlay.MapPings[targetPlayer].SetPlayerPingColor(color.Value);
						}

					}

				}

			} else {
				this.Monitor.Log($"Missing parameters.\n\n" +
					"Usage: ping_color [player] <color>|<RGB>\n\n" +
					"- player:\n\tOptional. The player name. If omitted the target player will be you.\n\n" +
					"- value:\n\tThe color for the ping. Can be use a color name, the command 'color-list' shows all the available colors." +
					"\n\tThe color can be also specified in RGB." +
					"\n\te.g.: ping_color \"Player Name\" 153 50 204 "); //TODO Make more clear and implement feature
			}

		}

		// TODO: Make event manager for menu open/close

		/*********
		** Private methods
		*********/

		/// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {

#if DEBUG
			// Pause time and set it to 09:00
			Helper.ConsoleCommands.Trigger("world_freezetime", new string[] { "1" });
			Helper.ConsoleCommands.Trigger("world_settime", new string[] { "0900" });
#endif

			mapOverlay = new MapOverlay(this.Helper, this.modConfig);

		}

	}

}
