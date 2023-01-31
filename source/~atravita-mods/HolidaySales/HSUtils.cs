/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection.Emit;
using AtraBase.Toolkit;
using AtraBase.Toolkit.Extensions;

using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;

namespace HolidaySales;

/// <summary>
/// Utilities for this mod.
/// </summary>
internal static class HSUtils
{
    /// <summary>
    /// looks for calls to Utility.isFestivalDay and replaces it with calls to my custom method here.
    /// </summary>
    /// <param name="helper">ILHelper.</param>
    internal static void AdjustIsFestivalCall(this ILHelper helper)
    {
        helper.ForEachMatch(
            new CodeInstructionWrapper[]
            {
                new(OpCodes.Call, typeof(Utility).GetCachedMethod(nameof(Utility.isFestivalDay), ReflectionCache.FlagTypes.StaticFlags)),
            },
            (helper) =>
            {
                helper.ReplaceOperand(typeof(HSUtils).GetCachedMethod(nameof(IsFestivalDayAdjustedForConfig), ReflectionCache.FlagTypes.StaticFlags));
                helper.Insert(new CodeInstruction[]
                {
                    new(OpCodes.Call, typeof(Game1).GetCachedProperty(nameof(Game1.currentLocation), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
                    new(OpCodes.Callvirt, typeof(GameLocation).GetCachedProperty(nameof(GameLocation.Name), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                });
                return true;
            });
    }

    /// <summary>
    /// Adjusts a call to Utility.isFestival to check for the town only.
    /// </summary>
    /// <param name="helper">ILHelper.</param>
    internal static void AdjustIsFestivalCallForTown(this ILHelper helper)
    {
        helper.ForEachMatch(
            new CodeInstructionWrapper[]
            {
                new(OpCodes.Call, typeof(Utility).GetCachedMethod(nameof(Utility.isFestivalDay), ReflectionCache.FlagTypes.StaticFlags)),
            },
            (helper) =>
            {
                helper.ReplaceOperand(typeof(HSUtils).GetCachedMethod(nameof(IsFestivalDayAdjustedForConfig), ReflectionCache.FlagTypes.StaticFlags));
                helper.Insert(new CodeInstruction[]
            {
                    new(OpCodes.Ldstr, "Town"),
            });
                return true;
        });
    }

    /// <summary>
    /// Whether or not stores are closed for the festival after adjustments.
    /// </summary>
    /// <returns>true if the store is closed, false otherwise.</returns>
    internal static bool StoresClosedForFestival()
    {
        if (IsFestivalDayAdjustedForConfig(Game1.dayOfMonth, Game1.currentSeason, "Town"))
        {
            return Utility.getStartTimeOfFestival() < 1900;
        }
        return false;
    }

    /// <summary>
    /// Whether or not the festival should be open or something.
    /// </summary>
    /// <param name="day">Day of month.</param>
    /// <param name="season">Season (as string).</param>
    /// <param name="mapname">Map to search for.</param>
    /// <returns>If it should be considered a festival day for this specific config.</returns>
    internal static bool IsFestivalDayAdjustedForConfig(int day, string season, string mapname)
    {
        return ModEntry.Config.StoreFestivalBehavior switch
        {
            FestivalsShopBehavior.Open => false,
            FestivalsShopBehavior.Closed => Utility.isFestivalDay(day, season),
            FestivalsShopBehavior.MapDependent => IsFestivalDayForMap(day, season, mapname),
            _ => TKThrowHelper.ThrowUnexpectedEnumValueException<FestivalsShopBehavior, bool>(ModEntry.Config.StoreFestivalBehavior),
        };
    }

    /// <summary>
    /// Whether or not it should be considered a festival date for that particular map.
    /// </summary>
    /// <param name="day">day.</param>
    /// <param name="season">season.</param>
    /// <param name="mapname">the map name.</param>
    /// <returns>true if it should be considered a festival day.</returns>
    internal static bool IsFestivalDayForMap(int day, string season, string mapname)
    {
        string? s = season + day;
        if (Game1.temporaryContent.Load<Dictionary<string, string>>(@"Data\Festivals\FestivalDates").ContainsKey(s))
        {
            int index = mapname.IndexOf('_');
            ReadOnlySpan<char> mapRegion;
            if (index == -1)
            {
                mapRegion = "Town";
            }
            else
            {
                index = mapname.IndexOf('_', index + 1);
                if (index == -1)
                {
                    mapRegion = "CustomMapRegion";
                }
                else
                {
                    mapRegion = mapname.AsSpan()[..index];
                }
            }

            try
            {
                Dictionary<string, string>? festivaldata = Game1.temporaryContent.Load<Dictionary<string, string>>($@"Data\Festivals\{s}");
                if (festivaldata.TryGetValue("conditions", out string? conditionsStr))
                {
                    ReadOnlySpan<char> conditions = conditionsStr.GetNthChunk('/', 0).Trim();

                    ModEntry.ModMonitor.Log($"Testing {conditions.ToString()} against {mapRegion.ToString()}");
                    if (conditions.Equals(mapRegion, StringComparison.Ordinal))
                    {
                        return true;
                    }
                    else if (!conditions.StartsWith("Custom", StringComparison.Ordinal) && mapRegion == "Town")
                    {
                        return true;
                    }
                    else if (conditions.StartsWith("Custom", StringComparison.Ordinal) && mapRegion == "CustomMapRegion")
                    {
                        return conditions.IndexOf('_') == conditions.LastIndexOf('_');
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"Error loading festival data for {season} {day}\n\n{ex}", LogLevel.Error);
            }
        }
        return false;
    }
}
