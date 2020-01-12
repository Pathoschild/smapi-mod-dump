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
            /// <summary>The item's stack size. Only supported by certain categories.</summary>
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? Stack { get; set; }
            /// <summary>The percent chance that the item will actually be spawned.</summary>
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? PercentChanceToSpawn { get; set; }
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
                            return SavedObject.ObjectType.Object;
                        case "barrel":
                        case "barrels":
                        case "breakable":
                        case "breakables":
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