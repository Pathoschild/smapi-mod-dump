/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/speeder1/SMAPISprinklerMod
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BetterSprinklers
{
    /// <summary>The API which provides access to Better Sprinklers for other mods.</summary>
    public interface IBetterSprinklersApi
    {
        /// <summary>Get the maximum sprinkler coverage supported by this mod (in tiles wide or high).</summary>
        int GetMaxGridSize();

        /// <summary>Get the relative tile coverage by supported sprinkler ID.</summary>
        IDictionary<int, Vector2[]> GetSprinklerCoverage();
    }
}