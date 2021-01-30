/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

namespace TrashCanReactions
{
    public class Reactor
    {
        public string dialogue;
        public int emote;
        public int points;

        public Reactor(int emote, int points, string dialogue)
        {
            this.emote = emote;
            this.points = points;
            this.dialogue = dialogue;
        }
    }
}