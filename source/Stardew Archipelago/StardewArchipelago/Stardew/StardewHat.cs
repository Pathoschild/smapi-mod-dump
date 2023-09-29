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

namespace StardewArchipelago.Stardew
{
    public class StardewHat : StardewItem
    {
        public string SkipHairDraw { get; }
        public bool IgnoreHairstyleOffset { get; }

        public StardewHat(int id, string name, string description, string skipHairDraw, bool ignoreHairstyleOffset, string displayName)
        : base(id, name, 0, displayName, description)
        {
        }

        public override Item PrepareForGivingToFarmer(int amount = 1)
        {
            return new StardewValley.Objects.Hat(Id);
        }

        public override Item PrepareForRecovery()
        {
            throw new System.NotImplementedException();
        }

        public override void GiveToFarmer(Farmer farmer, int amount = 1)
        {
            var boots = PrepareForGivingToFarmer();
            farmer.addItemByMenuIfNecessaryElseHoldUp(boots);
        }

        public override LetterAttachment GetAsLetter(ReceivedItem receivedItem, int amount = 1)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveHat, Id.ToString());
        }
    }
}
