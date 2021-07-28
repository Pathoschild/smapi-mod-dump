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
// Type: BetterFarmAnimalVariety.Framework.Events.LoadContentPacks
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System;
using System.Collections.Generic;
using System.IO;
using BetterFarmAnimalVariety.Framework.ContentPacks;
using StardewModdingAPI;

namespace BetterFarmAnimalVariety.Framework.Events
{
  internal class LoadContentPacks
  {
    public static void SetUpContentPacks(IEnumerable<IContentPack> contentPacks, IMonitor monitor)
    {
      foreach (var contentPack in contentPacks)
      {
        monitor.Log(string.Format("Reading content pack: {0} {1}", contentPack.Manifest.Name,
          contentPack.Manifest.Version));
        try
        {
          if (!File.Exists(Path.Combine(contentPack.DirectoryPath, "content.json")))
            throw new FileNotFoundException("content.json not found.");
          contentPack.ReadJsonFile<Content>("content.json").SetUp(contentPack);
        }
        catch (Exception ex)
        {
          monitor.Log(contentPack.Manifest.Name + " will not load: " + ex.Message, LogLevel.Warn);
        }
      }
    }
  }
}