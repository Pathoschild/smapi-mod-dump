using System;
using System.Reflection.Emit;
using StardewValley;
using StardewValley.Menus;

namespace StardewHack.CraftCounter
{
    public class ModEntry : Hack<ModEntry>
    {
        public static string AddTimesCraftedText(string description, CraftingRecipe recipe) {
            string prefix = recipe.isCookingRecipe
                ? "\n\nTimes Cooked: "
                : "\n\nTimes Crafted: ";
            return description + prefix + recipe.timesCrafted;
        }
    
        // Adds number of times crafted to the tooltip.
        [BytecodePatch("StardewValley.CraftingRecipe::getDescriptionHeight")]
        [BytecodePatch("StardewValley.CraftingRecipe::drawRecipeDescription")]
        void AddTimesCrafted() {
            var range = FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(CraftingRecipe), "description")
            );
            range.Append(
                Instructions.Ldarg_0(),
                Instructions.Call(typeof(ModEntry), nameof(AddTimesCraftedText), typeof(string), typeof(CraftingRecipe))
            );
        }
        
        // Update number of times crafted when crafting.
        [BytecodePatch("StardewValley.CraftingRecipe::consumeIngredients")]
        void UpdateCounter() {
            BeginCode().Append(
                Instructions.Ldarg_0(),
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(CraftingRecipe), nameof(CraftingRecipe.numberProducedPerCraft)),
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(CraftingRecipe), nameof(CraftingRecipe.timesCrafted)),
                Instructions.Add(),
                Instructions.Stfld(typeof(CraftingRecipe), nameof(CraftingRecipe.timesCrafted))
            );
        }
        
        public static void GetCookedCounter(CraftingRecipe recipe, Item item) {
            if (recipe.isCookingRecipe) {
                var item_key = item.ParentSheetIndex;
                recipe.timesCrafted = Game1.player.recipesCooked.ContainsKey(item_key) ? Game1.player.recipesCooked[item_key] : 0;
            }
        }
        
        // Initialize timesCrafted also for recipies.
        // We do this in the performHoverAction method, because I had some issues with Harmony patching constructors.
        [BytecodePatch("StardewValley.Menus.CraftingPage::performHoverAction(System.Int32,System.Int32)")]
        void CraftingPage_performHoverAction() {
            // Obtain the Item key of the recipe product
            FindCode(
                OpCodes.Ldarg_0,
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(CraftingPage), "hoverRecipe"),
                Instructions.Callvirt(typeof(CraftingRecipe), nameof(CraftingRecipe.createItem)),
                Instructions.Stfld(typeof(CraftingPage), "lastCookingHover")
            ).Append(
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(CraftingPage), "hoverRecipe"),
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(CraftingPage), "lastCookingHover"),
                Instructions.Call(typeof(ModEntry), nameof(GetCookedCounter), typeof(CraftingRecipe), typeof(Item))
            );
        }
    }
}

