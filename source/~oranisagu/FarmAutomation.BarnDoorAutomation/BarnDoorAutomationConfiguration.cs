/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/oranisagu/SDV-FarmAutomation
**
*************************************************/

using FarmAutomation.Common;

namespace FarmAutomation.BarnDoorAutomation
{
    /// <summary>
    /// the json serializable configuration for the barn door automation mod
    /// </summary>
    public class BarnDoorAutomationConfiguration : ConfigurationBase
    {
        public int OpenDoorsAfter { get; set; }
        public int CloseDoorsAfter { get; set; }
        public int FirstDayInSpringToOpen { get; set; }

        public override void InitializeDefaults()
        {
            EnableMod = true;
            FirstDayInSpringToOpen = 1;
            OpenDoorsAfter = 600;
            CloseDoorsAfter = 1930;
        }
    }
}
