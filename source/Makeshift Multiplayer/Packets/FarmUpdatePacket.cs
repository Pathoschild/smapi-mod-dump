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
using System.IO;

namespace StardewValleyMP.Packets
{
    // Client <-> Server
    // Tell everyone the farm has changed.
    public class FarmUpdatePacket : Packet
    {
        public string name;
        public int hay;

        public FarmUpdatePacket()
            : base(ID.FarmUpdate)
        {
        }

        public FarmUpdatePacket(Farm loc)
            : this()
        {
            name = loc.name;
            hay = loc.piecesOfHay;
        }

        protected override void read(BinaryReader reader)
        {
            name = reader.ReadString();
            hay = reader.ReadInt32();
        }

        protected override void write(BinaryWriter writer)
        {
            writer.Write(name);
            writer.Write(hay);
        }

        public override void process(Client client)
        {
            process();
        }

        public override void process(Server server, Server.Client client)
        {
            process();
            server.broadcast(this, client.id);
        }

        private void process()
        {
            Farm farm = Game1.getLocationFromName(name) as Farm;
            if (farm == null || !Multiplayer.locations.ContainsKey(name)) return;

            farm.piecesOfHay = hay;
            Multiplayer.locations[name].prevFarmHay = hay;
        }

        public override string ToString()
        {
            return base.ToString() + " " + name + " " + hay;
        }
    }
}
