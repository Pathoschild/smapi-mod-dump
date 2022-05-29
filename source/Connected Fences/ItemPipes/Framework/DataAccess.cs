/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SVObject = StardewValley.Objects;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Data;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace ItemPipes.Framework
{
    public class DataAccess
    {
        private static DataAccess myDataAccess;
        public Dictionary<GameLocation, List<Network>> LocationNetworks { get; set; }
        public Dictionary<GameLocation, List<Node>> LocationNodes { get; set; }
        public List<int> ModItems { get; set; }
        public List<int> NetworkItems { get; set; }
        public List<string> Buildings { get; set; }

        public Dictionary<GameLocation, List<int>>  UsedNetworkIDs { get; set; }
        public List<Thread> Threads { get; set; }

        public Dictionary<string, Texture2D> Sprites { get; set; }
        public Dictionary<string, string> Recipes { get; set; }
        public List<string> ItemIDNames { get; set; }
        public Dictionary<string, string> ItemNames { get; set; }
        public Dictionary<string, int> ItemIDs { get; set; }
        public Dictionary<string, string> ItemDescriptions { get; set; }
        public DataAccess()
        {
            LocationNetworks = new Dictionary<GameLocation, List<Network>>();
            LocationNodes = new Dictionary<GameLocation, List<Node>>();
            ModItems = new List<int>();
            NetworkItems = new List<int>();
            Buildings = new List<string>();
            Threads = new List<Thread>();

            UsedNetworkIDs = new Dictionary<GameLocation, List<int>>();
            Sprites = new Dictionary<string, Texture2D>();
            Recipes = new Dictionary<string, string>();
            ItemIDNames = new List<string>();
            ItemNames = new Dictionary<string, string>();
            ItemIDs = new Dictionary<string, int>();
            ItemDescriptions = new Dictionary<string, string>();

        }

        public static DataAccess GetDataAccess()
        {
            if(myDataAccess == null)
            {
                myDataAccess = new DataAccess();
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
        public int GetNewNetworkID(GameLocation location)
        {
            List<int> IDs = UsedNetworkIDs[location];
            if(IDs.Count == 0)
            {
                IDs.Add(1);
                return 1;
            }
            else
            {
                int newID = IDs[IDs.Count - 1] + 1;
                IDs.Add(newID);
                return newID;
            }
        }

        public List<Network> GetNetworkList(GameLocation location)
        {
            List<Network> graphList = null;
            foreach (KeyValuePair<GameLocation, List<Network>> pair in LocationNetworks)
            {
                if(pair.Key.Equals(location))
                {
                    graphList = pair.Value;
                }
            }
            return graphList;
        }

        public void LoadRecipes()
        {
            string dataPath = "assets/Data/RecipeInfo.json";
            RecipeData recipes = null;
            try
            {
                recipes = Helper.GetHelper().Data.ReadJsonFile<RecipeData>(dataPath);
                if (recipes == null)
                {
                    Printer.Error($"The {dataPath} file seems to be missing or invalid.");
                }
            }
            catch (Exception ex)
            {
                Printer.Error($"The {dataPath} file seems to be missing or invalid.\n{ex}");
            }
            Recipes = recipes.recipesData;
        }

        public void LoadItems()
        {
            string dataPath = "assets/Data/ItemInfo.json";
            ItemsData items = null;
            try
            {
                items = Helper.GetHelper().Data.ReadJsonFile<ItemsData>(dataPath);
                if (items == null)
                {
                    Printer.Error($"The {dataPath} file seems to be missing or invalid.");
                }
            }
            catch (Exception ex)
            {
                Printer.Error($"The {dataPath} file seems to be missing or invalid.\n{ex}");
            }
            ItemIDNames.Clear();
            ItemNames.Clear();
            ItemIDs.Clear();
            ItemDescriptions.Clear();
            var currLang = LocalizedContentManager.CurrentLanguageCode;
            for (int i=0;i< items.itemsData.Count; i++)
            {
                ItemIDNames.Add(items.itemsData[i].IDName);
                if(currLang != LocalizedContentManager.LanguageCode.en)
                {
                    if (items.itemsData[i].NameLocalization.ContainsKey(currLang.ToString())
                        && items.itemsData[i].NameLocalization[currLang.ToString()].Length > 0)
                    {
                        ItemNames.Add(items.itemsData[i].IDName, items.itemsData[i].NameLocalization[currLang.ToString()]);
                    }
                    else
                    {
                        ItemNames.Add(items.itemsData[i].IDName, items.itemsData[i].Name);
                    }
                    if (items.itemsData[i].DescriptionLocalization.ContainsKey(currLang.ToString())
                        && items.itemsData[i].DescriptionLocalization[currLang.ToString()].Length > 0)
                    {
                        ItemDescriptions.Add(items.itemsData[i].IDName, items.itemsData[i].DescriptionLocalization[currLang.ToString()]);
                    }
                    else
                    {
                        ItemDescriptions.Add(items.itemsData[i].IDName, items.itemsData[i].Description);
                    }
                }
                else
                {
                    ItemNames.Add(items.itemsData[i].IDName, items.itemsData[i].Name);
                    ItemDescriptions.Add(items.itemsData[i].IDName, items.itemsData[i].Description);
                }
                ItemIDs.Add(items.itemsData[i].IDName, items.itemsData[i].ID);
            }
        }


        public void LoadSprites()
        {
            try
            {
                List<string> pipes = new List<string>
                {"IronPipe", "GoldPipe", "IridiumPipe", "ExtractorPipe", "GoldExtractorPipe",
                 "IridiumExtractorPipe", "InserterPipe", "PolymorphicPipe", "FilterPipe"};
                foreach (string name in pipes)
                {
                    if (!name.Contains("Iridium"))
                    {
                        Sprites.Add($"{name}_Item", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/{name}_Item.png"));
                        Sprites.Add($"{name}_default_Sprite", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/{name}_default_Sprite.png"));
                        Sprites.Add($"{name}_connecting_Sprite", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/{name}_connecting_Sprite.png"));
                        Sprites.Add($"{name}_item_Sprite", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/{name}_item_Sprite.png"));
                    }
                    else
                    {
                        Sprites.Add($"{name}_Item", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/1/{name}_Item.png"));

                        Sprites.Add($"{name}_Item1", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/1/{name}_Item.png"));
                        Sprites.Add($"{name}_default_Sprite1", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/1/{name}_default_Sprite.png"));
                        Sprites.Add($"{name}_connecting_Sprite1", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/1/{name}_connecting_Sprite.png"));
                        Sprites.Add($"{name}_item_Sprite1", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/1/{name}_item_Sprite.png"));

                        Sprites.Add($"{name}_Item2", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/2/{name}_Item.png"));
                        Sprites.Add($"{name}_default_Sprite2", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/2/{name}_default_Sprite.png"));
                        Sprites.Add($"{name}_connecting_Sprite2", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/2/{name}_connecting_Sprite.png"));
                        Sprites.Add($"{name}_item_Sprite2", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/2/{name}_item_Sprite.png"));

                        Sprites.Add($"{name}_Item3", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/3/{name}_Item.png"));
                        Sprites.Add($"{name}_default_Sprite3", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/3/{name}_default_Sprite.png"));
                        Sprites.Add($"{name}_connecting_Sprite3", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/3/{name}_connecting_Sprite.png"));
                        Sprites.Add($"{name}_item_Sprite3", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{name}/3/{name}_item_Sprite.png"));
                    }
                }
                Sprites.Add("signal_on", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/on.png"));
                Sprites.Add("signal_off", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/off.png"));
                Sprites.Add("signal_unconnected", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/unconnected.png"));
                Sprites.Add("signal_nochest", ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/nochest.png"));

                Sprites.Add("PPM_Item", ModEntry.helper.Content.Load<Texture2D>($"assets/Objects/PPM/PPM_off.png"));
                Sprites.Add("PPM_on", ModEntry.helper.Content.Load<Texture2D>($"assets/Objects/PPM/PPM_on.png"));
                Sprites.Add("PPM_off", ModEntry.helper.Content.Load<Texture2D>($"assets/Objects/PPM/PPM_off.png"));
                Sprites.Add("Wrench_Item", ModEntry.helper.Content.Load<Texture2D>($"assets/Objects/Wrench/Wrench_Item.png"));

                Sprites.Add("nochest_state", ModEntry.helper.Content.Load<Texture2D>($"assets/Misc/nochest_state.png"));
                Sprites.Add("nochest1_state", ModEntry.helper.Content.Load<Texture2D>($"assets/Misc/nochest1_state.png"));
                //Sprites.Add("unconnected_state", ModEntry.helper.Content.Load<Texture2D>($"assets/Misc/unconnected_state.png"));
                Sprites.Add("unconnected1_state", ModEntry.helper.Content.Load<Texture2D>($"assets/Misc/unconnected1_state.png"));
            }
            catch (Exception e)
            {
                Printer.Info("Can't load Item Pipes mod sprites!");
                Printer.Info(e.Message);
                Printer.Info(e.StackTrace);
            }
        }
    }
}
