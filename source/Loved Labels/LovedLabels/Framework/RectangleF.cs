/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AdvizeGH/LovedLabels
**
*************************************************/

namespace LovedLabels.Framework
{
    /// <summary>An simple rectangle that accepts float values for boundary checks.</summary>
    internal class RectangleF
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The X position.</summary>
        public float X { get; }

        /// <summary>The Y position.</summary>
        public float Y { get; }

        /// <summary>The rectangle width starting from the X position.</summary>
        public float Width { get; }

        /// <summary>The rectangle height starting from the Y position.</summary>
        public float Height { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="width">The rectangle width starting from the X position.</param>
        /// <param name="height">The rectangle height starting from the Y position.</param>
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>Get whether the given position is within the rectangle bounds.</summary>
        /// <param name="x">The X position to check.</param>
        /// <param name="y">The Y position to check.</param>
        public bool Contains(float x, float y)
        {
            return
                x >= this.X
                && x < this.X + this.Width
                && y >= this.Y
                && y < this.Y + this.Height;
        }
    }
}
