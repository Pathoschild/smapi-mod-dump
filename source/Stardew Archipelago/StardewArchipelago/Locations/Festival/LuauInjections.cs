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
using Microsoft.Xna.Framework;
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

        // public virtual void command_switchEvent(GameLocation location, GameTime time, string[] split)
        public static void SwitchEvent_GovernorReactionToSoup_Postfix(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            try
            {
                var switchEventKey = split[1];
                var governorReactionKey = "governorReaction";
                if (!__instance.isSpecificFestival("summer11") || !switchEventKey.StartsWith(governorReactionKey))
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
