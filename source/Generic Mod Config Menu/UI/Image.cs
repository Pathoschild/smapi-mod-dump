/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/GenericModConfigMenu
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace GenericModConfigMenu.UI
{
    public class Image : Element
    {
        public Texture2D Texture { get; set; }
        public Rectangle? TextureRect { get; set; }
        public float Scale { get; set; } = 1;

        public Action<Element> Callback { get; set; }

        public override int Width => (int) GetActualSize().X;
        public override int Height => (int) GetActualSize().Y;
        public override string HoveredSound => (Callback != null) ? "shiny4" : null;

        public override void Update(bool hidden = false)
        {
            base.Update(hidden);            

            if (Clicked && Callback != null)
                Callback.Invoke(this);
        }

        public Vector2 GetActualSize()
        {
            if ( TextureRect.HasValue )
                return new Vector2( TextureRect.Value.Width, TextureRect.Value.Height ) * Scale;
            else
                return new Vector2( Texture.Width, Texture.Height ) * Scale;
        }

        public override void Draw(SpriteBatch b)
        {
            b.Draw( Texture, Position, TextureRect, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 1 );
        }
    }
}
