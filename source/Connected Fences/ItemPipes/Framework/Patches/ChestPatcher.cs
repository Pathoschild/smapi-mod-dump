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
using ItemPipes.Framework.Objects;
using Netcode;
using StardewValley.Menus;


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
                    original: AccessTools.Method(typeof(Chest), nameof(Chest.addItem)),
                    prefix: new HarmonyMethod(typeof(ChestPatcher), nameof(ChestPatcher.Chest_addItem_Prefix))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(Chest), nameof(Chest.grabItemFromInventory)),
                    prefix: new HarmonyMethod(typeof(ChestPatcher), nameof(ChestPatcher.Chest_grabItemFromInventory_Prefix))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(Utility), nameof(Utility.CollectSingleItemOrShowChestMenu)),
                    prefix: new HarmonyMethod(typeof(ChestPatcher), nameof(ChestPatcher.Utility_CollectSingleItemOrShowChestMenu_Prefix))
                );
            }
            catch (Exception ex)
            {
                Framework.Printer.Info($"Failed to add chest postfix: {ex}");
            }
        }


        private static bool Utility_CollectSingleItemOrShowChestMenu_Prefix(Chest __instance, object context = null)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Node> nodes;
            if (DataAccess.LocationNodes.TryGetValue(Game1.currentLocation, out nodes))
            {
                Node node = nodes.Find(n => n.Position.Equals(__instance.tileLocation));
                if (node is FilterPipe)
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
            else
            {
                return true;
            }
        }

        private static bool Chest_grabItemFromChest_Prefix(Chest __instance, Item item, Farmer who)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Node> nodes;
            if (DataAccess.LocationNodes.TryGetValue(Game1.currentLocation, out nodes))
            {
                Node node = nodes.Find(n => n.Position.Equals(__instance.tileLocation));
                if (node is FilterPipe)
                {
                    Printer.Info("GRAB FROM CHEST");

                    if (who.couldInventoryAcceptThisItem(item))
                    {
                        __instance.GetItemsForPlayer(Game1.player.UniqueMultiplayerID).Remove(item);
                        __instance.clearNulls();
                        __instance.ShowMenu();
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }


        private static bool Chest_grabItemFromInventory_Prefix(Chest __instance, Item item, Farmer who)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Node> nodes;
            if (DataAccess.LocationNodes.TryGetValue(Game1.currentLocation, out nodes))
            {
                Node node = nodes.Find(n => n.Position.Equals(__instance.tileLocation));
                if (node is FilterPipe)
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
            else
            {
                return true;
            }

        }



        private static bool Chest_addItem_Prefix(Chest __instance, Item item, ref Item __result)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Node> nodes;
            if (DataAccess.LocationNodes.TryGetValue(Game1.currentLocation, out nodes))
            {
                Node node = nodes.Find(n => n.Position.Equals(__instance.tileLocation));
                if(node is FilterPipe)
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
                }
                return false;
            }
            else
            {
                return true;
            }

        }


        /*
        private static void Chest_ShowMenu_Postfix(Chest __instance, Item item)
        {
            Printer.Info("SHOW MENU");
            if(false)
            {
                Game1.activeClickableMenu = new ItemGrabMenu(
                    __instance.GetItemsForPlayer(Game1.player.UniqueMultiplayerID), 
                    reverseGrab: false, 
                    showReceivingMenu: true, 
                    Utility.highlightShippableObjects, 
                    __instance.grabItemFromInventory, 
                    null, 
                    __instance.grabItemFromChest, 
                    snapToBottom: false, 
                    canBeExitedWithKey: true, 
                    playRightClickSound: true, 
                    allowRightClick: true, 
                    showOrganizeButton: false, 
                    1, 
                    __instance.fridge ? null : __instance, 
                    -1, 
                    __instance);

            }
            else
            {
                if (__instance.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
                {
                    Game1.activeClickableMenu = new ItemGrabMenu(__instance.GetItemsForPlayer(Game1.player.UniqueMultiplayerID), reverseGrab: false, showReceivingMenu: true, Utility.highlightShippableObjects, __instance.grabItemFromInventory, null, __instance.grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: false, 1, __instance.fridge ? null : __instance, -1, __instance);
                }
                else if (__instance.SpecialChestType == Chest.SpecialChestTypes.JunimoChest)
                {
                    Game1.activeClickableMenu = new ItemGrabMenu(__instance.GetItemsForPlayer(Game1.player.UniqueMultiplayerID), reverseGrab: false, showReceivingMenu: true, InventoryMenu.highlightAllItems, __instance.grabItemFromInventory, null, __instance.grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, 1, __instance.fridge ? null : __instance, -1, __instance);
                }
                else if (__instance.SpecialChestType == Chest.SpecialChestTypes.AutoLoader)
                {
                    ItemGrabMenu itemGrabMenu = new ItemGrabMenu(__instance.GetItemsForPlayer(Game1.player.UniqueMultiplayerID), reverseGrab: false, showReceivingMenu: true, InventoryMenu.highlightAllItems, __instance.grabItemFromInventory, null, __instance.grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, 1, __instance.fridge ? null : __instance, -1, __instance);
                    itemGrabMenu.exitFunction = (IClickableMenu.onExit)Delegate.Combine(itemGrabMenu.exitFunction, (IClickableMenu.onExit)delegate
                    {
                        __instance.CheckAutoLoad(Game1.player);
                    });
                    Game1.activeClickableMenu = itemGrabMenu;
                }
                else if (__instance.SpecialChestType == Chest.SpecialChestTypes.Enricher)
                {
                    Game1.activeClickableMenu = new ItemGrabMenu(__instance.GetItemsForPlayer(Game1.player.UniqueMultiplayerID), reverseGrab: false, showReceivingMenu: true, StardewValley.Object.HighlightFertilizers, __instance.grabItemFromInventory, null, __instance.grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, 1, __instance.fridge ? null : __instance, -1, __instance);
                }
                else
                {
                    Game1.activeClickableMenu = new ItemGrabMenu(__instance.GetItemsForPlayer(Game1.player.UniqueMultiplayerID), reverseGrab: false, showReceivingMenu: true, InventoryMenu.highlightAllItems, __instance.grabItemFromInventory, null, __instance.grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, 1, __instance.fridge ? null : __instance, -1, __instance);
                }
            }
        }

        private static void Chest_grabItemFromChest_Postfix(Chest __instance, Item item, Farmer who)
        {
            Printer.Info("CHEST GRAB CHEST POSTFIX");

            //Filter
            SGraphDB DataAccess = SGraphDB.GetSGraphDB();
            SGNode[,] locationMatrix;
            if (DataAccess.LocationMatrix.TryGetValue(Game1.currentLocation, out locationMatrix))
            {
                if (locationMatrix[(int)__instance.tileLocation.X, (int)__instance.tileLocation.Y] is FilterPipe)
                {
                    __instance.ShowMenu();
                }
                else
                {
                    if (who.couldInventoryAcceptThisItem(item))
                    {
                        __instance.GetItemsForPlayer(Game1.player.UniqueMultiplayerID).Remove(item);
                        __instance.clearNulls();
                        __instance.ShowMenu();
                    }
                }
            }
        }

        private static void Chest_grabItemFromInventory_Prefix(Chest __instance, Item item, Farmer who)
        {
            Printer.Info("CHEST GRAB INVENTROY POSTFIX");
            //Filter
            SGraphDB DataAccess = SGraphDB.GetSGraphDB();
            SGNode[,] locationMatrix;
            if (DataAccess.LocationMatrix.TryGetValue(Game1.currentLocation, out locationMatrix))
            {
                if (locationMatrix[(int)__instance.tileLocation.X, (int)__instance.tileLocation.Y] is FilterPipe)
                {
                    Printer.Info("LOADING INTO FILTER");
                    if (item.Stack == 0)
                    {
                        item.Stack = 1;
                    }
                    Item tmp = __instance.addItem(item);
                    if (tmp == null)
                    {
                        who.removeItemFromInventory(item);
                        tmp = who.addItemToInventory(tmp);
                    }
                    else
                    {
                        tmp = who.addItemToInventory(tmp);
                    }
                    __instance.clearNulls();
                    int oldID = ((Game1.activeClickableMenu.currentlySnappedComponent != null) ? Game1.activeClickableMenu.currentlySnappedComponent.myID : (-1));
                    __instance.ShowMenu();
                    (Game1.activeClickableMenu as ItemGrabMenu).heldItem = tmp;
                    if (oldID != -1)
                    {
                        Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(oldID);
                        Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
                    }
                }
                else
                {
                    
                    Printer.Info("NOT FILTER");
                    Printer.Info(item.Name);
                    Printer.Info(item.Stack.ToString());
                    if (item.Stack == 0)
                    {
                        item.Stack = 1;
                    }
                    Item tmp = __instance.addItem(item);
                    Printer.Info("Printing items");
                    Printer.Info(__instance.items.Count.ToString());
                    foreach(Item item2 in __instance.items.ToList())
                    {
                        Printer.Info(item2.Stack.ToString());
                    }
                    Printer.Info("end 2");
                    if (tmp == null)
                    {
                        who.removeItemFromInventory(item);
                    }
                    else
                    {
                        Printer.Info(tmp.Name);
                        tmp = who.addItemToInventory(tmp);
                    }
                    __instance.clearNulls();
                    Printer.Info("Printing items 2");
                    foreach (Item item2 in __instance.items.ToList())
                    {
                        Printer.Info(item2.Stack.ToString());
                    }
                    Printer.Info("end 2");
                    int oldID = ((Game1.activeClickableMenu.currentlySnappedComponent != null) ? Game1.activeClickableMenu.currentlySnappedComponent.myID : (-1));
                    __instance.ShowMenu();
                    (Game1.activeClickableMenu as ItemGrabMenu).heldItem = tmp;
                    Printer.Info(__instance.items.Count.ToString());
                    if (oldID != -1)
                    {
                        Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(oldID);
                        Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
                    }
                    Printer.Info("Printing items 3");
                    foreach (Item item2 in __instance.items.ToList())
                    {
                        Printer.Info(item2.Stack.ToString());
                    }
                    Printer.Info("end 3");
                    //El problema ocurre despues
                    //rollo en el siguiente metodo que se ejecuta
                }
            }
        }
        */
    }
}
