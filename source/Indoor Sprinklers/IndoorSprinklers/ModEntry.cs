/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mpomirski/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

namespace IndoorSprinklers
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
         ** Public methods
         *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }


        /*********
         ** Private methods
         *********/
        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            this.RunSprinklers();
            this.Monitor.Log("Ran sprinklers");
        }
        
        /// <summary>Returns all indoor locations, with the farm buildings interiors.</summary>
        private static IEnumerable<GameLocation> GetIndoorLocations()
        {
            IEnumerable<GameLocation> indoorLocations = Game1.locations.Where(location => !location.IsOutdoors);

            IEnumerable<GameLocation> farmIndoorLocations = Game1.getFarm().buildings
                .Where(building => building.HasIndoors())
                .Select(building => building.GetIndoors());

            return indoorLocations.Concat(farmIndoorLocations);
        }

        
        /// <summary>Runs sprinklers on all indoor pots in range.</summary>
        private void RunSprinklers()
        {
            foreach (var location in GetIndoorLocations())
            {
                foreach (var sprinkler in location.objects.Values.Where(o => o.IsSprinkler()))
                {
                    foreach (var gameObject in sprinkler.GetSprinklerTiles().SelectMany(sprinklerCoveredTile => location.objects.Pairs.Where(
                                 gameObject => gameObject.Key.Equals(sprinklerCoveredTile))))
                    {
                        this.Monitor.Log($"Running sprinkler on {gameObject.Key}");
                        if (gameObject.Value is not IndoorPot pot) continue;
                        pot.hoeDirt.Value.state.Value = 1;
                        pot.showNextIndex.Value = true;
                        this.Monitor.Log("Watered an indoor plant");
                    }
                }
            }
        }
    }
}