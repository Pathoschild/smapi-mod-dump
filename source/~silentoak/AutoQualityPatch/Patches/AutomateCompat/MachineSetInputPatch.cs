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
using SilentOak.Patching;
using SilentOak.QualityProducts;
using SilentOak.QualityProducts.Processors;
using StardewModdingAPI;
using SObject = StardewValley.Object;

namespace SilentOak.AutoQualityPatch.Patches.AutomateCompat
{
    internal static class MachineSetInputPatch
    {
        public static readonly PatchData PatchData = new PatchData(
            assembly: typeof(IStorage).Assembly,
            typeNames: new string[] {
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.KegMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.LoomMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.PreservesJarMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MayonnaiseMachine",
                "Pathoschild.Stardew.Automate.Framework.Machines.Objects.OilMakerMachine"
            },
            originalMethodName: "SetInput",
            originalMethodParams: new Type[]
            {
                typeof(IStorage)
            }
        );

        public static bool Prefix(object __instance, ref bool __result, IStorage input)
        {
            IReflectedProperty<SObject> instanceMachine = ModEntry.StaticHelper.Reflection.GetProperty<SObject>(__instance, "Machine");
            if (instanceMachine.GetValue() is Processor processor)
            {
                IConsumable consumable = null;
                Recipe acceptingRecipe = processor.Recipes.FirstOrDefault(recipe =>
                    recipe.PossibleIngredients.Any(pair =>
                        input.TryGetIngredient(pair.Key, pair.Value, out consumable)
                    )
                );

                if (acceptingRecipe == null)
                {
                    __result = false;
                    return false;
                }

                processor.heldObject.Value = Processor.WithQuality(acceptingRecipe.Process)(consumable.Take() as SObject);
                processor.minutesUntilReady.Value = acceptingRecipe.Minutes;
                __result = true;
                return false;
            }

            return true;
        }
    }
}
