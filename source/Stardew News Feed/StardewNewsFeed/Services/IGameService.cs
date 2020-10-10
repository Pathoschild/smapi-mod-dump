/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mikesnorth/StardewNewsFeed
**
*************************************************/

using StardewNewsFeed.Wrappers;

namespace StardewNewsFeed.Services {
    public interface IGameService {
        void CheckFarmCave();
        void CheckGreenhouse();
        void CheckCellar();
        void CheckShed();
        void CheckBarnForAnimalProducts();
        void CheckLocationForBirthdays(ILocation location);
        void CheckFarmBuildingsForHarvastableItems<T>() where T : StardewValley.Buildings.Building;
        void CheckSilos();
    }
}
