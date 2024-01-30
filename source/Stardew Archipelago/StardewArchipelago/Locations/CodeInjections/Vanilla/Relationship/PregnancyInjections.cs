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
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Events;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship
{

    public class PregnancyInjections
    {
        private const int FEMALE = 1;
        private const string FIRST_BABY = "Have a Baby";
        private const string SECOND_BABY = "Have Another Baby";

        private const string npcGiveBirthQuestion = "Would you like me to give birth to a {0}, {1}?";
        private const string playerGiveBirthQuestion = "Would you like to give birth to a {0}, {1}?";
        private const string orderBabyQuestion = "Should I order a {0}, {1}?";

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public bool canGetPregnant()
        public static bool CanGetPregnant_ShuffledPregnancies_Prefix(NPC __instance, ref bool __result)
        {
            try
            {
                if (__instance is Horse)
                {
                    __result = false;
                    return false; // don't run original logic
                }

                var farmer = __instance.getSpouse();
                if (farmer == null || farmer.divorceTonight.Value)
                {
                    __result = false;
                    return false; // don't run original logic
                }

                var heartLevelForNpc = farmer.getFriendshipHeartLevelForNPC(__instance.Name);
                var spouseFriendship = farmer.GetSpouseFriendship();
                __instance.DefaultMap = farmer.homeLocation.Value;
                var homeOfFarmer = Utility.getHomeOfFarmer(farmer);
                if (homeOfFarmer.cribStyle.Value <= 0 || homeOfFarmer.upgradeLevel < 2 ||
                    spouseFriendship.DaysUntilBirthing >= 0 || heartLevelForNpc < 10 || farmer.GetDaysMarried() < 7)
                {
                    __result = false;
                    return false; // don't run original logic
                }

                __result = _locationChecker.IsLocationMissing(FIRST_BABY) || _locationChecker.IsLocationMissing(SECOND_BABY);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CanGetPregnant_ShuffledPregnancies_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public bool setUp()
        public static bool Setup_PregnancyQuestionEvent_Prefix(QuestionEvent __instance, ref bool __result)
        {
            try
            {
                var whichQuestionField = _helper.Reflection.GetField<int>(__instance, "whichQuestion");
                var whichQuestion = whichQuestionField.GetValue();
                if (whichQuestion != 1 && whichQuestion != 3)
                {
                    return true; // run original logic
                }

                Response[] answerChoices1 = {
                    new("Yes", Game1.content.LoadString("Strings\\Events:HaveBabyAnswer_Yes")),
                    new("Not", Game1.content.LoadString("Strings\\Events:HaveBabyAnswer_No")),
                };
                string question;

                var npc = Game1.getCharacterFromName(Game1.player.spouse);

                if (npc.isRoommate() || npc.Name.Equals("Krobus"))
                {
                    question = orderBabyQuestion;
                }
                else if (npc.Gender == FEMALE)
                {
                    question = npcGiveBirthQuestion;
                }
                else
                {
                    question = playerGiveBirthQuestion;
                }

                var nextBirthLocation = _locationChecker.IsLocationMissing(FIRST_BABY) ? FIRST_BABY : SECOND_BABY;
                var scoutedItem = _archipelago.ScoutSingleLocation(nextBirthLocation);
                question = string.Format(question, scoutedItem.ItemName, Game1.player.Name);
                var answerPregnancyQuestionMethod = _helper.Reflection.GetMethod(__instance, "answerPregnancyQuestion");
                Game1.currentLocation.createQuestionDialogue(question, answerChoices1, (who, answer) => answerPregnancyQuestionMethod.Invoke(who, answer), npc);
                Game1.messagePause = true;
                __result = false;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Setup_PregnancyQuestionEvent_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // private void answerPregnancyQuestion(Farmer who, string answer)
        public static bool AnswerPregnancyQuestion_CorrectDate_Prefix(QuestionEvent __instance, Farmer who, string answer)
        {
            try
            {
                if (!answer.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                {
                    return false; // don't run original logic
                }

                var worldDate = new WorldDate(Game1.Date);
                worldDate.TotalDays += 14;
                who.GetSpouseFriendship().NextBirthingDate = worldDate;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerPregnancyQuestion_CorrectDate_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public bool tickUpdate(GameTime time)
        public static bool TickUpdate_BirthingEvent_Prefix(BirthingEvent __instance, GameTime time, ref bool __result)
        {
            try
            {
                var timerField = _helper.Reflection.GetField<int>(__instance, "timer");
                Game1.player.CanMove = false;
                timerField.SetValue(timerField.GetValue() + time.ElapsedGameTime.Milliseconds);
                Game1.fadeToBlackAlpha = 1f;

                if (timerField.GetValue() < 1500)
                {
                    __result = false;
                    return false; // don't run original logic
                }

                Game1.playSound("smallSelect");
                Game1.player.getSpouse().daysAfterLastBirth = 5;
                Game1.player.GetSpouseFriendship().NextBirthingDate = (WorldDate)null;
                var dialogueOptions = new[] { "NewChild_FirstChild", "NewChild_Adoption" };
                var chosenDialogue = dialogueOptions[Game1.random.Next(0, dialogueOptions.Length)];

                var locationBeingChecked = _locationChecker.IsLocationMissing(FIRST_BABY) ? FIRST_BABY : SECOND_BABY;
                var scoutedItem = _archipelago.ScoutSingleLocation(locationBeingChecked);
                var scoutedItemName = scoutedItem.ItemName;

                var marriageDialogue = new MarriageDialogueReference("Data\\ExtraDialogue", chosenDialogue, true, scoutedItemName);
                Game1.player.getSpouse().currentMarriageDialogue.Insert(0, marriageDialogue);

                var playerName = Lexicon.capitalize(Game1.player.Name);
                var spouseName = Game1.player.spouse;
                var pronoun = "it";
                var scoutedItemClassification = $"{scoutedItem.Classification.ToLower()} item";

                var multiplayerField = _helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer");
                var multiplayer = multiplayerField.GetValue();

                // "Baby" => "{0} and {1} welcomed a baby {2} to the family! They named {3} {4}."
                Game1.morningQueue.Enqueue(() => multiplayer.globalChatInfoMessage("Baby", playerName, spouseName, scoutedItemClassification, pronoun, scoutedItemName));
                
                // I think the following lines aren't necessary as I have prevented the keyboard dialogue from showing up in the first place. If things break, try uncommenting it.
                // if (Game1.keyboardDispatcher != null) Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber)null;
                Game1.player.Position = Utility.PointToVector2(Utility.getHomeOfFarmer(Game1.player).GetPlayerBedSpot()) * 64f;
                Game1.globalFadeToClear();

                _locationChecker.AddCheckedLocation(locationBeingChecked);
                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(TickUpdate_BirthingEvent_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
