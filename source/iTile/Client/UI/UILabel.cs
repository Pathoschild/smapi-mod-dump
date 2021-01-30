/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using iTile.Client.UI.Framework;
using StardewValley;
using iTile.Core;
using iTile.Utils;

namespace iTile.Client.UI
{
    public class UILabel : UIElement
    {
        private string text;
        private SpriteFont font;
        private float scale = 1;

        public float Scale
        {
            get => scale;
            set
            {
                scale = value;
                UpdateTransform();
            }
        }

        public string Text
        {
            get => text;
            set
            {
                text = value;
                UpdateTransform();
            }
        }

        public SpriteFont Font
            => font ?? AssetsManager.defaultFont;

        public UILabel(string name, Rectangle transform, string text, UIElement parent = null, SpriteFont font = null) : base(name, transform, parent)
        {
            if (text == null) text = string.Empty;
            Text = text;
            this.font = font ?? AssetsManager.defaultFont;
        }

        private void UpdateTransform()
        {
            Vector2 dim = Font.MeasureString(text) * scale;
            transform.Width = (int)dim.X;
            transform.Height = (int)dim.Y;
        }

        protected override void Draw()
        {
            if (!string.IsNullOrEmpty(text) && Font != null)
            {
                Game1.spriteBatch.DrawString(Font, text, new Vector2(transform.Location.X, transform.Location.Y), color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }
    }
}