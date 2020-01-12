using System;
using Microsoft.Xna.Framework;
using StardewValley;

namespace SpeedMod
{
    public class SpellEffects
    {
        public static void AddSprinklesToLocation(GameLocation location, float sparklesID, int sourceXTile, int sourceYTile, int tilesWide, int tilesHigh, int totalSprinkleDuration,
                                                  int millisecondsBetweenSprinkles, Color sprinkleColor, string sound = null, bool motionTowardCenter = false)
        {
            var area = new Rectangle(sourceXTile - tilesWide / 2, sourceYTile - tilesHigh / 2, tilesWide, tilesHigh);
            var numSprinkles = totalSprinkleDuration / millisecondsBetweenSprinkles;
            var random = new Random();

            for (var i = 0; i < numSprinkles; ++i)
            {
                var currentSprinklePosition = Utility.getRandomPositionInThisRectangle(area, random) * 64f;
                var velocityCenter = new Vector2(sourceXTile, sourceYTile) * 64f;
                var velocity = Vector2.Zero;

                if (motionTowardCenter)
                {
                    var distance = Vector2.Distance(velocityCenter, currentSprinklePosition);

                    velocity = Utility.getVelocityTowardPoint(currentSprinklePosition, velocityCenter, distance / 64f);
                }

                location.temporarySprites.Add(new TemporaryAnimatedSprite(random.Next(10, 12), currentSprinklePosition, sprinkleColor, 8, false, 50f)
                {
                    delayBeforeAnimationStart = millisecondsBetweenSprinkles * i,
                    motion = (motionTowardCenter ? velocity : Vector2.Zero),
                    xStopCoordinate = sourceXTile,
                    yStopCoordinate = sourceYTile,
                    startSound = sound,
                    id = sparklesID,
                    layerDepth = 1f,
                    interval = 100f
                });
            }
        }
    }
}
