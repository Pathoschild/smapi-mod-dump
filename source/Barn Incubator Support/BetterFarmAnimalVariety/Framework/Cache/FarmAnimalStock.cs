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
// Type: BetterFarmAnimalVariety.Framework.Cache.FarmAnimalStock
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using System.IO;
using System.Linq;
using BetterFarmAnimalVariety.Framework.Helpers;
using Newtonsoft.Json;
using Paritee.StardewValley.Core.Characters;

namespace BetterFarmAnimalVariety.Framework.Cache
{
  internal class FarmAnimalStock
  {
    [JsonProperty(Order = 1)] public string Description;

    [JsonProperty(Order = 3)] public List<string> Exclude;

    [JsonProperty(Order = 2)] public string Icon;

    [JsonProperty(Order = 0)] public string Name;

    public FarmAnimalStock()
    {
    }

    public FarmAnimalStock(LivestockCategory livestockCategory)
    {
      Name = livestockCategory.AnimalShop.Name;
      Description = livestockCategory.AnimalShop.Description;
      Icon = GetDefaultIconPath(livestockCategory.ToString());
      Exclude = livestockCategory.AnimalShop.Exclude == null || !livestockCategory.AnimalShop.Exclude.Any()
        ? new List<string>()
        : livestockCategory.AnimalShop.Exclude.Select(o => o.ToString()).ToList();
    }

    public string GetDefaultIconPath(string category)
    {
      return Mod.GetShortAssetPath(Path.Combine("AnimalShop", category.Replace(" ", "") + ".png"));
    }
  }
}