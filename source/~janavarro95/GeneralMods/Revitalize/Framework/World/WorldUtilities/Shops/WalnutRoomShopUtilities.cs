/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.Revitalize.Framework.Configs.ShopConfigs;
using Omegasis.Revitalize.Framework.Constants.CraftingIds;
using Omegasis.Revitalize.Framework.Constants.CraftingIds.RecipeIds;
using Omegasis.Revitalize.Framework.Constants.Ids.Items.BlueprintIds;
using Omegasis.Revitalize.Framework.Constants.Ids.Objects;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.World.Objects;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    public static class WalnutRoomShopUtilities
    {

        /// <summary>
        /// What happens if a new day starts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnNewDay(object sender, StardewModdingAPI.Events.DayStartedEventArgs args)
        {

        }

        public static void AddItemsToShop(ShopMenu Menu)
        {
            WalnutRoomShopConfig shopConfig = RevitalizeModCore.Configs.shopsConfigManager.walnutRoomShopConfig;
            ObjectManager objectManager = RevitalizeModCore.ModContentManager.objectManager;

            ShopUtilities.AddToWalnutShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.NuclearFurnaceRecipeId, objectManager.getItem(WorkbenchBlueprintIds.NuclearFurnaceBlueprint), shopConfig.NuclearFurnaceBlueprintQiGemPrice, 1);

            ShopUtilities.AddToWalnutShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.MagicalFurnaceRecipeId, objectManager.getItem(WorkbenchBlueprintIds.MagicalFurnaceBlueprint), shopConfig.MagicalFurnaceBlueprintQigemPrice, 1);
            ShopUtilities.AddToWalnutShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.RadioactiveFuelCellRecipeId, objectManager.getItem(WorkbenchBlueprintIds.RadioactiveFuelCellBlueprint), shopConfig.RadioactiveFuelBlueprintQiGemPrice, 1);

            ShopUtilities.AddToWalnutShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.NuclearMiningDrillRecipeId, objectManager.getItem(WorkbenchBlueprintIds.NuclearMiningDrillBlueprint), shopConfig.NuclearMiningDrillBlueprintPrice, 1, Game1.player.hasSkullKey);
            ShopUtilities.AddToWalnutShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.MagicalMiningDrillRecipeId, objectManager.getItem(WorkbenchBlueprintIds.MagicalMiningDrillBlueprint), shopConfig.MagicalMiningDrillBlueprintPrice, 1, Game1.player.hasSkullKey);

            ShopUtilities.AddToWalnutShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AdvancedSolarPanel, objectManager.getItem(WorkbenchBlueprintIds.AdvancedSolarPanel), shopConfig.AdvancedSolarPanelBlueprint, 1);
            ShopUtilities.AddToWalnutShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.SuperiorSolarPanel, objectManager.getItem(WorkbenchBlueprintIds.SuperiorSolarPanel), shopConfig.SuperiorSolarPanelBlueprint, 1);

            if (Game1.player.MiningLevel >= 10 && Game1.player.hasSkullKey)
            {
                ShopUtilities.AddItemToWalnutRoomShop(Menu, objectManager.getItem(ResourceObjectIds.CoalBush), shopConfig.CoalResourceBushQiGemPrice);
                ShopUtilities.AddItemToWalnutRoomShop(Menu, objectManager.getItem(ResourceObjectIds.CopperOreBush), shopConfig.CopperOreResourceBushQiGemPrice);
                ShopUtilities.AddItemToWalnutRoomShop(Menu, objectManager.getItem(ResourceObjectIds.IronOreBush), shopConfig.IronOreResourceBushQiGemPrice);
                ShopUtilities.AddItemToWalnutRoomShop(Menu, objectManager.getItem(ResourceObjectIds.GoldOreBush), shopConfig.GoldOreResourceBushQiGemPrice);
                ShopUtilities.AddItemToWalnutRoomShop(Menu, objectManager.getItem(ResourceObjectIds.IridiumOreBush), shopConfig.IridiumOreResoureceBushQiGemPrice);
                ShopUtilities.AddItemToWalnutRoomShop(Menu, objectManager.getItem(ResourceObjectIds.RadioactiveOreBush), shopConfig.RadioactiveOreResoureceBushQiGemPrice);
            }

            ShopUtilities.AddItemToWalnutRoomShop(Menu, objectManager.getItem(MiscObjectIds.StatueOfPerfectionTracking), shopConfig.StatueOfPerfectionTrackingPrice);


            ShopUtilities.AddToWalnutShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.DimensionalStorageChest, objectManager.getItem(StorageIds.DimensionalStorageBag), shopConfig.DimensionalStorageBagBlueprintsPrice);
            ShopUtilities.AddItemToWalnutRoomShop(Menu, objectManager.getItem(StorageIds.DimensionalStorageBag), shopConfig.DimensionalStorageBagBlueprintsPrice);
            ShopUtilities.AddToWalnutShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.DimensionalStorageBag ,objectManager.getItem(StorageIds.DimensionalStorageBag), shopConfig.DimensionalStorageBagBlueprintsPrice);

            ShopUtilities.AddToWalnutShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AbyssCask, objectManager.getItem(WorkbenchBlueprintIds.AbyssCask), shopConfig.AbyssCaskBlueprintPrice, 1);
        }

    }
}
