using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using StardewValley.Menus;
using StardewValley.Tools;
using System;

namespace MoreWeapons
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
        }

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.activeClickableMenu is ShopMenu)
            {
                ShopMenu shop = (ShopMenu)Game1.activeClickableMenu;
                int mineLevelReached = Game1.mine?.lowestLevelReached ?? 0;

                if (shop.portraitPerson is NPC && shop.portraitPerson.name == "Marlon")
                {


                    Dictionary<Item, int> newItemsToSell = new Dictionary<Item, int>();

                    if (Game1.player.mailReceived.Contains("galaxySword"))
                    {
                        MeleeWeapon Waffe53 = new MeleeWeapon(53); //Galaxy Mace
                        newItemsToSell.Add(Waffe53, 15000);

                    }
                    if (mineLevelReached > 15)
                    {
                        MeleeWeapon Waffe55 = new MeleeWeapon(55); //Short Staff
                        newItemsToSell.Add(Waffe55, 2500);

                    }
                    if (mineLevelReached > 85)
                    {
                        MeleeWeapon Waffe56 = new MeleeWeapon(56); //Rogue Sword
                        newItemsToSell.Add(Waffe56, 7500);

                    }
                    if (mineLevelReached > 120)
                    {
                        MeleeWeapon Waffe58 = new MeleeWeapon(58); //Hero's Sword
                        newItemsToSell.Add(Waffe58, 450000);

                    }
                    if (mineLevelReached > 80)
                    {
                        MeleeWeapon Waffe59 = new MeleeWeapon(59); //Double edged Axe
                        newItemsToSell.Add(Waffe59, 7500);

                    }
                    if (mineLevelReached > 60)
                    {
                        MeleeWeapon Waffe64 = new MeleeWeapon(64); //Battle Axe
                        newItemsToSell.Add(Waffe64, 6500);
                   
                    }
                    if (mineLevelReached > 25)
                    {
                        MeleeWeapon Waffe60 = new MeleeWeapon(60); //Basic Mace
                        newItemsToSell.Add(Waffe60, 3000);

                    }
                    if (mineLevelReached > 20)
                    {
                        MeleeWeapon Waffe62 = new MeleeWeapon(62); //Walking Stick
                        newItemsToSell.Add(Waffe62, 2500);

                    }
                    if (Game1.player.mailReceived.Contains("galaxySword"))
                    {
                        MeleeWeapon Waffe65 = new MeleeWeapon(65); //Galaxy Axe
                        newItemsToSell.Add(Waffe65, 15000);
                
                    }
                    if (mineLevelReached > 120)
                    {
                        MeleeWeapon Waffe63 = new MeleeWeapon(63); //Ice Sword
                        newItemsToSell.Add(Waffe63, 10000);

                    }
                    if (mineLevelReached > 5)
                    {
                        MeleeWeapon Waffe67 = new MeleeWeapon(66); //Forest Sword
                        newItemsToSell.Add(Waffe67, 750);
                    }
                    if (mineLevelReached > 90)
                    {
                        MeleeWeapon Waffe54 = new MeleeWeapon(54); //Steel Rapier
                        newItemsToSell.Add(Waffe54, 7000);
                    }
                    if (mineLevelReached > 10)
                    {
                        MeleeWeapon Waffe67 = new MeleeWeapon(67); //Old Dagger
                        newItemsToSell.Add(Waffe67, 200);
                    }
                    if (mineLevelReached > 80)
                    {
                        MeleeWeapon Waffe57 = new MeleeWeapon(57); //The blade of Wrath
                        newItemsToSell.Add(Waffe57, 10000);
                    }
                    if (mineLevelReached > 70)
                    {
                        MeleeWeapon Waffe68 = new MeleeWeapon(68); //The Death Reaper
                        newItemsToSell.Add(Waffe68, 5000);
                    }
                    if (mineLevelReached > 10)
                    {
                        MeleeWeapon Waffe69 = new MeleeWeapon(69); // Dark Katana
                        newItemsToSell.Add(Waffe69, 1500);
                    }
                    if (mineLevelReached > 50)
                    {
                        MeleeWeapon Waffe70 = new MeleeWeapon(70); // Mysterious Katana
                        newItemsToSell.Add(Waffe70, 5000);
                    }
                    if (mineLevelReached > 100)
                    {
                        MeleeWeapon Waffe71 = new MeleeWeapon(71); // Viking King Blade
                        newItemsToSell.Add(Waffe71, 10000);
                    }





                    Dictionary<Item, int[]> items = Helper.Reflection.GetPrivateValue<Dictionary<Item, int[]>>(shop, "itemPriceAndStock");
                    List<Item> selling = Helper.Reflection.GetPrivateValue<List<Item>>(shop, "forSale");

                    foreach (Item item in newItemsToSell.Keys)
                    {
                        items.Add(item, new int[] { newItemsToSell[item], int.MaxValue });
                        selling.Add(item);
                    }

                }

            }


        }
    }
}
