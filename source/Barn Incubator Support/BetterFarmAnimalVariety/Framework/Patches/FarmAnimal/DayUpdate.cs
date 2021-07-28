/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

// Decompiled with JetBrains decompiler
// Type: BetterFarmAnimalVariety.Framework.Patches.FarmAnimal.DayUpdate
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using System.Linq;
using BetterFarmAnimalVariety.Framework.Cache;
using Microsoft.Xna.Framework;
using Paritee.StardewValley.Core.Locations;
using StardewValley;
using StardewValley.Objects;
using FarmAnimals = BetterFarmAnimalVariety.Framework.Helpers.FarmAnimals;
using Game = Paritee.StardewValley.Core.Utilities.Game;
using Random = System.Random;

namespace BetterFarmAnimalVariety.Framework.Patches.FarmAnimal
{
  internal class DayUpdate
  {
    private const double FullnessChanceOdds = 200.0;
    private const double HappinessChanceOdds = 70.0;
    private const int WhiteChickenEgg = 176;
    private const int BrownChickenEgg = 180;
    private const int Wool = 440;
    private const int DuckEgg = 442;

    public static bool Prefix(ref StardewValley.FarmAnimal __instance, ref GameLocation environtment)
    {
      var flag = __instance.home != null &&
                 !(__instance.home.indoors.Value as StardewValley.AnimalHouse).animals.ContainsKey(
                   __instance.myID.Value) && environtment is Farm && __instance.home.animalDoorOpen.Value;
      __instance.pushAccumulator = flag ? 1 : 0;
      if (flag)
        return true;
      __instance.hitGlowTimer =
        __instance.fullness.Value >= (byte) 200 && Game1.timeOfDay >= 1700 ||
        !(environtment is StardewValley.AnimalHouse) ||
        !environtment.objects.Pairs.Where(kvp => kvp.Value.Name == "Hay").Any()
          ? __instance.fullness.Value
          : (int) byte.MaxValue;
      __instance.pauseTimer = __instance.daysToLay.Value;
      __instance.daysToLay.Value = byte.MaxValue;
      return true;
    }

    public static void Postfix(ref StardewValley.FarmAnimal __instance, ref GameLocation environtment)
    {
      if (__instance.pushAccumulator == 1)
      {
        __instance.pushAccumulator = 0;
      }
      else
      {
        __instance.daysToLay.Value = (byte) __instance.pauseTimer;
        var moddedAnimal = new Decorators.FarmAnimal(__instance);
        moddedAnimal.SetPauseTimer(0);
        var hitGlowTimer = (byte) moddedAnimal.GetHitGlowTimer();
        moddedAnimal.SetHitGlowTimer(0);
        HandleCurrentProduce(ref moddedAnimal, hitGlowTimer);
      }
    }

    private static void HandleCurrentProduce(ref Decorators.FarmAnimal moddedAnimal, byte originalFullness)
    {
      var daysToLay = moddedAnimal.GetDaysToLay(moddedAnimal.GetOwner());
      var seed = (int) moddedAnimal.GetUniqueId() / 2 + Game.GetDaysPlayed();
      var flag = RollRandomProduceChance(moddedAnimal, originalFullness, seed);
      if (!moddedAnimal.IsAProducer() || moddedAnimal.IsBaby())
      {
        moddedAnimal.SetCurrentProduce(-1);
      }
      else
      {
        if (moddedAnimal.GetDaysSinceLastLay() < daysToLay)
          return;
        if (!flag)
        {
          moddedAnimal.SetCurrentProduce(-1);
        }
        else
        {
          HandleGameStats(moddedAnimal);
          var player = Game.GetPlayer();
          var farmAnimals = FarmAnimals.ReadCache();
          var typeStr = moddedAnimal.GetTypeString();
          var farmAnimalType = farmAnimals.Categories.SelectMany(o => (IEnumerable<FarmAnimalType>) o.Types)
            .Where(o => o.Type == typeStr).FirstOrDefault();
          var deluxeProduceLuck = farmAnimalType == null ? 0.0 : farmAnimalType.DeluxeProduceLuck;
          var produceIndex = moddedAnimal.RollProduce(seed, player, deluxeProduceLuck);
          moddedAnimal.SetCurrentProduce(produceIndex);
          if (!Paritee.StardewValley.Core.Characters.FarmAnimal.IsProduceAnItem(produceIndex))
            return;
          moddedAnimal.SetDaysSinceLastLay(0);
          HandleProduceQuality(moddedAnimal, seed);
          HandleProduceSpawn(moddedAnimal);
        }
      }
    }

    private static bool RollRandomProduceChance(Decorators.FarmAnimal moddedAnimal, byte fullness, int seed)
    {
      var random = new Random(seed);
      var happiness = moddedAnimal.GetHappiness();
      return (random.NextDouble() < fullness / 200.0) & (random.NextDouble() < happiness / 70.0);
    }

    private static void HandleGameStats(Decorators.FarmAnimal moddedAnimal)
    {
      try
      {
        switch (moddedAnimal.GetDefaultProduce())
        {
          case 176:
          case 180:
            ++Game1.stats.ChickenEggsLayed;
            break;
          case 440:
            ++Game1.stats.RabbitWoolProduced;
            break;
          case 442:
            ++Game1.stats.DuckEggsLayed;
            break;
        }
      }
      catch
      {
      }
    }

    private static void HandleProduceQuality(Decorators.FarmAnimal moddedAnimal, int seed)
    {
      var quality = moddedAnimal.RollProduceQuality(moddedAnimal.GetOwner(), seed);
      moddedAnimal.SetProduceQuality(quality);
    }

    private static void HandleProduceSpawn(Decorators.FarmAnimal moddedAnimal)
    {
      if (!moddedAnimal.LaysProduce() || !moddedAnimal.HasHome())
        return;
      var indoors = Paritee.StardewValley.Core.Locations.AnimalHouse.GetIndoors(moddedAnimal.GetHome());
      var tileLocation = moddedAnimal.GetTileLocation();
      var currentProduce = moddedAnimal.GetCurrentProduce();
      var produceQuality = moddedAnimal.GetProduceQuality();
      var flag = true;
      foreach (var @object in indoors.Objects.Values)
      {
        int num;
        if ((bool) @object.bigCraftable && (int) @object.parentSheetIndex == 165 && @object.heldObject.Value != null)
          num = (@object.heldObject.Value as Chest).addItem(
            new StardewValley.Object(Vector2.Zero, currentProduce, null, false, true, false, false)
            {
              Quality = produceQuality
            }) == null
            ? 1
            : 0;
        else
          num = 0;
        if (num != 0)
        {
          @object.showNextIndex.Value = true;
          flag = false;
          break;
        }
      }

      if (flag && !indoors.Objects.ContainsKey(tileLocation))
      {
        var @object = new StardewValley.Object(Vector2.Zero, currentProduce, null, false, true, false, true)
        {
          Quality = produceQuality
        };
        Location.SpawnObject(indoors, tileLocation, @object);
      }

      moddedAnimal.SetCurrentProduce(-1);
    }
  }
}