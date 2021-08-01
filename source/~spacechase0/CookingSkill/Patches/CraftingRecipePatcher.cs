/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CookingSkill.Framework;
using HarmonyLib;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace CookingSkill.Patches
{
    /// <summary>Applies Harmony patches to <see cref="CraftingRecipe"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class CraftingRecipePatcher : BasePatcher
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether to actually consume items for the current recipe.</summary>
        public static bool ShouldConsumeItems { get; set; } = true;

        /// <summary>The items consumed by the last recipe, if any.</summary>
        public static IList<ConsumedItem> LastUsedItems { get; } = new List<ConsumedItem>();


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<CraftingRecipe>(nameof(CraftingRecipe.consumeIngredients)),
                prefix: this.GetHarmonyMethod(nameof(Before_ConsumeIngredients))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="CraftingRecipe.consumeIngredients"/>.</summary>
        /// <returns>Returns whether to run the original method.</returns>
        /// <remarks>This is copied verbatim from the original method with some changes (marked with comments).</remarks>
        public static bool Before_ConsumeIngredients(ref CraftingRecipe __instance, List<Chest> additional_materials)
        {
            CraftingRecipePatcher.LastUsedItems.Clear();
            var recipe = __instance;
            if (!recipe.isCookingRecipe)
                return true;

            for (int recipeIndex = recipe.recipeList.Count - 1; recipeIndex >= 0; --recipeIndex)
            {
                int requiredCount = recipe.recipeList[recipe.recipeList.Keys.ElementAt(recipeIndex)];
                bool foundInBackpack = false;
                for (int itemIndex = Game1.player.Items.Count - 1; itemIndex >= 0; --itemIndex)
                {
                    if (Game1.player.Items[itemIndex] is SObject obj && !obj.bigCraftable.Value && (obj.ParentSheetIndex == recipe.recipeList.Keys.ElementAt(recipeIndex) || obj.Category == recipe.recipeList.Keys.ElementAt(recipeIndex) || CraftingRecipe.isThereSpecialIngredientRule(obj, recipe.recipeList.Keys.ElementAt(recipeIndex))))
                    {
                        int toRemove = recipe.recipeList[recipe.recipeList.Keys.ElementAt(recipeIndex)];
                        requiredCount -= obj.Stack;

                        // custom code begins
                        CraftingRecipePatcher.LastUsedItems.Add(new ConsumedItem(obj));
                        if (CraftingRecipePatcher.ShouldConsumeItems)
                        {
                            // custom code ends
                            obj.Stack -= toRemove;
                            if (obj.Stack <= 0)
                                Game1.player.Items[itemIndex] = null;
                        }
                        if (requiredCount <= 0)
                        {
                            foundInBackpack = true;
                            break;
                        }
                    }
                }
                if (additional_materials != null && !foundInBackpack)
                {
                    foreach (Chest chest in additional_materials)
                    {
                        if (chest == null)
                            continue;

                        bool removedItem = false;
                        for (int materialIndex = chest.items.Count - 1; materialIndex >= 0; --materialIndex)
                        {
                            if (chest.items[materialIndex] != null && chest.items[materialIndex] is SObject && (chest.items[materialIndex].ParentSheetIndex == recipe.recipeList.Keys.ElementAt(recipeIndex) || chest.items[materialIndex].Category == recipe.recipeList.Keys.ElementAt(recipeIndex) || CraftingRecipe.isThereSpecialIngredientRule((SObject)chest.items[materialIndex], recipe.recipeList.Keys.ElementAt(recipeIndex))))
                            {
                                int removedCount = Math.Min(requiredCount, chest.items[materialIndex].Stack);
                                requiredCount -= removedCount;
                                // custom code begins
                                CraftingRecipePatcher.LastUsedItems.Add(new ConsumedItem(chest.items[materialIndex] as SObject));
                                if (CraftingRecipePatcher.ShouldConsumeItems)
                                {
                                    // custom code ends
                                    chest.items[materialIndex].Stack -= removedCount;
                                    if (chest.items[materialIndex].Stack <= 0)
                                    {
                                        chest.items[materialIndex] = null;
                                        removedItem = true;
                                    }
                                }
                                if (requiredCount <= 0)
                                    break;
                            }
                        }
                        if (removedItem)
                            chest.clearNulls();
                        if (requiredCount <= 0)
                            break;
                    }
                }
            }

            return false;
        }
    }
}
