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
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unlockable_Bundles.NetLib;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.Lib.WalletCurrency
{
    public class WalletCurrencyHandler
    {
        public const string Asset = "UnlockableBundles/WalletCurrencies";
        public const string PowerPrefix = "UB_Currency.";
        public static List<AnimatedTexture> OverheadAnimations = new();
        public static WalletCurrencyBillboard Display = new();

        public static void Initialize()
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Farmer), nameof(Farmer.OnItemReceived), new[] { typeof(Item), typeof(int), typeof(Item), typeof(bool) }),
                postfix: new HarmonyMethod(typeof(WalletCurrencyHandler), nameof(WalletCurrencyHandler.OnItemReceived_Postfix))
            );

            Helper.Events.Display.RenderedWorld += Draw;
            GameStateQuery.Register("UB_DISCOVERED_CURRENCY", UB_DISCOVERED_CURRENCY);
            GameLocation.RegisterTouchAction("UB_ShowCurrency", ShowCurrencyTouchAction);
            TriggerActionManager.RegisterAction("UB_AddCurrency", UB_AddCurrency);
        }

        private static bool UB_AddCurrency(string[] args, TriggerActionContext context, out string error)
        {
            if (!ArgUtility.TryGet(args, 1, out string who, out error, allowBlank: false))
                return false;

            if (!new string[] { "all", "current", "host" }.Contains(who.ToLower())) {
                error = $"Invalid Target Player {who}. Must be one of 'All', 'Current', 'Host'";
                return false;

            }
            who = who.ToLower();

            if (!ArgUtility.TryGet(args, 2, out string currencyId, out error, allowBlank: false))
                return false;

            var currency = getCurrencyById(currencyId, false);
            if (currency is null) {
                error = $"Invalid Currency ID: {currencyId}";
                return false;
            }

            if (!ArgUtility.TryGet(args, 3, out string amountString, out error, allowBlank: false))
                return false;

            if (!int.TryParse(amountString, out var amount)) {
                error = $"Currency amount did not parse to a number";
                return false;
            }

            if (who == "current") {
                var relevantPlayer = getRelevantPlayer(currency, Game1.player.UniqueMultiplayerID);
                addWalletCurrency(currency, relevantPlayer, amount, true, true);

            } else if (who == "host") {
                var relevantPlayer = getRelevantPlayer(currency, Game1.MasterPlayer.UniqueMultiplayerID);
                addWalletCurrency(currency, relevantPlayer, amount, true, true);

            } else if (who == "all")
                foreach (var farmer in Game1.getAllFarmers()) {
                    var relevantPlayer = getRelevantPlayer(currency, farmer.UniqueMultiplayerID);
                    addWalletCurrency(currency, relevantPlayer, amount, true, true);
                }

            return true;
        }

        private static void ShowCurrencyTouchAction(GameLocation location, string[] args, Farmer player, Vector2 tile)
        {
            if (args.Length < 2)
                return;

            var currency = getCurrencyById(args[1]);
            if (currency is null)
                return;

            var relevantPlayer = getRelevantPlayer(currency, player.UniqueMultiplayerID);

            if (args.Length > 3 && args[2].ToLower() == "true") {
                if (currency.PowersData.UnlockedCondition is null)
                    currency.PowersData.UnlockedCondition = "UB_DISCOVERED_CURRENCY " + currency.Id;

                if (!GameStateQuery.CheckConditions(currency.PowersData.UnlockedCondition))
                    return;
            }

            var value = ModData.getWalletCurrency(currency.Id, relevantPlayer);
            WalletCurrencyBillboard.Register(currency, value, value, false);
        }

        private static bool UB_DISCOVERED_CURRENCY(string[] query, GameStateQueryContext context)
        {
            if (!ArgUtility.TryGet(query, 1, out var currencyId, out var error)) {
                return GameStateQuery.Helpers.ErrorResult(query, error);
            }

            var currency = getCurrencyById(currencyId);
            var relevantPlayer = getRelevantPlayer(currency, Game1.player.UniqueMultiplayerID);

            return ModData.walletCurrencyDiscovered(currencyId, relevantPlayer);
        }

        private static void Draw(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            DrawOverheadAnimations(e.SpriteBatch);
        }

        private static void DrawOverheadAnimations(SpriteBatch b)
        {
            for (int i = OverheadAnimations.Count - 1; i >= 0; i--) {
                var animation = OverheadAnimations[i];

                var texture = animation.Texture;
                var sourceRectangle = animation.getOffsetRectangle();
                if (animation.update(Game1.currentGameTime)) {
                    var position = Game1.GlobalToLocal(Game1.viewport, Game1.player.Position) + animation.Position + new Vector2(0, -96f);
                    var scale = 64f / sourceRectangle.Width;

                    animation.Position.Y = Math.Clamp(animation.Position.Y - 3f, -96f, 0f);
                    b.Draw(texture, position, sourceRectangle, color: Color.White, 0f, new Vector2(), scale, SpriteEffects.None, 1f);

                } else
                    OverheadAnimations.RemoveAt(i);
            }
        }

        public static void OnItemReceived_Postfix(Item item, int countAdded, Item mergedIntoStack, bool hideHudNotification, Farmer __instance)
        {
            if (!__instance.IsLocalPlayer)
                return;

            Item actualItem = mergedIntoStack ?? item;
            itemReceivedIsRelevant(actualItem, __instance);
        }


        public static bool itemReceivedIsRelevant(Item item, Farmer who)
        {

            if (!getCurrencyItemMatch(item.QualifiedItemId, out var match, out var currency, out _))
                return false;

            var relevantPlayer = getRelevantPlayer(currency, who.UniqueMultiplayerID);
            var sumItemValue = item.Stack * match.Value;

            if (currency.HoldItemUpOnDiscovery && !ModData.walletCurrencyDiscovered(currency.Id, relevantPlayer)) {
                who.completelyStopAnimatingOrDoingAction();
                who.holdUpItemThenMessage(item);

            } else if (currency.DrawOverheadPickupAnimation) {
                if (currency.OverheadPickupTexture != "") {
                    var texture = Helper.GameContent.Load<Texture2D>(currency.OverheadPickupTexture);
                    OverheadAnimations.Add(
                        new AnimatedTexture(texture, currency.OverheadPickupAnimation, currency.OverheadPickupTextureSize, currency.OverheadPickupTextureSize, 1)
                    );

                } else {
                    var definition = ItemRegistry.GetTypeDefinition(item.TypeDefinitionId);
                    var itemData = definition.GetData(item.ItemId);
                    who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(itemData.TextureName, itemData.GetSourceRect(), 100f, 1, 8, new Vector2(0f, -96f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f) {
                        motion = new Vector2(0f, -6f),
                        acceleration = new Vector2(0f, 0.2f),
                        stopAcceleratingWhenVelocityIsZero = true,
                        attachedCharacter = who,
                        positionFollowsAttachedCharacter = true
                    });

                }
            }

            addWalletCurrency(currency, relevantPlayer, sumItemValue, true, true);

            who.removeItemFromInventory(item);

            return true;
        }

        public static int addWalletCurrency(WalletCurrencyModel currency, long relevantPlayer, int addedValue, bool broadcast, bool registerBillboard)
        {
            var oldValue = ModData.getWalletCurrency(currency.Id, relevantPlayer);
            var newValue = ModData.addWalletCurrency(currency.Id, relevantPlayer, addedValue);

            if (registerBillboard)
                WalletCurrencyBillboard.Register(currency, oldValue, newValue);

            if (broadcast) {
                var transferData = new CurrencyTransferModel {
                    CurrencyId = currency.Id,
                    Who = relevantPlayer,
                    AddedValue = addedValue
                };

                Helper.Multiplayer.SendMessage(transferData, "WalletCurrencyChanged", modIDs: new[] { ModManifest.UniqueID });
            }

            return newValue;
        }

        public static bool getCurrencyItemMatch(string targetItemId, out WalletCurrencyItem match, out WalletCurrencyModel currency, out long relevantPlayer)
        {
            var currencies = Helper.GameContent.Load<Dictionary<string, WalletCurrencyModel>>(Asset);
            foreach (var currencyElement in currencies) {
                currency = currencyElement.Value;
                currency.Id = currencyElement.Key;

                match = currency.Items.Find(el => el.ItemId == targetItemId);
                if (match is null)
                    continue;

                relevantPlayer = currency.Shared ? Game1.MasterPlayer.UniqueMultiplayerID : Game1.player.UniqueMultiplayerID;
                return true;
            }

            relevantPlayer = 0;
            match = null;
            currency = null;
            return false;
        }

        public static WalletCurrencyModel getCurrencyById(string currencyId, bool expectedToExist = true)
        {
            var currencies = Helper.GameContent.Load<Dictionary<string, WalletCurrencyModel>>(Asset);
            if (!currencies.TryGetValue(currencyId, out WalletCurrencyModel currency)) {
                if (expectedToExist)
                    Monitor.LogOnce("Invalid currency ID: " + currencyId, StardewModdingAPI.LogLevel.Error);
                return null;
            }

            currency.Id = currencyId;
            return currency;
        }

        public static long getRelevantPlayer(WalletCurrencyModel currency, long who)
            => currency.Shared ? Game1.MasterPlayer.UniqueMultiplayerID : who;
    }
}