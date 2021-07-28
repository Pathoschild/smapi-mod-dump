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
// Type: BetterFarmAnimalVariety.Framework.Helpers.FarmAnimals
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using System.Linq;
using BetterFarmAnimalVariety.Framework.Cache;
using StardewValley;
using FarmAnimal = Paritee.StardewValley.Core.Characters.FarmAnimal;

namespace BetterFarmAnimalVariety.Framework.Helpers
{
  internal class FarmAnimals
  {
    private static string CacheFileName => Constants.Mod.FarmAnimalsCacheFileName;

    public static Cache.FarmAnimals ReadCache()
    {
      return Mod.ReadCache<Cache.FarmAnimals>(CacheFileName);
    }

    public static void Write(Cache.FarmAnimals animals)
    {
      Mod.WriteCache(CacheFileName, animals);
    }

    public static void RemoveCategory(string category)
    {
      var animals = ReadCache();
      animals.RemoveCategory(category);
      Write(animals);
    }

    public static void AddOrReplaceCategory(FarmAnimalCategory category)
    {
      var animals = ReadCache();
      var index = animals.Categories.FindIndex(o => o.Category == category.Category);
      if (index == -1)
        animals.Categories.Add(category);
      else
        animals.Categories[index] = category;
      Write(animals);
    }

    public static bool CategoryExists(string category)
    {
      return ReadCache().CategoryExists(category);
    }

    public static List<FarmAnimalCategory> GetCategories()
    {
      return ReadCache().Categories;
    }

    public static Dictionary<string, List<string>> GroupTypesByCategory()
    {
      return ReadCache().GroupTypesByCategory();
    }

    public static Dictionary<string, List<string>> GroupPurchaseableTypesByCategory()
    {
      return ReadCache().GroupPurchaseableTypesByCategory();
    }

    public static FarmAnimalCategory GetCategory(string category)
    {
      return ReadCache().GetCategory(category);
    }

    public static List<Object> GetPurchaseAnimalStock(Farm farm)
    {
      return ReadCache().GetPurchaseAnimalStock(farm);
    }

    public static bool CanBePurchased(string category)
    {
      return ReadCache().CanBePurchased(category);
    }

    public static List<FarmAnimalCategory> GetVanillaCategories()
    {
      return FarmAnimal.GetVanillaCategories()
        .Select(o =>
          new FarmAnimalCategory(ModEntry.Instance.Helper.DirectoryPath, o)).ToList();
    }

    public static bool IsVanillaCategory(string category)
    {
      return FarmAnimal.IsVanillaCategory(category);
    }
  }
}