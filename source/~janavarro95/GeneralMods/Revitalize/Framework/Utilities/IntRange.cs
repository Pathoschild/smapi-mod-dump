/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace Revitalize.Framework.Utilities
{
    /// <summary>
    /// A class for dealing with integer value ranges.
    /// </summary>
    public class IntRange
    {
        /// <summary>
        /// The min value for the range.
        /// </summary>
        public int min;
        /// <summary>
        /// The max value for the range.
        /// </summary>
        public int max;

        public IntRange()
        {

        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="SingleValue">The single value to be tested on for min and max. Note that this will fail every test except for ContainsInclusive.</param>
        public IntRange(int SingleValue)
        {
            this.min = this.max = SingleValue;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Min">The min value.</param>
        /// <param name="Max">The max value.</param>
        public IntRange(int Min, int Max)
        {
            this.min = Min;
            this.max = Max;
        }

        /// <summary>
        /// Checks to see if the value is inside the range.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsInclusive(int value)
        {
            if (value >= this.min && value <= this.max) return true;
            else return false;
        }

        /// <summary>
        /// Checks to see if the value is greater/equal than the min but less than the max.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsExclusiveCeil(int value)
        {
            if (value >= this.min && value < this.max) return true;
            else return false;
        }
        /// <summary>
        /// Checks to see if the value is greater than the min and less/equal to the max.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsExclusiveFloor(int value)
        {
            if (value >= this.min && value < this.max) return true;
            else return false;
        }
        /// <summary>
        /// Checks to see if the value is inside of the range but not equal to min or max.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsExclusive(int value)
        {
            if (value > this.min && value < this.max) return true;
            else return false;
        }

        /// <summary>
        /// Returns an int value within the range of min and max inclusive.
        /// </summary>
        /// <returns></returns>
        public int getRandomInclusive()
        {
            int number = Game1.random.Next(this.min, this.max + 1);
            return number;
        }

        /// <summary>
        /// Returns an int value within the range of min and max exclusive.
        /// </summary>
        /// <returns></returns>
        public int getRandomExclusive()
        {
            int number = Game1.random.Next(this.min, this.max);
            return number;
        }
    }
}
