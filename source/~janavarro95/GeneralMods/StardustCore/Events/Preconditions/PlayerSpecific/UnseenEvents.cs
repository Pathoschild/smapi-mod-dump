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
using StardewValley;

namespace StardustCore.Events.Preconditions.PlayerSpecific
{
    public class UnseenEvents:EventPrecondition
    {
        public List<int> unseenEvents;

        public UnseenEvents()
        {
            this.unseenEvents = new List<int>();
        }

        public UnseenEvents(int ID)
        {
            this.unseenEvents.Add(ID);
        }

        public UnseenEvents(List<int> IDS)
        {
            this.unseenEvents = IDS.ToList();
        }

        public override string ToString()
        {
            return this.precondition_playerHasNotSeenEvents();
        }

        /// <summary>
        /// Current player has seen the specified events.
        /// </summary>
        /// <param name="IDS"></param>
        /// <returns></returns>
        public string precondition_playerHasNotSeenEvents()
        {
            StringBuilder b = new StringBuilder();
            b.Append("k ");
            for (int i = 0; i < this.unseenEvents.Count; i++)
            {
                b.Append(this.unseenEvents[i]);
                if (i != this.unseenEvents.Count - 1)
                {
                    b.Append(" ");
                }
            }
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            foreach (int v in this.unseenEvents)
            {
                if (Game1.player.eventsSeen.Contains(v) == true) return false;
            }
            return true;
        }
    }
}
