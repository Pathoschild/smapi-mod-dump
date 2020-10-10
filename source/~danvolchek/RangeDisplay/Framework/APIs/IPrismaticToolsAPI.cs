/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RangeDisplay.Framework.APIs
{
    /// <summary>The prismatic tools API.</summary>
    public interface IPrismaticToolsAPI
    {
        /*********
        ** Accessors
        *********/

        /// <summary>The parentSheetIndex of prismatic sprinklers.</summary>
        int SprinklerIndex { get; }

        /// <summary>Whether prismatic sprinklers also act as scarecrows.</summary>
        bool ArePrismaticSprinklersScarecrows { get; }

        /*********
        ** Methods
        *********/

        /// <summary>Gets the coverage of a prismatic sprinkler.</summary>
        /// <param name="origin">The position of the prismatic sprinkler.</param>
        /// <returns>The sprinkler coverage.</returns>
        IEnumerable<Vector2> GetSprinklerCoverage(Vector2 origin);
    }
}
