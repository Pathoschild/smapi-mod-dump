/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/AnimalProduceExpansion
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using AnimalProduceExpansion.API;
using AnimalProduceExpansion.Data;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace AnimalProduceExpansion.Patches
{
  [HarmonyPatch(typeof(FarmAnimal))]
  [HarmonyPatch("dayUpdate")]
  // ReSharper disable once UnusedMember.Global
  internal class DayUpdate : Utility
  {
    public static CurrentDrops CurrentDrops;
    private static readonly IDictionary<long, int> AnimalAges = new Dictionary<long, int>();

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    [HarmonyAfter("Paritee.BetterFarmAnimalVariety")]
    public static bool Prefix(ref FarmAnimal __instance)
    {
      if (!Config.HasAnimal(__instance.type.Value)) return true;
      UpdateOrInsert(ref __instance);

      CurrentDrops.DefaultDrop = __instance.defaultProduceIndex.Value;
      CurrentDrops.DeluxeDrop = __instance.deluxeProduceIndex.Value;
      CurrentDrops.IsAltered = true;

      CurrentDrops.NewDrop = Config.GetMainDropForAnimal(__instance.type.Value);
      __instance.defaultProduceIndex.Value = CurrentDrops.NewDrop;
      __instance.deluxeProduceIndex.Value = -1;

      if (RegisterGameEvents.IsModLoaded("Paritee.BetterFarmAnimalVariety"))
        __instance.pushAccumulator = 1;

      return true;
    }

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    [HarmonyAfter("Paritee.BetterFarmAnimalVariety")]
    public static void Postfix(ref FarmAnimal __instance)
    {
      if (!Config.HasAnimal(__instance.type.Value) ||
          !IsNewDay(ref __instance) ||
          !CurrentDrops.IsAltered)
        return;
      if (!__instance.isBaby() && __instance.daysSinceLastLay.Value >= __instance.daysToLay.Value)
      {
        if (RegisterGameEvents.IsModLoaded("Paritee.BetterFarmAnimalVariety"))
          HandleItemDrop(CurrentDrops.NewDrop, ref __instance);

        if (Config.HasExtraDrop(__instance.type.Value))
        {
          var extraDrop = Config.GetExtraDropForAnimal(__instance.type.Value);
          if (extraDrop != -1)
            HandleItemDrop(extraDrop, ref __instance);
        }
      }

      __instance.defaultProduceIndex.Value = CurrentDrops.DefaultDrop;
      __instance.deluxeProduceIndex.Value = CurrentDrops.DeluxeDrop;
      CurrentDrops.DefaultDrop = -1;
      CurrentDrops.DefaultDrop = -1;

      CurrentDrops.IsAltered = false;
    }

    // ReSharper disable once InconsistentNaming
    private static void HandleItemDrop(int itemToDrop, ref FarmAnimal __instance)
    {
      var flag3 = true;
      var quality = ChooseQuality(ref __instance);
      foreach (var @object in __instance.home.indoors.Value.objects.Values
        .Where(@object =>
          @object.bigCraftable.Value && @object.ParentSheetIndex == 165 && @object.heldObject.Value != null)
        .Where(@object => (@object.heldObject.Value as Chest)?
          .addItem(new Object(Vector2.Zero, itemToDrop, null, false, true,
            false, false)
          {
            Quality = quality
          }) == null))
      {
        @object.showNextIndex.Value = true;
        flag3 = false;
        break;
      }

      if (flag3 && !__instance.home.indoors.Value.Objects.ContainsKey(__instance.getTileLocation()))
        StardewValley.Utility.spawnObjectAround(__instance.getTileLocation(),
          new Object(Vector2.Zero, itemToDrop, null, false, true, false, true)
          {
            Quality = quality
          },
          __instance.home.indoors.Value);
    }

    private static int ChooseQuality(ref FarmAnimal farmAnimal)
    {
      var happinessValue =
        farmAnimal.friendshipTowardFarmer.Value / 1000.0 - (1.0 - farmAnimal.happiness.Value / 225.0);

      if (!farmAnimal.isCoopDweller() && Game1.getFarmer(farmAnimal.ownerID.Value).professions.Contains(3) ||
          farmAnimal.isCoopDweller() && Game1.getFarmer(farmAnimal.ownerID.Value).professions.Contains(2))
        happinessValue += 0.33;

      var random = Api.GetApi<IRandomApi>().GetNewRandom();
      if (happinessValue >= 0.95 && random.NextDouble() < happinessValue / 2.0)
        farmAnimal.produceQuality.Value = 4;
      else if (random.NextDouble() < happinessValue / 2.0)
        farmAnimal.produceQuality.Value = 2;
      else if (random.NextDouble() < happinessValue)
        farmAnimal.produceQuality.Value = 1;
      else
        farmAnimal.produceQuality.Value = 0;
      return farmAnimal.produceQuality.Value;
    }

    private static void UpdateOrInsert(ref FarmAnimal animal)
    {
      if (AnimalAges.ContainsKey(animal.myID.Value))
      {
        AnimalAges[animal.myID.Value] = animal.age.Value;
        return;
      }

      AnimalAges.Add(animal.myID.Value, animal.age.Value);
    }

    private static bool IsNewDay(ref FarmAnimal animal) => animal.age.Value == AnimalAges[animal.myID.Value] + 1;
  }
}