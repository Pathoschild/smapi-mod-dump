using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Represents a Stardew Valley game letter with an optionally attached recipe.
    /// </summary>
    public class RecipeMail : Mail, IRecipeMailContent
    {
        /// <summary>
        /// Create a new instance of the <see cref="RecipeMail"/> class.
        /// </summary>
        /// <param name="id">The ID of the mail.</param>
        /// <param name="text">The text content of the mail.</param>
        /// <param name="recipe">The recipe attached to the mail. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentException">The specified <paramref name="id"/> is <c>null</c>, empty or contains only whitespace characters.</exception>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        public RecipeMail(string id, string text, RecipeData recipe)
            : base(id, text)
        {
            this.Recipe = recipe;
        }

        /// <summary>
        /// The recipe to attach to the mail. Can be <c>null</c>.
        /// </summary>
        public RecipeData Recipe { get; set; }
    }
}
