/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using ItemPipes.Framework.Data;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Recipes
    ;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ItemPipes.Framework
{
    public class DataAccess
    {
        private static DataAccess myDataAccess;

        public IModHelper Helper { get; set; }
        public ITranslationHelper Translate { get; set; }

        public Dictionary<GameLocation, List<Network>> LocationNetworks { get; set; }
        public Dictionary<GameLocation, List<Node>> LocationNodes { get; set; }
        public Dictionary<string, int> ModItemsIDs { get; set; }
        public List<int> ModItems { get; set; }
        public List<int> NetworkItems { get; set; }
        public List<string> Buildings { get; set; }

        public Dictionary<GameLocation, List<long>>  UsedNetworkIDs { get; set; }
        public List<Thread> Threads { get; set; }

        public Dictionary<string, Texture2D> Sprites { get; set; }
        public Dictionary<string, string> Recipes { get; set; }
        public Dictionary<string, string> FakeRecipes { get; set; }
        public List<string> ItemIDNames { get; set; }
        public Dictionary<string, string> ItemNames { get; set; }
        public Dictionary<string, int> ItemIDs { get; set; }
        public Dictionary<string, string> ItemDescriptions { get; set; }
        public List<Item> LostItems { get; set; }
        public Dictionary<string, string> Letters { get; set; }
        public Dictionary<string, string> Warnings { get; set; }


        public DataAccess(IModHelper helper)
        {
            Helper = helper;
            Translate = helper.Translation;
            LocationNetworks = new Dictionary<GameLocation, List<Network>>();
            LocationNodes = new Dictionary<GameLocation, List<Node>>();
            ModItemsIDs = new Dictionary<string, int>();
            ModItems = new List<int>();
            NetworkItems = new List<int>();
            Buildings = new List<string>();
            Threads = new List<Thread>();
            UsedNetworkIDs = new Dictionary<GameLocation, List<long>>();
            Sprites = new Dictionary<string, Texture2D>();
            Recipes = new Dictionary<string, string>();
            FakeRecipes = new Dictionary<string, string>();
            ItemIDNames = new List<string>();
            ItemNames = new Dictionary<string, string>();
            ItemIDs = new Dictionary<string, int>();
            ItemDescriptions = new Dictionary<string, string>();
            LostItems = new List<Item>();

            Letters = new Dictionary<string, string>();
            Warnings = new Dictionary<string, string>();
        }

        public static DataAccess GetDataAccess()
        {
            if(myDataAccess == null)
            {
                myDataAccess = new DataAccess(ModEntry.helper);
            }
            return myDataAccess;
        }

        public bool RemoveThread(Thread thread)
        {
            try
            {
                if (DataAccess.GetDataAccess().Threads.Contains(thread))
                {
                    DataAccess.GetDataAccess().Threads.Remove(thread);
                    thread.Abort();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                DataAccess.GetDataAccess().Threads.Clear();
                return true;
            }
        }

        public long GetNewNetworkID(GameLocation location)
        {
            List<long> IDs = UsedNetworkIDs[location];
            if(IDs.Count == 0)
            {
                IDs.Add(1);
                return 1;
            }
            else
            {
                long newID = IDs[IDs.Count - 1] + 1;
                IDs.Add(newID);
                return newID;
            }
        }

        public List<Network> GetNetworkList(GameLocation location)
        {
            List<Network> networkList = null;
            foreach (KeyValuePair<GameLocation, List<Network>> pair in LocationNetworks)
            {
                if(pair.Key.Equals(location))
                {
                    networkList = pair.Value;
                }
            }
            return networkList;
        }

        public void LoadConfig()
        {
            ModConfig config = null;
            try
            {
                config = ModEntry.helper.ReadConfig<ModConfig>();
                if (config == null)
                {
                    Printer.Error($"The config file seems to be empty or invalid. Data class returned null.");
                }
            }
            catch (Exception ex)
            {
                Printer.Error($"The config file seems to be missing or invalid.\n{ex}");
            }


            //Normal debug = only errors
            if (config.DebugMode)
            {
                Globals.Debug = true;
                Printer.Debug("Debug mode ENABLED");
            }
            else
            {
                Globals.Debug = false;
                Printer.Debug("Debug mode DISABLED");
            }
            //Ultra debug = all the prints like step by step
            if (config.UltraDebugMode)
            {
                Globals.UltraDebug = true;
                Printer.Debug("UltraDebug mode ENABLED");
            }
            else
            {
                Globals.UltraDebug = false;
                Printer.Debug("UltraDebug mode DISABLED");
            }
            if (config.ItemSending)
            {
                Globals.ItemSending = true;
                Printer.Debug("Item sending ENABLED");
            }
            else
            {
                Globals.ItemSending = false;
                Printer.Debug("Item sending DISABLED");
            }
            if (config.IOPipeStatePopup)
            {
                Globals.IOPipeStatePopup = true;
                Printer.Debug("IOPipe state bubble popup ENABLED");
            }
            else
            {
                Globals.IOPipeStatePopup = false;
                Printer.Debug("IOPipe state bubble popup DISABLED");
            }
        }

        public void LoadAssets()
        {
            LoadIDs();
            LoadRecipes();
            ItemNames.Clear();
            ItemDescriptions.Clear();
            IEnumerable<Translation> translations = Translate.GetTranslations();
            foreach(Translation translation in translations)
            {
                string key = translation.Key;
                if(key.Contains("item") && !key.Contains("itempipes"))
                {
                    string IDName = key.Split(".")[1];
                    if(!ItemIDNames.Contains(IDName)) { ItemIDNames.Add(IDName); }
                    if(!ItemIDs.ContainsKey(IDName)) { ItemIDs.Add(IDName, ModItemsIDs[IDName]); }
                    if(key.Contains("name"))
                    {
                        if(!ItemNames.ContainsKey(IDName)) { ItemNames.Add(IDName, Translate.Get(key)); }
                    }
                    else if(key.Contains("description"))
                    {
                        if(!ItemDescriptions.ContainsKey(IDName)) { ItemDescriptions.Add(IDName, Translate.Get(key)); }
                    }
                }
                else if(key.Contains("letter"))
                {
                    if(!Letters.ContainsKey(key.Split(".")[1])) { Letters.Add(key.Split(".")[1], Translate.Get(key)); }
                }
                else if (key.Contains("warnings"))
                {
                    if(!Warnings.ContainsKey(key.Split(".")[1])) { Warnings.Add(key.Split(".")[1], Translate.Get(key)); }
                }
            }
            LoadSprites();
        }

        public void LoadIDs()
        {
            string dataPath = "assets/Data/ItemIDsData.json";
            ItemIDs IDs = null;
            try
            {
                IDs = ModEntry.helper.Data.ReadJsonFile<ItemIDs>(dataPath);
                if (IDs == null)
                {
                    Printer.Error($"The {dataPath} file seems to be empty or invalid. Data class returned null.");
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                Printer.Error($"The {dataPath} file seems to be missing or invalid.\n{ex}");
            }
            ModItemsIDs = IDs.ModItemsIDs;
            ModItems = IDs.ModItems;
            NetworkItems = IDs.NetworkItems;
            Buildings = IDs.Buildings;
        }
        
        public void LoadRecipes()
        {
            string dataPath = "assets/Data/RecipeData.json";
            RecipeData recipes = null;
            try
            {
                recipes = ModEntry.helper.Data.ReadJsonFile<RecipeData>(dataPath);
                if (recipes == null)
                {
                    Printer.Error($"The {dataPath} file seems to be empty or invalid. Data class returned null.");
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                Printer.Error($"The {dataPath} file seems to be missing or invalid.\n{ex}");
            }
            Recipes = recipes.recipesData;
            FakeRecipes = recipes.fakeRecipesData;
            foreach (KeyValuePair<string, string> pair in FakeRecipes)
            {
                if (!Game1.player.knowsRecipe(pair.Key) && CanLearnRecipe(pair.Value))
                {
                    Game1.player.craftingRecipes.Add(pair.Key, 0);
                }
            }
        }

        private bool CanLearnRecipe(string recipe)
        {
            bool can = false;
            int neededLvl = Int32.Parse(recipe.Split("/")[4].Split(" ")[1]);
            if(Game1.player.miningLevel.Value >= neededLvl)
            {
                can = true;
            }
            
            return can;
        }

        public void LoadSprites()
        {
            Sprites.Clear();
            IModContentHelper helper = ModHelper.GetHelper();
            try
            {
                List<string> pipes = new List<string>
                {"ironpipe", "goldpipe", "iridiumpipe", "extractorpipe", "goldextractorpipe",
                 "iridiumextractorpipe", "inserterpipe", "polymorphicpipe", "filterpipe"};
                foreach (string name in pipes)
                {
                    if (!name.Contains("iridium"))
                    {
                        Sprites.Add($"{name}_item", helper.Load<Texture2D>($"assets/Pipes/{name}/{name}_item.png"));
                        Sprites.Add($"{name}_default_sprite", helper.Load<Texture2D>($"assets/Pipes/{name}/{name}_default_sprite.png"));
                        Sprites.Add($"{name}_connecting_sprite", helper.Load<Texture2D>($"assets/Pipes/{name}/{name}_connecting_sprite.png"));
                        Sprites.Add($"{name}_item_sprite", helper.Load<Texture2D>($"assets/Pipes/{name}/{name}_item_sprite.png"));
                    }
                    else
                    {
                        Sprites.Add($"{name}_item", helper.Load<Texture2D>($"assets/Pipes/{name}/1/{name}_item.png"));

                        Sprites.Add($"{name}_item1", helper.Load<Texture2D>($"assets/Pipes/{name}/1/{name}_item.png"));
                        Sprites.Add($"{name}_default_sprite1", helper.Load<Texture2D>($"assets/Pipes/{name}/1/{name}_default_sprite.png"));
                        Sprites.Add($"{name}_connecting_sprite1", helper.Load<Texture2D>($"assets/Pipes/{name}/1/{name}_connecting_sprite.png"));
                        Sprites.Add($"{name}_item_sprite1", helper.Load<Texture2D>($"assets/Pipes/{name}/1/{name}_item_sprite.png"));

                        Sprites.Add($"{name}_item2", helper.Load<Texture2D>($"assets/Pipes/{name}/2/{name}_item.png"));
                        Sprites.Add($"{name}_default_sprite2", helper.Load<Texture2D>($"assets/Pipes/{name}/2/{name}_default_sprite.png"));
                        Sprites.Add($"{name}_connecting_sprite2", helper.Load<Texture2D>($"assets/Pipes/{name}/2/{name}_connecting_sprite.png"));
                        Sprites.Add($"{name}_item_sprite2", helper.Load<Texture2D>($"assets/Pipes/{name}/2/{name}_item_sprite.png"));

                        Sprites.Add($"{name}_item3", helper.Load<Texture2D>($"assets/Pipes/{name}/3/{name}_item.png"));
                        Sprites.Add($"{name}_default_sprite3", helper.Load<Texture2D>($"assets/Pipes/{name}/3/{name}_default_sprite.png"));
                        Sprites.Add($"{name}_connecting_sprite3", helper.Load<Texture2D>($"assets/Pipes/{name}/3/{name}_connecting_sprite.png"));
                        Sprites.Add($"{name}_item_sprite3", helper.Load<Texture2D>($"assets/Pipes/{name}/3/{name}_item_sprite.png"));
                    }
                }
                Sprites.Add("signal_on", helper.Load<Texture2D>($"assets/Pipes/on.png"));
                Sprites.Add("signal_off", helper.Load<Texture2D>($"assets/Pipes/off.png"));
                Sprites.Add("signal_unconnected", helper.Load<Texture2D>($"assets/Pipes/unconnected.png"));
                Sprites.Add("signal_nochest", helper.Load<Texture2D>($"assets/Pipes/nochest.png"));

                Sprites.Add("pipo_item", helper.Load<Texture2D>($"assets/Objects/PIPO/pipo_offC.png"));
                Sprites.Add("pipo_onR", helper.Load<Texture2D>($"assets/Objects/PIPO/pipo_onR.png"));
                Sprites.Add("pipo_onL", helper.Load<Texture2D>($"assets/Objects/PIPO/pipo_onL.png"));
                Sprites.Add("pipo_onC", helper.Load<Texture2D>($"assets/Objects/PIPO/pipo_onC.png"));
                Sprites.Add("pipo_offR", helper.Load<Texture2D>($"assets/Objects/PIPO/pipo_offR.png"));
                Sprites.Add("pipo_offL", helper.Load<Texture2D>($"assets/Objects/PIPO/pipo_offL.png"));
                Sprites.Add("pipo_offC", helper.Load<Texture2D>($"assets/Objects/PIPO/pipo_offC.png"));

                Sprites.Add("wrench_item", helper.Load<Texture2D>($"assets/Objects/Wrench/wrench_item.png"));

                Sprites.Add("nochest_state", helper.Load<Texture2D>($"assets/Misc/nochest_state.png"));
                Sprites.Add("nochest1_state", helper.Load<Texture2D>($"assets/Misc/nochest1_state.png"));
                //Sprites.Add("unconnected_state", ModEntry.helper.Content.Load<Texture2D>($"assets/Misc/unconnected_state.png"));
                Sprites.Add("unconnected1_state", helper.Load<Texture2D>($"assets/Misc/unconnected1_state.png"));
            }
            catch (Exception e)
            {
                Printer.Error("Can't load Item Pipes mod sprites!");
                Printer.Error(e.Message);
                Printer.Error(e.StackTrace);
            }
        }

        public void Reset()
        {
            LocationNodes.Clear();
            LocationNetworks.Clear();
            UsedNetworkIDs.Clear();
            Threads.Clear();
        }
    }
}
