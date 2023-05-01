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
    public abstract class LetterAttachment
    {
        public ReceivedItem ArchipelagoItem { get; private set; }
        protected abstract bool IsEmptyLetter { get; }

        public LetterAttachment(ReceivedItem apItem)
        {
            ArchipelagoItem = apItem;
        }

        public virtual string GetEmbedString()
        {
            return "";
        }

        public virtual void SendToPlayer(Mailman mailman)
        {
            mailman.SendArchipelagoMail(GetMailKey(), ArchipelagoItem.ItemName, ArchipelagoItem.PlayerName, ArchipelagoItem.LocationName, "");
        }

        public virtual MailKey GetMailKey()
        {
            return new MailKey(ArchipelagoItem.ItemName, ArchipelagoItem.PlayerName, ArchipelagoItem.LocationName,
                ArchipelagoItem.UniqueId.ToString(), IsEmptyLetter);
        }
    }
}
