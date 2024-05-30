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

namespace EscasModdingPlugins
{
    /// <summary>A Content Patcher token that accepts a player stat key and outputs the stat's current value.</summary>
    /// <remarks>This token targets the current local player, i.e. <see cref="Game1.player"/>.</remarks>
    public class PlayerStatToken
    {
        /* Private fields */

        /// <summary>A set of token inputs and outputs for the most recent context update.</summary>
        /// <remarks>Unlike other player stat tokens, this uses PerScreen caches. The output depends on the current local player, which varies in split-screen mode.</remarks>
        private PerScreen<Dictionary<string, uint>> InputOutputCache { get; set; } = new PerScreen<Dictionary<string, uint>>(() => new Dictionary<string, uint>());

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

        /** State methods **/

        /// <summary>Update the values when the context changes.</summary>
        /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
        public bool UpdateContext()
        {
            InputOutputCache.Value = new Dictionary<string, uint>(Game1.player.stats.Values); //update the cache of the local player's stats
            return true; //assume values have changed
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
                yield return "0";
            else if (InputOutputCache.Value.TryGetValue(input, out uint statValue)) //if the input stat has a cached value
            {
                yield return statValue.ToString(); //output it as a string (e.g. "0")
            }
            else //if the input stat does NOT have a cached value
            {
                yield return "0";
            }
        }
    }
}
