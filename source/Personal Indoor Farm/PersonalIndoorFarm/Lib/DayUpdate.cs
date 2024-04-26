/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Enums;
using StardewValley.Locations;
using StardewValley.Objects;
using static PersonalIndoorFarm.ModEntry;


namespace PersonalIndoorFarm.Lib
{
    internal class DayUpdate
    {
        public static List<long> WasOnlineToday = new(); //Currently not multiplayer synced!!
        public static bool CanRunDayUpdate = false;

        public static void Initialize()
        {
            Helper.Events.Specialized.LoadStageChanged += LoadStageChanged;
            Helper.Events.GameLoop.DayStarted += DayStarted;
            Helper.Events.GameLoop.DayEnding += DayEnding;
            Helper.Events.Multiplayer.PeerConnected += PeerConnected;
            Helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
        }

        private static void ReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
            if (Context.ScreenId > 0)
                return;

            removeAllPifLocations();
        }

        private static void removeAllPifLocations()
        {
            var locationAsset = DataLoader.Locations(Game1.content);

            var oldAssets = locationAsset.Where(el => el.Key.StartsWith(PersonalFarm.BaseLocationKey));
            foreach (var asset in oldAssets)
                locationAsset.Remove(asset.Key);

            var oldLocations = Game1.locations.Where(e => e.Name.StartsWith(PersonalFarm.BaseLocationKey));
            var i = oldLocations.Count();
            while (i-- > 0)
                Game1.locations.Remove(oldLocations.First());
        }

        private static void LoadStageChanged(object sender, StardewModdingAPI.Events.LoadStageChangedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            if (e.NewStage != LoadStage.SaveLoadedBasicInfo)
                return;

            recreateAllLocations();
        }

        private static void DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            if (Context.ScreenId > 0)
                return;

            if (Context.IsMainPlayer)
                fillWasOnline();
            else
                recreateAllLocations();
            var farmers = Game1.getAllFarmers();

            foreach (var farmer in farmers) {
                foreach (var doorId in Door.getDoorIds(farmer)) {
                    if (!farmer.modData.TryGetValue(PersonalFarm.generateFarmerPIDKey(doorId), out var pid))
                        continue;

                    var loc = Game1.getLocationFromName(PersonalFarm.generateLocationKey(pid, farmer.UniqueMultiplayerID, doorId));

                    if (loc is null) {
                        Monitor.LogOnce("Location does not exist: " + PersonalFarm.generateLocationKey(pid, farmer.UniqueMultiplayerID, doorId), LogLevel.Error);
                        continue;
                    }

                    PersonalFarm.checkHouseUpgraded(farmer, loc, pid);

                    loc.updateSeasonalTileSheets(); //Not sure if I need this, just in case : )
                }
            }
        }

        private static void DayEnding(object sender, StardewModdingAPI.Events.DayEndingEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            var farmers = Game1.getAllFarmers();

            foreach (var farmer in farmers)
                runDayUpdate(farmer);
        }

        //I can't rely on Data/Locations directly, because I rely on a asset that doesn't get filled by CP before my Data/Locations asset edit
        //Also Game1.getLocationByName ignores unique names (mostly), so I'd have to create unique Data/Locations entries per location anyway, so it's whatever
        private static void recreateAllLocations()
        {
            var farmers = Game1.getAllFarmers();
            var locationAsset = DataLoader.Locations(Game1.content);

            var old = locationAsset.Where(el => el.Key.StartsWith(PersonalFarm.BaseLocationKey));
            foreach (var asset in old)
                locationAsset.Remove(asset.Key);


            foreach (var farmer in farmers) {
                foreach (var doorId in Door.getDoorIds(farmer)) {
                    if (!farmer.modData.TryGetValue(PersonalFarm.generateFarmerPIDKey(doorId), out var pid))
                        continue;

                    PersonalFarm.createLocation(pid, farmer, doorId);
                }
            }
        }

        private static void runDayUpdate(Farmer who)
        {
            if (!WasOnlineToday.Contains(who.UniqueMultiplayerID))
                return;

            foreach (var doorId in Door.getDoorIds(who)) {
                if (!who.modData.TryGetValue(PersonalFarm.generateFarmerPIDKey(doorId), out var pid))
                    continue;

                var loc = Game1.getLocationFromName(PersonalFarm.generateLocationKey(pid, who.UniqueMultiplayerID, doorId));

                if (loc is null) {
                    Monitor.LogOnce("Location does not exist: " + PersonalFarm.generateLocationKey(pid, who.UniqueMultiplayerID, doorId));
                    continue;
                }

                CanRunDayUpdate = true;

                PersonalFarm.incrementDayOfMonth(loc);
                var day = PersonalFarm.getDayOfMonth(loc);
                var season = PersonalFarm.getSeason(loc);

                loc.DayUpdate(day);

                CanRunDayUpdate = false;
            }
        }

        private static void PeerConnected(object sender, StardewModdingAPI.Events.PeerConnectedEventArgs e)
            => addWasOnline(e.Peer.PlayerID);


        private static void addWasOnline(long id)
        {
            if (!WasOnlineToday.Contains(id))
                WasOnlineToday.Add(id);
        }

        private static void fillWasOnline()
        {
            WasOnlineToday.Clear();
            var online = Game1.getOnlineFarmers();
            foreach (var farmer in online)
                addWasOnline(farmer.UniqueMultiplayerID);
        }
    }
}
