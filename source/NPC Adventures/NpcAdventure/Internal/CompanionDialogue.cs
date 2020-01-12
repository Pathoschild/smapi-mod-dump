using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Internal
{
    internal class CompanionDialogue : Dialogue
    {
        public string Tag { get; set; }
        public bool Remember { get; set; }
        public CompanionDialogue(string masterDialogue, NPC speaker) : base(masterDialogue, speaker)
        {
        }
    }
}
