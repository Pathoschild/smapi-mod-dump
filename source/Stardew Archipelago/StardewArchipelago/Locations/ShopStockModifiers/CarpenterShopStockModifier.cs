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
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Shops;
using ArchipelagoLocation = StardewArchipelago.Locations.InGameLocations.ArchipelagoLocation;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.ShopStockModifiers
{
    public class CarpenterShopStockModifier : ShopStockModifier
    {
        public const string BUILDING_BLUEPRINT_LOCATION_NAME = "{0} Blueprint";
        public const string BUILDING_HOUSE_KITCHEN = "Kitchen";
        public const string BUILDING_HOUSE_KIDS_ROOM = "Kids Room";
        public const string BUILDING_HOUSE_CELLAR = "Cellar";

        public const string BUILDING_COOP = "Coop";
        public const string BUILDING_BARN = "Barn";
        public const string BUILDING_WELL = "Well";
        public const string BUILDING_SILO = "Silo";
        public const string BUILDING_MILL = "Mill";
        public const string BUILDING_SHED = "Shed";
        public const string BUILDING_FISH_POND = "Fish Pond";
        public const string BUILDING_STABLE = "Stable";
        public const string BUILDING_SLIME_HUTCH = "Slime Hutch";

        public const string BUILDING_BIG_COOP = "Big Coop";
        public const string BUILDING_DELUXE_COOP = "Deluxe Coop";
        public const string BUILDING_BIG_BARN = "Big Barn";
        public const string BUILDING_DELUXE_BARN = "Deluxe Barn";
        public const string BUILDING_BIG_SHED = "Big Shed";

        public const string BUILDING_SHIPPING_BIN = "Shipping Bin";

        public const string TRACTOR_GARAGE_ID = "Pathoschild.TractorMod_Stable";
        public const string TRACTOR_GARAGE_NAME = "Tractor Garage";

        public CarpenterShopStockModifier(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, StardewItemManager stardewItemManager) : base(monitor, modHelper, archipelago, stardewItemManager)
        {
        }

        public override void OnShopStockRequested(object sender, AssetRequestedEventArgs e)
        {
            if (!AssetIsShops(e))
            {
                return;
            }

            e.Edit(asset =>
                {
                    var shopsData = asset.AsDictionary<string, ShopData>().Data;
                    var carpenterShopData = shopsData["Carpenter"];
                    AddChecks(carpenterShopData);
                },
                AssetEditPriority.Late
            );
        }

        private void AddChecks(ShopData carpenterShopData)
        {
            if (!_archipelago.SlotData.BuildingProgression.HasFlag(BuildingProgression.Progressive))
            {
                return;
            }

            var checksToAdd = new List<ShopItemData>();

            AddCheckToStock(checksToAdd, BUILDING_HOUSE_KITCHEN, 10000, new[] { Wood(450) }, GetFarmhouseRequirementCondition(0));
            AddCheckToStock(checksToAdd, BUILDING_HOUSE_KIDS_ROOM, 50000, new[] { Hardwood(150) }, GetFarmhouseRequirementCondition(1));
            AddCheckToStock(checksToAdd, BUILDING_HOUSE_CELLAR, 100000, Array.Empty<Item>(), GetFarmhouseRequirementCondition(2));

            AddCheckToStock(checksToAdd, BUILDING_COOP, 4000, new[] { Wood(300), Stone(100) });
            AddCheckToStock(checksToAdd, BUILDING_BIG_COOP, 10000, new[] { Wood(400), Stone(150) }, GetBuildingRequirementCondition(BUILDING_COOP));
            AddCheckToStock(checksToAdd, BUILDING_DELUXE_COOP, 20000, new[] { Wood(500), Stone(200) }, GetBuildingRequirementCondition(BUILDING_BIG_COOP));

            AddCheckToStock(checksToAdd, BUILDING_BARN, 6000, new[] { Wood(350), Stone(150) });
            AddCheckToStock(checksToAdd, BUILDING_BIG_BARN, 12000, new[] { Wood(400), Stone(200) }, GetBuildingRequirementCondition(BUILDING_BARN));
            AddCheckToStock(checksToAdd, BUILDING_DELUXE_BARN, 25000, new[] { Wood(500), Stone(300) }, GetBuildingRequirementCondition(BUILDING_BIG_BARN));

            AddCheckToStock(checksToAdd, BUILDING_FISH_POND, 5000, new[] { Stone(200), Seaweed(5), GreenAlgae(5) });
            AddCheckToStock(checksToAdd, BUILDING_MILL, 2500, new[] { Stone(50), Wood(150), Cloth(4) });

            AddCheckToStock(checksToAdd, BUILDING_SHED, 15000, new[] { Wood(300) });
            AddCheckToStock(checksToAdd, BUILDING_BIG_SHED, 20000, new[] { Wood(550), Stone(300) }, GetBuildingRequirementCondition(BUILDING_SHED));

            AddCheckToStock(checksToAdd, BUILDING_SILO, 100, new[] { Stone(100), Clay(10), CopperBar(5) });
            AddCheckToStock(checksToAdd, BUILDING_SLIME_HUTCH, 10000, new[] { Stone(500), RefinedQuartz(10), IridiumBar(1) });
            AddCheckToStock(checksToAdd, BUILDING_STABLE, 10000, new[] { Hardwood(100), IronBar(5) });
            AddCheckToStock(checksToAdd, BUILDING_WELL, 1000, new[] { Stone(75) });
            AddCheckToStock(checksToAdd, BUILDING_SHIPPING_BIN, 250, new[] { Wood(150) });
            if (_archipelago.SlotData.Mods.HasMod(ModNames.TRACTOR))
            {
                AddCheckToStock(checksToAdd, TRACTOR_GARAGE_NAME, 150000, new[] { IronBar(20), IridiumBar(5), BatteryPack(5) });
            }

            carpenterShopData.Items.InsertRange(0, checksToAdd);
        }

        private void AddCheckToStock(List<ShopItemData> shopItems, string buildingName, int price, Item[] materials, string condition = null)
        {
            var locationName = string.Format(BUILDING_BLUEPRINT_LOCATION_NAME, buildingName);
            var id = $"{IDProvider.AP_LOCATION} {locationName}";

            var priceMultiplier = _archipelago.SlotData.BuildingPriceMultiplier;
            var finalPrice = (int)(price * priceMultiplier);
            var materialsString = string.Join(',', materials.Select(x => GetMaterialString(x, priceMultiplier)));

            var customFields = new Dictionary<string, string>()
            {
                { ArchipelagoLocation.EXTRA_MATERIALS_KEY, materialsString },
            };
            var blueprintCheck = new ShopItemData()
            {
                Id = id,
                ItemId = id,
                AvailableStock = 1,
                Price = finalPrice,
                IsRecipe = false,
                MaxItems = 1,
                Condition = condition,
                ModData = customFields,
                CustomFields = customFields,
            };

            shopItems.Add(blueprintCheck);
        }

        private static string GetFarmhouseRequirementCondition(int level)
        {
            if (level > 0)
            {
                return $"PLAYER_FARMHOUSE_UPGRADE Any {level}";
            }

            return null;
        }

        private static string GetBuildingRequirementCondition(string requiredBuilding)
        {
            if (requiredBuilding != null)
            {
                return $"BUILDINGS_CONSTRUCTED ALL \"{requiredBuilding}\"";
            }

            return null;
        }

        private string GetMaterialString(ISalable material, double priceMultiplier)
        {
            var amount = Math.Max(1, (int)Math.Round(material.Stack * priceMultiplier));
            return $"{QualifiedItemIds.UnqualifyId(material.QualifiedItemId)}:{amount}";
        }

        private static Item Wood(int amount)
        {
            return StardewObject(388, amount);
        }

        private static Item Stone(int amount)
        {
            return StardewObject(390, amount);
        }

        private static Item Seaweed(int amount)
        {
            return StardewObject(152, amount);
        }

        private static Item GreenAlgae(int amount)
        {
            return StardewObject(153, amount);
        }

        private static Item Cloth(int amount)
        {
            return StardewObject(428, amount);
        }

        private static Item Clay(int amount)
        {
            return StardewObject(330, amount);
        }

        private static Item CopperBar(int amount)
        {
            return StardewObject(334, amount);
        }

        private static Item RefinedQuartz(int amount)
        {
            return StardewObject(338, amount);
        }

        private static Item IridiumBar(int amount)
        {
            return StardewObject(337, amount);
        }

        private static Item Hardwood(int amount)
        {
            return StardewObject(709, amount);
        }

        private static Item IronBar(int amount)
        {
            return StardewObject(335, amount);
        }

        private static Item BatteryPack(int amount)
        {
            return StardewObject(787, amount);
        }

        private static Item StardewObject(int id, int amount)
        {
            return new Object(id.ToString(), amount);
        }
    }
}
