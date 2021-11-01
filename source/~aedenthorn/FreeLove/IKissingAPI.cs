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

namespace FreeLove
{
    public interface IKissingAPI
    {
        void FarmerKiss(Farmer farmer, NPC npc);
        void NPCKiss(NPC kisser, NPC kissee);
        SoundEffect GetKissSound();
        bool IsKissing(string name);
    }
}