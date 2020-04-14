using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Pathoschild.Stardew.Common;
using mpcomplete.Stardew.QuickCraft.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;

namespace mpcomplete.Stardew.QuickCraft
{
  internal class QuickCraftMod : Mod
  {
    private ModConfig Config;

    public override void Entry(IModHelper helper) {
      this.Config = helper.ReadConfig<ModConfig>();

      Helper.Events.Input.ButtonPressed += this.InputEvents_ButtonPressed;
    }

    private void InputEvents_ButtonPressed(object sender, ButtonPressedEventArgs e) {
      if (!Context.IsWorldReady)
        return;

      this.Monitor.InterceptErrors("handling your input", $"handling input '{e.Button}'", () => {
        if (e.Button.IsUseToolButton() && IsEnabled()) {
          switch (Game1.activeClickableMenu) {
            case GameMenu menu: {
                List<IClickableMenu> pages = this.Helper.Reflection.GetField<List<IClickableMenu>>(menu, "pages").GetValue();
                CraftingPage craftingPage = pages[menu.currentTab] as CraftingPage;
                if (craftingPage == null)
                  return;

                CraftingRecipe recipe = this.Helper.Reflection.GetField<CraftingRecipe>(craftingPage, "hoverRecipe").GetValue();
                if (recipe == null)
                  return;

                int repeat = this.Is5x() ? 5 : 1;
                bool didCraft = false;
                while (repeat-- > 0 && recipe.doesFarmerHaveIngredientsInInventory()) {
                  Item item = recipe.createItem();
                  if (!Game1.player.couldInventoryAcceptThisItem(item))
                    break;
                  recipe.consumeIngredients(new List<StardewValley.Objects.Chest>());
                  Game1.player.addItemToInventory(item);
                  didCraft = true;
                }

                if (didCraft)
                  Game1.playSound("Ship");

                Helper.Input.Suppress(e.Button);
                break;
              }
            case ShopMenu menu: {
                int repeat = this.Is5x() ? 500 : 100;
                BuyHoveredItem(menu, repeat, Game1.getMousePosition());
                Helper.Input.Suppress(e.Button);
                break;
              }
          }
        }
      });
    }

    public void BuyHoveredItem(ShopMenu menu, int amount, Point clickLocation) {
      Item hoverItem = this.Helper.Reflection.GetField<Item>(menu, "hoveredItem").GetValue();
      if (hoverItem == null)
        return;

      var heldItem = this.Helper.Reflection.GetField<Item>(menu, "heldItem").GetValue();
      var priceAndStockField = this.Helper.Reflection.GetField<Dictionary<Item, int[]>>(menu, "itemPriceAndStock");
      var priceAndStockMap = priceAndStockField.GetValue();
      //Debug.Assert(priceAndStockMap.ContainsKey(hoverItem));

      // Calculate the number to purchase
      int numInStock = priceAndStockMap[hoverItem][1];
      int itemPrice = priceAndStockMap[hoverItem][0];
      int ShopCurrencyType = this.Helper.Reflection.GetField<int>(menu, "currency").GetValue();
      int currentMonies = ShopMenu.getPlayerCurrencyAmount(Game1.player, ShopCurrencyType);
      amount = Math.Min(Math.Min(amount, currentMonies / itemPrice), Math.Min(numInStock, hoverItem.maximumStackSize()));

      // If we couldn't grab all that we wanted then only subtract the amount we were able to grab
      int numHeld = heldItem?.Stack ?? 0;
      int overflow = Math.Max((numHeld + amount) - hoverItem.maximumStackSize(), 0);
      amount -= overflow;

      this.Monitor.Log($"Attempting to purchase {amount} of {hoverItem.Name} for {itemPrice * amount}");

      if (amount <= 0)
        return;

      // Try to purchase the item - method returns true if it should be removed from the shop since there's no more.
      var purchaseMethodInfo = this.Helper.Reflection.GetMethod(menu, "tryToPurchaseItem");
      int index = this.GetClickedItemIndex(this.Helper.Reflection, menu, clickLocation);
      if (purchaseMethodInfo.Invoke<bool>(hoverItem, heldItem, amount, clickLocation.X, clickLocation.Y, index)) {
        this.Monitor.Log($"Purchase of {hoverItem.Name} successful");

        // remove the purchased item from the stock etc.
        priceAndStockMap.Remove(hoverItem);
        priceAndStockField.SetValue(priceAndStockMap);

        var itemsForSaleField = this.Helper.Reflection.GetField<List<Item>>(menu, "forSale");
        var itemsForSale = itemsForSaleField.GetValue();
        itemsForSale.Remove(hoverItem);
        itemsForSaleField.SetValue(itemsForSale);
      }
    }

    public int GetClickedItemIndex(IReflectionHelper reflection, ShopMenu shopMenu, Point p) {
      int currentItemIndex = reflection.GetField<int>(shopMenu, "currentItemIndex").GetValue();
      int saleButtonIndex = shopMenu.forSaleButtons.FindIndex(button => button.containsPoint(p.X, p.Y));
      return saleButtonIndex > -1 ? currentItemIndex + saleButtonIndex : -1;
    }

    private bool IsEnabled() {
      KeyboardState state = Keyboard.GetState();
      return this.Config.Controls.HoldToActivate.Any(button => button.TryGetKeyboard(out Keys key) && state.IsKeyDown(key));
    }

    private bool Is5x() {
      KeyboardState state = Keyboard.GetState();
      return this.Config.Controls.HoldFor5x.Any(button => button.TryGetKeyboard(out Keys key) && state.IsKeyDown(key));
    }
  }
}
