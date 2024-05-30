/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using Leclair.Stardew.BetterCrafting.Menus;
using Leclair.Stardew.Common;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Delegates;
using StardewValley.Objects;
using StardewValley.Triggers;

namespace Leclair.Stardew.BetterCrafting.Managers;

[Flags]
public enum BenchMode {
	Standard = 0,
	Area = 1,
	Map = 2,
	World = 4,
	Buildings = 8
}

public class TriggerManager : BaseManager {

	public TriggerManager(ModEntry mod) : base(mod) {

		TriggerActionManager.RegisterAction("leclair.bettercrafting_OpenMenu", Trigger_OpenMenu);

		GameLocation.RegisterTileAction("leclair.bettercrafting_OpenMenu", Map_OpenMenu);

		GameStateQuery.Register("leclair.bettercrafting_HAS_WORKBENCH", HAS_WORKBENCH);

	}

	private static bool FindBenchRecursive(GameLocation where) {
		if (where.Objects.Values.Any(obj => obj is Workbench))
			return true;

		foreach (var building in where.buildings) {
			if (building.GetIndoors() is GameLocation interior && FindBenchRecursive(interior))
				return true;
		}

		return false;
	}

	public static bool HAS_WORKBENCH(string[] query, GameStateQueryContext context) {
		Farm farm = Game1.getFarm();
		return FindBenchRecursive(farm);
	}


	public (bool, string?, BenchMode, int?)? ParseArguments(Farmer who, string[] args, out string? error) {

		Log($"ParseArguments: {string.Join(' ', args)}", StardewModdingAPI.LogLevel.Debug);

		bool isCooking = false;
		string? station = null;
		int? range = null;
		BenchMode mode = BenchMode.Standard;

		// First Argument: bool (for cooking/not cooking) or station name
		// See if we can get a bool. If not, try getting a station.
		if (args.Length > 1 && !ArgUtility.TryGetBool(args, 1, out isCooking, out _)) {
			// Was not a valid boolean. Must be a station then.
			station = args[1];
		}

		// Second argument: bool (for include buildings yes/no) or flags
		if (args.Length > 2) {
			if (ArgUtility.TryGetBool(args, 2, out bool isBuilding, out _)) {
				if (isBuilding)
					mode |= BenchMode.Buildings;
			} else if (!ArgUtility.TryGetEnum<BenchMode>(args, 2, out mode, out _)) {
				error = "Second argument must be: true, false or comma separated list of: Area, Map, World, or Buildings";
				return null;
			}
		}

		// Third argument: possibly area if area flag. Possibly station name.
		if (mode.HasFlag(BenchMode.Area)) {
			if (ArgUtility.TryGetInt(args, 3, out int r, out _))
				range = r;
			else {
				error = "Must include range argument after Area";
				return null;
			}

		} else if (args.Length > 3) {
			if (station != null) {
				error = "Station name already given as first argument. Third argument cannot be set.";
				return null;

			} else
				station = args[3];
		}

		// Fourth argument: possibly station name if third was area
		if (args.Length > 4) {
			if (mode.HasFlag(BenchMode.Area)) {
				if (station != null) {
					error = "Station name already given as first argument. Third argument cannot be set.";
					return null;
				} else
					station = args[4];
			} else {
				error = "Unknown arguments";
				return null;
			}
		}

		// Station validation.
		if (station != null) {
			if (!Mod.Stations.TryGetStation(station, who, out var stationData)) {
				error = $"Tried to open invalid station: {station}";
				return null;
			}

			// Get the station's cooking flag.
			isCooking = stationData.IsCooking;
		}

		// Calculate the actual radius value.
		if (mode.HasFlag(BenchMode.World))
			range = -2;
		else if (mode.HasFlag(BenchMode.Map))
			range = -1;
		else if (!mode.HasFlag(BenchMode.Area))
			range = null;

		error = null;
		return (isCooking, station, mode, range);
	}


	public bool Map_OpenMenu(GameLocation location, string[] args, Farmer who, Point pos) {

		var result = ParseArguments(who, args, out string? error);
		if (!result.HasValue) {
			Log($"Error parsing arguments to open Better Crafting menu: {error}", StardewModdingAPI.LogLevel.Error);
			return false;
		}

		BenchMode mode = result.Value.Item3;

		if (Game1.player != who)
			return false;

		Log($"OpenMenu {who}, {pos}, {location}, mode: {mode}, cooking: {result.Value.Item1}, station: {result.Value.Item2}, radius: {result.Value.Item4}", StardewModdingAPI.LogLevel.Debug);

		// Ensure we're not doing anything naughty.
		if (Game1.activeClickableMenu != null) {
			if (!Game1.activeClickableMenu.readyToClose())
				return false;

			CommonHelper.YeetMenu(Game1.activeClickableMenu);
		}

		Game1.activeClickableMenu = BetterCraftingPage.Open(
			Mod,
			station: result.Value.Item2,
			location: location,
			position: pos.ToVector2(),
			standalone_menu: true,
			cooking: result.Value.Item1,
			material_containers: (IList<LocatedInventory>?) null,
			discover_buildings: mode.HasFlag(BenchMode.Buildings),
			areaOverride: result.Value.Item4
		);

		return true;
	}

	public bool Trigger_OpenMenu(string[] args, TriggerActionContext ctx, out string? error) {
		var result = ParseArguments(Game1.player, args, out error);
		if (result is null)
			return false;

		BenchMode mode = result.Value.Item3;

		Log($"OpenMenu mode: {mode}, cooking: {result.Value.Item1}, station: {result.Value.Item2}, radius: {result.Value.Item4}", StardewModdingAPI.LogLevel.Debug);

		// Ensure we're not doing anything naughty.
		if (Game1.activeClickableMenu != null) {
			if (!Game1.activeClickableMenu.readyToClose())
				return false;

			CommonHelper.YeetMenu(Game1.activeClickableMenu);
		}

		Game1.activeClickableMenu = BetterCraftingPage.Open(
			Mod,
			station: result.Value.Item2,
			standalone_menu: true,
			cooking: result.Value.Item1,
			material_containers: (IList<LocatedInventory>?) null,
			discover_buildings: mode.HasFlag(BenchMode.Buildings),
			areaOverride: result.Value.Item4
		);

		return true;
	}

}
