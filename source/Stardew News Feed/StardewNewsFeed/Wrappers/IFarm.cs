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
