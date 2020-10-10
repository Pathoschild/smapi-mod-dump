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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

/***
 * Modified from https://github.com/Pathoschild/StardewMods/blob/3fb09f230e947202e0a66e0b9df47269e6374a57/Automate/Framework/Recipe.cs
 ***/
namespace SilentOak.QualityProducts.Processors
{
    public class Recipe
    {
        /*************
         * Properties
         *************/
        /// <summary>
        /// Gets the name of this recipe.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the total amount of minutes needed for processing.
        /// </summary>
        /// <value>Minutes for processing.</value>
        public int Minutes { get; }

        /// <summary>
        /// Gets the dictionary associating each possible ingredient ID with the amount required.
        /// </summary>
        /// <value>The ingredient choices.</value>
        public Dictionary<int, int> PossibleIngredients { get; }

        /// <summary>
        /// Gets the action to be executed when input does not have required stack size.
        /// </summary>
        /// <value>The fail.</value>
        public Action FailAmount { get; } = () => { };

        /// <summary>
        /// Gets the processing function that transforms ingredients into products.
        /// </summary>
        /// <value>The processing function.</value>
        public Func<SObject, SObject> Process { get; }

        /// <summary>
        /// Gets the action to be executed when input is consumed.
        /// </summary>
        /// <value>The input effects.</value>
        public Action<GameLocation, Vector2> InputEffects { get; }

        /// <summary>
        /// Gets the action to be executed during processing.
        /// </summary>
        /// <value>The working effects.</value>
        public Action<GameLocation, Vector2> WorkingEffects { get; }

        /// <summary>
        /// Instantiates a <see cref="T:QualityProducts.Processors.Recipe"/>.
        /// </summary>
        /// <param name="minutes">Total amount of minutes for processing.</param>
        /// <param name="process">Function that transforms the ingredient into the product.</param>
        /// <param name="failAmount">Action to be executed when input doesn't have required stack size.</param>
        /// <param name="inputEffects">Effects to be executed on input.</param>
        /// <param name="workingEffects">Effects to be executed during processing.</param>
        private Recipe(
            string name,
            int minutes,
            Func<SObject, SObject> process,
            Action failAmount,
            Action<GameLocation, Vector2> inputEffects,
            Action<GameLocation, Vector2> workingEffects
        )
        {
            Name = name;
            Minutes = minutes;
            Process = WithQuality(process);
            FailAmount = failAmount ?? FailAmount;
            InputEffects = inputEffects;
            WorkingEffects = workingEffects;
        }

        /// <summary>
        /// Instantiates a <see cref="T:QualityProducts.Processors.Recipe"/>.
        /// </summary>
        /// <param name="inputID">Ingredient ID for this recipe, can be either object ID or category ID.</param>
        /// <param name="inputAmount">How much ingredients are needed.</param>
        /// <param name="minutes">Total amount of minutes for processing.</param>
        /// <param name="process">Function that transforms the ingredient into the product.</param>
        /// <param name="failAmount">Action to be executed when input doesn't have required stack size.</param>
        /// <param name="inputEffects">Effects to be executed on input.</param>
        /// <param name="workingEffects">Effects to be executed during processing.</param>
        public Recipe(
            string name,
            int inputID,
            int inputAmount,
            int minutes,
            Func<SObject, SObject> process,
            Action failAmount = null,
            Action<GameLocation, Vector2> inputEffects = null,
            Action<GameLocation, Vector2> workingEffects = null
        ) : this(name, minutes, process, failAmount, inputEffects, workingEffects)
        {
            PossibleIngredients = new Dictionary<int, int>
            {
                { inputID, inputAmount }
            };
        }


        /// <summary>
        /// Instantiates a <see cref="T:QualityProducts.Processors.Recipe"/>.
        /// </summary>
        /// <param name="inputIDs">Possible inputs IDs.</param>
        /// <param name="inputAmount">Input amount.</param>
        /// <param name="minutes">Total amount of minutes for processing.</param>
        /// <param name="process">Function that transforms the ingredient into the product.</param>
        /// <param name="failAmount">Action to be executed when input doesn't have required stack size.</param>
        /// <param name="inputEffects">Effects to be executed on input.</param>
        /// <param name="workingEffects">Effects to be executed during processing.</param>
        public Recipe(
            string name,
            IEnumerable<int> inputIDs,
            int inputAmount,
            int minutes,
            Func<SObject, SObject> process,
            Action failAmount = null,
            Action<GameLocation, Vector2> inputEffects = null,
            Action<GameLocation, Vector2> workingEffects = null
        ) : this(name, minutes, process, failAmount, inputEffects, workingEffects)
        {
            PossibleIngredients = new Dictionary<int, int>();
            foreach (int id in inputIDs)
            {
                PossibleIngredients.Add(id, inputAmount);
            }
        }

        /// <summary>
        /// Instantiates a <see cref="T:QualityProducts.Processors.Recipe"/>.
        /// </summary>
        /// <param name="possibleIngredients">Possible ingredients with amounts.</param>
        /// <param name="minutes">Total amount of minutes for processing.</param>
        /// <param name="process">Function that transforms the ingredient into the product.</param>
        /// <param name="failAmount">Action to be executed when input doesn't have required stack size.</param>
        /// <param name="inputEffects">Effects to be executed on input.</param>
        /// <param name="workingEffects">Effects to be executed during processing.</param>
        public Recipe(
            string name,
            Dictionary<int, int> possibleIngredients,
            int minutes,
            Func<SObject, SObject> process,
            Action failAmount = null,
            Action<GameLocation, Vector2> inputEffects = null,
            Action<GameLocation, Vector2> workingEffects = null
        ) : this(name, minutes, process, failAmount, inputEffects, workingEffects)
        {
            PossibleIngredients = possibleIngredients;
        }

        /// <summary>
        /// Determines if the given object is a valid ingredient for the recipe.
        /// </summary>
        /// <returns><c>true</c>, if <paramref name="object"/> is an ingredient, <c>false</c> otherwise.</returns>
        /// <param name="object">Object.</param>
        public bool AcceptsInput(SObject @object)
        {
            return GetAmount(@object) > 0;
        }

        /// <summary>
        /// Gets the amount required for the given object as ingredient.
        /// </summary>
        /// <returns>The amount (0 if not needed).</returns>
        /// <param name="object">Object.</param>
        public int GetAmount(SObject @object)
        {
            if (!PossibleIngredients.TryGetValue(@object.ParentSheetIndex, out int amount))
            {
                PossibleIngredients.TryGetValue(@object.Category, out amount);
            }
            return amount;
        }

        /// <summary>
        /// Returns a function that executes the same process as the given one,
        /// but adds the ingredient's quality to the final product.
        /// </summary>
        /// <returns>The modified processing function.</returns>
        /// <param name="process">Function that transforms ingredients into products.</param>
        private static Func<SObject, SObject> WithQuality(Func<SObject, SObject> process)
        {
            return input =>
            {
                SObject output = process(input);
                output.Quality = input.Quality;
                return output;
            };
        }
    }
}
