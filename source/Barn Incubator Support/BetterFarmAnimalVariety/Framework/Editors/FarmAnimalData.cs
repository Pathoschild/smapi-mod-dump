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
// Type: BetterFarmAnimalVariety.Framework.Editors.FarmAnimalData
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System;
using System.Collections.Generic;
using System.Linq;
using BetterFarmAnimalVariety.Framework.Cache;
using BetterFarmAnimalVariety.Framework.Exceptions;
using BetterFarmAnimalVariety.Framework.Helpers;
using Paritee.StardewValley.Core.Utilities;
using StardewModdingAPI;
using FarmAnimals = BetterFarmAnimalVariety.Framework.Helpers.FarmAnimals;

namespace BetterFarmAnimalVariety.Framework.Editors
{
  internal class FarmAnimalData : IAssetEditor
  {
    private readonly IModHelper Helper;
    private readonly IMonitor Monitor;

    public FarmAnimalData(IModHelper helper, IMonitor monitor)
    {
      Helper = helper;
      Monitor = monitor;
    }

    public bool CanEdit<T>(IAssetInfo asset)
    {
      return asset.AssetNameEquals(Content.DataFarmAnimalsContentPath);
    }

    public void Edit<T>(IAssetData asset)
    {
      if (!asset.AssetNameEquals(Content.DataFarmAnimalsContentPath))
        return;
      var data = asset.AsDictionary<string, string>().Data;
      var locale = asset.Locale.Split('-')[0];
      foreach (var farmAnimalType in FarmAnimals.GetCategories().SelectMany(o => (IEnumerable<FarmAnimalType>) o.Types))
        if (farmAnimalType.Data != null)
        {
          data[farmAnimalType.Type] = farmAnimalType.LocalizeData(locale);
          data[farmAnimalType.Type] = SanitizeData(farmAnimalType.Data);
        }

      Monitor.Log("Data/FarmAnimals:\n" + string.Join("\n", data.Select(o => o.Key + ": " + o.Value)));
    }

    private string SanitizeData(string data)
    {
      var dataValue = Content.ParseDataValue(data);
      dataValue[2] = SanitizeItems(dataValue[2], "produce item");
      dataValue[3] = SanitizeItems(dataValue[3], "deluxe produce item");
      if (ModEntry.Instance.Helper.ModRegistry.IsLoaded("DIGUS.ANIMALHUSBANDRYMOD"))
        dataValue[23] = SanitizeItems(dataValue[23], "meat item");
      return string.Join('/'.ToString(), dataValue);
    }

    private string SanitizeItems(string indexStr, string whichField = "item")
    {
      var str = "-1";
      try
      {
        int index;
        Assert.ValidObject(Helper, indexStr, out index);
        indexStr = index.ToString();
      }
      catch (SaveNotLoadedException ex)
      {
        Monitor.Log("Cannot set " + whichField + " to \"" + indexStr + "\": " + ex.Message +
                    ". Will be temporarily set to \"none\" (" + str + ").");
        indexStr = str;
      }
      catch (Exception ex)
      {
        Monitor.Log(
          "Cannot set " + whichField + " to \"" + indexStr + "\": " + ex.Message + ". Will be set to \"none\" (" + str +
          ").", LogLevel.Debug);
        indexStr = str;
      }

      return indexStr;
    }
  }
}