/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomTokens
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace CustomTokens
{
    /// <summary>
    /// Provides necessary methods for tokens using the advanced api that can be overriden or provided a body
    /// </summary>
    internal abstract class BaseAdvancedToken
    {
        internal static readonly string main = "hostPlayer", local = "localPlayer";

        /// <summary>
        /// Get whether the token allows input arguments (e.g. an NPC name for a relationship token).
        /// </summary>
        public virtual bool AllowsInput()
        {
            return true;
        }

        /// <summary>
        /// Whether the token requires input arguments to work, and does not provide values without it.
        /// </summary>
        public virtual bool RequiresInput()
        {
            return true;
        }

        /// <summary>
        /// Whether the token may return multiple values for the given input.
        /// </summary>
		/// <param name="input">The input arguments, if applicable.</param>
        public virtual bool CanHaveMultipleValues(string input = null)
        {
            return false;
        }

        /// <summary>
        /// Get whether the token is available for use.
        /// </summary>
        public virtual bool IsReady()
        {
            return Context.IsWorldReady;
        }

        /// <summary>
        /// Update the values when the context changes.
        /// </summary>
        /// <returns>Returns whether the value changed.</returns>
        public virtual bool UpdateContext()
        {
            bool hasChanged = false;

            if (Context.IsWorldReady == true)
            {
                hasChanged = DidDataChange();
            }

            return hasChanged;
        }

        /// <summary>
        /// Get the current values.
        /// </summary>
        /// <param name="input">The input arguments, if applicable.</param>
        /// <returns>The retrieved values</returns>
        public abstract IEnumerable<string> GetValues(string input);

        /// <summary>Validate that the provided input arguments are valid.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
        /// <param name="error">The validation error, if validation fails.</param>
        /// <returns>Returns whether validation succeeded.</returns>
        public abstract bool TryValidateInput(string input, out string error);

        /// <summary>
        /// Get whether any data has changed.
        /// </summary>
        /// <returns>Whether the data has changed</returns>
        protected abstract bool DidDataChange();
    }
}
