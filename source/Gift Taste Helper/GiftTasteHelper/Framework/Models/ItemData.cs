using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace GiftTasteHelper.Framework
{
    internal struct ItemCategory
    {
        public const int InvalidId = 0;

        public string Name;
        public int ID;

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
        public int ID;
        public Rectangle TileSheetSourceRect;
        public SVector2 NameSize;

        public bool Edible => Edibility > InEdible;
        public bool TastesBad => Edibility < 0;

        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"{{ID: {this.ID}, Name: {this.DisplayName}}}";
        }

        public static ItemCategory GetCategory(int itemId)
        {
            return GetCategory(Game1.objectInformation[itemId]);
        }

        public static ItemCategory GetCategory(string objectInfo)
        {
            int categoryId = ItemCategory.InvalidId;
            var typeInfo = objectInfo.Split('/')[TypeIndex];
            var parts = typeInfo.Split(' '); // CategoryName [CategoryNumber]
            // Not all items have category numbers for some reason
            if (parts.Length > 1)
            {
                int.TryParse(parts[1], out categoryId);
            }
            return new ItemCategory{ Name = parts[0], ID = categoryId };
        }

        public static ItemData MakeItem(int itemId)
        {
            if (!Game1.objectInformation.ContainsKey(itemId))
            {
                throw new System.ArgumentException($"Tried creating an item with an invalid id: {itemId}");
            }

            string objectInfo = Game1.objectInformation[itemId];
            string[] parts = objectInfo.Split('/');

            return new ItemData
            {
                Name = parts[ItemData.NameIndex],
                DisplayName = parts[ItemData.DisplayNameIndex],
                Price = int.Parse(parts[ItemData.PriceIndex]),
                Edibility = int.Parse(parts[ItemData.EdibilityIndex]),
                ID = itemId,
                Category = ItemData.GetCategory(objectInfo),
                TileSheetSourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, itemId, 16, 16),
                NameSize = SVector2.MeasureString(parts[ItemData.DisplayNameIndex], Game1.smallFont)
            };
        }

        public static ItemData[] MakeItemsFromIds(IEnumerable<int> itemIds)
        {
            return itemIds
                .Where(id => Game1.objectInformation.ContainsKey(id))
                .Select(id => ItemData.MakeItem(id)).ToArray();
        }
    }
}
