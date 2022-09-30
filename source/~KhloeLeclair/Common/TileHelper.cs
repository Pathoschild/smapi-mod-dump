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

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

using SObject = StardewValley.Object;

namespace Leclair.Stardew.Common;

public static class TileHelper {

	internal static IEnumerable<Vector2> IterArea(this Vector2 origin, int radius) {
		return IterArea(origin, -radius, radius, -radius, radius);
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
		if (location is FarmHouse farmHouse && position.X == farmHouse.fridgePosition.X && position.Y == farmHouse.fridgePosition.Y) {
			obj = farmHouse.fridge.Value;
			return obj is not null;

		} else if (location is IslandFarmHouse islandHouse && position.X == islandHouse.fridgePosition.X && position.Y == islandHouse.fridgePosition.Y) {
			obj = islandHouse.fridge.Value;
			return obj is not null;
		}

		return location.objects.TryGetValue(position, out obj);
	}

	internal static Vector2 GetRealPosition(this SObject obj, GameLocation? location) {
		if (location is FarmHouse farmHouse && farmHouse.fridge.Value == obj)
			return new Vector2(farmHouse.fridgePosition.X, farmHouse.fridgePosition.Y);

		if (location is IslandFarmHouse islandHouse && islandHouse.fridge.Value == obj)
			return new Vector2(islandHouse.fridgePosition.X, islandHouse.fridgePosition.Y);

		return obj.TileLocation;
	}


	// Crafting Page Stuff

	internal static Rectangle? GetBenchRegion(this CraftingPage menu, Farmer? who = null) {
		who ??= Game1.player;

		// When using the kitchen in the farmhouse, it locks the fridge's mutex. Check
		// to see if it's locked. If it is, then return the position one tile to the
		// left of the fridge.
		if (menu.IsCooking() && who.currentLocation is FarmHouse farmHouse && farmHouse.fridge.Value.GetMutex().IsLockHeld())
			return new Rectangle(
				farmHouse.fridgePosition.X - 4,
				farmHouse.fridgePosition.Y,
				4, 1
			);

		if (menu.IsCooking() && who.currentLocation is IslandFarmHouse islandHouse && islandHouse.fridge.Value.GetMutex().IsLockHeld())
			return new Rectangle(
				islandHouse.fridgePosition.X - 2,
				islandHouse.fridgePosition.Y,
				2, 1
			);

		return null;
	}

	internal static Vector2? GetBenchPosition(this CraftingPage menu, Farmer? who = null) {
		who ??= Game1.player;
		bool cooking = menu.IsCooking();

		// When using the kitchen in the farmhouse, it locks the fridge's mutex. Check
		// to see if it's locked. If it is, then return the position one tile to the
		// left of the fridge.
		if (cooking && who.currentLocation is FarmHouse farmHouse && farmHouse.fridge.Value.GetMutex().IsLockHeld())
			return new Vector2(farmHouse.fridgePosition.X - 1, farmHouse.fridgePosition.Y);

		if (cooking && who.currentLocation is IslandFarmHouse islandHouse && islandHouse.fridge.Value.GetMutex().IsLockHeld())
			return new Vector2(islandHouse.fridgePosition.X - 1, islandHouse.fridgePosition.Y);

		// Next, scan a 3x3 region centered on the player, looking for a Workbench object
		// that the player holds the mutex for.
		foreach (Vector2 pos in who.getTileLocation().IterArea()) {
			if (!GetObjectAtPosition(who.currentLocation, pos, out SObject? obj))
				continue;

			if (obj is Workbench bench && bench.checkForAction(who, true) && bench.mutex.IsLockHeld())
				return obj.GetRealPosition(who.currentLocation);

			if (obj is Torch torch && cooking && torch.checkForAction(who, true) && torch.bigCraftable.Value && torch.ParentSheetIndex == 278)
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
