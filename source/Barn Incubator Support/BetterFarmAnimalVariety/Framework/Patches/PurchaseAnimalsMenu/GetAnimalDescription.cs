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
// Type: BetterFarmAnimalVariety.Framework.Patches.PurchaseAnimalsMenu.GetAnimalDescription
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using BetterFarmAnimalVariety.Framework.Decorators;
using BetterFarmAnimalVariety.Framework.Helpers;
using Paritee.StardewValley.Core.Utilities;

namespace BetterFarmAnimalVariety.Framework.Patches.PurchaseAnimalsMenu
{
  internal class GetAnimalDescription : Patch
  {
    public static bool Prefix(ref string name, ref string __result)
    {
      var str = name;
      __result = FarmAnimals.GetCategory(str).AnimalShop.Description;
      var farmer = new Farmer(Game.GetPlayer());
      var types1 = FarmAnimals.GroupPurchaseableTypesByCategory()[str];
      var types2 = farmer.SanitizeBlueChickens(types1);
      var count1 = types2.Count;
      var count2 = farmer.SanitizeAffordableTypes(types2).Count;
      if (count2 > 0 && count2 < count1)
      {
        var strArray = new string[3]
        {
          count2.ToString(),
          count1.ToString(),
          "$"
        };
        __result = __result + " (" +
                   Content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat", strArray) + ")";
      }

      return false;
    }
  }
}