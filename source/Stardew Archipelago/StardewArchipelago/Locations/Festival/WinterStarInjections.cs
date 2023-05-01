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
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.Festival
{
    public class WinterStarInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static ShopReplacer _shopReplacer;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, ShopReplacer shopReplacer)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _shopReplacer = shopReplacer;
        }

        // public void chooseSecretSantaGift(Item i, Farmer who)
        public static bool ChooseSecretSantaGift_SuccessfulGift_Prefix(Event __instance, Item i, Farmer who)
        {
            try
            {
                if (i is not Object gift || _archipelago.SlotData.FestivalLocations == FestivalLocations.Vanilla)
                {
                    return true; // don't run original logic
                }

                var recipient = __instance.getActorByName(__instance.secretSantaRecipient.Name);
                var taste = (GiftTaste) recipient.getGiftTasteForThisItem(gift);

                if (_archipelago.SlotData.FestivalLocations != FestivalLocations.Hard || taste == GiftTaste.Love)
                {
                    _locationChecker.AddCheckedLocation(FestivalLocationNames.SECRET_SANTA);
                }
                
                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ChooseSecretSantaGift_SuccessfulGift_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
