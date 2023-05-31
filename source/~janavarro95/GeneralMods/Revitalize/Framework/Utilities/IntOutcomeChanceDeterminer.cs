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
using Omegasis.Revitalize.Framework.Utilities.Ranges;

namespace Omegasis.Revitalize.Framework.Utilities
{
    /// <summary>
    /// A utility class for determining if a value is within a valid range. If so, return the value stored by this object.
    /// </summary>
    public class IntOutcomeChanceDeterminer
    {
        /// <summary>
        /// The chance range for if this value should occur. For example a range of 0-20 means there is a 20% chance a range check of 1-100 (inclusive) passes and retuns the value determined by <see cref="getValueIfInInclusiveBounds(int)"/> For 100% chance always do a range from 0-100.
        /// </summary>
        public DoubleRange validRangeForChance;
        /// <summary>
        /// The returned value if the <see cref="getValueIfInInclusiveBounds(int)"/> returns true.
        /// </summary>
        public IntRange outcomeValue;

        public IntOutcomeChanceDeterminer()
        {

        }

        public IntOutcomeChanceDeterminer(DoubleRange validRangeForChance, int outcomeValue)
        {
            this.validRangeForChance = validRangeForChance;
            this.outcomeValue = new IntRange(outcomeValue,outcomeValue);
        }

        public IntOutcomeChanceDeterminer(DoubleRange validRangeForChance, IntRange outcomeValue)
        {
            this.validRangeForChance = validRangeForChance;
            this.outcomeValue = outcomeValue;
        }

        public virtual int getValueIfInInclusiveBounds(double Determiner)
        {
            if (this.validRangeForChance.containsInclusive(Determiner))
            {
                return this.outcomeValue.getRandomInclusive();
            }
            return 0;
        }

        public virtual bool containsInclusive(double Determiner)
        {
            return this.validRangeForChance.containsInclusive(Determiner);
        }
    }
}
