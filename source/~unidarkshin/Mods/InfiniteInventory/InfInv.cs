using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System.Timers;
using StardewValley.TerrainFeatures;
using StardewValley.Monsters;
using StardewValley.Locations;
using System.Collections;
using Newtonsoft.Json;
using System.Linq;

namespace InfiniteInventory
{

    public class InfInv
    {
        //public IList<Item> items;
        public List<IList<Item>> invs;
        public IList<Item> tb;
        public int currTab;
        public int maxTab;
        public int cost;
        public static Mod instance;
        public ModData config;


        public InfInv(Mod ins)
        {
            instance = ins;

            //items = new List<Item>();
            invs = new List<IList<Item>>();
            invs.Add(new List<Item>());
            currTab = 1;

            loadData();
        }
        //JsonSerializer a = new JsonSerializer();

        public void loadData()
        {
            config = instance.Helper.ReadJsonFile<ModData>($"data/{Constants.SaveFolderName}.json") ?? new ModData();

            List<List<string>> itemInfo = config.itemInfo;

            maxTab = config.maxTab;

            cost = config.cost;

            int y = 0;
            int z = 1;

            invs[0] = Game1.player.Items;
            invs.Add(new List<Item>());

            for (int i = 0; i < itemInfo.Count; i++)
            {


                if (y == 36)
                {
                    invs.Add(new List<Item>());
                    z++;
                    y = 0;
                }

                if (itemInfo[i][0] == "null")
                {
                    invs[z].Add((null));
                }
                else if (itemInfo[i][0] == "toolz")
                {
                    Tool x = (Tool)null;

                    if (itemInfo[i][1] == "0")
                    {
                        x = new Axe();
                        x.UpgradeLevel = int.Parse(itemInfo[i][2]);
                    }
                    else if (itemInfo[i][1] == "1")
                    {
                        x = new Hoe();
                    }
                    else if (itemInfo[i][1] == "2")
                    {
                        x = new FishingRod(int.Parse(itemInfo[i][2]));
                    }
                    else if (itemInfo[i][1] == "3")
                    {
                        x = new Pickaxe();
                        x.UpgradeLevel = int.Parse(itemInfo[i][2]);
                    }
                    else if (itemInfo[i][1] == "4")
                    {
                        x = new WateringCan();
                        x.UpgradeLevel = int.Parse(itemInfo[i][2]);
                    }
                    else if (itemInfo[i][1] == "5")
                    {
                        x = new MeleeWeapon(int.Parse(itemInfo[i][3])); //sprite index??
                        x.UpgradeLevel = int.Parse(itemInfo[i][2]);
                    }
                    else if (itemInfo[i][1] == "6")
                    {
                        x = new Slingshot();
                        x.UpgradeLevel = int.Parse(itemInfo[i][2]);
                    }
                    else if (itemInfo[i][1] == "7")
                    {
                        x = new Lantern();
                        //x.UpgradeLevel = int.Parse(itemInfo[i][2]);
                    }
                    else if (itemInfo[i][1] == "8")
                    {
                        x = new MagnifyingGlass();
                    }
                    else if (itemInfo[i][1] == "9")
                    {
                        x = new MilkPail();
                    }
                    else if (itemInfo[i][1] == "10")
                    {
                        x = new Pan();
                    }
                    else if (itemInfo[i][1] == "11")
                    {
                        x = new Raft();
                    }
                    else if (itemInfo[i][1] == "12")
                    {
                        x = new Seeds(itemInfo[i][2], int.Parse(itemInfo[i][3]));
                    }
                    else if (itemInfo[i][1] == "13")
                    {
                        x = new Shears();
                    }
                    else if (itemInfo[i][1] == "14")
                    {
                        x = new Sword(itemInfo[i][2], int.Parse(itemInfo[i][3]));
                    }
                    else if (itemInfo[i][1] == "15")
                    {
                        x = new Wand();
                    }

                    invs[z].Add(x);
                }
                else
                {
                    StardewValley.Object x = new StardewValley.Object(int.Parse(itemInfo[i][0]), int.Parse(itemInfo[i][1]));

                    //x.SpecialVariable = int.Parse(itemInfo[i][2]);
                    //x.Category = int.Parse(itemInfo[i][3]);
                    //x.Name = itemInfo[i][4];
                    //x.DisplayName = itemInfo[i][5];    

                    invs[z].Add((Item)(x));
                }

                y++;

            }



        }

        public void saveData()
        {

            config.maxTab = maxTab;
            config.cost = cost;

            List<List<string>> itemInfo = new List<List<string>>();

            //int y = 0;
            int z = 0;

            bool first = false;

            //itemInfo[0].Add(null);
            //itemInfo.Add(new List<string>());

            foreach (IList<Item> items in invs)
            {

                if (!first)
                {
                    first = true;
                }
                else
                {
                    foreach (Item item in items)
                    {
                        //string f = item != null ? item.Name : "Actually Null";
                        //instance.Monitor.Log($"Item: ---------->> {f}");
                        if (itemInfo.Count <= z)
                        {
                            itemInfo.Add(new List<string>());
                        }

                        if (item == null)
                        {
                            itemInfo[z].Add("null"); //Default index

                        }
                        else if (item is Tool)
                        {
                            //public Tool(string name, int upgradeLevel, int initialParentTileIndex, int indexOfMenuItemView, bool stackable, int numAttachmentSlots = 0)

                            itemInfo[z].Add("toolz");

                            Tool x = (item as Tool);


                            if (x is Axe)
                            {
                                itemInfo[z].Add("0");
                                itemInfo[z].Add(x.UpgradeLevel.ToString());
                            }
                            else if (x is Hoe)
                            {
                                itemInfo[z].Add("1");
                                itemInfo[z].Add(x.UpgradeLevel.ToString());
                            }
                            else if (x is FishingRod)
                            {
                                itemInfo[z].Add("2");
                                itemInfo[z].Add(x.UpgradeLevel.ToString());
                            }
                            else if (x is Pickaxe)
                            {
                                itemInfo[z].Add("3");
                                itemInfo[z].Add(x.UpgradeLevel.ToString());
                            }
                            else if (x is WateringCan)
                            {
                                itemInfo[z].Add("4");
                                itemInfo[z].Add(x.UpgradeLevel.ToString());
                            }
                            else if (x is MeleeWeapon)
                            {
                                itemInfo[z].Add("5");
                                itemInfo[z].Add(x.UpgradeLevel.ToString());
                                itemInfo[z].Add(x.InitialParentTileIndex.ToString());
                            }
                            else if (x is Slingshot)
                            {
                                itemInfo[z].Add("6");
                                itemInfo[z].Add(x.UpgradeLevel.ToString());
                            }
                            else if (x is Lantern)
                            {
                                itemInfo[z].Add("7");
                                //itemInfo[z].Add(x.UpgradeLevel.ToString());
                            }
                            else if (x is MagnifyingGlass)
                            {
                                itemInfo[z].Add("8");
                                //itemInfo[z].Add(x.UpgradeLevel.ToString());
                            }
                            else if (x is MilkPail)
                            {
                                itemInfo[z].Add("9");
                                //itemInfo[z].Add(x.UpgradeLevel.ToString());
                            }
                            else if (x is Pan)
                            {
                                itemInfo[z].Add("10");
                                //itemInfo[z].Add(x.UpgradeLevel.ToString());
                            }
                            else if (x is Raft)
                            {
                                itemInfo[z].Add("11");
                                //itemInfo[z].Add(x.UpgradeLevel.ToString());
                            }
                            else if (x is Seeds)
                            {
                                itemInfo[z].Add("12");
                                itemInfo[z].Add((x as Seeds).SeedType);
                                itemInfo[z].Add((x as Seeds).NumberInStack.ToString());
                            }
                            else if (x is Shears)
                            {
                                itemInfo[z].Add("13");
                            }
                            else if (x is Sword)
                            {
                                itemInfo[z].Add("14");
                                itemInfo[z].Add((x as Sword).Name);
                                itemInfo[z].Add(x.InitialParentTileIndex.ToString());
                            }
                            else if (x is Wand)
                            {
                                itemInfo[z].Add("15");
                            }
                            else
                            {
                                itemInfo[z][0] = "null";
                            }



                        }
                        else
                        {
                            itemInfo[z].Add(item.ParentSheetIndex.ToString()); //Index : [0]
                            itemInfo[z].Add(item.Stack.ToString()); //Stack : [1]
                                                                    //itemInfo[z].Add(item.SpecialVariable.ToString()); //Special Var : [2]
                                                                    //itemInfo[z].Add(item.Category.ToString()); //Category : [3]
                                                                    //itemInfo[z].Add(item.Name); //Name : [4]
                                                                    //itemInfo[z].Add(item.DisplayName); //Display Name : [5]



                            //instance.Monitor.Log($"Name --------->: {item.Name}");
                            //psi.Add(item.ParentSheetIndex);
                            //stack.Add(item.Stack);
                        }

                        z++;
                    }


                }

            }

            config.itemInfo = itemInfo;

            instance.Helper.WriteJsonFile<ModData>($"data/{Constants.SaveFolderName}.json", config);
        }

        public void changeTabs(int n)
        {
            if (n == maxTab + 1)
                n = 1;
            else if (n == 0)
                n = maxTab;

            if (currTab != n && n <= maxTab && n > 0)
            {
                while (invs.Count < n)
                {
                    invs.Add(new List<Item>());

                }

                IList<Item> x = new List<Item>();
                tb = new List<Item>();
                int y = 1;
                foreach (Item item in Game1.player.Items)
                {
                    if (y <= 12)
                    {
                        tb.Add(item);
                        x.Add(null);
                    }
                    else
                    {
                        x.Add(item);
                    }

                    y++;
                }

                invs[currTab - 1] = x;

                y = 1;
                IList<Item> tb_n = new List<Item>();
                foreach (Item item in tb)
                {
                    tb_n.Add(item);
                }
                foreach (Item item in invs[n - 1])
                {
                    if (y <= 12)
                    {

                    }
                    else
                    {
                        tb_n.Add(item);
                    }

                    y++;
                }

                Game1.player.setInventory((List<Item>)tb_n);


                currTab = n;
            }
            else
            {
                Game1.chatBox.addMessage($"You dont have {n} inventory tabs unlocked!", Color.Violet);
                return;
            }
        }

    }
}
