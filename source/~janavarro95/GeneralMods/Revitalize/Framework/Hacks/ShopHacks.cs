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
using PyTK.Types;

namespace Revitalize.Framework.Hacks
{
    /// <summary>
    /// Deals with modifications for SDV shops.
    /// </summary>
    public class ShopHacks
    {

        public static void AddInCustomItemsToShops()
        {
            AddItemsToRobinsShop();
            AddOreToClintsShop();
        }


        private static void AddItemsToRobinsShop()
        {
            PyTK.Extensions.PyEvents.addToNPCShop(new InventoryItem(ModCore.ObjectManager.GetItem("Workbench", 1), 500), "Robin");
            PyTK.Extensions.PyEvents.addToNPCShop(new InventoryItem(new StardewValley.Object((int)Enums.SDVObject.Clay,1),50), "Robin");
            PyTK.Extensions.PyEvents.addToNPCShop(new InventoryItem(ModCore.ObjectManager.resources.getResource("Sand", 1), 25), "Robin");
        }
        /// <summary>
        /// Adds in ore to clint's shop.
        /// </summary>
        private static void AddOreToClintsShop()
        {
            PyTK.Extensions.PyEvents.addToNPCShop(new InventoryItem(ModCore.ObjectManager.resources.getOre("Tin",1),ModCore.Configs.shops_blacksmithConfig.tinOreSellPrice), "Clint");
            PyTK.Extensions.PyEvents.addToNPCShop(new InventoryItem(ModCore.ObjectManager.resources.getOre("Bauxite", 1), ModCore.Configs.shops_blacksmithConfig.bauxiteOreSellPrice), "Clint");
            PyTK.Extensions.PyEvents.addToNPCShop(new InventoryItem(ModCore.ObjectManager.resources.getOre("Lead", 1), ModCore.Configs.shops_blacksmithConfig.leadOreSellPrice), "Clint");
            PyTK.Extensions.PyEvents.addToNPCShop(new InventoryItem(ModCore.ObjectManager.resources.getOre("Silver", 1), ModCore.Configs.shops_blacksmithConfig.silverOreSellPrice), "Clint");
            PyTK.Extensions.PyEvents.addToNPCShop(new InventoryItem(ModCore.ObjectManager.resources.getOre("Titanium", 1), ModCore.Configs.shops_blacksmithConfig.titaniumOreSellPrice), "Clint");
        }
    }
}
