/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StardewModdingAPI;
using StardewValley;

namespace LastingCTs;

internal class ConsoleCommandManager
{
    internal static void InitializeConsoleCommands()
    {
        Globals.CCHelper.Add("sophie.ct.list", "Prints current conversation topics and durations.", (_, _) =>
            {
                if (!Context.IsWorldReady)
                {
                    Log.Warn("This command should only be used while a save is loaded.");
                    return;
                }

                try
                {
                    StringBuilder output = new("Active conversation topics (days remaining):\n");
                    var permanentCTs = Globals.GameContent.Load<Dictionary<string, string>>(Globals.ContentPath);

                    foreach (KeyValuePair<string, int> kvp in Game1.player.activeDialogueEvents.Pairs)
                    {
                        output.AppendLine(permanentCTs.ContainsKey(kvp.Key)
                            ? $"\t{kvp.Key} (Permanent)"
                            : $"\t{kvp.Key} ({kvp.Value})");
                    }

                    Log.Info(output.ToString());
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to print conversation topics with exception: {ex}");
                }
            }
        );

        Globals.CCHelper.Add("sophie.ct.listbynpc",
            "Lists CTs which the specified NPC has responded to, or which all NPCs have responded to if none is specified.",
            (_, args) =>
            {
                if (!Context.IsWorldReady)
                {
                    Log.Warn("This command should only be used while a save is loaded.");
                    return;
                }

                try
                {
                    StringBuilder output = new("Conversation topics responded to by NPC:");

                    if (args.Any())
                    {
                        foreach (string npc in args)
                        {
                            output.AppendLine($"\n\t{npc}:");

                            output.AppendLine("\t\t" + string.Join("\n\t\t",
                                Game1.player.mailReceived.Where(mail =>
                                    mail.Contains(npc, StringComparison.OrdinalIgnoreCase))));
                        }
                    }
                    else
                    {
                        var npcList = Utility.getAllCharacters(new List<NPC>()).Where(npc => npc.isVillager())
                            .Select(npc => npc.Name);

                        output.AppendLine("\n\t" + string.Join("\n\t",
                            Game1.player.mailReceived.Where(mail =>
                                mail.Contains('_') && npcList.Contains(mail[..mail.IndexOf('_')]))));
                    }

                    Log.Info(output.ToString());
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to list CTs responded to by NPC with exception: {ex}");
                }
            }
        );
    }
}
