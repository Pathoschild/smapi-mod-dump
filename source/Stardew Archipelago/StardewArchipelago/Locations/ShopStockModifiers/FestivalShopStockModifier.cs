/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Locations;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Locations.Festival;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Shops;
using Category = StardewArchipelago.Constants.Vanilla.Category;

namespace StardewArchipelago.Locations.ShopStockModifiers
{
    public class FestivalShopStockModifier : ShopStockModifier
    {
        public FestivalShopStockModifier(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, StardewItemManager stardewItemManager) : base(monitor, modHelper, archipelago, stardewItemManager)
        {
        }

        public override void OnShopStockRequested(object sender, AssetRequestedEventArgs e)
        {
            if (_archipelago.SlotData.FestivalLocations == FestivalLocations.Vanilla)
            {
                return;
            }

            if (!AssetIsShops(e))
            {
                return;
            }

            e.Edit(asset =>
                {
                    var shopsData = asset.AsDictionary<string, ShopData>().Data;

                    foreach (var (shopId, shopData) in shopsData)
                    {
                        AddArchipelagoChecks(shopId, shopData);
                    }
                },
                AssetEditPriority.Late
            );
        }

        private void AddArchipelagoChecks(string shopId, ShopData shopData)
        {
            for (var i = shopData.Items.Count - 1; i >= 0; i--)
            {
                var item = shopData.Items[i];
                if (!IsItemKnownAsACheck(item, out var locationName, out var itemName))
                {
                    continue;
                }

                var apShopitem = CreateArchipelagoLocation(item, locationName);
                shopData.Items.Insert(i, apShopitem);
                ReplaceWithArchipelagoCondition(item, itemName);
            }
        }

        /// <summary>
        ///     Checks if a shop item in a festival should be an Archipelago check
        /// </summary>
        /// <param name="item">The shop item</param>
        /// <param name="locationName">The location name to create a check for</param>
        /// <param name="itemName">
        ///     the item required for the original item to show up in the shop alongside the check
        ///     If the item should always be there, returns string.empty. If the item should never be there, this will be an
        ///     invalid item name
        /// </param>
        /// <returns>true if this should be a check, false if not</returns>
        private bool IsItemKnownAsACheck(ShopItemData item, out string locationName, out string itemName)
        {
            if (_archipelago.LocationExists(item.ObjectInternalName) || _archipelago.LocationExists(item.ItemId))
            {
                locationName = item.ObjectInternalName;
                itemName = item.ObjectInternalName;
                return true;
            }

            locationName = null;
            itemName = null;
            if (item.ItemId == null)
            {
                return false;
            }
            var unqualifiedItemId = QualifiedItemIds.UnqualifyId(item.ItemId);

            if (item.IsRecipe)
            {
                var stardewItem = _stardewItemManager.GetItemByQualifiedId(item.ItemId) ?? _stardewItemManager.GetItemByQualifiedId(QualifiedItemIds.QualifiedObjectId(item.ItemId));
                var location = $"{stardewItem.Name}{Suffix.RECIPE}";
                if (_archipelago.LocationExists(location))
                {
                    locationName = location;
                    itemName = $"{location} (not purchaseable)";
                    return true;
                }

                return false;
            }

            var bigCraftablesData = DataLoader.BigCraftables(Game1.content);
            if (QualifiedItemIds.IsBigCraftable(item.ItemId) && bigCraftablesData.ContainsKey(unqualifiedItemId))
            {
                var bigCraftableData = bigCraftablesData[unqualifiedItemId];
                if (_archipelago.LocationExists(bigCraftableData.Name))
                {
                    locationName = bigCraftableData.Name;
                    itemName = bigCraftableData.Name;
                    return true;
                }

                if (IsRarecrow(unqualifiedItemId, bigCraftableData, out var rarecrowNumber))
                {
                    GetRarecrowCheckName(rarecrowNumber, out locationName, out itemName);
                    return true;
                }

                return false;
            }

            var objectsData = DataLoader.Objects(Game1.content);
            if (QualifiedItemIds.IsObject(item.ItemId) && objectsData.ContainsKey(unqualifiedItemId))
            {
                var objectData = objectsData[unqualifiedItemId];
                if (_archipelago.LocationExists(objectData.Name))
                {
                    locationName = objectData.Name;
                    itemName = objectData.Name;
                    return true;
                }

                if (objectData.Category == Category.SEEDS && objectData.Name == "Strawberry Seeds")
                {
                    locationName = FestivalLocationNames.STRAWBERRY_SEEDS;
                    itemName = string.Empty;
                    return true;
                }

                if (objectData.Name == "Stardrop" && item.Condition.Contains("PLAYER_HAS_MAIL Current CF_Fair"))
                {
                    locationName = FestivalLocationNames.FAIR_STARDROP;
                    itemName = "Fair Stardrop (not purchaseable)";
                    return true;
                }

                return false;
            }


            var hatsData = DataLoader.Hats(Game1.content);
            if (QualifiedItemIds.IsHat(item.ItemId) && hatsData.ContainsKey(unqualifiedItemId))
            {
                var hatData = hatsData[unqualifiedItemId];
                var hatName = hatData.Split('/')[0];
                if (_archipelago.LocationExists(hatName))
                {
                    locationName = hatName;
                    itemName = hatName;
                    return true;
                }

                return false;
            }

            var furnituresData = DataLoader.Furniture(Game1.content);
            if (QualifiedItemIds.IsFurniture(item.ItemId) && furnituresData.ContainsKey(unqualifiedItemId))
            {
                var furnitureData = furnituresData[unqualifiedItemId];
                var furnitureName = furnitureData.Split('/')[0];
                if (_archipelago.LocationExists(furnitureName))
                {
                    locationName = furnitureName;
                    itemName = furnitureName;
                    return true;
                }

                return false;
            }

            return false;
        }

        private bool IsRarecrow(string bigCraftableId, BigCraftableData bigCraftableData, out int rarecrowNumber)
        {
            if (!bigCraftableData.Name.Equals("Rarecrow", StringComparison.InvariantCultureIgnoreCase))
            {
                rarecrowNumber = 0;
                return false;
            }


            rarecrowNumber = bigCraftableId switch
            {
                "110" => 1,
                "113" => 2,
                "126" => 3,
                "136" => 4,
                "137" => 5,
                "138" => 6,
                "139" => 7,
                "140" => 8,
                _ => 0,
            };
            return true;
        }

        public void GetRarecrowCheckName(int rarecrowNumber, out string locationName, out string itemName)
        {
            locationName = rarecrowNumber switch
            {
                1 => FestivalLocationNames.RARECROW_1,
                2 => FestivalLocationNames.RARECROW_2,
                3 => FestivalLocationNames.RARECROW_3,
                4 => FestivalLocationNames.RARECROW_4,
                5 => FestivalLocationNames.RARECROW_5,
                6 => FestivalLocationNames.RARECROW_6,
                7 => FestivalLocationNames.RARECROW_7,
                8 => FestivalLocationNames.RARECROW_8,
            };

            itemName = locationName.Split("(")[0].Trim();
        }
    }
}
