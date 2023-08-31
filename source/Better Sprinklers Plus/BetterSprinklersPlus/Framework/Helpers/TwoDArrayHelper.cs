/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamescodesthings/smapi-better-sprinklers
**
*************************************************/

using System;

namespace BetterSprinklersPlus.Framework.Helpers
{
  public static class TwoDArrayHelper
  {
    public static int[,] Resize(this int[,] array, int to)
    {
      var length = array.GetLength(0);
      var diff = to - length;
      var diffHalved = diff / 2;

      var result = new int[to, to];

      for(var r = 0; r < to; r++)
      {
        for(var c = 0; c < to; c++)
        {
          try
          {
            result[r, c] = array[r - diffHalved, c - diffHalved];
          }
          catch (Exception)
          {
            result[r, c] = 0;
          }
        }
      }

      return result;
    }
  }
}
