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
// Type: BetterFarmAnimalVariety.Framework.Events.ConvertDirtyFarmAnimals
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using System.Linq;
using BetterFarmAnimalVariety.Framework.Helpers;
using BetterFarmAnimalVariety.Framework.SaveData;
using StardewModdingAPI.Events;
using StardewValley;
using FarmAnimal = BetterFarmAnimalVariety.Framework.Decorators.FarmAnimal;
using FarmAnimals = BetterFarmAnimalVariety.Framework.SaveData.FarmAnimals;

namespace BetterFarmAnimalVariety.Framework.Events
{
  internal class ConvertDirtyFarmAnimals
  {
    public static void OnSaving(SavingEventArgs e)
    {
      var farmAnimals = Mod.ReadSaveData<FarmAnimals>("farm-animals");
      var animalIds = new List<long>();
      for (var index1 = 0; index1 < Game1.locations.Count; ++index1)
        if (Game1.locations[index1] is Farm location)
        {
          for (var index2 = 0; index2 < location.buildings.Count; ++index2)
            if (location.buildings[index2].indoors.Value is AnimalHouse animalHouse)
              for (var index3 = 0; index3 < animalHouse.animalsThatLiveHere.Count(); ++index3)
              {
                var key = animalHouse.animalsThatLiveHere.ElementAt(index3);
                var farmAnimal = new FarmAnimal(animalHouse.animals[key]);
                animalIds.Add(key);
                if (farmAnimal.IsVanilla())
                {
                  if (!farmAnimals.AnimalExists(farmAnimal.GetUniqueId()))
                    farmAnimals.AddAnimal(farmAnimal);
                }
                else
                {
                  var typeString = farmAnimal.GetTypeString();
                  var savedTypeOrDefault = farmAnimals.GetSavedTypeOrDefault(farmAnimal);
                  farmAnimal.UpdateFromData(savedTypeOrDefault);
                  var typeLog = new TypeLog(typeString, savedTypeOrDefault);
                  farmAnimals.AddAnimal(farmAnimal, typeLog);
                }
              }

          break;
        }

      if (farmAnimals.HasAnyAnimals())
      {
        var list = farmAnimals.Animals.Where(o => !animalIds.Contains(o.Id)).Select(o => o.Id).ToList();
        farmAnimals.RemoveAnimals(list);
      }

      farmAnimals.Write();
    }

    public static void OnSaved(SavedEventArgs e)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.ReloadAll();
    }

    public static void OnSaveLoaded(SaveLoadedEventArgs e)
    {
      Paritee.StardewValley.Core.Characters.FarmAnimal.ReloadAll();
    }
  }
}