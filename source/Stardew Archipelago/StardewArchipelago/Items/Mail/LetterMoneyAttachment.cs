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
    public class LetterMoneyAttachment : LetterAttachment
    {
        public int MoneyAmount { get; private set; }

        protected override bool IsEmptyLetter => false;

        public LetterMoneyAttachment(ReceivedItem apItem, int moneyAmount) : base(apItem)
        {
            MoneyAmount = moneyAmount;
        }

        public override string GetEmbedString()
        {
            return $"%item money {MoneyAmount} %%";
        }

        public override void SendToPlayer(Mailman mailman)
        {
            mailman.SendArchipelagoMail(GetMailKey(), ArchipelagoItem.ItemName, ArchipelagoItem.PlayerName, ArchipelagoItem.LocationName, GetEmbedString());
        }
    }
}
