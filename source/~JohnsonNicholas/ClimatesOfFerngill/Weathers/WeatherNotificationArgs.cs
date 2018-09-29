using System;

namespace ClimatesOfFerngillRebuild
{
    public class WeatherNotificationArgs : EventArgs
    {
        public bool Present { get; private set; }
        public string Weather { get; private set; }

        public WeatherNotificationArgs(string weather, bool present)
        {
            Weather = weather;
            Present = present;
        }
    }
}
