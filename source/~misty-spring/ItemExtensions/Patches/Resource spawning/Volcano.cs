/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using ItemExtensions.Additions.Clumps;
using ItemExtensions.Models.Enums;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using static ItemExtensions.Additions.Sorter;
using System.Text;
using Object = StardewValley.Object;

namespace ItemExtensions.Patches.Resource_spawning;

internal class VolcanoPatches
{
    private static readonly string[] VolcanoStones =
    {
        "32", "34", "36", "38", "40", "42", "48", "50", "52", "54", "56", "58", "450", "668", "670", "760", "762", "845", "846", "847" //849 850 
    };
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif

    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(VolcanoPatches)}\": postfixing SDV method \"VolcanoDungeon.createStone()\".");

        harmony.Patch(
            original: AccessTools.Method(typeof(VolcanoDungeon), "GenerateEntities"),
            postfix: new HarmonyMethod(typeof(VolcanoPatches), nameof(Post_GenerateEntities))
        );
    }

    private static void Post_GenerateEntities(VolcanoDungeon __instance)
    {
        try
        {
            //don't patch anything that's negative
            if (__instance.level.Value < 1 || __instance.level.Value % 10 == 0)
                return;

            CheckResourceNodes(__instance);
            CheckResourceClumps(__instance);
        }
        catch (Exception e)
        {
            Log($"Error when postfixing entity generation for level {__instance.level.Value}: {e}", LogLevel.Error);
        }
    }

    private static void CheckResourceNodes(VolcanoDungeon volcano)
    {
        var stones = VolcanoStones;
        if (Game1.player.MiningLevel > 4)
        {
            if (Game1.player.MiningLevel < 7)
            {
                stones = new[] { "32", "34", "36", "38", "40", "42", "48", "50", "52", "54", "56", "58", "450", "668", "670", "760", "762", "845", "846", "847", "849" , "850" };
            }
            else
            {
                stones = new[] { "32", "34", "36", "38", "40", "42", "48", "50", "52", "54", "56", "58", "450", "668", "670", "760", "762", "845", "846", "847", "849" };
            }
        }

        //if none are an ore
        var all = volcano.Objects.Values.Where(o => stones.Contains(o.ItemId));

        if (all?.Any() == false)
            return;

        var canApply = GetForDungeonLevel(volcano);
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

#if DEBUG
            Log($"Spawning {ore.DisplayName}...");
#endif
            //replace & break to avoid re-setting. no need to check ladder because it's the dungeon
            volcano.Objects[stone.TileLocation] = ore;
        }
    }

    private static void CheckResourceClumps(VolcanoDungeon volcano)
    {
        //get all clumps
        var all = volcano.resourceClumps;

        var canApply = GetForDungeonLevel(volcano, true);
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
            var index = volcano.resourceClumps.IndexOf(stone);
            toReplace.Add(index, newClump);
        }

        foreach (var (index, clump) in toReplace)
        {
            volcano.resourceClumps[index] = clump;
        }
    }

    private static Dictionary<string, double> GetForDungeonLevel(VolcanoDungeon volcano, bool isClump = false)
    {
        var mineLevel = volcano.level.Value;
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

                    //skip any not set as volcano
                    if (spawns.Type != MineType.Volcano)
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
}
