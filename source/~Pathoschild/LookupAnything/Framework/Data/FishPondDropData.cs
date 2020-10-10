/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.LookupAnything.Framework.Data
{
    /// <summary>An item that can be produced by a fish pond.</summary>
    internal class FishPondDropData : ItemDropData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The minimum population needed for the item to drop.</summary>
        public int MinPopulation { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="minPopulation">The minimum population needed for the item to drop.</param>
        /// <param name="itemID">The item's parent sprite index.</param>
        /// <param name="minDrop">The minimum number to drop.</param>
        /// <param name="maxDrop">The maximum number to drop.</param>
        /// <param name="probability">The probability that the item will be dropped.</param>
        public FishPondDropData(int minPopulation, int itemID, int minDrop, int maxDrop, float probability)
            : base(itemID, minDrop, maxDrop, probability)
        {
            this.MinPopulation = minPopulation;
        }
    }
}
