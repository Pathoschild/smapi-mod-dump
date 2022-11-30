/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

namespace ConfigurableSpecialOrdersUnlock;

public class ModConfig
{
    public int UnlockYear = 1;
    public string UnlockSeason = "Fall";
    public int UnlockDay = 2;

    public bool SkipCutscene = false;

    /// <summary>
    /// Converts the provided year, season and day provided by the config file to an <c>int</c> number of days played.
    /// </summary>
    public int GetUnlockDaysPlayed()
    {
        return ((Clamp(UnlockYear, 1, 10) - 1) * 4 + GetSeasonIndex(UnlockSeason)) * 28 + Clamp(UnlockDay, 1, 28);
    }

    public static int GetSeasonIndex(string season)
    {
        return season.ToLower() switch
        {
            "spring" => 0,
            "summer" => 1,
            "fall" => 2,
            "winter" => 3,
            _ => 0
        };
    }

    /// <summary>
    /// Clamps the passed in value to be within <c>min</c>(inclusive) and <c>max</c>(inclusive).
    /// </summary>
    public static int Clamp(int num, int min, int max)
    {
        if (num < min) return min;
        return num > max ? max : num;
    }
}
