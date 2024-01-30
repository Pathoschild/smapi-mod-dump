/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace ProfitCalculator.ui
{
    /// <summary>
    /// An object that can be drawn to the screen. With update and reset methods.
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// Draws the object.
        /// </summary>
        /// <param name="b"> The spritebatch to draw to. </param>
        void Draw(SpriteBatch b);

        /// <summary>
        /// Updates the state of the object.
        /// </summary>
        void Update();

        /// <summary>
        /// Resets the state of the object to its default state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Called when the game window size changes.
        /// </summary>
        void GameWindowSizeChanged();
    }
}