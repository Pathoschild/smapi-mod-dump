/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Extensions;

namespace StardewDruid.Dialogue
{
    public class ConsumeEdible : StardewValley.HUDMessage
    {

        public StardewValley.Item edible;

        public ConsumeEdible(string message, StardewValley.Item Item)
          : base(message)
        {

            edible = Item;

        }

        public override void draw(SpriteBatch b, int i, ref int heightUsed)
        {

            Rectangle titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();

            int num2 = 112;

            Vector2 vector = new Vector2(titleSafeArea.Left + 16, titleSafeArea.Bottom - num2 - heightUsed - 64);

            heightUsed += num2;

            if (Game1.isOutdoorMapSmallerThanViewport())
            {
                vector.X = Math.Max(titleSafeArea.Left + 16, -Game1.uiViewport.X + 16);
            }

            if (Game1.uiViewport.Width < 1400)
            {
                vector.Y -= 48f;
            }

            b.Draw(Game1.mouseCursors, vector, (messageSubject is StardewValley.Object @object && @object.sellToStorePrice(-1L) > 500) ? new Rectangle(163, 399, 26, 24) : new Rectangle(293, 360, 26, 24), Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            
            float x = Game1.smallFont.MeasureString(message).X;
            
            b.Draw(Game1.mouseCursors, new Vector2(vector.X + 104f, vector.Y), new Rectangle(319, 360, 1, 24), Color.White * transparency, 0f, Vector2.Zero, new Vector2(x, 4f), SpriteEffects.None, 1f);
            
            b.Draw(Game1.mouseCursors, new Vector2(vector.X + 104f + x, vector.Y), new Rectangle(323, 360, 6, 24), Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            
            vector.X += 16f;
            
            vector.Y += 16f;
            
            edible.drawInMenu(b, vector, 1f + Math.Max(0f, (timeLeft - 3000f) / 900f), transparency, 1f, StackDrawType.Hide);

            b.Draw(Game1.mouseCursors, vector + new Vector2(24,24) + new Vector2(8f, 8f) * 4f, new Rectangle(0, 411, 16, 16), Color.White * transparency, 0f, new Vector2(8f, 8f), 3f + Math.Max(0f, (timeLeft - 3000f) / 900f), SpriteEffects.None, 1f);

            vector.X += 51f;

            vector.Y += 51f;

            vector.X += 32f;

            vector.Y -= 33f;

            Utility.drawTextWithShadow(b, message, Game1.smallFont, vector, Game1.textColor * transparency, 1f, 1f, -1, -1, transparency);

        }

    }

}
