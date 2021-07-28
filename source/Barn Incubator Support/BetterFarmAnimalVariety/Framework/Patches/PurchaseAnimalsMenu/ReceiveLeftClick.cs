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
// Type: BetterFarmAnimalVariety.Framework.Patches.PurchaseAnimalsMenu.ReceiveLeftClick
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System.Linq;
using BetterFarmAnimalVariety.Framework.Decorators;
using Microsoft.Xna.Framework;
using Paritee.StardewValley.Core.Utilities;
using Game = Paritee.StardewValley.Core.Utilities.Game;
using Mod = BetterFarmAnimalVariety.Framework.Helpers.Mod;

namespace BetterFarmAnimalVariety.Framework.Patches.PurchaseAnimalsMenu
{
  internal class ReceiveLeftClick
  {
    public static bool Prefix(
      ref StardewValley.Menus.PurchaseAnimalsMenu __instance,
      ref int x,
      ref int y,
      ref bool playSound)
    {
      var moddedMenu = new Decorators.PurchaseAnimalsMenu(__instance);
      if (!IsActionable(moddedMenu) || IsClosingMenu(moddedMenu, x, y))
        return true;
      var moddedPlayer = new Farmer(Game.GetPlayer());
      return moddedMenu.IsOnFarm()
        ? HandleOnFarm(ref moddedMenu, x, y, moddedPlayer)
        : HandleStockSelection(ref moddedMenu, x, y, moddedPlayer);
    }

    private static bool IsActionable(Decorators.PurchaseAnimalsMenu moddedMenu)
    {
      return !BellsAndWhistles.IsFaded() && !moddedMenu.IsFrozen();
    }

    private static bool IsClosingMenu(Decorators.PurchaseAnimalsMenu moddedMenu, int x, int y)
    {
      return moddedMenu.HasTappedOkButton(x, y) && moddedMenu.IsReadyToClose();
    }

    private static bool HandleStockSelection(
      ref Decorators.PurchaseAnimalsMenu moddedMenu,
      int x,
      int y,
      Farmer moddedPlayer)
    {
      var textureComponent = moddedMenu.GetAnimalsToPurchase().Where(o => (o.item as StardewValley.Object).Type == null)
        .FirstOrDefault(o => o.containsPoint(x, y));
      if (textureComponent == null)
        return true;
      var amount = textureComponent.item.salePrice();
      if (!moddedPlayer.CanAfford(amount))
        return true;
      var randomAnimalShopType =
        new Api.BetterFarmAnimalVariety(Mod.ReadConfig<ModConfig>()).GetRandomAnimalShopType(textureComponent.hoverText,
          moddedPlayer.GetOriginal());
      var farmAnimal1 = moddedPlayer.CreateFarmAnimal(randomAnimalShopType);
      SelectedStockBellsAndWhistles(ref moddedMenu);
      moddedMenu.SetOnFarm(true);
      moddedMenu.SetAnimalBeingPurchased(farmAnimal1);
      var farmAnimal2 = new Decorators.FarmAnimal(farmAnimal1);
      moddedMenu.SetPriceOfAnimal(farmAnimal2.GetPrice());
      return false;
    }

    private static void SelectedStockBellsAndWhistles(ref Decorators.PurchaseAnimalsMenu moddedMenu)
    {
      BellsAndWhistles.FadeToBlack(afterFade: moddedMenu.GetOriginal().setUpForAnimalPlacement);
      BellsAndWhistles.PlaySound("smallSelect");
    }

    private static bool HandleOnFarm(
      ref Decorators.PurchaseAnimalsMenu moddedMenu,
      int x,
      int y,
      Farmer moddedPlayer)
    {
      if (moddedMenu.IsNamingAnimal())
        return true;
      var viewport = Game.GetViewport();
      var buildingAt = Game.GetFarm().getBuildingAt(new Vector2((x + viewport.X) / 64, (y + viewport.Y) / 64));
      if (buildingAt == null)
        return true;
      var animalBeingPurchased = moddedMenu.GetAnimalBeingPurchased();
      var moddedAnimal = new Decorators.FarmAnimal(animalBeingPurchased);
      if (!moddedAnimal.CanLiveIn(buildingAt) || new Building(buildingAt).IsFull() || !moddedAnimal.CanBeNamed())
        return true;
      var priceOfAnimal = moddedMenu.GetPriceOfAnimal();
      if (!moddedPlayer.CanAfford(priceOfAnimal))
        return true;
      moddedAnimal.AddToBuilding(buildingAt);
      moddedMenu.SetAnimalBeingPurchased(animalBeingPurchased);
      moddedMenu.SetNewAnimalHome(null);
      moddedMenu.SetNamingAnimal(false);
      moddedPlayer.SpendMoney(priceOfAnimal);
      PurchasedAnimalBellsAndWhistles(moddedAnimal);
      return false;
    }

    private static void PurchasedAnimalBellsAndWhistles(Decorators.FarmAnimal moddedAnimal)
    {
      if (moddedAnimal.MakesSound())
        BellsAndWhistles.CueSound(moddedAnimal.GetSound(), "Pitch", 1200 + Random.Next(-200, 201));
      BellsAndWhistles.AddHudMessage(
        Content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11324", moddedAnimal.GetDisplayType()),
        Color.LimeGreen, 3500f);
    }
  }
}