/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Mail;

namespace StardewArchipelago.Items.Unlocks.Vanilla
{
    public class EquipmentUnlockManager : IUnlockManager
    {
        public const string PROGRESSIVE_WEAPON = "Progressive Weapon";
        public const string PROGRESSIVE_SWORD = "Progressive Sword";
        public const string PROGRESSIVE_CLUB = "Progressive Club";
        public const string PROGRESSIVE_DAGGER = "Progressive Dagger";
        public const string PROGRESSIVE_BOOTS = "Progressive Footwear";
        public const string PROGRESSIVE_SLINGSHOT = "Progressive Slingshot";

        public EquipmentUnlockManager()
        {
        }

        public void RegisterUnlocks(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            RegisterEquipment(unlocks);
        }

        private void RegisterEquipment(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add(PROGRESSIVE_WEAPON, SendProgressiveWeaponLetter);
            unlocks.Add(PROGRESSIVE_SWORD, SendProgressiveSwordLetter);
            unlocks.Add(PROGRESSIVE_CLUB, SendProgressiveClubLetter);
            unlocks.Add(PROGRESSIVE_DAGGER, SendProgressiveDaggerLetter);
            unlocks.Add(PROGRESSIVE_BOOTS, SendProgressiveBootsLetter);
            unlocks.Add(PROGRESSIVE_SLINGSHOT, SendProgressiveSlingshotLetter);
        }

        private LetterActionAttachment SendProgressiveWeaponLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveWeapon);
        }

        private LetterActionAttachment SendProgressiveSwordLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveSword);
        }

        private LetterActionAttachment SendProgressiveClubLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveClub);
        }

        private LetterActionAttachment SendProgressiveDaggerLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveDagger);
        }

        private LetterActionAttachment SendProgressiveBootsLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveProgressiveBoots);
        }

        private LetterActionAttachment SendProgressiveSlingshotLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveProgressiveSlingshot);
        }
    }
}
