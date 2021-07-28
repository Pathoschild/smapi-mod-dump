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
// Type: BetterFarmAnimalVariety.Framework.Cache.FarmAnimalType
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using System.Linq;
using Paritee.StardewValley.Core.Characters;
using Paritee.StardewValley.Core.Utilities;

namespace BetterFarmAnimalVariety.Framework.Cache
{
  internal class FarmAnimalType
  {
    public enum LocalizationIndex
    {
      DisplayType,
      DisplayBuilding
    }

    public string Data;
    public double DeluxeProduceLuck;
    public Dictionary<string, string[]> Localization = new Dictionary<string, string[]>();
    public FarmAnimalSprites Sprites = new FarmAnimalSprites();
    public string Type;

    public FarmAnimalType()
    {
    }

    public FarmAnimalType(string type, double deluxeProduceLuck)
    {
      Type = type;
      DeluxeProduceLuck = deluxeProduceLuck;
    }

    public FarmAnimalType(Livestock livestock)
    {
      Type = livestock.ToString();
      DeluxeProduceLuck = livestock.DeluxeProduceLuck;
    }

    public FarmAnimalType(
      string type,
      string data,
      double deluxeProduceLuck,
      string babySprite,
      string adultSprite,
      string readyForHarvestSprite,
      Dictionary<string, string[]> localization)
    {
      Type = type;
      Data = data;
      DeluxeProduceLuck = deluxeProduceLuck;
      Sprites = new FarmAnimalSprites(babySprite, adultSprite, readyForHarvestSprite);
      Localization = localization;
    }

    public string LocalizeData(string locale)
    {
      if (Localization == null || !Localization.ContainsKey(locale))
        return Data;
      var dataValue = Content.ParseDataValue(Data);
      dataValue[25] = Localization[locale][0];
      dataValue[26] = Localization[locale][1];
      return string.Join('/'.ToString(), dataValue);
    }

    public bool HasLocalization()
    {
      return Localization != null && Localization.Any();
    }

    public bool HasSprites()
    {
      return Sprites != null;
    }

    public bool HasAdultSprite()
    {
      return HasSprites() && Sprites.Adult != null;
    }

    public bool HasBabySprite()
    {
      return HasSprites() && Sprites.Baby != null;
    }

    public bool HasReadyForHarvestSprite()
    {
      return HasSprites() && Sprites.ReadyForHarvest != null;
    }
  }
}