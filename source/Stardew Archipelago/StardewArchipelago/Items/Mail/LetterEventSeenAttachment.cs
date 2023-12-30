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
using StardewArchipelago.Archipelago;
using StardewValley;

namespace StardewArchipelago.Items.Mail
{
    public class LetterEventSeenAttachment : LetterInformationAttachment
    {
        public List<int> EventsToMarkAsSeen { get; private set; }

        protected override bool IsEmptyLetter => true;

        public LetterEventSeenAttachment(ReceivedItem apItem, int eventToMarkAsSeen) : this(apItem, new List<int> { eventToMarkAsSeen })
        {
        }

        public LetterEventSeenAttachment(ReceivedItem apItem, List<int> eventsToMarkAsSeen) : base(apItem)
        {
            EventsToMarkAsSeen = eventsToMarkAsSeen;
        }

        public override void SendToPlayer(Mailman mailman)
        {
            Game1.player.eventsSeen.AddRange(EventsToMarkAsSeen);
            base.SendToPlayer(mailman);
        }
    }
}
