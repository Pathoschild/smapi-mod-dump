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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using static ItemExtensions.Additions.Sorter;
using System.Text;
using Object = StardewValley.Object;
using StardewValley.Extensions;
using ItemExtensions.Models.Enums;


namespace ItemExtensions.Patches.Resource_spawning;

internal class MountainPatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif

    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(MountainPatches)}\": postfixing SDV method \"Mountain.quarryDayUpdate()\".");

        harmony.Patch(
            original: AccessTools.Method(typeof(Mountain), "DayUpdate"),
            postfix: new HarmonyMethod(typeof(MountainPatches), nameof(Post_DayUpdate))
        );
    }

    public static void Post_DayUpdate(Mountain __instance, int dayOfMonth)
    {
        var stones = MineShaftPatches.VanillaStones;
        if (Game1.player.MiningLevel > 4)
        {
            if (Game1.player.MiningLevel < 7)
            {
                stones = new[] { "32", "34", "36", "38", "40", "42", "48", "50", "52", "54", "56", "58", "450", "668", "670", "760", "762", "845", "846", "847", "849", "850" };
            }
            else
            {
                stones = new[] { "32", "34", "36", "38", "40", "42", "48", "50", "52", "54", "56", "58", "450", "668", "670", "760", "762", "845", "846", "847", "849" };
            }
        }

        //if none are an ore
        var all = __instance.Objects.Values.Where(o => stones.Contains(o.ItemId));

        if (all?.Any() == false)
            return;

        var canApply = GetForMountain(__instance);
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

            //replace & break to avoid re-setting. no need to check ladder because it's the mountain
            __instance.Objects[stone.TileLocation] = ore;
        }
    }

    private static Dictionary<string, double> GetForMountain(Mountain mtn, bool isClump = false)
    {
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

                    //skip any not set as volcano
                    if (spawns.Type != MineType.Mountain)
                        continue;

                    foreach (var floor in spawns.RealFloors)
                    {
#if DEBUG
                        Log($"Data: {floor}");
#endif
                        if (string.IsNullOrWhiteSpace(floor))
                            continue;

                        all.Add(id, spawns.SpawnFrequency);
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
        Log($"In mountain: " + sb);
#endif
        return all;
    }
}
