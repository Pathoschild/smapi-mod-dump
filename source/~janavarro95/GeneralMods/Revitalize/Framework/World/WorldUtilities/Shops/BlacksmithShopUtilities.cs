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
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.CraftingIds;
using Omegasis.Revitalize.Framework.Constants.CraftingIds.RecipeIds;
using Omegasis.Revitalize.Framework.Constants.Ids.Items.BlueprintIds;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.World.Objects;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    public static class BlacksmithShopUtilities
    {

        /// <summary>
        /// Adds in ore to clint's shop.
        /// </summary>
        public static void AddStockToClintsShop(ShopMenu Menu)
        {
            //ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Ores.TinOre, 1), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.tinOrePrice, -1);
            //ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Ores.BauxiteOre, 1), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.bauxiteOreSellPrice, -1);
            //ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Ores.LeadOre, 1), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.leadOrePrice, -1);
            //ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Ores.SilverOre, 1), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.silverOrePrice, -1);
            //ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Ores.TitaniumOre, 1), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.titaniumOreSellPrice, -1);

            ObjectManager objectManager = RevitalizeModCore.ModContentManager.objectManager;

            BlacksmithShopConfig shopConfig = RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig;

            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AnvilRecipeId, objectManager.getItem(WorkbenchBlueprintIds.AnvilBlueprint), shopConfig.anvilBlueprintsPrice, 1);
            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.CoalMiningDrillRecipeId, objectManager.getItem(WorkbenchBlueprintIds.CoalMiningDrillBlueprint), shopConfig.coalMiningDrillBlueprintPrice, 1);
            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.ElectricMiningDrillRecipeId, objectManager.getItem(WorkbenchBlueprintIds.ElectricMiningDrillBlueprint), shopConfig.electricMiningDrillBlueprintPrice, 1);
            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AdvancedGeodeCrusher, objectManager.getItem(WorkbenchBlueprintIds.CoalAdvancedGeodeCrusherBlueprint), shopConfig.advancedGeodeCrusherBlueprintPrice, 1, Game1.stats.GeodesCracked >= 200);
            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.ElectricGeodeCrusher, objectManager.getItem(WorkbenchBlueprintIds.ElectricAdvancedGeodeCrusherBlueprint), shopConfig.electricGeodeCrusherBlueprintPrice, 1, Game1.stats.GeodesCracked >= 500);
            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.NuclearGeodeCrusher, objectManager.getItem(WorkbenchBlueprintIds.NuclearAdvancedGeodeCrusherBlueprint), shopConfig.nuclearGeodeCrusherBlueprintPrice, 1, Game1.stats.GeodesCracked >= 750 && PlayerUtilities.GetNumberOfGoldenWalnutsFound() >= 100);
            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.MagicalGeodeCrusher, objectManager.getItem(WorkbenchBlueprintIds.MagicalAdvancedGeodeCrusherBlueprint), shopConfig.magicalGeodeCrusherBlueprintPrice, 1, Game1.stats.GeodesCracked >= 1000 && PlayerUtilities.GetNumberOfGoldenWalnutsFound() >= 100);

            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AdvancedCharcoalKiln, objectManager.getItem(WorkbenchBlueprintIds.AdvancedCharcoalKiln, 1), shopConfig.advancedCharcoalKilnBlueprintPrice, 1, Game1.player.ForagingLevel >= 8);
            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.DeluxeCharcoalKiln, objectManager.getItem(WorkbenchBlueprintIds.DeluxeCharcoalKiln, 1), shopConfig.deluxCharcoalKilnBlueprintPrice, 1, Game1.player.ForagingLevel >= 8 && PlayerUtilities.GetNumberOfGoldenWalnutsFound()>=1);
            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.SuperiorCharcoalKiln, objectManager.getItem(WorkbenchBlueprintIds.SuperiorCharcoalKiln, 1), shopConfig.superiorCharcoalKilnBlueprintPrice, 1, Game1.player.ForagingLevel >= 8 && PlayerUtilities.GetNumberOfGoldenWalnutsFound()>=100);

            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.BurnerGenerator, objectManager.getItem(WorkbenchBlueprintIds.BurnerGenerator), shopConfig.burnerBatteryGeneratorBlueprintPrice);
            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AdvancedGenerator, objectManager.getItem(WorkbenchBlueprintIds.AdvancedGenerator), shopConfig.advancedBatteryGeneratorBlueprintPrice, 1, PlayerUtilities.HasObtainedItem(Enums.SDVObject.SolarEssence));
            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.NuclearGenerator, objectManager.getItem(WorkbenchBlueprintIds.NuclearGenerator), shopConfig.nuclearBatteryGeneratorBlueprintPrice, 1, PlayerUtilities.GetNumberOfGoldenWalnutsFound() >= 100);

            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.ItemVault, objectManager.getItem(WorkbenchBlueprintIds.ItemVault), shopConfig.itemVaultBlueprintPrice, 1);
            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.BigItemVault, objectManager.getItem(WorkbenchBlueprintIds.BigItemVault), shopConfig.bigItemVaultBlueprintPrice, 1, PlayerUtilities.KnowsCraftingRecipe(CraftingRecipeBooks.WorkbenchCraftingRecipies,WorkbenchRecipeIds.ItemVault));
            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.LargeItemVault, objectManager.getItem(WorkbenchBlueprintIds.LargeItemVault), shopConfig.largeItemVaultBlueprintPrice, 1, PlayerUtilities.KnowsCraftingRecipe(CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.BigItemVault));
            ShopUtilities.AddToShopIfCraftingRecipeNotKnown(Menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.HugeItemVault, objectManager.getItem(WorkbenchBlueprintIds.HugeItemVault), shopConfig.hugeItemVaultBlueprintPrice, 1, PlayerUtilities.KnowsCraftingRecipe(CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.LargeItemVault));

            AddAxeBlueprintsToShop(Menu);
            AddHoeBlueprintsToShop(Menu);
            AddPickaxeBlueprintsToShop(Menu);
            AddWateringCanBlueprintsToShop(Menu);
        }

        /// <summary>
        /// Adds various tool
        /// </summary>
        /// <param name="Menu"></param>
        private static void AddAxeBlueprintsToShop(ShopMenu Menu)
        {

            int toolLevel = PlayerUtilities.GetToolLevel<Axe>();

            if (toolLevel >= Tool.iridium && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.IridiumAxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.IridiumAxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.iridiumAxeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.gold && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.GoldAxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.GoldAxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.goldAxeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.steel && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.SteelAxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.SteelAxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.steelAxeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.copper && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.CopperAxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.CopperAxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.copperAxeBlueprintPrice, 1);
            }
            if (!RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.Axe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.AxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.axeBlueprintPrice, 1);
            }

        }

        private static void AddHoeBlueprintsToShop(ShopMenu Menu)
        {

            int toolLevel = PlayerUtilities.GetToolLevel<Hoe>();

            if (toolLevel >= Tool.iridium && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.IridiumHoe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.IridiumHoeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.iridiumHoeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.gold && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.GoldHoe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.GoldHoeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.goldHoeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.steel && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.SteelHoe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.SteelHoeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.steelHoeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.copper && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.CopperHoe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.CopperHoeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.copperHoeBlueprintPrice, 1);
            }
            if (!RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.Hoe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.HoeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.hoeBlueprintPrice, 1);
            }

        }


        private static void AddPickaxeBlueprintsToShop(ShopMenu Menu)
        {

            int toolLevel = PlayerUtilities.GetToolLevel<Pickaxe>();

            if (toolLevel >= Tool.iridium && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.IridiumPickaxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.IridiumPickaxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.iridiumPickaxeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.gold && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.GoldPickaxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.GoldPickaxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.goldPickaxeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.steel && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.SteelPickaxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.SteelPickaxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.steelPickaxeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.copper && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.CopperPickaxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.CopperPickaxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.copperPickaxeBlueprintPrice, 1);
            }
            if (!RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.Pickaxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.PickaxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.pickaxeBlueprintPrice, 1);
            }

        }


        private static void AddWateringCanBlueprintsToShop(ShopMenu Menu)
        {

            int toolLevel = PlayerUtilities.GetToolLevel<WateringCan>();

            if (toolLevel >= Tool.iridium && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.IridiumWateringCan"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.IridiumWateringCanBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.iridiumWateringCanBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.gold && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.GoldWateringCan"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.GoldWateringCanBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.goldWateringCanBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.steel && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.SteelWateringCan"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.SteelWateringCanBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.steelWateringCanBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.copper && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.CopperWateringCan"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.CopperWateringCanBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.copperWateringCanBlueprintPrice, 1);
            }
            if (!RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.WateringCan"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.WateringCanBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.wateringCanBlueprintPrice, 1);
            }

        }


    }
}
