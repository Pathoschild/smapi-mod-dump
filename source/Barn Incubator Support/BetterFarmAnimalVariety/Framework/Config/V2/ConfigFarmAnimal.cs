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
// Type: BetterFarmAnimalVariety.Framework.Config.V2.ConfigFarmAnimal
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using Newtonsoft.Json;

namespace BetterFarmAnimalVariety.Framework.Config.V2
{
  public class ConfigFarmAnimal
  {
    public ConfigFarmAnimalAnimalShop AnimalShop;
    public string[] Buildings;
    public string[] Types;

    [JsonConstructor]
    public ConfigFarmAnimal()
    {
    }

    public bool ShouldSerializeCategory()
    {
      return false;
    }

    public bool CanBePurchased()
    {
      return AnimalShop.Name != null && AnimalShop.Description != null && AnimalShop.Price != null &&
             AnimalShop.Icon != null;
    }
  }
}