/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using WarpNetwork.models;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace WarpNetwork
{
	class DataPatcher
	{
		private static readonly string[] DefaultDests = { "farm", "mountain", "beach", "desert", "island" };

		public static Dictionary<string, WarpLocation> ApiLocs = new(StringComparer.OrdinalIgnoreCase);
		public static Dictionary<string, WarpItem> ApiItems = new(StringComparer.OrdinalIgnoreCase);
		internal static HashSet<string> buildingTypes = new(StringComparer.OrdinalIgnoreCase);

		internal static void Init()
		{
			ModEntry.helper.Events.Content.AssetRequested += AssetRequested;
			ModEntry.helper.Events.GameLoop.SaveLoaded += SaveLoaded;
			ModEntry.helper.Events.World.BuildingListChanged += BuildingsChanged;
		}

		private static void SaveLoaded(object _, SaveLoadedEventArgs ev)
		{
			buildingTypes.Clear();
			foreach(var b in Utils.GetAllBuildings())
				buildingTypes.Add(b.buildingType.Value.Collapse());

			ModEntry.helper.GameContent.InvalidateCache(ModEntry.pathLocData);
		}
		private static void BuildingsChanged(object _, BuildingListChangedEventArgs ev)
		{
			// remove THEN add, in case a building was removed and one of the same type was added
			foreach (var b in ev.Removed)
				buildingTypes.Remove(b.buildingType.Value.Collapse());

			foreach (var b in ev.Added)
				buildingTypes.Add(b.buildingType.Value.Collapse());
		}
		internal static void AssetRequested(object _, AssetRequestedEventArgs ev)
		{
			if (ev.NameWithoutLocale.IsEquivalentTo(ModEntry.pathLocData))
				ev.Edit((a) => EditLocations(a.AsDictionary<string, WarpLocation>().Data));
			else if (ev.NameWithoutLocale.IsEquivalentTo(ModEntry.pathItemData))
				ev.Edit((a) => AddApiItems(a.AsDictionary<string, WarpItem>().Data));
			else if (ModEntry.config.MenuEnabled && MapHasWarpStatue(ev.NameWithoutLocale))
				ev.Edit((a) => AddVanillaWarpStatue(a.AsMap(), ev.NameWithoutLocale.ToString()), AssetEditPriority.Late);
		}
		private static bool MapHasWarpStatue(IAssetName name)
		{
			return
					name.IsEquivalentTo("Maps/Beach") ||
					PathUtilities.GetSegments(name.ToString(), 2)[^1].StartsWith("Beach-") ||
					name.IsEquivalentTo("Maps/Island_S") ||
					name.IsEquivalentTo("Maps/Mountain") ||
					name.IsEquivalentTo("Maps/Desert") ||
					name.IsEquivalentTo("Maps/" + Utils.GetFarmMapPath())
				;
		}
		private static void AddApiItems(IDictionary<string, WarpItem> dict)
		{
			foreach (string key in ApiItems.Keys)
				dict[key] = ApiItems[key];
		}
		private static void EditLocations(IDictionary<string, WarpLocation> dict)
		{
			foreach (string key in ApiLocs.Keys)
				dict[key] = ApiLocs[key];

			foreach (string key in DefaultDests)
				if (dict.TryGetValue(key, out var dest))
				{
					Translation label = ModEntry.i18n.Get("dest." + key);
					
					if (label.HasValue())
						dest.Label = label.ToString();
					dest.Enabled = ModEntry.config.WarpsEnabled != WarpEnabled.Never;
				}
		}
		private static void AddVanillaWarpStatue(IAssetDataForMap map, string Name)
		{
			Name = PathUtilities.GetSegments(Name)[^1];
			Name = (Name == "Island_S") ? "island" : Name.StartsWith("Beach") ? "beach" : Name.ToLowerInvariant();
			string id = (Name == Path.GetFileName(Utils.GetFarmMapPath()).ToLowerInvariant()) ? "farm" : Name;
			if (!map.Data.Properties.ContainsKey("WarpNetworkEntry"))
			{
				var locs = Utils.GetWarpLocations();
				if (!locs.ContainsKey(id))
				{
					ModEntry.monitor.Log("No destination entry for vanilla location '" + id + "'; skipping!", LogLevel.Warn);
					return;
				}
				Layer Buildings = map.Data.GetLayer("Buildings");
				if (Buildings is null)
				{
					ModEntry.monitor.Log("Could not add Warp Network to vanilla location '" + id + "'; Map is missing Buildings layer", LogLevel.Warn);
				}
				else
				{
					if (locs[id].X >= 0 && locs[id].Y > 0)
					{
						Location spot;
						if (id == "farm")
						{
							Point pt = Utils.GetActualFarmPoint(map.Data, locs["farm"].X, locs["farm"].Y, Name);
							spot = new Location(pt.X, pt.Y).Above;
						}
						else
						{
							spot = locs[id].CoordsAsLocation().Above;
						}
						ModEntry.monitor.Log("Adding access point for destination '" + id + "' @ " + spot.X + ", " + spot.Y);
						Tile tile = Buildings.Tiles[spot];
						if (tile is null)
							ModEntry.monitor.Log("No tile in building layer, could not add access point: '" + id + "' @ " + spot.X + ", " + spot.Y, LogLevel.Warn);
						else
						tile.Properties["Action"] = "WarpNetwork " + id;
					}
					else
					{
						ModEntry.monitor.Log("Could not add Warp Network to vanilla location '" + id + "'; Coordinates are outside map bounds.", LogLevel.Warn);
					}
				}
			}
		}
	}
}
