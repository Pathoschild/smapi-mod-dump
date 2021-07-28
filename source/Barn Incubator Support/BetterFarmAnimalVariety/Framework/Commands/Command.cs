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
// Type: BetterFarmAnimalVariety.Framework.Commands.Command
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using BetterFarmAnimalVariety.Framework.Cache;
using StardewModdingAPI;

namespace BetterFarmAnimalVariety.Framework.Commands
{
  internal abstract class Command
  {
    protected readonly IModHelper Helper;
    protected readonly IMonitor Monitor;
    public string Description;
    public string Name;

    public Command(string name, string description, IModHelper helper, IMonitor monitor)
    {
      Name = name;
      Description = description;
      Helper = helper;
      Monitor = monitor;
    }

    public abstract void Callback(string command, string[] args);

    protected string DescribeFarmAnimalCategory(FarmAnimalCategory animal)
    {
      var str1 = "" + animal.Category + "\n" + "- Buildings: " + string.Join(",", animal.Buildings) + "\n";
      var str2 = (!animal.CanBePurchased()
        ? str1 + "- AnimalShop: null\n"
        : str1 + "- AnimalShop:\n" + "-- Name: " + animal.AnimalShop.Name + "\n" + "-- Description: " +
          animal.AnimalShop.Description + "\n" + "-- Icon: " + animal.AnimalShop.Icon + "\n") + "- Types:\n";
      foreach (var type in animal.Types)
      {
        str2 = str2 + "-- Type: " + type.Type + "\n";
        str2 = str2 + "--- Data: " + (type.Data ?? "null") + "\n";
        var flag1 = type.HasAdultSprite();
        var flag2 = type.HasBabySprite();
        var flag3 = type.HasReadyForHarvestSprite();
        if (flag1 | flag2 | flag3)
        {
          str2 += "--- Sprites:\n";
          str2 += flag1 ? "---- Adult: " + (type.Sprites.Adult ?? "null") + "\n" : "";
          str2 += flag2 ? "---- Baby: " + (type.Sprites.Baby ?? "null") + "\n" : "";
          str2 += flag3 ? "---- ReadyForHarvest: " + (type.Sprites.ReadyForHarvest ?? "null") + "\n" : "";
        }

        if (type.HasLocalization())
        {
          str2 += "--- Localization:\n";
          foreach (var keyValuePair in type.Localization)
            str2 = str2 + "---- " + keyValuePair.Key + ": " + string.Join(",", keyValuePair.Value) + "\n";
        }
        else
        {
          str2 += "--- Localization: null\n";
        }
      }

      return str2;
    }
  }
}