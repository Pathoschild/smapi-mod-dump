/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace FishingTrawler.UI
{
    internal static class TrawlerUI
    {
        private static float _scale = 2.5f;

        internal static void DrawUI(SpriteBatch b, int fishingTripTimer, int amountOfFish, int floodLevel, bool isHullLeaking, int rippedNetsCount, int fuelLevel, bool isComputerReady, bool hasLeftCabin)
        {
            _scale = 2.5f;
            int languageOffset = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? 8 : LocalizedContentManager.CurrentLanguageLatin ? 16 : 8;

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            b.Draw(FishingTrawler.assetManager.UITexture, new Vector2(22f, 16f) + new Vector2(-3f, -3f) * _scale, new Rectangle(0, 16, 7, 93), Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f - 0.001f);
            b.Draw(FishingTrawler.assetManager.UITexture, new Vector2(22f, 16f) + new Vector2(-1f, -3f) * _scale, new Rectangle(2, 16, 74, 93), Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f - 0.001f);
            b.Draw(FishingTrawler.assetManager.UITexture, new Vector2(22f, 16f) + new Vector2(65f, -3f) * _scale, new Rectangle(68, 16, 8, 103), Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f - 0.001f);

            b.DrawString(Game1.smallFont, $"{FishingTrawler.i18n.Get("ui.flooding.name")}: {floodLevel}%", new Vector2(32f, 24f), floodLevel > 75 ? Color.Red : isHullLeaking ? Color.Yellow : Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            b.DrawString(Game1.smallFont, string.Concat(FishingTrawler.i18n.Get("ui.nets.name"), ": ", rippedNetsCount < 1 ? FishingTrawler.i18n.Get("ui.generic.working") : rippedNetsCount > 1 ? FishingTrawler.i18n.Get("ui.nets.ripped") : FishingTrawler.i18n.Get("ui.nets.ripping")), new Vector2(32f, 60f), rippedNetsCount < 1 ? Color.White : rippedNetsCount > 1 ? Color.Red : Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            b.DrawString(Game1.smallFont, $"{FishingTrawler.i18n.Get("ui.engine.name")}: {fuelLevel}%", new Vector2(32f, 96f), fuelLevel < 30 ? Color.Red : fuelLevel <= 50 ? Color.Yellow : Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            b.DrawString(Game1.smallFont, $"{FishingTrawler.i18n.Get("ui.gps.name")}: {(isComputerReady ? FishingTrawler.i18n.Get("ui.gps.ready") : FishingTrawler.i18n.Get("ui.gps.plotting"))}", new Vector2(32f, 132f), isComputerReady is true ? Color.Yellow : Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            b.Draw(FishingTrawler.assetManager.UITexture, new Vector2(28f, 176f), new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
            b.DrawString(Game1.smallFont, $"{amountOfFish}", new Vector2(64f, 169f + languageOffset), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            b.Draw(FishingTrawler.assetManager.UITexture, new Vector2(96f, 169f), new Rectangle(16, 0, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);

            Color timeColor = Color.White;
            if (hasLeftCabin is false && Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2000.0 > 1000.0)
            {
                timeColor *= 0.5f;
            }
            b.DrawString(Game1.smallFont, Utility.getMinutesSecondsStringFromMilliseconds(fishingTripTimer), new Vector2(140f, 169f + languageOffset), timeColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        }
    }
}
