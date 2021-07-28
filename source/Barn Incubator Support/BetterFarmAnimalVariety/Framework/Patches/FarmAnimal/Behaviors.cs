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
// Type: BetterFarmAnimalVariety.Framework.Patches.FarmAnimal.Behaviors
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using Microsoft.Xna.Framework;
using Paritee.StardewValley.Core.Locations;
using Paritee.StardewValley.Core.Utilities;
using StardewValley;
using Farmer = BetterFarmAnimalVariety.Framework.Decorators.Farmer;
using Game = Paritee.StardewValley.Core.Utilities.Game;
using Location = BetterFarmAnimalVariety.Framework.Decorators.Location;

namespace BetterFarmAnimalVariety.Framework.Patches.FarmAnimal
{
  internal class Behaviors
  {
    private const double FindProduceChance = 0.0002;

    public static bool Prefix(
      ref StardewValley.FarmAnimal __instance,
      ref GameTime time,
      ref GameLocation location,
      ref bool __result)
    {
      var moddedAnimal = new Decorators.FarmAnimal(__instance);
      if (!moddedAnimal.HasHome() || moddedAnimal.IsEating() || Game1.IsClient || __instance.controller != null)
        return true;
      var moddedLocation = new Location(location);
      HandleFindGrassToEat(ref moddedAnimal, ref moddedLocation);
      if (HandleNightTimeRoutine(ref moddedAnimal, ref moddedLocation))
      {
        __result = true;
      }
      else
      {
        HandleFindProduce(ref moddedAnimal, ref moddedLocation);
        __result = false;
      }

      return false;
    }

    private static void HandleFindGrassToEat(
      ref Decorators.FarmAnimal moddedAnimal,
      ref Location moddedLocation)
    {
      if (!moddedLocation.IsOutdoors() || moddedAnimal.GetFullness() >= 195 || Random.NextDouble() >= 0.002 ||
          !Paritee.StardewValley.Core.Characters.FarmAnimal.UnderMaxPathFindingPerTick())
        return;
      Paritee.StardewValley.Core.Characters.FarmAnimal.IncreasePathFindingThisTick();
      moddedAnimal.SetFindGrassPathController(moddedLocation.GetOriginal());
    }

    private static bool HandleNightTimeRoutine(
      ref Decorators.FarmAnimal moddedAnimal,
      ref Location moddedLocation)
    {
      if (Game.GetTimeOfDay() < 1700 || !moddedLocation.IsOutdoors() || moddedAnimal.HasController() ||
          Random.NextDouble() >= 0.002)
        return false;
      if (moddedLocation.AnyFarmers())
      {
        moddedLocation.RemoveAnimal(moddedAnimal.GetOriginal());
        moddedAnimal.ReturnHome();
        return true;
      }

      if (Paritee.StardewValley.Core.Characters.FarmAnimal.UnderMaxPathFindingPerTick())
      {
        Paritee.StardewValley.Core.Characters.FarmAnimal.IncreasePathFindingThisTick();
        moddedAnimal.SetFindHomeDoorPathController(moddedLocation.GetOriginal());
      }

      return false;
    }

    private static bool IsValidLocation(Location moddedLocation)
    {
      return moddedLocation.IsOutdoors() && !Weather.IsRaining() && !Season.IsWinter();
    }

    private static bool CanFindProduce(Decorators.FarmAnimal moddedAnimal, Farmer moddedPlayer)
    {
      return !moddedAnimal.IsBaby() && moddedAnimal.CanFindProduce() &&
             Paritee.StardewValley.Core.Characters.FarmAnimal.IsProduceAnItem(moddedAnimal.GetCurrentProduce());
    }

    private static bool HasNoImpediments(Decorators.FarmAnimal moddedAnimal, Location moddedLocation)
    {
      var boundingBox = moddedAnimal.GetBoundingBox();
      for (var corner = 0; corner < 4; ++corner)
      {
        var cornersOfThisRectangle = StardewValley.Utility.getCornersOfThisRectangle(ref boundingBox, corner);
        var key = new Vector2(cornersOfThisRectangle.X / 64f, cornersOfThisRectangle.Y / 64f);
        if (moddedLocation.GetOriginal().terrainFeatures.ContainsKey(key) ||
            moddedLocation.GetOriginal().objects.ContainsKey(key))
          return false;
      }

      return true;
    }

    private static void HandleFindProduce(ref Decorators.FarmAnimal moddedAnimal, ref Location moddedLocation)
    {
      var moddedPlayer = new Farmer(Game.GetPlayer());
      if (!IsValidLocation(moddedLocation) || !CanFindProduce(moddedAnimal, moddedPlayer) ||
          Random.NextDouble() >= 0.0002 || !HasNoImpediments(moddedAnimal, moddedLocation))
        return;
      if (moddedPlayer.IsCurrentLocation(moddedLocation.GetOriginal()))
      {
        BellsAndWhistles.PlaySound("dirtyHit", 450);
        BellsAndWhistles.PlaySound("dirtyHit", 900);
        BellsAndWhistles.PlaySound("dirtyHit", 1350);
      }

      if (Game.IsCurrentLocation(moddedLocation.GetOriginal()))
        moddedAnimal.AnimateFindingProduce();
      else
        moddedAnimal.FindProduce(moddedPlayer.GetOriginal());
    }
  }
}