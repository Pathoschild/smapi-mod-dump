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
using StardewValley.Locations;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;

namespace WhatDoYouWant
{
    internal class Museum
    {
        public const string SortOrder_Type = "Type";
        public const string SortOrder_ItemName = "ItemName";
        public const string SortOrder_CollectionsTabs = "CollectionsTabs";

        private static int GetTypeSortOrder(ParsedItemData parsedItemData)
        {
            var baseContextTags = ItemContextTagManager.GetBaseContextTags(parsedItemData.QualifiedItemId);
            if (baseContextTags.Contains("item_type_arch"))
            {
                return 1;
            }
            if (baseContextTags.Contains("item_type_minerals"))
            {
                return 2;
            }
            return 3;
        }

        public static void ShowMuseumList(ModEntry modInstance)
        {
            var linesToDisplay = new List<string>();

            var sortByType = (modInstance.Config.MuseumSortOrder == SortOrder_Type);
            var sortByItemName = (modInstance.Config.MuseumSortOrder == SortOrder_ItemName);
            var sortByCollectionsTabs = (modInstance.Config.MuseumSortOrder == SortOrder_CollectionsTabs);

            var itemType_Mineral = Game1.content.LoadString("Strings\\UI:Collections_Minerals");
            var itemType_Artifact = Game1.content.LoadString("Strings\\UI:Collections_Artifacts");

            // adapted from base game logic to award A Complete Collection achievement
            foreach (var parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData()
                .OrderBy(entry => sortByType || sortByCollectionsTabs ? GetTypeSortOrder(entry) : 0)
                .ThenBy(entry => sortByCollectionsTabs ? entry.TextureName : "")
                .ThenBy(entry => sortByCollectionsTabs ? entry.SpriteIndex : 0)
                .ThenBy(entry => entry.DisplayName)
            )
            {
                // Does it need to be donated?
                var qualifiedItemId = parsedItemData.QualifiedItemId;
                if (!LibraryMuseum.IsItemSuitableForDonation(qualifiedItemId, checkDonatedItems: true))
                {
                    continue;
                }
                var baseContextTags = ItemContextTagManager.GetBaseContextTags(qualifiedItemId);
                if (
                    !baseContextTags.Contains("museum_donatable")
                        && !baseContextTags.Contains("item_type_minerals")
                        && !baseContextTags.Contains("item_type_arch")
                )
                {
                    continue;
                }

                // Add it to the list
                string itemType;
                if (baseContextTags.Contains("item_type_minerals"))
                {
                    itemType = itemType_Mineral;
                } else
                {
                    if (baseContextTags.Contains("item_type_arch"))
                    {
                        itemType = itemType_Artifact;
                    } else
                    {
                        itemType = "???";
                    }
                }

                var dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(qualifiedItemId);
                var itemDescription = modInstance.GetItemDescription(dataOrErrorItem);
                if (sortByItemName)
                {
                    itemDescription = $"{itemDescription} - {itemType}";
                } else
                {
                    itemDescription = $"{itemType} - {itemDescription}";
                }

                linesToDisplay.Add($"* {itemDescription}{ModEntry.LineBreak}");
            }

            if (linesToDisplay.Count == 0)
            {
                var completeDescription = modInstance.Helper.Translation.Get("Museum_Complete", new { title = ModEntry.GetTitle_Museum() });
                Game1.drawDialogueNoTyping(completeDescription);
                return;
            }

            modInstance.ShowLines(linesToDisplay, title: ModEntry.GetTitle_Museum());
        }

    }
}
