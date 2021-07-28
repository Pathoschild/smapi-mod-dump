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
// Type: BetterFarmAnimalVariety.Framework.Events.RefreshCache
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System;
using System.Collections.Generic;
using System.Linq;
using BetterFarmAnimalVariety.Framework.Helpers;
using StardewModdingAPI;

namespace BetterFarmAnimalVariety.Framework.Events
{
  internal class RefreshCache
  {
    public static void ValidateCachedFarmAnimals(IModHelper helper, IMonitor monitor)
    {
      var animals = FarmAnimals.ReadCache();
      var categoriesToBeRemoved = new List<string>();
      foreach (var category in animals.Categories)
        try
        {
          Assert.ValidStringLength("category", category.Category, 1);
          var typesToBeRemoved = new List<string>();
          foreach (var type in category.Types)
            try
            {
              Assert.FarmAnimalTypeIsNotRestricted(type.Type);
              Assert.FarmAnimalTypeExists(type.Type);
            }
            catch (Exception ex)
            {
              monitor.Log(type.Type + " type will not load: " + ex.Message, LogLevel.Warn);
              typesToBeRemoved.Add(type.Type);
            }

          if (typesToBeRemoved.Any())
            category.Types.RemoveAll(o => typesToBeRemoved.Contains(o.Type));
          Assert.AtLeastOneTypeRequired(category.Types.Select(o => o.Type).ToList());
          Assert.AtLeastOneBuildingRequired(category.Buildings);
          Assert.BuildingsExist(category.Buildings);
          if (category.CanBePurchased())
          {
            Assert.ValidStringLength("name", category.AnimalShop.Name, 1);
            Assert.ValidStringLength("description", category.AnimalShop.Description, 1);
            Assert.FileExists(category.AnimalShop.Icon);
            Assert.ValidFileExtension(category.AnimalShop.Icon, ".png");
            if (category.AnimalShop.Exclude != null)
              Assert.FarmAnimalTypesExist(category.AnimalShop.Exclude);
          }
        }
        catch (Exception ex)
        {
          monitor.Log(category.Category + " category will not load: " + ex.Message, LogLevel.Warn);
          categoriesToBeRemoved.Add(category.Category);
        }

      if (categoriesToBeRemoved.Any())
        animals.Categories.RemoveAll(o => categoriesToBeRemoved.Contains(o.Category));
      FarmAnimals.Write(animals);
    }

    public static void SeedCacheWithVanillaFarmAnimals()
    {
      FarmAnimals.Write(new Cache.FarmAnimals(FarmAnimals.GetVanillaCategories()));
    }
  }
}