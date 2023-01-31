/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions.Collections;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for <see cref="Tool"/> class.</summary>
public static class ToolExtensions
{
    /// <summary>Determines whether the specified <paramref name="tool"/> is a scythe.</summary>
    /// <param name="tool">The <see cref="Tool"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="tool"/> is a scythe, otherwise <see langword="false"/>.</returns>
    public static bool IsScythe(this Tool tool)
    {
        return tool is MeleeWeapon weapon && weapon.isScythe();
    }

    /// <summary>Determines whether the specified <paramref name="tool"/> contains any enchantment of the specified <paramref name="enchantmentTypes"/>.</summary>
    /// <param name="tool">The <see cref="Tool"/>.</param>
    /// <param name="enchantmentTypes">The types of the enchantments to search for.</param>
    /// <returns><see langword="true"/> if the <paramref name="tool"/> contains at least one enchantment of the specified <paramref name="enchantmentTypes"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasAnyEnchantmentOf(this Tool tool, params Type[] enchantmentTypes)
    {
        return enchantmentTypes.Any(t => tool.enchantments.ContainsType(t));
    }
}
