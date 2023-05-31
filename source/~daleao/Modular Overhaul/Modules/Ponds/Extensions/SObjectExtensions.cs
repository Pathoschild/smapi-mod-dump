/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Ponds.Extensions;

#region using directives

using DaLion.Shared.Extensions;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    /// <summary>Determines whether the <paramref name="obj"/> is a radioactive fish.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a mutant or radioactive fish species, otherwise <see langword="false"/>.</returns>
    internal static bool IsRadioactiveFish(this SObject obj)
    {
        return obj.Category == SObject.FishCategory && obj.Name.ContainsAnyOf("Mutant", "Radioactive");
    }

    /// <summary>Determines whether the <paramref name="obj"/> is a non-radioactive ore or ingot.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is either copper, iron, gold or iridium, otherwise <see langword="false"/>.</returns>
    internal static bool CanEnrich(this SObject obj)
    {
        return obj.ParentSheetIndex is SObject.copper or SObject.iron or SObject.gold or SObject.iridium
            or SObject.copperBar or SObject.ironBar or SObject.goldBar or SObject.iridiumBar;
    }
}
