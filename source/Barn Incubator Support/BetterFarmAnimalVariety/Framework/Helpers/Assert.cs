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
// Type: BetterFarmAnimalVariety.Framework.Helpers.Assert
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BetterFarmAnimalVariety.Framework.Decorators;
using BetterFarmAnimalVariety.Framework.Events;
using BetterFarmAnimalVariety.Framework.Exceptions;
using Paritee.StardewValley.Core.Locations;
using Paritee.StardewValley.Core.Utilities;
using StardewModdingAPI;
using Location = BetterFarmAnimalVariety.Framework.Decorators.Location;
using Object = Paritee.StardewValley.Core.Objects.Object;

namespace BetterFarmAnimalVariety.Framework.Helpers
{
  internal class Assert
  {
    public static void VersionIsSupported(string version, string target)
    {
      if (!(version == target))
        throw new NotSupportedException("Version " + version + " is not supported");
    }

    public static void ApiExists<TInterface>(IModHelper helper, string key, out TInterface api) where TInterface : class
    {
      if (!Mod.TryGetApi(helper, key, out api))
        throw new ApiNotFoundException(key);
    }

    public static void SaveLoaded()
    {
      if (!Game.IsSaveLoaded())
        throw new SaveNotLoadedException();
    }

    public static void SaveNotLoaded()
    {
      if (Game.IsSaveLoaded())
        throw new ApplicationException("Save has been loaded");
    }

    public static void ArgumentInRange(int total, int expected)
    {
      if (total > expected)
        throw new ArgumentOutOfRangeException("Use quotation marks (\") around your text if you are using spaces");
    }

    public static void RequiredArgumentOrder(int length, int expected, string argument)
    {
      if (length < expected)
        throw new ArgumentException("\"" + argument + "\" is required");
    }

    public static void ValidStringLength(
      string argument,
      string str,
      int minLength,
      int maxLength = -1)
    {
      if (str.Length < minLength)
        throw new ArgumentException(string.Format("\"{0}\" must be at least {1} characters", argument, minLength));
      if (maxLength >= 0 && str.Length > maxLength)
        throw new ArgumentException(string.Format("\"{0}\" must be at most {1} characters", argument, minLength));
    }

    public static void FarmAnimalTypeExists(string type)
    {
      FarmAnimalTypesExist(new List<string>
      {
        type
      });
    }

    public static void FarmAnimalTypesExist(List<string> types)
    {
      var dictionary = Content.LoadData<string, string>(Content.DataFarmAnimalsContentPath);
      foreach (var type in types)
        if (!dictionary.ContainsKey(type))
          throw new NotImplementedException("\"" + type + "\" does not exist in Data/FarmAnimals");
    }

    public static void FarmAnimalTypeIsNotRestricted(string type)
    {
      if (Constants.Mod.RestrictedFarmAnimalTypes.Select(o => o.ToString().ToLower()).Contains(type.ToLower()))
        throw new NotSupportedException("\"" + type + "\" is a restricted type and cannot be used");
    }

    public static void FarmAnimalTypesAreNotRestricted(List<string> types)
    {
      foreach (var type in types)
        FarmAnimalTypeIsNotRestricted(type);
    }

    public static void ValidObject(IModHelper helper, string strIndex, out int index)
    {
      if (!Context.IsWorldReady)
        throw new SaveNotLoadedException();
      if (!int.TryParse(strIndex, out index))
      {
        if (!IntegrateWithJsonAssets.TryParseObjectName(strIndex, out index))
          throw new NotImplementedException("\"" + strIndex + "\" is not a known Object");
      }
      else if (Paritee.StardewValley.Core.Characters.FarmAnimal.IsProduceAnItem(index) && !Object.ObjectExists(index))
      {
        throw new NotImplementedException("\"" + strIndex + "\" is not a known Object");
      }
    }

    public static void BuildingsExist(List<string> buildings)
    {
      var dictionary = Content.Load<Dictionary<string, string>>(Content.DataBlueprintsContentPath);
      foreach (var building in buildings)
        if (!dictionary.ContainsKey(building))
          throw new NotImplementedException("\"" + building + "\" does not exist in Data/Blueprints");
    }

    public static void ValidBoolean(string strBool, string argument, out bool result)
    {
      if (!bool.TryParse(strBool, out result))
      {
        var strArray = new string[5]
        {
          argument,
          " must be ",
          null,
          null,
          null
        };
        var flag = true;
        strArray[2] = flag.ToString().ToLower();
        strArray[3] = " or ";
        flag = false;
        strArray[4] = flag.ToString().ToLower();
        throw new FormatException(string.Concat(strArray));
      }
    }

    public static void FarmAnimalCategoryExists(string category)
    {
      if (!FarmAnimals.CategoryExists(category))
        throw new NotImplementedException(category + " category does not exist");
    }

    public static void AtLeastOneTypeRequired(List<string> types)
    {
      if (types.Count < 1)
        throw new ArgumentException("At least one type is required");
    }

    public static void AtLeastOneBuildingRequired(List<string> buildings)
    {
      if (buildings.Count < 1)
        throw new ArgumentException("At least one building is required");
    }

    public static void ChangeInPurchaseState(
      string animalShop,
      bool canBePurchasedNow,
      out bool result)
    {
      ValidBoolean(animalShop, nameof(animalShop), out result);
      if (result.Equals(true) & canBePurchasedNow)
        throw new ArgumentException("Already available in the animal shop");
      if (result.Equals(false) && !canBePurchasedNow)
        throw new ArgumentException("Already not available in the animal shop");
    }

    public static void FarmAnimalCanBePurchased(string category)
    {
      if (!FarmAnimals.CanBePurchased(category))
        throw new NotImplementedException("\"" + category + "\" is not available in the animal shop");
    }

    public static void ValidMoneyAmount(string amount)
    {
      int result;
      if (!int.TryParse(amount, out result))
        throw new FormatException("Amount must be a positive number");
      ValidMoneyAmount(result);
    }

    public static void ValidMoneyAmount(int amount)
    {
      if (amount < 0)
        throw new FormatException("Amount must be a positive number");
    }

    public static void FileExists(string filePath)
    {
      if (!File.Exists(filePath))
        throw new FileNotFoundException(filePath + " does not exist");
    }

    public static void ValidAnimalShopIcon(string filePath)
    {
      FileExists(filePath);
      ValidFileExtension(filePath, ".png");
    }

    public static void ValidFileExtension(string fileName, string extension)
    {
      if (!Path.GetExtension(fileName).ToLower().Equals(extension))
        throw new FormatException(fileName + " must be a " + extension);
    }

    public static void UniqueValues<T>(List<T> values)
    {
      var objSet = new HashSet<T>();
      foreach (var obj in values)
        if (!objSet.Add(obj))
          throw new FormatException(string.Format("Multiple instances of \"{0}\" exists in the same set", obj));
    }

    public static void UniqueFarmAnimalCategory(string category)
    {
      if (FarmAnimals.ReadCache().CategoryExists(category))
        throw new ArgumentException("\"" + category + "\" category already exists");
    }

    public static void Outdoors(Location moddedLocation)
    {
      if (!moddedLocation.IsOutdoors())
        throw new ApplicationException("Location is not outdoors");
    }

    public static void NotRaining()
    {
      if (Weather.IsRaining())
        throw new ApplicationException("It is raining");
    }

    public static void NotWinter()
    {
      if (Season.IsWinter())
        throw new ApplicationException("It is winter");
    }

    public static void ProduceIsAnItem(int produceIndex)
    {
      if (!Paritee.StardewValley.Core.Characters.FarmAnimal.IsProduceAnItem(produceIndex))
        throw new KeyNotFoundException(string.Format("\"{0}\" is not produce", produceIndex));
    }

    public static void CanFindProduce(FarmAnimal moddedAnimal)
    {
      if (!moddedAnimal.CanFindProduce())
        throw new ApplicationException(string.Format("{0} cannot find produce", moddedAnimal.GetType()));
    }
  }
}