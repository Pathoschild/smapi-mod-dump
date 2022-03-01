/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Common.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

#endregion using directives

public static class KeybindListExtensions
{
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