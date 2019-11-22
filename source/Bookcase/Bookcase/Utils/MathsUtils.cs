using Bookcase;

namespace Bookcase.Utils {

    public class MathsUtils {

        /// <summary>
        /// Checks if a double value is within a range. This method is inclusive of the range limits.
        /// </summary>
        /// <param name="min">The lowest possible value.</param>
        /// <param name="max">The highest possible value.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>Whether or not the value was within the inclusive range.</returns>
		public static bool InRange(double min, double max, double value) {

            return value <= max && value >= min;
        }

        /// <summary>
        /// Runs a random percentage check.
        /// </summary>
        /// <param name="percent">The percentage to check.</param>
        /// <returns>Whether or not the check random check was successful.</returns>
		public static bool TryPercentage(double percent) {

            return BookcaseMod.random.NextDouble() < percent;
        }
    }
}
