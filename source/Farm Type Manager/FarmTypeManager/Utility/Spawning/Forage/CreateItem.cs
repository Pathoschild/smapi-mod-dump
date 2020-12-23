/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

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
                        //these are valid item types
                        break;
                    default:
                        Monitor.Log($"Failed to create an item. Saved object does not appear to be an item.", LogLevel.Debug);
                        Monitor.Log($"Item name: {save.Name}", LogLevel.Debug);
                        return null;
                }   

                if (!save.ID.HasValue && save.Type != SavedObject.ObjectType.Container) //if this save doesn't have an ID (and isn't a container)
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
                    case "bigcraftable":
                    case "bigcraftables":
                    case "big craftable":
                    case "big craftables":
                        item = new StardewValley.Object(tile, save.ID.Value, false); //create an object as a "big craftable" item
                        break;
                    case "boot":
                    case "boots":
                        item = new Boots(save.ID.Value);
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
                        item = new BuriedItems(tile, contents); //create an item burial location with the given contents
                        break;
                    case "chest":
                    case "chests":
                        item = new Chest(0, contents, tile, false, 0); //create a mineshaft-style chest with the given contents
                        break;
                    case "cloth":
                    case "clothes":
                    case "clothing":
                    case "clothings":
                        item = new Clothing(save.ID.Value);
                        break;
                    case "crate":
                    case "crates":
                        item = new BreakableContainerFTM(tile, contents, false); //create a mineshaft-style breakable crate with the given contents
                        break;
                    case "furniture":
                        item = new Furniture(save.ID.Value, tile);
                        break;
                    case "hat":
                    case "hats":
                        item = new Hat(save.ID.Value);
                        break;
                    case "object": //treat objects as items when creating them as Items
                    case "objects":
                    case "item":
                    case "items":
                        item = new StardewValley.Object(tile, save.ID.Value, 1); //create an object with the preferred constructor for "held" or "dropped" items
                        break;
                    case "ring":
                    case "rings":
                        item = new Ring(save.ID.Value);
                        break;
                    case "weapon":
                    case "weapons":
                        item = new MeleeWeapon(save.ID.Value);
                        break;
                }

                if (item == null) //if no item could be generated
                {
                    Monitor.Log("Failed to create an item. Category setting was not recognized.", LogLevel.Debug);
                    Monitor.Log($"Item Category: {category}", LogLevel.Debug);
                    return null;
                }

                if (configItem?.Stack > 1) //if this item has a custom stack setting
                {
                    item.Stack = configItem.Stack.Value; //apply it
                }

                if (save.ID.HasValue) //if this object type uses an ID
                {
                    item.ParentSheetIndex = save.ID.Value; //manually set this index value, due to it being ignored by some item subclasses
                }

                return item;
            }
        }
    }
}