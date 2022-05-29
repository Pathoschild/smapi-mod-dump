/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using Microsoft.Xna.Framework;

namespace Survivalistic.Framework.Bars
{
    public class BarsInformations
    {
        public static float hunger_percentage;
        public static float thirst_percentage;

        public static Color hunger_color = new Color(207, 98, 7);
        public static Color thirst_color = new Color(13, 151, 151);

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
    }
}