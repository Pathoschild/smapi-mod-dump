/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
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

namespace Unlockable_Bundles.API.ContentPatcher
{
    internal class DaysSincePurchaseToken
    {
        public bool RequiresContextUpdate = true;

        public bool RequiresInput()
            => true;

        public bool CanHaveMultipleValues(string input = null)
            => false;

        public bool UpdateContext()
            => BaseToken.UpdateContext(ref RequiresContextUpdate);

        public bool IsReady()
            => BaseToken.IsReady();

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
