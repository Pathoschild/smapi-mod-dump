/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gingajamie/smapi-better-sprinklers-plus-encore
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace BetterSprinklersPlus.Framework.Helpers
{
  public static class ScarecrowHelper
  {
    public static bool IsScarecrow(this Object obj)
    {
      return obj.bigCraftable.Value && obj.Name.Contains("arecrow");
    }

    public static int[,] GetScarecrowGrid()
    {
      const int maxGridSize = BetterSprinklersPlusConfig.ScarecrowGridSize;
      var grid = new int[maxGridSize, maxGridSize];
      const int scarecrowCenterValue = maxGridSize / 2;
      var scarecrowCenter = new Vector2(scarecrowCenterValue, scarecrowCenterValue);
      for (var x = 0; x < maxGridSize; x++)
      {
        for (var y = 0; y < maxGridSize; y++)
        {
          var vector = new Vector2(x, y);
          grid[x, y] = Vector2.Distance(vector, scarecrowCenter) < 9f ? 1 : 0;
        }
      }

      return grid;
    }
  }
}
