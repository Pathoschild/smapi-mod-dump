using System.Collections.Generic;
using Pathoschild.Stardew.Automate;
using SilentOak.QualityProducts;
using SilentOak.QualityProducts.Processors;

namespace SilentOak.AutoQualityPatch
{
    internal static class RecipeManager
    {
        /*********
         * Fields
         *********/

        /// <summary>
        /// The recipe adaptors for each processor type.
        /// </summary>
        private static readonly Dictionary<Processor.ProcessorTypes, IRecipe[]> _recipeAdaptorsOfProcessorType = new Dictionary<Processor.ProcessorTypes, IRecipe[]>();


        /*************
         * Properties 
         *************/

        /// <summary>
        /// Gets the quality products API.
        /// </summary>
        /// <value>The quality products API.</value>
        internal static IQualityProductsAPI QualityProductsAPI { get; private set; }


        /*******************
         * Internal methods
         *******************/

        /// <summary>
        /// Initializes this class with the specified Quality Products API.
        /// </summary>
        /// <param name="qualityProductsAPI">Quality Products API.</param>
        internal static void Init(IQualityProductsAPI qualityProductsAPI)
        {
            QualityProductsAPI = qualityProductsAPI;
        }

        /// <summary>
        /// Gets the recipe adaptors for the corresponding processor.
        /// </summary>
        /// <returns>The recipe adaptors.</returns>
        /// <param name="processor">Processor.</param>
        internal static IRecipe[] GetRecipeAdaptorsFor(Processor processor, IRecipe[] fallbacks = null)
        {
            if (_recipeAdaptorsOfProcessorType.TryGetValue(processor.ProcessorType, out IRecipe[] recipes))
            {
                return recipes;
            }

            List<IRecipe> temporaryRecipeList = new List<IRecipe>(2*(fallbacks?.Length ?? 0));

            foreach (Recipe recipe in processor.Recipes)
            {
                if (QualityProductsAPI.Config.IsEnabled(recipe, processor))
                {
                    foreach (int inputId in recipe.PossibleIngredients.Keys)
                    {
                        temporaryRecipeList.Add(new RecipeAdaptor(recipe, inputId));
                    }
                }
            }

            temporaryRecipeList.AddRange(fallbacks);

            recipes = temporaryRecipeList.ToArray();
            _recipeAdaptorsOfProcessorType.Add(processor.ProcessorType, recipes);
            return recipes;
        }
    }
}
