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
using Newtonsoft.Json;
using Omegasis.StardustCore.Networking;
using StardewValley;

namespace Omegasis.Revitalize.Framework.Utilities.Ranges
{
    /// <summary>
    /// A class for dealing with integer value ranges.
    /// </summary>
    public class IntRange : NetObject
    {
        [JsonIgnore]
        /// <summary>
        /// The min value for the range.
        /// </summary>
        public readonly NetInt min = new NetInt();
        [JsonIgnore]
        /// <summary>
        /// The max value for the range.
        /// </summary>
        public readonly NetInt max = new NetInt();

        [JsonProperty("min")]
        public int Min
        {
            get { return this.min.Value; }
            set { this.min.Value = value; }
        }

        [JsonProperty("max")]
        public int Max
        {
            get { return this.max.Value; }
            set { this.max.Value = value; }
        }

        public IntRange() : this(0, 0)
        {

        }



        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Min">The min value.</param>
        /// <param name="Max">The max value.</param>
        public IntRange(int Min, int Max)
        {
            this.Min = Min;
            this.Max = Max;

            this.initializeNetFields();
        }

        protected override void initializeNetFields()
        {
            base.initializeNetFields();
            this.NetFields.AddFields(this.min, this.max);
        }

        /// <summary>
        /// Checks to see if the value is inside the range.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsInclusive(int value)
        {
            if (value >= this.Min && value <= this.Max) return true;
            else return false;
        }

        /// <summary>
        /// Checks to see if the value is greater/equal than the min but less than the max.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsExclusiveCeil(int value)
        {
            if (value >= this.Min && value < this.Max) return true;
            else return false;
        }
        /// <summary>
        /// Checks to see if the value is greater than the min and less/equal to the max.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsExclusiveFloor(int value)
        {
            if (value >= this.Min && value < this.Max) return true;
            else return false;
        }
        /// <summary>
        /// Checks to see if the value is inside of the range but not equal to min or max.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsExclusive(int value)
        {
            if (value > this.Min && value < this.Max) return true;
            else return false;
        }

        /// <summary>
        /// Returns an int value within the range of min and max inclusive.
        /// </summary>
        /// <returns></returns>
        public int getRandomInclusive()
        {
            int number = Game1.random.Next(this.Min, this.Max + 1);
            return number;
        }

        /// <summary>
        /// Returns an int value within the range of min and max exclusive.
        /// </summary>
        /// <returns></returns>
        public int getRandomExclusive()
        {
            int number = Game1.random.Next(this.Min, this.Max);
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
            writer.Write(this.min.Value);
            writer.Write(this.max.Value);
        }
    }
}
