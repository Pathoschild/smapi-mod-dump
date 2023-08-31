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

namespace Pathoschild.Stardew.Common.Integrations.BetterSprinklersPlus
{
    /// <summary>The API provided by the Better Sprinklers mod.</summary>
    public interface IBetterSprinklersPlusApi
    {
        /// <summary>Get the maximum supported coverage width or height.</summary>
        int GetMaxGridSize();

        /// <summary>Get the relative tile coverage by supported sprinkler ID.</summary>
        IDictionary<int, Vector2[]> GetSprinklerCoverage();
    }
}
