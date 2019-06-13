using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Phrasefable_Modding_Tools {

    [UsedImplicitly]
    public class PhrasefableModdingTools : Mod {

        private bool _doTallyObjects;


        public override void Entry([NotNull] IModHelper helper) {

            var desc = "prints data on the tiles around the mouse cursor";
            helper.ConsoleCommands.Add("ground", desc, GroundCommand);

            SetUpTallyStuff();

            RegisterEventLogging();

        }


        private void RegisterEventLogging() {
            Helper.Events.World.DebrisListChanged +=
                    (sender, args) => Monitor.Log("WorldOnDebrisListChanged", LogLevel.Trace);
            Helper.Events.World.ObjectListChanged +=
                    (sender, args) => Monitor.Log("WorldOnObjectListChanged", LogLevel.Trace);
            Helper.Events.World.LocationListChanged +=
                    (sender, args) => Monitor.Log("WorldOnLocationListChanged", LogLevel.Trace);
            Helper.Events.World.TerrainFeatureListChanged +=
                    (sender, args) => Monitor.Log("WorldOnTerrainFeatureListChanged", LogLevel.Trace);
            Helper.Events.World.LargeTerrainFeatureListChanged +=
                    (sender, args) => Monitor.Log("WorldOnLargeTerrainFeatureListChanged", LogLevel.Trace);


            Helper.Events.GameLoop.Saving += (sender, args) => Monitor.Log("GameLoopOnSaving", LogLevel.Trace);
            Helper.Events.GameLoop.Saved += (sender, args) => Monitor.Log("GameLoopOnSaved", LogLevel.Trace);
            Helper.Events.GameLoop.SaveLoaded += (sender, args) => Monitor.Log("GameLoopOnSaveLoaded", LogLevel.Trace);
            Helper.Events.GameLoop.DayStarted += (sender, args) => Monitor.Log("GameLoopOnDayStarted", LogLevel.Trace);
            Helper.Events.GameLoop.DayEnding += (sender, args) => Monitor.Log("GameLoopOnDayEnding", LogLevel.Trace);
        }


        private void SetUpTallyStuff() {
            Helper.Events.Player.Warped += Tally;

            var desc = "Counts the different types of objects in the current GameLocation";
            Helper.ConsoleCommands.Add("obj-count", desc, (s, strings) => CountObjects(Game1.currentLocation));

            desc = "Counts the types of objects in each new GameLocation";
            Helper.ConsoleCommands.Add("obj-count-start", desc, (s, strings) => _doTallyObjects = true);

            desc = "Stops counting all the objects each time you move to a new GameLocation";
            Helper.ConsoleCommands.Add("obj-count-stop", desc, (s, strings) => _doTallyObjects = false);

            desc = "Tallies the objects by type for each loaded GameLocation";
            Helper.ConsoleCommands.Add("obj-count-all", desc, (s, strings) => {
                foreach (var location in Game1.locations) CountObjects(location);
            });
        }


        private void Tally(object sender, [NotNull] WarpedEventArgs e) {
            if (_doTallyObjects && e.IsLocalPlayer) CountObjects(e.NewLocation);
        }


        private void CountObjects([NotNull] GameLocation location) {

            var results = from obj in location.objects.Values
                          group obj by obj.ParentSheetIndex
                          into grouping
                          orderby grouping.Key
                          select grouping.ToList();

            Monitor.Log($"Counted objects in {location.Name}:", LogLevel.Info);
            foreach (var objects in results) {
                var first = objects.First();
                Monitor.Log($"    {first.ParentSheetIndex} {first.DisplayName} - {objects.Count}");
            }
        }


        private void GroundCommand(string arg1, string[] arg2) {
            var cursor = Helper.Input.GetCursorPosition().Tile;
            var radius = 1;

            for (var x = -radius; x > radius; x++) {
                for (var y = -radius; y > radius; y++) {
                    var tile = cursor + new Vector2(x, y);
                    CheckGround(Game1.currentLocation, tile);
                }
            }
        }


        private void CheckGround([NotNull] GameLocation location, Vector2 tile) {
            var message = new StringBuilder($"{location.Name} {tile}:");

            if (location.Objects.TryGetValue(tile, out var sObject)) {
                message.AppendLine($"    Object {sObject.Name} of type {sObject.Type} id {sObject.ParentSheetIndex}");
                if (sObject.IsSpawnedObject) message.Append(" [Spawned]");
                if (sObject.CanBeGrabbed) message.Append(" [CanBeGrabbed]");
                if (sObject.bigCraftable.Value) message.Append(" [BigCraftable]");
                message.Append(";");
            }

            if (location.terrainFeatures.TryGetValue(tile, out var feature)) {
                message.AppendLine($"    Terrain feature {feature.GetType().FullName}");
            }

            Monitor.Log(message.ToString(), LogLevel.Info);

        }
    }

}