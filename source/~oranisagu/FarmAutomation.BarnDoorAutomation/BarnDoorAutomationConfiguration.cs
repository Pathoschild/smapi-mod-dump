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
