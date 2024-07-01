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
using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.GameModifications.CodeInjections;
using StardewArchipelago.Locations;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewArchipelago.Stardew;
using StardewArchipelago.Stardew.Ids.Vanilla;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.Shops;
using Category = StardewArchipelago.Constants.Vanilla.Category;

namespace StardewArchipelago.GameModifications
{
    public class SeedShopStockModifier
    {
        private const float JOJA_PRICE_MULTIPLIER = 0.8f;

        private IMonitor _monitor;
        private IModHelper _modHelper;
        private ArchipelagoClient _archipelago;
        private LocationChecker _locationChecker;
        private StardewItemManager _stardewItemManager;

        public SeedShopStockModifier(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, StardewItemManager stardewItemManager)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _stardewItemManager = stardewItemManager;
        }

        public void OnSeedShopStockRequested(object sender, AssetRequestedEventArgs e)
        {
            if (!e.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
            {
                return;
            }

            e.Edit(asset =>
                {
                    var shopsData = asset.AsDictionary<string, ShopData>().Data;

                    foreach (var (shopId, shopData) in shopsData)
                    {
                        OverhaulSeedAmounts(shopId, shopData);
                        FilterUnlockedSeeds(shopData);
                    }
                },
                AssetEditPriority.Late
            );
        }

        private void OverhaulSeedAmounts(string shopId, ShopData shopData)
        {
            if (!ModEntry.Instance.Config.EnableSeedShopOverhaul)
            {
                return;
            }

            OverhaulSmallBusinessSeeds(shopId, shopData);
            OverhaulBigCorporationSeeds(shopId, shopData);
        }

        private void OverhaulSmallBusinessSeeds(string shopId, ShopData shopData)
        {
            var isPierre = shopId == "SeedShop";
            var isSandy = shopId == "Sandy";
            if (!isPierre && !isSandy)
            {
                return;
            }

            var hasStocklist = Game1.MasterPlayer.hasOrWillReceiveMail("PierreStocklist");

            for (var i = shopData.Items.Count - 1; i >= 0; i--)
            {
                var item = shopData.Items[i];
                if (!SetSmallBusinessStock(shopData, item, hasStocklist, isPierre, i, out var priceMultiplier))
                {
                    continue;
                }

                if (isPierre)
                {
                    item.Price = (int)Math.Round(item.Price * priceMultiplier);
                }

                RemoveCondition(item, "YEAR 2");
                RemoveCondition(item, "DAYS_PLAYED 15");

                item.AvoidRepeat = true;
            }
        }

        private static void RemoveCondition(ShopItemData item, string condition)
        {
            if (item.Condition != null && item.Condition.Contains(condition))
            {
                item.Condition = string.Join(", ", item.Condition.Split(",").Select(x => x.Trim()).Where(x => !x.Contains(condition)));
            }
        }

        private bool SetSmallBusinessStock(ShopData shopData, ShopItemData shopItem, bool hasStocklist, bool isPierre, int i, out float priceMultiplier)
        {
            priceMultiplier = 1.0f;
            if (shopItem.AvailableStock != -1 || shopItem.IsRecipe)
            {
                return true;
            }

            var objectId = QualifiedItemIds.UnqualifyId(shopItem.ItemId);

            if (!_stardewItemManager.ObjectExistsById(objectId))
            {
                return true;
            }

            var stardewItem = _stardewItemManager.GetObjectById(objectId);
            if (stardewItem.Id == ObjectIds.BOUQUET)
            {
                return true;
            }

            var random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + shopItem.GetHashCode());
            var maxAmount = GetVillagerMaxAmountAndPrice(shopItem.Condition, hasStocklist && isPierre, ref priceMultiplier);
            var todayStock = random.Next(maxAmount);
            if (todayStock < 5)
            {
                shopData.Items.RemoveAt(i);
                return false; // Don't run the rest of the code
            }

            shopItem.AvailableStock = stardewItem.Name.Contains("Sapling", StringComparison.InvariantCultureIgnoreCase) ? 1 : todayStock;
            shopItem.AvailableStockLimit = LimitedStockMode.Global;

            return true;
        }

        private void OverhaulBigCorporationSeeds(string shopId, ShopData shopData)
        {
            if (shopId != "Joja")
            {
                return;
            }

            if (Game1.player.friendshipData.ContainsKey("Sandy"))
            {
                AddToJojaShop(shopData, QualifiedItemIds.RHUBARB_SEEDS);
                AddToJojaShop(shopData, QualifiedItemIds.STARFRUIT_SEEDS);
                AddToJojaShop(shopData, QualifiedItemIds.BEET_SEEDS);
            }

            if (TravelingMerchantInjections.HasAnyTravelingMerchantDay())
            {
                AddToJojaShop(shopData, QualifiedItemIds.RARE_SEED, 10, 1000);
            }

            // Seeds that Joja doesn't have in vanilla
            AddToJojaShop(shopData, QualifiedItemIds.GARLIC_SEEDS);
            AddToJojaShop(shopData, QualifiedItemIds.RICE_SHOOT);
            AddToJojaShop(shopData, QualifiedItemIds.BLUEBERRY_SEEDS);
            AddToJojaShop(shopData, QualifiedItemIds.RED_CABBAGE_SEEDS);
            AddToJojaShop(shopData, QualifiedItemIds.ARTICHOKE_SEEDS);

            // Overwrite these with the new version, stacked
            AddToJojaShop(shopData, QualifiedItemIds.SUGAR, 20);
            AddToJojaShop(shopData, QualifiedItemIds.WHEAT_FLOUR, 20);
            AddToJojaShop(shopData, QualifiedItemIds.RICE, 20);
            AddToJojaShop(shopData, QualifiedItemIds.OIL, 20);
            AddToJojaShop(shopData, QualifiedItemIds.VINEGAR, 20);

            shopData.PriceModifierMode = QuantityModifier.QuantityModifierMode.Stack;
            shopData.PriceModifiers = new List<QuantityModifier>()
            {
                new()
                {
                    Id = "MatchPierre",
                    Condition = null,
                    Modification = QuantityModifier.ModificationType.Multiply,
                    Amount = 2.0f,
                    RandomAmount = null,
                },
                new()
                {
                    Id = "BigCorporationDiscount",
                    Condition = null,
                    Modification = QuantityModifier.ModificationType.Multiply,
                    Amount = JOJA_PRICE_MULTIPLIER,
                    RandomAmount = null,
                }
            };

            var objectsData = DataLoader.Objects(Game1.content);
            foreach (var item in shopData.Items)
            {
                if (!QualifiedItemIds.IsObject(item.ItemId))
                {
                    continue;
                }

                var itemData = objectsData[QualifiedItemIds.UnqualifyId(item.ItemId)];
                item.AvailableStock = -1;
                item.AvoidRepeat = true;
                if (item.MinStack == -1)
                {
                    item.MinStack = itemData.Name.Contains("cola", StringComparison.InvariantCultureIgnoreCase) ? 6 : 50;
                }

                if (item.Price == -1)
                {
                    item.Price = itemData.Price;
                }

                item.Price *= item.MinStack;
                item.Condition = null;
            }
        }

        private void AddToJojaShop(ShopData shopData, string itemId, int stack = -1, int pricePerUnit = -1)
        {
            var existingItem = shopData.Items.Find(x => x.ItemId.Equals(itemId, StringComparison.InvariantCultureIgnoreCase));
            if (existingItem != null)
            {
                shopData.Items.Remove(existingItem);
            }

            var price = pricePerUnit;
            if (pricePerUnit > 0 && stack > 0)
            {
                price = stack * pricePerUnit;
            }
            var item = new ShopItemData()
            {
                Id = itemId,
                ItemId = itemId,
                MinStack = stack,
                MaxStack = -1,
                Price = price,
                Condition = null,
            };
            shopData.Items.Add(item);
        }

        private void FilterUnlockedSeeds(ShopData shopData)
        {
            if (_archipelago.SlotData.Cropsanity != Cropsanity.Shuffled)
            {
                return;
            }

            var itemsData = DataLoader.Objects(Game1.content);
            for (var i = shopData.Items.Count - 1; i >= 0; i--)
            {
                var item = shopData.Items[i];
                if (!QualifiedItemIds.IsObject(item.ItemId))
                {
                    continue;
                }

                var itemData = itemsData[QualifiedItemIds.UnqualifyId(item.ItemId)];

                if (itemData.Category != Category.SEEDS || itemData.Name == "Mixed Seeds")
                {
                    continue;
                }

                item.Condition ??= "";
                var conditions = item.Condition.Split(",").Select(x => x.Trim()).ToList();
                conditions.Add(GameStateConditionProvider.CreateHasReceivedItemCondition(itemData.Name));
                item.Condition = string.Join(", ", conditions);
            }
        }

        private int GetVillagerMaxAmountAndPrice(string itemSeason, bool hasStocklist, ref float priceMultiplier)
        {
            var maxAmount = 20;
            if (itemSeason != null && itemSeason != Game1.currentSeason)
            {
                priceMultiplier *= 1.5f;
            }

            if (hasStocklist)
            {
                maxAmount *= 2;
            }

            var numberMovieTheater = _archipelago.GetReceivedItemCount(APItem.MOVIE_THEATER);
            maxAmount *= (int)Math.Pow(2, numberMovieTheater);
            priceMultiplier *= (int)Math.Pow(1.5f, numberMovieTheater);

            return maxAmount;
        }
    }
}
