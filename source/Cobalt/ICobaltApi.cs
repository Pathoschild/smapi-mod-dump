/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/Cobalt
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Cobalt
{
    /// <summary>The API which provides access to Cobalt for other mods.</summary>
    public interface ICobaltApi
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get the cobalt bar's object ID.</summary>
        int GetBarId();

        /// <summary>Get the cobalt sprinkler's object ID.</summary>
        int GetSprinklerId();

        /// <summary>Get the cobalt sprinkler coverage.</summary>
        /// <param name="origin">The tile position containing the sprinkler.</param>
        IEnumerable<Vector2> GetSprinklerCoverage(Vector2 origin);
    }
}