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
using StardewArchipelago.Constants.Vanilla;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace StardewArchipelago.Locations.Festival
{
    internal class SpiritEveInjections
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

        // public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        public static bool CheckForAction_SpiritEveChest_Prefix(Chest __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            try
            {
                if (justCheckingForActivity || __instance.giftbox.Value || __instance.playerChest.Value || Game1.CurrentEvent == null || !Game1.CurrentEvent.isSpecificFestival("fall27"))
                {
                    return true; // run original logic
                }

                if (__instance.Items.Count <= 0 || __instance.Items.Count > 1)
                {
                    return true; // run original logic
                }

                who.currentLocation.playSound("openChest");
                if (__instance.synchronized.Value)
                    __instance.GetMutex().RequestLock(() => __instance.openChestEvent.Fire());
                else
                    __instance.performOpenChest();

                if (_archipelago.HasReceivedItem("Golden Pumpkin"))
                {
                    __instance.Items[0] = ItemRegistry.Create(QualifiedItemIds.GOLDEN_PUMPKIN);
                }
                else
                {
                    __instance.Items[0] = ItemRegistry.Create(QualifiedItemIds.PRIZE_TICKET);
                }

                _locationChecker.AddCheckedLocation(FestivalLocationNames.GOLDEN_PUMPKIN);

                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckForAction_SpiritEveChest_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
