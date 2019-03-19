using Microsoft.Xna.Framework;
using StardewValley;

namespace QualityProducts
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
            Multiplayer multiplayer = QualityProducts.Instance.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            multiplayer.broadcastSprites(gameLocation, animatedSprites);
        }
    }
}
