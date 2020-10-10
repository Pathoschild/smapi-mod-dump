/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

namespace ClimatesOfFerngillRebuild
{
    public class WeatherData
    {
        public WeatherIcon Icon { get; set; }
        public WeatherIcon IconBasic { get; set; }
        public string ConditionName { get; set; }
        public string ConditionDescDay { get; set; }
        public string ConditionDescNight { get; set; }
        
        public WeatherData()
        {

        }

        public WeatherData(WeatherIcon Icon, WeatherIcon IconBasic, string CondName, string CondDesc, string CondDescNight = null)
        {
            this.Icon = Icon;
            this.IconBasic = IconBasic;
            ConditionName = CondName;
            ConditionDescDay = CondDesc;
            ConditionDescNight = CondDescNight;
        }

        public string GetConditionString(bool IsNight)
        {
            return IsNight && !string.IsNullOrEmpty(ConditionDescNight) ? ConditionDescNight : ConditionDescDay;
        }
    }
}
