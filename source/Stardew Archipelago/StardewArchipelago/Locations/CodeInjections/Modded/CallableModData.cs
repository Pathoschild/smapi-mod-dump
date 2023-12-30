/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewValley;

namespace StardewArchipelago.Locations.CodeInjections.Modded
{
    public class CallableModData
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public CallableModData(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            GuntherInitializer();
        }

        public void GuntherInitializer()
        {
            if (_archipelago.SlotData.Mods.HasMod(ModNames.SVE) && !Game1.player.mailReceived.Contains("GuntherUnlocked"))
            {
                Game1.player.mailReceived.Add("GuntherUnlocked");
                Game1.player.eventsSeen.Add(103042015); // Gunther says hi
                Game1.player.eventsSeen.Add(1579125); // Marlon entering sewer immediately after; just annoying to see day one
            }
        }
    }
}