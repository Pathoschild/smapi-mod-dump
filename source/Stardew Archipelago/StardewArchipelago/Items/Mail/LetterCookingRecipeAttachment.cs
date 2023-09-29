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
    public class LetterCookingRecipeAttachment : LetterActionAttachment
    {
        public string RecipeItemName { get; private set; }

        protected override bool IsEmptyLetter => false;

        public LetterCookingRecipeAttachment(ReceivedItem apItem, string recipeItemName) : base(apItem, LetterActionsKeys.LearnCookingRecipe, recipeItemName)
        {
            RecipeItemName = recipeItemName;
        }
    }
}
