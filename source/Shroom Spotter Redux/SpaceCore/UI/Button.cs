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

namespace SpaceCore.UI
{
    public class Button : Element
    {
        public Texture2D Texture { get; set; }
        public Rectangle IdleTextureRect { get; set; }
        public Rectangle HoverTextureRect { get; set; }

        public Action<Element> Callback { get; set; }

        private float scale = 1f;

        public Button(Texture2D tex)
        {
            Texture = tex;
            IdleTextureRect = new Rectangle(0, 0, tex.Width / 2, tex.Height);
            HoverTextureRect = new Rectangle(tex.Width / 2, 0, tex.Width / 2, tex.Height);
        }

        public override int Width => IdleTextureRect.Width;
        public override int Height => IdleTextureRect.Height;
        public override string HoveredSound => "Cowboy_Footstep";

        public override void Update(bool hidden = false)
        {
            base.Update(hidden);

            scale = Hover ? Math.Min(scale + 0.013f, 1.083f) : Math.Max(scale - 0.013f, 1f);

            if (Clicked && Callback != null)
                Callback.Invoke(this);
        }

        public override void Draw(SpriteBatch b)
        {
            Vector2 origin = new Vector2(Texture.Width / 4f, Texture.Height / 2f);
            b.Draw(Texture, Position + origin, Hover ? HoverTextureRect : IdleTextureRect, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            Game1.activeClickableMenu?.drawMouse(b);
        }
    }
}
