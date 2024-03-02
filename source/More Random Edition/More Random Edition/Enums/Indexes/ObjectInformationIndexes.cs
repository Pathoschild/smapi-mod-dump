/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

namespace Randomizer
{
    /// <summary>
    /// The indexes in the Data/ObjectInformation.xnb dictionary
    /// Add to this enum as these are used
    /// </summary>
    public enum ObjectInformationIndexes
    {
        Name = 0,
        Price = 1,
        DisplayName = 4,
        Description = 5,

        /// <summary>
        /// This is at the end of the object information - unsure if it's actually used anywhere
        /// See FishItem.ObjectInformationSuffix
        /// </summary>
        AdditionalFishInfo = 6
    }
}
