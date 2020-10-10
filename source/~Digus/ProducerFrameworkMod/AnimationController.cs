/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using ProducerFrameworkMod.ContentPack;
using StardewValley;

namespace ProducerFrameworkMod
{
    internal class AnimationController
    {
        public static void DisplayAnimation(PlacingAnimation animation, Color animationColor, GameLocation currentLocation, Vector2 tileLocation, Vector2 offset)
        {
            Multiplayer multiplayer = DataLoader.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            switch (animation)
            {
                case PlacingAnimation.Bubbles:
                    multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite[1]
                    {
                        new TemporaryAnimatedSprite("TileSheets\\animations",
                            new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999,
                            tileLocation * 64f + new Vector2(0.0f, (float) sbyte.MinValue) + offset, false, false,
                            (float) (((double) tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f,
                            animationColor * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                        {
                            alphaFade = 0.005f
                        }
                    });
                    break;
                case PlacingAnimation.Fire:
                    multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite[1]
                    {
                        new TemporaryAnimatedSprite(30, tileLocation * 64f + new Vector2(0.0f, -16f) + offset, animationColor, 4, false,
                            50f, 10, 64, (float) (((double) tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), -1,
                            0)
                        {
                            alphaFade = 0.005f
                        }
                    });
                    break;
                case PlacingAnimation.CharcoalKilnSmoke:
                    multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite[1]
                    {
                        new TemporaryAnimatedSprite(27, tileLocation * 64f + new Vector2(-16f, (float) sbyte.MinValue) + offset, animationColor, 4, false, 50f, 10, 64, (float) (((double) tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), -1, 0)
                        {
                            alphaFade = 0.005f
                        }
                    });
                    break;
            }
        }
    }
}