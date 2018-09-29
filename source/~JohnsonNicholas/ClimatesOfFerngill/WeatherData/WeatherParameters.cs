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
