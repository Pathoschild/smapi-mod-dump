/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Represents BFAV's 'content.json' file for deserialization.</summary>
    public class BfavContent
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The content wrapper for each animal.</summary>
        public List<BfavCategory> Categories { get; set; }
    }
}
