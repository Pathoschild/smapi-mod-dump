/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Internal;
using StardewValley.Menus;

namespace MailServicesMod
{
    internal class GuildRecoveryController
    {
        private static readonly PerScreen<List<Item>> _itemsToRecover = new();

        internal const string RecoveryResponseKeyOne = "One";
        internal const string RecoveryResponseKeyAll = "All";
        internal const string RecoveryResponseKeyNone = "None";
        internal const string RecoveryDialogKey = "MailServiceMod_RecoveryOffer";

        internal static List<Item> GetItemsToRecover()
        {
            if (_itemsToRecover.Value == null)
            {
                Dictionary<ISalable, ItemStockInformation> adventureRecoveryStock = ShopBuilder.GetShopStock("AdventureGuildRecovery");

                int money = Game1.player.Money;
                Random random = new((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed * Game1.stats.DaysPlayed));

                List<Item> itemsToRecover = new();
                while (adventureRecoveryStock.Count > 0)
                {
                    KeyValuePair<ISalable, ItemStockInformation> itemAndValue = adventureRecoveryStock.ToList()[random.Next(0, adventureRecoveryStock.Count)];
                    adventureRecoveryStock.Remove(itemAndValue.Key);
                    RecoveryConfig recoveryConfig = DataLoader.GetRecoveryConfig(Game1.player);
                    if (itemAndValue.Value.Price <= money || recoveryConfig.RecoverForFree)
                    {
                        money -= itemAndValue.Value.Price;
                        itemsToRecover.Add((Item)itemAndValue.Key);
                        if (!recoveryConfig.RecoverAllItems) break;
                    }
                }
                if (itemsToRecover.Count > 0)
                {
                    _itemsToRecover.Value = itemsToRecover;
                }
            }
            return _itemsToRecover.Value;
        }

        /// <summary>
        /// To call when the items were recovered.
        /// Charge the player and clear the lost items list if needed.
        /// </summary>
        public static void ItemsRecovered()
        {
            Dictionary<ISalable, ItemStockInformation> adventureRecoveryStock = ShopBuilder.GetShopStock("AdventureGuildRecovery");
            RecoveryConfig recoveryConfig = DataLoader.GetRecoveryConfig(Game1.player);
            _itemsToRecover.Value.ForEach(item =>
            {
                if (!recoveryConfig.RecoverForFree)
                {
                    ShopMenu.chargePlayer(Game1.player, 0, adventureRecoveryStock[item].Price);
                }
                Game1.player.itemsLostLastDeath.Remove(item);
            });
            if (!recoveryConfig.DisableClearLostItemsOnRandomRecovery && !recoveryConfig.RecoverAllItems) Game1.player.itemsLostLastDeath.Clear();
            _itemsToRecover.Value = null;
        }

        public static void ClearItemsToRecover()
        {
            _itemsToRecover.Value = null;
        }

        internal static void OpenOfferDialog()
        {
            List<Response> options = new()
                        {
                            new Response(RecoveryResponseKeyOne, DataLoader.I18N.Get("Delivery.Marlon.RecoveryOffer.One")),
                            new Response(RecoveryResponseKeyAll, DataLoader.I18N.Get("Delivery.Marlon.RecoveryOffer.All")),
                            new Response(RecoveryResponseKeyNone, DataLoader.I18N.Get("Delivery.Marlon.RecoveryOffer.None"))
                        };
            Game1.player.currentLocation.createQuestionDialogue(DataLoader.I18N.Get("Delivery.Marlon.RecoveryOffer.Question"), options.ToArray(), RecoveryDialogKey);
        }
    }
}
