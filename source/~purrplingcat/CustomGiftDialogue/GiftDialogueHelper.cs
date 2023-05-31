/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using PurrplingCore.Dialogues;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace CustomGiftDialogue
{
    internal static class GiftDialogueHelper
    {
        static readonly int[] heartLevels = { 14, 12, 10, 8, 6, 4, 2 };
        /// <summary>
        /// Try find a gift reaction dialogue text for gifted object to an NPC
        /// </summary>
        /// <param name="npc">NPC had recieved a gift</param>
        /// <param name="obj">Gifted item recieved by this NPC</param>
        /// <param name="dialogue">A reaction dialogue text for this gifted item to this NPC</param>
        /// <returns>True if any gift reaction dialogue was found, otherwise false</returns>
        public static bool FetchGiftReaction(NPC npc, SObject obj, out string dialogue, string suffix = "")
        {
            HashSet<string> possibleKeys = new () { 
                $"GiftReaction_{obj.Name.Replace(' ', '_')}{suffix}",
                $"GiftReactionCategory_{obj.Category}{suffix}",
                $"GiftReactionPreserved_{obj.preserve.Value}{suffix}",
                $"GiftReactionPreserved_{(obj.preserve.Value.HasValue ? "Any" : "")}{suffix}",
            };

            // Add item context tags as possibly dialogue lines for gift reactions
            possibleKeys.UnionWith(obj.GetContextTags().Select(tag => $"GiftReactionTag_{tag}"));

            foreach (string dialogueKey in possibleKeys)
            {
                if (DialogueHelper.GetRawDialogue(npc.Dialogue, dialogueKey, out KeyValuePair<string, string> reaction))
                {
                    dialogue = reaction.Value;
                    return true;
                }
            }

            dialogue = "";
            return false;
        }

        /// <summary>
        /// Cancels current spoken dialogue for an NPC. 
        /// DialogueBox for this dialogue will not shown.
        /// </summary>
        /// <param name="speaker">The NPC speaker</param>
        public static void CancelCurrentDialogue(NPC speaker)
        {
            if (Game1.activeClickableMenu is DialogueBox dialogueBox && Game1.currentSpeaker == speaker)
            {
                // We wants override the default gift reaction dialogue
                if (speaker.CurrentDialogue.Count > 0
                    && GetDialogueBoxDialogue(dialogueBox) == speaker.CurrentDialogue.Peek())
                {
                    speaker.CurrentDialogue.Pop();
                }

                Game1.dialogueUp = false;
                Game1.currentSpeaker = null;
                Game1.activeClickableMenu = null;
            }
        }

        public static bool GetRevealDialogue(NPC npc, out string dialogue, string npcName = null)
        {
            dialogue = null;
            var dispositionsData = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
            var randomNpcName = npcName ?? dispositionsData.ElementAt(Game1.random.Next(dispositionsData.Count)).Key;

            // For a random player known NPC
            if (Game1.player.friendshipData.ContainsKey(randomNpcName))
            {

                // With heart level
                int heartLevel = Game1.player.friendshipData[randomNpcName].Points / 250;
                foreach (int targetHeartLevel in heartLevels)
                {
                    if (heartLevel >= targetHeartLevel && DialogueHelper.GetRawDialogue(npc.Dialogue, $"Reveal_{randomNpcName}{targetHeartLevel}", out var dialoguePair))
                    {
                        dialogue = dialoguePair.Value;
                        return true;
                    }
                }

                // Without heart level
                if (DialogueHelper.GetRawDialogue(npc.Dialogue, $"Reveal_{randomNpcName}", out var dialoguePair2))
                {
                    dialogue = dialoguePair2.Value;
                    return true;
                }
            }

            // Fallback reveal dialogue
            if (DialogueHelper.GetRawDialogue(npc.Dialogue, $"Reveal", out var dialoguePair3))
            {
                dialogue = dialoguePair3.Value;
                return true;
            }

            return false;
        }

        public static bool TryGetFriendshipDialogue(NPC n, string key, out string dialogue)
        {
            // With heart level
            int heartLevel = Game1.player.friendshipData[n.Name].Points / 250;
            foreach (int targetHeartLevel in heartLevels)
            {
                if (heartLevel >= targetHeartLevel && DialogueHelper.GetRawDialogue(n.Dialogue, $"{key}{targetHeartLevel}", out var dialoguePair))
                {
                    dialogue = dialoguePair.Value;
                    return true;
                }
            }

            // Without heart level
            if (DialogueHelper.GetRawDialogue(n.Dialogue, key, out var dialoguePair2))
            {
                dialogue = dialoguePair2.Value;
                return true;
            }

            dialogue = "";
            return false;
        }

        private static Dialogue GetDialogueBoxDialogue(DialogueBox dialogueBox)
        {
            return CustomGiftDialogueMod.Reflection
                .GetField<Dialogue>(dialogueBox, "characterDialogue")
                .GetValue();
        }
    }
}
