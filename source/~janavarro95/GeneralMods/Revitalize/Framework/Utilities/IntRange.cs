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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netcode;
using StardewValley;

namespace Omegasis.Revitalize.Framework.Utilities
{
    /// <summary>
    /// A class for dealing with integer value ranges.
    /// </summary>
    public class IntRange
    {
        /// <summary>
        /// The min value for the range.
        /// </summary>
        public readonly NetInt min = new NetInt();
        /// <summary>
        /// The max value for the range.
        /// </summary>
        public readonly NetInt max = new NetInt();

        public IntRange()
        {

        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="SingleValue">The single value to be tested on for min and max. Note that this will fail every test except for ContainsInclusive.</param>
        public IntRange(int SingleValue)
        {
            this.min.Value = this.max.Value = SingleValue;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Min">The min value.</param>
        /// <param name="Max">The max value.</param>
        public IntRange(int Min, int Max)
        {
            this.min.Value = Min;
            this.max.Value = Max;
        }

        public virtual List<INetSerializable> getNetFields()
        {
            return new List<INetSerializable>()
            {
                this.min,
                this.max
            };
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

        public virtual IntRange readIntRange(BinaryReader reader)
        {
            this.min.Value = reader.ReadInt32();
            this.max.Value = reader.ReadInt32();
            return this;
        }

        public virtual void writeIntRange(BinaryWriter writer)
        {
            writer.Write(this.min);
            writer.Write(this.max);
        }
    }
}
