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
// Type: BetterFarmAnimalVariety.Framework.Patches.PurchaseAnimalsMenu.Draw
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using Microsoft.Xna.Framework.Graphics;
using Paritee.StardewValley.Core.Utilities;
using StardewValley.BellsAndWhistles;

namespace BetterFarmAnimalVariety.Framework.Patches.PurchaseAnimalsMenu
{
  internal class Draw : Patch
  {
    public static bool Prefix(ref StardewValley.Menus.PurchaseAnimalsMenu __instance, ref SpriteBatch b)
    {
      var purchaseAnimalsMenu = new Decorators.PurchaseAnimalsMenu(__instance);
      var farmAnimal = new Decorators.FarmAnimal(purchaseAnimalsMenu.GetAnimalBeingPurchased());
      if (!BellsAndWhistles.IsFaded() && purchaseAnimalsMenu.IsOnFarm())
      {
        var str = Content.FormatMoneyString(farmAnimal.GetPrice());
        var x = Game.GetViewport().Width / 2 - Content.GetWidthOfString(str) / 2;
        var y = (int) (18.0 * SpriteText.fontPixelZoom * 2.0);
        BellsAndWhistles.DrawScroll(b, str, x, y);
      }

      return true;
    }
  }
}