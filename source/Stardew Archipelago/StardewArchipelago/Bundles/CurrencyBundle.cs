/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StardewArchipelago.Stardew;

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

        public override string GetNumberRequiredItemsWithSeparator()
        {
            return "";
        }

        public static readonly Dictionary<string, int> CurrencyIds = new()
        {
            {"Money", -1}, 
            {"Star Token", -2},
            {"Qi Coin", -3},
            {"Golden Walnut", -4},
            {"Qi Gem", -5}
        };
    }
}
