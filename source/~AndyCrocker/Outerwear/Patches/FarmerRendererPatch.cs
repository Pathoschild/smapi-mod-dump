/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Collections.Generic;
using System.Reflection;

namespace Outerwear.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="FarmerRenderer"/> class.</summary>
    internal static class FarmerRendererPatch
    {
        /*********
        ** Fields
        *********/
        /// <summary>The source rectangle for each type of outerwear for each facing direction.</summary>
        private static readonly Dictionary<OuterwearType, List<Rectangle>> SourceRectangles = new Dictionary<OuterwearType, List<Rectangle>>
        {
            [OuterwearType.Shirt] = new List<Rectangle> {
                new Rectangle(0, 24, 8, 8),
                new Rectangle(0, 8, 8, 8),
                new Rectangle(0, 0, 8, 8),
                new Rectangle(0, 16, 8, 8)
            },
            [OuterwearType.Accessory] = new List<Rectangle> {
                new Rectangle(0, 0, 0, 0),
                new Rectangle(0, 16, 16, 16),
                new Rectangle(0, 0, 16, 16),
                new Rectangle(0, 16, 16, 16)
            },
            [OuterwearType.Hair] = new List<Rectangle> {
                new Rectangle(0, 62, 16, 15),
                new Rectangle(0, 32, 16, 15),
                new Rectangle(0, 0, 16, 15),
                new Rectangle(0, 32, 16, 15)
            },
            [OuterwearType.Hat] = new List<Rectangle> {
                new Rectangle(0, 60, 20, 20),
                new Rectangle(0, 20, 20, 20),
                new Rectangle(0, 0, 20, 20),
                new Rectangle(0, 40, 20, 20)
            }
        };

        /// <summary>Whether a sprite is flipped horizontally for each type of outerwear for each facing direction.</summary>
        private static readonly Dictionary<OuterwearType, List<bool>> IsFlipped = new Dictionary<OuterwearType, List<bool>>
        {
            [OuterwearType.Accessory] = new List<bool> { false, false, false, true },
            [OuterwearType.Hair] = new List<bool> { false, false, false, true }
        };


        /*********
        ** Internal Methods
        *********/
        /// <summary>The post fix for the <see cref="FarmerRenderer.draw(SpriteBatch, FarmerSprite.AnimationFrame, int, Rectangle, Vector2, Vector2, float, int, Color, float, float, Farmer)"/> method.</summary>
        /// <param name="b">The sprite batch to draw the outerwear to.</param>
        /// <param name="animationFrame">The animation frame of the farmer.</param>
        /// <param name="currentFrame">The current frame of the farmer.</param>
        /// <param name="sourceRect">The source rectangle of the farmer.</param>
        /// <param name="position">The position of the farmer.</param>
        /// <param name="origin">The origin of the farmer.</param>
        /// <param name="layerDepth">The layer depth of the farmer.</param>
        /// <param name="facingDirection">The facing direction of the farmer.</param>
        /// <param name="overrideColor">The override colour of the farmer.</param>
        /// <param name="rotation">The rotation of the farmer.</param>
        /// <param name="scale">The scale of the farmer.</param>
        /// <param name="who">The farmer being drawn.</param>
        /// <param name="__instance">The current <see cref="FarmerRenderer"/> instance being patched.</param>
        /// <remarks>This is used to draw the outerwear on the farmer.</remarks>
        internal static void DrawPostFix(SpriteBatch b, FarmerSprite.AnimationFrame animationFrame, int currentFrame, Rectangle sourceRect, Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation, float scale, Farmer who, FarmerRenderer __instance)
        {
            // ensure farmer being drawn is wearing some outerwear
            var objectId = ModEntry.Instance.Api.GetEquippedOuterwearId(who);
            if (objectId == -1)
                return;

            // get the outerwear with the specified objectId
            var outerwearData = ModEntry.Instance.Api.GetOuterwearData(objectId);
            if (outerwearData == null)
                return;

            // retrieve private members
            var positionOffset = (Vector2)typeof(FarmerRenderer).GetField("positionOffset", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var rotationAdjustment = (Vector2)typeof(FarmerRenderer).GetField("rotationAdjustment", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            // get outerwear type dependant values
            var drawPosition = Vector2.Zero;
            var drawSourceRectangle = sourceRect;
            var drawEffects = SpriteEffects.None;
            var drawLayerDepth = layerDepth;

            switch (outerwearData.Type)
            {
                case OuterwearType.Shirt:
                    drawPosition = position + origin + positionOffset + new Vector2(16 * scale + FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + __instance.heightOffset * scale);
                    drawSourceRectangle = SourceRectangles[OuterwearType.Shirt][facingDirection];
                    drawLayerDepth = layerDepth + 2.81E-07f;
                    break;
                case OuterwearType.Accessory:
                    drawPosition = position + origin + positionOffset + rotationAdjustment + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 4 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + __instance.heightOffset);
                    drawSourceRectangle = SourceRectangles[OuterwearType.Accessory][facingDirection];
                    drawEffects = IsFlipped[OuterwearType.Accessory][facingDirection] ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    drawLayerDepth = layerDepth + 2.9E-05f;
                    break;
                case OuterwearType.Hair:
                    drawPosition = position + origin + positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + 4 + (who.IsMale ? -4 : 0));
                    drawSourceRectangle = SourceRectangles[OuterwearType.Hair][facingDirection];
                    drawEffects = IsFlipped[OuterwearType.Hair][facingDirection] ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    drawLayerDepth = layerDepth + 2.21E-05f;
                    break;
                case OuterwearType.Hat:
                    drawPosition = position + origin + positionOffset + new Vector2(-8 + (who.FarmerSprite.CurrentAnimationFrame.flip ? -1 : 1) * FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, -16 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + 4 + __instance.heightOffset);
                    drawSourceRectangle = SourceRectangles[OuterwearType.Hat][facingDirection];
                    drawLayerDepth = layerDepth + 3.91E-05f;
                    break;
                case OuterwearType.Pants:
                    drawPosition = position + origin + positionOffset;
                    drawSourceRectangle = sourceRect;
                    drawEffects = animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    drawLayerDepth = layerDepth + (who.FarmerSprite.CurrentAnimationFrame.frame == 5 ? 9.9E-04f : 9.9E-08f);
                    break;
            }

            // draw outerwear
            b.Draw(
                texture: outerwearData.EquippedTexture,
                position: drawPosition,
                sourceRectangle: drawSourceRectangle,
                color: Color.White,
                rotation: rotation,
                origin: origin,
                scale: scale * 4,
                effects: drawEffects,
                layerDepth: drawLayerDepth
            );
        }
    }
}
