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
using StardewArchipelago.Constants;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Locations.ShopStockModifiers;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Shops;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public class AnimalShopStockModifier : ShopStockModifier
    {
        public AnimalShopStockModifier(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, StardewItemManager stardewItemManager) : base(monitor, helper, archipelago, stardewItemManager)
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
                    var marnieShopData = shopsData["AnimalShop"];
                    foreach (var shopItemData in marnieShopData.Items)
                    {
                        if (shopItemData.Id != QualifiedItemIds.GOLDEN_EGG)
                        {
                            continue;
                        }

                        shopItemData.Condition = GameStateConditionProvider.CreateHasReceivedItemCondition("Golden Egg");
                    }
                },
                AssetEditPriority.Late
            );
        }
    }
}
