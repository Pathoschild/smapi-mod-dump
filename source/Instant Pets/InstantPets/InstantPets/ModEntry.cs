/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/InstantPets
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace InstantPets
{
	public class ModEntry : Mod
	{
		internal static Config Config;


		public override void Entry(IModHelper helper)
		{
			ModEntry.Config = helper.ReadConfig<Config>();
			this.Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
		}

		private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// Add on-day-started handler for instant pets
			this.Helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
			this.Helper.Events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;
			// Add GMCM entries
			this.AddGenericModConfigMenu();
			// Add SMAPI console commands
			ConsoleCommands.AddConsoleCommands(helper: this.Helper, monitor: this.Monitor);
		}

		private void AddGenericModConfigMenu()
		{
			var api = this.Helper.ModRegistry.GetApi
				<IGenericModConfigMenuAPI>
				("spacechase0.GenericModConfigMenu");
			if (api is not null)
			{
				api.Register(
					mod: this.ModManifest,
					reset: () => ModEntry.Config = new Config(),
					save: () => this.Helper.WriteConfig(ModEntry.Config));

				// Add all specified config properties
				foreach (KeyValuePair<string, string> entry in Config.ConfigPropertiesAndDescriptions)
				{
					// Add spaces in CamelCase names
					string name = System.Text.RegularExpressions.Regex.Replace(entry.Key, "([A-Z])", " $1").Trim();
					string description = entry.Value;
					System.Reflection.PropertyInfo property = typeof(Config).GetProperty(entry.Key);
					api.AddBoolOption(
						mod: this.ModManifest,
						name: () => name,
						tooltip: () => string.IsNullOrWhiteSpace(description) ? null : description,
						getValue: () => (bool)property.GetValue(ModEntry.Config),
						setValue: (bool value) => property.SetValue(ModEntry.Config, value: value));
				}
			}
		}

		private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			this.Helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
		}

		private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
		{
			this.Helper.Events.GameLoop.DayStarted -= this.GameLoop_DayStarted;

			if (Game1.IsMasterGame)
			{
				Utils.InstantPet(isMessageVisible: true);
				Utils.InstantCave(isMessageVisible: true);
			}
		}
	}
}
