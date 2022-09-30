/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.StardustCore.Events;
using Omegasis.StardustCore.Events.Preconditions;
using Omegasis.StardustCore.Events.Preconditions.TimeSpecific;
using StardewValley;

namespace Omegasis.HappyBirthday.Framework.Events
{
    /// <summary>
    /// A custom <see cref="EventHelper"/> written for HappyBirthday to be used to be able to share as much data as possible across all vanilla SDV spouses and modded spouses.
    /// </summary>
    public class HappyBirthdayEventHelper:EventHelper
    {
        public const string SPOUSE_IDENTIFIER_TOKEN= "$Spouse";

        public HappyBirthdayEventHelper()
        {
            this.eventData = new StringBuilder();
            this.eventPreconditions = new List<EventPrecondition>();
        }

        public HappyBirthdayEventHelper(string EventName, int ID, int version, GameLocationPrecondition Location, TimeOfDayPrecondition Time, DayOfWeekPrecondition NotTheseDays, EventStartData StartData)
        {
            this.eventStringId = EventName;
            this.eventData = new StringBuilder();
            this.eventPreconditions = new List<EventPrecondition>();
            this.stardewEventID = ID;
            this.version = version;
            this.addEventPrecondition(Location);
            this.addEventPrecondition(Time);
            this.addEventPrecondition(NotTheseDays);
            this.addEventData(StartData.ToString());
        }

        public HappyBirthdayEventHelper(string EventName, int ID, int version, List<EventPrecondition> Conditions, EventStartData StartData)
        {
            this.eventStringId = EventName;
            this.stardewEventID = ID;
            this.eventData = new StringBuilder();
            this.eventPreconditions = new List<EventPrecondition>();
            this.version = version;
            foreach (var v in Conditions)
            {
                this.addEventPrecondition(v);
            }
            this.addEventData(StartData.ToString());
        }

        public override string getEventString()
        {
            
            string eventData = base.getEventString();

            //We need to keep this check here so that we can write the .json string data to disk. Otherwise with the correct preconiditions, this check will never occur when running the actual event.
            if (Game1.player.getSpouse() == null || BirthdayEventUtilities.NEED_TO_WRITE_DEFAULT_BIRTHDAY_EVENTS_TO_JSON)
            {
                return eventData;
            }

            return eventData.Replace(SPOUSE_IDENTIFIER_TOKEN, Game1.player.getSpouse().Name);
        }
    }
}
