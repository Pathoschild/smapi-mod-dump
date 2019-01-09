using System.Collections.Generic;
using StardewModdingAPI;

namespace StardewNewsFeed.Wrappers {

    /// <summary>
    /// Wrapper for StardewValley.Farm
    /// </summary>
    public interface IFarm {
        IList<ILocation> GetBuildings<T>(ITranslationHelper translationHelper);
    }
}
