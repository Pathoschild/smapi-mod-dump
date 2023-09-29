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
    public class LetterVanillaAttachment : LetterInformationAttachment
    {
        public string[] VanillaMailTitles { get; private set; }
        public bool NoLetter { get; private set; }

        public LetterVanillaAttachment(ReceivedItem apItem, string mailTitle, bool noLetter) : this(apItem, new []{mailTitle}, noLetter)
        {
        }

        public LetterVanillaAttachment(ReceivedItem apItem, string[] mailTitles, bool noLetter) : base(apItem)
        {
            VanillaMailTitles = mailTitles;
            NoLetter = noLetter;
        }

        public override void SendToPlayer(Mailman mailman)
        {
            foreach (var vanillaMailTitle in VanillaMailTitles)
            {
                mailman.SendVanillaMail(vanillaMailTitle, NoLetter);
            }
            base.SendToPlayer(mailman);
        }
    }
}
