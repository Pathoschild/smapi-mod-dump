/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using TehPers.FishingOverhaul.Extensions.Drawing;
using SObject = StardewValley.Object;

namespace TehPers.FishingOverhaul.Extensions
{
    internal static class DrawingExtensions
    {
        /// <summary>Draws a string with a shadow behind it.</summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="font">The font the text should use.</param>
        /// <param name="text">The string to draw.</param>
        /// <param name="position">The position of the string.</param>
        /// <param name="color">The color of the string. The shadow is black.</param>
        /// <param name="depth">The depth of the string.</param>
        /// <param name="shadowDepth">The depth of the shadow.</param>
        public static void DrawStringWithShadow(
            this SpriteBatch batch,
            SpriteFont font,
            string text,
            Vector2 position,
            Color color,
            float depth = 0F,
            float shadowDepth = 0.005F
        )
        {
            batch.DrawStringWithShadow(
                font,
                text,
                position,
                color,
                Color.Black,
                Vector2.One,
                depth,
                shadowDepth
            );
        }

        /// <summary>Draws a string with a shadow behind it.</summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="font">The font the text should use.</param>
        /// <param name="text">The string to draw.</param>
        /// <param name="position">The position of the string.</param>
        /// <param name="color">The color of the string. The shadow is black.</param>
        /// <param name="shadowColor">The color of the shadow.</param>
        /// <param name="scale">The amount to scale the size of the string by.</param>
        /// <param name="depth">The depth of the string.</param>
        /// <param name="shadowDepth">The depth of the shadow.</param>
        public static void DrawStringWithShadow(
            this SpriteBatch batch,
            SpriteFont font,
            string text,
            Vector2 position,
            Color color,
            Color shadowColor,
            Vector2 scale,
            float depth,
            float shadowDepth
        )
        {
            batch.DrawString(
                font,
                text,
                position + Vector2.One * Game1.pixelZoom * scale / 2f,
                shadowColor,
                0F,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                shadowDepth
            );
            batch.DrawString(
                font,
                text,
                position,
                color,
                0F,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                depth
            );
        }

        public static void DrawInMenuCorrected(
            this Item item,
            SpriteBatch spriteBatch,
            Vector2 location,
            float scaleSize,
            float transparency,
            float layerDepth,
            StackDrawType drawStackNumber,
            Color color,
            bool drawShadow,
            IDrawOrigin origin
        )
        {
            IDrawingProperties drawingProperties = item switch
            {
                // BigCraftable
                SObject { bigCraftable: { Value: true } } => new BigCraftableDrawingProperties(),
                // Boots
                Boots => new BootsDrawingProperties(),
                // Clothing
                Clothing => new ClothingDrawingProperties(),
                // Flooring
                Wallpaper { isFloor: { Value: true } } => new FlooringDrawingProperties(),
                // Wallpaper
                Wallpaper => new WallpaperDrawingProperties(),
                // Furniture
                Furniture { sourceRect: { Value: var sourceRect } } => new
                    FurnitureDrawingProperties(
                        new(sourceRect.Width, sourceRect.Height),
                        (sourceRect.Width / 16, sourceRect.Height / 16) switch
                        {
                            (>= 7, _) => 0.5f,
                            (>= 6, _) => 0.66f,
                            (>= 5, _) => 0.75f,
                            (_, >= 5) => 0.8f,
                            (_, >= 3) => 1f,
                            (<= 2, _) => 2f,
                            (<= 4, _) => 1f,
                        }
                    ),
                // Hat
                Hat => new HatDrawingProperties(),
                // Object
                SObject => new ObjectDrawingProperties(),
                // Ring
                Ring => new RingDrawingProperties(),
                // Weapon
                MeleeWeapon { type: { Value: var type } } weapon =>
                    new MeleeWeaponDrawingProperties(type, weapon.isScythe()),
                // Tool
                Tool => new ToolDrawingProperties(),
                _ => throw new ArgumentException(
                    $"Unknown drawing properties for {item} ({item.GetType().FullName})",
                    nameof(item)
                ),
            };

            item.DrawInMenuCorrected(
                spriteBatch,
                location,
                scaleSize,
                transparency,
                layerDepth,
                drawStackNumber,
                color,
                drawShadow,
                origin,
                drawingProperties
            );
        }

        public static void DrawInMenuCorrected(
            this Item item,
            SpriteBatch spriteBatch,
            Vector2 location,
            float scaleSize,
            float transparency,
            float layerDepth,
            StackDrawType drawStackNumber,
            Color color,
            bool drawShadow,
            IDrawOrigin origin,
            IDrawingProperties drawingProperties
        )
        {
            var realOrigin = drawingProperties.Origin(scaleSize);
            var realScale = drawingProperties.RealScale(scaleSize);
            var realOffset = drawingProperties.Offset(scaleSize);
            var realSize = drawingProperties.SourceSize * realScale;
            var positionCorrection =
                -realOffset + realOrigin * realScale + origin.GetTranslation(realSize);
            item.drawInMenu(
                spriteBatch,
                location + positionCorrection,
                scaleSize,
                transparency,
                layerDepth,
                drawStackNumber,
                color,
                drawShadow
            );
        }
    }
}