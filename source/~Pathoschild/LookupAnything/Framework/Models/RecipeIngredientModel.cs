/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using StardewValley;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.LookupAnything.Framework.Models
{
    /// <summary>Metadata for an ingredient in a machine recipe.</summary>
    internal class RecipeIngredientModel
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The unique item ID or comma-separated context tags that can be used for this ingredient slot, or <c>null</c> if it's fully based on <see cref="InputContextTags"/>.</summary>
        public string? InputId { get; }

        /// <summary>The context tags which must be matched for this ingredient slot.</summary>
        public string[] InputContextTags { get; }

        /// <summary>The number required.</summary>
        public int Count { get; }

        /// <summary>The <see cref="SObject.preserve"/> value to match (or <c>null</c> to ignore it).</summary>
        public SObject.PreserveType? PreserveType { get; }

        /// <summary>The <see cref="SObject.preservedParentSheetIndex"/> value to match (or <c>null</c> to ignore it).</summary>
        public string? PreservedItemId { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="inputId">The unique item ID that can be used for this ingredient slot.</param>
        /// <param name="count">The number required.</param>
        /// <param name="inputContextTags">The context tags which must be matched for this ingredient slot.</param>
        /// <param name="preserveType">The <see cref="SObject.preserve"/> value to match (or <c>null</c> to ignore it).</param>
        /// <param name="preservedItemId">The <see cref="SObject.preservedParentSheetIndex"/> value to match (or <c>null</c> to ignore it).</param>
        public RecipeIngredientModel(string? inputId, int count, string[]? inputContextTags = null, SObject.PreserveType? preserveType = null, string? preservedItemId = null)
        {
            this.InputId = inputId;
            this.InputContextTags = inputContextTags ?? Array.Empty<string>();
            this.Count = count;
            this.PreserveType = preserveType;
            this.PreservedItemId = preservedItemId;
        }

        /// <summary>Get whether the ingredient matches a given item.</summary>
        /// <param name="item">The item to check.</param>
        public bool Matches(Item? item)
        {
            // ignore if null
            if (item == null)
                return false;

            // item fields
            bool matchesId =
                (this.InputId != null || this.InputContextTags.Length > 0)
                && (
                    this.InputId == null
                    || this.InputId == item.Category.ToString()
                    || this.InputId == item.ItemId
                    || this.InputId == item.QualifiedItemId
                )
                && (
                    this.InputContextTags.Length == 0
                    || ItemContextTagManager.DoAllTagsMatch(this.InputContextTags, item.GetContextTags())
                );
            if (!matchesId)
                return false;

            // object fields
            if (this.PreservedItemId != null || this.PreserveType != null)
            {
                if (item is not SObject obj)
                    return false;
                if (this.PreservedItemId != null && this.PreservedItemId != obj.preservedParentSheetIndex.Value)
                    return false;
                if (this.PreserveType != null && this.PreserveType != obj.preserve.Value)
                    return false;
            }

            return true;
        }
    }
}
