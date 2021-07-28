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
// Type: BetterFarmAnimalVariety.Framework.Events.MigrateDeprecatedConfig
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BetterFarmAnimalVariety.Framework.Cache;
using BetterFarmAnimalVariety.Framework.Constants;
using BetterFarmAnimalVariety.Framework.ContentPacks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Paritee.StardewValley.Core.Characters;
using StardewModdingAPI;
using FarmAnimals = BetterFarmAnimalVariety.Framework.Helpers.FarmAnimals;

namespace BetterFarmAnimalVariety.Framework.Events
{
  internal class MigrateDeprecatedConfig
  {
    public static void OnEntry(ModEntry mod, string targetFormat, out ModConfig config)
    {
      var deprecatedConfig =
        (Config.V2.ModConfig) JsonConvert.DeserializeObject<Config.V2.ModConfig>(
          File.ReadAllText(Path.Combine(ModEntry.Instance.Helper.DirectoryPath, "config.json")));
      if (deprecatedConfig.Format == null || int.Parse(deprecatedConfig.Format) > int.Parse(targetFormat) ||
          !ToCurrentFormat(mod, deprecatedConfig, targetFormat, out config))
        throw new FormatException("Invalid config format. " + mod.ModManifest.Version.ToString() + " requires format:" +
                                  mod.ModManifest.Version.MajorVersion + ".");
    }

    public static bool ToCurrentFormat<T>(
      ModEntry mod,
      T deprecatedConfig,
      string targetFormat,
      out ModConfig config)
    {
      if ((object) deprecatedConfig is Config.V2.ModConfig)
      {
        HandleV2(mod, (Config.V2.ModConfig) Convert.ChangeType(deprecatedConfig, typeof(Config.V2.ModConfig)),
          targetFormat, out config);
        return true;
      }

      config = new ModConfig();
      return false;
    }

    public static void HandleV2(
      ModEntry mod,
      Config.V2.ModConfig deprecatedConfig,
      string targetFormat,
      out ModConfig config)
    {
      config = new ModConfig
      {
        Format = targetFormat,
        IsEnabled = deprecatedConfig.IsEnabled
      };
      CreateContentPack(mod, deprecatedConfig);
    }

    public static bool CreateContentPack(ModEntry mod, Config.V2.ModConfig deprecatedConfig)
    {
      var str1 = FarmAnimal.VoidChicken.ToString();
      var content1 = new Content(new List<Category>());
      var stringList1 = new List<string>();
      if (deprecatedConfig.FarmAnimals == null)
        return false;
      foreach (var farmAnimal in deprecatedConfig.FarmAnimals)
      {
        var action = FarmAnimals.IsVanillaCategory(farmAnimal.Key) ? Category.Actions.Update : Category.Actions.Create;
        FarmAnimalStock farmAnimalStock = null;
        var flag1 = true;
        var flag2 = false;
        if (farmAnimal.Value.CanBePurchased())
        {
          int.TryParse(farmAnimal.Value.AnimalShop.Price, out var _);
          var stringList2 = new List<string>();
          if (!deprecatedConfig.IsChickenCategory(farmAnimal.Key) || farmAnimal.Value.Types.Contains(str1) &&
            deprecatedConfig.AreVoidFarmAnimalsInShopAlways())
            stringList2.Add(str1);
          var path2 = farmAnimal.Value.AnimalShop.Icon;
          if (File.Exists(Path.Combine(ModEntry.Instance.Helper.DirectoryPath, path2)))
            stringList1.Add(path2);
          else
            path2 = null;
          farmAnimalStock = new FarmAnimalStock
          {
            Name = farmAnimal.Value.AnimalShop.Name,
            Description = farmAnimal.Value.AnimalShop.Description,
            Icon = path2,
            Exclude = stringList2
          };
          flag1 = false;
          flag2 = true;
        }

        var category1 = new Category(action);
        category1.Category = farmAnimal.Key;
        category1.Types = farmAnimal.Value.Types.Select(str => new FarmAnimalType(str, 0.0)).ToList();
        category1.Buildings = farmAnimal.Value.Buildings.ToList();
        category1.AnimalShop = farmAnimalStock;
        category1.ForceOverrideTypes = true;
        category1.ForceOverrideBuildings = true;
        category1.ForceRemoveFromShop = flag1;
        category1.ForceOverrideExclude = flag2;
        var category2 = category1;
        content1.Categories.Add(category2);
      }

      if (!content1.Categories.Any())
        return false;
      var migrationFullPath = ContentPack.ConfigMigrationFullPath;
      var id = Guid.NewGuid().ToString("N");
      new DirectoryInfo(migrationFullPath).Create();
      var path = Path.Combine(migrationFullPath, "content.json");
      var content2 = content1;
      var serializerSettings = new JsonSerializerSettings();
      serializerSettings.NullValueHandling = NullValueHandling.Ignore;
      var contents = JsonConvert.SerializeObject(content2, (Formatting) 1, serializerSettings);
      var jobject = JObject.FromObject(new
      {
        Name = ContentPack.ConfigMigrationName,
        Author = "Anonymous",
        Version = "1.0.0",
        Description = "Your custom content pack for BFAV",
        UniqueID = id,
        ContentPackFor = new
        {
          UniqueID = "Paritee.BetterFarmAnimalVariety"
        }
      });
      File.WriteAllText(Path.Combine(migrationFullPath, "manifest.json"),
        JsonConvert.SerializeObject(jobject, (Formatting) 1));
      File.WriteAllText(path, contents);
      foreach (var path2 in stringList1)
      {
        var str2 = Path.Combine(migrationFullPath, path2);
        new FileInfo(str2).Directory.Create();
        File.Copy(Path.Combine(ModEntry.Instance.Helper.DirectoryPath, path2), str2);
      }

      LoadContentPacks.SetUpContentPacks(new List<IContentPack>
      {
        mod.Helper.ContentPacks.CreateTemporary(migrationFullPath, id, ContentPack.ConfigMigrationName,
          "Your custom content pack for BFAV", "Anonymous", new SemanticVersion("1.0.0"))
      }, mod.Monitor);
      return true;
    }
  }
}