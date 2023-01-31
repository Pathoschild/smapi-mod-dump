/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewValley;

namespace DialaTarotCSharp;

internal class ConsoleCommandManager
{
    internal static void InitializeConsoleCommands()
    {
        Globals.CCHelper.Add("sophie.dt.testevent", "Test Diala's tarot event", (command, args) =>
            {
                string eventString = Game1.content.Load<Dictionary<string, string>>("sophie.DialaTarot/Event")["Event"];

                GameLocation currentLoc = Game1.currentLocation;
                currentLoc.startEvent(new Event(eventString));
            }
        );

        Globals.CCHelper.Add("sophie.dt.clearmoddata", "Clears the flag that blocks you from having multiple tarot readings done in a single day", (command, args) =>
            {
                Game1.player.modData.Remove("sophie.DialaTarot/ReadingDoneForToday");
            }
        );
    }
}
