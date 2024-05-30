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
    /// <param name="Sprite">The sprite to display.</param>
    /// <param name="DisplayText">The display text for the item name and count.</param>
    /// <param name="Quality">The item quality that will be produced, if applicable.</param>
    internal record RecipeItemEntry(SpriteInfo? Sprite, string DisplayText, int? Quality);
}
