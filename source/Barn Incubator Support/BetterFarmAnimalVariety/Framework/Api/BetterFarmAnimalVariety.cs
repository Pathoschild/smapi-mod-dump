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
// Type: BetterFarmAnimalVariety.Framework.Api.BetterFarmAnimalVariety
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using System.Linq;
using BetterFarmAnimalVariety.Framework.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Paritee.StardewValley.Core.Utilities;
using StardewValley;
using Mod = BetterFarmAnimalVariety.Framework.Helpers.Mod;

namespace BetterFarmAnimalVariety.Framework.Api
{
  public class BetterFarmAnimalVariety : IBetterFarmAnimalVariety
  {
    private readonly ModConfig Config;

    public BetterFarmAnimalVariety(ModConfig config)
    {
      Config = config;
    }

    public bool IsEnabled()
    {
      return Config.IsEnabled;
    }

    public Dictionary<string, List<string>> GetFarmAnimalCategories()
    {
      return FarmAnimals.GetCategories().ToDictionary(o => o.Category,
        o =>
          o.Types.Select(t => t.Type).ToList());
    }

    public List<Object> GetAnimalShopStock(Farm farm)
    {
      return FarmAnimals.GetPurchaseAnimalStock(farm);
    }

    public Dictionary<string, Texture2D> GetAnimalShopIcons()
    {
      return FarmAnimals.GetCategories().Where(o => o.CanBePurchased())
        .ToDictionary(o => o.Category,
          o => o.GetAnimalShopIconTexture());
    }

    public string GetRandomAnimalShopType(string category, Farmer farmer)
    {
      var farmer1 = new Decorators.Farmer(farmer);
      var types1 = FarmAnimals.GroupPurchaseableTypesByCategory()[category];
      var types2 = farmer1.SanitizeBlueChickens(types1);
      var stringList = farmer1.SanitizeAffordableTypes(types2);
      return stringList[Random.Next(stringList.Count)];
    }

    public Dictionary<long, KeyValuePair<string, string>> GetFarmAnimalTypeHistory()
    {
      Assert.SaveLoaded();
      return Mod.ReadSaveData<SaveData.FarmAnimals>("farm-animals").Animals
        .ToDictionary(
          o => o.Id, o => o.TypeLog.ConvertToKeyValuePair());
    }
  }
}