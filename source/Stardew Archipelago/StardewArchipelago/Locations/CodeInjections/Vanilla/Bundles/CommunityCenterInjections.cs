/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Goals;
using StardewArchipelago.Locations.Modded.SVE;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using xTile.Dimensions;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Bundles
{
    public static class CommunityCenterInjections
    {
        public const string AP_LOCATION_PANTRY = "Complete Pantry";
        public const string AP_LOCATION_CRAFTS_ROOM = "Complete Crafts Room";
        public const string AP_LOCATION_FISH_TANK = "Complete Fish Tank";
        public const string AP_LOCATION_BOILER_ROOM = "Complete Boiler Room";
        public const string AP_LOCATION_VAULT = "Complete Vault";
        public const string AP_LOCATION_BULLETIN_BOARD = "Complete Bulletin Board";
        public const string AP_LOCATION_ABANDONED_JOJA_MART = "The Missing Bundle";

        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static FriendshipReleaser _friendshipReleaser;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago, LocationChecker locationChecker, BundleReader bundleReader)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _friendshipReleaser = new FriendshipReleaser(locationChecker, bundleReader);
        }

        public static bool DoAreaCompleteReward_AreaLocations_Prefix(CommunityCenter __instance, int whichArea)
        {
            try
            {
                var AreaAPLocationName = "";
                var mailToSend = "";
                switch ((Area)whichArea)
                {
                    case Area.Pantry:
                        AreaAPLocationName = AP_LOCATION_PANTRY;
                        mailToSend = "apccPantry";
                        break;
                    case Area.CraftsRoom:
                        AreaAPLocationName = AP_LOCATION_CRAFTS_ROOM;
                        mailToSend = "apccCraftsRoom";
                        break;
                    case Area.FishTank:
                        AreaAPLocationName = AP_LOCATION_FISH_TANK;
                        mailToSend = "apccFishTank";
                        break;
                    case Area.BoilerRoom:
                        AreaAPLocationName = AP_LOCATION_BOILER_ROOM;
                        mailToSend = "apccBoilerRoom";
                        break;
                    case Area.Vault:
                        AreaAPLocationName = AP_LOCATION_VAULT;
                        mailToSend = "apccVault";
                        break;
                    case Area.Bulletin:
                        AreaAPLocationName = AP_LOCATION_BULLETIN_BOARD;
                        mailToSend = "apccBulletin";
                        break;
                    case Area.AbandonedJojaMart:
                        AreaAPLocationName = AP_LOCATION_ABANDONED_JOJA_MART;
                        mailToSend = "apccMovieTheater";
                        break;
                }

                if (!Game1.player.mailReceived.Contains(mailToSend))
                {
                    Game1.player.mailForTomorrow.Add(mailToSend + "%&NL&%");
                }

                _locationChecker.AddCheckedLocation(AreaAPLocationName);
                _friendshipReleaser.ReleaseMorrisHeartsIfNeeded();
                GoalCodeInjection.CheckCommunityCenterGoalCompletion();

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(DoAreaCompleteReward_AreaLocations_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool ShouldNoteAppearInArea_AllowAccessEverything_Prefix(CommunityCenter __instance, int area, ref bool __result)
        {
            try
            {
                switch ((Area)area)
                {
                    case Area.Pantry:
                        __result = !Game1.player.hasOrWillReceiveMail("apccPantry"); // _locationChecker.IsLocationNotChecked(AP_LOCATION_PANTRY);
                        return false; // don't run original logic
                    case Area.CraftsRoom:
                        __result = !Game1.player.hasOrWillReceiveMail("apccCraftsRoom"); // _locationChecker.IsLocationNotChecked(AP_LOCATION_CRAFTS_ROOM);
                        return false; // don't run original logic
                    case Area.FishTank:
                        __result = !Game1.player.hasOrWillReceiveMail("apccFishTank"); // _locationChecker.IsLocationNotChecked(AP_LOCATION_FISH_TANK);
                        return false; // don't run original logic
                    case Area.BoilerRoom:
                        __result = !Game1.player.hasOrWillReceiveMail("apccBoilerRoom"); // _locationChecker.IsLocationNotChecked(AP_LOCATION_BOILER_ROOM);
                        return false; // don't run original logic
                    case Area.Vault:
                        __result = !Game1.player.hasOrWillReceiveMail("apccVault"); // _locationChecker.IsLocationNotChecked(AP_LOCATION_VAULT);
                        return false; // don't run original logic
                    case Area.Bulletin:
                        __result = !Game1.player.hasOrWillReceiveMail("apccBulletin"); // _locationChecker.IsLocationNotChecked(AP_LOCATION_BULLETIN_BOARD);
                        return false; // don't run original logic
                    case Area.AbandonedJojaMart:
                        __result = !Game1.player.hasOrWillReceiveMail("apccMovieTheater"); // _locationChecker.IsLocationNotChecked(AP_LOCATION_BULLETIN_BOARD);
                        return false; // don't run original logic
                }
                __result = false;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ShouldNoteAppearInArea_AllowAccessEverything_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool CheckAction_BulletinBoardNoRequirements_Prefix(CommunityCenter __instance,
            Location tileLocation, Rectangle viewport, Farmer who, ref bool __result)
        {
            try
            {
                var tile = __instance.map.GetLayer("Buildings").Tiles[tileLocation];
                if (tile == null || tile.TileIndex != 1799)
                {
                    return true; // run original logic
                }

                __instance.checkBundle(5);
                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckAction_BulletinBoardNoRequirements_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // private void checkForMissedRewards()
        public static bool CheckForMissedRewards_DontBother_Prefix(CommunityCenter __instance)
        {
            try
            {
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckForMissedRewards_DontBother_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
