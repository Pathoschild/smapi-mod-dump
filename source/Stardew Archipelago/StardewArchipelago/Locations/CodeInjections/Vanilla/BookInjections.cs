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
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Locations;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.GameModifications.CodeInjections.Television;
using StardewArchipelago.Stardew.Ids.Vanilla;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class BookInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static QueenOfSauceManager _qosManager;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, QueenOfSauceManager qosManager)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _qosManager = qosManager;

            // So the rest of the world doesn't try to spawn them
            Game1.netWorldState.Value.LostBooksFound = 21;
        }

        // protected override void resetLocalState()
        public static bool ResetLocalState_BooksanityLostBooks_Prefix(LibraryMuseum __instance)
        {
            try
            {
                if (!Game1.player.eventsSeen.Contains("0") && __instance.doesFarmerHaveAnythingToDonate(Game1.player))
                {
                    Game1.player.mailReceived.Add("somethingToDonate");
                }
                if (LibraryMuseum.HasDonatedArtifacts())
                {
                    Game1.player.mailReceived.Add("somethingWasDonated");
                }

                CallBaseResetLocalState(__instance);
                
                var lostBooksFound = _archipelago.GetReceivedItemCount("Progressive Lost Book");

                // private Dictionary<int, Vector2> getLostBooksLocations()
                var getLostBooksLocationsMethod = _modHelper.Reflection.GetMethod(__instance, "getLostBooksLocations");
                var lostBooksLocations = getLostBooksLocationsMethod.Invoke<Dictionary<int, Vector2>>();

                foreach (var (number, position) in lostBooksLocations)
                {
                    if (number <= lostBooksFound && !Game1.player.mailReceived.Contains("lb_" + number))
                    {
                        __instance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(144, 447, 15, 15), new Vector2(position.X * 64f, (float)(position.Y * 64.0 - 96.0 - 16.0)), false, 0.0f, Color.White)
                        {
                            interval = 99999f,
                            animationLength = 1,
                            totalNumberOfLoops = 9999,
                            yPeriodic = true,
                            yPeriodicLoopTime = 4000f,
                            yPeriodicRange = 16f,
                            layerDepth = 1f,
                            scale = 4f,
                            id = number,
                        });
                    }
                }
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ResetLocalState_BooksanityLostBooks_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public void readNote(int which)
        public static bool ReadNote_BooksanityLostBook_Prefix(GameLocation __instance, int which)
        {
            try
            {
                var lostBooksFound = _archipelago.GetReceivedItemCount("Progressive Lost Book");
                if (lostBooksFound >= which)
                {
                    var bookTitle = LostBooks.BooksByNumber[which];
                    var location = $"Read {bookTitle}";
                    _locationChecker.AddCheckedLocation(location);

                    var message = Game1.content.LoadString("Strings\\Notes:" + which).Replace('\n', '^');
                    Game1.player.mailReceived.Add("lb_" + which);
                    __instance.removeTemporarySpritesWithIDLocal(which);
                    Game1.drawLetterMessage(message);
                }
                else
                {
                    Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Notes:Missing")));
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ReadNote_BooksanityLostBook_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void CallBaseResetLocalState(LibraryMuseum museum)
        {
            // base.resetLocalState();
            var gameLocationResetLocalStateMethod = typeof(GameLocation).GetMethod("resetLocalState", BindingFlags.Instance | BindingFlags.NonPublic);
            var functionPointer = gameLocationResetLocalStateMethod.MethodHandle.GetFunctionPointer();
            var baseResetLocalState = (Action)Activator.CreateInstance(typeof(Action), museum, functionPointer);
            baseResetLocalState();
        }

        // private void readBook(GameLocation location)
        public static bool ReadBook_Booksanity_Prefix(Object __instance, GameLocation location)
        {
            try
            {
                ReadBook(__instance, location);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ReadBook_Booksanity_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void ReadBook(Object book, GameLocation location)
        {
            PlayReadBookAnimation(book, location);

            var possibleNames = new[] { book.Name, PowerBooks.BookIdsToNames[book.ItemId] };
            var isThisBookRandomized = false;
            foreach (var possibleName in possibleNames)
            {
                var locationName = $"Read {possibleName}";
                if (_locationChecker.IsLocationMissing(locationName))
                {
                    _locationChecker.AddCheckedLocation(locationName);
                    return;
                }

                isThisBookRandomized = isThisBookRandomized || _archipelago.LocationExists(locationName);
            }

            if (ObjectIds.IsSkillBook(book.ItemId))
            {
                ReadSkillBook(book);
                return;
            }

            var itemId = book.ItemId;
            var bookHasSubsequentReward = itemId != ObjectIds.PRICE_CATALOGUE && itemId != ObjectIds.ANIMAL_CATALOGUE;

            var previousReads = Game1.player.stats.Get(book.itemId.Value);
            var shouldGiveNormalReward = !isThisBookRandomized && previousReads <= 0;
            var shouldGiveSubsequentReward = bookHasSubsequentReward && (isThisBookRandomized || previousReads > 0);

            if (shouldGiveSubsequentReward)
            {
                if (!Game1.player.mailReceived.Contains("read_a_book"))
                {
                    Game1.player.mailReceived.Add("read_a_book");
                }
                GiveBookXpReward(book);
                return;
            }

            if (itemId == ObjectIds.QUEEN_OF_SAUCE_COOKBOOK)
            {
                ReadCookbook(book);
                return;
            }

            if (itemId == ObjectIds.BOOK_OF_STARS)
            {
                Game1.player.gainExperience(0, 250);
                Game1.player.gainExperience(1, 250);
                Game1.player.gainExperience(2, 250);
                Game1.player.gainExperience(3, 250);
                Game1.player.gainExperience(4, 250);
                return;
            }

            if (shouldGiveNormalReward)
            {
                var num = (int)Game1.player.stats.Increment(book.itemId.Value);
                DelayedAction.functionAfterDelay(() => Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:LearnedANewPower")), 1000);
                if (!Game1.player.mailReceived.Contains("read_a_book"))
                {
                    Game1.player.mailReceived.Add("read_a_book");
                }
            }
            Utility.checkForBooksReadAchievement();
        }

        private static void ReadSkillBook(Object book)
        {
            var count = Game1.player.newLevels.Count;
            Game1.player.gainExperience(Convert.ToInt32(book.ItemId.Last().ToString() ?? ""), 250);
            if (Game1.player.newLevels.Count != count && (Game1.player.newLevels.Count <= 1 || count < 1))
            {
                return;
            }
            DelayedAction.functionAfterDelay(() => Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:SkillBookMessage", Game1.content.LoadString("Strings\\1_6_Strings:SkillName_" + book.ItemId.Last()).ToLower())), 1000);
        }

        private static void GiveBookXpReward(Object book)
        {
            foreach (var contextTag in book.GetContextTags())
            {
                if (contextTag.StartsWith("book_xp"))
                {
                    Game1.player.gainExperience(Farmer.getSkillNumberFromName(contextTag.Split('_')[2]), 100);
                    return;
                }
            }
            for (var which = 0; which < 5; ++which)
            {
                Game1.player.gainExperience(which, 20);
            }
        }

        private static void ReadCookbook(Object cookBook)
        {
            var allRerunRecipes = _qosManager.GetAllRerunRecipes();
            var allRecipes = DataLoader.Tv_CookingChannel(Game1.content);
            var numberOfLearnedRecipes = 0;
            foreach (var recipeWeek in allRerunRecipes)
            {
                numberOfLearnedRecipes += LearnRecipe(allRecipes, recipeWeek);
            }
            Game1.player.stats.Increment(cookBook.itemId.Value);
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:QoS_Cookbook", numberOfLearnedRecipes.ToString()));
            Utility.checkForBooksReadAchievement();
        }

        private static int LearnRecipe(Dictionary<string, string> allRecipes, int recipeWeek)
        {
            var recipeInfo = allRecipes[recipeWeek.ToString()].Split('/');
            var recipeKey = recipeInfo[0];
            if (_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.QueenOfSauce))
            {
                var recipeLocation = $"{recipeKey}{Suffix.CHEFSANITY}";
                if (!_locationChecker.IsLocationMissing(recipeLocation))
                {
                    return 0;
                }

                _locationChecker.AddCheckedLocation(recipeLocation);
                return 1;
            }
            if (!Game1.player.cookingRecipes.ContainsKey(recipeKey))
            {
                Game1.player.cookingRecipes.Add(recipeKey, 0);
                return 1;
            }
            return 0;
        }

        private static void PlayReadBookAnimation(Object book, GameLocation location)
        {
            Game1.player.canMove = false;
            Game1.player.freezePause = 1030;
            Game1.player.faceDirection(2);
            Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[]
            {
                new(57, 1000, false, false, Farmer.canMoveNow, true)
                {
                    frameEndBehavior = x =>
                    {
                        location.removeTemporarySpritesWithID(1987654);
                        Utility.addRainbowStarExplosion(location, Game1.player.getStandingPosition() + new Vector2(-40f, -156f), 8);
                    },
                },
            });
            Game1.MusicDuckTimer = 4000f;
            Game1.playSound("book_read");
            Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Book_Animation", new Microsoft.Xna.Framework.Rectangle(0, 0, 20, 20), 10f, 45, 1, Game1.player.getStandingPosition() + new Vector2(-48f, -156f), false, false, Game1.player.getDrawLayer() + 1f / 1000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
                holdLastFrame = true,
                id = 1987654,
            });
            var colorFromTags = ItemContextTagManager.GetColorFromTags(book);
            if (colorFromTags.HasValue)
            {
                Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Book_Animation", new Microsoft.Xna.Framework.Rectangle(0, 20, 20, 20), 10f, 45, 1, Game1.player.getStandingPosition() + new Vector2(-48f, -156f), false, false, Game1.player.getDrawLayer() + 0.0012f, 0.0f, colorFromTags.Value, 4f, 0.0f, 0.0f, 0.0f)
                {
                    holdLastFrame = true,
                    id = 1987654,
                });
            }
        }
    }
}
