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
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;

namespace CasualLife
{
    class DayTimeMoneyBoxPatch
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }
        public static bool Is24Hour { get; set; }

        public static bool receiveRightClick(int x, int y, bool playSound = true)
        {
            Is24Hour = !Is24Hour;
            return true;
        }

        public static bool drawFromDecom(SpriteBatch b, ref DayTimeMoneyBox __instance, ref Rectangle ___sourceRect, ref string ____hoverText)
		{
            if (!____hoverText.Equals("") && __instance.isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                IClickableMenu.drawHoverText(b, "Journal (F)\nRight Click to Change Clock", Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null, null);
            }

            TimeSpan elapsedGameTime;
            string str;
            float obj = 0;
            float obj1 = 0;
            bool totalMilliseconds;
            string str1;
            string str2;
            SpriteFont font = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? Game1.smallFont : Game1.dialogueFont);

            __instance.position = new Vector2((float)(Game1.viewport.Width - 300), 8f);
            if (Game1.isOutdoorMapSmallerThanViewport())
            {
                __instance.position = new Vector2(Math.Min(__instance.position.X, (float)(-Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 300)), 8f);
            }
            Utility.makeSafe(ref __instance.position, 300, 284);
            __instance.xPositionOnScreen = (int)__instance.position.X;
            __instance.yPositionOnScreen = (int)__instance.position.Y;
            __instance.questButton.bounds = new Rectangle(__instance.xPositionOnScreen + 212, __instance.yPositionOnScreen + 240, 44, 46);
            __instance.zoomOutButton = new ClickableTextureComponent(new Rectangle(__instance.xPositionOnScreen + 92, __instance.yPositionOnScreen + 244, 28, 32), Game1.mouseCursors, new Rectangle(177, 345, 7, 8), 4f, false);
            __instance.zoomInButton = new ClickableTextureComponent(new Rectangle(__instance.xPositionOnScreen + 124, __instance.yPositionOnScreen + 244, 28, 32), Game1.mouseCursors, new Rectangle(184, 345, 7, 8), 4f, false);


            if (__instance.timeShakeTimer > 0)
            {
                int num = __instance.timeShakeTimer;
                elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
                __instance.timeShakeTimer = num - elapsedGameTime.Milliseconds;
            }
            if (__instance.questPulseTimer > 0)
            {
                int num1 = __instance.questPulseTimer;
                elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
                __instance.questPulseTimer = num1 - elapsedGameTime.Milliseconds;
            }
            if (__instance.whenToPulseTimer >= 0)
            {
                int num2 = __instance.whenToPulseTimer;
                elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
                __instance.whenToPulseTimer = num2 - elapsedGameTime.Milliseconds;
                if (__instance.whenToPulseTimer <= 0)
                {
                    __instance.whenToPulseTimer = 3000;
                    if (Game1.player.hasNewQuestActivity())
                    {
                        __instance.questPulseTimer = 1000;
                    }
                }
            }
            b.Draw(Game1.mouseCursors, __instance.position, new Rectangle?(___sourceRect), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja)
            {
                str = string.Concat(new object[] { Game1.dayOfMonth, "日 (", Game1.shortDayDisplayNameFromDayOfSeason(Game1.dayOfMonth), ")" });
            }
            else
            {
                str = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh ? string.Concat(new object[] { Game1.shortDayDisplayNameFromDayOfSeason(Game1.dayOfMonth), " ", Game1.dayOfMonth, "日" }) : string.Concat(Game1.shortDayDisplayNameFromDayOfSeason(Game1.dayOfMonth), ". ", Game1.dayOfMonth));
            }
            string dateText = str;
            Vector2 daySize = font.MeasureString(dateText);
            Vector2 dayPosition = new Vector2((float)___sourceRect.X * 0.55f - daySize.X / 2f, (float)___sourceRect.Y * (LocalizedContentManager.CurrentLanguageLatin ? 0.1f : 0.1f) - daySize.Y / 2f);
            Utility.drawTextWithShadow(b, dateText, font, __instance.position + dayPosition, Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            b.Draw(Game1.mouseCursors, __instance.position + new Vector2(212f, 68f), new Rectangle?(new Rectangle(406, 441 + Utility.getSeasonNumber(Game1.currentSeason) * 8, 12, 8)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
            b.Draw(Game1.mouseCursors, __instance.position + new Vector2(116f, 68f), new Rectangle?(new Rectangle(317 + 12 * Game1.weatherIcon, 421, 12, 8)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
            string zeroPad = (Game1.timeOfDay % 100 < 10 ? "0" : "");
            string hours = (Game1.timeOfDay / 100 % 12 == 0 ? "12" : string.Concat(Game1.timeOfDay / 100 % 12));
            switch (LocalizedContentManager.CurrentLanguageCode)
            {
                case LocalizedContentManager.LanguageCode.en:
                case LocalizedContentManager.LanguageCode.ko:
                case LocalizedContentManager.LanguageCode.it:
                    {
                        if (Is24Hour)
                        {
                            hours = string.Concat(Game1.timeOfDay / 100 % 24);
                            hours = (Game1.timeOfDay / 100 % 24 <= 9 ? string.Concat("0", hours) : hours);
                        }
                        else
                        {
                            hours = (Game1.timeOfDay / 100 % 12 == 0 ? "12" : string.Concat(Game1.timeOfDay / 100 % 12));
                        }
                        break;
                    }
                case LocalizedContentManager.LanguageCode.ja:
                    {
                        hours = (Game1.timeOfDay / 100 % 12 == 0 ? "0" : string.Concat(Game1.timeOfDay / 100 % 12));
                        break;
                    }
                case LocalizedContentManager.LanguageCode.ru:
                case LocalizedContentManager.LanguageCode.pt:
                case LocalizedContentManager.LanguageCode.es:
                case LocalizedContentManager.LanguageCode.de:
                case LocalizedContentManager.LanguageCode.th:
                case LocalizedContentManager.LanguageCode.fr:
                case LocalizedContentManager.LanguageCode.tr:
                case LocalizedContentManager.LanguageCode.hu:
                    {
                        hours = string.Concat(Game1.timeOfDay / 100 % 24);
                        hours = (Game1.timeOfDay / 100 % 24 <= 9 ? string.Concat("0", hours) : hours);
                        break;
                    }
                case LocalizedContentManager.LanguageCode.zh:
                    {
                        if (Game1.timeOfDay / 100 % 24 == 0)
                        {
                            str2 = "00";
                        }
                        else
                        {
                            str2 = (Game1.timeOfDay / 100 % 12 == 0 ? "12" : string.Concat(Game1.timeOfDay / 100 % 12));
                        }
                        hours = str2;
                        break;
                    }
            }

            string timeText = string.Concat(new object[] { hours, ":", zeroPad, Game1.timeOfDay % 100 });
            if (!Is24Hour)
            {
                if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it)
                {
                    timeText = string.Concat(timeText, " ", (Game1.timeOfDay < 1200 || Game1.timeOfDay >= 2400 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370") : Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371")));
                }
                else if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
                {
                    timeText = string.Concat(timeText, (Game1.timeOfDay < 1200 || Game1.timeOfDay >= 2400 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370") : Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371")));
                }
                else if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja)
                {
                    timeText = (Game1.timeOfDay < 1200 || Game1.timeOfDay >= 2400 ? string.Concat(Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370"), " ", timeText) : string.Concat(Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371"), " ", timeText));
                }
                else if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh)
                {
                    if (Game1.timeOfDay < 600 || Game1.timeOfDay >= 2400)
                    {
                        str1 = string.Concat("凌晨 ", timeText);
                    }
                    else if (Game1.timeOfDay < 1200)
                    {
                        str1 = string.Concat(Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370"), " ", timeText);
                    }
                    else if (Game1.timeOfDay < 1300)
                    {
                        str1 = string.Concat("中午  ", timeText);
                    }
                    else
                    {
                        str1 = (Game1.timeOfDay < 1900 ? string.Concat(Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371"), " ", timeText) : string.Concat("晚上  ", timeText));
                    }
                    timeText = str1;
                }
            }
            Vector2 txtSize = font.MeasureString(timeText);
            float x = (float)___sourceRect.X * 0.55f - txtSize.X / 2f;
            if (__instance.timeShakeTimer > 0)
            {
                obj = Game1.random.Next(-2, 3);
            }

            float single = x + obj;
            float y = (float)___sourceRect.Y * (LocalizedContentManager.CurrentLanguageLatin ? 0.31f : 0.31f) - txtSize.Y / 2f;
            if (__instance.timeShakeTimer > 0)
            {
                obj1 = Game1.random.Next(-2, 3);
            }

            Vector2 timePosition = new Vector2(single, y + obj1);
            if (Game1.shouldTimePass() || Game1.fadeToBlack)
            {
                totalMilliseconds = true;
            }
            else
            {
                elapsedGameTime = Game1.currentGameTime.TotalGameTime;
                totalMilliseconds = elapsedGameTime.TotalMilliseconds % 2000 > 1000;
            }
            bool nofade = totalMilliseconds;
            Utility.drawTextWithShadow(b, timeText, font, __instance.position + timePosition, (Game1.timeOfDay >= 2400 ? Color.Red : Game1.textColor * (nofade ? 1f : 0.5f)), 1f, -1f, -1, -1, 1f, 3);
            int adjustedTime = (int)((float)(Game1.timeOfDay - Game1.timeOfDay % 100) + (float)(Game1.timeOfDay % 100 / 10) * 16.66f);
            if (Game1.player.visibleQuestCount > 0)
            {
                __instance.questButton.draw(b);
                if (__instance.questPulseTimer > 0)
                {
                    float scaleMult = 1f / ((float)Math.Max(300f, (float)Math.Abs(__instance.questPulseTimer % 1000 - 500)) / 500f);
                    b.Draw(Game1.mouseCursors, new Vector2((float)(__instance.questButton.bounds.X + 24), (float)(__instance.questButton.bounds.Y + 32)) + (scaleMult > 1f ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(new Rectangle(395, 497, 3, 8)), Color.White, 0f, new Vector2(2f, 4f), 4f * scaleMult, SpriteEffects.None, 0.99f);
                }
            }
            if (Game1.options.zoomButtons)
            {
                __instance.zoomInButton.draw(b, Color.White * (Game1.options.zoomLevel >= 1.25f ? 0.5f : 1f), 1f, 0);
                __instance.zoomOutButton.draw(b, Color.White * (Game1.options.zoomLevel <= 0.75f ? 0.5f : 1f), 1f, 0);
            }
            __instance.drawMoneyBox(b, -1, -1);
            b.Draw(Game1.mouseCursors, __instance.position + new Vector2(88f, 88f), new Rectangle?(new Rectangle(324, 477, 7, 19)), Color.White, (float)(3.14159265358979 + Math.Min(3.14159265358979, (double)(((float)adjustedTime + (float)Game1.gameTimeInterval / 7000f * 16.6f - 600f) / 2000f) * 3.14159265358979)), new Vector2(3f, 17f), 4f, SpriteEffects.None, 0.9f);
            return false;
        }


	}
}
