/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Extensions;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.VirtualProperties;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="Tool"/> class.</summary>
internal static class ToolExtensions
{
    /// <summary>Invalidates the cached stats for the specified <see cref="MeleeWeapon"/> or <see cref="Slingshot"/>.</summary>
    /// <param name="tool">A <see cref="MeleeWeapon"/> or <see cref="Slingshot"/>.</param>
    internal static void Invalidate(this Tool tool)
    {
        switch (tool)
        {
            case MeleeWeapon weapon:
                MeleeWeapon_Stats.Invalidate(weapon);
                break;
            case Slingshot slingshot:
                Slingshot_Stats.Invalidate(slingshot);
                break;
        }
    }
}
