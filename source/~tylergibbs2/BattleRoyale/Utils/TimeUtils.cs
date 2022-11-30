/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace BattleRoyale.Utils
{
    internal class TimeUtils
    {
        // Courtesy of https://github.com/CJBok/SDV-Mods/blob/master/CJBCheatsMenu/Framework/Cheats/Time/SetTimeCheat.cs
        private static void SafelySetTime(int time)
        {
            // move time back
            int intervals = Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay, time) / 10;
            if (intervals > 0)
            {
                for (int i = 0; i < intervals; i++)
                    Game1.performTenMinuteClockUpdate();
            }
            else if (intervals < 0)
            {
                for (int i = 0; i > intervals; i--)
                {
                    Game1.timeOfDay = Utility.ModifyTime(Game1.timeOfDay, -20); // offset 20 mins so game updates to next interval
                    Game1.performTenMinuteClockUpdate();
                }
            }

            // reset ambient light
            // White is the default non-raining color. If it's raining or dark out, UpdateGameClock
            // below will update it automatically.
            Game1.outdoorLight = Color.White;
            Game1.ambientLight = Color.White;

            // run clock update (to correct lighting, etc)
            Game1.gameTimeInterval = 0;
            Game1.UpdateGameClock(Game1.currentGameTime);
        }

        public static void SetTime(string season, int time)
        {
            Game1.currentSeason = season;

            SafelySetTime(time);

            Game1.outdoorLight = Color.White;
            Game1.ambientLight = Color.White;

            Game1.UpdateGameClock(Game1.currentGameTime);

            Game1.setGraphicsForSeason();
        }
    }
}
