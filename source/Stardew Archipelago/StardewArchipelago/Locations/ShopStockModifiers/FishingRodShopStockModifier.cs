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
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.GameData.Tools;

namespace StardewArchipelago.Locations.ShopStockModifiers
{
    public class FishingRodShopStockModifier : ShopStockModifier
    {

        public FishingRodShopStockModifier(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, StardewItemManager stardewItemManager) : base(monitor, helper, archipelago, stardewItemManager)
        {
        }

        public override void OnShopStockRequested(object sender, AssetRequestedEventArgs e)
        {
            if (!_archipelago.SlotData.ToolProgression.HasFlag(ToolProgression.Progressive))
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
                    var willyShopData = shopsData["FishShop"];
                    ReplaceFishingRodsWithToolsChecks(willyShopData);
                },
                AssetEditPriority.Late
            );
        }

        private void ReplaceFishingRodsWithToolsChecks(ShopData fishShopData)
        {
            var toolsData = DataLoader.Tools(Game1.content);
            for (var i = fishShopData.Items.Count - 1; i >= 0; i--)
            {
                var item = fishShopData.Items[i];
                var unqualifiedId = QualifiedItemIds.UnqualifyId(item.ItemId);
                if (unqualifiedId == null || !toolsData.ContainsKey(unqualifiedId))
                {
                    continue;
                }

                var toolData = toolsData[unqualifiedId];
                CreateEquivalentArchipelagoLocation(fishShopData, item, toolData);
                ReplaceWithArchipelagoCondition(item, toolData);
            }
        }

        private void CreateEquivalentArchipelagoLocation(ShopData fishShopData, ShopItemData item, ToolData toolData)
        {
            var location = $"Purchase {toolData.Name}";
            if (_archipelago.LocationExists(location))
            {
                fishShopData.Items.Insert(fishShopData.Items.IndexOf(item), CreateArchipelagoLocation(item, location));
            }
        }

        private void ReplaceWithArchipelagoCondition(ShopItemData shopItem, ToolData toolData)
        {
            var amount = toolData.UpgradeLevel + 1;
            // For some reason, the training rod is 1 and the bamboo pole is 0
            if (toolData.UpgradeLevel <= 1)
            {
                amount = 2 - toolData.UpgradeLevel;
            }

            var toolName = toolData.Name;
            if (toolData.ClassName == "FishingRod")
            {
                toolName = "Fishing Rod";
            }

            ReplaceWithArchipelagoCondition(shopItem, $"Progressive {toolName}", amount);
        }
    }
}
