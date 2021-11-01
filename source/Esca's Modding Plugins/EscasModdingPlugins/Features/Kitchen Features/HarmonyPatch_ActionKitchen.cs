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
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace EscasModdingPlugins
{
    /// <summary>Adds a version of the "Action" "Kitchen" tile property that can function outside of FarmHouse and IslandFarmHouse locations.</summary>
    public static class HarmonyPatch_ActionKitchen
	{
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
			ActionName = ModEntry.PropertyPrefix + "Kitchen";

			Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_ActionKitchen)}\": postfixing SDV method \"GameLocation.performAction(string, Farmer, Location)\".", LogLevel.Trace);
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), new[] { typeof(string), typeof(Farmer), typeof(Location) }),
				postfix: new HarmonyMethod(typeof(HarmonyPatch_ActionKitchen), nameof(GameLocation_performAction))
			);

			Applied = true;
		}

		/// <summary>Adds a modified "Kitchen" action type for the Buildings layer "Action" property.</summary>
		/// <param name="__instance">The instance calling the original method.</param>
		/// <param name="action">The value of the "Action" tile property being parsed.</param>
		/// <param name="who">The farmer performing the action.</param>
		/// <param name="tileLocation">The tile containing this action property.</param>
		/// <param name="__result">True if an action was performed; false otherwise.</param>
		private static void GameLocation_performAction(GameLocation __instance, string action, Farmer who, Location tileLocation, ref bool __result)
		{
			try
			{
				if (action == null || __result || !who.IsLocalPlayer) //if this action is null, already performed successfully, or NOT performed by the local player
					return; //do nothing

				if (tileLocation == null) //if a tile location was not provided (required for this action)
					return; //do nothing

				string[] actionParams = action.Split(' '); //split into parameters by spaces

				if (actionParams[0].Equals(ActionName, StringComparison.OrdinalIgnoreCase)) //if this action's first parameter is "Esca.EMP/Kitchen"
				{
					if (Monitor.IsVerbose)
						Monitor.Log($"Opening kitchen menu at {__instance.NameOrUniqueName}.", LogLevel.Trace);

					ActivateKitchen(__instance);

					__result = true; //override the result (indicating that an action was performed successfully)
					return;
				}

				//if the action does not start with "Esca.EMP/Kitchen" and is unrelated to this patch, do nothing
			}
			catch (Exception ex)
			{
				Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_ActionKitchen)}..{nameof(GameLocation_performAction)}\" has encountered an error. Custom kitchens might not work correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
				return; //run the original method
			}
		}

		/// <summary>Opens the kitchen menu using any available containers. Imitates <see cref="GameLocation.ActivateKitchen"/> but avoids issues with a null "fridge" parameter.</summary>
		/// <param name="location">The location of the kitchen.</param>
		/// <returns>True if the menu successfully opened. False otherwise, e.g. if no storage containers exist or one is currently in use.</returns>
		private static void ActivateKitchen(GameLocation location)
        {
			//get the current location's static fridge if applicable
			NetRef<Chest> fridge = null;
			if (location is FarmHouse farmhouse)
				fridge = farmhouse.fridge;
			else if (location is IslandFarmHouse islandfarmhouse)
				fridge = islandfarmhouse.fridge;

			//imitate GameLocation.ActivateKitchen except where otherwise noted
			List<NetMutex> muticies = new List<NetMutex>();
			List<Chest> mini_fridges = new List<Chest>();
			foreach (StardewValley.Object item in location.objects.Values)
			{
				if (item != null && item is Chest chest)
				{
					if ((chest.bigCraftable.Value && chest.ParentSheetIndex == 216) || chest.fridge.Value) //if this chest is a Mini-Fridge OR has "fridge" set to true (the original method only checks for Mini-Fridge)
					{
						mini_fridges.Add(chest);
						muticies.Add(chest.mutex);
					}
				}
			}
			if (fridge != null && fridge.Value.mutex.IsLocked())
			{
				Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
				return;
			}
			MultipleMutexRequest multiple_mutex_request = null;
			multiple_mutex_request = new MultipleMutexRequest(muticies, delegate
			{
				if (fridge != null) //if this location has a fridge, use the original code
				{
					fridge.Value.mutex.RequestLock(delegate
					{
						List<Chest> list = new List<Chest>();
						//skip redundant null check on the fridge
						list.Add(fridge);
						list.AddRange(mini_fridges);
						Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2);
						Game1.activeClickableMenu = new CraftingPage((int)topLeftPositionForCenteringOnScreen.X, (int)topLeftPositionForCenteringOnScreen.Y, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, cooking: true, standalone_menu: true, list);
						Game1.activeClickableMenu.exitFunction = delegate
						{
							fridge.Value.mutex.ReleaseLock();
							multiple_mutex_request.ReleaseLocks();
						};
					}, delegate
					{
						Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
						multiple_mutex_request.ReleaseLocks();
					});
				}
				else //if this location does NOT have a fridge, only use mini-fridges
                {
					//don't RequestLock the fridge mutex
					List<Chest> list = new List<Chest>();
					//don't add the fridge to this list
					list.AddRange(mini_fridges);
					Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2);
					Game1.activeClickableMenu = new CraftingPage((int)topLeftPositionForCenteringOnScreen.X, (int)topLeftPositionForCenteringOnScreen.Y, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, cooking: true, standalone_menu: true, mini_fridges);
					Game1.activeClickableMenu.exitFunction = delegate
					{
						//don't ReleaseLock the fridge mutex
						multiple_mutex_request.ReleaseLocks();
					};
				}
			}, delegate
			{
				Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
			});
		}
	}
}
