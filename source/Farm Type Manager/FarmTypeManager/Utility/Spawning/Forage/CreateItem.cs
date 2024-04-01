/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Generates an item described by a saved object.</summary>
            /// <param name="save">A saved object descibing an item.</param>
            /// <param name="tile">The object's intended tile location. Generally necessary for items derived from StardewValley.Object.</param>
            public static Item CreateItem(SavedObject save, Vector2 tile = default(Vector2))
            {
                switch (save.Type) //check the object's type
                {
                    case SavedObject.ObjectType.Object:
                    case SavedObject.ObjectType.Item:
                    case SavedObject.ObjectType.Container:
                    case SavedObject.ObjectType.DGA:
                        //these are valid item types
                        break;
                    default:
                        Monitor.Log($"Failed to create an item. Saved object does not appear to be an item.", LogLevel.Debug);
                        Monitor.Log($"Item name: {save.Name}", LogLevel.Debug);
                        return null;
                }

                if (save.ID == null && save.Type != SavedObject.ObjectType.Container && save.Type != SavedObject.ObjectType.DGA) //if this save doesn't have an ID (and isn't a container or a DGA item)
                {
                    Monitor.Log("Failed to create an item. Saved object contained no ID.", LogLevel.Debug);
                    Monitor.Log($"Item name: {save.Name}", LogLevel.Debug);
                    return null;
                }

                Item item = null; //the item to be generated
                ConfigItem configItem = save.ConfigItem; //the ConfigItem class describing the item (null if unavailable)

                //parse container contents, if applicable
                List<Item> contents = new List<Item>();
                if (save.Type == SavedObject.ObjectType.Container) //if this is a container
                {
                    string areaID = $"[unknown; parsing chest contents at {save.MapName}]"; //placeholder string; this method has no easy access to the areaID that created a given item
                    List<SavedObject> contentSaves = ParseSavedObjectsFromItemList(configItem.Contents, areaID); //parse the contents into saved objects for validation purposes

                    foreach (SavedObject contentSave in contentSaves) //for each successfully parsed save
                    {
                        Item content = CreateItem(contentSave); //call this method recursively to create this item
                        if (content != null) //if this item was created successfully
                        {
                            contents.Add(content); //add it to the contents list
                        }
                    }
                }

                string category = "item";
                if (configItem != null && configItem.Category != null)
                {
                    category = configItem.Category.ToLower();
                }

                switch (category) //based on the category
                {
                    case "barrel":
                    case "barrels":
                        item = new BreakableContainerFTM(tile, contents, true); //create a mineshaft-style breakable barrel with the given contents
                        break;
                    case "(bc)":
                    case "bc":
                    case "bigcraftable":
                    case "bigcraftables":
                    case "big craftable":
                    case "big craftables":
                        item = ItemRegistry.Create("(BC)" + save.StringID);
                        break;
                    case "(b)":
                    case "b":
                    case "boot":
                    case "boots":
                        item = new Boots(save.StringID);
                        break;
                    case "breakable":
                    case "breakables":
                        bool barrel = RNG.Next(0, 2) == 0 ? true : false; //randomly select whether this is a barrel or crate
                        if (configItem != null)
                        {
                            //rewrite the category to save the selection
                            if (barrel)
                            {
                                configItem.Category = "barrel";
                            }
                            else
                            {
                                configItem.Category = "crate";
                            }
                        }
                        item = new BreakableContainerFTM(tile, contents, barrel); //create a mineshaft-style breakable container with the given contents
                        break;
                    case "buried":
                    case "burieditem":
                    case "burieditems":
                    case "buried item":
                    case "buried items":
                        item = new BuriedItems(contents); //create an item burial location with the given contents
                        break;
                    case "chest":
                    case "chests":
                        item = new Chest(contents, tile); //create a mineshaft-style chest with the given contents
                        break;
                    case "crate":
                    case "crates":
                        item = new BreakableContainerFTM(tile, contents, false); //create a mineshaft-style breakable crate with the given contents
                        break;
                    case "dga":
                        try
                        {
                            object rawDGA = DGAItemAPI.SpawnDGAItem(save.Name); //create an item with DGA's API

                            if (rawDGA is Item itemDGA) //if this is a non-null Item
                                item = itemDGA; //use it
                            else
                            {
                                Monitor.Log("Failed to create an item. Dynamic Game Assets (DGA) item was null or an unrecognized type.", LogLevel.Debug);
                                Monitor.Log($"Item name: {save.Name}", LogLevel.Debug);
                                Monitor.Log($"Item type (C# code): {rawDGA?.GetType()?.Name ?? "null"}", LogLevel.Debug);
                                return null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"Error spawning a Dynamic Game Assets (DGA) item. The auto-generated error message has been added to the log.", LogLevel.Info);
                            Monitor.Log($"----------", LogLevel.Trace);
                            Monitor.Log($"{ex.ToString()}", LogLevel.Trace);
                            return null;
                        }
                        break;
                    case "(f)":
                    case "f":
                    case "furniture":
                        item = ItemRegistry.Create("(F)" + save.StringID);
                        break;
                    case "(h)":
                    case "h":
                    case "hat":
                    case "hats":
                        item = ItemRegistry.Create("(H)" + save.StringID);
                        break;
                    case "(o)":
                    case "o":
                    case "object":
                    case "objects":
                    case "item":
                    case "items":
                        item = new StardewValley.Object(save.StringID, 1); //create an "normal" object (in whatever way is preferable for a held/dropped item)
                        break;
                    case "(p)":
                    case "p":
                    case "pant":
                    case "pants":
                        item = ItemRegistry.Create("(P)" + save.StringID);
                        break;
                    case "ring":
                    case "rings":
                        item = new Ring(save.StringID);
                        break;
                    case "(s)":
                    case "s":
                    case "shirt":
                    case "shirts":
                        item = ItemRegistry.Create("(S)" + save.StringID);
                        break;
                    case "(t)":
                    case "t":
                    case "tool":
                    case "tools":
                        item = ItemRegistry.Create("(T)" + save.StringID);
                        break;
                    case "(w)":
                    case "w":
                    case "weapon":
                    case "weapons":
                        item = ItemRegistry.Create("(W)" + save.StringID);
                        break;
                }

                if (item == null) //if no item could be generated
                {
                    Monitor.Log("Failed to create an item. This may be caused by missing or uninstalled item mods.", LogLevel.Debug);
                    Monitor.Log($"Item category: {category}", LogLevel.Debug);
                    Monitor.Log($"Item name: {save.Name}", LogLevel.Debug);
                    return null;
                }

                if (configItem?.Stack > 1) //if this item has a custom stack setting
                {
                    int clampedStackValue = Math.Min(configItem.Stack.Value, item.maximumStackSize()); //limit the custom stack value to the maximum allowed by this item
                    item.Stack = clampedStackValue; //apply it
                }

                if (item is Furniture furniture) //if the created item is furniture (including from DGA, etc)
                {
                    int rotations = configItem.Rotation ?? 0; //if this item has a custom rotation setting, use it
                    furniture.SetPlacement(tile, rotations); //set the furniture's tile and rotation
                }

                return item;
            }
        }
    }
}