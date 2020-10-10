/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kevinforrestconnors/RealisticFishing
**
*************************************************/

using System;
using System.Collections.Generic;

namespace RealisticFishing
{
    public static class EvolutionHelpers
    {

        private static Random rand = new Random();

        public static float MutationRate = 2f;


        public static double GetMutatedFishLength(double fishLength, int minLength, int maxLength) {
            
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = fishLength + EvolutionHelpers.MutationRate * randStdNormal; // mean + stdDev * randStdNormal   random normal(mean,stdDev^2)

            if (randNormal > maxLength)
            {
                return maxLength;
            }
            else if (randNormal < minLength)
            {
                return minLength;
            }
            else
            {
                return randNormal;
            }

        }
    }
}
