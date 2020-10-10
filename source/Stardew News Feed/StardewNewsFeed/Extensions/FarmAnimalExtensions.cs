/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mikesnorth/StardewNewsFeed
**
*************************************************/

using StardewValley;

namespace StardewNewsFeed.Extensions {
    public static class AnimalExtensions {
        public static bool HasAvailableProduce(this FarmAnimal @this) {
            return @this?.currentProduce?.Value > 0;
        }
    }
}