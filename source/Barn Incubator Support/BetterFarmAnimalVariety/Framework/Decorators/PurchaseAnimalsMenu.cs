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
// Type: BetterFarmAnimalVariety.Framework.Decorators.PurchaseAnimalsMenu
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Paritee.StardewValley.Core.Menus;
using StardewValley;
using StardewValley.Menus;

namespace BetterFarmAnimalVariety.Framework.Decorators
{
  internal class PurchaseAnimalsMenu : Decorator
  {
    public PurchaseAnimalsMenu(StardewValley.Menus.PurchaseAnimalsMenu original)
      : base(original)
    {
    }

    public StardewValley.Menus.PurchaseAnimalsMenu GetOriginal()
    {
      return GetOriginal<StardewValley.Menus.PurchaseAnimalsMenu>();
    }

    public void SetUpAnimalsToPurchase(
      List<Object> stock,
      Dictionary<string, Texture2D> icons,
      out int iconHeight)
    {
      PurchaseAnimals.SetUpAnimalsToPurchase(GetOriginal(), stock, icons, out iconHeight);
    }

    public List<ClickableTextureComponent> GetAnimalsToPurchase()
    {
      return PurchaseAnimals.GetAnimalsToPurchase(GetOriginal());
    }

    public void SetHeight(int height)
    {
      PurchaseAnimals.SetHeight(GetOriginal(), height);
    }

    public int GetRows()
    {
      return PurchaseAnimals.GetRows(GetOriginal());
    }

    public void AdjustHeightBasedOnIcons(int iconHeight)
    {
      PurchaseAnimals.AdjustHeightBasedOnIcons(GetOriginal(), iconHeight);
    }

    public bool IsNamingAnimal()
    {
      return PurchaseAnimals.IsNamingAnimal(GetOriginal());
    }

    public StardewValley.FarmAnimal GetAnimalBeingPurchased()
    {
      return PurchaseAnimals.GetAnimalBeingPurchased(GetOriginal());
    }

    public int GetPriceOfAnimal()
    {
      return PurchaseAnimals.GetPriceOfAnimal(GetOriginal());
    }

    public void SetAnimalBeingPurchased(StardewValley.FarmAnimal animal)
    {
      PurchaseAnimals.SetAnimalBeingPurchased(GetOriginal(), animal);
    }

    public void SetNewAnimalHome(StardewValley.Buildings.Building building)
    {
      PurchaseAnimals.SetNewAnimalHome(GetOriginal(), building);
    }

    public void SetNamingAnimal(bool namingAnimal)
    {
      PurchaseAnimals.SetNamingAnimal(GetOriginal(), namingAnimal);
    }

    public bool IsOnFarm()
    {
      return PurchaseAnimals.IsOnFarm(GetOriginal());
    }

    public void SetOnFarm(bool onFarm)
    {
      PurchaseAnimals.SetOnFarm(GetOriginal(), onFarm);
    }

    public void SetPriceOfAnimal(int price)
    {
      PurchaseAnimals.SetPriceOfAnimal(GetOriginal(), price);
    }

    public bool IsFrozen()
    {
      return PurchaseAnimals.IsFrozen(GetOriginal());
    }

    public bool HasTappedOkButton(int x, int y)
    {
      return PurchaseAnimals.HasTappedOkButton(GetOriginal(), x, y);
    }

    public bool IsReadyToClose()
    {
      return PurchaseAnimals.IsReadyToClose(GetOriginal());
    }
  }
}