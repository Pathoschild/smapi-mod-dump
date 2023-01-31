/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.ConstantsAndEnums;

namespace AtraShared.Integrations.Interfaces;

public interface ILastDayToPlantAPI
{
    /// <summary>
    /// Gets the days needed for a specific crop for to grow.
    /// </summary>
    /// <param name="profession">Profession.</param>
    /// <param name="fertilizer">Int ID of fertilizer.</param>
    /// <param name="crop">crop ID.</param>
    /// <param name="season">The season to check for.</param>
    /// <returns>number of days, or null for no entry.</returns>
    /// <remarks>This is not calculated until a Low priority DayStarted. You'll need an even lower priority.</remarks>
    public int? GetDays(Profession profession, int fertilizer, int crop, StardewSeasons season);

    /// <summary>
    /// Gets all the data associated with a specific condition.
    /// </summary>
    /// <param name="profession">Profession to check.</param>
    /// <param name="fertilizer">Fertilizer to check.</param>
    /// <param name="season">The season to get.</param>
    /// <returns>The available data.</returns>
    /// <remarks>Note that profession data is not calculated if there's no player with that profession, and fertilizer data is dependent on player config.
    /// No data = not calculated.</remarks>
    public IReadOnlyDictionary<int, int>? GetAll(Profession profession, int fertilizer, StardewSeasons season);

    /// <summary>
    /// Gets the grow conditions for a specific crop.
    /// </summary>
    /// <param name="crop">Crop to check.</param>
    /// <param name="season">The season to check for.</param>
    /// <returns>(profession, fertilizer) => days.</returns>
    public KeyValuePair<KeyValuePair<Profession, int>, int>[]? GetConditionsPerCrop(int crop, StardewSeasons season);


    /// <summary>
    /// Get the crops we have tracked.
    /// </summary>
    /// <returns>int array of tracked crops.</returns>
    public int[]? GetTrackedCrops();
}

/// <summary>
/// A enum corresponding to the profession to check.
/// </summary>
public enum Profession
{
    None,
    Agriculturalist,
    Prestiged,
}