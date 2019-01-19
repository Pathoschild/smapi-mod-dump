using StardewValley;

namespace StardewNewsFeed.Extensions {
    public static class AnimalExtensions {
        public static bool HasAvailableProduce(this FarmAnimal @this) {
            return @this?.currentProduce?.Value > 0;
        }
    }
}