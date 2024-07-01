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
using Force.DeepCloner;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Locations.ShopStockModifiers;
using StardewArchipelago.Stardew;
using StardewArchipelago.Stardew.Ids.Vanilla;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Shops;

namespace StardewArchipelago.GameModifications.Modded
{
    public class JunimoShopStockModifier : BarterShopStockModifier
    {
        private static readonly string[] spring = new string[]{"spring"};
        private static readonly string[] summer = new string[]{"summer"};
        private static readonly string[] fall = new string[]{"fall"};
        private static readonly string[] winter = new string[]{"winter"};

        private static readonly Dictionary<string, string> _junimoPhrase = new()
        {
            { "Orange", "Look! Me gib pretty for orange thing!" },
            { "Red", "Give old things for red gubbins!" },
            { "Grey", "I trade rocks for grey what's-its!" },
            { "Yellow", "I hab seeds, gib yellow gubbins!" },
            { "Blue", "I hab fish! You give blue pretty?" },
            { "Purple", "Rare thing?  Purple thing!  Yay!"}
        };

        public JunimoShopStockModifier(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, StardewItemManager stardewItemManager) : base(monitor, helper, archipelago, stardewItemManager)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
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
                    var orangeShop = shopsData["FlashShifter.StardewValleyExpandedCP_OrangeJunimoVendor"];
                    var purpleShop = shopsData["FlashShifter.StardewValleyExpandedCP_PurpleJunimoVendor"];
                    var redShop = shopsData["FlashShifter.StardewValleyExpandedCP_RedJunimoVendor"];
                    var blueShop = shopsData["FlashShifter.StardewValleyExpandedCP_BlueJunimoVendor"];
                    var greyShop = shopsData["FlashShifter.StardewValleyExpandedCP_GreyJunimoVendor"];
                    var yellowShop = shopsData["FlashShifter.StardewValleyExpandedCP_YellowJunimoVendor"];
                    var discount = GiveApplesFriendshipDiscount();
                    var stockCount = StockBasedOnApplesFriendship();
                    GenerateBlueJunimoStock(blueShop, stockCount, discount);
                    GenerateRedJunimoStock(redShop, stockCount, discount);
                    GenerateGreyJunimoStock(greyShop, stockCount, discount);
                    GenerateYellowJunimoStock(yellowShop, stockCount, discount);
                    GenerateOrangeJunimoStock(orangeShop, stockCount, discount);
                    GeneratePurpleJunimoStock(purpleShop, stockCount, discount);
                },
                AssetEditPriority.Late
            );
        }
        
        private void GenerateBlueJunimoStock(ShopData shopData, int stockCount, double discount)
        {
            shopData.Owners[0].Dialogues[0].Dialogue = _junimoPhrase["Blue"];
            var fishData = DataLoader.Fish(Game1.content);
            shopData.Items.Clear();
            var blueObjects = _stardewItemManager.GetObjectsByColor("Blue");

            foreach (var (id, fishInfo) in fishData)
            {
                string[] fishSeasons = null;
                if (fishInfo.Split("/")[1] != "trap")
                {
                    fishSeasons = fishInfo.Split("/")[6].Split(" ");
                }
                var fishItem = _stardewItemManager.GetObjectById(id);
                var condition = $"SYNCED_RANDOM day junimo_shops 0.4 @addDailyLuck, PLAYER_HAS_CAUGHT_FISH Current {id}";
                if (fishSeasons is not null)
                {
                    condition = $"{GameStateConditionProvider.CreateSeasonsCondition(fishSeasons)}, {condition}";
                }
                shopData.Items.Add(CreateBarterItem(blueObjects, fishItem, condition, offeredStock: stockCount, discount: discount));
            }
        }

        private void GenerateGreyJunimoStock(ShopData shopData, int stockCount, double discount)
        {
            shopData.Owners[0].Dialogues[0].Dialogue = _junimoPhrase["Grey"];
            var mineralObjects = _stardewItemManager.GetObjectsByType("Minerals");
            shopData.Items.Clear();
            var greyItems = _stardewItemManager.GetObjectsByColor("Grey");

            foreach (var rock in mineralObjects)
            {
                var mineralCondition = $"{GameStateConditionProvider.FOUND_MINERAL} {rock.Id}, SYNCED_RANDOM day junimo_shops 0.4 @addDailyLuck";
                shopData.Items.Add(CreateBarterItem(greyItems, rock, mineralCondition, offeredStock: stockCount, discount: discount));
            }
        }

        private void GenerateRedJunimoStock(ShopData shopData, int stockCount, double discount)
        {
            shopData.Owners[0].Dialogues[0].Dialogue = _junimoPhrase["Red"];
            var artifactObjects = _stardewItemManager.GetObjectsByType("Arch");
            shopData.Items.Clear();
            var redObjects = _stardewItemManager.GetObjectsByColor("Red");
            foreach (var artifact in artifactObjects)
            {
                var artifactCondition = $"{GameStateConditionProvider.FOUND_ARTIFACT} {artifact.Id}, SYNCED_RANDOM day junimo_shops 0.4 @addDailyLuck";
                shopData.Items.Add(CreateBarterItem(redObjects, artifact, artifactCondition, offeredStock: stockCount, discount: discount));
            }
        }

        private void GenerateOrangeJunimoStock(ShopData shopData, int stockCount, double discount)
        {
            shopData.Owners[0].Dialogues[0].Dialogue = _junimoPhrase["Orange"];
            var orangeObjects = _stardewItemManager.GetObjectsByColor("Orange");
            foreach (var item in shopData.Items)
            {
                ReplaceCurrencyWithBarterGivenObjects(orangeObjects, item, offeredStock: stockCount, discount: discount);
            }
        }

        private void GeneratePurpleJunimoStock(ShopData shopData, int stockCount, double discount)
        {
            
            shopData.Owners[0].Dialogues[0].Dialogue = _junimoPhrase["Purple"];
            var itemToKeep = shopData.Items.First(x => x.ItemId == "FlashShifter.StardewValleyExpandedCP_Super_Starfruit");
            shopData.Items.Clear();
            var purpleObjects = _stardewItemManager.GetObjectsByColor("Purple");
            shopData.Items.Add(CreateBarterItem(purpleObjects, _stardewItemManager.GetObjectById(itemToKeep.Id), itemToKeep.Condition));
            shopData.Items.Add(CreateBarterItem(purpleObjects, _stardewItemManager.GetObjectByName("Magic Rock Candy")));
            shopData.Items.Add(CreateBarterItem(purpleObjects, _stardewItemManager.GetObjectByName("Dewdrop Berry"), overridePrice: 4000, offeredStock: stockCount, discount: discount));
            shopData.Items.Add(CreateBarterItem(purpleObjects, _stardewItemManager.GetObjectByName("Qi Gem"), overridePrice: 10000, offeredStock: stockCount, discount: discount));
            shopData.Items.Add(CreateBarterItem(purpleObjects, _stardewItemManager.GetObjectByName("Calico Egg"), overridePrice: 500, offeredStock: stockCount, discount: discount));
            shopData.Items.Add(CreateBarterItem(purpleObjects, _stardewItemManager.GetObjectByName("Hardwood"), overridePrice: 500, offeredStock: stockCount, discount: discount));
            shopData.Items.Add(CreateBarterItem(purpleObjects, _stardewItemManager.GetObjectByName("Tea Set"), overridePrice: 100000, offeredStock: stockCount, discount: discount));
        }

        private void GenerateYellowJunimoStock(ShopData shopData, int stockCount, double discount)
        {
            shopData.Owners[0].Dialogues[0].Dialogue = _junimoPhrase["Yellow"];
            shopData.Items.Clear();
            var yellowObjects = _stardewItemManager.GetObjectsByColor("Yellow");
            AddSpringSeedsToYellowStock(yellowObjects, shopData, stockCount, discount);
            AddSummerSeedsToYellowStock(yellowObjects, shopData, stockCount, discount);
            AddFallSeedsToYellowStock(yellowObjects, shopData, stockCount, discount);
            AddSaplingsToShop(yellowObjects, shopData, stockCount, discount);
        }

        private int StockBasedOnApplesFriendship()
        {
            var applesHearts = 0;
            if (Game1.player.friendshipData.ContainsKey("Apples"))
            {
                applesHearts = Game1.player.friendshipData["Apples"].Points / 500;
            }
            return applesHearts + 1;
        }

        private double GiveApplesFriendshipDiscount()
        {
            var applesHearts = 0;
            if (Game1.player.friendshipData.ContainsKey("Apples"))
            {
                applesHearts = Game1.player.friendshipData["Apples"].Points / 250; // Get discount from being friends with Apples
            }
            return 1 - applesHearts * 0.05f;
        }

        private ShopItemData CreateJunimoSeedItem(List<StardewObject> yellowObjects, string qualifiedId, int stockCount, double discount, string[] season = null)
        {
            var seedItemName = _stardewItemManager.GetItemByQualifiedId(qualifiedId).Name;
            var condition = "SYNCED_RANDOM day junimo_shops 0.4 @addDailyLuck";;
            if (season is not null)
            {
                condition = $"{GameStateConditionProvider.CreateSeasonsCondition(season)}, {condition}";
            }
            if (_archipelago.SlotData.Cropsanity == Cropsanity.Shuffled)
            {
                condition = $"{GameStateConditionProvider.CreateHasReceivedItemCondition(seedItemName)}, {condition}";
            }
            return CreateBarterItem(yellowObjects, _stardewItemManager.GetItemByQualifiedId(qualifiedId), condition, offeredStock: stockCount, discount: discount);
        }

        private void AddSpringSeedsToYellowStock(List<StardewObject> yellowObjects, ShopData shopData, int stockCount, double discount)
        {
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.PARSNIP_SEEDS, stockCount, discount, spring));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.BEAN_STARTER, stockCount, discount, spring));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.CAULIFLOWER_SEEDS, stockCount, discount, spring));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.POTATO_SEEDS, stockCount, discount, spring));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.TULIP_BULB, stockCount, discount, spring));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.KALE_SEEDS, stockCount, discount, spring));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.JAZZ_SEEDS, stockCount, discount, spring));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.GARLIC_SEEDS, stockCount, discount, spring));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.RICE_SHOOT, stockCount, discount, spring));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.RHUBARB_SEEDS, stockCount, discount, spring));
        }

        private void AddSummerSeedsToYellowStock(List<StardewObject> yellowObjects, ShopData shopData, int stockCount, double discount)
        {
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.MELON_SEEDS, stockCount, discount, summer));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.TOMATO_SEEDS, stockCount, discount, summer));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.BLUEBERRY_SEEDS, stockCount, discount, summer));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.PEPPER_SEEDS, stockCount, discount, summer));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.WHEAT_SEEDS, stockCount, discount, summer));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.RADISH_SEEDS, stockCount, discount, summer));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.POPPY_SEEDS, stockCount, discount, summer));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.SPANGLE_SEEDS, stockCount, discount, summer));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.HOPS_STARTER, stockCount, discount, summer));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.CORN_SEEDS, stockCount, discount, summer));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.SUNFLOWER_SEEDS, stockCount, discount, summer));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.RED_CABBAGE_SEEDS, stockCount, discount, summer));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.STARFRUIT_SEEDS, stockCount, discount, summer));
        }

        private void AddFallSeedsToYellowStock(List<StardewObject> yellowObjects, ShopData shopData, int stockCount, double discount)
        {
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.PUMPKIN_SEEDS, stockCount, discount, fall));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.CORN_SEEDS, stockCount, discount, fall));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.EGGPLANT_SEEDS, stockCount, discount, fall));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.BOK_CHOY_SEEDS, stockCount, discount, fall));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.YAM_SEEDS, stockCount, discount, fall));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.CRANBERRY_SEEDS, stockCount, discount, fall));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.WHEAT_SEEDS, stockCount, discount, fall));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.SUNFLOWER_SEEDS, stockCount, discount, fall));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.FAIRY_SEEDS, stockCount, discount, fall));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.AMARANTH_SEEDS, stockCount, discount, fall));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.GRAPE_STARTER, stockCount, discount, fall));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.ARTICHOKE_SEEDS, stockCount, discount, fall));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.BEET_SEEDS, stockCount, discount, fall));
        }

        private void AddSaplingsToShop(List<StardewObject> yellowObjects, ShopData shopData, int stockCount, double discount)
        {
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.CHERRY_SAPLING, stockCount, discount));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.APRICOT_SAPLING, stockCount, discount));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.ORANGE_SAPLING, stockCount, discount));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.PEACH_SAPLING, stockCount, discount));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.POMEGRANATE_SAPLING, stockCount, discount));
            shopData.Items.Add(CreateJunimoSeedItem(yellowObjects, QualifiedItemIds.APPLE_SAPLING, stockCount, discount));
        }
    }
}