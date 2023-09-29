/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Enums;

#region using directives

using StardewValley.Tools;

#endregion using directives

/// <summary>The option with which to refresh a <see cref="MeleeWeapon"/>'s damage stats.</summary>
internal enum WeaponRefreshOption
{
    /// <summary>Restores the <see cref="MeleeWeapon"/>'s initial stats if available.</summary>
    Initial,

    /// <summary>Forcefully set the <see cref="MeleeWeapon"/>'s stats to the default values from the game's data.</summary>
    FromData,

    /// <summary>Forcefully randomizes the <see cref="MeleeWeapon"/>'s stats according the player's current progression.</summary>
    Randomized,
}
