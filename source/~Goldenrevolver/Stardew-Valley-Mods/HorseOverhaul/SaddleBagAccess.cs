/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using StardewModdingAPI.Events;
    using StardewValley.Characters;
    using StardewValley.GameData.Objects;
    using StardewValley.GameData.Powers;
    using StardewValley.GameData.Shops;
    using System.Collections.Generic;
    using static StardewValley.GameData.QuantityModifier;
    using StardewObject = StardewValley.Object;

    internal class SaddleBagAccess
    {
        private static HorseOverhaulConfig config;
        private const string horseBookNonQID = "Book_Horse";

        public static bool HasAccessToSaddleBag(Horse horse)
        {
            if (config == null || !config.SaddleBag)
            {
                return false;
            }

            if (config.SaddleBagUnlockCondition != SaddleBagUnlockConditionOption.Buy_From_Animal_Shop.ToString())
            {
                return true;
            }

            return horse.getOwner() != null && horse.getOwner().stats.Get(HorseOverhaul.saddleBagBookNonQID) != 0;
        }

        internal static void ApplySaddleBagUnlockChanges(AssetRequestedEventArgs e, HorseOverhaul mod)
        {
            config = mod.Config;

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, ObjectData> data = asset.AsDictionary<string, ObjectData>().Data;

                    if (!data.TryGetValue(horseBookNonQID, out var horseBookData))
                    {
                        return;
                    }

                    var saddleBagData = new ObjectData
                    {
                        Name = HorseOverhaul.saddleBagBookNonQID,
                        DisplayName = mod.Helper.Translation.Get("BookSaddleBagName"),
                        Description = mod.Helper.Translation.Get("BookSaddleBagDescription"),
                        Type = horseBookData.Type,
                        Category = StardewObject.skillBooksCategory,
                        Price = 500,
                        Texture = horseBookData.Texture,
                        SpriteIndex = horseBookData.SpriteIndex,
                        Edibility = horseBookData.Edibility,
                        IsDrink = false,
                        Buffs = null,
                        GeodeDropsDefaultItems = false,
                        GeodeDrops = null,
                        ArtifactSpotChances = null,
                        ExcludeFromFishingCollection = true,
                        ExcludeFromShippingCollection = true,
                        ExcludeFromRandomSale = false,
                        ContextTags = new List<string>() { "color_brown", "book_item" },
                        CustomFields = null
                    };

                    data[HorseOverhaul.saddleBagBookNonQID] = saddleBagData;
                }, AssetEditPriority.Late);
            }

            if (mod.Config.SaddleBagUnlockCondition != SaddleBagUnlockConditionOption.Buy_From_Animal_Shop.ToString())
            {
                return;
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, ShopData> data = asset.AsDictionary<string, ShopData>().Data;

                    if (!data.TryGetValue("AnimalShop", out var animalShopData))
                    {
                        return;
                    }

                    var saddleBagShopData = new ShopItemData
                    {
                        ActionsOnPurchase = null,
                        CustomFields = null,
                        TradeItemId = null,
                        TradeItemAmount = 1,
                        Price = mod.Config.SaddleBagUnlockPrice,
                        ApplyProfitMargins = null,
                        AvailableStock = -1,
                        AvailableStockLimit = LimitedStockMode.Global,
                        AvoidRepeat = false,
                        UseObjectDataPrice = false,
                        IgnoreShopPriceModifiers = false,
                        PriceModifiers = null,
                        PriceModifierMode = QuantityModifierMode.Stack,
                        AvailableStockModifiers = null,
                        AvailableStockModifierMode = QuantityModifierMode.Stack,
                        Condition = null,
                        Id = HorseOverhaul.saddleBagBookQID,
                        ItemId = HorseOverhaul.saddleBagBookQID,
                        RandomItemId = null,
                        MaxItems = null,
                        MinStack = -1,
                        MaxStack = -1,
                        Quality = -1,
                        ObjectInternalName = null,
                        ObjectDisplayName = null,
                        ToolUpgradeLevel = -1,
                        IsRecipe = false,
                        StackModifiers = null,
                        StackModifierMode = QuantityModifierMode.Stack,
                        QualityModifiers = null,
                        QualityModifierMode = QuantityModifierMode.Stack,
                        ModData = null,
                        PerItemCondition = null
                    };

                    animalShopData.Items.Add(saddleBagShopData);
                }, AssetEditPriority.Late);
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Powers"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, PowersData> data = asset.AsDictionary<string, PowersData>().Data;

                    if (!data.TryGetValue(horseBookNonQID, out var horseBookData))
                    {
                        return;
                    }

                    var saddleBagPowerData = new PowersData
                    {
                        DisplayName = mod.Helper.Translation.Get("BookSaddleBagName"),
                        Description = mod.Helper.Translation.Get("BookSaddleBagDescription"),
                        TexturePath = horseBookData.TexturePath,
                        TexturePosition = horseBookData.TexturePosition,
                        UnlockedCondition = $"PLAYER_STAT Current {HorseOverhaul.saddleBagBookNonQID} 1",
                        CustomFields = null
                    };

                    data[HorseOverhaul.saddleBagBookNonQID] = saddleBagPowerData;
                }, AssetEditPriority.Late);
            }
        }
    }
}