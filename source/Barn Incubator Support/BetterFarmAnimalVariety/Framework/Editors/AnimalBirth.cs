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
// Type: BetterFarmAnimalVariety.Framework.Editors.AnimalBirth
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using Paritee.StardewValley.Core.Utilities;
using StardewModdingAPI;

namespace BetterFarmAnimalVariety.Framework.Editors
{
  internal class AnimalBirth : IAssetEditor
  {
    private readonly IModHelper Helper;

    public AnimalBirth(IModHelper helper)
    {
      Helper = helper;
    }

    public bool CanEdit<T>(IAssetInfo asset)
    {
      return asset.AssetNameEquals("Strings/Events") || asset.AssetNameEquals("Strings/Locations");
    }

    public void Edit<T>(IAssetData asset)
    {
      if (asset.AssetNameEquals("Strings/Events"))
      {
        var data = asset.AsDictionary<string, string>().Data;
        data[nameof(AnimalBirth)] = Helper.Translation.Get("Strings.Events.AnimalBirth");
        data["AnimalNamingTitle"] = Content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11357");
      }

      if (!asset.AssetNameEquals("Strings/Locations"))
        return;
      var data1 = asset.AsDictionary<string, string>().Data;
      data1["AnimalHouse_Incubator_Hatch_RegularEgg"] =
        Helper.Translation.Get("Strings.Locations.AnimalHouse_Incubator_Hatch");
      data1["AnimalHouse_Incubator_Hatch_VoidEgg"] =
        Helper.Translation.Get("Strings.Locations.AnimalHouse_Incubator_Hatch");
      data1["AnimalHouse_Incubator_Hatch_DuckEgg"] =
        Helper.Translation.Get("Strings.Locations.AnimalHouse_Incubator_Hatch");
      data1["AnimalHouse_Incubator_Hatch_DinosaurEgg"] =
        Helper.Translation.Get("Strings.Locations.AnimalHouse_Incubator_Hatch");
      data1["AnimalHouse_Incubator_Hatch_OstrichEgg"] =
        Helper.Translation.Get("Strings.Locations.AnimalHouse_Incubator_Hatch");
    }
  }
}