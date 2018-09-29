using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Omegasis.CustomShopsRedux.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;

/*
TO DO:
Items
furniture
swords -
hats -
boots   -
wallpapers -
carpets -
ring -
lamp-
craftables-
*/
namespace Omegasis.CustomShopsRedux
{
    /// <summary>The mod entry point.</summary>
    public class CustomShopsRedux : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /// <summary>The prices for the items to list.</summary>
        private readonly Dictionary<Item, int[]> ListPrices = new Dictionary<Item, int[]>();

        /// <summary>The folder path containing shop data files.</summary>
        private string DataPath => Path.Combine(this.Helper.DirectoryPath, "Custom_Shops");


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            ControlEvents.KeyPressed += this.ControlEvents_KeyPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the presses a keyboard button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (Context.IsPlayerFree && e.KeyPressed.ToString() == this.Config.KeyBinding)
                this.OpenSelectFileMenu();
        }

        /// <summary>Open the menu which lets the player choose a file.</summary>
        private void OpenSelectFileMenu()
        {
            // get mod folder
            Directory.CreateDirectory(this.DataPath);
            DirectoryInfo modFolder = new DirectoryInfo(this.DataPath);
            this.Monitor.Log(modFolder.FullName);

            // get text files
            FileInfo[] files = modFolder.GetFiles("*.txt");
            if (!files.Any())
            {
                this.Monitor.Log("No shop .txt information is found. You should create one.", LogLevel.Error);
                return;
            }

            // parse options
            List<string> options = new List<string>();
            foreach (FileInfo file in files)
                options.Add(file.Name);
            if (!options.Any())
            {
                this.Monitor.Log("No shop .txt information is found. You should create one.", LogLevel.Error);
                return;
            }

            // load menu
            Game1.activeClickableMenu = new ChooseFromListMenu(options, this.OnChoiceSelected);
        }

        /// <summary>The method called when the player chooses an option from the file list.</summary>
        /// <param name="filename">The selected file name.</param>
        private void OnChoiceSelected(string filename)
        {
            // read file data
            string path = Path.Combine(this.DataPath, filename);
            string[] text = File.ReadAllLines(path);
            int lineCount = text.Length;

            // validate
            if (filename == "Custom_Shop_Redux_Config.txt")
            {
                this.Monitor.Log("Silly human. The config file is not a shop.", LogLevel.Info);
                return;
            }

            // parse data
            string objType = Convert.ToString(text[4]);
            int i = 6;
            while (i < lineCount)
            {
                if (i >= lineCount || objType == "" || text[i] == "") break;

                //read in a line for obj type here
                this.Monitor.Log(i.ToString());
                this.Monitor.Log(objType);
                int objID;
                int price;
                if (objType == "item" || objType == "Item" || objType == "Object" || objType == "object")
                {
                    objID = Convert.ToInt16(text[i]);
                    i += 2;

                    this.Monitor.Log(i.ToString());
                    bool isRecipe = Convert.ToBoolean(text[i]);
                    i += 2;
                    this.Monitor.Log(i.ToString());
                    price = Convert.ToInt32(text[i]);
                    i += 2;
                    this.Monitor.Log(i.ToString());

                    int quality = Convert.ToInt32(text[i]);

                    if (price == -1)
                    {
                        StardewValley.Object myobj = new StardewValley.Object(objID, int.MaxValue, isRecipe, price, quality);
                        price = myobj.salePrice();
                    }

                    this.ListPrices.Add(new StardewValley.Object(objID, int.MaxValue, isRecipe, price, quality), new[] { price, int.MaxValue });
                    i += 3;
                    if (i >= lineCount)
                        break;
                }

                if (objType == "Furniture" || objType == "furniture")
                {
                    objID = Convert.ToInt32(text[i]);
                    i += 2;
                    price = Convert.ToInt32(text[i]);

                    this.ListPrices.Add(new Furniture(objID, Vector2.Zero), new[] { price, int.MaxValue });
                    i += 3;
                    if (i >= lineCount)
                        break;
                }

                if (objType == "Boots" || objType == "boots" || objType == "shoe" || objType == "Shoe")  //just incase someone forgets it's called boots and they type shoe.
                {
                    objID = Convert.ToInt32(text[i]);
                    i += 2;
                    price = Convert.ToInt32(text[i]);
                    this.ListPrices.Add(new Boots(objID), new[] { price, int.MaxValue });
                    i += 3;
                    if (i >= lineCount)
                        break;
                }

                if (objType == "WallPaper" || objType == "Wallpaper" || objType == "wallPaper" || objType == "wallpaper")
                {
                    if (i + 3 > lineCount)
                        break;
                    objID = Convert.ToInt32(text[i]);
                    i += 2;
                    price = Convert.ToInt32(text[i]);
                    this.ListPrices.Add(new Wallpaper(objID), new[] { price, int.MaxValue }); //add in support for wallpapers and carpets
                    i += 3;
                    if (i >= lineCount)
                        break;

                }

                if (objType == "Carpet" || objType == "carpet" || objType == "Floor" || objType == "floor" || objType == "Rug" || objType == "rug")
                {
                    objID = Convert.ToInt32(text[i]);
                    i += 2;
                    price = Convert.ToInt32(text[i]);
                    this.ListPrices.Add(new Wallpaper(objID, true), new[] { price, int.MaxValue }); //add in support for wallpapers and carpets
                    i += 3;
                    if (i >= lineCount)
                        break;
                }

                if (objType == "Ring" || objType == "ring")
                {
                    objID = Convert.ToInt32(text[i]);
                    i += 2;
                    price = Convert.ToInt32(text[i]);
                    this.ListPrices.Add(new Ring(objID), new[] { price, int.MaxValue });
                    i += 3;
                    if (i >= lineCount)
                        break;
                }

                if (objType == "Lamp" || objType == "lamp" || objType == "Torch" || objType == "torch" || objType == "Craftable" || objType == "craftable" || objType == "BigCraftable" || objType == "bigcraftable")
                {
                    objID = Convert.ToInt32(text[i]);
                    i += 2;
                    price = Convert.ToInt32(text[i]);
                    this.ListPrices.Add(new Torch(Vector2.Zero, objID, true), new[] { price, int.MaxValue });
                    i += 3;
                    if (i >= lineCount)
                        break;
                }

                if (objType == "Sword" || objType == "sword" || objType == "Weapon" || objType == "weapon")
                {
                    objID = Convert.ToInt32(text[i]);
                    i += 2;
                    price = Convert.ToInt32(text[i]);
                    this.ListPrices.Add(new MeleeWeapon(objID), new[] { price, int.MaxValue });

                    i += 3;
                    if (i >= lineCount)
                        break;
                }

                if (objType == "Hat" || objType == "hat" || objType == "Hats" || objType == "hats")
                {
                    objID = Convert.ToInt32(text[i]);
                    i += 2;
                    price = Convert.ToInt32(text[i]);
                    this.ListPrices.Add(new Hat(objID), new[] { price, int.MaxValue });
                    i += 3;
                    if (i >= lineCount)
                        break;
                }

                //TODO:
                //add in support for colored objects
                //add in support for tools
                this.Monitor.Log(i.ToString());
                if (i >= lineCount)
                    break;

                objType = Convert.ToString(text[i]);
                i += 2;
            }

            //NEED TO TEST ALL DATA FILES TO SEE WHAT CAN AND CANT BE ADDED
            //list.Add((Item)new StardewValley.Objects.ColoredObject(475,300, Color.Aqua));
            Game1.activeClickableMenu = new ShopMenu(this.ListPrices, 0, "Pierre");
        }

        //basic default of Pierre
        //private void external_shop_file_call(string path, string fileName)
        //{
        //    List<Item> list = new List<Item>();


        //    string mylocation = Path.Combine(path, fileName);

        //    ///      Log.Info(mylocation);


        //    string[] readtext = File.ReadAllLines(mylocation);

        //    var lineCount = File.ReadLines(mylocation).Count();


        //    //          Log.Info(lineCount);

        //    int i = 4;

        //    int obj_id = 0;
        //    bool is_recipe;
        //    int price;
        //    int quality;

        //    string obj_type;

        //    obj_type = Convert.ToString(readtext[i]);
        //    i += 2;



        //    while (i < lineCount)
        //    {
        //        if (i >= lineCount) break;
        //        if (obj_type == "") break;
        //        if (readtext[i] == "") break;

        //        //read in a line for obj type here
        //        Log.Info(i);
        //        Log.Info(obj_type);
        //        if (obj_type == "item" || obj_type == "Item" || obj_type == "Object" || obj_type == "object")
        //        {

        //            obj_id = Convert.ToInt16(readtext[i]);
        //            i += 2;

        //            Log.Info(i);
        //            is_recipe = Convert.ToBoolean(readtext[i]);
        //            i += 2;
        //            Log.Info(i);
        //            price = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            Log.Info(i);

        //            quality = Convert.ToInt32(readtext[i]);
        //            // if (quality > 2) quality = 0;


        //            if (price == -1)
        //            {
        //                StardewValley.Object myobj = new StardewValley.Object(obj_id, int.MaxValue, is_recipe, price, quality);

        //                //   Log.Info("MYPRICE");
        //                //   Log.Info(myobj.salePrice());
        //                price = myobj.salePrice();

        //            }


        //            //   list.Add((Item)new StardewValley.Object(obj_id, int.MaxValue, is_recipe, price, quality));
        //            this.ListPrices.Add(new StardewValley.Object(obj_id, int.MaxValue, is_recipe, price, quality), new[] { price, int.MaxValue });
        //            i += 3;
        //            if (i >= lineCount) break;
        //        }
        //        if (obj_type == "Furniture" || obj_type == "furniture")
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);

        //            //  list.Add((Item)new Furniture(obj_id, Vector2.Zero)); //ADD FUNCTIONALITY TO SHOP FILES TO TEST IF FURNITURE OR NOT.
        //            this.ListPrices.Add(new Furniture(obj_id, Vector2.Zero), new[] { price, int.MaxValue });
        //            i += 3;
        //            if (i >= lineCount) break;
        //        }

        //        if (obj_type == "Boots" || obj_type == "boots" || obj_type == "shoe" || obj_type == "Shoe")  //just incase someone forgets it's called boots and they type shoe.
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            this.ListPrices.Add(new Boots(obj_id), new[] { price, int.MaxValue });
        //            i += 3;
        //            if (i >= lineCount) break;

        //        }

        //        if (obj_type == "WallPaper" || obj_type == "Wallpaper" || obj_type == "wallPaper" || obj_type == "wallpaper")  //just incase someone forgets it's called boots and they type shoe.
        //        {
        //            if (i + 3 > lineCount) break;
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            this.ListPrices.Add(new Wallpaper(obj_id, false), new[] { price, int.MaxValue }); //add in support for wallpapers and carpets
        //            i += 3;
        //            if (i >= lineCount) break;

        //        }

        //        if (obj_type == "Carpet" || obj_type == "carpet" || obj_type == "Floor" || obj_type == "floor" || obj_type == "Rug" || obj_type == "rug")
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            this.ListPrices.Add(new Wallpaper(obj_id, true), new[] { price, int.MaxValue }); //add in support for wallpapers and carpets
        //            i += 3;
        //            if (i >= lineCount) break;
        //        }

        //        if (obj_type == "Ring" || obj_type == "ring")
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            this.ListPrices.Add(new Ring(obj_id), new[] { price, int.MaxValue });
        //            i += 3;
        //            if (i >= lineCount) break;
        //        }
        //        if (obj_type == "Lamp" || obj_type == "lamp" || obj_type == "Torch" || obj_type == "torch" || obj_type == "Craftable" || obj_type == "craftable" || obj_type == "BigCraftable" || obj_type == "bigcraftable")
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            this.ListPrices.Add(new Torch(Vector2.Zero, obj_id, true), new[] { price, int.MaxValue });
        //            i += 3;
        //            if (i >= lineCount) break;
        //        }

        //        if (obj_type == "Sword" || obj_type == "sword" || obj_type == "Weapon" || obj_type == "weapon")
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            this.ListPrices.Add(new MeleeWeapon(obj_id), new[] { price, int.MaxValue });

        //            i += 3;
        //            if (i >= lineCount) break;
        //        }

        //        if (obj_type == "Hat" || obj_type == "hat" || obj_type == "Hats" || obj_type == "hats")
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            //  list_price.Add((Item)new Hat(obj_id), new[] { price, int.MaxValue });
        //            this.ListPrices.Add(new Hat(obj_id), new[] { price, int.MaxValue });
        //            i += 3;
        //            if (i >= lineCount) break;
        //        }


        //        //TODO:
        //        //add in support for colored objects
        //        //add in support for tools
        //        Log.Success(i);
        //        if (i >= lineCount) break;
        //        else
        //        {
        //            obj_type = Convert.ToString(readtext[i]);
        //            i += 2;
        //        }
        //    }


        //    //NEED TO TEST ALL DATA FILES TO SEE WHAT CAN AND CANT BE ADDED
        //    //list.Add((Item)new StardewValley.Objects.ColoredObject(475,300, Color.Aqua));
        //    //   Game1.activeClickableMenu = (IClickableMenu)new ShopMenu(list, 0, "Pierre");

        //    this.Menu = new ShopMenu(this.ListPrices, 0, "Pierre");
        //    Game1.activeClickableMenu = this.Menu;
        //}


        //uses NPCS with new dialogue and portraits
        //private void external_shop_file_call(string path, string fileName, string shopChat, NPC my_npc)
        //{

        //    List<Item> list = new List<Item>();


        //    string mylocation = Path.Combine(path, fileName);

        //    ///      Log.Info(mylocation);


        //    string[] readtext = File.ReadAllLines(mylocation);

        //    var lineCount = File.ReadLines(mylocation).Count();


        //    //          Log.Info(lineCount);

        //    int i = 4;

        //    int obj_id = 0;
        //    bool is_recipe;
        //    int price;
        //    int quality;

        //    string obj_type;

        //    obj_type = Convert.ToString(readtext[i]);
        //    i += 2;



        //    while (i < lineCount)
        //    {
        //        if (i >= lineCount) break;
        //        if (obj_type == "") break;
        //        if (readtext[i] == "") break;

        //        //read in a line for obj type here
        //        Log.Info(i);
        //        Log.Info(obj_type);
        //        if (obj_type == "item" || obj_type == "Item" || obj_type == "Object" || obj_type == "object")
        //        {

        //            obj_id = Convert.ToInt16(readtext[i]);
        //            i += 2;

        //            Log.Info(i);
        //            is_recipe = Convert.ToBoolean(readtext[i]);
        //            i += 2;
        //            Log.Info(i);
        //            price = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            Log.Info(i);

        //            quality = Convert.ToInt32(readtext[i]);
        //            // if (quality > 2) quality = 0;


        //            if (price == -1)
        //            {
        //                StardewValley.Object myobj = new StardewValley.Object(obj_id, int.MaxValue, is_recipe, price, quality);

        //                //   Log.Info("MYPRICE");
        //                //   Log.Info(myobj.salePrice());
        //                price = myobj.salePrice();

        //            }


        //            //   list.Add((Item)new StardewValley.Object(obj_id, int.MaxValue, is_recipe, price, quality));
        //            this.ListPrices.Add(new StardewValley.Object(obj_id, int.MaxValue, is_recipe, price, quality), new[] { price, int.MaxValue });
        //            i += 3;
        //            if (i >= lineCount) break;
        //        }
        //        if (obj_type == "Furniture" || obj_type == "furniture")
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);

        //            //  list.Add((Item)new Furniture(obj_id, Vector2.Zero)); //ADD FUNCTIONALITY TO SHOP FILES TO TEST IF FURNITURE OR NOT.
        //            this.ListPrices.Add(new Furniture(obj_id, Vector2.Zero), new[] { price, int.MaxValue });
        //            i += 3;
        //            if (i >= lineCount) break;




        //        }

        //        if (obj_type == "Boots" || obj_type == "boots" || obj_type == "shoe" || obj_type == "Shoe")  //just incase someone forgets it's called boots and they type shoe.
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            this.ListPrices.Add(new Boots(obj_id), new[] { price, int.MaxValue });
        //            i += 3;
        //            if (i >= lineCount) break;

        //        }

        //        if (obj_type == "WallPaper" || obj_type == "Wallpaper" || obj_type == "wallPaper" || obj_type == "wallpaper")  //just incase someone forgets it's called boots and they type shoe.
        //        {
        //            if (i + 3 > lineCount) break;
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            this.ListPrices.Add(new Wallpaper(obj_id, false), new[] { price, int.MaxValue }); //add in support for wallpapers and carpets
        //            i += 3;
        //            if (i >= lineCount) break;

        //        }

        //        if (obj_type == "Carpet" || obj_type == "carpet" || obj_type == "Floor" || obj_type == "floor" || obj_type == "Rug" || obj_type == "rug")
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            this.ListPrices.Add(new Wallpaper(obj_id, true), new[] { price, int.MaxValue }); //add in support for wallpapers and carpets
        //            i += 3;
        //            if (i >= lineCount) break;
        //        }

        //        if (obj_type == "Ring" || obj_type == "ring")
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            this.ListPrices.Add(new Ring(obj_id), new[] { price, int.MaxValue });
        //            i += 3;
        //            if (i >= lineCount) break;
        //        }
        //        if (obj_type == "Lamp" || obj_type == "lamp" || obj_type == "Torch" || obj_type == "torch" || obj_type == "Craftable" || obj_type == "craftable" || obj_type == "BigCraftable" || obj_type == "bigcraftable")
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            this.ListPrices.Add(new Torch(Vector2.Zero, obj_id, true), new[] { price, int.MaxValue });
        //            i += 3;
        //            if (i >= lineCount) break;
        //        }

        //        if (obj_type == "Sword" || obj_type == "sword" || obj_type == "Weapon" || obj_type == "weapon")
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            this.ListPrices.Add(new MeleeWeapon(obj_id), new[] { price, int.MaxValue });

        //            i += 3;
        //            if (i >= lineCount) break;
        //        }

        //        if (obj_type == "Hat" || obj_type == "hat" || obj_type == "Hats" || obj_type == "hats")
        //        {
        //            obj_id = Convert.ToInt32(readtext[i]);
        //            i += 2;
        //            price = Convert.ToInt32(readtext[i]);
        //            //  list_price.Add((Item)new Hat(obj_id), new[] { price, int.MaxValue });
        //            this.ListPrices.Add(new Hat(obj_id), new[] { price, int.MaxValue });
        //            i += 3;
        //            if (i >= lineCount) break;
        //        }


        //        //TODO:
        //        //add in support for colored objects
        //        //add in support for tools
        //        Log.Success(i);
        //        if (i >= lineCount) break;
        //        else
        //        {
        //            obj_type = Convert.ToString(readtext[i]);
        //            i += 2;
        //        }
        //    }


        //    //NEED TO TEST ALL DATA FILES TO SEE WHAT CAN AND CANT BE ADDED
        //    //list.Add((Item)new StardewValley.Objects.ColoredObject(475,300, Color.Aqua));
        //    //   Game1.activeClickableMenu = (IClickableMenu)new ShopMenu(list, 0, "Pierre");

        //    this.Menu = new ShopMenu(this.ListPrices, 0, my_npc.name)
        //    {
        //        potraitPersonDialogue = Game1.parseText(shopChat, Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4),
        //        portraitPerson = my_npc
        //    };
        //    Game1.activeClickableMenu = this.Menu;
        //}

        //example of a shop that I don't use.
        //private List<Item> MyShop()
        //{
        //    List<Item> list = new List<Item>();
        //    list.Add(new StardewValley.Object(478, int.MaxValue, false, -1, 0)); //int parentsheet index OR object_ID/int.MaxValue, bool is recipe, price, quality 
        //    list.Add(new StardewValley.Object(486, int.MaxValue, false, -1, 0));
        //    list.Add(new StardewValley.Object(494, int.MaxValue, false, -1, 0)); //Might be able to manipulate this code to give me recipes!!!!
        //    list.Add(new StardewValley.Object(495, int.MaxValue, false, 800, 0));  //price is *2 of value shown. -1 means inherit default value
        //    switch (Game1.dayOfMonth % 7)
        //    {
        //        case 0:
        //            list.Add(new StardewValley.Object(233, int.MaxValue, false, -1, 0));
        //            break;
        //        case 1:
        //            list.Add(new StardewValley.Object(88, int.MaxValue, false, -1, 0));
        //            break;
        //        case 2:
        //            list.Add(new StardewValley.Object(90, int.MaxValue, false, -1, 0));
        //            break;
        //        case 3:
        //            list.Add(new StardewValley.Object(749, int.MaxValue, false, 500, 0));
        //            break;
        //        case 4:
        //            list.Add(new StardewValley.Object(466, int.MaxValue, false, -1, 0));
        //            break;
        //        case 5:
        //            list.Add(new StardewValley.Object(340, int.MaxValue, false, -1, 0));
        //            break;
        //        case 6:
        //            list.Add(new StardewValley.Object(371, int.MaxValue, false, 100, 0));
        //            break;
        //    }
        //    return list;
        //}
    }
}
