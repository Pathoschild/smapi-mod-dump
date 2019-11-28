namespace ClimatesOfFerngillRebuild
{
    public class WeatherSystems
    {
        public string WeatherType;
        public double TypeChances;

        public WeatherSystems()
        {

        }

        public WeatherSystems(string type, double pref)
        {
            WeatherType = type;
            TypeChances = pref;
        }

        public WeatherSystems(WeatherSystems ws)
        {
            this.TypeChances = ws.TypeChances;
            this.WeatherType = ws.WeatherType;
        }
    }
}
