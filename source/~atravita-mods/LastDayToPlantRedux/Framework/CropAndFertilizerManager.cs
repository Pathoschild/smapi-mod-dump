/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Collections.ObjectModel;
using System.Text;

using AtraBase.Collections;
using AtraBase.Toolkit;
using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.StringHandler;

using AtraShared.Caching;
using AtraShared.ConstantsAndEnums;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.Shims;
using AtraShared.Wrappers;

using StardewValley.TerrainFeatures;

namespace LastDayToPlantRedux.Framework;

// fertilizers are filtered while loading, while seeds (which I expect more change for) are filtered while calculating.

/// <summary>
/// Handles a cache of crop and fertilizer data.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1214:Readonly fields should appear before non-readonly fields", Justification = "Reviewed.")]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Keeping like methods together.")]
internal static class CropAndFertilizerManager
{
    private static readonly TickCache<bool> HasStocklist = new(() => Game1.MasterPlayer.hasOrWillReceiveMail("PierreStocklist"));

    // Map conditions to the number of days it takes to grow a crop, per season.
    private static readonly Dictionary<CropCondition, Dictionary<int, int>>[] DaysPerCondition = new Dictionary<CropCondition, Dictionary<int, int>>[4]
    {
        new(),
        new(),
        new(),
        new(),
    };

    /// <summary>
    /// a mapping of fertilizers to a HoeDirt that has them.
    /// </summary>
    private static readonly Dictionary<int, DummyHoeDirt> Dirts = new();

    /// <summary>
    /// a cache of crop data.
    /// </summary>
    private static Dictionary<int, CropEntry> crops = new();

    /// <summary>
    /// a mapping of fertilizers to their localized names.
    /// </summary>
    private static Dictionary<int, string> fertilizers = new();

    // the inverse of DaysPerCondition;
    private static readonly Lazy<Dictionary<int, List<KeyValuePair<CropCondition, int>>>?>[] LastGrowthPerCrop
        = new Lazy<Dictionary<int, List<KeyValuePair<CropCondition, int>>>?>[]
        {
            new(() => GenerateReverseMap(0)),
            new(() => GenerateReverseMap(1)),
            new(() => GenerateReverseMap(2)),
            new(() => GenerateReverseMap(3)),
        };

    private static bool cropsNeedRefreshing = true;
    private static bool fertilizersNeedRefreshing = true;
    private static bool hadStocklistLastCheck = false;
    private static bool requiresReset = true;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "StyleCop doesn't understand records.")]
    private record CropEntry(StardewSeasons Seasons, string GrowthData);

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "StyleCop doesn't understand records.")]
    private record CropCondition(Profession Profession, int Fertilizer);

    #region API

    /// <inheritdoc cref="ILastDayToPlantAPI.GetDays(Profession, int, int, StardewSeasons)"/>
    internal static int? GetDays(Profession profession, int fertilizer, int crop, StardewSeasons season)
    {
        if (season.CountSeasons() != 1)
        {
            return null;
        }

        var seasonIndex = season.ToSeasonIndex();
        var seasonDict = DaysPerCondition[seasonIndex];

        // check with specific fertilizer.
        CropCondition? key = new(profession, fertilizer);
        if (seasonDict.TryGetValue(key, out Dictionary<int, int>? daysDict)
            && daysDict.TryGetValue(crop, out var days))
        {
            return days;
        }

        // check with no fertilizer.
        key = new(profession, 0);
        if (seasonDict.TryGetValue(key, out daysDict)
            && daysDict.TryGetValue(crop, out days))
        {
            return days;
        }
        return null;
    }

    /// <inheritdoc cref="ILastDayToPlantAPI.GetAll(Profession, int, StardewSeasons)"/>
    internal static IReadOnlyDictionary<int, int>? GetAll(Profession profession, int fertilizer, StardewSeasons season)
    {
        if (season.CountSeasons() != 1)
        {
            return null;
        }

        var seasonIndex = season.ToSeasonIndex();
        var seasonDict = DaysPerCondition[seasonIndex];

        CropCondition? key = new(profession, fertilizer);
        if (seasonDict.TryGetValue(key, out var daysDict))
        {
            return new ReadOnlyDictionary<int, int>(daysDict);
        }
        return null;
    }

    /// <inheritdoc cref="ILastDayToPlantAPI.GetConditionsPerCrop(int)"/>
    internal static KeyValuePair<KeyValuePair<Profession, int>, int>[]? GetConditionsPerCrop(int crop, StardewSeasons season)
    {
        if (season.CountSeasons() != 1)
        {
            return null;
        }

        if (LastGrowthPerCrop[season.ToSeasonIndex()].Value?.TryGetValue(crop, out var val) == true)
        {
            return val.Select((kvp) => new KeyValuePair<KeyValuePair<Profession, int>, int>(new KeyValuePair<Profession, int>(kvp.Key.Profession, kvp.Key.Fertilizer), kvp.Value))
                      .OrderBy((kvp) => kvp.Value)
                      .ToArray();
        }
        return null;
    }

    /// <inheritdoc cref="ILastDayToPlantAPI.GetTrackedCrops"/>
    internal static int[]? GetTrackedCrops() => LastGrowthPerCrop.SelectMany(a => a.Value?.Keys ?? Enumerable.Empty<int>()).ToArray();
    #endregion

    #region processing

    /// <summary>
    /// Requests a cache clear.
    /// </summary>
    internal static void RequestReset() => requiresReset = true;

    /// <summary>
    /// Generates the message string to show to the player.
    /// </summary>
    /// <returns>String message, and a bool that indicates if there are crops with a last day to plant today.</returns>
    internal static (string message, bool showplayer) GenerateMessageString()
    {
        ModEntry.ModMonitor.Log($"Processing for day {Game1.dayOfMonth}");
        if (!StardewSeasonsExtensions.TryParse(Game1.currentSeason, value: out StardewSeasons season, ignoreCase: true) || season.CountSeasons() != 1)
        {
            ModEntry.ModMonitor.Log("Invalid season?");
        }

        MultiplayerManager.UpdateOnDayStart();
        Process(season);

        int daysRemaining = 28 - Game1.dayOfMonth;

        if (daysRemaining < 0)
        {
            ModEntry.ModMonitor.Log($"Day of Month {Game1.dayOfMonth} seems odd, do you have a mod adjusting that?", LogLevel.Warn);
            return (string.Empty, false);
        }

        StringBuilder? sb = StringBuilderCache.Acquire();
        sb.Append(I18n.Intro())
           .Append("^^");
        bool hasCrops = false;

        foreach ((CropCondition condition, Dictionary<int, int> cropvalues) in DaysPerCondition[season.ToSeasonIndex()])
        {
            if (cropvalues.Count == 0)
            {
                continue;
            }

            foreach ((int index, int days) in cropvalues)
            {
                if (days != daysRemaining)
                {
                    continue;
                }

                if (!Game1Wrappers.ObjectInfo.TryGetValue(index, out string? data))
                {
                    continue;
                }

                ReadOnlySpan<char> name = data.GetNthChunk('/', SObject.objectInfoDisplayNameIndex);

                if (name.Length == 0)
                {
                    continue;
                }
                hasCrops = true;

                sb.Append(I18n.CropInfo(name.ToString(), days));
                if (condition.Fertilizer != 0)
                {
                    sb.Append(I18n.CropInfo_Fertilizer(fertilizers[condition.Fertilizer]));
                }
                switch (condition.Profession)
                {
                    case Profession.Agriculturalist:
                        sb.Append(I18n.CropInfo_Agriculturalist());
                        break;
                    case Profession.Prestiged:
                        sb.Append(I18n.CropInfo_Prestiged());
                        break;
                    default:
                        break;
                }
                sb.Append('^');
            }
        }

        if (!hasCrops)
        {
            sb.Clear();
            StringBuilderCache.Release(sb);
            return (I18n.None(), false);
        }
        else
        {
            sb.Append("[#]")
                .Append(I18n.CropInfo_Title());
            return (StringBuilderCache.GetStringAndRelease(sb), true);
        }
    }

    private static void Process(StardewSeasons season)
    {
        int seasonIndex = season.ToSeasonIndex();

        // if there is a season change, or if our backing data has changed, dump the cache.
        // first pipe is single intentionally, don't want shortcutting there.
        if (LoadCropData() | LoadFertilizerData()
            || requiresReset || hadStocklistLastCheck != HasStocklist.GetValue())
        {
            if (DaysPerCondition[seasonIndex] is null)
            {
                DaysPerCondition[seasonIndex] = new();
            }
            else
            {
                DaysPerCondition[seasonIndex].Clear();
            }
        }

        StardewSeasons nextSeason = season.GetNextSeason();
        ModEntry.ModMonitor.DebugOnlyLog($"Checking for {season} - next is {nextSeason}", LogLevel.Info);

        // load relevant crops.
        List<int>? currentCrops = crops.Where(c => c.Value.Seasons.HasFlag(season) && !c.Value.Seasons.HasFlag(nextSeason) && FilterCropsToUserConfig(c.Key))
                                .Select(c => c.Key)
                                .ToList();
        ModEntry.ModMonitor.DebugOnlyLog($"Found {currentCrops.Count} relevant crops");

        if (MultiplayerManager.PrestigedAgriculturalistFarmer?.TryGetTarget(out Farmer? prestiged) == true)
        {
            ProcessForProfession(Profession.Prestiged, currentCrops, seasonIndex,  prestiged);

            if (!Context.IsMultiplayer)
            {
                goto SUCCESS;
            }
        }

        if (MultiplayerManager.AgriculturalistFarmer?.TryGetTarget(out Farmer? agriculturalist) == true)
        {
            ProcessForProfession(Profession.Agriculturalist, currentCrops, seasonIndex, agriculturalist);

            if (!Context.IsMultiplayer)
            {
                goto SUCCESS;
            }
        }

        if (MultiplayerManager.NormalFarmer?.TryGetTarget(out Farmer? normal) == true)
        {
            ProcessForProfession(Profession.None, currentCrops, seasonIndex, normal);
            goto SUCCESS;
        }

        ModEntry.ModMonitor.Log($"No available farmers for data analysis, how did this happen?", LogLevel.Warn);
        return;

SUCCESS:
        requiresReset = false;
        hadStocklistLastCheck = HasStocklist.GetValue();
        InventoryWatcher.Reset();
        LastGrowthPerCrop[seasonIndex] = new(() => GenerateReverseMap(seasonIndex));
    }

    private static Dictionary<int, List<KeyValuePair<CropCondition, int>>>? GenerateReverseMap(int season)
    {
        var seasonDict = DaysPerCondition[season];
        if (seasonDict?.Count is 0 or null)
        {
            return null;
        }

        var result = new Dictionary<int, List<KeyValuePair<CropCondition, int>>>();

        foreach ((CropCondition condition, Dictionary<int, int> dictionary) in seasonDict)
        {
            foreach (var (crop, days) in dictionary)
            {
                if (!result.TryGetValue(crop, out var pairs))
                {
                    result[crop] = pairs = new();
                }

                pairs.Add(new(condition, days));
            }
        }

        return result;
    }

    private static void ProcessForProfession(Profession profession, List<int> currentCrops, int seasonIndex, Farmer? farmer = null)
    {
        if (farmer is null)
        {
            ModEntry.ModMonitor.Log($"Could not find farmer for {profession}, continuing.");
            return;
        }
        var seasonDict = DaysPerCondition[seasonIndex];

        if (!seasonDict.TryGetValue(new CropCondition(profession, 0), out Dictionary<int, int>? unfertilized))
        {
            unfertilized = new();
            seasonDict[new CropCondition(profession, 0)] = unfertilized;
        }

        // set up unfertilized.
        foreach (int crop in currentCrops)
        {
            if (!unfertilized.ContainsKey(crop))
            {
                Crop c = new(crop, 0, 0);
                DummyHoeDirt? dirt = Dirts[0];
                dirt.crop = c;
                dirt.nearWaterForPaddy.Value = c.isPaddyCrop() ? 1 : 0;
                int? days = dirt.CalculateTimings(farmer);
                if (days is not null)
                {
                    unfertilized[crop] = days.Value;
                    ModEntry.ModMonitor.DebugOnlyLog($"{crop} takes {days.Value} days");
                }
            }
        }

        foreach (int fertilizer in fertilizers.Keys)
        {
            CropCondition condition = new(profession, fertilizer);

            if (!seasonDict.TryGetValue(condition, out Dictionary<int, int>? dict))
            {
                dict = new();
                seasonDict[condition] = dict;
            }
            else if (!InventoryWatcher.HasSeedChanges)
            {
                // if we've processed this fertilizer before and
                // don't have new seed changes, we can skip this round.
                continue;
            }

            foreach (int crop in currentCrops)
            {
                if (!dict.ContainsKey(crop))
                {
                    Crop c = new(crop, 0, 0);
                    DummyHoeDirt? dirt = Dirts[fertilizer];
                    dirt.crop = c;
                    dirt.nearWaterForPaddy.Value = c.isPaddyCrop() ? 1 : 0;
                    int? days = dirt.CalculateTimings(farmer);

                    // only save when there's a difference from unfertilized.
                    // most fertilizers don't change the time to grow.
                    if (days is not null && unfertilized[crop] != days)
                    {
                        dict[crop] = days.Value;
                        ModEntry.ModMonitor.DebugOnlyLog($"{crop} takes {days.Value} days for fertilizer {fertilizer}.");
                    }
                }
            }
        }
    }

    private static bool FilterCropsToUserConfig(int crop)
    {
        // mixed seeds.
        if (crop == 770)
        {
            return false;
        }

        if (ModEntry.Config.GetAllowedSeeds().Contains(crop)
            || AssetManager.AllowedSeeds.Contains(crop))
        {
            return true;
        }

        if (AssetManager.DeniedSeeds.Contains(crop) || SObject.isWildTreeSeed(crop))
        {
            return false;
        }

        if (!Game1Wrappers.ObjectInfo.TryGetValue(crop, out string? data))
        {
            return false;
        }

        switch (ModEntry.Config.CropsToDisplay)
        {
            case CropOptions.All:
                return true;
            case CropOptions.Purchaseable:
            {
                if (HasStocklist.GetValue())
                {
                    goto case CropOptions.All;
                }

                if (crop < 3000)
                {
                    return Game1.year > 1 || !(crop is 476 or 485 or 489); // the year2 seeds.
                }
                string? name = data.GetNthChunk('/', 0).ToString();
                try
                {
                    if (JsonAssetsShims.IsAvailableSeed(name))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ModEntry.ModMonitor.Log($"Failed while trying to check event preconditions: {ex}", LogLevel.Error);
                    return false;
                }

                goto case CropOptions.Seen;
            }
            case CropOptions.Seen:
            {
                string? name = data.GetNthChunk('/', 0).ToString();
                return InventoryWatcher.Model?.Seeds?.Contains(name) != false;
            }
            default:
                return true;
        }
    }
    #endregion

    #region loading

    /// <summary>
    /// Requests a refresh to the crops cache.
    /// </summary>
    internal static void RequestInvalidateCrops() => cropsNeedRefreshing = true;

    /// <summary>
    /// Requests a refresh to the fertilizer data.
    /// </summary>
    internal static void RequestInvalidateFertilizers() => fertilizersNeedRefreshing = true;

    /// <summary>
    /// Parses crop data into a more optimized format.
    /// </summary>
    /// <returns>If any values have changed..</returns>
    private static bool LoadCropData()
    {
        if (!cropsNeedRefreshing)
        {
            return false;
        }

        cropsNeedRefreshing = false;

        Dictionary<int, CropEntry> ret = new();

        Dictionary<int, string> cropData = Game1.content.Load<Dictionary<int, string>>(AssetManager.CropName.BaseName);
        foreach ((int index, string vals) in cropData)
        {
            ReadOnlySpan<char> seasons = vals.GetNthChunk('/', 1).Trim();

            StardewSeasons seasonEnum = StardewSeasons.None;
            foreach (SpanSplitEntry season in seasons.StreamSplit(null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (StardewSeasonsExtensions.TryParse(season, value: out StardewSeasons s, ignoreCase: true))
                {
                    seasonEnum |= s;
                }
                else
                {
                    ModEntry.ModMonitor.Log($"Crop {index} - {vals} seems to have unparseable season data, skipping", LogLevel.Warn);
                    goto breakcontinue;
                }
            }

            string? growthData = vals.GetNthChunk('/', 0).Trim().ToString();
            ret[index] = new CropEntry(seasonEnum, growthData);
breakcontinue:
            ;
        }

        bool changed = !ret.IsEquivalentTo(crops);
        crops = ret;
        return changed;
    }

    /// <summary>
    /// Loads a list of fertilizers.
    /// </summary>
    /// <returns>If anything changed.</returns>
    private static bool LoadFertilizerData()
    {
        if (!fertilizersNeedRefreshing)
        {
            return false;
        }
        fertilizersNeedRefreshing = false;

        Dictionary<int, string> ret = new();

        DummyHoeDirt dirt = new(0);

        foreach ((int index, string vals) in Game1Wrappers.ObjectInfo)
        {
            ReadOnlySpan<char> catName = vals.GetNthChunk('/', SObject.objectInfoTypeIndex);
            int spaceIndx = catName.GetLastIndexOfWhiteSpace();
            if (spaceIndx < 0)
            {
                continue;
            }

            if (!int.TryParse(catName[(spaceIndx + 1)..], out int category) ||
                (category is not SObject.fertilizerCategory))
            {
                continue;
            }

            dirt.fertilizer.Value = HoeDirt.noFertilizer;
            if (!dirt.plant(index, 0, 0, Game1.player, true, Game1.getFarm()))
            {
                continue;
            }

            string? name = vals.GetNthChunk('/', SObject.objectInfoNameIndex).Trim().ToString();
            if (!IsAllowedFertilizer(index, name))
            {
                continue;
            }

            ret[index] = name;
        }

        bool changed = !ret.IsEquivalentTo(fertilizers);
        fertilizers = ret;
        if (changed || Dirts.Count == 0)
        {
            PopulateFertilizerList();
        }
        return changed;
    }

    private static void PopulateFertilizerList()
    {
        Dirts.Clear();

        Dirts[0] = new(0);

        foreach (int fert in fertilizers.Keys)
        {
            Dirts[fert] = new(fert);
        }

        ModEntry.ModMonitor.DebugOnlyLog($"Set up {Dirts.Count} hoedirts");
    }

    private static bool IsAllowedFertilizer(int id, string name)
    {
        if (ModEntry.Config.GetAllowedFertilizers().Contains(id)
            || AssetManager.AllowedFertilizers.Contains(id))
        {
            return true;
        }
        if (name.Equals("Tree Fertilizer", StringComparison.OrdinalIgnoreCase)
            || AssetManager.DeniedFertilizers.Contains(id))
        {
            return false;
        }
        if (ModEntry.Config.FertilizersToDisplay == FertilizerOptions.All)
        {
            return true;
        }
        return InventoryWatcher.Model?.Fertilizers?.Contains(name) != false;
    }
    #endregion
}
