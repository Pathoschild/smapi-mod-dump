/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

using StardewValley;

namespace NPCTPHere.TP
{
    public class NPCData
    {
        public NPC npc { get; }

        public string locationName { get; }

        public int tileX { get; }
        
        public int tileY { get; }

        public NPCData(NPC npc, string locationName,int tileX,int tileY)
        {
            this.npc = npc;
            this.locationName = locationName;
            this.tileX = tileX;
            this.tileY = tileY;
        }
    }
}