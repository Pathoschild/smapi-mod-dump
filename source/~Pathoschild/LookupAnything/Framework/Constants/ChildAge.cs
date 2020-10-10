/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewValley.Characters;

namespace Pathoschild.Stardew.LookupAnything.Framework.Constants
{
    /// <summary>The growth stage for a player's child.</summary>
    internal enum ChildAge
    {
        /// <summary>The child was born days ago.</summary>
        Newborn = Child.newborn,

        /// <summary>The child is older than newborn, and can sit on its own.</summary>
        Baby = Child.baby,

        /// <summary>The child is older than baby, and can crawl around.</summary>
        Crawler = Child.crawler,

        /// <summary>The child is older than crawler, and can toddle around.</summary>
        Toddler = Child.toddler
    }
}
