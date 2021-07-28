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
// Type: BetterFarmAnimalVariety.Framework.Patches.PurchaseAnimalsMenu.GetAnimalTitle
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using BetterFarmAnimalVariety.Framework.Helpers;

namespace BetterFarmAnimalVariety.Framework.Patches.PurchaseAnimalsMenu
{
  internal class GetAnimalTitle : Patch
  {
    public static bool Prefix(ref string name, ref string __result)
    {
      var category = name;
      __result = FarmAnimals.GetCategory(category).AnimalShop.Name;
      return false;
    }
  }
}