using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using System;

namespace ClimatesOfFerngillRebuild.Patches
{
    public static class Game1Patches
    {
		public static bool DrawWeatherPrefix(Game1 __instance, GameTime time, RenderTarget2D target_screen)
        {
            if (Game1.isSnowing && Game1.currentLocation.IsOutdoors && !(Game1.currentLocation is Desert))
            {
                Game1.snowPos.X %= 64f;
                Vector2 position = new Vector2();
                for (float num1 = (float)((double)Game1.snowPos.X % 64.0 - 64.0); (double)num1 < (double)Game1.viewport.Width; num1 += 64f)
                {
                    for (float num2 = (float)((double)Game1.snowPos.Y % 64.0 - 64.0); (double)num2 < (double)Game1.viewport.Height; num2 += 64f)
                    {
                        position.X = (float)(int)num1;
                        position.Y = (float)(int)num2;
                        Game1.spriteBatch.Draw(Game1.mouseCursors, position, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(368 + (int)(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1200.0) / 75 * 16, 192, 16, 16)), ClimatesOfFerngill.GetSnowColor() * 0.8f * Game1.options.snowTransparency, 0.0f, Vector2.Zero, 4.001f, SpriteEffects.None, 1f);
                    }
                }
            }
            if (Game1.isDebrisWeather && Game1.currentLocation.IsOutdoors && (!Game1.currentLocation.ignoreDebrisWeather.Value && !Game1.currentLocation.Name.Equals("Desert")))
            {
                if (__instance.takingMapScreenshot)
                {
                    if (Game1.debrisWeather != null)
                    {
                        foreach (WeatherDebris weatherDebris in Game1.debrisWeather)
                        {
                            Vector2 position = weatherDebris.position;
                            weatherDebris.position = new Vector2(Game1.random.Next(Game1.viewport.Width - weatherDebris.sourceRect.Width * 3), Game1.random.Next(Game1.viewport.Height - weatherDebris.sourceRect.Height * 3));
                            weatherDebris.draw(Game1.spriteBatch);
                            weatherDebris.position = position;
                        }
                    }
                }
                else if (Game1.viewport.X > -Game1.viewport.Width)
                {
                    foreach (WeatherDebris weatherDebris in Game1.debrisWeather)
                        weatherDebris.draw(Game1.spriteBatch);
                }
            }
            if (!Game1.isRaining || !Game1.currentLocation.IsOutdoors || (Game1.currentLocation.Name.Equals("Desert") || Game1.currentLocation is Summit))
                return false;
            if (__instance.takingMapScreenshot)
            {
                for (int index = 0; index < Game1.rainDrops.Length; ++index)
                {
                    Vector2 position = new Vector2(Game1.random.Next(Game1.viewport.Width - 64), Game1.random.Next(Game1.viewport.Height - 64));
                    Game1.spriteBatch.Draw(Game1.rainTexture, position, new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.rainTexture, Game1.rainDrops[index].frame, -1, -1)), ClimatesOfFerngill.GetRainColor());
                }
            }
            else
            {
                if (Game1.eventUp && !Game1.currentLocation.isTileOnMap(new Vector2((float)(Game1.viewport.X / 64), (float)(Game1.viewport.Y / 64))))
                    return false;
                for (int index = 0; index < Game1.rainDrops.Length; ++index)
                    Game1.spriteBatch.Draw(Game1.rainTexture, Game1.rainDrops[index].position, new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.rainTexture, Game1.rainDrops[index].frame, -1, -1)), ClimatesOfFerngill.GetRainColor());
            }
			
			
			return false;
        }
	}
}