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
using StardewArchipelago.Items.Mail;
using StardewValley;
using StardewValley.Objects;

namespace StardewArchipelago.Stardew
{
    public class StardewRing : StardewObject
    {
        public StardewRing(int id, string name, int sellPrice, int edibility, string type, string category, string displayName, string description, string misc1 = "", string misc2 = "", string buffDuration = "")
        : base(id, name, sellPrice, edibility, type, category, displayName, description, misc1, misc2, buffDuration)
        {
        }

        public override Item PrepareForGivingToFarmer(int amount = 1)
        {
            return new Ring(Id);
        }

        public override LetterAttachment GetAsLetter(ReceivedItem receivedItem, int amount = 1)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveRing, Id.ToString());
        }
    }
}
