using System;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace DynamicConversationTopics
{
    /// <summary>Class containing the mod's console commands.</summary>
    public class ConsoleCommands
    {
        /*********
        ** Accessors
        *********/
        protected static IModHelper Helper => ModEntry.Instance.Helper;
        protected static IMonitor Monitor => ModEntry.Instance.Monitor;
        protected static ModConfig Config => ModConfig.Instance;


        /*********
        ** Fields
        *********/
        protected static ITranslationHelper i18n = Helper.Translation;


        /*********
        ** Public methods
        *********/
        /// <summary>
        /// Use the Mod Helper to register the commands in this class.
        /// </summary>
        public static void Apply()
        {
            string NL = Environment.NewLine;

            Helper.ConsoleCommands.Add("DCT",
                "Provides some useful debugging commands for conversation topics." + NL + "Usage: DCT <command>" + NL + "Enter <DCT help> to see a list of available commands.",
                cmdDCT);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Invokes one of the DCT console commands (first argument specifies a subcommand).</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private static void cmdDCT(string _command, string[] _args)
        {
            string command;
            string[] args;
            string argtopic = "";

            try
            {
                if (!Context.IsWorldReady)
                {
                    Monitor.Log($"ERROR: a save game must be loaded before using these commands.", LogLevel.Info);
                    return;
                }

                if (_args.Length > 0)
                {
                    command = _args[0];
                    args = _args.Skip(1).ToArray();
                    if (args.Length > 0)
                    {
                        argtopic = args[0];
                    }

                    switch(command.ToLower())
                    {
                        case "show":
                            {
                                if (Game1.player.activeDialogueEvents.Count() == 0)
                                {
                                    Monitor.Log($"No conversation topics are active right now.", LogLevel.Info);
                                }
                                else
                                {
                                    List<string> topiclist = new List<string>();
                                    foreach (KeyValuePair<string, int> item in Game1.player.activeDialogueEvents.Pairs)
                                    {
                                        string listitem = item.Value.ToString().PadRight(7, ' ');
                                        listitem += item.Key;
                                        topiclist.Add(listitem);
                                    }
                                    Monitor.Log($"Active conversation topics:\n" +
                                        $"Days | Dialogue key\n" +
                                        string.Join("\n", topiclist), LogLevel.Info);
                                }
                                return;
                            }
                        case "add":
                            {
                                if (string.IsNullOrEmpty(argtopic))
                                {
                                    Monitor.Log($"ERROR: No <topic> argument was provided.\n" +
                                        $"Usage: DCT {command} <topic> [days]", LogLevel.Info);
                                }
                                else if (!Game1.player.activeDialogueEvents.ContainsKey(argtopic))
                                {
                                    int days = args.Count() > 1 ? Convert.ToInt32(args[1]) : 4;
                                    Game1.player.activeDialogueEvents.Add(argtopic, days);
                                    Monitor.Log($"Added conversation topic <{argtopic}> with length {days} days.", LogLevel.Info);
                                }
                                else
                                {
                                    Monitor.Log($"A conversation topic named <{argtopic}> is already active.", LogLevel.Info);
                                }
                                return;
                            }
                        case "remove":
                            {
                                if (string.IsNullOrEmpty(argtopic))
                                {
                                    Monitor.Log($"ERROR: No <topic> argument was provided.\n" +
                                        $"Usage: DCT {command} <topic>", LogLevel.Info);
                                }
                                else if (Game1.player.activeDialogueEvents.ContainsKey(argtopic))
                                {
                                    while (Game1.player.activeDialogueEvents.ContainsKey(argtopic))
                                    {
                                        Game1.player.activeDialogueEvents.Remove(argtopic);
                                    }
                                    Monitor.Log($"Removed conversation topic <{argtopic}>.", LogLevel.Info);
                                }
                                else
                                {
                                    Monitor.Log($"No active conversation topic <{argtopic}> was found.", LogLevel.Info);
                                }
                                return;
                            }
                        case "removeall":
                            {
                                Game1.player.activeDialogueEvents.Clear();
                                Monitor.Log($"All active conversation topics were removed.", LogLevel.Info);
                                return;
                            }
                        case "info":
                            {
                                string status = "Status: inactive";
                                string description = "No description found.";
                                if (string.IsNullOrEmpty(argtopic))
                                {
                                    Monitor.Log($"ERROR: No <topic> argument was provided.\n" +
                                        $"Usage: DCT {command} <topic>", LogLevel.Info);
                                }
                                else
                                {
                                    if (Game1.player.activeDialogueEvents.ContainsKey(argtopic))
                                    {
                                        int d = Game1.player.activeDialogueEvents[argtopic];
                                        status = $"Status: active      Days remaining: {d}";
                                    }
                                    if (true) //TODO: fancy lookup function that provides info
                                    {
                                        description = "This is a test description for a conversation topic.";
                                    }
                                    Monitor.Log($"Info lookup results for <{argtopic}>:\n" +
                                        $"{status}\n" +
                                        $"{description}", LogLevel.Info);
                                }
                                return;
                            }
                        case "test":
                            {
                                // TODO: make this add something to a "test topics" somewhere that the DialogueEditor pulls from.
                                return;
                            }
                        case "history":
                            {
                                if (string.IsNullOrEmpty(argtopic))
                                {
                                    Monitor.Log($"ERROR: No <topic> argument was provided.\n" +
                                        $"Usage: DCT {command} <topic>", LogLevel.Info);
                                    return;
                                }

                                var responded = new List<string>();
                                string suffix = "_" + argtopic;

                                foreach (string flag in Game1.player.mailReceived)
                                {
                                    if (flag.EndsWith(suffix))
                                    {
                                        string npc = flag.Remove(flag.Length - suffix.Length);
                                        responded.Add(npc);
                                    }
                                }

                                if (responded.Count > 0)
                                {
                                    responded.Sort();
                                    Monitor.Log($"Dialogue response history for <{argtopic}>:\n" +
                                        $"Characters spoken to - {string.Join(", ", responded)}", LogLevel.Info);
                                }
                                else
                                {
                                    Monitor.Log($"No response history found for <{argtopic}>.", LogLevel.Info);
                                }
                                return;
                            }
                        case "clear":
                            {
                                if (string.IsNullOrEmpty(argtopic))
                                {
                                    Monitor.Log($"ERROR: No <topic> argument was provided.\n" +
                                        $"Usage: DCT {command} <topic>", LogLevel.Info);
                                }
                                else
                                {
                                    string suffix = "_" + argtopic;
                                    int tflags = 0;
                                    int rflags = 0;
                                    var todelete = new List<string>();

                                    foreach (string flag in Game1.player.mailReceived)
                                    {   
                                        if (flag == argtopic) //TODO: Change this later when I add tracking for expired topics
                                        {
                                            todelete.Add(flag);
                                            tflags++;
                                        }
                                        else if (flag.EndsWith(suffix))
                                        {
                                            string name = flag.Remove(flag.Length - suffix.Length);
                                            if (ModConfig.NPCs.Contains(name)
                                                || (Game1.getCharacterFromName(name) != null))
                                            {
                                                todelete.Add(flag);
                                                rflags++;
                                            }
                                        }
                                    }
                                    foreach (string flag in todelete)
                                    {
                                        Game1.player.mailReceived.Remove(flag);
                                    }
                                    Monitor.Log($"{rflags} response flags and {tflags} topic flags were cleared.", LogLevel.Info);
                                }
                                return;
                            }
                        case "clearall":
                            {
                                //TODO: move these list structures to some other class
                                var vanillatopics = new List<string>()
                                {
                                   "cc_Begin", "cc_Complete", "cc_Boulder", "cc_Bridge", "cc_Bus", "cc_Greenhouse", "cc_Minecart", "joja_Begin", "movieTheater", "elliottGone", "ElliottGone1", "ElliottGone2", "ElliottGone3", "ElliottGone4", "ElliottGone5", "ElliottGone6", "ElliottGone7", "emilyFiber", "haleyCakewalk1", "haleyCakewalk2", "leahPaint", "pennyRedecorating", "samJob1", "samJob2", "samJob3", "sebastianFrog", "sebastianFrog2", "shaneSaloon1", "shaneSaloon2", "dumped_Girls", "dumped_Guys", "secondChance_Girls", "secondChance_Guys", "pamHouseUpgrade", "pamHouseUpgradeAnonymous", "willyCrabs", "Introduction", "FullCrabPond"
                                };
                                var DCTtopics = new List<string>()
                                {
                                   "DCT.Test0", "DCT.Test1", "DCT.Test2", "DCT.Test3", "DCT.Test4", "DCT.Test5", "DCT.Test6", "DCT.Test7", "DCT.Test8", "DCT.Test9"
                                };
                                List<string> alltopics = vanillatopics.Concat<string>(DCTtopics).ToList();

                                int tflags = 0;
                                int rflags = 0;
                                var todelete = new List<string>();

                                foreach (string flag in Game1.player.mailReceived)
                                {
                                    foreach (string topic in alltopics)
                                    {
                                        string suffix = "_" + topic;

                                        if (flag == topic) //TODO: Change this later when I add tracking for expired topics
                                        {
                                            todelete.Add(flag);
                                            tflags++;
                                        }
                                        else if (flag.EndsWith(suffix))
                                        {
                                            string name = flag.Remove(flag.Length - suffix.Length);
                                            if (ModConfig.NPCs.Contains(name)
                                                || (Game1.getCharacterFromName(name) != null))
                                            {
                                                todelete.Add(flag);
                                                rflags++;
                                            }
                                        }
                                    }
                                }
                                foreach (string flag in todelete)
                                {
                                    Game1.player.mailReceived.Remove(flag);
                                }
                                Monitor.Log($"{rflags} response flags and {tflags} topic flags were cleared.", LogLevel.Info);
                                return;
                            }
                        case "help":
                            break;
                        default:
                            Monitor.Log($"Unrecognized subcommand [{command}]\n" +
                                $"Please use DCT with one of the following: show, add, remove, removeall, info, test, history, clear, clearall, help", LogLevel.Info);
                            break;
                    }
                }

                Monitor.Log($"AVAILABLE COMMANDS\n" +
                    $"DCT show                 Show a list of active topics and days remaining on each\n" +
                    $"DCT add <topic> [days]   Add an active conversation topic. [days] defaults to 4\n" +
                    $"DCT remove <topic>       Remove an active conversation topic\n" +
                    $"DCT removeall            Remove ALL active conversation topics\n" +
                    $"DCT info <topic>         Look up a topic's status, plus description if available\n" +
                    $"DCT test <topic>         Give test dialogue to all NPCs for a conversation topic\n" +
                    $"DCT history <topic>      Lists all NPCs whose dialogues for a topic have been seen\n" +
                    $"DCT clear <topic>        Clear saved history of NPC responses to a conversation topic\n" +
                    $"DCT clearall             Clear ALL NPC responses for DCT and vanilla conversation topics\n" +
                    $"DCT help                 Show this list of commands", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command DCT failed:\n{ex}", LogLevel.Warn);
            }
        }
    }
}