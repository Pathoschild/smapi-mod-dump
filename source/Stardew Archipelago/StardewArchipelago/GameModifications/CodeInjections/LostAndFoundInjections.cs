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

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public static class LostAndFoundInjections
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
        }

        // public void CheckReturnedDonations()
        public static bool CheckReturnedDonations_UpgradeToolsProperly_Prefix(FarmerTeam __instance)
        {
            try
            {
                foreach (var lostAndFoundItem in __instance.returnedDonations)
                {
                    if (lostAndFoundItem is not Tool lostAndFoundTool)
                    {
                        continue;
                    }

                    lostAndFoundTool.UpgradeLevel = 0;
                    var baseName = lostAndFoundTool.Name;
                    var apName = $"Progressive {baseName}";
                    var receivedUpgrades = _archipelago.GetReceivedItemCount(apName);
                    lostAndFoundTool.UpgradeLevel = receivedUpgrades;
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckReturnedDonations_UpgradeToolsProperly_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
