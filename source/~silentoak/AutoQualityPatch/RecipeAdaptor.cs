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
using Pathoschild.Stardew.Automate;
using SilentOak.QualityProducts.Processors;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.AutoQualityPatch
{
    internal class RecipeAdaptor : Pathoschild.Stardew.Automate.IRecipe
    {
        /*********
         * Fields
         *********/

        /// <summary>
        /// The underlying recipe.
        /// </summary>
        private readonly SilentOak.QualityProducts.Processors.Recipe _underlying;


        /*************
         * Properties
         *************/
                 
        /// <summary>
        /// Gets the input identifier.
        /// </summary>
        /// <value>The input identifier.</value>
        public int InputID { get; }

        /// <summary>
        /// Gets the input count.
        /// </summary>
        /// <value>The input count.</value>
        public int InputCount => _underlying.PossibleIngredients[InputID];

        /// <summary>
        /// Gets the output (given an input).
        /// </summary>
        /// <value>The output.</value>
        public Func<Item, SObject> Output => item => _underlying.Process(item as SObject);

        /// <summary>
        /// Gets the recipe's total preparing time in minutes.
        /// </summary>
        /// <value>The minutes.</value>
        public int Minutes => _underlying.Minutes;


        /*****************
         * Public methods
         *****************/

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SilentOak.AutoQualityPatch.RecipeAdaptor"/> class.
        /// </summary>
        /// <param name="recipe">Recipe.</param>
        /// <param name="inputID">Input identifier.</param>
        public RecipeAdaptor(Recipe recipe, int inputID)
        {
            _underlying = recipe;
            InputID = inputID;
        }

        /// <summary>
        /// Checks if this recipe accepts the given input.
        /// </summary>
        /// <returns><c>true</c>, if input is accepted, <c>false</c> otherwise.</returns>
        /// <param name="stack">Stack.</param>
        public bool AcceptsInput(ITrackedStack stack)
        {
            if (stack.Sample is SObject @object)
            {
                return _underlying.AcceptsInput(@object);
            }
            return false;
        }
    }
}
