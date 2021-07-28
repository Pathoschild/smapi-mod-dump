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
// Type: BetterFarmAnimalVariety.Framework.Events.IntegrateWithJsonAssets
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using BetterFarmAnimalVariety.Framework.Api;
using BetterFarmAnimalVariety.Framework.Helpers;
using Paritee.StardewValley.Core.Objects;
using Paritee.StardewValley.Core.Utilities;
using StardewModdingAPI;

namespace BetterFarmAnimalVariety.Framework.Events
{
  internal class IntegrateWithJsonAssets
  {
    private static void AssertApiIsReady(IModHelper helper, out IJsonAssets api)
    {
      Assert.ApiExists(helper, "spacechase0.JsonAssets", out api);
      Assert.SaveLoaded();
    }

    public static void RefreshFarmAnimalData(IModHelper helper)
    {
      helper.Content.InvalidateCache(Content.DataFarmAnimalsContentPath);
      helper.Content.Load<Dictionary<string, string>>(Content.DataFarmAnimalsContentPath, ContentSource.GameContent);
    }

    public static bool TryParseObjectName(string objectName, out int index)
    {
      return Object.TryParse(objectName, out index);
    }
  }
}