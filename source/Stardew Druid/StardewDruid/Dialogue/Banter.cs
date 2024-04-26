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

                Utility.drawTextWithShadow(b, useName, Game1.smallFont, vector, color * transparency, 1.4f, 1f, -1, -1, transparency);

                Vector2 vectortwo = new Vector2(titleSafeArea.Center.X + ((messOffset + nameOffset + 16) / 2) - messOffset, titleSafeArea.Bottom - (height * 2.5f) - 160);

                Utility.drawTextWithShadow(b, banterText, Game1.smallFont, vectortwo, Color.White * transparency, 1.25f, 1f, -1, -1, transparency);
            
            }

            if (banterType == 1)
            {

                float messOffset = Game1.smallFont.MeasureString(banterText).X * 1.25f;

                float height = (4000 - timeLeft) / 100;

                Vector2 vectortwo = new Vector2(titleSafeArea.Center.X - (messOffset / 2), titleSafeArea.Bottom - (height * 2.5f) - 160);

                Utility.drawTextWithShadow(b, banterText, Game1.smallFont, vectortwo, color * transparency, 1.25f, 1f, -1, -1, transparency);

            }

        }

    }

}
