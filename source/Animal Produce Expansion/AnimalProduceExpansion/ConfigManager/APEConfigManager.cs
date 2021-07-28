/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/AnimalProduceExpansion
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using AnimalProduceExpansion.API;
using AnimalProduceExpansion.Data;
using StardewValley;

namespace AnimalProduceExpansion.ConfigManager
{
  internal partial class ConfigManager
  {
    public ConfigManager(Config config) =>
      _config = config;


    internal bool HasAnimal(string animalName) =>
      _config.AnimalDrops.Any(a => a.Animal == animalName);

    internal bool HasExtraDrop(string animalName) =>
      _config.AnimalDrops.Last(a => a.Animal == animalName).ChanceForExtraDrop > 0;

    internal int GetMainDropForAnimal(string animalName) =>
      GetObjectId(GetDroppedItem(_config.AnimalDrops.FirstOrDefault(a => a.Animal == animalName)));

    internal int GetExtraDropForAnimal(string animalName) =>
      GetObjectId(GetExtraDrop(_config.AnimalDrops.FirstOrDefault(a => a.Animal == animalName)));

    private static int GetObjectId(string itemName)
    {
      if (int.TryParse(itemName, out var itemNumber))
        return itemNumber;

      var objectId = Api.GetApi<IJsonAssetsApi>().GetObjectId(itemName);
      if (objectId != -1) return objectId;
      return Game1.objectInformation.Any(oi => oi.Value.Split('/')[0] == itemName)
        ? Game1.objectInformation.FirstOrDefault(oi => oi.Value.Split('/')[0] == itemName).Key
        : objectId;
    }


    public static string GetWeightedDrop(IList<Drop> itemList)
    {
      var newRand = Api.GetApi<IRandomApi>().GetNewRandom();
      var newDbl = newRand.NextDouble();
      var cumulative = itemList[itemList.Count - 1].Cumulative;
      var value = newDbl * cumulative;
      return itemList.First(drop => drop.Cumulative >= value).Item;
    }

    internal string GetDroppedItem(AnimalDrops animalDrops)
    {
      if (!animalDrops.Cached)
        BuildCumulative(animalDrops);
      animalDrops.Cached = true;
      return GetWeightedDrop(animalDrops.Drops);
    }

    internal string GetExtraDrop(AnimalDrops animalDrops) =>
      animalDrops.ChanceForExtraDrop == 0
        ? string.Empty
        : Api.GetApi<IRandomApi>().GetNewRandom().NextDouble() <= animalDrops.ChanceForExtraDrop
          ? GetWeightedDrop(animalDrops.ExtraDrops.Any() ? animalDrops.ExtraDrops : animalDrops.Drops)
          : string.Empty;

    private static void BuildCumulative(AnimalDrops animalDrops)
    {
      double cumulative = 0;
      foreach (var t in animalDrops.Drops)
      {
        cumulative += t.Weight;
        t.Cumulative = cumulative;
      }

      if (!animalDrops.ExtraDrops.Any()) return;

      cumulative = 0;
      foreach (var t in animalDrops.ExtraDrops)
      {
        cumulative += t.Weight;
        t.Cumulative = cumulative;
      }
    }
  }
}