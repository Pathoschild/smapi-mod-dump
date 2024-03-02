/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace SAML.Utilities
{
    public interface IElement : INotifyPropertyChanged
    {
        /// <summary>
        /// The name of the current <see cref="IElement"/>
        /// </summary>
        /// <remarks>
        /// Currently unused
        /// </remarks>
        string Name { get; set; }

        /// <summary>
        /// The horizontal position at which this <see cref="IElement"/> appears on screen
        /// </summary>
        int X { get; set; }

        /// <summary>
        /// The vertical position at which this <see cref="IElement"/> appears on screen
        /// </summary>
        int Y { get; set; }

        /// <summary>
        /// The width of this <see cref="IElement"/>
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// The height of this <see cref="IElement"/>
        /// </summary>
        int Height { get; set; }

        /// <summary>
        /// Changes the X value of this <see cref="IElement"/> based on it's parent element
        /// </summary>
        HorizontalAlignment HorizontalAlignment { get; set; }

        /// <summary>
        /// Changes the Y value of this <see cref="IElement"/> based on it's parent element
        /// </summary>
        VerticalAlignment VerticalAlignment { get; set; }

        /// <summary>
        /// Draw this <see cref="IElement"/> to the screen
        /// </summary>
        /// <param name="b">Framework helper class to draw text and sprites to the screen</param>
        void Draw(SpriteBatch b);
    }
}
