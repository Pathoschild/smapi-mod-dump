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
