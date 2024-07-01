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
using Netcode;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Bundles;
using StardewArchipelago.Constants;
using StardewArchipelago.Serialization;
using StardewArchipelago.Stardew.Ids.Vanilla;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Events;
using StardewValley.Internal;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network.NetEvents;
using xTile.Dimensions;
using Bundle = StardewValley.Menus.Bundle;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Bundles
{
    public static class RaccoonInjections
    {
        private const string GIANT_STUMP = "The Giant Stump";

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static ArchipelagoStateDto _state;
        private static LocationChecker _locationChecker;
        private static BundlesManager _bundlesManager;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, ArchipelagoStateDto state,
            LocationChecker locationChecker, BundlesManager bundlesManager)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _state = state;
            _locationChecker = locationChecker;
            _bundlesManager = bundlesManager;
        }

        // public override bool performAction(string[] action, Farmer who, Location tileLocation)
        public static bool PerformAction_CheckStump_Prefix(Forest __instance, string[] action, Farmer who, Location tileLocation, ref bool __result)
        {
            try
            {
                if (action.Length <= 0 || action[0] != "FixRaccoonStump" || !Game1.MasterPlayer.mailReceived.Contains("raccoonTreeFallen"))
                {
                    return true; // run original logic
                }

                if (!_locationChecker.IsLocationMissing(GIANT_STUMP))
                {
                    Game1.drawObjectDialogue("Eventually someone will move in...");
                    __result = true;
                    return false; // don't run original logic
                }

                if (who.Items.ContainsId("(O)709", 100))
                {
                    __instance.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:FixRaccoonStump_Question"), __instance.createYesNoResponses(), "ForestStump");
                }
                else
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:FixRaccoonStump_Hint"));
                    if (!who.mailReceived.Contains("checkedRaccoonStump"))
                    {
                        who.addQuest("134");
                        who.mailReceived.Add("checkedRaccoonStump");
                    }
                }

                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformAction_CheckStump_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
        
        // public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
        public static bool AnswerDialogueAction_FixStump_Prefix(Forest __instance, string questionAndAnswer, string[] questionParams, ref bool __result)
        {
            try
            {
                if (questionAndAnswer != "ForestStump_Yes")
                {
                    return true; // run original logic
                }

                if (_locationChecker.IsLocationChecked(GIANT_STUMP))
                {
                    __result = true;
                    return false; // don't run original logic
                }

                Game1.globalFadeToBlack(() => FadedForStumpFix(__instance));
                Game1.player.Items.ReduceId("(O)709", 100);
                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_FixStump_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void FadedForStumpFix(Forest forest)
        {
            Game1.freezeControls = true;
            DelayedAction.playSoundAfterDelay("crafting", 1000);
            DelayedAction.playSoundAfterDelay("crafting", 1500);
            DelayedAction.playSoundAfterDelay("crafting", 2000);
            DelayedAction.playSoundAfterDelay("crafting", 2500);
            DelayedAction.playSoundAfterDelay("axchop", 3000);
            DelayedAction.playSoundAfterDelay("discoverMineral", 3200);
            Game1.viewportFreeze = true;
            Game1.viewport.X = -10000;
            Game1.pauseThenDoFunction(4000, forest.doneWithStumpFix);
            _locationChecker.AddCheckedLocation(GIANT_STUMP);
            forest.stumpFixed.Value = true;
            Forest.fixStump(forest);
            Game1.player.team.RequestSetSimpleFlag(SimpleFlagType.HasQuest, PlayerActionTarget.All, QuestIds.GIANT_STUMP, false);
        }

        // public void activate()
        public static bool Activate_DisplayDialogueOrBundle_Prefix(Raccoon __instance)
        {
            try
            {
                if (__instance.mrs_raccoon.Value)
                {
                    Utility.TryOpenShopMenu(nameof(Raccoon), __instance.Name);
                }
                else
                {
                    var receivedRaccoons = _archipelago.GetReceivedItemCount(APItem.PROGRESSIVE_RACCOON);
                    if (!_archipelago.SlotData.QuestLocations.StoryQuestsEnabled)
                    {
                        receivedRaccoons += 1;
                    }

                    var nextRaccoonRequestNumber = GetCurrentRaccoonBundleNumber();
                    var raccoonBundleAvailable = nextRaccoonRequestNumber < receivedRaccoons;

                    // private bool wasTalkedTo;
                    var wasTalkedToField = _modHelper.Reflection.GetField<bool>(__instance, "wasTalkedTo");
                    var wasTalkedTo = wasTalkedToField.GetValue();

                    if (!wasTalkedTo)
                    {
                        var timesFedRaccoons = Game1.netWorldState.Value.TimesFedRaccoons;
                        if (timesFedRaccoons >= 5 && raccoonBundleAvailable)
                        {
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Raccoon_intro"));
                        }
                        else if (timesFedRaccoons > 5 & !raccoonBundleAvailable)
                        {
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Raccoon_interim"));
                        }
                        else
                        {
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Raccoon_" + (!raccoonBundleAvailable ? "interim_" : "intro_") + timesFedRaccoons));
                        }
                        if (!raccoonBundleAvailable)
                        {
                            return false; // don't run original logic
                        }
                        Game1.afterDialogues = () => ActivateRaccoonBundleMutex(__instance);
                    }
                    else
                    {
                        if (!raccoonBundleAvailable)
                        {
                            return false; // don't run original logic
                        }
                        ActivateRaccoonBundleMutex(__instance);
                    }
                }
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Activate_DisplayDialogueOrBundle_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void ActivateRaccoonBundleMutex(Raccoon raccoon)
        {
            raccoon.mutex.RequestLock(() => ActivateMrRaccoon(raccoon), () => Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Raccoon_busy")));
        }

        // private void _activateMrRaccoon()
        private static void ActivateMrRaccoon(Raccoon raccoon)
        {
            // private bool wasTalkedTo;
            var wasTalkedToField = _modHelper.Reflection.GetField<bool>(raccoon, "wasTalkedTo");
            wasTalkedToField.SetValue(true);

            var currentRaccoonBundleNumber = GetCurrentRaccoonBundleNumber();
            if (currentRaccoonBundleNumber <= 0)
            {
                return;
            }

            if (Game1.netWorldState.Value.SeasonOfCurrentRacconBundle != currentRaccoonBundleNumber)
            {
                _state.CurrentRaccoonBundleStatus.Clear();
                Game1.netWorldState.Value.SeasonOfCurrentRacconBundle = currentRaccoonBundleNumber;
            }

            var ingredients = new List<BundleIngredientDescription>();
            var raccoonRequestsRoom = _bundlesManager.BundleRooms.Rooms[APName.RACCOON_REQUESTS_ROOM];
            var currentRaccoonBundleName = $"{APName.RACCOON_REQUEST_PREFIX}{currentRaccoonBundleNumber}";
            var raccoonBundle = (ItemBundle)raccoonRequestsRoom.Bundles[currentRaccoonBundleName];
            for (var i = 0; i < raccoonBundle.Items.Count; i++)
            {
                if (_state.CurrentRaccoonBundleStatus.Count <= i)
                {
                    _state.CurrentRaccoonBundleStatus.Add(false);
                }
                if (raccoonBundle.Items[i] is null)
                {
                    throw new ArgumentException($"The raccoon must only have item bundles");
                }
                var bundleIngredient = raccoonBundle.Items[i].CreateBundleIngredientDescription(_state.CurrentRaccoonBundleStatus[i]);
                ingredients.Add(bundleIngredient);
            }

            var whichBundle = (currentRaccoonBundleNumber - 1) % 5;
            var bundle = new Bundle("Seafood", null, ingredients, new bool[1])
            {
                bundleTextureOverride = Game1.content.Load<Texture2D>("LooseSprites\\BundleSprites"),
                bundleTextureIndexOverride = 14 + whichBundle,
            };
            var raccoonNoteMenu = new JunimoNoteMenu(bundle, "LooseSprites\\raccoon_bundle_menu");
            raccoonNoteMenu.onIngredientDeposit = x => _state.CurrentRaccoonBundleStatus[x] = true;
            raccoonNoteMenu.onBundleComplete = _ => BundleComplete(raccoon);
            raccoonNoteMenu.onScreenSwipeFinished = _ => BundleCompleteAfterSwipe(raccoon);
            raccoonNoteMenu.behaviorBeforeCleanup = _ => raccoon.mutex?.ReleaseLock();
            Game1.activeClickableMenu = raccoonNoteMenu;
        }

        // private void bundleComplete(JunimoNoteMenu menu)
        private static void BundleComplete(Raccoon raccoon)
        {
            JunimoNoteMenu.screenSwipe = new ScreenSwipe(1);
            _locationChecker.AddCheckedLocation($"{APName.RACCOON_REQUEST_PREFIX}{Game1.netWorldState.Value.SeasonOfCurrentRacconBundle}");
            Game1.netWorldState.Value.SeasonOfCurrentRacconBundle = -1;
            _state.CurrentRaccoonBundleStatus.Clear();

            // private bool wasTalkedTo;
            var wasTalkedToField = _modHelper.Reflection.GetField<bool>(raccoon, "wasTalkedTo");
            wasTalkedToField.SetValue(false);
        }

        // private void bundleCompleteAfterSwipe(JunimoNoteMenu menu)
        private static void BundleCompleteAfterSwipe(Raccoon raccoon)
        {
            Game1.activeClickableMenu = null;
            raccoon.mutex?.ReleaseLock();
            Game1.netWorldState.Value.DaysPlayedWhenLastRaccoonBundleWasFinished = Game1.netWorldState.Value.Date.TotalDays;
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Raccoon_receive"));
        }

        private static int GetCurrentRaccoonBundleNumber()
        {
            var remainingRacoonBundleNumbers = _locationChecker.GetAllMissingLocationNames()
                .Where(x => x.StartsWith(APName.RACCOON_REQUEST_PREFIX))
                .Select(x => int.Parse(x[APName.RACCOON_REQUEST_PREFIX.Length..]));
            if (remainingRacoonBundleNumbers.Any())
            {
                return remainingRacoonBundleNumbers.Min();
            }

            return -1;
        }

        // public bool IsValidItemForThisIngredientDescription(Item item,BundleIngredientDescription ingredient)
        public static bool IsValidItemForThisIngredientDescription_TestPatch_Prefix(Bundle __instance, Item item, BundleIngredientDescription ingredient, ref bool __result)
        {
            try
            {
                if (item == null || ingredient.completed || ingredient.quality > item.Quality)
                {
                    __result = false;
                    return false; // don't run original logic
                }

                if (ingredient.preservesId != null)
                {
                    var flavoredIngredientQuery = "FLAVORED_ITEM " + ingredient.id + " " + ingredient.preservesId;
                    var queryContext = new ItemQueryContext(Game1.currentLocation, Game1.player, Game1.random);
                    var resolvedIngredientQueryResult = ItemQueryResolver.TryResolve(flavoredIngredientQuery, queryContext);
                    var resolvedIngredient = resolvedIngredientQueryResult.FirstOrDefault<ItemQueryResult>()?.Item;
                    if (resolvedIngredient is Object ingredientObject && item is Object itemObject && itemObject.preservedParentSheetIndex?.Value != null)
                    {
                        var qualifiedIdsMatch = item.QualifiedItemId == ingredientObject.QualifiedItemId;
                        var preservesIdMatch = ingredient.preservesId.Contains(itemObject.preservedParentSheetIndex.Value);
                        __result = qualifiedIdsMatch && preservesIdMatch;
                        return false; // don't run original logic
                    }

                    __result = false;
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(IsValidItemForThisIngredientDescription_TestPatch_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public override void draw(SpriteBatch spriteBatch)
        public static void Draw_TreeStumpFix_Postfix(Forest __instance, SpriteBatch spriteBatch)
        {
            try
            {
                if (!_locationChecker.IsLocationMissing("The Giant Stump") || !Game1.player.hasQuest(QuestIds.GIANT_STUMP))
                {
                    return;
                }

                var num = (float)(4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2) - 8.0);
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(3576f, 272f + num)), new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0504009947f);
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(3616f, 312f + num)), new Microsoft.Xna.Framework.Rectangle(175, 425, 12, 12), Color.White * 0.75f, 0.0f, new Vector2(6f, 6f), 4f, SpriteEffects.None, 0.050409995f);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Draw_TreeStumpFix_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // protected override void resetSharedState()
        public static void ResetSharedState_WalkThroughRaccoons_Postfix(Forest __instance)
        {
            try
            {
                if (!_locationChecker.IsLocationMissing("The Giant Stump"))
                {
                    return;
                }

                foreach (var character in __instance.characters)
                {
                    if (character is Raccoon raccoon)
                    {
                        raccoon.farmerPassesThrough = true;
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ResetSharedState_WalkThroughRaccoons_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
