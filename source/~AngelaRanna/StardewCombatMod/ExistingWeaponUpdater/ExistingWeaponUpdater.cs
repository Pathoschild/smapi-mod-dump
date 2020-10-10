using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace ExistingWeaponUpdater
{
    public class ExistingWeaponUpdater : Mod
    {
        private ModConfig Config;
        bool iMadeThis = false;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.SaveLoaded += this.saveLoaded;
            helper.Events.Player.InventoryChanged += this.inventoryChanged;
            helper.Events.GameLoop.DayStarted += this.dayStarted;
        }

        private void dayStarted(object sender, DayStartedEventArgs e)
        {
            // Update the switches from the config
            this.Config = this.Helper.ReadConfig<ModConfig>();
        }

        private void saveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // If updating on save load is disabled, return
            if (!Config.updateOnSaveLoad) return;

            // Loop through the player's inventory and update each weapon
            for (int i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.items[i] != null && Game1.player.items[i] is MeleeWeapon)
                {
                    Game1.player.items[i] = (Item)new MeleeWeapon(((MeleeWeapon)Game1.player.items[i]).InitialParentTileIndex);
                    iMadeThis = true;
                }
            }
        }

        private void inventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            // If updating on inventory add is disabled, return
            if (!Config.updateOnItemAddedToInventory) return;

            // Loop through any added items and update each weapon
            if (!iMadeThis)
            {
                foreach (Item i in e.Added)
                {
                    if (i != null && i is MeleeWeapon)
                    {
                        int weaponIndex = ((MeleeWeapon)i).InitialParentTileIndex;
                        Game1.player.removeItemFromInventory(i);
                        Game1.player.addItemToInventory(new MeleeWeapon(weaponIndex));
                    }
                }
                iMadeThis = true;
            }
            else iMadeThis = false;
        }
    }

    public class ModConfig
    {
        public bool updateOnSaveLoad { get; set; } = true;
        public bool updateOnItemAddedToInventory { get; set; } = true;
    }
}