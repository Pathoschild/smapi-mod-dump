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
using System.Collections.Generic;
using StardewModdingAPI;
using Newtonsoft.Json;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A set of "advanced" options for a StardewValley.Item. Designed for readable JSON serialization.</summary>
        public class ConfigItem
        {
            /// <summary>The item's category, e.g. "Weapon" or "Chest".</summary>
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Category { get; set; }
            /// <summary>The item's name, e.g. "Galaxy Sword".</summary>
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Name { get; set; }
            /// <summary>The item's stack size.</summary>
            /// <remarks>This is only supported by categories that implement stack sizes.</remarks>
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? Stack { get; set; }
            /// <summary>The percent chance that the item will actually be spawned.</summary>
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public double? PercentChanceToSpawn { get; set; }
            /// <summary>The weighted chance that this item will be selected in a forage area's item list.</summary>
            /// <remarks>This setting is equivalent to adding multiple copies of the item to its forage list. It has no effect in "contents" or "loot" lists, which spawn all items.</remarks>
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? SpawnWeight { get; set; }
            /// <summary>A list of other items contained within this item. Only supported by certain categories.</summary>
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public List<object> Contents { get; set; }

            /// <summary>The object's type, determined by the Category string. Defaults to the Item type.</summary>
            [JsonIgnore]
            public SavedObject.ObjectType Type
            {
                get
                {
                    switch (Category.ToLower()) //based on the category
                    {
                        case "object":
                        case "objects":
                            if (Stack > 1) //if this has a custom stack size
                                return SavedObject.ObjectType.Item; //treat it as an item
                            else
                                return SavedObject.ObjectType.Object;
                        case "barrel":
                        case "barrels":
                        case "breakable":
                        case "breakables":
                        case "buried":
                        case "burieditem":
                        case "burieditems":
                        case "buried item":
                        case "buried items":
                        case "chest":
                        case "chests":
                        case "crate":
                        case "crates":
                            return SavedObject.ObjectType.Container;
                    }

                    return SavedObject.ObjectType.Item;
                }
            }

            public ConfigItem()
            {

            }
        }
    }
}