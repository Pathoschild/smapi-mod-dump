/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using static DeepWoodsMod.DeepWoodsGlobals;

namespace DeepWoodsMod
{
    public class DeepWoodsDebris
    {
        public static void Initialize(DeepWoods deepWoods)
        {
            Clear(deepWoods);
            if (!Game1.isRaining)
            {
                int mapWidth = deepWoods.mapWidth.Value;
                int mapHeight = deepWoods.mapHeight.Value;

                if (DeepWoodsSettings.Settings.Performance.BaubleDensity > 0)
                {
                    int numBaubles = (mapWidth * mapHeight) / MINIMUM_TILES_FOR_BAUBLE;
                    if (DeepWoodsSettings.Settings.Performance.BaubleDensity < 100)
                    {
                        numBaubles = (int)(numBaubles * DeepWoodsSettings.Settings.Performance.BaubleDensity / 100);
                    }

                    for (int index = 0; index < numBaubles; ++index)
                    {
                        deepWoods.baubles.Add(new Vector2(Game1.random.Next(0, mapWidth * 64), Game1.random.Next(0, mapHeight * 64)));
                    }
                }

                if (Game1.currentSeason != "winter" && !deepWoods.isLichtung.Value && DeepWoodsSettings.Settings.Performance.LeafDensity > 0)
                {
                    int numWeatherDebris = (mapWidth * mapHeight) / MINIMUM_TILES_FOR_LEAVES;
                    if (DeepWoodsSettings.Settings.Performance.LeafDensity < 100)
                    {
                        numWeatherDebris = (int)(numWeatherDebris * DeepWoodsSettings.Settings.Performance.LeafDensity / 100);
                    }

                    for (int index = 0; index < numWeatherDebris; ++index)
                    {
                        Vector2 v = new Vector2(Game1.random.Next(0, mapWidth * 64), Game1.random.Next(0, mapHeight * 64));
                        deepWoods.weatherDebris.Add(new WeatherDebris(v, GetLeafType(deepWoods), Game1.random.Next(15) / 500f, Game1.random.Next(-10, 0) / 50f, Game1.random.Next(10) / 50f));
                    }
                }
            }
        }

        private static int GetLeafType(DeepWoods deepWoods)
        {
            if (deepWoods.isLichtung.Value)
            {
                return WeatherDebris.pinkPetals;
            }
            else if (Game1.currentSeason == "fall")
            {
                return WeatherDebris.fallLeaves;
            }
            else
            {
                return WeatherDebris.greenLeaves;
            }
        }

        public static void Clear(DeepWoods deepWoods)
        {
            deepWoods.baubles = new List<Vector2>();
            deepWoods.weatherDebris = new List<WeatherDebris>();
        }

        public static void Update(DeepWoods deepWoods, GameTime time)
        {
            if (deepWoods.baubles != null)
            {
                for (int index = 0; index < deepWoods.baubles.Count; ++index)
                {
                    Vector2 vector2 = new Vector2();
                    ref Vector2 local = ref vector2;
                    double num1 = deepWoods.baubles[index].X - Math.Max(0.4f, Math.Min(1f, index * 0.01f));
                    double num2 = index * 0.00999999977648258;
                    double num3 = 2.0 * Math.PI;
                    TimeSpan timeSpan = time.TotalGameTime;
                    double milliseconds = timeSpan.Milliseconds;
                    double num4 = Math.Sin(num3 * milliseconds / 8000.0);
                    double num5 = num2 * num4;
                    double num6 = num1 - num5;
                    local.X = (float)num6;
                    vector2.Y = deepWoods.baubles[index].Y + Math.Max(0.5f, Math.Min(1.2f, index * 0.02f));
                    if (vector2.Y > deepWoods.map.DisplayHeight || vector2.X < 0)
                    {
                        vector2.X = (float)Game1.random.Next(0, deepWoods.map.DisplayWidth);
                        vector2.Y = -64f;
                    }
                    deepWoods.baubles[index] = vector2;
                }
            }

            if (deepWoods.weatherDebris != null && !deepWoods.isLichtung.Value)
            {
                foreach (WeatherDebris weatherDebris in deepWoods.weatherDebris)
                {
                    weatherDebris.update();
                }
                Game1.updateDebrisWeatherForMovement(deepWoods.weatherDebris);
            }
        }

        public static void Draw(DeepWoods deepWoods, SpriteBatch b)
        {
            if (deepWoods.baubles != null)
            {
                for (int index = 0; index < deepWoods.baubles.Count; ++index)
                {
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, deepWoods.baubles[index]), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(346 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double)(index * 25)) % 600.0) / 150 * 5, 1971, 5, 5)), Color.White, (float)index * 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

                }
            }

            if (deepWoods.weatherDebris != null && !deepWoods.isLichtung.Value)
            {
                foreach (WeatherDebris weatherDebris in deepWoods.weatherDebris)
                {
                    weatherDebris.draw(b);
                }
            }
        }
    }
}
