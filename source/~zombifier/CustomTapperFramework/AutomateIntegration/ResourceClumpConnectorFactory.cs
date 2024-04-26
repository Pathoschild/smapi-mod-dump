/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;
using Pathoschild.Stardew.Automate;
using SObject = StardewValley.Object;

namespace CustomTapperFramework;
public class ResourceClumpConnectorFactory : IAutomationFactory
{
  public IAutomatable GetFor(SObject obj, GameLocation location, in Vector2 tile) {
    return null;
  }

  public IAutomatable GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile) {
    return null;
  }

  public IAutomatable GetFor(Building building, GameLocation location, in Vector2 tile) {
    return null;
  }

  public IAutomatable GetForTile(GameLocation location, in Vector2 tile) {
    foreach (var resourceClump in location.resourceClumps) {
    if (resourceClump.occupiesTile((int)tile.X, (int)tile.Y) &&
        location.objects.TryGetValue(Utils.GetTapperLocationForClump(resourceClump), out SObject tapper) &&
        tapper.IsTapper()) {
      return new ResourceClumpConnector(resourceClump, tile);
    }

    }
    return null;
  }
}
