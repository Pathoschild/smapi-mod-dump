using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Encapsulates player interaction data specific to a <see cref="RecipeMail"/> instance.
    /// </summary>
    public class RecipeMailInteractionRecord : MailInteractionRecord
    {
        /// <summary>
        /// Create a new instance of the <see cref="RecipeMailInteractionRecord"/> class.
        /// </summary>
        /// <param name="recipe">The recipe received by the player. Can be <c>null</c>.</param>
        public RecipeMailInteractionRecord(RecipeData recipe)
        {
            this.Recipe = recipe;
        }

        /// <summary>
        /// The recipe which was received by the player.
        /// </summary>
        public RecipeData Recipe { get; }
    }
}
