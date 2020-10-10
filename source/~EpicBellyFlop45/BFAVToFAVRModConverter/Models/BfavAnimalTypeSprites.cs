/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Represents the sprites of an animal's sub type in BFAV's 'content.json' file.</summary>
    public class BfavAnimalTypeSprites
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The path to the adult sprite sheet.</summary>
        public string Adult { get; set; }

        /// <summary>The path to the baby sprite sheet.</summary>
        public string Baby { get; set; }

        /// <summary>The path to the ready to harvest sprite sheet.</summary>
        public string ReadyForHarvest { get; set; }
    }
}
