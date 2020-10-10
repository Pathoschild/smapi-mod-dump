/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMP
**
*************************************************/

using StardewValley;
using StardewValley.Buildings;
using System.IO;

namespace StardewValleyMP.Packets
{
    // Server -> Client
    // Synchronizes time for the clients.
    public class TimeSyncPacket : Packet
    {
        public int time;

        public TimeSyncPacket()
            : base(ID.TimeSync)
        {
            time = Game1.timeOfDay;
        }

        protected override void read(BinaryReader reader)
        {
            time = reader.ReadInt32();
        }

        protected override void write(BinaryWriter writer)
        {
            writer.Write(time);
        }

        public override void process( Client client )
        {
            Game1.timeOfDay = time;
            foreach ( GameLocation loc in Game1.locations )
            {
                if ( loc is Farm )
                {
                    Farm farm = loc as Farm;
                    foreach ( Building building in farm.buildings )
                    {
                        if (building.indoors == null) continue;
                        foreach ( NPC npc in building.indoors.characters )
                        {
                            npc.checkSchedule(Game1.timeOfDay);
                            npc.checkSchedule(Game1.timeOfDay - 50);
                            npc.checkSchedule(Game1.timeOfDay - 60);
                            npc.checkSchedule(Game1.timeOfDay - 70);
                            npc.checkSchedule(Game1.timeOfDay - 80);
                            npc.checkSchedule(Game1.timeOfDay - 90);
                        }
                    }
                }
                foreach (NPC npc in loc.characters)
                {
                    npc.checkSchedule(Game1.timeOfDay);
                    npc.checkSchedule(Game1.timeOfDay - 50);
                    npc.checkSchedule(Game1.timeOfDay - 60);
                    npc.checkSchedule(Game1.timeOfDay - 70);
                    npc.checkSchedule(Game1.timeOfDay - 80);
                    npc.checkSchedule(Game1.timeOfDay - 90);
                }
            }
        }

        public override string ToString()
        {
            return base.ToString() + " " + time;
        }
    }
}
