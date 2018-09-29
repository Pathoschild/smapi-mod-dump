namespace ClimatesOfFerngillRebuild
{
    internal class ClimatesOfFerngillApi
    {
        private SDVMoon Termina;
        private WeatherConditions CurrentConditions;
        private WeatherConfig ModConfig;
        private StaminaDrain StaminaManager;

        //Constructor
        internal ClimatesOfFerngillApi(SDVMoon moon, WeatherConditions cond, StaminaDrain manager, WeatherConfig config)
        {
            Termina = moon;
            CurrentConditions = cond;
            StaminaManager = manager;
            ModConfig = config;
        }

        MoonPhase GetCurrentMoonPhase()
        {
            return Termina.CurrentPhase;
        }

        bool IsFarmerSick()
        {
            return StaminaManager.IsSick();
        }

        CurrentWeather GetWeatherConditions()
        {
            return CurrentConditions.GetCurrentConditions();
        }

        bool IsFoggyOutside()
        {
            foreach (ISDVWeather w in CurrentConditions.GetWeatherMatchingType("Fog"))
            {
                if (w.IsWeatherVisible)
                    return true;
            }

            return false;            
        }

        bool HasPrecip()
        {
            return CurrentConditions.HasPrecip();
        }

    }
}
