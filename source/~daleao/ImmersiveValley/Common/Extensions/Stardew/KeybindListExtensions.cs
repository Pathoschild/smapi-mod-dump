/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Extensions for the <see cref="KeybindList"/> class.</summary>
public static class KeybindListExtensions
{
    /// <summary>Determines whether a <see cref="KeybindList"/> shares any <see cref="Keybind"/> with another <see cref="KeybindList"/>.</summary>
    /// <param name="b">A <see cref="KeybindList"/> to compare with.</param>
    public static bool HasCommonKeybind(this KeybindList a, KeybindList b)
    {
        return (from keybindA in a.Keybinds
            from keybindB in b.Keybinds
            let buttonsA = new HashSet<SButton>(keybindA.Buttons)
            let buttonsB = new HashSet<SButton>(keybindB.Buttons)
            where buttonsA.SetEquals(buttonsB)
            select buttonsA).Any();
    }
}