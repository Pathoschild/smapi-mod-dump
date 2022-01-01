/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using TwilightShards.Common;

namespace ClimatesOfFerngillRebuild
{
    public class FerngillClimateTimeSpan
    {
        public string BeginSeason;
        public string EndSeason;
        public int BeginDay;
        public int EndDay;
        public double EveningFogChance;
        public double HeatwaveChance;
        public double ChillwaveChance;
        public List<WeatherParameters> WeatherChances;
        public List<WeatherSystems> SystemChances;

        public FerngillClimateTimeSpan()
        {
            this.WeatherChances = new List<WeatherParameters>();
            this.SystemChances = new List<WeatherSystems>();
        }

        public FerngillClimateTimeSpan(string BeginSeason, string EndSeason, int BeginDay, int EndDay, double Heatwave, double Chillwave, List<WeatherParameters> wp, List<WeatherSystems> ws)
        {
            this.BeginSeason = BeginSeason;
            this.EndSeason = EndSeason;

            this.HeatwaveChance = Heatwave;
            this.ChillwaveChance = Chillwave;

            this.BeginDay = BeginDay;
            this.EndDay = EndDay;
            this.WeatherChances = new List<WeatherParameters>();

            foreach (WeatherParameters w in wp)
            {
                this.WeatherChances.Add(new WeatherParameters(w));
            }

            this.SystemChances = new List<WeatherSystems>();
            foreach (WeatherSystems w in ws)
            {
                this.SystemChances.Add(new WeatherSystems(w));
            }
        }

        public FerngillClimateTimeSpan(FerngillClimateTimeSpan CTS)
        {
            this.BeginSeason = CTS.BeginSeason;
            this.EndSeason = CTS.EndSeason;
            this.BeginDay = CTS.BeginDay;
            this.EndDay = CTS.EndDay;
            this.ChillwaveChance = CTS.ChillwaveChance;
            this.HeatwaveChance = CTS.HeatwaveChance;

            this.WeatherChances = new List<WeatherParameters>();
            foreach (WeatherParameters w in CTS.WeatherChances)
                this.WeatherChances.Add(new WeatherParameters(w));

            this.SystemChances = new List<WeatherSystems>();
            foreach (WeatherSystems w in CTS.SystemChances)
                this.SystemChances.Add(new WeatherSystems(w));
        }

        public void AddWeatherChances(WeatherParameters wp)
        {
            if (this.WeatherChances is null)
                WeatherChances = new List<WeatherParameters>();

            WeatherChances.Add(new WeatherParameters(wp));
        }

        public RangePair GetTemperatures(Random dice, int day)
        {
            var temps = new RangePair(RetrieveTemp(dice, "lowtemp", day), RetrieveTemp(dice, "hightemp", day), true);
            ClimatesOfFerngill.Logger.Log($"We are gathering temperatures from the climate file. Temps is {temps.LowerBound}, {temps.HigherBound}");
            return temps;
        }

        public double RetrieveOdds(Random dice, string weather, int day, bool EnforceHigherOverLower = true)
        {
            double Odd = 0;

            List<WeatherParameters> wp = this.WeatherChances.Where(w => w.WeatherType == weather).ToList();

            if (wp.Count == 0)
              return 0;

            Odd = wp[0].BaseValue + (wp[0].ChangeRate * day);
            RangePair range = new RangePair(wp[0].VariableLowerBound, wp[0].VariableHigherBound, EnforceHigherOverLower);
            Odd += range.RollInRange(dice);

            //sanity check.
            if (Odd < 0) Odd = 0;
            if (Odd > 1) Odd = 1;

            return Odd;
        }

        public double RetrieveSystemOdds(string weather)
        {
            List<WeatherSystems> ws = this.SystemChances.Where(w => w.WeatherType == weather).ToList();
            //this.WeatherChances.Where(w => w.WeatherType == weather).ToList();

            if (ws.Count == 0)
                return 0;

            return ws[0].TypeChances;
        }

        public double RetrieveMeanTemp(string weat,int day)
        {
            double Temp = 0;
            List<WeatherParameters> wp = this.WeatherChances.Where(w => w.WeatherType == weat).ToList<WeatherParameters>();

            if (wp.Count == 0)
                return 0;

            Temp = wp[0].BaseValue + (wp[0].ChangeRate * day);

            return Temp;
        }

        public double RetrieveMaxTemp(string weat, int day)
        {
            double Temp = 0;
            List<WeatherParameters> wp = this.WeatherChances.Where(w => w.WeatherType == weat).ToList<WeatherParameters>();

            if (wp.Count == 0)
                return 0;

            Temp = wp[0].BaseValue + (wp[0].ChangeRate * day);
            Temp += wp[0].VariableHigherBound;

            return Temp;
        }

        public double RetrieveMinTemp(string weat, int day)
        {
            double Temp = 0;
            List<WeatherParameters> wp = this.WeatherChances.Where(w => w.WeatherType == weat).ToList<WeatherParameters>();

            if (wp.Count == 0)
                return 0;

            Temp = wp[0].BaseValue + (wp[0].ChangeRate * day);
            Temp += wp[0].VariableLowerBound;

            return Temp;
        }

        public double RetrieveTemp(Random dice, string temp, int day, bool EnforceHigherOverLower = true)
        {
            double Temp = 0;
            List<WeatherParameters> wp = this.WeatherChances.Where(w => w.WeatherType == temp).ToList<WeatherParameters>();

            if (wp.Count == 0)
                return 0;

            Temp = wp[0].BaseValue + (wp[0].ChangeRate * day);
            RangePair range = new RangePair(wp[0].VariableLowerBound, wp[0].VariableHigherBound, EnforceHigherOverLower);
            Temp += range.RollInRange(dice);

            return Temp;
        }
    }
}
