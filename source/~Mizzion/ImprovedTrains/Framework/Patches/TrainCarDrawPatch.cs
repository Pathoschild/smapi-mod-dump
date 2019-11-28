using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace ImprovedTrains.Framework.Patches
{
    internal class TrainCarDrawPatch
    {
        private static IMonitor _monitor;

        public TrainCarDrawPatch(IMonitor monitor)
        {
            _monitor = monitor;
        }
        public static bool Prefix(TrainCar __instance, SpriteBatch b, Vector2 globalPosition, float wheelRotation)
        {
            TrainCar car = __instance;
            _monitor.Log($"Starting the draw proccess. Car Type: { car.carType.Value }, Resource Type: { car.resourceType.Value }");
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle(192 + car.carType.Value * 128, 512 - (car.alternateCar.Value ? 64 : 0), 128, 57), car.color.Value, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((globalPosition.Y + 256.0) / 10000.0));
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(0.0f, 228f)), new Rectangle(192 + car.carType.Value * 128, 569, 128, 7), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((globalPosition.Y + 256.0) / 10000.0));
            if (car.carType.Value == 1)
            {
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle(448 + car.resourceType.Value * 128 % 256, 576 + car.resourceType.Value / 2 * 32, 128, 32), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((globalPosition.Y + 260.0) / 10000.0));
                if (car.loaded.Value > 0 && Game1.random.NextDouble() < 0.003 && (globalPosition.X > 256.0 && globalPosition.X < (double)(Game1.currentLocation.map.DisplayWidth - 256)))
                {
                    --car.loaded.Value;
                    int objectIndex = -1;
                    switch (car.resourceType.Value)
                    {
                        case 0:
                            objectIndex = 521;
                            break;
                        case 1:
                            objectIndex = car.color.R > car.color.G ? 378 : (car.color.G > car.color.B ? 522 : (car.color.B > car.color.R ? 523 : 524));
                            break;
                        case 2:
                            objectIndex = 525;
                            break;
                        case 6:
                            objectIndex = 526;
                            break;
                        case 7:
                            objectIndex = Game1.currentSeason.Equals("winter") ? 536 : (Game1.stats.DaysPlayed <= 120U || car.color.R <= car.color.G ? 535 : 537);
                            break;
                    }
                    if (objectIndex != -1)
                        Game1.createObjectDebris(objectIndex, (int)globalPosition.X / 64, (int)globalPosition.Y / 64, (int)(globalPosition.Y + 320.0));
                    _monitor.Log($"Objects created should have been {objectIndex}");
                }
            }
            if (car.carType.Value == 0)
            {
                for (int index = 0; index < car.topFeatures.Count; index += 64)
                {
                    if (car.topFeatures[index] != -1)
                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(64 + index, 20f)), new Rectangle(192, 608 + car.topFeatures[index] * 16, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((globalPosition.Y + 260.0) / 10000.0));
                }
            }
            if (car.frontDecal.Value != -1 && (car.carType.Value == 0 || car.carType.Value == 1))
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(192f, 92f)), new Rectangle(224 + car.frontDecal.Value * 32 % 224, 576 + car.frontDecal.Value * 32 / 224 * 32, 32, 32), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((globalPosition.Y + 260.0) / 10000.0));
            if (car.carType.Value != 3)
                return false;
            Vector2 local1 = Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(72f, 208f));
            Vector2 local2 = Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(316f, 208f));
            b.Draw(Game1.mouseCursors, local1, new Rectangle(192, 576, 20, 20), Color.White, wheelRotation, new Vector2(10f, 10f), 4f, SpriteEffects.None, (float)((globalPosition.Y + 260.0) / 10000.0));
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(228f, 208f)), new Rectangle(192, 576, 20, 20), Color.White, wheelRotation, new Vector2(10f, 10f), 4f, SpriteEffects.None, (float)((globalPosition.Y + 260.0) / 10000.0));
            b.Draw(Game1.mouseCursors, local2, new Rectangle(192, 576, 20, 20), Color.White, wheelRotation, new Vector2(10f, 10f), 4f, SpriteEffects.None, (float)((globalPosition.Y + 260.0) / 10000.0));
            int x1 = (int)(local1.X + 4.0 + 24.0 * Math.Cos(wheelRotation));
            int y1 = (int)(local1.Y + 4.0 + 24.0 * Math.Sin(wheelRotation));
            int x2 = (int)(local2.X + 4.0 + 24.0 * Math.Cos(wheelRotation));
            int y2 = (int)(local2.Y + 4.0 + 24.0 * Math.Sin(wheelRotation));
            Utility.drawLineWithScreenCoordinates(x1, y1, x2, y2, b, new Color(112, 98, 92), (float)((globalPosition.Y + 264.0) / 10000.0));
            Utility.drawLineWithScreenCoordinates(x1, y1 + 2, x2, y2 + 2, b, new Color(112, 98, 92), (float)((globalPosition.Y + 264.0) / 10000.0));
            Utility.drawLineWithScreenCoordinates(x1, y1 + 4, x2, y2 + 4, b, new Color(53, 46, 43), (float)((globalPosition.Y + 264.0) / 10000.0));
            Utility.drawLineWithScreenCoordinates(x1, y1 + 6, x2, y2 + 6, b, new Color(53, 46, 43), (float)((globalPosition.Y + 264.0) / 10000.0));
            b.Draw(Game1.mouseCursors, new Vector2(x1 - 8, y1 - 8), new Rectangle(192, 640, 24, 24), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((globalPosition.Y + 268.0) / 10000.0));
            b.Draw(Game1.mouseCursors, new Vector2(x2 - 8, y2 - 8), new Rectangle(192, 640, 24, 24), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((globalPosition.Y + 268.0) / 10000.0));

            //Stop the original from running. This one will be so similar it wont matter.
            return false;
        }
    }
}
