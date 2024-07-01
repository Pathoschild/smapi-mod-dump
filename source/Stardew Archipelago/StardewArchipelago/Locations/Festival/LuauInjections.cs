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
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.Festival
{
    public static class LuauInjections
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

        // public static void SwitchEvent(Event @event, string[] args, EventContext context)
        public static void SwitchEvent_GovernorReactionToSoup_Postfix(Event @event, string[] args, EventContext context)
        {
            try
            {
                var switchEventKey = args[1];
                var governorReactionKey = "governorReaction";
                if (!@event.isSpecificFestival("summer11") || !switchEventKey.StartsWith(governorReactionKey))
                {
                    return;
                }

                var soupScore = int.Parse(switchEventKey[governorReactionKey.Length..]);
                var isEasyMode = _archipelago.SlotData.FestivalLocations != FestivalLocations.Hard;

                if (soupScore == 4 || (isEasyMode && soupScore is 2 or 3))
                {
                    _locationChecker.AddCheckedLocation(FestivalLocationNames.LUAU_SOUP);
                }
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SwitchEvent_GovernorReactionToSoup_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
