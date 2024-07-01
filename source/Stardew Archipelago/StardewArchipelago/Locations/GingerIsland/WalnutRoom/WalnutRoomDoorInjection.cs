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
using StardewArchipelago.Items.Unlocks.Vanilla;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using xTile.Dimensions;

namespace StardewArchipelago.Locations.GingerIsland.WalnutRoom
{
    public class WalnutRoomDoorInjection
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        public static bool CheckAction_WalnutRoomDoorBasedOnAPItem_Prefix(IslandWest __instance, Location tileLocation, Rectangle viewport, Farmer who, ref bool __result)
        {
            try
            {
                if (__instance.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings") == -1 ||
                    __instance.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings") != 1470)
                {
                    return true; // run original logic
                }

                if (_archipelago.HasReceivedItem(VanillaUnlockManager.QI_WALNUT_ROOM))
                {
                    Game1.playSound("doorClose");
                    Game1.warpFarmer("QiNutRoom", 7, 8, 0);
                }
                else
                {
                    var walnutWarning = "You hear a strange voice from behind the door...#'Only the greatest walnut hunters may enter here.'^    Your current status: {0}/100";
                    var walnutWarningAP = "You hear a strange voice from behind the door...#'Usually, only the greatest walnut hunters may enter here, but your case is a bit special, isn't it? I believe a friend can help you out.'";
                    Game1.drawObjectDialogue(walnutWarningAP);
                }

                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckAction_WalnutRoomDoorBasedOnAPItem_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
