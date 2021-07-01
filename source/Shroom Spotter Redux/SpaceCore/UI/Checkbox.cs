/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace SpaceCore.UI
{
    public class Checkbox : Element
    {
        public Texture2D Texture { get; set; }
        public Rectangle CheckedTextureRect { get; set; }
        public Rectangle UncheckedTextureRect { get; set; }

        public Action<Element> Callback { get; set; }

        public bool Checked { get; set; } = true;

        public Checkbox()
        {
            Texture = Game1.mouseCursors;
            CheckedTextureRect = OptionsCheckbox.sourceRectChecked;
            UncheckedTextureRect = OptionsCheckbox.sourceRectUnchecked;
        }

        public override int Width => CheckedTextureRect.Width * 4;
        public override int Height => CheckedTextureRect.Height * 4;
        public override string ClickedSound => "drumkit6";

        public override void Update(bool hidden = false)
        {
            base.Update(hidden);

            if (Clicked && Callback != null)
            {
                Checked = !Checked;
                Callback.Invoke(this);
            }
        }

        public override void Draw(SpriteBatch b)
        {
            b.Draw(Texture, Position, Checked ? CheckedTextureRect : UncheckedTextureRect, Color.White, 0, Vector2.Zero, 4, SpriteEffects.None, 0);
            Game1.activeClickableMenu?.drawMouse(b);
        }
    }
}
