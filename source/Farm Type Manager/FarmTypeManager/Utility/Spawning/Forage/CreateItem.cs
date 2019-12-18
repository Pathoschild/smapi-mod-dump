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
            /// <param name="save">A saved object of the "Item" type.</param>
            public static Item CreateItem(SavedObject save)
            {
                if (save.Type != SavedObject.ObjectType.Item && save.Type != SavedObject.ObjectType.Object)
                {
                    Monitor.Log($"Failed to create an item. Saved object does not appear to be an item.", LogLevel.Debug);
                    Monitor.Log($"Item name: {save.Name}", LogLevel.Debug);
                    return null;
                }

                if (!save.ID.HasValue) //if this save doesn't have an ID
                {
                    Monitor.Log("Failed to create an item. Saved object contained no ID.", LogLevel.Debug);
                    Monitor.Log($"Item name: {save.Name}", LogLevel.Debug);
                    return null;
                }

                Item item = null; //the item to be generated
                ConfigItem configItem = save.ConfigItem; //the ConfigItem class describing the item (null if unavailable)

                string category = "item";
                if (configItem != null && configItem.Category != null)
                {
                    category = configItem.Category.ToLower();
                }

                switch (category) //based on the category
                {
                    case "bigcraftable":
                    case "bigcraftables":
                    case "big craftable":
                    case "big craftables":
                        item = new StardewValley.Object(default(Vector2), save.ID.Value, false); //create an object as a "big craftable" item
                        if (configItem?.Stack > 1) //if this item has a valid stack setting
                        {
                            item.Stack = configItem.Stack.Value; //apply it
                        }
                        break;
                    case "boot":
                    case "boots":
                        item = new Boots(save.ID.Value);
                        break;
                    case "cloth":
                    case "clothes":
                    case "clothing":
                    case "clothings":
                        item = new Clothing(save.ID.Value);
                        break;
                    case "furniture":
                        item = new Furniture(save.ID.Value, default(Vector2));
                        break;
                    case "hat":
                    case "hats":
                        item = new Hat(save.ID.Value);
                        break;
                    case "object":
                    case "objects":
                        item = new StardewValley.Object(default(Vector2), save.ID.Value, null, false, true, false, true); //create an object with the preferred constructor for "placed" objects
                        break;
                    case "item":
                    case "items":
                        int stackSize = 1;
                        if (configItem?.Stack > 1) //if this item has a valid stack setting
                        {
                            stackSize = configItem.Stack.Value; //apply it
                        }
                        item = new StardewValley.Object(default(Vector2), save.ID.Value, stackSize); //create an object with the preferred constructor for "held" or "dropped" items
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
                    Monitor.Log($"Item Category: {save.Name}", LogLevel.Debug);
                    Monitor.Log($"Item ID: {save.ID}", LogLevel.Debug);
                    return null;
                }

                item.ParentSheetIndex = save.ID.Value; //manually set this, due to it being ignored by some item subclasses

                return item;
            }
        }
    }
}