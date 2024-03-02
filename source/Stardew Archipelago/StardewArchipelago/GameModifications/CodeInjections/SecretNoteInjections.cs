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
using StardewArchipelago.Locations;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    internal class SecretNoteInjections
    {
        private const int MAX_SECRET_NOTES = 25;

        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public Object tryToCreateUnseenSecretNote(Farmer who)
        public static void TryToCreateUnseenSecretNote_AllowSecretNotesIfStillNeedToShipThem_Postfix(GameLocation __instance, Farmer who, ref Object __result)
        {
            try
            {
                if (__result != null || !who.hasMagnifyingGlass)
                {
                    return;
                }

                var isIsland = __instance.GetLocationContext() == GameLocation.LocationContext.Island;
                if (isIsland)
                {
                    return;
                }
                
                if (who.secretNotesSeen.Count < MAX_SECRET_NOTES || who.hasItemWithNameThatContains("Secret Note") != null)
                {
                    return;
                }

                if (Game1.random.NextDouble() > 0.4)
                {
                    return;
                }

                var needsExtraSecretNotes = _locationChecker.IsAnyLocationNotChecked("Secret Note");
                if (!needsExtraSecretNotes)
                {
                    return;
                }

                var secretNoteNumber = Game1.random.Next(1, MAX_SECRET_NOTES+1);
                var secretNote = new Object(79, 1);
                secretNote.name = secretNote.name + " #" + secretNoteNumber;
                __result = secretNote;
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(TryToCreateUnseenSecretNote_AllowSecretNotesIfStillNeedToShipThem_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
