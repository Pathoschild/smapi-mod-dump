using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Provides an API to interact with the content of a <see cref="RecipeMail"/> instance.
    /// </summary>
    public class RecipeMailContent : MailContent, IRecipeMailContent
    {
        /// <summary>
        /// Create a new instance of the <see cref="RecipeMailContent"/> class.
        /// </summary>
        /// <param name="text">The text content of the mail.</param>
        /// <param name="recipe">The recipe attached to the mail. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        public RecipeMailContent(string text, RecipeData recipe)
            : base(text)
        {
            this.Recipe = recipe;
        }

        /// <summary>
        /// The recipe attached to the mail. Can be <c>null</c>.
        /// </summary>
        public RecipeData Recipe { get; set; }
    }
}
