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
    public class ItemBundle : Bundle
    {
        public int NumberRequired { get; set; }
        public List<BundleItem> Items { get; }

        public ItemBundle(StardewItemManager itemManager, string roomName, string bundleName, Dictionary<string, string> bundleContent) : base(roomName, bundleName)
        {
            NumberRequired = int.Parse(bundleContent[NUMBER_REQUIRED_KEY]);
            Items = new List<BundleItem>();
            foreach (var (key, itemDetails) in bundleContent)
            {
                if (key == NUMBER_REQUIRED_KEY)
                {
                    continue;
                }

                var itemFields = itemDetails.Split("|");
                var itemName = itemFields[0];
                var amount = int.Parse(itemFields[1]);
                var quality = itemFields[2].Split(" ")[0];
                var bundleItem = new BundleItem(itemManager, itemName, amount, quality);
                Items.Add(bundleItem);
            }
        }

        public override string GetItemsString()
        {
            var itemsString = "";
            foreach (var item in Items)
            {
                itemsString += $" {item.StardewObject.Id} {item.Amount} {item.Quality}";
            }

            return itemsString.Trim();
        }

        public override string GetNumberRequiredItemsWithSeparator()
        {
            if (NumberRequired == Items.Count)
            {
                return "";
            }

            return $"/{NumberRequired}";
        }
    }
}
