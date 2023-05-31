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
using Omegasis.Revitalize.Framework.Constants.Ids.Items.BlueprintIds;
using Omegasis.Revitalize.Framework.Constants;
using StardewValley.Menus;
using Omegasis.Revitalize.Framework.Constants.CraftingIds.RecipeIds;
using Omegasis.Revitalize.Framework.Constants.CraftingIds;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    public static class DesertTraderShopUtilities
    {

        public static void AddStockToShop(ShopMenu menu)
        {
            //TODO: Make a config for the island and desert traders where it's a list of ItemReferences for the prices for goods in these shops.

            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.DarkwoodCask ,RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.DarkwoodCask), 20, Enums.SDVObject.Wine);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.HardwoodPreservesJar, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.HardwoodPreservesJar), 20, Enums.SDVObject.Jelly);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.HardwoodPreservesJar, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.HardwoodPreservesJar), 20, Enums.SDVObject.Pickles);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AncientPreservesJar, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.AncientPreservesJar), 50, Enums.SDVObject.Jelly);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AncientPreservesJar, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.AncientPreservesJar), 50, Enums.SDVObject.Pickles);

            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.HardwoodKeg, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.HardwoodKeg), 20, Enums.SDVObject.Juice);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.HardwoodKeg, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.HardwoodKeg), 20, Enums.SDVObject.Wine);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.HardwoodKeg, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.HardwoodKeg), 40, Enums.SDVObject.Beer);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.HardwoodKeg, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.HardwoodKeg), 30, Enums.SDVObject.PaleAle);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.HardwoodKeg, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.HardwoodKeg), 75, Enums.SDVObject.Coffee);

            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AutomaticTreeFarm, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.AutomaticTreeFarm), 25, Enums.SDVObject.MapleSeed);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AutomaticTreeFarm, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.AutomaticTreeFarm), 25, Enums.SDVObject.Acorn);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AutomaticTreeFarm, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.AutomaticTreeFarm), 25, Enums.SDVObject.PineCone);
            ShopUtilities.AddItemToShopIfCraftingRecipeNotKnown(menu, CraftingRecipeBooks.WorkbenchCraftingRecipies, WorkbenchRecipeIds.AutomaticTreeFarm, RevitalizeModCore.ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.AutomaticTreeFarm), 5, Enums.SDVObject.MahoganySeed);

        }
    }
}
