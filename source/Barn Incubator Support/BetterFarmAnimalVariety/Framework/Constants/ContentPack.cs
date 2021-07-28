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
// Type: BetterFarmAnimalVariety.Framework.Constants.ContentPack
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.IO;

namespace BetterFarmAnimalVariety.Framework.Constants
{
  internal class ContentPack
  {
    public const string ContentFileName = "content.json";
    public const string ManifestFileName = "manifest.json";
    public const string ConfigMigrationPrefix = "[BFAV]";
    public const string ConfigMigrationAuthor = "Anonymous";
    public const string ConfigMigrationVersion = "1.0.0";
    public const string ConfigMigrationDescription = "Your custom content pack for BFAV";

    public static string ConfigMigrationName => "[BFAV] My Content Pack";

    public static string ConfigMigrationFullPath =>
      Path.Combine(Directory.GetParent(ModEntry.Instance.Helper.DirectoryPath).FullName, ConfigMigrationName);
  }
}