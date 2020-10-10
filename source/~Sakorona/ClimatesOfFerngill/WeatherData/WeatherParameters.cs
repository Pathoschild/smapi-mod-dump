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
    public class WeatherParameters
    {
        public string WeatherType;
        public double BaseValue;
        public double ChangeRate;
        public double VariableLowerBound;
        public double VariableHigherBound;

        public WeatherParameters()
        {

        }

        public WeatherParameters(string wType, double bValue, double cRate, double vLowerBound, double vHigherBound)
        {
            this.WeatherType = wType;
            this.BaseValue = bValue;
            this.ChangeRate = cRate;
            this.VariableLowerBound = vLowerBound;
            this.VariableHigherBound = vHigherBound;
        }

        public WeatherParameters(WeatherParameters c)
        {
            this.WeatherType = c.WeatherType;
            this.BaseValue = c.BaseValue;
            this.ChangeRate = c.ChangeRate;
            this.VariableLowerBound = c.VariableLowerBound;
            this.VariableHigherBound = c.VariableHigherBound;
        }
    }
}
