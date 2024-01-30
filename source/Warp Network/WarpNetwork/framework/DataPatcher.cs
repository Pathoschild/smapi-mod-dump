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
using Microsoft.Xna.Framework.Graphics;
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

namespace WarpNetwork.framework
{
	class DataPatcher
	{
		private static readonly string[] DefaultDests = 
			{ "farm", "mountain", "beach", "desert", "island" };

		private static readonly HashSet<string> knownIcons =
			new(new[] { "DEFAULT", "farm", "mountain", "island", "desert", "beach", "RETURN" });

		private static readonly Func<LegacyWarpLocation, WarpLocation> convert_destination =
			typeof(LegacyWarpLocation).GetMethod(nameof(LegacyWarpLocation.Convert))
			.CreateDelegate<Func<LegacyWarpLocation, WarpLocation>>();

		// TODO replace statue edits with location edits

		internal static void Init()
		{
			ModEntry.helper.Events.Content.AssetRequested += AssetRequested;
		}
		internal static void AssetRequested(object _, AssetRequestedEventArgs ev)
		{
			if (ev.NameWithoutLocale.IsDirectlyUnderPath(ModEntry.AssetPath)) 
			{
				var name = ev.NameWithoutLocale.ToString().WithoutPath(ModEntry.AssetPath);
				switch (name)
				{
					case "Totems":
						ev.LoadFromModFile<Dictionary<string, WarpItem>>("assets/Totems.json", AssetLoadPriority.Medium);
						ev.Edit(static (a) => PortData(a.Data as Dictionary<string, WarpItem>, "WarpItems")); 
						break;
					case "PlacedTotems":
						ev.LoadFromModFile<Dictionary<string, WarpItem>>("assets/PlacedTotems.json", AssetLoadPriority.Medium);
						ev.Edit(static (a) => PortData(a.Data as Dictionary<string, WarpItem>, "Objects")); 
						break;
					case "Strings":
						ev.LoadFromModFile<Dictionary<string, string>>(
							$"i18n/{ModEntry.helper.ToLocalLocale(ev.Name.LocaleCode)}.json", 
							AssetLoadPriority.High);
						break;
					case "Destinations":
						ev.LoadFromModFile<Dictionary<string, WarpLocation>>("assets/Destinations.json", AssetLoadPriority.Medium);
						ev.Edit(static (a) => PortData(a.Data as Dictionary<string, WarpLocation>, "Destinations", convert_destination));
						break;

				}
			} else if (ev.NameWithoutLocale.IsDirectlyUnderPath(ModEntry.LegacyAssetPath))
			{
				var name = ev.NameWithoutLocale.ToString().WithoutPath(ModEntry.LegacyAssetPath);
				switch (name)
				{
					case "Objects":
					case "WarpItems":
						ev.LoadFrom(static () => new Dictionary<string, WarpItem>(), AssetLoadPriority.Low);
						break;
					case "Destinations":
						ev.LoadFrom(static () => new Dictionary<string, LegacyWarpLocation>(), AssetLoadPriority.Low);
						break;
				}
			} else if (ev.NameWithoutLocale.IsDirectlyUnderPath(ModEntry.AssetPath + "/Icons"))
			{
				var name = ev.NameWithoutLocale.ToString().WithoutPath(ModEntry.AssetPath + "/Icons");
				ev.LoadFromModFile<Texture2D>($"assets/icons/{name}.png", AssetLoadPriority.Low);
			}
		}
		private static void PortData<T>(Dictionary<string, T> data, string oldName)
		{
			var legacy = ModEntry.helper.GameContent.Load<Dictionary<string, T>>(ModEntry.LegacyAssetPath + '/' + oldName);
			foreach ((var key, var val) in legacy)
				data.TryAdd(key, val);
		}
		private static void PortData<TI, TO>(Dictionary<string, TO> data, string oldName, Func<TI, TO> converter)
		{
			var legacy = ModEntry.helper.GameContent.Load<Dictionary<string, TI>>(ModEntry.LegacyAssetPath + '/' + oldName);
			foreach ((var key, var val) in legacy)
				data.TryAdd(key, converter(val));
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
		private static void AddVanillaWarpStatue(IAssetDataForMap map, string Name)
		{
			Name = PathUtilities.GetSegments(Name)[^1];
			Name = Name == "Island_S" ? "island" : Name.StartsWith("Beach") ? "beach" : Name.ToLowerInvariant();
			string id = Name == Path.GetFileName(Utils.GetFarmMapPath()).ToLowerInvariant() ? "farm" : Name;

			if (!map.Data.Properties.ContainsKey("WarpNetworkEntry"))
			{
				var locs = Utils.GetWarpLocations();
				if (!locs.TryGetValue(id, out var loc))
				{
					ModEntry.monitor.Log($"No destination entry for vanilla location '{id}'; skipping!", LogLevel.Warn);
					return;
				}
				Layer Buildings = map.Data.GetLayer("Buildings");
				if (Buildings is null)
				{
					ModEntry.monitor.Log($"Could not add Warp Network to vanilla location '{id}'; Map is missing Buildings layer", LogLevel.Warn);
				}
				else if (loc is WarpLocation warp)
				{
					if (warp.Position != default)
					{
						Point tilePos = warp.Position;
						if (id == "farm")
						{
							Utils.TryGetActualFarmPoint(ref tilePos, map.Data, Name);
						}
						var spot = new Location(tilePos.X, tilePos.Y).Above;

						ModEntry.monitor.Log($"Adding access point for destination '{id}' @ {spot.X}, {spot.Y}");

						Tile tile = Buildings.Tiles[spot];
						if (tile is null)
							ModEntry.monitor.Log($"No tile in building layer, could not add access point: '{id}' @ {spot.X}, {spot.Y}", LogLevel.Warn);
						else
							tile.Properties["Action"] = "WarpNetwork " + id;
					}
					else
					{
						ModEntry.monitor.Log($"Could not add Warp Network to vanilla location '{id}'; Coordinates are outside map bounds.", LogLevel.Warn);
					}
				}
				else
				{
					ModEntry.monitor.Log($"Could not add warp stature to Vanilla destination '{id}' because it's using a custom handler!", LogLevel.Warn);
				}
			}
		}
	}
}
