/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mikesnorth/StardewNewsFeed
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;

namespace StardewNewsFeed.Wrappers {

    /// <summary>
    /// Wrapper for StardewValley.Farm
    /// </summary>
    public interface IFarm {
        IEnumerable<FarmAnimal> BarnAnimalsWithAvailableProduce { get; }
        IEnumerable<ILocation> GetBuildings<T>(ITranslationHelper translationHelper);
    }
}
