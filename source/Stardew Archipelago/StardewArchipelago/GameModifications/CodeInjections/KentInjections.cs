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
    internal class KentInjections
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
        }

        // public static bool AddCharacterIfNecessary(string characterId, bool bypassConditions = false)
        public static bool AddCharacterIfNecessary_ConsiderSeasonsRandomizerForKent_Prefix(string characterId, ref bool bypassConditions)
        {
            try
            {
                if (characterId != "Kent")
                {
                    return true; // run original logic
                }

                if (Game1.Date.TotalDays < 112)
                {
                    return false; // don't run original logic
                }

                bypassConditions = true;
                return true; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AddCharacterIfNecessary_ConsiderSeasonsRandomizerForKent_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
