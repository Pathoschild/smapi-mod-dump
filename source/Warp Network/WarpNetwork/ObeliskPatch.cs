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
using StardewValley;
using System.Collections.Generic;

namespace WarpNetwork
{
    class ObeliskPatch
    {
        private static IMonitor Monitor;
        private static Config Config;
        private static readonly Dictionary<string, Point> ObeliskTargets = new Dictionary<string, Point>()
        {
            {"Farm", new Point(48, 7)},
            {"IslandSouth",new Point(11, 11)},
            {"Mountain",new Point(31, 20)},
            {"Beach",new Point(20, 4)},
            {"Desert",new Point(35, 43)}
        };
        internal static void Init(IMonitor monitor, Config config)
        {
            Monitor = monitor;
            Config = config;
        }
        public static void MoveAfterWarp(object sender, WarpedEventArgs ev)
        {
            if (ev.IsLocalPlayer)
            {
                string Name = (ev.NewLocation.Name == "BeachNightMarket") ? "Beach" : ev.NewLocation.Name;
                if (Name == "Desert")
                {
                    //desert warp patch

                    Point point = ObeliskTargets["Desert"];
                    if (!(WarpHandler.DesertWarp is null))
                    {
                        Point to = (Point)WarpHandler.DesertWarp;
                        WarpHandler.DesertWarp = null;
                        ev.Player.setTileLocation(new Vector2(to.X, to.Y));
                    } else if (Config.PatchObelisks && point == ev.Player.getTileLocationPoint())
                    {
                        Dictionary<string, WarpLocation> dests = Utils.GetWarpLocations();
                        if (dests.ContainsKey("desert"))
                        {
                            WarpLocation dest = dests["desert"];
                            if (dest.OverrideMapProperty)
                            {
                                WarpHandler.DesertWarp = null;
                                ev.Player.setTileLocation(new Vector2(dest.X, dest.Y));
                                return;
                            }
                        }
                        Point to = ev.NewLocation.GetMapPropertyPosition("WarpNetworkEntry", point.X, point.Y);
                        WarpHandler.DesertWarp = null;
                        ev.Player.setTileLocation(new Vector2(to.X, to.Y));
                    }
                }
                else if (Config.PatchObelisks)
                {
                    if (ObeliskTargets.ContainsKey(Name))
                    {
                        Point point = ObeliskTargets[Name];
                        if (Name == "Farm")
                        {
                            if (ModEntry.IslandObeliskFixed || ev.OldLocation.Name != "IslandWest")
                            {
                                point = Utils.GetActualFarmPoint(point.X, point.Y);
                            } else
                            {
                                //if the island obelisk isn't fixed, match the broken coords instead of the expected ones

                                switch (Game1.whichFarm)
                                {
                                    //four corners
                                    case 5: point = new Point(48, 39); break;

                                    //beach
                                    case 6: point = new Point(82, 29); break;
                                }
                            }
                        }
                        if (ev.Player.getTileLocationPoint() == point)
                        {
                            Dictionary<string, WarpLocation> dests = Utils.GetWarpLocations();
                            string target = (Name == "IslandSouth") ? "island" : Name;
                            if (dests.ContainsKey(target))
                            {
                                WarpLocation dest = dests[target];
                                if (dest.OverrideMapProperty)
                                {
                                    ev.Player.setTileLocation(new Vector2(dest.X, dest.Y));
                                    return;
                                }
                            } else if(Name == "Farm" && !ModEntry.IslandObeliskFixed && !ev.NewLocation.map.Properties.ContainsKey("WarpNetworkEntry"))
                            {
                                //will still fix the broken island obelisk IF it isn't already fixed, even if the farm destination is deleted and the farm has no warp network override.

                                Point pos = ev.NewLocation.GetMapPropertyPosition("WarpTotemEntry", point.X, point.Y);
                                ev.Player.setTileLocation(new Vector2(pos.X, pos.Y));
                                return;
                            }
                            Point to = ev.NewLocation.GetMapPropertyPosition("WarpNetworkEntry", point.X, point.Y);
                            ev.Player.setTileLocation(new Vector2(to.X, to.Y));
                        }
                    }
                }
            }
        }
    }
}
