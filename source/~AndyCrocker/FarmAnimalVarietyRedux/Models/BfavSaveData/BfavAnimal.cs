/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

namespace FarmAnimalVarietyRedux.Models.BfavSaveData
{
    /// <summary>Represents the Bfav 'FarmAnimal' class responsible for containing all the data that's saved for a custom animal.</summary>
    public class BfavAnimal
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the custom animal.</summary>
        public long Id { get; set; }

        /// <summary>The custom type of the animal.</summary>
        public BfavAnimalType TypeLog { get; set; }
    }
}
