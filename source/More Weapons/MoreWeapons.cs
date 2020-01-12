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
    public class MoreWeapons : Mod, IAssetEditor, IAssetLoader
    {

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("TileSheets/Weapons"))
            {
                return true;
            }

            return false;
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("TileSheets/Weapons"))
            {
                return this.Helper.Content.Load<T>("assets/Images/weapons.png", ContentSource.ModFolder);
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }







        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/Weapons"))
            {
                return true;
            }

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/Weapons"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

                data[53] = "Double Edged Axe/A doubled sided war axe. It's really heavy!/65/80/1/8/0/0/2/0/0/0/.01/1.5";
                data[54] = "Steel Rapier/A slender, sharply pointed sword that originated from Spain./50/60/1/5/0/0/3/0/0/0/.02/1.4";
                data[55] = "Short Staff/A straight, short wooden staff, common in martial arts such as karate./20/30/1/8/0/0/3/0/0/-1/.01/1.4";
                data[56] = "Rogue Sword/An old sword once used by rogue bandits./90/100/1/7/0/0/3/0/0/-1/.01/1.2";
                data[57] = "The Blade of Wrath/An ancient blade of great legend, with a blade of flowing lava and a haft of burning coals./65/75/1/0/0/0/3/0/0/0/.02/2";
                data[58] = "Hero's Sword/This sword was once used by famed heroes Joco80 and Sasara to rid the valley of evil./250/300/1/0/5/0/3/0/0/0/.3/3";
                data[59] = "Double Edged Axe/A doubled sided war axe. It's really heavy!/65/80/1/8/0/0/2/0/0/0/.01/1.5";
                data[60] = "Old Mace/An old weapon composed of a hardy stick and a heavy spike ball./35/45/1/8/0/0/2/0/0/0/.01/1";
                data[62] = "Walking Stick/A long, heavy walking stick. Even has a cool curve at the top for a real rustic feel./20/30/1/1/0/0/3/-1/-1/-1/.01/1.2";
                data[63] = "Ice Sword/A freezing sword created from ice crystals./150/175/1/4/0/0/0/-1/-1/-1/.01/1.2";
                data[64] = "Battle Axe/An axe specifically designed for comba,a specialized version of utility axes./50/75/1/-6/0/0/2/0/0/0/.02/1.7";
                data[65] = "Galaxy Axe/A devastating combat weapon, forged of unknown and rare materials./150/175/1/0/0/0/2/-1/-1/-1/.01/3";
                data[66] = "Emerald Sword/A tough sword forged in the mines and infused with shards of emeralds.../10/15/1/1/0/0/3/0/0/0/.02/2";
                data[67] = "Old Dagger/A large dagger, made for quick jabs and thrusts./5/10/1/0/0/0/1/0/0/0/.02/1.8";
                data[68] = "The Death Reaper/A gruesome weapon of surprisingly light material and a sharp edge./60/65/1/3/0/0/3/0/0/0/.02/2.4";
                data[69] = "Dark Katana/A traditional blade, once brandished by Japanese samurai's./10/20/1/2/0/0/3/0/0/0/.02/1.4";
                data[70] = "Mysterious Katana/A mysterious katana, what can it do, if anything?/5/60/1/0/0/0/3/0/0/0/.01/3.5";
                data[71] = "Viking King Blade/A sword onced used by the King of Vikings, very strong and powerful, for their conquest of the North/85/95/1/0/0/0/3/0/0/0/.03/2";
            }
        }


        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += DisplayMenuChanged;
        }

        private void DisplayMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.activeClickableMenu is ShopMenu)
            {
                ShopMenu shop = (ShopMenu)Game1.activeClickableMenu;

                if (shop.portraitPerson is NPC && shop.portraitPerson.Name == "Marlon")
                {


                    Dictionary<ISalable, int> newItemsToSell = new Dictionary<ISalable, int>();
                    int ReachedMineLevel = Game1.player.deepestMineLevel;

                    if (Game1.player.mailReceived.Contains("galaxySword"))
                    {
                        MeleeWeapon Waffe53 = new MeleeWeapon(53); //Galaxy Mace
                        newItemsToSell.Add(Waffe53, 15000);

                    }
                    if (ReachedMineLevel < 15)
                    {
                        MeleeWeapon Waffe55 = new MeleeWeapon(55); //Short Staff
                        newItemsToSell.Add(Waffe55, 2500);

                    }
                    if (ReachedMineLevel > 85)
                    {
                        MeleeWeapon Waffe56 = new MeleeWeapon(56); //Rogue Sword
                        newItemsToSell.Add(Waffe56, 7500);

                    }
                    if (ReachedMineLevel > 120)
                    {
                        MeleeWeapon Waffe58 = new MeleeWeapon(58); //Hero's Sword
                        newItemsToSell.Add(Waffe58, 450000);

                    }
                    if (ReachedMineLevel > 80)
                    {
                        MeleeWeapon Waffe59 = new MeleeWeapon(59); //Double edged Axe
                        newItemsToSell.Add(Waffe59, 7500);

                    }
                    if (ReachedMineLevel > 60)
                    {
                        MeleeWeapon Waffe64 = new MeleeWeapon(64); //Battle Axe
                        newItemsToSell.Add(Waffe64, 6500);
                   
                    }
                    if (ReachedMineLevel > 25)
                    {
                        MeleeWeapon Waffe60 = new MeleeWeapon(60); //Basic Mace
                        newItemsToSell.Add(Waffe60, 3000);

                    }
                    if (ReachedMineLevel > 20)
                    {
                        MeleeWeapon Waffe62 = new MeleeWeapon(62); //Walking Stick
                        newItemsToSell.Add(Waffe62, 2500);

                    }
                    if (Game1.player.mailReceived.Contains("galaxySword"))
                    {
                        MeleeWeapon Waffe65 = new MeleeWeapon(65); //Galaxy Axe
                        newItemsToSell.Add(Waffe65, 15000);
                
                    }
                    if (ReachedMineLevel > 120)
                    {
                        MeleeWeapon Waffe63 = new MeleeWeapon(63); //Ice Sword
                        newItemsToSell.Add(Waffe63, 10000);

                    }
                    if (ReachedMineLevel > 5)
                    {
                        MeleeWeapon Waffe67 = new MeleeWeapon(66); //Forest Sword
                        newItemsToSell.Add(Waffe67, 750);
                    }
                    if (ReachedMineLevel > 90)
                    {
                        MeleeWeapon Waffe54 = new MeleeWeapon(54); //Steel Rapier
                        newItemsToSell.Add(Waffe54, 7000);
                    }
                    if (ReachedMineLevel > 10)
                    {
                        MeleeWeapon Waffe67 = new MeleeWeapon(67); //Old Dagger
                        newItemsToSell.Add(Waffe67, 200);
                    }
                    if (ReachedMineLevel > 80)
                    {
                        MeleeWeapon Waffe57 = new MeleeWeapon(57); //The blade of Wrath
                        newItemsToSell.Add(Waffe57, 10000);
                    }
                    if (ReachedMineLevel > 70)
                    {
                        MeleeWeapon Waffe68 = new MeleeWeapon(68); //The Death Reaper
                        newItemsToSell.Add(Waffe68, 5000);
                    }
                    if (ReachedMineLevel > 10)
                    {
                        MeleeWeapon Waffe69 = new MeleeWeapon(69); // Dark Katana
                        newItemsToSell.Add(Waffe69, 1500);
                    }
                    if (ReachedMineLevel > 50)
                    {
                        MeleeWeapon Waffe70 = new MeleeWeapon(70); // Mysterious Katana
                        newItemsToSell.Add(Waffe70, 5000);
                    }
                    if (ReachedMineLevel > 100)
                    {
                        MeleeWeapon Waffe71 = new MeleeWeapon(71); // Viking King Blade
                        newItemsToSell.Add(Waffe71, 10000);
                    }





                    Dictionary<ISalable, int[]> items = this.Helper.Reflection.GetField<Dictionary<ISalable, int[]>>(shop, "itemPriceAndStock").GetValue();
                    List<ISalable> selling = this.Helper.Reflection.GetField<List<ISalable>>(shop, "forSale").GetValue();

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
