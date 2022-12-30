/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="MeleeWeapon"/> class.</summary>
public static class MeleeWeaponExtensions
{
    /// <summary>Checks whether the <paramref name="weapon"/> is a dagger.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/> is a dagger, otherwise <see langword="false"/>.</returns>
    public static bool IsDagger(this MeleeWeapon weapon)
    {
        return weapon.type.Value == MeleeWeapon.dagger;
    }

    /// <summary>Checks whether the <paramref name="weapon"/> is a club.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/> is a club, otherwise <see langword="false"/>.</returns>
    public static bool IsClub(this MeleeWeapon weapon)
    {
        return weapon.type.Value == MeleeWeapon.club;
    }
}
