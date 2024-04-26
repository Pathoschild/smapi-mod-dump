/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace Leclair.Stardew.Common;

public static class TileHelper {

	internal static Vector2 Move(this Vector2 origin, float x, float y) {
		return new(origin.X + x, origin.Y + y);
	}

	internal static IEnumerable<Vector2> IterArea(this Vector2 origin, int radius, bool lazy = true) {
		var result = IterArea(origin, -radius, radius, -radius, radius);
		if (lazy)
			return result;

		return result.Where(other => origin.Distance(other.X, other.Y) <= radius);
	}

	internal static float Distance(this Vector2 start, float x, float y) {
		return MathF.Sqrt(MathF.Pow(start.X - x, 2) + MathF.Pow(start.Y - y, 2));
	}


	internal static IEnumerable<Vector2> IterArea(this Vector2 origin, int width, int height) {
		width = Math.Max(0, width - 1);
		height = Math.Max(0, height - 1);

		int left = width / 2;
		int right = width - left;

		int top = height / 2;
		int bottom = height - top;

		return IterArea(origin, -left, right, -top, bottom);
	}

	internal static IEnumerable<Vector2> IterArea(this Vector2 origin, int minX = -1, int maxX = 1, int minY = -1, int maxY = 1) {
		for (int x = minX; x <= maxX; x++) {
			for (int y = minY; y <= maxY; y++) {
				yield return new Vector2(origin.X + x, origin.Y + y);
			}
		}
	}

	internal static bool GetObjectAtPosition(this GameLocation location, Vector2 position, [NotNullWhen(true)] out SObject? obj) {
		if (location.GetFridgePosition() is Point pos && pos.X == position.X && pos.Y == position.Y) {
			obj = location.GetFridge(false);
			return obj is not null;
		}

		return location.Objects.TryGetValue(position, out obj);
	}

	internal static Vector2 GetRealPosition(this SObject obj, GameLocation? location) {
		location ??= obj.Location;

		if (location is not null && location.GetFridge(false) == obj && location.GetFridgePosition() is Point pos)
			return new Vector2(pos.X, pos.Y);

		return obj.TileLocation;
	}


	// Crafting Page Stuff

	internal static Rectangle? GetBenchRegion(this CraftingPage menu, Farmer? who = null) {
		who ??= Game1.player;

		// When using the kitchen, it locks the fridge's mutex. Check to see if it's locked.
		// If it is, then return the position one tile to the left of the fridge.
		if (menu.cooking && (who.currentLocation.GetFridge(false)?.GetMutex()?.IsLockHeld() ?? false) && who.currentLocation.GetFridgePosition() is Point p)
			return new Rectangle(
				p.X - 4,
				p.Y,
				4, 1
			);

		return null;
	}

	internal static Vector2? GetBenchPosition(this CraftingPage menu, Farmer? who = null) {
		who ??= Game1.player;

		// When using the kitchen in the farmhouse, it locks the fridge's mutex. Check
		// to see if it's locked. If it is, then return the position one tile to the
		// left of the fridge.
		if (menu.cooking && (who.currentLocation.GetFridge(false)?.GetMutex()?.IsLockHeld() ?? false) && who.currentLocation.GetFridgePosition() is Point p)
			return new Vector2(p.X - 1, p.Y);

		// Next, scan a 3x3 region centered on the player, looking for a Workbench object
		// that the player holds the mutex for.
		foreach (Vector2 pos in who.Tile.IterArea()) {
			if (!GetObjectAtPosition(who.currentLocation, pos, out SObject? obj))
				continue;

			if (obj is Workbench bench && bench.checkForAction(who, true) && bench.mutex.IsLockHeld())
				return obj.GetRealPosition(who.currentLocation);

			// We also support crafting from Cookout Kits.
			if (obj is Torch torch && menu.cooking && torch.checkForAction(who, true) && torch.bigCraftable.Value && torch.ParentSheetIndex == 278)
				return obj.GetRealPosition(who.currentLocation);
		}

		// Fall back to scanning every object in the current location, in case players are
		// using a mod that lets them access objects from a distance.
		foreach (SObject obj in who.currentLocation.objects.Values) {
			if (obj is Workbench bench && bench.checkForAction(who, true) && bench.mutex.IsLockHeld())
				return obj.GetRealPosition(who.currentLocation);
		}

		// TODO: Check for non-Workbench type objects.

		// RIP. There is no Workbench. Time is an illusion.
		return null;
	}

}
