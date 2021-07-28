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
// Type: BetterFarmAnimalVariety.Framework.SaveData.FarmAnimals
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using System.Linq;
using BetterFarmAnimalVariety.Framework.Helpers;

namespace BetterFarmAnimalVariety.Framework.SaveData
{
  public class FarmAnimals
  {
    public List<FarmAnimal> Animals = new List<FarmAnimal>();

    public void Write()
    {
      Mod.WriteSaveData("farm-animals", this);
    }

    public bool HasAnyAnimals()
    {
      return Animals.Any();
    }

    public FarmAnimal GetAnimal(long id)
    {
      return Animals.FirstOrDefault(o => o.Id == id);
    }

    public void AddAnimals(List<FarmAnimal> animalToBeAdded)
    {
      foreach (var animalToBeAdded1 in animalToBeAdded)
        AddAnimal(animalToBeAdded1);
    }

    public void AddAnimal(FarmAnimal animalToBeAdded)
    {
      if (GetAnimal(animalToBeAdded.Id) == null)
        Animals.Add(animalToBeAdded);
    }

    public void AddAnimal(Decorators.FarmAnimal animalToBeAdded)
    {
      var typeLog = new TypeLog(animalToBeAdded.GetTypeString(), animalToBeAdded.GetTypeString());
      AddAnimal(animalToBeAdded, typeLog);
    }

    public void AddAnimal(Decorators.FarmAnimal animalToBeAdded, TypeLog typeLog)
    {
      AddAnimal(new FarmAnimal(animalToBeAdded.GetUniqueId(), typeLog));
    }

    public void RemoveAnimals(List<long> ids)
    {
      if (!ids.Any())
        return;
      Animals = Animals.Where(o => !ids.Contains(o.Id)).ToList();
    }

    public void RemoveAnimal(long id)
    {
      RemoveAnimals(new List<long>
      {
        id
      });
    }

    public bool AnimalExists(long myId)
    {
      return Animals.Exists(o => o.Id == myId);
    }

    public string GetSavedTypeOrDefault(StardewValley.FarmAnimal animal)
    {
      return GetSavedTypeOrDefault(new Decorators.FarmAnimal(animal));
    }

    public string GetSavedTypeOrDefault(Decorators.FarmAnimal moddedAnimal)
    {
      return GetSavedTypeOrDefault(moddedAnimal.GetUniqueId(), moddedAnimal.IsCoopDweller());
    }

    public string GetSavedTypeOrDefault(long animalId, bool isCoop)
    {
      var animal = GetAnimal(animalId);
      return animal == null
        ? Paritee.StardewValley.Core.Characters.FarmAnimal.GetDefaultType(isCoop)
        : animal.GetSavedType();
    }

    public TypeLog GetTypeLog(long myId)
    {
      return GetAnimal(myId)?.TypeLog;
    }

    public void OverwriteFarmAnimal(ref Decorators.FarmAnimal moddedAnimal, string requestedType)
    {
      if (!moddedAnimal.HasName())
        return;
      var typeLog = GetTypeLog(moddedAnimal.GetUniqueId());
      var type = typeLog == null ? requestedType ?? moddedAnimal.GetTypeString() : typeLog.Current;
      moddedAnimal.UpdateFromData(type);
    }
  }
}