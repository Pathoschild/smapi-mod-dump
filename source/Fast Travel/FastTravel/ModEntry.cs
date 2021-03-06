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
using System.Text;

namespace FastTravel
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		public static ModConfig Config;
        private ITranslationHelper translationHelper;
        private CommandHandler CommandHandler;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Config = helper.ReadConfig<ModConfig>();
            this.translationHelper = helper.Translation;

            this.CommandHandler = new CommandHandler(this.Monitor, Config);

            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
			helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            helper.ConsoleCommands.Add("ft_helper", "Run commands to help in develop mode.", this.CommandHandler.HandleCommand);
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
                string message = translationHelper.Get("BALANCED_MODE");
                Game1.showGlobalMessage($"{message}: {Config.BalancedMode}");
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
					Game1.showGlobalMessage(translationHelper.Get("BALANCED_MODE_NEED_HORSE"));
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

                    if (Config.DebugMode)
                    {
                        StringBuilder reportDebug = new StringBuilder();
                        reportDebug.AppendLine("\n #### FastTravel Debug ####");
                        reportDebug.AppendLine($"   point.myID: {point.myID}");
                        reportDebug.AppendLine($"   point.name: {point.name}");
                        reportDebug.AppendLine($"   point.region: {point.region}");
                        reportDebug.AppendLine(" ###############");
                        this.Monitor.Log(reportDebug.ToString(), LogLevel.Warn);
                    }

                    // Make sure the location is valid
                    if (!FastTravelUtils.PointExistsInConfig(point))
					{
                        string message = translationHelper.Get("WARP_FAILED").ToString().Replace("{0}", $"[{point.name}]"); 
                        Monitor.Log(message, LogLevel.Warn);

                        // Right now this closes the map and opens the players bag and doesn't give
                        // the player any information in game about what just happened
                        // so we tell them a warp point wasnt found and close the menu.
                        Game1.showGlobalMessage(translationHelper.Get("WARP_NOT_FOUND"));
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


                    if (!CheckValidationBeforeTeleport(targetPoint, out string errorValidationMessage))
                    {
                        Game1.showGlobalMessage(errorValidationMessage);
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
					var newThread = new Thread(CheckIfWarped);
                    newThread.Start(targetLocation.Name);
				}
			}
		}

		private bool CheckBalancedTransition(FastTravelPoint targetPoint, out string errorMessage)
		{
			errorMessage = "";
			if (!Config.BalancedMode)
				return true;

			// Block fast travel to calico entirely when balanced
			if (targetPoint.GameLocationIndex == Consts.GameLocationDesertCalico)
			{
				errorMessage = translationHelper.Get("DISABLED").ToString().Replace("{0}", targetPoint.MapName);
				return false;	
			}
            
			return true;
		}

        private bool CheckValidationBeforeTeleport(FastTravelPoint targetPoint, out string errorMessage)
        {
            errorMessage = "";
            bool hasValidations = targetPoint.requires != null && targetPoint.requires?.mails.Length > 0;
            if (hasValidations)
            {
                var validateTargetPoint = FastTravelUtils.CheckPointRequiredMails(targetPoint.requires?.mails);
                if (!validateTargetPoint.isValid)
                {
                    errorMessage = translationHelper.Get($"mail.{validateTargetPoint.messageKeyId}");
                    return false;
                }
            }
            return true;
        }
		
		private void CheckIfWarped(object locationName)
		{
			var locName = (string) locationName;

			// We need to wait atleast 1.5 seconds to let the location change complete before checking for it.
			Thread.Sleep(Consts.MillisecondsToCheckIfWarped);

            // Check if we are at the new location and if its a festival day.
            bool hasFestivalToday = Game1.currentLocation.Name != locName && Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason);

            if (hasFestivalToday)
				Game1.showGlobalMessage(translationHelper.Get("TODAY_HAS_FESTIVAL"));
			else
            {
                string message = translationHelper.Get("WARP_FAILED").ToString().Replace("{0}", $"[{locName}]");
                this.Monitor.Log(message);
            }
		}
	}
}