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