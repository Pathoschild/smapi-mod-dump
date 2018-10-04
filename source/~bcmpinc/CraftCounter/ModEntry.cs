using System;
using System.Reflection.Emit;

namespace StardewHack.CraftCounter
{
    public class ModEntry : Hack<ModEntry>
    {
        public static string AddTimesCraftedText(string description, StardewValley.CraftingRecipe recipe) {
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
                Instructions.Ldfld(typeof(StardewValley.CraftingRecipe), "description")
            );
            range.Append(
                Instructions.Ldarg_0(),
                Instructions.Call(typeof(ModEntry), "AddTimesCraftedText", typeof(string), typeof(StardewValley.CraftingRecipe))
            );
        }
        
        // Update number of times crafted when crafting.
        [BytecodePatch("StardewValley.CraftingRecipe::consumeIngredients")]
        void UpdateCounter() {
            BeginCode().Append(
                Instructions.Ldarg_0(),
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(StardewValley.CraftingRecipe),"numberProducedPerCraft"),
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(StardewValley.CraftingRecipe),"timesCrafted"),
                Instructions.Add(),
                Instructions.Stfld(typeof(StardewValley.CraftingRecipe),"timesCrafted")
            );
        }
        
        public static int GetCookedCounter(int item_key) {
            return StardewValley.Game1.player.recipesCooked.ContainsKey(item_key) ? StardewValley.Game1.player.recipesCooked[item_key] : 0;
        }
        
        // Initialize timesCrafted also for recipies.
        [BytecodePatch("StardewValley.CraftingRecipe::.ctor(System.String,System.Boolean)")]
        void SetCookedCounter() {
            // Obtain the Item key of the recipe product
            FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(StardewValley.CraftingRecipe), "itemToProduce"),
                OpCodes.Ldloc_3,
                OpCodes.Ldloc_S,
                OpCodes.Ldelem_Ref,
                Instructions.Call(typeof(Convert), "ToInt32", typeof(string))
            ).Append(
                Instructions.Stloc_S(4),
                Instructions.Ldloc_S(4)
            );
            // If this is a cooking recipe, obtain how often it has been crafted.
            var range = FindCode(
                Instructions.Stfld(typeof(StardewValley.CraftingRecipe),"timesCrafted")
            ).End;
            range.Append(
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(StardewValley.CraftingRecipe), "isCookingRecipe"),
                Instructions.Brfalse(AttachLabel(range[1])),
                Instructions.Ldarg_0(),
                Instructions.Ldloc_S(4),
                Instructions.Call(typeof(ModEntry), "GetCookedCounter", typeof(int)),
                Instructions.Stfld(typeof(StardewValley.CraftingRecipe),"timesCrafted")
            );
        }
    }
}

