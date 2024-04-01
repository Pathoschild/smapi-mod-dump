/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using StardewValley;

namespace HugsAndKisses
{
    public interface IKissingAPI
    {
        public void PlayerNPCKiss(Farmer farmer, NPC npc);
        public void NPCKiss(NPC kisser, NPC kissee);
        public SoundEffect GetKissSound();
        public SoundEffect GetHugSound();
        public int LastKissed(string name);
    }
}