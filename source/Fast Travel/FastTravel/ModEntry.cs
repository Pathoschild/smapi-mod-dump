using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Threading;

namespace FastTravel
{
    /// <summary>
    /// The mod entry point.
    /// </summary>
    public class ModEntry : Mod
    {
        public static ModConfig Config;

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            ControlEvents.MouseChanged += this.ControlEvents_MouseChanged;
            ControlEvents.KeyPressed += this.ControlEvents_KeyPressed;

            // We need to perform atleast one 10 min clock update before allowing fast travel
            // so set the time back 10 mins and then perform one update when the day has started.
            TimeEvents.AfterDayStarted += (sender, args) =>
            {
                Game1.timeOfDay -= 10;
                Game1.performTenMinuteClockUpdate();
            };

        }

        private void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            // If the world isn't ready, or the player didn't left click, we have nothing to work with.
            if (!Context.IsWorldReady || e.NewState.LeftButton != ButtonState.Pressed)
                return;

            // Create a reference to the current menu, and make sure it isn't null.
            var menu = (Game1.activeClickableMenu as GameMenu);
            if (menu == null || menu.currentTab != GameMenu.mapTab) // Also make sure it's on the right tab(Map)
                return;

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

            int x = Game1.getMouseX();
            int y = Game1.getMouseY();
            foreach (ClickableComponent point in mapPage.points)
            {
                // If the player isn't hovering over this point, don't worry about anything.
                if (!point.containsPoint(x, y))
                    continue;

                // Lonely Stone is blocked because it's not an actual place
                // Quarry is blocked because it's broken currently.
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
                    Game1.showGlobalMessage($"No warp point found.");
                    Game1.exitActiveMenu();
                    continue;
                }

                // Get the location, and warp the player to it's first warp.
                var location = FastTravelUtils.GetLocationForMapPoint(point);
                var fastTravelPoint = FastTravelUtils.GetFastTravelPointForMapPoint(point);

                // If the player is in balanced mode, block warping to calico altogether.
                if (Config.BalancedMode && fastTravelPoint.GameLocationIndex == 28)
                {
                    Game1.showGlobalMessage("Fast-Travel to Calico Desert is disabled in balanced mode!");
                    Game1.exitActiveMenu();
                    return;
                }

                // Dismount the player if they're going to calico desert, since the bus glitches with mounts.
                if (fastTravelPoint.GameLocationIndex == 28 && Game1.player.mount != null)
                    Game1.player.mount.dismount();

                // Warp the player to their location, and exit the map.
                Game1.warpFarmer(fastTravelPoint.RerouteName == null ? location.Name : fastTravelPoint.RerouteName, fastTravelPoint.SpawnPosition.X, fastTravelPoint.SpawnPosition.Y, false);
                Game1.exitActiveMenu();

                // Lets check for warp status and give the player feed back on what happened to the warp.
                // We are doing this check on a thread because we have to wait untill the warp has finished
                // to check its result.
                var locationNames = new String[] {fastTravelPoint.RerouteName, location.Name};
                var t1 = new Thread(new ParameterizedThreadStart(CheckIfWarped));
                t1.Start(locationNames);
            }
        }

        private void CheckIfWarped(object locationNames)
        {
            var locNames = (string[]) locationNames;

            // We need to wait atleast 1.5 seconds to let the location change be complet before checking for it.
            Thread.Sleep(1500);

            // If RerouteName is null we want the LocationName instead.
            // 0 = RerouteName, 1 = LocationName
            var tmpLocName = locNames[0] ?? locNames[1];

            // Check if we are at the new location and if its a festival day.
            if (Game1.currentLocation.Name != tmpLocName && Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
                // If there is a festival and we werent able to warp let the player know.
                Game1.showGlobalMessage($"Today's festival is being set up. Try going later.");
            else
                // Finally, if we managed to warp log that we were warped.
                this.Monitor.Log($"Warping player to " + tmpLocName);
        }

        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            switch (e.KeyPressed)
            {
                case Keys.N:
                    Config.BalancedMode = !Config.BalancedMode;
                    Game1.showGlobalMessage("Balanced Mode: " + Config.BalancedMode);
                    break;
            }
        }
    }
}