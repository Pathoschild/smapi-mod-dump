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
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;

namespace EscasModdingPlugins
{
	/// <summary>Allows map modders to create basic <see cref="SpecialOrdersBoard"/> with a custom "order type", i.e. a separate pool of special orders.</summary>
	/// <remarks>
	/// This class's goals:
	/// 1. Add a value to the Action tile property on the Buildings layer, allowing tiles to load the <see cref="SpecialOrdersBoard"/> menu with a custom "type" argument
	/// 2. Add additional loading logic for non-default order types, allowing access to non-default order types (only "" and "Qi" load/update by default)
	/// </remarks>
	public static class HarmonyPatch_CustomOrderBoards
	{
		/// <summary>The short (not prefixed with mod ID) name of this patch's "Action" tile property value.</summary>
		public static string ShortActionName { get; set; } = "CustomBoard";

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

			Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_CustomOrderBoards)}\": postfixing SDV method \"GameLocation.PerformAction(string, Farmer, Location)\".", LogLevel.Trace);
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), new[] { typeof(string), typeof(Farmer), typeof(Location) }),
				postfix: new HarmonyMethod(typeof(HarmonyPatch_CustomOrderBoards), nameof(GameLocation_performAction))
			);

			Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_CustomOrderBoards)}\": postfixing SDV method \"SpecialOrder.UpdateAvailableSpecialOrders(bool)\".", LogLevel.Trace);
			harmony.Patch(
				original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.UpdateAvailableSpecialOrders), new[] { typeof(bool) }),
				postfix: new HarmonyMethod(typeof(HarmonyPatch_CustomOrderBoards), nameof(SpecialOrder_UpdateAvailableSpecialOrders))
			);

			Applied = true;
		}

		/// <summary>Adds the "CustomBoard" action type for the Buildings layer "Action" property.</summary>
		/// <param name="action">The value of the "Action" tile property being parsed.</param>
		/// <param name="who">The farmer performing the action.</param>
		/// <param name="__result">True if an action was performed; false otherwise.</param>
		private static void GameLocation_performAction(string action, Farmer who, ref bool __result)
		{
			try
			{
				if (__result || !who.IsLocalPlayer) //if this action was already performed/handled OR the action was NOT taken by the local player
					return; //do nothing

				string[] actionParams = action.Split(' '); //split into parameters by spaces

				if (actionParams[0].Equals(ShortActionName, StringComparison.OrdinalIgnoreCase) || actionParams[0].Equals(ModEntry.PropertyPrefix + ShortActionName, StringComparison.OrdinalIgnoreCase)) //if this action's first parameter is "CustomBoard" or "Esca.EMP/CustomBoard"...
				{
					if (actionParams.Length > 1) //if this action has at least 2 parameters
					{
						string orderType = actionParams[1]; //use the second param as the order type

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
						Monitor.LogOnce($"Invalid \"Action\" value for custom order board: \"{action}\". No order type was provided. Valid formats: \"{ShortActionName} OrderType\" or \"{ModEntry.PropertyPrefix + ShortActionName} OrderType\".", LogLevel.Debug);
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

		/// <summary>Loads 2 additional available orders of each EMP-specific order type.</summary>
		private static void SpecialOrder_UpdateAvailableSpecialOrders()
		{
			try
			{
				var orderData = AssetHelper.GetAsset<Dictionary<string, SpecialOrderData>>("Data/SpecialOrders"); //load the special orders data
				List<string> orderKeys = GetAvailableOrderKeys(orderData); //get a list of keys to use for new orders

				//get each distinct OrderType that starts with "Esca.EMP/"
				HashSet<string> orderTypesEMP = new HashSet<string>(orderData.Select(entry => entry.Value.OrderType)
					.Where(t => t.StartsWith(ModEntry.PropertyPrefix, StringComparison.OrdinalIgnoreCase)));

				foreach (string orderTypeEMP in orderTypesEMP) //for each EMP-specific order type
				{
					if (Monitor.IsVerbose)
						Monitor.Log($"Updating available special orders for custom order type: \"{orderTypeEMP}\"", LogLevel.Trace);
					LoadCustomOrderType(orderTypeEMP, orderData, orderKeys); //load 2 available orders of this type
				}
			}
			catch (Exception ex)
			{
				Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_CustomOrderBoards)}\" has encountered an error. Special orders with custom OrderType values might not load correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
				return; //run the original method
			}
		}

		/// <summary>Gets a list of valid order keys for use when loading available special orders.</summary>
		/// <remarks>Imitates logic from <see cref="SpecialOrder.UpdateAvailableSpecialOrders(bool)"/> to validate special order keys.</remarks>
		/// <param name="order_data">Loaded data from the asset "Data/SpecialOrders".</param>
		/// <returns>A list of each key from "Data/SpecialOrders" that should be available on special order boards.</returns>
		private static List<string> GetAvailableOrderKeys(Dictionary<string, SpecialOrderData> order_data)
		{
			List<string> keys = new List<string>(order_data.Keys);
			for (int k = 0; k < keys.Count; k++)
			{
				string key = keys[k];
				bool invalid = false;
				if (!invalid && order_data[key].Repeatable != "True" && Game1.MasterPlayer.team.completedSpecialOrders.ContainsKey(key))
				{
					invalid = true;
				}
				if (Game1.dayOfMonth >= 16 && order_data[key].Duration == "Month")
				{
					invalid = true;
				}
				if (!invalid && !SpecialOrder.CheckTags(order_data[key].RequiredTags)) //prefix CheckTags with SpecialOrders (normally called internally)
				{
					invalid = true;
				}
				if (!invalid)
				{
					foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
					{
						if ((string)specialOrder.questKey == key)
						{
							invalid = true;
							break;
						}
					}
				}
				if (invalid)
				{
					keys.RemoveAt(k);
					k--;
				}
			}

			return keys; //return the completed key list
		}

		/// <summary>Loads 2 orders of the provided order type if possible.</summary>
		/// <remarks>Imitates logic from <see cref="SpecialOrder.UpdateAvailableSpecialOrders(bool)"/> to load the provided order type. Note that the original only loads "" and "Qi" types.</remarks>
		/// <param name="customOrderType">The <see cref="SpecialOrder.orderType"/> to load.</param>
		/// <param name="order_data">Loaded data from the asset "Data/SpecialOrders".</param>
		/// <param name="keys">Valid keys for available orders. See <see cref="GetAvailableOrderKeys"/>.</param>
		private static void LoadCustomOrderType(string customOrderType, Dictionary<string, SpecialOrderData> order_data, List<string> keys)
		{
			if (Game1.player.team.availableSpecialOrders.Any(order => order.orderType.Value == customOrderType)) //if any available orders already have this type
				return; //do nothing

			//imitate the original update method's loading logic, but load 2 orders of the custom type
			//note that comments below indicate edited code (actual comments) or removed code (commented out)

			Random r = new Random((int)Game1.uniqueIDForThisGame + (int)((float)Game1.stats.DaysPlayed * 1.3f));
			//Game1.player.team.availableSpecialOrders.Clear();
			//string[] array = new string[2]{ "", "Qi" };
			//foreach (string type_to_find in array)
			//{
			List<string> typed_keys = new List<string>();
			foreach (string key3 in keys)
			{
				if (order_data[key3].OrderType == customOrderType) //replace type_to_find with customOrderType
				{
					typed_keys.Add(key3);
				}
			}
			List<string> all_keys = new List<string>(typed_keys);
			//if (type_to_find != "Qi")
			//{
			for (int j = 0; j < typed_keys.Count; j++)
			{
				if (Game1.player.team.completedSpecialOrders.ContainsKey(typed_keys[j]))
				{
					typed_keys.RemoveAt(j);
					j--;
				}
			}
			//}
			for (int i = 0; i < 2; i++)
			{
				if (typed_keys.Count == 0)
				{
					if (all_keys.Count == 0)
					{
						break;
					}
					typed_keys = new List<string>(all_keys);
				}
				int index = r.Next(typed_keys.Count);
				string key2 = typed_keys[index];
				Game1.player.team.availableSpecialOrders.Add(SpecialOrder.GetSpecialOrder(key2, r.Next())); //prefix GetSpecialOrder with the SpecialOrder class (normally called internally)
				typed_keys.Remove(key2);
				all_keys.Remove(key2);
			}
			//}
		}
	}
}
