using System;
using System.Reflection.Emit;

namespace StardewHack.CraftCounter
{
    public class ModEntry : Hack<ModEntry>
    {
        [BytecodePatch("StardewValley.CraftingRecipe::getDescriptionHeight")]
        [BytecodePatch("StardewValley.CraftingRecipe::drawRecipeDescription")]
        void AddTimesCrafted() {
            var range = FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(StardewValley.CraftingRecipe), "description")
            );
            range.Append(
                Instructions.Ldstr("\n\nTimes crafted: "),
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(StardewValley.CraftingRecipe),"timesCrafted"),
                Instructions.Call(typeof(Convert), "ToString", typeof(int)),
                Instructions.Call(typeof(string), "Concat", typeof(string), typeof(string), typeof(string))
            );
        }
    }
}

