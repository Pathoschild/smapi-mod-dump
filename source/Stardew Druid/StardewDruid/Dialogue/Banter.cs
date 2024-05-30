/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;

namespace StardewDruid.Dialogue
{
    public class Banter : HUDMessage
    {

        public string banterText;

        public string banterName;

        public int banterType;

        public Microsoft.Xna.Framework.Color color;

        public Banter(string name, Color colour, string message, int type = 0)
          : base(message)
        {

            banterName = name;

            banterText = message;

            banterType = type;

            color = colour;

            timeLeft = 3000 + (type * 1000);
            
        }

        public override void draw(SpriteBatch b, int i, ref int heightUsed)
        {

            Rectangle titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();

            if (banterType == 0)
            {

                string useName = banterName + ":";

                float nameOffset = Game1.smallFont.MeasureString(useName).X * 1.4f;

                float messOffset = Game1.smallFont.MeasureString(banterText).X * 1.25f;

                float height = timeLeft / 100;

                Vector2 vector = new Vector2(titleSafeArea.Center.X - ((messOffset + nameOffset + 16) / 2), titleSafeArea.Bottom - (height * 2.5f) - 162);

                float width = (messOffset + nameOffset + 32);

                Utility.DrawSquare(b, new((int)(titleSafeArea.Center.X - width / 2), (int)(titleSafeArea.Bottom - (height * 2.5) - 164), (int)width, 48), 2, color * 0.75f, color * 0.5f);

                b.DrawString(Game1.smallFont, useName, vector, Color.White * transparency, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 1f);

                Vector2 vectortwo = new Vector2(titleSafeArea.Center.X + ((messOffset + nameOffset + 16) / 2) - messOffset, titleSafeArea.Bottom - (height * 2.5f) - 160);

                b.DrawString(Game1.smallFont, banterText, vectortwo, Color.White * transparency, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 1f);

            }

            if (banterType == 1)
            {

                float messOffset = Game1.smallFont.MeasureString(banterText).X * 1.25f;

                float height = (4000 - timeLeft) / 100;

                float width = (messOffset + 32);

                Utility.DrawSquare(b, new((int)(titleSafeArea.Center.X - width / 2), (int)(titleSafeArea.Bottom - (height * 2.5) - 164), (int)width, 48), 1, color * 0.75f, color * 0.5f);

                Vector2 vectortwo = new Vector2(titleSafeArea.Center.X - (messOffset / 2), titleSafeArea.Bottom - (height * 2.5f) - 160);

                b.DrawString(Game1.smallFont, banterText, vectortwo, Color.White * transparency, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 1f);

            }

        }

    }

}
