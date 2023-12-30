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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Bundles;
using StardewArchipelago.Serialization;
using StardewArchipelago.Stardew;
using StardewArchipelago.Textures;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using Bundle = StardewValley.Menus.Bundle;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.CC
{
    public static class JunimoNoteMenuInjections
    {
        private const int REMIXED_BUNDLE_INDEX_THRESHOLD = 100;
        private const int CUSTOM_BUNDLE_INDEX_THRESHOLD = 200;

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static ArchipelagoStateDto _state;
        private static LocationChecker _locationChecker;
        private static BundleReader _bundleReader;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, ArchipelagoStateDto state, LocationChecker locationChecker, BundleReader bundleReader)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _state = state;
            _locationChecker = locationChecker;
            _bundleReader = bundleReader;
        }

        // public void checkForRewards()
        public static void CheckForRewards_SendBundleChecks_PostFix(JunimoNoteMenu __instance)
        {
            try
            {
                CheckAllBundleLocations();
                MarkAllRewardsAsAlreadyGrabbed();
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckForRewards_SendBundleChecks_PostFix)}:\n{ex}", LogLevel.Error);
            }
        }

        private static void CheckAllBundleLocations()
        {
            var completedBundleNames = _bundleReader.GetAllCompletedBundles();
            foreach (var completedBundleName in completedBundleNames)
            {
                _locationChecker.AddCheckedLocation(completedBundleName + " Bundle");
            }
        }

        private static void MarkAllRewardsAsAlreadyGrabbed()
        {
            var communityCenter = Game1.locations.OfType<CommunityCenter>().First();
            var bundleRewardsDictionary = communityCenter.bundleRewards;
            foreach (var bundleRewardKey in bundleRewardsDictionary.Keys)
            {
                bundleRewardsDictionary[bundleRewardKey] = false;
            }
        }

        // public void setUpMenu(int whichArea, Dictionary<int, bool[]> bundlesComplete)
        public static void SetupMenu_AddTextureOverrides_Postfix(JunimoNoteMenu __instance, int whichArea, Dictionary<int, bool[]> bundlesComplete)
        {
            try
            {
                var remixedBundlesTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\BundleSprites");
                foreach (var bundle in __instance.bundles)
                {
                    var textureOverride = BundleIcons.GetBundleIcon(_modHelper, bundle.name);
                    if (textureOverride == null)
                    {
                        if (bundle.bundleIndex < REMIXED_BUNDLE_INDEX_THRESHOLD)
                        {
                            bundle.bundleTextureOverride = null;
                            bundle.bundleTextureIndexOverride = -1;
                            continue;
                        }

                        if (bundle.bundleIndex < CUSTOM_BUNDLE_INDEX_THRESHOLD)
                        {
                            bundle.bundleTextureOverride = remixedBundlesTexture;
                            bundle.bundleTextureIndexOverride = bundle.bundleIndex - REMIXED_BUNDLE_INDEX_THRESHOLD;
                            continue;
                        }

                        var bundleIndexString = bundle.bundleIndex.ToString();
                        if (bundleIndexString.Length == 4)
                        {
                            if (TryGetBundleName(bundleIndexString, out var moneyBundleName))
                            {
                                bundle.bundleTextureOverride = BundleIcons.GetBundleIcon(_modHelper, moneyBundleName);
                                bundle.bundleTextureIndexOverride = 0;
                                continue;
                            }
                        }

                        textureOverride = ArchipelagoTextures.GetColoredLogo(_modHelper, 32, ArchipelagoTextures.COLOR);
                    }

                    bundle.bundleTextureOverride = textureOverride;
                    bundle.bundleTextureIndexOverride = 0;
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SetupMenu_AddTextureOverrides_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static bool TryGetBundleName(string bundleIndexString, out string moneyBundleName)
        {
            switch (bundleIndexString[..2])
            {
                case "23":
                    moneyBundleName = "money_cheap";
                    return true;
                case "24":
                    moneyBundleName = "money_medium";
                    return true;
                case "25":
                    moneyBundleName = "money_expensive";
                    return true;
                case "26":
                    moneyBundleName = "money_rich";
                    return true;
                default:
                    moneyBundleName = "";
                    return false;
            }
        }

        // public string getRewardNameForArea(int whichArea)
        public static bool GetRewardNameForArea_ScoutRoomRewards_Prefix(JunimoNoteMenu __instance, int whichArea, ref string __result)
        {
            try
            {
                var apAreaToScout = "???";
                switch ((Area)whichArea)
                {
                    case Area.Pantry:
                        apAreaToScout = CommunityCenterInjections.AP_LOCATION_PANTRY;
                        break;
                    case Area.CraftsRoom:
                        apAreaToScout = CommunityCenterInjections.AP_LOCATION_CRAFTS_ROOM;
                        break;
                    case Area.FishTank:
                        apAreaToScout = CommunityCenterInjections.AP_LOCATION_FISH_TANK;
                        break;
                    case Area.BoilerRoom:
                        apAreaToScout = CommunityCenterInjections.AP_LOCATION_BOILER_ROOM;
                        break;
                    case Area.Vault:
                        apAreaToScout = CommunityCenterInjections.AP_LOCATION_VAULT;
                        break;
                    case Area.Bulletin:
                        apAreaToScout = CommunityCenterInjections.AP_LOCATION_BULLETIN_BOARD;
                        break;
                    case Area.AbandonedJojaMart:
                        apAreaToScout = CommunityCenterInjections.AP_LOCATION_ABANDONED_JOJA_MART;
                        break;
                    default:
                        __result = "???";
                        return false; // don't run original logic
                }

                var scoutedItem = _archipelago.ScoutSingleLocation(apAreaToScout);
                var rewardText = $"Reward: {scoutedItem.PlayerName}'s {scoutedItem.GetItemName()}";
                __result = rewardText;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetRewardNameForArea_ScoutRoomRewards_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // private bool specificBundlePage;
        private static IReflectedField<bool> _specificBundlePageField;

        // public override void draw(SpriteBatch b)
        public static void Draw_AddCurrencyBoxes_Postfix(JunimoNoteMenu __instance, SpriteBatch b)
        {
            try
            {
                var specificBundlePageField = _modHelper.Reflection.GetField<bool>(__instance, "specificBundlePage");
                var specificBundlePage = specificBundlePageField.GetValue();
                if (!specificBundlePage || !Game1.player.hasOrWillReceiveMail("canReadJunimoText"))
                {
                    Game1.specialCurrencyDisplay.ShowCurrency(null);
                    return;
                }

                // private Bundle currentPageBundle;
                var currentPageBundleField = _modHelper.Reflection.GetField<Bundle>(__instance, "currentPageBundle");
                var currentPageBundle = currentPageBundleField.GetValue();
                var ingredient = currentPageBundle.ingredients.Last();
                var ingredientIndex = ingredient.index;
                if (ingredientIndex >= -1)
                {
                    return;
                }
                
                var amountText = $"{ingredient.stack}";

                if (ingredientIndex == CurrencyBundle.CurrencyIds["Qi Gem"])
                {
                    Game1.specialCurrencyDisplay.ShowCurrency("qiGems");
                    amountText += " Qi Gems";
                }
                else if (ingredientIndex == CurrencyBundle.CurrencyIds["Qi Coin"])
                {
                    SpriteText.drawStringWithScrollBackground(b, Game1.player.clubCoins.ToString(), 64, 16);
                    amountText += " Qi Coins";
                }
                else if (ingredientIndex == CurrencyBundle.CurrencyIds["Star Token"])
                {
                    DrawStarTokenCurrency();
                    amountText += " Star Tokens";
                }
                else
                {
                    Game1.specialCurrencyDisplay.ShowCurrency(null);
                    return;
                }

                var textSize = Game1.dialogueFont.MeasureString(amountText).X;
                var textPosition = new Vector2(__instance.xPositionOnScreen + 936 - textSize / 2f, __instance.yPositionOnScreen + 292);
                b.DrawString(Game1.dialogueFont, amountText, textPosition, Game1.textColor * 0.9f);

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Draw_AddCurrencyBoxes_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void DrawStarTokenCurrency()
        {
            var spriteBatch = Game1.spriteBatch;
            spriteBatch.End();
            Game1.PushUIMode();
            spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            var tokenAmount = _state.StoredStarTokens;
            spriteBatch.Draw(Game1.fadeToBlackRect, new Rectangle(16, 16, 128 + (tokenAmount > 999 ? 16 : 0), 64), Color.Black * 0.75f);
            spriteBatch.Draw(Game1.mouseCursors, new Vector2(32f, 32f), new Rectangle(338, 400, 8, 8), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            Game1.drawWithBorder(tokenAmount.ToString() ?? "", Color.Black, Color.White, new Vector2(72f, (float)(21 + (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? 8 : (LocalizedContentManager.CurrentLanguageLatin ? 16 : 8)))), 0.0f, 1f, 1f, false);
            //if (Game1.activeClickableMenu == null)
            //{
            // Game1.dayTimeMoneyBox.drawMoneyBox(spriteBatch, Game1.dayTimeMoneyBox.xPositionOnScreen, 4);
            //}
            spriteBatch.End();
            Game1.PopUIMode();
            spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            //if (Game1.IsMultiplayer)
            //{
            //    Game1.player.team.festivalScoreStatus.Draw(spriteBatch, new Vector2(32f, (float)(Game1.viewport.Height - 32)), draw_layer: 0.99f, vertical_origin: PlayerStatusList.VerticalAlignment.Bottom);
            //}
        }

        // public override void receiveLeftClick(int x, int y, bool playSound = true)
        public static bool ReceiveLeftClick_PurchaseCurrencyBundle_Prefix(JunimoNoteMenu __instance, int x, int y, bool playSound)
        {
            try
            {
                // private bool specificBundlePage;
                var specificBundlePageField = _modHelper.Reflection.GetField<bool>(__instance, "specificBundlePage");
                if (!JunimoNoteMenu.canClick || __instance.scrambledText || !specificBundlePageField.GetValue() || __instance.purchaseButton == null || !__instance.purchaseButton.containsPoint(x, y))
                {
                    return true; // run original logic
                }

                // private Bundle currentPageBundle;
                var currentPageBundleField = _modHelper.Reflection.GetField<Bundle>(__instance, "currentPageBundle");
                var currentPageBundle = currentPageBundleField.GetValue();

                var ingredient = currentPageBundle.ingredients.Last();
                var currency = ingredient.index;
                if (currency == -1)
                {
                    return true; // run original logic
                }
                
                TryPurchaseCurrentBundle(__instance, ingredient, currentPageBundle);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ReceiveLeftClick_PurchaseCurrencyBundle_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void TryPurchaseCurrentBundle(JunimoNoteMenu junimoNote, BundleIngredientDescription ingredient, Bundle currentPageBundle)
        {
            if (ingredient.index == CurrencyBundle.CurrencyIds["Qi Gem"])
            {
                TryPurchaseCurrentBundleWithQiGems(junimoNote, ingredient, currentPageBundle);
                return;
            }

            if (ingredient.index == CurrencyBundle.CurrencyIds["Qi Coin"])
            {
                TryPurchaseCurrentBundleWithQiCoins(junimoNote, ingredient, currentPageBundle);
                return;
            }

            if (ingredient.index == CurrencyBundle.CurrencyIds["Star Token"])
            {
                TryPurchaseCurrentBundleWithStarTokens(junimoNote, ingredient, currentPageBundle);
                return;
            }
        }

        private static void TryPurchaseCurrentBundleWithQiGems(JunimoNoteMenu junimoNote, BundleIngredientDescription ingredient, Bundle currentPageBundle)
        {
            if (Game1.player.QiGems < ingredient.stack)
            {
                Game1.dayTimeMoneyBox.moneyShakeTimer = 600;
                return;
            }

            Game1.player.QiGems -= ingredient.stack;

            PerformCurrencyPurchase(junimoNote, currentPageBundle);
        }

        private static void TryPurchaseCurrentBundleWithQiCoins(JunimoNoteMenu junimoNote, BundleIngredientDescription ingredient, Bundle currentPageBundle)
        {
            if (Game1.player.clubCoins < ingredient.stack)
            {
                Game1.dayTimeMoneyBox.moneyShakeTimer = 600;
                return;
            }

            Game1.player.clubCoins -= ingredient.stack;

            PerformCurrencyPurchase(junimoNote, currentPageBundle);
        }

        private static void TryPurchaseCurrentBundleWithStarTokens(JunimoNoteMenu junimoNote, BundleIngredientDescription ingredient, Bundle currentPageBundle)
        {
            if (_state.StoredStarTokens < ingredient.stack)
            {
                Game1.dayTimeMoneyBox.moneyShakeTimer = 600;
                return;
            }

            _state.StoredStarTokens -= ingredient.stack;

            PerformCurrencyPurchase(junimoNote, currentPageBundle);
        }

        private static void PerformCurrencyPurchase(JunimoNoteMenu junimoNote, Bundle currentPageBundle)
        {
            Game1.playSound("select");
            currentPageBundle.completionAnimation(junimoNote);
            if (junimoNote.purchaseButton == null)
            {
            }
            else
            {
                junimoNote.purchaseButton.scale = junimoNote.purchaseButton.baseScale * 0.75f;
            }

            var communityCenter = (CommunityCenter)Game1.getLocationFromName("CommunityCenter");
            communityCenter.bundleRewards[currentPageBundle.bundleIndex] = true;
            communityCenter.bundles.FieldDict[currentPageBundle.bundleIndex][0] = true;
            junimoNote.checkForRewards();
            var flag = junimoNote.bundles.Any(bundle => !bundle.complete && !bundle.Equals(currentPageBundle));
            // private int whichArea;
            var whichAreaField = _modHelper.Reflection.GetField<int>(junimoNote, "whichArea");
            var whichArea = whichAreaField.GetValue();
            if (!flag)
            {
                // private void restoreAreaOnExit()
                var restoreAreaOnExitMethod = _modHelper.Reflection.GetMethod(junimoNote, "restoreAreaOnExit");
                communityCenter.markAreaAsComplete(whichArea);
                junimoNote.exitFunction = () => restoreAreaOnExitMethod.Invoke();
                communityCenter.areaCompleteReward(whichArea);
            }
            else
            {
                communityCenter.getJunimoForArea(whichArea)?.bringBundleBackToHut(Bundle.getColorFromColorIndex(currentPageBundle.bundleColor),
                    Game1.getLocationFromName("CommunityCenter"));
            }

            // Game1.multiplayer.globalChatInfoMessage("Bundle");
        }
    }
}
