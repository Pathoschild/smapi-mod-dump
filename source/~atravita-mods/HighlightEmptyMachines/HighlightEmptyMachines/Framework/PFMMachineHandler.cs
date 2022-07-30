/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Collections;
using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.Utils.Extensions;
using StardewModdingAPI.Utilities;

namespace HighlightEmptyMachines.Framework;

/// <summary>
/// Enum to hold the possible PFM Machine statuses.
/// </summary>
internal enum PFMMachineStatus
{
    /// <summary>
    /// This machine is enabled in settings and can recieve input.
    /// </summary>
    Enabled,

    /// <summary>
    /// This machine is invalid for some reason.
    /// </summary>
    Invalid,

    /// <summary>
    /// This machine is disabled in settings.
    /// </summary>
    Disabled,
}

/// <summary>
/// Static class that handles PFM machines.
/// </summary>
internal static class PFMMachineHandler
{
    private static readonly PerScreen<Dictionary<int, PFMMachineStatus>> ValidMachinesPerScreen = new(() => new Dictionary<int, PFMMachineStatus>());
    private static readonly DefaultDict<int, HashSet<PFMMachineData>> Recipes = new(() => new HashSet<PFMMachineData>());
    private static IProducerFrameworkModAPI? pfmAPI = null;

    /// <summary>
    /// Gets a lookup table between machines and their current status.
    /// </summary>
    internal static Dictionary<int, PFMMachineStatus> ValidMachines => ValidMachinesPerScreen.Value;

    /// <summary>
    /// Gets a list of PFM machines (for use in GMCM).
    /// </summary>
    internal static IEnumerable<int> PFMMachines => Recipes.Keys;

    private static List<Dictionary<string, object>>? MachineRecipes => pfmAPI?.GetRecipes();

    /// <summary>
    /// Tries to grab the PFM api.
    /// </summary>
    /// <param name="modRegistry">ModRegistry.</param>
    /// <returns>True if API grabbed, false otherwise.</returns>
    internal static bool TryGetAPI(IModRegistry modRegistry)
        => new IntegrationHelper(ModEntry.ModMonitor, ModEntry.TranslationHelper, modRegistry)
            .TryGetAPI("Digus.ProducerFrameworkMod", "1.7.4", out pfmAPI);

    /// <summary>
    /// Prints the pfm machine recipes to console.
    /// </summary>
    /// <param name="command">Command name.</param>
    /// <param name="args">Command arguments.</param>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Expected format of SMAPI commands")]
    internal static void PrintPFMRecipes(string command, string[] args)
    {
        ModEntry.ModMonitor.Log("PFM Machine Recipes", LogLevel.Info);
        foreach ((int id, HashSet<PFMMachineData> v) in Recipes)
        {
            ModEntry.ModMonitor.Log($"Looking at machine {id.GetBigCraftableName()}", LogLevel.Info);
            foreach (PFMMachineData recipe in v)
            {
                ModEntry.ModMonitor.Log('\t' + recipe.ToString(), LogLevel.Info);
            }
        }
    }

    /// <summary>
    /// Run at save load, looks up the PFM machines and saves their data.
    /// </summary>
    internal static void ProcessPFMRecipes()
    {
        List<Dictionary<string, object>>? recipes = MachineRecipes;

        if (recipes is null || recipes.Count == 0)
        {
            ModEntry.ModMonitor.Log($"PFM recipes not found?");
            return;
        }
        foreach (Dictionary<string, object>? item in recipes)
        {
            if (!item.TryGetValue("MachineID", out object? id) || id is not int)
            {
                continue;
            }

            bool outdoorsOnly = item.TryGetValue("RequiredOutdoors", out object? val) && val is bool b && b;

            StardewSeasons seasons;
            if (item.TryGetValue("RequiredSeason", out object? seasonList)
                && seasonList is List<string> seasonListActual && seasonListActual.Count > 0)
            {
                seasons = seasonListActual.ParseSeasonList();
            }
            else
            {
                seasons = StardewSeasons.All;
            }

            StardewWeather weather;
            if (item.TryGetValue("RequiredWeather", out object? weatherList)
                && weatherList is List<string> weatherListActual && weatherListActual.Count > 0)
            {
                weather = weatherListActual.ParseWeatherList();
            }
            else
            {
                weather = StardewWeather.All;
            }

            PFMMachineData recipe = new(
                outdoorsOnly,
                seasons,
                weather,
                item.TryGetValue("RequiredLocation", out object? locs) && locs is List<string> locationList && locationList.Count > 0 ? locationList : null);
            Recipes[(int)id].Add(recipe);
        }
    }

    /// <summary>
    /// Refreshes the validity list.
    /// </summary>
    /// <param name="location">The location to analyze.</param>
    internal static void RefreshValidityList(GameLocation? location)
    {
        if (location is null || pfmAPI is null)
        {
            return;
        }

        ValidMachines.Clear();
        StardewSeasons season = SeasonExtensions.GetSeasonFromGame(location);
        StardewWeather weather = GetPFMWeather();
        bool isOutDoors = location.IsOutdoors;
        foreach (int machine in PFMMachines)
        {
            if (ModEntry.Config.ProducerFrameworkModMachines.TryGetValue(machine.GetBigCraftableName(), out bool setting) && setting)
            {
                foreach (PFMMachineData recipe in Recipes[machine])
                {
                    if ((isOutDoors || !recipe.OutdoorsOnly)
                        && recipe.AllowedSeasons.HasFlag(season)
                        && recipe.AllowedWeathers.HasFlag(weather)
                        && (recipe.ValidLocations is null || recipe.ValidLocations.Contains(location.Name)))
                    {
                        ValidMachines[machine] = PFMMachineStatus.Enabled;
                        ModEntry.ModMonitor.DebugOnlyLog($"{machine.GetBigCraftableName()} is enabled");
                        goto Continue;
                    }
                }
                ModEntry.ModMonitor.DebugOnlyLog($"{machine.GetBigCraftableName()} is invalid");
                ValidMachines[machine] = PFMMachineStatus.Invalid;
            }
            else
            {
                ModEntry.ModMonitor.DebugOnlyLog($"{machine.GetBigCraftableName()} is disabled in config.");
                ValidMachines[machine] = PFMMachineStatus.Disabled;
            }
Continue:
            ;
        }
    }

    /// <summary>
    /// Get's PFM's idea of the current weather.
    /// </summary>
    /// <returns>Weather enum.</returns>
    private static StardewWeather GetPFMWeather()
    {
        if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason) || (SaveGame.loaded?.weddingToday ?? Game1.weddingToday))
        {
            return StardewWeather.Sunny;
        }
        else if (Game1.isSnowing)
        {
            return StardewWeather.Snowy;
        }
        else if (Game1.isRaining)
        {
            return Game1.isLightning ? StardewWeather.Stormy : StardewWeather.Rainy;
        }
        else if (SaveGame.loaded?.isDebrisWeather ?? Game1.isDebrisWeather)
        {
            return StardewWeather.Windy;
        }
        return StardewWeather.Sunny;
    }

    /// <summary>
    /// Data from a PFM machine, processed.
    /// </summary>
    internal readonly struct PFMMachineData
    {
        /// <summary>
        /// If the recipe can only be run for an outside machine.
        /// </summary>
        internal readonly bool OutdoorsOnly;

        /// <summary>
        /// If there's season limitations on the recipe.
        /// </summary>
        internal readonly StardewSeasons AllowedSeasons;

        /// <summary>
        /// If there's weather limitations on the recipe.
        /// </summary>
        internal readonly StardewWeather AllowedWeathers;

        /// <summary>
        /// If there's location limiations on the recipe.
        /// </summary>
        internal readonly List<string>? ValidLocations;

        /// <summary>
        /// Initializes a new instance of the <see cref="PFMMachineData"/> struct.
        /// </summary>
        /// <param name="outdoorsOnly">Whether the machine recipe is outdoors only.</param>
        /// <param name="allowedSeasons">Whether there's season limitations.</param>
        /// <param name="allowedWeathers">Whether there's weather limitations.</param>
        /// <param name="validLocations">Whether there's location limitations.</param>
        public PFMMachineData(bool outdoorsOnly, StardewSeasons allowedSeasons, StardewWeather allowedWeathers, List<string>? validLocations)
        {
            this.OutdoorsOnly = outdoorsOnly;
            this.AllowedSeasons = allowedSeasons;
            this.AllowedWeathers = allowedWeathers;
            this.ValidLocations = validLocations;
        }

        /// <inheritdoc />
        public override string ToString()
            => $"Outdoors {this.OutdoorsOnly}, AllowedSeasons {this.AllowedSeasons}, AllowedWeathers {this.AllowedSeasons}, ValidLocations {(this.ValidLocations is null ? "null" : string.Join(", ", this.ValidLocations))}";
    }
}