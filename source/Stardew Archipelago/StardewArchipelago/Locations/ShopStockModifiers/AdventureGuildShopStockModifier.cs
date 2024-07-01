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
using StardewArchipelago.Constants;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Shops;

namespace StardewArchipelago.Locations.ShopStockModifiers
{
    public class AdventureGuildShopStockModifier : ShopStockModifier
    {
        public AdventureGuildShopStockModifier(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, StardewItemManager stardewItemManager) : base(monitor, modHelper, archipelago, stardewItemManager)
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
                    var adventureShop = shopsData["AdventureShop"];
                    var adventureRecovery = shopsData["AdventureGuildRecovery"];
                    AddAllWeaponsWithReceivedConditions(adventureShop);
                    AddRandomWeaponRecoveries(adventureRecovery);
                },
                AssetEditPriority.Late
            );
        }

        private void AddAllWeaponsWithReceivedConditions(ShopData adventureShop)
        {
            for (var i = adventureShop.Items.Count - 1; i >= 0; i--)
            {
                var item = adventureShop.Items[i];
                if (!item.Id.StartsWith("(W)", StringComparison.InvariantCultureIgnoreCase) && !item.Id.StartsWith("(B)", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                adventureShop.Items.RemoveAt(i);
            }

            var shopEquipments = new ShopItemData()
            {
                Id = IDProvider.ARCHIPELAGO_EQUIPMENTS,
                ItemId = $"{IDProvider.ARCHIPELAGO_EQUIPMENTS} {IDProvider.ARCHIPELAGO_EQUIPMENTS_SALE}",
                AvoidRepeat = true,
            };
            adventureShop.Items.Add(shopEquipments);
        }

        private void AddRandomWeaponRecoveries(ShopData adventureRecovery)
        {
            var shopEquipmentRecoveries = new ShopItemData()
            {
                Id = IDProvider.ARCHIPELAGO_EQUIPMENTS,
                ItemId = $"{IDProvider.ARCHIPELAGO_EQUIPMENTS} {IDProvider.ARCHIPELAGO_EQUIPMENTS_RECOVERY}",
                AvoidRepeat = true,
            };
            adventureRecovery.Items.Add(shopEquipmentRecoveries);
        }
    }
}
