/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace StardewValleyMods.CategorizeChests.Interface.Widgets
{
    /// <summary>
    /// A simple text element.
    /// </summary>
    class Label : Widget
    {
        private string _Text;
        public string Text
        {
            get => _Text;
            set
            {
                _Text = value;
                RecalculateDimensions();
            }
        }

        public readonly SpriteFont Font;
        public readonly Color Color;

        public Label(string text, Color color, SpriteFont font)
        {
            Font = font;
            Color = color;
            Text = text;

            RecalculateDimensions();
        }

        public Label(string text, Color color)
            : this(text, color, Game1.smallFont)
        {
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.DrawString(Font, Text, new Vector2(GlobalPosition.X, GlobalPosition.Y), Color);
        }

        private void RecalculateDimensions()
        {
            var measure = Font.MeasureString(Text);
            Width = (int) measure.X;
            Height = (int) measure.Y;
        }
    }
}