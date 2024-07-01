/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/WhatDoYouWant
**
*************************************************/

using StardewValley;
using StardewValley.Extensions;

namespace WhatDoYouWant
{
    internal class Shipping
    {
        public const string SortOrder_Category = "Category";
        public const string SortOrder_ItemName = "ItemName";
        public const string SortOrder_CollectionsTab = "CollectionsTab"; // not exact; Wine, Pickles, Jelly, Juice are manually moved from end to specific spots in middle

        public static void ShowShippingList(ModEntry modInstance, Farmer who)
        {
            var linesToDisplay = new List<string>();

            var sortByCategory = (modInstance.Config.ShippingSortOrder == SortOrder_Category);
            var sortByItemName = (modInstance.Config.ShippingSortOrder == SortOrder_ItemName);
            var sortByCollectionsTab = (modInstance.Config.ShippingSortOrder == SortOrder_CollectionsTab);

            // adapted from base game logic to calculate full shipment %
            foreach (var parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData()
                .OrderBy(entry => sortByCollectionsTab ? entry.TextureName : "")
                .ThenBy(entry => sortByCollectionsTab ? entry.SpriteIndex : 0)
                .ThenBy(entry => sortByCategory ? StardewValley.Object.GetCategoryDisplayName(entry.Category) : "")
                .ThenBy(entry => entry.DisplayName)
            )
            {
                switch (parsedItemData.Category)
                {
                    // Is it part of Full Shipment?
                    case StardewValley.Object.CookingCategory:
                    case StardewValley.Object.GemCategory:
                        continue;
                    default:
                        if (!StardewValley.Object.isPotentialBasicShipped(
                            itemId: parsedItemData.ItemId,
                            category: parsedItemData.Category,
                            objectType: parsedItemData.ObjectType
                        ))
                        {
                            continue;
                        }

                        // Has it already been shipped?
                        if (who.basicShipped.ContainsKey(parsedItemData.ItemId))
                        {
                            continue;
                        }

                        // Add it to the list
                        var itemName = parsedItemData.DisplayName;
                        var categoryName = StardewValley.Object.GetCategoryDisplayName(parsedItemData.Category);
                        if (string.IsNullOrWhiteSpace(categoryName))
                        {
                            categoryName = "???";
                        }
                        if (sortByItemName)
                        {
                            itemName = $"{itemName} - {categoryName}";
                        } else
                        {
                            itemName = $"{categoryName} - {itemName}";
                        }
                        linesToDisplay.Add($"* {itemName}{ModEntry.LineBreak}");
                        break;
                }
            }

            if (linesToDisplay.Count == 0)
            {
                var completeDescription = modInstance.Helper.Translation.Get("Shipping_Complete", new { title = ModEntry.GetTitle_Shipping() });
                Game1.drawDialogueNoTyping(completeDescription);
                return;
            }
            
            modInstance.ShowLines(linesToDisplay, title: ModEntry.GetTitle_Shipping());
        }

    }
}
