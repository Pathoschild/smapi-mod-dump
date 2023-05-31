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
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Quests;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public class WorldChangeEventInjections
    {
        private static IMonitor _monitor;

        public static void Initialize(IMonitor monitor)
        {
            _monitor = monitor;
        }

        // public bool setUp()
        public static bool SetUp_MakeSureEventsAreNotDuplicated_Prefix(WorldChangeEvent __instance, ref bool __result)
        {
            try
            {
                var eventToBeAdded = "";
                switch (__instance.whichEvent.Value)
                {
                    case 1:
                        eventToBeAdded = "cc_Greenhouse";
                        break;
                    case 2:
                    case 3:
                        eventToBeAdded = "cc_Minecart";
                        break;
                    case 4:
                    case 5:
                        eventToBeAdded = "cc_Bridge";
                        break;
                    case 6:
                    case 7:
                        eventToBeAdded = "cc_Bus";
                        break;
                    case 8:
                    case 9:
                        eventToBeAdded = "cc_Boulder";
                        break;
                    case 10:
                    case 11:
                        eventToBeAdded = "movieTheater";
                        break;
                    default:
                        return true; // run original logic
                }

                if (Game1.player.activeDialogueEvents.ContainsKey(eventToBeAdded))
                {
                    Game1.player.activeDialogueEvents.Remove(eventToBeAdded);
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SetUp_MakeSureEventsAreNotDuplicated_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
