/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/Nightshade
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ichortower.ui
{
    public class Checkbox : Widget
    {
        public static string Sound = "drumkit6";
        public bool Value;

        public Checkbox(IClickableMenu parent, int x, int y, string name = "", bool Value = false)
            : base(parent, new Rectangle(x, y, 27, 27), name)
        {
            this.Value = Value;
        }

        public override void draw(SpriteBatch b)
        {
            Texture2D tex = Game1.mouseCursors;
            Rectangle sourceRect = new(this.Value ? 236 : 227, 425, 9, 9);
            Rectangle destRect = new(
                    (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X,
                    (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y,
                    this.Bounds.Width, this.Bounds.Height);
            b.Draw(tex, color: Color.White,
                    sourceRectangle: sourceRect,
                    destinationRectangle: destRect);
        }

        public override void click(int x, int y, bool playSound = true)
        {
            this.Value = !this.Value;
            if (playSound) {
                Game1.playSound(Checkbox.Sound);
            }
            if (this.parent is ShaderMenu m) {
                m.onChildChange(this);
            }
        }
    }
}
