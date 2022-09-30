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
using Omegasis.Revitalize.Framework.Constants.CraftingIds;
using Omegasis.Revitalize.Framework.Constants.ItemIds.Items.BlueprintIds;
using Omegasis.Revitalize.Framework.Constants.ItemIds.Resources.EarthenResources;
using Omegasis.Revitalize.Framework.Player;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    public static class ClintsShopUtilities
    {

        /// <summary>
        /// Adds in ore to clint's shop.
        /// </summary>
        public static void AddStockToClintsShop(ShopMenu Menu)
        {
            ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Ores.TinOre, 1), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.tinOrePrice, -1);
            //ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Ores.BauxiteOre, 1), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.bauxiteOreSellPrice, -1);
            ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Ores.LeadOre, 1), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.leadOrePrice, -1);
            ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Ores.SilverOre, 1), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.silverOrePrice, -1);
            //ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Ores.TitaniumOre, 1), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.titaniumOreSellPrice, -1);

            if (!RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.WorkbenchCraftingRecipies, "Revitalize.Anvil"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.Workbench_AnvilCraftingRecipeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.anvilBlueprintsPrice, 1);
            }

            AddAxeBlueprintsToShop(Menu);
            AddHoeBlueprintsToShop(Menu);
            AddPickaxeBlueprintsToShop(Menu);
            AddWateringCanBlueprintsToShop(Menu);
        }

        private static void AddAxeBlueprintsToShop(ShopMenu Menu)
        {

            int toolLevel = PlayerUtilities.GetToolLevel<Axe>();

            if (toolLevel >= Tool.iridium && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.IridiumAxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.IridiumAxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.iridiumAxeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.gold && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.GoldAxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.GoldAxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.goldAxeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.steel && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.SteelAxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.SteelAxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.steelAxeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.copper && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.CopperAxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.CopperAxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.copperAxeBlueprintPrice, 1);
            }
            if (toolLevel >= 0 && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.Axe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.AxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.axeBlueprintPrice, 1);
            }

        }

        private static void AddHoeBlueprintsToShop(ShopMenu Menu)
        {

            int toolLevel = PlayerUtilities.GetToolLevel<Hoe>();

            if (toolLevel >= Tool.iridium && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.IridiumHoe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.IridiumHoeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.iridiumHoeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.gold && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.GoldHoe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.GoldHoeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.goldHoeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.steel && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.SteelHoe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.SteelHoeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.steelHoeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.copper && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.CopperHoe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.CopperHoeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.copperHoeBlueprintPrice, 1);
            }
            if (toolLevel >= 0 && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.Hoe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.HoeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.hoeBlueprintPrice, 1);
            }

        }


        private static void AddPickaxeBlueprintsToShop(ShopMenu Menu)
        {

            int toolLevel = PlayerUtilities.GetToolLevel<Pickaxe>();

            if (toolLevel >= Tool.iridium && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.IridiumPickaxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.IridiumPickaxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.iridiumPickaxeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.gold && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.GoldPickaxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.GoldPickaxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.goldPickaxeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.steel && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.SteelPickaxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.SteelPickaxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.steelPickaxeBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.copper && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.CopperPickaxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.CopperPickaxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.copperPickaxeBlueprintPrice, 1);
            }
            if (toolLevel >= 0 && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.Pickaxe"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.PickaxeBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.pickaxeBlueprintPrice, 1);
            }

        }


        private static void AddWateringCanBlueprintsToShop(ShopMenu Menu)
        {

            int toolLevel = PlayerUtilities.GetToolLevel<WateringCan>();

            if (toolLevel >= Tool.iridium && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.IridiumWateringCan"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.IridiumWateringCanBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.iridiumWateringCanBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.gold && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.GoldWateringCan"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.GoldWateringCanBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.goldWateringCanBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.steel && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.SteelWateringCan"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.SteelWateringCanBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.steelWateringCanBlueprintPrice, 1);
            }
            if (toolLevel >= Tool.copper && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.CopperWateringCan"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.CopperWateringCanBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.copperWateringCanBlueprintPrice, 1);
            }
            if (toolLevel >= 0 && !RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipe(CraftingRecipeBooks.AnvilCraftingRecipes, "StardewValley.Tools.WateringCan"))
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(AnvilBlueprintsIds.WateringCanBlueprint), RevitalizeModCore.Configs.shopsConfigManager.blacksmithShopsConfig.wateringCanBlueprintPrice, 1);
            }

        }


    }
}
