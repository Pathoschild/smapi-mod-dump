/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using WarpNetwork.models;

namespace WarpNetwork.framework
{
	class ObeliskPatch
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
				typeof(Building).GetMethod(nameof(Building.PerformObeliskWarp)),
				new(typeof(ObeliskPatch), nameof(MoveTarget))
			);
		}

		public static bool MoveTarget(Building __instance, ref string destination, ref int warp_x, ref int warp_y, Farmer who)
		{
			if (who == Game1.player)
			{
				string Name = destination is "BeachNightMarket" ? "Beach" : destination;
				if (ModEntry.config.PatchObelisks)
				{
					if (ObeliskTargets.TryGetValue(Name, out var dest) && new Point(warp_x, warp_y) == dest)
					{
						string target = Name is "IslandSouth" ? "island" : Name;
						var targetLoc = Game1.getLocationFromName(Name);
						if (Utils.GetWarpLocations().TryGetValue(target, out var loc))
						{
							if (loc is WarpLocation warp)
							{
								if (warp.OverrideMapProperty ||
									!targetLoc.TryGetMapPropertyAs("WarpNetworkEntry", out Point tile))
									tile = warp.Position;

								warp_x = tile.X; 
								warp_y = tile.Y;
								destination = warp.Location.Trim();
							} else
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
