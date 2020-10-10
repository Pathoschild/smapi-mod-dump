/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AxesOfEvil/SV_DeliveryService
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace DeliveryService.Menus.Components
{
    /// <summary>An input control which represents a boolean value.</summary>
    internal class Checkbox
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The underlying value.</summary>
        public bool Value { get; set; }

        /// <summary>The X position of the rendered textbox.</summary>
        public int X { get; set; }

        /// <summary>The Y position of the rendered textbox.</summary>
        public int Y { get; set; }

        /// <summary>The width of the rendered textbox.</summary>
        public int Width { get; set; } = Sprites.Icons.EmptyCheckbox.Width * Game1.pixelZoom;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="value">The initial value.</param>
        public Checkbox(bool value = false)
        {
            this.Value = value;
        }

        /// <summary>Toggle the checkbox value.</summary>
        public void Toggle()
        {
            this.Value = !this.Value;
        }

        /// <summary>Get the checkbox's bounds on the screen.</summary>
        public Rectangle GetBounds()
        {
            return new Rectangle(this.X, this.Y, this.Width, this.Width);
        }

        /// <summary>Draw the checkbox to the screen.</summary>
        /// <param name="batch">The sprite batch.</param>
        public void Draw(SpriteBatch batch)
        {
            float scale = this.Width / (float)Sprites.Icons.FilledCheckbox.Width;
            batch.Draw(Sprites.Icons.Sheet, new Vector2(this.X, this.Y), this.Value ? Sprites.Icons.FilledCheckbox : Sprites.Icons.EmptyCheckbox, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
        }
    }
}