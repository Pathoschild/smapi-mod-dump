using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Phrasefable_Modding_Tools
{
    public partial class PhrasefableModdingTools
    {
        private ToggleableEventHandler<WarpedEventArgs> _tallyHandler;


        private void SetUp_Tally()
        {
            _tallyHandler = new ToggleableEventHandler<WarpedEventArgs>(Tally);
            Helper.Events.Player.Warped += _tallyHandler.OnEvent;

            var desc = new StringBuilder("Counts the objects in the current location.");
            desc.AppendLine("Usage: count-objects [all|start|stop]");
            desc.AppendLine("    all   - count the objects in every location");
            desc.AppendLine("    start - start counting each time a location is entered");
            desc.Append("    stop  - stop counting each time a location is entered");
            Helper.ConsoleCommands.Add("count-objects", desc.ToString(), TallyObjectCommand);
            Helper.ConsoleCommands.Add("count-terrain", "counts terrain features", CountTerrainFeatures);
        }


        private void Tally(object sender, [NotNull] WarpedEventArgs e)
        {
            if (e.IsLocalPlayer) CountObjects(e.NewLocation);
        }


        private void TallyObjectCommand(string command, [NotNull] string[] args)
        {
            if (args.Length == 0)
            {
                CountObjects();
            }
            else
            {
                switch (args[0])
                {
                    case "start":
                        _tallyHandler.Set(ToggleAction.Enable);
                        break;
                    case "stop":
                        _tallyHandler.Set(ToggleAction.Disable);
                        break;
                    case "all":
                        CountObjects(true);
                        break;
                    default:
                        Monitor.Log($"Arguments `{string.Join(" ", args)}` malformed.", LogLevel.Info);
                        break;
                }
            }
        }


        private void CountTerrainFeatures(string arg1, string[] arg2)
        {
            if (Context.IsWorldReady)
            {
                CountTerrainFeatures(Game1.currentLocation);
            }
            else
            {
                Monitor.Log("World not ready", LogLevel.Info);
            }
        }


        private void CountObjects(bool allLocations = false)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("World not ready", LogLevel.Info);
                return;
            }

            if (allLocations)
            {
                foreach (GameLocation location in Common.Utilities.GetLocations(Helper)) CountObjects(location);
            }
            else
            {
                CountObjects(Game1.currentLocation);
            }
        }


        private void CountObjects([NotNull] GameLocation location)
        {
            IEnumerable<List<Object>> results = from obj in location.objects.Values
                group obj by obj.ParentSheetIndex
                into grouping
                orderby grouping.Key
                select grouping.ToList();

            Monitor.Log($"Counted objects in {location.Name}:", LogLevel.Info);
            foreach (List<Object> objects in results)
            {
                Object first = objects.First();
                Monitor.Log($"    {first.ParentSheetIndex} {first.DisplayName} - {objects.Count}", LogLevel.Info);
            }
        }


        private void CountTerrainFeatures([NotNull] GameLocation location)
        {
            var results = from feat in location.terrainFeatures.Values
                group feat by feat.GetType()
                into grouping
                orderby grouping.Key.Name
                select new {grouping.Key.Name, Count = grouping.Count()};

            Monitor.Log($"Counted terrain features in {location.Name}", LogLevel.Info);
            foreach (var result in results)
            {
                Monitor.Log($"    {result.Name} - {result.Count}");
            }
        }


        // todo make some sort of command that will rapidly warp through all mine floors.
        // todo add name filter?
    }
}
