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
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewArchipelago.GameModifications.CodeInjections;
using StardewArchipelago.Goals;
using StardewArchipelago.Locations.CodeInjections;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.Archipelago
{
    public class ChatForwarder
    {
        public const string COMMAND_PREFIX = "!!";

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private Harmony _harmony;
        private static GiftHandler _giftHandler;
        private static BankHandler _bankHandler;

        public ChatForwarder(IMonitor monitor, IModHelper helper, Harmony harmony, ArchipelagoClient archipelago, GiftHandler giftHandler)
        {
            _monitor = monitor;
            _helper = helper;
            _harmony = harmony;
            _archipelago = archipelago;
            _giftHandler = giftHandler;
            _bankHandler = new BankHandler(_archipelago);
        }

        public void ListenToChatMessages()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(ChatBox), nameof(ChatBox.receiveChatMessage)),
                postfix: new HarmonyMethod(typeof(ChatForwarder), nameof(ReceiveChatMessage_ForwardToAp_PostFix))
            );
        }

        public static void ReceiveChatMessage_ForwardToAp_PostFix(ChatBox __instance, long sourceFarmer, int chatKind, LocalizedContentManager.LanguageCode language, string message)
        {
            try
            {
                if (sourceFarmer == 0 || chatKind != 0)
                {
                    return;
                }

                if (TryHandleCommand(message))
                {
                    return;
                }

                var messagesField = _helper.Reflection.GetField<List<ChatMessage>>(__instance, "messages");
                var messages = messagesField.GetValue();
                messages.RemoveAt(messages.Count - 1);
                _archipelago.SendMessage(message);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ReceiveChatMessage_ForwardToAp_PostFix)}:\n{ex}", LogLevel.Error);
            }
        }

        private static bool TryHandleCommand(string message)
        {
            if (message == null || !message.StartsWith(COMMAND_PREFIX))
            {
                return false;
            }

            var messageLower = message.ToLower();
            if (HandleGoalCommand(messageLower))
            {
                return true;
            }

            if (HandleExperienceCommand(messageLower))
            {
                return true;
            }

            if (HandleArcadeReleaseCommand(messageLower))
            {
                return true;
            }

            if (_giftHandler.HandleGiftItemCommand(message))
            {
                return true;
            }

            if (_bankHandler.HandleBankCommand(message))
            {
                return true;
            }

            if (HandleHideEmptyLettersCommand(messageLower))
            {
                return true;
            }

            if (HandleOverrideSpriteRandomizerCommand(messageLower))
            {
                return true;
            }

            if (HandleSyncCommand(messageLower))
            {
                return true;
            }

            if (HandleDeathlinkCommand(messageLower))
            {
                return true;
            }

            if (HandleHelpCommand(messageLower))
            {
                return true;
            }

            if (message.StartsWith(COMMAND_PREFIX))
            {
                Game1.chatBox?.addMessage($"Unrecognized command. Use {COMMAND_PREFIX}help for a list of commands", Color.Gold);
                return true;
            }

            return false;
        }

        private static bool HandleGoalCommand(string message)
        {
            if (message != $"{COMMAND_PREFIX}goal")
            {
                return false;
            }

            var goal = GoalCodeInjection.GetGoalString();
            var goalMessage = $"Your Goal is: {goal}";
            Game1.chatBox?.addMessage(goalMessage, Color.Gold);
            return true;
        }

        private static bool HandleExperienceCommand(string message)
        {
            if (message != $"{COMMAND_PREFIX}experience")
            {
                return false;
            }

            var skillsExperiences = SkillInjections.GetArchipelagoExperienceForPrinting();
            foreach (var skill in skillsExperiences)
            {
                Game1.chatBox?.addMessage(skill, Color.Gold);
            }

            return true;
        }

        private static bool HandleSyncCommand(string message)
        {
            if (message != $"{COMMAND_PREFIX}sync")
            {
                return false;
            }

            _archipelago.Sync();
            return true;
        }

        private static bool HandleDeathlinkCommand(string message)
        {
            if (message != $"{COMMAND_PREFIX}deathlink")
            {
                return false;
            }

            _archipelago.ToggleDeathlink();
            return true;
        }

        private static bool HandleHideEmptyLettersCommand(string message)
        {
            if (message != $"{COMMAND_PREFIX}letters")
            {
                return false;
            }

            var currentSetting = ModEntry.Instance.State.HideEmptyArchipelagoLetters;
            var newSetting = !currentSetting;
            var status = newSetting ? "hidden" : "visible";
            ModEntry.Instance.State.HideEmptyArchipelagoLetters = newSetting;
            Game1.chatBox?.addMessage($"Empty archipelago letters are now {status}. Changes will take effect when opening your mailbox", Color.Gold);
            return true;
        }

        private static bool HandleArcadeReleaseCommand(string message)
        {
            var arcadePrefix = $"{COMMAND_PREFIX}arcade_release ";
            if (!message.StartsWith(arcadePrefix))
            {
                return false;
            }

            var remainder = message.Substring(arcadePrefix.Length);

            var isJunimoCart = IsJunimoKart(remainder);
            var isPrairieKing = IsPrairieKing(remainder);

            if (!isJunimoCart && !isPrairieKing)
            {
                Game1.chatBox?.addMessage($"Unrecognized arcade game: {remainder} (Options: JotPK, JK)", Color.Gold);
                return true;
            }

            if (isJunimoCart)
            {
                if (!_archipelago.GetAllCheckedLocations().Keys.Contains(ArcadeMachineInjections.JK_VICTORY))
                {
                    Game1.chatBox?.addMessage($"You must complete Junimo Kart before releasing it", Color.Gold);
                    return true;
                }

                Game1.chatBox?.addMessage($"Releasing all remaining checks in Junimo Kart", Color.Gold);
                ArcadeMachineInjections.ReleaseJunimoKart();
                return true;
            }

            if (isPrairieKing)
            {
                if (!_archipelago.GetAllCheckedLocations().Keys.Contains(ArcadeMachineInjections.JOTPK_VICTORY))
                {
                    Game1.chatBox?.addMessage($"You must complete Journey of the Prairie King before releasing it", Color.Gold);
                    return true;
                }

                Game1.chatBox?.addMessage($"Releasing all remaining checks in Journey of the Prairie King", Color.Gold);
                ArcadeMachineInjections.ReleasePrairieKing();
                return true;
            }

            return false;
        }

        private static bool IsJunimoKart(string remainder)
        {
            var trimmedToMinimum = remainder.Replace(" ", "").Replace("-", "").Replace("_", "").ToLower();
            return trimmedToMinimum is "jk" or "junimokart" or "junimocart" or "junimo" or "kart" or "cart";
        }

        private static bool IsPrairieKing(string remainder)
        {
            var trimmedToMinimum = remainder.Replace(" ", "").Replace("-", "").Replace("_", "").ToLower();
            return trimmedToMinimum is "jotpk" or "journeyoftheprairieking" or "journey" or "prairieking" or "prairie" or "king";
        }

        private static bool HandleOverrideSpriteRandomizerCommand(string message)
        {
            if (!message.ToLower().Equals($"{COMMAND_PREFIX}sprite"))
            {
                return false;
            }

            var currentOverride = ModEntry.Instance.State.AppearanceRandomizerOverride;
            var overrideStatus = "off";
            if (currentOverride == null || currentOverride == AppearanceRandomization.Disabled)
            {
                currentOverride = AppearanceRandomization.Villagers;
                overrideStatus = "on";
            }
            else
            {
                currentOverride = AppearanceRandomization.Disabled;
            }
            ModEntry.Instance.State.AppearanceRandomizerOverride = currentOverride;
            Game1.chatBox?.addMessage($"Sprite Randomizer is now {overrideStatus}. Changes will take effect after sleeping, then reloading your game.", Color.Gold);
            return true;
        }

        private static bool HandleHelpCommand(string message)
        {
            if (message != $"{COMMAND_PREFIX}help")
            {
                return false;
            }

            PrintCommandHelp();
            return true;
        }

        private static void PrintCommandHelp()
        {
            Game1.chatBox?.addMessage($"{COMMAND_PREFIX}help - Shows the list of client commands", Color.Gold);
            Game1.chatBox?.addMessage($"{COMMAND_PREFIX}goal - Shows your current Archipelago Goal", Color.Gold);
            Game1.chatBox?.addMessage($"{COMMAND_PREFIX}experience - Shows your current progressive skills experience levels", Color.Gold);
            Game1.chatBox?.addMessage($"{COMMAND_PREFIX}bank [deposit|withdraw] [amount] - Deposit or withdraw money from your shared bank account", Color.Gold);
            Game1.chatBox?.addMessage($"{COMMAND_PREFIX}gift [slotName] - Sends your currently held item stack to a chosen player as a gift", Color.Gold);
            Game1.chatBox?.addMessage($"{COMMAND_PREFIX}letters - Toggle Hiding Empty Archipelago Letters", Color.Gold);
            Game1.chatBox?.addMessage($"{COMMAND_PREFIX}deathlink - Toggles Deathlink on/off. Saves when sleeping", Color.Gold);
            Game1.chatBox?.addMessage($"{COMMAND_PREFIX}sprite - Enable/Disable the sprite randomizer", Color.Gold);
#if DEBUG
            Game1.chatBox?.addMessage($"{COMMAND_PREFIX}sync - Sends a Sync packet to the Archipelago server", Color.Gold);
#endif
            Game1.chatBox?.addMessage($"{COMMAND_PREFIX}arcade_release [game] - Releases all remaining checks in an arcade machine that you have already completed", Color.Gold);
        }
    }
}
