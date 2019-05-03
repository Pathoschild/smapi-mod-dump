using System;
using System.Collections.Generic;
using System.Linq;
using SilentOak.QualityProducts.Extensions;
using SilentOak.QualityProducts.Processors;
using StardewValley;

namespace SilentOak.QualityProducts
{
    public class QualityProductsConfig
    {
        /*********
         * Fields
         *********/

        /// <summary>
        /// Gets or sets the list of items to have quality enabled.
        /// </summary>
        /// <value>Items to have quality enabled.</value>
        public ICollection<string> EnableQuality { get; set; }

        /// <summary>
        /// Gets or sets the list of items to have quality disabled.
        /// </summary>
        /// <value>Items to have quality disabled.</value>
        public ICollection<string> DisableQuality { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether custom mead texture should be enabled.
        /// </summary>
        /// <value><c>true</c> if custom mead textures should be enabled; otherwise, <c>false</c>.</value>
        public bool EnableMeadTextures { get; set; } = true;

        /// <summary>
        /// Gets or sets the texture for mead types.
        /// </summary>
        /// <value>The texture for mead types.</value>
        public string TextureForMeadTypes { get; set; } = "assets/mead-coloredbottles.png";


        /**********
         * Public methods
         **********/

        /// <summary>
        /// Whether the <see cref="EnableQuality"/> property should be serialized.
        /// </summary>
        /// <returns><c>true</c>, if serialize enable quality was shoulded, <c>false</c> otherwise.</returns>
        public bool ShouldSerializeEnableQuality()
        {
            return EnableQuality != null;
        }

        /// <summary>
        /// Whether the <see cref="DisableQuality"/> property should be serialized.
        /// </summary>
        /// <returns><c>true</c>, if serialize disable quality was shoulded, <c>false</c> otherwise.</returns>
        public bool ShouldSerializeDisableQuality()
        {
            return DisableQuality != null;
        }


        /*******************
         * Internal methods 
         *******************/

        /***
         * Think thoroughly before changing the logic below      
         ***/       

        /// <summary>
        /// Checks if the given processor is enabled by the config.
        /// </summary>
        /// <returns><c>true</c>, if processor is enabled, <c>false</c> otherwise.</returns>
        /// <param name="processor">Processor.</param>
        public bool IsEnabled(Processor processor)
        {
            if (!EnableQuality.IsNullOrEmpty())
            {
                return EnableQuality.Contains("All") || EnableQuality.Contains(processor.Name) || processor.Recipes.Any(recipe => EnableQuality.Contains(recipe.Name));
            }
            if (!DisableQuality.IsNullOrEmpty())
            {
                return !DisableQuality.Contains("All") && !DisableQuality.Contains(processor.Name) && processor.Recipes.Any(recipe => !DisableQuality.Contains(recipe.Name));
            }
            return true;
        }

        /// <summary>
        /// Checks if the given recipe produced by the given processor is enabled by the config. 
        /// </summary>
        /// <returns><c>true</c>, if recipe is enabled, <c>false</c> otherwise.</returns>
        /// <param name="recipe">Recipe.</param>
        /// <param name="processor">Processor that produces the recipe.</param>
        public bool IsEnabled(Recipe recipe, Processor processor)
        {
            if (!EnableQuality.IsNullOrEmpty())
            {
                return EnableQuality.Contains("All") ||  EnableQuality.Contains(processor.Name) || EnableQuality.Contains(recipe.Name);
            }
            if (!DisableQuality.IsNullOrEmpty())
            {
                return !DisableQuality.Contains("All") && !DisableQuality.Contains(processor.Name) && !DisableQuality.Contains(recipe.Name);
            }
            return true;
        }

        /// <summary>
        /// Checks if cooking is enabled.
        /// </summary>
        /// <returns><c>true</c>, if cooking is enabled, <c>false</c> otherwise.</returns>
        public bool IsCookingEnabled()
        {
            if (!EnableQuality.IsNullOrEmpty())
            {
                return EnableQuality.Contains("All") || EnableQuality.Contains("Cooking");
            }
            if (!DisableQuality.IsNullOrEmpty())
            {
                return !DisableQuality.Contains("All") && !DisableQuality.Contains("Cooking");
            }
            return true;
        }


        /// <summary>
        /// Checks if anything is enabled at all.
        /// </summary>
        /// <returns><c>true</c>, if anything is enabled, <c>false</c> otherwise.</returns>
        public bool IsAnythingEnabled()
        {
            if (!EnableQuality.IsNullOrEmpty())
            {
                return true;
            }

            if (!DisableQuality.IsNullOrEmpty())
            {
                return !DisableQuality.Contains("All");
            }

            return true;
        }
    }
}
