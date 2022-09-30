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
using ItemPipes.Framework.Recipes;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ItemPipes.Framework.APIs;
using ItemPipes.Framework.Items.Objects;
using ItemPipes.Framework.Items.Tools;
using MaddUtil;


namespace ItemPipes.Framework
{
    public class DataAccess
    {
        private static DataAccess myDataAccess;

        public IModHelper Helper { get; set; }
        public ITranslationHelper Translate { get; set; }
        public List<GameLocation> AllLocations { get; set; }

        public Dictionary<GameLocation, List<Network>> LocationNetworks { get; set; }
        public Dictionary<GameLocation, List<Node>> LocationNodes { get; set; }
        public Dictionary<string, int> ModItemsIDs { get; set; }
        public List<int> ModItems { get; set; }
        public List<int> NetworkItems { get; set; }
        public List<string> Buildings { get; set; }

        public Dictionary<GameLocation, List<long>>  UsedNetworkIDs { get; set; }

        public Dictionary<string, Texture2D> Sprites { get; set; }
        public Dictionary<string, string> Recipes { get; set; }
        public Dictionary<string, string> FakeRecipes { get; set; }
        public List<string> ItemIDNames { get; set; }
        public Dictionary<string, string> ItemNames { get; set; }
        public Dictionary<string, int> ItemIDs { get; set; }
        public Dictionary<string, string> ItemDescriptions { get; set; }
        //public List<Item> LostItems { get; set; }
        public Dictionary<string, string> Letters { get; set; }
        public Dictionary<string, string> Warnings { get; set; }
        public Dictionary<string, string> UI { get; set; }


        //Vanilla items
        public List<int> VanillaObjects { get; set; }
        public List<int> VanillaBigCraftables { get; set; }
        public List<int> VanillaBoots { get; set; }
        public List<int> VanillaClothing { get; set; }
        public List<int> VanillaFurniture { get; set; }
        public List<int> VanillaHats { get; set; }
        public List<int> VanillaWeapons { get; set; }
        public List<int> VanillaTools { get; set; }


        public DataAccess(IModHelper helper)
        {
            Helper = helper;
            Translate = helper.Translation;

            AllLocations = new List<GameLocation>();
            LocationNetworks = new Dictionary<GameLocation, List<Network>>();
            LocationNodes = new Dictionary<GameLocation, List<Node>>();
            ModItemsIDs = new Dictionary<string, int>();
            ModItems = new List<int>();
            NetworkItems = new List<int>();
            Buildings = new List<string>();
            UsedNetworkIDs = new Dictionary<GameLocation, List<long>>();
            Sprites = new Dictionary<string, Texture2D>();
            Recipes = new Dictionary<string, string>();
            FakeRecipes = new Dictionary<string, string>();
            ItemIDNames = new List<string>();
            ItemNames = new Dictionary<string, string>();
            ItemIDs = new Dictionary<string, int>();
            ItemDescriptions = new Dictionary<string, string>();
            //LostItems = new List<Item>();

            Letters = new Dictionary<string, string>();
            Warnings = new Dictionary<string, string>();
            UI = new Dictionary<string, string>();

            VanillaObjects = new List<int>();
            VanillaBigCraftables = new List<int>();
            VanillaBoots = new List<int>();
            VanillaClothing = new List<int>();
            VanillaFurniture = new List<int>();
            VanillaHats = new List<int>();
            VanillaWeapons = new List<int>();
            VanillaTools = new List<int>();
        }

        public static DataAccess GetDataAccess()
        {
            if(myDataAccess == null)
            {
                myDataAccess = new DataAccess(ModEntry.helper);
            }
            return myDataAccess;
        }

        public void InitSave()
        {
            LoadLocations();
            LoadAssets();
            foreach (GameLocation location in AllLocations)
            {
                NetworkBuilder.BuildLocationNetworksTEMP(location);
                NetworkManager.UpdateLocationNetworks(location);
            }
        }

        public void LoadLocations()
        {
            List<GameLocation> locations = Utilities.YieldAllLocations().ToList();
            foreach (GameLocation location in locations)
            {
                TryRegisterLocation(location);
            }
        }

        public void TryRegisterLocation(GameLocation location)
        {
            if (!AllLocations.Contains(location)) { AllLocations.Add(location); }
            if (!LocationNetworks.ContainsKey(location)) { LocationNetworks.Add(location, new List<Network>()); }
            if (!LocationNodes.ContainsKey(location)) { LocationNodes.Add(location, new List<Node>()); }
            if (!UsedNetworkIDs.ContainsKey(location)) { UsedNetworkIDs.Add(location, new List<long>()); }
        }

        public void TryUnRegisterLocation(GameLocation location)
        {
            if (AllLocations.Contains(location)) { AllLocations.Remove(location); }
            if (LocationNetworks.ContainsKey(location)) { LocationNetworks.Remove(location); }
            if (LocationNodes.ContainsKey(location)) { LocationNodes.Remove(location); }
            if (UsedNetworkIDs.ContainsKey(location)) { UsedNetworkIDs.Remove(location); }
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

        public ModConfig LoadConfig()
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

            if (config.DebugMode)
            {
                Printer.Debug("Debug mode ENABLED");
            }
            else
            {
                Printer.Debug("Debug mode DISABLED");
            }
            if (config.ItemSending)
            {
                Printer.Debug("Item sending ENABLED");
            }
            else
            {
                Printer.Debug("Item sending DISABLED");
            }
            if (config.IOPipeStatePopup)
            {
                Printer.Debug("IOPipe state bubble popup ENABLED");
            }
            else
            {
                Printer.Debug("IOPipe state bubble popup DISABLED");
            }
            if (config.IOPipeStateSignals)
            {
                Printer.Debug("IOPipe signals ENABLED");
            }
            else
            {
                Printer.Debug("IOPipe signals DISABLED");
            }
            if(config == null)
            {
                throw new Exception("Error loading ModConfig from file");
            }

            return config;
        }

        public void LoadAssets()
        {
            LoadVanillaData();
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
                else if (key.Contains("warning"))
                {
                    if(!Warnings.ContainsKey(key.Split(".")[1])) { Warnings.Add(key.Split(".")[1], Translate.Get(key)); }
                }
                else if (key.Contains("ui"))
                {
                    if (!UI.ContainsKey(key.Split(".")[1])) { UI.Add(key.Split(".")[1], Translate.Get(key)); }
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
            IModContentHelper helper = Helpers.GetModContentHelper();
            try
            {
                List<string> pipes = new List<string>
                {"ironpipe", "goldpipe", "iridiumpipe", "extractorpipe", "goldextractorpipe",
                 "iridiumextractorpipe", "inserterpipe", "polymorphicpipe", "filterpipe", "invisibilizer"};
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

                Sprites.Add("rroff", helper.Load<Texture2D>($"assets/Pipes/rroff.png"));
                Sprites.Add("rron", helper.Load<Texture2D>($"assets/Pipes/rron.png"));

                Sprites.Add("invisibilizer_signal_offC", helper.Load<Texture2D>($"assets/Pipes/invisibilizer/invisibilizer_signal_offC.png"));
                Sprites.Add("invisibilizer_signal_offL", helper.Load<Texture2D>($"assets/Pipes/invisibilizer/invisibilizer_signal_offL.png"));
                Sprites.Add("invisibilizer_signal_offR", helper.Load<Texture2D>($"assets/Pipes/invisibilizer/invisibilizer_signal_offR.png"));
                Sprites.Add("invisibilizer_signal_onC", helper.Load<Texture2D>($"assets/Pipes/invisibilizer/invisibilizer_signal_onC.png"));
                Sprites.Add("invisibilizer_signal_onL", helper.Load<Texture2D>($"assets/Pipes/invisibilizer/invisibilizer_signal_onL.png"));
                Sprites.Add("invisibilizer_signal_onR", helper.Load<Texture2D>($"assets/Pipes/invisibilizer/invisibilizer_signal_onR.png"));

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
            AllLocations.Clear();
            LocationNodes.Clear();
            LocationNetworks.Clear();
            UsedNetworkIDs.Clear();
        }

        public void LoadVanillaData()
        {
            LocalizedContentManager manager = new LocalizedContentManager
                (Game1.game1.Content.ServiceProvider, Game1.game1.Content.RootDirectory);
            VanillaObjects = manager.Load<Dictionary<int, string>>("Data\\ObjectInformation").Keys.ToList();
            VanillaBigCraftables = manager.Load<Dictionary<int, string>>("Data\\BigCraftablesInformation").Keys.ToList();
            VanillaBoots = manager.Load<Dictionary<int, string>>("Data\\Boots").Keys.ToList();
            VanillaClothing = manager.Load<Dictionary<int, string>>("Data\\ClothingInformation").Keys.ToList();
            VanillaFurniture = manager.Load<Dictionary<int, string>>("Data\\Furniture").Keys.ToList();
            VanillaHats = manager.Load<Dictionary<int, string>>("Data\\hats").Keys.ToList();
            VanillaWeapons = manager.Load<Dictionary<int, string>>("Data\\weapons").Keys.ToList();
            //Load hardcoded tools
            VanillaTools.Add(189);//Axe
            VanillaTools.Add(105);//Pickaxe
            VanillaTools.Add(21);//Hoe
            VanillaTools.Add(273);//Watering can
            //VanillaTools.Add(189);//Fishing rod (same que axe?)
        }

        //Tools like Iridium pickaxe are not getting recognized
        public bool IsVanillaItem(Item item)
        {
            bool itis = false;
            string idTag = item.GetContextTagList()[0];
            string type = idTag.Split("_")[1];
            int id = Int32.Parse(idTag.Split("_")[2]);
            if (type == "")
            {
                type = item.getCategoryName();
            }
            switch (type)
            {
                case "b"://boots
                    if (VanillaBoots.Contains(id))
                    {
                        itis = true;
                    }
                    break;
                case "bbl"://big craftable recipe TODO
                    break;
                case "bl"://object recipe TODO
                    break;
                case "bo"://big craftable
                    if (VanillaBigCraftables.Contains(id))
                    {
                        itis = true;
                    }
                    break;
                case "c"://clothing
                    if (VanillaClothing.Contains(id))
                    {
                        itis = true;
                    }
                    break;
                case "f"://furniture
                    if (VanillaFurniture.Contains(id))
                    {
                        itis = true;
                    }
                    break;
                case "h"://hat
                    if (VanillaHats.Contains(id))
                    {
                        itis = true;
                    }
                    break;
                case "o"://object
                    if (VanillaObjects.Contains(id))
                    {
                        itis = true;
                    }
                    break;
                case "r"://ring
                    if (VanillaObjects.Contains(id))
                    {
                        itis = true;
                    }
                    break;
                case "w"://melee weapon
                    if (VanillaWeapons.Contains(id))
                    {
                        itis = true;
                    }
                    break;
                case "Tool"://tool
                    id = (item as Tool).InitialParentTileIndex;
                    if (VanillaTools.Contains(id))
                    {
                        itis = true;
                    }
                    break;
            }

            return itis;
        }
    }
}
