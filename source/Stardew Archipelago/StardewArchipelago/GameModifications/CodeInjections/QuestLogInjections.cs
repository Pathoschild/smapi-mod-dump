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
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Quests;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public class QuestLogInjections
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
        }

        // public QuestLog()
        public static void Constructor_MakeQuestsNonCancellable_Postfix(QuestLog __instance)
        {
            try
            {
                foreach (var quest in Game1.player.questLog)
                {
                    quest.canBeCancelled.Value = false;
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Constructor_MakeQuestsNonCancellable_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
