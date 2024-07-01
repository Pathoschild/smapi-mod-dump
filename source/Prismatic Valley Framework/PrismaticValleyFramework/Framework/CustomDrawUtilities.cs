/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jolly-Alpaca/PrismaticValleyFramework
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace PrismaticValleyFramework.Framework
{
    static class CustomDrawUtilities
    {
        /// <summary>
        /// Draw boots in a separate draw from the base farmer sprite
        /// </summary>
        /// <param name="__instance">The calling FarmerRenderer instance</param>
        /// <param name="who">The Farmer to draw</param>
        /// <param name="b">The SpriteBatch</param>
        /// <param name="position"></param>
        /// <param name="origin"></param>
        /// <param name="positionOffset"></param>
        /// <param name="sourceRect"></param>
        /// <param name="overrideColor"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="animationFrame"></param>
        /// <param name="layerDepth"></param>        
        public static void DrawCustomBoots(FarmerRenderer __instance, Farmer who, SpriteBatch b, Vector2 position, Vector2 origin, Vector2 positionOffset, Rectangle sourceRect, Color overrideColor, float rotation, float scale, FarmerSprite.AnimationFrame animationFrame, float layerDepth)
        {
            // Return if equipped boots are not custom boots
            if (who.boots.Value is null) return;
            string BootsId = who.boots.Value.ItemId;
            if (!ParseCustomFields.HasCustomColorData(BootsId)) return;
            // Use the custom texture if provided. Otherwise, use the default textures in the Assets folder.
            string BootsTextureTarget = ParseCustomFields.getCustomTextureTargetFromStringDictItem(BootsId) ?? (who.IsMale ? "JollyLlama.PrismaticValleyFramework/farmer_shoes" : "JollyLlama.PrismaticValleyFramework/farmer_girl_shoes");
            Texture2D BootsTexture = ModEntry.ModHelper.GameContent.Load<Texture2D>(BootsTextureTarget);
            // Get the custom color and draw the boots
            b.Draw(BootsTexture, position + origin + positionOffset, sourceRect, ParseCustomFields.getCustomColorFromStringDictItemWithColor(BootsId, overrideColor), rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
        }
    }
}