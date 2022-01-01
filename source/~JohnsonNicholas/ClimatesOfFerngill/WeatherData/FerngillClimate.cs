/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using TwilightShards.Common;

namespace ClimatesOfFerngillRebuild
{
    public class FerngillClimate
    {
        public bool AllowRainInWinter;
        public bool AllowThunderSnow;
        public bool AllowSnowInFall;
        public double ChanceForNonNormalRain;
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
            foreach (FerngillClimateTimeSpan cts in fCTS)
                ClimateSequences.Add(new FerngillClimateTimeSpan(cts));
        }

        //climate access functions
        /// <summary>
        /// This function returns the general climate data for a day. It's meant if you want to do processing elsewhere.
        /// </summary>
        /// <param name="target">The day being looked at </param>
        /// <returns>The climate data.</returns>
        public FerngillClimateTimeSpan GetClimateForDate(SDate target)
        {
            foreach (FerngillClimateTimeSpan s in ClimateSequences)
            {
                SDate beginDate = new SDate(s.BeginDay, s.BeginSeason, target.Year);
                SDate endDate = new SDate(s.EndDay, s.EndSeason, target.Year);

                if (target >= beginDate && target <= endDate)
                    return s;
            }

            return default;
        }

        /// <summary>
        /// This function returns the temperatures for a day
        /// </summary>
        /// <param name="target">The day being looked at</param>
        /// <param name="dice">The PRNG object</param>
        /// <returns>The temperature range</returns>
        public RangePair GetTemperatures(SDate target, Random dice)
        {
            var weather = GetClimateForDate(target);
            var temps = new RangePair(weather.RetrieveTemp(dice, "lowtemp", target.Day), 
                                 weather.RetrieveTemp(dice, "hightemp", target.Day), true);
            ClimatesOfFerngill.Logger.Log($"We are gathering temperatures from the climate file. Temps is {temps.LowerBound:N3}, {temps.HigherBound:N3}");
            return temps;
        }

        public double GetStormOdds(SDate target,Random dice)
        {
            return GetClimateForDate(target).RetrieveOdds(dice, "storm", target.Day);
        }

        public double GetEveningFogOdds(SDate target)
        {
            return GetClimateForDate(target).EveningFogChance;
        }

    }
}

