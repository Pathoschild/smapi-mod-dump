/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Randomizer
{
    /// <summary>
    /// An extension of the Random class with extra commonly used features 
    /// </summary>
    public class RNG : Random
    {
        /// <summary>
        /// The only version of Random that we need is the one that uses a seed
        /// This is private because classes should instantiate this using the static functions below
        /// </summary>
        /// <param name="seed">The seed for the RNG</param>
        private RNG(int seed) : base(seed) { }

        /// <summary>
        /// Gets an RNG value based on the farm name and the given seed
        /// </summary>
        /// <param name="seed">The seed to use</param>
        /// <returns>The Random object</returns>
        public static RNG GetFarmRNG(string seed)
        {
            byte[] seedvar = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(Game1.player.farmName.Value + seed));
            int fullSeed = BitConverter.ToInt32(seedvar, 0);

            return new RNG(fullSeed);
        }

        /// <summary>
        /// Gets an RNG value based on the seed and the ingame day
        /// Essentially, this is a seed that changes once every day
        /// Seeded on the given string, the farm name, and the days played
        /// <param name="seed">The seed to use</param>
        /// </summary>
        /// <returns>The Random object</returns>
        public static RNG GetDailyRNG(string seed)
        {
            int time = Game1.Date.TotalDays;
            byte[] seedvar = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(Game1.player.farmName.Value + seed));
            int fullSeed = BitConverter.ToInt32(seedvar, 0) + time;

            return new RNG(fullSeed);
        }

        /// <summary>
        /// Gets an RNG value based on the seed and the ingame day
        /// Essentially, this is a seed that changes once a week (every Monday)
        /// Seeded on the given string, the farm name, and the days played
        /// </summary>
        /// <param name="seed">The seed to use</param>
        /// <returns>The Random object</returns>
        public static RNG GetWeeklyRNG(string seed)
        {
            int time = Game1.Date.TotalDays / 7;
            byte[] seedvar = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(Game1.player.farmName.Value + seed));
            int fullSeed = BitConverter.ToInt32(seedvar, 0) + time;

            return new RNG(fullSeed);
        }

        /// <summary>
        /// Gets a random boolean value
        /// </summary>
        /// <returns />
        public bool NextBoolean()
        {
            return Next(0, 2) == 0;
        }

        /// <summary>
        /// Gets a random boolean value
        /// </summary>
        /// <param name="percentage">The percentage of the boolean being true - 10 would be 10%, etc.</param>
        /// <returns />
        public bool NextBoolean(int percentage)
        {
            if (percentage < 0 || percentage > 100)
            {
                Globals.ConsoleWarn("Percentage is invalid (less than 0 or greater than 100)");
            }

            return Next(0, 100) < percentage;
        }

        /// <summary>
        /// Gets a random integer value + or - the given percentage (rounds up)
        /// ex) value of 10 with percentage of 50 returns a value between 5 and 15
        /// </summary>
        /// <param name="value">The base value</param>
        /// <param name="percentage">The percentage of the base value to use</param>
        /// <returns>The random value retrieved</returns>
        public int NextIntWithinPercentage(int value, int percentage)
        {
            var difference = (int)Math.Ceiling(value * ((double)percentage / 100));
            return NextIntWithinRange(value - difference, value + difference);
        }

        /// <summary>
        /// Gets a random value between the min and max value, inclusive
        /// </summary>
        /// <param name="min">The min value</param>
        /// <param name="max">The max value</param>
        /// <returns>The retrieved random value within min and max, inclusive</returns>
        public int NextIntWithinRange(int min, int max)
        {
            return Next(min, max + 1);
        }

        /// <summary>
        /// Gets a random value in the given range
        /// </summary>
        /// <param name="range">The range</param>
        /// <returns>The retrieved random value within the range, inclusive</returns>
        public int NextIntWithinRange(Range range)
        {
            return NextIntWithinRange(range.MinValue, range.MaxValue);
        }

        /// <summary>
        /// Gets a random value out of the given list
        /// </summary>
        /// <typeparam name="T">The type of the list</typeparam>
        /// <param name="list">The list</param>
        /// <returns />
        public T GetRandomValueFromList<T>(List<T> list)
        {
            return GetRandomValueFromListUsingRNG(list, this);
        }

        /// <summary>
        /// Gets a random value out of the given list using the given RNG value
        /// Separate here as a static function because some places want to use Game1.random
        /// </summary>
        /// <typeparam name="T">The type of the list</typeparam>
        /// <param name="list">The list</param>
        /// <param name="rng">The Random object to use</param>
        /// <returns />
        public static T GetRandomValueFromListUsingRNG<T>(List<T> list, Random rng)
        {
            if (list == null || list.Count == 0)
            {
                Globals.ConsoleError("Attempted to get a random value out of an empty list!");
                return default;
            }

            return list[rng.Next(list.Count)];
        }

        /// <summary>
        /// Gets a random value out of the given list and removes it
        /// </summary>
        /// <typeparam name="T">The type of the list</typeparam>
        /// <param name="list">The list</param>
        /// <returns />
        public T GetAndRemoveRandomValueFromList<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                Globals.ConsoleError("Attempted to get a random value out of an empty list!");
                return default;
            }
            int selectedIndex = Next(list.Count);
            T selectedValue = list[selectedIndex];
            list.RemoveAt(selectedIndex);
            return selectedValue;
        }

        /// <summary>
        /// Gets a random set of values form a list
        /// </summary>
        /// <typeparam name="T">The type of the list</typeparam>
        /// <param name="inputList">The list</param>
        /// <param name="numberOfvalues">The number of values to return</param>
        /// <param name="forceNumberOfValuesRNGCalls">
        /// Forces this to advance the RNG even if number of values is less than the list length
        /// This is for situations where different settings result in different lengths of lists
        /// </param>
        /// <returns>
        /// The randomly chosen values - might be less than the number of values if the list doesn't contain that many
        /// </returns>
        public List<T> GetRandomValuesFromList<T>(
            List<T> inputList,
            int numberOfvalues,
            bool forceNumberOfValuesRNGCalls = false)
        {
            List<T> listToChooseFrom = new(inputList); // Don't modify the original list
            List<T> randomValues = new();
            if (listToChooseFrom == null || listToChooseFrom.Count == 0)
            {
                Globals.ConsoleError("Attempted to get random values out of an empty list!");
                return randomValues;
            }

            int numberOfIterations = Math.Min(numberOfvalues, listToChooseFrom.Count);
            int i;
            for (i = 0; i < numberOfIterations; i++)
            {
                randomValues.Add(GetAndRemoveRandomValueFromList(listToChooseFrom));
            }

            // If we're forcing RNG to advance, we must call it for each item that's left
            if (forceNumberOfValuesRNGCalls)
            {
                for (; i < inputList.Count - 1; i++)
                {
                    Next();
                }
            }

            return randomValues;
        }
    }
}
