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
    public class SeenEvents : EventPrecondition
    {

        public List<int> seenEvents;

        public SeenEvents()
        {
            this.seenEvents = new List<int>();
        }

        public SeenEvents(int ID)
        {
            this.seenEvents.Add(ID);
        }

        public SeenEvents(List<int> IDS)
        {
            this.seenEvents = IDS.ToList();
        }

        public override string ToString()
        {
            return this.precondition_playerHasSeenEvents();
        }

        /// <summary>
        /// Current player has seen the specified events.
        /// </summary>
        /// <param name="IDS"></param>
        /// <returns></returns>
        public string precondition_playerHasSeenEvents()
        {
            StringBuilder b = new StringBuilder();
            b.Append("e ");
            for (int i = 0; i < this.seenEvents.Count; i++)
            {
                b.Append(this.seenEvents[i].ToString());
                if (i != this.seenEvents.Count - 1)
                {
                    b.Append(" ");
                }
            }
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            foreach (int v in this.seenEvents)
            {
                if (Game1.player.eventsSeen.Contains(v) == false) return false;
            }
            return true;
        }

    }
}
