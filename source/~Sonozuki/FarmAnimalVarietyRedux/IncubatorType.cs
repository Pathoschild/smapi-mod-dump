/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace FarmAnimalVarietyRedux
{
    /// <summary>The types of incubator a recipe can use.</summary>
    public enum IncubatorType
    {
        /// <summary>The regular incubator in the coop.</summary>
        Regular = 1,

        /// <summary>The ostrich incubator in the barn.</summary>
        Ostrich = 2,

        /// <summary>Either incubator.</summary>
        Both = Regular | Ostrich
    }
}
