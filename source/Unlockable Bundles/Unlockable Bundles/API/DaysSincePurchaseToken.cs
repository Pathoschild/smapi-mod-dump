/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unlockable_Bundles.Lib;

namespace Unlockable_Bundles.API
{
    internal class DaysSincePurchaseToken
    {
        public bool RequiresContextUpdate = true;
        public bool Ready = false;


        /*********
        ** Public methods
        *********/
        /****
        ** Metadata
        ****/
        /// <summary>Get whether the token allows input arguments (e.g. an NPC name for a relationship token).</summary>
        public bool AllowsInput()
        {
            return true;
        }

        /// <summary>Whether the token may return multiple values for the given input.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
        public bool CanHaveMultipleValues(string input = null)
        {
            return false;
        }

        /****
        ** State
        ****/
        /// <summary>Update the values when the context changes.</summary>
        /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
        public bool UpdateContext()
        {
            //This may cause multiple context updates on daystart, but it's consistent
            //Won't rewrite for now
            if (SaveGame.loaded is not null && Context.IsMainPlayer)
                return true;

            if (RequiresContextUpdate) {
                RequiresContextUpdate = false;
                return true;
            }

            return false;
        }

        /// <summary>Get whether the token is available for use.</summary>
        public bool IsReady()
        {
            if (SaveGame.loaded is null && !Context.IsWorldReady)
                return false;

            if (!Context.IsMainPlayer)
                return Ready;

            if (ModData.Instance is null)
                SaveDataEvents.LoadModData();

            return true;
        }

        /// <summary>Get the current values.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
        public IEnumerable<string> GetValues(string input)
        {
            if (input == null)
                yield break;

            var days = ModData.getDaysSincePurchase(input);

            yield return days.ToString();
        }
    }
}
