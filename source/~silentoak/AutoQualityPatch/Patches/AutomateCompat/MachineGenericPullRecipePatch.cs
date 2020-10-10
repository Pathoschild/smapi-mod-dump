/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

using System;
using System.Linq;
using Pathoschild.Stardew.Automate;
using SilentOak.AutoQualityPatch.Utils;
using SilentOak.Patching;
using SilentOak.QualityProducts;
using StardewModdingAPI;
using SObject = StardewValley.Object;

namespace SilentOak.AutoQualityPatch.Patches.AutomateCompat
{
    /// <summary>
    /// Patch for Automate compatibility.
    /// </summary>
    internal static class MachineGenericPullRecipePatch
    {
        public static PatchData PatchData = new PatchData(
            assembly: typeof(IRecipe).Assembly,
            typeNames: new string[] 
            {
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.KegMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.LoomMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MayonnaiseMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.PreservesJarMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.OilMakerMachine"
            },
            originalMethodName: "SetInput",
            originalMethodParams: new Type[]
            {
                typeof(IStorage)
            }
        );
        
        /// <summary>
        /// Replaces the recipes with the ones defined by the processor.
        /// </summary>
        public static bool Prefix(IMachine __instance, ref bool __result, IStorage input)
        {
            IReflectedProperty<SObject> instanceMachine = Util.Helper.Reflection.GetProperty<SObject>(__instance, "Machine");
            if (instanceMachine.GetValue() is Processor processor)
            {
                IReflectedField<IRecipe[]> privateRecipes = Util.Helper.Reflection.GetField<IRecipe[]>(__instance, "Recipes");

                IRecipe[] recipes = RecipeManager.GetRecipeAdaptorsFor(processor, privateRecipes?.GetValue());

                IConsumable consumable = null;
                IRecipe acceptingRecipe = null;

                foreach (ITrackedStack item in input.GetItems())
                {
                    acceptingRecipe = recipes.FirstOrDefault(recipe => recipe.AcceptsInput(item));
                    if (acceptingRecipe != null)
                    {
                        input.TryGetIngredient(item.Sample.ParentSheetIndex, acceptingRecipe.InputCount, out consumable);
                        break;
                    }
                }

                if (acceptingRecipe != null)
                {
                    processor.heldObject.Value = acceptingRecipe.Output(consumable.Take());
                    processor.MinutesUntilReady = acceptingRecipe.Minutes;
                    __result = true;
                    return false;
                }

                __result = false;
                return false;
            }

            return true;
        }
    }
}
