/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using StardewValley;
using System;

namespace ClimatesOfFerngillRebuild.Patches
{
    class GameClockPatch
    {
        public static void Postfix()
        {
            bool ShouldDarken = (Game1.isRaining || (ClimatesOfFerngill.Conditions.ContainsCondition(CurrentWeather.Overcast)));

            if (Game1.timeOfDay >= Game1.getTrulyDarkTime())
            {
                float num = Math.Min(0.93f, (float)(0.75 + ((int)(Game1.timeOfDay - Game1.timeOfDay % 100 + (Game1.timeOfDay % 100 / 10) * 16.6599998474121) - Game1.getTrulyDarkTime() + (double)Game1.gameTimeInterval / 7000.0 * 16.6000003814697) * 0.000624999986030161));
                Game1.outdoorLight = (ShouldDarken ? Game1.ambientLight : Game1.eveningColor) * num;
            }
            else if (Game1.timeOfDay >= Game1.getStartingToGetDarkTime())
            {
                float num = Math.Min(0.93f, (float)(0.300000011920929 + (((int)((Game1.timeOfDay - Game1.timeOfDay % 100) + (Game1.timeOfDay % 100 / 10) * 16.6599998474121) - Game1.getStartingToGetDarkTime()) + (double)Game1.gameTimeInterval / 7000.0 * 16.6000003814697) * 0.00224999990314245));
                Game1.outdoorLight = (ShouldDarken ? Game1.ambientLight : Game1.eveningColor) * num;
            }
            else if (ShouldDarken)
                Game1.outdoorLight = Game1.ambientLight * 0.3f;
        }
    }
}
