/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kiranmurmu/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace KiranMurmu.StardewValleyMods.LuckyReclamationTrashCan
{
  /// <summary>The mod patch for Harmony</summary>
  internal class ModPatch
  {
    /// <summary>A method called via Harmony after <see cref="StardewValley.Utility.getTrashReclamationPrice(Item, Farmer)"/>.</summary>
    /// <param name="__result">The return value.</param>
    public static void After_getTrashReclamationPrice(ref int __result)
    {
      double luk = Game1.player.DailyLuck * 100.0 + Game1.player.LuckLevel;
      __result = (int)(__result / (double)(0.75 - (luk > 10.0 ? 10.0 : luk < -10.0 ? -10.0 : luk) * 0.05));
    }
  }
}