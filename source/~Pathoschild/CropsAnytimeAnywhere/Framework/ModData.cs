/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace Pathoschild.Stardew.CropsAnytimeAnywhere.Framework
{
    /// <summary>The model for the raw data file.</summary>
    internal class ModData
    {
        /// <summary>The tile types to use for back tile IDs which don't have a type property and aren't marked diggable.Indexed by tilesheet image source(without path or season) and type.</summary>
        public IDictionary<string, IDictionary<string, int[]>> FallbackTileTypes { get; set; }
    }
}
