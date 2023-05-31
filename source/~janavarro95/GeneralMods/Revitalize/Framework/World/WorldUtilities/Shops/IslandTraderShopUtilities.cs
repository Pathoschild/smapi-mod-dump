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
using Omegasis.Revitalize.Framework.Constants.Ids.Items.BlueprintIds;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    public static class IslandTraderShopUtilities
    {
        public static void AddStockToShop(ShopMenu menu)
        {
            //TODO: Make a config for the island and desert traders where it's a list of ItemReferences for the prices for goods in these shops.

            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.MidnightCask, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.MidnightCask), 50, Enums.SDVObject.Wine);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AdvancedSolarPanel, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.AdvancedSolarPanel), 250, Enums.SDVObject.SolarEssence);

            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.HardwoodPreservesJar, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.HardwoodPreservesJar), 10, Enums.SDVObject.Jelly);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.HardwoodPreservesJar, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.HardwoodPreservesJar), 10, Enums.SDVObject.Pickles);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AncientPreservesJar, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.AncientPreservesJar), 40, Enums.SDVObject.Jelly);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AncientPreservesJar, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.AncientPreservesJar), 40, Enums.SDVObject.Pickles);

            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.IndustrialKeg, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.IndustrialKeg), 40, Enums.SDVObject.Juice);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.IndustrialKeg, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.IndustrialKeg), 40, Enums.SDVObject.Wine);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.IndustrialKeg, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.IndustrialKeg), 80, Enums.SDVObject.Beer);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.IndustrialKeg, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.IndustrialKeg), 50, Enums.SDVObject.PaleAle);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.IndustrialKeg, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.IndustrialKeg), 200, Enums.SDVObject.Coffee);
        }

    }
}
