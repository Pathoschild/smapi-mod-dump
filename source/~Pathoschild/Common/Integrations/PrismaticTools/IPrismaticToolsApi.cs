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
using Microsoft.Xna.Framework;

namespace Pathoschild.Stardew.Common.Integrations.PrismaticTools
{
    /// <summary>The API provided by the Prismatic Tools mod.</summary>
    public interface IPrismaticToolsApi
    {
        /// <summary>Whether prismatic sprinklers also act as scarecrows.</summary>
        bool ArePrismaticSprinklersScarecrows { get; }

        /// <summary>The prismatic sprinkler object ID.</summary>
        int SprinklerIndex { get; }

        /// <summary>Get the relative tile coverage for a prismatic sprinkler.</summary>
        /// <param name="origin">The sprinkler tile.</param>
        IEnumerable<Vector2> GetSprinklerCoverage(Vector2 origin);
    }
}
