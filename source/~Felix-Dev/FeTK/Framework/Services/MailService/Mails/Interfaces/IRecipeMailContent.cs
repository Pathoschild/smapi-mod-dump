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
    public interface IRecipeMailContent : IMailContent
    {
        /// <summary>
        /// The recipe attached to the mail.
        /// </summary>
        RecipeData Recipe { get; set; }
    }
}
