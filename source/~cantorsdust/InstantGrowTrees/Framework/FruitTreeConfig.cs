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
    /// <summary>The mod configuration for fruit trees.</summary>
    internal class FruitTreeConfig
    {
        /// <summary>Whether fruit trees instantly age to iridium quality.</summary>
        public bool InstantlyAge { get; set; } = false;

        /// <summary>Whether fruit trees grow instantly overnight.</summary>
        public bool InstantlyGrow { get; set; } = true;

        /// <summary>Whether fruit trees grow instantly in winter (if <see cref="InstantlyGrow"/> is also true).</summary>
        public bool InstantlyGrowInWinter { get; set; } = true;

        /// <summary>Whether fruit trees grow instantly even if they'd normally not grow (e.g. too close to another tree).</summary>
        public bool InstantlyGrowWhenInvalid { get; set; } = false;
    }
}
