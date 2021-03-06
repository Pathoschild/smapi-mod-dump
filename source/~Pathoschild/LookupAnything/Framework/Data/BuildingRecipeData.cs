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
    internal class BuildingRecipeData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The building key.</summary>
        public string BuildingKey { get; set; }

        /// <summary>The items needed to craft the recipe (item ID => number needed).</summary>
        public IDictionary<int, int> Ingredients { get; set; }

        /// <summary>The ingredients which can't be used in this recipe (typically exceptions for a category ingredient).</summary>
        public int[] ExceptIngredients { get; set; }

        /// <summary>The item created by the recipe.</summary>
        public int Output { get; set; }

        /// <summary>The number of items produced by the recipe (or <c>null</c> for the default).</summary>
        public int? OutputCount { get; set; }
    }
}
