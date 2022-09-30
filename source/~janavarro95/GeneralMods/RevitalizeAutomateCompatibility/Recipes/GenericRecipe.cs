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
using Omegasis.Revitalize.Framework.World.Objects;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Pathoschild.Stardew.Automate;
using StardewValley;

namespace Omegasis.RevitalizeAutomateCompatibility.Recipes
{
    /// <summary>
    /// TODO: Finish this class for compatibility with automate.
    /// </summary>
    public class GenericRecipe : IRecipe
    {
        /// <summary>The item type to accept, or <c>null</c> to accept any.</summary>
        public ItemType? Type { get; } = ItemType.Object;

        public GenericRecipe()
        {

        }

        public Func<Item, bool> Input {get;}

        public int InputCount { get; }

        public Func<Item, Item> Output { get; }

        public Func<Item, int> Minutes { get; }

        public List<Item> inputItems;
        public List<Item> outputItems;

        public GenericRecipe(Item ItemForRecipeInput, int inputAmount, Item ItemForRecipeOutput, int StackSizeOutputs ,int minutes)
        {
            this.Input = inputItem => this.MatchesInputId(inputItem);
            this.InputCount = inputAmount;
            this.Output = output=> {

                Item item = output.getOne();
                item.Stack = StackSizeOutputs;
                return item;
                };
            this.Minutes = _ => minutes;

            this.inputItems = new List<Item>() { ItemForRecipeInput };
            this.outputItems = new List<Item>() { ItemForRecipeOutput };
        }

        /// <summary>Get whether the recipe can accept a given item as input (regardless of stack size).</summary>
        /// <param name="stack">The item to check.</param>
        public bool AcceptsInput(ITrackedStack stack)
        {
            return
                (this.Type == null || stack.Type == this.Type)
                && this.Input(stack.Sample);
        }

        private bool MatchesInputId(Item itemToCheck)
        {
            foreach (Item input in this.inputItems)
            {
                if (!(itemToCheck is ICustomModObject))
                {
                    return itemToCheck.ParentSheetIndex == input.ParentSheetIndex || itemToCheck.Category == input.Category;
                }
                else
                {
                    ICustomModObject icustomObject = (input as ICustomModObject);
                    return (itemToCheck as ICustomModObject).Id.Equals(icustomObject.Id);
                }
            }
            return false;
        }
    }
}
