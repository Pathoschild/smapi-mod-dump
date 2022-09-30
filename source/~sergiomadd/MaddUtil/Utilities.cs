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
using System.Linq;
using System.Text;
using System.Collections.Generic;
using StardewValley;
using SObject = StardewValley.Object;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewValley.Buildings;
using StardewValley.Locations;


namespace MaddUtil
{
    public static class Utilities
    {
        public static string GetNetworkLegend()
        {
            StringBuilder legend = new StringBuilder();

            legend.AppendLine("Legend: ");
            legend.AppendLine("C: Connector Pipe");
            legend.AppendLine("O: Output Pipe");
            legend.AppendLine("I: Input Pipe");
            legend.AppendLine("P: PIPO");

            return legend.ToString();
        }


        public static void DropItem(Item item, Vector2 position, GameLocation location)
        {
            Vector2 convertedPosition = new Vector2(position.X * 64, position.Y * 64);
            Debris itemDebr = new Debris(item, convertedPosition);
            location.debris.Add(itemDebr);
        }

        public static string GetIDName(string name)
        {
            string trimmed = "";
            if (name.Equals("PIPO"))
            {
                trimmed = name.ToLower();
            }
            else
            {
                trimmed = String.Concat(name.Where(c => !Char.IsWhiteSpace(c))).ToLower();
            }
            return trimmed;
        }

        public static string GetIDNameFromType(Type type)
        {
            string name = type.Name;
            string trimmed = name.Substring(0, name.Length - 4).ToLower();
            return trimmed;
        }

        public static string GetItemCategoryTag(Item item)
        {
            foreach (string tag in item.GetContextTags())
            {
                if (tag.Contains("category"))
                {
                    return tag.Split("_")[1];
                }
            }
            return "";
        }

        /*
        public static string GetIndexFromItem(Item item)
        {
            string index = "";
            string idTag = item.GetContextTagList()[0];
            string type = idTag.Split("_")[1];
            string tileSheetId = idTag.Split("_")[2];
            if (item is PipeItem)
            {
                type = "ip";
                tileSheetId = (item as PipeItem).ParentSheetIndex.ToString();
            }
            //no compara entre sub tipos. Tomato juice -> juice al crear el obj onLoad
            if(item is SObject)
            {
                index += type + "-" + tileSheetId+"-"+(item as SObject).Quality.ToString();
                if((item as SObject).preservedParentSheetIndex != null)
                {
                    index += "-" + (item as SObject).preservedParentSheetIndex.ToString();
                }
            }
            else
            {
                index += type + "-" + tileSheetId;
            }
            Printer.Info("saving");
            Printer.Info((item as SObject).preserve.ToString());
            foreach (string tag in item.GetContextTagList())
            {
                Printer.Info(tag);
            }
            Printer.Info("saved");
            return index;
        }
        
        public static Item GetItemFromIndex(string index)
        {
            string type = index.Split("-")[0];
            int tileSheetId = 1;
            if (index.Split("-")[1] != "")
            {
                tileSheetId = Int32.Parse(index.Split("-")[1]);
            }
            Item item = null;
			switch(type)
            {
				case "b"://boots
                    item = new Boots(tileSheetId);
					break;
                case "bo"://big craftable
                    item = new SObject(Vector2.Zero, tileSheetId, false);
                    break;
                case "c"://clothing
                    item = new Clothing(tileSheetId);
                    break;
                case "f"://furniture
                    item = new Furniture(tileSheetId, Vector2.Zero);
                    break;
                case "h"://hat
                    item = new Hat(tileSheetId);
                    break;
                case "o"://object
                    item = new SObject(Vector2.Zero, tileSheetId, 1);
                    if (index.Split("-")[2] != "")
                    {
                        (item as SObject).Quality = Int32.Parse(index.Split("-")[2]);
                    }
                    if(index.Split("-").Length >= 4)
                    {
                        item = test(item, index.Split("-")[3]);
                    }
                    /*
                    foreach (string tag in item.GetContextTagList())
                    {
                        Printer.Info(tag);
                    }
                    
                    break;
                case "r"://ring
                    item = new Ring(tileSheetId);
                    break;
                case "w"://melee weapon
                    item = new MeleeWeapon(tileSheetId);
                    break;
                case "ip"://item pipe
                    item = ItemFactory.CreateItemFromID(tileSheetId);
                    break;
                default:
                    Printer.Warn("Item type not supported in fitlers");
                    break;
            }
            return item;
		}
        */
        public static void ShowInGameMessage(string message, string type)
        {
            int numType = 0;
            switch(type)
            {
                case "achievement":
                    numType = 1;
                    break;
                case "quest":
                    numType = 2;
                    break;
                case "error":
                    numType = 3;
                    break;
                case "stamina":
                    numType = 4;
                    break;
                case "health":
                    numType = 5;
                    break;
                case "screenshot":
                    numType = 6;
                    break;
            }
            Game1.addHUDMessage(new HUDMessage(message, numType));
        }

        public static bool ToBool(string state)
        {
            if (state.Equals("True"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //Location getters made by Atravita
        public static IEnumerable<GameLocation> YieldAllLocations()
        {
            foreach (GameLocation location in Game1.locations)
            {
                yield return location;
                if (location is BuildableGameLocation buildableloc)
                {
                    foreach (GameLocation loc in YieldInteriorLocations(buildableloc))
                    {
                        yield return loc;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the buildings.
        /// </summary>
        /// <returns>IEnumerable of all buildings.</returns>
        public static IEnumerable<Building> GetBuildings()
        {
            foreach (GameLocation? loc in Game1.locations)
            {
                if (loc is BuildableGameLocation buildable)
                {
                    foreach (Building? building in GetBuildings(buildable))
                    {
                        yield return building;
                    }
                }
            }
        }

        private static IEnumerable<GameLocation> YieldInteriorLocations(BuildableGameLocation loc)
        {
            foreach (Building building in loc.buildings)
            {
                if (building.indoors?.Value is GameLocation indoorloc)
                {
                    yield return indoorloc;
                    if (indoorloc is BuildableGameLocation buildableIndoorLoc)
                    {
                        foreach (GameLocation nestedLocation in YieldInteriorLocations(buildableIndoorLoc))
                        {
                            yield return nestedLocation;
                        }
                    }
                }
            }
        }

        private static IEnumerable<Building> GetBuildings(BuildableGameLocation loc)
        {
            foreach (Building building in loc.buildings)
            {
                yield return building;
                if (building.indoors?.Value is BuildableGameLocation buildable)
                {
                    foreach (Building interiorBuilding in GetBuildings(buildable))
                    {
                        yield return interiorBuilding;
                    }
                }
            }
        }
    }
}
