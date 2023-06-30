/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using System;
using Microsoft.Xna.Framework;

namespace Survivalistic.Framework.Bars
{
    public class BarsInformations
    {
        public static float hunger_percentage;

        public static float thirst_percentage;

        public static Color hunger_color = new Color(1, .7f, 0);
        public static Color thirst_color = new Color(0, .7f, 1);

        public static void ResetStatus()
        {
            ModEntry.data.actual_hunger = ModEntry.data.max_hunger;
            ModEntry.data.actual_thirst = ModEntry.data.max_thirst;

            BarsUpdate.CalculatePercentage();
        }

        public static void NormalizeStatus()
        {
            if (ModEntry.data.actual_hunger < 0) ModEntry.data.actual_hunger = 0;
            if (ModEntry.data.actual_thirst < 0) ModEntry.data.actual_thirst = 0;

            if (ModEntry.data.actual_hunger > ModEntry.data.max_hunger) ModEntry.data.actual_hunger = ModEntry.data.max_hunger;
            if (ModEntry.data.actual_thirst > ModEntry.data.max_thirst) ModEntry.data.actual_thirst = ModEntry.data.max_thirst;

            BarsUpdate.CalculatePercentage();
        }

        public static Color GetOffsetHungerColor()
        {
            double maxHunger = ModEntry.data.max_hunger * 1.0;
            double currentHunger = ModEntry.data.actual_hunger * 1.0;
            double offset = currentHunger / maxHunger;

            Color color = hunger_color;
            color.R = Convert.ToByte(Math.Abs(offset - 1) * byte.MaxValue);
            color.G = Convert.ToByte(offset * color.G);
            color.B = Convert.ToByte(offset * color.B);

            return hunger_color;
        }

        public static Color GetOffsetThirstyColor()
        {
            double maxThirsty = ModEntry.data.max_thirst * 1.0;
            double currentThirsty = ModEntry.data.actual_thirst * 1.0;
            double offset = currentThirsty / maxThirsty;

            Color color = thirst_color;
            color.R = Convert.ToByte(Math.Abs(offset - 1) * byte.MaxValue);
            color.G = Convert.ToByte(offset * color.G);
            color.B = Convert.ToByte(offset * color.B);

            return thirst_color;
        }
    }
}