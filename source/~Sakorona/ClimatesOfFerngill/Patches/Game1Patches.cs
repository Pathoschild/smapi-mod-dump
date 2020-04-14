using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using System;

namespace ClimatesOfFerngillRebuild.Patches
{
    public static class Game1Patches
    {
        public static bool UpdateWeatherPrefix(GameTime time)
        {
            if (Game1.isSnowing && Game1.currentLocation.IsOutdoors && !(Game1.currentLocation is Desert))
            {
                Vector2 current = new Vector2(Game1.viewport.X, Game1.viewport.Y);
                Game1.snowPos = Game1.updateFloatingObjectPositionForMovement(Game1.snowPos, current, Game1.previousViewportPosition, -1f);
            }
            if (Game1.isRaining && Game1.currentLocation.IsOutdoors)
            {
                for (int index = 0; index < Game1.rainDrops.Length; ++index)
                {
                    if (Game1.rainDrops[index].frame == 0)
                    {
                        Game1.rainDrops[index].accumulator += time.ElapsedGameTime.Milliseconds;
                        if (Game1.rainDrops[index].accumulator > 70)
                        {
                            //   Game1.rainDrops[index].position += new Vector2(index * 8 / Game1.rainDrops.Length - 16 + ClimatesOfFerngill.RainX, 32 - index * 8 / Game1.rainDrops.Length + ClimatesOfFerngill.RainY);
                            Game1.rainDrops[index].position += new Vector2(index * 8 / Game1.rainDrops.Length - 16, 32 - index * 8 / Game1.rainDrops.Length);
                            Game1.rainDrops[index].accumulator = 0;
                            if (Game1.random.NextDouble() < 0.1)
                                ++Game1.rainDrops[index].frame;
                            if (Game1.rainDrops[index].position.Y > (double)(Game1.viewport.Height + 64))
                                Game1.rainDrops[index].position.Y = -64f;
                        }
                    }
                    else
                    {
                        Game1.rainDrops[index].accumulator += time.ElapsedGameTime.Milliseconds;
                        if (Game1.rainDrops[index].accumulator > 70)
                        {
                            Game1.rainDrops[index].frame = (Game1.rainDrops[index].frame + 1) % 4;
                            Game1.rainDrops[index].accumulator = 0;
                            if (Game1.rainDrops[index].frame == 0)
                                Game1.rainDrops[index].position = new Vector2(Game1.random.Next(Game1.viewport.Width), Game1.random.Next(Game1.viewport.Height));
                        }
                    }
                }
            }
            if (Game1.isDebrisWeather && Game1.currentLocation.IsOutdoors && !Game1.currentLocation.ignoreDebrisWeather.Value)
            {
                //Game1.currentSeason.Equals("fall") && Game1.random.NextDouble() < 0.001 
                //default windthreshold is -0.5f
                if (Game1.currentSeason.Equals("fall") && Game1.random.NextDouble() < 0.001 && (Game1.windGust == 0.0 && (double)WeatherDebris.globalWind >= -0.5f))
                {
                    Game1.windGust += Game1.random.Next(-10, -1) / 100f;
                    if (Game1.soundBank != null)
                    {
                        Game1.wind = Game1.soundBank.GetCue("wind");
                        Game1.wind.Play();
                    }
                }
                else if (Game1.windGust != 0.0)
                {
                    //Game1.windGust = Math.Max(-5f, Game1.windGust * 1.02f);
                    /*if (ClimatesOfFerngill.WindOverrideSpeed == 0.0)
                        Game1.windGust = Math.Max(ClimatesOfFerngill.WindCap, ClimatesOfFerngill.WindMin);
                    else*/
                    Game1.windGust = Math.Max(-5f, Game1.windGust * 1.02f);

                    WeatherDebris.globalWind = Game1.windGust - 0.5f;
                    if (Game1.windGust < -0.200000002980232 && Game1.random.NextDouble() < 0.007)
                        Game1.windGust = 0.0f;

                    if (Game1.random.NextDouble() < 0.004) //kill long gusts, potentially, .4% every update tick
                        Game1.windGust = 0.0f;
                }
                foreach (WeatherDebris weatherDebris in Game1.debrisWeather)
                    weatherDebris.update();
            }
            if (WeatherDebris.globalWind >= -0.5 || Game1.wind == null)
                return false;
            WeatherDebris.globalWind = Math.Min(-0.5f, WeatherDebris.globalWind + 0.015f);
            Game1.wind.SetVariable("Volume", (float)(-WeatherDebris.globalWind * 20.0));
            Game1.wind.SetVariable("Frequency", (float)(-WeatherDebris.globalWind * 20.0));
            if (WeatherDebris.globalWind != -0.5)
                return false;
            Game1.wind.Stop(AudioStopOptions.AsAuthored);

            return false;
        }

#pragma warning disable IDE0060 
        public static bool DrawWeatherPrefix(Game1 __instance, GameTime time, RenderTarget2D target_screen)
#pragma warning restore IDE0060 
        {
            if (Game1.isSnowing && Game1.currentLocation.IsOutdoors && !(Game1.currentLocation is Desert))
            {
                Game1.snowPos.X %= 64f;
                Vector2 position = new Vector2();
                for (float num1 = (float)(Game1.snowPos.X % 64.0 - 64.0); num1 < (double)Game1.viewport.Width; num1 += 64f)
                {
                    for (float num2 = (float)(Game1.snowPos.Y % 64.0 - 64.0); num2 < (double)Game1.viewport.Height; num2 += 64f)
                    {
                        position.X = (int)num1;
                        position.Y = (int)num2;
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
                if (Game1.eventUp && !Game1.currentLocation.isTileOnMap(new Vector2(Game1.viewport.X / 64, Game1.viewport.Y / 64)))
                    return false;
                for (int index = 0; index < Game1.rainDrops.Length; ++index)
                    Game1.spriteBatch.Draw(Game1.rainTexture, Game1.rainDrops[index].position, new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.rainTexture, Game1.rainDrops[index].frame, -1, -1)), ClimatesOfFerngill.GetRainColor());
            }
			
			
			return false;
        }
	}
}