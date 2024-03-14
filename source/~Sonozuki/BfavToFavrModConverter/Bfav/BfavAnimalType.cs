/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace BfavToFavrModConverter.Bfav
{
    /// <summary>Represetns an animal subtype in BFAV's 'content.json' file.</summary>
    public class BfavAnimalType
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the sub type.</summary>
        public string Type { get; set; }

        /// <summary>The data string of the sub type.</summary>
        public string Data { get; set; }

        /// <summary>The sprites of the sub type.</summary>
        public BfavAnimalTypeSprites Sprites { get; set; }
    }
}
