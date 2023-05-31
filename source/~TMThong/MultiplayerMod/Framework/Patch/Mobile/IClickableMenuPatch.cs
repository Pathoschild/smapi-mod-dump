/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    public class IClickableMenuPatch : IPatch
    {
        public static IReflectedField<Rectangle> lastTextureBoxRectField;
        public IClickableMenuPatch()
        {
            lastTextureBoxRectField = ModUtilities.Helper.Reflection.GetField<Rectangle>(typeof(IClickableMenu), "lastTextureBoxRect");
        }

        public static xTile.Dimensions.Rectangle viewport
        {
            get
            {
                return Game1.uiViewport;
            }
        }

        public static void drawTextureBox(SpriteBatch b, Texture2D texture, Microsoft.Xna.Framework.Rectangle sourceRect, int x, int y, int width, int height, Color color, float scale = 1f, bool drawShadow = true, float draw_layer = -1f, bool ignoreBorder = false)
        {
            lastTextureBoxRectField.SetValue(new Microsoft.Xna.Framework.Rectangle(x, y, width, height));
            int num = sourceRect.Width / 3;
            float layerDepth = draw_layer - 0.03f;
            if (draw_layer < 0f)
            {
                draw_layer = 0.8f - (float)y * 1E-06f;
                layerDepth = 0.77f;
            }
            if (drawShadow && !ignoreBorder)
            {
                b.Draw(texture, new Vector2((float)(x - 8), (float)(y + 8)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X, sourceRect.Y, num, num)), Color.Black * 0.4f, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
                b.Draw(texture, new Vector2((float)(x + width - (int)((float)num * scale) - 8), (float)(y + 8)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X + sourceRect.Width - num, sourceRect.Y, num, num)), Color.Black * 0.4f, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
                b.Draw(texture, new Vector2((float)(x - 8), (float)(y + height - (int)((float)num * scale) + 8)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X, sourceRect.Y + sourceRect.Height - num, num, num)), Color.Black * 0.4f, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
                b.Draw(texture, new Vector2((float)(x + width - (int)((float)num * scale) - 8), (float)(y + height - (int)((float)num * scale) + 8)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X + sourceRect.Width - num, sourceRect.Y + sourceRect.Height - num, num, num)), Color.Black * 0.4f, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
                b.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + (int)((float)num * scale) - 8, y + 8, width - (int)((float)num * scale) * 2, (int)((float)num * scale)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X + num, sourceRect.Y, num, num)), Color.Black * 0.4f, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
                b.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + (int)((float)num * scale) - 8, y + height - (int)((float)num * scale) + 8, width - (int)((float)num * scale) * 2, (int)((float)num * scale)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X + num, sourceRect.Y + sourceRect.Height - num, num, num)), Color.Black * 0.4f, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
                b.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x - 8, y + (int)((float)num * scale) + 8, (int)((float)num * scale), height - (int)((float)num * scale) * 2), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X, num + sourceRect.Y, num, num)), Color.Black * 0.4f, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
                b.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + width - (int)((float)num * scale) - 8, y + (int)((float)num * scale) + 8, (int)((float)num * scale), height - (int)((float)num * scale) * 2), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X + sourceRect.Width - num, num + sourceRect.Y, num, num)), Color.Black * 0.4f, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
                b.Draw(texture, new Microsoft.Xna.Framework.Rectangle((int)((float)num * scale / 2f) + x - 8, (int)((float)num * scale / 2f) + y + 8, width - (int)((float)num * scale), height - (int)((float)num * scale)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num)), Color.Black * 0.4f, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
            }
            b.Draw(texture, new Microsoft.Xna.Framework.Rectangle((int)((float)num * scale) + x, (int)((float)num * scale) + y, width - (int)((float)num * scale * 2f), height - (int)((float)num * scale * 2f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num)), color, 0f, Vector2.Zero, SpriteEffects.None, draw_layer);
            if (!ignoreBorder)
            {
                b.Draw(texture, new Vector2((float)x, (float)y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X, sourceRect.Y, num, num)), color, 0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
                b.Draw(texture, new Vector2((float)(x + width - (int)((float)num * scale)), (float)y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X + sourceRect.Width - num, sourceRect.Y, num, num)), color, 0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
                b.Draw(texture, new Vector2((float)x, (float)(y + height - (int)((float)num * scale))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X, sourceRect.Y + sourceRect.Height - num, num, num)), color, 0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
                b.Draw(texture, new Vector2((float)(x + width - (int)((float)num * scale)), (float)(y + height - (int)((float)num * scale))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X + sourceRect.Width - num, sourceRect.Y + sourceRect.Height - num, num, num)), color, 0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
                b.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + (int)((float)num * scale), y, width - (int)((float)num * scale) * 2, (int)((float)num * scale)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X + num, sourceRect.Y, num, num)), color, 0f, Vector2.Zero, SpriteEffects.None, draw_layer);
                b.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + (int)((float)num * scale), y + height - (int)((float)num * scale), width - (int)((float)num * scale) * 2, (int)((float)num * scale)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X + num, sourceRect.Y + sourceRect.Height - num, num, num)), color, 0f, Vector2.Zero, SpriteEffects.None, draw_layer);
                b.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x, y + (int)((float)num * scale), (int)((float)num * scale), height - (int)((float)num * scale) * 2), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X, num + sourceRect.Y, num, num)), color, 0f, Vector2.Zero, SpriteEffects.None, draw_layer);
                b.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + width - (int)((float)num * scale), y + (int)((float)num * scale), (int)((float)num * scale), height - (int)((float)num * scale) * 2), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.X + sourceRect.Width - num, num + sourceRect.Y, num, num)), color, 0f, Vector2.Zero, SpriteEffects.None, draw_layer);
            }
        }

        public void Apply(Harmony harmony)
        {

        }
    }
}
