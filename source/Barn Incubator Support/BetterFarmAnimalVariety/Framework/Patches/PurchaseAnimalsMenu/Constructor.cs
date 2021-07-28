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
// Type: BetterFarmAnimalVariety.Framework.Patches.PurchaseAnimalsMenu.Constructor
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using System.Linq;
using BetterFarmAnimalVariety.Framework.Helpers;

namespace BetterFarmAnimalVariety.Framework.Patches.PurchaseAnimalsMenu
{
  internal class Constructor : Patch
  {
    public static void Prefix(ref StardewValley.Menus.PurchaseAnimalsMenu __instance,
      ref List<StardewValley.Object> stock)
    {
      StardewValley.Menus.PurchaseAnimalsMenu.menuWidth = 640;
      __instance.width = 720;
    }

    public static void Postfix(ref StardewValley.Menus.PurchaseAnimalsMenu __instance,
      ref List<StardewValley.Object> stock)
    {
      var moddedMenu = new Decorators.PurchaseAnimalsMenu(__instance);
      var dictionary = FarmAnimals.GetCategories().Where(o => o.CanBePurchased())
        .ToDictionary(o => o.Category, o => o.GetAnimalShopIconTexture());
      int iconHeight;
      moddedMenu.SetUpAnimalsToPurchase(stock, dictionary, out iconHeight);
      AdjustMenuHeight(ref moddedMenu, iconHeight);
      StardewValley.Menus.PurchaseAnimalsMenu.menuWidth = 640;
    }

    private static void AdjustMenuHeight(ref Decorators.PurchaseAnimalsMenu moddedMenu, int iconHeight)
    {
      if (iconHeight <= 0)
        return;
      moddedMenu.AdjustHeightBasedOnIcons(iconHeight);
    }
  }
}