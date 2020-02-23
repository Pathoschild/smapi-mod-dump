using System;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace JoysOfEfficiency.Huds
{
    internal class FishInformationHud
    {
        private static IReflectionHelper Reflection => InstanceHolder.Reflection;
        private static ITranslationHelper Translation => InstanceHolder.Translation;

        public static void DrawFishingInfoBox(SpriteBatch batch, BobberBar bar, SpriteFont font)
        {
            int width = 0, height = 120;


            float scale = 1.0f;


            int whichFish = Reflection.GetField<int>(bar, "whichFish").GetValue();
            int fishSize = Reflection.GetField<int>(bar, "fishSize").GetValue();
            int fishQuality = Reflection.GetField<int>(bar, "fishQuality").GetValue();
            bool treasure = Reflection.GetField<bool>(bar, "treasure").GetValue();
            bool treasureCaught = Reflection.GetField<bool>(bar, "treasureCaught").GetValue();
            float treasureAppearTimer = Reflection.GetField<float>(bar, "treasureAppearTimer").GetValue() / 1000;

            bool perfect = Reflection.GetField<bool>(bar, "perfect").GetValue();
            if (perfect)
            {
                if(fishQuality >= 2)
                    fishQuality = 4;
                else if (fishQuality >= 1)
                    fishQuality = 3;
            }
            Object fish = new Object(whichFish, 1, quality: fishQuality);
            int salePrice = fish.sellToStorePrice();

            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
            {
                scale = 0.7f;
            }

            string speciesText = Util.TryFormat(Translation.Get("fishinfo.species").ToString(), fish.DisplayName);
            string sizeText = Util.TryFormat(Translation.Get("fishinfo.size").ToString(), GetFinalSize(fishSize));
            string qualityText1 = Translation.Get("fishinfo.quality").ToString();
            string qualityText2 = Translation.Get(GetKeyForQuality(fishQuality)).ToString();
            string incomingText = Util.TryFormat(Translation.Get("fishinfo.treasure.incoming").ToString(), treasureAppearTimer);
            string appearedText = Translation.Get("fishinfo.treasure.appear").ToString();
            string caughtText = Translation.Get("fishinfo.treasure.caught").ToString();
            string priceText = Util.TryFormat(Translation.Get("fishinfo.price"), salePrice);

            {
                Vector2 size = font.MeasureString(speciesText) * scale;
                if (size.X > width)
                {
                    width = (int)size.X;
                }
                height += (int)size.Y;

                size = font.MeasureString(sizeText) * scale;
                if (size.X > width)
                {
                    width = (int)size.X;
                }
                height += (int)size.Y;

                Vector2 temp = font.MeasureString(qualityText1);
                Vector2 temp2 = font.MeasureString(qualityText2);
                size = new Vector2(temp.X + temp2.X, Math.Max(temp.Y, temp2.Y));
                if (size.X > width)
                {
                    width = (int)size.X;
                }
                height += (int)size.Y;

                size = font.MeasureString(priceText) * scale;
                if (size.X > width)
                {
                    width = (int)size.X;
                }
                height += (int)size.Y;
            }

            if (treasure)
            {
                if (treasureAppearTimer > 0)
                {
                    Vector2 size = font.MeasureString(incomingText) * scale;
                    if (size.X > width)
                    {
                        width = (int)size.X;
                    }
                    height += (int)size.Y;
                }
                else
                {
                    if (!treasureCaught)
                    {
                        Vector2 size = font.MeasureString(appearedText) * scale;
                        if (size.X > width)
                        {
                            width = (int)size.X;
                        }
                        height += (int)size.Y;
                    }
                    else
                    {
                        Vector2 size = font.MeasureString(caughtText) * scale;
                        if (size.X > width)
                        {
                            width = (int)size.X;
                        }
                        height += (int)size.Y;
                    }
                }
            }

            width += 64;

            int x = bar.xPositionOnScreen + bar.width + 96;
            if (x + width > Game1.viewport.Width)
            {
                x = bar.xPositionOnScreen - width - 96;
            }
            int y = (int)Util.Cap(bar.yPositionOnScreen, 0, Game1.viewport.Height - height);

            Util.DrawWindow(x, y, width, height);
            fish.drawInMenu(batch, new Vector2(x + width / 2 - 32, y + 16), 1.0f, 1.0f, 0.9f, StackDrawType.Hide);

            Vector2 vec2 = new Vector2(x + 32, y + 96);
            Util.DrawString(batch, font, ref vec2, speciesText, Color.Black, scale);
            Util.DrawString(batch, font, ref vec2, sizeText, Color.Black, scale);

            Util.DrawString(batch, font, ref vec2, qualityText1, Color.Black, scale, true);
            Util.DrawString(batch, font, ref vec2, qualityText2, GetColorForQuality(fishQuality), scale);

            vec2.X = x + 32;
            Util.DrawString(batch, font, ref vec2, priceText, Color.Black, scale);

            if (treasure)
            {
                if (!treasureCaught)
                {
                    if (treasureAppearTimer > 0f)
                    {
                        Util.DrawString(batch, font, ref vec2, incomingText, Color.Red, scale);
                    }
                    else
                    {
                        Util.DrawString(batch, font, ref vec2, appearedText, Color.LightGoldenrodYellow, scale);
                    }
                }
                else
                {
                    Util.DrawString(batch, font, ref vec2, caughtText, Color.ForestGreen, scale);
                }
            }
        }

        private static string GetKeyForQuality(int fishQuality)
        {
            switch (fishQuality)
            {
                case 1: return "quality.silver";
                case 2:
                case 3: return "quality.gold";
                case 4: return "quality.iridium";
                default: return "quality.normal";
            }
        }

        private static Color GetColorForQuality(int fishQuality)
        {
            switch (fishQuality)
            {
                case 1: return Color.AliceBlue;
                case 2:
                case 3: return Color.Tomato;
                case 4: return Color.Purple;
            }
            return Color.WhiteSmoke;
        }

        private static int GetFinalSize(int inch)
        {
            return LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? inch : (int)Math.Round(inch * 2.54);
        }
    }
}
