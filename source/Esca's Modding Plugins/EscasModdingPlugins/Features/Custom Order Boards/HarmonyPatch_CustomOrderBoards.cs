/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.SpecialOrders;
using StardewValley.Menus;
using StardewValley.SpecialOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;

namespace EscasModdingPlugins
{
    /// <summary>Allows map mods to create basic <see cref="SpecialOrdersBoard"/> with a custom "order type", i.e. a separate pool of special orders.</summary>
    /// <remarks>
    /// This class's goals:
    /// 1. Add a value to the Action tile property on the Buildings layer, allowing tiles to load the <see cref="SpecialOrdersBoard"/> menu with a custom "type" argument
    /// 2. Add additional loading logic for non-default order types, allowing custom boards to offer them (Stardew's default order boards only load or update the "" and "Qi" types)
    /// </remarks>
    public static class HarmonyPatch_CustomOrderBoards
    {
        /// <summary>The short (not prefixed with mod ID) name of this patch's "Action" tile property value.</summary>
        public static string ShortActionName { get; set; }
        /// <summary>The full name of this patch's "Action" tile property value.</summary>
        public static string ActionName { get; set; }

        /// <summary>True if this patch is currently applied.</summary>
        public static bool Applied { get; private set; } = false;
        /// <summary>The monitor instance to use for log messages. Null if not provided.</summary>
        private static IMonitor Monitor { get; set; } = null;

        /// <summary>Applies this Harmony patch to the game.</summary>
        /// <param name="harmony">The <see cref="Harmony"/> created with this mod's ID.</param>
        /// <param name="monitor">The <see cref="IMonitor"/> provided to this mod by SMAPI. Used for log messages.</param>
        public static void ApplyPatch(Harmony harmony, IMonitor monitor)
        {
            if (Applied)
                return;

            //store args
            Monitor = monitor;

            //initialize assets/properties
            ShortActionName = "CustomBoard";
            ActionName = ModEntry.PropertyPrefix + ShortActionName;

            Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_CustomOrderBoards)}\": postfixing method \"GameLocation.PerformAction(string[], Farmer, Location)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), new[] { typeof(string[]), typeof(Farmer), typeof(Location) }),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_CustomOrderBoards), nameof(GameLocation_performAction))
            );

            Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_CustomOrderBoards)}\": postfixing method \"SpecialOrder.UpdateAvailableSpecialOrders(bool)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.UpdateAvailableSpecialOrders), new[] { typeof(string), typeof(bool) }),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_CustomOrderBoards), nameof(SpecialOrder_UpdateAvailableSpecialOrders))
            );

            Applied = true;
        }

        /// <summary>Adds the "CustomBoard" action type for the Buildings layer "Action" property.</summary>
        /// <param name="fullActionString">The value of the "Action" tile property being parsed.</param>
        /// <param name="who">The farmer performing the action.</param>
        /// <param name="__result">True if an action was performed; false otherwise.</param>
        private static void GameLocation_performAction(string[] action, Farmer who, ref bool __result)
        {
            try
            {
                if (action == null || __result || !who.IsLocalPlayer) //if this action is null, already performed successfully, or NOT performed by the local player
                    return; //do nothing

                if (action[0].Equals(ShortActionName, StringComparison.OrdinalIgnoreCase) || action[0].Equals(ActionName, StringComparison.OrdinalIgnoreCase)) //if this action's first parameter is "CustomBoard" or "Esca.EMP/CustomBoard"...
                {
                    if (action.Length > 1) //if this action has at least 2 parameters
                    {
                        string orderType = action[1]; //use the second param as the order type

                        if (!orderType.StartsWith(ModEntry.PropertyPrefix, StringComparison.OrdinalIgnoreCase)) //if the order type does NOT start with "Esca.EMP/"
                            orderType = ModEntry.PropertyPrefix + orderType; //add that prefix before using it

                        if (Monitor.IsVerbose)
                            Monitor.Log($"Opening special orders board with type \"{orderType}\".", LogLevel.Trace);

                        Game1.player.team.ordersBoardMutex.RequestLock(delegate //share the Town board's mutex lock (see the original method's "SpecialOrders" action)
                        {
                            Game1.activeClickableMenu = new SpecialOrdersBoard(orderType) //open a board menu with the first parameter as its board type
                            {
                                behaviorBeforeCleanup = delegate //when the menu closes...
                                {
                                    Game1.player.team.ordersBoardMutex.ReleaseLock(); //...release the mutex lock
                                }
                            };
                        });

                        __result = true; //override the result (indicating that an action was performed successfully)
                        return;
                    }
                    else //if a valid order type parameter was NOT provided
                    {
                        Monitor.LogOnce($"Invalid \"Action\" value for custom order board: \"{String.Join(' ', action)}\". No order type was provided. Valid formats: \"{ShortActionName} OrderType\" or \"{ActionName} OrderType\".", LogLevel.Debug);
                    }
                }

                //if the action does not start with "CustomBoard" and is unrelated to this patch, do nothing
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_CustomOrderBoards)}\" has encountered an error. Custom special order boards might not open correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }

        /// <summary>Reloads the available orders for each EMP-specific order type.</summary>
        private static void SpecialOrder_UpdateAvailableSpecialOrders(string orderType, bool forceRefresh)
        {
            try
            {
                if (orderType != "" || forceRefresh == false) //if this method is NOT refreshing the "default" special orders (which should happen once at the start of each day)
                    return; //do nothing

                var orderData = DataLoader.SpecialOrders(Game1.content); //load all special order data

                //get each distinct OrderType that starts with "Esca.EMP/"
                HashSet<string> orderTypesEMP = new HashSet<string>(orderData.Select(entry => entry.Value.OrderType)
                    .Where(t => t.StartsWith(ModEntry.PropertyPrefix, StringComparison.OrdinalIgnoreCase)));

                foreach (string orderTypeEMP in orderTypesEMP) //for each EMP-specific order type
                {
                    if (Monitor.IsVerbose)
                        Monitor.Log($"Updating available special orders for custom order type: \"{orderTypeEMP}\"", LogLevel.Trace);
                    SpecialOrder.UpdateAvailableSpecialOrders(orderTypeEMP, true); //force refresh on this order type (NOTE: beware of recursion here, since this is a patch on the same method)
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_CustomOrderBoards)}\" has encountered an error. Custom special orders might not update correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }
    }
}
