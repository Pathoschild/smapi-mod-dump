/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DeathGameDev/SDV-FastTravel
**
*************************************************/

using System.Collections.Generic;
using System.Threading;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace FastTravel
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		public static ModConfig Config;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Config = helper.ReadConfig<ModConfig>();

			helper.Events.GameLoop.DayStarted += this.OnDayStarted;
			helper.Events.Input.ButtonPressed += this.OnButtonPressed;
		}

		/// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			// We need to perform atleast one 10 min clock update before allowing fast travel
			// so set the time back 10 mins and then perform one update when the day has started.
			Game1.timeOfDay -= 10;
			Game1.performTenMinuteClockUpdate();
		}

		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (!Context.IsWorldReady)
				return;

			// toggle balanced mode
			if (e.Button == SButton.N)
			{
				Config.BalancedMode = !Config.BalancedMode;
				Game1.showGlobalMessage($"Balanced Mode: {Config.BalancedMode}");
				return;
			}

			// handle map click
			if (e.Button == SButton.MouseLeft && Game1.activeClickableMenu is GameMenu menu && menu.currentTab == GameMenu.mapTab)
			{
				// Get the map page from the menu.
				var mapPage = (Helper.Reflection.GetField<List<IClickableMenu>>(menu, "pages").GetValue()[3]) as MapPage;
				if (mapPage == null) // Gotta be safe
					return;

				// Do balanced behavior.
				// (This is done after getting the map/menu to prevent spamming notifications when the player isn't in the menu)
				if (Config.BalancedMode && Game1.player.mount == null)
				{
					Game1.showGlobalMessage("You can't fast travel without a horse!");
					Game1.exitActiveMenu();
					return;
				}

				int x = (int) e.Cursor.ScreenPixels.X;
				int y = (int) e.Cursor.ScreenPixels.Y;
				foreach (ClickableComponent point in mapPage.points)
				{
					// If the player isn't hovering over this point, don't worry about anything.
					if (!point.containsPoint(x, y))
						continue;

					// Lonely Stone is blocked because it's not an actual place
					// TODO - Fix the visual bug with Quarry
					if (point.name == "Lonely Stone")
						continue;

					// Make sure the location is valid
					if (!FastTravelUtils.PointExistsInConfig(point))
					{
						Monitor.Log($"Failed to find a warp for point [{point.name}]!", LogLevel.Warn);

						// Right now this closes the map and opens the players bag and doesn't give
						// the player any information in game about what just happened
						// so we tell them a warp point wasnt found and close the menu.
						Game1.showGlobalMessage("No warp point found.");
						Game1.exitActiveMenu();
						continue;
					}

					// Get the location, and warp the player to it's first warp.
					GameLocation targetLocation = FastTravelUtils.GetLocationForMapPoint(point);
					FastTravelPoint targetPoint = FastTravelUtils.GetFastTravelPointForMapPoint(point);

					if (!CheckBalancedTransition(targetPoint, out string errorMessage))
					{
						Game1.showGlobalMessage(errorMessage);
						Game1.exitActiveMenu();
						return;
					}

					// Dismount the player if they're going to calico desert, since the bus glitches with mounts.
					if (targetPoint.GameLocationIndex == 28 && Game1.player.mount != null)
						Game1.player.mount.dismount();
					
					// Warp the player to their location, and exit the map.
					Game1.warpFarmer(targetPoint.RerouteName ?? targetLocation.Name, targetPoint.SpawnPosition.X, targetPoint.SpawnPosition.Y, false);
					Game1.exitActiveMenu();

					// Lets check for warp status and give the player feed back on what happened to the warp.
					// We are doing this check on a thread because we have to wait until the warp has finished
					// to check its result.
					var locationNames = new[] {targetPoint.RerouteName, targetLocation.Name};
					var t1 = new Thread(CheckIfWarped);
					t1.Start(locationNames);
				}
			}
		}

		// TODO - Convert inline int/string declarations to a constant class
		private bool CheckBalancedTransition(FastTravelPoint targetPoint, out string errorMessage)
		{
			errorMessage = "";
			if (!Config.BalancedMode)
				return true;

			// Block fast travel to calico entirely when balanced
			if (targetPoint.GameLocationIndex == 28)
			{
				errorMessage = "Fast-Travel to Calico Desert is disabled in balanced mode!";
				return false;	
			}

			// Block fast travel to the mines unless it has been unlocked.
			if (targetPoint.GameLocationIndex == 25 && !Game1.player.mailReceived.Contains("CF_Mines"))
			{
				errorMessage = "You must unlock the Mines before fast travel is available in balanced mode!";
				return false;
			}

			return true;
		}
		
		private void CheckIfWarped(object locationNames)
		{
			var locNames = (string[]) locationNames;

			// We need to wait atleast 1.5 seconds to let the location change complete before checking for it.
			Thread.Sleep(1500);

			// If RerouteName is null we want the LocationName instead.
			// 0 = RerouteName, 1 = LocationName
			var tmpLocName = locNames[0] ?? locNames[1];

			// Check if we are at the new location and if its a festival day.
			if (Game1.currentLocation.Name != tmpLocName && Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
				// If there is a festival and we werent able to warp let the player know.
				Game1.showGlobalMessage("Today's festival is being set up. Try going later.");
			else
				// Finally, if we managed to warp log that we were warped.
				this.Monitor.Log($"Warping player to {tmpLocName}");
		}
	}
}