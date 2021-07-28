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
// Type: BetterFarmAnimalVariety.Framework.Constants.Mod
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using System.IO;
using Paritee.StardewValley.Core.Characters;

namespace BetterFarmAnimalVariety.Framework.Constants
{
  internal class Mod
  {
    public const string Key = "Paritee.BetterFarmAnimalVariety";
    public const string ConfigFileName = "config.json";
    public const string AnimalShopIconDirectory = "AnimalShop";
    public const string AnimalShopIconExtension = ".png";
    public const string FarmAnimalsSaveDataKey = "farm-animals";
    public const string CacheDirectory = "cache";
    public const string AssetsDirectory = "assets";
    public static string FarmAnimalsCacheFileName = "farm-animals.json";

    public static List<Animal> RestrictedFarmAnimalTypes => new List<Animal>
    {
      Pet.Cat,
      Pet.Dog,
      Mount.Horse
    };

    public static string CacheFullPath => Path.Combine(ModEntry.Instance.Helper.DirectoryPath, "cache");
  }
}