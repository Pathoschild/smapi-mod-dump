/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Numerics;

using CommunityToolkit.Diagnostics;
using NetEscapades.EnumGenerators;

namespace AtraShared.ConstantsAndEnums;

/// <summary>
/// Seasons as flags, typically used for season constraints.
/// </summary>
[Flags]
[EnumExtensions]
public enum StardewSeasons : byte
{
    /// <summary>
    /// No season constraints.
    /// </summary>
    None = 0,

    /// <summary>
    /// Spring.
    /// </summary>
    Spring = 0b1,

    /// <summary>
    /// Summer.
    /// </summary>
    Summer = 0b10,

    /// <summary>
    /// Fall.
    /// </summary>
    Fall = 0b100,

    /// <summary>
    /// Winter.
    /// </summary>
    Winter = 0b1000,

    /// <summary>
    /// Every season.
    /// </summary>
    All = Spring | Summer | Fall | Winter,
}

/// <summary>
/// Extensions for the seasons enum.
/// </summary>
public static partial class SeasonExtensions
{
    /// <summary>
    /// Parses a list of strings into the season enum.
    /// </summary>
    /// <param name="seasonList">List of strings of seasons...</param>
    /// <returns>Stardew Seasons.</returns>
    public static StardewSeasons ParseSeasonList(this IEnumerable<string> seasonList)
    {
        Guard.IsNotNull(seasonList);

        StardewSeasons season = StardewSeasons.None;
        foreach (string? seasonstring in seasonList)
        {
            if (StardewSeasonsExtensions.TryParse(name: seasonstring.AsSpan().Trim(), value: out StardewSeasons s, ignoreCase: true))
            {
                season |= s;
            }
        }
        return season;
    }

    public static bool TryParseSeasonList(this IEnumerable<string> seasonList, out StardewSeasons seasons)
    {
        Guard.IsNotNull(seasonList);

        seasons = StardewSeasons.None;
        foreach (string? seasonstring in seasonList)
        {
            if (StardewSeasonsExtensions.TryParse(name: seasonstring.AsSpan().Trim(), value: out StardewSeasons s, ignoreCase: true))
            {
                seasons |= s;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Gets the StardewSeason enum for the current location.
    /// </summary>
    /// <param name="loc">GameLocation.</param>
    /// <returns>Season.</returns>
#warning - need to fix in Stardew 1.6.
    public static StardewSeasons GetSeasonFromGame(GameLocation? loc)
        => GetSeasonFromIndex(Utility.getSeasonNumber(Game1.GetSeasonForLocation(loc)));

    public static StardewSeasons GetSeasonFromIndex(int index)
        => index switch
        {
            0 => StardewSeasons.Spring,
            1 => StardewSeasons.Summer,
            2 => StardewSeasons.Fall,
            3 => StardewSeasons.Winter,
            _ => StardewSeasons.None,
        };

    /// <summary>
    /// Shifts all values in a StardewSeason enum over by one month.
    /// </summary>
    /// <param name="seasons">Initial seasons.</param>
    /// <returns>Seasons shifted by one.</returns>
    public static StardewSeasons GetNextSeason(this StardewSeasons seasons)
    {
        int shifted = (byte)seasons << 1;

        if (seasons.HasFlag(StardewSeasons.Winter))
        {
            shifted |= 0b1;
        }
        shifted &= 0b1111;
        return (StardewSeasons)shifted;
    }

    public static StardewSeasons GetPreviousSeason(this StardewSeasons seasons)
    {
        int shifted = (byte)seasons >> 1;
        if (seasons.HasFlag(StardewSeasons.Spring))
        {
            shifted |= 0b1000;
        }
        shifted &= 0b1111;
        return (StardewSeasons)shifted;
    }

    public static int CountSeasons(this StardewSeasons seasons) => BitOperations.PopCount((uint)seasons);

    public static int ToSeasonIndex(this StardewSeasons seasons)
        => seasons switch
        {
            StardewSeasons.Spring => 0,
            StardewSeasons.Summer => 1,
            StardewSeasons.Fall => 2,
            StardewSeasons.Winter => 3,
            _ => ThrowHelper.ThrowArgumentException<int>("Expected a single season.")
        };
}