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

namespace BetterGardenPots.APIs
{
    /// <summary>API for prismatic tools.</summary>
    public interface IPrismaticToolsAPI
    {
        int SprinklerIndex { get; }

        IEnumerable<Vector2> GetSprinklerCoverage(Vector2 origin);
    }
}
