/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewValley;

namespace Omegasis.HappyBirthday
{
    // TODO: Make all the events
    // Resources:https://stardewvalleywiki.com/Modding:Event_data
    public class BirthdayEvents
    {
        public Event communityCenterJunimoEvent;
        public Event marriedNoKidsEvent;
        public Event surpriseBirthdayPartyEvent;
        public Event marriedWithOneKidEvent;
        public Event marriedWithTwoKidsEvent;

        public BirthdayEvents()
        {
            this.initializeEvents();
        }

        public void initializeEvents()
        {
            Event e = new Event("", -1, Game1.player);
            Game1.player.currentLocation.currentEvent = new Event();
        }
    }
}
