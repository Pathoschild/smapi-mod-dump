/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Mail;
using StardewValley;

namespace StardewArchipelago.Stardew
{
    public abstract class StardewRecipe
    {

        public string ItemName { get; protected set; }
        public Dictionary<int, int> Ingredients { get; private set; }
        public int ItemId { get; private set; }
        public int ItemAmount { get; private set; }
        public string UnlockConditions { get; private set; }
        public string DisplayName { get; private set; }

        protected StardewRecipe(string itemName, Dictionary<int, int> ingredients, int itemId, int itemAmount, string unlockConditions, string displayName)
        {
            ItemName = itemName;
            Ingredients = ingredients;
            ItemId = itemId;
            ItemAmount = itemAmount;
            UnlockConditions = unlockConditions;
            DisplayName = displayName;
        }

        public abstract void TeachToFarmer(Farmer farmer);

        public abstract LetterAttachment GetAsLetter(ReceivedItem receivedItem);
    }
}
