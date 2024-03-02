/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.ItemTypeDefinitions;

namespace SAML.Utilities
{
    public static class DrawUtilities
    {
        /// <summary>
        /// An implementation of <see cref="Item.drawInMenu(SpriteBatch, Vector2, float, float, float, StardewValley.StackDrawType, Color, bool)"/> to use SAML's <see cref="StackDrawType"/>
        /// </summary>
        public static void DrawInMenu(this Item? item, SpriteBatch b, Vector2 position, float scale, float transparency, float rotation, Vector2 origin, SpriteEffects spriteEffects, float zIndex, StackDrawType stackDrawType, Color color, bool drawShadow)
        {
            if (item is null)
                return;

            if (item.IsRecipe)
            {
                transparency = .5f;
                scale *= .75f;
            }

            {
                if (drawShadow && (item is not Object o || !o.bigCraftable.Value) && item.QualifiedItemId != "(O)590") 
                    b.Draw(Game1.shadowTexture, position + new Vector2(32f, 48f), Game1.shadowTexture.Bounds, color * .5f, rotation, new Vector2(Game1.shadowTexture.Bounds.Center.X + origin.X, Game1.shadowTexture.Bounds.Center.Y + origin.Y), 3f, spriteEffects, zIndex - 0.0001f);
            }

            ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId);

            {
                if (item is Object o && o.bigCraftable.Value && scale > .2f)
                    scale /= 2f;
            }

            Rectangle sourceRect = dataOrErrorItem.GetSourceRect(spriteIndex: item.ParentSheetIndex);
            b.Draw(dataOrErrorItem.GetTexture(), position + new Vector2(32f), sourceRect, color * transparency, rotation, new Vector2(sourceRect.Width / 2 + origin.X, sourceRect.Height / 2 + origin.Y), 4f * scale, spriteEffects, zIndex);
            DrawItemDecoration(item, b, position, scale, transparency, rotation, origin, spriteEffects, zIndex, stackDrawType, color);
        }

        /// <summary>
        /// A method to draw the decoration for item's in a menu (e.g. quality star and stack numbers)
        /// </summary>
        public static void DrawItemDecoration(this Item? item, SpriteBatch b, Vector2 position, float scale, float transparency, float rotation, Vector2 origin, SpriteEffects spriteEffects, float zIndex, StackDrawType stackDrawType, Color color)
        {
            if (item is null)
                return;

            if (stackDrawType != StackDrawType.None)
            {
                bool stackFlag = (stackDrawType == StackDrawType.StackNumOnly || stackDrawType == StackDrawType.Both) && item.maximumStackSize() > 1 && item.Stack > 1 && scale > .3f && item.Stack < int.MaxValue && !item.IsRecipe;
                if (stackFlag)
                    Utility.drawTinyDigits(item.Stack, b, position + new Vector2(64f - Utility.getWidthOfTinyDigitString(item.Stack, 3f * scale) + 3f * scale, 64f - 18f * scale + 1f), 3f * scale, zIndex + .1f, color * transparency);
                if ((stackDrawType == StackDrawType.QualityOnly || stackDrawType == StackDrawType.Both) && item.Quality > 0)
                {
                    Rectangle sourceRect = (item.Quality < 4) ? new(338 + (item.Quality - 1) * 8, 400, 8, 8) : new(346, 392, 8, 8);
                    float textureBob = (item.Quality < 4) ? 0f : (((float)Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1f) * 0.05f);
                    b.Draw(Game1.mouseCursors, position + new Vector2(12f, 52f + textureBob), sourceRect, color * transparency, rotation, new Vector2(4f + origin.X, 4f + origin.Y), 3f * scale * (1f + textureBob), spriteEffects, zIndex + .1f);
                }
            }

            if (item.IsRecipe)
                b.Draw(Game1.objectSpriteSheet, position + new Vector2(16f), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 451, 16, 16), color, rotation, origin, 3f, spriteEffects, zIndex + .1f);
        }
    }
}
