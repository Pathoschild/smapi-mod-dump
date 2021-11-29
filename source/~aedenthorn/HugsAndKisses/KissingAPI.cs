/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using StardewValley;

namespace HugsAndKisses
{
    public class KissingAPI
    {
        public void PlayerNPCKiss(Farmer player, NPC npc)
        {
            Kissing.PlayerNPCKiss(player, npc);
        }
        public void NPCKiss(NPC kisser, NPC kissee)
        {
            Kissing.PerformEmbrace(kisser, kissee);
        }
        public SoundEffect GetKissSound()
        {
            return Kissing.kissEffect;
        }
        public SoundEffect GetHugSound()
        {
            return Kissing.hugEffect;
        }
        public int LastKissed(string name)
        {
            return Kissing.lastKissed.ContainsKey(name) ? Kissing.elapsedSeconds - Kissing.lastKissed[name] : -1;
        }
        public void ShowHeart(NPC npc)
        {
            Misc.ShowHeart(npc);
        }
        public void ShowSmiley(NPC npc)
        {
            Misc.ShowSmiley(npc);
        }
    }
}