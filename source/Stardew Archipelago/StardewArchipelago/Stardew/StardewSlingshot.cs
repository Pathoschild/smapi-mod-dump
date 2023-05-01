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
using StardewValley.Tools;

namespace StardewArchipelago.Stardew
{
    public class StardewSlingshot : StardewWeapon
    {
        public StardewSlingshot(int id, string name, string description, int minDamage, int maxDamage, double knockBack, double speed, double addedPrecision, double addedDefence, int type, int baseMineLevel, int minMineLevel, double addedAoe, double criticalChance, double criticalDamage, string displayName)
        : base(id, name, description, minDamage, maxDamage, knockBack, speed, addedPrecision, addedDefence, type, baseMineLevel, minMineLevel, addedAoe, criticalChance, criticalDamage, displayName)
        {
        }

        public override Item PrepareForGivingToFarmer(int amount = 1)
        {
            return new Slingshot(Id);
        }

        public override LetterAttachment GetAsLetter(ReceivedItem receivedItem, int amount = 1)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveSlingshot, Id.ToString());
        }
    }
}
