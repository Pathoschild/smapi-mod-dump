/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamescodesthings/smapi-better-sprinklers
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace BetterSprinklersPlus.Framework.Helpers
{
  public static class PlacementHelper
  {
    public static Vector2? GetPlacementPosition()
    {
      var activeObject = Game1.player.ActiveObject;
      if (activeObject == null) return null;

      var x = (int)Game1.GetPlacementGrabTile().X * 64;
      var y = (int)Game1.GetPlacementGrabTile().Y * 64;
      Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
      if (Game1.isCheckingNonMousePlacement)
      {
        var placementPosition =
          Utility.GetNearbyValidPlacementPosition(Game1.player, Game1.currentLocation,
            Game1.player.ActiveObject, x, y);
        x = (int)placementPosition.X;
        y = (int)placementPosition.Y;
      }

      return new Vector2
      {
        // ReSharper disable once PossibleLossOfFraction
        X = x / 64,
        // ReSharper disable once PossibleLossOfFraction
        Y = y / 64
      };
    }
  }
}