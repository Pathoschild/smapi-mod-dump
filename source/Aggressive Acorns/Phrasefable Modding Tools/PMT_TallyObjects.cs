using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Phrasefable_Modding_Tools {

    public partial class PhrasefableModdingTools {

        private bool _doTallyObjects;


        private void SetUp_Tally() {
            Helper.Events.Player.Warped += Tally;

            var desc = "Counts the different types of objects in the current GameLocation";
            Helper.ConsoleCommands.Add("obj-count", desc, CountObjects);

            desc = "Counts the types of objects in each new GameLocation";
            Helper.ConsoleCommands.Add("obj-count-start", desc, (s, strings) => _doTallyObjects = true);

            desc = "Stops counting all the objects each time you move to a new GameLocation";
            Helper.ConsoleCommands.Add("obj-count-stop", desc, (s, strings) => _doTallyObjects = false);

            desc = "Tallies the objects by type for each loaded GameLocation";
            Helper.ConsoleCommands.Add("obj-count-all", desc, CountAllObjects);

            Helper.ConsoleCommands.Add("count-terrain", "counts terrain features", CountTerrainFeatures);
        }


        private void CountTerrainFeatures(string arg1, string[] arg2) {
            if (Context.IsWorldReady) {
                CountTerrainFeatures(Game1.currentLocation);
            } else {
                Monitor.Log("World not ready", LogLevel.Info);
            }
        }


        private void CountObjects(string s, string[] strings) {
            if (Context.IsWorldReady) {
                CountObjects(Game1.currentLocation);
            } else {
                Monitor.Log("World not ready", LogLevel.Info);
            }
        }


        private void CountAllObjects(string s, string[] strings) {
            if (Context.IsWorldReady) {
                foreach (var location in Game1.locations) CountObjects(location);
            } else {
                Monitor.Log("World not ready", LogLevel.Info);
            }
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
                Monitor.Log($"    {first.ParentSheetIndex} {first.DisplayName} - {objects.Count}", LogLevel.Info);
            }
        }


        private void CountTerrainFeatures([NotNull] GameLocation location) {
            var results = from feat in location.terrainFeatures.Values
                          group feat by feat.GetType()
                          into grouping
                          orderby grouping.Key.Name
                          select new {grouping.Key.Name, Count = grouping.Count()};

            Monitor.Log($"Counted terrain features in {location.Name}", LogLevel.Info);
            foreach (var result in results) {
                Monitor.Log($"    {result.Name} - {result.Count}");
            }
        }


        // todo make some sort of command that will rapidly warp through all mine floors.
        // todo add name filter?, make commands better.
    }

}