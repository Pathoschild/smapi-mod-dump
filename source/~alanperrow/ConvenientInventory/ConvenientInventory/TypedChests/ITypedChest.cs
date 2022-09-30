/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

namespace ConvenientInventory.TypedChests
{
    /// <summary>
    /// Interface for a wrapper for <see cref="StardewValley.Objects.Chest"/>, holding information about what type of chest this is.
    /// </summary>
    public interface ITypedChest
    {
        /// <summary>The actual chest which is being wrapped by this class.</summary>
        Chest Chest { get; }

        /// <summary>Draws <see cref="Chest"/> in the quick stack tooltip.</summary>
        /// <returns>The number of tooltip position indexes skipped while drawing (due to buildings occupying > 1 index).</returns>
        int DrawInToolTip(SpriteBatch spriteBatch, Point toolTipPosition, int posIndex);

        /// <summary>Determines whether this typed chest is that of a building, such as a Mill or Junimo Hut.</summary>
        /// <returns>Whether this chest type is that of a building.</returns>
        bool IsBuildingChestType();
    }
}
