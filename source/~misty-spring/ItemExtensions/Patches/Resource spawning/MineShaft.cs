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
using HarmonyLib;
using ItemExtensions.Additions.Clumps;
using ItemExtensions.Models.Enums;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using static ItemExtensions.Additions.Sorter;
using Object = StardewValley.Object;

namespace ItemExtensions.Patches;

public class MineShaftPatches
{
    internal static readonly string[] VanillaStones =
    {
        //copper (751) and iron (290) are fairly low-cost, so they're replaced by default. but because gold and iridium are rarer, they're excluded. the rest of IDs are stones
        "32", "34", "36", "38", "40", "42", "48", "50", "52", "54", "56", "58", "450", "668", "670", "760", "762" //"290", "751", 
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

            if (ModEntry.Config.TerrainFeatures)
                CheckTerrainFeatures(__instance);

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
        var stones = VanillaStones;
        if (Game1.player.MiningLevel > 4)
        {
            if (Game1.player.MiningLevel < 7)
            {
                
                stones = new[]{ "32", "34", "36", "38", "40", "42", "48", "50", "52", "54", "56", "58", "450", "668", "670", "760", "762", "290" };
            }
            else
            {
                stones = new[]{ "32", "34", "36", "38", "40", "42", "48", "50", "52", "54", "56", "58", "450", "668", "670", "760", "762", "290", "751" };
            }
        }
        
        //if none are an ore
        var all = mineShaft.Objects.Values.Where(o => stones.Contains(o.ItemId));

        if (all?.Any() == false)
            return;
        
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

            //check ladder
            if (mineShaft.tileBeneathLadder == ore.TileLocation)
            {
#if DEBUG
                Log($"Changing tile...(old {mineShaft.tileBeneathLadder})");
#endif
                var tile = Vector2.Zero;
                var canReplace = false;
                for (var i = 0; i < mineShaft.Objects.Length; i++)
                {
                    tile = mineShaft.getRandomTile();
                    if (mineShaft.getObjectAtTile((int)tile.X, (int)tile.Y) is null)
                        continue;

                    canReplace = true;
                    break;
                }
                
                if(canReplace)
                    mineShaft.tileBeneathLadder = tile;
#if DEBUG
                Log($"Tile changed to {mineShaft.tileBeneathLadder}.");
#endif
            }
        }
    }

    private static void CheckResourceClumps(MineShaft mineShaft)
    {
        //get all clumps
        var all = mineShaft.resourceClumps;

        var canApply = GetAllForThisLevel(mineShaft, true);
        if (canApply is null || canApply.Any() == false)
            return;

        var toReplace = new Dictionary<int, ResourceClump>();
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
            var index = mineShaft.resourceClumps.IndexOf(stone);
            toReplace.Add(index, newClump);
        }

        foreach (var (index,clump) in toReplace)
        {
            mineShaft.resourceClumps[index] = clump;
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

                    if (spawns.Type == MineType.Frenzy && spawns.LastFrenzy == (Game1.dayOfMonth, Game1.season))
                        continue;

                    if (spawns.Type == MineType.Volcano)
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

                            //(for frenzy types, set last frenzy)
                            if (spawns.Type == MineType.Frenzy)
                            {
                                spawns.LastFrenzy = (Game1.dayOfMonth, Game1.season);
                            }
                            
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
    
    private static void CheckTerrainFeatures(MineShaft mineShaft)
    {
        var mineLevel = mineShaft.mineLevel;
        var currentCount = 0;
        var maxCount = GetMaxFeatures(mineLevel);
        var all = new Dictionary<string, double>();
        //check every tree data
        foreach (var (id, data) in ModEntry.MineTerrain)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(data.TerrainFeatureId))
                    continue;
                
                //if not spawnable on mines, skip
                if (data.RealSpawnData is null || data.RealSpawnData.Any() == false)
                    continue;

                foreach (var spawns in data.RealSpawnData)
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
                    if (spawns.Type == MineType.Qi && mineShaft.GetAdditionalDifficulty() <= 0)
                        continue;

                    //if vanilla-only & qi on, skip
                    if (spawns.Type == MineType.Normal && mineShaft.GetAdditionalDifficulty() > 0)
                        continue;

                    if (spawns.Type == MineType.Frenzy && spawns.LastFrenzy == (Game1.dayOfMonth, Game1.season))
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

                            //(for frenzy types, set last frenzy)
                            if (spawns.Type == MineType.Frenzy)
                            {
                                spawns.LastFrenzy = (Game1.dayOfMonth, Game1.season);
                            }
                            
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
                Log($"Error while parsing mine level for '{id}': {e}\n  This specific entry will be skipped.", LogLevel.Warn);
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
        if(all.Any() == false)
            return;

        //for every stone we selected
        foreach (var (treeType, chance) in all)
        {
            //choose a %
            var nextDouble = Game1.random.NextDouble();
#if DEBUG
            Log($"Chance: {nextDouble} for data {treeType}");
#endif
            var sorted = GetAllForThisDouble(nextDouble, all);

            //shouldn't happen but a safe check is a safe check
            if (sorted.Any() == false)
                continue;
            
            //get tile
            var tile = Vector2.Zero;
            var canReplace = false;
            for (var i = 0; i < mineShaft.Objects.Length; i++)
            {
                tile = mineShaft.getRandomTile();
                if (mineShaft.getObjectAtTile((int)tile.X, (int)tile.Y) is not null)
                    continue;

                canReplace = true;
                break;
            }

            //if didn't find a valid data tile
            if(canReplace == false)
            {
                continue;
            }

            var id = Game1.random.ChooseFrom(sorted);
            TerrainFeature terrainFeature;

            var data = ModEntry.MineTerrain[id];

            if(data.Type == FeatureType.Tree) 
            {
                terrainFeature = new Tree(data.TerrainFeatureId);
                
                if(data.GrowthStage > -1)
                    (terrainFeature as Tree).growthStage.Value = data.GrowthStage;

                if(data.Stump)
                    (terrainFeature as Tree).stump.Value = true;

                if(Game1.random.NextBool(data.MossChance))
                    (terrainFeature as Tree).hasMoss.Value = true;

            }
            else if(data.Type == FeatureType.FruitTree)
            {
                terrainFeature = new FruitTree(data.TerrainFeatureId, data.GrowthStage);
                
                if(data.GrowthStage > -1)
                    (terrainFeature as FruitTree).growthStage.Value = data.GrowthStage;

                for(var i=0; i<data.FruitAmount;i++)
                {
                    (terrainFeature as FruitTree).TryAddFruit();
                }
            }
            else if (data.Type == FeatureType.GiantCrop)
                terrainFeature = new GiantCrop(data.TerrainFeatureId, tile);
            else
                continue;

            //replace & break to avoid re-setting
            mineShaft.terrainFeatures.Add(tile, terrainFeature);

            //check ladder
            if (mineShaft.tileBeneathLadder == tile)
            {
#if DEBUG
                Log($"Changing tile...(old {mineShaft.tileBeneathLadder})");
#endif
                var tile2 = Vector2.Zero;
                var canReplace2 = false;
                for (var i = 0; i < mineShaft.Objects.Length; i++)
                {
                    tile2 = mineShaft.getRandomTile();
                    if (mineShaft.getObjectAtTile((int)tile2.X, (int)tile2.Y) is null)
                        continue;

                    canReplace2 = true;
                    break;
                }
                
                if(canReplace2)
                    mineShaft.tileBeneathLadder = tile2;
#if DEBUG
                Log($"Tile changed to {mineShaft.tileBeneathLadder}.");
#endif
            }

            //up count of spawned features
            currentCount++;

            //if it reaches level max, break
            if(currentCount >= maxCount)
                break;
        }
    }
}
