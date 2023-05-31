/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;

namespace DialogueTester
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private static readonly string _commandName = "dh_testdialogue";
        private static readonly string _commandDescription = "Test some modded dialogue.";

        private static readonly string _commandUsage = $"\nUsage: \t\t\t{_commandName} <Dialogue ID> <NPC name>\n" +
                                                       $"Advanced usage: \t{_commandName} <Full dialogue path> <NPC name> -manual\n\n" +
                                                       $"Regular example: \t{_commandName} summer_Wed2 Abigail\n" +
                                                       $"Advanced example: \t{_commandName} Characters\\Dialogue\\Abigail:summer_Wed2 Abigail -manual";

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.ConsoleCommands.Add(_commandName,
                $"{_commandDescription}\n\n{_commandUsage}", this.TestDialogue);
        }

        private void PrintUsage(string error)
        {
            this.Monitor.Log(error, LogLevel.Warn);
            this.Monitor.Log(_commandUsage, LogLevel.Warn);
        }

        private void TestDialogue(string command, string[] args)
        {
            // TODO: Obviously, clean this up and modularise it heavily. It's currently very bad.
            if (!Context.IsWorldReady) // We only want to do this if we're actually in-game. May not be necessary.
            {
                this.Monitor.Log("You should only use this command once the game has loaded.", LogLevel.Info);

                return;
            }

            string dialogueIdOrKey, npcName;
            bool manualDialogueKey = false;

            // If we don't have the required amount of parameters, we quit.
            // TODO: Add default NPC case if one is missing.
            if (args.Length < 2 || args.Length > 3)
            {
                this.PrintUsage("Invalid number of arguments.");

                return;
            }

            // If our third argument is the -manual option, we set a bool to tell us to
            // use the passed dialogue key verbatim.
            if (args.Length == 3) manualDialogueKey = args[2].Equals("-manual");

            // Parameter 0 = Dialogue ID or full key
            // Parameter 1 = NPC name
            // Parameter 2 = Manual option
            dialogueIdOrKey = args[0];
            npcName = args[1];

            var speakingNpc = Utility.fuzzyCharacterSearch(npcName);
            string finalDialogue;

            if (speakingNpc == null)
            {
                // If the NPC doesn't exist, warn as appropriate and return.
                this.PrintUsage("Invalid NPC name.");

                return;
            }

            if (manualDialogueKey)
            {
                try // TODO: Make this not terrible. Works for now, though!
                {
                    finalDialogue = Game1.content.LoadStringReturnNullIfNotFound(dialogueIdOrKey);
                }
                catch (Exception e)
                {
                    finalDialogue = null;
                }

                if (finalDialogue == null)
                {
                    this.PrintUsage(
                        "Invalid dialogue ID. Maybe this character doesn't have this dialogue? Did you specify a full dialogue key path and forget the -manual option?");

                    return;
                }
            }
            else
            {
                try // TODO: Make this not terrible. Works for now, though!
                {
                    finalDialogue =
                        Game1.content.LoadStringReturnNullIfNotFound(
                            $"Characters\\Dialogue\\{npcName}:{dialogueIdOrKey}");
                }
                catch (Exception e)
                {
                    finalDialogue = null;
                }

                if (finalDialogue == null)
                {
                    this.PrintUsage(
                        "Invalid dialogue ID. Maybe this character doesn't have this dialogue? Did you specify a full dialogue key path and forget the -manual option?");

                    return;
                }
            }

            Game1.drawDialogue(speakingNpc, finalDialogue);
        }
    }
}
