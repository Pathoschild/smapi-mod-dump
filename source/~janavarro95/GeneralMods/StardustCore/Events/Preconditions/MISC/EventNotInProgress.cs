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

namespace StardustCore.Events.Preconditions.MISC
{
    public class EventNotInProgress:EventPrecondition
    {
        public string id;

        public EventNotInProgress()
        {

        }

        public EventNotInProgress(string ID)
        {
            this.id = ID;
        }

        public override string ToString()
        {
            return this.precondition_EventNotInProgress();
        }

        /// <summary>
        /// Quote from SDV wiki.
        /// "The special dialogue event with the given ID is not in progress.
        /// This can be a custom event ID, but these are the in-game IDs:
        /// cc_Begin, cc_Boulder, cc_Bridge, cc_Bus, cc_Complete, cc_Greenhouse, cc_Minecart, dumped_Girls, dumped_Guys, Introduction, joja_Begin, pamHouseUpgrade, pamHouseUpgradeAnonymous, secondChance_Girls, secondChance_Guys, willyCrabs."
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string precondition_EventNotInProgress()
        {
            StringBuilder b = new StringBuilder();
            b.Append("A ");
            b.Append(this.id.ToString());
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            return Game1.player.activeDialogueEvents.ContainsKey(this.id) == false;
        }
    }
}
