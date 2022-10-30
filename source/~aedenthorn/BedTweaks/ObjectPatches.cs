/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using Object = StardewValley.Object;

namespace BedTweaks
{
    public partial class ModEntry
    {
        public static bool Object_draw_Prefix(Object __instance, SpriteBatch spriteBatch, float alpha)
        {
            if (!(__instance is BedFurniture) || (__instance is BedFurniture && (__instance as BedFurniture)?.bedType != BedFurniture.BedType.Double))
                return true;

            if (!Config.EnableMod)
            {
                __instance.boundingBox.Width = 3 * 64;
                return true;
            }


            int bedWidth = Math.Max(Config.BedWidth, 3);
            NetVector2 drawPosition = SHelper.Reflection.GetField<NetVector2>(__instance as BedFurniture, "drawPosition").GetValue();
            __instance.boundingBox.Width = bedWidth * 64;

            Rectangle drawn_rect = (__instance as BedFurniture).sourceRect.Value;
            int third = drawn_rect.Width / 3;
            Rectangle drawn_first = new Rectangle(drawn_rect.X, drawn_rect.Y, third, drawn_rect.Height);
            Rectangle drawn_second = new Rectangle(drawn_rect.X + third, drawn_rect.Y, third, drawn_rect.Height);
            Rectangle drawn_third = new Rectangle(drawn_rect.X + third + third, drawn_rect.Y, third, drawn_rect.Height);
            Rectangle pillowRect = new Rectangle(drawn_rect.X + 6, drawn_rect.Y + 19, 15, 7);
            bool redrawPillows = Config.RedrawMiddlePillows && !__instance.Name.StartsWith("Modern ") && !__instance.Name.StartsWith("Tropical ");

            spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)), new Rectangle?(drawn_first), Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (__instance.boundingBox.Value.Top + 1) / 10000f);
            for (int i = 1; i < bedWidth - 1; i++)
            {
                spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)) + new Vector2(third * i * 4, 0), new Rectangle?(drawn_second), Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (__instance.boundingBox.Value.Top + 1) / 10000f);
                spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)) + new Vector2(third * i * 4, 0), new Rectangle?(pillowRect), Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (__instance.boundingBox.Value.Top + 2) / 10000f);
                if (redrawPillows && i < bedWidth - 2)
                {
                    spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)) + new Vector2(third * i * 4 + 36, 19 * 4), new Rectangle?(pillowRect), Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (__instance.boundingBox.Value.Top + 2) / 10000f);
                }
            }
            spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)) + new Vector2(third * (bedWidth - 1) * 4, 0), new Rectangle?(drawn_third), Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (__instance.boundingBox.Value.Top + 1) / 10000f);

            drawn_rect.X += drawn_rect.Width;
            drawn_first = new Rectangle(drawn_rect.X, drawn_rect.Y, third, drawn_rect.Height);
            drawn_second = new Rectangle(drawn_rect.X + third, drawn_rect.Y, third, drawn_rect.Height);
            drawn_third = new Rectangle(drawn_rect.X + third + third, drawn_rect.Y, third, drawn_rect.Height);

            int solidHeight = 41;

            Rectangle drawn_first_t = new Rectangle(drawn_rect.X, drawn_rect.Y + solidHeight, third, drawn_rect.Height - solidHeight);
            Rectangle drawn_second_t = new Rectangle(drawn_rect.X + third, drawn_rect.Y + solidHeight, third, drawn_rect.Height - solidHeight);
            Rectangle drawn_third_t = new Rectangle(drawn_rect.X + third * 2, drawn_rect.Y + solidHeight, third, drawn_rect.Height - solidHeight);

            float sheetAlpha = alpha;
            sheetAlpha *= Config.SheetTransparency;

            spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)), new Rectangle?(drawn_first), Color.White * sheetAlpha, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (__instance.boundingBox.Value.Bottom - 1) / 10000f);
            spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)) + new Vector2(0, solidHeight * 4), new Rectangle?(drawn_first_t), Color.White, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (__instance.boundingBox.Value.Bottom - 2) / 10000f);

            for (int i = 1; i < bedWidth - 1; i++)
            {
                spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)) + new Vector2(third * i * 4, 0), new Rectangle?(drawn_second), Color.White * sheetAlpha, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (__instance.boundingBox.Value.Bottom - 1) / 10000f);
                spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)) + new Vector2(0, solidHeight * 4) + new Vector2(third * i * 4, 0), new Rectangle?(drawn_second_t), Color.White, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (__instance.boundingBox.Value.Bottom - 1) / 10000f);
            }
            spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)) + new Vector2(third * (bedWidth - 1) * 4, 0), new Rectangle?(drawn_third), Color.White * sheetAlpha, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (__instance.boundingBox.Value.Bottom - 1) / 10000f);
            spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)) + new Vector2(0, solidHeight * 4) + new Vector2(third * (bedWidth - 1) * 4, 0), new Rectangle?(drawn_third_t), Color.White, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (__instance.boundingBox.Value.Bottom - 1) / 10000f);

            return false;

        }
    }
}