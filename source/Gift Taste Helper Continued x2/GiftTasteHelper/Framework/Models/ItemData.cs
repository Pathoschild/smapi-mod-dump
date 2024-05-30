/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/GiftTasteHelper
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.TokenizableStrings;

namespace GiftTasteHelper.Framework
{
    internal struct ItemCategory
    {
        public const string InvalidId = "-";

        public string Name;
        public string ID;

        public bool Valid => ID != InvalidId;
    }

    internal struct ItemData
    {
        public const int InEdible = -300;

        // Indices of data in yaml file: http://stardewvalleywiki.com/Modding:Object_data#Format
        public const int NameIndex = 0;
        public const int PriceIndex = 1;
        public const int EdibilityIndex = 2;
        public const int TypeIndex = 3; // Type and category
        public const int DisplayNameIndex = 4;
        public const int DescriptionIndex = 5;

        /*********
        ** Accessors
        *********/
        public string Name; // Always english        
        public string DisplayName; // Localized display name
        public int Price;
        public int Edibility;
        public ItemCategory Category;
        public string ID;
        public int SpriteIndex;
        public SVector2 NameSize;

        public readonly bool Edible => Edibility > InEdible;
        public readonly bool TastesBad => Edibility < 0;

        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"{{ID: {this.ID}, Name: {this.DisplayName}}}";
        }

        public static ItemCategory GetCategory(string itemId)
        {
            if (!Game1.objectData.TryGetValue(itemId, out var objectInfo)) 
            {
                return new ItemCategory { Name = "", ID = ItemCategory.InvalidId };
            }
            return new ItemCategory { Name = objectInfo.Name, ID = itemId };
        }

        public static ItemData MakeItem(string itemId)
        {
            if (!Game1.objectData.TryGetValue(itemId, out var objectInfo))
            {
                throw new System.ArgumentException($"Tried creating an item with an invalid id: {itemId}");
            }

            string tokenizedName = TokenParser.ParseText(objectInfo.DisplayName);
            return new ItemData
            {
                Name = objectInfo.Name,
                DisplayName = tokenizedName,
                Price = objectInfo.Price,
                Edibility = objectInfo.Edibility,
                ID = itemId,
                Category = ItemData.GetCategory(itemId),
                SpriteIndex = objectInfo.SpriteIndex,
                NameSize = SVector2.MeasureString(tokenizedName, Game1.smallFont)
            };
        }

        public static ItemData[] MakeItemsFromIds(IEnumerable<string> itemIds)
        {
            return itemIds
                .Where(id => Game1.objectData.ContainsKey(id))
                .Select(id => ItemData.MakeItem(id)).ToArray();
        }
    }
}
