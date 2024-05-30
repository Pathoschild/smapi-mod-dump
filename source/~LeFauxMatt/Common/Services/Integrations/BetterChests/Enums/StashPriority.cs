/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Services.Integrations.BetterChests;
#else
namespace StardewMods.Common.Services.Integrations.BetterChests;
#endif

using NetEscapades.EnumGenerators;

/// <summary>The possible values for Stash to Chest Priority.</summary>
[EnumExtensions]
public enum StashPriority
{
    /// <summary>Represents the default priority.</summary>
    Default = 0,

    /// <summary>Represents the lowest priority.</summary>
    Lowest = -3,

    /// <summary>Represents a lower priority.</summary>
    Lower = -2,

    /// <summary>Represents a low priority.</summary>
    Low = -1,

    /// <summary>Represents a high priority.</summary>
    High = 1,

    /// <summary>Represents a higher priority.</summary>
    Higher = 2,

    /// <summary>Represents the highest priority.</summary>
    Highest = 3,
}