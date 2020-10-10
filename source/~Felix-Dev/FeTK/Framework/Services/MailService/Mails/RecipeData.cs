/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// The class <see cref="RecipeData"/> encapsulates infomration about a recipe.
    /// </summary>
    public class RecipeData
    {
        /// <summary>The name of the recipe attached to the mail.</summary>
        private string name;

        /// <summary>The type of the recipe attached to the mail.</summary>
        private RecipeType type;

        /// <summary>
        /// Create a new instance of the <see cref="RecipeData"/> class.
        /// </summary>
        /// <param name="name">The name of the recipe attached to the mail.</param>
        /// <param name="type">The type of the recipe attached to the mail.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="type"/> is invalid.</exception>
        public RecipeData(string name, RecipeType type)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));

            if (!Enum.IsDefined(typeof(RecipeType), type))
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            this.type = type;
        }

        /// <summary>
        /// The name of the recipe included in the mail.
        /// </summary>
        /// <exception cref="ArgumentNullException">The specified recipe name is <c>null</c>.</exception>
        public string Name
        {
            get => name;
            set => name = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// The type of the recipe included in the mail.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The specified recipe type is invalid.</exception>
        public RecipeType Type
        {
            get => type;
            set
            {
                if (!Enum.IsDefined(typeof(RecipeType), value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                type = value;
            }
        }
    }
}
