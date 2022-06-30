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
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.IO;
using WarpNetwork.models;
using xTile;
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

        internal static void AssetRequested(object _, AssetRequestedEventArgs ev)
        {
            if (ev.Name.IsEquivalentTo(ModEntry.pathLocData))
                ev.Edit((a) => EditLocations(a.AsDictionary<string, WarpLocation>().Data));
            else if (ev.Name.IsEquivalentTo(ModEntry.pathItemData))
                ev.Edit((a) => AddApiItems(a.AsDictionary<string, WarpItem>().Data));
            else if (ModEntry.config.MenuEnabled && MapHasWarpStatue(ev.Name))
                ev.Edit((a) => AddVanillaWarpStatue(a.AsMap(), ev.Name.ToString()));
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
                if (dict.ContainsKey(key))
                {
                    Translation label = ModEntry.i18n.Get("dest-" + key);
                    if (label.HasValue())
                        dict[key].Label = label;
                }

            if (ModEntry.config.FarmWarpEnabled == WarpEnabled.Never && ModEntry.config.VanillaWarpsEnabled == WarpEnabled.Never)
                return;

            if (ModEntry.config.VanillaWarpsEnabled == WarpEnabled.Always && ModEntry.config.FarmWarpEnabled != WarpEnabled.AfterObelisk)
            {
                for(int i = 0; i < DefaultDests.Length; i++)
                    EnableLocation(dict, DefaultDests[i]);
            }
            else
            {
                bool AnyObelisk = ModEntry.config.FarmWarpEnabled == WarpEnabled.Always;
                bool ObeliskAlways = ModEntry.config.VanillaWarpsEnabled == WarpEnabled.Always;

                bool ObeliskWater = ObeliskAlways;
                bool ObeliskEarth = ObeliskAlways;
                bool ObeliskDesert = ObeliskAlways;
                bool ObeliskIsland = ObeliskAlways;

                Farm farm = Game1.getFarm();
                if (farm is not null)
                {
                    //dependency loop when editing farm map leaves farm as null
                    foreach (Building building in farm.buildings)
                    {
                        switch (building.buildingType.ToString())
                        {
                            case "Water Obelisk":
                                AnyObelisk = true;
                                ObeliskWater = true;
                                break;
                            case "Earth Obelisk":
                                AnyObelisk = true;
                                ObeliskEarth = true;
                                break;
                            case "Desert Obelisk":
                                AnyObelisk = true;
                                ObeliskDesert = true;
                                break;
                            case "Island Obelisk":
                                AnyObelisk = true;
                                ObeliskIsland = true;
                                break;
                        }
                    }
                }

                if (ModEntry.config.VanillaWarpsEnabled != WarpEnabled.Never)
                {
                    EnableLocation(dict, "beach", ObeliskWater);
                    EnableLocation(dict, "mountain", ObeliskEarth);
                    EnableLocation(dict, "desert", ObeliskDesert);
                    EnableLocation(dict, "island", ObeliskIsland);
                }
                if (ModEntry.config.FarmWarpEnabled != WarpEnabled.Never)
                    EnableLocation(dict, "farm", AnyObelisk);
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
        private static void EnableLocation(IDictionary<string, WarpLocation> dict, string key, bool enabled = true)
        {
            if (dict.ContainsKey(key))
                dict[key].Enabled = enabled;
        }
    }
}
