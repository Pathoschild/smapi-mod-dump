/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using StardewModdingAPI;
using System.Linq;

namespace MasterFisher
{
    /// <summary>Handles console commands.</summary>
    public static class CommandManager
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Logs the current fish data to the console.</summary>
        public static void LogSummary()
        {
            ModEntry.Instance.Monitor.Log("Categories:", LogLevel.Info);
            foreach (var category in ModEntry.Instance.Categories)
            {
                ModEntry.Instance.Monitor.Log($"    Name:           | {category.Name}", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    FishPondCap:    | {category.FishPondCap}", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    MinigameSprite: | {category.MinigameSprite}\n", LogLevel.Info);
            }

            ModEntry.Instance.Monitor.Log("LocationAreas:", LogLevel.Info);
            foreach (var locationArea in ModEntry.Instance.LocationAreas)
            {
                ModEntry.Instance.Monitor.Log($"    UniqueName:     | {locationArea.UniqueName}", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    LocationName:   | {locationArea.LocationName}", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    Area:           | {locationArea.Area}", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    SpringFish:     | [{string.Join(", ", locationArea.SpringFish.Select(fish => $"\"{fish}\""))}]", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    SummerFish:     | [{string.Join(", ", locationArea.SummerFish.Select(fish => $"\"{fish}\""))}]", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    FallFish:       | [{string.Join(", ", locationArea.FallFish.Select(fish => $"\"{fish}\""))}]", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    WinterFish:     | [{string.Join(", ", locationArea.WinterFish.Select(fish => $"\"{fish}\""))}]", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    TreasureChance: | {locationArea.TreasureChance}\n", LogLevel.Info);
            }

            ModEntry.Instance.Monitor.Log("Bait:", LogLevel.Info);
            foreach (var bait in ModEntry.Instance.Bait)
            {
                ModEntry.Instance.Monitor.Log($"    ObjectId:            | {bait.ObjectId}", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    ResolvedObjectId:    | {bait.ResolvedObjectId}", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    ContextTagsAffected: | [{string.Join(", ", bait.ContextTagsAffected.Select(contextTag => $"\"{contextTag}\""))}]", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    FishAffected:        | [{string.Join(", ", bait.FishAffected.Select(fish => $"\"{fish}\""))}]", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"    BiteRate:            | {bait.BiteRate}", LogLevel.Info);
            }
        }
    }
}
