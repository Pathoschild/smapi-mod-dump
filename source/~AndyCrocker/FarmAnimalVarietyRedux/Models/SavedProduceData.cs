/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

namespace FarmAnimalVarietyRedux.Models
{
    /// <summary>Contains the data that gets saved on each animal about the current state of a produce.</summary>
    public class SavedProduceData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the default product.</summary>
        public int DefaultProductId { get; set; }

        /// <summary>The id of the upgraded product.</summary>
        public int UpgradedProductId { get; set; }

        /// <summary>The days until the animal can produce this produce again.</summary>
        public int DaysLeft { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="defaultProductId">The id of the default product.</param>
        /// <param name="upgradedProduceId">The id of the upgraded product.</param>
        /// <param name="daysLeft">The days until the animal can produce this produce again.</param>
        public SavedProduceData(int defaultProductId, int upgradedProduceId, int daysLeft)
        {
            DefaultProductId = defaultProductId;
            UpgradedProductId = upgradedProduceId;
            DaysLeft = daysLeft;
        }
    }
}
