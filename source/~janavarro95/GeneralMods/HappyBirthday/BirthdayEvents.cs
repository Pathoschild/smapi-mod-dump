using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.HappyBirthday
{
    /// <summary>
    /// TODO:Make all the events
    /// Resources:https://stardewvalleywiki.com/Modding:Event_data
    /// </summary>
    public class BirthdayEvents
    {
        public Event communityCenterJunimoEvent;
        public Event marriedNoKidsEvent;
        public Event surpriseBirthdayPartyEvent;
        public Event marriedWithOneKidEvent;
        public Event marriedWithTwoKidsEvent;

        public BirthdayEvents()
        {
            initializeEvents();
        }

        public void initializeEvents()
        {
            Event e = new Event("", -1, Game1.player);
            Game1.player.currentLocation.currentEvent = new Event();
        }


    }
}
