/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cilekli-link/SDVMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkUI
{
    [HarmonyPatch(typeof(Utility))]
    [HarmonyPatch("drawTextWithShadow")]
    [HarmonyPatch(new Type[] { typeof(SpriteBatch), typeof(string), typeof(SpriteFont), typeof(Vector2), typeof(Color), typeof(float), typeof(float), typeof(int), typeof(int), typeof(float), typeof(int) })]
    class ShadowPatch
    {
        static bool Prefix(SpriteBatch b, string text, SpriteFont font, Vector2 position, Color color, float scale = 1f, float layerDepth = -1f, int horizontalShadowOffset = -1, int verticalShadowOffset = -1, float shadowIntensity = 1f, int numShadows = 3)
        {
            if (layerDepth == -1f)
            {
                layerDepth = position.Y / 10000f;
            }
            bool longWords = Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ru || Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.de || Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ko;
            if (horizontalShadowOffset == -1)
            {
                horizontalShadowOffset = ((font.Equals(Game1.smallFont) | longWords) ? (-2) : (-3));
            }
            if (verticalShadowOffset == -1)
            {
                verticalShadowOffset = ((font.Equals(Game1.smallFont) | longWords) ? 2 : 3);
            }
            if (text == null)
            {
                text = "";
            }
            b.DrawString(font, text, position + new Vector2(horizontalShadowOffset, verticalShadowOffset), new Color(32, 32, 32) * shadowIntensity, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0001f);
            if (numShadows == 2)
            {
                b.DrawString(font, text, position + new Vector2(horizontalShadowOffset, 0f), new Color(32, 32, 32) * shadowIntensity, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0002f);
            }
            if (numShadows == 3)
            {
                b.DrawString(font, text, position + new Vector2(0f, verticalShadowOffset), new Color(32, 32, 32) * shadowIntensity, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0003f);
            }
            b.DrawString(font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
            return false;
        }
    }
    [HarmonyPatch(typeof(Utility))]
    [HarmonyPatch("drawTextWithShadow")]
    [HarmonyPatch(new Type[] { typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(Vector2), typeof(Color), typeof(float), typeof(float), typeof(int), typeof(int), typeof(float), typeof(int) })]
    class ShadowPatch2
    {
        static bool Prefix(SpriteBatch b, StringBuilder text, SpriteFont font, Vector2 position, Color color, float scale = 1f, float layerDepth = -1f, int horizontalShadowOffset = -1, int verticalShadowOffset = -1, float shadowIntensity = 1f, int numShadows = 3)
        {
            {
                if ((double)layerDepth == -1.0)
                    layerDepth = position.Y / 10000f;
                bool flag = Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ru || Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.de;
                if (horizontalShadowOffset == -1)
                    horizontalShadowOffset = font.Equals((object)Game1.smallFont) | flag ? -2 : -3;
                if (verticalShadowOffset == -1)
                    verticalShadowOffset = font.Equals((object)Game1.smallFont) | flag ? 2 : 3;
                if (text == null)
                    throw new ArgumentNullException(nameof(text));
                b.DrawString(font, text, position + new Vector2((float)horizontalShadowOffset, (float)verticalShadowOffset), new Color(32, 32, 32) * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0001f);
                if (numShadows == 2)
                    b.DrawString(font, text, position + new Vector2((float)horizontalShadowOffset, 0.0f), new Color(32, 32, 32) * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0002f);
                if (numShadows == 3)
                    b.DrawString(font, text, position + new Vector2(0.0f, (float)verticalShadowOffset), new Color(32, 32, 32) * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0003f);
                b.DrawString(font, text, position, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
                return false;
            }
        }
    }
}
