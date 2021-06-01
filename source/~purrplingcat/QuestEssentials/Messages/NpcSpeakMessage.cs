/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using QuestFramework.Messages;
using StardewValley;

namespace QuestEssentials.Messages
{
    public class NpcSpeakMessage : StoryMessage, ITalkMessage
    {
        public Farmer Farmer { get; }

        public NPC Npc { get; }

        public NpcSpeakMessage(Farmer farmer, NPC speaker) : base("NpcSpeak")
        {
            this.Farmer = farmer;
            this.Npc = speaker;
        }
    }
}
