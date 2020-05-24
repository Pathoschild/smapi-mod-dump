using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace NpcAdventure.Dialogues
{
    internal class CompanionDialogue : Dialogue
    {
        public string Tag { get; set; }
        public string Kind { get => DialogueKindOf(this.Tag); }
        public bool Remember { get; set; }
        public HashSet<string> SpecialAttributes { get; set; }

        public CompanionDialogue(string masterDialogue, NPC speaker) : base(masterDialogue, speaker)
        {
            this.SpecialAttributes = new HashSet<string>();
        }

        public static CompanionDialogue Create(string masterDialogue, NPC speaker, string tag = null)
        {
            var dialogue = new CompanionDialogue(masterDialogue, speaker);

            if (tag != null)
            {
                dialogue.Tag = $"{speaker.Name}.{tag}";
            }

            return dialogue;
        }

        public static string DialogueKindOf(string dialogueTag)
        {
            return dialogueTag?.Split(DialogueProvider.FLAG_RANDOM, DialogueProvider.FLAG_CHANCE).First();
        }
    }
}
