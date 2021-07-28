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
// Type: BetterFarmAnimalVariety.Framework.ContentPacks.Content
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BetterFarmAnimalVariety.Framework.Cache;
using BetterFarmAnimalVariety.Framework.Helpers;
using StardewModdingAPI;
using FarmAnimals = BetterFarmAnimalVariety.Framework.Helpers.FarmAnimals;

namespace BetterFarmAnimalVariety.Framework.ContentPacks
{
  internal class Content
  {
    public List<Category> Categories;

    public Content()
    {
    }

    public Content(List<Category> categories)
    {
      Categories = categories;
    }

    public void SetUp(IContentPack contentPack)
    {
      Assert.UniqueValues(Categories.Select(o => o.Category).ToList());
      foreach (var category in Categories)
        switch (category.Action)
        {
          case Category.Actions.Create:
            HandleCreateAction(contentPack, category);
            break;
          case Category.Actions.Update:
            HandleUpdateAction(contentPack, category);
            break;
          case Category.Actions.Remove:
            HandleRemoveAction(contentPack, category);
            break;
          default:
            throw new NotSupportedException(string.Format("{0} is not a valid action", category.Action));
        }
    }

    private FarmAnimalType CastSpritesToFullPaths(
      FarmAnimalType type,
      string directoryPath)
    {
      if (type.HasAdultSprite())
        type.Sprites.Adult = Path.Combine(directoryPath,
          ModEntry.Instance.Helper.Content.NormalizeAssetName(type.Sprites.Adult));
      if (type.HasBabySprite())
        type.Sprites.Baby = Path.Combine(directoryPath,
          ModEntry.Instance.Helper.Content.NormalizeAssetName(type.Sprites.Baby));
      if (type.HasReadyForHarvestSprite())
        type.Sprites.ReadyForHarvest = Path.Combine(directoryPath,
          ModEntry.Instance.Helper.Content.NormalizeAssetName(type.Sprites.ReadyForHarvest));
      return type;
    }

    public void HandleCreateAction(IContentPack contentPack, Category category)
    {
      Assert.UniqueFarmAnimalCategory(category.Category);
      Assert.UniqueValues(category.Types.Select(o => o.Type).ToList());
      category.Types = category.Types.Select(o => CastSpritesToFullPaths(o, contentPack.DirectoryPath)).ToList();
      if (category.CanBePurchased())
        category.AnimalShop.Icon = Path.Combine(contentPack.DirectoryPath,
          ModEntry.Instance.Helper.Content.NormalizeAssetName(category.AnimalShop.Icon));
      FarmAnimals.AddOrReplaceCategory(new FarmAnimalCategory(category));
    }

    public void HandleUpdateAction(IContentPack contentPack, Category category)
    {
      var category1 = FarmAnimals.GetCategory(category.Category) ?? new FarmAnimalCategory(category);
      if (category.Types != null)
      {
        var list = category.Types.Select(o => CastSpritesToFullPaths(o, contentPack.DirectoryPath)).ToList();
        if (category.ForceOverrideTypes)
          category1.Types = list;
        else
          foreach (var farmAnimalType1 in list)
          {
            var type = farmAnimalType1;
            if (category1.Types.FirstOrDefault(o => o.Type == type.Type) != null)
            {
              var farmAnimalType2 = type;
            }
            else
            {
              category1.Types.Add(type);
            }
          }
      }

      if (category.Buildings != null)
        category1.Buildings = category.ForceOverrideBuildings
          ? category.Buildings
          : category1.Buildings.Union(category.Buildings).ToList();
      if (category.ForceRemoveFromShop)
      {
        category1.AnimalShop = null;
      }
      else if (category.AnimalShop != null)
      {
        if (!category1.CanBePurchased())
          category1.AnimalShop = new FarmAnimalStock();
        if (category.AnimalShop.Name != null)
          category1.AnimalShop.Name = category.AnimalShop.Name;
        if (category.AnimalShop.Description != null)
          category1.AnimalShop.Description = category.AnimalShop.Description;
        if (category.AnimalShop.Icon != null)
          category1.AnimalShop.Icon = Path.Combine(contentPack.DirectoryPath,
            ModEntry.Instance.Helper.Content.NormalizeAssetName(category.AnimalShop.Icon));
        if (category.AnimalShop.Exclude != null)
          category1.AnimalShop.Exclude = category.ForceOverrideExclude || category1.AnimalShop.Exclude == null
            ? category.AnimalShop.Exclude
            : category1.AnimalShop.Exclude.Union(category.AnimalShop.Exclude).ToList();
      }

      FarmAnimals.AddOrReplaceCategory(category1);
    }

    public void HandleRemoveAction(IContentPack contentPack, Category category)
    {
      Assert.FarmAnimalCategoryExists(category.Category);
      FarmAnimals.RemoveCategory(category.Category);
    }
  }
}