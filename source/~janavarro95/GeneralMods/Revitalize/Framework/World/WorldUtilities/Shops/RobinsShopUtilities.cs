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
using Omegasis.Revitalize.Framework.Configs.ShopConfigs;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.CraftingIds;
using Omegasis.Revitalize.Framework.Constants.CraftingIds.RecipeIds;
using Omegasis.Revitalize.Framework.Constants.Ids.Items.BlueprintIds;
using Omegasis.Revitalize.Framework.Constants.Ids.Objects;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using StardewValley;
using StardewValley.Menus;
using static Omegasis.Revitalize.Framework.World.WorldUtilities.Shops.ShopUtilities;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    public static class RobinsShopUtilities
    {


        /// <summary>
        /// Keeps track of the number of hardwood pieces to sell in Robin's shop for a given day.
        /// </summary>
        public static int RobinsShop_NumberOfHardwoodToSellToday;
        /// <summary>
        /// What happens when something is bought from Robin's shop by default.
        /// </summary>
        public static Func<ISalable, Farmer, int, bool> RobinsShop_DefaultOnPurchaseMethod;

        /// <summary>
        /// What happens if 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnNewDay(object sender, StardewModdingAPI.Events.DayStartedEventArgs args)
        {
            RobinsShop_NumberOfHardwoodToSellToday = RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.SellsInfiniteHardWood ? -1 : Game1.random.Next(RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.HardwoodMinStockAmount, RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.HardwoodMaxStockAmount + 1);
        }


        /// <summary>
        /// Adds additional items to be sold by Robin.
        /// </summary>
        /// <param name="Menu"></param>
        public static void AddItemsToRobinsShop(ShopMenu Menu)
        {
            RobinsShop_DefaultOnPurchaseMethod = Menu.onPurchase;
            Menu.onPurchase = OnPurchaseFromRobinsShop;

            List<ShopInventoryProbe> shopInventoryProbes = new List<ShopInventoryProbe>();

            shopInventoryProbes.Add(AddItemToStockAfterStone(Enums.SDVObject.Clay, Game1.year > 1 ? RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.ClaySellPriceYear2AndBeyond : RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.ClaySellPrice));
            if (PlayerUtilities.HasCompletedHardwoodDonationSpecialOrderForRobin())
            {
                shopInventoryProbes.Add(AddItemToStockAfterStone(Enums.SDVObject.Hardwood, RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.HardwoodSellPrice, RobinsShop_NumberOfHardwoodToSellToday));
            }
            shopInventoryProbes.Add(AddItemToStockAfterStardewValleyWorkbench(CraftingStations.WorkBench_Id, RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.WorkStationSellPrice));

            //Adds in blueprints to Robin's Shop.
            shopInventoryProbes.AddRange(AddBlueprintsToShop());


            ShopUtilities.UpdateShopStockAndPriceInSortedOrder(Menu, shopInventoryProbes);

        }

        /// <summary>
        /// Returns a list of all the blueprints to be sold by Robin.
        /// </summary>
        /// <returns></returns>
        private static List<ShopInventoryProbe> AddBlueprintsToShop()
        {
            List<ShopInventoryProbe> blueprintsToAdd = new List<ShopInventoryProbe>();
            RobinsShopConfig shopConfig = RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig;

            if (PlayerUtilities.HasObtainedItem(Enums.SDVObject.BatteryPack))
            {
                blueprintsToAdd.Add(AddBlueprintToShop(CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.ElectricFurnaceRecipeId, WorkbenchBlueprintIds.ElectricFurnaceBlueprint, shopConfig.ElectricFurnaceBlueprintPrice));
            }

            if (Game1.player.FarmingLevel >= 10)
            {
                blueprintsToAdd.Add(AddBlueprintToShop(CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.IrrigatedWateringPotRecipeId, WorkbenchBlueprintIds.IrrigatedGardenPotBlueprint, shopConfig.IrrigatedWateringPotBlueprintPrice));
                blueprintsToAdd.Add(AddBlueprintToShop(CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AutomatedFarmingSystemRecipeId, WorkbenchBlueprintIds.AutomaticFarmingSystemBlueprint, shopConfig.AutomaticFarmingSystemBlueprintPrice));
                blueprintsToAdd.Add(AddBlueprintToShop(CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AutoHarvesterIrrigatedWateringPotAttachmentRecipeId, WorkbenchBlueprintIds.AutoHarvesterGardenPotAttachmentBlueprint, shopConfig.AutoHarvesterIrrigatedGardenPotAttachmentBlueprintPrice));
                blueprintsToAdd.Add(AddBlueprintToShop(CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AutoPlanterIrrigatedWateringPotAttachmentRecipeId, WorkbenchBlueprintIds.AutoPlanterGardenPotBlueprint, shopConfig.AutoPlanterIrrigatedWateringPotAttachmentBlueprintPrice));
            }

            if (Game1.player.ForagingLevel >= 5)
            {
                blueprintsToAdd.Add(AddBlueprintToShop(CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.Windmill, WorkbenchBlueprintIds.Windmill, shopConfig.IrrigatedWateringPotBlueprintPrice));
            }

            //Filter out all blueprints that would already be known.
            blueprintsToAdd.RemoveAll(blueprint => blueprint == null);

            return blueprintsToAdd;

        }


        /// <summary>
        /// Creates a <see cref="ShopInventoryProbe"/> to sell a blueprint in Robin's shop as long as the Player does not already know the crafting recipe.
        /// </summary>
        /// <param name="CraftingBookId"></param>
        /// <param name="CraftingRecipeId"></param>
        /// <param name="BlueprintObjectId"></param>
        /// <param name="Price"></param>
        /// <returns></returns>
        private static ShopInventoryProbe AddBlueprintToShop(string CraftingBookId, string CraftingRecipeId, string BlueprintObjectId, int Price)
        {
            if (!RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingBookId, CraftingRecipeId))
            {
                return AddItemToStockAfterStone(BlueprintObjectId, Price, 1);
            }
            return null;

        }

        /// <summary>
        /// Creates a <see cref="ShopInventoryProbe"/> to add an item after stone in the shop's stock.
        /// </summary>
        /// <param name="ItemToAdd"></param>
        /// <param name="SellingPrice"></param>
        /// <param name="AmountToSell"></param>
        /// <returns></returns>
        private static ShopInventoryProbe AddItemToStockAfterStone(Enums.SDVObject ItemToAdd, int SellingPrice, int AmountToSell = -1)
        {
            return ShopUtilities.CreateInventoryShopProbe(new ItemFoundInShopInventory((itemForSale, FoundItemPrice, Stock) => itemForSale.GetType().Equals(typeof(StardewValley.Object)) && (itemForSale as StardewValley.Object).parentSheetIndex == (int)Enums.SDVObject.Stone), ItemToAdd, SellingPrice, AmountToSell);
        }
        /// <summary>
        /// Creates a <see cref="ShopInventoryProbe"/> to add an item after stone in the shop's stock.
        /// </summary>
        /// <param name="ItemToAdd"></param>
        /// <param name="SellingPrice"></param>
        /// <param name="AmountToSell"></param>
        /// <returns></returns>
        private static ShopInventoryProbe AddItemToStockAfterStone(string ItemToAdd, int SellingPrice, int AmountToSell = -1)
        {
            return ShopUtilities.CreateInventoryShopProbe(new ItemFoundInShopInventory((itemForSale, FoundItemPrice, Stock) => itemForSale.GetType().Equals(typeof(StardewValley.Object)) && (itemForSale as StardewValley.Object).parentSheetIndex == (int)Enums.SDVObject.Stone), ItemToAdd, SellingPrice, AmountToSell);
        }
        /// <summary>
        /// Creates a <see cref="ShopInventoryProbe"/> to add an item after stone in the shop's stock.
        /// </summary>
        /// <param name="ItemToAdd"></param>
        /// <param name="SellingPrice"></param>
        /// <param name="AmountToSell"></param>
        /// <returns></returns>
        private static ShopInventoryProbe AddItemToStockAfterStardewValleyWorkbench(string ItemToAdd, int SellingPrice, int AmountToSell = -1)
        {
            return ShopUtilities.CreateInventoryShopProbe(new ItemFoundInShopInventory((itemForSale, FoundItemPrice, Stock) => itemForSale.GetType().Equals(typeof(StardewValley.Object)) && (itemForSale as StardewValley.Object).parentSheetIndex == (int)Enums.SDVBigCraftable.Workbench && (itemForSale as StardewValley.Object).bigCraftable == true), ItemToAdd, SellingPrice, AmountToSell);
        }


        /// <summary>
        /// Called when purchasing an item from robins shop.
        /// </summary>
        /// <param name="purchasedItem"></param>
        /// <param name="who"></param>
        /// <param name="AmountPurchased"></param>
        /// <returns>A bool representing if the menu should be closed or not.</returns>
        private static bool OnPurchaseFromRobinsShop(ISalable purchasedItem, Farmer who, int AmountPurchased)
        {
            if (purchasedItem is StardewValley.Object)
            {
                StardewValley.Object itemForSale = (purchasedItem as StardewValley.Object);
                if (itemForSale.parentSheetIndex == (int)Enums.SDVObject.Hardwood)
                {
                    RobinsShop_NumberOfHardwoodToSellToday -= AmountPurchased;
                    return false;
                }
            }

            if (RobinsShop_DefaultOnPurchaseMethod != null)
            {
                return RobinsShop_DefaultOnPurchaseMethod.Invoke(purchasedItem, who, AmountPurchased);
            }
            return false;
        }

    }
}
