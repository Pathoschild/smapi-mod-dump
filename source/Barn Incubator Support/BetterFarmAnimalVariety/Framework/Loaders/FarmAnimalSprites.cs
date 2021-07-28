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
// Type: BetterFarmAnimalVariety.Framework.Loaders.FarmAnimalSprites
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System;
using System.Collections.Generic;
using System.Linq;
using BetterFarmAnimalVariety.Framework.Cache;
using Paritee.StardewValley.Core.Characters;
using StardewModdingAPI;
using FarmAnimals = BetterFarmAnimalVariety.Framework.Helpers.FarmAnimals;
using Mod = Paritee.StardewValley.Core.Utilities.Mod;

namespace BetterFarmAnimalVariety.Framework.Loaders
{
  internal class FarmAnimalSprites : IAssetLoader
  {
    public bool CanLoad<T>(IAssetInfo asset)
    {
      foreach (var farmAnimalType in FarmAnimals.GetCategories().SelectMany(o => (IEnumerable<FarmAnimalType>) o.Types))
        if (farmAnimalType.HasAdultSprite() &&
            asset.AssetNameEquals(FarmAnimal.BuildSpriteAssetName(farmAnimalType.Type)) ||
            farmAnimalType.HasBabySprite() &&
            asset.AssetNameEquals(FarmAnimal.BuildSpriteAssetName(farmAnimalType.Type, true)) ||
            farmAnimalType.HasReadyForHarvestSprite() &&
            asset.AssetNameEquals(FarmAnimal.BuildSpriteAssetName(farmAnimalType.Type, isSheared: true)))
          return true;
      return false;
    }

    public T Load<T>(IAssetInfo asset)
    {
      foreach (var farmAnimalType in FarmAnimals.GetCategories()
        .SelectMany(
          o => (IEnumerable<FarmAnimalType>) o.Types))
      {
        if (farmAnimalType.HasAdultSprite() &&
            asset.AssetNameEquals(FarmAnimal.BuildSpriteAssetName(farmAnimalType.Type)))
          return (T) (object) Mod.LoadTexture(farmAnimalType.Sprites.Adult);
        if (farmAnimalType.HasBabySprite() &&
            asset.AssetNameEquals(FarmAnimal.BuildSpriteAssetName(farmAnimalType.Type, true)))
          return (T) (object) Mod.LoadTexture(farmAnimalType.Sprites.Baby);
        if (farmAnimalType.HasReadyForHarvestSprite() &&
            asset.AssetNameEquals(FarmAnimal.BuildSpriteAssetName(farmAnimalType.Type, isSheared: true)))
          return (T) (object) Mod.LoadTexture(farmAnimalType.Sprites.ReadyForHarvest);
      }

      throw new InvalidOperationException("Unexpected asset '" + asset.AssetName + "'.");
    }
  }
}