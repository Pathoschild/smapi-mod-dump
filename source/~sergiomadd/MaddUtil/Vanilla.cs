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
using System.Text;
using StardewValley;
using System.Linq;


namespace MaddUtil
{
    public static class Vanilla
    {

        //Vanilla items
        public static List<int> VanillaObjects { get; set; }
        public static List<int> VanillaBigCraftables { get; set; }
        public static List<int> VanillaBoots { get; set; }
        public static List<int> VanillaClothing { get; set; }
        public static List<int> VanillaFurniture { get; set; }
        public static List<int> VanillaHats { get; set; }
        public static List<int> VanillaWeapons { get; set; }
        public static List<int> VanillaTools { get; set; }

        public static void Init()
        {
            VanillaObjects = new List<int>();
            VanillaBigCraftables = new List<int>();
            VanillaBoots = new List<int>();
            VanillaClothing = new List<int>();
            VanillaFurniture = new List<int>();
            VanillaHats = new List<int>();
            VanillaWeapons = new List<int>();
            VanillaTools = new List<int>();
            LoadVanillaData();
        }
        //Faltan weapons y wallpaper

        public static void LoadVanillaData()
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
        public static bool IsVanillaItem(Item item)
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
