using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace AutoGrabberMod.UserInterfaces
{
    public class OptionsCheckbox : OptionsElement
    {
        public const int pixelsWide = 9;

        private readonly Action<bool> SetValue;

        public bool isChecked;

        public static Rectangle sourceRectUnchecked = new Rectangle(227, 425, 9, 9);

        public static Rectangle sourceRectChecked = new Rectangle(236, 425, 9, 9);

        public OptionsCheckbox(string label, bool initialValue, Action<bool> setValue) : base(label, -1, -1, 36, 36)
        {
            isChecked = initialValue;
            SetValue = setValue;
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (!base.greyedOut)
            {
                Game1.playSound("drumkit6");
                base.receiveLeftClick(x, y);
                this.isChecked = !this.isChecked;
                this.SetValue(this.isChecked);
            }
        }

        public override void draw(SpriteBatch b, int slotX, int slotY)
        {
            b.Draw(
                Game1.mouseCursors,
                new Vector2((float)(slotX + base.bounds.X + 10), (float)(slotY + base.bounds.Y + 5)),
                this.isChecked ? OptionsCheckbox.sourceRectChecked : OptionsCheckbox.sourceRectUnchecked,
                Color.White * (base.greyedOut ? 0.33f : 1f),
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                0.4f
            );

            base.draw(b, slotX + 10, slotY);
        }
    }
}
