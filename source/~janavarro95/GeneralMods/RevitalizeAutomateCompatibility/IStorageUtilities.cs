/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.RevitalizeAutomateCompatibility.Recipes;
using Pathoschild.Stardew.Automate;

namespace Omegasis.RevitalizeAutomateCompatibility
{
    public static class IStorageUtilities
    {
        /// <summary>
        /// Function used to match finding a vanilla item from storage if it's parent sheet index matches.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static Func<ITrackedStack, bool> GetVanillaItemFromStorage(Enums.SDVObject itemId)
        {
            return new Func<ITrackedStack, bool>(itemStack => (itemStack.Sample is StardewValley.Object) && itemStack.Sample.ParentSheetIndex == (int)itemId && (itemStack.Sample as StardewValley.Object).bigCraftable.Value == false);
        }

        /// <summary>
        /// Function used to match finding a big craftable from storage if it's parent sheet index matches.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static Func<ITrackedStack, bool> GetBigCraftableFromStorage(Enums.SDVBigCraftable itemId)
        {
            return new Func<ITrackedStack, bool>(itemStack => (itemStack.Sample is StardewValley.Object) && itemStack.Sample.ParentSheetIndex == (int)itemId && (itemStack.Sample as StardewValley.Object).bigCraftable.Value == true);
        }

        /// <summary>
        /// Function used to match Revitalize's <see cref="CustomObject"/> and <see cref="CustomItem"/> ids.
        /// </summary>
        /// <param name="ItemId"></param>
        /// <returns></returns>
        public static Func<ITrackedStack, bool> GetICustomModObjectItemFromStorage(string ItemId)
        {
            return new Func<ITrackedStack, bool>(itemStack => IsValidCustomModObject(itemStack,ItemId));
        }

        public static bool IsValidCustomModObject(ITrackedStack TrackedStack,string ItemId)
        {
            if (TrackedStack == null)
            {
                Revitalize.RevitalizeModCore.log("Null Tracked stack!");
            }
            if (TrackedStack.Sample == null)
            {
                Revitalize.RevitalizeModCore.log("Null item in tracked stack!");
            }

            if((TrackedStack.Sample is ICustomModObject))
            {
                if((TrackedStack.Sample as ICustomModObject).Id.Equals(ItemId))
                {
                    return true;
                }
            }
            return false;


        }


        /// <summary>
        /// Extention to be able to specifically get a vanilla item from an <see cref="IStorage"/> provider.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="sdvObject"></param>
        /// <param name="RequiredAmount"></param>
        /// <param name="consumable"></param>
        /// <returns></returns>
        public static bool TryGetVanillaIngredient(this IStorage storage, Enums.SDVObject sdvObject, int RequiredAmount, out IConsumable? consumable)
        {
            return storage.TryGetIngredient(GetVanillaItemFromStorage(sdvObject), RequiredAmount, out consumable);
        }

        /// <summary>
        /// Extention to be able to specifically get a vanilla big craftable item from an <see cref="IStorage"/> provider.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="sdvObject"></param>
        /// <param name="RequiredAmount"></param>
        /// <param name="consumable"></param>
        /// <returns></returns>
        public static bool TryGetBigCraftableIngredient(this IStorage storage, Enums.SDVBigCraftable sdvObject, int RequiredAmount, out IConsumable? consumable)
        {
            return storage.TryGetIngredient(GetBigCraftableFromStorage(sdvObject), RequiredAmount, out consumable);
        }

        /// <summary>
        /// Extention to be able to specifically get a vanilla big craftable item from an <see cref="IStorage"/> provider.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="sdvObject"></param>
        /// <param name="RequiredAmount"></param>
        /// <param name="consumable"></param>
        /// <returns></returns>
        public static bool TryGetRevitalizeItemIngredient(this IStorage storage, string ItemId, int RequiredAmount, out IConsumable consumable)
        {
            Func<ITrackedStack, bool> matchingFunction = GetICustomModObjectItemFromStorage(ItemId);
            return storage.TryGetIngredient(matchingFunction, RequiredAmount, out consumable);
        }

        /// <summary>
        /// Tries to get all of the recipe inputs and consumes them if all present in order to "craft" a recipe.
        /// </summary>
        /// <param name="storage">The storage container provided by Automate</param>
        /// <param name="inputs">All of the ingredient inputs.</param>
        /// <param name="consumables">All of the items that have already been consumed.</param>
        /// <returns></returns>
        public static bool TryGetAndConsumeRecipeInputIngredients(this IStorage storage, params Recipes.RecipeInput[] inputs)
        {
            List<IConsumable?> consumables = new List<IConsumable?>();


            foreach (RecipeInput input in inputs)
            {
                if (input.sdvItem != Enums.SDVObject.NULL)
                {
                    TryGetVanillaIngredient(storage, input.sdvItem, input.amountRequired, out IConsumable? consumable);
                    consumables.Add(consumable);

                    continue;
                }
                if (input.sdvBigCraftable != Enums.SDVBigCraftable.NULL)
                {
                    TryGetBigCraftableIngredient(storage, input.sdvBigCraftable, input.amountRequired, out IConsumable? consumable);
                    consumables.Add(consumable);
                    continue;
                }
                if (input.revitalizeItemId != null)
                {
                    TryGetRevitalizeItemIngredient(storage, input.revitalizeItemId, input.amountRequired, out IConsumable? consumable);
                    consumables.Add(consumable);
                    continue;
                }
                return false;
            }

            foreach (IConsumable? consumable in consumables)
            {
                consumable.Reduce();
            }

            return true;

        }
    }
}
