/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.Objects.Items.Rewards;
using FishingTrawler.Framework.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FishingTrawler.Objects
{
    internal class Murphy : NPC
    {
        public Murphy() : base()
        {

        }

        public Murphy(AnimatedSprite sprite, Vector2 position, int facingDir, string name, Texture2D portrait, LocalizedContentManager content = null) : base(sprite, position, facingDir, name, content)
        {
            Portrait = portrait;
        }

        internal void DisplayDialogue(Farmer who)
        {
            who.Halt();
            who.faceGeneralDirection(getStandingPosition(), 0, opposite: false, useTileCalculations: false);

            string playerTerm = GetDialogue("dialogue.player_title." + (who.IsMale ? "male" : "female"));

            if (!who.hasOrWillReceiveMail("FishingTrawler_IntroductionsMurphy"))
            {
                CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.introduction", playerTerm), this));
                Game1.drawDialogue(this);

                who.modData[ModDataKeys.MURPHY_WAS_GREETED_TODAY_KEY] = "true";
                Game1.addMailForTomorrow("FishingTrawler_IntroductionsMurphy", true);
            }
            else if (who.modData[ModDataKeys.MURPHY_WAS_GREETED_TODAY_KEY].ToLower() == "false" && who.modData[ModDataKeys.MURPHY_SAILED_TODAY_KEY].ToLower() == "false")
            {
                if (Game1.IsRainingHere(currentLocation) || Game1.IsSnowingHere(currentLocation))
                {
                    CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.greeting_rainy", playerTerm), this));
                    Game1.drawDialogue(this);
                    Game1.afterDialogues = AskQuestionAfterGreeting;
                }
                else if (currentLocation is IslandSouthEast)
                {
                    CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.greeting_island", playerTerm), this));
                    Game1.drawDialogue(this);
                    Game1.afterDialogues = AskQuestionAfterGreeting;
                }
                else
                {
                    CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.greeting", playerTerm), this));
                    Game1.drawDialogue(this);
                    Game1.afterDialogues = AskQuestionAfterGreeting;
                }

                who.modData[ModDataKeys.MURPHY_WAS_GREETED_TODAY_KEY] = "true";
            }
            else if (who.modData[ModDataKeys.MURPHY_WAS_GREETED_TODAY_KEY].ToLower() == "true" && who.modData[ModDataKeys.MURPHY_HAS_SEEN_FLAG_KEY].ToLower() == "false" && PlayerHasUnidentifiedFlagInInventory(who))
            {
                CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.reward_explanation_flags", playerTerm), this));
                Game1.drawDialogue(this);
                Game1.afterDialogues = TakeAndIdentifyFlag;

                who.modData[ModDataKeys.MURPHY_HAS_SEEN_FLAG_KEY] = "true";
            }
            else if (who.modData[ModDataKeys.MURPHY_WAS_GREETED_TODAY_KEY].ToLower() == "true" && who.modData[ModDataKeys.MURPHY_SAILED_TODAY_KEY].ToLower() == "false")
            {
                // Show questions
                AskQuestionAfterGreeting();
            }
            else if (who.modData[ModDataKeys.MURPHY_SAILED_TODAY_KEY].ToLower() == "true" && who.modData[ModDataKeys.MURPHY_FINISHED_TALKING_KEY].ToLower() == "false")
            {
                string tripState = who.modData[ModDataKeys.MURPHY_WAS_TRIP_SUCCESSFUL_KEY].ToLower() == "true" ? "successful" : "failure";
                CurrentDialogue.Push(new Dialogue(GetDialogue(string.Concat("dialogue.after_trip_", tripState), playerTerm), this));
                Game1.drawDialogue(this);

                who.modData[ModDataKeys.MURPHY_FINISHED_TALKING_KEY] = "true";
            }
            else if (who.modData[ModDataKeys.MURPHY_FINISHED_TALKING_KEY].ToLower() == "true" && PlayerHasUnidentifiedFlagInInventory(who))
            {
                CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.identify_flag", playerTerm), this));
                Game1.drawDialogue(this);
                Game1.afterDialogues = TakeAndIdentifyFlag;
            }
            else if (who.modData[ModDataKeys.MURPHY_FINISHED_TALKING_KEY].ToLower() == "true")
            {
                CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.trip_finished", playerTerm), this));
                Game1.drawDialogue(this);
            }
        }

        public override bool checkAction(Farmer who, GameLocation l)
        {
            if (who.CurrentItem != null && AncientFlag.IsFlag(who.CurrentItem) is true)
            {
                tryToReceiveActiveObject(who);
                return true;
            }

            DisplayDialogue(who);
            return true;
        }

        public override void tryToReceiveActiveObject(Farmer who)
        {
            who.Halt();
            who.faceGeneralDirection(getStandingPosition(), 0, opposite: false, useTileCalculations: false);

            if (who.CurrentItem != null && AncientFlag.IsFlag(who.CurrentItem) is true)
            {
                var ancientFlagType = AncientFlag.GetFlagType(who.CurrentItem);
                if (ancientFlagType == FlagType.Unknown)
                {
                    return;
                }

                string playerTerm = GetDialogue("dialogue.player_title." + (who.IsMale ? "male" : "female"));

                who.currentLocation.localSound("coin");
                who.reduceActiveItemByOne();

                if (FishingTrawler.GetHoistedFlag() == FlagType.Unknown)
                {
                    CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.given_flag_to_hoist", playerTerm), this));
                }
                else
                {
                    CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.given_flag_to_hoist_return_old", playerTerm), this));
                    who.addItemByMenuIfNecessary(AncientFlag.CreateInstance(FishingTrawler.GetHoistedFlag()));

                    // Set their toolbar to one to avoid player accidentally giving back the flag they just got?
                    who.CurrentToolIndex = 0;
                }

                Game1.drawDialogue(this);
                FishingTrawler.SetHoistedFlag(ancientFlagType);
            }
        }

        private void ConfirmFirstTrip(Farmer who)
        {
            string playerTerm = GetDialogue("dialogue.player_title." + (who.IsMale ? "male" : "female"));
            Response[] answers = new Response[2]
            {
                new Response("YesExplain", FishingTrawler.i18n.Get("response.yes_explain")),
                new Response("NoExplain", FishingTrawler.i18n.Get("response.no_explain"))
            };

            currentLocation.createQuestionDialogue(GetDialogue("dialogue.confirm_first_trip", playerTerm), answers, OnPlayerResponse, this);
        }

        private void ExplainMinigame(Farmer who)
        {
            string playerTerm = GetDialogue("dialogue.player_title." + (who.IsMale ? "male" : "female"));

            CurrentDialogue.Push(new Dialogue(GetMinigameExplanation(playerTerm), this));
            Game1.drawDialogue(this);

            if (!who.hasOrWillReceiveMail("PeacefulEnd.FishingTrawler_minigameExplanation"))
            {
                Game1.addMailForTomorrow("PeacefulEnd.FishingTrawler_minigameExplanation", true);
            }
        }

        private string GetDialogue(string dialogueTitle, object playerTitle = null)
        {
            return string.Format(FishingTrawler.i18n.Get(dialogueTitle), playerTitle);
        }

        private string GetMinigameExplanation(object title = null)
        {
            return string.Concat(GetDialogue("dialogue.minigame_explanation_hull", title), GetDialogue("dialogue.minigame_explanation_bailing", title), GetDialogue("dialogue.minigame_explanation_nets", title), GetDialogue("dialogue.minigame_explanation_fuel", title), GetDialogue("dialogue.minigame_explanation_computer", title), GetDialogue("dialogue.minigame_explanation_finish", title));
        }

        private void AskQuestionAfterGreeting()
        {
            string playerTerm = GetDialogue("dialogue.player_title." + (Game1.player.IsMale ? "Male" : "Female"));

            List<Response> answers = new List<Response>()
            {
                new Response("StartTrip", FishingTrawler.i18n.Get("response.start_trip"))
            };

            if (PlayerHasUnidentifiedFlagInInventory(Game1.player))
            {
                answers.Add(new Response("IdentifyFlag", FishingTrawler.i18n.Get("response.found_another_flag")));
            }
            if (Context.IsMultiplayer)
            {
                answers.Add(new Response("MultipleDeckhands", FishingTrawler.i18n.Get("response.bring_some_friends")));
            }

            answers.Add(new Response("GotQuestion", FishingTrawler.i18n.Get("response.more_questions")));
            answers.Add(new Response("NoDeparture", FishingTrawler.i18n.Get("response.no_departure")));

            currentLocation.createQuestionDialogue(GetDialogue("dialogue.options", playerTerm), answers.ToArray(), OnPlayerResponse, this);
        }

        private void StartDepartureDialogue(Farmer who)
        {
            string playerTerm = GetDialogue("dialogue.player_title." + (who.IsMale ? "male" : "female"));

            // Verify main player has empty spot for bucket and fuel stacks
            if (who.freeSpotsInInventory() < 2)
            {
                CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.full_inventory", playerTerm), this));
                Game1.drawDialogue(this);
                return;
            }

            if (FishingTrawler.GetFarmersOnTrawler().Count > 0 || !FishingTrawler.ShouldMurphyAppear(currentLocation))
            {
                // Do nothing and bail, as Murphy is being interacted with on beach despite being on Trawler
                FishingTrawler.monitor.Log($"{who.Name} tried to start a trip while one already departed!", LogLevel.Trace);
                return;
            }

            // Check if any deckhands have open menus
            //List<Farmer> deckhands = ModEntry.trawlerObject.GetFarmersToDepart(true);

            CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.start_departure", playerTerm), this));
            Game1.afterDialogues = delegate () { FishingTrawler.trawlerObject.StartDeparture(who); };
            Game1.drawDialogue(this);
        }

        private void HowToHoistDialogue(Farmer who)
        {
            string playerTerm = GetDialogue("dialogue.player_title." + (who.IsMale ? "male" : "female"));

            CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.how_to_hoist_flag", playerTerm), this));
            Game1.drawDialogue(this);
        }

        private void WhatFlagIsHoisted(Farmer who)
        {
            string flagName = AncientFlag.GetFlagName(FishingTrawler.GetHoistedFlag());
            if (FishingTrawler.modHelper.Translation.LocaleEnum == LocalizedContentManager.LanguageCode.en)
            {
                flagName = flagName.Replace("The", "the");
                if (!flagName.Contains("the"))
                {
                    flagName = string.Concat("the", " ", flagName);
                }
            }

            CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.what_flag_is_hoisted" + (FishingTrawler.GetHoistedFlag() == FlagType.Unknown ? "_None" : ""), flagName), this));
            Game1.drawDialogue(this);
        }

        private void RemoveHoistedFlag(Farmer who)
        {
            string playerTerm = GetDialogue("dialogue.player_title." + (who.IsMale ? "male" : "female"));

            CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.remove_current_flag", playerTerm), this));
            Game1.drawDialogue(this);

            Game1.player.addItemByMenuIfNecessary(AncientFlag.CreateInstance(FishingTrawler.GetHoistedFlag()));
            FishingTrawler.SetHoistedFlag(FlagType.Unknown);
        }

        private void IdentifyFlag(Farmer who)
        {
            string playerTerm = GetDialogue("dialogue.player_title." + (who.IsMale ? "male" : "female"));

            CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.identify_flag", playerTerm), this));
            Game1.drawDialogue(this);
            Game1.afterDialogues = TakeAndIdentifyFlag;
        }

        private void MultipleDeckhands(Farmer who)
        {
            string playerTerm = GetDialogue("dialogue.player_title." + (who.IsMale ? "male" : "female"));

            CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.multiple_deckhands", playerTerm), this));
            Game1.drawDialogue(this);
        }

        private void SayGoodbye(Farmer who)
        {
            string playerTerm = GetDialogue("dialogue.player_title." + (who.IsMale ? "male" : "female"));

            CurrentDialogue.Push(new Dialogue(GetDialogue("dialogue.goodbye", playerTerm), this));
            Game1.drawDialogue(this);
        }

        private void ShowMoreQuestions(Farmer who)
        {
            string playerTerm = GetDialogue("dialogue.player_title." + (who.IsMale ? "male" : "female"));
            List<Response> answers = new List<Response>()
            {
                new Response("MinigameExplanation", FishingTrawler.i18n.Get("response.what_does_deckhand_do"))
            };

            if (PlayerHasIdentifiedFlagInInventory(Game1.player))
            {
                answers.Add(new Response("WantToHoist", FishingTrawler.i18n.Get("response.like_to_hoist_flag")));
            }
            if (Game1.player.modData[ModDataKeys.MURPHY_HAS_SEEN_FLAG_KEY] == "true")
            {
                answers.Add(new Response("WhatFlag", FishingTrawler.i18n.Get("response.what_flag_is_trawler_flying")));
            }
            if (FishingTrawler.GetHoistedFlag() != FlagType.Unknown)
            {
                answers.Add(new Response("GetFlag", FishingTrawler.i18n.Get("response.want_have_flag_back")));
            }

            answers.Add(new Response("NeverMind", FishingTrawler.i18n.Get("response.never_mind")));

            currentLocation.createQuestionDialogue(GetDialogue("dialogue.more_questions", playerTerm), answers.ToArray(), OnPlayerResponse, this);
        }

        private void OnPlayerResponse(Farmer who, string answer)
        {
            switch (answer)
            {
                case "StartTrip":
                    if (!who.hasOrWillReceiveMail("PeacefulEnd.FishingTrawler_minigameExplanation"))
                    {
                        Game1.afterDialogues = delegate () { ConfirmFirstTrip(who); };
                        Game1.addMailForTomorrow("PeacefulEnd.FishingTrawler_minigameExplanation", true);
                    }
                    else
                    {
                        // Start trip
                        StartDepartureDialogue(who);
                    }
                    break;
                case "MinigameExplanation":
                case "YesExplain":
                    ExplainMinigame(who);
                    break;
                case "NoExplain":
                    // Start trip
                    StartDepartureDialogue(who);
                    break;
                case "WantToHoist":
                    Game1.afterDialogues = delegate () { HowToHoistDialogue(who); };
                    break;
                case "WhatFlag":
                    Game1.afterDialogues = delegate () { WhatFlagIsHoisted(who); };
                    break;
                case "GetFlag":
                    Game1.afterDialogues = delegate () { RemoveHoistedFlag(who); };
                    break;
                case "IdentifyFlag":
                    Game1.afterDialogues = delegate () { IdentifyFlag(who); };
                    break;
                case "MultipleDeckhands":
                    Game1.afterDialogues = delegate () { MultipleDeckhands(who); };
                    break;
                case "GotQuestion":
                    Game1.afterDialogues = delegate () { ShowMoreQuestions(who); };
                    break;
                case "NeverMind":
                    Game1.afterDialogues = delegate () { SayGoodbye(who); };
                    break;
            }
        }

        private bool PlayerHasUnidentifiedFlagInInventory(Farmer who)
        {
            return who.Items.Any(i => i != null && i.modData.ContainsKey(ModDataKeys.ANCIENT_FLAG_KEY) && i.modData[ModDataKeys.ANCIENT_FLAG_KEY] == FlagType.Unknown.ToString());
        }

        private bool PlayerHasIdentifiedFlagInInventory(Farmer who)
        {
            return who.Items.Any(i => i != null && i.modData.ContainsKey(ModDataKeys.ANCIENT_FLAG_KEY) && i.modData[ModDataKeys.ANCIENT_FLAG_KEY] != FlagType.Unknown.ToString());
        }

        private void TakeAndIdentifyFlag()
        {
            if (!PlayerHasUnidentifiedFlagInInventory(Game1.player))
            {
                return;
            }

            // Get the count of possible flags
            int uniqueFlagTypes = Enum.GetNames(typeof(FlagType)).Length;

            // Remove the ancient flag, then add the randomly identified one
            var ancientFlag = Game1.player.Items.FirstOrDefault(i => AncientFlag.IsFlag(i) && AncientFlag.GetFlagType(i) == FlagType.Unknown);

            Game1.player.removeItemFromInventory(ancientFlag);
            Game1.player.addItemByMenuIfNecessary(AncientFlag.CreateInstance((FlagType)Game1.random.Next(1, uniqueFlagTypes)));
        }
    }
}
