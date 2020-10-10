/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mpcomplete/StardewMods
**
*************************************************/

using System;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using PyTK.Types;
using PyTK.Extensions;
using PyTK.CustomElementHandler;
using Pathoschild.Stardew.Common;
using Microsoft.Xna.Framework;

namespace IntravenousCoffee {
  public class IntravenousCoffeeMod : Mod {
    const int kCoffeeDurationMillis = (1 * 60 + 23) * 1000;  // 1:23 min
    const int kWithdrawalDurationMillis = 2 * 60 * 60 * 1000; // 2 hours
    const int kBuffWhich = 998;

    //internal static EventHandler<MenuChangedEventArgs> addtoshop;

    internal static IModHelper _helper;
    internal static IMonitor _monitor;

    enum AddictionState {
      Clean,
      Addicted,
      Withdrawal
    };
    AddictionState addiction = AddictionState.Clean;

    public override void Entry(IModHelper helper) {
      _helper = helper;
      _monitor = Monitor;

      Helper.Events.Input.ButtonPressed += this.InputEvents_ButtonPressed;
      Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
      Helper.Events.GameLoop.OneSecondUpdateTicked += this.GameEvents_UpdateTick;
      Helper.Events.GameLoop.DayStarted += delegate { addiction = AddictionState.Clean; };
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
      IntravenousCoffeeTool tool = new IntravenousCoffeeTool();
      CustomObjectData.newObject("mpcomplete.IntravenousCoffee.Tool", IntravenousCoffeeTool.texture, Color.White, tool.Name, tool.description, 0, customType: typeof(IntravenousCoffeeTool));

      // Add it to the Hospital shop.
      new InventoryItem(tool, 10000, 1).addToShop(
          (ShopMenu shop) => true
          //(ShopMenu shop) => shop.getForSale().Exists(
          //    (ISalable item) => item.Name == "Energy Tonic" || item.Name == "Muscle Remedy"
          //)
      );
    }

    private void InputEvents_ButtonPressed(object sender, ButtonPressedEventArgs e) {
      if (!Context.IsWorldReady)
        return;

      this.Monitor.InterceptErrors("handling your input", $"handling input '{e.Button}'", () => {
        // HACK: This prevents the IV bag from being used as a tool. There must be a better way
        // to do this.
        if (e.Button.IsUseToolButton()
            && Game1.player.CurrentTool as IntravenousCoffeeTool != null
            && Game1.activeClickableMenu == null) {
          Helper.Input.Suppress(e.Button);
        }
      });
    }

    private void GameEvents_UpdateTick(object sender, EventArgs e) {
      if (!Context.IsWorldReady)
        return;

      removeDrinkBuff(true);

      // Wait till the buff runs out.
      if (Game1.buffsDisplay.hasBuff(kBuffWhich))
        return;

      // Find an IV bag with coffee remaining.
      IntravenousCoffeeTool ivTool = Game1.player.Items.Where(
          (i) => (i is IntravenousCoffeeTool iv && iv.hasCoffee())).FirstOrDefault()
          as IntravenousCoffeeTool;

      if (ivTool != null) {
        // Consume some coffee.
        ivTool.consumeCoffee();
        addBuff(1, kCoffeeDurationMillis, "+1 Speed", "Coffee Drip");
        this.addiction = AddictionState.Addicted;  // caffeine's a hell of a drug
        removeDrinkBuff(false);
      } else if (this.addiction == AddictionState.Addicted) {
        // Ran out of coffee. Go into withdrawal.
        addBuff(-1, kWithdrawalDurationMillis, "-1 Speed", "Coffee Withdrawal");
        this.addiction = AddictionState.Withdrawal;
      } else if (this.addiction == AddictionState.Withdrawal) {
        // Withdrawal buff wore off (also happens when player sleeps). We made it.
        this.addiction = AddictionState.Clean;
      }
    }

    private void addBuff(int amount, int millisecondsDuration, string description, string source) {
      if (Game1.buffsDisplay.hasBuff(kBuffWhich))
        return;

      Buff buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, amount, 0, 0, 2, source, source);
      buff.description = description;
      buff.millisecondsDuration = millisecondsDuration;
      buff.which = kBuffWhich;
      buff.sheetIndex = 9;
      if (amount <= 0)
        buff.glow = Color.Red;

      Game1.buffsDisplay.addOtherBuff(buff);
    }

    private void removeDrinkBuff(bool warn) {
      if (this.addiction != AddictionState.Clean && Game1.buffsDisplay.drink?.source == "Coffee") {
        Game1.buffsDisplay.drink.millisecondsDuration = 1;
        if (warn)
          Game1.showRedMessage("Ingested coffee fails to satisfy your coffee addiction.");
      }
    }
  }
}
