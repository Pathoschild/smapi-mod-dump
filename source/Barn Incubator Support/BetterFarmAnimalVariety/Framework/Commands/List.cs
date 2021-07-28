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
// Type: BetterFarmAnimalVariety.Framework.Commands.List
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using BetterFarmAnimalVariety.Framework.Helpers;
using StardewModdingAPI;

namespace BetterFarmAnimalVariety.Framework.Commands
{
  internal class List : Command
  {
    public List(IModHelper helper, IMonitor monitor)
      : base("livestock_categories", "List the farm animal categories and types.\nUsage: livestock_categories", helper,
        monitor)
    {
    }

    public override void Callback(string command, string[] args)
    {
      var message = "Listing farm animals\n";
      FarmAnimals.ReadCache();
      foreach (var category in FarmAnimals.GetCategories())
        message += DescribeFarmAnimalCategory(category);
      Monitor.Log(message, LogLevel.Info);
    }
  }
}