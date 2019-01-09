using StardewNewsFeed.Wrappers;

namespace StardewNewsFeed.Services {
    public interface IGameService {
        void CheckFarmCave();
        void CheckGreenhouse();
        void CheckCellar();
        void CheckShed();
        void CheckLocationForBirthdays(ILocation location);
        void CheckFarmBuildings<T>() where T : StardewValley.Buildings.Building;
    }
}
