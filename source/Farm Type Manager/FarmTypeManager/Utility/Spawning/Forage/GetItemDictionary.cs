/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Get a dictionary of the game's loaded item information for a given category of items.</summary>
            /// <param name="category">The name of the item category to retrieve.</param>
            /// <returns>A dictionary of IDs and data strings for each item of that category.</returns>
            public static IDictionary<int, string> GetItemDictionary(string category)
            {
                IDictionary<int, string> dictionary = null;

                switch (category.ToLower()) //based on the category
                {
                    case "bigcraftable":
                    case "bigcraftables":
                    case "big craftable":
                    case "big craftables":
                        dictionary = Game1.bigCraftablesInformation;
                        break;
                    case "boot":
                    case "boots":
                        dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Boots");
                        break;
                    case "cloth":
                    case "clothes":
                    case "clothing":
                    case "clothings":
                        dictionary = Game1.clothingInformation;
                        break;
                    case "furniture":
                        dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
                        break;
                    case "hat":
                    case "hats":
                        dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\hats");
                        break;
                    case "object":
                    case "objects":
                    case "item":
                    case "items":
                        dictionary = Game1.objectInformation;
                        break;
                    case "ring":
                    case "rings":
                        dictionary = Game1.objectInformation.Where(obj => obj.Value.Split('/')[3].Equals("Ring")).ToDictionary(obj => obj.Key, obj => obj.Value); //copy rings from the object dictionary
                        break;
                    case "weapon":
                    case "weapons":
                        dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\weapons");
                        break;
                }

                return dictionary;
            }
        }
    }
}