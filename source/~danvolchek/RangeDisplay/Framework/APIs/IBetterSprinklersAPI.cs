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
    /// <summary>The better sprinklers API.</summary>
    public interface IBetterSprinklersAPI
    {
        /*********
        ** Methods
        *********/

        /// <summary>Gets the coverage of sprinklers by parentSheetIndex.</summary>
        /// <returns>The sprinkler coverage.</returns>
        IDictionary<int, Vector2[]> GetSprinklerCoverage();
    }
}
