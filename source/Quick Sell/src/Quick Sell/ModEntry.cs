/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AlejandroAkbal/Stardew-Valley-Quick-Sell-Mod
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Quick_Sell
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/

        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;

        private ModLogger Logger;

        private ModUtils Utils;
        private ModPlayer Player;

        /*********
        ** Public methods
        *********/

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            this.Logger = new ModLogger(this.Monitor);

            this.Utils = new ModUtils(this.Config);
            this.Player = new ModPlayer(helper, this.Config, this.Logger, this.Utils);

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            //if (!Context.IsPlayerFree)
            //    return;

            //if (!Game1.displayHUD)
            //    return;

            if (e.Button == this.Config.SellKey)
                this.OnSellButtonPressed(sender, e);
        }

        private void OnSellButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            Item item = this.Player.GetHoveredItem();

            if (item == null)
            {
                this.Logger.Debug("Item was null.");
                return;
            }

            this.Logger.Debug($"{Game1.player.Name} pressed {e.Button} and has selected {item}.");

            if (!this.Player.CheckIfItemCanBeShipped(item))
            {
                this.Logger.Debug("Item was null or couldn't be shipped.");
                return;
            }

            this.Player.AddItemToPlayerShippingBin(item);

            this.Player.RemoveItemFromPlayerInventory(item);

            this.Utils.SendHUDMessage($"Sent {item.Stack} {item.DisplayName} to the Shipping Bin!");

            Game1.playSound("Ship");
        }
    }
}