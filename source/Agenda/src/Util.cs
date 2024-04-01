/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/StardewValley-Agenda
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;

namespace MyAgenda
{
    internal class Util
    {
        // 昨天运气，昨天是否下雨，昨天姜岛是否下雨
        public static double previousLuckLevel = 0;
        public static bool MainlandRained = false, IslandRained = false;

        public static void drawMiddle(SpriteBatch b, string text, Rectangle box, Color color, SpriteFont font)
        {
            // 在一个框框中间画string
            Vector2 measured = font.MeasureString(text);
            int startX = (int)((box.Width - measured.X)/2);
            int startY = (int)((box.Height - measured.Y)/2);
            // 取中间
            b.DrawString(font, text, new Vector2(box.X + startX, box.Y + startY), color);
        }

        public static Vector2 drawStr(SpriteBatch b, string str, Rectangle rec, SpriteFont font, int start_x = 0)
        {
            // 画string的一个helper function。可以换行。返回最后一个字符的坐标
            int baseIndex = 0, ypos = rec.Y;
            // 遍历
            for (int i = 0; i < str.Length; i++)
            {
                // 获取字符宽度
                Vector2 measured = font.MeasureString(str.Substring(baseIndex, i - baseIndex));
                // 如果Y超了，罢工不画了
                if (measured.Y + ypos > rec.Y + rec.Height)
                {
                    measured.X = rec.X;
                    measured.Y = rec.Y;
                    return measured;
                }
                // 如果X超了，换行
                if (measured.X + start_x > rec.Width)
                {
                    b.DrawString(font, str.Substring(baseIndex, Math.Max(0, i - baseIndex - 1)), new Vector2(rec.X + start_x, ypos), Color.Black);
                    ypos += (int)measured.Y;
                    start_x = 0;
                    baseIndex = Math.Max(i - 1, 0);
                }
            }
            // 最后画一下还没画好的
            b.DrawString(font, str.Substring(baseIndex), new Vector2(rec.X + start_x, ypos), Color.Black);
            Vector2 finalPoint = font.MeasureString(str.Substring(baseIndex));
            finalPoint.Y = ypos;
            finalPoint.X += rec.X + start_x;
            return finalPoint;
        }

        public static byte examinHelper(int[] trigger)
        {
            // 小助手
            if (trigger[1] == 11) return 0xF0;
            return (trigger[1] == 1) ? (byte)0xFF : (byte)0xF0;
        }

        public static bool isWeatherRain(string weather)
        {
            // 康康这个是不是下雨天
            return weather == Game1.weather_rain || weather == Game1.weather_snow || weather == Game1.weather_lightning;
        }

        public static bool isRainTomorrow()
        {
            // 从 UIInfoSuite2 抄过来的不知道啥意思
            var date = new WorldDate(Game1.Date);
            ++date.TotalDays;
            var tomorrowWeather = Game1.IsMasterGame
                ? Game1.weatherForTomorrow
                : Game1.netWorldState.Value.WeatherForTomorrow;
            string weather = Game1.getWeatherModificationsForDate(date, tomorrowWeather);
            return isWeatherRain(weather);
        }

        public static bool isIslandRainTomorrow()
        {
            // 从 UIInfoSuite2 抄过来的不知道啥意思
            return isWeatherRain(Game1.netWorldState.Value.GetWeatherForLocation(LocationContexts.IslandId).weatherForTomorrow.Value);
        }

        public static bool isRainHere(string context)
        {
            //当前这里是否下雨
            //Trigger.monitor.Log($"context: {context}", LogLevel.Info);
            // 拿到当前季节
            var weather = Game1.netWorldState.Value.GetWeatherForLocation(context);
            //Trigger.monitor.Log($"weather: {weather.isRaining.Value}, {weather.isSnowing.Value}, {weather.isLightning.Value}", LogLevel.Info);
            // 康康是否为下雨下雪打雷
            return weather.isRaining.Value || weather.isSnowing.Value || weather.isLightning.Value;
        }

        /*
         * 高四位：是否触发
         * 低四位：是否删除
         */
        public static byte examinDate(int[] trigger)
        {
            // 如果任意为空，不合格
            if (trigger[0] == 0 || trigger[1] == 0 || trigger[2] == 0) return 0;
            // 明天的运气我咋知道
            if ((trigger[2] == 12 || trigger[2] == 13) && trigger[0] == 1) return 1;

            // 星期，直接比较
            if (trigger[2] > 0 && trigger[2] < 8)
                if((Game1.dayOfMonth-1) % 7 == trigger[2] - 1) return examinHelper(trigger);

            // 康康日期是啥
            switch (trigger[0])
            {
                // 前一天触发，查看明天数据。
                case 1:
                    if (trigger[2] == 8) return isRainTomorrow() ? examinHelper(trigger) : (byte)2;
                    if (trigger[2] == 9) return isIslandRainTomorrow() ? examinHelper(trigger) : (byte)3;
                    if (trigger[2] == 10) return isRainTomorrow() ? (byte)4 : examinHelper(trigger);
                    if (trigger[2] == 11) return isIslandRainTomorrow() ? (byte)5 : examinHelper(trigger);
                    break;

                // 当天，查看当日数据
                case 2:
                    if (trigger[2] == 8) return isRainHere(LocationContexts.DefaultId) ? examinHelper(trigger) : (byte)6;
                    if (trigger[2] == 9) return isRainHere(LocationContexts.IslandId) ? examinHelper(trigger) : (byte)7;
                    if (trigger[2] == 10) return isRainHere(LocationContexts.DefaultId) ? (byte)8 : examinHelper(trigger);
                    if (trigger[2] == 11) return isRainHere(LocationContexts.IslandId) ? (byte)9 : examinHelper(trigger);
                    if (trigger[2] == 12 && Game1.player.DailyLuck > 0.02) return examinHelper(trigger);
                    if (trigger[2] == 13 && Game1.player.DailyLuck < -0.02) return examinHelper(trigger);
                    break;

                //后一天触发，查看昨天数据
                case 3:
                    if (trigger[2] == 8) return MainlandRained ? examinHelper(trigger) : (byte)10;
                    if (trigger[2] == 9) return IslandRained ? examinHelper(trigger) : (byte)11;
                    if (trigger[2] == 10) return MainlandRained ? (byte)12 : examinHelper(trigger);
                    if (trigger[2] == 11) return IslandRained ? (byte)13 : examinHelper(trigger);
                    if (trigger[2] == 12 && previousLuckLevel > 0.02) return examinHelper(trigger);
                    if (trigger[2] == 13 && previousLuckLevel < -0.02) return examinHelper(trigger);
                    break;
            }

            return 14;
        }
    }
}
