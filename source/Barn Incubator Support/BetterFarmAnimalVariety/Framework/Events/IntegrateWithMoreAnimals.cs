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
// Type: BetterFarmAnimalVariety.Framework.Events.IntegrateWithMoreAnimals
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using BetterFarmAnimalVariety.Framework.Api;
using BetterFarmAnimalVariety.Framework.Helpers;
using Paritee.StardewValley.Core.Characters;
using Paritee.StardewValley.Core.Utilities;
using StardewModdingAPI;

namespace BetterFarmAnimalVariety.Framework.Events
{
  internal class IntegrateWithMoreAnimals
  {
    public static void RegisterAnimals(IModHelper helper, IMonitor monitor)
    {
      IMoreAnimals api;
      Assert.ApiExists(helper, "Entoarox.MoreAnimals", out api);
      foreach (var entry in Content.LoadData<string, string>(Content.DataFarmAnimalsContentPath))
      {
        bool hasBaby;
        bool canShear;
        RegisterAnimalType(api, entry, out hasBaby, out canShear);
        monitor.Log(string.Format("Registered {0} (hasBaby:{1}, canShear:{2}) with {3}", (object) entry.Key,
          (object) hasBaby, (object) canShear, (object) "Entoarox.MoreAnimals"));
      }
    }

    private static bool HasBabyAsset(string type, string[] dataValues)
    {
      return FarmAnimal.TryBuildSpriteAssetName(type, true, false, out var _);
    }

    private static bool HasShearedAsset(string type, string[] dataValues)
    {
      bool result;
      return bool.TryParse(dataValues[14], out result) && result &&
             FarmAnimal.TryBuildSpriteAssetName(type, false, true, out var _);
    }

    private static void RegisterAnimalType(
      IMoreAnimals api,
      KeyValuePair<string, string> entry,
      out bool hasBaby,
      out bool canShear)
    {
      var dataValue = Content.ParseDataValue(entry.Value);
      hasBaby = HasBabyAsset(entry.Key, dataValue);
      canShear = HasShearedAsset(entry.Key, dataValue);
      api.RegisterAnimalType(entry.Key, hasBaby, canShear);
    }
  }
}