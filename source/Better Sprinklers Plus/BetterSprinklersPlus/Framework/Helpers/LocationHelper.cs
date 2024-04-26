/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamescodesthings/smapi-better-sprinklers
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace BetterSprinklersPlus.Framework.Helpers
{
  public static class LocationHelper
  {
    /// <summary>Get all game location.</summary>
    public static IEnumerable<GameLocation> GetAllBuildableLocations()
    {
      return Game1.locations
        .Concat(
          from location in Game1.locations
          where location.IsBuildableLocation()
          from building in location.buildings
          where building.indoors.Value != null
          select building.indoors.Value
        );
    }
  }
}