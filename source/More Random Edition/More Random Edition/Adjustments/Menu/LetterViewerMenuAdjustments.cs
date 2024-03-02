/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace Randomizer
{
    public class LetterViewerMenuAdjustments
    {
        /// <summary>
        /// Adjusts the cooking recipe name to be our changed version
        /// </summary>
        /// <param name="menu">The letter UI menu</param>
        public static void AdjustCookingRecipeName(LetterViewerMenu menu)
        {
            // The game specifically names these mails with <NPC Name>Cooking, so
            // this should be safe
            if (!menu.isMail || !menu.mailTitle.EndsWith("Cooking"))
            {
                return;
            }

            string recipeName = GetCookingRecipeKey(menu.learnedRecipe);
            if (recipeName == null)
            {
                return;
            }

            // No reason for the if other than a clean way to execute only the first one of these
            // if the recipe is found to be a crop
            if (TryReplaceRecipeDisplayName(menu, CraftingRecipeAdjustments.CropDishesMap, recipeName) ||
                TryReplaceRecipeDisplayName(menu, CraftingRecipeAdjustments.FishDishesMap, recipeName)) {
                return;
            }
        }

        /// <summary>
        /// Gets the English recipe key from the given menu.learnedRecipe value
        /// This is the same if in English, otherwise we look up the key based
        /// on the display name found in Data/CookingRecipes (a reverse search
        /// from what Stardew does in LetterViewerMenu.cs)
        /// </summary>
        /// <param name="learnedRecipe">The recipe the menu displays</param>
        /// <returns>The Data/CookingRecipe key, or null if not found</returns>
        private static string GetCookingRecipeKey(string learnedRecipe)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
            {
                return learnedRecipe;
            }

            const int DisplayNameIndex = 4;
            var cookingRecipeData = Globals.ModRef.Helper.GameContent
                .Load<Dictionary<string, string>>("Data/CookingRecipes");

            foreach(KeyValuePair<string, string> data in cookingRecipeData)
            {
                string[] tokens = data.Value.Split("/");
                if (tokens.Length != DisplayNameIndex + 1)
                {
                    // We only want the entries that actually have a display name
                    continue;
                }

                if (tokens[DisplayNameIndex] == learnedRecipe)
                {
                    return data.Key;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Try to replace the recipe display name with the new name
        /// Will do nothing if the recipe isn't in the dictionary, which is okay
        /// </summary>
        /// <param name="menu">The letter menu</param>
        /// <param name="recipes">The map of recipes from CraftingRecipeAdjustments</param>
        /// <param name="recipeName">The English recipe name (the key in Data/CookingRecipes)</param>
        /// <returns>Whether it replaced the name or not</returns>
        private static bool TryReplaceRecipeDisplayName(
            LetterViewerMenu menu,
            Dictionary<string, ObjectIndexes> recipes, 
            string recipeName) 
        {
            if (recipes.TryGetValue(recipeName, out ObjectIndexes cookedItem))
            {
                menu.learnedRecipe = ItemList.Items[cookedItem].OverrideDisplayName;
                return true;
            }
            return false;
        }
    }
}
