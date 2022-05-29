/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

namespace Survivalistic.Framework.Bars
{
    public class BarsUpdate
    {
        public static void UpdateBarsInformation()
        {
            if (ModEntry.data.actual_hunger > 0) ModEntry.data.actual_hunger -= BarsDatabase.hunger_velocity;
            else ModEntry.data.actual_hunger = 0;

            if (ModEntry.data.actual_thirst > 0) ModEntry.data.actual_thirst -= BarsDatabase.thirst_velocity;
            else ModEntry.data.actual_thirst = 0;
        }

        public static void CalculatePercentage()
        {
            BarsInformations.hunger_percentage = (ModEntry.data.actual_hunger / ModEntry.data.max_hunger) * 168;
            BarsInformations.thirst_percentage = (ModEntry.data.actual_thirst / ModEntry.data.max_thirst) * 168;
        }
    }
}
