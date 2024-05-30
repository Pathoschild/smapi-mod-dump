/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace EscasModdingPlugins
{
    /// <summary>A Content Patcher token that accepts a game state query and outputs the result.</summary>
    public class GameStateQueryToken
    {
        /* Private fields */

        /// <summary>A set of game state queries and results. If a specific query has already been checked since the last context update, the result will be cached here as the value.</summary>
        private PerScreen<Dictionary<string, bool>> QueryResultsCache { get; set; } = new PerScreen<Dictionary<string, bool>>(() => new Dictionary<string, bool>());

        /* Public methods */

        /** Metadata methods **/

        /// <summary>Get whether the token allows input arguments (e.g. an NPC name for a relationship token).</summary>
        public bool AllowsInput()
        {
            return true;
        }

        /// <summary>Whether the token requires input arguments to work, and does not provide values without it (see <see cref="AllowsInput"/>).</summary>
        public bool RequiresInput()
        {
            return true;
        }

        /// <summary>Whether the token may return multiple values for the given input.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
        public bool CanHaveMultipleValues(string input = null)
        {
            return false;
        }

        /// <summary>Get whether the token always chooses from a set of known values for the given input. Mutually exclusive with <see cref="HasBoundedRangeValues"/>.</summary>
        /// <param name="input">The input arguments, if any.</param>
        /// <param name="allowedValues">The possible values for the input.</param>
        /// <remarks>Default unrestricted.</remarks>
        public bool HasBoundedValues(string input, out IEnumerable<string> allowedValues)
        {
            allowedValues = new string[2] { "True", "False" };
            return true; //yes, this has bounded values
        }

        /// <summary>Validate that the provided input arguments are valid.</summary>
        /// <param name="input">The input arguments, if any.</param>
        /// <param name="error">The validation error, if any.</param>
        /// <returns>Returns whether validation succeeded.</returns>
        /// <remarks>Default true.</remarks>
        public bool TryValidateInput(string input, out string error)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                error = "GameStateQuery input was null or blank.";
                return false;
            }

            if (QueryResultsCache.Value.ContainsKey(input)) //if this input was already validated
            {
                error = null;
                return true;
            }

            foreach (var parsedQueryComponent in GameStateQuery.Parse(input)) //try to parse the input into an array of queries
            {
                if (parsedQueryComponent.Error != null) //if a parsing error occurred
                {
                    error = $"GameStateQuery input could not be parsed: {parsedQueryComponent.Error}";
                    return false;
                }
            }

            error = null;
            return true;
        }

        /** State methods **/

        /// <summary>Update the values when the context changes.</summary>
        /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
        public bool UpdateContext()
        {
            bool anyResultsChanged = false;

            foreach (var query in QueryResultsCache.Value.Keys.ToList()) //for each cached query
            {
                var newResult = GameStateQuery.CheckConditions(query); //check the query and get its current result
                if (QueryResultsCache.Value[query] != newResult) //if the result has changed since the last update
                {
                    anyResultsChanged = true;
                    QueryResultsCache.Value[query] = newResult; //update the cached result
                }
            }

            return anyResultsChanged;
        }

        /// <summary>Get whether the token is available for use.</summary>
        public bool IsReady()
        {
            return Context.IsWorldReady;
        }

        /// <summary>Get the current values.</summary>
        /// <param name="input">The input arguments, if any.</param>
        public IEnumerable<string> GetValues(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                yield return "False";
            }
            else
            {
                if (QueryResultsCache.Value.ContainsKey(input) == false) //if no cached result exists for this query
                {
                    bool newResult = GameStateQuery.CheckConditions(input); //check the query and get its current result
                    QueryResultsCache.Value[input] = newResult; //cache it
                }

                yield return QueryResultsCache.Value[input].ToString(); //return the result as a string ("True" or "False")
            }
        }
    }
}
