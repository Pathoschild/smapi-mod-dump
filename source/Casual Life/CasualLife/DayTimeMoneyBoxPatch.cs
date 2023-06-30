/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/adverserath/StardewValley-CasualLifeMod
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Text;

namespace CasualLife
{
    class DayTimeMoneyBoxPatch
    {
        private static IMonitor Monitor;
        public static ModConfig Config;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }
        public static bool Is24Hour { get { return Config.Is24HourDefault; } set { Config.Is24HourDefault = value; } }

        public static bool receiveRightClick(int x, int y, bool playSound = true)
        {
            Is24Hour = !Is24Hour;
            return true;
        }

        private static StringBuilder _hoverText = new StringBuilder();

        private static StringBuilder _timeText = new StringBuilder();

        private static StringBuilder _dateText = new StringBuilder();

        private static StringBuilder _hours = new StringBuilder();

        private static StringBuilder _padZeros = new StringBuilder();

        private static StringBuilder _temp = new StringBuilder();

        private static string _amString;

        private static string _pmString;

        public static bool drawFromDecom(SpriteBatch b, ref DayTimeMoneyBox __instance,
            ref Rectangle ___sourceRect,
            ref string ____hoverText,
            ref int ____lastDayOfMonth,
            ref string ____lastDayOfMonthString,
            ref StringBuilder ____dateText)
        {
            if (!____hoverText.Equals("") && __instance.isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                IClickableMenu.drawHoverText(b, "Journal (F)\nRight Click to Change Clock", Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null, null);
            }

            SpriteFont spriteFont = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? Game1.smallFont : Game1.dialogueFont;

            __instance.position = new Vector2(Game1.uiViewport.Width - 300, 8f);
            if (Game1.isOutdoorMapSmallerThanViewport())
            {
                __instance.position = new Vector2(Math.Min(__instance.position.X, -Game1.uiViewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 300), 8f);
            }

            Utility.makeSafe(ref __instance.position, 300, 284);
            __instance.xPositionOnScreen = (int)__instance.position.X;
            __instance.yPositionOnScreen = (int)__instance.position.Y;
            __instance.questButton.bounds = new Rectangle(__instance.xPositionOnScreen + 212, __instance.yPositionOnScreen + 240, 44, 46);
            __instance.zoomOutButton.bounds = new Rectangle(__instance.xPositionOnScreen + 92, __instance.yPositionOnScreen + 244, 28, 32);
            __instance.zoomInButton.bounds = new Rectangle(__instance.xPositionOnScreen + 124, __instance.yPositionOnScreen + 244, 28, 32);

            if (__instance.timeShakeTimer > 0)
            {
                __instance.timeShakeTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            }

            if (__instance.questPulseTimer > 0)
            {
                __instance.questPulseTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            }

            if (__instance.whenToPulseTimer >= 0)
            {
                __instance.whenToPulseTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
                if (__instance.whenToPulseTimer <= 0)
                {
                    __instance.whenToPulseTimer = 3000;
                    if (Game1.player.hasNewQuestActivity())
                    {
                        __instance.questPulseTimer = 1000;
                    }
                }
            }

            b.Draw(Game1.mouseCursors, __instance.position, ___sourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
            if (Game1.dayOfMonth != ____lastDayOfMonth)
            {
                ____lastDayOfMonth = Game1.dayOfMonth;
                ____lastDayOfMonthString = Game1.shortDayDisplayNameFromDayOfSeason(____lastDayOfMonth);
            }

            ____dateText.Clear();
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja)
            {
                ____dateText.AppendEx(Game1.dayOfMonth);
                ____dateText.Append("日 (");
                ____dateText.Append(____lastDayOfMonthString);
                ____dateText.Append(")");
            }
            else if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh)
            {
                ____dateText.Append(____lastDayOfMonthString);
                ____dateText.Append(" ");
                ____dateText.AppendEx(Game1.dayOfMonth);
                ____dateText.Append("日");
            }
            else if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.mod)
            {
                ____dateText.Append(LocalizedContentManager.CurrentModLanguage.ClockDateFormat.Replace("[DAY_OF_WEEK]", ____lastDayOfMonthString).Replace("[DAY_OF_MONTH]", Game1.dayOfMonth.ToString()));
            }
            else
            {
                ____dateText.Append(____lastDayOfMonthString);
                ____dateText.Append(". ");
                ____dateText.AppendEx(Game1.dayOfMonth);
            }

            Vector2 vector = spriteFont.MeasureString(____dateText);
            Vector2 value = new Vector2((float)___sourceRect.X * 0.55f - vector.X / 2f, (float)___sourceRect.Y * (LocalizedContentManager.CurrentLanguageLatin ? 0.1f : 0.1f) - vector.Y / 2f);
            Utility.drawTextWithShadow(b, ____dateText, spriteFont, __instance.position + value, Game1.textColor);
            b.Draw(Game1.mouseCursors, __instance.position + new Vector2(212f, 68f), new Rectangle(406, 441 + Utility.getSeasonNumber(Game1.currentSeason) * 8, 12, 8), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
            b.Draw(Game1.mouseCursors, __instance.position + new Vector2(116f, 68f), new Rectangle(317 + 12 * Game1.weatherIcon, 421, 12, 8), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
            _padZeros.Clear();
            string zeroPad = (Game1.timeOfDay % 100) < 10 && (Game1.timeOfDay % 100) > 0 ? "0" : "";
            string hours = (Game1.timeOfDay / 100 % 12 == 0 ? "12" : string.Concat(Game1.timeOfDay / 100 % 12));

            if (Game1.timeOfDay % 100 == 0)
            {
                _padZeros.Append("0");
            }

            _hours.Clear();
            switch (LocalizedContentManager.CurrentLanguageCode)
            {
                case LocalizedContentManager.LanguageCode.zh:
                    if (Game1.timeOfDay / 100 % 24 == 0)
                    {
                        _hours.Append("00");
                    }
                    else if (Game1.timeOfDay / 100 % 12 == 0)
                    {
                        _hours.Append("12");
                    }
                    else
                    {
                        _hours.AppendEx(Game1.timeOfDay / 100 % 12);
                    }

                    break;
                case LocalizedContentManager.LanguageCode.ru:
                case LocalizedContentManager.LanguageCode.pt:
                case LocalizedContentManager.LanguageCode.es:
                case LocalizedContentManager.LanguageCode.de:
                case LocalizedContentManager.LanguageCode.th:
                case LocalizedContentManager.LanguageCode.fr:
                case LocalizedContentManager.LanguageCode.tr:
                case LocalizedContentManager.LanguageCode.hu:
                    _temp.Clear();
                    _temp.AppendEx(Game1.timeOfDay / 100 % 24);
                    if (Game1.timeOfDay / 100 % 24 <= 9)
                    {
                        _hours.Append("0");
                    }

                    _hours.AppendEx(_temp);
                    break;
                default:
                    if (Is24Hour)
                    {
                        hours = string.Concat(Game1.timeOfDay / 100 % 24);
                        hours = (Game1.timeOfDay / 100 % 24 <= 9 ? string.Concat("0", hours) : hours);
                        _hours.Append(hours);

                    }
                    else
                    {

                        if (Game1.timeOfDay / 100 % 12 == 0)
                        {
                            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja)
                            {
                                _hours.Append("0");
                            }
                            else
                            {
                                _hours.Append("12");
                            }
                        }
                        else
                        {
                            _hours.AppendEx(Game1.timeOfDay / 100 % 12);
                        }
                    }
                    break;
            }

            _timeText.Clear();
            _timeText.AppendEx(_hours);
            _timeText.Append(":");
            _timeText.Append(zeroPad);
            _timeText.AppendEx(Game1.timeOfDay % 100);
            _timeText.AppendEx(_padZeros);
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it)
            {
                _timeText.Append(" ");
                if (Game1.timeOfDay < 1200 || Game1.timeOfDay >= 2400)
                {
                    _timeText.Append(_amString);
                }
                else
                {
                    _timeText.Append(_pmString);
                }
            }
            else if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
            {
                if (Game1.timeOfDay < 1200 || Game1.timeOfDay >= 2400)
                {
                    _timeText.Append(_amString);
                }
                else
                {
                    _timeText.Append(_pmString);
                }
            }
            else if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja)
            {
                _temp.Clear();
                _temp.AppendEx(_timeText);
                _timeText.Clear();
                if (Game1.timeOfDay < 1200 || Game1.timeOfDay >= 2400)
                {
                    _timeText.Append(_amString);
                    _timeText.Append(" ");
                    _timeText.AppendEx(_temp);
                }
                else
                {
                    _timeText.Append(_pmString);
                    _timeText.Append(" ");
                    _timeText.AppendEx(_temp);
                }
            }
            else if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh)
            {
                _temp.Clear();
                _temp.AppendEx(_timeText);
                _timeText.Clear();
                if (Game1.timeOfDay < 600 || Game1.timeOfDay >= 2400)
                {
                    _timeText.Append("凌晨 ");
                    _timeText.AppendEx(_temp);
                }
                else if (Game1.timeOfDay < 1200)
                {
                    _timeText.Append(_amString);
                    _timeText.Append(" ");
                    _timeText.AppendEx(_temp);
                }
                else if (Game1.timeOfDay < 1300)
                {
                    _timeText.Append("中午  ");
                    _timeText.AppendEx(_temp);
                }
                else if (Game1.timeOfDay < 1900)
                {
                    _timeText.Append(_pmString);
                    _timeText.Append(" ");
                    _timeText.AppendEx(_temp);
                }
                else
                {
                    _timeText.Append("晚上  ");
                    _timeText.AppendEx(_temp);
                }
            }
            else if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.mod)
            {
                _timeText.Clear();
                _timeText.Append(LocalizedContentManager.FormatTimeString(Game1.timeOfDay, LocalizedContentManager.CurrentModLanguage.ClockTimeFormat));
            }

            Vector2 vector2 = spriteFont.MeasureString(_timeText);
            Vector2 value2 = new Vector2((float)___sourceRect.X * 0.55f - vector2.X / 2f + (float)((__instance.timeShakeTimer > 0) ? Game1.random.Next(-2, 3) : 0), (float)___sourceRect.Y * (LocalizedContentManager.CurrentLanguageLatin ? 0.31f : 0.31f) - vector2.Y / 2f + (float)((__instance.timeShakeTimer > 0) ? Game1.random.Next(-2, 3) : 0));
            bool flag = Game1.shouldTimePass() || Game1.fadeToBlack || Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2000.0 > 1000.0;
            Utility.drawTextWithShadow(b, _timeText, spriteFont, __instance.position + value2, (Game1.timeOfDay >= 2400) ? Color.Red : (Game1.textColor * (flag ? 1f : 0.5f)));
            int num = (int)((float)(Game1.timeOfDay - Game1.timeOfDay % 100) + (float)(Game1.timeOfDay % 100 / 10) * 16.66f);
            if (Game1.player.visibleQuestCount > 0)
            {
                __instance.questButton.draw(b);
                if (__instance.questPulseTimer > 0)
                {
                    float num2 = 1f / (Math.Max(300f, Math.Abs(__instance.questPulseTimer % 1000 - 500)) / 500f);
                    b.Draw(Game1.mouseCursors, new Vector2(__instance.questButton.bounds.X + 24, __instance.questButton.bounds.Y + 32) + ((num2 > 1f) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle(395, 497, 3, 8), Color.White, 0f, new Vector2(2f, 4f), 4f * num2, SpriteEffects.None, 0.99f);
                }

                if (__instance.questPingTimer > 0)
                {
                    b.Draw(Game1.mouseCursors, new Vector2(Game1.dayTimeMoneyBox.questButton.bounds.Left - 16, Game1.dayTimeMoneyBox.questButton.bounds.Bottom + 8), new Rectangle(128 + ((__instance.questPingTimer / 200 % 2 != 0) ? 16 : 0), 208, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
                }
            }

            if (Game1.options.zoomButtons)
            {
                __instance.zoomInButton.draw(b, Color.White * ((Game1.options.desiredBaseZoomLevel >= 2f) ? 0.5f : 1f), 1f);
                __instance.zoomOutButton.draw(b, Color.White * ((Game1.options.desiredBaseZoomLevel <= 0.75f) ? 0.5f : 1f), 1f);
            }

            __instance.drawMoneyBox(b);
            if (_hoverText.Length > 0 && __instance.isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                IClickableMenu.drawHoverText(b, _hoverText, Game1.dialogueFont);
            }

            b.Draw(Game1.mouseCursors, __instance.position + new Vector2(88f, 88f), new Rectangle(324, 477, 7, 19), Color.White, (float)(Math.PI + Math.Min(Math.PI, (double)(((float)num + (float)Game1.gameTimeInterval / 7000f * 16.6f - 600f) / 2000f) * Math.PI)), new Vector2(3f, 17f), 4f, SpriteEffects.None, 0.9f);
            return false;

        }

    }
}
