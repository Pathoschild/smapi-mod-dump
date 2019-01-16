using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using PyTK.Types;
using PyTK.Extensions;
using Pathoschild.Stardew.Common;
using Microsoft.Xna.Framework;

namespace IntravenousCoffee
{
    public class IntravenousCoffeeMod : Mod
    {
        const int kCoffeeDurationMillis = (1*60 + 23) * 1000;  // 1:23 min
        const int kWithdrawalDurationMillis = 2 * 60 * 60 * 1000; // 2 hours
        const int kBuffWhich = 998;

        internal static EventHandler<EventArgsClickableMenuChanged> addtoshop;

        internal static IModHelper _helper;
        internal static IMonitor _monitor;
 
        int updateTicks = 60;

        enum AddictionState {
            Clean,
            Addicted,
            Withdrawal
        };
        AddictionState addiction = AddictionState.Clean;

        public override void Entry(IModHelper helper)
        {
            _helper = helper;
            _monitor = Monitor;

            // Add it to the Hospital shop.
            addtoshop = new InventoryItem(new IntravenousCoffeeTool(), 10000, 1).addToShop(
                (ShopMenu shop) => shop.getForSale().Exists(
                    (Item item) => item.Name == "Energy Tonic" || item.Name == "Muscle Remedy"
                )
            );

            InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
        }

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (!Context.IsWorldReady)
                return;

            this.Monitor.InterceptErrors("handling your input", $"handling input '{e.Button}'", () => {
                // HACK: This prevents the IV bag from being used as a tool. There must be a better way
                // to do this.
                if (e.IsUseToolButton
                    && Game1.player.CurrentTool as IntravenousCoffeeTool != null
                    && Game1.activeClickableMenu == null) {
                    e.SuppressButton();
                }
            });
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (updateTicks-- > 0)
                return;
            updateTicks = 60;

            removeDrinkBuff(true);

            // Wait till the buff runs out.
            if (Game1.buffsDisplay.hasBuff(kBuffWhich))
                return;

            // Find an IV bag with coffee remaining.
            IntravenousCoffeeTool ivTool = Game1.player.items.Find(
                (i) => (i is IntravenousCoffeeTool iv && iv.hasCoffee()))
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
            if (amount > 0)
                buff.glow = Color.Azure;
            else
                buff.glow = Color.Red;

            Game1.buffsDisplay.addOtherBuff(buff);
        }

        private void removeDrinkBuff(bool warn)
        {
            if (this.addiction != AddictionState.Clean && Game1.buffsDisplay.drink?.source == "Coffee") {
                Game1.buffsDisplay.drink.millisecondsDuration = 1;
                if (warn)
                    Game1.showRedMessage("Ingested coffee fails to satisfy your coffee addiction.");
            }
        }
    }
}
