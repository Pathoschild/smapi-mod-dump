/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

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
