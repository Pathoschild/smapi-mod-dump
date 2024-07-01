/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

namespace StardewArchipelago.Extensions
{
    public static class LocationRequestExtensions
    {
        public static (LocationRequest, Point) PerformLastLocationRequestChanges(this LocationRequest locationRequest, GameLocation origin, Point warpPoint, Point warpPointTarget)
        {
            // locationRequest = MakeBeachNightMarketChanges(locationRequest);
            (locationRequest, warpPointTarget) = MakeFarmToGreenhouseChanges(locationRequest, origin, warpPointTarget);
            // warpPointTarget = MakeIslandSouthChanges(locationRequest, warpPointTarget);
            // Warp offset volcano dungeon
            // Dismount horse on warp
            // (locationRequest, warpPointTarget) = MakeTrailerChanges(locationRequest, warpPointTarget);
            warpPointTarget = MakeFarmChanges(locationRequest, origin, warpPointTarget);
            // Club Bouncer attacks you
            // Bypass festival if exiting the hospital
            // Enter Festival
            return (locationRequest, warpPointTarget);
        }

        private static LocationRequest MakeBeachNightMarketChanges(LocationRequest locationRequest)
        {
            if (!locationRequest.Name.Equals("Beach") || !Game1.currentSeason.Equals("winter") ||
                Game1.dayOfMonth < 15 || Game1.dayOfMonth > 17 || Game1.eventUp)
            {
                return locationRequest;
            }

            return Game1.getLocationRequest("BeachNightMarket");
        }

        private static (LocationRequest, Point) MakeFarmToGreenhouseChanges(LocationRequest locationRequest, GameLocation origin, Point warpPointTarget)
        {
            if (!locationRequest.Name.Equals("Farm") || origin.NameOrUniqueName != "Greenhouse")
            {
                return (locationRequest, warpPointTarget);
            }

            foreach (var warp in origin.warps)
            {
                if (warp.TargetX != warpPointTarget.X || warp.TargetY != warpPointTarget.Y)
                {
                    continue;
                }

                foreach (var building in Game1.getFarm().buildings)
                {
                    if (building is not GreenhouseBuilding greenhouse)
                    {
                        continue;
                    }

                    warpPointTarget = new Point(greenhouse.getPointForHumanDoor().X, greenhouse.getPointForHumanDoor().Y + 1);
                    return (locationRequest, warpPointTarget);
                }

                return (locationRequest, warpPointTarget);
            }

            return (locationRequest, warpPointTarget);
        }

        private static Point MakeIslandSouthChanges(LocationRequest locationRequest, Point warpPointTarget)
        {
            if (locationRequest.Name == "IslandSouth" && warpPointTarget.X <= 15 && warpPointTarget.Y <= 6)
            {
                return new Point(21, 43);
            }

            return warpPointTarget;
        }

        private static (LocationRequest locationRequest, Point warpPointTarget) MakeTrailerChanges(LocationRequest locationRequest, Point warpPointTarget)
        {
            if (locationRequest.Name.Equals("Trailer") && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
            {
                return (Game1.getLocationRequest("Trailer_Big"), new Point(13, 24));
            }

            return (locationRequest, warpPointTarget);
        }

        private static Point MakeFarmChanges(LocationRequest locationRequest, GameLocation origin,
            Point warpPointTarget)
        {
            if (!locationRequest.Name.Equals("Farm"))
            {
                return warpPointTarget;
            }

            var farm = Game1.getFarm();
            warpPointTarget = MakeFarmcaveToFarmChanges(origin, warpPointTarget, farm);
            warpPointTarget = MakeForestToFarmChanges(origin, warpPointTarget, farm);
            warpPointTarget = MakeBusStopToFarmChanges(origin, warpPointTarget, farm);
            warpPointTarget = MakeBackwoodsToFarmChanges(origin, warpPointTarget, farm);
            warpPointTarget = MakeFarmhouseToFarmChanges(origin, warpPointTarget, farm);

            return warpPointTarget;
        }

        private static Point MakeFarmcaveToFarmChanges(GameLocation origin, Point warpPointTarget, Farm farm)
        {
            if (origin.NameOrUniqueName == "FarmCave" && warpPointTarget.X == 34 && warpPointTarget.Y == 6)
            {
                switch (Game1.whichFarm)
                {
                    case 5:
                        warpPointTarget = new Point(30, 36);
                        break;
                    case 6:
                        warpPointTarget = new Point(34, 16);
                        break;
                }

                if (farm.TryGetMapPropertyAs("FarmCaveEntry", out Point farmCavePoint))
                {
                    return farmCavePoint;
                }
            }

            return warpPointTarget;
        }

        private static Point MakeForestToFarmChanges(GameLocation origin, Point warpPointTarget, Farm farm)
        {
            if (origin.NameOrUniqueName == "Forest" && warpPointTarget.X == 41 && warpPointTarget.Y == 64)
            {
                switch (Game1.whichFarm)
                {
                    case 5:
                        warpPointTarget = new Point(40, 64);
                        break;
                    case 6:
                        warpPointTarget = new Point(82, 103);
                        break;
                }

                if (farm.TryGetMapPropertyAs("ForestEntry", out Point forestPoint))
                {
                    return forestPoint;
                }
            }

            return warpPointTarget;
        }

        private static Point MakeBusStopToFarmChanges(GameLocation origin, Point warpPointTarget, Farm farm)
        {
            if (origin.NameOrUniqueName == "BusStop" && warpPointTarget.X == 79 && warpPointTarget.Y == 17)
            {
                if (farm.TryGetMapPropertyAs("BusStopEntry", out Point busStopPoint))
                {
                    return busStopPoint;
                }
            }

            return warpPointTarget;
        }

        private static Point MakeBackwoodsToFarmChanges(GameLocation origin, Point warpPointTarget, Farm farm)
        {
            if (origin.NameOrUniqueName == "Backwoods" && warpPointTarget.X == 40 && warpPointTarget.Y == 0)
            {
                if (farm.TryGetMapPropertyAs("BackwoodsEntry", out Point backwoodsPoint))
                {
                    return backwoodsPoint;
                }
            }

            return warpPointTarget;
        }

        private static Point MakeFarmhouseToFarmChanges(GameLocation origin, Point warpPointTarget, Farm farm)
        {
            if (origin.NameOrUniqueName == "FarmHouse" && warpPointTarget.X == 64 && warpPointTarget.Y == 15)
            {
                return farm.GetMainFarmHouseEntry();
            }

            return warpPointTarget;
        }
    }
}
