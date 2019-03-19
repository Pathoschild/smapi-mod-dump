using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace NoOre {

    public class NoOre : Mod {
        public override void Entry(IModHelper helper) {
            helper.Events.World.ObjectListChanged += WorldOnObjectListChanged; // object list changed
            helper.Events.Player.Warped += PlayerOnWarped;
            helper.ConsoleCommands.Add("object", "get object near cursor", ReportObject);
            KeyValuePair<int, string> hi = new KeyValuePair<int, string>(1, "");
        }


        private void ReportObject(string arg1, string[] arg2) {
            var screenX = Game1.getMouseX();
            var screenY = Game1.getMouseY();
            var tile = new Vector2(Game1.player.getTileX(), Game1.player.getTileY());
            foreach (var pos in Utility.getSurroundingTileLocationsArray(tile)) {
                var obj = Game1.currentLocation.getObjectAtTile((int) pos.X, (int) pos.Y);
                if (obj != null) {
                    Monitor.Log($"found object {obj.Name} {obj.Type}");
                }
            }
        }


        private void PlayerOnWarped(object sender, WarpedEventArgs e) {
            Monitor.Log($"Warped to {e.NewLocation.Name}");
        }


        private void WorldOnObjectListChanged(object sender, ObjectListChangedEventArgs e) {
            if (e.IsCurrentLocation && e.Removed.Any()) {
                foreach (var keyValuePair in e.Removed) {
                    var pos = keyValuePair.Key;
                    var obj = keyValuePair.Value;

                    // Game1.createRadialDebris();
                    // Game1.createMultipleObjectDebris();
                    // e.Location.debris.Add(new Debris(Item));
                }
            }
        }
    }

}