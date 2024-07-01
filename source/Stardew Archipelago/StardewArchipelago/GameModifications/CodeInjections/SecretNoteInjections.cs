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
using StardewArchipelago.Locations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using Object = StardewValley.Object;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    internal class SecretNoteInjections
    {
        private const int MAX_SECRET_NOTES = 25;
        private const string SECRET_NOTE_ID = "(O)79";

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
                if (who == null || __result != null || !who.hasMagnifyingGlass)
                {
                    return;
                }

                var isIsland = __instance.InIslandContext();
                if (isIsland)
                {
                    var needsExtraJournalScrap = _locationChecker.IsAnyLocationNotChecked("Journal Scrap");
                    if (!needsExtraJournalScrap)
                    {
                        return;
                    }
                }
                else
                {
                    var needsExtraSecretNotes = _locationChecker.IsAnyLocationNotChecked("Secret Note");
                    if (!needsExtraSecretNotes)
                    {
                        return;
                    }
                }

                var itemId = isIsland ? QualifiedItemIds.JOURNAL_SCRAP : QualifiedItemIds.SECRET_NOTE;
                if (who.Items.ContainsId(itemId))
                {
                    return;
                }

                var unseenSecretNotes = Utility.GetUnseenSecretNotes(who, isIsland, out var totalNotes);
                if (unseenSecretNotes.Length > 0)
                {
                    return;
                }

                if (Game1.random.NextBool(GameLocation.LAST_SECRET_NOTE_CHANCE * 0.25))
                {
                    return;
                }

                var secretNote = ItemRegistry.Create<Object>(itemId);
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
