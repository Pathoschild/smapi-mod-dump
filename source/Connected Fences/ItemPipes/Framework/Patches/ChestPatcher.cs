/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using StardewValley.Objects;
using StardewModdingAPI;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Nodes;
using ItemPipes.Framework.Util;
using Netcode;
using StardewValley.Menus;
using ItemPipes.Framework.Nodes.ObjectNodes;


namespace ItemPipes.Framework.Patches
{
    [HarmonyPatch(typeof(Chest))]
    public static class ChestPatcher
    {

        public static void Apply(Harmony harmony)
        {
            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Utility), nameof(Utility.CollectSingleItemOrShowChestMenu)),
                    prefix: new HarmonyMethod(typeof(ChestPatcher), nameof(ChestPatcher.Utility_CollectSingleItemOrShowChestMenu_Prefix))
                );
            }
            catch (Exception ex)
            {
                Printer.Info($"Failed to add chest postfix: {ex}");
            }
        }


        private static bool Utility_CollectSingleItemOrShowChestMenu_Prefix(Chest __instance, object context = null)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
            Node node = nodes.Find(n => n.Position.Equals(__instance.tileLocation));
            if (node is FilterPipeNode)
            {
                Printer.Info("COLLECTING");
                int item_count = 0;
                Item item_to_grab = null;
                for (int i = 0; i < __instance.items.Count; i++)
                {
                    if (__instance.items[i] != null)
                    {
                        item_count++;
                        if (item_count == 1)
                        {
                            item_to_grab = __instance.items[i];
                        }
                        if (item_count == 2)
                        {
                            item_to_grab = null;
                            break;
                        }
                    }
                }
                bool exit = false;
                if (item_count == 0)
                {
                    exit = true;
                }
                if (item_to_grab != null && !exit)
                {
                    Printer.Info("IN");

                    Game1.playSound("coin");
                    __instance.items.Remove(item_to_grab);
                    __instance.clearNulls();
                    exit = true;
                }
                if(!exit)
                {
                    Game1.activeClickableMenu = new ItemGrabMenu(__instance.items, reverseGrab: false, showReceivingMenu: true, InventoryMenu.highlightAllItems, __instance.grabItemFromInventory, null, __instance.grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, 1, null, -1, context);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool Chest_grabItemFromChest_Prefix(Chest __instance, Item item, Farmer who)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
            Node node = nodes.Find(n => n.Position.Equals(__instance.tileLocation));
            if (node is FilterPipeNode)
            {
                Printer.Info("GRAB FROM CHEST");
                __instance.GetItemsForPlayer(Game1.player.UniqueMultiplayerID).Remove(item);
                __instance.clearNulls();
                __instance.ShowMenu();
                return false;
            }
            else
            {
                return true;
            }
        }


        private static bool Chest_grabItemFromInventory_Prefix(Chest __instance, Item item, Farmer who)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
            Node node = nodes.Find(n => n.Position.Equals(__instance.tileLocation));
            if (node is FilterPipeNode)
            {
                Printer.Info("GRAB FROM INV");

                if (item.Stack == 0)
                {
                    item.Stack = 1;
                }
                __instance.addItem(item);
                Item tmp = who.addItemToInventory(item);
                __instance.clearNulls();
                int oldID = ((Game1.activeClickableMenu.currentlySnappedComponent != null) ? Game1.activeClickableMenu.currentlySnappedComponent.myID : (-1));
                __instance.ShowMenu();
                (Game1.activeClickableMenu as ItemGrabMenu).heldItem = tmp;
                if (oldID != -1)
                {
                    Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(oldID);
                    Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
                }
                return false;
            }
            else
            {
                return true;
            }
        }



        private static bool Chest_addItem_Prefix(Chest __instance, Item item, ref Item __result)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
            Node node = nodes.Find(n => n.Position.Equals(__instance.tileLocation));
            if(node is FilterPipeNode)
            {
                item.resetState();
                __instance.clearNulls();
                NetObjectList<Item> item_list = __instance.items;
                if (__instance.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin || __instance.SpecialChestType == Chest.SpecialChestTypes.JunimoChest)
                {
                    item_list = __instance.GetItemsForPlayer(Game1.player.UniqueMultiplayerID);
                }
                if(!item_list.Any(x => x.Name.Equals(item.Name)))
                {
                    if (item_list.Count < __instance.GetActualCapacity())
                    {
                        item_list.Add(item.getOne());
                        __result = null;
                    }
                    else
                    {
                        __result = item;
                    }
                }
                return false;
            }
            return true;
        }
    }
}
