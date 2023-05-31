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
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.Utilities.Ranges;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Items
{
    /// <summary>
    /// Determines things like an item that can have a random stack size and a random chance of being obtained for machines, buildings, etc.
    /// </summary>
    public class LootTableEntry
    {

        /// <summary>
        /// Contains basic item information.
        /// </summary>
        public ItemReference item;

        /// <summary>
        /// Determines the odds for getting a specific type of item given some input values. The checked ranges should always be between 1-100.
        /// </summary>
        public List<IntOutcomeChanceDeterminer> stackSizeDeterminer;

        /// <summary>
        /// This value should be between 0 and 100, but since it's a double range it should be very flexible in terms of granular control. 100 means this will always be obtained.
        /// </summary>
        public DoubleRange chanceToObtain = new DoubleRange();

        public LootTableEntry()
        {

        }

        /// <summary>
        /// Creates a determined item outcome with a stack size reflected by the passed in reference's <see cref="ItemReference.StackSize"/> with a 100% chance of being obtained.
        /// </summary>
        /// <param name="reference"></param>
        public LootTableEntry(ItemReference reference) : this(reference, new List<IntOutcomeChanceDeterminer>() { new IntOutcomeChanceDeterminer(new DoubleRange(0, 100), reference.StackSize) }, new DoubleRange(0, 100))
        {
        }
        /// <summary>
        /// Creates a determined item outcome with a fixed stack size with a 100% chance of being obtained.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="StackSize"></param>
        public LootTableEntry(ItemReference reference, int StackSize) : this(reference, new List<IntOutcomeChanceDeterminer>() { new IntOutcomeChanceDeterminer(new DoubleRange(0, 100), StackSize) }, new DoubleRange(0, 100))
        {
        }

        /// <summary>
        /// Creates an item determined outcome with a ranged stack size with a 100% chance of being obtained.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="StackSize"></param>
        public LootTableEntry(ItemReference reference, IntRange StackSize) : this(reference, new List<IntOutcomeChanceDeterminer>() { new IntOutcomeChanceDeterminer(new DoubleRange(0, 100), StackSize) }, new DoubleRange(0, 100))
        {
        }

        /// <summary>
        /// Creates an item determined outcome with a fixed stack size, but a random chance of being obtained.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="StackSize"></param>
        /// <param name="ChanceToObtain"></param>
        public LootTableEntry(ItemReference reference, int StackSize, DoubleRange ChanceToObtain) : this(reference, new List<IntOutcomeChanceDeterminer>() { new IntOutcomeChanceDeterminer(new DoubleRange(0, 100), StackSize) }, ChanceToObtain)
        {
        }

        /// <summary>
        /// Creates an item determined outcome with a random stack size, and a random chance of being obtained.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="StackSize"></param>
        /// <param name="ChanceToObtain"></param>
        public LootTableEntry(ItemReference reference, IntRange StackSize, DoubleRange ChanceToObtain) : this(reference, new List<IntOutcomeChanceDeterminer>() { new IntOutcomeChanceDeterminer(new DoubleRange(0, 100), StackSize) }, ChanceToObtain)
        {
        }

        /// <summary>
        /// Creates an item determined outcome with various chances for having variable stack size outcomes with a 100% chance of being obtained.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="chancedStackSizeOutcomeValues"></param>
        public LootTableEntry(ItemReference reference, List<IntOutcomeChanceDeterminer> chancedStackSizeOutcomeValues) : this(reference, chancedStackSizeOutcomeValues, new DoubleRange(0, 100))
        {
        }


        /// <summary>
        /// Creates an item determined outcome with various chances for having variable stack size outcomes with a random chance of being obtained.
        /// </summary>
        /// <param name="baseOutputItem"></param>
        /// <param name="chancedStackSizeOutcomeValues"></param>
        /// <param name="chanceToObtain"></param>
        public LootTableEntry(ItemReference baseOutputItem, List<IntOutcomeChanceDeterminer> chancedStackSizeOutcomeValues, DoubleRange chanceToObtain)
        {
            this.item = baseOutputItem;
            this.stackSizeDeterminer = chancedStackSizeOutcomeValues;
            this.chanceToObtain = chanceToObtain;
        }



        /// <summary>
        /// Calculates the final output amount for a given player. Note that the actual values will be determined by .json and not by the game's definition of if it should count or not.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public virtual int getFinalOutputAmount(Farmer who = null)
        {
            double rand = Game1.random.NextDouble() * 100; //Get a value from 0 to less than 100.
            int outcomeValue = 0;
            foreach (IntOutcomeChanceDeterminer potentialOutcomeChanceRange in this.stackSizeDeterminer)
            {
                //Skip 0 chance outcomes. Why they would be included in this list, I'm not sure, but we should filter them out regardless becuse logically it doesn't make sense.
                //Note that if rand is 0 and the out potentialOutcoemChanceRange is greater than 0, this code will still trigger.
                if (potentialOutcomeChanceRange.validRangeForChance.Min == 0 && potentialOutcomeChanceRange.validRangeForChance.Max == 0)
                    continue;
                if (potentialOutcomeChanceRange.containsInclusive(rand))
                {
                    outcomeValue = Math.Max(outcomeValue,potentialOutcomeChanceRange.getValueIfInInclusiveBounds(rand));
                }
            }

            int amount = outcomeValue;

            return amount;
        }

        public virtual Item getOutputItem()
        {
            double chance=Game1.random.NextDouble() * 100;
            if (this.chanceToObtain.containsInclusive(chance))
            {
                Item item = this.item.getItem();
                item.Stack = this.getFinalOutputAmount();
                return item;
            }
            else
            {
                return null;
            }
        }

    }
}
