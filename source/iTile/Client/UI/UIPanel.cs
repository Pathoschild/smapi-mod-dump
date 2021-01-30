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

namespace iTile.Client.UI
{
    public class UIPanel : UIMovable
    {
        public Texture2D texture;
        public Rectangle source = Rectangle.Empty;

        public UIPanel(string name, Rectangle transform, UIElement parent = null, Texture2D texture = null) : base(name, transform, parent)
        {
            this.texture = texture;
            processClicks = false;
            canBeMoved = false;
        }

        protected override void Draw()
        {
            if (texture != null)
            {
                if (source != Rectangle.Empty)
                {
                    Game1.spriteBatch.Draw(texture, transform, source, color);
                }
                else
                {
                    Game1.spriteBatch.Draw(texture, transform, color);
                }
            }
        }
    }
}
