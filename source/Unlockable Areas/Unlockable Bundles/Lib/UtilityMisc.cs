/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;

namespace Unlockable_Bundles.Lib
{
    public class UtilityMisc
    {
        public static Mod Mod;
        public static IMonitor Monitor;
        public static IModHelper Helper;

        public static Texture2D TinyLetters;
        public static Texture2D MoneyTexture;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = ModEntry._Monitor;
            Helper = ModEntry._Helper;

            TinyLetters = Helper.ModContent.Load<Texture2D>("assets/TinyLetters.png");
        }

        public static Texture2D createSubTexture(Texture2D src, Rectangle rect)
        {
            Texture2D tex = new Texture2D(Game1.graphics.GraphicsDevice, rect.Width, rect.Height);
            int count = rect.Width * rect.Height;
            Color[] data = new Color[count];
            src.GetData(0, rect, data, 0, count);
            tex.SetData(data);
            return tex;
        }

        public static void drawMoneyKiloFormat(SpriteBatch b, int num, int x, int y, Color color, float scale = 4f)
        {
            if (MoneyTexture == null)
                MoneyTexture = createSubTexture(Game1.mouseCursors, new Rectangle(280, 412, 15, 14));

            b.Draw(MoneyTexture, new Vector2(x, y), new Rectangle(0, 0, 15, 14), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.9f);
            UtilityMisc.drawKiloFormat(b, num, x, y, color, scale * 0.75f);
        }

        public static void drawKiloFormat(SpriteBatch b, int num, int x, int y, Color color, float scale = 3f)
        {
            int offset = 0;

            if (num >= 1000000) {
                num = num / 1000000;
                offset = 7;
            } else if (num >= 1000) {
                num = num / 1000;
                offset = 5;
            }

            Utility.drawTinyDigits(num, b, new Vector2(x, y) + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(num, scale)) + 3f - offset * 3f, 64f - 18f + 1f), scale, 1f, color);

            if (offset != 0)
                b.Draw(TinyLetters, new Vector2(x, y) + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(num, scale)) + 3f + offset * 3f, 64f - 18f + 1f), new Rectangle(offset == 5 ? 0 : 5, 0, offset, 7), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        }
    }
}
