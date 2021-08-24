/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using SatoCore.Extensions;
using System.Linq;

namespace SatoCore
{
    /// <summary>The base of a model that can be used in a <see cref="Repository{T, TIdentifier}"/>.</summary>
    public abstract class ModelBase
    {
        /*********
        ** Accessors
        *********/
        /// <summary>How the model data should be interpreted.</summary>
        public Action Action { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Retrieves the identifier of the item.</summary>
        /// <returns>The value of the identitifer, if one exists; otherwise, <see langword="null"/>.</returns>
        public object GetIdentifier() => this.GetType().GetIdentifierProperties().FirstOrDefault()?.GetValue(this);
    }
}
