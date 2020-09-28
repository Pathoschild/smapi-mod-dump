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
        private const string PATTERN = @"\[quest:([^\]]+?)( ([^\]]+?))?\]";
        public override string Name => nameof(DialoguePatch);

        QuestManager QuestManager { get; }

        public DialoguePatch(QuestManager questManager)
        {
            this.QuestManager = questManager;
            Instance = this;
        }

        private static bool MatchQuestName(string input, out string questName)
        {
            if (Regex.IsMatch(input, PATTERN))
            {
                Match match = Regex.Match(input, PATTERN);
                string name = match.Groups[1].Value;
                string modUid = match.Groups[3].Value;

                questName = string.IsNullOrEmpty(modUid) ? name : $"{name}@{modUid}";

                return true;
            }

            questName = null;
            return false;
        }

        private static bool Before_getCurrentDialogue(Dialogue __instance, List<string> ___dialogues, bool ___finishedLastDialogue)
        {
            try
            {
                if (__instance.currentDialogueIndex >= ___dialogues.Count || ___finishedLastDialogue)
                    return true;

                if (___dialogues.Count > 0 && MatchQuestName(___dialogues[__instance.currentDialogueIndex], out string questName))
                {
                    if (!int.TryParse(questName, out int id))
                    {
                        id = Instance.QuestManager.ResolveGameQuestId(questName);
                        
                        if (id == -1)
                            Instance.Monitor.Log($"Unable to resolve quest id for `{questName}` - quest doesn't exist.", LogLevel.Error);
                    }

                    ___dialogues[__instance.currentDialogueIndex] = Regex.Replace(___dialogues[__instance.currentDialogueIndex], PATTERN, "");

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
        }
    }
}
