/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System.Linq;
using PyromancersJourney.Framework;
using SpaceCore.Events;
using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using xTile.Dimensions;

namespace PyromancersJourney
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod Instance;

        public override void Entry(IModHelper helper)
        {
            Mod.Instance = this;
            Log.Monitor = this.Monitor;

            helper.Events.Player.Warped += this.OnWarped;

            SpaceEvents.ActionActivated += this.OnActionActivated;

            helper.ConsoleCommands.Add("pyrojourney", "Start the minigame!", this.DoCommands);
        }

        private void OnActionActivated(object sender, EventArgsAction e)
        {
            if (e.Action == "FireArcadeGame")
                Game1.currentMinigame = new PyromancerMinigame();
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation is VolcanoDungeon vd && vd.level.Value == 5)
            {
                var ts = vd.Map.TileSheets.FirstOrDefault(t => t.ImageSource.Contains("arcade-machine"));
                if (ts == null)
                {
                    ts = new xTile.Tiles.TileSheet(vd.Map, this.Helper.Content.GetActualAssetKey("assets/arcade-machine.png"), new Size(2, 2), new Size(16, 16));
                    ts.Id = "z" + ts.Id;
                    vd.Map.AddTileSheet(ts);
                    vd.setMapTile(31, 28, 3, "Buildings", "FireArcadeGame", vd.Map.TileSheets.IndexOf(ts));
                    vd.setMapTileIndex(31, 27, 1, "Front", vd.Map.TileSheets.IndexOf(ts));
                    Game1.mapDisplayDevice.LoadTileSheet(ts);
                }
            }
        }

        private void DoCommands(string cmd, string[] args)
        {
            if (cmd == "pyrojourney")
            {
                if (!Context.IsPlayerFree)
                    Log.Info("You must have a save loaded and be not busy.");
                else
                    Game1.currentMinigame = new PyromancerMinigame();
            }
        }
    }
}
