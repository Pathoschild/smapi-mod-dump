/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace HighlightEmptyMachines.Framework;

/// <summary>
/// Enum to hold the possible Machine statuses.
/// </summary>
internal enum MachineStatus
{
    /// <summary>
    /// This machine is enabled in settings and can receive input.
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