/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons;

/// <summary>The hit steps of a <see cref="StardewValley.Tools.MeleeWeapon"/> combo.</summary>
public enum ComboHitStep
{
    /// <summary>Not currently attacking.</summary>
    Idle,

    /// <summary>The first hit of the combo.</summary>
    FirstHit,

    /// <summary>The second hit of the combo.</summary>
    SecondHit,

    /// <summary>The third hit of the combo.</summary>
    ThirdHit,

    /// <summary>The fourth hit of the combo.</summary>
    FourthHit,

    /// <summary>The infinity-th hit of the combo.</summary>
    Infinite = int.MaxValue,
}
