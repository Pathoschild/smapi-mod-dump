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
// Type: BetterFarmAnimalVariety.Framework.Cache.FarmAnimals
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace BetterFarmAnimalVariety.Framework.Cache
{
  internal class FarmAnimals
  {
    public List<FarmAnimalCategory> Categories = new List<FarmAnimalCategory>();

    public FarmAnimals()
    {
    }

    public FarmAnimals(List<FarmAnimalCategory> categories)
    {
      Categories = categories;
    }

    public bool CategoryExists(string category)
    {
      return Categories != null && Categories.Exists(o => o.Category.Equals(category));
    }

    public Dictionary<string, List<string>> GroupTypesByCategory()
    {
      return Categories.ToDictionary(o => o.Category,
        o =>
          o.Types.Select(t => t.Type).ToList());
    }

    private List<string> TypesInShop(FarmAnimalCategory category)
    {
      return category.CanBePurchased()
        ? category.Types
          .Where(o =>
            category.AnimalShop.Exclude == null || !category.AnimalShop.Exclude.Contains(o.Type))
          .Select(o => o.Type).ToList()
        : new List<string>();
    }

    public Dictionary<string, List<string>> GroupPurchaseableTypesByCategory()
    {
      return Categories
        .ToDictionary(o => o.Category,
          TypesInShop)
        .Where(kvp => kvp.Value.Any())
        .ToDictionary(
          kvp => kvp.Key,
          kvp => kvp.Value);
    }

    public FarmAnimalCategory GetCategory(string category)
    {
      return Categories.FirstOrDefault(o => o.Category.Equals(category));
    }

    public void RemoveCategory(string category)
    {
      Categories.RemoveAll(o => o.Category.Equals(category));
    }

    public List<Object> GetPurchaseAnimalStock(Farm farm)
    {
      return Categories.Where(o => o.CanBePurchased())
        .Select(
          o => o.ToAnimalAvailableForPurchase(farm))
        .ToList();
    }

    public bool CanBePurchased(string category)
    {
      return Categories.Exists(o => o.Category.Equals(category) && o.CanBePurchased());
    }
  }
}