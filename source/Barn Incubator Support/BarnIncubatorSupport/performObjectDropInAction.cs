/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using StardewValley;

// ReSharper disable InconsistentNaming
// ReSharper disable once RedundantAssignment
namespace BarnIncubatorSupport
{
  internal class performObjectDropInAction
  {
    public static bool Prefix(ref Object __instance, ref Item dropInItem,
      ref bool probe, ref Farmer who, ref bool __result)
    {
      __result = false;
      if (__instance.isTemporarilyInvisible || !(dropInItem is Object))
        return true;

      var dropItem = (Object) dropInItem;

      var isIncubator = IsIncubator(__instance);
      var isOstrichIncubator = IsOstrichIncubator(__instance);
      if (dropItem.Category != -5 && !dropInItem.Name.Contains("Egg") || !isIncubator && !isOstrichIncubator) return true;
      var eggInfo = IsAllowedInBarns(dropItem.ParentSheetIndex.ToString());
      if (eggInfo.Length == 0) return true;
      if (__instance.heldObject.Value != null ||
          eggInfo[0] && !isOstrichIncubator ||
          !eggInfo[0] && !isIncubator)
        return false;

      __instance.heldObject.Value = new Object(dropItem.ParentSheetIndex, 1);
      if (probe) return true;
      who.currentLocation.playSound("coin");
      __instance.MinutesUntilReady = (eggInfo[0] ? 15000 : 9000) * (eggInfo[1] ? 2 : 1);
      if (who.professions.Contains(2))
        __instance.MinutesUntilReady /= 2;
      ++__instance.ParentSheetIndex;
      if (who?.currentLocation != null && who.currentLocation is AnimalHouse house)
        house.hasShownIncubatorBuildingFullMessage = false;

      __result = true;
      return false;
    }

    private static bool IsIncubator(Item __instance) => new[] {101, 102, 193}.Contains(__instance.ParentSheetIndex);
    private static bool IsOstrichIncubator(Item __instance) => new[] {254, 255}.Contains(__instance.ParentSheetIndex);

    private static bool[] IsAllowedInBarns(string dropItemId)
    {
      var animals = Game1.content.Load<IDictionary<string, string>>(Path.Combine("Data", "FarmAnimals"));
      foreach (var animal in animals)
      {
        var animalProps = animal.Value.Split('/');
        if (animalProps[2] == dropItemId || animalProps[3] == dropItemId)
          return new[] {animalProps[15].ToLower() == "barn", animal.Key.ToLower().Contains("dinosaur")};
      }

      return new bool[0];
    }
  }
}