/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Extensions;

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    /// <summary>Determines whether the <paramref name="object"/> can be harvested with a sickle.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/>'s category should be harvestable as defined by the player's config settings, otherwise <see langword="false"/>.</returns>
    internal static bool IsHarvestableForage(this SObject @object)
    {
        return (@object.Category is SObject.VegetableCategory or SObject.FruitsCategory &&
                ToolsModule.Config.Scythe.HarvestCrops) ||
               (@object.Category == SObject.flowersCategory && ToolsModule.Config.Scythe.HarvestFlowers) ||
               (@object.Category == SObject.GreensCategory && ToolsModule.Config.Scythe.HarvestForage);
    }
}
