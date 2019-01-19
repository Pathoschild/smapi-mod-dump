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
