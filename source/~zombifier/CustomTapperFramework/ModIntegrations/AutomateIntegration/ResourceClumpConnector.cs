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
using StardewValley.TerrainFeatures;
using Pathoschild.Stardew.Automate;

namespace Selph.StardewMods.MachineTerrainFramework;

// Turns the tile of a resource clump with a tapper on it into connectors.
class ResourceClumpConnector : IAutomatable {
  private ResourceClump resourceClump;
  private Vector2 tile;

  public ResourceClumpConnector(ResourceClump rc, Vector2 t) {
    this.resourceClump = rc;
    this.tile = t;
  }

  public GameLocation Location {
    get {
      return resourceClump.Location;
    }
  }

  public Rectangle TileArea {
    get {
      return new Rectangle(
          (int)tile.X,
          (int)tile.Y,
          1, 1
      );
    }
  }
}
