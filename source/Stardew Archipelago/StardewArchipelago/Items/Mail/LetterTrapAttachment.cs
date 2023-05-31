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

namespace StardewArchipelago.Items.Mail
{
    public class LetterTrapAttachment : LetterAttachment
    {
        public string TrapName { get; private set; }

        protected override bool IsEmptyLetter => false;

        public LetterTrapAttachment(ReceivedItem apItem, string trapName) : base(apItem)
        {
            TrapName = trapName;
        }

        public override void SendToPlayer(Mailman mailman)
        {
            var key = GetMailKey();
            mailman.SendArchipelagoMail(key, ArchipelagoItem.ItemName, ArchipelagoItem.PlayerName, ArchipelagoItem.LocationName, GetEmbedString());
        }

        public override MailKey GetMailKey()
        {
            return new MailKey(ArchipelagoItem.ItemName, ArchipelagoItem.PlayerName, ArchipelagoItem.LocationName, LetterActionsKeys.Trap, TrapName, ArchipelagoItem.UniqueId.ToString(), IsEmptyLetter);
        }
    }
}
