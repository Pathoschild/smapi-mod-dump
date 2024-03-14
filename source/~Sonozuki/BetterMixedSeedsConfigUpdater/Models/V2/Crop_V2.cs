/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace BetterMixedSeedsConfigUpdater.Models.V2
{
    /// <summary>Metadata about a crop.</summary>
    public class Crop_V2
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The crops name, this is the display name used in the configuration.</summary>
        public string Name { get; set; }

        /// <summary>Whether the crop should get added the seed list.</summary>
        public bool Enabled { get; set; }

        /// <summary>The chance the crop will have at being picked.</summary>
        public int Chance { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">Crop name.</param>
        /// <param name="enabled">Whether the crop is enabled.</param>
        /// <param name="chance">The crop chance.</param>
        public Crop_V2(string name, bool enabled, int chance)
        {
            Name = name;
            Enabled = enabled;
            Chance = chance;
        }
    }
}
