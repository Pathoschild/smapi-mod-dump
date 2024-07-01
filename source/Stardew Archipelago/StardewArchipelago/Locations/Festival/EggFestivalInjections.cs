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
using System.Collections.Generic;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.Festival
{
    public static class EggFestivalInjections
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

        // public static void AwardFestivalPrize(Event @event, string[] args, EventContext context)
        public static bool AwardFestivalPrize_Strawhat_Prefix(Event @event, string[] args, EventContext context)
        {
            try
            {
                // private HashSet<long> festivalWinners = new HashSet<long>();
                var festivalWinnersField = _modHelper.Reflection.GetField<HashSet<long>>(@event, "festivalWinners");

                // private Dictionary<string, string> festivalData;
                var festivalDataField = _modHelper.Reflection.GetField<Dictionary<string, string>>(@event, "festivalData");

                var festivalWinners = festivalWinnersField.GetValue();
                var festivalData = festivalDataField.GetValue();

                if (festivalWinners == null || festivalData == null)
                {
                    return true; // run original logic
                }

                var playerWonFestival = festivalWinners.Contains(Game1.player.UniqueMultiplayerID);
                var isEggFestivalDay = festivalData["file"] == "spring13";

                if (!playerWonFestival || !isEggFestivalDay)
                {
                    return true; // run original logic
                }

                _locationChecker.AddCheckedLocation(FestivalLocationNames.EGG_HUNT);
                if (Game1.player.mailReceived.Contains("Egg Festival"))
                {
                    return true; // run original logic
                }

                Game1.player.mailReceived.Add("Egg Festival");
                @event.CurrentCommand += 2;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AwardFestivalPrize_Strawhat_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
