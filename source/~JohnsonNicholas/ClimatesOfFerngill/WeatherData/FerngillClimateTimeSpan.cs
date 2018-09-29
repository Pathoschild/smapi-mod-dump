using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwilightShards.Common;

namespace ClimatesOfFerngillRebuild
{
    public class FerngillClimateTimeSpan
    {
        public string BeginSeason;
        public string EndSeason;
        public int BeginDay;
        public int EndDay;
        public List<WeatherParameters> WeatherChances;

        public FerngillClimateTimeSpan()
        {
            this.WeatherChances = new List<WeatherParameters>();
        }

        public FerngillClimateTimeSpan(string BeginSeason, string EndSeason, int BeginDay, int EndDay, List<WeatherParameters> wp)
        {
            this.BeginSeason = BeginSeason;
            this.EndSeason = EndSeason;

            this.BeginDay = BeginDay;
            this.EndDay = EndDay;
            this.WeatherChances = new List<WeatherParameters>();

            foreach (WeatherParameters w in wp)
            {
                this.WeatherChances.Add(new WeatherParameters(w));
            }

        }

        public FerngillClimateTimeSpan(FerngillClimateTimeSpan CTS)
        {
            this.BeginSeason = CTS.BeginSeason;
            this.EndSeason = CTS.EndSeason;
            this.BeginDay = CTS.BeginDay;
            this.EndDay = CTS.EndDay;

            this.WeatherChances = new List<WeatherParameters>();
            foreach (WeatherParameters w in CTS.WeatherChances)
                this.WeatherChances.Add(new WeatherParameters(w));
        }

        public void AddWeatherChances(WeatherParameters wp)
        {
            if (this.WeatherChances is null)
                WeatherChances = new List<WeatherParameters>();

            WeatherChances.Add(new WeatherParameters(wp));
        }

        public double RetrieveOdds(MersenneTwister dice, string weather, int day, StringBuilder Output)
        {
            double Odd = 0;

            List<WeatherParameters> wp = this.WeatherChances.Where(w => w.WeatherType == weather).ToList<WeatherParameters>();

            if (wp.Count == 0)
            {
                Output.Append($"{weather} weather is not found in this time span.");
                return 0;
            }

            Output.Append($"Retrieved data for {weather} on {day} with Base Value {wp[0].BaseValue} and Change Rate {wp[0].ChangeRate} " +
                          $"with the variable bound of ({wp[0].VariableLowerBound} , {wp[0].VariableHigherBound}) {Environment.NewLine}");

            Odd = wp[0].BaseValue + (wp[0].ChangeRate * day);
            RangePair range = new RangePair(wp[0].VariableLowerBound, wp[0].VariableHigherBound);
            Odd = Odd + range.RollInRange(dice);

            //sanity check.
            if (Odd < 0) Odd = 0;
            if (Odd > 1) Odd = 1;

            return Odd;
        }

        public double RetrieveTemp(MersenneTwister dice, string temp, int day, StringBuilder Output)
        {
            double Temp = 0;
            List<WeatherParameters> wp = this.WeatherChances.Where(w => w.WeatherType == temp).ToList<WeatherParameters>();

            if (wp.Count == 0)
            {
                Output.Append($"{temp} weather is not found in this time span.");
                return 0;
            }

            Output.Append($"Retrieved data for {temp} on {day} with Base Value {wp[0].BaseValue} and Change Rate {wp[0].ChangeRate} " +
                          $"with the variable bound of ({wp[0].VariableLowerBound} , {wp[0].VariableHigherBound}) {Environment.NewLine}");

            Temp = wp[0].BaseValue + (wp[0].ChangeRate * day);
            RangePair range = new RangePair(wp[0].VariableLowerBound, wp[0].VariableHigherBound);
            Temp = Temp + range.RollInRange(dice);

            return Temp;
        }
    }
}
