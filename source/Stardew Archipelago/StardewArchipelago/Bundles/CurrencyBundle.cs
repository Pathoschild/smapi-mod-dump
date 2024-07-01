/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using StardewArchipelago.Constants;

namespace StardewArchipelago.Bundles
{
    public class CurrencyBundle : Bundle
    {
        public string Currency { get; }
        public int Amount { get; set; }

        public CurrencyBundle(string roomName, string bundleName, Dictionary<string, string> bundleContent) : base(roomName, bundleName)
        {
            foreach (var (key, itemDetails) in bundleContent)
            {
                if (key == NUMBER_REQUIRED_KEY)
                {
                    continue;
                }

                var itemFields = itemDetails.Split("|");
                Currency = itemFields[0];
                Amount = int.Parse(itemFields[1]);
                return;
            }
        }

        public override string GetItemsString()
        {
            return $"{CurrencyIds[Currency]} {Amount} {Amount}";
        }

        public override string GetNumberRequiredItems()
        {
            return "";
        }

        public static readonly Dictionary<string, string> CurrencyIds = new()
        {
            { "Money", IDProvider.MONEY },
            { "Star Token", IDProvider.STAR_TOKEN },
            { "Qi Coin", IDProvider.QI_COIN },
            // {"Golden Walnut", IDProvider.GOLDEN_WALNUT},
            { "Qi Gem", IDProvider.QI_GEM },
        };
    }
}
