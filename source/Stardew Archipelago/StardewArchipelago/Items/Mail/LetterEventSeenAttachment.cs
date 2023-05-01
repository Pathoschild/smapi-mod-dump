/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewArchipelago.Archipelago;
using StardewValley;

namespace StardewArchipelago.Items.Mail
{
    public class LetterEventSeenAttachment : LetterInformationAttachment
    {
        public int EventToMarkAsSeen { get; private set; }

        protected override bool IsEmptyLetter => true;

        public LetterEventSeenAttachment(ReceivedItem apItem, int eventToMarkAsSeen) : base(apItem)
        {
            EventToMarkAsSeen = eventToMarkAsSeen;
        }

        public override void SendToPlayer(Mailman mailman)
        {
            Game1.player.eventsSeen.Add(EventToMarkAsSeen);
            base.SendToPlayer(mailman);
        }
    }
}
