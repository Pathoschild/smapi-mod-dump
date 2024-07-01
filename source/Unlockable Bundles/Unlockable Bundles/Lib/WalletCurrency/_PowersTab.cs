/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.Lib.WalletCurrency
{
    public class _PowersTab
    {
        public static void Initialize()
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(PowersTab), nameof(PowersTab.draw), new[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(_PowersTab), nameof(_PowersTab.PowersTab_Draw_Postfix))
            );

            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(PowersTab), nameof(PowersTab.draw), new[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(_PowersTab), nameof(_PowersTab.PowersTab_Draw_Postfix))
            );
        }

        public static void PowersTab_Draw_Postfix(SpriteBatch b, PowersTab __instance)
        {
            WalletCurrencyModel hoverCurrency = null;

            var currencies = Helper.GameContent.Load<Dictionary<string, WalletCurrencyModel>>(WalletCurrencyHandler.Asset);
            foreach (var item in __instance.powers[__instance.currentPage]) {
                if (!item.drawShadow)
                    continue;

                //I don't have access to the powers id here, so I compare all other fields, which are still unique
                //Not ideal, but better than crazy harmony transpilers
                var currency = currencies.FirstOrDefault(el =>
                    Helper.GameContent.ParseAssetName(el.Value.PowersData.TexturePath).IsEquivalentTo(item.texture.Name)
                    && el.Value.PowersData.TexturePosition.X == item.sourceRect.X
                    && el.Value.PowersData.TexturePosition.Y == item.sourceRect.Y
                    && TokenParser.ParseText(el.Value.PowersData.DisplayName) == item.name);

                if (currency.Value is null)
                    continue;

                currency.Value.Id = currency.Key;
                var relevantPlayer = WalletCurrencyHandler.getRelevantPlayer(currency.Value, Game1.player.UniqueMultiplayerID);
                var value = ModData.getWalletCurrency(currency.Key, relevantPlayer);
                UtilityMisc.drawKiloFormat(b, value, item.bounds.X + 2, item.bounds.Y + 2, Color.White);

                if (item.bounds.Contains(Game1.getMousePosition()))
                    hoverCurrency = currency.Value;
            }

            if (hoverCurrency is not null) {
                var relevantPlayer = WalletCurrencyHandler.getRelevantPlayer(hoverCurrency, Game1.player.UniqueMultiplayerID);
                WalletCurrencyBillboard.ShowCurrency(hoverCurrency, relevantPlayer, true);
            } else
                WalletCurrencyBillboard.StopForceShow();

        }
    }
}
