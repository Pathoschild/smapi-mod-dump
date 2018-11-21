using StardewModdingAPI.Utilities;
using System.Collections.Generic;
using System.Text;
using TwilightShards.Common;

namespace ClimatesOfFerngillRebuild
{
    public class FerngillClimate
    {
        public bool AllowRainInWinter;
        public bool AllowThunderSnow;
        public bool AllowSnowInFall;
        public List<string> LocationsAffected;
        public List<FerngillClimateTimeSpan> ClimateSequences;

        //constructor
        public FerngillClimate()
        {
            ClimateSequences = new List<FerngillClimateTimeSpan>();
        }

        //constructor
        public FerngillClimate(List<FerngillClimateTimeSpan> fCTS)
        {
            ClimateSequences = new List<FerngillClimateTimeSpan>();
            foreach (FerngillClimateTimeSpan CTS in fCTS)
                ClimateSequences.Add(new FerngillClimateTimeSpan(CTS));
        }

        //climate access functions
        /// <summary>
        /// This function returns the general climate data for a day. It's meant if you want to do processing elsewhere.
        /// </summary>
        /// <param name="Target">The day being looked at </param>
        /// <returns>The climate data.</returns>
        public FerngillClimateTimeSpan GetClimateForDate(SDate Target)
        {
            foreach (FerngillClimateTimeSpan s in ClimateSequences)
            {
                SDate BeginDate = new SDate(s.BeginDay, s.BeginSeason, Target.Year);
                SDate EndDate = new SDate(s.EndDay, s.EndSeason, Target.Year);

                if (Target >= BeginDate && Target <= EndDate)
                    return s;
            }

            return default(FerngillClimateTimeSpan);
        }

        /// <summary>
        /// This function returns the temperatures for a day
        /// </summary>
        /// <param name="Target">The day being looked at</param>
        /// <returns>The temperature range</returns>
        public RangePair GetTemperatures(SDate Target, MersenneTwister dice)
        {
            var Weather = GetClimateForDate(Target);
            return new RangePair(Weather.RetrieveTemp(dice, "lowtemp", Target.Day), 
                                 Weather.RetrieveTemp(dice, "hightemp", Target.Day));
        }

        public double GetStormOdds(SDate Target, MersenneTwister dice)
        {
            return GetClimateForDate(Target).RetrieveOdds(dice, "storm", Target.Day);
        }

        public double GetEveningFogOdds(SDate Target, MersenneTwister dice, StringBuilder Debug)
        {
            return GetClimateForDate(Target).EveningFogChance;
        }

    }
}

