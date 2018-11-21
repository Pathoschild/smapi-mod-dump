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
