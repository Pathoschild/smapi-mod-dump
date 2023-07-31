/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Graphics;

namespace Unlockable_Bundles.Lib
{
    public class AssetRequested
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static Dictionary<string, string> MailData = new Dictionary<string, string>();
        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            Helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("UnlockableBundles/Bundles")) {
                //We need to provide an empty initial dictionary where others can append their changes
                e.LoadFrom(delegate () {
                    return new Dictionary<string, UnlockableModel>() { };
                }, AssetLoadPriority.Medium);

            } else if (e.NameWithoutLocale.StartsWith("UnlockableBundles/ShopTextures/")) {
                var asset = e.NameWithoutLocale.BaseName[31..];
                e.LoadFrom(delegate () {
                    return Helper.ModContent.Load<Texture2D>($"assets/{asset}.png");
                }, AssetLoadPriority.Low);

            } else if(e.NameWithoutLocale.IsEquivalentTo("Data/Mail")) {
                e.Edit(asset => {
                    var data = asset.AsDictionary<string, string>().Data;

                    foreach (var entry in MailData)
                        data.TryAdd(entry.Key, entry.Value);
                });

            }
        }
    }
}
