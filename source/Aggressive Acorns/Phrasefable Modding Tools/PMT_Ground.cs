using System.Text;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace Phrasefable_Modding_Tools {

    public partial class PhrasefableModdingTools {
        private void SetUp_Ground() {
            var desc = new StringBuilder("Prints data on the tiles around the player.");
            desc.AppendLine("Usage: ground [radius]");
            Helper.ConsoleCommands.Add("ground", desc.ToString(), GroundCommand);
        }


        private void GroundCommand(string command, string[] args) {
            if (!Context.IsWorldReady) {
                Monitor.Log("World not ready.", LogLevel.Info);
                return;
            }

            var radius = 1;
            if (args.Length > 0 && int.TryParse(args[0], out var value)) {
                radius = value;
            }

            var loc = Game1.player.Position;
            var playerX = loc.X / Game1.tileSize;
            var playerY = loc.Y / Game1.tileSize;
            var playerPosition = new Vector2((int) playerX, (int) playerY);

            for (var dy = -radius; dy <= radius; dy++) {
                for (var dx = -radius; dx <= radius; dx++) {
                    var tile = playerPosition + new Vector2(dx, dy);
                    CheckGround(Game1.currentLocation, tile);
                }
            }
        }


        private void CheckGround([NotNull] GameLocation location, Vector2 tile) {
            var message = new StringBuilder($"{location.Name} {tile}:");
            var any = false;

            if (location.Objects.TryGetValue(tile, out var sObject)) {
                message.Append($" Object {sObject.Name} id {sObject.ParentSheetIndex}");
                if (sObject.IsSpawnedObject) message.Append(" [Spawned]");
                if (sObject.CanBeGrabbed) message.Append(" [CanBeGrabbed]");
                if (sObject.bigCraftable.Value) message.Append(" [BigCraftable]");
                message.Append(";");
                any = true;
            }

            if (location.terrainFeatures.TryGetValue(tile, out var feature)) {
                message.Append($" Terrain feature {feature.GetType().FullName}");
                any = true;
            }

            if (!any) {
                message.Append(" (none)");
            }

            Monitor.Log(message.ToString(), LogLevel.Info);

        }
    }

}
