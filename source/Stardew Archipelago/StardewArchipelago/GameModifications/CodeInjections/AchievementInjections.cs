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

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public static class AchievementInjections
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
        }

        public static bool GetSteamAchievement_DisableUndeservedAchievements_Prefix(string which)
        {
            try
            {
                if (_archipelago.SlotData.ArcadeMachineLocations == ArcadeLocations.FullShuffling || _archipelago.SlotData.ArcadeMachineLocations == ArcadeLocations.VictoriesEasy)
                {
                    var jotpkVictory = "Achievement_PrairieKing";
                    var fector = "Achievement_FectorsChallenge";
                    if (which == jotpkVictory || which == fector)
                    {
                        return false; // don't run original logic
                    }
                }

                if (_archipelago.SlotData.ElevatorProgression != ElevatorProgression.Vanilla)
                {
                    var bottom_of_the_mine = "Achievement_TheBottom";
                    if (which == bottom_of_the_mine)
                    {
                        return false; // don't run original logic
                    }
                }

                return true; // run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetSteamAchievement_DisableUndeservedAchievements_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
