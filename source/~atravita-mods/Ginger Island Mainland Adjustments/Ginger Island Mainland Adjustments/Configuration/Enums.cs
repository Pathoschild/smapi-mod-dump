/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace GingerIslandMainlandAdjustments.Configuration;

/// <summary>
/// Enum for day of week, for the settings.
/// </summary>
public enum DayOfWeek
{
    /// <summary>
    /// None.
    /// </summary>
    None,

    /// <summary>
    /// Monday.
    /// </summary>
    Monday,

    /// <summary>
    /// Tuesday.
    /// </summary>
    Tuesday,

    /// <summary>
    /// Wednesday.
    /// </summary>
    Wednesday,

    /// <summary>
    /// Thursday.
    /// </summary>
    Thursday,

    /// <summary>
    /// Friday.
    /// </summary>
    Friday,

    /// <summary>
    /// Saturday.
    /// </summary>
    Saturday,

    /// <summary>
    /// Sunday.
    /// </summary>
    Sunday,
}

/// <summary>
/// Whether or not NPCs should wear their beach outfits.
/// </summary>
public enum WearIslandClothing
{
    /// <summary>
    /// Follow game defaults for who wears island clothing.
    /// </summary>
    Default,

    /// <summary>
    /// Everyone, if they have the island outfit, should wear it.
    /// </summary>
    All,

    /// <summary>
    /// No one should wear island clothing.
    /// </summary>
    None,
}

/// <summary>
/// Whether to prioritize [season]_[day] and marriage_[season]_[day] over Ginger Island schedules.
/// </summary>
public enum ScheduleStrictness
{
    /// <summary>
    /// Default - follow exclusions.
    /// </summary>
    Default,

    /// <summary>
    /// Always prioritize a defined [season]_[day]/marriage_[season]_[day].
    /// </summary>
    Strict,

    /// <summary>
    /// Never prioritize a defined [season]_[day]/marriage_[season]_[day].
    /// </summary>
    Loose,
}

public enum VillagerExclusionOverride
{
    Yes,
    No,
    IfMarried,
}