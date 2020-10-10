/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MattDeDuck/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace BuyableDwarfScrolls
{
    public class ModEntry : Mod, IAssetEditor
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += Display_MenuChanged;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

            // Read the config file
            this.Config = this.Helper.ReadConfig<ModConfig>();
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            // Check if the menu is a shop menu
            if (e.NewMenu is ShopMenu menu)
            {
                // Grab the shop owners name
                string shopOwner = menu.portraitPerson.Name;

                // Check to see if the owner is Pierre
                if (shopOwner == "Pierre")
                {
                    // Grab the amount of hearts the player has with the Wizard
                    int wizardHearts = getWizardHearts();

                    // Grab the config values for scroll price and heart level
                    int heartLevel = this.Config.WizardHeartLevel;
                    int scrollPrice = this.Config.DwarfScrollPrice;

                    // If they have less than 5 hearts with the Wizard, continue
                    if (wizardHearts < heartLevel)
                        return;

                    // Grab the stock and items for sale in the shop
                    Dictionary<ISalable, int[]> stock = menu.itemPriceAndStock;
                    List<ISalable> initialItems = menu.forSale;

                    // Store the IDs of the Dwarf Scrolls
                    int[] scrollIDs = { 96, 97, 98, 99 };

                    // For each Dwarf scroll at it to the shop for 2500g
                    foreach (int i in scrollIDs)
                    {
                        // Create the object with the item ID and stack amount
                        Item objectToAdd = new SObject(Vector2.Zero, i, 1);
                        // Add the object to the stock with a price and amount
                        stock.Add(objectToAdd, new[] { scrollPrice, 1 });
                        initialItems.Add(objectToAdd);
                    }
                }
            }
        }

        // Get the amount of hearts the player has with the Wizard
        public int getWizardHearts()
        {
            int levels = Game1.player.getFriendshipHeartLevelForNPC("Wizard");
            return levels;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\mail");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals(@"Data\mail"))
            {
                // Get the letter data from the Mail file
                IDictionary<string, string> l = asset.AsDictionary<string, string>().Data;
                l["WizardScrollPush"] = "Dear @,^^We have become quite good friends now^^I have met with Pierre to allow him to sell the Dwarf Scrolls you seek^^I got you a little gift as well^^   -M. Rasmodius, Wizard^      %item object 422 1 %%";
                this.Monitor.Log("Added WizardScrollPush letter to the mail file", LogLevel.Info);
            }
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            // Grab the heart level you need to get with the Wizard from the config
            int heartLevel = this.Config.WizardHeartLevel;
            // Grab the current amount of hearts the player has with the Wizard
            int wizardHeartAmount = getWizardHearts();

            // If the player has reached or surpassed the heart level and not received the letter...send it
            if(wizardHeartAmount >= heartLevel && !Game1.player.hasOrWillReceiveMail("WizardScrollPush"))
            {
                // Add the letter to the mailbox
                Game1.mailbox.Add("WizardScrollPush");
            }
        }
    }
}