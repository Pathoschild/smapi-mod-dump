/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-CombineMachines
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombineMachines.Helpers
{
    public static class RNGHelpers
    {
        public static readonly Random Randomizer = new Random();
        /// <summary>Returns true if a randomly generated number between 0.0 and 1.0 is less than or equal to the given ChanceOfSuccess</summary>
        /// <param name="ChanceOfSuccess">A value between 0.0 and 1.0. EX: if 0.7, there is a 70% chance that this function returns true.</param>
        public static bool RollDice(double ChanceOfSuccess)
        {
            return Randomizer.NextDouble() <= ChanceOfSuccess;
        }

        /// <summary>Returns a random number between the given Minimum and Maximum values.</summary>
        public static double GetRandomNumber(double Minimum, double Maximum)
        {
            return Randomizer.NextDouble() * (Maximum - Minimum) + Minimum;
        }

        /// <summary>Rounds the given double up or down to an integer. The result is more likely to be rounded to whichever is closer.<para/>
        /// EX: If Value=4.3, there is a 70% chance of rounding down to 4, 30% chance of rounding up to 5.</summary>
        public static int WeightedRound(double Value)
        {
            int BaseAmount = (int)Value;
            double RoundUpChance = Value - BaseAmount;
            int NewValue = BaseAmount + Convert.ToInt32(RollDice(RoundUpChance));
            return NewValue;
        }
    }
}
