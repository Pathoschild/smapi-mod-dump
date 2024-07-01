/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData;
using System.IO;
using Unlockable_Bundles.Lib.AdvancedPricing;
using static Unlockable_Bundles.ModEntry;
using Unlockable_Bundles.Lib.WalletCurrency;
using StardewValley.GameData.Powers;
using System.Linq;

namespace Unlockable_Bundles.Lib
{
    public class AssetRequested
    {
        public static Dictionary<string, string> MailData = new Dictionary<string, string>();
        public static void Initialize()
        {
            Helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("UnlockableBundles/Bundles")) {
                //We need to provide an empty initial dictionary where others can append their changes
                e.LoadFrom(delegate () {
                    return new Dictionary<string, UnlockableModel>() { };
                }, AssetLoadPriority.Medium);

            } else if (e.NameWithoutLocale.IsEquivalentTo(AdvancedPricingItem.Asset)) {
                //We need to provide an empty initial dictionary where others can append their changes
                e.LoadFrom(delegate () {
                    return new Dictionary<string, AdvancedPricing.AdvancedPricing>() { };
                }, AssetLoadPriority.Medium);

            } else if (e.NameWithoutLocale.IsEquivalentTo(WalletCurrency.WalletCurrencyHandler.Asset)) {
                //We need to provide an empty initial dictionary where others can append their changes
                e.LoadFrom(delegate () {
                    return new Dictionary<string, WalletCurrency.WalletCurrencyModel>() { };
                }, AssetLoadPriority.Medium);

            } else if (e.NameWithoutLocale.StartsWith("UnlockableBundles/ShopTextures/")) {
                var asset = e.NameWithoutLocale.BaseName[31..];
                e.LoadFrom(delegate () {
                    return Helper.ModContent.Load<Texture2D>($"assets/ShopTextures/{asset}.png");
                }, AssetLoadPriority.Low);

            } else if (e.NameWithoutLocale.StartsWith("UnlockableBundles/UI/")) {
                var asset = e.NameWithoutLocale.BaseName[21..];
                e.LoadFrom(delegate () {
                    return Helper.ModContent.Load<Texture2D>($"assets/UI/{asset}.png");
                }, AssetLoadPriority.Low);

            } else if (e.NameWithoutLocale.IsEquivalentTo("Data/Mail")) {
                e.Edit(asset => {
                    var data = asset.AsDictionary<string, string>().Data;

                    foreach (var entry in MailData)
                        data.TryAdd(entry.Key, entry.Value);
                });

            } else if (e.NameWithoutLocale.IsEquivalentTo("Data/AudioChanges")) {
                e.Edit(asset => {
                    var data = asset.AsDictionary<string, AudioCueData>().Data;
                    data.Add("ub_pageflip", new AudioCueData() {
                        Id = "ub_pageflip",
                        Category = "Sound",
                        FilePaths = new() { Path.Combine(Helper.DirectoryPath, "assets", "pageflip.ogg") },
                    });
                });

            } else if(e.NameWithoutLocale.IsEquivalentTo("Data/Powers")) {
                var currencies = Helper.GameContent.Load<Dictionary<string, WalletCurrencyModel>>(WalletCurrencyHandler.Asset);
                e.Edit(asset => {
                    var data = asset.AsDictionary<string, PowersData>().Data;
                    KeyValuePair<string, PowersData>[] prependData = new KeyValuePair<string, PowersData>[data.Count + currencies.Count];

                    var i = 0;
                    foreach (var currency in currencies) {
                        if (currency.Value.PowersData.UnlockedCondition is null)
                            currency.Value.PowersData.UnlockedCondition = "UB_DISCOVERED_CURRENCY " + currency.Key;

                        prependData[i++] = new KeyValuePair<string, PowersData>(WalletCurrencyHandler.PowerPrefix + currency.Key, currency.Value.PowersData);
                    }

                    //I want the wallet currencies to be first in order, so they're easier to look up and their placement has consistency
                    data.CopyTo(prependData, i);
                    asset.AsDictionary<string, PowersData>().ReplaceWith(prependData.ToDictionary(e => e.Key, e => e.Value));
                });
            }
        }
    }
}
