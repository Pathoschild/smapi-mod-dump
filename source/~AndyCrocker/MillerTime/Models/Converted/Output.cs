/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

namespace MillerTime.Models.Converted
{
    /// <summary>Represents the output of a recipe.</summary>
    public class Output
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the output.</summary>
        public int Id { get; }

        /// <summary>The number of objects to output.</summary>
        public int Amount { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="id">The id of the output.</param>
        /// <param name="amount">The number of objects to output.</param>
        public Output(int id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}
