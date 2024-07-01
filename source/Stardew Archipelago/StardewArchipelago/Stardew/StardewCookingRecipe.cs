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
    public class StardewCookingRecipe : StardewRecipe
    {
        public StardewCookingRecipe(string itemName, Dictionary<string, int> ingredients, string yieldItemId, int yieldItemAmount, string unlockConditions, string displayName) : base(itemName, ingredients, yieldItemId, yieldItemAmount, unlockConditions, displayName)
        {
        }

        public override void TeachToFarmer(Farmer farmer)
        {
            farmer.cookingRecipes.Add(ItemName, 0);
        }

        public override LetterAttachment GetAsLetter(ReceivedItem receivedItem)
        {
            return new LetterCookingRecipeAttachment(receivedItem, ItemName);
        }
    }
}
