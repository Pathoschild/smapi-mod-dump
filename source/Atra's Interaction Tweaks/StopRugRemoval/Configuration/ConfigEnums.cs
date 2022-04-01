/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

namespace StopRugRemoval.Configuration;

/// <summary>
/// Whether or not to confirm bombs.
/// </summary>
[Flags]
public enum ConfirmBombEnum
{
    /// <summary>
    /// Always confirm bombs in this area.
    /// </summary>
    On = 0b11,

    /// <summary>
    /// Never confirm bombs in this area.
    /// </summary>
    Off = 0b00,

    /// <summary>
    /// Only confirm bombs in multiplayer.
    /// </summary>
    InMultiplayerOnly = 0b10,

    /// <summary>
    /// Confirm bombs except in multiplayer.
    /// </summary>
    NotInMultiplayer = 0b01,
}

/// <summary>
/// Whether or not the location should be considered a "safe location" when it comes to bombs.
/// </summary>
public enum IsSafeLocationEnum
{
    /// <summary>
    /// This location is always considered safe.
    /// </summary>
    Safe,

    /// <summary>
    /// This location is always considered dangerous.
    /// </summary>
    Dangerous,

    /// <summary>
    /// This location will be considered safe when there are no monsters there.
    /// </summary>
    Dynamic,
}