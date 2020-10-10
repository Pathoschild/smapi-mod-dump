/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using SilentOak.QualityProducts.Utils;
using StardewValley;

namespace SilentOak.QualityProducts
{
    internal static class Animation
    {
        public static TemporaryAnimatedSprite Bubbles(Vector2 position, Color color)
        {
            return new TemporaryAnimatedSprite(
                textureName: "TileSheets\\animations",
                sourceRect: new Rectangle(256, 1856, 64, 128),
                animationInterval: 80f,
                animationLength: 6,
                numberOfLoops: 999999,
                position: position * 64f + new Vector2(0f, -128f),
                flicker: false,
                flipped: false,
                layerDepth: (position.Y + 1f) * 64f / 10000f + 0.0001f,
                alphaFade: 0.005f,
                color: color * 0.75f,
                scale: 1f,
                scaleChange: 0f,
                rotation: 0f,
                rotationChange: 0f,
                local: false
            );
        }

        public static void PerformGraphics(GameLocation gameLocation, TemporaryAnimatedSprite animatedSprites)
        {
            Multiplayer multiplayer = Util.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            multiplayer.broadcastSprites(gameLocation, animatedSprites);
        }
    }
}
