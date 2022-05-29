/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace Pathoschild.Stardew.LookupAnything.Framework.Data
{
    /// <summary>Metadata for a recipe that can be crafted using a building.</summary>
    /// <param name="BuildingKey">The building key.</param>
    /// <param name="Ingredients">The items needed to craft the recipe (item ID => number needed).</param>
    /// <param name="ExceptIngredients">The ingredients which can't be used in this recipe (typically exceptions for a category ingredient).</param>
    /// <param name="Output">The item created by the recipe.</param>
    /// <param name="OutputCount">The number of items produced by the recipe (or <c>null</c> for the default).</param>
    internal record BuildingRecipeData(string BuildingKey, Dictionary<int, int> Ingredients, int[]? ExceptIngredients, int Output, int? OutputCount);
}
