/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

namespace Randomizer
{
    /// <summary>
    /// Represents a range of values - used for possible ranges of randomly generated values
    /// </summary>
    public class Range
	{
		public int MinValue { get; set; }
		public int MaxValue { get; set; }

		/// <summary>
		/// Constructor - has safety checks for whether the min and max values are correct
		/// </summary>
		/// <param name="minValue">The minimum value in the range</param>
		/// <param name="maxValue">The maximum value in the range</param>
		public Range(int minValue, int maxValue)
		{
			if (minValue < maxValue)
			{
				MinValue = minValue;
				MaxValue = maxValue;
			}

			else
			{
				MinValue = maxValue;
				MaxValue = minValue;
			}
		}

        /// <summary>
        /// Gets a random value between the min and max value, inclusive
        /// </summary>
        /// <param name="rng">The RNG object to use</param>
        /// <returns />
        public int GetRandomValue(RNG rng)
		{
            return rng.NextIntWithinRange(this);
		}
    }
}
