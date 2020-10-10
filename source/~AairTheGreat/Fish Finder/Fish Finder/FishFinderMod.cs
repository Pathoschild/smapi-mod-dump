/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

using FishFinder.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FishFinder
{
    public class FishFinderMod : Mod
    {
        public static FishFinderMod Instance { get; private set; }
        internal ModConfig config;

        private Dictionary<string, string> translations;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            helper.Events.Display.RenderedHud += Display_RenderedHud;

            try
            {
                config = this.Helper.Data.ReadJsonFile<ModConfig>("config.json") ?? ModConfigDefaultConfig.CreateDefaultConfig("config.json");
                config = ModConfigDefaultConfig.UpdateConfigToLatest(config, "config.json");
            }
            catch  //Really the only time this is going to error is when going from old version to new version of the config file or there is a bad config file
            {
                config = ModConfigDefaultConfig.UpdateConfigToLatest(config, "config.json") ?? ModConfigDefaultConfig.CreateDefaultConfig("config.json");
            }

            if (!helper.Translation.GetTranslations().Any())
                this.Monitor.Log("The translation files in this mod's i18n folder seem to be missing. The mod will still work, but you'll see 'missing translation' messages. Try reinstalling the mod to fix this.", LogLevel.Warn);

            ConfigStaticTranslationStrings();
        }

        private void ConfigStaticTranslationStrings()
        {
            translations = new Dictionary<string, string>();
            translations.Add("hud.foundFishingSpot", Helper.Translation.Get("hud.foundFishingSpot"));
            translations.Add("hud.noFishingSpot", Helper.Translation.Get("hud.noFishingSpot"));
            translations.Add("hud.north", Helper.Translation.Get("hud.north"));
            translations.Add("hud.northwest", Helper.Translation.Get("hud.northwest"));
            translations.Add("hud.northeast", Helper.Translation.Get("hud.northeast"));
            translations.Add("hud.south", Helper.Translation.Get("hud.south"));
            translations.Add("hud.southwest", Helper.Translation.Get("hud.southwest"));
            translations.Add("hud.southeast", Helper.Translation.Get("hud.southeast"));
            translations.Add("hud.west", Helper.Translation.Get("hud.west"));
            translations.Add("hud.east", Helper.Translation.Get("hud.east"));
            //translations.Add("hud.direction", Helper.Translation.Get("hud.direction"));
            //translations.Add("hud.distance", Helper.Translation.Get("hud.distance"));
        }

        private void Display_RenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (!config.showHudData || Game1.eventUp || !(Game1.player.CurrentTool is FishingRod))
                return;

            Color textColor = Color.White;
            SpriteFont font = Game1.smallFont;

            // Draw the panning info GUI to the screen
            float boxWidth = 0;
            float lineHeight = font.LineSpacing;
            Vector2 boxTopLeft = new Vector2(config.hudXPostion, config.hudYPostion);
            Vector2 boxBottomLeft = boxTopLeft;

            // Setup the sprite batch
            SpriteBatch batch = Game1.spriteBatch;
            batch.End();
            batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            Point splashPoint = Game1.player.currentLocation.fishSplashPoint.Value;
            string hudTextLine1 = "";
            string hudTextLine2 = "";
            string hudTextLine3 = "";

            if (!splashPoint.Equals(Point.Zero))
            {
                hudTextLine1 = translations["hud.foundFishingSpot"];
                string fishRelativePostion = GetFishingSpotRelativePostion(splashPoint);
                long fishSportDistance = GetDistanceToFishingSpot(splashPoint);
                if (config.showDistance)
                {
                    hudTextLine2 = Helper.Translation.Get("hud.direction", new { direction = fishRelativePostion });
                    hudTextLine3 = Helper.Translation.Get("hud.distance", new { distance = fishSportDistance });
                }
            }
            else
            {
                hudTextLine1 = translations["hud.noFishingSpot"];
            }

            batch.DrawStringWithShadow(font, hudTextLine1, boxBottomLeft, textColor, 1.0f);
            boxWidth = Math.Max(boxWidth, font.MeasureString(hudTextLine1).X + 5);
            boxBottomLeft += new Vector2(0, lineHeight);

            batch.DrawStringWithShadow(font, hudTextLine2, boxBottomLeft, textColor, 1.0f);
            boxWidth = Math.Max(boxWidth, font.MeasureString(hudTextLine2).X + 5);
            boxBottomLeft += new Vector2(0, lineHeight);

            batch.DrawStringWithShadow(font, hudTextLine3, boxBottomLeft, textColor, 1.0f);
            boxWidth = Math.Max(boxWidth, font.MeasureString(hudTextLine3).X + 5);
            boxBottomLeft += new Vector2(0, lineHeight);

            Texture2D box = Game1.staminaRect;
            // Draw the background rectangle DrawHelpers.WhitePixel
            batch.Draw(box, new Rectangle((int)boxTopLeft.X, (int)boxTopLeft.Y, (int)boxWidth, (int)(boxBottomLeft.Y - boxTopLeft.Y)), null,
                new Color(0, 0, 0, 0.25F), 0f, Vector2.Zero, SpriteEffects.None, 0.85F);

            batch.End();
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
        }

        private string GetFishingSpotRelativePostion(Point fishingSpot)
        {
            if (fishingSpot.X == Game1.player.getTileX())
            {
                return (fishingSpot.Y < Game1.player.getTileY()) ? translations["hud.north"] : translations["hud.south"];
            }

            if (fishingSpot.Y == Game1.player.getTileY())
            {
                return (fishingSpot.X < Game1.player.getTileX()) ? translations["hud.west"] : translations["hud.east"];
            }

            if (fishingSpot.X < Game1.player.getTileX())
            {
                return (fishingSpot.Y < Game1.player.getTileY()) ? translations["hud.northwest"] : translations["hud.southwest"];
            }
            else
            {
                return (fishingSpot.Y < Game1.player.getTileY()) ? translations["hud.northeast"] : translations["hud.southeast"];
            }
        }

        private long GetDistanceToFishingSpot(Point fishingSpot)
        {
            return (long)Math.Round(Math.Sqrt(Math.Pow(fishingSpot.X - Game1.player.getTileX(), 2) + Math.Pow(fishingSpot.Y - Game1.player.getTileY(), 2)));
        }
    }

    public static class DrawingExtensions
    {
        public static void DrawStringWithShadow(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color, float depth = 0F, float shadowDepth = 0.005F)
        {
            batch.DrawStringWithShadow(font, text, position, color, Color.Black, Vector2.One, depth, shadowDepth);
        }

        /// <summary>Draws a string with a shadow behind it.</summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="font">The font the text should use.</param>
        /// <param name="text">The string to draw.</param>
        /// <param name="position">The position of the string.</param>
        /// <param name="color">The color of the string. The shadow is black.</param>
        /// <param name="shadowColor">The color of the shadow.</param>
        /// <param name="scale">The amount to scale the size of the string by.</param>
        /// <param name="depth">The depth of the string.</param>
        /// <param name="shadowDepth">The depth of the shadow.</param>
        public static void DrawStringWithShadow(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color, Color shadowColor, Vector2 scale, float depth, float shadowDepth)
        {
            batch.DrawString(font, text, position + Vector2.One * Game1.pixelZoom * scale / 2f, shadowColor, 0F, Vector2.Zero, scale, SpriteEffects.None, shadowDepth);
            batch.DrawString(font, text, position, color, 0F, Vector2.Zero, scale, SpriteEffects.None, depth);
        }
    }
}
