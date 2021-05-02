/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewValley;

namespace Revitalize.Framework
{
    /// <summary>Provides extension methods on <see cref="StackDrawType"/>.</summary>
    internal static class StackDrawTypeExtensions
    {
        /// <summary>Get whether stack numbers should be drawn for a given item.</summary>
        /// <param name="drawStackNumber">The stack draw mode.</param>
        /// <param name="item">The item being drawn.</param>
        public static bool ShouldDrawFor(this StackDrawType drawStackNumber, Item item)
        {
            return
                drawStackNumber == StackDrawType.Draw_OneInclusive
                || (drawStackNumber == StackDrawType.Draw && item.Stack > 1);
        }
    }
}
