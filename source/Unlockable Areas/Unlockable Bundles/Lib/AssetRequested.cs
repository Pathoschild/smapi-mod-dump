/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
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
                return;

            } else if (e.NameWithoutLocale.IsEquivalentTo("UnlockableBundles/ShopTextures/Sign")) {
                e.LoadFrom(delegate () {
                    return Helper.ModContent.Load<Texture2D>($"assets/Sign.png");
                }, AssetLoadPriority.Low);
            } else if (e.NameWithoutLocale.IsEquivalentTo("UnlockableBundles/ShopTextures/CCBundle")) {
                e.LoadFrom(delegate () {
                    return Helper.ModContent.Load<Texture2D>($"assets/CCBundle.png");
                }, AssetLoadPriority.Low);
            } else if (e.NameWithoutLocale.IsEquivalentTo("UnlockableBundles/ShopTextures/Scroll")) {
                e.LoadFrom(delegate () {
                    return Helper.ModContent.Load<Texture2D>($"assets/Scroll.png");
                }, AssetLoadPriority.Low);
            } else if (e.NameWithoutLocale.IsEquivalentTo("UnlockableBundles/ShopTextures/Blue_Junimo")) {
                e.LoadFrom(delegate () {
                    return Helper.ModContent.Load<Texture2D>($"assets/Blue_Junimo.png");
                }, AssetLoadPriority.Low);
            } else if (e.NameWithoutLocale.IsEquivalentTo("UnlockableBundles/ShopTextures/ParrotPerch")) {
                e.LoadFrom(delegate () {
                    return Helper.ModContent.Load<Texture2D>($"assets/ParrotPerch.png");
                }, AssetLoadPriority.Low);
            }
        }
    }
}
