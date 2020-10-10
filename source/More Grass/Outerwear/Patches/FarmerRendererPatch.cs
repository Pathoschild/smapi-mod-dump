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
using Outerwear.Models;
using StardewValley;
using System.Linq;
using System.Reflection;

namespace Outerwear.Patches
{
    internal class FarmerRendererPatch
    {
        internal static void DrawPostFix(SpriteBatch b, FarmerSprite.AnimationFrame animationFrame, int currentFrame, Rectangle sourceRect, Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation, float scale, Farmer who, FarmerRenderer __instance)
        {
            if (ModEntry.EquippedOuterwear == null)
                return;

            OuterwearData outerwearData = ModEntry.OuterwearData
                .Where(data => data.DisplayName == ModEntry.EquippedOuterwear.DisplayName)
                .FirstOrDefault();

            // get private member values
            Vector2 positionOffset = (Vector2)typeof(FarmerRenderer).GetField("positionOffset", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            positionOffset.Y -= 4;

            Rectangle? sourceRectangle = null;
            SpriteEffects spriteEffects = SpriteEffects.None;
            switch (facingDirection)
            {
                case 0:
                        positionOffset.Y += 4;
                        sourceRectangle = new Rectangle(0, 64, 16, 32);
                        break;
                case 1:
                        sourceRectangle = new Rectangle(0, 32, 16, 32);
                        break;
                case 2:
                        sourceRectangle = new Rectangle(0, 0, 16, 32);
                        break;
                case 3:
                        sourceRectangle = new Rectangle(0, 32, 16, 32);
                        spriteEffects = SpriteEffects.FlipHorizontally;
                        break;
            }

            b.Draw(
                texture: outerwearData.EquippedTexture,
                position: position + origin + positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4),
                sourceRectangle: sourceRectangle,
                color: Color.White,
                rotation: rotation,
                origin: origin,
                scale: 4f * scale,
                effects: spriteEffects,
                layerDepth: layerDepth + 4.9E-04f
            );
        }
    }
}
