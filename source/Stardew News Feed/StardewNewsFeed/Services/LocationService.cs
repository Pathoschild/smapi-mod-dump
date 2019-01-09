
using StardewNewsFeed.Wrappers;
using StardewModdingAPI;
using System.Linq;

namespace StardewNewsFeed.Services {
    public class LocationService : ILocationService {

        private readonly ITranslationHelper _translationHelper;

        public LocationService(ITranslationHelper translationHelper) {
            _translationHelper = translationHelper;
        }

        public ILocation GetLocationByName(string locationName) {
            var location = new Game()
                .GetLocations(_translationHelper)
                .FirstOrDefault(l => l.GetLocationName() == locationName);

            return location;
        }

        public ILocation GetGreenhouse() {
            var greenhouse = new Game()
                .GetLocations(_translationHelper)
                .SingleOrDefault(l => l.IsGreenhouse());

            return greenhouse;
        }
    }
}
