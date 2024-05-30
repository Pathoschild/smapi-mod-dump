/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;


namespace NermNermNerm.Junimatic
{
    public class WorkFinder : ISimpleLog
    {
        private ModEntry mod = null!;
        private readonly Dictionary<GameLocation, IReadOnlyList<MachineNetwork>> cachedNetworks = new();

        private int timeOfDayAtLastCheck = -1;
        private int numActionsAtThisGameTime;

        private bool isDayStarted = false;

        /// <summary>The number of Junimos that are being simulated out doing stuff.</summary>
        private readonly Dictionary<JunimoType, int> numAutomatedJunimos = Enum.GetValues<JunimoType>().ToDictionary(t => t, t => 0);

        private static readonly Point[] walkableDirections = [new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1)];
        private static readonly Point[] crabPotReachableDirections = [new Point(-2, 0), new Point(2, 0), new Point(0, -2), new Point(0, 2)];
        private static readonly Point[] reachableDirections = [
            new Point(-1, -1), new Point(0, -1), new Point(1, -1),
            new Point(-1, 0), /*new Point(0, 0),*/ new Point(1, 0),
            new Point(-1, 1), new Point(0, 1), new Point(1, 1)];

        public void Entry(ModEntry mod)
        {
            this.mod = mod;
            mod.Helper.Events.GameLoop.OneSecondUpdateTicked += this.GameLoop_OneSecondUpdateTicked;
            mod.Helper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
            mod.Helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
        }

        private void GameLoop_DayStarted(object? sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            this.LogTrace("WorkFinder.OnDayStarted unleashed the junimos");
            this.isDayStarted = true;
        }

        private void GameLoop_DayEnding(object? sender, StardewModdingAPI.Events.DayEndingEventArgs e)
        {
            this.isDayStarted = false;
            if (!Game1.IsMasterGame)
            {
                this.LogTrace("WorkFinder.OnDayEnding - not doing anything because this is not the master game.");
                return;
            }

            foreach (var location in Game1.locations)
            {
                foreach (var junimo in location.characters.OfType<JunimoShuffler>().ToArray())
                {
                    junimo.OnDayEnding(location);
                }
            }
            this.LogTrace("WorkFinder.OnDayEnding - not doing anything because this is not the master game.");
        }

        // 10 minutes in SDV takes 7.17 seconds of real time.  So our setting of 3 means
        //  that we assume that junimo actions take about 2 seconds to do.
        private const int numActionsPerTenMines = 3;

        private void GameLoop_OneSecondUpdateTicked(object? sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                this.cachedNetworks.Clear();
                return;
            }

            if (Game1.isTimePaused || !Game1.IsMasterGame)
            {
                return;
            }

            if (!this.isDayStarted)
            {
                this.LogTrace("Canceling OnSecondUpdateTicked processing because the day hasn't started yet.");
                return;
            }

            // timeOfDay is the time as shown by the game's clock.  740 means 7:40AM. 1920 means 7:20PM.
            //   I don't see how to get a more granular time than that.  The ticks given in the event args
            //   proceed whether the game is moving along or not.
            // currentTime is simply how much playtime has happened - perhaps it just synchronizes games?
            //
            if (this.timeOfDayAtLastCheck != Game1.timeOfDay)
            {
                this.timeOfDayAtLastCheck = Game1.timeOfDay;
                this.numActionsAtThisGameTime = 0;
            }
            else
            {
                ++this.numActionsAtThisGameTime;
            }

            if (this.numActionsAtThisGameTime >= numActionsPerTenMines)
            {
                return;
            }

            this.DoJunimos(true);
        }

        private void DoJunimos(bool isAutomationInterval)
        {
            var numAvailableJunimos = this.GetNumUnlockedJunimos();
            foreach (var junimoType in Enum.GetValues<JunimoType>())
            {
                if (isAutomationInterval)
                {
                    this.numAutomatedJunimos[junimoType] = 0;
                }
            }

            // Junimos only work on the farm or in farm buildings.
            var allJunimoFriendlyLocations =
                Game1.getFarm().buildings
                    .Select(b => b.indoors.Value)
                    .Where(l => l is not null).Select(l => l!)
                    .ToList();
            allJunimoFriendlyLocations.Add(Game1.getFarm());
            allJunimoFriendlyLocations.AddRange(
                new string[] { "FarmCave", "IslandWest", "Cellar", "FarmHouse", "IslandFarmHouse", "Greenhouse" }
                .Select(name => Game1.getLocationFromName(name))
                .Where(l => l is not null));

            foreach (var location in allJunimoFriendlyLocations)
            {
                foreach (var animatedJunimo in location.characters.OfType<JunimoShuffler>())
                {
                    if (animatedJunimo.Assignment is null)
                    {
                        continue; // Should not happen - Assignment is only null when on a non-master multiplayer game, and we know we're on the master game.
                    }

                    if (numAvailableJunimos[animatedJunimo.Assignment.projectType] > 0) // Should always be true
                    {
                        numAvailableJunimos[animatedJunimo.Assignment.projectType] -= 1;
                    }
                }
            }

            // Try to employ junimos in visible locations first:
            HashSet<GameLocation> animatedLocations = new HashSet<GameLocation>(Game1.getOnlineFarmers().Select(f => f.currentLocation).Where(l => l is not null && allJunimoFriendlyLocations.Contains(l)));
            foreach (GameLocation location in animatedLocations)
            {
                this.cachedNetworks.Remove(location);
                foreach (var portal in new GameMap(location).GetPortals())
                {
                    bool junimoCreated = false;
                    foreach (var junimoType in Enum.GetValues<JunimoType>())
                    {
                        if (numAvailableJunimos[junimoType] > 0)
                        {
                            var project = this.FindProject(portal, junimoType, null);
                            if (project is not null)
                            {
                                this.LogTrace($"Starting Animated Junimo for {project}");
                                location.characters.Add(new JunimoShuffler(project, this));
                                junimoCreated = true; // Only create one animated junimo per portal per second
                            }
                        }
                        if (junimoCreated) break;
                    }
                    if (junimoCreated) break;
                }
            }

            if (isAutomationInterval)
            {
                foreach (GameLocation location in allJunimoFriendlyLocations.Where(l => !animatedLocations.Contains(l)))
                {
                    foreach (var junimoType in Enum.GetValues<JunimoType>())
                    {
                        if (numAvailableJunimos[junimoType] > 0)
                        {
                            if (this.TryDoAutomationsForLocation(location, junimoType))
                            {
                                numAvailableJunimos[junimoType] -= 1;
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<JunimoType, int> GetNumUnlockedJunimos()
        {
            var result = new Dictionary<JunimoType, int>
            {
                { JunimoType.CropProcessing, this.mod.CropMachineHelperQuest.IsUnlocked ? 1 : 0 },
                { JunimoType.MiningProcessing, this.mod.UnlockMiner.IsUnlocked ? 1 : 0 },
                { JunimoType.Animals, this.mod.UnlockAnimal.IsUnlocked ? 1 : 0 },
                { JunimoType.Fishing, this.mod.UnlockFishing.IsUnlocked ? 1 : 0 },
                { JunimoType.Forestry, this.mod.UnlockForest.IsUnlocked ? 1 : 0 }
            };
            return result;
        }

        private bool TryDoAutomationsForLocation(GameLocation location, JunimoType projectType)
        {
            if (!this.cachedNetworks.TryGetValue(location, out var networks))
            {
                networks = this.BuildNetwork(location);
                this.cachedNetworks.Add(location, networks);
            }

            foreach (var network in networks)
            {
                var machines = network.Machines[projectType];

                // Try and load a machine
                foreach (var emptyMachine in machines.Where(m => m.IsIdle && m.IsCompatibleWithJunimo(projectType)))
                {
                    foreach (var chest in network.Chests)
                    {
                        if (emptyMachine.FillMachineFromChest(chest))
                        {
                            this.LogTrace($"Automatic machine fill of {emptyMachine} on {location.Name} from {chest}");
                            return true;
                        }
                    }
                }

                // Try and empty a machine
                foreach (var fullMachine in machines)
                {
                    if (fullMachine.HeldObject is not null && fullMachine.IsCompatibleWithJunimo(projectType))
                    {
                        var goodChest = network.Chests.FirstOrDefault(c => c.IsPreferredStorageForMachinesOutput(fullMachine.HeldObject));
                        if (goodChest is null)
                        {
                            goodChest = network.Chests.FirstOrDefault(c => c.IsPossibleStorageForMachinesOutput(fullMachine.HeldObject));
                        }

                        if (goodChest is not null)
                        {
                            string wasHolding = fullMachine.HeldObject.Name;
                            if (fullMachine.TryPutHeldObjectInStorage(goodChest))
                            {
                                this.LogTrace($"Automatic machine empty of {fullMachine} holding {wasHolding} on {location.Name} into {goodChest}");
                            }
                            else
                            {
                                this.LogError($"FAILED: Automatic machine empty of {fullMachine} holding {fullMachine.HeldObject.Name} on {location.Name} into {goodChest}");
                            }

                            return true;
                        }
                    }
                }
            }
            return false;
        }

        record class MachineNetwork(
            IReadOnlyDictionary<JunimoType,IReadOnlyList<GameMachine>> Machines,
            IReadOnlyList<GameStorage> Chests);

        private List<MachineNetwork> BuildNetwork(GameLocation location)
        {
            var watch = Stopwatch.StartNew();

            var map = new GameMap(location);
            var result = new List<MachineNetwork>();
            var portals = map.GetPortals();
            foreach (var portal in portals)
            {
                var machines = new Dictionary<JunimoType, List<GameMachine>>(Enum.GetValues<JunimoType>().Select(e => new KeyValuePair<JunimoType, List<GameMachine>>(e, new List<GameMachine>())));
                var chests = new List<GameStorage>();
                var checkedForWorkTiles = new HashSet<Point>();
                var walkedTiles = new HashSet<Point>();

                map.GetStartingInfo(portal, out var startingPoints, out var walkableFloorTypes);

                foreach (var startingTile in startingPoints)
                {
                    var tilesToInvestigate = new Queue<Point>();
                    tilesToInvestigate.Enqueue(startingTile);

                    while (tilesToInvestigate.TryDequeue(out var reachableTile))
                    {
                        if (walkedTiles.Contains(reachableTile))
                        {
                            continue;
                        }

                        foreach (var direction in reachableDirections)
                        {
                            var adjacentTile = reachableTile + direction;
                            if (checkedForWorkTiles.Contains(adjacentTile))
                            {
                                continue;
                            }

                            map.GetThingAt(adjacentTile, reachableTile, walkableFloorTypes, out bool isWalkable, out var machine, out var storage);
                            if (storage is not null)
                            {
                                chests.Add(storage);
                                checkedForWorkTiles.Add(adjacentTile);
                            }
                            else if (machine is not null)
                            {
                                foreach (JunimoType junimoType in Enum.GetValues<JunimoType>())
                                {
                                    if (machine.IsCompatibleWithJunimo(junimoType))
                                    {
                                        machines[junimoType].Add(machine);
                                    }
                                }
                                checkedForWorkTiles.Add(adjacentTile);
                            }
                            else if (isWalkable && (direction.X == 0 || direction.Y == 0)) // && is not on a diagonal
                            {
                                tilesToInvestigate.Enqueue(adjacentTile);
                            }
                            else if (!isWalkable)
                            {
                                checkedForWorkTiles.Add(adjacentTile);
                            }
                        }

                        checkedForWorkTiles.Add(reachableTile);
                        walkedTiles.Add(reachableTile);
                    }

                    result.Add(new MachineNetwork(machines.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<GameMachine>)pair.Value), chests));
                }
            }

            long elapsedMs = watch.ElapsedMilliseconds;
            if (elapsedMs > 0)
            {
                this.LogInfo($"WorkFinder.BuildNetwork for {location.Name} took {watch.ElapsedMilliseconds}ms");
            }
            return result;
        }

        public JunimoAssignment? FindProject(StardewValley.Object portal, JunimoType projectType, JunimoShuffler? forJunimo)
        {
            // This duplicates the logic in BuildNetwork, except that it's trying to find the closest path and
            // it stops as soon as it cooks up something to do.

            var location = portal.Location;
            // These lists are all in order of nearest to farthest from the portal
            var emptyMachines = new List<GameMachine>();
            var fullMachines = new List<GameMachine>();
            var knownChests = new List<GameStorage>();
            var visitedTiles = new HashSet<Point>();

            var map = new GameMap(location);

            map.GetStartingInfo(portal, out var startingPoints, out var walkableFloorTypes);
            if (forJunimo is not null)
            {
                startingPoints = new List<Point>() { forJunimo.Tile.ToPoint() };
            }

            HashSet<object> busyMachines = portal.Location.characters
                .OfType<JunimoShuffler>()
                .Where(j => j != forJunimo)
                .Select(junimo =>
                    junimo.Assignment?.source is GameMachine machine
                    ? machine.GameObject
                    : (junimo.Assignment?.target is GameMachine targetMachine
                        ? targetMachine.GameObject
                        : null))
                .Where(machine => machine is not null)
                .Select(machine => machine!)
                .ToHashSet();

            foreach (var startingTile in startingPoints)
            {
                var originTile = forJunimo?.Assignment?.origin ?? startingTile;
                var tilesToInvestigate = new Queue<Point>();
                tilesToInvestigate.Enqueue(startingTile);

                while (tilesToInvestigate.TryDequeue(out var tile))
                {
                    if (visitedTiles.Contains(tile))
                    {
                        continue;
                    }

                    foreach (var direction in walkableDirections)
                    {
                        var adjacentTile = tile + direction;
                        if (visitedTiles.Contains(adjacentTile))
                        {
                            continue;
                        }

                        map.GetThingAt(adjacentTile, tile, walkableFloorTypes, out bool isWalkable, out var machine, out var chest);

                        if (chest is not null)
                        {
                            // See if we can create a mission to carry from a full machine to this chest
                            foreach (var machineNeedingPickup in fullMachines)
                            {
                                if (chest.IsPreferredStorageForMachinesOutput(machineNeedingPickup.HeldObject!))
                                {
                                    return new JunimoAssignment(projectType, location, portal, originTile, machineNeedingPickup, chest, itemsToRemoveFromChest: null);
                                }
                            }

                            // See if we can create a mission to carry from this chest to an idle machine
                            foreach (var machineNeedingDelivery in emptyMachines)
                            {
                                var inputs = machineNeedingDelivery.GetRecipeFromChest(chest);
                                if (inputs is not null)
                                {
                                    return new JunimoAssignment(projectType, location, portal, originTile, chest, machineNeedingDelivery, inputs);
                                }
                            }

                            knownChests.Add(chest);
                            visitedTiles.Add(adjacentTile);
                        }
                        else if (machine is not null && machine.IsCompatibleWithJunimo(projectType) && !busyMachines.Contains(machine.GameObject))
                        {
                            if (machine.HeldObject is not null)
                            {
                                // Try and find a chest to tote it to
                                var targetChest = knownChests.FirstOrDefault(chest => chest.IsPreferredStorageForMachinesOutput(machine.HeldObject));
                                if (targetChest is not null)
                                {
                                    return new JunimoAssignment(projectType, location, portal, originTile, machine, targetChest, itemsToRemoveFromChest: null);
                                }

                                fullMachines.Add(machine);
                            }
                            else if (machine.IsIdle)
                            {
                                // Try and find a chest to supply it from
                                foreach (var sourceChest in knownChests)
                                {
                                    var inputs = machine.GetRecipeFromChest(sourceChest);
                                    if (inputs is not null)
                                    {
                                        return new JunimoAssignment(projectType, location, portal, originTile, sourceChest, machine, inputs);
                                    }
                                }

                                emptyMachines.Add(machine);
                            }
                            visitedTiles.Add(adjacentTile);
                        }
                        else if (isWalkable)
                        {
                            tilesToInvestigate.Enqueue(adjacentTile);
                        }
                        else
                        {
                            visitedTiles.Add(adjacentTile);
                        }
                    }

                    if (location.IsOutdoors && projectType == JunimoType.Fishing)
                    {
                        foreach (var direction in crabPotReachableDirections)
                        {
                            var adjacentTile = tile + direction;
                            if (visitedTiles.Contains(adjacentTile))
                            {
                                continue;
                            }

                            map.GetCrabPotAt(adjacentTile, tile, out var machine);

                            if (machine is not null && machine.IsCompatibleWithJunimo(projectType) && !busyMachines.Contains(machine.GameObject))
                            {
                                if (machine.HeldObject is not null)
                                {
                                    // Try and find a chest to tote it to
                                    var targetChest = knownChests.FirstOrDefault(chest => chest.IsPreferredStorageForMachinesOutput(machine.HeldObject));
                                    if (targetChest is not null)
                                    {
                                        return new JunimoAssignment(projectType, location, portal, originTile, machine, targetChest, itemsToRemoveFromChest: null);
                                    }

                                    fullMachines.Add(machine);
                                }
                                else if (machine.IsIdle)
                                {
                                    // Try and find a chest to supply it from
                                    foreach (var sourceChest in knownChests)
                                    {
                                        var inputs = machine.GetRecipeFromChest(sourceChest);
                                        if (inputs is not null)
                                        {
                                            return new JunimoAssignment(projectType, location, portal, originTile, sourceChest, machine, inputs);
                                        }
                                    }

                                    emptyMachines.Add(machine);
                                }
                                visitedTiles.Add(adjacentTile);
                            }
                        }
                    }

                    visitedTiles.Add(tile);
                }

                // Couldn't find any work delivering to machines or pulling from machines where there was an existing stack.
                // The last type of work we're willing to take on is to pull from a machine and place the item in the closest
                // chest with room to spare for a new stack.
                var fullMachine = fullMachines.FirstOrDefault();
                if (fullMachine is not null)
                {
                    var chestWithSpace = knownChests.FirstOrDefault(chest => chest.IsPossibleStorageForMachinesOutput(fullMachine.HeldObject!));
                    if (chestWithSpace is not null)
                    {
                        return new JunimoAssignment(projectType, location, portal, originTile, fullMachine, chestWithSpace, itemsToRemoveFromChest: null);
                    }
                }

                // Maybesomeday:  Store a list of all the tiles we walked, and walk the whole list again looking for chests and
                // machines on diagonals.
            }

            return null;
        }

        public void WriteToLog(string message, LogLevel level, bool isOnceOnly)
        {
            this.mod.WriteToLog(message, level, isOnceOnly);
        }
    }
}
