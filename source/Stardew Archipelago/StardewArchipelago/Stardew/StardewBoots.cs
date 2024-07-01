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
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Items.Mail;
using StardewValley;
using StardewValley.Objects;

namespace StardewArchipelago.Stardew
{
    public class StardewBoots : StardewItem
    {
        public int AddedDefense { get; }
        public int AddedImmunity { get; }
        public int ColorIndex { get; }

        public StardewBoots(string id, string name, int sellPrice, string description, int addedDefense, int addedImmunity, int colorIndex, string displayName)
            : base(id, name, sellPrice, displayName, description)
        {
            AddedDefense = addedDefense;
            AddedImmunity = addedImmunity;
            ColorIndex = colorIndex;
        }

        public override Item PrepareForGivingToFarmer(int amount = 1)
        {
            return new Boots(Id);
        }

        public override void GiveToFarmer(Farmer farmer, int amount = 1)
        {
            var boots = PrepareForGivingToFarmer();
            farmer.addItemByMenuIfNecessaryElseHoldUp(boots);
        }

        public override LetterAttachment GetAsLetter(ReceivedItem receivedItem, int amount = 1)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveSpecificBoots, Id.ToString());
        }

        public override string GetQualifiedId()
        {
            return $"{QualifiedItemIds.BOOTS_QUALIFIER}{Id}";
        }
    }
}
