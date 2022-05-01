/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace MultipleSpouseDialog
{
    public static class HelperEvents
    {
        private static IModHelper Helper;

        // call this method from your Entry class
        public static void Initialize(IModHelper helper)
        {
            Helper = helper;
        }

        public static void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Misc.ResetSpouses(Game1.player);
            Misc.SetNPCRelations();
        }


        public static void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            Helper.Events.GameLoop.OneSecondUpdateTicked -= GameLoop_OneSecondUpdateTicked;
        }

        public static void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            Helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
            Misc.ResetSpouses(Game1.player);
        }

        public static void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            Helper.Events.GameLoop.OneSecondUpdateTicked -= GameLoop_OneSecondUpdateTicked;
        }

        private static void GameLoop_OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            foreach (var location in Game1.locations)
            {
                if (location is not FarmHouse fh) continue;
                if (fh.owner == null) continue;
                if (fh.Equals(Game1.player.currentLocation) && ModEntry.config.AllowSpousesToChat) Chatting.TryChat();
            }
        }
    }
}