/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using StardewValley.Tools;
using ToolUpgradeCosts.Framework;

namespace ToolUpgradeCosts
{
    public class ModEntry : Mod
    {
        private static ModEntry _instance;

        private readonly Dictionary<UpgradeMaterials, string> _defaultMaterials = new()
        {
            {UpgradeMaterials.Copper, "334"},
            {UpgradeMaterials.Steel, "335"},
            {UpgradeMaterials.Gold, "336"},
            {UpgradeMaterials.Iridium, "337"}
        };

        private Config _config;

        public override void Entry(IModHelper helper)
        {
            _instance = this;
            this._config = helper.ReadConfig<Config>();
            this.Helper.Events.GameLoop.SaveLoaded += this.GetIndexes;

            Harmony harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                AccessTools.Method(typeof(ShopBuilder), nameof(ShopBuilder.GetShopStock), new[] { typeof(string), typeof(ShopData) }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ShopBuilder_GetShopStock_Postfix))
            );
        }

        private void GetIndexes(object sender, SaveLoadedEventArgs e)
        {
            foreach (KeyValuePair<UpgradeMaterials, Upgrade> upgrade in this._config.UpgradeCosts)
            {
                string name = upgrade.Value.MaterialName;

                string id = Game1.objectData.FirstOrDefault(kvp => kvp.Value.Name == name).Key;
                if (id is null)
                {
                    this.Monitor.Log($"Object named \"{name}\" not found for the tool upgrade level of {upgrade.Key}. Vanilla upgrade item will be used", LogLevel.Error);
                    id = this._defaultMaterials[upgrade.Key];
                }
                upgrade.Value.MaterialId = id;
            }
        }

        public static void ShopBuilder_GetShopStock_Postfix(string shopId, ref Dictionary<ISalable, ItemStockInformation> __result)
        {
            if (shopId != Game1.shop_blacksmithUpgrades)
                return;

            try
            {
                Dictionary<ISalable, ItemStockInformation> editedStock = new Dictionary<ISalable, ItemStockInformation>();
                foreach ((ISalable item, ItemStockInformation stockInfo) in __result)
                {
                    if (item is Tool tool && Enum.IsDefined(typeof(UpgradeMaterials), tool.UpgradeLevel))
                    {
                        UpgradeMaterials upgradeLevel = (UpgradeMaterials)tool.UpgradeLevel;
                        if (tool is GenericTool)
                        {
                            upgradeLevel++;
                        }
                        editedStock[tool] = stockInfo with
                        {
                            Price = _instance._config.UpgradeCosts[upgradeLevel].Cost,
                            TradeItem = _instance._config.UpgradeCosts[upgradeLevel].MaterialId,
                            TradeItemCount = _instance._config.UpgradeCosts[upgradeLevel].MaterialStack
                        };
                    }
                    else
                    {
                        editedStock[item] = stockInfo;
                    }
                }
                __result = editedStock;
            }
            catch (Exception ex)
            {
                _instance.Monitor.Log($"Failed in {nameof(ShopBuilder_GetShopStock_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
