/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PersonalIndoorFarm.ModEntry;

namespace PersonalIndoorFarm.Lib
{
    public class PersonalFarm
    {
        public const string BaseFarmerPidKey = "DLX.PIF_PersonalFarmID";
        private const string DayOfMonthKey = "DLX.PIF_DayOfMonth";
        private const string SeasonKey = "DLX.PIF_Season";
        public const string OwnerKey = "DLX.PIF_Owner";
        public const string DoorIdKey = "DLX.PIF_DoorId";

        //Mine because the game needs that hard coded string to play footsteps properly :)
        public const string BaseLocationKey = "DLX.PIF_MINE";

        public static void Initialize()
        {
            GameLocation.RegisterTouchAction("DLX.PIF_WarpToFarmHouse", HandleWarpToFarmHouse);
            //I had to rename it to 'Door' because the game does a stupid .contains "Warp" check and throws a pointless warning 
            GameLocation.RegisterTileAction("DLX.PIF_DoorToFarmHouse", HandleWarpToFarmHouseAction);
            Helper.Events.Multiplayer.ModMessageReceived += ModMessageReceived;
        }

        private static bool HandleWarpToFarmHouseAction(GameLocation location, string[] args, Farmer player, Point tile)
        {
            Game1.currentLocation.playSound("doorOpen", Game1.player.Tile);
            HandleWarpToFarmHouse(location, args, player, new Vector2());
            return true;
        }

        private static void ModMessageReceived(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            if (e.Type == "shareLocation") {
                var data = e.ReadAs<ShareLocationModel>();
                var farmer = Game1.getFarmerMaybeOffline(data.PlayerId);
                createLocation(data.Pid, farmer, data.DoorId);

            } else if (e.Type == "removeFarmHand") {
                var data = e.ReadAs<RemoveFarmhandModel>();
                foreach (var location in data.PifLocations)
                    removeLocation(location, data.CabinLocation);

            } else if (e.Type == "sealLocation") {
                var name = e.ReadAs<string>();
                removeLocation(name);

            }
        }

        private static void HandleWarpToFarmHouse(GameLocation location, string[] args, Farmer player, Vector2 tile)
        {
            var warp = Door.getWarpToLast(player);
            Game1.warpFarmer(warp.TargetName, warp.TargetX, warp.TargetY, 2, true);
            player.currentLocation = Game1.getLocationFromName(warp.TargetName); //If I don't do this the targetX and Y are ignored :)
        }

        public static CreateLocationData getCreateLocationData(string pid, Farmer who)
        {
            var model = getModel(pid);

            if (model.CreateOnLoad is null)
                model.CreateOnLoad = new CreateLocationData();

            model.CreateOnLoad.MapPath = getMapAsset(who, model);
            model.CreateOnLoad.Type ??= "StardewValley.Locations.DecoratableLocation";

            return model.CreateOnLoad;
        }

        public static PersonalFarmModel getModel(string pid)
        {
            var asset = Helper.GameContent.Load<Dictionary<string, PersonalFarmModel>>(AssetRequested.FarmsAsset);

            if (!asset.TryGetValue(pid, out var model)) {
                Monitor.LogOnce($"Invalid Personal Farm ID: {pid}", LogLevel.Error);
                return null;
            }

            return model;
        }

        public static string getMapAsset(Farmer who, PersonalFarmModel model)
        {
            return who.HouseUpgradeLevel switch {
                >= 2 => model.MapAsset_T2,
                1 => model.MapAsset_T1,
                _ => model.MapAsset_T0
            };
        }

        public static Point getArrivalTile(Farmer who, PersonalFarmModel model)
        {
            return who.HouseUpgradeLevel switch {
                >= 2 => model.ArrivalTile_T2,
                1 => model.ArrivalTile_T1,
                _ => model.ArrivalTile_T0
            };
        }

        public static void setInitialDayAndSeason(GameLocation location)
        {
            location.modData[DayOfMonthKey] = Game1.dayOfMonth.ToString();
            location.modData[SeasonKey] = Game1.season.ToString();
        }

        public static Season getSeason(GameLocation location)
        {
            if (!location.modData.TryGetValue(SeasonKey, out var season))
                season = Game1.season.ToString();

            return Enum.Parse<Season>(season);
        }

        public static KeyValuePair<string, string> getDoorModData(GameLocation location, Farmer owner)
        {
            foreach (var entry in owner.modData.Pairs)
                if (entry.Key.StartsWith(BaseFarmerPidKey) && location.Name == generateLocationKey(entry.Value, owner.UniqueMultiplayerID, getDoorIdFromFarmerPIDKey(entry.Key)))
                    return entry;

            return default(KeyValuePair<string, string>);
        }

        public static void incrementDayOfMonth(GameLocation location)
        {
            var day = getDayOfMonth(location);
            var season = getSeason(location);

            day++;

            if (day >= 29) {
                day = 1;
                season = season switch {
                    Season.Spring => Season.Summer,
                    Season.Summer => Season.Fall,
                    Season.Fall => Season.Winter,
                    Season.Winter or _ => Season.Spring,
                };
            }

            location.modData[DayOfMonthKey] = day.ToString();
            location.modData[SeasonKey] = season.ToString();
        }

        public static int getDayOfMonth(GameLocation location)
        {
            if (!location.modData.TryGetValue(DayOfMonthKey, out var day)) {
                day = Game1.dayOfMonth.ToString();
            }

            return int.Parse(day);
        }

        public static GameLocation createLocation(string pid, Farmer who, string doorId)
        {
            var model = getModel(pid);
            var locationKey = generateLocationKey(pid, who.UniqueMultiplayerID, doorId);
            var location = Game1.getLocationFromName(locationKey);
            if (location is null) {
                var createLocationData = getCreateLocationData(pid, who);
                location = Game1.CreateGameLocation(locationKey, createLocationData);
                Game1.locations.Add(location);

            }

            location.modData[OwnerKey] = who.UniqueMultiplayerID.ToString();
            location.modData[BaseFarmerPidKey] = pid;
            location.modData[DoorIdKey] = doorId;

            var locationAsset = DataLoader.Locations(Game1.content);
            locationAsset.TryAdd(locationKey, model);
            return location;
        }
        public static void checkHouseUpgraded(Farmer who, GameLocation loc, string pid)
        {
            var model = getModel(pid);
            var mapAsset = getMapAsset(who, model);

            if (loc.mapPath.Value != mapAsset) {
                loc.mapPath.Value = mapAsset;
                loc.reloadMap();

            }
        }

        /// <summary></summary>
        /// <param name="name">The PIF location to be removed</param>
        /// <param name="homeLocation">The players homeLocation. Provide this only when a player cabin is being destroyed</param>
        public static void removeLocation(string name, string homeLocation = null)
        {
            if (homeLocation is null)
                HandleWarpToFarmHouse(null, null, Game1.player, default(Vector2));

            else if (Game1.currentLocation.NameOrUniqueName == name || Game1.currentLocation.NameOrUniqueName == homeLocation) {
                if (Game1.player.lastSleepLocation.Value == homeLocation)
                    Game1.player.lastSleepLocation.Value = null;

                BedFurniture.ApplyWakeUpPosition(Game1.player);
            }

            var location = Game1.getLocationFromName(name);
            if (location is not null)
                Game1.locations.Remove(location);

            var locationAsset = DataLoader.Locations(Game1.content);
            locationAsset.Remove(name);
        }
        public static string generateLocationKey(string pid, long farmerId, string doorId) => $"{BaseLocationKey}_{pid}_{farmerId}_{doorId}";
        public static string generateFarmerPIDKey(string doorId) => $"{BaseFarmerPidKey}_{doorId}";

        public static string getDoorIdFromFarmerPIDKey(string farmerPID) => farmerPID.Substring(generateFarmerPIDKey("").Length);

        public static bool isOwner(GameLocation location, Farmer who)
        {
            if (location.modData.ContainsKey(OwnerKey) && location.modData[OwnerKey] == who.UniqueMultiplayerID.ToString())
                return true;

            if (location.Name is null)
                return false;

            if (who.modData.Pairs.Any(e => e.Key.StartsWith(BaseFarmerPidKey) && location.Name.StartsWith(generateLocationKey(e.Value, who.UniqueMultiplayerID, ""))))
                return true;

            return false;
        }
    }
}
