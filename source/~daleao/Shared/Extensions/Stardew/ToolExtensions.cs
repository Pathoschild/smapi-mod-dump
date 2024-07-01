/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions.Collections;
using StardewValley.Enchantments;

#endregion using directives

/// <summary>Extensions for <see cref="Tool"/> class.</summary>
public static class ToolExtensions
{
    /// <summary>Determines whether the specified <paramref name="tool"/> contains any <see cref="BaseEnchantment"/> of the specified <paramref name="enchantmentTypes"/>.</summary>
    /// <param name="tool">The <see cref="Tool"/>.</param>
    /// <param name="enchantmentTypes">The candidate <see cref="BaseEnchantment"/> types to search for.</param>
    /// <returns><see langword="true"/> if the <paramref name="tool"/> contains at least one enchantment of the specified <paramref name="enchantmentTypes"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasAnyEnchantmentOf(this Tool tool, params Type[] enchantmentTypes)
    {
        return enchantmentTypes.Any(t => tool.enchantments.ContainsType(t));
    }
}
