/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.IO;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Omegasis.Revitalize.Framework.Environment
{
    /// <summary>Deals with making night time darker in Stardew.</summary>
    public class DarkerNight
    {

        /// <summary>The calculated night color.</summary>
        private static Color CalculatedColor;

        /// <summary>Sets the color of darkness at night.</summary>
        public static void SetDarkerColor()
        {
            if (!RevitalizeModCore.Configs.worldConfigManager.darkerNightConfig.Enabled || Game1.player?.currentLocation == null)
                return;

            if (Game1.player.currentLocation.IsOutdoors && Game1.timeOfDay >= Game1.getStartingToGetDarkTime())
                Game1.outdoorLight = CalculatedColor;
        }

        /// <summary>Calculates how dark it should be a night.</summary>
        public static void CalculateDarkerNightColor()
        {
            if (!RevitalizeModCore.Configs.worldConfigManager.darkerNightConfig.Enabled || Game1.player?.currentLocation == null)
                return;

            //Calculate original lighting.
            if (Game1.timeOfDay >= Game1.getTrulyDarkTime())
            {
                float num = Math.Min(0.93f, (float)(0.75 + ((int)(Game1.timeOfDay - Game1.timeOfDay % 100 + Game1.timeOfDay % 100 / 10 * 16.6599998474121) - Game1.getTrulyDarkTime() + Game1.gameTimeInterval / 7000.0 * 16.6000003814697) * 0.000624999986030161));
                Game1.outdoorLight = (Game1.isRaining ? Game1.ambientLight : Game1.eveningColor) * num;
            }
            else if (Game1.timeOfDay >= Game1.getStartingToGetDarkTime())
            {
                float num = Math.Min(0.93f, (float)(0.300000011920929 + ((int)(Game1.timeOfDay - Game1.timeOfDay % 100 + Game1.timeOfDay % 100 / 10 * 16.6599998474121) - Game1.getStartingToGetDarkTime() + Game1.gameTimeInterval / 7000.0 * 16.6000003814697) * 0.00224999990314245));
                Game1.outdoorLight = (Game1.isRaining ? Game1.ambientLight : Game1.eveningColor) * num;
            }

            //ModCore.log("OUT: " + Game1.outdoorLight);

            int red = Game1.outdoorLight.R;

            if (Game1.player.currentLocation.IsOutdoors && Game1.timeOfDay >= Game1.getStartingToGetDarkTime())
                //Game1.ambientLight = Game1.ambientLight.GreyScaleAverage();
                CalculatedColor = Game1.ambientLight * ((red + 30) / 255f) * RevitalizeModCore.Configs.worldConfigManager.darkerNightConfig.DarknessIntensity;
        }
    }
}
