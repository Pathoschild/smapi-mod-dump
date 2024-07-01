/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Locations;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Shops;

namespace StardewArchipelago.Locations.ShopStockModifiers
{
    public class CookingRecipePurchaseStockModifier : ShopStockModifier
    {
        public CookingRecipePurchaseStockModifier(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, StardewItemManager stardewItemManager) : base(monitor, helper, archipelago, stardewItemManager)
        {
        }

        public override void OnShopStockRequested(object sender, AssetRequestedEventArgs e)
        {
            if (!_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Purchases))
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
                        ReplaceCookingRecipesWithChefsanityChecks(shopId, shopData);
                    }
                },
                AssetEditPriority.Late
            );
        }

        private void ReplaceCookingRecipesWithChefsanityChecks(string shopId, ShopData shopData)
        {
            for (var i = shopData.Items.Count - 1; i >= 0; i--)
            {
                var item = shopData.Items[i];

                if (!item.IsRecipe)
                {
                    continue;
                }

                var stardewItem = _stardewItemManager.GetItemByQualifiedId(item.ItemId) ?? _stardewItemManager.GetItemByQualifiedId(QualifiedItemIds.QualifiedObjectId(item.ItemId));
                var location = $"{stardewItem.Name}{Suffix.CHEFSANITY}";
                if (!CraftingRecipe.cookingRecipes.ContainsKey(stardewItem.Name) || !_archipelago.LocationExists(location))
                {
                    continue;
                }

                var apShopItem = CreateArchipelagoLocation(item, location);
                shopData.Items.RemoveAt(i);
                shopData.Items.Insert(i, apShopItem);
            }
        }
    }
}
