using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using System.IO;

namespace CustomShopMod
{
    class myclass : Mod
    {
        public override void Entry(params object[] objects)
        {
            Command.RegisterCommand("shop_mod", "Allows purchasing from custom shops. | shopmod <value> <shop_list_num> <quantity>", new[] { "(String)<value> (Int32)<shop_list_num> (Int32)<quantity>" }).CommandFired += player_addItem;
        }
        public void player_addItem(object sender, EventArgsCommand e)
        {
            if (e.Command.CalledArgs.Length > 0)
            {
                if (e.Command.CalledArgs.Length <= 3)
                {
                    int quantity = Convert.ToInt32(e.Command.CalledArgs[2]);
                    int list_num = Convert.ToInt32(e.Command.CalledArgs[1]);
                    string newstring = e.Command.CalledArgs[0];
                    DataLoader(newstring, list_num, quantity);
                }
                else Log.Error("Invalid number of parameters! The paramaters are as follows: shop_mod (shop_name.txt) (Shop_List_Number) (quantity)");
            }
            else Log.Error("Invalid Parameters. Try Shop_Name.txt Shop_List_Id Quantity");
        }

        public void DataLoader(string funnystring, int list_num, int quantity)
        {
            int object_id = 0;
            int count = 0;
            int price = 0;

            //loads the data to the variables upon loading the game.
            //string myname = StardewValley.Game1.player.name;
            string mylocation = Path.Combine(PathOnDisk, funnystring);
            string mylocation3 = mylocation + ".txt";
            if (!File.Exists(mylocation3)) //if not data.json exists, initialize the data variables to the ModConfig data. I.E. starting out.
            {
                Log.Error("Invalid shop.Please make sure that " + mylocation3 + " exists!");

            }

            else
            {
                string[] separators = { "/", " " };
                var lineCount = File.ReadLines(mylocation3).Count();

                if (lineCount > 12 + ((list_num - 1) * 11))
                { //my logic seems reversed here?
                    string[] readtext = File.ReadAllLines(mylocation3);
                    string obj_name = readtext[6 + ((list_num - 1) * 11)];

                    string string_obj_id = readtext[8 + ((list_num - 1) * 11)];
                    string string_count_id = readtext[10 + ((list_num - 1) * 11)];
                    string string_price_id = readtext[12 + ((list_num - 1) * 11)];

                    string[] string_obj_id_arr = string_obj_id.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    string[] string_count_id_arr = string_count_id.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    //                    string[] string_price_id_arr = string_price_id.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    object_id = Convert.ToInt32(string_obj_id);
                    count = Convert.ToInt32(string_count_id);
                    price = Convert.ToInt32(string_price_id);

                    int array_size = string_obj_id_arr.Length;
                    int array_size2 = string_count_id_arr.Length;
                    Log.Info(array_size2);
                    Log.Info(array_size);

                    int i = 0;
                    //   string[] object_id_array = object_id.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    //Log.Info(quantity);
                    //Log.Info(price);
                    if (array_size == array_size2)
                    {
                        while (i < array_size && i < array_size2)
                        {
                            object_id = Convert.ToInt32(string_obj_id_arr[i]);
                            count = Convert.ToInt32(string_count_id_arr[i]);
                            price = Convert.ToInt32(string_price_id);
                            i++;
                            if (quantity * count <= 999)
                            {
                                if (StardewModdingAPI.Entities.SPlayer.Player.money >= quantity * price)
                                {
                                    StardewModdingAPI.Entities.SPlayer.Player.money -= quantity * price;
                                    var o = new StardewValley.Object(object_id, count * quantity);
                                    Game1.player.addItemByMenuIfNecessary(o);
                                    Log.Success("Successfully purchased " + obj_name + " from " + funnystring);
                                }
                                else Log.Error("Not enough Money!");

                            }
                            else Log.Error("Warning. Attempting to obtain more than 999 of an item. This loss was prevented.");
                        }
                    }
                    else Log.Error("Item Object Id's and Object count values are of different lengths. Please make sure that they are the same length! i.e each has the same amount of elements.");
                }


                else Log.Error("You tried to call something not in the text file. Please stick to the original format!");
                    

                }
            }
        }
    }