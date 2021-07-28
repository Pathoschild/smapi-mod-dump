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
// Type: BetterFarmAnimalVariety.Framework.Helpers.Mod
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewModdingAPI;

namespace BetterFarmAnimalVariety.Framework.Helpers
{
  internal class Mod
  {
    public static T ReadSaveData<T>(string saveDataKey) where T : new()
    {
      return Paritee.StardewValley.Core.Utilities.Mod.ReadSaveData<T>("Paritee.BetterFarmAnimalVariety", saveDataKey);
    }

    public static void WriteSaveData<T>(string saveDataKey, T data)
    {
      Paritee.StardewValley.Core.Utilities.Mod.WriteSaveData("Paritee.BetterFarmAnimalVariety", saveDataKey, data);
    }

    public static T ReadConfig<T>() where T : new()
    {
      return Paritee.StardewValley.Core.Utilities.Mod.ReadConfig<T>(ModEntry.Instance.Helper.DirectoryPath,
        "config.json");
    }

    public static void WriteCache<T>(string cacheFilePath, T data)
    {
      var contents = JsonConvert.SerializeObject(data, (Formatting) 1);
      var str = Path.Combine(Constants.Mod.CacheFullPath, cacheFilePath);
      new FileInfo(str).Directory.Create();
      File.WriteAllText(str, contents);
    }

    public static T ReadCache<T>(string cacheFilePath) where T : new()
    {
      var path = Path.Combine(Constants.Mod.CacheFullPath, cacheFilePath);
      return !File.Exists(path) ? new T() : JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
    }

    public static Texture2D LoadTexture(string filePath)
    {
      return Paritee.StardewValley.Core.Utilities.Mod.LoadTexture(Path.Combine(ModEntry.Instance.Helper.DirectoryPath,
        filePath));
    }

    public static string GetShortAssetPath(string filePath)
    {
      return Path.Combine("assets", filePath);
    }

    public static bool TryGetApi<TInterface>(IModHelper helper, string key, out TInterface api) where TInterface : class
    {
      api = helper.ModRegistry.GetApi<TInterface>(key);
      return api != null;
    }
  }
}