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
