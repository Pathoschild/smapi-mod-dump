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
// Type: BetterFarmAnimalVariety.Framework.Api.IBetterFarmAnimalVariety
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace BetterFarmAnimalVariety.Framework.Api
{
  public interface IBetterFarmAnimalVariety
  {
    bool IsEnabled();

    Dictionary<string, List<string>> GetFarmAnimalCategories();

    List<Object> GetAnimalShopStock(Farm farm);

    Dictionary<string, Texture2D> GetAnimalShopIcons();

    string GetRandomAnimalShopType(string category, Farmer farmer);

    Dictionary<long, KeyValuePair<string, string>> GetFarmAnimalTypeHistory();
  }
}