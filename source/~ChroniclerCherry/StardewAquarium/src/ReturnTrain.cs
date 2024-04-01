/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using StardewAquarium.Models;
using StardewModdingAPI;
using StardewValley;

namespace StardewAquarium
{
    class ReturnTrain
    {
        private IModHelper _helper;
        private IMonitor _monitor;
        private ITrainStationAPI TSApi;
        public ReturnTrain(IModHelper helper, IMonitor monitor)
        {
            this._helper = helper;
            this._monitor = monitor;

            this._helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            this.TSApi = this._helper.ModRegistry.GetApi<ITrainStationAPI>("Cherry.TrainStation");
            if (this.TSApi == null)
            {
                this._monitor.Log("The train station API was not found. Warps back to the Railroad will default to a map warp.", LogLevel.Warn);
                return;
            }

            this._helper.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
        }

        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.currentLocation?.Name != ModEntry.Data.ExteriorMapName)
                return;

            if (Game1.player.Position.Y > 32)
                return;

            Game1.player.position.Y += 32;
            this.TSApi.OpenTrainMenu();
        }
    }
}
