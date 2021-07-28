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
// Type: BetterFarmAnimalVariety.ModConfig
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using BetterFarmAnimalVariety.Framework.Helpers;
using StardewModdingAPI;

namespace BetterFarmAnimalVariety
{
  public class ModConfig
  {
    public string Format;
    public bool IsEnabled;

    public ModConfig()
    {
      Format = null;
      IsEnabled = true;
    }

    public void Write(IModHelper helper)
    {
      helper.WriteConfig(this);
    }

    public void AssertValidFormat(string targetFormat)
    {
      Assert.VersionIsSupported(Format, targetFormat);
    }
  }
}