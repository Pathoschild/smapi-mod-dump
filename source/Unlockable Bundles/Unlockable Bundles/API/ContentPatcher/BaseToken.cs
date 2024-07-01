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
using Unlockable_Bundles.Lib.WalletCurrency;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.API.ContentPatcher
{
    internal class BaseToken
    {
        public static bool RequiresContextUpdate { set {
                ContentPatcherHandling.DaysSincePurchaseToken.RequiresContextUpdate = value;
                ContentPatcherHandling.PersonalCurrencyToken.RequiresContextUpdate = value;
                ContentPatcherHandling.PersonalCurrencyTotalToken.RequiresContextUpdate = value;
                ContentPatcherHandling.CollectiveCurrencyToken.RequiresContextUpdate = value;
                ContentPatcherHandling.CollectiveCurrencyTotalToken.RequiresContextUpdate = value;
            } }
        public static bool Ready = false;


        /****
        ** State
        ****/
        /// <summary>Update the values when the context changes.</summary>
        /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
        public static bool UpdateContext(ref bool requiresContextUpdate)
        {
            //This may cause multiple context updates on daystart, but it's consistent
            //Won't rewrite for now
            if (SaveGame.loaded is not null && Context.IsMainPlayer)
                return true;

            if (requiresContextUpdate) {
                requiresContextUpdate = false;
                return true;
            }

            return false;
        }

        /// <summary>Get whether the token is available for use.</summary>
        public static bool IsReady()
        {
            if (SaveGame.loaded is null && !Context.IsWorldReady)
                return false;

            if (!Context.IsMainPlayer)
                return Ready;

            if (ModData.Instance is null)
                SaveDataEvents.LoadModData();

            return true;
        }

        public static IEnumerable<string> GetValidInputsForWalletCurrency()
        {
            var currencies = Helper.GameContent.Load<Dictionary<string, WalletCurrencyModel>>(WalletCurrencyHandler.Asset);
            foreach (var currency in currencies)
                yield return currency.Key;
        }
    }
}
