/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using Pathoschild.Stardew.Common;

namespace Pathoschild.Stardew.LookupAnything.Framework.Fields.Models
{
    /// <summary>An input or output item for a recipe model.</summary>
    internal class RecipeItemEntry
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The sprite to display.</summary>
        public SpriteInfo Sprite { get; }

        /// <summary>The display text for the item name and count.</summary>
        public string DisplayText { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="sprite">The sprite to display.</param>
        /// <param name="displayText">The display text for the item name and count.</param>
        public RecipeItemEntry(SpriteInfo sprite, string displayText)
        {
            this.Sprite = sprite;
            this.DisplayText = displayText;
        }
    }
}
