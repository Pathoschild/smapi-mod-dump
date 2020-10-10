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
    public interface ILocationService {
        ILocation GetLocationByName(string locationName);
        ILocation GetGreenhouse();
    }
}
