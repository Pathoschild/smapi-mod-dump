/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Text;
using System.Xml.Schema;
using HarmonyLib;
using ItemExtensions.Additions.Clumps;
using ItemExtensions.Models.Enums;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace ItemExtensions.Patches;

public class MineShaftPatches
{
    private static readonly string[] VanillaStones =
    {
        //copper (751) and iron (290) are fairly low-cost, so they're replaced by default. but because gold and iridium are rarer, they're excluded. the rest of IDs are stones
        "32", "34", "36", "38", "40", "42", "48", "50", "52", "54", "56", "58", "290", "450", "668", "670", "751", "760", "762"
    };
    internal static List<string> OrderedByChance { get; set; }= new();
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(MineShaftPatches)}\": postfixing SDV method \"MineShaft.populateLevel()\".");
        
        harmony.Patch(
            original: AccessTools.Method(typeof(MineShaft), "populateLevel"),
            postfix: new HarmonyMethod(typeof(MineShaftPatches), nameof(Post_populateLevel))
        );
    }
    
    private static void Post_populateLevel(MineShaft __instance)
    {
        try
        {
            //don't patch anything that's negative
            if (__instance.mineLevel < 1 || __instance.mineLevel % 10 == 0)
                return;

            CheckResourceNodes(__instance);
            
            //clumps aren't changed here to avoid issues because the zone is special
            if(__instance.mineLevel != 77377)
                CheckResourceClumps(__instance);
            else
            {
                var canApply = GetAllForThisLevel(__instance, true);
                if (canApply is null || canApply.Any() == false)
                    return;

                foreach (var( id, chance) in canApply)
                {
                    if(Game1.random.NextDouble() > chance)
                        continue;
                    
                    for (var i = 0; i < 10; i++)
                    {
                        var placeable = true;
                        var tile = __instance.getRandomTile();
                        for (var j = 1; j < ModEntry.BigClumps[id].Width; j++)
                        {
                            for (var k = 1; k < ModEntry.BigClumps[id].Height; k++)
                            {
                                if(__instance.isTileClearForMineObjects(tile + new Vector2(j,k)))
                                    continue;
                                
                                placeable = false;
                                break;
                            }

                            if (!placeable)
                                break;
                        }
                        if(!placeable)
                            continue;

                        __instance.resourceClumps.Add(ExtensionClump.Create(id, ModEntry.BigClumps[id],tile));
                        break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log($"Error when postfixing populate for level {__instance.mineLevel}: {e}", LogLevel.Error);
        }
    }

    private static void CheckResourceNodes(MineShaft mineShaft)
    {
        //if none are an ore
        if (mineShaft.Objects.Values.Any(o => VanillaStones.Contains(o.ItemId)) == false)
            return;

        //randomly chooses which stones to replace
        var all = mineShaft.Objects.Values.Where(o => VanillaStones.Contains(o.ItemId));

        var canApply = GetAllForThisLevel(mineShaft);
        if (canApply is null || canApply.Any() == false)
            return;

        //for every stone we selected
        foreach (var stone in all)
        {
            //choose a %
            var nextDouble = Game1.random.NextDouble();
#if DEBUG
            Log($"Chance: {nextDouble} for stone at {stone.TileLocation}");
#endif
            var sorted = GetAllForThisDouble(nextDouble, canApply);

            //shouldn't happen but a safe check is a safe check
            if (sorted.Any() == false)
                continue;
            
            var id = Game1.random.ChooseFrom(sorted);
            var ore = new Object(id, 1)
            {
                TileLocation = stone.TileLocation, 
                //Location = stone.Location,
                MinutesUntilReady = ModEntry.Ores[id].Health
            };

            //replace & break to avoid re-setting
            mineShaft.Objects[stone.TileLocation] = ore;
        }
    }

    private static void CheckResourceClumps(MineShaft mineShaft)
    {
        //if none are a clump
        if (mineShaft.terrainFeatures.Values.Any(t => t is ResourceClump == false))
            return;

        //randomly chooses which stones to replace
        var all = mineShaft.terrainFeatures.Values.Where(t => t is ResourceClump);

        var canApply = GetAllForThisLevel(mineShaft, true);
        if (canApply is null || canApply.Any() == false)
            return;

        //for every stone we selected
        foreach (var stone in all)
        {
            //choose a %
            var nextDouble = Game1.random.NextDouble();
            var sorted = GetAllForThisDouble(nextDouble, canApply);

            //shouldn't happen but a safe check is a safe check
            if (sorted.Any() == false)
                continue;
            
            var clump = Game1.random.ChooseFrom(sorted);
#if DEBUG
            Log($"Chance: {nextDouble}. Chose {clump}");
#endif
            var newClump = ExtensionClump.Create(clump, ModEntry.BigClumps[clump], stone.Tile);

            //replace & break to avoid re-setting
            mineShaft.terrainFeatures[stone.Tile] = newClump;
        }
    }

    /// <summary>
    /// Grabs all ores that match a random double. (E.g, all with a chance bigger than 0.x, starting from smallest)
    /// </summary>
    /// <param name="randomDouble"></param>
    /// <param name="canApply"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private static IList<string> GetAllForThisDouble(double randomDouble, Dictionary<string, double> canApply)
    {
        try
        {
            var validResources = new Dictionary<string, double>();
            foreach (var (id, chance) in canApply)
            {
                //e.g. if randomDouble is 0.56 and this ore's chance is 0.3, it'll be skipped
                if (randomDouble > chance)
                    continue;
                validResources.Add(id, chance);
            }

            if (validResources.Any() == false)
                return ArraySegment<string>.Empty;

            //sorts by smallest to biggest
            var sorted = from entry in validResources orderby entry.Value select entry;
            //turns sorted to list. we do this instead of calculating directly because IOrdered has no indexOf, and I'm too exhausted to think of something better (perhaps optimize in the future)
            var convertedSorted = new List<string>();
            foreach (var pair in sorted)
            {
                convertedSorted.Add(pair.Key);
#if DEBUG
            Log($"Added {pair.Key} to sorted list ({pair.Value})");
#endif
            }

            var result = new List<string>();
            for (var i = 0; i < convertedSorted.Count; i++)
            {
                result.Add(convertedSorted[i]);
                if (i + 1 >= convertedSorted.Count)
                    break;
                
                var current = convertedSorted[i];
                var next = convertedSorted[i + 1];
#if DEBUG
                Log($"Added node with {convertedSorted[i]} chance to list.");
#endif

                //if next one has higher %
                //because doubles are always a little off, we do a comparison of difference
                if (Math.Abs(validResources[next] - validResources[current]) > 0.0000001)
                {
                    break;
                }
            }

            return result;
        }
        catch (Exception e)
        {
            Log($"Error while sorting spawn chances: {e}.\n  Will be skipped.", LogLevel.Warn);
            return new List<string>();
        }
    }

    /// <summary>
    /// Gets all allowed ores for this level.
    /// </summary>
    /// <param name="mine">The mine</param>
    /// <param name="isClump">Whether to grab clumps instead of nodes</param>
    /// <returns>An unsorted list with all available spawns.</returns>
    private static Dictionary<string, double> GetAllForThisLevel(MineShaft mine, bool isClump = false)
    {
        var mineLevel = mine.mineLevel;
        var all = new Dictionary<string, double>();
        //check every ore
        foreach (var (id, ore) in isClump ? ModEntry.BigClumps : ModEntry.Ores)
        {
            try
            {
                //if not spawnable on mines, skip
                if (ore.RealSpawnData is null || ore.RealSpawnData.Any() == false)
                    continue;

                foreach (var spawns in ore.RealSpawnData)
                {
                    //if GSQ exists & not valid
                    if (string.IsNullOrWhiteSpace(spawns.Condition) == false &&
                        GameStateQuery.CheckConditions(spawns.Condition) == false)
                        continue;
#if DEBUG
                    Log($"{spawns?.RealFloors.Count} in {id}");
#endif
                    if (spawns?.RealFloors is null)
                        continue;

                    var extraforLevel = spawns.AdditionalChancePerLevel * mineLevel;

                    //if qi-only & not qi on, skip
                    if (spawns.Type == MineType.Qi && mine.GetAdditionalDifficulty() <= 0)
                        continue;

                    //if vanilla-only & qi on, skip
                    if (spawns.Type == MineType.Normal && mine.GetAdditionalDifficulty() > 0)
                        continue;

                    foreach (var floor in spawns.RealFloors)
                    {
#if DEBUG
                        Log($"Data: {floor}");
#endif
                        if (string.IsNullOrWhiteSpace(floor))
                            continue;

                        //if it's of style minSpawnLevel-maxSpawnLevel
                        if (floor.Contains('/'))
                        {
                            var both = ArgUtility.SplitQuoteAware(floor, '/');
                            //if less than 2 values, or can't parse either as int
                            if (both.Length < 2 || int.TryParse(both[0], out var startLevel) == false ||
                                int.TryParse(both[1], out var endLevel) == false)
                                break;

#if DEBUG
                            Log($"Level range: {startLevel} to {endLevel}");
#endif
                            //initial is bigger than current OR max is less than current (& end level isn't max)
                            if (startLevel > mineLevel || (endLevel < mineLevel && endLevel != -999))
                                break; //skip

                            //otherwise, add & break loop
                            all.Add(id, spawns.SpawnFrequency + extraforLevel);
                            break;
                        }

                        //or if level is explicitly included
                        if (int.TryParse(floor, out var isInt) && (isInt == -999 || isInt == mineLevel))
                            all.Add(id, spawns.SpawnFrequency + extraforLevel);
                    }
                }
            }
            catch (Exception e)
            {
                Log($"Error while parsing mine level for {id}: {e}\n  This specific ore will be skipped.", LogLevel.Warn);
            }
        }
        
        #if DEBUG
        var sb = new StringBuilder();
        foreach (var pair in all)
        {
            sb.Append("{ ");
            sb.Append(pair.Key);
            sb.Append(", ");
            sb.Append(pair.Value);
            sb.Append(" }");
            sb.Append(", ");
        }
        Log($"In level {mineLevel}: " + sb);
        #endif
        return all;
    }
}