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
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Shops;

namespace StardewArchipelago.Locations.ShopStockModifiers
{
    public class KrobusStockModifier : ShopStockModifier
    {
        public KrobusStockModifier(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, StardewItemManager stardewItemManager) : base(monitor, helper, archipelago, stardewItemManager)
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
                    var krobusShop = shopsData["ShadowShop"];
                    ReplaceStardropWithCheck(krobusShop);
                },
                AssetEditPriority.Late
            );
        }

        private void ReplaceStardropWithCheck(ShopData shopData)
        {
            const string krobusStardropLocationName = "Krobus Stardrop";
            var itemsData = DataLoader.Objects(Game1.content);
            for (var i = shopData.Items.Count - 1; i >= 0; i--)
            {
                var item = shopData.Items[i];
                if (!item.Id.Equals(QualifiedItemIds.STARDROP, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var apShopItem = CreateArchipelagoLocation(item, krobusStardropLocationName);
                shopData.Items.RemoveAt(i);
                shopData.Items.Insert(i, apShopItem);
            }
        }
    }
}
