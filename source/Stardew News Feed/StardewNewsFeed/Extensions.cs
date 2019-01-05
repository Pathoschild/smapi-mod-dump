
using StardewModdingAPI;
using StardewValley;

namespace StardewNewsFeed {
    public static class Extensions {

        public static string getDisplayName(this GameLocation @this, ITranslationHelper translationHelper) {
            if(@this.Name == Constants.FARM_CAVE_LOCATION_NAME) {
                return translationHelper.Get("news-feed.cave-display-name");
            }
            return @this.Name;
        }

    }
}
