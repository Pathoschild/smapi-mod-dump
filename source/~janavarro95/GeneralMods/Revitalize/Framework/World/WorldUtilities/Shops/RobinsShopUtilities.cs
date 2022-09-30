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
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.CraftingIds;
using Omegasis.Revitalize.Framework.Constants.CraftingIds.RecipeIds;
using Omegasis.Revitalize.Framework.Constants.ItemIds.Items.BlueprintIds;
using Omegasis.Revitalize.Framework.Constants.ItemIds.Objects;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.World.Structures;
using StardewModdingAPI;
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
        public static Func<ISalable, Farmer, int, bool> RobinsShop_DefaultOnPurchaseMethod;

        public static void OnNewDay(object sender, StardewModdingAPI.Events.DayStartedEventArgs args)
        {
            RobinsShop_NumberOfHardwoodToSellToday = Game1.random.Next(RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.HardwoodMinStockAmount, RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.HardwoodMaxStockAmount + 1);
        }



        public static void AddItemsToRobinsShop(ShopMenu Menu)
        {
            RobinsShop_DefaultOnPurchaseMethod = Menu.onPurchase;
            Menu.onPurchase = OnPurchaseFromRobinsShop;

            List<ShopInventoryProbe> shopInventoryProbes = new List<ShopInventoryProbe>()
            {

                new ShopInventoryProbe(
                    new ItemFoundInShopInventory((itemForSale, Price,Stock)=> itemForSale.GetType().Equals(typeof(StardewValley.Object)) && (itemForSale as StardewValley.Object).parentSheetIndex== (int)Enums.SDVObject.Stone),
                    new UpdateShopInventory((ShopInventory,ItemForSale,Price,Stock)=>{
                        Item clay = RevitalizeModCore.ModContentManager.objectManager.getItem(Enums.SDVObject.Clay, -1);
                        ShopInventory.addItemForSale(clay,Game1.year>1? RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.ClaySellPriceYear2AndBeyond: RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.ClaySellPrice, -1);
                        return ShopInventory ;
                }
                )),

                new ShopInventoryProbe(
                    new ItemFoundInShopInventory((itemForSale, Price,Stock)=> itemForSale.GetType().Equals(typeof(StardewValley.Object)) && (itemForSale as StardewValley.Object).parentSheetIndex== (int)Enums.SDVObject.Stone && PlayerUtilities.HasCompletedHardwoodDonationSpecialOrderForRobin()),
                    new UpdateShopInventory((ShopInventory,ItemForSale,Price,Stock)=>{
                        StardewValley.Item hardwood = RevitalizeModCore.ModContentManager.objectManager.getItem(Enums.SDVObject.Hardwood, 1);
                        if (RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.SellsInfiniteHardWood)
                        {
                            ShopInventory.addItemForSale(hardwood,RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.HardwoodSellPrice, -1);
                        }
                        else
                        {
                            hardwood.Stack = RobinsShop_NumberOfHardwoodToSellToday;
                            ShopInventory.addItemForSale(hardwood,RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.HardwoodSellPrice, RobinsShop_NumberOfHardwoodToSellToday);
                        }
                        return ShopInventory;
                }
                )),

                 new ShopInventoryProbe(
                    new ItemFoundInShopInventory((itemForSale, Price,Stock)=>itemForSale.GetType().Equals(typeof(StardewValley.Object)) && (itemForSale as StardewValley.Object).parentSheetIndex==(int)Enums.SDVBigCraftable.Workbench && (itemForSale as StardewValley.Object).bigCraftable == true),
                    new UpdateShopInventory((ShopInventory,ItemForSale,Price,Stock)=>{
                        Item workbench = RevitalizeModCore.ModContentManager.objectManager.getItem(CraftingStations.WorkStation_Id, 1);
                        ShopInventory.addItemForSale(workbench,RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.WorkStationSellPrice, -1);
                        return ShopInventory;
                }
                )),

            };

            //Add in blueprits below.

            if (!RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.ElectricFurnaceRecipeId))
            {
                shopInventoryProbes.Add(
                  new ShopInventoryProbe(
                    new ItemFoundInShopInventory((itemForSale, Price, Stock) => itemForSale.GetType().Equals(typeof(StardewValley.Object)) && (itemForSale as StardewValley.Object).parentSheetIndex == (int)Enums.SDVObject.Stone),
                    new UpdateShopInventory((ShopInventory, ItemForSale, Price, Stock) =>
                    {
                        Item electricFurnaceBlueprint = RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.Workbench_ElectricFurnaceCraftingRecipeBlueprint);
                        ShopInventory.addItemForSale(electricFurnaceBlueprint, RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.ElectricFurnaceBlueprintPrice, 1);
                        return ShopInventory;
                    }
                )));
            }

            if (!RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.IrrigatedWateringPotRecipeId) && Game1.player.FarmingLevel >= 10 && Game1.netWorldState.Value.GoldenWalnutsFound.Value >= 1)
            {
                shopInventoryProbes.Add(
                  new ShopInventoryProbe(
                    new ItemFoundInShopInventory((itemForSale, Price, Stock) => itemForSale.GetType().Equals(typeof(StardewValley.Object)) && (itemForSale as StardewValley.Object).parentSheetIndex == (int)Enums.SDVObject.Stone),
                    new UpdateShopInventory((ShopInventory, ItemForSale, Price, Stock) =>
                    {
                        Item irrigatedWateringPotBlueprints = RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.Workbench_IrrigatedGardenPotCraftingRecipeBlueprint);
                        ShopInventory.addItemForSale(irrigatedWateringPotBlueprints, RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.IrrigatedWateringPotBlueprintPrice, 1);
                        return ShopInventory;
                    }
                )));
            }

            if (!RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AutoPlanterIrrigatedWateringPotAttachmentRecipeId) && Game1.player.FarmingLevel >= 10 && Game1.netWorldState.Value.GoldenWalnutsFound.Value >= 1)
            {
                shopInventoryProbes.Add(
                  new ShopInventoryProbe(
                    new ItemFoundInShopInventory((itemForSale, Price, Stock) => itemForSale.GetType().Equals(typeof(StardewValley.Object)) && (itemForSale as StardewValley.Object).parentSheetIndex == (int)Enums.SDVObject.Stone),
                    new UpdateShopInventory((ShopInventory, ItemForSale, Price, Stock) =>
                    {
                        Item autoPlanterBlueprints = RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.Workbench_AutoPlanterGardenPotCraftingRecipeBlueprint);
                        ShopInventory.addItemForSale(autoPlanterBlueprints, RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.AutoPlanterIrrigatedWateringPotAttachmentBlueprintPrice, 1);
                        return ShopInventory;
                    }
                )));
            }

            if (!RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AutoHarvesterIrrigatedWateringPotAttachmentRecipeId) && Game1.player.FarmingLevel >= 10 && Game1.netWorldState.Value.GoldenWalnutsFound.Value >= 1)
            {

                shopInventoryProbes.Add(
                  new ShopInventoryProbe(
                    new ItemFoundInShopInventory((itemForSale, Price, Stock) => itemForSale.GetType().Equals(typeof(StardewValley.Object)) && (itemForSale as StardewValley.Object).parentSheetIndex == (int)Enums.SDVObject.Stone),
                    new UpdateShopInventory((ShopInventory, ItemForSale, Price, Stock) =>
                    {
                        Item autoHarvesterBlueprints = RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.Workbench_AutoHarvesterGardenPotAttachmentCraftingRecipeBlueprint);
                        ShopInventory.addItemForSale(autoHarvesterBlueprints, RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.AutoHarvesterIrrigatedGardenPotAttachmentBlueprintPrice, 1);
                        return ShopInventory;
                    }
                )));
            }

            if (!RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AutomatedFarmingSystemRecipeId) && Game1.player.FarmingLevel >= 10 && Game1.netWorldState.Value.GoldenWalnutsFound.Value >= 1)
            {
                shopInventoryProbes.Add(
                  new ShopInventoryProbe(
                    new ItemFoundInShopInventory((itemForSale, Price, Stock) => itemForSale.GetType().Equals(typeof(StardewValley.Object)) && (itemForSale as StardewValley.Object).parentSheetIndex == (int)Enums.SDVObject.Stone),
                    new UpdateShopInventory((ShopInventory, ItemForSale, Price, Stock) =>
                    {
                        Item automaticFarmingSystemBlueprints = RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.Workbench_AutomaticFarmingSystemCraftingRecipeBlueprint);
                        ShopInventory.addItemForSale(automaticFarmingSystemBlueprints, RevitalizeModCore.Configs.shopsConfigManager.robinsShopConfig.AutomaticFarmingSystemBlueprintPrice, 1);
                        return ShopInventory;
                    }
                )));
            }

            ShopUtilities.UpdateShopStockAndPriceInSortedOrder(Menu, shopInventoryProbes);

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
