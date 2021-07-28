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
// Type: BetterFarmAnimalVariety.Framework.Patches.AnimalHouse.AddNewHatchedAnimal
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Linq;
using BetterFarmAnimalVariety.Framework.Decorators;
using BetterFarmAnimalVariety.Framework.Helpers;
using Paritee.StardewValley.Core.Locations;
using Paritee.StardewValley.Core.Utilities;
using StardewValley.Events;

namespace BetterFarmAnimalVariety.Framework.Patches.AnimalHouse
{
  internal class AddNewHatchedAnimal
  {
    public static bool Prefix(ref StardewValley.AnimalHouse __instance, ref string name)
    {
      var moddedAnimalHouse = new Decorators.AnimalHouse(__instance);
      var moddedPlayer = new Farmer(Game.GetPlayer());
      if (moddedAnimalHouse.GetBuilding() is StardewValley.Buildings.Barn _)
        if (Event.IsFarmEventOccurring(out QuestionEvent farmEvent))
          HandleNewborn(ref moddedAnimalHouse, name, ref farmEvent, moddedPlayer);
      HandleHatchling(ref moddedAnimalHouse, name, moddedPlayer);
      Event.GoToNextEventCommandInLocation(Game.GetCurrentLocation());
      Game.ExitActiveMenu();
      return false;
    }

    private static void HandleHatchling(
      ref Decorators.AnimalHouse moddedAnimalHouse,
      string name,
      Farmer moddedPlayer)
    {
      if (moddedAnimalHouse.IsFull())
        return;
      var withEggReadyToHatch = moddedAnimalHouse.GetIncubatorWithEggReadyToHatch();
      if (withEggReadyToHatch == null)
        return;
      string type;
      type = new Incubator(withEggReadyToHatch)
        .GetRandomType(FarmAnimals.GroupTypesByCategory()
          .ToDictionary
          (kvp => kvp.Key,
            kvp => moddedPlayer.SanitizeBlueChickens(kvp.Value)));

      var building = moddedAnimalHouse.GetBuilding();
      var farmAnimal = new Decorators.FarmAnimal(moddedPlayer.CreateFarmAnimal(type, name, building));
      farmAnimal.AddToBuilding(building);
      moddedAnimalHouse.ResetIncubator(withEggReadyToHatch);
      farmAnimal.SetCurrentProduce(-1);
    }

    private static void HandleNewborn(
      ref Decorators.AnimalHouse moddedAnimalHouse,
      string name,
      ref QuestionEvent questionEvent,
      Farmer moddedPlayer)
    {
      var withEggReadyToHatch = moddedAnimalHouse.GetIncubatorWithEggReadyToHatch();
      var moddedParent = new Decorators.FarmAnimal(questionEvent.animal);
      var dictionary = FarmAnimals.GroupTypesByCategory().Where(kvp => kvp.Value.Contains(moddedParent.GetTypeString()))
        .ToDictionary(kvp => kvp.Key, kvp => moddedPlayer.SanitizeBlueChickens(kvp.Value));
      var randomTypeFromProduce = moddedParent.GetRandomTypeFromProduce(dictionary);
      var building = moddedAnimalHouse.GetBuilding();
      var farmAnimal = new Decorators.FarmAnimal(moddedPlayer.CreateFarmAnimal(randomTypeFromProduce, name, building));
      farmAnimal.AssociateParent(questionEvent.animal);
      farmAnimal.AddToBuilding(building);
      farmAnimal.SetCurrentProduce(-1);
      Event.ForceQuestionEventToProceed(questionEvent);
    }
  }
}