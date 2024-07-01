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
using Unlockable_Bundles.Lib.WalletCurrency;

namespace Unlockable_Bundles.API.ContentPatcher
{
    internal class PersonalCurrencyTotalToken
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

        public IEnumerable<string> GetValidInputs()
            => BaseToken.GetValidInputsForWalletCurrency();

        /// <summary>Get the current values.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
        public IEnumerable<string> GetValues(string input)
        {
            if (input == null)
                yield break;

            var currency = WalletCurrencyHandler.getCurrencyById(input, false);
            if (currency is null)
                yield break;

            var relevantPlayer = WalletCurrencyHandler.getRelevantPlayer(currency, Game1.player.UniqueMultiplayerID);
            var value = ModData.getWalletCurrencyTotal(currency.Id, relevantPlayer);

            yield return value.ToString();
        }
    }
}
