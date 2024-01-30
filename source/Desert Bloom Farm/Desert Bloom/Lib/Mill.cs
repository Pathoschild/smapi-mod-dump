/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-desert-bloom-farm
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Desert_Bloom.Lib
{
    public class Mill
    {
        public static Mod Mod;
        public static IMonitor Monitor;
        public static IModHelper Helper;
        public static Texture2D Tileset;
        public static int MsPerFrame = 20;
        public static int Tier = 0;
        public static void main()
        {
            Mod = ModEntry.Mod;
            Monitor = ModEntry._Monitor;
            Helper = ModEntry._Helper;

            Helper.Events.Display.RenderedWorld += Display_RenderedWorld;
            Helper.Events.GameLoop.DayStarted += DayStarted;
        }

        private static void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (ModEntry.IsMyFarm()) {
                Tileset = Helper.GameContent.Load<Texture2D>("Maps/Desert_Bloom_Tiles");
                var minmax = new KeyValuePair<int, int>(10, 24);
                var weather = Game1.getFarm().GetWeather();
                if (weather.IsLightning)
                    minmax = new(7, 10);
                else if (weather.IsSnowing)
                    minmax = new(14, 18);
                else if (weather.IsRaining)
                    minmax = new(20, 24);

                MsPerFrame = Game1.random.Next(minmax.Key, minmax.Value);
            }
        }

        private static void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!ModEntry.IsMyFarm() || Game1.currentLocation is not Farm)
                return;

            if (Tileset == null)
                Tileset = Helper.GameContent.Load<Texture2D>("Maps/Desert_Bloom_Tiles");

            var b = e.SpriteBatch;

            bool playerBehindMill = new Rectangle(41 * 64, 11 * 64, 3 * 64, 6 * 64).Contains(Game1.player.Position);
            int rg = (int)(255 - ((Game1.timeOfDay - 1800) * (180f / 400f)));
            Color color = Game1.timeOfDay >= 1800
                ? new Color(rg, rg, 255) * (playerBehindMill ? 0.5f : 1f)
                : Color.White * (playerBehindMill ? 0.5f : 1f);

            draw_mill_top(b, color);
            draw_mill_upgrades(b, color);
        }

        private static void draw_mill_upgrades(SpriteBatch b, Color color)
        {
            if (Tier == 0)
                draw_mill_cracks(b, color);

            if (Tier >= 1) {
                draw_sail(b, 0, color);
                draw_sail(b, 90, color);
                draw_sail(b, 180, color);
                draw_sail(b, 270, color);
            }

            if (Tier >= 5)
                draw_iridium_foundation(b, color);

            if (Tier >= 2)
                draw_pipes(b, color);

            if (Tier >= 4)
                draw_motor(b, color);
        }

        private static void draw_iridium_foundation(SpriteBatch b, Color color)
        {
            b.Draw(
                texture: Tileset,
                position: Game1.GlobalToLocal(Game1.viewport, new Vector2(41 * 64, 18 * 64 - 4)),
                sourceRectangle: new Rectangle(0, 192, 64, 16),
                color: color,
                rotation: 0f,
                origin: new Vector2(0, 0),
                scale: 4f,
                effects: SpriteEffects.FlipHorizontally,
                layerDepth: 0f
                );
        }

        private static void draw_pipes(SpriteBatch b, Color color)
        {
            b.Draw(
                texture: Tileset,
                position: Game1.GlobalToLocal(Game1.viewport, new Vector2(41 * 64, 18 * 64 - 4)),
                sourceRectangle: new Rectangle(112, 112, 16, 16),
                color: color,
                rotation: 0f,
                origin: new Vector2(0, 0),
                scale: 4f,
                effects: SpriteEffects.FlipHorizontally,
                layerDepth: 0f
                );
        }

        private static void draw_motor(SpriteBatch b, Color color)
        {
            b.Draw(
                texture: Tileset,
                position: Game1.GlobalToLocal(Game1.viewport, new Vector2(44 * 64, 18 * 64)),
                sourceRectangle: new Rectangle(112, 144, 16, 16),
                color: color,
                rotation: 0f,
                origin: new Vector2(0, 0),
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: 0f
                );
        }

        private static void draw_mill_cracks(SpriteBatch b, Color color)
        {
            b.Draw(
                texture: Tileset,
                position: Game1.GlobalToLocal(Game1.viewport, new Vector2(41 * 64 + 32, 11 * 64)),
                sourceRectangle: new Rectangle(64, 80, 48, 128),
                color: color,
                rotation: 0f,
                origin: new Vector2(0, 0),
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: 0f
                );
        }

        private static void draw_mill_top(SpriteBatch b, Color color)
        {
            b.Draw(
                texture: Tileset,
                position: Game1.GlobalToLocal(Game1.viewport, new Vector2(41 * 64, 11 * 64)),
                sourceRectangle: new Rectangle(0, 64, 64, 128),
                color: color,
                rotation: 0f,
                origin: new Vector2(0, 0),
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: 0f
                );
        }

        private static void draw_sail(SpriteBatch b, int initialRotation, Color color)
        {
            int frames = 360;
            float rotationMultiplier = (float)Math.Tau / frames;
            float rotation = rotationMultiplier * -(initialRotation + (DateTime.Now.Ticks / 10000) % (MsPerFrame * frames) / MsPerFrame);

            b.Draw(
                texture: Tileset,
                position: Game1.GlobalToLocal(Game1.viewport, new Vector2(43 * 64, 12 * 64)),
                sourceRectangle: new Rectangle(64, 64, 80, 16),
                color: color,
                rotation: rotation,
                origin: new Vector2(0, 0),
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: 0f
                );
        }
    }
}
