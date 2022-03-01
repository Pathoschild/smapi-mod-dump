/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/CustomEmotes
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomEmotes
{
    internal static class HarmonyPatches
    {
        private static readonly string EMOTE_PATTERN = @"\%emote_(?<emoteName>[A-z0-9]+)";
        private static CustomEmotes Instance => CustomEmotes.Instance;

        public static bool Prefix_command_emote(Event __instance, GameLocation location, GameTime time, ref string[] split)
        {
            if (split.Length > 2 && !int.TryParse(split[2], out var _))
            {
                if (Instance.EmoteIndexMap.TryGetValue(split[2], out int index))
                {
                    split[2] = index.ToString();
                    return true;
                }

                Instance.Monitor.Log($"Unknown emote '{split[2]}' ");
                __instance.CurrentCommand++;
                __instance.checkForNextCommand(location, time);

                return false;
            }

            return true;
        }

        public static void Prefix_getCurrentDialogue(Dialogue __instance, ref string __result)
        {
            if (__instance.dialogues[__instance.currentDialogueIndex].Contains("%emote_"))
            {
                var api = Instance.GetApi();
                var m = Regex.Match(__instance.dialogues[__instance.currentDialogueIndex], EMOTE_PATTERN);

                if (!m.Success) return;

                string emoteName = m.Groups["emoteName"].Value;
                DelayedAction.functionAfterDelay(() => api.DoEmote(__instance.speaker, emoteName), 10);

                __result = __instance.dialogues[__instance.currentDialogueIndex] 
                    = Regex.Replace(__instance.dialogues[__instance.currentDialogueIndex], EMOTE_PATTERN, "");
            }
        }
    }
}
