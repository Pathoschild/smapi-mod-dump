/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/voltaek/StardewMods
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneyHarvestSync
{
	internal sealed class ModEntry : Mod
	{
		/// <summary>The mod configuration from the player.</summary>
		internal static ModConfig Config { get; private set; }

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// Read user's config
			Config = Helper.ReadConfig<ModConfig>();

			// Hold onto the monitor so we can do logging
			HoneyUpdater.Monitor = Monitor;

			// Rig up event handler to set up Generic Mod Config Menu integration
			Helper.Events.GameLoop.GameLaunched += OnGameLaunched;

			// Rig up the event handlers we need to do proper tracking of bee houses and flowers
			Helper.Events.GameLoop.DayStarted += HoneyUpdater.OnDayStarted;
			Helper.Events.GameLoop.TimeChanged += HoneyUpdater.OnTimeChanged;
			Helper.Events.GameLoop.OneSecondUpdateTicked += HoneyUpdater.OnOneSecondUpdateTicked;
			Helper.Events.World.ObjectListChanged += HoneyUpdater.OnObjectListChanged;
			Helper.Events.World.LocationListChanged += HoneyUpdater.OnLocationListChanged;
		}

		/// <summary>Event handler for when the game launches.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// get Generic Mod Config Menu's API (if it's installed)
			IGenericModConfigMenuApi configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

			if (configMenu is null)
			{
				return;
			}

			// Register mod
			configMenu.Register(
				mod: ModManifest,
				reset: () => Config = new ModConfig(),
				save: () => Helper.WriteConfig(Config)
			);

			// Add each config value
			configMenu.AddTextOption(
				mod: ModManifest,
				name: () => "Bee House Ready Icon",
				tooltip: () => "Controls icon type shown above bee houses with honey ready. 'Flower' (default) - flower that will flavor the honey. 'Honey' - artisan honey you'll get (artisan icons not included).",
				getValue: () => Config.BeeHouseReadyIcon,
				setValue: value => {
					string oldValue = Config.BeeHouseReadyIcon;
					Config.BeeHouseReadyIcon = value;

					Monitor.Log($"Updated {nameof(Config.BeeHouseReadyIcon)} config value via GMCM from '{oldValue}' to '{value}'", LogLevel.Debug);

					HoneyUpdater.RefreshBeeHouseHeldObjects();
				},
				allowedValues: Enum.GetNames<ModConfig.ReadyIcon>()
			);
		}
	}
}
