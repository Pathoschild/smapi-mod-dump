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
    /// <summary>Represents the Bfav 'TypeLog' class responsible for storing the type the animal is saved as and the custom type of it.</summary>
    public class BfavAnimalType
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The custom type of the animal.</summary>
        public string Current { get; set; }

        /// <summary>The type the animal is saved as.</summary>
        public string Saved { get; set; }
    }
}
