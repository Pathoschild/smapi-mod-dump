
using System.Collections.Generic;
using StardewModdingAPI;
using StardewNewsFeed.Enums;

namespace StardewNewsFeed.Wrappers {
    /// <summary>
    /// Wrapper for Game1
    /// </summary>
    public interface IGame {
        IList<ILocation> GetLocations(ITranslationHelper translationHelper);
        IFarm GetFarm();
        void DisplayMessage(IHudMessage message);
        FarmCaveChoice GetFarmCaveChoice();
    }
}
