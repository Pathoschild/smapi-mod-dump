/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ForageFantasy
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewValley;
    using StardewValley.Menus;
    using System;

    internal class MushroomTapperCalendar
    {
        private static readonly Rectangle redMushroom = new(192, 272, 16, 16);
        private static readonly Rectangle purpleMushroom = new(224, 272, 16, 16);
        private static readonly Rectangle commonMushroom = new(320, 256, 16, 16);

        private static ForageFantasyConfig config;

        internal static void ApplyPatches(ForageFantasyConfig forageFantasyConfig, Harmony harmony)
        {
            config = forageFantasyConfig;

            harmony.Patch(
               original: AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new Type[] { typeof(SpriteBatch) }),
               postfix: new HarmonyMethod(typeof(MushroomTapperCalendar), nameof(Draw_Postfix)));
        }

        public static void Draw_Postfix(Billboard __instance, SpriteBatch b, bool ___dailyQuestBoard)
        {
            if (___dailyQuestBoard || !config.MushroomTapperCalendar)
            {
                return;
            }

            if (config.TapperDaysNeededChangesEnabled && config.MushroomTreeTappersConsistentDaysNeeded)
            {
                for (int day = 2; day <= 28; day += 2)
                {
                    var toDraw = GetConsistentRecommendedHarvestCalendar(Game1.season, day);

                    if (toDraw != Rectangle.Empty)
                    {
                        DrawCalendarMushroom(__instance, b, toDraw, day);
                    }
                }
            }
            else
            {
                if (Game1.season == Season.Fall)
                {
                    for (int day = 1; day <= 28; day++)
                    {
                        Rectangle toDraw = (day is 11 or 21) ? purpleMushroom : redMushroom;

                        DrawCalendarMushroom(__instance, b, toDraw, day);
                    }
                }
                else
                {
                    for (int day = 2; day <= 28; day += 2)
                    {
                        var toDraw = GetBaseGameRecommendedHarvestCalendar(Game1.season, day);

                        if (toDraw != Rectangle.Empty)
                        {
                            DrawCalendarMushroom(__instance, b, toDraw, day);
                        }
                    }
                }
            }
        }

        private static void DrawCalendarMushroom(Billboard billboard, SpriteBatch b, Rectangle toDraw, int day)
        {
            Utility.drawWithShadow(b, Game1.objectSpriteSheet, new Vector2((float)(billboard.calendarDays[day - 1].bounds.X + 95), (float)(billboard.calendarDays[day - 1].bounds.Y + 5) - Game1.dialogueButtonScale / 2f), toDraw, Color.White, 0f, Vector2.Zero, 2f, false, 1f, -1, -1, 0.35f);
        }

        private static Rectangle GetBaseGameRecommendedHarvestCalendar(Season season, int day)
        {
            if (season == Season.Fall)
            {
                return (day is 11 or 21) ? purpleMushroom : redMushroom;
            }

            if (season is Season.Spring or Season.Summer || (season is Season.Winter && ForageFantasy.MushroomTreeTapperWorksInWinter))
            {
                switch (day)
                {
                    case 12:
                    case 22:
                        return purpleMushroom;

                    case 14:
                        return redMushroom;

                    case 24:
                        return season != Season.Summer ? redMushroom : Rectangle.Empty;

                    case 2:
                        return season == Season.Winter ? redMushroom : commonMushroom;

                    case 4:
                    case 6:
                    case 8:
                    case 10:
                    case 16:
                    case 18:
                    case 20:
                        return commonMushroom;

                    case 26:
                    case 28:
                        return season != Season.Summer ? commonMushroom : Rectangle.Empty;
                }
            }

            return Rectangle.Empty;
        }

        private static Rectangle GetConsistentRecommendedHarvestCalendar(Season season, int day)
        {
            if (season is Season.Fall)
            {
                return (day is 2 or 12 or 22) ? purpleMushroom : redMushroom;
            }

            if (season is Season.Spring or Season.Summer || (season is Season.Winter && ForageFantasy.MushroomTreeTapperWorksInWinter))
            {
                if (day is 2 or 12 or 22)
                {
                    return purpleMushroom;
                }
                else if (day is 4 or 14 or 24)
                {
                    return redMushroom;
                }
                else
                {
                    return commonMushroom;
                }
            }

            return Rectangle.Empty;
        }
    }
}