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
// Type: BetterFarmAnimalVariety.Framework.Cache.FarmAnimalCategory
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using System.IO;
using System.Linq;
using BetterFarmAnimalVariety.Framework.ContentPacks;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Paritee.StardewValley.Core.Characters;
using Paritee.StardewValley.Core.Utilities;
using StardewValley;

namespace BetterFarmAnimalVariety.Framework.Cache
{
  internal class FarmAnimalCategory
  {
    [JsonProperty(Order = 3)] public FarmAnimalStock AnimalShop;

    [JsonProperty(Order = 2)] public List<string> Buildings = new List<string>();

    [JsonProperty(Order = 0)] public string Category;

    [JsonProperty(Order = 1)] public List<FarmAnimalType> Types = new List<FarmAnimalType>();

    public FarmAnimalCategory()
    {
    }

    public FarmAnimalCategory(Category category)
    {
      Category = category.Category;
      Types = category.Types;
      Buildings = category.Buildings;
      AnimalShop = category.AnimalShop;
    }

    public FarmAnimalCategory(string assetSourceDirectory, LivestockCategory category)
    {
      Category = category.ToString();
      Types = category.Types.Select(o => new FarmAnimalType(o)).ToList();
      Buildings = category.Buildings.ToList();
      AnimalShop = category.CanBePurchased() ? new FarmAnimalStock(category) : null;
      if (!CanBePurchased())
        return;
      AnimalShop.Icon = Path.Combine(assetSourceDirectory, AnimalShop.Icon);
    }

    public Texture2D GetAnimalShopIconTexture()
    {
      return CanBePurchased() ? Mod.LoadTexture(AnimalShop.Icon) : null;
    }

    public bool CanBePurchased()
    {
      return AnimalShop != null;
    }

    public bool CanBePurchased(string type)
    {
      return CanBePurchased() && (AnimalShop.Exclude == null || !AnimalShop.Exclude.Contains(type));
    }

    public Object ToAnimalAvailableForPurchase(Farm farm)
    {
      return !CanBePurchased()
        ? null
        : Paritee.StardewValley.Core.Locations.AnimalShop.FormatAsAnimalAvailableForPurchase(farm, Category,
          AnimalShop.Name,
          Types.Select(o => o.Type).ToArray(),
          Buildings.ToArray());
    }
  }
}