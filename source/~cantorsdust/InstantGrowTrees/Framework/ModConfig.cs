/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

namespace InstantGrowTrees.Framework
{
    /// <summary>The mod configuration model.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The configuration for fruit trees.</summary>
        public FruitTreeConfig FruitTrees { get; set; } = new FruitTreeConfig();

        /// <summary>The configuration for non-fruit trees.</summary>
        public RegularTreeConfig NonFruitTrees { get; set; } = new RegularTreeConfig();
    }
}
