
using StardewModdingAPI;
using StardewValley;
using Netcode;

namespace StardewNewsFeed {
    public static class Extensions {

        public static string GetDisplayName(this GameLocation @this, ITranslationHelper translationHelper) {
            if(@this.Name == Constants.FARM_CAVE_LOCATION_NAME) {
                return translationHelper.Get("news-feed.cave-display-name");
            }
            return @this.Name;
        }

        public static NetBool TileIsReadyForHarvest(this GameLocation @this, int height, int width) {
            var objectAtLocation = @this.getObjectAtTile(height, width);
            if(objectAtLocation == null) {
                return new NetBool(false);
            }
            return objectAtLocation.readyForHarvest;
        }

    }
}
