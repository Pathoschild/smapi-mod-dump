using Harmony;
using PurrplingCore.Patching;
using QuestFramework.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuestFramework.Patches
{
    class DialoguePatch : Patch<DialoguePatch>
    {
        public override string Name => nameof(DialoguePatch);

        QuestManager QuestManager { get; }

        public DialoguePatch(QuestManager questManager)
        {
            this.QuestManager = questManager;
            Instance = this;
        }

        private static bool MatchQuestName(string input, out string questName)
        {
            string pattern = @"!\[(.+)\]";
            
            if (Regex.IsMatch(input, pattern))
            {
                questName = Regex.Match(input, pattern).Groups[1].Value;
                return true;
            }

            questName = null;
            return false;
        }

        private static bool Before_parseDialogueString(ref string masterString)
        {
            try
            {
                string pattern = @"!\[(.+)\]";

                foreach (Match m in Regex.Matches(masterString, pattern))
                {
                    string name = m.Groups[1].Value;
                    int id = Instance.QuestManager.ResolveGameQuestId(name);

                    masterString = masterString.Replace($"![{name}]", $"![{id}]");

                    if (id == -1)
                    {
                        Instance.Monitor.Log($"Unable to resolve quest id for `{name}` - quest doesn't exist.", LogLevel.Error);
                    }
                }
            }
            catch (Exception e)
            {
                Instance.LogFailure(e, nameof(Instance.Before_parseDialogueString));
            }

            return true;
        }

        private static bool Before_getCurrentDialogue(Dialogue __instance, List<string> ___dialogues, bool ___finishedLastDialogue)
        {
            try
            {
                if (__instance.currentDialogueIndex >= ___dialogues.Count || ___finishedLastDialogue)
                    return true;

                if (___dialogues.Count > 0 && MatchQuestName(___dialogues[__instance.currentDialogueIndex], out string questName))
                {
                    int id = int.Parse(questName);

                    ___dialogues[__instance.currentDialogueIndex] = ___dialogues[__instance.currentDialogueIndex].Replace("![" + questName + "]", "");

                    if (!Game1.player.hasQuest(id))
                    {
                        Instance.QuestManager.AcceptQuest(id);
                    }
                }
            }
            catch (Exception e)
            {
                Instance.LogFailure(e, nameof(Instance.Before_getCurrentDialogue));
            }

            return true;
        }

        protected override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Dialogue), nameof(Dialogue.getCurrentDialogue)),
                prefix: new HarmonyMethod(typeof(DialoguePatch), nameof(DialoguePatch.Before_getCurrentDialogue))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Dialogue), "parseDialogueString"),
                prefix: new HarmonyMethod(typeof(DialoguePatch), nameof(DialoguePatch.Before_parseDialogueString))
            );
        }
    }
}
