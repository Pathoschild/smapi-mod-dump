/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.GameLocations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;

namespace FishingTrawler.Framework.Managers
{
    public class NotificationManager
    {
        private IMonitor _monitor;

        // Notification related
        private string _activeNotification;
        private float _notificationAlpha;
        private bool _isNotificationFading;
        private double _remainingMilliseconds;

        public NotificationManager(IMonitor monitor)
        {
            _monitor = monitor;

            Reset();
        }

        private void Reset()
        {

            // Set up notification related variables
            _activeNotification = string.Empty;
            _notificationAlpha = 1f;
            _isNotificationFading = false;
            _remainingMilliseconds = 3000f;
        }

        internal string GetActiveNotification()
        {
            return _activeNotification;
        }

        internal void SetNotification(string message, bool forceRefresh = false)
        {
            if (String.IsNullOrEmpty(_activeNotification) is true || forceRefresh is true)
            {
                Reset();

                _activeNotification = message;
                FishingTrawler.BroadcastNotification(message, FishingTrawler.GetFarmersOnTrawler());
            }
        }

        internal bool HasExpired(double milliseconds)
        {
            if (String.IsNullOrEmpty(_activeNotification) is false)
            {
                _remainingMilliseconds -= milliseconds;
            }

            return _remainingMilliseconds <= 0;
        }


        internal void FadeNotification(float fadeAmount)
        {
            if (_isNotificationFading is false)
            {
                return;
            }
            _notificationAlpha -= fadeAmount;

            if (_notificationAlpha < 0f)
            {
                Reset();
            }
        }

        internal void StartFading()
        {
            if (String.IsNullOrEmpty(_activeNotification) is false)
            {
                _isNotificationFading = true;
            }
        }

        internal void DrawNotification(SpriteBatch b, GameLocation location)
        {
            FishingTrawler.modHelper.Reflection.GetMethod(typeof(SpriteText), "setUpCharacterMap").Invoke();

            int x = 0;
            int y = 0;

            int color = 8;
            float layerDepth = 0.88f;
            int width = SpriteText.getWidthOfString(_activeNotification) * 2;

            Vector2 position;
            switch (location)
            {
                case TrawlerSurface trawlerSurface:
                    x = 41;
                    y = 23;
                    position = FishingTrawler.config.useOldTrawlerSprite ? Game1.GlobalToLocal(new Vector2(40.45f, 22.8f) * 64f) : Game1.GlobalToLocal(new Vector2(41.5f, 23.7f) * 64f);
                    break;
                case TrawlerHull trawlerHull:
                    x = 14;
                    y = 1;
                    position = Game1.GlobalToLocal(new Vector2(1.59f, 0.58f) * 64f);
                    break;
                case TrawlerCabin trawlerCabin:
                    x = 8;
                    y = 0;
                    position = Game1.GlobalToLocal(new Vector2(8.5f, 0f) * 64f);
                    break;
                default:
                    position = new Vector2(0f, 0f);
                    break;
            }

            if (SpriteText.fontPixelZoom < 4f && LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.ko)
            {
                y += (int)((4f - SpriteText.fontPixelZoom) * 4f);
            }
            int accumulatedHorizontalSpaceBetweenCharacters = 0;

            int text_width = SpriteText.getWidthOfString(_activeNotification);
            Vector2 speech_position = position;
            if (Game1.currentLocation != null && Game1.currentLocation.map != null && Game1.currentLocation.map.Layers[0] != null)
            {
                int left_edge = -Game1.viewport.X + 28;
                int right_edge = -Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 28;

                speech_position.X += text_width / 2;

                speech_position.X = Utility.Clamp(speech_position.X, position.X, position.X + text_width - 24f);
            }

            position *= 1f / Game1.options.zoomLevel;
            b.Draw(Game1.mouseCursors, position + new Vector2(-7f, -3f) * 4f, new Rectangle(324, 299, 7, 17), Color.White * _notificationAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 0.001f);
            b.Draw(Game1.mouseCursors, position + new Vector2(0f, -3f) * 4f, new Rectangle(331, 299, 1, 17), Color.White * _notificationAlpha, 0f, Vector2.Zero, new Vector2(SpriteText.getWidthOfString(_activeNotification), 4f), SpriteEffects.None, layerDepth - 0.001f);
            b.Draw(Game1.mouseCursors, position + new Vector2(text_width, -12f), new Rectangle(332, 299, 7, 17), Color.White * _notificationAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 0.001f);
            b.Draw(Game1.mouseCursors, position + new Vector2(0f, 52f), new Rectangle(341, 308, 6, 5), Color.White * _notificationAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 0.0001f);

            x = (int)position.X;
            position.Y += (4f - SpriteText.fontPixelZoom) * 4f;

            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
            {
                position.Y -= 8f;
            }
            _activeNotification = _activeNotification.Replace(Environment.NewLine, "");
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.th)
            {
                position.Y -= (4f - SpriteText.fontPixelZoom) * 4f;
            }
            _activeNotification = _activeNotification.Replace('♡', '<');
            for (int i = 0; i < Math.Min(_activeNotification.Length, 9999); i++)
            {
                if (LocalizedContentManager.CurrentLanguageLatin || FishingTrawler.modHelper.Reflection.GetMethod(typeof(SpriteText), "IsSpecialCharacter").Invoke<bool>(_activeNotification[i]) || SpriteText.forceEnglishFont)
                {
                    float tempzoom = SpriteText.fontPixelZoom;
                    if (FishingTrawler.modHelper.Reflection.GetMethod(typeof(SpriteText), "IsSpecialCharacter").Invoke<bool>(_activeNotification[i]) || SpriteText.forceEnglishFont)
                    {
                        SpriteText.fontPixelZoom = 3f;
                    }
                    if (_activeNotification[i] == '^')
                    {
                        position.Y += 18f * SpriteText.fontPixelZoom;
                        position.X = x;
                        accumulatedHorizontalSpaceBetweenCharacters = 0;
                        SpriteText.fontPixelZoom = tempzoom;
                        continue;
                    }
                    accumulatedHorizontalSpaceBetweenCharacters = (int)(0f * SpriteText.fontPixelZoom);
                    bool upper = char.IsUpper(_activeNotification[i]) || _activeNotification[i] == 'ß';
                    Vector2 spriteFontOffset = new Vector2(0f, -1 + (upper ? -3 : 0));
                    if (_activeNotification[i] == 'Ç')
                    {
                        spriteFontOffset.Y += 2f;
                    }
                    if (SpriteText.positionOfNextSpace(_activeNotification, i, (int)position.X - x, accumulatedHorizontalSpaceBetweenCharacters) >= width)
                    {
                        position.Y += 18f * SpriteText.fontPixelZoom;
                        accumulatedHorizontalSpaceBetweenCharacters = 0;
                        position.X = x;
                        if (_activeNotification[i] == ' ')
                        {
                            SpriteText.fontPixelZoom = tempzoom;
                            continue;
                        }
                    }
                    b.Draw(color != -1 ? SpriteText.coloredTexture : SpriteText.spriteTexture, position + spriteFontOffset * SpriteText.fontPixelZoom, FishingTrawler.modHelper.Reflection.GetMethod(typeof(SpriteText), "getSourceRectForChar").Invoke<Rectangle>(_activeNotification[i], false), (FishingTrawler.modHelper.Reflection.GetMethod(typeof(SpriteText), "IsSpecialCharacter").Invoke<bool>(_activeNotification[i]) ? Color.White : SpriteText.getColorFromIndex(color)) * _notificationAlpha, 0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth);
                    if (i < _activeNotification.Length - 1)
                    {
                        position.X += 8f * SpriteText.fontPixelZoom + accumulatedHorizontalSpaceBetweenCharacters + SpriteText.getWidthOffsetForChar(_activeNotification[i + 1]) * SpriteText.fontPixelZoom;
                    }
                    if (_activeNotification[i] != '^')
                    {
                        position.X += SpriteText.getWidthOffsetForChar(_activeNotification[i]) * SpriteText.fontPixelZoom;
                    }
                    SpriteText.fontPixelZoom = tempzoom;
                    continue;
                }
            }
        }
    }
}
