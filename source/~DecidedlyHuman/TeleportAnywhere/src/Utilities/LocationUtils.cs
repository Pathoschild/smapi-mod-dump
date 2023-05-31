/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;

namespace TeleportAnywhere.Utilities
{
    public class LocationUtils
    {
        private IModHelper helper;
        private readonly IMonitor monitor;

        public LocationUtils(IMonitor monitor, IModHelper helper)
        {
            this.monitor = monitor;
            this.helper = helper;
        }

        public void ResetToStartingLocation(GameLocation location, Vector2 tile)
        {
            // if (Game1.currentLocation.Equals(location))
            // {
            //     Game1.player.position.Value = tile;
            //     Game1.globalFadeToClear();
            //     return;
            // }

            // if (Game)
            // {
            //     Game1.showRedMessage("Cannot teleport to temporary locations.");
            //
            //     return;
            // }

            var requestedLocation = Game1.getLocationRequest(location.Name, location.isStructure);
            // LocationRequest requestedLocation = new LocationRequest(location.Name, location.isStructure, location);
            Game1.locationRequest = requestedLocation;
            Game1.currentLocation.cleanupBeforePlayerExit();

            if (!location.isStructure)
                Game1.warpFarmer(requestedLocation, (int)tile.X, (int)tile.Y, 2);
            else
                Game1.warpFarmer(location.NameOrUniqueName, (int)tile.X, (int)tile.Y, false);

            Game1.player.viewingLocation.Value = null;
            // Game1.globalFadeToClear();
            Game1.displayHUD = true;
            Game1.displayFarmer = true;
            Game1.player.canMove = true;

            Game1.activeClickableMenu = null;
        }

        public void ObserveLocation(GameLocation l)
        {
            if (l == null)
            {
                this.monitor.Log("Location was null.", LogLevel.Error);

                return;
            }

            this.monitor.Log($"Trying to observe {l.Name}", LogLevel.Info);

            Game1.locationRequest = new LocationRequest(l.Name, l.isStructure, l);
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = l;
            Game1.player.viewingLocation.Value = l.Name;
            Game1.currentLocation.resetForPlayerEntry();
            // Game1.globalFadeToClear();
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;

            int viewportWidth = Game1.viewport.Width;
            int viewportHeight = Game1.viewport.Height;

            // Game1.viewport.Location = new Location(0, 0);
            this.monitor.Log($"Game1.viewport before adjustment: {Game1.viewport}", LogLevel.Info);
            this.monitor.Log($"Map width: {l.map.DisplayWidth}", LogLevel.Info);
            this.monitor.Log($"Map height: {l.map.DisplayHeight}", LogLevel.Info);

            // Game1.viewport.X = l.map.DisplayWidth - Game1.viewport.Width / 2;
            // Game1.viewport.Y = l.map.DisplayHeight - Game1.viewport.Height / 2;

            Game1.viewport.Location = new Location(l.map.DisplayWidth / 2 - viewportWidth / 2,
                l.map.DisplayHeight / 2 - viewportHeight / 2);

            // Game1.viewport = new Rectangle(l.map.DisplayWidth / 2, l.map.DisplayHeight / 2, Game1.viewport.Width, Game1.viewport.Height);
            // Game1.panScreen(l.map.DisplayWidth / 2, l.map.DisplayHeight / 2);
            // Game1.currentViewportTarget = new Vector2(l.map.DisplayWidth / 2, l.map.DisplayHeight / 2);

            this.monitor.Log($"Game1.viewport.Location after adjustment: {Game1.viewport.Location}", LogLevel.Info);
            // Game1.panScreen(centreX / 64, centreY / 64);

            Game1.displayFarmer = false;

            this.monitor.Log($"Is Game1.locationRequest null at end of switching?: {Game1.locationRequest == null}",
                LogLevel.Info);
        }
    }
}
