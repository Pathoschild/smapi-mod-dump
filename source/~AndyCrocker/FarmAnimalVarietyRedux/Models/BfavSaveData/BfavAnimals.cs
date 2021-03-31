/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace FarmAnimalVarietyRedux.Models.BfavSaveData
{
    /// <summary>Represents the Bfav 'FarmAnimals' class responsible for containing all the data that's saved, used to keep track of custom animals.</summary>
    public class BfavAnimals
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The Bfav saved animal data.</summary>
        public List<BfavAnimal> Animals { get; set; }
    }
}
