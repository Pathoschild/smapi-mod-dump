/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using StardewValley;
using SObject = StardewValley.Object;
using System;
using HarmonyLib;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using WarpNetwork.models;

namespace WarpNetwork.framework
{
	internal class Patches
	{
		private static readonly Dictionary<string, Point> ObeliskTargets = new()
		{
			{ "Farm", new Point(48, 7) },
			{ "IslandSouth", new Point(11, 11) },
			{ "Mountain", new Point(31, 20) },
			{ "Beach", new Point(20, 4) },
			{ "Desert", new Point(35, 43) }
		};

		internal static void Patch(Harmony harmony)
		{
			harmony.Patch(
				typeof(SObject).GetMethod(nameof(SObject.performUseAction)), 
				new(typeof(Patches), nameof(UsePrefix)));
			harmony.Patch(
				typeof(SObject).GetMethod(nameof(SObject.checkForAction)), 
				new(typeof(Patches), nameof(ActionPrefix)));
			harmony.Patch(
				typeof(Building).GetMethod(nameof(Building.PerformObeliskWarp)),
				new(typeof(Patches), nameof(MoveTarget))
			);
		}

		private static bool UsePrefix(GameLocation location, SObject __instance, ref bool __result)
		{
			if (!Game1.player.canMove || __instance.isTemporarilyInvisible)
				return true;

			if (!Game1.eventUp && !Game1.isFestival() &&
				!Game1.fadeToBlack && !Game1.player.swimming.Value &&
				!Game1.player.bathingClothes.Value && !Game1.player.onBridge.Value)
				return true;

			if (!ItemHandler.TryUseTotem(Game1.player, __instance))
				return true;

			__result = false;
			return false;
		}

		private static bool ActionPrefix(Farmer who, bool justCheckingForActivity, SObject __instance, ref bool __result)
		{
			if (__instance.isTemporarilyInvisible || who is null)
				return true;

			if (!ItemHandler.ActivateObject(__instance, justCheckingForActivity, who))
				return true;

			__result = true;
			return false;
		}

		private static bool MoveTarget(Building __instance, ref string destination, ref int warp_x, ref int warp_y, Farmer who)
		{
			if (who == Game1.player)
			{
				string Name = destination;
				if (ModEntry.config.PatchObelisks)
				{
					if (ObeliskTargets.TryGetValue(Name, out var dest) && new Point(warp_x, warp_y) == dest)
					{
						string target = Name is "IslandSouth" ? "island" : Name;
						if (Utils.GetWarpLocations().TryGetValue(target, out var loc))
						{
							if (loc is WarpLocation warp)
							{
								var tile = warp.GetLandingPoint();
								warp_x = tile.X;
								warp_y = tile.Y;
								destination = warp.Location.Trim();
							}
							else
							{
								WarpHandler.ActivateWarp(loc, __instance.GetParentLocation(), who);
								return false;
							}
						}
					}
				}
			}

			return true;
		}
	}
}
