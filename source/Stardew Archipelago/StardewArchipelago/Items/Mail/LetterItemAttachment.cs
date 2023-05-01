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
using StardewArchipelago.Stardew;

namespace StardewArchipelago.Items.Mail
{
    public class LetterItemAttachment : LetterAttachment
    {
        public StardewItem ItemAttachment { get; private set; }
        public int AttachmentAmount { get; private set; }

        protected override bool IsEmptyLetter => false;

        public LetterItemAttachment(ReceivedItem apItem, StardewItem itemAttachment, int attachmentAmount = 1) : base(apItem)
        {
            ItemAttachment = itemAttachment;
            AttachmentAmount = attachmentAmount;
        }

        public override string GetEmbedString()
        {
            return $"%item object {ItemAttachment.Id} {AttachmentAmount} %%";
        }

        public override void SendToPlayer(Mailman mailman)
        {
            mailman.SendArchipelagoMail(GetMailKey(), ArchipelagoItem.ItemName, ArchipelagoItem.PlayerName, ArchipelagoItem.LocationName, GetEmbedString());
        }
    }
}
